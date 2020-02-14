using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using MotionInterface;
using COMMON;
using Shared;

namespace MotionComizoa
{

    //public struct MOTION_STAT
    //{
    //    public bool bHomeSnsr;
    //    public bool bNLimSnsr;
    //    public bool bPLimSnsr;
    //    public bool bZphaseSgnl;
    //    public bool bAlarmSgnl;
    //    public bool bInPosSgnl;
    //}
    
    public enum ENC_MULTI : uint 
    {
        cmIMODE_AB1X  = 0, //1채배
        cmIMODE_AB2X  = 1, //2채배
        cmIMODE_AB4X  = 2, //4채배
        cmIMODE_CWCCW = 3, //CW/CCW(A펄스 - 카운트 증가, B펄스 - 카운트 감소)
        cmIMODE_STEP  = 4  //스텝모터(엔코더 피드백 없는 경우) 사용 시
    }
    public enum ENC_INPUT : uint
    {
        EA_EB = 0,
        EB_EA = 1
    }
    public enum CM_BOOL : uint
    {
        cmFALSE = 0, //변경안함
        cmTRUE  = 1  //변경함
    }
    public enum PULSE_MODE : uint
    {
        High_CCW_CW_1P = 0,
        Low_CCW_CW_1P  = 1,
        High_CW_CCW_1P = 2,
        Low_CW_CCW_1P  = 3,
        High_CW_CCW_2P = 4,
        Low_CW_CCW_2P  = 5,
        High_CCW_CW_2P = 6,
        Low_CCW_CW_2P  = 7
    }





    public enum PHASE_1 : uint
    {
        RowActive  = 0,
        HighActive = 1
    }
    public enum PHASE_2 : uint
    {
        RowActive  = 0,
        HighActive = 1,
        NotUse     = 2 
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
    //public struct MOTION_INFO
    //{
    //    public double dCmdPos;      // Command 위치[0x01]
    //    public double dActPos;      // Encoder 위치[0x02]
    //    public uint uMechSig;       // Mechanical Signal[0x04]
    //    public uint uDrvStat;       // Driver Status[0x08]
    //    public uint uInput;         // Universal Signal Input[0x10]
    //    public uint uOutput;        // Universal Signal Output[0x10]
    //    public uint uMask;          // 읽기 설정 Mask Ex) 0x1F, 모든정보 읽기
    //}
    public struct MOTION_STAT
    {
        public bool bHomeSnsr;
        public bool bNLimSnsr;
        public bool bPLimSnsr;
        public bool bZphaseSgnl;
        public bool bAlarmSgnl;
        public bool bInPosSgnl;
    }

    // HClrTim : HomeClear Time : 원점 검색 Encoder 값 Set하기 위한 대기시간 
    // HmDir(홈 방향): DIR_CCW (0) -방향 , DIR_CW(1) +방향
    // HOffset - 원점검출후 이동거리.
    // uZphas: 1차 원점검색 완료 후 엔코더 Z상 검출 유무 설정  0: 사용안함 , 1: Hmdir과 반대 방향, 2: Hmdir과 같은 방향
    // HmSig : PosEndLimit(0) -> +Limit
    //         NegEndLimit(1) -> -Limit
    //         HomeSensor (4) -> 원점센서(범용 입력 0)

    [Serializable]
    public class CParaMotorComi
    {   
        [CategoryAttribute("ComiPara"    ), DisplayNameAttribute("Physical No"          )]public int           iPhysicalNo    {get; set;} //실제모터 물리 어드레스. 
        //[CategoryAttribute("ComiPara"    ), DisplayNameAttribute("S Curve Acc Percent"  )]public double        dSCurveAcPer   {get; set;} // S-Curve구동 Acc Percent ,
        //[CategoryAttribute("ComiPara"    ), DisplayNameAttribute("S Curve Dcc Percent"  )]public double        dSCurveDcPer   {get; set;} // S-Curve구동 Dec Percent , 
        [CategoryAttribute("ComiPara"    ), DisplayNameAttribute("Use Inposition Signal")]public bool          bUseInpos      {get; set;} // 서보팩의 인포지션 시그널 이용 여부. 
        [CategoryAttribute("ComiPara"    ), DisplayNameAttribute("EncMulti"             )]public ENC_MULTI     eEncMulti      {get; set;} // 엔코더 입력 모드 설정
        [CategoryAttribute("ComiPara"    ), DisplayNameAttribute("EncInputMode"         )]public ENC_INPUT     eEncInputMode  {get; set;} // 엔코더 방향 설정
        [CategoryAttribute("ComiPara"    ), DisplayNameAttribute("Pulse Output"         )]public PULSE_MODE    ePulseOutput   {get; set;} // 펄스 출력 방식 설정             PULSE_OUTPUT 
        
        
        
        
        
        
        
        [CategoryAttribute("ComiPara"    ), DisplayNameAttribute("Plus Limit Phase"     )]public CM_BOOL       ePLimPhase     {get; set;} // 정방향 리미트(+End limit)의 액티브레벨 설정 
        [CategoryAttribute("ComiPara"    ), DisplayNameAttribute("Minas Limit Phase"    )]public CM_BOOL       eNLimPhase     {get; set;} // 역방향 리미트(-End limit)의 액티브레벨 설정 
        [CategoryAttribute("ComiPara"    ), DisplayNameAttribute("Alram Phase"          )]public CM_BOOL       eAlarmPhase    {get; set;} // 알람(Alarm) 신호 액티브레벨 설정 
        [CategoryAttribute("ComiPara"    ), DisplayNameAttribute("Inposition Phase"     )]public CM_BOOL       eInposPhase    {get; set;} // 인포지션(Inposition) 신호 액티브레벨 설정 
        [CategoryAttribute("ComiPara"    ), DisplayNameAttribute("Home Phase"           )]public CM_BOOL       eHomePhase     {get; set;} // 홈 엑티브 레벨
        [CategoryAttribute("ComiPara"    ), DisplayNameAttribute("Z Phase"              )]public CM_BOOL       eZphaPhase     {get; set;} 
        [CategoryAttribute("ComiPara"    ), DisplayNameAttribute("Servo Phase"          )]public CM_BOOL       eServoPhase    {get; set;} 
        
        [CategoryAttribute("ComiPara"    ), DisplayNameAttribute("Set Max Position"     )]public double        dSetMaxPos     {get; set;} //모터 최대 표시값 지정.링카운터
        //다이렉트케이블이 같이 엮겨 있을때 Machine에서 그냥 MT_SetY로 쓰기때문에 사용한하게 됨. 
        [CategoryAttribute("ComiPara"    ), DisplayNameAttribute("Break Off IO Address" )]public int           iBreakOffAdd   {get; set;} //브레이크 타입 브레이크 IO Address
        [CategoryAttribute("ComiPara"    ), DisplayNameAttribute("Use Break"            )]public bool          bUseBreak      {get; set;} //브레이크 타입 모터 설정.

        [CategoryAttribute("GantryPara" ), DisplayNameAttribute("Gantry Enable"        )]public bool           bGantryEnable  {get; set;} 
        [CategoryAttribute("GantryPara" ), DisplayNameAttribute("Gantry SubPhyAdd"     )]public int            iGantrySubAdd  {get; set;} 
        [CategoryAttribute("GantryPara" ), DisplayNameAttribute("Gantry Mathod"        )]public GANTRY_MATHOD  eGantryMathod  {get; set;} 
        [CategoryAttribute("GantryPara" ), DisplayNameAttribute("Gantry Offset"        )]public double         dGantryOffset  {get; set;} 
        [CategoryAttribute("GantryPara" ), DisplayNameAttribute("Gantry Offset Range"  )]public double         dGantryOfsRange{get; set;} 

        [CategoryAttribute("HomePara"   ), DisplayNameAttribute("Home Direction is Neg")]public bool           bHomeNegDir    {get; set;}
        [CategoryAttribute("HomePara"   ), DisplayNameAttribute("Home Signal Selection")]public HOME_SIGNAL    eHomeSignal    {get; set;} 
        [CategoryAttribute("HomePara"   ), DisplayNameAttribute("Home Z Mathod"        )]public Z_METHOD       eHomeZMethod   {get; set;} 
        [CategoryAttribute("HomePara"   ), DisplayNameAttribute("Home Clear Delay Time")]public double         dHomeClrTime   {get; set;}
        [CategoryAttribute("HomePara"   ), DisplayNameAttribute("Home Offset"          )]public double         dHomeOffset    {get; set;}
    } 

    


    public class CMotor : IMotor
    {
        private CParaMotorComi Para = new CParaMotorComi();
        //private MOTION_INFO MotionInfo;
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
                if (CMDLL.cmmGnDeviceLoad((int)MotnDefines._TCmBool.cmTRUE, ref m_iTotalAxis) != MotnDefines.cmERR_NONE)
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
                //if (bExtCmeFile) CMDLL.cmmGnInitFromFile(Path);
                //CMDLL.cmmAdvGetNumDefinedAxes(ref m_iTotalAxis);
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
            int iOn = _bOn ? 1 : 0;

            Stop();

            //다이렉트케이블이 같이 엮겨 있을때 Machine에서 그냥 MT_SetY로 쓰기때문에 사용한하게 됨. 
            if (Para.bUseBreak) SetY(Para.iBreakOffAdd, _bOn);

            CMDLL.cmmGnSetServoOn(Para.iPhysicalNo, iOn);

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
            if (Para.eServoPhase == CM_BOOL.cmTRUE)
            {
                //bRet = ((MotionInfo.uOutput >> 0) & 0x01) == 0x01;
                iRet = CMDLL.cmmGnGetServoOn(Para.iPhysicalNo, ref iOn);
            }
            else
            {
                //bRet = ((MotionInfo.uOutput >> 0) & 0x01) != 0x01;
                iRet = CMDLL.cmmGnGetServoOn(Para.iPhysicalNo, ref iOn);
            }
            return iRet == MotnDefines.cmERR_NONE;
        }
        /// <summary>
        /// 리셑 시그널 온오프 제어
        /// </summary>
        /// <param name="_bOn">온오프</param>
        public void SetReset(bool _bOn)
        {
            CMDLL.cmmGnDeviceReset();
        }

        /// <summary>
        /// 펄스 커멘드 리턴.
        /// </summary>
        /// <returns>펄스 리턴</returns>
        public double GetCmdPos()
        {
            int iCount = 0;
            CMDLL.cmmStGetCount(Para.iPhysicalNo, (int)MotnDefines._TCmCntr.cmCNT_COMM, ref iCount);
            return iCount;
        }
        /// <summary>
        /// 펄스 엔코더 리턴.
        /// </summary>
        /// <returns>펄스 리턴</returns>
        public double GetEncPos()
        {
            int iCount = 0;
            CMDLL.cmmStGetCount(Para.iPhysicalNo, (int)MotnDefines._TCmCntr.cmCNT_FEED, ref iCount);
            return iCount;
        }
        /// <summary>
        /// Command Encoder Target포지션 세팅.
        /// </summary>
        /// <param name="_dPos">세팅할 펄스</param>
        public void SetPos(double _dPos)
        {
            CMDLL.cmmStSetPosition(Para.iPhysicalNo, 1, _dPos); //Feedback position
            CMDLL.cmmStSetPosition(Para.iPhysicalNo, 0, _dPos); //Command position
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
            if (MotionStat.bAlarmSgnl) return false;
            SetServo(true);
            long lFuncRet;
            int iHomeMode = 1;

            //double lCripVel = dVel * ((IsServo) ? 0.03 : 0.03);
            //double lEscDist = (IsServo) ? m_dCoef : (m_dCoef / 5.0);

            //if (m_iMoveHomeSensor == 1)
            //{//Pos Limit
            //    return false;
            //}
            //else if (m_iMoveHomeSensor == 2)
            //{//Neg Limit
            //    iHomeMode = 6;
            //}
            int iDir = Para.bHomeNegDir ? 0 : 1;
            CMDLL.cmmHomeSetConfig(Para.iPhysicalNo, iHomeMode, 0, 1, 0.0);//EscDist 원점탈출거리 -> 무슨 의미인지 몰라서 일단 1 집어넣음. 진섭
            CMDLL.cmmHomeSetSuccess(Para.iPhysicalNo, 0); //HomeDone 강제 Setting. 0 == false, 1 == true. 진섭
            CMDLL.cmmHomeSetSpeedPattern(Para.iPhysicalNo, (int)MotnDefines._TCmSpeedMode.cmSMODE_T, _dHomeVelFirst, _dHomeAccFirst, _dHomeAccFirst, _dHomeVelLast);

            lFuncRet = CMDLL.cmmHomeMoveStart(Para.iPhysicalNo, iDir);

            return (lFuncRet == MotnDefines.cmERR_NONE);
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
            CMDLL.cmmHomeGetSuccess(Para.iPhysicalNo, ref iRet);
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
            CMDLL.cmmHomeSetSuccess(Para.iPhysicalNo, iOn); //HomeDone 강제 Setting. 0 == false, 1 == true. 진섭
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
            CMDLL.cmmHomeIsBusy(Para.iPhysicalNo, ref iBusy);
            return (iBusy == IsBusy);
        }


        //Motion Functions.
        /// <summary>
        /// 모터 정지 명령.
        /// </summary>
        public void Stop()
        {
            CMDLL.cmmSxStop(Para.iPhysicalNo, 0, 1);
        }
        /// <summary>
        /// 모터 감속 무시 긴급정지.
        /// </summary>
        public void EmgStop() //Stop Without Deceleration.
        {
            CMDLL.cmmSxStopEmg(Para.iPhysicalNo);
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

            //
            CMDLL.cmmCfgSetSpeedPattern(Para.iPhysicalNo, (int)MotnDefines._TCmSpeedMode.cmSMODE_T, _dVel, _dAcc, _dDec);
            CMDLL.cmmSxVMoveStart(Para.iPhysicalNo, iDir) ;
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
            CMDLL.cmmCfgSetSpeedPattern(Para.iPhysicalNo, (int)MotnDefines._TCmSpeedMode.cmSMODE_T, _dVel, _dAcc, _dDec);
            CMDLL.cmmSxVMoveStart(Para.iPhysicalNo, iDir) ;
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
            //int iSMode = Para.dSCurveAcPer == 0 || Para.dSCurveDcPer == 0 ? (int)MotnDefines._TCmSpeedMode.cmSMODE_T : (int)MotnDefines._TCmSpeedMode.cmSMODE_S;
            CMDLL.cmmCfgSetSpeedPattern(Para.iPhysicalNo, (int)MotnDefines._TCmSpeedMode.cmSMODE_T, _dVel, _dAcc, _dDec);
            CMDLL.cmmSxMoveToStart(Para.iPhysicalNo, _dPos);
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
            //CMDLL.cmmCfgSetSpeedPattern(Para.iPhysicalNo, iMode, 2000, 10000, 10000 );

            //Use Inpos
            int iDir = Para.bUseInpos ? (int)CM_BOOL.cmTRUE : (int)CM_BOOL.cmFALSE;
            CMDLL.cmmCfgSetMioProperty(Para.iPhysicalNo, (int)MotnDefines._TCmMioPropId.cmINP_EN, iDir);

            //EncInputMode 설정
            CMDLL.cmmCfgSetInMode(Para.iPhysicalNo, (int)Para.eEncMulti, (int)Para.eEncInputMode);
            //Pulse Output 설정
            CMDLL.cmmCfgSetOutMode(Para.iPhysicalNo, (int)Para.ePulseOutput);

            //하드웨어 리밋
            CMDLL.cmmCfgSetMioProperty(Para.iPhysicalNo, (int)MotnDefines._TCmMioPropId.cmEL_LOGIC, (int)Para.ePLimPhase);
            CMDLL.cmmCfgSetMioProperty(Para.iPhysicalNo, (int)MotnDefines._TCmMioPropId.cmEL_LOGIC, (int)Para.eNLimPhase);

            CMDLL.cmmCfgSetMioProperty(Para.iPhysicalNo, (int)MotnDefines._TCmMioPropId.cmALM_LOGIC , (int)Para.eAlarmPhase);
            CMDLL.cmmCfgSetMioProperty(Para.iPhysicalNo, (int)MotnDefines._TCmMioPropId.cmINP_LOGIC , (int)Para.eInposPhase);
            CMDLL.cmmCfgSetMioProperty(Para.iPhysicalNo, (int)MotnDefines._TCmMioPropId.cmORG_LOGIC , (int)Para.eHomePhase );
            CMDLL.cmmCfgSetMioProperty(Para.iPhysicalNo, (int)MotnDefines._TCmMioPropId.cmEZ_LOGIC  , (int)Para.eZphaPhase );
            CMDLL.cmmCfgSetMioProperty(Para.iPhysicalNo, (int)MotnDefines._TCmMioPropId.cmSVON_LOGIC, (int)Para.eServoPhase);

            //모터 최대 표시값. 링카운터
            int iR_CntrMode = Para.dSetMaxPos == 0 ? (int)MotnDefines._TCmBool.cmFALSE : (int)CM_BOOL.cmTRUE;
            CMDLL.cmmCfgSetRingCntr(Para.iPhysicalNo, (int)MotnDefines._TCmCntr.cmCNT_COMM, iR_CntrMode, Para.dSetMaxPos);



            SetGantryEnable();
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

        }
        /// <summary>
        /// SetTrgPos로 세팅된 포지션 리셑
        /// </summary>
        public void ResetTrgPos()
        {

        }
        /// <summary>
        /// 테스트로 한번의 트리거를 출력.
        /// </summary>
        /// <param name="_bOnLevel">출력 레벨</param>
        /// <param name="_iTime">시간us</param>
        public void OneShotTrg(bool _bOnLevel, int _iTime)
        {

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
            CMDLL.cmmStReadMioStatuses(Para.iPhysicalNo, ref iMioState);

            MotionStat.bInPosSgnl  = ((iMioState >> (int)MotnDefines._TCmMioState.cmIOST_INP) & 0x01) == 0x01;
            MotionStat.bHomeSnsr   = ((iMioState >> (int)MotnDefines._TCmMioState.cmIOST_ORG) & 0x01) == 0x01;
            MotionStat.bNLimSnsr   = ((iMioState >> (int)MotnDefines._TCmMioState.cmIOST_ELN) & 0x01) == 0x01;
            MotionStat.bPLimSnsr   = ((iMioState >> (int)MotnDefines._TCmMioState.cmIOST_ELP) & 0x01) == 0x01;
            MotionStat.bZphaseSgnl = ((iMioState >> (int)MotnDefines._TCmMioState.cmIOST_EZ ) & 0x01) == 0x01;
            MotionStat.bAlarmSgnl  = ((iMioState >> (int)MotnDefines._TCmMioState.cmIOST_ALM) & 0x01) == 0x01;

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

        //2축 제어 겐트리.
        public void SetGantryEnable()
        {
            int iSlaveState = 0;
            CMDLL.cmmMsCheckSlaveState(Para.iGantrySubAdd, ref iSlaveState);
            if(iSlaveState != 1)
            {
                SetGantryDisable();
            }
            if (Para.bGantryEnable)
            {
                CMDLL.cmmMsRegisterSlave(Para.iGantrySubAdd, 500000, (int)MotnDefines._TCmBool.cmFALSE);//맥스 스피드를 하필 여기서 넣냐....그냥 상수로 때려넣음
            }

        }
        public void SetGantryDisable()
        {
            CMDLL.cmmMsUnregisterSlave(Para.iPhysicalNo + 1);//Axis 인자가 Slave로 설정되어 +1하여 갠트리 해제함.
        }

        //===============================================================================요 밑으로 필요없는 애들(동기구동)
        /// <summary>
        /// Master/Slave모터간 동기구동
        /// 이 함수는 밖에서 자유롭게 Link Enable 할수 있다.
        /// </summary>
        public bool SetLinkEnable(int _iSlvMotrNo)
        {
            return true;
        }

        /// <summary>
        /// 현재 동기구동 상태이면 return true;
        /// </summary>
        /// <returns></returns>
        public bool GetLinkMode()
        {
            return true;
        }

        /// <summary>
        /// 동기구동 해제
        /// </summary>
        public bool SetLinkDisable(int _iSlvMotrNo)
        {
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
