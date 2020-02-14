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
        public struct CDevInfo
        {   
        } ;

                     
                                            
        public struct CDevOptn               //Device에 따라 바뀌는 옵션.
        {                                   
            //                              
            public string sPattern           ; 
            public int    iRepeatCnt         ;     
            public int    iCogRotation       ;
            public double dLeftRightDist     ;
            public int    iLotWorkCount      ;
            public int    iGuidStopDealy     ; //가이드 앞뒤 끝에서 멈추고 먹는 딜레이.

            public int    iWRK_TSwit0Work    ; //패턴별 작업 방향 0:왼쪽,1:오른쪽
            public int    iWRK_TSwit1Work    ; //패턴별 작업 방향 0:왼쪽,1:오른쪽
            public int    iWRK_TSwit2Work    ; 
            public int    iWRK_TSwit3Work    ; 
            public int    iWRK_TSwit4Work    ; 
            public int    iWRK_TSwit5Work    ; 
            public int    iWRK_TSwit6Work    ; 
            public int    iWRK_TSwit7Work    ; 
            public int    iWRK_TSwit8Work    ; 
            public int    iWRK_TSwit9Work    ; 
        } ;


        public struct CCmnOptn //장비 공용 옵션.
        {            
            public bool   bRewindMode ;   
            public bool   bUseRear    ;
            public bool   bUseFrnt    ;
            public bool   bUseYBfPos  ;  //블레이드Y 대기포지션 사용.
            public double dRewindRVel ;
            public double dRewindYVel ;
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
        } ;

        public struct CEqpStat
        {
            public bool   bMaint          ; //Failure 기록용. 
            public bool   bOutGuideToRear ; //아웃쪽 실 가이드모터가 뒤쪽으로 가고 있는지 
            public int    iWorkStep       ; //WorkInfoList 에서 현재 작업중인 인덱스 번호.
            public int    iWorkCount      ; //작업중인 갯수.

            public double dWorkTime       ; //1개 작업시 걸린시간. 
            public double dUPH            ; //1개 작업시 걸린시간으로 1시간에 얼마나 생산가능한지 역산.  

            public double dCutTime        ; //1번 커팅 하는데 걸린시간. 
        } ;



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
            LoadCmnOptn();
            LoadEqpStat();
            LoadJobFile(m_sCrntDev);
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
            CAutoIniFile.LoadStruct<CDevInfo>(sDevInfoPath,"DevInfo",ref DevInfo);          
        }

        public static void SaveDevInfo(string _sJobName)
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevInfoPath = sExeFolder + "JobFile\\" + _sJobName + "\\DevInfo.ini";
            CAutoIniFile.SaveStruct<CDevInfo>(sDevInfoPath,"DevInfo",ref DevInfo);   
        }

        public static void LoadDevOptn(string _sJobName) 
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevOptnPath = sExeFolder + "JobFile\\" + _sJobName + "\\DevOptn.ini";
            CAutoIniFile.LoadStruct<CDevOptn>(sDevOptnPath,"DevOptn",ref DevOptn);               
        }

        public static void SaveDevOptn(string _sJobName) 
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevOptnPath = sExeFolder + "JobFile\\" + _sJobName + "\\DevOptn.ini";
            CAutoIniFile.SaveStruct<CDevOptn>(sDevOptnPath,"DevOptn",ref DevOptn);
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
