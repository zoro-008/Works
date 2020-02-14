using System;
using System.ComponentModel;
using System.Windows.Forms;

using MotionInterface;
using COMMON;
using System.Collections.Generic;

namespace MotionEzPlusR
{    
    public enum HOME_METHODE : uint
    {
        //메뉴얼에 나와있는데로 인데 잘 안됌.
        //Home          = 0 ,//origin
        //Home_Z        = 1 ,//Z origin
        //Limit         = 2 ,//Limit origin
        //Limit_Z       = 3 ,//torque origin
        //CurrentPos    = 4 ,//torque z origin
        //Z             = 5 ,//Set origin
        //Torque        = 6 , 
        //Torque_Z      = 7   

        Home          = 0 ,
        Home_Z        = 1 ,// 이것 이상함 메뉴얼상 상식상 홈센서 찾고 +방향으로 Z상을 찾아야 하는데 -방향으로 z상을 찾음.파스텍 문의 결과 맞다고 함-_-;
        Limit         = 2 ,
        Torque        = 3 ,//torque origin
        Torgue_Z      = 4 ,//torque z origin  ===이상하게 토크로홈잡고 첫번째Z상 못찾고 두번째 Z상을 찾음.
        SetHome       = 5  //Set origin


    }

    [Serializable]
    public class CParaMotorEz
    {   
        [CategoryAttribute("EzPara"   ), DisplayNameAttribute("Port No"     )]public uint         iPortNo        {get; set;} //귀속 포트
        [CategoryAttribute("EzPara"   ), DisplayNameAttribute("Motor ID"    )]public uint         iMotorID       {get; set;} //모터에서 설정하는 ID
        [CategoryAttribute("EzPara"   ), DisplayNameAttribute("Direction"   )]public bool         bDirection     {get; set;} //모터방향.

        [CategoryAttribute("HomePara" ), DisplayNameAttribute("Home Methode")]public HOME_METHODE eHomeMethode   {get; set;} //홈방법.
        [CategoryAttribute("HomePara" ), DisplayNameAttribute("Home Offset" )]public double       dHomeOffset    {get; set;}
    } 

    public struct MOTION_STAT
    {
        public bool bHomeSnsr  ;
        public bool bNLimSnsr  ;
        public bool bPLimSnsr  ;
        public bool bZphaseSgnl;
        public bool bAlarmSgnl ;
        public bool bInPosSgnl ;

        public bool bRun       ;
        public bool bServo     ;
        public bool bHomeDone  ; //홈완료상태.
        public bool bHomming   ; //홈동작중.

        public int  iCmdPos    ; //실시간목적위치
        public int  iEncPos    ; //엔코다현재위치.
        public int  iSpeed     ; //속도.


        public bool bJogP    ; //+방향조그중.
        public bool bJogN    ; //-방향조그중.
        public int  iTrgPos  ; //현재 움직이려는 타겟위치           

        //홈관련세팅
        public double dHomeVelFirst ; 
        public double dHomeVelLast  ; 
        public double dHomeAccFirst ;
        public double dPulsePerUnit ;
    }

    public class CMotor:IMotor
    {
        //
        static List<uint> OpenPorts = new List<uint>();

        private  bool          m_bConnected = false ;
        private  CParaMotorEz  Para      = new CParaMotorEz() ;
        private  MOTION_STAT   Stat ;
        double   m_dPulsePerUnit ;
        public   CMotor() {}
        
        /// <summary>
        /// 초기화 함수.
        /// </summary>
        /// <returns>초기화 성공여부</returns>
        bool bDisplayedErr = false ;
        public bool Init()
        {
            int iRet = 0 ;
            //포트 혹시 안열려 있으면 열고 열렸으면 리스트에 넣어둠.
            if(Para.iPortNo!=0 && !OpenPorts.Contains(Para.iPortNo)) //포트아이디가 0인경우 아직 세팅 안한상태.
            {

                //여기서 뻑나면 dll위치와 dll이름 64 32비트 여부 확인
                //iRet = EziMOTIONPlusRLib.FAS_OpenPort((byte)Para.iPortNo, 115200) ;

                iRet = EziMOTIONPlusRLib.FAS_Connect((byte)Para.iPortNo, 115200) ;
                if (iRet == 0)
                {
                    if(!bDisplayedErr) 
                    {
                        bDisplayedErr = true ;
                        Log.ShowMessageModal("EzServoPlusR" , Para.iPortNo.ToString() + "Port Open Failed!");
                    }
                    return false;
                }
                OpenPorts.Add(Para.iPortNo);
            }



            if (EziMOTIONPlusRLib.FAS_IsSlaveExist((byte)Para.iPortNo, (byte)Para.iMotorID) == 0)
            {
                return false ;
            }

            m_bConnected = true ;

            return true;
        }
        /// <summary>
        /// 종료 함수
        /// </summary>
        /// <returns>종료 성공여부</returns>
        public bool Close()
        {
            m_bConnected = false ;

            if(Para.iPortNo!=0 && OpenPorts.Contains(Para.iPortNo)) //포트아이디가 0인경우 아직 세팅 안한상태.
            {
                EziMOTIONPlusRLib.FAS_Close((byte)Para.iPortNo);
                OpenPorts.Remove(Para.iPortNo);
            }

            return false ;
        }
        /// <summary>
        /// 모터의 인포지션 확인 안한 정지여부.
        /// </summary>
        /// <returns>정지상태 true , 구동상태 false</returns>
        public bool GetStop()
        {
           return !Stat.bRun ;
        }

        /// <summary>
        /// 서보 온오프
        /// </summary>
        /// <param name="_bOn">온오프</param>
        public void SetServo(bool _bOn)
        {
            int iRet = EziMOTIONPlusRLib.FAS_ServoEnable((byte)Para.iPortNo, (byte)Para.iMotorID, _bOn?1:0);            
        }
        /// <summary>
        /// 서보상태를 받아옴.
        /// </summary>
        /// <returns>온오프</returns>
        public bool GetServo()//나중에 서보 오프시에 홈시그널 깨지는지 확인해봐야함.
        {
            return Stat.bServo ;
        }
        /// <summary>
        /// 리셑 시그널 온오프 제어
        /// </summary>
        /// <param name="_bOn">온오프</param>
        public void SetReset(bool _bOn)
        {   
            int iRet = 0 ;
            if(_bOn) iRet = EziMOTIONPlusRLib.FAS_ServoAlarmReset((byte)Para.iPortNo, (byte)Para.iMotorID);
        }

        /// <summary>
        /// 펄스 커멘드 리턴.
        /// </summary>
        /// <returns>펄스 리턴</returns>
        public double GetCmdPos()
        {
            return Stat.iCmdPos ; 
        }
        /// <summary>
        /// 펄스 엔코더 리턴.
        /// </summary>
        /// <returns>펄스 리턴</returns>
        public double GetEncPos()
        {
            return Stat.iEncPos ;
        }
        /// <summary>
        /// Command Encoder Target포지션 세팅.
        /// </summary>
        /// <param name="_dPos">세팅할 펄스</param>
        public void SetPos(double _dPos)
        {
            int iCmdPos = (int)_dPos ;
            int iRet = 0 ;
            iRet = EziMOTIONPlusRLib.FAS_SetCommandPos((byte)Para.iPortNo, (byte)Para.iMotorID , iCmdPos);
            iRet = EziMOTIONPlusRLib.FAS_SetActualPos ((byte)Para.iPortNo, (byte)Para.iMotorID , iCmdPos);
        }

        //Signal...
        /// <summary>
        /// 홈센서 시그널.
        /// </summary>
        /// <returns>OnOff상태</returns>
        public bool GetHomeSnsr()
        {
            return Stat.bHomeSnsr ;
        }

  
        /// <summary>
        /// -리밋 센서 시그널
        /// </summary>
        /// <returns>OnOff상태</returns>
        public bool GetNLimSnsr()
        {
            return Stat.bNLimSnsr ;
        }
        /// <summary>
        /// +리밋 센서 시그널
        /// </summary>
        /// <returns>OnOff상태</returns>
        public bool GetPLimSnsr()
        {
            return Stat.bPLimSnsr ;
        }
        /// <summary>
        /// 엔코더 Z상 시그널 상태
        /// </summary>
        /// <returns>OnOff상태</returns>
        public bool GetZphaseSgnl()
        {
            return Stat.bZphaseSgnl ;
        }
        /// <summary>
        /// 서보펙 인포지션 시그널 상태
        /// </summary>
        /// <returns>OnOff상태</returns>
        public bool GetInPosSgnl() //Servo Pack InPosition Signal.
        {
            return Stat.bInPosSgnl ;
        }
        /// <summary>
        /// 서보펙 알람 시그널 상태
        /// </summary>
        /// <returns>OnOff상태</returns>
        public bool GetAlarmSgnl()
        {
            return Stat.bAlarmSgnl ;
        }

        /// <summary>
        /// 홈을 수행한다.
        /// </summary>
        /// <returns>수행 성공여부.</returns>
        public bool GoHome(double _dHomeVelFirst, double _dHomeVelLast, double _dHomeAccFirst)
        {
            int iRet = 0 ;
            
            iRet = EziMOTIONPlusRLib.FAS_SetParameter((byte)Para.iPortNo, (byte)Para.iMotorID ,17 , (int)_dHomeVelFirst);//홈속도
            iRet = EziMOTIONPlusRLib.FAS_SetParameter((byte)Para.iPortNo, (byte)Para.iMotorID ,18 , (int)_dHomeVelLast );//홈 엣지스캔속도
            iRet = EziMOTIONPlusRLib.FAS_SetParameter((byte)Para.iPortNo, (byte)Para.iMotorID ,19 , (int)(1000*_dHomeVelFirst / _dHomeAccFirst));//홈 가감속 시간.
            
            
            int iVal =  (int)Para.eHomeMethode ;
            iRet = EziMOTIONPlusRLib.FAS_SetParameter((byte)Para.iPortNo, (byte)Para.iMotorID ,20 , iVal               );//홈 잡는 방법. ?? 좀 이상하다 ;;
            iRet = EziMOTIONPlusRLib.FAS_SetParameter((byte)Para.iPortNo, (byte)Para.iMotorID ,21 , (int)1             );//-방향.
            iRet = EziMOTIONPlusRLib.FAS_SetParameter((byte)Para.iPortNo, (byte)Para.iMotorID ,22 , (int)(Para.dHomeOffset * m_dPulsePerUnit));//홈잡고 오프셑
            iRet = EziMOTIONPlusRLib.FAS_SetParameter((byte)Para.iPortNo, (byte)Para.iMotorID ,23 , (int)0             );//홈잡고 포지션 세팅값.
            
            
            iRet = EziMOTIONPlusRLib.FAS_MoveOriginSingleAxis((byte)Para.iPortNo, (byte)Para.iMotorID);

            
            return iRet == EziMOTIONPlusRLib.FMM_OK ;
        }

        /// <summary>
        /// 홈동작 완료 확인.
        /// </summary>
        /// <returns>홈동작 완료여부</returns>
        public bool GetHomeDone()
        {

            return Stat.bHomeDone ;
        }
        /// <summary>
        /// 강제로 홈돈 시그널 세팅.
        /// </summary>
        /// <param name="_bOn">세팅 값.</param>
        public void SetHomeDone(bool _bOn)
        {
            //기능이 없음.
        }
        /// <summary>
        /// 홈시퀜스를 중단한다.
        /// </summary>
        public void StopHome()
        {
            Stop();
        }

        /// <summary>
        /// 홈이 진행중인지 확인.
        /// </summary>
        public bool GetHoming()
        {
            return Stat.bHomming ;
        }


        private void CycleHome()
        {

        }

        //Motion Functions.
        /// <summary>
        /// 모터 정지 명령.
        /// </summary>
        public void Stop()
        {
            
            int iRet = 0 ;
            iRet = EziMOTIONPlusRLib.FAS_MoveStop((byte)Para.iPortNo, (byte)Para.iMotorID);
            
        }
        /// <summary>
        /// 모터 감속 무시 긴급정지.
        /// </summary>
        public void EmgStop()
        {
            int iRet = 0 ;
            iRet = EziMOTIONPlusRLib.FAS_EmergencyStop((byte)Para.iPortNo, (byte)Para.iMotorID);
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
            Stat.bJogP = true ;
            EziMOTIONPlusRLib.VELOCITY_OPTION_EX Option = new EziMOTIONPlusRLib.VELOCITY_OPTION_EX();
            Option.BIT_USE_CUSTOMACCDEC = true ;
            Option.wCustomAccDecTime = (ushort)(_dVel*1000/_dAcc) ;
            int iRet = 0 ;
            iRet = EziMOTIONPlusRLib.FAS_MoveVelocityEx ((byte)Para.iPortNo, (byte)Para.iMotorID , (uint)_dVel , 1 , Option);
        }
        /// <summary>
        /// -방향 조그 이동
        /// </summary>
        /// <param name="_dVel">초당 구동 속도 펄스</param>
        /// <param name="_dAcc">구동 가속율 펄스</param>
        /// <param name="_dDec">감속율 펄스</param>
        public void JogN(double _dVel,double _dAcc,double _dDec)
        {
            Stat.bJogP = false ;
            EziMOTIONPlusRLib.VELOCITY_OPTION_EX Option = new EziMOTIONPlusRLib.VELOCITY_OPTION_EX();
            Option.BIT_USE_CUSTOMACCDEC = true ;
            Option.wCustomAccDecTime = (ushort)(_dVel*1000/_dAcc) ;
            int iRet = 0 ;
            iRet = EziMOTIONPlusRLib.FAS_MoveVelocityEx ((byte)Para.iPortNo, (byte)Para.iMotorID , (uint)_dVel , 0 , Option);
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
        public void GoAbs(double _dPos,double _dVel,double _dAcc,double _dDec)
        {
            Stat.iTrgPos = (int)_dPos ;
            EziMOTIONPlusRLib.MOTION_OPTION_EX Option = new EziMOTIONPlusRLib.MOTION_OPTION_EX();
            Option.BIT_USE_CUSTOMACCEL = true ;
            Option.BIT_USE_CUSTOMDECEL = true ;
            Option.wCustomAccelTime = (ushort)(_dVel*1000/_dAcc) ;
            Option.wCustomDecelTime = (ushort)(_dVel*1000/_dDec) ;
            int iRet = 0 ;
            iRet = EziMOTIONPlusRLib.FAS_MoveSingleAxisAbsPosEx ((byte)Para.iPortNo, (byte)Para.iMotorID ,(int)_dPos , (uint)_dVel , Option);
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

            
        }
        /// <summary>
        /// 현재 파라미터들을 아진 함수를 이용하여 세팅함.
        /// </summary>

        
        public void ApplyPara(double _dPulsePerUnit)
        {
            m_dPulsePerUnit = _dPulsePerUnit ;

            //포트관련. 원래 있던 포트는 다른애가 쓸수도 있어서 그냥 냅둔다.
            //한번 여기 타면 파일에 저장 되어서 다음에 켤때는 생성 안됌.
            if(!OpenPorts.Contains(Para.iPortNo))
            {
                m_bConnected = false ;
                Init();
            }

            if(!m_bConnected) return ;

            int iRet = 0 ;
            iRet = EziMOTIONPlusRLib.FAS_SetParameter((byte)Para.iPortNo, (byte)Para.iMotorID ,0  , (int)9     );//레졸루션 10000 ;
            iRet = EziMOTIONPlusRLib.FAS_SetParameter((byte)Para.iPortNo, (byte)Para.iMotorID ,1  , (int)500000);//맥스스피드.
            iRet = EziMOTIONPlusRLib.FAS_SetParameter((byte)Para.iPortNo, (byte)Para.iMotorID ,2  , (int)1     );//시작스피드
            iRet = EziMOTIONPlusRLib.FAS_SetParameter((byte)Para.iPortNo, (byte)Para.iMotorID ,26 , (int)0     );//인포지션밸류.

            iRet = EziMOTIONPlusRLib.FAS_SetParameter((byte)Para.iPortNo, (byte)Para.iMotorID ,28 , Para.bDirection?1:0);//모터방향.

            
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

        }
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
        public void OneShotTrg(bool _bOnLevel,int _iTime)
        {

        }


        // 다축제어 라서 static이여야함.
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
            int iIdx = 0 ;
            return iIdx ;
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

        //2축 제어 겐트리.
        public void SetGantryEnable()
        {
        }
        public void SetGantryDisable()
        {
        }

        //동기구동, 버그 있는듯. 갠트리로 대신 쓴다.
        //현재 동기구동 상태인지 확인하기 위해 uint로 리턴한다.
        //AXT_RT_SUCCESS 반환하면 동기구동상태
        public bool SetLinkEnable(int _iSlvMotrNo)
        {
            
            return false;
        }

        public bool GetLinkMode()
        {
            
            return false;
        }

        public bool SetLinkDisable(int _iSlvMotrNo)
        {
           
            return false;

        }

        /// <summary>
        /// 홈이 지원 안되는 보드 같은경우 돌려주고 Update 함수 내부에서 처리 해야 한다.
        /// </summary>
        public void Update()
        {
            if(!m_bConnected )return ; 

            uint   dwOutStatus  = 0 ;
            uint   dwInStatus   = 0 ;
            uint   dwAxisStatus = 0 ;
            int    lCmdPos      = 0 ; 
            int    lActPos      = 0 ; 
            int    lPosErr      = 0 ; 
            int    lActVel      = 0 ; 
            ushort wPosItemNo   = 0 ;


            int iRet = 0 ;
            iRet = EziMOTIONPlusRLib.FAS_GetAllStatus((byte)Para.iPortNo  , (byte)Para.iMotorID , 
                ref dwInStatus   , 
                ref dwOutStatus  , 
                ref dwAxisStatus ,
                ref lCmdPos      ,
                ref lActPos      ,
                ref lPosErr      ,
                ref lActVel      ,
                ref wPosItemNo   );

            if(iRet == EziMOTIONPlusRLib.FMM_OK)
            {
                Stat.bHomeSnsr  = (dwAxisStatus & 0X00800000) != 0 ;
                Stat.bNLimSnsr  = (dwAxisStatus & 0X00000004) != 0 ;
                Stat.bPLimSnsr  = (dwAxisStatus & 0X00000002) != 0 ;
                
                Stat.bAlarmSgnl = (dwAxisStatus & 0X00000001) != 0 ;
                
                Stat.bInPosSgnl = (dwAxisStatus & 0X00080000) != 0 ;
                Stat.bZphaseSgnl= (dwAxisStatus & 0X01000000) != 0 ;
                
                Stat.bHomeDone  = (dwAxisStatus & 0X02000000) != 0 ;
                
                Stat.bServo     = (dwAxisStatus & 0X00100000) != 0 ;
                
                Stat.bRun       = lActVel != 0;
                Stat.bHomming   = (dwAxisStatus & 0X00040000) != 0 ;
                
                Stat.iCmdPos    = lCmdPos ;
                Stat.iEncPos    = lActPos ;
                Stat.iSpeed     = lActVel ;

                if(Stat.bJogN || Stat.bJogP)
                {
                    Stat.iTrgPos = Stat.iCmdPos ;
                }

                if(!Stat.bRun) 
                {
                    Stat.bJogP = false ;
                    Stat.bJogN = false ;
                }

            }

            

            
        }

        /// <summary>
        /// 해당모터축의 서브파라미터를 LoadSave함.
        /// </summary>
        /// <param name="_bLoad">true=로드 false=세이브</param>
        /// <returns>성공여부</returns>
        public bool LoadSave(bool _bLoad, string _sParaFolderPath, int _iMotrNo)
        {
            string sFilePath = _sParaFolderPath + "MotrEz" + _iMotrNo.ToString() + ".xml";
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

        public bool GetX(int _iNo)
        {
            return false ;
        }

        public bool GetY(int _iNo)
        {
            return false ;
        }

        public void SetY(int _iNo, bool _bOn)
        {

        }

        public void SetAbsRelMode(uint _uiAbsRelMode)
        {

        }

        public void LinkJogP(int _iSlvMotrNo, double _dVel, double _dAcc, double _dDec)
        {

        }

        public void LinkJogN(int _iSlvMotrNo, double _dVel, double _dAcc, double _dDec)
        {


        }
    }
}
