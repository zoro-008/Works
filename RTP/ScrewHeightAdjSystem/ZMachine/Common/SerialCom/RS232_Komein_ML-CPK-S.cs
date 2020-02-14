using COMMON;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace Machine
{
    //전자마이크로메타
    //http://www.komein.kr/korea/
    //ML-CPK-S4

        //처음에 Init로 마이크로 미터 클리어 하고 스타트는 끄고
        //측정시에 Start로 측정시작
        //끝낫는지 확인은 End로 체크 하고 중간에 GetErr 에러 체크 하고 
        //End 에서 5초딜레이 걸려 있음 스타트 시작

    public class RS232_MLCPKS
    {
        private SerialPort Port = null;

        private   int    iPortId     = 0  ;
        private   string sName       = "" ;
        private   int    iSeq        = 0  ;

        private   bool   bRcvEnd    = false;
        public    string sErr       = "";
        
        private yi yiStart;
        private yi yiClear;

        public struct THeight
        {
            public double dLeft1  ;
            public double dLeft2  ;
            public double dRight1 ;
            public double dRight2 ;
        };
        public THeight sHeight;

        public RS232_MLCPKS(int _iPortId , string _sName , yi _yiStart , yi _yiClear)
        {           
            iPortId    = _iPortId   ;
            sName      = _sName     ;

            yiStart = _yiStart;
            yiClear = _yiClear;
            Port = new SerialPort();

            Port.PortName     = "Com" + _iPortId.ToString();
            Port.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
            //Port.Encoding = System.Text.Encoding.GetEncoding("iso-8859-1");//이것이 8비트 문자 모두 가능 Ascii는 7비트라 63이상의 값은 표현 안됌.
            Port.BaudRate     = 9600         ;  // 15200/57600/38400/19200/9600 bps
            Port.DataBits     = 8            ;  //8
            Port.Parity       = Parity  .None;//None = 0,Odd = 1,Even = 2,Mark = 3 ,Space = 4, 
            Port.StopBits     = StopBits.One ;//None = 0,One = 1,Two  = 2,OnePointFive = 3, None은 예외상황발생함..
            Port.ReadTimeout  = (int)500;
            Port.WriteTimeout = (int)500;
            Port.Handshake    = Handshake.None ;

            PortOpen();

        }

        ~RS232_MLCPKS()
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
                
        public void Init()
        {
            sErr       = "";
            bRcvEnd    = false;
            sRecvedMsg = "";

            //ML.IO_SetY(yiClear,true ); //결과값을 얼마만큼 보여줄지가 넣을때 생각해야 할듯
            ML.IO_SetY(yiStart,false);
        }

        private CDelayTimer tmTimer;
        public void Start()
        {
            tmTimer = new CDelayTimer();
            sErr       = "";
            bRcvEnd    = false;
            sRecvedMsg = "";

            sHeight = new THeight();

            //ML.IO_SetY(yiClear,false);
            ML.IO_SetY(yiStart,true );
            tmTimer.Clear();
        }

        public bool End()
        {
            if (tmTimer.OnDelay(2000)) {
                sErr = "마이크로미터 측정 결과 전송 안됨";
                ML.IO_SetY(yiStart, false);
                Log.Trace(sErr);
            }
            return bRcvEnd;
        }

        public string GetErr()
        {
            return sErr;
        }

        private string sRecvedMsg = "";
        public string  RecvedMsg { get => sRecvedMsg; set => sRecvedMsg = value; }
        const char ETX = (char)0x03; //End Text 
        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                int iByteCntToRead = Port.BytesToRead ;
                if (iByteCntToRead <= 0) return;            

                byte[] ByteRead = new byte[iByteCntToRead];
                int iReadCount = Port.Read(ByteRead, 0, iByteCntToRead);
                sRecvedMsg += Encoding.ASCII.GetString(ByteRead, 0, iReadCount);

                //Ex : OK,05,+0.001,+0.000,+0.009,+0.001,+0.001@@
                //ENQ,OK,92,+0043,-0025,ETX,@@,CR,LF
                //sRecvedMsg += Port.ReadExisting();
                //"\u000501,04,+0000,+0000,+0000,+0000\u0003@@\r\n"
                if (!sRecvedMsg.Contains("@@\r\n")) return;
                sRecvedMsg = sRecvedMsg.Insert(sRecvedMsg.IndexOf(ETX),",");

                int i1 = sRecvedMsg.IndexOf(ETX);
                string[] split = sRecvedMsg.Split(',');
                if(split.Length < 7) {
                    sErr = "전자마이크로미터 수신값 오류 " + sRecvedMsg ;
                    Log.Trace(sErr);
                    return; 
                }

                //TODO:
                //맞는지 꼭 확인
                split[2] = split[2].Insert(3, ".") ;
                split[3] = split[3].Insert(3, ".") ;
                split[4] = split[4].Insert(3, ".") ;
                split[5] = split[5].Insert(3, ".") ;
                sHeight.dLeft1  = CConfig.StrToDoubleDef(split[2],0); //안쪽이 빠른번호로 되어 있음
                sHeight.dLeft2  = CConfig.StrToDoubleDef(split[3],0);
                sHeight.dRight1 = CConfig.StrToDoubleDef(split[4],0);
                sHeight.dRight2 = CConfig.StrToDoubleDef(split[5],0);

                bRcvEnd = true;
                sRecvedMsg = "";
                ML.IO_SetY(yiStart, false);

                Log.TraceListView(" Left 1-" + sHeight.dLeft1 .ToString() +
                                  " Left 2-" + sHeight.dLeft2 .ToString() +
                                  " Right 1-" + sHeight.dRight1.ToString() +
                                  " Right 2-" + sHeight.dRight2.ToString() );

            }
            catch (Exception _e)
            {
                Log.Trace("전자 마이크로 미터 전송 결과값 오류" + _e.Message);
                ML.IO_SetY(yiStart, false);
                bRcvEnd    = false;
                sRecvedMsg = ""   ;

            }
        }
    }
}
