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
        {   //device에 대한 Dev_Info
            //Rail Option
            public double dRailClnSpeed; //Rail Cleaning Speed

            //Left Picker Option
            public double dLPCKClnSpeed; //Left Picker Cleaning Speed
            public int    iLPCKBfPickDly;
            public int    iLPCKPickDly ; //Left Picker Pick Delay
            public int    iLPCKPlaceDly; //Left Picker Place Delay

            //Right Picker
            public int    iRPCKPickDly ; //Left Picker Pick Delay
            public int    iRPCKPlaceDly; //Left Picker Place Delay
            
            //Vacuum Stage
            public double dVSTGClnSpeed; //Vacuum Stage Cleaning Speed

            public int    iPSTR_OutDelay;
           
        } ;                                 
                                            
        public struct CCmnOptn //장비 공용 옵션.
        {
            public bool   bIgnrDoor           ; //도어 스킵.

            public bool   bLPCK_IonOff        ;
            public bool   bPSTR_IonBtmOff     ;
            public bool   bPSTR_IonTopOff     ;
            public bool   bVSTG_IonOff        ;

            public bool   bLPCK_Air1Off        ;
            public bool   bPSTR_Air1BtmOff     ;
            public bool   bPSTR_Air1TopOff     ;
            public bool   bVSTG_Air1Off        ;

            public bool   bLPCK_Air2Off        ;
            public bool   bPSTR_Air2BtmOff     ;
            public bool   bPSTR_Air2TopOff     ;
            public bool   bVSTG_Air2Off        ;

            public double dTrgOfs             ; //X비전 트리거 오프셑.

            public string sLdrPreLotNo ;
            public string sLdrPreLotId ;

        } ;  

        //Master Option.
        public struct CMstOptn                  //마스타 옵션. 화면상에 있음 FrmMain 디바이스 라벨 더블 클릭.
        {
            public bool   bDebugMode    ; //true = 로그기능강화, 디버깅시 타임아웃무시.
            public bool   bIdleRun      ; //IdleRun 장비 통신 및 비전 테스트.
            
            public bool   bMarkAlgin    ; //마킹 얼라인 타이밍을 미리 수정해서 옵션으로 뺴놓음.

            public string sVisnPath     ; //비전관련 인터페이스 폴더.

            public bool   bUseAutoShutDown ; //드라이런닝 시작하고 타이머 지났을때 PC 종료하도록 설정하는 옵션
            public string sShutDownTimer; //셧다운 타이머 실제 시간

            //충돌조건 옵션
            //조그버튼 움직일때 충돌조건 
            
        };

        //Eqipment Option.
        public struct CEqpOptn                 //모델별 하드웨어 옵션.  화면상에 없고 텍스트 파일에 있음.
        {
            public string sEquipName   ;
            public string sEquipSerial ;

            public string sVisnPath    ;
        } ;



        public struct CEqpStat
        {
            //public string sVsn1LastLot ;
            //public string sVsn2LastLot ;
            //public string sVsn3LastLot ;
            //
            //public int    iLodrLastStep; //Loader     마지막 CycleStep
            //public int    iWorkLastStep; //WorkZone   마지막 CycleStep
            //public int    iUldrLastStep; //Unloader   마지막 CycleStep
            //
            //public double dLodrCycleTime;
            //public double dWorkCycleTime;
            //public double dUldrCycleTime;
            //
            //public string sBarcode     ; //바코드 저장

            public int    iTotalWorkCnt;//장비 총 작업 수량(클리어 안함)
            public double dCycleTime ;
            public double dPreSttTime;//CycleClean End Setting
            public double dSttTime   ;//CycleSupply Start Setting
            public double dEndTime   ;


            public bool   bMaint        ;

            //SPC LOT INFOMATION
            //public int    iWorkCnt      ; //총 작업 갯수.
            //public int    iFailCnt      ; //총 불량 갯수.
            //public double dWorkUPH      ;
            //public double dWorkTime     ; //Cycle Tac Time 
            //SPC LOT DAY INFORMATION
            public int    iDayWorkCnt   ;//장비 하루 작업 수량(하루마다 클리어)
            public double iDayWorkUPH   ;//장비 하루 작업 수량(하루마다 클리어)

            //public int[]  iRsltCnts     ; //이번랏.
            //public int[]  iPreRsltCnts  ; //이건 전에랏 
            //
            //public int    iReinputCnt   ; //포스트 버퍼에 메뉴얼로 넣은 카운트 랏오픈시에 클리어됌.
        } ;


        public static string m_sCrntDev; //Current open device.
        
        public static CDevInfo DevInfo;
        public static CCmnOptn CmnOptn;
        public static CMstOptn MstOptn;
        public static CEqpOptn EqpOptn;
        public static CEqpStat EqpStat;

        public static ArrayPos StripPos = new ArrayPos();


        //public static bool VsSkip(vi _iVid)
        //{
        //         if (_iVid == vi.Vs1L) return OM.DevInfo.bVs1_Skip || OM.DevInfo.bVsL_NotUse ; 
        //    else if (_iVid == vi.Vs1R) return OM.DevInfo.bVs1_Skip || OM.DevInfo.bVsR_NotUse ;
        //    else if (_iVid == vi.Vs2L) return OM.DevInfo.bVs2_Skip || OM.DevInfo.bVsL_NotUse ;
        //    else if (_iVid == vi.Vs2R) return OM.DevInfo.bVs2_Skip || OM.DevInfo.bVsR_NotUse ;
        //    else if (_iVid == vi.Vs3L) return OM.DevInfo.bVs3_Skip || OM.DevInfo.bVsL_NotUse ;
        //    else if (_iVid == vi.Vs3R) return OM.DevInfo.bVs3_Skip || OM.DevInfo.bVsR_NotUse ;
        //    else                       return true ;
        //}


        //======================================================배열저작 테스트.
        //public struct CTest
        //{
        //    public string    sTest  ;
        //    public string [] sTests ;

        //    public int       iTest  ;
        //    public int []    iTests ;

        //    public double    dTest  ;
        //    public double [] dTests ;

        //    public bool      bTest  ;
        //    public bool []   bTests ;


        //    public CTest(int _iSize)
        //    {
        //        sTest  = "";
        //        sTests = new string [_iSize];
            
        //        iTest  = 0 ;
        //        iTests = new int    [_iSize];
            
        //        dTest  = 0.0 ;
        //        dTests = new double [_iSize];
            
        //        bTest  = false ;
        //        bTests = new bool   [_iSize];
        //    }

        //}
        //public static CTest Test = new CTest((int)cs.MAX_CHIP_STAT);
        //public static void LoadTest(string _sJobName)
        //{
        //    string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
        //    string sDevInfoPath = sExeFolder + "JobFile\\" + _sJobName + "\\Test.ini";
        //    CAutoIniFile.LoadStruct(sDevInfoPath,"Test",ref Test);          
        //}

        //public static void SaveTest(string _sJobName)
        //{
        //    string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
        //    string sDevInfoPath = sExeFolder + "JobFile\\" + _sJobName + "\\Test.ini";
        //    CAutoIniFile.SaveStruct(sDevInfoPath,"Test",ref Test);   
        //}
       //=========================================================


        public static void Init()
        {
            //EqpStat.iRsltCnts    = new int [(int)cs.MAX_CHIP_STAT];
            //EqpStat.iPreRsltCnts = new int [(int)cs.MAX_CHIP_STAT];


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
