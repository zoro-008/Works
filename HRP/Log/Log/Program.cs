using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Log
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool bnew;
            Mutex mutex = new Mutex(true, "MutexLog", out bnew);
            if (bnew)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new LogMain());
                mutex.ReleaseMutex();
            }
            else
            {
                //MessageBox.Show("Program is running","Confirm");
                Application.Exit();
            }

            //Application.Run(new LogMain());
        }
    }
}
