using SML;
using System;
using System.Drawing;
using System.Linq;

//TODO :: LIST
//Common Option Epuip Option등 예전꺼가 너무 들어가 있음 Bin 파일내에 다 지우고 새로좀...

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
        public const string sEqpName  = "WAFER LOADING SYSTEM";
        public const string sLanguage = "Korean"   ; //Korean , English , Chinese
        public const bool   bBiosLock = false      ; //true - 실행파일 루트에 db.lock파일 및 BIOS 암호화 내용이 있어야 동작.
    }
    
    public enum vi : uint //비전아이디.
    {

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
        LDR  = 0 , //로더 & 언로더
        
        
        MAX_PART,
    }

    //Array Id
    public class ri
    {
        public const int CST      = 0  ; //카세트
        public const int PCK      = 1  ; //피커
        public const int STG      = 2  ; //스테이지

        public const int MAX_ARAY = 3  ;
    }

    //Chip Status
    public enum cs : int
    {
        RetFail = -1 , //                                                                   
        None    =  0 , //카세트 없음
        Empty        , //카세트 웨이퍼 없는 자리

        Unkn         , //웨이퍼 작업 전
        Mask         , //웨이퍼 뺀자리
        Work         , //웨이퍼 작업 완료
        
        MAX_CHIP_STAT

    };

    //매트로UI 사용시 스타일 컬러.
    public class MetroColor
    {
        public Color Black   = Color.FromArgb(0  , 0  , 0  );
        public Color White   = Color.FromArgb(255, 255, 255);
        public Color Silver  = Color.FromArgb(85 , 85 , 85 );
        public Color Blue    = Color.FromArgb(0  , 174, 219);
        public Color Green   = Color.FromArgb(0  , 177, 89 );
        public Color Lime    = Color.FromArgb(142, 188, 0  );
        public Color Teal    = Color.FromArgb(0  , 170, 173);
        public Color Orange  = Color.FromArgb(243, 119, 53 );
        public Color Brown   = Color.FromArgb(165, 81 , 0  );
        public Color Pink    = Color.FromArgb(231, 113, 189);
        public Color Magenta = Color.FromArgb(255, 0  , 148);
        public Color Purple  = Color.FromArgb(124, 65 , 153);
        public Color Red     = Color.FromArgb(209, 17 , 65 );
        public Color Yellow  = Color.FromArgb(255, 196, 37 );
        public Color _custom = Color.FromArgb(225, 195, 143);
    }
    



    //mi<파트명3자리>_<축방샹1자리><부가명칭4> Motor Id
    public enum mi : uint
    {
        LDR_ZStg = 0 ,
        LDR_XPck = 1 ,
        LDR_YPck = 2 ,
        LDR_ZPck = 3 ,
        MAX_MOTR
    };

    //ai<파트명3자리>_<부가명칭><FWD시에위치2자리><BWD시에위치2자리>
    //대기가 Bwd 작동이 Fwd
    //장비 전면 기준으로 앞에 있는게 Front(Ft), 뒤에가 Rear(Rr)
    public enum ci : uint
    {
        //LDR_WorkStprRrFt    =  0  , //로더 워크랙 스토퍼
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
    //LDR   PKR   SRG
    public enum xi
    {
        LoadingSw           = 0   , //0x000  
        UnloadingSw         = 1   , //0x001  
        ResetSw             = 2   , //0x002  
        StopSw              = 3   , //0x003  
        Spare4              = 4   , //0x004  
        EmergencySw         = 5   , //0x005  
        WaferDtSsr          = 6   , //0x006  
        ManualInspLimit     = 7   , //0x007  
        WaferOverload       = 8   , //0x008  
        PickerVacuum        = 9   , //0x009  
        StageVacuum         = 10  , //0x00A  
        CassetteLeft        = 11  , //0x00B  
        CassetteRight       = 12  , //0x00C  
        Spare13             = 13  , //0x00D  
        Spare14             = 14  , //0x00E  
        Spare15             = 15  , //0x00F  

        MAX_INPUT
    };

    //그냥 아이오31.  <파트 3자리>+_+<세부설명>
    //복동 실린더 아이오 <파트 3자리>+_+<세부설명>+<해당행동 ex) Fw , Bw , Dn 등등 2자리>     자리
    //단동 실린더 아이오 <파트 3자리>+_+<세부설명>+<Fw시에해당행동2자리><Bw시에해당행동2자리> 동
    public enum yi
    {
        LoadingLamp          = 0   , //0x000  
        UnloadingLamp        = 1   , //0x001  
        ResetLamp            = 2   , //0x002  
        StopLamp             = 3   , //0x003  
        StageZBreakOff       = 4   , //0x004  
        PickerZBreakOff      = 5   , //0x005  
        PickerVacuum         = 6   , //0x006  
        StageVacuum          = 7   , //0x007  
        Spare8               = 8   , //0x008  
        Spare9               = 9   , //0x009  
        Spare10              = 10  , //0x00C  
        Spare11              = 11  , //0x00B  
        Spare12              = 12  , //0x00A  
        Spare13              = 13  , //0x00D  
        Spare14              = 14  , //0x00E  
        Spare15              = 15  , //0x00F  

        MAX_OUTPUT
    };

    public enum pv : uint
    {
        LDR_ZStgWait            = 0 ,
        //LDR_ZStgRecive              ,
        //LDR_ZStgBeforeAlign         ,
        LDR_ZStgAlign               ,
        LDR_ZStgBeforeStage         ,
        LDR_ZStgStage               ,
        LDR_ZStgStageDown           ,
        MAX_PSTN_MOTR0              ,

        LDR_XPckWait            = 0 , 
        LDR_XPckCst                 , 
        LDR_XPckStg                 , 
        MAX_PSTN_MOTR1              ,

        LDR_YPckWait            = 0 ,
        LDR_YPckBwd                 , 
        LDR_YPckFwdCst              ,
        LDR_YPckFwdStg              ,
        LDR_YPckStgMoveAble         ,
        MAX_PSTN_MOTR2              ,        

        LDR_ZPckWait            = 0 ,
        LDR_ZPck1stWaferBtm         ,
        LDR_ZPck1stWafer            ,
        LDR_ZPckStgAlignUp          ,
        LDR_ZPckStgAlign            ,
        LDR_ZPckStgAlignDown        ,

        //LDR_ZPckStgTop              ,
        //LDR_ZPckStg                 ,
        //LDR_ZPckStgBtm              ,
        MAX_PSTN_MOTR3              ,

       
    };

    public class Pstn
    {
        public static uint[] Cnt = { (uint)pv.MAX_PSTN_MOTR0 , 
                                     (uint)pv.MAX_PSTN_MOTR1 ,
                                     (uint)pv.MAX_PSTN_MOTR2 ,
                                     (uint)pv.MAX_PSTN_MOTR3 };// , (uint)pv.MAX_PSTN_MOTR1 , (uint)pv.MAX_PSTN_MOTR2};
    }

    

    //Manual Cycle.
    public enum mc : uint
    {
        NoneCycle             = 0  ,
        AllHome               = 1  ,
        LDR_CycleHome              ,

        CycleLoading      = 10 , //10
        CycleUnLoading         , //11
        LoadingPick            , //12
        LoadingPlace           , //13
        StageDown              , //14
        StageUp                , //15
        UnloadingPick          , //16
        UnloadingPlace         , //17
        
        CycleWait              , //18 추가

        CycleIdle              , //19 여자 아이들

        MAX_MANUAL_CYCLE       , 
    };

    //Error Id
    public enum ei : uint
    {
        ETC_Emergency     =0   ,
        ETC_ToStartTO          ,
        ETC_ToStopTO           ,
        ETC_HomeTO             ,
        ETC_ManCycleTO         ,
        ETC_CycleTO            ,
                               
        MTR_HomeEnd            ,
        MTR_NegLim             ,
        MTR_PosLim             ,
        MTR_Alarm              ,
                               
        PKG_Dispr              , 
        PKG_Unknwn             ,
                               
        LDR_WaferVacuum        ,
        LDR_StageVacuum        ,
                               
        LDR_NoCst              , //카세트 감지 안됨
        LDR_CstLocationErr     , //카세트 센서 두개다 안들어 오면 에러
        LDR_CstNoMask          , //자재 뺀위치가 없음
        LDR_CstNoWafer         , //카세트에 자재가 없음

        LDR_ManualInspLimit    , //엣지 검사기 위치 충돌 에러
        LDR_WaferOverload      ,

        LDR_NoWafer            , //언로딩 하려는데 어딘가 자재가 없다
                               
        PCK_NoWafer            ,
        PCK_Wafer              , //피커에 웨이퍼를 제거 해 주세요.
        STG_Wafer              , //스테이지에 웨이퍼를 제거 해 주세요.
        STG_NoWafer            ,
                               
        STG_AlignFail          ,
        STG_RemoveWafer        ,
                               
        PCK_NeedtoBwd          ,

        MAX_ERR    
    };

    //Error Id

}





