using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using COMMON;

namespace Machine
{
    //컨티녀스 모드 사용하여 명령 없이 계속 들어와서 안씀.
    enum KBS205_CMD{
        None     = 0,
        SetCheck = 1,
        SetHold  = 2,
        SetZero  = 3,
        MAX_KBS205_CMD
    };

    /*
     9600
     Parity even
     다이어테치는 볼트비 1.983
     */
    public class RS232_KBS205 : CSerialPort
    {
        private KBS205_CMD m_iCmdNo;
        private double m_dLoadCellData ; // Measurement _sEndOfText = (char)0x03
        public RS232_KBS205(int _iPortId , string _sName, string _sEndOfText ):base(_iPortId , _sName, _sEndOfText)
        {
            //BaudRate=9600
            //DataBit=8
            //ParityBit=2
            //StopBit=1
        }

        protected override void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            base.DataReceived(sender,e);

            if(!IsReceiveEnd()) return ;
            if(sRecvMsg.IndexOf((char)0x02)<0)return ;

            //짤려서 STX가 ETX보다 높은경우.
            if(sRecvMsg.IndexOf((char)0x03) < sRecvMsg.IndexOf((char)0x02)) {
                sRecvMsg.Remove(0,sRecvMsg.IndexOf((char)0x02));//Delete(1,m_sReadMsg.Pos((char)0x02)-1);
                return ; //ETX
            }
            
            //디버깅 걸지 마시오 계속 데이터 들어와서 뻑남...
            string sTempMsg = sRecvMsg ;
            if(sTempMsg == "")return ;
            sTempMsg = sTempMsg.Remove(0, 2);                //처음 STX와 Indicator ID삭제
            sTempMsg = sTempMsg.Remove(7, 1);//마지막 측정 데이터 외, 다른 데이터 삭제.
            sTempMsg = sTempMsg.Remove(1, 1) ; //- 0.003 이런 식으로 공백이껴있어서 뺌.

            double dTemp = 0.0;
            if(double.TryParse(sTempMsg , out dTemp)){
                m_dLoadCellData = dTemp ;
            }
            else {
                Log.Trace("LoadCellData Err", sRecvMsg);
            }

            //sRecvMsg = "";
            
            m_iCmdNo = KBS205_CMD.None ;

        }

        public override bool IsReceiveEnd()
        {
            if (sRecvMsg != "" && sRecvMsg.IndexOf((char)0x03) >= 0) //End of Text문자가 있을경우.
            {
                return true ;
            }
            return false ;
        }

        bool GetMsgEnd()
        {
            return m_iCmdNo == KBS205_CMD.None;

        }

        public bool WeightCheck()
        {
            string sSendMsg ;

            m_iCmdNo = KBS205_CMD.SetCheck;
            sRecvMsg = "";
            //sSendMsg = "0R" ; 원래 프로토콜인데 이렇게 하면 안됌.
            //SendMsg(sSendMsg);
            sSendMsg = "0" ; //Chanel
            SendMsg(sSendMsg);
            sSendMsg = "R" ;
            SendMsg(sSendMsg);
        
            
        
            return true ;
        }
        public bool SetHold(bool _bOnOff)
        {
            
            string sSendMsg;

            m_iCmdNo = KBS205_CMD.SetHold;
            sRecvMsg = "";

            //sSendMsg = "0" ; //Chanel
            //m_pRS232C -> SendData(sSendMsg.Length() , sSendMsg.c_str());
            sSendMsg = _bOnOff ? "0H" : "0L";
            SendMsg(sSendMsg);

            return true;
        }

        public bool SetZero()
        {
            
            String sSendMsg ;

            sRecvMsg = "";
            m_iCmdNo = KBS205_CMD.SetZero;
            //sSendMsg = "0" ; //Chanel
            //m_pRS232C -> SendData(sSendMsg.Length() , sSendMsg.c_str());
            sSendMsg = "0Z" ;
            SendMsg(sSendMsg);
        
        
            m_iCmdNo = KBS205_CMD.None ; //오토제로 응답없음.
        
            return true ;
        }


        public double GetLoadCellData()
        {
            return m_dLoadCellData;
        }

    }
}
