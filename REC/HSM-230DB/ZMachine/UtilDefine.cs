using System;

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
        public const string sEqpName = "HSM-230DB";
    }

    public enum fb : uint
    {
        Bwd    = 0 ,
        Fwd    = 1 , 
    }

    //Part ID
    public enum pi : uint
    {
        WSTG     = 0 , //웨이퍼 인덱스+ 스테이지 쪼금.
        SSTG         , //서브스트레이트 인덱스+ 스테이지 쪼금
        TOOL         , //리어툴,프론트툴,서브스트레이트스테이지,웨이퍼스테이지
        MAX_PART     , 
    }

    //aRay Id
    //public enum ri : uint
    //{
    //    WLDR     = 0,
    //    SLDR        ,
    //    WSTG        ,
    //    SSTG        ,
    //    PCKR        ,
    //    MAX_ARAY    ,
    //};
    public class ri
    {
        public const int WLDT = 0 ; //웨이퍼 로더 탑
        public const int WLDB = 1 ; //웨이퍼 로더 바텀
        public const int SLDT = 2 ; //서브스트레이트 로더 탑
        public const int SLDB = 3 ; //서브스트레이트 로더 바텀
        public const int WSTG = 4 ;
        public const int SSTG = 5 ;
        public const int PCKR = 6 ;
        public const int MAX_ARAY = 7;
        
    }
    
    //Chip Status
    //웨이퍼 및 서브스트레이트 추가시에 각각 FindChip 확인 및 autorun  확인 해야 함.
    public enum cs : int//<sun>화면에 팝업메뉴 손봐야함.
    {
        RetFail   = -1, //함수 리턴 
        None      =  0, //All
        Unknown       , //WLDR SLDR
        Mask          , //WLDR SLDR
        EjectAlign    , //WSTG
        Eject         , //WSTG
        Align         , //WSTG SSTG PCKR
        Pick          , //WSTG
        Empty         , //WSTG SSTG
        Fail          , //WSTG
        BtmVisn       , //          PCKR
        SubHeight     , //     SSTG
        Disp          , //     SSTG
        DispVisn      , //     SSTG
        Distance      , //          PCKR
        Attach        , //     SSTG PCKR
        EndVisn       , //     SSTG
        EndHeight     , //     SSTG
        BltHeight     , //     SSTG
        WorkEnd       , //     SSTG
        MAX_CHIP_STAT

    };
    //SSTG : Align->Height->Disp    ->DispVisn->Attach                                                ->EndVisn->EndHeight->WorkEnd
    //WSTG :                                            EjectAlign   ->Eject    ->Align->Pick  ->Empty   
    //PCKR :                                            None         ->BtmVisn  -> Attach

    


    //mi<파트명3자리>_<축방샹1자리><부가명칭4> Motor Id
    public enum mi : uint
    {                    //       홈위치
        WLDR_ZElev = 0 , //Sigma5 밑홈                           
        SLDR_ZElev     , //Sigma5 밑홈
        WSTG_YGrpr     , //Sigma5 뒤홈
        SSTG_YGrpr     , //Sigma5 뒤홈
        WSTG_TTble     , //Sigma5 시게방향홈 웨이퍼 중간이 홈.
        TOOL_YEjtr     , //Sigma5 뒤홈
        TOOL_ZEjtr     , //Sigma5 밑홈
        TOOL_ZPckr     , //Sigma5 위홈
        TOOL_YGent     , //Panasonic A5 뒤홈
        TOOL_YRsub     , //Panasonic A5
        TOOL_XRght     , //Panasonic A5 오른쪽홈
        TOOL_ZDisp     , //EZ Servo 위홈
        TOOL_XLeft     , //Panasonic A5 왼쪽홈
        TOOL_XEjtL     , //EZ Servo 오른쪽홈
        TOOL_XEjtR     , //EZ Servo 왼쪽홈
        TOOL_YVisn     , //EZ Servo 뒤홈
        SSTG_XRail     , //EZ Servo 벌리는게홈
        Spare17        , //EZ Servo 케이블만 말아져 있음. 
        WSTG_ZExpd     , //Autonics 2상 Stepping MD2U-DM20 위홈
        SSTG_ZRail     , //Autonics 2상 Stepping MD2U-DM20 위홈
        SSTG_YLeft     , //오리엔탈 5상. 뒤홈
        SSTG_YRght     , //오리엔탈 5상. 뒤홈
        SSTG_XFrnt     , //오리엔탈 5상. 오른쪽홈
        TOOL_ZVisn     , //오리엔탈 5상. 위홈.                   
        MAX_MOTR       
    };

    //ai<파트명3자리>_<부가명칭><FWD시에위치2자리><BWD시에위치2자리>
    //대기가 Bwd 작동이 Fwd
    public enum ci : uint
    {
        TOOL_GuidFtDwUp         = 0, // 
        TOOL_GuidRrDwUp         = 1, // 
        TOOL_DispCvFwBw         = 2, // 
        WSTG_RailCvsLtFwBw      = 3, // 
        WSTG_RailCvsRtFwBw      = 4,      
        WSTG_GrprClOp           = 5,
        SSTG_GrprClOp           = 6,
        SSTG_BoatClampClOp      = 7,
        MAX_ACTR
    };

    //X Id
    //그냥 아이오.  <파트 3자리>+_+<세부설명>
    //실린더 아이오 <파트 3자리>+_+<세부설명>+<해당행동2자리 ex) Fw,Bw,Up,Dn>
    //Ft - Front , Rr - Rear 으로 통일 함.
    public enum xi : uint
    {
        ETC_StartSwL        =  0 , //
        ETC_StopSwL         =  1 , //
        ETC_ResetSwL        =  2 , //
        ETC_InitSwL         =  3 , //  
        ETC_StartSwR        =  4 , //  
        ETC_StopSwR         =  5 , //  
        ETC_ResetSwR        =  6 , //  
        ETC_InitSwR         =  7 , //
        ETC_FtEmgSwL        =  8 , //  
        ETC_FtEmgSwR        =  9 , //  
        ETC_RtEmgSwF        = 10 , //  
        ETC_RtEmgSwR        = 11 , //  
        ETC_LtEmgSwF        = 12 , //
        ETC_LtEmgSwR        = 13 , //
        ETC_RrEmgSw         = 14 , //  
        ETC_15              = 15 , //  

        ETC_MainPower       = 16 , //  
        ETC_MainAir         = 17 , //  
        ETC_FtDoor          = 18 , //  
        ETC_RrDoor          = 19 , //  
        SSTG_BoatRailIn     = 20 , //  
        SSTG_BoatCheck      = 21 , //  
        SSTG_BoatVac        = 22 , //  
        SSTG_SubStratVac    = 23 , //  
        TOOL_NdleNotCheck   = 24 , //  
        WSTG_GrprOverload   = 25 , // 
        WSTG_GrprPkgCheck   = 26 , // 
        SSTG_GrprOverload   = 27 , // 
        SSTG_GrprPkgCheck   = 28 , // 
        WLDR_WfrOutCheck    = 29 , //  
        WLDR_MgzCheck       = 30 , //  
        SLDR_SstOutCheck    = 31 , //

        SLDR_MgzCheck       = 32 , //  
        TOOL_GuidRrUp       = 33, //  
        TOOL_GuidRrDw       = 34, //  
        TOOL_GuidFtUp       = 35,
        TOOL_GuidFtDw       = 36,
        TOOL_PckrVac        = 37 ,
        TOOL_DispCvFw       = 38 ,
        TOOL_DispCvBw       = 39 ,
        WSTG_RailCvsLtFw    = 40 ,
        WSTG_RailCvsLtBw    = 41 ,
        WSTG_RailCvsRtFw    = 42 ,
        WSTG_RailCvsRtBw    = 43 ,
        WSTG_EjtShortRt     = 44 ,
        WSTG_EjtShortLt     = 45 ,
        WSTG_EjtCrash       = 46 ,
        TOOL_Crash          = 47 ,

        ETC_48              = 48 ,
        ETC_49              = 49 ,
        ETC_50              = 50 ,
        ETC_51              = 51 ,
        ETC_52              = 52 ,
        ETC_53              = 53 ,
        VISN_Ready          = 54 ,//Vision1
        VISN_Busy           = 55 ,
        VISN_InspOk         = 56 ,
        VISN_Spare1         = 57 ,
        VISN_Spare2         = 58 ,
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
        ETC_StartLpL          =  0, // 
        ETC_StopLpL           =  1, // 
        ETC_ResetLpL          =  2, // 
        ETC_InitLpL           =  3, // 
        ETC_StartLpR          =  4, // 
        ETC_StopLpR           =  5, // 
        ETC_ResetLpR          =  6, // 
        ETC_InitLpR           =  7, // 
        ETC_TwLpR             =  8, // 
        ETC_TwLpY             =  9, // 
        ETC_TwLpG             = 10, // 
        ETC_TwBzz             = 11, // 
        ETC_012               = 12, // 
        ETC_LightOn           = 13, // 
        TOOL_PckrZBreak       = 14, // 
        WSTG_EjctZBreak       = 15, // 

        TOOL_LoadCellOff      = 16, // 
        SSTG_HeaterOn         = 17, // 
        WSTG_IonizerOn        = 18, // 
        SSTG_IonizerOn        = 19, // 
        WSTG_HeatGunOn        = 20, // 
        SSTG_BoatClampClOp    = 21, // 
        SSTG_SubsVac          = 22, // 
        TOOL_PckrVac          = 23, // 
        WSTG_GrprCl           = 24, // 
        SSTG_GrprCl           = 25, // 
        TOOL_GuidRrDw         = 26, // 
        TOOL_GuidFtDw         = 27, // 
        TOOL_DispCvFw         = 28, // 
        TOOL_DispCvBw         = 29, // 
        WSTG_RailCvsLtFw      = 30, // 
        WSTG_RailCvsLtBw      = 31, // 

        WSTG_RailCvsRtFw      = 32, //  
        WSTG_RailCvsRtBw      = 33, //  
        TOOL_DispOnOff        = 34, //  
        ETC_035               = 35, //  
        ETC_036               = 36, //  
        TOOL_PckrEjt          = 37, //  
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
        VISN_Command          = 54, //Vision1
        VISN_JobChange        = 55, //
        VISN_Reset            = 56, //
        VISN_InspStart        = 57, //
        VISN_Live             = 58, //
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
        WLDR_ZElevWait             = 0,
        WLDR_ZElevTopWorkStt          ,
        WLDR_ZElevBtmWorkStt          ,
        MAX_PSTN_MOTR0                ,

        SLDR_ZElevWait             = 0,
        SLDR_ZElevTopWorkStt          ,
        SLDR_ZElevBtmWorkStt          ,
        MAX_PSTN_MOTR1                ,
                                   
        WSTG_YGrprWait             = 0,    
        WSTG_YGrprPick                ,
        WSTG_YGrprPickWait            ,
        WSTG_YGrprBarcode             ,
        WSTG_YGrprPlace               ,
        MAX_PSTN_MOTR2                ,
                                   
        SSTG_YGrprWait             = 0,    
        SSTG_YGrprPick                ,
        SSTG_YGrprPickWait            ,
        SSTG_YGrprBarcode             ,
        SSTG_YGrprWorkStt             ,
        MAX_PSTN_MOTR3                ,
                                   
        WSTG_TTbleWait             = 0,
        WSTG_TTbleWork                ,
        WSTG_TTbleWfrWork             , //웨이퍼별로 세타가 많이 돌아가 있어서 웨이퍼 로딩시에 WSTG_TTbleWork에서 복사하여 넣어주고 에러시에 오퍼레이터가 이값을 바꿔준다.
        MAX_PSTN_MOTR4                ,
                                   
        TOOL_YEjtrWait             = 0,
        TOOL_YEjtrWorkStt             ,
        MAX_PSTN_MOTR5                ,
                                   
        TOOL_ZEjtrWait             = 0,
        TOOL_ZEjtrWork                ,
        MAX_PSTN_MOTR6                ,
                                   
        TOOL_ZPckrWait             = 0,
        TOOL_ZPckrPick                ,
        TOOL_ZPckrBVisn               ,
        TOOL_ZPckrPlce                ,
        TOOL_ZPckrCheck               ,//
        MAX_PSTN_MOTR7                ,
                                 
        TOOL_YGentWait             = 0, //디스펜서 더미샷하는 포지션으로 씀
        TOOL_YGentPkPickStt           ,
        TOOL_YGentPkBVsnM             ,
        TOOL_YGentPkBVsnS             ,
        TOOL_YGentPkPlce              , //서브스트레이트 비젼 검사시에 이포지션을 같이 쓴다.
        TOOL_YGentVsWStgVsnMStt       ,
        TOOL_YGentVsWStgVsnSStt       ,
        TOOL_YGentDispStt             ,
        TOOL_YGentHghtStt             ,
        TOOL_YGentBltStt              ,
        TOOL_YGentDispVisn1           ,
        TOOL_YGentDispVisn2           ,
        TOOL_YGentTVsnCheck           ,
        TOOL_YGentDispCheck           ,
        TOOL_YGentHghtCheck           ,
        TOOL_YGentPckrCheck           ,//
        TOOL_YGentVsWStgCtr           ,
        TOOL_YGentVsSStgCtr           ,
        TOOL_YGentConv                ,
        MAX_PSTN_MOTR8                ,

        TOOL_YRghtWait             = 0,
        MAX_PSTN_MOTR9                ,

        TOOL_XRghtWait             = 0, //디스펜서 더미샷하는 포지션으로 씀
        TOOL_XRghtVsSStgVsnRtM        ,
        TOOL_XRghtVsSStgVsnRtS        ,
        TOOL_XRghtVsSStgVsnLtM        ,
        TOOL_XRghtVsSStgVsnLtS        ,
        TOOL_XRghtVsWStgVsnMStt       ,
        TOOL_XRghtVsWStgVsnSStt       ,
        TOOL_XRghtDispStt             ,
        TOOL_XRghtHghtStt             ,
        TOOL_XRghtBltStt              ,
        TOOL_XRghtDispVisn1           ,
        TOOL_XRghtDispVisn2           ,
        TOOL_XRghtTVsnCheck           ,
        TOOL_XRghtDispCheck           ,
        TOOL_XRghtHghtCheck           ,
        TOOL_XRghtVsWStgCtr           ,
        TOOL_XRghtVsSStgCtr           ,
        MAX_PSTN_MOTR10               ,        

        TOOL_ZDispWait             = 0,
        TOOL_ZDispMove                ,
        TOOL_ZDispWork                ,
        TOOL_ZDispCheck               ,        
        MAX_PSTN_MOTR11               ,

        TOOL_XLeftWait             = 0,
        TOOL_XLeftPkPickStt           ,
        TOOL_XLeftPkBVsnM             ,
        TOOL_XLeftPkBVsnS             ,
        TOOL_XLeftPkPlce              ,
        TOOL_XLeftPkCheck             ,//
        TOOL_XLeftConv                ,
        MAX_PSTN_MOTR12               ,
                   
        WSTG_XEjtLWait             = 0,
        WSTG_XEjtLStart               ,
        WSTG_XEjtLEnd                 ,
        MAX_PSTN_MOTR13               ,

        WSTG_XEjtRWait             = 0,
        WSTG_XEjtRStart               ,
        WSTG_XEjtREnd                 ,
        MAX_PSTN_MOTR14               ,

        TOOL_YVisnWait             = 0,
        TOOL_YVisnWork                ,
        TOOL_YVisnSStgVsnRtM          ,
        TOOL_YVisnSStgVsnRtS          ,
        TOOL_YVisnSStgVsnLtM          ,
        TOOL_YVisnSStgVsnLtS          ,
        MAX_PSTN_MOTR15               ,
        
        SSTG_XRailWait             = 0,
        SSTG_XRailWork                ,
        MAX_PSTN_MOTR16               ,
        
        Spare17Wait                = 0,
        MAX_PSTN_MOTR17               ,
   
        WSTG_ZExpdWait             = 0,
        WSTG_ZExpdWork                ,
        MAX_PSTN_MOTR18               ,

        SSTG_ZRailWait             = 0,
        SSTG_ZRailDown                ,
        MAX_PSTN_MOTR19               ,
        
        SSTG_YLeftWait             = 0,
        MAX_PSTN_MOTR20               ,
        
        SSTG_YRghtWait             = 0,
        MAX_PSTN_MOTR21               ,
        
        SSTG_XFrntWait             = 0,
        MAX_PSTN_MOTR22               ,
              
        TOOL_ZVisnWait             = 0,
        TOOL_ZVisnSStgWfrWork         , // 서브스트레이트 스테이지 위에 웨이퍼 붙이기전에 검사시에 웨이퍼 높이
        TOOL_ZVisnSStgSbsWork         , // 서브스트레이트 높이
        TOOL_ZVisnSStgSbsEndWork      , // 다붙이고 난 웨이퍼 높이
        TOOL_ZVisnWStgWork            ,
        MAX_PSTN_MOTR23               
        
    };

    //Manual Cycle.
    public enum mc : uint
    {
        NoneCycle                 = 0,
        AllHome                   = 1,
        TOOL_Home                     ,
        WSTG_Home                     ,
        SSTG_Home                     ,
        TOOL_SubsAlignVisn        = 10,
        TOOL_WafrAlignVisn        = 11,
        WSTG_WaferGet             = 13,
        TOOL_Eject                = 14,
        TOOL_DispCheck            = 15,
        TOOL_HghtCheck            = 16,
        WSTG_ExpdWork             = 17,
        SSTG_SubsRailConv         = 18,
        SSTG_WafrRailConv         = 19,
        TOOL_PckrCheck            = 20,
        ETC_ConvPos               = 21,
        WSTG_WaferOut             = 22,
        SSTG_SubsOut              = 23,

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
        ETC_006             ,
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
        LTL_UVUseTime       ,
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
        STG_PickMiss        ,
        PRT_LoadCell        ,
        ETC_RS232           ,
        ETC_CalErr          , //연산 에러
        ULD_MgzSupply       , //Index작업 하려는데 Unloader에 자리 없으면...
        ETC_NeedPkg         , //장비에 자제 공급해주세용
        MAX_ERR    
    };  
}





