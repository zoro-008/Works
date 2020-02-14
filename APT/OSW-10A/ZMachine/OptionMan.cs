using COMMON;
using System.IO;


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
            public int    iTRAY_PcktCntX   ;
            public int    iTRAY_PcktCntY   ;
            public double dTRAY_PcktPitchX ;
            public double dTRAY_PcktPitchY ;
            public int    iTRAY_StackingCnt;
            public int    iTRAY_BundleCnt  ;
            
        } ;                                 
                                            
        public struct CDevOptn               //Device에 따라 바뀌는 옵션.
        {                                   
            //                              
            public int    iPickDelay         ;     
            public int    iPlceDelay         ;
                                             
            public int    iInspSpeed         ; //Vision 검사 시 인덱스 속도
            //public double dVisnTolXY         ; //절대값 Stage 검사시에 검사 허용 치수.
            //public double dVisnTolAng        ; //절대값 각도.
            public bool   bUseBtmCover       ; //트레이 맨밑에 한장 엠티커버 트레이 넣는 옵션.
            public int    iTotalInspCnt      ; //1~4까지 설정 가능 하게 콤보박스. 한패키지 검사 횟수. 제일큰건 4방까지 가능.
            public int    iNgInspCnt         ;
            public double dTrgOfs            ;
            public int    iVisnNGCntErr      ; //한줄 검사시에 검사에러가 이거 이상 나오면 에러.

            public bool   bUnitID             ;
            public bool   bDuplicateDMC1      ;
            public bool   bDMC1Grouping       ;
            public int    iDMC1MonthLimit     ;//검사 기간 설정.
            public int    iDMC2CheckMathod    ;//DMC2검사는 2가지 방법이 있어서 그거 설정.
            public bool   bBrightnessCheck    ;
            public bool   bLDOMCheck          ;
            public bool   bCxCy               ;

            public int    iCompareDmc2Cnt     ;//DMC2 검사 중에 스트링 컴페어 방식일때 DB에 있는 거랑 비교시에 처음엔 12개로 픽스였다가 180118에 추한테 연락와서 옵션처리함.\
            public bool   bUseDmc2CharLimit   ;//DMC2 검사 중에 전체 글자수 카운트 사용여부.
            public int    iDmc2CharLimit      ;//DMC2 검사 에서 허용 글자수.

            public bool   bUseBarcCyl         ; //바코드존 실린더 올려서 작업할지 정하는 옵션
        } ;


        public struct CCmnOptn //장비 공용 옵션.
        {
            //public bool   bIgnrDoor           ;
            public bool   bLoadingStop        ;
            public int    iEmptyCheckPrcs     ; //0 : Error, 1 : Reject
            public bool   bIdleRun            ;

            public string sNG0                ;
            public string sNG1                ;
            public string sNG2                ;
            public string sNG3                ;
            public string sNG4                ;
            public string sNG5                ;
            public string sNG6                ;
            public string sNG7                ;
            public string sNG8                ;
            public string sNG9                ;
            public string sNG10               ;

            public bool   bOracleNotUse       ;
            public string sOracleIP           ;
            public string sOraclePort         ;
            public string sOracleID           ;
            public string sOraclePassword     ;
            public string sOracleSID          ;//서비스아이디
            public bool   bOracleNotWriteVIT  ;
            public bool   bOracleNotWriteInsp ;
            public bool   bOracleNotWriteVITFile ;
            public bool   bUseApTestTable     ;



            public double dIdxFSplyPos        ; //작업중인 인덱스 트레이에서 해당 포지션을 넘어가야지 로더 공급을 한다.
            public double dIdxRSplyPos        ; //작업중인 인덱스 트레이에서 해당 포지션을 넘어가야지 로더 공급을 한다.

            public int    iBarcYOffset        ; //Barcode Print Y Offset
            public int    iBarcToff           ; //Barcode Print Toff    
            
            public int    iBrcdPickDelay      ;                  
                                              
            public bool   bIdxFSkip           ; //Index Front Skip
            public bool   bIdxRSkip           ; //Index Rear Skip

            public bool   bGoldenTray         ;
            public int    iInspCrvrTray       ; //Empty vision 검사 옵션.
            public int    iGoodPickMissCnt    ; //Good Pick 실패시 에러 띄우는 횟수
            public int    iPickRtryCnt        ;

            public int    iLotEndDelay        ; //밴딩기로 넘기고 바로 랏엔드 하던것에 문제 있어서 딜레이 주고 밴딩 대략 한 시점에 랏엔드 하게.


            public string sBackupFolder       ; //데이터 백업 폴더 설정.

            public string sMachinID           ;

            public string sVITFolder          ; //VIT파일 랏엔드시에 남기는 루트

            public bool   bSkipBarAttach      ; //바코드 붙이는것 스킵.

            public bool   bIdxDetectVisnZone  ; //인덱스가 비전 첫 포지션에 갔을때 검사 하는것.
            
        } ;

        //Master Option.
        public struct CMstOptn                  //마스타 옵션. 화면상에 있음 FrmMain 디바이스 라벨 더블 클릭.
        {
            public bool   bDebugMode    ; //true = 로그기능강화, 디버깅시 타임아웃무시.
            //public bool   bIdleRun      ; //IdleRun 장비 통신 및 비전 테스트.
            public double dTrgOfs       ; //연속 트리거 거리 오프셑
            
        };

        //Eqipment Option.
        public struct CEqpOptn                 //모델별 하드웨어 옵션.  화면상에 없고 텍스트 파일에 있음.
        {
        } ;

        public struct CEqpStat
        {
            //public int    iLDRSplyCnt   ;
            public double dLastIDXRPos  ;
            public double dLastIDXFPos  ;
            public bool   bMaint        ;
            public double dTrayWorkTime ;
            public double dTrayUPH      ;
            public int    iBrcdRemoveCnt;

            public string sTraySttTime  ;
            public string sLotSttTime   ;
         
            public bool   bWrapingEnd   ;//dont supply Flag
            //About Oracle.
            public string sTrayLabel    ;

            //Barcode Printer ;
            public string sPrintedBarcode ;

            public int    iWorkBundle   ; // 1랏이 여러 뭉테기로 되어 있음.  30Tray(1Lot) = 10Tray(1Bundle) * 3 ;

            //public string sLotOpenID    ; // 장비로그온 ID와 랏을 운용 하는 사람의 아이디를 분리 하여 랏오픈시에 입력 하게 함.
        } ;

        static public int GetSupplyCnt()
        {
            int iCnt = 0 ;
            if (!DM.ARAY[ri.SPLR].CheckAllStat(cs.None))
            {
                iCnt++;
            }
            if (!DM.ARAY[ri.IDXF].CheckAllStat(cs.None))
            {
                iCnt++;
            }
            if (!DM.ARAY[ri.IDXR].CheckAllStat(cs.None))
            {
                iCnt++;
            }
            if (!DM.ARAY[ri.OUTZ].CheckAllStat(cs.None))
            {
                iCnt++;
            }
            if (!DM.ARAY[ri.PSTC].CheckAllStat(cs.None))
            {
                iCnt++;
            }
            if (DM.ARAY[ri.STCK].GetCntStat(cs.Good)!=0)
            {
                iCnt+=DM.ARAY[ri.STCK].GetCntStat(cs.Good);
            }
            if (!DM.ARAY[ri.BARZ].CheckAllStat(cs.None))
            {
                iCnt+= OM.DevInfo.iTRAY_StackingCnt ;
            }
            return iCnt ;

        }



        public static string m_sCrntDev; //Current open device.
        
        public static CDevInfo DevInfo;
        public static CDevOptn DevOptn;
        public static CCmnOptn CmnOptn;
        public static CMstOptn MstOptn;
        public static CEqpOptn EqpOptn;
        public static CEqpStat EqpStat;

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

        public static bool LoadJobFile(string _sDevName) 
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevInfoPath = sExeFolder + "JobFile\\" + _sDevName ;
            if(!Directory.Exists(sDevInfoPath)) return false ;
            LoadDevInfo(_sDevName);
            LoadDevOptn(_sDevName);

            //Set Current Device Name.
            SetCrntDev(_sDevName);

            return true ;
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
