using System;
using System.Runtime.InteropServices;

namespace CNet
{
	/// <summary>
	/// VC6.0 에서 제작된 CNETSDK.dll 파일을 CSharp 에서 사용하기 위한 함수 마샬링입니다.
	/// </summary>
    public static unsafe class SafeNativeMethods 
    {
        //******************************************************************************
        //* [ ComiNETSDKDef.h ]
        //* Header file for definitions of constants and data in CMNSDK library
        //* - Update Data: 2007/04/23
        //* - Provider: COMIZOA Co., Ltd.
        //* - Phone: +82-42-936-6500~6
        //* - Fax  : +82-42-936-6507
        //* - URL  : http://www.comizoa.co.kr,  http://www.comizoa.com
        //'*******************************************************************************

        //*******************************************************************************
        public const int VERSION_STRING_LENGTH = 21 + 1;
        public const string VERSION_STRING = "C-NET Library 3.0";

        ///////////////////////////////////////////////////////////////////////
        public const int cmnERR_NONE = 0;
        public const int cmnERR_INVALID_PARAMETER = -190;
        public const int cmnERR_MEM_ALLOC_FAIL = -290;
        public const int cmnERR_GLOBAL_MEM_FAIL = -292;

        ///////////////////////////////////////////////////////////////////////
        public const int cmnTRUE = 1;
        public const int cmnFALSE = 0;

        ///////////////////////////////////////////////////////////////////////
        public const int cmnERR_GN_RELOAD_DEVICE_FAIL = -9000;
        public const int cmnERR_GN_LOAD_DEVICE_FAIL = -10000;
        public const int cmnERR_GN_UNLOAD_DEVICE_FAIL = -10001;
        public const int cmnERR_GET_SLAVE_INFO_FAIL = -10002;
        public const int cmnERR_GET_SLAVE_INFO_ALL_FAIL = -10003;
        public const int cmnERR_GET_GLOBAL_CHANNEL_MAP_FAIL = -10004;
        public const int cmnERR_GN_GET_GLOBAL_CHANNEL_MAP_FAIL = -10005;
        public const int cmnERR_GN_PUT_USER_CHANNEL_MAP_FAIL = -10006;
        public const int cmnERR_GN_DLG_SETUP_FAIL = -10007;
        public const int cmnERR_LOCAL_DI_GET_ONE_FAIL = -10008;
        public const int cmnERR_LOCAL_DI_GET_MULTI_FAIL = -10009;
        public const int cmnERR_LOCAL_DO_PUT_ONE_FAIL = -10010;
        public const int cmnERR_LOCAL_DO_PUT_MULTI_FAIL = -10011;
        public const int cmnERR_LOCAL_DO_GET_ONE_FAIL = -10012;
        public const int cmnERR_LOCAL_DO_GET_MULTI_FAIL = -10013;
        public const int cmnERR_DI_GET_ONE_FAIL = -10014;
        public const int cmnERR_DI_GET_MULTI_FAIL = -10015;
        public const int cmnERR_DO_PUT_ONE_FAIL = -10016;
        public const int cmnERR_DO_PUT_MULTI_FAIL = -10017;
        public const int cmnERR_DO_GET_ONE_FAIL = -10018;
        public const int cmnERR_DO_GET_MULTI_FAIL = -10019;
        public const int cmnERR_GN_START_RING_ALL_FAIL = -10020;
        public const int cmnERR_GN_START_RING_FAIL = -10021;
        public const int cmnERR_GN_RESET_RING_FAIL = -10022;
        public const int cmnERR_GN_STOP_RING_FAIL = -10023;
        public const int cmnERR_GN_RESET_DEVICE_FAIL = -10024;
        public const int cmnERR_GN_ISRESET_DEVICE_FAIL = -10025;
        public const int cmnERR_GN_GET_COM_STATUS_FAIL = -10026;
        public const int cmmERR_GN_GET_COM_SPEED_FAIL = -10027;
        public const int cmnERR_GN_SET_COM_SPEED = -10028;
        public const int cmnERR_GN_DLG_GET_LAST_MESSAGE_FAIL = -10029;
        public const int cmnERR_GN_DLG_GET_NO_MORE_MESSAGE_FAIL = -10030;
        public const int cmnERR_GN_DLG_GET_BUFFER_SMALL_FAIL = -10031;

        ///////////////////////////////////////////////////////////////////////
        public const int cmnLOGIC_A = 0;
        public const int cmnLOGIC_B = 1;

        ///////////////////////////////////////////////////////////////////////

        // Device Information

        ///////////////////////////////////////////////////////////////////////
        public const int DI_00_DO_32C = 0x0;
        public const int DI_08_DO_08C = 0x1;
        public const int DI_16_DO_16C = 0x2;
        public const int DI_16_DO_OOH = 0x3;
        public const int DI_32_DO_00C = 0x4;
        public const int DI_08_DO_08H = DI_08_DO_08C;
        public const int DI_00_DO_16H = 0x5;

        ///////////////////////////////////////////////////////////////////////

        // Communication Speed

        ///////////////////////////////////////////////////////////////////////
        public const int SPEED_1X = 0x0;
        //// 2.5 Mbps 
        public const int SPEED_2X = 0x1;
        //// 5 Mbps 
        public const int SPEED_3X = 0x2;
        //// 10Mbps 
        public const int SPEED_4X = 0x3;
        //// 20Mbps 

        ///////////////////////////////////////////////////////////////////////

        //// Communication Status

        ///////////////////////////////////////////////////////////////////////
        //public const int RING_DISCONNECTED = 0x0;
        //public const int RING_CONNECTED    = 0x1;
        //public const int SLAVE_ERROR       = 0x2;
        //public const int RING_STOP         = 0x3;

        //public const int RING_DISCONNECTED = 13;
        //public const int RING_CONNECTED = 12;
        //public const int SLAVE_ERROR = 3;
        //public const int RING_STOP = 2;

        public const int RING_DISCONNECTED = 0x2000;
        public const int RING_CONNECTED = 0x1000;
        public const int SLAVE_ERROR = 0x8;
        public const int RING_STOP = 0x4;

        ///////////////////////////////////////////////////////////////////////

        //// Debugging

        ///////////////////////////////////////////////////////////////////////
        public const int DEBUG_LEVEL_SUMMERY = 0;
        public const int DEBUG_LEVEL_DETAIL = 1;
        public const int DEBUG_LEVEL_DEBUG = 2;
        public const int DEBUG_LEVEL_ALL = 3;
        public const int DEBUG_DISABLE = 0;
        public const int DEBUG_ENABLE = 1;
        public const int DEBUG_OUT_WINDOW = 0;
        public const int DEBUG_OUT_LOCALFILE = 1;
        public const int DEBUG_OUT_CONSOLE = 2;
        public const int DEBUG_OUT_SOCKET = 3;
        public const int DEBUG_OUT_CALLBACK = 4;

        ///////////////////////////////////////////////////////////////////////

        //// 개별 Master 장치에 대한 정보

        ///////////////////////////////////////////////////////////////////////
        public const int MAX_SLAVE = 64;
        public const int MAX_RING = 2;

        ////////////////////////////////////////////////////////////////////
        // Device Load / Unload
        ////////////////////////////////////////////////////////////////////

        [DllImport("CNETSDK.dll", EntryPoint = "cmnGnLoadDevice")]
        internal static extern unsafe int cmnGnLoadDevice([MarshalAs(UnmanagedType.I4)]int IsResetDevice, [MarshalAs(UnmanagedType.I4)] ref int pnTotalMasterDevices);

        [DllImport("CNETSDK.dll", EntryPoint = "cmnGnUnloadDevice")]
        internal static extern unsafe int cmnGnUnloadDevice();

        [DllImport("CNETSDK.dll", EntryPoint = "cmnGnReloadDevice")]
        internal static extern unsafe int cmnGnReloadDevice([MarshalAs(UnmanagedType.I4)]int IsResetDevice, [MarshalAs(UnmanagedType.I4)] ref int nTotalMasterDevices);

        [DllImport("CNETSDK.dll", EntryPoint = "cmnGnCtrlBoost")]
        internal static extern unsafe int cmnGnCtrlBoost([MarshalAs(UnmanagedType.I4)]int IsEnable);

        ////////////////////////////////////////////////////////////////////
        // Basic Control
        ////////////////////////////////////////////////////////////////////
        [DllImport("CNETSDK.dll", EntryPoint = "cmnGnIsResetDevice")]
        internal static extern unsafe int cmnGnIsResetDevice([MarshalAs(UnmanagedType.I4)]int nDeviceNo, [MarshalAs(UnmanagedType.I4)]int nRingNo, [MarshalAs(UnmanagedType.I4)] ref int IsReset);

        [DllImport("CNETSDK.dll", EntryPoint = "cmnGnResetDevice", SetLastError = true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnGnResetDevice([MarshalAs(UnmanagedType.I4)]int nDeviceNo, [MarshalAs(UnmanagedType.I4)]int nRingNo);

        [DllImport("CNETSDK.dll", EntryPoint = "cmnGnSetSearchSpeicalDelay", SetLastError = true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnGnSetSearchSpeicalDelay([MarshalAs(UnmanagedType.I4)]int nDeviceNo, [MarshalAs(UnmanagedType.I4)]int nDelay_us);

        ////////////////////////////////////////////////////////////////////
        // Debug Function
        ////////////////////////////////////////////////////////////////////
        [DllImport("CNETSDK.dll", EntryPoint = "cmnGnDlgSetup", SetLastError = true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnGnDlgSetup([MarshalAs(UnmanagedType.I1)]bool Enable, [MarshalAs(UnmanagedType.R8)]double nLevel, double nDebugType, [MarshalAs(UnmanagedType.I4)]int Handler, [MarshalAs(UnmanagedType.I4)]object lParam, [MarshalAs(UnmanagedType.LPStr)] ref string szDebugFileName);

        [DllImport("CNETSDK.dll", EntryPoint ="cmnGnDlgGetLastMessage", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnGnDlgGetLastMessage([MarshalAs(UnmanagedType.I4)]int nBufferSize, [MarshalAs(UnmanagedType.LPStr)] ref string szBuffer, [MarshalAs(UnmanagedType.I4)] ref int StoredMsg);

        ////////////////////////////////////////////////////////////////////
        // C-NET Communcation internal  (MASTER)
        ////////////////////////////////////////////////////////////////////
        [DllImport("CNETSDK.dll", EntryPoint ="cmnLocalDiSetInputLogic", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnLocalDiSetInputLogic([MarshalAs(UnmanagedType.I4)]int GlobalChNo, [MarshalAs(UnmanagedType.I4)]int InputLogic);

        [DllImport("CNETSDK.dll", EntryPoint ="cmnLocalDiGetInputLogic", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnLocalDiGetInputLogic([MarshalAs(UnmanagedType.I4)]int GlobalChNo, [MarshalAs(UnmanagedType.I4)] ref int InputLogic);

        [DllImport("CNETSDK.dll", EntryPoint ="cmnLocalDoSetOutputLogic", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnLocalDoSetOutputLogic([MarshalAs(UnmanagedType.I4)]int GlobalChNo, [MarshalAs(UnmanagedType.I4)]int OutputLogic);

        [DllImport("CNETSDK.dll", EntryPoint ="cmnLocalDoGetOutputLogic", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnLocalDoGetOutputLogic([MarshalAs(UnmanagedType.I4)]int GlobalChNo, [MarshalAs(UnmanagedType.I4)] ref int OutputLogic);

        [DllImport("CNETSDK.dll", EntryPoint ="cmnLocalDiGetOne", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnLocalDiGetOne([MarshalAs(UnmanagedType.I4)]int GlobalChNo, [MarshalAs(UnmanagedType.I4)] ref int InputStatus);

        [DllImport("CNETSDK.dll", EntryPoint ="cmnLocalDiGetMulti", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnLocalDiGetMulti([MarshalAs(UnmanagedType.I4)]int IniChannel, [MarshalAs(UnmanagedType.I4)]int NumChannel, [MarshalAs(UnmanagedType.I4)] ref int InuptStates);

        [DllImport("CNETSDK.dll", EntryPoint ="cmnLocalDoPutOne", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnLocalDoPutOne([MarshalAs(UnmanagedType.I4)]int GlobalChNo, [MarshalAs(UnmanagedType.I4)]int OutState);

        [DllImport("CNETSDK.dll", EntryPoint ="cmnLocalDoPutMulti", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnLocalDoPutMulti([MarshalAs(UnmanagedType.I4)]int IniChannel, [MarshalAs(UnmanagedType.I4)]int NumChannel, [MarshalAs(UnmanagedType.I4)]int OutStates);

        [DllImport("CNETSDK.dll", EntryPoint ="cmnLocalDoPutMulti", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnLocalDoGetOne([MarshalAs(UnmanagedType.I4)]int GlobalChNo, [MarshalAs(UnmanagedType.I4)] ref int OutputStatus);

        [DllImport("CNETSDK.dll", EntryPoint ="cmnLocalDoPutMulti", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnLocalDoGetMulti([MarshalAs(UnmanagedType.I4)]int IniChannel, [MarshalAs(UnmanagedType.I4)]int NumChannel, [MarshalAs(UnmanagedType.I4)] ref int OutStates);

        ////////////////////////////////////////////////////////////////////
        // C-NET Communication between slave and master  (SLAVE)
        ////////////////////////////////////////////////////////////////////
        [DllImport("CNETSDK.dll", EntryPoint ="cmnDiSetInputLogic", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnDiSetInputLogic([MarshalAs(UnmanagedType.I4)]int GlobalChNo, [MarshalAs(UnmanagedType.I4)]int InputLogic);
        
        [DllImport("CNETSDK.dll", EntryPoint ="cmnDiGetInputLogic", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnDiGetInputLogic([MarshalAs(UnmanagedType.I4)]int GlobalChNo, [MarshalAs(UnmanagedType.I4)] ref int InputLogic);

        [DllImport("CNETSDK.dll", EntryPoint ="cmnDoSetOutputLogic", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnDoSetOutputLogic([MarshalAs(UnmanagedType.I4)]int GlobalChNo, [MarshalAs(UnmanagedType.I4)]int OutputLogic);

        [DllImport("CNETSDK.dll", EntryPoint ="cmnDoGetOutputLogic", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnDoGetOutputLogic([MarshalAs(UnmanagedType.I4)]int GlobalChNo, [MarshalAs(UnmanagedType.I4)] ref int OutputLogic);

        [DllImport("CNETSDK.dll", EntryPoint ="cmnDiGetOne", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnDiGetOne([MarshalAs(UnmanagedType.I4)]int GlobalChNo, [MarshalAs(UnmanagedType.I4)] ref int InputStatus);

        [DllImport("CNETSDK.dll", EntryPoint ="cmnDiGetMulti", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnDiGetMulti([MarshalAs(UnmanagedType.I4)]int IniChannel, [MarshalAs(UnmanagedType.I4)]int nChannels, [MarshalAs(UnmanagedType.I4)] ref int InputStates);

        [DllImport("CNETSDK.dll", EntryPoint ="cmnDoPutOne", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnDoPutOne([MarshalAs(UnmanagedType.I4)]int GlobalChNo, [MarshalAs(UnmanagedType.I4)]int OutState);

        [DllImport("CNETSDK.dll", EntryPoint ="cmnDoPutMulti", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnDoPutMulti([MarshalAs(UnmanagedType.I4)]int IniChannel, [MarshalAs(UnmanagedType.I4)]int nChannels, [MarshalAs(UnmanagedType.I4)]int OutStates);

        [DllImport("CNETSDK.dll", EntryPoint ="cmnDoGetOne", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnDoGetOne([MarshalAs(UnmanagedType.I4)]int GlobalChNo, [MarshalAs(UnmanagedType.I4)] ref int OutputStatus);

        [DllImport("CNETSDK.dll", EntryPoint ="cmnDoGetMulti", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnDoGetMulti([MarshalAs(UnmanagedType.I4)]int IniChannel, [MarshalAs(UnmanagedType.I4)]int nChannels, [MarshalAs(UnmanagedType.I4)] ref int OutStates);

        ////////////////////////////////////////////////////////////////////
        // C-NET Communication between slave and master  (SLAVE) (EX Functions)
        ////////////////////////////////////////////////////////////////////
        [DllImport("CNETSDK.dll", EntryPoint ="cmnDiGetOneEx", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnDiGetOneEx([MarshalAs(UnmanagedType.I4)]int nMasterCardNo, [MarshalAs(UnmanagedType.I4)]int nRingNo, [MarshalAs(UnmanagedType.I4)]int nDevIp, [MarshalAs(UnmanagedType.I4)]int nPortNo, [MarshalAs(UnmanagedType.I4)]int nChannelNo, [MarshalAs(UnmanagedType.I4)] ref int InputStatus);

        [DllImport("CNETSDK.dll", EntryPoint ="cmnDiGetMultiEx", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnDiGetMultiEx([MarshalAs(UnmanagedType.I4)]int nMasterCardNo, [MarshalAs(UnmanagedType.I4)]int nRingNo, [MarshalAs(UnmanagedType.I4)]int nDevIp, [MarshalAs(UnmanagedType.I4)]int nPortNo, [MarshalAs(UnmanagedType.I4)] ref int InputStatus);

        [DllImport("CNETSDK.dll", EntryPoint ="cmnDoPutOneEx", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnDoPutOneEx([MarshalAs(UnmanagedType.I4)]int nMasterCardNo, [MarshalAs(UnmanagedType.I4)]int nRingNo, [MarshalAs(UnmanagedType.I4)]int nDevIp, [MarshalAs(UnmanagedType.I4)]int nPortNo, [MarshalAs(UnmanagedType.I4)]int nChannel, [MarshalAs(UnmanagedType.I4)]int OutState);

        [DllImport("CNETSDK.dll", EntryPoint ="cmnDoPutMultiEx", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnDoPutMultiEx([MarshalAs(UnmanagedType.I4)]int nMasterCardNo, [MarshalAs(UnmanagedType.I4)]int nRingNo, [MarshalAs(UnmanagedType.I4)]int nDevIp, [MarshalAs(UnmanagedType.I4)]int nPortNo, [MarshalAs(UnmanagedType.I4)]int OutStates);

        [DllImport("CNETSDK.dll", EntryPoint ="cmnDoGetOneEx", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnDoGetOneEx([MarshalAs(UnmanagedType.I4)]int nMasterCardNo, [MarshalAs(UnmanagedType.I4)]int nRingNo, [MarshalAs(UnmanagedType.I4)]int nDevIp, [MarshalAs(UnmanagedType.I4)]int nPortNo, [MarshalAs(UnmanagedType.I4)]int nChannelNo, [MarshalAs(UnmanagedType.I4)] ref int OutState);

        [DllImport("CNETSDK.dll", EntryPoint ="cmnDoGetMultiEx", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnDoGetMultiEx([MarshalAs(UnmanagedType.I4)]int nMasterCardNo, [MarshalAs(UnmanagedType.I4)]int nRingNo, [MarshalAs(UnmanagedType.I4)]int nDevIp, [MarshalAs(UnmanagedType.I4)]int nPortNo, [MarshalAs(UnmanagedType.I4)] ref int OutStates);

        ////////////////////////////////////////////////////////////////////
        // Communication Control
        ////////////////////////////////////////////////////////////////////
        [DllImport("CNETSDK.dll", EntryPoint ="cmnGnStartRingAll", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnGnStartRingAll();

        [DllImport("CNETSDK.dll", EntryPoint ="cmnGnStartRing", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnGnStartRing([MarshalAs(UnmanagedType.I4)]int nDeviceNo, [MarshalAs(UnmanagedType.I4)]int nRingNo, [MarshalAs(UnmanagedType.I4)]int nSlaveIp);

        [DllImport("CNETSDK.dll", EntryPoint ="cmnGnResetRing", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnGnResetRing([MarshalAs(UnmanagedType.I4)]int nDeviceNo, [MarshalAs(UnmanagedType.I4)]int nRingNo);

        [DllImport("CNETSDK.dll", EntryPoint ="cmnGnStopRing", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnGnStopRing([MarshalAs(UnmanagedType.I4)]int nDeviceNo, [MarshalAs(UnmanagedType.I4)]int nRingNo);

        [DllImport("CNETSDK.dll", EntryPoint ="cmnGnGetComStatus", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnGnGetComStatus([MarshalAs(UnmanagedType.I4)]int nDeviceNo, [MarshalAs(UnmanagedType.I4)]int nRingNo, [MarshalAs(UnmanagedType.I4)] ref int nStatus);

        ////////////////////////////////////////////////////////////////////
        // Communication Speed Control
        ////////////////////////////////////////////////////////////////////
        [DllImport("CNETSDK.dll", EntryPoint ="cmnGnGetComSpeed", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnGnGetComSpeed([MarshalAs(UnmanagedType.I4)]int nDeviceNo, [MarshalAs(UnmanagedType.I4)]int nRingNo, [MarshalAs(UnmanagedType.I4)] ref int pStatus);

        [DllImport("CNETSDK.dll", EntryPoint ="cmnGnSetComSpeed", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnGnSetComSpeed([MarshalAs(UnmanagedType.I4)]int nDeviceNo, [MarshalAs(UnmanagedType.I4)]int nRingNo, [MarshalAs(UnmanagedType.I4)]int nStatus);

        ////////////////////////////////////////////////////////////////////
        // General Device Information Functions
        ////////////////////////////////////////////////////////////////////
        [DllImport("CNETSDK.dll", EntryPoint ="cmnGnGetMasterTotal", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnGnGetMasterTotal([MarshalAs(UnmanagedType.I4)] ref int pTotalDeviceNum);

        [DllImport("CNETSDK.dll", EntryPoint ="cmnGnGetSlaveTotal", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnGnGetSlaveTotal([MarshalAs(UnmanagedType.I4)] ref int pTotalDeviceNum);

        [DllImport("CNETSDK.dll", EntryPoint ="cmnGnGetDITotal", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnGnGetDITotal([MarshalAs(UnmanagedType.I4)] ref int pTotalDiChannelNum);

        [DllImport("CNETSDK.dll", EntryPoint ="cmnGnGetDOTotal", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnGnGetDOTotal([MarshalAs(UnmanagedType.I4)] ref int pTotalDOChannelNum);

        // 해당 디바이스에 장착된 링의 갯수를 반환한다.
        [DllImport("CNETSDK.dll", EntryPoint ="cmnGnGetRingTotal", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnGnGetRingTotal([MarshalAs(UnmanagedType.I4)]int nDeviceNo, [MarshalAs(UnmanagedType.I4)] ref int pRingTotal);

        ////////////////////////////////////////////////////////////////////
        // Master Device Information Functions
        ////////////////////////////////////////////////////////////////////
        [DllImport("CNETSDK.dll", EntryPoint ="cmnGnGetMasterInfo", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnGnGetMasterInfo([MarshalAs(UnmanagedType.I4)]int nDeviceNo, [MarshalAs(UnmanagedType.I4)] ref int pDeviceId);

        ////////////////////////////////////////////////////////////////////
        // Slave Device Information Functions
        ////////////////////////////////////////////////////////////////////
        //[DllImport("CNETSDK.dll", EntryPoint ="cmnGnGetSlaveInfoAll", SetLastError =true,
        //CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        //internal static extern unsafe int cmnGnGetSlaveInfoAll([MarshalAs(UnmanagedType.I4)]int nDeviceNo, ByRef pDevInfo As TCMNDevInfo, ByRef pRing0SlaveTotal, ByRef pRing1SlaveTotal);

        ////////////////////////////////////////////////////////////////////
        // Set/Get of Error Communication Slave Device Number
        ////////////////////////////////////////////////////////////////////
        [DllImport("CNETSDK.dll", EntryPoint ="cmnGnGetSlaveComError", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnGnGetSlaveComError([MarshalAs(UnmanagedType.I4)]int nDeviceNo, [MarshalAs(UnmanagedType.I4)]int nRingNo, [MarshalAs(UnmanagedType.I4)] ref int pStatus1, [MarshalAs(UnmanagedType.I4)] ref int pStatus2);

        [DllImport("CNETSDK.dll", EntryPoint ="cmnGnClrSlaveComError", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnGnClrSlaveComError([MarshalAs(UnmanagedType.I4)]int nDeviceNo, [MarshalAs(UnmanagedType.I4)]int nRingNo, [MarshalAs(UnmanagedType.I4)]int nStatus1, [MarshalAs(UnmanagedType.I4)]int nStatus2);

        ////////////////////////////////////////////////////////////////////
        // Interrupt Functions
        ////////////////////////////////////////////////////////////////////
        [DllImport("CNETSDK.dll", EntryPoint ="cmnGnGetMasterInterruptStatus", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnGnGetMasterInterruptStatus([MarshalAs(UnmanagedType.I4)]int nDeviceNo, [MarshalAs(UnmanagedType.I4)]int nRingNo, [MarshalAs(UnmanagedType.I4)] ref int pStatus);

        [DllImport("CNETSDK.dll", EntryPoint ="cmnGnSetInputInterruptEnable", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnGnSetInputInterruptEnable([MarshalAs(UnmanagedType.I4)]int nDeviceNo, [MarshalAs(UnmanagedType.I4)]int nRingNo, [MarshalAs(UnmanagedType.I4)] ref int pStatus1, [MarshalAs(UnmanagedType.I4)] ref int pStatus2);

        [DllImport("CNETSDK.dll", EntryPoint ="cmnGnGetInputInterruptStatus", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnGnGetInputInterruptStatus([MarshalAs(UnmanagedType.I4)]int nDeviceNo, [MarshalAs(UnmanagedType.I4)]int nRingNo, [MarshalAs(UnmanagedType.I4)] ref int pStatus1, [MarshalAs(UnmanagedType.I4)] ref int pStatus2);

        ////////////////////////////////////////////////////////////////////
        // User Mapped Channel And Global Channel
        ////////////////////////////////////////////////////////////////////
        [DllImport("CNETSDK.dll", EntryPoint ="cmnGnGetGlobalChannelMap", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnGnGetGlobalChannelMap([MarshalAs(UnmanagedType.LPStr)] ref string szFileName);

        [DllImport("CNETSDK.dll", EntryPoint ="cmnGnPutUserChannelMap", SetLastError =true,
        CharSet =CharSet.Ansi, ExactSpelling =true, CallingConvention =CallingConvention.StdCall)]
        internal static extern unsafe int cmnGnPutUserChannelMap([MarshalAs(UnmanagedType.LPStr)] ref string szFileName);        
    }
}

