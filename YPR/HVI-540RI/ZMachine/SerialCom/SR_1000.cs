using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using COMMON;
using System.Runtime.CompilerServices;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;

namespace Machine
{
    public class SR_1000 : ML
    {
        /*
        IP 어드레스    ： 192.168.100.100
        서브넷 마스크  ： 255.255.255.0 (24 bit)
        기본 게이트웨이： 0.0.0.0
        */

        private   const char   cSTX  = (char)0x02 ;
        private   const char   cETX  = (char)0x03 ;
        private   const char   cCR   = (char)0x0D ;
        private   const char   cLF   = (char)0x0A ;

        private const int READER_COUNT  = 1    ; // number of readers to connect
        private const int RECV_DATA_MAX = 10240;
        private const int COMMAND_PORT  = 9004 ; // Command port
        private const int DATA_PORT     = 9004 ; // Data port

        private ClientSocket clientSocketInstance;

        private int    iStep       ;
        private int    iPreStep    ;
        private string sErrMsg     ;
        private string sSendedMsg  ;
        private string sRecvedMsg  ;
        private string sAppendGrade;//바코드 품질 A_D
        private bool   bTriggerSsr;
        //private string sEndOfText;
        public string  ErrMsg {get { return sErrMsg; } set { sErrMsg = value; } }

        private CDelayTimer Timer = new CDelayTimer();

        xi      TriggerSsr;
        int     m_iPartId ;
        byte[]  ip1       = { 192, 168, 100, 100 }; //밖으로 빼기 귗낳아서 냅둠.
        public SR_1000(/*xi _TriggerSsr*/)
        {
            //센서 등록 자재 감지 센서 터치후 바코드 조사용
            //TriggerSsr = _TriggerSsr;

            //로그 위치
            m_iPartId = (int)pi.MAX_PART + ti.Max ; //마지막에

            // First reader to connect.
            //ip1 = { 192, 168, 100, 100 };
            //clientSocketInstance = new ClientSocket(ip1, COMMAND_PORT, DATA_PORT);  // 9003 for command, 9004 for data
            if(!ConnectToServer())
            {
                Log.ShowMessage("Barcode", "Barcode Not Connect");
                SEQ.TTBL.Trace("ConnectToServer Fail");
            }

            Init();
            //sEndOfText = "[CR]";
        }

        public bool GetReady()
        {
            if (clientSocketInstance.commandSocket == null) return false;
            return true;
        }

        //public bool GetTriggerSsr(bool _bDown = false)
        //{
        //    if(!bTriggerSsr)
        //    {
        //        if(_bDown) bTriggerSsr = ML.IO_GetXDn(TriggerSsr);
        //        else       bTriggerSsr = ML.IO_GetXUp(TriggerSsr);
        //    }
        //    return bTriggerSsr;
        //}

        //셋팅한 바코드 가져오기
        //S1~?YYMMS1~? (텍스트 날짜 텍스트)
        public string GetInputBarcode()
        {
            string sBarcode = "";
            sBarcode  = OM.DevInfo.sBarcodeReadData1.Trim();
            if(OM.DevInfo.bUseYearMonth1          ) sBarcode += DateTime.Now.ToString("yyMM");
            if (OM.DevInfo.sBarcodeReadData2 != "") sBarcode += OM.DevInfo.sBarcodeReadData2.Trim();
            if(OM.DevInfo.bUseYearMonth2          ) sBarcode += DateTime.Now.ToString("yyMM");

            return sBarcode ;

        }

        public bool ConnectToServer()
        {
            clientSocketInstance = new ClientSocket(ip1, COMMAND_PORT, DATA_PORT);  // 9003 for command, 9004 for data

            //if (clientSocketInstance.dataSocket == null) return false;
            // Connect to the command port.
            try
            {
                clientSocketInstance.readerCommandEndPoint.Port = COMMAND_PORT;
                clientSocketInstance.readerDataEndPoint.Port = DATA_PORT;
                // Close the socket if opened.
                if (clientSocketInstance.commandSocket != null)
                {
                    clientSocketInstance.commandSocket.Close();
                }

                // Create a new socket.
                clientSocketInstance.commandSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                clientSocketInstance.commandSocket.Connect(clientSocketInstance.readerCommandEndPoint);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                // Catch exceptions and show the message.
                sErrMsg = clientSocketInstance.readerCommandEndPoint.ToString() + " Failed to connect.";
                clientSocketInstance.commandSocket = null;
                return false;
            }
            catch (SocketException ex)
            {
                // Catch exceptions and show the message.
                sErrMsg = clientSocketInstance.readerCommandEndPoint.ToString() + " Failed to connect.";
                clientSocketInstance.commandSocket = null;
                return false;
            }

            // Connect to the data port.
            try
            {
                // Close the socket if opend.
                if (clientSocketInstance.dataSocket != null)
                {
                    clientSocketInstance.dataSocket.Close();
                }

                // If the same port number is used for command port and data port, unify the sockets and skip a new connection. 
                if (clientSocketInstance.readerCommandEndPoint.Port == clientSocketInstance.readerDataEndPoint.Port)
                {
                    clientSocketInstance.dataSocket = clientSocketInstance.commandSocket;
                }
                else
                {
                    // Create a new socket.
                    clientSocketInstance.dataSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    clientSocketInstance.dataSocket.Connect(clientSocketInstance.readerDataEndPoint);
                }

                // Set 100 milliseconds to receive timeout.
                clientSocketInstance.dataSocket.ReceiveTimeout = 100;
            }
            catch (SocketException ex)
            {
                // Catch exceptions and show the message.
                sErrMsg = clientSocketInstance.readerDataEndPoint.ToString() + " Failed to connect.";
                clientSocketInstance.dataSocket = null;
                return false;
            }

            return true;
            
        }

        public void Init()
        {
            sErrMsg      = ""   ;
            iPreStep     = 0    ;
            iStep        = 10   ;
            bTriggerSsr  = false;
            sRecvedMsg   = "";
            sAppendGrade = "";
        }

        public void Reset()
        {
            Init();
        }

        public void Close()
        {
            if (clientSocketInstance.commandSocket != null)
            {
                clientSocketInstance.commandSocket.Close();
                clientSocketInstance.commandSocket = null;
            }
            if (clientSocketInstance.dataSocket != null)
            {
                clientSocketInstance.dataSocket.Close();
                clientSocketInstance.dataSocket = null;
            }
            
        }

        private bool SendMsg(string _sMsg)
        //public bool SendMsg(byte[] _baMsg)
        {
            if (clientSocketInstance.commandSocket == null) return false;

            string sSendMsg = "";

            sSendMsg = _sMsg;
            sSendMsg += "\r";

            Byte[] command = ASCIIEncoding.ASCII.GetBytes(sSendMsg);

            sRecvedMsg = "";
         
            clientSocketInstance.commandSocket.Send(command);

            return true;
        }

        private bool DataReceived()
        {
            sRecvedMsg = "";
            sAppendGrade = "";
            Byte[] recvBytes = new Byte[RECV_DATA_MAX];
            int recvSize = 0;

            if (clientSocketInstance.dataSocket != null)
            {
                try
                {
                    recvSize = clientSocketInstance.dataSocket.Receive(recvBytes);
                }
                catch (SocketException)
                {
                    // Catch the exception, if cannot receive any data.
                    sErrMsg = "Catch the exception, if cannot receive any data.";
                    recvSize = 0;
                }
            }
            else
            {
                sErrMsg = clientSocketInstance.readerDataEndPoint.ToString() + " is disconnected.";
                return false;
            }

            if (recvSize == 0)
            {
                sErrMsg = clientSocketInstance.readerDataEndPoint.ToString() + " has no data.";
                return false;
            }
            else
            {
                // Show the receive data after converting the receive data to Shift-JIS.
                // Terminating null to handle as string.
                recvBytes[recvSize] = 0;
                string sTemp = Encoding.GetEncoding("Shift_JIS").GetString(recvBytes);
                if (sTemp.Contains(":")) //바코드 정상적으로 들어오면
                {
                    sRecvedMsg   = sTemp.Substring(0, sTemp.IndexOf(":"));
                    sAppendGrade = sTemp.Substring(sTemp.IndexOf(":") + 1, 1);
                }
                else //Received Msg를 ERROR로 날렸을때
                {
                    sRecvedMsg = sTemp.Substring(0, sTemp.IndexOf("\r"));
                    sAppendGrade = "";
                }
                
                //받은 데이터 로그 남기기
                SEQ.TTBL.Trace(sTemp       );
                SEQ.TTBL.Trace(sRecvedMsg  );
                SEQ.TTBL.Trace(sAppendGrade);

                //업체 확인용 로그 남기기 마지막 탭에 남김.
                Trace(sAppendGrade);
                //if (sRecvedMsg.Contains(GetInputBarcode()))
            
                
            }
            
            return true;
        }

        public bool BarcodeOn()
        {
            bool bRet = SendMsg("LON");
            return bRet;
        }

        public bool IsReceiveEnd()
        {
            //End of Text문자가 있을경우.
            if (sRecvedMsg.Contains("\r"))  
            {
                return true;
            }
            return false;
        }

        public bool TestDataReceived()
        {
            return DataReceived();
        }

        public string GetBarcode()
        {
            return sRecvedMsg;
        }

        public string GetGrade()
        {
            return sAppendGrade;
        }

        public bool GetGradeError()
        {
            bool bError = true;
            if ((sAppendGrade == "A" || sAppendGrade == "B") && sAppendGrade != "") bError = false;

            return bError;
        }

        /*
        public void CycleRead(bool _bInit)
        {
            if (_bInit) Init();
        }
        
        public bool CycleRead()
        {
            //Check Cycle Time Out.
            string sTemp;
            if (Timer.OnDelay(iStep != 0 && iStep == iPreStep, 5000))
            {
                sErrMsg = "Time Out " + string.Format("Cycle iStep={0:00}", iStep);
                Trace(sErrMsg);
                iStep = 0;
                return true;
            }

            if (iStep != iPreStep)
            {
                sTemp = string.Format("Cycle iStep={0:00}", iStep);
                Trace(sTemp);
            }

            iPreStep = iStep;

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
                    DataReceived();
                    iStep++;
                    return false;

                case 11:
                    if (!IsReceiveEnd()) return false;

                    //if (!sRecvedMsg.Contains("OK"))
                    //{
                    //    sErrMsg = "RcvMsg not exist OK - " + sRecvedMsg;
                    //    return true;
                    //}

                    iStep = 0;
                    return true;
            }
        }
        */
        public bool CycleReading(bool _bInit = true)
        {
            if (_bInit) {
                Init();
                SendMsg("LON");
            }
            else {
                SendMsg("LOFF");
            }

            return true;
        }

        public bool Read()
        {
            if (!DataReceived()) return false;
            return true;
        }

        public bool CycleReading()
        {
            //Check Cycle Time Out.
            string sTemp;
            if (Timer.OnDelay(iStep != 0 && iStep == iPreStep, 5000))
            {
                CycleReading(false);
                sErrMsg = "Time Out " + string.Format("Cycle iStep={0:00}", iStep);
                SEQ.TTBL.Trace(sErrMsg);
                iStep = 0;
                return true;
            }

            if (iStep != iPreStep)
            {
                sTemp = string.Format("Cycle iStep={0:00}", iStep);
                SEQ.TTBL.Trace(sTemp);
            }

            iPreStep = iStep;

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
                    DataReceived();
                    iStep++;
                    return false;

                case 11:
                    if (!IsReceiveEnd()) return false;
                    //CycleReading(false);
                    iStep = 0;
                    sErrMsg = "";
                    return true;
            }
        }

        public void Trace(string _sMsg, [CallerLineNumber] int sourceLineNumber = 0, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "")
        {
            string sHdr = "SR_1000";
            string sMsg = _sMsg.Replace(",", "");
            string sTag = string.Format("{0:00}", m_iPartId);
            string sFullMsg = string.Format("{0}, {1} ,{2},{3},{4}", sTag, sHdr + " " + sMsg, sourceLineNumber, memberName, sourceFilePath);
            Log.SendMessage(sFullMsg);
        }    

    }

    /// <summary>
    /// Socket class for a reader.
    /// </summary>
    class ClientSocket
    {
        public Socket commandSocket;   // socket for command
        public Socket dataSocket;      // socket for data
        public IPEndPoint readerCommandEndPoint;
        public IPEndPoint readerDataEndPoint;

        public ClientSocket(byte[] ipAddress, int readerCommandPort, int readerDataPort)
        {
            IPAddress readerIpAddress = new IPAddress(ipAddress);
            readerCommandEndPoint = new IPEndPoint(readerIpAddress, readerCommandPort);
            readerDataEndPoint = new IPEndPoint(readerIpAddress, readerDataPort);
            commandSocket = null;
            dataSocket = null;
        }
    }
}
