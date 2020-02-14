using System;
using COMMON;
using System.Runtime.CompilerServices;
using SML;

namespace Machine
{
    
    public class Chamber : Part
    {
        //헤더 및 생성자 파트기본적인것들.
        #region Base
        public struct SStat
        {
            public bool bWorkEnd   ;
            public bool bReqStop   ;
            public bool bNeedAir   ; //메인에어 필요함.

            //public bool bRqstFill  ; //혈액 주입후 2차 필링.

            //public bool bRqstFillChamber1 ;
            //public bool bRqstFillChamber2 ;
            //public bool bRqstFillChamber3 ;
            //public bool bRqstFillChamber4 ;
            //public bool bRqstFillChamber5 ;
            //public bool bRqstFillChamber6 ;

            //이건 열었다가 그냥 Stop을 안탈수도 있어 
            //오토런에서 포지션보고 판단하게 한다.
            //public bool bRqstStartClean    ; //클린벨브 열기.
            //public bool bRqstStopClean     ; //클린벨브 닫기.

            public void Clear()
            {
                bWorkEnd    = false ;
                bReqStop    = false ;
                bNeedAir    = false ;

                //bRqstFill   = false ;
            }
        };   

        public enum sc
        {
            Idle          = 0,
            FillTank         ,
            FillChamber      ,
            InspChamber      ,
            EmptyChamber     ,
            CleanChamber     ,
            MAX_SEQ_CYCLE
        };

        public struct SStep
        {
            public int iHome   ;
            public int iToStart;
            public sc  eSeq    ;
            public int iCycle  ;
            public int iToStop ;
            public sc  eLastSeq;
            
            //스페샬
            public int iCycleSubDC ;
            public int iCycleSubHgb;
            public int iCycleSubFcm;
            public void Clear()
            {
                iHome    = 0;
                iToStart = 0;
                eSeq     = sc.Idle;
                iCycle   = 0;
                iToStop  = 0;
                eLastSeq = sc.Idle;

                //스페샬
                iCycleSubDC = 0 ;
                iCycleSubHgb = 0 ;
                iCycleSubFcm = 0 ;
            }
        };

        protected String      m_sPartName;
        protected int         m_iPartId  ;
        //Timer.
        protected CDelayTimer m_tmMain   ;
        protected CDelayTimer m_tmCycle  ;
        protected CDelayTimer m_tmHome   ;
        protected CDelayTimer m_tmToStop ;
        protected CDelayTimer m_tmToStart;
        protected CDelayTimer m_tmDelay  ; 
        


        protected CDelayTimer m_tmCycleSubDC  ;
        protected CDelayTimer m_tmCycleSubHGB ;
        protected CDelayTimer m_tmCycleSubFCM ;

        protected CDelayTimer m_tmDelaySubDC  ;       
        protected CDelayTimer m_tmDelaySubHGB ;      
        protected CDelayTimer m_tmDelaySubFCM ;      

        public    SStat Stat;
        protected SStep Step, PreStep;

        protected double m_dLastIdxPos;
        protected string m_sCheckSafeMsg;

        public CTimer[] m_CycleTime;

        

        

        public TankValve    TankCP1  ;
        public TankValve    TankCP2  ;
        public TankValve    TankCP3  ;
        public TankValve    TankCSF  ;
        public TankValve    TankCSR  ;
        public TankValve    TankSULF ;
        public TankValve    TankFB   ;
        public TankValve    Tank4DL  ;
        public TankValve    TankRET  ;
        public TankValve    TankNR   ;
                                     
        public ChamberValve Cmbr1    ;
        public ChamberValve Cmbr2    ;
        public ChamberValve Cmbr3    ;
        public ChamberValve Cmbr4    ;
        public ChamberValve Cmbr5    ;
        public ChamberValve Cmbr6    ;


        public Chamber(int _iPartId = 0)
        {
            m_sPartName = this.GetType().Name;

            m_tmMain      = new CDelayTimer();
            m_tmCycle     = new CDelayTimer();
            m_tmHome      = new CDelayTimer();
            m_tmToStop    = new CDelayTimer();
            m_tmToStart   = new CDelayTimer();
            m_tmDelay     = new CDelayTimer();

            m_tmCycleSubDC  = new CDelayTimer();
            m_tmCycleSubHGB = new CDelayTimer();
            m_tmCycleSubFCM = new CDelayTimer();

            m_tmDelaySubDC  = new CDelayTimer();
            m_tmDelaySubHGB = new CDelayTimer();
            m_tmDelaySubFCM = new CDelayTimer();

            m_CycleTime   = new CTimer[(int)sc.MAX_SEQ_CYCLE];

            for(int i = 0 ; i < (int)sc.MAX_SEQ_CYCLE ; i++)
            {
                m_CycleTime [i]  = new CTimer();
            }

            m_iPartId = _iPartId;

            TankCP1  = new TankValve(yi.TankCP1_InSt  ,yi.TankCP1_OtSt  , yi.TankCP1_VcAr );
            TankCP2  = new TankValve(yi.TankCP2_InSt  ,yi.TankCP2_OtSt  , yi.TankCP2_VcAr );
            TankCP3  = new TankValve(yi.TankCP3_InSt  ,yi.TankCP3_OtSt  , yi.TankCP3_VcAr );
            TankCSF  = new TankValve(yi.TankCSF_InSt  ,yi.TankCSF_OtSt  , yi.TankCSF_VcAr );
            TankCSR  = new TankValve(yi.TankCSR_InSt  ,yi.TankCSR_OtSt  , yi.TankCSR_VcAr );
            TankSULF = new TankValve(yi.TankSULF_InSt ,yi.TankSULF_OtSt , yi.TankSULF_VcAr);
            TankFB   = new TankValve(yi.TankFB_InSt   ,yi.TankFB_OtSt   , yi.TankFB_VcAr  );
            Tank4DL  = new TankValve(yi.Tank4DL_InSt  ,yi.Tank4DL_OtSt  , yi.Tank4DL_VcAr );
            TankRET  = new TankValve(yi.TankRET_InSt  ,yi.TankRET_OtSt  , yi.TankRET_VcAr );
            TankNR   = new TankValve(yi.TankNR_InSt   ,yi.TankNR_OtSt   , yi.TankNR_VcAr  );
            
            //Cmbr1    = new ChamberValve(yi.Cmb1CP3_InSt , yi.Cmb1CP2_InSt , yi.Cmb1Air_InVt , yi.Cmb1Out_VcOt , yi.Cmb1Out_OtSt , null     , PumpID.NotUse) ;
            //Cmbr2    = new ChamberValve(yi.Cmb2CP3_InSt , yi.Cmb2CP2_InSt , yi.Cmb2Air_InVt , yi.Cmb2Out_VcOt , yi.Cmb2Out_OtSt , TankSULF , PumpID.NotUse) ;
            //Cmbr3    = new ChamberValve(yi.Cmb3CP3_InSt , (yi)(-1)        , yi.Cmb3Air_InVt , yi.Cmb3Out_VcOt , yi.Cmb3Out_OtSt , TankFB   , PumpID.NotUse) ;
            //Cmbr4    = new ChamberValve(yi.Cmb4CP3_InSt , (yi)(-1)        , yi.Cmb4Air_InVt , yi.Cmb4Out_VcOt , yi.Cmb4Out_OtSt , Tank4DL  , PumpID.FDS   ) ;
            //Cmbr5    = new ChamberValve(yi.Cmb5CP3_InSt , (yi)(-1)        , yi.Cmb5Air_InVt , yi.Cmb5Out_VcOt , yi.Cmb5Out_OtSt , TankRET  , PumpID.RET   ) ;
            //Cmbr6    = new ChamberValve(yi.Cmb6CP3_InSt , (yi)(-1)        , yi.Cmb6Air_InVt , yi.Cmb6Out_VcOt , yi.Cmb6Out_OtSt , TankNR   , PumpID.NR    ) ;

            Cmbr1    = new ChamberValve(yi.Cmb1CP3_InSt , yi.Cmb1CP2_InSt , (yi)(-1)        , yi.Cmb1Out_VcOt , yi.Cmb1Out_OtSt , null     , PumpID.NotUse) ;
            Cmbr2    = new ChamberValve(yi.Cmb2CP3_InSt , yi.Cmb2CP2_InSt , yi.Cmb2Air_InVt , yi.Cmb2Out_VcOt , yi.Cmb2Out_OtSt , TankSULF , PumpID.NotUse) ;
            Cmbr3    = new ChamberValve(yi.Cmb3CP3_InSt , (yi)(-1)        , (yi)(-1)        , yi.Cmb3Out_VcOt , yi.Cmb3Out_OtSt , TankFB   , PumpID.NotUse) ;
            Cmbr4    = new ChamberValve(yi.Cmb4CP3_InSt , (yi)(-1)        , (yi)(-1)        , yi.Cmb4Out_VcOt , yi.Cmb4Out_OtSt , Tank4DL  , PumpID.FDS   ) ;
            Cmbr5    = new ChamberValve(yi.Cmb5CP3_InSt , (yi)(-1)        , (yi)(-1)        , yi.Cmb5Out_VcOt , yi.Cmb5Out_OtSt , TankRET  , PumpID.RET   ) ;
            Cmbr6    = new ChamberValve(yi.Cmb6CP3_InSt , (yi)(-1)        , (yi)(-1)        , yi.Cmb6Out_VcOt , yi.Cmb6Out_OtSt , TankNR   , PumpID.NR    ) ;



            Reset();                  
        }
        protected double GetLastCmd(mi _iMotr)
        {
            double dLastIdxPos = 0.0;

            return dLastIdxPos;
        }
        protected void ResetTimer()
        {
            //Clear Timer.
            m_tmMain   .Clear();
            m_tmCycle  .Clear();
            m_tmHome   .Clear();
            m_tmToStop .Clear();
            m_tmToStart.Clear();
            m_tmDelay  .Clear();
        }

        public SStep GetStep() { return Step; }
        #endregion 

        //PartInterface 부분.
        //인터페이스 상속.====================================================================================================================
        #region Interface
        override public void Reset() //리셑 버튼 눌렀을때 타는 함수.
        {

            ResetTimer();

            Stat.Clear();
            Step.Clear();
            PreStep.Clear();
        }

        //Running Functions.
        CDelayTimer tmValve = new CDelayTimer();//벨브 그냥 켜져 있을때 과열 되면 파손 되서 타이머 걸고 끔.
        override public void Update()
        {
            //역류 방지용.
            if(!IO_GetX(xi.CP_1NotFull) && IO_GetY(yi.TankCP1_InSt )) IO_SetY(yi.TankCP1_InSt  , false);
            if(!IO_GetX(xi.CP_2NotFull) && IO_GetY(yi.TankCP2_InSt )) IO_SetY(yi.TankCP2_InSt  , false);
            if(!IO_GetX(xi.CP_3NotFull) && IO_GetY(yi.TankCP3_InSt )) IO_SetY(yi.TankCP3_InSt  , false);
            if(!IO_GetX(xi.CS_FNotFull) && IO_GetY(yi.TankCSF_InSt )) IO_SetY(yi.TankCSF_InSt  , false);
            if(!IO_GetX(xi.CS_RNotFull) && IO_GetY(yi.TankCSR_InSt )) IO_SetY(yi.TankCSR_InSt  , false);
            if(!IO_GetX(xi.SULFNotFull) && IO_GetY(yi.TankSULF_InSt)) IO_SetY(yi.TankSULF_InSt , false);
            if(!IO_GetX(xi.FBNotFull  ) && IO_GetY(yi.TankFB_InSt  )) IO_SetY(yi.TankFB_InSt   , false);
            if(!IO_GetX(xi.FDLNotFull ) && IO_GetY(yi.Tank4DL_InSt )) IO_SetY(yi.Tank4DL_InSt  , false);
            if(!IO_GetX(xi.RETNotFull ) && IO_GetY(yi.TankRET_InSt )) IO_SetY(yi.TankRET_InSt  , false);
            if(!IO_GetX(xi.NRNotFull  ) && IO_GetY(yi.TankNR_InSt  )) IO_SetY(yi.TankNR_InSt   , false);

            if(!IO_GetX(xi.FCMWasteNotFull) && IO_GetY(yi.FCMOut_OtSt)) IO_SetY(yi.FCMOut_OtSt , false);
            if(!IO_GetX(xi.WasteNotFull   ))
            {
                if(IO_GetY(yi.Cmb1Out_VcOt)) IO_SetY(yi.Cmb1Out_VcOt   , false);
                if(IO_GetY(yi.Cmb2Out_VcOt)) IO_SetY(yi.Cmb2Out_VcOt   , false);
                if(IO_GetY(yi.Cmb3Out_VcOt)) IO_SetY(yi.Cmb3Out_VcOt   , false);
                if(IO_GetY(yi.Cmb4Out_VcOt)) IO_SetY(yi.Cmb4Out_VcOt   , false);
                if(IO_GetY(yi.Cmb5Out_VcOt)) IO_SetY(yi.Cmb5Out_VcOt   , false);
                if(IO_GetY(yi.Cmb6Out_VcOt)) IO_SetY(yi.Cmb6Out_VcOt   , false);

                if(IO_GetY(yi.Dcc1Vcm_InSt)) IO_SetY(yi.Dcc1Vcm_InSt   , false);
                if(IO_GetY(yi.Dcc2Vcm_InSt)) IO_SetY(yi.Dcc2Vcm_InSt   , false);
                if(IO_GetY(yi.HGBOut_OtSt )) IO_SetY(yi.HGBOut_OtSt    , false);
                if(IO_GetY(yi.FCMWOut_OtSt)) IO_SetY(yi.FCMWOut_OtSt   , false);
                if(IO_GetY(yi.NidlOut_OtSt)) IO_SetY(yi.NidlOut_OtSt   , false);

                if(SEQ._bRun)
                {
                    ER_SetErr(ei.CHA_TankFull , "Waste 탱크가 가득찼습니다.");
                }
            }

            //장비가 에러든 뭐든 멈추고 메뉴얼동작중도 아니면 솔 보호를 위해 3분 있다가 솔 다 끈다.
            if (tmValve.OnDelay(GetValveBusy()&& MM.GetManNo()==mc.NoneCycle && SEQ._iStep == EN_SEQ_STEP.Idle , 2*60000) )
            {
                Log.ShowMessage("Warning" , "솔 보호를 위해 모든 솔을 Off합니다.");
                InitValve();
            }

            //이블럭은 차라리 그냥 무조건 챔버 1개씩 검사한다 치고 싸이클로 감.
            //업데이트 에서 하는것은 답없음.
            //if(!SEQ._bRun) return ;
            ////여기서는 벨브 자동화.... 탱크 하단 벨브중에 중복으로 쓰면서 사용 하는 것들이 있어서 한번쓰고 다시 잠그면 그사이에 다른곳에서 쓰기 시작하면 잠거서는 안됌.
            ////그래서 탱크 하단 공급 벨브 자동화.
            ////이것들 외엔 다들 단독사용.
            //bool bOpenTankCP1_OtSt  = IO_GetY(yi.FCMOut_OtSt ) ;
            //bool bOpenTankCP2_OtSt  = IO_GetY(yi.Dcc5CP2_InSt) || SEQ.Pump.GetBusy((int)PumpID.FCM) || SEQ.Pump.GetBusy((int)PumpID.DC) || IO_GetY(yi.Cmb1CP2_InSt) || IO_GetY(yi.Cmb2CP2_InSt);
            //bool bOpenTankCP3_OtSt  = IO_GetY(yi.NidlCP3_InSt) || IO_GetY(yi.ClenCP3_InSt)      || 
            //                          IO_GetY(yi.Cmb1CP3_InSt) || IO_GetY(yi.Cmb2CP3_InSt)      || IO_GetY(yi.Cmb3CP3_InSt)     || IO_GetY(yi.Cmb4CP3_InSt) || IO_GetY(yi.Cmb5CP3_InSt) || IO_GetY(yi.Cmb6CP3_InSt) ;
            //bool bOpenTankCSF_OtSt  = IO_GetY(yi.Dcc3CSf_InSt) ;
            //bool bOpenTankCSR_OtSt  = IO_GetY(yi.Dcc4CSR_InSt) ;
            ////bool bOpenTankSULF_OtSt =  ;
            //
            //
            ////자동 벨브. 1,2,3,4,5 번탱크 까지 쓰고 
            //TankCP1.Supply(bOpenTankCP1_OtSt);
            //TankCP2.Supply(bOpenTankCP2_OtSt);
            //TankCP3.Supply(bOpenTankCP3_OtSt);
            //TankCSF.Supply(bOpenTankCSF_OtSt);
            //TankCSR.Supply(bOpenTankCSR_OtSt);
            //
            //
            //
            //
            //
            //
            //
            //IO_SetY(yi.TankCP1_OtSt , bOpenTankCP1_OtSt); IO_SetY(yi.TankCP1_ArVc , bOpenTankCP1_OtSt);
            //IO_SetY(yi.TankCP2_OtSt , bOpenTankCP2_OtSt); IO_SetY(yi.TankCP2_ArVc , bOpenTankCP2_OtSt);
            //IO_SetY(yi.TankCP3_OtSt , bOpenTankCP3_OtSt); IO_SetY(yi.TankCP3_ArVc , bOpenTankCP3_OtSt);
            //IO_SetY(yi.TankCSF_OtSt , bOpenTankCSF_OtSt); IO_SetY(yi.TankCSF_ArVc , bOpenTankCSF_OtSt);
            //IO_SetY(yi.TankCSR_OtSt , bOpenTankCSR_OtSt); IO_SetY(yi.TankCSR_ArVc , bOpenTankCSR_OtSt);







            //아... 여기 어떻게 해야함..
            //계속 타게 되서 안됨.
            //if(IO_GetY(yi.TankCP1_OtSt) != bOpenTankCP1_OtSt) {  }
            //if(IO_GetY(yi.TankCP1_OtSt) != bOpenTankCP1_OtSt) {  }
            //if(IO_GetY(yi.TankCP1_OtSt) != bOpenTankCP1_OtSt) {  }
            //if(IO_GetY(yi.TankCP1_OtSt) != bOpenTankCP1_OtSt) {  }
            //if(IO_GetY(yi.TankCP1_OtSt) != bOpenTankCP1_OtSt) {  }
            //                                                  { IO_SetY(yi.TankSULF_ArVc, IO_GetY(yi.TankSULF_OtSt));                                       }
            //                                                  { IO_SetY(yi.TankFB_ArVc  , IO_GetY(yi.TankFB_OtSt  ));                                       }
            //                                                  { IO_SetY(yi.Tank4DL_ArVc , IO_GetY(yi.Tank4DL_OtSt ));                                       }
            //                                                  { IO_SetY(yi.TankRET_ArVc , IO_GetY(yi.TankRET_OtSt ));                                       }
            //                                                  { IO_SetY(yi.TankNR_ArVc  , IO_GetY(yi.TankNR_OtSt  ));                                       }
            //
            //

            /*
            bool IsFillingCP_1         = !IO_GetY(yi.TankCP1_InSt    ) ;
            bool IsFillingCP_2         = !IO_GetY(yi.TankCP2_InSt    ) ;
            bool IsFillingCP_3         = !IO_GetY(yi.TankCP3_InSt    ) ;
            bool IsFillingCS_F         = !IO_GetY(yi.TankCSF_InSt    ) ;
            bool IsFillingCS_R         = !IO_GetY(yi.TankCSR_InSt    ) ;
            bool IsFillingSULF         = !IO_GetY(yi.TankSULF_InSt   ) ;
            bool IsFillingFB           = !IO_GetY(yi.TankFB_InSt     ) ;
            bool IsFilling4DL          = !IO_GetY(yi.Tank4DL_InSt    ) ;
            bool IsFillingRET          = !IO_GetY(yi.TankRET_InSt    ) ;
            bool IsFillingNR           = !IO_GetY(yi.TankNR_InSt     ) ;

            bool IsDischargingCP_1     = !IO_GetY(yi.TankCP1_OtSt    ) ;
            bool IsDischargingCP_2     = !IO_GetY(yi.TankCP2_OtSt    ) ;
            bool IsDischargingCP_3     = !IO_GetY(yi.TankCP3_OtSt    ) ;
            bool IsDischargingCS_F     = !IO_GetY(yi.TankCSF_OtSt    ) ;
            bool IsDischargingCS_R     = !IO_GetY(yi.TankCSR_OtSt    ) ;
            bool IsDischargingSULF     = !IO_GetY(yi.TankSULF_OtSt   ) ;
            bool IsDischargingFB       = !IO_GetY(yi.TankFB_OtSt     ) ;
            bool IsDischarging4DL      = !IO_GetY(yi.Tank4DL_OtSt    ) ;
            bool IsDischargingRET      = !IO_GetY(yi.TankRET_OtSt    ) ;
            bool IsDischargingNR       = !IO_GetY(yi.TankNR_OtSt     ) ;

            bool bNeedFillCP_1         = !IO_GetX(xi.FullCP_1        ) && DM.ARAY[ri.CHA].CheckAllStat(cs.None);
            bool bNeedFillCP_2         = !IO_GetX(xi.FullCP_2        ) && DM.ARAY[ri.CHA].CheckAllStat(cs.None);
            bool bNeedFillCP_3         = !IO_GetX(xi.FullCP_3        ) && DM.ARAY[ri.CHA].CheckAllStat(cs.None);
            bool bNeedFillCS_F         = !IO_GetX(xi.FullCS_F        ) && DM.ARAY[ri.CHA].CheckAllStat(cs.None);
            bool bNeedFillCS_R         = !IO_GetX(xi.FullCS_R        ) && DM.ARAY[ri.CHA].CheckAllStat(cs.None);
            bool bNeedFillSULF         = !IO_GetX(xi.FullSULF        ) && DM.ARAY[ri.CHA].CheckAllStat(cs.None);
            bool bNeedFillFB           = !IO_GetX(xi.FullFB          ) && DM.ARAY[ri.CHA].CheckAllStat(cs.None);
            bool bNeedFill4DL          = !IO_GetX(xi.Full4DL         ) && DM.ARAY[ri.CHA].CheckAllStat(cs.None);
            bool bNeedFillRET          = !IO_GetX(xi.FullRET         ) && DM.ARAY[ri.CHA].CheckAllStat(cs.None);
            bool bNeedFillNR           = !IO_GetX(xi.FullNR          ) && DM.ARAY[ri.CHA].CheckAllStat(cs.None);

            //탱크 채울 차람 이면 켜고.
            //            bool bNeedAir = false ;
            if(!IsFillingCP_1 && !IsDischargingCP_1 && bNeedFillCP_1 ){IO_SetY(yi.TankCP1_InSt    ,true);  IO_GetY(yi.TankCP1_ArVc    ,false);}//bNeedAir = true ;
            if(!IsFillingCP_2 && !IsDischargingCP_2 && bNeedFillCP_2 ){IO_SetY(yi.TankCP2_InSt    ,true);  IO_GetY(yi.TankCP2_ArVc    ,false);}//bNeedAir = true ;
            if(!IsFillingCP_3 && !IsDischargingCP_3 && bNeedFillCP_3 ){IO_SetY(yi.TankCP3_InSt    ,true);  IO_GetY(yi.TankCP3_ArVc    ,false);}//bNeedAir = true ;
            if(!IsFillingCS_F && !IsDischargingCS_F && bNeedFillCS_F ){IO_SetY(yi.TankCSF_InSt    ,true);  IO_GetY(yi.TankCSF_ArVc    ,false);}//bNeedAir = true ;
            if(!IsFillingCS_R && !IsDischargingCS_R && bNeedFillCS_R ){IO_SetY(yi.TankCSR_InSt    ,true);  IO_GetY(yi.TankCSR_ArVc    ,false);}//bNeedAir = true ;
            if(!IsFillingSULF && !IsDischargingSULF && bNeedFillSULF ){IO_SetY(yi.TankSULF_InSt   ,true);  IO_GetY(yi.TankSULF_ArVc   ,false);}//bNeedAir = true ;
            if(!IsFillingFB   && !IsDischargingFB   && bNeedFillFB   ){IO_SetY(yi.TankFB_InSt     ,true);  IO_GetY(yi.TankFB_ArVc     ,false);}//bNeedAir = true ;
            if(!IsFilling4DL  && !IsDischarging4DL  && bNeedFill4DL  ){IO_SetY(yi.Tank4DL_InSt    ,true);  IO_GetY(yi.Tank4DL_ArVc    ,false);}//bNeedAir = true ;
            if(!IsFillingRET  && !IsDischargingRET  && bNeedFillRET  ){IO_SetY(yi.TankRET_InSt    ,true);  IO_GetY(yi.TankRET_ArVc    ,false);}//bNeedAir = true ;//Stat.bNeedAir = bNeedAir ;
            if(!IsFillingNR   && !IsDischargingNR   && bNeedFillNR   ){IO_SetY(yi.TankNR_InSt     ,true);  IO_GetY(yi.TankNR_ArVc     ,false);}//bNeedAir = true ;
            //탱크 다 찻음 끄고
            if(IsFillingCP_1     && !bNeedFillCP_1    ){IO_SetY(yi.TankCP1_InSt ,false); IO_GetY(yi.TankCP1_ArVc    ,true);}
            if(IsFillingCP_2     && !bNeedFillCP_2    ){IO_SetY(yi.TankCP2_InSt ,false); IO_GetY(yi.TankCP2_ArVc    ,true);}
            if(IsFillingCP_3     && !bNeedFillCP_3    ){IO_SetY(yi.TankCP3_InSt ,false); IO_GetY(yi.TankCP3_ArVc    ,true);}
            if(IsFillingCS_F     && !bNeedFillCS_F    ){IO_SetY(yi.TankCSF_InSt ,false); IO_GetY(yi.TankCSF_ArVc    ,true);}
            if(IsFillingCS_R     && !bNeedFillCS_R    ){IO_SetY(yi.TankCSR_InSt ,false); IO_GetY(yi.TankCSR_ArVc    ,true);}
            if(IsFillingSULF     && !bNeedFillSULF    ){IO_SetY(yi.TankSULF_InSt,false); IO_GetY(yi.TankSULF_ArVc   ,true);}
            if(IsFillingFB       && !bNeedFillFB      ){IO_SetY(yi.TankFB_InSt  ,false); IO_GetY(yi.TankFB_ArVc     ,true);}
            if(IsFilling4DL      && !bNeedFill4DL     ){IO_SetY(yi.Tank4DL_InSt ,false); IO_GetY(yi.Tank4DL_ArVc    ,true);}
            if(IsFillingRET      && !bNeedFillRET     ){IO_SetY(yi.TankRET_InSt ,false); IO_GetY(yi.TankRET_ArVc    ,true);}
            if(IsFillingNR       && !bNeedFillNR      ){IO_SetY(yi.TankNR_InSt  ,false); IO_GetY(yi.TankNR_ArVc     ,true);}



            */
        }

        override public bool ToStopCon() //스탑을 하기 위한 조건을 보는 함수.
        {
            Stat.bReqStop = true;
            //During the auto run, do not stop.
            if (Step.eSeq != sc.Idle) return false;

            Step.iToStop = 10;
            //Ok.
            return true;


        }
        override public bool ToStartCon() //스타트를 하기 위한 조건을 보는 함수.
        {
            Step.iToStart = 10;
            //Ok.
            return true;

        }
        override public bool ToStart() //스타트를 하기 위한 함수.
        {
            //Check Time Out.
            if (m_tmToStart.OnDelay(Step.iToStart != 0 && PreStep.iToStart != 0 && PreStep.iToStart == Step.iToStart && CheckStop(), 5000))
            {
                ER_SetErr(ei.ETC_ToStartTO, m_sPartName );
            }

            String sTemp = String.Format("Step.iToStart={0:00}", Step.iToStart);

            if (Step.iToStart != PreStep.iToStart)
            {
                Trace(sTemp);
            }

            PreStep.iToStart = Step.iToStart;

            //Move Home.
            switch (Step.iToStart)
            {
                default: 
                    Step.iToStart = 0;
                    return true;

                case 10:

                    Step.iToStart = 0;
                    return true;
                    
            }

        }
        override public bool ToStop() //스탑을 하기 위한 함수.
        {
            //Check Time Out.
            if (m_tmToStop.OnDelay(Step.iToStop != 0 && PreStep.iToStop != 0 && PreStep.iToStop == Step.iToStop && CheckStop(), 10000)) ER_SetErr(ei.ETC_ToStopTO, m_sPartName); //EM_SetErr(eiLDR_ToStartTO);

            String sTemp;
            sTemp = string.Format("Step.iToStop={0:00}", Step.iToStop);
            if (Step.iToStop != PreStep.iToStop)
            {
                Trace(sTemp);
            }

            PreStep.iToStop = Step.iToStop;

            Stat.bReqStop = false;

            //Move Home.
            switch (Step.iToStop)
            {
                default: 
                    Step.iToStop = 0;
                    return true;

                case 10:
                    Step.iToStop = 0;
                    return true;
            }


        }

        override public int GetHomeStep   () { return      Step.iHome    ; } override public int GetPreHomeStep   () { return PreStep.iHome    ; } override public void InitHomeStep () { Step.iHome  = 10; PreStep.iHome  = 0; }
        override public int GetToStartStep() { return      Step.iToStart ; } override public int GetPreToStartStep() { return PreStep.iToStart ; }
        override public int GetSeqStep    () { return (int)Step.eSeq     ; } override public int GetPreSeqStep    () { return (int)PreStep.eSeq     ; }
        override public int GetCycleStep  () { return      Step.iCycle   ; } override public int GetPreCycleStep  () { return      PreStep.iCycle   ; } override public void InitCycleStep() { Step.iCycle = 10; PreStep.iCycle = 0; Step.iCycleSubDC = 10; PreStep.iCycleSubDC = 0; Step.iCycleSubHgb = 10; PreStep.iCycleSubHgb = 0; Step.iCycleSubFcm = 10; PreStep.iCycleSubFcm = 0; }
        override public int GetToStopStep () { return      Step.iToStop  ; } override public int GetPreToStopStep () { return      PreStep.iToStop  ; }
        
        override public string GetCrntCycleName(         ) { return Step.eSeq.ToString();}
        override public String GetCycleName    (int _iSeq) { return ((sc)_iSeq).ToString(); }
        override public double GetCycleTime    (int _iSeq) { return m_CycleTime[_iSeq].Duration; }
        override public String GetPartName     (         ) { return m_sPartName; }

        override public int GetCycleMaxCnt() { return (int)sc.MAX_SEQ_CYCLE; }

        override public int  GetPartId(        ) { return m_iPartId; }
        override public void SetPartId(int _iId) { m_iPartId = _iId; }

        override public bool Autorun() //오토런닝시에 계속 타는 함수.
        {
            PreStep.eSeq = Step.eSeq;
            string sCycle = GetCrntCycleName() ;

            //Check Error & Decide Step.
            if (Step.eSeq == sc.Idle)
            {
                if (Stat.bReqStop) return false;

                //if (OM.CmnOptn.bAutoQc) { Stat.bWorkEnd = true; return true; }

                //사이클
                bool bNeedFillTank    = IO_GetX(xi.CP_1NotFull) || IO_GetX(xi.CP_2NotFull ) || IO_GetX(xi.CP_3NotFull) || 
                                        IO_GetX(xi.CS_FNotFull) || IO_GetX(xi.CS_RNotFull ) || IO_GetX(xi.SULFNotFull) || 
                                        (IO_GetX(xi.FBNotFull ) && !OM.CmnOptn.bNotUseFB  ) ||
                                        (IO_GetX(xi.FDLNotFull) && !OM.CmnOptn.bNotUse4DLS) ||
                                        (IO_GetX(xi.RETNotFull) && !OM.CmnOptn.bNotUseRet ) ||
                                        (IO_GetX(xi.NRNotFull ) && !OM.CmnOptn.bNotUseNr  ) ;

                bNeedFillTank = bNeedFillTank ; //&& !OM.CmnOptn.bNotUseInsp ;

                bool bCmbNone         = DM.ARAY[ri.CHA].IsExist(cs.None);
                bool bExistWork       = DM.ARAY[ri.PKR].IsExist(cs.Shake , cs.Barcode) || DM.ARAY[ri.SUT].IsExist(cs.Barcode, cs.Shake) || DM.ARAY[ri.SYR].IsExist(cs.Work);
                //Stat.bRqstFillChamber1 || Stat.bRqstFillChamber2 || Stat.bRqstFillChamber3 || Stat.bRqstFillChamber4 || Stat.bRqstFillChamber5 || Stat.bRqstFillChamber6 ;

                bool isFillTank       =  bNeedFillTank&& DM.ARAY[ri.CHA].CheckAllStat(cs.None); //센서풀 안된놈들만 골라서 한번에 채움.
                bool isFillChamber    = (bCmbNone && !bNeedFillTank && bExistWork) || DM.ARAY[ri.CHA].CheckAllStat(cs.Fill);                  //실린지에서 피 공급 하면서 넣어줘야 해서 1번에 1챔버씩 넣음.
                bool isInspChamber    = !DM.ARAY[ri.CHA].IsExist(cs.Fill) && DM.ARAY[ri.CHA].CheckAllStat(cs.Work , cs.WorkEnd);         //일단 1,2,3번 같이 하고 6,4,5번 순서. 어차피 혈액 넣고 일정시간 대기타임이 있어야 되서 챔버에 혈액 다넣고 검사하는걸로.
                bool isEmptyChamber   =  DM.ARAY[ri.CHA].CheckAllStat(cs.WorkEnd);
                bool isCleanChamber   =  DM.ARAY[ri.CHA].CheckAllStat(cs.Clean  );
                bool isWorkEnd        =  DM.ARAY[ri.CHA].CheckAllStat(cs.None   );

                //this.ToString() + 이거 붙이기




                if (ER_IsErr())
                {
                    return false;
                }
                //Normal Decide Step.
                     if (isEmptyChamber  ) { DM.ARAY[ri.CHA].Trace(m_sPartName); Step.eSeq = sc.EmptyChamber ; }
                else if (isCleanChamber  ) { DM.ARAY[ri.CHA].Trace(m_sPartName); Step.eSeq = sc.CleanChamber ; }
                else if (isInspChamber   ) { DM.ARAY[ri.CHA].Trace(m_sPartName); Step.eSeq = sc.InspChamber  ; }
                else if (isFillChamber   ) { DM.ARAY[ri.CHA].Trace(m_sPartName); Step.eSeq = sc.FillChamber  ; }
                else if (isFillTank      ) { DM.ARAY[ri.CHA].Trace(m_sPartName); Step.eSeq = sc.FillTank     ; }
                else if (isWorkEnd       ) { Stat.bWorkEnd = true; return true; }
                Stat.bWorkEnd = false;

                if(Step.eSeq != sc.Idle){
                    Trace(Step.eSeq.ToString() +" Start");
                    Log.TraceListView(Step.eSeq.ToString() + " 동작 시작");
                    InitCycleStep();
                    m_CycleTime[(int)Step.eSeq].Start();
                }
            }
            
            //Cycle.
            Step.eLastSeq = Step.eSeq;
            switch (Step.eSeq)
            {
                default               : Trace("default End"); Step.eSeq = sc.Idle; return false; 
                case (sc.Idle        ):                            return false; 
                case (sc.EmptyChamber): if (!CycleEmptyChamber ()) return false; break;
                case (sc.CleanChamber): if (!CycleCleanChamber ()) return false; break;
                case (sc.InspChamber ): if (!CycleInspChamber  ()) return false; break;
                case (sc.FillChamber ): if (!CycleFillChamber  ()) return false; break;
                case (sc.FillTank    ): if (!CycleFillTank     ()) return false; break;
                
            }
            Trace(sCycle+" End");
            Log.TraceListView(sCycle + " 동작 종료"); 
            m_CycleTime[(int)Step.eSeq].End(); 
            Step.eSeq = sc.Idle;
            return false ;
        }
        //인터페이스 상속 끝.==================================================
        #endregion        

        public void InitValve()
        {
            for(int i = (int)yi.TankCP1_InSt ; i < (int)yi.Sol76 ; i++ )
            {
                IO_SetY(i , false );
            }
        }

        public bool GetValveBusy()
        {
            for (int i = (int)yi.TankCP1_InSt; i < (int)yi.Sol76; i++)
            {
                if(IO_GetY(i)) return true ;
            }
            return false ;
        }

        //위에 부터 작업.
        //인덱스 데이터에서 작업해야 되는 컬로우를 뽑아서 리턴.
        public bool FindChip(int _iId , out int c, out int r, cs _iChip=cs.RetFail) 
        {
            c=0 ; r=0 ;
            _iId = 0;
            
            return DM.ARAY[_iId].FindLastColFrstRow(ref c, ref r , _iChip);
        }

        public bool CycleHome()
        {
            String sTemp ;
            if (m_tmCycle.OnDelay(Step.iHome != 0 && Step.iHome == PreStep.iHome && CheckStop() /*&&!OM.MstOptn.bDebugMode*/, 10000 )) {
                sTemp = string.Format("Home Step.iHome={0:00}", Step.iHome);
                sTemp  = m_sPartName + sTemp ;
                ER_SetErr(ei.ETC_HomeTO ,sTemp);
                Trace(sTemp);
                InitValve();
                return true;
            }
            
            if(Step.iHome != PreStep.iHome) {
                sTemp = string.Format("Home Step.iHome={0:00}", Step.iHome);
                Trace(sTemp);
            }
            
            PreStep.iHome = Step.iHome ;
            
            if (Stat.bReqStop)
            {
                //return true ;
            }

            switch (Step.iHome) {

                default: 
                    sTemp = string.Format("Cycle Default Clear Home Step.iCycle={0:00}", Step.iHome);
                    InitValve();
                    return true ;

                case 10:
                    InitValve();

                    IO_SetY(yi.TankCP2_OtSt, true);

                    SEQ.SyringePump.YR((int)PumpID.Blood);
                    SEQ.SyringePump.YR((int)PumpID.DC   );
                    SEQ.SyringePump.YR((int)PumpID.FCM  );
                    SEQ.SyringePump.YR((int)PumpID.FDS  );
                    SEQ.SyringePump.YR((int)PumpID.NR   );
                    SEQ.SyringePump.YR((int)PumpID.RET  );

                    
                    
                    m_tmDelay.Clear();

                    Step.iHome++;
                    return false;

                case 11: 
                    if(!m_tmCycle.OnDelay(100))return false ;

                    if(SEQ.SyringePump.GetBusy((int)PumpID.Blood)) return false ;
                    if(SEQ.SyringePump.GetBusy((int)PumpID.DC   )) return false ;
                    if(SEQ.SyringePump.GetBusy((int)PumpID.FCM  )) return false ;
                    if(SEQ.SyringePump.GetBusy((int)PumpID.FDS  )) return false ;
                    if(SEQ.SyringePump.GetBusy((int)PumpID.NR   )) return false ;
                    if(SEQ.SyringePump.GetBusy((int)PumpID.RET  )) return false ;

                    IO_SetY(yi.TankCP2_OtSt, false);

                    Step.iHome = 0;
                    return true;
            }
        }

        public bool CycleFillTank()
        {
            string sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 300000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                ER_SetErr(ei.ETC_CycleTO, "시약통을 확인 하세요.");
                
                InitValve();

                Trace(sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                Trace(sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                TankCP1 .Fill(false) ;
                TankCP2 .Fill(false) ;
                TankCP3 .Fill(false) ;
                TankCSF .Fill(false) ;
                TankCSR .Fill(false) ;
                TankSULF.Fill(false) ;
                TankFB  .Fill(false) ;
                Tank4DL .Fill(false) ;
                TankRET .Fill(false) ;
                TankNR  .Fill(false) ;

                IO_SetY(yi.WastAir_InVc, false);
                IO_SetY(yi.WastOut_OtSt, false);

                return true ;
            }

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    InitValve();
                    return true;

                case 10:
                    InitValve();

                    if(IO_GetX(xi.CP_1NotFull))TankCP1 .Fill(true) ;
                    if(IO_GetX(xi.CP_2NotFull))TankCP2 .Fill(true) ;
                    if(IO_GetX(xi.CP_3NotFull))TankCP3 .Fill(true) ;
                    if(IO_GetX(xi.CS_FNotFull))TankCSF .Fill(true) ;
                    if(IO_GetX(xi.CS_RNotFull))TankCSR .Fill(true) ;
                    if(IO_GetX(xi.SULFNotFull))TankSULF.Fill(true) ;
                    if(IO_GetX(xi.FBNotFull  )&& !OM.CmnOptn.bNotUseFB  ) TankFB  .Fill(true) ;
                    if(IO_GetX(xi.FDLNotFull )&& !OM.CmnOptn.bNotUse4DLS) Tank4DL .Fill(true) ;
                    if(IO_GetX(xi.RETNotFull )&& !OM.CmnOptn.bNotUseRet ) TankRET .Fill(true) ;
                    if(IO_GetX(xi.NRNotFull  )&& !OM.CmnOptn.bNotUseNr  ) TankNR  .Fill(true) ;

                    //IO_SetY(yi.WastAir_InVc, true);
                    //IO_SetY(yi.WastOut_OtSt, true);

                    Step.iCycle++;
                    return false ;

                case 11:
                    //찰랑거림때문에 혹시나 싶어. 다시켬.
                    if(IO_GetX(xi.CP_1NotFull) && !TankCP1 .GetBusy())TankCP1 .Fill(true) ;//return false ;
                    if(IO_GetX(xi.CP_2NotFull) && !TankCP2 .GetBusy())TankCP2 .Fill(true) ;//return false ;
                    if(IO_GetX(xi.CP_3NotFull) && !TankCP3 .GetBusy())TankCP3 .Fill(true) ;//return false ;
                    if(IO_GetX(xi.CS_FNotFull) && !TankCSF .GetBusy())TankCSF .Fill(true) ;//return false ;
                    if(IO_GetX(xi.CS_RNotFull) && !TankCSR .GetBusy())TankCSR .Fill(true) ;//return false ;
                    if(IO_GetX(xi.SULFNotFull) && !TankSULF.GetBusy())TankSULF.Fill(true) ;//return false ;
                    if(IO_GetX(xi.FBNotFull  ) && !TankFB  .GetBusy()&& !OM.CmnOptn.bNotUseFB  )TankFB  .Fill(true) ;//return false ;
                    if(IO_GetX(xi.FDLNotFull ) && !Tank4DL .GetBusy()&& !OM.CmnOptn.bNotUse4DLS)Tank4DL .Fill(true) ;//return false ;
                    if(IO_GetX(xi.RETNotFull ) && !TankRET .GetBusy()&& !OM.CmnOptn.bNotUseRet )TankRET .Fill(true) ;//return false ;
                    if(IO_GetX(xi.NRNotFull  ) && !TankNR  .GetBusy()&& !OM.CmnOptn.bNotUseNr  )TankNR  .Fill(true) ;//return false ;

                    //만땅차면 끔.
                    if(!IO_GetX(xi.CP_1NotFull) && TankCP1 .GetBusy())TankCP1 .Fill(false) ;
                    if(!IO_GetX(xi.CP_2NotFull) && TankCP2 .GetBusy())TankCP2 .Fill(false) ;
                    if(!IO_GetX(xi.CP_3NotFull) && TankCP3 .GetBusy())TankCP3 .Fill(false) ;
                    if(!IO_GetX(xi.CS_FNotFull) && TankCSF .GetBusy())TankCSF .Fill(false) ;
                    if(!IO_GetX(xi.CS_RNotFull) && TankCSR .GetBusy())TankCSR .Fill(false) ;
                    if(!IO_GetX(xi.SULFNotFull) && TankSULF.GetBusy())TankSULF.Fill(false) ;
                    if(!IO_GetX(xi.FBNotFull  ) && TankFB  .GetBusy())TankFB  .Fill(false) ;
                    if(!IO_GetX(xi.FDLNotFull ) && Tank4DL .GetBusy())Tank4DL .Fill(false) ;
                    if(!IO_GetX(xi.RETNotFull ) && TankRET .GetBusy())TankRET .Fill(false) ;
                    if(!IO_GetX(xi.NRNotFull  ) && TankNR  .GetBusy())TankNR  .Fill(false) ;
                    //여기 부터 체크

                    if(IO_GetX(xi.CP_1NotFull))return false ;
                    if(IO_GetX(xi.CP_2NotFull))return false ;
                    if(IO_GetX(xi.CP_3NotFull))return false ;
                    if(IO_GetX(xi.CS_FNotFull))return false ;
                    if(IO_GetX(xi.CS_RNotFull))return false ;
                    if(IO_GetX(xi.SULFNotFull))return false ;
                    if(IO_GetX(xi.FBNotFull  )&& !OM.CmnOptn.bNotUseFB  )return false ;
                    if(IO_GetX(xi.FDLNotFull )&& !OM.CmnOptn.bNotUse4DLS)return false ;
                    if(IO_GetX(xi.RETNotFull )&& !OM.CmnOptn.bNotUseRet )return false ;
                    if(IO_GetX(xi.NRNotFull  )&& !OM.CmnOptn.bNotUseNr  )return false ;

                    //IO_SetY(yi.WastAir_InVc, false);
                    //IO_SetY(yi.WastOut_OtSt, false);


                    Step.iCycle=0;
                    return true ;

            }
        }


        public int iManFillCmbID = 0 ;
        bool bRqstFillChamber1 = false ;
        bool bRqstFillChamber2 = false ;
        bool bRqstFillChamber3 = false ;
        bool bRqstFillChamber4 = false ;
        bool bRqstFillChamber5 = false ;
        bool bRqstFillChamber6 = false ;
        double dRatio = 0.0;
        public bool CycleFillChamber()
        {
            string sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 10000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                ER_SetErr(ei.ETC_CycleTO, sTemp);

                InitValve();
                
                Trace(sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                Trace(sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //return true ;
            }

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    InitValve();
                    return true ;

                case 10:
                    InitValve();

                    if(SEQ._iSeqStat == EN_SEQ_STAT.Manual)//여기 폼디바이스셋에서 누르면 병신짓 하니 기억하자.
                    {
                        bRqstFillChamber1 = false ;
                        bRqstFillChamber2 = false ;
                        bRqstFillChamber3 = false ;
                        bRqstFillChamber4 = false ;
                        bRqstFillChamber5 = false ;
                        bRqstFillChamber6 = false ;
                        if (iManFillCmbID == 0)
                        {
                            bRqstFillChamber1 = true ;
                            bRqstFillChamber2 = true ;
                            bRqstFillChamber3 = true ;
                            bRqstFillChamber4 = true ;
                            bRqstFillChamber5 = true ;
                            bRqstFillChamber6 = true ;
                        }
                        else if(iManFillCmbID == 1)
                        {
                            bRqstFillChamber1 = true ;
                        }
                        else if(iManFillCmbID == 2)
                        {
                            bRqstFillChamber2 = true ;
                        }
                        else if(iManFillCmbID == 3)
                        {
                            bRqstFillChamber3 = true ;
                        }
                        else if(iManFillCmbID == 4)
                        {
                            bRqstFillChamber4 = true ;
                        }
                        else if(iManFillCmbID == 5)
                        {
                            bRqstFillChamber5 = true ;
                        }
                        else if(iManFillCmbID == 6)
                        {
                            bRqstFillChamber6 = true ;
                        }
                    }
                    else if(DM.ARAY[ri.CHA].IsExist(cs.Fill))//2차 필링.
                    {
                        //if(OM.CmnOptn.bNotUseInsp)
                        //{
                        //    DM.ARAY[ri.CHA].SetStat(0,0,cs.Work);
                        //    DM.ARAY[ri.CHA].SetStat(1,0,cs.Work);
                        //    DM.ARAY[ri.CHA].SetStat(2,0,cs.Work);
                        //    DM.ARAY[ri.CHA].SetStat(3,0,cs.Work);
                        //    DM.ARAY[ri.CHA].SetStat(4,0,cs.Work);
                        //    DM.ARAY[ri.CHA].SetStat(5,0,cs.Work);
                        //    Step.iCycle=0;
                        //    return true ;
                        //}
                        bRqstFillChamber1 =  DM.ARAY[ri.CHA].CheckStat(0,0,cs.Fill);//Stat.bRqstFillChamber1 ;
                        bRqstFillChamber2 =  DM.ARAY[ri.CHA].CheckStat(1,0,cs.Fill);//Stat.bRqstFillChamber2 ;
                        bRqstFillChamber3 =  !OM.CmnOptn.bNotUseFB  && DM.ARAY[ri.CHA].CheckStat(2,0,cs.Fill);//Stat.bRqstFillChamber3 ;
                        bRqstFillChamber4 =  !OM.CmnOptn.bNotUse4DLS&& DM.ARAY[ri.CHA].CheckStat(3,0,cs.Fill);//Stat.bRqstFillChamber4 ;
                        bRqstFillChamber5 =  !OM.CmnOptn.bNotUseRet && DM.ARAY[ri.CHA].CheckStat(4,0,cs.Fill);//Stat.bRqstFillChamber5 ;
                        bRqstFillChamber6 =  !OM.CmnOptn.bNotUseNr  && DM.ARAY[ri.CHA].CheckStat(5,0,cs.Fill);//Stat.bRqstFillChamber6 ;
                        dRatio = 0.7;
                    }
                    else //1차 필링.
                    {
                        //if(OM.CmnOptn.bNotUseInsp)
                        //{
                        //    DM.ARAY[ri.CHA].SetStat(0,0,cs.Ready);
                        //    DM.ARAY[ri.CHA].SetStat(1,0,cs.Ready);
                        //    DM.ARAY[ri.CHA].SetStat(2,0,cs.Ready);
                        //    DM.ARAY[ri.CHA].SetStat(3,0,cs.Ready);
                        //    DM.ARAY[ri.CHA].SetStat(4,0,cs.Ready);
                        //    DM.ARAY[ri.CHA].SetStat(5,0,cs.Ready);
                        //    Step.iCycle=0;
                        //    return true ;
                        //}
                        bRqstFillChamber1 = DM.ARAY[ri.CHA].CheckStat(0,0,cs.None);//Stat.bRqstFillChamber1 ;
                        bRqstFillChamber2 = DM.ARAY[ri.CHA].CheckStat(1,0,cs.None);//Stat.bRqstFillChamber2 ;
                        bRqstFillChamber3 = !OM.CmnOptn.bNotUseFB  && DM.ARAY[ri.CHA].CheckStat(2,0,cs.None);//Stat.bRqstFillChamber3 ;
                        bRqstFillChamber4 = !OM.CmnOptn.bNotUse4DLS&& DM.ARAY[ri.CHA].CheckStat(3,0,cs.None);//Stat.bRqstFillChamber4 ;
                        bRqstFillChamber5 = !OM.CmnOptn.bNotUseRet && DM.ARAY[ri.CHA].CheckStat(4,0,cs.None);//Stat.bRqstFillChamber5 ;
                        bRqstFillChamber6 = !OM.CmnOptn.bNotUseNr  && DM.ARAY[ri.CHA].CheckStat(5,0,cs.None);//Stat.bRqstFillChamber6 ;
                        dRatio = 0.3;

                    }

                    ////여러가지 이점 때문에 따로 버퍼에 넣어놓고 바로 false 시킴.
                    //Stat.bRqstFillChamber1 = false ;
                    //Stat.bRqstFillChamber2 = false ;
                    //Stat.bRqstFillChamber3 = false ;
                    //Stat.bRqstFillChamber4 = false ;
                    //Stat.bRqstFillChamber5 = false ;
                    //Stat.bRqstFillChamber6 = false ;

                    TankCP2.Supply(true);

                    //피 들어가기 전에 1차 피들어가고 난후 2차 2번에 하기때문에 2로 나눔.
                    if(bRqstFillChamber1) Cmbr1.FillChamber((int)(OM.DevInfo.iCmb1Cp2Time * dRatio), (int)0                                  , (int)0                                , (int)0                                   , true);
                    if(bRqstFillChamber2) Cmbr2.FillChamber((int)(OM.DevInfo.iCmb2Cp2Time * dRatio), (int)(OM.DevInfo.iCmb2TankTime * dRatio), (int)0                                , (int)0                                   , true);
                    if(bRqstFillChamber3) Cmbr3.FillChamber((int)0                                 , (int)(OM.DevInfo.iCmb3TankTime * dRatio), (int)(OM.DevInfo.iCmb3SylnPos*dRatio) , (int)(OM.DevInfo.iCmb3SylSpdCode*dRatio) , true);
                    if(bRqstFillChamber4) Cmbr4.FillChamber((int)0                                 , (int)(OM.DevInfo.iCmb4TankTime * dRatio), (int)(OM.DevInfo.iCmb4SylnPos*dRatio) , (int)(OM.DevInfo.iCmb4SylSpdCode*dRatio) , true);
                    if(bRqstFillChamber5) Cmbr5.FillChamber((int)0                                 , (int)(OM.DevInfo.iCmb5TankTime * dRatio), (int)(OM.DevInfo.iCmb5SylnPos*dRatio) , (int)(OM.DevInfo.iCmb5SylSpdCode*dRatio) , true);
                    if(bRqstFillChamber6) Cmbr6.FillChamber((int)0                                 , (int)(OM.DevInfo.iCmb6TankTime * dRatio), (int)(OM.DevInfo.iCmb6SylnPos*dRatio) , (int)(OM.DevInfo.iCmb6SylSpdCode*dRatio) , true);


                    //if (bRqstFillChamber1) IO_SetY(yi.Cmb1CP2_InSt, true);
                    //if (bRqstFillChamber2) IO_SetY(yi.Cmb2CP2_InSt, true);
                    //if(bRqstFillChamber3)
                    //if(bRqstFillChamber4)
                    //if(bRqstFillChamber5)
                    //if(bRqstFillChamber6)

                    Step.iCycle++;
                    return false ;

                case 11:
                    bool bRet1 = true ;
                    bool bRet2 = true ;
                    bool bRet3 = true ;
                    bool bRet4 = true ;
                    bool bRet5 = true ;
                    bool bRet6 = true ;

                    if(bRqstFillChamber1) bRet1 = Cmbr1.FillChamber((int)(OM.DevInfo.iCmb1Cp2Time * dRatio), (int)0                                  , (int)0                                , (int)0                                   );
                    if(bRqstFillChamber2) bRet2 = Cmbr2.FillChamber((int)(OM.DevInfo.iCmb2Cp2Time * dRatio), (int)(OM.DevInfo.iCmb2TankTime * dRatio), (int)0                                , (int)0                                   );
                    if(bRqstFillChamber3) bRet3 = Cmbr3.FillChamber((int)0                                 , (int)(OM.DevInfo.iCmb3TankTime * dRatio), (int)(OM.DevInfo.iCmb3SylnPos*dRatio) , (int)(OM.DevInfo.iCmb3SylSpdCode*dRatio) );
                    if(bRqstFillChamber4) bRet4 = Cmbr4.FillChamber((int)0                                 , (int)(OM.DevInfo.iCmb4TankTime * dRatio), (int)(OM.DevInfo.iCmb4SylnPos*dRatio) , (int)(OM.DevInfo.iCmb4SylSpdCode*dRatio) );
                    if(bRqstFillChamber5) bRet5 = Cmbr5.FillChamber((int)0                                 , (int)(OM.DevInfo.iCmb5TankTime * dRatio), (int)(OM.DevInfo.iCmb5SylnPos*dRatio) , (int)(OM.DevInfo.iCmb5SylSpdCode*dRatio) );
                    if(bRqstFillChamber6) bRet6 = Cmbr6.FillChamber((int)0                                 , (int)(OM.DevInfo.iCmb6TankTime * dRatio), (int)(OM.DevInfo.iCmb6SylnPos*dRatio) , (int)(OM.DevInfo.iCmb6SylSpdCode*dRatio) );

                    //if(!bRet1)IO_SetY(yi.Cmb1CP2_InSt, false);
                    //if(!bRet2)IO_SetY(yi.Cmb2CP2_InSt, false);

                    if(!bRet1) return false ;
                    if(!bRet2) return false ;
                    if(!bRet3) return false ;
                    if(!bRet4) return false ;
                    if(!bRet5) return false ;
                    if(!bRet6) return false ;

                    TankCP2.Supply(false);

                    //1차 필링용.
                    if(/*bRqstFillChamber1 &&*/ DM.ARAY[ri.CHA].CheckStat(0, 0, cs.None)) DM.ARAY[ri.CHA].SetStat(0,0,cs.Ready);
                    if(/*bRqstFillChamber2 &&*/ DM.ARAY[ri.CHA].CheckStat(1, 0, cs.None)) DM.ARAY[ri.CHA].SetStat(1,0,cs.Ready);
                    if(/*bRqstFillChamber3 &&*/ DM.ARAY[ri.CHA].CheckStat(2, 0, cs.None)) DM.ARAY[ri.CHA].SetStat(2,0,cs.Ready);
                    if(/*bRqstFillChamber4 &&*/ DM.ARAY[ri.CHA].CheckStat(3, 0, cs.None)) DM.ARAY[ri.CHA].SetStat(3,0,cs.Ready);
                    if(/*bRqstFillChamber5 &&*/ DM.ARAY[ri.CHA].CheckStat(4, 0, cs.None)) DM.ARAY[ri.CHA].SetStat(4,0,cs.Ready);
                    if(/*bRqstFillChamber6 &&*/ DM.ARAY[ri.CHA].CheckStat(5, 0, cs.None)) DM.ARAY[ri.CHA].SetStat(5,0,cs.Ready);

                    //if(OM.CmnOptn.bNotUseFB  && DM.ARAY[ri.CHA].CheckStat(2, 0, cs.None)) DM.ARAY[ri.CHA].SetStat(2,0,cs.Ready);
                    //if(OM.CmnOptn.bNotUse4DLS&& DM.ARAY[ri.CHA].CheckStat(3, 0, cs.None)) DM.ARAY[ri.CHA].SetStat(3,0,cs.Ready);
                    //if(OM.CmnOptn.bNotUseRet && DM.ARAY[ri.CHA].CheckStat(4, 0, cs.None)) DM.ARAY[ri.CHA].SetStat(4,0,cs.Ready);
                    //if(OM.CmnOptn.bNotUseNr  && DM.ARAY[ri.CHA].CheckStat(5, 0, cs.None)) DM.ARAY[ri.CHA].SetStat(5,0,cs.Ready);


                    //2차 필링용.
                    if (/*bRqstFillChamber1 &&*/ DM.ARAY[ri.CHA].CheckStat(0, 0, cs.Fill)) DM.ARAY[ri.CHA].SetStat(0, 0, cs.Work);
                    if (/*bRqstFillChamber2 &&*/ DM.ARAY[ri.CHA].CheckStat(1, 0, cs.Fill)) DM.ARAY[ri.CHA].SetStat(1, 0, cs.Work);
                    if (/*bRqstFillChamber3 &&*/ DM.ARAY[ri.CHA].CheckStat(2, 0, cs.Fill)) DM.ARAY[ri.CHA].SetStat(2, 0, cs.Work);
                    if (/*bRqstFillChamber4 &&*/ DM.ARAY[ri.CHA].CheckStat(3, 0, cs.Fill)) DM.ARAY[ri.CHA].SetStat(3, 0, cs.Work);
                    if (/*bRqstFillChamber5 &&*/ DM.ARAY[ri.CHA].CheckStat(4, 0, cs.Fill)) DM.ARAY[ri.CHA].SetStat(4, 0, cs.Work);
                    if (/*bRqstFillChamber6 &&*/ DM.ARAY[ri.CHA].CheckStat(5, 0, cs.Fill)) DM.ARAY[ri.CHA].SetStat(5, 0, cs.Work);
                    //
                    //if (OM.CmnOptn.bNotUseFB  && DM.ARAY[ri.CHA].CheckStat(2, 0, cs.Fill)) DM.ARAY[ri.CHA].SetStat(2, 0, cs.Work);
                    //if (OM.CmnOptn.bNotUse4DLS&& DM.ARAY[ri.CHA].CheckStat(3, 0, cs.Fill)) DM.ARAY[ri.CHA].SetStat(3, 0, cs.Work);
                    //if (OM.CmnOptn.bNotUseRet && DM.ARAY[ri.CHA].CheckStat(4, 0, cs.Fill)) DM.ARAY[ri.CHA].SetStat(4, 0, cs.Work);
                    //if (OM.CmnOptn.bNotUseNr  && DM.ARAY[ri.CHA].CheckStat(5, 0, cs.Fill)) DM.ARAY[ri.CHA].SetStat(5, 0, cs.Work);

                    Step.iCycle=0;
                    return true ;
            }
        }

        Random Ran = new Random(); //헤모글로빈 검사안함 옵션시 얍삽이.
        int iRan = 0 ;
        int iWorkChamFcm = 0 ; //CycleInspFcm 에서 쓰는 작업해야 할 챔버.
        bool bNeedDc  = false ;
        bool bNeedHGB = false ;
        public bool CycleInspChamber()
        {
            string sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle!= 14 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 31000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                ER_SetErr(ei.ETC_CycleTO, sTemp);
                InitValve();
                IO_SetY(yi.CHA_FcmLaserOn, false);

                Trace(sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                Trace(sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //return true ;
            }


            //스페샬. 서브 사이클 타이밍 차트상 FCM3,4,5,6 랑 동시에 DC HGB를 같이 해야해서 넣음.
            //챔버 혈액투입후 검사가능시간.
            //1    15~30
            //2    10~60
            //3    14~30
            //4    30~60
            //5    30~60
            //6    15~30
            //* 타이밍 차트상으로 혈액 투입 순서는 2,1,4,5,6,3
            //* 검사순서는 1,2,6->3->4->5

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    InitValve();
                    IO_SetY(yi.CHA_FcmLaserOn, false);
                    return true ;

                case 10:
                    
                    //작업 종료후 마스킹.

                    if(DM.ARAY[ri.CHA].CheckAllStat(cs.Work)) 
                    {
                        InitValve();                        
                    }

                    

                    SEQ.FcmTester.SendReqStatus();

                    //이때쯤 혈액 남은거 버리느라 클린쪽에서 내려 가고 있어서 이거 하면 안됨.
                    //SEQ.SYR.MoveMotr(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove);
                    Step.iCycle++;
                    return false ;

                case 11:
                    //나중에 풀자.
                    if(!OM.CmnOptn.bIgnrFCMTester)
                    {
                        if (!SEQ.FcmTester.RcvdMsg) return false;
                        if (SEQ.FcmTester.State == TCPIP_NewOpticsFCM.EState.Running)
                        {
                            SEQ.FcmTester.SendReqStatus(); //10으로 보내지 말고 11에 머물게 해서 타임아웃 유도.
                            return false;
                        }
                        else if (SEQ.FcmTester.State == TCPIP_NewOpticsFCM.EState.Error)
                        {
                            ER_SetErr(ei.ETC_Communicate, "FCM Tester 에러상태");
                            return true;
                        }
                    }

                    //첫 검사시 시퀜스 확인 전송.
                    if (DM.ARAY[ri.CHA].CheckAllStat(cs.Work)) 
                    {
                        Log.TraceListView("NI-Start : "+OM.EqpStat.sBloodID);
                        SEQ.FcmTester.SendTestSeq(TCPIP_NewOpticsFCM.ETestSeq.Start, OM.EqpStat.sBloodID);
                        bFirstFcm = true ; //첫번째 FCM검사는 DC랑 겹치지 않기 위해 딜레이가 필요.
                    }

                    Step.iCycle++;
                    return false;

                case 12:
                    if(!OM.CmnOptn.bIgnrFCMTester && DM.ARAY[ri.CHA].CheckAllStat(cs.Work))
                    {
                        if (!SEQ.FcmTester.RcvdMsg) return false;                        
                    }

                    MoveCyl(ci.CHA_MixCoverOpCl , fb.Bwd);

                    

                    Step.iCycle++;
                    return false ;

                case 13:
                    if(!CL_Complete(ci.CHA_MixCoverOpCl , fb.Bwd))return false ;

                    //DC테스트.
                    if (MM.GetManNo() == mc.CHA_CycleInspDC)
                    {
                        //FCM DC공용 
                        TankCP2.Supply(true);
                        if (!CycleInspChamberSubDc())return false ;
                        TankCP2.Supply(false);
                        Step.iCycle= 0 ;
                        return true ;

                    }

                    if (OM.CmnOptn.bNotUseInsp) //검수용.============================================================================
                    {
                        Step.iCycle = 50;
                        return false;
                    }

                    bNeedDc  = false ;
                    bNeedHGB = false ;
                    iWorkChamFcm = 0 ;

                    if(DM.ARAY[ri.CHA].CheckStat(0,0,cs.Work)) { bNeedDc  = true ;  Step.iCycleSubDC  = 10; } //  OM.EqpStat.iInspProgress = 10 ;}//헤모글로빈  DC는 같이 함.
                    if(DM.ARAY[ri.CHA].CheckStat(1,0,cs.Work)) { bNeedHGB = true ;  Step.iCycleSubHgb = 10; } //  OM.EqpStat.iInspProgress = 10 ;}//헤모글로빈  DC는 같이 함.

                    //if(OM.CmnOptn.bNotUse4DLS)

                    //검사 무시 하는 애들 미리 마스킹.
                    if (DM.ARAY[ri.CHA].CheckStat(5, 0, cs.Work) && OM.CmnOptn.bNotUseNr  )DM.ARAY[ri.CHA].SetStat(5, 0, cs.WorkEnd);
                    if (DM.ARAY[ri.CHA].CheckStat(2, 0, cs.Work) && OM.CmnOptn.bNotUseFB  )DM.ARAY[ri.CHA].SetStat(2, 0, cs.WorkEnd);
                    if (DM.ARAY[ri.CHA].CheckStat(3, 0, cs.Work) && OM.CmnOptn.bNotUse4DLS)DM.ARAY[ri.CHA].SetStat(3, 0, cs.WorkEnd);
                    if (DM.ARAY[ri.CHA].CheckStat(4, 0, cs.Work) && OM.CmnOptn.bNotUseRet )DM.ARAY[ri.CHA].SetStat(4, 0, cs.WorkEnd);



                         if(DM.ARAY[ri.CHA].CheckStat(5,0,cs.Work) ) {iWorkChamFcm = 5 ; Step.iCycleSubFcm  = 10;}//Step.iCycle = 600;                          OM.EqpStat.iInspProgress = 20 ;
                    else if(DM.ARAY[ri.CHA].CheckStat(2,0,cs.Work) ) {iWorkChamFcm = 2 ; Step.iCycleSubFcm  = 10;}//Step.iCycle = 300;                          OM.EqpStat.iInspProgress = 30 ;
                    else if(DM.ARAY[ri.CHA].CheckStat(3,0,cs.Work) ) {iWorkChamFcm = 3 ; Step.iCycleSubFcm  = 10;}//Step.iCycle = 400;                          OM.EqpStat.iInspProgress = 40 ;
                    else if(DM.ARAY[ri.CHA].CheckStat(4,0,cs.Work) ) {iWorkChamFcm = 4 ; Step.iCycleSubFcm  = 10;}//Step.iCycle = 500;                          OM.EqpStat.iInspProgress = 50 ;
                    else                                             {iWorkChamFcm = 0 ; Step.iCycleSubFcm  = 10;}
                    if(MM.GetManNo() != mc.NoneCycle)OM.EqpStat.iInspProgress = 0 ;

                    if(bNeedDc || iWorkChamFcm != 0)
                    {
                        TankCP2.Supply(true);//FCM DC 공용으로 써서 밖에서 제어.
                    }

                    Step.iCycle++;
                    return false ;

                case 14://타임아웃 조건에서 뺌...
                    bool bRetDC  = true ;
                    bool bRetHgb = true ;
                    bool bRetFcm = true ;

                    

                    if(bNeedDc          ) bRetDC  = CycleInspChamberSubDc  ();
                    if(bNeedHGB         ) bRetHgb = CycleInspChamberSubHGB ();
                    if(iWorkChamFcm != 0) bRetFcm = CycleInspChamberSubFcm (iWorkChamFcm); //FCM 4개는 구조상 같이 못돌림

                    double dTemp = DM.ARAY[ri.CHA].GetCntStat(cs.WorkEnd)/6.0 ;
                    dTemp = dTemp * 50 ; //검사에서 프로그래스 바 50까지 할당.

                    OM.EqpStat.iInspProgress = (int)dTemp ;

                    if (!bRetDC || !bRetHgb || !bRetFcm)return false ;

                    TankCP2.Supply(false);//FCM DC 공용으로 써서 밖에서 제어.

                    //에러로 끝나던 정상으로 끝나던 일단 초기화.
                    InitValve();

                    Step.iCycle++;
                    return false ;

                case 15:
                    //마지막 검사였으면 검사끝 전송.
                    if (DM.ARAY[ri.CHA].CheckAllStat(cs.WorkEnd)) { SEQ.FcmTester.SendTestSeq(TCPIP_NewOpticsFCM.ETestSeq.End, ""); Log.TraceListView("NI-End"); }
                    Step.iCycle++;
                    return false;

                case 16:
                    if (DM.ARAY[ri.CHA].CheckAllStat(cs.WorkEnd) && !OM.CmnOptn.bIgnrFCMTester && !SEQ.FcmTester.RcvdMsg) return false;
                    Step.iCycle = 0;
                    return true;

                //================검수용.
                case 50:
                    ML.IO_SetY(yi.Cmb1Out_VcOt , true);
                    ML.IO_SetY(yi.Cmb2Out_VcOt , true);
                    ML.IO_SetY(yi.Cmb3Out_VcOt , !OM.CmnOptn.bNotUseFB  );
                    ML.IO_SetY(yi.Cmb4Out_VcOt , !OM.CmnOptn.bNotUse4DLS);
                    ML.IO_SetY(yi.Cmb5Out_VcOt , !OM.CmnOptn.bNotUseRet );
                    ML.IO_SetY(yi.Cmb6Out_VcOt , !OM.CmnOptn.bNotUseNr  );

                    Step.iCycle++;
                    return false;

                case 51:
                    //검사 시그널..
                    SEQ.FcmTester.SendTestSub(TCPIP_NewOpticsFCM.ETest.DC_TEST);
                    Log.TraceListView("NI-DC_TEST");

                    Step.iCycle++;
                    return false;

                case 52:
                    if (!OM.CmnOptn.bIgnrFCMTester && !SEQ.FcmTester.RcvdMsg) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 53:
                    if (!m_tmDelay.OnDelay(OM.CmnOptn.iVtlDCTime)) return false;

                    iRan = Ran.Next(10,100);
                    SEQ.FcmTester.SendHemog("12" + iRan.ToString());
                    Log.TraceListView("NI-HEMO-" + "12" + iRan.ToString());

                    //IO_SetY(yi.HGBOut_OtSt, true);

                    //m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 54:
                    if (!OM.CmnOptn.bIgnrFCMTester && !SEQ.FcmTester.RcvdMsg) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 55:
                    if (!m_tmDelay.OnDelay(100)) return false;

                    //검사 시그널..
                    if (OM.CmnOptn.bAutoQc) { SEQ.FcmTester.SendTestSub(TCPIP_NewOpticsFCM.ETest.QC_FCM_2_TEST); Log.TraceListView("NI-QC_FCM_2_TEST"); }
                    else                    { SEQ.FcmTester.SendTestSub(TCPIP_NewOpticsFCM.ETest.FCM_2_TEST   ); Log.TraceListView("NI-FCM_2_TEST"); }


                    Step.iCycle++;
                    return false;

                case 56:
                    if (!OM.CmnOptn.bIgnrFCMTester && !SEQ.FcmTester.RcvdMsg) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 57:
                    if (!m_tmDelay.OnDelay(OM.CmnOptn.iVtlFCMTime)) return false;
                    //m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 58:
                    //if (!m_tmDelay.OnDelay(3000)) return false;

                    SEQ.FcmTester.SendTestSeq(TCPIP_NewOpticsFCM.ETestSeq.End, ""); 
                    Log.TraceListView("NI-End");
                    Step.iCycle++;
                    return false;

                case 59:
                    if (!OM.CmnOptn.bIgnrFCMTester && !SEQ.FcmTester.RcvdMsg) return false;
                    //여기까지 검사 패턴.===========================================

                    InitValve();

                    //웨이스트 비우기.
                    ML.IO_SetY(yi.WastAir_InVc , true);
                    ML.IO_SetY(yi.WastOut_OtSt , true);

                    if (OM.EqpStat.bWorkOneStop)//한자제만 하고 끄는 옵션시에 작업 종료 되면 끔.
                    {
                        OM.EqpStat.bWorkOneStop = false;
                        SEQ._bBtnStop = true;
                    }


                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 60:
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iWasteToExtTime /2))return false ;//메인웨이스트 비우기.

                    ML.IO_SetY(yi.WastAir_InVc, false);
                    ML.IO_SetY(yi.WastOut_OtSt, false);

                    DM.ARAY[ri.CHA].SetStat(cs.None);

                    Step.iCycle = 0;
                    return true;



            }
        }

        public bool CycleInspChamberSubDc()
        {
            string sTemp;
            if (m_tmCycleSubDC.OnDelay(Step.iCycleSubDC != 0 && Step.iCycleSubDC == PreStep.iCycleSubDC && CheckStop() && !OM.MstOptn.bDebugMode, 20000))
            {
                sTemp = string.Format("Time Out Step.iCycleSubDC={0:00}", Step.iCycleSubDC);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                ER_SetErr(ei.ETC_CycleTO, sTemp);
                //InitValve();
                //IO_SetY(yi.CHA_FcmLaserOn, false);
                Cmbr1.InspSupply(false); //챔버 닫고.
                TankCSF.Supply(false); //CSF 탱크 닫고.
                //FCM DC공용 TankCP2.Supply(false); //CP2탱크열고.
                TankCSR.Supply(false);
                IO_SetY(yi.Dcc1Vcm_InSt, false);
                IO_SetY(yi.Dcc3CSf_InSt, false);
                IO_SetY(yi.Dcc2Vcm_InSt, false);
                IO_SetY(yi.Dc1Air_InVt , false);
                IO_SetY(yi.WastAir_InVc, false);
                IO_SetY(yi.FCMWOut_OtSt, false);

                Trace(sTemp);
                return true;
            }

            if (Step.iCycleSubDC != PreStep.iCycleSubDC)
            {
                sTemp = string.Format("Cycle iCycleSubDC.iCycle={0:00}", Step.iCycleSubDC);
                Trace(sTemp);
            }

            PreStep.iCycleSubDC = Step.iCycleSubDC;

            if (Stat.bReqStop)
            {
                //return true ;
            }


            switch (Step.iCycleSubDC)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycleSubDC={0:00}", Step.iCycleSubDC);
                    //InitValve();
                    //IO_SetY(yi.CHA_FcmLaserOn, false);
                    return true;

                //DC=====================================================
                case 10:
                    //==============================================================================================
                    //iCmb1DeadVol < iCmb1DCSylPos 이 조건이 되어야 시퀜스 가능.
                    //삼거리전까지 챔버용량빨기 -> 빨은만큼 버리기. -> 검사용량 빨기 -> 데드볼륨밀기 -> cp2대드볼륨용량만큼 빨기 -> 검사용량만큼 밀기.
                    //==============================================================================================

                    //삼거리전까지 챔버용량빨기================================================================
                    Cmbr1.InspSupply(true);
                    SEQ.SyringePump.PickupIncPos((int)PumpID.DC, VALVE_POS.Output, OM.DevInfo.iCmb1ToInter, OM.DevInfo.iDeadVolSpd); //느리게 할려면 크게                                   

                    m_tmDelaySubDC.Clear();

                    Step.iCycleSubDC++;
                    return false;

                case 11://시리얼 통신 렉 기다려줌.
                    if (!m_tmDelaySubDC.OnDelay(300)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.DC)) return false;

                    Cmbr1.InspSupply(false); //챔버 닫고.

                    //삼거리 까지 데드볼륨밀기 DC쪽 2,3번 열고 밀어낸다.===================================================
                    TankCSF.Supply(true);
                    IO_SetY(yi.Dcc3CSf_InSt, true);
                    IO_SetY(yi.Dcc2Vcm_InSt, true);
                    SEQ.SyringePump.DispIncPos((int)PumpID.DC, VALVE_POS.Output, OM.DevInfo.iCmb1ToInter, OM.DevInfo.iDeadVolSpd); //느리게 할려면 크게      \

                    m_tmDelaySubDC.Clear();
                    Step.iCycleSubDC++;
                    return false;


                case 12: //통신렉
                    if (!m_tmDelaySubDC.OnDelay(300)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.DC)) return false;
                    TankCSF.Supply(false);
                    IO_SetY(yi.Dcc3CSf_InSt, false);
                    IO_SetY(yi.Dcc2Vcm_InSt, false);


                    //검사용량 빨기==============================================================================                 
                    Cmbr1.InspSupply(true);
                    SEQ.SyringePump.PickupIncPos((int)PumpID.DC, VALVE_POS.Output, OM.DevInfo.iCmb1DCSylPos, OM.DevInfo.iDeadVolSpd); //느리게 할려면 크게                                   

                    m_tmDelaySubDC.Clear();

                    Step.iCycleSubDC++;
                    return false;

                case 13://시리얼 통신 렉 기다려줌.
                    if (!m_tmDelaySubDC.OnDelay(300)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.DC)) return false;

                    Cmbr1.InspSupply(false); //챔버 닫고.

                    //데드볼륨밀기 DC쪽 2,3번 열고 밀어낸다.===================================================
                    //밀때 해야할놈들.
                    TankCSF.Supply(true);
                    IO_SetY(yi.Dcc3CSf_InSt, true);
                    IO_SetY(yi.Dcc2Vcm_InSt, true);

                    //빨때 해야할 놈들.
                    //FCM DC공용 TankCP2.Supply(true); //CP2탱크열고.

                    SEQ.SyringePump.DispAndPickupInc((int)PumpID.DC, VALVE_POS.Output, VALVE_POS.Input, OM.DevInfo.iCmb1DeadVol, OM.DevInfo.iDeadVolSpd, OM.DevInfo.iCmb1DeadTimes); //느리게 할려면 크게      \

                    m_tmDelaySubDC.Clear();
                    Step.iCycleSubDC++;
                    return false;


                case 14: //통신렉
                    if (!m_tmDelaySubDC.OnDelay(300)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.DC)) return false;
                    //밀때 쓴놈들.
                    TankCSF.Supply(false);
                    IO_SetY(yi.Dcc3CSf_InSt, false);
                    IO_SetY(yi.Dcc2Vcm_InSt, false);

                    //빨때 쓴놈들.
                    //FCM DC공용 TankCP2.Supply(false); //CP2탱크 닫고

                    Step.iCycleSubDC++;
                    return false;

                //cp2대드볼륨용량만큼 빨기================================================================

                //    TankCP2.Supply(true); //CP2탱크열고.
                //    SEQ.SyringePump.PickupIncPos((int)PumpID.DC , VALVE_POS.Input , OM.DevInfo.iCmb1DeadVol , OM.DevInfo.iDeadVolSpd); //느리게 할려면 크게      \
                //    m_tmDelaySubDC.Clear();
                //
                //    Step.iCycleSubDC++;
                //    return false ;
                //
                //case 105://다빨았는지 확인.
                //    if(!m_tmDelaySubDC.OnDelay(300))return false ; 
                //    if(SEQ.SyringePump.GetBusy((int)PumpID.DC )) return false; 
                //
                //    TankCP2.Supply(false); //CP2탱크 닫고

                //실제 검사.=====================================================================
                case 15:
                    m_tmCycleSubDC.Clear();
                    Step.iCycleSubDC++;
                    return false ;

                case 16:
                    if(!m_tmCycleSubDC.OnDelay(OM.DevInfo.iDccInspDelayTime)) return false ; //FCM과 동시에 검사 들어가면 안되서 DCC를 딜레이 시킨다.




                    TankCSR.Supply(true);
                    TankCSF.Supply(true);

                    //IO_SetY(yi.Dcc2Vcm_InSt , true); 2번은 크리닝시에만.
                    IO_SetY(yi.Dcc1Vcm_InSt, true);
                    IO_SetY(yi.Dcc3CSf_InSt, true);
                    IO_SetY(yi.Dcc4CSR_InSt, true);
                    //IO_SetY(yi.Dcc5CP2_InSt , true);

                    //Dc2Air_InVt는 청소할때씀.
                    IO_SetY(yi.Dc1Air_InVt , true );
                    IO_SetY(yi.WastAir_InVc, false);

                    //피도 넣어줌.
                    SEQ.SyringePump.DispIncPos((int)PumpID.DC, VALVE_POS.Output, OM.DevInfo.iCmb1DCSylPos, OM.DevInfo.iCmb1DCSylSpdCode); //속도 느리게 할려면 크게
                    Log.TraceListView("DC Sylinge Start");

                    //FCM탱크가 너무 작어서..
                    //IO_SetY(yi.FCMWOut_OtSt, true);동시에 진행하게 되어서 여기서 건들면 안됌.


                    m_tmDelaySubDC.Clear();
                    Step.iCycleSubDC++;
                    return false;

                case 17:
                    if (!m_tmDelaySubDC.OnDelay(OM.DevInfo.iFCMTestStartDelay)) return false; //어느정도 주입 되고 나서 검사 시작.
                    //FCM탱크가 너무 작어서..
                    //IO_SetY(yi.FCMWOut_OtSt, false);
                    //검사 시그널..
                    SEQ.FcmTester.SendTestSub(TCPIP_NewOpticsFCM.ETest.DC_TEST);
                    Log.TraceListView("NI-DC_TEST");

                    Step.iCycleSubDC++;
                    return false;

                case 18:
                    if (!OM.CmnOptn.bIgnrFCMTester && !SEQ.FcmTester.RcvdMsg) return false;
                    m_tmDelaySubDC.Clear();
                    Step.iCycleSubDC++;
                    return false;

                case 19:
                    if (!m_tmDelaySubDC.OnDelay(OM.DevInfo.iFCMTestEndDelay)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.DC)) return false; //대략 15초 검사해야함.

                    Log.TraceListView("DC Sylinge End");

                    TankCSR.Supply(false);
                    TankCSF.Supply(false);


                    IO_SetY(yi.Dcc3CSf_InSt, false);
                    IO_SetY(yi.Dcc4CSR_InSt, false);
                    //IO_SetY(yi.Dcc5CP2_InSt , false);
                    m_tmDelaySubDC.Clear();
                    Step.iCycleSubDC++;
                    return false;

                case 20:
                    if (!m_tmDelaySubDC.OnDelay(1000)) return false;//0.5초 있다 닫음.

                    IO_SetY(yi.Dcc1Vcm_InSt, false);

                    //Dc2Air_InVt는 청소할때씀.
                    IO_SetY(yi.Dc1Air_InVt, false);

                    //검사끝 마스킹 하고.
                    DM.ARAY[ri.CHA].SetStat(0, 0, cs.WorkEnd);

                    if (MM.GetManNo() == mc.CHA_CycleInspDC)
                    {
                        Step.iCycleSubDC = 0;
                        return true;

                    }

                    Step.iCycleSubDC=0;
                    return true ;


            }
        }


        public bool CycleInspChamberSubHGB()
        {
            string sTemp;
            if (m_tmCycleSubHGB.OnDelay(Step.iCycleSubHgb != 0 && Step.iCycleSubHgb == PreStep.iCycleSubHgb && !m_tmDelaySubHGB.GetUsing() && !OM.MstOptn.bDebugMode, OM.DevInfo.iCmb2BubbleTime + 5000))
            {
                sTemp = string.Format("Time Out Step.iCycleSubHgb={0:00}", Step.iCycleSubHgb);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                ER_SetErr(ei.ETC_CycleTO, sTemp);
                Cmbr2.InspSupply(false); //챔버 닫고.
                IO_SetY(yi.HGBOut_OtSt, false);

                Trace(sTemp);
                return true;
            }

            if (Step.iCycleSubHgb != PreStep.iCycleSubHgb)
            {
                sTemp = string.Format("Cycle Step.iCycleSubHgb={0:00}", Step.iCycleSubHgb);
                Trace(sTemp);
            }

            PreStep.iCycleSubHgb = Step.iCycleSubHgb;

            if (Stat.bReqStop)
            {
                //return true ;
            }

            switch (Step.iCycleSubHgb)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    Cmbr2.InspSupply(false); //챔버 닫고.
                    IO_SetY(yi.HGBOut_OtSt, false);
                    return true;

                //HGB=====================================================
                case 10:
                    if (DM.ARAY[ri.CHA].CheckStat(1, 0, cs.WorkEnd)) //이미 작업 끝난상태.
                    {
                        Step.iCycleSubHgb = 0;
                        return true;
                    }

                    Cmbr2.InspSupply(true); //챔버 열고.

                    m_tmDelaySubHGB.Clear();
                    Step.iCycleSubHgb++;
                    return false;

                case 11:
                    if (!m_tmDelaySubHGB.OnDelay(OM.DevInfo.iCmb2BubbleTime)) return false;

                    Cmbr2.InspSupply(false); //챔버 닫고.

                    OM.EqpStat.sSpectro = "0000";
                    if (!OM.CmnOptn.bNotUseSpec)
                    {
                        SEQ.Spec.Aquire();
                        SEQ.Spec.CalPeakVal();
                        OM.EqpStat.sSpectro = (SEQ.Spec.GetCalHemoglobin(OM.DevInfo.iCmb2Blk, OM.DevInfo.dCmb2SpecAngle) * 100).ToString(); //소수점2자리인데 정수로 100곱해서 보냄.
                    }


                    SEQ.FcmTester.SendHemog(OM.EqpStat.sSpectro);
                    Log.TraceListView("NI-HEMO-" + OM.EqpStat.sSpectro);

                    //IO_SetY(yi.HGBOut_OtSt, true);

                    //m_tmDelay.Clear();
                    Step.iCycleSubHgb++;
                    return false;

                case 12:
                    if (!OM.CmnOptn.bIgnrFCMTester && !SEQ.FcmTester.RcvdMsg) return false;

                    DM.ARAY[ri.CHA].SetStat(1, 0, cs.WorkEnd);

                    Step.iCycleSubHgb = 0;
                    return true;
            }
        }

        bool bFirstFcm = false ;
        public bool CycleInspChamberSubFcm(int _iCmbNo)
        {
            string sTemp;
            if (m_tmCycleSubFCM.OnDelay(Step.iCycleSubFcm != 0 && Step.iCycleSubFcm == PreStep.iCycleSubFcm && CheckStop() && !OM.MstOptn.bDebugMode, 30000))
            {
                sTemp = string.Format("Time Out Step.iCycleSubFcm={0:00}", Step.iCycleSubFcm);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                ER_SetErr(ei.ETC_CycleTO, sTemp);
                //InitValve();
                //IO_SetY(yi.CHA_FcmLaserOn, false);

                Cmbr3.InspSupply(false);
                IO_SetY(yi.FCMOut_OtSt, false);//FCM밑에 노즐.
                //FCM DC공용 TankCP2.Supply(false);
                TankCP1.Supply(false);
                IO_SetY(yi.FCMOut_OtSt, false);
                IO_SetY(yi.FCMWOut_OtSt, false);
                IO_SetY(yi.CHA_FcmLaserOn, false);

                Cmbr4.InspSupply(false);
                Cmbr5.InspSupply(false);
                Cmbr6.InspSupply(false);

                Trace(sTemp);
                return true;
            }

            if (Step.iCycleSubFcm != PreStep.iCycleSubFcm)
            {
                sTemp = string.Format("Cycle iCycleSubFcm.iCycle={0:00}", Step.iCycleSubFcm);
                Trace(sTemp);
            }

            PreStep.iCycleSubFcm = Step.iCycleSubFcm;

            if (Stat.bReqStop)
            {
                //return true ;
            }

            //bool bFirstFcm = DM.ARAY[ri.CHA].CheckStat(2, 0, cs.Work) && DM.ARAY[ri.CHA].CheckStat(3, 0, cs.Work) &&
            //                 DM.ARAY[ri.CHA].CheckStat(4, 0, cs.Work) && DM.ARAY[ri.CHA].CheckStat(5, 0, cs.Work);


            switch (Step.iCycleSubFcm)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycleSubFcm={0:00}", Step.iCycleSubFcm);
                    //InitValve();
                    IO_SetY(yi.CHA_FcmLaserOn, false);
                    return true;

                case 10:
                         if (_iCmbNo == 2) {Step.iCycleSubFcm = 300 ; return false;}
                    else if (_iCmbNo == 3) {Step.iCycleSubFcm = 400 ; return false;}
                    else if (_iCmbNo == 4) {Step.iCycleSubFcm = 500 ; return false;}
                    else if (_iCmbNo == 5) {Step.iCycleSubFcm = 600 ; return false;}
                    
                    Step.iCycleSubFcm = 0 ; 
                    return true ; 

                //3번챔버 FB =====================================================
                case 300:
                    //==============================================================================================
                    //iCmb1DeadVol < iCmb1DCSylPos 이 조건이 되어야 시퀜스 가능.
                    //삼거리전까지 챔버용량빨기 -> 빨은만큼 버리기. -> 검사용량 빨기 -> 데드볼륨밀기 -> cp2대드볼륨용량만큼 빨기 -> 검사용량만큼 밀기.
                    //==============================================================================================
                    if (OM.CmnOptn.bNotUseFB)
                    {
                        DM.ARAY[ri.CHA].SetStat(2, 0, cs.WorkEnd);

                        Step.iCycleSubFcm = 0;
                        return true;
                    }
                    Cmbr3.InspSupply(true); //챔버 열고.
                    SEQ.SyringePump.PickupIncPos((int)PumpID.FCM, VALVE_POS.Output, OM.DevInfo.iCmb3ToInter, OM.DevInfo.iDeadVolSpd); //삼거리까지 담기.
                    m_tmDelaySubFCM.Clear();

                    Step.iCycleSubFcm++;
                    return false;

                case 301://시리얼 통신 렉 기다려줌.
                    if (!m_tmDelaySubFCM.OnDelay(300)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.FCM)) return false;
                    Cmbr3.InspSupply(false); //챔버 닫고.

                    IO_SetY(yi.FCMOut_OtSt, true);//FCM밑에 노즐.
                    SEQ.SyringePump.DispIncPos((int)PumpID.FCM, VALVE_POS.Output, OM.DevInfo.iCmb3ToInter, OM.DevInfo.iDeadVolSpd); //삼거리 까지 빤것 일단 뱉고.          

                    m_tmDelaySubFCM.Clear();
                    Step.iCycleSubFcm++;
                    return false;

                case 302://다빨았는지 확인.
                    if (!m_tmDelaySubFCM.OnDelay(300)) return false;  //잠시 기다렸다가.
                    if (SEQ.SyringePump.GetBusy((int)PumpID.FCM)) return false;
                    IO_SetY(yi.FCMOut_OtSt, false);



                    //혈액 빨기.
                    Cmbr3.InspSupply(true); //챔버 열고.
                    SEQ.SyringePump.PickupIncPos((int)PumpID.FCM, VALVE_POS.Output, OM.DevInfo.iCmb3FCMSylPos, OM.DevInfo.iDeadVolSpd); //혈액샘플 담기.
                    m_tmDelaySubFCM.Clear();

                    Step.iCycleSubFcm++;
                    return false;

                case 303://시리얼 통신 렉 기다려줌.
                    if (!m_tmDelaySubFCM.OnDelay(300)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.FCM)) return false;
                    Cmbr3.InspSupply(false); //챔버 닫고.

                    //뱉을때 쓰는 놈.
                    IO_SetY(yi.FCMOut_OtSt, true);//FCM밑에 노즐.

                    //빨때 쓰는 놈.
                    //FCM DC공용 TankCP2.Supply(true);
                    //SEQ.SyringePump.DispIncPos((int)PumpID.FCM , VALVE_POS.Output , OM.DevInfo.iCmb3DeadVol , OM.DevInfo.iDeadVolSpd); //대드 볼륨 밀어내기.          
                    SEQ.SyringePump.DispAndPickupInc((int)PumpID.FCM, VALVE_POS.Output, VALVE_POS.Input, OM.DevInfo.iCmb3DeadVol, OM.DevInfo.iDeadVolSpd, OM.DevInfo.iCmb3DeadTimes); //느리게 할려면 크게      \


                    m_tmDelaySubFCM.Clear();
                    Step.iCycleSubFcm++;
                    return false;

                case 304://다빨았는지 확인.
                    if (!m_tmDelaySubFCM.OnDelay(300)) return false;  //잠시 기다렸다가.
                    if (SEQ.SyringePump.GetBusy((int)PumpID.FCM)) return false;

                    //뱉을때 쓰는 놈.
                    IO_SetY(yi.FCMOut_OtSt, false);

                    //빨때 쓰던놈.
                    //FCM DC공용 TankCP2.Supply(false);
                    Step.iCycleSubFcm++;
                    return false;


                    //검사시작=========================================================================
                case 305:
                    m_tmDelaySubFCM.Clear();
                    Step.iCycleSubFcm++;
                    return false;

                case 306:
                    if (bFirstFcm && !m_tmDelaySubFCM.OnDelay(OM.DevInfo.iFCMTestDelayTime)) return false;

                    //검사시에 CP1 먼저 투여.
                    IO_SetY(yi.FCMOut_OtSt, true);
                    TankCP1.Supply(true); //CP1 먼저 공급. 먼저cP1을 공급.

                    //LD On
                    IO_SetY(yi.CHA_FcmLaserOn, true);

                    m_tmDelaySubFCM.Clear();
                    Step.iCycleSubFcm++;
                    return false;

                case 307:
                    if (!m_tmDelaySubFCM.OnDelay(1000)) return false; //CP1부터 투여 하고 1초 있다가.
                    SEQ.SyringePump.DispIncPos((int)PumpID.FCM, VALVE_POS.Output, OM.DevInfo.iCmb3FCMSylPos, OM.DevInfo.iCmb3FCMSylSpdCode); //샘플 FCM으로 밀어넣기.  
                    Log.TraceListView("FCM3 Sylinge Start");

                    //FCM탱크가 너무 작어서..
                    IO_SetY(yi.FCMWOut_OtSt, true);

                    m_tmDelaySubFCM.Clear();
                    Step.iCycleSubFcm++;
                    return false;

                case 308:
                    if (!m_tmDelaySubFCM.OnDelay(OM.DevInfo.iFCMTestStartDelay)) return false; //혈액샘플이 FCM에 들어가게 잠깐 쉬었다. 

                    //FCM탱크가 너무 작어서..
                    IO_SetY(yi.FCMWOut_OtSt, false);

                    //검사기에게 프로토콜.
                    if (OM.CmnOptn.bAutoQc) { SEQ.FcmTester.SendTestSub(TCPIP_NewOpticsFCM.ETest.QC_FCM_1_TEST); Log.TraceListView("NI-QC_FCM_1_TEST"); }
                    else { SEQ.FcmTester.SendTestSub(TCPIP_NewOpticsFCM.ETest.FCM_1_TEST); Log.TraceListView("NI-FCM_1_TEST"); }



                    Step.iCycleSubFcm++;
                    return false;

                case 309:
                    if (!OM.CmnOptn.bIgnrFCMTester && !SEQ.FcmTester.RcvdMsg) return false;
                    m_tmDelaySubFCM.Clear();
                    Step.iCycleSubFcm++;
                    return false;

                case 310:
                    if (!m_tmDelaySubFCM.OnDelay(OM.DevInfo.iFCMTestEndDelay)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.FCM)) return false;

                    Log.TraceListView("FCM3 Sylinge End");

                    TankCP1.Supply(false);
                    IO_SetY(yi.FCMOut_OtSt, false);//FCM밑에 노즐.
                                                   //Cmbr3.InspSupply(false); //챔버 닫고.

                    Step.iCycleSubFcm++;
                    return false;

                case 311:

                    DM.ARAY[ri.CHA].SetStat(2, 0, cs.WorkEnd);

                    IO_SetY(yi.CHA_FcmLaserOn, false);
                    Step.iCycleSubFcm = 0;
                    return true;


                //4번챔버 4DLS =====================================================
                case 400:
                    //==============================================================================================
                    //iCmb1DeadVol < iCmb1DCSylPos 이 조건이 되어야 시퀜스 가능.
                    //삼거리전까지 챔버용량빨기 -> 빨은만큼 버리기. -> 검사용량 빨기 -> 데드볼륨밀기 -> cp2대드볼륨용량만큼 빨기 -> 검사용량만큼 밀기.
                    //==============================================================================================
                    if (OM.CmnOptn.bNotUse4DLS)
                    {
                        DM.ARAY[ri.CHA].SetStat(3, 0, cs.WorkEnd);
               
                        Step.iCycleSubFcm = 0;
                        return true;
                    }

                    Cmbr4.InspSupply(true); //챔버 열고.
                    SEQ.SyringePump.PickupIncPos((int)PumpID.FCM, VALVE_POS.Output, OM.DevInfo.iCmb4ToInter, OM.DevInfo.iDeadVolSpd); //혈액샘플 담기.
                    m_tmDelaySubFCM.Clear();

                    Step.iCycleSubFcm++;
                    return false;

                case 401://시리얼 통신 렉 기다려줌.
                    if (!m_tmDelaySubFCM.OnDelay(300)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.FCM)) return false;
                    Cmbr4.InspSupply(false); //챔버 닫고.

                    IO_SetY(yi.FCMOut_OtSt, true);//FCM밑에 노즐.
                    SEQ.SyringePump.DispIncPos((int)PumpID.FCM, VALVE_POS.Output, OM.DevInfo.iCmb4ToInter, OM.DevInfo.iDeadVolSpd); //대드 볼륨 밀어내기.          

                    m_tmDelaySubFCM.Clear();
                    Step.iCycleSubFcm++;
                    return false;

                case 402://다빨았는지 확인.
                    if (!m_tmDelaySubFCM.OnDelay(300)) return false;  //잠시 기다렸다가.
                    if (SEQ.SyringePump.GetBusy((int)PumpID.FCM)) return false;
                    IO_SetY(yi.FCMOut_OtSt, false);

                    m_tmDelaySubFCM.Clear();
                    Step.iCycleSubFcm++;
                    return false;

                case 403:
                    if (bFirstFcm && !m_tmDelaySubFCM.OnDelay(OM.DevInfo.iFCMTestDelayTime)) return false;

                    //검사 혈액 담기.
                    Cmbr4.InspSupply(true); //챔버 열고.
                    SEQ.SyringePump.PickupIncPos((int)PumpID.FCM, VALVE_POS.Output, OM.DevInfo.iCmb4FCMSylPos, OM.DevInfo.iDeadVolSpd); //혈액샘플 담기.
                    m_tmDelaySubFCM.Clear();

                    Step.iCycleSubFcm++;
                    return false;

                case 404://시리얼 통신 렉 기다려줌.
                    if (!m_tmDelaySubFCM.OnDelay(300)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.FCM)) return false;
                    Cmbr4.InspSupply(false); //챔버 닫고.

                    IO_SetY(yi.FCMOut_OtSt, true);//FCM밑에 노즐.

                    //FCM DC공용 TankCP2.Supply(true);

                    //SEQ.SyringePump.DispIncPos((int)PumpID.FCM , VALVE_POS.Output , OM.DevInfo.iCmb4DeadVol , OM.DevInfo.iDeadVolSpd); //대드 볼륨 밀어내기.      
                    SEQ.SyringePump.DispAndPickupInc((int)PumpID.FCM, VALVE_POS.Output, VALVE_POS.Input, OM.DevInfo.iCmb4DeadVol, OM.DevInfo.iDeadVolSpd, OM.DevInfo.iCmb4DeadTimes); //느리게 할려면 크게      \

                    m_tmDelaySubFCM.Clear();
                    Step.iCycleSubFcm++;
                    return false;

                case 405://다빨았는지 확인.
                    if (!m_tmDelaySubFCM.OnDelay(300)) return false;  //잠시 기다렸다가.
                    if (SEQ.SyringePump.GetBusy((int)PumpID.FCM)) return false;
                    IO_SetY(yi.FCMOut_OtSt, false);

                    //FCM DC공용 TankCP2.Supply(false);
                    Step.iCycleSubFcm++;
                    return false;


                case 406://다빨았는지 확인.
                    //검사시작=========================================================================
                    m_tmDelaySubFCM.Clear();
                    Step.iCycleSubFcm++;
                    return false;

                case 407:
                    if (bFirstFcm && !m_tmDelaySubFCM.OnDelay(OM.DevInfo.iFCMTestDelayTime)) return false;

                    //검사시에 CP1 먼저 투여.
                    IO_SetY(yi.FCMOut_OtSt, true);
                    TankCP1.Supply(true); //CP1 먼저 공급. 먼저cP1을 공급.

                    //LD On
                    IO_SetY(yi.CHA_FcmLaserOn, true);

                    m_tmDelaySubFCM.Clear();
                    Step.iCycleSubFcm++;
                    return false;

                case 408:
                    if (!m_tmDelaySubFCM.OnDelay(1000)) return false; //CP1부터 투여 하고 1초 있다가.
                    SEQ.SyringePump.DispIncPos((int)PumpID.FCM, VALVE_POS.Output, OM.DevInfo.iCmb4FCMSylPos, OM.DevInfo.iCmb4FCMSylSpdCode); //샘플 FCM으로 밀어넣기.  
                    Log.TraceListView("FCM2 Sylinge Start");
                    //FCM탱크가 너무 작어서..
                    IO_SetY(yi.FCMWOut_OtSt, true);
                    m_tmDelaySubFCM.Clear();
                    Step.iCycleSubFcm++;
                    return false;

                case 409:
                    if (!m_tmDelaySubFCM.OnDelay(OM.DevInfo.iFCMTestStartDelay)) return false; //혈액샘플이 FCM에 들어가게 잠깐 쉬었다. 

                    //FCM탱크가 너무 작어서..
                    IO_SetY(yi.FCMWOut_OtSt, false);

                    //검사기에게 프로토콜.
                    if (OM.CmnOptn.bAutoQc) { SEQ.FcmTester.SendTestSub(TCPIP_NewOpticsFCM.ETest.QC_FCM_2_TEST); Log.TraceListView("NI-QC_FCM_2_TEST"); }
                    else                    { SEQ.FcmTester.SendTestSub(TCPIP_NewOpticsFCM.ETest.FCM_2_TEST   ); Log.TraceListView("NI-FCM_2_TEST"); }


                    Step.iCycleSubFcm++;
                    return false;

                case 410:
                    if (!OM.CmnOptn.bIgnrFCMTester && !SEQ.FcmTester.RcvdMsg) return false;
                    m_tmDelaySubFCM.Clear();
                    Step.iCycleSubFcm++;
                    return false;

                case 411:
                    if (!m_tmDelaySubFCM.OnDelay(OM.DevInfo.iFCMTestEndDelay)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.FCM)) return false;

                    Log.TraceListView("FCM2 Sylinge End");

                    TankCP1.Supply(false);
                    IO_SetY(yi.FCMOut_OtSt, false);//FCM밑에 노즐.
                    Step.iCycleSubFcm++;
                    return false;

                case 412:

                    DM.ARAY[ri.CHA].SetStat(3, 0, cs.WorkEnd);

                    IO_SetY(yi.CHA_FcmLaserOn, false);
                    Step.iCycleSubFcm = 0;
                    return true;

                //5번챔버 RET =====================================================
                case 500:
                    //==============================================================================================
                    //iCmb1DeadVol < iCmb1DCSylPos 이 조건이 되어야 시퀜스 가능.
                    //삼거리전까지 챔버용량빨기 -> 빨은만큼 버리기. -> 검사용량 빨기 -> 데드볼륨밀기 -> cp2대드볼륨용량만큼 빨기 -> 검사용량만큼 밀기.
                    //==============================================================================================
                    if (OM.CmnOptn.bNotUseRet)
                    {
                        DM.ARAY[ri.CHA].SetStat(4, 0, cs.WorkEnd);
                        Step.iCycleSubFcm = 0;
                        return true;
                    }

                    Cmbr5.InspSupply(true); //챔버 열고.
                    SEQ.SyringePump.PickupIncPos((int)PumpID.FCM, VALVE_POS.Output, OM.DevInfo.iCmb5ToInter, OM.DevInfo.iDeadVolSpd); //혈액샘플 담기.
                    m_tmDelaySubFCM.Clear();

                    Step.iCycleSubFcm++;
                    return false;

                case 501://시리얼 통신 렉 기다려줌.
                    if (!m_tmDelaySubFCM.OnDelay(300)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.FCM)) return false;
                    Cmbr5.InspSupply(false); //챔버 닫고.

                    IO_SetY(yi.FCMOut_OtSt, true);//FCM밑에 노즐.
                    SEQ.SyringePump.DispIncPos((int)PumpID.FCM, VALVE_POS.Output, OM.DevInfo.iCmb5ToInter, OM.DevInfo.iDeadVolSpd); //대드 볼륨 밀어내기.     
                    m_tmDelaySubFCM.Clear();
                    Step.iCycleSubFcm++;
                    return false;

                case 502://다빨았는지 확인.
                    if (!m_tmDelaySubFCM.OnDelay(300)) return false;  //잠시 기다렸다가.
                    if (SEQ.SyringePump.GetBusy((int)PumpID.FCM)) return false;
                    IO_SetY(yi.FCMOut_OtSt, false);
                    Step.iCycleSubFcm++;
                    return false ;

                case 503:
                    if (bFirstFcm && !m_tmDelaySubFCM.OnDelay(OM.DevInfo.iFCMTestDelayTime)) return false;

                    //혈액검사 샘플 담기.
                    Cmbr5.InspSupply(true); //챔버 열고.
                    SEQ.SyringePump.PickupIncPos((int)PumpID.FCM, VALVE_POS.Output, OM.DevInfo.iCmb5FCMSylPos, OM.DevInfo.iDeadVolSpd); //혈액샘플 담기.
                    m_tmDelaySubFCM.Clear();

                    Step.iCycleSubFcm++;
                    return false;

                case 504://시리얼 통신 렉 기다려줌.
                    if (!m_tmDelaySubFCM.OnDelay(300)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.FCM)) return false;
                    Cmbr5.InspSupply(false); //챔버 닫고.

                    IO_SetY(yi.FCMOut_OtSt, true);//FCM밑에 노즐.

                    //FCM DC공용 TankCP2.Supply(true);
                    //SEQ.SyringePump.DispIncPos((int)PumpID.FCM , VALVE_POS.Output , OM.DevInfo.iCmb5DeadVol , OM.DevInfo.iDeadVolSpd); //대드 볼륨 밀어내기.     
                    SEQ.SyringePump.DispAndPickupInc((int)PumpID.FCM, VALVE_POS.Output, VALVE_POS.Input, OM.DevInfo.iCmb5DeadVol, OM.DevInfo.iDeadVolSpd, OM.DevInfo.iCmb5DeadTimes); //느리게 할려면 크게      \
                    m_tmDelaySubFCM.Clear();
                    Step.iCycleSubFcm++;
                    return false;

                case 505://다빨았는지 확인.
                    if (!m_tmDelaySubFCM.OnDelay(300)) return false;  //잠시 기다렸다가.
                    if (SEQ.SyringePump.GetBusy((int)PumpID.FCM)) return false;
                    IO_SetY(yi.FCMOut_OtSt, false);

                    //FCM DC공용 TankCP2.Supply(false);
                    Step.iCycleSubFcm++;
                    return false;

                //  TankCP2.Supply(true);
                //    SEQ.SyringePump.PickupIncPos((int)PumpID.FCM , VALVE_POS.Input , OM.DevInfo.iCmb5DeadVol , OM.DevInfo.iDeadVolSpd); //대드볼륨 밀어낼꺼 담기.   
                //    m_tmDelay.Clear();
                //    Step.iCycleSubFcm++;
                //    return false ;
                //
                //case 505://다빨았는지 확인.
                //    if(!m_tmDelay.OnDelay(300))return false ; 
                //    if(SEQ.SyringePump.GetBusy((int)PumpID.FCM )) return false;
                //    TankCP2.Supply(false);

                case 506://다빨았는지 확인.
                    //검사시작=========================================================================
                    m_tmDelaySubFCM.Clear();
                    Step.iCycleSubFcm++;
                    return false;

                case 507:
                    if (bFirstFcm && !m_tmDelaySubFCM.OnDelay(OM.DevInfo.iFCMTestDelayTime)) return false;
                    //검사시에 CP1 먼저 투여.
                    IO_SetY(yi.FCMOut_OtSt, true);
                    TankCP1.Supply(true); //CP1 먼저 공급. 먼저cP1을 공급.

                    //LD On
                    IO_SetY(yi.CHA_FcmLaserOn, true);

                    m_tmDelaySubFCM.Clear();
                    Step.iCycleSubFcm++;
                    return false;

                case 508:
                    if (!m_tmDelaySubFCM.OnDelay(1000)) return false; //CP1부터 투여 하고 1초 있다가.
                    SEQ.SyringePump.DispIncPos((int)PumpID.FCM, VALVE_POS.Output, OM.DevInfo.iCmb5FCMSylPos, OM.DevInfo.iCmb5FCMSylSpdCode); //샘플 FCM으로 밀어넣기. 
                    Log.TraceListView("FCM3 Sylinge Start");

                    //FCM탱크가 너무 작어서..
                    IO_SetY(yi.FCMWOut_OtSt, true);

                    m_tmDelaySubFCM.Clear();
                    Step.iCycleSubFcm++;
                    return false;

                case 509:
                    if (!m_tmDelaySubFCM.OnDelay(OM.DevInfo.iFCMTestStartDelay)) return false; //혈액샘플이 FCM에 들어가게 잠깐 쉬었다. 

                    //FCM탱크가 너무 작어서..
                    IO_SetY(yi.FCMWOut_OtSt, false);

                    //검사기에게 프로토콜.
                    if (OM.CmnOptn.bAutoQc) { SEQ.FcmTester.SendTestSub(TCPIP_NewOpticsFCM.ETest.QC_FCM_3_TEST); Log.TraceListView("NI-QC_FCM_3_TEST"); }
                    else { SEQ.FcmTester.SendTestSub(TCPIP_NewOpticsFCM.ETest.FCM_3_TEST); Log.TraceListView("NI-FCM_3_TEST"); }


                    Step.iCycleSubFcm++;
                    return false;

                case 510:
                    if (!OM.CmnOptn.bIgnrFCMTester && !SEQ.FcmTester.RcvdMsg) return false;
                    m_tmDelaySubFCM.Clear();
                    Step.iCycleSubFcm++;
                    return false;


                case 511:
                    if (!m_tmDelaySubFCM.OnDelay(OM.DevInfo.iFCMTestEndDelay)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.FCM)) return false;

                    Log.TraceListView("FCM2 Sylinge End");

                    TankCP1.Supply(false);
                    IO_SetY(yi.FCMOut_OtSt, false);//FCM밑에 노즐.

                    Step.iCycleSubFcm++;
                    return false;

                case 512:
                    DM.ARAY[ri.CHA].SetStat(4, 0, cs.WorkEnd);
                    //LD Off
                    IO_SetY(yi.CHA_FcmLaserOn, false);

                    Step.iCycleSubFcm = 0;
                    return true;

                //6번챔버 NR =====================================================
                case 600:
                    //==============================================================================================
                    //iCmb1DeadVol < iCmb1DCSylPos 이 조건이 되어야 시퀜스 가능.
                    //삼거리전까지 챔버용량빨기 -> 빨은만큼 버리기. -> 검사용량 빨기 -> 데드볼륨밀기 -> cp2대드볼륨용량만큼 빨기 -> 검사용량만큼 밀기.
                    //==============================================================================================
                    if (OM.CmnOptn.bNotUseNr)
                    {
                        DM.ARAY[ri.CHA].SetStat(5, 0, cs.WorkEnd);
                        Step.iCycleSubFcm = 0;
                        return true;
                    }
                    Cmbr6.InspSupply(true); //챔버 열고.
                    SEQ.SyringePump.PickupIncPos((int)PumpID.FCM, VALVE_POS.Output, OM.DevInfo.iCmb6ToInter, OM.DevInfo.iDeadVolSpd); //혈액샘플 담기.
                    m_tmDelaySubFCM.Clear();

                    Step.iCycleSubFcm++;
                    return false;

                case 601://시리얼 통신 렉 기다려줌.
                    if (!m_tmDelaySubFCM.OnDelay(300)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.FCM)) return false;
                    Cmbr6.InspSupply(false); //챔버 닫고.

                    IO_SetY(yi.FCMOut_OtSt, true);//FCM밑에 노즐.
                    SEQ.SyringePump.DispIncPos((int)PumpID.FCM, VALVE_POS.Output, OM.DevInfo.iCmb6ToInter, OM.DevInfo.iDeadVolSpd); //대드 볼륨 밀어내기.        
                    m_tmDelaySubFCM.Clear();
                    Step.iCycleSubFcm++;
                    return false;

                case 602://다빨았는지 확인.
                    if (!m_tmDelaySubFCM.OnDelay(300)) return false;  //잠시 기다렸다가.
                    if (SEQ.SyringePump.GetBusy((int)PumpID.FCM)) return false;
                    IO_SetY(yi.FCMOut_OtSt, false);


                    Cmbr6.InspSupply(true); //챔버 열고.
                    SEQ.SyringePump.PickupIncPos((int)PumpID.FCM, VALVE_POS.Output, OM.DevInfo.iCmb6FCMSylPos, OM.DevInfo.iDeadVolSpd); //혈액샘플 담기.
                    m_tmDelaySubFCM.Clear();

                    Step.iCycleSubFcm++;
                    return false;

                case 603://시리얼 통신 렉 기다려줌.
                    if (!m_tmDelaySubFCM.OnDelay(300)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.FCM)) return false;
                    Cmbr6.InspSupply(false); //챔버 닫고.

                    IO_SetY(yi.FCMOut_OtSt, true);//FCM밑에 노즐.

                    //FCM DC공용 TankCP2.Supply(true);

                    //SEQ.SyringePump.DispIncPos((int)PumpID.FCM , VALVE_POS.Output , OM.DevInfo.iCmb6DeadVol , OM.DevInfo.iDeadVolSpd); //대드 볼륨 밀어내기.        
                    SEQ.SyringePump.DispAndPickupInc((int)PumpID.FCM, VALVE_POS.Output, VALVE_POS.Input, OM.DevInfo.iCmb6DeadVol, OM.DevInfo.iDeadVolSpd, OM.DevInfo.iCmb6DeadTimes); //느리게 할려면 크게      \
                    m_tmDelaySubFCM.Clear();
                    Step.iCycleSubFcm++;
                    return false;

                case 604://다빨았는지 확인.
                    if (!m_tmDelaySubFCM.OnDelay(300)) return false;  //잠시 기다렸다가.
                    if (SEQ.SyringePump.GetBusy((int)PumpID.FCM)) return false;
                    IO_SetY(yi.FCMOut_OtSt, false);

                    //FCM DC공용 TankCP2.Supply(false);
                    Step.iCycleSubFcm++;
                    return false;

                //
                //
                //    TankCP2.Supply(true);
                //    SEQ.SyringePump.PickupIncPos((int)PumpID.FCM , VALVE_POS.Input , OM.DevInfo.iCmb6DeadVol , OM.DevInfo.iDeadVolSpd); //대드볼륨 밀어낼꺼 담기.   
                //    m_tmDelay.Clear();
                //    Step.iCycle++;
                //    return false ;
                //
                //case 605://다빨았는지 확인.
                //    if(!m_tmDelay.OnDelay(300))return false ; 
                //    if(SEQ.SyringePump.GetBusy((int)PumpID.FCM )) return false;
                //    TankCP2.Supply(false);


                case 605://다빨았는지 확인.
                    //검사시작=========================================================================
                    m_tmDelaySubFCM.Clear();
                    Step.iCycleSubFcm++;
                    return false;

                case 606:
                    if (bFirstFcm && !m_tmDelaySubFCM.OnDelay(OM.DevInfo.iFCMTestDelayTime)) return false;
                    //검사시에 CP1 먼저 투여.
                    IO_SetY(yi.FCMOut_OtSt, true);
                    TankCP1.Supply(true); //CP1 먼저 공급. 먼저cP1을 공급.

                    //LD On
                    IO_SetY(yi.CHA_FcmLaserOn, true);

                    m_tmDelaySubFCM.Clear();
                    Step.iCycleSubFcm++;
                    return false;

                case 607:
                    if (!m_tmDelaySubFCM.OnDelay(1000)) return false; //CP1부터 투여 하고 1초 있다가.
                    SEQ.SyringePump.DispIncPos((int)PumpID.FCM, VALVE_POS.Output, OM.DevInfo.iCmb6FCMSylPos, OM.DevInfo.iCmb6FCMSylSpdCode); //샘플 FCM으로 밀어넣기.   
                    Log.TraceListView("FCM4 Sylinge Start");

                    //FCM탱크가 너무 작어서..
                    IO_SetY(yi.FCMWOut_OtSt, true);

                    m_tmDelaySubFCM.Clear();
                    Step.iCycleSubFcm++;
                    return false;

                case 608:
                    if (!m_tmDelaySubFCM.OnDelay(OM.DevInfo.iFCMTestStartDelay)) return false; //혈액샘플이 FCM에 들어가게 잠깐 쉬었다. 
                    //FCM탱크가 너무 작어서..
                    IO_SetY(yi.FCMWOut_OtSt, false);
                    //검사기에게 프로토콜.
                    if (OM.CmnOptn.bAutoQc) { SEQ.FcmTester.SendTestSub(TCPIP_NewOpticsFCM.ETest.QC_FCM_4_TEST); Log.TraceListView("NI-QC_FCM_4_TEST"); }
                    else { SEQ.FcmTester.SendTestSub(TCPIP_NewOpticsFCM.ETest.FCM_4_TEST); Log.TraceListView("NI-FCM_4_TEST"); }

                    Step.iCycleSubFcm++;
                    return false;

                case 609:
                    if (!OM.CmnOptn.bIgnrFCMTester && !SEQ.FcmTester.RcvdMsg) return false;
                    m_tmDelaySubFCM.Clear();
                    Step.iCycleSubFcm++;
                    return false;

                case 610:
                    if (!m_tmDelaySubFCM.OnDelay(OM.DevInfo.iFCMTestEndDelay)) return false;  //원래 검사 끝나는 프로토콜이 있는줄 알았는데 NI쪽에서 알아서 함. 그래서 의미 없음.
                    if (SEQ.SyringePump.GetBusy((int)PumpID.FCM)) return false;

                    Log.TraceListView("FCM4 Sylinge End");

                    TankCP1.Supply(false);
                    IO_SetY(yi.FCMOut_OtSt, false);//FCM밑에 노즐.

                    Step.iCycleSubFcm++;
                    return false;

                case 611:
                    DM.ARAY[ri.CHA].SetStat(5, 0, cs.WorkEnd);

                    //LD Off
                    IO_SetY(yi.CHA_FcmLaserOn, false);

                    Step.iCycleSubFcm = 0;
                    return true;

            }
        }


       
        
        

        public bool CycleEmptyChamber()
        {
            string sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                ER_SetErr(ei.ETC_CycleTO, sTemp);
                
                InitValve();

                Trace(sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                Trace(sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //return true ;
            }

            switch (Step.iCycle)
            {

                default:
                    InitValve();
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    InitValve();

                    if(MM.GetManNo() == mc.NoneCycle)OM.EqpStat.iInspProgress = 50 ;

                    //SEQ.SYR.MoveMotr(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove);
                    Step.iCycle++;
                    return false ;

                case 11:
                    //if(SEQ.SYR.MoveMotr(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove))return false ;
                    MoveCyl(ci.CHA_MixCoverOpCl , fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 12:
                    if(!CL_Complete(ci.CHA_MixCoverOpCl , fb.Bwd))return false ;

                    //1번챔버.==============================================
                    if(MM.GetManNo() == mc.NoneCycle)OM.EqpStat.iInspProgress = 53 ;
                    Cmbr1.WasteOut(true);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;
                case 13:
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iCmbEmptyTime)) return false ;
                    Cmbr1.WasteOut(false);

                    //2번챔버.==============================================
                    if(MM.GetManNo() == mc.NoneCycle)OM.EqpStat.iInspProgress = 56 ;
                    Cmbr2.WasteOut(true);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;
                case 14:
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iCmbEmptyTime)) return false ;
                    Cmbr2.WasteOut(false);

                    //3번챔버.==============================================
                    if(MM.GetManNo() == mc.NoneCycle)OM.EqpStat.iInspProgress = 59 ;
                    if (OM.CmnOptn.bNotUseFB)
                    {
                        Step.iCycle++;
                        return false;
                    }

                    Cmbr3.WasteOut(true);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;
                case 15:
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iCmbEmptyTime)) return false ;
                    Cmbr3.WasteOut(false);

                    //4번챔버.==============================================
                    if(MM.GetManNo() == mc.NoneCycle)OM.EqpStat.iInspProgress = 62 ;
                    if (OM.CmnOptn.bNotUse4DLS)
                    {
                        Step.iCycle++;
                        return false;
                    }
                    Cmbr4.WasteOut(true);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;
                case 16:
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iCmbEmptyTime)) return false ;
                    Cmbr4.WasteOut(false);

                    //5번챔버.==============================================
                    if(MM.GetManNo() == mc.NoneCycle)OM.EqpStat.iInspProgress = 65 ;
                    if(OM.CmnOptn.bNotUseRet)
                    {
                        Step.iCycle++;
                        return false;
                    }
                    Cmbr5.WasteOut(true);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;
                case 17:
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iCmbEmptyTime)) return false ;
                    Cmbr5.WasteOut(false);

                    //6번챔버.==============================================
                    if(MM.GetManNo() == mc.NoneCycle)OM.EqpStat.iInspProgress = 68 ;
                    if (OM.CmnOptn.bNotUseNr)
                    {
                        Step.iCycle++;
                        return false;
                    }
                    Cmbr6.WasteOut(true);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;
                case 18:
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iCmbEmptyTime)) return false ;
                    Cmbr6.WasteOut(false);

                    //HGB FCM과 같이 하지 않는이유는 HGB FCM 둘다 벤트가 있어서 
                    //먼저 끝나는쪽에서 공기가 유입 되어 남은 하나의 흡입력이 딸림.
                    //HGB는 상대적으로 빨리 끝나서 후딱하고 FCM을 하는 방식.
                    IO_SetY(yi.HGBOut_OtSt , true);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 19:
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iHgbToWastTime)) return false ;
                    IO_SetY(yi.HGBOut_OtSt , false);
                    if(MM.GetManNo() == mc.NoneCycle)OM.EqpStat.iInspProgress = 70 ;
                    //FCM도 비운다.
                    IO_SetY(yi.FCMWOut_OtSt , true);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 20:
                    if (!m_tmDelay.OnDelay(OM.DevInfo.iFcmWastToWastTime)) return false;
                    IO_SetY(yi.FCMWOut_OtSt , false);

                    if(MM.GetManNo() == mc.NoneCycle)OM.EqpStat.iInspProgress = 75 ;

                    DM.ARAY[ri.CHA].SetStat(cs.Clean);


                    Step.iCycle = 0 ;
                    return true ;
            }
        }

        //기존에 전체 같이 하는것 .... 리크 때문에 같이 하면 안됌. 일단 냄겨둠.
        public bool CycleEmptyChamberAll()
        {
            string sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                ER_SetErr(ei.ETC_CycleTO, sTemp);
                
                InitValve();

                Trace(sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                Trace(sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //return true ;
            }

            switch (Step.iCycle)
            {

                default:
                    InitValve();
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true;

                case 10:
                    InitValve();

                    //SEQ.SYR.MoveMotr(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove);
                    Step.iCycle++;
                    return false ;

                case 11:
                    //if(SEQ.SYR.MoveMotr(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove))return false ;
                    MoveCyl(ci.CHA_MixCoverOpCl , fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 12:
                    if(!CL_Complete(ci.CHA_MixCoverOpCl , fb.Bwd))return false ;

                    Cmbr1.WasteOut(true);
                    Cmbr2.WasteOut(true);
                    Cmbr3.WasteOut(true);
                    Cmbr4.WasteOut(true);
                    Cmbr5.WasteOut(true);
                    Cmbr6.WasteOut(true);

                    //시간이 많이 걸려 여기서도 FCM도 같이 한다.
                    IO_SetY(yi.FCMWOut_OtSt, true);

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 13:
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iCmbEmptyTime)) return false ;
                    Cmbr1.WasteOut(false);
                    Cmbr2.WasteOut(false);
                    Cmbr3.WasteOut(false);
                    Cmbr4.WasteOut(false);
                    Cmbr5.WasteOut(false);
                    Cmbr6.WasteOut(false);

                    IO_SetY(yi.FCMWOut_OtSt, false);

                    //HGB FCM과 같이 하지 않는이유는 HGB FCM 둘다 벤트가 있어서 
                    //먼저 끝나는쪽에서 공기가 유입 되어 남은 하나의 흡입력이 딸림.
                    //HGB는 상대적으로 빨리 끝나서 후딱하고 FCM을 하는 방식.
                    IO_SetY(yi.HGBOut_OtSt , true);

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 14:
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iHgbToWastTime)) return false ;
                    IO_SetY(yi.HGBOut_OtSt , false);

                    //FCM도 비운다.
                    IO_SetY(yi.FCMWOut_OtSt , true);

                    

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 15:
                    if (!m_tmDelay.OnDelay(OM.DevInfo.iFcmWastToWastTime)) return false;
                    IO_SetY(yi.FCMWOut_OtSt , false);

                    DM.ARAY[ri.CHA].SetStat(cs.Clean);


                    Step.iCycle = 0 ;
                    return true ;
            }
        }

        public bool CycleCleanChamber()
        {
            string sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                ER_SetErr(ei.ETC_CycleTO, sTemp);
                InitValve();
                Trace(sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                Trace(sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //return true ;
            }

            switch (Step.iCycle)
            {

                default:
                    InitValve();
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    return true ;

                
                case 10:
                    InitValve();

                    //DC 테스트.
                    if(MM.GetManNo() == mc.CHA_CycleCleanDcc)
                    {
                        Step.iCycle = 24;
                        return false;

                    }

                    if(OM.CmnOptn.bNotUseClean)
                    {
                        Step.iCycle = 24;
                        return false;
                    }

                    if(MM.GetManNo() == mc.NoneCycle)OM.EqpStat.iInspProgress = 75 ;
                    //
                    //

                    SEQ.SYR.MoveMotr(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove);
                    Step.iCycle++;
                    return false ;

                case 11:
                    if(!MT_GetStopPos(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove))return false ;


                    MoveCyl(ci.CHA_MixCoverOpCl , fb.Bwd);
                    Step.iCycle++;
                    return false ;

                case 12:
                    if(!CL_Complete(ci.CHA_MixCoverOpCl , fb.Bwd))return false ;

                    //CP탱크 열고.
                    //FCM DC공용 TankCP2.Supply(true); //

                    //실린지로 CP2 들이마셔서.
                    SEQ.SyringePump.PickupAndDispInc((int)PumpID.DC  , VALVE_POS.Input, VALVE_POS.Output, OM.DevInfo.iCmb1CleanRvsPos, 15); IO_SetY(yi.Cmb1Out_OtSt, true);//역류용 밸브 열고.
                    SEQ.SyringePump.PickupAndDispInc((int)PumpID.FCM , VALVE_POS.Input, VALVE_POS.Output, OM.DevInfo.iCmb3CleanRvsPos, 15); IO_SetY(yi.Cmb3Out_OtSt, true);//역류용 밸브 열고.
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 13:
                    if (!m_tmDelay.OnDelay(300)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.DC )) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.FCM)) return false;

                    IO_SetY(yi.Cmb1Out_OtSt, false);//역류용 밸브 닫고.
                    IO_SetY(yi.Cmb3Out_OtSt, false);//역류용 밸브 닫고.

                    SEQ.SyringePump.PickupAndDispInc((int)PumpID.FCM, VALVE_POS.Input, VALVE_POS.Output, OM.DevInfo.iCmb4CleanRvsPos, 15); IO_SetY(yi.Cmb4Out_OtSt, true);//역류용 밸브 열고.
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 14:
                    if (!m_tmDelay.OnDelay(300)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.FCM)) return false;
                    if(MM.GetManNo() == mc.NoneCycle)OM.EqpStat.iInspProgress = 78 ;
                    IO_SetY(yi.Cmb4Out_OtSt, false);//역류용 밸브 닫고.

                    if (OM.CmnOptn.bNotUseRet)
                    {
                        Step.iCycle++;
                        return false;
                    }

                    SEQ.SyringePump.PickupAndDispInc((int)PumpID.FCM, VALVE_POS.Input, VALVE_POS.Output, OM.DevInfo.iCmb5CleanRvsPos, 15); IO_SetY(yi.Cmb5Out_OtSt, true);//역류용 밸브 열고.
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 15:
                    if (!m_tmDelay.OnDelay(300)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.FCM)) return false;
                    if(MM.GetManNo() == mc.NoneCycle)OM.EqpStat.iInspProgress = 81 ;
                    IO_SetY(yi.Cmb5Out_OtSt, false);//역류용 밸브 닫고.

                    if (OM.CmnOptn.bNotUseNr)
                    {
                        Step.iCycle++;
                        return false;
                    }

                    SEQ.SyringePump.PickupAndDispInc((int)PumpID.FCM, VALVE_POS.Input, VALVE_POS.Output, OM.DevInfo.iCmb6CleanRvsPos, 15); IO_SetY(yi.Cmb6Out_OtSt, true);//역류용 밸브 열고.
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 16:
                    if (!m_tmDelay.OnDelay(300)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.FCM)) return false;
                    if(MM.GetManNo() == mc.NoneCycle)OM.EqpStat.iInspProgress = 84 ;
                    //FCM DC공용 TankCP2.Supply(false);
                    IO_SetY(yi.Cmb6Out_OtSt, false);//역류용 밸브 닫고.                    

                    TankCP3.Supply(true);
                    Cmbr1.FillCP3 (true);
                    Cmbr2.FillCP3 (true);
                    Cmbr3.FillCP3 (!OM.CmnOptn.bNotUseFB  );
                    Cmbr4.FillCP3 (!OM.CmnOptn.bNotUse4DLS);
                    Cmbr5.FillCP3 (!OM.CmnOptn.bNotUseRet );
                    Cmbr6.FillCP3 (!OM.CmnOptn.bNotUseNr  );

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 17:
                    bool bRet1 = m_tmDelay.OnDelay(OM.DevInfo.iCmb1Cp3Time) ;
                    bool bRet2 = m_tmDelay.OnDelay(OM.DevInfo.iCmb2Cp3Time) ;
                    bool bRet3 = m_tmDelay.OnDelay(OM.DevInfo.iCmb3Cp3Time) ;
                    bool bRet4 = m_tmDelay.OnDelay(OM.DevInfo.iCmb4Cp3Time) ;
                    bool bRet5 = m_tmDelay.OnDelay(OM.DevInfo.iCmb5Cp3Time) ;
                    bool bRet6 = m_tmDelay.OnDelay(OM.DevInfo.iCmb6Cp3Time) ;

                    if(!OM.CmnOptn.bNotUseFB  )bRet3 = true;
                    if(!OM.CmnOptn.bNotUse4DLS)bRet4 = true;
                    if(!OM.CmnOptn.bNotUseRet )bRet5 = true;
                    if(!OM.CmnOptn.bNotUseNr  )bRet6 = true;

                    if(bRet1){Cmbr1.FillCP3(false);}//IO_SetY(yi.Cmb1CP3_InSt, true);}
                    if(bRet2){Cmbr2.FillCP3(false);}//IO_SetY(yi.Cmb2CP3_InSt, true);}
                    if(bRet3){Cmbr3.FillCP3(false);}//IO_SetY(yi.Cmb3CP3_InSt, true);}
                    if(bRet4){Cmbr4.FillCP3(false);}//IO_SetY(yi.Cmb4CP3_InSt, true);}
                    if(bRet5){Cmbr5.FillCP3(false);}//IO_SetY(yi.Cmb5CP3_InSt, true);}
                    if(bRet6){Cmbr6.FillCP3(false);}//IO_SetY(yi.Cmb6CP3_InSt, true);}

                    //모두다 끝났으면 
                    if(!bRet1 || !bRet2 || !bRet3 || !bRet4 || !bRet5 || !bRet6) return false ;

                    TankCP3.Supply(false);

                     //1번챔버.==============================================
                     if(MM.GetManNo() == mc.NoneCycle)OM.EqpStat.iInspProgress = 78 ;
                    Cmbr1.WasteOut(true);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;
                case 18:
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iCmbEmptyTime)) return false ;
                    Cmbr1.WasteOut(false);

                    //2번챔버.==============================================
                    if(MM.GetManNo() == mc.NoneCycle)OM.EqpStat.iInspProgress = 81 ;
                    Cmbr2.WasteOut(true);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;
                case 19:
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iCmbEmptyTime)) return false ;
                    Cmbr2.WasteOut(false);

                    //3번챔버.==============================================
                    if(MM.GetManNo() == mc.NoneCycle)OM.EqpStat.iInspProgress = 84 ;
                    if (OM.CmnOptn.bNotUseFB)
                    {
                        Step.iCycle++;
                        return false;
                    }
                    Cmbr3.WasteOut(true);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;
                case 20:
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iCmbEmptyTime)) return false ;
                    Cmbr3.WasteOut(false);

                    //4번챔버.==============================================
                    if(MM.GetManNo() == mc.NoneCycle)OM.EqpStat.iInspProgress = 87 ;
                    if (OM.CmnOptn.bNotUse4DLS)
                    {
                        Step.iCycle++;
                        return false;
                    }
                    Cmbr4.WasteOut(true);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;
                case 21:
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iCmbEmptyTime)) return false ;
                    Cmbr4.WasteOut(false);

                    //5번챔버.==============================================
                    if(MM.GetManNo() == mc.NoneCycle)OM.EqpStat.iInspProgress = 90 ;
                    if (OM.CmnOptn.bNotUseRet)
                    {
                        Step.iCycle++;
                        return false;
                    }
                    Cmbr5.WasteOut(true);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;
                case 22:
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iCmbEmptyTime)) return false ;
                    Cmbr5.WasteOut(false);

                    //6번챔버.==============================================
                    if(MM.GetManNo() == mc.NoneCycle)OM.EqpStat.iInspProgress = 93 ;
                    if (OM.CmnOptn.bNotUseNr)
                    {
                        Step.iCycle++;
                        return false;
                    }
                    Cmbr6.WasteOut(true);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;
               
                case 23:
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iCmbEmptyTime)) return false ;
                    Cmbr6.WasteOut(false);
                    Step.iCycle++;
                    return false ;

                //위에서 씀.
                case 24:
                    //InitValve(); 이시점에 한번 클리어 해주면 좋은데 확인 해서 주석 까자.
                    //여기서는 스페셜 DC클린.
                    //DCC Front 와 Rear 동시에 작업 가능.



                    //Front
                    TankCSF.Supply(true);
                    TankCP2.Supply(true);
                    IO_SetY(yi.Dcc3CSf_InSt , true);
                    IO_SetY(yi.Dcc5CP2_InSt , true);
                    IO_SetY(yi.Dcc2Vcm_InSt , true);

                    //Rear
                    TankCSR.Supply(true);
                    IO_SetY(yi.Dcc4CSR_InSt , true);
                    IO_SetY(yi.Dcc1Vcm_InSt , true);
                    //IO_SetY(yi.Dc1Air_InVt  , true);
                    IO_SetY(yi.Dc2Air_InVt  , true);

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 25:
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iDccCleanTime))return false ;
                    if(MM.GetManNo() == mc.NoneCycle)OM.EqpStat.iInspProgress = 95 ;
                    //Front 동시 잠금.
                    TankCSF.Supply(false);
                    TankCP2.Supply(false);
                    IO_SetY(yi.Dcc3CSf_InSt , false);
                    IO_SetY(yi.Dcc5CP2_InSt , false);
                    IO_SetY(yi.Dcc2Vcm_InSt , false);

                    //Rear Dcc4CSR_InSt잠그고 1초있다가 Air2개와 1번 잠금.
                    TankCSR.Supply(false);
                    IO_SetY(yi.Dcc4CSR_InSt , false); 
                    
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 26:
                    if(!m_tmDelay.OnDelay(2000)) return false ;

                    //Rear
                    IO_SetY(yi.Dcc1Vcm_InSt , false);
                    //IO_SetY(yi.Dc1Air_InVt  , false);
                    IO_SetY(yi.Dc2Air_InVt  , false);


                    IO_SetY(yi.WastOut_OtSt , true);
                    IO_SetY(yi.WastAir_InVc , true);
                    

                    m_tmDelay.Clear();
                    Step.iCycle++;

                    return false ;

                case 27:
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iWasteToExtTime)) return false ;
                    if(MM.GetManNo() == mc.NoneCycle)OM.EqpStat.iInspProgress = 100 ;
                    IO_SetY(yi.WastOut_OtSt , false);
                    IO_SetY(yi.WastAir_InVc , false);

                    if (MM.GetManNo() == mc.CHA_CycleCleanDcc)
                    {
                        Step.iCycle = 0;
                        return true;
                    }

                    if (OM.EqpStat.bWorkOneStop)//한자제만 하고 끄는 옵션시에 작업 종료 되면 끔.
                    {
                        OM.EqpStat.bWorkOneStop = false;
                        SEQ._bBtnStop = true;
                    }


                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 28:
                    if (!m_tmDelay.OnDelay(1000)) return false; //관로에 에어 빼기.. 이거 안하면 니들 클린할때 터짐.
                    if(MM.GetManNo() == mc.NoneCycle)OM.EqpStat.iInspProgress = 0 ; //초기화.
                    //로더 다음꺼 찝게 됨.
                    DM.ARAY[ri.CHA].SetStat(cs.None);
                    
                    Step.iCycle = 0 ;
                    return true ;

               
            }
        
        }

        int iRepeatCnt = 0;
        public bool CycleManReadyDC()
        {
            string sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                ER_SetErr(ei.ETC_CycleTO, sTemp);
                InitValve();

                Trace(sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                Trace(sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //return true ;
            }

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    InitValve();
                    return true;

                case 10:
                    //Step.iCycle=0;
                    //return true;

                    

                    iRepeatCnt = 0;

                    InitValve();

                    //SEQ.SyringePump.YR((int)PumpID.DC);
                    SEQ.SyringePump.AbsMove((int)PumpID.DC, VALVE_POS.Output, 0, 15);
                    //DC쪽 아웃 솔을 켜야 하는데 뭐를 켜야하는지 모르겠음.
                    //DCC쪽에 자체 밴트가 있으면 천천히 켜도 되지만 
                    //DCC쪽에 밴트가 없으면 실린지로 밀때 같이 켜줘야 함.
                    IO_SetY(yi.Dcc1Vcm_InSt, true);
                    IO_SetY(yi.Dcc2Vcm_InSt, true);

                    IO_SetY(yi.Dc1Air_InVt , true);


                    m_tmDelay.Clear();

                    Step.iCycle++;
                    return false;


                //밑에서씀.
                case 11:
                    //펌프 다 밀기 기다림.
                    if (!m_tmDelay.OnDelay(300)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.DC)) return false;

                    //3000까지 빨기.
                    IO_SetY(yi.Dcc1Vcm_InSt, false);
                    IO_SetY(yi.Dcc2Vcm_InSt, false);

                    IO_SetY(yi.Dc1Air_InVt , false);

                    TankCP2.Supply(true);
                    SEQ.SyringePump.AbsMove((int)PumpID.DC, VALVE_POS.Input, 3000, 15);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 12:
                    if (!m_tmDelay.OnDelay(300)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.DC)) return false;
                    TankCP2.Supply(false);

                    IO_SetY(yi.Dcc1Vcm_InSt, true);
                    IO_SetY(yi.Dcc2Vcm_InSt, true);

                    IO_SetY(yi.Dc1Air_InVt , true);

                    TankCSR.Supply(true);
                    IO_SetY(yi.Dcc4CSR_InSt, true);

                    SEQ.SyringePump.AbsMove((int)PumpID.DC, VALVE_POS.Output, 0, 15);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 13:
                    if (!m_tmDelay.OnDelay(300)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.DC)) return false;

                    //에어끄기 2초전에 켠다.
                    TankCSF.Supply(true);
                    IO_SetY(yi.Dcc3CSf_InSt, true);
                    


                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 14:
                    if(!m_tmDelay.OnDelay(2000))return false ;
                    TankCSF.Supply(false);
                    TankCSR.Supply(false);
                    IO_SetY(yi.Dcc3CSf_InSt, false);
                    IO_SetY(yi.Dcc4CSR_InSt, false);

                    IO_SetY(yi.Dcc1Vcm_InSt, false);
                    IO_SetY(yi.Dcc2Vcm_InSt, false);

                    IO_SetY(yi.Dc1Air_InVt , false);
                    if (iRepeatCnt < OM.DevInfo.iDCReadyCnt - 1)
                    {
                        iRepeatCnt++;
                        Step.iCycle = 11;
                        return false;
                    }
                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CycleManReadyFCM()
        {
            string sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                ER_SetErr(ei.ETC_CycleTO, sTemp);
                InitValve();
                Trace(sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                Trace(sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //return true ;
            }

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    InitValve();
                    
                    return true;

                case 10:
                    //Step.iCycle = 0;
                    //return true;
                    iRepeatCnt = 0;
                    InitValve();

                    
                    SEQ.SyringePump.AbsMove((int)PumpID.FCM, VALVE_POS.Output, 0, 15);
                    
                    IO_SetY(yi.FCMOut_OtSt, true);
                    


                    m_tmDelay.Clear();

                    Step.iCycle++;
                    return false;


                //밑에서씀.
                case 11:
                    //펌프 다 밀기 기다림.
                    if (!m_tmDelay.OnDelay(300)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.FCM)) return false;

                    //3000까지 빨기.
                    IO_SetY(yi.FCMOut_OtSt, false);
                    TankCP2.Supply(true);
                    SEQ.SyringePump.AbsMove((int)PumpID.FCM, VALVE_POS.Input, 3000, 15);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 12:
                    if (!m_tmDelay.OnDelay(300)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.FCM)) return false;

                    TankCP2.Supply(false);
                    IO_SetY(yi.FCMOut_OtSt,  true);

                    SEQ.SyringePump.AbsMove((int)PumpID.FCM, VALVE_POS.Output, 0, 15);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 13:
                    if (!m_tmDelay.OnDelay(300)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.FCM)) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 14:
                    if(!m_tmDelay.OnDelay(1000))return false ;
                    IO_SetY(yi.FCMOut_OtSt, false);
                    if (iRepeatCnt < OM.DevInfo.iFCMReadyCnt - 1)
                    {
                        iRepeatCnt++;
                        Step.iCycle = 11;
                        return false;
                    }
                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CycleManReadyNR()
        {
            string sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                ER_SetErr(ei.ETC_CycleTO, sTemp);
                InitValve();

                Trace(sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                Trace(sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //return true ;
            }

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    InitValve();

                    return true;

                case 10:
                    //Step.iCycle = 0;
                    //return true;

                    iRepeatCnt = 0;
                    InitValve();
                    SEQ.SyringePump.AbsMove((int)PumpID.NR, VALVE_POS.Output, 0, 15);
                    IO_SetY(yi.Cmb6Out_VcOt, true);
                    m_tmDelay.Clear();

                    Step.iCycle++;
                    return false;


                //밑에서씀.
                case 11:
                    //펌프 다 밀기 기다림.
                    if (!m_tmDelay.OnDelay(300)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.NR)) return false;

                    //3000까지 빨기.
                    IO_SetY(yi.Cmb6Out_VcOt, false);

                    SEQ.SyringePump.AbsMove((int)PumpID.NR, VALVE_POS.Input, 3000, 15);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 12:
                    if (!m_tmDelay.OnDelay(300)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.NR)) return false;

                    IO_SetY(yi.Cmb6Out_VcOt, true);

                    SEQ.SyringePump.AbsMove((int)PumpID.NR, VALVE_POS.Output, 0, 15);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 13:
                    if (!m_tmDelay.OnDelay(300)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.NR)) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 14:
                    if(!m_tmDelay.OnDelay(1000))return false ;
                    IO_SetY(yi.Cmb6Out_VcOt, false);
                    if (iRepeatCnt < OM.DevInfo.iNRReadyCnt - 1)
                    {
                        iRepeatCnt++;
                        Step.iCycle = 11;
                        return false;
                    }
                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CycleManReadyRET()
        {
            string sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                ER_SetErr(ei.ETC_CycleTO, sTemp);
                InitValve();

                Trace(sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                Trace(sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //return true ;
            }

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    InitValve();

                    return true;

                case 10:
                    //Step.iCycle = 0;
                    //return true;

                    iRepeatCnt = 0;
                    InitValve();
                    SEQ.SyringePump.AbsMove((int)PumpID.RET, VALVE_POS.Output, 0, 15);
                    IO_SetY(yi.Cmb5Out_VcOt, true);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;


                //밑에서씀.
                case 11:
                    //펌프 다 밀기 기다림.
                    if (!m_tmDelay.OnDelay(300)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.RET)) return false;

                    //3000까지 빨기.
                    IO_SetY(yi.Cmb5Out_VcOt, false);

                    SEQ.SyringePump.AbsMove((int)PumpID.RET, VALVE_POS.Input, 3000, 15);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 12:
                    if (!m_tmDelay.OnDelay(300)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.RET)) return false;

                    IO_SetY(yi.Cmb5Out_VcOt, true);

                    SEQ.SyringePump.AbsMove((int)PumpID.RET, VALVE_POS.Output, 0, 15);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 13:
                    if (!m_tmDelay.OnDelay(300)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.RET)) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 14:
                    if(!m_tmDelay.OnDelay(1000))return false ;
                    IO_SetY(yi.Cmb5Out_VcOt, false);
                    if (iRepeatCnt < OM.DevInfo.iRETReadyCnt - 1)
                    {
                        iRepeatCnt++;
                        Step.iCycle = 11;
                        return false;
                    }
                    Step.iCycle = 0;
                    return true;
            }
        }

        public bool CycleManReady4DL()
        {
            string sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                ER_SetErr(ei.ETC_CycleTO, sTemp);
                InitValve();

                Trace(sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                Trace(sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //return true ;
            }

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    InitValve();

                    return true;

                case 10:
                    //Step.iCycle = 0;
                    //return true;
                    iRepeatCnt = 0;
                    InitValve();
                    SEQ.SyringePump.AbsMove((int)PumpID.FDS, VALVE_POS.Output, 0, 15);
                    IO_SetY(yi.Cmb4Out_VcOt, true);
                    m_tmDelay.Clear();

                    Step.iCycle++;
                    return false;


                //밑에서씀.
                case 11:
                    //펌프 다 밀기 기다림.
                    if (!m_tmDelay.OnDelay(300)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.FDS)) return false;

                    //3000까지 빨기.
                    IO_SetY(yi.Cmb4Out_VcOt, false);

                    SEQ.SyringePump.AbsMove((int)PumpID.FDS, VALVE_POS.Input, 3000, 15);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 12:
                    if (!m_tmDelay.OnDelay(300)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.FDS)) return false;

                    IO_SetY(yi.Cmb4Out_VcOt, true);

                    SEQ.SyringePump.AbsMove((int)PumpID.FDS, VALVE_POS.Output, 0, 15);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 13:
                    if (!m_tmDelay.OnDelay(300)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.FDS)) return false;

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 14:
                    if(!m_tmDelay.OnDelay(1000))return false ;
                    IO_SetY(yi.Cmb4Out_VcOt, false);
                    if (iRepeatCnt < OM.DevInfo.i4DLReadyCnt - 1)
                    {
                        iRepeatCnt++;
                        Step.iCycle = 11;
                        return false;
                    }
                    Step.iCycle = 0;
                    return true;
            }
        }

        public int  iTankID ;
        public int  iTime ;
        public bool CycleManTimeSupply()
        {
            string sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                ER_SetErr(ei.ETC_CycleTO, sTemp);
                InitValve();

                Trace(sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                Trace(sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //return true ;
            }

            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    InitValve();

                    return true;

                case 10:

                    //InitValve();
                    //if(iTankID == null)return true ;
                    if (iTankID == 0 ) TankCP1 .Supply(true);
                    if (iTankID == 1) { TankCP2.Supply(true); IO_SetY(yi.Cmb1CP2_InSt, true); IO_SetY(yi.Cmb2CP2_InSt, true); }
                    if (iTankID == 2 ) TankCP3 .Supply(true);
                    if (iTankID == 3 ) TankCSF .Supply(true);
                    if (iTankID == 4 ) TankCSR .Supply(true);
                    if (iTankID == 5 ) TankSULF.Supply(true);
                    if (iTankID == 6 ) TankFB  .Supply(true);
                    if (iTankID == 7 ) Tank4DL .Supply(true);
                    if (iTankID == 8 ) TankRET .Supply(true);
                    if (iTankID == 9 ) TankNR  .Supply(true);


                    //if (iTankID == 0 )
                    //if (iTankID == 1 )
                    //if (iTankID == 2 )
                    //if (iTankID == 3 )
                    //if (iTankID == 4 )
                    //if (iTankID == 5 )
                    //if (iTankID == 6 )
                    //if (iTankID == 7 )
                    //if (iTankID == 8 )
                    //if (iTankID == 9 )

                    m_tmDelay.Clear();

                    Step.iCycle++;
                    return false;


                //밑에서씀.
                case 11:
                    //펌프 다 밀기 기다림.
                    if (!m_tmDelay.OnDelay(iTime)) return false;
                    if (iTankID == 0 ) TankCP1 .Supply(false);
                    if (iTankID == 1 ) TankCP2 .Supply(false);
                    if (iTankID == 2 ) TankCP3 .Supply(false);
                    if (iTankID == 3 ) TankCSF .Supply(false);
                    if (iTankID == 4 ) TankCSR .Supply(false);
                    if (iTankID == 5 ) TankSULF.Supply(false);
                    if (iTankID == 6 ) TankFB  .Supply(false);
                    if (iTankID == 7 ) Tank4DL .Supply(false);
                    if (iTankID == 8 ) TankRET .Supply(false);
                    if (iTankID == 9 ) TankNR  .Supply(false);

                    InitValve();

                    Step.iCycle = 0;
                    return true;
            }
        }

        //메뉴얼 창에서 쓰는 것들.
        //==============================================================================================================================
        public void AllClose()//메뉴얼 동작시에 너무 닫아야 할게 많아서 사람이 먼저 이버튼을 누르고 시작 하는 걸로.
        {
            InitValve();
        }

        public void TankCP1toFCMWST(bool _bOn) //CP1 탱크->FCM->FCMWST->MAINWST
        {
            //IO_SetY(yi.Cmb3Out_OtSt, false);
            //IO_SetY(yi.Cmb4Out_OtSt, false);
            //IO_SetY(yi.Cmb5Out_OtSt, false);
            //IO_SetY(yi.Cmb6Out_OtSt, false);
            InitValve(); //이걸쓰면 동시에 작업이 안될수 있을것 같음.
            
            TankCP1.Supply(_bOn);
            IO_SetY(yi.FCMOut_OtSt , _bOn );
        }

        //FCM웨이스트에서 메인웨이스트로.
        public void FCMWSTtoMAINWST(bool _bOn)
        {
            //IO_SetY(yi.Cmb1Out_VcOt, false);
            //IO_SetY(yi.Cmb2Out_VcOt, false);
            //IO_SetY(yi.Cmb3Out_VcOt, false);
            //IO_SetY(yi.Cmb4Out_VcOt, false);
            //IO_SetY(yi.Cmb5Out_VcOt, false);
            //IO_SetY(yi.Cmb6Out_VcOt, false);
            //
            //IO_SetY(yi.WastAir_InVc, false);
            InitValve();

            IO_SetY(yi.FCMWOut_OtSt, _bOn);

        }

        //메인웨이스트 비우기.
        public void MainWtoExtW(bool _bOn)
        {
            ////챔버나가는쪽 닫고
            //IO_SetY(yi.Cmb1Out_VcOt, false);
            //IO_SetY(yi.Cmb2Out_VcOt, false);
            //IO_SetY(yi.Cmb3Out_VcOt, false);
            //IO_SetY(yi.Cmb4Out_VcOt, false);
            //IO_SetY(yi.Cmb5Out_VcOt, false);
            //IO_SetY(yi.Cmb6Out_VcOt, false);
            //
            ////메인웨이스트에 연결되어 있는 벨브 다잠그고.
            //IO_SetY(yi.Dcc1Vcm_InSt, false);
            //IO_SetY(yi.Dcc2Vcm_InSt, false);
            //IO_SetY(yi.NidlOut_OtSt, false);
            //IO_SetY(yi.HGBOut_OtSt , false);
            //IO_SetY(yi.FCMWOut_OtSt, false);

            InitValve();
            
            IO_SetY(yi.WastAir_InVc, _bOn);
            IO_SetY(yi.WastOut_OtSt, _bOn);

        }


        public void TankCP2toMainWST (bool _bOn) //CP2 tank->Chmber1->mainwast
        {
            //IO_SetY(yi.WastAir_InVc, false);
            //IO_SetY(yi.WastOut_OtSt, false);
            InitValve();

            TankCP2.Supply(_bOn);
            IO_SetY(yi.Cmb1CP2_InSt, _bOn);
            IO_SetY(yi.Cmb1Out_VcOt, _bOn);


        }

        public void TankCP3toMainWST (bool _bOn)//CP2 tank->Chmber1->mainwast
        {
            //IO_SetY(yi.WastAir_InVc, false);
            //IO_SetY(yi.WastOut_OtSt, false);
            InitValve();

            TankCP3.Supply(_bOn);
            IO_SetY(yi.Cmb1CP3_InSt, _bOn);
            IO_SetY(yi.Cmb1Out_VcOt, _bOn);

            IO_SetY(yi.Cmb2CP3_InSt, _bOn);
            IO_SetY(yi.Cmb2Out_VcOt, _bOn);

            IO_SetY(yi.Cmb3CP3_InSt, _bOn);
            IO_SetY(yi.Cmb3Out_VcOt, _bOn);

            IO_SetY(yi.Cmb4CP3_InSt, _bOn);
            IO_SetY(yi.Cmb4Out_VcOt, _bOn);

            IO_SetY(yi.Cmb5CP3_InSt, _bOn);
            IO_SetY(yi.Cmb5Out_VcOt, _bOn);

            IO_SetY(yi.Cmb6CP3_InSt, _bOn);
            IO_SetY(yi.Cmb6Out_VcOt, _bOn);

        }

        public void TankCSFtoMainWST (bool _bOn)
        {
            //IO_SetY(yi.WastAir_InVc, false);
            //IO_SetY(yi.WastOut_OtSt, false);
            //아웃쪽 배관 잠그고.
            //IO_SetY(yi.NidlOut_OtSt, false);
            //IO_SetY(yi.HGBOut_OtSt , false);
            //IO_SetY(yi.FCMWOut_OtSt, false);
            //
            ////FCM관련 배관 잠그고.
            //IO_SetY(yi.Cmb3Out_VcOt, false);
            //IO_SetY(yi.Cmb4Out_VcOt, false);
            //IO_SetY(yi.Cmb5Out_VcOt, false);
            //IO_SetY(yi.Cmb6Out_VcOt, false);

            InitValve();            

            TankCSF.Supply(_bOn);
            IO_SetY(yi.Dcc1Vcm_InSt, _bOn);
            IO_SetY(yi.Dcc2Vcm_InSt, _bOn);

            IO_SetY(yi.Dcc3CSf_InSt, _bOn);
        }

        public void TankCSRtoMainWST (bool _bOn)
        {
            //IO_SetY(yi.WastAir_InVc, false);
            //IO_SetY(yi.WastOut_OtSt, false);
            InitValve();

            //아웃쪽 배관 잠그고.
            IO_SetY(yi.NidlOut_OtSt, false);
            IO_SetY(yi.HGBOut_OtSt , false);
            IO_SetY(yi.FCMWOut_OtSt, false);

            //FCM관련 배관 잠그고.
            IO_SetY(yi.Cmb3Out_VcOt, false);
            IO_SetY(yi.Cmb4Out_VcOt, false);
            IO_SetY(yi.Cmb5Out_VcOt, false);
            IO_SetY(yi.Cmb6Out_VcOt, false);

            TankCSR.Supply(_bOn);
            //IO_SetY(yi.Dcc1Vcm_InSt, _bOn);
            ////IO_SetY(yi.Dcc2Vcm_InSt, _bOn);
            //
            //IO_SetY(yi.Dcc4CSR_InSt, _bOn);
            


            //=============================
            IO_SetY(yi.Dc1Air_InVt, _bOn);
            IO_SetY(yi.Dc2Air_InVt, _bOn);

            IO_SetY(yi.Dcc1Vcm_InSt , _bOn);
            //IO_SetY(yi.Dcc2Vcm_InSt , _bOn);
            //IO_SetY(yi.Dcc3CSf_InSt , _bOn);
            IO_SetY(yi.Dcc4CSR_InSt , _bOn);
            //IO_SetY(yi.Dcc5CP2_InSt , _bOn);



            //IO_SetY(yi.dc)
        }

        public void TankSULFtoMainWST(bool _bOn)
        {
            //IO_SetY(yi.WastAir_InVc, false);
            //IO_SetY(yi.WastOut_OtSt, false);
            InitValve();

            TankSULF.Supply(_bOn);
            IO_SetY(yi.Cmb2Out_VcOt, _bOn);
        }

        public void TankFBtoMainWST  (bool _bOn)
        {
            //IO_SetY(yi.WastAir_InVc, false);
            //IO_SetY(yi.WastOut_OtSt, false);
            InitValve();

            TankFB.Supply(_bOn);
            IO_SetY(yi.Cmb3Out_VcOt, _bOn);
        }

        public void Tank4DLtoMainWST (bool _bOn)
        {
            //IO_SetY(yi.WastAir_InVc, false);
            //IO_SetY(yi.WastOut_OtSt, false);
            InitValve();

            Tank4DL.Supply(_bOn);
            IO_SetY(yi.Cmb4Out_VcOt, _bOn);
        }

        public void TankRETtoMainWST (bool _bOn)
        {
            //IO_SetY(yi.WastAir_InVc, false);
            //IO_SetY(yi.WastOut_OtSt, false);
            InitValve();

            TankRET.Supply(_bOn);
            IO_SetY(yi.Cmb5Out_VcOt, _bOn);
        }

        public void TankNRtoMainWST  (bool _bOn)
        {
            //IO_SetY(yi.WastAir_InVc, false);
            //IO_SetY(yi.WastOut_OtSt, false);
            InitValve();

            TankNR.Supply(_bOn);
            IO_SetY(yi.Cmb6Out_VcOt, _bOn);
        }

        //public void MainWtoExtW(bool _bOn)
        //{
        //    IO_SetY(yi.Cmb1Out_VcOt, false);
        //    IO_SetY(yi.Cmb2Out_VcOt, false);
        //    IO_SetY(yi.Cmb3Out_VcOt, false);
        //    IO_SetY(yi.Cmb4Out_VcOt, false);
        //    IO_SetY(yi.Cmb5Out_VcOt, false);
        //    IO_SetY(yi.Cmb6Out_VcOt, false);
        //
        //    IO_SetY(yi.Dcc1Vcm_InSt, false);
        //    IO_SetY(yi.Dcc2Vcm_InSt, false);
        //    IO_SetY(yi.NidlOut_OtSt, false);
        //    IO_SetY(yi.HGBOut_OtSt, false);
        //    IO_SetY(yi.FCMWOut_OtSt, false);
        //
        //    if (_bOn)
        //    {
        //        IO_SetY(yi.WastAir_InVc, true);
        //        IO_SetY(yi.WastOut_OtSt, true);
        //    }
        //    else
        //    {
        //        IO_SetY(yi.FCMOut_OtSt, false);
        //        IO_SetY(yi.WastOut_OtSt, false);
        //    }
        //}

        public bool CheckSafe(ci _eActr, fb _eFwd, bool _bChecked = false)
        {
            if (CL_Complete(_eActr, _eFwd)) return true;

            String sMsg = "";
            bool bRet = true;

            if (_eActr == ci.CHA_MixCoverOpCl)
            {
                bool bZUnderMove = MT_GetCmdPos(mi.SYR_ZSyrg) > PM.GetValue(mi.SYR_ZSyrg,pv.SYR_ZSyrgMove)+0.5 ;
                bool bXOverChamber = MT_GetCmdPos(mi.SYR_XSyrg) > PM.GetValue(mi.SYR_XSyrg,pv.SYR_XSyrgClean)+0.5 ;
                if(bZUnderMove && bXOverChamber)
                {
                    sMsg = "혈액주사기가 챔버뚜껑과 충돌위치에 있습니다.";
                    bRet = false ;
                }
            }
            else if (!_bChecked)
            {
                sMsg = "Cylinder " + CL_GetName(_eActr) + " is Not this parts.";
                bRet = false;
            }

            if (!bRet)
            {
                m_sCheckSafeMsg = sMsg;
                Trace(CL_GetName(_eActr) + " " + sMsg);
                if (Step.iCycle==0) Log.ShowMessage(CL_GetName(_eActr), sMsg);
            }
            else
            {
                m_sCheckSafeMsg = "";
            }

            return bRet;
        }

        public bool CheckSafe(mi _eMotr, pv _ePstn, double _dOfsPos = 0)
        {
            if (OM.MstOptn.bDebugMode) return true;
            double dDstPos = PM_GetValue(_eMotr, _ePstn) + _dOfsPos;
            if (MT_CmprPos(_eMotr, dDstPos)) return true;

            bool bRet = true;
            string sMsg = "";

            //if (_eMotr == mi.LDR_XBelt)
            //{
            //
            //}
            //else
            {
                sMsg = "Motor " + MT_GetName(_eMotr) + " is Not this parts.";
                bRet = false;
            }

            if (!bRet)
            {
                m_sCheckSafeMsg = sMsg;
                Trace(MT_GetName(_eMotr) + " " + sMsg);
                //메뉴얼 동작일때.
                if (Step.eSeq == sc.Idle) Log.ShowMessage(MT_GetName(_eMotr), sMsg);
            }
            else
            {
                m_sCheckSafeMsg = "";
            }

            return bRet;
        }

        public bool JogCheckSafe(mi _eMotr, EN_JOG_DIRECTION _eDir, EN_UNIT_TYPE _eType, double _dDist)
        {
            if (OM.MstOptn.bDebugMode) return true;
            bool bRet = true;
            string sMsg = "";

            //if (_eMotr == mi.LDR_XBelt)
            //{
            //    if (_eDir == EN_JOG_DIRECTION.Neg) //아래
            //    {
            //
            //        if (_eType == EN_UNIT_TYPE.utJog)
            //        {
            //
            //        }
            //        else if (_eType == EN_UNIT_TYPE.utMove)
            //        {
            //
            //        }
            //    }
            //    else //위
            //    {
            //        if (_eType == EN_UNIT_TYPE.utJog)
            //        {
            //
            //        }
            //        else if (_eType == EN_UNIT_TYPE.utMove)
            //        {
            //
            //        }
            //    }
            //}
            //else
            {
                sMsg = "Motor " + MT_GetName(_eMotr) + " is Not this parts.";
                bRet = false;
            }

            if (!bRet)
            {
                m_sCheckSafeMsg = sMsg;
                Trace(MT_GetName(_eMotr) + " " + sMsg);
                //메뉴얼 동작일때.
                if (Step.eSeq == sc.Idle) Log.ShowMessage(MT_GetName(_eMotr), sMsg);
            }
            else
            {
                m_sCheckSafeMsg = "";
            }

            return bRet;
        }

        public bool MoveMotrSlow(mi _eMotr , pv _ePstn ,  double _dOfsPos=0)
        {
            if (!CheckSafe(_eMotr, _ePstn , _dOfsPos)) return false;

            double dDstPos = PM_GetValue(_eMotr,_ePstn) + _dOfsPos;
            MT_GoAbsSlow(_eMotr, dDstPos); 
            return true ;

        }

        //무브함수들의 리턴이 Done을 의미 한게 아니고 명령 전달이 됐는지 여부로 바꿈.
        //Done 확인을 위해서는 GetStop을 써야함.
        public bool MoveMotr(mi _eMotr , pv _ePstn ,  double _dOfsPos=0, bool _bLink = true)
        {
            if (!CheckSafe(_eMotr, _ePstn , _dOfsPos)) return false;

            double dDstPos = PM_GetValue(_eMotr,_ePstn) + _dOfsPos;
            int    iSpdPer = PM_GetValueSpdPer(_eMotr, _ePstn);

            if (Step.iCycle!=0) MT_GoAbsRun(_eMotr , dDstPos, iSpdPer);
            else                MT_GoAbsMan(_eMotr , dDstPos);

            return true ;        
        }

        //무브함수들의 리턴이 Done을 의미 한게 아니고 명령 전달이 됐는지 여부로 바꿈.
        //Done 확인을 위해서는 GetStop을 써야함.
        public bool MoveCyl(ci _eActr, fb _eFwd)
        {
            if (!CheckSafe(_eActr, _eFwd)) return false;

            CL_Move(_eActr, _eFwd);
            
            return true;
        }

        public bool CheckStop()
        {
            //if( !MT_GetStop(mi.LDR_XBelt)) return false;
           
            
            if (!CL_Complete(ci.CHA_MixCoverOpCl)) return false;

            if(m_tmDelay.GetUsing())return false ;

            if (SEQ.SyringePump.GetBusy((int)PumpID.Blood)) return false;
            if (SEQ.SyringePump.GetBusy((int)PumpID.DC   )) return false;
            if (SEQ.SyringePump.GetBusy((int)PumpID.FCM  )) return false;
            if (SEQ.SyringePump.GetBusy((int)PumpID.FDS  )) return false;
            if (SEQ.SyringePump.GetBusy((int)PumpID.NR   )) return false;
            if (SEQ.SyringePump.GetBusy((int)PumpID.RET  )) return false;

            return true ;
        }

        public void Trace(string _name, params bool[] _val)
        {
            string sLog = _name + " = ";
            for(int i=0; i<_val.Length; i++) sLog += _val[i].ToString() + ",";
            Trace(sLog);
        }
        public void Trace(string _sMsg, [CallerLineNumber] int sourceLineNumber = 0, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "")
        {
            string sHdr = m_sPartName.Replace(",", "");
            string sMsg = _sMsg.Replace(",", "");
            string sTag = string.Format("{0:00}", m_iPartId);
            string sFullMsg = string.Format("{0}, {1} ,{2},{3},{4}", sTag, sHdr + " " + sMsg, sourceLineNumber, memberName, sourceFilePath);
            Log.SendMessage(sFullMsg);
        }       



        //실린지에서 데려다 쓰는 함수.
        //-------------------------------------------------------------------------------------------------------
        public void NiddleClean(bool _bOn)
        {
            TankCP3.Supply(_bOn);
            IO_SetY(yi.NidlCP3_InSt , _bOn);
            IO_SetY(yi.NidlOut_OtSt , _bOn);
        }

        public void WastClean(bool _bOn)
        {
            IO_SetY(yi.ClenCP3_InSt , _bOn);
            IO_SetY(yi.ClenOut_OtSt , _bOn); //이건 오프 딜레이 넣자.
        }
        
    };

    /* 원래 함수.
    bool bHGBRet = false ;
        public bool CycleInspChamber()
        {
            string sTemp;
            if (m_tmCycle.OnDelay(Step.iCycle != 0 && Step.iCycle == PreStep.iCycle && CheckStop() && !OM.MstOptn.bDebugMode, 10000))
            {
                sTemp = string.Format("Time Out Step.iCycle={0:00}", Step.iCycle);
                sTemp = m_sPartName + " " + Step.eSeq.ToString() + sTemp;
                ER_SetErr(ei.ETC_CycleTO, sTemp);
                InitValve();
                IO_SetY(yi.CHA_FcmLaserOn, false);

                Trace(sTemp);
                return true;
            }

            if (Step.iCycle != PreStep.iCycle)
            {
                sTemp = string.Format("Cycle Step.iCycle={0:00}", Step.iCycle);
                Trace(sTemp);
            }

            PreStep.iCycle = Step.iCycle;

            if (Stat.bReqStop)
            {
                //return true ;
            }


            //스페샬. 서브 사이클 타이밍 차트상 FCM3,4,5,6 랑 동시에 DC HGB를 같이 해야해서 넣음.
            //챔버 혈액투입후 검사가능시간.
            //1    15~30
            //2    10~60
            //3    14~30
            //4    30~60
            //5    30~60
            //6    15~30
            //* 타이밍 차트상으로 혈액 투입 순서는 2,1,4,5,6,3
            //* 검사순서는 1,2,6->3->4->5

            //return CycleInspChamberSubHGB();

            //DM.ARAY[ri.CHA].CheckStat(1,0,cs.Work) ;
            //DC에 같이 돌림.
            if (Step.iCycle >= 100 && Step.iCycle < 200 && Step.iCycleSubHgb != 0)// && DM.ARAY[ri.CHA].CheckStat(1,0,cs.Work))
            {
                bHGBRet = CycleInspChamberSubHGB();
            }


            switch (Step.iCycle)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear Step.iCycle={0:00}", Step.iCycle);
                    InitValve();
                    IO_SetY(yi.CHA_FcmLaserOn, false);
                    return true ;

                case 10:
                    if(OM.CmnOptn.bNotUseInsp)
                    {
                        //기냥 한방에 가자... 귀찮다.
                        DM.ARAY[ri.CHA].SetStat(cs.None);
                        MoveCyl(ci.CHA_MixCoverOpCl, fb.Bwd);
                        Step.iCycle=0;
                        return true;
                    }


                    if(DM.ARAY[ri.CHA].CheckAllStat(cs.Work)) 
                    {
                        InitValve();                        
                    }

                    

                    SEQ.FcmTester.SendReqStatus();

                    //이때쯤 혈액 남은거 버리느라 클린쪽에서 내려 가고 있어서 이거 하면 안됨.
                    //SEQ.SYR.MoveMotr(mi.SYR_ZSyrg , pv.SYR_ZSyrgMove);
                    Step.iCycle++;
                    return false ;

                case 11:
                    //나중에 풀자.
                    if(!OM.CmnOptn.bIgnrFCMTester)
                    {
                        if (!SEQ.FcmTester.RcvdMsg) return false;
                        if (SEQ.FcmTester.State == TCPIP_NewOpticsFCM.EState.Running)
                        {
                            SEQ.FcmTester.SendReqStatus(); //10으로 보내지 말고 11에 머물게 해서 타임아웃 유도.
                            return false;
                        }
                        else if (SEQ.FcmTester.State == TCPIP_NewOpticsFCM.EState.Error)
                        {
                            ER_SetErr(ei.ETC_Communicate, "FCM Tester 에러상태");
                            return true;
                        }
                    }

                    //첫 검사시 시퀜스 확인 전송.
                    if (DM.ARAY[ri.CHA].CheckAllStat(cs.Work)) 
                    {
                        Log.TraceListView("NI-Start : "+OM.EqpStat.sBloodID);
                        SEQ.FcmTester.SendTestSeq(TCPIP_NewOpticsFCM.ETestSeq.Start, OM.EqpStat.sBloodID);
                    }

                    Step.iCycle++;
                    return false;

                case 12:
                    if(!OM.CmnOptn.bIgnrFCMTester && DM.ARAY[ri.CHA].CheckAllStat(cs.Work))
                    {
                        if (!SEQ.FcmTester.RcvdMsg) return false;                        
                    }

                    MoveCyl(ci.CHA_MixCoverOpCl , fb.Bwd);

                    

                    Step.iCycle++;
                    return false ;

                case 13:
                    if(!CL_Complete(ci.CHA_MixCoverOpCl , fb.Bwd))return false ;

                    //서브사이클로 뺌.
                    //else if(DM.ARAY[ri.CHA].CheckStat(1,0,cs.Work)) 
                    //{
                    //    Step.iCycle = 200 ;
                    //    return false ;
                    //}

                    //DC테스트.
                    if (MM.GetManNo() == mc.CHA_CycleInspDC)
                    {
                        Step.iCycle = 100;
                        return false;

                    }
                    //Step.iCycle = 100;
                    //return false;

                    //1,2->6->3->4->5   NI쪽 연결되어 있는것은 순차로 검사해야함.  DC , FCM 
                    //일단 테스트 완료 하고 연속동작으로 하게 수정하자.

                    bool bNeedHGB = DM.ARAY[ri.CHA].CheckStat(1,0,cs.Work) ;
                  

                         if(DM.ARAY[ri.CHA].CheckStat(0,0,cs.Work)) {Step.iCycle = 100; Step.iCycleSubHgb = 10 ; OM.EqpStat.iInspProgress = 10 ;}//헤모글로빈  DC는 같이 함.
                    else if(DM.ARAY[ri.CHA].CheckStat(5,0,cs.Work)) {Step.iCycle = 600;                          OM.EqpStat.iInspProgress = 20 ;}
                    else if(DM.ARAY[ri.CHA].CheckStat(2,0,cs.Work)) {Step.iCycle = 300;                          OM.EqpStat.iInspProgress = 30 ;}
                    else if(DM.ARAY[ri.CHA].CheckStat(3,0,cs.Work)) {Step.iCycle = 400;                          OM.EqpStat.iInspProgress = 40 ;}
                    else if(DM.ARAY[ri.CHA].CheckStat(4,0,cs.Work)) {Step.iCycle = 500;                          OM.EqpStat.iInspProgress = 50 ;}

                    if(MM.GetManNo() != mc.NoneCycle)OM.EqpStat.iInspProgress = 0 ;


                    return false ;

                //DC=====================================================
                case 100:
                    //==============================================================================================
                    //iCmb1DeadVol < iCmb1DCSylPos 이 조건이 되어야 시퀜스 가능.
                    //삼거리전까지 챔버용량빨기 -> 빨은만큼 버리기. -> 검사용량 빨기 -> 데드볼륨밀기 -> cp2대드볼륨용량만큼 빨기 -> 검사용량만큼 밀기.
                    //==============================================================================================

                    //삼거리전까지 챔버용량빨기================================================================
                    Cmbr1.InspSupply(true);
                    SEQ.SyringePump.PickupIncPos((int)PumpID.DC, VALVE_POS.Output, OM.DevInfo.iCmb1ToInter, OM.DevInfo.iDeadVolSpd); //느리게 할려면 크게                                   

                    m_tmDelay.Clear();

                    Step.iCycle++;
                    return false;

                case 101://시리얼 통신 렉 기다려줌.
                    if (!m_tmDelay.OnDelay(300)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.DC)) return false;

                    Cmbr1.InspSupply(false); //챔버 닫고.

                    //삼거리 까지 데드볼륨밀기 DC쪽 2,3번 열고 밀어낸다.===================================================
                    TankCSF.Supply(true);
                    IO_SetY(yi.Dcc3CSf_InSt, true);
                    IO_SetY(yi.Dcc2Vcm_InSt, true);
                    SEQ.SyringePump.DispIncPos((int)PumpID.DC, VALVE_POS.Output, OM.DevInfo.iCmb1ToInter, OM.DevInfo.iDeadVolSpd); //느리게 할려면 크게      \

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;


                case 102: //통신렉
                    if (!m_tmDelay.OnDelay(300)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.DC)) return false;
                    TankCSF.Supply(false);
                    IO_SetY(yi.Dcc3CSf_InSt, false);
                    IO_SetY(yi.Dcc2Vcm_InSt, false);


                    //검사용량 빨기==============================================================================                 
                    Cmbr1.InspSupply(true);                                          
                    SEQ.SyringePump.PickupIncPos((int)PumpID.DC , VALVE_POS.Output , OM.DevInfo.iCmb1DCSylPos , OM.DevInfo.iDeadVolSpd); //느리게 할려면 크게                                   
                    
                    m_tmDelay.Clear();

                    Step.iCycle++;
                    return false ;

                case 103://시리얼 통신 렉 기다려줌.
                    if(!m_tmDelay.OnDelay(300))return false ; 
                    if(SEQ.SyringePump.GetBusy((int)PumpID.DC )) return false; 

                    Cmbr1.InspSupply(false); //챔버 닫고.

                    //데드볼륨밀기 DC쪽 2,3번 열고 밀어낸다.===================================================
                    //밀때 해야할놈들.
                    TankCSF.Supply(true);
                    IO_SetY(yi.Dcc3CSf_InSt , true);
                    IO_SetY(yi.Dcc2Vcm_InSt , true);

                    //빨때 해야할 놈들.
                    TankCP2.Supply(true); //CP2탱크열고.

                    SEQ.SyringePump.DispAndPickupInc((int)PumpID.DC , VALVE_POS.Output , VALVE_POS.Input , OM.DevInfo.iCmb1DeadVol , OM.DevInfo.iDeadVolSpd , OM.DevInfo.iCmb1DeadTimes); //느리게 할려면 크게      \

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;


                case 104: //통신렉
                    if(!m_tmDelay.OnDelay(300))return false ;
                    if(SEQ.SyringePump.GetBusy((int)PumpID.DC )) return false; 
                    //밀때 쓴놈들.
                    TankCSF.Supply(false);
                    IO_SetY(yi.Dcc3CSf_InSt , false);
                    IO_SetY(yi.Dcc2Vcm_InSt , false);

                    //빨때 쓴놈들.
                    TankCP2.Supply(false); //CP2탱크 닫고

                    Step.iCycle++;
                    return false;

                    //cp2대드볼륨용량만큼 빨기================================================================

                    //    TankCP2.Supply(true); //CP2탱크열고.
                    //    SEQ.SyringePump.PickupIncPos((int)PumpID.DC , VALVE_POS.Input , OM.DevInfo.iCmb1DeadVol , OM.DevInfo.iDeadVolSpd); //느리게 할려면 크게      \
                    //    m_tmDelay.Clear();
                    //
                    //    Step.iCycle++;
                    //    return false ;
                    //
                    //case 105://다빨았는지 확인.
                    //    if(!m_tmDelay.OnDelay(300))return false ; 
                    //    if(SEQ.SyringePump.GetBusy((int)PumpID.DC )) return false; 
                    //
                    //    TankCP2.Supply(false); //CP2탱크 닫고

                    //실제 검사.=====================================================================
                case 105:
                    TankCSR.Supply(true);
                    TankCSF.Supply(true);

                    //IO_SetY(yi.Dcc2Vcm_InSt , true); 2번은 크리닝시에만.
                    IO_SetY(yi.Dcc1Vcm_InSt , true);
                    IO_SetY(yi.Dcc3CSf_InSt , true);
                    IO_SetY(yi.Dcc4CSR_InSt , true);
                    //IO_SetY(yi.Dcc5CP2_InSt , true);

                    //Dc2Air_InVt는 청소할때씀.
                    IO_SetY(yi.Dc1Air_InVt  , true);
                    IO_SetY(yi.WastAir_InVc , false);

                    //피도 넣어줌.
                    SEQ.SyringePump.DispIncPos((int)PumpID.DC , VALVE_POS.Output , OM.DevInfo.iCmb1DCSylPos , OM.DevInfo.iCmb1DCSylSpdCode); //속도 느리게 할려면 크게
                    Log.TraceListView("DC Sylinge Start");

                    //FCM탱크가 너무 작어서..
                    IO_SetY(yi.FCMWOut_OtSt, true);
                    

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 106:
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iFCMTestStartDelay))return false ; //어느정도 주입 되고 나서 검사 시작.
                    //FCM탱크가 너무 작어서..
                    IO_SetY(yi.FCMWOut_OtSt, false);
                    //검사 시그널..
                    SEQ.FcmTester.SendTestSub(TCPIP_NewOpticsFCM.ETest.DC_TEST);
                    Log.TraceListView("NI-DC_TEST");

                    Step.iCycle++;
                    return false ;

                case 107:
                    if (!OM.CmnOptn.bIgnrFCMTester &&!SEQ.FcmTester.RcvdMsg)return false ;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 108:
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iFCMTestEndDelay))return false ;
                    if(SEQ.SyringePump.GetBusy((int)PumpID.DC))return false ; //대략 25초 검사해야함.
                    
                    Log.TraceListView("DC Sylinge End");

                    TankCSR.Supply(false);
                    TankCSF.Supply(false);

                    
                    IO_SetY(yi.Dcc3CSf_InSt , false);
                    IO_SetY(yi.Dcc4CSR_InSt , false);
                    //IO_SetY(yi.Dcc5CP2_InSt , false);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 109:
                    if (!m_tmDelay.OnDelay(1000)) return false;//0.5초 있다 닫음.

                    IO_SetY(yi.Dcc1Vcm_InSt, false);

                    //Dc2Air_InVt는 청소할때씀.
                    IO_SetY(yi.Dc1Air_InVt  , false);

                    //검사끝 마스킹 하고.
                    DM.ARAY[ri.CHA].SetStat(0,0,cs.WorkEnd);

                    if (MM.GetManNo() == mc.CHA_CycleInspDC)
                    {
                        Step.iCycle = 0;
                        return true;

                    }

                    //마지막 검사였으면 검사끝 전송.
                    if(DM.ARAY[ri.CHA].CheckAllStat(cs.WorkEnd)){SEQ.FcmTester.SendTestSeq(TCPIP_NewOpticsFCM.ETestSeq.End , ""); Log.TraceListView("NI-End");}
                    Step.iCycle++;
                    return false;

                case 110:
                    if (DM.ARAY[ri.CHA].CheckAllStat(cs.WorkEnd) && !OM.CmnOptn.bIgnrFCMTester && !SEQ.FcmTester.RcvdMsg) return false;
                    Step.iCycle++;
                    return false ;

                case 111:
                    if(Step.iCycleSubHgb != 0)return false ;//HGB할때기다리기.

                    Step.iCycle=0;
                    return true ;

                //3번챔버 FB =====================================================
                case 300:
                    //==============================================================================================
                    //iCmb1DeadVol < iCmb1DCSylPos 이 조건이 되어야 시퀜스 가능.
                    //삼거리전까지 챔버용량빨기 -> 빨은만큼 버리기. -> 검사용량 빨기 -> 데드볼륨밀기 -> cp2대드볼륨용량만큼 빨기 -> 검사용량만큼 밀기.
                    //==============================================================================================
                    if (OM.CmnOptn.bNotUseFB)
                    {
                        Step.iCycle = 310;
                        return false;
                    }
                    Cmbr3.InspSupply(true); //챔버 열고.
                    SEQ.SyringePump.PickupIncPos((int)PumpID.FCM, VALVE_POS.Output, OM.DevInfo.iCmb3ToInter, OM.DevInfo.iDeadVolSpd); //삼거리까지 담기.
                    m_tmDelay.Clear();

                    Step.iCycle++;
                    return false;

                case 301://시리얼 통신 렉 기다려줌.
                    if (!m_tmDelay.OnDelay(300)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.FCM)) return false;
                    Cmbr3.InspSupply(false); //챔버 닫고.

                    IO_SetY(yi.FCMOut_OtSt, true);//FCM밑에 노즐.
                    SEQ.SyringePump.DispIncPos((int)PumpID.FCM, VALVE_POS.Output, OM.DevInfo.iCmb3ToInter, OM.DevInfo.iDeadVolSpd); //삼거리 까지 빤것 일단 뱉고.          

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 302://다빨았는지 확인.
                    if (!m_tmDelay.OnDelay(300)) return false;  //잠시 기다렸다가.
                    if (SEQ.SyringePump.GetBusy((int)PumpID.FCM)) return false;
                    IO_SetY(yi.FCMOut_OtSt, false);

                    //혈액 빨기.
                    Cmbr3.InspSupply(true); //챔버 열고.
                    SEQ.SyringePump.PickupIncPos((int)PumpID.FCM , VALVE_POS.Output , OM.DevInfo.iCmb3FCMSylPos , OM.DevInfo.iDeadVolSpd); //혈액샘플 담기.
                    m_tmDelay.Clear();

                    Step.iCycle++;
                    return false ;

                case 303://시리얼 통신 렉 기다려줌.
                    if(!m_tmDelay.OnDelay(300))return false ; 
                    if(SEQ.SyringePump.GetBusy((int)PumpID.FCM )) return false;
                    Cmbr3.InspSupply(false); //챔버 닫고.

                    //뱉을때 쓰는 놈.
                    IO_SetY(yi.FCMOut_OtSt , true);//FCM밑에 노즐.

                    //빨때 쓰는 놈.
                    TankCP2.Supply(true);
                    //SEQ.SyringePump.DispIncPos((int)PumpID.FCM , VALVE_POS.Output , OM.DevInfo.iCmb3DeadVol , OM.DevInfo.iDeadVolSpd); //대드 볼륨 밀어내기.          
                    SEQ.SyringePump.DispAndPickupInc((int)PumpID.FCM, VALVE_POS.Output , VALVE_POS.Input , OM.DevInfo.iCmb3DeadVol, OM.DevInfo.iDeadVolSpd , OM.DevInfo.iCmb3DeadTimes); //느리게 할려면 크게      \


                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 304://다빨았는지 확인.
                    if(!m_tmDelay.OnDelay(300))return false ;  //잠시 기다렸다가.
                    if(SEQ.SyringePump.GetBusy((int)PumpID.FCM )) return false;

                    //뱉을때 쓰는 놈.
                    IO_SetY(yi.FCMOut_OtSt , false);

                    //빨때 쓰던놈.
                    TankCP2.Supply(false);
                    Step.iCycle++;
                    return false;

                //    TankCP2.Supply(true);
                //    SEQ.SyringePump.PickupIncPos((int)PumpID.FCM , VALVE_POS.Input , OM.DevInfo.iCmb3DeadVol , OM.DevInfo.iDeadVolSpd); //대드볼륨 밀어낼꺼 담기.   
                //    m_tmDelay.Clear();
                //    Step.iCycle++;
                //    return false ;
                //
                //case 305://다빨았는지 확인.
                //    if(!m_tmDelay.OnDelay(300))return false ; 
                //    if(SEQ.SyringePump.GetBusy((int)PumpID.FCM )) return false;
                //    TankCP2.Supply(false);


                //검사시작=========================================================================
                case 305:
                    //검사시에 CP1 먼저 투여.
                    IO_SetY(yi.FCMOut_OtSt , true);
                    TankCP1.Supply(true); //CP1 먼저 공급. 먼저cP1을 공급.

                    //LD On
                    IO_SetY(yi.CHA_FcmLaserOn, true);

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 306:
                    if(!m_tmDelay.OnDelay(1000))return false ; //CP1부터 투여 하고 1초 있다가.
                    SEQ.SyringePump.DispIncPos((int)PumpID.FCM , VALVE_POS.Output , OM.DevInfo.iCmb3FCMSylPos , OM.DevInfo.iCmb3FCMSylSpdCode); //샘플 FCM으로 밀어넣기.  
                    Log.TraceListView("FCM3 Sylinge Start");
                    
                    //FCM탱크가 너무 작어서..
                    IO_SetY(yi.FCMWOut_OtSt, true);

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 307:
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iFCMTestStartDelay))return false ; //혈액샘플이 FCM에 들어가게 잠깐 쉬었다. 

                    //FCM탱크가 너무 작어서..
                    IO_SetY(yi.FCMWOut_OtSt, false);

                    //검사기에게 프로토콜.
                    if (OM.CmnOptn.bAutoQc) {SEQ.FcmTester.SendTestSub(TCPIP_NewOpticsFCM.ETest.QC_FCM_1_TEST);Log.TraceListView("NI-QC_FCM_1_TEST");}
                    else                    {SEQ.FcmTester.SendTestSub(TCPIP_NewOpticsFCM.ETest.FCM_1_TEST   );Log.TraceListView("NI-FCM_1_TEST");}



                    Step.iCycle++;
                    return false ;

                case 308:
                    if (!OM.CmnOptn.bIgnrFCMTester && !SEQ.FcmTester.RcvdMsg) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 309:
                    if (!m_tmDelay.OnDelay(OM.DevInfo.iFCMTestEndDelay))return false ;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.FCM )) return false;

                    Log.TraceListView("FCM3 Sylinge End");

                    TankCP1.Supply(false);
                    IO_SetY(yi.FCMOut_OtSt , false);//FCM밑에 노즐.
                    //Cmbr3.InspSupply(false); //챔버 닫고.
                    
                    Step.iCycle++;
                    return false ;

                case 310:

                    DM.ARAY[ri.CHA].SetStat(2,0,cs.WorkEnd);

                    if(DM.ARAY[ri.CHA].CheckAllStat(cs.WorkEnd)){SEQ.FcmTester.SendTestSeq(TCPIP_NewOpticsFCM.ETestSeq.End , ""); Log.TraceListView("NI-End");}
                    Step.iCycle++;
                    return false;

                case 311:
                    if (DM.ARAY[ri.CHA].CheckAllStat(cs.WorkEnd) && !OM.CmnOptn.bIgnrFCMTester && !SEQ.FcmTester.RcvdMsg) return false;
                    IO_SetY(yi.CHA_FcmLaserOn, false);
                    Step.iCycle=0;
                    return true ;


                //4번챔버 4DLS =====================================================
                case 400:
                    //==============================================================================================
                    //iCmb1DeadVol < iCmb1DCSylPos 이 조건이 되어야 시퀜스 가능.
                    //삼거리전까지 챔버용량빨기 -> 빨은만큼 버리기. -> 검사용량 빨기 -> 데드볼륨밀기 -> cp2대드볼륨용량만큼 빨기 -> 검사용량만큼 밀기.
                    //==============================================================================================
                    if (OM.CmnOptn.bNotUse4DLS)
                    {
                        Step.iCycle = 410;
                        return false;
                    }

                    Cmbr4.InspSupply(true); //챔버 열고.
                    SEQ.SyringePump.PickupIncPos((int)PumpID.FCM, VALVE_POS.Output, OM.DevInfo.iCmb4ToInter , OM.DevInfo.iDeadVolSpd); //혈액샘플 담기.
                    m_tmDelay.Clear();

                    Step.iCycle++;
                    return false;

                case 401://시리얼 통신 렉 기다려줌.
                    if (!m_tmDelay.OnDelay(300)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.FCM)) return false;
                    Cmbr4.InspSupply(false); //챔버 닫고.

                    IO_SetY(yi.FCMOut_OtSt, true);//FCM밑에 노즐.
                    SEQ.SyringePump.DispIncPos((int)PumpID.FCM, VALVE_POS.Output, OM.DevInfo.iCmb4ToInter, OM.DevInfo.iDeadVolSpd); //대드 볼륨 밀어내기.          

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 402://다빨았는지 확인.
                    if (!m_tmDelay.OnDelay(300)) return false;  //잠시 기다렸다가.
                    if (SEQ.SyringePump.GetBusy((int)PumpID.FCM)) return false;
                    IO_SetY(yi.FCMOut_OtSt, false);

                    //검사 혈액 담기.
                    Cmbr4.InspSupply(true); //챔버 열고.
                    SEQ.SyringePump.PickupIncPos((int)PumpID.FCM , VALVE_POS.Output , OM.DevInfo.iCmb4FCMSylPos , OM.DevInfo.iDeadVolSpd); //혈액샘플 담기.
                    m_tmDelay.Clear();

                    Step.iCycle++;
                    return false ;

                case 403://시리얼 통신 렉 기다려줌.
                    if(!m_tmDelay.OnDelay(300))return false ; 
                    if(SEQ.SyringePump.GetBusy((int)PumpID.FCM )) return false;
                    Cmbr4.InspSupply(false); //챔버 닫고.

                    IO_SetY(yi.FCMOut_OtSt , true);//FCM밑에 노즐.

                    TankCP2.Supply(true);

                    //SEQ.SyringePump.DispIncPos((int)PumpID.FCM , VALVE_POS.Output , OM.DevInfo.iCmb4DeadVol , OM.DevInfo.iDeadVolSpd); //대드 볼륨 밀어내기.      
                    SEQ.SyringePump.DispAndPickupInc((int)PumpID.FCM, VALVE_POS.Output, VALVE_POS.Input, OM.DevInfo.iCmb4DeadVol, OM.DevInfo.iDeadVolSpd, OM.DevInfo.iCmb4DeadTimes); //느리게 할려면 크게      \

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 404://다빨았는지 확인.
                    if(!m_tmDelay.OnDelay(300))return false ;  //잠시 기다렸다가.
                    if(SEQ.SyringePump.GetBusy((int)PumpID.FCM )) return false;        
                    IO_SetY(yi.FCMOut_OtSt , false);

                    TankCP2.Supply(false);
                    Step.iCycle++;
                    return false;

                //    TankCP2.Supply(true);
                //    SEQ.SyringePump.PickupIncPos((int)PumpID.FCM , VALVE_POS.Input , OM.DevInfo.iCmb4DeadVol , OM.DevInfo.iDeadVolSpd); //cp2대드볼륨용량만큼 담기.   
                //    m_tmDelay.Clear();
                //    Step.iCycle++;
                //    return false ;
                //
                //case 405://다빨았는지 확인.
                //    if(!m_tmDelay.OnDelay(300))return false ; 
                //    if(SEQ.SyringePump.GetBusy((int)PumpID.FCM )) return false;
                //    TankCP2.Supply(false);



                case 405://다빨았는지 확인.
                    //검사시작=========================================================================

                    //검사시에 CP1 먼저 투여.
                    IO_SetY(yi.FCMOut_OtSt , true);
                    TankCP1.Supply(true); //CP1 먼저 공급. 먼저cP1을 공급.

                    //LD On
                    IO_SetY(yi.CHA_FcmLaserOn, true);

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 406:
                    if(!m_tmDelay.OnDelay(1000))return false ; //CP1부터 투여 하고 1초 있다가.
                    SEQ.SyringePump.DispIncPos((int)PumpID.FCM , VALVE_POS.Output , OM.DevInfo.iCmb4FCMSylPos , OM.DevInfo.iCmb4FCMSylSpdCode); //샘플 FCM으로 밀어넣기.  
                    Log.TraceListView("FCM2 Sylinge Start");
                    //FCM탱크가 너무 작어서..
                    IO_SetY(yi.FCMWOut_OtSt, true);
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 407:
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iFCMTestStartDelay))return false ; //혈액샘플이 FCM에 들어가게 잠깐 쉬었다. 

                    //FCM탱크가 너무 작어서..
                    IO_SetY(yi.FCMWOut_OtSt, false);

                    //검사기에게 프로토콜.
                    if (OM.CmnOptn.bAutoQc) {SEQ.FcmTester.SendTestSub(TCPIP_NewOpticsFCM.ETest.QC_FCM_2_TEST);Log.TraceListView("NI-QC_FCM_2_TEST");}
                    else                    {SEQ.FcmTester.SendTestSub(TCPIP_NewOpticsFCM.ETest.FCM_2_TEST   );Log.TraceListView("NI-FCM_2_TEST"   );}


                    Step.iCycle++;
                    return false ;

                case 408:
                    if (!OM.CmnOptn.bIgnrFCMTester && !SEQ.FcmTester.RcvdMsg) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 409:
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iFCMTestEndDelay)) return false ;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.FCM )) return false;

                    Log.TraceListView("FCM2 Sylinge End");

                    TankCP1.Supply(false);
                    IO_SetY(yi.FCMOut_OtSt , false);//FCM밑에 노즐.
                    Step.iCycle++;
                    return false ;

                case 410: //위에서 씀.

                    DM.ARAY[ri.CHA].SetStat(3,0,cs.WorkEnd);

                    if(DM.ARAY[ri.CHA].CheckAllStat(cs.WorkEnd)){SEQ.FcmTester.SendTestSeq(TCPIP_NewOpticsFCM.ETestSeq.End , ""); Log.TraceListView("NI-End");}
                    Step.iCycle++;
                    return false;

                case 411:
                    if (DM.ARAY[ri.CHA].CheckAllStat(cs.WorkEnd) && !OM.CmnOptn.bIgnrFCMTester && !SEQ.FcmTester.RcvdMsg) return false;
                    //LD Off
                    IO_SetY(yi.CHA_FcmLaserOn, false);
                    Step.iCycle=0;
                    return true;

                //5번챔버 RET =====================================================
                case 500:
                    //==============================================================================================
                    //iCmb1DeadVol < iCmb1DCSylPos 이 조건이 되어야 시퀜스 가능.
                    //삼거리전까지 챔버용량빨기 -> 빨은만큼 버리기. -> 검사용량 빨기 -> 데드볼륨밀기 -> cp2대드볼륨용량만큼 빨기 -> 검사용량만큼 밀기.
                    //==============================================================================================
                    if (OM.CmnOptn.bNotUseRet)
                    {
                        Step.iCycle = 510;
                        return false;
                    }

                    Cmbr5.InspSupply(true); //챔버 열고.
                    SEQ.SyringePump.PickupIncPos((int)PumpID.FCM, VALVE_POS.Output, OM.DevInfo.iCmb5ToInter, OM.DevInfo.iDeadVolSpd); //혈액샘플 담기.
                    m_tmDelay.Clear();

                    Step.iCycle++;
                    return false;

                case 501://시리얼 통신 렉 기다려줌.
                    if (!m_tmDelay.OnDelay(300)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.FCM)) return false;
                    Cmbr5.InspSupply(false); //챔버 닫고.

                    IO_SetY(yi.FCMOut_OtSt, true);//FCM밑에 노즐.
                    SEQ.SyringePump.DispIncPos((int)PumpID.FCM, VALVE_POS.Output, OM.DevInfo.iCmb5ToInter, OM.DevInfo.iDeadVolSpd); //대드 볼륨 밀어내기.     
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 502://다빨았는지 확인.
                    if (!m_tmDelay.OnDelay(300)) return false;  //잠시 기다렸다가.
                    if (SEQ.SyringePump.GetBusy((int)PumpID.FCM)) return false;
                    IO_SetY(yi.FCMOut_OtSt, false);

                    //혈액검사 샘플 담기.
                    Cmbr5.InspSupply(true); //챔버 열고.
                    SEQ.SyringePump.PickupIncPos((int)PumpID.FCM , VALVE_POS.Output , OM.DevInfo.iCmb5FCMSylPos , OM.DevInfo.iDeadVolSpd); //혈액샘플 담기.
                    m_tmDelay.Clear();

                    Step.iCycle++;
                    return false ;

                case 503://시리얼 통신 렉 기다려줌.
                    if(!m_tmDelay.OnDelay(300))return false ; 
                    if(SEQ.SyringePump.GetBusy((int)PumpID.FCM )) return false;
                    Cmbr5.InspSupply(false); //챔버 닫고.

                    IO_SetY(yi.FCMOut_OtSt , true);//FCM밑에 노즐.

                    TankCP2.Supply(true);
                    //SEQ.SyringePump.DispIncPos((int)PumpID.FCM , VALVE_POS.Output , OM.DevInfo.iCmb5DeadVol , OM.DevInfo.iDeadVolSpd); //대드 볼륨 밀어내기.     
                    SEQ.SyringePump.DispAndPickupInc((int)PumpID.FCM, VALVE_POS.Output, VALVE_POS.Input, OM.DevInfo.iCmb5DeadVol, OM.DevInfo.iDeadVolSpd, OM.DevInfo.iCmb5DeadTimes); //느리게 할려면 크게      \
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 504://다빨았는지 확인.
                    if(!m_tmDelay.OnDelay(300))return false ;  //잠시 기다렸다가.
                    if(SEQ.SyringePump.GetBusy((int)PumpID.FCM )) return false;        
                    IO_SetY(yi.FCMOut_OtSt , false);

                    TankCP2.Supply(false);
                    Step.iCycle++;
                    return false;

                //  TankCP2.Supply(true);
                //    SEQ.SyringePump.PickupIncPos((int)PumpID.FCM , VALVE_POS.Input , OM.DevInfo.iCmb5DeadVol , OM.DevInfo.iDeadVolSpd); //대드볼륨 밀어낼꺼 담기.   
                //    m_tmDelay.Clear();
                //    Step.iCycle++;
                //    return false ;
                //
                //case 505://다빨았는지 확인.
                //    if(!m_tmDelay.OnDelay(300))return false ; 
                //    if(SEQ.SyringePump.GetBusy((int)PumpID.FCM )) return false;
                //    TankCP2.Supply(false);

                case 505://다빨았는지 확인.
                    //검사시작=========================================================================
                    //검사시에 CP1 먼저 투여.
                    IO_SetY(yi.FCMOut_OtSt , true);
                    TankCP1.Supply(true); //CP1 먼저 공급. 먼저cP1을 공급.

                    //LD On
                    IO_SetY(yi.CHA_FcmLaserOn, true);

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 506:
                    if(!m_tmDelay.OnDelay(1000))return false ; //CP1부터 투여 하고 1초 있다가.
                    SEQ.SyringePump.DispIncPos((int)PumpID.FCM, VALVE_POS.Output, OM.DevInfo.iCmb5FCMSylPos, OM.DevInfo.iCmb5FCMSylSpdCode); //샘플 FCM으로 밀어넣기. 
                    Log.TraceListView("FCM3 Sylinge Start");

                    //FCM탱크가 너무 작어서..
                    IO_SetY(yi.FCMWOut_OtSt, true);

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 507:
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iFCMTestStartDelay))return false ; //혈액샘플이 FCM에 들어가게 잠깐 쉬었다. 

                    //FCM탱크가 너무 작어서..
                    IO_SetY(yi.FCMWOut_OtSt, false);

                    //검사기에게 프로토콜.
                    if (OM.CmnOptn.bAutoQc) {SEQ.FcmTester.SendTestSub(TCPIP_NewOpticsFCM.ETest.QC_FCM_3_TEST);Log.TraceListView("NI-QC_FCM_3_TEST");}
                    else                    {SEQ.FcmTester.SendTestSub(TCPIP_NewOpticsFCM.ETest.FCM_3_TEST   );Log.TraceListView("NI-FCM_3_TEST"   );}


                    Step.iCycle++;
                    return false ;

                case 508:
                    if (!OM.CmnOptn.bIgnrFCMTester && !SEQ.FcmTester.RcvdMsg) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;


                case 509:
                    if (!m_tmDelay.OnDelay(OM.DevInfo.iFCMTestEndDelay)) return false ;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.FCM )) return false;

                    Log.TraceListView("FCM2 Sylinge End");

                    TankCP1.Supply(false);
                    IO_SetY(yi.FCMOut_OtSt , false);//FCM밑에 노즐.

                    Step.iCycle++;
                    return false;

                //위에서 씀.
                case 510:
                    DM.ARAY[ri.CHA].SetStat(4,0,cs.WorkEnd);

                    if(DM.ARAY[ri.CHA].CheckAllStat(cs.WorkEnd)){SEQ.FcmTester.SendTestSeq(TCPIP_NewOpticsFCM.ETestSeq.End , ""); Log.TraceListView("NI-End");}
                    Step.iCycle++;
                    return false;

                case 511:
                    if (DM.ARAY[ri.CHA].CheckAllStat(cs.WorkEnd) && !OM.CmnOptn.bIgnrFCMTester && !SEQ.FcmTester.RcvdMsg) return false;

                    //LD Off
                    IO_SetY(yi.CHA_FcmLaserOn, false);

                    Step.iCycle=0;
                    return true ;

                //6번챔버 NR =====================================================
                case 600:
                    //==============================================================================================
                    //iCmb1DeadVol < iCmb1DCSylPos 이 조건이 되어야 시퀜스 가능.
                    //삼거리전까지 챔버용량빨기 -> 빨은만큼 버리기. -> 검사용량 빨기 -> 데드볼륨밀기 -> cp2대드볼륨용량만큼 빨기 -> 검사용량만큼 밀기.
                    //==============================================================================================
                    if (OM.CmnOptn.bNotUseNr)
                    {
                        Step.iCycle = 610;
                        return false;
                    }
                    Cmbr6.InspSupply(true); //챔버 열고.
                    SEQ.SyringePump.PickupIncPos((int)PumpID.FCM, VALVE_POS.Output, OM.DevInfo.iCmb6ToInter, OM.DevInfo.iDeadVolSpd); //혈액샘플 담기.
                    m_tmDelay.Clear();

                    Step.iCycle++;
                    return false;

                case 601://시리얼 통신 렉 기다려줌.
                    if (!m_tmDelay.OnDelay(300)) return false;
                    if (SEQ.SyringePump.GetBusy((int)PumpID.FCM)) return false;
                    Cmbr6.InspSupply(false); //챔버 닫고.

                    IO_SetY(yi.FCMOut_OtSt, true);//FCM밑에 노즐.
                    SEQ.SyringePump.DispIncPos((int)PumpID.FCM, VALVE_POS.Output, OM.DevInfo.iCmb6ToInter, OM.DevInfo.iDeadVolSpd); //대드 볼륨 밀어내기.        
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false;

                case 602://다빨았는지 확인.
                    if (!m_tmDelay.OnDelay(300)) return false;  //잠시 기다렸다가.
                    if (SEQ.SyringePump.GetBusy((int)PumpID.FCM)) return false;
                    IO_SetY(yi.FCMOut_OtSt, false);


                    Cmbr6.InspSupply(true); //챔버 열고.
                    SEQ.SyringePump.PickupIncPos((int)PumpID.FCM , VALVE_POS.Output , OM.DevInfo.iCmb6FCMSylPos , OM.DevInfo.iDeadVolSpd); //혈액샘플 담기.
                    m_tmDelay.Clear();

                    Step.iCycle++;
                    return false ;

                case 603://시리얼 통신 렉 기다려줌.
                    if(!m_tmDelay.OnDelay(300))return false ; 
                    if(SEQ.SyringePump.GetBusy((int)PumpID.FCM )) return false;
                    Cmbr6.InspSupply(false); //챔버 닫고.

                    IO_SetY(yi.FCMOut_OtSt , true);//FCM밑에 노즐.

                    TankCP2.Supply(true);

                    //SEQ.SyringePump.DispIncPos((int)PumpID.FCM , VALVE_POS.Output , OM.DevInfo.iCmb6DeadVol , OM.DevInfo.iDeadVolSpd); //대드 볼륨 밀어내기.        
                    SEQ.SyringePump.DispAndPickupInc((int)PumpID.FCM, VALVE_POS.Output, VALVE_POS.Input, OM.DevInfo.iCmb6DeadVol, OM.DevInfo.iDeadVolSpd, OM.DevInfo.iCmb6DeadTimes); //느리게 할려면 크게      \
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 604://다빨았는지 확인.
                    if(!m_tmDelay.OnDelay(300))return false ;  //잠시 기다렸다가.
                    if(SEQ.SyringePump.GetBusy((int)PumpID.FCM )) return false;        
                    IO_SetY(yi.FCMOut_OtSt , false);

                    TankCP2.Supply(false);
                    Step.iCycle++;
                    return false;

                //
                //
                //    TankCP2.Supply(true);
                //    SEQ.SyringePump.PickupIncPos((int)PumpID.FCM , VALVE_POS.Input , OM.DevInfo.iCmb6DeadVol , OM.DevInfo.iDeadVolSpd); //대드볼륨 밀어낼꺼 담기.   
                //    m_tmDelay.Clear();
                //    Step.iCycle++;
                //    return false ;
                //
                //case 605://다빨았는지 확인.
                //    if(!m_tmDelay.OnDelay(300))return false ; 
                //    if(SEQ.SyringePump.GetBusy((int)PumpID.FCM )) return false;
                //    TankCP2.Supply(false);

                case 605://다빨았는지 확인.
                    //검사시작=========================================================================

                    //검사시에 CP1 먼저 투여.
                    IO_SetY(yi.FCMOut_OtSt , true);
                    TankCP1.Supply(true); //CP1 먼저 공급. 먼저cP1을 공급.

                    //LD On
                    IO_SetY(yi.CHA_FcmLaserOn, true);

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 606:
                    if(!m_tmDelay.OnDelay(1000))return false ; //CP1부터 투여 하고 1초 있다가.
                    SEQ.SyringePump.DispIncPos((int)PumpID.FCM , VALVE_POS.Output , OM.DevInfo.iCmb6FCMSylPos , OM.DevInfo.iCmb6FCMSylSpdCode); //샘플 FCM으로 밀어넣기.   
                    Log.TraceListView("FCM4 Sylinge Start");

                    //FCM탱크가 너무 작어서..
                    IO_SetY(yi.FCMWOut_OtSt, true);

                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 607:
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iFCMTestStartDelay))return false ; //혈액샘플이 FCM에 들어가게 잠깐 쉬었다. 
                    //FCM탱크가 너무 작어서..
                    IO_SetY(yi.FCMWOut_OtSt, false);
                    //검사기에게 프로토콜.
                    if (OM.CmnOptn.bAutoQc) {SEQ.FcmTester.SendTestSub(TCPIP_NewOpticsFCM.ETest.QC_FCM_4_TEST);Log.TraceListView("NI-QC_FCM_4_TEST");}
                    else                    {SEQ.FcmTester.SendTestSub(TCPIP_NewOpticsFCM.ETest.FCM_4_TEST   );Log.TraceListView("NI-FCM_4_TEST");}

                    Step.iCycle++;
                    return false ;

                case 608:
                    if (!OM.CmnOptn.bIgnrFCMTester && !SEQ.FcmTester.RcvdMsg) return false;
                    m_tmDelay.Clear();
                    Step.iCycle++;
                    return false ;

                case 609:
                    if(!m_tmDelay.OnDelay(OM.DevInfo.iFCMTestEndDelay))return false ;  //원래 검사 끝나는 프로토콜이 있는줄 알았는데 NI쪽에서 알아서 함. 그래서 의미 없음.
                    if (SEQ.SyringePump.GetBusy((int)PumpID.FCM )) return false;

                    Log.TraceListView("FCM4 Sylinge End");

                    TankCP1.Supply(false);
                    IO_SetY(yi.FCMOut_OtSt , false);//FCM밑에 노즐.

                    Step.iCycle++;
                    return false;

                //위에서 씀.
                case 610:
                    DM.ARAY[ri.CHA].SetStat(5,0,cs.WorkEnd);

                    if(DM.ARAY[ri.CHA].CheckAllStat(cs.WorkEnd)){SEQ.FcmTester.SendTestSeq(TCPIP_NewOpticsFCM.ETestSeq.End , ""); Log.TraceListView("NI-End");}
                    Step.iCycle++;
                    return false;

                case 611:
                    if (DM.ARAY[ri.CHA].CheckAllStat(cs.WorkEnd) && !OM.CmnOptn.bIgnrFCMTester && !SEQ.FcmTester.RcvdMsg) return false;

                    //LD Off
                    IO_SetY(yi.CHA_FcmLaserOn, false); 

                    Step.iCycle=0;
                    return true ;                   

            }
        } 
     
     */


}
