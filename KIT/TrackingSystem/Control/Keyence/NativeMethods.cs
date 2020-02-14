//------------------------------------------------------------------------------ 
// <copyright file="NativeMethods.cs" company="KEYENCE">
//     Copyright (c) 2012 KEYENCE CORPORATION.  All rights reserved.
// </copyright>
//----------------------------------------------------------------------------- 
using System.Runtime.InteropServices;

namespace Control
{
	public enum Rc
	{
		Ok = 0x0000,
		ErrOpen = 0x1000,
		ErrNotOpen,
		ErrSend,
		ErrReceive,
		ErrTimeout,
		ErrNoMemory,
		ErrParameter,
		ErrRecvFmt,
		ErrOpenYet = 0x100A,
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct LS9IF_ETHERNET_CONFIG
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public byte[] abyIpAddress;
		public ushort wPortNo;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public byte[] reserve;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct LS9IF_TARGET_SETTING
	{
		public byte byType;
		public byte byCategory;
		public byte byItem;
		public byte reserve;
		public byte byTarget;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct LS9IF_TIME
	{
		public byte byYear;
		public byte byMonth;
		public byte byDay;
		public byte byHour;
		public byte byMinute;
		public byte bySecond;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public byte[] reserve;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct LS9IF_MEASURE_VALUE
	{
		public byte byDataInfo;
		public byte byJudge;
		public byte byTimZero;
		public float fValue;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct LS9IF_MEASURE_DATA
	{
		public byte byYear;
		public byte byMonth;
		public byte byDay;
		public byte byHour;
		public byte byMinute;
		public byte bySecond;
		public byte byMillsecond;
		public uint dwPulseCnt;
		public byte byTotalJudge;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public LS9IF_MEASURE_VALUE[] stMesureValue;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct LS9IF_STAT_SAMP
	{
		public byte byStatus;
		public float fAverage;
		public float fMaximum;
		public float fMinimum;
		public float fMax_Min;
		public float fStdDeviation;
		public uint dwDenominator;
		public uint dwHH_Count;
		public uint dwHI_Count;
		public uint dwGO_Count;
		public uint dwLO_Count;
		public uint dwLL_Count;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct LS9IF_STORAGE_INFO
	{
		public byte byStatus;
		public byte reserve;
		public ushort wStrageVer;
		public uint dwStorageCnt;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct LS9IF_STORAGE_VALUE
	{
		public byte byDataInfo;
		public byte byJudge;
		public float fValue;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct LS9IF_STORAGE_DATA
	{
		public byte byYear;
		public byte byMonth;
		public byte byDay;
		public byte byHour;
		public byte byMinute;
		public byte bySecond;
		public byte byMillsecond;
		public byte byDstFlg;
		public uint dwPulseCnt;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public LS9IF_STORAGE_VALUE[] stStorageValue;
	}

	public static class NativeMethods
	{
		[DllImport("Dll\\LS9_IF.dll")]
		public static extern uint LS9IF_GetVersion();

		[DllImport("Dll\\LS9_IF.dll")]
		public static extern int LS9IF_UsbOpen();

		[DllImport("Dll\\LS9_IF.dll")]
		public static extern int LS9IF_EthernetOpen(ref LS9IF_ETHERNET_CONFIG pstEthernetConfig);

		[DllImport("Dll\\LS9_IF.dll")]
		public static extern int LS9IF_CommClose();

		[DllImport("Dll\\LS9_IF.dll")]
		public static extern int LS9IF_RebootController();

		[DllImport("Dll\\LS9_IF.dll")]
		public static extern int LS9IF_RetrunToFactorySetting();

		[DllImport("Dll\\LS9_IF.dll")]
		public static extern int LS9IF_StatSampStart(byte byStatCh);

		[DllImport("Dll\\LS9_IF.dll")]
		public static extern int LS9IF_StatSampStop(byte byStatCh);

		[DllImport("Dll\\LS9_IF.dll")]
		public static extern int LS9IF_StatSampClear(byte byStatCh);

		[DllImport("Dll\\LS9_IF.dll")]
		public static extern int LS9IF_AutoZero(byte byOnOff, uint dwOut);

		[DllImport("Dll\\LS9_IF.dll")]
		public static extern int LS9IF_Timing(byte byOnOff, uint dwOut);

		[DllImport("Dll\\LS9_IF.dll")]
		public static extern int LS9IF_Reset(uint dwOut);

		[DllImport("Dll\\LS9_IF.dll")]
		public static extern int LS9IF_SyncAutoZero(byte byOnOff);

		[DllImport("Dll\\LS9_IF.dll")]
		public static extern int LS9IF_SyncTiming(byte byOnOff);

		[DllImport("Dll\\LS9_IF.dll")]
		public static extern int LS9IF_SyncReset();

		[DllImport("Dll\\LS9_IF.dll")]
		public static extern int LS9IF_ClearMemory();

		[DllImport("Dll\\LS9_IF.dll")]
		public static extern int LS9IF_SetSetting(byte byDepth, 
											LS9IF_TARGET_SETTING stTargetSetting,
											int lDataSize,
											[In, Out] byte[] pbyDatas, 
											ref uint pdwError);

		/// <returns>@@@ リターンコード</returns>
		[DllImport("Dll\\LS9_IF.dll")]
		public static extern int LS9IF_GetSetting(byte byDepth,
											LS9IF_TARGET_SETTING stTargetSetting,
											[In, Out] byte[] pbyDatas,
											ref int plDataSize);

		[DllImport("Dll\\LS9_IF.dll")]
		public static extern int LS9IF_ReflectSetting(byte byDepth, ref uint pdwError);

		[DllImport("Dll\\LS9_IF.dll")]
		public static extern int LS9IF_RewriteTemporarySetting(byte byDepth);

		[DllImport("Dll\\LS9_IF.dll")]
		public static extern int LS9IF_CheckMemoryAccess(ref byte pbyBusy);

		[DllImport("Dll\\LS9_IF.dll")]
		public static extern int LS9IF_SetTime(LS9IF_TIME stTime);

		[DllImport("Dll\\LS9_IF.dll")]
		public static extern int LS9IF_GetTime(ref LS9IF_TIME stTime);

		[DllImport("Dll\\LS9_IF.dll")]
		public static extern int LS9IF_ChangeActiveProgram(byte byProgNo);

		[DllImport("Dll\\LS9_IF.dll")]
		public static extern int LS9IF_GetActiveProgram(ref byte pbyProgNo);

		[DllImport("Dll\\LS9_IF.dll")]
		public static extern int LS9IF_LightCalib(byte byHead, byte byMode, byte byTarget);

		[DllImport("Dll\\LS9_IF.dll")]
		public static extern int LS9IF_ConfirmLightCalib(ref byte pbyBusyFlg, ref byte pbyErrCode);

		[DllImport("Dll\\LS9_IF.dll")]
		public static extern int LS9IF_GetMeasurementValue(ref LS9IF_MEASURE_DATA pstMeasureData);

		[DllImport("Dll\\LS9_IF.dll")]
		public static extern int LS9IF_GetStatSamp(byte byStatCh, ref LS9IF_STAT_SAMP pstStatSamp);

		[DllImport("Dll\\LS9_IF.dll")]
		public static extern int LS9IF_StartStorage();

		[DllImport("Dll\\LS9_IF.dll")]
		public static extern int LS9IF_StopStorage();

		[DllImport("Dll\\LS9_IF.dll")]
		public static extern int LS9IF_GetStorageStatus(ref LS9IF_STORAGE_INFO pstStorageInfo);

		[DllImport("Dll\\LS9_IF.dll")]
		public static extern int LS9IF_GetStorageData(uint dwStartPoint, uint dwReadCount, [In, Out] LS9IF_STORAGE_DATA[] pstStorageData);
	}
};
