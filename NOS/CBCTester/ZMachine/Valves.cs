using COMMON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Machine
{
    public class TankValve
    {
        struct Para
        {
            public yi InSt ; //용액 인입밸브
            public yi OtSt ; //용액 배출밸브
            public yi VcAr ; //에어배큠 전환벨브

            //public xi Full ; //탱크 만땅센서.
        }

        Para tPara  ; 
        bool bBusy  ;

        public TankValve(yi _yIn , yi _yOut , yi _yVcAr)// , xi _xFull )
        {
            tPara.InSt = _yIn  ;
            tPara.OtSt = _yOut ;
            tPara.VcAr = _yVcAr ;
        }

        //박에서 타임아웃일때 사용.
        //public void TimeoutInit()
        //{
        //    ML.IO_SetY(tPara.InSt , false );
        //    ML.IO_SetY(tPara.OtSt , false );
        //
        //}

        public bool GetBusy()
        {
            if(ML.IO_GetY(tPara.InSt) || ML.IO_GetY(tPara.OtSt)) return true ;
            return false ;
        }

        public bool Fill(bool _bOn)
        {
            if(_bOn && GetBusy()) return false ;

            if(_bOn)
            {
                ML.IO_SetY(tPara.InSt , true );
                ML.IO_SetY(tPara.OtSt , false);
                ML.IO_SetY(tPara.VcAr , true );
            }
            else
            {
                ML.IO_SetY(tPara.InSt , false );
                ML.IO_SetY(tPara.OtSt , false );

                ML.IO_SetY(tPara.VcAr , false );
            }
            return true ;
        }

        public bool Supply(bool _bOn)
        {
            if(_bOn && GetBusy()) return false ;

            if(_bOn)
            {
                ML.IO_SetY(tPara.InSt , false);
                ML.IO_SetY(tPara.OtSt , true );
                ML.IO_SetY(tPara.VcAr , false);
            }
            else
            {
                ML.IO_SetY(tPara.InSt , false );
                ML.IO_SetY(tPara.OtSt , false );
                ML.IO_SetY(tPara.VcAr , false );
            }
            return true ;
        }
    }

    public class ChamberValve
    {
        struct TPara
        {
            //청소시에 쓰는 벨브.
            public yi             CP3_InSt   ; //CP3용액 인입밸브 모든 챔버 다있음. 이건 순수하게 청소용.
            public yi             CP2_InSt   ; //CP2용액 인입밸브 1,2번만 있고 나머지는 -1
            public yi             Air_InVt   ; //에어 인/벤트
                                 
            public yi             Out_VcOt   ; //챔버 아웃/배큠 벨브.
            public yi             Out_OtSt   ; //챔버 아웃쪽 핀치벨브. 2번챔버는 핀치벨브가 아니다.

            //검사전에 용액 채울때 쓰는거.
            //1,2번은 CP2 탱크를 공용으로 쓴다. 공용탱크 용액배출밸브는 클래스에서 판단 못함. 외부에서 열어줘야함.
            public TankValve      Tank       ; //메인 용액 탱크밸브. 현재챔버 독점 탱크들. 1번탱크는 없음. 
            public PumpID         eSylingeID ; //실린지 있는 타입. 4,5,6번 NR RET 4DS 나머지 없는애들은 -1
        }
        TPara Para  ;

        struct TStat
        {
            //public bool bBusy  ;
            public int  iStep  ;

            //public int  iTimeCP2  ;
            //public int  iTimeTank ;

        }
        TStat Stat ; 

        //공급할때 시간.
        CDelayTimer CP3Timer   = new CDelayTimer();
        CDelayTimer CP2Timer   = new CDelayTimer();
        CDelayTimer TankTimer  = new CDelayTimer();
        //CDelayTimer OutTimer   = new CDelayTimer();
        
        //검사끝나고 나머지 버리는 시간.
        CDelayTimer WasteTimer = new CDelayTimer();

        public ChamberValve(yi _yCP3_InSt , yi _yCP2_InSt , yi _yAir_InVt , yi _yOut_VcOt , yi _yOut_OtSt , TankValve _Tank , PumpID _eSylingeID)// , xi _xFull )
        {
            Para.CP3_InSt   = _yCP3_InSt ;
            Para.CP2_InSt   = _yCP2_InSt ;
            Para.Air_InVt   = _yAir_InVt ;
            Para.Out_VcOt   = _yOut_VcOt ;
            Para.Out_OtSt   = _yOut_OtSt ;

            Para.Tank       = _Tank       ;
            Para.eSylingeID = _eSylingeID ;
        }

        //시약 공급.bInit을 이용하여 2스텝으로 사용. 
        CDelayTimer tmFillChamber = new CDelayTimer();
        public bool FillChamber(int _iTimeCP2 , int _iTimeTank , int _iSylIncPos , int _iSylSpdCode , bool bInit= false)
        {
            bool bRet = true;

            if (bInit)
            {
                CP3Timer.Clear();
                CP2Timer.Clear();
                TankTimer.Clear();

                //통신 타입들 딜레이.
                tmFillChamber.Clear();

                //혹시 켜져 있으면 에어=>벤틸로 전환.
                if (Para.Air_InVt >= 0) ML.IO_SetY(Para.Air_InVt, false);
                ML.IO_SetY(Para.Out_VcOt, false);
                ML.IO_SetY(Para.Out_OtSt, false);

                //Stat.iTimeCP2 = _iTimeCP2;
                //Stat.iTimeTank = _iTimeTank;

                //어드레스 세팅되어 있고 시간이 0보다 많을때.
                //CP2쓰는놈들.
                if (Para.CP2_InSt >= 0 && _iTimeCP2 > 0)
                {
                    ML.IO_SetY(Para.CP2_InSt, true); //상위 탱크는 챔버스 업데이트 함수에서 켬. 여러군데서 사용해서 그럼 ;; 개짜증....                
                }

                //독점 탱크 가진놈들.
                if (Para.Tank != null && _iTimeTank > 0)
                {
                    if(!Para.Tank.Supply(true))bRet= false ;
                }

                //실린지 가지고 있는 놈들.
                if ((int)Para.eSylingeID >= 0 && _iSylIncPos > 0)
                {
                    if(!SEQ.SyringePump.PickupAndDispInc((int)Para.eSylingeID, VALVE_POS.Input, VALVE_POS.Output, _iSylIncPos, _iSylSpdCode)) bRet = false ;
                }
                return bRet ;
            }

            //통신 타입들 딜레이.
            if(!tmFillChamber.OnDelay(300))return false ;

            //CP2 쓰는 놈들.
            if(Para.CP2_InSt >= 0 && _iTimeCP2 > 0)
            {
                if(CP2Timer.OnDelay(_iTimeCP2))
                {
                    ML.IO_SetY(Para.CP2_InSt , false ); 
                }
                else
                {
                    bRet = false ;
                }
            }

            //독점 탱크 가진놈들.
            if(Para.Tank != null && _iTimeTank > 0)
            {
                if(CP2Timer.OnDelay(_iTimeTank))
                {
                    Para.Tank.Supply(false);
                }
                else
                {
                    bRet = false ;
                }
                //bRet &= Para.Tank.Supply(false);                
            }

            //독점 실린지 가진놈들.
            if((int)Para.eSylingeID >= 0)
            {
                if(!SEQ.SyringePump.GetBusy((int)Para.eSylingeID))
                {
                }
                else
                {
                    bRet = false ;
                }         
            }
            return bRet ;       
        }

        //밖에서 타임아웃시 사용.
        public void TimeoutInit()
        {
            if(Para.CP3_InSt >= 0) ML.IO_SetY(Para.CP3_InSt , false);
            if(Para.CP2_InSt >= 0) ML.IO_SetY(Para.CP2_InSt , false);
            if(Para.Air_InVt >= 0) ML.IO_SetY(Para.Air_InVt , false);
            if(Para.Out_VcOt >= 0) ML.IO_SetY(Para.Out_VcOt , false);
            if(Para.Out_OtSt >= 0) ML.IO_SetY(Para.Out_OtSt , false);

            if(Para.Tank != null) Para.Tank.Supply(false);
            //if(Para.eSylingeID>=0) SEQ.CHA.Pump.]
        }

        //시약 검사
        public void InspSupply(bool _bOn)
        {
            //bool bRet = true ;
            if(_bOn)
            {
                //OutTimer.Clear();
                if(Para.CP2_InSt >= 0)
                {
                    ML.IO_SetY(Para.CP2_InSt , false );
                }
                if(Para.Tank != null)
                {
                    Para.Tank.Supply(false);
                }

                if (Para.Air_InVt >= 0) ML.IO_SetY(Para.Air_InVt , true );//챔버 에어/벤트 에어로.
                ML.IO_SetY(Para.Out_VcOt , false);//챔버 배큠/아웃 아웃으로.
                ML.IO_SetY(Para.Out_OtSt , true );//챔버 아웃/스탑 아웃으로.
            }
            else
            {
                if (Para.Air_InVt >= 0) ML.IO_SetY(Para.Air_InVt, false);//챔버 에어/벤트 에어로.
                ML.IO_SetY(Para.Out_VcOt, false);//챔버 배큠/아웃 아웃으로.
                ML.IO_SetY(Para.Out_OtSt, false);//챔버 아웃/스탑 아웃으로.
            }

            
        }

        //시약 버리기
        public void WasteOut(bool _bOn)
        {
            if (Para.CP2_InSt >= 0)
            {
                ML.IO_SetY(Para.CP2_InSt, false);
            }
            if (Para.Tank != null)
            {
                Para.Tank.Supply(false);
            }

            ML.IO_SetY(Para.CP3_InSt, false);
            ML.IO_SetY(Para.Out_OtSt, false);//챔버 아웃/스탑 아웃으로.

            //ML.IO_SetY(Para.Air_InVt , _bOn );//챔버 에어/벤트 에어로.
            ML.IO_SetY(Para.Out_VcOt , _bOn );//챔버 배큠/아웃 배큠으로. 

        }

        //CP3으로 행구려고 채움.
        public void FillCP3(bool _bOn)
        {
            //bool bRet = true ;
            if(_bOn)
            {
                if (Para.Air_InVt >= 0) ML.IO_SetY(Para.Air_InVt , false);//챔버 에어/벤트 에어로.
                ML.IO_SetY(Para.Out_VcOt , false);//챔버 배큠/아웃 배큠으로.
                ML.IO_SetY(Para.Out_OtSt , false);//챔버 아웃/스탑 아웃으로.

                ML.IO_SetY(Para.CP3_InSt , true) ;

            }
            else
            {
                if (Para.Air_InVt >= 0) ML.IO_SetY(Para.Air_InVt, false);//챔버 에어/벤트 에어로.
                ML.IO_SetY(Para.Out_VcOt, false);//챔버 배큠/아웃 아웃으로.
                ML.IO_SetY(Para.Out_OtSt, false);//챔버 아웃/스탑 아웃으로.

                ML.IO_SetY(Para.CP3_InSt , false) ;
            }
        }
    }
}
