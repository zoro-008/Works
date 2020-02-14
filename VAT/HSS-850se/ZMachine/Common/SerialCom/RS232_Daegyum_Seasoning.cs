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
        private  SerialPort Port    = null;
        private  SerialPort Port1   = null;
        private  SerialPort Port2   = null;

        public   const int    LOG_ID    = 8 ;

        private   int    iPortId    = 0 ; private   int    iPortId1    = 0 ; private   int    iPortId2    = 0 ;
        private   int    iPortNo    = 1 ; private   int    iPortNo1    = 1 ; private   int    iPortNo2    = 1 ;
        private   string sName      = "";
        private   int    iId        = 0 ;

        private   string sSetMsg    = "";
        private   string sRecvedMsg = ""; private   string sRecvedMsg1 = ""; private   string sRecvedMsg2 = "";
        private   string sSendedMsg = ""; private   string sSendedMsg1 = ""; private   string sSendedMsg2 = "";
        private   bool   bRcvEnd    = true; 
        private   bool   bRcvEnd1   = true; 
        private   bool   bRcvEnd2   = true;

        public    bool   bFocusEnd { get { return bRcvEnd1; } set { bRcvEnd1 = value; } }
        public    bool   bGateEnd  { get { return bRcvEnd2; } set { bRcvEnd2 = value; } }

        
        protected CDelayTimer m_tmOut ;

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

            Port1 = new SerialPort();
            Port1.DataReceived += new SerialDataReceivedEventHandler(DataReceived1);

            Port2 = new SerialPort();
            Port2.DataReceived += new SerialDataReceivedEventHandler(DataReceived2);
            //Port.Encoding = System.Text.Encoding.GetEncoding("iso-8859-1");//이것이 8비트 문자 모두 가능 Ascii는 7비트라 63이상의 값은 표현 안됌.

            Stat    = new TStat   ();
            PreStat = new TPreStat();
            
            m_tmOut = new CDelayTimer();

            Load(sName); 

            Port.PortName= "Com" + iPortNo.ToString();
            //PortOpen();
            
            Port.BaudRate     = 57600; //9600 , 
            Port.DataBits     = 8    ;  //8
            Port.Parity       = Parity  .None;//None = 0,Odd = 1,Even = 2,Mark = 3 ,Space = 4, 
            Port.StopBits     = StopBits.One ;//None = 0,One = 1,Two  = 2,OnePointFive = 3, None은 예외상황발생함..
            Port.ReadTimeout  = (int)500;
            Port.WriteTimeout = (int)500;

            Port1.PortName= "Com" + iPortNo1.ToString();
            //PortOpen();

            Port1.BaudRate     = 19200; //9600 , 
            Port1.DataBits     = 8    ;  //8
            Port1.Parity       = Parity  .None;//None = 0,Odd = 1,Even = 2,Mark = 3 ,Space = 4, 
            Port1.StopBits     = StopBits.One ;//None = 0,One = 1,Two  = 2,OnePointFive = 3, None은 예외상황발생함..
            Port1.ReadTimeout  = (int)500;
            Port1.WriteTimeout = (int)500;

            Port2.PortName= "Com" + iPortNo2.ToString();
            PortOpen();

            Port2.BaudRate     = 19200; //9600 , 
            Port2.DataBits     = 8    ;  //8
            Port2.Parity       = Parity  .None;//None = 0,Odd = 1,Even = 2,Mark = 3 ,Space = 4, 
            Port2.StopBits     = StopBits.One ;//None = 0,One = 1,Two  = 2,OnePointFive = 3, None은 예외상황발생함..
            Port2.ReadTimeout  = (int)500;
            Port2.WriteTimeout = (int)500;
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
                Port1.Open(); 
                Port2.Open(); 
            }
            catch
            {
                Log.ShowMessage(sName + " COM PORT ERROR", Port.PortName + " COM Port not Exist"); 
                //MessageBox.Show(new Form(){TopMost = true}, sName + " COM PORT ERROR", Port.PortName + " COM Port Not Exist");
                return false;
            }    
            string sPortName = "";//Port.PortName ;
            if(!Port .IsOpen) sPortName = Port .PortName ;
            if(!Port1.IsOpen) sPortName = Port1.PortName ;
            if(!Port2.IsOpen) sPortName = Port2.PortName ;
            if(sPortName != "")
            {
                Log.ShowMessage(sName + " COM PORT ERROR", sPortName + " COM Port not opened");   
                return false;
            }
            return true;            
        }

        private void PortClose()
        {
            Port .Close();
            Port .Dispose();
            Port1.Close();
            Port1.Dispose();
            Port2.Close();
            Port2.Dispose();
        }

        private bool SendMsg(string _sMsg,int _iId = 0)
        {
            //Delay(15);

            int iMsgLeghth = _sMsg.Length;
            byte[] ByteMsg = new Byte[iMsgLeghth];
            ByteMsg = Encoding.ASCII.GetBytes(_sMsg);

                 if(_iId == 1) return SendMsg1(ByteMsg) ;
            else if(_iId == 2) return SendMsg2(ByteMsg) ;
            return SendMsg(ByteMsg) ;
        }

        private bool SendMsg(byte [] _baMsg)
        {
            if (!Port.IsOpen) return false ;
            //bRcvEnd    = false;
            sRecvedMsg = "";
            iLow   = -999 ;
            iHigh  = -999 ;
            Encoding enc = Encoding.GetEncoding("iso-8859-1");
            sSendedMsg = enc.GetString(_baMsg);
            Port.Write(_baMsg, 0, _baMsg.Length);

            return true ;
        }
        private bool SendMsg1(byte [] _baMsg)
        {
            if (!Port1.IsOpen) return false ;
            //bRcvEnd    = false;
            sRecvedMsg1 = "";
            iLow1  = -999 ;
            iHigh1 = -999 ;
            Encoding enc = Encoding.GetEncoding("iso-8859-1");
            sSendedMsg = enc.GetString(_baMsg);
            Port1.Write(_baMsg, 0, _baMsg.Length);

            return true ;
        }
        private bool SendMsg2(byte [] _baMsg)
        {
            if (!Port2.IsOpen) return false ;
            //bRcvEnd    = false;
            sRecvedMsg2 = "";
            iLow2  = -999 ;
            iHigh2 = -999 ;
            Encoding enc = Encoding.GetEncoding("iso-8859-1");
            sSendedMsg = enc.GetString(_baMsg);
            Port2.Write(_baMsg, 0, _baMsg.Length);

            return true ;
        }
        private double Correction(double dVal, int iId =0)
        {
            double dRet = 0    ;
            double A = 0 , B = 0 , C = 0;

            if(iId == 0) {
                A = OM.CmnOptn.dCA ;
                B = OM.CmnOptn.dCB ;
                C = OM.CmnOptn.dCC ;
                if(!OM.CmnOptn.bCUse) return dVal;
            }
            else if(iId == 1) {
                A = OM.CmnOptn.dFA ;
                B = OM.CmnOptn.dFB ;
                C = OM.CmnOptn.dFC ;
                if(!OM.CmnOptn.bFUse) return dVal;
            }
            else if(iId == 2) {
                A = OM.CmnOptn.dGA ;
                B = OM.CmnOptn.dGB ;
                C = OM.CmnOptn.dGC ;
                if(!OM.CmnOptn.bGUse) return dVal;
            }

            dRet = C * Math.Pow(dVal-A,2) + B ;
            //double dTemp = Math.Sqrt(dVal-A);
            return Math.Round(dRet,2);
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
            
            if(sOut == "SendCathod"  ) { 

                if(iReadCount == 1 && iLow == -999)
                {
                    iLow  = ByteRead[0]      ;
                    return ;
                }
                else if(iReadCount == 1)
                {
                    iHigh = ByteRead[0] << 8 ;
                }
                else if(iReadCount == 2)
                {
                    iLow  = ByteRead[0]      ;
                    iHigh = ByteRead[1] << 8 ;
                }

                int iData = iHigh + iLow     ;
                Stat.dCathod = (double)iData/100.0; 
                if(PreStat.dCathod != Stat.dCathod ) {
                    Log.Trace("Cathod Count : " + Stat.dCathod.ToString(),LOG_ID); 
                } 
                PreStat.dCathod = Stat.dCathod ;
                sOut    = "";
                bRcvEnd = true;
                return ;

                
            }


            bool bRet0 = sMsg.IndexOf(cACK) >= 0 ;

            if(bRet0)
            {
                     if(sOut == "SendOutOn"   ) { bOut = true ;            }
                else if(sOut == "SendOutOff"  ) { bOut = false;            }
                else if(sOut == "SendOnTime"  ) { dOnTime  = dOnTimeTemp ; } 
                else if(sOut == "SendOffTime" ) { dOffTime = dOffTimeTemp; }
                else if(sOut == "SendArcReset") {                          }
                else if(sOut == "SendDisplay" ) {                          }
                else if(sOut == "SendLive"    ) {                          }
                //else if(sOut == "SendReset"   ) {                          }

                sOut    = "";
                bRcvEnd = true;
                return ;
                //sRecvedMsg.Replace(cACK.ToString(),"");
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
            else if(bRet2) {Stat.dCathod = Correction(CConfig.StrToDoubleDef(sTemp,0) / 100,0); if(PreStat.dCathod != Stat.dCathod ) {Log.Trace("Cathod Count : " + Stat.dCathod.ToString(),LOG_ID); } PreStat.dCathod = Stat.dCathod ;}
            else if(bRet3) {Stat.dFocus  = Correction(CConfig.StrToDoubleDef(sTemp,0) / 100,1); if(PreStat.dFocus  != Stat.dFocus  ) {Log.Trace("Focus Count : "  + Stat.dFocus .ToString(),LOG_ID); } PreStat.dFocus  = Stat.dFocus  ;}
            else if(bRet4) {Stat.dGate   = Correction(CConfig.StrToDoubleDef(sTemp,0) / 100,2); if(PreStat.dGate   != Stat.dGate   ) {Log.Trace("Gate Count : "   + Stat.dGate  .ToString(),LOG_ID); } PreStat.dGate   = Stat.dGate   ;}
            else if(bRet5) {
                string sData = "";
                sData = sTemp.Substring( 0, 4);
                Stat.dCathod = Correction(CConfig.StrToDoubleDef(sData,0) /100 ,0); 
                if(PreStat.dCathod  != Stat.dCathod  ) {Log.Trace("Cathod Count : "  + Stat.dCathod .ToString(),LOG_ID); } 
                sData = sTemp.Substring( 5, 4);
                Stat.dFocus  = Correction(CConfig.StrToDoubleDef(sData,1) /100 ,0); 
                if(PreStat.dFocus   != Stat.dFocus   ) {Log.Trace("Focus  Count : "  + Stat.dFocus  .ToString(),LOG_ID); } 
                sData = sTemp.Substring(10, 4);
                Stat.dGate   = Correction(CConfig.StrToDoubleDef(sData,2) /100 ,0); 
                if(PreStat.dGate    != Stat.dGate    ) {Log.Trace("Gate   Count : "  + Stat.dGate   .ToString(),LOG_ID); } 

                PreStat.dCathod = Stat.dCathod ;
                PreStat.dFocus  = Stat.dFocus  ;
                PreStat.dGate   = Stat.dGate   ;
            }
            
            bRcvEnd = true;
        }

        private int iLow   = -999 ;
        private int iHigh  = -999 ;
        private int iLow1  = -999 ;
        private int iHigh1 = -999 ;
        private int iLow2  = -999 ;
        private int iHigh2 = -999 ;

        private void DataReceived1(object sender, SerialDataReceivedEventArgs e)
        {
            //lock(lockObject)
            //{ 
            int iByteCntToRead = Port1.BytesToRead ;
            if (iByteCntToRead <= 0) return ;            

            byte[] ByteRead = new byte[iByteCntToRead];
            int iReadCount = Port1.Read(ByteRead, 0, iByteCntToRead);
            sRecvedMsg1 += Encoding.ASCII.GetString(ByteRead, 0, iReadCount);
            string sMsg = sRecvedMsg1;

            if(iReadCount == 1 && iLow1 == -999)
            {
                iLow1  = ByteRead[0]      ;
                return ;
            }
            else if(iReadCount == 1)
            {
                iHigh1 = ByteRead[0] << 8 ;
            }
            else if(iReadCount == 2)
            {
                iLow1  = ByteRead[0]      ;
                iHigh1 = ByteRead[1] << 8 ;
            }

            //if(iReadCount == 2)
            //{
                //int iLow  = ByteRead[0]      ;
                //int iHigh = ByteRead[1] << 8 ;
                int iData = iHigh1 + iLow1     ;
                double dFocus = (double)iData/100.0; 

                if(dFocus > 0 && dFocus < 0.3)
                {
                    Stat.dFocus = dFocus;
                }
                else if(dFocus < 0)
                {
                    Stat.dFocus = 0;
                }
                else
                {
                    Stat.dFocus = Correction(dFocus,1); 
                }

                if(PreStat.dFocus != Stat.dFocus) {
                    Log.Trace("Focus Count : " + Stat.dFocus.ToString(),LOG_ID); 
                } 
                PreStat.dFocus = Stat.dFocus ;

                bRcvEnd1 = true;
            //}    
                
            //bool bRet0 = sMsg.IndexOf("[I") >= 0 && sMsg.IndexOf("#") >= 0; //]IG:nnnnn\r\n 0~65535
            //
            //if(!bRet0) return ;
            //
            //int iStt = sMsg.IndexOf("[I"   ) + 2   ;  
            //int iEnd = sMsg.IndexOf("#") - iStt;
            //string sTemp = sMsg.Substring(iStt,iEnd);
            //
            //string sData = sTemp;
            //Stat.dFocus  = Correction(CConfig.StrToDoubleDef(sData,0) /100,1); 


            //Stat[iId].bRcvEnd = true;
            //bRcvEnd1 = true;
        }

        private void DataReceived2(object sender, SerialDataReceivedEventArgs e)
        {
            //lock(lockObject)
            //{ 
            int iByteCntToRead = Port2.BytesToRead ;
            if (iByteCntToRead <= 0) return ; 

            byte[] ByteRead = new byte[iByteCntToRead];
            int iReadCount = Port2.Read(ByteRead, 0, iByteCntToRead);
            sRecvedMsg2 += Encoding.ASCII.GetString(ByteRead, 0, iReadCount);
            string sMsg = sRecvedMsg2;

            if(iReadCount == 1 && iLow2 == -999)
            {
                iLow2  = ByteRead[0]      ;
                return ;
            }
            else if(iReadCount == 1)
            {
                iHigh2 = ByteRead[0] << 8 ;
            }
            else if(iReadCount == 2)
            {
                iLow2  = ByteRead[0]      ;
                iHigh2 = ByteRead[1] << 8 ;
            }
            //iLow1  = ByteRead[0]      ;
            //iHigh1 = ByteRead[1] << 8 ;
            //iData = iHigh1 + iLow1     ;
            //Log.Trace("TEST : " + iData1.ToString(),LOG_ID); 


            //if(iReadCount == 2)
            //{
                //int iLow  = ByteRead[0]      ;
                //int iHigh = ByteRead[1] << 8 ;
                int iData = iHigh2 + iLow2     ;
                double dGate = (double)iData/100.0; 

                if(dGate > 0.3)
                {
                    Stat.dGate = dGate;
                }
                else if(dGate < 0)
                {
                    Stat.dGate = 0;
                }
                else
                {
                    Stat.dGate = Correction(dGate,2); 
                }

                if(PreStat.dGate != Stat.dGate) {
                    Log.Trace("Gate Count : " + Stat.dGate.ToString(),LOG_ID); 
                } 
                PreStat.dGate = Stat.dGate ;
                bRcvEnd2 = true;
            //}   
            //bool bRet0 = sMsg.IndexOf("[I") >= 0 && sMsg.IndexOf("#") >= 0; //]IG:nnnnn\r\n 0~65535
            //
            //if(!bRet0) return ;
            //
            //int iStt = sMsg.IndexOf("[I"   ) + 2   ;  
            //int iEnd = sMsg.IndexOf("#") - iStt;
            //string sTemp = sMsg.Substring(iStt,iEnd);
            //
            //string sData = sTemp;
            //Stat.dGate = Correction(CConfig.StrToDoubleDef(sData,0) /100,2); 
            ////Stat[iId].bRcvEnd = true;
            //bRcvEnd2 = true;
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

            Ini.Load(sTitle, "PortNo"  , out iPortNo  ); if(iPortNo  ==0) iPortNo  = 1 ;
            Ini.Save(sTitle, "PortNo"  ,     iPortNo  );
            Ini.Load(sTitle, "PortNo1" , out iPortNo1 ); if(iPortNo1 ==0) iPortNo1 = 1 ;
            Ini.Save(sTitle, "PortNo1" ,     iPortNo1 );
            Ini.Load(sTitle, "PortNo2" , out iPortNo2 ); if(iPortNo2 ==0) iPortNo2 = 1 ;
            Ini.Save(sTitle, "PortNo2" ,     iPortNo2 );


            
        }

        //A 파워서플라이의 상태를 전송.
        public  double  dOnTime      = 0 ;
        private double  dOnTimeTemp  = 0 ;
        public  double  dOffTime     = 0 ;
        private double  dOffTimeTemp = 0 ;
        public bool SendOnTime(double _dOnTime)
        {
            //Delay(10);
            bRcvEnd = false;
            sOut = "SendOnTime" ;
            string sSend = "[PON," + _dOnTime.ToString() + "#" ;
            dOnTimeTemp = _dOnTime ;
            if (OM.MstOptn.bDebugMode) Log.Trace(sSend,LOG_ID);
            SendMsg(sSend);
            return CheckReceiveEnd();
        }
        public bool SendOffTime(double _dOffTime)
        {
            //Delay(10);
            bRcvEnd = false;
            sOut = "SendOffTime" ;
            string sSend = "[POF," + _dOffTime.ToString() + "#" ;
            dOffTimeTemp = _dOffTime ;
            if (OM.MstOptn.bDebugMode) Log.Trace(sSend,LOG_ID);
            SendMsg(sSend);
            return CheckReceiveEnd();
        }

        private string sOut = "";
        public  bool   bOut = false;
        public bool SendOutOnOff(bool _bOn)
        {
            bRcvEnd = false;
            if(_bOn) sOut = "SendOutOn" ;
            else     sOut = "SendOutOff";

            int iOn = Convert.ToInt32(_bOn);
            string sSend = "[OUT," + iOn.ToString() + "#" ;
            if (OM.MstOptn.bDebugMode) Log.Trace(sSend,LOG_ID);
            SendMsg(sSend);
            return CheckReceiveEnd();
        }

        public bool SendArc()
        {
            bRcvEnd = false;
            sOut = "SendArc" ;
            string sSend = "[ARC#" ;
            if (OM.MstOptn.bDebugMode) Log.Trace(sSend,LOG_ID);
            SendMsg(sSend);
            return CheckReceiveEnd();
        }
        public bool SendArcReset()
        {
            bRcvEnd = false;
            sOut = "SendArcReset" ;
            string sSend = "[RST#" ;
            if (OM.MstOptn.bDebugMode) Log.Trace(sSend,LOG_ID);
            SendMsg(sSend);
            return CheckReceiveEnd();
        }
        /// <summary>
        /// Display 모드 변경
        /// </summary>
        /// <param name="_bOn">0이면 캐소드 전류 표시 , 1이면 아킹 카운트값 표시</param>
        /// <returns></returns>
        public bool SendDisplay(bool _bOn)
        {
            bRcvEnd = false;
            sOut = "SendDisplay";
            int iOn = Convert.ToInt32(_bOn);
            string sSend = "[MOD,"+iOn.ToString()+"#" ;
            if (OM.MstOptn.bDebugMode) Log.Trace(sSend,LOG_ID);
            SendMsg(sSend);
            return CheckReceiveEnd();
        }
        public bool SendCathod()
        {
            bRcvEnd = false;
            sOut = "SendCathod";
            //Stat.dCathod = 0;
            string sSend = "[HIC#" ;
            if (OM.MstOptn.bDebugMode) Log.Trace(sSend,LOG_ID);
            SendMsg(sSend);
            return CheckReceiveEnd();
        }
        public bool SendFocus()
        {
            bRcvEnd1 = false;
            //Stat.dFocus = 0;
            //string sSend = "[HIF#" ;
            string sSend = "AD#" ;
            if (OM.MstOptn.bDebugMode) Log.Trace(sSend,LOG_ID);
            SendMsg(sSend,1);
            return CheckReceiveEnd1();
        }
        public bool SendGate()
        {
            bRcvEnd2 = false;
            //Stat.dGate = 0;
            //string sSend = "[HIG#" ;
            string sSend = "AD#" ;
            if (OM.MstOptn.bDebugMode) Log.Trace(sSend,LOG_ID);
            SendMsg(sSend,2);
            return CheckReceiveEnd2();
        }
        public bool SendAll()
        {
            bRcvEnd = false;
            Stat.dCathod = 0;
            Stat.dFocus  = 0;
            Stat.dGate   = 0;
            string sSend = "[CFG#" ;
            if (OM.MstOptn.bDebugMode) Log.Trace(sSend,LOG_ID);
            SendMsg(sSend);
            return CheckReceiveEnd();
        }
        public bool SendLive()
        {
            bRcvEnd = false;
            sOut = "SendLive" ;
            string sSend = "[@#" ;
            if (OM.MstOptn.bDebugMode) Log.Trace(sSend,LOG_ID);
            SendMsg(sSend);
            return CheckReceiveEnd();
            //return false;
        }
        public bool SendReset()
        {
            bRcvEnd = false;
            dOnTime  = 0;
            dOffTime = 0;
            Stat.dCathod = 0 ;
            sOut = "SendReset" ;
            string sSend = "[RST#" ;
            if (OM.MstOptn.bDebugMode) Log.Trace(sSend,LOG_ID);
            SendMsg(sSend);
            //return CheckReceiveEnd();
            return true;
        }
        public bool SendReset1()
        {
            bRcvEnd1 = false;
            Stat.dFocus = 0 ;
            string sSend = "[RST#" ;
            if (OM.MstOptn.bDebugMode) Log.Trace(sSend,LOG_ID);
            SendMsg(sSend,1);
            //return CheckReceiveEnd();
            return true;
        }
        public bool SendReset2()
        {
            bRcvEnd2 = false;
            Stat.dGate = 0 ;
            string sSend = "[RST#" ;
            if (OM.MstOptn.bDebugMode) Log.Trace(sSend,LOG_ID);
            SendMsg(sSend,2);
            //return CheckReceiveEnd();
            return true;
        }
        public bool IsReceiveEnd()
        {
            return bRcvEnd; 
        }                        

        public int i1Pass = 0;
        public int i2Pass = 0;
        public int i3Pass = 0;

        public bool CheckReceiveEnd()
        {
            m_tmOut.Clear();
            while(!bRcvEnd)
            {
                //System.Windows.Forms.Application.DoEvents();
                if (m_tmOut.OnDelay(true, 200))
                {
                    i1Pass++;
                    Log.Trace("Cathod TimeOut Count : " + i1Pass.ToString(),LOG_ID); 
                    SendReset();
                    if(i1Pass > OM.CmnOptn.iRdErr1) ML.ER_SetErr(ei.ETC_Daegyum,"Cathod 응답 없음 Count : " + i1Pass.ToString());
                    return true;
                    
                    
                }
            }
            i1Pass = 0;
            return bRcvEnd;
        }  

        public bool CheckReceiveEnd1()
        {
            m_tmOut.Clear();
            while(!bRcvEnd1)
            {
                //System.Windows.Forms.Application.DoEvents();
                if (m_tmOut.OnDelay(true, 200))
                {
                    i2Pass++;
                    Log.Trace("Focus TimeOut Count : " + i1Pass.ToString(),LOG_ID); 
                    SendReset1();
                    if(i2Pass > OM.CmnOptn.iRdErr2) ML.ER_SetErr(ei.ETC_Daegyum,"Focus 응답 없음 Count : " + i2Pass.ToString());
                    return true;
                }
            }
            i2Pass = 0;
            return bRcvEnd1;
        }  

        public bool CheckReceiveEnd2()
        {
            m_tmOut.Clear();
            while(!bRcvEnd2)
            {
                //System.Windows.Forms.Application.DoEvents();
                if (m_tmOut.OnDelay(true, 200))
                {
                    i3Pass++;
                    Log.Trace("Gate TimeOut Count : " + i1Pass.ToString(),LOG_ID); 
                    SendReset2();
                    if(i3Pass > OM.CmnOptn.iRdErr3) ML.ER_SetErr(ei.ETC_Daegyum,"게이트 응답 없음 Count : " + i3Pass.ToString());
                    return true;
                }
            }
            i3Pass = 0;
            return bRcvEnd2;
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
