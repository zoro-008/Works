using SML;
using System;
using System.Linq;

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
        public const string sEqpName  = "Tube Seasoning";
        public const string sLanguage = "English"  ; //Korean , English , Chinese
        public const bool   bIgnrCam  = false;
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
        PCON     ,
        MAX_PART , 
    }

    //aRay Id
    public class ri
    {
        public const int MAX_ARAY  = 0;
    }
    
    //Chip Status
    public enum cs : int//<sun>화면에 팝업메뉴 손봐야함.
    {
        RetFail   = -1, //함수 리턴 
        None      =  0, //All
        Unknown       , //검사전
        Vision        , 
        Empty         , //포켓이 실제로 비어서 자제를 채워 넣어야 하는경우.
        NGVisn        , //비전 페일
        NGMark        , //마킹 페일
        NG2           , //
        NG3           , //
        NG4           , //
        NG5           ,
        NG6           ,
        NG7           ,
        NG8           ,
        NG9           ,
        NG10          , //userDefine
        Good          , //여기까지 검사 결과
        Barcode       , //바코드 붙여야 되는 상태.
        Work          , //작업중일때.
        WorkEnd       , //바코드 붙이고 밴딩기 태워야 함.
        BarRead       , //바코드 읽는거 에러일경우
        MAX_CHIP_STAT

    };
   


    //mi<파트명3자리>_<축방샹1자리><부가명칭4> Motor Id
    public enum mi : uint
    {                   //                           홈위치_장비전면 기준
        MAX_MOTR //Enum.GetValues(typeof(xxxxx)).Length;
    };

    //ai<파트명3자리>_<부가명칭><FWD시에위치2자리><BWD시에위치2자리>
    //대기가 Bwd 작동이 Fwd
    public enum ci : uint
    {
        MAX_ACTR //Enum.GetValues(typeof(xxxxx)).Length;
    };
    public enum ax : uint
    {
    }
    public enum ay : uint
    {
    }
    
    
    //X Id
    //그냥 아이오.  <파트 3자리>+_+<세부설명>
    //실린더 아이오 <파트 3자리>+_+<세부설명>+<해당행동2자리 ex) Fw,Bw,Up,Dn>
    //Ft - Front , Rr - Rear 으로 통일 함.
    public enum xi : uint
    {
        StartSw         =  0 , //
        StopSw          =  1 , //
        HoldSw          =  2 , //
        ResetSw         =  3 , //  
        ManualSw        =  4 , //  
        AutoSw          =  5 , //  
        EmgSw1          =  6 , //  
        EmgSw2          =  7 , //
        Spare8          =  8 , //  
        PSAnode         =  9 , //  
        PSGate          = 10 , //그냥 메뉴얼로 뿌리는 버튼1.
        PSFocus         = 11 , //그냥 메뉴얼로 뿌리는 버튼2.  
        GatePower       = 12 , //그냥 메뉴얼로 뿌리는 버튼3.
        GateGND         = 13 , //그냥 메뉴얼로 뿌리는 버튼4.
        FocusPower      = 14 , //그냥 메뉴얼로 뿌리는 버튼5.  
        FocusGND        = 15 , //

        CathodeSWT      = 16 , //  
        CathodeGND      = 17 , //  
        Door1= 18 , //Left  
        Door2= 19 , //Right
        ShieldDoorClose = 20 , //  
        ShieldDoorOpen  = 21 , //  
        Spare22         = 22 , //  
        Spare23         = 23 , //  
        Spare24         = 24 , //  
        Spare25         = 25 , // 
        Spare26         = 26 , // 
        Spare27         = 27 , // 
        Spare28         = 28 , // 
        Spare29         = 29 , //  
        Spare30         = 30 , //
        Spare31         = 31 , //

        MAX_INPUT //Enum.GetValues(typeof(xxxxx)).Length;
    };

    //그냥 아이오31.  <파트 3자리>+_+<세부설명>
    //복동 실린더 아이오 <파트 3자리>+_+<세부설명>+<해당행동 ex) Fw , Bw , Dn 등등 2자리>     자리
    //단동 실린더 아이오 <파트 3자리>+_+<세부설명>+<Fw시에해당행동2자리><Bw시에해당행동2자리> 동
    public enum yi : uint
    {
        StartLamp          =  0, // 
        StopLamp           =  1, // 
        HoldLamp           =  2, // 
        ResetLamp          =  3, // 
        TowerLampR         =  4, // 
        TowerLampY         =  5, // 
        TowerLampG         =  6, // 
        BuzzerVoice        =  7, // 
        GatePower          =  8, // 
        GateGND            =  9, // 
        FocusPower         = 10, // 
        FocusGND           = 11, // 
        CathodeSWT         = 12, // 
        CathodeGND         = 13, // 
        ShieldUnLock       = 14, // 
        PowerAuto          = 15, //

        Spare16            = 16, // 
        Spare17            = 17, // 
        Spare18            = 18, // 
        Spare19            = 19, // 
        Spare20            = 20, // 
        Spare21            = 21, // 
        Spare22            = 22, // 
        Spare23            = 23, // 
        Spare24            = 24, // 
        Spare25            = 25, // 
        Spare26            = 26, // 
        Spare27            = 27, // 
        Spare28            = 28, // 
        Spare29            = 29, // 
        Spare30            = 30, // 
        Spare31            = 31, // 

        MAX_OUTPUT //Enum.GetValues(typeof(xxxxx)).Length;
    };

    //Position Value id
    public enum pv : uint
    {
        MAX_PSTN_MOTR0 ,
    };

    //Manual Cycle.
    public enum mc : uint
    {
        NoneCycle                 = 0,
        AllHome                   = 1,
        LODR_Home                    ,
        TTBL_Home                    ,
        VISN_Home                    ,
        MARK_Home                    ,
        ULDR_Home                    ,
        REJM_Home                    ,
        REJV_Home                    ,

        LODR_CycleHold          = 20 , 
        LODR_CyclePush               ,
        LODR_CyclePick               ,
        LODR_ManLotSupply            ,

        TTBL_CycleMove          = 30 ,
        TTBL_CLAllFwd                ,
        TTBL_CLAllBwd                ,

        VISN_CycleWork          = 40 ,
        
        MARK_CycleWork          = 50 ,
        MARK_CycleManChage           ,

        ULDR_CycleMove          = 60 ,
        ULDR_CycleDlvr               ,
        ULDR_CyclePick               ,
        ULDR_CyclePlce               ,
        ULDR_CyclePaint              ,

        RJEM_CycleWork          = 70 ,

        RJEV_CycleWork          = 80 ,

        MAX_MANUAL_CYCLE
    };

    //Error Id
    public enum ei : uint
    {
        ETC_000           =0,
        ETC_Emergency       ,
        ETC_Door            ,
        ETC_ShieldDoor      ,
        ETC_LotOpen         ,
        ETC_ToStartTO       ,
        ETC_ToStopTO        ,
        ETC_AllHomeTO       ,
        ETC_ManCycleTO      ,
        AGG_CheckRecipe     ,
        AGG_CathodTrans     ,
        AGG_GateTrans       ,
        AGG_FocusTrans      ,
        AGG_CathodLimit     ,
        ETC_Daegyum         ,
        ETC_Manual          ,
        ETC_CvtCom          ,
        ETC_DgCom           ,

        MAX_ERR //Enum.GetValues(typeof(xxxxx)).Length;
    };

    //Error Id

}





