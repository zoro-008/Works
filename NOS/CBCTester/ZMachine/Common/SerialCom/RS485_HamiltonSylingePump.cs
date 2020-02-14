using COMMON;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Machine
{
    //미국 해밀턴사의 실린지 펌프 뉴옵틱스 프로젝트 하면서 5개 사용.
    public enum VALVE_POS
    {
        Input  ,
        Output ,
        Bypass ,
        Extra  
    }

    /*
Speed Code   Motor steps per second Maximum Velocity in seconds per stroke Speed Code Motor steps per second Maximum Velocity in seconds per stroke
1 5,600 1.2    21 160 37.5
2 5,000 1.3    22 150 40.0
3 4,400 1.4    23 140 43.0
4 3,800 1.6    24 130 46.0
5 3,200 1.9    25 120 50.0
6 2,600 2.2    26 110 55.0
7 2,200 2.6    27 100 60.0
8 2,000 2.9    28 90 67.0
9 1,800 3.3    29 80 75.0
10 1,600 3.7   30 70 86.0
11 1,400 4.3   31 60 100.0
12 1,200 5.0   32 50 120.0
13 1,000 6.0   33 40 150.0
14 800 7.5    34  30 200.0
15 600 10.0    35 20 300.0
16 400 15.0    36 18 333.3
17 200 30.0    37 16 375.0
18 190 31.0    38 14 428.6
19 180 33.0    39 12 500.0
20 170 35.5    40 10 600.0
         */




    public class RS485_HamiltonSyringePump
    {


        private SerialPort Port = null;

        private CDelayTimer m_tmTimeOut = new CDelayTimer();

        private   int    iPortId     = 0  ;
        private   string sName       = "" ;
        private   int    iSeq        = 0  ;

        //이게 원래 실린지펌프 갯수만큼 있어야 하는데.. 일단 보류
        //private   byte[] ByteMsg          ; //보낼 메세지를 넣어놓고 업데이트에서 보낸다.
        private   int    iLastSendedAdd = -1; //-1 답변받은상태    1~ 
        private   List<string> lsMsgQue = new List<string>();


        private   int    iErrCode    = 0  ;//실린지펌프에서의 에러코드 
        private   bool[] bBusy             ; //실린지펌프에서 의 비지.

        //private   string sSetMsg    = "";
        //private   string sRecvedMsg = "";
        List<byte> lsRcvMsg  = new List<byte>();
        private   bool   bRcvEnd    = false;
        
        private   const byte  STX  = 0x02 ;
        private   const byte  ETX  = 0x03 ;
        private   const byte  CR   = 0x0D ;
        private   const byte  LF   = 0x0A ;

        public struct TStat
        {

        };
        public TStat Stat;

        public RS485_HamiltonSyringePump(int _iPortId , string _sName , int _iMaxPumpCnt=1)
        {           
            iPortId    = _iPortId   ;
            sName      = _sName     ;

            Port = new SerialPort();

            // ,9600    38400사용하려면 딥스위치 3번 켜야함.
            // 1개만 쓰던가 체인에서 맨마지막 유닛은 7,8번 온켜야함.

            Port.PortName     = "Com" + _iPortId.ToString();
            Port.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
            //Port.Encoding = System.Text.Encoding.GetEncoding("iso-8859-1");//이것이 8비트 문자 모두 가능 Ascii는 7비트라 63이상의 값은 표현 안됌.
            Port.BaudRate     = 38400 ; // ,9600 38400사용하려면 딥스위치 3번 켜야함.
            Port.DataBits     = 8   ;  //8
            Port.Parity       = Parity  .None;//None = 0,Odd = 1,Even = 2,Mark = 3 ,Space = 4, 
            Port.StopBits     = StopBits.One ;//None = 0,One = 1,Two  = 2,OnePointFive = 3, None은 예외상황발생함..
            Port.ReadTimeout  = (int)500;
            Port.WriteTimeout = (int)500;
            Port.Handshake    = Handshake.None ;

            PortOpen();

            bBusy = new bool [_iMaxPumpCnt];

            //for(int i = 0; i < _iMaxPumpCnt; i++)
            //{
            //    YR(i);
            //}

            
        }

        ~RS485_HamiltonSyringePump()
        {

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

        int iIdIdx = 0 ;
        public void Update()
        {
            if(bRcvEnd)
            {
                if(lsMsgQue.Count > 0)
                {
                    Write(lsMsgQue[0]);
                    lsMsgQue.RemoveAt(0);
                    m_tmTimeOut.Clear();
                }
                else
                {
                    if(iIdIdx >= bBusy.Length) //실린지가 몇개인지 근냥 Busy플래그 갯수로 파악
                    {
                        iIdIdx = 0 ;
                    }
                    SendMsg(iIdIdx , "QR");//"?20000"); //큐에 넣는다.
                    iIdIdx++;
                }
            }
            else
            {
                if(m_tmTimeOut.OnDelay(200))//타임아웃.
                {
                    iLastSendedAdd = -1 ; 
                    bRcvEnd = true ;
                }

            }
        }

        //<STX><ADDRESS><SEQUENCE><DATA><ETX><CHECKSUM>
        public bool SendMsg(int _iAdd , string _sData , bool bResend = false )
        {
            if(_iAdd > bBusy.Length) //아이디 오버.
            {
                return false ;
            }

            


            if(!bResend)iSeq ++ ;            
            if(iSeq > 7)iSeq = 0 ;
            
            byte Seq = 0b110000 ;
            Seq += (byte)iSeq ;
            if(bResend) Seq += 0b1000 ; //다시 보내는 경우.

            const int iEtcCnt = 5 ; //STX , Address , Seq , ETX , CHECKSUM
            int iMsgLeghth = _sData.Length;
            byte[] ByteData = Encoding.ASCII.GetBytes(_sData); 
            int iFullMsgCnt = ByteData.Length+iEtcCnt ;
            byte[] ByteMsg = new Byte[iFullMsgCnt];
            

            ByteMsg[0] = STX ;
            ByteMsg[1] = GetAddress(_iAdd);
            ByteMsg[2] = Seq ;
            int i ;
            for(i = 0 ; i < ByteData.Length ; i++)
            {
                ByteMsg[3+i] = ByteData[i];
            }
            ByteMsg[ByteMsg.Length - 2] = ETX;

            // ZR
            //02H 31H 31H 5AH 52H 03H 09H 

            ByteMsg[ByteMsg.Length - 1] = GetCheckSum(ByteMsg , 0 , ByteMsg.Length-1);//맨마지막은 체크썸 자리라서 뺌.

            string sTemp = Encoding.Default.GetString(ByteMsg);

            lsMsgQue.Add(sTemp);
            

            return true ;
        }

        private bool Write(string _sMsg)
        {
            bRcvEnd = false ;
            iErrCode = 0 ;

            try
            {
                
                if(!int.TryParse(_sMsg.Substring(1,1),out iLastSendedAdd) )
                {
                    iLastSendedAdd = -1 ;
                    return false;
                }
                iLastSendedAdd = iLastSendedAdd - 1 ; //실린지는 1번부터 카운팅

                Port.Write(_sMsg);
            }
            catch(Exception _e)
            {
                return false ;
            }
            return true ;
        }

        //비트별로 1의갯수 새서 홀수면 1 짝수면 0
        byte GetCheckSum(Byte [] _bData , int _iOffset , int _iLength)
        {
            byte ret = 0 ;

            for(int i = 0 ; i < _iLength ; i++)
            {
                ret ^= _bData[_iOffset + i];
            }
            return ret ;
        }

        byte GetAddress(int _iAdd)
        {
            byte Add = 0x31 ;            
            Add += (byte)_iAdd ;
            return Add ;
        }
        
        //<STX>0<STATUS_BYTE><DATA><ETX><CHECKSUM>
        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int iByteCntToRead = Port.BytesToRead ;
            if (iByteCntToRead <= 0) return ;            

            byte[] ByteRead = new byte[iByteCntToRead];
            int iReadCount = Port.Read(ByteRead, 0, iByteCntToRead);

            lsRcvMsg.AddRange(ByteRead);

            //sRecvedMsg += Encoding.ASCII.GetString(ByteRead, 0, iReadCount);
            int iTemp = lsRcvMsg.IndexOf(ETX) ;
            if (lsRcvMsg.IndexOf(STX) < 0) return;//STX 글자가 있어야 하고.   
            if (lsRcvMsg.IndexOf(ETX) < 0) return;//ETX 글자가 있어야 하고.
            if (lsRcvMsg.Count  <= lsRcvMsg.LastIndexOf(ETX)+1) return; //ETX+1 에 CheckSum까지 받아야함.
            int iSTXPos = lsRcvMsg.LastIndexOf(STX) ;
            int iETXPos = lsRcvMsg.LastIndexOf(ETX) ;
            if(iSTXPos > iETXPos) return ; //아직 마지막 스타트텍스트에 대한 엔드 텍스트 도착 안함.

            
             
            


            if (lsRcvMsg[iSTXPos + 1] != '0')
            {
                Log.Trace("Error" , Port.PortName + " ID Error");
                iLastSendedAdd =-1 ;
                bRcvEnd = true;
                return; //0번이 상위제어기 아이디.
            }
            if (lsRcvMsg[iETXPos+1] != GetCheckSum(lsRcvMsg.ToArray() ,iSTXPos , iETXPos - iSTXPos+1))
            {
                //체크썸 이상.
                Log.Trace("Error" , Port.PortName + " CheckSum Error");
                iLastSendedAdd = -1 ;
                bRcvEnd = true;
                return ;
            }

            //0x60 == 0110 0000 Ready
            //0x40 == 0100 0000 Busy
            try
            {
                byte Status = lsRcvMsg[iSTXPos + 2];
                if(iLastSendedAdd >= 0 && iLastSendedAdd < bBusy.Length)  bBusy[iLastSendedAdd] = (Status & 0x20) == 0;
                bool bTemp = (Status & 0b0001000) == 0;
                iErrCode = Status & 0b00001111;

                lsRcvMsg.RemoveRange(0, lsRcvMsg.IndexOf(ETX) + 2);
            }
            catch(Exception _e)
            {
                Log.TraceListView(_e.Message);
                lsRcvMsg.Clear();
            }
            

            bRcvEnd = true;

        }

        //private void Load(string _sName , SerialPort _Port , string _sPath = "")
        //{
        //    //Set Dir.
        //    string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
        //    string sFileName = "Util\\SerialPort" ;
        //    string sSerialPath = sExeFolder + sFileName + ".ini";
        //    string sTitle = "";
        //    
        //    if(sName=="")sTitle = "PortID_" + iPortId.ToString();
        //    else         sTitle = _sName ;
        //
        //    CIniFile Ini ;
        //    if(_sPath == "")Ini = new CIniFile(sSerialPath);
        //    else            Ini = new CIniFile(_sPath     );
        //
        //    int iPortNo = 0  ;
        //    Ini.Load(sTitle, "PortNo", ref iPortNo ); if(iPortNo==0)iPortNo= 1 ;
        //
        //    //맨처음 파일 없을때 대비.
        //    Ini.Save(sTitle, "PortNo   ", iPortNo   );
        //
        //    _Port.PortName     = "Com" + iPortNo.ToString();
        //}

        //병신같이 인풋아웃풋 설정 하려면 꼭 실린지 초기화 동작이 필요함. Extention Command H100xx를 이용하면 
        //설정 빼고 실린지만 초기화 할 수 있는데 이경우 껐다켰을때 ZR YR를 기억 하고 있는지 확인 해봐야 하는데 귀찮아서 이방법을 씀.
        //YR 설정시에 벨브 이동 함수에서 반전해줌.
        //YR로 하는 이유는 input으로 하고 실린지 초기화를 해야 하는데 이놈의 ZR YR 을 하면 아웃풋 쪽으로 벨브를 돌리고 실린지 초기화를 함.
        const bool bYRSetting = true; 
        
        public bool AbsMove(int _iAdd , VALVE_POS _eV , int _iPos , int _iSpdCode = 0 )
        {
            string sCmd = "" ;

            //pos : 0~3000
            //Sx :  스피드 세팅 1~40까지. 1이가장빠르고 40이 가장느림.
            //Vx :  맥시멈 벨로서티 세팅
            if(_iSpdCode < 1)
            {
                sCmd = "";
            }
            else if(_iSpdCode > 40)
            {
                sCmd = "S40";
            }
            else if(_iSpdCode > 0)

            {
                sCmd = "S"+_iSpdCode.ToString();
            }

            if(_eV == VALVE_POS.Input)
            {
                if(bYRSetting) sCmd += "O";
                else           sCmd += "I";
            }
            else if(_eV == VALVE_POS.Output)
            {
                if (bYRSetting) sCmd += "I";
                else            sCmd += "O";
            }
            else if(_eV == VALVE_POS.Bypass)
            {
                sCmd +="B";
            }
            else
            {
                sCmd +="B";
            }

            sCmd += "A" + _iPos.ToString() + "R";

            bool bRet = SendMsg(_iAdd , sCmd);

            return bRet ;
        }

        public bool DispIncPos(int _iAdd , VALVE_POS _eV , int _iPos , int _iSpdCode = 0 )
        {
            string sCmd = "" ;

            //pos : 0~3000
            //Sx :  스피드 세팅 1~40까지.1이가장빠르고 40이 가장느림.
            //Vx :  맥시멈 벨로서티 세팅
            if(_iSpdCode < 1)
            {
                sCmd = "";
            }
            else if(_iSpdCode > 40)
            {
                sCmd = "S40";
            }
            else if(_iSpdCode > 0)

            {
                sCmd = "S"+_iSpdCode.ToString();
            }

            if(_eV == VALVE_POS.Input)
            {
                if(bYRSetting) sCmd += "O";
                else           sCmd += "I";
            }
            else if(_eV == VALVE_POS.Output)
            {
                if (bYRSetting) sCmd += "I";
                else            sCmd += "O";
            }
            else if(_eV == VALVE_POS.Bypass)
            {
                sCmd +="B";
            }
            else
            {
                sCmd +="B";
            }

            sCmd += "D" + _iPos.ToString() + "R";

            bool bRet = SendMsg(_iAdd , sCmd);

            return bRet ;
        }

        public bool PickupIncPos(int _iAdd , VALVE_POS _eV , int _iPos , int _iSpdCode = 0 )
        {
            string sCmd = "" ;

            //pos : 0~3000
            //Sx :  스피드 세팅 1~40까지.1이가장빠르고 40이 가장느림.
            //Vx :  맥시멈 벨로서티 세팅
            if(_iSpdCode < 1)
            {
                sCmd = "";
            }
            else if(_iSpdCode > 40)
            {
                sCmd = "S40";
            }
            else if(_iSpdCode > 0)

            {
                sCmd = "S"+_iSpdCode.ToString();
            }

            if(_eV == VALVE_POS.Input)
            {
                if(bYRSetting) sCmd += "O";
                else           sCmd += "I";
            }
            else if(_eV == VALVE_POS.Output)
            {
                if (bYRSetting) sCmd += "I";
                else            sCmd += "O";
            }
            else if(_eV == VALVE_POS.Bypass)
            {
                sCmd +="B";
            }
            else
            {
                sCmd +="B";
            }

            sCmd += "P" + _iPos.ToString() + "R";

            bool bRet = SendMsg(_iAdd , sCmd);

            return bRet ;
        }

        public bool PickupAndDispInc(int _iAdd , VALVE_POS _eVPickup , VALVE_POS _eVDisp , int _iPos , int _iSpdCode , int _iTimes = 1)
        {
            string sCmd = "" ;

            if (_iTimes < 1) _iTimes = 1;

            //Sx :  스피드 세팅 1~40까지.1이가장빠르고 40이 가장느림.
            //Vx :  맥시멈 벨로서티 세팅
            if (_iSpdCode < 1)
            {
                sCmd = "";
            }
            else if(_iSpdCode > 40)
            {
                sCmd = "S40";
            }
            else if(_iSpdCode > 0)

            {
                sCmd = "S"+_iSpdCode.ToString();
            }

            

            if(_eVPickup == VALVE_POS.Input)
            {
                if(bYRSetting) sCmd += "O";
                else           sCmd += "I";
            }
            else if(_eVPickup == VALVE_POS.Output)
            {
                if (bYRSetting) sCmd += "I";
                else            sCmd += "O";
            }
            else if(_eVPickup == VALVE_POS.Bypass)
            {
                sCmd +="B";
            }
            else
            {
                sCmd +="B";
            }

            sCmd += "P" + _iPos.ToString() ; //+ "R";


            if(_eVDisp == VALVE_POS.Input)
            {
                if(bYRSetting) sCmd += "O";
                else           sCmd += "I";
            }
            else if(_eVDisp == VALVE_POS.Output)
            {
                if (bYRSetting) sCmd += "I";
                else            sCmd += "O";
            }
            else if(_eVDisp == VALVE_POS.Bypass)
            {
                sCmd +="B";
            }
            else
            {
                sCmd +="B";
            }

            sCmd += "D" + _iPos.ToString() + ((_iTimes > 1) ? "G"+_iTimes.ToString() : "") +"R"; 


            bool bRet = SendMsg(_iAdd , sCmd);

            return bRet ;
        }

        public bool DispAndPickupInc(int _iAdd, VALVE_POS _eVDisp, VALVE_POS _eVPickup , int _iPos, int _iSpdCode, int _iTimes = 1)
        {
            string sCmd = "";

            if (_iTimes < 1) _iTimes = 1;

            //Sx :  스피드 세팅 1~40까지.1이가장빠르고 40이 가장느림.
            //Vx :  맥시멈 벨로서티 세팅
            if (_iSpdCode < 1)
            {
                sCmd = "";
            }
            else if (_iSpdCode > 40)
            {
                sCmd = "S40";
            }
            else if (_iSpdCode > 0)

            {
                sCmd = "S" + _iSpdCode.ToString();
            }

            //디스펜스 시퀜스
            if (_eVDisp == VALVE_POS.Input)
            {
                if (bYRSetting) sCmd += "O";
                else sCmd += "I";
            }
            else if (_eVDisp == VALVE_POS.Output)
            {
                if (bYRSetting) sCmd += "I";
                else sCmd += "O";
            }
            else if (_eVDisp == VALVE_POS.Bypass)
            {
                sCmd += "B";
            }
            else
            {
                sCmd += "B";
            }
            sCmd += "D" + _iPos.ToString();

            //픽업 시퀜스.
            if (_eVPickup == VALVE_POS.Input)
            {
                if (bYRSetting) sCmd += "O";
                else sCmd += "I";
            }
            else if (_eVPickup == VALVE_POS.Output)
            {
                if (bYRSetting) sCmd += "I";
                else sCmd += "O";
            }
            else if (_eVPickup == VALVE_POS.Bypass)
            {
                sCmd += "B";
            }
            else
            {
                sCmd += "B";
            }
            sCmd += "P" + _iPos.ToString(); //+ "R";


            sCmd += ((_iTimes > 1) ? "G" + _iTimes.ToString() : "") + "R";


            bool bRet = SendMsg(_iAdd, sCmd);

            return bRet;
        }

        public bool YR(int _iAdd )
        {
            string sCmd ="YR";
            bool bRet = SendMsg(_iAdd , sCmd);

            return bRet ;

        }

        //아.. 씨부엉....
        //응답이 채널넘버를 붙여서 오는게 아니라 나중에 해보고 생각하자.
        //결국엔 했다 ;;ㅜㅠ
        public bool GetBusy(int _iAdd)
        {
            if(_iAdd > bBusy.Length) //아이디 오버.
            {
                return false ;
            }

            return bBusy[_iAdd];
        }
    }
}
