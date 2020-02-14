using System;
using System.Linq;
using System.Threading;
using MotionInterface;
using System.ComponentModel;
using COMMON;


namespace Control
{
    public enum MOTION_DIR : uint
    {
        LeftRight = 0, //정면에서   봤을때 Left 가 - Right가 +
        RightLeft    , //정면에서   봤을때 Right가 - Left 가 +
        BwdFwd       , //정면에서   봤을때 Bwd  가 - Fwd  가 +
        FwdBwd       , //정면에서   봤을때 Fwd  가 - Bwd  가 +
        UpDown       , //정면에서   봤을때 Up   가 - Down 가 +
        DownUp       , //정면에서   봤을때 Down 가 - Up   가 +
        CwCcw        , //회전축에서 봤을때 Clock가 - AntiC가 +
        CcwCw          //회전축에서 봤을때 AntiC가 - Clock가 +
    } 
    
    [Serializable]
    public class CParaMotr
    {   
        [CategoryAttribute("Motor Para" ), DisplayNameAttribute("Name"                    )]public string     sMotrName     {get; set;} //모터이름 UI혹은 LOG용.
//      [CategoryAttribute("Motor Para" ), DisplayNameAttribute("Physical No"             )]public int        iPhysicalNo   {get; set;} //실제모터 물리 어드레스.
        [CategoryAttribute("Motor Para" ), DisplayNameAttribute("Pulse per Revolution"    )]public uint       iPulsePerRev  {get; set;} //한바퀴의 펄스수.
        [CategoryAttribute("Motor Para" ), DisplayNameAttribute("Unit per Revolution"     )]public double     dUnitPerRev   {get; set;} //한바퀴당 이동거리 혹은 각도.
        [CategoryAttribute("Motor Para" ), DisplayNameAttribute("Max Limit Position"      )]public double     dMaxPos       {get; set;} //MAX Position.
        [CategoryAttribute("Motor Para" ), DisplayNameAttribute("Min Limit Position"      )]public double     dMinPos       {get; set;} //MIN Position.
        [CategoryAttribute("Motor Para" ), DisplayNameAttribute("Use Encoder"             )]public bool       bExistEnc     {get; set;} //엔코더가 있는지 없는지.없으면 CmdPos가 EncPos로 리턴.
        [CategoryAttribute("Motor Para" ), DisplayNameAttribute("Break Off IO Address"    )]public int        iBreakOffAdd  {get; set;} //브레이크 타입 브레이크 IO Address
        [CategoryAttribute("Motor Para" ), DisplayNameAttribute("Direction Type"          )]public MOTION_DIR iDirType      {get; set;} //모터 이동방향. 화면디스플레이용.
        [CategoryAttribute("Motor Para" ), DisplayNameAttribute("Use Break"               )]public bool       bUseBreak     {get; set;} //브레이크 타입 모터 설정.
        [CategoryAttribute("Motor Para" ), DisplayNameAttribute("Use Inposition"          )]public double     dInposition   {get; set;} //인포지션.
                                                                                                              
        [CategoryAttribute("Speed Para" ), DisplayNameAttribute("Acceleration"            )]public double     dAcceleration {get; set;} //가속시간.
        [CategoryAttribute("Speed Para" ), DisplayNameAttribute("Deceleration"            )]public double     dDeceleration {get; set;} //감속
        [CategoryAttribute("Speed Para" ), DisplayNameAttribute("Run Speed"               )]public double     dRunSpeed     {get; set;} //구동속도
        [CategoryAttribute("Speed Para" ), DisplayNameAttribute("Slow Speed"              )]public double     dSlowSpeed    {get; set;} //느린구동속도.
        [CategoryAttribute("Speed Para" ), DisplayNameAttribute("Manual Speed"            )]public double     dManSpeed     {get; set;} //메뉴얼 구동속도
        [CategoryAttribute("Speed Para" ), DisplayNameAttribute("Home Velocity"           )]public double     dHomeVelFirst {get; set;} //홈잡을때 기본적인 속도
        [CategoryAttribute("Speed Para" ), DisplayNameAttribute("Home Last Velocity"      )]public double     dHomeVelLast  {get; set;} //홈잡을때 마지막 미세조정용 속도.
        [CategoryAttribute("Speed Para" ), DisplayNameAttribute("Home Acceleration"       )]public double     dHomeAccFirst {get; set;} //홈잡을때 가감속.
        [CategoryAttribute("Speed Para" ), DisplayNameAttribute("Jog Speed"               )]public double     dJogSpeed     {get; set;} //조그구동 속도.
                                                                                                              
        [CategoryAttribute("Repeat Para" ), DisplayNameAttribute("First Position"         )]public double     dFstPos       {get; set;} //첫위치
        [CategoryAttribute("Repeat Para" ), DisplayNameAttribute("Second Position"        )]public double     dScdPos       {get; set;} //두번째위치.
        [CategoryAttribute("Repeat Para" ), DisplayNameAttribute("Repat  Delay"           )]public uint       iRepeatDelay  {get; set;} //모션돈 후 정지 딜레이.
        [CategoryAttribute("Repeat Para" ), DisplayNameAttribute("Repeat Speed"           )]public double     dRepeatSpeed  {get; set;} //리피트 스피드.
        [CategoryAttribute("Repeat Para" ), DisplayNameAttribute("Repeat Count"           )]public uint       iRepeatCount  {get; set;} //리피트 카운트 0이면 무한대.
                                                                                                              
    } ;                                                                                                       
                                                                                                              
    public class CStatMotr
    {
        public bool   bMovingP ;
        public bool   bMovingN ;

        public double dPreCmdPos;
        public double dTrgPos  ;

        public bool   bHoming ;

        public uint   iRepeatCntRemain;
        public double dRepeatTackTime;


    };

    public class CMotrMan
    {
        public IMotor        [] Mtr ;
        public CParaMotr     [] Para;
        public CStatMotr     [] Stat;

         //파익스가 홈던 셋을 못함..씨발 짜증나.
        public bool          [] HomeDone ;
        public bool          [] Homming  ;        
        public CDelayTimer   [] RptTimer ;
        public CCycleTimer   [] CclTimer ;

        //EN_LAN_SEL   m_eLangSel        ; //Languge Selection.
        string       m_sParaFolderPath ;
        int          m_iMaxMotr        ; public int _iMaxMotr { get { return m_iMaxMotr; } }
        
        
        //CDioMan Dio = null;


        //public bool Init(EN_LAN_SEL _eLanSel,string _sParaFolderPath,int _iMaxMtrCnt,EN_MOTR_SEL [] _eMotrSels ,CDioMan _Dio )
        public bool Init(string _sParaFolderPath,int _iMaxMtrCnt )
        {
             
            //m_eLangSel = _eLanSel;
            m_sParaFolderPath = _sParaFolderPath;

            Para    = new CParaMotr   [_iMaxMtrCnt];
            Stat    = new CStatMotr   [_iMaxMtrCnt];

            RptTimer = new CDelayTimer[_iMaxMtrCnt];
            CclTimer = new CCycleTimer[_iMaxMtrCnt];

            HomeDone = new bool[_iMaxMtrCnt];
            Homming  = new bool[_iMaxMtrCnt];
            //ParaSub = new object[_iMaxMtrCnt];
            Mtr     = new IMotor[_iMaxMtrCnt];

            m_iMaxMotr = _iMaxMtrCnt; 
            for (int i = 0; i < m_iMaxMotr; i++ )
            {
                Para  [i] = new CParaMotr  ();
                Stat  [i] = new CStatMotr  ();

                RptTimer[i] = new CDelayTimer();
                CclTimer[i] = new CCycleTimer();

                HomeDone[i] = false ;
                Homming [i] = false ;

                Mtr[i] = new MotionAXL.CMotor();
                Mtr[i].Init();
            }                 
           
            LoadSaveAll(true);
            ApplyParaAll();
            
            return true;
        }

        public bool Close()
        {
            for (int i = 0; i < m_iMaxMotr; i++)
            {
                Mtr[i].Close();
            }
            return true ;
        }

        public bool LoadSaveAll(bool _bLoad)
        {
            bool bRet = true ;

            
            string sFilePath;
            if (_bLoad)
            {
                //sFilePath = m_sParaFolderPath + "MotrAxl.xml";
                //object oParaMotrSub = ParaSub ;
                //if (!CXml.LoadXml(sFilePath, ref oParaMotrSub)) { bRet = false; }
                //ParaSub = (object [])oParaMotrSub;
                for (int i = 0; i < m_iMaxMotr; i++)
                {
                    sFilePath = m_sParaFolderPath + "MotrPara"+i.ToString()+".xml";
                    if (!CXml.LoadXml<CParaMotr>(sFilePath, ref Para[i])) { bRet = false; }    
                    if (!Mtr[i].LoadSave(_bLoad,m_sParaFolderPath, i)) { bRet = false; }    
                }                
            }
            else
            {
                //sFilePath = m_sParaFolderPath + "MotrAxl.xml";
                //object oParaMotrSub = ParaSub;
                //if (!CXml.SaveXml(sFilePath, ref oParaMotrSub)) { bRet = false; }
                for (int i = 0; i < m_iMaxMotr; i++)
                {
                    sFilePath = m_sParaFolderPath + "MotrPara"+i.ToString()+".xml";
                    if (!CXml.SaveXml<CParaMotr>(sFilePath, ref Para[i])) { bRet = false; }
                    if (!Mtr[i].LoadSave(_bLoad,m_sParaFolderPath, i)){ bRet = false; }
                }

            }

            return bRet ;
        }

        //public bool LoadSave(bool _bLoad,int _iMotrNo)
        //{
        //    string sFilePath ;

        //    bool bRet = true ;
        //    if (_bLoad)
        //    {
        //        sFilePath = m_sParaFolderPath + "MotrAxl_" + _iMotrNo.ToString() + ".xml";
        //        object oParaMotrSub = ParaSub[_iMotrNo];
        //        if (!CXml.LoadXml(sFilePath, ref oParaMotrSub)) { bRet = false; }
 
        //        sFilePath = m_sParaFolderPath + "Motr_"    + _iMotrNo.ToString() + ".xml"; 
        //        if (!CXml.LoadXml<CParaMotr>(sFilePath, ref Para[_iMotrNo] )){bRet = false;}
        //    }
        //    else
        //    {
        //        sFilePath = m_sParaFolderPath + "MotrAxl_" + _iMotrNo.ToString() + ".xml";
        //        if (!CXml.SaveXml(sFilePath, ref ParaSub[_iMotrNo])){bRet = false;}
                
        //        sFilePath = m_sParaFolderPath + "Motr_"    + _iMotrNo.ToString() + ".xml";
        //        if (!CXml.SaveXml<CParaMotr>(sFilePath, ref Para[_iMotrNo])){bRet = false;}

        //    }
        //    return bRet ;
        //}

        public void ApplyParaAll()
        {
            double dPulsePerUnit = 0.0;
            for (int i = 0; i < m_iMaxMotr; i++)
            {
                dPulsePerUnit = Para[i].dUnitPerRev != 0 ? Para[i].iPulsePerRev / Para[i].dUnitPerRev : 0.0;
                Mtr[i].ApplyPara(dPulsePerUnit);
            }
        }

        

        

        
        //=======================================================================

        

        public double UnitToPulse(int _iMotrNo , double _dUnit)
        {   
            double dPulse = Para[_iMotrNo].iPulsePerRev ;
            double dUnit  = Para[_iMotrNo].dUnitPerRev  ;

            return (dPulse * _dUnit) / dUnit; 
        }

        public double PulseToUnit(int _iMotrNo , double _dPulse)
        {
            double dPulse = Para[_iMotrNo].iPulsePerRev ;
            double dUnit  = Para[_iMotrNo].dUnitPerRev  ;

            return (dUnit * _dPulse )/ dPulse; 
        }
        
        public bool GetStop(int _iMotrNo)
        {
            if( _iMotrNo < 0 || _iMotrNo >= m_iMaxMotr) 
            {
                Log.ShowMessageFunc(string.Format("ERR Motr{0} is not Between 0 and {1}",_iMotrNo,m_iMaxMotr));
                return false; 
            }
            return Mtr[_iMotrNo].GetStop();
        }

        public bool GetStopInpos(int _iMotrNo)
        {
            if( _iMotrNo < 0 || _iMotrNo >= m_iMaxMotr) 
            {
                Log.ShowMessageFunc(string.Format("ERR Motr{0} is not Between 0 and {1}",_iMotrNo,m_iMaxMotr));
                return false; 
            }

            if(!Mtr[_iMotrNo].GetStop()) return false ;

            if (!Para[_iMotrNo].bExistEnc) return true ;

            const int MIN_PULSE = 10  ; //10 Pulse 미만이면 모션돈이 안될수 있다.
            const int MAX_PULSE = 500 ; //500 Pulse 이상이면 하나마나 일수 있다.
            double dMinUnit  = PulseToUnit(_iMotrNo , MIN_PULSE ) ;
            double dMaxUnit  = PulseToUnit(_iMotrNo , MAX_PULSE ) ;
            double dInp      = Para[_iMotrNo].dInposition  ;
            
            if (dInp < dMinUnit) dInp = dMinUnit ;
            if (dInp > dMaxUnit) dInp = dMaxUnit ;

            //int dInpUnit = dInp
            
            double dCmdPos = GetCmdPos(_iMotrNo);
            double dEncPos = GetEncPos(_iMotrNo);

            if (Math.Abs(dCmdPos - dEncPos) > dInp) return false;

            return true ;
        }


        public void SetServo(int _iMotrNo , bool _bOn)
        {
            if( _iMotrNo < 0 || _iMotrNo >= m_iMaxMotr) 
            {
                Log.ShowMessageFunc(string.Format("ERR Motr{0} is not Between 0 and {1}",_iMotrNo,m_iMaxMotr));
                return ; 
            }

            if(!_bOn)Stop(_iMotrNo);

            Mtr[_iMotrNo].SetServo(_bOn);

            if (!_bOn)
            {
                SetHomeDone(_iMotrNo,false);
            }
            else
            {
                SetPos(_iMotrNo,GetEncPos(_iMotrNo));

            }



        }

        public bool GetServo(int _iMotrNo)
        {
            if( _iMotrNo < 0 || _iMotrNo >= m_iMaxMotr) 
            {
                Log.ShowMessageFunc(string.Format("ERR Motr{0} is not Between 0 and {1}",_iMotrNo,m_iMaxMotr));
                return false; 
            }

            return Mtr[_iMotrNo].GetServo();
        }

        public void SetReset(int _iMotrNo,bool _bOn)
        {
            if( _iMotrNo < 0 || _iMotrNo >= m_iMaxMotr) 
            {
                Log.ShowMessageFunc(string.Format("ERR Motr{0} is not Between 0 and {1}",_iMotrNo,m_iMaxMotr));
                return ; 
            }

            
            Mtr[_iMotrNo].SetReset(_bOn);
        }


        public double GetCmdPos(int _iMotrNo)
        {
            if( _iMotrNo < 0 || _iMotrNo >= m_iMaxMotr) 
            {
                Log.ShowMessageFunc(string.Format("ERR Motr{0} is not Between 0 and {1}",_iMotrNo,m_iMaxMotr));
                return 0.0; 
            }

            double dPulse = Mtr[_iMotrNo].GetCmdPos();
            double dRet = PulseToUnit(_iMotrNo, dPulse) ;
            return dRet ;
        }

        public double GetEncPos(int _iMotrNo)
        {
            if( _iMotrNo < 0 || _iMotrNo >= m_iMaxMotr) 
            {
                Log.ShowMessageFunc(string.Format("ERR Motr{0} is not Between 0 and {1}",_iMotrNo,m_iMaxMotr));
                return 0.0; 
            }

            double dPulse = Mtr[_iMotrNo].GetEncPos();
            double dRet = PulseToUnit(_iMotrNo, dPulse) ;
            return dRet ;
        }

        public double GetTrgPos(int _iMotrNo)
        {
            if (_iMotrNo < 0 || _iMotrNo >= m_iMaxMotr)
            {
                Log.ShowMessageFunc(string.Format("ERR Motr{0} is not Between 0 and {1}", _iMotrNo, m_iMaxMotr));
                return 0.0;
            }

            return Stat[_iMotrNo].dTrgPos;
        }

        public void SetPos(int _iMotrNo,double _dPos)
        {
            if( _iMotrNo < 0 || _iMotrNo >= m_iMaxMotr) 
            {
                Log.ShowMessageFunc(string.Format("ERR Motr{0} is not Between 0 and {1}",_iMotrNo,m_iMaxMotr));
                return ; 
            }

            double dPulse = UnitToPulse(_iMotrNo, _dPos) ;
            Mtr[_iMotrNo].SetPos(dPulse);
            Stat[_iMotrNo].dTrgPos = _dPos;
        }

        public bool CmprPos(int _iMotrNo, double _dPos, double _dRange = 0.0)
        {
            double dRange;

            if (_dRange != 0.0) dRange = _dRange;
            else                dRange = Para[_iMotrNo].dInposition;

            double dPosGap = Math.Abs(GetCmdPos(_iMotrNo) - _dPos) ;
            bool isOk = dPosGap <= dRange  ;//&& dPosGap < dRange ;

            return isOk;
        }

        public bool GetHomeSnsr(int _iMotrNo)
        {
            if( _iMotrNo < 0 || _iMotrNo >= m_iMaxMotr) 
            {
                Log.ShowMessageFunc(string.Format("ERR Motr{0} is not Between 0 and {1}",_iMotrNo,m_iMaxMotr));
                return false; 
            }
            return Mtr[_iMotrNo].GetHomeSnsr();
        }

        public bool GetNLimSnsr(int _iMotrNo)
        {
            if( _iMotrNo < 0 || _iMotrNo >= m_iMaxMotr) 
            {
                Log.ShowMessageFunc(string.Format("ERR Motr{0} is not Between 0 and {1}",_iMotrNo,m_iMaxMotr));
                return false; 
            }
            return Mtr[_iMotrNo].GetNLimSnsr();
        }

        public bool GetPLimSnsr(int _iMotrNo)
        {
            if (_iMotrNo < 0 || _iMotrNo >= m_iMaxMotr)
            {
                Log.ShowMessageFunc(string.Format("ERR Motr{0} is not Between 0 and {1}", _iMotrNo, m_iMaxMotr));
                return false;
            }
            return Mtr[_iMotrNo].GetPLimSnsr();
        }

        public bool GetZphaseSgnl(int _iMotrNo)
        {
            if (_iMotrNo < 0 || _iMotrNo >= m_iMaxMotr)
            {
                Log.ShowMessageFunc(string.Format("ERR Motr{0} is not Between 0 and {1}", _iMotrNo, m_iMaxMotr));
                return false;
            }
            return Mtr[_iMotrNo].GetZphaseSgnl();
        }

        public bool GetInPosSgnl(int _iMotrNo)
        {
            if (_iMotrNo < 0 || _iMotrNo >= m_iMaxMotr)
            {
                Log.ShowMessageFunc(string.Format("ERR Motr{0} is not Between 0 and {1}", _iMotrNo, m_iMaxMotr));
                return false;
            }
            return Mtr[_iMotrNo].GetInPosSgnl();
        }

        public bool GetAlarmSgnl(int _iMotrNo)
        {
            if (_iMotrNo < 0 || _iMotrNo >= m_iMaxMotr)
            {
                Log.ShowMessageFunc(string.Format("ERR Motr{0} is not Between 0 and {1}", _iMotrNo, m_iMaxMotr));
                return false;
            }
            return Mtr[_iMotrNo].GetAlarmSgnl();
        }

        public bool GoHome(int _iMotrNo)
        {
            if (_iMotrNo < 0 || _iMotrNo >= m_iMaxMotr)
            {
                Log.ShowMessageFunc(string.Format("ERR Motr{0} is not Between 0 and {1}", _iMotrNo, m_iMaxMotr));
                return false;
            }

            HomeDone[_iMotrNo] = false ;
            Homming [_iMotrNo] = true  ;

            double dVel1 = UnitToPulse(_iMotrNo, Para[_iMotrNo].dHomeVelFirst);
            double dVel2 = UnitToPulse(_iMotrNo, Para[_iMotrNo].dHomeVelLast );
            double dAcc  = UnitToPulse(_iMotrNo, Para[_iMotrNo].dHomeAccFirst);
            
            return Mtr[_iMotrNo].GoHome(dVel1, dVel2, dAcc);//, dPulsePerUnit);
        }

        //아진은 그냥 쓰면 되는데 파익스에 홈던 깨는 함수가 없어서 이지랄.
        public bool GetHomeDone(int _iMotrNo)
        {
            if (_iMotrNo < 0 || _iMotrNo >= m_iMaxMotr)
            {
                Log.ShowMessageFunc(string.Format("ERR Motr{0} is not Between 0 and {1}", _iMotrNo, m_iMaxMotr));
                return false;
            }

            return HomeDone[_iMotrNo] ;//Mtr[_iMotrNo].GetHomeDone(i)
        }

        public bool GetHomeDoneAll()
        {
            for (int i = 0; i < m_iMaxMotr; i++)
            {
                if(!HomeDone[i]) return false;
            }

            return true;
        }

        public bool GetServoAll()
        {
            for (int i = 0; i < m_iMaxMotr; i++) { if (!Mtr[i].GetServo()) return false; }

            return true;
        }

        public void SetServoAll(bool _bOn)
        {
            for (int i = 0; i < m_iMaxMotr; i++) {
                Mtr[i].SetServo(_bOn);
            }
        }

        public void ResetAll()
        {
            
            for (int i = 0; i < m_iMaxMotr; i++) { 
                if(Mtr[i].GetAlarmSgnl()) Mtr[i].SetReset(true); 
            }    //원래 Reset으로 되어있던거. 되는지 확인해봐야함. 
            //sleep이 필요할수 있음.
            Thread.Sleep(30);
            
            for (int i = 0; i < m_iMaxMotr; i++) { Mtr[i].SetReset(false); }    //원래 Reset으로 되어있던거. 되는지 확인해봐야함. 


        }

        public void StopAll()
        {
            for (int i = 0; i < m_iMaxMotr; i++) { Mtr[i].Stop(); }
        }

        public void EmgStopAll()
        {
            for (int i = 0; i < m_iMaxMotr; i++) { Mtr[i].EmgStop(); }
        }

        public void SetHomeDone(int _iMotrNo, bool _bOn)
        {
            if (_iMotrNo < 0 || _iMotrNo >= m_iMaxMotr)
            {
                Log.ShowMessageFunc(string.Format("ERR Motr{0} is not Between 0 and {1}", _iMotrNo, m_iMaxMotr));
                return;
            }
            Mtr[_iMotrNo].SetHomeDone(_bOn);
            HomeDone[_iMotrNo] = _bOn ;
        }


        public void StopHome(int _iMotrNo)
        {
            if (_iMotrNo < 0 || _iMotrNo >= m_iMaxMotr)
            {
                Log.ShowMessageFunc(string.Format("ERR Motr{0} is not Between 0 and {1}", _iMotrNo, m_iMaxMotr));
                return;
            }
            Mtr[_iMotrNo].StopHome();
        }
        // uHomeResult 설정
        // HOME_SUCCESS                             = 0x01        // 홈 완료
        // HOME_SEARCHING                           = 0x02        // 홈검색중
        // HOME_ERR_GNT_RANGE                       = 0x10        // 홈 검색 범위를 벗어났을경우
        // HOME_ERR_USER_BREAK                      = 0x11        // 속도 유저가 임의로 정지명령을 내렸을경우
        // HOME_ERR_VELOCITY                        = 0x12        // 속도 설정 잘못했을경우
        // HOME_ERR_AMP_FAULT                       = 0x13        // 서보팩 알람 발생 에러
        // HOME_ERR_NEG_LIMIT                       = 0x14        // (-)방향 구동중 (+)리미트 센서 감지 에러
        // HOME_ERR_POS_LIMIT                       = 0x15        // (+)방향 구동중 (-)리미트 센서 감지 에러
        // HOME_ERR_NOT_DETECT                      = 0x16        // 지정한 신호 검출하지 못 할 경우 에러
        // HOME_ERR_UNKNOWN                         = 0xFF     
        public bool GetHoming(int _iMotrNo)
        {
            if (_iMotrNo < 0 || _iMotrNo >= m_iMaxMotr)
            {
                Log.ShowMessageFunc(string.Format("ERR Motr{0} is not Between 0 and {1}", _iMotrNo, m_iMaxMotr));
                return false;
            }
            return Mtr[_iMotrNo].GetHoming();
        }

        public void Stop(int _iMotrNo)
        {
            if (_iMotrNo < 0 || _iMotrNo >= m_iMaxMotr)
            {
                Log.ShowMessageFunc(string.Format("ERR Motr{0} is not Between 0 and {1}", _iMotrNo, m_iMaxMotr));
                return;
            }
            Mtr[_iMotrNo].Stop();
            Stat[_iMotrNo].iRepeatCntRemain = 0;
            Log.Trace(GetName(_iMotrNo) , "Stop");
        }

        public void EmgStop(int _iMotrNo)
        {
            if (_iMotrNo < 0 || _iMotrNo >= m_iMaxMotr)
            {
                Log.ShowMessageFunc(string.Format("ERR Motr{0} is not Between 0 and {1}", _iMotrNo, m_iMaxMotr));
                return;
            }
            Mtr[_iMotrNo].EmgStop();
            Stat[_iMotrNo].iRepeatCntRemain = 0;
            Log.Trace(GetName(_iMotrNo) , "Emg Stop");
        }

        public void JogP(int _iMotrNo)
        {
            if (_iMotrNo < 0 || _iMotrNo >= m_iMaxMotr)
            {
                Log.ShowMessageFunc(string.Format("ERR Motr{0} is not Between 0 and {1}", _iMotrNo, m_iMaxMotr));
                return;
            }
           
            double dVel = UnitToPulse(_iMotrNo, Para[_iMotrNo].dJogSpeed);
            double dAcc = UnitToPulse(_iMotrNo, Para[_iMotrNo].dAcceleration);
            double dDec = UnitToPulse(_iMotrNo, Para[_iMotrNo].dDeceleration);
            Mtr[_iMotrNo].JogP(dVel, dAcc, dDec);
            Log.Trace(GetName(_iMotrNo) , "Jog P");
        }

        public void JogN(int _iMotrNo)
        {
            if (_iMotrNo < 0 || _iMotrNo >= m_iMaxMotr)
            {
                Log.ShowMessageFunc(string.Format("ERR Motr{0} is not Between 0 and {1}", _iMotrNo, m_iMaxMotr));
                return;
            }
            double dVel = UnitToPulse(_iMotrNo, Para[_iMotrNo].dJogSpeed);
            double dAcc = UnitToPulse(_iMotrNo, Para[_iMotrNo].dAcceleration);
            double dDec = UnitToPulse(_iMotrNo, Para[_iMotrNo].dDeceleration);
            Mtr[_iMotrNo].JogN(dVel, dAcc, dDec);
            Log.Trace(GetName(_iMotrNo) , "Jog N");
        }

        protected void Go(int _iMotrNo, double _dPos, double _dVel, double _dAcc, double _dDec)
        {
            if (_iMotrNo < 0 || _iMotrNo >= m_iMaxMotr)
            {
                Log.ShowMessageFunc(string.Format("ERR Motr{0} is not Between 0 and {1}", _iMotrNo, m_iMaxMotr));
                return;
            }

            double dPos = UnitToPulse(_iMotrNo, _dPos) ;
            double dVel = UnitToPulse(_iMotrNo, _dVel) ;
            double dAcc = UnitToPulse(_iMotrNo, _dAcc) ;
            double dDec = UnitToPulse(_iMotrNo, _dDec) ;

            if (GetStop(_iMotrNo)) { 
                Log.Trace(GetName(_iMotrNo) , "Go From " + GetCmdPos(_iMotrNo).ToString() + " To " + _dPos.ToString() );
            }

            if (_dPos > Para[_iMotrNo].dMaxPos)
            {
                Log.ShowMessageFunc(string.Format("ERR Motr{0} is Max Position Over{1}", _iMotrNo, _dPos));
                return;
            }
            if (_dPos < Para[_iMotrNo].dMinPos)
            {
                Log.ShowMessageFunc(string.Format("ERR Motr{0} is Min Position Over{1}", _iMotrNo, _dPos));
                return;
            }

            Mtr[_iMotrNo].GoAbs(dPos , dVel , dAcc , dDec );
            
            Stat[_iMotrNo].dTrgPos = _dPos;
        }

        public void GoAbs(int _iMotrNo, double _dPos, double _dVel, double _dAcc, double _dDec)
        {
            Go(_iMotrNo, _dPos, _dVel, _dAcc, _dDec);
        }

        public void GoAbsVel(int _iMotrNo, double _dPos, double _dVel)
        {
            GoAbs(_iMotrNo , _dPos , _dVel , Para[_iMotrNo].dAcceleration , Para[_iMotrNo].dDeceleration );
        }

        public void GoAbsRun(int _iMotrNo, double _dPos, int _iPer = 0)
        {
            double dPer = _iPer;
            if (dPer <=  0  ) dPer = 100;
            if (dPer >   100) dPer = 100;
            dPer /= 100;
            GoAbs(_iMotrNo , _dPos , Para[_iMotrNo].dRunSpeed * dPer, Para[_iMotrNo].dAcceleration , Para[_iMotrNo].dDeceleration );
        }

        public void GoAbsMan(int _iMotrNo, double _dPos)
        {
            GoAbs(_iMotrNo , _dPos , Para[_iMotrNo].dManSpeed , Para[_iMotrNo].dAcceleration , Para[_iMotrNo].dDeceleration );
        }

        public void GoAbsSlow(int _iMotrNo, double _dPos)
        {
            GoAbs(_iMotrNo , _dPos , Para[_iMotrNo].dSlowSpeed , Para[_iMotrNo].dAcceleration , Para[_iMotrNo].dDeceleration );
        }

        public void GoAbsRepeatFst(int _iMotrNo)
        {
            GoAbs(_iMotrNo, Para[_iMotrNo].dFstPos , Para[_iMotrNo].dRepeatSpeed , Para[_iMotrNo].dAcceleration, Para[_iMotrNo].dDeceleration);
        }

        public void GoAbsRepeatScd(int _iMotrNo)
        {
            GoAbs(_iMotrNo, Para[_iMotrNo].dScdPos, Para[_iMotrNo].dRepeatSpeed, Para[_iMotrNo].dAcceleration, Para[_iMotrNo].dDeceleration);
        }
        public void GoMultiAbs(int[] _iMotrPhyAdds, double[] _dPoses , double _dVel)
        {
            for (int i = 0; i < _iMotrPhyAdds.Count(); i++ )
            {
                _dPoses[i] = UnitToPulse(_iMotrPhyAdds[i], _dPoses[i]);
            }
            if (Mtr[_iMotrPhyAdds[0]] is MotionAXL.CMotor)
            {
                Mtr[_iMotrPhyAdds[0]].GoMultiAbs(_iMotrPhyAdds, _dPoses, UnitToPulse(_iMotrPhyAdds[0], _dVel), 
                                                   UnitToPulse(_iMotrPhyAdds[0], (double)Para[_iMotrPhyAdds[0]].dAcceleration),
                                                   UnitToPulse(_iMotrPhyAdds[0], (double)Para[_iMotrPhyAdds[0]].dDeceleration));
            }
            else
            {
                Log.ShowMessage("Error","Function Not Supported!");
            }

        }
        public void GoInc(int _iMotrNo, double _dPos, double _dVel, double _dAcc, double _dDec)
        {
            Go(_iMotrNo, _dPos + GetCmdPos(_iMotrNo), _dVel, _dAcc, _dDec);

        }

        public void GoIncVel(int _iMotrNo, double _dPos, double _dVel)
        {
            Go(_iMotrNo, _dPos + GetCmdPos(_iMotrNo), _dVel, Para[_iMotrNo].dAcceleration, Para[_iMotrNo].dDeceleration);
        }

        public void GoIncRun(int _iMotrNo, double _dPos)
        {
            Go(_iMotrNo, _dPos + GetCmdPos(_iMotrNo), Para[_iMotrNo].dRunSpeed, Para[_iMotrNo].dAcceleration, Para[_iMotrNo].dDeceleration);
        }

        public void GoIncMan(int _iMotrNo, double _dPos)
        {
            Go(_iMotrNo, _dPos + GetCmdPos(_iMotrNo), Para[_iMotrNo].dManSpeed, Para[_iMotrNo].dAcceleration, Para[_iMotrNo].dDeceleration);
        }

        public void GoIncSlow(int _iMotrNo, double _dPos)
        {
            Go(_iMotrNo, _dPos + GetCmdPos(_iMotrNo), Para[_iMotrNo].dSlowSpeed, Para[_iMotrNo].dAcceleration, Para[_iMotrNo].dDeceleration);
        }


        public void SetTrgPos(int _iMotrNo, double[] _dPos, double _dTrgTime, bool _bActual, bool _bOnLevel)
        {
            if (_iMotrNo < 0 || _iMotrNo >= m_iMaxMotr)
            {
                Log.ShowMessageFunc(string.Format("ERR Motr{0} is not Between 0 and {1}", _iMotrNo, m_iMaxMotr));
                return;
            }

            double [] dPosPulses = new double [_dPos.Length];
            for(int i = 0 ; i < _dPos.Length ; i++)
            {
                dPosPulses[i] = UnitToPulse(_iMotrNo ,_dPos[i]);
            }


            Mtr[_iMotrNo].SetTrgPos(dPosPulses, _dTrgTime, _bActual, _bOnLevel);
        }

        public void ResetTrgPos(int _iMotrNo)
        {
            if (_iMotrNo < 0 || _iMotrNo >= m_iMaxMotr)
            {
                Log.ShowMessageFunc(string.Format("ERR Motr{0} is not Between 0 and {1}", _iMotrNo, m_iMaxMotr));
                return;
            }
            Mtr[_iMotrNo].ResetTrgPos();
        }

        public void OneShotTrg(int _iMotrNo, bool _bOnLevel, int _iTime)
        {
            if (_iMotrNo < 0 || _iMotrNo >= m_iMaxMotr)
            {
                Log.ShowMessageFunc(string.Format("ERR Motr{0} is not Between 0 and {1}", _iMotrNo, m_iMaxMotr));
                return;
            }
            Mtr[_iMotrNo].OneShotTrg(_bOnLevel,_iTime);
        }


        // 보간 구동을 위해 추가
        public void ContiSetAxisMap(int _iMotrNo , int _iCoord, uint _uiAxisCnt, int [] _iaAxisNo)
        {
            //아진 전용 함수.
            if(Mtr[_iaAxisNo[0]].GetType() == typeof(MotionAXL.CMotor)){
                int [] iaPhyAdd = new int [_uiAxisCnt];
                for(int i = 0 ; i < _uiAxisCnt ; i++)
                {
                    iaPhyAdd[i] = ((MotionAXL.CParaMotorAxl)Mtr[_iaAxisNo[i]].GetPara()).iPhysicalNo ;
                }
                Mtr[_iMotrNo].ContiSetAxisMap(_iCoord, _uiAxisCnt, iaPhyAdd) ;
            }
        }
        public void ContiSetAbsRelMode(int _iMotrNo , int _iCoord, uint _uiAbsRelMode)
        {
            Mtr[_iMotrNo].ContiSetAbsRelMode(_iCoord , _uiAbsRelMode);
        }
        public void ContiBeginNode(int _iMotrNo , int _iCoord)
        {
            Mtr[_iMotrNo].ContiBeginNode( _iCoord);
        }
        public void ContiEndNode(int _iMotrNo , int _iCoord)
        {
            Mtr[_iMotrNo].ContiEndNode(_iCoord);
        }
        public void ContiStart(int _iMotrNo , int _iCoord, uint _uiProfileset, int _iAngle)
        {
            Mtr[_iMotrNo].ContiStart(_iCoord,_uiProfileset,_iAngle);
        }
        public int GetContCrntIdx(int _iMotrNo , int _iCoord)
        {
            return Mtr[_iMotrNo].GetContCrntIdx(_iCoord);
        }
        public void LineMove (int _iMotrNo , int _iCoord, double []_daEndPos, double   _dVel , double   _dAcc    , double _dDec)
        {
            //double dPos = UnitToPulse(_iMotrNo, _dPos) ;
            double dVel = UnitToPulse(_iMotrNo, _dVel) ;
            double dAcc = UnitToPulse(_iMotrNo, _dAcc) ;
            double dDec = UnitToPulse(_iMotrNo, _dDec) ;
            for(int i = 0; i < _daEndPos.Length;i++) {
                _daEndPos[i] = UnitToPulse(_iMotrNo, _daEndPos[i]);
            }
            Mtr[_iMotrNo].LineMove(_iCoord, _daEndPos, dVel ,  dAcc  , dDec);
        }
        public void CircleCenterMove  (int _iMotrNo , int _iCoord, int []_iaAxisNo, double []_daCenterPos , double []_daEndPos, double _dVel, double _dAcc , double _dDec, uint _uiCWDir  )
        {
            Mtr[_iMotrNo].CircleCenterMove(_iCoord , _iaAxisNo, _daCenterPos , _daEndPos, _dVel, _dAcc , _dDec, _uiCWDir );
        }
        public void CirclePointMove   (int _iMotrNo , int _iCoord, int []_iaAxisNo, double []_daMidPos    , double []_daEndPos, double _dVel, double _dAcc , double _dDec, int _iArcCircle)
        {
            Mtr[_iMotrNo].CirclePointMove ( _iCoord, _iaAxisNo, _daMidPos , _daEndPos, _dVel, _dAcc , _dDec, _iArcCircle);
        }
        public void CircleRadiusMove  (int _iMotrNo , int _iCoord, int []_iaAxisNo, double   _dRadius     , double []_daEndPos, double _dVel, double _dAcc , double _dDec, uint _uiCWDir  , uint _uiShortDistance)
        {
            Mtr[_iMotrNo].CircleRadiusMove  (_iCoord, _iaAxisNo, _dRadius , _daEndPos, _dVel, _dAcc , _dDec, _uiCWDir , _uiShortDistance);
        }
        public void CircleAngleMove   (int _iMotrNo , int _iCoord, int []_iaAxisNo, double []_daCenterPos , double   _dAngle  , double _dVel, double _dAcc , double _dDec, uint _uiCWDir  )
        {
            Mtr[_iMotrNo].CircleAngleMove (_iCoord, _iaAxisNo, _daCenterPos , _dAngle  , _dVel, _dAcc , _dDec, _uiCWDir  );
        }
        public void StartRepeat(int _iMotrNo)
        {
            if (_iMotrNo < 0 || _iMotrNo >= m_iMaxMotr)
            {
                Log.ShowMessageFunc(string.Format("ERR Motr{0} is not Between 0 and {1}", _iMotrNo, m_iMaxMotr));
                return;
            }
            Stat[_iMotrNo].iRepeatCntRemain = Para[_iMotrNo].iRepeatCount;
            GoAbsVel(_iMotrNo, Para[_iMotrNo].dFstPos, Para[_iMotrNo].dRepeatSpeed);
        }



        public void Update()
        {
            
            for (int i = 0; i < m_iMaxMotr; i++)
            {
                Mtr[i].Update();
                if (Stat[i].dPreCmdPos == GetCmdPos(i))
                {
                    if (GetStop(i))
                    {
                        Stat[i].bMovingP = false;
                        Stat[i].bMovingN = false;
                    }
                }
                else if (Stat[i].dPreCmdPos > GetCmdPos(i))
                {
                    Stat[i].bMovingP = false;
                    Stat[i].bMovingN = true ;
                }
                else
                {
                    Stat[i].bMovingP = true;
                    Stat[i].bMovingN = false;
                }
                Stat[i].dPreCmdPos = GetCmdPos(i);

                   

                //Check Limit Software Pos.
                bool bHoming = GetHoming(i);
                double dPos;
                if (Para[i].bExistEnc)
                {
                    dPos = GetEncPos(i);
                }
                else
                {
                    dPos = GetCmdPos(i);
                }
                
                if (GetServo(i) && GetHomeDone(i) && !GetStop(i))
                {
                    //+포지션 리밋 처리.
                    if (Para[i].dMaxPos != 0.0)//+리밋 포지션 세팅 한경우만.
                    {
                        if ((dPos > Para[i].dMaxPos + Para[i].dInposition) && Stat[i].bMovingP)
                        {

                                Stop(i);
                                Log.Trace(Para[i].sMotrName + " Motr Stop", "+Position Software Limit Stop");
                            
                        }
                    }

                    //-포지션 리밋 처리.
                    if ((dPos < Para[i].dMinPos - Para[i].dInposition) && Stat[i].bMovingN)
                    {

                        Stop(i);
                        Log.Trace(Para[i].sMotrName + " Motr Stop", "-Position Software Limit Stop");

                    }
                }



                //Check Limit Sensor.
                //홈스텝에서 잡게 되면 Z축 밑으로 리밑 치고 있을때 홈을 못잡음.
                if (!GetStop(i) && GetServo(i) && !bHoming)
                {
                    //+리밋 센서 처리.
                    if (GetPLimSnsr(i) && Stat[i].bMovingP)
                    {

                        EmgStop(i);
                        Log.Trace(Para[i].sMotrName + " Axis", "P EndLim Sensor Touched");

                    }

                    //-리밋 센서 처리.
                    if (GetNLimSnsr(i) && Stat[i].bMovingN)
                    {
                        EmgStop(i);
                        Log.Trace(Para[i].sMotrName + " Axis", "N EndLim Sensor Touched");
                    }

                }

                if(Stat[i].iRepeatCntRemain > 0)
                {
                    CycleRpt(i);
                }

                
                if(Homming[i] ){
                    HomeDone[i]=Mtr[i].GetHomeDone();
                }

                
                



            }
        }
        public void CycleRpt(int _iMotrNo)
        {
            if (!GetServo(_iMotrNo) || GetAlarmSgnl(_iMotrNo))
            {
                Stat[_iMotrNo].iRepeatCntRemain = 0;
                return ;
            }

            if (Stat[_iMotrNo].iRepeatCntRemain == Para[_iMotrNo].iRepeatCount)
            {
                CclTimer[_iMotrNo].Clear();
            }

            Stat[_iMotrNo].dRepeatTackTime = CclTimer[_iMotrNo].CheckTime_ms();

            

            if(GetStop(_iMotrNo))
            {
                if(RptTimer[_iMotrNo].OnDelay((int)Para[_iMotrNo].iRepeatDelay))
                {
                    if (GetCmdPos(_iMotrNo) <= Para[_iMotrNo].dFstPos+Para[_iMotrNo].dInposition &&
                        GetCmdPos(_iMotrNo) >= Para[_iMotrNo].dFstPos-Para[_iMotrNo].dInposition )
                    {
                        
                        
                        GoAbsVel(_iMotrNo, Para[_iMotrNo].dScdPos, Para[_iMotrNo].dRepeatSpeed);
                        
                    }
                    else
                    {
                        
                        GoAbsVel(_iMotrNo, Para[_iMotrNo].dFstPos, Para[_iMotrNo].dRepeatSpeed);
                        Stat[_iMotrNo].iRepeatCntRemain--;
                        
                    }
                    
                }

            }
            else
            {
                RptTimer[_iMotrNo].Clear();
            }

        }

        public MOTION_DIR GetDirType(int _iMotrNo)
        {
            return Para[_iMotrNo].iDirType;
        }

        public string GetName(int _iMotrNo)
        {
            return Para[_iMotrNo].sMotrName;
        }

        public double GetMinPosition(int _iMotrNo)
        {
            return Para[_iMotrNo].dMinPos;
        }

        public double GetMaxPosition(int _iMotrNo)
        {
            return Para[_iMotrNo].dMaxPos;
        }

        public bool GetX(int _iMaxMotr, int _iNo)
        {
            return Mtr[_iMaxMotr].GetX(_iNo);
        }

        public bool GetY(int _iMaxMotr, int _iNo)
        {
            return Mtr[_iMaxMotr].GetY(_iNo);
        }

        public void SetY(int _iMaxMotr, int _iNo, bool _bOn)
        {
            Mtr[_iMaxMotr].SetY(_iNo, _bOn);
        }

    }

}


