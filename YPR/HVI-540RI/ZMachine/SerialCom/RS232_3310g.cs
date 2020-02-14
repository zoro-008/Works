using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using COMMON;

namespace Machine
{
    public class RS232_3310g : CSerialPort
    {
        string m_dBarcordCh = "";

        //키엔스 높이 측정기.
        public RS232_3310g(int _iPortId , string _sName, string _sEndOfText="\r"):base(_iPortId , _sName, _sEndOfText)
        {
            /*초기화 상태.
            BaudRate=115200
            DataBit=8
            ParityBit=0
            StopBit=1           
            */
        }

        //public string sRecData = "";
        protected override void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            base.DataReceived(sender,e);
        
            if(sRecvMsg == "" ) return ;
        
            string sTemp = sRecvMsg;
            string dRet = sTemp;
            if (sRecvMsg.Length>=0)
            {
                m_dBarcordCh = dRet;
            }   

        }

        //현재 발주가 안나간상태여서 일단 1채널만 구현하고
        //발주및 제품이 확정되면 
        public bool SendScanOn()
        {
            m_dBarcordCh = "";

            byte[] baSendMsg = new byte[3];
            baSendMsg[0] = (byte)0x16;
            baSendMsg[1] = (byte)0x54;
            baSendMsg[2] = (byte)0x0D;
            sRecvMsg = "";
            bool bRet = SendMsg(baSendMsg);
            return bRet;
        }

        

        public bool SendScanOff()
        {

            byte[] baSendMsg = new byte[3];
            baSendMsg[0] = (byte)0x16;
            baSendMsg[1] = (byte)0x55;
            baSendMsg[2] = (byte)0x0D;

            bool bRet = SendMsg(baSendMsg);
            return bRet;
        }

        //protected override bool IsReceiveEnd()
        //{
        //    bool bRet = base.IsReceiveEnd();
        //
        //    bRet &= (m_eSendCmd == TK4S_CMD.None) ;
        //
        //    return bRet ;
        //}      

        public string GetText()
        {
            return m_dBarcordCh;
        }
    }
}
