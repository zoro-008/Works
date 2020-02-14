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
        Test              = 65  ,
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
    public enum PHASE : uint
    {
        RowActive = 0,
        HighActive = 1
    }
    
    [Serializable]
    public class CParaMotorEmcl
    {   
        [CategoryAttribute("EmclPara" ), DisplayNameAttribute("Physical No"         )]public int     iPhysicalNo  {get; set;} //실제모터 물리 어드레스.   
        [CategoryAttribute("EmclPara" ), DisplayNameAttribute("+ Limit Phase"       )]public PHASE   ePLimPhase   {get; set;} // 정방향 리미트(+End limit)의 액티브레벨 설정 
        [CategoryAttribute("EmclPara" ), DisplayNameAttribute("- Limit Phase"       )]public PHASE   eNLimPhase   {get; set;} // 역방향 리미트(-End limit)의 액티브레벨 설정 

                                                                                                            
        [CategoryAttribute("HomePara" ), DisplayNameAttribute("Home Mode"           )]public HOME_MODE   eHomeMode    {get; set;}
        

        
        
    } 

    //32비트만 지원....
    public class CMotor : IMotor
    {
        //[DllImport("EraeMotionApi.dll", EntryPoint = "?MyFunc@@ABXZ")]
        //public static extern int MyImportedFunc();

        CParaMotorEmcl Para ;
        public CMotor() { }

        //Test..
        static private int   m_iMaxMotor;
        static private int   m_iPortID  ; //RS485카드 무조건 한장만 쓴다.
        static private int   m_iBoudRate; 
        static private bool  m_bInit = false; 

        static private EMCL.MotorStatus2[] MotorStat ;
        static private double [] MotorEncPos ;
        static private double [] MotorCmdPos ;
        static private bool   [] Inposition  ;
        static private bool   [] Alarm       ;

        
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

                m_iPortID = 3 ;
                //9600
                //14400
                //19200
                //38400
                //57600
                //115200
                m_iBoudRate = 115200;

                //ERAETech_EMCL_OpenComm
                EMCL.ERAETech_EMCL_OpenComm(m_iPortID, m_iBoudRate);

                if (!EMCL.ERAETech_EMCL_IsPortOpen(m_iPortID))
                {
                   Log.ShowMessage("EMCL" , "Port Open Error PortNo =" + m_iPortID.ToString());
                   //return false ;
                }
                
                
                m_iMaxMotor = EMCL.ERAETech_EMCL_GetNodeCount(m_iPortID, 10);//포트에 몇개의 모터가 달려 있는지...확인 10개까지 확인해 본다.


                //m_iMaxMotor = EMCL.ERAETech_EMCL_GetNodeCount(3, 2);//포트에 몇개의 모터가 달려 있는지...확인 10개까지 확인해 본다.
                //m_iMaxMotor = EMCL.ERAETech_EMCL_GetNodeCount(4, 2);//포트에 몇개의 모터가 달려 있는지...확인 10개까지 확인해 본다.

                //if (m_iMaxMotor == 0)
                //{
                //    m_iMaxMotor = 10;
                //}

                //Physical address 
                //m_iMaxMotor++;

                
                MotorStat   = new EMCL.MotorStatus2[m_iMaxMotor];
                MotorEncPos = new double[m_iMaxMotor];
                MotorCmdPos = new double[m_iMaxMotor];

                Inposition  = new bool [m_iMaxMotor];
                Alarm       = new bool [m_iMaxMotor];


            }

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
            //ERARTech Address Start No = 1;
            int iERARRPhysicalNo = Para.iPhysicalNo + 1;


            if (EMCL.ERAETech_EMCL_Sync_SetServoOn(m_iPortID, (byte)iERARRPhysicalNo, _bOn ? 1 : 0) != (int)EMCL.ReplyCode.EMCL_NO_ERROR)
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
            //ERARTech Address Start No = 1;
            int iERARRPhysicalNo = Para.iPhysicalNo + 1;


            if (EMCL.ERAETech_EMCL_Sync_ResetAlarm(m_iPortID, (byte)iERARRPhysicalNo) != (int)EMCL.ReplyCode.EMCL_NO_ERROR)
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
            return MotorCmdPos[Para.iPhysicalNo];            
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
            //ERARTech Address Start No = 1;
            int iERARRPhysicalNo = Para.iPhysicalNo + 1;

            EMCL.ERAETech_EMCL_Sync_SendCommand(m_iPortID, (byte)iERARRPhysicalNo, (byte)EMCL.MotorCmd.EMCL_SAP, (byte)EMCL.AxisParam.AXIS_PARAM_ACTUAL_POS, 0, (int)_dPos, ref iRet);
            //EMCL.ERAETech_EMCL_Sync_SendCommand(m_iPortID, 1, (byte)EMCL.MotorCmd.EMCL_SAP, (byte)EMCL.AxisParam.AXIS_PARAM_ACTUAL_POS, (byte)iERARRPhysicalNo, (int)_dPos, ref iRet);
            if (iRet != 0)
            {
                //sunsun
                //Log.ShowMessage("EMCL_"+Para.iPhysicalNo.ToString()+"Axis" , GetErrMsg(iRet));
                //return ;
            }
            EMCL.ERAETech_EMCL_Sync_SendCommand(m_iPortID, (byte)iERARRPhysicalNo, (byte)EMCL.MotorCmd.EMCL_SAP, (byte)EMCL.AxisParam.AXIS_PARAM_ENCODER_POS, 0, (int)_dPos, ref iRet);
            if (iRet != 0)
            {
                //Log.ShowMessage("EMCL_"+Para.iPhysicalNo.ToString()+"Axis" , GetErrMsg(iRet));
                //return ;
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
            if (Para.eNLimPhase == PHASE.RowActive)
            {
                return MotorStat[Para.iPhysicalNo].nRightLimitStatus != 0;    
            }
            return MotorStat[Para.iPhysicalNo].nRightLimitStatus == 0;            
        }
        /// <summary>
        /// +리밋 센서 시그널
        /// </summary>
        /// <returns>OnOff상태</returns>
        public bool GetPLimSnsr()
        {
            if (Para.ePLimPhase == PHASE.RowActive)
            {
                return MotorStat[Para.iPhysicalNo].nLeftLimitStatus != 0;
            }
            return MotorStat[Para.iPhysicalNo].nLeftLimitStatus == 0;
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
            //return MotorStat[Para.iPhysicalNo].nInPosition != 0;

            return Inposition[Para.iPhysicalNo];
                 

        }
        /// <summary>
        /// 서보펙 알람 시그널 상태
        /// </summary>
        /// <returns>OnOff상태</returns>
        public bool GetAlarmSgnl()
        {
            return Alarm[Para.iPhysicalNo];
        }

        /// <summary>
        /// 홈을 수행한다.
        /// </summary>
        /// <returns>수행 성공여부.</returns>
        public bool GoHome( double _dHomeVelFirst, double _dHomeVelLast, double _dHomeAccFirst, double _dMinPosPulse, double _dMaxPosPulse)
        {

            SetServo(true);

            int   iRet=0;
            //ERARTech Address Start No = 1;
            int iERARRPhysicalNo = Para.iPhysicalNo + 1;

            //application Reset
            if (EMCL.ERAETech_EMCL_Sync_SendCommand(m_iPortID, (byte)iERARRPhysicalNo, 129, 1, 0, 1, ref iRet) != (int)EMCL.ReplyCode.EMCL_NO_ERROR)
            {
                Log.ShowMessage("EMCL_" + Para.iPhysicalNo.ToString() + "Axis", GetErrMsg(iRet));
                return false;
            }

            Thread.Sleep(10);






            ////홈찾는 방법            
            //if (EMCL.ERAETech_EMCL_Sync_SetSearchMode(m_iPortID, (byte)iERARRPhysicalNo, (int)Para.eHomeMode) != (int)EMCL.ReplyCode.EMCL_NO_ERROR)
            //{
            //    Log.ShowMessage("EMCL_"+Para.iPhysicalNo.ToString()+"Axis" , GetErrMsg(iRet));
            //    return false ;
            //}

            ////가감속            
            //if (EMCL.ERAETech_EMCL_Sync_SetMaxAcceleration(m_iPortID, (byte)iERARRPhysicalNo, (int)_dHomeAccFirst) != (int)EMCL.ReplyCode.EMCL_NO_ERROR)
            //{
            //    Log.ShowMessage("EMCL_"+Para.iPhysicalNo.ToString()+"Axis" , GetErrMsg(iRet));
            //    return false ;
            //}

            

            ////첫번째 속도.            
            //if (EMCL.ERAETech_EMCL_Sync_SetRefSearchSpeed(m_iPortID, (byte)iERARRPhysicalNo, (int)_dHomeVelFirst) != (int)EMCL.ReplyCode.EMCL_NO_ERROR)
            //{
            //    Log.ShowMessage("EMCL_"+Para.iPhysicalNo.ToString()+"Axis" , GetErrMsg(iRet));
            //    return false ;
            //}

            ////마지막 속도.      
            //if (EMCL.ERAETech_EMCL_Sync_SetRefSwitchSpeed(m_iPortID, (byte)iERARRPhysicalNo, (int)_dHomeVelLast) != (int)EMCL.ReplyCode.EMCL_NO_ERROR)
            //{
            //    Log.ShowMessage("EMCL_"+Para.iPhysicalNo.ToString()+"Axis" , GetErrMsg(iRet));
            //    return false ;
            //}

            ////요이 땅.
            ////sunsun

            //if (EMCL.ERAETech_EMCL_Sync_StartRefSearch(m_iPortID, (byte)iERARRPhysicalNo) != (int)EMCL.ReplyCode.EMCL_NO_ERROR)
            //{
            //    Log.ShowMessage("EMCL_"+Para.iPhysicalNo.ToString()+"Axis" , GetErrMsg(iRet));
            //    return false ;
            //}

            
            if (EMCL.ERAETech_EMCL_Sync_SetGlobalParamB(m_iPortID, (byte)iERARRPhysicalNo, 0, 2, 10) != (int)EMCL.ReplyCode.EMCL_NO_ERROR)
            {
                Log.ShowMessage("EMCL_" + Para.iPhysicalNo.ToString() + "Axis", GetErrMsg(iRet));
                return false;
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
            //ERARTech Address Start No = 1;
            int iERARRPhysicalNo = Para.iPhysicalNo + 1;

            //EMCL.ERAETech_EMCL_Sync_GetRefSearchStatus(m_iPortID, (byte)iERARRPhysicalNo, ref iRet);

            if (EMCL.ERAETech_EMCL_Sync_GetGlobalParamB(m_iPortID, (byte)iERARRPhysicalNo, 0, 2, ref iRet) != (int)EMCL.ReplyCode.EMCL_NO_ERROR)
            {
                Log.ShowMessage("EMCL_" + Para.iPhysicalNo.ToString() + "Axis", GetErrMsg(iRet));
                return false;
            }

            

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
            //ERARTech Address Start No = 1;
            int iERARRPhysicalNo = Para.iPhysicalNo + 1;


            if (EMCL.ERAETech_EMCL_Sync_StopRefSearch(m_iPortID, (byte)iERARRPhysicalNo) != (int)EMCL.ReplyCode.EMCL_NO_ERROR)
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
            //ERARTech Address Start No = 1;
            int iERARRPhysicalNo = Para.iPhysicalNo + 1;

            //if(EMCL.ERAETech_EMCL_GetHomeSwitchStatus)

            int iHome = 0 ;

            //if (EMCL.ERAETech_EMCL_Sync_GetRefSearchStatus(m_iPortID, (byte)iERARRPhysicalNo, ref iHome) != (int)EMCL.ReplyCode.EMCL_NO_ERROR)
            //{
            //    Log.ShowMessage("EMCL_" + Para.iPhysicalNo.ToString() + "Axis", GetErrMsg(iRet));
            //}
            //
            //if (iHome == 0)
            //{
            //    if (EMCL.ERAETech_EMCL_Sync_Stop(m_iPortID, (byte)iERARRPhysicalNo) != (int)EMCL.ReplyCode.EMCL_NO_ERROR)
            //    {
            //        Log.ShowMessage("EMCL_" + Para.iPhysicalNo.ToString() + "Axis", GetErrMsg(iRet));
            //    }
            //}
            //else
            //{
            //    StopHome();
            //}

            if (EMCL.ERAETech_EMCL_Sync_Stop(m_iPortID, (byte)iERARRPhysicalNo) != (int)EMCL.ReplyCode.EMCL_NO_ERROR)
            {
                Log.ShowMessage("EMCL_" + Para.iPhysicalNo.ToString() + "Axis", GetErrMsg(iRet));
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
            //ERARTech Address Start No = 1;
            int iERARRPhysicalNo = Para.iPhysicalNo + 1;


            if (EMCL.ERAETech_EMCL_Sync_SendCommand(m_iPortID, (byte)iERARRPhysicalNo, (byte)EMCL.MotorCmd.EMCL_SAP, 5, 0, (int)_dAcc, ref iRet) != (int)EMCL.ReplyCode.EMCL_NO_ERROR)  //3번째 인자가 가감속 설정하는 부분 가속 = 5, 감속 = 17 != 0)
            {
                Log.ShowMessage("EMCL_"+Para.iPhysicalNo.ToString()+"Axis" , GetErrMsg(iRet));
            }
              //3번째 인자가 가감속 설정하는 부분 가속 = 5, 감속 = 17
            int iFuncRet = EMCL.ERAETech_EMCL_Sync_SendCommand(m_iPortID, (byte)iERARRPhysicalNo,(byte)EMCL.MotorCmd.EMCL_SAP , 17, 0, (int)_dDec, ref iRet) ;
            if (iFuncRet != (int)EMCL.ReplyCode.EMCL_NO_ERROR)
            {
                Log.ShowMessage("EMCL_"+Para.iPhysicalNo.ToString()+"Axis" , GetErrMsg(iRet));
            }


            
            if (EMCL.ERAETech_EMCL_Sync_RotateRight(m_iPortID, (byte)iERARRPhysicalNo, (int)_dVel) != (int)EMCL.ReplyCode.EMCL_NO_ERROR)
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
            //ERARTech Address Start No = 1;
            int iERARRPhysicalNo = Para.iPhysicalNo + 1;

            //EMCL.ERAETech_EMCL_Sync_SetMaxAcceleration(m_iPortID, (byte)Para.iPhysicalNo, (int)_dAcc, ref iRet);
            //if (iRet != 0)
            //{
            //    Log.ShowMessage("EMCL_"+Para.iPhysicalNo.ToString()+"Axis" , GetErrMsg(iRet));
            //}
                                               //(int nPortNum, byte nModuleId        , byte nCmd, byte nType, byte nMotorId, Int32 nValue, ref Int32 nRetValue);
            //EMCL.ERAETech_EMCL_Sync_SendCommand(m_iPortID   , (byte)iERARRPhysicalNo, 5        , 0         ,  0           , (int)_dAcc  , ref iRet);  //3번째 인자가 가감속 설정하는 부분 가속 = 5, 감속 = 17
             //3번째 인자가 가감속 설정하는 부분 가속 = 5, 감속 = 17
            //EMCL.ERAETech_EMCL_Sync_GetRefSearchStatus(m_iPortID, 0, ref iRet);
            if (EMCL.ERAETech_EMCL_Sync_SendCommand(m_iPortID, (byte)iERARRPhysicalNo, (byte)EMCL.MotorCmd.EMCL_SAP, 5, 0, (int)_dAcc, ref iRet) != (int)EMCL.ReplyCode.EMCL_NO_ERROR)
            {
                //sunsun
                //Log.ShowMessage("EMCL_"+Para.iPhysicalNo.ToString()+"Axis" , GetErrMsg(iRet));
            }
            //3번째 인자가 가감속 설정하는 부분 가속 = 5, 감속 = 17
            if (EMCL.ERAETech_EMCL_Sync_SendCommand(m_iPortID, (byte)iERARRPhysicalNo, (byte)EMCL.MotorCmd.EMCL_SAP, 17, 0, (int)_dDec, ref iRet) != (int)EMCL.ReplyCode.EMCL_NO_ERROR)
            {
                //sunsun
                //Log.ShowMessage("EMCL_"+Para.iPhysicalNo.ToString()+"Axis" , GetErrMsg(iRet));
            }
            ;
            if (EMCL.ERAETech_EMCL_Sync_RotateLeft(m_iPortID, (byte)iERARRPhysicalNo, (int)_dVel) != (int)EMCL.ReplyCode.EMCL_NO_ERROR)
            {
                //sunsun
                //Log.ShowMessage("EMCL_"+Para.iPhysicalNo.ToString()+"Axis" , GetErrMsg(iRet));
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
            //ERARTech Address Start No = 1;
            int iERARRPhysicalNo = Para.iPhysicalNo + 1;
            //EMCL.ERAETech_EMCL_Sync_SetMaxAcceleration(m_iPortID, (byte)Para.iPhysicalNo, (int)_dAcc, ref iRet);

            ;  //3번째 인자가 가감속 설정하는 부분 가속 = 5, 감속 = 17
            if (EMCL.ERAETech_EMCL_Sync_SendCommand(m_iPortID, (byte)iERARRPhysicalNo, 5, 0, 0, (int)_dAcc, ref iRet) != (int)EMCL.ReplyCode.EMCL_NO_ERROR)
            {
                //sunsun
                //Log.ShowMessage("EMCL_"+Para.iPhysicalNo.ToString()+"Axis" , GetErrMsg(iRet));
            }
            ;  //3번째 인자가 가감속 설정하는 부분 가속 = 5, 감속 = 17
            if (EMCL.ERAETech_EMCL_Sync_SendCommand(m_iPortID, (byte)iERARRPhysicalNo, 17, 0, 0, (int)_dDec, ref iRet) != (int)EMCL.ReplyCode.EMCL_NO_ERROR)
            {
                //sunsun
                //Log.ShowMessage("EMCL_"+Para.iPhysicalNo.ToString()+"Axis" , GetErrMsg(iRet));
            }

            ;
            if (EMCL.ERAETech_EMCL_Sync_SetMaxPositionSpeed(m_iPortID, (byte)iERARRPhysicalNo, (int)_dVel) != (int)EMCL.ReplyCode.EMCL_NO_ERROR)
            {
                //sunsun
                //Log.ShowMessage("EMCL_"+Para.iPhysicalNo.ToString()+"Axis" , GetErrMsg(iRet));
            }

            ;
            if (EMCL.ERAETech_EMCL_Sync_MoveToPosition(m_iPortID, (byte)iERARRPhysicalNo, 0, (int)_dPos) != (int)EMCL.ReplyCode.EMCL_NO_ERROR)
            {
                //sunsun
                //Log.ShowMessage("EMCL_"+Para.iPhysicalNo.ToString()+"Axis" , GetErrMsg(iRet));
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
        public void ApplyPara()
        {
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

        /// <summary>
        /// 홈이 지원 안되는 보드 같은경우 돌려주고 Update 함수 내부에서 처리 해야 한다.
        /// 파익스는 네트워크 타입이라 너무 느려서 업데이트에서 한번만 스캔하여 담아두고 
        /// </summary>
        public void Update()
        {    
            if(Para.iPhysicalNo != 1) return ;

            int iPos  = 0 ;

            int iRet = 0;

            int iRetText = 0;

            //ERARTech Address Start No = 1;
            int iERARRPhysicalNo = 0;
            

            for (int i = 0; i < m_iMaxMotor; i++)
            {
                iERARRPhysicalNo = i + 1;
                EMCL.ERAETech_EMCL_Sync_GetAllStatusEx2(m_iPortID, (byte)iERARRPhysicalNo, ref iRet, ref MotorStat[i]);

                //EMCL.ERAETech_EMCL_Sync_GetAllStatusEx2(m_iPortID, (byte)iERARRPhysicalNo, ref iRet, ref MotorStat[i]);
                


                //EMCL.ERAETech_EMCL_GetRefSearchStatus(m_iPortID, (byte)i);

                //int nHomeStatus;
                //int nRightLimitStatus;
                //int nLeftLimitStatus;
                //int nIsServoOn;
                //int nInPos;
                //int nIsBusy;

                iRetText = EMCL.ERAETech_EMCL_Sync_GetEncoderPos(m_iPortID, (byte)iERARRPhysicalNo, ref iPos);

                if(iRetText != 0)
                {
                    iRet = 0;
                }
                    
                MotorEncPos[i] = (double)iPos;

                iRetText = EMCL.ERAETech_EMCL_Sync_GetActualPos(m_iPortID, (byte)iERARRPhysicalNo, ref iPos);
                if (iRetText != 0)
                {
                    iRet = 0;
                }
                MotorCmdPos[i] = (double)iPos;

                //EMCL.ERAETech_EMCL_Sync_GetEncoderPos(m_iPortID, (byte)iERARRPhysicalNo, ref iPos);
                //MotorEncPos[i] = (double)iPos;

                //EMCL.ERAETech_EMCL_Sync_GetActualPos(m_iPortID, (byte)iERARRPhysicalNo, ref iPos);
                //MotorCmdPos[i] = (double)iPos;

                EMCL.ERAETech_EMCL_Sync_GetAlarm(m_iPortID, (byte)iERARRPhysicalNo, ref iRet);
                Alarm[i] = iRet != 0;
                //if (Alarm[i] == true)
                //{
                //    Log.ShowMessage("Test", "Alarm");
                //}

                EMCL.ERAETech_EMCL_Sync_IsPositionReached(m_iPortID, (byte)iERARRPhysicalNo, ref iRet);
                Inposition[i] = iRet != 0;
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
