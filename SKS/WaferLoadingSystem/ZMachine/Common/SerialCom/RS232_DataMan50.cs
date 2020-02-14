using COMMON;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Machine
{
    //DataMan 5.7.0을 깔고.
    //공장 초기화 후
    //설정-빠른설정눌러서 라이브 상태로 만들고 영상 확인 하고.
    //설정-판독설정눌르고-글로벌설정으로 라디오버튼바꾸고. 
    //  통신설정에 사용자지정명령에 직렬트리거에서 사용 체크.
    //  데이터서식에서 범용 표준 설정하고 표준서식에 범용 CR/LF 체크 해서 종료 문제 세팅.
    //  표준서식 범용 범용 들어가서 전체 문자열 추가.
    //기호 설정 일반에서 써야할 코드 빼고 모두 체크해지하면 빨라짐.
    //
    //시스템에 설정저장 누름.
    // + 날리면 바코드리딩.
    // - 날리면 바코드 리딩종료.dsd
                                                         
    public class RS232_DataMan50
    {
        private string     m_sText   = "";
        private string     m_sName   = "";
        private int        m_iPortNo = 0 ;
        private SerialPort Port      = new SerialPort() ;
        private bool       m_bRcvEnded = false ;

        //키엔스 높이 측정기.
        public RS232_DataMan50(int _iPortId , string _sName)
        {
            m_iPortNo = _iPortId ;
            m_sName   = _sName   ;

            Port.PortName     = "Com" + _iPortId.ToString();
            Port.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
            //Port.Encoding = System.Text.Encoding.GetEncoding("iso-8859-1");//이것이 8비트 문자 모두 가능 Ascii는 7비트라 63이상의 값은 표현 안됌.
            Port.BaudRate     = 115200; 
            Port.DataBits     = 8   ; 
            Port.Parity       = Parity  .None;//None = 0,Odd = 1,Even = 2,Mark = 3 ,Space = 4, 
            Port.StopBits     = StopBits.One ;//None = 0,One = 1,Two  = 2,OnePointFive = 3, None은 예외상황발생함..
            Port.ReadTimeout  = (int)500;
            Port.WriteTimeout = (int)500;
            Port.Handshake    = Handshake.None ;

            try
            {
                Port.Open();    
            }
            catch
            {
                Log.ShowMessage("Error","Could not open the port " + Port.PortName);
            }
            

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

        
        //+ : 바코드리딩 스타트.
        //- : 바코드리딩 스탑.
        //클래스 상단에 설정방법 대로 설정하지 않으면 안된다.
        public bool SendMsg(string _sData)
        {
            m_bRcvEnded = false ;
            m_sText     = "";
            try
            {
                Port.Write(_sData);
            }
            catch(Exception _e)
            {
                
                return false ;
            }
            

            return true ;
        }

        public bool Read()
        {
            m_bRcvEnded = false;
            m_sText = "";
            return SendMsg("+");
        }

        public bool Stop()
        {
            return SendMsg("-");
        }

        public string GetReadingText()
        {
            return m_sText ;
        }

        public bool ReadEnded()
        {
            return m_bRcvEnded ;
        }

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int iByteCntToRead = Port.BytesToRead ;
            if (iByteCntToRead <= 0) return ;            

            m_sText +=  Port.ReadExisting();

            if(!m_sText.Contains("\r\n")) return ;

            m_sText = m_sText.Substring(0,m_sText.IndexOf("\r\n"));
            m_bRcvEnded = true;     
        }    


    }
}
