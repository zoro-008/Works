using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.CompilerServices; 
using System.Runtime.InteropServices;
using System.Reflection;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using System.Configuration;

namespace COMMON
{
    static class Constants
    {
        public const bool LOG_SERILOG = true ;
        public const bool LOG_MESSAGE = false;
    }
    public class ForContext
    {
        public const string Frm    = "Form"  ;
        public const string Dev    = "Device";
        public const string Sts    = "Status";
        public const string Dxi    = "DIn"   ;
        public const string Dyi    = "DOut"  ;
    }
    public class SERILOG
    {
        public static string Path    { get; } = ConfigurationManager.AppSettings["Serilog_Path"  ];
        public static string SeqUrl  { get; } = ConfigurationManager.AppSettings["Serilog_SeqUrl"];
        public static string Period  { get; } = ConfigurationManager.AppSettings["Serilog_Period"];
        public static string Ignore  { get; } = ConfigurationManager.AppSettings["Serilog_Ignore"];

        public static int    iIgnore = 0;
    }
    public class Log
    {
        //LOG_MESSAGE
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

        //COMMON
        private static List<string> _pIgnrList = new List<string>();
        static bool bStarted = false;

        //TODO :: 
        //기존 메세지 방식으로 하려면 태그를 숫자로 넣어 줘야 하는데 지금 문자로 다 바꿔놔서 추후 수정해야됨
        //지금은 딱히 어떻게 해야 할지 모르겟음 추후에 사용해야 되면 생각
        public static void Trace(string _sMsg, string _sTag = "", [CallerLineNumber] int sourceLineNumber = 0, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "")
        {
            if(Constants.LOG_SERILOG)
            {
                SendSerilog(_sMsg,_sTag,sourceLineNumber,memberName,sourceFilePath);
            }
            if(Constants.LOG_MESSAGE)
            {
                if (s_iLogManHandle == 0) return;
                string sMsg = _sMsg.Replace(",", "");
                string sTag = string.Format("{0:00}", _sTag);
                string sFullMsg = string.Format("{0}, {1} ,{2},{3},{4}", sTag, sMsg , sourceLineNumber, memberName, sourceFilePath );
                SendMessage(sFullMsg);
            }
        }

        public static void TraceListView(string _sMsg, string _sTag = "", [CallerLineNumber] int sourceLineNumber = 0, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "")
        {
            if(Constants.LOG_SERILOG)
            {
                SendSerilog(_sMsg,_sTag,sourceLineNumber,memberName,sourceFilePath);
            }
            if(Constants.LOG_MESSAGE)
            {
                if (s_iLogManHandle == 0) return;
                string sMsg = _sMsg.Replace(",", "");
                string sTag = string.Format("{0:00}", _sTag);
                string sFullMsg = string.Format("{0}, {1} ,{2},{3},{4}", sTag, sMsg , sourceLineNumber, memberName, sourceFilePath );
                SendMessage(sFullMsg);
            }
            if(lvMsg != null) lvMsg.Invoke(new SendMsg(ListMsg), new string[]{_sMsg});
        }

        public static void SendSerilog(string _sMsg, string _sPart = "", [CallerLineNumber] int sourceLineNumber = 0, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "")
        {
            //Start Check
            if(!bStarted) StartLogMan();
            //중복 삭제.
            if (SERILOG.iIgnore > 0)
            {
                while (_pIgnrList.Count > SERILOG.iIgnore) { _pIgnrList.RemoveAt(0); }

                for (int j = 0; j < _pIgnrList.Count; j++)
                {
                    if(_pIgnrList[j] == _sMsg + _sPart) return;
                }
                _pIgnrList.Add(_sMsg + _sPart);
            }
            //Init
            Serilog.Log.Logger
                .ForContext("Part"            ,_sPart          )
                .ForContext("CallerLineNumber",sourceLineNumber)
                .ForContext("CallerMemberName",memberName      )
                .ForContext("File"            ,Path.GetFileName(sourceFilePath))
                .Information(_sMsg);
        }

        static FormOk FrmOk = new FormOk();
        //static string sPreShowMessage = "";
        public static void ShowMessage(string _sHeader, string _sMsg, int _iTime = 0, [CallerLineNumber] int sourceLineNumber = 0, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "")
        {
            if(Constants.LOG_SERILOG)
            {
                SendSerilog(_sHeader + " " + _sMsg,"",sourceLineNumber,memberName,sourceFilePath);
            }
            if(Constants.LOG_MESSAGE)
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
            }
            FrmOk.ShowForm(_sHeader, _sMsg, _iTime);
            //sPreShowMessage = sHdr + sMsg + memberName;
        }

        static FormYesNo FrmYesNo = new FormYesNo();
        public static DialogResult ShowMessageModal(string _sHeader, string _sMsg, [CallerLineNumber] int sourceLineNumber = 0, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "")
        {
            if(Constants.LOG_SERILOG)
            {
                SendSerilog(_sHeader + " " + _sMsg,"",sourceLineNumber,memberName,sourceFilePath);
            }
            if(Constants.LOG_MESSAGE)
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
            }
            return FrmYesNo.ShowForm(_sHeader, _sMsg);

        }

        static string sPreShowMessageFunc = "";
        public static void ShowMessageFunc(string _sMsg, [CallerLineNumber] int sourceLineNumber = 0, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "")
        {
            if(Constants.LOG_SERILOG)
            {
                SendSerilog(_sMsg,"",sourceLineNumber,memberName,sourceFilePath);
            }
            if(Constants.LOG_MESSAGE)
            {
                string sMsg = _sMsg.Replace(",", "");
                string sTag = string.Format("{0:00}", 1);

                if (s_iLogManHandle != 0)
                {
                    string sFullMsg = string.Format("{0}, {1} ,{2} ,{3}, {4}", sTag,  sMsg, sourceLineNumber, memberName, sourceFilePath);
                    SendMessage(sFullMsg);
                }

                if (sPreShowMessageFunc == memberName + _sMsg) return;
                //MessageBox.Show(new Form { TopMost = true }, _sMsg, memberName);
                sPreShowMessageFunc = memberName + _sMsg;
            }
            FrmOk.ShowForm("Confirm",_sMsg,0);
        }

        public static void SendMessage(string _sMsg)
        {
            if(!bStarted) StartLogMan();

            byte[] buff = System.Text.Encoding.Default.GetBytes(_sMsg);
            int len = buff.Length;
            COPYDATASTRUCT cds;
            cds.dwData = (IntPtr)100;
            cds.cbData = len + 1;
            cds.lpData = _sMsg;
            SendMessage(s_iLogManHandle, WM_COPYDATA, 0, ref cds);
        }

        public static void CloseForm()
        {
            if (FrmOk    != null) FrmOk.CloseForm();
            if (FrmYesNo != null) FrmYesNo.CloseForm();
        }

        static System.Diagnostics.Process Process = new System.Diagnostics.Process();
        public static void StartLogMan()
        {
            if(bStarted) return;

            if(Constants.LOG_MESSAGE)
            {
                Process.StartInfo.FileName = LOG_MAN_NAME;
                try
                {
                    Process.Start();
                    //Process.WaitForInputIdle(1000);
                    Process.WaitForExit(1500);
                }
                catch (Exception e)
                {
                }
                //if (s_iLogManHandle == 0) s_iLogManHandle = FindWindow(sLOG_WIN_NAME, null);
                if (s_iLogManHandle == 0) s_iLogManHandle = FindWindow(null, LOG_WIN_NAME);
                if (s_iLogManHandle!=0) { Trace("<START>"); }
            }
            if(Constants.LOG_SERILOG)
            {
                int.TryParse(SERILOG.Ignore, out SERILOG.iIgnore);
                int.TryParse(SERILOG.Period, out int iPeriod    );
                if(iPeriod == 0) iPeriod = 30;

                Serilog.Log.Logger = new LoggerConfiguration()
                .WriteTo.Seq(SERILOG.SeqUrl)//.WriteTo.Seq("http://localhost:5341")
                .WriteTo.RollingFile(new JsonFormatter(),@SERILOG.Path + @"Log-{Date}.txt", LogEventLevel.Verbose,retainedFileCountLimit: iPeriod)
                .CreateLogger();

                
            }
            
            bStarted = true;
        }

        //public static void 
        public static void EndLogMan()
        {
            if (Constants.LOG_MESSAGE)
            {
                try
                {
                    Process.Kill();
                }
                catch (Exception e)
                {
                }
            }
            Trace("<EXIT>");//메시지방식에서 종료용으로 사용
        }

        private static ListView lvMsg = null ;
        public static void SetMessageListBox(ListView _lvMsg)
        {
            lvMsg = _lvMsg ;

            lvMsg.View = View.Details;
            lvMsg.GridLines = false;
            lvMsg.HeaderStyle = ColumnHeaderStyle.None ;
            lvMsg.Columns.Add("Message", lvMsg.Size.Width, HorizontalAlignment.Left);
            var PropError = lvMsg.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropError.SetValue(lvMsg, true, null);
        }
        delegate void SendMsg(string _sMsg);
        public void SendListMsg(string _sMsg)
        {
            
        }

        private static void ListMsg(string _sMsg)
        {
            string sMsg = DateTime.Now.ToString("HH:mm:ss ") + _sMsg ;
            lvMsg.Items.Add(sMsg);
            if(lvMsg.Items.Count > 300 ){
                lvMsg.Items.RemoveAt(0);
            }
            lvMsg.Items[lvMsg.Items.Count - 1].EnsureVisible();
        }

        public  void Clear()
        {
            lvMsg.Items.Clear();
        }
    }
}
