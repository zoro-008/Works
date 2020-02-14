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
        public const string sEqpName  = "SCREW HEIGHT ADJUST SYSTEM";
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
        ADJ   = 0 , //
        
        MAX_PART,
    }

    //Array Id
    public class ri
    {
        public const int STT      = 0  ; //초기 투입
        public const int PRE      = 1  ; //높이조절 전 높이 측정
        public const int ADJ      = 2  ; //높이조절모터
        public const int PST      = 3  ; //높이조절 후 높이 측정

        public const int MAX_ARAY = 4  ;
    }

    //Chip Status
    public enum cs : int
    {
        RetFail = -1 , //                                                                   
        None    =  0 , //자재 없음

        Unkn         , //높이 측정/조절 작업 전
        Work         , //높이 측정/조절 작업 완료
        Good         , //높이 조절 후 높이 측정 시 양품
        NG1          , //높이불량
        NG2          , //상한토크 불량
        NG3          , //하한토크 불량

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
        //LDR_ZStg = 0 ,
        //LDR_XPck = 1 ,
        //LDR_YPck = 2 ,
        //LDR_ZPck = 3 ,
        MAX_MOTR
    };

    //ai<파트명3자리>_<부가명칭><FWD시에위치2자리><BWD시에위치2자리>
    //대기가 Bwd 작동이 Fwd
    //장비 전면 기준으로 앞에 있는게 Front(Ft), 뒤에가 Rear(Rr)
    public enum ci : uint
    {
        ADJ_TurnTableCw     = 0, //턴테이블 Cw 실린더(역회전 안됨)
        ADJ_PreStageUpDn    = 1, //높이조절 전 높이측정 툴 업/다운 실린더
        ADJ_PostStageUpDn   = 2, //높이조절 후 높이측정 툴 업/다운 실린더
        ADJ_AdjTransferFwBw = 3, //높이조절 툴 위치 변경 Fwd/Bwd 실린더
        ADJ_AdjustUpDn      = 4, //높이조절 툴 업/다운 실린더 주의!!Fwd가 업이다.
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
        StartSw             = 0   , //0x000  
        ResetSw             = 1   , //0x001  
        EmergencySw         = 2   , //0x002  
        ADJ_TurnTableReady  = 3   , //0x003 
        ADJ_TurnTableHome   = 4   , //0x004 ADJ_TurnTableCw 실린더 돌리기전에 들어와있어야함
        HighTqBoxDtct       = 5   , //0x005 이거 불량박스 감지센서로 쓸거임 
        ADJ_PreStageDn      = 6   , //0x006  
        HghtNGBoxDtct       = 7   , //0x007 이거 불량박스 감지센서로 쓸거임 
        ADJ_PostStageDn     = 8   , //0x008  
        ADJ_AdjTransferFw   = 9   , //0x009  
        ADJ_AdjTransferBw   = 10  , //0x00A  
        ADJ_AdjustUp        = 11  , //0x00B  
        ADJ_AdjustUpDnBuff  = 12  , //높이조절용 실린더 완충용 솔 밸브   
        ADJ_AdjustDn        = 13  , //
        LowTqBoxDtct= 14  , //0x00E  
        HSensorReady        = 15  , //0x00F  
        NutReady            = 16  , //0x010  
        NutAlarm            = 17  , //0x011  
        NutBusy             = 18  , //0x012  
        NutComplete         = 19  , //0x013  
        NutFastenOK         = 20  , //0x014  
        NutTRQHighNG        = 21  , //0x015  
        NutTRQLowNG         = 22  , //0x016  
        NutANGHighNG        = 23  , //0x017  
        NutANGLowNG         = 24  , //0x018  
        NutTimeNG           = 25  , //0x019  
        NutMonitorNG        = 26  , //0x01A  
        NutCHOut1           = 27  , //0x01B  
        NutCHOut2           = 28  , //0x01C  
        NutCHOut4           = 29  , //0x01D  
        NutCHOut8           = 30  , //0x01E  
        NutCHOut16          = 31  , //0x01F  

        MAX_INPUT
    };

    //그냥 아이오31.  <파트 3자리>+_+<세부설명>
    //복동 실린더 아이오 <파트 3자리>+_+<세부설명>+<해당행동 ex) Fw , Bw , Dn 등등 2자리>     자리
    //단동 실린더 아이오 <파트 3자리>+_+<세부설명>+<Fw시에해당행동2자리><Bw시에해당행동2자리> 동
    public enum yi
    {
        OKLamp              = 0   , //0x000  
        NGLamp              = 1   , //0x001  
        BuzzerVoice         = 2   , //0x002  
        ADJ_TurnTableCw     = 3   , //0x003  
        ADJ_TurnTableInit   = 4   , //0x004  //ADJ_TurnTableCw 실린더 돌리기 전에 한번 켰다 꺼줘야함(안돌아감)
        ADJ_PreStageUpDn    = 5   , //0x005  
        ADJ_PostStageUpDn   = 6   , //0x006  
        ADJ_AdjTransferFw   = 7   , //0x007  
        ADJ_AdjTransferBw   = 8   , //0x008  
        ADJ_AdjustDn        = 9   , //0x009  
        ADJ_AdjustUp        = 10  , //0x00C  
        ADJ_AdjustUpDnBuff  = 11  , //0x00B  
        HSensorClear        = 12  , //0x00A  
        HSensorZero         = 13  , //0x00D  
        HSensorStart        = 14  , //0x00E  
        Spare15             = 15  , //0x00F  
        NutSkip             = 16  , //0x010  
        NutStop             = 17  , //0x011  
        NutReset            = 18  , //0x012  
        NutQStart           = 19  , //0x013  
        NutFStart           = 20  , //0x014  
        NutSStart           = 21  , //0x015  
        NutOStart           = 22  , //0x016  
        NutDataOut          = 23  , //0x017  
        NutSVOn             = 24  , //0x018  
        NutPJog             = 25  , //0x019  
        NutNJog             = 26  , //0x01A  
        NutCHSel1           = 27  , //0x01B  
        NutCHSel2           = 28  , //0x01C  
        NutCHSel4           = 29  , //0x01D  
        NutCHSel8           = 30  , //0x01E  
        NutCHSel16          = 31  , //0x01F  

        ETC_TwLpR           = 32  , //0x000  
        ETC_TwLpY           = 33  , //0x001  
        ETC_TwLpG           = 34  , //0x002  
        ETC_LowTqNGLp       = 35  , //0x003  
        ETC_HighTqNGLp      = 36  , //0x004  //ADJ_TurnTableCw 실린더 돌리기 전에 한번 켰다 꺼줘야함(안돌아감)
        ETC_HghtNGLp        = 37  , //0x005  
        Spare38             = 38  , //0x006  
        Spare39             = 39  , //0x007  
        Spare40             = 40  , //0x008  
        Spare41             = 41  , //0x009  
        Spare42             = 42  , //0x00C  
        Spare43             = 43  , //0x00B  
        Spare44             = 44  , //0x00A  
        Spare45             = 45  , //0x00D  
        Spare46             = 46  , //0x00E  
        Spare47             = 47  , //0x00F  
        
        MAX_OUTPUT
    };

    public enum pv : uint
    {
        //LDR_ZStgWait            = 0 ,
        MAX_PSTN_MOTR0              ,

        //LDR_XPckWait            = 0 , 
        MAX_PSTN_MOTR1              ,

        //LDR_YPckWait            = 0 ,
        MAX_PSTN_MOTR2              ,        

        //LDR_ZPckWait            = 0 ,
        MAX_PSTN_MOTR3              ,

       
    };

    public class Pstn
    {
        public static uint[] Cnt = { /*(uint)pv.MAX_PSTN_MOTR0 , 
                                     (uint)pv.MAX_PSTN_MOTR1 ,
                                     (uint)pv.MAX_PSTN_MOTR2 ,
                                     (uint)pv.MAX_PSTN_MOTR3*/ };// , (uint)pv.MAX_PSTN_MOTR1 , (uint)pv.MAX_PSTN_MOTR2};
    }

    

    //Manual Cycle.
    public enum mc : uint
    {
        NoneCycle             = 0  ,
        AllHome               = 1  ,
        ADJ_CycleHome              ,

        CycleRun              = 10 , //10
        TableTurn                  , //11
        PreCheck                   , //12
        Adjust                     , //13
        PostCheck                  , //14

        NutCylUp               = 20,
        NutCylDown                 ,
        NutStart                   ,
        NutStop                    ,             //너트런너 스탑 매뉴얼 사이클
                    
        
                             
        MAX_MANUAL_CYCLE           , 
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

        ATR_TimeOut            ,

        PKG_Dispr              , 
        PKG_Unknwn             ,

        ETC_LotOpen           ,

        ADJ_WorkCountOver     ,//토탈 워크 카운트보다 현재 작업카운트가 많으면 띄우는 에러
                               
        ADJ_HghtCheck         ,//높이측정 에러, 혹시 몰라서 넣었음.
        //ADJ_SafetySensor      ,//안전센서 에러
                               
        ADJ_Communication     , //통신에러

        MAX_ERR    
    };

    //Error Id

}





