using System;
using System.Runtime.InteropServices;

namespace Shared
{
	/// <summary>
	/// VC6.0 ���� ���۵� ceSDKDLL.dll ������ CSharp ���� ����ϱ� ���� �Լ� �������Դϴ�.
	/// </summary>
    public unsafe class CEDLL
    {
        //====================== General FUNCTIONS ====================================================//
        // 1. ceGnLoad
        // ���̺귯���� �ε�� ���¿��� ��ġ�� �ε�.
        [DllImport("ceSDKDLL.dll", EntryPoint = "ceGnLoad", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern unsafe int ceGnLoad();

        // 2. ceGnUnload
        // ���̺귯���� �ε�� ���¿��� ��ġ�� ��ε�.
        [DllImport("ceSDKDLL.dll", EntryPoint = "ceGnUnload", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern unsafe int ceGnUnload();

        // 3. ceGnReSearchDevice
				// ���̺귯���� �ε�� ���¿��� ��带 �ٽ� Ž���մϴ�.
				// [RealNode: ���� ��� ����, nTimeout: Ÿ�� �ƿ�, IsBlocking: �Ϸ�ɶ����� �޼��� ��� ����, pResultNode: �ε�� ��� �� ��ȯ]
        [DllImport("ceSDKDLL.dll", EntryPoint = "ceGnReSearchDevice", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern unsafe int ceGnReSearchDevice([MarshalAs(UnmanagedType.I4)]int RealNode, [MarshalAs(UnmanagedType.U4)] uint nTimeout, [MarshalAs(UnmanagedType.I4)]int IsBlocking, [MarshalAs(UnmanagedType.I4)] ref int pResultNode);

        // 4. ceGnIsSearchedDevice
				// ��� Ž���� �Ϸ�Ǿ� �ִ��� Ȯ���մϴ�. ���� ���μ������� �ٸ� ���μ������� �̹� ��尡 Ž���Ǿ��ٸ�, �� �Լ��� ����
				// �̹� Ž���� ��带 ������� �� Ž���� �ϰų� ���� �ʵ��� �� �� �ֽ��ϴ�.
				// [IsSearchedDevice: �̹� ��� Ž���� �Ǿ� �ִ� ��� TRUE �� ��ȯ�ϸ�, Ž���� �Ǿ� ���� ���� ��� FALSE �� ��ȯ�մϴ�]  
        [DllImport("ceSDKDLL.dll", EntryPoint = "ceGnIsSearchedDevice", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern unsafe int ceGnIsSearchedDevice([MarshalAs(UnmanagedType.I4)] ref int pIsSearchedDevice);

        // 5. ceGnSearchDevice
				// ��ġ�� �ε�� ���¿��� ��ġ�� ȯ�� ������ ��ȯ.
				// [RealNode: ���� ��� ����, nTimeout: Ÿ�� �ƿ�, IsBlocking: �Ϸ�ɶ����� �޼��� ��� ����, pResultNode: ��ü ��� �� ��ȯ]
        [DllImport("ceSDKDLL.dll", EntryPoint = "ceGnSearchDevice", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern unsafe int ceGnSearchDevice([MarshalAs(UnmanagedType.I4)]int RealNode, [MarshalAs(UnmanagedType.I4)]uint nTimeout, [MarshalAs(UnmanagedType.I4)]int IsBlocking, [MarshalAs(UnmanagedType.I4)] ref int pResultNode);

        // 6. ceGnUnSearchDevice
				// ��� Ž���� �ʱ�ȭ �ϸ�, �� �Լ��� ����� ���� ceGnSearchDevice �� ���� ��� Ž���� �� �� �ֽ��ϴ�.
        [DllImport("ceSDKDLL.dll", EntryPoint = "ceGnUnSearchDevice", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern unsafe int ceGnUnSearchDevice();

        // 7. ceGnTotalNode
				// �ε�� ��ü ����� ������ ��ȯ.
				// [Node : �ε�� ��� �� ��ȯ]
        [DllImport("ceSDKDLL.dll", EntryPoint = "ceGnTotalNode", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern unsafe int ceGnTotalNode([MarshalAs(UnmanagedType.I4)] ref int pNumNodes);

        // 7-2. ceGnTotalMotionChannel
        // �ε�� ��� �� ������ ��ȯ.
        // [Channel : �ε�� ��� �� ����]
        [DllImport("ceSDKDLL.dll", EntryPoint = "ceGnTotalMotionChannel", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern unsafe int ceGnTotalMotionChannel([MarshalAs(UnmanagedType.I4)] ref int pNumChannels);

        // 8. ceGnTotalDIOChannel
				// �ε�� ������ ����� ä�� ������ ��ȯ.
				// [Channel : �ε�� ������ ����� ä�� ���� ��ȯ]
        [DllImport("ceSDKDLL.dll", EntryPoint = "ceGnTotalDIOChannel", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern unsafe int ceGnTotalDIOChannel([MarshalAs(UnmanagedType.I4)] ref int pNumChannels);

        // 9. ceGnTotalAIChannel
				// �ε�� �Ƴ��α� �Է� ä�� ������ ��ȯ.
				// [Channel : �ε�� �Ƴ��α� �Է� ä�� ���� ��ȯ]
        [DllImport("ceSDKDLL.dll", EntryPoint = "ceGnTotalAIChannel", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern unsafe int ceGnTotalAIChannel([MarshalAs(UnmanagedType.I4)] ref int pNumChannels);

        // 10. ceGnTotalAOChannel
				// �ε�� �Ƴ��α� ��� ä�� ������ ��ȯ.
				// [Channel : �ε�� �Ƴ��α� ��� ä�� ���� ��ȯ]
        [DllImport("ceSDKDLL.dll", EntryPoint = "ceGnTotalAOChannel", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern unsafe int ceGnTotalAOChannel([MarshalAs(UnmanagedType.I4)] ref int pNumChannels);

        // 11. ceGnModuleCount_Dio
				// �ش� ����� ������ ����� ��� ������ ��ȯ.
				// [NodeID : ��� ��ȣ, ModuleCount : �ش� ����� ������ ����� ��� ���� ��ȯ]
        [DllImport("ceSDKDLL.dll", EntryPoint = "ceGnModuleCount_Dio", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern unsafe int ceGnModuleCount_Dio([MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)] ref int pNumModules);

        // 12. ceGnModuleCount_Ai
       	// �ش� ����� �Ƴ��α� �Է� ��� ������ ��ȯ.
				// [NodeID : ��� ��ȣ, ModuleCount : �ش� ����� �Ƴ��α� �Է� ��� ���� ��ȯ]
        [DllImport("ceSDKDLL.dll", EntryPoint = "ceGnModuleCount_Ai", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern unsafe int ceGnModuleCount_Ai([MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)] ref int pNumModules);

        // 13. ceGnModuleCount_Ao
				// �ش� ����� �Ƴ��α� ��� ��� ������ ��ȯ.
				// [NodeID : ��� ��ȣ, ModuleCount : �ش� ����� �Ƴ��α� ��� ��� ���� ��ȯ]
        [DllImport("ceSDKDLL.dll", EntryPoint = "ceGnModuleCount_Ao", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern unsafe int ceGnModuleCount_Ao([MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)] ref int pNumModules);

        // 14. ceGnChannelCount_Dio
				// �ش� ����� ������ ����� ��⿡ ���� ä�� ������ ��ȯ .
				// [NodeID : ��� ��ȣ, ModuleIdx : ������ ����� ��� ��ȣ, ChannelCount : ������ ����� ä�� ���� ��ȯ]
        [DllImport("ceSDKDLL.dll", EntryPoint = "ceGnChannelCount_Dio", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern unsafe int ceGnChannelCount_Dio([MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)]int ModuleIdx, [MarshalAs(UnmanagedType.I4)] ref int pNumChannels);

        // 15. ceGnChannelCount_Ai
				// �ش� ����� �Ƴ��α� �Է� ��⿡ ���� ä�� ������ ��ȯ.
				// [NodeID : ��� ��ȣ, ModuleIdx : �Ƴ��α� �Է� ��� ��ȣ, ChannelCount : �Ƴ��α� �Է� ä�� ���� ��ȯ]
        [DllImport("ceSDKDLL.dll", EntryPoint = "ceGnChannelCount_Ai", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern unsafe int ceGnChannelCount_Ai([MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)]int ModuleIdx, [MarshalAs(UnmanagedType.I4)] ref int pNumChannels);

        // 16. ceGnChannelCount_Ao
				// �ش� ����� �Ƴ��α� ��� ��⿡ ���� ä�� ������ ��ȯ.
				// [NodeID : ��� ��ȣ, ModuleIdx : �Ƴ��α� ��� ��� ��ȣ, ChannelCount : �Ƴ��α� ��� ä�� ���� ��ȯ]
        [DllImport("ceSDKDLL.dll", EntryPoint = "ceGnChannelCount_Ao", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern unsafe int ceGnChannelCount_Ao([MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)]int ModuleIdx, [MarshalAs(UnmanagedType.I4)] ref int pNumChannels);

        // 17. ceGnNodeIsActive
				// �ش� ��尡 ����Ǿ� �ִ��� �����Ǿ� �ִ��� Ȯ���ϴ� �Լ�
				// [NodeID : ��� ���, IsActive : ���� Ȥ�� ���� ����]
        [DllImport("ceSDKDLL.dll", EntryPoint = "ceGnNodeIsActive", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern unsafe int ceGnNodeIsActive([MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)] ref int pIsActive);

        // 18. ceGnLocalDIO_Get
				// �ش� ������ I/O ä�ο� ���� ��� �� ��� ������ ��ȯ.
				// [Channel : ���� ������ ����� ä�� ��ȣ, NodeIP : ��� IP �ּ� ��ȯ, NodeID : ��� ��ȣ ��ȯ, NodeInGlobal : �ش� ����� ���� ������ ����� ä�� ��ȣ ��ȯ, ModuleIdx : �ش� ����� ��� ��ȣ ��ȯ, ModuleInCh : ��� �� ������ ����� ä�� ��ȣ ��ȯ]
				//EXTERN LONG (__stdcall *ceGnLocalDIO_Get)	(__in LONG Channel,  __out PLONG NodeIP, __out PLONG NodeID, __out PLONG NodeInGlobal, __out PLONG ModuleIdx, __out PLONG ModuleInCh);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceGnLocalDIO_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceGnLocalDIO_Get([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)] ref int NodeIP, [MarshalAs(UnmanagedType.I4)] ref int NodeID, 
					[MarshalAs(UnmanagedType.I4)] ref int NodeInGlobal, [MarshalAs(UnmanagedType.I4)] ref int ModuleIdx, 
					[MarshalAs(UnmanagedType.I4)] ref int ModuleInCh);
			
        // 19. ceGnLocalAI_Get			
				// �ش� �Ƴ��α� �Է� ä�ο� ���� ��� �� ��� ������ ��ȯ.
				// [Channel: ���� �Ƴ��α� �Է� ä�� ��ȣ, NodeIP : ��� IP �ּ� ��ȯ, NodeID : ��� ��ȣ ��ȯ, NodeInGlobal : �ش� ����� ���� �Ƴ��α� �Է� ä�� ��ȣ ��ȯ, ModuleIdx : �ش� ����� ��� ��ȣ ��ȯ, ModuleInCh : ��� �� �Ƴ��α� �Է� ä�� ��ȣ ��ȯ]
				//EXTERN LONG (__stdcall *ceGnLocalAI_Get)	(__in LONG Channel,  __out PLONG NodeIP, __out PLONG NodeID, __out PLONG NodeInGlobal, __out PLONG ModuleIdx, __out PLONG ModuleInCh);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceGnLocalAI_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceGnLocalAI_Get([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)] ref int NodeIP, [MarshalAs(UnmanagedType.I4)] ref int NodeID,
					[MarshalAs(UnmanagedType.I4)] ref int NodeInGlobal, [MarshalAs(UnmanagedType.I4)] ref int ModuleIdx,
					[MarshalAs(UnmanagedType.I4)] ref int ModuleInCh);
			
        // 20. ceGnLocalAO_Get						
				// �ش� �Ƴ��α� ��� ä�ο� ���� ��� �� ��� ������ ��ȯ.
				// [Channel: ���� �Ƴ��α� ��� ä�� ��ȣ, NodeIP : ��� IP �ּ� ��ȯ, NodeID : ��� ��ȣ ��ȯ, NodeInGlobal : �ش� ����� ���� �Ƴ��α� ��� ä�� ��ȣ ��ȯ, ModuleIdx : �ش� ����� ��� ��ȣ ��ȯ, ModuleInCh : ��� �� �Ƴ��α� ��� ä�� ��ȣ ��ȯ]
				//EXTERN LONG (__stdcall *ceGnLocalAO_Get)	(__in LONG Channel, __out PLONG NodeIP,  __out PLONG NodeID, __out PLONG NodeInGlobal, __out PLONG ModuleIdx, __out PLONG ModuleInCh);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceGnLocalAO_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceGnLocalAO_Get([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)] ref int NodeIP, [MarshalAs(UnmanagedType.I4)] ref int NodeID,
					[MarshalAs(UnmanagedType.I4)] ref int NodeInGlobal, [MarshalAs(UnmanagedType.I4)] ref int ModuleIdx, 
					[MarshalAs(UnmanagedType.I4)] ref int ModuleInCh);
			
				// �ش� ��� ������ ����� ä�ο� ���� ��� �� ��� ������ ��ȯ.
				// [Channel: ���� ī���� ä�� ��ȣ, NodeIP : ��� IP �ּ� ��ȯ, NodeID : ��� ��ȣ ��ȯ, NodeInGlobal : �ش� ����� ���� ��� ������ ����� ä�� ��ȣ ��ȯ, ModuleIdx : �ش� ����� ��� ��ȣ ��ȯ, ModuleInCh : ��� �� ��� ������ �� ��� ä�� ��ȣ ��ȯ]
				//EXTERN LONG (__stdcall *ceGnLocalMDIO_Get)   (__in LONG Channel, __out PLONG NodeIP,  __out PLONG NodeID, __out PLONG NodeInGlobal, __out PLONG ModuleIdx, __out PLONG ModuleInCh);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceGnLocalMDIO_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceGnLocalMDIO_Get([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)] ref int NodeIP, [MarshalAs(UnmanagedType.I4)] ref int NodeID, 
					[MarshalAs(UnmanagedType.I4)] ref int NodeInGlobal, [MarshalAs(UnmanagedType.I4)] ref int ModuleIdx, 
					[MarshalAs(UnmanagedType.I4)] ref int ModuleInCh);
			
				// �ش� ī���� ä�ο� ���� ��� �� ��� ������ ��ȯ.
				// [Channel: ���� ī���� ä�� ��ȣ, NodeIP : ��� IP �ּ� ��ȯ, NodeID : ��� ��ȣ ��ȯ, NodeInGlobal : �ش� ����� ���� ī���� ä�� ��ȣ ��ȯ, ModuleIdx : �ش� ����� ��� ��ȣ ��ȯ, ModuleInCh : ��� �� ī���� ��� ä�� ��ȣ ��ȯ]
				//EXTERN LONG (__stdcall *ceGnLocalCNT_Get)   (__in LONG Channel, __out PLONG NodeIP,  __out PLONG NodeID, __out PLONG NodeInGlobal, __out PLONG ModuleIdx, __out PLONG ModuleInCh);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceGnLocalCNT_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceGnLocalCNT_Get([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)] ref int NodeIP, [MarshalAs(UnmanagedType.I4)] ref int NodeID, 
					[MarshalAs(UnmanagedType.I4)] ref int NodeInGlobal, [MarshalAs(UnmanagedType.I4)] ref int ModuleIdx, 
					[MarshalAs(UnmanagedType.I4)] ref int ModuleInCh);
			
				// �ش� ��� ��� ����� ���� �� ��ȣ�� ���Ͽ� ���� �� ��ȣ�� ��ȯ.
				// [NodeID : ��� ��ȣ, ModuleIdx : �ش� ����� ��� ��ȣ, ModuleInCh : ��� �� ��� ���� �� ��ȣ, GlobalAxis : ���� ��� ���� �� ��ȣ ��ȯ]
				//EXTERN LONG (__stdcall *ceGnGlobalAxis_Get)	(__in LONG NodeID, __in  LONG ModuleIdx, __in  LONG ModuleInCh, __out PLONG GlobalAxis);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceGnGlobalAxis_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceGnGlobalAxis_Get([MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)]int ModuleIdx, [MarshalAs(UnmanagedType.I4)]int ModuleIxCh, [MarshalAs(UnmanagedType.I4)] ref int GlobalAxis);
			
				// �ش� ��� ������ ����� ����� ���� ä�� ��ȣ�� ���Ͽ� ���� ä�� ��ȣ�� ��ȯ.
				// [NodeID : ��� ��ȣ, ModuleIdx : �ش� ����� ��� ��ȣ, ModuleInCh : ��� �� ������ ����� ä�� ��ȣ, GlobalDIO : ���� ������ ����� ä�� ��ȣ ��ȯ]
				//EXTERN LONG (__stdcall *ceGnGlobalDIO_Get)	(__in LONG NodeID, __in  LONG ModuleIdx, __in  LONG ModuleInCh, __out PLONG GlobalDIO);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceGnGlobalDIO_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceGnGlobalDIO_Get([MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)]int ModuleIdx, [MarshalAs(UnmanagedType.I4)]int ModuleInCh, [MarshalAs(UnmanagedType.I4)] ref int GlobalDIO);
			
				// �ش� ��� �Ƴ��α� �Է� ����� ���� ä�� ��ȣ�� ���Ͽ� ���� ä�� ��ȣ�� ��ȯ.
				// [NodeID : ��� ��ȣ, ModuleIdx : �ش� ����� ��� ��ȣ, ModuleInCh : ��� �� �Ƴ��α� �Է� ä�� ��ȣ, GlobalAI : ���� �Ƴ��α� �Է� ä�� ��ȣ ��ȯ]
				//EXTERN LONG (__stdcall *ceGnGlobalAI_Get)	(__in LONG NodeID, __in  LONG ModuleIdx, __in  LONG ModuleInCh, __out PLONG GlobalAI);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceGnGlobalAI_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceGnGlobalAI_Get([MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)]int ModuleIdx, [MarshalAs(UnmanagedType.I4)]int ModuleInCh, [MarshalAs(UnmanagedType.I4)] ref int GlobalAI);
			
				// �ش� ��� �Ƴ��α� ��� ����� ���� ä�� ��ȣ�� ���Ͽ� ���� ä�� ��ȣ�� ��ȯ.
				// [NodeID : ��� ��ȣ, ModuleIdx : �ش� ����� ��� ��ȣ, ModuleInCh : ��� �� �Ƴ��α� ��� ä�� ��ȣ, GlobalAO : ���� �Ƴ��α� ��� ä�� ��ȣ ��ȯ]
				//EXTERN LONG (__stdcall *ceGnGlobalAO_Get)	(__in LONG NodeID, __in  LONG ModuleIdx, __in  LONG ModuleInCh, __out PLONG GlobalAO);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceGnGlobalAO_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceGnGlobalAO_Get([MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)]int ModuleIdx, [MarshalAs(UnmanagedType.I4)]int ModuleInCh, [MarshalAs(UnmanagedType.I4)] ref int GlobalAO);
			
				// �ش� ��� ��� ������ �� ��� ����� ���� ä�� ��ȣ�� ���Ͽ� ���� ä�� ��ȣ�� ��ȯ.
				// [NodeID : ��� ��ȣ, ModuleIdx : �ش� ����� ��� ��ȣ, ModuleInCh : ��� �� ��� ������ �� ��� ä�� ��ȣ, GlobalAO : ���� ��� ������ �� ��� ä�� ��ȣ ��ȯ]
				//EXTERN LONG (__stdcall *ceGnGlobalMDIO_Get)	(__in LONG NodeID, __in  LONG ModuleIdx, __in  LONG ModuleInCh, __out PLONG GlobalMDIO);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceGnGlobalMDIO_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceGnGlobalMDIO_Get([MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)]int ModuleIdx, [MarshalAs(UnmanagedType.I4)]int ModuleInCh, [MarshalAs(UnmanagedType.I4)] ref int GlobalMDIO);
			
				// �ش� ��� ī���� ����� ���� ä�� ��ȣ�� ���Ͽ� ���� ä�� ��ȣ�� ��ȯ.
				// [NodeID : ��� ��ȣ, ModuleIdx : �ش� ����� ��� ��ȣ, ModuleInCh : ��� �� ī���� ä�� ��ȣ, GlobalAO : ���� ī���� ä�� ��ȣ ��ȯ]
				//EXTERN LONG (__stdcall *ceGnGlobalCNT_Get)	(__in LONG NodeID, __in  LONG ModuleIdx, __in  LONG ModuleInCh, __out PLONG GlobalCNT);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceGnGlobalCNT_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceGnGlobalCNT_Get([MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)]int ModuleIdx, [MarshalAs(UnmanagedType.I4)]int ModuleInCh, [MarshalAs(UnmanagedType.I4)] ref int GlobalCNT);
			
				//****************************************************************************
				//*************** START OF GENERAL MOTION FUNCTION DECLARATIONS **************
				//****************************************************************************
			
				// SERVO-ON ��ȣ ����� �ΰ� Ȥ�� ����.
				// [Channel : ���� �� ��ȣ, Enable : SERVO-ON ��ȣ ��� ON/OFF]
				//EXTERN LONG (__stdcall *cemGnServoOn_Set)	(__in LONG Channel, __in LONG Enable);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemGnServoOn_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemGnServoOn_Set([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)]int Enable);
			
				// SERVO-ON ��ȣ�� ��� ���¸� ��ȯ.
				// [Channel: ���� �� ��ȣ, Enable: SERVO-ON ��ȣ�� ��� ���¸� ��ȯ]
				//EXTERN LONG (__stdcall *cemGnServoOn_Get)	(__in LONG Channel, __in PLONG Enable);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemGnServoOn_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemGnServoOn_Get([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)] ref int Enable);
			
				// �ش� ��� ���� �˶� ���� ��ȣ ��� ����.
				// [Axis : ���� ��� ���� �� ��ȣ, IsReset: �˶� ���� ��ȣ ��� ����]
				//EXTERN LONG (__stdcall *cemGnAlarmReset) (__in LONG Axis, __in LONG IsReset);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemGnAlarmReset", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemGnAlarmReset([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int IsReset);
			
				//****************************************************************************
				//*************** START OF CONFIGURATION FUNCTION DECLARATIONS ***************
				//****************************************************************************
			
				// ��� ����� ��ȣ�� ȯ�� ���� ����. PropId�� ceSDKDef.h ���Ͽ� ���ǵ� enum _TCmMioPropId �� ����Ʈ�� ����.
				// [Axis : ���� �� ��ȣ, PropId : ȯ�� ���� �Ű� ����, PropVal : PropId�� ������ ȯ�濡 ���� ������]
				//EXTERN LONG (__stdcall *cemCfgMioProperty_Set)	(__in LONG Axis, __in  LONG PropId, __in  LONG PropVal);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgMioProperty_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgMioProperty_Set([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int PropId, [MarshalAs(UnmanagedType.I4)]int Propval);
			
				// ��� ����� ��ȣ�� ȯ�� ���� ���� ��ȯ. PropId�� ceSDKDef.h ���Ͽ� ���ǵ� enum _TCmMioPropId �� ����Ʈ�� ����.
				// [Axis : ���� �� ��ȣ, PropId : ȯ�� ���� �Ű� ����, PropVal : PropId�� ������ ȯ�濡 ���� ��ȯ��]
				//EXTERN LONG (__stdcall *cemCfgMioProperty_Get)	(__in LONG Axis, __in  LONG PropId, __out PLONG PropVal);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgMioProperty_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgMioProperty_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int PropId, [MarshalAs(UnmanagedType.I4)] ref int PropVal);
			
				// ���� I/O(Input/Output) ��ȣ�� ���� ������ ���� ���� ��� ����.
				// [Axis : ���� �� ��ȣ, IsEnable : ���� ���� ���� ����]
				//EXTERN LONG (__stdcall *cemCfgFilter_Set)	(__in LONG Axis, __in LONG IsEnable);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgFilter_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgFilter_Set([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int IsEnable);
			
				// ���� I/O(Input/Output) ��ȣ�� ���� ������ ���� ���� ��� ���� ���¸� ��ȯ.
				// [Axis : ���� �� ��ȣ, IsEnabled : ���� ���� ���� ���� ��ȯ]
				//EXTERN LONG (__stdcall *cemCfgFilter_Get)	(__in LONG Axis, __out PLONG IsEnabled);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgFilter_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgFilter_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int IsEnabled);
			
				// EA/EB �Ǵ� PA/PB ��ȣ �Է� ȸ�ο� ������ ���͸� ������ ���� ����.
				// [Channel : ���� �� ��ȣ, Target : �Լ��� ���� ���(EA/EB or PA/PB), IsEnable : ���� ������ ���� ����]
				//EXTERN LONG (__stdcall *cemCfgFilterAB_Set)	(__in LONG Channel, __in LONG Target, __in LONG IsEnable);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgFilterAB_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgFilterAB_Set([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)]int Target, [MarshalAs(UnmanagedType.I4)]int IsEnable);
			
				// EA/EB �Ǵ� PA/PB ��ȣ �Է� ȸ�ο� ������ ���͸� ������ ���� ���� ���� ���¸� ��ȯ
				// [Channel : ���� �� ��ȣ, Target : �Լ��� ���� ���(EA/EB or PA/PB), IsEnabled : ���� ���� ���� ���� ��ȯ]
				//EXTERN LONG (__stdcall *cemCfgFilterAB_Get)	(__in LONG Channel, __in LONG Target, __out PLONG IsEnabled);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgFilterAB_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgFilterAB_Get([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)]int Target, [MarshalAs(UnmanagedType.I4)] ref int IsEnabled);
			
				// ���ڴ� �޽�(Feedback Pulse) ��ȣ�� �Է� ��� ����.
				// [Axis : ���� �� ��ȣ, InputMode : Feedback Pulse �Է� ���, IsReverse : Feedback Count ���� UP/DOWN ������ �ݴ�� �� ������ ����]
				//EXTERN LONG (__stdcall *cemCfgInMode_Set)	(__in LONG Axis, __in LONG InputMode, __in LONG IsReverse);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgInMode_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgInMode_Set([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int InputMode, [MarshalAs(UnmanagedType.I4)]int IsReverse);
			
				// ���ڴ� �޽�(Feedback Pulse) ��ȣ�� �Է� ��� ���� ���¸� ��ȯ.
				// [Axis : ���� �� ��ȣ, InputMode: Feedback Pulse �Է� ���, IsReverse : Feedback Count ���� UP/DOWN ������ �ݴ�� �� ������ ���� ��ȯ]
				//EXTERN LONG (__stdcall *cemCfgInMode_Get)	(__in LONG Axis, __out PLONG InputMode, __out PLONG IsReverse);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgInMode_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgInMode_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int InputMode, [MarshalAs(UnmanagedType.I4)] ref int IsReverse);
			
				// ���� �޽�(Command Pulse) ��ȣ ��� ��� ����.
				// [Axis : ���� �� ��ȣ, OutputMode : Command Pulse ��� ���]
				//EXTERN LONG (__stdcall *cemCfgOutMode_Set)	(__in LONG Axis, __in LONG OutputMode);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgOutMode_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgOutMode_Set([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int OutputMode);
			
				// ���� �޽�(Command Pulse) ��ȣ ��� ��� ���� ���¸� ��ȯ.
				// [Axis : ���� �� ��ȣ, OutputMode: Command �޽��� ��� ��� ��ȯ]
				//EXTERN LONG (__stdcall *cemCfgOutMode_Get)	(__in LONG Axis, __out PLONG OutputMode);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgOutMode_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgOutMode_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int OutputMode);
			
				// �̼� ��ǥ ��ǥ�� ������ Ŀ�ǵ� ��ġ�� ���� �ǵ�� ��ġ�� ������ ����.
				// [Axis : ���� �� ��ȣ, CtrlMode : ���� ���]
				//EXTERN LONG (__stdcall *cemCfgCtrlMode_Set)	(__in LONG Axis, __in LONG CtrlMode);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgCtrlMode_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgCtrlMode_Set([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int CtrlMode);
			
				// �̼� ��ǥ ��ǥ�� ������ Ŀ�ǵ� ��ġ�� ���� �ǵ�� ��ġ�� ������ ���� ���� ���¸� ��ȯ.
				// [Axis : ���� �� ��ȣ, CtrlMode : ���� ���]
				//EXTERN LONG (__stdcall *cemCfgCtrlMode_Get)	(__in LONG Axis, __out PLONG CtrlMode);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgCtrlMode_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgCtrlMode_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int CtrlMode);
			
				// �Է� �޽�(Feedback Pulse)�� ��� �޽�(Command Pulse)�� ���ش��� ����.
				// [Axis : ���� �� ��ȣ, Ratio : Feedback Pulse�� Command Pulse�� ���ش� ����]
				//EXTERN LONG (__stdcall *cemCfgInOutRatio_Set)	(__in LONG Axis, __in DOUBLE Ratio);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgInOutRatio_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgInOutRatio_Set([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.R8)]double Ratio );
			
				// �Է� �޽�(Feedback Pulse)�� ��� �޽�(Command Pulse)�� ���ش� ���� ���¸� ��ȯ.
				// [Axis : ���� �� ��ȣ, Ratio : Feedback Pulse�� Command Pulse�� ���ش� ������ ��ȯ]
				//EXTERN LONG (__stdcall *cemCfgInOutRatio_Get)	(__in LONG Axis, __out PDOUBLE Ratio);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgInOutRatio_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgInOutRatio_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int Ratio);
			
				// ���� �Ÿ� ������ ����.
				// [Axis : ���� �� ��ȣ, UnitDist : ���� �Ÿ� 1�� �̵��ϱ� ���� ��� �޽� ��]
				//EXTERN LONG (__stdcall *cemCfgUnitDist_Set)	(__in LONG Axis, __in DOUBLE UnitDist);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgUnitDist_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgUnitDist_Set([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.R8)]double UnitDist );
			
				// ���� �Ÿ� ������ ��ȯ.
				// [Axis : ���� �� ��ȣ, UnitDist : ���� �Ÿ� 1�� �̵��ϱ� ���� ��� �޽� ���� ��ȯ]
				//EXTERN LONG (__stdcall *cemCfgUnitDist_Get)	(__in LONG Axis, __out PDOUBLE UnitDist);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgUnitDist_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgUnitDist_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int UnitDist);
			
				// ���� �ӵ� ������ ����.
				// [Axis : ���� �� ��ȣ, UnitSpeed : ���� �ӵ��� ���� �޽� ��� �ӵ�(PPS)]
				//EXTERN LONG (__stdcall *cemCfgUnitSpeed_Set)	(__in LONG Axis, __in DOUBLE UnitSpeed);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgUnitSpeed_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgUnitSpeed_Set([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.R8)]double UnitSpeed );
			
				// ���� �ӵ� ������ ��ȯ.
				// [Axis : ���� �� ��ȣ, UnitSpeed : ���� �ӵ��� ���� �޽� ��� �ӵ�(PPS)�� ��ȯ]
				//EXTERN LONG (__stdcall *cemCfgUnitSpeed_Get)	(__in LONG Axis, __out PDOUBLE UnitSpeed);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgUnitSpeed_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgUnitSpeed_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int UnitSpeed);
			
				// ��� �ӵ� ���ѹ����� ����.
				// [Axis : ���� �� ��ȣ, MaxPPS : ��� �ְ� �ӵ�(PPS)]
				//EXTERN LONG (__stdcall *cemCfgSpeedRange_Set)	(__in LONG Axis, __in  DOUBLE MaxPPS);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgSpeedRange_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgSpeedRange_Set([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.R8)]double MaxPPS );
			
				// ��� �ӵ� ���ѹ��� ���� ���¸� ��ȯ.
				// [Axis : ���� �� ��ȣ, MinPPS : ��� ���� �ӵ�(PPS) ��ȯ, MaxPPS : ��� �ְ� �ӵ�(PPS) ��ȯ]
				//EXTERN LONG (__stdcall *cemCfgSpeedRange_Get)	(__in LONG Axis, __out PDOUBLE MinPPS, __out PDOUBLE MaxPPS);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgSpeedRange_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgSpeedRange_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int MinPPS, [MarshalAs(UnmanagedType.I4)] ref int MaxPPS);
			
				// ��� �̼� ���� �ӵ� ����.
				// [Axis : ���� �� ��ȣ, SpeedMode : �ӵ� ���, WorkSpeed : �۾� �ӵ�, Accel : ���ӵ�, Decel : ���ӵ�]
				//EXTERN LONG (__stdcall *cemCfgSpeedPattern_Set)	(__in LONG Axis, __in  LONG SpeedMode, __in  DOUBLE  WorkSpeed, __in  DOUBLE Accel,  __in  DOUBLE Decel);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgSpeedPattern_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgSpeedPattern_Set([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int SpeedMode, [MarshalAs(UnmanagedType.R8)]double WorkSpeed, 
					[MarshalAs(UnmanagedType.R8)]double Acc, [MarshalAs(UnmanagedType.R8)]double Dec );
			
				// ��� �̼� ���� �ӵ� ���� ���¸� ��ȯ.
				// [Axis : ���� �� ��ȣ, SpeedMode : �ӵ� ��� ��ȯ, WorkSpeed : �۾� �ӵ� ��ȯ, Accel : ���ӵ� ��ȯ, Decel : ���ӵ� ��ȯ]
				//EXTERN LONG (__stdcall *cemCfgSpeedPattern_Get)	(__in LONG Axis, __out PLONG SpeedMode,__out PDOUBLE WorkSpeed, __out PDOUBLE Accel, __out PDOUBLE Decel);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgSpeedPattern_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgSpeedPattern_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int SpeedMode, [MarshalAs(UnmanagedType.I4)] ref int WorkSpeed, 
					[MarshalAs(UnmanagedType.I4)] ref int Accel, [MarshalAs(UnmanagedType.I4)] ref int Decel);
			
				// �Է� �޽�(Feedback Pulse)�� ���� ����� �����ӵ� ���� ����.
				// [Axis : ���� �� ��ȣ, IsEnable : Feedback �ӵ� Ȯ�� ��� Ȱ�� ����, Interval : Feedback �޽� �� Ȯ�� �ֱ�(ms)]
				//EXTERN LONG (__stdcall *cemCfgActSpdCheck_Set)	(__in LONG Axis, __in LONG IsEnable, __in LONG Interval);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgActSpdCheck_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgActSpdCheck_Set([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int IsEnable, [MarshalAs(UnmanagedType.I4)]int Interval);
			
				// �Է� �޽�(Feedback Pulse)�� ���� ��� �����ӵ� ���� ���� ���¸� ��ȯ.
				// [Axis : ���� �� ��ȣ, IsEnable : Feedback �ӵ� Ȯ�� ��� Ȱ�����θ� ��ȯ, Interval : Feedback �޽� �� Ȯ�� �ֱ�(ms) ��ȯ]
				//EXTERN LONG (__stdcall *cemCfgActSpdCheck_Get)	(__in LONG Axis, __out PLONG IsEnable, __out PLONG Interval);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgActSpdCheck_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgActSpdCheck_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int IsEnable, [MarshalAs(UnmanagedType.I4)] ref int Interval);
			
				// ����� �̼� ������ ����Ʈ�������� �̼����ѹ����� �����Ͽ� ����.
				// [Axis : ���� �� ��ȣ, IsEnable : ����Ʈ���� ���� ��� Ȱ�� ����, LimitN : (-)���� Limit��, LimitP : (+)���� Limit��]
				//EXTERN LONG (__stdcall *cemCfgSoftLimit_Set)	(__in LONG Axis, __in LONG IsEnable, __in DOUBLE LimitN, __in DOUBLE LimitP);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgSoftLimit_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgSoftLimit_Set([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int IsEnable, [MarshalAs(UnmanagedType.R8)]double LimitN, [MarshalAs(UnmanagedType.R8)]double LimitP );
			
				// ����� ����Ʈ�������� �̼����ѹ����� ���� ������ ��ȯ.
				// [Axis : ���� �� ��ȣ, IsEnable : ����Ʈ���� ���� ��� Ȱ�� ���� ��ȯ, LimitN : (-)���� Limit�� ��ȯ, LimitP : (+)���� Limit�� ��ȯ]
				//EXTERN LONG (__stdcall *cemCfgSoftLimit_Get)	(__in LONG Axis, __out PLONG IsEnable, __out PDOUBLE LimitN, __out PDOUBLE LimitP);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgSoftLimit_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgSoftLimit_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int IsEnable, [MarshalAs(UnmanagedType.I4)] ref int LimitN,
					[MarshalAs(UnmanagedType.I4)] ref int LimitP);
			
				// ��ī����(Ring-Counter) ��� ����.
				// [Channel : ���� �� ��ȣ, TargCntr : �Լ��� ���� ���(Command or Feedback Counter), IsEnable : Ring-Counter ��� Ȱ�� ����, CntMax: ī���� ����]
				//EXTERN LONG (__stdcall *cemCfgRingCntr_Set)	(__in LONG Channel, __in LONG TargCntr, __in LONG IsEnable, __in DOUBLE CntMax);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgRingCntr_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgRingCntr_Set([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)]int TargCntr, [MarshalAs(UnmanagedType.I4)]int IsEnable, [MarshalAs(UnmanagedType.I4)]int CntMax);
			
				// ��ī����(Ring-Counter) ��� ���� ���¸� ��ȯ.
				// [Channel: ���� �� ��ȣ, TargCntr: �Լ��� ���� ���(Command or Feedback Counter) ����, IsEnable: Ring-Counter ��� Ȱ�� ���� ��ȯ, CntMax: ī���� ���� ��ȯ]
				//EXTERN LONG (__stdcall *cemCfgRingCntr_Get)	(__in LONG Channel, __in LONG TargCntr, __out PLONG IsEnable, __out PDOUBLE CntMax);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgRingCntr_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgRingCntr_Get([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)]int TargCntr, [MarshalAs(UnmanagedType.I4)] ref int IsEnable, [MarshalAs(UnmanagedType.I4)] ref int CntMax);
			
				// �۾��ӵ� ���� �� ������ �۾��ӵ��� �����ϴ� ��� �� ����.
				// [Axis: ���� �� ��ȣ, fCorrRatio: �ӵ� ���� ������(%)]
				//EXTERN LONG (__stdcall *cemCfgVelCorrRatio_Set)	(__in LONG Axis, __in DOUBLE fCorrRatio);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgVelCorrRatio_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgVelCorrRatio_Set([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.R8)]double fCorrRatio );
			
				// �۾��ӵ� ���� �� ������ �۾��ӵ��� �����ϴ� ��� ���� ��ȯ.
				// [Axis: ���� �� ��ȣ, fCorrRatio: �ӵ� ���� ������(%) ��ȯ]
				//EXTERN LONG (__stdcall *cemCfgVelCorrRatio_Get)	(__in LONG Axis, __out PDOUBLE fCorrRatio);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgVelCorrRatio_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgVelCorrRatio_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int fCorrRatio);
			
				// ������ �̼� �۾��� �Ϸ���� ���� �࿡ ���ο� �̼� ����� �ϴ޵Ǿ��� ���� ó�� ��å�� ����.
				// [seqMode: ������(Sequence) ���]
				//EXTERN LONG (__stdcall *cemCfgSeqMode_Set)	(__in LONG SeqMode);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgSeqMode_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgSeqMode_Set([MarshalAs(UnmanagedType.I4)]int SeqMode);
			
				// ������ �̼� �۾��� �Ϸ���� ���� �࿡ ���ο� �̼� ����� �ϴ޵Ǿ��� ���� ó�� ��å�� ���� ������ ��ȯ.
				// [seqMode: ������(Sequence) ��� ��ȯ]
				//EXTERN LONG (__stdcall *cemCfgSeqMode_Get)	(__out PLONG SeqMode);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgSeqMode_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgSeqMode_Get([MarshalAs(UnmanagedType.I4)] ref int SeqMode);
			
				//****************************************************************************
				//*************** START OF HOME RETURN FUNCTION DECLARATIONS *****************
				//****************************************************************************
			
				// �������� ȯ�� ����.
				// [Axis: ���� �� ��ȣ, HomeMode: �������� ���(0~12), Dir: �������� ��� ���� ���� EzCount: EzCount ��(0~15), EscDist: ����Ż�� �Ÿ�, Offset: �������� ���� ��ġ�� �������� �߰� ��� �̵���(�����ǥ �̼�)]
				//EXTERN LONG (__stdcall *cemHomeConfig_Set)	(__in LONG Axis, __in  LONG HomeMode,  __in  LONG Dir,  __in LONG EzCount,  __in  DOUBLE EscDist,  __in  DOUBLE Offset);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemHomeConfig_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemHomeConfig_Set([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int HomeMode, [MarshalAs(UnmanagedType.I4)]int Dir, 
					[MarshalAs(UnmanagedType.I4)]int EzCount, [MarshalAs(UnmanagedType.R8)]double EscDist, [MarshalAs(UnmanagedType.R8)]double Offset);
			
				// �������� ȯ�� ���� ���¸� ��ȯ.
				// [Axis: ���� �� ��ȣ, HomeMode: �������� ���(0~12) ��ȯ, Dir: �������� ��� ���� ���� ��ȯ, EzCount: EzCount ��(0~15) ��ȯ, EscDist: ����Ż�� �Ÿ� ��ȯ, Offset: �������� ���� ��ġ�� �������� �߰� ��� �̵��� ��ȯ]
				//EXTERN LONG (__stdcall *cemHomeConfig_Get)	(__in LONG Axis, __out PLONG HomeMode, __out PLONG Dir, __out PLONG EzCount, __out PDOUBLE EscDist, __out PDOUBLE Offset);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemHomeConfig_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemHomeConfig_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int HomeMode, [MarshalAs(UnmanagedType.I4)] ref int Dir, 
					[MarshalAs(UnmanagedType.I4)] ref int EzCount, [MarshalAs(UnmanagedType.I4)] ref int EscDist, 
					[MarshalAs(UnmanagedType.I4)] ref int Offset);
			
				// �������� �Ϸ� �� �߻��ϴ� ��� ��Ʈ�ѷ��� ���� ����̺갣�� ���� ������ ���� �߻��� �Է� �޽�(Feedback Pulse) ó���� ���� ����.
				// [Axis: ���� �� ��ȣ, PosClrMode: Command �� Feedback ��ġ�� Ŭ����Ǵ� ���]
				//EXTERN LONG (__stdcall *cemHomePosClrMode_Set)	(__in LONG Axis, __in LONG PosClrMode);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemHomePosClrMode_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemHomePosClrMode_Set([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int PosClrMode);
			
				// �������� �Ϸ� �� �߻��ϴ� ��� ��Ʈ�ѷ��� ���� ����̺갣�� ���� ������ ���� �߻��� �Է� �޽�(Feedback Pulse) ó���� ���� ���� ���¸� ��ȯ.
				//[Axis: ���� �� ��ȣ, PosClrMode: Command �� Feedback ��ġ�� Ŭ���� �Ǵ� ��� ��ȯ]
				//EXTERN LONG (__stdcall *cemHomePosClrMode_Get)	(__in LONG Axis, __out PLONG PosClrMode);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemHomePosClrMode_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemHomePosClrMode_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int PosClrMode);
			
				// ���� ���� �ӵ� ����.
				// [Axis: ���� �� ��ȣ, SpeedMode: �������� �ӵ� ���, Vel: �������� �۾� �ӵ�, Accel: �������� ���ӵ�, Decel: �������� ���ӵ�, RevVel: Revers Speed]
				//EXTERN LONG (__stdcall *cemHomeSpeedPattern_Set)	(__in LONG Axis, __in LONG SpeedMode, __in DOUBLE Vel, __in DOUBLE Accel, __in DOUBLE Decel, __in DOUBLE RevVel);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemHomeSpeedPattern_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemHomeSpeedPattern_Set([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int SpeedMode, [MarshalAs(UnmanagedType.R8)]double Vel, 
					[MarshalAs(UnmanagedType.R8)]double Acc, [MarshalAs(UnmanagedType.R8)]double Dec, [MarshalAs(UnmanagedType.R8)]double RevVel);
			
				// ���� ���� �ӵ� ���� ���¸� ��ȯ.
				// [Axis: ���� �� ��ȣ, SpeedMode: �������� �ӵ� ��� ��ȯ, Vel: �������� �۾� �ӵ� ��ȯ, Accel:�������� ���ӵ� ��ȯ, Decel:�������� ���ӵ� ��ȯ, RevVel: Revers Speed ��ȯ]
				//EXTERN LONG (__stdcall *cemHomeSpeedPattern_Get)	(__in LONG Axis, __out PLONG SpeedMode, __out PDOUBLE Vel, __out PDOUBLE Accel, __out PDOUBLE Decel, __out PDOUBLE RevVel);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemHomeSpeedPattern_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemHomeSpeedPattern_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int SpeedMode, [MarshalAs(UnmanagedType.I4)] ref int Vel,
					[MarshalAs(UnmanagedType.I4)] ref int Accel, [MarshalAs(UnmanagedType.I4)] ref int Decel,
					[MarshalAs(UnmanagedType.I4)] ref int RevVel);
			
				// ���� ���� ���� �̼�. �� �Լ��� ����� �Ϸ�Ǳ� ������ ��ȯ���� �ʽ��ϴ�.
				// [Axis: ���� �� ��ȣ, IsBlocking: �Ϸ�ɶ����� �޼��� ��� ����]
				//EXTERN LONG (__stdcall *cemHomeMove)	(__in LONG Axis, __in LONG IsBlocking);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemHomeMove", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemHomeMove([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int IsBlocking);
			
				// ���� ���� ���� �̼�. �� �Լ��� ����� ���۽�Ų �Ŀ� �ٷ� ��ȯ�˴ϴ�.
				// [Axis: ���� �� ��ȣ]
				//EXTERN LONG (__stdcall *cemHomeMoveStart)	(__in LONG Axis);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemHomeMoveStart", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemHomeMoveStart([MarshalAs(UnmanagedType.I4)]int Axis);
			
				// ������ ����� �������� ���� �Ϸ� ���¸� Ȯ��.
				// [Axis: ���� �� ��ȣ, IsSuccess: ������ ����� ���� ���� ���� �Ϸ� ���� ��ȯ]
				//EXTERN LONG (__stdcall *cemHomeSuccess_Get)	(__in LONG Axis, __out PLONG IsSuccess);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemHomeSuccess_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemHomeSuccess_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int IsSuccess);
			
				// ������ ����� �������� ���� �Ϸ� ���¸� ������ ����.
				// [Axis: ���� �� ��ȣ, IsSuccess: ���� ������ ���� ���θ� ������ ����]
				//EXTERN LONG (__stdcall *cemHomeSuccess_Set)	(__in LONG Axis, __in LONG IsSuccess);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemHomeSuccess_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemHomeSuccess_Set([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int IsSuccess);
			
				// �������� ���� ���¸� ��ȯ.
				// [Axis: ���� �� ��ȣ, IsBusy: ���� �������� ���� ���� ��ȯ]
				//EXTERN LONG (__stdcall *cemHomeIsBusy)	(__in LONG Axis, __out PLONG IsBusy);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemHomeIsBusy", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemHomeIsBusy([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int IsBusy);
			
				// �������� �Ϸ� �ñ��� ���.
				// [Channel: ���� �� ��ȣ, IsBlocking: �Ϸ�ɶ����� �޼��� ��� ����]
				//EXTERN LONG (__stdcall *cemHomeWaitDone)	(__in LONG Axis, __in LONG IsBlocking);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemHomeWaitDone", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemHomeWaitDone([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int IsBlocking);
			
				//****************************************************************************
				//*************** START OF SINGLE AXIS CONTROL FUNCTION DECLARATIONS *********
				//****************************************************************************
			
				// ���� ���� �� �ش� �࿡ ���� �ӵ���� �� �ӵ����� ����.
				// [Axis: ���� �� ��ȣ, SpeedMode: �ӵ� ���, VelRatio: �۾��ӵ� ����(%), AccRatio: ���ӵ� ����(%), DecRatio: ���ӵ� ����(%)]
				//EXTERN LONG (__stdcall *cemSxSpeedRatio_Set)	(__in LONG Axis, __in LONG SpeedMode, __in DOUBLE VelRatio, __in DOUBLE AccRatio, __in DOUBLE DecRatio);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemSxSpeedRatio_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemSxSpeedRatio_Set([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int SpeedMode, [MarshalAs(UnmanagedType.R8)]double VelRatio,
					[MarshalAs(UnmanagedType.R8)]double AccRatio, [MarshalAs(UnmanagedType.R8)]double DecRatio);
			
				// ���� ���� �� �ش� �࿡ ���� �ӵ���� �� �ӵ����� ���� ���¸� ��ȯ.
				// [Axis: ���� �� ��ȣ, SpeedMode: �ӵ� ��� ��ȯ, VelRatio: �۾��ӵ� ����(%) ��ȯ, AccRatio: ���ӵ� ����(%) ��ȯ, DecRatio: ���ӵ� ����(%) ��ȯ]
				//EXTERN LONG (__stdcall *cemSxSpeedRatio_Get)	(__in LONG Axis, __out PLONG SpeedMode, __out PDOUBLE VelRatio, __out PDOUBLE AccRatio, __out PDOUBLE DecRatio);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemSxSpeedRatio_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemSxSpeedRatio_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int SpeedMode, [MarshalAs(UnmanagedType.I4)] ref int VelRatio, 
					[MarshalAs(UnmanagedType.I4)] ref int AccRatio, [MarshalAs(UnmanagedType.I4)] ref int DecRatio);
			
				// ���� ��� ��ǥ �̼�. �� �Լ��� ����� �Ϸ�Ǳ� ������ ��ȯ���� �ʽ��ϴ�.
				// [Axis: ���� �� ��ȣ, Distance: �̵��� �Ÿ�, IsBlocking: �Ϸ�ɶ����� �޼��� ��� ����]
				//EXTERN LONG (__stdcall *cemSxMove)	(__in LONG Axis, __in DOUBLE Distance, __in LONG IsBlocking);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemSxMove", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemSxMove([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.R8)]double Distance, [MarshalAs(UnmanagedType.I4)]int IsBlocking );
			
				// ���� ��� ��ǥ �̼�. �� �Լ��� ����� ���۽�Ų �Ŀ� �ٷ� ��ȯ�˴ϴ�.
				// [Axis: ���� �� ��ȣ, Distance: �̵��� �Ÿ�]
				//EXTERN LONG (__stdcall *cemSxMoveStart)	(__in LONG Axis, __in DOUBLE Distance);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemSxMoveStart", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemSxMoveStart([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.R8)]double Distance );
			
				// ���� ���� ��ǥ �̼�. �� �Լ��� ����� �Ϸ�Ǳ� ������ ��ȯ���� �ʽ��ϴ�.
				// [Axis: ���� �� ��ȣ, Distance: �̵��� ���� ��ǥ��, IsBlocking: �Ϸ�ɶ����� �޼��� ��� ����]
				//EXTERN LONG (__stdcall *cemSxMoveTo)	(__in LONG Axis, __in DOUBLE Position, __in LONG IsBlocking);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemSxMoveTo", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemSxMoveTo([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.R8)]double Position, [MarshalAs(UnmanagedType.I4)]int IsBlocking );
			
				// ���� ���� ��ǥ �̼�. �� �Լ��� ����� ���۽�Ų �Ŀ� �ٷ� ��ȯ�˴ϴ�.
				// [Axis: ���� �� ��ȣ, Distance: �̵��� ���� ��ǥ ��]
				//EXTERN LONG (__stdcall *cemSxMoveToStart)	(__in LONG Axis, __in DOUBLE Position);	
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemSxMoveToStart", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemSxMoveToStart([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.R8)]double Position );
			
				// ���� ���� �ӵ� �̼�. �� �Լ��� ����� ���۽�Ų �Ŀ� �ٷ� ��ȯ�˴ϴ�.
				// [Axis: ���� �� ��ȣ, Direction: ��� �̼� ����]
				//EXTERN LONG (__stdcall *cemSxVMoveStart)	(__in LONG Axis, __in LONG Direction);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemSxVMoveStart", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemSxVMoveStart([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int Direction);
			
				// ���� �̼� ���� �� ����.
				// [Axis: ���� �� ��ȣ, IsWaitComplete: ���� �Ϸ�ɶ����� �Լ� ��ȯ ����, IsBlocking: �Ϸ�ɶ����� �޼��� ��� ����]
				//EXTERN LONG (__stdcall *cemSxStop)	(__in LONG Axis, __in LONG IsWaitComplete, __in LONG IsBlocking);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemSxStop", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemSxStop([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int IsWaitComplete, [MarshalAs(UnmanagedType.I4)]int IsBlocking);
			
				// ���� �̼� ��� ����.
				// [Axis: ���� �� ��ȣ]
				//EXTERN LONG (__stdcall *cemSxStopEmg)	(__in LONG Axis);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemSxStopEmg", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemSxStopEmg([MarshalAs(UnmanagedType.I4)]int Axis);
			
				// ���� �̼� �Ϸ� Ȯ��.
				// [Axis: ���� �� ��ȣ, IsDone: ��� �۾� �Ϸ� ���� ��ȯ]
				//EXTERN LONG (__stdcall *cemSxIsDone)	(__in LONG Axis, __out PLONG IsDone);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemSxIsDone", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemSxIsDone([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int IsDone);
			
				// ���� �̼� �Ϸ� �������� ���.
				// [Axis: ���� �� ��ȣ, IsBlocking: �Ϸ�ɶ����� �޼��� ��� ����]
				//EXTERN LONG (__stdcall *cemSxWaitDone)	(__in LONG Axis, __in LONG IsBlocking);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemSxWaitDone", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemSxWaitDone([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int IsBlocking);
			
				// �ش� ���� �࿡ ���� ���������� �̼���(��� or ���� ��ǥ) ��ġ�� ��ȯ.
				// [Channel: ���� �� ��ȣ, Position: ���������� �̼��� ��ġ �� ��ȯ]
				//EXTERN LONG (__stdcall *cemSxTargetPos_Get)	(__in LONG Channel, __out PDOUBLE Position);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemSxTargetPos_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemSxTargetPos_Get([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)] ref int Position);
			
				// ���� ����� �ʱ� �ӵ� ����.
				// [Axis: ���� �� ��ȣ, IniSpeed: �ʱ� �ӵ�]
				//EXTERN LONG (__stdcall *cemSxOptIniSpeed_Set)	(__in LONG Axis, __in DOUBLE IniSpeed);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemSxOptIniSpeed_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemSxOptIniSpeed_Set([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.R8)]double IniSpeed );
			
				// ���� ����� �ʱ� �ӵ� ������ ��ȯ.
				// [Axis: ���� �� ��ȣ, IniSpeed: �ʱ� �ӵ� ��ȯ]
				//EXTERN LONG (__stdcall *cemSxOptIniSpeed_Get)	(__in LONG Axis, __out PDOUBLE IniSpeed);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemSxOptIniSpeed_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemSxOptIniSpeed_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int IniSpeed);
			
				// ���� ��� ��� ��ǥ 2�ܰ� �ӵ� �̼� ���. �� �Լ��� ����� ���۽�Ų �Ŀ� �ٷ� ��ȯ�˴ϴ�.
				// [Axis: ���� �� ��ȣ, Distance: �̵��� �Ÿ�(��� ��ǥ ��), Vel2: 2�ܰ� �۾� �ӵ�]
				//EXTERN LONG (__stdcall *cemSxMoveStart2V)	(__in LONG Axis, __in  DOUBLE Distance, __in DOUBLE Vel2);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemSxMoveStart2V", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemSxMoveStart2V([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.R8)]double Distance, [MarshalAs(UnmanagedType.R8)]double Vel2);
			
				// ���� ��� ���� ��ǥ 2�ܰ� �ӵ� �̼� ���. �� �Լ��� ����� ���۽�Ų �Ŀ� �ٷ� ��ȯ�˴ϴ�.
				// [Axis: ���� �³� ��ȣ, Position: �̵��� ��ġ (���� ��ǥ ��), Vel2: 2�ܰ� �۾� �ӵ�]
				//EXTERN LONG (__stdcall *cemSxMoveToStart2V)	(__in LONG Axis, __in  DOUBLE Position, __in DOUBLE Vel2);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemSxMoveToStart2V", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemSxMoveToStart2V([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.R8)]double Position, [MarshalAs(UnmanagedType.R8)]double Vel2);
			
				// ���� ����� �鷡�� Ȥ�� ���� ������ ���� ����.
				// [Axis: ���� �� ��ȣ, CorrMode: ���� ���, CorrAmount: ���� �޽� ��, CorrVel: ���� �޽��� ��� ���ļ�, CntrMask: ���� �޽� ��½� ī������ ���� ����]
				//EXTERN LONG (__stdcall *cemSxCorrection_Set)	(__in LONG Axis, __in LONG CorrMode, __in DOUBLE CorrAmount, __in DOUBLE CorrVel, __in LONG CntrMask);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemSxCorrection_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemSxCorrection_Set([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int CorrMode, [MarshalAs(UnmanagedType.R8)]double CorrAmount,
					[MarshalAs(UnmanagedType.R8)]double CorrVel, [MarshalAs(UnmanagedType.I4)]int CntrMask);
			
				// ���� ����� �鷡�� Ȥ�� ���� ������ ������ ��ȯ.
				// [Axis: ���� �� ��ȣ, CorrMode: ���� ��� ��ȯ, CorrAmount: ���� �޽� �� ��ȯ, CorrVel: ���� �޽��� ��� ���ļ� ��ȯ, CntrMask: ���� �޽� ��½� ī������ ���� ���� ��ȯ]
				//EXTERN LONG (__stdcall *cemSxCorrection_Get)	(__in LONG Axis, __out PLONG CorrMode, __out PDOUBLE CorrAmount, __out PDOUBLE CorrVel, __out PLONG CntrMask);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemSxCorrection_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemSxCorrection_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int CorrMode, [MarshalAs(UnmanagedType.I4)] ref int CorrAmount, 
					[MarshalAs(UnmanagedType.I4)] ref int CorrVel, [MarshalAs(UnmanagedType.I4)] ref int CntrMask);
			
				//****************************************************************************
				//*************** START OF INTERPOLATION CONTROL FUNCTION DECLARATIONS *******
				//****************************************************************************
			
				// ���� ��� �� �׷� ����.
				// [MapIndex: �� ��ȣ(0~7), NodeID: ��� ��ȣ, MapMask1: �� �ʿ� ������ ����� ������ ����ũ��(���� 32��Ʈ, BIT0~BIT31),
				// MapMask2: �� �ʿ� ������ ����� ������ ����ũ��(���� 32��Ʈ, BIT32~BIT63)]
				//EXTERN LONG (__stdcall *cemIxMapAxes)	(__in LONG MapIndex,__in LONG NodeID, __in LONG MapMask1, __in LONG MapMask2);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxMapAxes", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxMapAxes([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)]int MapMask1, [MarshalAs(UnmanagedType.I4)]int MapMask2);
			
				// ���� �ӵ� ������� ���� [MapIndex: �� ��ȣ, �ɼ�1: VelCorrOpt1, �ɼ�2: VelCorrOpt2] 
				//EXTERN LONG (__stdcall *cemIxVelCorrMode_Set)	(__in LONG MapIndex, __in LONG VelCorrOpt1, __in LONG VelCorrOpt2);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxVelCorrMode_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxVelCorrMode_Set([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.I4)]int VelCorrOpt1, [MarshalAs(UnmanagedType.I4)]int VelCorrOpt2);
			
				// ���� �ӵ� ������� ��ȯ [MapIndex: �� ��ȣ, �ɼ�1: VelCorrOpt1, �ɼ�2: VelCorrOpt2]
				//EXTERN LONG (__stdcall *cemIxVelCorrMode_Get)	(__in LONG MapIndex, __out PLONG VelCorrOpt1, __out PLONG VelCorrOpt2);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxVelCorrMode_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxVelCorrMode_Get([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.I4)] ref int VelCorrOpt1, [MarshalAs(UnmanagedType.I4)] ref int VelCorrOpt2);
			
				// ���� ��� �� �׷� ���� ����.
				// [MapIndex: �� ��ȣ]
				//EXTERN LONG (__stdcall *cemIxUnMap)	(__in LONG MapIndex);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxUnMap", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxUnMap([MarshalAs(UnmanagedType.I4)]int MapIndex);
			
				// ���� �̼� �ӵ� ����.
				// [MapIndex: �� ��ȣ, IsVectorSpeed: ���� Ȥ�� ������ ���ǵ� ��� ����, SpeedMode: �ӵ� ���, Vel: �۾� �ӵ�, Acc: ���ӵ�, Dec: ���ӵ�]
				//EXTERN LONG (__stdcall *cemIxSpeedPattern_Set)	(__in LONG MapIndex, __in LONG IsVectorSpeed, __in LONG SpeedMode, __in DOUBLE Vel, __in DOUBLE Acc, __in DOUBLE Dec);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxSpeedPattern_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxSpeedPattern_Set([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.I4)]int IsVectorSpeed, [MarshalAs(UnmanagedType.I4)]int SpeedMode,
					[MarshalAs(UnmanagedType.R8)] double Vel, [MarshalAs(UnmanagedType.R8)]double Acc, [MarshalAs(UnmanagedType.R8)]double Dec);
			
				// ���� �̼� �ӵ� ������ ��ȯ.
				// [MapIndex: �� ��ȣ, IsVectorSpeed: ���� Ȥ�� ������ ���ǵ� ��� ���� ��ȯ, SpeedMode: �ӵ� ��� ��ȯ, Vel: �۾� �ӵ� ��ȯ, Acc: ���ӵ� ��ȯ, Dec: ���ӵ� ��ȯ]
				//EXTERN LONG (__stdcall *cemIxSpeedPattern_Get)	(__in LONG MapIndex, __out PLONG IsVectorSpeed, __out PLONG SpeedMode, __out PDOUBLE Vel, __out PDOUBLE Acc, __out PDOUBLE Dec);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxSpeedPattern_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxSpeedPattern_Get([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.I4)] ref int IsVectorSpeed, [MarshalAs(UnmanagedType.I4)] ref int SpeedMode, 
					[MarshalAs(UnmanagedType.I4)] ref int Vel, [MarshalAs(UnmanagedType.I4)] ref int Acc, 
					[MarshalAs(UnmanagedType.I4)] ref int Dec);
			
				// ���� ���� �����ǥ �̼�. �� �Լ��� ����� �Ϸ�Ǳ� ������ ��ȯ���� �ʽ��ϴ�.
				// [MapIndex: �� ��ȣ, DistList: �̵��� �Ÿ���(�����ǥ) �迭 �ּ�, IsBlocking: �Ϸ�ɶ����� �޼��� ��� ����]
				//EXTERN LONG (__stdcall *cemIxLine)	(__in LONG MapIndex, __in PDOUBLE DistList, __in LONG IsBlocking);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxLine", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxLine([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.I4)] ref int DistList, [MarshalAs(UnmanagedType.I4)]int IsBlocking);
			
				// ���� ���� �����ǥ �̼�. �� �Լ��� ����� ���۽�Ų �Ŀ� �ٷ� ��ȯ�˴ϴ�.
				// [MapIndex: �� ��ȣ, DistList: �̵��� �Ÿ���(�����ǥ) �迭 �ּ�]
				//EXTERN LONG (__stdcall *cemIxLineStart)	(__in LONG MapIndex, __in PDOUBLE DistList);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxLineStart", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxLineStart([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.I4)] ref int DistList);
			
				// ���� ���� ������ǥ �̼�. �� �Լ��� ����� �Ϸ�Ǳ� ������ ��ȯ���� �ʽ��ϴ�.
				// [MapIndex: �� ��ȣ, PosList: �̵��� ��ġ��(������ǥ) �迭 �ּ�, IsBlocking: �Ϸ�ɶ����� �޼��� ��� ����]
				//EXTERN LONG (__stdcall *cemIxLineTo)	(__in LONG MapIndex, __in PDOUBLE PosList, __in LONG IsBlocking);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxLineTo", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxLineTo([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.I4)] ref int PosList, [MarshalAs(UnmanagedType.I4)]int IsBlocking);
			
				// ���� ���� ������ǥ �̼�. �� �Լ��� ����� ���۽�Ų �Ŀ� �ٷ� ��ȯ�˴ϴ�.
				// [MapIndex: �� ��ȣ, PosList: �̵��� ��ġ��(������ǥ) �迭 �ּ�]
				//EXTERN LONG (__stdcall *cemIxLineToStart)	(__in LONG MapIndex, __in PDOUBLE PosList);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxLineToStart", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxLineToStart([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.I4)] ref int PosList);
			
				// ��ȣ ���� �����ǥ(����� �߽���ǥ�� ����) �̼�. �� �Լ��� ����� �Ϸ�Ǳ� ������ ��ȯ���� �ʽ��ϴ�.
				// [MapIndex: �� ��ȣ, XCentOffset: ���� ��ġ�κ��� ���� �߽ɱ��� X�� �����ǥ, YCentOffset: ���� ��ġ���� ���� �߽ɱ��� Y�� �����ǥ,
				// EndAngle: ��ȣ ������ �Ϸ��� ��ǥ������ ���� ��ġ�� ���� ����(Degree), IsBlocking: �Ϸ�ɶ����� �޼��� ��� ����]
				//EXTERN LONG (__stdcall *cemIxArcA)	(__in LONG MapIndex, __in DOUBLE XCentOffset, __in DOUBLE YCentOffset, __in DOUBLE EndAngle, __in LONG IsBlocking);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxArcA", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxArcA([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.R8)]double XCentOffset, [MarshalAs(UnmanagedType.R8)]double YCentOffset, 
					[MarshalAs(UnmanagedType.R8)]double EndAngle, [MarshalAs(UnmanagedType.I4)]int IsBlocking);
			
				// ��ȣ ���� �����ǥ(����� �߽���ǥ�� ����) �̼�. �� �Լ��� ����� ���۽�Ų �Ŀ� �ٷ� ��ȯ�˴ϴ�.
				// [MapIndex: �� ��ȣ, XCentOffset: ���� ��ġ�κ��� ���� �߽ɱ��� X�� �����ǥ, YCentOffset: ���� ��ġ�κ��� ���� �߽ɱ��� Y�� �����ǥ,
				// EndAngle: ��ȣ ������ �Ϸ��� ��ǥ ������ ���� ��ġ�� ���� ����(Degree)]
				//EXTERN LONG (__stdcall *cemIxArcAStart)	(__in LONG MapIndex, __in DOUBLE XCentOffset, __in DOUBLE YCentOffset, __in DOUBLE EndAngle);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxArcAStart", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxArcAStart([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.R8)]double XCentOffset, [MarshalAs(UnmanagedType.R8)]double YCentOffset,
					[MarshalAs(UnmanagedType.R8)]double EndAngle);
			
				// ��ȣ ���� ������ǥ(������ �߽���ǥ�� ����) �̼�. �� �Լ��� ����� �Ϸ�Ǳ� ������ ��ȯ���� �ʽ��ϴ�.
				// [MapIndex: �� ��ȣ, XCent: �߽����� X�� ������ǥ, YCent: �߽����� Y�� ������ǥ,
				// EndAngle: ��ȣ ������ �Ϸ��� ��ǥ ������ ���� ��ġ�� ���� ����(Degree), IsBlocking: �Ϸ�ɶ����� �޼��� ��� ����]
				//EXTERN LONG (__stdcall *cemIxArcATo)	(__in LONG MapIndex, __in DOUBLE XCent, __in DOUBLE YCent, __in DOUBLE EndAngle, __in LONG IsBlocking);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxArcATo", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxArcATo([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.R8)]double Xcent, [MarshalAs(UnmanagedType.R8)]double YCent,
					[MarshalAs(UnmanagedType.R8)]double EndAngle, [MarshalAs(UnmanagedType.I4)]int IsBlocking);
			
				// ��ȣ ���� ������ǥ(������ �߽���ǥ�� ����) �̼�. �� �Լ��� ����� ���۽�Ų �Ŀ� �ٷ� ��ȯ�˴ϴ�.
				// [MapIndex: �� ��ȣ, XCent: �߽����� X�� ������ǥ, YCent: �߽����� Y�� ������ǥ, EndAngle: ��ȣ ������ �Ϸ��� ��ǥ ������ ���� ��ġ�� ���� ����(Degree)]
				//EXTERN LONG (__stdcall *cemIxArcAToStart)	(__in LONG MapIndex, __in DOUBLE XCent, __in DOUBLE YCent, __in DOUBLE EndAngle);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxArcAToStart", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxArcAToStart([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.R8)]double XCent, [MarshalAs(UnmanagedType.R8)]double YCent,
					[MarshalAs(UnmanagedType.R8)]double EndAngle);
			
				// ��ȣ���� �����ǥ(����� �߽���ǥ�� ������ǥ) �̼�. �� �Լ��� ����� �Ϸ�Ǳ� ������ ��ȯ���� �ʽ��ϴ�.
				// [MapIndex: �� ��ȣ, XCentOffset: ���� ��ġ���� �� �߽ɱ��� X����� �Ÿ�, YCentOffset: ���� ��ġ���� �� �߽ɱ��� Y����� �Ÿ�,
				// XEndPointDist: ��ȣ���� �̵��� �Ϸ��� ��ǥ������ ���� ��ġ�κ��� X��� �Ÿ�, YEndPointDist: ��ȣ ���� �̵��� �Ϸ��� ��ǥ������ ������ġ�� ������ Y��� �Ÿ�, Direction: ȸ�� ����, IsBlocking: �Ϸ�ɶ����� �޼��� ��� ���� ]
				//EXTERN LONG (__stdcall *cemIxArcP)	(__in LONG MapIndex, __in DOUBLE XCentOffset, __in DOUBLE YCentOffset, __in DOUBLE XEndPointDist, __in DOUBLE YEndPointDist, __in LONG Direction, __in LONG IsBlocking);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxArcP", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxArcP([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.R8)]double XCentOffset, [MarshalAs(UnmanagedType.R8)]double YCentOffset,
					[MarshalAs(UnmanagedType.R8)]double XEndPointDist, [MarshalAs(UnmanagedType.R8)]double YEndPointDist, [MarshalAs(UnmanagedType.I4)]int Direction, [MarshalAs(UnmanagedType.I4)]int IsBlocking);
			
				// ��ȣ���� �����ǥ(����� �߽���ǥ�� ������ǥ) �̼�. �� �Լ��� ����� ���۽�Ų �Ŀ� �ٷ� ��ȯ�˴ϴ�.
				// [MapIndex: �� ��ȣ, XCentOffset: �� ��ġ���� �� �߽ɱ��� X����� �Ÿ�, YCentOffset: �� ��ġ���� �� �߽ɱ��� Y����� �Ÿ�,
				// XEndPointDist: ��ȣ ���� �̵��� �Ϸ��� ��ǥ������ ������ġ�� ������ X��� �Ÿ�, YEndPointDist: ��ȣ ���� �̵��� �Ϸ��� ��ǥ������ ������ġ�� ������ Y��� �Ÿ�, Direction: ȸ�� ����]
				//EXTERN LONG (__stdcall *cemIxArcPStart)	(__in LONG MapIndex, __in DOUBLE XCentOffset, __in DOUBLE YCentOffset, __in DOUBLE XEndPointDist, __in DOUBLE YEndPointDist, __in LONG Direction);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxArcPStart", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxArcPStart([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.R8)]double XCentOffset, [MarshalAs(UnmanagedType.R8)]double YCentOffset,
					[MarshalAs(UnmanagedType.R8)]double XEndPointDist, [MarshalAs(UnmanagedType.R8)]double YEndPointDist, [MarshalAs(UnmanagedType.I4)]int Direction);
			
				// ��ȣ���� ������ǥ(������ �߽���ǥ�� ������ǥ) �̼�. �� �Լ��� ����� �Ϸ�Ǳ� ������ ��ȯ���� �ʽ��ϴ�.
				// [MapIndex: �� ��ȣ, XCent: �߽����� X�� ������ǥ, YCent: �߽����� Y�� ������ǥ, XEndPointDist: ��ȣ���� �̵��� �Ϸ��� ��ǥ������ X�� ������ǥ,
				// YEndPointDist: ��ȣ���� �̵��� �Ϸ��� ��ǥ������ Y�� ������ǥ, Direction: ȸ�� ����, IsBlocking: �Ϸ�ɶ����� �޼��� ��� ����]
				//EXTERN LONG (__stdcall *cemIxArcPTo)	(__in LONG MapIndex, __in DOUBLE XCent, __in DOUBLE YCent, __in DOUBLE XEndPointDist, __in DOUBLE YEndPointDist, __in LONG Direction, __in LONG IsBlocking);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxArcPTo", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxArcPTo([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.R8)]double XCent, [MarshalAs(UnmanagedType.R8)]double YCent,
					[MarshalAs(UnmanagedType.R8)]double XEndPointDist, [MarshalAs(UnmanagedType.R8)]double YEndPointDist, [MarshalAs(UnmanagedType.I4)]int Direction, [MarshalAs(UnmanagedType.I4)]int IsBlocking);
			
				// ��ȣ���� ������ǥ(������ �߽���ǥ�� ������ǥ) �̼�. �� �Լ��� ����� ���۽�Ų �Ŀ� �ٷ� ��ȯ�˴ϴ�.
				// [MapIndex: �� ��ȣ, XCent: �߽����� X�� ������ǥ, YCent: �߽����� Y�� ������ǥ, XEndPointDist: ��ȣ���� �̵��� �Ϸ��� ��ǥ������ X�� ������ǥ,
				// YEndPointDist: ��ȣ���� �̵��� �Ϸ��� ��ǥ������ Y�� ������ǥ, Direction: ȸ�� ����]
				//EXTERN LONG (__stdcall *cemIxArcPToStart)	(__in LONG MapIndex, __in DOUBLE XCent, __in DOUBLE YCent, __in DOUBLE XEndPointDist, __in DOUBLE YEndPointDist, __in LONG Direction);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxArcPToStart", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxArcPToStart([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.R8)]double XCent, [MarshalAs(UnmanagedType.R8)]double YCent,
					[MarshalAs(UnmanagedType.R8)]double XEndPointDist, [MarshalAs(UnmanagedType.R8)]double YEndPointDist, [MarshalAs(UnmanagedType.I4)]int Direction);
			
				// 3��(Point)�� ���� ��ȣ���� �����ǥ �̼�. �� �Լ��� ����� �Ϸ�Ǳ� ������ ��ȯ���� �ʽ��ϴ�.
				// [MapIndex: �� ��ȣ, P2X: ���� ��ġ���� 2��° ������ X�� �����ǥ, P2Y: ���� ��ġ���� 2��° ������ Y�� �����ǥ,
				// P3X: 2��° ������ 3��° ������ X�� �����ǥ, P3Y: 2��° ������ 3��° ������ Y�� �����ǥ, EndAngle: ������, IsBlocking: �Ϸ�ɶ����� �޼��� ��� ����]
				//EXTERN LONG (__stdcall *cemIxArc3P)	(__in LONG MapIndex, __in DOUBLE P2X,__in  DOUBLE P2Y,__in  DOUBLE P3X, __in DOUBLE P3Y, __in DOUBLE EndAngle, __in LONG IsBlocking);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxArc3P", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxArc3P([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.R8)]double P2X, [MarshalAs(UnmanagedType.R8)]double P2Y,
					[MarshalAs(UnmanagedType.R8)]double P3X, [MarshalAs(UnmanagedType.R8)]double P3Y, [MarshalAs(UnmanagedType.R8)]double EndAngle, [MarshalAs(UnmanagedType.I4)]int IsBlocking);
			
				// 3��(Point)�� ���� ��ȣ���� �����ǥ �̼�. �� �Լ��� ����� ���۽�Ų �Ŀ� �ٷ� ��ȯ�˴ϴ�.
				// [MapIndex: �� ��ȣ, P2X: ���� ��ġ���� 2��° ������ X�� �����ǥ, P2Y: ���� ��ġ���� 2��° ������ Y�� �����ǥ,
				// P3X: 2��° ������ 3��° ������ X�� �����ǥ, P3Y: 2��° ������ 3��° ������ Y�� �����ǥ, EndAngle: ������]
				//EXTERN LONG (__stdcall *cemIxArc3PStart)	(__in LONG MapIndex, __in DOUBLE P2X, __in DOUBLE P2Y, __in DOUBLE P3X,__in  DOUBLE P3Y,__in  DOUBLE EndAngle);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxArc3PStart", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxArc3PStart([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.R8)]double P2X, [MarshalAs(UnmanagedType.R8)]double P2Y,
					[MarshalAs(UnmanagedType.R8)]double P3X, [MarshalAs(UnmanagedType.R8)]double P3Y, [MarshalAs(UnmanagedType.R8)]double EndAngle);
			
				// 3��(Point)�� ���� ��ȣ���� ������ǥ �̼�. �� �Լ��� ����� �Ϸ�Ǳ� ������ ��ȯ���� �ʽ��ϴ�.
				// [MapIndex: �� ��ȣ, P2X: 2��° ���� X�� ������ǥ, P2Y: 2��° ���� Y�� ������ǥ,
				// P3X: 3��° ���� X�� ������ǥ, P3Y: 3��° ���� Y�� ������ǥ, EndAngle: ������, IsBlocking: �Ϸ�ɶ����� �޼��� ��� ����]
				//EXTERN LONG (__stdcall *cemIxArc3PTo)	(__in LONG MapIndex, __in DOUBLE P2X,__in  DOUBLE P2Y,__in  DOUBLE P3X, __in DOUBLE P3Y, __in DOUBLE EndAngle, __in LONG IsBlocking);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxArc3PTo", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxArc3PTo([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.R8)]double P2X, [MarshalAs(UnmanagedType.R8)]double P2Y,
					[MarshalAs(UnmanagedType.R8)]double P3X, [MarshalAs(UnmanagedType.R8)]double P3Y, [MarshalAs(UnmanagedType.R8)]double EndAngle, [MarshalAs(UnmanagedType.I4)]int IsBlocking);
			
				// 3��(Point)�� ���� ��ȣ���� ������ǥ �̼�. �� �Լ��� ����� ���۽�Ų �Ŀ� �ٷ� ��ȯ�˴ϴ�.
				// [MapIndex: �� ��ȣ, P2X: 2��° ���� X�� ������ǥ, P2Y: 2��° ���� Y�� ������ǥ, P3X: 3��° ���� X�� ������ǥ, P3Y: 3��° ���� Y�� ������ǥ, EndAngle: ������]
				//EXTERN LONG (__stdcall *cemIxArc3PToStart)	(__in LONG MapIndex, __in DOUBLE P2X, __in DOUBLE P2Y, __in DOUBLE P3X, __in DOUBLE P3Y, __in DOUBLE EndAngle);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxArc3PToStart", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxArc3PToStart([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.R8)]double P2X, [MarshalAs(UnmanagedType.R8)]double P2Y,
					[MarshalAs(UnmanagedType.R8)]double P3X, [MarshalAs(UnmanagedType.R8)]double P3Y, [MarshalAs(UnmanagedType.R8)]double EndAngle);
			
				// ���� ���� ���� �̼� ���� �� ����.
				// [MapIndex: �� ��ȣ]
				//EXTERN LONG (__stdcall *cemIxStop)	(__in LONG MapIndex);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxStop", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxStop([MarshalAs(UnmanagedType.I4)]int MapIndex);
			
				// ���� ���� ���� �̼� �������.
				// [MapIndex: �� ��ȣ]
				//EXTERN LONG (__stdcall *cemIxStopEmg)	(__in LONG MapIndex);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxStopEmg", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxStopEmg([MarshalAs(UnmanagedType.I4)]int MapIndex);
			
				// ���� ���� ���� �̼��� ���� �Ϸ� Ȯ��.
				// [MapIndex: �� ��ȣ, IsDone: ���� ���� ���� �̼��� ���� �Ϸ� ����]
				//EXTERN LONG (__stdcall *cemIxIsDone)	(__in LONG MapIndex, __out PLONG IsDone);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxIsDone", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxIsDone([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.I4)] ref int IsDone);
			
				// ���� ���� ���� �̼��� ���� �Ϸ� �������� ���.
				// [MapIndex: �� ��ȣ, IsBlocking: �Ϸ�ɶ����� �޼��� ��� ����]
				//EXTERN LONG (__stdcall *cemIxWaitDone)	(__in LONG MapIndex, __in LONG IsBlocking);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxWaitDone", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxWaitDone([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.I4)]int IsBlocking);
			
				//****************************************************************************
				//*************** START OF MASTER/SLAVE CONTROL FUNCTION DECLARATIONS ********
				//****************************************************************************
			
				// ��� ��� �࿡ ���ؼ�, Master/Slave ���� ������ Slave ������ ���.
				// [Axis: Slave ������ ������ ���� �� ��ȣ, MaxSpeed: Slave�� ���� �ִ� �ӵ�, IsInverse: Slave�� �� Master���� ���������� �ݴ�� �Ұ����� ����]
				//EXTERN LONG (__stdcall *cemMsRegisterSlave)	(__in LONG Axis, __in DOUBLE MaxSpeed, __in LONG IsInverse);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemMsRegisterSlave", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemMsRegisterSlave([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.R8)]double MaxSpeed, [MarshalAs(UnmanagedType.I4)]int IsInverse);
			
				// ��� ��� �࿡ ���ؼ�, Master/Slave ���� ������ Slave �� ����.
				// [Axis: Slave �࿡�� ������ ���� �� ��ȣ]
				//EXTERN LONG (__stdcall *cemMsUnregisterSlave)	(__in LONG Axis);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemMsUnregisterSlave", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemMsUnregisterSlave([MarshalAs(UnmanagedType.I4)]int Axis);
			
				// ��� ��� �࿡ ���ؼ�, Master/Slave ���� ������ Slave �� ��� ���� ȯ��.
				// [SlaveAxis: Slave ���¸� Ȯ���� ��� ���� �� ��ȣ, SlaveState: Slave �� ���� ��ȯ]
				//EXTERN LONG (__stdcall *cemMsCheckSlaveState)	(__in LONG SlaveAxis, __out PLONG SlaveState);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemMsCheckSlaveState", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemMsCheckSlaveState([MarshalAs(UnmanagedType.I4)]int SlaveAxis, [MarshalAs(UnmanagedType.I4)] ref int SlaveState);
			
				// ��� ��� �࿡ ���ؼ�, Master/Slave ���� ������ Master ���� ���� �� ��ȣ�� Ȯ��.
				// [SlaveAxis: Slave ���� �� ��ȣ, MasterAxis: Master ���� �� ��ȣ]
				//EXTERN LONG (__stdcall *cemMsMasterAxis_Get)	(__in LONG SlaveAxis, __out PLONG MasterAxis);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemMsMasterAxis_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemMsMasterAxis_Get([MarshalAs(UnmanagedType.I4)]int SlaveAxis, [MarshalAs(UnmanagedType.I4)] ref int MasterAxis);
			
				//****************************************************************************
				//*************** START OF Manual Pulsar FUNCTION SECTION ********************
				//****************************************************************************
			
				// Manual Pulsar �Է� ��ȣ�� ���� ȯ�漳��.
				// [Channel: ���� �� ��ȣ, InputMode: Pulsar �Է� ��ȣ�� �Է� ���, IsInverse: Pulsar �Է� ��ȣ�� ��Ÿ���� ����� ����� ���� ��ġ ����]
				//EXTERN LONG (__stdcall *cemPlsrInMode_Set)	(__in LONG Channel, __in  LONG InputMode,   __in  LONG IsInverse);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemPlsrInMode_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemPlsrInMode_Set([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)]int InputMode, [MarshalAs(UnmanagedType.I4)]int IsInverse);
			
				// Manual Pulsar �Է� ��ȣ�� ���� ȯ�漳�� ��ȯ.
				// [Channel: ���� �� ��ȣ, InputMode: Pulsar �Է� ��ȣ�� �Է� ��� ��ȯ, Pulsar �Է� ��ȣ�� ��Ÿ���� ����� ����� ���� ��ġ ���� ��ȯ]
				//EXTERN LONG (__stdcall *cemPlsrInMode_Get)	(__in LONG Channel, __out PLONG InputMode, __out  PLONG IsInverse);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemPlsrInMode_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemPlsrInMode_Get([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)] ref int InputMode, [MarshalAs(UnmanagedType.I4)] ref int IsInverse);
			
				// Manual Pulsar�� PA/PB �Է��޽� ���, ����޽� ���� ���� ���� ����.
				// [Channel: ���� �� ��ȣ, GainFactor: PMG ȸ�ο� �����Ǵ� ����� ����(1~32), DivFactor: PDIV ȸ�ο� �����Ǵ� ����� ����(1~2048)]
				//EXTERN LONG (__stdcall *cemPlsrGain_Set)	(__in LONG Channel, __in  LONG GainFactor,  __in  LONG DivFactor);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemPlsrGain_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemPlsrGain_Set([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)]int GainFactor, [MarshalAs(UnmanagedType.I4)]int DivFactor);
			
				// Manual Pulsar�� PA/PB �Է��޽� ���, ����޽� ���� ���� ���� ���� ��ȯ.
				// [Channel: ���� �� ��ȣ, GainFactor: PMG ȸ�ο� �����Ǵ� ����� ���� ��ȯ, DivFactor: PDIV ȸ�ο� �����Ǵ� ����� ���� ��ȯ]
				//EXTERN LONG (__stdcall *cemPlsrGain_Get)	(__in LONG Channel, __out  PLONG GainFactor, __out PLONG DivFactor);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemPlsrGain_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemPlsrGain_Get([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)] ref int GainFactor, [MarshalAs(UnmanagedType.I4)] ref int DivFactor);
			
				// Manual Pulsar �Է� ��ȣ�� ���� �������� �̼�. �� �Լ��� ����� ���۽�Ų �Ŀ� �ٷ� ��ȯ�˴ϴ�.
				// [Channel: ���� �� ��ȣ, HomeType: Pulsar Input�� ���� ���� ���� ���� ���]
				//EXTERN LONG (__stdcall *cemPlsrHomeMoveStart)	(__in LONG Channel, __in  LONG  HomeType);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemPlsrHomeMoveStart", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemPlsrHomeMoveStart([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)]int HomeType);
			
				// Manual Pulsar �Է� ��ȣ�� ���� �����ǥ �̼�. �� �Լ��� ����� �Ϸ�Ǳ� ������ ��ȯ���� �ʽ��ϴ�.
				// [Channel: ���� �� ��ȣ, Distance: �̵��� �Ÿ�(�����ǥ), IsBlocking: �Ϸ�ɶ����� �޼��� ��� ����]
				//EXTERN LONG (__stdcall *cemPlsrMove)	(__in LONG Channel, __in DOUBLE Distance, __in LONG IsBlocking);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemPlsrMove", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemPlsrMove([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.R8)]double Distance, [MarshalAs(UnmanagedType.I4)]int IsBlocking);
			
				// Manual Pulsar �Է� ��ȣ�� ���� �����ǥ �̼�. �� �Լ��� ����� ���۽�Ų �Ŀ� �ٷ� ��ȯ�˴ϴ�.
				// [Channel: ���� �� ��ȣ, Distance: �̵��� �Ÿ�(�����ǥ)]
				//EXTERN LONG (__stdcall *cemPlsrMoveStart)	(__in LONG Channel, __in DOUBLE Distance);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemPlsrMoveStart", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemPlsrMoveStart([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.R8)]double Distance);
			
				// Manual Pulsar �Է� ��ȣ�� ���� ������ǥ �̼�. �� �Լ��� ����� �Ϸ�Ǳ� ������ ��ȯ���� �ʽ��ϴ�.
				// [Channel: ���� �� ��ȣ, Position: �̵��� ��ġ(������ǥ), IsBlocking: �Ϸ�ɶ����� �޼��� ��� ����]
				//EXTERN LONG (__stdcall *cemPlsrMoveTo)         (__in LONG Channel, __in DOUBLE Position, __in LONG IsBlocking);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemPlsrMoveTo", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemPlsrMoveTo([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.R8)]double Position, [MarshalAs(UnmanagedType.I4)]int IsBlocking);
			
				// Manual Pulsar �Է� ��ȣ�� ���� ������ǥ �̼�. �� �Լ��� ����� ���۽�Ų �Ŀ� �ٷ� ��ȯ�˴ϴ�.
				// [Channel: ���� �� ��ȣ, Position: �̵��� ��ġ(������ǥ)]
				//EXTERN LONG (__stdcall *cemPlsrMoveToStart)    (__in LONG Channel, __in DOUBLE Position);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemPlsrMoveToStart", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemPlsrMoveToStart([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.R8)]double Position);
			
				// Manual Pulsar �Է� ��ȣ�� ���� ���Ӽӵ� �̼�. �� �Լ��� ����� ���۽�Ų �Ŀ� �ٷ� ��ȯ�˴ϴ�.
				// [Channel: ���� �� ��ȣ]
				//EXTERN LONG (__stdcall *cemPlsrVMoveStart)	(__in LONG Channel);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemPlsrVMoveStart", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemPlsrVMoveStart([MarshalAs(UnmanagedType.I4)]int Channel);
			
				//****************************************************************************
				//*************** START OF OVERRIDE FUNCTION DECLARATIONS ********************
				//****************************************************************************
			
				// ���� �̼� �۾� �߿� �ӵ��� ����.
				// [Axis: ���� �� ��ȣ]
				//EXTERN LONG (__stdcall *cemOverrideSpeedSet)	(__in LONG Axis);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemOverrideSpeedSet", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemOverrideSpeedSet([MarshalAs(UnmanagedType.I4)]int Axis);
			
				// ���� �̼� �۾� �߿� ��� ��鿡 ���Ͽ� ���ÿ� �ӵ��� ����.
				// [NumAxes : ���ÿ� �۾��� ������ ��� ���� ��, AxisList : ���ÿ� �۾��� ������ ��� ���� �迭 �ּ�]
				// EXTERN LONG (__stdcall *cemOverrideSpeedSetAll)	(__in LONG NumAxes, __in LONG AxisList);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemOverrideSpeedSetAll", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemOverrideSpeedSetAll([MarshalAs(UnmanagedType.I4)]int NumAxes, [MarshalAs(UnmanagedType.I4)]int AxisList);
			
				// ��������ǥ �̼� �߿�, �����ǥ���� ��ǥ �� �Ÿ����� ����.
				// [Axis: ���� �� ��ȣ, NewDistance: ���ο� ��ǥ �Ÿ�(�����ǥ), IsHardApply: Override �Ұ� ��������, Override ������ ������ ���� ����, AppliedState: Override ���� ���� ���� ��ȯ]
				//EXTERN LONG (__stdcall *cemOverrideMove)	(__in LONG Axis, __in DOUBLE NewDistance, __in LONG IsHardApply, __out PLONG AppliedState);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemOverrideMove", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemOverrideMove([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.R8)]double NewDistance, [MarshalAs(UnmanagedType.I4)]int IsHardApply, [MarshalAs(UnmanagedType.I4)] ref int AppliedState);
			
				// ����������ǥ �̼� �߿�, ������ǥ���� ��ǥ � �Ÿ����� ����.
				// [Axis: ���� �� ��ȣ, NewPosition: ���ο� ��ǥ �Ÿ�(������ǥ), IsHardApply: Override �Ұ� ��������, Override ������ ������ ���� ����, AppliedState: Override ���� ���� ���� ��ȯ]
				//EXTERN LONG (__stdcall *cemOverrideMoveTo)	(__in LONG Axis, __in DOUBLE NewPosition, __in LONG IsHardApply, __out PLONG AppliedState);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemOverrideMoveTo", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemOverrideMoveTo([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.R8)]double NewPosition, [MarshalAs(UnmanagedType.I4)]int IsHardApply, [MarshalAs(UnmanagedType.I4)] ref int AppliedState);
			
				//****************************************************************************
				//*************** START OF MONITORING FUNCTION DECLARATIONS ******************
				//****************************************************************************
			
				// ��� ���� ������ �ϵ���� ī���� ��(�޽���) ����. Target ���� ceSDKDef.h ���Ͽ� ���ǵ� enum _TCnmCntr �� ����Ʈ�� ����.
				// [Axis: ���� �� ��ȣ, Target: ������ ī���� ��ȣ, Count: ��� ī���� ��(PPS)]
				//EXTERN LONG (__stdcall *cemStCount_Set)	(__in LONG Axis, __in LONG Target, __in LONG Count);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemStCount_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemStCount_Set([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int Target, [MarshalAs(UnmanagedType.I4)]int Count);
			
				// ��� ���� ������ �ϵ���� ī���� ��(�޽���) ��ȯ. Target ���� ceSDKDef.h ���Ͽ� ���ǵ� enum _TCnmCntr �� ����Ʈ�� ����.
				// [Axis: ���� �� ��ȣ, Source: ���� ���� ī���� ��ȣ, Count: ��� ī���� ��(PPS) ��ȯ]
				//EXTERN LONG (__stdcall *cemStCount_Get)	(__in LONG Axis, __in LONG Source, __out PLONG Count);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemStCount_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemStCount_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int Source, [MarshalAs(UnmanagedType.I4)] ref int Count);
			
				// ��� ���� ������ ���� ī���� ��(���� �Ÿ�) ����. Target ���� ceSDKDef.h ���Ͽ� ���ǵ� enum _TCnmCntr �� ����Ʈ�� ����.
				// [Axis: ���� �� ��ȣ, Target: ������ ī���� ��ȣ, Position: ��� ī���� ��(���� �Ÿ�)]
				//EXTERN LONG (__stdcall *cemStPosition_Set)	(__in LONG Axis, __in LONG Target, __in DOUBLE Position);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemStPosition_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemStPosition_Set([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int Target, [MarshalAs(UnmanagedType.R8)]double Posotion);
			
				// ��� ���� ������ ���� ī���� ��(���� �Ÿ�) ��ȯ. Target ���� ceSDKDef.h ���Ͽ� ���ǵ� enum _TCnmCntr �� ����Ʈ�� ����.
				// [Axis: ���� �� ��ȣ, Source: ���� ���� ī���� ��ȣ, Position: ��� ī���� ��(���� �Ÿ�) ��ȯ]
				//EXTERN LONG (__stdcall *cemStPosition_Get)	(__in LONG Axis, __in LONG Source, __out PDOUBLE Position);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemStPosition_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemStPosition_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int Source, [MarshalAs(UnmanagedType.R8)] ref double Position);
			
				// ��� ���� ���� �ӵ� ��ȯ. Target ���� ceSDKDef.h ���Ͽ� ���ǵ� enum _TCnmCntr �� ����Ʈ�� ����.
				// [Axis: ���� �� ��ȣ, Source: �ӵ� ��ȯ����� �Ǵ� ī���� ��ȣ, Speed: ������ Source�� �ӵ�(���� �ӵ�) ��ȯ]
				//EXTERN LONG (__stdcall *cemStSpeed_Get)		(__in LONG Axis, __in LONG Source, __out PDOUBLE Speed);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemStSpeed_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemStSpeed_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int Source, [MarshalAs(UnmanagedType.R8)] ref double Speed);
			
				// ��� ���� ��� ���� ���� ��ȯ.
				// [Axis: ���� �� ��ȣ, MotStates: ��� ���� ��ȯ]
				//EXTERN LONG (__stdcall *cemStReadMotionState)	(__in LONG Axis, __out PLONG MotStates);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemStReadMotionState", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemStReadMotionState([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int MotStates);
			
				// ��� ���� ��� ���� I/O ���� ��ȯ.
				// [Axis: ���� �� ��ȣ, MiotStates: Machine I/O ���� ��ȯ]
				//EXTERN LONG (__stdcall *cemStReadMioStatuses)	(__in LONG Axis, __out PLONG MioStates);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemStReadMioStatuses", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemStReadMioStatuses([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int MioStates);
			
				// ��� ���ۻ��¿� ���õ� ���ڿ� ��ȯ. cemStReadMotionState() �Լ��� ���� ���� ��� ����.
				// [MstCode: ��� ���� ���� ��, Buffer: ��� ���ۻ��¸� ���� ���ڿ� ������ �ּ� ��ȯ, Bufferlen: ���ڿ� ������ ����]
				//EXTERN LONG (__stdcall *cemStGetMstString)	(__in LONG MstCode, __out PCHAR Buffer, __in LONG BufferLen);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemStGetMstString", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
                public static extern unsafe int cmmStGetMstString([MarshalAs(UnmanagedType.I4)] int MstCode, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I1)] byte[] Buffer, [MarshalAs(UnmanagedType.I4)] int BufferLen);
			
				// ���ŵ� I/O �޽��� ���� ��ȯ.
				// [IOMessageCount: I/O �޽��� ��]
				//EXTERN LONG (__stdcall *cemStReadIOMessageCount)	(__out PDWORD IOMessageCount);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemStReadIOMessageCount", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemStReadIOMessageCount([MarshalAs(UnmanagedType.I4)] ref int IOMessageCount);
			
				//****************************************************************************
				//*************** START OF LTC FUNCTION DECLARATIONS *************************
				//****************************************************************************
			
				// �ش����� LTC ī���� Ȱ��ȭ ���� Ȯ�� [Axis: ��(ä��)��ȣ, IsLatched: ī���� Ȱ��ȭ ���� ��ȯ]
				//EXTERN LONG (__stdcall *cemLtcIsLatched)	(__in LONG Axis, __out PLONG IsLatched);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemLtcIsLatched", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemLtcIsLatched([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int IsLatched);
			
				// �ش����� LTC ī���� ���� Ȯ�� [Axis: ��(ä��)��ȣ, Counter: ��� ī����, LatchedPos: ������ ���� ��ġ�� ī��Ʈ �� ��ȯ]
				//EXTERN LONG (__stdcall *cemLtcReadLatch)	(__in LONG Axis, __in LONG Counter, __out PDOUBLE LatchedPos);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemLtcReadLatch", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemLtcReadLatch([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int Counter, [MarshalAs(UnmanagedType.I4)] ref int LatchedPos);
			
				//****************************************************************************
				//*************** START OF ADVANCED FUNCTION DECLARATIONS ********************
				//****************************************************************************
			
				// �ش� �࿡ ERC ��ȣ�� ����մϴ�.
				//EXTERN LONG (__stdcall *cemAdvErcOut)				      (__in LONG Axis);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemAdvErcOut", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemAdvErcOut([MarshalAs(UnmanagedType.I4)]int Axis);
			
				// �ش� �࿡ ERC ��ȣ�� ����� �����մϴ�.
				//EXTERN LONG (__stdcall *cemAdvErcReset)				  (__in LONG Axis);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemAdvErcReset", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemAdvErcReset([MarshalAs(UnmanagedType.I4)]int Axis);
			
				// Undocument Function �Դϴ�. �� �Լ��� ��� �����̳� �� ���������� ���˴ϴ�.
				//EXTERN LONG (__stdcall *cemAdvManualPacket)			  (__in LONG NodeID, __in LONG CommandNo, __in PDOUBLE SendBuffer, __in LONG NumSendData, __out PDOUBLE RecvBuffer, __out LONG NumRecvData, __in LONG SendFlag, __in LONG RecvFlag);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemAdvManualPacket", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemAdvManualPacket([MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)]int CommandNo, [MarshalAs(UnmanagedType.I4)] ref int SendBuffer, 
					[MarshalAs(UnmanagedType.I4)]int NumSendData, [MarshalAs(UnmanagedType.I4)] ref int RecvBuffer, [MarshalAs(UnmanagedType.I4)]int NumRecvData, [MarshalAs(UnmanagedType.I4)]int SendFlag, [MarshalAs(UnmanagedType.I4)]int RecvFlag);
			
				//****************************************************************************
				//*************** START OF SYSTEM DIO CONFIGURATION FUNCTION DECLARATIONS ****
				//****************************************************************************
				// �ý��� ������ ������� �����Ǵ� ��ǰ���� ���� ä�ο� ���ؼ� �Է��� Ȯ���մϴ�.
				//EXTERN LONG (__stdcall *cemDiOne_Get)       (__in LONG Channel,     __out PLONG State);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemDiOne_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemDiOne_Get([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)] ref int State);
			
				// �ý��� ������ ������� �����Ǵ� ��ǰ���� ���� ä�ο� ���ؼ� �Է��� Ȯ���մϴ�.
				//EXTERN LONG (__stdcall *cemDiMulti_Get)		(__in LONG IniChannel,	__in LONG NumChannels, __out PLONG InputState);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemDiMulti_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemDiMulti_Get([MarshalAs(UnmanagedType.I4)]int IniChannel, [MarshalAs(UnmanagedType.I4)]int NumChannels, [MarshalAs(UnmanagedType.I4)] ref int InputState);
			
				// �ý��� ������ ������� �����Ǵ� ��ǰ���� ���� ä�ο� ���ؼ� ����� �߻��մϴ�.
				//EXTERN LONG (__stdcall *cemDoOne_Put)		(__in LONG Channel,		__in LONG OutState);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemDoOne_Put", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemDoOne_Put([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)]int OutState);
			
				// �ý��� ������ ������� �����Ǵ� ��ǰ���� ���� ä�ο� ���ؼ� ����� Ȯ���մϴ�.
				//EXTERN LONG (__stdcall *cemDoOne_Get)		(__in LONG Channel,		__in PLONG OutState);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemDoOne_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemDoOne_Get([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)] ref int OutState);
			
				// �ý��� ������ ������� �����Ǵ� ��ǰ���� ���� ä�ο� ���ؼ� ����� �߻��մϴ�.
				//EXTERN LONG (__stdcall *cemDoMulti_Put)		(__in LONG IniChannel,	__in LONG NumChannels, __in LONG OutStates);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemDoMulti_Put", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemDoMulti_Put([MarshalAs(UnmanagedType.I4)]int IniChannel, [MarshalAs(UnmanagedType.I4)]int NumChannels, [MarshalAs(UnmanagedType.I4)]int OutStates);
			
				// �ý��� ������ ������� �����Ǵ� ��ǰ���� ���� ä�ο� ���ؼ� ����� Ȯ���մϴ�.
				//EXTERN LONG (__stdcall *cemDoMulti_Get)		(__in LONG IniChannel,	__in LONG NumChannels, __out PLONG OutStates);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemDoMulti_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemDoMulti_Get([MarshalAs(UnmanagedType.I4)]int IniChannel, [MarshalAs(UnmanagedType.I4)]int NumChannels, [MarshalAs(UnmanagedType.I4)] ref int OutStates);
			
				//****************************************************************************
				//*************** START OF DIO CONFIGURATION FUNCTION DECLARATIONS ***********
				//****************************************************************************
			
				// ��� ������ ����� ä���� ����� ���(Input/Output) ����.
				// [Channel: ���� ä�� ��ȣ, InOutMode: ����� ���]
				//EXTERN LONG (__stdcall *cedioMode_Set)	(__in LONG Channel, __in LONG InOutMode);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cedioMode_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cedioMode_Set([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)]int InOutMode);
			
				// ��� ������ ����� ä���� ����� ���(Input/Output) ���� ���� ��ȯ.
				// [Channel: ���� ä�� ��ȣ, InOutMode: ����� ��� ��ȯ]
				//EXTERN LONG (__stdcall *cedioMode_Get)	(__in LONG Channel, __out PLONG InOutMode);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cedioMode_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cedioMode_Get([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)] ref int InOutMode);
			
				// ��� ������ ����� ä�� ������ ���� ����� ä�ο� ���� ���ÿ� ����� ���(Input/Output) ����.
				// [IniChan: ���� ���� ä�� ��ȣ, NumChan: ��� ä�� ����, InOutModeMask: ����� ��� ����ũ��]
				//EXTERN LONG (__stdcall *cedioModeMulti_Set) (__in LONG IniChan, __in LONG NumChan, __in LONG InOutModeMask);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cedioModeMulti_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cedioModeMulti_Set([MarshalAs(UnmanagedType.I4)]int IniChan, [MarshalAs(UnmanagedType.I4)]int NumChan, [MarshalAs(UnmanagedType.I4)]int InOutModeMask);
			
				// ��� ������ ����� ä�� ������ ���� ����� ä�ο� ���� ���ÿ� ����� ���(Input/Output) ���� ���� ��ȯ.
				// [IniCahn: ���� ���� ä�� ��ȣ, NumChan: ��� ä�� ����, InOutModeMask: ����� ��� ����ũ�� ��ȯ]
				//EXTERN LONG (__stdcall *cedioModeMulti_Get) (__in LONG IniChan, __in LONG NumChan, __out PLONG InOutModeMask);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cedioModeMulti_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cedioModeMulti_Get([MarshalAs(UnmanagedType.I4)]int IniChan, [MarshalAs(UnmanagedType.I4)]int NumChan, [MarshalAs(UnmanagedType.I4)] ref int InOutModeMask);
			
				// ��� ������ ����� ä���� ����� ��(Logic) ����.
				// [Channel: ���� ä�� ��ȣ, InputLogic: ����� ����]
				//EXTERN LONG (__stdcall *cedioLogicOne_Set)	(__in LONG Channel, __in  LONG Logic);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cedioLogicOne_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cedioLogicOne_Set([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)]int Logic);
			
				// ��� ������ ����� ä���� ����� ��(Logic) ���� ���� ��ȯ.
				// [Channel: ���� ä�� ��ȣ, InputLogic: ����� ���� ��ȯ]
				//EXTERN LONG (__stdcall *cedioLogicOne_Get)	(__in LONG Channel, __out PLONG Logic);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cedioLogicOne_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cedioLogicOne_Get([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)] ref int Logic);
			
				// ��� ������ ����� ä�� ������ ���� ����� ä�ο� ���� ���ÿ� ����� ��(Logic) ����.
				// [IniChan: ���� ���� ä�� ��ȣ, NumChan: ��� ä�� ����, LogicMask: ����� ��(Logic) ����ũ]
				//EXTERN LONG (__stdcall *cedioLogicMulti_Set)	(__in LONG IniChan, __in LONG NumChan, __in  LONG  LogicMask);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cedioLogicMulti_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cedioLogicMulti_Set([MarshalAs(UnmanagedType.I4)]int IniChan, [MarshalAs(UnmanagedType.I4)]int NumChan, [MarshalAs(UnmanagedType.I4)]int LogicMask);
			
				// ��� ������ ����� ä�� ������ ���� ����� ä�ο� ���� ���ÿ� ����� ��(Logic) ���� ���� ��ȯ
				// ������ ����� ä���� ����³� �������� ��ȯ [IniChan: ���� ���� ä�� ��ȣ, NumChan: ä�� ����, LogicMask: ���� ����ũ]
				//EXTERN LONG (__stdcall *cedioLogicMulti_Get)	(__in LONG IniChan, __in LONG NumChan, __out PLONG LogicMask);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cedioLogicMulti_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cedioLogicMulti_Get([MarshalAs(UnmanagedType.I4)]int IniChan, [MarshalAs(UnmanagedType.I4)]int NumChan, [MarshalAs(UnmanagedType.I4)] ref int LogicMask);
			
				// ��� ������ ����� ä���� �Է� �Ǵ� ��� ���� ��ȯ.
				// [Channel: ���� ä�� ��ȣ, OutState: ä�� ���� ��ȯ]
				//EXTERN LONG (__stdcall *cedioOne_Get)	(__in LONG Channel, __out PLONG State);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cedioOne_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cedioOne_Get([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)] ref int State);
			
				// ��� ������ ����� ä���� �Է� �Ǵ� ��� ���� ����.
				// [Channel: ���� ä�� ��ȣ, OutState: ä�� ����]
				//EXTERN LONG (__stdcall *cedioOne_Put)	(__in LONG Channel, __in  LONG  State);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cedioOne_Put", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cedioOne_Put([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)]int State);
			
				// ��� ������ ����� ä�� ������ ���� ����� ä�ο� ���� ���ÿ� �Է� �Ǵ� ��� ���¸� ��ȯ.
				// [IniChan: ���� ���� ä�� ��ȣ, NumChan: ��� ä�� ����, States: ä�� ���� ��ȯ]
				//EXTERN LONG (__stdcall *cedioMulti_Get)	(__in LONG IniChan, __in LONG NumChan, __out PLONG States);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cedioMulti_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cedioMulti_Get([MarshalAs(UnmanagedType.I4)]int IniChan, [MarshalAs(UnmanagedType.I4)]int NumChan, [MarshalAs(UnmanagedType.I4)] ref int States);
			
				// ��� ������ ����� ä�� ������ ���� ����� ä�ο� ���� ���ÿ� �Է� �Ǵ� ��� ���¸� ����.
				// [ IniChan: ���� ���� ä�� ��ȣ, NumChan: ��� ä�� ����, States: ä�� ����]
				//EXTERN LONG (__stdcall *cedioMulti_Put)	(__in LONG IniChan, __in LONG NumChan, __in  LONG  States);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cedioMulti_Put", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cedioMulti_Put([MarshalAs(UnmanagedType.I4)]int IniChan, [MarshalAs(UnmanagedType.I4)]int NumChan, [MarshalAs(UnmanagedType.I4)]int States);
			
				// ��� ������ ����� ä���� ������ ���� ����� Ȱ��ȭ �Ͽ� ������ �Է� �Ǵ� ��� ���¸� ��ȯ.
				// [Channel: ���� ä�� ��ȣ, CutoffTime_us: ������ �Է� ��ȣ ���� �ð�(us), State: ä�� ���� ��ȯ]
				//EXTERN LONG (__stdcall *cedioOneF_Get)	(__in LONG Channel,    __in LONG CutoffTime_us, __out PLONG State);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cedioOneF_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cedioOneF_Get([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)]int CutoffTime_us, [MarshalAs(UnmanagedType.I4)] ref int State);
			
				// ��� ������ ����� ä�� ������
				// ���� ������ ����� ä���� ������ ���� ���
				// [IniChan: ���� ���� ä�� ��ȣ, NumChan: ä�� ��, CutoffTime_us: ������ �Է� ��ȣ ���� �ð�(us), InputStates: �ش� ä���� ������ �Է� ����]
				//EXTERN LONG (__stdcall *cedioMultiF_Get)	(__in LONG IniChan, __in LONG NumChan, __in LONG CutoffTime_us, __out PLONG States);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cedioMultiF_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cedioMultiF_Get([MarshalAs(UnmanagedType.I4)]int IniChan, [MarshalAs(UnmanagedType.I4)]int NumChan, [MarshalAs(UnmanagedType.I4)]int CutoffTime_us, [MarshalAs(UnmanagedType.I4)] ref int States);
			
				// ��� ������ ����� ä���� ���� ������ ä���� ���� ���� �޽� ����� �߻�
				// [Channel: ���� ä�� ��ȣ, IsOnPulse: ������ ������ ��� ���� ���� �ʱ� �޽� ����� ���¸� ����, Duration: �޽� ��� �ð� ����,
				// IsWaitPulseEnd: �޽� ��� ���۽ÿ� �Լ��� �ٷ� ��ȯ�� ������, �ƴϸ� �޽� ��� �ð����� �Լ� ��ȯ�� ����� ������ ����]
				//EXTERN LONG (__stdcall *cedioPulseOne)	(__in LONG Channel, __in LONG IsOnPulse, __in LONG Duration,  __in LONG IsWaitPulseEnd);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cedioPulseOne", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cedioPulseOne([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)]int IsOnPulse, [MarshalAs(UnmanagedType.I4)]int Duration, [MarshalAs(UnmanagedType.I4)]int IsWaitPulseEnd);
			
				// ������ ��� ��ä�� ������ ������ ���� ������ ä���� ���� ���� �޽� ����� �߻�
				// [IniChan: ���� ä��, NumChan: ä�� ��, OutStates: ������ ��� ����, Duration: �޽� ��� �ð� ����,
				// IsWaitPulseEnd: �޽� ��� ���۽ÿ� �Լ��� �ٷ� ��ȯ�� ������, �ƴϸ� �޽� ��� �ð����� �Լ� ��ȯ�� ����� ������ ����]
				//EXTERN LONG (__stdcall *cedioPulseMulti)	(__in LONG IniChan, __in LONG NumChan,   __in LONG OutStates, __in LONG Duration, __in LONG IsWaitPulseEnd);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cedioPulseMulti", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cedioPulseMulti([MarshalAs(UnmanagedType.I4)]int IniChan, [MarshalAs(UnmanagedType.I4)]int NumChan, [MarshalAs(UnmanagedType.I4)]int OutStates, [MarshalAs(UnmanagedType.I4)]int Duration, [MarshalAs(UnmanagedType.I4)]int IsWaitPulseEnd);
			
				//****************************************************************************
				//*************** START OF ANALOG INPUT FUNCTION DECLARATIONS ****************
				//****************************************************************************
			
				// �Ƴ��α� �Է¿� ���� ���� ������ ������ ��带 ���� �����մϴ�.
				// [Channel: �Ƴ��α� �Է� ä�� ��ȣ, RangeMode: ���� ���� ���� ���]
				//RangeMode (0~3)
				//A	B	�Է¹���
				//0	0	+10V ~ -10V
				//0	1	+5V ~ -5V
				//1	0	+2.5V ~ -2.5V
				//1	1	0V ~ +10V (0~20mA)
				//EXTERN LONG (__stdcall *ceaiVoltRangeMode_Set)		(__in LONG Channel, __in  LONG  RangeMode);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceaiVoltRangeMode_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceaiVoltRangeMode_Set([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)]int RangeMode);
			
				// �Ƴ��α� �Է¿� ���� ������ ���� ������ �ش��ϴ� ��带 ��ȯ�մϴ�.
				// [Channel: �Ƴ��α� �Է� ä�� ��ȣ, RangeMode: ���� ���� ���� ���]
				//EXTERN LONG (__stdcall *ceaiVoltRangeMode_Get)		(__in LONG Channel, __out PLONG RangeMode);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceaiVoltRangeMode_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceaiVoltRangeMode_Get([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)] ref int RangeMode);
			
				// �Ƴ��α� �Է¿� ���� ������ �Է� Range �� Digit ������ ��ȯ�մϴ� => �� �Լ��� ���Ŀ� ������� ���� �� �ֽ��ϴ�.
				// [Channel: �Ƴ��α� �Է� ä�� ��ȣ, DigitMin: �ּ� �Է� Digit ��, DigitMax: �ִ� �Է� Digit ��]
				//EXTERN LONG (__stdcall *ceaiRangeDigit_Get)         (__in LONG Channel, __out PLONG DigitMin, __out PLONG DigitMax);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceaiRangeDigit_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceaiRangeDigit_Get([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)] ref int DigitMin, [MarshalAs(UnmanagedType.I4)] ref int DigitMax);
			
				// �ش� �Ƴ��α� �Է� ä���� �Է� Digit ���� ��ȯ�մϴ�.
				// [Channel: �Ƴ��α� �Է� ä�� ��ȣ, Digit: �Էµ� Digit ��]
				//EXTERN LONG (__stdcall *ceaiDigit_Get)              (__in LONG Channel, __out PLONG Digit);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceaiDigit_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceaiDigit_Get([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)] ref int Digit);
			
				// �ش� �Ƴ��α� �Է� ä���� �Է� ���� ���� ��ȯ�մϴ�.
				// [Channel: �Ƴ��α� �Է� ä�� ��ȣ, fVolt: �Էµ� ���� ��]
				//EXTERN LONG (__stdcall *ceaiVolt_Get)               (__in LONG Channel, __out PDOUBLE fVolt);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceaiVolt_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceaiVolt_Get([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)] ref int fVolt);
			
				// �ش� �Ƴ��α� �Է� ä���� �Է� ���� ���� ��ȯ�մϴ�.
				// [Channel: �Ƴ��α� �Է� ä�� ��ȣ, fCurrent: �Էµ� ���� ��]
				//EXTERN LONG (__stdcall *ceaiCurrent_Get)            (__in LONG Channel, __out PDOUBLE fCurrent);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceaiCurrent_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceaiCurrent_Get([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)] ref int fCurrent);
			
				//****************************************************************************
				//*************** START OF ANALOG OUTPUT FUNCTION DECLARATIONS ***************
				//****************************************************************************
			
				// �ش� �Ƴ��α� ��� ä���� ���� Digit ���� ����մϴ�.
				// [Channel: �Ƴ��α� ��� ä�� ��ȣ, Digit: ��� Digit ��]
				//EXTERN LONG (__stdcall *ceaoDigit_Out)              (__in LONG Channel, __in LONG Digit);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceaoDigit_Out", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceaoDigit_Out([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)]int Digit);
			
				// �ش� �Ƴ��α� ��� ä���� ���� ���� ���� ����մϴ�.
				// [Channel: �Ƴ��α� ��� ä�� ��ȣ, fVolt: ��� ���� ��]
				//EXTERN LONG (__stdcall *ceaoVolt_Out)               (__in LONG Channel, __in DOUBLE fVolt);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceaoVolt_Out", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceaoVolt_Out([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.R8)]double fVolt);
			
				// �ش� �Ƴ��α� ��� ä���� ���� ���� ���� ����մϴ�.
				// [Channel: �Ƴ��α� ��� ä�� ��ȣ, fCurrent: ��� ���� ��]
				//EXTERN LONG (__stdcall *ceaoCurrent_Out)            (__in LONG Channel, __in DOUBLE fCurrent);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceaoCurrent_Out", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceaoCurrent_Out([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.R8)]double fCurrent);
			
				//****************************************************************************
				//*************** START OF UTILITY FUNCTION DECLARATIONS *********************
				//****************************************************************************
			
				// �ִ� 32����Ʈ�� ������ ���ڿ��� ����� ���� ���� ��ġ�� ����մϴ�.
				// [NodeID : ��� ��� ��ȣ]
				// [NumByte: ����� ������ ����(����Ʈ ����)]
				// [szText : ����� ���ڿ�]
				//EXTERN LONG (__stdcall *ceutlUserData_Set)		(__in LONG NodeID, __in  LONG NumByte,  __in  PCHAR szText);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceutlUserData_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceutlUserData_Set([MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)]int NumByte, [MarshalAs(UnmanagedType.I4)] ref int szText);
			
				// �ִ� 32����Ʈ�� ������ ���ڿ��� ����� ���� ���� ��ġ���� �о�ɴϴ�.
				// [NodeID : ��� ��� ��ȣ]
				// [NumByte: �о�� ������ ����(����Ʈ ����)]
				// [szText : �о�� ���ڿ� ��ȯ]
				//EXTERN LONG (__stdcall *ceutlUserData_Get)		(__in LONG NodeID, __out PLONG NumByte, __out PCHAR szText);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceutlUserData_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceutlUserData_Get([MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)] ref int NumByte, [MarshalAs(UnmanagedType.I4)] ref int szText);
			
				// ��� ���� ��忡 ����� ���� ������ ����մϴ�.
				// [NodeID : ��� ��� ��ȣ]
				// [Version: ����� ����]
				//EXTERN LONG (__stdcall *ceutlUserVersion_Set)	(__in LONG NodeID, __in  LONG Version);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceutlUserVersion_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceutlUserVersion_Set([MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)]int Version);
			
				// ��� ���� ����� ����� ���� ������ �о�ɴϴ�.
				// [NodeID : ��� ��� ��ȣ]
				// [Version: ��ϵ� ���� ��ȯ]
				//EXTERN LONG (__stdcall *ceutlUserVersion_Get)	(__in LONG NodeID, __out PLONG pVersion);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceutlUserVersion_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceutlUserVersion_Get([MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)] ref int pVersion);
			
				// ��� ���� ����� �߿��� ������ �о�ɴϴ�.
				// [NodeID : ��� ��� ��ȣ]
				// [Version: ��ϵ� �߿��� ���� ��ȯ]
				//EXTERN LONG (__stdcall *ceutlNodeVersion_Get)	(__in LONG NodeID, __out PLONG pVersion);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceutlNodeVersion_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceutlNodeVersion_Get([MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)] ref int pVersion);
			
				// �� ���̺귯���� ������ �����ɴϴ�. ���� �� ���� 4����Ʈ�� �ּҿ� �� 2����Ʈ�� ������ �Ҵ��մϴ�. ������ �ڸ����� �� 4�ڸ��Դϴ�.
				// ���̺귯�� ������ ��� ����� ������ �����ϴ�.
				// [pVersionMS : ���� ��Ʈ ���� ���� ��ȯ]
				// [pVersionLS : ���� ��Ʈ ���� ���� ��ȯ]
			
				// printf("Dynamic Link Library Version = [%d].[%d].[%d].[%d]"
				//	_X(pVersionMS >> 16 & 0xFF)
				//	_X(pVersionMS >> 0 & 0xFF)
				//	_X(pVersionLS >> 16 & 0xFF)
				//	_X(pVersionLS >> 0 & 0xFF));
				//EXTERN LONG (__stdcall *ceutlLibVersion_Get)		(__out PLONG pVersionMS, __out PLONG pVersionLS);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceutlLibVersion_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceutlLibVersion_Get([MarshalAs(UnmanagedType.I4)] ref int pVersionMS, [MarshalAs(UnmanagedType.I4)] ref int pVersionLS);
			
				// ���� ������ �޼����� ó���մϴ�.
				//EXTERN LONG (__stdcall *ceutlPumpSingleMessage)     ();
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceutlPumpSingleMessage", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceutlPumpSingleMessage();
			
				// ���� ������ �޼����� ó���մϴ�. ���ڷ� ������ nTimeout �� ������ �ð����� ������ �޼����� ó���ϰ� �˴ϴ�.
				// ���� nTimeout�� CN_INFINITE �� �����ϰԵǸ�, ��� ������ �޼����� ó���� �� ��ȯ�˴ϴ�.
				// nTimeout �� ������ ms �Դϴ�
				// [nTimeout : ������ �ð����� ������ �޼����� ó���մϴ�.]
				//EXTERN LONG (__stdcall *ceutlPumpMultiMessage)      (__in LONG nTimeout);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceutlPumpMultiMessage", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceutlPumpMultiMessage([MarshalAs(UnmanagedType.I4)]int nTimeout);
			
				// ��� ����� ����ȭ �ϱ� ���� ���� ī��Ʈ�� ��ȯ
				// [NodeID : ��� ���, pSyncCount : ��� ����� ���� ī��Ʈ ��ȯ]
				//EXTERN LONG (__stdcall *ceutlSyncCount_Get)		(__in LONG NodeID, __out PLONG pSyncCount);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceutlSyncCount_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceutlSyncCount_Get([MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)] ref int pSyncCount);
			
				// ��� ����� ����ȭ �ϱ� ���� I/O �޼��� ���� ī��Ʈ�� ��ȯ
				// [NodeID : ��� ���, pSyncCount : ��� ����� ���� ī��Ʈ ��ȯ]
				//EXTERN LONG (__stdcall *ceutlIOSyncCount_Get)		(__in LONG NodeID, __out PLONG pSyncCount);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceutlIOSyncCount_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceutlIOSyncCount_Get([MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)] ref int pSyncCount);
			
				// ���� ���� �������α׷��� ��� ����ȭ�� ���� ó���մϴ�. 
				// ���� ��ɵ� ��� ����� �ֱ����� �޼����� ����ȭ�� ���� ����մϴ�.
				// [NodeID : ��� ���, IsBlocking : ���⸦ ���� ����ϴ� ���� ������ �޼��� Blocking ����]
				//EXTERN LONG (__stdcall *ceutlSyncWait)		   (__in LONG NodeID, __in LONG IsBlocking);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceutlSyncWait", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceutlSyncWait([MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)]int IsBlocking);
    }
}

