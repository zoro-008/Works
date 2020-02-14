using System;
using System.Runtime.InteropServices;

namespace ComiDll
{
	/// <summary>
	/// ImportComiDasDLL에 대한 요약 설명입니다.
	/// </summary>
    internal unsafe class SafeNativeMethods
    {
        [StructLayout(LayoutKind.Sequential/*, Pack=1*/)]
        internal struct ScanData
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
            internal float[] fData;
        }
        
        // COMI-DAQ Device ID
        internal enum TCmDeviceID
        {
            // CP-Seriese
	        COMI_CP101=0xC101, COMI_CP201=0xC201, COMI_CP301=0xC301, COMI_CP302=0xC302, COMI_CP401=0xC401, COMI_CP501=0xC501, COMI_SD101=0xB101,
	        // SD-Seriese
	        COMI_SD102=0xB102, COMI_SD103=0xB103, COMI_SD104=0xB104, COMI_SD201=0xB201, COMI_SD202=0xB202, COMI_SD203=0xB203, COMI_SD301=0xB301,
	        COMI_SD401=0xB401, COMI_SD402=0xB402, COMI_SD403=0xB403, COMI_SD404=0xB404, COMI_SD501=0xB501, COMI_SD502=0xB502, COMI_LX101=0xA101,
	        // LX-Seriese
	        COMI_LX102=0xA102, COMI_LX103=0xA103, COMI_LX201=0xA201, COMI_LX202=0xA202, COMI_LX203=0xA203, COMI_LX301=0xA301, COMI_LX401=0xA401,
	        COMI_LX402=0xA402, 
	        // ST-Seriese
	        COMI_ST101=0xD101, COMI_ST201=0xD201, COMI_ST202=0xD202, COMI_ST203=0xD203, COMI_ST301=0xD301, COMI_ST401=0xD401, COMI_ST402=0xD402,
	        // MU-Seriese
	        COMI_MU101=0xE101, COMI_MU201=0xE201, COMI_MU301=0xE301, COMI_MU401=0xE401, COMI_MU402=0xE402, COMI_MU403=0xE403, COMI_MU501=0xE501,
	        COMI_MU701=0xE701, 
	        // MB-Seriese
	        MB_DAC101=0x0101,  MB_DAC201=0x0201,  MB_DAC301=0x0301,  MB_DAC401=0x0401,  MB_DAC501=0x0501,  MB_DAC601=0x0601
        }


        internal enum TCdAiScanTrs
        {
            cmTRS_SINGLE = 1,
            cmTRS_BLOCK = 2
        }

        internal enum TCdVarType
        {
            VT_SHORT = 0, VT_FLOAT, VT_DOUBLE
        }

        internal struct TComiDevInfo
        {
            internal int wSubSysID;
            internal int nInstance;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            internal String szDevName;

            internal char bDevCaps;

            internal char nNumAdChan, nNumDaChan, nNumDiChan, nNumDoChan, nNumCntrChan;
        }


        internal struct TComiDevList
        {
            internal int nNumDev;

            [MarshalAs(UnmanagedType.Struct, SizeConst = 16)]
            internal TComiDevInfo[] Devinfo;
        }

        internal struct TScanFileHead
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
            internal String szDate;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
            internal String szTime;

            internal int nNumChan;

            [MarshalAs(UnmanagedType.I4, SizeConst = 64)]
            internal int[] nChanList;

            internal int dmin, dmax;
            

            [MarshalAs(UnmanagedType.I4, SizeConst = 64)]
            internal float[] vmin;
            [MarshalAs(UnmanagedType.I4, SizeConst = 64)]
            internal float[] vmax;

            internal int dwSavedScanCnt;
        }         

        internal struct TPidParams
        {
            internal float Ref, lim_h, lim_l;
            internal float Kp;
            internal float Td, Ti;
            internal int ch_ref, ch_ad, ch_da;
        }

        internal struct THelicalUserInfo
        {
            internal int c_map, z_axis;
            internal double c_xcen, c_ycen;
            internal int c_dir;
            internal int c_num;
            internal double c_la;
            internal double z_dist;
        }

        internal delegate void EventFunc(IntPtr lParam);

    

 //__________ General Functions ________________________________________________//
 
 //COMIDAS_DEVID deviceID
		[DllImport("Comidll.dll", EntryPoint = "COMI_LoadDevice")]
		internal static extern unsafe IntPtr	COMI_LoadDevice([MarshalAs(UnmanagedType.I4)] int deviceID, [MarshalAs(UnmanagedType.I4)] int instance);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_UnloadDevice")]
		internal static extern unsafe void	COMI_UnloadDevice(IntPtr hDevice /*HANDLE*/);
        
// TComiDevList
		[DllImport("Comidll.dll", EntryPoint = "COMI_GetAvailDevList")]
		internal static extern unsafe int	COMI_GetAvailDevList(ref TComiDevList pDevList);
        
// TComiDevInfo
		[DllImport("Comidll.dll", EntryPoint = "COMI_GetDevInfo")]
		internal static extern unsafe int	COMI_GetDevInfo(IntPtr hDevice /*HANDLE*/, ref TComiDevInfo pDevInfo);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_Write8402")]
		internal static extern unsafe void	COMI_Write8402(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  ch, [MarshalAs(UnmanagedType.I4)] int  addr, [MarshalAs(UnmanagedType.I4)] int  data);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_WriteEEPR")]
		internal static extern unsafe void	COMI_WriteEEPR(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  addr, [MarshalAs(UnmanagedType.I4)] int  data);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_ReadEEPR")]
		internal static extern unsafe int		COMI_ReadEEPR(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  addr);
 
 //__________ A/D General Functions ________________________________________________//
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_AD_SetRange")]
		internal static extern unsafe int	COMI_AD_SetRange(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  ch, [MarshalAs(UnmanagedType.R4)] float vmin, [MarshalAs(UnmanagedType.R4)] float vmax);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_AD_GetDigit")]
		internal static extern unsafe short	COMI_AD_GetDigit(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  ch);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_AD_GetVolt")]
		internal static extern unsafe float	COMI_AD_GetVolt(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  ch);
 
 //__________ A/D Unlimited Scan Functions _________________________________//
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_US_Start")]
		internal static extern unsafe int	COMI_US_Start(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  numCh, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I4)] int[] chanList, [MarshalAs(UnmanagedType.I4)] int scanFreq, [MarshalAs(UnmanagedType.I4)] int msb, [MarshalAs(UnmanagedType.I4)] int  trsMethod);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_US_StartEx")]
		internal static extern unsafe int	COMI_US_StartEx(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int dwScanFreq, [MarshalAs(UnmanagedType.I4)] int nFrameSize, [MarshalAs(UnmanagedType.I4)] int nBufSizeGain);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_US_Stop")]
		internal static extern unsafe int	COMI_US_Stop(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int bReleaseBuf);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_US_SetPauseAtFull")]
		internal static extern unsafe int	COMI_US_SetPauseAtFull(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int bPauseAtFull);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_US_Resume")]
		internal static extern unsafe int	COMI_US_Resume(IntPtr hDevice /*HANDLE*/);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_US_ChangeScanFreq")]
		internal static extern unsafe int	COMI_US_ChangeScanFreq(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int dwScanFreq);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_US_ResetCount")]
		internal static extern unsafe void	COMI_US_ResetCount(IntPtr hDevice /*HANDLE*/);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_US_ChangeSampleFreq")]
		internal static extern unsafe void	COMI_US_ChangeSampleFreq(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int dwSampleFreq);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_US_CurCount")]
		internal static extern unsafe int	COMI_US_CurCount(IntPtr hDevice /*HANDLE*/);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_US_SBPos")]
		internal static extern unsafe int	COMI_US_SBPos(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  chOrder, [MarshalAs(UnmanagedType.I4)] int scanCount);

// short*        
		[DllImport("Comidll.dll", EntryPoint = "COMI_US_GetBufPtr")]
		internal static extern unsafe short[]	COMI_US_GetBufPtr(IntPtr hDevice /*HANDLE*/);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_US_ReleaseBuf")]
		internal static extern unsafe int	COMI_US_ReleaseBuf(IntPtr hDevice /*HANDLE*/);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_US_RetrvOne")]
		internal static extern unsafe int	COMI_US_RetrvOne(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  chOrder, [MarshalAs(UnmanagedType.I4)] int scanCount);

//TCOmiVarType VarType        
		[DllImport("Comidll.dll", EntryPoint = "COMI_US_RetrvChannel")]
        internal static extern unsafe int COMI_US_RetrvChannel(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int chOrder, [MarshalAs(UnmanagedType.I4)] int startCount, [MarshalAs(UnmanagedType.I4)] int maxNumData, IntPtr Buffer, [MarshalAs(UnmanagedType.I4)] int VarType);

//TCOmiVarType VarType  
		[DllImport("Comidll.dll", EntryPoint = "COMI_US_RetrvBlock")]
        internal static extern unsafe int COMI_US_RetrvBlock(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int startCount, [MarshalAs(UnmanagedType.I4)] int maxNumScan, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.R8)] IntPtr Buffer, [MarshalAs(UnmanagedType.I4)] int VarType);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_US_FileSaveFirst")]
		internal static extern unsafe int	COMI_US_FileSaveFirst(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.LPStr)] string szFilePath, [MarshalAs(UnmanagedType.I4)] int bIsFromStart);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_US_FileSaveNext")]
		internal static extern unsafe int	COMI_US_FileSaveNext(IntPtr hDevice /*HANDLE*/);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_US_FileSaveStop")]
		internal static extern unsafe int	COMI_US_FileSaveStop(IntPtr hDevice /*HANDLE*/);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_US_FileConvert")]
		internal static extern unsafe void	COMI_US_FileConvert([MarshalAs(UnmanagedType.LPStr)] string szBinFilePath, [MarshalAs(UnmanagedType.LPStr)] string szTextFilePath, [MarshalAs(UnmanagedType.I4)] int nMaxDataRow);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_US_CheckFileConvert")]
		internal static extern unsafe double	COMI_US_CheckFileConvert();
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_US_CancelFileConvert")]
		internal static extern unsafe void	COMI_US_CancelFileConvert();
 
 //___________ PID Functions _______________________________________________//
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_PID_Enable")]
		internal static extern unsafe int	COMI_PID_Enable(IntPtr hDevice /*HANDLE*/); 

// TPidParams pPidParams       
		[DllImport("Comidll.dll", EntryPoint = "COMI_PID_SetParams")]
		internal static extern unsafe int	COMI_PID_SetParams(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  nNumCtrls, ref TPidParams pPidParams); 
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_PID_Disable")]
		internal static extern unsafe int	COMI_PID_Disable(IntPtr hDevice /*HANDLE*/); 
 
 //___________ DIO Common __________________________________________________//
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_DIO_SetUsage")]
		internal static extern unsafe void	COMI_DIO_SetUsage(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  usage);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_DIO_GetUsage")]
		internal static extern unsafe int		COMI_DIO_GetUsage(IntPtr hDevice /*HANDLE*/);
 
 //__________ D/I Functions ________________________________________________//
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_DI_GetOne")]
		internal static extern unsafe int		COMI_DI_GetOne(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  ch);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_DI_GetAll")]
		internal static extern unsafe int	COMI_DI_GetAll(IntPtr hDevice /*HANDLE*/);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_DI_GetAllEx")]
		internal static extern unsafe int	COMI_DI_GetAllEx(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  nGroupIdx);
 
 //__________ D/O Functions ________________________________________________//
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_DO_PutOne")]
		internal static extern unsafe int	COMI_DO_PutOne(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  ch, [MarshalAs(UnmanagedType.I4)] int  status);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_DO_PutAll")]
		internal static extern unsafe int	COMI_DO_PutAll(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int dwStatuses);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_DO_PutAllEx")]
		internal static extern unsafe void	COMI_DO_PutAllEx(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  nGroupIdx, [MarshalAs(UnmanagedType.I4)] int dwStatuses);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_DO_GetOne")]
		internal static extern unsafe int		COMI_DO_GetOne(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  ch);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_DO_GetAll")]
		internal static extern unsafe int	COMI_DO_GetAll(IntPtr hDevice /*HANDLE*/);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_DO_GetAllEx")]
		internal static extern unsafe int	COMI_DO_GetAllEx(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  nGroupIdx);
 
 //__________ D/A Functions ________________________________________________//
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_DA_Out")]
		internal static extern unsafe int	COMI_DA_Out(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  ch, [MarshalAs(UnmanagedType.R4)] float volt);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_DA_SetRange")]
		internal static extern unsafe void	COMI_DA_SetRange(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  ch, [MarshalAs(UnmanagedType.I4)] int  VMin, [MarshalAs(UnmanagedType.I4)] int  VMax);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_WFM_Start")]
		internal static extern unsafe int	COMI_WFM_Start(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  ch, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.R8)] ref double Buffer, [MarshalAs(UnmanagedType.I4)] int nNumData, [MarshalAs(UnmanagedType.I4)] int nPPS, [MarshalAs(UnmanagedType.I4)] int  nMaxLoops);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_WFM_RateChange")]
		internal static extern unsafe int	COMI_WFM_RateChange(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  ch, [MarshalAs(UnmanagedType.I4)] int nPPS);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_WFM_GetCurPos")]
		internal static extern unsafe int	COMI_WFM_GetCurPos(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  ch);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_WFM_GetCurLoops")]
		internal static extern unsafe int	COMI_WFM_GetCurLoops(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  ch);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_WFM_Stop")]
		internal static extern unsafe void	COMI_WFM_Stop(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  ch);
 
 //__________ Counter Functions ____________________________________________//
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_SetCounter")]
		internal static extern unsafe void	COMI_SetCounter(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  ch, [MarshalAs(UnmanagedType.I4)] int  rw, [MarshalAs(UnmanagedType.I4)] int  op, [MarshalAs(UnmanagedType.I4)] int  bcd_bin, [MarshalAs(UnmanagedType.I4)] int load_value);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_LoadCount")]
		internal static extern unsafe void	COMI_LoadCount(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  ch, [MarshalAs(UnmanagedType.I4)] int load_value);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_ReadCount")]
		internal static extern unsafe int	COMI_ReadCount(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  ch);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_ReadCounter32")]
		internal static extern unsafe int 	COMI_ReadCounter32(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  ch);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_ClearCounter32")]
		internal static extern unsafe void 	COMI_ClearCounter32(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  ch);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_FC_SelectGate")]
		internal static extern unsafe void	COMI_FC_SelectGate(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  ch, [MarshalAs(UnmanagedType.I4)] int  nGateIndex);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_FC_GateTime")]
		internal static extern unsafe int	COMI_FC_GateTime(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  ch);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_FC_ReadCount")]
		internal static extern unsafe int	COMI_FC_ReadCount(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  ch);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_FC_ReadFreq")]
		internal static extern unsafe int	COMI_FC_ReadFreq(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  ch);
 
 
		[DllImport("Comidll.dll", EntryPoint = "COMI_ENC_Config")]
		internal static extern unsafe void	COMI_ENC_Config(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  ch, [MarshalAs(UnmanagedType.I4)] int  mode, [MarshalAs(UnmanagedType.I4)] int bResetByZ);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_ENC_Reset")]
		internal static extern unsafe void	COMI_ENC_Reset(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  ch);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_ENC_Load")]
		internal static extern unsafe void	COMI_ENC_Load(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  ch, [MarshalAs(UnmanagedType.I4)] int Count);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_ENC_Read")]
		internal static extern unsafe int	COMI_ENC_Read(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  ch);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_ENC_ResetZ")]
		internal static extern unsafe void	COMI_ENC_ResetZ(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  ch);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_ENC_LoadZ")]
		internal static extern unsafe void	COMI_ENC_LoadZ(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  ch,  [MarshalAs(UnmanagedType.I4)] int  count);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_ENC_ReadZ")]
		internal static extern unsafe int	COMI_ENC_ReadZ(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  ch);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_PG_Start")]
		internal static extern unsafe double	COMI_PG_Start(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  ch, [MarshalAs(UnmanagedType.R8)] double freq, [MarshalAs(UnmanagedType.I4)] int num_pulses);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_PG_ChangeFreq")]
		internal static extern unsafe double	COMI_PG_ChangeFreq(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  ch, [MarshalAs(UnmanagedType.R8)] double freq);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_PG_IsActive")]
		internal static extern unsafe int	COMI_PG_IsActive(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  ch);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_PG_Stop")]
		internal static extern unsafe void	COMI_PG_Stop(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  ch);

        
		[DllImport("Comidll.dll", EntryPoint = "COMI_SD502_SetCounter")]
		internal static extern unsafe void	COMI_SD502_SetCounter(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  ch, [MarshalAs(UnmanagedType.I4)] int  nMode, [MarshalAs(UnmanagedType.I4)] int nClkSource);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_SD502_ReadNowCount")]
		internal static extern unsafe int	COMI_SD502_ReadNowCount(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  ch);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_SD502_ReadOldCount")]
		internal static extern unsafe int	COMI_SD502_ReadOldCount(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  ch);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_SD502_GetGateState")]
		internal static extern unsafe int	COMI_SD502_GetGateState(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  ch);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_SD502_GetClkFreq")]
		internal static extern unsafe double	COMI_SD502_GetClkFreq([MarshalAs(UnmanagedType.I4)] int nClkSrcIdx);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_SD502_Clear")]
		internal static extern unsafe void	COMI_SD502_Clear(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int  ch);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_SD502_ClearMulti")]
		internal static extern unsafe void	COMI_SD502_ClearMulti(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int dwCtrlBits);

 //__________ Utility Functions ____________________________________________//
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_DigitToVolt")]
		internal static extern unsafe double	COMI_DigitToVolt([MarshalAs(UnmanagedType.I4)] int digit, [MarshalAs(UnmanagedType.R8)] double vmin, [MarshalAs(UnmanagedType.R8)] double vmax);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_Digit14ToVolt")]
		internal static extern unsafe double	COMI_Digit14ToVolt([MarshalAs(UnmanagedType.I4)] int digit, [MarshalAs(UnmanagedType.R8)] double vmin, [MarshalAs(UnmanagedType.R8)] double vmax);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_Digit16ToVolt")]
		internal static extern unsafe double	COMI_Digit16ToVolt([MarshalAs(UnmanagedType.I4)] int digit, [MarshalAs(UnmanagedType.R8)] double vmin, [MarshalAs(UnmanagedType.R8)] double vmax);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_LastError")]
		internal static extern unsafe int		COMI_LastError();
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_ErrorString")]
		internal static extern unsafe string COMI_ErrorString([MarshalAs(UnmanagedType.I4)] int nErrCode);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_GetResources")]
		internal static extern unsafe void	COMI_GetResources(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I4)] int[]  pdwIntVect, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I4)] int[] pdwIoPorts, [MarshalAs(UnmanagedType.I4)] int  nNumPorts, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I4)] int[] pdwMemPorts, [MarshalAs(UnmanagedType.I4)] int  nNumMemPorts);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_WriteIoPortDW")]
		internal static extern unsafe void	COMI_WriteIoPortDW(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int dwPortBase, [MarshalAs(UnmanagedType.I4)] int nOffset, [MarshalAs(UnmanagedType.I4)] int dwOutVal);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_ReadIoPortDW")]
		internal static extern unsafe int	COMI_ReadIoPortDW(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int dwPortBase, [MarshalAs(UnmanagedType.I4)] int nOffset);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_WriteMemPortDW")]
		internal static extern unsafe void	COMI_WriteMemPortDW(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int dwPortBase, [MarshalAs(UnmanagedType.I4)] int nOffset, [MarshalAs(UnmanagedType.I4)] int dwOutVal);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_ReadMemPortDW")]
		internal static extern unsafe int	COMI_ReadMemPortDW(IntPtr hDevice /*HANDLE*/, [MarshalAs(UnmanagedType.I4)] int dwPortBase, [MarshalAs(UnmanagedType.I4)] int nOffset);
        
		[DllImport("Comidll.dll", EntryPoint = "COMI_GotoURL")]
		internal static extern unsafe void	COMI_GotoURL([MarshalAs(UnmanagedType.LPStr)] string szUrl, [MarshalAs(UnmanagedType.I4)] int bMaximize);
    }
}

