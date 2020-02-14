using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;

using MotionInterface;
using COMMON;
using System.Text;
using System.Threading;
using System.Runtime.Remoting.Contexts;

namespace MotionUI
{
    public class CMessenger
    {
        public CMessenger()
        {

        }
        SerialPort siPort = new SerialPort() ;

        //센드메세지.
        //원래 관리를 여기서 할라 했는데 그냥 여기는 중계만 하고 각축에서 관리 하는것이 나을듯 해서 주석.
        //    //보낼 메세지를 모았다가 한번에 보냄.
        //    //Queue<string> MessageQue        = new Queue<string>();
        //    //보낼 메세지 보내면 답장 올때까지 여기에 보관하여 모든 답장 체크.
        //    //Queue<string> SendingMessageQue = new Queue<string>();
        //    //라킹용 오브젝트. 큐를 에딩하고있을때 센딩 하면 안됨.
        //    //센딩하고 있을때 에딩 해도 안됌.
        //    //쓰레드 1개에서 보통 쓸거라 상관은 없을듯 한데 UI쪽에서 들어올수가 있어서 만들어 놓는다.
        //    //readonly object MessageQueLock = new object();

        //존나 이상하게 센딩프로토콜은 아스키로 되어 있고  리시브는 바이트기준으로 만들어놔서 바이트 비교 쉽게 string 으로 안받고 byte배열로 받음.
        Queue<byte> AckQue = new Queue<byte>();

        //각축별 콜백함수 등록 딕셔너리.
        Dictionary<uint,DataRcvCallback> diCallback = new Dictionary<uint, DataRcvCallback>();

        public string Error ;

        public bool IsPortOpen(){ return siPort.IsOpen ;}
        public bool PortOpen(uint _uiPortNo)
        {
            //_uiPortNo =13 ;
            siPort.BaudRate = 115200;
            siPort.DataBits = 8;
            siPort.Parity = Parity.None;
            siPort.StopBits = StopBits.One;
            siPort.ReadTimeout = 500;
            siPort.WriteTimeout = 500;
            siPort.PortName = "COM" + _uiPortNo.ToString();

            siPort.DataReceived += new SerialDataReceivedEventHandler(DataReceived);

            try
            {
                siPort.Open();
            }
            catch (Exception _e)
            {
                Error = _e.Message ;
                return false ;
            }

            return IsPortOpen() ;
        }
        public void PortClose()
        {
            siPort.Close();
        }

        //각축마다 콜백을 등록하여 명령어가 응답 받았는지 확인.
        public delegate void DataRcvCallback(byte [] _byMsg);
        public void RegistDataRcvCallback(uint _iId , DataRcvCallback _pfCallback)
        {
            diCallback[_iId] = _pfCallback ;
        }

        public bool SendMessage(string _sMsg)
        {
            try
            {
                siPort.Write(_sMsg);
            }
            catch(Exception _e)
            {
                Error = _e.Message ;
                return false ;
            }
            return true ;
        }

        private void DataReceived(object Sender, SerialDataReceivedEventArgs e)
        {
            int intRecSize = siPort.BytesToRead;

            if (intRecSize != 0)
            {
                //iTestSet = 1;
                byte[] RecvMsg = new byte[intRecSize];
                siPort.Read(RecvMsg, 0, intRecSize);                

                foreach(byte b in RecvMsg)
                {
                    AckQue.Enqueue(b);
                }

                //응답 종료 문자 FF(255) 없으면 리턴.
                if(!AckQue.Contains(0xFF)) return  ; 
                
                //한패킷용.
                Queue<byte> AckPacket  = new Queue<byte>();                
                byte bb ;
                int iCnt ; 
                while (AckQue.Contains(0xFF))//한번에 응답이 여러게 들어오기도해서.
                {
                    iCnt = AckQue.Count ;
                    for (int i = 0; i < iCnt ; i++)//AckPacket여기에 한패킷만 넣음.
                    {
                        bb = AckQue.Dequeue();

                        AckPacket.Enqueue(bb);
                        if (bb == 0xFF) break;
                    }
                    byte [] byPacket = AckPacket.ToArray() ;
                    AckPacket.Clear();

                    //byPacket의 두번째 바이트가 모터아이디라서 callback딕셔너리에 등록되어 있는 함수 호출.
                    if(diCallback.ContainsKey(byPacket[1]))
                    {
                        if(diCallback[byPacket[1]] != null)diCallback[byPacket[1]](byPacket);
                    }
                }
            }
        }
    }


    [Serializable]
    public class CParaUI
    {   
        [CategoryAttribute("UiPara"), DisplayNameAttribute("PortNo"         )] public uint   iPortNo         {get; set;} = 0     ;
        [CategoryAttribute("UiPara"), DisplayNameAttribute("ID"             )] public uint   iID             {get; set;} = 0     ; //실제모터 어드레스.    
        [CategoryAttribute("UiPara"), DisplayNameAttribute("Direction"      )] public bool   bDirection      {get; set;} = false ; //모터방향.
        [CategoryAttribute("UiPara"), DisplayNameAttribute("OpenLoop"       )] public bool   bOpenLoop       {get; set;} = false ; //엔코더 없는 오픈루프 타입 모터.
        [CategoryAttribute("UiPara"), DisplayNameAttribute("MotrEncResRatio")] public double dMotrEncResRatio {get; set;} = 1.6   ; //엔코더 없는 오픈루프 타입 모터.
        [CategoryAttribute("UiPara"), DisplayNameAttribute("UseStall"       )] public bool   bUseStall       { get; set; } = false; //엔코더 없는 오픈루프 타입 모터.

        [CategoryAttribute("HomePara"), DisplayNameAttribute("Home Clear Delay Time")]public int     iHomeClrTime   {get; set;}
        [CategoryAttribute("HomePara"), DisplayNameAttribute("Home Offset"          )]public double  dHomeOffset    {get; set;}
    } 

    public class CMotor:IMotor
    {        
        //이건 포트 몇개쓰냐에 따라 갯수가 정해짐.
        static Dictionary<uint,CMessenger> Ports = new Dictionary<uint, CMessenger>();

        public CMotor() { }       

        private  CParaUI Para = new CParaUI();
        //private  byte[]  RecvMsg  ;

        //bool bCmdPos = false;
        //bool bEncPos = false;

        //메세지 센드 동기화를 위해서.
        private Queue<string> SendingMsgQue = new Queue<string>();

        //메세지 센드 동기화 크리티컬섹션 락킹
        private object CriticalSection = new object();

        

        

        public struct TStat
        {
            //우리쪽에서 핸들링.
            public int    iHomeStep    ; //홈 동작중인지.                
            public bool   bHomeDone    ; //홈완료상태.

            public bool   bJogP    ; //+방향조그중.
            public bool   bJogN    ; //-방향조그중.
            public bool   bJogMoved; //조그 작동시 움직이기 시작했는지.
            public bool   bRun     ; //정지반대상태.
            public int    iTrgPos  ; //현재 움직이려는 타겟위치           
            
            
            //받아오기
            public int    iCmdPos  ; //실시간목적위치
            public int    iEncPos  ; //엔코다현재위치. 아 병신같이 크로즈루프에선 엔코더 기준이라 1.6을 곱해야 함.. ;;;
            public bool   bAlram   ; //알람.
            //public bool   bInpos   ;              시그널 없어서 GetStop 리턴   
            public bool   bServo   ;
            public bool   bNegLSen ; //-리밋센서
            public bool   bHomeSen ; //홈센서
            public bool   bPosLSen ; //+리밋센서

            //public bool bNegLSenUp; //-리밋센서 라이징
            //public bool bHomeSenUp; //홈센서
            //public bool bPosLSenUp; //+리밋센서
            //
            //public bool bNegLSenDn; //-리밋센서 폴링.
            //public bool bHomeSenDn; //홈센서
            //public bool bPosLSenDn; //+리밋센서





            public int    iSpeed   ; //모터스피드.

            //홈관련세팅
            public double dHomeVelFirst ; 
            public double dHomeVelLast  ; 
            public double dHomeAccFirst ;
            public double dPulsePerUnit ; //마지막 홈 오프셑 이동시에 필요함.
            //통신렉때문에 bStop을 우리쪽에서 핸들링 하면서 필요한 놈.
            //public int    iPreSpeed; //엣지검출용 스피드로본 러닝상태.


        }
        public TStat Stat ;
        const int iStallSet = 500;

        /// <summary>
        /// 초기화 함수.
        /// </summary>
        /// <returns>초기화 성공여부</returns>
        public bool Init()
        {
            if(Para.iPortNo==0) return false ;

            if(!Ports.ContainsKey(Para.iPortNo))
            {
                Ports[Para.iPortNo] = new CMessenger();

                if(!Ports[Para.iPortNo].PortOpen(Para.iPortNo))
                {
                    //포트가 안열림.
                    return false ;
                }
            }

            //등록후 혹은 기등록되어 있는경우 포트에
            Ports[Para.iPortNo].RegistDataRcvCallback(Para.iID , DataReceived);

            return true ;
        }

        public void SendLimitSet()
        {
            /*
            enum EEdgeMotion
          
            NoAction_NoRtcn    = 0b0000,
            NoAction_RTCN_MCFG = 0b0001,
            NegDirRun          = 0b0010,
            PosDirRun          = 0b1010,
            DecStop            = 0b0011,
            SetOri_DecStop     = 0b1011,
            SetOri_IncMove     = 0b0111,
            EmgStop            = 0b0100,
            SetOri_EmgStop     = 0b1100,
            NegIncMove         = 0b0101,
            PosIncMove         = 0b1101,
            SetOri             = 0b0110,
            ToggleDirIncMove   = 0b1001,
            ToggleDirRun       = 0b1110,
            EnableIntrFunction = 0b1000,
            Disable            = 0b1111,
          
            */
            //리밋센서 동작 세팅.
            string sSetting;
            //sSetting = "{ADR=" + Para.iID.ToString() + ";}" + "{STG0;}{ADR=" + Para.iID.ToString() + ";}{STG1;}{ADR=" + Para.iID.ToString() + ";}SCFG1024;";
            sSetting = "{STG0;}{ADR=" + Para.iID.ToString() + ";}{STG1;}{ADR=" + Para.iID.ToString() + ";}SCFG1024;";
            SendMsg(sSetting);

            //sSetting = "{ADR=" + Para.iID.ToString() + ";}" + "{STG2;}{ADR=" + Para.iID.ToString() + (Para.bUseStall ? ";}SCFG984065;" : ";}SCFG1025;");//SCFG1025//SCFG984065
            sSetting = "{STG2;}{ADR=" + Para.iID.ToString() + (Para.bUseStall ? ";}SCFG984065;" : ";}SCFG1025;");//SCFG1025//SCFG984065
            SendMsg(sSetting);
        }

        public void SendClear()
        {
            ////미리 날려놓은 명령어를 프로그램 켜면서 서보온 되면 그때 동작함. ;;;;;시부랄시부랄시부ㄹ랄
            //sSetting = "SPD0;QER500;SQT500;";
            //SendMsg(sSetting);
            string sSetting;
            sSetting = "QER500;"; //엔코더 분해능 고정500이고 4체배로 2000으로 들어옴.
            //sSetting = "QER500"; //엔코더 분해능 고정500이고 4체배로 2000으로 들어옴.
            SendMsg(sSetting);


            sSetting = "SQT" + iStallSet.ToString() + ";";  //stall세팅 알람범위 500 이고 알람은 홈동작 할때 알람떳을때 모터 서보오프.
            SendMsg(sSetting);
        }

        public void SendMsg(string _sMsg , bool _bSplit = false)
        {
            if (!Ports.ContainsKey(Para.iPortNo))
            {
                SendingMsgQue.Clear();
                return ;
            }
            if (!Ports[Para.iPortNo].IsPortOpen())
            {
                SendingMsgQue.Clear();
                return ;
            }
            if (Para.iID<= 5) 
            {
                SendingMsgQue.Clear();
                return ; //6번부터 설정해야함.
            }

            string sIdMsg = "{ADR=" + Para.iID + ";}" ; //모터아이디. 5번부터인데 6번부터 쓰자.
            
            //여러개 들어오면 분해해서 넣는다.
            //Ack도 분해해서 들어와서 그럼.
            //char [] sperator = {';'};
            //string [] sMsgs = _sMsg.Split(sperator);
            

            try
            {
                Ports[Para.iPortNo].SendMessage(sIdMsg + _sMsg); 
            }
            catch(Exception _e)
            {
                return ;
            }


            return ;

            //if(_bSplit) //같이 붙여서 보냈을때 응답이 붙인 갯수만큼 들어오는 명령
            //{
            //    foreach(string sMsg in sMsgs)
            //    {
            //        if(sMsg == "")continue;
            //        SendingMsgQue.Enqueue(sMsg+";"); //세퍼레이터로 사용한 ; 다시 장착.
            //    }
            //}
            //else //많이 이것저것 붙여도 응답이 한개만 들어올때.
            //{
            //    SendingMsgQue.Enqueue(_sMsg);
            //}
            
            //테스트용.
            //SendingMsgQue.Clear();
        }

        //존나 특이하게 UI로봇은 7비트를 사용함 ;;
        //그래서 1바이트에서 맨먼저들어오는 최상위비트는 없다고 생각 해야해서 데려다 쓰려면
        //맨위에꺼빼고 다시 Byte로 땡겨서 만들어야 함.
        //이게 16비트와 32비트가 있어서 2개만 만들음.
        //16비트는 2바이튼데 ui꺼는 3바이트, 32비트는 4바이튼데 ui꺼는 5바이트
        //씨부랄씨부랄씨부랄씨부랄씨부랄씨부랄씨부랄씨부랄씨부랄씨부랄씨부랄씨부랄씨부랄씨부랄
        public int Convert7bitToInt(byte [] _bt7BitData)
        {
            byte [] btData = new byte[4];
            byte btTemp ;

            //16비트
            if(_bt7BitData.Length == 3)//씨부랄씨부랄씨부랄씨부랄씨부랄씨부랄씨부랄씨부랄씨부랄씨부랄씨부랄씨부랄씨부랄씨부랄
            {
                btTemp = (byte)((_bt7BitData[0] & 0x7F) << 6); //0000 0011
                btData[1] = btTemp ;
                btTemp = (byte)((_bt7BitData[1] & 0x7F) >> 1); //0111 1111
                btData[1] = (byte)(btData[1] | btTemp) ;

                btTemp = (byte)((_bt7BitData[1] & 0x7F) << 7); //0000 0011
                btData[0] = btTemp ;
                btTemp = (byte)((_bt7BitData[2] & 0x7F)     ); //0111 1111
                btData[0] = (byte)(btData[0] | btTemp) ;
            }
            else if(_bt7BitData.Length == 5)//씨부랄씨부랄씨부랄씨부랄씨부랄씨부랄씨부랄씨부랄씨부랄씨부랄씨부랄씨부랄씨부랄씨부랄씨부랄씨부랄씨부랄씨부랄
            {
                btTemp = (byte)((_bt7BitData[0] & 0x7F) << 4); 
                btData[3] = btTemp ;
                btTemp = (byte)((_bt7BitData[1] & 0x7F) >> 3); 
                btData[3] = (byte)(btData[3] | btTemp) ;

                btTemp = (byte)((_bt7BitData[1] & 0x7F) << 5); 
                btData[2] = btTemp ;
                btTemp = (byte)((_bt7BitData[2] & 0x7F) >> 2); 
                btData[2] = (byte)(btData[2] | btTemp) ;

                btTemp = (byte)((_bt7BitData[2] & 0x7F) << 6); 
                btData[1] = btTemp ;
                btTemp = (byte)((_bt7BitData[3] & 0x7F) >> 1); 
                btData[1] = (byte)(btData[1] | btTemp) ;

                btTemp = (byte)((_bt7BitData[3] & 0x7F) << 7); 
                btData[0] = btTemp ;
                btTemp = (byte)((_bt7BitData[4] & 0x7F)     ); 
                btData[0] = (byte)(btData[0] | btTemp) ;
            }
            else
            {
                return 0 ; 
            }
            int iRet = BitConverter.ToInt32(btData ,0);

            return iRet ;

        }

        //한번에 한Cmd에 대한 Ack만 들어오게 위쪽에서 분할해서 보냄.
        CDelayTimer tmStopDelay = new CDelayTimer();
        void DataReceived(byte [] _btMsg)
        {
            int intRecSize = _btMsg.Length;

            if (intRecSize == 0) return;

            //현재 Ack대기중인 센딩했던 메세지.
            /*
            _btMsg[0]
            헤더에는 3 가지 종류가 있습니다.
            AA는 수신 된 명령의 반복 인 ACK 메시지를 나타냅니다.
            CC는 현재 작업 상태에 대한 설명 피드백을 나타냅니다.
            EE는 오류 메시지를 나타냅니다.
            _btMsg[1]
            콘트롤러 ID (5 – 125)
             */

            bool bPreServoOn = false;

            //메세지에 대한 피드백을 구분하는 수단이 없음..
            //일단 디버깅용도로 큐를 쓰자.
            string sSendedMsg = "" ;            

            //이건 메세지의 응답이 아니고 모터에서 이벤트발생시에 그냥 보내주는 프로토콜.
            if(_btMsg[0] == 0xCC && (_btMsg[2] & 0xF0)==0xA0) 
            {
                int iMsgId = _btMsg[2] & 0x0F ;
                     if (iMsgId == 0) { }//센서1 폴링
                else if (iMsgId == 1) { }//센서1 라이징
                else if (iMsgId == 2) { }//센서2 폴링
                else if (iMsgId == 3) { }//센서2 라이징
                else if (iMsgId == 4) { }//센서3 폴링
                else if (iMsgId == 5) { }//센서3 라이징
                else if (iMsgId == 6) { }//P4 폴링
                else if (iMsgId == 7) { }//P4 라이징
                else if (iMsgId == 8) //Target 포지션 도착 메세지 수신하도록 설정하면 모션돈 미리 세팅됌. 이상하게 조그모드들어갈때도 한번옴.
                {
                    if (!Stat.bJogN && !Stat.bJogP)
                    {
                        Stat.bRun = false;
                    }
                }
                else if (iMsgId == 9) { }//0점 도착.                
            }
            else if(SendingMsgQue.Count > 0)//이벤트 메세지 아니면 보낸 메세지에 대한 응답이므로 보낸메세지에서 하나 꺼낸다.
            {
                sSendedMsg = SendingMsgQue.Dequeue() ; 
            }
            else
            {
                //테스트.
                //return ;
            }
            
            
            if(/*sSendedMsg == "SFB;" &&*/ _btMsg.Length == 9 && _btMsg[2] == 0xc1 )//193 센서 입력 상태 확인.
            {
                

                Stat.bNegLSen = _btMsg[3] != 0 ; 
                Stat.bHomeSen = _btMsg[4] != 0 ;
                Stat.bPosLSen = _btMsg[5] != 0 ;
            }
            else if(/*sSendedMsg == "FBK;"*/_btMsg.Length==13 && _btMsg[0] == 0xcc) //모터 상태확인. 개짜증나게 메세지 아이디가 없음... 씨부랄씨부랄씨부랄씨부랄씨부랄씨부랄
            {
                //CC [Controller ID] [ASB] [CUR] [V0] [V1] [V2] [P0] [P1] [P2] [P3] [P4] FF
                //
                //Bit        7       6    5          4    3 2 1 0 
                //Defination N/A(=0) ACR  ENA / OFF  DIR  MCS – 1（0 = full step,15 = 1/16 step)
                Stat.bServo = (_btMsg[2] & 0b00100000) != 0 ;
                bool bDirection = (_btMsg[2] & 0b00010000) != 0 ;

                //알람을 받아보고 싶은데 없어서 이렇게 했는데 잘 안됌.. ㅜㅠ
                //if(bPreServoOn && !Stat.bServo && Math.Abs(Stat.iEncPos - Stat.iCmdPos) > iStallSet)Stat.bAlram = true;
                if (!Stat.bServo)
                {
                    //알람시그널이 없어서 그냥 홈던을 깨는 방식.
                    Stat.bHomeDone = false ;
                    
                }

                byte [] data = new byte[3];
                //4,5,6 이 모터 속도.
                Array.Copy(_btMsg , 4 , data , 0 , 3);
                Stat.iSpeed = bDirection ? Convert7bitToInt(data) : -Convert7bitToInt(data);

                bPreServoOn = Stat.bServo;
            }
            else if(/*sSendedMsg == "POS;" &&*/_btMsg.Length==9 && _btMsg[0] == 0xcc && _btMsg[2] == 0xb0)//CMD포지션 확인. 
            {//뻑났을때 AA 하고 배열7개
                byte [] data = new byte[5];
                //3,4,5,6,7이 포지션이고 합쳐서 
                Array.Copy(_btMsg , 3 , data , 0 , 5);
                Stat.iCmdPos = Convert7bitToInt(data);
                if(Para.bOpenLoop)Stat.iEncPos = 0 ;//다른숫자넣으려면 
            }
            else if(/*sSendedMsg == "QEC;" &&*/_btMsg[0] == 0xcc && _btMsg[2] == 0xb1)//ENC포지션 확인.
            {
                byte [] data = new byte[5];
                //3,4,5,6,7이 포지션이고 합쳐서 
                Array.Copy(_btMsg , 3 , data , 0 , 5);
                //Stat.iEncPos = (int)(Convert7bitToInt(data)* Para.MotrEncResRatio);
                Stat.iEncPos = Convert7bitToInt(data);
            }
            else
            {

            }


            if (Stat.iSpeed == 0 &&tmStopDelay.OnDelay(100))
            {
                Stat.bJogN = false;
                Stat.bJogP = false;
                Stat.bRun = false;

            }



            //알람 받아오는법 찾자.
            //public bool   bAlram   ; //알람.

        }

        int iPreHomeStep ; //홈 타임아웃 체킹용.
        CDelayTimer m_tmCycleHome = new CDelayTimer();
        CDelayTimer m_tmDelay = new CDelayTimer();


        private bool CycleHome()
        {
            string Msg = "";
            string sSetting = "";
            if (m_tmCycleHome.OnDelay((iPreHomeStep == Stat.iHomeStep) && !Stat.bRun, 50000))
            {
                //Trace Log.
                Msg = string.Format("UIRobot timeout m_tmCycleHome : Stat.iHomeStep=%d", Stat.iHomeStep);
                Log.Trace("UIRobot", Msg);

                SendLimitSet();

                Stat.iHomeStep = 0;
                return true ;
            }

            iPreHomeStep = Stat.iHomeStep;

            switch (Stat.iHomeStep)
            {
                default : 
                    Msg = string.Format("UIRobot default out m_tmCycleHome : Stat.iHomeStep=%d", Stat.iHomeStep);
                    Log.Trace("UIRobot", Msg);

                    SendLimitSet();

                    Stat.iHomeStep = 0;
                    //나가기 전에 리밋들 스탑 세팅.
                    return true ;

                case 10 ://마이너스 방향으로 홈센서 라이징 찾기 그중에 혹시 마이너스 센서 나오면 스탑 하고
                    //엣지로 스탑하는 방식이여서 감지되어 있는경우는 예외처리해야함.
                    sSetting = "STP0;";
                    SendMsg(sSetting);


                    if (Stat.bNegLSen)//네가티브센서가 감지되어 있는경우.
                    {
                        sSetting = "{STG0;}{ADR=" + Para.iID.ToString() + ";}{STG1;}{ADR=" + Para.iID.ToString() + ";}SCFG0;"; //-라이징 급정지쓰면 한번멈춤.
                        SendMsg(sSetting);
                        sSetting = "{STG2;}{ADR=" + Para.iID.ToString() + (Para.bUseStall ? ";}SCFG984065;" : ";}SCFG1025;"); ;//SCFG1025//SCFG984065
                        SendMsg(sSetting);
                        Stat.iHomeStep=22;//플러스 방향으로 가면서 홈 폴링 찾음.
                    }
                    else if(Stat.bHomeSen)//홈센서가 감지되어 있는경우.
                    {
                        Stat.iHomeStep=24;//플러스 방향으로 가면서 홈 폴링 찾음.
                    }
                    else//아무것도 감지 되어 있지 않은경우 혹은 +리밋만 감지된경우.
                    {
                        sSetting = "{STG0;}{ADR=" + Para.iID.ToString() + ";}{STG1;}{ADR=" + Para.iID.ToString() + ";}SCFG1024;"; //-라이징 급정지쓰면 한번멈춤.
                        SendMsg(sSetting);
                        sSetting = "{STG2;}{ADR=" + Para.iID.ToString() + (Para.bUseStall ? ";}SCFG983041;" : ";}SCFG1;");  //";}SCFG983041;";//SCFG1//SCFG984065  (Para.bUseStall ? ";}SCFG984065;" : ";}SCFG1025;");
                        SendMsg(sSetting);
                        Stat.iHomeStep=20;
                    }                    
                    return false ;

                //아무것도 감지 안된상황.
                //위에서 씀.
                case 20 : //-방향 진행하면서 홈센서 라이징 , -센서라이징 찾음.
                    //-쪽으로 조그이동.
                    JogN(Stat.dHomeVelFirst, Stat.dHomeAccFirst, Stat.dHomeAccFirst);
                    Stat.iHomeStep++;
                    return false;

                case 21:
                    if (Stat.bHomeSen)
                    {
                        HomeJogStop();
                        Stat.iHomeStep=24;
                        return false;
                    }
                    if (Stat.bNegLSen)//재수없게 -센서 감지시 방향전환.
                    {
                        HomeJogStop();
                        sSetting = "{STG0;}{ADR=" + Para.iID.ToString() + ";}{STG1;}{ADR=" + Para.iID.ToString() + ";}SCFG0;"; //-라이징 급정지쓰면 한번멈춤.
                        SendMsg(sSetting);
                        sSetting = "{STG2;}{ADR=" + Para.iID.ToString() + (Para.bUseStall ? ";}SCFG984065;" : ";}SCFG1025;"); ;//SCFG1025//SCFG984065
                        SendMsg(sSetting);

                        Stat.iHomeStep ++;

                        return false;
                    }

                    if (Stat.bPosLSen)//+리밋센서 감지시... 세팅 문제...
                    {
                        Msg = string.Format("UIRobot +Limit Error m_tmCycleHome : Stat.iHomeStep=%d", Stat.iHomeStep);
                        Log.Trace("UIRobot", Msg);
                        Stat.iHomeStep = 0;
                        return true;
                    }

                    return false;



                //-센서 감지시 예외처리=======================
                case 22 :
                    if (!GetStop()) return false;

                    

                    JogP(Stat.dHomeVelFirst, Stat.dHomeAccFirst, Stat.dHomeAccFirst);
                    Stat.iHomeStep++;
                    return false;
                case 23:
                    if (!Stat.bHomeSen) return false;
                    HomeJogStop();
                    Stat.iHomeStep++;
                    return false;
                //=======================================

                //위에서씀.
                case 24 ://+방향 진행하면서 홈센서 폴링 찾음.
                    if (!GetStop()) return false;
                    JogP(Stat.dHomeVelFirst, Stat.dHomeAccFirst, Stat.dHomeAccFirst);
                    
                    Stat.iHomeStep++;
                    return false;

                case 25:
                    if (Stat.bHomeSen) return false;
                    HomeJogStop();
                    Stat.iHomeStep++;
                    return false ;

                case 26:
                    JogN(Stat.dHomeVelLast, Stat.dHomeAccFirst, Stat.dHomeAccFirst);
                    Stat.iHomeStep++;
                    return false ;

                case 27://-방향 진행하면서 홈센서 라이징 , -센서라이징 찾음.
                    if (!Stat.bHomeSen) return false;
                    HomeJogStop();
                    //-쪽으로 조그이동.
                    

                    Stat.iHomeStep++;
                    return false  ;

                case 28 : //+방향 진행하면서 홈센서 폴링 찾음.
                    if(GetStop())return false ;
                    if (Stat.bNegLSen)//-리밋센서 감지시... 홈엣지 못찾음.
                    {
                        Msg = string.Format("UIRobot -Limit Error m_tmCycleHome : Stat.iHomeStep=%d", Stat.iHomeStep);
                        Log.Trace("UIRobot", Msg);
                        Stat.iHomeStep = 0 ; 
                        return true ;
                    }
                    if (Stat.bPosLSen)//+리밋센서 감지시... 세팅 문제...
                    {
                        Msg = string.Format("UIRobot +Limit Error m_tmCycleHome : Stat.iHomeStep=%d", Stat.iHomeStep);
                        Log.Trace("UIRobot", Msg);
                        Stat.iHomeStep = 0 ; 
                        return true ;
                    }
                    Stat.iHomeStep++;
                    return false  ;

                //위에서씀.
                case 29 :
                    //센서 엣지 모션 세팅.
                    sSetting = "{STG0;}{ADR=" + Para.iID.ToString() + ";}{STG1;}{ADR=" + Para.iID.ToString() +";}SCFG17152;" ; //홈센서 폴링시 급정지. -센서라이징시 감속정지.
                    SendMsg(sSetting);
                    sSetting = "{STG2;}{ADR=" + Para.iID.ToString() + (Para.bUseStall ? ";}SCFG984065;" : ";}SCFG1025;"); ;//SCFG1025//SCFG984065
                    SendMsg(sSetting);

                    
                    JogP(Stat.dHomeVelLast, Stat.dHomeAccFirst, Stat.dHomeAccFirst);
                    m_tmDelay.Clear();
                    Stat.iHomeStep++;
                    return false ;

                case 30 : //딜레이.
                    if(!GetStop()) return false ;

                    m_tmDelay.Clear();
                    Stat.iHomeStep++;
                    return false ;
                
                case 31 : //오프셑 이동.
                    if(!m_tmDelay.OnDelay(Para.iHomeClrTime)) return false ;

                    SendLimitSet();

                    SetPos(0);
                    GoAbs(Para.dHomeOffset * Stat.dPulsePerUnit , Stat.dHomeVelFirst , Stat.dHomeAccFirst , Stat.dHomeAccFirst);
                    Stat.iHomeStep++;
                    return false ;

                case 32 :
                    if(!GetStop())return false ;
                    m_tmDelay.Clear();
                    Stat.iHomeStep++;
                    return false;

                case 33:
                    if (!m_tmDelay.OnDelay(Para.iHomeClrTime)) return false;
                    SetPos(0);
                    Stat.bHomeDone = true ;
                    Stat.iHomeStep=0;
                    return true ;
            }


        }
        


        /// <summary>
        /// 종료 함수
        /// </summary>
        /// <returns>종료 성공여부</returns>
        public bool Close()
        {
            if(Ports.ContainsKey(Para.iPortNo))
            {
                Ports[Para.iPortNo].RegistDataRcvCallback(Para.iID , null);
                Ports[Para.iPortNo].PortClose();
                Ports.Remove(Para.iPortNo);
            }
            return true ;
        }
        /// <summary>
        /// 모터의 인포지션 확인 안한 정지여부.
        /// </summary>
        /// <returns>정지상태 true , 구동상태 false</returns>
        public bool GetStop()
        {
            return !Stat.bRun;
        }

        /// <summary>
        /// 서보 온오프
        /// </summary>
        /// <param name="_bOn">온오프</param>
        public void SetServo(bool _bOn)
        {
            

            if (_bOn) SendMsg("ENA;");
            else     SendMsg("OFF;");
        }
        /// <summary>
        /// 서보상태를 받아옴.
        /// </summary>
        /// <returns>온오프</returns>
        public bool GetServo()//나중에 서보 오프시에 홈시그널 깨지는지 확인해봐야함.
        {
            return Stat.bServo;
        }
        /// <summary>
        /// 리셑 시그널 온오프 제어
        /// </summary>
        /// <param name="_bOn">온오프</param>
        public void SetReset(bool _bOn)
        {
            if(_bOn)
            {
                Stat.bAlram = false;
            }
        }

        /// <summary>
        /// 펄스 커멘드 리턴.
        /// </summary>
        /// <returns>펄스 리턴</returns>
        public double GetCmdPos()
        {            
            return Stat.iCmdPos;
        }
        /// <summary>
        /// 펄스 엔코더 리턴.
        /// </summary>
        /// <returns>펄스 리턴</returns>
        public double GetEncPos()
        {
            if (Para.bOpenLoop) return Stat.iEncPos ;
            else                return Stat.iEncPos * Para.dMotrEncResRatio ;
        }
        /// <summary>
        /// Command Encoder Target포지션 세팅.
        /// </summary>
        /// <param name="_dPos">세팅할 펄스</param>
        public void SetPos(double _dPos)
        {
            int iPos; //= (int)_dPos ;d
            if (Para.bOpenLoop)
            {
                iPos = (int)_dPos;
            }
            else
            {//클로즈 루프시에 엔코더 포지션이 기준임. 모터는 3200펄스 엔코더4체배 2000펄스 
                double dPos = (_dPos / Para.dMotrEncResRatio);
                iPos = (int)Math.Round(dPos);
            }
            SendMsg("ORG" + iPos.ToString() + ";");
        }

        //Signal...
        /// <summary>
        /// 홈센서 시그널.
        /// </summary>
        /// <returns>OnOff상태</returns>
        public bool GetHomeSnsr()
        {
            return Stat.bHomeSen ;
        }

  
        /// <summary>
        /// -리밋 센서 시그널
        /// </summary>
        /// <returns>OnOff상태</returns>
        public bool GetNLimSnsr()
        {
            return Stat.bNegLSen ;
        }
        /// <summary>
        /// +리밋 센서 시그널
        /// </summary>
        /// <returns>OnOff상태</returns>
        public bool GetPLimSnsr()
        {
            return Stat.bPosLSen ;
        }
        /// <summary>
        /// 엔코더 Z상 시그널 상태
        /// </summary>
        /// <returns>OnOff상태</returns>
        public bool GetZphaseSgnl()
        {
            //센서입력이 3개밖에 안되서 어떻게 되는지 알아봐야함. 일단 스킵.
            return false;
        }
        /// <summary>
        /// 서보펙 인포지션 시그널 상태
        /// </summary>
        /// <returns>OnOff상태</returns>
        public bool GetInPosSgnl() //Servo Pack InPosition Signal.
        {
            return GetStop();
        }
        /// <summary>
        /// 서보펙 알람 시그널 상태
        /// </summary>
        /// <returns>OnOff상태</returns>
        public bool GetAlarmSgnl()
        {
            //이거 확인 해야함.... 알람범위 세팅은 있는데 어떻게 가져오는지 모르겠음.
            return Stat.bAlram;
        }

        /// <summary>
        /// 홈을 수행한다.
        /// </summary>
        /// <returns>수행 성공여부.</returns>
        public bool GoHome(double _dHomeVelFirst , double _dHomeVelLast , double _dHomeAccFirst)
        {
            Stat.dHomeVelFirst = _dHomeVelFirst ;
            Stat.dHomeVelLast  = _dHomeVelLast  ;
            Stat.dHomeAccFirst = _dHomeAccFirst ;

            Stat.bHomeDone = false ;
            Stat.iHomeStep = 10 ;
            return true ;
        }

        /// <summary>
        /// 홈동작 완료 확인.
        /// </summary>
        /// <returns>홈동작 완료여부</returns>
        public bool GetHomeDone()
        {
            return Stat.bHomeDone;
        }
        /// <summary>
        /// 강제로 홈돈 시그널 세팅.
        /// </summary>
        /// <param name="_bOn">세팅 값.</param>
        public void SetHomeDone(bool _bOn)
        {
            Stat.bHomeDone = _bOn ;  
        }
        /// <summary>
        /// 홈시퀜스를 중단한다.
        /// </summary>
        public void StopHome()
        {
            Stat.iHomeStep = 0 ;
            Stop();                  
        }

        /// <summary>
        /// 홈이 진행중인지 확인.
        /// </summary>
        public bool GetHoming()
        {
            return Stat.iHomeStep != 0;
        }

        




        //홈동작중 홈조그 스탑을 해야 클리어가 안됌.
        public void HomeJogStop()
        {
            //Stat.iHomeStep = 0;
            //if(Stat.iSpeed == 0) return ;
            SendMsg("SPD0;");

            //Stat.bInpos = false;
            Stat.bJogN = false;
            Stat.bJogP = false;


        }

        //Motion Functions.
        /// <summary>
        /// 모터 정지 명령.
        /// </summary>
        public void Stop()
        {
            Stat.iHomeStep = 0;
            //if(Stat.iSpeed == 0) return ;
            SendMsg("SPD0;");

            //Stat.bInpos = false;
            Stat.bJogN = false;
            Stat.bJogP = false;

            //홈잡을때 중간에 멈췄을경우.
            SendLimitSet();

        }
        /// <summary>
        /// 모터 감속 무시 긴급정지.
        /// </summary>
        public void EmgStop()
        {
            Stat.iHomeStep = 0;
            //if(Stat.iSpeed == 0) return ;
            //SendSetDcc(double _dDcc);
            SendMsg("SPD0;");

            //Stat.bInpos = false;
            Stat.bJogN = false;
            Stat.bJogP = false;

            SendLimitSet();
        }

        //위치제어시에 위치가 세팅 되어 있으면 speed세팅으로 이동 시작할 수 있어서 이건 잘 못쓸듯.
        //public void SendSetSpeed(double _dSpeed)
        //{
        //    int iSpeed = (int)_dSpeed ;
        //    SendMsg("SPD" + iSpeed.ToString() + ";");
        //}
        //
        //public void SendSetAccDcc(double _dAcc , double _dDcc)
        //{
        //    int iAcc = (int)_dAcc;
        //    int iDcc = (int)_dDcc;
        //
        //    SendMsg("MACC"+iAcc.ToString()+";" + "MDEC"+iDcc.ToString()+";");
        //}
        //
        //public void SendSetAcc(double _dAcc)
        //{
        //    int iAcc = (int)_dAcc;
        //
        //    SendMsg("MACC"+iAcc.ToString()+";");
        //
        //
        //}

        public void SendSetDcc(double _dDcc)
        {
            int iDcc = (int)_dDcc;

            SendMsg("MDEC"+iDcc.ToString()+";");


        }




        //내부 설정에 따라 Acc Dcc는 가감속율 즉 초당 증가pulse단위 
        /// <summary>
        /// +방향 조그 이동.
        /// </summary>
        /// <param name="_dVel">초당 구동 속도 펄스.</param>
        /// <param name="_dAcc">구동 가속율 펄스 </param>
        /// <param name="_dDec">감속율 펄스</param>
        public void JogP(double _dVel,double _dAcc,double _dDec)
        {
            //STP0 하면 스피드컨트롤모드 진입(조그모드)
            string sMsg = "STP0;" + "MACC"+((int)_dAcc).ToString()+";" + "MDEC"+((int)_dDec).ToString()+";" + "SPD" + ((int)_dVel).ToString()+";" ;
            SendMsg(sMsg);
            Stat.bJogP  = true ;
            Stat.bRun = true;
            tmStopDelay.Clear();
            //Stat.bJogMoved = false ;

            //STP0;MACC42000;MDEC42000;SPD420



        }
        /// <summary>
        /// -방향 조그 이동
        /// </summary>
        /// <param name="_dVel">초당 구동 속도 펄스</param>
        /// <param name="_dAcc">구동 가속율 펄스</param>
        /// <param name="_dDec">감속율 펄스</param>
        public void JogN(double _dVel,double _dAcc,double _dDec)
        {
            //STP0 하면 스피드컨트롤모드 진입(조그모드)
            string sMsg = "STP0;" + "MACC"+((int)_dAcc).ToString()+";" + "MDEC"+((int)_dDec).ToString()+";" + "SPD" + ((int)-_dVel).ToString()+";" ;
            SendMsg(sMsg);
            Stat.bJogN = true;
            Stat.bRun = true;
            tmStopDelay.Clear();
            //Stat.bRun  = true ;
            //Stat.bJogN = true ;
            //Stat.bJogMoved = false ;
        }
        /// <summary>
        /// 홈센서 기준 절대 위치로 이동.
        /// </summary>
        /// <param name="_dPos">이동할 위치 펄스단위</param>
        /// <param name="_dVel">이동할 속도 펄스단위</param>
        /// <param name="_dAcc">이동할 가속 펄스단위</param>
        /// <param name="_dDec">이동할 감속 펄스단위</param>
        /// <returns>성공여부</returns>
        public void GoAbs(double _dPos,double _dVel,double _dAcc,double _dDec)
        {
            //일단 멈추고 세팅하고 이동."SPD0;" + 
            string sMsg;
            int  iPos;
            double dPos;
            
            if (Para.bOpenLoop)
            {
                iPos = (int)_dPos  ;
            }
            else
            {//클로즈 루프시에 엔코더 포지션이 기준임. 모터는 3200펄스 엔코더4체배 2000펄스 
                dPos = (_dPos / Para.dMotrEncResRatio);
                iPos = (int)Math.Round(dPos);
            }

            if (Para.bOpenLoop)
            {
                sMsg = "POS" + iPos.ToString() + ";" + "MACC" + ((int)_dAcc).ToString() + ";" + "MDEC" + ((int)_dDec).ToString() + ";" + "SPD" + ((int)_dVel).ToString() + ";";
            }
            else
            {
                sMsg = "QEC" + iPos.ToString() + ";" + "MACC" + ((int)_dAcc).ToString() + ";" + "MDEC" + ((int)_dDec).ToString() + ";" + "SPD" + ((int)_dVel).ToString() + ";";
                //sMsg = "MACC" + ((int)_dAcc).ToString() + ";" + "MDEC" + ((int)_dDec).ToString() + ";" + "SPD" + ((int)_dVel).ToString() + ";" + "QEC" + iPos.ToString() + ";";
            }
            SendMsg(sMsg);
            Stat.bJogN = false;
            Stat.bJogP = false;

            //SendMsg("SPD0;");
            //SendMsg("POS"+((int)_dPos).ToString()+";");
            //SendMsg("MACC"+((int)_dAcc).ToString()+";");
            //SendMsg("MDEC"+((int)_dDec).ToString()+";");
            //SendMsg("SPD"+((int)_dVel).ToString() + ";");


            tmStopDelay.Clear();

            Stat.iTrgPos = (int)_dPos ;
            Stat.bRun = true ;

        }

        /// <summary>
        /// 홈센서 기준 절대 위치로 다축 이동.
        /// </summary>
        /// <param name="_iPhysicalNos">이동할축들 0번이 마스터</param>
        /// <param name="_dPoses">이동할 위치 펄스단위</param>
        /// <param name="_dVel">이동할 속도 펄스단위</param>
        /// <param name="_dAcc">이동할 가속 펄스단위</param>
        /// <param name="_dDec">이동할 감속 펄스단위</param>
        /// <returns>성공여부</returns>
        public void GoMultiAbs(int [] _iPhysicalNos , double [] _dPoses , double _dVel , double _dAcc , double _dDec)
        {
            //구현 아직 안함.
        }


        /// <summary>
        /// 시그널 찾아서 감속 정지한다..
        /// </summary>
        /// <param name="_dVel">구동 속도 펄스단위</param>
        /// <param name="_dAcc">구동 가속 펄스단위</param>
        /// <param name="_eSignal">사용 시그널</param>
        /// <param name="_bEdgeUp">업엣지인지 다운엣지인지.</param>
        /// <returns></returns>
        public void FindEdgeStop(double _dVel,double _dAcc,EN_FIND_EDGE_SIGNAL _eSignal,bool _bEdgeUp , bool _bEmgStop=false)
        {
            
            
        }
        /// <summary>
        /// 현재 파라미터들을 아진 함수를 이용하여 세팅함.η 
        /// </summary>

        string sLastSendedICFG = "" ;
        public void ApplyPara(double _dPulsePerUnit)
        {
            if(Para.iPortNo == 0) return ; //1번부터 가능.

            //포트관련. 원래 있던 포트는 다른애가 쓸수도 있어서 그냥 냅둔다.
            //한번 여기 타면 파일에 저장 되어서 다음에 켤때는 생성 안됌.
            if (!Ports.ContainsKey(Para.iPortNo))
            {
                Init();
                sLastSendedICFG = "";
            }


            string sSetting = "" ;

            //모터방향.
            //==============================================
            //주의 ICFG  명령 보내면 초기화 되는듯 함. 서보 오프되고 센서 엣지 세팅 초기화 됌.
            //===============================================
            sSetting = Para.bDirection ? "ICFG2;"    : "ICFG0;"    ; 
            if(sLastSendedICFG != sSetting) //설정이 바뀌었을때만 보낸다. 
            {
                SendMsg(sSetting);//이거 보내면  바뀌지 않았을땐 안보내는게 좋음.
                sLastSendedICFG = sSetting ;

                Stat.bHomeDone = false ;

                //Thread.Sleep(200);
                //
                SendLimitSet();
                SendClear();

                sSetting = "ENA;";
                SendMsg(sSetting);
                
            }

            //여기 엔코더 있는타입 확인 해서 바꿔야 함. 
            //가속도/가속시간 설정도 같이 들어 있음. 가속도로 씀.
            if(Para.bOpenLoop )
            {
                sSetting = "MCFG1040;";
            }
            else 
            {
                sSetting = Para.bUseStall ? "MCFG11344;" : "MCFG11280;";
            }
            SendMsg(sSetting);


            Stat.dPulsePerUnit = _dPulsePerUnit ;


            





        }

	    


        //특수기능
        /// <summary>
        /// 하드웨어 트리거를 발생시킨다.
        /// </summary>
        /// <param name="_dPos">발생시킬 트리거 위치들</param>
        /// <param name="_dTrgTime">트리거 시그널의 시간us</param>
        /// <param name="_bActual">엔코더기준인지 커멘드 기준인지</param>
        /// <param name="_bOnLevel">트리거 레벨</param>
        public void SetTrgPos(double[] _dPos,double _dTrgTime,bool _bActual,bool _bOnLevel)
        {
            
        }

        /// <summary>
        /// SetTrgPos로 세팅된 포지션 리셑
        /// </summary>
        public void ResetTrgPos()
        {
           
        }
        /// <summary>
        /// 테스트로 한번의 트리거를 출력.
        /// </summary>
        /// <param name="_bOnLevel">출력 레벨</param>
        /// <param name="_iTime">시간us</param>
        public void OneShotTrg(bool _bOnLevel, int _iTime)
        {

        }

        
        /// <summary>
        /// 해당 모터의 입력 비트를 확인
        /// </summary>
        /// <param name="_iNo">사용할 비트 넘버</param>
        /// <returns>true=ON, false=OFF</returns>
        public bool GetX(int _iNo)
        {
            return false ;
        }

        /// <summary>
        /// 해당 모터의 출력 비트를 확인
        /// </summary>
        /// <param name="_iNo">사용할 비트 넘버</param>
        /// <returns>true=ON, false=OFF</returns>
        public bool GetY(int _iNo)
        {
            return false ;
        }

        /// <summary>
        /// 해당 모터의 출력 비트를 제어
        /// </summary>
        /// <param name="_iNo">사용할 비트 넘버</param>
        /// <param name="_bOn">true=ON, false=OFF</param>
        public void SetY(int _iNo, bool _bOn)
        {

        }
        /// <summary>
        /// 하드웨어 트리거를 시작 위치 부터 끝위치 까지 일정 간격으로 트리거를 발생 시킨다.
        /// </summary>
        /// <param name="_dStt">시작 위치</param>
        /// <param name="_dEnd">종료 위치</param>
        /// <param name="_dDist">주기</param>
        /// <param name="_dTrgTime">트리거 시그널의 시간us</param>
        /// <param name="_bActual">엔코더기준인지 커맨드 기준인지</param>
        /// <param name="_bOnLevel">트리거 레벨</param>
        public void SetTrgBlock(double _dStt, double _dEnd, double _dPeriod, double _dTrgTime, bool _bActual, bool _bOnLevel){ }

        /// <summary>
        /// OverrideVel을 하기전에 구동속도중에 가장 높은 놈을 세팅을 한다.
        /// </summary>
        /// <param name="_dMaxVel">가장높은 속도값</param>
        /// <returns></returns>
        public bool SetOverrideMaxVel(double _dMaxVel){ return false ;}
        
        /// <summary>
        /// 구동중 속도를 오버라이딩한다.
        /// 정속구간에서만 먹는 함수 이고. SetOverrideMaxVel 구동 속도중 가장 빠른 속도를 세팅 하고 수행해야함.
        /// </summary>
        /// <param name="_dVel">오버라이딩 할 속도</param>
        /// <returns></returns>
        public bool OverrideVel(double _dVel){ return false ;}



        public void ContiSetAxisMap(int _iCoord, uint _uiAxisCnt, int[] _iaAxisNo){}
        public void ContiSetAbsRelMode(int _iCoord, uint _uiAbsRelMode){}
        public void ContiBeginNode(int _iCoord){}
        public void ContiEndNode(int _iCoord){}
        public void ContiStart(int _iCoord, uint _uiProfileset, int _iAngle){}
        public int  GetContCrntIdx(int _iCoord){return 0;}
        public void LineMove(int _iCoord, double[] _daEndPos, double _dVel, double _dAcc, double _dDec){}

        public void CircleCenterMove(int _iCoord, int[] _iaAxisNo, double[] _daCenterPos, double[] _daEndPos, double _dVel, double _dAcc, double _dDec, uint _uiCWDir){}
        public void CirclePointMove(int _iCoord, int[] _iaAxisNo, double[] _daMidPos, double[] _daEndPos, double _dVel, double _dAcc, double _dDec, int _iArcCircle){}
        public void CircleRadiusMove(int _iCoord, int[] _iaAxisNo, double _dRadius, double[] _daEndPos, double _dVel, double _dAcc, double _dDec, uint _uiCWDir, uint _uiShortDistance){}
        public void CircleAngleMove(int _iCoord, int[] _iaAxisNo, double[] _daCenterPos, double _dAngle, double _dVel, double _dAcc, double _dDec, uint _uiCWDir){}

        /// <summary>
        /// Master/Slave모터간 동기구동
        /// 이 함수는 밖에서 자유롭게 Link Enable 할수 있다.
        /// </summary>
        public bool SetLinkEnable(int _iSlvMotrNo)
        {
            return false ;
        }

        /// <summary>
        /// 현재 동기구동 상태이면 return true;
        /// </summary>
        /// <returns></returns>
        public bool GetLinkMode()
        {
            return false ;
        }

        /// <summary>
        /// 동기구동 해제
        /// </summary>
        public bool SetLinkDisable(int _iSlvMotrNo)
        {
            return false ;
        }

        /// <summary>
        /// 상대위치, 절대위치 변경
        /// </summary>
        /// <param name="_iAxisNo"> 축 넘버 </param>
        /// <param name="_uiAbsRelMode">0 : 절대모드, 1:상대모드</param>
        public void SetAbsRelMode(uint _uiAbsRelMode)
        {
            //return false ;
        }

        /// <summary>
        /// Master축이 +로 이동할때 Slave 축이 -로 이동하도록 하는 조그 기능
        /// 실제 동기구동은 아님
        /// </summary>
        /// <param name="iSlvMotrNo"> Slave 축 넘버 </param>
        /// <param name="_dVel"> 마스터축 속도 </param>
        /// <param name="_dAcc"> 마스터축 가속 </param>
        /// <param name="_dDec"> 마스터축 감속 </param>
        public void LinkJogP(int iSlvMotrNo, double _dVel, double _dAcc, double _dDec)
        {

        }

        /// <summary>
        /// Master축이 -로 이동할때 Slave 축이 +로 이동하도록 하는 조그 기능
        /// 실제 동기구동은 아님
        /// </summary>
        /// <param name="iSlvMotrNo"> Slave 축 넘버 </param>
        /// <param name="_dVel"> 마스터축 속도 </param>
        /// <param name="_dAcc"> 마스터축 가속 </param>
        /// <param name="_dDec"> 마스터축 감속 </param>
        public void LinkJogN(int iSlvMotrNo, double _dVel, double _dAcc, double _dDec)
        {

        }

        
        private byte[] GetStrToByte(string _sStr)
        {
            byte[] Bytes = new byte[_sStr.Length];
            for(int i = 0 ; i < _sStr.Length ; i++)
            {
                Bytes[i] = (byte)_sStr[i];
            }

            return Bytes;
            
        }

        private byte[] GetListToByte(List<byte> _lStr)
        {
            byte[] Bytes = new byte[_lStr.Count];
            for (int i = 0; i < _lStr.Count; i++)
            {
                Bytes[i] = (byte)_lStr[i];
            }

            return Bytes;

        }

        /// <summary>
        /// 홈이 지원 안되는 보드 같은경우 돌려주고 Update 함수 내부에서 처리 해야 한다.
        /// </summary>
        private int iUpdateStep = 0 ;
        private int iPreUpdateStep = 0;

        CDelayTimer m_tmUpdate = new CDelayTimer();
        CDelayTimer m_tmUpdateDelay = new CDelayTimer();

        public void Update()
        {
            if(Stat.iHomeStep != 0) CycleHome();
            //if(!SendingMsgQue.Contains("SFB") && !SendingMsgQue.Contains("QEC")) 
            
            

            //일단 업데이트가 필요 없네 ;
            if (m_tmUpdate.OnDelay(iPreUpdateStep == iUpdateStep, 1000))
            {
                //Trace Log.
                string Msg;
                Msg = string.Format("m_tmUpdate : m_iStep=%d", iUpdateStep);
                Log.Trace("UiRobot", Msg);
                iUpdateStep = 0;
            }

            iPreUpdateStep = iUpdateStep;

            switch (iUpdateStep)
            {
                default : 
                    iUpdateStep = 10 ;
                    break ;


                case 10:
                    //break ;
                    if(Para.bOpenLoop)SendMsg("SFB;FBK;POS;" , true); 
                    else              SendMsg("SFB;FBK;POS;QEC;" , true); 
                    

                    //SendMsg("FBK;" ); 

                    m_tmUpdateDelay.Clear();
                    iUpdateStep++;
                    break;

                case 11 :
                    if(!m_tmUpdateDelay.OnDelay(50)) break ;
                    iUpdateStep++;
                    break ;

                case 12 :
                    iUpdateStep=10;
                    break ;

                //알람
            }
        } 

        /// <summary>
        /// 해당모터축의 서브파라미터를 LoadSave함.
        /// </summary>
        /// <param name="_bLoad">true=로드 false=세이브</param>
        /// <returns>성공여부</returns>
        public bool LoadSave(bool _bLoad, string _sParaFolderPath, int _iMotrNo)
        {
            string sFilePath = _sParaFolderPath + "MotrUi" + _iMotrNo.ToString() + ".xml";
            //object oParaMotrSub = Para ;
            //CXml.SaveXml(sFilePath, ref Para);
            if (_bLoad)
            {
                //if (!CXml.LoadXml(sFilePath, ref Para)) { return false; }
                try
                {
                    CXml.LoadXml(sFilePath, ref Para);
                }
                catch
                {
                    return false ;
                }
            }
            else
            {
                if (!CXml.SaveXml(sFilePath, ref Para)) { return false; }
            }
            
            return true ;
        }

        public object GetPara()
        {
            return Para;
        }   
    }
}
