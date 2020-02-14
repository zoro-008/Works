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
        enum EN_WORK_MODE { wmNormal = 0, wmHeight = 1 }; //0:정상 작업 1:로더에서 꺼내어 로테이터에서 높이 측정만 하고 다시 넣는다.
        public struct CDevInfo
        {   //device에 대한 Dev_Info
            public int    iTrayColCnt  ;
            public int    iTrayRowCnt  ;
            public double dTrayColPitch;
            public double dTrayRowPitch;

            public int    iVsnInspColCnt ;
            public int    iVsnInspRowCnt ;

            public double dTrayHeight   ;

            public string sVsnIndx      ;

            public string sMrkData      ;
            public int    iMrkReptDelay ;

        } ;

        public struct CDevOptn             //Device에 따라 바뀌는 옵션.
        {
            public int    iLDRTrayCheckTime; 
            public int    iPickWaitTime    ;  //Pick 하기 전 대기 시간.
            public int    iAlgnWaitTime    ;  //Align 하기 전 대기 시간.
            public int    iPlceWaitTime    ; //클린 후 납 중복 공급 카운트

            public double dPckShakeDistance; //픽커 흔드는 거리 넣는 옵션
            public bool   bUsePckShake     ; //픽커 흔드는 옵션 사용 여부
            public int    iPckShakeCnt     ; //픽커 흔드는 옵션 사용 여부
            public double dPckShakeZOfs    ; //픽커 흔드는 옵션 사용 여부
            

        } ;


        public struct CCmnOptn //장비 공용 옵션.
        {
            public bool bIgnrDoor;
            
            public bool bVisnSkip;
            public bool bAirBlwrSkip;

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

        public static string m_sCrntDev; //Current open device.
        
        public static CDevInfo DevInfo;
        public static CDevOptn DevOptn;
        public static CCmnOptn CmnOptn;
        public static CMstOptn MstOptn;
        public static CEqpOptn EqpOptn;


        public static void Init()
        {
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

            IniDevInfo.Load("DevInfo", "iTrayColCnt"    , out DevInfo.iTrayColCnt   );
            IniDevInfo.Load("DevInfo", "iTrayRowCnt"    , out DevInfo.iTrayRowCnt   );
            IniDevInfo.Load("DevInfo", "dTrayColPitch"  , out DevInfo.dTrayColPitch );
            IniDevInfo.Load("DevInfo", "dTrayRowPitch"  , out DevInfo.dTrayRowPitch );
                                                        
            IniDevInfo.Load("DevInfo", "iVsnInspColCnt" , out DevInfo.iVsnInspColCnt);
            IniDevInfo.Load("DevInfo", "iVsnInspRowCnt" , out DevInfo.iVsnInspRowCnt);

            IniDevInfo.Load("DevInfo", "dTrayHeight"    , out DevInfo.dTrayHeight   );

            IniDevInfo.Load("DevInfo", "sVsnIndx"       , out DevInfo.sVsnIndx      );

            IniDevInfo.Load("DevInfo", "sMrkData"       , out DevInfo.sMrkData      );
            IniDevInfo.Load("DevInfo", "iMrkReptDelay"  , out DevInfo.iMrkReptDelay );

            
            
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

            IniDevInfo.Save("DevInfo", "iVsnInspColCnt", DevInfo.iVsnInspColCnt);
            IniDevInfo.Save("DevInfo", "iVsnInspRowCnt", DevInfo.iVsnInspRowCnt);

            IniDevInfo.Save("DevInfo", "dTrayHeight"   , DevInfo.dTrayHeight   );

            IniDevInfo.Save("DevInfo", "sVsnIndx"      , DevInfo.sVsnIndx      );

            IniDevInfo.Save("DevInfo", "sMrkData"      , DevInfo.sMrkData      );
            IniDevInfo.Save("DevInfo", "iMrkReptDelay" , DevInfo.iMrkReptDelay );
            

        }

        public static void LoadDevOptn(string _sJobName) 
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevOptnPath = sExeFolder + "JobFile\\" + _sJobName + "\\DevOptn.ini";

            CIniFile IniDevOptn = new CIniFile(sDevOptnPath);


            IniDevOptn.Load("DevOptn", "iLDRTrayCheckTime", out DevOptn.iLDRTrayCheckTime);
            IniDevOptn.Load("DevOptn", "iPickWaitTime"    , out DevOptn.iPickWaitTime    );
            IniDevOptn.Load("DevOptn", "iAlgnWaitTime"    , out DevOptn.iAlgnWaitTime    );
            IniDevOptn.Load("DevOptn", "iPlceWaitTime"    , out DevOptn.iPlceWaitTime    );
            IniDevOptn.Load("DevOptn", "dPckShakeDistance", out DevOptn.dPckShakeDistance);
            IniDevOptn.Load("DevOptn", "bUsePckShake"     , out DevOptn.bUsePckShake     );
            IniDevOptn.Load("DevOptn", "dPckShakeZOfs"    , out DevOptn.dPckShakeZOfs    );

            IniDevOptn.Load("DevOptn", "iPckShakeCnt"     , out DevOptn.iPckShakeCnt     );

            
            
            
           
        }

        public static void SaveDevOptn(string _sJobName) 
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevOptnPath = sExeFolder + "JobFile\\" + _sJobName + "\\DevOptn.ini";

            CIniFile IniDevOptn = new CIniFile(sDevOptnPath);

            IniDevOptn.Save("DevOptn", "iLDRTrayCheckTime", DevOptn.iLDRTrayCheckTime);
            IniDevOptn.Save("DevOptn", "iPickWaitTime"    , DevOptn.iPickWaitTime    );
            IniDevOptn.Save("DevOptn", "iAlgnWaitTime"    , DevOptn.iAlgnWaitTime    );
            IniDevOptn.Save("DevOptn", "iPlceWaitTime"    , DevOptn.iPlceWaitTime    );
            IniDevOptn.Save("DevOptn", "dPckShakeDistance", DevOptn.dPckShakeDistance);
            IniDevOptn.Save("DevOptn", "bUsePckShake"     , DevOptn.bUsePckShake     );
            IniDevOptn.Save("DevOptn", "dPckShakeZOfs"    , DevOptn.dPckShakeZOfs    );

            IniDevOptn.Save("DevOptn", "iPckShakeCnt"     , DevOptn.iPckShakeCnt     );
            
        }

        public static void LoadCmnOptn() 
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sCmnOptnPath = sExeFolder + "Util\\CmnOptn.ini";

            CIniFile IniCmnOptn = new CIniFile(sCmnOptnPath);

            IniCmnOptn.Load("CmnOptn", "bIgnrDoor"     , out CmnOptn.bIgnrDoor     );
                                                                                   
            IniCmnOptn.Load("CmnOptn", "bVisnSkip"     , out CmnOptn.bVisnSkip     );
            IniCmnOptn.Load("CmnOptn", "bAirBlwrSkip"  , out CmnOptn.bAirBlwrSkip  );

            

            

        }
        public static void SaveCmnOptn()
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sCmnOptnPath = sExeFolder + "Util\\CmnOptn.ini";

            CIniFile IniCmnOptn = new CIniFile(sCmnOptnPath);

            IniCmnOptn.Save("CmnOptn", "bIgnrDoor"     , CmnOptn.bIgnrDoor     );
                                                                               
            IniCmnOptn.Save("CmnOptn", "bVisnSkip"     ,  CmnOptn.bVisnSkip    );
            IniCmnOptn.Save("CmnOptn", "bAirBlwrSkip"  ,  CmnOptn.bAirBlwrSkip );
            
        }

        public static void LoadMstOptn() 
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sMstOptnPath = sExeFolder + "Util\\MstOptn.ini";

            CIniFile IniMstOptn = new CIniFile(sMstOptnPath);

            IniMstOptn.Load("MstOptn", "bDebugMode     "  , out MstOptn.bDebugMode   );
            IniMstOptn.Load("MstOptn", "bIdleRun       "  , out MstOptn.bIdleRun     );

            IniMstOptn.Load("MstOptn", "dTrgOfs        "  , out MstOptn.dTrgOfs      );
            
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
            string sModelName = "PMK-100";
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
