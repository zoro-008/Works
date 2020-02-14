using System;

namespace Machine
{
    //Serial port ID
    public enum si : uint
    {
        MAX_RS232 
    }

    //Part ID
    public enum pi : uint
    {
        ASY     =0,
        MAX_PART
    }

    //aRay Id
    public enum ri : int
    {
        NONE       = -1 ,
        PICK       = 0  ,
        REAR       ,    //Rear  Housing Stage
        FRNT       ,    //Front Housing Stage
        LENS       ,    //Lens Stage                   
        MAX_ARAY
    };
    
    //Chip Status
    public enum cs : int
    {
        RetFail = -1,
        None    =  0,
        Unkwn       ,
        Visn        ,
        Align       ,
        Work        ,
        Empty       ,
        Fail        ,
        TorqueFail  ,
        SensorFail  ,
        LastVisn    ,
        LastVisnFail,
        MAX_CHIP_STAT
    };



    //mi<파트명3자리>_<축방샹1자리><부가명칭4> Motor Id
    public enum mi : uint
    {
          
        PCK_X  = 0,
        STG_Y     ,
        PCK_ZL    ,
        PCK_ZR    ,
        PCK_TL    ,
        PCK_TR    ,
        MAX_MOTR
    };

    //ai<파트명3자리>_<부가명칭><FWD시에위치2자리><BWD시에위치2자리>
    //대기가 Bwd 작동이 Fwd
    public enum ai : uint
    {
        MAX_ACTR = 0
    };

    //X Id
    //그냥 아이오.  <파트 3자리>+_+<세부설명>
    //실린더 아이오 <파트 3자리>+_+<세부설명>+<해당행동2자리 ex) Fw,Bw,Up,Dn>
    public enum xi : uint
    {
        //Ft - Front , Rr - Rear 으로 통일 함.
        ETC_StartSw         =  0 , //  
        ETC_ResetSw         =  1 , //  
        ETC_StopSw          =  2 , //  
        ETC_EmgSw           =  3 , //  
        ETC_DoorFt          =  4 , //  
        ETC_DoorRr          =  5 , //  
        PCK_VacLt           =  6 , //  
        PCK_VacRt           =  7 , //  
        STG_VacLens         =  8 , //  
        PCK_HghtLt          =  9 , //  
        PCK_HghtRt          = 10 , //  
        ETC_11              = 11 , //  
        ETC_12              = 12 , //  
        ETC_13              = 13 , //  
        ETC_14              = 14 , //
        ETC_15              = 15 , //  
        //
        ETC_16              = 16,  //  
        ETC_17              = 17,  //  
        ETC_18              = 18,  //  
        ETC_19              = 19,  //  
        ETC_20              = 20 , //  
        ETC_21              = 21 , //  
        ETC_22              = 22 , //  
        ETC_23              = 23 , //  
        ETC_24              = 24 , //  
        ETC_25              = 25 , // 인덱스 프리인덱스 자제 유무  
        ETC_26              = 26 , // 인덱스 프리인덱스 자제 완전나감 
        ETC_27              = 27 , // 언로더 자제 유무.
        ETC_28              = 28 , // 언로더 꽉참.
        ETC_29              = 29 , //  
        ETC_30              = 30 , //  
        ETC_31              = 31 , // 아마도 마킹기 앞의 센서.
        
        MAX_INPUT
    };

    //그냥 아이오31.  <파트 3자리>+_+<세부설명>
    //복동 실린더 아이오 <파트 3자리>+_+<세부설명>+<해당행동 ex) Fw , Bw , Dn 등등 2자리>     자리
    //단동 실린더 아이오 <파트 3자리>+_+<세부설명>+<Fw시에해당행동2자리><Bw시에해당행동2자리> 동
    public enum yi : uint
    {
        ETC_StartLp           =  0, //  
        ETC_ResetLp           =  1, //  
        ETC_StopLp            =  2, //  
        ETC_TwRedLp           =  3, //  
        ETC_TwYelLp           =  4, //  
        ETC_TwGrnLp           =  5, //  
        ETC_TwBzz             =  6, //  
        PCK_VacLtOn           =  7, //  
        PCK_VacRtOn           =  8, //  
        PCK_EjtLtOn           =  9, //  
        PCK_EjtRtOn           = 10, //  
        STG_VacLenOn          = 11, //렌즈 스테이지 배큠온.
        ETC_LightOnOff        = 12, //  
        PCK_13                = 13, //  
        PCK_14                = 14, //  
        PCK_15                = 15, //  

        ETC_16                = 16, //  
        ETC_17                = 17, //  
        ETC_18                = 18, //  
        ETC_19                = 19, //  
        ETC_20                = 20, //  
        ETC_21                = 21, //  
        ETC_22                = 22, //  
        ETC_23                = 23, //  
        ETC_24                = 24, //  
        ETC_25                = 25, //  
        ETC_26                = 26, //  
        ETC_27                = 27, //  
        ETC_28                = 28, //  
        ETC_29                = 29, //  
        ETC_30                = 30, //  
        ETC_31                = 31, //  

        MAX_OUTPUT
    };

    //Position Value id
    //STG_YRPlceStt R은 Rear, F는 Front  ,
    public enum pv : uint
    {
        //mi.PCK_X = 1
        PCK_XWait = 0       ,
        PCK_XVisnLensStt    ,
        PCK_XVisnRearStt    ,
        PCK_XVisnFrntStt    ,
        PCK_XAlgn           ,
        PCK_XVisnPck1Ofs    , //Left Tool
        PCK_XVisnPck2Ofs    , //Right Tool
        MAX_PSTN_MOTR0      ,

        //mi.STG_Y = 0  ,
        STG_YWait = 0       ,
        STG_YVisnLensStt    ,
        STG_YVisnRearStt    ,
        STG_YVisnFrntStt    ,
        STG_YAlgn           ,
        STG_YChange         ,
        STG_YVisnPck1Ofs    , //Vision Start Positioin으로 부터 Picker1까지의 Offset, Left Tool
        STG_YVisnPck2Ofs    , //Vision Start Positioin으로 부터 Picker2까지의 Offset, Right Tool
        MAX_PSTN_MOTR1      ,

        //mi.PCK_ZL = 2  ,
        PCK_ZLWait       = 0,
        PCK_ZLPick          , 
        PCK_ZLPlce          , 
        PCK_ZLAlgnPick      ,
        PCK_ZLAlgnPlce      ,
        PCK_ZLUnlock        ,
        MAX_PSTN_MOTR2      ,
                         
        //mi.PCK_ZR = 3  
        PCK_ZRWait       = 0,
        PCK_ZRPick          , 
        PCK_ZRPlce          , 
        PCK_ZRAlgnPick      ,
        PCK_ZRAlgnPlce      ,
        PCK_ZRUnlock        ,
        MAX_PSTN_MOTR3      ,
                         
        //mi.PCK_TL = 4  
        PCK_TLWait       = 0,
        PCK_TLVisnZero      , //픽커와 비전 켈리브레이션 하면 자동 세팅됌.
        PCK_TLRvrsWork      , //처음에 돌리기 전에 역방향으로 조금 돌려서 채움. 끼우는방식은 0으로 사용.
        PCK_TLWorkOfs       , //높이 방식이 아닌 상태 혹은 틈새에 끼우는 방식에서 에서 채우는 량.
        PCK_TLUnlockWork    , //채워져 있는 것을 풀르는 양. 토크방식사용시에 돌리다 토크 걸리면 푸를때랑 , 홀더에서 렌즈빼는 언락사이클에서 이용.
        PCK_TLHolderPutOfs  , //틈새에 끼우는 디바이스용. 보통 체결방식은 그냥 0으로 놓으면됌.
        MAX_PSTN_MOTR4      ,
                         
        //mi.PCK_TR = 5  
        PCK_TRWait       = 0,
        PCK_TRVisnZero      ,
        PCK_TRRvrsWork      ,
        PCK_TRWorkOfs       ,
        PCK_TRUnlockWork    ,
        PCK_TRHolderPutOfs  , //틈새에 끼우는 디바이스용. 보통 체결방식은 그냥 0으로 놓으면됌.
        MAX_PSTN_MOTR5      ,
    };

    //Manual Cycle.
    public enum mc : uint
    {
        NoneCycle                 = 0,
        AllHome                   = 1,
        ASY_Home                  = 2,
       
        ASY_CycleToolCalib        = 11,
        ASY_CycleTorqueCheck      = 12,
        ASY_CycleTrayOut          = 13,
        ASY_CycleTrayIn           = 14,
        ASY_CycleHoldrCalib       = 15,
        ASY_CycleToolEccentric    = 16,

        ASY_CycleInsp             = 20,

        MAX_MANUAL_CYCLE
    };

    //Error Id
    public enum ei : uint
    {
        /*000*/eiETC_MainAir      = 0,
        /*001*/ei001                 ,
        /*002*/ETC_Emergency         ,
        /*003*/ei003                 ,
        /*004*/ETC_Door              ,
        /*005*/ei005                 ,
        /*006*/ei006                 ,
        /*007*/PRT_Crash             ,
        /*008*/ei008                 ,
        /*009*/ETC_ToStartTO         ,
        /*010*/ETC_ToStopTO          ,
        /*011*/ETC_AllHomeTO         ,
        /*012*/ETC_ManCycleTO        ,
        /*013*/ei013                 ,
        /*014*/PRT_CycleTO           ,
        /*015*/PRT_HomeTo            ,
        /*016*/PRT_ToStartTO         ,
        /*017*/PRT_ToStopTO          ,
        /*018*/ei018                 ,
        /*019*/MTR_HomeEnd           ,
        /*020*/MTR_NegLim            ,
        /*021*/MTR_PosLim            ,
        /*022*/MTR_Alarm             ,
        /*023*/ei023                 ,
        /*024*/ATR_TimeOut           ,
        /*025*/ei025                 ,
        /*026*/PKG_Dispr             ,
        /*027*/PKG_Unknwn            ,
        /*028*/PKG_WorkEnd           ,
        /*029*/ei029                 ,
        /*030*/PRT_OverLoad          ,
        /*031*/VSN_InspRangeOver     ,
        /*032*/VSN_InspNG            ,
        /*033*/VSN_ComErr            ,
        /*034*/PRT_Missed            ,
        /*035*/PRT_Detect            ,
        /*036*/ei036                 ,
        /*037*/ei037                 ,
        /*038*/PCK_PickMiss          ,
        /*039*/ei039                 ,
        /*040*/ei040                 ,
        /*041*/ei041                 ,
        /*042*/ei042                 ,
        /*043*/PRT_VacErr            ,
        /*044*/PRT_VaccSensor        ,
        /*045*/PRT_LensPut           ,
        /*046*/ei046                 ,
        /*047*/ei047                 ,
        /*048*/ETC_RS232             ,
        /*049*/PRT_CalibrationFail   ,
        /*050*/PRT_TraySupply        ,
        /*051*/ei051                 ,
        /*052*/ei052                 ,
        /*053*/ei053                 ,
        /*054*/ei054                 ,
        /*055*/ei055                 ,
        /*056*/ei056                 ,
        /*057*/ei057                 ,
        /*058*/ei058                 ,
        /*059*/ei059                 ,
    
        MAX_ERR    
    };  
}





