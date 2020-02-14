using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;
using SML;
using COMMON;

namespace Machine
{

    public class VC
    {


        //클래스 밖에서 사용할때 아이디 Vision Send Message
        public enum sm : uint
        {
            None   = 0,
            Ready  = 1,
            Busy   = 2,
            Reset  = 3,
            Change = 4,
            Insp   = 5, //개별검사...    
            End    = 6,
        };

        //클래스 밖에서 사용할때 아이디 Vision Receive Message
        public enum rm : uint
        {
            None = 0,
            Insp = 1, //하드웨어 트리거 사용시에는 비젼쪽이 프로토콜 먼저시작함.
        }


        //Vision Status.
        //Message Id.
        //이것은 비젼쪽과 협의 해서 만듬.
        //Message Code
        private enum mc : uint
        {
            NONE   = 0,
            OK     = 1000,
            NG     = 1001,
            BUSY   = 1002,
            READY  = 1003,
            CHANGE = 1004,
            RESULT = 1005,
            INSP   = 1006,
            RESET  = 1007,
            END    = 1008,

        };

        [DllImport("User32.dll")]
        private static extern IntPtr FindWindow(String lpClassName, String lpWindowName);
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, ref COPYDATASTRUCT lParam);
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
            public void Clear()
            {
                dwData = (IntPtr)mc.NONE;
                cbData = 0;
                lpData = "";
            }
        }

        //보내기 받기 데이터.
        private struct SendMsgStat
        {
            public  sm     MsgId    ;
            public  int    iStep    ;
            public  int    iPreStep ;
            public  string sMsg     ; //최종적으로 주고받고 했을때 내가 받아야하는 데이터.
            public  string sSubMsg  ; //메세지 아이디와 같이 보내줘야 하는 메세지. 잡체인지 할때 잡파일을 실어넣어줘야 한다.
            public  bool   bEnded   ;
            public  string sErrMsg  ;
            public void Clear()
            {
                MsgId    = sm.None ;
                iStep    = 0 ;
                iPreStep = 0 ;
                sMsg     = "" ;
                sSubMsg  = "" ;
                bEnded   = false ;
                sErrMsg  = "";
            }
            public void CycleInit()
            {
                iStep = 10;
                iPreStep = 0;
            }
        }
        private struct RecvMsgStat
        {
            public  mc     MsgId    ;
            public int     iStep    ;
            public int     iPreStep ;
            public string  sMsg     ;//최종적으로 주고받고 했을때 내가 받아야하는 데이터. 결과값등등..
            public  bool   bEnded   ;
            public  string sErrMsg  ;
            public void Clear()
            {
                MsgId = mc.NONE;
                iStep = 0;
                iPreStep = 0;
                sMsg = "";
                bEnded = false;
                sErrMsg = "";
            }
            public void CycleInit()
            {
                iStep = 10;
                iPreStep = 0;
            }
        }

        //메세지 사이클의 상태를 나타냄.
        static private SendMsgStat SendStat;
        static private RecvMsgStat RecvStat;

        //Master Viewer.
        static private COPYDATASTRUCT RecvMstView;

        //메세지 사이클 안에서 받는 스트럭쳐.
        static private COPYDATASTRUCT RecvData;


        static private CDelayTimer m_tmTimeOut;



        //통신 상태.확인용.
        //public static bool m_bOK     ;
        //public static bool m_bNG     ;
        //public static bool m_bBUSY   ;
        //public static bool m_bREADY  ;
        //public static bool m_bCHANGE ;
        //public static bool m_bRESULT ;       

        static public void Init()
        {
            SendStat.Clear();
            RecvStat.Clear();

            m_tmTimeOut = new CDelayTimer();
            m_tmTimeOut.Clear();


        }

        //내부에서 쓰는것 밖에서 볼필요 없음.
        static private void SendMsg(mc _MsgId, string _sSubMsg = "")
        {
            //Vision Program 이름.
            const int WM_COPYDATA = 0x4A;
            const string VsName = "HPS";

            IntPtr hwnd = FindWindow(null, VsName);
            //IntPtr hwnd = EmbededExe.CameraProcess.MainWindowHandle;

            //IntPtr hwnd = FindWindow(null, VsName);

            if ((int)hwnd <= 0)
            {
                Log.ShowMessage("Vision Program Error", "Vision Program not Exist!");
                return;
            }

            COPYDATASTRUCT SendData;
            SendData.dwData = (IntPtr)_MsgId;
            SendData.cbData = _sSubMsg.Length + 1;
            SendData.lpData = _sSubMsg;

            RecvData.Clear();

            SendMessage(hwnd, WM_COPYDATA, IntPtr.Zero, ref SendData);
        }

        //내부에서 쓰는것 밖에서 볼필요 없음.
        static public void SendMsgTest(int _MsgId, string _sSubMsg = "")
        {
            //Vision Program 이름.
            const uint WM_COPYDATA = 0x4A;
            const string VsName = "HPS";

            IntPtr hwnd = FindWindow(null, VsName);
            //IntPtr hwnd = EmbededExe.CameraProcess.MainWindowHandle;

            //IntPtr hwnd = FindWindow(null, VsName);

            if ((int)hwnd <= 0)
            {
                Log.ShowMessage("Vision Program Error", "Vision Program not Exist!");
                return;
            }

            COPYDATASTRUCT SendData;
            SendData.dwData = (IntPtr)_MsgId;
            //SendData.cbData = _sSubMsg.Length;
            //기존에는 위에꺼.. 근데 +1은 왜 들어갔는지 모르것다.. 
            //+1이 들어가야지만 비전쪽에서 메시지를 받음..
            //JS
            SendData.cbData = _sSubMsg.Length + 1;
            
            SendData.lpData = _sSubMsg;

            RecvData.Clear();

            SendMessage(hwnd, WM_COPYDATA, IntPtr.Zero, ref SendData);
            //System.Diagnostics.Process[] pro = System.Diagnostics.Process.GetProcessesByName("HPS");

            //IntPtr ptTargerWindowsHandle = (IntPtr)FindWindow(null, "HPS");
            ////if (pro.Length > 0)
            ////{
            //    byte[] buff = System.Text.Encoding.Default.GetBytes(_MsgId.ToString());
            //
            //    COPYDATASTRUCT cds = new COPYDATASTRUCT();
            //    cds.dwData = (IntPtr)Convert.ToInt32(_MsgId);
            //    cds.cbData = _sSubMsg.Length + 1;
            //    cds.lpData = _sSubMsg;
            //
            //    //SendMessage(pro[0].MainWindowHandle, WM_COPYDATA, 0, ref cds);
            //    SendMessage(ptTargerWindowsHandle, WM_COPYDATA, IntPtr.Zero, ref cds);
            //    RecvData.Clear();
            

                //SendMessage(ptTargerWindowsHandle, WM_COPYDATA, IntPtr.Zero, ref cds);
        }


        static public void Update()
        {
            string sTemp;
            switch (SendStat.MsgId)
            {
                default: sTemp = string.Format("UnDefined Message Sended");
                    SM.ER.SetErrMsg((int)ei.VSN_ComErr, sTemp);
                    SendStat.MsgId = sm.None; SendStat.bEnded = true; break;
                case sm.None: break;
                case sm.Ready : if (CycleSendReady ()) { SendStat.MsgId = sm.None; SendStat.bEnded = true; } break;
                case sm.Busy  : if (CycleSendBusy  ()) { SendStat.MsgId = sm.None; SendStat.bEnded = true; } break;
                case sm.Reset : if (CycleSendReset ()) { SendStat.MsgId = sm.None; SendStat.bEnded = true; } break;
                case sm.Change: if (CycleSendChange()) { SendStat.MsgId = sm.None; SendStat.bEnded = true; } break;
                case sm.Insp  : if (CycleSendInsp  ()) { SendStat.MsgId = sm.None; SendStat.bEnded = true; } break;
                case sm.End   : if (CycleSendEnd   ()) { SendStat.MsgId = sm.None; SendStat.bEnded = true; } break;
            }

            switch (RecvStat.MsgId)
            {
                default: sTemp = string.Format("UnDefined Message Received");
                    SM.ER.SetErrMsg((int)ei.VSN_ComErr, sTemp);
                    RecvStat.MsgId = mc.NONE; RecvStat.bEnded = true; break;
                case mc.NONE   : break;
                case mc.RESULT : if (CycleRecvInsp()) { RecvStat.MsgId = mc.NONE; RecvStat.bEnded = true; } break;

            }
        }

        static private bool CycleSendReady()
        {
            string sTemp;
            if (m_tmTimeOut.OnDelay(SendStat.iStep != 0 && SendStat.iStep == SendStat.iPreStep, 500))
            {
                SendStat.sErrMsg = string.Format("Cycle iSendStep={0:00} TimeOut", SendStat.iStep);
                SendStat.iStep = 0;
                return true;
            }

            if (SendStat.iStep != SendStat.iPreStep)
            {
                sTemp = string.Format("Cycle iSendStep={0:00}", SendStat.iStep);
                Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name, sTemp);
            }

            SendStat.iPreStep = SendStat.iStep;

            switch (SendStat.iStep)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear iSendStep={0:00}", SendStat.iStep);
                    Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name, sTemp);
                    SendStat.iStep = 0;
                    return true;

                case 10:
                    SendMsg(mc.READY, mc.READY.ToString());
                    SendStat.iStep++;
                    return false;

                case 11:
                    if (RecvData.dwData == (IntPtr)mc.NONE) return false;
                    SendStat.sMsg = RecvData.lpData;
                    SendStat.iStep = 0;
                    return true;
            }
        }
        static private bool CycleSendBusy()
        {
            string sTemp;
            if (m_tmTimeOut.OnDelay(SendStat.iStep != 0 && SendStat.iStep == SendStat.iPreStep, 500))
            {
                SendStat.sErrMsg = string.Format("Cycle iSendStep={0:00} TimeOut", SendStat.iStep);
                SendStat.iStep = 0;
                return true;
            }

            if (SendStat.iStep != SendStat.iPreStep)
            {
                sTemp = string.Format("Cycle iSendStep={0:00}", SendStat.iStep);
                Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name, sTemp);
            }

            SendStat.iPreStep = SendStat.iStep;

            switch (SendStat.iStep)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear iSendStep={0:00}", SendStat.iStep);
                    Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name, sTemp);
                    SendStat.iStep = 0;
                    return true;

                case 10:
                    SendMsg(mc.BUSY, mc.BUSY.ToString());
                    SendStat.iStep++;
                    return false;

                case 11:
                    if (RecvData.dwData == (IntPtr)mc.NONE) return false;
                    SendStat.sMsg = RecvData.lpData;
                    SendStat.iStep = 0;
                    return true;
            }
        }
        static private bool CycleSendReset()
        {
            string sTemp;
            if (m_tmTimeOut.OnDelay(SendStat.iStep != 0 && SendStat.iStep == SendStat.iPreStep, 2000))
            {
                SendStat.sErrMsg = string.Format("Cycle iSendStep={0:00} TimeOut", SendStat.iStep);
                SendStat.iStep = 0;
                return true;
            }

            if (SendStat.iStep != SendStat.iPreStep)
            {
                sTemp = string.Format("Cycle iSendStep={0:00}", SendStat.iStep);
                Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name, sTemp);
            }

            SendStat.iPreStep = SendStat.iStep;

            switch (SendStat.iStep)
            {
                default:
                    sTemp = string.Format("Cycle Default Clear iSendStep={0:00}", SendStat.iStep);
                    Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name, sTemp);
                    SendStat.iStep = 0;
                    return true;

                case 10:
                    SendMsg(mc.RESET, mc.RESET.ToString());
                    SendStat.iStep++;
                    return false;

                case 11:
                    if (RecvData.dwData != (IntPtr)mc.OK) return false;
                    SendStat.sMsg = RecvData.lpData;

                    //2번째 전송...
                    //비전쪽에 리셑은 2번 보내야 함.
                    //SendStat.Clear();
                    SendMsg(mc.RESET, mc.RESET.ToString());
                    SendStat.iStep++;
                    return false;

                case 12:
                    if (RecvData.dwData != (IntPtr)mc.OK) return false;
                    SendStat.sMsg = RecvData.lpData;
                    SendStat.iStep = 0;
                    return true;
            }
        }

        //원래 래디를 따로 태우고 체인지를 보내야 하는데 
        //보통 UI에서 하기때문에 그냥 싸이클에  레디를 같이 넣음.
        static private bool CycleSendChange()
        {
            string sTemp;
            if (m_tmTimeOut.OnDelay(SendStat.iStep != 0 && SendStat.iStep == SendStat.iPreStep, 500))
            {
                SendStat.sErrMsg = string.Format("Cycle iSendStep={0:00} TimeOut", SendStat.iStep);
                SendStat.iStep = 0;
                return true;
            }

            if (SendStat.iStep != SendStat.iPreStep)
            {
                sTemp = string.Format("Cycle iSendStep={0:00}", SendStat.iStep);
                Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name, sTemp);
            }

            SendStat.iPreStep = SendStat.iStep;

            switch (SendStat.iStep)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear iSendStep={0:00}", SendStat.iStep);
                    Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name, sTemp);
                    SendStat.iStep = 0;
                    return true;

                case 10: //Ready Check
                    SendMsg(mc.CHANGE, SendStat.sSubMsg);
                    SendStat.iStep++;
                    return false;

                case 11:
                    if (RecvData.dwData == (IntPtr)mc.NONE) return false;
                    SendStat.sMsg = RecvData.lpData;

                    if (RecvData.dwData == (IntPtr)mc.OK)
                    {
                        SendMsg(mc.CHANGE, SendStat.sSubMsg);
                        SendStat.iStep++;
                        return false; 
                        
                    }
                    SendStat.iStep = 0;
                    return true;
                    

                case 12:
                    if (RecvData.dwData == (IntPtr)mc.NONE) return false;
                    SendStat.sMsg = RecvData.lpData;
                    SendStat.iStep = 0;
                    return true;
            }
        }

        //시퀜스에서 돌리기 때문에 레디는 시퀜스에서 적절할때 하고 검사만 한다.
        static private bool CycleSendInsp()
        {
            string sTemp;
            if (m_tmTimeOut.OnDelay(SendStat.iStep != 0 && SendStat.iStep == SendStat.iPreStep, 5000))
            {
                SendStat.sErrMsg = string.Format("Cycle iSendStep={0:00} TimeOut", SendStat.iStep);
                SendStat.iStep = 0;
                return true;
            }

            if (SendStat.iStep != SendStat.iPreStep)
            {
                sTemp = string.Format("Cycle iSendStep={0:00}", SendStat.iStep);
                Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name, sTemp);
            }

            SendStat.iPreStep = SendStat.iStep;

            switch (SendStat.iStep)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear iSendStep={0:00}", SendStat.iStep);
                    Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name, sTemp);
                    SendStat.iStep = 0;
                    return true;

                case 10:
                    SendMsg(mc.INSP, SendStat.sSubMsg);
                    SendStat.iStep++;
                    return false;

                case 11:
                    if (RecvData.dwData == (IntPtr)mc.NONE) return false;
                    SendStat.sMsg = RecvData.lpData;
                    SendStat.iStep = 0;
                    return true;
            }
        }
        static private bool CycleSendEnd()
        {
            string sTemp;
            if (m_tmTimeOut.OnDelay(SendStat.iStep != 0 && SendStat.iStep == SendStat.iPreStep, 5000))
            {
                SendStat.sErrMsg = string.Format("Cycle iSendStep={0:00} TimeOut", SendStat.iStep);
                SendStat.iStep = 0;
                return true;
            }

            if (SendStat.iStep != SendStat.iPreStep)
            {
                sTemp = string.Format("Cycle iSendStep={0:00}", SendStat.iStep);
                Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name, sTemp);
            }

            SendStat.iPreStep = SendStat.iStep;

            switch (SendStat.iStep)
            {

                default:
                    sTemp = string.Format("Cycle Default Clear iSendStep={0:00}", SendStat.iStep);
                    Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name, sTemp);
                    SendStat.iStep = 0;
                    return true;

                case 10:
                    SendMsg(mc.END, mc.RESET.ToString());
                    SendStat.iStep++;
                    return false;

                case 11:
                    if (RecvData.dwData != (IntPtr)mc.OK) return false;
                    SendStat.sMsg = RecvData.lpData;
                    SendStat.iStep = 0;
                    return true;
            }
        }
        static private bool CycleRecvInsp()
        {
            string sTemp;
            if (m_tmTimeOut.OnDelay(RecvStat.iStep != 0 && RecvStat.iStep == RecvStat.iPreStep, 5000))
            {
                RecvStat.sErrMsg = string.Format("Cycle iSendStep={0:00} TimeOut", RecvStat.iStep);
                RecvStat.iStep = 0;
                return true;
            }

            if (RecvStat.iStep != RecvStat.iPreStep)
            {
                sTemp = string.Format("Cycle iSendStep={0:00}", RecvStat.iStep);
                Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name, sTemp);
            }

            RecvStat.iPreStep = RecvStat.iStep;
            switch (RecvStat.iStep)
            {
                default:
                    sTemp = string.Format("Cycle Default Clear iSendStep={0:00}", RecvStat.iStep);
                    Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name, sTemp);
                    RecvStat.iStep = 0;
                    return true;

                case 10:
                    RecvStat.sMsg = RecvData.lpData;
                    SendMsg(mc.OK);
                    RecvStat.iStep = 0;
                    return true;
            }
        }
        
        //밖에서 쓰는 퍼블릭 모음.
        static public void SetReceivedMsg(COPYDATASTRUCT _DataMsg)
        {
            RecvData.Clear();
            RecvData = _DataMsg;

            if (SendStat.MsgId == sm.None && RecvData.dwData == (IntPtr)mc.RESULT)
            {
                RecvStat.Clear();
                RecvStat.CycleInit();
                //RecvStat    .MsgId  = (mc)RecvData.dwData;
                RecvStat    .MsgId  = (mc)RecvData.dwData;
                RecvStat    .bEnded = false;
                RecvMstView.lpData  = RecvData.lpData;
            }
            else
            {
                RecvMstView.dwData = RecvData.dwData;
                RecvMstView.lpData = RecvData.lpData;
                RecvStat.bEnded = false;
            }
        }

        //비젼쪽 센드 관련.
        static public void SendVisnMsg(sm _MsgStat, string _sSubMsg = "")
        {
            if (SendStat.iStep != 0) return;
            if (RecvStat.iStep != 0) return;
            SendStat.Clear();
            SendStat.CycleInit();
                        
            SendStat.MsgId = _MsgStat;
            SendStat.sSubMsg = _sSubMsg;
        }
        static public bool IsEndSendMsg()//메세지 주고받기 끝났는지 확인 하는 함수.
        {
            return SendStat.bEnded;
        }
        static public string GetVisnSendMsg() //결과 메세지 확인 하는 함수.
        {
            return SendStat.sMsg;
        }
        static public string GetVisnSendErrMsg()
        {
            return SendStat.sErrMsg;
        }

        //비젼쪽 리시브 관련.
        static public void ClearRecvData()//검사가 예상되기 전에 미리 클리어 해놔야 한다.
        {
            RecvStat.Clear();
        }
        //메세지 주고받기 끝났는지 확인 하는 함수.
        static public bool IsRecvMsg()//그래야 이놈으로 검사 결과값이 들어왔는지 확인 가능.
        {
            return RecvStat.bEnded;
        }
        static public string GetVisnRecvMsg()
        {
            return RecvStat.sMsg;
        }
        static public string GetVisnRecvErrMsg()
        {
            return RecvStat.sErrMsg;
        }
        static public int GetVisnRecvViewMsgId()
        {
            return (int)RecvData.dwData;
        }
        static public string GetVisnRecvViewMsg()
        {
            return RecvData.lpData;
        }

        static public string GetVisnEndMsg()
        {
            return RecvStat.sMsg;
        }
    }
}
