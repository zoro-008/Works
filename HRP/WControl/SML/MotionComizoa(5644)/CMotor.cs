using CMDLL;
using COMMON;
using MotionInterface;
using System;
using System.ComponentModel;
using Cmmsdk = CMDLL.SafeNativeMethods;

namespace MotionComizoa
{
    public enum ENC_MULTI : uint 
    {
        MODE_AB1X  = 0, //1채배
        MODE_AB2X  = 1, //2채배
        MODE_AB4X  = 2, //4채배
        MODE_CWCCW = 3, //CW/CCW(A펄스 - 카운트 증가, B펄스 - 카운트 감소)
        MODE_STEP  = 4  //스텝모터(엔코더 피드백 없는 경우) 사용 시
    }

    public enum ENC_INPUT : uint
    {
        Normal  = 0,
        Inverse = 1
    }

    public enum PULSE_MODE : uint
    {
        PulseDirection0 = 0,
        PulseDirection1 = 1,
        PulseDirection2 = 2,
        PulseDirection3 = 3,
        CwCCw0          = 4,
        CwCCw1          = 5,
        CwCCw2          = 6,
        CwCCw3          = 7,
        ABPhaseMode0    = 8,   
        ABPhaseMode1    = 9,
    }

    public enum LOGIC : uint
    {
        LOGIC_A = 0,//ARBEIT_NO = 0,
        LOGIC_B = 1,//BREAK_NC  = 1
    }

    public enum SPEED_MODE : uint
    {
        //public enum _TCmSpeedMode{ cmSMODE_KEEP=-1/* Keep previous setting*/, cmSMODE_C=0, cmSMODE_T, cmSMODE_S };
        CONSTANT    = 0 , //0 또는 cmSMODE_C CONSTANT 속도모드 => 가감속을 수행하지 않습니다.
        TRAPEZOIDAL = 1 , //1 또는 cmSMODE_T TRAPEZOIDAL 속도모드 => 사다리꼴 가감속을 수행합니다.
        S_CURVE     = 2 , //2 또는 cmSMODE_S S-CURVE 속도모드 => S-CURVE 가감속을 수행합니다.
                          //-1 또는 cmSMODE_KEEP 이전 속도 모드는 그대로 설정합니다.
    }

    public enum HOME_SIGNAL : uint
    {
        PLimitSensor = 0 ,
        NLimitSensor = 1 ,
        HomeSensor   = 4 ,
        ZPhase       = 5 
    }

    public enum Z_METHOD : uint
    {
        NotUse           = 0 ,
        HomeDirection    = 1 ,
        NotHomeDirection = 2 
    }

    public enum GANTRY_MATHOD : uint
    {
        NotUse       = 0,
        BothUse      = 1,
        MasureOffset = 2
    }

    public struct MOTION_STAT
    {
        public bool bHomeSnsr;
        public bool bNLimSnsr;
        public bool bPLimSnsr;
        public bool bZphaseSgnl;
        public bool bAlarmSgnl;
        public bool bInPosSgnl;
    }

    public struct MOTION_INFO
    {
        public int iCmdPos;      // Command 위치[0x01]
        public int iActPos;      // Encoder 위치[0x02]
        //public uint uMechSig;       // Mechanical Signal[0x04]
        //public uint uDrvStat;       // Driver Status[0x08]
        //public uint uInput;         // Universal Signal Input[0x10]
        //public uint uOutput;        // Universal Signal Output[0x10]
        //public uint uMask;          // 읽기 설정 Mask Ex) 0x1F, 모든정보 읽기
    }

    [Serializable]
    public class CParaMotorComi
    {   
        [CategoryAttribute("ComiPara"    ), DisplayNameAttribute("Physical No"          )]public int           iPhysicalNo    {get; set;} //실제모터 물리 어드레스. 
        [CategoryAttribute("ComiPara"    ), DisplayNameAttribute("Speed Mode"           )]public SPEED_MODE    eSpeedMode     {get; set;} // 가감속 수행 안함, 사다리꼴 가감속, S-CURVE, 이전속도
        //[CategoryAttribute("ComiPara"    ), DisplayNameAttribute("S Curve Dcc Percent"  )]public double        dSCurveDcPer   {get; set;} // S-Curve구동 Dec Percent , 
        [CategoryAttribute("ComiPara"    ), DisplayNameAttribute("Use Inposition Signal")]public bool          bUseInpos      {get; set;} // 서보팩의 인포지션 시그널 이용 여부. 
        [CategoryAttribute("ComiPara"    ), DisplayNameAttribute("EncMulti"             )]public ENC_MULTI     eEncMulti      {get; set;} // 엔코더 입력 모드 설정
        [CategoryAttribute("ComiPara"    ), DisplayNameAttribute("EncInputMode"         )]public ENC_INPUT     eEncInputMode  {get; set;} // 엔코더 방향 설정
        [CategoryAttribute("ComiPara"    ), DisplayNameAttribute("Pulse Output"         )]public PULSE_MODE    ePulseOutput   {get; set;} // 펄스 출력 방식 설정             PULSE_OUTPUT 
        
        [CategoryAttribute("ComiPara"    ), DisplayNameAttribute("Limit Phase"          )]public LOGIC         eLimPhase      {get; set;} // 정방향 리미트(+End limit)의 액티브레벨 설정 
        //[CategoryAttribute("ComiPara"    ), DisplayNameAttribute("Minas Limit Phase"    )]public LOGIC         eNLimPhase     {get; set;} // 역방향 리미트(-End limit)의 액티브레벨 설정 
        [CategoryAttribute("ComiPara"    ), DisplayNameAttribute("Alram Phase"          )]public LOGIC         eAlarmPhase    {get; set;} // 알람(Alarm) 신호 액티브레벨 설정 
        [CategoryAttribute("ComiPara"    ), DisplayNameAttribute("Inposition Phase"     )]public LOGIC         eInposPhase    {get; set;} // 인포지션(Inposition) 신호 액티브레벨 설정 
        [CategoryAttribute("ComiPara"    ), DisplayNameAttribute("Home Phase"           )]public LOGIC         eHomePhase     {get; set;} // 홈 엑티브 레벨
        [CategoryAttribute("ComiPara"    ), DisplayNameAttribute("Z Phase"              )]public LOGIC         eZphaPhase     {get; set;} 
        [CategoryAttribute("ComiPara"    ), DisplayNameAttribute("Servo Phase"          )]public LOGIC         eServoPhase    {get; set;} 
        
        [CategoryAttribute("ComiPara"    ), DisplayNameAttribute("Set Max Position"     )]public double        dSetMaxPos     {get; set;} //모터 최대 표시값 지정.링카운터
        //다이렉트케이블이 같이 엮겨 있을때 Machine에서 그냥 MT_SetY로 쓰기때문에 사용한하게 됨. 
        //[CategoryAttribute("ComiPara"    ), DisplayNameAttribute("Break Off IO Address" )]public int           iBreakOffAdd   {get; set;} //브레이크 타입 브레이크 IO Address
        //[CategoryAttribute("ComiPara"    ), DisplayNameAttribute("Use Break"            )]public bool          bUseBreak      {get; set;} //브레이크 타입 모터 설정.

        [CategoryAttribute("GantryPara" ), DisplayNameAttribute("Gantry Enable"        )]public bool           bGantryEnable  {get; set;} 
        [CategoryAttribute("GantryPara" ), DisplayNameAttribute("Gantry SubPhyAdd"     )]public int            iGantrySubAdd  {get; set;} 
        //[CategoryAttribute("GantryPara" ), DisplayNameAttribute("Gantry Mathod"        )]public GANTRY_MATHOD  eGantryMathod  {get; set;} 
        //[CategoryAttribute("GantryPara" ), DisplayNameAttribute("Gantry Offset"        )]public double         dGantryOffset  {get; set;} 
        //[CategoryAttribute("GantryPara" ), DisplayNameAttribute("Gantry Offset Range"  )]public double         dGantryOfsRange{get; set;} 

        [CategoryAttribute("HomePara"   ), DisplayNameAttribute("Home Direction is Neg")]public bool           bHomeNegDir    {get; set;}
        [CategoryAttribute("HomePara"   ), DisplayNameAttribute("Home Signal Selection")]public HOME_SIGNAL    eHomeSignal    {get; set;} 
        [CategoryAttribute("HomePara"   ), DisplayNameAttribute("Home Z Mathod"        )]public Z_METHOD       eHomeZMethod   {get; set;} 
        //[CategoryAttribute("HomePara"   ), DisplayNameAttribute("Home Clear Delay Time")]public double         dHomeClrTime   {get; set;}
        [CategoryAttribute("HomePara"   ), DisplayNameAttribute("Home Offset"          )]public double         dHomeOffset    {get; set;}
    } 

    


    public class CMotor : IMotor
    {
        private CParaMotorComi Para = new CParaMotorComi();
        private MOTION_INFO MotionInfo;
        private MOTION_STAT MotionStat;
        double m_dPulsePerUnit;

        private int    m_iTotalAxis;
        private string m_sCmePath  ;

        public CMotor() { }

        /// <summary>
        /// 초기화 함수.
        /// </summary>
        /// <returns>초기화 성공여부</returns>
        public bool Init()
        {
            //Local Var.
            bool bExtCmeFile = false;
            String Path = m_sCmePath;
            //Check Already Init.
            if (m_iTotalAxis > 0) return true; //이미 Initial 했으므로 

            try
            {
                if (Cmmsdk.cmmGnDeviceLoad((int)MotnDefines._TCmBool.cmTRUE, ref m_iTotalAxis) != MotnDefines.cmERR_NONE)
                {
                    Log.ShowMessage("Motor", "[Comizoa Motion]Cannot Load Device Fail!");
                    return false;
                }
                if (m_iTotalAxis <= 0)
                {
                    Log.ShowMessage("Motor", "[Comizoa Motion]Cannot Load Device (Load Axis is Zero!)");
                    return false;
                }
                //Check Exist CME2 File.
                //bExtCmeFile = FNC.FileExists(Path);
                //if (bExtCmeFile) Cmmsdk.cmmGnInitFromFile(Path);
                //Cmmsdk.cmmAdvGetNumDefinedAxes(ref m_iTotalAxis);
            }
            catch (Exception ex)
            {
                Log.Trace("TAxisComi. Open " + ex.ToString());
            }
            //Return.
            return (m_iTotalAxis > 0);
        }
        /// <summary>
        /// 종료 함수
        /// </summary>
        /// <returns>종료 성공여부</returns>
        public bool Close()
        {
            return true;
        }
        /// <summary>
        /// 모터의 인포지션 확인 안한 정지여부.
        /// </summary>
        /// <returns>정지여부</returns>
        public bool GetStop()
        {
            return MotionStat.bInPosSgnl;
        }

        /// <summary>
        /// 서보 온오프
        /// </summary>
        /// <param name="_bOn">온오프</param>
        public void SetServo(bool _bOn)
        {
            int iOn ;//= _bOn ? 0 : 1;
            if (Para.eServoPhase == LOGIC.LOGIC_B) iOn = _bOn ? 0 : 1;
            else                                   iOn = _bOn ? 1 : 0;                     

            Stop();

            //다이렉트케이블이 같이 엮겨 있을때 Machine에서 그냥 MT_SetY로 쓰기때문에 사용한하게 됨. 
            //if (Para.bUseBreak) SetY(Para.iBreakOffAdd, _bOn);

            Cmmsdk.cmmGnSetServoOn(Para.iPhysicalNo, iOn);

            if (!_bOn)
            {
                SetHomeDone(false);
            }
            //else
            //{
            //    //가끔 장비껐다가 켜서 홈잡을때 겐트리가 안엮여서 병신짓 할때 있어서 
            //    //서보 온에서 한번씩 해본다. 
            //    //밖에 사이클홈에서 홈시작전에 확인 하여 알람 띄워보자.
            //    //동기구동을 Gantry 이용해서 거는데 Para.bGantryEnable 조건 없으면
            //    //Reset할때마다 GetLinkMode false 시켜서 조건 건다. 진섭.
            //    if (Para.bGantryEnable)
            //    {
            //        SetGantryEnable();
            //    }
            //
            //}
        }
        /// <summary>
        /// 서보상태를 받아옴.
        /// </summary>
        /// <returns>온오프</returns>
        public bool GetServo()
        {
            int iRet = -1;
            int iOn = 0;
            //bRet = ((MotionInfo.uOutput >> 0) & 0x01) == 0x01;
            iRet = Cmmsdk.cmmGnGetServoOn(Para.iPhysicalNo, ref iOn);

            bool bRet = Convert.ToBoolean(iOn);
            if (Para.eServoPhase == LOGIC.LOGIC_B) bRet = !bRet;
            
            return Convert.ToBoolean(bRet);//== MotnDefines.cmERR_NONE;
        }

        /// <summary>
        /// 리셋 시그널 온오프 제어
        /// </summary>
        /// <param name="_bOn">온오프</param>
        public void SetReset(bool _bOn)
        {
            if(_bOn) Cmmsdk.cmmGnSetAlarmRes(Para.iPhysicalNo,1);
            else     Cmmsdk.cmmGnSetAlarmRes(Para.iPhysicalNo,0);

            //public static extern unsafe int cmmGnPulseAlarmRes(
            //[MarshalAs(UnmanagedType.I4)] int Axis,
            //[MarshalAs(UnmanagedType.I4)] int IsOnPulse,
            //[MarshalAs(UnmanagedType.I4)] int dwDuration,
            //[MarshalAs(UnmanagedType.I4)] int IsWaitPulseEnd);
            //if(_bOn) Cmmsdk.cmmGnPulseAlarmRes(Para.iPhysicalNo,0,100,100);
            //else     Cmmsdk.cmmGnPulseAlarmRes(Para.iPhysicalNo,0,100,100);
            
        }

        /// <summary>
        /// 펄스 커멘드 리턴.
        /// </summary>
        /// <returns>펄스 리턴</returns>
        public double GetCmdPos()
        {
            return MotionInfo.iCmdPos;
            //int iCount = 0;
            //Cmmsdk.cmmStGetCount(Para.iPhysicalNo, (int)MotnDefines._TCmCntr.cmCNT_COMM, ref iCount);
            //return iCount;
        }

        /// <summary>
        /// 펄스 엔코더 리턴.
        /// </summary>
        /// <returns>펄스 리턴</returns>
        public double GetEncPos()
        {
            return MotionInfo.iActPos;
            //int iCount = 0;
            //Cmmsdk.cmmStGetCount(Para.iPhysicalNo, (int)MotnDefines._TCmCntr.cmCNT_FEED, ref iCount);
            //return iCount;
        }

        /// <summary>
        /// Command Encoder Target포지션 세팅.
        /// </summary>
        /// <param name="_dPos">세팅할 펄스</param>
        public void SetPos(double _dPos)
        {
            Cmmsdk.cmmStSetPosition(Para.iPhysicalNo, 0, _dPos); //Command position
            Cmmsdk.cmmStSetPosition(Para.iPhysicalNo, 1, _dPos); //Feedback position
        }

        //Signal...
        /// <summary>
        /// 홈센서 시그널.
        /// </summary>
        /// <returns>OnOff상태</returns>
        public bool GetHomeSnsr()
        {
            return MotionStat.bHomeSnsr;
        }

        /// <summary>
        /// -리밋 센서 시그널
        /// </summary>
        /// <returns>OnOff상태</returns>
        public bool GetNLimSnsr()
        {
            return MotionStat.bNLimSnsr;
        }

        /// <summary>
        /// +리밋 센서 시그널
        /// </summary>
        /// <returns>OnOff상태</returns>
        public bool GetPLimSnsr()
        {
            return MotionStat.bPLimSnsr;
        }

        /// <summary>
        /// 엔코더 Z상 시그널 상태
        /// </summary>
        /// <returns>OnOff상태</returns>
        public bool GetZphaseSgnl()
        {
            return MotionStat.bZphaseSgnl;
        }

        /// <summary>
        /// 서보펙 인포지션 시그널 상태
        /// </summary>
        /// <returns>OnOff상태</returns>
        public bool GetInPosSgnl() //Servo Pack InPosition Signal.
        {
            return MotionStat.bInPosSgnl;
        }

        /// <summary>
        /// 서보펙 알람 시그널 상태
        /// </summary>
        /// <returns>OnOff상태</returns>
        public bool GetAlarmSgnl()
        {
            return MotionStat.bAlarmSgnl;
        }

        //Home씨퀜스.

        /// <summary>
        /// 홈을 수행한다.
        /// </summary>
        /// <returns>수행 성공여부.</returns>
        public bool GoHome(double _dHomeVelFirst, double _dHomeVelLast, double _dHomeAccFirst)
        {
            //원점복귀 작업을 수행합니다. cmmHomeMove() 함수는 모션이 완료되기 전까지 반환되지 않으며,
            //cmmHomeMoveStart() 함수는 모션을 시작시킨 후에 바로 반환됩니다.
            SetServo(true);

            //Home Mode
            //0 : ORG > Stop
            //1 : ORG > Stop > Back (Vr) > ORG-OFF > Foward(Vr) > ORG > Stop
            //2 : ORG > Slow down (Vini) > Stop on EZ Count
            //3 : ORG > Stop on EZ Count
            //4 : ORG > Stop > Back (Vr) > Stop on EZ Count
            //5 : ORG > Stop > Back (Vwork) > Stop on EZ
            //6 : EL > Stop > Back (Vr) > EL 0 > Stop
            //7 : EL > Stop > Back (Vr) > Stop on EZ count
            //8 : EL > Stop > Back (Vwork) > Stop on EZ count
            //9 : MODE0 > Operate till dev. counter 0
            //10 : MODE3 > Operate till dev. counter 0
            //11 : MODE5 > Operate till dev. counter 0
            //12 : MODE8 > Operate till dev. counter 0
            int iHomeMode = 0;

            if(Para.eHomeZMethod == Z_METHOD.HomeDirection)
            {
                if(Para.eHomeSignal == HOME_SIGNAL.HomeSensor  ) iHomeMode = 3;
                if(Para.eHomeSignal == HOME_SIGNAL.NLimitSensor) iHomeMode = 7;
                if(Para.eHomeSignal == HOME_SIGNAL.PLimitSensor) iHomeMode = 7;
                if(Para.eHomeSignal == HOME_SIGNAL.ZPhase      ) iHomeMode = 3;  //이거는 없네;;;
            }
            else if(Para.eHomeZMethod == Z_METHOD.NotHomeDirection)
            {
                if(Para.eHomeSignal == HOME_SIGNAL.HomeSensor  ) iHomeMode = 4;
                if(Para.eHomeSignal == HOME_SIGNAL.NLimitSensor) iHomeMode = 7;
                if(Para.eHomeSignal == HOME_SIGNAL.PLimitSensor) iHomeMode = 7;
                if(Para.eHomeSignal == HOME_SIGNAL.ZPhase      ) iHomeMode = 3;  //이거는 없네;;;
            }
            else
            {
                if(Para.eHomeSignal == HOME_SIGNAL.HomeSensor  ) iHomeMode = 1;
                if(Para.eHomeSignal == HOME_SIGNAL.NLimitSensor) iHomeMode = 6;
                if(Para.eHomeSignal == HOME_SIGNAL.PLimitSensor) iHomeMode = 6;
                if(Para.eHomeSignal == HOME_SIGNAL.ZPhase      ) iHomeMode = 1;
            }

            //Manual
            //long nHomeMode = 0; // 원점 복귀 모드 설정. 0 ~ 12 번 모드가 있습니다.
            //long nHomeDir = cmDIR_N; // 원점 복귀 방향. cmDIR_N: (-) 방향, cmDIR_P: (+) 방향
            //long nEzCount = 0; // Encoder Z 상 카운트. ‘0’ 은 EZ 상 1 회 카운트를 의미합니다.
            //double fEscDist = 10.0f; // 원점 탈출 거리. 자동 원점 검색 기능에 사용되며,
            //// 최소 ‘1’ 이상의 값이어야 합니다.
            //double fOffset = 0.0f; // 원점 복귀 완료 후 Offset 값 (상대 거리)
            int iHomeDir = Para.bHomeNegDir ? 0 : 1; // 0 - N , 1 - P
            Cmmsdk.cmmHomeSetConfig(Para.iPhysicalNo, iHomeMode, iHomeDir, 1, Para.dHomeOffset * m_dPulsePerUnit);

            //iHomeClearMode
            //0 : Clears both position(C&F) at the moment ORG(/EL/EZ) signal works.
            //1 : Clears both position(C&F) after home completes.
            //2 : Set command smae as feedback after home completes.
            //-1 : Disable HomePosClearMode.
            int iHomeClearMode = 1;
            Cmmsdk.cmmHomeSetPosClrMode(Para.iPhysicalNo, iHomeClearMode);
            
            //Vr(Reverse Speed)
            //▶ Revel : cmmHomeSetSpeedPattern 함수의 인자이며, Reverse Speed 를 설정합니다. 복귀모드에 따라 Reverse Speed 를
            //필요로 하는 모드가 있습니다. 앞의 복귀 모드 설명에서 Reverse Speed 는 Vr 로 표기되었습니다.
            //SpeedMode
            //0 또는 cmSMODE_C CONSTANT 속도모드 => 가감속을 수행하지 않습니다.
            //1 또는 cmSMODE_T TRAPEZOIDAL 속도모드 => 사다리꼴 가감속을 수행합니다.
            //2 또는 cmSMODE_S S-CURVE 속도모드 => S-CURVE 가감속을 수행합니다.
            Cmmsdk.cmmHomeSetSpeedPattern(Para.iPhysicalNo, (int)MotnDefines._TCmSpeedMode.cmSMODE_T, _dHomeVelFirst, _dHomeAccFirst, _dHomeAccFirst, _dHomeVelLast);
            //0 또는 cmDIR_N (-) 방향
            //1 또는 cmDIR_P (+) 방향
            int iFuncRet = Cmmsdk.cmmHomeMoveStart(Para.iPhysicalNo, iHomeDir);

            return (iFuncRet == MotnDefines.cmERR_NONE);
        }
        /// <summary>
        /// 홈동작 완료 확인.
        /// </summary>
        /// <returns>홈동작 완료여부</returns>
        public bool GetHomeDone()
        {
            int iRet = 0;
            bool bRet;
            const int HOME_SUCCESS = 1;       // 홈 완료
            Cmmsdk.cmmHomeGetSuccess(Para.iPhysicalNo, ref iRet);
            bRet = (iRet == HOME_SUCCESS);

            return bRet;
        }
        /// <summary>
        /// 강제로 홈돈 시그널 세팅.
        /// </summary>
        /// <param name="_bOn">세팅 값.</param>
        public void SetHomeDone(bool _bOn)
        {
            int iOn = _bOn ? 1 : 0;
            Cmmsdk.cmmHomeSetSuccess(Para.iPhysicalNo, iOn); //HomeDone 강제 Setting. 0 == false, 1 == true. 진섭
        }
        /// <summary>
        /// 홈시퀜스를 중단한다.
        /// </summary>
        public void StopHome()
        {
            Stop();

            SetHomeDone(false);
        }
        /// <summary>
        /// 홈이 진행중인지 확인.
        /// </summary>
        public bool GetHoming()
        {
            int IsBusy = 1;
            int iBusy = 0;
            Cmmsdk.cmmHomeIsBusy(Para.iPhysicalNo, ref iBusy);
            return (iBusy == IsBusy);
        }


        //Motion Functions.
        /// <summary>
        /// 모터 정지 명령.
        /// </summary>
        public void Stop()
        {
            Cmmsdk.cmmSxStop(Para.iPhysicalNo, 0, 1);
        }
        /// <summary>
        /// 모터 감속 무시 긴급정지.
        /// </summary>
        public void EmgStop() //Stop Without Deceleration.
        {
            Cmmsdk.cmmSxStopEmg(Para.iPhysicalNo);
        }

        //내부 설정에 따라 Acc Dcc는 가감속율 즉 초당 증가pulse단위 
        /// <summary>
        /// +방향 조그 이동.
        /// </summary>
        /// <param name="_dVel">초당 구동 속도 펄스.</param>
        /// <param name="_dAcc">구동 가속율 펄스 </param>
        /// <param name="_dDec">감속율 펄스</param>
        public void JogP(double _dVel, double _dAcc, double _dDec) //Jogging to CW.
        {
            //Check Status.
            //if (m_bAlarm ) return false;
            //if (!m_bServo) return false;
            //
            //if (Vel <= 0 ) return false;
            //if (Acc <= 0 ) return false;
            //if (Dec <= 0 ) Acc = Dec;

            //double dVel = ConvVel(Vel);
            //double dAcc = ConvAcc(Vel, Acc);
            //double dDec = ConvAcc(Vel, Dec);

            int iDir    = (int)MotnDefines._TCmDir.cmDIR_P;

            Cmmsdk.cmmCfgSetSpeedPattern(Para.iPhysicalNo, (int)Para.eSpeedMode, _dVel, _dAcc, _dDec);
            int iRet = Cmmsdk.cmmSxVMoveStart(Para.iPhysicalNo, iDir) ;
        }
        /// <summary>
        /// -방향 조그 이동
        /// </summary>
        /// <param name="_dVel">초당 구동 속도 펄스</param>
        /// <param name="_dAcc">구동 가속율 펄스</param>
        /// <param name="_dDec">감속율 펄스</param>
        public void JogN(double _dVel, double _dAcc, double _dDec) //Jogging to CCW.
        {
            //Move Jog.
            //Check Status.
            //if (m_bAlarm ) return false;
            //if (!m_bServo) return false;
            //if (Vel <= 0 ) return false;
            //if (Dec <= 0 ) Acc = Dec;
            
            //double dVel = ConvVel(Vel);
            //double dAcc = ConvAcc(Vel, Acc);
            //double dDec = ConvAcc(Vel, Dec);
            int iDir    = (int)MotnDefines._TCmDir.cmDIR_N;
            //
            Cmmsdk.cmmCfgSetSpeedPattern(Para.iPhysicalNo, (int)Para.eSpeedMode, _dVel, _dAcc, _dDec);
            Cmmsdk.cmmSxVMoveStart(Para.iPhysicalNo, iDir) ;
        }

        /// <summary>
        /// OverrideVel을 하기전에 구동속도중에 가장 높은 놈을 세팅을 한다.
        /// </summary>
        /// <param name="_dMaxVel">가장높은 속도값</param>
        /// <returns></returns>
        public bool SetOverrideMaxVel(double _dMaxVel)
        {
            return true;
        }

        /// <summary>
        /// 구동중 속도를 오버라이딩한다.
        /// 정속구간에서만 먹는 함수 이고. SetOverrideMaxVel 구동 속도중 가장 빠른 속도를 세팅 하고 수행해야함.
        /// </summary>
        /// <param name="_dVel">오버라이딩 할 속도</param>
        /// <returns></returns>
        public bool OverrideVel(double _dVel)
        {
            return true;
        }

        /// <summary>
        /// 홈센서 기준 절대 위치로 이동.
        /// </summary>
        /// <param name="_dPos">이동할 위치 펄스단위</param>
        /// <param name="_dVel">이동할 속도 펄스단위</param>
        /// <param name="_dAcc">이동할 가속 펄스단위</param>
        /// <param name="_dDec">이동할 감속 펄스단위</param>
        /// <returns>성공여부</returns>
        public void GoAbs(double _dPos, double _dVel, double _dAcc, double _dDec)  //abs move
        {
            //SpeedMode
            //0 또는 cmSMODE_C CONSTANT 속도모드 => 가감속을 수행하지 않습니다.
            //1 또는 cmSMODE_T TRAPEZOIDAL 속도모드 => 사다리꼴 가감속을 수행합니다.
            //2 또는 cmSMODE_S S-CURVE 속도모드 => S-CURVE 가감속을 수행합니다.
            //-1 또는 cmSMODE_KEEP 이전 속도 모드는 그대로 설정합니다.
            //int iSMode = Para.dSCurveAcPer == 0 || Para.dSCurveDcPer == 0 ? (int)MotnDefines._TCmSpeedMode.cmSMODE_T : (int)MotnDefines._TCmSpeedMode.cmSMODE_S;
            Cmmsdk.cmmCfgSetSpeedPattern(Para.iPhysicalNo, (int)Para.eSpeedMode, _dVel, _dAcc, _dDec);
            int iRet = Cmmsdk.cmmSxMoveToStart(Para.iPhysicalNo, _dPos);
        }


        /// <summary>
        /// 홈센서 기준 절대 위치로 다축 이동.
        /// </summary>
        /// <param name="_iMotrPhyAdds">이동할축들 0번이 마스터</param>
        /// <param name="_dPoses">이동할 위치 펄스단위</param>
        /// <param name="_dVel">이동할 속도 펄스단위</param>
        /// <param name="_dAcc">이동할 가속 펄스단위</param>
        /// <param name="_dDec">이동할 감속 펄스단위</param>
        /// <returns>성공여부</returns>
        public void GoMultiAbs(int[] _iMotrPhyAdds, double[] _dPoses, double _dVel, double _dAcc, double _dDec)
        {

        }

        /// <summary>
        /// 시그널 찾아서 감속 정지한다..
        /// </summary>
        /// <param name="_dVel">구동 속도 펄스단위</param>
        /// <param name="_dAcc">구동 가속 펄스단위</param>
        /// <param name="_eSignal">사용 시그널</param>
        /// <param name="_bEdgeUp">업엣지인지 다운엣지인지.</param>
        /// <returns></returns>
        public void FindEdgeStop(double _dVel, double _dAcc, EN_FIND_EDGE_SIGNAL _eSignal, bool _bEdgeUp, bool _bEmgStop)
        {

        }


        /// <summary>
        /// 현재 파라미터들을 아진 함수를 이용하여 세팅함.
        /// </summary>
        public void ApplyPara(double _dPulsePerUnit)
        {
            m_dPulsePerUnit = _dPulsePerUnit;

            int iRet2;
            int iMode;

            //S-Curve 설정할때 가/감속을 Percentage로 변환해서 셋팅해야하는데 셋팅된 가/감속 가져오기 애매해서 주석처리.
            //if (Para.dSCurveAcPer == 0 && Para.dSCurveDcPer == 0)//커브 안쓸때.
            //{
            //    iMode = (int)MotnDefines._TCmSpeedMode.cmSMODE_T;//대칭      
            //}
            //else
            //{
            //    iMode = (int)MotnDefines._TCmSpeedMode.cmSMODE_S;//비대칭
            //}
            ////S-Curve 설정
            //Cmmsdk.cmmCfgSetSpeedPattern(Para.iPhysicalNo, iMode, 2000, 10000, 10000 );

            //Use Inpos
            
            int iUse = Para.bUseInpos ? 1 : 0;
            int iRet = Cmmsdk.cmmCfgSetMioProperty(Para.iPhysicalNo, (int)MotnDefines._TCmMioPropId.cmINP_EN, iUse);

            //EncInputMode 설정
            Cmmsdk.cmmCfgSetInMode(Para.iPhysicalNo, (int)Para.eEncMulti, (int)Para.eEncInputMode);
            //Pulse Output 설정
            int iRst = Cmmsdk.cmmCfgSetOutMode(Para.iPhysicalNo, (int)Para.ePulseOutput);

            //하드웨어 리밋
            Cmmsdk.cmmCfgSetMioProperty(Para.iPhysicalNo, (int)MotnDefines._TCmMioPropId.cmEL_MODE   , 1 ); // 0-즉시정지,1-감속정지
            Cmmsdk.cmmCfgSetMioProperty(Para.iPhysicalNo, (int)MotnDefines._TCmMioPropId.cmEL_LOGIC  , (int)Para.eLimPhase  );

            Cmmsdk.cmmCfgSetMioProperty(Para.iPhysicalNo, (int)MotnDefines._TCmMioPropId.cmALM_MODE  , 1 ); // 0-즉시정지,1-감속정지
            Cmmsdk.cmmCfgSetMioProperty(Para.iPhysicalNo, (int)MotnDefines._TCmMioPropId.cmALM_LOGIC , (int)Para.eAlarmPhase);

            Cmmsdk.cmmCfgSetMioProperty(Para.iPhysicalNo, (int)MotnDefines._TCmMioPropId.cmEZ_LOGIC  , (int)Para.eZphaPhase );
            Cmmsdk.cmmCfgSetMioProperty(Para.iPhysicalNo, (int)MotnDefines._TCmMioPropId.cmINP_LOGIC , (int)Para.eInposPhase);
            Cmmsdk.cmmCfgSetMioProperty(Para.iPhysicalNo, (int)MotnDefines._TCmMioPropId.cmORG_LOGIC , (int)Para.eHomePhase );
            Cmmsdk.cmmCfgSetMioProperty(Para.iPhysicalNo, (int)MotnDefines._TCmMioPropId.cmSVON_LOGIC, (int)Para.eServoPhase);

            //링카운터 (ex, 원으로 돌때 해당 포지션에서 0로 변경)
            //이 기능은 COMI-LX502 제품에서는 지원하지 않습니다. 대여받은게 하필 이거라 테스트 못해봄
            iUse = Para.dSetMaxPos == 0 ? 0 : 1;
            Cmmsdk.cmmCfgSetRingCntr(Para.iPhysicalNo, (int)MotnDefines._TCmCntr.cmCNT_COMM, iUse, Para.dSetMaxPos*m_dPulsePerUnit);
            Cmmsdk.cmmCfgSetRingCntr(Para.iPhysicalNo, (int)MotnDefines._TCmCntr.cmCNT_FEED, iUse, Para.dSetMaxPos*m_dPulsePerUnit);

            if (Para.bGantryEnable) SetLinkEnable();
            else                    SetLinkDisable();
            
        }


        //특수기능
        /// <summary>
        /// 하드웨어 트리거를 발생시킨다.
        /// </summary>
        /// <param name="_daPos">발생시킬 트리거 위치들</param>
        /// <param name="_dTrgTime">트리거 시그널의 시간us</param>
        /// <param name="_bActual">엔코더기준인지 커멘드 기준인지</param>
        /// <param name="_bOnLevel">트리거 레벨</param>
        public void SetTrgPos(double[] _daPos, double _dTrgTime, bool _bActual, bool _bOnLevel)    //Target Actual Position or Command Position.
        {
            //CMP 출력은 One-shot pulse 로 출력되는데, 출력되는 펄스의 폭을 조절할
            //수 있습니다. 설정 및 반환되는 PropVal 은 다음과 같습니다.
            //• 0 : 트리거 시점의 Command 펄스의 펄스폭과 동일한 펄스폭을 가짐
            //• 양수의 값 : 이 값에 1.5us 가 곱해진 값이 펄스폭이 됩니다. 즉, 이 값을
            //1 로 하면 1.5us, 2 로 하면 3us…와 같이 됩니다.
            int iTrgTime = (int)(_dTrgTime/1.5) ;
            int iRet = Cmmsdk.cmmCfgSetMioProperty(Para.iPhysicalNo, (int)MotnDefines._TCmMioPropId.cmCMP_PWIDTH, iTrgTime);

            int iCmpSrc = 0;
            if(_bActual) iCmpSrc = (int)MotnDefines._TCmCntr.cmCNT_COMM; // Command Position 0 
            else         iCmpSrc = (int)MotnDefines._TCmCntr.cmCNT_FEED; // Feedback Position 1
            
            //CmpMethod 1번으로 사용하는건 잘 모르겟음 일단 기본 베이스 맞춰서 1,2사용으로 함
            //0 (cmDISABLE) Disable comparator
            //1 (cmEQ_BIDIR) CmpData = CmpSrc_Value (regardless of counting direction) 카운팅 방향에 상관 없이
            //2 (cmEQ_PDIR) CmpData = CmpSrc_Value (while counting up)
            //3 (cmEQ_NDIR) CmpData = CmpSrc_Value (while counting down)
            //4 (cmLESS) CmpData > CmpSrc_Value
            //5 (cmGREATER) CmpData < CmpSrc_Value

            //(+)방향쪽으로 이동시에만 트리거펄스가 출력되고 (-)방향으로 이동할 때는 출력하지
            //않도록(cmEQ_PDIR) 합니다. 엣지 개념이 아닌듯해서 1번으로 사용
            int iCmpMethod = (int)MotnDefines._TCmCmpMethod.cmEQ_BIDIR;
            
            //if(_bActual) iCmpMethod = (int)MotnDefines._TCmCmpMethod.cmEQ_PDIR; // CmpData = CmpSrc_Value (while counting up)
            //else         iCmpMethod = (int)MotnDefines._TCmCmpMethod.cmEQ_NDIR; // CmpData = CmpSrc_Value (while counting down)

            //위치 비교기 조건 설정 및 확인 
            Cmmsdk.cmmCmpTrgSetConfig(Para.iPhysicalNo,iCmpSrc,iCmpMethod);

            //연속적인 위치데이터 등록
            Cmmsdk.cmmCmpTrgContRegTable(Para.iPhysicalNo,_daPos,_daPos.Length);

            //연속적인 위치 비교 기능 시작
            Cmmsdk.cmmCmpTrgContStart(Para.iPhysicalNo);

        }
        /// <summary>
        /// 하드웨어 트리거를 시작 위치 부터 끝위치 까지 일정 간격으로 트리거를 발생 시킨다.
        /// </summary>
        /// <param name="_dStt">시작 위치</param>
        /// <param name="_dEnd">종료 위치</param>
        /// <param name="_dDist">주기</param>
        /// <param name="_dTrgTime">트리거 시그널의 시간us</param>
        /// <param name="_bActual">엔코더기준인지 커맨드 기준인지</param>
        /// <param name="_bOnLevel">트리거 레벨</param>
        public void SetTrgBlock(double _dStt, double _dEnd, double _dPeriod, double _dTrgTime, bool _bActual, bool _bOnLevel)
        {
            //CMP 출력은 One-shot pulse 로 출력되는데, 출력되는 펄스의 폭을 조절할
            //수 있습니다. 설정 및 반환되는 PropVal 은 다음과 같습니다.
            //• 0 : 트리거 시점의 Command 펄스의 펄스폭과 동일한 펄스폭을 가짐
            //• 양수의 값 : 이 값에 1.5us 가 곱해진 값이 펄스폭이 됩니다. 즉, 이 값을
            //1 로 하면 1.5us, 2 로 하면 3us…와 같이 됩니다.
            int iTrgTime = (int)(_dTrgTime/1.5) ;
            int iRet = Cmmsdk.cmmCfgSetMioProperty(Para.iPhysicalNo, (int)MotnDefines._TCmMioPropId.cmCMP_PWIDTH, iTrgTime);

            //일정 간격의 비교위치 데이터 등록
            //연속적인 위치 비교 출력 기능을 사용하기 위해서 일정한 위치 간격을 가지는 연속적인 위치 데이터를 자동으로 생성하여
            //등록 하도록 합니다.
            //이 함수는 일정한 위치 간격으로 CMP 출력을 내보낼 때 cmmCmpTrgContRegTable()함수 대신에 사용할 수 있습니다. 
            
            int iCmpSrc = 0;
            if(_bActual) iCmpSrc = (int)MotnDefines._TCmCntr.cmCNT_COMM; // Command Position 0 
            else         iCmpSrc = (int)MotnDefines._TCmCntr.cmCNT_FEED; // Feedback Position 1
            
            //CmpMethod 1번으로 사용하는건 잘 모르겟음 일단 기본 베이스 맞춰서 1,2사용으로 함
            //0 (cmDISABLE) Disable comparator
            //1 (cmEQ_BIDIR) CmpData = CmpSrc_Value (regardless of counting direction) 카운팅 방향에 상관 없이
            //2 (cmEQ_PDIR) CmpData = CmpSrc_Value (while counting up)
            //3 (cmEQ_NDIR) CmpData = CmpSrc_Value (while counting down)
            //4 (cmLESS) CmpData > CmpSrc_Value
            //5 (cmGREATER) CmpData < CmpSrc_Value

            //(+)방향쪽으로 이동시에만 트리거펄스가 출력되고 (-)방향으로 이동할 때는 출력하지
            //않도록(cmEQ_PDIR) 합니다. 엣지 개념이 아닌듯해서 1번으로 사용
            int iCmpMethod = (int)MotnDefines._TCmCmpMethod.cmEQ_BIDIR;
            //if(_bActual) iCmpMethod = (int)MotnDefines._TCmCmpMethod.cmEQ_PDIR; // CmpData = CmpSrc_Value (while counting up)
            //else         iCmpMethod = (int)MotnDefines._TCmCmpMethod.cmEQ_NDIR; // CmpData = CmpSrc_Value (while counting down)

            //위치 비교기 조건 설정 및 확인 
            Cmmsdk.cmmCmpTrgSetConfig(Para.iPhysicalNo,iCmpSrc,iCmpMethod);

            //연속적인 위치데이터 등록
            //이거 갯수는 맞을런지 모르겟다...추후 검증 필요
            //NumData : 자동생성되는 총 데이터 수 라고 되어 있음
            //public static extern unsafe int cmmCmpTrgContBuildTable(
            //[MarshalAs(UnmanagedType.I4)] int Axis,
            //[MarshalAs(UnmanagedType.R8)] double StartData,
            //[MarshalAs(UnmanagedType.R8)] double Interval,
            //[MarshalAs(UnmanagedType.I4)] int NumData);
            double dNumData = (_dEnd-_dStt) / _dPeriod ;
            int    iNumData = (int)dNumData + 1;
            Cmmsdk.cmmCmpTrgContBuildTable(Para.iPhysicalNo,_dStt,_dPeriod,iNumData);

            //연속적인 위치 비교 기능 시작
            Cmmsdk.cmmCmpTrgContStart(Para.iPhysicalNo);
            
        }
        /// <summary>
        /// SetTrgPos로 세팅된 포지션 리셑
        /// </summary>
        public void ResetTrgPos()
        {
            //연속위치비교 기능 종료
            Cmmsdk.cmmCmpTrgContStop(Para.iPhysicalNo);
        }
        /// <summary>
        /// 테스트로 한번의 트리거를 출력.
        /// </summary>
        /// <param name="_bOnLevel">출력 레벨</param>
        /// <param name="_iTime">시간us</param>
        public void OneShotTrg(bool _bOnLevel, int _iTime)
        {
            //CMP 출력은 One-shot pulse 로 출력되는데, 출력되는 펄스의 폭을 조절할
            //수 있습니다. 설정 및 반환되는 PropVal 은 다음과 같습니다.
            //• 0 : 트리거 시점의 Command 펄스의 펄스폭과 동일한 펄스폭을 가짐
            //• 양수의 값 : 이 값에 1.5us 가 곱해진 값이 펄스폭이 됩니다. 즉, 이 값을
            //1 로 하면 1.5us, 2 로 하면 3us…와 같이 됩니다.
            int iTrgTime = (int)(_iTime/1.5) ;
            int iRet = Cmmsdk.cmmCfgSetMioProperty(Para.iPhysicalNo, (int)MotnDefines._TCmMioPropId.cmCMP_PWIDTH, iTrgTime);

            int iCmpSrc = (int)MotnDefines._TCmCntr.cmCNT_COMM; // Command Position 0 
            
            //CmpMethod 1번으로 사용하는건 잘 모르겟음 일단 기본 베이스 맞춰서 1,2사용으로 함
            //0 (cmDISABLE) Disable comparator
            //1 (cmEQ_BIDIR) CmpData = CmpSrc_Value (regardless of counting direction) 카운팅 방향에 상관 없이
            //2 (cmEQ_PDIR) CmpData = CmpSrc_Value (while counting up)
            //3 (cmEQ_NDIR) CmpData = CmpSrc_Value (while counting down)
            //4 (cmLESS) CmpData > CmpSrc_Value
            //5 (cmGREATER) CmpData < CmpSrc_Value

            //(+)방향쪽으로 이동시에만 트리거펄스가 출력되고 (-)방향으로 이동할 때는 출력하지
            //않도록(cmEQ_PDIR) 합니다. 엣지 개념이 아닌듯해서 1번으로 사용
            int iCmpMethod = (int)MotnDefines._TCmCmpMethod.cmEQ_BIDIR;
            
            //if(_bActual) iCmpMethod = (int)MotnDefines._TCmCmpMethod.cmEQ_PDIR; // CmpData = CmpSrc_Value (while counting up)
            //else         iCmpMethod = (int)MotnDefines._TCmCmpMethod.cmEQ_NDIR; // CmpData = CmpSrc_Value (while counting down)

            //위치 비교기 조건 설정 및 확인 
            Cmmsdk.cmmCmpTrgSetConfig (Para.iPhysicalNo,iCmpSrc,iCmpMethod);
            Cmmsdk.cmmCmpTrgSetOneData(Para.iPhysicalNo,MotionInfo.iCmdPos);
        }


        // 다축제어 라서 static이여야함.
        // 보간 구동을 위해 추가
        public void ContiSetAxisMap(int _iCoord, uint _uiAxisCnt, int[] _iaAxisNo)
        {

        }
        public void ContiSetAbsRelMode(int _iCoord, uint _uiAbsRelMode)
        {

        }
        public void ContiBeginNode(int _iCoord)
        {

        }
        public void ContiEndNode(int _iCoord)
        {

        }
        public void ContiStart(int _iCoord, uint _uiProfileset, int _iAngle)
        {

        }
        public int GetContCrntIdx(int _iCoord)
        {
            int iRet = 0;
            return iRet;
        }
        public void LineMove(int _iCoord, double[] _daEndPos, double _dVel, double _dAcc, double _dDec)
        {

        }

        public void CircleCenterMove(int _iCoord, int[] _iaAxisNo, double[] _daCenterPos, double[] _daEndPos, double _dVel, double _dAcc, double _dDec, uint _uiCWDir)
        {

        }
        public void CirclePointMove(int _iCoord, int[] _iaAxisNo, double[] _daMidPos, double[] _daEndPos, double _dVel, double _dAcc, double _dDec, int _iArcCircle)
        {

        }
        public void CircleRadiusMove(int _iCoord, int[] _iaAxisNo, double _dRadius, double[] _daEndPos, double _dVel, double _dAcc, double _dDec, uint _uiCWDir, uint _uiShortDistance)
        {

        }
        public void CircleAngleMove(int _iCoord, int[] _iaAxisNo, double[] _daCenterPos, double _dAngle, double _dVel, double _dAcc, double _dDec, uint _uiCWDir)
        {

        }

        /// <summary>
        /// 네트워크 타입들은 시그널을 받아 놓는다.
        /// </summary>
        public void Update()
        {
            int iMioState = 0;
            Cmmsdk.cmmStReadMioStatuses(Para.iPhysicalNo, ref iMioState);

            MotionStat.bAlarmSgnl  = ((iMioState >> (int)MotnDefines._TCmMioState.cmIOST_ALM) & 0x01) == 0x01;
            MotionStat.bPLimSnsr   = ((iMioState >> (int)MotnDefines._TCmMioState.cmIOST_ELP) & 0x01) == 0x01;
            MotionStat.bNLimSnsr   = ((iMioState >> (int)MotnDefines._TCmMioState.cmIOST_ELN) & 0x01) == 0x01;
            MotionStat.bHomeSnsr   = ((iMioState >> (int)MotnDefines._TCmMioState.cmIOST_ORG) & 0x01) == 0x01;
            MotionStat.bInPosSgnl  = ((iMioState >> (int)MotnDefines._TCmMioState.cmIOST_INP) & 0x01) == 0x01;
            MotionStat.bZphaseSgnl = ((iMioState >> (int)MotnDefines._TCmMioState.cmIOST_EZ ) & 0x01) == 0x01;

            Cmmsdk.cmmStGetCount(Para.iPhysicalNo, (int)MotnDefines._TCmCntr.cmCNT_COMM, ref MotionInfo.iCmdPos);
            Cmmsdk.cmmStGetCount(Para.iPhysicalNo, (int)MotnDefines._TCmCntr.cmCNT_FEED, ref MotionInfo.iActPos);

        }

        /// <summary>
        /// 해당모터축의 서브파라미터를 LoadSave함.
        /// </summary>
        /// <param name="_bLoad">true=로드 false=세이브</param>
        /// <returns>성공여부</returns>
        public bool LoadSave(bool _bLoad, string _sParaFolderPath, int _iMotrNo)
        {
            string sFilePath = _sParaFolderPath + "MotrComi" + _iMotrNo.ToString() + ".xml";
            //object oParaMotrSub = Para ;
            if (_bLoad)
            {
                if (!CXml.LoadXml(sFilePath, ref Para)) { return false; }
            }
            else
            {
                if (!CXml.SaveXml(sFilePath, ref Para)) { return false; }
            }



            return true;
        }

        /// <summary>
        /// Para Return
        /// </summary>
        /// <returns></returns>
        public object GetPara()
        {
            return Para;
        }

        /// <summary>
        /// 해당 모터의 입력 비트를 확인
        /// </summary>
        /// <param name="_iNo">사용할 비트 넘버</param>
        /// <returns>true=ON, false=OFF</returns>
        public bool GetX(int _iNo)
        {
            //if(_iNo < 0 || _iNo > 4) return false;
            //uint iOn = 0;
            //CAXM.AxmSignalReadInputBit(Para.iPhysicalNo,  _iNo , ref iOn);
            //if(iOn == 1) return true;
            //return false;

            //if (_iNo < 0 || _iNo > 4) return false;
            //bool bRet = ((MotionInfo.uInput >> _iNo) & 0x01) == 0x01;
            //return bRet;
            return true;
        }

        /// <summary>
        /// 해당 모터의 출력 비트를 확인
        /// </summary>
        /// <param name="_iNo">사용할 비트 넘버</param>
        /// <returns>true=ON, false=OFF</returns>
        public bool GetY(int _iNo)
        {
            //uint iOn = 0;
            //CAXM.AxmSignalReadOutputBit (Para.iPhysicalNo, _iNo , ref iOn);
            //if(iOn == 1) return true;
            //return false;

            //if (_iNo < 0 || _iNo > 4) return false;
            //bool bRet = ((MotionInfo.uOutput >> _iNo) & 0x01) == 0x01;
            //return bRet;
            return true;
        }

        /// <summary>
        /// 해당 모터의 출력 비트를 제어
        /// </summary>
        /// <param name="_iNo">사용할 비트 넘버</param>
        /// <param name="_bOn">true=ON, false=OFF</param>
        public void SetY(int _iNo, bool _bOn)
        {
 //           uint iOn = _bOn ? (uint)1 : (uint)0;
 //           CAXM.AxmSignalWriteOutputBit(Para.iPhysicalNo, _iNo, iOn);
             
        }

        //===============================================================================요 밑으로 필요없는 애들(동기구동)
        /// <summary>
        /// Master/Slave모터간 동기구동
        /// 이 함수는 밖에서 자유롭게 Link Enable 할수 있다.
        /// </summary>
        public bool SetLinkEnable(int _iSlvMotrNo=0)
        {
            
            //if (Para.bGantryEnable)
            //int iRet = Cmmsdk.cmmMsRegisterSlave(Para.iGantrySubAdd, 500000, (int)MotnDefines._TCmBool.cmFALSE);//맥스 스피드를 하필 여기서 넣냐....그냥 상수로 때려넣음
            //double.MaxValue 값 넣으면 슬레이브 커맨드가 막 왓다 갓다 하면서 병신짓함
            int iRet = Cmmsdk.cmmMsRegisterSlave(Para.iGantrySubAdd, int.MaxValue, (int)MotnDefines._TCmBool.cmFALSE);//맥스 스피드를 하필 여기서 넣냐....그냥 상수로 때려넣음

            if(iRet == 1) return true;
            return false;
        }

        /// <summary>
        /// 현재 동기구동 상태이면 return true;
        /// </summary>
        /// <returns></returns>
        public bool GetLinkMode()
        {
            int iSlaveState = 0;
            Cmmsdk.cmmMsCheckSlaveState(Para.iGantrySubAdd,ref iSlaveState);
            //-1 해당 축이 Slave 축으로 등록되어 있지만 모션 에러가 발생하였음을 의미합니다. 이
            //때는 Slave 축을 다시 등록할 필요는 없지만 에러 상황은 해제 시켜주어야 합니다.
            //0 해당 축이 Slave 축으로 등록되어 있지 않습니다.
            //1 해당 축이 Slave 축으로서 등록되어 있으며 정상 동작을 하고 있습니다.
            if(iSlaveState == 1) return true;
            return false;
        }

        /// <summary>
        /// 동기구동 해제
        /// </summary>
        public bool SetLinkDisable(int _iSlvMotrNo=0)
        {
            Cmmsdk.cmmMsUnregisterSlave(Para.iGantrySubAdd);//Axis 인자가 Slave로 설정되어 +1하여 갠트리 해제함.
            return true;
        }

        /// <summary>
        /// 상대위치, 절대위치 변경
        /// </summary>
        /// <param name="_iAxisNo"> 축 넘버 </param>
        /// <param name="_uiAbsRelMode">0 : 절대모드, 1:상대모드</param>
        public void SetAbsRelMode(uint _uiAbsRelMode)
        {
            
        }

        /// <summary>
        /// Master축이 +로 이동할때 Slave 축이 -로 이동하도록 하는 조그 기능
        /// 실제 동기구동은 아님
        /// </summary>
        /// <param name="iSlvMotrNo"> Slave 축 넘버 </param>
        /// <param name="_dVel"> 마스터축 속도 </param>
        /// <param name="_dAcc"> 마스터축 가속 </param>
        /// <param name="_dDec"> 마스터축 감속 </param>
        public void LinkJogP(int iSlvMotrNo, double _dVel, double _dAcc, double _dDec)
        {
            
        }

        /// <summary>
        /// Master축이 -로 이동할때 Slave 축이 +로 이동하도록 하는 조그 기능
        /// 실제 동기구동은 아님
        /// </summary>
        /// <param name="iSlvMotrNo"> Slave 축 넘버 </param>
        /// <param name="_dVel"> 마스터축 속도 </param>
        /// <param name="_dAcc"> 마스터축 가속 </param>
        /// <param name="_dDec"> 마스터축 감속 </param>
        public void LinkJogN(int iSlvMotrNo, double _dVel, double _dAcc, double _dDec)
        {

        }
    }
}
