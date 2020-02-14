using System;
using System.IO.Ports;
using System.Text;
using COMMON;
using SML;
using System.Runtime.CompilerServices;


namespace Machine
{
    //키엔스 비전내장 레이져마킹기.
    //MD-X1000 , MD-X1500 용 시리얼 통신 클래스.
    //초기값=>통신속도:38400 , 페리티:none , 스탑비트:1 ,체크섬 : 없음.
    //세팅값=>통신속도:38400 , 페리티:none , 스탑비트:1 ,체크섬 : 있음. <= 레이저 설정에서 체크섬을 켜야 함.
    //케이블은 메뉴얼에는 다이렉트로 되어 있음. 이건 실제 체크 해봐야 함.
    //마킹 에너지 체크 기능은 인쇄 완료 후 2 초 이내에 다음 트리거가 들어오면 무시됩니다. 인쇄 완료와 다음 인쇄 시작까지는 2 초 이상 두십시오. 
    //20171114 선계원.
    delegate void ReceivedCallback();
    public class MD_X1000 : ML
    {   
        private int iStep      ;
        private int iPreStep   ;
        private CDelayTimer Timer = new CDelayTimer();
        private CDelayTimer Delay = new CDelayTimer();
        private SerialPort Port = new SerialPort();

        private   int    iPortId     ;
        private   string sRecvingMsg ;
        private   string sRecvedMsg  ;
        private   string sSendedMsg  ;
        private   string sEndOfText  ; //응답메세지가 끊겨서 2번에 들어오는경우가 있는데 그럴때 세팅 하고 이문자가 들어올때까지 메세지를 받지않는걸로 함.

        private xi xError   ;
        private xi xWarning ;
        private xi xReady   ;
        private xi xWorking ;
        private xi xWorkEnd ;
        private xi xCheckOk ;
        private xi xCheckNg ;

        private yi yTrigger ;
        private yi yCheck   ;

        private bool bRsltOk ;

        private string sErrMsg ;
        
        public CTimer m_CycleTime = new CTimer();

        //파일로 초기화.
        //_iComNo은 1번부터 입력 되어야 한다.
        public MD_X1000(int _iPortId , 
                        xi  _xError  ,
                        xi  _xWarning,
                        xi  _xReady  ,
                        xi  _xWorking,
                        xi  _xWorkEnd,
                        xi  _xCheckOk,
                        xi  _xCheckNg,
                        yi  _yTrigger,
                        yi  _yCheck  )
        {
           
            iPortId    = _iPortId   ;

            xError     = _xError   ;
            xWarning   = _xWarning ;
            xReady     = _xReady   ;
            xWorking   = _xWorking ;
            xWorkEnd   = _xWorkEnd ;
            xCheckOk   = _xCheckOk ;
            xCheckNg   = _xCheckNg ;

            yTrigger   = _yTrigger ;
            yCheck     = _yCheck   ;

            sEndOfText = "\r\n";

            Port.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
            Port.Encoding = System.Text.Encoding.GetEncoding("iso-8859-1");//이것이 8비트 문자 모두 가능 Ascii는 7비트라 63이상의 값은 표현 안됌.

            Port.PortName     = "Com" + iPortId.ToString();
            Port.BaudRate     = 38400; 
            Port.DataBits     = 8    ;
            Port.Parity       = Parity.None ;
            Port.StopBits     = StopBits.One;
            Port.ReadTimeout  = 1000;
            Port.WriteTimeout = 1000;

            PortOpen();
            
        }

        public bool PortOpen()
        {
            try
            {
                Port.Open(); 
            }
            catch(Exception _e)
            {
                sErrMsg = iPortId.ToString() + "COM ERROR(Keyence MD_X1000 " + _e.Message + ")" ; 
                return false ;
            }    
            if (!Port.IsOpen)
            {
                sErrMsg = iPortId.ToString() + "COM ERROR(Keyence MD_X1000 COM Port not opened)" ; 
                return false;
            }

            return true;
            
        }

        public void PortClose()
        {
            Port.Close();
            Port.Dispose();
        }


        private bool SendMsg(string _sMsg)
        {
            if (!Port.IsOpen) {
                sErrMsg = iPortId.ToString() + "COM ERROR(Keyence MD_X1000 COM Port not opened)" ; 
                return false ;
            }
            sRecvedMsg  = "";
            sRecvingMsg = "";
            sSendedMsg  = _sMsg;

            Port.Write(_sMsg);

            return true ;
        }

        private bool SendMsg(byte [] _baMsg)
        {
            if (!Port.IsOpen) {
                sErrMsg = iPortId.ToString() + "COM ERROR(Keyence MD_X1000 COM Port not opened)" ; 
                return false ;
            }
            sRecvedMsg  = "";
            sRecvingMsg = "";
            Encoding enc = Encoding.GetEncoding("iso-8859-1");
            sSendedMsg = enc.GetString(_baMsg);
            Port.Write(_baMsg, 0, _baMsg.Length);

            return true ;
        }

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int intRecSize = Port.BytesToRead;
            string strRecData= "";

            if (intRecSize == 0) return ;
            strRecData = Port.ReadExisting();
            sRecvingMsg += strRecData;

            if(sRecvingMsg.Contains(sEndOfText)) return ;

            sRecvedMsg = sRecvingMsg ;           
        }

        private bool IsReceiveEnd()
        {
            if (sRecvedMsg != "") //End of Text문자가 있을경우.
            {
                return true ;
            }
            return false ;
        }                        

        //public string GetRecvedMsg()
        //{
        //    return sRecvedMsg;
        //}




        //키엔스 장비용.
        //===================================================================================================================
                //전체 익스클루시브오어 연산.
        private bool GetCheckSum(string _sMsg , out string _sCheckSum)
        {
            //WX,Command,
            //48
            //WX,OK,
            //3B
            string sMsg = _sMsg ;
            _sCheckSum = "";

            if(sMsg.Length <= 0) {
                sErrMsg = "Message Length is less than 1" ;
                return false ;
            }
            int iExor = 0;
            int iChar = 0;

            for(int i = 0 ; i < sMsg.Length ; i++){
                if(i==0){
                    iChar = Convert.ToInt32(sMsg[i]) ;
                    iExor = iChar ;
                }
                else {
                    iChar = Convert.ToInt32(sMsg[i]) ;
                    iExor ^= iChar ;
                }
            }
            
            _sCheckSum = string.Format("{0:X}",iExor);

            return true ;
        }

        //CR 을 기준으로 앞으로 2칸가서 검사 하기때문에 CR이 꼭필요.
        private bool CheckCheckSum(string _sMsg)
        {
            //WX,OK,3B[cr]
            //012345678
            int iCRPos = 0 ;
            string sCheckSum = "";

            iCRPos = _sMsg.IndexOf("\r\n");
            //iCRPos = _sMsg.IndexOf("rn");
            if(iCRPos < 0) {
                sErrMsg = "Message dosn't have [CR]" ;
                return false ;
            }
            string sMsg = _sMsg.Substring(0,iCRPos-2) ;

            if(!GetCheckSum(sMsg , out sCheckSum)) {
                return false ;
            }

            return true ;
        }

        public bool CycleProgramNo(int _iProgramNo , bool _bInit = false )
        {
            if(_bInit){
                sErrMsg = "";
                iPreStep = 0 ;
                iStep = 10 ;
            }
            //Check Cycle Time Out.
            string sTemp ;
            string sMsg  ;
            string sCheckSum ;

            if (Timer.OnDelay(iStep!= 0 && iStep == iPreStep , 5000 )) {
                sTemp = string.Format("Cycle iStep={0:00}", iStep);
                sTemp = "Laser-" + sTemp;
                sErrMsg = sTemp ; 
                iStep = 0;
                return true;
            }
            
            if(iStep != iPreStep) {
                sTemp = string.Format("Laser Cycle iStep={0:00}", iStep);
                Log.Trace("Laser", sTemp);
            }
            
            iPreStep = iStep;
            
            //Cycle.
            switch (iStep) 
            {            
                default : 
                    if(iStep != 0) {
                        sErrMsg = string.Format("Laser Cycle Default Clear from iStep={0:00}", iStep);
                        iStep = 0 ;
                    }                          
                    return true ;
            
                case 10: 
                    if(_iProgramNo < 0   ) {sErrMsg = "ProgramNo is less than 0"      ; return true ;}
                    if(_iProgramNo > 1999) {sErrMsg = "ProgramNo is bigger than 1999" ; return true ;}
                    if(!IO_GetX(xReady  )) { sErrMsg = "Ready Signal is not On"; return true ; }
                    if( IO_GetX(xWorking)) { sErrMsg = "Working Signal is On";   return true ; }
                    if( IO_GetX(xWarning)) { sErrMsg = "Warning Signal is On";   return true ; }
                    if( IO_GetX(xError  )) { sErrMsg = "Error Signal is On";     return true ; }

                    //sMsg = "WX,ProgramNo="+ _iProgramNo.ToString() + ",";
                    sMsg = "WX,PROGRAMNO=" + _iProgramNo.ToString() + ",";
                    if(!GetCheckSum(sMsg , out sCheckSum)){
                        return true ;
                    }

                    sMsg = sMsg + sCheckSum + "\r\n";
                    if(!SendMsg(sMsg)){
                        return true ;
                    }
                    iStep++;
                    return false;

                case 11: 
                    if(!IsReceiveEnd()) return false ;

                    if(!sRecvedMsg.Contains("WX,OK")) {
                        sErrMsg = "RcvMsg not exist WX,OK - " + sRecvedMsg ;
                        return true ;
                    }

                    iStep=0;
                    sErrMsg = "";
                    return true ;
            }
        }

        public bool CycleTrigger(bool _bInit = false )
        {
            if(_bInit){
                sErrMsg = "";
                iPreStep = 0 ;
                iStep = 10 ;
                ML.IO_SetY(yCheck , false);
            }
            //Check Cycle Time Out.
            string sTemp ;

            if (Timer.OnDelay(iStep!= 0 && iStep == iPreStep , 5000 )) {
                IO_SetY(yTrigger , false);
                sTemp = string.Format("Cycle iStep={0:00}", iStep);
                sTemp = "Laser-" + sTemp;
                sErrMsg = sTemp ; 
                iStep = 0;
                return true;
            }
            
            if(iStep != iPreStep) {
                sTemp = string.Format("Laser Cycle iStep={0:00}", iStep);
                Log.Trace("Laser", sTemp);
            }
            
            iPreStep = iStep;
            
            //Cycle.
            switch (iStep) 
            {            
                default : 
                    if(iStep != 0) {
                        sErrMsg = string.Format("Laser Cycle Default Clear from iStep={0:00}", iStep);
                        iStep = 0 ;
                    }                          
                    return true ;
            
                case 10:
                    m_CycleTime.Start();
                    if(!IO_GetX(xReady  )) { sErrMsg = "Ready Signal is not On"; ER_SetErr(ei.MAX_ERR , sErrMsg); return true ; }
                    if( IO_GetX(xWorking)) { sErrMsg = "Working Signal is On";   ER_SetErr(ei.MAX_ERR , sErrMsg); return true ; }
                    if( IO_GetX(xWarning)) { sErrMsg = "Warning Signal is On";   ER_SetErr(ei.MAX_ERR , sErrMsg); return true ; }
                    if( IO_GetX(xError  )) { sErrMsg = "Error Signal is On";     ER_SetErr(ei.MAX_ERR , sErrMsg); return true ; }
                    if( IO_GetY(yTrigger)) { sErrMsg = "Trigger Signal is On";   ER_SetErr(ei.MAX_ERR , sErrMsg); return true ; }

                    IO_SetY(yTrigger , true);
                    iStep++;
                    return false;

                case 11: 
                    if(!IO_GetX(xWorking)) return false ; //워킹이 온되는 것을 기다림.
                    IO_SetY(yTrigger , false);
                    iStep++;
                    return false ;

                case 12:
                    if(IO_GetX(xWorking)) return false ; //워킹이 오프되는 것을 기다림.
                    iStep++;
                    return false ;

                case 13:
                    if(!IO_GetX(xWorkEnd)) return false ; //워킹이 오프되는 것을 기다림.
                    IO_SetY(yCheck , true);

                    iStep++;
                    return false ;

                case 14:
                    if(!IO_GetX(xCheckOk) && !IO_GetX(xCheckNg)) return false ;
                    bRsltOk = IO_GetX(xCheckOk);
                    IO_SetY(yCheck , false);
                    m_CycleTime.End();

                    iStep=0;
                    sErrMsg = "";
                    return true ;
            }
        }
        public bool GetCheckRslt()
        {
            //if(IO_GetX(xCheckOk) ) return true ;
            
            //return false ;

            return bRsltOk;
        }
    }
}
