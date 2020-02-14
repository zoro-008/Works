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
        public const string sEqpName  = "HVI-540RI";
        public const string sLanguage = "English"  ; //Korean , English , Chinese
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
        PLDR     , //로더.
        LODR     , //로더.
        VISN     , //비전
        MARK     , //마킹
        PULD     , //프리 언로더
        ULDR     , //언로더
        REJM     , //마킹 불량 제거 존
        REJV     , //비전 불량 제거 존
        TBLE     , //턴테이블 돌아가는 파트.
        MAX_PART , 
    }

    //aRay Id
    public class ri
    {
        public const int LODR      = 0 ; //로더 존
        public const int PLDR      = 1 ; //로더 존
        public const int TLDR      = 2 ; //피커 로더 존
        public const int TVSN      = 3 ; //리어 인덱스
        public const int TMRK      = 4 ; //프론트 인덱스
        public const int TULD      = 5 ; //피커 언로더 존
        public const int TRJM      = 6 ; //트레이 페일.
        public const int TRJV      = 7 ; //트레이 굿.
        public const int PULD      = 8 ; //트레이 굿.
        public const int ULDR      = 9 ; //언로더 존
        public const int PICK      = 10; //언로더 존
        public const int PSHR      = 11; //언로더 푸셔
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
        LODR_YIndx = 0 , //로더 인덱스 
        LODR_XPshr = 1 , //로더 푸셔
        TBLE_TTble = 2 , //툴 턴테이블.
        LODR_XPckr = 3 , //로더 피커
        VISN_ZGrpr = 4 , //비전 그립퍼 
        ULDR_ZPckr = 5 , //언로더 피커
        ULDR_XGrpr = 6 , //언로더 그립퍼
        ULDR_YIndx = 7 , //언로더 인덱스
        ULDR_ZNzzl = 8 , //언로더 노즐 Z축
        ULDR_XNzzl = 9 , //언로더 노즐 X축
        MAX_MOTR //Enum.GetValues(typeof(xxxxx)).Length;
    };

    //ai<파트명3자리>_<부가명칭><FWD시에위치2자리><BWD시에위치2자리>
    //대기가 Bwd 작동이 Fwd
    public enum ci : uint
    {
        TBLE_Grpr1FwBw    =  0 ,   //턴테이블 그립퍼0
        TBLE_Grpr2FwBw    =  1 ,   //턴테이블 그립퍼
        TBLE_Grpr3FwBw    =  2 ,   //턴테이블 그립퍼
        TBLE_Grpr4FwBw    =  3 ,   //턴테이블 그립퍼
        TBLE_Grpr5FwBw    =  4 ,   //턴테이블 그립퍼
        TBLE_Grpr6FwBw    =  5 ,   //턴테이블 그립퍼
        LODR_PshrRtrCwCCw =  6 ,   //로더 푸셔 로터리 실린더
        LODR_RngJigFwBw   =  7 ,   //로더 링 지그 실린더
        LODR_GuideOpCl    =  8 ,   //로더 가이드 오픈
        LODR_RngGrpFwBw   =  9 ,   //로더 링 그립퍼
        LODR_GrpRtrCwCCw  = 10 ,   //로더 그립퍼로터리 실린더
        LODR_PckrRtrCwCCw = 11 ,   //로더 픽커로터리 실린더
        LODR_PckrFwBw     = 12 ,   //로더 픽커 실린더.
        VISN_TurnGrpFwBw  = 13 ,   //비전 턴 그립퍼
        VISN_TurnRtrCwCCw = 14 ,   //비전 턴 로터리
        VISN_FixRtrCwCCw  = 15 ,   //비전 픽 로터리
        VISN_GrpRtrCwCCw  = 16 ,   //비전 그립퍼 로터리 
        MARK_AlgnFwBw     = 17 ,   //마크 얼라인
        MARK_AlgnPinFwBw  = 18 ,   //마크 얼라인 핀
        REJM_GrpRtrCwCCw  = 19 ,   //리젝 마킹존 그립퍼 로터리
        REJM_RngGrpFwBw   = 20 ,   //리젝 마킹존 링 그립퍼
        REJV_GrpRtrCwCCw  = 21 ,   //리젝 비전 존 그핍퍼 로터리
        REJV_RngGrpFwBw   = 22 ,   //리젝 비전 링 그립퍼
        ULDR_ReGrRtrCwCCw = 23 ,   //언로더 리그립 로터리
        ULDR_RngReGrFwBw  = 24 ,   //언로더 링 리그립퍼
        ULDR_OutPshrFwBw  = 25 ,   //언로더 아웃 푸셔
        ULDR_RngGrpFwBw   = 26 ,   //언로더 링 그립퍼
        ULDR_GrpRtrCwCCw  = 27 ,   //언로더 그립퍼 로터리
        ULDR_GrpFwBw      = 28 ,   //언로더 그립퍼 
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
        ETC_LStartSw        =  0 , //
        ETC_LStopSw         =  1 , //
        ETC_LResetSw        =  2 , //
        ETC_LAirSw          =  3 , //  
        ETC_LInitSw         =  4 , //  
        ETC_RStartSw        =  5 , //  
        ETC_RStopSw         =  6 , //  
        ETC_RResetSw        =  7 , //
        ETC_RAirSw          =  8 , //  
        ETC_RInitSw         =  9 , //  
        ETC_Manual1         = 10 , //그냥 메뉴얼로 뿌리는 버튼1.
        ETC_Manual2         = 11 , //그냥 메뉴얼로 뿌리는 버튼2.  
        ETC_Manual3         = 12 , //그냥 메뉴얼로 뿌리는 버튼3.
        ETC_Manual4         = 13 , //그냥 메뉴얼로 뿌리는 버튼4.
        ETC_Manual5         = 14 , //그냥 메뉴얼로 뿌리는 버튼5.  
        ETC_MainPower       = 15 , //

        ETC_DoorFtRt        = 16 , //  
        ETC_DoorFtMd        = 17 , //  
        ETC_DoorFtLt        = 18 , //  
        ETC_DoorLt          = 19 , //  
        ETC_DoorRrLt        = 20 , //  
        ETC_DoorRrMd        = 21 , //  
        ETC_DoorRrRt        = 22 , //  
        ETC_DoorRt          = 23 , //  
        ETC_LEmgSw          = 24 , //  
        ETC_REmgSw          = 25 , // 
        ETC_MainAir         = 26 , // 
        LODR_PshrLimt       = 27 , // 
        LODR_PshrRtrCw      = 28 , // 
        LODR_PshrRtrCcw     = 29 , //  
        LODR_PngDtct        = 30 ,
        LODR_PckrVac        = 31 , //

        LODR_PckrRtrCw      = 32 , //  
        LODR_PckrRtrCCw     = 33 , //  
        LODR_LastDetect     = 34 , //  
        LODR_GuideOp        = 35 ,
        LODR_GuideCl        = 36 ,
        LODR_RngJigFw       = 37 ,
        LODR_RngJigBw       = 38 ,
        LODR_GrpRtrCw       = 39 ,
        LODR_GrpRtrCCw      = 40 ,
        VISN_FixRtrCw       = 41 ,
        VISN_FixRtrCCw      = 42 ,
        VISN_GrpRtrCw       = 43 ,
        VISN_GrpRtrCCw      = 44 ,
        VISN_TurnGrpFw      = 45 ,
        VISN_TurnGrpBw      = 46 ,
        VISN_TurnRtrCw      = 47 ,

        VISN_TurnRtrCcw     = 48 ,
        MARK_AlgnFw         = 49 ,
        MARK_AlgnBw         = 50 ,
        MARK_AlgnpinUp      = 51 ,
        MARK_AlgnpinDn      = 52 ,
        TBLE_LODRClmpDtct   = 53 ,
        TBLE_VISNClmpDtct   = 54 ,
        TBLE_MARKClmpDtct   = 55 ,
        TBLE_ULDRClmpDtct   = 56 ,
        TBLE_RJEMClmpDtct   = 57 ,
        TBLE_RJEVClmpDtct   = 58 ,
        TBLE_VISNPkgDtct    = 59 ,
        TBLE_RJCMPkgDtct    = 60 ,
        TBLE_RJCVPkgDtct    = 61 ,
        TBLE_ULDRPkgDtct    = 62 ,
        ETC_063             = 63 ,

        RJCV_GrpRtrUp       = 64 , //  
        RJCV_GrpRtrDn       = 65 , //  
        RJCM_GrpRtrUp       = 66 , //  
        RJCM_GrpRtrDn       = 67 ,
        ULDR_GrpRtrCw       = 68 ,
        ULDR_GrpRtrCcw      = 69 ,
        ULDR_GrpFw          = 70 ,
        ULDR_GrpBw          = 71 ,
        ULDR_RgrpRtrCw      = 72 ,
        ULDR_RgrpRtrCcw     = 73 ,
        ULDR_OutPshrFw      = 74 ,
        ULDR_OutPshrBw      = 75 ,
        LODR_PickrFw        = 76 ,
        LODR_PickrBw        = 77 ,
        ETC_LDREmgSw        = 78 ,
        ETC_ULDEmgSw        = 79 ,

        LSR_Error           = 80 ,//이상
        LSR_Warning         = 81 ,//경고
        LSR_TrgReady        = 82 ,//트리거 레디
        LSR_InPrint         = 83 ,//인쇄 중
        LSR_PrintEnd        = 84 ,//인쇄 완료
        LSR_CheckOk         = 85 ,
        LSR_CheckNg         = 86 ,
        ETC_87              = 87 ,
        ETC_88              = 88 ,
        ETC_89              = 89 ,
        ETC_90              = 90 ,
        ETC_91              = 91 ,
        ETC_92              = 92 ,
        ETC_93              = 93 ,
        ETC_94              = 94 ,
        ETC_95              = 95 ,

        MAX_INPUT //Enum.GetValues(typeof(xxxxx)).Length;
    };

    //그냥 아이오31.  <파트 3자리>+_+<세부설명>
    //복동 실린더 아이오 <파트 3자리>+_+<세부설명>+<해당행동 ex) Fw , Bw , Dn 등등 2자리>     자리
    //단동 실린더 아이오 <파트 3자리>+_+<세부설명>+<Fw시에해당행동2자리><Bw시에해당행동2자리> 동
    public enum yi : uint
    {
        ETC_LStartLp          =  0, // 
        ETC_LStopLp           =  1, // 
        ETC_LResetLp          =  2, // 
        ETC_LAirLp            =  3, // 
        ETC_LInitLp           =  4, // 
        ETC_RStartLp          =  5, // 
        ETC_RStopLp           =  6, // 
        ETC_RResetLp          =  7, // 
        ETC_RAirLp            =  8, // 
        ETC_RInitLp           =  9, // 
        ETC_Manual1           = 10, // 
        ETC_Manual2           = 11, // 
        ETC_Manual3           = 12, // 
        ETC_Manual4           = 13, // 
        ETC_Manual5           = 14, // 
        ETC_MainAir           = 15, //

        ETC_LightOn           = 16, // 
        RJCM_OutAc            = 17, // 
        RJCV_OutAc            = 18, // 
        ETC_TwRedLp           = 19, // 
        ETC_TwYelLp           = 20, // 
        ETC_TwGrnLp           = 21, // 
        ETC_TwBzz             = 22, // 
        LODR_PshrRtrCcw       = 23, // 
        LODR_PshrRtrCw        = 24, // 
        LODR_PckrRtrCw        = 25, // 
        LODR_GudeOpenUp       = 26, // 
        LODR_RngJigFw         = 27, // 
        LODR_RngGrp           = 28, // 
        LODR_GrpRtrCw         = 29, // 
        VISN_FixRtrClamp      = 30, // 
        VISN_GrpRtrUp         = 31, // 

        VISN_TurnGrpClamp     = 32, //  
        VISN_TurnRtrCw        = 33, //  
        MARK_AlgnFw           = 34, //  
        MARK_AlgnPinUp        = 35, //  
        TBLE_Grpr1            = 36, //  
        TBLE_Grpr2            = 37, //  
        TBLE_Grpr3            = 38, //  
        TBLE_Grpr4            = 39, //  
        TBLE_Grpr5            = 40, //  
        TBLE_Grpr6            = 41, //  
        RJCM_RngGrp           = 42, //  
        RJCV_RngGrp           = 43, //  
        RJCM_GprRtrUp         = 44, //  
        RJCV_GprRtrUp         = 45, //  
        ULDR_GrpRtrCw         = 46, //  
        ULDR_GrpFw            = 47, //  
        
        ULDR_RGrpRtrCw        = 48, //
        ULDR_OutPshrFw        = 49, //
        LODR_PickrVac         = 50, //
        LODR_PickrFw          = 51, //
        LSR_Triger            = 52, //
        VISN_TurnGrpUnClamp   = 53, //
        VISN_TurnRtrCcw       = 54, //
        ULDR_PickRngGrp       = 55, //
        ULDR_PickRngReGrp     = 56, //
        LSR_Check             = 57, //
        ETC_058               = 58, //
        MARK_Light            = 59, //
        ETC_060               = 60, //
        ETC_061               = 61, //
        ETC_062               = 62, //
        ETC_063               = 63, //
        MAX_OUTPUT //Enum.GetValues(typeof(xxxxx)).Length;
    };

    //Position Value id
    public enum pv : uint
    {
      
        //LODR_YIndx                      
        LODR_YIndxWait             = 0,
        LODR_YIndxWork                ,
        MAX_PSTN_MOTR0                ,

        //LODR_XPshr                  
        LODR_XPshrWait             = 0,
        LODR_XPshrWorkStt             ,
        LODR_XPshrWorkEnd             ,
        LODR_XPshrBackOfs             ,
        MAX_PSTN_MOTR1                ,

        //TBLE_TTlbe
        TBLE_TTbleWait             = 0,
        TBLE_TTbleWorkPitch           ,
        MAX_PSTN_MOTR2                ,

        //LODR_XPckr
        LODR_XPckrWait             = 0,
        LODR_XPckrPick                ,
        LODR_XPckrPickRtt             ,
        LODR_XPckrPlce                ,
        LODR_XPckrPlceRtt             ,
        MAX_PSTN_MOTR3                ,

        //VISN_ZGrpr                     
        VISN_ZGrprWait             = 0,
        VISN_ZGrprWork                ,
        MAX_PSTN_MOTR4                ,

        //ULDR_ZPckr                      
        ULDR_ZPckrWait             = 0,
        ULDR_ZPckrPick                ,
        ULDR_ZPckrPlace               ,//delivery
        MAX_PSTN_MOTR5                ,
          
        //ULDR_XGrpr                     
        ULDR_XGrprWait             = 0,
        ULDR_XGrprPick                ,
        ULDR_XGrprPlace               ,
        MAX_PSTN_MOTR6                ,
         
        //ULDR_YIndx                               
        ULDR_YIndxWait             = 0,
        ULDR_YIndxWork                , //픽 위치로 가기 전 실린더 Fwd 시키는 위치
        MAX_PSTN_MOTR7                ,
           
        //ULDR_ZNzzl              
        ULDR_ZNzzlWait             = 0,
        ULDR_ZNzzlWork                ,
        MAX_PSTN_MOTR8                ,

        //ULDR_XNzzl
        ULDR_XNzzlWait             = 0,
        ULDR_XNzzlWorkStt             ,
        ULDR_XNzzlWorkEnd             ,
        MAX_PSTN_MOTR9                ,
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
        STG_PickMiss        ,
        ETC_BarcPrint       , //Barcode Print
        ETC_RS232           ,
        ETC_CalErr          , //연산 에러
        PRT_TraySupply      , //Index작업 하려는데 Unloader에 자리 없으면...
        PRT_FullTray        ,
        PRT_RemoveTray      ,
        ETC_FlowMeter       ,
        LSC_ComErr          ,
        MAX_ERR //Enum.GetValues(typeof(xxxxx)).Length;
    };

    //Error Id

}





