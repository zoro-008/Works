using System;

namespace Machine
{
    //Serial port ID
    public enum si : uint
    {
        Marking  = 1, 
        MAX_RS232 
    }
    public enum fb : uint
    {
        Bwd    = 0 ,
        Fwd    = 1 , 
    }

    //Part ID
    public enum pi : uint
    {
        LDR      = 0,
        IDX         ,
        PCK         ,
        MAX_PART
    }

    //aRay Id
    public enum ri : uint
    {
        LDR      = 0,
        PRI         ,
        WRK         ,
        PCK         ,
        ALN         ,    
        MAX_ARAY
    };
    
    //Chip Status
    public enum cs : int
    {
        RetFail = -1,
        None    =  0,
        Unkwn       ,
        Align       ,
        Marking     ,
        Good        ,
        Fail        ,
        Empty       ,
        Working     ,
        MAX_CHIP_STAT
    };



    //mi<파트명3자리>_<축방샹1자리><부가명칭4> Motor Id
    public enum mi : uint
    {
        LDR_Z = 0,
        IDX_X    ,
        PCK_Y    ,
        PCK_Z    , 
        MAX_MOTR
    };

    //ai<파트명3자리>_<부가명칭><FWD시에위치2자리><BWD시에위치2자리>
    //대기가 Bwd 작동이 Fwd
    public enum ai : uint
    {
        LDR_GripClOp    = 0 ,
        LDR_GripDnUp    = 1 ,
        LDR_GripRtLt    = 2 ,
        LDR_TrayFixClOp = 3 ,//트레이 들어올릴때 끼는 애들이 간혹 있어서 이 실린더로 붙잡은 상태에서 들어올림
        IDX_IdxUpDn     = 4 ,//3 ,
        IDX_StockUpDn   = 5 ,//4 ,
        
        MAX_ACTR
    };

    //X Id
    //그냥 아이오.  <파트 3자리>+_+<세부설명>
    //실린더 아이오 <파트 3자리>+_+<세부설명>+<해당행동2자리 ex) Fw,Bw,Up,Dn>
    public enum xi : uint
    {
        //Ft - Front , Rr - Rear 으로 통일 함.
        ETC_StartSw         =  0 , //  
        ETC_StopSw          =  1 , //  
        ETC_ResetSw         =  2 , //  
        ETC_AirSw           =  3 , //  
        ETC_InitSw          =  4 , //  
        ETC_EmgSw1          =  5 , //  
        ETC_EmgSw2          =  6 , //  
        ETC_DoorFt          =  7 , //  
        ETC_DoorRr          =  8 , //  
        ETC_DoorLt          =  9 , //  
        ETC_DoorRt          = 10 , //  
        ETC_MainPowor       = 11 , //  
        ETC_MainAir         = 12 , //  
        LDR_TrayPstn        = 13,  //트레이 적정 높이 감지 센서.    
        LDR_TrayDir         = 14 , //Tray Direction.
        LDR_TrayOver        = 15 , //로더쪽에 Z축 대기 포지션일때 사람이 너무 많이 넣으면 감지 해서 에러.  
        //
        LDR_GripLt          = 16,  //  
        LDR_GripRt          = 17,  //  
        LDR_GripUp          = 18,  //  
        LDR_GripDn          = 19,  //  
        LDR_GripCl          = 20 , //  
        LDR_GripOp          = 21 , //  
        LDR_TrayDetect      = 22 , //  
        IDX_IdxUp           = 23 , //  
        IDX_IdxDn           = 24 , //  
        IDX_Pri             = 25 , // 인덱스 프리인덱스 자제 유무  
        IDX_PriOut          = 26 , // 인덱스 프리인덱스 자제 완전나감 
        IDX_Uld             = 27 , // 언로더 자제 유무.
        IDX_UldOver         = 28 , // 언로더 꽉참.
        IDX_StockUp         = 29 , //  
        IDX_StockDn         = 30 , //  
        PCK_MrkrPkg         = 31 , // 아마도 마킹기 앞의 센서.
        
        PCK_Vcc1            = 32 , //  
        PCK_Vcc2            = 33 , //  
        PCK_Vcc3            = 34 , //  
        PCK_Vcc4            = 35 , //  
        PCK_Vcc5            = 36 , //  
        PCK_Vcc6            = 37 , //  
        PCK_Vcc7            = 38 , //  
        PCK_Vcc8            = 39 , //  
        ETC_040             = 40 , //  
        ETC_041             = 41 , //  
        ETC_042             = 42 , //  
        ETC_043             = 43 , //  
        ETC_044             = 44 , //  
        ETC_045             = 45 , //  
        ETC_046             = 46 , //  
        ETC_047             = 47 , // 
 
        ETC_048             = 48 , //
        ETC_049             = 49 , //
        ETC_050             = 50 , //
        ETC_051             = 51 , //
        ETC_052             = 52 , //
        ETC_053             = 53 , //
        ETC_054             = 54 , //
        ETC_055             = 55 , //
        ETC_056             = 56 , //
        ETC_057             = 57 , //
        ETC_058             = 58 , //
        ETC_059             = 59 , //
        ETC_060             = 60 , //
        ETC_061             = 61 , //
        ETC_062             = 62 , //
        ETC_063             = 63 , //
       

        MAX_INPUT
    };

    //그냥 아이오31.  <파트 3자리>+_+<세부설명>
    //복동 실린더 아이오 <파트 3자리>+_+<세부설명>+<해당행동 ex) Fw , Bw , Dn 등등 2자리>     자리
    //단동 실린더 아이오 <파트 3자리>+_+<세부설명>+<Fw시에해당행동2자리><Bw시에해당행동2자리> 동
    public enum yi : uint
    {
        ETC_StartLp           =  0, //  yETC_StartLp
        ETC_StopLp            =  1, //  yETC_StopLp
        ETC_ResetLp           =  2, //  yETC_ResetLp
        ETC_AirLp             =  3, //  yETC_AirLp
        ETC_InitLp            =  4, //  yETC_InitLp
        ETC_LightOnOff        =  5, //  yETC_LightOnOff
        LDR_ZBreak            =  6, //  yLDR_ZBreak
        ETC_MainAirSol        =  7, //  yETC_MainAirSol
        PCK_ZBreak            =  8, //  yPCK_ZBreak
        ETC_TwRedLp           =  9, //  ETC_TwRedLp
        ETC_TwYelLp           = 10, //  ETC_TwYelLp
        ETC_TwGrnLp           = 11, //  ETC_TwGrnLp
        ETC_TwBzz             = 12, //  ETC_TwBzz
        IDX_AlgnBlw           = 13, //  IDX_AlgnBlw
        LDR_GripCl            = 14, //  LDR_TrayGripUp
        LDR_GripOp            = 15, //  LDR_TrayGripDn

        LDR_GripDn            = 16, //  yLDR_TrayZUp
        LDR_GripUp            = 17, //  yLDR_TrayZDn
        LDR_GripRt            = 18, //  yLDR_TrayMoveXRt
        LDR_GripLt            = 19, //  yLDR_TrayMoveXLt
        IDX_IdxUp             = 20, //  yIDX_TrayIdxBarZUp
        IDX_IdxDn             = 21, //  yIDX_TrayIdxBarZDn
        IDX_StockUp           = 22, //  yIDX_TrayUldZUp
        IDX_StockDn           = 23, //  yIDX_TrayUldZDn
        LDR_TrayFixClOp       = 24, //PCK_VacSol            = 24, //  PCK_VacSol
        ETC_025               = 25, //  y025
        PCK_PrntOn            = 26, //  y026
        ETC_027               = 27, //  y027
        ETC_028               = 28, //  y028
        ETC_029               = 29, //  y029
        ETC_030               = 30, //  y030
        ETC_031               = 31, //  y031

        PCK_Vcc1              = 32, //  
        PCK_Ejt1              = 33, //  
        PCK_Vcc2              = 34, //  
        PCK_Ejt2              = 35, //  
        PCK_Vcc3              = 36, //  
        PCK_Ejt3              = 37, //  
        PCK_Vcc4              = 38, //  
        PCK_Ejt4              = 39, //  
        PCK_Vcc5              = 40, //  
        PCK_Ejt5              = 41, //  
        PCK_Vcc6              = 42, //  
        PCK_Ejt6              = 43, //  
        PCK_Vcc7              = 44, //  
        PCK_Ejt7              = 45, //  
        PCK_Vcc8              = 46, //  
        PCK_Ejt8              = 47, // 
 
        ETC_048               = 48, //
        ETC_049               = 49, //
        ETC_050               = 50, //
        ETC_051               = 51, //
        ETC_052               = 52, //
        ETC_053               = 53, //
        ETC_054               = 54, //
        ETC_055               = 55, //
        ETC_056               = 56, //
        ETC_057               = 57, //
        ETC_058               = 58, //
        ETC_059               = 59, //
        ETC_060               = 60, //
        ETC_061               = 61, //
        ETC_062               = 62, //
        ETC_063               = 63, //

        MAX_OUTPUT
    };

    //Position Value id
    public enum pv : uint
    {
        //mi.LDR_Z = 0  ,
        LDR_ZWait    = 0,
        LDR_ZWorkStt    ,
        MAX_PSTN_MOTR0  ,

        //mi.IDX_X = 1  
        IDX_XWait   = 0,
        IDX_XPickStt   ,
        IDX_XOut       ,
        MAX_PSTN_MOTR1 ,

        //mi.PCK_Y = 2
        PCK_YWait   = 0,
        PCK_YPick      ,
        PCK_YVisn      ,
        PCK_YPrnt      ,
        PCK_YPlce      ,
        MAX_PSTN_MOTR2 ,

        //mi.PCK_Z = 3  ,
        PCK_ZWait   = 0,
        PCK_ZMove      ,
        PCK_ZPick      ,
        PCK_ZVisn      ,
        PCK_ZPrnt      ,
        PCK_ZPlce      ,
        MAX_PSTN_MOTR3 ,

        
    };

    //Manual Cycle.
    public enum mc : uint
    {
        NoneCycle                 = 0,
        AllHome                   = 1,
        LDR_Home                  = 2,
        IDX_Home                  = 3,
        PCK_Home                  = 4,
        TrayPlce                  = 5,

        LDR_CycleSupply           = 10,
        LDR_CycleWork             = 11,
        LDR_TraySupply            = 12,
        
        PCK_CyclePick             = 20,
        PCK_CycleVisn             = 21,
        PCK_CyclePrnt             = 22,           
        PCK_CyclePlce             = 23,           
        
        IDX_CycleSupply           = 30,
        IDX_CycleWork             = 31,
        IDX_CycleOut              = 32,
        
        
        MAX_MANUAL_CYCLE
    };

    //Error Id
    public enum ei : uint
    {
        /*000*/ETC_MainAir      = 0,
        /*001*/ei001               ,
        /*002*/ETC_Emergency       ,
        /*003*/ei003               ,
        /*004*/ETC_Door            ,
        /*005*/ei005               ,
        /*006*/ei006               ,
        /*007*/PRT_Crash           ,
        /*008*/ei008               ,
        /*009*/ETC_ToStartTO       ,
        /*010*/ETC_ToStopTO        ,
        /*011*/ETC_AllHomeTO       ,
        /*012*/ETC_ManCycleTO      ,
        /*013*/ei013               ,
        /*014*/PRT_CycleTO         ,
        /*015*/PRT_HomeTo          ,
        /*016*/PRT_ToStartTO       ,
        /*017*/PRT_ToStopTO        ,
        /*018*/ei018               ,
        /*019*/MTR_HomeEnd         ,
        /*020*/MTR_NegLim          ,
        /*021*/MTR_PosLim          ,
        /*022*/MTR_Alarm           ,
        /*023*/ei023               ,
        /*024*/ATR_TimeOut         ,
        /*025*/ei025               ,
        /*026*/PKG_Dispr           ,
        /*027*/PKG_Unknwn          ,
        /*028*/ei028               ,
        /*029*/ei029               ,
        /*030*/PRT_OverLoad        ,
        /*031*/VSN_InspRangeOver   ,
        /*032*/VSN_InspNG          ,
        /*033*/ei033               ,
        /*034*/PRT_Missed          ,
        /*035*/PRT_Detect          ,
        /*036*/PRT_RemovePkg       ,
        /*037*/VSN_PkgCrash        ,
        /*038*/PCK_PickMiss        ,
        /*039*/ei039               ,
        /*040*/ei040               ,
        /*041*/ei041               ,
        /*042*/PRT_CheckErr        ,
        /*043*/PRT_VacErr          ,
        /*044*/PRT_VaccSensor      ,
        /*045*/VSN_ComErr          ,
        /*046*/ei046               ,
        /*047*/ei047               ,
        /*048*/ETC_RS232           ,
        /*049*/ETC_CalErr          ,
        /*050*/PRT_TrayErr         ,
        /*051*/PCK_VisnComErr      ,
    
        MAX_ERR    
    };  
}





