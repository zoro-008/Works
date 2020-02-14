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

    public delegate void AddMsgDelegate(string _sMsg);
    public delegate void ClearMsgDelegate();
    public delegate void ChangeScript();
    public delegate void SpectroRun(bool _bRun);
    public delegate void SepctroSave();
    public static class Eqp
    {
        public const string sEqpName    = "Rotor System";
        public const string sLanguage   = "English"  ; //Korean , English , Chinese
        
        public static AddMsgDelegate   AddMsg     ;
        public static ClearMsgDelegate ClearMsg   ;  
        public static ChangeScript     ChangeScrt ;  
        public static SpectroRun       SpectroRun ;
        public static SepctroSave      SpectroSave;
    }

    public enum fb : uint
    {
        Bwd    = 0 ,
        Fwd    = 1 , 
    }

    //Part ID
    public enum pi : uint
    {
        MAX_PART = 0 , 
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

        MAX_CHIP_STAT

    };
   


    //mi<파트명3자리>_<축방샹1자리><부가명칭4> Motor Id
    public enum mi : uint
    {   
        RotorT    =  0 ,
        LaserX         ,
        MagnetX        ,
        MAX_MOTR 
    };

    //ai<파트명3자리>_<부가명칭><FWD시에위치2자리><BWD시에위치2자리>
    //대기가 Bwd 작동이 Fwd
    public enum ci : uint
    {
        CoverUD   =  0 ,
        CoverFB   =  1 ,
        HeatrUD   =  2 ,
        MAX_ACTR 
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
        HomeSw          =  2, //
        CoolSw          =  3 , //
        EmgSw           =  4 , //
        CoverUp         =  5 , //
        HeaterUp        =  6 , //
        CoverFw         =  7 , //
        Door1Left       =  8 , //
        Door2Front      =  9 , //
        Door3Right      = 10 , //
        Door4Rear       = 11 , //
        Spare12         = 12 , //
        Spare13         = 13 , //
        Spare14         = 14 , //  
        Spare15         = 15 , //
        MAX_INPUT //Enum.GetValues(typeof(xxxxx)).Length;
    };

    //그냥 아이오31.  <파트 3자리>+_+<세부설명>
    //복동 실린더 아이오 <파트 3자리>+_+<세부설명>+<해당행동 ex) Fw , Bw , Dn 등등 2자리>     자리
    //단동 실린더 아이오 <파트 3자리>+_+<세부설명>+<Fw시에해당행동2자리><Bw시에해당행동2자리> 동
    public enum yi : uint
    {
        StartLamp          =  0, // 
        StopLamp           =  1, // 
        HomeLamp           =  2, // 
        CoolingLamp        =  3, // 
        LightOnOff         =  4, // 
        CoverUp            =  5, // 
        HeaterUp           =  6, // 
        CoverFw            =  7, // 
        LaserOnOff         =  8, // 
        CoverBw            =  9, // 
        SepctroLight       = 10, //  =  0, // 
        Spare11            = 11, // 
        Spare12            = 12, // 
        Spare13            = 13, // 
        Spare14            = 14, // 
        Spare15            = 15, //
        MAX_OUTPUT //Enum.GetValues(typeof(xxxxx)).Length;
    };

    //Position Value id
    public enum pv : uint
    {
        MAX_PSTN_MOTR0 =0 ,
        MAX_PSTN_MOTR1 =0 ,
        MAX_PSTN_MOTR2 =0 ,
        MAX_PSTN_MOTR3 =0 
    };

    //Manual Cycle.
    public enum mc : uint
    {
        NoneCycle                 = 0,
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
        ETC_007             ,
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
        TMP_Com             ,
        CAM_Error           ,
        RUN_Error           , 
        
        MAX_ERR    
    };

    //Sequence Code
    public enum sc : uint
    {
        Run     = 0 ,
        Cool    = 1 ,
        Example = 2 ,
        Test1   = 3 ,
        Test2   = 4 ,

        MAX_SEQUENCE_CODE
    }

}





