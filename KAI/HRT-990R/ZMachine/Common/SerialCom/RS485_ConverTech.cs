using COMMON;
using System;
using System.IO.Ports;
using System.Text;

namespace Machine
{
    //컨버텍 파워 서플라이.
    public class RS485_ConverTech
    {
        private  SerialPort Port = null;
        public   const int    MAX_ARRAY = 3 ;
        public   const int    LOG_ID    = 7 ;

        private   int    iPortId    = 0 ;
        private   int    iPortNo    = 1 ;
        private   string sName      = "";
        //private   int    iId        = 0 ;
        private   int    iPreId     = 0 ;

        private   string sSetMsg    = "";
        private   string sRecvedMsg = "";
        private   string sSendedMsg = "";
        //private   bool   bRcvEnd    = false;


        private   const char   cSTX  = (char)0x02 ;
        private   const char   cETX  = (char)0x03 ;
        private   const char   cCR   = (char)0x0D ;
        private   const char   cLF   = (char)0x0A ;
        private   const char   cACK  = (char)0x06 ;
        private   const char   cSTR  = (char)0x40 ;

        

        
        public struct TStat
        { 
            public double dVoltage    ;//측정전압
            public double dCurrent    ;//측정전류
            public double dSetVoltage ;//설정전압
            public double dSetCurrent ;//설정전류

            public bool   bRcvEnd     ;
            public bool   bRemote     ;
            public bool   bOutput     ;
            public int    iError      ;//1 Over Voltaae Protection , 2 Over Current Protection , 3 Over Temperature Protection

            public string sMsg        ;
            public void Clear()
            {
                dVoltage    = 0;
                dCurrent    = 0;
                dSetVoltage = 0;
                dSetCurrent = 0;
                            
                bRcvEnd     = false;
                bRemote     = false;
                bOutput     = true ;
                iError      = 0;

                sMsg        = "";
            }
        };
        public TStat[] Stat;

        public struct TPreStat
        { 
            public string sSendA ;
            public string sSendB ;
            public string sSendC ;
            public string sSendD ;
            public string sSendE ;
            public string sRsv   ;

            public void Clear()
            {
                sSendA = "";
                sSendB = "";
                sSendC = "";
                sSendD = "";
                sSendE = "";
            }
        }
        public TPreStat[] PreStat;

        public struct TPara
        {
            public double dVCapacity ;
            public double dACapacity ;

        };
        public TPara[] Para ;

        

        //파일로 초기화.
        //_iComNo은 1번부터 입력 되어야 한다.
        public RS485_ConverTech(int _iPortId , string _sName)
        {           
            iPortId    = _iPortId   ;
            sName      = _sName     ;

            Port = new SerialPort();
            Port.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
            //Port.Encoding = System.Text.Encoding.GetEncoding("iso-8859-1");//이것이 8비트 문자 모두 가능 Ascii는 7비트라 63이상의 값은 표현 안됌.

            Stat    = new TStat   [MAX_ARRAY];
            Para    = new TPara   [MAX_ARRAY];
            PreStat = new TPreStat[MAX_ARRAY];

            for(int i=0; i<MAX_ARRAY; i++)
            {
                PreStat[i].Clear();
            }
            Load(sName); 

            Port.PortName= "Com" + iPortNo.ToString();
            PortOpen();

            Port.BaudRate     = 19200; //9600 , 
            Port.DataBits     = 8    ;  //8
            Port.Parity       = Parity  .None;//None = 0,Odd = 1,Even = 2,Mark = 3 ,Space = 4, 
            Port.StopBits     = StopBits.One ;//None = 0,One = 1,Two  = 2,OnePointFive = 3, None은 예외상황발생함..
            Port.ReadTimeout  = (int)500;
            Port.WriteTimeout = (int)500;

            Init();

        }

        ~RS485_ConverTech()
        {
            PortClose();
        }

        private void Init()
        {
            //Init
            Para[0].dVCapacity = 100000 ; 
            Para[0].dACapacity = 60000  ;

            Para[1].dVCapacity = 6000   ; 
            Para[1].dACapacity = 50000  ;

            Para[2].dVCapacity = 6000   ; 
            Para[2].dACapacity = 50000  ;

        }

        private bool PortOpen()
        {
            try
            {
                Port.Open(); 
            }
            catch
            {
                Log.ShowMessage(sName + " COM PORT ERROR", Port.PortName + " COM Port not Exist"); 
                //MessageBox.Show(new Form(){TopMost = true}, sName + " COM PORT ERROR", Port.PortName + " COM Port Not Exist");
                return false;
            }    
            if (!Port.IsOpen)
            {
                //MessageBox.Show(new Form(){TopMost = true}, sName + " COM PORT ERROR", Port.PortName + " COM Port Not Opened");
                Log.ShowMessage(sName + " COM PORT ERROR", Port.PortName + " COM Port not opened");   
                return false;
            }
            return true;            
        }

        private void PortClose()
        {
            Port.Close();
            Port.Dispose();
        }

        private bool SendMsg(string _sMsg)
        {
            Delay(15);

            int iMsgLeghth = _sMsg.Length;
            byte[] ByteMsg = new Byte[iMsgLeghth];
            ByteMsg = Encoding.ASCII.GetBytes(_sMsg);

            return SendMsg(ByteMsg) ;
        }
        
        private bool SendMsg(byte [] _baMsg)
        {
            //lock(lockObject)
            //{ 
            if (!Port.IsOpen) return false ;
            //bRcvEnd    = false;
            sRecvedMsg = "";
            Encoding enc = Encoding.GetEncoding("iso-8859-1");
            sSendedMsg = enc.GetString(_baMsg);
            Port.Write(_baMsg, 0, _baMsg.Length);
            return true ;
            //}
            
        }
        
        static public object lockObject = new object();
        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            lock(lockObject)
            { 
            int iByteCntToRead = Port.BytesToRead ;
            if (iByteCntToRead <= 0) return ;            

            byte[] ByteRead = new byte[iByteCntToRead];
            int iReadCount = Port.Read(ByteRead, 0, iByteCntToRead);
            sRecvedMsg += Encoding.ASCII.GetString(ByteRead, 0, iReadCount);
            string sMsg = sRecvedMsg;
            bool bRet1 = sMsg.Split(cETX).Length >= 3 && sMsg.Split(cSTX).Length >= 3 && sMsg.IndexOf("@A@@") >= 0; //A Ack
            bool bRet2 = sMsg.Split(cETX).Length >= 3 && sMsg.Split(cSTX).Length >= 3 && sMsg.IndexOf("AA@@") >= 0; //A Ack
            bool bRet3 = sMsg.Split(cETX).Length >= 3 && sMsg.Split(cSTX).Length >= 3 && sMsg.IndexOf("BA@@") >= 0; //A Ack
            //bool bRet4 = sMsg.Split(cSTX).Length >= 3 && sMsg.Split(cACK).Length >= 3 ;//&& sMsg.IndexOf("BA@@") >= 0; //A Ack
            bool bRet4 = sMsg.IndexOf(cSTX)      >= 0 && sMsg.IndexOf(cACK ) >= 0 ; //A Ack

            //int i1 = sMsg.Split(cETX).Length - 1;
            int i1 = sMsg.Split(cETX).Length;
            if(bRet4) 
            { 
                if(sRecvedMsg != "") sRecvedMsg.Remove(sMsg.IndexOf(cSTX) , sMsg.IndexOf(cACK) - sMsg.IndexOf(cSTX) + 1); 
                //Stat[iId].bRcvEnd = true;
                return; 
            }
            if (!bRet1 && !bRet2 && !bRet3) return;
         
            

            if(PreStat[iPreId].sRsv != sMsg)
            {
                if (OM.MstOptn.bDebugMode) Log.Trace(sMsg,LOG_ID);
            }
            PreStat[iPreId].sRsv = sMsg ;

            
            //STX ETX 빼고 그사이것만 가져옴.
            //상태읽기
            //"\u0002@A@@\u0003\u0002@@@A@@@@S@@@@\u0003"
            //"\u0002AA@@\u0003\u0002@A@A@@@@S@@@@\u0003"
            //"\u0002BA@@\u0003\u0002@A@@@@@@S@@@@\u0003"

            //출력온
            //"\u0002@D@@\u0003\u0006" //ON
            //"\u0002@E@@\u0003\u0006" //OFF
            //"\u0002AD@@\u0003\u0006" //ON
            //"\u0002AE@@\u0003\u0006" //OFF
            //"\u0002BD@@\u0003\u0006" //ON
            //"\u0002BE@@\u0003\u0006" //OFF

            //전체상태 가져오는 것의 응답.
            //CommentA
            int iId = 0;
            if (sMsg.Length == 21)
            {
                string sEcho = sMsg.Substring(sMsg.IndexOf(cSTX) + 1, 1);

                     if(sEcho == "@") iId = 0;
                else if(sEcho == "A") iId = 1;
                else if(sEcho == "B") iId = 2;
                else                  return ;

                sEcho = sMsg.Remove(sMsg.IndexOf(cSTX) , sMsg.IndexOf(cETX) - sMsg.IndexOf(cSTX) + 1);
                string sTemp = sEcho.Substring(sEcho.IndexOf(cSTX) , sEcho.IndexOf(cETX) - sEcho.IndexOf(cSTX) + 1);
                //측정전압
                string sValue1 , sValue2;
                int    iValue1 , iValue2;
                int    iValue ;
                char   cValue1 , cValue2;
                
                if(sTemp == "") return;
                //볼트 이것은 0~1000까지 들어오고 비율값임 즉 %임.   
                //10V 300A 사양이면 10V에 대한 퍼센트.
                sValue1 = sTemp.Substring(1, 1); //High
                sValue2 = sTemp.Substring(2, 1); //Low
                iValue1 = sValue1[0] ;
                iValue2 = sValue2[0] ;
                iValue        = (iValue1&0x3f) * 64 + (iValue2&0x3f) ; //여기까진 퍼센트값.
                Stat[iId].dVoltage = (Para[iId].dVCapacity * iValue) / 1000000 ;

                //전류 이것은 0~1000까지 들어오고 비율값임 즉 %임.   
                //10V 300A 사양이면 300A에 대한 퍼센트.
                sValue1 = sTemp.Substring(3, 1); //High
                sValue2 = sTemp.Substring(4, 1); //Low
                iValue1 = sValue1[0] ;
                iValue2 = sValue2[0] ;                
                iValue        = (iValue1&0x3f) * 64 + (iValue2&0x3f) ;//여기는 퍼센트값.
                Stat[iId].dCurrent = (Para[iId].dACapacity * iValue) / 1000000 ;

                //설정볼트 이것은 0~1000까지 들어오고 비율값임 즉 %임.   
                //10V 300A 사양이면 10V에 대한 퍼센트.
                sValue1 = sTemp.Substring(5, 1); //High
                sValue2 = sTemp.Substring(6, 1); //Low
                iValue1 = sValue1[0] ;
                iValue2 = sValue2[0] ;
                iValue           = (iValue1&0x3f) * 64 + (iValue2&0x3f) ; //여기까진 퍼센트값.
                Stat[iId].dSetVoltage = (Para[iId].dVCapacity * iValue) / 1000000 ;

                //설정전류 이것은 0~1000까지 들어오고 비율값임 즉 %임.   
                //10V 300A 사양이면 300A에 대한 퍼센트.
                sValue1 = sTemp.Substring(7, 1); //High
                sValue2 = sTemp.Substring(8, 1); //Low
                iValue1 = sValue1[0] ;
                iValue2 = sValue2[0] ;                
                iValue           = (iValue1&0x3f) * 64 + (iValue2&0x3f) ;//여기는 퍼센트값.
                Stat[iId].dSetCurrent = (Para[iId].dACapacity * iValue) / 1000000 ;

                //Remote , Error Check
                sValue1 = sTemp.Substring(9 ,1); //Remote
                sValue2 = sTemp.Substring(11,1); //Error
                iValue1 = sValue1[0] ;
                iValue2 = sValue2[0] ;
                Stat[iId].bRemote = ((iValue1 >> 0) & 0x01) == 0x01; //Remote
                Stat[iId].bOutput = ((iValue1 >> 2) & 0x01) == 0x01; //현재 출력 ON, OFF상태
                if(iValue2 != 64)
                {
                    if(((iValue2 >> 0) & 0x01) == 0x01) Stat[iId].iError = 1 ;
                    if(((iValue2 >> 1) & 0x01) == 0x01) Stat[iId].iError = 2 ;
                    if(((iValue2 >> 2) & 0x01) == 0x01) Stat[iId].iError = 3 ;

                }
                Stat[iId].bRcvEnd = true;
            }
            else //if(sMsg.Length == 1)
            {
                //Stat[iId].bRcvEnd = true;
                //sRecvedMsg = "";
            }

            //sRecvedMsg = "";
            //Stat[iId].bRcvEnd = true;
            //bRcvEnd = true;
            }
        }

        private void Load(string _sName , string _sPath = "")
        {
            //Set Dir.
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sFileName = "Util\\SerialPort" ;
            string sSerialPath = sExeFolder + sFileName + ".ini";
            string sTitle = "";
            
            if(sName=="")sTitle = "PortID_" + iPortId.ToString();
            else         sTitle = _sName ;

            CIniFile Ini ;
            if(_sPath == "")Ini = new CIniFile(sSerialPath);
            else            Ini = new CIniFile(_sPath     );

            Ini.Load(sTitle, "PortNo"    , out iPortNo    ); if(iPortNo    ==0) iPortNo = 1 ;
            Ini.Save(sTitle, "PortNo"    ,     iPortNo    );

            
        }

        //A 파워서플라이의 상태를 전송.
        public bool SendA(int _iId)
        {
            if(_iId < 0 || _iId >= MAX_ARRAY) return false;
            Delay(15);
            Stat[_iId].Clear();
            Stat[_iId].bRcvEnd = false;
            //sun.       
            //iId = _iId ;
            int iSendId = _iId + cSTR ;
            string sSend = cSTX.ToString() + (char)(iSendId) + "A" + cSTR.ToString() + cSTR.ToString() + cETX.ToString();

            if(PreStat[_iId].sSendA != sSend)
            {
                if (OM.MstOptn.bDebugMode) Log.Trace("SendA_" + _iId.ToString() + "_" + sSend,LOG_ID);
            }
            PreStat[_iId].sSendA = sSend;

            iPreId = _iId;

            Stat[_iId].sMsg = "";
            return SendMsg(sSend);

        }


        //B 전압설정.
        public bool SendB(int _iId , double _dVolt)
        {
            if(_iId < 0 || _iId >= MAX_ARRAY ) return false;
            if(Para[_iId].dVCapacity <= 0    ) return false;
            if(Para[_iId].dVCapacity < _dVolt) return false;
            Delay(15);
            //Stat[_iId].bRcvEnd = false;

            int iSendId = _iId + cSTR ;

            //Stat[iId].Clear();
            double dVolt = _dVolt * 1000 ;

            double dSetPer = dVolt / Para[_iId].dVCapacity ;
            int    iSetPer = (int)(dSetPer * 1000) ;
            byte   bSetPer1 = (byte)(iSetPer / 64) ;
            //bSetPer1 |= cSTR ;
            byte   bSetPer2 = (byte)(iSetPer % 64) ;
            //bSetPer2 |= cSTR ;
            string sSend = cSTX.ToString() + (char)(iSendId) + "B" + (char)(bSetPer1 | cSTR) + (char)(bSetPer2 | cSTR) + cETX.ToString();

            if(PreStat[_iId].sSendB != sSend)
            {
                if (OM.MstOptn.bDebugMode) Log.Trace("SendB_" + _iId.ToString() + "_" + sSend,LOG_ID);
            }
            PreStat[_iId].sSendB = sSend;

            Stat[_iId].sMsg = "";
            return SendMsg(sSend);

        }

        //C 전류설정.
        public bool SendC(int _iId , double _dAmpere)
        {
            if(_iId < 0 || _iId >= MAX_ARRAY   ) return false;
            if(Para[_iId].dACapacity <= 0      ) return false;
            if(Para[_iId].dACapacity < _dAmpere) return false;
            //Stat[_iId].bRcvEnd = false;
            Delay(15);

            int iSendId = _iId + cSTR ;

            //Stat[iId].Clear();
            double dAmpere = _dAmpere * 1000 ;

            double dSetPer = dAmpere / Para[_iId].dACapacity ;
            int    iSetPer = (int)(dSetPer * 1000) ;
            byte   bSetPer1 = (byte)(iSetPer / 64) ;
            //bSetPer1 |= 0x40 ;
            byte   bSetPer2 = (byte)(iSetPer % 64) ;
            //bSetPer2 |= 0x40 ;
            string sSend = cSTX.ToString() + (char)(iSendId) + "C" + (char)(bSetPer1 | cSTR) + (char)(bSetPer2 | cSTR) + cETX.ToString();

            if(PreStat[_iId].sSendC != sSend)
            {
                if (OM.MstOptn.bDebugMode) Log.Trace("SendC_" + _iId.ToString() + "_" + sSend,LOG_ID);
            }
            PreStat[_iId].sSendC = sSend;

            Stat[_iId].sMsg = "";
            return SendMsg(sSend);

        }

        /// <summary>
        /// 출력 ON
        /// </summary>
        /// <param name="_iId"></param>
        /// <returns></returns>
        public bool SendD(int _iId)
        {
            if(_iId < 0 || _iId >= MAX_ARRAY) return false;
            //Stat[_iId].bRcvEnd = false;
            Delay(10);
            
            int iSendId = _iId + cSTR ;
            //string sSend = cSTX.ToString() + (_iId | 0x40).ToString() + "D" + 0x40.ToString() + 0x40.ToString() + cETX.ToString();
            string sSend = cSTX.ToString() + (char)(iSendId) + "D" + cSTR.ToString() + cSTR.ToString() + cETX.ToString();
            //string sSend = "AD@@";

            if(PreStat[_iId].sSendD != sSend)
            {
                if (OM.MstOptn.bDebugMode) Log.Trace("SendD_" + _iId.ToString() + "_" + sSend,LOG_ID);
            }
            PreStat[_iId].sSendD = sSend;

            Stat[_iId].sMsg = "";
            return SendMsg(sSend);

        }

        /// <summary>
        /// 출력 OFF
        /// </summary>
        /// <param name="_iId"></param>
        /// <returns></returns>
        public bool SendE(int _iId)
        {
            if(_iId < 0 || _iId >= MAX_ARRAY) return false;
            //Stat[_iId].bRcvEnd = false;
            Delay(10);

            int iSendId = _iId + cSTR ;
            string sSend = cSTX.ToString() + (char)(iSendId) + "E" + cSTR.ToString() + cSTR.ToString() + cETX.ToString();

            if(PreStat[_iId].sSendE != sSend)
            {
                if (OM.MstOptn.bDebugMode) Log.Trace("SendE_" + _iId.ToString() + "_" + sSend,LOG_ID);
            }
            PreStat[_iId].sSendE = sSend;

            Stat[_iId].sMsg = "";
            return SendMsg(sSend);

        }



        public bool IsReceiveEnd(int _iId)
        {
            if(_iId < 0 || _iId >= MAX_ARRAY) return false;
            return Stat[_iId].bRcvEnd;
        }                        

        public string GetRecvedMsg()
        {
            return sRecvedMsg;
        }

        public string GetSendedMsg()
        {
            return sSendedMsg;
        }

        public void Delay(int ms)
        {
            DateTime dateTimeNow = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, ms);
            DateTime dateTimeAdd = dateTimeNow.Add(duration);
            while (dateTimeAdd >= dateTimeNow)
            {
                System.Windows.Forms.Application.DoEvents();
                dateTimeNow = DateTime.Now;
            }
            return;
        }
    }
}
