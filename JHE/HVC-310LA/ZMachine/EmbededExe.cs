using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using COMMON;


namespace Machine
{
    static public class EmbededExe
    {
        
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string IPClassName, String IpWindowName);

        [DllImport("User32.dll", EntryPoint = "SetForegroundWindow")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        static public Process CameraProcess;
        static IntPtr procHandler;

        const string ProcessName = "HPS";

        static public void CameraInit()
        {
            System.Diagnostics.Process[] process = System.Diagnostics.Process.GetProcessesByName(ProcessName);//"AmCap");
            if (process.Length > 0)
            {
                string procTitle = null;
                procTitle = process[0].MainWindowTitle;
                procHandler = FindWindow(null, procTitle);
                SetForegroundWindow(procHandler);

                //close procceses
                try
                {
                    Process currentProcess = Process.GetCurrentProcess();
                    foreach (Process proc in process)
                    {
                        if (proc.Id != currentProcess.Id)
                        {
                            //proc.Kill();
                            CameraProcess = Process.Start("C:\\JHLensAISystem\\LensInspection\\NeptuneReset\\NeptuneReset.exe", "HPS.exe");
                            //CameraProcess = Process.Start("C:\\JHLensAISystem\\LensInspection\\NeptuneReset\\NeptuneReset.exe", "HPS.exe"); 
                            Thread.Sleep(500);

                            //CameraProcess.WaitForInputIdle(10000);
                        }


                    }
                }
                catch (Exception _e)
                {
                    Log.Trace("Error", "NeptuneReset Failed!");
                }
            }

            try
            {
                //CameraProcess = Process.Start("Microscope.exe");
                //CameraProcess = Process.Start("C:\\JHLensAISystem\\LensInspection\\HPS.exe");//"AmCap.exe");
                
                
                                             //C:\JHLensAISystem\LensInspection
                CameraProcess = Process.Start("C:\\JHLensAISystem\\LensInspection\\"+ProcessName+".exe");//"AmCap.exe");
                CameraProcess.WaitForInputIdle(1000);
                procHandler = CameraProcess.MainWindowHandle;
            }
            catch
            {
                Log.ShowMessage("Error", "Camera Init Failed!");

                return ;
            }



            //int delay = 0 ;
            //while(FindWindow(null,"HPS")==null)
            //{
            //    delay++;
            //    Thread.Sleep(100);

            //    if (delay > 100)
            //    {
            //        break ;
            //    }

            //}

            //Thread.

            
             int delay = 0 ;
            while(CameraProcess.MainWindowHandle.ToInt32()==0)
            {
                delay++;
                Thread.Sleep(100);
                CameraProcess.Refresh();//이거 수행안해주면 CameraProcess.MainWindowHandle 이놈이 갱신 안된다.

                if (delay > 100)
                {
                    break ;
                }

            }
             
            
            
        }

        static public int GetHPSHandle()
        {
            if ((int)procHandler == 0) return -1;
            return (int)procHandler;
        }

        static private bool CheckWindow()
        {
            System.Diagnostics.Process[] process = System.Diagnostics.Process.GetProcessesByName(ProcessName);//"AmCap");
            if (process.Length > 0)
            {   
                return true;
            }
            return false;
        }
        static public void SetCamParent(IntPtr _Handle)
        {
            if (CameraProcess == null) return ;
            if (!CheckWindow()) CameraInit();
            SetParent(CameraProcess.MainWindowHandle, _Handle);

            const int SW_SHOWMAXIMIZED = 3;
            ShowWindow(CameraProcess.MainWindowHandle, SW_SHOWMAXIMIZED);

        }

        static public void CameraClose()
        {
            try
            {
                CameraProcess.Kill();

            }
            catch
            {
                Log.Trace("Error", "Camera Close Failed!");
            }

            //CameraProcess.Dispose();
        }

        static Process KeyboardProcess;
        static public void ShowKeyboard()
        {
            System.Diagnostics.Process[] process = System.Diagnostics.Process.GetProcessesByName("osk");
            if (process.Length > 0)
            {
                string procTitle = null;
                procTitle = process[0].MainWindowTitle;
                IntPtr procHandler = FindWindow(null, procTitle);
                SetForegroundWindow(procHandler);
            }
            else
            {
                try
                {
                    //CameraProcess = Process.Start("Microscope.exe");
                    KeyboardProcess = Process.Start("osk.exe");
                    KeyboardProcess.WaitForInputIdle();
                }
                catch
                {
                    Log.ShowMessage("Error", "Keyboard Show Failed!");
                }
            }

        }







        //System.Diagnostics.Process[] process = System.Diagnostics.Process.GetProcessesByName("osk");
        ////System.Diagnostics.Process[] process = System.AppDomain.CurrentDomain.BaseDirectory + .GetProcessesByName("osk");
        ////
        //if (process.Length > 0)
        //{
        //    string procTitle = null;
        //    procTitle = process[0].MainWindowTitle;
        //    IntPtr procHandler = FindWindow(null, procTitle);
        //    SetForegroundWindow(procHandler);


        //    Process currentProcess = Process.GetCurrentProcess();
        //    foreach (Process proc in process)
        //    {
        //        if (proc.Id != currentProcess.Id)
        //            proc.Kill();
        //    }
        //}
        //else
        //{
        //    //Process.Start("C:\\Windows\\system32\\osk.exe");
        //    Process.Start("osk.exe");
        //}

        //System.Diagnostics.Process[] process = System.Diagnostics.Process.GetProcessesByName("Microscope");



        //if (process.Length > 0)
        //{

        //    // 윈도우 핸들러
        //    //string procTitle = null;
        //    //procTitle = process[0].MainWindowTitle;
        //    //IntPtr procHandler = FindWindow(null, procTitle);
        //    //// 활성화
        //    //ShowWindow(procHandler, SW_SHOWMAXVIEW);
        //    //SetForegroundWindow(procHandler);

        //    //string procTitle = null;
        //    //procTitle = process[0].MainWindowTitle;
        //    //IntPtr procHandler = FindWindow(null, procTitle);
        //    //SetForegroundWindow(procHandler);

        //}
        //else
        //{
        //    //Process.Start("C:\\Program Files\\DigiBird\\DigiView OPT-230KR\\Microscope.exe");
        //    CameraHide();

        //}
        //}
    }
}
