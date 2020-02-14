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
        public const string sEqpName  = "HVI-1500i";
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
        LODR    , //Loader
        PREB    , //PreBuffer
        VSNZ    , //Vision Zone
        PSTB    , //PostBuffer
        ULDR    , //Unloader
        MAX_PART,
    }

    //aRay Id
    public class ri
    {
        public const int LODR     = 0  ;  //로더  데이터.
        public const int PREB     = 1  ;  //레일 프리버퍼.
        public const int WRK1     = 2  ;  //워크존1 합산결과값.
        public const int VSN1     = 3  ;  //워크존1 비젼프로세스.
        public const int RLT1     = 4  ;  //워크존1 에서만 결과값.   
        public const int WRK2     = 5  ;  //워크존2 합산결과값.
        public const int VSN2     = 6  ;  //워크존2 비젼프로세스.
        public const int RLT2     = 7  ;  //워크존2 에서만 결과값.   
        public const int WRK3     = 8  ;  //워크존3 합산결과값.
        public const int VSN3     = 9  ;  //워크존3 비젼프로세스.
        public const int RLT3     = 10 ;  //워크존3 에서만 결과값.   
        public const int PSTB     = 11 ;  //포스트 버퍼 & 마킹.
        public const int ULDR     = 12 ;  //언로더 .

        public const int SPC      = 13 ;  //랏엔드시에 쓰임. 포스트 버퍼에서 작업다 끝나고 카운팅 하려고 PSTB에 들어오면
                                          //여기에 저장했다가 언로더에 넣을때 이걸로 불량 카운팅 및 SPC에 남김.
        public const int Buf      = 14 ;  //아오 비전 더블클릭해서 보는거 이거 데이터 덮어 씌울라고 ... dd
        public const int MAX_ARAY = 15 ;
    }

    //Chip Status
    public enum cs : int//<sun>화면에 팝업메뉴 손봐야함.
    {
        RetFail = -1 , //함수 리턴 
        None    =  0 , ////스트립이나 카세트가 없는 상태.                                            >
        Mask         , ////PKG가 없지만 채크를 해놓은 상태.                                          >  요거 세개가 PKG 없는거.
        Empty        , ////카세트에 스트립이 없는 상태 혹은 더미패드에 토출자국이 없는경우. 프로브끝 >
        Unknown      , ////작업전 있는지 없는지 모르는 경우.
        Work         , //// 작업 끝.
        Wait         , //
        Fail         , //
        Error        , //
        Good         , // 8
        //Etc          , //
        //ToBuf        , // To Buffer       맨끝에서 두개 빼서 spAll 일때만 Visible = true
        //FromBuf      , // Buffer To Here  맨끝에서 두개 빼서 spAll 일때만 Visible = true
        Rslt0        , //9 비전에서 결과값 굿으로 쓰고 있고 핸들러에서는 Good을 쓰고 이놈은 안씀.
        Rslt1        , //10 EMPTY
        Rslt2        , //11 MATCHING
        Rslt3        , //12 CHIP
        Rslt4        , //13 DUST
        Rslt5        , //14 LFLOW
        Rslt6        , //15 BROKEN
        Rslt7        , //16 BUBBLE
        Rslt8        , //17 ZENER
        Rslt9        , //18 WIRE
        RsltA        , //19 PWIRE
        RsltB        , //20 SWIRE
        RsltC        , //21 FLOW
        RsltD        , //22 MARKING
        RsltE        , //23
        RsltF        , //24
        RsltG        , //25
        RsltH        , //26
        RsltI        , //27
        RsltJ        , //28
        RsltK        , //29
        RsltL        , //30
        MAX_CHIP_STAT

    };



    //mi<파트명3자리>_<축방샹1자리><부가명칭4> Motor Id
    public enum mi : uint
    {//                           홈위치_장비전면 기준
        HEAD_XVisn = 0 ,
        HEAD_YVisn = 1 ,
        PSTB_XMark = 2 ,
        PSTB_YMark = 3 ,
        HEAD_XCvr1 = 4 ,
        HEAD_XCvr2 = 5 ,
        HEAD_XCvr3 = 6 ,
        LODR_YClmp = 7 ,
        LODR_ZClmp = 8 ,
        ULDR_YClmp = 9 ,  
        ULDR_ZClmp = 10,
        MAX_MOTR
    };

    //ai<파트명3자리>_<부가명칭><FWD시에위치2자리><BWD시에위치2자리>
    //대기가 Bwd 작동이 Fwd
    //장비 전면 기준으로 앞에 있는게 Front(Ft), 뒤에가 Rear(Rr)
    public enum ci : uint
    {
        RAIL_Vsn1AlignFwBw  =  0 ,   //1번 Vision Align Fwd/Bwd
        RAIL_Vsn2AlignFwBw  =  1 ,   //2번 Vision Align Fwd/Bwd
        RAIL_Vsn3AlignFwBw  =  2 ,   //3번 Vision Align Fwd/Bwd
        PSTB_MarkAlignFWBw  =  3 ,   //Pen Marking Align Fwd/Bwd
        PSTB_PusherFwBw     =  4 ,   //PostBuffer Pusher Fwd/Bwd
        PSTB_MarkingPenUpDn =  5 ,   //Marking Pen Up/Down
        PREB_StprUpDn       =  6 ,   //PreBuffer Stopper Up/Down
        RAIL_Vsn1StprUpDn   =  7 ,   //1번 Vision Stoppper Up/Down
        RAIL_Vsn2StprUpDn   =  8 ,   //2번 Vision Stoppper Up/Down
        RAIL_Vsn3StprUpDn   =  9 ,   //3번 Vision Stoppper Up/Down
        PSTB_MarkStprUpDn   = 10 ,   //Pen Marking Stoppper Up/Down
        RAIL_Vsn1SttnUpDn   = 11 ,   //1번 Vision Station Up/Down
        RAIL_Vsn2SttnUpDn   = 12 ,   //2번 Vision Station Up/Down
        RAIL_Vsn3SttnUpDn   = 13 ,   //3번 Vision Station Up/Down
        PSTB_MarkSttnUpDn   = 14 ,   //Pen Marking Station Up/Down
        LODR_PusherFwBw     = 15 ,   //로더 Pusher Fwd/Bwd
        LODR_ClampUpDn      = 16 ,   //로더 Clamp Up/Down
        ULDR_ClampUpDn      = 17 ,   //언로더 Clamp Up/Down
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
        ETC_LStartSw        =  0 , //Start   (왼쪽  )
        ETC_LStopSw         =  1 , //Stop    (왼쪽  )
        ETC_LResetSw        =  2 , //Reset   (왼쪽  )
        ETC_LAirSw          =  3 , //Air     (왼쪽  )
        ETC_LInitSw         =  4 , //Initial (왼쪽  )
        ETC_LVisnSw         =  5 , //Vision  (왼쪽  )
        ETC_RStartSw        =  6 , //Start   (오른쪽)
        ETC_RStopSw         =  7 , //Stop    (오른쪽)
        ETC_RResetSw        =  8 , //Reset   (오른쪽)
        ETC_RAirSw          =  9 , //Air     (오른쪽)
        ETC_RInitSw         = 10 , //Initial (오른쪽)
        ETC_RVisnSw         = 11 , //Vision  (오른쪽)  
        ETC_012             = 12 , //Spare
        ETC_013             = 13 , //Spare
        ETC_EmgSw2          = 14 , //전면 이머전시 스위치(왼쪽)
        ETC_EmgSw3          = 15 , //전면 이머전시 스위치(오른쪽)
                            
        ETC_016             = 16 , //Spare
        ETC_MainPower       = 17 , //Servo Power  
        ETC_MainAir         = 18 , //Main Air
        ETC_FDoor           = 19 , //Front Door
        ETC_RDoor           = 20 , //Rear Door
        ETC_021             = 21 , //Spare
        ETC_022             = 22 , //Spare
        ETC_023             = 23 , //Spare
        PREB_PkgInDetect    = 24 , //진입단 자재 감지 센서
        RAIL_Vsn1AlignFwBw  = 25 , //1번 Vision Align Pusher Fwd/Bwd
        RAIL_Vsn2AlignFwBw  = 26 , //2번 Vision Align Pusher Fwd/Bwd
        RAIL_Vsn3AlignFwBw  = 27 , //3번 Vision Align Pusher Fwd/Bwd
        RAIL_MarkAlignFWBw  = 28 , //펜마킹 Align Pusher Fwd/Bwd
        ETC_029             = 29 , //Spare
        ETC_030             = 30 , //Spare
        ETC_031             = 31 , //Spare

        PSTB_PusherFw       = 32 , //PostBuffer 자재 Out Pusher Fwd
        PSTB_PusherBw       = 33 , //PostBuffer 자재 Out Pusher Bwd  
        PSTB_PkgDetect1     = 34 , //PostBuffer 자재 감지 센서1
        PSTB_PkgDetect2     = 35 , //PostBuffer 자재 감지 센서2
        ETC_036             = 36 , //Spare
        ETC_037             = 37 , //Spare
        PSTB_MarkingPenUp   = 38 , //Marking Pen Up
        PSTB_MarkingPenDn   = 39 , //Marking Pen Down
        PREB_StprUp         = 40 , //PreBuffer Stopper Up
        PREB_StprDn         = 41 , //PreBuffer Stopper Down
        PREB_StrpDetect     = 42 , //PreBuffer Stopper Strip Detect Sensor
        RAIL_Vsn1StprUp     = 43 , //1번 Vision Stopper Up
        RAIL_Vsn1StprDn     = 44 , //1번 Vision Stopper Down
        RAIL_Vsn1Detect     = 45 , //1번 Vision Stopper Strip Detect Sensor
        RAIL_Vsn2StprUp     = 46 , //2번 Vision Stopper Up
        RAIL_Vsn2StprDn     = 47 , //2번 Vision Stopper Down

        RAIL_Vsn2Detect     = 48 , //2번 Vision Stopper Strip Detect Sensor
        RAIL_Vsn3StprUp     = 49 , //3번 Vision Stopper Up
        RAIL_Vsn3StprDn     = 50 , //3번 Vision Stopper Down
        RAIL_Vsn3Detect     = 51 , //3번 Vision Stopper Strip Detect Sensor
        PSTB_MarkStprUp     = 52 , //Marking Pen Stopper Up
        PSTB_MarkStprDn     = 53 , //Marking Pen Stopper Down
        PSTB_MarkDetect     = 54 , //Marking Pen Stopper Strip Detect Sensor
        ETC_055             = 55 , //Spare
        RAIL_Vsn1SttnUp     = 56 , //1번 Vision Station Up
        RAIL_Vsn1SttnDn     = 57 , //1번 Vision Station Down
        RAIL_Vsn2SttnUp     = 58 , //2번 Vision Station Up
        RAIL_Vsn2SttnDn     = 59 , //2번 Vision Station Down
        RAIL_Vsn3SttnUp     = 60 , //3번 Vision Station Up
        RAIL_Vsn3SttnDn     = 61 , //3번 Vision Station Down
        PSTB_MarkSttnUp     = 62 , //Pen Marking Station Up
        PSTB_MarkSttnDn     = 63 , //Pen Marking Station Down

        HEAD_Vsn1_LReady    = 64 , //1_1번 Vision
        HEAD_Vsn1_LBusy     = 65 , //1_1번 Vision
        HEAD_Vsn1_LEnd      = 66 , //1_1번 Vision
        HEAD_Vsn1_LSpare1   = 67 , //1_1번 Vision
        HEAD_Vsn1_LSpare2   = 68 , //1_1번 Vision
        HEAD_Vsn1_RReady    = 69 , //1_2번 Vision
        HEAD_Vsn1_RBusy     = 70 , //1_2번 Vision
        HEAD_Vsn1_REnd      = 71 , //1_2번 Vision
        HEAD_Vsn1_RSpare1   = 72 , //1_2번 Vision
        HEAD_Vsn1_RSpare2   = 73 , //1_2번 Vision
        HEAD_Vsn2_LReady    = 74 , //2_1번 Vision
        HEAD_Vsn2_LBusy     = 75 , //2_1번 Vision
        HEAD_Vsn2_LEnd      = 76 , //2_1번 Vision
        HEAD_Vsn2_LSpare1   = 77 , //2_1번 Vision
        HEAD_Vsn2_LSpare2   = 78 , //2_1번 Vision
        HEAD_Vsn2_RReady    = 79 , //2_2번 Vision
                                     
        HEAD_Vsn2_RBusy     = 80 , //2_2번 Vision
        HEAD_Vsn2_REnd      = 81 , //2_2번 Vision
        HEAD_Vsn2_RSpare1   = 82 , //2_2번 Vision
        HEAD_Vsn2_RSpare2   = 83 , //2_2번 Vision
        HEAD_Vsn3_LReady    = 84 , //3_1번 Vision
        HEAD_Vsn3_LBusy     = 85 , //3_1번 Vision
        HEAD_Vsn3_LEnd      = 86 , //3_1번 Vision
        HEAD_Vsn3_LSpare1   = 87 , //3_1번 Vision
        HEAD_Vsn3_LSpare2   = 88 , //3_1번 Vision
        HEAD_Vsn3_RReady    = 89 , //3_2번 Vision
        HEAD_Vsn3_RBusy     = 90 , //3_2번 Vision
        HEAD_Vsn3_REnd      = 91 , //3_2번 Vision
        HEAD_Vsn3_RSpare1   = 92 , //3_2번 Vision
        HEAD_Vsn3_RSpare2   = 93 , //3_2번 Vision
        ETC_094             = 94 , //Spare
        ETC_095             = 95 , //Spare

        LODR_PushOverload    = 96 , //로더 Pusher Overload
        LODR_PusherFw        = 97 , //로더 Pusher Fwd
        LODR_PusherBw        = 98 , //로더 Pusher Bwd
        LODR_MgzDetect1      = 99 , //로더 Magazine Detect 1
        LODR_MgzDetect2      = 100, //로더 Magazine Detect 2
        LODR_ClampUp         = 101, //로더 Clamp Up
        LODR_ClampDn         = 102, //로더 Clamp Down
        ETC_103              = 103, //Spare
        LODR_MgzIn           = 104, //로더 투입(상) 컨베이어 매거진 감지 센서
        LODR_MgzOutFull      = 105, //로더 배출(하) 컨베이어 매거진 Full 감지 센서
        ETC_106              = 106, //Spare
        ETC_107              = 107, //Spare
        ETC_108              = 108, //Spare
        ETC_109              = 109, //Spare
        LODR_EmgSw1          = 110, //Loader 이머전시 스위치
        ETC_111              = 111, //Spare
                            
        ULDR_MgzDetect1      = 112, //언로더 Magazine Detect 1
        ULDR_MgzDetect2      = 113, //언로더 Magazine Detect 2
        ULDR_ClampUp         = 114, //언로더 Clamp Up
        ULDR_ClampDn         = 115, //언로더 Clamp Down
        ETC_116              = 116, //Spare
        ULDR_MgzOutFull      = 117, //언로더 배출(상) 컨베이어 매거진 Full 감지 센서
        ULDR_MgzIn           = 118, //언로더 투입(하) 컨베이어 매거진 감지 센서
        ETC_119              = 119, //Spare
        ETC_120              = 120, //Spare
        ETC_121              = 121, //Spare
        ULDR_EmgSw4          = 122, //Spare
        ETC_123              = 123, //Spare
        ETC_124              = 124, //Spare
        ETC_125              = 125, //Spare
        ETC_126              = 126, //Spare
        ETC_127              = 127, //Spare

        MAX_INPUT
    };

    //그냥 아이오31.  <파트 3자리>+_+<세부설명>
    //복동 실린더 아이오 <파트 3자리>+_+<세부설명>+<해당행동 ex) Fw , Bw , Dn 등등 2자리>     자리
    //단동 실린더 아이오 <파트 3자리>+_+<세부설명>+<Fw시에해당행동2자리><Bw시에해당행동2자리> 동
    public enum yi : uint
    {
        ETC_LStartLp         =  0, //Start   Lamp (왼쪽)
        ETC_LStopLp          =  1, //Stop    Lamp (왼쪽)
        ETC_LResetLp         =  2, //Reset   Lamp (왼쪽)
        ETC_LAirLp           =  3, //Air     Lamp (왼쪽)
        ETC_LInitLp          =  4, //Initial Lamp (왼쪽)
        ETC_LVisnLp          =  5, //Vision  Lamp (왼쪽)
        ETC_RStartLp         =  6, //Start   Lamp (오른쪽)
        ETC_RStopLp          =  7, //Stop    Lamp (오른쪽)
        ETC_RResetLp         =  8, //Reset   Lamp (오른쪽)   
        ETC_RAirLp           =  9, //Air     Lamp (오른쪽)
        ETC_RInitLp          = 10, //Initial Lamp (오른쪽)
        ETC_RVisnLp          = 11, //Vision  Lamp (오른쪽)
        ETC_TwRedLp          = 12, //타워램프 (Red   )
        ETC_TwYelLp          = 13, //타워램프 (Yellow)
        ETC_TwGrnLp          = 14, //타워램프 (Green )
        ETC_TwBzz            = 15, //타워램프 (Buzzer)

        ETC_MainAirOnOff     = 16, //메인 에어 솔 On/Off
        ETC_Light            = 17, //Light On/Off
        RAIL_FeedingAC1      = 18, //Rail Feeding Ac 모터 구동(PreBuffer )
        RAIL_FeedingAC2      = 19, //Rail Feeding Ac 모터 구동(Work      )
        RAIL_FeedingAC3      = 20, //Rail Feeding Ac 모터 구동(PostBuffer)
        ETC_021              = 21, //Spare
        ETC_022              = 22, //Spare
        ETC_023              = 23, //Spare
        PREB_AirBlower       = 24, //PreBuffer Air Blower
        RAIL_Vsn1AlignFwBw   = 25, //1번 Vision Align Pusher Fwd/Bwd
        RAIL_Vsn2AlignFwBw   = 26, //2번 Vision Align Pusher Fwd/Bwd
        RAIL_Vsn3AlignFwBw   = 27, //3번 Vision Align Pusher Fwd/Bwd
        PSTB_MarkAlignFWBw   = 28, //펜마킹 Align Pusher Fwd/Bwd
        PSTB_PusherFwBw      = 29, //PostBuffer Pusher Fwd/Bwd
        ETC_030              = 30, //Spare
        ETC_031              = 31, //Spare

        PREB_StprUpDn        = 32, //PreBuffer Stopper Up/Down
        RAIL_Vsn1StprUpDn    = 33, //1번 Vision Stoppper Up/Down
        RAIL_Vsn2StprUpDn    = 34, //2번 Vision Stoppper Up/Down
        RAIL_Vsn3StprUpDn    = 35, //3번 Vision Stoppper Up/Down
        PSTB_MarkStprUpDn    = 36, //Pen Marking Stoppper Up/Down
        PSTB_MarkingPenDnUp  = 37, //Marking Pen Up/Down
        ETC_038              = 38, //Spare
        ETC_039              = 39, //Spare
        RAIL_Vsn1SttnUp      = 40, //1번 Vision Station Up
        RAIL_Vsn1SttnDn      = 41, //1번 Vision Station Down
        RAIL_Vsn2SttnUp      = 42, //2번 Vision Station Up
        RAIL_Vsn2SttnDn      = 43, //2번 Vision Station Down
        RAIL_Vsn3SttnUp      = 44, //3번 Vision Station Up
        RAIL_Vsn3SttnDn      = 45, //3번 Vision Station Down
        RAIL_MarkSttnUp      = 46, //Pen Marking Station Up
        RAIL_MarkSttnDn      = 47, //Pen Marking Station Down

        ETC_048              = 48, //Spare
        ETC_049              = 49, //Spare
        ETC_050              = 50, //Spare
        ETC_051              = 51, //Spare
        ETC_052              = 52, //Spare
        ETC_053              = 53, //Spare
        ETC_054              = 54, //Spare
        ETC_055              = 55, //Spare
        ETC_056              = 56, //Spare
        ETC_057              = 57, //Spare
        ETC_058              = 58, //Spare
        ETC_059              = 59, //Spare
        ETC_060              = 60, //Spare
        ETC_061              = 61, //Spare
        ETC_062              = 62, //Spare
        ETC_063              = 63, //Spare

        HEAD_Vsn1_LESReady   = 64, //1_1번 Vision
        HEAD_Vsn1_LJobChange = 65, //1_1번 Vision
        HEAD_Vsn1_LReset     = 66, //1_1번 Vision
        HEAD_Vsn1_LLotStart  = 67, //1_1번 Vision
        HEAD_Vsn1_LSpare     = 68, //1_1번 Vision   

        HEAD_Vsn1_RESReady   = 69, //1_2번 Vision
        HEAD_Vsn1_RJobChange = 70, //1_2번 Vision
        HEAD_Vsn1_RReset     = 71, //1_2번 Vision
        HEAD_Vsn1_RLotStart  = 72, //1_2번 Vision
        HEAD_Vsn1_RSpare     = 73, //1_2번 Vision
        
        
        HEAD_Vsn2_LESReady   = 74, //2_1번 Vision
        HEAD_Vsn2_LJobChange = 75, //2_1번 Vision
        HEAD_Vsn2_LReset     = 76, //2_1번 Vision
        HEAD_Vsn2_LLotStart  = 77, //2_1번 Vision
        HEAD_Vsn2_LSpare     = 78, //2_1번 Vision

        HEAD_Vsn2_RESReady   = 79, //2_2번 Vision 
        HEAD_Vsn2_RJobChange = 80, //2_2번 Vision 
        HEAD_Vsn2_RReset     = 81, //2_2번 Vision 
        HEAD_Vsn2_RLotStart  = 82, //2_2번 Vision
        HEAD_Vsn2_RSpare     = 83, //2_2번 Vision
        
        HEAD_Vsn3_LESReady   = 84, //3_1번 Vision 
        HEAD_Vsn3_LJobChange = 85, //3_1번 Vision 
        HEAD_Vsn3_LReset     = 86, //3_1번 Vision 
        HEAD_Vsn3_LLotStart  = 87, //3_1번 Vision 
        HEAD_Vsn3_LSpare     = 88, //3_1번 Vision 

        HEAD_Vsn3_RESReady   = 89, //3_2번 Vision 
        HEAD_Vsn3_RJobChange = 90, //3_2번 Vision 
        HEAD_Vsn3_RReset     = 91, //3_2번 Vision 
        HEAD_Vsn3_RLotStart  = 92, //3_2번 Vision 
        HEAD_Vsn3_RSpare     = 93, //3_2번 Vision 

        ETC_094              = 94, //Spare 
        ETC_095              = 95, //Spare 

        LODR_MgzInAC         = 96 , //매거진 진입부 컨베이어
        LODR_MgzOutAC        = 97 , //매거진 배출부 컨베이어
        LODR_ClampDn         = 98 , //로더 클램프 Down
        LODR_ClampUp         = 99 , //로더 클램프 Up
        LODR_PusherFwBw      = 100, //로더 푸셔 Fwd/Bwd
        ETC_101              = 101, //Spare
        ETC_102              = 102, //Spare
        ETC_103              = 103, //Spare
        ETC_104              = 104, //Spare
        ETC_105              = 105, //Spare
        ETC_106              = 106, //Spare  
        ETC_107              = 107, //Spare  
        ETC_108              = 108, //Spare  
        ETC_109              = 109, //Spare  
        ETC_110              = 110, //Spare  
        ETC_111              = 111, //Spare  
        
        ULDR_MgzOutAC        = 112, //매거진 배출부 컨베이어
        ULDR_MgzInAC         = 113, //매거진 진입부 컨베이어
        ULDR_ClampDn         = 114, //로더 클램프 Down
        ULDR_ClampUp         = 115, //로더 클램프 Up
        ETC_116              = 116, //Spare
        ETC_117              = 117, //Spare
        ETC_118              = 118, //Spare
        ETC_119              = 119, //Spare
        ETC_120              = 120, //Spare
        ETC_121              = 121, //Spare
        ETC_122              = 122, //Spare
        ETC_123              = 123, //Spare
        ETC_124              = 124, //Spare
        ETC_125              = 125, //Spare
        ETC_126              = 126, //Spare
        ETC_127              = 127, //Spare

        MAX_OUTPUT
    };

    //Position Value id
    public enum pv : uint
    {
        HEAD_XVisnWait       = 0 ,
        HEAD_XVisnWorkStart      ,
        HEAD_XVisnRWorkStart     , //오른쪽비젼만 쓸때 시작 위치.
        //HEAD_XVisnLWorkEnd       , //왼쪽비젼만 쓸때 끝 위치.
        MAX_PSTN_MOTR0           , //HEAD_XVisn
        
        HEAD_YVisnWait       = 0 ,
        HEAD_YVisnWorkStart      ,
        MAX_PSTN_MOTR1           , //HEAD_YVisn
        
        PSTB_XMarkWait       = 0 ,
        PSTB_XMarkWorkStart      ,
        PSTB_XReplace            ,
        MAX_PSTN_MOTR2           , //PSTB_XMark
        
        PSTB_YMarkWait       = 0 ,
        PSTB_YMarkWorkStart      ,
        PSTB_YReplace            ,
        MAX_PSTN_MOTR3           , //PSTB_YMark
        
        HEAD_XCvr1Wait       = 0 ,
        HEAD_XCvr1Work           ,
        MAX_PSTN_MOTR4           , //HEAD_XCvr1

        HEAD_XCvr2Wait       = 0 ,
        HEAD_XCvr2Work           ,
        MAX_PSTN_MOTR5           , //HEAD_XCvr2

        HEAD_XCvr3Wait       = 0 ,
        HEAD_XCvr3Work           ,
        MAX_PSTN_MOTR6           , //HEAD_XCvr3
        
        LODR_YClmpWait       = 0 , //
        LODR_YClmpWork           , //
        LODR_YClmpPick           , //
        LODR_YClmpPlace          , //
        MAX_PSTN_MOTR7           , //LODR_YClmp
        
        LODR_ZClmpWait       = 0 , //
        LODR_ZClmpPickFwd        , //
        LODR_ZClmpClampOn        , //
        LODR_ZClmpPickBwd        , //
        LODR_ZClmpWorkStart      , //
        LODR_ZClmpPlaceFwd       , //
        LODR_ZClmpClampOff       , //
        LODR_ZClmpPlaceBwd       , //
        MAX_PSTN_MOTR8           , //LODR_ZClmp

        ULDR_YClmpWait       = 0 ,
        ULDR_YClmpWork           ,
        ULDR_YClmpPick           ,
        ULDR_YClmpPlace          ,
        MAX_PSTN_MOTR9           , //ULDR_YClmp
        
        ULDR_ZClmpWait       = 0 ,
        ULDR_ZClmpPickFwd        ,
        ULDR_ZClmpClampOn        ,
        ULDR_ZClmpPickBwd        ,
        ULDR_ZClmpWorkStart      ,
        ULDR_ZClmpPlaceFwd       ,
        ULDR_ZClmpClampOff       ,
        ULDR_ZClmpPlaceBwd       ,
        MAX_PSTN_MOTR10          , //ULDR_ZClmp
    };

    public class Pstn
    {
        public static uint[] Cnt = {  //1,3,3,2,4,3
            (uint)pv.MAX_PSTN_MOTR0 ,
            (uint)pv.MAX_PSTN_MOTR1 ,
            (uint)pv.MAX_PSTN_MOTR2 ,
            (uint)pv.MAX_PSTN_MOTR3 ,
            (uint)pv.MAX_PSTN_MOTR4 ,
            (uint)pv.MAX_PSTN_MOTR5 ,
            (uint)pv.MAX_PSTN_MOTR6 ,
            (uint)pv.MAX_PSTN_MOTR7 ,
            (uint)pv.MAX_PSTN_MOTR8 ,
            (uint)pv.MAX_PSTN_MOTR9 ,
            (uint)pv.MAX_PSTN_MOTR10 ,                         
        };// , (uint)pv.MAX_PSTN_MOTR1 , (uint)pv.MAX_PSTN_MOTR2};
    }

    

    //Manual Cycle.
    public enum mc : uint
    {
        NoneCycle             = 0  ,
        AllHome               = 1  ,
        LODR_Home                  ,//2
        PREB_Home                  ,//3
        HEAD_Home                  ,//4
        PSTB_Home                  ,//5
        ULDR_Home                  ,//6

        LODR_Wait             = 10 , //
        LODR_Supply           = 11 , //
        LODR_Pick             = 12 , //
        LODR_Work             = 13 , //
        LODR_Drop             = 14 , // 

        VSNZ_Wait             = 20 , // 
        VSNZ_Stt                   , // 21
        VSNZ_Move                  , // 22
        VSNZ_Insp                  , // 23
        VSNZ_Next                  , // 24

        PSTB_Wait             = 30 , // 
        PSTB_Stt                   , // 31
        PSTB_In                    , // 32
        PSTB_Work                  , // 33
        PSTB_Out                   , // 34
        PSTB_Next                  , // 35
        PSTB_Replace               , // 36
                              
        ULDR_Wait             = 40 , // 
        ULDR_Supply           = 41 , // 
        ULDR_Pick             = 42 , // 
        ULDR_Work             = 43 , // 
        ULDR_Drop             = 44 , // 


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
        ETC_Barcode         ,
        ETC_NotReady        ,
        LODR_PushOverload   ,
        LODR_SupplyFail     , //공급 실패, 픽실패
        PRT_PusherMiss      , //포스트버퍼랑 언로더에서 Push Miss 할때 쓰는듯
        VSN_ComErr          , //비전 통신 에러.
        VSN_VsnFailCnt      ,
        VSN_VsnSFailCnt     ,
        PRT_MgzFull         ,
        PRT_NeedMgz         ,
        VSN_VisnResetCnt    , //리셋 날렸는데 Busy 계속 살아있으면 해당 카운팅 넘어갈때 에러 띄우는듯
        VSN_TotalFailCnt    ,
        VSN_SameFailCnt     ,
        RAIL_FeedingFail    , //자재가 스테이션 위치에 제대로 안착되지 않으면 쓸 예정
        MTR_PosCal          , //포지션 계산 페일.
        ULDR_SupplyFail     , //공급 실패, 픽실패
        ETC_045             ,
        ETC_046             ,
        ETC_047             ,
        ETC_048             ,
        ETC_049             ,
        ETC_050             ,
        ETC_051             ,                                                                           
        ETC_052             ,
        ETC_053             ,
        LSC_ComErr          ,
        MAX_ERR    
    };

    //Error Id

}





