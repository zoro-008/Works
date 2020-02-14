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
        public const string sEqpName  = "Cell Clean System";
        public const string sLan_Kor  = "Korean"   ; //Korean , English , Chinese
        public const string sLan_Eng  = "English"  ; //Korean , English , Chinese
        public const string sLanguage = sLan_Kor   ; //Korean , English , Chinese
        public const bool   bBiosLock = false      ; //true - 실행파일 루트에 db.lock파일 및 BIOS 암호화 내용이 있어야 동작.
    }
     
    public enum vi : uint //비전아이디.
    {

        MAX_VI 
    }

    public enum PumpID
    {
        NotUse = -1 ,
        NR     = 0  ,
        RET    = 1  ,
        FDS    = 2  ,
        DC     = 3  ,
        FCM    = 4  ,
        Blood  = 5  ,
        MAX_PUMP_ID
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
        PKR      , //픽커 & 셔틀  1
        QCB      , //QC버퍼
        SYR      , //실린지 & 믹싱챔버.
        CHA      , //챔버공압.
        MAX_PART,
    }

    //Array Id
    public class ri
    {
        public const int LDR      = 0  ; //Loader Work 1
        public const int PKR      = 1  ; //Picker  1
        public const int SUT      = 2  ; //Suttle. 1
        public const int SYR      = 3  ; //Syringe 1
        public const int CHA      = 4  ; //Chamber 6
        public const int FRG      = 5  ; //Fridge  3
        public const int BFR      = 6  ; //Buffer  3 
        public const int RPK      = 7  ; //Right Buffer Picker 1
        public const int LPK      = 8  ; //Left QC Picker 1
        public const int MAX_ARAY = 9  ;
    }

    //Chip Status
    public enum cs : int
    {
        RetFail = -1 , //                                                                   
        None    =  0 , //
        Mask         , //오토QC자제에서 뺀자리.
        Shake        , //쉐이킹 필요.
        Freeze       , //얼어있음.
        Barcode      , //바코드 필요.
        Work         , //바코드찍고 작업준비완료.
        Empty        , //비어있음.
        Clean        , //바늘씻기.
        Waste        , //남은거 버리기.
        Ready        , //챔버에 용액 공급 한후.
        Fill         , //Ready->Fill->Work 챔버에 혈액공급후 마져 채우기.
        WorkEnd      , //검사완료자제.
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
        LDR_XBelt = 0 , //로더 벨트X
        PKR_YSutl = 1 , //셔틀 Y
        PKR_ZPckr = 2 , //픽커 Z
        LDR_YBelt = 3 , //로더 벨트Y
        SYR_XSyrg = 4 , //실린지 X
        SYR_ZSyrg = 5 , //실린지 Z
        MAX_MOTR
    };

    //ai<파트명3자리>_<부가명칭><FWD시에위치2자리><BWD시에위치2자리>
    //대기가 Bwd 작동이 Fwd
    //장비 전면 기준으로 앞에 있는게 Front(Ft), 뒤에가 Rear(Rr)
    public enum ci : uint
    {
        LDR_WorkStprRrFt    =  0  , //로더 워크랙 스토퍼
        LDR_OutPshrFtRr     =  1  , //로더 아웃 푸셔.
        PKR_PkrRrFt         =  2  , //픽커
        PKR_PkrClampClOp    =  3  , //픽커그립
        PKR_PkrShakeUpDn    =  4  , //픽커 쉐이크 
        QCB_RtPckCmpClOp    =  5  , //버퍼 픽커 글래프 클로즈.
        QCB_RtPckCmpDnUp    =  6  , //버퍼 픽커 클램프 업
        QCB_RtPckCmpLtRt    =  7  , //버퍼 픽커 클램프 왼쪽
        QCB_LtPckCmpClOp    =  8  , //QC 픽커 클램프 클로즈.
        QCB_LtPckCmpDnUp    =  9  , //QC 픽커 클램프 업
        QCB_LtPckCmpLtRt    = 10  , //QC 픽커 클램프 왼쪽
        QCB_BfStg1stFtRr    = 11  , //버퍼스테이지 프론트
        QCB_BfStg2ndFtRr    = 12  , //버퍼스테이지 미들
        CHA_MixCoverOpCl    = 13  , 
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
        ETC_EmgSw            = 0   , //0x000  비상정지. 
        LDR_InNotArrival     = 1   , //0x001  로더투입완료 실린더
        LDR_WorkRackExist    = 2   , //0x002  워크존로더렉유무센서
        Spear03              = 3   , //0x003  워크존로더렉스토퍼Fw
        Spear04              = 4   , //0x004  워크존로더렉스토퍼Bw
        LDR_OutNotArrival    = 5   , //0x005  로더작업완료존 도착센서.
        LDR_OutPshrBw        = 6   , //0x006  로더 아웃존 아웃푸셔Bw 
        LDR_OutPshrFw        = 7   , //0x007  로더 아웃존 아웃푸셔Fw
        LDR_OutNotFull       = 8   , //0x008  로더 아웃버퍼 풀센서
        PKR_PckBw            = 9   , //0x009  픽커 Bw 
        PKR_PckFw            = 10  , //0x00A  픽커 Fw
        Spear11              = 11  , //0x00B  픽커 Up
        Spear12              = 12  , //0x00C  픽커 Dn
        Spear13              = 13  , //0x00D  픽커 클램프 클로즈.
        Spear14              = 14  , //0x00E  픽커 클래프 오픈.
        SYR_MixCoverCl       = 15  , //0x00F  믹싱 챔버 뚜껑 클로즈.
        SYR_MixCoverOp       = 16  , //0x010  믹싱 챔버 뚜껑 오픈.
        Spear17              = 17  , //0x011  버퍼 픽커 글래프 클로즈.
        Spear18              = 18  , //0x012  버퍼 픽커 클램프 오픈
        QCB_RtPckCmpUp       = 19  , //0x013  버퍼 픽커 클램프 업
        QCB_RtPckCmpDn       = 20  , //0x014  버퍼 픽커 클램프 다운.
        QCB_RtPckCmpLt       = 21  , //0x015  버퍼 픽커 클램프 왼쪽
        QCB_RtPckCmpRt       = 22  , //0x016  버퍼 픽커 클램프 오른쪽.
        Spear23              = 23  , //0x017  QC 픽커 클램프 클로즈.
        Spear24              = 24  , //0x018  QC 픽커 클램프 오픈
        QCB_LtPckCmpUp       = 25  , //0x019  QC 픽커 클램프 업
        QCB_LtPckCmpDn       = 26  , //0x01A  QC 픽커 클램프 다운.
        QCB_LtPckCmpRt       = 27  , //0x01B  QC 픽커 클램프 오른쪽
        QCB_LtPckCmpLt       = 28  , //0x01C  QC 픽커 클램프 .왼쪽
        QCB_BfStgRear        = 29  , //0x01D  버퍼스테이지 리어
        QCB_BfStgMidl        = 30  , //0x01E  버퍼스테이지 미들
        QCB_BfStgFrnt        = 31  , //0x01F  버퍼스테이지 프론트
        SYR_ChbrPres1        = 32  , //0x020  챔버 압력 센서
        SYR_ChbrPres2        = 33  , //0x021  챔버 압력 센서
        SYR_ChbrPres3        = 34  , //0x022  챔버 압력 센서
        SYR_ChbrPres4        = 35  , //0x023  챔버 압력 센서
        SYR_ChbrPres5        = 36  , //0x024  챔버 압력 센서
        SYR_ChbrPres6        = 37  , //0x025  챔버 압력 센서
        SYR_HeatUpLmt3       = 38  , //0x026  히터 온도상한 알람.
        SYR_HeatUpLmt4       = 39  , //0x027  히터 온도상한 알람.
        SYR_HeatUpLmt5       = 40  , //0x028  히터 온도상한 알람.
        SYR_HeatUpLmt6       = 41  , //0x029  히터 온도상한 알람.
        LDR_WorkTubeExist    = 42  , //0x02A  튜브감지 하는놈.
        CP_1NotFull          = 43  , //0x02B  탱크 Full CP_1     
        CP_2NotFull          = 44  , //0x02C  탱크 Full CP_2     
        CP_3NotFull          = 45  , //0x02D  탱크 Full CP_3     
        CS_FNotFull          = 46  , //0x02E  탱크 Full CS_Front 
        CS_RNotFull          = 47  , //0x02F  탱크 Full CS_Rear  
        SULFNotFull          = 48  , //0x030  탱크 Full SULF     
        FBNotFull            = 49  , //0x031  탱크 Full FB       
        FDLNotFull           = 50  , //0x032  탱크 Full 4DL      
        RETNotFull           = 51  , //0x033  탱크 Full RET      
        NRNotFull            = 52  , //0x034  탱크 Full NR       
        FCMWasteNotFull      = 53  , //0x035  탱크 Full FCMWaste 
        WasteNotFull         = 54  , //0x036  탱크 Full Waste    
        Spear55              = 55  , //0x037
        Spear56              = 56  , //0x038
        Spear57              = 57  , //0x039
        Spear58              = 58  , //0x03A
        Spear59              = 59  , //0x03B
        Spear60              = 60  , //0x03C
        Spear61              = 61  , //0x03D
        Spear62              = 62  , //0x03E
        Spear63              = 63  , //0x03F

        MAX_INPUT
    };

    //그냥 아이오31.  <파트 3자리>+_+<세부설명>
    //복동 실린더 아이오 <파트 3자리>+_+<세부설명>+<해당행동 ex) Fw , Bw , Dn 등등 2자리>     자리
    //단동 실린더 아이오 <파트 3자리>+_+<세부설명>+<Fw시에해당행동2자리><Bw시에해당행동2자리> 동
    public enum yi
    {
        ETC_TwRed            = 0   , //0x000  타워램프 RED
        ETC_TwYellow         = 1   , //0x001  타워램프 YELLOW
        ETC_TwGreen          = 2   , //0x002  타워램프 GREEN
        ETC_TwBuzz1          = 3   , //0x003  타워램프 BUZZER1
        ETC_TwBuzz2          = 4   , //0x004  타워램프 BUZZER2
        ETC_TwBuzz3          = 5   , //0x005  타워램프 BUZZER3
        ETC_TwBuzz4          = 6   , //0x006  타워램프 BUZZER4
        LDR_WorkStprFw       = 7   , //0x007  로더워크존 렉스토퍼 FW
        LDR_OutPshrFw        = 8   , //0x008  로더아웃푸셔 FW
        PKR_PckBw            = 9   , //0x009  픽커 Bw
        PKR_PckShakeUp       = 10  , //0x00C  믹스챔버 뚜껑 Open
        PKR_PckClampCl       = 11  , //0x00B  픽커 클램프 Close
        SYR_MixCoverOp       = 12  , //0x00A  픽커 쉐이커 Up
        QCB_RtPckCmpCl       = 13  , //0x00D  버퍼 픽커 클램프 Close
        QCB_RtPckCmpDn       = 14  , //0x00E  버퍼 픽커 클램프 Down
        QCB_RtPckCmpLt       = 15  , //0x00F  버퍼 픽커 클램프 Left
        QCB_LtPckCmpLt       = 16  , //0x010  QC 픽커 클램프 
        QCB_LtPckCmpDn       = 17  , //0x011  QC 픽커 글래프 클로즈.
        QCB_LtPckCmpCl       = 18  , //0x012  QC 픽커 클램프 오픈
        QCB_BfStgFrstFw      = 19  , //0x013  QC 스테이지 1st Fw     
        QCB_BfStgScndFw      = 20  , //0x014  QC 스테이지 2st Fw
        QCB_BfStgScndBw      = 21  , //0x015  QC 스테이지 3st Bw
        ETC_MainPumpOff      = 22  , //0x016  외장펌프 Off AutoQC에 쓴다.
        PKR_BarcodeSpin      = 23  , //0x017  바코드 리딩부 모터 회전.
        CHA_FcmLaserOn       = 24  , //0x018  FCM 레이져 온.
        Spare25              = 25  , //0x019  
        Spare26              = 26  , //0x01A  
        Spare27              = 27  , //0x01B  
        Spare28              = 28  , //0x01C  
        Spare29              = 29  , //0x01D  
        Spare30              = 30  , //0x01E  
        Spare31              = 31  , //0x01F       

        //솔 네이밍 : 목적위치 , 움직이는물체 , 온상태 , 오프상태.
        //======================================여기까지는 탱크쪽=====================
        TankCP1_InSt         = 32  , //0x020 01    탱크 시약 인 스탑
        TankCP2_InSt         = 33  , //0x021 02    탱크 시약 인 스탑
        TankCP3_InSt         = 34  , //0x022 03    탱크 시약 인 스탑
        TankCSF_InSt         = 35  , //0x023 04    탱크 시약 인 스탑
        TankCSR_InSt         = 36  , //0x024 05    탱크 시약 인 스탑
        TankSULF_InSt        = 37  , //0x025 06    탱크 시약 인 스탑 
        TankFB_InSt          = 38  , //0x026 07    탱크 시약 인 스탑 
        Tank4DL_InSt         = 39  , //0x027 08    탱크 시약 인 스탑 
        TankRET_InSt         = 40  , //0x028 09    탱크 시약 인 스탑 
        TankNR_InSt          = 41  , //0x029 10    탱크 시약 인 스탑 
        TankCP1_VcAr         = 42  , //0x02A 11    탱크 에어 정압 배큠
        TankCP2_VcAr         = 43  , //0x02B 12    탱크 에어 정압 배큠
        TankCP3_VcAr         = 44  , //0x02C 13    탱크 에어 정압 배큠
        TankCSF_VcAr         = 45  , //0x02D 14    탱크 에어 정압 배큠
        TankCSR_VcAr         = 46  , //0x02E 15    탱크 에어 정압 배큠
        TankSULF_VcAr        = 47  , //0x02F 16    탱크 에어 정압 배큠
        TankFB_VcAr          = 48  , //0x030 17    탱크 에어 정압 배큠
        Tank4DL_VcAr         = 49  , //0x031 18    탱크 에어 정압 배큠
        TankRET_VcAr         = 50  , //0x032 19    탱크 에어 정압 배큠
        TankNR_VcAr          = 51  , //0x033 20    탱크 에어 정압 배큠
        TankCP1_OtSt         = 52  , //0x034 21    탱크 시약 아웃 스탑
        TankCP2_OtSt         = 53  , //0x035 22    탱크 시약 아웃 스탑
        TankCP3_OtSt         = 54  , //0x036 23    탱크 시약 아웃 스탑
        TankCSF_OtSt         = 55  , //0x037 24    탱크 시약 아웃 스탑
        TankCSR_OtSt         = 56  , //0x038 25    탱크 시약 아웃 스탑
        TankSULF_OtSt        = 57  , //0x039 26    탱크 시약 아웃 스탑
        TankFB_OtSt          = 58  , //0x03A 27    탱크 시약 아웃 스탑
        Tank4DL_OtSt         = 59  , //0x03B 28    탱크 시약 아웃 스탑
        TankRET_OtSt         = 60  , //0x03C 29    탱크 시약 아웃 스탑
        TankNR_OtSt          = 61  , //0x03D 30    탱크 시약 아웃 스탑
        SpareSol1            = 62  , //0x03E 31
        SpareSol2            = 63  , //0x03F 32


        //======================================여기부턴 챔버 인 아웃.====================
        //CP3 250ml 는 모두 청소용.
        NidlCP3_InSt         = 64  , //0x040 33   니들 CP3 인/스탑.
        ClenCP3_InSt         = 65  , //0x041 34   클린존 CP3 탱크에서 공급함.
        Cmb1CP3_InSt         = 66  , //0x042 35   챔버1에 CP3 탱크에서 공급함.
        Cmb2CP3_InSt         = 67  , //0x043 36   챔버2에 CP3 탱크에서 공급함.
        Cmb3CP3_InSt         = 68  , //0x044 37   챔버3에 CP3 탱크에서 공급함.
        Cmb4CP3_InSt         = 69  , //0x045 38   챔버4에 CP3 탱크에서 공급함.
        Cmb5CP3_InSt         = 70  , //0x046 39   챔버5에 CP3 탱크에서 공급함.
        Cmb6CP3_InSt         = 71  , //0x047 40   챔버6에 CP3 탱크에서 공급함.
        Cmb1CP2_InSt         = 72  , //0x048 41   챔버1에 CP2 탱크에서 공급함.
        Cmb2CP2_InSt         = 73  , //0x049 42   챔버2에 CP2 탱크에서 공급함.
        Cmb1Air_InVt         = 74  , //0x04A 43   챔버에 Air 공급 & 벤트.
        Cmb2Air_InVt         = 75  , //0x04B 44   챔버에 Air 공급 & 벤트.
        Cmb3Air_InVt         = 76  , //0x04C 45   챔버에 Air 공급 & 벤트.
        Cmb4Air_InVt         = 77  , //0x04D 46   챔버에 Air 공급 & 벤트.
        Cmb5Air_InVt         = 78  , //0x04E 47   챔버에 Air 공급 & 벤트.
        Cmb6Air_InVt         = 79  , //0x04F 48   챔버에 Air 공급 & 벤트.

        Cmb1Out_VcOt         = 80  , //0x050 49   챔버 아웃쪽에 3웨이 밸브 아웃/배큠. 노멀일때 검사기쪽 온일때 배큠.
        Cmb2Out_VcOt         = 81  , //0x051 50   챔버 아웃쪽에 3웨이 밸브 아웃/배큠.
        Cmb3Out_VcOt         = 82  , //0x052 51   챔버 아웃쪽에 3웨이 밸브 아웃/배큠.
        Cmb4Out_VcOt         = 83  , //0x053 52   챔버 아웃쪽에 3웨이 밸브 아웃/배큠. 
        Cmb5Out_VcOt         = 84  , //0x054 53   챔버 아웃쪽에 3웨이 밸브 아웃/배큠. 
        Cmb6Out_VcOt         = 85  , //0x055 54   챔버 아웃쪽에 3웨이 밸브 아웃/배큠. 
        Cmb1Out_OtSt         = 86  , //0x056 55   챔버 아웃쪽에 핀치벨브 제어 2웨이 밸브 핀치/릴리즈  
        Cmb2Out_OtSt         = 87  , //0x057 56   챔버 아웃쪽에 핀치벨브 제어 2웨이 밸브 핀치/릴리즈 
        Cmb3Out_OtSt         = 88  , //0x058 57   챔버 아웃쪽에 핀치벨브 제어 2웨이 밸브 핀치/릴리즈 
        Cmb4Out_OtSt         = 89  , //0x059 58   챔버 아웃쪽에 핀치벨브 제어 2웨이 밸브 핀치/릴리즈 
        Cmb5Out_OtSt         = 90  , //0x05A 59   챔버 아웃쪽에 핀치벨브 제어 2웨이 밸브 핀치/릴리즈 
        Cmb6Out_OtSt         = 91  , //0x05B 60   챔버 아웃쪽에 핀치벨브 제어 2웨이 밸브 핀치/릴리즈 
        ClenOut_OtSt         = 92  , //0x05C 61   클린존 아웃쪽 밸브. 
        NidlOut_OtSt         = 93  , //0x05D 62   니들클린 아웃쪽 밸브.
        HGBOut_OtSt          = 94  , //0x05E 63   챔버 아웃쪽 HGB다음 2웨이 밸브 스탑/아웃. 
        SpareSol4            = 95  , //0x05F 64   

        FCMOut_OtSt          = 96  , //0x060 65   FCM검사후 아웃벨브 
        FCMWOut_OtSt         = 97  , //0x061 66   FCM Waste 아웃 스탑.
        Dc1Air_InVt          = 98  , //0x062 67   DC1 에어 인스탑.
        Dc2Air_InVt          = 99  , //0x063 68   DC2 에어 인스탑.
        Dcc1Vcm_InSt         = 100 , //0x064 79   DCC1 배큠 인스탑.
        Dcc2Vcm_InSt         = 101 , //0x065 70   DCC2 배큠 인스탑.
        Dcc3CSf_InSt         = 102 , //0x066 71   DCC3 C/S 프론트 인스탑.
        Dcc4CSR_InSt         = 103 , //0x067 72   DCC4 C/S 리어 인스탑.
        Dcc5CP2_InSt         = 104 , //0x068 73   DCC5 CP2 인스탑.
        WastOut_OtSt         = 105 , //0x069 74   Waste Out / Stop   
        WastAir_InVc         = 106 , //0x06A 75   Waste Air / Vaccum.
        Sol75                = 107 , //0x06B 76  
        Sol76                = 108,  //0x06C 77  TankCP1_InSt Sol76
        Spare109             = 109 , //0x06D
        Spare110             = 110 , //0x06E
        Spare111             = 111 , //0x06F

        Spare112             = 112 , //0x070
        Spare113             = 113 , //0x071
        Spare114             = 114 , //0x072
        Spare115             = 115 , //0x073
        Spare116             = 116 , //0x074
        Spare117             = 117 , //0x075
        Spare118             = 118 , //0x076
        Spare119             = 119 , //0x077
        Spare120             = 120 , //0x078
        Spare121             = 121 , //0x079
        Spare122             = 122 , //0x07A
        Spare123             = 123 , //0x07B
        Spare124             = 124 , //0x07C
        Spare125             = 125 , //0x07D
        Spare126             = 126 , //0x07E
        Spare127             = 127 , //0x07F

        MAX_OUTPUT
    };

    /*
             LDR_XBelt = 0 ,
        PKR_YSutl = 1 ,
        PKR_ZPckr = 2 ,
        LDR_YBelt = 3 ,
        SYR_XSyrg = 4 ,
        SYR_ZSyrg = 5 ,
         */
    //Position Value id
    public enum pv : uint
    {
        LDR_XBeltPitch          = 0 , //PRER_X
        MAX_PSTN_MOTR0              ,

        PKR_YSutlSyinge         = 0 ,
        PKR_YSutlBuffer             ,
        PKR_YSutlPicker             ,
        MAX_PSTN_MOTR1              ,        

        PKR_ZPckrShake          = 0 ,
        PKR_ZPckrRackPlce           ,
        PKR_ZPckrRackPick           ,
        PKR_ZPckrSuttlePlce         ,
        PKR_ZPckrSuttlePick         ,
        MAX_PSTN_MOTR2              ,

        LDR_YBeltWorkStart      = 0 ,
        LDR_YBeltWorkEnd            ,
        MAX_PSTN_MOTR3              ,
                
        SYR_XSyrgSuttle         = 0 ,
        SYR_XSyrgClean              , 
        SYR_XSyrgChamberStt         ,
        SYR_XSyrgChamberPitch       ,
        MAX_PSTN_MOTR4              ,
        
        SYR_ZSyrgMove           = 0 ,
        SYR_ZSyrgSuttleLid          ,
        SYR_ZSyrgSuttle             ,
        SYR_ZSyrgChamber            ,
        MAX_PSTN_MOTR5

       
    };

    public class Pstn
    {
        public static uint[] Cnt = { (uint)pv.MAX_PSTN_MOTR0 , //1,3,3,2,4,3
                                     (uint)pv.MAX_PSTN_MOTR1 ,
                                     (uint)pv.MAX_PSTN_MOTR2 ,
                                     (uint)pv.MAX_PSTN_MOTR3 ,
                                     (uint)pv.MAX_PSTN_MOTR4 ,
                                     (uint)pv.MAX_PSTN_MOTR5 };// , (uint)pv.MAX_PSTN_MOTR1 , (uint)pv.MAX_PSTN_MOTR2};
    }

    

    //Manual Cycle.
    public enum mc : uint
    {
        NoneCycle             = 0  ,
        AllHome               = 1  ,

        LDR_CycleHome         = 10 ,
        LDR_CycleSupply       = 11 ,
        LDR_CycleMove              ,           
        LDR_CycleOut               , 
        LDR_CycleOutPush           ,                 

        PKR_CycleHome         = 20 ,
        PKR_CyclePickLdr      = 21 ,
        PKR_CycleShake             ,
        PKR_CyclePlceSut           ,
        PKR_CycleBarcode           ,
        PKR_CyclePickSut           ,
        PKR_CyclePlceLdr           ,

        QCB_CycleHome         = 30 , 
        QCB_CycleRtPickFrg    = 31 ,
        QCB_CycleRtPlceBfr         ,
        QCB_CycleUnFreeze          ,
        QCB_CycleLtPickBfr         ,
        QCB_CycleLtPlceSut         ,
        QCB_CycleLtPickSut         ,
        QCB_CycleLtPlceBfr         ,
        QCB_CycleRtPickBfr         ,
        QCB_CycleRtPlceFrg         ,

        SYR_CycleHome         = 40 , 
        SYR_CycleSuck              ,
        SYR_CycleSupply            ,
        SYR_CycleClean             ,
        SYR_CycleReadyBlood        ,

        CHA_CycleHome         = 50 ,
        CHA_CycleFillTank          ,
        CHA_CycleFillChamber       ,
        CHA_CycleInspChamber       ,
        CHA_CycleEmptyChamber      ,
        CHA_CycleCleanChamber      ,
        CHA_CycleReadyDC           ,
        CHA_CycleReadyFCM          ,
        CHA_CycleReadyNR           ,
        CHA_CycleReadyRET          ,
        CHA_CycleReady4DL     = 60 ,
        CHA_CycleTimeSupply        ,
        CHA_CycleCleanDcc          ,
        CHA_CycleInspDC            ,

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
        ETC_014             ,
        MTR_HomeEnd         ,
        MTR_NegLim          ,
        MTR_PosLim          ,
        MTR_Alarm           ,
        ETC_019             ,
        ATR_TimeOut         ,
        ETC_021             ,
        PKG_Dispr           , 
        PKG_Unknwn          ,
        ETC_024             , //--------------------------------- 장비공통에러.
        ETC_025             , 
        LDR_NeedLoading     , //로더에 자제 필요.
        LDR_UnloaderFull    , //언로더 저제 만땅.
        ETC_028             ,
        ETC_029             , //PCK_PickMiss        , //픽커 튜브 픽킹 실패
        ETC_030             , //SYR_ChamberCover    , //챔버 커버 리크발생.
        SYR_ChamberTemp     , //온도 컨트롤러 에러.
        PKR_Barcode         , //바코드 관련 에러.
        CHA_TankFull        ,
        ETC_Communicate     , //각종 통신 에러.
        ETC_035             ,
        ETC_036             , 
        ETC_037             ,
        ETC_038             ,
        ETCL039             , 
        ETC_040             , 
        ETC_041             , 
        ETC_042             ,
        ETC_043             ,
        ETC_044             ,
        MAX_ERR    
    };

    //Error Id

}





