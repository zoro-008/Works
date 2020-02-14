using COMMON;

namespace Machine
{

    //---------------------------------------------------------------------------
    public static class OM
    {
        enum EN_WORK_MODE { wmNormal = 0, wmHeight = 1 }; //0:정상 작업 1:로더에서 꺼내어 로테이터에서 높이 측정만 하고 다시 넣는다.
        public struct CDevInfo
        {   //device에 대한 Dev_Info
            public int    iTrayColCnt  ;
            public int    iTrayRowCnt  ;
            public double dTrayColPitch;
            public double dTrayRowPitch;

            public double dTrayHeight   ;

            public int    iTrayMaxLoading ; 

        } ;

        public struct CDevOptn             //Device에 따라 바뀌는 옵션.
        {
            public int    iLDRTrayCheckTime; 

            public string sLaserProject    ; //도미노 쪽 로딩해야 할 프로젝트
            public int    iStartSerial     ; //도미노 쪽에 시리얼 스타트 번호.

            public bool   bUseSerialDMC    ; //시리얼번호와 DMC사용 여부.
            

        } ;


        public struct CCmnOptn //장비 공용 옵션.
        {
            public bool bIgnrDoor;
            public bool bMarkSkip;
            public bool bBarSkip ;
            

        } ;

        //Master Option.
        public struct CMstOptn                  //마스타 옵션. 화면상에 있음 FrmMain 디바이스 라벨 더블 클릭.
        {
            public bool   bDebugMode; //true = 로그기능강화, 디버깅시 타임아웃무시.
            public bool   bIdleRun  ; //IdleRun 장비 통신 및 비전 테스트.
            public double dTrgOfs   ; //연속 트리거 거리 오프셑
        };

        //Eqipment Option.
        public struct CEqpOptn                 //모델별 하드웨어 옵션.  화면상에 없고 텍스트 파일에 있음.
        {
            public string sModelName;
        } ;

        //Eqipment Option.
        public struct CEqpStat                 //모델별 하드웨어 옵션.  화면상에 없고 텍스트 파일에 있음.
        {
            public int iSerialNo ;
        };

        public static string m_sCrntDev; //Current open device.
        
        public static CDevInfo DevInfo;
        public static CDevOptn DevOptn;
        public static CCmnOptn CmnOptn;
        public static CMstOptn MstOptn;
        public static CEqpOptn EqpOptn;
        public static CEqpStat EqpStat;


        public static void Init()
        {
            //Load
            LoadLastInfo();
            LoadMstOptn();
            LoadEqpOptn();
            LoadEqpStat();
            LoadCmnOptn();
            LoadJobFile(m_sCrntDev);
        }
        public static void Close()
        {
            SaveLastInfo();
            SaveEqpStat();
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
            LoadDevOptn(_sDevName);

            //Set Current Device Name.
            SetCrntDev(_sDevName);
        }
        public static void SaveJobFile(string _sDevName)
        {
            SaveDevInfo(_sDevName);
            SaveDevOptn(_sDevName);

            //Set Current Device Name.
            SetCrntDev(_sDevName);
        }

        public static void LoadDevInfo(string _sJobName)
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevInfoPath = sExeFolder + "JobFile\\" + _sJobName + "\\DevInfo.ini";

            CIniFile IniDevInfo = new CIniFile(sDevInfoPath);

            IniDevInfo.Load("DevInfo", "iTrayColCnt"    , ref DevInfo.iTrayColCnt   );
            IniDevInfo.Load("DevInfo", "iTrayRowCnt"    , ref DevInfo.iTrayRowCnt   );
            IniDevInfo.Load("DevInfo", "dTrayColPitch"  , ref DevInfo.dTrayColPitch );
            IniDevInfo.Load("DevInfo", "dTrayRowPitch"  , ref DevInfo.dTrayRowPitch );

            IniDevInfo.Load("DevInfo", "dTrayHeight"    , ref DevInfo.dTrayHeight   );
            IniDevInfo.Load("DevInfo", "iTrayMaxLoading", ref DevInfo.iTrayMaxLoading);


        }

        public static void SaveDevInfo(string _sJobName)
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevInfoPath = sExeFolder + "JobFile\\" + _sJobName + "\\DevInfo.ini";

            //File.Delete(sDevInfoPath);
            CIniFile IniDevInfo = new CIniFile(sDevInfoPath);

            IniDevInfo.Save("DevInfo", "iTrayColCnt"   , DevInfo.iTrayColCnt   );
            IniDevInfo.Save("DevInfo", "iTrayRowCnt"   , DevInfo.iTrayRowCnt   );
            IniDevInfo.Save("DevInfo", "dTrayColPitch" , DevInfo.dTrayColPitch );
            IniDevInfo.Save("DevInfo", "dTrayRowPitch" , DevInfo.dTrayRowPitch );     
            
            IniDevInfo.Save("DevInfo", "dTrayHeight"   , DevInfo.dTrayHeight   );
            IniDevInfo.Save("DevInfo", "iTrayMaxLoading",DevInfo.iTrayMaxLoading);
        }

        public static void LoadDevOptn(string _sJobName) 
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevOptnPath = sExeFolder + "JobFile\\" + _sJobName + "\\DevOptn.ini";

            CIniFile IniDevOptn = new CIniFile(sDevOptnPath);

            IniDevOptn.Load("DevOptn", "iLDRTrayCheckTime", ref DevOptn.iLDRTrayCheckTime);
            IniDevOptn.Load("DevOptn", "sLaserProject"    , ref DevOptn.sLaserProject    );
            IniDevOptn.Load("DevOptn", "iStartSerial"     , ref DevOptn.iStartSerial     ); 
            IniDevOptn.Load("DevOptn", "bUseSerialDMC"    , ref DevOptn.bUseSerialDMC    ); 

        }

        public static void SaveDevOptn(string _sJobName) 
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevOptnPath = sExeFolder + "JobFile\\" + _sJobName + "\\DevOptn.ini";

            CIniFile IniDevOptn = new CIniFile(sDevOptnPath);


            IniDevOptn.Save("DevOptn", "iLDRTrayCheckTime", DevOptn.iLDRTrayCheckTime);
            IniDevOptn.Save("DevOptn", "sLaserProject"    , DevOptn.sLaserProject    );
            IniDevOptn.Save("DevOptn", "iStartSerial"     , DevOptn.iStartSerial     );
            IniDevOptn.Save("DevOptn", "bUseSerialDMC"    , DevOptn.bUseSerialDMC    ); 

            
        }

        public static void LoadCmnOptn() 
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sCmnOptnPath = sExeFolder + "Util\\CmnOptn.ini";

            CIniFile IniCmnOptn = new CIniFile(sCmnOptnPath);

            IniCmnOptn.Load("CmnOptn", "bIgnrDoor"     , ref CmnOptn.bIgnrDoor     );                                                                                   
            IniCmnOptn.Load("CmnOptn", "bVisnSkip"     , ref CmnOptn.bMarkSkip     );
            IniCmnOptn.Load("CmnOptn", "bBarSkip"      , ref CmnOptn.bBarSkip      );

            

            

        }
        public static void SaveCmnOptn()
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sCmnOptnPath = sExeFolder + "Util\\CmnOptn.ini";

            CIniFile IniCmnOptn = new CIniFile(sCmnOptnPath);

            IniCmnOptn.Save("CmnOptn", "bIgnrDoor"     , CmnOptn.bIgnrDoor     );                                                                               
            IniCmnOptn.Save("CmnOptn", "bVisnSkip"     , CmnOptn.bMarkSkip     );
            IniCmnOptn.Save("CmnOptn", "bBarSkip"      , CmnOptn.bBarSkip      );

            
        }

        public static void LoadMstOptn() 
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sMstOptnPath = sExeFolder + "Util\\MstOptn.ini";

            CIniFile IniMstOptn = new CIniFile(sMstOptnPath);

            IniMstOptn.Load("MstOptn", "bDebugMode     "  , ref MstOptn.bDebugMode   );
            IniMstOptn.Load("MstOptn", "bIdleRun       "  , ref MstOptn.bIdleRun     );

            IniMstOptn.Load("MstOptn", "dTrgOfs        "  , ref MstOptn.dTrgOfs      );
            
        }

        public static void SaveMstOptn()
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sMstOptnPath = sExeFolder + "Util\\MstOptn.ini";

            CIniFile IniMstOptn = new CIniFile(sMstOptnPath);

            IniMstOptn.Save("MstOptn", "bDebugMode     "  , MstOptn.bDebugMode       );
            IniMstOptn.Save("MstOptn", "bIdleRun       "  , MstOptn.bIdleRun         );
            
            IniMstOptn.Save("MstOptn", "dTrgOfs        "  , MstOptn.dTrgOfs          );
            
        }

        //여기부터
        public static void LoadEqpOptn()
        {
            string sModelName = "PMK-110";
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sEqpOptnPath = sExeFolder + "Util\\EqpOptn.ini";

            CIniFile IniEqpOptn = new CIniFile(sEqpOptnPath);

            IniEqpOptn.Load("DevInfo", "sModelName", ref EqpOptn.sModelName);
            
            if (EqpOptn.sModelName != "") EqpOptn.sModelName = sModelName;
        }
        public static void SaveEqpOptn()
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sEqpOptnPath = sExeFolder + "Util\\EqpOptn.ini";

            CIniFile IniEqpOptn = new CIniFile(sEqpOptnPath);

            IniEqpOptn.Save("DevInfo", "sModelName", EqpOptn.sModelName);
        }

        //여기부터
        public static void LoadEqpStat()
        {
            string sModelName = "PMK-110";
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sEqpOptnPath = sExeFolder + "Util\\EqpStat.ini";

            CIniFile IniEqpOptn = new CIniFile(sEqpOptnPath);

            IniEqpOptn.Load("EqpStat", "iSerialNo", ref EqpStat.iSerialNo);

            if (EqpOptn.sModelName != "") EqpOptn.sModelName = sModelName;
        }
        public static void SaveEqpStat()
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sEqpOptnPath = sExeFolder + "Util\\EqpStat.ini";

            CIniFile IniEqpOptn = new CIniFile(sEqpOptnPath);

            IniEqpOptn.Save("EqpStat", "iSerialNo", EqpStat.iSerialNo);
        }

        public static void LoadLastInfo() 
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sLastInfoPath = sExeFolder + "SeqData\\LastInfo.ini";

            CIniFile IniLastInfo = new CIniFile(sLastInfoPath);

            //Load
            IniLastInfo.Load("LAST WORK INFO", "Device",ref m_sCrntDev);

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
