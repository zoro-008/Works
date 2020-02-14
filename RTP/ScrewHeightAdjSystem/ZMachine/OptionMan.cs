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
            public double dBoltPitch     ;//너트런너로 돌려야 할 볼트 나사산 피치
            public int    iChannelNo     ;//0~31번까지 너트러너 채널 넘버
            public bool   bUseAdjTransfer; //볼트체결기의 트랜스퍼 실린더 사용 유무 옵션
            public double dCheckTolerance;//PostCheck 측정 오차범위
            public double dNutWorkOptn1  ;//너트러너 Second Stage까지 완료하고 역방향 1바퀴 -> 정방향 2바퀴 -> 역방향 1바퀴 돌려서 Second Stage, 역/정/역 각각 체결토크, 최대토크 받아온다.
            public double dNutWorkOptn2  ;//너트러너 Second Stage까지 완료하고 역방향 1바퀴 -> 정방향 2바퀴 -> 역방향 1바퀴 돌려서 Second Stage, 역/정/역 각각 체결토크, 최대토크 받아온다.
            public double dNutWorkOptn3  ;//너트러너 Second Stage까지 완료하고 역방향 1바퀴 -> 정방향 2바퀴 -> 역방향 1바퀴 돌려서 Second Stage, 역/정/역 각각 체결토크, 최대토크 받아온다.
            public double dNutLastMotn   ;//너트런너 마지막에 살짝 반대방향으로 돌려서 빼야 자재 안딸려올라가서 넣는다. 

            public int    iBoltFindOptn  ; //볼트머리 홈 찾기 옵션
            public bool   bAddWork       ; //본체결후 추가작업 수행 여부

            public double dWorkOfs       ; //측정 시와 체결 검사시의 높이 단차가 있게 센서가 세팅되어 있어 체결시 더해서 작업 한다.
            public double dBfMaxTq       ; //토크 측정 전 최대 토크값 설정 볼트가 뻑뻑해서 토크 6 이상 올라가서 1차 체결 시 너트런너 사용 가능 최대 토크 사용
            public double dMaxTq         ; //측정 최대 토크값 설정 본체결 후 옵션 체결 시에만 적용하기 
            public double dMinTq         ; //측정 최소 토크값 설정 본체결 후 옵션 체결 시에만 적용하기
            
            public string sImgPath       ; //Operator 모드에서 장비 상태 대신 현재 가동중인 제품 사진 보여줄때 사진 경로
        } ;                                 
                                            
        public struct CCmnOptn //장비 공용 옵션.
        {
            public bool   bLoadStop           ; //로더 구간 작업 대기
            public bool   bIgnrDoor           ; //도어 스킵.
            public double dFindBoltWork1      ; //볼트 홈 찾는 각도1
            public double dFindBoltWork2      ; //볼트 홈 찾는 각도1
            public double dFindBoltWork3      ; //볼트 홈 찾는 각도1



        } ;  
 
        //Master Option.
        public struct CMstOptn                  //마스타 옵션. 화면상에 있음 FrmMain 디바이스 라벨 더블 클릭.
        {
            public bool   bDebugMode    ; //true = 로그기능강화, 디버깅시 타임아웃무시.
            public bool   bIdleRun      ; //IdleRun 장비 통신 및 비전 테스트.

            public double dNutDegree; //너트런너 테스트용
            public int    iCode;
            public int    iData;

            
        };

        //Eqipment Option.
        public struct CEqpOptn                 //모델별 하드웨어 옵션.  화면상에 없고 텍스트 파일에 있음.
        {
            public string sEquipName   ;
            public string sEquipSerial ;
        } ;



        public struct CEqpStat
        {
            public bool bMaint            ;
            public int  iSkin             ;
            public int  iTotalWorkCount   ;//1Lot의 최대 자재 갯수
            public int  iCrntWorkCount    ;//1Lot의 최대 자재 갯수
            public int  iGoodCount        ;
            public int  iHghtNG           ;//높이 불량 카운트
            public int  iHighTqNG         ;//상한 토크 불량 카운트
            public int  iLowTqNG          ;//하한 토크 불량 카운트
            public int  iReworkCount      ;//하루에 Rework 한 횟수

            public double dPreCheckGap    ; //높이조절 전 높이 체크한 갭
            public double dAdjustHeight   ;//조절 높이
            public double dPostCheckGap   ;//높이조절 후 높이 체크한 갭

            public double dSStgFastenTorq ; //너트러너 본체결 체결토크
            public double dSStgMaxTorq    ; //너트러너 본체결 최대토크
            public double dOptnFastenTorq1; //OM.DevInfo.bAdjustOption 켜져있을때 첫번째 역방향 1회전 체결토크값
            public double dOptnMaxTorq1   ; //OM.DevInfo.bAdjustOption 켜져있을때 첫번째 역방향 1회전 최대토크값
            public double dOptnFastenTorq2; //OM.DevInfo.bAdjustOption 켜져있을때 정방향 2회전 체결토크값
            public double dOptnMaxTorq2   ; //OM.DevInfo.bAdjustOption 켜져있을때 정방향 2회전 최대토크값
            public double dOptnFastenTorq3; //OM.DevInfo.bAdjustOption 켜져있을때 두번째 역방향 1회전 체결토크값
            public double dOptnMaxTorq3   ; //OM.DevInfo.bAdjustOption 켜져있을때 두번째 역방향 1회전 최대토크값

            public double dSttCycleTime   ;//택타임 잴때 쓰려고 만든다.
            public double dEndCycleTime   ;//택타임 잴때 쓰려고 만든다.
            public double dCycleTime   ;//택타임 잴때 쓰려고 만든다.
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

            //고정할라고 여기 씀요
            //OM.DevInfo.iRowCount = 25; //화면땜에 고정시킴
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
