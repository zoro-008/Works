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
            public string s1           ; //Brand ID
            public string s2           ; //Model Name
            public string s3           ; //Part Number
            public string s4           ; //Manufacturer Name
            public string s5           ; //H/W Version
            public string s6           ; //Calibration ref site location
            public string s7           ; //Calibration reference system
            public string s8           ; //Active Area_UP
            public string s9           ; //Active Area_Down
            public string sA           ; //Active Area_Left
            public string sB           ; //Active Area_Right
            public string sC           ; //ENP_Inspection Width
            public string sD           ; //ENP_Inspection Height

            public string sAppPath1    ; //DressyIOS
            public string sAppName1    ; //
            public int    iPosX1       ; //Window Position X
            public int    iPosY1       ; //Window Position Y
            public string sAppPath2    ; //View 16
            public string sAppName2    ; //
            public int    iPosX2       ; //Window Position X
            public int    iPosY2       ; //Window Position Y
            public string sAppPath3    ; //ENP_Inspection
            public string sAppName3    ; //
            public int    iPosX3       ; //Window Position X
            public int    iPosY3       ; //Window Position Y
            public string sIniPath1    ; //Default INI File Path(Dressy)
            public string sIniPath2    ; //Default INI File Path(View16)
            public string sRsltPath    ; //Save Result Folder Path
            public string sCalPath     ; //Cal File Save Folder Path     

            public int iNPSArea1  ;
            public int iNPSLeft1  ;
            public int iNPSSub1   ;
            public int iNPSTop1   ;
           
            public int iNPSArea2  ;
            public int iNPSLeft2  ;
            public int iNPSSub2   ;
            public int iNPSTop2   ;
      
            public int iMTFROILeft1;
            public int iMTFROILen1 ;
            public int iMTFROINum1 ;
            public int iMTFROITop1 ;
         
            public int iMTFEgHght1 ;
            public int iMTFEgLeft1 ;
            public int iMTFEgTop1  ;
            public int iMTFEgWidth1;

            public int iMTFROILeft2;
            public int iMTFROILen2 ;
            public int iMTFROINum2 ;
            public int iMTFROITop2 ;
         
            public int iMTFEgHght2 ;
            public int iMTFEgLeft2 ;
            public int iMTFEgTop2  ;
            public int iMTFEgWidth2;

            public string sProductCode;
            public string sProductVer ;
            public string sFPGAVer    ;
            public string sAcqSW      ;
            public string sEvalSW     ;
            public string sPerform    ;

            
            public bool bIgnrCycleAnalyze; //Cycle Analyze 사용 유무, 평가 사이클에서 Cal 이미지 안쓰는데 Cal 이미지만 받아야 되는 경우가 생겨서 추가함. 진섭
            public bool bIgnrCycleCheck  ; //Cycle Check 사용 유무, Cal만 받는 경우 플래시 메모리에 이미지 저장 못해서 Check 사이클 사용못함. 진섭
                                           //bIgnrCycleAnalyze == true 시 bIgnrCycleCheck도 true 되도록 사용함. 
            public int  iBfXrayDelay     ; //X-Ray 조사 전 딜레이
            public int iSelGetDarkBtn    ; //Cal 공정 시 Get Dark 버튼 클릭 -> Get Bright 버튼 클릭할껀지 Aging 버튼 클릭할껀지 선택(콤보박스)
            //1.0.1.6
            public int iTolerance        ; //Cal 공정에서 편차 계산할때 쓰는 허용 오차 범위
            public int iCalDeleteDelay   ; //Cal 공정에서 편차 확인하고 파일 삭제한다음 딜레이. X-Ray 과열로 추가요청

            //1.0.1.7
            public int iTrgErrProc       ; //트리거 불량일때 에러처리할껀지 다음 자재 작업할껀지 선택하는 옵션
            public int iCalRptErrProc    ; //Calibration 공정 편차 계산 반복 작업 에러일때 에러처리할껀지 다음 자재 작업할껀지 선택하는 옵션
            public int iCalRptCnt        ; //Calibration 공정 편차 계산 반복 작업 횟수. 이 카운트 넘어가면 에러 띄우던지 다음 자재 작업 할것.
        }


        public struct CEzSensorInfo
        {
            //EzSensor
            public int    iEzType         ; //EzSensor/BI 구분한다.
            public bool   bUseEzP         ;
            public int    iImgSize        ; //1x1, 2x2, 1x1 -> 2x2 선택 옵션
            public int    iEzGbCnt        ; //Ez Get Bright 영상 획득 카운트
            public int    iGbDelay        ; //Get Bright, Get Image 딜레이
            public int    iBfMacDelay     ; //매크로 시작 전에 딜레이, USB 연결하고 자꾸 뻑난다고 해서 넣어달라고 요청(오성철과장)
            public int    iAtMacDelay     ; //매크로 끝난 후에 딜레이, USB 연결하고 자꾸 뻑난다고 해서 넣어달라고 요청(오성철과장)
            public int    iProductID      ; //Product ID
            public bool   bInvertacq      ; //Invert acquired pixels
            public int    iTimeout        ; //Time-out <sec> (0-infinite)
            public int    iOntheFly       ; //On-the-fly-offset day<s>
            public bool   bEnableSerial   ; //Enable Serial ID
            public int    iDescramble     ; //Descramble
            public int    iVRest          ; //V-Reset
            public int    iBinning1x1     ; //Binning1x1
            public int    iBinning2x2     ; //Binning2x2
            public int    iCutoffbevel1x1 ; //Cut-off bevel 45''
            public int    iCutoffbevel2x2 ; //Cut-off bevel 45''
            public bool   bBright61       ; //Bright61
            public int    iGain           ; //Gain 
            public int    iMode           ; //Mode
            public int    iCutoffR        ; //Cut-off R
            public int    iPattern        ; //Pattern
            public bool   bDebugdump      ; //Debug dump of received transport-stream
            public int    iNsWidth        ; //Noise Width          
            public int    iNsHeight       ; //Noise Height         
            public string sDarkImgPath1x1 ; //Noise Dark Image Path
            public string sDarkImgPath2x2 ; //Noise Dark Image Path
            public string sAppPath1       ; //EzSensor Exe Folder      
            public string sAppName1       ; //EzSensor Application name
            public string sAppPath2       ; //Noise Exe Folder         
            public string sAppName2       ; //Noise Application name  
            public string sAppPath3       ; //DQE Exe Folder      
            public string sAppName3       ; //DQE Application name
            public int    iPosX1          ; //Window Position X(EzSensor)
            public int    iPosY1          ; //Window Position Y(EzSensor)
            public int    iPosX2          ; //Window Position X(Noise)
            public int    iPosY2          ; //Window Position Y(Noise)
            public int    iPosX3          ; //Window Position X(DQE)
            public int    iPosY3          ; //Window Position Y(DQE)
            public string sIniPath1       ;
            public string sIniPath2       ;
            public string sRsltPath       ;
            public bool   bUseIOSPgm      ; //Ver 2019.10.15.1 신규 디바이스(해상도만 틀림)추가됐는데 ini 파일 따라서 Descramble 옵션 항목 달리 보여줘야해서 추가. 진섭

            //입고공정 ~ Skull 공정까지 옵션
            public int    iEntr1x1Width   ; 
            public int    iEntr1x1Hght    ; 
            public int    iEntr2x2Width   ;
            public int    iEntr2x2Hght    ;
            public int    iAg1x1Width     ; 
            public int    iAg1x1Hght      ; 
            public int    iAg2x2Width     ; 
            public int    iAg2x2Hght      ; 
            public int    iAgRotate       ; 
            public bool   bAgFlipHorz     ; 
            public bool   bAgFlipVert     ; 
            public int    iAgCropTop      ; 
            public int    iAgCropLeft     ; 
            public int    iAgCropRight    ; 
            public int    iAgCropBtm      ; 
            public int    iAcqMaxFrame1x1 ; 
            public int    iAcqInterval1x1 ; 

            public int    iAcqMaxFrame2x2 ; 
            public int    iAcqInterval2x2 ;
                                   
            public int    iDimWidth1x1    ;
            public int    iDimHght1x1     ;
            public double dPixelPitch1x1  ;
            public int    iDimWidth2x2    ;
            public int    iDimHght2x2     ;
            public double dPixelPitch2x2  ;
                                          
            public int    iNPSLeft1x1     ;
            public int    iNPSTop1x1      ;
            public int    iNPSW1x1        ;
            public int    iNPSH1x1        ;
                                          
            public int    iMTFLeft1x1     ;
            public int    iMTFTop1x1      ;
            public int    iMTFW1x1        ;
            public int    iMTFH1x1        ;
                                          
            public int    iNPSLeft2x2     ;
            public int    iNPSTop2x2      ;
            public int    iNPSW2x2        ;
            public int    iNPSH2x2        ;
                                          
            public int    iMTFLeft2x2     ;
            public int    iMTFTop2x2      ;
            public int    iMTFW2x2        ;
            public int    iMTFH2x2        ;  
                                      
            public double dDoze           ;

            public int  iSkRotate         ;
            public bool bSkFlipHorz       ;
            public bool bSkFlipVert       ;
            public int  iSkCropTop        ;
            public int  iSkCropLeft       ;
            public int  iSkCropRight      ;
            public int  iSkCropBtm        ;
        }

        //EzSensor 조사조건
        public struct CEzSensor
        {
            //입고영상 공용
            public double dEzEntrXmA  ;
            public double dEzEntrXKvp ;
            public double dEzEntrXTime;
            public int    iEzEntrFltr ;
            //EzSensor
            public double dEzGbXmA1   ;//특성 Get Bright1
            public double dEzGbXKvp1  ;
            public double dEzGbXTime1 ;
            public int    iEzGbFltr1  ;
            public double dEzGiXmA1   ;//특성 Get Image1
            public double dEzGiXKvp1  ;
            public double dEzGiXTime1 ;
            public int    iEzGiFltr1  ;
            public double dEzGbXmA2   ;//특성 Get Bright2
            public double dEzGbXKvp2  ;
            public double dEzGbXTime2 ;
            public int    iEzGbFltr2  ;
            public double dEzGiXmA2   ;//특성 Get Image2
            public double dEzGiXKvp2  ;
            public double dEzGiXTime2 ;
            public int    iEzGiFltr2  ;
            public double dEzTrXmA    ;//Cal Trigger
            public double dEzTrXKvp   ;
            public double dEzTrXTime  ;
            public int    iEzTrFltr   ;
            public double dEzGbXmA3   ;//Cal Get Bright3 
            public double dEzGbXKvp3  ;
            public double dEzGbXTime3 ;
            public int    iEzGbFltr3  ;
            public double dEzSkXmA    ;//Skull 
            public double dEzSkXKvp   ;
            public double dEzSkXTime  ;
            public int    iEzSkFltr   ;
        }

        public struct CDressy
        {   
            public double dXmA     ;
            public double dXKvp    ;
            public double dXTime   ;

            public string sFileName;
            public string sType    ;
            public int    iFilter  ;
            public int    iBind    ;

            public string sAcq1    ; //3  offset Calibration
            public string sAcq2    ; //4  Gain Calibration
            public string sAcq3    ; //5  Bpm Correction
            public string sAcq4    ; //6  Direct Hit Filtering
            public string sAcq5    ; //7  Gamma Correction
            public string sAcq6    ; //10 Char. Curve
            public string sAcq7    ; //8  Continuous acquisition
            public string sStep    ; //Dressy 스텝 건너뛰어야 할때 여기 저장된 리스트 넘버로 점프.

        }

      


        public struct CDevInfo
        {   //device에 대한 Dev_Info
            public int    iTRAY_PcktCntX   ;
            public double dTRAY_PcktPitchX ;
          
            public int    iLODR_SlotCnt    ;
            public double dLODR_SlotPitch  ;

            

            //X-Ray 옵션
            public int    iUseUSBOptn      ;//0:Left Only, 1:Right Only, 2:Left -> Right 
         

            //매크로 옵션
            public string sCrntDevMacro    ;

            public string sL_Name;
            public string sR_Name;

            public int iL_UsbCnt;
            public int iR_UsbCnt;

            
			
			public int    iMacroType   ; //Macro Type

            public int iCoolingTime; //X-Ray 조사기 쿨링 타임
            
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
        } ;


        public struct CCmnOptn //장비 공용 옵션.
        {
            //public bool   bIgnrDoor           ;
            public int iSetLvNo ;
            public int iCrntLvNo;

            //인덱스 옵션
            public bool bBarcodeSkip;

            //바깥 문만 도어 스킵
            public bool bIgnrDoor;
            //USB커넥트 에러 스킵
            public bool bIgnrCnctErr;
            public bool bIgnrSerialErr;//시리얼넘버 매칭 안되면 에러 or 워크엔드로 바꿔서 작업 종료
            //public int iCoolingTime; //X-Ray 조사기 쿨링 타임

            public int iXrayRptCnt; //X-Ray 조사 후 Trigger 안터졌을때 반복할 횟수, 0이면 사용 안함

            public string sDressyPath; //레포트 남길때 시리얼 넘버 가져오는 경로
            public string sEzSensorPath;

            public bool   bSkipEntr ;
            public bool   bSkipAging;
            public bool   bSkipMTF  ;
            public bool   bSkipCalib;
            public bool   bSkipSkull;

            public int    iBuzzOffTime; //일정 시간 이상 지났을 경우 타워램프 알람을 오프한다.

            //Ver 1.0.3.0 화면 녹화 기능
            public bool   bUseRecord;
            public string sRecordPath;
        } ;

        //Master Option.
        public struct CMstOptn                  //마스타 옵션. 화면상에 있음 FrmMain 디바이스 라벨 더블 클릭.
        {
            public bool   bDebugMode    ; //true = 로그기능강화, 디버깅시 타임아웃무시.
            public bool   bIdleRun      ; //IdleRun 장비 통신 및 비전 테스트.
            public double dTrgOfs       ; //연속 트리거 거리 오프셑
            public int    iMacroCycle   ; //매크로 사이클만 태울때 쓰는 놈

            public int    iXrayVolt     ;
            public double dXrayAmp      ;
            public double dXrayTime     ;

            public bool   bUseSwTrg     ;
            
        };

        //Eqipment Option.
        public struct CEqpOptn                 //모델별 하드웨어 옵션.  화면상에 없고 텍스트 파일에 있음.
        {
        } ;

        public struct CEqpStat
        {
            public string sBarcode;
            public int    iLastWorkStep;
            

            public double dAmp ;
            public int    iVolt;
            public double dTime;



            public int    iLDRSplyCnt   ;
            public double dLastIDXRPos  ;
            public double dLastIDXFPos  ;
            public bool   bMaint        ;
            public double dTrayWorkTime ;
            public double dTrayUPH      ;
            public int    iBrcdRemoveCnt;

            public string sCrntSerial; //센서에 입력되어있는 시리얼 넘버
            public string sSerialList; //시리얼 넘버 파일에 있는 시리얼넘버 중에 매칭 된놈
            //드레시 전용
            public string sVolt;
            public string sTemp ;
            public string sHumid;

            //드레시 이지센서 시리얼넘버 받아오면 같이 딸려오는 파라미터. 제이슨파일 만들때 넣는다.
            public string sWaferNo;
            public string sFosNo  ;
            public string sBdNo   ;
            public string sVReset ;
            public string sDarkOfs;
            //드레시 용
            public double dStartTime;
            public double dEndTime  ;
            //이지센서용 1x1, 2x2 표기를 달리 한다.
            public double dStartTime1x1;
            public double dEndTime1x1  ;
            public double dStartTime2x2;
            public double dEndTime2x2  ;
            //작업중 다음날로 날짜 넘어가면 파일 정보 불러오는 부분에서 에러 떠서 랏오픈 시점 시간으로 설정한다.
            //Now로 설정된 애들 다 이걸로 바꿈. 진섭
            public string sYear  ;
            public string sMonth ;
            public string sDay   ;
        } ;



        public static string m_sCrntDev; //Current open device.
        
        public static CDevInfo DevInfo;
        public static CDevOptn DevOptn;


        public static CCmnOptn CmnOptn;
        public static CMstOptn MstOptn;
        public static CEqpOptn EqpOptn;
        public static CEqpStat EqpStat;

        
        public static CEzSensorInfo   EzSensorInfo; 
        public static CDressyInfo     DressyInfo  ;
        public static CDressy     []  Dressy      = new CDressy     [100];
        public static CEzSensor   []  EzSensor    = new CEzSensor   [ 11];
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

        public static string GetCrntDevMacro()
        {
            if (DevInfo.sCrntDevMacro == "") DevInfo.sCrntDevMacro = "Default";
            return DevInfo.sCrntDevMacro;
        }

        public static void SetCrntDev(string _sName)
        {
            m_sCrntDev = _sName;
        }

        public static void LoadJobFile(string _sDevName) 
        {
            LoadDevInfo(_sDevName);
            LoadDevOptn(_sDevName);
            LoadDressy(_sDevName);
            LoadDressyInfo(_sDevName);
            LoadEzSensorInfo(_sDevName);
            LoadEzSensor(_sDevName);

            //Set Current Device Name.
            SetCrntDev(_sDevName);
        }
        public static void SaveJobFile(string _sDevName)
        {
            SaveDevInfo(_sDevName);
            SaveDevOptn(_sDevName);
            SaveDressy(_sDevName);
            SaveDressyInfo(_sDevName);
            SaveEzSensorInfo(_sDevName);
            SaveEzSensor(_sDevName);

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


        public static void LoadDressy(string _sJobName)
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevInfoPath = sExeFolder + "JobFile\\" + _sJobName + "\\Dressy.ini";
            CIniFile IniLotInfo = new CIniFile(sDevInfoPath);
            for(int i=0; i<100; i++)
            {
                IniLotInfo.Load("dXmA"     , i.ToString(), out Dressy[i].dXmA     );
                IniLotInfo.Load("dXKvp"    , i.ToString(), out Dressy[i].dXKvp    );
                IniLotInfo.Load("dXTime"   , i.ToString(), out Dressy[i].dXTime   );
                                                                                  
                IniLotInfo.Load("sFileName", i.ToString(), out Dressy[i].sFileName);
                IniLotInfo.Load("sType"    , i.ToString(), out Dressy[i].sType    );
                IniLotInfo.Load("iFilter"  , i.ToString(), out Dressy[i].iFilter  );
                IniLotInfo.Load("iBind"    , i.ToString(), out Dressy[i].iBind    );

                IniLotInfo.Load("sAcq1"    , i.ToString(), out Dressy[i].sAcq1    );
                IniLotInfo.Load("sAcq2"    , i.ToString(), out Dressy[i].sAcq2    );
                IniLotInfo.Load("sAcq3"    , i.ToString(), out Dressy[i].sAcq3    );
                IniLotInfo.Load("sAcq4"    , i.ToString(), out Dressy[i].sAcq4    );
                IniLotInfo.Load("sAcq5"    , i.ToString(), out Dressy[i].sAcq5    );
                IniLotInfo.Load("sAcq6"    , i.ToString(), out Dressy[i].sAcq6    );
                IniLotInfo.Load("sAcq7"    , i.ToString(), out Dressy[i].sAcq7    ); 
                IniLotInfo.Load("sStep"    , i.ToString(), out Dressy[i].sStep    );
            }
        }

        public static void SaveDressy(string _sJobName)
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevInfoPath = sExeFolder + "JobFile\\" + _sJobName + "\\Dressy.ini";
            CIniFile IniLotInfo = new CIniFile(sDevInfoPath);
            for(int i=0; i<100; i++)
            {
                IniLotInfo.Save("dXmA"     , i.ToString(), Dressy[i].dXmA     );
                IniLotInfo.Save("dXKvp"    , i.ToString(), Dressy[i].dXKvp    );
                IniLotInfo.Save("dXTime"   , i.ToString(), Dressy[i].dXTime   );
                                                                              
                IniLotInfo.Save("sFileName", i.ToString(), Dressy[i].sFileName);
                IniLotInfo.Save("sType"    , i.ToString(), Dressy[i].sType    );
                IniLotInfo.Save("iFilter"  , i.ToString(), Dressy[i].iFilter  );
                IniLotInfo.Save("iBind"    , i.ToString(), Dressy[i].iBind    );

                IniLotInfo.Save("sAcq1"    , i.ToString(), Dressy[i].sAcq1    );
                IniLotInfo.Save("sAcq2"    , i.ToString(), Dressy[i].sAcq2    );
                IniLotInfo.Save("sAcq3"    , i.ToString(), Dressy[i].sAcq3    );
                IniLotInfo.Save("sAcq4"    , i.ToString(), Dressy[i].sAcq4    );
                IniLotInfo.Save("sAcq5"    , i.ToString(), Dressy[i].sAcq5    );
                IniLotInfo.Save("sAcq6"    , i.ToString(), Dressy[i].sAcq6    );
                IniLotInfo.Save("sAcq7"    , i.ToString(), Dressy[i].sAcq7    );                                                   
                IniLotInfo.Save("sStep"    , i.ToString(), Dressy[i].sStep    );
            }
        }     

        public static void LoadEzSensor(string _sJobName)
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevInfoPath = sExeFolder + "JobFile\\" + _sJobName + "\\EzSensor.ini";
            CIniFile IniLotInfo = new CIniFile(sDevInfoPath);
            for(int i=0; i<10; i++)
            {
                IniLotInfo.Load("dEzEntrXmA"   , i.ToString(), out EzSensor[i].dEzEntrXmA  );
                IniLotInfo.Load("dEzEntrXKvp"  , i.ToString(), out EzSensor[i].dEzEntrXKvp );
                IniLotInfo.Load("dEzEntrXTime" , i.ToString(), out EzSensor[i].dEzEntrXTime);
                IniLotInfo.Load("iEzEntrFltr"  , i.ToString(), out EzSensor[i].iEzEntrFltr );
                                            
                IniLotInfo.Load("dEzGbXmA1"    , i.ToString(), out EzSensor[i].dEzGbXmA1   );
                IniLotInfo.Load("dEzGbXKvp1"   , i.ToString(), out EzSensor[i].dEzGbXKvp1  );
                IniLotInfo.Load("dEzGbXTime1"  , i.ToString(), out EzSensor[i].dEzGbXTime1 );
                IniLotInfo.Load("iEzGbFltr1"   , i.ToString(), out EzSensor[i].iEzGbFltr1  );
                IniLotInfo.Load("dEzGiXmA1"    , i.ToString(), out EzSensor[i].dEzGiXmA1   );
                IniLotInfo.Load("dEzGiXKvp1"   , i.ToString(), out EzSensor[i].dEzGiXKvp1  );
                IniLotInfo.Load("dEzGiXTime1"  , i.ToString(), out EzSensor[i].dEzGiXTime1 );
                IniLotInfo.Load("iEzGiFltr1"   , i.ToString(), out EzSensor[i].iEzGiFltr1  );
                IniLotInfo.Load("dEzGbXmA2"    , i.ToString(), out EzSensor[i].dEzGbXmA2   );
                IniLotInfo.Load("dEzGbXKvp2"   , i.ToString(), out EzSensor[i].dEzGbXKvp2  );
                IniLotInfo.Load("dEzGbXTime2"  , i.ToString(), out EzSensor[i].dEzGbXTime2 );
                IniLotInfo.Load("iEzGbFltr2"   , i.ToString(), out EzSensor[i].iEzGbFltr2  );
                IniLotInfo.Load("dEzGiXmA2"    , i.ToString(), out EzSensor[i].dEzGiXmA2   );
                IniLotInfo.Load("dEzGiXKvp2"   , i.ToString(), out EzSensor[i].dEzGiXKvp2  );
                IniLotInfo.Load("dEzGiXTime2"  , i.ToString(), out EzSensor[i].dEzGiXTime2 );
                IniLotInfo.Load("iEzGiFltr2"   , i.ToString(), out EzSensor[i].iEzGiFltr2  );
                IniLotInfo.Load("dEzTrXmA"     , i.ToString(), out EzSensor[i].dEzTrXmA    );
                IniLotInfo.Load("dEzTrXKvp"    , i.ToString(), out EzSensor[i].dEzTrXKvp   );
                IniLotInfo.Load("dEzTrXTime"   , i.ToString(), out EzSensor[i].dEzTrXTime  );
                IniLotInfo.Load("iEzTrFltr"    , i.ToString(), out EzSensor[i].iEzTrFltr   );
                IniLotInfo.Load("dEzGbXmA3"    , i.ToString(), out EzSensor[i].dEzGbXmA3   );
                IniLotInfo.Load("dEzGbXKvp3"   , i.ToString(), out EzSensor[i].dEzGbXKvp3  );
                IniLotInfo.Load("dEzGbXTime3"  , i.ToString(), out EzSensor[i].dEzGbXTime3 );
                IniLotInfo.Load("iEzGbFltr3"   , i.ToString(), out EzSensor[i].iEzGbFltr3  );
                IniLotInfo.Load("dEzSkXmA"     , i.ToString(), out EzSensor[i].dEzSkXmA    );
                IniLotInfo.Load("dEzSkXKvp"    , i.ToString(), out EzSensor[i].dEzSkXKvp   );
                IniLotInfo.Load("dEzSkXTime"   , i.ToString(), out EzSensor[i].dEzSkXTime  );
                IniLotInfo.Load("iEzSkFltr"    , i.ToString(), out EzSensor[i].iEzSkFltr   );                            
            }
        }
        
        public static void SaveEzSensor(string _sJobName)
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevInfoPath = sExeFolder + "JobFile\\" + _sJobName + "\\EzSensor.ini";
            CIniFile IniLotInfo = new CIniFile(sDevInfoPath);
            for(int i=0; i<10; i++)
            {
                IniLotInfo.Save("dEzEntrXmA"   , i.ToString(), EzSensor[i].dEzEntrXmA  );
                IniLotInfo.Save("dEzEntrXKvp"  , i.ToString(), EzSensor[i].dEzEntrXKvp );
                IniLotInfo.Save("dEzEntrXTime" , i.ToString(), EzSensor[i].dEzEntrXTime);
                IniLotInfo.Save("iEzEntrFltr"  , i.ToString(), EzSensor[i].iEzEntrFltr );
                                            
                IniLotInfo.Save("dEzGbXmA1"    , i.ToString(), EzSensor[i].dEzGbXmA1   );
                IniLotInfo.Save("dEzGbXKvp1"   , i.ToString(), EzSensor[i].dEzGbXKvp1  );
                IniLotInfo.Save("dEzGbXTime1"  , i.ToString(), EzSensor[i].dEzGbXTime1 );
                IniLotInfo.Save("iEzGbFltr1"   , i.ToString(), EzSensor[i].iEzGbFltr1  );
                IniLotInfo.Save("dEzGiXmA1"    , i.ToString(), EzSensor[i].dEzGiXmA1   );
                IniLotInfo.Save("dEzGiXKvp1"   , i.ToString(), EzSensor[i].dEzGiXKvp1  );
                IniLotInfo.Save("dEzGiXTime1"  , i.ToString(), EzSensor[i].dEzGiXTime1 );
                IniLotInfo.Save("iEzGiFltr1"   , i.ToString(), EzSensor[i].iEzGiFltr1  );
                IniLotInfo.Save("dEzGbXmA2"    , i.ToString(), EzSensor[i].dEzGbXmA2   );
                IniLotInfo.Save("dEzGbXKvp2"   , i.ToString(), EzSensor[i].dEzGbXKvp2  );
                IniLotInfo.Save("dEzGbXTime2"  , i.ToString(), EzSensor[i].dEzGbXTime2 );
                IniLotInfo.Save("iEzGbFltr2"   , i.ToString(), EzSensor[i].iEzGbFltr2  );
                IniLotInfo.Save("dEzGiXmA2"    , i.ToString(), EzSensor[i].dEzGiXmA2   );
                IniLotInfo.Save("dEzGiXKvp2"   , i.ToString(), EzSensor[i].dEzGiXKvp2  );
                IniLotInfo.Save("dEzGiXTime2"  , i.ToString(), EzSensor[i].dEzGiXTime2 );
                IniLotInfo.Save("iEzGiFltr2"   , i.ToString(), EzSensor[i].iEzGiFltr2  );
                IniLotInfo.Save("dEzTrXmA"     , i.ToString(), EzSensor[i].dEzTrXmA    );
                IniLotInfo.Save("dEzTrXKvp"    , i.ToString(), EzSensor[i].dEzTrXKvp   );
                IniLotInfo.Save("dEzTrXTime"   , i.ToString(), EzSensor[i].dEzTrXTime  );
                IniLotInfo.Save("iEzTrFltr"    , i.ToString(), EzSensor[i].iEzTrFltr   );
                IniLotInfo.Save("dEzGbXmA3"    , i.ToString(), EzSensor[i].dEzGbXmA3   );
                IniLotInfo.Save("dEzGbXKvp3"   , i.ToString(), EzSensor[i].dEzGbXKvp3  );
                IniLotInfo.Save("dEzGbXTime3"  , i.ToString(), EzSensor[i].dEzGbXTime3 );
                IniLotInfo.Save("iEzGbFltr3"   , i.ToString(), EzSensor[i].iEzGbFltr3  );
                IniLotInfo.Save("dEzSkXmA"     , i.ToString(), EzSensor[i].dEzSkXmA    );
                IniLotInfo.Save("dEzSkXKvp"    , i.ToString(), EzSensor[i].dEzSkXKvp   );
                IniLotInfo.Save("dEzSkXTime"   , i.ToString(), EzSensor[i].dEzSkXTime  );
                IniLotInfo.Save("iEzSkFltr"    , i.ToString(), EzSensor[i].iEzSkFltr   );                            
            }
        } 

        public static void LoadDressyInfo(string _sJobName) 
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevInfoPath = sExeFolder + "JobFile\\" + _sJobName + "\\DressyInfo.ini";
            CAutoIniFile.LoadStruct<CDressyInfo>(sDevInfoPath,"DressyInfo",ref DressyInfo);               
        }

        public static void SaveDressyInfo(string _sJobName)  
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevInfoPath = sExeFolder + "JobFile\\" + _sJobName + "\\DressyInfo.ini";
            CAutoIniFile.SaveStruct<CDressyInfo>(sDevInfoPath,"DressyInfo",ref DressyInfo);
        }

        public static void LoadEzSensorInfo(string _sJobName) 
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevInfoPath = sExeFolder + "JobFile\\" + _sJobName + "\\EzSensorInfo.ini";
            CAutoIniFile.LoadStruct<CEzSensorInfo>(sDevInfoPath,"EzSensor",ref EzSensorInfo);               
        }

        public static void SaveEzSensorInfo(string _sJobName)  
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevInfoPath = sExeFolder + "JobFile\\" + _sJobName + "\\EzSensorInfo.ini";
            CAutoIniFile.SaveStruct<CEzSensorInfo>(sDevInfoPath,"EzSensor",ref EzSensorInfo);
        }

        

        
    };
}
