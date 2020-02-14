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
using SMDll2;

using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

//using System.Runtime.InteropServices;
//using SMDll2.CXmlBase;
//using SMD2Define;
//using SMDll2App;

namespace Machine
{
              
    //---------------------------------------------------------------------------
    public static class OM
    {
        //enum EN_WORK_MODE { wmNormal = 0, wmHeight = 1 }; //0:정상 작업 1:로더에서 꺼내어 로테이터에서 높이 측정만 하고 다시 넣는다.
        public struct CDevInfo
        {   //device에 대한 Dev_Info
            //Rear Housing Stage, 장비 정면 기준 뒤에 있는 스테이지
            public int    iRearColCnt   ;
            public int    iRearRowCnt   ;
            public double dRearColPitch ;
            public double dRearRowPitch ;

            //Front Housing Stage , 장비 정면 기준 앞에 있는 스테이지
            public int    iFrntColCnt   ;
            public int    iFrntRowCnt   ;
            public double dFrntColPitch ;
            public double dFrntRowPitch ;

            //Lens Stage
            public int    iLensColCnt  ;
            public int    iLensRowCnt  ;
            public double dLensColPitch;
            public double dLensRowPitch;
        } ;

        public struct CDevOptn             //Device에 따라 바뀌는 옵션.
        {
            public int    iPCKGapCnt      ; //Picker1,2 사이의 Unit 개수
            //public bool   bUseMultiHldr   ; //홀더 작업 할때 픽커를 동시에쓰냐 개별로쓰냐.
            public double dHldrPitch      ; //홀더 나사산 피치
            public double dThetaWorkSpeed ; //T축 모터 런스피드
            public double dThetaWorkAcc   ; //T축 가속도
            public double dThetaBackPos   ;
            public int    iPlceDelay      ;
            public int    iWrkRptCnt      ; //토크 페일 1포인트 리핏 사용 시 재작업 할 횟수

            //비젼 옵션
            public double dLensVisnXTol   ;
            public double dLensVisnYTol   ;
            public double dHldrVisnXTol   ;
            public double dHldrVisnYTol   ;
            public double dAtVisnTTol     ;

            //토크 옵션
            public double dTorqueMax      ;
            public double dTorqueLimit    ;
            public double dTorqueTime     ;

            public int    iWorkMode       ;//2호기는 포지션으로 체결해야해서 Z축 센서 or 포지션 체결 옵션 처리



        } ;


        public struct CCmnOptn //장비 공용 옵션.
        {
            public int  iVisnBfDelay ;
            //public bool bUnlock      ;
            public bool   bSkipFrnt    ;
            public bool   bSkipRear    ;
            public bool   bUseMultiHldr; //홀더 작업 할때 픽커를 동시에쓰냐 개별로쓰냐.
            public bool   bIgnrLeftPck ;
            public bool   bIgnrRightPck;
            public bool   bTorqChck    ; //토크 체크 모드. ON : 토크에러시 역회전 안하고 Z축만 올림. OFF:토크에러시 역회전하여 끌러서 렌즈트레이에 담음.
            public double dSetMotrTorq1; //모터 자체 토크 셋팅값
            public double dSetMotrTorq2; //모터 자체 토크 셋팅값
            public double dGaugeTorq1  ; //토크게이지 측정값
            public double dGaugeTorq2  ; //토크게이지 측정값
            public bool   b1PntRpt     ; //토크페일 발생 시 페일 발생한 자재 계속 작업

            public int    iPickVisnRetryCnt  ; //픽커 찝는 비전 재시도 횟수.
            public int    iPickRetryCnt      ; //픽커 비전찍고 찝는 재시도 횟수.
            public int    iPlaceVisnRetryCnt ; //픽커 홀더 비전 재시도 횟수.
            public int    iPlaceRetryCnt     ; //픽커 홀더 플레이스 재시도 횟수.


            //Ver1.0.1.0
            //렌즈 체결 후 비젼 인스펙션 사용 옵션 처리
            public bool bUseAtInsp;
        };

        //Master Option.
        public struct CMstOptn                  //마스타 옵션. 화면상에 있음 FrmMain 디바이스 라벨 더블 클릭.
        {
            public bool   bDebugMode; //true = 로그기능강화, 디버깅시 타임아웃무시.
            public double dTrgOfs;

            public int    iMaxTorqueMax  ;
            public int    iMaxTorqueLimit;
            public int    iMaxTorqueTime ;
            public bool   bUnlock        ;
            public bool   bUseEccntrCorr ; //Picker 편심 보정 기능 사용 유무

            //public int iColRowDir;
        };

        //Eqipment Option.
        public struct CEqpOptn                 //모델별 하드웨어 옵션.  화면상에 없고 텍스트 파일에 있음.
        {
            public string sModelName;
        } ;

        //Picker Theta LookUpTable
        public const int MAX_TABLE = 18;
        public struct TLookUpTable
        {
            public double dX;
            public double dY;
            public double dT;
        }

        public static string m_sCrntDev; //Current open device.
        

        
    
        public static CDevInfo DevInfo;
        public static CDevOptn DevOptn;
        public static CCmnOptn CmnOptn;
        public static CMstOptn MstOptn;
        public static CEqpOptn EqpOptn;

        public static TLookUpTable[] LeftTable ;
        public static TLookUpTable[] RightTable;

        public static void Init()
        {
            LeftTable  = new TLookUpTable[MAX_TABLE];
            RightTable = new TLookUpTable[MAX_TABLE];
            //Load
            LoadLastInfo();
            LoadMstOptn();
            LoadEqpOptn();
            LoadCmnOptn();
            LoadJobFile(m_sCrntDev);
        }
        public static void Close()
        {
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

            CIniFile IniDevInfo = new CIniFile(sDevInfoPath);

            IniDevInfo.Load("DevInfo", "iRearColCnt   ", out DevInfo.iRearColCnt  );
            IniDevInfo.Load("DevInfo", "iRearRowCnt   ", out DevInfo.iRearRowCnt  );
            IniDevInfo.Load("DevInfo", "dRearColPitch ", out DevInfo.dRearColPitch);
            IniDevInfo.Load("DevInfo", "dRearRowPitch ", out DevInfo.dRearRowPitch);
                                                    
            IniDevInfo.Load("DevInfo", "iFrntColCnt   ", out DevInfo.iFrntColCnt  );
            IniDevInfo.Load("DevInfo", "iFrntRowCnt   ", out DevInfo.iFrntRowCnt  );
            IniDevInfo.Load("DevInfo", "dFrntColPitch ", out DevInfo.dFrntColPitch);
            IniDevInfo.Load("DevInfo", "dFrntRowPitch ", out DevInfo.dFrntRowPitch);

            IniDevInfo.Load("DevInfo", "iLensColCnt  ", out DevInfo.iLensColCnt  );
            IniDevInfo.Load("DevInfo", "iLensRowCnt  ", out DevInfo.iLensRowCnt  );
            IniDevInfo.Load("DevInfo", "dLensColPitch", out DevInfo.dLensColPitch);
            IniDevInfo.Load("DevInfo", "dLensRowPitch", out DevInfo.dLensRowPitch);
        }

        public static void SaveDevInfo(string _sJobName)
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevInfoPath = sExeFolder + "JobFile\\" + _sJobName + "\\DevInfo.ini";

            //File.Delete(sDevInfoPath);
            CIniFile IniDevInfo = new CIniFile(sDevInfoPath);

            IniDevInfo.Save("DevInfo", "iRearColCnt   ", DevInfo.iRearColCnt  );
            IniDevInfo.Save("DevInfo", "iRearRowCnt   ", DevInfo.iRearRowCnt  );
            IniDevInfo.Save("DevInfo", "dRearColPitch ", DevInfo.dRearColPitch);
            IniDevInfo.Save("DevInfo", "dRearRowPitch ", DevInfo.dRearRowPitch);
                                                     
            IniDevInfo.Save("DevInfo", "iFrntColCnt   ", DevInfo.iFrntColCnt  );
            IniDevInfo.Save("DevInfo", "iFrntRowCnt   ", DevInfo.iFrntRowCnt  );
            IniDevInfo.Save("DevInfo", "dFrntColPitch ", DevInfo.dFrntColPitch);
            IniDevInfo.Save("DevInfo", "dFrntRowPitch ", DevInfo.dFrntRowPitch);

            IniDevInfo.Save("DevInfo", "iLensColCnt  ", DevInfo.iLensColCnt  );
            IniDevInfo.Save("DevInfo", "iLensRowCnt  ", DevInfo.iLensRowCnt  );
            IniDevInfo.Save("DevInfo", "dLensColPitch", DevInfo.dLensColPitch);
            IniDevInfo.Save("DevInfo", "dLensRowPitch", DevInfo.dLensRowPitch);
        }

        public static void LoadDevOptn(string _sJobName) 
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevOptnPath = sExeFolder + "JobFile\\" + _sJobName + "\\DevOptn.ini";

            CIniFile IniDevOptn = new CIniFile(sDevOptnPath);

            IniDevOptn.Load("DevOptn", "iPCKGapCnt     ", out DevOptn.iPCKGapCnt     );
            //IniDevOptn.Load("DevOptn", "bUseMultiHldr  ", out DevOptn.bUseMultiHldr  );
            IniDevOptn.Load("DevOptn", "dHldrPitch     ", out DevOptn.dHldrPitch     );
            IniDevOptn.Load("DevOptn", "dThetaWorkSpeed", out DevOptn.dThetaWorkSpeed);
            IniDevOptn.Load("DevOptn", "dThetaWorkAcc  ", out DevOptn.dThetaWorkAcc  );
            IniDevOptn.Load("DevOptn", "dThetaBackPos  ", out DevOptn.dThetaBackPos  );
            IniDevOptn.Load("DevOptn", "iPlceDelay     ", out DevOptn.iPlceDelay     );
            IniDevOptn.Load("DevOptn", "dLensVisnXTol  ", out DevOptn.dLensVisnXTol  );
            IniDevOptn.Load("DevOptn", "dLensVisnYTol  ", out DevOptn.dLensVisnYTol  );
            IniDevOptn.Load("DevOptn", "dHldrVisnXTol  ", out DevOptn.dHldrVisnXTol  );
            IniDevOptn.Load("DevOptn", "dHldrVisnYTol  ", out DevOptn.dHldrVisnYTol  );
            IniDevOptn.Load("DevOptn", "dTorqueMax     ", out DevOptn.dTorqueMax     );
            IniDevOptn.Load("DevOptn", "dTorqueLimit   ", out DevOptn.dTorqueLimit   );
            IniDevOptn.Load("DevOptn", "dTorqueTime    ", out DevOptn.dTorqueTime    );
            IniDevOptn.Load("DevOptn", "iWrkRptCnt     ", out DevOptn.iWrkRptCnt     );
            //Ver1.0.1.0
            IniDevOptn.Load("DevOptn", "dAtVisnTTol    ", out DevOptn.dAtVisnTTol    );
            IniDevOptn.Load("CmnOptn", "iWorkMode      ", out DevOptn.iWorkMode      );
        }

        public static void SaveDevOptn(string _sJobName) 
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevOptnPath = sExeFolder + "JobFile\\" + _sJobName + "\\DevOptn.ini";

            CIniFile IniDevOptn = new CIniFile(sDevOptnPath);
            IniDevOptn.Save("DevOptn", "iPCKGapCnt     ", DevOptn.iPCKGapCnt      );
            //IniDevOptn.Save("DevOptn", "bUseMultiHldr  ", DevOptn.bUseMultiHldr   );
            IniDevOptn.Save("DevOptn", "dHldrPitch     ", DevOptn.dHldrPitch      );
            IniDevOptn.Save("DevOptn", "dThetaWorkSpeed", DevOptn.dThetaWorkSpeed );
            IniDevOptn.Save("DevOptn", "dThetaWorkAcc  ", DevOptn.dThetaWorkAcc   );
            IniDevOptn.Save("DevOptn", "dThetaBackPos  ", DevOptn.dThetaBackPos   );
            IniDevOptn.Save("DevOptn", "iPlceDelay     ", DevOptn.iPlceDelay      );
            IniDevOptn.Save("DevOptn", "dLensVisnXTol  ", DevOptn.dLensVisnXTol   );
            IniDevOptn.Save("DevOptn", "dLensVisnYTol  ", DevOptn.dLensVisnYTol   );
            IniDevOptn.Save("DevOptn", "dHldrVisnXTol  ", DevOptn.dHldrVisnXTol   );
            IniDevOptn.Save("DevOptn", "dHldrVisnYTol  ", DevOptn.dHldrVisnYTol   );
            IniDevOptn.Save("DevOptn", "dTorqueMax     ", DevOptn.dTorqueMax      );
            IniDevOptn.Save("DevOptn", "dTorqueLimit   ", DevOptn.dTorqueLimit    );
            IniDevOptn.Save("DevOptn", "dTorqueTime    ", DevOptn.dTorqueTime     );
            IniDevOptn.Save("DevOptn", "iWrkRptCnt     ", DevOptn.iWrkRptCnt      );
            //Ver1.0.1.0
            IniDevOptn.Save("DevOptn", "dAtVisnTTol    ", DevOptn.dAtVisnTTol     );
            IniDevOptn.Save("CmnOptn", "iWorkMode      ", DevOptn.iWorkMode       );


        }

        public static void LoadCmnOptn() 
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sCmnOptnPath = sExeFolder + "Util\\CmnOptn.ini";

            CIniFile IniCmnOptn = new CIniFile(sCmnOptnPath);

            IniCmnOptn.Load("CmnOptn", "iVisnBfDelay"    , out CmnOptn.iVisnBfDelay    );
            IniCmnOptn.Load("CmnOptn", "bSkipFrnt"       , out CmnOptn.bSkipFrnt       );
            IniCmnOptn.Load("CmnOptn", "bSkipRear"       , out CmnOptn.bSkipRear       );
            IniCmnOptn.Load("CmnOptn", "bUseMultiHldr"   , out CmnOptn.bUseMultiHldr   );
            IniCmnOptn.Load("CmnOptn", "bIgnrLeftPck"    , out CmnOptn.bIgnrLeftPck    );
            IniCmnOptn.Load("CmnOptn", "bIgnrRightPck"   , out CmnOptn.bIgnrRightPck   );
            IniCmnOptn.Load("CmnOptn", "dSetMotrTorq1"   , out CmnOptn.dSetMotrTorq1   );
            IniCmnOptn.Load("CmnOptn", "dSetMotrTorq2"   , out CmnOptn.dSetMotrTorq2   );
            IniCmnOptn.Load("CmnOptn", "dGaugeTorq1"     , out CmnOptn.dGaugeTorq1     );
            IniCmnOptn.Load("CmnOptn", "dGaugeTorq2"     , out CmnOptn.dGaugeTorq2     );
            IniCmnOptn.Load("CmnOptn", "bTorqChck"       , out CmnOptn.bTorqChck       );
            IniCmnOptn.Load("CmnOptn", "bUseAtInsp"      , out CmnOptn.bUseAtInsp      );
            
            IniCmnOptn.Load("CmnOptn", "iPickVisnRetryCnt" , out CmnOptn.iPickVisnRetryCnt );
            IniCmnOptn.Load("CmnOptn", "iPickRetryCnt"     , out CmnOptn.iPickRetryCnt     );
            IniCmnOptn.Load("CmnOptn", "iPlaceVisnRetryCnt", out CmnOptn.iPlaceVisnRetryCnt);
            IniCmnOptn.Load("CmnOptn", "iPlaceRetryCnt"    , out CmnOptn.iPlaceRetryCnt    );



        }
        public static void SaveCmnOptn()
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sCmnOptnPath = sExeFolder + "Util\\CmnOptn.ini";

            CIniFile IniCmnOptn = new CIniFile(sCmnOptnPath);

            IniCmnOptn.Save("CmnOptn", "iVisnBfDelay"      , CmnOptn.iVisnBfDelay      );
            IniCmnOptn.Save("CmnOptn", "bSkipFrnt"         , CmnOptn.bSkipFrnt         );
            IniCmnOptn.Save("CmnOptn", "bSkipRear"         , CmnOptn.bSkipRear         );
            IniCmnOptn.Save("CmnOptn", "bUseMultiHldr"     , CmnOptn.bUseMultiHldr     );
            IniCmnOptn.Save("CmnOptn", "bIgnrLeftPck"      , CmnOptn.bIgnrLeftPck      );
            IniCmnOptn.Save("CmnOptn", "bIgnrRightPck"     , CmnOptn.bIgnrRightPck     );
            IniCmnOptn.Save("CmnOptn", "dSetMotrTorq1"     , CmnOptn.dSetMotrTorq1     );
            IniCmnOptn.Save("CmnOptn", "dSetMotrTorq2"     , CmnOptn.dSetMotrTorq2     );
            IniCmnOptn.Save("CmnOptn", "dGaugeTorq1"       , CmnOptn.dGaugeTorq1       );
            IniCmnOptn.Save("CmnOptn", "dGaugeTorq2"       , CmnOptn.dGaugeTorq2       );
            IniCmnOptn.Save("CmnOptn", "bTorqChck"         , CmnOptn.bTorqChck         );
            IniCmnOptn.Save("CmnOptn", "bUseAtInsp"        , CmnOptn.bUseAtInsp        );

            IniCmnOptn.Save("CmnOptn", "iPickVisnRetryCnt" , CmnOptn.iPickVisnRetryCnt );
            IniCmnOptn.Save("CmnOptn", "iPickRetryCnt"     , CmnOptn.iPickRetryCnt     );
            IniCmnOptn.Save("CmnOptn", "iPlaceVisnRetryCnt", CmnOptn.iPlaceVisnRetryCnt);
            IniCmnOptn.Save("CmnOptn", "iPlaceRetryCnt"    , CmnOptn.iPlaceRetryCnt    );
            


        }

        public static void LoadMstOptn() 
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sMstOptnPath = sExeFolder + "Util\\MstOptn.ini";

            CIniFile IniMstOptn = new CIniFile(sMstOptnPath);

            IniMstOptn.Load("MstOptn", "bDebugMode     ", out MstOptn.bDebugMode     );
            IniMstOptn.Load("MstOptn", "dTrgOfs        ", out MstOptn.dTrgOfs        );
            IniMstOptn.Load("MstOptn", "iMaxTorqueMax  ", out MstOptn.iMaxTorqueMax  );
            IniMstOptn.Load("MstOptn", "iMaxTorqueLimit", out MstOptn.iMaxTorqueLimit);
            IniMstOptn.Load("MstOptn", "iMaxTorqueTime ", out MstOptn.iMaxTorqueTime );
            IniMstOptn.Load("MstOptn", "bUnlock"        , out MstOptn.bUnlock        );
            IniMstOptn.Load("MstOptn", "bUseEccntrCorr" , out MstOptn.bUseEccntrCorr );

            for (int i = 0; i < MAX_TABLE; i++)
            {
                IniMstOptn.Load("MstOptn", "LeftTableX" + i.ToString(), out LeftTable[i].dX);
                IniMstOptn.Load("MstOptn", "LeftTableY" + i.ToString(), out LeftTable[i].dY);
                IniMstOptn.Load("MstOptn", "LeftTableT" + i.ToString(), out LeftTable[i].dT);

                IniMstOptn.Load("MstOptn", "RightTableX" + i.ToString(), out RightTable[i].dX);
                IniMstOptn.Load("MstOptn", "RightTableY" + i.ToString(), out RightTable[i].dY);
                IniMstOptn.Load("MstOptn", "RightTableT" + i.ToString(), out RightTable[i].dT);
            }



            //IniMstOptn.Load("MstOptn", "iColRowDir     "  , out MstOptn.iColRowDir      );

        }

        public static void SaveMstOptn()
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sMstOptnPath = sExeFolder + "Util\\MstOptn.ini";

            CIniFile IniMstOptn = new CIniFile(sMstOptnPath);

            IniMstOptn.Save("MstOptn", "bDebugMode     "  , MstOptn.bDebugMode     );
            IniMstOptn.Save("MstOptn", "dTrgOfs        "  , MstOptn.dTrgOfs        );
            IniMstOptn.Save("MstOptn", "iMaxTorqueMax  "  , MstOptn.iMaxTorqueMax  );
            IniMstOptn.Save("MstOptn", "iMaxTorqueLimit"  , MstOptn.iMaxTorqueLimit);
            IniMstOptn.Save("MstOptn", "iMaxTorqueTime "  , MstOptn.iMaxTorqueTime );
            IniMstOptn.Save("MstOptn", "bUnlock"          , MstOptn.bUnlock        );
            IniMstOptn.Save("MstOptn", "bUseEccntrCorr"   , MstOptn.bUseEccntrCorr );
            //IniMstOptn.Save("MstOptn", "iColRowDir    "   , MstOptn.iColRowDir       );

            for (int i = 0; i < MAX_TABLE; i++)
            {
                IniMstOptn.Save("MstOptn", "LeftTableX" + i.ToString(), LeftTable[i].dX);
                IniMstOptn.Save("MstOptn", "LeftTableY" + i.ToString(), LeftTable[i].dY);
                IniMstOptn.Save("MstOptn", "LeftTableT" + i.ToString(), LeftTable[i].dT);

                IniMstOptn.Save("MstOptn", "RightTableX" + i.ToString(), RightTable[i].dX);
                IniMstOptn.Save("MstOptn", "RightTableY" + i.ToString(), RightTable[i].dY);
                IniMstOptn.Save("MstOptn", "RightTableT" + i.ToString(), RightTable[i].dT);
            }

        }

        //여기부터
        public static void LoadEqpOptn()
        {
            string sModelName = "HVC-310LA";
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sEqpOptnPath = sExeFolder + "Util\\EqpOptn.ini";

            CIniFile IniEqpOptn = new CIniFile(sEqpOptnPath);

            IniEqpOptn.Load("DevInfo", "sModelName", out EqpOptn.sModelName);
            
            if (EqpOptn.sModelName != "") EqpOptn.sModelName = sModelName;
        }
        public static void SaveEqpOptn()
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sEqpOptnPath = sExeFolder + "Util\\EqpOptn.ini";

            CIniFile IniEqpOptn = new CIniFile(sEqpOptnPath);

            IniEqpOptn.Save("DevInfo", "sModelName", EqpOptn.sModelName);
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
