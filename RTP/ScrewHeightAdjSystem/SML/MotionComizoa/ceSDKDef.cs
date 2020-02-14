using System;
using System.Text;

namespace Shared
{
    public sealed class Defines
    {
			//////////////////////////////////////////////////////////////////////////
			// General Defines
			//////////////////////////////////////////////////////////////////////////
			public const int CE_TRUE	=1;
			public const int CE_FALSE =0;
			
			//////////////////////////////////////////////////////////////////////////
			// cnGnResetNode
			//////////////////////////////////////////////////////////////////////////
			public const int CE_RESET_DIO			=0x00000001;
			public const int CE_RESET_MOTION			=0x00000002;
			public const int CE_RESET_AIO			=0x00000004;
			public const int CE_RESET_ALL			=CE_RESET_DIO | CE_RESET_MOTION | CE_RESET_AIO;
			public const int CE_RESET_CFG_RESTORE	=0x00010000;
			
			//////////////////////////////////////////////////////////////////////////
			// General Error Codes
			//////////////////////////////////////////////////////////////////////////
			public const int ceERR_NONE							   =0;
			
			public const int ceGnERR_TIMEOUT                     =-101; // communication timeout error
			public const int ceGnERR_INVALID_PACKET              =-102; // Packet data error
			public const int ceGnERR_CHECKSUM                    =-103; // checksum mismatch
			public const int ceGnERR_FLASH_ERASE_FAIL            =-104; // fali to erase flash-memory
			public const int ceGnERR_UNDEFINE_COMMAND            =-105; // Undefined control command has been received
			
			//////////////////////////////////////////////////////////////////////////
			// Motion API Error Codes
			//////////////////////////////////////////////////////////////////////////
			public const int cemERR_MEM_ALLOC_FAIL  =-290; // Memory allocation fail
			public const int cemERR_GLOBAL_MEM_FAIL  =-292; // Global memory allocation fail
			public const int cemERR_ISR_CONNEC_FAIL   =-310; // ISR registration fail
			public const int cemERR_DIVIDE_BY_ZERO  =-400; // Cause divide by zero error 
			public const int cemERR_WORNG_NUM_DATA  =-500; // Number of data is too small or too big
			public const int cemERR_VER_MISMATCH   =-600;  // Version(of file or device) mismatch
			
			public const int cemERR_INVALID_DEVICE_ID  =-1010; // Invalid device id => Load Device 또는 SetDeviceId()에서...
			public const int cemERR_INVALID_HANDLE  =-1020;
			public const int cemERR_UNSUPORTED_FUNC  =-1030 ;
			public const int cemERR_INVALID_PARAMETER  =-1101;
			public const int cemERR_INVALID_CHANNEL  =-1105;
			public const int cemERR_INVALID_INPUT_RANGE =-1111; // Invalid range value (AI, AO)
			public const int cemERR_INVALID_FREQ_RANGE =-1121; // Invalid input or output frequency
			public const int cemERR_FILE_CREATE_FAIL  =-1501; // File create fail 
			public const int cemERR_FILE_OPEN_FAIL  =-1511; // File open fail
			public const int cemERR_FILE_READ_FAIL  =-1522; // File reading fail
			public const int cemERR_EVENT_CREATE_FAIL  =-1550; // Event handle creation fail   
			public const int cemERR_INT_INSTANCE_FAIL  =-1560; // Interrupt event instance creation fail
			public const int cemERR_DITHREAD_CRE   =-1570 ;// D/I state change monitor thread creation fail
			public const int cemERR_BUFFER_SMALL   =-1580; // Buffer size is too small
			public const int cemERR_HIGH_TIMER_UNSUPP  =-1590; // The installed hardware does not support a high-resolution performance counter (cemmUtlDelayMicroSec() function fails)
			public const int cemERR_OUT_OF_RANGE   =-1600; // The range of some parameter is out of range
			
			public const int cemERR_ON_MOTION    =-5001;
			public const int cemERR_STOP_BY_SLP   =-5002; // Abnormally stopped by positive soft limit
			public const int cemERR_STOP_BY_SLN   =-5003; // Abnormally stopped by negative soft limit
			public const int cemERR_STOP_BY_CMP3   =-5004; // Abnormally stopped by comparator3
			public const int cemERR_STOP_BY_CMP4   =-5005; // Abnormally stopped by comparator4
			public const int cemERR_STOP_BY_CMP5   =-5006; // Abnormally stopped by comparator5
			public const int cemERR_STOP_BY_ELP   =-5007; // Abnormally stopped by (+) external limit
			public const int cemERR_STOP_BY_ELN   =-5008; // Abnormally stopped by (-) external limit
			public const int cemERR_STOP_BY_ALM   =-5009; // Abnormally stopped by alarm input signal
			public const int cemERR_STOP_BY_CSTP   =-5010; // Abnormally stopped by CSTP input signal
			public const int cemERR_STOP_BY_CEMG   =-5011; // Abnormally stopped by CEMG input signal
			public const int cemERR_STOP_BY_SD   =-5012; // Abnormally stopped by SD input signal
			public const int cemERR_STOP_BY_DERROR  =-5013; // Abnormally stopped by operation data error
			public const int cemERR_STOP_BY_IP   =-5014; // Abnormally stopped by other axis error during interpolation
			public const int cemERR_STOP_BY_PO   =-5015; // An overflow occurred in the PA/PB input buffer
			public const int cemERR_STOP_BY_AO   =-5016; // Out of range position counter during interpolation
			public const int cemERR_STOP_BY_EE   =-5017; // An EA/EB input error occurred (does not stop)
			public const int cemERR_STOP_BY_PE   =-5018; // An PA/PB input error occurred (does not stop)
			public const int cemERR_STOP_BY_SLVERR  =-5019; // Abnormally stopped because slave axis has been stopped
			public const int cemERR_STOP_BY_SEMG   =-5020; // Abnormally stopped by software emergency setting
			
			public const int cemERR_MOT_MAOMODE			=-5110; // Master output mode is not CW/CCW mode // Master/Slave 동작시에 Master output모드가 CW/CCW모드가 아니다.
			public const int cemERR_MOT_SLAVE_SET			=-5120; // Slave start fail (Motion state가 "Wait for Pulsar Input"으로 변하지 않는다.
			public const int cemERR_SPEED_RANGE_OVER		=-5130; // 
			public const int cemERR_INVALID_SPEED_SET		=-5140; // Speed setting value is not valid
			public const int cemERR_ACC_LOW_LIMIT_OVER	=-5142; // Acceleration setting value is too low
			public const int cemERR_ACC_HIGH_LIMIT_OVER	=-5143; // Acceleration setting value is too high
			public const int cemERR_DEC_LOW_LIMIT_OVER	=-5144; // Deceleration setting value is too low
			public const int cemERR_DEC_HIGH_LIMIT_OVER	=-5145; // Deceleration setting value is too high
			public const int cemERR_INVALID_IXMAP			=-5150; // Invalid interpolation map
			public const int cemERR_INVALID_LMMAP			=-5160; // Invalid List-Motion Map 
			public const int cemERR_MOT_SEQ_SKIPPED		=-5170; // Motion command is skipped because the axis is already running.  
			
			//////////////////////////////////////////////////////////////////////////
			// Process Boost Mode & Level
			//////////////////////////////////////////////////////////////////////////
			
			// MODE
			public const int CE_PROCESS_ONLY_BOOST	=0;
			public const int CE_SERVICE_ONLY_BOOST	=1;
			public const int CE_ALL_BOOST			=2;
			
			// LEVEL
			public const int	CE_ABOVE_NORMAL_PRIORITY_CLASS		=0x00008000;
			public const int	CE_BELOW_NORMAL_PRIORITY_CLASS		=0x00004000;
			public const int	CE_HIGH_PRIORITY_CLASS				=0x00000080;
			public const int	CE_IDLE_PRIORITY_CLASS				=0x00000040;
			public const int	CE_NORMAL_PRIORITY_CLASS			=0x00000020;
			public const int	CE_PROCESS_MODE_BACKGROUND_BEGIN	=0x00100000;
			public const int	CE_PROCESS_MODE_BACKGROUND_END		=0x00200000;
			public const int	CE_REALTIME_PRIORITY_CLASS			=0x00000100;
			
			//////////////////////////////////////////////////////////////////////////
			// API Argument or Value Definition
			
			// Motion I/O Property ID //
			// MIO Property ID //
			public enum _TCemMioPropId{
				cemALM_LOGIC, cemALM_MODE, cemCMP_LOGIC, cemDR_LOGIC, cemEL_LOGIC, cemEL_MODE, 
				cemERC_LOGIC, cemERC_OUT, cemEZ_LOGIC, cemINP_EN, cemINP_LOGIC, cemLTC_LOGIC, 
				cemLTC_LTC2SRC, cemORG_LOGIC, cemSD_EN, cemSD_LOGIC, cemSD_LATCH, cemSD_MODE, cemSTA_MODE,
				cemSTA_TRG, cemSTP_MODE, cemCLR_CNTR, cemCLR_SIGTYPE, cemCMP_PWIDTH, cemERC_ONTIME, cemSVON_LOGIC,
			};
			
			public enum _TCemMioPropIdEx{
				cemMPID_ALM_LOGIC,	cemMPID_ALM_MODE,	cemMPID_CMP_LOGIC,	cemMPID_DR_LOGIC,	cemMPID_EL_LOGIC,	cemMPID_EL_MODE, 
				cemMPID_ERC_LOGIC,	cemMPID_ERC_OUT,		cemMPID_EZ_LOGIC,	cemMPID_INP_EN,		cemMPID_INP_LOGIC,	cemMPID_LTC_LOGIC, 
				cemMPID_LTC_LTC2SRC,	cemMPID_ORG_LOGIC,	cemMPID_SD_EN,		cemMPID_SD_LOGIC,	cemMPID_SD_LATCH,	cemMPID_SD_MODE, 
				cemMPID_STA_MODE,	cemMPID_STA_TRG,		cemMPID_STP_MODE,	cemMPID_CLR_CNTR,	cemMPID_CLR_SIGTYPE,	cemMPID_CMP_PWIDTH, 
				cemMPID_ERC_ONTIME,	cemMPID_SVON_LOGIC,	cemMPID_ERC_OUT_EL,	cemMPID_CNT_D_SRC,	cemMPID_CNT_G_SRC,	cemMPID_LTC_TRGMODE,
				cemMPID_SLIM_EN=100,	cemMPID_OUT_MODE,	cemMPID_IN_MODE,		cemMPID_IN_INV,		cemMPID_CEMG_EN
			};
			
			// Bit order of StReadMioStatuses() return value  //
			public enum _TCemMioState{
				cemIOST_RDY, cemIOST_ALM,	  cemIOST_ELN,  cemIOST_ELP, cemIOST_ORG, 
				cemIOST_DIR, cemIOST_EZ,	  cemIOST_LTC,  cemIOST_SD,  cemIOST_INP, 
				cemIOST_DRN, cemIOST_DRP,	  cemIOST_STA,  cemIOST_STP, cemIOST_ALMR,
				cemIOST_EMG, cemIOST_SVON,  cemIOST_HOMS, cemIOST_PLSA
			};
			
			// Motion operation status ID //
			public enum _TCemMotionState{ 
				cemMST_STOP,			cemMST_WAIT_DR,	cemMST_WAIT_STA,		cemMST_WAIT_INSYNC,	cemMST_WAIT_OTHER, 
				cemMST_WAIT_ERC,		cemMST_WAIT_DIR, cemMST_RESERVED1,	cemMST_WAIT_PLSR,	cemMST_IN_RVSSPD, 
				cemMST_IN_INISPD,		cemMST_IN_ACC,	cemMST_IN_WORKSPD,	cemMST_IN_DEC,		cemMST_WAIT_INP, 
				cemMST_SPARE0,			cemMST_HOMMING,
			};
			
			// Signal logic definition //
			public enum _TCemSigLogic{ 
				cemLOGIC_A=0 /*Normal open*/, cemLOGIC_B=1/*Normal close*/ 
			};
			
			// Axis index definition //
			public enum _TCemAxis{ 
				cemX1, cemY1, cemZ1, cemU1, cemX2, cemY2, cemZ2, cemU2 
			};
			
			// Definition for axes mask  //
			public enum _TCemAxisMask{ 
				cemX1_MASK=0x1, cemY1_MASK=0x2, cemZ1_MASK=0x4, cemU1_MASK=0x8,
				cemX2_MASK=0x10, cemY2_MASK=0x20, cemZ2_MASK=0x40, cemU2_MASK=0x80
			};
			
			// Encoder and PA/PB input mode definition //
			public enum _TCemInMode{ 
				cemIMODE_AB1X, cemIMODE_AB2X, cemIMODE_AB4X, cemIMODE_CWCCW, cemIMODE_STEP
			};
			
			// Command output mode definition //
			public enum _TCemOutMode{ 
				cemOMODE_PDIR0, cemOMODE_PDIR1, cemOMODE_PDIR2, cemOMODE_PDIR3,
				cemOMODE_CWCCW0, cemOMODE_CWCCW1, cemOMODE_CCWCW0, cemOMODE_CCWCW1
			};
			
			// Control Mode //
			public enum _TCemCtrlMode{ 
				cemCTRL_OPEN, // Open loop control mode
				cemCTRL_SEMI_C, // Semi-closed loop control mode (applied only to absolute in-position commands)
				cemCTRL_FULL_C // Full-closed loop control mode (this is not supported at current version)
			};
			
			// (Linear)Operation direction //	
			public enum _TCemDir{
				cemDIR_N /*(-)Dir*/, cemDIR_P /*(+)Dir*/
			};
			
			// Counter name //
			public enum _TCemCntr { 
				cemCNT_COMM/*Command*/, cemCNT_FEED/*Feedback*/, cemCNT_DEV/*Deviation*/, 
				cemCNT_GEN/*General*/, cemCNT_REM/*Remained*/
			};
			
			// Speed mode index definition //	
			public enum _TCemSpeedMode{ 
				cemSMODE_KEEP=-1/* Keep previous setting*/, cemSMODE_C=0 /*Constant */, cemSMODE_T /*Trapeziodal*/, cemSMODE_S /*S-curve*/
			};
			
			// Arc operation direction //
			public enum _TCemArcDir{ 
				cemARC_CW, cemARC_CCW
			};
			
			// Compare Method //
			public enum _TCemCmpMethod{
				cemDISABLE, cemEQ_BIDIR, cemEQ_PDIR, cemEQ_NDIR, cemLESS/*Cnt<Data*/, cemGREATER/*Cnt>Data*/
			};
			
			// Action when general comparator met the condition //
			public enum _TCemCmpAction{
				cemEVNT_ONLY, cemEVNT_IS, cemEVNT_DS, cemEVNT_SPDCHG
			};
			
			// Backlash/Slip correction mode //
			public enum _TCemCorrMode{
				cemCORR_DIS, // Disable correction 
				cemCORR_BACK, // Backlash correction mode 
				cemCORR_SLIP // Slip correction mode
			};
			
			// Using for preregister option
			public enum _TCemExtOptionId{
				cemEXOPT_SET_USE_PREREG
			};
			
			// Interrupt Handler Type //
			public enum _TCemIntHandlerType{
				cemIHT_MESSAGE=0, cemIHT_EVENT, cemIHT_CALLBACK
			};
			
			// Interrupt Handler Type //
			public enum _TCemStringID{
				cemSTR_AXIS_NAME, cemSTR_DIST_UNIT, cemSTR_SPEED_UNIT
			};
			
			// Sequence Mode //
			public enum _TCemSeqMode{
				cemSEQM_SKIP_RUN, cemSEQM_WAIT_RUN
			};
			
			// Map Type //
			public enum _TCemDevMapType{
				cemDMAP_MOTION, cemDMAP_DIO, cemDMAP_ALL
			};
			
			// SetFilterAB의 대상 //
			public enum _TCemABFilter{
				cemAB_ENC, cemAB_PULSAR
			};
			
			// Axis Capability ID //
			public enum _TCemAxisCapID{
				cemCAPX_CMD_DIR=0, /* Command direction change function */
				cemCAPX_EL_MAN_SET, /* -/+EL Manual control function */
				cemCAPX_CMP_HIGH /* High-speed Compare Output function */
			};
			
			// Axis Return to home clear mode //
			public enum _TCemHomePosClrMode{
				cemHPCM_DISABLE=-1, // Disable HomePosClearMode
				cemHPCM_M0, // ORG(/EL/EZ) 신호가 발생할 때 COMMAND & FEEDBACK 위치를 0으로 클리어한다.
				cemHPCM_M1, // 원점복귀를 모두 완료하고 나서 COMMAND & FEEDBACK 위치를 모두 0으로 클리어한다.
				cemHPCM_M2  // 원점복귀를 모두 완료하고 나서 FEEDBACK 위치는 그대로 두고 COMMAND 위치를 FEEDBACK 위치에 일치시킨다.
			};
			
			public enum _TCemLatchTrgMode{
				cemLTM_LTC, // LTC 입력 신호에 의해서 포지션 래치가 수행됩니다.
				cemLTM_ORG  // ORG 입력 신호에 의해서 포지션 래치가 수행됩니다.
			};
			
			// Sync mode //
			public enum _TCemSyncMode{
				cemSYNC_DISABLE,
				cemSYNC_INT_SYNC,
				cemSYNC_OTHER_STOP
			};
			
			// Internal sync. conditions //
			public enum _TCemIntSyncCond{
				cemISYNC_ACC_STA, // 0: at start of acceleration
				cemISYNC_ACC_END, // 1: at end of acceleration
				cemISYNC_DEC_STA, // 2: at start of deceleration
				cemISYNC_DEC_END, // 3: at end of deceleration
				cemISYNC_SLN, // 4: when (-)software limit met
				cemISYNC_SLP, // 5: when (+)software limit met
				cemISYNC_GCMP, // 6: when General Comparator condition is satisfied
				cemISYNC_TCMP // 7: when Trigger Comparator condition is satisfied
			};
			
			// DIO Mode //
			public enum _TCemDioMode{
				cemDIOMODE_IN /*Input Mode*/, cemDIOMODE_OUT /*Output Mode*/
			};              
			
			//////////////////////////////////////////////////////////////////////////
			// Basic Node Information 
			//////////////////////////////////////////////////////////////////////////
			public const int MAX_NODE				=255;
			public const int MAX_MOT_MODULE			=10;
			public const int MAX_DIO_MODULE			=10;
			public const int MAX_AI_MODULE			=10;
			public const int MAX_AO_MODULE			=10;
			public const int MAX_MDIO_MODULE		=10;
			public const int MAX_CNT_MODULE			=10;
			
			//////////////////////////////////////////////////////////////////////////
			// Sub Module Type Information 
			//////////////////////////////////////////////////////////////////////////
			public const int MODULE_DI32_ONLY_TYPE  =0x0;	// DI 32ch 
			public const int MODULE_DO32_ONLY_TYPE  =0x1;  // DO 32ch
			public const int MODULE_MOT_TYPE        =0x2;
			public const int MODULE_AI_TYPE         =0x3;
			public const int MODULE_AO_TYPE         =0x4;
			public const int MODULE_DIO_TYPE		  =0x5;// DIO bidirectional 16ch
			public const int MODULE_MDIO_TYPE	  = 0x6;
            public const int MODULE_CNT_TYPE = 0x7;	    	

    }
}
