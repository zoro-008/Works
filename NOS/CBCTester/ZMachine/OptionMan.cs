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
        public struct CDevInfo//잡파일 개념 없는것으로....
        {   
            //레일 무빙시에 스토퍼들간의 딜레이.
            public int    iShakeCnt           ; //쉐이크 카운트.
            public int    iBloodChamberSpd    ; //챔버에 넣는 피공급 스피드. 수치가 아니고 인덱스임 1~40번까지 있고 해당 스피드 는 헤밀톤 클래스 참조.
            public int    iDeadVolSpd         ; //관로에 있는 무의미한 CP들을 밀어낼때 스피드.

            public int    iBloodReadyCnt      ; //블러드 실린지 준비동작 몇번 빨아 땡길건지..
            public int    iDCReadyCnt         ; //DC  실린지 준비동작 몇번 빨아 땡길건지..
            public int    iFCMReadyCnt        ; //FCM 실린지 준비동작 몇번 빨아 땡길건지..
            public int    iNRReadyCnt         ; //NR  실린지 준비동작 몇번 빨아 땡길건지..
            public int    iRETReadyCnt        ; //RET 실린지 준비동작 몇번 빨아 땡길건지..
            public int    i4DLReadyCnt        ; //4DL 실린지 준비동작 몇번 빨아 땡길건지..

            public int    iCmbEmptyTime       ; // 챔버 비울때 시간.
            
            public int    iFcmWastToWastTime  ; //FCM웨이스트 에서 웨이스트로 보내는 시간.

            public int    iWasteToExtTime     ; //웨이스트에서 Ext웨이스트로 보내는 시간.
            public int    iHgbToWastTime      ; //HGB에서 웨이스트 보내는 시간.
            public int    iBloodPreCutVol     ; //니들에 혈액뽑고 앞에 혹시 있는 에어볼륨 제거.

            public int    iFCMTestStartDelay  ; //FCM통신시에 액체 주입시작후 스타트 시그널까지의 딜레이.
            public int    iFCMTestEndDelay    ; //FCM통신시에 스타트 시그널 후 엔드 시그널까지의 지연시간.

            //public int iDccInsp; // FCM 과 같이 검사하면 안되는 경우엔 이 딜레이를 이용한다.
            public int    iFCMTestDelayTime   ; //DC와 같이 검사 하면 안되어서 설정 하는 딜레이.




            //챔버1 용량.========================================================================
            public int    iCmb1BloodVol       ; //챔버에 넣는 피의 양.

            public int    iCmb1Cp2Time        ;
          //public int    iCmb1TankTime       ;
          //public int    iCmb1SylnPos        ;
          //public int    iCmb1SylSpdCode     ;
            public int    iCmb1Cp3Time        ;

            public int    iCmb1DCSylPos       ; //DC에 들어가는 CP2 실린지.
            public int    iCmb1DCSylSpdCode   ;

            public int    iCmb1CleanRvsPos    ; //클린시에 챔버에게 실린지가 쏘는 포지션.
            public int    iCmb1DeadVol        ; //실린지 챔버 검사기 분기점에서 부터 검사기 까지앞단까지 보내는 볼륨.
            public int    iCmb1DeadTimes      ; //실린지 챔버 검사기 분기점에서 부터 검사기 까지앞단까지 보내는 횟수.

            public int    iDccCleanTime       ; // DC 컨트롤러쪽 안쪽 씻을때 시간.
            public int    iDccInspDelayTime   ; // FCM 과 같이 검사하면 안되는 경우엔 이 딜레이를 이용한다.

            public int    iCmb1ToInter        ; // 챔버에서 3거리 까지의 용량.

            //챔버2 용량.========================================================================
            public int    iCmb2BloodVol       ; //챔버에 넣는 피의 양.

            public int    iCmb2Cp2Time        ;
            public int    iCmb2TankTime       ;
          //public int    iCmb2SylnPos        ;
          //public int    iCmb2SylSpdCode     ;
            public int    iCmb2Cp3Time        ;

          //public int    iCmb2DCSylPos       ; //DC에 들어가는 CP2 실린지.
          //public int    iCmb2DCSylSpdCode   ;

            public int    iCmb2CleanRvsPos    ; //클린시에 챔버에게 실린지가 쏘는 포지션.
          //public int    iCmb2DeadVol        ; //실린지 챔버 검사기 분기점에서 부터 검사기 까지앞단까지 보내는 볼륨.
            public int    iCmb2BubbleTime     ; //거품가라앉는 시간 옵션.
            public int    iCmb2Blk            ; //스펙트로미터 인자값.
            public double dCmb2SpecAngle      ; //스펙트로미터 앵글.

            //public int    iCmb2ToInter        ; // 챔버에서 3거리 까지의 용량.
            
            //챔버3 용량.=========================================================================
            public int    iCmb3BloodVol       ; //챔버에 넣는 피의 양.

          //public int    iCmb3Cp2Time        ;
            public int    iCmb3TankTime       ;
            public int    iCmb3SylnPos        ;
            public int    iCmb3SylSpdCode     ;
            public int    iCmb3Cp3Time        ;

            public int    iCmb3FCMSylPos      ; //FCM에 들어가는 실린지.
            public int    iCmb3FCMSylSpdCode  ;

            public int    iCmb3CleanRvsPos    ; //클린시에 챔버에게 실린지가 쏘는 포지션.
            public int    iCmb3DeadVol        ; //실린지 챔버 검사기 분기점에서 부터 검사기 까지앞단까지 보내는 볼륨.
            public int    iCmb3DeadTimes      ; //실린지 챔버 검사기 분기점에서 부터 검사기 까지앞단까지 보내는 횟수.

            public int    iCmb3ToInter        ; // 챔버에서 3거리 까지의 용량.

            //챔버4 용량.===========================================================================
            public int    iCmb4BloodVol       ; //챔버에 넣는 피의 양.

          //public int    iCmb4Cp2Time        ;
            public int    iCmb4TankTime       ;
            public int    iCmb4SylnPos        ;
            public int    iCmb4SylSpdCode     ;
            public int    iCmb4Cp3Time        ;

            public int    iCmb4FCMSylPos      ; //FCM에 들어가는 실린지.
            public int    iCmb4FCMSylSpdCode  ;

            public int    iCmb4CleanRvsPos    ; //클린시에 챔버에게 실린지가 쏘는 포지션.
            public int    iCmb4DeadVol        ; //실린지 챔버 검사기 분기점에서 부터 검사기 까지앞단까지 보내는 볼륨.
            public int    iCmb4DeadTimes      ; //실린지 챔버 검사기 분기점에서 부터 검사기 까지앞단까지 보내는 횟수.

            public int    iCmb4ToInter        ; // 챔버에서 3거리 까지의 용량.

            //챔버5 용량.===============================================================================
            public int    iCmb5BloodVol       ; //챔버에 넣는 피의 양.

          //public int    iCmb5Cp2Time        ;
            public int    iCmb5TankTime       ;
            public int    iCmb5SylnPos        ;
            public int    iCmb5SylSpdCode     ;
            public int    iCmb5Cp3Time        ;

            public int    iCmb5FCMSylPos      ; //FCM에 들어가는 실린지.
            public int    iCmb5FCMSylSpdCode  ;

            public int    iCmb5CleanRvsPos    ; //클린시에 챔버에게 실린지가 쏘는 포지션.
            public int    iCmb5DeadVol        ; //실린지 챔버 검사기 분기점에서 부터 검사기 까지앞단까지 보내는 볼륨.
            public int    iCmb5DeadTimes      ; //실린지 챔버 검사기 분기점에서 부터 검사기 까지앞단까지 보내는 횟수.

            public int    iCmb5ToInter        ; // 챔버에서 3거리 까지의 용량.

            //챔버6 용량.===============================================================================
            public int    iCmb6BloodVol       ; //챔버에 넣는 피의 양.

          //public int    iCmb6Cp2Time        ;
            public int    iCmb6TankTime       ;
            public int    iCmb6SylnPos        ;
            public int    iCmb6SylSpdCode     ;
            public int    iCmb6Cp3Time        ;

            public int    iCmb6FCMSylPos      ; //FCM에 들어가는 실린지.
            public int    iCmb6FCMSylSpdCode  ;

            public int    iCmb6CleanRvsPos    ; //클린시에 챔버에게 실린지가 쏘는 포지션.
            public int    iCmb6DeadVol        ; //실린지 챔버 검사기 분기점에서 부터 검사기 까지앞단까지 보내는 볼륨.
            public int    iCmb6DeadTimes      ; //실린지 챔버 검사기 분기점에서 부터 검사기 까지앞단까지 보내는 횟수.

            public int    iCmb6ToInter        ; // 챔버에서 3거리 까지의 용량.
        } ;                                 
                                            
        public struct CCmnOptn //장비 공용 옵션.
        {
            public bool   bLoadStop           ; //로더 구간 작업 대기
            public bool   bIgnrDoor           ; //도어 스킵.

            public bool   bAutoQc             ; //오토큐씨 적용여부.
            public int    iQcStartHour        ; //오토큐씨 시작시간.
            public int    iQcStartMin         ; //오토큐씨 시작분.
            public int    iUnFreezeMin        ; //해동시간 분.

            public bool   bIgnrBarcode        ; //바코드 무시
            public bool   bIgnrFCMTester      ; //테스터 통신 피드백 안받음.
            public int    iVtlFCMTime         ; //테스터 통신 피드백 안받음 모드에서 가상 FCM 검사 타임.
            public int    iVtlDCTime          ; //테스터 통신 피드백 안받음 모드에서 가상 DC검사 타임.

            public bool   bNotUseFB           ;
            public bool   bNotUse4DLS         ;
            public bool   bNotUseRet          ;
            public bool   bNotUseNr           ;
            
            public bool   bNotUseClean        ;//관로 닦기 안하기..

            public bool   bNotUseSpec         ; //스펙트로 미터 사용안함.

            public bool   bNotUseInsp         ; //인스펙션 안함. KTC검수 옵션. 이였다가 최종검수 옵션으로 바꿈.정부과재 검수용이 됌.


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
            public string sEquipName   ;
            public string sEquipSerial ;
        } ;



        public struct CEqpStat
        {
            //public bool bAutoQCMode ;
            public string sBloodID   ;//혈액넘버.
            public string sSpectro ;
            public int    iSkin ;
            public int    iInspProgress ; //프로그래스 바용.
            public bool   bWorkOneStop ; //1자제만 작업하고 스탑 예약.
        } ;


        public static string m_sCrntDev; //Current open device.
        
        public static CDevInfo DevInfo;
        public static CCmnOptn CmnOptn;
        public static CMstOptn MstOptn;
        public static CEqpOptn EqpOptn;
        public static CEqpStat EqpStat;

        public static ArrayPos StripPos = new ArrayPos();


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
