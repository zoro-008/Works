using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//------------------------------------------------------------------------------
//NSIS를 이용한 설치파일 만들기 (김동우)
//NUGET 에서 NSIS 설치 필요
//실행시에 설치 폴더를 BACKUP\VERSION\에 백업하고 수행
//빌드 이벤트에 실행후 파일삭제(매번 수행되게 하는 방법으로 사용)

//파일 추가하기
//필요한 파일은 info.File.Add를 이용하여 추가
//------------------------------------------------------------------------------

namespace Installer
{
    class Program
    {
        #region DllImport
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string IPClassName, String IpWindowName);

        [DllImport("User32.dll", EntryPoint = "SetForegroundWindow")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();
        
        //[DllImport("user32.dll")]
        //static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0; // 숨기기
        const int SW_SHOW = 1; // 보이기
        #endregion

        #region Variable
        static string sError ;
        static List<string> listScript = new List<string>();
        static string stext;
        static public  string sText
        {
            get { return stext; }
            set {
                stext = value;
                listScript.Add(stext.Trim());
                //if(stext != Environment.NewLine) richTextBox1.AppendText(stext + "\n");
                //else                             richTextBox1.AppendText(stext);
            }
        }

        struct Info
        {
            public string PRODUCT_NAME       ;  
            public string PRODUCT_VERSION    ;
            public string PRODUCT_PUBLISHER  ;
            public string PRODUCT_WEB_SITE   ;
            public string PRODUCT_DIR_REGKEY ;
                           
            public string MUI_ICON           ;
            public string MUI_PAGE_LICENSE   ;
            public string MUI_FINISHPAGE_RUN ;
            public string MUI_LANGUAGE       ;
                           
            public string OutFile            ;
            public string InstallDir         ;
            public string InstallDirRegKey   ;
                           
            public List<string> SetOutPath   ;
            public List<string> File         ;

        };
        static Info info ;

        #endregion

        static void Main(string[] args)
        {
            //var handle = GetConsoleWindow();
            //ShowWindow(handle, SW_HIDE); // 숨기기

            sError = "" ;

            info.SetOutPath = new List<string>();
            info.File       = new List<string>();

            SetInfo  (); //정보 입력 하기
            SetScript(); //스크립트 만들기
            Make     (); //인스톨 파일 만들기

            if(sError != "") {
                //Console.WriteLine("INSTALL ERROR \r\nFile not found : " + sError);
                MessageBox.Show("FILE NOT FOUND : " + sError , "INSTALL ERROR");
                //Console.ReadKey();
                //MessageBox.Show("File not found : " + sError , "Install Error");
            }
        }

        #region Form
        /*
        public static Install Instance { 
			get { return instance == null ? (instance = new Install()) : instance; } 
		}
		private static Install instance = null;

        public Install()
        {
            //var handle = GetConsoleWindow();
            //ShowWindow(handle, SW_HIDE); // 숨기기

            sError = "" ;

            info.SetOutPath = new List<string>();
            info.File       = new List<string>();

            SetInfo  ();
            SetScript();
            Make     ();

            if(sError != "") {
                MessageBox.Show("File not found : " + sError , "Install Error");
            }
        }
        */
        #endregion

        static private void SetInfo()
        {
            var directory = Environment.CurrentDirectory         ;
            
            DirectoryInfo directoryInfo = Directory.GetParent(directory).Parent;
            directory = directoryInfo.FullName ;

            info.File.Add(@"\Bin\Control.exe"                  );
            info.File.Add(@"\Bin\ko\Control.resources.dll"     );
            info.File.Add(@"\Bin\zh-Hans\Control.resources.dll");

            info.File.Add(@"\Bin\Dll\SML\SML.dll");
            info.File.Add(@"\Bin\Dll\SML\ko\SML.resources.dll");
            info.File.Add(@"\Bin\Dll\SML\zh-Hans\SML.resources.dll");

            info.File.Add(@"\Bin\Dll\SML\Common.dll");

            info.File.Add(@"\Bin\Dll\SML\AXL.dll");
            info.File.Add(@"\Bin\Dll\SML\EzBasicAxl.dll"           );
            
            var PathLicense     = directory + @"\Installer\Resources\License.txt"    ;

            switch(Machine.Eqp.sLanguage)
            {
                default       : 
                    //info.File.Add(@"\Bin\Util\AInput_E.ini"  );
                    //info.File.Add(@"\Bin\Util\Cylinder_E.ini");
                    //info.File.Add(@"\Bin\Util\DInput_E.ini"  );
                    //info.File.Add(@"\Bin\Util\DOutput_E.ini" );
                    info.File.Add(@"\Bin\Util\Error_E.ini"   );
                    PathLicense = directory + @"\Installer\Resources\License_E.txt"    ;
                    break;
                case "English": 
                    //info.File.Add(@"\Bin\Util\AInput_E.ini"  );
                    //info.File.Add(@"\Bin\Util\Cylinder_E.ini");
                    //info.File.Add(@"\Bin\Util\DInput_E.ini"  );
                    //info.File.Add(@"\Bin\Util\DOutput_E.ini" );
                    info.File.Add(@"\Bin\Util\Error_E.ini"   );
                    PathLicense = directory + @"\Installer\Resources\License_E.txt"    ;
                    break;
                case "Korean" : 
                    //info.File.Add(@"\Bin\Util\AInput_K.ini"  );
                    //info.File.Add(@"\Bin\Util\Cylinder_K.ini");
                    //info.File.Add(@"\Bin\Util\DInput_K.ini"  );
                    //info.File.Add(@"\Bin\Util\DOutput_K.ini" );
                    info.File.Add(@"\Bin\Util\Error_K.ini"   );
                    PathLicense = directory + @"\Installer\Resources\License_K.txt"    ;
                    break;
                case "Chinese": 
                    //info.File.Add(@"\Bin\Util\AInput_C.ini"  );
                    //info.File.Add(@"\Bin\Util\Cylinder_C.ini");
                    //info.File.Add(@"\Bin\Util\DInput_C.ini"  );
                    //info.File.Add(@"\Bin\Util\DOutput_C.ini" );
                    info.File.Add(@"\Bin\Util\Error_C.ini"   );
                    PathLicense = directory + @"\Installer\Resources\License_E.txt"    ;
                    break;
            }

            FileInfo file;
            for(int i=0; i<info.File.Count; i++)
            {
                info.SetOutPath.Add(@"$INSTDIR" + Path.GetDirectoryName(info.File[i]) + @"");
                info.File[i] = directory + info.File[i];
                file = new FileInfo(info.File[i]);
                if(!file.Exists) sError = info.File[i] ;
            }

            //첫번째걸로 버전 체크
            var PathExe = info.File[0] ; 
            file = new FileInfo(PathExe);
            PathExe = file.FullName;

            //아이콘 라이센스 파일
            var PathIcon        = directory + @"\Installer\Resources\HanraCircle.ico";
            //var PathLicense     = directory + @"\Installer\Resources\License.txt"    ;
            file = new FileInfo(PathIcon   ); if(!file.Exists) sError =  PathIcon    ;
            file = new FileInfo(PathLicense); if(!file.Exists) sError =  PathLicense ;
            
            info.PRODUCT_NAME       = @"""" + Machine.Eqp.sEqpName + @"""";
            info.PRODUCT_VERSION    = @"""" + GetVer(PathExe)      + @"""";
            info.PRODUCT_PUBLISHER  = @"""HANRA PRECISION ENG.CO.,LTD""";
            info.PRODUCT_WEB_SITE   = @"""http://www.hanra.co.kr""";
            info.PRODUCT_DIR_REGKEY = @"""Software\Microsoft\Windows\CurrentVersion\App Paths\Control.exe""";
                        
            info.MUI_ICON           = @"""" + PathIcon    + @"""";
            info.MUI_PAGE_LICENSE   = @"""" + PathLicense + @"""";
            info.MUI_FINISHPAGE_RUN = @"""" + PathExe     + @"""";
            //info.MUI_LANGUAGE       = @"""Korean""";
            info.MUI_LANGUAGE       = @"""" + Machine.Eqp.sLanguage + @"""";
            

            info.OutFile            = @"""Installer.exe""";            
            //info.OutFile            = @"""" + Machine.Eqp.sEqpName + @".exe""";
            info.InstallDir         = @"""" + directory + @"""";
            info.InstallDirRegKey   = @"HKLM ""${PRODUCT_DIR_REGKEY}"" """"";

        }

        static private void SetScript()
        {
            sText = "; Script generated by the HM NIS Edit Script Wizard."; 
            sText = Environment.NewLine;
            sText = @"; HM NIS Edit Wizard helper defines";
            sText = @"!define PRODUCT_NAME "       + info.PRODUCT_NAME       ;
            sText = @"!define PRODUCT_VERSION "    + info.PRODUCT_VERSION    ;
            sText = @"!define PRODUCT_PUBLISHER "  + info.PRODUCT_PUBLISHER  ;
            sText = @"!define PRODUCT_WEB_SITE "   + info.PRODUCT_WEB_SITE   ;
            sText = @"!define PRODUCT_DIR_REGKEY " + info.PRODUCT_DIR_REGKEY ;
            sText = Environment.NewLine;
            sText = @"; MUI 1.67 compatible ------";
            sText = @"!include ""MUI.nsh""";
            sText = Environment.NewLine;
            sText = @"; MUI Settings";
            sText = @"!define MUI_ABORTWARNING";
            sText = @"!define MUI_ICON "              + info.MUI_ICON ;
            sText = Environment.NewLine;
            sText = @"; Welcome page";
            sText = @"!insertmacro MUI_PAGE_WELCOME";
            sText = @"; License page";
            sText = @"!insertmacro MUI_PAGE_LICENSE " + info.MUI_PAGE_LICENSE;
            sText = @"; Directory page";
            sText = @"!insertmacro MUI_PAGE_DIRECTORY";
            sText = @"; Instfiles page";
            sText = @"!insertmacro MUI_PAGE_INSTFILES";
            sText = @"; Finish page";
            sText = @";!define MUI_FINISHPAGE_RUN " + info.MUI_FINISHPAGE_RUN;
            sText = @"!insertmacro MUI_PAGE_FINISH";
            sText = Environment.NewLine;
            sText = @"; Language files";
            sText = @"!insertmacro MUI_LANGUAGE "   + info.MUI_LANGUAGE;
            sText = Environment.NewLine;
            sText = @"; MUI end ------";
            sText = Environment.NewLine;
            sText = @"Name ""${PRODUCT_NAME} ${PRODUCT_VERSION}""";
            sText = @"OutFile "          + info.OutFile         ;
            sText = @"InstallDir "       + info.InstallDir      ;
            sText = @"InstallDirRegKey " + info.InstallDirRegKey;
            sText = @"ShowInstDetails show";
            sText = Environment.NewLine;

            //Function Stt
            sText = @"Function GetFileVersion";
            sText = @"	!define GetFileVersion `!insertmacro GetFileVersionCall`";

            sText = @"	!macro GetFileVersionCall _FILE _RESULT";
            sText = @"		Push `${_FILE}`";
            sText = @"		Call GetFileVersion";
            sText = @"		Pop ${_RESULT}";
            sText = @"	!macroend";

            sText = @"	Exch $0";
            sText = @"	Push $1";
            sText = @"	Push $2";
            sText = @"	Push $3";
            sText = @"	Push $4";
            sText = @"	Push $5";
            sText = @"	Push $6";
            sText = @"	ClearErrors";

            sText = @"	GetDllVersion '$0' $1 $2";
            sText = @"	IfErrors error";
            sText = @"	IntOp $3 $1 >> 16";
            sText = @"	IntOp $3 $3 & 0x0000FFFF";
            sText = @"	IntOp $4 $1 & 0x0000FFFF";
            sText = @"	IntOp $5 $2 >> 16";
            sText = @"	IntOp $5 $5 & 0x0000FFFF";
            sText = @"	IntOp $6 $2 & 0x0000FFFF";
            sText = @"	StrCpy $0 '$3.$4.$5.$6'";
            sText = @"	goto end";

            sText = @"	error:";
            sText = @"	SetErrors";
            sText = @"	StrCpy $0 ''";

            sText = @"	end:";
            sText = @"	Pop $6";
            sText = @"	Pop $5";
            sText = @"	Pop $4";
            sText = @"	Pop $3";
            sText = @"	Pop $2";
            sText = @"	Pop $1";
            sText = @"	Exch $0";
            sText = @"FunctionEnd";
            sText = Environment.NewLine;
            //Function End

            sText = @"Section ""MainSection"" SEC01";
            //sText = @";  CreateDirectory "$SMPROGRAMS\테스트 #1"
            //sText = @";  CreateShortCut "$SMPROGRAMS\테스트 #1\테스트 #1.lnk" "$INSTDIR\Control.exe"
            //sText = @";  CreateShortCut "$DESKTOP\테스트 #1.lnk" "$INSTDIR\Control.exe"
            sText = @"  SetOverwrite on";

            string sFileName = @"""" + info.SetOutPath[0] + @"\" + Path.GetFileName(info.File[0]) + @"""" ;
            sText = @"Var /GLOBAL Version";
            sText = @"${GetFileVersion} " + sFileName + " $version";
            sText = @"CopyFiles ""$INSTDIR\Bin\*.*"" ""$INSTDIR\BACKUP\$Version\""";
            //sText = @"SetOutPath ""$INSTDIR\BACKUP\$version\""";
            //sText = @"File /nonfatal /a /r ""$INSTDIR\Bin\"" #note back slash at the end";
            for(int i=0; i<info.File.Count; i++)
            {
                sText = @"  SetOutPath """ + info.SetOutPath[i]  + @"""";
                sText = @"  File """       + info.File[i]        + @""""; 
            }
            sText = @"SectionEnd";

            
        }

        static public void Make()
        {
            //richTextBox1.SaveFile(@"D:\Temp\qqq.nsi");
            var directory = Environment.CurrentDirectory         ;
            DirectoryInfo directoryInfo = Directory.GetParent(directory).Parent;
            directory = directoryInfo.FullName ;

            var InstallFile = directory + @"\Install.nsi" ;
            var NsisFile    = directory + @"\packages\NSIS.2.51\tools\makensis.exe";

            FileInfo file = new FileInfo(NsisFile); if(!file.Exists) sError =  NsisFile ;

            StreamWriter sw = new StreamWriter(InstallFile);
            for(int i=0; i<listScript.Count; i++)
            {
                sw.WriteLine(listScript[i]);
            }
            sw.Close();


            System.Diagnostics.ProcessStartInfo proInfo = new System.Diagnostics.ProcessStartInfo();
            System.Diagnostics.Process pro = new System.Diagnostics.Process();

            // 실행할 파일명 입력 -- cmd
            proInfo.FileName = @"cmd";
            // cmd 창 띄우기 -- true(띄우지 않기.) false(띄우기)
            proInfo.CreateNoWindow = true;
            proInfo.UseShellExecute = false;
            // cmd 데이터 받기
            proInfo.RedirectStandardOutput = true;
            // cmd 데이터 보내기
            proInfo.RedirectStandardInput = true;
            // cmd 오류내용 받기
            proInfo.RedirectStandardError = true;
           
            pro.StartInfo = proInfo;           
            pro.Start();
               
            // CMD 에 보낼 명령어를 입력 합니다.
            pro.StandardInput.Write(NsisFile + " " + InstallFile + Environment.NewLine);
            pro.StandardInput.Close();

            // 결과 값을 리턴 받습니다.
            string resultValue = pro.StandardOutput.ReadToEnd();
            pro.WaitForExit();
            pro.Close();
        }

        static private string GetVer(string _sName)
        {
            //Version vVer ;
            string sVer ;
            string sName = _sName;
            try { 
                //vVer = System.Reflection.AssemblyName.GetAssemblyName(sName).Version ;
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(sName);
                sVer = fvi.FileVersion ;
            }
            catch (Exception _e)
            {
                sVer = "Loading Failed - " + _e.Message ;
            }
            return sVer;
        }

    }
}
