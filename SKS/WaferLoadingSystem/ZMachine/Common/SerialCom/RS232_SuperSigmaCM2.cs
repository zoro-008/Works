using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using COMMON;

namespace Machine
{
    enum CM2_CMD{
        None        = 0,
        DisprOnOff  = 1,
        SigmaModeOn = 2,
        SigmaModeOff= 3,
        Mode        = 4,
        LoadCh      = 5,
        PTV         = 6,
        SyringeSize = 7,
        //무사시에서 받아오는 것들.
        SylFill     = 8, //현재 잔량  받아오기.
        DispData    = 9, //무사시에 세팅값 받아오기.
        MAX_CM2_CMD
    };
    public struct TCm2Data {
        public double dPrsPres   ;
        public double dVacPres   ;
        public bool   bSigmaMode ;
    };

    //
    public class RS232_SuperSigmaCM2 : CSerialPort
    {
        int         m_iSylFill   ; //현재 잔량. 0~100
        CDelayTimer m_tmDipsr    = new CDelayTimer();

        string      m_sAskMsg    ;
        string      m_sReadMsg   ;
        string      m_sSendMsg   ;
        string      m_sErrMsg    ;

        CM2_CMD     m_iCmdNo     ;
        TCm2Data    DispData     ; //무사시에 세팅되어 있는데이터를 받아온값.

        int         m_iSeqCycle   ;
        int         m_iPreSeqCycle;

        bool        m_bDisprOn    ;

        const char STX = (char)0x02;
        const char ETX = (char)0x03; //End Text [응답용Asc]
        const char EOT = (char)0x04; //End of Text[요구용 Asc]
        const char ENQ = (char)0x05; //Enquire[프레임시작코드]
        const char ACK = (char)0x06; //Acknowledge[응답 시작]
        const char CAN = (char)0x18; //Not Acknoledge[에러응답시작]

        string MSG_OK  = STX + "02A02D" + ETX;                       
        string MSG_ERR = STX + "02A22B" + ETX;      

        public RS232_SuperSigmaCM2(int _iPortId , string _sName, string _sEndOfText=""):base(_iPortId , _sName, _sEndOfText)
        {
            //BaudRate=19200
            //DataBit=8
            //ParityBit=0
            //StopBit=1
        }

        //밑에 있는 4개 배열 나중에 주석 처리 하던가 범용성 있게 변경.....
        private static string GetChksum(string sData) //원래는 STX는 짤라서 보내 줘야 하는데 귀찮음.
        {
            int iDataLength = sData.Length ;
            int iChksum=0;
            //int iHex1,iHex2;
            char [] sHex= new char[3];
            string sReturn ;
            char cData ;
            int iTemp = 0;
        
            for(int i=0 ; i<iDataLength; i++) {//STX는 포함 안하고 안시스트링은 첫문자가 1부터 시작이다 그래서 2부터.
                cData = sData[i] ;
                iTemp = cData;
                iChksum = iChksum + cData;
            }
            iChksum = iChksum % 256;
            iChksum = 256 - iChksum;
            //iHex1=iChksum/16;
            //iHex2=iChksum%16;
            sReturn = string.Format("{0:X}", iChksum);
            return sReturn ;
        }

        protected override void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            base.DataReceived(sender,e);

            if(!IsReceiveEnd()) return ;

                 if(sRecvMsg[0] == (char)EOT) m_sAskMsg = sRecvMsg;
            else if(sRecvMsg[0] == (char)ENQ) m_sAskMsg = sRecvMsg;
            else if(sRecvMsg[0] == (char)ACK) m_sAskMsg = sRecvMsg;
            else {
                if(sRecvMsg.IndexOf(ETX) > 0) m_sReadMsg += sRecvMsg;
                //Trace("Musashi m_sReadMsg",m_sReadMsg.c_str());
            }          
        }

        public string GetSendMsg(string _sMsg)
        {
            string sSendMsg = "";
        
            sSendMsg += (char)STX;
            sSendMsg += _sMsg;
            sSendMsg += GetChksum(_sMsg);
            sSendMsg += (char)ETX;
        
            return sSendMsg;
        }


        //이것은 안쓰고 IO에서 터트린다.
        public bool SetDisprOnOff(bool _bOnOff)
        {        
            if(m_bDisprOn  == _bOnOff) return false ;
        
            string sSendMsg;
            m_bDisprOn  = _bOnOff;
            m_iCmdNo    = CM2_CMD.DisprOnOff ;
        
            sSendMsg    = "04DI  ";
            sSendMsg    = GetSendMsg(sSendMsg);
            m_sSendMsg  = sSendMsg ;
            m_iSeqCycle = 10;
            m_sAskMsg   = "";
            m_sReadMsg  = "";
            m_sErrMsg   = "";
                
            return true ;
        }

        public bool SetSigmaModeOn()
        {
            string sSendMsg;
            m_iCmdNo = CM2_CMD.SigmaModeOn;

            sSendMsg = "04SM  ";
            sSendMsg = GetSendMsg(sSendMsg);

            m_sSendMsg = sSendMsg;

            m_iSeqCycle = 10;
            m_sAskMsg   = "";
            m_sReadMsg  = "";
            m_sErrMsg   = "";

            return true;
        }

        public bool SetSigmaModeOff(int _iCh)
        {
            string sSendMsg;
            m_iCmdNo = CM2_CMD.SigmaModeOff;

            if (_iCh > 100) _iCh = 100;
            if (_iCh < 1  ) _iCh = 1  ;

            string sTemp;
            sTemp = string.Format("{0:000}",_iCh);
            sSendMsg = "05CL" + sTemp;

            m_sSendMsg = GetSendMsg(sSendMsg); ;

            m_iSeqCycle = 10;
            m_sAskMsg  = "";
            m_sReadMsg = "";
            m_sErrMsg  = "";

            return true;
        }

        //Timed 모드와 Manual모드 전환.
        public bool SetMode(bool _bManual)
        {
            string sSendMsg;
            string sMode   ;
            m_iCmdNo = CM2_CMD.Mode;
        
            if(_bManual) sMode= "1";
            else         sMode= "0";
        
            sSendMsg = "04TM0" + sMode;
            sSendMsg = GetSendMsg(sSendMsg);
            m_sSendMsg = sSendMsg ;
            m_iSeqCycle = 10;
            m_sAskMsg   = "";
            m_sReadMsg  = "";
            m_sErrMsg   = "";
        
            return true ;
        }

        public bool SetLoadCh (int _iCh)
        {
            string sSendMsg;
            string sCh     ;
        
            m_iCmdNo = CM2_CMD.LoadCh;
        
            if(_iCh > 100) _iCh = 100;
            if(_iCh <   1) _iCh =   1;
        
            sCh = string.Format("{0:00}",_iCh);
        
            sSendMsg = "04LH" + sCh ; 
            sSendMsg = GetSendMsg(sSendMsg);
            m_sSendMsg = sSendMsg ;
        
            m_iSeqCycle = 10;
            m_sAskMsg   = "";
            m_sReadMsg  = "";
            m_sErrMsg   = "";
        
            return true ;
        }

        public bool SetPTV(double _dPres , int _iTime, double _dVac)
        {
            string sSendMsg;
            string sPres, sTime, sVac;
        
            m_iCmdNo = CM2_CMD.PTV;
        
            int iPres = (int)(_dPres * 10 ) ;   //무사시쪽 단위가 0.1 kPa
            int iVac  = (int)(_dVac  * 100) ;   //무사시쪽 단위가 0.01 kPa       
        
        
            sPres = string.Format("{0:0000}", iPres );
            sTime = string.Format("{0:0000}", _iTime);
            sVac  = string.Format("{0:0000}", iVac  );
        
            sSendMsg = "14PR  P" + sPres + "T" + sTime + "V-" + sVac;
            sSendMsg = GetSendMsg(sSendMsg);
            m_sSendMsg = sSendMsg ;
        
            m_iSeqCycle = 10;
            m_sAskMsg   = "";
            m_sReadMsg  = "";
            m_sErrMsg   = "";
        
            return true ;
        }

        public bool SetSyringeSize(int _iSize)
        {
            string sSendMsg;        
            m_iCmdNo = CM2_CMD.SyringeSize;
        
            //Syringe Size
            // 3CC : 0 ,
            // 5CC : 1 ,
            //10CC : 2 ,
            //20CC : 3 ,
            //30CC : 4 ,
            //50CC : 5 ,
            //70CC : 6
        
            if(_iSize > 6) _iSize = 6;
            if(_iSize < 1) _iSize = 1;
        
            sSendMsg = "07SY001S0" + _iSize.ToString();
            //sSendMsg = "08DA02S" + (String)_iSize + "A1" ;
            sSendMsg = GetSendMsg(sSendMsg);
            m_sSendMsg = sSendMsg ;
            m_iSeqCycle = 10;
            m_sAskMsg   = "";
            m_sReadMsg  = "";
            m_sErrMsg   = "";
        
            return true ;
        }

        public bool GetSylVolm(int _iCh) //잔양체크
        {
            string sSendMsg;
        
            m_iCmdNo = CM2_CMD.SylFill;
        
            if(_iCh > 100) _iCh = 100;
            if(_iCh <   1) _iCh =   1;
        
            string sCh = string.Format("{0:000}", _iCh);
            sSendMsg = "08UL" + sCh + "D05"; //(String)_iCh;
            sSendMsg = GetSendMsg(sSendMsg);
            m_sSendMsg = sSendMsg ;
        
            m_iSeqCycle = 10;
            m_sAskMsg   = "";
            m_sReadMsg  = "";
            m_sErrMsg   = "";
        
            return true ;
        }

        public bool GetDispData(int _iCh) //토출세팅값 받아오기.
        {
            string sSendMsg;
        
            m_iCmdNo = CM2_CMD.DispData;
        
            if(_iCh > 100) _iCh = 100;
            if(_iCh <   1) _iCh =   1;
        
            string sCh = string.Format("{0:000}", _iCh);
            sSendMsg = "08UL" + sCh + "D01"; //(String)_iCh;
            sSendMsg = GetSendMsg(sSendMsg);
            m_sSendMsg = sSendMsg ;
        
            m_iSeqCycle = 10;
            m_sAskMsg   = "";
            m_sReadMsg  = "";
            m_sErrMsg   = "";
        
            return true ;
        }

        public TCm2Data GetDispData()
        {
            return DispData ;
        }

        public int GetSylFill ()
        {
            return m_iSylFill ;
        
        }
        public string GetErrMsg()
        {
        
            return m_sErrMsg;
        }

        public bool GetMsgEnd()
        {
            if(m_iCmdNo == CM2_CMD.None) return true ;
            else                         return false;
        
        }

        public bool Update()
        {
            if (m_iCmdNo == CM2_CMD.None) return true;

            if (m_iPreSeqCycle != m_iSeqCycle)
            {
                //Trace("Musashi Step",String(m_iSeqCycle).c_str());

            }

            m_iPreSeqCycle = m_iSeqCycle;

            switch (m_iSeqCycle)
            {
                default: 
                    if (m_iSeqCycle != 0)
                    {
                        //ShowMessage("Sequence Default"); //2015 JS
                    }
                    m_iCmdNo = CM2_CMD.None;
                    m_iSeqCycle = 0;
                    return true;

                //Send ENQ
                case 10: 
                    string sSendMsg = "" ;
                    sSendMsg += ENQ;      //통신 준비 상태 체크
                    //Trace("Musashi Send","Send ENQ");
                    m_sReadMsg = m_sAskMsg = "";
                    SendMsg(sSendMsg);

                    m_tmDipsr.Clear();
                    m_iSeqCycle++;
                    return false;

                //Receive ACK
                case 11: 
                    if (m_tmDipsr.OnDelay(true, 500))
                    {
                        if (m_sAskMsg != ACK.ToString())
                        { //대기 상태가 아닐때..
                            m_sErrMsg = "ACK Error";
                            m_iSeqCycle = 0;
                            return false;
                        }
                    }
                    if (m_sAskMsg != ACK.ToString()) return false;
                    m_iSeqCycle++;
                    return false;

                //Send COMMAND
                case 12:
                    //Trace("Musashi Send","Send COMMAND");
                    m_sReadMsg = m_sAskMsg = "";
                    SendMsg(m_sSendMsg);

                    m_tmDipsr.Clear();
                    m_iSeqCycle++;
                    return false;

                //Receive A0
                case 13: 
                    if (m_tmDipsr.OnDelay(true, 500))
                    {
                        if (m_sReadMsg == MSG_ERR)
                        {     //통신 정상 전송이 아닐때
                            m_iSeqCycle = 50;
                            return false;
                        }
                    }
                    if (m_sReadMsg != MSG_OK) return false;


                    //업로드계는 한번 더 주고 받아야함.
                    if (m_iCmdNo == CM2_CMD.SylFill || m_iCmdNo == CM2_CMD.DispData)
                    {
                        m_iSeqCycle = 20;
                        return false;
                    }
                    m_iSeqCycle = 30; //다운로드계는 명령이 30번으로 가서 끝남.
                    return false;
                //Send ACK
                case 20: 
                    sSendMsg = ACK.ToString();      //통신 준비 상태 체크
                    //Trace("Musashi Send","Send ACK");
                    m_sReadMsg = m_sAskMsg = "";
                    SendMsg(sSendMsg);

                    m_tmDipsr.Clear();
                    m_iSeqCycle++;
                    return false;

                //Receive Data
                case 21: 
                    if (m_tmDipsr.OnDelay(true, 500))
                    { //타임아웃.
                        m_iSeqCycle = 50;
                        return false;
                    }

                    if (m_sReadMsg.IndexOf(ETX) < 0) return false;
                    //if(m_sReadMsg != sMsgOk) { //통신 정상 전송이 아닐때
                    //    m_iSeqCycle = 50 ;
                    //    return false ;
                    //}


                    //stx08DA05R050C7etx
                    if (m_iCmdNo == CM2_CMD.SylFill)
                    {
                        string sTemp;
                        sTemp = m_sReadMsg.Substring(8, 3);//050
                        m_iSylFill = int.Parse(sTemp);
                    }
                    if (m_iCmdNo == CM2_CMD.DispData)
                    {
                        string sTemp;

                        sTemp = m_sReadMsg.Substring(8, 4); //토출압력 4자리 
                        DispData.dPrsPres = int.Parse(sTemp) / 10.0;

                        sTemp = m_sReadMsg.Substring(19, 4); //배큠압력 4자리
                        DispData.dVacPres = int.Parse(sTemp) / 100.0;

                        sTemp = m_sReadMsg.Substring(24, 1); //모드 1자리
                        if (int.Parse(sTemp) == 2 || int.Parse(sTemp) == 3)
                        {
                            DispData.bSigmaMode = true;
                        }
                        else
                        {
                            DispData.bSigmaMode = false;
                        }
                    }
                    m_iSeqCycle = 30;
                    return false;


                case 30: 
                    sSendMsg = EOT.ToString();                                         //통신 종료 메시지 전송
                    //Trace("Musashi Send","Send End");
                    m_sReadMsg = m_sAskMsg = "";
                    SendMsg(sSendMsg);

                    m_iCmdNo = CM2_CMD.None;
                    m_iSeqCycle = 0;
                    return true;

                //OnErr
                case 50: 
                    sSendMsg = CAN.ToString();                                         //통신 커맨드 취소 신호 전송
                    m_sReadMsg = m_sAskMsg = "";
                    SendMsg(sSendMsg);

                    sSendMsg = EOT.ToString();                                         //통신 종료 메시지 전송
                    SendMsg(sSendMsg);
                    //Trace("Musashi Send","Error");
                    m_sErrMsg = "Communication Error";
                    m_iCmdNo = CM2_CMD.None;
                    m_iSeqCycle = 0;
                    return true;
            }
        }
    }
}
