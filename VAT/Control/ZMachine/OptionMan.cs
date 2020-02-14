using COMMON;
using System.Windows.Forms;


//using System.Runtime.InteropServices;
//using SML.CXmlBase;
//using SMLDefine;
//using SMLApp;

namespace Machine
{
              
    //---------------------------------------------------------------------------
    public static class OM
    {
        public struct CDevInfo
        {   //device에 대한 Dev_Info
            public int iNo1 ;
            public int iNo2 ;
            public int iNo3 ;
            public int iNo4 ;
            public int iNo5 ;
            public int iNo6 ;
            public int iNo7 ;
            public int iNo8 ;
            public int iNo9 ;
            public int iNo10;
            public int iNo11;
            public int iNo12;
            public int iNo13;
            public int iNo14;
            public int iNo15;
            public int iNo16;
            public int iNo17;
            public int iNo18;
            public int iNo19;
            public int iNo20;
            public int iNo21;
            public int iNo22;
            public int iNo23;
            public int iNo24;
            
        } ;                                 
                                            
        public struct CCmnOptn //장비 공용 옵션.
        {
            public int    iThreshold          ;
            public int    iMinA               ;
            public int    iMaxA               ;
            public int    iDetectCount        ;
            public int    iArcCount1          ;
            public int    iArcCount2          ;
            public double dArcVoltage         ; //아킹값 레벨 체크

            public string sName1              ;
            public string sName2              ;
            public string sName3              ;
            public string sName4              ;
            public string sName5              ;
            public string sName6              ;
            public string sName7              ;
            public string sName8              ;
            public string sName9              ;
            public string sName10             ;
            public string sName11             ;
            public string sName12             ;
            public string sName13             ;
            public string sName14             ;
            public string sName15             ;
            public string sName16             ;
            public string sName17             ;
            public string sName18             ;
            public string sName19             ;
            public string sName20             ;

            //Total
            public double min11               ; public double max11               ; 
            public double min12               ; public double max12               ;
            //Anode                                                               
            public double min21               ; public double max21               ;
            public double min22               ; public double max22               ;
            public double min23               ; public double max23               ;
            public double min24               ; public double max24               ;
            public double min25               ; public double max25               ;
            public double min26               ; public double max26               ;
            //Gate                                                   
            public double min31               ; public double max31               ;
            public double min32               ; public double max32               ;
            public double min33               ; public double max33               ;
            public double min34               ; public double max34               ;
            public double min35               ; public double max35               ;
            public double min36               ; public double max36               ;
            //Focus                                                   
            public double min41               ; public double max41               ;
            public double min42               ; public double max42               ;
            public double min43               ; public double max43               ;
            public double min44               ; public double max44               ;
            //Cathod                                                   
            public double min51               ; public double max51               ;
            public double min52               ; public double max52               ;
            public double min53               ; public double max53               ;
            public double min54               ; public double max54               ;

            public int    iStepDelay          ;

            public double dCMul               ; public double dCAdd               ;
            public double dFMul               ; public double dFAdd               ;
            public double dGMul               ; public double dGAdd               ;

            //public bool   bIgnrDoor           ;
            //public bool   bLoadingStop        ;
            //public bool   bVisnSkip           ;
            //public bool   bMarkSkip           ;

            //public string sPaintName1         ;
            //public string sPaintName2         ;
            //public string sPaintName3         ;
            //public string sPaintName4         ;
            //public string sPaintName5         ;

            //public int    iEmptyCheckPrcs     ; //0 : Error, 1 : Reject
            
        } ;

        //Master Option.
        public struct CMstOptn                  //마스타 옵션. 화면상에 있음 FrmMain 디바이스 라벨 더블 클릭.
        {
            public bool   bDebugMode    ; //true = 로그기능강화, 디버깅시 타임아웃무시.
            public bool   bIdleRun      ; //IdleRun 장비 통신 및 비전 테스트.
            public double dTrgOfs       ; //연속 트리거 거리 오프셑
            public bool   bMarkAlgin    ; //마킹 얼라인 타이밍을 미리 수정해서 옵션으로 뺴놓음.
            
        };

        //Eqipment Option.
        public struct CEqpOptn                 //모델별 하드웨어 옵션.  화면상에 없고 텍스트 파일에 있음.
        {
            public string sEquipName    ;
            public string sEquipSerial  ;
            public bool   bTestMode     ; //테스트 모드 다 오픈하고 0.1씩 증가하면 테스트 시작
        } ;

        public struct CEqpStat
        {
            public bool   bMaint        ;

            public int    iWorkCnt      ; //총 작업 갯수.
            public int    iULDRCnt      ; //언로더에 들어간 갯수
            public double dWorkUPH      ;
            public double dWorkTime     ;
            public int    iRJCMCnt      ; //리젝 마킹 카운트
            public int    iRJCVCnt      ; //리젝 마킹 카운트
            public int    iULDRWorkNo   ; //작업 하고 있는 위치 번호.

            //public int    iULDRWorkCnt  ; //언로더 현재 작업중인거.

        } ;



        public static string m_sCrntDev; //Current open device.
        
        public static CDevInfo DevInfo;
        public static CCmnOptn CmnOptn;
        public static CMstOptn MstOptn;
        public static CEqpOptn EqpOptn;
        public static CEqpStat EqpStat;

        public static DataGridView SeasoningOptnView    ;
        public static DataGridView SeasoningOptnViewSub ;
        //public static CArray TrayMask = new CArray();



        public static void Init()
        {
            //Load
            LoadLastInfo();
            LoadMstOptn();
            LoadEqpOptn();
            LoadCmnOptn();
            LoadEqpStat();
            LoadJobFile(m_sCrntDev);
            //LoadTrayMask(m_sCrntDev);
        }
        public static void Close()
        {
            SaveEqpStat();
            SaveLastInfo();
        }

        public static string GetCrntDev() 
        {
            return m_sCrntDev ; 
        }

        public static void SetCrntDev(string _sName)
        {
            m_sCrntDev = _sName;
        }

        public static void LoadJobFile(string _sDevName) 
        {
            LoadDevInfo(_sDevName);

            //Set Current Device Name.
            SetCrntDev(_sDevName);
        }
        public static void SaveJobFile(string _sDevName)
        {
            SaveDevInfo(_sDevName);

            //Set Current Device Name.
            SetCrntDev(_sDevName);
        }

        

        public static void LoadDevInfo(string _sJobName)
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevInfoPath = sExeFolder + "JobFile\\" + _sJobName + "\\DevInfo.ini";
            CAutoIniFile.LoadStruct<CDevInfo>(sDevInfoPath,"DevInfo",ref DevInfo);          
        }

        public static void SaveDevInfo(string _sJobName)
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevInfoPath = sExeFolder + "JobFile\\" + _sJobName + "\\DevInfo.ini";
            CAutoIniFile.SaveStruct<CDevInfo>(sDevInfoPath,"DevInfo",ref DevInfo);   
        }

        public static void LoadCmnOptn() 
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sCmnOptnPath = sExeFolder + "Util\\CmnOptn.ini";
            CAutoIniFile.LoadStruct<CCmnOptn>(sCmnOptnPath,"CmnOptn",ref CmnOptn);
        }
        public static void SaveCmnOptn()
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sCmnOptnPath = sExeFolder + "Util\\CmnOptn.ini";
            CAutoIniFile.SaveStruct<CCmnOptn>(sCmnOptnPath,"CmnOptn",ref CmnOptn);
            
        }

        public static void LoadMstOptn() 
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sMstOptnPath = sExeFolder + "Util\\MstOptn.ini";
            CAutoIniFile.LoadStruct<CMstOptn>(sMstOptnPath,"MstOptn",ref MstOptn);
            
        }

        public static void SaveMstOptn()
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sMstOptnPath = sExeFolder + "Util\\MstOptn.ini";
            CAutoIniFile.SaveStruct<CMstOptn>(sMstOptnPath,"MstOptn",ref MstOptn);
            
        }

        public static void LoadEqpOptn()
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sEqpOptnPath = sExeFolder + "Util\\EqpOptn.ini";
            CAutoIniFile.LoadStruct<CEqpOptn>(sEqpOptnPath,"EqpOptn",ref EqpOptn);
        }
        public static void SaveEqpOptn()
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sEqpOptnPath = sExeFolder + "Util\\EqpOptn.ini";
            CAutoIniFile.SaveStruct<CEqpOptn>(sEqpOptnPath,"EqpOptn",ref EqpOptn);
        }
        public static void LoadEqpStat()
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sEqpOptnPath = sExeFolder + "Util\\EqpStat.ini";
            CAutoIniFile.LoadStruct<CEqpStat>(sEqpOptnPath,"EqpStat",ref EqpStat);
        }
        public static void SaveEqpStat()
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sEqpOptnPath = sExeFolder + "Util\\EqpStat.ini";
            CAutoIniFile.SaveStruct<CEqpStat>(sEqpOptnPath,"EqpStat",ref EqpStat);
        }

        public static void LoadLastInfo() 
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sLastInfoPath = sExeFolder + "SeqData\\LastInfo.ini";

            CIniFile IniLastInfo = new CIniFile(sLastInfoPath);

            //Load
            IniLastInfo.Load("LAST WORK INFO", "Device",out m_sCrntDev);
            if (m_sCrntDev == "") m_sCrntDev = "NONE";
        }
        public static void SaveLastInfo()
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sLastInfoPath = sExeFolder + "SeqData\\LastInfo.ini";

            CIniFile IniLastInfo = new CIniFile(sLastInfoPath);
            
            //Save
            IniLastInfo.Save("LAST WORK INFO", "Device", m_sCrntDev);
        }

        
    };
}
