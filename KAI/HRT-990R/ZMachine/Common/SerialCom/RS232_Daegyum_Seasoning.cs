using COMMON;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Machine
{
    public class RS232_Daegyum_Seasoning
    {
        private  SerialPort Port = null;

        public   const int    LOG_ID    = 8 ;

        private   int    iPortId    = 0 ;
        private   int    iPortNo    = 1 ;
        private   string sName      = "";
        private   int    iId        = 0 ;

        private   string sSetMsg    = "";
        private   string sRecvedMsg = "";
        private   string sSendedMsg = "";
        private   bool   bRcvEnd    = true;

        private   const char   cACK  = (char)0x06 ;

        public struct TStat
        { 
            //public double dAnode      ;//
            public double dGate       ;//
            public double dFocus      ;//
            public double dCathod     ;//
            public int    iArc        ;

            public void Clear()
            {
                //dAnode    = 0 ;
                dGate     = 0 ;
                dFocus    = 0 ;
                dCathod   = 0 ;
                iArc      = 0 ;
            }
        };
        public TStat Stat;

        public struct TPreStat
        { 
            //public double dAnode      ;//
            public double dGate       ;//
            public double dFocus      ;//
            public double dCathod     ;//
            public int    iArc        ;

            public void Clear()
            {
                //dAnode    = 0 ;
                dGate     = 0 ;
                dFocus    = 0 ;
                dCathod   = 0 ;
                iArc      = 0 ;
            }
        };
        public TPreStat PreStat;
        //파일로 초기화.
        //_iComNo은 1번부터 입력 되어야 한다.
        public RS232_Daegyum_Seasoning(int _iPortId , string _sName)
        {           
            iPortId    = _iPortId   ;
            sName      = _sName     ;

            Port = new SerialPort();
            Port.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
            //Port.Encoding = System.Text.Encoding.GetEncoding("iso-8859-1");//이것이 8비트 문자 모두 가능 Ascii는 7비트라 63이상의 값은 표현 안됌.

            Stat    = new TStat   ();
            PreStat = new TPreStat();

            Load(sName); 

            Port.PortName= "Com" + iPortNo.ToString();
            PortOpen();

            Port.BaudRate     = 57600; //9600 , 
            Port.DataBits     = 8    ;  //8
            Port.Parity       = Parity  .None;//None = 0,Odd = 1,Even = 2,Mark = 3 ,Space = 4, 
            Port.StopBits     = StopBits.One ;//None = 0,One = 1,Two  = 2,OnePointFive = 3, None은 예외상황발생함..
            Port.ReadTimeout  = (int)500;
            Port.WriteTimeout = (int)500;
        }

        ~RS232_Daegyum_Seasoning()
        {
            PortClose();
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
            if (!Port.IsOpen) return false ;
            //bRcvEnd    = false;
            sRecvedMsg = "";
            Encoding enc = Encoding.GetEncoding("iso-8859-1");
            sSendedMsg = enc.GetString(_baMsg);
            Port.Write(_baMsg, 0, _baMsg.Length);

            return true ;
        }
        
        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int iByteCntToRead = Port.BytesToRead ;
            if (iByteCntToRead <= 0) return ;            

            byte[] ByteRead = new byte[iByteCntToRead];
            int iReadCount = Port.Read(ByteRead, 0, iByteCntToRead);
            sRecvedMsg += Encoding.ASCII.GetString(ByteRead, 0, iReadCount);
            string sMsg = sRecvedMsg;
            bool bRet1 = sMsg.IndexOf("]AC:") >= 0 && sMsg.IndexOf("\r\n") >= 0; //]AC:nnnnn\r\n 0~65535
            bool bRet2 = sMsg.IndexOf("]IC:") >= 0 && sMsg.IndexOf("\r\n") >= 0; //]IC:nnnnn\r\n 0~65535
            bool bRet3 = sMsg.IndexOf("]IF:") >= 0 && sMsg.IndexOf("\r\n") >= 0; //]IF:nnnnn\r\n 0~65535
            bool bRet4 = sMsg.IndexOf("]IG:") >= 0 && sMsg.IndexOf("\r\n") >= 0; //]IG:nnnnn\r\n 0~65535
            bool bRet5 = sMsg.IndexOf("]IA:") >= 0 && sMsg.IndexOf("\r\n") >= 0; //]IG:nnnnn\r\n 0~65535
            
            bool bRet0 = sMsg.IndexOf(cACK) >= 0 ;
            if(bRet0)
            {
                     if(sOut == "SendOutOn" ) { bOut = true ; sOut = ""; }// bRcvEnd = true;}
                else if(sOut == "SendOutOff") { bOut = false; sOut = ""; }//bRcvEnd = true;}

                     if(sOut == "OnTime" ) { dOnTime  = dOnTimeTemp ; sOut = ""; }//bRcvEnd = true;} 
                else if(sOut == "OffTime") { dOffTime = dOffTimeTemp; sOut = ""; }//bRcvEnd = true;}

                //     if(sOut == "SendArc"     ) { sOut = ""; bRcvEnd = true;}
                if(sOut == "SendArcReset") { sOut = ""; }//bRcvEnd = true;}

                if(sOut == "SendDisplay" ) { sOut = ""; }//bRcvEnd = true;}

                sRecvedMsg.Replace(cACK.ToString(),"");
                //if (OM.MstOptn.bDebugMode)
                //{
                //    Log.Trace(cACK.ToString(), LOG_ID);
                //}
            }

            if (!bRet1 && !bRet2 && !bRet3 && !bRet4 && !bRet5) return;
            if (OM.MstOptn.bDebugMode)
            {
                Log.Trace(sMsg, LOG_ID);
            }

            int iStt = sMsg.IndexOf(":"   ) + 1   ;  
            int iEnd = sMsg.IndexOf("\r\n") - iStt;
            string sTemp = sMsg.Substring(iStt,iEnd);
                 if(bRet1) {Stat.iArc    = CConfig.StrToIntDef   (sTemp,0);       if(PreStat.iArc    != Stat.iArc    ) {Log.Trace("Arc Count : "    + Stat.iArc   .ToString(),LOG_ID); } PreStat.iArc    = Stat.iArc    ;}
            else if(bRet2) {Stat.dCathod = CConfig.StrToDoubleDef(sTemp,0) / 100; if(PreStat.dCathod != Stat.dCathod ) {Log.Trace("Cathod Count : " + Stat.dCathod.ToString(),LOG_ID); } PreStat.dCathod = Stat.dCathod ;}
            else if(bRet3) {Stat.dFocus  = CConfig.StrToDoubleDef(sTemp,0) / 10 ; if(PreStat.dFocus  != Stat.dFocus  ) {Log.Trace("Focus Count : "  + Stat.dFocus .ToString(),LOG_ID); } PreStat.dFocus  = Stat.dFocus  ;}
            else if(bRet4) {Stat.dGate   = CConfig.StrToDoubleDef(sTemp,0) / 10 ; if(PreStat.dGate   != Stat.dGate   ) {Log.Trace("Gate Count : "   + Stat.dGate  .ToString(),LOG_ID); } PreStat.dGate   = Stat.dGate   ;}
            else if(bRet5) {
                string sData = "";
                sData = sTemp.Substring( 0, 4);
                Stat.dCathod = CConfig.StrToDoubleDef(sData,0) /100 * OM.CmnOptn.dCMul + OM.CmnOptn.dCAdd; 
                if(PreStat.dCathod  != Stat.dCathod  ) {Log.Trace("Cathod Count : "  + Stat.dCathod .ToString(),LOG_ID); } 
                sData = sTemp.Substring( 5, 4);
                Stat.dFocus  = CConfig.StrToDoubleDef(sData,0) /10  * OM.CmnOptn.dFMul + OM.CmnOptn.dFAdd; 
                if(PreStat.dFocus   != Stat.dFocus   ) {Log.Trace("Focus  Count : "  + Stat.dFocus  .ToString(),LOG_ID); } 
                sData = sTemp.Substring(10, 4);
                Stat.dGate   = CConfig.StrToDoubleDef(sData,0) /10  * OM.CmnOptn.dGMul + OM.CmnOptn.dGAdd; 
                if(PreStat.dGate    != Stat.dGate    ) {Log.Trace("Gate   Count : "  + Stat.dGate   .ToString(),LOG_ID); } 

                PreStat.dCathod = Stat.dCathod ;
                PreStat.dFocus  = Stat.dFocus  ;
                PreStat.dGate   = Stat.dGate   ;
            }
            
            bRcvEnd = true;
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
        public  double  dOnTime      = 0 ;
        private double  dOnTimeTemp  = 0 ;
        public  double  dOffTime     = 0 ;
        private double  dOffTimeTemp = 0 ;
        public bool SendOnTime(double _dOnTime)
        {
            Delay(10);
            sOut = "OnTime" ;
            string sSend = "[PON," + _dOnTime.ToString() + "#" ;
            dOnTimeTemp = _dOnTime ;
            if (OM.MstOptn.bDebugMode) Log.Trace(sSend,LOG_ID);
            return SendMsg(sSend);
        }
        public bool SendOffTime(double _dOffTime)
        {
            Delay(10);
            sOut = "OffTime" ;
            string sSend = "[POF," + _dOffTime.ToString() + "#" ;
            dOffTimeTemp = _dOffTime ;
            if (OM.MstOptn.bDebugMode) Log.Trace(sSend,LOG_ID);
            return SendMsg(sSend);
        }

        private string sOut = "";
        public  bool   bOut = false;
        public bool SendOutOnOff(bool _bOn)
        {
            if(_bOn) sOut = "SendOutOn" ;
            else     sOut = "SendOutOff";

            int iOn = Convert.ToInt32(_bOn);
            string sSend = "[OUT," + iOn.ToString() + "#" ;
            if (OM.MstOptn.bDebugMode) Log.Trace(sSend,LOG_ID);
            return SendMsg(sSend);
        }

        public bool SendArc()
        {
            bRcvEnd = false;
            sOut = "SendArc" ;
            string sSend = "[ARC#" ;
            if (OM.MstOptn.bDebugMode) Log.Trace(sSend,LOG_ID);
            return SendMsg(sSend);
        }
        public bool SendArcReset()
        {
            sOut = "SendArcReset" ;
            string sSend = "[RST#" ;
            if (OM.MstOptn.bDebugMode) Log.Trace(sSend,LOG_ID);
            return SendMsg(sSend);
        }
        /// <summary>
        /// Display 모드 변경
        /// </summary>
        /// <param name="_bOn">0이면 캐소드 전류 표시 , 1이면 아킹 카운트값 표시</param>
        /// <returns></returns>
        public bool SendDisplay(bool _bOn)
        {
            sOut = "SendDisplay";
            int iOn = Convert.ToInt32(_bOn);
            string sSend = "[MOD,"+iOn.ToString()+"#" ;
            if (OM.MstOptn.bDebugMode) Log.Trace(sSend,LOG_ID);
            return SendMsg(sSend);
        }
        public bool SendCathod()
        {
            bRcvEnd = false;
            Stat.dCathod = 0;
            string sSend = "[HIC#" ;
            if (OM.MstOptn.bDebugMode) Log.Trace(sSend,LOG_ID);
            return SendMsg(sSend);
        }
        public bool SendFocus()
        {
            bRcvEnd = false;
            Stat.dFocus = 0;
            string sSend = "[HIF#" ;
            if (OM.MstOptn.bDebugMode) Log.Trace(sSend,LOG_ID);
            return SendMsg(sSend);
        }
        public bool SendGate()
        {
            bRcvEnd = false;
            Stat.dGate = 0;
            string sSend = "[HIG#" ;
            if (OM.MstOptn.bDebugMode) Log.Trace(sSend,LOG_ID);
            return SendMsg(sSend);
        }
        public bool SendAll()
        {
            bRcvEnd = false;
            Stat.dCathod = 0;
            Stat.dFocus  = 0;
            Stat.dGate   = 0;
            string sSend = "[CFG#" ;
            if (OM.MstOptn.bDebugMode) Log.Trace(sSend,LOG_ID);
            return SendMsg(sSend);
        }
        public bool SendLive()
        {
            string sSend = "[@#" ;
            if (OM.MstOptn.bDebugMode) Log.Trace(sSend,LOG_ID);
            return SendMsg(sSend);
            //return false;
        }

        public bool IsReceiveEnd()
        {
            return bRcvEnd; 
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
