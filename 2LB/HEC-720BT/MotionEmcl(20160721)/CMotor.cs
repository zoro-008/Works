using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using COMMON;
using MotionInterface;
using EraeMotionApi;
using System.IO.Ports;
using System.Threading;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Runtime.CompilerServices;
using System.Reflection;
using DWORD = System.UInt32;
using System.Diagnostics;


namespace MotionEmcl
{
    /*
    1 좌측 스톱 스위치만을 검색 
    2 우측 스톱 스위치 검색 후 좌측 스톱 스위치 검색 
    3 우측 스톱 스위치 검색 후 양측으로부터 좌측 스톱 스위치 검색 
    4 양측으로부터 좌측 스톱 스위치 검색 
    5 네거티브 방향에서 홈 스위치 검색, 좌측 스톱 스위치 도달 시 방향 전환 
    6 포지티브 방향에서 홈 스위치 검색, 우측 스위치 도달 시 방향 전환 
    7 포지티브 방향에서 홈 스위치 검색, 엔드 스위치 무시 
    8 네거티브 방향에서 홈 스위치 검색, 엔드 스위치 무시 
     * 이 값들에 128을 더하면 홈 스위치 입력의 극성이 반전됨.

    */
    public enum HOME_MODE : uint
    {
        NLimit_On         = 1   ,
        PLimit_NLimit_On  = 2   ,
        PLimit_NLimit_Off = 3   , 
        NLimit_Off        = 4   ,
        NHome_Off         = 5   ,
        PHome_Off         = 6   ,
        PHome_Off_NoLimit = 7   ,
        NHome_Off_NoLimit = 8   ,
        NHome_On          = 133 ,
        PHome_On          = 134 ,
        PHome_On_NoLimit  = 135 ,
        NHome_On_NoLimit  = 136                
    }


    public enum ERR_DEFINE : uint
    {
        
    }
    //public enum PHASE_MODE : uint
    //{
    //    NormalClose = 0 ,
    //    NormalOpen  = 1 
    //}

    //public enum ENC_MULTI : uint
    //{
    //    Multi_4 = 0 ,
    //    Multi_2 = 1 ,
    //    Multi_1 = 2
    //}

    //public enum ENC_INPUT : uint
    //{
    //    EA_EB = 0,
    //    EB_EA = 1,
    //    UP_DN = 2,
    //    DN_UP = 3
    //}

    //public enum PULSE_MODE : uint
    //{
    //    Low_CW_CCW_2P  = 0 ,
    //    Low_CCW_CW_2P  = 1 , 
    //    High_CW_CCW_2P = 2 ,
    //    High_CCW_CW_2P = 3 ,
    //    Low_CW_CCW_1P  = 4 ,
    //    Low_CCW_CCW_1P = 5 ,
    //    High_CW_CCW_1P = 6 ,
    //    High_CCW_CW_1P = 7  
    //}

    
    [Serializable]
    public class CParaMotorEmcl
    {   
        [CategoryAttribute("EmclPara" ), DisplayNameAttribute("Physical No"         )]public int         iPhysicalNo  {get; set;} //실제모터 물리 어드레스.   
                                                                                                                    
        [CategoryAttribute("HomePara" ), DisplayNameAttribute("Home Mode"           )]public HOME_MODE   eHomeMode    {get; set;}
        
        
    } 

    //32비트만 지원....
    public class CMotor : IMotor
    {
        //[DllImport("EraeMotionApi.dll", EntryPoint = "?MyFunc@@ABXZ")]
        //public static extern int MyImportedFunc();

        private CParaMotorEmcl Para ;
        private double m_dPulsePerUnit;
        public CMotor() { }

        static private int   m_iMaxMotor;
        static private int   m_iPortID  ; //RS485카드 무조건 한장만 쓴다.
        static private int   m_iBoudRate; 
        static private bool  m_bInit = false; 

        static private EMCL.MotorStatus[] MotorStat ;
        static private double [] MotorEncPos ;

        
        /// <summary>
        /// 인터페이스 상속 아니고 그냥 이클레스에서만 쓰는 함수
        /// </summary>
        /// <param name="_iCode"></param>
        /// <returns></returns>
        private string GetErrMsg(int _iCode)
        {
            EMCL.ReplyCode eCode = (EMCL.ReplyCode)_iCode ;
            switch (eCode)
            {
                default : return "" ;
                case EMCL.ReplyCode.EMCL_NO_ERROR				: return ""                   ;
                case EMCL.ReplyCode.EMCL_ERR_CMD_FULL			: return "Command is Full"    ;
                case EMCL.ReplyCode.EMCL_ERR_BADCHECKSUM		: return "Bad Checksum"       ;
                case EMCL.ReplyCode.EMCL_ERR_INVALID_COMMAND	: return "Invalid Command"    ;
                case EMCL.ReplyCode.EMCL_ERR_INVALID_TYPE		: return "Invalid Type"       ;
                case EMCL.ReplyCode.EMCL_ERR_INVALID_VALUE		: return "Invalid Value"      ;
                case EMCL.ReplyCode.EMCL_ERR_EEPROM_LOCKED		: return "EEPROM Locked"      ;
                case EMCL.ReplyCode.EMCL_ERR_UNKNOWN_COMMAND	: return "Unknown Command"    ;
                case EMCL.ReplyCode.EMCL_ERR_COMM_CLOSED		: return "Com is Closed"      ;
                case EMCL.ReplyCode.EMCL_ERR_DATA_NOTREADY		: return "Data Not Ready"     ;
                case EMCL.ReplyCode.EMCL_ERR_COMM_TIMEOUT		: return "Com Timeout"        ;
                case EMCL.ReplyCode.EMCL_ERR_PARAM_OUTOFRANGE   : return "Param Out of Range" ;
            }
        }

        /// <summary>
        /// 초기화 함수.
        /// </summary>
        /// <returns>초기화 성공여부</returns>
        public bool Init()
        {
            Para = new CParaMotorEmcl();
            if (m_bInit == false)
            {
                m_bInit = true;
                //통합 보드 초기화 부분.

                m_iPortID = 1 ;
                //9600
                //14400
                //19200
                //38400
                //57600
                //115200
                m_iBoudRate = 57600 ;

                EMCL.ERAETech_EMCL_OpenComm(m_iPortID, m_iBoudRate);

                if (!EMCL.ERAETech_EMCL_IsPortOpen(m_iPortID))
                {
                   Log.ShowMessage("EMCL" , "Port Open Error PortNo =" + m_iPortID.ToString());
                   //return false ;
                }

                
                m_iMaxMotor = EMCL.ERAETech_EMCL_GetNodeCount(m_iPortID, 10);//포트에 몇개의 모터가 달려 있는지...확인 10개까지 확인해 본다.
                if (m_iMaxMotor == 0)
                {
                    m_iMaxMotor = 10;
                    //Log.ShowMessage("EMCL", "Motor Connected Error PortNo =" + m_iPortID.ToString());

                }
                
                MotorStat   = new EMCL.MotorStatus[m_iMaxMotor];
                MotorEncPos = new double[m_iMaxMotor];

            }

            return true;
        }
        /// <summary>
        /// 종료 함수
        /// </summary>
        /// <returns>종료 성공여부</returns>
        public bool Close()
        {
            EMCL.ERAETech_EMCL_CloseComm(-1);//해당포트를 닫는다 -1 이면 모든 포트 닫음.
            return true;
        }
        /// <summary>
        /// 모터의 인포지션 확인 안한 정지여부.
        /// </summary>
        /// <returns>정지상태 true , 구동상태 false</returns>
        public bool GetStop()
        {
         
            return MotorStat[Para.iPhysicalNo].nIsBusy == 0 ;

            
        }

        /// <summary>
        /// 서보 온오프
        /// </summary>
        /// <param name="_bOn">온오프</param>
        public void SetServo( bool _bOn)
        {
            int iRet = 0 ;
            EMCL.ERAETech_EMCL_Sync_SetServoOn(m_iPortID, (byte)Para.iPhysicalNo, _bOn ? 1 : 0, ref iRet);
            if (iRet != 0)
            {
                Log.ShowMessage("EMCL_"+Para.iPhysicalNo.ToString()+"Axis" , GetErrMsg(iRet));
            }

        }
        /// <summary>
        /// 서보상태를 받아옴.
        /// </summary>
        /// <returns>온오프</returns>
        public bool GetServo()
        {
            bool bRet;

            bRet = MotorStat[Para.iPhysicalNo].nIsServoOn != 0 ;

            return bRet;
        }
        /// <summary>
        /// 리셑 시그널 온오프 제어
        /// </summary>
        /// <param name="_bOn">온오프</param>
        public void SetReset( bool _bOn)
        {
            if(!_bOn) return;
            
            int iRet=0;

            EMCL.ERAETech_EMCL_Sync_ResetAlarm(m_iPortID, (byte)Para.iPhysicalNo, ref iRet);
            if (iRet != 0)
            {
                Log.ShowMessage("EMCL_"+Para.iPhysicalNo.ToString()+"Axis" , GetErrMsg(iRet));
            }
        }

        /// <summary>
        /// 펄스 커멘드 리턴.
        /// </summary>
        /// <returns>펄스 리턴</returns>
        public double GetCmdPos()
        {
            return MotorStat[Para.iPhysicalNo].nActualPos;            
        }
        /// <summary>
        /// 펄스 엔코더 리턴.
        /// </summary>
        /// <returns>펄스 리턴</returns>
        public double GetEncPos()
        {
            return MotorEncPos[Para.iPhysicalNo] ;
        }
        /// <summary>
        /// Command Encoder Target포지션 세팅.
        /// </summary>
        /// <param name="_dPos">세팅할 펄스</param>
        public void SetPos( double _dPos)
        {
            int iRet = 0 ;
            EMCL.ERAETech_EMCL_Sync_SendCommand(m_iPortID, (byte)Para.iPhysicalNo, (byte)EMCL.MotorCmd.EMCL_MVP, (byte)EMCL.AxisParam.AXIS_PARAM_ACTUAL_POS, 0, (int)_dPos,ref iRet);
            if (iRet != 0)
            {
                Log.ShowMessage("EMCL_"+Para.iPhysicalNo.ToString()+"Axis" , GetErrMsg(iRet));
                return ;
            }
            EMCL.ERAETech_EMCL_Sync_SendCommand(m_iPortID, (byte)Para.iPhysicalNo, (byte)EMCL.MotorCmd.EMCL_MVP, (byte)EMCL.AxisParam.AXIS_PARAM_ENCODER_POS, 0, (int)_dPos,ref iRet);
            if (iRet != 0)
            {
                Log.ShowMessage("EMCL_"+Para.iPhysicalNo.ToString()+"Axis" , GetErrMsg(iRet));
                return ;
            }
        }

        //Signal...
        /// <summary>
        /// 홈센서 시그널.
        /// </summary>
        /// <returns>OnOff상태</returns>
        public bool GetHomeSnsr()
        {
            return MotorStat[Para.iPhysicalNo].nHomeStatus != 0;
        }


        /// <summary>
        /// -리밋 센서 시그널
        /// </summary>
        /// <returns>OnOff상태</returns>
        public bool GetNLimSnsr()
        {
            return MotorStat[Para.iPhysicalNo].nRightLimitStatus != 0;            
        }
        /// <summary>
        /// +리밋 센서 시그널
        /// </summary>
        /// <returns>OnOff상태</returns>
        public bool GetPLimSnsr()
        {
            return MotorStat[Para.iPhysicalNo].nLeftLimitStatus != 0;
        }
        /// <summary>
        /// 엔코더 Z상 시그널 상태
        /// </summary>
        /// <returns>OnOff상태</returns>
        public bool GetZphaseSgnl()
        {
            //z Phase dosen't exist 
            return false ;
        }
        /// <summary>
        /// 서보펙 인포지션 시그널 상태
        /// </summary>
        /// <returns>OnOff상태</returns>
        public bool GetInPosSgnl() //Servo Pack InPosition Signal.
        {
            return MotorStat[Para.iPhysicalNo].nInPosition != 0;
        }
        /// <summary>
        /// 서보펙 알람 시그널 상태
        /// </summary>
        /// <returns>OnOff상태</returns>
        public bool GetAlarmSgnl()
        {
            return MotorStat[Para.iPhysicalNo].nAlarm != 0;
        }

        /// <summary>
        /// 홈을 수행한다.
        /// </summary>
        /// <returns>수행 성공여부.</returns>
        public bool GoHome(double _dHomeVelFirst, double _dHomeVelLast, double _dHomeAccFirst)
        {

            SetServo(true);

            int   iRet=0;

            //홈찾는 방법
            EMCL.ERAETech_EMCL_Sync_SetSearchMode(m_iPortID, (byte)Para.iPhysicalNo, (int)Para.eHomeMode, ref iRet);
            if (iRet != 0)
            {
                Log.ShowMessage("EMCL_"+Para.iPhysicalNo.ToString()+"Axis" , GetErrMsg(iRet));
                return false ;
            }

            //가감속
            EMCL.ERAETech_EMCL_Sync_SetMaxAcceleration(m_iPortID, (byte)Para.iPhysicalNo, (int)_dHomeAccFirst, ref iRet);
            if (iRet != 0)
            {
                Log.ShowMessage("EMCL_"+Para.iPhysicalNo.ToString()+"Axis" , GetErrMsg(iRet));
                return false ;
            }

            

            //첫번째 속도.
            EMCL.ERAETech_EMCL_Sync_SetRefSearchSpeed(m_iPortID, (byte)Para.iPhysicalNo, (int)_dHomeVelFirst, ref iRet);
            if (iRet != 0)
            {
                Log.ShowMessage("EMCL_"+Para.iPhysicalNo.ToString()+"Axis" , GetErrMsg(iRet));
                return false ;
            }

            //마지막 속도.
            EMCL.ERAETech_EMCL_Sync_SetRefSwitchSpeed(m_iPortID, (byte)Para.iPhysicalNo, (int)_dHomeVelLast, ref iRet);
            if (iRet != 0)
            {
                Log.ShowMessage("EMCL_"+Para.iPhysicalNo.ToString()+"Axis" , GetErrMsg(iRet));
                return false ;
            }

            //요이 땅.
            EMCL.ERAETech_EMCL_Sync_StartRefSearch(m_iPortID, (byte)Para.iPhysicalNo, ref iRet);
            if (iRet != 0)
            {
                Log.ShowMessage("EMCL_"+Para.iPhysicalNo.ToString()+"Axis" , GetErrMsg(iRet));
                return false ;
            }


            return true;
        }

        /// <summary>
        /// 홈동작 완료 확인.
        /// </summary>
        /// <returns>홈동작 완료여부</returns>
        public bool GetHomeDone()
        {
            int iRet = 0;
            EMCL.ERAETech_EMCL_Sync_GetRefSearchStatus(m_iPortID, (byte)Para.iPhysicalNo , ref iRet);
            return  iRet == 0 ;

        }
        /// <summary>
        /// 강제로 홈돈 시그널 세팅.
        /// </summary>
        /// <param name="_bOn">세팅 값.</param>
        public void SetHomeDone( bool _bOn)
        {
            return;
           
        }
        /// <summary>
        /// 홈시퀜스를 중단한다.
        /// </summary>
        public void StopHome()
        {
            int   iRet=0;
            EMCL.ERAETech_EMCL_Sync_StopRefSearch(m_iPortID, (byte)Para.iPhysicalNo, ref iRet);
            if (iRet != 0)
            {
                Log.ShowMessage("EMCL_"+Para.iPhysicalNo.ToString()+"Axis" , GetErrMsg(iRet));
            }
        }

        public bool GetHoming()
        {
            return false ;
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
            //혹시병신짓 하면 Acc설정해 보자.
            int iRet = 0;

            EMCL.ERAETech_EMCL_Sync_Stop(m_iPortID, (byte)Para.iPhysicalNo, ref iRet);
            if (iRet != 0)
            {
                Log.ShowMessage("EMCL_"+Para.iPhysicalNo.ToString()+"Axis" , GetErrMsg(iRet));
            }
        }
        /// <summary>
        /// 모터 감속 무시 긴급정지.
        /// </summary>
        public void EmgStop()
        {
            Stop();
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
            int iRet=0;

            EMCL.ERAETech_EMCL_Sync_SendCommand(m_iPortID, (byte)Para.iPhysicalNo,  5, 0, 0, (int)_dAcc,ref iRet);  //3번째 인자가 가감속 설정하는 부분 가속 = 5, 감속 = 17
            if (iRet != 0)
            {
                Log.ShowMessage("EMCL_"+Para.iPhysicalNo.ToString()+"Axis" , GetErrMsg(iRet));
            }
            EMCL.ERAETech_EMCL_Sync_SendCommand(m_iPortID, (byte)Para.iPhysicalNo, 17, 0, 0, (int)_dDec,ref iRet);  //3번째 인자가 가감속 설정하는 부분 가속 = 5, 감속 = 17
            if (iRet != 0)
            {
                Log.ShowMessage("EMCL_"+Para.iPhysicalNo.ToString()+"Axis" , GetErrMsg(iRet));
            }

            
            EMCL.ERAETech_EMCL_Sync_RotateRight(m_iPortID, (byte)Para.iPhysicalNo, (int)_dVel, ref iRet);
            if (iRet != 0)
            {
                Log.ShowMessage("EMCL_"+Para.iPhysicalNo.ToString()+"Axis" , GetErrMsg(iRet));
            }
        }
        /// <summary>
        /// -방향 조그 이동
        /// </summary>
        /// <param name="_dVel">초당 구동 속도 펄스</param>
        /// <param name="_dAcc">구동 가속율 펄스</param>
        /// <param name="_dDec">감속율 펄스</param>
        public void JogN( double _dVel, double _dAcc, double _dDec)
        {
            int iRet=0;

            //EMCL.ERAETech_EMCL_Sync_SetMaxAcceleration(m_iPortID, (byte)Para.iPhysicalNo, (int)_dAcc, ref iRet);
            //if (iRet != 0)
            //{
            //    Log.ShowMessage("EMCL_"+Para.iPhysicalNo.ToString()+"Axis" , GetErrMsg(iRet));
            //}
            EMCL.ERAETech_EMCL_Sync_SendCommand(m_iPortID, (byte)Para.iPhysicalNo,  5, 0, 0, (int)_dAcc,ref iRet);  //3번째 인자가 가감속 설정하는 부분 가속 = 5, 감속 = 17
            if (iRet != 0)
            {
                Log.ShowMessage("EMCL_"+Para.iPhysicalNo.ToString()+"Axis" , GetErrMsg(iRet));
            }
            EMCL.ERAETech_EMCL_Sync_SendCommand(m_iPortID, (byte)Para.iPhysicalNo, 17, 0, 0, (int)_dDec,ref iRet);  //3번째 인자가 가감속 설정하는 부분 가속 = 5, 감속 = 17
            if (iRet != 0)
            {
                Log.ShowMessage("EMCL_"+Para.iPhysicalNo.ToString()+"Axis" , GetErrMsg(iRet));
            }
            EMCL.ERAETech_EMCL_Sync_RotateLeft(m_iPortID, (byte)Para.iPhysicalNo, (int)_dVel, ref iRet);
            if (iRet != 0)
            {
                Log.ShowMessage("EMCL_"+Para.iPhysicalNo.ToString()+"Axis" , GetErrMsg(iRet));
            }
            
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
            int iRet=0;
            //EMCL.ERAETech_EMCL_Sync_SetMaxAcceleration(m_iPortID, (byte)Para.iPhysicalNo, (int)_dAcc, ref iRet);
            
            EMCL.ERAETech_EMCL_Sync_SendCommand(m_iPortID, (byte)Para.iPhysicalNo, 5, 0, 0, (int)_dAcc, ref iRet);  //3번째 인자가 가감속 설정하는 부분 가속 = 5, 감속 = 17
            if (iRet != 0)
            {
                Log.ShowMessage("EMCL_"+Para.iPhysicalNo.ToString()+"Axis" , GetErrMsg(iRet));
            }
            EMCL.ERAETech_EMCL_Sync_SendCommand(m_iPortID, (byte)Para.iPhysicalNo, 17, 0, 0, (int)_dDec, ref iRet);  //3번째 인자가 가감속 설정하는 부분 가속 = 5, 감속 = 17
            if (iRet != 0)
            {
                Log.ShowMessage("EMCL_"+Para.iPhysicalNo.ToString()+"Axis" , GetErrMsg(iRet));
            }

            EMCL.ERAETech_EMCL_Sync_SetMaxPositionSpeed(m_iPortID, (byte)Para.iPhysicalNo, (int)_dVel, ref iRet);
            if (iRet != 0)
            {
                Log.ShowMessage("EMCL_"+Para.iPhysicalNo.ToString()+"Axis" , GetErrMsg(iRet));
            }

            EMCL.ERAETech_EMCL_Sync_MoveToPosition(m_iPortID, (byte)Para.iPhysicalNo, 0, (int)_dPos, ref iRet);
            if (iRet != 0)
            {
                Log.ShowMessage("EMCL_"+Para.iPhysicalNo.ToString()+"Axis" , GetErrMsg(iRet));
            }
            
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
        /// 현재 파라미터들을 함수를 이용하여 세팅함.
        /// </summary>
        public void ApplyPara(double _dPulsePerUnit)
        {
            m_dPulsePerUnit = _dPulsePerUnit;
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

        //2축 제어 겐트리.
        public void SetGantryEnable(double _dPulsePerUnit)
        {
        }
        public void SetGantryDisable()
        {
        }

        /// <summary>
        /// 홈이 지원 안되는 보드 같은경우 돌려주고 Update 함수 내부에서 처리 해야 한다.
        /// 파익스는 네트워크 타입이라 너무 느려서 업데이트에서 한번만 스캔하여 담아두고 
        /// </summary>
        public void Update()
        {    
            if(Para.iPhysicalNo != 0) return ;

            int iEncPos  = 0 ;

            for (int i = 0; i < m_iMaxMotor; i++)
            {
                EMCL.ERAETech_EMCL_GetAllStatus(m_iPortID, (byte)i, ref MotorStat[i]);
                //EMCL.ERAETech_EMCL_GetRefSearchStatus(m_iPortID, (byte)i);

                EMCL.ERAETech_EMCL_Sync_GetEncoderPos(m_iPortID, (byte)i, ref iEncPos);
                MotorEncPos[i] = (double)iEncPos ;
            }            
        }

        /// <summary>
        /// 해당모터축의 서브파라미터를 LoadSave함.
        /// </summary>
        /// <param name="_bLoad">true=로드 false=세이브</param>
        /// <returns>성공여부</returns>
        public bool LoadSave(bool _bLoad, string _sParaFolderPath, int _iMotrNo)
        {
            string sFilePath = _sParaFolderPath + "MotrEmcl" + _iMotrNo.ToString() + ".xml";
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
    }
}
