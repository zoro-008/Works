using COMMON;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Machine
{
    public class MacroCmd
    {
        public static int m_iPartId;
        public MacroCmd(int _iPartId = 0)
        {
            m_iPartId = _iPartId;
        }
        //=========================================================================================
        [DllImport("user32")]
        public static extern int IsWindowVisible(IntPtr hwnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        protected static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int  SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, string lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SendMessage(IntPtr hWnd, uint Msg, int wParam, StringBuilder lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int SendMessage(IntPtr window, int message, int wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, string lParam);
        
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int IsWindowEnabled(IntPtr hwnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, UInt32 uFlags);


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



        //internal struct WINDOWPLACEMENT
        //{
        //    public int length;
        //    public int flags;
        //    public ShowWindowCommands showCmd;
        //    public System.Drawing.Point ptMinPosition;
        //    public System.Drawing.Point ptMaxPosition;
        //    public System.Drawing.Rectangle rcNormalPosition;
        //}



        //[DllImport("user32.dll")]
        //private static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);
        //
        //[DllImport("user32.dll")]
        //private static extern bool GetWindowPlacement(IntPtr hWnd, out WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);
        
        //[DllImport("user32.dll")]
        //internal static extern bool GetWindowPlacement(int hWnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        /*
         * private static WINDOWPLACEMENT GetPlacement(IntPtr hwnd)
        {
            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
            placement.length = Marshal.SizeOf(placement);
            GetWindowPlacement(hwnd, ref placement);
            return placement;
        }
        

 

        // Windows 의 Position 을 가져옴.

        

        Point getLocationPoint()
        {            
            Process me = Process.GetCurrentProcess(); // 현재 실행중인 Program 의 Process 를 가져온다.                       
            IntPtr hwnd = (IntPtr)me.MainWindowHandle; // me.ID 는 자신의 PID, me.MainWindowHandle 은 Spy++ 에서 확인할 수 있는 핸들 값이다.
            int ptrPhwnd = 0, ptrNhwnd = 0;
            Point ptPoint = new Point();
            Size szSize = new Size();
            WNDSTATE intShowCmd = 0;

            GetWindowPos(hwnd, ref ptrPhwnd, ref ptrNhwnd, ref ptPoint, ref szSize, ref intShowCmd);

            Console.WriteLine("X : {0}", ptPoint.X);
            Console.WriteLine("Y : {0}", ptPoint.Y);
            return ptPoint;
        }
         */




        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hwnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("kernel32", CharSet = CharSet.Auto)]
        public static extern IntPtr OpenProcess(Int32 Access, Boolean InheritHandle, Int32 ProcessId);


        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint dwFreeType);
        
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, int nSize, int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out()] IntPtr lpBuffer, int dwSize, int lpNumberOfBytesRead);




        [StructLayoutAttribute(LayoutKind.Sequential)]
        struct LV_ITEM
        {
            public UInt32 mask;
            public Int32 iItem;
            public Int32 iSubItem;
            public UInt32 state;
            public UInt32 stateMask;
            public IntPtr pszText;
            public Int32 cchTextMax;
            public Int32 iImage;
            public IntPtr lParam;
        }
        //=========================================================================================
        //사용하는 윈도우 메시지
        protected const UInt32 WM_LBUTTONDOWN   = 0x0201;//0x0002
        protected const UInt32 WM_LBUTTONUP     = 0x0202;//0x0004
        protected const UInt32 WM_RBUTTONDOWN   = 0x0204;
        protected const UInt32 WM_RBUTTONUP     = 0x0205;
        protected const UInt32 BM_CLICK         = 0x00F5;
        protected const UInt32 WM_GETTEXT       = 0x000D;
        protected const UInt32 WM_GETTEXTLENGTH = 0x000E;
        protected const UInt32 WM_COPYDATA      = 0x4A  ;
        protected const UInt32 WM_CLOSE         = 0x0010;
        protected const UInt32 WM_SETTEXT       = 0x000C;
        protected const UInt32 CB_GETCURSEL     = 0x0147;
        protected const UInt32 CB_SELECTSTRING  = 0x014D;
        protected const UInt32 TCM_SETCURFOCUS  = 0x1330;
        protected const UInt32 CB_FINDSTRING    = 0x014C;
        protected const UInt32 TCM_SETCURSEL    = 0x1312;
        protected const UInt32 WM_COMMAND       = 0x111 ;
        protected const UInt32 BM_SETSTATE      = 0x00F3;
        protected const UInt32 CB_SETCURSEL     = 0x014E;
        protected const UInt32 BM_SETCHECK      = 0x00F1;
        protected const UInt32 BM_GETCHECK      = 0x00F0;
        
        protected const UInt32 SWP_NOSIZE       = 0x0001;
        protected const UInt32 SWP_NOZORDER     = 0x0004;
        protected const UInt32 SWP_SHOWWINDOW   = 0x0040;

        protected const int LVM_FIRST           = 0x1000;
        protected const int LVM_GETITEMCOUNT    = LVM_FIRST + 4;
        protected const int LVM_GETITEM         = LVM_FIRST + 75;
        protected const int LVIF_TEXT           = 0x0001;

        public const int WM_ENABLE   = 0x000A;
        public const int WM_KEYDOWN  = 0x0100;
        public const int WM_CHAR     = 0x0102;
        public const int VK_RETURN   = 0x0D  ;
        public const int WM_SETFOCUS = 0x0007;
        public const int WM_KEYUP    = 0x101 ;
        public const int PROCESS_ALL_ACCESS = 0x1F0FFF;

        //=========================================================================================
        public static IntPtr MakeLParam(int LoWord, int HiWord)
        {
            return (IntPtr) ((HiWord >> 16) | (LoWord & 0xffff));
        }
        private static int HiWord(int number) 
        {
            if((number & 0x80000000) == 0x80000000) return (number >> 16); 
            else                                    return (number >> 16) & 0xffff ;
        }
        private static int LoWord(int number) 
        {
            return number & 0xffff;
        }
        //=========================================================================================
        //구형
        public struct Tools
        {
            public const string ComboBox = "ComboBox";
            public const string Button   = "Button";
            public const string Edit     = "Edit";
            public const string Static   = "Static";
            public const string RichEdit = "RichEdit20W";
        }

         

        public static void InitProcess(string _sAppPath,int _iWaitTime , int x = 0, int y = 0)
        {
            System.Diagnostics.Process Proc = new System.Diagnostics.Process();
            Proc.StartInfo.FileName = _sAppPath;
            Proc.StartInfo.UseShellExecute = true ;
            Proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(_sAppPath);


            //Process process = new Process()
            //{
            //    StartInfo = new ProcessStartInfo(path, "{Arguments If Needed}")
            //    {
            //        WindowStyle = ProcessWindowStyle.Normal,
            //        WorkingDirectory = Path.GetDirectoryName(path)
            //    }
            //};



            //Proc1.StartInfo.Verb = "runas";
            try
            {
                Trace("Before Process Start");
                bool bRet = Proc.Start();
                Trace("Process Start");
                //Proc.WaitForInputIdle(8000);
                //if(x != 0 && y != 0) 

                //Maximize(Proc.MainWindowHandle);//,IntPtr.Zero,x,y,0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);
                IntPtr ProcessHandle = FindWindow("SunAwtFrame","Ocean Optics SpectrSuite");

                WINDOWPLACEMENT wInf = new WINDOWPLACEMENT();
                wInf.length = System.Runtime.InteropServices.Marshal.SizeOf(wInf);
                bRet = GetWindowPlacement(ProcessHandle , ref wInf);

                if(!Proc.HasExited)
                {
                    wInf.showCmd = ShowWindowCommands.Maximized ;
                    SetWindowPlacement(Proc.MainWindowHandle, ref wInf);
                }



                





                
                //placement.length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));
                //placement.flags = 0;
                



                //Trace("Before Wait");
                //Proc.WaitForExit(_iWaitTime);
                //Trace(_iWaitTime.ToString()+"ms Wait");
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        void GetWindowPos(IntPtr hwnd, ref int ptrPhwnd, ref int ptrNhwnd, ref Point ptPoint, ref Size szSize, ref WNDSTATE intShowCmd)
        {
            WINDOWPLACEMENT wInf = new WINDOWPLACEMENT();
            wInf.length = System.Runtime.InteropServices.Marshal.SizeOf(wInf);
            GetWindowPlacement(hwnd, ref wInf);
            szSize = new Size(wInf.rcNormalPosition.Right - (wInf.rcNormalPosition.Left * 2),
            wInf.rcNormalPosition.Bottom - (wInf.rcNormalPosition.Top * 2));
            ptPoint = new Point(wInf.rcNormalPosition.Left, wInf.rcNormalPosition.Top);            
        }

        public static void Maximize(IntPtr windowHandle)
        {

            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();

            placement.length = Marshal.SizeOf(placement);
            //GetWindowPlacement(windowHandle, ref placement);

            






            placement.length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));
            placement.flags = 0;
            placement.showCmd = ShowWindowCommands.Maximized; //WNDSTATE.SW_MAXIMIZE ;
            SetWindowPlacement(windowHandle, ref placement);
        }

        public static bool ExitProcess(string _sAppCaption = "")
        {
            bool bRet = true;
            Process[] pList = Process.GetProcessesByName(_sAppCaption);

            if (_sAppCaption != "" && pList.Length > 0)
            {
                for (int i = 0; i < pList.Length; i++) pList[i].Kill();
            }
            if (pList.Length > 0) bRet = false;
            return bRet;
        }

        public static bool ExistProces(string _sAppCaption = "")
        {
            bool bRet = false;
            Process[] pList = Process.GetProcessesByName(_sAppCaption);
            if (pList.Length > 0) bRet = true;
            return bRet;
        }

        protected static IntPtr FindWindowL(string strClassName, string strWindowName) 
        {
            IntPtr iRet = IntPtr.Zero;
            try
            {
                iRet = FindWindow(strClassName, strWindowName);
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return iRet;

        }
        //프로그램에 종속된 컨트롤 핸들 찾는부분
        protected static IntPtr FindWindowChild(IntPtr _ipParent , IntPtr _ipFrom , string _sClass , string _sCaption)
        {
            //Delay(50);
            IntPtr iRet = IntPtr.Zero;
            try
            {
                iRet = FindWindowEx(_ipParent, _ipFrom, _sClass, _sCaption);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return iRet;        
        }

        protected static IntPtr FindWindowIndex(IntPtr _ipParent, string Tool , int _iIdx = 0) 
        {
            int iIdx = _iIdx;
            IntPtr iRet = FindWindowChild(_ipParent, IntPtr.Zero, Tool, null);
            if (iRet != IntPtr.Zero)
            {
                for (int i = 0; i < iIdx; i++)
                {
                    iRet = FindWindowChild(_ipParent, iRet, Tool, null);
                    if (iRet == IntPtr.Zero) break;
                }
            }
            return iRet;
        }

        protected static IntPtr SetWindow(IntPtr _ipParent, string Tool , int _iIdx = 0 , int _iValue = 0) 
        {
            IntPtr iRet = FindWindowIndex(_ipParent, Tool, _iIdx);

            if(iRet != IntPtr.Zero)
            {
                if(Tool == Tools.ComboBox   ) PostMessage(iRet, CB_SETCURSEL, _iValue, 0); //0 ~
                else if(Tool == Tools.Button) PostMessage(iRet, BM_SETCHECK , _iValue, 0); // 0 - uncheck 1 - check
            }
            return iRet;
        }

        protected static IntPtr SetWindowText(IntPtr _ipParent, string Tool , int _iIdx = 0 , string  _sValue = "") 
        {
            IntPtr iRet = FindWindowIndex(_ipParent, Tool, _iIdx);

            if(iRet != IntPtr.Zero)
            {
                SendMessage(iRet, WM_SETTEXT, IntPtr.Zero, _sValue); //need to be delivered synchronously
            }
            return iRet;
        }

        protected static int GetWindow(IntPtr _ipParent, string Tool , int _iIdx = 0) 
        {
            int    iText = 0;
            IntPtr iRet  = FindWindowIndex(_ipParent, Tool, _iIdx);
            
            if(iRet != IntPtr.Zero)
            {
                if(Tool == Tools.ComboBox   ) iText = SendMessage(iRet, CB_GETCURSEL, 0, 0); //0 ~
                else if(Tool == Tools.Button) iText = SendMessage(iRet, BM_GETCHECK , 0, 0); //0 - uncheck 1 - check
            }
            return iText;
        }

        protected static string GetWindowText(IntPtr _ipParent, string Tool , int _iIdx = 0 ) 
        {
            IntPtr textPtr = IntPtr.Zero;
            string sText   = "";
            IntPtr iRet    = FindWindowIndex(_ipParent, Tool, _iIdx);

            if(iRet != IntPtr.Zero)
            {
                StringBuilder title = new StringBuilder();
                int size = SendMessage(iRet, WM_GETTEXTLENGTH, 0, 0);
                if (size > 0)
                {
                    title = new StringBuilder(size + 1);
                    SendMessage(iRet, WM_GETTEXT, title.Capacity, title);
                    sText = title.ToString();
                }
            }
            return sText;
        }

        //윈도우에 해당하는 텍스트를 호출자가 제공 한 버퍼로 복사합니다.
        protected static string GetWindowText(IntPtr _ipWnd)
        {
            //Delay(500);
            IntPtr textPtr = IntPtr.Zero;
            string sTemp = "";
            try
            {
                StringBuilder title = new StringBuilder();

                // Get the size of the string required to hold the window title. 
                int size = SendMessage(_ipWnd, WM_GETTEXTLENGTH, 0, 0);

                // If the return is 0, there is no title. 
                if (size > 0)
                {
                    title = new StringBuilder(size + 1);
                    SendMessage(_ipWnd, WM_GETTEXT, title.Capacity, title);
                    sTemp = title.ToString();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return sTemp ;
        }

        //Listview 아이템 텍스트 가져올때 사용        
        protected static string GetItemText(IntPtr handle, int index, int subIndex)
        {
            int pid;

            //핸들을 이용하여 프로세스 id를 얻어 온다.
            GetWindowThreadProcessId(handle, out pid);


            //해당 프로세스의 핸들을 얻어 옵니다.
            IntPtr hProcess = OpenProcess(0x0008 | 0x0010 | 0x0020 | 0x0400, false, pid);
         

            //해당 프로세스 영역에 메모리를 할당 합니다.

            IntPtr vPtr = VirtualAllocEx(hProcess, IntPtr.Zero, Marshal.SizeOf(typeof(LV_ITEM)), 0x1000, 0x04);


            //우리가 원하는 리스트뷰의 항목 구조체를 할당 합니다.

            LV_ITEM item = new LV_ITEM();

            //텍스트를 얻어오겠다는 마스크 입니다.
            item.mask = LVIF_TEXT;

            //텍스트의 최대 크기를 지정 합니다.
            item.cchTextMax = 512;
            //원하는 항목의 인덱스 입니다.
            item.iItem = index;
            //원하는 서브 항목의 인덱스 입니다.
            item.iSubItem = subIndex;

            //이부분에 항목문자열이 저장 됩니다. 역시 해당 프로세스 영역에 할당 합니다.
            item.pszText = VirtualAllocEx(hProcess, IntPtr.Zero, 512, 0x1000, 0x04);
           

            //방금만든 구조체를 비관리 영역에 할당 합니다.
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(LV_ITEM)));

            //할당한 포인터에 복사 합니다.
            Marshal.StructureToPtr(item, ptr, true);


            //비관리 영역에 할당한 포인터를 프로세스 영역에 할당한 포인터에 씁니다.

            WriteProcessMemory(hProcess, vPtr, ptr, Marshal.SizeOf(typeof(LV_ITEM)), 0);

            //쓰고나면 포인터는 해제 합니다.
            Marshal.FreeHGlobal(ptr);


            //항목 포인터를 얻어 옵니다.

            SendMessage(handle, LVM_GETITEM, 0, vPtr);


            //얻어온 항목은 프로세스 영역에 할당되어 있기 때문에 읽거나 쓸 수 없습니다.

            //다시 비관리 영역으로 읽어 들여야 합니다. 이를위해 다시 할당 합니다.

            ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(LV_ITEM)));


            //해당 프로세스 영역에서 비관리 영역으로 읽어 들입니다.
            ReadProcessMemory(hProcess, vPtr, ptr, Marshal.SizeOf(typeof(LV_ITEM)), 0);

            //읽어들인 포인터를 구조체로 변환 합니다.
            item = (LV_ITEM)Marshal.PtrToStructure(ptr, typeof(LV_ITEM));

            //쓰고난 포인터 해제
            Marshal.FreeHGlobal(ptr);


            //이제 다왔습니다. 문자열을 얻어오기 위한 작업 입니다. 위의 작업과 거의 동일하니 주석 생략 합니다.

            ptr = Marshal.AllocHGlobal(512);
            ReadProcessMemory(hProcess, item.pszText, ptr, 512, 0);
            string text = Marshal.PtrToStringUni(ptr);
            Marshal.FreeHGlobal(ptr);


            //메모리 해제

            VirtualFreeEx(hProcess, item.pszText, 0, 0x8000);
            VirtualFreeEx(hProcess, ptr, 0, 0x8000);


            return text;
        }

        //콤보박스 인덱스 가져올때 사용
        protected static Int32 GetComboIndex(IntPtr _ipWnd)
        {
            //Delay(500);
            Int32 iTemp = 0;
            try
            {
                iTemp = SendMessage(_ipWnd, CB_GETCURSEL, 0, 0);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return iTemp;
        }
       
        //콤보박스 내용 변경할때 사용
        protected static void SetComboBox(IntPtr _ipWnd, string _cNewCaption) 
        {
            try
            {
                PostMessage(_ipWnd, CB_SELECTSTRING, IntPtr.Zero, _cNewCaption);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        protected static void SetComboBoxIndex(IntPtr _ipWnd, int _Index)
        {
            try
            {
                PostMessage(_ipWnd, CB_SETCURSEL, _Index, 0);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        //탭컨트롤 변경할때 사용
        protected static void SetTabControl(IntPtr _ipWnd, int _iTabNumber)
        {
            try
            {
                PostMessage(_ipWnd, TCM_SETCURFOCUS, _iTabNumber, 0);
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        //핸들 찾아서 해당 좌표에 마우스 좌클릭 실행
        protected static void Click(IntPtr _ipHwnd , int iTimes = 2/*, int _iX, int _iY*/)
        {
            for(int i=0; i< iTimes; i++)
            {
                PostMessage(_ipHwnd, BM_CLICK, 0, 0);
            }
        }
        public static void RClick(IntPtr _ipHwnd, int iTimes = 2/*, int _iX, int _iY*/)
        {
            for (int i = 0; i < iTimes; i++)
            {
                PostMessage(_ipHwnd, WM_RBUTTONUP  , 0, 0);
            }
        }


        //Folder Copy하는 함수
        public static void CopyFolder(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
            {
                Directory.CreateDirectory(destFolder);
            }

            string[] files = Directory.GetFiles(sourceFolder);
            string[] folders = Directory.GetDirectories(sourceFolder);

            foreach (string file in files)
            {
                string name = Path.GetFileName(file);
                string dest = Path.Combine(destFolder, name);
                File.Copy(file, dest, true);
            }

            foreach (string folder in folders)
            {
                string name = Path.GetFileName(folder); 
                string dest = Path.Combine(destFolder, name);
                CopyFolder(folder, dest);
            }
        }

        /// <summary>
        /// FOLDER DELETE
        /// </summary>
        /// <param name="path">삭제할 폴더 경로</param>
        /// <returns>true,false반환</returns>
        public static bool DeleteFolder(string path)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                FileInfo[] files = dir.GetFiles("*.*", SearchOption.AllDirectories);
                foreach (FileInfo file in files)
                    file.Attributes = FileAttributes.Normal;
                Directory.Delete(path, true);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool TestMoveWindow(IntPtr _hwnd, int _X, int _Y, int _nWidth, int _nHeight, bool _bRepaint)
        {
            return MoveWindow(_hwnd, _X, _Y, _nWidth, _nHeight, _bRepaint);
        }
        public static IntPtr TestFindWindowL(string strClassName, string strWindowName)
        {
            IntPtr iRet = IntPtr.Zero;
            try
            {
                iRet = FindWindow(strClassName, strWindowName);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return iRet;

        }

        public static void Delay(int ms)
        {
            DateTime dateTimeNow = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, ms);
            DateTime dateTimeAdd = dateTimeNow.Add(duration);
            while (dateTimeAdd >= dateTimeNow)
            {
                System.Windows.Forms.Application.DoEvents();
                dateTimeNow = DateTime.Now;
            }
            return;
        }
//=========================================================================================
        public static void Trace(string _sMsg, [CallerLineNumber] int sourceLineNumber = 0, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "")
        {
            string sHdr = "Macro";
            string sMsg = _sMsg.Replace(",", "");
            string sTag = string.Format("{0:00}", m_iPartId);
            string sFullMsg = string.Format("{0}, {1} ,{2},{3},{4}", sTag, sHdr + " " + sMsg, sourceLineNumber, memberName, sourceFilePath);
            Log.SendMessage(sFullMsg);
        }

    }
}
