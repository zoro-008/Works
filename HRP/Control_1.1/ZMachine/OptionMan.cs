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

            //일단 공식 그대로 쓰기위해 이름 똑같이 하고 나중에 밑에꺼 같이 한방에 바꾸자.
            public double dColPitch  ; //기본 X피치
            public double dRowPitch  ; //기본 Y피치
            public int    iColGrCnt  ; //스트립에서 X 그룹의 갯수.
            public int    iRowGrCnt  ; //스트립에서 Y 그풉의 갯수.
            public double dColGrGap  ; //1그룹마지막 자제 와 2그룹첫 자제 간의 피치.
            public double dRowGrGap  ; //1그룹마지막 자제 와 2그룹첫 자제 간의 피치.
            public int    iColCnt    ; //전체 X갯수.
            public int    iRowCnt    ; //전체 Y갯수.

            //스페셜 자제용 Row SubGroup 기본적으로 사용 안하지만 가끔 사용 자제 있음. 1그룹안에서 또 그룹이 나눠져 있을때
            //사용 안할시에는 iRowSubGroupCount==0  iColSubGroupCount==0
            public int    iColSbGrCnt ;
            public double dColSbGrGap ;
                                      
            public int    iRowSbGrCnt ;
            public double dRowSbGrGap ;

            //Loader Unloader
            public int    iMgzCntPerLot    ; //한랏당 매거진 갯수
            public double dMgzPitch        ; //매거진 피치
            public int    iMgzSlotCnt      ; //매거진 슬롯 갯수
            public int    iLdrOutDelay     ; //로더 매거진 배출후 딜레이
            public int    iUdrOutDelay     ; //언로더 매거진 배출후 딜레이
            //?? 메거진 슬롯 카운트.
            
            //Vision쪽 검사관련.
            public string sVisnIndexId ;
            public int    iColInspCnt  ;
            public int    iRowInspCnt  ;

            //비전사용 옵션. 잡파일에 따라서 사용 하지 않는 비전존이 있어서...
            //트리거까지는 날리지만 결과값이나 응답을 받지 않음.
            public bool   bVs1_Skip   ;
            public bool   bVs2_Skip   ;
            public bool   bVs3_Skip   ;

            //왼쪽 오른쪽 비전의 스트록이 안나오거나 
            //카메라 혹은 비전피씨등이 고장 났을때 한쪽 비전으로만 검사할때 사용.
            public bool   bVsL_NotUse ;
            public bool   bVsR_NotUse ;

            //레일 무빙시에 스토퍼들간의 딜레이.
            public int    iStprDnDelay ; 

            //public int    iUldOutDelay     ; //언로더에 박스 감지 센서 감지 후 컨베이어 정지 딜레이
            //public int    iUldAlignInDelay ; //언로더 Align Stopper에 센서가 박스 감지를 못해서 딜레이 먹여 세워야되는데 그때 쓰는 딜레이
            //public double dMvYRearPlaceOfs ; //CyclePlace에서 Moving E/V 한쪽방향으로 이동시켜 박스에 넣을 때 쓰는 오프셋
            //public double dWorkXBaseOfs    ; //CycleInputWork에서 베이스에 다 내려놓고 X축 움직여서 볼트 구멍 찾을 때 쓰는 오프셋
        } ;                                 
                                            
        public struct CCmnOptn //장비 공용 옵션.
        {
            public bool   bLoadStop           ; //로더 구간 작업 대기
            public bool   bIgnrDoor           ; //도어 스킵.
            public bool   bIgnrVisn           ; //Rail Feeding Only not insp.
            public bool   bIgnrMark           ; //마킹스킵

            public double dTrgOfs             ; //X비전 트리거 오프셑.


            public int iRsltLevel0 ; public string sRsltName0 ; public int iRsltColor0 ; public int iVsLim0 ; public bool bNotMark0 ;
            public int iRsltLevel1 ; public string sRsltName1 ; public int iRsltColor1 ; public int iVsLim1 ; public bool bNotMark1 ;
            public int iRsltLevel2 ; public string sRsltName2 ; public int iRsltColor2 ; public int iVsLim2 ; public bool bNotMark2 ;
            public int iRsltLevel3 ; public string sRsltName3 ; public int iRsltColor3 ; public int iVsLim3 ; public bool bNotMark3 ;
            public int iRsltLevel4 ; public string sRsltName4 ; public int iRsltColor4 ; public int iVsLim4 ; public bool bNotMark4 ;
            public int iRsltLevel5 ; public string sRsltName5 ; public int iRsltColor5 ; public int iVsLim5 ; public bool bNotMark5 ;
            public int iRsltLevel6 ; public string sRsltName6 ; public int iRsltColor6 ; public int iVsLim6 ; public bool bNotMark6 ;
            public int iRsltLevel7 ; public string sRsltName7 ; public int iRsltColor7 ; public int iVsLim7 ; public bool bNotMark7 ;
            public int iRsltLevel8 ; public string sRsltName8 ; public int iRsltColor8 ; public int iVsLim8 ; public bool bNotMark8 ;
            public int iRsltLevel9 ; public string sRsltName9 ; public int iRsltColor9 ; public int iVsLim9 ; public bool bNotMark9 ;
            public int iRsltLevelA ; public string sRsltNameA ; public int iRsltColorA ; public int iVsLimA ; public bool bNotMarkA ;
            public int iRsltLevelB ; public string sRsltNameB ; public int iRsltColorB ; public int iVsLimB ; public bool bNotMarkB ;
            public int iRsltLevelC ; public string sRsltNameC ; public int iRsltColorC ; public int iVsLimC ; public bool bNotMarkC ;
            public int iRsltLevelD ; public string sRsltNameD ; public int iRsltColorD ; public int iVsLimD ; public bool bNotMarkD ;
            public int iRsltLevelE ; public string sRsltNameE ; public int iRsltColorE ; public int iVsLimE ; public bool bNotMarkE ;
            public int iRsltLevelF ; public string sRsltNameF ; public int iRsltColorF ; public int iVsLimF ; public bool bNotMarkF ;
            public int iRsltLevelG ; public string sRsltNameG ; public int iRsltColorG ; public int iVsLimG ; public bool bNotMarkG ;
            public int iRsltLevelH ; public string sRsltNameH ; public int iRsltColorH ; public int iVsLimH ; public bool bNotMarkH ;
            public int iRsltLevelI ; public string sRsltNameI ; public int iRsltColorI ; public int iVsLimI ; public bool bNotMarkI ;
            public int iRsltLevelJ ; public string sRsltNameJ ; public int iRsltColorJ ; public int iVsLimJ ; public bool bNotMarkJ ;
            public int iRsltLevelK ; public string sRsltNameK ; public int iRsltColorK ; public int iVsLimK ; public bool bNotMarkK ;
            public int iRsltLevelL ; public string sRsltNameL ; public int iRsltColorL ; public int iVsLimL ; public bool bNotMarkL ;
            public int iVsLimT     ;

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
            public string sVsn1LastLot ;
            public string sVsn2LastLot ;
            public string sVsn3LastLot ;

            public int    iLodrLastStep; //Loader     마지막 CycleStep
            public int    iWorkLastStep; //WorkZone   마지막 CycleStep
            public int    iUldrLastStep; //Unloader   마지막 CycleStep

            public double dLodrCycleTime;
            public double dWorkCycleTime;
            public double dUldrCycleTime;

            public string sBarcode     ; //바코드 저장

            public int    iTotalWorkCnt;//장비 총 작업 수량(클리어 안함)
            


            public bool   bMaint        ;

            //SPC LOT INFOMATION
            public int    iWorkCnt      ; //총 작업 갯수.
            public int    iFailCnt      ; //총 불량 갯수.
            public double dWorkUPH      ;
            public double dWorkTime     ; //Cycle Tac Time 
            //SPC LOT DAY INFORMATION
            public int    iDayWorkCnt   ;//장비 하루 작업 수량(하루마다 클리어)
            public double iDayWorkUPH   ;//장비 하루 작업 수량(하루마다 클리어)

            public int[]  iRsltCnts     ; //이번랏.
            public int[]  iPreRsltCnts  ; //이건 전에랏 

            public int    iReinputCnt   ; //포스트 버퍼에 메뉴얼로 넣은 카운트 랏오픈시에 클리어됌.
        } ;


        public static string m_sCrntDev; //Current open device.
        
        public static CDevInfo DevInfo;
        public static CCmnOptn CmnOptn;
        public static CMstOptn MstOptn;
        public static CEqpOptn EqpOptn;
        public static CEqpStat EqpStat;

        public static ArrayPos StripPos = new ArrayPos();


        public static bool VsSkip(vi _iVid)
        {
                 if (_iVid == vi.Vs1L) return OM.DevInfo.bVs1_Skip || OM.DevInfo.bVsL_NotUse ; 
            else if (_iVid == vi.Vs1R) return OM.DevInfo.bVs1_Skip || OM.DevInfo.bVsR_NotUse ;
            else if (_iVid == vi.Vs2L) return OM.DevInfo.bVs2_Skip || OM.DevInfo.bVsL_NotUse ;
            else if (_iVid == vi.Vs2R) return OM.DevInfo.bVs2_Skip || OM.DevInfo.bVsR_NotUse ;
            else if (_iVid == vi.Vs3L) return OM.DevInfo.bVs3_Skip || OM.DevInfo.bVsL_NotUse ;
            else if (_iVid == vi.Vs3R) return OM.DevInfo.bVs3_Skip || OM.DevInfo.bVsR_NotUse ;
            else                       return true ;
        }


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
            EqpStat.iRsltCnts    = new int [(int)cs.MAX_CHIP_STAT];
            EqpStat.iPreRsltCnts = new int [(int)cs.MAX_CHIP_STAT];


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
