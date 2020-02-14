using System;
using System.Runtime.InteropServices;

namespace Shared
{
	/// <summary>
	/// VC6.0 에서 제작된 ceSDKDLL.dll 파일을 CSharp 에서 사용하기 위한 함수 마샬링입니다.
	/// </summary>
    public unsafe class CEDLL
    {
        //====================== General FUNCTIONS ====================================================//
        // 1. ceGnLoad
        // 라이브러리가 로드된 상태에서 장치를 로드.
        [DllImport("ceSDKDLL.dll", EntryPoint = "ceGnLoad", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern unsafe int ceGnLoad();

        // 2. ceGnUnload
        // 라이브러리가 로드된 상태에서 장치를 언로드.
        [DllImport("ceSDKDLL.dll", EntryPoint = "ceGnUnload", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern unsafe int ceGnUnload();

        // 3. ceGnReSearchDevice
				// 라이브러리가 로드된 상태에서 노드를 다시 탐색합니다.
				// [RealNode: 실제 노드 정보, nTimeout: 타임 아웃, IsBlocking: 완료될때까지 메세지 블록 여부, pResultNode: 로드된 노드 수 반환]
        [DllImport("ceSDKDLL.dll", EntryPoint = "ceGnReSearchDevice", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern unsafe int ceGnReSearchDevice([MarshalAs(UnmanagedType.I4)]int RealNode, [MarshalAs(UnmanagedType.U4)] uint nTimeout, [MarshalAs(UnmanagedType.I4)]int IsBlocking, [MarshalAs(UnmanagedType.I4)] ref int pResultNode);

        // 4. ceGnIsSearchedDevice
				// 노드 탐색이 완료되어 있는지 확인합니다. 다중 프로세스에서 다른 프로세스에서 이미 노드가 탐색되었다면, 이 함수를 통해
				// 이미 탐색된 노드를 대상으로 재 탐색을 하거나 하지 않도록 할 수 있습니다.
				// [IsSearchedDevice: 이미 노드 탐색이 되어 있는 경우 TRUE 를 반환하며, 탐색이 되어 있지 않은 경우 FALSE 를 반환합니다]  
        [DllImport("ceSDKDLL.dll", EntryPoint = "ceGnIsSearchedDevice", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern unsafe int ceGnIsSearchedDevice([MarshalAs(UnmanagedType.I4)] ref int pIsSearchedDevice);

        // 5. ceGnSearchDevice
				// 장치가 로드된 상태에서 장치의 환경 정보를 반환.
				// [RealNode: 실제 노드 정보, nTimeout: 타임 아웃, IsBlocking: 완료될때까지 메세지 블록 여부, pResultNode: 전체 노드 수 반환]
        [DllImport("ceSDKDLL.dll", EntryPoint = "ceGnSearchDevice", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern unsafe int ceGnSearchDevice([MarshalAs(UnmanagedType.I4)]int RealNode, [MarshalAs(UnmanagedType.I4)]uint nTimeout, [MarshalAs(UnmanagedType.I4)]int IsBlocking, [MarshalAs(UnmanagedType.I4)] ref int pResultNode);

        // 6. ceGnUnSearchDevice
				// 노드 탐색을 초기화 하며, 이 함수가 수행된 이후 ceGnSearchDevice 를 통해 노드 탐색을 할 수 있습니다.
        [DllImport("ceSDKDLL.dll", EntryPoint = "ceGnUnSearchDevice", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern unsafe int ceGnUnSearchDevice();

        // 7. ceGnTotalNode
				// 로드된 전체 노드의 개수를 반환.
				// [Node : 로드된 노드 수 반환]
        [DllImport("ceSDKDLL.dll", EntryPoint = "ceGnTotalNode", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern unsafe int ceGnTotalNode([MarshalAs(UnmanagedType.I4)] ref int pNumNodes);

        // 7-2. ceGnTotalMotionChannel
        // 로드된 모션 축 개수를 반환.
        // [Channel : 로드된 모션 축 개수]
        [DllImport("ceSDKDLL.dll", EntryPoint = "ceGnTotalMotionChannel", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern unsafe int ceGnTotalMotionChannel([MarshalAs(UnmanagedType.I4)] ref int pNumChannels);

        // 8. ceGnTotalDIOChannel
				// 로드된 디지털 입출력 채널 개수를 반환.
				// [Channel : 로드된 디지털 입출력 채널 개수 반환]
        [DllImport("ceSDKDLL.dll", EntryPoint = "ceGnTotalDIOChannel", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern unsafe int ceGnTotalDIOChannel([MarshalAs(UnmanagedType.I4)] ref int pNumChannels);

        // 9. ceGnTotalAIChannel
				// 로드된 아날로그 입력 채널 개수를 반환.
				// [Channel : 로드된 아날로그 입력 채널 개수 반환]
        [DllImport("ceSDKDLL.dll", EntryPoint = "ceGnTotalAIChannel", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern unsafe int ceGnTotalAIChannel([MarshalAs(UnmanagedType.I4)] ref int pNumChannels);

        // 10. ceGnTotalAOChannel
				// 로드된 아날로그 출력 채널 개수를 반환.
				// [Channel : 로드된 아날로그 출력 채널 개수 반환]
        [DllImport("ceSDKDLL.dll", EntryPoint = "ceGnTotalAOChannel", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern unsafe int ceGnTotalAOChannel([MarshalAs(UnmanagedType.I4)] ref int pNumChannels);

        // 11. ceGnModuleCount_Dio
				// 해당 노드의 디지털 입출력 모듈 개수를 반환.
				// [NodeID : 노드 번호, ModuleCount : 해당 노드의 디지털 입출력 모듈 개수 반환]
        [DllImport("ceSDKDLL.dll", EntryPoint = "ceGnModuleCount_Dio", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern unsafe int ceGnModuleCount_Dio([MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)] ref int pNumModules);

        // 12. ceGnModuleCount_Ai
       	// 해당 노드의 아날로그 입력 모듈 개수를 반환.
				// [NodeID : 노드 번호, ModuleCount : 해당 노드의 아날로그 입력 모듈 개수 반환]
        [DllImport("ceSDKDLL.dll", EntryPoint = "ceGnModuleCount_Ai", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern unsafe int ceGnModuleCount_Ai([MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)] ref int pNumModules);

        // 13. ceGnModuleCount_Ao
				// 해당 노드의 아날로그 출력 모듈 개수를 반환.
				// [NodeID : 노드 번호, ModuleCount : 해당 노드의 아날로그 출력 모듈 개수 반환]
        [DllImport("ceSDKDLL.dll", EntryPoint = "ceGnModuleCount_Ao", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern unsafe int ceGnModuleCount_Ao([MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)] ref int pNumModules);

        // 14. ceGnChannelCount_Dio
				// 해당 노드의 디지털 입출력 모듈에 대한 채널 개수를 반환 .
				// [NodeID : 노드 번호, ModuleIdx : 디지털 입출력 모듈 번호, ChannelCount : 디지털 입출력 채널 개수 반환]
        [DllImport("ceSDKDLL.dll", EntryPoint = "ceGnChannelCount_Dio", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern unsafe int ceGnChannelCount_Dio([MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)]int ModuleIdx, [MarshalAs(UnmanagedType.I4)] ref int pNumChannels);

        // 15. ceGnChannelCount_Ai
				// 해당 노드의 아날로그 입력 모듈에 대한 채널 개수를 반환.
				// [NodeID : 노드 번호, ModuleIdx : 아날로그 입력 모듈 번호, ChannelCount : 아날로그 입력 채널 개수 반환]
        [DllImport("ceSDKDLL.dll", EntryPoint = "ceGnChannelCount_Ai", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern unsafe int ceGnChannelCount_Ai([MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)]int ModuleIdx, [MarshalAs(UnmanagedType.I4)] ref int pNumChannels);

        // 16. ceGnChannelCount_Ao
				// 해당 노드의 아날로그 출력 모듈에 대한 채널 개수를 반환.
				// [NodeID : 노드 번호, ModuleIdx : 아날로그 출력 모듈 번호, ChannelCount : 아날로그 출력 채널 개수 반환]
        [DllImport("ceSDKDLL.dll", EntryPoint = "ceGnChannelCount_Ao", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern unsafe int ceGnChannelCount_Ao([MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)]int ModuleIdx, [MarshalAs(UnmanagedType.I4)] ref int pNumChannels);

        // 17. ceGnNodeIsActive
				// 해당 노드가 연결되어 있는지 단절되어 있는지 확인하는 함수
				// [NodeID : 대상 노드, IsActive : 연결 혹은 단절 상태]
        [DllImport("ceSDKDLL.dll", EntryPoint = "ceGnNodeIsActive", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern unsafe int ceGnNodeIsActive([MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)] ref int pIsActive);

        // 18. ceGnLocalDIO_Get
				// 해당 디지털 I/O 채널에 대한 노드 및 모듈 정보를 반환.
				// [Channel : 통합 디지털 입출력 채널 번호, NodeIP : 노드 IP 주소 반환, NodeID : 노드 번호 반환, NodeInGlobal : 해당 노드의 통합 디지털 입출력 채널 번호 반환, ModuleIdx : 해당 노드의 모듈 번호 반환, ModuleInCh : 모듈 내 디지털 입출력 채널 번호 반환]
				//EXTERN LONG (__stdcall *ceGnLocalDIO_Get)	(__in LONG Channel,  __out PLONG NodeIP, __out PLONG NodeID, __out PLONG NodeInGlobal, __out PLONG ModuleIdx, __out PLONG ModuleInCh);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceGnLocalDIO_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceGnLocalDIO_Get([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)] ref int NodeIP, [MarshalAs(UnmanagedType.I4)] ref int NodeID, 
					[MarshalAs(UnmanagedType.I4)] ref int NodeInGlobal, [MarshalAs(UnmanagedType.I4)] ref int ModuleIdx, 
					[MarshalAs(UnmanagedType.I4)] ref int ModuleInCh);
			
        // 19. ceGnLocalAI_Get			
				// 해당 아날로그 입력 채널에 대한 노드 및 모듈 정보를 반환.
				// [Channel: 통합 아날로그 입력 채널 번호, NodeIP : 노드 IP 주소 반환, NodeID : 노드 번호 반환, NodeInGlobal : 해당 노드의 통합 아날로그 입력 채널 번호 반환, ModuleIdx : 해당 노드의 모듈 번호 반환, ModuleInCh : 모듈 내 아날로그 입력 채널 번호 반환]
				//EXTERN LONG (__stdcall *ceGnLocalAI_Get)	(__in LONG Channel,  __out PLONG NodeIP, __out PLONG NodeID, __out PLONG NodeInGlobal, __out PLONG ModuleIdx, __out PLONG ModuleInCh);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceGnLocalAI_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceGnLocalAI_Get([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)] ref int NodeIP, [MarshalAs(UnmanagedType.I4)] ref int NodeID,
					[MarshalAs(UnmanagedType.I4)] ref int NodeInGlobal, [MarshalAs(UnmanagedType.I4)] ref int ModuleIdx,
					[MarshalAs(UnmanagedType.I4)] ref int ModuleInCh);
			
        // 20. ceGnLocalAO_Get						
				// 해당 아날로그 출력 채널에 대한 노드 및 모듈 정보를 반환.
				// [Channel: 통합 아날로그 출력 채널 번호, NodeIP : 노드 IP 주소 반환, NodeID : 노드 번호 반환, NodeInGlobal : 해당 노드의 통합 아날로그 출력 채널 번호 반환, ModuleIdx : 해당 노드의 모듈 번호 반환, ModuleInCh : 모듈 내 아날로그 출력 채널 번호 반환]
				//EXTERN LONG (__stdcall *ceGnLocalAO_Get)	(__in LONG Channel, __out PLONG NodeIP,  __out PLONG NodeID, __out PLONG NodeInGlobal, __out PLONG ModuleIdx, __out PLONG ModuleInCh);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceGnLocalAO_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceGnLocalAO_Get([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)] ref int NodeIP, [MarshalAs(UnmanagedType.I4)] ref int NodeID,
					[MarshalAs(UnmanagedType.I4)] ref int NodeInGlobal, [MarshalAs(UnmanagedType.I4)] ref int ModuleIdx, 
					[MarshalAs(UnmanagedType.I4)] ref int ModuleInCh);
			
				// 해당 모션 디지털 입출력 채널에 대한 노드 및 모듈 정보를 반환.
				// [Channel: 통합 카운터 채널 번호, NodeIP : 노드 IP 주소 반환, NodeID : 노드 번호 반환, NodeInGlobal : 해당 노드의 통합 모션 디지털 입출력 채널 번호 반환, ModuleIdx : 해당 노드의 모듈 번호 반환, ModuleInCh : 모듈 내 모션 디지털 입 출력 채널 번호 반환]
				//EXTERN LONG (__stdcall *ceGnLocalMDIO_Get)   (__in LONG Channel, __out PLONG NodeIP,  __out PLONG NodeID, __out PLONG NodeInGlobal, __out PLONG ModuleIdx, __out PLONG ModuleInCh);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceGnLocalMDIO_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceGnLocalMDIO_Get([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)] ref int NodeIP, [MarshalAs(UnmanagedType.I4)] ref int NodeID, 
					[MarshalAs(UnmanagedType.I4)] ref int NodeInGlobal, [MarshalAs(UnmanagedType.I4)] ref int ModuleIdx, 
					[MarshalAs(UnmanagedType.I4)] ref int ModuleInCh);
			
				// 해당 카운터 채널에 대한 노드 및 모듈 정보를 반환.
				// [Channel: 통합 카운터 채널 번호, NodeIP : 노드 IP 주소 반환, NodeID : 노드 번호 반환, NodeInGlobal : 해당 노드의 통합 카운터 채널 번호 반환, ModuleIdx : 해당 노드의 모듈 번호 반환, ModuleInCh : 모듈 내 카운터 출력 채널 번호 반환]
				//EXTERN LONG (__stdcall *ceGnLocalCNT_Get)   (__in LONG Channel, __out PLONG NodeIP,  __out PLONG NodeID, __out PLONG NodeInGlobal, __out PLONG ModuleIdx, __out PLONG ModuleInCh);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceGnLocalCNT_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceGnLocalCNT_Get([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)] ref int NodeIP, [MarshalAs(UnmanagedType.I4)] ref int NodeID, 
					[MarshalAs(UnmanagedType.I4)] ref int NodeInGlobal, [MarshalAs(UnmanagedType.I4)] ref int ModuleIdx, 
					[MarshalAs(UnmanagedType.I4)] ref int ModuleInCh);
			
				// 해당 노드 모션 모듈의 로컬 축 번호를 통하여 통합 축 번호를 반환.
				// [NodeID : 노드 번호, ModuleIdx : 해당 노드의 모듈 번호, ModuleInCh : 모듈 내 모션 제어 축 번호, GlobalAxis : 통합 모션 제어 축 번호 반환]
				//EXTERN LONG (__stdcall *ceGnGlobalAxis_Get)	(__in LONG NodeID, __in  LONG ModuleIdx, __in  LONG ModuleInCh, __out PLONG GlobalAxis);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceGnGlobalAxis_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceGnGlobalAxis_Get([MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)]int ModuleIdx, [MarshalAs(UnmanagedType.I4)]int ModuleIxCh, [MarshalAs(UnmanagedType.I4)] ref int GlobalAxis);
			
				// 해당 노드 디지털 입출력 모듈의 로컬 채널 번호를 통하여 통합 채널 번호를 반환.
				// [NodeID : 노드 번호, ModuleIdx : 해당 노드의 모듈 번호, ModuleInCh : 모듈 내 디지털 입출력 채널 번호, GlobalDIO : 통합 디지털 입출력 채널 번호 반환]
				//EXTERN LONG (__stdcall *ceGnGlobalDIO_Get)	(__in LONG NodeID, __in  LONG ModuleIdx, __in  LONG ModuleInCh, __out PLONG GlobalDIO);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceGnGlobalDIO_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceGnGlobalDIO_Get([MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)]int ModuleIdx, [MarshalAs(UnmanagedType.I4)]int ModuleInCh, [MarshalAs(UnmanagedType.I4)] ref int GlobalDIO);
			
				// 해당 노드 아날로그 입력 모듈의 로컬 채널 번호를 통하여 통합 채널 번호를 반환.
				// [NodeID : 노드 번호, ModuleIdx : 해당 노드의 모듈 번호, ModuleInCh : 모듈 내 아날로그 입력 채널 번호, GlobalAI : 통합 아날로그 입력 채널 번호 반환]
				//EXTERN LONG (__stdcall *ceGnGlobalAI_Get)	(__in LONG NodeID, __in  LONG ModuleIdx, __in  LONG ModuleInCh, __out PLONG GlobalAI);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceGnGlobalAI_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceGnGlobalAI_Get([MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)]int ModuleIdx, [MarshalAs(UnmanagedType.I4)]int ModuleInCh, [MarshalAs(UnmanagedType.I4)] ref int GlobalAI);
			
				// 해당 노드 아날로그 출력 모듈의 로컬 채널 번호를 통하여 통합 채널 번호를 반환.
				// [NodeID : 노드 번호, ModuleIdx : 해당 노드의 모듈 번호, ModuleInCh : 모듈 내 아날로그 출력 채널 번호, GlobalAO : 통합 아날로그 출력 채널 번호 반환]
				//EXTERN LONG (__stdcall *ceGnGlobalAO_Get)	(__in LONG NodeID, __in  LONG ModuleIdx, __in  LONG ModuleInCh, __out PLONG GlobalAO);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceGnGlobalAO_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceGnGlobalAO_Get([MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)]int ModuleIdx, [MarshalAs(UnmanagedType.I4)]int ModuleInCh, [MarshalAs(UnmanagedType.I4)] ref int GlobalAO);
			
				// 해당 노드 모션 디지털 입 출력 모듈의 로컬 채널 번호를 통하여 통합 채널 번호를 반환.
				// [NodeID : 노드 번호, ModuleIdx : 해당 노드의 모듈 번호, ModuleInCh : 모듈 내 모션 디지털 입 출력 채널 번호, GlobalAO : 통합 모션 디지털 입 출력 채널 번호 반환]
				//EXTERN LONG (__stdcall *ceGnGlobalMDIO_Get)	(__in LONG NodeID, __in  LONG ModuleIdx, __in  LONG ModuleInCh, __out PLONG GlobalMDIO);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceGnGlobalMDIO_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceGnGlobalMDIO_Get([MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)]int ModuleIdx, [MarshalAs(UnmanagedType.I4)]int ModuleInCh, [MarshalAs(UnmanagedType.I4)] ref int GlobalMDIO);
			
				// 해당 노드 카운터 모듈의 로컬 채널 번호를 통하여 통합 채널 번호를 반환.
				// [NodeID : 노드 번호, ModuleIdx : 해당 노드의 모듈 번호, ModuleInCh : 모듈 내 카운터 채널 번호, GlobalAO : 통합 카운터 채널 번호 반환]
				//EXTERN LONG (__stdcall *ceGnGlobalCNT_Get)	(__in LONG NodeID, __in  LONG ModuleIdx, __in  LONG ModuleInCh, __out PLONG GlobalCNT);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceGnGlobalCNT_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceGnGlobalCNT_Get([MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)]int ModuleIdx, [MarshalAs(UnmanagedType.I4)]int ModuleInCh, [MarshalAs(UnmanagedType.I4)] ref int GlobalCNT);
			
				//****************************************************************************
				//*************** START OF GENERAL MOTION FUNCTION DECLARATIONS **************
				//****************************************************************************
			
				// SERVO-ON 신호 출력을 인가 혹은 차단.
				// [Channel : 통합 축 번호, Enable : SERVO-ON 신호 출력 ON/OFF]
				//EXTERN LONG (__stdcall *cemGnServoOn_Set)	(__in LONG Channel, __in LONG Enable);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemGnServoOn_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemGnServoOn_Set([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)]int Enable);
			
				// SERVO-ON 신호의 출력 상태를 반환.
				// [Channel: 통합 축 번호, Enable: SERVO-ON 신호의 출력 상태를 반환]
				//EXTERN LONG (__stdcall *cemGnServoOn_Get)	(__in LONG Channel, __in PLONG Enable);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemGnServoOn_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemGnServoOn_Get([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)] ref int Enable);
			
				// 해당 모션 축의 알람 리셋 신호 출력 제어.
				// [Axis : 통합 모션 제어 축 번호, IsReset: 알람 리셋 신호 출력 여부]
				//EXTERN LONG (__stdcall *cemGnAlarmReset) (__in LONG Axis, __in LONG IsReset);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemGnAlarmReset", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemGnAlarmReset([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int IsReset);
			
				//****************************************************************************
				//*************** START OF CONFIGURATION FUNCTION DECLARATIONS ***************
				//****************************************************************************
			
				// 모션 입출력 신호의 환경 설정 구성. PropId는 ceSDKDef.h 파일에 정의된 enum _TCmMioPropId 값 리스트를 참고.
				// [Axis : 통합 축 번호, PropId : 환경 설정 매개 변수, PropVal : PropId로 지정된 환경에 대한 설정값]
				//EXTERN LONG (__stdcall *cemCfgMioProperty_Set)	(__in LONG Axis, __in  LONG PropId, __in  LONG PropVal);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgMioProperty_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgMioProperty_Set([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int PropId, [MarshalAs(UnmanagedType.I4)]int Propval);
			
				// 모션 입출력 신호의 환경 설정 구성 반환. PropId는 ceSDKDef.h 파일에 정의된 enum _TCmMioPropId 값 리스트를 참고.
				// [Axis : 통합 축 번호, PropId : 환경 설정 매개 변수, PropVal : PropId로 지정된 환경에 대한 반환값]
				//EXTERN LONG (__stdcall *cemCfgMioProperty_Get)	(__in LONG Axis, __in  LONG PropId, __out PLONG PropVal);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgMioProperty_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgMioProperty_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int PropId, [MarshalAs(UnmanagedType.I4)] ref int PropVal);
			
				// 각종 I/O(Input/Output) 신호에 대해 노이즈 대응 필터 기능 설정.
				// [Axis : 통합 축 번호, IsEnable : 필터 로직 적용 여부]
				//EXTERN LONG (__stdcall *cemCfgFilter_Set)	(__in LONG Axis, __in LONG IsEnable);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgFilter_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgFilter_Set([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int IsEnable);
			
				// 각종 I/O(Input/Output) 신호에 대해 노이즈 대응 필터 기능 설정 상태를 반환.
				// [Axis : 통합 축 번호, IsEnabled : 필터 로직 적용 여부 반환]
				//EXTERN LONG (__stdcall *cemCfgFilter_Get)	(__in LONG Axis, __out PLONG IsEnabled);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgFilter_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgFilter_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int IsEnabled);
			
				// EA/EB 또는 PA/PB 신호 입력 회로에 노이즈 필터를 적용할 지를 설정.
				// [Channel : 통합 축 번호, Target : 함수의 적용 대상(EA/EB or PA/PB), IsEnable : 필터 로직의 적용 여부]
				//EXTERN LONG (__stdcall *cemCfgFilterAB_Set)	(__in LONG Channel, __in LONG Target, __in LONG IsEnable);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgFilterAB_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgFilterAB_Set([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)]int Target, [MarshalAs(UnmanagedType.I4)]int IsEnable);
			
				// EA/EB 또는 PA/PB 신호 입력 회로에 노이즈 필터를 적용할 지에 대한 설정 상태를 반환
				// [Channel : 통합 축 번호, Target : 함수의 적용 대상(EA/EB or PA/PB), IsEnabled : 필터 로직 적용 여부 반환]
				//EXTERN LONG (__stdcall *cemCfgFilterAB_Get)	(__in LONG Channel, __in LONG Target, __out PLONG IsEnabled);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgFilterAB_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgFilterAB_Get([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)]int Target, [MarshalAs(UnmanagedType.I4)] ref int IsEnabled);
			
				// 인코더 펄스(Feedback Pulse) 신호의 입력 모드 설정.
				// [Axis : 통합 축 번호, InputMode : Feedback Pulse 입력 모드, IsReverse : Feedback Count 값의 UP/DOWN 방향을 반대로 할 것인지 여부]
				//EXTERN LONG (__stdcall *cemCfgInMode_Set)	(__in LONG Axis, __in LONG InputMode, __in LONG IsReverse);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgInMode_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgInMode_Set([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int InputMode, [MarshalAs(UnmanagedType.I4)]int IsReverse);
			
				// 인코더 펄스(Feedback Pulse) 신호의 입력 모드 설정 상태를 반환.
				// [Axis : 통합 축 번호, InputMode: Feedback Pulse 입력 모드, IsReverse : Feedback Count 값의 UP/DOWN 방향을 반대로 할 것인지 여부 반환]
				//EXTERN LONG (__stdcall *cemCfgInMode_Get)	(__in LONG Axis, __out PLONG InputMode, __out PLONG IsReverse);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgInMode_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgInMode_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int InputMode, [MarshalAs(UnmanagedType.I4)] ref int IsReverse);
			
				// 지령 펄스(Command Pulse) 신호 출력 모드 설정.
				// [Axis : 통합 축 번호, OutputMode : Command Pulse 출력 모드]
				//EXTERN LONG (__stdcall *cemCfgOutMode_Set)	(__in LONG Axis, __in LONG OutputMode);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgOutMode_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgOutMode_Set([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int OutputMode);
			
				// 지령 펄스(Command Pulse) 신호 출력 모드 설정 상태를 반환.
				// [Axis : 통합 축 번호, OutputMode: Command 펄스의 출력 모드 반환]
				//EXTERN LONG (__stdcall *cemCfgOutMode_Get)	(__in LONG Axis, __out PLONG OutputMode);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgOutMode_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgOutMode_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int OutputMode);
			
				// 이송 목표 좌표의 기준을 커맨드 위치로 할지 피드백 위치로 할지를 설정.
				// [Axis : 통합 축 번호, CtrlMode : 제어 모드]
				//EXTERN LONG (__stdcall *cemCfgCtrlMode_Set)	(__in LONG Axis, __in LONG CtrlMode);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgCtrlMode_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgCtrlMode_Set([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int CtrlMode);
			
				// 이송 목표 좌표의 기준을 커맨드 위치로 할지 피드백 위치로 할지에 대한 설정 상태를 반환.
				// [Axis : 통합 축 번호, CtrlMode : 제어 모드]
				//EXTERN LONG (__stdcall *cemCfgCtrlMode_Get)	(__in LONG Axis, __out PLONG CtrlMode);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgCtrlMode_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgCtrlMode_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int CtrlMode);
			
				// 입력 펄스(Feedback Pulse)와 출력 펄스(Command Pulse)의 분해능율 설정.
				// [Axis : 통합 축 번호, Ratio : Feedback Pulse와 Command Pulse의 분해능 비율]
				//EXTERN LONG (__stdcall *cemCfgInOutRatio_Set)	(__in LONG Axis, __in DOUBLE Ratio);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgInOutRatio_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgInOutRatio_Set([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.R8)]double Ratio );
			
				// 입력 펄스(Feedback Pulse)와 출력 펄스(Command Pulse)의 분해능 설정 상태를 반환.
				// [Axis : 통합 축 번호, Ratio : Feedback Pulse와 Command Pulse의 분해능 비율을 반환]
				//EXTERN LONG (__stdcall *cemCfgInOutRatio_Get)	(__in LONG Axis, __out PDOUBLE Ratio);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgInOutRatio_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgInOutRatio_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int Ratio);
			
				// 논리적 거리 단위를 설정.
				// [Axis : 통합 축 번호, UnitDist : 논리적 거리 1을 이동하기 위한 출력 펄스 수]
				//EXTERN LONG (__stdcall *cemCfgUnitDist_Set)	(__in LONG Axis, __in DOUBLE UnitDist);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgUnitDist_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgUnitDist_Set([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.R8)]double UnitDist );
			
				// 논리적 거리 단위를 반환.
				// [Axis : 통합 축 번호, UnitDist : 논리적 거리 1을 이동하기 위한 출력 펄스 수를 반환]
				//EXTERN LONG (__stdcall *cemCfgUnitDist_Get)	(__in LONG Axis, __out PDOUBLE UnitDist);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgUnitDist_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgUnitDist_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int UnitDist);
			
				// 논리적 속도 단위를 설정.
				// [Axis : 통합 축 번호, UnitSpeed : 단위 속도에 대한 펄스 출력 속도(PPS)]
				//EXTERN LONG (__stdcall *cemCfgUnitSpeed_Set)	(__in LONG Axis, __in DOUBLE UnitSpeed);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgUnitSpeed_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgUnitSpeed_Set([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.R8)]double UnitSpeed );
			
				// 논리적 속도 단위를 반환.
				// [Axis : 통합 축 번호, UnitSpeed : 단위 속도에 대한 펄스 출력 속도(PPS)를 반환]
				//EXTERN LONG (__stdcall *cemCfgUnitSpeed_Get)	(__in LONG Axis, __out PDOUBLE UnitSpeed);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgUnitSpeed_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgUnitSpeed_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int UnitSpeed);
			
				// 모션 속도 제한범위를 설정.
				// [Axis : 통합 축 번호, MaxPPS : 모션 최고 속도(PPS)]
				//EXTERN LONG (__stdcall *cemCfgSpeedRange_Set)	(__in LONG Axis, __in  DOUBLE MaxPPS);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgSpeedRange_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgSpeedRange_Set([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.R8)]double MaxPPS );
			
				// 모션 속도 제한범위 설정 상태를 반환.
				// [Axis : 통합 축 번호, MinPPS : 모션 최저 속도(PPS) 반환, MaxPPS : 모션 최고 속도(PPS) 반환]
				//EXTERN LONG (__stdcall *cemCfgSpeedRange_Get)	(__in LONG Axis, __out PDOUBLE MinPPS, __out PDOUBLE MaxPPS);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgSpeedRange_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgSpeedRange_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int MinPPS, [MarshalAs(UnmanagedType.I4)] ref int MaxPPS);
			
				// 모션 이송 기준 속도 설정.
				// [Axis : 통합 축 번호, SpeedMode : 속도 모드, WorkSpeed : 작업 속도, Accel : 가속도, Decel : 감속도]
				//EXTERN LONG (__stdcall *cemCfgSpeedPattern_Set)	(__in LONG Axis, __in  LONG SpeedMode, __in  DOUBLE  WorkSpeed, __in  DOUBLE Accel,  __in  DOUBLE Decel);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgSpeedPattern_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgSpeedPattern_Set([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int SpeedMode, [MarshalAs(UnmanagedType.R8)]double WorkSpeed, 
					[MarshalAs(UnmanagedType.R8)]double Acc, [MarshalAs(UnmanagedType.R8)]double Dec );
			
				// 모션 이송 기준 속도 설정 상태를 반환.
				// [Axis : 통합 축 번호, SpeedMode : 속도 모드 반환, WorkSpeed : 작업 속도 반환, Accel : 가속도 반환, Decel : 감속도 반환]
				//EXTERN LONG (__stdcall *cemCfgSpeedPattern_Get)	(__in LONG Axis, __out PLONG SpeedMode,__out PDOUBLE WorkSpeed, __out PDOUBLE Accel, __out PDOUBLE Decel);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgSpeedPattern_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgSpeedPattern_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int SpeedMode, [MarshalAs(UnmanagedType.I4)] ref int WorkSpeed, 
					[MarshalAs(UnmanagedType.I4)] ref int Accel, [MarshalAs(UnmanagedType.I4)] ref int Decel);
			
				// 입력 펄스(Feedback Pulse)를 통해 모션의 실제속도 검출 설정.
				// [Axis : 통합 축 번호, IsEnable : Feedback 속도 확인 기능 활성 여부, Interval : Feedback 펄스 수 확인 주기(ms)]
				//EXTERN LONG (__stdcall *cemCfgActSpdCheck_Set)	(__in LONG Axis, __in LONG IsEnable, __in LONG Interval);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgActSpdCheck_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgActSpdCheck_Set([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int IsEnable, [MarshalAs(UnmanagedType.I4)]int Interval);
			
				// 입력 펄스(Feedback Pulse)를 통한 모션 실제속도 검출 설정 상태를 반환.
				// [Axis : 통합 축 번호, IsEnable : Feedback 속도 확인 기능 활성여부를 반환, Interval : Feedback 펄스 수 확인 주기(ms) 반환]
				//EXTERN LONG (__stdcall *cemCfgActSpdCheck_Get)	(__in LONG Axis, __out PLONG IsEnable, __out PLONG Interval);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgActSpdCheck_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgActSpdCheck_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int IsEnable, [MarshalAs(UnmanagedType.I4)] ref int Interval);
			
				// 모션의 이송 범위를 소프트웨어적인 이송제한범위를 설정하여 제한.
				// [Axis : 통합 축 번호, IsEnable : 소프트웨어 리밋 기능 활성 여부, LimitN : (-)방향 Limit값, LimitP : (+)방향 Limit값]
				//EXTERN LONG (__stdcall *cemCfgSoftLimit_Set)	(__in LONG Axis, __in LONG IsEnable, __in DOUBLE LimitN, __in DOUBLE LimitP);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgSoftLimit_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgSoftLimit_Set([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int IsEnable, [MarshalAs(UnmanagedType.R8)]double LimitN, [MarshalAs(UnmanagedType.R8)]double LimitP );
			
				// 모션의 소프트웨어적인 이송제한범위에 대한 설정을 반환.
				// [Axis : 통합 축 번호, IsEnable : 소프트웨어 리밋 기능 활성 여부 반환, LimitN : (-)방향 Limit값 반환, LimitP : (+)방향 Limit값 반환]
				//EXTERN LONG (__stdcall *cemCfgSoftLimit_Get)	(__in LONG Axis, __out PLONG IsEnable, __out PDOUBLE LimitN, __out PDOUBLE LimitP);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgSoftLimit_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgSoftLimit_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int IsEnable, [MarshalAs(UnmanagedType.I4)] ref int LimitN,
					[MarshalAs(UnmanagedType.I4)] ref int LimitP);
			
				// 링카운터(Ring-Counter) 기능 설정.
				// [Channel : 통합 축 번호, TargCntr : 함수의 적용 대상(Command or Feedback Counter), IsEnable : Ring-Counter 기능 활성 여부, CntMax: 카운터 범위]
				//EXTERN LONG (__stdcall *cemCfgRingCntr_Set)	(__in LONG Channel, __in LONG TargCntr, __in LONG IsEnable, __in DOUBLE CntMax);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgRingCntr_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgRingCntr_Set([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)]int TargCntr, [MarshalAs(UnmanagedType.I4)]int IsEnable, [MarshalAs(UnmanagedType.I4)]int CntMax);
			
				// 링카운터(Ring-Counter) 기능 설정 상태를 반환.
				// [Channel: 통합 축 번호, TargCntr: 함수의 적용 대상(Command or Feedback Counter) 선택, IsEnable: Ring-Counter 기능 활성 여부 반환, CntMax: 카운터 범위 반환]
				//EXTERN LONG (__stdcall *cemCfgRingCntr_Get)	(__in LONG Channel, __in LONG TargCntr, __out PLONG IsEnable, __out PDOUBLE CntMax);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgRingCntr_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgRingCntr_Get([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)]int TargCntr, [MarshalAs(UnmanagedType.I4)] ref int IsEnable, [MarshalAs(UnmanagedType.I4)] ref int CntMax);
			
				// 작업속도 보정 시 보정된 작업속도를 산출하는 비례 값 설정.
				// [Axis: 통합 축 번호, fCorrRatio: 속도 보정 비율값(%)]
				//EXTERN LONG (__stdcall *cemCfgVelCorrRatio_Set)	(__in LONG Axis, __in DOUBLE fCorrRatio);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgVelCorrRatio_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgVelCorrRatio_Set([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.R8)]double fCorrRatio );
			
				// 작업속도 보정 시 보정된 작업속도를 산출하는 비례 값을 반환.
				// [Axis: 통합 축 번호, fCorrRatio: 속도 보정 비율값(%) 반환]
				//EXTERN LONG (__stdcall *cemCfgVelCorrRatio_Get)	(__in LONG Axis, __out PDOUBLE fCorrRatio);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgVelCorrRatio_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgVelCorrRatio_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int fCorrRatio);
			
				// 이전의 이송 작업이 완료되지 않은 축에 새로운 이송 명령이 하달되었을 때의 처리 정책을 설정.
				// [seqMode: 시퀀스(Sequence) 모드]
				//EXTERN LONG (__stdcall *cemCfgSeqMode_Set)	(__in LONG SeqMode);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgSeqMode_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgSeqMode_Set([MarshalAs(UnmanagedType.I4)]int SeqMode);
			
				// 이전의 이송 작업이 완료되지 않은 축에 새로운 이송 명령이 하달되었을 때의 처리 정책에 대한 설정을 반환.
				// [seqMode: 시퀀스(Sequence) 모드 반환]
				//EXTERN LONG (__stdcall *cemCfgSeqMode_Get)	(__out PLONG SeqMode);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemCfgSeqMode_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemCfgSeqMode_Get([MarshalAs(UnmanagedType.I4)] ref int SeqMode);
			
				//****************************************************************************
				//*************** START OF HOME RETURN FUNCTION DECLARATIONS *****************
				//****************************************************************************
			
				// 원점복귀 환경 설정.
				// [Axis: 통합 축 번호, HomeMode: 원점복귀 모드(0~12), Dir: 원점복귀 모션 수행 방향 EzCount: EzCount 값(0~15), EscDist: 원점탈출 거리, Offset: 원점복귀 종료 위치를 기준으로 추가 모션 이동값(상대좌표 이송)]
				//EXTERN LONG (__stdcall *cemHomeConfig_Set)	(__in LONG Axis, __in  LONG HomeMode,  __in  LONG Dir,  __in LONG EzCount,  __in  DOUBLE EscDist,  __in  DOUBLE Offset);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemHomeConfig_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemHomeConfig_Set([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int HomeMode, [MarshalAs(UnmanagedType.I4)]int Dir, 
					[MarshalAs(UnmanagedType.I4)]int EzCount, [MarshalAs(UnmanagedType.R8)]double EscDist, [MarshalAs(UnmanagedType.R8)]double Offset);
			
				// 원점복귀 환경 설정 상태를 반환.
				// [Axis: 통합 축 번호, HomeMode: 원점복귀 모드(0~12) 반환, Dir: 원점복귀 모션 수행 방향 반환, EzCount: EzCount 값(0~15) 반환, EscDist: 원점탈출 거리 반환, Offset: 원점복귀 종료 위치를 기준으로 추가 모션 이동값 반환]
				//EXTERN LONG (__stdcall *cemHomeConfig_Get)	(__in LONG Axis, __out PLONG HomeMode, __out PLONG Dir, __out PLONG EzCount, __out PDOUBLE EscDist, __out PDOUBLE Offset);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemHomeConfig_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemHomeConfig_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int HomeMode, [MarshalAs(UnmanagedType.I4)] ref int Dir, 
					[MarshalAs(UnmanagedType.I4)] ref int EzCount, [MarshalAs(UnmanagedType.I4)] ref int EscDist, 
					[MarshalAs(UnmanagedType.I4)] ref int Offset);
			
				// 원점복귀 완료 후 발생하는 모션 컨트롤러와 서보 드라이브간의 제어 편차에 의해 발생한 입력 펄스(Feedback Pulse) 처리에 대한 설정.
				// [Axis: 통합 축 번호, PosClrMode: Command 및 Feedback 위치가 클리어되는 모드]
				//EXTERN LONG (__stdcall *cemHomePosClrMode_Set)	(__in LONG Axis, __in LONG PosClrMode);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemHomePosClrMode_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemHomePosClrMode_Set([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int PosClrMode);
			
				// 원점복귀 완료 후 발생하는 모션 컨트롤러와 서보 드라이브간의 제어 편차에 의해 발생한 입력 펄스(Feedback Pulse) 처리에 대한 설정 상태를 반환.
				//[Axis: 통합 축 번호, PosClrMode: Command 및 Feedback 위치가 클리어 되는 모드 반환]
				//EXTERN LONG (__stdcall *cemHomePosClrMode_Get)	(__in LONG Axis, __out PLONG PosClrMode);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemHomePosClrMode_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemHomePosClrMode_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int PosClrMode);
			
				// 원점 복귀 속도 설정.
				// [Axis: 통합 축 번호, SpeedMode: 원점복귀 속도 모드, Vel: 원점복귀 작업 속도, Accel: 원점복귀 가속도, Decel: 원점복귀 감속도, RevVel: Revers Speed]
				//EXTERN LONG (__stdcall *cemHomeSpeedPattern_Set)	(__in LONG Axis, __in LONG SpeedMode, __in DOUBLE Vel, __in DOUBLE Accel, __in DOUBLE Decel, __in DOUBLE RevVel);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemHomeSpeedPattern_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemHomeSpeedPattern_Set([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int SpeedMode, [MarshalAs(UnmanagedType.R8)]double Vel, 
					[MarshalAs(UnmanagedType.R8)]double Acc, [MarshalAs(UnmanagedType.R8)]double Dec, [MarshalAs(UnmanagedType.R8)]double RevVel);
			
				// 원점 복귀 속도 설정 상태를 반환.
				// [Axis: 통합 축 번호, SpeedMode: 원점복귀 속도 모드 반환, Vel: 원점복귀 작업 속도 반환, Accel:원점복귀 가속도 반환, Decel:원점복귀 감속도 반환, RevVel: Revers Speed 반환]
				//EXTERN LONG (__stdcall *cemHomeSpeedPattern_Get)	(__in LONG Axis, __out PLONG SpeedMode, __out PDOUBLE Vel, __out PDOUBLE Accel, __out PDOUBLE Decel, __out PDOUBLE RevVel);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemHomeSpeedPattern_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemHomeSpeedPattern_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int SpeedMode, [MarshalAs(UnmanagedType.I4)] ref int Vel,
					[MarshalAs(UnmanagedType.I4)] ref int Accel, [MarshalAs(UnmanagedType.I4)] ref int Decel,
					[MarshalAs(UnmanagedType.I4)] ref int RevVel);
			
				// 단축 원점 복귀 이송. 이 함수는 모션이 완료되기 전까지 반환되지 않습니다.
				// [Axis: 통합 축 번호, IsBlocking: 완료될때까지 메세지 블록 여부]
				//EXTERN LONG (__stdcall *cemHomeMove)	(__in LONG Axis, __in LONG IsBlocking);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemHomeMove", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemHomeMove([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int IsBlocking);
			
				// 단축 원점 복귀 이송. 이 함수는 모션을 시작시킨 후에 바로 반환됩니다.
				// [Axis: 통합 축 번호]
				//EXTERN LONG (__stdcall *cemHomeMoveStart)	(__in LONG Axis);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemHomeMoveStart", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemHomeMoveStart([MarshalAs(UnmanagedType.I4)]int Axis);
			
				// 이전에 실행된 원점복귀 구동 완료 상태를 확인.
				// [Axis: 통합 축 번호, IsSuccess: 이전에 실행된 원점 복귀 구동 완료 여부 반환]
				//EXTERN LONG (__stdcall *cemHomeSuccess_Get)	(__in LONG Axis, __out PLONG IsSuccess);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemHomeSuccess_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemHomeSuccess_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int IsSuccess);
			
				// 이전에 실행된 원점복귀 구동 완료 상태를 강제로 설정.
				// [Axis: 통합 축 번호, IsSuccess: 원점 복귀의 성공 여부를 강제로 설정]
				//EXTERN LONG (__stdcall *cemHomeSuccess_Set)	(__in LONG Axis, __in LONG IsSuccess);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemHomeSuccess_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemHomeSuccess_Set([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int IsSuccess);
			
				// 원점복귀 구동 상태를 반환.
				// [Axis: 통합 축 번호, IsBusy: 현재 원점복귀 진행 여부 반환]
				//EXTERN LONG (__stdcall *cemHomeIsBusy)	(__in LONG Axis, __out PLONG IsBusy);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemHomeIsBusy", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemHomeIsBusy([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int IsBusy);
			
				// 원점복귀 완료 시까지 대기.
				// [Channel: 통합 축 번호, IsBlocking: 완료될때까지 메세지 블록 여부]
				//EXTERN LONG (__stdcall *cemHomeWaitDone)	(__in LONG Axis, __in LONG IsBlocking);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemHomeWaitDone", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemHomeWaitDone([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int IsBlocking);
			
				//****************************************************************************
				//*************** START OF SINGLE AXIS CONTROL FUNCTION DECLARATIONS *********
				//****************************************************************************
			
				// 단축 구동 시 해당 축에 대한 속도방식 및 속도비율 설정.
				// [Axis: 통합 축 번호, SpeedMode: 속도 모드, VelRatio: 작업속도 비율(%), AccRatio: 가속도 비율(%), DecRatio: 감속도 비율(%)]
				//EXTERN LONG (__stdcall *cemSxSpeedRatio_Set)	(__in LONG Axis, __in LONG SpeedMode, __in DOUBLE VelRatio, __in DOUBLE AccRatio, __in DOUBLE DecRatio);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemSxSpeedRatio_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemSxSpeedRatio_Set([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int SpeedMode, [MarshalAs(UnmanagedType.R8)]double VelRatio,
					[MarshalAs(UnmanagedType.R8)]double AccRatio, [MarshalAs(UnmanagedType.R8)]double DecRatio);
			
				// 단축 구동 시 해당 축에 대한 속도방식 및 속도비율 설정 상태를 반환.
				// [Axis: 통합 축 번호, SpeedMode: 속도 모드 반환, VelRatio: 작업속도 비율(%) 반환, AccRatio: 가속도 비율(%) 반환, DecRatio: 감속도 비율(%) 반환]
				//EXTERN LONG (__stdcall *cemSxSpeedRatio_Get)	(__in LONG Axis, __out PLONG SpeedMode, __out PDOUBLE VelRatio, __out PDOUBLE AccRatio, __out PDOUBLE DecRatio);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemSxSpeedRatio_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemSxSpeedRatio_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int SpeedMode, [MarshalAs(UnmanagedType.I4)] ref int VelRatio, 
					[MarshalAs(UnmanagedType.I4)] ref int AccRatio, [MarshalAs(UnmanagedType.I4)] ref int DecRatio);
			
				// 단축 상대 좌표 이송. 이 함수는 모션이 완료되기 전까지 반환되지 않습니다.
				// [Axis: 통합 축 번호, Distance: 이동할 거리, IsBlocking: 완료될때까지 메세지 블록 여부]
				//EXTERN LONG (__stdcall *cemSxMove)	(__in LONG Axis, __in DOUBLE Distance, __in LONG IsBlocking);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemSxMove", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemSxMove([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.R8)]double Distance, [MarshalAs(UnmanagedType.I4)]int IsBlocking );
			
				// 단축 상대 좌표 이송. 이 함수는 모션을 시작시킨 후에 바로 반환됩니다.
				// [Axis: 통합 축 번호, Distance: 이동할 거리]
				//EXTERN LONG (__stdcall *cemSxMoveStart)	(__in LONG Axis, __in DOUBLE Distance);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemSxMoveStart", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemSxMoveStart([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.R8)]double Distance );
			
				// 단축 절대 좌표 이송. 이 함수는 모션이 완료되기 전까지 반환되지 않습니다.
				// [Axis: 통합 축 번호, Distance: 이동할 절대 좌표값, IsBlocking: 완료될때까지 메세지 블록 여부]
				//EXTERN LONG (__stdcall *cemSxMoveTo)	(__in LONG Axis, __in DOUBLE Position, __in LONG IsBlocking);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemSxMoveTo", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemSxMoveTo([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.R8)]double Position, [MarshalAs(UnmanagedType.I4)]int IsBlocking );
			
				// 단축 절대 좌표 이송. 이 함수는 모션을 시작시킨 후에 바로 반환됩니다.
				// [Axis: 통합 축 번호, Distance: 이동할 절대 좌표 값]
				//EXTERN LONG (__stdcall *cemSxMoveToStart)	(__in LONG Axis, __in DOUBLE Position);	
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemSxMoveToStart", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemSxMoveToStart([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.R8)]double Position );
			
				// 단축 연속 속도 이송. 이 함수는 모션을 시작시킨 후에 바로 반환됩니다.
				// [Axis: 통합 축 번호, Direction: 모션 이송 방향]
				//EXTERN LONG (__stdcall *cemSxVMoveStart)	(__in LONG Axis, __in LONG Direction);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemSxVMoveStart", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemSxVMoveStart([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int Direction);
			
				// 단축 이송 감속 후 정지.
				// [Axis: 통합 축 번호, IsWaitComplete: 동작 완료될때까지 함수 반환 여부, IsBlocking: 완료될때까지 메세지 블록 여부]
				//EXTERN LONG (__stdcall *cemSxStop)	(__in LONG Axis, __in LONG IsWaitComplete, __in LONG IsBlocking);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemSxStop", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemSxStop([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int IsWaitComplete, [MarshalAs(UnmanagedType.I4)]int IsBlocking);
			
				// 단축 이송 비상 정지.
				// [Axis: 통합 축 번호]
				//EXTERN LONG (__stdcall *cemSxStopEmg)	(__in LONG Axis);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemSxStopEmg", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemSxStopEmg([MarshalAs(UnmanagedType.I4)]int Axis);
			
				// 단축 이송 완료 확인.
				// [Axis: 통합 축 번호, IsDone: 모션 작업 완료 여부 반환]
				//EXTERN LONG (__stdcall *cemSxIsDone)	(__in LONG Axis, __out PLONG IsDone);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemSxIsDone", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemSxIsDone([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int IsDone);
			
				// 단축 이송 완료 시점까지 대기.
				// [Axis: 통합 축 번호, IsBlocking: 완료될때까지 메세지 블록 여부]
				//EXTERN LONG (__stdcall *cemSxWaitDone)	(__in LONG Axis, __in LONG IsBlocking);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemSxWaitDone", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemSxWaitDone([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int IsBlocking);
			
				// 해당 통합 축에 대해 마지막으로 이송한(상대 or 절대 좌표) 위치를 반환.
				// [Channel: 통합 축 번호, Position: 마지막으로 이송한 위치 값 반환]
				//EXTERN LONG (__stdcall *cemSxTargetPos_Get)	(__in LONG Channel, __out PDOUBLE Position);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemSxTargetPos_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemSxTargetPos_Get([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)] ref int Position);
			
				// 단축 모션의 초기 속도 설정.
				// [Axis: 통합 축 번호, IniSpeed: 초기 속도]
				//EXTERN LONG (__stdcall *cemSxOptIniSpeed_Set)	(__in LONG Axis, __in DOUBLE IniSpeed);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemSxOptIniSpeed_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemSxOptIniSpeed_Set([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.R8)]double IniSpeed );
			
				// 단축 모션의 초기 속도 설정을 반환.
				// [Axis: 통합 축 번호, IniSpeed: 초기 속도 반환]
				//EXTERN LONG (__stdcall *cemSxOptIniSpeed_Get)	(__in LONG Axis, __out PDOUBLE IniSpeed);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemSxOptIniSpeed_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemSxOptIniSpeed_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int IniSpeed);
			
				// 단축 모션 상대 좌표 2단계 속도 이송 기능. 이 함수는 모션을 시작시킨 후에 바로 반환됩니다.
				// [Axis: 통합 축 번호, Distance: 이동할 거리(상대 좌표 값), Vel2: 2단계 작업 속도]
				//EXTERN LONG (__stdcall *cemSxMoveStart2V)	(__in LONG Axis, __in  DOUBLE Distance, __in DOUBLE Vel2);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemSxMoveStart2V", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemSxMoveStart2V([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.R8)]double Distance, [MarshalAs(UnmanagedType.R8)]double Vel2);
			
				// 단축 모션 절대 좌표 2단계 속도 이송 기능. 이 함수는 모션을 시작시킨 후에 바로 반환됩니다.
				// [Axis: 통합 태널 번호, Position: 이동할 위치 (절대 좌표 값), Vel2: 2단계 작업 속도]
				//EXTERN LONG (__stdcall *cemSxMoveToStart2V)	(__in LONG Axis, __in  DOUBLE Position, __in DOUBLE Vel2);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemSxMoveToStart2V", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemSxMoveToStart2V([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.R8)]double Position, [MarshalAs(UnmanagedType.R8)]double Vel2);
			
				// 단축 모션의 백래쉬 혹은 슬립 보정을 위해 설정.
				// [Axis: 통합 축 번호, CorrMode: 보정 모드, CorrAmount: 보정 펄스 수, CorrVel: 보정 펄스의 출력 주파수, CntrMask: 보정 펄스 출력시 카운터의 동작 여부]
				//EXTERN LONG (__stdcall *cemSxCorrection_Set)	(__in LONG Axis, __in LONG CorrMode, __in DOUBLE CorrAmount, __in DOUBLE CorrVel, __in LONG CntrMask);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemSxCorrection_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemSxCorrection_Set([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int CorrMode, [MarshalAs(UnmanagedType.R8)]double CorrAmount,
					[MarshalAs(UnmanagedType.R8)]double CorrVel, [MarshalAs(UnmanagedType.I4)]int CntrMask);
			
				// 단축 모션의 백래쉬 혹은 슬립 보정의 설정을 반환.
				// [Axis: 통합 축 번호, CorrMode: 보정 모드 반환, CorrAmount: 보정 펄스 수 반환, CorrVel: 보정 펄스의 출력 주파수 반환, CntrMask: 보정 펄스 출력시 카운터의 동작 여부 반환]
				//EXTERN LONG (__stdcall *cemSxCorrection_Get)	(__in LONG Axis, __out PLONG CorrMode, __out PDOUBLE CorrAmount, __out PDOUBLE CorrVel, __out PLONG CntrMask);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemSxCorrection_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemSxCorrection_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int CorrMode, [MarshalAs(UnmanagedType.I4)] ref int CorrAmount, 
					[MarshalAs(UnmanagedType.I4)] ref int CorrVel, [MarshalAs(UnmanagedType.I4)] ref int CntrMask);
			
				//****************************************************************************
				//*************** START OF INTERPOLATION CONTROL FUNCTION DECLARATIONS *******
				//****************************************************************************
			
				// 보간 대상 축 그룹 설정.
				// [MapIndex: 맵 번호(0~7), NodeID: 노드 번호, MapMask1: 축 맵에 포함할 축들을 지정할 마스크값(하위 32비트, BIT0~BIT31),
				// MapMask2: 축 맵에 포함할 축들을 지정할 마스크값(상위 32비트, BIT32~BIT63)]
				//EXTERN LONG (__stdcall *cemIxMapAxes)	(__in LONG MapIndex,__in LONG NodeID, __in LONG MapMask1, __in LONG MapMask2);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxMapAxes", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxMapAxes([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)]int MapMask1, [MarshalAs(UnmanagedType.I4)]int MapMask2);
			
				// 보간 속도 보정모드 설정 [MapIndex: 맵 번호, 옵션1: VelCorrOpt1, 옵션2: VelCorrOpt2] 
				//EXTERN LONG (__stdcall *cemIxVelCorrMode_Set)	(__in LONG MapIndex, __in LONG VelCorrOpt1, __in LONG VelCorrOpt2);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxVelCorrMode_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxVelCorrMode_Set([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.I4)]int VelCorrOpt1, [MarshalAs(UnmanagedType.I4)]int VelCorrOpt2);
			
				// 보간 속도 보정모드 반환 [MapIndex: 맵 번호, 옵션1: VelCorrOpt1, 옵션2: VelCorrOpt2]
				//EXTERN LONG (__stdcall *cemIxVelCorrMode_Get)	(__in LONG MapIndex, __out PLONG VelCorrOpt1, __out PLONG VelCorrOpt2);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxVelCorrMode_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxVelCorrMode_Get([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.I4)] ref int VelCorrOpt1, [MarshalAs(UnmanagedType.I4)] ref int VelCorrOpt2);
			
				// 보간 대상 축 그룹 설정 해제.
				// [MapIndex: 맵 번호]
				//EXTERN LONG (__stdcall *cemIxUnMap)	(__in LONG MapIndex);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxUnMap", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxUnMap([MarshalAs(UnmanagedType.I4)]int MapIndex);
			
				// 보간 이송 속도 설정.
				// [MapIndex: 맵 번호, IsVectorSpeed: 벡터 혹은 마스터 스피드 모드 설정, SpeedMode: 속도 모드, Vel: 작업 속도, Acc: 가속도, Dec: 감속도]
				//EXTERN LONG (__stdcall *cemIxSpeedPattern_Set)	(__in LONG MapIndex, __in LONG IsVectorSpeed, __in LONG SpeedMode, __in DOUBLE Vel, __in DOUBLE Acc, __in DOUBLE Dec);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxSpeedPattern_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxSpeedPattern_Set([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.I4)]int IsVectorSpeed, [MarshalAs(UnmanagedType.I4)]int SpeedMode,
					[MarshalAs(UnmanagedType.R8)] double Vel, [MarshalAs(UnmanagedType.R8)]double Acc, [MarshalAs(UnmanagedType.R8)]double Dec);
			
				// 보간 이속 속도 설정을 반환.
				// [MapIndex: 맵 번호, IsVectorSpeed: 벡터 혹은 마스터 스피드 모드 설정 반환, SpeedMode: 속도 모드 반환, Vel: 작업 속도 반환, Acc: 가속도 반환, Dec: 감속도 반환]
				//EXTERN LONG (__stdcall *cemIxSpeedPattern_Get)	(__in LONG MapIndex, __out PLONG IsVectorSpeed, __out PLONG SpeedMode, __out PDOUBLE Vel, __out PDOUBLE Acc, __out PDOUBLE Dec);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxSpeedPattern_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxSpeedPattern_Get([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.I4)] ref int IsVectorSpeed, [MarshalAs(UnmanagedType.I4)] ref int SpeedMode, 
					[MarshalAs(UnmanagedType.I4)] ref int Vel, [MarshalAs(UnmanagedType.I4)] ref int Acc, 
					[MarshalAs(UnmanagedType.I4)] ref int Dec);
			
				// 직선 보간 상대좌표 이송. 이 함수는 모션이 완료되기 전까지 반환되지 않습니다.
				// [MapIndex: 맵 번호, DistList: 이동할 거리의(상대좌표) 배열 주소, IsBlocking: 완료될때까지 메세지 블록 여부]
				//EXTERN LONG (__stdcall *cemIxLine)	(__in LONG MapIndex, __in PDOUBLE DistList, __in LONG IsBlocking);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxLine", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxLine([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.I4)] ref int DistList, [MarshalAs(UnmanagedType.I4)]int IsBlocking);
			
				// 직선 보간 상대좌표 이송. 이 함수는 모션을 시작시킨 후에 바로 반환됩니다.
				// [MapIndex: 맵 번호, DistList: 이동할 거리의(상대좌표) 배열 주소]
				//EXTERN LONG (__stdcall *cemIxLineStart)	(__in LONG MapIndex, __in PDOUBLE DistList);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxLineStart", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxLineStart([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.I4)] ref int DistList);
			
				// 직선 보간 절대좌표 이송. 이 함수는 모션이 완료되기 전까지 반환되지 않습니다.
				// [MapIndex: 맵 번호, PosList: 이동할 위치의(절대좌표) 배열 주소, IsBlocking: 완료될때까지 메세지 블록 여부]
				//EXTERN LONG (__stdcall *cemIxLineTo)	(__in LONG MapIndex, __in PDOUBLE PosList, __in LONG IsBlocking);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxLineTo", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxLineTo([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.I4)] ref int PosList, [MarshalAs(UnmanagedType.I4)]int IsBlocking);
			
				// 직선 보간 절대좌표 이송. 이 함수는 모션을 시작시킨 후에 바로 반환됩니다.
				// [MapIndex: 맵 번호, PosList: 이동할 위치의(절대좌표) 배열 주소]
				//EXTERN LONG (__stdcall *cemIxLineToStart)	(__in LONG MapIndex, __in PDOUBLE PosList);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxLineToStart", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxLineToStart([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.I4)] ref int PosList);
			
				// 원호 보간 상대좌표(상대적 중심좌표와 각도) 이송. 이 함수는 모션이 완료되기 전까지 반환되지 않습니다.
				// [MapIndex: 맵 번호, XCentOffset: 현재 위치로부터 원의 중심까지 X축 상대좌표, YCentOffset: 현재 위치에서 원의 중심까지 Y축 상대좌표,
				// EndAngle: 원호 보간을 완료할 목표지점의 현재 위치에 대한 각도(Degree), IsBlocking: 완료될때까지 메세지 블록 여부]
				//EXTERN LONG (__stdcall *cemIxArcA)	(__in LONG MapIndex, __in DOUBLE XCentOffset, __in DOUBLE YCentOffset, __in DOUBLE EndAngle, __in LONG IsBlocking);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxArcA", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxArcA([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.R8)]double XCentOffset, [MarshalAs(UnmanagedType.R8)]double YCentOffset, 
					[MarshalAs(UnmanagedType.R8)]double EndAngle, [MarshalAs(UnmanagedType.I4)]int IsBlocking);
			
				// 원호 보간 상대좌표(상대적 중심좌표와 각도) 이송. 이 함수는 모션을 시작시킨 후에 바로 반환됩니다.
				// [MapIndex: 맵 번호, XCentOffset: 현재 위치로부터 원의 중심까지 X축 상대좌표, YCentOffset: 현재 위치로부터 원의 중심까지 Y축 상대좌표,
				// EndAngle: 원호 보간을 완료할 목표 지점의 현재 위치에 대한 각도(Degree)]
				//EXTERN LONG (__stdcall *cemIxArcAStart)	(__in LONG MapIndex, __in DOUBLE XCentOffset, __in DOUBLE YCentOffset, __in DOUBLE EndAngle);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxArcAStart", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxArcAStart([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.R8)]double XCentOffset, [MarshalAs(UnmanagedType.R8)]double YCentOffset,
					[MarshalAs(UnmanagedType.R8)]double EndAngle);
			
				// 원호 보간 절대좌표(절대적 중심좌표와 각도) 이송. 이 함수는 모션이 완료되기 전까지 반환되지 않습니다.
				// [MapIndex: 맵 번호, XCent: 중심점의 X축 절대좌표, YCent: 중심점의 Y축 절대좌표,
				// EndAngle: 원호 보간을 완료할 목표 지점의 현재 위치에 대한 각도(Degree), IsBlocking: 완료될때까지 메세지 블록 여부]
				//EXTERN LONG (__stdcall *cemIxArcATo)	(__in LONG MapIndex, __in DOUBLE XCent, __in DOUBLE YCent, __in DOUBLE EndAngle, __in LONG IsBlocking);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxArcATo", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxArcATo([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.R8)]double Xcent, [MarshalAs(UnmanagedType.R8)]double YCent,
					[MarshalAs(UnmanagedType.R8)]double EndAngle, [MarshalAs(UnmanagedType.I4)]int IsBlocking);
			
				// 원호 보간 절대좌표(절대적 중심좌표와 각도) 이송. 이 함수는 모션을 시작시킨 후에 바로 반환됩니다.
				// [MapIndex: 맵 번호, XCent: 중심점의 X축 절대좌표, YCent: 중심점의 Y축 절대좌표, EndAngle: 원호 보간을 완료할 목표 지점의 현재 위치에 대한 각도(Degree)]
				//EXTERN LONG (__stdcall *cemIxArcAToStart)	(__in LONG MapIndex, __in DOUBLE XCent, __in DOUBLE YCent, __in DOUBLE EndAngle);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxArcAToStart", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxArcAToStart([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.R8)]double XCent, [MarshalAs(UnmanagedType.R8)]double YCent,
					[MarshalAs(UnmanagedType.R8)]double EndAngle);
			
				// 원호보간 상대좌표(상대적 중심좌표와 종점좌표) 이송. 이 함수는 모션이 완료되기 전까지 반환되지 않습니다.
				// [MapIndex: 맵 번호, XCentOffset: 현재 위치에서 원 중심까지 X축상의 거리, YCentOffset: 현재 위치에서 원 중심까지 Y축상의 거리,
				// XEndPointDist: 원호보간 이동을 완료할 목표지점의 현재 위치로부터 X축상 거리, YEndPointDist: 원호 보간 이동을 완료할 목표지점의 현재위치로 부터의 Y축상 거리, Direction: 회전 방향, IsBlocking: 완료될때까지 메세지 블록 여부 ]
				//EXTERN LONG (__stdcall *cemIxArcP)	(__in LONG MapIndex, __in DOUBLE XCentOffset, __in DOUBLE YCentOffset, __in DOUBLE XEndPointDist, __in DOUBLE YEndPointDist, __in LONG Direction, __in LONG IsBlocking);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxArcP", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxArcP([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.R8)]double XCentOffset, [MarshalAs(UnmanagedType.R8)]double YCentOffset,
					[MarshalAs(UnmanagedType.R8)]double XEndPointDist, [MarshalAs(UnmanagedType.R8)]double YEndPointDist, [MarshalAs(UnmanagedType.I4)]int Direction, [MarshalAs(UnmanagedType.I4)]int IsBlocking);
			
				// 원호보간 상대좌표(상대적 중심좌표와 종점좌표) 이송. 이 함수는 모션을 시작시킨 후에 바로 반환됩니다.
				// [MapIndex: 맵 번호, XCentOffset: 현 위치에서 원 중심까지 X축상의 거리, YCentOffset: 현 위치에서 원 중심까지 Y축상의 거리,
				// XEndPointDist: 원호 보간 이동을 완료할 목표지점의 현재위치로 부터의 X축상 거리, YEndPointDist: 원호 보간 이동을 완료할 목표지점의 현재위치로 부터의 Y축상 거리, Direction: 회전 방향]
				//EXTERN LONG (__stdcall *cemIxArcPStart)	(__in LONG MapIndex, __in DOUBLE XCentOffset, __in DOUBLE YCentOffset, __in DOUBLE XEndPointDist, __in DOUBLE YEndPointDist, __in LONG Direction);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxArcPStart", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxArcPStart([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.R8)]double XCentOffset, [MarshalAs(UnmanagedType.R8)]double YCentOffset,
					[MarshalAs(UnmanagedType.R8)]double XEndPointDist, [MarshalAs(UnmanagedType.R8)]double YEndPointDist, [MarshalAs(UnmanagedType.I4)]int Direction);
			
				// 원호보간 절대좌표(절대적 중심좌표와 종점좌표) 이송. 이 함수는 모션이 완료되기 전까지 반환되지 않습니다.
				// [MapIndex: 맵 번호, XCent: 중심점의 X축 절대좌표, YCent: 중심점의 Y축 절대좌표, XEndPointDist: 원호보간 이동을 완료할 목표지점의 X축 절대좌표,
				// YEndPointDist: 원호보간 이동을 완료할 목표지점의 Y축 절대좌표, Direction: 회전 방향, IsBlocking: 완료될때까지 메세지 블록 여부]
				//EXTERN LONG (__stdcall *cemIxArcPTo)	(__in LONG MapIndex, __in DOUBLE XCent, __in DOUBLE YCent, __in DOUBLE XEndPointDist, __in DOUBLE YEndPointDist, __in LONG Direction, __in LONG IsBlocking);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxArcPTo", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxArcPTo([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.R8)]double XCent, [MarshalAs(UnmanagedType.R8)]double YCent,
					[MarshalAs(UnmanagedType.R8)]double XEndPointDist, [MarshalAs(UnmanagedType.R8)]double YEndPointDist, [MarshalAs(UnmanagedType.I4)]int Direction, [MarshalAs(UnmanagedType.I4)]int IsBlocking);
			
				// 원호보간 절대좌표(절대적 중심좌표와 종점좌표) 이송. 이 함수는 모션을 시작시킨 후에 바로 반환됩니다.
				// [MapIndex: 맵 번호, XCent: 중심점의 X축 절대좌표, YCent: 중심점의 Y축 절대좌표, XEndPointDist: 원호보간 이동을 완료할 목표지점의 X축 절대좌표,
				// YEndPointDist: 원호보간 이동을 완료할 목표지점의 Y축 절대좌표, Direction: 회전 방향]
				//EXTERN LONG (__stdcall *cemIxArcPToStart)	(__in LONG MapIndex, __in DOUBLE XCent, __in DOUBLE YCent, __in DOUBLE XEndPointDist, __in DOUBLE YEndPointDist, __in LONG Direction);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxArcPToStart", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxArcPToStart([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.R8)]double XCent, [MarshalAs(UnmanagedType.R8)]double YCent,
					[MarshalAs(UnmanagedType.R8)]double XEndPointDist, [MarshalAs(UnmanagedType.R8)]double YEndPointDist, [MarshalAs(UnmanagedType.I4)]int Direction);
			
				// 3점(Point)을 통한 원호보간 상대좌표 이송. 이 함수는 모션이 완료되기 전까지 반환되지 않습니다.
				// [MapIndex: 맵 번호, P2X: 현재 위치에서 2번째 점까지 X축 상대좌표, P2Y: 현재 위치에서 2번째 점까지 Y축 상대좌표,
				// P3X: 2번째 점에서 3번째 점까지 X축 상대좌표, P3Y: 2번째 점에서 3번째 점까지 Y축 상대좌표, EndAngle: 각도값, IsBlocking: 완료될때까지 메세지 블록 여부]
				//EXTERN LONG (__stdcall *cemIxArc3P)	(__in LONG MapIndex, __in DOUBLE P2X,__in  DOUBLE P2Y,__in  DOUBLE P3X, __in DOUBLE P3Y, __in DOUBLE EndAngle, __in LONG IsBlocking);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxArc3P", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxArc3P([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.R8)]double P2X, [MarshalAs(UnmanagedType.R8)]double P2Y,
					[MarshalAs(UnmanagedType.R8)]double P3X, [MarshalAs(UnmanagedType.R8)]double P3Y, [MarshalAs(UnmanagedType.R8)]double EndAngle, [MarshalAs(UnmanagedType.I4)]int IsBlocking);
			
				// 3점(Point)을 통한 원호보간 상대좌표 이송. 이 함수는 모션을 시작시킨 후에 바로 반환됩니다.
				// [MapIndex: 맵 번호, P2X: 현재 위치에서 2번째 점까지 X축 상대좌표, P2Y: 현재 위치에서 2번째 점까지 Y축 상대좌표,
				// P3X: 2번째 점에서 3번째 점까지 X축 상대좌표, P3Y: 2번째 점에서 3번째 점까지 Y축 상대좌표, EndAngle: 각도값]
				//EXTERN LONG (__stdcall *cemIxArc3PStart)	(__in LONG MapIndex, __in DOUBLE P2X, __in DOUBLE P2Y, __in DOUBLE P3X,__in  DOUBLE P3Y,__in  DOUBLE EndAngle);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxArc3PStart", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxArc3PStart([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.R8)]double P2X, [MarshalAs(UnmanagedType.R8)]double P2Y,
					[MarshalAs(UnmanagedType.R8)]double P3X, [MarshalAs(UnmanagedType.R8)]double P3Y, [MarshalAs(UnmanagedType.R8)]double EndAngle);
			
				// 3점(Point)을 통한 원호보간 절대좌표 이송. 이 함수는 모션이 완료되기 전까지 반환되지 않습니다.
				// [MapIndex: 맵 번호, P2X: 2번째 점의 X축 절대좌표, P2Y: 2번째 점의 Y축 절대좌표,
				// P3X: 3번째 점의 X축 절대좌표, P3Y: 3번째 점의 Y축 절대좌표, EndAngle: 각도값, IsBlocking: 완료될때까지 메세지 블록 여부]
				//EXTERN LONG (__stdcall *cemIxArc3PTo)	(__in LONG MapIndex, __in DOUBLE P2X,__in  DOUBLE P2Y,__in  DOUBLE P3X, __in DOUBLE P3Y, __in DOUBLE EndAngle, __in LONG IsBlocking);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxArc3PTo", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxArc3PTo([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.R8)]double P2X, [MarshalAs(UnmanagedType.R8)]double P2Y,
					[MarshalAs(UnmanagedType.R8)]double P3X, [MarshalAs(UnmanagedType.R8)]double P3Y, [MarshalAs(UnmanagedType.R8)]double EndAngle, [MarshalAs(UnmanagedType.I4)]int IsBlocking);
			
				// 3점(Point)을 통한 원호보간 절대좌표 이송. 이 함수는 모션을 시작시킨 후에 바로 반환됩니다.
				// [MapIndex: 맵 번호, P2X: 2번째 점의 X축 절대좌표, P2Y: 2번째 점의 Y축 절대좌표, P3X: 3번째 점의 X축 절대좌표, P3Y: 3번째 점의 Y축 절대좌표, EndAngle: 각도값]
				//EXTERN LONG (__stdcall *cemIxArc3PToStart)	(__in LONG MapIndex, __in DOUBLE P2X, __in DOUBLE P2Y, __in DOUBLE P3X, __in DOUBLE P3Y, __in DOUBLE EndAngle);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxArc3PToStart", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxArc3PToStart([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.R8)]double P2X, [MarshalAs(UnmanagedType.R8)]double P2Y,
					[MarshalAs(UnmanagedType.R8)]double P3X, [MarshalAs(UnmanagedType.R8)]double P3Y, [MarshalAs(UnmanagedType.R8)]double EndAngle);
			
				// 보간 제어 구동 이송 감속 후 정지.
				// [MapIndex: 맵 번호]
				//EXTERN LONG (__stdcall *cemIxStop)	(__in LONG MapIndex);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxStop", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxStop([MarshalAs(UnmanagedType.I4)]int MapIndex);
			
				// 보간 제어 구동 이송 비상정지.
				// [MapIndex: 맵 번호]
				//EXTERN LONG (__stdcall *cemIxStopEmg)	(__in LONG MapIndex);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxStopEmg", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxStopEmg([MarshalAs(UnmanagedType.I4)]int MapIndex);
			
				// 보간 제어 구동 이송의 보간 완료 확인.
				// [MapIndex: 맵 번호, IsDone: 보간 제어 구동 이송의 보간 완료 여부]
				//EXTERN LONG (__stdcall *cemIxIsDone)	(__in LONG MapIndex, __out PLONG IsDone);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxIsDone", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxIsDone([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.I4)] ref int IsDone);
			
				// 보간 제어 구동 이송의 보간 완료 시점까지 대기.
				// [MapIndex: 맵 번호, IsBlocking: 완료될때까지 메세지 블록 여부]
				//EXTERN LONG (__stdcall *cemIxWaitDone)	(__in LONG MapIndex, __in LONG IsBlocking);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemIxWaitDone", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemIxWaitDone([MarshalAs(UnmanagedType.I4)]int MapIndex, [MarshalAs(UnmanagedType.I4)]int IsBlocking);
			
				//****************************************************************************
				//*************** START OF MASTER/SLAVE CONTROL FUNCTION DECLARATIONS ********
				//****************************************************************************
			
				// 대상 모션 축에 대해서, Master/Slave 동기 구동의 Slave 축으로 등록.
				// [Axis: Slave 축으로 지정할 통합 축 번호, MaxSpeed: Slave축 구동 최대 속도, IsInverse: Slave축 과 Master축의 구동방향을 반대로 할것인지 여부]
				//EXTERN LONG (__stdcall *cemMsRegisterSlave)	(__in LONG Axis, __in DOUBLE MaxSpeed, __in LONG IsInverse);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemMsRegisterSlave", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemMsRegisterSlave([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.R8)]double MaxSpeed, [MarshalAs(UnmanagedType.I4)]int IsInverse);
			
				// 대상 모션 축에 대해서, Master/Slave 동기 구동의 Slave 축 해제.
				// [Axis: Slave 축에서 해제할 통합 축 번호]
				//EXTERN LONG (__stdcall *cemMsUnregisterSlave)	(__in LONG Axis);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemMsUnregisterSlave", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemMsUnregisterSlave([MarshalAs(UnmanagedType.I4)]int Axis);
			
				// 대상 모션 축에 대해서, Master/Slave 동기 구동의 Slave 축 등록 여부 환인.
				// [SlaveAxis: Slave 상태를 확인할 대상 통합 축 번호, SlaveState: Slave 축 상태 반환]
				//EXTERN LONG (__stdcall *cemMsCheckSlaveState)	(__in LONG SlaveAxis, __out PLONG SlaveState);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemMsCheckSlaveState", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemMsCheckSlaveState([MarshalAs(UnmanagedType.I4)]int SlaveAxis, [MarshalAs(UnmanagedType.I4)] ref int SlaveState);
			
				// 대상 모션 축에 대해서, Master/Slave 동기 구동의 Master 축의 통합 축 번호를 확인.
				// [SlaveAxis: Slave 통합 축 번호, MasterAxis: Master 통합 축 번호]
				//EXTERN LONG (__stdcall *cemMsMasterAxis_Get)	(__in LONG SlaveAxis, __out PLONG MasterAxis);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemMsMasterAxis_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemMsMasterAxis_Get([MarshalAs(UnmanagedType.I4)]int SlaveAxis, [MarshalAs(UnmanagedType.I4)] ref int MasterAxis);
			
				//****************************************************************************
				//*************** START OF Manual Pulsar FUNCTION SECTION ********************
				//****************************************************************************
			
				// Manual Pulsar 입력 신호에 대한 환경설정.
				// [Channel: 통합 축 번호, InputMode: Pulsar 입력 신호의 입력 모드, IsInverse: Pulsar 입력 신호가 나타내는 방향과 모션의 방향 일치 여부]
				//EXTERN LONG (__stdcall *cemPlsrInMode_Set)	(__in LONG Channel, __in  LONG InputMode,   __in  LONG IsInverse);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemPlsrInMode_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemPlsrInMode_Set([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)]int InputMode, [MarshalAs(UnmanagedType.I4)]int IsInverse);
			
				// Manual Pulsar 입력 신호에 대한 환경설정 반환.
				// [Channel: 통합 축 번호, InputMode: Pulsar 입력 신호의 입력 모드 반환, Pulsar 입력 신호가 나타내는 방향과 모션의 방향 일치 여부 반환]
				//EXTERN LONG (__stdcall *cemPlsrInMode_Get)	(__in LONG Channel, __out PLONG InputMode, __out  PLONG IsInverse);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemPlsrInMode_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemPlsrInMode_Get([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)] ref int InputMode, [MarshalAs(UnmanagedType.I4)] ref int IsInverse);
			
				// Manual Pulsar의 PA/PB 입력펄스 대비, 출력펄스 수에 대한 비율 설정.
				// [Channel: 통합 축 번호, GainFactor: PMG 회로에 설정되는 사용자 정수(1~32), DivFactor: PDIV 회로에 설정되는 사용자 정수(1~2048)]
				//EXTERN LONG (__stdcall *cemPlsrGain_Set)	(__in LONG Channel, __in  LONG GainFactor,  __in  LONG DivFactor);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemPlsrGain_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemPlsrGain_Set([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)]int GainFactor, [MarshalAs(UnmanagedType.I4)]int DivFactor);
			
				// Manual Pulsar의 PA/PB 입력펄스 대비, 출력펄스 수에 대한 비율 설정 반환.
				// [Channel: 통합 축 번호, GainFactor: PMG 회로에 설정되는 사용자 정수 반환, DivFactor: PDIV 회로에 설정되는 사용자 정수 반환]
				//EXTERN LONG (__stdcall *cemPlsrGain_Get)	(__in LONG Channel, __out  PLONG GainFactor, __out PLONG DivFactor);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemPlsrGain_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemPlsrGain_Get([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)] ref int GainFactor, [MarshalAs(UnmanagedType.I4)] ref int DivFactor);
			
				// Manual Pulsar 입력 신호에 의한 원점복귀 이송. 이 함수는 모션을 시작시킨 후에 바로 반환됩니다.
				// [Channel: 통합 축 번호, HomeType: Pulsar Input에 의한 원점 복귀 수행 모드]
				//EXTERN LONG (__stdcall *cemPlsrHomeMoveStart)	(__in LONG Channel, __in  LONG  HomeType);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemPlsrHomeMoveStart", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemPlsrHomeMoveStart([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)]int HomeType);
			
				// Manual Pulsar 입력 신호에 의한 상대좌표 이송. 이 함수는 모션이 완료되기 전까지 반환되지 않습니다.
				// [Channel: 통합 축 번호, Distance: 이동할 거리(상대좌표), IsBlocking: 완료될때까지 메세지 블록 여부]
				//EXTERN LONG (__stdcall *cemPlsrMove)	(__in LONG Channel, __in DOUBLE Distance, __in LONG IsBlocking);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemPlsrMove", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemPlsrMove([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.R8)]double Distance, [MarshalAs(UnmanagedType.I4)]int IsBlocking);
			
				// Manual Pulsar 입력 신호에 의한 상대좌표 이송. 이 함수는 모션을 시작시킨 후에 바로 반환됩니다.
				// [Channel: 통합 축 번호, Distance: 이동할 거리(상대좌표)]
				//EXTERN LONG (__stdcall *cemPlsrMoveStart)	(__in LONG Channel, __in DOUBLE Distance);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemPlsrMoveStart", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemPlsrMoveStart([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.R8)]double Distance);
			
				// Manual Pulsar 입력 신호에 의한 절대좌표 이송. 이 함수는 모션이 완료되기 전까지 반환되지 않습니다.
				// [Channel: 통합 축 번호, Position: 이동할 위치(절대좌표), IsBlocking: 완료될때까지 메세지 블록 여부]
				//EXTERN LONG (__stdcall *cemPlsrMoveTo)         (__in LONG Channel, __in DOUBLE Position, __in LONG IsBlocking);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemPlsrMoveTo", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemPlsrMoveTo([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.R8)]double Position, [MarshalAs(UnmanagedType.I4)]int IsBlocking);
			
				// Manual Pulsar 입력 신호에 의한 절대좌표 이송. 이 함수는 모션을 시작시킨 후에 바로 반환됩니다.
				// [Channel: 통합 축 번호, Position: 이동할 위치(절대좌표)]
				//EXTERN LONG (__stdcall *cemPlsrMoveToStart)    (__in LONG Channel, __in DOUBLE Position);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemPlsrMoveToStart", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemPlsrMoveToStart([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.R8)]double Position);
			
				// Manual Pulsar 입력 신호에 의한 연속속도 이송. 이 함수는 모션을 시작시킨 후에 바로 반환됩니다.
				// [Channel: 통합 축 번호]
				//EXTERN LONG (__stdcall *cemPlsrVMoveStart)	(__in LONG Channel);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemPlsrVMoveStart", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemPlsrVMoveStart([MarshalAs(UnmanagedType.I4)]int Channel);
			
				//****************************************************************************
				//*************** START OF OVERRIDE FUNCTION DECLARATIONS ********************
				//****************************************************************************
			
				// 단축 이송 작업 중에 속도를 변경.
				// [Axis: 통합 축 번호]
				//EXTERN LONG (__stdcall *cemOverrideSpeedSet)	(__in LONG Axis);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemOverrideSpeedSet", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemOverrideSpeedSet([MarshalAs(UnmanagedType.I4)]int Axis);
			
				// 다축 이송 작업 중에 대상 축들에 대하여 동시에 속도를 변경.
				// [NumAxes : 동시에 작업을 수행할 대상 축의 수, AxisList : 동시에 작업을 수행할 대상 축의 배열 주소]
				// EXTERN LONG (__stdcall *cemOverrideSpeedSetAll)	(__in LONG NumAxes, __in LONG AxisList);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemOverrideSpeedSetAll", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemOverrideSpeedSetAll([MarshalAs(UnmanagedType.I4)]int NumAxes, [MarshalAs(UnmanagedType.I4)]int AxisList);
			
				// 단축상대좌표 이송 중에, 상대좌표상의 목표 논리 거리값을 수정.
				// [Axis: 통합 축 번호, NewDistance: 새로운 목표 거리(상대좌표), IsHardApply: Override 불가 시점에서, Override 설정을 강제로 적용 여부, AppliedState: Override 적용 성공 여부 반환]
				//EXTERN LONG (__stdcall *cemOverrideMove)	(__in LONG Axis, __in DOUBLE NewDistance, __in LONG IsHardApply, __out PLONG AppliedState);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemOverrideMove", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemOverrideMove([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.R8)]double NewDistance, [MarshalAs(UnmanagedType.I4)]int IsHardApply, [MarshalAs(UnmanagedType.I4)] ref int AppliedState);
			
				// 단축절대좌표 이송 중에, 절대좌표상의 목표 놀리 거리값을 수정.
				// [Axis: 통합 축 번호, NewPosition: 새로운 목표 거리(절대좌표), IsHardApply: Override 불가 시점에서, Override 설정을 강제로 적용 여부, AppliedState: Override 적용 성공 여부 반환]
				//EXTERN LONG (__stdcall *cemOverrideMoveTo)	(__in LONG Axis, __in DOUBLE NewPosition, __in LONG IsHardApply, __out PLONG AppliedState);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemOverrideMoveTo", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemOverrideMoveTo([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.R8)]double NewPosition, [MarshalAs(UnmanagedType.I4)]int IsHardApply, [MarshalAs(UnmanagedType.I4)] ref int AppliedState);
			
				//****************************************************************************
				//*************** START OF MONITORING FUNCTION DECLARATIONS ******************
				//****************************************************************************
			
				// 대상 축의 지정한 하드웨어 카운터 값(펄스수) 설정. Target 값은 ceSDKDef.h 파일에 정의된 enum _TCnmCntr 값 리스트를 참고.
				// [Axis: 통합 축 번호, Target: 설정할 카운터 번호, Count: 대상 카운터 값(PPS)]
				//EXTERN LONG (__stdcall *cemStCount_Set)	(__in LONG Axis, __in LONG Target, __in LONG Count);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemStCount_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemStCount_Set([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int Target, [MarshalAs(UnmanagedType.I4)]int Count);
			
				// 대상 축의 지정한 하드웨어 카운터 값(펄스수) 반환. Target 값은 ceSDKDef.h 파일에 정의된 enum _TCnmCntr 값 리스트를 참고.
				// [Axis: 통합 축 번호, Source: 값을 읽을 카운터 번호, Count: 대상 카운터 값(PPS) 반환]
				//EXTERN LONG (__stdcall *cemStCount_Get)	(__in LONG Axis, __in LONG Source, __out PLONG Count);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemStCount_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemStCount_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int Source, [MarshalAs(UnmanagedType.I4)] ref int Count);
			
				// 대상 축의 지정한 논리적 카운터 값(논리적 거리) 설정. Target 값은 ceSDKDef.h 파일에 정의된 enum _TCnmCntr 값 리스트를 참고.
				// [Axis: 통합 축 번호, Target: 설정할 카운터 번호, Position: 대상 카운터 값(논리적 거리)]
				//EXTERN LONG (__stdcall *cemStPosition_Set)	(__in LONG Axis, __in LONG Target, __in DOUBLE Position);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemStPosition_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemStPosition_Set([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int Target, [MarshalAs(UnmanagedType.R8)]double Posotion);
			
				// 대상 축의 지정한 논리적 카운터 값(논리적 거리) 반환. Target 값은 ceSDKDef.h 파일에 정의된 enum _TCnmCntr 값 리스트를 참고.
				// [Axis: 통합 축 번호, Source: 값을 읽을 카운터 번호, Position: 대상 카운터 값(논리적 거리) 반환]
				//EXTERN LONG (__stdcall *cemStPosition_Get)	(__in LONG Axis, __in LONG Source, __out PDOUBLE Position);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemStPosition_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemStPosition_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int Source, [MarshalAs(UnmanagedType.R8)] ref double Position);
			
				// 대상 축의 논리적 속도 반환. Target 값은 ceSDKDef.h 파일에 정의된 enum _TCnmCntr 값 리스트를 참고.
				// [Axis: 통합 축 번호, Source: 속도 반환대상이 되는 카운터 번호, Speed: 지정한 Source의 속도(논리적 속도) 반환]
				//EXTERN LONG (__stdcall *cemStSpeed_Get)		(__in LONG Axis, __in LONG Source, __out PDOUBLE Speed);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemStSpeed_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemStSpeed_Get([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int Source, [MarshalAs(UnmanagedType.R8)] ref double Speed);
			
				// 대상 축의 모션 동작 상태 반환.
				// [Axis: 통합 축 번호, MotStates: 모션 상태 반환]
				//EXTERN LONG (__stdcall *cemStReadMotionState)	(__in LONG Axis, __out PLONG MotStates);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemStReadMotionState", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemStReadMotionState([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int MotStates);
			
				// 대상 축의 모션 관련 I/O 상태 반환.
				// [Axis: 통합 축 번호, MiotStates: Machine I/O 상태 반환]
				//EXTERN LONG (__stdcall *cemStReadMioStatuses)	(__in LONG Axis, __out PLONG MioStates);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemStReadMioStatuses", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemStReadMioStatuses([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int MioStates);
			
				// 모션 동작상태와 관련된 문자열 반환. cemStReadMotionState() 함수를 통해 얻어온 모션 상태.
				// [MstCode: 모션 동작 상태 값, Buffer: 모션 동작상태를 담은 문자열 버퍼의 주소 반환, Bufferlen: 문자열 버퍼의 길이]
				//EXTERN LONG (__stdcall *cemStGetMstString)	(__in LONG MstCode, __out PCHAR Buffer, __in LONG BufferLen);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemStGetMstString", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
                public static extern unsafe int cmmStGetMstString([MarshalAs(UnmanagedType.I4)] int MstCode, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I1)] byte[] Buffer, [MarshalAs(UnmanagedType.I4)] int BufferLen);
			
				// 수신된 I/O 메시지 개수 반환.
				// [IOMessageCount: I/O 메시지 수]
				//EXTERN LONG (__stdcall *cemStReadIOMessageCount)	(__out PDWORD IOMessageCount);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemStReadIOMessageCount", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemStReadIOMessageCount([MarshalAs(UnmanagedType.I4)] ref int IOMessageCount);
			
				//****************************************************************************
				//*************** START OF LTC FUNCTION DECLARATIONS *************************
				//****************************************************************************
			
				// 해당축의 LTC 카운터 활성화 여부 확인 [Axis: 축(채널)번호, IsLatched: 카운터 활성화 여부 반환]
				//EXTERN LONG (__stdcall *cemLtcIsLatched)	(__in LONG Axis, __out PLONG IsLatched);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemLtcIsLatched", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemLtcIsLatched([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)] ref int IsLatched);
			
				// 해당축의 LTC 카운터 값을 확인 [Axis: 축(채널)번호, Counter: 대상 카운터, LatchedPos: 지정한 축의 래치된 카운트 값 반환]
				//EXTERN LONG (__stdcall *cemLtcReadLatch)	(__in LONG Axis, __in LONG Counter, __out PDOUBLE LatchedPos);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemLtcReadLatch", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemLtcReadLatch([MarshalAs(UnmanagedType.I4)]int Axis, [MarshalAs(UnmanagedType.I4)]int Counter, [MarshalAs(UnmanagedType.I4)] ref int LatchedPos);
			
				//****************************************************************************
				//*************** START OF ADVANCED FUNCTION DECLARATIONS ********************
				//****************************************************************************
			
				// 해당 축에 ERC 신호를 출력합니다.
				//EXTERN LONG (__stdcall *cemAdvErcOut)				      (__in LONG Axis);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemAdvErcOut", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemAdvErcOut([MarshalAs(UnmanagedType.I4)]int Axis);
			
				// 해당 축에 ERC 신호를 출력을 리셋합니다.
				//EXTERN LONG (__stdcall *cemAdvErcReset)				  (__in LONG Axis);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemAdvErcReset", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemAdvErcReset([MarshalAs(UnmanagedType.I4)]int Axis);
			
				// Undocument Function 입니다. 이 함수는 기술 지원이나 고객 지원용으로 사용됩니다.
				//EXTERN LONG (__stdcall *cemAdvManualPacket)			  (__in LONG NodeID, __in LONG CommandNo, __in PDOUBLE SendBuffer, __in LONG NumSendData, __out PDOUBLE RecvBuffer, __out LONG NumRecvData, __in LONG SendFlag, __in LONG RecvFlag);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemAdvManualPacket", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemAdvManualPacket([MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)]int CommandNo, [MarshalAs(UnmanagedType.I4)] ref int SendBuffer, 
					[MarshalAs(UnmanagedType.I4)]int NumSendData, [MarshalAs(UnmanagedType.I4)] ref int RecvBuffer, [MarshalAs(UnmanagedType.I4)]int NumRecvData, [MarshalAs(UnmanagedType.I4)]int SendFlag, [MarshalAs(UnmanagedType.I4)]int RecvFlag);
			
				//****************************************************************************
				//*************** START OF SYSTEM DIO CONFIGURATION FUNCTION DECLARATIONS ****
				//****************************************************************************
				// 시스템 디지털 입출력이 지원되는 제품에서 단일 채널에 대해서 입력을 확인합니다.
				//EXTERN LONG (__stdcall *cemDiOne_Get)       (__in LONG Channel,     __out PLONG State);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemDiOne_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemDiOne_Get([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)] ref int State);
			
				// 시스템 디지털 입출력이 지원되는 제품에서 다중 채널에 대해서 입력을 확인합니다.
				//EXTERN LONG (__stdcall *cemDiMulti_Get)		(__in LONG IniChannel,	__in LONG NumChannels, __out PLONG InputState);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemDiMulti_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemDiMulti_Get([MarshalAs(UnmanagedType.I4)]int IniChannel, [MarshalAs(UnmanagedType.I4)]int NumChannels, [MarshalAs(UnmanagedType.I4)] ref int InputState);
			
				// 시스템 디지털 입출력이 지원되는 제품에서 단일 채널에 대해서 출력을 발생합니다.
				//EXTERN LONG (__stdcall *cemDoOne_Put)		(__in LONG Channel,		__in LONG OutState);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemDoOne_Put", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemDoOne_Put([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)]int OutState);
			
				// 시스템 디지털 입출력이 지원되는 제품에서 단일 채널에 대해서 출력을 확인합니다.
				//EXTERN LONG (__stdcall *cemDoOne_Get)		(__in LONG Channel,		__in PLONG OutState);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemDoOne_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemDoOne_Get([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)] ref int OutState);
			
				// 시스템 디지털 입출력이 지원되는 제품에서 다중 채널에 대해서 출력을 발생합니다.
				//EXTERN LONG (__stdcall *cemDoMulti_Put)		(__in LONG IniChannel,	__in LONG NumChannels, __in LONG OutStates);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemDoMulti_Put", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemDoMulti_Put([MarshalAs(UnmanagedType.I4)]int IniChannel, [MarshalAs(UnmanagedType.I4)]int NumChannels, [MarshalAs(UnmanagedType.I4)]int OutStates);
			
				// 시스템 디지털 입출력이 지원되는 제품에서 다중 채널에 대해서 출력을 확인합니다.
				//EXTERN LONG (__stdcall *cemDoMulti_Get)		(__in LONG IniChannel,	__in LONG NumChannels, __out PLONG OutStates);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cemDoMulti_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cemDoMulti_Get([MarshalAs(UnmanagedType.I4)]int IniChannel, [MarshalAs(UnmanagedType.I4)]int NumChannels, [MarshalAs(UnmanagedType.I4)] ref int OutStates);
			
				//****************************************************************************
				//*************** START OF DIO CONFIGURATION FUNCTION DECLARATIONS ***********
				//****************************************************************************
			
				// 대상 디지털 입출력 채널의 입출력 모드(Input/Output) 설정.
				// [Channel: 통합 채널 번호, InOutMode: 입출력 모드]
				//EXTERN LONG (__stdcall *cedioMode_Set)	(__in LONG Channel, __in LONG InOutMode);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cedioMode_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cedioMode_Set([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)]int InOutMode);
			
				// 대상 디지털 입출력 채널의 입출력 모드(Input/Output) 설정 상태 반환.
				// [Channel: 통합 채널 번호, InOutMode: 입출력 모드 반환]
				//EXTERN LONG (__stdcall *cedioMode_Get)	(__in LONG Channel, __out PLONG InOutMode);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cedioMode_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cedioMode_Get([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)] ref int InOutMode);
			
				// 대상 디지털 입출력 채널 범위의 다중 입출력 채널에 대해 동시에 입출력 모드(Input/Output) 설정.
				// [IniChan: 시작 통합 채널 번호, NumChan: 대상 채널 개수, InOutModeMask: 입출력 모드 마스크값]
				//EXTERN LONG (__stdcall *cedioModeMulti_Set) (__in LONG IniChan, __in LONG NumChan, __in LONG InOutModeMask);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cedioModeMulti_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cedioModeMulti_Set([MarshalAs(UnmanagedType.I4)]int IniChan, [MarshalAs(UnmanagedType.I4)]int NumChan, [MarshalAs(UnmanagedType.I4)]int InOutModeMask);
			
				// 대상 디지털 입출력 채널 범위의 다중 입출력 채널에 대해 동시에 입출력 모드(Input/Output) 설정 상태 반환.
				// [IniCahn: 시작 통합 채널 번호, NumChan: 대상 채널 개수, InOutModeMask: 입출력 모드 마스크값 반환]
				//EXTERN LONG (__stdcall *cedioModeMulti_Get) (__in LONG IniChan, __in LONG NumChan, __out PLONG InOutModeMask);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cedioModeMulti_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cedioModeMulti_Get([MarshalAs(UnmanagedType.I4)]int IniChan, [MarshalAs(UnmanagedType.I4)]int NumChan, [MarshalAs(UnmanagedType.I4)] ref int InOutModeMask);
			
				// 대상 디지털 입출력 채널의 입출력 논리(Logic) 설정.
				// [Channel: 통합 채널 번호, InputLogic: 입출력 로직]
				//EXTERN LONG (__stdcall *cedioLogicOne_Set)	(__in LONG Channel, __in  LONG Logic);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cedioLogicOne_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cedioLogicOne_Set([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)]int Logic);
			
				// 대상 디지털 입출력 채널의 입출력 논리(Logic) 설정 상태 반환.
				// [Channel: 통합 채널 번호, InputLogic: 입출력 로직 반환]
				//EXTERN LONG (__stdcall *cedioLogicOne_Get)	(__in LONG Channel, __out PLONG Logic);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cedioLogicOne_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cedioLogicOne_Get([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)] ref int Logic);
			
				// 대상 디지털 입출력 채널 범위의 다중 입출력 채널에 대해 동시에 입출력 논리(Logic) 설정.
				// [IniChan: 시작 통합 채널 번호, NumChan: 대상 채널 개수, LogicMask: 입출력 논리(Logic) 마스크]
				//EXTERN LONG (__stdcall *cedioLogicMulti_Set)	(__in LONG IniChan, __in LONG NumChan, __in  LONG  LogicMask);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cedioLogicMulti_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cedioLogicMulti_Set([MarshalAs(UnmanagedType.I4)]int IniChan, [MarshalAs(UnmanagedType.I4)]int NumChan, [MarshalAs(UnmanagedType.I4)]int LogicMask);
			
				// 대상 디지털 입출력 채널 범위의 다중 입출력 채널에 대해 동시에 입출력 논리(Logic) 설정 상태 반환
				// 디지털 입출력 채널의 입출력논리 설정상태 반환 [IniChan: 시작 통합 채널 번호, NumChan: 채널 개수, LogicMask: 로직 마스크]
				//EXTERN LONG (__stdcall *cedioLogicMulti_Get)	(__in LONG IniChan, __in LONG NumChan, __out PLONG LogicMask);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cedioLogicMulti_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cedioLogicMulti_Get([MarshalAs(UnmanagedType.I4)]int IniChan, [MarshalAs(UnmanagedType.I4)]int NumChan, [MarshalAs(UnmanagedType.I4)] ref int LogicMask);
			
				// 대상 디지털 입출력 채널의 입력 또는 출력 상태 반환.
				// [Channel: 통합 채널 번호, OutState: 채널 상태 반환]
				//EXTERN LONG (__stdcall *cedioOne_Get)	(__in LONG Channel, __out PLONG State);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cedioOne_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cedioOne_Get([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)] ref int State);
			
				// 대상 디지털 입출력 채널의 입력 또는 출력 상태 설정.
				// [Channel: 통합 채널 번호, OutState: 채널 상태]
				//EXTERN LONG (__stdcall *cedioOne_Put)	(__in LONG Channel, __in  LONG  State);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cedioOne_Put", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cedioOne_Put([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)]int State);
			
				// 대상 디지털 입출력 채널 범위의 다중 입출력 채널에 대해 동시에 입력 또는 출력 상태를 반환.
				// [IniChan: 시작 통합 채널 번호, NumChan: 대상 채널 개수, States: 채널 상태 반환]
				//EXTERN LONG (__stdcall *cedioMulti_Get)	(__in LONG IniChan, __in LONG NumChan, __out PLONG States);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cedioMulti_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cedioMulti_Get([MarshalAs(UnmanagedType.I4)]int IniChan, [MarshalAs(UnmanagedType.I4)]int NumChan, [MarshalAs(UnmanagedType.I4)] ref int States);
			
				// 대상 디지털 입출력 채널 범위의 다중 입출력 채널에 대해 동시에 입력 또는 출력 상태를 설정.
				// [ IniChan: 시작 통합 채널 번호, NumChan: 대상 채널 개수, States: 채널 상태]
				//EXTERN LONG (__stdcall *cedioMulti_Put)	(__in LONG IniChan, __in LONG NumChan, __in  LONG  States);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cedioMulti_Put", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cedioMulti_Put([MarshalAs(UnmanagedType.I4)]int IniChan, [MarshalAs(UnmanagedType.I4)]int NumChan, [MarshalAs(UnmanagedType.I4)]int States);
			
				// 대상 디지털 입출력 채널의 노이즈 필터 기능을 활성화 하여 디지털 입력 또는 출력 상태를 반환.
				// [Channel: 통합 채널 번호, CutoffTime_us: 디지털 입력 신호 유지 시간(us), State: 채널 상태 반환]
				//EXTERN LONG (__stdcall *cedioOneF_Get)	(__in LONG Channel,    __in LONG CutoffTime_us, __out PLONG State);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cedioOneF_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cedioOneF_Get([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)]int CutoffTime_us, [MarshalAs(UnmanagedType.I4)] ref int State);
			
				// 대상 디지털 입출력 채널 범위의
				// 다중 디지털 입출력 채널의 노이즈 필터 기능
				// [IniChan: 시작 통합 채널 번호, NumChan: 채널 수, CutoffTime_us: 디지털 입력 신호 유지 시간(us), InputStates: 해당 채널의 디지털 입력 상태]
				//EXTERN LONG (__stdcall *cedioMultiF_Get)	(__in LONG IniChan, __in LONG NumChan, __in LONG CutoffTime_us, __out PLONG States);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cedioMultiF_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cedioMultiF_Get([MarshalAs(UnmanagedType.I4)]int IniChan, [MarshalAs(UnmanagedType.I4)]int NumChan, [MarshalAs(UnmanagedType.I4)]int CutoffTime_us, [MarshalAs(UnmanagedType.I4)] ref int States);
			
				// 대상 디지털 입출력 채널의 단일 디지털 채널을 통해 단일 펄스 출력을 발생
				// [Channel: 통합 채널 번호, IsOnPulse: 설정된 디지털 출력 논리에 따라 초기 펄스 출력의 형태를 결정, Duration: 펄스 출력 시간 설정,
				// IsWaitPulseEnd: 펄스 출력 동작시에 함수를 바로 반환할 것인지, 아니면 펄스 출력 시간동안 함수 반환을 대기할 것인지 여부]
				//EXTERN LONG (__stdcall *cedioPulseOne)	(__in LONG Channel, __in LONG IsOnPulse, __in LONG Duration,  __in LONG IsWaitPulseEnd);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cedioPulseOne", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cedioPulseOne([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)]int IsOnPulse, [MarshalAs(UnmanagedType.I4)]int Duration, [MarshalAs(UnmanagedType.I4)]int IsWaitPulseEnd);
			
				// 디지털 출력 입채널 범위의 지정한 다중 디지털 채널을 통해 단일 펄스 출력을 발생
				// [IniChan: 시작 채널, NumChan: 채널 수, OutStates: 디지털 출력 상태, Duration: 펄스 출력 시간 설정,
				// IsWaitPulseEnd: 펄스 출력 동작시에 함수를 바로 반환할 것인지, 아니면 펄스 출력 시간동안 함수 반환을 대기할 것인지 여부]
				//EXTERN LONG (__stdcall *cedioPulseMulti)	(__in LONG IniChan, __in LONG NumChan,   __in LONG OutStates, __in LONG Duration, __in LONG IsWaitPulseEnd);
				[DllImport("ceSDKDLL.dll", EntryPoint = "cedioPulseMulti", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int cedioPulseMulti([MarshalAs(UnmanagedType.I4)]int IniChan, [MarshalAs(UnmanagedType.I4)]int NumChan, [MarshalAs(UnmanagedType.I4)]int OutStates, [MarshalAs(UnmanagedType.I4)]int Duration, [MarshalAs(UnmanagedType.I4)]int IsWaitPulseEnd);
			
				//****************************************************************************
				//*************** START OF ANALOG INPUT FUNCTION DECLARATIONS ****************
				//****************************************************************************
			
				// 아날로그 입력에 대한 전압 범위를 지정된 모드를 통해 설정합니다.
				// [Channel: 아날로그 입력 채널 번호, RangeMode: 전압 범위 설정 모드]
				//RangeMode (0~3)
				//A	B	입력범위
				//0	0	+10V ~ -10V
				//0	1	+5V ~ -5V
				//1	0	+2.5V ~ -2.5V
				//1	1	0V ~ +10V (0~20mA)
				//EXTERN LONG (__stdcall *ceaiVoltRangeMode_Set)		(__in LONG Channel, __in  LONG  RangeMode);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceaiVoltRangeMode_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceaiVoltRangeMode_Set([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)]int RangeMode);
			
				// 아날로그 입력에 대해 설정된 전압 범위에 해당하는 모드를 반환합니다.
				// [Channel: 아날로그 입력 채널 번호, RangeMode: 전압 범위 설정 모드]
				//EXTERN LONG (__stdcall *ceaiVoltRangeMode_Get)		(__in LONG Channel, __out PLONG RangeMode);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceaiVoltRangeMode_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceaiVoltRangeMode_Get([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)] ref int RangeMode);
			
				// 아날로그 입력에 대해 설정된 입력 Range 를 Digit 값으로 반환합니다 => 이 함수는 추후에 사용하지 않을 수 있습니다.
				// [Channel: 아날로그 입력 채널 번호, DigitMin: 최소 입력 Digit 값, DigitMax: 최대 입력 Digit 값]
				//EXTERN LONG (__stdcall *ceaiRangeDigit_Get)         (__in LONG Channel, __out PLONG DigitMin, __out PLONG DigitMax);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceaiRangeDigit_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceaiRangeDigit_Get([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)] ref int DigitMin, [MarshalAs(UnmanagedType.I4)] ref int DigitMax);
			
				// 해당 아날로그 입력 채널의 입력 Digit 값을 반환합니다.
				// [Channel: 아날로그 입력 채널 번호, Digit: 입력된 Digit 값]
				//EXTERN LONG (__stdcall *ceaiDigit_Get)              (__in LONG Channel, __out PLONG Digit);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceaiDigit_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceaiDigit_Get([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)] ref int Digit);
			
				// 해당 아날로그 입력 채널의 입력 전압 값을 반환합니다.
				// [Channel: 아날로그 입력 채널 번호, fVolt: 입력된 전압 값]
				//EXTERN LONG (__stdcall *ceaiVolt_Get)               (__in LONG Channel, __out PDOUBLE fVolt);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceaiVolt_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceaiVolt_Get([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)] ref int fVolt);
			
				// 해당 아날로그 입력 채널의 입력 전류 값을 반환합니다.
				// [Channel: 아날로그 입력 채널 번호, fCurrent: 입력된 전류 값]
				//EXTERN LONG (__stdcall *ceaiCurrent_Get)            (__in LONG Channel, __out PDOUBLE fCurrent);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceaiCurrent_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceaiCurrent_Get([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)] ref int fCurrent);
			
				//****************************************************************************
				//*************** START OF ANALOG OUTPUT FUNCTION DECLARATIONS ***************
				//****************************************************************************
			
				// 해당 아날로그 출력 채널을 통해 Digit 값을 출력합니다.
				// [Channel: 아날로그 출력 채널 번호, Digit: 출력 Digit 값]
				//EXTERN LONG (__stdcall *ceaoDigit_Out)              (__in LONG Channel, __in LONG Digit);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceaoDigit_Out", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceaoDigit_Out([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.I4)]int Digit);
			
				// 해당 아날로그 출력 채널을 통해 전압 값을 출력합니다.
				// [Channel: 아날로그 출력 채널 번호, fVolt: 출력 전압 값]
				//EXTERN LONG (__stdcall *ceaoVolt_Out)               (__in LONG Channel, __in DOUBLE fVolt);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceaoVolt_Out", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceaoVolt_Out([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.R8)]double fVolt);
			
				// 해당 아날로그 출력 채널을 통해 전류 값을 출력합니다.
				// [Channel: 아날로그 출력 채널 번호, fCurrent: 출력 전류 값]
				//EXTERN LONG (__stdcall *ceaoCurrent_Out)            (__in LONG Channel, __in DOUBLE fCurrent);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceaoCurrent_Out", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceaoCurrent_Out([MarshalAs(UnmanagedType.I4)]int Channel, [MarshalAs(UnmanagedType.R8)]double fCurrent);
			
				//****************************************************************************
				//*************** START OF UTILITY FUNCTION DECLARATIONS *********************
				//****************************************************************************
			
				// 최대 32바이트의 임의의 문자열을 노드의 영구 저장 장치에 기록합니다.
				// [NodeID : 대상 노드 번호]
				// [NumByte: 기록할 데이터 길이(바이트 단위)]
				// [szText : 기록할 문자열]
				//EXTERN LONG (__stdcall *ceutlUserData_Set)		(__in LONG NodeID, __in  LONG NumByte,  __in  PCHAR szText);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceutlUserData_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceutlUserData_Set([MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)]int NumByte, [MarshalAs(UnmanagedType.I4)] ref int szText);
			
				// 최대 32바이트의 임의의 문자열을 노드의 영구 저장 장치에서 읽어옵니다.
				// [NodeID : 대상 노드 번호]
				// [NumByte: 읽어온 데이터 길이(바이트 단위)]
				// [szText : 읽어온 문자열 반환]
				//EXTERN LONG (__stdcall *ceutlUserData_Get)		(__in LONG NodeID, __out PLONG NumByte, __out PCHAR szText);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceutlUserData_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceutlUserData_Get([MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)] ref int NumByte, [MarshalAs(UnmanagedType.I4)] ref int szText);
			
				// 대상 원격 노드에 사용자 정의 버전을 기록합니다.
				// [NodeID : 대상 노드 번호]
				// [Version: 기록할 버전]
				//EXTERN LONG (__stdcall *ceutlUserVersion_Set)	(__in LONG NodeID, __in  LONG Version);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceutlUserVersion_Set", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceutlUserVersion_Set([MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)]int Version);
			
				// 대상 원격 노드의 사용자 정의 버전을 읽어옵니다.
				// [NodeID : 대상 노드 번호]
				// [Version: 기록된 버전 반환]
				//EXTERN LONG (__stdcall *ceutlUserVersion_Get)	(__in LONG NodeID, __out PLONG pVersion);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceutlUserVersion_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceutlUserVersion_Get([MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)] ref int pVersion);
			
				// 대상 원격 노드의 펌웨어 버전을 읽어옵니다.
				// [NodeID : 대상 노드 번호]
				// [Version: 기록된 펌웨어 버전 반환]
				//EXTERN LONG (__stdcall *ceutlNodeVersion_Get)	(__in LONG NodeID, __out PLONG pVersion);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceutlNodeVersion_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceutlNodeVersion_Get([MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)] ref int pVersion);
			
				// 본 라이브러리의 버젼을 가져옵니다. 상위 및 하위 4바이트의 주소에 각 2바이트씩 버전을 할당합니다. 버전의 자리수는 총 4자리입니다.
				// 라이브러리 버전을 얻는 방법은 다음과 같습니다.
				// [pVersionMS : 상위 비트 버전 정보 반환]
				// [pVersionLS : 하위 비트 버전 정보 반환]
			
				// printf("Dynamic Link Library Version = [%d].[%d].[%d].[%d]"
				//	_X(pVersionMS >> 16 & 0xFF)
				//	_X(pVersionMS >> 0 & 0xFF)
				//	_X(pVersionLS >> 16 & 0xFF)
				//	_X(pVersionLS >> 0 & 0xFF));
				//EXTERN LONG (__stdcall *ceutlLibVersion_Get)		(__out PLONG pVersionMS, __out PLONG pVersionLS);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceutlLibVersion_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceutlLibVersion_Get([MarshalAs(UnmanagedType.I4)] ref int pVersionMS, [MarshalAs(UnmanagedType.I4)] ref int pVersionLS);
			
				// 단일 윈도우 메세지를 처리합니다.
				//EXTERN LONG (__stdcall *ceutlPumpSingleMessage)     ();
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceutlPumpSingleMessage", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceutlPumpSingleMessage();
			
				// 다중 윈도우 메세지를 처리합니다. 인자로 설정된 nTimeout 은 설정된 시간동안 윈도우 메세지를 처리하게 됩니다.
				// 만약 nTimeout을 CN_INFINITE 로 설정하게되면, 모든 윈도우 메세지를 처리한 후 반환됩니다.
				// nTimeout 의 단위는 ms 입니다
				// [nTimeout : 설정된 시간동안 윈도우 메세지를 처리합니다.]
				//EXTERN LONG (__stdcall *ceutlPumpMultiMessage)      (__in LONG nTimeout);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceutlPumpMultiMessage", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceutlPumpMultiMessage([MarshalAs(UnmanagedType.I4)]int nTimeout);
			
				// 노드 명령을 동기화 하기 위한 동기 카운트를 반환
				// [NodeID : 대상 노드, pSyncCount : 대상 노드의 현재 카운트 반환]
				//EXTERN LONG (__stdcall *ceutlSyncCount_Get)		(__in LONG NodeID, __out PLONG pSyncCount);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceutlSyncCount_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceutlSyncCount_Get([MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)] ref int pSyncCount);
			
				// 노드 명령을 동기화 하기 위한 I/O 메세지 동기 카운트를 반환
				// [NodeID : 대상 노드, pSyncCount : 대상 노드의 현재 카운트 반환]
				//EXTERN LONG (__stdcall *ceutlIOSyncCount_Get)		(__in LONG NodeID, __out PLONG pSyncCount);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceutlIOSyncCount_Get", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceutlIOSyncCount_Get([MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)] ref int pSyncCount);
			
				// 노드와 상위 응용프로그램의 명령 동기화에 대해 처리합니다. 
				// 실제 명령된 명령 계수와 주기적인 메세지의 동기화를 위해 대기합니다.
				// [NodeID : 대상 노드, IsBlocking : 동기를 위해 대기하는 동안 윈도우 메세지 Blocking 여부]
				//EXTERN LONG (__stdcall *ceutlSyncWait)		   (__in LONG NodeID, __in LONG IsBlocking);
				[DllImport("ceSDKDLL.dll", EntryPoint = "ceutlSyncWait", ExactSpelling = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
				public static extern unsafe int ceutlSyncWait([MarshalAs(UnmanagedType.I4)]int NodeID, [MarshalAs(UnmanagedType.I4)]int IsBlocking);
    }
}

