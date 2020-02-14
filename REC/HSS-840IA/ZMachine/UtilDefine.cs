
namespace Machine
{
    //이거 나중에 없애고 그냥 
    //클래스로 만들기.
    //public enum si : uint
    //{
    //    Marking  = 0, 
    //    MAX_RS232 
    //}
    public class Eqp
    {
        public const string sEqpName = "HSS-840IA";
    }

    public enum vi : uint //Vision Inspection Option
    {
        One    = 0 ,
        Col    = 1 ,
        All    = 2 ,
    }
    public class DressyMac
    {
        public const int Setting       = 1 ;
        public const int Configuration = 2 ;
        public const int Trigger       = 3 ;
        public const int Calibration   = 4 ;
        public const int Generate      = 5 ;
        public const int BPM4P         = 6 ;
        public const int CalUpdate     = 7 ;
        public const int Acquisition   = 8 ;
        public const int VIET          = 9 ;
        public const int Write         = 10;
        public const int FolderCopy    = 11; //1.0.1.1 Cal 공정만 사용하는 경우 있어서 이미지 폴더 카피 위해 Write에서 분리.
        public const int FileCheck     = 12;
        public const int Aging         = 13; //1.0.1.2 Aging 버튼 사용해야해서 추가됨
    }

    public class EzMac
    {
        public const int Entering     = 1 ;//CycleEntering   
        public const int Trigger      = 2 ;//CycleTrigger    
        public const int Aging        = 3 ;//CycleAging      
        public const int GetBright    = 4 ;//CycleGetBright  
        public const int GetImage     = 5 ;//CycleGetImage   
        public const int CalGen       = 6 ;//CycleCalGen     
        public const int PreGen       = 7 ;//CyclePreGen     
        public const int DQE1         = 8 ;//CycleDQE1       
        public const int DQE2         = 9 ;//CycleDQE2  
        public const int Calibration1 = 10;
        public const int Calibration2 = 11;
        public const int Skull        = 12;//CycleSkull      
        public const int ResultCopy   = 13;//Result Folder Copy
    }
    public enum fb : uint
    {
        Bwd    = 0 ,
        Fwd    = 1 , 
    }

    //Part ID
    public enum pi : uint
    {
        INDX     , //Loader, Index, Feeder, Door //자재 꺼내고 다시 넣는 부분때문에 로더를 이 파트에 넣음
        XRYD     , //Dressy   자재 , X-Ray,  USB Connect
        XRYE     , //EzSensor 자재 , X-Ray,  USB Connect
        MAX_PART , 
    }


    //aRay Id
    public class ri
    {
        public const int LODR      = 0 ; //로더 트레이 공급존
        public const int INDX      = 1 ; //프론트 인덱스

        public const int MAX_ARAY  = 12;
        
    }

    // Id
    public enum ax : uint
    {
        ETC_00 = 00, //
        ETC_01 = 01, //
        ETC_02 = 02, //
        ETC_03 = 03, //
        ETC_04 = 04, //
        ETC_05 = 05, //
        ETC_06 = 06, //
        ETC_07 = 07, //

    }

    public enum ay : uint
    {
        ETC_00 = 00, //
        ETC_01 = 01, //
        ETC_02 = 02, //
        ETC_03 = 03, //
        ETC_04 = 04, //
        ETC_05 = 05, //
        ETC_06 = 06, //
        ETC_07 = 07, //
    }

    //Chip Status
    public enum cs : int//<sun>화면에 팝업메뉴 손봐야함.
    {
        RetFail   = -1, //함수 리턴 
        None      =  0, //All
        Unknown       , //검사전
        Empty         , //실제 비어있는 자재
        Mask          , //로더 현재 작업중인 트레이 슬롯
        Barcode       , //바코드 리딩해야하는 
        Ready         , //여기부터 Dressy, Work전에 준비 작업.
        Work          , //                 리스트뷰에 조건으로 조사.
        Analyze       , //                 분석.
        Check         , //                 USB 뺐다껴서 플래시메모리에 저장됬는지 확인
        Entering1x1   , //여기부터 EzSensor
        Aging1x1      ,
        MTFNPS1x1     ,
        Calibration1x1,
        Skull1x1      ,
        Entering2x2   ,
        Aging2x2      ,
        MTFNPS2x2     ,
        Calibration2x2,
        Skull2x2      ,
        WorkEnd       , //작업 완료
        MAX_CHIP_STAT

    };

    //mi<파트명3자리>_<축방샹1자리><부가명칭4> Motor Id
    public enum mi : uint
    {                   //                    홈위치_장비전면 기준
        LODR_ZElev = 0 , //Loader                   , 아래
        INDX_XRail     , //Index                    , 오른쪽
        XRAY_XFltr     , //X-Ray X Filter           , 오른쪽
        XRAY_ZXRay     , //X-Ray Z X-Ray            , 위쪽
        XRAY_XCnct     , //USB Connector            , 오른쪽
        MAX_MOTR       
    };

    //ai<파트명3자리>_<부가명칭><FWD시에위치2자리><BWD시에위치2자리>
    //대기가 Bwd 작동이 Fwd
    public enum ci : uint
    {
        INDX_TrayFixClOp   =  0 ,
        INDX_TrayClampClOp =  1 ,
        INDX_TrayClampUpDn =  2 ,
        INDX_TrayClampBwFw =  3 ,
        INDX_TrayFeedFwBw  =  4 ,
        INDX_DoorClOp      =  5 , 
        XRAY_Filter1Dn     =  6 ,
        XRAY_Filter2Dn     =  7 ,
        XRAY_Filter3Dn     =  8 ,
        XRAY_Filter4Dn     =  9 ,
        XRAY_Filter5Dn     = 10 ,
        XRAY_Filter6Dn     = 11 ,
        XRAY_Filter7Dn     = 12 ,
        XRAY_LeftUSBFwBw   = 13 ,
        XRAY_RightUSBFwBw  = 14 ,
        MAX_ACTR
    };

    //X Id
    //그냥 아이오.  <파트 3자리>+_+<세부설명>
    //실린더 아이오 <파트 3자리>+_+<세부설명>+<해당행동2자리 ex) Fw,Bw,Up,Dn>
    //Ft - Front , Rr - Rear 으로 통일 함.
    public enum xi : uint
    {
        ETC_StartSw         =  0 , //
        ETC_StopSw          =  1 , //
        ETC_ResetSw         =  2 , //
        ETC_AirSw           =  3 , //  
        ETC_InitSw          =  4 , //  
        ETC_FtLtEmgSw       =  5 , //  
        ETC_FtRtEmgSw       =  6 , //  
        ETC_RrEmgSw         =  7 , //
        ETC_TrayLiftSw      =  8 , //로더에 대차 높이까지 올리는 스위치  
        ETC_FtOutDoor       =  9 , //  
        ETC_LtOutDoor       = 10 , //  
        ETC_RrOutDoor       = 11 , //  
        ETC_RtOutDoor       = 12 , //
        ETC_FtInDoor        = 13 , //
        ETC_RtInDoor        = 14 , //  
        ETC_RrInDoor        = 15 , //

        ETC_MainServo       = 16 , //  
        ETC_MainAir         = 17 , //  
        INDX_TrayFixCl      = 18 , //  
        INDX_TrayFixOp      = 19 , //  
        INDX_TrayDtct1      = 20 , //  
        INDX_TrayDtct2      = 21 , //  
        INDX_TrayClampCl    = 22 , //  
        INDX_TrayClampOp    = 23 , //  
        INDX_FeedTrayDtct   = 24 , //  
        INDX_Overload       = 25 , // 
        INDX_TrayClampUp    = 26 , // 
        INDX_TrayClampDn    = 27 , // 
        INDX_TrayClampFw    = 28 , // 
        INDX_TrayClampBw    = 29 , //  
        INDX_TrayFeedFw     = 30 , //  
        INDX_TrayFeedBw     = 31 , //

        XRAY_Filter1Dn      = 32 , //  
        XRAY_Filter2Dn      = 33 , //  
        XRAY_Filter3Dn      = 34 , //  
        XRAY_Filter4Dn      = 35 ,
        XRAY_Filter5Dn      = 36 ,
        XRAY_Filter6Dn      = 37 ,
        XRAY_Filter7Dn      = 38 ,
        XRAY_LeftUSBFw      = 39 ,
        XRAY_LeftUSBBw      = 40 ,
        XRAY_RightUSBFw     = 41 ,
        XRAY_RightUSBBw     = 42 ,
        INDX_DoorOp         = 43 ,
        INDX_DoorCl         = 44 ,
        INDX_LdrTrayDtct    = 45 ,
        ETC_046             = 46 ,
        ETC_047             = 47 ,

        ETC_048             = 48 ,
        ETC_049             = 49 ,
        ETC_050             = 50 ,
        ETC_051             = 51 ,
        ETC_052             = 52 ,
        ETC_053             = 53 ,
        ETC_054             = 54 ,
        ETC_055             = 55 ,
        ETC_056             = 56 ,
        ETC_057             = 57 ,
        ETC_058             = 58 ,
        ETC_059             = 59 ,//VSNRB_Ready         = 59 ,
        ETC_060             = 60 ,//VSNRB_Busy          = 60 ,
        ETC_061             = 61 ,//VSNRB_InspOk        = 61 ,
        ETC_062             = 62 ,//VSNRB_Spare1        = 62 ,
        ETC_063             = 63 ,//VSNRB_Spare2        = 63 ,
        MAX_INPUT
    };

    //그냥 아이오31.  <파트 3자리>+_+<세부설명>
    //복동 실린더 아이오 <파트 3자리>+_+<세부설명>+<해당행동 ex) Fw , Bw , Dn 등등 2자리>     자리
    //단동 실린더 아이오 <파트 3자리>+_+<세부설명>+<Fw시에해당행동2자리><Bw시에해당행동2자리> 동
    public enum yi : uint
    {
        ETC_StartLp           =  0, // 
        ETC_StopLp            =  1, // 
        ETC_ResetLp           =  2, // 
        ETC_AirLp             =  3, // 
        ETC_InitLp            =  4, // 
        ETC_LightOnOff        =  5, // 
        ETC_MainAirSol        =  6, // 
        INDX_LdrZElevBkOff    =  7, // 
        XRAY_ZXRayBkOff       =  8, // 
        XRAY_XRayOn           =  9, // 
        ETC_TwRedLp           = 10, // 
        ETC_TwYelLp           = 11, // 
        ETC_TwGrnLp           = 12, // 
        ETC_TwBzz             = 13, // 
        INDX_TrayFixCl        = 14, // 
        INDX_TrayClampClOp    = 15, //

        INDX_TrayClampUp      = 16, // 
        INDX_TrayClampDn      = 17, // 
        INDX_TrayClampFw      = 18, // 
        INDX_TrayClampBw      = 19, // 
        INDX_TrayFeedFw       = 20, // 
        INDX_TrayFeedBw       = 21, // 
        XRAY_Filter1Dn        = 22, // 
        XRAY_Filter2Dn        = 23, // 
        XRAY_Filter3Dn        = 24, // 
        XRAY_Filter4Dn        = 25, // 
        XRAY_Filter5Dn        = 26, // 
        XRAY_Filter6Dn        = 27, // 
        XRAY_Filter7Dn        = 28, // 
        XRAY_LeftUSBFwBw      = 29, // 
        XRAY_RightUSBFwBw     = 30, // 
        ETC_IonizerPower      = 31, // 

        INDX_DoorOp           = 32, //  
        INDX_DoorCl           = 33, //  
        ETC_IonizerSol        = 34, //  
        INDX_TrayFixOp        = 35, // 
        ETC_ColdGunOnOff      = 36, //  
        ETC_037               = 37, //  
        ETC_038               = 38, //  
        ETC_039               = 39, //  
        ETC_040               = 40, //  
        ETC_041               = 41, //  
        ETC_042               = 42, //  
        ETC_043               = 43, //  
        ETC_044               = 44, //  
        ETC_045               = 45, //  
        ETC_046               = 46, //  
        ETC_047               = 47, // 
 
        ETC_048               = 48, //
        ETC_049               = 49, //
        ETC_050               = 50, //
        ETC_051               = 51, //
        ETC_052               = 52, //
        ETC_053               = 53, //
        ETC_054               = 54, //Vision1
        ETC_055               = 55, //
        ETC_056               = 56, //
        ETC_057               = 57, //
        ETC_058               = 58, //ETC_058
        ETC_059               = 59, //VSNB_Ready            = 59, //
        ETC_060               = 60, //VSNB_Busy             = 60, //
        ETC_061               = 61, //VSNB_InspOk           = 61, //
        ETC_062               = 62, //VSNB_Spare1           = 62, //
        ETC_063               = 63, //VSNB_Spare2           = 63, //
        MAX_OUTPUT
    };

    //Position Value id
    public enum pv : uint
    {
        //LODR_ZElev
        LODR_ZElevWait             = 0,
        LODR_ZElevWorkStt             ,
        LODR_ZElevRefill              ,
        MAX_PSTN_MOTR0                ,

        //INDX_XRail
        INDX_XRailWait             = 0,
        INDX_XRailGet                 ,
        INDX_XRailBarcode             ,
        INDX_XRailWorkStt             ,
        MAX_PSTN_MOTR1                ,

        //RGPY_XFltr                   
        XRAY_XFltrWait             = 0,
        XRAY_XFltr1Work               ,
        XRAY_XFltr2Work               ,
        XRAY_XFltr3Work               ,
        XRAY_XFltr4Work               ,
        XRAY_XFltr5Work               ,
        XRAY_XFltr6Work               ,
        XRAY_XFltr7Work               ,
        MAX_PSTN_MOTR2                ,

        //RGPY_ZXRay                   
        XRAY_ZXRayWait             = 0,
        XRAY_ZXRayFltrMove            ,
        XRAY_ZXRayWork                ,
        MAX_PSTN_MOTR3                ,

        //USBC_XCnct                   
        XRAY_XCnctWait             = 0,
        XRAY_XCnctLeftWork            ,//이거 나중에 1개씩 늘어날수 있음
        XRAY_XCnctRightWork           ,//이거 나중에 1개씩 늘어날수 있음
        MAX_PSTN_MOTR4                ,
                                   
        
    };

    //Manual Cycle.
    public enum mc : uint
    {
        NoneCycle                 = 0,
        AllHome                   = 1,
        INDX_Home                    ,
        XRAY_Home                    ,
        //USBC_Home                    ,
       
        INDX_CycleGet           = 20 ,
        INDX_CycleBarcode            ,
        INDX_CycleOut                ,
    
        XRAY_CycleConnect       = 30 ,
        XRAY_CycleReady              ,
        XRAY_CycleWork               ,
        XRAY_CycleAnalyze            ,
        XRAY_CycleCheck              ,

        XRAY_CycleManMacro      = 40 ,
        //USBC_CycleConnect       = 40 ,

        INDX_LdrPitchUp         = 50,
        INDX_LdrPitchDn             ,
        INDX_IdxPitchLeft           ,
        INDX_IdxPitchRight          ,  
        
        MAX_MANUAL_CYCLE
    };

    //Error Id
    public enum ei : uint
    {
        ETC_MainAir       =0,
        ETC_001             ,
        ETC_Emergency       ,
        ETC_003             ,
        ETC_Door            ,
        ETC_005             ,
        ETC_LotOpen         ,
        PRT_Crash           ,
        ETC_008             ,
        ETC_ToStartTO       ,
        ETC_ToStopTO        ,
        ETC_AllHomeTO       ,
        ETC_ManCycleTO      ,
        ETC_013             ,
        PRT_CycleTO         ,
        PRT_HomeTo          ,
        PRT_ToStartTO       ,
        PRT_ToStopTO        ,
        ETC_018             ,
        MTR_HomeEnd         ,
        MTR_NegLim          ,
        MTR_PosLim          ,
        MTR_Alarm           ,
        ETC_023             ,
        ATR_TimeOut         ,
        ETC_025             ,
        PKG_Dispr           , //
        PKG_Unknwn          ,
        ETC_028             , //--------------------------------- 장비공통에러.
        ETC_029             ,
        PRT_OverLoad        ,
        ETC_031             , 
        ETC_032             , 
        ETC_033             , // 비전 통신 에러. 현재 안씀
        ETC_034             ,
        ETC_035             ,
        ETC_036             ,
        ETC_037             ,
        ETC_038             ,
        ETC_039             , //높이 센서 스펙 Range 에러 화면에 Retry Skip창을 띄울때 이에러를 쓴다.
        ETC_040             , //높이 센서 Range 에러...  센서가 튀었을때 이에러가 뜬다.
        ETC_041             , // 디스펜서 에러
        ETC_042             , //체크 센서 터치 에러.
        ETC_043             , //배큠에러.
        ETC_044             , //픽 동작인데 배큠센서 온되어서 집을수 없는경우
        PRT_Barcode         , //바코드 에러.
        ETC_OptnSet         , //옵션 셋팅 에러
        ETC_UsbDisConnect   , //Barcode Print
        ETC_RS232           ,
        ETC_049             , //연산 에러
        ETC_050             , //Index작업 하려는데 Unloader에 자리 없으면...
        ETC_051             ,
        ETC_052             ,
        ETC_MacroErr        ,
        MAX_ERR    
    };

    //Error Id

    public class ti
    {
        public const int Frm = 1;
        public const int Dev = 2;
        public const int Sts = 3;
        public const int Max = 4;

    }

}





