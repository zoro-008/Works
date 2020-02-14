using SML;
using System;
using System.Linq;

//TODO :: LIST
//홈잡을때 자재 들고 있으면 물어봐야 함 Z축 부터 잡게 되어 있음

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
        public const string sEqpName  = "Cell Clean System";
        public const string sLan_Kor  = "Korean"   ; //Korean , English , Chinese
        public const string sLan_Eng  = "English"  ; //Korean , English , Chinese 
        public const string sLanguage = sLan_Eng   ; //Korean , English , Chinese
    }
    
    public enum vi : uint //비전아이디.
    {
        Vs1L  = 0 ,
        Vs1R  = 1 ,
        Vs2L  = 2 ,
        Vs2R  = 3 ,
        Vs3L  = 4 ,
        Vs3R  = 5 ,
        MAX_VI 
    }

    public enum fb : uint
    {
        Bwd    = 0 ,
        Fwd    = 1 , 
    }

    //Part ID
    public enum pi : uint
    {
        PRER    , //Pre Rail
        LPCK    , //Left Picker
        RPCK    , //Right Picker
        PSTR    , //Post Rail
        VSTG    , //Vacuum Stage
        MAX_PART,
    }

    //aRay Id
    public class ri
    {
        public const int LODR     = 0  ;  //Loading
        public const int PRER     = 1  ;  //Pre Rail
        public const int LPCK     = 2  ;  //Left Picker
        public const int VSTG     = 3  ;  //Vacuum Stage
        public const int RPCK     = 4  ;  //Right Picker
        public const int PSTR     = 5  ;  //Post Rail
        public const int ULDR     = 6  ;  //Unloading
        public const int MAX_ARAY = 7  ;
    }

    //Chip Status
    public enum cs : int//<sun>화면에 팝업메뉴 손봐야함.
    {
        RetFail = -1 , //함수 리턴 
        None    =  0 , ////스트립이나 카세트가 없는 상태.                                            >
        Exist        , //Pre Rail After Lift Up
        Empty        , ////카세트에 스트립이 없는 상태 혹은 더미패드에 토출자국이 없는경우. 프로브끝 >
        Unknown      , //PRER
        WorkEnd      , //PSTR
        LiftUp       , //PRER
        Cleaning     , //세척 중
        Clean        , //세척 완료 PCKL,PSTR,VSTG
        Pckrclean    , //LPCK, Left Picker 돌아갈때 cleaning
        Pick         , //PCKL, PCKR
        Move         , //PCKR
        Place        , //PCKL, PCKR
        
        MAX_CHIP_STAT

    };



    //mi<파트명3자리>_<축방샹1자리><부가명칭4> Motor Id
    public enum mi : uint
    {//                           홈위치_장비전면 기준
        PRER_X = 0 ,
        PSTR_X = 1 ,
        LPCK_Y = 2 ,
        RPCK_Y = 3 ,
        VSTG_X = 4 ,
      
        MAX_MOTR
    };

    //ai<파트명3자리>_<부가명칭><FWD시에위치2자리><BWD시에위치2자리>
    //대기가 Bwd 작동이 Fwd
    //장비 전면 기준으로 앞에 있는게 Front(Ft), 뒤에가 Rear(Rr)
    public enum ci : uint
    {
        PRER_RollerUpDn    =  0 ,   //Pre Rail Roller Up/Down
        PRER_TrayAlignFwBw =  1 ,   //Pre Rail Tray Align Fwd/Bwd
        PSTR_TrayAlignFwBw =  2 ,   //Post Rail Tray Align Fwd/Bwd
        PRER_PreStprUpDn   =  3 ,   //Pre Rail Pre Stopper Up/Down
        PRER_SttnStprUpDn  =  4 ,   //Pre Rail Station Stopper Up/Down
        PSTR_SttnStprUpDn  =  5 ,   //Post Rail Station Stopper Up/Down
        LPCK_PickerDnUp    =  6 ,   //Left Picker Down/Up
        RPCK_PickerDnUp    =  7 ,   //Right Picker Down/Up
        PRER_SttnClampClOp =  8 ,   //Pre Rail Station Clamp Close/Open
        PSTR_SttnClampClOp =  9 ,   //Post Rail Station Clamp close/Open
        PRER_SttnUpDn      = 10 ,   //Pre Rail Station Up/Down
        PSTR_SttnUpDn      = 11 ,   //Post Rail Station Up/Down
        
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
        ETC_StartSw         =  0 , //Start   
        ETC_StopSw          =  1 , //Stop    
        ETC_ResetSw         =  2 , //Reset   
        ETC_AirSw           =  3 , //Air     
        ETC_InitSw          =  4 , //Initial 
        ETC_EmgLtSw         =  5 , //이머전시 스위치(Left)  
        ETC_EmgFtSw         =  6 , //이머전시 스위치(Front)
        ETC_EmgRtSw         =  7 , //이머전시 스위치(Right)
        ETC_MainPower       =  8 , //서보모터 파워
        ETC_MainAir1        =  9 , //메인 에어1(실린더)
        ETC_DoorLt          = 10 , //Left  Door
        ETC_DoorFt          = 11 , //Front Door  
        ETC_DoorRt          = 12 , //Right Door
        ETC_DoorRr          = 13 , //Rear  Doot
        PRER_TrayDetect1    = 14 , //Pre Rail Tray Detect Sensor1(Rail)
        PRER_TrayDetect2    = 15 , //Pre Rail Tray Detect Sensor2(Station)
                            
        PSTR_TrayDetect1    = 16 , //Post Rail Tray Detect Sensor3(Station)
        PSTR_TrayDetect2    = 17 , //Post Rail Tray Detect Sensor4(Rail)
        PSTR_TrayDetect3    = 18 , //Post Rail Tray Detect Sensor5(Rail)
        PRER_RollerUp       = 19 , //Pre Rail (Rear)  Roller Up
        PRER_RollerDn       = 20 , //Pre Rail (Front) Roller Down
        PRER_TrayAlignFw    = 21 , //Pre Rail Tray Align Fwd
        PRER_TrayAlignBw    = 22 , //Pre Rail Tray Align Bwd
        PSTR_TrayAlignFw    = 23 , //Post Rail Tray Align Fwd
        PSTR_TrayAlignBw    = 24 , //Post Rail Tray Align Bwd
        PRER_PreStprUp      = 25 , //Pre Rail Pre Stopper Up
        PRER_PreStprDn      = 26 , //Pre Rail Pre Stopper Down
        PRER_SttnStprUp     = 27 , //Pre Rail Station Stopper Up
        PRER_SttnStprDn     = 28 , //Pre Rail Station Stopper Down
        PSTR_SttnStprUp     = 29 , //Post Rail Station Stopper Up
        PSTR_SttnStprDn     = 30 , //Post Rail Station Stopper Down
        ETC_031             = 31 , //Spare

        LPCK_PickerUp       = 32 , //Left  Picker Up
        LPCK_PickerDn       = 33 , //Left  Picker Down  
        RPCK_PickerUp       = 34 , //Right Picker Up
        RPCK_PickerDn       = 35 , //Right Picker Down
        LPCK_Vacuum1        = 36 , //Left  Picker Vacuum Sensor1
        LPCK_Vacuum2        = 37 , //Left  Picker Vacuum Sensor2
        LPCK_Vacuum3        = 38 , //Left  Picker Vacuum Sensor3
        LPCK_Vacuum4        = 39 , //Left  Picker Vacuum Sensor4
        LPCK_Vacuum5        = 40 , //Left  Picker Vacuum Sensor5
        LPCK_Vacuum6        = 41 , //Left  Picker Vacuum Sensor6
        RPCK_Vacuum1        = 42 , //Right Picker Vacuum Sensor1
        RPCK_Vacuum2        = 43 , //Right Picker Vacuum Sensor2
        RPCK_Vacuum3        = 44 , //Right Picker Vacuum Sensor3
        RPCK_Vacuum4        = 45 , //Right Picker Vacuum Sensor4
        RPCK_Vacuum5        = 46 , //Right Picker Vacuum Sensor5
        RPCK_Vacuum6        = 47 , //Right Picker Vacuum Sensor6

        VSTG_Vacuum1        = 48 , //Vacuum Stage Vacuum Sensor1
        VSTG_Vacuum2        = 49 , //Vacuum Stage Vacuum Sensor2
        VSTG_Vacuum3        = 50 , //Vacuum Stage Vacuum Sensor3
        VSTG_Vacuum4        = 51 , //Vacuum Stage Vacuum Sensor4
        VSTG_Vacuum5        = 52 , //Vacuum Stage Vacuum Sensor5
        VSTG_Vacuum6        = 53 , //Vacuum Stage Vacuum Sensor6
        PRER_SttnClampCl    = 54 , //Pre   Rail Station Clamp Cylinder Close
        PRER_SttnClampOp    = 55 , //Pre   Rail Station Clamp Cylinder Open
        PSTR_SttnClampCl    = 56 , //Post  Rail Station Clamp Cylinder Close
        PSTR_SttnClampOp    = 57 , //Post  Rail Station Clamp Cylinder Open
        PRER_SttnUp         = 58 , //Pre  Rail Station Up
        PRER_SttnDn         = 59 , //Pre  Rail Station Down
        PSTR_SttnUp         = 60 , //Post Rail Station Up
        PSTR_SttnDn         = 61 , //Post Rail Station Down
        PRER_TrayOutSnsr    = 62 , //Spare
        ETC_063             = 63 , //Spare

        LPCK_IonBlwrBtm     = 64 , //Spare
        LPCK_HighVtgBtm     = 65 , //Spare
        PSTR_IonBlwrBtm     = 66 , //Spare
        PSTR_HighVtgBtm     = 67 , //Spare
        PSTR_IonBlwrTop     = 68 , //Spare
        PSTR_HighVtgTop     = 69 , //Spare
        VSTG_IonBlwrTop     = 70 , //Spare
        VSTG_HighVtgTop     = 71 , //Spare
        ETC_MainAir2        = 72 , //메인에어 2(Vacuum/Ejector)
        ETC_MainAir3        = 73 , //메인에어 3(Air Blower)
        ETC_074             = 74 , //Spare
        ETC_075             = 75 , //Spare
        ETC_076             = 76 , //Spare
        ETC_077             = 77 , //Spare
        ETC_078             = 78 , //Spare
        ETC_079             = 79 , //Spare
                                     
        ETC_080             = 80 , //Spare
        ETC_081             = 81 , //Spare
        ETC_082             = 82 , //Spare
        ETC_083             = 83 , //Spare
        ETC_084             = 84 , //Spare
        ETC_085             = 85 , //Spare
        ETC_086             = 86 , //Spare
        ETC_087             = 87 , //Spare
        ETC_088             = 88 , //Spare
        ETC_089             = 89 , //Spare
        ETC_090             = 90 , //Spare
        ETC_091             = 91 , //Spare
        ETC_092             = 92 , //Spare
        ETC_093             = 93 , //Spare
        ETC_094             = 94 , //Spare
        ETC_095             = 95 , //Spare

        MAX_INPUT
    };

    //그냥 아이오31.  <파트 3자리>+_+<세부설명>
    //복동 실린더 아이오 <파트 3자리>+_+<세부설명>+<해당행동 ex) Fw , Bw , Dn 등등 2자리>     자리
    //단동 실린더 아이오 <파트 3자리>+_+<세부설명>+<Fw시에해당행동2자리><Bw시에해당행동2자리> 동
    public enum yi : uint
    {
        ETC_StartLp         =  0, //Start   Lamp
        ETC_StopLp          =  1, //Stop    Lamp
        ETC_ResetLp         =  2, //Reset   Lamp
        ETC_AirLp           =  3, //Air     Lamp
        ETC_InitLp          =  4, //Initial Lamp
        ETC_MainAirOnOff    =  5, //메인 에어 솔 On/Off
        ETC_LightOnOff      =  6, //Light On/Off
        ETC_007             =  7, //Spare
        ETC_TwRedLp         =  8, //타워램프 (Red   )   
        ETC_TwYelLp         =  9, //타워램프 (Yellow)
        ETC_TwGrnLp         = 10, //타워램프 (Green )
        ETC_TwBzz           = 11, //타워램프 (Buzzer)
        PRER_RollerUpDn     = 12, //Pre  Rail Roller Up/Down
        PRER_PreStprUpDn    = 13, //Pre  Rail Pre Stopper Up/Down
        PRER_SttnStprUpDn   = 14, //Pre  Rail Station Stopper Up/Down
        PSTR_SttnStprUpDn   = 15, //Post Rail Station Stopper Up/Down

        PRER_TrayAlignFwBw  = 16, //Pre  Rail Tray Align Fwd/Bwd
        PSTR_TrayAlignFwBw  = 17, //Post Rail Tray Align Fwd/Bwd
        LPCK_AirBlwrBtm1    = 18, //Left Picker  Air Blower Bottom 1 (Rail과 스테이지 사이 바닥에 붙어있는 Air Blower. 동작을 Picker 파트에서 할 생각이어서 Picker 파트에 넣었음)
        PSTR_AirBlwrBtm1    = 19, //Pre Rail     Air Blower Bottom 1 (Rail 아래쪽에 붙어있는 Air Blower)
        PSTR_AirBlwrTop1    = 20, //Pre Rail     Air Blower Top    1 (Rail 위쪽에 붙어있는 Air Blower)
        VSTG_AirBlwrTop1    = 21, //Vacuum Stage Air Blower Top    1 (스테이지 위쪽에 붙어있는 Air Blower)
        LPCK_AirBlwrBtm2    = 22, //Left Picker  Air Blower Bottom 2 (Rail과 스테이지 사이 바닥에 붙어있는 Air Blower. 동작을 Picker 파트에서 할 생각이어서 Picker 파트에 넣었음)
        PSTR_AirBlwrBtm2    = 23, //Pre Rail     Air Blower Bottom 2 (Rail 아래쪽에 붙어있는 Air Blower)
        PSTR_AirBlwrTop2    = 24, //Pre Rail     Air Blower Top    2 (Rail 위쪽에 붙어있는 Air Blower)
        VSTG_AirBlwrTop2    = 25, //Vacuum Stage Air Blower Top    2 (스테이지 위쪽에 붙어있는 Air Blower)
        PRER_SttnUpDn       = 26, //Pre Rail Station Up/Down
        PSTR_SttnUpDn       = 27, //Post Rail Station Up/Down
        PRER_SttnClampClOp  = 28, //Pre Rail Station Clamp Close/Open
        PSTR_SttnClampClOp  = 29, //Post Rail Station Clamp Close/Open
        LPCK_PickerUp       = 30, //Left Picker UP
        LPCK_PickerDn       = 31, //Left Picker Down

        RPCK_PickerUp       = 32, //Right Picker UP
        RPCK_PickerDn       = 33, //Right Picker Down
        LPCK_Vacuum1        = 34, //Left Picker Vacuum1
        LPCK_Eject1         = 35, //Left Picker Eject1
        LPCK_Vacuum2        = 36, //Left Picker Vacuum2
        LPCK_Eject2         = 37, //Left Picker Eject2
        LPCK_Vacuum3        = 38, //Left Picker Vacuum3
        LPCK_Eject3         = 39, //Left Picker Eject3
        LPCK_Vacuum4        = 40, //Left Picker Vacuum4
        LPCK_Eject4         = 41, //Left Picker Eject4
        LPCK_Vacuum5        = 42, //Left Picker Vacuum5
        LPCK_Eject5         = 43, //Left Picker Eject5
        LPCK_Vacuum6        = 44, //Left Picker Vacuum6
        LPCK_Eject6         = 45, //Left Picker Eject6
        RPCK_Vacuum1        = 46, //Right Picker Vacuum1
        RPCK_Eject1         = 47, //Right Picker Eject1

        RPCK_Vacuum2        = 48, //Right Picker Vacuum2
        RPCK_Eject2         = 49, //Right Picker Eject2
        RPCK_Vacuum3        = 50, //Right Picker Vacuum3
        RPCK_Eject3         = 51, //Right Picker Eject3
        RPCK_Vacuum4        = 52, //Right Picker Vacuum4
        RPCK_Eject4         = 53, //Right Picker Eject4
        RPCK_Vacuum5        = 54, //Right Picker Vacuum5
        RPCK_Eject5         = 55, //Right Picker Eject5
        RPCK_Vacuum6        = 56, //Right Picker Vacuum6
        RPCK_Eject6         = 57, //Right Picker Eject6
        VSTG_Vacuum1        = 58, //Vacuum Stage Vacuum1
        VSTG_Eject1         = 59, //Vacuum Stage Eject1
        VSTG_Vacuum2        = 60, //Vacuum Stage Vacuum2
        VSTG_Eject2         = 61, //Vacuum Stage Eject2
        VSTG_Vacuum3        = 62, //Vacuum Stage Vacuum3
        VSTG_Eject3         = 63, //Vacuum Stage Eject3

        VSTG_Vacuum4        = 64, //Vacuum Stage Vacuum4
        VSTG_Eject4         = 65, //Vacuum Stage Eject4
        VSTG_Vacuum5        = 66, //Vacuum Stage Vacuum5
        VSTG_Eject5         = 67, //Vacuum Stage Eject5
        VSTG_Vacuum6        = 68, //Vacuum Stage Vacuum6   
        VSTG_Eject6         = 69, //Vacuum Stage Eject6
        LPCK_IonBlwrBtm     = 70, //Left Picker  Ion Blower Bottom (Rail과 스테이지 사이 바닥에 붙어있는 이오나이저. 동작을 Picker 파트에서 할 생각이어서 Picker 파트에 넣었음)
        PSTR_IonBlwrBtm     = 71, //Pre Rail     Ion Blower Bottom (Rail 아래쪽에 붙어있는 이오나이저)
        PSTR_IonBlwrTop     = 72, //Pre Rail     Ion Blower Top    (Rail 위쪽에 붙어있는 이오나이저)
        VSTG_IonBlwrTop     = 73, //Vacuum Stage Ion Blower Top    (스테이지 위쪽에 붙어있는 이오나이저)
        ETC_074             = 74, //Spare
        ETC_075             = 75, //Spare
        ETC_076             = 76, //Spare
        ETC_077             = 77, //Spare
        ETC_078             = 78, //Spare
        ETC_079             = 79, //Spare 
        
        ETC_080             = 80, //Spare
        ETC_081             = 81, //Spare
        ETC_082             = 82, //Spare
        ETC_083             = 83, //Spare
        ETC_084             = 84, //Spare
        ETC_085             = 85, //Spare
        ETC_086             = 86, //Spare
        ETC_087             = 87, //Spare
        ETC_088             = 88, //Spare
        ETC_089             = 89, //Spare
        ETC_090             = 90, //Spare
        ETC_091             = 91, //Spare
        ETC_092             = 92, //Spare
        ETC_093             = 93, //Spare
        ETC_094             = 94, //Spare 
        ETC_095             = 95, //Spare 

        MAX_OUTPUT
    };

    //Position Value id
    public enum pv : uint
    {
        PRER_XWait           = 0 ,
        MAX_PSTN_MOTR0           , //PRER_X
        
        PSTR_XWait           = 0 ,
        MAX_PSTN_MOTR1           , //PSTR_X
        
        LPCK_YWait           = 0 ,
        LPCK_YPick               ,
        LPCK_YCleanStt           ,
        LPCK_YCleanEnd           ,
        LPCK_YPlace              ,
        MAX_PSTN_MOTR2           , //PCKL_Y
        
        RPCK_YWait           = 0 ,
        RPCK_YPick               ,
        RPCK_YPlace              ,
        MAX_PSTN_MOTR3           , //PCKR_Y
        
        VSTG_XWait           = 0 ,
        VSTG_XWorkStt            ,
        VSTG_XCleanStt           ,
        VSTG_XCleanEnd           ,
        VSTG_XWorkEnd            ,
        MAX_PSTN_MOTR4           , //VSTG_X
    };

    //Manual Cycle.
    public enum mc : uint
    {
        NoneCycle             = 0  ,
        AllHome               = 1  ,
        PRER_Home                  ,//2
        PSTR_Home                  ,//3
        LPCK_Home                  ,//4
        RPCK_Home                  ,//5
        VSTG_Home                  ,//6

        PRER_Reload           = 10 , //
        PRER_Supply           = 11 , //
        PRER_LiftUp           = 12 , //
        PRER_LiftDown         = 13 , //
        

        PSTR_Clear            = 20 ,
        PSTR_Clean                 , // 
        PSTR_LiftUp                , // 21
        PSTR_LiftDown              , // 22
        PSTR_Out                   , // 23

        LPCK_Pick             = 30 , // 
        LPCK_Clean                 , // 31
        LPCK_Place                 , // 32
        LPCK_PckrClean             ,
                              
        RPCK_Pick             = 40 , // 
        RPCK_Move                  , // 
        RPCK_Place                 , // 

        VSTG_Ready            = 50 ,
        VSTG_Clean                 ,

        PRER_CleanerOnOff     = 60 ,
        LPCK_CleanerOnOff     = 61 ,
        VSTG_CleanerOnOff     = 62 ,
        LPCK_VacuumOnOff      = 63 ,
        LPCK_EjectOnOff       = 64 ,
        RPCK_VacuumOnOff      = 65 ,
        RPCK_EjectOnOff       = 66 ,
        VSTG_VacuumOnOff      = 67 ,
        VSTG_EjectOnOff       = 68 ,
        RAIL_RailRun          = 69 ,//Pre Rail, Post Rail 모터 동시에 굴려주는 매뉴얼 사이클
        



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
        ETC_HomeTO          ,
        ETC_ManCycleTO      ,
        ETC_CycleTO         ,
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
        ETC_030             ,
        ETC_NotReady        ,
        ETC_032             ,
        PCK_PickMiss        , //Picker Pick Miss
        VSTG_VacMiss        , //Vacuum Stage Vacuum Miss
        PRT_CleanerOnFail   , //Ionizer On Fail
        PRT_TrayFull        , //Post Rail 배출단 트레이 있음
        ETC_037             ,
        ETC_038             ,
        ETC_039             ,
        ETC_040             , 
        ETC_041             ,
        ETC_042             ,
        ETCL043             , 
        ETC_044             , 
        ETC_045             , 
        ETC_046             ,
        ETC_047             ,
        ETC_048             ,
        ETC_049             ,
        ETC_050             ,
        ETC_051             ,
        ETC_052             ,                                                                           
        ETC_053             ,
        ETC_054             ,
        LSC_ComErr          ,
        MAX_ERR    
    };

    //Error Id

}





