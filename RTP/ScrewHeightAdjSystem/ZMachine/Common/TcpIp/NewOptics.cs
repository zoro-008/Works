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
    class NewOptics
    {
        #region private
        bool   bConnected = false;
        bool   bCompleted = false;
        bool   bChanged   = false;
        state  iState     = state.None;
        #endregion

        //서버에 연결되어 있는지 확인
        public bool Connected { get => mTcpClient == null? false : mTcpClient.Connected; }
        //잘 보내졋는지 확인 SendMsg시에 false되서 보내고 나서 확인가능
        public bool Completed { get => bCompleted; }// set => bCompleted = value; } 
        //200ms마다 보내고 있는게 있어서 보내고 있는지 확인용, 밖에서 바뀌고 있는지 검사 해야됨
        public bool Changed   { get => bChanged;   }// set => bChanged   = value; } 
        //200ms마다 보내는 상태 확인용
        internal state State { get => iState; set => iState = value; }

        #region enum
        public enum state : uint
        {
            None    = 0,
            Ready   = 1,
            Running = 2,
            Error   = 3
        }

        public enum test : uint
        {
            FCM_1_TEST    =  1,
            FCM_2_TEST    =  2,
            FCM_3_TEST    =  3,
            FCM_4_TEST    =  4,
            DC_TEST       = 11,
            QC_FCM_1_TEST = 21,
            QC_FCM_2_TEST = 22,
            QC_FCM_3_TEST = 23,
            QC_FCM_4_TEST = 24
        }
        #endregion

        TcpClient mTcpClient;
        byte[] mRx;

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

        public NewOptics() 
        { 
            //test
            //var bytes = HexToBytes("0201000401");
            //string sSend  = cReady.ToString() + cRunning.ToString() + cCRC16_1.ToString() + cCRC16_2.ToString();
            //string str = "A1234567890";
            //int itet = str.Length;
            SendMsg(test.FCM_3_TEST,"A1234567890");

            /*
             * //Ex
             * //Server -> Client
             * byte [] bSendMsg = new byte[8];
             * bSendMsg[0] = (byte)cStart   ;//(byte)0x02        ;
             * bSendMsg[1] = (byte)cFuction ;//(byte)0x01        ;
             * bSendMsg[2] = (byte)cDataH   ;//(byte)0x00        ;
             * bSendMsg[3] = (byte)cDataL   ;//(byte)0x04        ;
             * bSendMsg[4] = (byte)cReady   ;//(byte)0x01        ;
             * bSendMsg[5] = (byte)( CRC16( bSendMsg, bSendMsg.Length-3)&0x00FF    ); //체크에러의 하위바이트
             * bSendMsg[6] = (byte)((CRC16( bSendMsg, bSendMsg.Length-3)&0xFF00)>>8); //체크에러의 상위바이트  //SizeOf(TWriteSigleRegster) -2);
             * //bSendMsg[5] = (byte)0x9F        ;
             * //bSendMsg[6] = (byte)0x3C        ;
             * bSendMsg[7] = (byte)cEnd     ;//(byte)0x03        ;
             * 
             */

            /*
             * ////Ex
             * //Client -> Server
             * byte [] bRsvMsg = new byte[20];
             * byte [] bRsv    = new byte[ 2];
             * bRsvMsg[ 0] = (byte)0x02 ;//Start
             * bRsvMsg[ 1] = (byte)0x02 ;//Test
             * bRsvMsg[ 2] = (byte)0x00 ;//Length - High
             * bRsvMsg[ 3] = (byte)0x10 ;//Length - Low
             * bRsvMsg[ 4] = (byte)0x01 ;//Test - 01 : Run , 02 : Stop
             * bRsvMsg[ 5] = (byte)0x03 ;//시험종류 01-FCM1Test, 02-FCM2TEST, 03-FCM3TEST, 04-FCM4TEST, 11-DC TEST, 12-(Double 8 Bytes) X (Fz Array Length), 21-QC-FCM1TEST, 22-QC-FCM2TEST, 23-QC-FCM3TEST, 24-QC-FCM4TEST
             * bRsvMsg[ 6] = (byte)0x41 ;//Barcode
             * bRsvMsg[ 7] = (byte)0x31 ;//Barcode
             * bRsvMsg[ 8] = (byte)0x32 ;//Barcode
             * bRsvMsg[ 9] = (byte)0x33 ;//Barcode
             * bRsvMsg[10] = (byte)0x34 ;//Barcode
             * bRsvMsg[11] = (byte)0x35 ;//Barcode
             * bRsvMsg[12] = (byte)0x36 ;//Barcode
             * bRsvMsg[13] = (byte)0x37 ;//Barcode
             * bRsvMsg[14] = (byte)0x38 ;//Barcode
             * bRsvMsg[15] = (byte)0x39 ;//Barcode
             * bRsvMsg[16] = (byte)0x30 ;//Barcode
             * //bRsv[0] = (byte)( CalcCRC( bRsvMsg, bRsvMsg.Length-3)&0x00FF     ); //체크에러의 하위바이트(byte)0x6C ;//CRC16
             * //bRsv[1] = (byte)((CalcCRC( bRsvMsg, bRsvMsg.Length-3)&0xFF00)>>8 ); //체크에러의 상위바이트(byte)0xDC ;//CRC16
             * //string sRsv0 = BitConverter.ToString(bRsv,0,1);
             * //string sRsv1 = BitConverter.ToString(bRsv,1,1);
             * //bRsv = Encoding.UTF8.GetBytes(sRsv0);
             * //bRsv = Encoding.UTF8.GetBytes(sRsv1);
             * bRsvMsg[17] = (byte)( CRC16( bRsvMsg, bRsvMsg.Length-3)&0x00FF     ); //체크에러의 상위바이트(byte)0x6C ;//CRC16
             * bRsvMsg[18] = (byte)((CRC16( bRsvMsg, bRsvMsg.Length-3)&0xFF00)>>8 ); //체크에러의 하위바이트(byte)0xDC ;//CRC16
             * bRsvMsg[19] = (byte)0x03 ;//End
             */ 

        }

        public NewOptics(string _sIp, int _iPort) { Connect(_sIp,_iPort); }

        /// <summary>
        /// 서버 연결하기
        /// </summary>
        /// <param name="_sIp"></param>
        /// <param name="_iPort"></param>
        public bool Connect(string _sIp, int _iPort)
        {
            if(mTcpClient != null && mTcpClient.Connected) return false; //연결이 되어 있는데 또 연결 방지용, 서버쪽에 연결된 노드가 늘어남
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
                mTcpClient.BeginConnect(ipa, _iPort,onCompleteConnect,mTcpClient);
                
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
                mRx = new byte[512];
                tcpc.GetStream().BeginRead(mRx, 0, mRx.Length, onCompleteReadFromServerStream, tcpc);
                bConnected = true; //연결 성공
            }
            catch (Exception e)
            {
                Log.ShowMessage("TcpIp Error",e.Message);
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
            string strReceived;

            try
            {
                tcpc = (TcpClient)iar.AsyncState;
                nCountBytesReceivedFromServer = tcpc.GetStream().EndRead(iar);

                if (nCountBytesReceivedFromServer == 0)
                {
                    Log.ShowMessage("TcpIp Error","Connection broken.");
                    return;
                }
                strReceived = Encoding.ASCII.GetString(mRx, 0, nCountBytesReceivedFromServer);

                //데이터 복사
                byte [] bMsg = mRx ;//new byte[nCountBytesReceivedFromServer];

                mRx = new byte[512];
                tcpc.GetStream().BeginRead(mRx, 0, mRx.Length, onCompleteReadFromServerStream, tcpc);

                //받은 데이터 체크
                if(nCountBytesReceivedFromServer == 8)
                {
                    ushort usCRC16  = CRC16(bMsg , 5) ; //nCountBytesReceivedFromServer - 3
                    byte   bCRC16Lo = (byte)(usCRC16 & 0x00FF) ;
                    byte   bCRC16Hi = (byte)((usCRC16 & 0xFF00)>>8) ;
                    
                    if (bCRC16Lo == bMsg[5] && bCRC16Hi == bMsg[6])
                    {
                             if(bMsg[4] == (byte)cReady  ) iState = state.Ready  ;
                        else if(bMsg[4] == (byte)cRunning) iState = state.Running;
                        else                               iState = state.Error  ;

                        bChanged = !bChanged;
                    }
                }
                //Log.Trace(strReceived); 남기기 애매해서 안남김

                bConnected = true;

            }
            catch (Exception e)
            {
                Log.ShowMessage("TcpIp Error",e.Message);
                bConnected = false;
            }
        }

        public void SendMsg(string _sMsg,byte[] _tx = null)
        {
            bCompleted = false;

            string sMsg = _sMsg;
            byte[] tx;
            
            if (string.IsNullOrEmpty(sMsg) && _tx == null) return;

            try
            {
                if(_tx == null) tx = Encoding.ASCII.GetBytes(sMsg);
                else            tx = _tx;

                if (mTcpClient != null)
                {
                    if (mTcpClient.Client.Connected)
                    {
                        mTcpClient.GetStream().BeginWrite(tx, 0, tx.Length, onCompleteWriteToServer, mTcpClient);
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
        /// FCM서버로 데이터 보내기
        /// </summary>
        /// <param name="_test"></param>
        /// <param name="_barcode"></param>
        public void SendMsg(test _test,string _barcode)
        {
            byte [] bMsg = new byte[512];
            bMsg[ 0] = (byte)0x02 ;//Start
            bMsg[ 1] = (byte)0x02 ;//Test
            bMsg[ 2] = (byte)0x00 ;//Length - High
            //string sHex = (_barcode.Length+5).ToString() ;//Length에서 1을 뺀걸 왜 헥사값으로 가지는지 알수가 없네...
            //int    iDec =int.Parse(sHex,System.Globalization.NumberStyles.HexNumber);
            //알고보니 5번째바이트부터 끝까지의 갯수임 (Test Run, FCM 3 Test, Barcode, CRC16, End)
            bMsg[ 3] = (byte)(_barcode.Length+5); //(iDec) ;//Length - Low
            bMsg[ 4] = (byte)0x01 ;//Test - 01 : Run , 02 : Stop
            bMsg[ 5] = (byte)_test ;//시험종류 01-FCM1Test, 02-FCM2TEST, 03-FCM3TEST, 04-FCM4TEST, 11-DC TEST, 12-(Double 8 Bytes) X (Fz Array Length), 21-QC-FCM1TEST, 22-QC-FCM2TEST, 23-QC-FCM3TEST, 24-QC-FCM4TEST
            
            int i = 0;
            for(i=0; i<_barcode.Length; i++)
            {
                //int    iMsg = _barcode[i];//Convert.ToInt32(((int)_barcode[i]).ToString("X"));
                //string sHex = Convert.ToString(iMsg, 16);
                //int.TryParse(sHex,out int _iHex);
                //bMsg[ 6+i] = (byte)_iHex;//Convert.ToByte(_barcode[i]);//_barcode[i].ToString("x");//(byte)_barcode[i]. ;//Barcode
                bMsg[ 6+i] = (byte)_barcode[i];
            }
            bMsg[6+i] = (byte)( CRC16( bMsg,9+i-3)&0x00FF    ); //체크에러의 하위바이트(byte)0x6C ;//CRC16
            bMsg[7+i] = (byte)((CRC16( bMsg,9+i-3)&0xFF00)>>8); //체크에러의 상위바이트  //SizeOf(TWriteSigleRegster) -2);(byte)0xDC ;//CRC16
            bMsg[8+i] = (byte)0x03 ;//End

            //문제소지가 있을까봐 크기 맞춰준다.
            byte [] bSendMsg = new byte[9+i];
            System.Buffer.BlockCopy(bMsg,0,bSendMsg,0,9+i);
            //보내기
            bCompleted = false;

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
            } return usCRC;
        }         
    }

}
