using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Windows.Forms;
using COMMON;

namespace COMMON
{
    /*
         
         // Int로 변환 후 Byte로 Casting
         byte a = (byte)Convert.ToInt32("0x" + "FF", 16);   -> 결과는 255 같음, 단 16을 생략하면 10진수로 해석함
         byte a = (byte)Convert.ToInt32("FF", 16);          -> 결과는 255가 된다.
          
         // 16진수의 Strting 화면 표시
         string.Format("{0:x2}, 10);        // -> "0a" 가 표시됨
         string.Format("{0:x5}, 255);       // -> "000ff" 가 표시됨
         string.Format("{0:X5}, 255);       // -> "000FF" 가 표시됨
         
         // 10진수의 String 화면 표시
         string.Format("{0:000}", 41);      // -> "041" 이 표시됨
         
     * 
     * 
     * 
     * 
     * 
     * 
      */
    /*
    처음 프린터를 받으면 파라미터에서 tear off에서 Peel Off모드로 설정 하고 MEDIA AND RIBON 파라미터로 가서 용지에 대한 켈리브레이션을 수행 해야 한다.
    그리고 다크네스가 처음에 4인가로 설정 되어 있는데 이것을 올려야 찍힌다 4는 안찍힌다.
    MaximumLength 를 50mm언더로는 설정이 안되나 50mm로 설정 해야지 위아래 인쇄 짤림이 없음.    
     유튜브에 찾아보면 잘나와 있고 제공 시디에도 동영상 있음.
     */
    delegate void ReceivedCallback();
    public class RS232_110Xi4
    {
        private SerialPort Port = null;

        private   int    iPortId     = 0  ;
        private   string sName       = "" ;

        private   string sSetMsg    = "";
        private   string sRecvedMsg = "";
        private   string sSendedMsg = "";
        private   bool   bRcvEnd    = false;
        
        private   const char   cSTX  = (char)0x02 ;
        private   const char   cETX  = (char)0x03 ;
        private   const char   cCR   = (char)0x0D ;
        private   const char   cLF   = (char)0x0A ;

        public struct TStat
        {
            public bool isPaperout      ;  // Paper out flag(1 = paper out)
            public bool isPauseFlag     ;  // pause flag(1 = pause active)
            public bool isDataBusy      ;  // Patial format flag( 1 = partial format inprogress)
            public bool isHeadupFlag    ;  // head up flag(1 = head up position)
            public bool isRibonoutFlag  ;  // ribion out flag( 1 = ribon out)
            public bool isLabelWaitFlag ;  // label waiting flag(1 = label waiting)
        };
        public TStat Stat;

        //파일로 초기화.
        //_iComNo은 1번부터 입력 되어야 한다.
        public RS232_110Xi4(int _iPortId , string _sName)
        {           
            iPortId    = _iPortId   ;
            sName      = _sName     ;

            Port = new SerialPort();
            Port.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
            //Port.Encoding = System.Text.Encoding.GetEncoding("iso-8859-1");//이것이 8비트 문자 모두 가능 Ascii는 7비트라 63이상의 값은 표현 안됌.

            Load(sName, Port); //"Zebra110Xi4"
            PortOpen();

            Port.BaudRate     = 9600; //9600 , 
            Port.DataBits     = 8   ;  //8
            Port.Parity       = Parity  .None;//None = 0,Odd = 1,Even = 2,Mark = 3 ,Space = 4, 
            Port.StopBits     = StopBits.One ;//None = 0,One = 1,Two  = 2,OnePointFive = 3, None은 예외상황발생함..
            Port.ReadTimeout  = (int)500;
            Port.WriteTimeout = (int)500;
        }

        ~RS232_110Xi4()
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
            if (!Port.IsOpen) return false ;

            bRcvEnd    = false;
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
                    Port.Write(ByteMsg8, 0, (j % 8)+1);
                }
            }
            return true ;
        }
        
        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int iByteCntToRead = Port.BytesToRead ;
            if (iByteCntToRead <= 0) return ;            

            byte[] ByteRead = new byte[iByteCntToRead];
            int iReadCount = Port.Read(ByteRead, 0, iByteCntToRead);
            sRecvedMsg += Encoding.ASCII.GetString(ByteRead, 0, iReadCount);

            string sEndText = cETX.ToString() + cCR.ToString() + cLF.ToString();
            if (sRecvedMsg.IndexOf(sEndText) < 0) return;//ETX 글자가 있어야 하고.
            if (sRecvedMsg.IndexOf(cSTX    ) < 0) return;//STX 글자가 있어야 하고.            

            //STX ETX 빼고 그사이것만 가져옴.
            string sTemp = sRecvedMsg.Substring(sRecvedMsg.IndexOf(cSTX) + 1, sRecvedMsg.IndexOf(cETX) - sRecvedMsg.IndexOf(cSTX) - 1);
            sRecvedMsg = sRecvedMsg.Remove(0, sRecvedMsg.IndexOf(sEndText) + sEndText.Length);
            if (sTemp.Length == 32)
            {
                string[] Items = sTemp.Split(new string[] { "," }, StringSplitOptions.None);
                if (sTemp.Substring(0, 3) == "014")//"014,0,0,0156,000,0,0,0,000,0,0,0\r\n001"
                {
                    Stat.isPaperout  = Items[1] == "1";
                    Stat.isPauseFlag = Items[2] == "1";
                    Stat.isDataBusy  = Items[7] == "1";
                }
                else if (sTemp.Substring(0, 3) == "001")//"001,0,0,0,1,2,6,0,00000000,1,000"
                {
                    /*
                     <STX>mmm,n,o,p,q,r,s,t,uuuuuuuu,v,www<ETX><CR><LF>
                     mmm = Function Settings(*) n = 0 (Unused) o = “Head Up” Flag (1 = Head in UP Position) p = “Ribbon Out” Flag (1= Ribbon Out) q = “Thermal Transfer Mode” Flag (1 = Thermal Transfer Mode Selected) r = Print Mode 0 = Rewind 1=Peel Off 2=Tear Off 3=Reserved s = Print Width Mode 6= 4.41" t = “Label Waiting” Flag (1=Label Waiting in Peel-Off Mode) uuuuuuuu = Labels remaining in Batch v = “Format While Printing” Flag (Always 1) www = Number of Graphic Images stored in Memory
                     */
                    Stat.isHeadupFlag    = Items[2] == "1";
                    Stat.isRibonoutFlag  = Items[3] == "1";
                    Stat.isLabelWaitFlag = Items[7] == "1";
                }
                else
                {


                }
            }
            else if (sTemp.Length == 6)//"1234,0"
            {
                int a=0;
                a++;

            }

            bRcvEnd = true;
        }

        private void Load(string _sName , SerialPort _Port , string _sPath = "")
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

            int iPortNo   ;
            Ini.Load(sTitle, "PortNo", out iPortNo ); if(iPortNo==0)iPortNo= 1 ;

            //맨처음 파일 없을때 대비.
            Ini.Save(sTitle, "PortNo   ", iPortNo   );

            _Port.PortName     = "Com" + iPortNo.ToString();
        }

        //보내는 메세지임.
        public void PrintBar(int _iYOffset , String _sTrayID, String _sFDType, String _sMatNo, String _sLotID, String _sQty, String _sBinNo, String _sDesc, String _sToff)
        {
            //~JS 백피드 
            //String strTemp = "CT~~CD,~CC^~CT~";
            //strTemp += @"^XA~TA000~JSB^LT0^MNW^MTT^PON^PMN^LH0,0^JMA^PR3,3~SD15^JUS^LRN^CI0^XZ";
            //strTemp += @"^XA^LH0,0";
            ////strTemp += @"^MMP";
            ////strTemp += @"^PW709";
            ////strTemp += @"^LL0142";
            ////strTemp += @"^LS0";
            //strTemp += "^^BY3,3,80^FT50,132^BCN,,N,N";
            //strTemp += @"^FD>:";
            //strTemp += strTray_ID+"^FS";
            //strTemp += @"^FT20,150^A0N,21,21^FH\^FDTray_ID:^FS";
            //strTemp += @"^FT95,150^A0N,21,21^FH\^FD";
            //strTemp += strTray_ID+"^FS";
            //strTemp += @"^FT22,50^A0N,21,21^FH\^FDTYPE:";
            //strTemp += strFDType+"^FS";
            //strTemp += @"^FT240,50^A0N,21,21^FH\^FDMat#:";
            //strTemp += strMatNo+"^FS";
            //strTemp += @"^FT394,50^A0N,21,21^FH\^FDLotID:";
            //strTemp += strLotID+"^FS";
            //strTemp += @"^FT581,50^A0N,21,21^FH\^FDQty:";
            //strTemp += strQty+"^FS";
            //strTemp += @"^FT295,150^A0N,21,21^FH\^FDBIN:";
            //strTemp += strBinNo+"^FS";
            //strTemp += @"^FT424,150^A0N,20,19^FH\^FDDesc.:";
            //strTemp += strDesc+"^FS";
            //strTemp += @"^PQ1,0,1,Y^XZ";
            //strTemp += "~TA";
            //strTemp += strToff;

            //^XA : 

            /*오리지날 코드 오프셑 없는 버전.
                         string sTemp = "";
            sTemp += @"^XA^LH0,45";
            sTemp += "^^BY3,3,80^FT50,82^BCN,,N,N";
            sTemp += @"^FD>:";
            sTemp += _sTrayID + "^FS";
            sTemp += @"^FT20,100^A0N,21,21^FH\^FDTray_ID:^FS";
            sTemp += @"^FT95,100^A0N,21,21^FH\^FD";
            sTemp += _sTrayID + "^FS";
            sTemp += @"^FT22,0^A0N,21,21^FH\^FDTYPE:";
            sTemp += _sFDType + "^FS";
            sTemp += @"^FT240,0^A0N,21,21^FH\^FDMat#:";
            sTemp += _sMatNo + "^FS";
            sTemp += @"^FT394,0^A0N,21,21^FH\^FDLotID:";
            sTemp += _sLotID + "^FS";
            sTemp += @"^FT581,0^A0N,21,21^FH\^FDQty:";
            sTemp += _sQty + "^FS";
            sTemp += @"^FT295,100^A0N,21,21^FH\^FDBIN:";
            sTemp += _sBinNo + "^FS";
            sTemp += @"^FT424,100^A0N,20,19^FH\^FDDesc.:";
            sTemp += _sDesc + "^FS";
            sTemp += @"^PQ1,0,1,Y^XZ";
            sTemp += "~TA";
            sTemp += _sToff;
             */


            string sTemp = "";
            sTemp += @"^XA^LH0," + _iYOffset.ToString(); //^XA=StartFormat ^LH = 0,0 좌표의 절대값을 설정.전체포지션이 이동된다.
            sTemp += "^^BY3,3,80^FT50,82^BCN,,N,N"; //^BY=최고좁은바의 넓이가3dots이고 넓은바와 좁은바의 비율이 3.0유닛이다 ^BC=Code128포멧
            sTemp += @"^FD>:";//^FD= Start of Field data for bar code.
            sTemp += _sTrayID + "^FS";
            sTemp += @"^FT20,100^A0N,21,21^FH\^FDTray_ID:^FS";  //^FT=문장의 시작위치 Y축은 밑쪽이다.  ^A0N=디폴트폰트셀렉트
            sTemp += @"^FT95,100^A0N,21,21^FH\^FD";
            sTemp += _sTrayID + "^FS";
            sTemp += @"^FT22,0^A0N,21,21^FH\^FDTYPE:";
            sTemp += _sFDType + "^FS";
            sTemp += @"^FT240,0^A0N,21,21^FH\^FDMat#:";
            sTemp += _sMatNo + "^FS";
            sTemp += @"^FT394,0^A0N,21,21^FH\^FDLotID:";
            sTemp += _sLotID + "^FS";
            sTemp += @"^FT581,0^A0N,21,21^FH\^FDQty:";
            sTemp += _sQty + "^FS";
            sTemp += @"^FT295,100^A0N,21,21^FH\^FDBIN:";
            sTemp += _sBinNo + "^FS";
            sTemp += @"^FT424,100^A0N,20,19^FH\^FDDesc.:";
            sTemp += _sDesc + "^FS";
            sTemp += @"^PQ1,0,1,Y^XZ";
            sTemp += "~TA"; // command lets you adjust the rest position of the media after a label is printed, which changes the position at which the label is torn or cut.
            sTemp += _sToff;

            SetMsg(sTemp);
        }


        public void SetMsg(string _sMsg)
        {
            sSetMsg = _sMsg;
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

        public int iPreStep = 0 ;
        public int iStep    = 10;
        CDelayTimer Delay = new CDelayTimer();
        public bool Update()
        {
            if (!Port.IsOpen) return false;

            if (iPreStep != iStep)
            {
            }

            iPreStep = iStep;

            switch (iStep)
            {
                default: 
                    if (iStep != 0)
                    {
                        //ShowMessage("Sequence Default"); //2015 JS
                    }
                    iStep = 10;
                    return false;

                case 10: 
                    String sTemp = "";
                    if (sSetMsg != "")
                    {
                        sTemp = sSetMsg; //메세지가 셑 되어 있으면 메세지 아니면 스테이터스를 보냄.
                        sSetMsg = "";
                        SendMsg(sTemp);
                        iStep=12; //프린트 명령은 응답이 없음.
                    }
                    else
                    {
                        sTemp = "~HS"   ;
                        SendMsg(sTemp);
                        iStep++;
                    }
                    return false;

                case 11: 
                    if(!IsReceiveEnd()) return false ;
                    Delay.Clear();
                    iStep++;
                    return false ;

                //위에서씀.
                case 12:
                    if (!Delay.OnDelay(100)) return false;
                    iStep = 10;
                    return true;
            }
        }
    }
}
