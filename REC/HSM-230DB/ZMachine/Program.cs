using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using COMMON;

namespace Machine
{
    static class Program
    {
        static public FormMain FrmMain;
        static public void SendListMsg(string _sMsg)
        {
            FrmMain.FrmOperation.SendListMsg(_sMsg);
        }
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        /// 
        [STAThread]
        static void Main()
         {
            bool bFirst;
            Mutex mMutex = new Mutex(true, "Machine", out bFirst);

            

            if (bFirst)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                FrmMain = new FormMain();
                Application.Run(FrmMain);
            }
            else
            {
                Log.ShowMessage("ERROR", "Machine Running Already");
            }
            
        }
    }
}
