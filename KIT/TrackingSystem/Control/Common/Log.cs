using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.CompilerServices; 
using System.Runtime.InteropServices;

namespace COMMON
{
    public class Log
    {
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(int hWnd, int Msg, int wParam, ref COPYDATASTRUCT lParam);
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }

        [DllImport("user32.dll")]
        public static extern int FindWindow(string lpClassName, string lpWindowName);

        private static int  s_iLogManHandle = 0;
        const string LOG_MAN_NAME = "Log\\Log.exe";
        const string LOG_WIN_NAME = "Log";

        public const int WM_COPYDATA = 0x4A;

        //private static 
        public static void Trace(string _sHeader, string _sMsg, int _iTag = 0, [CallerLineNumber] int sourceLineNumber = 0, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "")
        {
            if (s_iLogManHandle == 0) return;
            string sHdr = _sHeader.Replace(",", "");
            string sMsg = _sMsg.Replace(",", "");
            string sTag = string.Format("{0:00}", _iTag);
            string sFullMsg = string.Format("{0}, {1} ,{2},{3},{4}", sTag, sHdr + " " + sMsg, sourceLineNumber, memberName, sourceFilePath);
            SendMessage(sFullMsg);
        }
        public static void Trace(string _sMsg, int _iTag = 0, [CallerLineNumber] int sourceLineNumber = 0, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "")
        {
            if (s_iLogManHandle == 0) return;
            string sMsg = _sMsg.Replace(",", "");
            string sTag = string.Format("{0:00}", _iTag);
            string sFullMsg = string.Format("{0}, {1} ,{2},{3},{4}", sTag, sMsg , sourceLineNumber, memberName, sourceFilePath );
            SendMessage(sFullMsg);
        }

        public static void TraceFunc(string _sHeader, string _sMsg, int _iTag = 0, [CallerLineNumber] int sourceLineNumber = 0, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "")
        {
            if (s_iLogManHandle == 0) return;
            string sHdr = _sHeader.Replace(",", "");
            string sMsg = _sMsg.Replace(",", "");
            string sTag = string.Format("{0:00}", _iTag);
            string sFullMsg = string.Format("{0}, {1}, {2}, {3}, {4}", sTag, sHdr + " " + sMsg, sourceLineNumber, memberName, sourceFilePath);
            SendMessage(sFullMsg);
        }
        public static void TraceFunc(string _sMsg, int _iTag = 0, [CallerLineNumber] int sourceLineNumber = 0, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "")
        {
            if (s_iLogManHandle == 0) return;
            string sMsg = _sMsg.Replace(",", "");
            string sTag = string.Format("{0:00}", _iTag);
            string sFullMsg = string.Format("{0}, {1}, {2}, {3}, {4}", sTag, sMsg, sourceLineNumber, memberName, sourceFilePath);
            SendMessage(sFullMsg);
        }

        static FormOk FrmOk = new FormOk();
        static string sPreShowMessage = "";
        public static void ShowMessage(string _sHeader, string _sMsg, int _iTime = 0, [CallerLineNumber] int sourceLineNumber = 0, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "")
        {
            string sHdr = _sHeader.Replace(",", "");
            string sMsg = _sMsg.Replace(",", "");
            string sTag = string.Format("{0:00}", 1);

            if (s_iLogManHandle != 0)
            {
                string sFullMsg = string.Format("{0}, {1} ,{2} ,{3}, {4}", sTag, sHdr + " " + sMsg, sourceLineNumber, memberName, sourceFilePath);
                SendMessage(sFullMsg);
            }

            //if(sPreShowMessage == sHdr + sMsg + memberName) return ;
            //MessageBox.Show(new Form{TopMost = true} ,_sMsg, _sHeader);//이 형식의 모든 공용 static멤버는 스레드로부터 안전합니다. 인터페이스 멤버는 스레드로부터 안전하지 않습니다.
            FrmOk.ShowForm(_sHeader, _sMsg, _iTime);
            //sPreShowMessage = sHdr + sMsg + memberName;
        }

        static FormYesNo FrmYesNo = new FormYesNo();
        public static DialogResult ShowMessageModal(string _sHeader, string _sMsg, [CallerLineNumber] int sourceLineNumber = 0, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "")
        {
            string sHdr = _sHeader.Replace(",", "");
            string sMsg = _sMsg.Replace(",", "");
            string sTag = string.Format("{0:00}", 1);

            if (s_iLogManHandle != 0)
            {
                string sFullMsg = string.Format("{0}, {1} ,{2} ,{3}, {4}", sTag, sHdr + " " + sMsg, sourceLineNumber, memberName, sourceFilePath);
                SendMessage(sFullMsg);
            }

            //if(sPreShowMessage == _sHeader + _sMsg) return ;
            //MessageBox.Show(new Form{TopMost = true} ,_sMsg, _sHeader);//이 형식의 모든 공용 static멤버는 스레드로부터 안전합니다. 인터페이스 멤버는 스레드로부터 안전하지 않습니다.
            //sPreShowMessage = _sHeader + _sMsg;
            return FrmYesNo.ShowForm(_sHeader, _sMsg);

        }

        static string sPreShowMessageFunc = "";
        public static void ShowMessageFunc(string _sMsg, [CallerLineNumber] int sourceLineNumber = 0, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "")
        {
            string sMsg = _sMsg.Replace(",", "");
            string sTag = string.Format("{0:00}", 1);

            if (s_iLogManHandle != 0)
            {
                string sFullMsg = string.Format("{0}, {1} ,{2} ,{3}, {4}", sTag,  sMsg, sourceLineNumber, memberName, sourceFilePath);
                SendMessage(sFullMsg);
            }

            //if (sPreShowMessageFunc == memberName + _sMsg) return;
            //MessageBox.Show(new Form { TopMost = true }, _sMsg, memberName);
            //sPreShowMessageFunc = memberName + _sMsg;
            FrmOk.ShowForm("Confirm",_sMsg,0);
        }

        public static void SendMessage(string _sMsg)
        {
            byte[] buff = System.Text.Encoding.Default.GetBytes(_sMsg);
            int len = buff.Length;
            COPYDATASTRUCT cds;
            cds.dwData = (IntPtr)100;
            cds.cbData = len + 1;
            cds.lpData = _sMsg;
            SendMessage(s_iLogManHandle, WM_COPYDATA, 0, ref cds);
        }

        //StartLogMan, EndLogMan 함수에 필요한거
        string sFileName = Path.GetFullPath(LOG_MAN_NAME); //saves the filename for future use
        static System.Diagnostics.Process Process = new System.Diagnostics.Process();

        public static void StartLogMan()
        {
            Process.StartInfo.FileName = LOG_MAN_NAME;
            try
            {
                Process.Start();
                //Process.WaitForInputIdle(1000);
                Process.WaitForExit(2000);
            }
            catch (Exception e)
            {
            }
            //if (s_iLogManHandle == 0) s_iLogManHandle = FindWindow(sLOG_WIN_NAME, null);
            if (s_iLogManHandle == 0) s_iLogManHandle = FindWindow(null, LOG_WIN_NAME);
            if (s_iLogManHandle!=0) { Trace("<START>"); }
        }

        //public static void 
        public static void EndLogMan()
        {
            try
            {
                Process.Kill();
            }
            catch (Exception e)
            {
            }
            Trace("<EXIT>");

        }
    }
}
