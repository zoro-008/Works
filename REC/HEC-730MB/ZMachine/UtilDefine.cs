
namespace Machine
{
    public class Eqp
    {
        public const string sEqpName    = "HEC-730MB";
        public static int   iUsbCnt     = 0;
        public static bool  bDeviceCng1 = false;
        public static bool  bDeviceCng2 = false;
    }

    
    //public enum vi : uint //Vision Inspection Option
    //{
    //    One    = 0 ,
    //    Col    = 1 ,
    //    All    = 2 ,
    //}
    public enum Mode : uint
    {
        Height    = 0 ,
        Weight    = 1 ,
        Pull_Dest = 2 ,
        GripH     = 3 ,
        //Dest   = 3,
    }

    public enum fb : uint
    {
        Bwd    = 0 ,
        Fwd    = 1 , 
    }

    //Part ID
    public enum pi : uint
    {
        LEFT     , //Bending.
        RIGH     , //Pulling, Biting.
        MAX_PART , 
    }

    //aRay Id
    public class ri
    {
        public const int ARAY      = 0 ; //로더 트레이 공급존
        public const int MAX_ARAY  = 1 ;
        
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
    {                    //                           홈위치_장비전면 기준
        L_UP_ZLift = 0 , //좌측 UPSIDE Z축          , 위
        L_DN_ZLift     , //좌측 DNSIDE Z축          , 아래                           
        R_UP_ZLift     , //우측 UPSIDE Z축          , 위
        MAX_MOTR       
    };

    //ai<파트명3자리>_<부가명칭><FWD시에위치2자리><BWD시에위치2자리>
    //대기가 Bwd 작동이 Fwd
    public enum ci : uint
    {
        //LODR_ClampFwBw     = 0  ,
        MAX_ACTR
    };

    // Id
    public enum ax : uint
    {
        ETC_LoadCell1       = 00, //
        ETC_LoadCell2       = 01, //
        ETC_LoadCell3       = 02, //
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
        ETC_StartSw_R       =  00 , //
        ETC_StopSw_R        =  01 , //
        ETC_ResetSw_R       =  02 , //
        ETC_StartSw_L       =  03 , //
        ETC_StopSw_L        =  04 , //
        ETC_ResetSw_L       =  05 , //
        ETC_EmgSw           =  06 , //
        ETC_ServoOn         =  07 , //
        ETC_DoorSw_F        =  08 , //
        ETC_DoorSw_R        =  09 , //
        ETC_LoadCellLOW1    =  10 , //
        ETC_LoadCellOK1     =  11 , //
        ETC_LoadCellHIGH1   =  12 , //
        ETC_LoadCellLOW2    =  13 , //
        ETC_LoadCellOK2     =  14 , //
        ETC_LoadCellHIGH2   =  15 , //
                  
        ETC_LoadCellLOW3    =  16 , //  
        ETC_LoadCellOK3     =  17 , //  
        ETC_LoadCellHIGH3   =  18 , //  
        ETC_19              =  19 , //  
        ETC_20              =  20 , //  
        ETC_21              =  21 , //  
        ETC_22              =  22 , //  
        ETC_23              =  23 , //  
        ETC_24              =  24 , //  
        ETC_25              =  25 , // 
        ETC_26              =  26 , // 
        ETC_27              =  27 , // 
        ETC_28              =  28 , // 
        ETC_29              =  29 , //  
        ETC_30              =  30 , //  
        ETC_31              =  31 , //
        MAX_INPUT
    };

    //그냥 아이오31.  <파트 3자리>+_+<세부설명>
    //복동 실린더 아이오 <파트 3자리>+_+<세부설명>+<해당행동 ex) Fw , Bw , Dn 등등 2자리>     자리
    //단동 실린더 아이오 <파트 3자리>+_+<세부설명>+<Fw시에해당행동2자리><Bw시에해당행동2자리> 동
    public enum yi : uint
    {
        ETC_StartLp_R         = 00, // 
        ETC_StopLp_R          = 01, // 
        ETC_ResetLp_R         = 02, // 
        ETC_StartLp_L         = 03, // 
        ETC_StopLp_L          = 04, // 
        ETC_ResetLp_L         = 05, // 
        ETC_LightOnOff        = 06, // 
        ETC_07                = 07, // 
        ETC_TwRedLp           = 08, // 
        ETC_TwYelLp           = 09, // 
        ETC_TwGrnLp           = 10, // 
        ETC_TwBzz             = 11, //
        ETC_LoadZero1         = 12, //
        ETC_LoadZero2         = 13, //
        ETC_LoadZero3         = 14, //
        ETC_15                = 15, //

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
    public enum pv : uint
    {
        L_UP_ZLiftWait             = 0,
        L_UP_ZLiftWaitG               , //그립용 웨이트 포지션.
        L_UP_ZLiftTchB                , //터치전 상대값 0~
        //L_UP_ZLiftTchH                , //터치시 포지션 설정값 Height
        //L_UP_ZLiftTchW                , //터치시 포지션 설정값 Weight
        L_UP_ZLiftTchA                , //터치시 포지션 측정값 Auto
        MAX_PSTN_MOTR0                ,

        L_DN_ZLiftWait             = 0,
        L_DN_ZLiftTchB                , //터치전 상대값 0~
        //L_DN_ZLiftTchH                , //터치시 포지션 설정값
        //L_DN_ZLiftTchW                , //터치시 포지션 설정값
        L_DN_ZLiftTchA                , //터치시 포지션 측정값
        MAX_PSTN_MOTR1                ,

        R_UP_ZLiftWait             = 0,
        R_UP_ZLiftWaitP               , //풀링용 웨이트 포지션
        //R_UP_ZLiftWaitG               , //그립용 웨이트 포지션
        R_UP_ZLiftTchB                , //터치전 상대값 0~
        //R_UP_ZLiftTchH                , //터치시 포지션 설정값
        //R_UP_ZLiftTchW                , //터치시 포지션 설정값
        R_UP_ZLiftTchA                , //터치시 포지션 측정값
        MAX_PSTN_MOTR2                ,

    };

    //Manual Cycle.
    public enum mc : uint
    {
        NoneCycle                 = 0,
        AllHome                   = 1,
        LEFT_Home                    ,
        LEFT_Wait                    ,
        LEFT_Zero                    ,
        LEFT_Once                    ,
        LEFT_Work                    ,
        LEFT_Rest                    ,

        RIGH_Home                    ,
        RIGH_Wait                    ,
        RIGH_Zero                    ,
        RIGH_Once                    ,
        RIGH_Work                    ,
        RIGH_Rest                    ,

        MAX_MANUAL_CYCLE
    };

    //Error Id
    public enum ei : uint
    {
        ETC_MainAir       =0,
        ETC_Emergency       ,
        ETC_MainPower       ,
        ETC_DoorF           ,
        ETC_DoorR           ,
        ATR_TimeOut         ,
        MTR_Alarm           ,
        MTR_NegLim          ,
        MTR_PosLim          ,
        MTR_HomeEnd         ,
        ETC_LotOpen         ,
        ETC_1               ,
        ETC_ToStartTO       ,
        ETC_ToStopTO        ,
        ETC_AllHomeTO       ,
        ETC_ManCycleTO      ,
        ETC_2               ,
        ETC_LoadCellOver1   ,
        ETC_LoadCellOver2   ,
        ETC_LoadCellOver3   ,
        ETC_LoadCellOk1     ,
        ETC_LoadCellOk2     ,
        ETC_LoadCellOk3     ,
        LEFT_CycleTO        ,
        LEFT_HomeTo         ,
        LEFT_ToStartTO      ,
        LEFT_ToStopTO       ,
        LEFT_ZeroCheckUpFail,
        LEFT_ZeroCheckDnFail,
        ETC_3               ,
        RIGH_CycleTO        ,
        RIGH_HomeTo         ,
        RIGH_ToStartTO      ,
        RIGH_ToStopTO       ,
        RIGH_ZeroCheckFail  ,
        ETC_4               ,
        ETC_UsbDisConnect   ,
        ETC_6               ,
        LEFT_NeedZeroCheck  ,
        RIGH_NeedZeroCheck  ,
        ETC_9               ,
        ETC_A               ,
        ETC_MacroErr        ,
        //RIGH_GripWaitCheck  ,
        LEFT_GripWaitCheck  ,

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





