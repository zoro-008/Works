using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using COMMON;

namespace Machine
{
    public class serialPort
    {
        public SerialPort siPort = null;

        public int iPortNo = 0;

        public string sPortName = "";
        public int iBaudRate = 9600;
        public int iDataBit = 8;
        public int iParityBit = 0;
        public int iStopBit = 1;

        public string sRecvMsg = "";


        public void Init(int _iComNo)
        {
            //Load();

            siPort = new SerialPort();      
            iPortNo = _iComNo;
            sPortName = string.Format("Com{0:0}", _iComNo);
            siPort.DataReceived += new SerialDataReceivedEventHandler(DataReceived);

            siPort.PortName = sPortName;
            siPort.BaudRate = iBaudRate;
            siPort.DataBits = iDataBit;
            siPort.Parity = (Parity)iParityBit;
            siPort.StopBits = (StopBits)iStopBit;
            siPort.ReadTimeout = (int)500;
            siPort.WriteTimeout = (int)500;
            
        }

        public bool PortOpen()
        {
                siPort.Open(); 
            if (!siPort.IsOpen)
            {
                Log.ShowMessage("COM PORT ERROR", "COM Port not opened");   
                return false;
            }
            return true;
            
        }

        public void PortClose()
        {
            siPort.Close();
            siPort.Dispose();
        }

        public void PrintSendMsg()
        {
            string sMsg = "";

            //S001 = S   -> 프린트 할 내용. 
            //       001 -> 디바이스 이름.
            sMsg = "S001" + OM.DevInfo.sMrkData;
            SEQ.Com[1].SendMsg(sMsg);

            string sRepCnt = "";
            string sRepDelay = "";

            int iRepCnt = 0;
            int iRepDelay = 0;

            iRepCnt = OM.DevInfo.iVsnInspRowCnt - 1;
            sRepCnt = iRepCnt.ToString("D2");

            iRepDelay = OM.DevInfo.iMrkReptDelay;
            sRepDelay = iRepDelay.ToString("D4");


            //G1 = G         -> Repeat관련 메시지 전송.
            //     1         -> Head No.
            //     sRepCnt   -> Repeat Count.
            //     sRepDelay -> Repeat Delay.
            sMsg = "G1" + sRepCnt + sRepDelay;

            SEQ.Com[1].SendMsg(sMsg);

            //P1001 = P  -> 전송 메시지 입력.
            //        1  -> Head No.
            //        001-> 디바이스 이름.
            sMsg = "P1001";
            SEQ.Com[1].SendMsg(sMsg);

        }

        public void SendMsg(string _sMsg)
        {
            string sMsg = string.Empty;
            byte si_ESC = 0x1B;
            byte si_EOT = 0x04;
            char[] si_Data = _sMsg.ToCharArray();
            //char[] si_Msg  = Encoding.UTF8.GetBytes(_sMsg);

            char[] si_Msg = new char[_sMsg.Length + 2];

            si_Msg[0] = (char)si_ESC;
            for (int i = 1; i < _sMsg.Length + 1; i++ )
            {
                si_Msg[i] = si_Data[i - 1];
            }
            si_Msg[_sMsg.Length + 1] = (char)si_EOT;
            
            siPort.Write(si_Msg, 0, si_Msg.Length);
        }

        void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            int intRecSize = siPort.BytesToRead;
            string strRecData;

            if (intRecSize != 0)
            {
                strRecData = "";
                byte[] buff = new byte[intRecSize];

                siPort.Read(buff, 0, intRecSize);
                for (int iTemp = 0; iTemp < intRecSize; iTemp++)
                {

                    strRecData += Convert.ToChar(buff[iTemp]);
                }
                sRecvMsg += strRecData;
            }
        }

        public string ReadMsg()
        {
            return sRecvMsg;
        }
    }

    public class siCom : serialPort
    {
        public serialPort siPort;
        
        public siCom(int _iComNo)
        {
            Init(_iComNo);
            iPortNo = _iComNo;
        }
        public void Load()
        {
            //Set Dir.
            //ERR_FOLDER    ;
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sFileName = string.Format("ComPort_{0:0}", iPortNo);

            
            string sSerialPath = sExeFolder + sFileName + ".ini";

            string sTitle = string.Format("ComPort_{0:0}", iPortNo);

            CIniFile IniSerialInfo = new CIniFile(sSerialPath);

            IniSerialInfo.Load(sTitle, "PortNo   ", out iPortNo);
            IniSerialInfo.Load(sTitle, "PortName ", out sPortName);
            IniSerialInfo.Load(sTitle, "BaudRate ", out iBaudRate);
            IniSerialInfo.Load(sTitle, "DataBit  ", out iDataBit);
            IniSerialInfo.Load(sTitle, "ParityBit", out iParityBit);
            IniSerialInfo.Load(sTitle, "StopBit  ", out iStopBit);

            //IniSerialInfo.Save(sTitle, "PortNo   ", iPortNo);
            //IniSerialInfo.Save(sTitle, "PortName ", sPortName);
            //IniSerialInfo.Save(sTitle, "BaudRate ", iBaudRate);
            //IniSerialInfo.Save(sTitle, "DataBit  ", iDataBit);
            //IniSerialInfo.Save(sTitle, "ParityBit", iParityBit);
            //IniSerialInfo.Save(sTitle, "StopBit  ", iStopBit);

        }

        //public void Load(int _iCom)
        //{
        //    //Set Dir.
        //    //ERR_FOLDER    ;
        //    string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
        //    string sFileName  = string.Format("\\ComPort_%d.ini", _iCom);
        //    string sSerialPath = sExeFolder + sFileName;

        //    string sTitle = string.Format("ComPort_%d", _iCom);

        //    CIniFile IniLastErrInfo = new CIniFile(sSerialPath);

        //    IniLastErrInfo.Load(sTitle, "PortNo   ", out iPortNo   );
        //    IniLastErrInfo.Load(sTitle, "PortName ", out sPortName );
        //    IniLastErrInfo.Load(sTitle, "BaudRate ", out iBaudRate );
        //    IniLastErrInfo.Load(sTitle, "DataBit  ", out iDataBit  );
        //    IniLastErrInfo.Load(sTitle, "ParityBit", out iParityBit);
        //    IniLastErrInfo.Load(sTitle, "StopBit  ", out iStopBit  );
        //}
    }
}
