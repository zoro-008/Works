//19.07.18.1 
//비전 Command에서 타임아웃 떠서 타임아웃 VisnCom 5000->8000로 수정. 선계원

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
        public const string sEqpName = "OSW-10A";
    }

    public enum vi : uint //Vision Inspection Option
    {
        One    = 0 ,
        Col    = 1 ,
        All    = 2 ,
    }
    public enum fb : uint
    {
        Bwd    = 0 ,
        Fwd    = 1 , 
    }

    //Part ID
    public enum pi : uint
    {
        LODR     , //로더.
        IDXR     , //리어 인덱스
        IDXF     , //프론트 인덱스 
        TOOL     , //Vision & Picker & Reject Tray 부분
        STCK     , //Stocker
        BARZ     , //Barcode Picker 부분                                         BARZ
        MAX_PART , 
    }

    //aRay Id
    public class ri
    {
        public const int SPLR      = 0 ; //로더 트레이 공급존
        public const int IDXR      = 1 ; //리어 인덱스
        public const int IDXF      = 2 ; //프론트 인덱스
        public const int PCKR      = 3 ; //픽커
        public const int TRYF      = 4 ; //트레이 페일.
        public const int TRYG      = 5 ; //트레이 굿.
        public const int OUTZ      = 6 ; //인덱스 아웃존
        public const int STCK      = 7 ; //트레이 스타커
        public const int BARZ      = 8 ; //바코드 부착존                       BARZ
        public const int INSP      = 9 ; //비전 결과값 임시저장소.
        public const int PSTC      = 10; //스테커로 올리기전 위치.
        public const int MASK      = 11; //트레이가 안쓰는 포켓들이 있어 마스크가 있음.
 //       public const int BPCK      = 11; //바코드 픽커.
        public const int MAX_ARAY  = 12;
        
    }
    
    //Chip Status
    public enum cs : int//<sun>화면에 팝업메뉴 손봐야함.
    {
        RetFail   = -1, //함수 리턴 
        None      =  0, //All
        Unknown       , //검사전
        Vision        , 
        Empty         , //포켓이 실제로 비어서 자제를 채워 넣어야 하는경우.
        NG0           , //
        NG1           , //
        NG2           , 
        NG3           ,
        NG4           ,
        NG5           ,
        NG6           ,
        NG7           ,
        NG8           ,
        NG9           ,
        NG10          , //userDefine
        Good          , //여기까지 검사 결과
        Barcode       , //바코드 붙여야 되는 상태.
        WorkEnd       , //바코드 붙이고 밴딩기 태워야 함.
        BarRead       , //바코드 읽는거 에러일경우
        MAX_CHIP_STAT

    };
   


    //mi<파트명3자리>_<축방샹1자리><부가명칭4> Motor Id
    public enum mi : uint
    {                   //                           홈위치_장비전면 기준
        LODR_ZLift = 0 , //트레이 로더 Z축          , 아래                           
        TOOL_XRjct     , //Reject Tray X축          , 왼쪽
        IDXR_XRear     , //장비 전면 기준 뒤쪽 Index, 왼쪽
        IDXF_XFrnt     , //장비 전면 기준 앞쪽 Index, 왼쪽
        TOOL_YTool     , //툴 Y축                   , 뒤쪽
        TOOL_ZPckr     , //픽커 Z축                 , 위
        BARZ_XPckr     , //바코드 픽커 X축          , 홈위치
        BARZ_ZPckr     , //바코드 픽커 Z축          , 위
        STCK_ZStck     , //언로더 Stacking Z        , 아래
        TOOL_ZVisn     , //비젼 Z축                 , 위
        MAX_MOTR       
    };

    //ai<파트명3자리>_<부가명칭><FWD시에위치2자리><BWD시에위치2자리>
    //대기가 Bwd 작동이 Fwd
    public enum ci : uint
    {
        LODR_ClampClOp     = 0  ,
        LODR_SperatorUpDn  = 1  ,
        STCK_RailClOp      = 2  ,
        IDXR_ClampUpDn     = 3  ,
        IDXF_ClampUpDn     = 4  ,
        IDXR_ClampClOp     = 5  ,
        IDXF_ClampClOp     = 6  ,
        STCK_RailTrayUpDn  = 7  ,
        STCK_StackStprUpDn = 8  ,
        STCK_StackOpCl     = 9  ,
        BARZ_BrcdStprUpDn  = 10 ,
        BARZ_BrcdTrayUpDn  = 11 ,
        BARZ_YPckrFwBw     = 12 ,
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
        ETC_InitSw          =  3 , //  
        ETC_MstrSw          =  4 , //  
        ETC_FtDoor          =  5 , //  
        ETC_FtDoorSd        =  6 , //  
        ETC_LtDoorsd        =  7 , //
        ETC_RrDoorLt        =  8 , //  
        ETC_RrDoorRt        =  9 , //  
        ETC_RtDoorSd        = 10 , //  
        ETC_FtEmgSw         = 11 , //  
        ETC_LtEmgSw         = 12 , //
        ETC_RrEmgSw         = 13 , //
        ETC_RtEmgSw         = 14 , //  
        ETC_MainAir         = 15 , //

        ETC_MainPower       = 16 , //  
        LODR_TrayDtct       = 17 , //  
        RAIL_TrayDtct1      = 18 , //  
        RAIL_TrayDtct2      = 19 , //  
        RAIL_TrayDtct3      = 20 , //  
        RAIL_TrayDtct4      = 21 , //  
        STCK_RailOp         = 22 , //  
        STCK_RailCl         = 23 , //  
        IDXR_ClampUp        = 24 , //  
        IDXF_ClampUp        = 25 , // 
        IDXR_ClampCl        = 26 , // 
        IDXF_ClampCl        = 27 , // 
        IDXR_TrayDtct       = 28 , // 
        IDXF_TrayDtct       = 29 , //  
        IDXR_Overload       = 30 , //  
        IDXF_Overload       = 31 , //

        TOOL_PckrVac        = 32 , //  
        STCK_RailTrayUp     = 33 , //  
        STCK_RailTrayDn     = 34 , //  
        STCK_StackLtFw      = 35 ,
        STCK_StackLtBw      = 36 ,
        STCK_StackRtFw      = 37 ,
        STCK_StackRtBw      = 38 ,
        STCK_StackTrayDtct  = 39 ,
        STCK_StackStprUp    = 40 ,
        STCK_StackStprDn    = 41 ,
        BARZ_BrcdTrayDtct   = 42 ,
        BARZ_BrcdTrayUp     = 43 ,
        BARZ_BrcdTrayDn     = 44 ,
        BARZ_BrcdStprUp     = 45 ,
        BARZ_BrcdStprDn     = 46 ,
        BARZ_TrayOutDtct    = 47 ,

        BARZ_PckrFw         = 48 ,
        BARZ_PckrBw         = 49 ,
        BARZ_PckrVac        = 50 ,
        ETC_FlowMeter       = 51 ,
        ETC_BandingOut      = 52 ,
        STCK_StackUpDtct    = 53 ,
        BARZ_PckrBrcdDtct   = 54 ,//Vision1
        ETC_055             = 55 ,
        ETC_056             = 56 ,
        ETC_057             = 57 ,
        ETC_058             = 58 ,
        VISN_Ready          = 59 ,//VSNRB_Ready         = 59 ,
        VISN_Busy           = 60 ,//VSNRB_Busy          = 60 ,
        VISN_Result         = 61 ,//VSNRB_InspOk        = 61 ,
        VISN_Spare1         = 62 ,//VSNRB_Spare1        = 62 ,
        VISN_Spare2         = 63 ,//VSNRB_Spare2        = 63 ,
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
        ETC_InitLp            =  3, // 
        ETC_DoorLockLtFt      =  4, // 
        ETC_DoorLockRtFt      =  5, // 
        ETC_DoorLockLtLt      =  6, // 
        ETC_DoorLockRtLt      =  7, // 
        ETC_DoorLockLtRr      =  8, // 
        ETC_DoorLockRtRr      =  9, // 
        ETC_DoorLockLtRt      = 10, // 
        ETC_DoorLockRtRt      = 11, // 
        ETC_TwRedLp           = 12, // 
        ETC_TwYelLp           = 13, // 
        ETC_TwGrnLp           = 14, // 
        ETC_TwBzz             = 15, //

        LODR_ClampFwBw        = 16, // 
        LODR_StackUpDn        = 17, // 
        STCK_RailOp           = 18, // 
        STCK_RailCl           = 19, // 
        ETC_020               = 20, // 
        ETC_021               = 21, // 
        IDXR_ClampUp          = 22, // 
        IDXR_ClampDn          = 23, // 
        IDXF_ClampUp          = 24, // 
        IDXF_ClampDn          = 25, // 
        IDXR_ClampClOp        = 26, // 
        IDXF_ClampClOp        = 27, // 
        TOOL_PckrVac          = 28, // 
        STCK_RailTrayUp       = 29, // 
        STCK_RailTrayDn       = 30, // 
        STCK_StackStprUpDn    = 31, // 

        STCK_StackFw          = 32, //  
        STCK_StackBw          = 33, //  
        ETC_034               = 34, //  
        ETC_035               = 35, //  
        BARZ_BrcdStprUpDn     = 36, //  
        BARZ_BrcdTrayUp       = 37, //  
        BARZ_BrcdTrayDn       = 38, //  
        BARZ_YPckrFwBw        = 39, //  
        BARZ_PckrVac          = 40, //  
        BARZ_Blower           = 41, //  
        STCK_StackAC          = 42, //  
        BARZ_BrcdAC           = 43, //  
        ETC_LightOnOff        = 44, //  
        LODR_ZLiftBkOff       = 45, //  
        TOOL_ZPckrBkOff       = 46, //  
        BARZ_ZPckrBkOff       = 47, //  

        STCK_ZStckBkOff       = 48, //
        ETC_049               = 49, //
        ETC_050               = 50, //
        ETC_051               = 51, //
        ETC_052               = 52, //
        ETC_053               = 53, //
        ETC_054               = 54, //Vision1
        ETC_055               = 55, //
        VISN_LotStart         = 56, //
        ETC_057               = 57, //
        ETC_058               = 58, //ETC_058
        VISN_Command          = 59, //VSNB_Ready            = 59, //
        VISN_Change           = 60, //VSNB_Busy             = 60, //
        VISN_Reset            = 61, //VSNB_InspOk           = 61, //
        VISN_ManMode          = 62, //VSNB_Spare1           = 62, //
        VISN_ManInsp          = 63, //VSNB_Spare2           = 63, //
        MAX_OUTPUT
    };

    //Position Value id
    public enum pv : uint
    {
        LODR_ZLiftWait             = 0,
        LODR_ZLiftPick                ,
        LODR_ZLiftSperate             ,
        LODR_ZLiftPlace               ,
        MAX_PSTN_MOTR0                ,

        TOOL_XRjctWait             = 0,
        TOOL_XRjctWrkStt              ,
        MAX_PSTN_MOTR1                ,
                                   
        IDXR_XRearWait             = 0,
        IDXR_XRearClamp               ,
        IDXR_XRearBarcode             ,
        IDXR_XRearVsnStt1             ,
        IDXR_XRearWorkStt             ,
        IDXR_XRearUld                 ,
        MAX_PSTN_MOTR2                ,
        IDXR_XRearVsnStt2          = 4,
        IDXR_XRearVsnStt3             ,
        IDXR_XRearVsnStt4             ,
                                   
        IDXF_XFrntWait             = 0,
        IDXF_XFrntClamp               ,
        IDXF_XFrntBarcode             ,
        IDXF_XFrntVsnStt1             ,
        IDXF_XFrntWorkStt             ,
        IDXF_XFrntUld                 ,
        MAX_PSTN_MOTR3                ,
        IDXF_XFrntVsnStt2          = 4,
        IDXF_XFrntVsnStt3             ,
        IDXF_XFrntVsnStt4             ,
                                   
        TOOL_YToolWait             = 0,
        TOOL_YToolVsnStt1             ,        
        TOOL_YToolIdxWorkStt          ,
        TOOL_YToolNgTWorkStt          ,
        TOOL_YToolGdTWorkStt          ,
        MAX_PSTN_MOTR4                ,
        TOOL_YToolVsnStt2          = 2,
        TOOL_YToolVsnStt3             ,
        TOOL_YToolVsnStt4             ,
                                   
        TOOL_ZPckrWait             = 0,
        TOOL_ZPckrIdxPick             ,
        TOOL_ZPckrIdxPlace            ,
        TOOL_ZPckrMove                ,
        TOOL_ZPckrNgTWork            ,
        TOOL_ZPckrGdTWork             ,
        MAX_PSTN_MOTR5                ,
                                    
        BARZ_XPckrWait             = 0,
        BARZ_XPckrPick                ,
        BARZ_XPckrPlace               ,
        BARZ_XPckrBarc                ,
        BARZ_XPckrRemove              ,
        MAX_PSTN_MOTR6                ,
                                   
        BARZ_ZPckrWait             = 0,
        BARZ_ZPckrCylFw               , //픽 위치로 가기 전 실린더 Fwd 시키는 위치
        BARZ_ZPckrPick                ,
        BARZ_ZPckrMove                ,
        BARZ_ZPckrPlaceCheck          ,
        BARZ_ZPckrBarc                , //바코드 스캔 포지션
        BARZ_ZPckrPlaceOfs            ,
        BARZ_ZPckrRemove              ,
        MAX_PSTN_MOTR7                ,
                                 
        STCK_ZStckWait             = 0,
        STCK_ZStckWork                ,
        MAX_PSTN_MOTR8                ,

        TOOL_ZVisnWait             = 0,
        TOOL_ZVisnWork1               ,        
        MAX_PSTN_MOTR9                ,
        TOOL_ZVisnWork2            = 2,
        TOOL_ZVisnWork3               ,
        TOOL_ZVisnWork4               ,

        



    };

    //Manual Cycle.
    public enum mc : uint
    {
        NoneCycle                 = 0,
        AllHome                   = 1,
        LODR_Home                    ,
        TOOL_Home                    ,
        BARZ_Home                    ,
        IDXR_Home                    ,
        IDXF_Home                    ,
        STCK_Home                    ,
        CycleVision                  , 

        LODR_CycleSply          = 20 , 

        IDXF_CycleGet           = 30 ,
        IDXF_CycleBarcode            ,
        IDXF_CycleOut                ,
        IDXR_CycleGet                ,
        IDXR_CycleBarcode            ,
        IDXR_CycleOut                ,

        TOOL_CycleVisn         = 40  ,
        TOOL_CycleNGPick             ,
        TOOL_CycleNGPlace            ,
        TOOL_CycleGoodPick           ,
        TOOL_CycleGoodPlace          ,

        STCK_CycleToStack      = 50  ,
        STCK_CycleStack              ,
        STCK_CycleOut                ,

        BARZ_CycleBarPick      = 60  ,
        BARZ_CycleBarPlace           ,
        BARZ_CycleOut                ,        

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
        PKG_Supply          ,
        PRT_OverLoad        ,
        VSN_InspRangeOver   , // 현재 안쓰고 있는 에러
        VSN_InspNG          , 
        VSN_ComErr          , //비전 통신 에러.
        PRT_Missed          ,
        PRT_Detect          ,
        PRT_RemovePkg       ,
        VSN_PkgCrash        ,
        PCK_PickMiss        ,
        HGT_RangeOver       , //높이 센서 스펙 Range 에러 화면에 Retry Skip창을 띄울때 이에러를 쓴다.
        HGT_RangeErr        , //높이 센서 Range 에러...  센서가 튀었을때 이에러가 뜬다.
        LTL_Disp            , // 디스펜서 에러
        PRT_CheckErr        , //체크 센서 터치 에러.
        PRT_VacErr          , //배큠에러.
        PRT_VaccSensor      , //픽 동작인데 배큠센서 온되어서 집을수 없는경우
        PRT_Barcode         , //바코드 에러.
        BAR_PickMiss        , //바코드 픽미스
        ETC_BarcPrint       , //Barcode Print
        ETC_RS232           ,
        ETC_CalErr          , //연산 에러
        PRT_NeedTraySupply  , //Index작업 하려는데 Unloader에 자리 없으면...
        PRT_FullTray        ,
        PRT_RemoveTray      ,
        ETC_FlowMeter       ,
        PCK_CoverTray       ,
        ETC_Oracle          , //디비 관련 에러.

        MAX_ERR    
    };  
}





