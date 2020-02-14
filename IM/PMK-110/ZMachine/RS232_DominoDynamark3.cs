using COMMON;
using SML;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Machine
{
    /*
    도미노사의 DynaMark3프로토콜 여러가지 레이져 마킹기에서 쓰이는듯. F220i 파이버 레이져 모델에서 씀.
    도미노 마커세팅에서 PrintNetwork인가 로 들어가면 RS232세팅이 있는데 거기서 ECHO를 체크 해지 해야 한다.
    이거 켜져 있으면 내가 보낸 메세지를 한번 다시 보내고 하기 때문에 번거러움.

    GETCURRENTPROJECT     => 현재 로딩되어 있는 프로젝트 리턴.
    LOADPROJECT "store:MsgStore1/Format1" => MsgStore1번저장소에 Format1을 로딩해라. 이걸 로딩하면 프린터는 대기 상태로 전환됨.
    MARK START    => 대기 상태로 전환된걸 다시 마킹 가능 상태로 전환.
    BEGINTRANS    => 트렌젝션 시작.
    SETTEXT "LotNo" "LotNo0000" => LotNo로 ID를 찾아 내용을 LotNo0000로 바꾼다.
    SETTEXT "SerialNo" "000001" => SerialNo로 ID를 찾아 내용을 000001로 바꾼다.
    SETTEXT "DMC" "LotNo0000 000001" => DCM로 되어 있는 ID를 찾아 내용을 LotNo0000 000001로 바꾼다.
    EXECTRANS

    TRIGGER   레이저가 READY 상태에서 이걸 날리면 마킹을 한다.
    SETMSG 1 1    작업 가능 상태가 되면 레이져에서 MSG 1 이라고 온게 설정 한다. 설정을 하면 위의 순서에서 EXECTRANS에서 날라온다.
    SETMSG 2 1    프린팅이 시작 되면 MSG 2 라고 날라오게 설정함.
    */
    /*
        안녕하세요.도미노코리아 이도한 입니다.
    
    1. 이 마킹을 한번에 10X10=100ea를 해야 하고 LotNo는 바뀌지 않지만 SerialNo는 100개가 1씩 증가 해야 합니다.
    이경우 트렌젝션안의 SetText를 100번을 전송해야 하나요?
    
    •	“BUFFERDATA” 명령을 사용해서 데이터를 한 번에 전송할 수 있습니다.
    SETTEXT : 고정 데이터 전송
    BUFFERDATA : 가변 데이터 전송 (ex.일련번호)
    
    2. SETMSG의 경우 해당 메시지의 활성화 기능인 듯 한데 위의 사이클을 1트레이 마킹 시 마다 전송해도 문제 없는지요?
    
    •	“SETMSG” 명령은 Laser Controller 로부터 해당 Message 에 대한 Event 를 받을 지 유무를 설정 합니다.
    “SETMSG” 명령은 “LOADPROJECT” 명령 전에 실행하면 됩니다.
    
    아래 절차대로 진행해 보십시오.
    1.	MARK STOP
    2.	SETMSG
    3.	LOADPROJECT
    4.	MARK START
    5.	BUFFERDATA (*** 다음 트레이 부터는 이 명령만 계속 사용해서 데이터 전송합니다)
    
    SETMSG<MsgID> <Mode>
    MsgID :
    ReadyToPrint = 1,
    EndOfAPrint = 3,
    NotReadyToPrint = 4,
    StatusChanged = 5
    LoadedMessage = 18
    DataCoding = 25 (* 저는 인쇄 완료를 이 Event로 판단합니다)
    
    3. 심민욱 차장님 말씀은 DMC 코드가 Text와 연계 되어 있어 따로 SETTEXT를 할 필요 없이 링크가 가능 하다고 하시던데 어떻게 구성을 해야 하나요?
    •	바코드 데이터의 경우, 외부 변수와 링크를 걸어서 데이터를 가져옵니다.
    디자인 편집에서 구성할 수 있는데, 이 부분은 도미노 프린터 엔지니어에게 문의 부탁 드립니다. (심민욱 차장님)
    
    4. 첫번째 메시지  "SETMSG 1 1" 부터 "EXECTRANS"까지 한줄 씩이 아닌 한번에 연계해서 보내도 괜찮나요?
        ex) SETMSG 1 1\r\nSETMSG 3 1\r\nLOADPROJECT "store:MsgStore1/Format1"\r\nMARK START\r\nBEGINTRANS\r\n …………………
    
    •	여러 명령을 한 번에 실행하면 안 됩니다.한 개씩 실행해 주십시오.



    20190910
    안녕하세요 한라정밀 선계원 입니다.

메일 답변과 통화로 설명 들은 데로 프로그램 작성 중에 있습니다.

작업 진행 시 통신을 해야 하는 시점 별 정리 인데 2가지 부분에서 막힘이 있어 메일 드립니다.

17일날 셋업 지원 오실 예정인데 이 부분을 작업해 두지 않으면 17일날 작업이 완료 되지 않을 것 같아 걱정이 됩니다.

	장비 켰을 때 , 작업자가 장비의 작업자제를 다른 제품으로 변경 시  전송 
MARK STOP
SETMSG 1
SETMSG 3
LOADPROJECT \"store:…..
MARK START
문제 없음.

	작업자가 제품 작업 시작 시에 Lot No를 장비PC에 입력 했을 때 해당 제품 작업종료 시 까지 고정적으로 사용할 LotNo 세팅.
 
제품작업 시작 시에 모든 물량은 똑 같은  LotNo이므로 미리 100개의 LotNo를 바꾸고 싶습니다.
제가 파악하기로는 SetBuffer를 이용 하면 단순히 Trigger를 할 때 마다 들어있던 큐에서 하나씩 빼서 작업 하는 것 같은데 
미리 다 바꾸는 것은 SetBuffer로는 안 되는 거겠지요?
SetText를 100회로 하면 느리거나 레이져가 문제가 생긴다고 하셨는데 다른 방안 있을까요?
혹시 트렌젝션 기능을 이용해서 하면 될까요?

	Serial No 통신으로 세팅 하여 트레이 마킹 준비 
 

BufferData에 대해 작성 중에 매뉴얼에 보면 1~10개의 텍스트만 삽입 할 수 있다고 나와 있는데 현재 
저희는 몇 개가 될지는 모르지만 대략 100ea의 마킹을 진행 해야 하는 상황 입니다.
어제 통화에서 STCOUNTVALUE를 사용 하라 하셨는데 매뉴얼 확인 결과 내용이 자세하지 않고 마킹디자이너와 연계되어 있는 부분도 있는 것 같아서 좀 더 자세한 설명을 부탁 드립니다.

	마킹
TRIGGER
문제 없음.



 안녕하세요. 도미노코리아 이도한 입니다.
“SetText” 명령을 사용해야 할 것 같습니다.
“STCOUNTVALUE” 명령은 이 경우 적절하지 않을 듯 합니다. (인쇄 완료 시점에 Count 가 올라감)

Lot No. 의 경우 1개 트레이 에서 모두 동일한 값입니다.
디자인 구성 시 한 개의 Lot 변수에 링크를 걸고, 나머지 99개의 Lot 변수는 링크 기능을 통해 값을 끌어 오는 형태로 하면 될 것 같습니다.

Serial No. 의 경우 디자인 구성 시, 100개의 변수를 만듭니다. 100번의 “SetText” 명령을 통해 데이터를 넣어줍니다.

1개 트레이 에서,

BEGINTRANS
SetText(Lot)
SetText(Serial 1)
SetText(Serial 2)
SetText(Serial 3)
...
...
SetText(Serial 100)
EXECTRANS

이렇게 하면 될 것 같습니다.
Loof 를 돌면서 “SetText” 명령을 여러 번 실행하기 때문에 Sleep 을 주면서 실행해야 합니다.


    */

    public class RS232_DominoDynamark3
    {
        public enum Cycle
        {
            None        = 0,
            ProjectLoad = 1 , //도미노 레이저 해당 프로젝트 파일 로딩. 잡파일 다운로드 혹은 장비 투스타트, 프로그램 켤때 사용.
            SetLotNo    = 2 , //랏넘버.
            SetSerialNo = 3 , //매번 자제의 시리얼 넘버를 바꿔 줘야 한다.
            Mark        = 4 , //레이저 마킹 시작.
        }

        private string      m_sText   = "";
        private string      m_sName   = "";
        private int         m_iPortNo = 0 ;
        private SerialPort  Port      = new SerialPort() ;
        private bool        m_bRcvEnded = false ;
                            
        private int         m_iSendStep = 0 ;
        private int         m_iPreSendStep = 0 ;
        private Cycle       m_eCycle       = Cycle.None ;
        private string      m_sErr = "";
        private CDelayTimer m_tmCycle = new CDelayTimer();
        private CDelayTimer m_tmDelay = new CDelayTimer();

        private bool        m_bShowErr = false ; //UI에서 보낼때는 자체적으로 에러를 띄워야 하고 시퀜스에서 보낼때는 밖에서 에러 띄움.
        //private int

        //
        public RS232_DominoDynamark3(int _iPortId , string _sName)
        {
            m_iPortNo = _iPortId ;
            m_sName   = _sName   ;

            Port.PortName     = "Com" + _iPortId.ToString();
            Port.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
            //Port.Encoding = System.Text.Encoding.GetEncoding("iso-8859-1");//이것이 8비트 문자 모두 가능 Ascii는 7비트라 63이상의 값은 표현 안됌.
            Port.BaudRate     = 9600; 
            Port.DataBits     = 8   ; 
            Port.Parity       = Parity  .None;//None = 0,Odd = 1,Even = 2,Mark = 3 ,Space = 4, 
            Port.StopBits     = StopBits.One ;//None = 0,One = 1,Two  = 2,OnePointFive = 3, None은 예외상황발생함..
            Port.ReadTimeout  = (int)500;
            Port.WriteTimeout = (int)500;
            Port.Handshake    = Handshake.None;

            try
            {
                Port.Open();    
            }
            catch
            {
                Log.ShowMessage("Error","Could not open the port " + Port.PortName);
            }
            

        }
        ~RS232_DominoDynamark3()
        {
            Port.Close();
        }
        public string GetErr()
        {
            return m_sErr ;
        }
        public void SetCycle(Cycle _eCycle , bool _bShowErr = false )
        {
            m_iSendStep = 10 ;
            m_eCycle = _eCycle;
            m_sErr = "";
            m_bShowErr = _bShowErr;
        }

        public Cycle GetCycle()
        {
            return m_eCycle ;
        }

        public bool GetCycleEnded()
        {
            return m_iSendStep == 0 ;
        }

        public void Update()
        {
            if(m_eCycle == Cycle.None) return ;

            if (m_eCycle == Cycle.ProjectLoad && m_iSendStep == 10 ) { Log.TraceListView("Laser InitProject Started"); }
            if (m_eCycle == Cycle.SetLotNo    && m_iSendStep == 10 ) { Log.TraceListView("Laser SetLotNo Started"   ); }
            if (m_eCycle == Cycle.SetSerialNo && m_iSendStep == 10 ) { Log.TraceListView("Laser SetSerialNo Started"); }
            if (m_eCycle == Cycle.Mark        && m_iSendStep == 10 ) { Log.TraceListView("Laser Marking Started"    ); }

            if (m_eCycle == Cycle.ProjectLoad && CycleInitProject()) { m_eCycle = Cycle.None ; Log.TraceListView("Laser InitProject Ended:"+ m_sErr); }
            if (m_eCycle == Cycle.SetLotNo    && CycleSetLotNo   ()) { m_eCycle = Cycle.None ; Log.TraceListView("Laser SetLotNo Ended:"   + m_sErr); }
            if (m_eCycle == Cycle.SetSerialNo && CycleSetSerialNo()) { m_eCycle = Cycle.None ; Log.TraceListView("Laser SetSerialNo Ended:"+ m_sErr); }
            if (m_eCycle == Cycle.Mark        && CycleMark       ()) { m_eCycle = Cycle.None ; Log.TraceListView("Laser Marking Ended:"    + m_sErr); }

        }

        bool CycleInitProject()
        {
            if(m_iSendStep == 0 )return true ;

            string sTemp ;

            if (m_tmCycle.OnDelay(m_iSendStep == m_iPreSendStep && !OM.MstOptn.bDebugMode, 50000))
            {
                sTemp = string.Format("CycleInitProject Timeout m_iSendStep={0:00}", m_iSendStep);
                sTemp = m_eCycle.ToString() + sTemp ;
                Log.Trace("DominoDynamark3", sTemp);
                //SM.ERR.SetErr((int)ei.LSR_ComNG, );
                m_sErr = sTemp ;

                if (m_bShowErr) SM.ERR.SetErr((int)ei.LSR_ComNG, sTemp);
                m_iSendStep = 0;
                return true;
            }

            if (m_iSendStep != m_iPreSendStep)
            {
                sTemp = string.Format(" m_iSendStep={0:00}", m_iSendStep);
                sTemp = m_eCycle.ToString() + sTemp;
                Log.Trace("DominoDynamark3", sTemp);
            }

            m_iPreSendStep = m_iSendStep;

            switch (m_iSendStep)
            {

                default:
                    sTemp = string.Format(" Cycle Default Clear m_iSendStep={0:00}", m_iSendStep);
                    sTemp = m_eCycle.ToString() + sTemp;
                    m_sErr = sTemp;
                    Log.Trace("DominoDynamark3", sTemp);
                    m_iSendStep = 0;
                    return true;

                case 0:

                    return false;

                case 10:
                    //마킹 준비 상태로 진입.
                    if(!SendMsg("MARK STOP")) {if(m_bShowErr) SM.ERR.SetErr((int)ei.LSR_ComNG , m_sErr); return true ; }
                    m_iSendStep++;
                    return false;

                case 11:
                    if (!m_bRcvEnded) return false;
                    if (m_sText != "OK")
                    {
                        sTemp = string.Format(" m_iSendStep={0:00}", m_iSendStep);
                        sTemp = m_eCycle.ToString() + sTemp + " m_sText=" + m_sText;
                        m_sErr = sTemp;
                        if (m_bShowErr) SM.ERR.SetErr((int)ei.LSR_ComNG, m_sErr);
                        m_iSendStep = 0;
                        return false;
                    }

                    //마킹 준비 완료 메세지 오게 세팅.
                    if(!SendMsg("SETMSG 1 1")) { if (m_bShowErr) SM.ERR.SetErr((int)ei.LSR_ComNG, m_sErr); return true; }
                    m_iSendStep++;
                    return false;

                case 12:
                    if (!m_bRcvEnded) return false;
                    if (m_sText != "OK")
                    {
                        sTemp = string.Format(" m_iSendStep={0:00}", m_iSendStep);
                        sTemp = m_eCycle.ToString() + sTemp + " m_sText=" + m_sText;
                        m_sErr = sTemp;
                        if (m_bShowErr) SM.ERR.SetErr((int)ei.LSR_ComNG, m_sErr);
                        m_iSendStep = 0;
                        return false;
                    }

                    //마킹 완료 메세지 오게 세팅.
                    if(!SendMsg("SETMSG 3 1")) { if (m_bShowErr) SM.ERR.SetErr((int)ei.LSR_ComNG, m_sErr); return true; }
                    m_iSendStep++;
                    return false;

                case 13:
                    if (!m_bRcvEnded) return false;
                    if (m_sText != "OK")
                    {
                        sTemp = string.Format(" m_iSendStep={0:00}", m_iSendStep);
                        sTemp = m_eCycle.ToString() + sTemp + " m_sText=" + m_sText;
                        m_sErr = sTemp;
                        if (m_bShowErr) SM.ERR.SetErr((int)ei.LSR_ComNG, m_sErr);
                        m_iSendStep = 0;
                        return false;
                    }

                    //마킹 데이터 다 바뀌면 26번 메세지 오게.
                    if (!SendMsg("SETMSG 26 1")) { if (m_bShowErr) SM.ERR.SetErr((int)ei.LSR_ComNG, m_sErr); return true; }
                    m_iSendStep++;
                    return false;

                case 14:
                    if (!m_bRcvEnded) return false;
                    if (m_sText != "OK")
                    {
                        sTemp = string.Format(" m_iSendStep={0:00}", m_iSendStep);
                        sTemp = m_eCycle.ToString() + sTemp + " m_sText=" + m_sText;
                        m_sErr = sTemp;
                        if (m_bShowErr) SM.ERR.SetErr((int)ei.LSR_ComNG, m_sErr);
                        m_iSendStep = 0;
                        return false;
                    }

                    //해당 잡파일용 프로젝트 로딩.
                    sTemp = $"LOADPROJECT \"store:";


                    sTemp += OM.DevOptn.sLaserProject;
                    sTemp += $"\"";
                    if(!SendMsg(sTemp)) { if (m_bShowErr) SM.ERR.SetErr((int)ei.LSR_ComNG, m_sErr); return true; }
                    m_iSendStep++;
                    return false;

                case 15:
                    if (!m_bRcvEnded) return false;
                    if (m_sText != "OK")
                    {
                        sTemp = string.Format(" m_iSendStep={0:00}", m_iSendStep);
                        sTemp = m_eCycle.ToString() + sTemp + " m_sText=" + m_sText;
                        m_sErr = sTemp;
                        if (m_bShowErr) SM.ERR.SetErr((int)ei.LSR_ComNG, m_sErr);
                        m_iSendStep = 0;
                        return false;
                    }
                    //마킹 완료 메세지 오게 세팅.
                    if(!SendMsg("MARK START")) return true;
                    m_iSendStep++;
                    return false;

                case 16:
                    if (!m_bRcvEnded) return false;
                    if (m_sText != "OK") //"OK" 한번 오고 "MSG 1" 이 옴.
                    {
                        sTemp = string.Format(" m_iSendStep={0:00}", m_iSendStep);
                        sTemp = m_eCycle.ToString() + sTemp + " m_sText=" + m_sText;
                        m_sErr = sTemp;
                        if (m_bShowErr) SM.ERR.SetErr((int)ei.LSR_ComNG, m_sErr);
                        m_iSendStep = 0;
                        return false;
                    }
                    m_sText = "";
                    m_bRcvEnded = false ;
                    m_iSendStep++;
                    return false ;

                    

                case 17:
                    if (!m_bRcvEnded) return false;
                    if (m_sText != "MSG 26") //"OK" 한번 오고 "MSG 26" 이 옴.
                    {
                        sTemp = string.Format(" m_iSendStep={0:00}", m_iSendStep);
                        sTemp = m_eCycle.ToString() + sTemp + " m_sText=" + m_sText;
                        m_sErr = sTemp;
                        if (m_bShowErr) SM.ERR.SetErr((int)ei.LSR_ComNG, m_sErr);
                        m_iSendStep = 0;
                        return false;
                    }
                    m_sText = "";
                    m_bRcvEnded = false;
                    m_iSendStep++;
                    return false;

                case 18:
                    if (!m_bRcvEnded) return false;
                    if (m_sText != "MSG 1") //"MSG 1" 이 옴.
                    {
                        sTemp = string.Format(" m_iSendStep={0:00}", m_iSendStep);
                        sTemp = m_eCycle.ToString() + sTemp + " m_sText=" + m_sText;
                        m_sErr = sTemp;
                        if (m_bShowErr) SM.ERR.SetErr((int)ei.LSR_ComNG, m_sErr);
                        m_iSendStep = 0;
                        return false;
                    }


                    m_iSendStep = 0;
                    return true;
            }

        }

        public int iTrayChipCnt = 0;
        bool CycleSetLotNo()
        {
            if (m_iSendStep == 0) return true;

            string sTemp;

            if (m_tmCycle.OnDelay(m_iSendStep == m_iPreSendStep && !OM.MstOptn.bDebugMode, 50000))
            {
                sTemp = string.Format("CycleSetLotNo TimeOut m_iSendStep={0:00}", m_iSendStep);
                sTemp = m_eCycle.ToString() + sTemp;
                Log.Trace("DominoDynamark3", sTemp);
                //SM.ERR.SetErr((int)ei.LSR_ComNG, );
                m_sErr = sTemp;

                if (m_bShowErr) SM.ERR.SetErr((int)ei.LSR_ComNG, sTemp);
                m_iSendStep = 0;
                return true;
            }

            if (m_iSendStep != m_iPreSendStep)
            {
                sTemp = string.Format(" m_iSendStep={0:00}", m_iSendStep);
                sTemp = m_eCycle.ToString() + sTemp;
                Log.Trace("DominoDynamark3", sTemp);
            }

            m_iPreSendStep = m_iSendStep;

            const bool bUseLink = true ; //디자인에서 링크를 모두 걸어놓으면 1개만 바꿔도 된다고 함.
            int iDeviceCntMax = OM.DevInfo.iTrayColCnt * OM.DevInfo.iTrayRowCnt ;
            switch (m_iSendStep)
            {

                default:
                    sTemp = string.Format(" Cycle Default Clear m_iSendStep={0:00}", m_iSendStep);
                    sTemp = m_eCycle.ToString() + sTemp;
                    m_sErr = sTemp;
                    Log.Trace("DominoDynamark3", sTemp);
                    m_iSendStep = 0;
                    return true;

                case 0:

                    return false;

                case 10:
                    //20190910 일단 100개정도 랏넘버를 바꿔야 하는데 시간이 오래 걸릴수 있어서 그냥 트렌젝션을 써봄.
                    //트렌젝션 시작.
                    //트렌젝션 사용하면 비긴하고 중간에 에러 걸리면 롤백이나 엔드를 해줘야 해서 일단 없앰
                    //if(!SendMsg("BEGINTRANS")) { if (m_bShowErr) SM.ERR.SetErr((int)ei.LSR_ComNG, m_sErr); return true; } //여기서 에러 나면 MARK STOP을 해봐야 함.
                    iTrayChipCnt = 0;
                    m_iSendStep++;
                    return false;

                case 11:
                    //if (!m_bRcvEnded) return false;
                    //if (m_sText != "OK")
                    //{
                    //    sTemp = string.Format(" m_iSendStep={0:00}", m_iSendStep);
                    //    sTemp = m_eCycle.ToString() + sTemp + " m_sText=" + m_sText;
                    //    m_sErr = sTemp;//{if(m_bShowErr) SM.ERR.SetErr((int)ei.LSR_ComNG , m_sErr); return true ; }
                    //    if (m_bShowErr) SM.ERR.SetErr((int)ei.LSR_ComNG, m_sErr);
                    //    m_iSendStep = 0;
                    //    return false;
                    //}

                    if (!SendMsg("SETTEXT L" + iTrayChipCnt +" "+ LOT.GetLotNo())) { if (m_bShowErr) SM.ERR.SetErr((int)ei.LSR_ComNG, m_sErr); return true; }

                    iTrayChipCnt++;
                    if (!bUseLink && iTrayChipCnt < iDeviceCntMax) // 10*10 이면 100번 보낸다.
                    {
                        return false ;
                    }

                    m_iSendStep ++;
                    return false ;

                case 12:
                    if (!m_bRcvEnded) return false;
                    if (m_sText != "OK")
                    {
                        sTemp = string.Format(" m_iSendStep={0:00}", m_iSendStep);
                        sTemp = m_eCycle.ToString() + sTemp + " m_sText=" + m_sText;
                        m_sErr = sTemp;
                        if (m_bShowErr) SM.ERR.SetErr((int)ei.LSR_ComNG, m_sErr);
                        m_iSendStep = 0;
                        return false;
                    }

                    m_bRcvEnded = false ;
                    m_sText = "";
                    //if (!SendMsg("EXECTRANS")) { if (m_bShowErr) SM.ERR.SetErr((int)ei.LSR_ComNG, m_sErr); return true; } //여기서 에러 나면 MARK STOP을 해봐야 함.
                    m_iSendStep++;
                    return false;

                case 13:
                    if (!m_bRcvEnded) return false;
                    if (m_sText != "MSG 26")
                    {
                        sTemp = string.Format(" m_iSendStep={0:00}", m_iSendStep);
                        sTemp = m_eCycle.ToString() + sTemp + " m_sText=" + m_sText;
                        m_sErr = sTemp;
                        if (m_bShowErr) SM.ERR.SetErr((int)ei.LSR_ComNG, m_sErr);
                        m_iSendStep = 0;
                        return false;
                    }

                    m_iSendStep = 0;
                    return true;
            }
        }

        
        bool CycleSetSerialNo1()
        {
            if (m_iSendStep == 0) return true;

            string sTemp;

            if (m_tmCycle.OnDelay(m_iSendStep == m_iPreSendStep && !OM.MstOptn.bDebugMode, 50000))
            {
                sTemp = string.Format("CycleSetSerialNo1 TimeOut m_iSendStep={0:00}", m_iSendStep);
                sTemp = m_eCycle.ToString() + sTemp;
                Log.Trace("DominoDynamark3", sTemp);
                //SM.ERR.SetErr((int)ei.LSR_ComNG, );
                m_sErr = sTemp;
                m_iSendStep = 0;
                return true;
            }

            if (m_iSendStep != m_iPreSendStep)
            {
                sTemp = string.Format(" m_iSendStep={0:00}", m_iSendStep);
                sTemp = m_eCycle.ToString() + sTemp;
                Log.Trace("DominoDynamark3", sTemp);
            }

            m_iPreSendStep = m_iSendStep;


            int iTrayChipCntMax = OM.DevInfo.iTrayColCnt * OM.DevInfo.iTrayRowCnt;

            switch (m_iSendStep)
            {

                default:
                    sTemp = string.Format(" Cycle Default Clear m_iSendStep={0:00}", m_iSendStep);
                    sTemp = m_eCycle.ToString() + sTemp;
                    m_sErr = sTemp;
                    Log.Trace("DominoDynamark3", sTemp);
                    m_iSendStep = 0;
                    return true;

                case 0:
                    return false;

                case 10:
                    //20190910 일단 100개정도 랏넘버를 바꿔야 하는데 시간이 오래 걸릴수 있어서 그냥 트렌젝션을 써봄.
                    //트렌젝션 시작.
                    //if (!SendMsg("BEGINTRANS")) { if (m_bShowErr) SM.ERR.SetErr((int)ei.LSR_ComNG, m_sErr); return true; } //여기서 에러 나면 MARK STOP을 해봐야 함.
                    iTrayChipCnt = 0;
                    m_tmDelay.Clear();
                    m_iSendStep++;
                    return false;

                //using step 12
                case 11: 
                    if(!m_tmDelay.OnDelay(10)) return false ;
                    if (!SendMsg("SETTEXT S" + iTrayChipCnt + " " + string.Format("{0:00000}", OM.EqpStat.iSerialNo)))
                    {
                        if (m_bShowErr)SM.ERR.SetErr((int)ei.LSR_ComNG, m_sErr);
                        return true;
                    }
                    iTrayChipCnt++; //트레이에서 칩 카운트
                    OM.EqpStat.iSerialNo++; //전체 랏에서 시리얼넘버용 카운트. 1랏에 같은 넘버가 있으면 안됌.
                    

                    m_iSendStep++;
                    return false;

                case 12:
                    if (!m_bRcvEnded) return false;
                    if (m_sText != "OK")
                    {
                        sTemp = string.Format(" m_iSendStep={0:00}", m_iSendStep);
                        sTemp = m_eCycle.ToString() + sTemp + " m_sText=" + m_sText;
                        m_sErr = sTemp;
                        if (m_bShowErr) SM.ERR.SetErr((int)ei.LSR_ComNG, m_sErr);
                        m_iSendStep = 0;
                        return true;
                    }

                    //if (!SendMsg("SETTEXT SerialNo" + iTrayChipCnt + " " + string.Format("{0:00000}", m_iSerialCnt)))
                    //{
                    //    if (m_bShowErr)
                    //        SM.ERR.SetErr((int)ei.LSR_ComNG, m_sErr);
                    //    return true;
                    //}

                    //iTrayChipCnt++; //트레이에서 칩 카운트
                    //m_iSerialCnt++; //전체 랏에서 시리얼넘버용 카운트. 1랏에 같은 넘버가 있으면 안됌.
                    if (iTrayChipCnt < iTrayChipCntMax) // 10*10 이면 100번 보낸다.
                    {
                        m_iSendStep = 11 ;
                        m_tmDelay.Clear();
                        return false;
                    }

                    m_iSendStep++;
                    return false;

                case 13:
                    if (!m_bRcvEnded) return false;
                    if (m_sText != "OK")
                    {
                        sTemp = string.Format(" m_iSendStep={0:00}", m_iSendStep);
                        sTemp = m_eCycle.ToString() + sTemp + " m_sText=" + m_sText;
                        m_sErr = sTemp;
                        if (m_bShowErr) SM.ERR.SetErr((int)ei.LSR_ComNG, m_sErr);
                        m_iSendStep = 0;
                        return false;
                    }

                    //if (!SendMsg("EXECTRANS")) { if (m_bShowErr) SM.ERR.SetErr((int)ei.LSR_ComNG, m_sErr); return true; } //여기서 에러 나면 MARK STOP을 해봐야 함.
                    m_iSendStep++;
                    return false ;

                case 14:
                    //if (!m_bRcvEnded) return false;
                    //if (m_sText != "OK")
                    //{
                    //    sTemp = string.Format(" m_iSendStep={0:00}", m_iSendStep);
                    //    sTemp = m_eCycle.ToString() + sTemp + " m_sText=" + m_sText;
                    //    m_sErr = sTemp;
                    //    if (m_bShowErr) SM.ERR.SetErr((int)ei.LSR_ComNG, m_sErr);
                    //    m_iSendStep = 0;
                    //    return false;
                    //}

                    if (!SendMsg("MARK START")) { if (m_bShowErr) SM.ERR.SetErr((int)ei.LSR_ComNG, m_sErr); return true; } //여기서 에러 나면 MARK STOP을 해봐야 함.
                    m_iSendStep++;
                    return false;

                case 15:
                    if (!m_bRcvEnded) return false;
                    if (m_sText != "OK")
                    {
                        sTemp = string.Format(" m_iSendStep={0:00}", m_iSendStep);
                        sTemp = m_eCycle.ToString() + sTemp + " m_sText=" + m_sText;
                        m_sErr = sTemp;
                        if (m_bShowErr) SM.ERR.SetErr((int)ei.LSR_ComNG, m_sErr);
                        m_iSendStep = 0;
                        return false;
                    }

                    m_iSendStep = 0;
                    return true;
            }
        }
             
        bool bBeginTran = false ;
        bool CycleSetSerialNo()
        {
            if (m_iSendStep == 0) return true;

            string sTemp;

            if (m_tmCycle.OnDelay(m_iSendStep == m_iPreSendStep && !OM.MstOptn.bDebugMode, 50000))
            {
                sTemp = string.Format("CycleSetSerialNo Timeout m_iSendStep={0:00}", m_iSendStep);
                sTemp = m_eCycle.ToString() + sTemp;
                Log.Trace("DominoDynamark3", sTemp);
                //
                if(bBeginTran)
                {
                    SendMsg("ROLLBACKTRANS");//트렌젝션 롤백 시킴. 리턴은 신경안씀. 이걸 안하면 다음 명령시 에러 남.
                }
                m_sErr = sTemp;
                if (m_bShowErr) SM.ERR.SetErr((int)ei.LSR_ComNG, sTemp);
                m_iSendStep = 0;
                return true;
            }

            if (m_iSendStep != m_iPreSendStep)
            {
                sTemp = string.Format(" m_iSendStep={0:00}", m_iSendStep);
                sTemp = m_eCycle.ToString() + sTemp;
                Log.Trace("DominoDynamark3", sTemp);
            }

            m_iPreSendStep = m_iSendStep;


            int iTrayChipCntMax = OM.DevInfo.iTrayColCnt * OM.DevInfo.iTrayRowCnt;

            switch (m_iSendStep)
            {

                default:
                    sTemp = string.Format(" Cycle Default Clear m_iSendStep={0:00}", m_iSendStep);
                    sTemp = m_eCycle.ToString() + sTemp;
                    m_sErr = sTemp;

                    if (bBeginTran)
                    {
                        SendMsg("ROLLBACKTRANS");//트렌젝션 롤백 시킴. 리턴은 신경안씀.
                    }

                    Log.Trace("DominoDynamark3", sTemp);
                    m_iSendStep = 0;
                    return true;

                case 0:
                    return false;

                case 10:
                    //20190910 일단 100개정도 랏넘버를 바꿔야 하는데 시간이 오래 걸릴수 있어서 그냥 트렌젝션을 써봄.
                    //트렌젝션 시작.
                    if (!SendMsg("BEGINTRANS")) { if (m_bShowErr) SM.ERR.SetErr((int)ei.LSR_ComNG, m_sErr); return true; } //여기서 에러 나면 MARK STOP을 해봐야 함.
                    
                    //20191107 고객사 요청사항
                    //장비 트러블로 마킹중 중간에 멈췄을때 기존에 돌리던 트레이는 버리고
                    //다음 트레이부터 다시 작업한다고 해서 iTrayChipCnt를 이전 작업하던 번호로
                    //날려줘야해서 시리얼 번호 날릴때는 처음에 초기화 안함. 진섭
                    iTrayChipCnt = 0;
                    m_iSendStep++;
                    return false;

                case 11:
                    if (!m_bRcvEnded) return false;
                    if (m_sText != "OK")
                    {
                        bBeginTran = true ; //트렌젝션 시작 되었음. 엔드전에 에러나면 롤백이나 엔드 해야함.

                        sTemp = string.Format(" m_iSendStep={0:00}", m_iSendStep);
                        sTemp = m_eCycle.ToString() + sTemp + " m_sText=" + m_sText;
                        m_sErr = sTemp;
                        if (m_bShowErr) SM.ERR.SetErr((int)ei.LSR_ComNG, m_sErr);
                        m_iSendStep = 0;
                        return false;
                    }

                    if (!SendMsg("SETTEXT S" + iTrayChipCnt + " " + string.Format("{0:00000}", OM.EqpStat.iSerialNo))) { if (m_bShowErr) SM.ERR.SetErr((int)ei.LSR_ComNG, m_sErr); return true; }

                    iTrayChipCnt++;
                    OM.EqpStat.iSerialNo++;
                    if (iTrayChipCnt < iTrayChipCntMax) // 10*10 이면 100번 보낸다.
                    {
                        m_tmCycle.Clear();
                        return false;
                    }

                    m_iSendStep++;
                    return false;

                case 12:
                    if (!m_bRcvEnded) return false;
                    if (m_sText != "OK")
                    {
                        sTemp = string.Format(" m_iSendStep={0:00}", m_iSendStep);
                        sTemp = m_eCycle.ToString() + sTemp + " m_sText=" + m_sText;
                        m_sErr = sTemp;
                        if (m_bShowErr) SM.ERR.SetErr((int)ei.LSR_ComNG, m_sErr);
                        m_iSendStep = 0;
                        return false;
                    }

                    if (!SendMsg("EXECTRANS")) { if (m_bShowErr) SM.ERR.SetErr((int)ei.LSR_ComNG, m_sErr); return true; } //여기서 에러 나면 MARK STOP을 해봐야 함.
                    m_iSendStep++;
                    return false ;

                case 13:
                    if (!m_bRcvEnded) return false;
                    if (m_sText != "OK")
                    {
                        bBeginTran = false;
                        sTemp = string.Format(" m_iSendStep={0:00}", m_iSendStep);
                        sTemp = m_eCycle.ToString() + sTemp + " m_sText=" + m_sText;
                        m_sErr = sTemp;
                        if (m_bShowErr) SM.ERR.SetErr((int)ei.LSR_ComNG, m_sErr);
                        m_iSendStep = 0;
                        return false;
                    }

                    if (!SendMsg("MARK START")) { if (m_bShowErr) SM.ERR.SetErr((int)ei.LSR_ComNG, m_sErr); return true; } //여기서 에러 나면 MARK STOP을 해봐야 함.
                    m_iSendStep++;
                    return false;

                case 14:
                    if (!m_bRcvEnded) return false;
                    if (m_sText != "OK")
                    {
                        sTemp = string.Format(" m_iSendStep={0:00}", m_iSendStep);
                        sTemp = m_eCycle.ToString() + sTemp + " m_sText=" + m_sText;
                        m_sErr = sTemp;
                        if (m_bShowErr) SM.ERR.SetErr((int)ei.LSR_ComNG, m_sErr);
                        m_iSendStep = 0;
                        return false;
                    }

                    m_sText = "" ;
                    m_bRcvEnded = false ;

                    m_iSendStep++;
                    return false;

                case 15://1.8초는 전에께 찍히고 2초는 정상동작 78개 동작시.
                    if (!m_bRcvEnded) return false;
                    if (m_sText != "MSG 26")
                    {
                        sTemp = string.Format(" m_iSendStep={0:00}", m_iSendStep);
                        sTemp = m_eCycle.ToString() + sTemp + " m_sText=" + m_sText;
                        m_sErr = sTemp;
                        if (m_bShowErr) SM.ERR.SetErr((int)ei.LSR_ComNG, m_sErr);
                        m_iSendStep = 0;
                        return false;
                    }

                    m_iSendStep = 0;
                    return true;
            }
        }

        bool CycleMark()
        {
            if (m_iSendStep == 0) return true;

            string sTemp;
            int iTimeout = 4000 ;
            if(m_iSendStep == 12)
            {
                iTimeout = 250000;
            }
            if (m_tmCycle.OnDelay(m_iSendStep == m_iPreSendStep && !OM.MstOptn.bDebugMode , iTimeout))
            {
                sTemp = string.Format("CycleMark TimeOut m_iSendStep={0:00}", m_iSendStep);
                sTemp = m_eCycle.ToString() + sTemp;
                Log.Trace("DominoDynamark3", sTemp);
                //SM.ERR.SetErr((int)ei.LSR_ComNG, );

                m_sErr = sTemp;

                if (m_bShowErr) SM.ERR.SetErr((int)ei.LSR_ComNG, sTemp);

                m_iSendStep = 0;
                return true;
            }

            if (m_iSendStep != m_iPreSendStep)
            {
                sTemp = string.Format(" m_iSendStep={0:00}", m_iSendStep);
                sTemp = m_eCycle.ToString() + sTemp;
                Log.Trace("DominoDynamark3", sTemp);
            }

            m_iPreSendStep = m_iSendStep;


            int iDeviceCntMax = OM.DevInfo.iTrayColCnt * OM.DevInfo.iTrayRowCnt;

            switch (m_iSendStep)
            {

                default:
                    sTemp = string.Format(" Cycle Default Clear m_iSendStep={0:00}", m_iSendStep);
                    sTemp = m_eCycle.ToString() + sTemp;
                    m_sErr = sTemp;
                    Log.Trace("DominoDynamark3", sTemp);
                    m_iSendStep = 0;
                    return true;

                case 0:
                    return false;

                case 10:
                    //20190910 일단 100개정도 랏넘버를 바꿔야 하는데 시간이 오래 걸릴수 있어서 그냥 트렌젝션을 써봄.
                    //트렌젝션 시작.
                    if (!SendMsg("TRIGGER")) { if (m_bShowErr) SM.ERR.SetErr((int)ei.LSR_ComNG, m_sErr); return true; } //여기서 에러 나면 MARK STOP을 해봐야 함.
                    iTrayChipCnt = 0;
                    m_iSendStep++;
                    return false;

                case 11:
                    if (!m_bRcvEnded) return false;
                    if (m_sText != "OK")
                    {
                        sTemp = string.Format(" m_iSendStep={0:00}", m_iSendStep);
                        sTemp = m_eCycle.ToString() + sTemp + " m_sText=" + m_sText;
                        m_sErr = sTemp;
                        if (m_bShowErr) SM.ERR.SetErr((int)ei.LSR_ComNG, m_sErr);
                        m_iSendStep = 0;
                        return true;
                    }

                    m_bRcvEnded = false ;
                    m_sText = "";

                    m_iSendStep++;
                    return false ;
                
                //using timeout
                case 12:
                    if (!m_bRcvEnded) return false;
                    if (m_sText != "MSG 3")
                    {
                        sTemp = string.Format(" m_iSendStep={0:00}", m_iSendStep);
                        sTemp = m_eCycle.ToString() + sTemp + " m_sText=" + m_sText;
                        m_sErr = sTemp;
                        if (m_bShowErr) SM.ERR.SetErr((int)ei.LSR_ComNG, m_sErr);
                        m_iSendStep = 0;
                        return true;
                    }
                    m_iSendStep = 0;
                    return true;
            }
        }

        private bool PortOpen()
        {


            try
            {
                Port.Open(); 
            }
            catch
            {
                Log.ShowMessage(m_sName + " COM PORT ERROR", Port.PortName + " COM Port not Exist"); 
                //MessageBox.Show(new Form(){TopMost = true}, sName + " COM PORT ERROR", Port.PortName + " COM Port Not Exist");
                return false;
            }    
            if (!Port.IsOpen)
            {
                //MessageBox.Show(new Form(){TopMost = true}, sName + " COM PORT ERROR", Port.PortName + " COM Port Not Opened");
                Log.ShowMessage(m_sName + " COM PORT ERROR", Port.PortName + " COM Port not opened");   
                return false;
            }
            return true;            
        }

        private void PortClose()
        {
            Port.Close();
            Port.Dispose();
        }

        private const byte CR = 0x0D;
        private const byte LF = 0x0A;
        public bool SendMsg(string _sData)
        {
            _sData += "\r\n";
            m_bRcvEnded = false ;
            m_sText     = "";

            Log.TraceListView(_sData);

            byte[] ByteData = Encoding.ASCII.GetBytes(_sData);

            try
            {
                Port.Write(_sData);
            }
            catch(Exception _e)
            {
                m_sErr = "SendMsgErr - " +_e.Message ;
                Log.TraceListView(m_sErr);
                return false ;
            }
            

            return true ;
        }

        public string GetReadingText()
        {
            return m_sText ;
        }

        public bool ReadEnded()
        {
            return m_bRcvEnded ;
        }

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //have to uncheck ECHO in DOMINO Print Network Setting.
            int iByteCntToRead = Port.BytesToRead ;
            if (iByteCntToRead <= 0) return ;

            //리턴값 읽은 바이트수.
            byte[] bRead = new byte[iByteCntToRead];
            Port.Read(bRead , 0 , iByteCntToRead);

            //string sReceved = "";
            //sReceved = Port.ReadExisting();

            m_sText += Encoding.ASCII.GetString(bRead);
        

            if(!m_sText.Contains("\r\n")) return ;
            Log.TraceListView(m_sText);
            m_sText = m_sText.Substring(0,m_sText.IndexOf("\r\n"));
            m_bRcvEnded = true;     
        }    


    }
}
