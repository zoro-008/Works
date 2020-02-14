using COMMON;
using SML;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Machine
{

    public class RS485_SeTechYD5010
    {

        private const byte STX = 0x02 ;
        private const byte ENQ = 0x05 ;
        private const byte ACK = 0x06 ;
        private const byte NAK = 0x15 ;
        private const byte EOT = 0x04 ;
        private const byte ETX = 0x03 ;

        //체결결과
        private const byte SOH = 0x01 ;

        //private const byte CR = 0x0D;
        //private const byte LF = 0x0A;

        private string      m_sText   = "";
        private string      m_sName   = "";
        private string      m_sErr    = "";
        //private int         m_iPortNo = 0 ;
        private SerialPort  Port      = new SerialPort() ;
        private bool        m_bRcvEnded = false ;

        //enum EErrCode
        //{
        //    Ok        ,
        //    TRQHighNG ,
        //    TRQLowNG  ,
        //    ANGHighNG ,
        //    ANGLowNG  ,
        //    TimeNG    ,
        //    MonitorNG ,
        //}

        xi xReady      ; 
        xi xAlarm      ; 
        xi xBusy       ; 
        xi xComplete   ; 
        xi xFastenOK   ; 
        xi xTRQHighNG  ; 
        xi xTRQLowNG   ; 
        xi xANGHighNG  ; 
        xi xANGLowNG   ; 
        xi xTimeNG     ; 
        xi xMonitorNG  ; 
        xi xCHOut1     ; 
        xi xCHOut2     ; 
        xi xCHOut4     ; 
        xi xCHOut8     ; 
        xi xCHOut16    ; 

        yi ySkip     ;
        yi yStop     ;
        yi yReset    ;
        yi yQStart   ; //가체결 + 본체결
        yi yFStart   ; //가체결
        yi ySStart   ; //본체결
        yi yOStart   ; //옵션체결
        yi yDataOut  ; //체결결과 
        yi ySVOn     ;
        yi yPJog     ;
        yi yNJog     ;
        yi yCHSel1   ;
        yi yCHSel2   ;
        yi yCHSel4   ;
        yi yCHSel8   ;
        yi yCHSel16  ;

                           
        //일고 쓰기용
        public struct CData
        {
            public bool   bWriteCom ;
            public double dData     ;
        }
        public CData sData ;

        //이것은 IO로 확인 하여 밖에선 안씀.
        public struct CErr
        {
            public bool bTMon ;
            public bool bTime ;
            public bool bLANG ;
            public bool bHANG ;
            public bool bLTOQ ;
            public bool bHTOQ ;
        }
        public CErr sErr;

        //체결결과 데이터 받아오기용
        public struct CResult
        {
            public int iChannel   ;
            public int iStage     ;
            public int iResultCode;
            public int iNgCode    ;
            public int iTorque    ;
            public int iDegree    ;
            public int iSec       ;

            public int iMaxTorque ;

        }
        public CResult sResult ;
        public RS485_SeTechYD5010(int _iPortId , string _sName)
        {
            m_sName   = _sName   ;

            Port.PortName     = "Com" + _iPortId.ToString();
            Port.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
            //Port.Encoding = System.Text.Encoding.GetEncoding("iso-8859-1");//이것이 8비트 문자 모두 가능 Ascii는 7비트라 63이상의 값은 표현 안됌.
            Port.BaudRate     = 9600; 
            Port.DataBits     = 8   ; 
            Port.Parity       = Parity  .None;//None = 0,Odd = 1,Even = 2,Mark = 3 ,Space = 4, 
            Port.StopBits     = StopBits.One ;//None = 0,One = 1,Two  = 2,OnePointFive = 3, None은 예외상황발생함..
            Port.ReadTimeout  = (int)500;
            Port.WriteTimeout = (int)500;
            Port.Handshake    = Handshake.None;

            PortOpen();

            SendWrite(37 , 0); //채널 설정을 아이오로 하게 변신.
            
            
        }

        public void SetIO(xi _xReady      , yi _ySkip    ,
                          xi _xAlarm      , yi _yStop    ,
                          xi _xBusy       , yi _yReset   ,
                          xi _xComplete   , yi _yQStart  ,
                          xi _xFastenOK   , yi _yFStart  ,
                          xi _xTRQHighNG  , yi _ySStart  ,
                          xi _xTRQLowNG   , yi _yOStart  ,
                          xi _xANGHighNG  , yi _yDataOut ,
                          xi _xANGLowNG   , yi _ySVOn    ,
                          xi _xTimeNG     , yi _yPJog    ,
                          xi _xMonitorNG  , yi _yNJog    ,
                          xi _xCHOut1     , yi _yCHSel1  ,
                          xi _xCHOut2     , yi _yCHSel2  ,
                          xi _xCHOut4     , yi _yCHSel4  ,
                          xi _xCHOut8     , yi _yCHSel8  ,
                          xi _xCHOut16    , yi _yCHSel16 )
        {
            xReady      = _xReady     ;
            xAlarm      = _xAlarm     ;
            xBusy       = _xBusy      ;
            xComplete   = _xComplete  ;
            xFastenOK   = _xFastenOK  ;
            xTRQHighNG  = _xTRQHighNG ;
            xTRQLowNG   = _xTRQLowNG  ;
            xANGHighNG  = _xANGHighNG ;
            xANGLowNG   = _xANGLowNG  ;
            xTimeNG     = _xTimeNG    ;
            xMonitorNG  = _xMonitorNG ;
            xCHOut1     = _xCHOut1    ;
            xCHOut2     = _xCHOut2    ;
            xCHOut4     = _xCHOut4    ;
            xCHOut8     = _xCHOut8    ;
            xCHOut16    = _xCHOut16   ;
                           
            ySkip          = _ySkip      ;
            yStop          = _yStop      ;
            yReset         = _yReset     ;
            yQStart        = _yQStart    ;
            yFStart        = _yFStart    ;
            ySStart        = _ySStart    ;
            yOStart        = _yOStart    ;
            yDataOut       = _yDataOut   ;
            ySVOn          = _ySVOn      ;
            yPJog          = _yPJog      ;
            yNJog          = _yNJog      ;
            yCHSel1        = _yCHSel1    ;
            yCHSel2        = _yCHSel2    ;
            yCHSel4        = _yCHSel4    ;
            yCHSel8        = _yCHSel8    ;
            yCHSel16       = _yCHSel16   ;



        }
            
            
            

        ~RS485_SeTechYD5010()
        {
            Port.Close();
        }

        public string GetErr()
        {
            return m_sErr ;
        }

        public void SetReset(yi _Reset, bool _bOn)
        {
            ML.IO_SetY(_Reset, _bOn);
        }

        private bool PortOpen()
        {
            try
            {
                Port.Open(); 
            }
            catch
            {
                Log.ShowMessage(m_sName + " COM PORT ERROR", Port.PortName + " COM Port not Exist"); 
                //MessageBox.Show(new Form(){TopMost = true}, sName + " COM PORT ERROR", Port.PortName + " COM Port Not Exist");
                return false;
            }    
            if (!Port.IsOpen)
            {
                //MessageBox.Show(new Form(){TopMost = true}, sName + " COM PORT ERROR", Port.PortName + " COM Port Not Opened");
                Log.ShowMessage(m_sName + " COM PORT ERROR", Port.PortName + " COM Port not opened");   
                return false;
            }
            return true;            
        }

        private void PortClose()
        {
            Port.Close();
            Port.Dispose();
        }

        private void Init()
        {
            lstRcv.Clear();
            m_sErr = "";
            sData = new CData();
            sErr  = new CErr ();
        }

        public bool SendMsg(string _sData)
        {
            Init();

            _sData += "\r\n";
            m_bRcvEnded = false ;
            m_sText     = "";

            //Log.TraceListView(_sData);

            byte[] ByteData = Encoding.ASCII.GetBytes(_sData);

            try
            {
                Port.Write(_sData);
            }
            catch(Exception _e)
            {
                m_sErr = "SendMsgErr - " +_e.Message ;
                //Log.TraceListView(m_sErr);
                return false ;
            }
            return true ;
        }

        public bool SendMsg(byte [] _bDatas)
        {
            //_sData += "\r\n";
            Init();

            m_bRcvEnded = false;
            m_sText = "";

            string sMsg = "";
            
            for (int i = 0 ; i < _bDatas.Length ; i++)
            {
                sMsg += _bDatas[i].ToString("X2") + " ";
            }

            //Log.TraceListView(sMsg.ToString());

            byte[] ByteData = _bDatas;

            try
            {
                Port.Write(ByteData , 0 , ByteData.Length);
            }
            catch (Exception _e)
            {
                m_sErr = "SendMsgErr - " + _e.Message;
                //Log.TraceListView(m_sErr);
                return false;
            }
            return true;
        }

        //개별 메뉴 변수 읽기
        public bool SendRead (int _iCode)
        {
            int iCode = _iCode;
            List<byte> lstCmd = new List<byte>();
            
            lstCmd.Add(ENQ );
            lstCmd.Add(0x30); //국번
            lstCmd.Add(0x31); //국번 1로 사용
            lstCmd.Add(0x52); //명령어 52-RSS, 72-rSS(테일(EOT)뒤에 프레임 체크)
            lstCmd.Add(0x53); //명령어 고정
            lstCmd.Add(0x53); //명령어 고정
            lstCmd.Add(0x30); //블록수
            lstCmd.Add(0x31); //블록수
            lstCmd.Add(0x30); //변수길이
            lstCmd.Add(0x36); //변수길이
            lstCmd.Add(0x25); //변수코드 형식 %
            lstCmd.Add(0x4D); //변수코드 형식 M
            lstCmd.Add(0x44); //변수코드 형식 D

            //lstCmd.Add(0x31); //변수코드 형식 D
            //lstCmd.Add(0x32); //변수코드 형식 D
            //lstCmd.Add(0x39); //변수코드 형식 D

            //string sHex = iCode.ToString("X3");
            
            byte bByte1 = (byte)((int)0x30 + iCode / 100);//(byte)sHex[0] ; //
            byte bByte2 = (byte)((int)0x30 + iCode % 100 / 10);//(byte)sHex[1] ; //
            byte bByte3 = (byte)((int)0x30 + iCode %  10);//(byte)sHex[2] ; //

            lstCmd.Add(bByte1);//bytes[0]); //변수코드 번호 30,30,31 -> 1
            lstCmd.Add(bByte2);//bytes[1]); //변수코드 번호
            lstCmd.Add(bByte3);//bytes[2]); //변수코드 번호
            
            lstCmd.Add(EOT ); //테일 EOT
            
            SendMsg(lstCmd.ToArray());
            return true;
        }

        //개별 메뉴 쓰기
        public bool SendWrite(int _iCode, int _iData)
        {
            int    iCode = _iCode;
            int    iData = _iData;
            List<byte> lstCmd = new List<byte>();
            
            lstCmd.Add(ENQ );
            lstCmd.Add(0x30); //국번
            lstCmd.Add(0x31); //국번 1로 사용
            lstCmd.Add(0x57); //명령어 57-RSS, 77-rSS(테일(EOT)뒤에 프레임 체크)
            lstCmd.Add(0x53); //명령어 고정
            lstCmd.Add(0x53); //명령어 고정
            lstCmd.Add(0x30); //블록수
            lstCmd.Add(0x31); //블록수
            lstCmd.Add(0x30); //변수길이
            lstCmd.Add(0x36); //변수길이
            lstCmd.Add(0x25); //변수코드 형식 %
            lstCmd.Add(0x4D); //변수코드 형식 M
            lstCmd.Add(0x44); //변수코드 형식 D

            string sHex = iCode.ToString("X3");
            byte bByte1 = (byte)((int)0x30 + iCode / 100);     //(byte)sHex[0] ; //
            byte bByte2 = (byte)((int)0x30 + iCode % 100 / 10);//(byte)sHex[1] ; //
            byte bByte3 = (byte)((int)0x30 + iCode % 10);      //(byte)sHex[2] ; //
            //byte bByte11 = (byte)Convert.ToInt32(sHex[0].ToString(),16) ; //
            //byte bByte22 = (byte)sHex[1] ; //
            //byte bByte33 = (byte)sHex[2] ; //

            lstCmd.Add(bByte1);//bytes[0]); //변수코드 번호 30,30,31 -> 1
            lstCmd.Add(bByte2);//bytes[1]); //변수코드 번호
            lstCmd.Add(bByte3);//bytes[2]); //변수코드 번호

                   sHex = _iData.ToString("X8");
                 bByte1 = (byte)sHex[0] ;
                 bByte2 = (byte)sHex[1] ;
                 bByte3 = (byte)sHex[2] ;
            byte bByte4 = (byte)sHex[3] ;
            byte bByte5 = (byte)sHex[4] ;
            byte bByte6 = (byte)sHex[5] ;
            byte bByte7 = (byte)sHex[6] ;
            byte bByte8 = (byte)sHex[7] ;

            //lstCmd.Add(0x30);////변수코드 데이터
            //lstCmd.Add(0x30);////변수코드 데이터
            //lstCmd.Add(0x30);////변수코드 데이터
            //lstCmd.Add(0x30);////변수코드 데이터
            //lstCmd.Add(0x30);////변수코드 데이터
            //lstCmd.Add(0x30);////변수코드 데이터
            //lstCmd.Add(0x30);////변수코드 데이터
            //lstCmd.Add(0x30);////변수코드 데이터
            //lstCmd.Add(0x32);////변수코드 데이터
            //lstCmd.Add(0x31);////변수코드 데이터
            lstCmd.Add(bByte1);////변수코드 데이터
            lstCmd.Add(bByte2);////변수코드 데이터
            lstCmd.Add(bByte3);////변수코드 데이터
            lstCmd.Add(bByte4);////변수코드 데이터
            lstCmd.Add(bByte5);////변수코드 데이터
            lstCmd.Add(bByte6);////변수코드 데이터
            lstCmd.Add(bByte7);////변수코드 데이터
            lstCmd.Add(bByte8);////변수코드 데이터
            
            lstCmd.Add(EOT ); //테일 EOT
            
            return SendMsg(lstCmd.ToArray());       
        }

        //체결결과 요구
        //이건 안쓰고 IO로 함.
        public bool SendReport() //Request
        {
            sResult = new CResult();
            List<byte> lstCmd = new List<byte>();
        
            lstCmd.Add( SOH); //SOH
            lstCmd.Add(0x30); //수신국번 : 로컬 너트런너 국번으로, 1 ~ 31까지 등록하십시오.
            lstCmd.Add(0x31); //수신국번
            lstCmd.Add(0x30); //송신국번
            lstCmd.Add(0x30); //송신국번 : 데이터를 수집하는 기기의 국번으로 일반적으로 0을 사용합니다.
            lstCmd.Add(0x65); //명령코드 45 , 65
            lstCmd.Add(0x4D); //명령코드
            lstCmd.Add(0x30); //명령코드
            lstCmd.Add(0x32); //명령코드
            lstCmd.Add(EOT ); //테일
        
            return SendMsg(lstCmd.ToArray());
                     
        }
        
        public string GetReadingText()
        {
            return m_sText ;
        }

        public bool ReadEnded()
        {
            return m_bRcvEnded ;
        }

        List<byte> lstRcv = new List<byte>();
        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //Log.TraceListView("0");
            int iByteCntToRead = Port.BytesToRead ;
            if (iByteCntToRead <= 0) return ;            

            byte[] ByteRead = new byte[iByteCntToRead];
            int iReadCount = Port.Read(ByteRead, 0, iByteCntToRead);

            lstRcv.AddRange(ByteRead);

            string sMsg1 = "";

            //for (int i = 0; i < lstRcv.Count; i++)
            //{
            //    sMsg1 += lstRcv[i].ToString("X2") + " ";
            //}

            //Log.TraceListView(sMsg1.ToString());

            //체결결과 테일 EOT 이외 ETX
            if (lstRcv.LastIndexOf(ETX) < 0 && lstRcv.LastIndexOf(EOT) < 0 ) return;
            //Log.TraceListView("------------------------------------");
            

            //에러일시
            if (lstRcv[0] == 21) //ACK 15 ERROR
            {
                try
                {
                    List<byte> lstByte = new List<byte>();
                    for (int i = 0; i < 4; i++) lstByte.Add(lstRcv[6+i]);
                    string sData = System.Text.Encoding.ASCII.GetString(lstByte.ToArray());
                    m_sErr = sData ;

                    if(sData == "0101") m_sErr = "요구프레임의 헤드가 ENQ 가 아닙니다                               " ;
                    if(sData == "0102") m_sErr = "지정된 명령어가 아닙니다                                          " ;
                    if(sData == "0103") m_sErr = "블록 수가 4 개를 초과했습니다                                     " ;
                    if(sData == "0104") m_sErr = "변수 형식이 [[%M 으로 시작하지 않습니다                           " ;
                    if(sData == "0105") m_sErr = "변수 형식이 W, D, B, X 가 아닙니다                                " ;
                    if(sData == "0106") m_sErr = "변수 번호가 지정된 범위를 벗어났습니다                            " ;
                    if(sData == "0107") m_sErr = "요구 프레임의 테일이 EOT 가 아닙니다                              " ;
                    if(sData == "0108") m_sErr = "Checksum 이 틀립니다                                              " ;
                    if(sData == "0109") m_sErr = "연속읽기 및 쓰기에서 최대 바이트 수 (256 Bytes) 를 초과했습니다   " ;
                    if(sData == "010A") m_sErr = "최대 송신 버퍼 수를 초과했습니다                                  " ;
                    if(sData == "010B") m_sErr = "변수 길이가 16 자를 초과했습니다                                  " ;
                    if(sData == "0201") m_sErr = "데이터 쓰기에서 전송된 데이터가 설정범위를 벗어났습니다           " ;
                    if(sData == "0202") m_sErr = "서보 ON 시 쓰기 금지 데이터 입니다                                " ;
                    if(sData == "0203") m_sErr = "모터 파라미터 쓰기 금지입니다 ( 모터 ID 가 0 이 아닌경우입니다    " ;     
                    if(sData == "0204") m_sErr = "등록되지 않은 모터 아이디입니다                                   " ;

                    Log.Trace(m_sErr);
                }
                catch(Exception _e)
                {
                    m_sErr = _e.ToString();
                    Log.Trace(_e.ToString());
                }
            }

            if(lstRcv[0] == ACK && lstRcv.Count == 19) //ACK 응답 프레임 (읽기)
            {
                try
                {
                    List<byte> lstByte = new List<byte>();
                    for (int i = 0; i < 8; i++) lstByte.Add(lstRcv[10+i]);
                    string sRead = System.Text.Encoding.ASCII.GetString(lstByte.ToArray());
                    int    iRead = Convert.ToInt32(sRead,16);
                    //sData.dData = (double)iRead /10.0 ;
                    //데이터 중에 정수형인 것도 있고 소수점 둘째자리까지 표기되는 애들 있어서
                    //나누기 10하면 엉뚱한 값 날라와서 없앤다. 진섭
                    sData.dData = (double)iRead;
                    //Log.TraceListView("개별 파라미터 읽기 : " + sData.dData.ToString());

                }
                catch(Exception _e)
                {
                    m_sErr = _e.ToString();
                    Log.Trace(_e.ToString());
                }
            }

            if(lstRcv[0] == ACK && lstRcv.Count == 7) //ACK 응답 프레임 (쓰기)
            {
                sData.bWriteCom = true;
            }

            //체결결과 응답 프레임 (정상)
            if(lstRcv[0] == SOH ) //ACK 응답 프레임 (쓰기)
            {
                try
                {
                    if(lstRcv[5] == 0x4E || lstRcv[5] == 0x6E) { m_sErr = "체결결과 응답 에러 발생"; return; } //에러 오류코드는 모르겟다
                    //데이터 구성 8자리 단위로 7개가 아래의 순서대로 구성됩니다.
                    List<byte> lstByte = new List<byte>();
                    for (int i = 0; i < 8; i++) lstByte.Add(lstRcv[9     + i]);
                    string sData1 = System.Text.Encoding.ASCII.GetString(lstByte.ToArray());
                    int iRead1 = Convert.ToInt32(sData1, 16);
                    sResult.iChannel    = iRead1;  

                    lstByte = new List<byte>();
                    for (int i = 0; i < 8; i++) lstByte.Add(lstRcv[9+8*1 + i]);
                    string sData2 = System.Text.Encoding.ASCII.GetString(lstByte.ToArray());
                    int iRead2 = Convert.ToInt32(sData2, 16);
                    sResult.iStage = iRead2;

                    lstByte = new List<byte>();
                    for (int i = 0; i < 8; i++) lstByte.Add(lstRcv[9+8*2 + i]);
                    string sData3 = System.Text.Encoding.ASCII.GetString(lstByte.ToArray());
                    int iRead3 = Convert.ToInt32(sData3, 16);
                    sResult.iResultCode = iRead3;

                    lstByte = new List<byte>();
                    for (int i = 0; i < 8; i++) lstByte.Add(lstRcv[9+8*3 + i]);
                    string sData4 = System.Text.Encoding.ASCII.GetString(lstByte.ToArray());
                    int iRead4 = Convert.ToInt32(sData4, 16); //Error
                    sResult.iNgCode = iRead4;

                    lstByte = new List<byte>();
                    for (int i = 0; i < 8; i++) lstByte.Add(lstRcv[9+8*4 + i]);
                    string sData5 = System.Text.Encoding.ASCII.GetString(lstByte.ToArray());
                    int iRead5 = Convert.ToInt32(sData5, 16);
                    sResult.iTorque = iRead5;

                    lstByte = new List<byte>();
                    for (int i = 0; i < 8; i++) lstByte.Add(lstRcv[9+8*5 + i]);
                    string sData6 = System.Text.Encoding.ASCII.GetString(lstByte.ToArray());
                    int iRead6 = Convert.ToInt32(sData6, 16);
                    sResult.iDegree = iRead6;

                    lstByte = new List<byte>();
                    for (int i = 0; i < 8; i++) lstByte.Add(lstRcv[9+8*6 + i]);
                    string sData7 = System.Text.Encoding.ASCII.GetString(lstByte.ToArray());
                    int iRead7 = Convert.ToInt32(sData7, 16);
                    sResult.iSec = iRead7;

                    sErr.bTMon = (iRead4 >> 5 & 0x01) == 0x01;
                    sErr.bTime = (iRead4 >> 4 & 0x01) == 0x01;
                    sErr.bLANG = (iRead4 >> 3 & 0x01) == 0x01;
                    sErr.bHANG = (iRead4 >> 2 & 0x01) == 0x01;
                    sErr.bLTOQ = (iRead4 >> 1 & 0x01) == 0x01;
                    sErr.bHTOQ = (iRead4 >> 0 & 0x01) == 0x01;

                    //List<byte> lstByte = new List<byte>();
                    //for (int i = 0; i < 8; i++) lstByte.Add(lstRcv[10 + i]);
                    //string sRead = System.Text.Encoding.ASCII.GetString(lstByte.ToArray());
                    //int iRead = Convert.ToInt32(sRead, 16);
                    //sData.dData = (double)iRead / 10.0;

                    //TODO :: 이건 받아봐야 알겟다
                }
                catch (Exception _e)
                {
                    m_sErr = _e.ToString();
                    Log.Trace(_e.ToString());
                }
            }



            //string sMsg = "";
            //
            //for (int i = 0; i < lstRcv.Count; i++)
            //{
            //    sMsg += lstRcv[i].ToString("X2") + " ";
            //}
            //
            //Log.TraceListView(sMsg.ToString());

            //string sReceved = "";
            //sReceved = Port.ReadExisting();

            //m_sText += Encoding.ASCII.GetString(ByteRead);
        

            //if(!m_sText.Contains("\r\n")) return ;
            //Log.TraceListView(m_sText);
            //m_sText = m_sText.Substring(0,m_sText.IndexOf("\r\n"));
            m_bRcvEnded = true;     
        }

        //채결하고 결과값 확인 할때.
        public CResult GetResult()
        {
            return sResult;
        }

        public void InitDO()
        {
            ML.IO_SetY(ySkip   , false);
            ML.IO_SetY(yStop   , false);
            ML.IO_SetY(yReset  , false);
            ML.IO_SetY(yQStart , false);
            ML.IO_SetY(yFStart , false);
            ML.IO_SetY(ySStart , false);
            ML.IO_SetY(yOStart , false);
            ML.IO_SetY(yDataOut, false);
            ML.IO_SetY(ySVOn   , false);
            ML.IO_SetY(yPJog   , false);
            ML.IO_SetY(yNJog   , false);
        }

        public void SetStop(bool _bOn)
        {
            if(_bOn) InitDO();
            ML.IO_SetY(yStop, _bOn);
        }

        public void SetCh(int _iCh)
        {
            if(_iCh < 1 || _iCh > 31) return ;

            byte bCh = (byte)_iCh ;

            ML.IO_SetY(yCHSel1 , (bCh & 0b00001) == 0b00001);
            ML.IO_SetY(yCHSel2 , (bCh & 0b00010) == 0b00010);
            ML.IO_SetY(yCHSel4 , (bCh & 0b00100) == 0b00100);
            ML.IO_SetY(yCHSel8 , (bCh & 0b01000) == 0b01000);
            ML.IO_SetY(yCHSel16, (bCh & 0b10000) == 0b10000);
        }

        public int GetCh()
        {
            byte bCh = 0 ;
            if (ML.IO_GetX(xCHOut1)) bCh += 0b00001 ;
            if (ML.IO_GetX(xCHOut2)) bCh += 0b00010 ;
            if (ML.IO_GetX(xCHOut4)) bCh += 0b00100 ;
            if (ML.IO_GetX(xCHOut8)) bCh += 0b01000 ;
            if (ML.IO_GetX(xCHOut16)) bCh += 0b10000 ;

            return (int) bCh ;

        }

        //초기화시에 앵글을 받아놨다가
        //리턴 트루 되면 GetErr()를 해서 확인해봤을때 "" 이면 OK 메세지 있으면 에러임.
        int iCycle = 0 ; 
        int iPreCycle = 0 ;
        CDelayTimer m_tmDelay = new CDelayTimer();
        CDelayTimer m_tmCycle = new CDelayTimer();
        int iTrgAng = 0 ;
        int iRvsAng = 0 ;
        int iMinTq  = 0 ;
        int iMaxTq  = 0 ;
        bool bFrstDirCW = true ;
        public bool CycleFirstStage(bool _bInit = false  , bool _bFrstDirCW = true )
        {
            if(_bInit)
            {
                iCycle = 10 ; 
                iPreCycle = 0 ;
                bFrstDirCW = _bFrstDirCW;
                m_sErr = "";
                InitDO();
            }
            string sTemp;
            if (m_tmCycle.OnDelay(iCycle != 0 && iCycle == iPreCycle && !ML.IO_GetX(xBusy) && !OM.MstOptn.bDebugMode, 2000))
            {
                
                sTemp = string.Format("Time Out iCycle={0:00}", iCycle);
                sTemp = "NutRunner " + MethodBase.GetCurrentMethod().Name +" "+ sTemp;
                m_sErr = sTemp + " " + m_sErr;
                //IO_SetY(yi.RAIL_FeedingAC3,false);
                InitDO();
                Log.TraceListView(sTemp);
                return true;
            }

            if (iCycle != iPreCycle)
            {
                sTemp = string.Format("Cycle iCycle={0:00}", iCycle);
                Log.Trace(sTemp);
            }

            iPreCycle = iCycle;

            switch (iCycle)
            {

                default:
                    sTemp = MethodBase.GetCurrentMethod().Name + string.Format(" Default Clear iCycle={0:00}", iCycle);
                    return true;

                case 10:

                    iCycle++;
                    return false;

                case 11:
                    if (bFrstDirCW)//양수이면(체결해야할 놈이 크면)
                    {
                        //에러나면 안에서 에러메세지 세팅 하고 리턴함.
                        if (!SendWrite(129, 1)) return true;//시계방향
                    }
                    else 
                    {
                        //에러나면 리턴.
                        if (!SendWrite(129, 0)) return true;//반시계방향
                    }

                    iCycle++;
                    return false ;

                case 12:
                    if(!m_bRcvEnded)return false ;
                    if (m_sErr != "")
                    {
                        return true;
                    }

                    iCycle++;
                    return false;

                case 13:
                    //if (!m_bRcvEnded) return false;
                    //
                    //
                    //
                    //
                    iCycle++;
                    return false ;

                case 14:
                    if (!ML.IO_GetX(xReady)) return false;
                    ML.IO_SetY(yFStart, true);

                    iCycle++;
                    return false;

                case 15:
                    if(!ML.IO_GetX(xBusy)) return false ;
                    ML.IO_SetY(yFStart, false);
                    iCycle++;
                    return false;

                case 16:
                    if (!ML.IO_GetX(xComplete)) return false;
                    //m_tmDelay.Clear();
                    iCycle++;
                    return false;

                case 17:
                    //if(!m_tmDelay.OnDelay(2000)) return false;

                         if (ML.IO_GetX(xTRQHighNG)) m_sErr = "TRQHighNG";
                    else if (ML.IO_GetX(xTRQLowNG )) m_sErr = "TRQLowNG ";
                    else if (ML.IO_GetX(xANGHighNG)) m_sErr = "ANGHighNG";
                    else if (ML.IO_GetX(xANGLowNG )) m_sErr = "ANGLowNG ";
                    else if (ML.IO_GetX(xTimeNG   )) m_sErr = "TimeNG   ";
                    else if (ML.IO_GetX(xMonitorNG)) m_sErr = "MonitorNG";
                    else if (ML.IO_GetX(xFastenOK )) m_sErr = "";

                    iCycle = 0 ;
                    return true ;

            }
        }

        //초기화시에 앵글을 받아놨다가
        //리턴 트루 되면 GetErr()를 해서 확인해봤을때 "" 이면 OK 메세지 있으면 에러임.
        CResult rst;
        string  CycleSecondStageErr;
        public bool CycleSecondStage(bool _bInit = false  , int _iTrgAng = 0 , int _iRvsAng = 0 , int _iMinTq = 0, int _iMaxTq = 0)
        {
            if(_bInit)
            {
                iCycle = 10 ; 
                iPreCycle = 0 ;
                if (_iTrgAng > 0) iTrgAng = _iTrgAng + _iRvsAng ;
                else              iTrgAng = _iTrgAng - _iRvsAng ;
                //iTrgAng = _iTrgAng ;
                iRvsAng = _iRvsAng ;

                iMinTq = _iMinTq ;
                iMaxTq = _iMaxTq ;
                m_sErr = "";
                InitDO();
            }
            string sTemp;
            if (m_tmCycle.OnDelay(iCycle != 0 && iCycle == iPreCycle && !ML.IO_GetX(xBusy) && !OM.MstOptn.bDebugMode, 5000))
            {
                sTemp = string.Format("Time Out iCycle={0:00}", iCycle);
                sTemp = "NutRunner " + MethodBase.GetCurrentMethod().Name +" "+ sTemp;
                m_sErr = sTemp + " " + m_sErr;
                //IO_SetY(yi.RAIL_FeedingAC3,false);
                Log.TraceListView(sTemp);
                InitDO();
                return true;
            }

            if (iCycle != iPreCycle)
            {
                sTemp = string.Format("Cycle iCycle={0:00}", iCycle);
                Log.Trace(sTemp);
            }

            iPreCycle = iCycle;

            switch (iCycle)
            {

                default:
                    sTemp = MethodBase.GetCurrentMethod().Name + string.Format(" Default Clear iCycle={0:00}", iCycle);
                    return true;

                case 10:
                    CycleSecondStageErr = "";
                    //m_tmDelay.Clear();
                    iCycle++;
                    return false;

                case 11:
                    //if(!m_tmDelay.OnDelay(1000)) return false;
                    if (iTrgAng > 0)//양수이면(체결해야할 놈이 크면)
                    {
                        if (!SendWrite(129, 1)) return true;//시계방향
                    }
                    else //if (iTrgAng <= 0)
                    {
                        if (!SendWrite(129, 0)) return true;//반시계방향
                    }
                    iCycle++;
                    return false ;

                case 12:
                    if(!m_bRcvEnded)return false ;
                    if (m_sErr != "")
                    {
                        return true;
                    }

                    int iTrg = Math.Abs(iTrgAng);
                    if (!SendWrite(149, iTrg)) return true;//회전량

                    iCycle++;
                    return false;

                case 13:
                    if (!m_bRcvEnded) return false;
                    if (m_sErr != "")
                    {
                        return true;
                    }
                    int iRvs = Math.Abs(iRvsAng) * 10; //이거 죄송 이게 계산할때 10곱한건데 넣을때는 100곱해야 해서
                    if (!SendWrite(159, iRvs)) return true;//회전량

                    iCycle++;
                    return false;

                case 14:
                    if (!m_bRcvEnded) return false;
                    if (m_sErr != "")
                    {
                        return true;
                    }
                    int iMin = Math.Abs(iMinTq) ;
                    if (!SendWrite(147, iMin)) return true;

                    iCycle++;
                    return false;

                case 15:
                    if (!m_bRcvEnded) return false;
                    if (m_sErr != "")
                    {
                        return true;
                    }
                    int iMax = Math.Abs(iMaxTq) ;
                    if (!SendWrite(146, iMax)) return true;

                    iCycle++;
                    return false;


                case 16:
                    if (!m_bRcvEnded) return false;
                    if (m_sErr != "")
                    {
                        return true;
                    }

                    iCycle=20;
                    return false ;

                case 20:
                    if (!ML.IO_GetX(xReady)) return false;
                    ML.IO_SetY(ySStart, true);

                    iCycle++;
                    return false;

                case 21:
                    if(!ML.IO_GetX(xBusy)) return false ;
                    ML.IO_SetY(ySStart, false);
                    iCycle++;
                    return false;

                case 22:
                    if (!ML.IO_GetX(xComplete)) return false; 
                    //m_tmDelay.Clear();
                    iCycle++;
                    return false;

                case 23: //왠지 모르겠는데 감속 구간에 컴플리트 들어와서 통신 바로 하면 삽질 하는듯.
                    //if (!m_tmDelay.OnDelay(2000)) return false;
                    iCycle++;
                    return false;

                case 24:
                         if (ML.IO_GetX(xTRQHighNG)) CycleSecondStageErr = "TRQHighNG";
                    else if (ML.IO_GetX(xTRQLowNG )) CycleSecondStageErr = "TRQLowNG ";
                    else if (ML.IO_GetX(xANGHighNG)) CycleSecondStageErr = "ANGHighNG";
                    else if (ML.IO_GetX(xANGLowNG )) CycleSecondStageErr = "ANGLowNG ";
                    else if (ML.IO_GetX(xTimeNG   )) CycleSecondStageErr = "TimeNG   ";
                    else if (ML.IO_GetX(xMonitorNG)) CycleSecondStageErr = "MonitorNG";
                    else if (ML.IO_GetX(xFastenOK )) CycleSecondStageErr = "";

                    SendReport();

                    //if (m_sErr != "") 
                    //{
                    //    //체결중 에러 있으면 리턴.
                    //    return true ;
                    //}

                    //데이터 요청.
                    //m_bRcvEnded = false;
                    //ML.IO_SetY(yDataOut, true);   
                    

                    iCycle++;
                    return false ;

                case 25:
                    if(!m_bRcvEnded) return false ;
                    //sResult = new CResult();
                    sResult = GetResult();
                    //if (m_sErr != "")
                    //{
                    //    //체결중 에러 있으면 리턴.
                    //    return true;
                    //}

                    SEQ.rsNut.SendRead(10);//최대토그 얻어오려고 소수점 둘째자리
                    iCycle++;
                    return false;

                case 26:
                    if (!m_bRcvEnded) return false;
                    sResult.iMaxTorque = (int)sData.dData;
                    //if (!SEQ.rsNut.ReadEnded()) return false;
                    if (m_sErr != "" || CycleSecondStageErr != "")
                    {
                        m_sErr += "최소토크 " + (OM.DevInfo.dMinTq / 10.0).ToString("N1") + " 체결토크: " + ((double)sResult.iTorque/1000.0).ToString("N3") + " 최대토크: " + ((double)sResult.iMaxTorque/1000.0).ToString("N3");
                        //체결중 에러 있으면 리턴.
                        return true;
                    }



                    //ML.IO_SetY(yDataOut, false);
                    iCycle = 0 ;
                    return true ;

            }
        }

        
        

    }

    
}

