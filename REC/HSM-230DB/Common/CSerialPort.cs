using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using COMMON;

namespace COMMON
{
    delegate void ReceivedCallback();
    public class CSerialPort
    {
        private SerialPort Port = null;

        private   int    iPortId     = 0  ;
        private   string sName       = "" ;
        protected string sRecvMsg    = "" ;
        protected string sSendedMsg  = "" ;
        private   string sEndOfText  = "" ; //응답메세지가 끊겨서 2번에 들어오는경우가 있는데 그럴때 세팅 하고 이문자가 들어올때까지 메세지를 받지않는걸로 함.

        //파일로 초기화.
        //_iComNo은 1번부터 입력 되어야 한다.
        public CSerialPort(int _iPortId , string _sName, string _sEndOfText)
        {
           
            iPortId    = _iPortId   ;
            sName      = _sName     ;
            sEndOfText = _sEndOfText;

            Port = new SerialPort();
            Port.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
            Port.Encoding = System.Text.Encoding.GetEncoding("iso-8859-1");//이것이 8비트 문자 모두 가능 Ascii는 7비트라 63이상의 값은 표현 안됌.

            Load(Port);            
        }

        public bool PortOpen()
        {
            try
            {
                Port.Open(); 
            }
            catch
            {
                Log.ShowMessage(sName + " COM PORT ERROR", Port.PortName + " COM Port not Exist");   
            }    
            if (!Port.IsOpen)
            {
                Log.ShowMessage(sName + " COM PORT ERROR", Port.PortName + " COM Port not opened");   
                return false;
            }
            //else
            //{
            //    Log.ShowMessage(sName + " COM PORT SUCCESS", iPortId.ToString() + " COM Port opened");   
            //}
            return true;
            
        }

        public void PortClose()
        {
            Port.Close();
            Port.Dispose();
        }


        public bool SendMsg(string _sMsg)
        {
            if (!Port.IsOpen) return false ;
            sRecvMsg  = "";
            sSendedMsg = _sMsg;

            Port.Write(_sMsg);

            return true ;
        }

        public bool SendMsg(byte [] _baMsg)
        {
            if (!Port.IsOpen) return false ;
            sRecvMsg  = "";
            Encoding enc = Encoding.GetEncoding("iso-8859-1");
            sSendedMsg = enc.GetString(_baMsg);
            //sSendedMsg = Encoding.Default.GetString(_baMsg);
            Port.Write(_baMsg, 0, _baMsg.Length);

            return true ;
        }

        protected virtual void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int intRecSize = Port.BytesToRead;
            string strRecData= "";


            if (intRecSize != 0)
            {
                strRecData = Port.ReadExisting();
                sRecvMsg += strRecData;
                Encoding enc = Encoding.GetEncoding("iso-8859-1");
                byte[] buff = enc.GetBytes(sRecvMsg);
                //int iCrntTemp1 = buff[0] ;

            }

            //if (intRecSize != 0)
            //{
            //    strRecData = "";
            //    byte[] buff = new byte[intRecSize];
            //    Port.Read(buff, 0, intRecSize);
            //    Encoding enc = Encoding.GetEncoding("iso-8859-1");
            //    strRecData = enc.GetString(buff);
            //    sRecvMsg += strRecData;
            //}
        }

        public virtual bool IsReceiveEnd()
        {
            if (sRecvMsg == "" || sRecvMsg.IndexOf(sEndOfText) >= 0) //End of Text문자가 있을경우.
            {
                return true ;
            }
            return false ;
        }                        

        public string ReadMsg()
        {
            return sRecvMsg;
        }

        public void Load(SerialPort _Port , string _sPath = "")
        {
            //Set Dir.
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sFileName = "Util\\SerialPort" ;
            string sSerialPath = sExeFolder + sFileName + ".ini";
            string sTitle = "";
            
            if(sName=="")sTitle = "PortID_" + iPortId.ToString();
            else         sTitle = sName ;

            CIniFile Ini ;
            if(_sPath == "")Ini = new CIniFile(sSerialPath);
            else            Ini = new CIniFile(_sPath     );

            int iPortNo   ;
            int iBaudRate ;
            int iDataBit  ;
            int iParityBit;
            int iStopBit  ;

            Ini.Load(sTitle, "PortNo   ", out iPortNo   ); if(iPortNo    ==0)iPortNo    = 1 ;
            Ini.Load(sTitle, "BaudRate ", out iBaudRate ); if(iBaudRate  ==0)iBaudRate  = 9600 ;
            Ini.Load(sTitle, "DataBit  ", out iDataBit  ); if(iDataBit<5 || iDataBit>8)iDataBit   = 8 ;
            Ini.Load(sTitle, "ParityBit", out iParityBit); 
            Ini.Load(sTitle, "StopBit  ", out iStopBit  ); if(iStopBit<1 || iStopBit>3)iStopBit   = 1 ;

            //맨처음 파일 없을때 대비.
            Ini.Save(sTitle, "PortNo   ", iPortNo   );
            Ini.Save(sTitle, "BaudRate ", iBaudRate );
            Ini.Save(sTitle, "DataBit  ", iDataBit  );
            Ini.Save(sTitle, "ParityBit", iParityBit);
            Ini.Save(sTitle, "StopBit  ", iStopBit  );

            _Port.PortName     = "Com" + iPortNo.ToString();
            _Port.BaudRate     = iBaudRate; //9600 , 
            _Port.DataBits     = iDataBit;  //8
            _Port.Parity       = (Parity)iParityBit;//None = 0,Odd = 1,Even = 2,Mark = 3 ,Space = 4, 
            _Port.StopBits     = (StopBits)iStopBit;//None = 0,One = 1,Two  = 2,OnePointFive = 3, None은 예외상황발생함..
            _Port.ReadTimeout  = (int)500;
            _Port.WriteTimeout = (int)500;

        }
    }
}
