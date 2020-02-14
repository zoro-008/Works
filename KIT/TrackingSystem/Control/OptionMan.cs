using COMMON;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Control
{
    public enum Mode : uint 
    {
        AutoMode1 , 
        AutoMode2 , 
        ManualMode 
    }

    [Serializable]
    public class CMode
    {
        [CategoryAttribute("Mode"), DisplayNameAttribute("Select Mode")]public Mode iMode { get; set; }
    };
    [Serializable]
    public class CAuto1
    {
        private int _iAStrokeStart     ;
        private int _iAStrokeEnd       ;
        private int _iAInterval        ;
        private int _iAResidenceTime   ;
        private int _iAMeasureCnt      ;
        private int _iAMeasureTime     ;
        [CategoryAttribute("Auto Mode1"  ), DisplayNameAttribute("Stroke Start (cm)"                  ),
            //1.0.0.1
            //DescriptionAttribute("0 ~ 210 cm \r\nStart position must be greater than end position.")]
            DescriptionAttribute("0 ~ 210 cm \r\nStart position must be less than end position.")]
            public  int iAStrokeStart  {
                get {
                    if (_iAStrokeStart > 210) _iAStrokeStart = 210 ;
                    if (_iAStrokeStart <   0) _iAStrokeStart =   0 ;
                    return _iAStrokeStart;
                }
                set {
                    _iAStrokeStart = value;
                }
            }
        [CategoryAttribute("Auto Mode1"  ), DisplayNameAttribute("Stroke End (cm)"                    ),
            //1.0.0.1
            //DescriptionAttribute("0 ~ 210 cm \r\nEnd position must be less than start position."   )]
            DescriptionAttribute("0 ~ 210 cm \r\nEnd position must be greater than start position."   )]
            public int iAStrokeEnd    {
                get {
                    if (_iAStrokeEnd > 210) _iAStrokeEnd = 210 ;
                    if (_iAStrokeEnd <   0) _iAStrokeEnd =   0 ;
                    return _iAStrokeEnd;
                }
                set {
                    _iAStrokeEnd = value;
                }
            }
        [CategoryAttribute("Auto Mode1"  ), DisplayNameAttribute("Interval (cm)"                      ),DescriptionAttribute("0 ~ 50 cm"     )]
            public int iAInterval {   
                get {
                    if (_iAInterval > 50) _iAInterval = 50 ;
                    if (_iAInterval <  0) _iAInterval =  0 ;
                    return _iAInterval;
                }
                set {
                    _iAInterval = value;
                }
            }
        [CategoryAttribute("Auto Mode1"  ), DisplayNameAttribute("Residence time (msec)"              ),DescriptionAttribute("0 ~ 6000 msec" )]
            public int iAResidenceTime { 
                get {
                    if (_iAResidenceTime > 6000) _iAResidenceTime = 6000 ;
                    if (_iAResidenceTime <    0) _iAResidenceTime =    0 ;
                    return _iAResidenceTime;
                }
                set {
                    _iAResidenceTime = value;
                }
            }
        [CategoryAttribute("Auto Mode1"  ), DisplayNameAttribute("Measuring count/position (repeat)"  ),DescriptionAttribute("0 ~ 30 count"  )]
            public int iAMeasureCnt {  
                get {
                    if (_iAMeasureCnt > 30) _iAMeasureCnt = 30 ;
                    if (_iAMeasureCnt <  0) _iAMeasureCnt =  0 ;
                    return _iAMeasureCnt;
                }
                set {
                    _iAMeasureCnt = value;
                }
            }
        [CategoryAttribute("Auto Mode1"  ), DisplayNameAttribute("Measuring time/position (msec)"     ),DescriptionAttribute("0 ~ 20000 msec"  )]
            public int iAMeasureTime {
                get {
                    if (_iAMeasureTime > 20000) _iAMeasureTime = 20000 ;
                    if (_iAMeasureTime <     0) _iAMeasureTime =    0  ;
                    return _iAMeasureTime;
                }
                set {
                    _iAMeasureTime = value;
                }
            }
    };

    [Serializable]
    public class CManual
    {
        private int _iMPosition       ;
        private int _iMResidenceTime  ;
        private int _iMMeasureCnt     ;
        private int _iMMeasureTime    ;

        [CategoryAttribute("Manual Mode" ), DisplayNameAttribute("Position (cm)"                      ),DescriptionAttribute("0 ~ 210 cm"    )]
            public int iMPosition     {
                get {
                    if (_iMPosition > 210) _iMPosition = 210 ;
                    if (_iMPosition <   0) _iMPosition =   0 ;
                    return _iMPosition;
                }
                set {
                    _iMPosition = value;
                }
            }
        [CategoryAttribute("Manual Mode" ), DisplayNameAttribute("Residence time (msec)"              ),DescriptionAttribute("0 ~ 6000 msec" )]
            public int iMResidenceTime{
                get {
                    if (_iMResidenceTime > 6000) _iMResidenceTime = 6000 ;
                    if (_iMResidenceTime <    0) _iMResidenceTime =    0 ;
                    return _iMResidenceTime;
                }
                set {
                    _iMResidenceTime = value;
                }
            }
        [CategoryAttribute("Manual Mode" ), DisplayNameAttribute("Measuring count/position (repeat)"  ),DescriptionAttribute("0 ~ 30 count"  )]
            public int iMMeasureCnt   {
                get {
                    if (_iMMeasureCnt > 30) _iMMeasureCnt = 30 ;
                    if (_iMMeasureCnt <  0) _iMMeasureCnt =  0 ;
                    return _iMMeasureCnt;
                }
                set {
                    _iMMeasureCnt = value;
                }
            }
        [CategoryAttribute("Manual Mode" ), DisplayNameAttribute("Measuring time/position (msec)"     ),DescriptionAttribute("0 ~ 20000 msec" )]
            public int iMMeasureTime  {
                get {
                    if (_iMMeasureTime > 20000) _iMMeasureTime = 20000 ;
                    if (_iMMeasureTime <     0) _iMMeasureTime =     0 ;
                    return _iMMeasureTime;
                }
                set {
                    _iMMeasureTime = value;
                }
            }
    } ;                                                                                               
    

    //---------------------------------------------------------------------------
    public static class OM
    {
        public const int MotionInput2 = 2 ;
        public const int MotionInput3 = 3 ;
        public const int m_iMaxArray = 211;

        public struct CDevInfo
        {   //device에 대한 Dev_Info
            public int    iTRAY_PcktCntX   ;
            public int    iTRAY_PcktCntY   ;
            public double dTRAY_PcktPitchX ;
            public double dTRAY_PcktPitchY ;
            public double dTRAY_Thickness  ;
            public int    iTRAY_StackingCnt;
            
        } ;

        public struct CAuto2
        {   
            public int iBTime;
            public int iCount;
            public int iATime;
        } ;

        
        
        

        public struct CCmnOptn
        {
            public bool bEthernet ; // other Usb
            public int KysAdd1 ;
            public int KysAdd2 ;
            public int KysAdd3 ;
            public int KysAdd4 ;
            public int KysPort ;

            public bool bOut1  ;
            public bool bOut2  ;
            public bool bOut3  ;
            public bool bOut4  ;


        }

        public static string m_sCrntDev; //Current open device.
        
        public static CDevInfo DevInfo;
        public static CCmnOptn CmnOptn;
        public static CMode    Mode   = new CMode();
        public static CAuto1   Auto1  = new CAuto1();
        public static CManual  Manual = new CManual();
        public static CAuto2[] Auto2  = new CAuto2[m_iMaxArray];

        public static void Init()
        {
            //Load
            LoadLastInfo();
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
            LoadPara(_sDevName);
            LoadAuto2Info(_sDevName);
            //Set Current Device Name.
            SetCrntDev(_sDevName);
            
        }
        public static void SaveJobFile(string _sDevName)
        {
            SaveDevInfo(_sDevName);
            SavePara(_sDevName);
            SaveAuto2Info(_sDevName);
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

        public static bool LoadPara(string _sJobName)
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sPath      = sExeFolder + "JobFile\\" + _sJobName + "\\Mode.ini";
            if (!CXml.LoadXml(sPath, ref Mode  )) { return false; }
            sPath      = sExeFolder + "JobFile\\" + _sJobName + "\\Auto1.ini";
            if (!CXml.LoadXml(sPath, ref Auto1 )) { return false; }
            sPath      = sExeFolder + "JobFile\\" + _sJobName + "\\Manual.ini";
            if (!CXml.LoadXml(sPath, ref Manual)) { return false; }
            return true;
        }
        public static bool SavePara(string _sJobName)
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sPath      = sExeFolder + "JobFile\\" + _sJobName + "\\Mode.ini";
            if (!CXml.SaveXml(sPath, ref Mode  )) { return false; }
            sPath      = sExeFolder + "JobFile\\" + _sJobName + "\\Auto1.ini";
            if (!CXml.SaveXml(sPath, ref Auto1 )) { return false; }
            sPath      = sExeFolder + "JobFile\\" + _sJobName + "\\Manual.ini";
            if (!CXml.SaveXml(sPath, ref Manual)) { return false; }
            return true;
        }

        public static void LoadAuto2Info(string _sJobName)
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevInfoPath = sExeFolder + "JobFile\\" + _sJobName + "\\Auto2Info.ini";
            CIniFile IniLotInfo = new CIniFile(sDevInfoPath);
            for(int i=0; i<m_iMaxArray; i++)
            {
                IniLotInfo.Load("iBTime", i.ToString(), out Auto2[i].iBTime);
                IniLotInfo.Load("iCount", i.ToString(), out Auto2[i].iCount);
                IniLotInfo.Load("iATime", i.ToString(), out Auto2[i].iATime);
            }
        }

        public static void SaveAuto2Info(string _sJobName)
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevInfoPath = sExeFolder + "JobFile\\" + _sJobName + "\\Auto2Info.ini";
            CIniFile IniLotInfo = new CIniFile(sDevInfoPath);
            for(int i=0; i<m_iMaxArray; i++)
            {
                IniLotInfo.Save("iBTime", i.ToString(), Auto2[i].iBTime);
                IniLotInfo.Save("iCount", i.ToString(), Auto2[i].iCount);
                IniLotInfo.Save("iATime", i.ToString(), Auto2[i].iATime);
            }
        }

        
    };
}
