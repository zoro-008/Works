﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using MotionInterface;
using COMMON;


namespace MotionAXL
{    
    public enum ENC_IN : uint 
    {
        Plus_Up_Down , 
        Plus_1Multi  , 
        Plus_2_Multi , 
        Plus_4_Multi , 
        Minas_Up_Down, 
        Minas_1Multi , 
        Minas_2_Multi, 
        Minas_4_Multi
    } 
    public enum PUL_OUT : uint
    {
        OneHighLowHigh = 0x0,// 0x0, 1펄스 방식, PULSE(Active High), 정방향(DIR=Low)  / 역방향(DIR=High)
        OneHighHighLow ,// 0x1, 1펄스 방식, PULSE(Active High), 정방향(DIR=High) / 역방향(DIR=Low)
        OneLowLowHigh  ,// 0x2, 1펄스 방식, PULSE(Active Low),  정방향(DIR=Low)  / 역방향(DIR=High)
        OneLowHighLow  ,// 0x3, 1펄스 방식, PULSE(Active Low),  정방향(DIR=High) / 역방향(DIR=Low)
        TwoCcwCwHigh   ,// 0x4, 2펄스 방식, PULSE(CCW:역방향),  DIR(CW:정방향),  Active High
        TwoCcwCwLow    ,// 0x5, 2펄스 방식, PULSE(CCW:역방향),  DIR(CW:정방향),  Active Low
        TwoCwCcwHigh   ,// 0x6, 2펄스 방식, PULSE(CW:정방향),   DIR(CCW:역방향), Active High
        TwoCwCcwLow     // 0x7, 2펄스 방식, PULSE(CW:정방향),   DIR(CCW:역방향), Active Low
    }
    public enum PHASE : uint
    {
        RowActive  = 0,
        HighActive = 1 
    }
    public enum HOME_SIGNAL : uint
    {
        PLimitSensor = 0 ,
        NLimitSensor = 1 ,
        HomeSensor   = 4 ,
        EncorderZ    = 5 

    }

    public enum Z_METHOD : uint
    {
        NotUse           = 0 ,
        HomeDirection    = 1 ,
        NotHomeDirection = 2 ,
    }

                // HClrTim : HomeClear Time : 원점 검색 Encoder 값 Set하기 위한 대기시간 
            // HmDir(홈 방향): DIR_CCW (0) -방향 , DIR_CW(1) +방향
            // HOffset - 원점검출후 이동거리.
            // uZphas: 1차 원점검색 완료 후 엔코더 Z상 검출 유무 설정  0: 사용안함 , 1: Hmdir과 반대 방향, 2: Hmdir과 같은 방향
            // HmSig : PosEndLimit(0) -> +Limit
            //         NegEndLimit(1) -> -Limit
            //         HomeSensor (4) -> 원점센서(범용 입력 0)

    [Serializable]
    public class CParaMotorAxl
    {   
        [CategoryAttribute("AxlPara" ), DisplayNameAttribute("Physical No"          )]public int         iPhysicalNo  {get; set;} //실제모터 물리 어드레스. 
        [CategoryAttribute("AxlPara" ), DisplayNameAttribute("S Curve Acc Percent"  )]public double      dSCurveAcPer {get; set;} // S-Curve구동 Acc Percent ,
        [CategoryAttribute("AxlPara" ), DisplayNameAttribute("S Curve Dcc Percent"  )]public double      dSCurveDcPer {get; set;} // S-Curve구동 Dec Percent , 
        [CategoryAttribute("AxlPara" ), DisplayNameAttribute("Use Inposition Signal")]public bool        bUseInpos    {get; set;} // 서보팩의 인포지션 시그널 이용 여부. 
        [CategoryAttribute("AxlPara" ), DisplayNameAttribute("Encoder Input"        )]public ENC_IN      eEncInput    {get; set;} // 엔코더 입력 방식 설정(체배설정) EXTERNAL_COUNTER_INPUT 
        [CategoryAttribute("AxlPara" ), DisplayNameAttribute("Pulse Output"         )]public PUL_OUT     ePulseOutput {get; set;} // 펄스 출력 방식 설정             PULSE_OUTPUT 
        [CategoryAttribute("AxlPara" ), DisplayNameAttribute("Plus Limit Phase"     )]public PHASE       ePLimPhase   {get; set;} // 정방향 리미트(+End limit)의 액티브레벨 설정 
        [CategoryAttribute("AxlPara" ), DisplayNameAttribute("Minas Limit Phase"    )]public PHASE       eNLimPhase   {get; set;} // 역방향 리미트(-End limit)의 액티브레벨 설정 
        [CategoryAttribute("AxlPara" ), DisplayNameAttribute("Alram Phase"          )]public PHASE       eAlarmPhase  {get; set;} // 알람(Alarm) 신호 액티브레벨 설정 
        [CategoryAttribute("AxlPara" ), DisplayNameAttribute("Inposition Phase"     )]public PHASE       eInposPhase  {get; set;} // 인포지션(Inposition) 신호 액티브레벨 설정 
        [CategoryAttribute("AxlPara" ), DisplayNameAttribute("Home Phase"           )]public PHASE       eHomePhase   {get; set;} // 홈 엑티브 레벨
        [CategoryAttribute("AxlPara" ), DisplayNameAttribute("Z Phase"              )]public PHASE       eZphaPhase   {get; set;} 
        [CategoryAttribute("AxlPara" ), DisplayNameAttribute("Servo Phase"          )]public PHASE       eServoPhase  {get; set;} 
        [CategoryAttribute("AxlPara" ), DisplayNameAttribute("Use 360 Count"        )]public bool        e360Count    {get; set;} 
                                                                                                                     
        [CategoryAttribute("HomePara"), DisplayNameAttribute("Home Direction is Neg")]public bool        bHomeNegDir  {get; set;}
        [CategoryAttribute("HomePara"), DisplayNameAttribute("Home Signal Selection")]public HOME_SIGNAL eHomeSignal  {get; set;} 
        [CategoryAttribute("HomePara"), DisplayNameAttribute("Home Z Mathod"        )]public Z_METHOD    eHomeZMethod {get; set;} 
        [CategoryAttribute("HomePara"), DisplayNameAttribute("Home Clear Delay Time")]public double      dHomeClrTime {get; set;}
        [CategoryAttribute("HomePara"), DisplayNameAttribute("Home Offset"          )]public double      dHomeOffset  {get; set;}
        
    } 

    public class CMotor:IMotor
    {
        private  CParaMotorAxl Para ;
        public CMotor() { }

        /// <summary>
        /// 초기화 함수.
        /// </summary>
        /// <returns>초기화 성공여부</returns>
        public bool Init()
        {
            Para = new CParaMotorAxl();

            //통합 보드 초기화 부분.
            if (CAXL.AxlIsOpened() == 0)
            {				// 통합 라이브러리가 사용 가능하지 (초기화가 되었는지) 확인
                if (CAXL.AxlOpenNoReset(7) != 0)//초기화 사용 하지 않는 오픈.
                {			// 통합 라이브러리 초기화
                    Log.ShowMessage("Motor", "AJIN AXL Lib Loading Error");
                    return false;
                }
            }
            uint uStatus = 0;
            uint uRet = CAXM.AxmInfoIsMotionModule(ref uStatus);
            if (uRet != 0)
            {
                Log.ShowMessage("Motor", "AJIN AXL Motion Module Loading Error");
                return false;
            }

            //public static extern uint AxmInfoIsMotionModule(ref uint upStatus);
            return true;
        }

        /// <summary>
        /// 초기화 함수.
        /// </summary>
        /// <returns>초기화 성공여부</returns>
        public bool Ready()
        {
            
            return true;
        }


        /// <summary>
        /// 종료 함수
        /// </summary>
        /// <returns>종료 성공여부</returns>
        public bool Close()
        {
            int iRet = CAXL.AxlClose();
            return iRet != 0 ;
        }
        /// <summary>
        /// 모터의 인포지션 확인 안한 정지여부.
        /// </summary>
        /// <returns>정지상태 true , 구동상태 false</returns>
        public bool GetStop()
        {
            uint   uiInMotion = 1;
	        CAXM.AxmStatusReadInMotion(Para.iPhysicalNo, ref uiInMotion );
	        bool   bRet = uiInMotion == 0 ;
	        return bRet;
        }

        /// <summary>
        /// 서보 온오프
        /// </summary>
        /// <param name="_bOn">온오프</param>
        public void SetServo(bool _bOn)
        {
            //const int iServoOnBit = 0 ;
            uint  uiOn = _bOn ? (uint)1 : (uint)0 ;
            
            //Stop the Motor.
            Stop();
            
            //Servo On/Off.
            CAXM.AxmSignalServoOn(Para.iPhysicalNo, uiOn);
            //CAXM.AxmSignalWriteOutputBit(Para.iPhysicalNo, iServoOnBit, uiOn);

            if(!_bOn)
            {
                SetHomeDone(false);
            }
        }
        /// <summary>
        /// 서보상태를 받아옴.
        /// </summary>
        /// <returns>온오프</returns>
        public bool GetServo()//나중에 서보 오프시에 홈시그널 깨지는지 확인해봐야함.
        {
            const int iServoOnBit = 0 ;
            uint  uiOn = 0 ;
            
            //CAXM.AxmSignalReadOutputBit(Para.iPhysicalNo,iServoOnBit,ref uiOn);
            CAXM.AxmSignalIsServoOn(Para.iPhysicalNo, ref uiOn);

            return uiOn != 0;
        }
        /// <summary>
        /// 리셑 시그널 온오프 제어
        /// </summary>
        /// <param name="_bOn">온오프</param>
        public void SetReset(bool _bOn)
        {
            const int iResetBit = 1 ;
            uint  uiOn = _bOn ? (uint)1 : (uint)0 ;
            //Servo On/Off.
            CAXM.AxmSignalWriteOutputBit(Para.iPhysicalNo, iResetBit, uiOn);
        }

        /// <summary>
        /// 펄스 커멘드 리턴.
        /// </summary>
        /// <returns>펄스 리턴</returns>
        public double GetCmdPos()
        {
            double dPos=0.0;
            uint uiRet;
            uiRet = CAXM.AxmStatusGetCmdPos(Para.iPhysicalNo, ref dPos);
            return dPos;
        }
        /// <summary>
        /// 펄스 엔코더 리턴.
        /// </summary>
        /// <returns>펄스 리턴</returns>
        public double GetEncPos()
        {
            double dPos=0.0;
            CAXM.AxmStatusGetActPos(Para.iPhysicalNo, ref dPos);
            return dPos;
        }
        /// <summary>
        /// Command Encoder Target포지션 세팅.
        /// </summary>
        /// <param name="_dPos">세팅할 펄스</param>
        public void SetPos(double _dPos)
        {
            CAXM.AxmStatusSetCmdPos(Para.iPhysicalNo , _dPos); //Commnad Position.
            CAXM.AxmStatusSetActPos(Para.iPhysicalNo , _dPos); //Actual Position.
        }

        //Signal...
        /// <summary>
        /// 홈센서 시그널.
        /// </summary>
        /// <returns>OnOff상태</returns>
        public bool GetHomeSnsr()
        {
            const int iBit = 7;
            uint uiSignal = 0;

            CAXM.AxmStatusReadMechanical(Para.iPhysicalNo, ref uiSignal);

            bool bRet = ((uiSignal >> iBit) & 0x01) == 0x01;

            return bRet;
        }

  
        /// <summary>
        /// -리밋 센서 시그널
        /// </summary>
        /// <returns>OnOff상태</returns>
        public bool GetNLimSnsr()
        {
            const int iBit = 1 ;
            uint  uiSignal = 0 ;
            
            CAXM.AxmStatusReadMechanical(Para.iPhysicalNo,ref uiSignal);

            bool bRet =((uiSignal >> iBit) & 0x01) == 0x01;

            return bRet;
        }
        /// <summary>
        /// +리밋 센서 시그널
        /// </summary>
        /// <returns>OnOff상태</returns>
        public bool GetPLimSnsr()
        {
            const int iBit = 0 ;
            uint  uiSignal = 0 ;
            
            CAXM.AxmStatusReadMechanical(Para.iPhysicalNo,ref uiSignal);

            bool bRet =((uiSignal >> iBit) & 0x01) == 0x01;

            return bRet;
        }
        /// <summary>
        /// 엔코더 Z상 시그널 상태
        /// </summary>
        /// <returns>OnOff상태</returns>
        public bool GetZphaseSgnl()
        {
            const int iBit = 1 ;
            uint  uiSignal = 0 ;
            
            CAXM.AxmSignalReadInput(Para.iPhysicalNo,ref uiSignal);

            bool bRet =((uiSignal >> iBit) & 0x01) == 0x01;

            return bRet;
        }
        /// <summary>
        /// 서보펙 인포지션 시그널 상태
        /// </summary>
        /// <returns>OnOff상태</returns>
        public bool GetInPosSgnl() //Servo Pack InPosition Signal.
        {
            //const int iBit = 5 ;
            uint  uiSignal = 0 ;

            
           
            //CAXM.AxmStatusReadMechanical(Para.iPhysicalNo,ref uiSignal);
            CAXM.AxmSignalReadInpos(Para.iPhysicalNo, ref uiSignal);


            return uiSignal == 1;
        }
        /// <summary>
        /// 서보펙 알람 시그널 상태
        /// </summary>
        /// <returns>OnOff상태</returns>
        public bool GetAlarmSgnl()
        {
            const int iBit = 4 ;
            uint  uiSignal = 0 ;
            
            CAXM.AxmStatusReadMechanical(Para.iPhysicalNo,ref uiSignal);

            bool bRet =((uiSignal >> iBit) & 0x01) == 0x01;

            return bRet;
        }

        /// <summary>
        /// 홈을 수행한다.
        /// </summary>
        /// <returns>수행 성공여부.</returns>
        public bool GoHome(double _dHomeVelFirst , double _dHomeVelLast , double _dHomeAccFirst, double _dMinPosPulse, double _dMaxPosPulse)
        {

            SetServo(true);

            // 해당 축의 원점검색을 수행하기 위해서는 반드시 원점 검색관련 파라메타들이 설정되어 있어야 됩니다. 
            // 만약 MotionPara설정 파일을 이용해 초기화가 정상적으로 수행됐다면 별도의 설정은 필요하지 않다. 
            // 원점검색 방법 설정에는 검색 진행방향, 원점으로 사용할 신호, 원점센서 Active Level, 엔코더 Z상 검출 여부 등을 설정 한다.
            // 주의사항 : 레벨을 잘못 설정시 -방향으로 설정해도  +방향으로 동작할수 있으며, 홈을 찾는데 있어 문제가 될수있다.
            // (자세한 내용은 AxmMotSaveParaAll 설명 부분 참조)
            // 홈레벨은 AxmSignalSetHomeLevel 사용한다.
            // HClrTim : HomeClear Time : 원점 검색 Encoder 값 Set하기 위한 대기시간 
            // HmDir(홈 방향): DIR_CCW (0) -방향 , DIR_CW(1) +방향
            // HOffset - 원점검출후 이동거리.
            // uZphas: 1차 원점검색 완료 후 엔코더 Z상 검출 유무 설정  0: 사용안함 , 1: Hmdir과 반대 방향, 2: Hmdir과 같은 방향
            // HmSig : PosEndLimit(0) -> +Limit
            //         NegEndLimit(1) -> -Limit
            //         HomeSensor (4) -> 원점센서(범용 입력 0)

            int iDir = Para.bHomeNegDir  ? 0 : 1 ;
            CAXM.AxmHomeSetMethod(Para.iPhysicalNo , iDir , (uint)Para.eHomeSignal , (uint)Para.eHomeZMethod , Para.dHomeClrTime , Para.dHomeOffset);
            CAXM.AxmHomeSetVel   (Para.iPhysicalNo,_dHomeVelFirst , _dHomeVelFirst , _dHomeVelLast , _dHomeVelLast , _dHomeAccFirst , _dHomeAccFirst);
            CAXM.AxmHomeSetStart (Para.iPhysicalNo);
            CAXM.AxmStatusSetPosType(Para.iPhysicalNo, Para.e360Count ? (uint)1 : (uint)0, _dMinPosPulse, _dMaxPosPulse);
            /*
            원점검색구동 원점검색구동 원점검색구동 원점검색구동 원점검색구동 원점검색구동
            하드웨어 초기화 , 파라미터 설정 , 서보온의 과정으로 모션구동에 모션구동에 작업이 준비되어 있다 하더라도 좌표계의 방 향만 설정되어 있을 뿐 좌표계의 좌표계의 기준이 되는 원점은 설정이 설정이 되어 있지 않은 상태이다 상태이다 . 기구부가 사용자가 원하는 일정한 위치로 이송되기 위해서는 정확한 원점이 보장되어야 한다 . 원점검색을 하는 방법에는 크게 신호검색구동함수를 이용하는 방법과 원점검색함수를 이용하는 이용하는 방법이 있다 . 신호검색을 신호검색을 이용하여 홈검색을 수행하기 위해서는 상당히 번거로운 잡업들이 이루어지게 이루어지게 되기 때문에 이러한 과정을 과정을 더 편하게 하기 위해 홈검색 함수를 제공한다 . 원점검색함수를 원점검색함수를 이용하기 위해서는 AxmHomeSetMethodAxmHomeSetMethodAxmHomeSetMethodAxmHomeSetMethodAxmHomeSetMethodAxmHomeSetMethodAxmHomeSetMethodAxmHomeSetMethoAxmHomeSetMethodAxmHomeSetMethodAxmHomeSetMethodAxmHomeSetMethodAxmHomeSetMethodAxmHomeSetMethod 함수와 AxmHomeSetVelAxmHomeSetVelAxmHomeSetVelAxmHomeSetVelAxmHomeSetVelAxmHomeSetVelAxmHomeSetVelAxmHomeSetVelAxmHomeSetVelAxmHomeSetelAxmHomeSetVelAxmHomeSetVelAxmHomeSetVel 함수를 이용하여 홈검색에 필요한 데이터들을 설정해주고 AxmHomeSetStartAxmHomeSetStartAxmHomeSetStartAxmHomeSetStartAxmHomeSetStartAxmHomeSetStartAxmHomeSetStartAxmHomeSetStartAxmHomeetStartAxmHomeSetStartAxmHomeSetStartAxmHomeSetStartAxmHomeSetStartAxmHomeSetStart 함수를 호출하여 홈검색 을 시 작하면 된다 .
            원점검색함수는 개별적인 쓰레드를 생성하여 작동하므로 , 만약 여러 축의 원점검색을 해야 하는 경우라면 , 각 각의 원점검색명령을 열거해서 열거해서 사용하면 된다 .
             원점검색 방법 및 속도 설정
            원점검색은 기구부에 따라 여러 가지 신호검색 순서가 발생할 발생할 수 있지만 , AXL , AXL, AXL 의 원점검색 원점검색 함수는 아래의 7단 계 원점검색 시퀀스로 이루어지며 이루어지며 , -방향으로 원점검색 실행할 실행할 경우 원점검색단계는 다음과 다음과 같다 . .
            다축구동Library User Manual Rev.2.0
            62 AJINEXTEK CO.,LTD.
            1단계 : -방향으로 이동하면서 이동하면서 홈센서의 Rising Edge Rising Edge Rising EdgeRising Edge Rising EdgeRising Edge신호를 검색하여 감속정지
            (홈센서가 이미 HIGH HIGH HIGH HIGH HIGH 상태라면 1단계를 생략 )
            2단계 : +방향으로 이동하면서 이동하면서 홈센서의 Falling Edge신호를 검색하여 감속정지
            3단계 : -방향으로 이동하면서 이동하면서 홈센서의 Rising Edge신호를 검색하여 급정지
            < Z < Z < Z < Z 상 검색을 사용하지 않은 경우 >
            4단계 : +방향으로 이동하면서 이동하면서 홈센서의 Falling EdgeFalling EdgeFalling EdgeFalling EdgeFalling Edge Falling EdgeFalling Edge Falling EdgeFalling Edge신호를 검색하여 급정지
            5단계 : -방향으로 이동하면서 이동하면서 홈센서의 Rising Edge Rising Edge Rising EdgeRising Edge Rising EdgeRising Edge신호를 검색하여 급정지
            <Z 상 검색을 사용한 경우 >
            4단계 : +방향으로 이동하면서 이동하면서 Z 상의 Falling EdgeFalling EdgeFalling EdgeFalling EdgeFalling Edge Falling EdgeFalling Edge Falling EdgeFalling Edge신호를 검색하여 급정지
            5단계 : -방향으로 이동하면서 이동하면서 Z 상의 Rising Edge Rising Edge Rising EdgeRising Edge Rising EdgeRising Edge신호를 검색하여 급정지
            6단계 : Offset : Offset: Offset: Offset: Offset: Offset: Offset: Offset값이 있다면 OffsetOffsetOffsetOffsetOffsetOffset값 만큼 추가 이동
            7단계 : : 사용자 설정 시간동안 시간동안 대기 후 위치값을 0으로 설정
             StepStepStepStep별 원점검출 원점검출 원점검출 원점검출 속도설정의 속도설정의 속도설정의 속도설정의 속도설정의 이해 (Z (Z (Z 상 검색을 사용할 사용할 사용할 경우 )
            Library User Manual Rev.2.0 다축구동
            AJINEXTEK CO.,LTD. 63
            ① VelFirst, AccFirst사용
            HmDir 방향으로 원점센서를 고속 검출하고 HAccF 감속도로 감속정지
            ② VelSecond, AccSecond사용
            HmDir 반대 방향으로 원점센서의 Down Edge를 검출하고 HAccF 감속도로 감속정지
            ③ VelThird사용
            HmDir 방향으로 원점센서의 Up Edge를 검출하고 급정지
            ④ VelLast 사용
            HmDir 반대 방향으로 Z상의 Down Edge를 검출하고 급정지
            ⑤ VelLast 사용
            HmDir 방향으로 Z상의 Up Edge를 검출하고 급정지
             Offset 값 설정의 이해
            기구적인 원점센서 검출이 완료된 후 특정 위치로 옮겨 원점을 설정해야 될 경우 HOffset에 값을 지정하면
            원점 센서 검출이 완료된 후 자동으로 해당 Offset만큼 이동 완료 후 원점을 재 설정하게 된다. 이때 주의할
            것은 (+)나 (-)Limit 을 벗어나지 않는 범위에서 Offset을 지정해야 된다. 그렇지 않을 경우 원점검색을 정상
            적으로 수행할 수 없습니다.
             */
            
            return true ;
        }

        /// <summary>
        /// 홈동작 완료 확인.
        /// </summary>
        /// <returns>홈동작 완료여부</returns>
        public bool GetHomeDone()
        {
            uint  iRet = 0 ;
            bool  bRet ;
            const uint HOME_SUCCESS = 0x01;       // 홈 완료
            CAXM.AxmHomeGetResult(Para.iPhysicalNo, ref iRet);
            bRet = (iRet == HOME_SUCCESS);
            
            return bRet ;
        }
        /// <summary>
        /// 강제로 홈돈 시그널 세팅.
        /// </summary>
        /// <param name="_bOn">세팅 값.</param>
        public void SetHomeDone(bool _bOn)
        {
            uint  iOn = _bOn ? (uint)1 : (uint)11 ;
            CAXM.AxmHomeSetResult(Para.iPhysicalNo, iOn);      
        }
        /// <summary>
        /// 홈시퀜스를 중단한다.
        /// </summary>
        public void StopHome()
        {
            Stop();
            
            SetHomeDone(false)  ;           
        }

        /// <summary>
        /// 홈이 진행중인지 확인.
        /// </summary>
        public bool GetHoming()
        {
            uint iRet = 0;
            bool bRet;
            const uint HOME_SEARCHING = 0x02 ;
            CAXM.AxmHomeGetResult(Para.iPhysicalNo, ref iRet);
            bRet = (iRet == HOME_SEARCHING);
            return bRet;
        }


        private void CycleHome()
        {
            /*
             * esOri  = 0, //홈센서
            esNeg  = 1, //-리밋
            esPos  = 2, //+리밋
            esZph  = 3, //Z상.
            switch(m_iHomeStep)
            {
                default : m_iHomeStep = 0 ; 
                          m_bReqHome = false ;
                          m_bHomeDone = false ;
                          return ;
                case 10 : 
            }*/

        }

        //Motion Functions.
        /// <summary>
        /// 모터 정지 명령.
        /// </summary>
        public void Stop()
        {
            CAXM.AxmMoveSStop(Para.iPhysicalNo) ;
        }
        /// <summary>
        /// 모터 감속 무시 긴급정지.
        /// </summary>
        public void EmgStop()
        {
            CAXM.AxmMoveEStop(Para.iPhysicalNo);
        }

        //내부 설정에 따라 Acc Dcc는 가감속율 즉 초당 증가pulse단위 
        /// <summary>
        /// +방향 조그 이동.
        /// </summary>
        /// <param name="_dVel">초당 구동 속도 펄스.</param>
        /// <param name="_dAcc">구동 가속율 펄스 </param>
        /// <param name="_dDec">감속율 펄스</param>
        public void JogP(double _dVel,double _dAcc,double _dDec)
        {
            CAXM.AxmMoveVel(Para.iPhysicalNo,  _dVel , _dAcc, _dDec);
        }
        /// <summary>
        /// -방향 조그 이동
        /// </summary>
        /// <param name="_dVel">초당 구동 속도 펄스</param>
        /// <param name="_dAcc">구동 가속율 펄스</param>
        /// <param name="_dDec">감속율 펄스</param>
        public void JogN(double _dVel,double _dAcc,double _dDec)
        {
            CAXM.AxmMoveVel(Para.iPhysicalNo,  -1*_dVel , _dAcc, _dDec);
        }
        /// <summary>
        /// 홈센서 기준 절대 위치로 이동.
        /// </summary>
        /// <param name="_dPos">이동할 위치 펄스단위</param>
        /// <param name="_dVel">이동할 속도 펄스단위</param>
        /// <param name="_dAcc">이동할 가속 펄스단위</param>
        /// <param name="_dDec">이동할 감속 펄스단위</param>
        /// <returns>성공여부</returns>
        public void GoAbs(double _dPos,double _dVel,double _dAcc,double _dDec)
        {
            CAXM.AxmMoveStartPos(Para.iPhysicalNo, _dPos , _dVel , _dAcc , _dDec );
        }

        /// <summary>
        /// 홈센서 기준 절대 위치로 다축 이동.
        /// </summary>
        /// <param name="_iPhysicalNos">이동할축들 0번이 마스터</param>
        /// <param name="_dPoses">이동할 위치 펄스단위</param>
        /// <param name="_dVel">이동할 속도 펄스단위</param>
        /// <param name="_dAcc">이동할 가속 펄스단위</param>
        /// <param name="_dDec">이동할 감속 펄스단위</param>
        /// <returns>성공여부</returns>
        public void GoMultiAbs(int [] _iPhysicalNos , double [] _dPoses , double _dVel , double _dAcc , double _dDec)
        {
            //구현 아직 안함.
        }


        /// <summary>
        /// 시그널 찾아서 감속 정지한다..
        /// </summary>
        /// <param name="_dVel">구동 속도 펄스단위</param>
        /// <param name="_dAcc">구동 가속 펄스단위</param>
        /// <param name="_eSignal">사용 시그널</param>
        /// <param name="_bEdgeUp">업엣지인지 다운엣지인지.</param>
        /// <returns></returns>
        public void FindEdgeStop(double _dVel,double _dAcc,EN_FIND_EDGE_SIGNAL _eSignal,bool _bEdgeUp , bool _bEmgStop=false)
        {
            
            int iEdge = _bEdgeUp ? 1 : 0 ;
            int iStopMethod = _bEmgStop ? 0 : 1 ; 
            CAXM.AxmMoveSignalSearch(Para.iPhysicalNo, _dVel, _dAcc, (int)_eSignal, iEdge , iStopMethod) ;
            /*
            public enum EN_FIND_EDGE_SIGNAL:uint
            {
                PosEndLimit                            = 0,            // +Elm(End limit) +방향 리미트 센서 신호
                NegEndLimit                            = 1,            // -Elm(End limit) -방향 리미트 센서 신호
                PosSloLimit                            = 2,            // +Slm(Slow Down limit) 신호 - 사용하지 않음
                NegSloLimit                            = 3,            // -Slm(Slow Down limit) 신호 - 사용하지 않음
                HomeSensor                             = 4,            // IN0(ORG)  원점 센서 신호
                EncodZPhase                            = 5,            // IN1(Z상)  Encoder Z상 신호
            };
            */
            
        }
        /// <summary>
        /// 현재 파라미터들을 아진 함수를 이용하여 세팅함.
        /// </summary>

        
        public void ApplyPara()
        {
            
            const int iStart_Stop_speed = 1  ; //모션프로파일 생성시 처음 치고 나가는 속도. 이보다 느리면 구동이 안됨.
            const int iMax_Speed_Pulse = 6000000; //최대 구동 펄스.
            CAXM.AxmMotSetMinVel(Para.iPhysicalNo, iStart_Stop_speed); //모션프로파일 생성시 처음 치고 나가는 속도. 이보다 느리면 구동이 안됨.
            CAXM.AxmMotSetMaxVel(Para.iPhysicalNo, iMax_Speed_Pulse); //최대 구동 속도를 설정

            uint iRet2;
            uint iMode;
            
            if(Para.dSCurveAcPer == 0 && Para.dSCurveDcPer == 0 )//커브 안쓸때.
            {
                iMode = (uint)AXT_MOTION_PROFILE_MODE.SYM_TRAPEZOIDE_MODE;//대칭      
            }
            else
            {
                iMode = (uint)AXT_MOTION_PROFILE_MODE.ASYM_TRAPEZOIDE_MODE;//비대칭
            }
            CAXM.AxmMotSetProfileMode(Para.iPhysicalNo, iMode);
            CAXM.AxmMotSetAccelJerk(Para.iPhysicalNo, Para.dSCurveAcPer);
            CAXM.AxmMotSetDecelJerk(Para.iPhysicalNo, Para.dSCurveDcPer);


            iRet2 = CAXM.AxmSignalSetZphaseLevel(Para.iPhysicalNo, (uint)Para.eZphaPhase); //Z상 접점.
            iRet2 = CAXM.AxmHomeSetSignalLevel(Para.iPhysicalNo, (uint)Para.eHomePhase); //홈센서 접점.
            CAXM.AxmSignalGetServoOnLevel(Para.iPhysicalNo, ref iMode);
            iRet2 = CAXM.AxmSignalSetServoOnLevel(Para.iPhysicalNo, (uint)Para.eServoPhase);
            CAXM.AxmSignalGetServoOnLevel(Para.iPhysicalNo, ref iMode);

            // 지정 축의 end limit sensor의 사용 유무 및 신호의 입력 레벨을 설정한다. 
            const uint iStopMode = 0;//EMERGENCY_STOP
            CAXM.AxmSignalSetLimit(Para.iPhysicalNo, iStopMode, (uint)Para.ePLimPhase, (uint)Para.eNLimPhase);

            CAXM.AxmSignalSetServoAlarm(Para.iPhysicalNo, (uint)Para.eAlarmPhase);//알람시에 비상정지 않씀 직접 세움.

            CAXM.AxmMotSetEncInputMethod(Para.iPhysicalNo, (uint)Para.eEncInput); //엔코더 입력 방식 설정 엔코더 방향 전환.
            CAXM.AxmMotSetPulseOutMethod(Para.iPhysicalNo, (uint)Para.ePulseOutput); //펄스 출력 방식 설정

            
            // uLevel : LOW(0), HIGH(1), UNUSED(2), USED(3)   
            CAXM.AxmSignalSetInpos(Para.iPhysicalNo, (uint)Para.eInposPhase); //인포지션(Inposition) 신호 액티브레벨 설정

            // AccelUnit : UNIT_SEC2   '0' - 가감속 단위를 unit/sec2 사용
            //             SEC         '1' - 가감속 단위를 sec 사용
            CAXM.AxmMotSetAccelUnit(Para.iPhysicalNo , 0);
        }

	    


        //특수기능
        /// <summary>
        /// 하드웨어 트리거를 발생시킨다.
        /// </summary>
        /// <param name="_dPos">발생시킬 트리거 위치들</param>
        /// <param name="_dTrgTime">트리거 시그널의 시간us</param>
        /// <param name="_bActual">엔코더기준인지 커멘드 기준인지</param>
        /// <param name="_bOnLevel">트리거 레벨</param>
        public void SetTrgPos(double[] _dPos,double _dTrgTime,bool _bActual,bool _bOnLevel)
        {
            //====================트리거 함수 ===================================================================================
            // 주의사항: 트리거 위치를 설정할경우 반드시 UNIT/PULSE의 맞추어서 설정한다.
            //           위치를 UNIT/PULSE 보다 작게할 경우 최소단위가 UNIT/PULSE로 맞추어지기때문에 그위치에 출력할수없다.
            
            // 지정 축에 트리거 기능의 사용 여부, 출력 레벨, 위치 비교기, 트리거 신호 지속 시간 및 트리거 출력 모드를 설정한다.
            // 트리거 기능 사용을 위해서는 먼저  AxmTriggerSetTimeLevel 를 사용하여 관련 기능 설정을 먼저 하여야 한다.
            // dTrigTime : 트리거 출력 시간, 1usec - 최대 50msec ( 1 - 50000 까지 설정)
            // upTriggerLevel  : 트리거 출력 레벨 유무 => LOW(0), HIGH(1)
            // uSelect         : 사용할 기준 위치      => COMMAND(0), ACTUAL(1)
            // uInterrupt      : 인터럽트 설정         => DISABLE(0), ENABLE(1)
            
            // 지정 축에 트리거 신호 지속 시간 및 트리거 출력 레벨, 트리거 출력방법을 설정한다.
            // public static extern uint AxmTriggerSetTimeLevel(int lAxisNo, double dTrigTime, uint uTriggerLevel, uint uSelect, uint uInterrupt);
            // 0축에 트리거 신호의 레벨과 지속시간을 설정한다.
            double dTrigTime = _dTrgTime; //1~50000(50ms)
            uint uTriggerLevel = _bOnLevel ? (uint)1 : (uint)0 ;
            uint uSelect = _bActual ? (uint)1 : (uint)0; // Encorder or Command Position 을 기준으로 트리거 발생
            uint uInterrupt = 0 ;
            uint dwRet = CAXM.AxmTriggerSetTimeLevel (Para.iPhysicalNo, dTrigTime, uTriggerLevel, uSelect, uInterrupt);
        
            //int nAxisNo, int nTrigNum, ref double dTrigPos);
            //배열 인자에 관해서 모르겠다. ;;
            //나중에 찾아보자.
            //CAXM.AxmTriggerOnlyAbs(mPara.iPhysicalNo,5, ref _dPos);
            //CAXM.AxmTriggerOnlyAbs(mPara.iPhysicalNo,_dPos.Length, _dPos);
        }

        /// <summary>
        /// SetTrgPos로 세팅된 포지션 리셑
        /// </summary>
        public void ResetTrgPos()
        {
            CAXM.AxmTriggerSetReset(Para.iPhysicalNo);
        }
        /// <summary>
        /// 테스트로 한번의 트리거를 출력.
        /// </summary>
        /// <param name="_bOnLevel">출력 레벨</param>
        /// <param name="_iTime">시간us</param>
        public void OneShotTrg(bool _bOnLevel,int _iTime)
        {
            double dTrigTime = _iTime; //1~50000(50ms)
            uint uTriggerLevel = _bOnLevel ? (uint)1 : (uint)0 ;
            uint uSelect = 1 ; //_bActual ? (uint)1 : (uint)0; // Encorder or Command Position 을 기준으로 트리거 발생
            uint uInterrupt = 0 ;
            uint dwRet = CAXM.AxmTriggerSetTimeLevel (Para.iPhysicalNo, dTrigTime, uTriggerLevel, uSelect, uInterrupt);
            CAXM.AxmTriggerOneShot(Para.iPhysicalNo);
        }

        /// <summary>
        /// 홈이 지원 안되는 보드 같은경우 돌려주고 Update 함수 내부에서 처리 해야 한다.
        /// </summary>
        public void Update()
        {

        }

        /// <summary>
        /// 해당모터축의 서브파라미터를 LoadSave함.
        /// </summary>
        /// <param name="_bLoad">true=로드 false=세이브</param>
        /// <returns>성공여부</returns>
        public bool LoadSave(bool _bLoad, string _sParaFolderPath, int _iMotrNo)
        {
            string sFilePath = _sParaFolderPath + "MotrAxl" + _iMotrNo.ToString() + ".xml";
            //object oParaMotrSub = Para ;
            if (_bLoad)
            {
                if (!CXml.LoadXml(sFilePath, ref Para)) { return false; }
            }
            else
            {
                if (!CXml.SaveXml(sFilePath, ref Para)) { return false; }
            }
            
            return true ;
        }

        public object GetPara()
        {
            return Para;
        }

        public void SetDoublePara(string _sParaName , double _dValue)
        {
            //여긴안씀..
        }

        public int GetIntStat(string _sStatName)
        {
            return 0;
        }


    }
}
