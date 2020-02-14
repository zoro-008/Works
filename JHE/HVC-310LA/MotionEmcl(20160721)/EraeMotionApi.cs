using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

/************************************************************************
*	Below describes Axis Command Ids and Values and APIs
*	Please see firmware specification for details
************************************************************************/
namespace EraeMotionApi
{
   public class EMCL
   {
      const string EMCL_API_VERSION		   	= ("2016.07.05");
      const int EMCL_MAX_TIMEOUT				   = 100;			// 수신타임아웃 msec

      /************************************************************************
      *	Max speed, position value
      ************************************************************************/
      const int EMCL_MAX_ACCELERATION			= 33554431;		//
      const int EMCL_MAX_VELOCITY				= 268435454;	// 최대음수값은 -(268435454+1)
      const int EMCL_MAX_POSITION				= 2147483647;	// 최대음수값은 -(2147483647+1)

      const int EMCL_RIGHT	   = 1;							// Right
      const int EMCL_LEFT		= 2;							// Left

      /************************************************************************
      *	Motion commands
      ************************************************************************/
      public enum MotorCmd
      {
         EMCL_ROR 							= 1,								// ROtate Right
         EMCL_ROL 							= 2,								// ROtate Left
         EMCL_MST 							= 3,								// Motor Stop
         EMCL_MVP 							= 4,								// Move to Position
         EMCL_SAP 							= 5,								// Set Axis Param
         EMCL_GAP 							= 6,								// Get Axis Param
         EMCL_STAP							= 7,								// STore Axis Param
         EMCL_RSAP							= 8,								// ReStore Axis Param
         EMCL_SGP 							= 9,								// Set Global Param
         EMCL_GGP 							= 10,								// Get Global Param
         EMCL_STGP							= 11,								// STore Global Param
         EMCL_RSGP							= 12,								// ReStore Global Param
         EMCL_RFS 							= 13,								// Reference Search
         EMCL_SIO 							= 14,								// Set digital output
         EMCL_GIO 							= 15,								// Get Input
         EMCL_SCO 							= 30,								// Set Coordinate
         EMCL_GCO 							= 31,								// Get Coordinate
         EMCL_CCO 							= 32,								// Capture Coordinate
         EMCL_CALX							= 33,
         EMCL_AAP						   	= 34,
         EMCL_AGP						   	= 35,
         EMCL_VECT							= 37,
         EMCL_RETI							= 38,
         EMCL_ACO						   	= 39,
         EMCL_UF0					   		= 64,
         EMCL_UF1					   		= 65,
         EMCL_UF2					   		= 66,
         EMCL_UF3					   		= 67,
         EMCL_VERSION						= 136,							// module version
      }

      /************************************************************************
      *	Axis param, see firmware specification for details
      ************************************************************************/
      public enum AxisParam
      {
         AXIS_PARAM_TARGET_POS			= 0,
         AXIS_PARAM_ACTUAL_POS			= 1,
         AXIS_PARAM_TARGET_SPEED			= 2,
         AXIS_PARAM_ACTUAL_SPEED			= 3,
         AXIS_PARAM_MAXPOS_SPEED			= 4,
         AXIS_PARAM_MAX_ACCEL				= 5,
         AXIS_PARAM_MAX_CURRENT			= 6,
         AXIS_PARAM_POS_REACHED			= 8,
         AXIS_PARAM_HOME_SWITCH_STATUS	= 9,
         AXIS_PARAM_RIGHT_SWITCH_STATUS	= 10,
         AXIS_PARAM_LEFT_SWITCH_STATUS	   = 11,
         AXIS_PARAM_CL_GAMMA_VMIN			= 108,
         AXIS_PARAM_CL_GAMMA_VMAX			= 109,
         AXIS_PARAM_CL_MAX_GAMMA			   = 110,
         AXIS_PARAM_CL_BETA					= 111,
         AXIS_PARAM_CL_OFFSET				   = 112,
         AXIS_PARAM_CL_CURRENT_MIN	   	= 113,
         AXIS_PARAM_CL_CURRENT_MAX	   	= 114,
         AXIS_PARAM_CL_CORRECTION_VELOCIY_P			= 115,
         AXIS_PARAM_CL_CORRECTION_VELOCIY_I			= 116,
         AXIS_PARAM_CL_CORRECTION_VELOCIY_I_CLIP	= 117,
         AXIS_PARAM_CL_CORRECTION_VELOCIY_DV_CLOCK	= 118,
         AXIS_PARAM_CL_CORRECTION_VELOCIY_DV_CLIP	= 119,
         AXIS_PARAM_CL_UPSCALE_DELAY	= 120,
         AXIS_PARAM_CL_DOWNSCALE_DELAY	= 121,
         AXIS_PARAM_CL_STARTUP			= 126,
         AXIS_PARAM_POSITIONING_WINDOW	= 134,
         AXIS_PARAM_ACTUAL_CURRENT		= 180,
         AXIS_PARAM_ACTUAL_LOAD			= 206,
         AXIS_PARAM_ENCODER_POS			= 209,
         AXIS_PARAM_ABS_ENCODER_POS		= 215,
         AXIS_PARAM_GET_ALL_STATUS		= 250,
     }

      /************************************************************************
      *	global param, see firmware specification for details
      ************************************************************************/
      public enum GlobalParam
      {
         GLOBAL_PARAM_BAUDRATE			 = 65,
         GLOBAL_PARAM_SERIAL_ADDR		 = 66,
      }

      public enum ReplyCode
      {
         /************************************************************************
         *	error from module(firmware)
         ************************************************************************/
         EMCL_NO_ERROR						= 100,		//아무런 오류 없이 성공적으로 실행
         EMCL_ERR_CMD_FULL					= 101,	//EMCL 프로그램 EEPROM 에 명령이 가득함
         EMCL_ERR_BADCHECKSUM				= 1,			//잘못된 checksum
         EMCL_ERR_INVALID_COMMAND		= 2,			//잘못된 명령
         EMCL_ERR_INVALID_TYPE			= 3,			//잘못된 유형
         EMCL_ERR_INVALID_VALUE			= 4,			//잘못된 값
         EMCL_ERR_EEPROM_LOCKED			= 5,			//EEPROM 구성이 잠김
         EMCL_ERR_UNKNOWN_COMMAND		= 6,			//사용할 수 없는 명령

         /************************************************************************
         *	error from APIs
         ************************************************************************/
         EMCL_ERR_COMM_CLOSED				= (-1),		// 포트 닫혀있음
         EMCL_ERR_DATA_NOTREADY			= (-2),		// 데이터 대기중
         EMCL_ERR_COMM_TIMEOUT			= (-3),		// 데이터 수신 타임아웃
         EMCL_ERR_PARAM_OUTOFRANGE		= (-4),		// 변수값 범위 오류
      }

      /************************************************************************
      *	Status structure
      ************************************************************************/
      public struct MotorStatus
      {
	     public int nInput0;	// input 0, 1, 2
         public int nInput1;
         public int nInput2;
         public int nInPosition;
         public int nIsBusy;
         public int nAlarm;
         public int nActualPos;
         public int nActualSpeed;
         public int nIsServoOn;
         public int nHomeStatus;
         public int nRightLimitStatus;
         public int nLeftLimitStatus;
         public int nCurrent;				// 2016.07.21
         public int nLoad;
         public int nPosError;
      }

      public struct MotorStatus2
      {
	      public int nInput0;	// input 0, 1, 2
	      public int nInput1;
	      public int nInput2;
	      public int nOutput0;	// Output 0, 1, 2
	      public int nOutput1;
	      public int nOutput2;
	      public int nHomeStatus;
	      public int nRightLimitStatus;
	      public int nLeftLimitStatus;
	      public int nIsServoOn;
	      public int nPosReached;	
	      public int nIsBusy;
      }


      /************************************************************************
      *	Serial Comm APIs
      ************************************************************************/
      [DllImport("EraeMotionApi.dll")] extern public static bool ERAETech_EMCL_OpenComm(int nPortNum, Int32 dwBaudRate);
      [DllImport("EraeMotionApi.dll")] extern public static void ERAETech_EMCL_CloseComm(int nPortNum);
      [DllImport("EraeMotionApi.dll")] extern public static bool ERAETech_EMCL_IsPortOpen(int nPortNum);
      [DllImport("EraeMotionApi.dll")] extern public static void ERAETech_EMCL_SetTimeout(int nPortNum, Int32 dwWaitTime);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_GetNodeCount(int nPortNum, Int32 nMaxModuleId);
      [DllImport("EraeMotionApi.dll")] extern public static IntPtr ERAETech_EMCL_GetFirmwareVersion(int nPortNum, byte nModuleId);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_IsModuleAlive(int nPortNum, byte nModuleId);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_GetQueueCount(int nPortNum);
      [DllImport("EraeMotionApi.dll")] extern public static void ERAETech_EMCL_EmptyQueue(int nPortNum);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_EnableTypeField(int nPortNum, byte nModuleId, byte bOnOff);

      // Synchronous functions
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_IsModuleAlive(int nPortNum, byte nModuleId, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_EnableTypeField(int nPortNum, byte nModuleId, byte bOnOff, ref Int32 nRetValue);

      /************************************************************************
      *	general command
      ************************************************************************/
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_SendCommand(int nPortNum, byte nModuleId, byte nCmd, byte nType, byte nMotorId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_GetReply(int nPortNum, ref Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_GetReply3(int nPortNum, ref byte nModuleId, ref byte nCmd, ref Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_GetReply4(int nPortNum, ref byte nModuleId, ref byte nCmd, ref Int32 nValue, ref byte nType);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_WaitAndGetReply(int nPortNum, ref Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_WaitAndGetReply3(int nPortNum, ref byte nModuleId, ref byte nCmd, ref Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_WaitAndGetReply4(int nPortNum, ref byte nModuleId, ref byte nCmd, ref Int32 nValue, ref byte nType);
      [DllImport("EraeMotionApi.dll")] extern public static IntPtr ERAETech_EMCL_GetSendPacket(int nPortNum);
      [DllImport("EraeMotionApi.dll")] extern public static IntPtr ERAETech_EMCL_GetRecvPacket(int nPortNum);

      // Synchronous functions
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_SendCommand(int nPortNum, byte nModuleId, byte nCmd, byte nType, byte nMotorId, Int32 nValue, ref Int32 nRetValue);

      /************************************************************************
      *	set/get axis, global params
      ************************************************************************/
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_SetAxisParam(int nPortNum, byte nModuleId, byte nType, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_GetAxisParam(int nPortNum, byte nModuleId, byte nType);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_StoreAxisParam(int nPortNum, byte nModuleId, byte nType);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_RestoreAxisParam(int nPortNum, byte nModuleId, byte nType);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_SetGlobalParam(int nPortNum, byte nModuleId, byte nType, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_GetGlobalParam(int nPortNum, byte nModuleId, byte nType);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_StoreGlobalParam(int nPortNum, byte nModuleId, byte nType);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_RestoreGlobalParam(int nPortNum, byte nModuleId, byte nType);

      // Synchronous functions
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_SetAxisParam(int nPortNum, byte nModuleId, byte nType, Int32 nValue, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_GetAxisParam(int nPortNum, byte nModuleId, byte nType, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_StoreAxisParam(int nPortNum, byte nModuleId, byte nType, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_RestoreAxisParam(int nPortNum, byte nModuleId, byte nType, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_SetGlobalParam(int nPortNum, byte nModuleId, byte nType, Int32 nValue, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_GetGlobalParam(int nPortNum, byte nModuleId, byte nType, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_SetGlobalParamB(int nPortNum, byte nModuleId, byte nType, byte nBank, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_GetGlobalParamB(int nPortNum, byte nModuleId, byte nType, byte nBank, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_StoreGlobalParam(int nPortNum, byte nModuleId, byte nType, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_RestoreGlobalParam(int nPortNum, byte nModuleId, byte nType, ref Int32 nRetValue);

      /************************************************************************
      *	resoultion
      ************************************************************************/
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_SetMicroStep(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_GetMicroStep(int nPortNum, byte nModuleId);

      // Synchronous functions
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_SetMicroStep(int nPortNum, byte nModuleId, Int32 nValue, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_GetMicroStep(int nPortNum, byte nModuleId, ref Int32 nRetValue);

      /************************************************************************
      *	move, rotate command
      ************************************************************************/
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_SetZeroPosition(int nPortNum, byte nModuleId);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_SetEncoderZeroPosition(int nPortNum, byte nModuleId);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_SetMaxPositionSpeed(int nPortNum, byte nModuleId, Int32 nAccel);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_SetMaxAcceleration(int nPortNum, byte nModuleId, Int32 nAccel);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Rotate(int nPortNum, byte nModuleId, int nDir, Int32 nVelocity);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_RotateRight(int nPortNum, byte nModuleId, Int32 nVelocity);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_RotateLeft(int nPortNum, byte nModuleId, Int32 nVelocity);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Stop(int nPortNum, byte nModuleId);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_MoveToPosition(int nPortNum, byte nModuleId, byte nMoveMode, Int32 nPosition);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_MoveAbsolute(int nPortNum, byte nModuleId, Int32 nPosition);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_MoveRelative(int nPortNum, byte nModuleId, Int32 nPosition);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_MoveCoordinate(int nPortNum, byte nModuleId, Int32 nCoordNum);

      // Synchronous functions
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_SetZeroPosition(int nPortNum, byte nModuleId, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_SetEncoderZeroPosition(int nPortNum, byte nModuleId, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_SetMaxPositionSpeed(int nPortNum, byte nModuleId, Int32 nAccel);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_SetMaxAcceleration(int nPortNum, byte nModuleId, Int32 nAccel);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_Rotate(int nPortNum, byte nModuleId, int nDir, Int32 nVelocity);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_RotateRight(int nPortNum, byte nModuleId, Int32 nVelocity);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_RotateLeft(int nPortNum, byte nModuleId, Int32 nVelocity);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_Stop(int nPortNum, byte nModuleId);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_MoveToPosition(int nPortNum, byte nModuleId, byte nMoveMode, Int32 nPosition);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_MoveAbsolute(int nPortNum, byte nModuleId, Int32 nPosition);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_MoveRelative(int nPortNum, byte nModuleId, Int32 nPosition);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_MoveCoordinate(int nPortNum, byte nModuleId, Int32 nCoordNum);

      /************************************************************************
      *	get position, status
      ************************************************************************/
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_IsMoving(int nPortNum, byte nModuleId);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_IsPositionReached(int nPortNum, byte nModuleId);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_GetActualPos(int nPortNum, byte nModuleId);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_GetEncoderPos(int nPortNum, byte nModuleId);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_GetActualCurrent(int nPortNum, byte nModuleId);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_GetHomeSwitchStatus(int nPortNum, byte nModuleId);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_GetRightLimitStatus(int nPortNum, byte nModuleId);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_GetLeftLimitStatus(int nPortNum, byte nModuleId);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_GetActualSpeed(int nPortNum, byte nModuleId);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_GetActualLoad(int nPortNum, byte nModuleId);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_GetAllStatus(int nPortNum, byte nModuleId, ref MotorStatus moStatus);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_GetAllStatus2(int nPortNum, byte nModuleId, ref MotorStatus moReqField, ref MotorStatus moRetStatus);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_GetAllStatusEx(int nPortNum, byte nModuleId);

      // Synchronous functions
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_IsMoving(int nPortNum, byte nModuleId, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_IsPositionReached(int nPortNum, byte nModuleId, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_GetActualPos(int nPortNum, byte nModuleId, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_GetEncoderPos(int nPortNum, byte nModuleId, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_GetActualCurrent(int nPortNum, byte nModuleId, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_GetHomeSwitchStatus(int nPortNum, byte nModuleId, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_GetRightLimitStatus(int nPortNum, byte nModuleId, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_GetLeftLimitStatus(int nPortNum, byte nModuleId, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_GetActualSpeed(int nPortNum, byte nModuleId, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_GetActualLoad(int nPortNum, byte nModuleId, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_GetAllStatusEx(int nPortNum, byte nModuleId, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_GetAllStatusEx2(int nPortNum, byte nModuleId, ref Int32 nRetValue, ref MotorStatus2 moStatus2);

      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_GetModuleError(int nPortNum, byte nModuleId, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_GetAlarm(int nPortNum, byte nModuleId, ref Int32 nRetValue);
      /************************************************************************
      *	io set, get
      ************************************************************************/
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_SetDigitalOutput(int nPortNum, byte nModuleId, byte nOutPort, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_GetDigitalOutput(int nPortNum, byte nModuleId, byte nOutPort);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_GetDigitalInput(int nPortNum, byte nModuleId, byte nInport);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_GetAnalogInput(int nPortNum, byte nModuleId, byte nInport);

      // Synchronous functions
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_SetDigitalOutput(int nPortNum, byte nModuleId, byte nOutPort, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_GetDigitalOutput(int nPortNum, byte nModuleId, byte nOutPort, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_GetDigitalInput(int nPortNum, byte nModuleId, byte nInport, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_GetAnalogInput(int nPortNum, byte nModuleId, byte nInport, ref Int32 nRetValue);

      /************************************************************************
      *	coordinate set,get,capture
      ************************************************************************/
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_SetCoordinate(int nPortNum, byte nModuleId, byte nCoordNum, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_GetCoordinate(int nPortNum, byte nModuleId, byte nCoordNum);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_CaptureCoordinate(int nPortNum, byte nModuleId, byte nCoordNum);

      // Synchronous functions
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_SetCoordinate(int nPortNum, byte nModuleId, byte nCoordNum, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_GetCoordinate(int nPortNum, byte nModuleId, byte nCoordNum, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_CaptureCoordinate(int nPortNum, byte nModuleId, byte nCoordNum);

      /************************************************************************
      *	reference
      ************************************************************************/
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_SetSearchMode(int nPortNum, byte nModuleId, Int32 nMode);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_SetRefSearchSpeed(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_SetRefSwitchSpeed(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_StartRefSearch(int nPortNum, byte nModuleId);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_StopRefSearch(int nPortNum, byte nModuleId);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_GetRefSearchStatus(int nPortNum, byte nModuleId);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_DisableRightSwitchStop(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_DisableLeftSwitchStop(int nPortNum, byte nModuleId, Int32 nValue);

      // Synchronous functions
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_SetSearchMode(int nPortNum, byte nModuleId, Int32 nMode);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_SetRefSearchSpeed(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_SetRefSwitchSpeed(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_StartRefSearch(int nPortNum, byte nModuleId);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_StopRefSearch(int nPortNum, byte nModuleId);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_GetRefSearchStatus(int nPortNum, byte nModuleId, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_DisableRightSwitchStop(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_DisableLeftSwitchStop(int nPortNum, byte nModuleId, Int32 nValue);

      /************************************************************************
      *	deviation value
      ************************************************************************/
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_SetEncoderDeviation(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_GetEncoderDeviation(int nPortNum, byte nModuleId);

      // Synchronous functions
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_SetEncoderDeviation(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_GetEncoderDeviation(int nPortNum, byte nModuleId, ref Int32 nRetValue);

      /************************************************************************
      *	CL tuning
      ************************************************************************/
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_SetMaxCurrent(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_GetMaxCurrent(int nPortNum, byte nModuleId);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_SetCLGammaVMin(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_GetCLGammaVMin(int nPortNum, byte nModuleId);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_SetCLGammaVMax(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_GetCLGammaVMax(int nPortNum, byte nModuleId);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_SetCLMaxGamma(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_GetCLMaxGamma(int nPortNum, byte nModuleId);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_SetCLBeta(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_GetCLBeta(int nPortNum, byte nModuleId);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_SetCLOffset(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_GetCLOffset(int nPortNum, byte nModuleId);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_SetCLCurrentMin(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_GetCLCurrentMin(int nPortNum, byte nModuleId);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_SetCLCurrentMax(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_GetCLCurrentMax(int nPortNum, byte nModuleId);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_SetCLCorrectionVelocityP(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_GetCLCorrectionVelocityP(int nPortNum, byte nModuleId);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_SetCLCorrectionVelocityI(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_GetCLCorrectionVelocityI(int nPortNum, byte nModuleId);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_SetCLCorrectionVelocityIClip(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_GetCLCorrectionVelocityIClip(int nPortNum, byte nModuleId);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_SetCLCorrectionVelocityDvClock(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_GetCLCorrectionVelocityDvClock(int nPortNum, byte nModuleId);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_SetCLCorrectionVelocityDvClip(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_GetCLCorrectionVelocityDvClip(int nPortNum, byte nModuleId);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_SetCLPositioningWindow(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_GetCLPositioningWindow(int nPortNum, byte nModuleId);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_SetCLStartup(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_GetCLStartup(int nPortNum, byte nModuleId);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_SetCLUpScaleDelay(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_GetCLUpScaleDelay(int nPortNum, byte nModuleId);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_SetCLDownScaleDelay(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_GetCLDownScaleDelay(int nPortNum, byte nModuleId);

      // Synchronous functions
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_SetMaxCurrent(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_GetMaxCurrent(int nPortNum, byte nModuleId, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_SetCLGammaVMin(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_GetCLGammaVMin(int nPortNum, byte nModuleId, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_SetCLGammaVMax(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_GetCLGammaVMax(int nPortNum, byte nModuleId, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_SetCLMaxGamma(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_GetCLMaxGamma(int nPortNum, byte nModuleId, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_SetCLBeta(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_GetCLBeta(int nPortNum, byte nModuleId, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_SetCLOffset(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_GetCLOffset(int nPortNum, byte nModuleId, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_SetCLCurrentMin(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_GetCLCurrentMin(int nPortNum, byte nModuleId, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_SetCLCurrentMax(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_GetCLCurrentMax(int nPortNum, byte nModuleId, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_SetCLCorrectionVelocityP(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_GetCLCorrectionVelocityP(int nPortNum, byte nModuleId, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_SetCLCorrectionVelocityI(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_GetCLCorrectionVelocityI(int nPortNum, byte nModuleId, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_SetCLCorrectionVelocityIClip(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_GetCLCorrectionVelocityIClip(int nPortNum, byte nModuleId, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_SetCLCorrectionVelocityDvClock(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_GetCLCorrectionVelocityDvClock(int nPortNum, byte nModuleId, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_SetCLCorrectionVelocityDvClip(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_GetCLCorrectionVelocityDvClip(int nPortNum, byte nModuleId, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_SetCLPositioningWindow(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_GetCLPositioningWindow(int nPortNum, byte nModuleId, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_SetCLStartup(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_GetCLStartup(int nPortNum, byte nModuleId, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_SetCLUpScaleDelay(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_GetCLUpScaleDelay(int nPortNum, byte nModuleId, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_SetCLDownScaleDelay(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_GetCLDownScaleDelay(int nPortNum, byte nModuleId, ref Int32 nRetValue);

      /************************************************************************
      *	etc
      ************************************************************************/
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_SetFreeWheeling(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_GetFreeWheeling(int nPortNum, byte nModuleId);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_ResetAlarm(int nPortNum, byte nModuleId);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_GetAlarmCode(int nPortNum, byte nModuleId);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_ResetPosError(int nPortNum, byte nModuleId);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_ResetError(int nPortNum, byte nModuleId, Int32 nErrorType);	// 1:alarm, 4: pos Err,  5: all
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_SetServoOn(int nPortNum, byte nModuleId, Int32 nOnOff);

      // Synchronous functions
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_SetFreeWheeling(int nPortNum, byte nModuleId, Int32 nValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_GetFreeWheeling(int nPortNum, byte nModuleId, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_ResetAlarm(int nPortNum, byte nModuleId);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_GetAlarmCode(int nPortNum, byte nModuleId, ref Int32 nRetValue);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_ResetPosError(int nPortNum, byte nModuleId);
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_ResetError(int nPortNum, byte nModuleId, Int32 nErrorType);	// 1:alarm, 4: pos Err,  5: all
      [DllImport("EraeMotionApi.dll")] extern public static int ERAETech_EMCL_Sync_SetServoOn(int nPortNum, byte nModuleId, Int32 nOnOff);

   }
}
