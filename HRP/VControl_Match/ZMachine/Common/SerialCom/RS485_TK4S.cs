using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using COMMON;

namespace Machine
{
    
    public class RS485_TK4S : CSerialPort
    {
        enum TK4S_CMD
        {
            None     = 0,
            RqstCrntTemp,
            SetTemp     ,
            MAX_TK4S_CMD
        };
        //모드버스 프로토콜.
        // High-Order Byte Table
        /* Table of CRC values for high.order byte */
        private static byte [] abCRCHi = {
        	0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
        	0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        	0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        	0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
        	0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        	0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
        	0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
        	0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        	0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        	0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
        	0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
        	0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        	0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
        	0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        	0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        	0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40
        };

        // Low-Order Byte Table
        /* Table of CRC values for low.order byte */
        private static byte [] abCRCLo = {
        	0x00, 0xC0, 0xC1, 0x01, 0xC3, 0x03, 0x02, 0xC2, 0xC6, 0x06, 0x07, 0xC7, 0x05, 0xC5, 0xC4, 0x04,
        	0xCC, 0x0C, 0x0D, 0xCD, 0x0F, 0xCF, 0xCE, 0x0E, 0x0A, 0xCA, 0xCB, 0x0B, 0xC9, 0x09, 0x08, 0xC8,
        	0xD8, 0x18, 0x19, 0xD9, 0x1B, 0xDB, 0xDA, 0x1A, 0x1E, 0xDE, 0xDF, 0x1F, 0xDD, 0x1D, 0x1C, 0xDC,
        	0x14, 0xD4, 0xD5, 0x15, 0xD7, 0x17, 0x16, 0xD6, 0xD2, 0x12, 0x13, 0xD3, 0x11, 0xD1, 0xD0, 0x10,
        	0xF0, 0x30, 0x31, 0xF1, 0x33, 0xF3, 0xF2, 0x32, 0x36, 0xF6, 0xF7, 0x37, 0xF5, 0x35, 0x34, 0xF4,
        	0x3C, 0xFC, 0xFD, 0x3D, 0xFF, 0x3F, 0x3E, 0xFE, 0xFA, 0x3A, 0x3B, 0xFB, 0x39, 0xF9, 0xF8, 0x38,
        	0x28, 0xE8, 0xE9, 0x29, 0xEB, 0x2B, 0x2A, 0xEA, 0xEE, 0x2E, 0x2F, 0xEF, 0x2D, 0xED, 0xEC, 0x2C,
        	0xE4, 0x24, 0x25, 0xE5, 0x27, 0xE7, 0xE6, 0x26, 0x22, 0xE2, 0xE3, 0x23, 0xE1, 0x21, 0x20, 0xE0,
        	0xA0, 0x60, 0x61, 0xA1, 0x63, 0xA3, 0xA2, 0x62, 0x66, 0xA6, 0xA7, 0x67, 0xA5, 0x65, 0x64, 0xA4,
        	0x6C, 0xAC, 0xAD, 0x6D, 0xAF, 0x6F, 0x6E, 0xAE, 0xAA, 0x6A, 0x6B, 0xAB, 0x69, 0xA9, 0xA8, 0x68,
        	0x78, 0xB8, 0xB9, 0x79, 0xBB, 0x7B, 0x7A, 0xBA, 0xBE, 0x7E, 0x7F, 0xBF, 0x7D, 0xBD, 0xBC, 0x7C,
        	0xB4, 0x74, 0x75, 0xB5, 0x77, 0xB7, 0xB6, 0x76, 0x72, 0xB2, 0xB3, 0x73, 0xB1, 0x71, 0x70, 0xB0,
        	0x50, 0x90, 0x91, 0x51, 0x93, 0x53, 0x52, 0x92, 0x96, 0x56, 0x57, 0x97, 0x55, 0x95, 0x94, 0x54,
        	0x9C, 0x5C, 0x5D, 0x9D, 0x5F, 0x9F, 0x9E, 0x5E, 0x5A, 0x9A, 0x9B, 0x5B, 0x99, 0x59, 0x58, 0x98,
        	0x88, 0x48, 0x49, 0x89, 0x4B, 0x8B, 0x8A, 0x4A, 0x4E, 0x8E, 0x8F, 0x4F, 0x8D, 0x4D, 0x4C, 0x8C,
        	0x44, 0x84, 0x85, 0x45, 0x87, 0x47, 0x46, 0x86, 0x82, 0x42, 0x43, 0x83, 0x41, 0x81, 0x80, 0x40
        }; 

        private ushort CRC16(char [] _caMsg, int _iDataLengh )
        {        
        	byte bCrcHi = 0xFF ;			/* high byte of CRC initialized */
        	byte bCrcLo = 0xFF ;			/* low byte of CRC initialized */
        	int iIndex ;					/* will index into CRC lookup table */
        
            int i = 0 ;
        	while (_iDataLengh-- > 0)		/* pass through message buffer */
        	{
        		iIndex = bCrcLo ^ _caMsg[i++];		/* calculate the CRC */
        		bCrcLo = (byte)(bCrcHi ^ abCRCHi[iIndex]);
        		bCrcHi = abCRCLo[iIndex];
        	}

            ushort usRet = 0 ;

            usRet |= (ushort)(bCrcHi << 8) ;
            usRet |= (ushort) bCrcLo ;
        
        	return usRet ;
        }     
        private ushort CRC16(byte [] _baMsg, int _iDataLengh )
        {        
        	byte bCrcHi = 0xFF ;			/* high byte of CRC initialized */
        	byte bCrcLo = 0xFF ;			/* low byte of CRC initialized */
        	int iIndex ;					/* will index into CRC lookup table */
        
            int i = 0 ;
        	while (_iDataLengh-- > 0)		/* pass through message buffer */
        	{
        		iIndex = bCrcLo ^ _baMsg[i++];		/* calculate the CRC */
        		bCrcLo = (byte)(bCrcHi ^ abCRCHi[iIndex]);
        		bCrcHi = abCRCLo[iIndex];
        	}

            ushort usRet = 0 ;

            usRet |= (ushort)(bCrcHi << 8) ;
            usRet |= (ushort) bCrcLo ;
        
        	return usRet ;
        }       

        private TK4S_CMD m_eSendCmd ;
        private int      m_iSendAdd ;
        private int []   m_iaCrntTemp ;
        private int []   m_iaSetTemp  ;

        private bool bErrMsgShow = false ;

        public RS485_TK4S(int _iPortId , string _sName, int _iConCnt , string _sEndOfText=""):base(_iPortId , _sName, _sEndOfText)
        {
            //9핀 커넥터 RX 3번 TX4번.
            /*초기화 상태.
            BaudRate=9600
            DataBit=8
            ParityBit=0
            StopBit=2             
            */
            if (_iConCnt < 1)_iConCnt=1 ;
            m_iaCrntTemp = new int[_iConCnt];
            m_iaSetTemp  = new int[_iConCnt]; 
        }      
 
        public bool SetTemp(int _iAdd , int _iTemp)
        {
            if(_iAdd < 0 || _iAdd >= m_iaCrntTemp.Length) return false;
            if(_iTemp < 0  ) _iTemp = 0 ;
            if(_iTemp > 300) _iTemp = 300 ;

            m_iaSetTemp[_iAdd] = _iTemp ;
            return true ;
        }

        public int GetSetTemp(int _iAdd)
        {
            if(_iAdd < 0 || _iAdd >= m_iaCrntTemp.Length) return 0;

            return m_iaSetTemp[_iAdd];
        }

        public int GetCrntTemp(int _iAdd)
        {
            if(_iAdd < 0 || _iAdd >= m_iaCrntTemp.Length) return 0;

            return m_iaCrntTemp[_iAdd];
        }
        
        protected override void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            base.DataReceived(sender,e);


            if(sRecvMsg.Length<5) return ;
            if (sSendedMsg.Length < 1) return;
            int  iConAdd  = sRecvMsg  [0]-1 ;
            char cRcvFunc = sRecvMsg  [1] ;//받은명령어
            char cSndFunc = sSendedMsg[1] ;//보낸명령어


            if (cRcvFunc == cSndFunc + 0x80)//예외상황은 Function에 0x80이 더해져서 리턴된다.
            {
                if (!bErrMsgShow)
                {
                    Log.ShowMessage("TK4S Heater", "Exception Response with ErrCode:" + ((int)sRecvMsg[2]).ToString());
                    bErrMsgShow = true;
                }
                else
                {
                    Log.Trace("TK4S Heater", "Exception Response with ErrCode:" + sRecvMsg[2].ToString());
                }

                m_eSendCmd = TK4S_CMD.None;
                return;
            }                


             
            if (m_eSendCmd == TK4S_CMD.RqstCrntTemp)
            {
                if (sRecvMsg.Length == 7)
                {
                    if (cRcvFunc != cSndFunc)
                    {
                        if (!bErrMsgShow)
                        {
                            Log.ShowMessage("TK4S Heater", "Send Func and Recv Func is Diffrent");
                            bErrMsgShow = true;
                        }
                        else
                        {
                            Log.Trace("TK4S Heater", "Send Func and Recv Func is Diffrent");
                        }
                    }
                    Encoding enc = Encoding.GetEncoding("iso-8859-1");
                    byte[] baRecvMsg = enc.GetBytes(sRecvMsg);
                    ushort usCRC16 = CRC16(baRecvMsg , baRecvMsg.Length-2) ;
                    byte   bCRC16Lo = (byte)(usCRC16 & 0x00FF) ;
                    byte   bCRC16Hi = (byte)((usCRC16 & 0xFF00)>>8) ;

                    //CRC16이상.
                    if (bCRC16Lo != baRecvMsg[5] || bCRC16Hi != baRecvMsg[6])
                    {
                        if (!bErrMsgShow)
                        {
                            Log.ShowMessage("TK4S Heater", "CRC16 :(" + usCRC16.ToString() + ") is Wrong");
                            bErrMsgShow = true;
                        }
                        else
                        {
                            Log.Trace("TK4S Heater", "CRC16 :(" + usCRC16.ToString() + ") is Wrong");
                        }
                    }

                    //어드레스 이상.
                    if (iConAdd < 0 || iConAdd >= m_iaCrntTemp.Length)
                    {
                        if (!bErrMsgShow)
                        {
                            Log.ShowMessage("TK4S Heater", "iConAdd:" + iConAdd.ToString() + " is RangeOver");
                            bErrMsgShow = true;
                        }
                        else
                        {
                            Log.Trace("TK4S Heater", "iConAdd:" + iConAdd.ToString() + " is RangeOver");
                        }
                    }
                    m_iaCrntTemp[iConAdd] = baRecvMsg[3] * 256 + baRecvMsg[4];
                    m_eSendCmd = TK4S_CMD.None;
                    //byte   bHiErr    = (CRC16(caRecvMsg, sRecvMsg.Length-2)&0xFF00>>8);
                }
            }
            else if(m_eSendCmd == TK4S_CMD.SetTemp)
            {
                if (sRecvMsg.Length == 8)
                {
                    if (sRecvMsg != sSendedMsg) //보낸메세지와 동일한 피드백
                    {
                        if (!bErrMsgShow)
                        {
                            Log.ShowMessage("TK4S Heater", "Send Func and Recv Func is Diffrent");
                            bErrMsgShow = true;
                        }
                        else
                        {
                            Log.Trace("TK4S Heater", "Send Func and Recv Func is Diffrent");
                        }
                    }
                
                    m_eSendCmd = TK4S_CMD.None;

                }
            }
        }

        public override bool IsReceiveEnd()
        {
            bool bRet = base.IsReceiveEnd();

            bRet &= (m_eSendCmd == TK4S_CMD.None) ;

            return bRet ;
        }      

        public bool SendSetTemp(int _iAdd, int _iData )
        //    protected bool SendSetTemp(int _iAdd, int _iData )
        {
            //4개까지 연결가능
            if(_iAdd < 0 || _iAdd >= m_iaCrntTemp.Length) return false;
            if(_iData < 0  ) _iData = 0 ;
            if(_iData > 300) _iData = 300 ;
     
            Encoding enc = Encoding.GetEncoding("iso-8859-1");
            byte [] baSendMsg = new byte[8];
            baSendMsg[0] = (byte)(_iAdd+1)   ;
            baSendMsg[1] = (byte)0x06        ;//WriteSigleRegster Function 6
            baSendMsg[2] = (byte)0x00        ;//SV설정의 Address가 0x0000의 상위바이트.
            baSendMsg[3] = (byte)0x00        ;//SV설정의 Address가 0x0000의 하위바이트.
            baSendMsg[4] = (byte)((_iData & 0xFF00)>>8); //설정값의 상위바이트.
            baSendMsg[5] = (byte) (_iData & 0x00FF);     //설정값의 하위바이트
            baSendMsg[6] = (byte)( CRC16( baSendMsg, baSendMsg.Length-2)&0x00FF    ); //체크에러의 하위바이트
            baSendMsg[7] = (byte)((CRC16( baSendMsg, baSendMsg.Length-2)&0xFF00)>>8); //체크에러의 상위바이트  //SizeOf(TWriteSigleRegster) -2);


            m_eSendCmd = TK4S_CMD.SetTemp ;
            m_iSendAdd = _iAdd ;

            SendMsg(baSendMsg);        
            return true ;
        }

        protected bool SendGetCrntTemp(int _iAdd )
        {
            if(_iAdd < 0 || _iAdd >= m_iaCrntTemp.Length) return false;

            byte [] baSendMsg = new byte[8];
            byte bTemp ;
            ushort usTemp ;
            int  iTemp ;

            baSendMsg[0] = (byte)(_iAdd+1) ;//온도제어기 어드레스. 485라 여러개 연결 할 수 있다. 이모델은 4개까지.
            baSendMsg[1] = (byte)0x04      ;//Read Input Registers Function 4
            baSendMsg[2] = (byte)0x03      ;//SV설정의 Address가 0x03E8의 상위바이트.
            baSendMsg[3] = (byte)0xE8      ;//SV설정의 Address가 0x03E8의 하위바이트.
            baSendMsg[4] = (byte)((1 & 0xFF00)>>8); //데이터갯수의 상위바이트.
            baSendMsg[5] = (byte) (1 & 0x00FF);     //데이터갯수의 하위바이트.
            usTemp = CRC16(baSendMsg, baSendMsg.Length-2);
            iTemp = (int)usTemp;
            bTemp        = (byte)(usTemp&0x00FF);
            baSendMsg[6] = bTemp; //체크에러의 상위바이트  //SizeOf(TWriteSigleRegster) -2);
            bTemp        = (byte)((usTemp&0xFF00)>>8   );
            baSendMsg[7] = bTemp; //체크에러의 하위바이트

            m_eSendCmd = TK4S_CMD.RqstCrntTemp ;
            m_iSendAdd = _iAdd ;

            string sTemp = baSendMsg.ToString() ;
        
            SendMsg(baSendMsg);        
            return true ;
        }

        int m_iPreSeqCycle = 0 ;
        int m_iSeqCycle    = 0 ;
        int m_iCrntAdd     = 0 ;//콘트롤러 많이 연결되어 있을경우 update에서 순차적을 한다.
        CDelayTimer dtUpdate   = new CDelayTimer() ;
        CDelayTimer dtTimeOver = new CDelayTimer();
        public void Update()
        {
            //return ;
            if(!dtUpdate.OnDelay(100)) return ;
            dtUpdate.Clear();

            if(dtTimeOver.OnDelay(m_iPreSeqCycle == m_iSeqCycle,1000)) {
                m_iSeqCycle = 10 ;
            }

            if (m_iPreSeqCycle != m_iSeqCycle)
            {
                //Trace("TK4S Heater",String(m_iSeqCycle).c_str());
            }

            m_iPreSeqCycle = m_iSeqCycle;

            switch (m_iSeqCycle)
            {
                default: 
                    if (m_iSeqCycle != 0)
                    {
                        Log.Trace("TK4S Heater","Step fall in Default step :" + m_iSeqCycle.ToString());
                    }        
                    m_iSeqCycle = 10;
                    break ;

                //Send ENQ
                case 10: 
                    SendSetTemp(m_iCrntAdd , m_iaSetTemp[m_iCrntAdd]);
                    m_iSeqCycle++;
                    return ;

                case 11:
                    if(!IsReceiveEnd())return ;
                    SendGetCrntTemp(m_iCrntAdd);
                    m_iSeqCycle++;
                    return ;

                case 12:
                    if(!IsReceiveEnd())return ;

                    m_iCrntAdd++;
                    if(m_iCrntAdd>=m_iaCrntTemp.Length)m_iCrntAdd=0;

                    m_iSeqCycle=10;
                    return ;
            }        
        }
    }
}


//오토닉스에 문의 남겨봤는데 (메뉴얼==병신 && 답변==병신) 이여서 
//병신자료+인터넷으로 해서 검증된부분만 되고 다른곳은 잘 모르겠다.
//모드버스방식이라는답변만 오고 샘플 재공 안된단다.
//자료 설명이 잘 안되어있어서 이해가 안됨.
//미리계산되어 있는 테이블 이용.
/*
위의 2개의 테이블을 1개로 합친것.
 unsigned short CRC16_0(unsigned char *nData, int wLength)
{
static const unsigned short wCRCTable[] = {
   0X0000, 0XC0C1, 0XC181, 0X0140, 0XC301, 0X03C0, 0X0280, 0XC241,
   0XC601, 0X06C0, 0X0780, 0XC741, 0X0500, 0XC5C1, 0XC481, 0X0440,
   0XCC01, 0X0CC0, 0X0D80, 0XCD41, 0X0F00, 0XCFC1, 0XCE81, 0X0E40,
   0X0A00, 0XCAC1, 0XCB81, 0X0B40, 0XC901, 0X09C0, 0X0880, 0XC841,
   0XD801, 0X18C0, 0X1980, 0XD941, 0X1B00, 0XDBC1, 0XDA81, 0X1A40,
   0X1E00, 0XDEC1, 0XDF81, 0X1F40, 0XDD01, 0X1DC0, 0X1C80, 0XDC41,
   0X1400, 0XD4C1, 0XD581, 0X1540, 0XD701, 0X17C0, 0X1680, 0XD641,
   0XD201, 0X12C0, 0X1380, 0XD341, 0X1100, 0XD1C1, 0XD081, 0X1040,

   0XF001, 0X30C0, 0X3180, 0XF141, 0X3300, 0XF3C1, 0XF281, 0X3240,
   0X3600, 0XF6C1, 0XF781, 0X3740, 0XF501, 0X35C0, 0X3480, 0XF441,
   0X3C00, 0XFCC1, 0XFD81, 0X3D40, 0XFF01, 0X3FC0, 0X3E80, 0XFE41,
   0XFA01, 0X3AC0, 0X3B80, 0XFB41, 0X3900, 0XF9C1, 0XF881, 0X3840,
   0X2800, 0XE8C1, 0XE981, 0X2940, 0XEB01, 0X2BC0, 0X2A80, 0XEA41,
   0XEE01, 0X2EC0, 0X2F80, 0XEF41, 0X2D00, 0XEDC1, 0XEC81, 0X2C40,
   0XE401, 0X24C0, 0X2580, 0XE541, 0X2700, 0XE7C1, 0XE681, 0X2640,
   0X2200, 0XE2C1, 0XE381, 0X2340, 0XE101, 0X21C0, 0X2080, 0XE041,

   0XA001, 0X60C0, 0X6180, 0XA141, 0X6300, 0XA3C1, 0XA281, 0X6240,
   0X6600, 0XA6C1, 0XA781, 0X6740, 0XA501, 0X65C0, 0X6480, 0XA441,
   0X6C00, 0XACC1, 0XAD81, 0X6D40, 0XAF01, 0X6FC0, 0X6E80, 0XAE41,
   0XAA01, 0X6AC0, 0X6B80, 0XAB41, 0X6900, 0XA9C1, 0XA881, 0X6840,
   0X7800, 0XB8C1, 0XB981, 0X7940, 0XBB01, 0X7BC0, 0X7A80, 0XBA41,
   0XBE01, 0X7EC0, 0X7F80, 0XBF41, 0X7D00, 0XBDC1, 0XBC81, 0X7C40,
   0XB401, 0X74C0, 0X7580, 0XB541, 0X7700, 0XB7C1, 0XB681, 0X7640,
   0X7200, 0XB2C1, 0XB381, 0X7340, 0XB101, 0X71C0, 0X7080, 0XB041,

   0X5000, 0X90C1, 0X9181, 0X5140, 0X9301, 0X53C0, 0X5280, 0X9241,
   0X9601, 0X56C0, 0X5780, 0X9741, 0X5500, 0X95C1, 0X9481, 0X5440,
   0X9C01, 0X5CC0, 0X5D80, 0X9D41, 0X5F00, 0X9FC1, 0X9E81, 0X5E40,
   0X5A00, 0X9AC1, 0X9B81, 0X5B40, 0X9901, 0X59C0, 0X5880, 0X9841,
   0X8801, 0X48C0, 0X4980, 0X8941, 0X4B00, 0X8BC1, 0X8A81, 0X4A40,
   0X4E00, 0X8EC1, 0X8F81, 0X4F40, 0X8D01, 0X4DC0, 0X4C80, 0X8C41,
   0X4400, 0X84C1, 0X8581, 0X4540, 0X8701, 0X47C0, 0X4680, 0X8641,
   0X8201, 0X42C0, 0X4380, 0X8341, 0X4100, 0X81C1, 0X8081, 0X4040 };

	unsigned char nTemp;
	unsigned short wCRCWord = 0xFFFF;

	while (wLength--)
	{
		nTemp = *nData++ ^ wCRCWord;
		wCRCWord >>= 8;
		wCRCWord  ^= wCRCTable[nTemp];
	}

	return wCRCWord;
}


 //룩업 테이블 없이 그냥 계산하는방식.
#define POLYNORMIAL	0xA001
unsigned short CRC16_1(unsigned char *puchMsg, int usDataLen) 
{
	int i;
	unsigned short crc, flag;

	crc = 0xFFFF;

	while(usDataLen--)
	{
		crc ^= *puchMsg++;

		for (i=0; i<8; i++)
		{
			flag = crc & 0x0001;
			crc >>= 1;
			if(flag) crc ^= POLYNORMIAL;
		}
	}

	return crc;
}
 */
