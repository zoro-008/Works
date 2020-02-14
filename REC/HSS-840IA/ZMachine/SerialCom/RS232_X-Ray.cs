using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using COMMON;
using System;

using System.Runtime.InteropServices;

using COMMON;
using System.Globalization;
using System.IO;

using System.Runtime.CompilerServices;
using System.Windows.Forms;


namespace Machine
{
    public class RS232_X_Ray
    {
        //public RS232_X_Ray(int _iPortId , string _sName, string _sEndOfText=""):base(_iPortId , _sName, _sEndOfText)
        //{
        //    /*초기화 상태.
        //    BaudRate=115200
        //    DataBit=8
        //    ParityBit=0
        //    StopBit=1           
        //    */
        //}
        private SerialPort Port = null;

        private int iPortId = 0;
        private string sName = "";

        private string sSetMsg = "";
        private string sRecvedMsg = "";
        private string sSendedMsg = "";
        private bool   bRcvEnd = false;
        private int    iStep          ;
        private int    iPreStep       ;
        private string sErrMsg        ;
        private CDelayTimer Timer = new CDelayTimer();

        public RS232_X_Ray(int _iPortId , string _sName)
        {
            iPortId = _iPortId;
            sName = _sName;

            Port = new SerialPort();
            //Port.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
            //Port.Encoding = System.Text.Encoding.GetEncoding("iso-8859-1");//이것이 8비트 문자 모두 가능 Ascii는 7비트라 63이상의 값은 표현 안됌.

            Load(sName, Port); //"Zebra110Xi4"
            PortOpen();

            Port.BaudRate = 19200; //9600 , 
            Port.DataBits = 8;  //8
            Port.Parity = Parity.None;//None = 0,Odd = 1,Even = 2,Mark = 3 ,Space = 4, 
            Port.StopBits = StopBits.One;//None = 0,One = 1,Two  = 2,OnePointFive = 3, None은 예외상황발생함..
            Port.ReadTimeout = (int)500;
            Port.WriteTimeout = (int)500;
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

        private void Load(string _sName, SerialPort _Port, string _sPath = "")
        {
            //Set Dir.
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sFileName = "Util\\SerialPort";
            string sSerialPath = sExeFolder + sFileName + ".ini";
            string sTitle = "";

            if (sName == "") sTitle = "PortID_" + iPortId.ToString();
            else sTitle = _sName;

            CIniFile Ini;
            if (_sPath == "") Ini = new CIniFile(sSerialPath);
            else Ini = new CIniFile(_sPath);

            int iPortNo;
            Ini.Load(sTitle, "PortNo", out iPortNo); if (iPortNo == 0) iPortNo = 1;

            //맨처음 파일 없을때 대비.
            Ini.Save(sTitle, "PortNo   ", iPortNo);

            _Port.PortName = "Com" + iPortNo.ToString();
        }

        private bool SendMsg(string _sMsg)
        {
            if (!Port.IsOpen) return false;

            bRcvEnd = false;
            sRecvedMsg = "";
            sSendedMsg = _sMsg;

            int iMsgLeghth = _sMsg.Length;
            byte[] ByteMsg = new Byte[iMsgLeghth];
            ByteMsg = Encoding.ASCII.GetBytes(_sMsg);

            byte[] ByteMsg8 = new Byte[8];
            int etc = iMsgLeghth % 8;

            //포트에 데이터를 8개씩 쓰기 위해
            //데이터의 길이가 8의 배수가 아니면 나머지 데이터는 따로 보내줌
            //왜 이렇게 되어 있는지 모르겠지만 일단 
            for (int j = 0; j < iMsgLeghth; j++)
            {
                ByteMsg8[j % 8] = ByteMsg[j];
                //8바이트마다 혹은 마지막에 보낸다.
                if (((j + 1) % 8 == 0) || (iMsgLeghth - 1 == j))
                {
                    Port.Write(ByteMsg8, 0, (j % 8) + 1);
                }
            }
            return true;
        }

        public void Init()
        {
            sErrMsg = "";
            iPreStep = 0;
            iStep = 10;
        }

        public void SetVolt(int _iVolt)
        {
            //string sMsg = "[spm_hv___" + _iVolt + "]";
            //bool bRet = SendMsg(sMsg);
            //bool bRet = false;
            string sTemp = "";
            sTemp = "[spm_hv___";
            sTemp += _iVolt.ToString();
            sTemp += "]";
            //bRet = SendMsg(sTemp);
            //bRet = SendMsg("[spm_hv___"+_iVolt+"]");
            //return bRet;
            SendMsg(sTemp);
        }

        public void SetAmpere(double _dAmpere)
        {
            int iAmpere = (int)(_dAmpere * 10);
            //bool bRet = false;
            string sTemp = "";
            sTemp = "[spm_ha___";
            sTemp += iAmpere.ToString();
            sTemp += "]";
            //bRet = SendMsg(sTemp);
            ////bRet = SendMsg("[spm_ha___" + iAmpere + "]");
            //return bRet;
            SendMsg(sTemp);
        }

        public void SetTime(double _dTime)
        {
            double dTime;
            int iSetTime;
            dTime = _dTime * 1000;
            iSetTime = (int)dTime;
            //bool bRet = false;
            string sTemp = "";
            sTemp = "[spm_xtst_";
            sTemp += iSetTime.ToString();
            sTemp += "]";
            //bRet = SendMsg(sTemp);
            //bRet = SendMsg("[spm_xtst_" + iSetTime + "]");
            //return bRet;
            SendMsg(sTemp);
        }

        //public bool SetVolt(string _sVolt)
        //{
        //    //string sMsg = "[spm_hv___" + _iVolt + "]";
        //    //bool bRet = SendMsg(sMsg);
        //    int iVolt = int.Parse(_sVolt);
        //    bool bRet = false;
        //    bRet = SendMsg("[spm_hv___" + iVolt + "]");
        //    return bRet;
        //}
        //
        //public bool SetAmpere(string _sAmpere)
        //{
        //    double dAmpere = double.Parse(_sAmpere);
        //    int iAmpere = (int)(dAmpere * 10);
        //    bool bRet = false;
        //    bRet = SendMsg("[spm_ha___" + iAmpere + "]");
        //    return bRet;
        //}
        //
        //public bool SetTime(string _sTime)
        //{
        //    double dTime = double.Parse(_sTime);
        //    int iSetTime;
        //    //dTime = _sTime * 1000;
        //    iSetTime = (int)dTime;
        //    bool bRet = false;
        //    bRet = SendMsg("[spm_xtst_" + iSetTime + "]");
        //    return bRet;
        //}

        //public bool Run()
        //{
        //    bool bRet = SendMsg("[run]");
        //    return bRet;
        //}

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
            return;// DateTime.Now;
        }

        public void SetXrayPara(int _iVolt, double _dAmpere, double _dTime)
        {
            if (!ML.IO_GetX(xi.ETC_FtInDoor)) { ML.ER_SetErr(ei.ETC_Door, "차폐함 앞문 열림."       ); return; }
            if (!ML.IO_GetX(xi.ETC_RtInDoor)) { ML.ER_SetErr(ei.ETC_Door, "차폐함 우측문 열림."     ); return; }
            if (!ML.IO_GetX(xi.ETC_RrInDoor)) { ML.ER_SetErr(ei.ETC_Door, "차폐함 뒷문 열림."       ); return; }
            if (!ML.IO_GetX(xi.INDX_DoorCl )) { ML.ER_SetErr(ei.ETC_Door, "차폐함 로더 출입구 열림."); return; }

            //Kvp값 안바껴서 사이사이에 딜레이 넣음 진섭.
            Delay(20);
            SetVolt  (_iVolt  );
            Delay(20);
            SetAmpere(_dAmpere);
            Delay(20);
            SetTime  (_dTime  );
            Delay(20);
        }

        public void SetXrayPara(string _iVolt, string _dAmpere, string _dTime)
        {
            if (!ML.IO_GetX(xi.ETC_FtInDoor)) { ML.ER_SetErr(ei.ETC_Door, "차폐함 앞문 열림."       ); return ; }
            if (!ML.IO_GetX(xi.ETC_RtInDoor)) { ML.ER_SetErr(ei.ETC_Door, "차폐함 우측문 열림."     ); return ; }
            if (!ML.IO_GetX(xi.ETC_RrInDoor)) { ML.ER_SetErr(ei.ETC_Door, "차폐함 뒷문 열림."       ); return ; }
            if (!ML.IO_GetX(xi.INDX_DoorCl )) { ML.ER_SetErr(ei.ETC_Door, "차폐함 로더 출입구 열림."); return ; }

            int    i1 = CConfig.StrToIntDef   (_iVolt  ,0);
            double d2 = CConfig.StrToDoubleDef(_dAmpere,0);
            double d3 = CConfig.StrToDoubleDef(_dTime  ,0);

            //Kvp값 안바껴서 사이사이에 딜레이 넣음 진섭.
            //int    i1 = int   .Parse(_iVolt  );
            //double d2 = double.Parse(_iAmpere);
            //double d3 = double.Parse(_iTime  );
           
            Delay(20);
            SetVolt(i1);
            Delay(20);
            SetAmpere(d2);
            Delay(20);
            SetTime(d3);
            Delay(20);
        }

        public bool SetV(int _iVolt)
        {
            string sTemp = "";
            sTemp = "[spm_hv___";
            sTemp += _iVolt.ToString();
            sTemp += "]";
            bool bRet = SendMsg(sTemp);
            return bRet;
        }

        public bool SetA(double _dAmpere)
        {
            int iAmpere = (int)(_dAmpere * 10);
            string sTemp = "";
            sTemp = "[spm_ha___";
            sTemp += iAmpere.ToString();
            sTemp += "]";
            if (sTemp == "") return false;
            bool bRet = SendMsg(sTemp);
            return bRet;
        }

        public bool SetT(double _dTime)
        {
            double dTime;
            int iSetTime;
            dTime = _dTime * 1000;
            iSetTime = (int)dTime;
            string sTemp = "";
            sTemp = "[spm_xtst_";
            sTemp += iSetTime.ToString();
            sTemp += "]";
            if (sTemp == "") return false;
            bool bRet = SendMsg(sTemp);
            return bRet;
        }

        //public bool IsReceiveEnd()
        //{
        //    //End of Text문자가 있을경우.
        //    if (sRecvedMsg.Contains("\r"))
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        public bool XrayOn(string _iVolt, string _dAmpere, string _dTime)
        {
            //Check Cycle Time Out.
            string sTemp;
            if (Timer.OnDelay(iStep != 0 && iStep == iPreStep, 5000))
            {
                sErrMsg = "Time Out " + string.Format("Cycle iStep={0:00}", iStep);
                //SEQ.TTBL.Trace(sErrMsg);
                iStep = 0;
                return true;
            }

            if (iStep != iPreStep)
            {
                sTemp = string.Format("Cycle iStep={0:00}", iStep);
                //SEQ.TTBL.Trace(sTemp);
            }

            iPreStep = iStep;

            int    i1 = CConfig.StrToIntDef   (_iVolt  ,0);
            double d2 = CConfig.StrToDoubleDef(_dAmpere,0);
            double d3 = CConfig.StrToDoubleDef(_dTime  ,0);

            //Cycle.
            switch (iStep)
            {
                default:
                    if (iStep != 0)
                    {
                        sErrMsg = string.Format("Cycle Default Clear from iStep={0:00}", iStep);
                        iStep = 0;
                    }
                    return true;

                case 10:
                    if (!SetV(i1)) return false;
                    
                    iStep++;
                    return false;

                case 11:
                    //if (!IsReceiveEnd()) return false;
                    if (!SetA(d2)) return false;
                    iStep++;
                    return false;


                case 12:
                    //if (!IsReceiveEnd()) return false;
                    if (!SetT(d3)) return false;
                    iStep++;
                    return false;

                case 13:
                    //if (!IsReceiveEnd()) return false;

                    iStep = 0;
                    sErrMsg = "";
                    return true;
            }
        }

    }
}
