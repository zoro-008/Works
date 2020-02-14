using System;

namespace Machine
{
    //Serial port ID
    public class Eqp
    {
        public const string sEqpName = "HEC-720BT";
    }

    public enum si : uint
    {
        MAX_RS232 
    }
    public enum fb : uint
    {
        Bwd = 0,
        Fwd = 1,
    }

    //Part ID
    public enum pi : uint
    {
        IDX     ,
        MAX_PART
    }

    //aRay Id
    public class ri
    {
        //public const int NONE       = -1 ,
        public const int IDX        = 0 ;
        public const int PST_IDX    = 1 ;
        public const int MAX_ARAY   = 2 ;
    };
    
    //Chip Status
    public enum cs : int
    {
        RetFail = -1,
        None    =  0,
        Tensn       ,
        Unkwn       ,
        Work        ,
        Move        ,
        Empty       ,
        MAX_CHIP_STAT
    };



    //mi<파트명3자리>_<축방샹1자리><부가명칭4> Motor Id
    public enum mi : uint
    {
        IDX_XCUT  = 0,
        IDX_XOUT     ,
        IDX_TTRN     ,
        

        MAX_MOTR
    };

    //ai<파트명3자리>_<부가명칭><FWD시에위치2자리><BWD시에위치2자리>
    //대기가 Bwd 작동이 Fwd
    public enum ci : uint
    {
        IDX_Hold1UpDn   = 0, //Loader 쪽부터 시작해서 1번
        IDX_CutLtFwBw   = 1, //실에 가시 내는 커터 실린더
        IDX_CutRtFwBw   = 2,
        IDX_TwstLtDnUp  = 3, //실 방향 돌려줄때 잡는 실린더
        IDX_TwstRtDnUp  = 4,
        IDX_Hold2UpDn   = 5,
        IDX_CutBaseUpDn = 6,
        IDX_OutDnUp     = 7,
        IDX_CutterDnUp  = 8, //마지막 제품 커팅하는 실린더
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
        ETC_EmgSw           =  4 , //  
        ETC_MainAir         =  5 , //  
        IDX_Detect1         =  6 , //  
        IDX_Detect2         =  7 , //  
        IDX_Detect3         =  8 , //  
        IDX_Detect4         =  9 , //  
        IDX_Detect5         = 10 , //  
        ULD_Detect1         = 11 , //  
        ULD_Detect2         = 12 , //  
        ULD_Detect3         = 13 , //  
        ULD_Detect4         = 14 , //  
        ULD_Detect5         = 15 , //  

        IDX_CutterDn        = 16,  //  
        IDX_Hold2Dn         = 17,  //  
        IDX_CutBaseUp       = 18,  //  
        IDX_ShiftUp         = 19,  //  
        IDX_OutUp           = 20 , //  
        IDX_TwstLtCl        = 21 , //이거 이상해서 문의중  
        IDX_TwstRtCl        = 22 , //이거 이상해서 문의중
        IDX_Hold1Dn         = 23 , //  
        IDX_CutLtFw         = 24 , //실에 가시 내는 Cylinder Forward  
        IDX_CutRtFw         = 25 , //실에 가시 내는 Cylinder Forward 
        ETC_26              = 26 , //  
        ETC_27              = 27 , // 
        ETC_28              = 28 , // 
        ETC_29              = 29 , //  
        ETC_30              = 30 , //  
        ETC_31              = 31 , // 
        
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
        ETC_AirLp             =  3, //  
        ETC_TwRedLp           =  4, //  
        ETC_TwYelLp           =  5, //  
        ETC_TwGrnLp           =  6, //  
        ETC_TwBzz             =  7, //  
        ETC_MainAirSol        =  8, //  
        IDX_CutterDnUp        =  9, //  
        IDX_Hold2UpDn         = 10, //  
        IDX_CutBaseUpDn       = 11, //렌즈 스테이지 배큠온.
        IDX_ShiftUpDn         = 12, //  
        IDX_OutDnUp           = 13, //  
        IDX_TwstLtDnUp        = 14, //  
        IDX_TwstRtDnUp        = 15, //  

        IDX_Hold1UpDn         = 16, //  
        IDX_CutLtFwBw         = 17, //  
        IDX_CutRtFwBw         = 18, //  
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
        //아직 모터 넘버 안나와서 임시로 쓴다.
        //mi.IDX_X = 0  ,
        IDX_XCUTWait        = 0,
        IDX_XCUTWorkStt        ,
        MAX_PSTN_MOTR0         ,

        //mi.ULD_X = 2  ,
        IDX_XOUTWait        = 0,
        IDX_XOUTStopWait       ,
        IDX_XOUTClamp          , 
        IDX_XOUTBin            ,
        IDX_XOUTTensnOfs       ,
        IDX_XOUTRvrsOfs        ,
        //IDX_XOUTPull           ,  포지션 삽입될수도..
        MAX_PSTN_MOTR1         ,

        //mi.IDX_T = 1  
        IDX_TTRNWait        = 0,
        MAX_PSTN_MOTR2         ,

        
    };

    //Manual Cycle.
    public enum mc : uint
    {
        NoneCycle                 = 0,
        AllHome                   = 1,
        IDX_Home                  = 2,

        IDX_CycleWork             = 11,
        IDX_CycleOut              = 12,
        //IDX_CycleSupply           = 13,
        IDX_CycleManSttWait       = 13,
        
        //ASY_CycleTrayInsp         = 12,
        //ASY_CycleTrayMove         = 13,
        IDX_HolderFwd             = 20,
        IDX_HolderBwd             = 21,
        IDX_CutterFwd             = 22,
        IDX_CutterBwd             = 23,


        MAX_MANUAL_CYCLE
    };

    //Error Id
    public enum ei : uint
    {
        /*000*/eiETC_MainAir      = 0,
        /*001*/ei001                 ,
        /*002*/ETC_Emergency         ,
        /*003*/ei003                 ,
        /*004*/eiETC_Door            ,
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
        /*026*/ei026                 ,
        /*027*/PKG_Unknwn          ,
        /*028*/ei028                 ,
        /*029*/ei029                 ,
        /*030*/PRT_OverLoad          ,
        /*031*/VSN_InspRangeOver     ,
        /*032*/VSN_InspNG            ,
        /*033*/VSN_ComErr            ,
        /*034*/PRT_Missed            ,
        /*035*/PRT_Detect            ,
        /*036*/ei036                 ,
        /*037*/ei037                 ,
        /*038*/ei038                 ,
        /*039*/ei039                 ,
        /*040*/ei040                 ,
        /*041*/ei041                 ,
        /*042*/ei042                 ,
        /*043*/ei043                 ,
        /*044*/ei044                 ,
        /*045*/ei045                 ,
        /*046*/ei046                 ,
        /*047*/ei047                  ,
        /*048*/eiETC_RS232           ,
        /*049*/ei049                 ,
        /*050*/ei050                 ,
    
        MAX_ERR    
    };  
}





