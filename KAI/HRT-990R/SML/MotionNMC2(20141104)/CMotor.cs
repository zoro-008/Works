using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using COMMON;
using MotionInterface;
using Paix_MotionControler;

namespace MotionNMC2
{
    public enum HOME_MODE : uint
    {
        P_LimitSensor = 0 ,
        N_LimitSensor     ,
        P_HomeSensor      , 
        N_HomeSensor      ,
        P_LimitSensor_Z   ,
        N_LimitSensor_Z   ,
        N_Z               ,
        P_Z               
    }

    public enum PHASE_MODE : uint
    {
        NormalClose = 0 ,
        NormalOpen  = 1 
    }

    public enum ENC_MULTI : uint
    {
        Multi_4 = 0 ,
        Multi_2 = 1 ,
        Multi_1 = 2
    }

    public enum ENC_INPUT : uint
    {
        EA_EB = 0,
        EB_EA = 1,
        UP_DN = 2,
        DN_UP = 3
    }

    public enum PULSE_MODE : uint
    {
        Low_CW_CCW_2P  = 0 ,
        Low_CCW_CW_2P  = 1 , 
        High_CW_CCW_2P = 2 ,
        High_CCW_CW_2P = 3 ,
        Low_CW_CCW_1P  = 4 ,
        Low_CCW_CCW_1P = 5 ,
        High_CW_CCW_1P = 6 ,
        High_CCW_CW_1P = 7  
    }

    
    [Serializable]
    public class CParaMotorNmc2
    {   
        [CategoryAttribute("Nmc2Para" ), DisplayNameAttribute("Physical No"          )]public int         iPhysicalNo      {get; set;} //실제모터 물리 어드레스. 
        [CategoryAttribute("Nmc2Para" ), DisplayNameAttribute("HomeLogic"            )]public PHASE_MODE  eHomeLogic       {get; set;} // S-Curve구동 Acc Percent ,
        [CategoryAttribute("Nmc2Para" ), DisplayNameAttribute("MinusLimitLogic"      )]public PHASE_MODE  eMinusLimitLogic {get; set;} // S-Curve구동 Acc Percent ,
        [CategoryAttribute("Nmc2Para" ), DisplayNameAttribute("PlusLimitLogic"       )]public PHASE_MODE  ePlusLimitLogic  {get; set;} // S-Curve구동 Acc Percent ,
        [CategoryAttribute("Nmc2Para" ), DisplayNameAttribute("AlarmLogic"           )]public PHASE_MODE  eAlarmLogic      {get; set;} // S-Curve구동 Acc Percent ,
        [CategoryAttribute("Nmc2Para" ), DisplayNameAttribute("ZLogic"               )]public PHASE_MODE  eZLogic          {get; set;} // S-Curve구동 Acc Percent ,
        [CategoryAttribute("Nmc2Para" ), DisplayNameAttribute("EncMulti"             )]public ENC_MULTI   eEncMulti        {get; set;} // S-Curve구동 Acc Percent ,
        [CategoryAttribute("Nmc2Para" ), DisplayNameAttribute("EncInputMode"         )]public ENC_INPUT   eEncInputMode    {get; set;} // S-Curve구동 Acc Percent ,
        [CategoryAttribute("Nmc2Para" ), DisplayNameAttribute("PulseMode"            )]public PULSE_MODE  ePulseLogic      {get; set;} // S-Curve구동 Acc Percent ,       
        [CategoryAttribute("Nmc2Para" ), DisplayNameAttribute("ServoLogic"           )]public PHASE_MODE  eServoLogic      {get; set;} // S-Curve구동 Acc Percent ,       
                                                                                                                     
        [CategoryAttribute("HomePara" ), DisplayNameAttribute("Home Mode"            )]public HOME_MODE   eHomeMode        {get; set;}
        [CategoryAttribute("HomePara" ), DisplayNameAttribute("Home Offset"          )]public double      dHomeOffset      {get; set;}
        
        
    } 


    public class CMotor : IMotor
    {
        private CParaMotorNmc2 Para ;
        private double m_dPulsePerUnit;
        public CMotor() { }

        static private NMC2.NMCAXESEXPR      NmcAxesExpr;
        static private NMC2.NMCAXESMOTIONOUT NmcAxesMotionOut;
        static private NMC2.NMCHOMEFLAG      NmcHomeFlag ;
        

        static private int   m_iMaxMotor;
        static private short m_nDevId;
        static private bool  m_bInit = false;
        static private bool  m_bNeededReopen = false ;

        /// <summary>
        /// 초기화 함수.
        /// </summary>
        /// <returns>초기화 성공여부</returns>
        public bool Init()
        {
            Para = new CParaMotorNmc2();
            if (m_bInit == false)
            {
                m_bInit = true;
                //통합 보드 초기화 부분.
                if (!MotionNMC2.CModule.OpenDevice(out m_nDevId)) return false;
                

                //Motor 개수 정보 가져오기 ========================
                short nRet;
                short nMotionType, nIOType, nExtIo, nMDio;
                nRet = NMC2.nmc_GetDeviceInfo(m_nDevId, out nMotionType, out nIOType, out nExtIo, out nMDio);
                m_iMaxMotor = nMotionType ;
            }

            return true;
        }
        /// <summary>
        /// 종료 함수
        /// </summary>
        /// <returns>종료 성공여부</returns>
        public bool Close()
        {
            MotionNMC2.CModule.CloseDevice();
            return true;
        }
        /// <summary>
        /// 모터의 인포지션 확인 안한 정지여부.
        /// </summary>
        /// <returns>정지상태 true , 구동상태 false</returns>
        public bool GetStop()
        {
            bool  bRet;
          
            bRet = NmcAxesExpr.nBusy[Para.iPhysicalNo] == 0;

            return bRet;
            
        }

        /// <summary>
        /// 서보 온오프
        /// </summary>
        /// <param name="_bOn">온오프</param>
        public void SetServo( bool _bOn)
        {
            short nRet;
            short nSignal = _bOn ? (short)0 : (short)1; //이지서보 다이렉트 케이블일 경우 접점이 반대라서 우선 뒤집어 놓는다.

            nRet = NMC2.nmc_SetServoOn(m_nDevId, (short)Para.iPhysicalNo, nSignal);


        }
        /// <summary>
        /// 서보상태를 받아옴.
        /// </summary>
        /// <returns>온오프</returns>
        public bool GetServo()
        {
            bool bRet;

            bRet = NmcAxesMotionOut.nServoOn[Para.iPhysicalNo] == 0;

            return bRet;
        }
        /// <summary>
        /// 리셑 시그널 온오프 제어
        /// </summary>
        /// <param name="_bOn">온오프</param>
        public void SetReset( bool _bOn)
        {
            short nRet;
            short nSignal = _bOn ? (short)1 : (short)0;

           

            nRet = NMC2.nmc_SetAlarmResetOn(m_nDevId, (short)Para.iPhysicalNo, nSignal);

            //이상하게 이머진시 오래 유지 되고 난후에 이머전시 풀르면 A접점들이 안살아나면서 
            //병신이 되서 프로그램 껏따 켜야되는데...
            //밑에 테스트 결과 안됌.
            //전기 문제 혹은 파익스 문제 인듯.
            //if (!_bOn)
            //{
            //    bool bRet = CModule.ReOpenDevice();
            //}

        }

        /// <summary>
        /// 펄스 커멘드 리턴.
        /// </summary>
        /// <returns>펄스 리턴</returns>
        public double GetCmdPos()
        {
            return NmcAxesExpr.dCmd[Para.iPhysicalNo];
        }
        /// <summary>
        /// 펄스 엔코더 리턴.
        /// </summary>
        /// <returns>펄스 리턴</returns>
        public double GetEncPos()
        {
            return NmcAxesExpr.dEnc[Para.iPhysicalNo];
        }
        /// <summary>
        /// Command Encoder Target포지션 세팅.
        /// </summary>
        /// <param name="_dPos">세팅할 펄스</param>
        public void SetPos( double _dPos)
        {
            short nRet;
            nRet = NMC2.nmc_SetCmdPos(m_nDevId, (short)Para.iPhysicalNo, _dPos);
            nRet = NMC2.nmc_SetEncPos(m_nDevId, (short)Para.iPhysicalNo, _dPos);
        }

        //Signal...
        /// <summary>
        /// 홈센서 시그널.
        /// </summary>
        /// <returns>OnOff상태</returns>
        public bool GetHomeSnsr()
        {
            bool bRet;

            bRet = NmcAxesExpr.nNear[Para.iPhysicalNo] == 1;

            return bRet;
        }


        /// <summary>
        /// -리밋 센서 시그널
        /// </summary>
        /// <returns>OnOff상태</returns>
        public bool GetNLimSnsr()
        {
            bool bRet;

            bRet = NmcAxesExpr.nMLimit[Para.iPhysicalNo] == 1;
            return bRet;
        }
        /// <summary>
        /// +리밋 센서 시그널
        /// </summary>
        /// <returns>OnOff상태</returns>
        public bool GetPLimSnsr()
        {
            bool bRet;
            bRet = NmcAxesExpr.nPLimit[Para.iPhysicalNo] == 1;
            return bRet;
        }
        /// <summary>
        /// 엔코더 Z상 시그널 상태
        /// </summary>
        /// <returns>OnOff상태</returns>
        public bool GetZphaseSgnl()
        {
            bool bRet;
            bRet = NmcAxesExpr.nEncZ[Para.iPhysicalNo] == 1;
            return bRet;
        }
        /// <summary>
        /// 서보펙 인포지션 시그널 상태
        /// </summary>
        /// <returns>OnOff상태</returns>
        public bool GetInPosSgnl() //Servo Pack InPosition Signal.
        {
            bool bRet;
            bRet = NmcAxesExpr.nInpo[Para.iPhysicalNo] == 1;
            return bRet;
        }
        /// <summary>
        /// 서보펙 알람 시그널 상태
        /// </summary>
        /// <returns>OnOff상태</returns>
        public bool GetAlarmSgnl()
        {
            bool bRet;
            bRet = NmcAxesExpr.nAlarm[Para.iPhysicalNo] == 1;
            return bRet;
        }

        /// <summary>
        /// 홈을 수행한다.
        /// </summary>
        /// <returns>수행 성공여부.</returns>
        public bool GoHome(double _dHomeVelFirst, double _dHomeVelLast, double _dHomeAccFirst)
        {
            
            SetServo(true);

            short nRet;
            bool  bRet;

            nRet = NMC2.nmc_SetSpeed    (m_nDevId, (short)Para.iPhysicalNo, 200, _dHomeAccFirst, _dHomeAccFirst, _dHomeVelFirst);
            nRet = NMC2.nmc_SetAccSpeed (m_nDevId, (short)Para.iPhysicalNo, _dHomeAccFirst);
            nRet = NMC2.nmc_SetDecSpeed (m_nDevId, (short)Para.iPhysicalNo, _dHomeAccFirst);
            nRet = NMC2.nmc_SetHomeSpeed(m_nDevId, (short)Para.iPhysicalNo, _dHomeVelFirst, _dHomeVelFirst, _dHomeVelLast);


            // 1. 0번 축의 원점 이동 모드 : +Near
            // 2. 원점 검색후(3차이동 까지 완료) CW회전으로 Z상 검출
            // 3. 지령/엔코더 위치 0으로 초기화
            // 4. Offset 300위치로 이동
            // 5. Offset이동후 지령/엔코더 위치 0으로 초기화
            //nRet = nmc_HomeMove(11, 0, 0x82 | 3, 0xF, 300);


            const short nHomeEndMode = NMC2.NMC_END_CMD_CLEAR_A_OFFSET |
                                       NMC2.NMC_END_ENC_CLEAR_A_OFFSET |
                                       NMC2.NMC_END_CMD_CLEAR_B_OFFSET |
                                       NMC2.NMC_END_ENC_CLEAR_B_OFFSET;

            //메뉴얼과 샘플이 달라서 샘플 방식으로 함...eHomeMethode 여기다 0x80을 덮어 씌워야 된다고 
            //메뉴얼에 나와 있었는데 잘 모르겠음...
            nRet = NMC2.nmc_HomeMove(m_nDevId, (short)Para.iPhysicalNo, (short)Para.eHomeMode, nHomeEndMode, Para.dHomeOffset*m_dPulsePerUnit, 0);
            bRet = nRet == 0;


            return bRet;
        }

        /// <summary>
        /// 홈동작 완료 확인.
        /// </summary>
        /// <returns>홈동작 완료여부</returns>
        public bool GetHomeDone()
        {
            bool  bRet;
            bRet = NmcHomeFlag.nSrchFlag[Para.iPhysicalNo] == 0;

            return bRet;
        }
        /// <summary>
        /// 강제로 홈돈 시그널 세팅.
        /// </summary>
        /// <param name="_bOn">세팅 값.</param>
        public void SetHomeDone( bool _bOn)
        {
            //파익스에는 홈던 체킹을 강제로 할 수 있는 함수가 없다.
            //구현 하기 귀찮으니 패스.
           
        }
        /// <summary>
        /// 홈시퀜스를 중단한다.
        /// </summary>
        public void StopHome()
        {
            Stop();

        }

        public bool GetHoming()
        {
            bool bRet;
            bRet = NmcHomeFlag.nSrchFlag[Para.iPhysicalNo] == 1;

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
            short nRet;
                        
            nRet = NMC2.nmc_DecStop(m_nDevId, (short)Para.iPhysicalNo);
        }
        /// <summary>
        /// 모터 감속 무시 긴급정지.
        /// </summary>
        public void EmgStop()
        {
            short nRet;
                        
            nRet = NMC2.nmc_SuddenStop(m_nDevId, (short)Para.iPhysicalNo);
        }

        //내부 설정에 따라 Acc Dcc는 가감속율 즉 초당 증가pulse단위 
        /// <summary>
        /// +방향 조그 이동.
        /// </summary>
        /// <param name="_dVel">초당 구동 속도 펄스.</param>
        /// <param name="_dAcc">구동 가속율 펄스 </param>
        /// <param name="_dDec">감속율 펄스</param>
        public void JogP( double _dVel, double _dAcc, double _dDec)
        {
            short nRet;
            nRet = NMC2.nmc_SetSpeed(m_nDevId, (short)Para.iPhysicalNo, 200, _dAcc, _dDec, _dVel);
            nRet = NMC2.nmc_JogMove(m_nDevId, (short)Para.iPhysicalNo, 0);
        }
        /// <summary>
        /// -방향 조그 이동
        /// </summary>
        /// <param name="_dVel">초당 구동 속도 펄스</param>
        /// <param name="_dAcc">구동 가속율 펄스</param>
        /// <param name="_dDec">감속율 펄스</param>
        public void JogN( double _dVel, double _dAcc, double _dDec)
        {
            short nRet;
            nRet = NMC2.nmc_SetSpeed(m_nDevId, (short)Para.iPhysicalNo, 200, _dAcc, _dDec, _dVel);
            nRet = NMC2.nmc_JogMove(m_nDevId, (short)Para.iPhysicalNo, 1);
        }

        /// <summary>
        /// OverrideVel을 하기전에 구동속도중에 가장 높은 놈을 세팅을 한다.
        /// </summary>
        /// <param name="_dMaxVel">가장높은 속도값</param>
        /// <returns>성공여부</returns>
        public bool SetOverrideMaxVel(double _dMaxVel)
        {
            return false ;
        }
        
        /// <summary>
        /// 구동중 속도를 오버라이딩한다.
        /// 정속구간에서만 먹는 함수 이고. SetOverrideMaxVel 구동 속도중 가장 빠른 속도를 세팅 하고 수행해야함.
        /// </summary>
        /// <param name="_dVel">오버라이딩 할 속도</param>
        /// <returns>성공여부</returns>
        public bool OverrideVel(double _dVel)
        {
            return false ;
        }

        /// <summary>
        /// 홈센서 기준 절대 위치로 이동.
        /// </summary>
        /// <param name="_dPos">이동할 위치 펄스단위</param>
        /// <param name="_dVel">이동할 속도 펄스단위</param>
        /// <param name="_dAcc">이동할 가속 펄스단위</param>
        /// <param name="_dDec">이동할 감속 펄스단위</param>
        /// <returns>성공여부</returns>
        public void GoAbs( double _dPos, double _dVel, double _dAcc, double _dDec)
        {
            short nRet;
            nRet = NMC2.nmc_SetSpeed(m_nDevId, (short)Para.iPhysicalNo, 200, _dAcc, _dDec, _dVel);
            nRet = NMC2.nmc_AbsMove(m_nDevId, (short)Para.iPhysicalNo, _dPos);
        }

        /// <summary>
        /// 상대 위치로 이동.
        /// </summary>
        /// <param name="_dPos">이동할 위치 펄스단위</param>
        /// <param name="_dVel">이동할 속도 펄스단위</param>
        /// <param name="_dAcc">이동할 가속 펄스단위</param>
        /// <param name="_dDec">이동할 감속 펄스단위</param>
        /// <returns>성공여부</returns>
        public void GoRel( double _dPos, double _dVel, double _dAcc, double _dDec)
        {
            //short nRet;
            //nRet = NMC2.nmc_SetSpeed(m_nDevId, (short)Para.iPhysicalNo, 200, _dAcc, _dDec, _dVel);
            //nRet = NMC2.nmc_AbsMove(m_nDevId, (short)Para.iPhysicalNo, _dPos);
        }

        /// <summary>
        /// 홈센서 기준 절대 위치로 다축 이동.
        /// </summary>
        /// <param name="Para.iPhysicalNos">이동할축들 0번이 마스터</param>
        /// <param name="_dPoses">이동할 위치 펄스단위</param>
        /// <param name="_dVel">이동할 속도 펄스단위</param>
        /// <param name="_dAcc">이동할 가속 펄스단위</param>
        /// <param name="_dDec">이동할 감속 펄스단위</param>
        /// <returns>성공여부</returns>
        /// 일단 2축만 구현한다.
        /// 축번호는 오름차순으로 배열 해야함.
        public void GoMultiAbs(int [] _iPhysicalNos , double [] _dPoses , double _dVel , double _dAcc , double _dDec)
        {

            if (_iPhysicalNos.Count() > 3 || _dPoses.Count() > 3) return;
            if (_iPhysicalNos.Count() < 2 || _dPoses.Count() < 2) return;
            if (_iPhysicalNos.Count()     != _dPoses.Count()    ) return;


            short nRet;
            nRet = NMC2.nmc_SetSpeed(m_nDevId, (short)_iPhysicalNos[0], 200, _dAcc, _dDec, _dVel);
            if(_iPhysicalNos.Count() == 2) nRet = NMC2.nmc_Interpolation2Axis(m_nDevId, (short)_iPhysicalNos[0], (double)_dPoses[0], (short)_iPhysicalNos[1], (double)_dPoses[1],1);
            if(_iPhysicalNos.Count() == 3) nRet = NMC2.nmc_Interpolation3Axis(m_nDevId, (short)_iPhysicalNos[0], (double)_dPoses[0], (short)_iPhysicalNos[1], (double)_dPoses[1], (short)_iPhysicalNos[2], (double)_dPoses[2],1);
        }


        /// <summary>
        /// 시그널 찾아서 감속 정지한다..
        /// </summary>
        /// <param name="_dVel">구동 속도 펄스단위</param>
        /// <param name="_dAcc">구동 가속 펄스단위</param>
        /// <param name="_eSignal">사용 시그널</param>
        /// <param name="_bEdgeUp">업엣지인지 다운엣지인지.</param>
        /// <returns></returns>
        public void FindEdgeStop( double _dVel, double _dAcc, EN_FIND_EDGE_SIGNAL _eSignal, bool _bEdgeUp, bool _bEmgStop = false)
        {
            //사용안함.
        }
        /// <summary>
        /// 현재 파라미터들을 아진 함수를 이용하여 세팅함.
        /// </summary>
        public void ApplyPara(double _dPulsePerUnit)
        {
            m_dPulsePerUnit = _dPulsePerUnit;
            short nRet;
            //이머전시 로직은 한모듈에서 그룹별로 나눠놨는데
            //4축 이하는 그룹이 무조건 1개 이다.
            //왜 이렇게 만들었는지 이해가 안됨...
            nRet = NMC2.nmc_SetEmgLogic (m_nDevId, 0, 0);
            nRet = NMC2.nmc_SetUnitPerPulse(m_nDevId, (short)Para.iPhysicalNo, 1); //1펄스당 1펄스로 세팅.

            nRet = NMC2.nmc_SetNearLogic      (m_nDevId,(short)Para.iPhysicalNo, (short)Para.eHomeLogic      );
            nRet = NMC2.nmc_SetMinusLimitLogic(m_nDevId,(short)Para.iPhysicalNo, (short)Para.eMinusLimitLogic);
            nRet = NMC2.nmc_SetPlusLimitLogic (m_nDevId,(short)Para.iPhysicalNo, (short)Para.ePlusLimitLogic );
            nRet = NMC2.nmc_SetAlarmLogic     (m_nDevId,(short)Para.iPhysicalNo, (short)Para.eAlarmLogic     );
            nRet = NMC2.nmc_SetEncoderZLogic  (m_nDevId,(short)Para.iPhysicalNo, (short)Para.eZLogic         );
            
            nRet = NMC2.nmc_SetEncoderCount   (m_nDevId,(short)Para.iPhysicalNo, (short)Para.eEncMulti       );
            nRet = NMC2.nmc_SetEncoderDir     (m_nDevId,(short)Para.iPhysicalNo, (short)Para.eEncInputMode   ); //진입점이 없다고 뻑남... 메뉴얼 샘플 과 DLL버전이 다른듯.
            
            nRet = NMC2.nmc_SetPulseLogic     (m_nDevId,(short)Para.iPhysicalNo, (short)Para.ePulseLogic     );
            nRet = NMC2.nmc_SetSReadyLogic    (m_nDevId,(short)Para.iPhysicalNo, (short)Para.eServoLogic     );

            //nRet = NMC2
            
        }

        //특수기능
        /// <summary>
        /// 하드웨어 트리거를 발생시킨다.
        /// </summary>
        /// <param name="_dPos">발생시킬 트리거 위치들</param>
        /// <param name="_dTrgTime">트리거 시그널의 시간us</param>
        /// <param name="_bActual">엔코더기준인지 커멘드 기준인지</param>
        /// <param name="_bOnLevel">트리거 레벨</param>
        public void SetTrgPos( double[] _dPos, double _dTrgTime, bool _bActual, bool _bOnLevel)
        {
            
        }
        public void SetTrgBlock(double _dStt, double _dEnd, double _dPeriod, double _dTrgTime, bool _bActual, bool _bOnLevel)
        {
        }
        /// <param name="_dPos">발생시킬 트리거 위치들</param>
        /// <param name="_bMethod">false - 주기 , true - 절대위치</param>
        /// <param name="_dTrgTime">트리거 시그널의 시간us</param>
        /// <param name="_bActual">엔코더기준인지 커멘드 기준인지</param>
        /// <param name="_bOnLevel">트리거 레벨</param>
        public void SetTrgAbs(double _dPos, bool _bMethod, double _dTrgTime, bool _bActual, bool _bOnLevel)
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
        public void OneShotTrg( bool _bOnLevel, int _iTime)
        {

        }

                // 보간 구동을 위해 추가
        public void ContiSetAxisMap   (int _iCoord, uint _uiAxisCnt, int [] _iaAxisNo)
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
             return 0 ;
        }
        public void LineMove (int _iCoord, double []_daEndPos, double   _dVel , double   _dAcc    , double _dDec)
        {
        }

        public void CircleCenterMove  (int _iCoord, int []_iaAxisNo, double []_daCenterPos , double []_daEndPos, double _dVel, double _dAcc , double _dDec, uint _uiCWDir  )
        {
        }
        public void CirclePointMove   (int _iCoord, int []_iaAxisNo, double []_daMidPos    , double []_daEndPos, double _dVel, double _dAcc , double _dDec, int _iArcCircle)
        {
        }
        public void CircleRadiusMove  (int _iCoord, int []_iaAxisNo, double   _dRadius     , double []_daEndPos, double _dVel, double _dAcc , double _dDec, uint _uiCWDir  , uint _uiShortDistance)
        {
        }
        public void CircleAngleMove   (int _iCoord, int []_iaAxisNo, double []_daCenterPos , double   _dAngle  , double _dVel, double _dAcc , double _dDec, uint _uiCWDir  )
        {
        }


        /// <summary>
        /// 홈이 지원 안되는 보드 같은경우 돌려주고 Update 함수 내부에서 처리 해야 한다.
        /// 파익스는 네트워크 타입이라 너무 느려서 업데이트에서 한번만 스캔하여 담아두고 
        /// </summary>
        public void Update()
        {
            if(Para.iPhysicalNo != 0)return ;
            short nRet;
            nRet = NMC2.nmc_GetAxesExpress(m_nDevId,out NmcAxesExpr);
            nRet = NMC2.nmc_GetAxesMotionOut(m_nDevId , out NmcAxesMotionOut);
            nRet = NMC2.nmc_GetHomeStatus(m_nDevId , out NmcHomeFlag);

            if (nRet != 0)
            {
                if(!m_bNeededReopen)Log.ShowMessage("PAIX", "Needed Reopen 192.168.0." + m_nDevId.ToString());
                m_bNeededReopen = true ;
            }
            if (m_bNeededReopen)
            {                
                if(CModule.ReOpenDevice()) m_bNeededReopen = false ;
            }
        }

        /// <summary>
        /// 해당모터축의 서브파라미터를 LoadSave함.
        /// </summary>
        /// <param name="_bLoad">true=로드 false=세이브</param>
        /// <returns>성공여부</returns>
        public bool LoadSave(bool _bLoad, string _sParaFolderPath, int _iMotrNo)
        {
            string sFilePath = _sParaFolderPath + "MotrNmc2" + _iMotrNo.ToString() + ".xml";
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

        public object GetPara()
        {
            return Para;
        }

        public bool GetX(int _iNo)
        {
            bool bRet = false;
            return bRet;
        }

        public bool GetY(int _iNo)
        {
            bool bRet = false;
            return bRet;
        }

        public void SetY(int _iNo, bool _bOn)
        {

        }

    }
}
