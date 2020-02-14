using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using COMMON;

namespace Machine
{
    //키엔스 LK_G5000시리즈중에 5001
    public class RS232_LK_G5001 : CSerialPort
    {
        double m_dHeightCh1 = 0.0;
        double m_dHeightCh2 = 0.0;

        //키엔스 높이 측정기.
        public RS232_LK_G5001(int _iPortId , string _sName, string _sEndOfText="\r"):base(_iPortId , _sName, _sEndOfText)
        {
            //BaudRate=9600
            //DataBit=8
            //ParityBit=0
            //StopBit=1
        }

        protected override void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            base.DataReceived(sender,e);

            if(!IsReceiveEnd()) return ;

            //MS,01,+000.073\r
            if (sRecvMsg.IndexOf("MS")>=0)
            {                
                //채널별로 분리해서 뽑는거 일단 무시..
                string sCh   = sRecvMsg.Substring(4, 1);
                int  iCh     = int.Parse(sCh);
                bool bPos    = sRecvMsg.Substring(6, 1) == "+";
                string sTemp = sRecvMsg.Substring(7, 7);

                double dRet = CConfig.StrToDoubleDef(sTemp,99); //더블형변환시에 0.005를 Parse하면 0.004999이렇게 된다.
                dRet = (int)(dRet * 1000);
                dRet = dRet / 1000;
                if(!bPos) dRet = -1 * dRet ;

                if (iCh == 1)
                {
                    m_dHeightCh1 = dRet ;
                }
                else if(iCh == 2)
                {
                    m_dHeightCh2 = dRet ;
                }
                else
                {
                    //이건 나중에 하자.
                }
            }           
        }

        //현재 발주가 안나간상태여서 일단 1채널만 구현하고
        //발주및 제품이 확정되면 
        public bool SendCheckHeight(int _iCh = 1)
        {
            //높이 측정.
            //string sTitle = string.Format("ComPort_{0:0}", iPortNo);
            string sMsg = string.Format("MS,{0:00}", _iCh);
            sMsg = sMsg + (char)0x0D ;
            bool bRet = SendMsg(sMsg);
            return bRet ;
        }

        public bool SendRezero(int _iCh = 1)
        {
            string sMsg = string.Format("VS,{0:00}", _iCh);
            sMsg = sMsg + (char)0x0D ;
            bool bRet = SendMsg(sMsg);
            return bRet ;
        }

        public double GetHeight(int _iCh = 1)
        {
            if(_iCh==1) return m_dHeightCh1 ;
            else        return m_dHeightCh2 ;
        }
    }
}
