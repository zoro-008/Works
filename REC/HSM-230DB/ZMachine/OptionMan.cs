using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Runtime.CompilerServices;
using System.Reflection;
using COMMON;
using SML2;

using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

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
            public int    iWLDR_SlotCnt     ;
            public double dWLDR_SlotPitch   ;
            public int    iWFER_DieCntX     ;
            public int    iWFER_DieCntY     ;
            public double dWFER_DiePitchX   ;
            public double dWFER_DiePitchY   ;
                                            
                                            
            public int    iSLDR_SlotCnt     ;
            public double dSLDR_SlotPitch   ;
            public int    iSBOT_PcktCnt     ;
            public double dSBOT_PcktPitch   ;
                                            
            public int    iWaferSize        ; //0: 8인치 , 1:12인치.
                                            
            public double dDieWidth         ;
            public double dDieHeight        ;
                                            
            public double dSubWidth         ;
            public double dSubHeight        ;
                                            
            public double dWFER_Tickness    ;
        } ;                                 
                                            
        public struct CDevOptn               //Device에 따라 바뀌는 옵션.
        {                                   
            //                              
            public int    iPickDelay        ;           
                                            
            public double dDieXOfs          ; //Die Offset
            public double dDieYOfs          ; //Die Offset
                                            
            public double dVisnTolXY        ; //절대값 Stage 검사시에 검사 허용 치수.
            public double dVisnTolAng       ; //절대값 각도.
            public double dRghtEndVisnTolXY ; //우측 엔드 비젼 검사 허용 치수
            public double dLeftEndVisnTolXY ; //좌측 엔드 비젼 검사 허용 치수
            
            //Vision 위치(PATTERN에 있음)
            public double dMstVsnX          ; //Master Vision 위치
            public double dMstVsnY          ; //Master Vision 위치
            public double dSlvVsnX          ; //Slave Vision 위치
            public double dSlvVsnY          ; //Slave Vision 위치
                                            
            //디스펜서                      
            public int    iDspCh            ; //디스펜서 채널
			public int    iDspMinAmount     ; //디스펜서 잔량
			public double dDspVacPres       ; //
            public double dDspPrsPres       ; //
            public double dDspZOfs          ; //Dispensor 작업 Ofset
            public int    iDispShotDelay    ; //Dispensor 더미 샷 딜레이
            public int    iDispAtShotDelay  ; //Dispensor 더미 샷 이후 딜레이
                                            
            //로드셀                        
            public double dShakeOffset      ; //보통 속도로 내려간다. 측정한 서브스트레이트 높이 - 웨이퍼 두께 + dShakeOffset 하여 붙어있지 않은 높이에서 에폭시를 비빈다. 0~음수값으로 설정.
            public double dShakeRange       ; //내려서 비빌때 범위.
            public double dAttachSpeed1     ; //shake 높이까지 가는 속도. 
            public double dAttachForce      ; //합착 무게.g단위.
            public double dAttachForceOfs   ; //합착 무게.g단위. 오프셑.
            public int    iAttachDelay      ; //합착 딜레이.
            public int    iAtAttachDelay    ; //배큠오프 하고 먹는 딜레이.
            
                                            
            public int    iSStgTemp         ; //스테이지 온도

            //public double dBoutClampOfs     ; //서브스트레이트 집어가서 돌아다니지 않게 레일 X축으로 클램핑

            //패턴 탭에 있는 그림 색 변경 옵션
            public int    iSubRgb           ; //서브스트레이트 색
            public int    iDieRgb           ; //다이 색
            public int    iVsnRgb           ; //비젼 타겟 색

            public double dToolCrashDist    ; //디바이스 별로 툴 충돌 거리가 다르다. 

            public double dUVWTOfs          ;

            public int    iEjectAtUpDelay   ;
            public double dEjectSpeed       ;
            public double dPickUpFrstOfs    ;
            public double dPickUpFrstSpeed  ;
            public double dAtPlaceUpSpeed   ;
            public double dAtPlaceUpOfs     ;

            public int    iGetHeatDelay     ;
            public int    iOutHeatDelay     ; 

            public double dSubMinFlat       ; //서브스트레이트 높이 측정 여러군데 하여서 맥스값 - 민값 해서 이수치를 넘으면 에러.
            public double dEndMinFlat       ; //End - Sub - Tickness 측정 하여 이값보다  한포인트라도 높으면 에러.

            public bool   bRvsWafer         ; //웨이퍼를 반대로 넣는경우 리버스 웨이퍼로 설정을 해야지 맵파일을 반대로 로딩한다.

            

        } ;


        public struct CCmnOptn //장비 공용 옵션.
        {
            public bool   bIgnrDoor         ;
                                            
            public bool   bWSTGLoadingStop  ;
            public bool   bSSTGLoadingStop  ;
            public bool   bWfrBarcodeSkip   ;
            public bool   bSubBarcodeSkip   ;
            public bool   bNotUseHeater     ;
            public bool   bUseGuideEjct     ; //픽커 가이드 실린더 사용한 이젝팅 사용 
                          
            public bool   bEpoxyPushTest    ; //에폭시 눌림 확인 검사용.
                          
            public bool   bNoDisp           ; //디스펜싱 출력 아이오 안내보냄.

            public string sMapFileFolder    ; //맵파일이 있는 폴더.
            public string sSampleFileName   ; //바코드가 있는 위치를 찾기 위해 샘플용으로 저장함.
            public string sFileLotIDMask    ; //파일이름에서 Lot아이디를 뽑아오기 위한 마스크.
            public string sSampleWaferBar   ; //웨이퍼 바코드 샘플
            public string sBarLotIDMask     ; //웨이퍼 바코드 에서 랏 아이디를 뽑아오는 마스크. 
            public string sBarWfrNoMask     ; //웨이퍼 바코드 에서 웨이퍼 넘버를 뽑아오는 마스크.
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
            public string sModelName;
        } ;

        public struct CEqpStat
        {
            public int    iSSTGStep ;
            public double dSubstrateHeight ; //서브스트레이트 높이.
            public double dSstgFtX         ;
            public double dSstgLtY         ;
            public double dSstgRtY         ;
            public bool   bNeedCheckPckr   ; // 잡파일 변경시에 세팅됩니다.
            public double dDieVisnPosOfsX  ;
            public double dDieVisnPosOfsY  ;

            public string sWSTGBarcode     ;
            public string sSSTGBarcode     ;
            
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
            //CAutoIniFile.LoadStruct<CCmnOptn>(sCmnOptnPath,"CmnOptn",ref CmnOptn);
            CAutoIniFile.LoadStruct(sCmnOptnPath,"CmnOptn",ref CmnOptn);
        }
        public static void SaveCmnOptn()
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sCmnOptnPath = sExeFolder + "Util\\CmnOptn.ini";
            //CAutoIniFile.SaveStruct<CCmnOptn>(sCmnOptnPath,"CmnOptn",ref CmnOptn);
            CAutoIniFile.SaveStruct(sCmnOptnPath,"CmnOptn",ref CmnOptn);
            
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
            //string sModelName = "PMK-100";
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
            //string sModelName = "PMK-100";
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
