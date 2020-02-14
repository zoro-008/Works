using COMMON;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Machine
{
    //키엔스 바코드 SR-700 시리즈
    // SR-700HA(고분해능) , SR-700(근거리) , SR-710(중거리)
                                                         
    public class RS232_SR700
    {
        private string     m_sText   = "";
        private string     m_sName   = "";
        private int        m_iPortNo = 0 ;
        private SerialPort Port      = new SerialPort() ;
        private bool       m_bRcvEnded = false ;

        //키엔스 높이 측정기.
        public RS232_SR700(int _iPortId , string _sName)
        {
            m_iPortNo = _iPortId ;
            m_sName   = _sName   ;

            Port.PortName     = "Com" + _iPortId.ToString();
            Port.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
            //Port.Encoding = System.Text.Encoding.GetEncoding("iso-8859-1");//이것이 8비트 문자 모두 가능 Ascii는 7비트라 63이상의 값은 표현 안됌.
            Port.BaudRate     = 115200; 
            Port.DataBits     = 8   ; 
            Port.Parity       = Parity  .Even;//None = 0,Odd = 1,Even = 2,Mark = 3 ,Space = 4, 
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
            return SendMsg("LON\r");
        }

        public bool Stop()
        {
            return SendMsg("LOFF\r");
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

            if(!m_sText.Contains("\r")) return ;

            m_sText = m_sText.Substring(0,m_sText.IndexOf("\r"));
            m_bRcvEnded = true;     
        }    


    }
}
