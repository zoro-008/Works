using COMMON;


//using System.Runtime.InteropServices;
//using SML.CXmlBase;
//using SMLDefine;
//using SMLApp;

namespace Machine
{
              
    //---------------------------------------------------------------------------
    public static class OM
    {

        public struct CDressyInfo
        {
            public string sAppPath1; //DressyIOS
            public string sAppName1; //
            public int    iPosX1   ; //Window Position X
            public int    iPosY1   ; //Window Position Y
        }

        public struct CEzSensor
        {
            public string sAppPath1; //EzSensor Exe Folder      
            public string sAppName1; //EzSensor Application name
            public int iPosX1; //Window Position X(EzSensor)
            public int iPosY1; //Window Position Y(EzSensor)
        }

        public struct CDevInfo
        {   //device에 대한 Dev_Info
            public int    iL_Mode     ;
            public int    iL_Motr     ;

            public double dL_H_Weight ;
            public double dL_H_Height ;
            public double dL_H_Acc    ;
            public double dL_H_Vel    ;
            public double dL_H_Dcc    ;
            public int    iL_H_Time   ;
            public int    iL_H_Count  ;
            public double dL_H_Over   ;
            public int    iL_H_Wait   ;
            public double dL_H_ZeroOfs1;
            public double dL_H_ZeroOfs2;

            public double dL_W_Weight ;
            public double dL_W_Acc    ;
            public double dL_W_Vel    ;
            public double dL_W_Dcc    ;
            public int    iL_W_Time   ;
            public int    iL_W_Count  ;
            public double dL_W_Over   ;
            public int    iL_W_Wait   ;

            public double dL_D_Weight ;
            public double dL_D_Height ;
            public int    iL_D_Time   ;
            public double dL_D_Over   ;

            public double dL_G_Height1;
            public double dL_G_Height2;
            public double dL_G_Acc    ;
            public double dL_G_Vel    ;
            public double dL_G_Dcc    ;
            public int    iL_G_Time   ;
            public int    iL_G_Count  ;
            public double dL_G_Over   ;
            public int    iL_G_Wait1  ;
            public int    iL_G_Wait2  ;


            public int    iR_Mode     ;

            public double dR_H_Weight ;
            public double dR_H_Height ;
            public double dR_H_Acc    ;
            public double dR_H_Vel    ;
            public double dR_H_Dcc    ;
            public int    iR_H_Time   ;
            public int    iR_H_Count  ;
            public double dR_H_Over   ;
            public int    iR_H_Manual ;
            public int    iR_H_Wait   ;
            public double dR_H_ZeroOfs1;

            public double dR_W_Weight ;
            public double dR_W_Acc    ;
            public double dR_W_Vel    ;
            public double dR_W_Dcc    ;
            public int    iR_W_Time   ;
            public int    iR_W_Count  ;
            public double dR_W_Over   ;
            public int    iR_W_Manual ;
            public int    iR_W_Wait   ;

            public double dR_P_Height ;
            public double dR_P_Acc    ;
            public double dR_P_Vel    ;
            public double dR_P_Dcc    ;
            public int    iR_P_Time   ;
            public int    iR_P_Count  ;
            public double dR_P_Over   ;
            public int    iR_P_Wait   ;
            

            public string sL_Name     ;
            public int    iL_UsbCnt   ;
            public string sR_Name     ;
            public int    iR_UsbCnt   ;
                                      
        } ;                                 
                                            
        public struct CCmnOptn //장비 공용 옵션.
        {
            public bool   bIgnrDoor           ;
            public bool   bIgnrCam            ;
            //public bool   bUse_L_Zero         ;
            //public bool   bUse_R_Zero         ;

            public string sLeftFolder         ;
            public string sRighFolder         ;

            public bool   bUse_L_Part         ;
            public bool   bUse_R_Part         ;

            public bool   bUse_L_Cort         ; //교정하기 내려서 하중 맞추기 옵션.
            public bool   bUse_R_Cort         ;

            public bool   bUse_L_Dark         ;
            public bool   bUse_R_Dark         ;

            public double dLoadOfs1           ;
            public double dLoadOfs2           ;
            public double dLoadOfs3           ;
            public int    iLoadTime           ;
            public double dLoadRange          ;

            public double dLS1 ;
            public double dLS2 ;
            public double dLS3 ;
            public double dLS4 ;
            public double dLS5 ;
            public double dLF1 ;
            public double dLF2 ;
            public double dLF3 ;
            public double dLF4 ;
            public double dLF5 ;


        } ;

        //Master Option.
        public struct CMstOptn                  //마스타 옵션. 화면상에 있음 FrmMain 디바이스 라벨 더블 클릭.
        {
            public bool   bDebugMode    ; //true = 로그기능강화, 디버깅시 타임아웃무시.
            public bool   bIdleRun      ; //IdleRun 장비 통신 및 비전 테스트.
            public double dTrgOfs       ; //연속 트리거 거리 오프셑
            
        };

        //Eqipment Option.
        public struct CEqpOptn                 //모델별 하드웨어 옵션.  화면상에 없고 텍스트 파일에 있음.
        {
        } ;

        public struct CEqpStat
        {
            public int    iLDRSplyCnt   ;
            public double dLastIDXRPos  ;
            public double dLastIDXFPos  ;
            public bool   bMaint        ;
            public double dTrayWorkTime ;
            public double dTrayUPH      ;
            public int    iBrcdRemoveCnt;
        } ;



        public static string m_sCrntDev; //Current open device.
        
        public static CDevInfo DevInfo;
        public static CCmnOptn CmnOptn;
        public static CMstOptn MstOptn;
        public static CEqpOptn EqpOptn;
        public static CEqpStat EqpStat;

        public static CDressyInfo DressyInfo;
        public static CEzSensor EzSensor;
        //public static CArray TrayMask = new CArray();



        public static void Init()
        {
            //Load
            LoadLastInfo();
            LoadMstOptn();
            LoadEqpOptn();
            LoadDressyInfo();
            LoadCmnOptn();
            LoadEqpStat();
            LoadJobFile(m_sCrntDev);
            //LoadTrayMask(m_sCrntDev);


        }
        public static void Close()
        {
            SaveDressyInfo();
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

        public static void LoadDressyInfo()
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevInfoPath = sExeFolder + "Util\\DressyInfo.ini";
            CAutoIniFile.LoadStruct<CDressyInfo>(sDevInfoPath, "DressyInfo", ref DressyInfo);
        }

        public static void SaveDressyInfo()
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevInfoPath = sExeFolder + "Util\\DressyInfo.ini";
            CAutoIniFile.SaveStruct<CDressyInfo>(sDevInfoPath, "DressyInfo", ref DressyInfo);
        }

        public static void LoadEzSensor(string _sJobName)
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevInfoPath = sExeFolder + "Util\\EzSensor.ini";
            CAutoIniFile.LoadStruct<CEzSensor>(sDevInfoPath, "EzSensor", ref EzSensor);
        }

        public static void SaveEzSensor(string _sJobName)
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevInfoPath = sExeFolder + "Util\\EzSensor.ini";
            CAutoIniFile.SaveStruct<CEzSensor>(sDevInfoPath, "EzSensor", ref EzSensor);
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
