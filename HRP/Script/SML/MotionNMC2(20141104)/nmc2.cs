
using System.Runtime.InteropServices;     // DLL support
namespace Paix_MotionControler
{
    class NMC2
    {
        // NMC2 Equip Type
        public const short NMC2_220S = 0;
        public const short NMC2_420S = 1;
        public const short NMC2_620S = 2;
        public const short NMC2_820S = 3;

        public const short NMC2_220_DIO32 = 4;
        public const short NMC2_220_DIO64 = 5;
        public const short NMC2_420_DIO32 = 6;
        public const short NMC2_420_DIO64 = 7;
        public const short NMC2_820_DIO32 = 8;
        public const short NMC2_820_DIO64 = 9;

        public const short NMC2_DIO32 = 10;
        public const short NMC2_DIO64 = 11;
        public const short NMC2_DIO96 = 12;
        public const short NMC2_DIO128 = 13;

        // NMC2 Enum Type
        public const short EQUIP_TYPE_NMC_2_AXIS = 0x0001;
        public const short EQUIP_TYPE_NMC_4_AXIS = 0x0003;
        public const short EQUIP_TYPE_NMC_6_AXIS = 0x0007;
        public const short EQUIP_TYPE_NMC_8_AXIS = 0x000F;
        // 16/16
        public const short EQUIP_TYPE_NMC_IO_32 = 0x0010;
        // 32/32
        public const short EQUIP_TYPE_NMC_IO_64 = 0x0030;
        // 48/48
        public const short EQUIP_TYPE_NMC_IO_96 = 0x0070;
        // 64/64
        public const short EQUIP_TYPE_NMC_IO_128 = 0x00F0;
        // 80/80
        public const short EQUIP_TYPE_NMC_IO_160 = 0x01F0;
        // 96/96
        public const short EQUIP_TYPE_NMC_IO_192 = 0x03F0;
        // 112/112
        public const short EQUIP_TYPE_NMC_IO_224 = 0x07F0;
        // 128/128
        public const short EQUIP_TYPE_NMC_IO_256 = 0x0FF0;

        public const short EQUIP_TYPE_NMC_IO_IE = 0x1000;
        public const short EQUIP_TYPE_NMC_IO_OE = 0x2000;

        public const short EQUIP_TYPE_NMC_M_IO_8 = 0x4000;

        // 모든 함수의 리턴값 
        public const short NMC_CONTI_BUF_FULL = -15;
        public const short NMC_CONTI_BUF_EMPTY = -14;
        public const short NMC_INTERPOLATION = -13;
        public const short NMC_FILE_LOAD_FAIL = -12;
        public const short NMC_ICMP_LOAD_FAIL = -11;
        public const short NMC_NOT_EXISTS = -10;
        public const short NMC_CMDNO_ERROR = -9;
        public const short NMC_NOTRESPONSE = -8;
        public const short NMC_BUSY = -7;
        public const short NMC_COMMERR = -6;
        public const short NMC_SYNTAXERR = -5;
        public const short NMC_INVALID = -4;
        public const short NMC_UNKOWN = -3;
        public const short NMC_SOCKINITERR = -2;
        public const short NMC_NOTCONNECT = -1;
        public const short NMC_OK = 0;

				// STOP MODE
        public const short NMC_STOP_OK = 0;
        public const short NMC_STOP_EMG = 1;
        public const short NMC_STOP_MLIMIT = 2;
        public const short NMC_STOP_PLIMIT = 3;
        public const short NMC_STOP_ALARM = 4;
        public const short NMC_STOP_NEARORG = 5;
        public const short NMC_STOP_ENCZ = 6;
        
        // HOME MODE
        public const short NMC_HOME_LIMIT_P = 0;
        public const short NMC_HOME_LIMIT_M = 1;
        public const short NMC_HOME_NEAR_P 	= 2;
        public const short NMC_HOME_NEAR_M 	= 3;
        public const short NMC_HOME_Z_P 		= 4;
        public const short NMC_HOME_Z_M 		= 5;
        
        public const short NMC_HOME_USE_Z 	= 0x80;
        
        public const short NMC_END_NONE 		= 0x00;
        public const short NMC_END_CMD_CLEAR_A_OFFSET = 0x01;
        public const short NMC_END_ENC_CLEAR_A_OFFSET = 0x02;
        public const short NMC_END_CMD_CLEAR_B_OFFSET = 0x04;
        public const short NMC_END_ENC_CLEAR_B_OFFSET = 0x08;
        
        // LOG
        public const short NMC_LOG_NONE 			= 0;
        public const short NMC_LOG_DEV 				= 0x01;
        public const short NMC_LOG_MO_MOV 		= 0x02;// 모션함수중 MOVE
        public const short NMC_LOG_MO_SET 		= 0x04;// 모션함수중 GET
        public const short NMC_LOG_MO_GET 		= 0x08;// 모션함수중 SET
        public const short NMC_LOG_MO_EXPRESS = 0x10;// 모션함수중 각종 상태값 읽는(빈번히 발생)
        public const short NMC_LOG_IO_SET 		= 0x20;
        public const short NMC_LOG_IO_GET 		= 0x40;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct NMCPARALOGIC
        {
            public short nEmg;			 // EMG
            public short nEncCount;		// 엔코더 카운트 모드

            public short nEncDir;		// 엔코더 카운트 방향
            public short nEncZ;			// 엔코더 Z

            public short nNear;			// NEAR(HOME)
            public short nMLimit;		// - Limit

            public short nPLimit;		// + Limit
            public short nAlarm;		// Alarm

            public short nInp;			// INPOSITION
            public short nSReady;		// Servo Ready

            public short nPulseMode;	// 1p/2p Mode
            //-------------------------------------------------------------

            public short nLimitStopMode; // Limit stop mode
            public short nBusyOff;		// Busy off mode

            public short nSWEnable;		// sw limit 활성화 여부
            //-------------------------------------------------------------
            public double dSWMLimitPos;
            public double dSWPLimitPos;
        }
        
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct NMCPARASPEED
        {
            public double dStart;
            public double dAcc;
            public double dDec;
            public double dDrive;
            public double dJerkAcc;
            public double dJerkDec;
        };

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct NMCAXESMOTIONOUT
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public short[] nCurrentOn;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public short[] nServoOn;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public short[] nDCCOn;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public short[] nAlarmResetOn;
        };

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct NMCAXESEXPR
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public short[] nBusy;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public short[] nError;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public short[] nNear;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public short[] nPLimit;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public short[] nMLimit;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public short[] nAlarm;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public short[] nEmer;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public short[] nSwPLimit;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public short[] nInpo;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public short[] nHome;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public short[] nEncZ;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public short[] nOrg;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public short[] nSReady;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public short[] nContStatus;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public short[] nDummy;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public short[] nSwMLimit;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public int[] lEnc;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public int[] lCmd;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public double[] dEnc;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public double[] dCmd ;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public char[] dummy;
        };
        
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct NMCSTOPMODE
        {
            // 1 축
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public short[] nEmg;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public short[] nMLimit;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public short[] nPLimit;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public short[] nAlarm;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public short[] nNear;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public short[] nEncZ;
        };
        
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct NMCCONTSTATUS
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public short[] nStatus;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public short[] nExeNodeNo;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct NMCHOMEFLAG
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public short[] nSrchFlag;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public short[] nStatusFlag;
        };

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct NMCEQUIPLIST
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 200)]
            public int[] lIp;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 200)]
            public int[] lModelType;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 200)]
            public short[] nMotionType;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 200)]
            public short[] nDioType;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 200)]
            public short[] nEXDIo ;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 200)]
            public short[] nMDio;

        };
    
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct NMCMAPDATA
		{
			int		nMapCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 52)]
			public	int []lMapData;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 52)]
			public	double	[]dMapData;
		};
		

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct NMCCONTISTATUS
		{
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
			public	short [] nContiRun;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
			public	short [] nContiWait;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
			public	short [] nContiRemainBuffNum;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
			public	ulong [] nContiExecutionNum;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
			public	short [] nDummy;
		};
        //------------------------------------------------------------------------------
        [DllImport("NMC2.dll")]
        public static extern short nmc_OpenDevice(short nNmcNo);
        [DllImport("NMC2.dll")]
        public static extern void nmc_CloseDevice(short nNmcNo);
        //------------------------------------------------------------------------------
        [DllImport("NMC2.dll")]
        public static extern short nmc_GetParaLogic(short nNmcNo, short nAxisNo, out NMCPARALOGIC pLogic);
        [DllImport("NMC2.dll")]
        public static extern short nmc_GetParaSpeed(short nNmcNo, short nAxisNo, out NMCPARASPEED pSpeed);
        [DllImport("NMC2.dll")]
        public static extern short nmc_GetRingCountMode(short nNmcNo, short nAxisNo, out int plMaxPulse, out int plMaxEncoder, out short pnRingMode);
        [DllImport("NMC2.dll")]
        public static extern short nmc_GetParaTargetPos(short nNmcNo, short nAxisNo, out double pTargetPos);
        [DllImport("NMC2.dll")]
        public static extern short nmc_GetDriveAxesSpeed(short nNmcNo, double[] pDrvSpeed);
        [DllImport("NMC2.dll")]
        public static extern short nmc_GetAxesMotionOut(short nNmcNo, out NMCAXESMOTIONOUT pOutStatus);
        //------------------------------------------------------------------------------
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetEmgLogic(short nNmcNo, short nGroupNo, short nLogic);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetPlusLimitLogic(short nNmcNo, short nAxisNo, short nLogic);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetMinusLimitLogic(short nNmcNo, short nAxisNo, short nLogic);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetSWLimitLogic(short nNmcNo, short nAxisNo, short nUse, double dSwMinusPos, double dSwPlusPos);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetSWLimitLogicEx(short nNmcNo, short nAxisNo, short nUse, double dSwMinusPos, double dSwPlusPos, short nOpt);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetAlarmLogic(short nNmcNo, short nAxisNo, short nLogic);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetNearLogic(short nNmcNo, short nAxisNo, short nLogic);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetInPoLogic(short nNmcNo, short nAxisNo, short nLogic);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetSReadyLogic(short nNmcNo, short nAxisNo, short nLogic);

        [DllImport("NMC2.dll")]
        public static extern short nmc_SetEncoderCount(short nNmcNo, short nAxisNo, short nCountMode);

        [DllImport("NMC2.dll")]
        public static extern short nmc_SetEncoderDir(short nNmcNo, short nAxisNo, short nCountDir);

        [DllImport("NMC2.dll")]
        public static extern short nmc_SetEncoderMode(short nNmcNo, short nAxisNo, short nMode);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetEncoderZLogic(short nNmcNo, short nAxisNo, short nLogic);
        //------------------------------------------------------------------------------
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetParaLogic(short nNmcNo, short nAxisNo, out NMCPARALOGIC pLogic);
        [DllImport("NMC2.dll")]
		public static extern short nmc_SetParaLogicFile(short nNmcNo, char[] pStr);
        //------------------------------------------------------------------------------
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetPulseMode(short nNmcNo, short nAxisNo,  short nMode);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetPulseLogic(short nNmcNo, short nAxisNo, short nLogic);
        [DllImport("NMC2.dll")]
        public static extern short nmc_Set2PulseDir(short nNmcNo, short nAxisNo,  short nDir);
        [DllImport("NMC2.dll")]
        public static extern short nmc_Set1PulseDir(short nNmcNo, short nAxisNo,  short nDir);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetPulseActive(short nNmcNo, short nAxisNo,  short nPulseActive);
        //------------------------------------------------------------------------------
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetSCurveSpeed(short nNmcNo, short nAxisNo, double dStartSpeed,
                                        double dAcc, double dDec, double dDriveSpeed);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetSpeed(short nNmcNo, short nAxisNo, double dStartSpeed,
                              double dAcc, double dDec, double dDriveSpeed);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetOverrideRunSpeed(short nNmcNo, short nAxisNo, double dAcc, double dDec, double dDriveSpeed);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetOverrideDriveSpeed(short nNmcNo, short nAxisNo, double dDriveSpeed);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetAccSpeed(short nNmcNo, short nAxisNo, double dAcc);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetDecSpeed(short nNmcNo, short nAxisNo, double dDec);
        //------------------------------------------------------------------------------
        [DllImport("NMC2.dll")]
        public static extern short nmc_AbsMove(short nNmcNo, short nAxisNo, double dPos);
        [DllImport("NMC2.dll")]
        public static extern short nmc_RelMove(short nNmcNo, short nAxisNo, double dAmount);
        [DllImport("NMC2.dll")]
        public static extern short nmc_VelMove(short nNmcNo, short nAxisNo, double dPos, double dDrive, short nMode);

        [DllImport("NMC2.dll")]
        public static extern short nmc_AbsOver(short nNmcNo, short nAxisNo, double dPos);

        [DllImport("NMC2.dll")]
        public static extern short nmc_VarRelMove(short nNmcNo, short nAxisCount, short[] pnAxisNo, double[] pdAmount);
        [DllImport("NMC2.dll")]
        public static extern short nmc_VarAbsMove(short nNmcNo, short nAxisCount, short[] pnAxisNo, double[] pdPosList);
        [DllImport("NMC2.dll")]
        public static extern short nmc_VarAbsOver(short nNmcNo, short nAxisCount, short[] pnAxisNo, double[] pdPosList);
        //------------------------------------------------------------------------------
        [DllImport("NMC2.dll")]
        public static extern short nmc_JogMove(short nNmcNo, short nAxis, short Dnir);
        //------------------------------------------------------------------------------
        [DllImport("NMC2.dll")]
        public static extern short nmc_SuddenStop(short nNmcNo, short nAxisNo);
        [DllImport("NMC2.dll")]
        public static extern short nmc_DecStop(short nNmcNo, short nAxisNo);

        [DllImport("NMC2.dll")]
        public static extern short nmc_AllAxisStop(short nNmcNo, short nMode);
        [DllImport("NMC2.dll")]
        public static extern short nmc_MultiAxisStop(short nNmcNo, short nCount, short[] pnAxisSelect, short nMode);
        //------------------------------------------------------------------------------
        [DllImport("NMC2.dll")]
        public static extern short nmc_GetAxesExpress(short nNmcNo, out NMCAXESEXPR pNmcData);
        [DllImport("NMC2.dll")]
        public static extern short nmc_GetStopInfo(short nNmcNo, short[] pnStopMode);
        //------------------------------------------------------------------------------
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetCmdPos(short nNmcNo, short nAxisNo, double dPos);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetEncPos(short nNmcNo, short nAxisNo, double dPos);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetCmdEncPos(short nNmcNo, short nAxisNo, double dPos);
        //------------------------------------------------------------------------------
        [DllImport("NMC2.dll")]
        public static extern short nmc_HomeMove(short nNmcNo, short nAxisNo, short nHomeMode, short nHomeEndMode, double dOffset, short nReserve);
        [DllImport("NMC2.dll")]
        public static extern short nmc_GetHomeStatus(short nNmcNo, out NMCHOMEFLAG pHomeFlag);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetHomeSpeed(short nNmcNo, short nAxisNo,
                                  double dHomeSpeed0, double dHomeSpeed1, double dHomeSpeed2);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetHomeSpeedEx(short nNmcNo, short nAxisNo,
                                  double dHomeSpeed0, double dHomeSpeed1, double dHomeSpeed2,double dOffsetSpeed);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetHomeDelay(short nNmcNo, int nHomeDelay);
        //------------------------------------------------------------------------------
        [DllImport("NMC2.dll")]
        public static extern short nmc_Interpolation2Axis(short nNmcNo, short nAxisNo0, double dPos0,
                                        short nAxisNo1, double dPos1, short nOpt);
        [DllImport("NMC2.dll")]
        public static extern short nmc_Interpolation3Axis(short nNmcNo, short nAxisNo0, double dPos0,
                short nAxisNo1, double dPos1, short nAxisNo2, double dPos2, short nOpt);
        //------------------------------------------------------------------------------
        [DllImport("NMC2.dll")]
        public static extern short nmc_InterpolationArc(short nNmcNo, short nAxisNo0, short nAxisNo1,
                                      double dCenter0, double dCenter1, double dAngle, short nOpt);

        [DllImport("NMC2.dll")]
        public static extern short nmc_InterpolationRelCircle(short nNmcNo, short nAxisNo0, double CenterPulse0, double EndPulse0,
                short nAxisNo1, double CenterPulse1, double EndPulse1, short nDir);
        [DllImport("NMC2.dll")]
        public static extern short nmc_InterpolationAbsCircle(short nNmcNo, short nAxisNo0, double CenterPulse0, double EndPulse0,
                short nAxisNo1, double CenterPulse1, double EndPulse1, short nDir);

        //------------------------------------------------------------------------------
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetCurrentOn(short nNmcNo, short nAxisNo, short nOut);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetServoOn(short nNmcNo, short nAxisNo, short nOut);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetAlarmResetOn(short nNmcNo, short nAxisNo, short nOut);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetDccOn(short nNmcNo, short nAxisNo, short nOut);
        //------------------------------------------------------------------------------
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetMultiCurrentOn(short nNmcNo, short nCount, short[] pnAxisSelect, short nOut);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetMultiServoOn(short nNmcNo, short nCount, short[] pnAxisSelect, short nOut);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetMultiAlarmOn(short nNmcNo, short nCount, short[] pnAxisSelect, short nOut);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetMultiDccOn(short nNmcNo, short nCount, short[] pnAxisSelect, short nOut);
        //------------------------------------------------------------------------------
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetEnableNear(short nNmcNo, short nAxisNo, short nMode);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetEnableEncZ(short nNmcNo, short nAxisNo, short nMode);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetLimitStopMode(short nNmcNo, short nAxisNo, short nStopMode);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetBusyOffMode(short nNmcNo, short nAxisNo, short nMode);

        //------------------------------------------------------------------------------
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetRingCountMode(short nNmcNo, short nAxisNo, int lMaxPulse, int lMaxEncoder, short nRingMode);
        [DllImport("NMC2.dll")]
        public static extern short nmc_MoveRing(short nNmcNo, short nAxisNo, double dPos, short nMoveMode);
        //------------------------------------------------------------------------------
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetSyncSetup(short nNmcNo, short nMainAxisNo,
                                short nSubAxisNoEnable0, short nSubAxisNoEnable1, short nSubAxisNoEnable2);
        //------------------------------------------------------------------------------
        [DllImport("NMC2.dll")]
		public static extern short nmc_SetSync(short nNmcNo, short nGroupNo, short[] pnSyncGrpList0, short[] pnSyncGrpList1);
        [DllImport("NMC2.dll")]
		public static extern short nmc_SyncFree(short nNmcNo, short nGroupNo);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetMDIOOutPin(short nNmcNo, short nPinNo, short nOutStatus);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetMDIOOutTogPin(short nNmcNo, short nPinNo);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetMDIOOutPins(short nNmcNo, short nCount, short[] pnPinNo, short[] pnStatus);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetMDIOOutTogPins(short nNmcNo, short nCount, short[] pnPinNo);
        [DllImport("NMC2.dll")]
        public static extern short nmc_GetMDIOInPin(short nNmcNo, short nPinNo, out short pnInStatus);

        [DllImport("NMC2.dll")]
        public static extern short nmc_GetMDIOInput(short nNmcNo, short[] pnInStatus);
        [DllImport("NMC2.dll")]
        public static extern short nmc_GetMDIOInputBit(short nNmcNo, short nBitNo, out short pnInStatus);
        [DllImport("NMC2.dll")]
        public static extern short nmc_GetMDIOOutput(short nNmcNo, short[] pnOutStatus);

        [DllImport("NMC2.dll")]
        public static extern short nmc_SetMDIOOutput(short nNmcNo, short[] pnOutStatus);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetMDIOOutputBit(short nNmcNo, short nBitNo, short nOutStatus);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetMDIOOutputTog(short nNmcNo, short nBitNo);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetMDIOOutputAll(short nNmcNo, short[] pnOnBitNo, short[] pnOffBitNo);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetMDIOOutputTogAll(short nNmcNo, short[] pnBitNo);

        [DllImport("NMC2.dll")]
        public static extern short nmc_GetMDIOInput32(short nNmcNo, out int plInStatus);
        [DllImport("NMC2.dll")]
        public static extern short nmc_GetMDIOOutput32(short nNmcNo, out int plOutStatus);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetMDIOOutput32(short nNmcNo, int lOutStatus);

        //-----------------------------------------------------------------------------
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetDIOOutPin(short nNmcNo, short nPinNo, short nOutStatus);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetDIOOutTogPin(short nNmcNo, short nPinNo);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetDIOOutPins(short nNmcNo, short nCount, short[] pnPinNo, short[] pnStatus);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetDIOOutTogPins(short nNmcNo, short nCount, short[] pnPinNo);
        [DllImport("NMC2.dll")]
        public static extern short nmc_GetDIOInPin(short nNmcNo, short nPinNo, out short pnInStatus);

        [DllImport("NMC2.dll")]
        public static extern short nmc_GetDIOInput(short nNmcNo, short[] pnInStatus);
        [DllImport("NMC2.dll")]
        public static extern short nmc_GetDIOInput128(short nNmcNo, short[] pnInStatus);
        [DllImport("NMC2.dll")]
        public static extern short nmc_GetDIOInputBit(short nNmcNo, short nBitNo, out short pnInStatus);

        [DllImport("NMC2.dll")]
        public static extern short nmc_GetDIOOutput(short nNmcNo, short[] pnOutStatus);
        [DllImport("NMC2.dll")]
        public static extern short nmc_GetDIOOutput128(short nNmcNo, short[] pnOutStatus);

        [DllImport("NMC2.dll")]
        public static extern short nmc_SetDIOOutput(short nNmcNo, short[] pnOutStatus);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetDIOOutput128(short nNmcNo, short[] pnOutStatus);

        [DllImport("NMC2.dll")]
        public static extern short nmc_SetDIOOutputBit(short nNmcNo, short nBitno, short nOutStatus);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetDIOOutputTog(short nNmcNo, short nBitNo);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetDIOOutputAll(short nNmcNo, short[] pnOnBitNo, short[] pnOffBitNo);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetDIOOutputTogAll(short nNmcNo, short[] pnBitNo);
        [DllImport("NMC2.dll")]
        public static extern short nmc_GetDIOInput64(short nNmcNo, out long plInStatus);
        [DllImport("NMC2.dll")]
        public static extern short nmc_GetDIOOutput64(short nNmcNo, out long plOutStatus);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetDIOOutput64(short nNmcNo, long lOutStatus);

        [DllImport("NMC2.dll")]
        public static extern short nmc_GetDIOInput32(short nNmcNo, short nIndex, out int plInStatus);
        [DllImport("NMC2.dll")]
        public static extern short nmc_GetDIOOutput32(short nNmcNo, short nIndex, out int plOutStatus);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetDIOOutput32(short nNmcNo, short nIndex, int lOutStatus);
        //-----------------------------------------------------------------------------
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetEXDIOOutPin(short nNmcNo, short nPinNo, out short nOutStatus);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetEXDIOOutTogPin(short nNmcNo, short nPinNo);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetEXDIOOutPins(short nNmcNo, short nCount, short[] pnPinNo, short[] pnStatus);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetEXDIOOutTogPins(short nNmcNo, short nCount, short[] pnPinNo);
        [DllImport("NMC2.dll")]
        public static extern short nmc_GetEXDIOInPin(short nNmcNo, short nPinNo, out short pnInStatus);

        [DllImport("NMC2.dll")]
        public static extern short nmc_GetEXDIOInput(short nNmcNo, short[] pnInStatus);
        [DllImport("NMC2.dll")]
        public static extern short nmc_GetEXDIOInputBit(short nNmcNo, short nBitNo, out short pnInStatus);
        [DllImport("NMC2.dll")]
        public static extern short nmc_GetEXDIOOutput(short nNmcNo, short[] pnOutStatus);

        [DllImport("NMC2.dll")]
        public static extern short nmc_SetEXDIOOutput(short nNmcNo, short[] pnOutStatus);

        [DllImport("NMC2.dll")]
        public static extern short nmc_SetEXDIOOutputBit(short nNmcNo, short nBitNo, short nOutStatus);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetEXDIOOutputTog(short nNmcNo, short nBitNo);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetEXDIOOutputAll(short nNmcNo, short[] pnOnBitNo, short[] pnOffBitNo);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetEXDIOOutputTogAll(short nNmcNo, short[] pnBitNo);

        [DllImport("NMC2.dll")]
        public static extern short nmc_GetEXDIOInput32(short nNmcNo, out int plInStatus);
        [DllImport("NMC2.dll")]
        public static extern short nmc_GetEXDIOOutput32(short nNmcNo, out int plOutStatus);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetEXDIOOutput32(short nNmcNo, int lOutStatus);
        //-----------------------------------------------------------------------------
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetOutLimitTimePin(short nNmcNo, short nIoType,short nPinNo,short nOn,int nTime);
        [DllImport("NMC2.dll")]
        public static extern short nmc_GetOutLimitTimePin(short nNmcNo, short nIoType,short nPinNo,out short pnSet,out short pnStatus,out short pnOutStatus, out int pnRemainTime);
        //-----------------------------------------------------------------------------
        [DllImport("NMC2.dll")]
        public static extern short nmc_GetFirmVersion(short nNmcNo, out char pStr);
        //-----------------------------------------------------------------------------
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetUnitPerPulse(short nNmcNo, short nAxisNo, double dRatio);
        [DllImport("NMC2.dll")]
        public static extern double nmc_GetUnitPerPulse(short nNmcNo, short nAxisNo);
        //-----------------------------------------------------------------------------
        [DllImport("NMC2.dll")]
        public static extern void nmc_SetProtocolMethod(short nNmcNo, short nMethod);
        [DllImport("NMC2.dll")]
        public static extern short nmc_GetProtocolMethod(short nNmcNo);
        //-----------------------------------------------------------------------------
        [DllImport("NMC2.dll")]
        public static extern short nmc_GetIPAddress(out short pnField0, out short pnField1, out short pnField2, out short pnField3);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetIPAddress(short nNmcNo, short nField0, short nField1, short nField2);
        [DllImport("NMC2.dll")]
        public static extern short nmc_WriteIPAddress(short nNmcNo, short[] pnIP, short[] pnSubNet, short nGateway);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetDefaultIPAddress(short nNmcNo);
        //-----------------------------------------------------------------------------
        [DllImport("NMC2.dll")]
        public static extern short nmc_GetDeviceType(short nNmcNo, out int plDeviceType);

        [DllImport("NMC2.dll")]
        public static extern short nmc_GetDeviceInfo(short nNmcNo, out short pnMotionType, out short pnDioType, out short pnEXDio, out short pnMDio);
        [DllImport("NMC2.dll")]
        public static extern int nmc_GetEnumList(out short pnIp, out NMCEQUIPLIST pNmcList);
        [DllImport("NMC2.dll")]
        public static extern int nmc_GetDIOInfo(short nNmcNo, out short pnInCount,out short pnOutCount);
        //-----------------------------------------------------------------------------
        [DllImport("NMC2.dll")]
        public static extern short nmc_DIOTest(short nNmcNo, short nMode, short nDelay);
        //-----------------------------------------------------------------------------
        [DllImport("NMC2.dll")]
        public static extern short nmc_MotCfgSaveToROM(short nNmcNo, short nMode);
        [DllImport("NMC2.dll")]
        public static extern short nmc_MotCfgSetDefaultROM(short nNmcNo, short nMode);
        [DllImport("NMC2.dll")]
        public static extern short nmc_MotCfgLoadFromROM(short nNmcNo, short nMode);

        //------------------------------------------------------------------------------
        [DllImport("NMC2.dll")]
        public static extern short nmc_AccOffsetCount(short nNmcNo, short nAxisNo, int lPulse);
        //------------------------------------------------------------------------------
        [DllImport("NMC2.dll")]
        public static extern short nmc_PingCheck(short nNmcNo, int lWaitTime);

        //------------------------------------------------------------------------------
        [DllImport("NMC2.dll")]
        public static extern short nmc_GetBusyStatus(short nNmcNo, short nAxisNo, out short pnBusyStatus);
        [DllImport("NMC2.dll")]
        public static extern short nmc_GetBusyStatusAll(short nNmcNo, short[] pnBusyStatus);
        [DllImport("NMC2.dll")]
		public static extern short nmc_SetTriggerCfg(short nNmcNo, short nAxis, short nCmpMode, int lCmpAmount, double dDioOnTime, short nPinNo, short nDioType, short nReserve);
        [DllImport("NMC2.dll")]
		public static extern short nmc_SetTriggerEnable(short nNmcNo, short nAxis,short nEnable);
        [DllImport("NMC2.dll")]
		public static extern short nmc_GetMapIO(short nNmcNo, short[] pnMapInStatus);
        [DllImport("NMC2.dll")]
		public static extern short nmc_MapMove(short nNmcNo,short nAxis, double dPos,short nMapIndex, short nOpt);
        [DllImport("NMC2.dll")]
		public static extern short nmc_MapMoveEx(short nNmcNo,short nAxis, double dPos,short nMapIndex, short nOpt, short nPosType);
        [DllImport("NMC2.dll")]
		public static extern short nmc_GetMapData(short nNmcNo,short nMapIndex, out NMCMAPDATA pNmcMapData);
        [DllImport("NMC2.dll")]
		public static extern short nmc_GetMapDataEx(short nNmcNo,short nMapIndex, short nDataIndex,out NMCMAPDATA pNmcMapData);
        [DllImport("NMC2.dll")]
		public static extern short nmc_GetAxesCmdSpeed(short nNmcNo, double[] pDrvSpeed);
        [DllImport("NMC2.dll")]
		public static extern short nmc_GetAxesEncSpeed(short nNmcNo, double[] pdEncSpeed);
        [DllImport("NMC2.dll")]
		public static extern short nmc_GetAxesCmdEncSpeed(short nNmcNo, double[] pdCmdSpeed, double[] pdEncSpeed);
        //------------------------------------------------------------------------------
        [DllImport("NMC2.dll")]
		public static extern short nmc_SetGantryAxis(short nNmcNo, short nGroupNo,short nMain, short nSub);
        [DllImport("NMC2.dll")]
		public static extern short nmc_SetGantryEnable(short nNmcNo, short nGroupNo, short nGantryEnable);
        [DllImport("NMC2.dll")]
		public static extern short nmc_GetGantryInfo(short nNmcNo, short[] pnEnable,short[] pnMainAxes,short[] pnSubAxes);
        //------------------------------------------------------------------------------
        [DllImport("NMC2.dll")]
    public static extern short nmc_ContRun(short nNmcNo, short nGroupNo,short nRunMode);
        [DllImport("NMC2.dll")]
		public static extern short nmc_GetContStatus (short nNmcNo, out NMCCONTSTATUS pContStatus);

        [DllImport("NMC2.dll")]
		public static extern short nmc_SetContNodeLine(short nNmcNo, short nGroupNo, short nNodeNo, 
				short nAxisNo0, short nAxisNo1, 
				double dPos0, double dPos1, 
				double dStart,double dAcc, double dDec , double dDriveSpeed);
        [DllImport("NMC2.dll")]
		public static extern short nmc_SetContNodeLineIO(short nNmcNo, short nGroupNo, short nNodeNo, 
				short nAxisNo0, short nAxisNo1, 
				double dPos0, double dPos1, 
				double dStart,double dAcc, double dDec , double dDriveSpeed, short nOnOff);


    [DllImport("NMC2.dll")]
		public static extern short nmc_SetContNode3Line(short nNmcNo, short nGroupNo, short nNodeNo, 
				short nAxisNo0, short nAxisNo1, short nAxisNo2,
				double dPos0, double dPos1, double dPos2, 
				double dStart,double dAcc, double dDec , double dDriveSpeed);
    [DllImport("NMC2.dll")]
		public static extern short nmc_SetContNode3LineIO(short nNmcNo, short nGroupNo, short nNodeNo, 
				short nAxisNo0, short nAxisNo1, short nAxisNo2,
				double dPos0, double dPos1, double dPos2, 
				double dStart,double dAcc, double dDec , double dDriveSpeed, short nOnOff);
				
		[DllImport("NMC2.dll")]
		public static extern short nmc_SetContNodeArc(short nNmcNo, short nGroupNo, short nNodeNo, 
				short nAxisNo0, short nAxisNo1, 
				double dCenter0, double dCenter1, double dAngle, 
				double dStart,double dAcc, double dDec, double dDriveSpeed);
		[DllImport("NMC2.dll")]
		public static extern short nmc_SetContNodeArcIO(short nNmcNo, short nGroupNo, short nNodeNo, 
				short nAxisNo0, short nAxisNo1, 
				double dCenter0, double dCenter1, double dAngle, 
				double dStart,double dAcc, double dDec, double dDriveSpeed, short nOnOff);
				
		[DllImport("NMC2.dll")]
		public static extern short nmc_ContNodeClear(short nNmcNo,short nGroupNo);
		
		[DllImport("NMC2.dll")]
		public static extern short nmc_ContSetIO(short nNmcNo,short nGroupNo,short nIoType,short nIoPinNo,short nEndNodeOnOff);
		
		[DllImport("NMC2.dll")]
		public static extern short nmc_GetCmdPos(short nNmcNo, short nAxis,out int plCmdPos);
		[DllImport("NMC2.dll")]
		public static extern short nmc_GetEncPos(short nNmcNo, short nAxis,out int plEncPos);
		[DllImport("NMC2.dll")]
		public static extern short nmc_SetDisconectedStopMode(short nNmcNo, int lTimeInterval, short nStopMode);
        [DllImport("NMC2.dll")]
        public static extern short nmc_SetDisconnectedStopMode(short nNmcNo, int lTimeInterval, short nStopMode);

		[DllImport("NMC2.dll")]
	    public static extern short nmc_SetEmgEnable(short nNmcNo, short nEnable);
	
		[DllImport("NMC2.dll")]
		public static extern short nmc_SetSerialConfig(short nNmcNo, short nBaud, short nData, short nStop, short nParity);
		[DllImport("NMC2.dll")]
		public static extern short nmc_SetSerialMode(short nNmcNo, short nMode);
		[DllImport("NMC2.dll")]
		public static extern short nmc_SerialWrite(short nNmcNo, short nLen, char[] pStr);
		[DllImport("NMC2.dll")]
		public static extern short nmc_SerialRead(short nNmcNo, out short pnReadLen, char[] pReadStr);
	
		[DllImport("NMC2.dll")]
		public static extern short nmc_SetMpgMode(short nNmcNo, short nAxisNo, short nMode, long lPulse);
	
		[DllImport("NMC2.dll")] // 무제한 연속 보간 큐버퍼 초기화 함수 - UCI 
		public static extern short nmc_ContiSetNodeClear(short nNmcNo, short nGroupNo);
		[DllImport("NMC2.dll")] // 무제한 연속 보간 초기 설정 함수 - UCI
		public static extern short nmc_ContiSetMode(short nNmcNo,short nGroupNo,short nAVTRIMode,short nEmptyMode,short n1stAxis,short n2ndAxis,short n3rdAxis,double dMaxDrvSpeed,short nIoType,int nIoCtrlPinMask,int nIoCtrlEndVal);
		[DllImport("NMC2.dll")] // 무제한 연속 보간 상태 체크 함수 - UCI
		public static extern short nmc_ContiGetStatus(short nNmcNo,out NMCCONTISTATUS pContiStatus);
		[DllImport("NMC2.dll")] // 무제한 연속 보간 2축 직선 보간 함수 - UCI
		public static extern short nmc_ContiAddNodeLine2Axis(short nNmcNo,short nGroupNo,double dPos0,double dPos1,double dStart,double dAcc,double dDec,double dDrvSpeed,int nIoCtrlVal);
		[DllImport("NMC2.dll")] // 무제한 연속 보간 3축 직선 보간 함수 - UCI
		public static extern short nmc_ContiAddNodeLine3Axis(short nNmcNo,short nGroupNo,double dPos0,double dPos1,double dPos2,double dStart,double dAcc,double dDec,double dDrvSpeed,int nIoCtrlVal);
		[DllImport("NMC2.dll")] // 무제한 연속 보간 2축 원호 보간 함수 - UCI
		public static extern short nmc_ContiAddNodeArc(short nNmcNo,short nGroupNo,double dCenter0,double dCenter1,double dAngle,double dStart,double dAcc,double dDec,double dDrvSpeed,int nIoCtrlVal);
		[DllImport("NMC2.dll")] // 무제한 연속 보간 노드 추가 종료 함수 - UCI
		public static extern short nmc_ContiSetCloseNode(short nNmcNo,short nGroupNo);
		[DllImport("NMC2.dll")] // 무제한 연속 보간 실행/정지 함수 - UCI
		public static extern short nmc_ContiRunStop(short nNmcNo, short nGroupNo, short nRunMode);
		
		[DllImport("NMC2.dll")] // 삼각파형 방지 기능 설정
		public static extern short nmc_AVTRISetMode(short nNmcNo,short nAxis,short nAVTRIMode);
		[DllImport("NMC2.dll")] // 삼각파형 방지기능 설정값 가져오기
		public static extern short nmc_AVTRIGetMode(short nNmcNo,short nAxis,out short nAVTRIMode);
		
		[DllImport("NMC2.dll")] //응답대기시간 설정
		public static extern short nmc_SetWaitTime(short nNmcNo, long lWaitTime);
    };
};
//------------------------------------------------------------------------------

//DESCRIPTION  'NMC Windows Dynamic Link Library'     -- *def file* description ....


