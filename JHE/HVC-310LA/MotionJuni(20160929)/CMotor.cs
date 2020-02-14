using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO.Ports;

using MotionInterface;
using COMMON;
/*
자화전자 렌즈체결기.
RS232 Com1 Com2 
2포트로 모터 2개 구성

컨피그상 4096분해능 우리프로그램 4000으로 세팅 이부분 집고 넘어가야함.

* JuniServoProgrammer_Setup_20161002 를 셑업하고 
* JSProgramer(Hanwha)_V1.0.33 안에 있는 파일을 실행폴더에 삽입하여 사용.
* JSD-B7_r2_1(수정중) 43페이지로 펌웨어 업그레이드
* JSProgramerParameter_NeedManualSetting 폴더에 있는 수치로 비교하여 넣는다.
* Configuration에 ConfigSave 옆의 Save 버튼을 누르면 세이브됌.
* Config Lock에 3457을 넣으면 PID값을 바꿀수 있다.
* PID 게인은 이문서 작성후 수정할 확율이 높으니 참고.
* P는 포지션 게인
* D는 오버슈트 억제 게인 너무 높이면 떨기 시작 함. 대략 P게인의 3배에서 5배사이
* I는 누적위치 게인?? 5이상은 해보니 안된다고 함. 
     
*/

namespace MotionJuni
{
    public enum BAUD_RATE : uint
    {
        BR9600  ,
        BR14400 ,
        BR19200 ,
        BR38400 ,
        BR57600 ,
        BR115200,
        BR229800,
        BR459700,
        BR930200
    } 

    [Serializable]
    public class CParaMotorJuni
    {   
        [CategoryAttribute("JuniPara"), DisplayNameAttribute("PortNo"               )]public int         iPortNo      {get; set;} //실제모터 물리 어드레스. 
        [CategoryAttribute("JuniPara"), DisplayNameAttribute("BaudRate"             )]public BAUD_RATE   eBaudRate    {get; set;} //실제모터 물리 어드레스. 
        //[CategoryAttribute("JuniPara"), DisplayNameAttribute("PulsePerRev"          )]public int         iPulsePerRev {get; set;} //실제모터 물리 어드레스. 
        //[CategoryAttribute("JuniPara"), DisplayNameAttribute("UnitPerRev"           )]public double      dUnitPerRev  {get; set;} //실제모터 물리 어드레스. 
                                                                                                                            
        //[CategoryAttribute("HomePara"), DisplayNameAttribute("Home Offset"          )]public double      dHomeOffset  {get; set;}
        
    } 

    public class CMotor:IMotor
    {
        
        public CMotor() { }


        private  CParaMotorJuni Para ;
        public   SerialPort siPort = null;
        public   bool bRcvdMsg = false; 
        public   byte[] RecvMsg;
        public List<byte> lRecvMsg ;
        public string sRecvMsg;

        bool bCmdPos = false;
        bool bEncPos = false;

        //메세지 센드 동기화를 위해서.
        //private Queue<byte[]> MsgQue = new Queue<byte[]>();

        //메세지 센드 동기화 크리티컬섹션 락킹
        //private object CriticalSection = new object();

        public struct TStat
        {
            //우리쪽에서 핸들링.
            public double dCmdPos  ;
            public double dTrgPos  ;            
            public bool   bHomming ;                     
            public bool   bJogP    ;
            public bool   bJogN    ;
            
            //받아오기
            public double dEncPos  ;
            public bool   bAlram   ;
            public double dTorque  ;
            public bool   bInpos   ;
            public bool   bStop    ;
            public bool   bHomeDone;
            public bool   bServo   ;
        }

        public TStat Stat ;

        /// <summary>
        /// 초기화 함수.
        /// </summary>
        /// <returns>초기화 성공여부</returns>
        public bool Init()
        {
            Para = new CParaMotorJuni();

            siPort = new SerialPort();

            lRecvMsg = new List<byte>();
            
            return true ;
        }

        /// <summary>
        /// 초기화 함수.
        /// </summary>
        /// <returns>초기화 성공여부</returns>
        public bool Ready()
        {

            string sPortName = string.Format("Com{0:0}", Para.iPortNo);

            siPort.DataReceived += new SerialDataReceivedEventHandler(DataReceived);

            int iBaudRate = 0;

            switch(Para.eBaudRate)
            {
                default                : iBaudRate = 115200; break;
                case BAUD_RATE.BR9600  : iBaudRate = 9600  ; break;
                case BAUD_RATE.BR14400 : iBaudRate = 14400 ; break;
                case BAUD_RATE.BR19200 : iBaudRate = 19200 ; break;
                case BAUD_RATE.BR38400 : iBaudRate = 38400 ; break;
                case BAUD_RATE.BR57600 : iBaudRate = 57600 ; break;
                case BAUD_RATE.BR115200: iBaudRate = 115200; break;
                case BAUD_RATE.BR229800: iBaudRate = 229800; break;
                case BAUD_RATE.BR459700: iBaudRate = 459700; break;
                case BAUD_RATE.BR930200: iBaudRate = 930200; break;

            }
            



            siPort.PortName = sPortName;
            siPort.BaudRate = iBaudRate; // Para.eBaudRate;
            siPort.DataBits = 8;
            siPort.Parity   = Parity.None;
            siPort.StopBits = StopBits.One ;
            siPort.ReadTimeout  = (int)500;
            siPort.WriteTimeout = (int)500;
            try
            {
                siPort.Open();
            }
            catch
            {
                Log.ShowMessage("PORT OPEN ERROR", "COM Port not opened");
            }

            if (!siPort.IsOpen)
            {
                Log.ShowMessage("COM PORT ERROR", "COM Port not opened");
                return false;
            }

            //프로그램 켜지면 풀 토크로 셋팅하는 거
            SendTorqueMax(4095);
            SendTorqueLimit(4000);
            SendTorqueTime(4000);

            //토크 셋팅하면 서보 오프/온 해줘야함
            SetServo(false);
            SetServo(true);

            return true;
        }

        
        public void SendMsg(byte [] _sMsg)
        {
            siPort.Write(_sMsg , 0, _sMsg.Length);           
        }


        //int iTestSet = 0;
        void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            int intRecSize = siPort.BytesToRead;

            if (intRecSize != 0)
            {
                //iTestSet = 1;
                RecvMsg = new byte[intRecSize];
                siPort.Read(RecvMsg, 0, intRecSize);
                //if (!bRcvdMsg)
                //{
                //    sRecvMsg = "";
                //}

                //Convert.

                for(int i = 0 ; i < intRecSize ; i++)
                {
                    lRecvMsg.Add(RecvMsg[i]);
                }

                sRecvMsg += RecvMsg.ToString();

                bRcvdMsg = true;

                //iTestSet = 0;
            }
        }

        public byte [] ReadMsg()
        {
            return RecvMsg;
        }

        public void ClearRcvMsg()
        {
            if (RecvMsg != null)
            {
                RecvMsg = null;
                //iTestSet = 9;
            }

        }

        //public double UnitToPulse(double _dUnit)
        //{
        //    double dPulse = Para.iPulsePerRev;
        //    double dUnit = Para.dUnitPerRev;
        //
        //    return (dPulse * _dUnit) / dUnit;
        //}
        //
        //public double PulseToUnit(double _dPulse)
        //{
        //    double dPulse = Para.iPulsePerRev;
        //    double dUnit = Para.dUnitPerRev;
        //
        //    return (dUnit * _dPulse) / dPulse;
        //}
        //
        //public double UnitToRPM(double _dUnit)
        //{
        //    //쎄타모터의 경우 1바퀴에 360도 
        //    //360 일때 60을 반환해야함.
        //
        //    double dPulse = Para.iPulsePerRev;
        //    double dUnit = Para.dUnitPerRev;
        //
        //    return (_dUnit / dUnit) * 60;
        //}

        //juni acc is .
        //public double PulseToJuni(double _dPulse)
        //{
        //    const double dJuni = 2500 ; //juni 1unit == about 2500pulse/sec^2
        //
        //    return _dPulse / dJuni;
        //
        //}
        //
        //public double PulseToRPM(double _dPulse)
        //{
        //    //쎄타모터의 경우 1바퀴에 360도 
        //    //360 일때 60을 반환해야함.
        //
        //    double dPulse = Para.iPulsePerRev;
        //    double dUnit = Para.dUnitPerRev;
        //
        //    return (_dPulse / dPulse) * 60;
        //}
    
        /// <summary>
        /// 종료 함수
        /// </summary>
        /// <returns>종료 성공여부</returns>
        public bool Close()
        {
            siPort.Close();
            siPort.Dispose();
            return true;
        }
        /// <summary>
        /// 모터의 인포지션 확인 안한 정지여부.
        /// </summary>
        /// <returns>정지상태 true , 구동상태 false</returns>
        public bool GetStop()
        {
            return Stat.bStop;
        }

        /// <summary>
        /// 서보 온오프
        /// </summary>
        /// <param name="_bOn">온오프</param>
        public void SetServo(bool _bOn)
        {
            const int iMsgLength = 7;
            byte[] cMsgs = new byte[iMsgLength];

            cMsgs[0] = 150;//head
            cMsgs[1] = 0;//id
            cMsgs[2] = 80;//cmdid
            cMsgs[3] = 2;//data length
            cMsgs[4] = _bOn ? (byte)1 : (byte)0;//data0
            cMsgs[5] = 0;//data1

            cMsgs[iMsgLength - 1] = 0;
            for (int i = 2; i < iMsgLength - 1; i++)
            {
                cMsgs[iMsgLength - 1] += cMsgs[i];
            }

            Stat.bServo = _bOn;

            SendMsg(cMsgs);
            //lock (CriticalSection)
            //{
            //    MsgQue.Enqueue(cMsgs);
            //}
            
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
            if (!_bOn) return;

            const int iMsgLength = 7;
            byte[] cMsgs = new byte[iMsgLength];

            cMsgs[0] = 150;//head
            cMsgs[1] = 0;//id
            cMsgs[2] = 84;//cmdid
            cMsgs[3] = 2;//data length
            cMsgs[4] = 0;//data0
            cMsgs[5] = 1;//data1

            cMsgs[iMsgLength - 1] = 0;
            for (int i = 2; i < iMsgLength - 1; i++)
            {
                cMsgs[iMsgLength - 1] += cMsgs[i];
            }

            SendMsg(cMsgs);
            //lock (CriticalSection)
            //{
            //    MsgQue.Enqueue(cMsgs);
            //}
        }

        /// <summary>
        /// 펄스 커멘드 리턴.
        /// </summary>
        /// <returns>펄스 리턴</returns>
        public double GetCmdPos()
        {
            
            return Stat.dCmdPos;
        }
        /// <summary>
        /// 펄스 엔코더 리턴.
        /// </summary>
        /// <returns>펄스 리턴</returns>
        public double GetEncPos()
        {
            return Stat.dEncPos;
        }
        /// <summary>
        /// Command Encoder Target포지션 세팅.
        /// </summary>
        /// <param name="_dPos">세팅할 펄스</param>
        public void SetPos(double _dPos)
        {
            
        }

        //Signal...
        /// <summary>
        /// 홈센서 시그널.
        /// </summary>
        /// <returns>OnOff상태</returns>
        public bool GetHomeSnsr()
        {
            //먼훗날 구현..
            return false;
        }

  
        /// <summary>
        /// -리밋 센서 시그널
        /// </summary>
        /// <returns>OnOff상태</returns>
        public bool GetNLimSnsr()
        {
            //먼훗날 구현..
            return false;
        }
        /// <summary>
        /// +리밋 센서 시그널
        /// </summary>
        /// <returns>OnOff상태</returns>
        public bool GetPLimSnsr()
        {
            //먼훗날 구현..
            return false;
        }
        /// <summary>
        /// 엔코더 Z상 시그널 상태
        /// </summary>
        /// <returns>OnOff상태</returns>
        public bool GetZphaseSgnl()
        {
            //먼훗날 구현..
            return false;
        }
        /// <summary>
        /// 서보펙 인포지션 시그널 상태
        /// </summary>
        /// <returns>OnOff상태</returns>
        public bool GetInPosSgnl() //Servo Pack InPosition Signal.
        {
            return Stat.bInpos;
        }
        /// <summary>
        /// 서보펙 알람 시그널 상태
        /// </summary>
        /// <returns>OnOff상태</returns>
        public bool GetAlarmSgnl()
        {
            return Stat.bAlram;
        }

        /// <summary>
        /// 홈을 수행한다.
        /// </summary>
        /// <returns>수행 성공여부.</returns>
        public bool GoHome(double _dHomeVelFirst , double _dHomeVelLast , double _dHomeAccFirst, double _dMinPosPulse, double _dMaxPosPulse)
        {
            SetReset(true);
            SetServo(true);

            SendSetAccDcc(_dHomeAccFirst);

            //Set Home Speed 50 set
            // 150 0 112 2 50 0 164 

            //double dRpmSpeed = _PulseToRPM(_dHomeVelFirst);// UnitToRPM(_dSpeed);
            int iRpmSpeed = (int)_dHomeVelFirst;

            int iMsgLength = 7;
            byte[] cMsgs = new byte[iMsgLength];

            cMsgs[0] = 150;//head
            cMsgs[1] = 0;//id
            cMsgs[2] = 112;//cmdid
            cMsgs[3] = 2;//data length
            cMsgs[4] = (byte)(iRpmSpeed     & 0x00FF) ; 
            cMsgs[5] = (byte)(iRpmSpeed>>8  & 0x00FF) ; 
            //cMsgs[6] = (byte)(iRpmSpeed>>16 & 0x00FF) ; 
            //cMsgs[7] = (byte)(iRpmSpeed>>24 & 0x00FF) ; 



            //byte[] cHexa = BitConverter.GetBytes(iRpmSpeed);
            //for (int i = 0; i < 2; i++)
            //{
            //    cMsgs[4 + i] = cHexa[i];
            //}
            cMsgs[iMsgLength - 1] = 0;
            for (int i = 2; i < iMsgLength - 1; i++)
            {
                cMsgs[iMsgLength - 1] += cMsgs[i];
            }

            SendMsg(cMsgs);
            //lock (CriticalSection)
            //{
            //    MsgQue.Enqueue(cMsgs);
            //}


            //SetGoHome
            iMsgLength = 7;
            cMsgs = new byte[iMsgLength];

            cMsgs[0] = 150;//head
            cMsgs[1] = 0;//id
            cMsgs[2] = 108;//cmdid
            cMsgs[3] = 2;//data length
            cMsgs[4] = 1;//data0
            cMsgs[5] = 0;//data1

            cMsgs[iMsgLength - 1] = 0;
            for (int i = 2; i < iMsgLength - 1; i++)
            {
                cMsgs[iMsgLength - 1] += cMsgs[i];
            }

            SendMsg(cMsgs);
            //lock (CriticalSection)
            //{
            //    MsgQue.Enqueue(cMsgs);
            //}
            
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
              
        }
        /// <summary>
        /// 홈시퀜스를 중단한다.
        /// </summary>
        public void StopHome()
        {
            Stop();                  
        }

        /// <summary>
        /// 홈이 진행중인지 확인.
        /// </summary>
        public bool GetHoming()
        {
            return Stat.bHomming;
        }


        private void CycleHome()
        {

        }

        //조그 구동후에 조스스탑으로 안스면 같은방향 조그가 안먹는다.
        public void JogStop()
        {
            // 150 0 38 2 0 0 40
            Stat.bJogN = false;
            Stat.bJogP = false;

            const int iMsgLength = 7;
            byte[] cMsgs = new byte[iMsgLength];

            cMsgs[0] = 150;//head
            cMsgs[1] = 0;//id
            cMsgs[2] = 38;//cmdid
            cMsgs[3] = 2;//data length
            cMsgs[4] = 0;//data0
            cMsgs[5] = 0;//data1

            cMsgs[iMsgLength - 1] = 0;
            for (int i = 2; i < iMsgLength - 1; i++)
            {
                cMsgs[iMsgLength - 1] += cMsgs[i];
            }

            SendMsg(cMsgs);
            //lock (CriticalSection)
            //{
            //    MsgQue.Enqueue(cMsgs);
            //}

        }

        //Motion Functions.
        /// <summary>
        /// 모터 정지 명령.
        /// </summary>
        public void Stop()
        {
            if (Stat.bJogN || Stat.bJogP)//조그 구동후에 조스스탑으로 안스면 같은방향 조그가 안먹는다.
            {
                JogStop();

                return;
            }
      
            // 150 0 80 2 8 0 90
            const int iMsgLength = 7;
            byte[] cMsgs = new byte[iMsgLength];

            cMsgs[0] = 150;//head
            cMsgs[1] = 0;//id
            cMsgs[2] = 80;//cmdid
            cMsgs[3] = 2;//data length
            cMsgs[4] = 8;//data0
            cMsgs[5] = 0;//data1

            cMsgs[iMsgLength - 1] = 0;
            for (int i = 2; i < iMsgLength - 1; i++)
            {
                cMsgs[iMsgLength - 1] += cMsgs[i];
            }

            SendMsg(cMsgs);
            //lock (CriticalSection)
            //{
            //    MsgQue.Enqueue(cMsgs);
            //}

        }
        /// <summary>
        /// 모터 감속 무시 긴급정지.
        /// </summary>
        public void EmgStop()
        {
            if (Stat.bJogN || Stat.bJogP)//조그 구동후에 조스스탑으로 안스면 같은방향 조그가 안먹는다.
            {
                JogStop();

                return;
            }
            // 150 0 80 2 16 0 98
            const int iMsgLength = 7;
            byte[] cMsgs = new byte[iMsgLength];

            cMsgs[0] = 150;//head
            cMsgs[1] = 0;//id
            cMsgs[2] = 80;//cmdid
            cMsgs[3] = 2;//data length
            cMsgs[4] = 16;//data0
            cMsgs[5] = 0;//data1

            cMsgs[iMsgLength - 1] = 0;
            for (int i = 2; i < iMsgLength - 1; i++)
            {
                cMsgs[iMsgLength - 1] += cMsgs[i];
            }

            SendMsg(cMsgs);
            //lock (CriticalSection)
            //{
            //    MsgQue.Enqueue(cMsgs);
            //}
        }


        public void SendSetSpeed(double _dSpeed)
        {
            //임시 상수.
            double dRpmSpeed = _dSpeed; // PulseToRPM(_dSpeed);// UnitToRPM(_dSpeed);
            int iRpmSpeed = (int)dRpmSpeed;

            //dRpmSpeed

            const int iMsgLength = 9;
            byte[] cMsgs = new byte[iMsgLength];

            cMsgs[0] = 150;//head
            cMsgs[1] = 0;//id
            cMsgs[2] = 52;//cmdid
            cMsgs[3] = 4;//data length
            cMsgs[4] = 0;//SpeedData1
            cMsgs[5] = 0;//SpeedData2
            cMsgs[6] = 0;//SpeedData3
            cMsgs[7] = 0;//SpeedData4

            byte[] cHexa = BitConverter.GetBytes(iRpmSpeed);

            for (int i = 0; i < 4; i++)
            {
                cMsgs[4 + i] = cHexa[i];
            }

            cMsgs[iMsgLength - 1] = 0;
            for (int i = 2; i < iMsgLength - 1; i++)
            {
                cMsgs[iMsgLength - 1] += cMsgs[i];
            }

            SendMsg(cMsgs);
            //lock (CriticalSection)
            //{
            //    MsgQue.Enqueue(cMsgs);
            //}

        }

        public void SendSetAccDcc(double _dAccDcc)
        {

            double dAccDcc = _dAccDcc; // PulseToJuni(_dAccDcc);
            // 150 0 86 2 100 0 188    100pro setting
            int iAccDcc = (int)dAccDcc;// _dAccDcc; // UnitToRPM(_dAccDcc);


            //dRpmSpeed

            const int iMsgLength = 9;
            byte[] cMsgs = new byte[iMsgLength];

            cMsgs[0] = 150;//head
            cMsgs[1] = 0;//id
            cMsgs[2] = 86;//cmdid
            cMsgs[3] = 4;//data length
            cMsgs[4] = (byte)(iAccDcc     & 0x00FF) ; 
            cMsgs[5] = (byte)(iAccDcc>>8  & 0x00FF) ; 
            cMsgs[6] = (byte)(iAccDcc>>16 & 0x00FF) ; 
            cMsgs[7] = (byte)(iAccDcc>>24 & 0x00FF) ; 

            cMsgs[iMsgLength - 1] = 0;
            for (int i = 2; i < iMsgLength - 1; i++)
            {
                cMsgs[iMsgLength - 1] += cMsgs[i];
            }

            SendMsg(cMsgs);
            //lock (CriticalSection)
            //{
            //    MsgQue.Enqueue(cMsgs);
            //}

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
            SendSetSpeed(_dVel);
            SendSetAccDcc(_dAcc);

            Stat.bJogP = true;
        }
        /// <summary>
        /// -방향 조그 이동
        /// </summary>
        /// <param name="_dVel">초당 구동 속도 펄스</param>
        /// <param name="_dAcc">구동 가속율 펄스</param>
        /// <param name="_dDec">감속율 펄스</param>
        public void JogN(double _dVel,double _dAcc,double _dDec)
        {
            SendSetSpeed(_dVel);
            SendSetAccDcc(_dAcc);

            Stat.bJogN = true;
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
            SendSetSpeed(_dVel);
            SendSetAccDcc(_dAcc);

            int iPos = (int)_dPos;

            const int iMsgLength = 9;
            byte[] cMsgs = new byte[iMsgLength];

            cMsgs[0] = 150;//head
            cMsgs[1] = 0;//id
            cMsgs[2] = 34;//cmdid
            cMsgs[3] = 4;//data length
            cMsgs[4] = 0;//SpeedData1
            cMsgs[5] = 0;//SpeedData2
            cMsgs[6] = 0;//SpeedData3
            cMsgs[7] = 0;//SpeedData4

            byte[] cHexa = BitConverter.GetBytes(iPos);

            for (int i = 0; i < 4; i++)
            {
                cMsgs[4 + i] = cHexa[i];
            }

            cMsgs[iMsgLength - 1] = 0;
            for (int i = 2; i < iMsgLength - 1; i++)
            {
                cMsgs[iMsgLength - 1] += cMsgs[i];
            }

            Stat.dTrgPos = _dPos;

            //야메다..... .
            Stat.bStop = false;

            SendMsg(cMsgs);
            //lock (CriticalSection)
            //{
            //    MsgQue.Enqueue(cMsgs);
            //}
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
        /// 현재 파라미터들을 아진 함수를 이용하여 세팅함.
        /// </summary>

        
        public void ApplyPara()
        {

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
        public void OneShotTrg(bool _bOnLevel,int _iTime)
        {
            
        }

        //모터 무빙시에 결과값이 들어오는시간이 오래걸리는데다가 스텝에다 실어넣으니 
        //자꾸 씹혀서 2번연속으로 움직일때 1번째무빙이 씹히는 경우가 있어서 동기화시키기 위해서 
        private void UpdateIO()
        {
            bRcvdMsg = false;
            SendGetIOStat();

            while (!bRcvdMsg)
            {
                
            }

            byte IOStat;
            
            IOStat = GetIOStat(RecvMsg);
            Stat.bHomeDone = (IOStat & 16) == 16;
            Stat.bAlram = (IOStat & 8) == 8;
            Stat.bInpos = (IOStat & 4) == 4;
            Stat.bStop = (IOStat & 2) != 2;
            Stat.bServo = (IOStat & 1) == 1;
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

        public void Update()
        {
            //if (Para.iPortNo == 7) return;
            //byte[] cMsgs;
            //while (MsgQue.Count > 0)
            //{
            //    lock (CriticalSection)
            //    {
            //        cMsgs = MsgQue.Dequeue();
            //    }

            //    SendMsg(cMsgs);
            //}
            //UpdateIO();

            if (m_tmUpdate.OnDelay(iPreUpdateStep == iUpdateStep, 100))
            {
                //Trace Log.
                string Msg;
                Msg = string.Format("m_tmUpdate : m_iStep=%d", iUpdateStep);
                Log.Trace("Juni", Msg);
                iUpdateStep = 0;
            }

            iPreUpdateStep = iUpdateStep;

            byte IOStat;
            switch (iUpdateStep)
            {
                default : 
                    iUpdateStep = 10 ;
                    break ;


                case 10 :
                         if (Stat.bJogN) SendJogN();
                    else if (Stat.bJogP) SendJogP();
                    iUpdateStep++;
                    break;

                case 11 :
                    bRcvdMsg = false;
                    sRecvMsg = "";
                    lRecvMsg.Clear();

                    SendGetEncPos();
                    iUpdateStep++;
                    break ;

                case 12 :
                    if (!bRcvdMsg) return;
                    if (lRecvMsg.Count != 9) return;
                    Stat.dEncPos = GetEncPos(GetListToByte(lRecvMsg));

                    if (bEncPos && Math.Abs(Stat.dEncPos) < 4000)
                    {
                        Log.Trace("ResetEncPos", Stat.dEncPos.ToString());
                        bEncPos = false;
                    }

                    bRcvdMsg = false;
                    sRecvMsg = "";
                    lRecvMsg.Clear();

                    SendGetCmdPos();
                    iUpdateStep++;
                    break;

                case 13:
                    if (!bRcvdMsg) return;
                    if (lRecvMsg.Count != 9) return;
                    Stat.dCmdPos = GetCmdPos(GetListToByte(lRecvMsg));

                    if (bCmdPos && Math.Abs(Stat.dCmdPos) < 4000)
                    {
                        Log.Trace("ResetCmdPos", Stat.dCmdPos.ToString());
                        bCmdPos = false;
                    }

                    bRcvdMsg = false;
                    sRecvMsg = "";
                    lRecvMsg.Clear();

                    SendGetTorque();

                    iUpdateStep++;
                    break;

                case 14:
                    if (!bRcvdMsg) return;
                    if (lRecvMsg.Count != 7) return;
                    Stat.dTorque = GetTorque(GetListToByte(lRecvMsg));

                    bRcvdMsg = false;
                    sRecvMsg = "";
                    lRecvMsg.Clear();

                    SendGetIOStat();
                    iUpdateStep++;
                    break;

                case 15:
                    if (!bRcvdMsg) return;
                    if (lRecvMsg.Count != 7) return;
                    IOStat = GetIOStat(GetListToByte(lRecvMsg));
                    Stat.bHomeDone = (IOStat & 16) == 16;
                    Stat.bAlram = (IOStat & 8) == 8;
                    Stat.bInpos = (IOStat & 4) == 4;
                    Stat.bStop = (IOStat & 2) != 2;
                    Stat.bServo = (IOStat & 1) == 1;                 
                    
                   
                    iUpdateStep=10;
                    break ;

                //알람
            }
        } 
        

        private int GetTorque(byte[] _sRcvMsg)
        {
            //7ea
            // 105 0 14 2 47 0 63
            if (!siPort.IsOpen) return 0;
            if (_sRcvMsg == null) return 0;
            int iMsgLength = _sRcvMsg.Length;

            if (_sRcvMsg.Length != 7)
            {
                return 0;
            }
            if (_sRcvMsg[2] != 14) return 0; //cmdid check

            byte CheckSum = 0;
            for (int i = 2; i < iMsgLength - 1; i++)
            {
                CheckSum += _sRcvMsg[i];
            }

            if (CheckSum != _sRcvMsg[iMsgLength - 1]) return 0;


            Int16 iRetValue = BitConverter.ToInt16(_sRcvMsg, 4);

            return iRetValue;
        }



        private int GetEncPos(byte[] _sRcvMsg)
        {
            // 105 0 16 4 0 160 0 0 180
            int iMsgLength = _sRcvMsg.Length;

            if (_sRcvMsg.Length != 9)
            {
                return 0;
            }
            if (_sRcvMsg[2] != 16) return 0; //cmdid check

            byte CheckSum = 0;
            for (int i = 2; i < iMsgLength - 1; i++)
            {
                CheckSum += _sRcvMsg[i];
            }

            if (CheckSum != _sRcvMsg[iMsgLength - 1]) return 0;//CheckSum Fail


            int iRetValue = BitConverter.ToInt32(_sRcvMsg, 4);

            return iRetValue;
        }

        private int GetCmdPos(byte[] _sRcvMsg)
        {
            // 105 0 226 4 0 0 0 0 230
            int iMsgLength = _sRcvMsg.Length;

            if (_sRcvMsg.Length != 9)
            {
                return 0;
            }
            if (_sRcvMsg[2] != 226) return 0; //cmdid check

            byte CheckSum = 0;
            for (int i = 2; i < iMsgLength - 1; i++)
            {
                CheckSum += _sRcvMsg[i];
            }

            if (CheckSum != _sRcvMsg[iMsgLength - 1]) return 0;//CheckSum Fail


            int iRetValue = BitConverter.ToInt32(_sRcvMsg, 4);

            return iRetValue;
        }

        private byte GetIOStat(byte[] _sRcvMsg)
        {
            // 105 0 28 2 0 17 47
            int iMsgLength = _sRcvMsg.Length;

            if (_sRcvMsg.Length != 7)
            {
                return 0;
            }
            if (_sRcvMsg[2] != 28) return 0; //cmdid check

            byte CheckSum = 0;
            for (int i = 2; i < iMsgLength - 1; i++)
            {
                CheckSum += _sRcvMsg[i];
            }

            if (CheckSum != _sRcvMsg[iMsgLength - 1]) return 0;//CheckSum Fail

            //17
            byte IOStat = _sRcvMsg[5];

            return IOStat;
        }

        private void SendJogP()
        {
            const int iMsgLength = 7;
            byte[] cMsgs = new byte[iMsgLength];

            cMsgs[0] = 150;//head
            cMsgs[1] = 0;//id
            cMsgs[2] = 38;//cmdid
            cMsgs[3] = 2;//data length
            cMsgs[4] = 2;//data0
            cMsgs[5] = 0;//data1

            cMsgs[iMsgLength - 1] = 0;
            for (int i = 2; i < iMsgLength - 1; i++)
            {
                cMsgs[iMsgLength - 1] += cMsgs[i];
            }

            SendMsg(cMsgs);
            //lock (CriticalSection)
            //{
            //    MsgQue.Enqueue(cMsgs);
            //}
        }

        private void SendJogN()
        {
            const int iMsgLength = 7;
            byte[] cMsgs = new byte[iMsgLength];

            cMsgs[0] = 150;//head
            cMsgs[1] = 0;//id
            cMsgs[2] = 38;//cmdid
            cMsgs[3] = 2;//data length
            cMsgs[4] = 1;//data0
            cMsgs[5] = 0;//data1

            cMsgs[iMsgLength - 1] = 0;
            for (int i = 2; i < iMsgLength - 1; i++)
            {
                cMsgs[iMsgLength - 1] += cMsgs[i];
            }

            SendMsg(cMsgs);
            //lock (CriticalSection)
            //{
            //    MsgQue.Enqueue(cMsgs);
            //}

        }

        public void SendGetTorque()
        {
            //150 0 14 0 14
            const int iMsgLength = 5;
            byte[] cMsgs = new byte[iMsgLength];

            cMsgs[0] = 150;//head
            cMsgs[1] = 0;//id
            cMsgs[2] = 14;//cmdid
            cMsgs[3] = 0;//data length            

            cMsgs[iMsgLength - 1] = 0;
            for (int i = 2; i < iMsgLength - 1; i++)
            {
                cMsgs[iMsgLength - 1] += cMsgs[i];
            }

            SendMsg(cMsgs);
            //lock (CriticalSection)
            //{
            //    MsgQue.Enqueue(cMsgs);
            //}
        }

        private void SendTorqueLimit(int _iTorqueLimit)
        {
            //150 0 170 2 0 14
            const int iMsgLength = 7;
            byte[] cMsgs = new byte[iMsgLength];

            cMsgs[0] = 150;//head
            cMsgs[1] = 0;//id
            cMsgs[2] = 170;//cmdid
            cMsgs[3] = 2;//data length             
            cMsgs[4] = (byte)(_iTorqueLimit    & 0x00FF) ; 
            cMsgs[5] = (byte)(_iTorqueLimit>>8 & 0x00FF) ; 

            cMsgs[iMsgLength - 1] = 0;
            for (int i = 2; i < iMsgLength - 1; i++)
            {
                cMsgs[iMsgLength - 1] += cMsgs[i];
            }

            SendMsg(cMsgs);
            //lock (CriticalSection)
            //{
            //    MsgQue.Enqueue(cMsgs);
            //}
        }

        private void SendTorqueTime(int _iTorqueTime)
        {
            // 150 0 172 2 50 0 224
            const int iMsgLength = 7;
            byte[] cMsgs = new byte[iMsgLength];

            cMsgs[0] = 150;//head
            cMsgs[1] = 0;//id
            cMsgs[2] = 172;//cmdid
            cMsgs[3] = 2;//data length 
            cMsgs[4] = (byte)(_iTorqueTime    & 0x00FF) ; 
            cMsgs[5] = (byte)(_iTorqueTime>>8 & 0x00FF) ; 

            cMsgs[iMsgLength - 1] = 0;
            for (int i = 2; i < iMsgLength - 1; i++)
            {
                cMsgs[iMsgLength - 1] += cMsgs[i];
            }

            SendMsg(cMsgs);
            //lock (CriticalSection)
            //{
            //    MsgQue.Enqueue(cMsgs);
            //}
        }

        
        private void SendTorqueMax(int _iTorqueMax)
        {
            //3000세팅시에.
            //  150 0 96 2 184 11 37 
            const int iMsgLength = 7;
            byte[] cMsgs = new byte[iMsgLength];

            cMsgs[0] = 150;//head
            cMsgs[1] = 0;//id
            cMsgs[2] = 96;//cmdid
            cMsgs[3] = 2;//data length 
            cMsgs[4] = (byte)(_iTorqueMax    & 0x00FF) ; 
            cMsgs[5] = (byte)(_iTorqueMax>>8 & 0x00FF) ; 

            cMsgs[iMsgLength - 1] = 0;
            for (int i = 2; i < iMsgLength - 1; i++)
            {
                cMsgs[iMsgLength - 1] += cMsgs[i];
            }

            SendMsg(cMsgs);
            //lock (CriticalSection)
            //{
            //    MsgQue.Enqueue(cMsgs);
            //}
        }

        public void SendGetIOStat()
        {
            // 150 0 28 0 28
            const int iMsgLength = 5;
            byte[] cMsgs = new byte[iMsgLength];

            cMsgs[0] = 150;//head
            cMsgs[1] = 0;//id
            cMsgs[2] = 28;//cmdid
            cMsgs[3] = 0;//data length            

            cMsgs[iMsgLength - 1] = 0;
            for (int i = 2; i < iMsgLength - 1; i++)
            {
                cMsgs[iMsgLength - 1] += cMsgs[i];
            }

            SendMsg(cMsgs);
            //lock (CriticalSection)
            //{
            //    MsgQue.Enqueue(cMsgs);
            //}
        }

        private void SendGetEncPos()
        {
            // 150 0 16 1 17
            if (!siPort.IsOpen)
            {
                return;
            }
            const int iMsgLength = 5;
            byte[] cMsgs = new byte[iMsgLength];

            cMsgs[0] = 150;//head
            cMsgs[1] = 0;//id
            cMsgs[2] = 16;//cmdid
            cMsgs[3] = 1;//data length

            cMsgs[iMsgLength - 1] = 0;
            for (int i = 2; i < iMsgLength - 1; i++)
            {
                cMsgs[iMsgLength - 1] += cMsgs[i];
            }

            SendMsg(cMsgs);
            //lock (CriticalSection)
            //{
            //    MsgQue.Enqueue(cMsgs);
            //}

        }

        private void SendGetCmdPos()
        {
            // 150 0 226 0 226
            const int iMsgLength = 5;
            byte[] cMsgs = new byte[iMsgLength];

            cMsgs[0] = 150;//head
            cMsgs[1] = 0;//id
            cMsgs[2] = 226;//cmdid
            cMsgs[3] = 1;//data length

            cMsgs[iMsgLength - 1] = 0;
            for (int i = 2; i < iMsgLength - 1; i++)
            {
                cMsgs[iMsgLength - 1] += cMsgs[i];
            }

            SendMsg(cMsgs);
            //lock (CriticalSection)
            //{
            //    MsgQue.Enqueue(cMsgs);
            //}

        }

        public void SendEncOffset(int  _iValue) //모터 회전수 만큼 되감는 동작 안하게하는 Offset Setting
        {
            // 150 0 106 2 161 15 28 //Encoder Offset 4001일때
            const int iMsgLength = 7;
            byte[] cMsgs = new byte[iMsgLength];

            cMsgs[0] = 150;//head
            cMsgs[1] = 0  ;//id
            cMsgs[2] = 106;//cmdid
            cMsgs[3] = 2  ;//data length
            cMsgs[4] = (byte)(_iValue & 0x00FF);
            cMsgs[5] = (byte)(_iValue >> 8 & 0x00FF); 

            cMsgs[iMsgLength - 1] = 0;
            for (int i = 2; i < iMsgLength - 1; i++)
            {
                cMsgs[iMsgLength - 1] += cMsgs[i];
            }

            SendMsg(cMsgs);
            //lock (CriticalSection)
            //{
            //    MsgQue.Enqueue(cMsgs);
            //}
        }

        private void SendInputEnable(int _iEnable)
        {
            //150 0 84 2 0 0 86 Input Enable
            //150 0 84 2 0 16 102 Input Disable
            const int iMsgLength = 7;
            byte[] cMsgs = new byte[iMsgLength];

            cMsgs[0] = 150;//head
            cMsgs[1] = 0;//id
            cMsgs[2] = 84;//cmdid
            cMsgs[3] = 2;//data length             
            cMsgs[4] = 0;// (byte)(iEnable & 0x00FF);
            cMsgs[5] = (byte)_iEnable;//(byte)(_iEnable >> 8 & 0x00FF);

            cMsgs[iMsgLength - 1] = 0;
            for (int i = 2; i < iMsgLength - 1; i++)
            {
                cMsgs[iMsgLength - 1] += cMsgs[i];
            }

            SendMsg(cMsgs);
            //lock (CriticalSection)
            //{
            //    MsgQue.Enqueue(cMsgs);
            //}
        }

        





        /// <summary>
        /// 해당모터축의 서브파라미터를 LoadSave함.
        /// </summary>
        /// <param name="_bLoad">true=로드 false=세이브</param>
        /// <returns>성공여부</returns>
        public bool LoadSave(bool _bLoad, string _sParaFolderPath, int _iMotrNo)
        {
            string sFilePath = _sParaFolderPath + "MotrJuni" + _iMotrNo.ToString() + ".xml";
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

        public void SetDoublePara(string _sParaName , double _dValue)
        {
            
            if (_sParaName == "EncoderOffset")
            {
                Log.Trace("BfResetEncPos", Stat.dEncPos.ToString());
                Log.Trace("BfResetCmdPos", Stat.dCmdPos.ToString());
                bCmdPos = true;
                bEncPos = true;
            }
                 if(_sParaName == "TorqueLockLimit"){SendTorqueLimit((int)_dValue) ;}
            else if(_sParaName == "TorqueLockTime" ){SendTorqueTime ((int)_dValue) ;}
            else if(_sParaName == "TorqueMax"      ){SendTorqueMax  ((int)_dValue) ;}
            else if(_sParaName == "EncoderOffset"  ){SendEncOffset  ((int)_dValue) ;}
            else if(_sParaName == "InputEnable"    ){SendInputEnable((int)_dValue) ;}
        }

        public int GetIntStat(string _sStatName)
        {
            if (_sStatName == "Torque") return GetTorque(RecvMsg);

            return 0;
        }


    }
}
