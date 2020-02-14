using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using COMMON;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace Machine
{
    public class EmbededExe
    {
        
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string IPClassName, String IpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("User32.dll", EntryPoint = "SetForegroundWindow")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        internal struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public ShowWindowCommands showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }

        internal enum ShowWindowCommands : int
        {
            Hide = 0,
            Normal = 1,
            Minimized = 2,
            Maximized = 3,
        }

        internal enum WNDSTATE : int
        {
            SW_HIDE = 0,
            SW_SHOWNORMAL = 1,
            SW_NORMAL = 1,
            SW_SHOWMINIMIZED = 2,
            SW_MAXIMIZE = 3,
            SW_SHOWNOACTIVATE = 4,
            SW_SHOW = 5,
            SW_MINIMIZE = 6,
            SW_SHOWMINNOACTIVE = 7,
            SW_SHOWNA = 8,
            SW_RESTORE = 9,
            SW_SHOWDEFAULT = 10,
            SW_MAX = 10
        }

        [DllImport("user32.dll")]
        private static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);
        
        //[DllImport("user32.dll")]
        //internal static extern bool GetWindowPlacement(int hWnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);



        Process process = new Process();
        IntPtr  procHandle ;

        string  procName;
        string  exeRoot;
        string  mainWinClassName ;

        public EmbededExe(string _sProcName , string _sMainWinClassName, string _sExeRoot)
        {
            procName = _sProcName ;
            mainWinClassName = _sMainWinClassName ;
            exeRoot = _sExeRoot ;
        }
        
        public void Init()
        {
            process = new Process();
            process.StartInfo.FileName = exeRoot;
            //process.StartInfo.UseShellExecute = true ;
            process.StartInfo.WorkingDirectory = Path.GetDirectoryName(exeRoot);

            Process[] processes = Process.GetProcessesByName(procName);//"AmCap");
            if (processes.Length > 0)
            {
                process = processes[0];
                process.Refresh();
            }
            else
            {
                try
                {
                    //process.
                    bool bRet = process.Start();
                    process.WaitForInputIdle(5000);
                    while (FindWindow(mainWinClassName, null) == IntPtr.Zero)
                    {
                        Thread.Sleep(100);
                        process.Refresh();
                    }
                    //procHandle = process.MainWindowHandle;

                }
                catch (Exception _e)
                {
                    Log.ShowMessage("Error", _e.Message);

                    return;
                }
            }

            Thread.Sleep(1000);
            process.Refresh();

            string procTitle = process.MainWindowTitle;
            //procHandle = FindWindow(null, "Ocean Optics SpectrSuite");
            //procHandle = FindWindow(mainWinClassName, "Ocean Optics SpectrSuite");
            procHandle = FindWindow(mainWinClassName, null);
            SetForegroundWindow(procHandle);

            WINDOWPLACEMENT wInf = new WINDOWPLACEMENT();
            wInf.length = System.Runtime.InteropServices.Marshal.SizeOf(wInf);
            bool bRet2 = GetWindowPlacement(procHandle , ref wInf);
            wInf.showCmd = ShowWindowCommands.Normal ;
            wInf.ptMaxPosition.X = 1921 ;
            wInf.ptMaxPosition.Y = 0    ;
            wInf.rcNormalPosition.X = 1921 ;
            wInf.rcNormalPosition.Y = 0    ;
            wInf.rcNormalPosition.Width  = 1920 ;
            wInf.rcNormalPosition.Height = 1080 ;


            bRet2 = SetWindowPlacement(procHandle , ref wInf);
            //Thread.Sleep(1000);
            wInf.showCmd = ShowWindowCommands.Maximized ;
            bRet2 = SetWindowPlacement(procHandle , ref wInf);

            while(true)
            {
                IntPtr hwnd = FindWindowEx(procHandle , IntPtr.Zero , "ToolButton" , null);
            }


            
        }

        //[DllImport("user32")]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //public static extern bool EnumChildWindows(IntPtr window, EnumWindowProc callback, IntPtr i);
        //
        ///// <summary>
        ///// Returns a list of child windows
        ///// </summary>
        ///// <param name="parent">Parent of the windows to return</param>
        ///// <returns>List of child windows</returns>
        //public static List<IntPtr> GetChildWindows(IntPtr parent)
        //{
        //    List<IntPtr> result = new List<IntPtr>();
        //    GCHandle listHandle = GCHandle.Alloc(result);
        //    try
        //    {
        //        EnumWindowProc childProc = new EnumWindowProc(EnumWindow);
        //        EnumChildWindows(parent, childProc, GCHandle.ToIntPtr(listHandle));
        //    }
        //    finally
        //    {
        //        if (listHandle.IsAllocated)
        //            listHandle.Free();
        //    }
        //    return result;
        //}

        public int GetHandle()
        {
		    if((int)procHandle == 0) return -1;
            return (int)procHandle;
        }

        private bool CheckWindow()
        {
            System.Diagnostics.Process[] process = System.Diagnostics.Process.GetProcessesByName(procName);//"AmCap");
            if (process.Length > 0)
            {   
                return true;
            }
            return false;
        }
        public void SetParent(IntPtr _Handle)
        {
            if (process == null) return ;
            if (!CheckWindow()) Init();
            SetParent(process.MainWindowHandle, _Handle);

            const int SW_SHOWMAXIMIZED = 3;
            ShowWindow(process.MainWindowHandle, SW_SHOWMAXIMIZED);

        }

        public void Close()
        {
            try
            {
                process.Kill();

            }
            catch
            {
                Log.Trace("Error", "Camera Close Failed!");
            }

            //CameraProcess.Dispose();
        }
    }
}
