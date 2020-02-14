
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
        public const string sEqpName = "HEC-725bt";
        public static bool  bUsbCamChange = false;
    }

    public class mac
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
        public const int FileCheck     = 11;
    }
    public enum fb : uint
    {
        Bwd    = 0 ,
        Fwd    = 1 , 
    }

    //Part ID
    public enum pi : uint
    {
        WRK      , //실 플르고 가시내고.
        OUT      , //실 감고 
        MAX_PART , 
    }


    //안씀.
    public class ri
    {
        public const int MAX_ARAY  = 0;        
    }
    
    //Chip Status
    //안씀.
    public enum cs : int
    {
        RetFail   = -1, //함수 리턴 
        None      =  0, //All
        MAX_CHIP_STAT

    };

    //mi<파트명3자리>_<축방샹1자리><부가명칭4> Motor Id
    public enum mi : uint
    {                   
        
        OUT_TRelT = 0 , //Reel1 360도 
        OUT_TRelB = 1 , //Reel2 360도
        OUT_YGuid = 2 , //벨트방식 지름 25.87mm 1바퀴당 81.273mm 3.14159265358979
        WRK_ZTabl = 3 , //실 써는 도마테이블 업다운.2.0mm
        WRK_YRott = 4 , //실 돌리는 모터.
        WRK_TFeed = 5 , //Roller Feeding Motor. 3.14159265358979 62.832
        WRK_YBlad = 6 , //칼날 In Out 
        WRK_TBlad = 7 , //칼날각도.
        WRK_TSwit = 8 , //칼날 써는 방향 선택
        
        MAX_MOTR       
    };

    //ai<파트명3자리>_<부가명칭><FWD시에위치2자리><BWD시에위치2자리>
    //대기가 Bwd 작동이 Fwd
    public enum ci : uint
    {
        MAX_ACTR = 0 ,
    };

    // Id
    public enum ax : uint
    {
        ETC_00              = 00, //
        ETC_01              = 01, //
        ETC_02              = 02, //
        ETC_03              = 03, //
        ETC_04              = 04, //
        ETC_05              = 05, //
        ETC_06              = 06, //
        ETC_07              = 07, //
    }

    public enum ay : uint
    {
        ETC_00              = 00, //
        ETC_01              = 01, //
        ETC_02              = 02, //
        ETC_03              = 03, //
        ETC_04              = 04, //
        ETC_05              = 05, //
        ETC_06              = 06, //
        ETC_07              = 07, //
    }
    //X Id
    //그냥 아이오.  <파트 3자리>+_+<세부설명>
    //실린더 아이오 <파트 3자리>+_+<세부설명>+<해당행동2자리 ex) Fw,Bw,Up,Dn>
    //Ft - Front , Rr - Rear 으로 통일 함.
    public enum xi : uint
    {
        ETC_StartSw     =  0 , //
        ETC_StopSw      =  1 , //
        ETC_ResetSw     =  2 , //
        ETC_MotorSw     =  3 , //  
        ETC_EmgSw       =  4 , //  
        ETC_ServoOn     =  5 , //  서보모터쪽 전체 파워
        ETC_SteppingOn  =  6 , //  스테핑쪽 전체 파워.
        ETC_HeaterOn    =  7 , //  히터쪽 파워
        OUT_TopSnsrR    =  8 , //  
        OUT_BtmSnsrF    =  9 , //  
        WRK_Heater1Alr  = 10 , //  
        WRK_Heater2Alr  = 11 , //  
        ETC_Spare12     = 12 , //
        ETC_Spare13     = 13 , //
        ETC_Spare14     = 14 , //  
        ETC_Spare15     = 15 , //
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
        ETC_MotorLp           =  3, // 
        ETC_SteppingOn        =  4, // 
        ETC_HeaterOnStage     =  5, // 
        ETC_HeaterOnBlade     =  6, //
        ETC_Spare7            =  7, // 
        ETC_TwLpR             =  8, // 
        ETC_TwLpY             =  9, // 
        ETC_TwLpG             = 10, // 
        ETC_TwLpBz            = 11, // 
        ETC_Spare12           = 12, // 
        ETC_Spare13           = 13, // 
        ETC_Spare14           = 14, // 
        ETC_Spare15           = 15, //
        MAX_OUTPUT
    };

    //Position Value id
    public enum pv : uint
    {
        //OUT_TRelT
        OUT_TRelTWait = 0,
        MAX_PSTN_MOTR0   ,

        //OUT_TRelB
        OUT_TRelBWait = 0,
        MAX_PSTN_MOTR1   ,

        //OUT_YGuid
        OUT_YGuidWait        = 0 ,
        OUT_YGuidFrnt            ,
        OUT_YGuidRear            ,
        MAX_PSTN_MOTR2           ,

        //WRK_ZTabl
        WRK_ZTablWait = 0,
        WRK_ZTabl0Wait   , //도마대기높이.
        WRK_ZTabl1Wait   , //도마대기높이.
        WRK_ZTabl2Wait   , //도마대기높이.
        WRK_ZTabl3Wait   , //도마대기높이.
        WRK_ZTabl4Wait   , //도마대기높이.
        WRK_ZTabl5Wait   , //도마대기높이.
        WRK_ZTabl6Wait   , //도마대기높이.
        WRK_ZTabl7Wait   , //도마대기높이.
        WRK_ZTabl8Wait   , //도마대기높이.
        WRK_ZTabl9Wait   , //도마대기높이.
        
        WRK_ZTabl0Work   , //도마높이.
        WRK_ZTabl1Work   , //도마높이.
        WRK_ZTabl2Work   , //도마높이.
        WRK_ZTabl3Work   , //도마높이.
        WRK_ZTabl4Work   , //도마높이.
        WRK_ZTabl5Work   , //도마높이.
        WRK_ZTabl6Work   , //도마높이.
        WRK_ZTabl7Work   , //도마높이.
        WRK_ZTabl8Work   , //도마높이.
        WRK_ZTabl9Work   , //도마높이.
        MAX_PSTN_MOTR3   ,

        //WRK_YRott
        WRK_YRottWait = 0,
        WRK_YRottMoveOfs ,
        WRK_YRottMoveEnd ,
        MAX_PSTN_MOTR4   ,

        //WRK_TFeed
        WRK_TFeedWait        = 0 ,
        WRK_TFeed0SpareOfs       , //1 처음시작하고 빈곳 오프셑 넣는것.
        WRK_TFeed0CogLengOfs     , //1 가시내는 구간 길이.
        WRK_TFeed0CogGapOfs      , //1 가시의 간격
        WRK_TFeed1SpareOfs       , //1 처음시작하고 빈곳 오프셑 넣는것.
        WRK_TFeed1CogLengOfs     , //1 가시내는 구간 길이.
        WRK_TFeed1CogGapOfs      , //1 가시의 간격
        WRK_TFeed2SpareOfs       , //2 처음시작하고 빈곳 오프셑 넣는것.
        WRK_TFeed2CogLengOfs     , //2 가시내는 구간 길이.
        WRK_TFeed2CogGapOfs      , //2 가시의 간격
        WRK_TFeed3SpareOfs       , //3 처음시작하고 빈곳 오프셑 넣는것.
        WRK_TFeed3CogLengOfs     , //3 가시내는 구간 길이.
        WRK_TFeed3CogGapOfs      , //3 가시의 간격
        WRK_TFeed4SpareOfs       , //4 처음시작하고 빈곳 오프셑 넣는것.
        WRK_TFeed4CogLengOfs     , //4 가시내는 구간 길이.
        WRK_TFeed4CogGapOfs      , //4 가시의 간격
        WRK_TFeed5SpareOfs       , 
        WRK_TFeed5CogLengOfs     , 
        WRK_TFeed5CogGapOfs      , 
        WRK_TFeed6SpareOfs       , 
        WRK_TFeed6CogLengOfs     , 
        WRK_TFeed6CogGapOfs      , 
        WRK_TFeed7SpareOfs       , 
        WRK_TFeed7CogLengOfs     , 
        WRK_TFeed7CogGapOfs      , 
        WRK_TFeed8SpareOfs       , 
        WRK_TFeed8CogLengOfs     , 
        WRK_TFeed8CogGapOfs      , 
        WRK_TFeed9SpareOfs       , 
        WRK_TFeed9CogLengOfs     , 
        WRK_TFeed9CogGapOfs      , 
        WRK_TFeedRepeatOfs       , //리피트 갯수가 2개 이상일때 그사이에 적용 되는 오프셑.
        WRK_TFeedEndOfs          , //1ea의 작업이 끝나고 그뒤에 붙는 옵셑.
        MAX_PSTN_MOTR5           ,

        //WRK_YBlad
        WRK_YBladWait = 0,
        WRK_YBlad0BfWork  ,       // 작업2단으로 구분하게 수정.
        WRK_YBlad1BfWork  ,       // 작업2단으로 구분하게 수정.
        WRK_YBlad2BfWork  ,       // 작업2단으로 구분하게 수정.
        WRK_YBlad3BfWork  ,       // 작업2단으로 구분하게 수정.
        WRK_YBlad4BfWork  ,       // 작업2단으로 구분하게 수정.
        WRK_YBlad5BfWork  ,       // 작업2단으로 구분하게 수정.
        WRK_YBlad6BfWork  ,       // 작업2단으로 구분하게 수정.
        WRK_YBlad7BfWork  ,       // 작업2단으로 구분하게 수정.
        WRK_YBlad8BfWork  ,       // 작업2단으로 구분하게 수정.
        WRK_YBlad9BfWork  ,       // 작업2단으로 구분하게 수정.
        
        WRK_YBlad0Work    ,
        WRK_YBlad1Work    ,
        WRK_YBlad2Work    ,
        WRK_YBlad3Work    ,
        WRK_YBlad4Work    ,
        WRK_YBlad5Work    ,
        WRK_YBlad6Work    ,
        WRK_YBlad7Work    ,
        WRK_YBlad8Work    ,
        WRK_YBlad9Work    ,
        MAX_PSTN_MOTR6,

        //WRK_TBlad
        WRK_TBladWait        = 0 , //
        WRK_TBlad0Cog            , //Left 썰때 높이.
        WRK_TBlad0CogUp          , //Left 위로 들출때 높이.
        WRK_TBlad1Cog            , //Left 썰때 높이.
        WRK_TBlad1CogUp          , //Left 위로 들출때 높이.
        WRK_TBlad2Cog            , //2 썰때 높이.
        WRK_TBlad2CogUp          , //2 위로 들출때 높이.
        WRK_TBlad3Cog            , //3 썰때 높이.
        WRK_TBlad3CogUp          , //3 위로 들출때 높이.
        WRK_TBlad4Cog            , //4 썰때 높이.
        WRK_TBlad4CogUp          , //4 위로 들출때 높이.
        WRK_TBlad5Cog            , //4 썰때 높이.
        WRK_TBlad5CogUp          , //4 위로 들출때 높이.
        WRK_TBlad6Cog            , //4 썰때 높이.
        WRK_TBlad6CogUp          , //4 위로 들출때 높이.
        WRK_TBlad7Cog            , //4 썰때 높이.
        WRK_TBlad7CogUp          , //4 위로 들출때 높이.
        WRK_TBlad8Cog            , //4 썰때 높이.
        WRK_TBlad8CogUp          , //4 위로 들출때 높이.
        WRK_TBlad9Cog            , //4 썰때 높이.
        WRK_TBlad9CogUp          , //4 위로 들출때 높이.
        MAX_PSTN_MOTR7           ,

        //WRK_TSwit
        WRK_TSwitWait        = 0 ,
        WRK_TSwitLWork           ,
        WRK_TSwitRWork           ,
        MAX_PSTN_MOTR8           ,

        

    };

    //Manual Cycle.
    public enum mc : uint
    {
        NoneCycle                 = 0,
        AllHome                   = 1,
        HomeEnd                   = 9, //혹시 실수 할까비.
        //메뉴얼 동작은 10번부터 1~9번까지는 파트 홈들.
        
        StepCut                   = 10 ,
        Move5mm                   = 11 ,
        Move100mm                 = 12 ,

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
        MAX_ERR    
    };

    public class ti
    {
        public const int Frm = 1;
        public const int Dev = 2;
        public const int Sts = 3;
        public const int Dxi = 4;
        public const int Dyi = 5;
        public const int Max = 6;
    }

}





