using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using COMMON;

namespace Machine
{
    //한영넉스 4/8채널 온도컨트롤러 MC9
    //http://kor.hynux.com/product/product_view.php?lcode=01&mcode=0102&pcode=1306100022&scode=0102004

    enum MC9_CMD {
        None         = 0 ,
        RqstCrntTemp     ,
        SetTemp          ,
        SetTempAll       ,
        MAX_MC9_CMD
    };
    public class RS232_MC9 : CSerialPort
    {
        private MC9_CMD m_eSendCmd ;
        const int MAX_MC9_CH = 8 ;
        static bool bDispErr = true ;
        int [] m_iCrntTemp = new int[MAX_MC9_CH];
        int [] m_iSetTemp  = new int[MAX_MC9_CH];
        
        public RS232_MC9(int _iPortId , string _sName, string _sEndOfText="\r\n"):base(_iPortId , _sName, _sEndOfText)
        {
        }

        string GetChksum(string sData) //원래는 STX는 짤라서 보내 줘야 하는데 귀찮음.
        {
            int iDataLength = sData.Length ;
            int iChksum=0;
            char [] sHex= new char[3];
            string sReturn ;
            char cData ;
        
            for(int i=1 ; i< iDataLength ; i++) {//STX는 포함 안하고  1부터 시작이다 
                cData = sData[i] ;
                iChksum = iChksum + cData;
            }
            iChksum = iChksum % 256;
            sReturn = string.Format("{0:X}", iChksum);
            return sReturn ;
        }
        /*  iHex1=iChksum/16;
            iHex2=iChksum%16;
            //sprintf(sHex,"%X%X",iHex1,iHex2);
            sReturn = sHex ;
            //Trace("m_pRS232C_Msg","4");
            return sReturn ;
         */

        protected override void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            base.DataReceived(sender,e);

            if(!IsReceiveEnd()) return ;

            string sCheckStr;
            string sCheckSum;
            string sSum;

            sCheckStr = sRecvMsg.Substring(0, sRecvMsg.IndexOf("\r\n") - 4);
            sCheckSum = sRecvMsg.Substring(sRecvMsg.IndexOf("\r\n") - 2, 2);

            sSum = GetChksum(sCheckStr);

            if (bDispErr && sSum != sCheckSum)
            {
                Log.ShowMessage("Heater Com Err","TempCon CheckSum is Wrong!!!!");
                bDispErr = false ;
            }

            if (sRecvMsg.IndexOf("OK")>=0)
            {
                if (bDispErr)
                {
                    Log.ShowMessage("Heater Com Err","TempCon NG Received");
                    bDispErr = false;
                }
                return;
            }


            if (m_eSendCmd == MC9_CMD.None)
            {

            }
            else if (m_eSendCmd == MC9_CMD.RqstCrntTemp)
            {
                string sTempData;
                string sOneData;
                int    iTemp = 0 ;

                sTempData = sCheckStr;
                sTempData.Remove(0, sTempData.IndexOf("OK") + 3);
                for (int i = 0; i < MAX_MC9_CH; i++)
                {
                    sOneData = sTempData.Substring((5 * i), 4);
                    if (int.TryParse(sOneData, out iTemp))
                    {
                        m_iCrntTemp[i] = iTemp;
                    }
                    
                }
            }
            else if (m_eSendCmd == MC9_CMD.SetTemp)
            {
            }
            else if (m_eSendCmd == MC9_CMD.SetTempAll)
            {
            }
            else
            {
            }

            m_eSendCmd = MC9_CMD.None;
        }
        bool RqstCrntTemp()
        {
            string sSendMsg ;
        
            sSendMsg  = "01" ; //컨트롤러 아이디 한개만 쓰기에 무조건 1
            sSendMsg += "DRR"; //D레지스터에 여러개 한번에 요청 명령.
            sSendMsg += ","  ;
            sSendMsg += string.Format("{0:00}",MAX_MC9_CH); //데이터 갯수.. 8개
            for(int i = 1 ; i <= MAX_MC9_CH ; i++) {
                sSendMsg += ","  ;
                sSendMsg += string.Format("{0:0000}",i); //현제 온도 메모리 영역 0001~0008
            }
            bool bRet ;
            bRet = SendMsg(sSendMsg);
        
            m_eSendCmd = MC9_CMD.RqstCrntTemp ;
        
            return bRet ;
        }

        bool SetTemp(int _iCh, int _iTemp)
        {
            if (_iCh < 1 || _iCh > 9) return false;
            if (_iTemp < 0) _iTemp = 0; if (_iTemp > MAX_MC9_CH) _iTemp = MAX_MC9_CH;

            string sSendMsg;

            sSendMsg = "01"; //컨트롤러 아이디 한개만 쓰기에 무조건 1
            sSendMsg += "DWR"; //D레지스터에 여러개 한번에 쓰는 명령.
            sSendMsg += ",";
            sSendMsg += "01"; //데이터 갯수.. 1개
            sSendMsg += ",";
            if (_iCh < 5) sSendMsg += (1001 + (_iCh - 1) * 8).ToString(); //온도 세팅 하는 메모리 1001번 부터 시작. 1번채널 1번존  존의 개념을 모르겠음.
            else sSendMsg += (1101 + (_iCh - 5) * 8).ToString(); //온도 세팅 하는 메모리 1001번 부터 시작. 1번채널 1번존  존의 개념을 모르겠음.
            sSendMsg += ",";
            sSendMsg += string.Format("{0:XXXX}",_iTemp);//IntToHex(_iTemp, 4); //의 반대는 StrToInt
            m_iSetTemp[_iCh - 1] = _iTemp;  
            bool bRet;
            bRet = SendMsg(sSendMsg);

            m_eSendCmd = MC9_CMD.SetTemp ;

            return bRet;
        }
        bool SetTempAll(int [] _iaTemp)                               
        {       
            string sSendMsg ;
            m_iSetTemp = _iaTemp ;
            //for(int i = 0 ; i < MAX_MC9_CH ; i++){
            //    m_iSetTemp[i] = _iTemp ;
            //}
        
            sSendMsg  = "01" ; //컨트롤러 아이디 한개만 쓰기에 무조건 1
            sSendMsg += "DWR"; //D레지스터에 여러개 한번에 쓰는 명령.
            sSendMsg += ","  ;
            sSendMsg += string.Format("0:0000",MAX_MC9_CH); //데이터 갯수.. 8개
            for(int i = 1 ; i <= MAX_MC9_CH ; i++) {
                sSendMsg += ","  ;
                if(i <  5) sSendMsg += (1001 + (i - 1) * 8 ).ToString() ; //온도 세팅 하는 메모리 1001번 부터 시작. 1번채널 1번존  존의 개념을 모르겠음.
                else       sSendMsg += (1101 + (i - 5) * 8 ).ToString() ; //온도 세팅 하는 메모리 1001번 부터 시작. 1번채널 1번존  존의 개념을 모르겠음.
                sSendMsg += ","  ;
                sSendMsg += string.Format("{0:XXXX}",m_iSetTemp[i-1]);
            }
        
            bool bRet ;
            bRet = SendMsg(sSendMsg);
            
            m_eSendCmd = MC9_CMD.SetTempAll ;

            return bRet ;
        }
        public double GetCrntTemp(int _iCh)
        {
            if (_iCh < 1 || _iCh > 9) return 0.0;
            return m_iCrntTemp[_iCh];
        }

    }
    
}
