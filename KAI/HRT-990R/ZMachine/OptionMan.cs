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
        public struct CInfo
        {
            public bool   bImageSave       ;
            public string sImageFileName   ;
            public int    iImageSaveCnt    ;

            public int    iImageSave       ;

            public bool   bTempSave        ;
            public int    iTempSaveInterval;
            public string sTempFileName    ;


        }
        public struct CDevInfo
        {   //device에 대한 Dev_Info
            public int iNo1 ;

            
        } ;                                 
                                            
        public struct CCmnOptn //장비 공용 옵션.
        {
            public bool   bIgnrDoor   ;
            public int    iLsrMaxDelay;
            public double dMagnetCheck;
            public double dLaserCheck ;
            public double dLsrSttPos  ;

        } ;

        //Master Option.
        public struct CMstOptn                  //마스타 옵션. 화면상에 있음 FrmMain 디바이스 라벨 더블 클릭.
        {
            public bool   bDebugMode    ; //true = 로그기능강화, 디버깅시 타임아웃무시.
            public bool   bIdleRun      ; //IdleRun 장비 통신 및 비전 테스트.
            
        };

        //Eqipment Option.
        public struct CEqpOptn                 //모델별 하드웨어 옵션.  화면상에 없고 텍스트 파일에 있음.
        {
            public string sEquipName    ;
            public string sEquipSerial  ;
            public bool   bIgnrSptr     ;
        } ;

        public struct CEqpStat
        {
            public bool   bMaint        ;
            //public int    iULDRWorkCnt  ; //언로더 현재 작업중인거.

        } ;

        public struct CSptStat
        {
            public int iIntergrationTime;
            public int iScanAveraging   ;
            public int iBoxcar          ;
            public bool bEDC            ;
            public bool bNLC            ;
            public bool bComputeNoise   ;
            public int  iRMS            ;
            public bool bEnable         ;
            public bool bSaveAll        ;
            public int  iSaveEvery      ;
            public int  iMaxScan        ;
            public int  iStopAfter      ;
            public bool bApplyIrradiance;
            public bool bStrobeEnable   ;

            //public int    iULDRWorkCnt  ; //언로더 현재 작업중인거.

        };



        public static string m_sCrntDev; //Current open device.
        
        public static CInfo    Info   ;
        public static CDevInfo DevInfo;
        public static CCmnOptn CmnOptn;
        public static CMstOptn MstOptn;
        public static CEqpOptn EqpOptn;
        public static CEqpStat EqpStat;
        public static CSptStat SptStat;

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

            LoadSptStat();
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
            CAutoIniFile.LoadStruct(sDevInfoPath,"DevInfo",ref DevInfo);          
        }

        public static void SaveDevInfo(string _sJobName)
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevInfoPath = sExeFolder + "JobFile\\" + _sJobName + "\\DevInfo.ini";
            CAutoIniFile.SaveStruct(sDevInfoPath,"DevInfo",ref DevInfo);   
        }

        public static void LoadCmnOptn() 
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sCmnOptnPath = sExeFolder + "Util\\CmnOptn.ini";
            CAutoIniFile.LoadStruct(sCmnOptnPath,"CmnOptn",ref CmnOptn);
        }
        public static void SaveCmnOptn()
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sCmnOptnPath = sExeFolder + "Util\\CmnOptn.ini";
            CAutoIniFile.SaveStruct(sCmnOptnPath,"CmnOptn",ref CmnOptn);
            
        }

        public static void LoadMstOptn() 
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sMstOptnPath = sExeFolder + "Util\\MstOptn.ini";
            CAutoIniFile.LoadStruct(sMstOptnPath,"MstOptn",ref MstOptn);
            
        }

        public static void SaveMstOptn()
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sMstOptnPath = sExeFolder + "Util\\MstOptn.ini";
            CAutoIniFile.SaveStruct(sMstOptnPath,"MstOptn",ref MstOptn);
            
        }

        public static void LoadEqpOptn()
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sEqpOptnPath = sExeFolder + "Util\\EqpOptn.ini";
            CAutoIniFile.LoadStruct(sEqpOptnPath,"EqpOptn",ref EqpOptn);
        }
        public static void SaveEqpOptn()
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sEqpOptnPath = sExeFolder + "Util\\EqpOptn.ini";
            CAutoIniFile.SaveStruct(sEqpOptnPath,"EqpOptn",ref EqpOptn);
        }
        public static void LoadEqpStat()
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sEqpOptnPath = sExeFolder + "Util\\EqpStat.ini";
            CAutoIniFile.LoadStruct(sEqpOptnPath,"EqpStat",ref EqpStat);
        }
        public static void SaveEqpStat()
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sEqpOptnPath = sExeFolder + "Util\\EqpStat.ini";
            CAutoIniFile.SaveStruct(sEqpOptnPath,"EqpStat",ref EqpStat);
        }
        public static void LoadSptStat()
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sSptOptnPath = sExeFolder + "Util\\SptStat.ini";
            CAutoIniFile.LoadStruct(sSptOptnPath, "SptStat", ref SptStat);
        }
        public static void SaveSptStat()
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sSptOptnPath = sExeFolder + "Util\\SptStat.ini";
            CAutoIniFile.SaveStruct(sSptOptnPath, "SptStat", ref SptStat);
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
