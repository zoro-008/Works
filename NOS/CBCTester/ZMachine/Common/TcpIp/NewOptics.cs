using COMMON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Machine
{
    //FCM 검사기가 서버
    //제어프로그램은 클라이언트
    //데이터 전송주기는 200msec, 주기적으로 상태정보 혹은 데이터를 송수신합니다.//메뉴얼 내용

        /*
         시퀜스 순서.
         1. 서버의 상태확인.
         2. 시험시작 전송(바코드포함)
         3. FCM1시험
         4. 서버 상태체크 (레디가 될때까지)
         5. FCM2시험
         6. 서버 상태체크 (레디가 될때까지)
         7. FCM3시험
         8. 헤모글로빈 전송(시험시작~종료 중에 아무때나 전송가능)
         9. 시험종료 전송.
         
         
         
         */
    public class TCPIP_NewOpticsFCM
    {
        #region private
        bool   bConnected = false;
        bool   bCompleted = false;
        bool   bChanged   = false;
        EState  iState     = EState.None;
        #endregion

        //서버에 연결되어 있는지 확인
        public bool Connected { get => mTcpClient == null? false : mTcpClient.Connected && bConnected; }
        //잘 보내졋는지 확인 SendMsg시에 false되서 보내고 나서 확인가능
        public bool Completed { get => bCompleted; }// set => bCompleted = value; } 
        //메세지 센딩에 대한 응답을 받았는지.
        public bool RcvdMsg   { get => bRcvdMsg;   }// set => bChanged   = value; } 
        //상태 체크
        public EState State { get => iState; set => iState = value; }

        #region enum
        public enum EFuncCode : uint
        {
            None       = 0x00 ,
            ReqStatus  = 0x01 ,
            Test       = 0x02 ,
            SubTest    = 0x03 ,
            Hemoglobin = 0x04 ,
            Ping       = 0x05 ,

        }

        public enum EState : uint
        {
            None    = 0x00 ,
            Ready   = 0x01 ,
            Running = 0x02 ,
            Error   = 0x03 
        }

        public enum ETestSeq :uint
        {
            None  = 0x00 ,
            Start = 0x01 ,
            End   = 0x02 ,
            Reset = 0x03 
        }

        public enum ETest : uint
        {
            FCM_1_TEST    =  0x01,
            FCM_2_TEST    =  0x02,
            FCM_3_TEST    =  0x03,
            FCM_4_TEST    =  0x04,
            DC_TEST       =  0x11,
            QC_FCM_1_TEST =  0x21,
            QC_FCM_2_TEST =  0x22,
            QC_FCM_3_TEST =  0x23,
            QC_FCM_4_TEST =  0x24
        }
        #endregion

        TcpClient mTcpClient;
        
        bool bRcvdMsg = false ;
        byte[] bReceivedText;
        byte[] bSendedText;

        private const char cReady   = (char)0x01 ; //Status //Cliend -> Server
        private const char cRunning = (char)0x02 ; //Status //Cliend -> Server
        private const char cError   = (char)0x03 ; //Status //Cliend -> Server

        private const char cStart   = (char)0x02 ; 
        private const char cFuction = (char)0x01 ; 
        private const char cDataH   = (char)0x00 ; 
        private const char cDataL   = (char)0x04 ; 

        private const char cCRC16_1 = (char)0x9F ; 
        private const char cCRC16_2 = (char)0x3C ; 
        private const char cEnd     = (char)0x03 ; 

        public TCPIP_NewOpticsFCM() 
        { 

        }

        public TCPIP_NewOpticsFCM(string _sIp, int _iPort) { Connect(_sIp,_iPort); }

        /// <summary>
        /// 서버 연결하기
        /// </summary>
        /// <param name="_sIp"></param>
        /// <param name="_iPort"></param>
        public bool Connect(string _sIp, int _iPort)
        {
            if(mTcpClient != null && Connected /*mTcpClient.Connected*/) return false; //연결이 되어 있는데 또 연결 방지용, 서버쪽에 연결된 노드가 늘어남
            IPAddress ipa;
            int nPort = _iPort;

            try
            {
                if (!IPAddress.TryParse(_sIp, out ipa))
                {
                    Log.ShowMessage("TcpIp Error","Please supply an IP Address.");
                    return false;
                }

                mTcpClient = new TcpClient();
                IAsyncResult Result = mTcpClient.BeginConnect(ipa, _iPort,onCompleteConnect,mTcpClient);
                
            }
            catch (Exception e)
            {
                Log.ShowMessage("Connect Error",e.Message);
                bConnected = false;
                return false;
            }
            return true;
        }

        void onCompleteConnect(IAsyncResult iar)
        {
            TcpClient tcpc;

            try
            {
                tcpc = (TcpClient)iar.AsyncState;
                tcpc.EndConnect(iar);
                bReceivedText = new byte[512];
                tcpc.GetStream().BeginRead(bReceivedText, 0, bReceivedText.Length, onCompleteReadFromServerStream, tcpc);
                bConnected = true; //연결 성공
                Log.TraceListView("Connected");
            }
            catch (Exception e)
            {
                if(bConnected) Log.ShowMessage("TCP/IP" ,"FCM-" + e.Message);

                bConnected = false; //연결 실패
            }
        }

        /// <summary>
        /// 서버로 부터 데이터 받아오기
        /// 사용전에 State = st.None으로 변경후에 받은걸로 확인
        /// </summary>
        /// <param name="iar"></param>
        void onCompleteReadFromServerStream(IAsyncResult iar)
        {
            TcpClient tcpc;
            int nCountBytesReceivedFromServer;
            //string strReceived;

            try
            {
                tcpc = (TcpClient)iar.AsyncState;
                nCountBytesReceivedFromServer = tcpc.GetStream().EndRead(iar);

                if (nCountBytesReceivedFromServer == 0)
                {
                    Log.TraceListView("<FCM>Err  : Reding Failed!");
                    return;
                }
                
                string sSendMsg = "<FCM>Recv : ";
                for(int i = 0 ; i < nCountBytesReceivedFromServer; i++)
                {
                    sSendMsg += string.Format("{0:X2} " , bReceivedText[i]);
                }
                if(bReceivedText.Length > 2 && bReceivedText[1] != (byte)EFuncCode.Ping) Log.TraceListView(sSendMsg);

                //받은 데이터 체크
                if (nCountBytesReceivedFromServer < 8 && bReceivedText.Length > 2 && bReceivedText[1] != (byte)EFuncCode.Ping)
                {
                    Log.TraceListView("<FCM>Err  : FCM Tester로 부터 수신된 메세지의 길이가 8 미만입니다.");
                    return;
                }

                //포인터 복사 및 mRx재할당.
                byte [] bMsg = bReceivedText ;//new byte[nCountBytesReceivedFromServer];
                bReceivedText = new byte[512];

                //다시 수신대기 상태 진입.
                tcpc.GetStream().BeginRead(bReceivedText, 0, bReceivedText.Length, onCompleteReadFromServerStream, tcpc);

                bRcvdMsg = true ;

                

                if (bMsg[1] == (byte)EFuncCode.ReqStatus) //스테이터스 체크만 응답이 송신한 메세지랑 다르다.
                {
                    ushort usCRC16 = CRC16(bMsg, 5); //nCountBytesReceivedFromServer - 3
                    byte bCRC16Lo = GetByteFromUshort(usCRC16, false);
                    byte bCRC16Hi = GetByteFromUshort(usCRC16, true);

                    if (bCRC16Lo == bMsg[5] && bCRC16Hi == bMsg[6])
                    {
                        if (bMsg[4] == (byte)cReady) iState = EState.Ready;
                        else if (bMsg[4] == (byte)cRunning) iState = EState.Running;
                        else iState = EState.Error;

                    }
                    else
                    {
                        Log.TraceListView("<FCM>Err  : FCM Tester로 부터 수신된 응답의 CRC16코드가 맞지않습니다.");
                        return;
                    }
                }
                else //나머지는 송신한대로 다시 수신됨. 
                {
                    string sSendedMsg   = Encoding.Default.GetString(bSendedText );
                    string sReceivedMsg = Encoding.Default.GetString(bMsg ,0,bSendedText.Length         );
                    if (sSendedMsg != sReceivedMsg)
                    {
                        Log.TraceListView("<FCM>Err  : FCM Tester로 부터 수신된 응답이 송신한 메세지와 다릅니다.");
                        return;
                    }
                }

                bConnected = true;

            }
            catch (Exception e)
            {
                Log.ShowMessage("TcpIp Error",e.Message);
                bConnected = false;
            }
        }

        byte GetByteFromUshort(ushort _usSrc  , bool _bHigh)
        {
            if(_bHigh)
            {
                return (byte)((_usSrc & 0xFF00)>>8) ;
            }
            else
            {
                return  (byte)(_usSrc & 0x00FF) ;
            }
        }

        private void SendMsg(string _sMsg,byte[] _tx = null)
        {
            bCompleted = false;
            bRcvdMsg = false ;

            string sMsg = _sMsg;
            
            
            if (string.IsNullOrEmpty(sMsg) && _tx == null) return;
            if (!Connected)
            {
                return;
            }
            

            if(_tx == null) bSendedText = Encoding.ASCII.GetBytes(sMsg);
            else            bSendedText = _tx;

            string sSendMsg = "<FCM>Send : ";
            for(int i = 0 ; i < bSendedText.Length ; i++)
            {
                sSendMsg += string.Format("{0:X2} " , bSendedText[i]);
            }

            //핑은 보내지 않는다.
            if(bSendedText.Length >= 2 && bSendedText[1] != (byte)EFuncCode.Ping) Log.TraceListView(sSendMsg);
            //if(bReceivedText.Length > 2 && bReceivedText[1] != (byte)EFuncCode.Ping) Log.TraceListView(sSendMsg);
            try
            {
                if (mTcpClient != null)
                {
                    if (mTcpClient.Client.Connected)
                    {
                        mTcpClient.GetStream().BeginWrite(bSendedText, 0, bSendedText.Length, onCompleteWriteToServer, mTcpClient);
                    }
                }
            }
            catch (Exception e) 
            { 
                Log.ShowMessage("TcpIp Error",e.Message); 
                bConnected = false;
            };
        }

        /// <summary>
        /// FCM서버와의 커넥션 확인.
        /// </summary>
        public void SendPing()
        {
            List<byte> lsMsg = new List<byte>();

            //02 01 00 03 10 5D 03
            //응답은 현재 상태 레디시 02 01 00 03 10 5D 03

            lsMsg.Add(0x02); //Start
            lsMsg.Add((byte)EFuncCode.Ping); //상태확인 코드 인덱스.
            lsMsg.Add(0x00); //Length - High
            lsMsg.Add(0x03); //(iDec) ;//Length - Low
            ushort usSum = CRC16(lsMsg.ToArray(), lsMsg.Count);
            lsMsg.Add(GetByteFromUshort(usSum, false)); //checksum 
            lsMsg.Add(GetByteFromUshort(usSum, true)); //checksum 
            lsMsg.Add(0x03); //ETX 

            //문제소지가 있을까봐 크기 맞춰준다.
            byte[] bSendMsg = lsMsg.ToArray();


            SendMsg("", bSendMsg);
        }

        /// <summary>
        /// FCM서버로 상태체크.
        /// </summary>
        public void SendReqStatus()
        {
            List<byte> lsMsg = new List<byte>();

            //02 01 00 03 10 5D 03
            //응답은 현재 상태 레디시 02 01 00 03 10 5D 03

            lsMsg.Add( 0x02 ); //Start
            lsMsg.Add( (byte)EFuncCode.ReqStatus ); //상태확인 코드 인덱스.
            lsMsg.Add( 0x00 ); //Length - High
            lsMsg.Add( 0x03 ); //(iDec) ;//Length - Low
            ushort usSum =  CRC16( lsMsg.ToArray(),lsMsg.Count) ;
            lsMsg.Add( GetByteFromUshort(usSum , false)); //checksum 
            lsMsg.Add( GetByteFromUshort(usSum , true )); //checksum 
            lsMsg.Add( 0x03 ); //ETX 

            //문제소지가 있을까봐 크기 맞춰준다.
            byte [] bSendMsg = lsMsg.ToArray();


            SendMsg("",bSendMsg);
        }

        /// <summary>
        /// FCM서버로 시험의 시퀜스 즉 시작(바코드포함),종료,리셋
        /// </summary>
        /// <param name="_eSeq"></param>
        /// <param name="_sBarcode"></param>
        public void SendTestSeq(ETestSeq _eSeq,string _sBarcode)
        {
            List<byte> lsMsg = new List<byte>();

            //
            //바코드 1234567890 보낼때. 02 02 00 0E 01 31 32 33 34 35 36 37 38 39 30 59 62 03
            //시험 종료.      02 02 00 04 02 DF 79 03
            //응답은 송신내용 그대로 온다.



            lsMsg.Add( 0x02 ); //Start
            lsMsg.Add( (byte)EFuncCode.Test ); //테스트 메세지 인덱스.

            ushort usLength = 0 ;
                 if(_eSeq == ETestSeq.Start) usLength = (ushort)(1/*TestStart/End/Reset*/ + _sBarcode.Length + 2/*CRC16*/ + 1/*ETX*/ );
            else if(_eSeq == ETestSeq.End  ) usLength = (ushort)(1/*TestStart/End/Reset*/ + 2/*CRC16*/ + 1/*ETX*/ );
            else if(_eSeq == ETestSeq.Reset) usLength = (ushort)(1/*TestStart/End/Reset*/ + 2/*CRC16*/ + 1/*ETX*/ );            

            lsMsg.Add( GetByteFromUshort(usLength , true ) ); //Length - High
            lsMsg.Add( GetByteFromUshort(usLength , false) ); //(iDec) ;//Length - Low
            lsMsg.Add( (byte)_eSeq);
            //바코드 인코딩.
            byte[] bBarcode  = Encoding.ASCII.GetBytes(_sBarcode);
            if(_eSeq == ETestSeq.Start)lsMsg.AddRange( bBarcode );

            ushort usSum =  CRC16( lsMsg.ToArray(),lsMsg.Count) ;
            lsMsg.Add( GetByteFromUshort(usSum , false)); //checksum 
            lsMsg.Add( GetByteFromUshort(usSum , true )); //checksum f
            lsMsg.Add( 0x03 ); //ETX 

            //문제소지가 있을까봐 크기 맞춰준다.
            byte [] bSendMsg = lsMsg.ToArray();


            SendMsg("",bSendMsg);
        }

        /// <summary>
        /// 테스트 종류를 보낸다.
        /// </summary>
        /// <param name="_eTest"></param>
        public void SendTestSub(ETest _eTest)
        {
            List<byte> lsMsg = new List<byte>();

            //먼저SendTestSeq 시험시작을 보내야 함.
            // FCM3 시험시작  02 03 00 04 03 1F 45 03
            // 응답은 송신 내용 그대로 수신.


            lsMsg.Add( 0x02 ); //Start
            lsMsg.Add( (byte)EFuncCode.SubTest ); //테스트 메세지 인덱스.
            ushort usLength = 4 ;           
            lsMsg.Add( GetByteFromUshort(usLength , true ) ); //Length - High
            lsMsg.Add( GetByteFromUshort(usLength , false) ); //(iDec) ;//Length - Low

            lsMsg.Add( (byte)_eTest ); //(iDec) ;//Length - Low

            ushort usSum =  CRC16( lsMsg.ToArray(),lsMsg.Count) ;
            lsMsg.Add( GetByteFromUshort(usSum , false)); //checksum 
            lsMsg.Add( GetByteFromUshort(usSum , true )); //checksum f
            lsMsg.Add( 0x03 ); //ETX 

            //문제소지가 있을까봐 크기 맞춰준다.
            byte [] bSendMsg = lsMsg.ToArray();


            SendMsg("",bSendMsg);
        }

        /// <summary>
        /// 해모글로빈 데이터 전송.
        /// </summary>
        /// <param name="_iValue">스펙트로미터 수치 12.34를 1234로 2자리 올림하여 스트링으로 변환해서 입력</param>
        public void SendHemog(string _sValue)
        {
            List<byte> lsMsg = new List<byte>();

            //먼저SendTestSeq 시험시작을 보내야 함.
            // 1234를전송  02 04 00 07 31 32 33 34 8B C6 03
            // 응답은 송신 내용 그대로 수신.



            lsMsg.Add( 0x02 ); //Start
            lsMsg.Add( (byte)EFuncCode.Hemoglobin ); //메세지 인덱스.
            ushort usLength = (ushort)( 4/*벨류 12.34 에서 숫자만 보냄*/ + 2/*CRC16*/ + 1/*ETX*/ );          
            lsMsg.Add( GetByteFromUshort(usLength , true ) ); //Length - High
            lsMsg.Add( GetByteFromUshort(usLength , false) ); //(iDec) ;//Length - Low

            //바코드 인코딩.
            byte[] bValue  = Encoding.ASCII.GetBytes(_sValue);
            lsMsg.AddRange( bValue );

            ushort usSum =  CRC16( lsMsg.ToArray(),lsMsg.Count) ;
            lsMsg.Add( GetByteFromUshort(usSum , false)); //checksum 
            lsMsg.Add( GetByteFromUshort(usSum , true )); //checksum f
            lsMsg.Add( 0x03 ); //ETX 

            //문제소지가 있을까봐 크기 맞춰준다.
            byte [] bSendMsg = lsMsg.ToArray();


            SendMsg("",bSendMsg);
        }

        void onCompleteWriteToServer(IAsyncResult iar)
        {
            TcpClient tcpc;
            try
            {
                tcpc = (TcpClient)iar.AsyncState;
                tcpc.GetStream().EndWrite(iar);
                bCompleted = true;
            }
            catch (Exception e) 
            { 
                Log.ShowMessage("TcpIp Error",e.Message); 
                bCompleted = false;
            };
        }

        static byte[] HexToBytes(string input)
        {
            byte[] result = new byte[input.Length / 2];
            for(int i = 0; i < result.Length; i++)
            {
                result[i] = Convert.ToByte(input.Substring(2 * i, 2), 16);
            }
            return result;
        }

        public static ushort CRC16(byte[] strPacket, int size)
        {
            if(strPacket.Length < size) return 0;

            ushort[] CRC16_TABLE = { 0x0000, 0xCC01, 0xD801, 0x1400, 0xF001, 0x3C00, 0x2800, 0xE401, 0xA001, 0x6C00, 0x7800, 0xB401, 0x5000, 0x9C01, 0x8801, 0x4400 };
            ushort usCRC = 0xFFFF;
            ushort usTemp = 0;
            
            //foreach (char cCurrent in strPacket)
            for(int i=0; i<size; i++)
            {
                byte bytCurrent = strPacket[i];// Convert.ToByte(cCurrent);// lower 4 bits 
                usTemp = CRC16_TABLE[usCRC & 0x000F];
                usCRC = (ushort)((usCRC >> 4) & 0x0FFF);
                usCRC = (ushort)(usCRC ^ usTemp ^ CRC16_TABLE[bytCurrent & 0x000F]); // Upper 4 Bits 
                usTemp = CRC16_TABLE[usCRC & 0x000F];
                usCRC = (ushort)((usCRC >> 4) & 0x0FFF);
                usCRC = (ushort)(usCRC ^ usTemp ^ CRC16_TABLE[(bytCurrent >> 4) & 0x000F]);
            } 
            return usCRC;
        }         
    }

}
