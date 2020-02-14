using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using COMMON;
using System.Globalization;
using System.IO;

using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Machine
{
    public class Macro : MacroCmd
    {
        public EzSensor Ez1;
        public Dressy   Dr1;
        public int m_iNo = 0;

        public bool CycleIng;
        public bool CycleEnd;
        public System.Windows.Forms.Timer tmUpdate;

        public Macro(int _iPartId = 0)
        {
            Ez1 = new EzSensor();
            Dr1 = new Dressy  ();
            m_iPartId = _iPartId;
            CycleIng = false;
            CycleEnd = false;

            Ez1.InitCycle();
            Dr1.InitCycle();

            tmUpdate = new System.Windows.Forms.Timer();
            tmUpdate.Interval = 150;
            tmUpdate.Tick += new EventHandler(tmUpdate_Tick); //
            tmUpdate.Enabled = true;

        }

        public void Stop()
        {
            if (OM.DevInfo.iMacroType == 0)//드레시
            {
                Dr1.Stop();
            }
            else
            {
                Ez1.Stop();
            }
        }

        public string GetErrCode()
        {
            string sErr = "";
            if (OM.DevInfo.iMacroType == 0)
            {
                sErr = Dr1.GetErrCode();
            }
            else if (OM.DevInfo.iMacroType == 1)
            {
                sErr = Ez1.GetErrCode();
            }

            if(sErr != "")
            {
                m_iNo = 0;
            }
            return sErr;
        }
        public string Test()
        {
            string sTemp = Dr1.Test();
            return sTemp;
        }
        public void Reset()
        {
            Ez1.Reset();
            Dr1.Reset();
            CycleIng = false;
            CycleEnd = false;
            m_iNo = 0;

            tmUpdate.Enabled = true;
          
        }
        
        private void Init()
        {
            CycleIng = false;
            CycleEnd = false;
            m_iNo = 0;
            tmUpdate.Enabled = true;
        }
        //드레시용임 다른거할때는 하나더 만들~
        public void CycleDressyInit(Dressy.SSetting Setting)
        {
            if (OM.DevInfo.iMacroType == 0)
            {
                Dr1.InitCycle();
                Dr1.Setting.smA       = Setting.smA       ;
                Dr1.Setting.sKvp      = Setting.sKvp      ;
                Dr1.Setting.sTime     = Setting.sTime     ;
                Dr1.Setting.sFileName1= Setting.sFileName1; 
                Dr1.Setting.sFileName2= Setting.sFileName2; 
                Dr1.Setting.sBind     = Setting.sBind     ;
                //Dr1.Setting.sGain     = Setting.sGain     ;
                Dr1.Setting.sAreaUp   = Setting.sAreaUp   ;
                Dr1.Setting.sAreaDn   = Setting.sAreaDn   ;
                Dr1.Setting.sAreaLt   = Setting.sAreaLt   ;
                Dr1.Setting.sAreaRt   = Setting.sAreaRt   ;
                Dr1.Setting.sPath1    = Setting.sPath1    ; //TODO :: 패스가 총 네개 인데 여기 두개만 있어서 헤깔림.
                Dr1.Setting.sPath2    = Setting.sPath2    ;

                Dr1.Setting.iAcq1     = Setting.iAcq1     ;
                Dr1.Setting.iAcq2     = Setting.iAcq2     ;
                Dr1.Setting.iAcq3     = Setting.iAcq3     ;
                Dr1.Setting.iAcq4     = Setting.iAcq4     ;
                Dr1.Setting.iAcq5     = Setting.iAcq5     ;
                Dr1.Setting.iAcq6     = Setting.iAcq6     ;
                Dr1.Setting.iAcq7     = Setting.iAcq7     ;

            }
            Init();
        }

        public void CycleEzSensorInit(EzSensor.SSetting Setting)
        {
            if (OM.DevInfo.iMacroType == 1)
            {
                Ez1.InitCycle();
                Ez1.Setting.smA           = Setting.smA          ;
                Ez1.Setting.sKvp          = Setting.sKvp         ;
                Ez1.Setting.sTime         = Setting.sTime        ;
                Ez1.Setting.iBinning      = Setting.iBinning     ;
                Ez1.Setting.iWidth        = Setting.iWidth       ;
                Ez1.Setting.iHeight       = Setting.iHeight      ;
                Ez1.Setting.sFileName     = Setting.sFileName    ;   
                Ez1.Setting.sGetImgFdName = Setting.sGetImgFdName; 
                Ez1.Setting.sFlatPath     = Setting.sFlatPath    ; 
                Ez1.Setting.sObjtPath     = Setting.sObjtPath    ;
                Ez1.Setting.sCalFolder    = Setting.sCalFolder   ;//1x1, 2x2에 따라 CAL, CAL_A에 저장 되서 셋팅함(최종 저장 폴더만 적용됨)
                Ez1.Setting.sDarkImgPath  = Setting.sDarkImgPath ;
                Ez1.Setting.sSaveFolder   = Setting.sSaveFolder  ;
                Ez1.Setting.iDimWidth     = Setting.iDimWidth    ;    
                Ez1.Setting.iDimHght      = Setting.iDimHght     ;
                Ez1.Setting.dPixelPitch   = Setting.dPixelPitch  ;
                Ez1.Setting.iCutoffbevel  = Setting.iCutoffbevel ;

                Ez1.Setting.iNPSLeft      = Setting.iNPSLeft     ;
                Ez1.Setting.iNPSTop       = Setting.iNPSTop      ;
                Ez1.Setting.iNPSW         = Setting.iNPSW        ;
                Ez1.Setting.iNPSH         = Setting.iNPSH        ;
                                                                 
                Ez1.Setting.iMTFLeft      = Setting.iMTFLeft     ;
                Ez1.Setting.iMTFTop       = Setting.iMTFTop      ;
                Ez1.Setting.iMTFW         = Setting.iMTFW        ;
                Ez1.Setting.iMTFH         = Setting.iMTFH        ;

                Ez1.Setting.iAcqMaxFrame  = Setting.iAcqMaxFrame ;
                Ez1.Setting.iAcqInterval  = Setting.iAcqInterval ;
            }
            Init();
        }

        public bool CycleDressy(int _iNo)
        {
            m_iNo    = _iNo;
            CycleIng = true;
            return CycleEnd;
        }

        public bool CycleEzSensor(int _iNo)
        {
            m_iNo    = _iNo;
            CycleIng = true;
            return CycleEnd;
        }

        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            tmUpdate.Enabled = false;
            bool bRet = false;
            if (OM.DevInfo.iMacroType == 0 && m_iNo != 0)
            {
                if(m_iNo ==  1) bRet = Dr1.Cycle1 ();
                if(m_iNo ==  2) bRet = Dr1.Cycle2 ();
                if(m_iNo ==  3) bRet = Dr1.Cycle3 ();
                if(m_iNo ==  4) bRet = Dr1.Cycle4 ();
                if(m_iNo ==  5) bRet = Dr1.Cycle5 ();
                if(m_iNo ==  6) bRet = Dr1.Cycle6 ();
                if(m_iNo ==  7) bRet = Dr1.Cycle7 ();
                if(m_iNo ==  8) bRet = Dr1.Cycle8 ();
                if(m_iNo ==  9) bRet = Dr1.Cycle9 ();
                if(m_iNo == 10) bRet = Dr1.Cycle10();
                if(m_iNo == 11) bRet = Dr1.Cycle11();
                if(m_iNo == 12) bRet = Dr1.Cycle12();
                if(m_iNo == 13) bRet = Dr1.Cycle13();
            }
            else if (OM.DevInfo.iMacroType == 1 && m_iNo != 0)
            {
                if (m_iNo == 1 ) bRet = Ez1.CycleEntering    ();
                if (m_iNo == 2 ) bRet = Ez1.CycleTrigger     ();
                if (m_iNo == 3 ) bRet = Ez1.CycleAging       ();
                if (m_iNo == 4 ) bRet = Ez1.CycleGetBright   ();
                if (m_iNo == 5 ) bRet = Ez1.CycleGetImage    ();
                if (m_iNo == 6 ) bRet = Ez1.CycleCalGen      ();
                if (m_iNo == 7 ) bRet = Ez1.CyclePreGen      ();
                if (m_iNo == 8 ) bRet = Ez1.CycleDQE1        ();
                if (m_iNo == 9 ) bRet = Ez1.CycleDQE2        ();
                if (m_iNo == 10) bRet = Ez1.CycleCalibration1();
                if (m_iNo == 11) bRet = Ez1.CycleCalibration2();
                if (m_iNo == 12) bRet = Ez1.CycleSkull       ();
                if (m_iNo == 13) bRet = Ez1.CycleRsltCopy    ();
            }
            
            if (bRet)
            {
                CycleIng = false;
                CycleEnd = true ;
                m_iNo    = 0;
                tmUpdate.Enabled = true;
                return;
            }
            tmUpdate.Enabled = true;
            
        }
    }

    public class Dressy : MacroCmd
    {
        private int m_iCycle;
        private int m_iStart; //Cycle0에서 case 넘길때 사용 m_iCycle은 Cycle1~13에서 써서 달리 쓴다.

        private int m_iPreCycle;
        private int m_iPreStart;//Cycle0에서 case 넘길때 사용 m_iCycle은 Cycle1~13에서 써서 달리 쓴다.
        public string sErrMsg;
        private const string m_sPartName = "Dressy ";
        protected CDelayTimer m_tmCycle = new CDelayTimer();
        protected CDelayTimer m_tmDelay = new CDelayTimer();

        public string Test()
        {
            return m_iCycle.ToString();

        }
        public struct SSetting
        {
            public string smA        ;
            public string sKvp       ;
            public string sTime      ;
            public string sFileName1 ;
            public string sFileName2 ;
            public string sBind      ;
            //public string sGain      ;       
            public string sAreaUp    ;
            public string sAreaDn    ;
            public string sAreaLt    ;
            public string sAreaRt    ;
            public string sPath1     ;
            public string sPath2     ;
            public int    iAcq1      ;//3  offset Calibration
            public int    iAcq2      ;//4  Gain Calibration
            public int    iAcq3      ;//5  Bpm Correction
            public int    iAcq4      ;//6  Direct Hit Filtering
            public int    iAcq5      ;//7  Gamma Correction
            public int    iAcq6      ;//10 Char. Curve
            public int    iAcq7      ;//8  Continuous acquisition

            //4Point
            public int    iMin       ;
            public int    iMax       ;
            
        };
        public struct SResult
        {
            public string Blemish_amount                ;
            public string Row_defects                   ;
            public string Column_defects                ;
            public string Adjacent_columns              ;
            public string Adjacent_rows                 ;
            public string Total_blemish_count           ;
            public string Offset                        ;
            public string Trigger_sensitivity           ;
            public string SNR_R60_100                   ;
            public string SNR_R70_100                   ;
            public string SNR_R60_400                   ;
            public string SNR_R70_400                   ;
            public string SNR_C60_100                   ;
            public string SNR_C70_100                   ;
            public string SNR_C60_400                   ;
            public string SNR_C70_400                   ;
            public string Signal_1x1_mode               ;
            public string Noise_1x1_mode                ;
            public string Sensitivity                   ;
            public string Low_frequency_non_uniformity1 ;
            public string Uniformity                    ;
            public string Low_frequency_non_uniformity2 ;
            public string MTF_mode_1x1_at_5_lp_mm       ;
            public string MTF_mode_1x1_at_8_lp_mm       ;
            public string MTF_mode_1x1_at_12_lp_mm      ;
            public string DQE                           ;
            public string Saturation_dose               ;
            public string Linearity_error               ;
            public string SNR_2x2_mode                  ;
            public string Signal_2x2_mode               ;
            public string Noise_2x2_mode                ;
            public string MTF_mode_2x2_at_8_lp_mm       ;

            public void Clear()
            {
                Blemish_amount                    = "" ;
                Row_defects                       = "" ;
                Column_defects                    = "" ;
                Adjacent_columns                  = "" ;
                Adjacent_rows                     = "" ;
                Total_blemish_count               = "" ;
                Offset                            = "" ;
                Trigger_sensitivity               = "" ;
                SNR_R60_100                       = "" ;
                SNR_R70_100                       = "" ;
                SNR_R60_400                       = "" ;
                SNR_R70_400                       = "" ;
                SNR_C60_100                       = "" ;
                SNR_C70_100                       = "" ;
                SNR_C60_400                       = "" ;
                SNR_C70_400                       = "" ;
                Signal_1x1_mode                   = "" ;
                Noise_1x1_mode                    = "" ;
                Sensitivity                       = "" ;
                Low_frequency_non_uniformity1     = "" ;
                Uniformity                        = "" ;
                Low_frequency_non_uniformity2     = "" ;
                MTF_mode_1x1_at_5_lp_mm           = "" ;
                MTF_mode_1x1_at_8_lp_mm           = "" ;
                MTF_mode_1x1_at_12_lp_mm          = "" ;
                DQE                               = "" ;
                Saturation_dose                   = "" ;
                Linearity_error                   = "" ;
                SNR_2x2_mode                      = "" ;
                Signal_2x2_mode                   = "" ;
                Noise_2x2_mode                    = "" ;
                MTF_mode_2x2_at_8_lp_mm           = "" ;
            }

        }
        public SSetting Setting   ;
        public SResult  Result    ;
        private string sSerial    ;
        private double dInspDely  ;
        private int    iCalRptCnt ; //1.0.1.7 Cal 편차 계산 반복한 횟수 이거 넘어가면 에러

        List<string> AllAgingFile  = new List<string>();//에이징할때 Offset 파일 말고 다른 파일 생성되는데 몇개 생성되는지 알수 없어 리스트에 넣는다.
        List<string> CrntAgingFile = new List<string>();//에이징할때 Offset 파일 말고 다른 파일 생성되는데 몇개 생성되는지 알수 없어 리스트에 넣는다.
        List<string> CalCrntImgFile = new List<string>();//Cal 이미지 받고 편차 구할 때 Cal 이미지 파일 이름 담을 리스트
        List<string> CalBfImgFile   = new List<string>();//Cal 공정이 2그룹으로 나눠지면 각 그룹별로 따로 담아야해서 이전에 작업한 Cal 그룹은 여기에 넣는다.
        private int iCalTypeCnt  = 0; //Cal 공정 총 수량. 전체에 대한 총 수량이 아닌 연속 작업 시 연속 작업한 총 수량이다. 중간에 끊고 다시 Cal 공정 들어간건 안 친다.
        //private int iDeleteIndex = 0; //지운 파일이 있던 리스트 인덱스 저장해놓고 리웍할때 그 위치에 고대로 쑤셔넣는다.

        public string GetRsltFd()
        {
            string s1 = "";
            string s2 = OM.EqpStat.sYear + "-" + OM.EqpStat.sMonth + "-" + OM.EqpStat.sDay;
            s1 = Setting.sPath1 + "\\" + s2 + "\\" + sSerial + "\\";
            return s1;
        }

        public string GetErrCode() { return sErrMsg; }

        public void InitCycle()
        {
            m_iCycle    = 10;
            m_iPreCycle =  0;
            //bErrDevice = false;
            iRptCnt = 0;
            iCalRptCnt = 0;
            sErrMsg = "";
        }
        public void InitStart()
        {
            m_iStart = 10;
            m_iPreStart = 0;
            sErrMsg = "";
        }

        public void Stop()
        {
            m_iCycle = 0;
        }

        public void Reset() { InitCycle(); }

        IntPtr m_iHwndM = IntPtr.Zero;
        IntPtr m_iHwnd1 = IntPtr.Zero; //휘발성
        IntPtr m_iHwnd2 = IntPtr.Zero; //휘발성
        IntPtr m_iHwnd3 = IntPtr.Zero; //휘발성
        IntPtr m_iHwnd4 = IntPtr.Zero; //휘발성

        IntPtr m_iHwndS = IntPtr.Zero;

        public int  iRptCnt       = 0; //X-Ray 조사 리핏 카운트
        public bool bDetectSerial = false; //시리얼 넘버 받아왔는지 알 길이 없어서 태그 두고 확인 함
        public bool bPass         = false; //Cal 공정 후 편차 확인하고 나서 이상 없으면 true 시킨다. iWorkStep을 제어쪽에서 관리해서 이렇게 쓴다.
        public bool bRework       = false; //편차 NG일때 자꾸 다음 스텝으로 움직여서 이거 false일때만 +1 하도록 한다.

        public void CalDataClear()
        {
            CalCrntImgFile.Clear();
            CalBfImgFile.Clear();
            iCalTypeCnt = 0;
            iCalRptCnt = 0;
            bRework = false;
        }

        /// <summary>
        /// Process Check
        /// 내부 사용 용
        /// </summary>
        /// <returns></returns>
        public bool Cycle0(bool _bFileCheck = false) //Process Check
        {
            String sTemp;
            if (m_tmCycle.OnDelay(m_iStart != 0 && m_iStart == m_iPreStart, 20000))
            {
                sTemp = string.Format("m_iStart={0:00}", m_iStart);
                if (sErrMsg == "") sErrMsg = "ERR100_" + m_sPartName + "Macro Time Out Error " + sTemp;
                Trace(sErrMsg);
                m_iStart = 0;
                return true;
            }

            if (m_iStart != m_iPreStart)
            {
                sTemp = string.Format("m_iStart={0:00}", m_iStart);
                Trace(sTemp);
                m_tmDelay.Clear();
            }

            m_iHwnd1 = IntPtr.Zero;
            m_iHwnd2 = IntPtr.Zero;
            m_iHwnd3 = IntPtr.Zero;
            m_iHwnd4 = IntPtr.Zero;

            string sPath = "";
            string sName = "";

            m_iPreStart = m_iStart;

            int iDly = 1000;//200;

            switch (m_iStart)
            {

                default:
                    sTemp = string.Format("Cycle_Default_Clear_m_iStart={0:000}", m_iStart);
                    sErrMsg = sTemp;
                    return true;


                case 0:
                    return true;

                case 10: //INI 디폴트로 덮어 씌우기
                    Trace("Process Check");
                    if (ExistProces(OM.DressyInfo.sAppName1))// || m_iHwndM == IntPtr.Zero*/)
                    {
                        m_tmDelay.Clear();
                        m_iStart = 20;
                        return false;
                    }
                    Trace("Process Check End");
                    m_tmDelay.Clear();
                    m_iStart++;
                    return false;

                case 11:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    Trace("Start Process");
                    InitProcess1(OM.DressyInfo.sAppPath1);
                    Trace("Start Process End");
                    m_tmDelay.Clear();
                    m_iStart++;
                    return false;

                case 12:
                    if (!m_tmDelay.OnDelay(10000)) return false;
                    Trace("Process Check");
                    if (!ExistProces(OM.DressyInfo.sAppName1))
                    {
                        sErrMsg = "ERR002_프로그램이 실행되지 않았습니다.(프로세스)";
                        return true;
                    }
                    Trace("Process Check End");
                    m_iHwndM = FindWindowL("#32770", "Dressy I/O Sensor Manufacturer Tool");
                    Trace("Handle Check");
                    if (m_iHwndM == IntPtr.Zero)
                    {
                        sErrMsg = "ERR002_프로그램이 실행되지 않았습니다.(핸들)";
                        return true;
                    }
                    Trace("Handle Check End");
                    Trace("Window Position Move");
                    MoveWindow(m_iHwndM, OM.DressyInfo.iPosX1, OM.DressyInfo.iPosY1, 825, 490, true);
                    //SetWindowPos(m_iHwndM, IntPtr.Zero, OM.DressyInfo.iPosX1, OM.DressyInfo.iPosY1, 0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);
                    Trace("Window Position Move End");
                    m_iStart = 20;
                    return false;

                case 20:
                    if (_bFileCheck)
                    {
                        if (!m_tmDelay.OnDelay(10000)) return false;
                    }
                    
                    Trace("Find Handle3");
                    m_iHwndM = FindWindowL("#32770", "Dressy I/O Sensor Manufacturer Tool");
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "#32770", null);
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    Trace("Find Serial");
                    sSerial = GetWindowText(m_iHwnd2, Tools.Static, 9);
                    Trace("Check Serial1");
                    if (sSerial == "-")
                    {
                        m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                        m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "#32770", null);
                        m_iHwnd3 = FindWindowChild(m_iHwnd2, IntPtr.Zero, "ComboBox", null);
                        PostMessage(m_iHwnd3, WM_KEYDOWN, 53, 0); //5
                        m_iStart++;
                        return false;
                    }
                    m_iStart = 25;
                    return false;

                case 21://Configuration Click
                    if (!m_tmDelay.OnDelay(7000)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "#32770", null);
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    Trace("Find Serial2");
                    sSerial = GetWindowText(m_iHwnd2, Tools.Static, 9);
                    Trace("Check Serial2");
                    if (sSerial == "-")
                    {
                        sErrMsg = "ERR003_시리얼 번호 확인 안됨.";
                        return true;
                    }
                    m_iStart = 25;
                    return false;

                case 25:
                    OM.EqpStat.sCrntSerial = sSerial;
                    bDetectSerial = true;
                    m_iStart = 0;
                    return true;
            }
        }

        /// <summary>
        /// Setting
        /// Configration Binding(1x1), Gain 1로 바꾸고 ~ Typical Data 까지
        /// </summary>
        /// <returns></returns>
        public bool Cycle1() //Setting
        {
            String sTemp;
            if (m_tmCycle.OnDelay(m_iCycle != 0 && m_iCycle == m_iPreCycle , 20000))
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                if (sErrMsg == "") sErrMsg = "ERR100_"+m_sPartName + "Macro Time Out Error " + sTemp;
                Trace(sErrMsg);
                m_iCycle = 0;
                return true;
            }

            if (m_iCycle != m_iPreCycle)
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                Trace(sTemp);
                m_tmDelay.Clear();
            }

            m_iHwnd1 = IntPtr.Zero;
            m_iHwnd2 = IntPtr.Zero;
            m_iHwnd3 = IntPtr.Zero;
            m_iHwnd4 = IntPtr.Zero;

            string sPath       = "";
            string sName       = "";

            string sYear  = OM.EqpStat.sYear  ;
            string sMonth = OM.EqpStat.sMonth ;
            string sDay   = OM.EqpStat.sDay   ;
            string sDateFull = sYear + "/" + sMonth + "/" + sDay;

            m_iPreCycle = m_iCycle;

            int iDly = 1000;//200;
            
            switch (m_iCycle)
            {

                default:
                    sTemp = string.Format("Cycle_Default_Clear_m_iCycle={0:000}", m_iCycle);
                    sErrMsg = sTemp;
                    return true;


                case 0:
                    return true;

                case 10: //INI 디폴트로 덮어 씌우기
                    string sPath1 = OM.DressyInfo.sCalPath + "\\Cal";
                    string sPath2 = OM.DressyInfo.sCalPath + "\\Temp";
                    string sPath3 = OM.DressyInfo.sCalPath + "\\Log";
                    DeleteFolder(sPath1);
                    DeleteFolder(sPath2);
                    DeleteFolder(sPath3);
                    if (!File.Exists(OM.DressyInfo.sIniPath1))
                    {
                        sErrMsg = "ERR001_Ini파일을 찾을 수 없습니다.";
                        return true;
                    }
                    sPath = OM.DressyInfo.sIniPath2 + "\\";
                    sName = Path.GetFileName(OM.DressyInfo.sIniPath1);
                    if (File.Exists(sPath + sName))
                    {
                        File.Delete(sPath + sName);
                    }
                    File.Copy(OM.DressyInfo.sIniPath1,sPath + sName);

                    Result.Clear();

                    m_iCycle++;
                    return false;

                case 11:
                    InitStart();
                    m_iCycle++;
                    return false;

                case 12:
                    if (!Cycle0()) return false;
                    m_tmDelay.Clear();
                    
                    m_iCycle = 22;
                    return false;

                case 22: //Device Enumeration 핸들 받아서 콤보 박스에서 5 클릭
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    sPath1 = OM.DressyInfo.sCalPath + "\\Cal\\" + sSerial;

                    DirectoryInfo dir = new DirectoryInfo(sPath1);
                    if (dir.Exists) //읽기전용 파일들 찾아서 노멀로 바꿔주면 삭제 가능
                    {
                        FileInfo[] files = dir.GetFiles("*.*",
                    
                        SearchOption.AllDirectories);

                        foreach (FileInfo file in files)
                        {
                            file.Attributes = FileAttributes.Normal;
                            file.Delete();
                        }
                    }

                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 23:
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "#32770", null);
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    
                    m_iHwnd3 = FindWindowChild(m_iHwnd2, IntPtr.Zero, "Button", "Configuration");
                    Click(m_iHwnd3);
                    m_iCycle++;
                    return false;

                case 24: //Configuration Setting
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770", "Device configuration");
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "ComboBox", 0);
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    SetComboBoxIndex(m_iHwnd2,  0); //PostMessage로 셋팅 안바껴서 이걸로 바꿈 진섭.

                    //20180716 오성철 과장 요청으로 선계원 수정.
                    //2로 수정.
                    //밑에도 세팅 하고 확인 하는 부분 있음.
                    //20180724 제어프로그램에서 관여하지 않도록 요청 
                    //SetWindowText(m_iHwnd1,Tools.Edit,4, "2"); //4번째가 Gain Level

                    SetWindowText(m_iHwnd1,Tools.Edit,7 , Setting.sAreaUp);
                    SetWindowText(m_iHwnd1,Tools.Edit,10, Setting.sAreaDn);
                    SetWindowText(m_iHwnd1,Tools.Edit,8 , Setting.sAreaLt);
                    SetWindowText(m_iHwnd1,Tools.Edit,9 , Setting.sAreaRt);

                    m_iCycle++;
                    return false;

                case 25: //Apply
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770", "Device configuration");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "Apply");
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    Click(m_iHwnd2);
                    m_iCycle++;
                    return false;

                case 26: //Close
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770", "Device configuration");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "Close");
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    Click(m_iHwnd2);
                    m_iCycle++;
                    return false;

                case 27: //Configuration
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "#32770", null);
                    m_iHwnd3 = FindWindowChild(m_iHwnd2, IntPtr.Zero, "Button", "Configuration");
                    if (m_iHwnd3 == IntPtr.Zero) return false;
                    Click(m_iHwnd3);
                    m_iCycle++;
                    return false;

                case 28: //값 잘 들어갓나 한번 확인하고
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770", "Device configuration");
                    if(GetWindowText(m_iHwnd1, Tools.ComboBox, 0) != "1x1")
                    {
                        sErrMsg = "ERR000_Binning Mode 셋팅 실패";
                        return true;
                    }
                    //20180716 오성철과장 요청으로 
                    //위에서 2로 바꾸고 확인 하는 부분도 2로 바꿈.    GetWindowText(m_iHwnd1,Tools.Edit, 4)
                    //20180724 제어프로그램에서 관여하지 않도록 요청 
                    //if (GetWindowText(m_iHwnd1, Tools.Edit, 4) != "2")
                    //{
                    //    sErrMsg = "ERR000_Gain Level 셋팅 실패";
                    //    return true;
                    //}
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "Close");
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    Click(m_iHwnd2);
                    m_iCycle++;
                    return false;

                case 29: //Typical Data Click (Device typical data)
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "#32770", null);
                    m_iHwnd3 = FindWindowChild(m_iHwnd2, IntPtr.Zero, "Button", "Typical Data");
                    if (m_iHwnd3 == IntPtr.Zero) return false;
                    Click(m_iHwnd3);
                    m_iCycle++;
                    return false;

                case 30: //Typical Data Setting
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770", "Device typical data");
                    SetWindowText(m_iHwnd1,Tools.Edit,1 ,OM.DressyInfo.s1); //
                    SetWindowText(m_iHwnd1,Tools.Edit,2 ,OM.DressyInfo.s2); //
                    SetWindowText(m_iHwnd1,Tools.Edit,3 ,OM.DressyInfo.s3); //
                    SetWindowText(m_iHwnd1,Tools.Edit,4 ,OM.DressyInfo.s4); //

                    
                    SetWindowText(m_iHwnd1,Tools.Edit,5 ,sDateFull); //오늘날짜 넣으면 되겟지?
                    SetWindowText(m_iHwnd1,Tools.Edit,6 ,sDateFull); //Write할때  yyyy-MM-dd 로 씀

                    SetWindowText(m_iHwnd1,Tools.Edit,11,OM.DressyInfo.s5); //
                    SetWindowText(m_iHwnd1,Tools.Edit,12,OM.DressyInfo.s6); //
                    SetWindowText(m_iHwnd1,Tools.Edit,13,OM.DressyInfo.s7); //
                    m_iCycle++;
                    return false;

                case 31: //Apply
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770", "Device typical data");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "Apply");
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    Click(m_iHwnd2);
                    m_iCycle++;
                    return false;

                case 32: //Close
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770", "Device typical data");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "Close");
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    Click(m_iHwnd2);
                    m_iCycle++;
                    return false;

                case 33: //Typical Data Click (Device typical data)
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "#32770", null);
                    m_iHwnd3 = FindWindowChild(m_iHwnd2, IntPtr.Zero, "Button", "Typical Data");
                    if (m_iHwnd3 == IntPtr.Zero) return false;
                    Click(m_iHwnd3);
                    m_iCycle++;
                    return false;

                case 34: //값 잘 들어갓나 한번 확인하고
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770", "Device typical data");
                    if(GetWindowText(m_iHwnd1,Tools.Edit, 1) != OM.DressyInfo.s1) sErrMsg = "ERR000_Brand ID 셋팅 실패";
                    if(GetWindowText(m_iHwnd1,Tools.Edit, 2) != OM.DressyInfo.s2) sErrMsg = "ERR000_Model Name 셋팅 실패";
                    if(GetWindowText(m_iHwnd1,Tools.Edit, 3) != OM.DressyInfo.s3) sErrMsg = "ERR000_PartNumber 셋팅 실패";
                    if(GetWindowText(m_iHwnd1,Tools.Edit, 4) != OM.DressyInfo.s4) sErrMsg = "ERR000_Manufacturer Name 셋팅 실패";

                    if(GetWindowText(m_iHwnd1,Tools.Edit, 5) != sDateFull) sErrMsg = "ERR000_Date of Manufacture 셋팅 실패";
                    if(GetWindowText(m_iHwnd1,Tools.Edit, 6) != sDateFull) sErrMsg = "ERR000_Date of last Calibration 셋팅 실패";                      

                    if(GetWindowText(m_iHwnd1,Tools.Edit,11) != OM.DressyInfo.s5) sErrMsg = "ERR000_H/W Version 셋팅 실패";
                    if(GetWindowText(m_iHwnd1,Tools.Edit,12) != OM.DressyInfo.s6) sErrMsg = "ERR000_Calibration reference site location 셋팅 실패";
                    if(GetWindowText(m_iHwnd1,Tools.Edit,13) != OM.DressyInfo.s7) sErrMsg = "ERR000_Calibration reference system 셋팅 실패";
                    if (sErrMsg != "") return true;

                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "Close");
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    Click(m_iHwnd2);

                    //1.0.1.5
                    //Calibration에서 사용하는 놈들 클리어
                    //CalCrntImgFile
                    //CalBfImgFile
                    //iCalTypeCnt
                    //bRework 
                    //CycleReady에서 초기화함.
                    CalDataClear();
                    m_iCycle = 0;
                    return true;
            }
        }

        /// <summary>
        /// Configuration 총3번 처음에 한번 중간에 바인딩바꿀대 한번 마지막에 마진 넣을때 한번.
        /// 처음에 태우는건 Cycle1에서 하고 있음
        /// 중간에 바인딩 바꿀때랑 마지막에 마진 넣을때만 쓸것
        /// Setting.sBind
        /// Setting.sGain  
        /// Setting.sAreaUp
        /// Setting.sAreaLt
        /// Setting.sAreaRt
        /// Setting.sAreaDn
        /// </summary>
        /// <returns></returns>
        public bool Cycle2() //Configuration
        {
            String sTemp;
            if (m_tmCycle.OnDelay(m_iCycle != 0 && m_iCycle == m_iPreCycle, 20000))
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                if (sErrMsg == "") sErrMsg = "ERR100_"+m_sPartName + "Macro Time Out Error " + sTemp;
                Trace(sErrMsg);
                m_iCycle = 0;
                return true;
            }

            if (m_iCycle != m_iPreCycle)
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                Trace(sTemp);
                m_tmDelay.Clear();
            }

            m_iHwnd1 = IntPtr.Zero;
            m_iHwnd2 = IntPtr.Zero;
            m_iHwnd3 = IntPtr.Zero;
            m_iHwnd4 = IntPtr.Zero;

            m_iPreCycle = m_iCycle;

            int iDly = 1000;

            switch (m_iCycle)
            {

                default:
                    sTemp = string.Format("Cycle_Default_Clear_m_iCycle={0:000}", m_iCycle);
                    sErrMsg = sTemp;
                    return true;


                case 0:
                    return true;

                case 10: //
                    InitStart();
                    m_iCycle++;
                    return false;

                case 11:
                    if (!Cycle0()) return false;
                    m_tmDelay.Clear();
                    m_iCycle = 20;
                    return false;

                case 20: //Configuration Click
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "#32770", null);
                    m_iHwnd3 = FindWindowChild(m_iHwnd2, IntPtr.Zero, "Button", "Configuration");
                    if (m_iHwnd3 == IntPtr.Zero) return false;
                    Click(m_iHwnd3);
                    m_iCycle++;
                    return false;

                case 21: //Configuration Setting
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770", "Device configuration");
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "ComboBox", 0);
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    if (CConfig.StrToIntDef(Setting.sBind, 1) == 1) SetComboBoxIndex(m_iHwnd2, 0); //PostMessage로 셋팅 안바껴서 이걸로 바꿈 진섭.
                    else                                            SetComboBoxIndex(m_iHwnd2, 1); //PostMessage로 셋팅 안바껴서 이걸로 바꿈 진섭.

                    //20180724 제어프로그램에서 관여하지 않도록 요청 
                    //SetWindowText(m_iHwnd1,Tools.Edit, 4,Setting.sGain  ); //4번째가 Gain Level
                    SetWindowText(m_iHwnd1,Tools.Edit, 7,Setting.sAreaUp); //
                    SetWindowText(m_iHwnd1,Tools.Edit, 8,Setting.sAreaLt); //
                    SetWindowText(m_iHwnd1,Tools.Edit, 9,Setting.sAreaRt); //
                    SetWindowText(m_iHwnd1,Tools.Edit,10,Setting.sAreaDn); //
                    m_iCycle++;
                    return false;

                case 22: //Apply
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770", "Device configuration");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "Apply");
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    Click(m_iHwnd2);
                    m_iCycle++;
                    return false;

                case 23: //Close
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770", "Device configuration");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "Close");
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    Click(m_iHwnd2);
                    m_iCycle++;
                    return false;

                case 24: //Configuration
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "#32770", null);
                    m_iHwnd3 = FindWindowChild(m_iHwnd2, IntPtr.Zero, "Button", "Configuration");
                    if (m_iHwnd3 == IntPtr.Zero) return false;
                    Click(m_iHwnd3);
                    m_iCycle++;
                    return false;

                case 25: //값 잘 들어갓나 한번 확인하고
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770", "Device configuration");
                    if(CConfig.StrToIntDef(Setting.sBind,1) == 1)
                    {
                        if(GetWindowText(m_iHwnd1, Tools.ComboBox, 0) != "1x1") { sErrMsg = "ERR000_Binning Mode 셋팅 실패"; return true; }
                    }
                    else
                    {
                        if (GetWindowText(m_iHwnd1, Tools.ComboBox, 0) != "2x2") { sErrMsg = "ERR000_Binning Mode 셋팅 실패"; return true; }
                    }
                    //if (GetWindowText(m_iHwnd1, Tools.Edit, 4 ) != Setting.sGain  ) { sErrMsg = "ERR000_Gain Level 셋팅 실패"; return true; }
                    if (GetWindowText(m_iHwnd1, Tools.Edit, 7 ) != Setting.sAreaUp) { sErrMsg = "ERR000_Area Up 셋팅 실패"   ; return true; }
                    if (GetWindowText(m_iHwnd1, Tools.Edit, 8 ) != Setting.sAreaLt) { sErrMsg = "ERR000_Area Lt 셋팅 실패"   ; return true; }
                    if (GetWindowText(m_iHwnd1, Tools.Edit, 9 ) != Setting.sAreaRt) { sErrMsg = "ERR000_Area Rt 셋팅 실패"   ; return true; }
                    if (GetWindowText(m_iHwnd1, Tools.Edit, 10) != Setting.sAreaDn) { sErrMsg = "ERR000_Area Dn 셋팅 실패"   ; return true; }

                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "Close");
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    Click(m_iHwnd2);
                    m_iCycle++;
                    return false;

                case 26: //Typical Data Click (Device typical data)
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iCycle = 0;
                    return true;
            }
        }

        /// <summary>
        /// Tigger
        /// Setting.sPath1 - Dress exe file path
        /// Setting.sFileName1 - offset
        /// Setting.sFileName2 - Trigger
        /// </summary>
        /// <returns></returns>
        public bool Cycle3() //Tigger
        {
            String sTemp;
            if (m_tmCycle.OnDelay(m_iCycle != 0 && m_iCycle == m_iPreCycle && !OM.MstOptn.bDebugMode, 20000))
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                if (sErrMsg == "") sErrMsg = "ERR100_"+m_sPartName + "Macro Time Out Error " + sTemp;
                Trace(sErrMsg);
                m_iCycle = 0;
                return true;
            }

            if (m_iCycle != m_iPreCycle)
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                Trace(sTemp);
                m_tmDelay.Clear();
            }

            m_iHwnd1 = IntPtr.Zero;
            m_iHwnd2 = IntPtr.Zero;
            m_iHwnd3 = IntPtr.Zero;
            m_iHwnd4 = IntPtr.Zero;

            string sPath = "";
            string sName = "";
            string CopySerial = "";

            //Tab Order
            int Dev = 0;
            int Acq = 1;
            int Cal = 2;
            int Man = 3;

            m_iPreCycle = m_iCycle;

            int iDly = 1500;

            switch (m_iCycle)
            {

                default:
                    sTemp = string.Format("Cycle_Default_Clear_m_iCycle={0:000}", m_iCycle);
                    sErrMsg = sTemp;
                    return true;


                case 0:
                    return true;

                case 10: //프로그램 실행되어 있는지 확인하고
                    InitStart();
                    m_iCycle++;
                    return false;

                case 11: //프로그램 실행하기
                    if (!Cycle0()) return false;
                    m_iCycle = 20;
                    return false;

                case 20: //겟 다크 클릭
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", Cal);
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    SetTabControl(m_iHwnd1, Cal);
                    m_iHwnd3 = FindWindowChild(m_iHwnd2, IntPtr.Zero, "Button", "Get Dark");
                    if (m_iHwnd3 == IntPtr.Zero) return false;
                    Click(m_iHwnd3);
                    m_iCycle++;
                    return false;

                case 21://트리거 파일 체크하고 겟 브라이트 클릭
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770", null);
                    if (IsWindowVisible(m_iHwnd1) == 1) return false;

                    sPath = OM.DressyInfo.sCalPath; //실행파일 경로랑 Cal File 저장하는 경로가 다를수도 있어서 따로 분리함 진섭
                    sName = OM.DressyInfo.sCalPath + "\\Cal\\offset.raw";
                    if (!File.Exists(sName)) //트리거 불량
                    {
                        sErrMsg = "ERR004_Offset.raw 파일 생성 실패(Get Dark)";
                        return true;
                    }

                    DirectoryInfo di1 = new DirectoryInfo(sPath+"\\Cal\\"+sSerial);
                    if(!di1.Exists) di1.Create();

                    if (!File.Exists(sPath + "\\Cal\\" + sSerial + "\\" + "offset.raw"))
                    {
                        File.Copy(sName, sPath + "\\Cal\\" + sSerial + "\\" + "offset.raw"); //offset 파일
                    }
                    else
                    {
                        File.Delete(sPath + "\\Cal\\" + sSerial + "\\" + "offset.raw");
                        File.Copy(sName, sPath + "\\Cal\\" + sSerial + "\\" + "offset.raw"); //offset 파일
                    }
                    CopySerial = sSerial;
                    m_iCycle = 30;
                    return false;

                case 30: //ENP_Inspection프로그램 실행하기
                    if (!ExitProcess(OM.DressyInfo.sAppName3)) return false; //
                    InitProcess2(OM.DressyInfo.sAppPath3);

                    m_tmDelay.Clear();
                    CopySerial = sSerial;
                    m_iCycle++;
                    return false;

                case 31: //
                    if (!m_tmDelay.OnDelay(7000)) return false;
                    m_iHwndS = FindWindowL("#32770", "Noise Evaluation for Factory [ver 1.0]");
                    if (!ExistProces(OM.DressyInfo.sAppName3) || m_iHwndS == IntPtr.Zero)
                    {
                        sErrMsg = "ERR002_프로그램이 실행되지 않았습니다.";
                        return true;
                    }
                    SetWindowPos(m_iHwndS, IntPtr.Zero, OM.DressyInfo.iPosX3, OM.DressyInfo.iPosY3, 0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);

                    m_iCycle = 40;
                    return false;

                case 40://
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "#32770", "ElectronicNoisePerformance");
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    SetWindowText(m_iHwnd1, Tools.Edit, 1, OM.DressyInfo.sC);
                    SetWindowText(m_iHwnd1, Tools.Edit, 2, OM.DressyInfo.sD);

                    m_iCycle++;
                    return false;

                case 41:
                    if (!m_tmDelay.OnDelay(true, iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "#32770", "ElectronicNoisePerformance");
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    sPath = OM.DressyInfo.sCalPath;
      
                    SetWindowText(m_iHwnd1, Tools.Edit, 7, sPath + "\\Cal\\" + sSerial);
                    m_iCycle++;
                    return false;

                case 42:
                    if (!m_tmDelay.OnDelay(true, iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "#32770", "ElectronicNoisePerformance");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "RUN");
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    Click(m_iHwnd2);
                    m_iCycle++;
                    return false;

                case 43:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770", "ENP_Inspection");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Static", "확인");
                    if (m_tmDelay.OnDelay(GetWindowText(m_iHwnd1, Tools.Static, 0).IndexOf("Complete") == -1, 12000))
                    {
                        sErrMsg = "ERR008_ENP_Inspection 프로그램에서 결과데이터를 가져오지 못했습니다.";
                        Trace(sErrMsg);
                        return true;
                    }
                    if (GetWindowText(m_iHwnd1, Tools.Static, 0).IndexOf("Complete") == -1) return false;
                    m_iCycle++;
                    return false;

                case 44:
                    if (!m_tmDelay.OnDelay(true, iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770", "ENP_Inspection");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "확인");
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    Click(m_iHwnd2);
                    m_iCycle++;
                    return false;

                case 45: //확인 취소창이 없어졋는지 확인.
                    if (!m_tmDelay.OnDelay(true, iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770", "ENP_Inspection");
                    if (m_iHwnd1 != IntPtr.Zero) return false;
                    m_iCycle++;
                    return false;

                case 46:
                    if (!m_tmDelay.OnDelay(true, iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "#32770", "ElectronicNoisePerformance");
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    Result.Offset = "";
                    Result.Offset = GetWindowText(m_iHwnd1, Tools.Edit, 13); 
                    m_iCycle++;
                    return false;

                case 47:
                    if (!m_tmDelay.OnDelay(true, iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "Button", "취소");
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    Click(m_iHwnd1);
                    m_iCycle = 50;
                    return false;

                case 50:
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", Cal);
                    m_iHwnd3 = FindWindowChild(m_iHwnd2, IntPtr.Zero, "Button", "Get Bright");
                    if (m_iHwnd3 == IntPtr.Zero) return false;
                    Click(m_iHwnd3);
                    m_iCycle++;
                    return false;

                case 51: //X ray 조사
                    if (!m_tmDelay.OnDelay(OM.DressyInfo.iBfXrayDelay)) return false;
                    m_iHwnd1 = FindWindowL("#32770", null);
                    if (IsWindowVisible(m_iHwnd1) == 0) return false;
                    //엑스레이 조사
                    if (OM.MstOptn.bUseSwTrg)
                    {
                        m_iHwnd3 = FindWindowChild(m_iHwndM, IntPtr.Zero, "Button", "S/W Trigger");
                        if (m_iHwnd3 == IntPtr.Zero) return false;
                        Click(m_iHwnd3);
                    }
                    else
                    {
                        SEQ.XrayCom.SetXrayPara(Setting.sKvp, Setting.smA, Setting.sTime);
                    }
                    
                    dInspDely = 5000;
                    //SEQ.Xray.Run();
                    m_iCycle++;
                    return false;

                case 52: //
                    if (!m_tmDelay.OnDelay((int)dInspDely)) return false;
                    m_iHwnd1 = FindWindowL("#32770", null);
                    if (IsWindowVisible(m_iHwnd1) == 1)
                    {
                        if (iRptCnt == OM.CmnOptn.iXrayRptCnt)
                        {
                            iRptCnt = 0;
                            //bErrDevice = true;
                            sErrMsg = "트리거 작업이 실패했습니다.";
                            m_iCycle = 0;
                            return true;
                        }
                        if (OM.CmnOptn.iXrayRptCnt > 0)
                        {
                            iRptCnt++;
                            m_tmDelay.Clear();
                            m_iCycle = 51;
                            return false;
                        }
                        else if (OM.CmnOptn.iXrayRptCnt <= 0)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        //bErrDevice = false;
                        iRptCnt = 0;
                    }
                    sName = "" ;
                    if (Directory.Exists(OM.DressyInfo.sCalPath + "\\Cal"))
                    {
                        DirectoryInfo di2 = new DirectoryInfo(OM.DressyInfo.sCalPath + "\\Cal");
                        
                        foreach (var item in di2.GetFiles())
                        {
                            if (item.Name.IndexOf("X") == 0)
                            {
                                sName = item.Name;
                            }
                        }
                    }
                    sPath = OM.DressyInfo.sCalPath;
                    sName = sPath + "\\Cal\\" + sName ;

                    if (!File.Exists(sName)) //트리거 불량
                    {
                        using (File.Create(sName + "\\" + sSerial + "\\Trigger_Fail")) ;
                        sErrMsg = "ERR005_Trigger_Fail";
                        return true;
                    }
                    else
                    {
                        if (File.Exists(sPath + "\\Cal\\" + sSerial + "\\" + @Setting.sFileName1))
                        {
                            File.Delete(sPath + "\\Cal\\" + sSerial + "\\" + @Setting.sFileName1);
                        }
                        File.Copy(sName, sPath + "\\Cal\\" + sSerial + "\\" + @Setting.sFileName1);
                        File.Delete(sName);
                    }

                    m_iCycle = 0;
                    return true;
            }
        }

        /// <summary>
        /// Calibration
        /// Setting.sKvp
        /// Setting.smA
        /// Setting.sTime
        /// Cal 이미지 받고 이상없으면 다음 스텝으로 이동
        /// 최대값-최소값 편차가 허용 오차 이상 발생하면 해당 파일 생성한 작업 리스트로 가서 삭제하고
        /// 다시 촬영하고 편차 확인 해서 OK 떨어지면 저장된 스텝으로 이동
        /// </summary>
        /// <returns></returns>
        public bool Cycle4() //Calibration
        {
            String sTemp;
            if (m_tmCycle.OnDelay(m_iCycle != 0 && m_iCycle == m_iPreCycle, 30000))
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                if (sErrMsg == "") sErrMsg = "ERR100_" + m_sPartName + "Macro Time Out Error " + sTemp;
                Trace(sErrMsg);
                m_iCycle = 0;
                return true;
            }

            if (m_iCycle != m_iPreCycle)
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                Trace(sTemp);
                m_tmDelay.Clear();
            }

            m_iHwnd1 = IntPtr.Zero;
            m_iHwnd2 = IntPtr.Zero;
            m_iHwnd3 = IntPtr.Zero;
            m_iHwnd4 = IntPtr.Zero;

            string sClassName1 = "#32770";
            string sPath = "";
            string sName = "";

            //Tab Order
            int Dev = 0;
            int Acq = 1;
            int Cal = 2;
            int Man = 3;

            m_iPreCycle = m_iCycle;

            int iDly = 1000;

            switch (m_iCycle)
            {
                default:
                    sTemp = string.Format("Cycle_Default_Clear_m_iCycle={0:000}", m_iCycle);
                    sErrMsg = sTemp;
                    return true;

                case 0:
                    return true;

                case 10: //프로그램 실행되어 있는지 확인하고
                    bPass = false;
                    InitStart();
                    m_iCycle++;
                    return false;

                case 11: //프로그램 실행하기
                    if (!Cycle0()) return false;
                    m_iCycle++;
                    return false;
                
                case 12: //Calibration 분기
                    if (iCalTypeCnt != 0 && CalCrntImgFile.Count == iCalTypeCnt)//편차 계산 시작이 true 이면 50번으로.
                    {
                        m_iCycle = 50;
                        return false;
                    }

                    if (OM.DressyInfo.iSelGetDarkBtn == 1) //Aging 버튼 사용시에는 Get Dark Skip
                    {
                        m_iCycle = 30;
                        return false;
                    }

                    m_iCycle = 20;
                    return false;

                //Get Dark 버튼 사용 사이클
                case 20: //겟 다크 클릭
                    sPath = Path.GetDirectoryName(OM.DressyInfo.sCalPath);
                    sName = Path.GetDirectoryName(OM.DressyInfo.sCalPath) + "\\Cal\\offset.raw";
                    if (File.Exists(sName))
                    {
                        m_iCycle = 30;
                        return false;
                    }
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", Cal);
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    SetTabControl(m_iHwnd1, Cal);
                    m_iHwnd3 = FindWindowChild(m_iHwnd2, IntPtr.Zero, "Button", "Get Dark");
                    if (m_iHwnd3 == IntPtr.Zero) return false;

                    Click(m_iHwnd3);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 21://트리거 파일 체크하고 겟 브라이트 클릭
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770", null);
                    if (IsWindowVisible(m_iHwnd1) == 1) return false;

                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 22:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    sPath = OM.DressyInfo.sCalPath;
                    sName = OM.DressyInfo.sCalPath + "\\Cal\\offset.raw";
                    if (!File.Exists(sName)) //트리거 불량
                    {
                        sErrMsg = "ERR004_Offset.raw 파일 생성 실패(Get Dark)";
                        return true;
                    }
                    m_iCycle = 30;
                    return false;

                case 30: //X ray 조사
                    sName = OM.DressyInfo.sCalPath + "\\Cal\\offset.raw";
                    if (!File.Exists(sName)) //트리거 불량
                    {
                        sErrMsg = "ERR004_Offset.raw 파일 생성 실패(Aging)";
                        return true;
                    }
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", Cal);
                    m_iHwnd3 = FindWindowChild(m_iHwnd2, IntPtr.Zero, "Button", "Get Bright");
                    if (m_iHwnd3 == IntPtr.Zero) return false;
                    Click(m_iHwnd3);
                    m_iCycle++;
                    return false;

                case 31: //X ray 조사
                    if (!m_tmDelay.OnDelay(OM.DressyInfo.iBfXrayDelay)) return false;
                    m_iHwnd1 = FindWindowL("#32770", null);
                    if (IsWindowVisible(m_iHwnd1) == 0) return false;
                    //엑스레이 조사
                    if (OM.MstOptn.bUseSwTrg)
                    {
                        m_iHwnd3 = FindWindowChild(m_iHwndM, IntPtr.Zero, "Button", "S/W Trigger");
                        if (m_iHwnd3 == IntPtr.Zero) return false;
                        Click(m_iHwnd3);
                    }
                    else
                    {
                        SEQ.XrayCom.SetXrayPara(Setting.sKvp, Setting.smA, Setting.sTime);
                    }

                    dInspDely = 5000;
                    m_iCycle++;
                    return false;

                case 32: //
                    if (!m_tmDelay.OnDelay((int)dInspDely)) return false;
                    m_iHwnd1 = FindWindowL("#32770", null);
                    if (IsWindowVisible(m_iHwnd1) == 1)
                    {
                        if (iRptCnt == OM.CmnOptn.iXrayRptCnt)
                        {
                            iRptCnt = 0;
                            sErrMsg = "Calibration 이미지 파일 생성에 실패했습니다.";
                            m_iCycle = 0;
                            return true;
                        }
                        if (OM.CmnOptn.iXrayRptCnt > 0)
                        {
                            iRptCnt++;
                            m_tmDelay.Clear();
                            m_iCycle = 31;
                            return false;
                        }
                        else if (OM.CmnOptn.iXrayRptCnt <= 0)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        //bErrDevice = false;
                        iRptCnt = 0;
                    }

                    m_iCycle++;
                    return false;

                case 33: //현재 작업한 Cal 이미지 파일 명을 리스트에 넣는다.
                    if (Directory.Exists(OM.DressyInfo.sCalPath + "\\Cal"))
                    {
                        DirectoryInfo di = new DirectoryInfo(OM.DressyInfo.sCalPath + "\\Cal");

                        foreach (var item in di.GetFiles())
                        {
                            if (item.Name.IndexOf("X") == 0 && !CalBfImgFile.Contains(item.Name) &&
                                !CalCrntImgFile.Contains(item.Name))
                            {
                                CalCrntImgFile.Add(item.Name);
                            }
                        }
                    }
                    if(iCalTypeCnt == 0 && OM.Dressy[SEQ.XRYD.iWorkStep + 1].sType != "2")//지금 Cal 받는 공정의 다음 공정이 Cal이 아니면 
                    {
                        iCalTypeCnt = CalCrntImgFile.Count; //Cal 공정 몇개있는지 체크
                    }

                    m_iCycle = 0;
                    return true;

                //편차 계산해서 오차범위 확인하고 이상있으면 재촬영, 이상 없으면 다음 리스트 작업
                case 50:
                    string sCalPath = OM.DressyInfo.sCalPath + "\\CAL";
                    int iMax = 0;
                    int iMin = 0;
                    int iAvr = 0;
                    int iSum = 0;
                    int ideviation = 0;
                    int iCrntCalNo = 0;

                    int[] iTemp = new int[CalCrntImgFile.Count];
                    for (int i = 0; i < CalCrntImgFile.Count; i++) //최대값을 구한다.
                    {
                        iTemp[i] = CConfig.StrToIntDef(CalCrntImgFile[i].Substring(1, 4), 0);

                        if (iTemp[i] > iMax) iMax = iTemp[i];
                    }

                    iMin = iMax;
                    for (int i = 0; i < CalCrntImgFile.Count; i++) //최소값을 구한다.
                    {
                        if (iTemp[i] < iMin) iMin = iTemp[i];
                    }


                    for (int i = 0; i < CalCrntImgFile.Count; i++)
                    {
                        iSum = iSum + iTemp[i];
                        iAvr = iSum / CalCrntImgFile.Count;
                    }

                    ideviation = iMax - iMin;
                    
                    string sDelFile = "";

                    int iAvrDev1 = Math.Abs(iMax - iAvr);
                    int iAvrDev2 = Math.Abs(iMin - iAvr);
                    if (ideviation <= OM.DressyInfo.iTolerance)
                    {
                        m_iCycle = 60;
                        return false;
                    }

                    else if (ideviation > OM.DressyInfo.iTolerance)
                    {
                        if (Directory.Exists(sCalPath))
                        {
                            if (iAvrDev1 < iAvrDev2 || iAvrDev2 == iAvrDev1)
                            {
                                DirectoryInfo di = new DirectoryInfo(sCalPath);
                                foreach (var item in di.GetFiles())
                                {
                                    if (item.Name.IndexOf("X" + string.Format("{0:0000}", iMin) + ".raw") == 0)
                                    {
                                        sDelFile = item.Name;
                                    }
                                }
                                FileInfo[] files = di.GetFiles(sDelFile, SearchOption.AllDirectories);

                                foreach (FileInfo file in files)
                                {
                                    file.Attributes = FileAttributes.Normal;
                                    file.Delete();
                                }
                                CalCrntImgFile.Remove("X" + string.Format("{0:0000}", iMin) + ".raw");
                            }
                            else if (iAvrDev1 > iAvrDev2)
                            {
                                DirectoryInfo di = new DirectoryInfo(sCalPath);
                                foreach (var item in di.GetFiles())
                                {
                                    if (item.Name.IndexOf("X" + string.Format("{0:0000}", iMax) + ".raw") == 0)
                                    {
                                        sDelFile = item.Name;
                                    }
                                }
                                FileInfo[] files = di.GetFiles(sDelFile, SearchOption.AllDirectories);

                                foreach (FileInfo file in files)
                                {
                                    file.Attributes = FileAttributes.Normal;
                                    file.Delete();
                                }
                                CalCrntImgFile.Remove("X" + string.Format("{0:0000}", iMax) + ".raw");
                            }
                        }

                        for (int j = SEQ.XRYD.iWorkStep; j > 0; j--)
                        {
                            if (OM.Dressy[j - 1].sType != "2") continue; //리스트 인덱스 넘버랑 iCrntCalNo 숫자 맞추기 위해 WorkStep - 1한다.
                            iCrntCalNo++; //현재 위치에서 위로 Cal 공정 몇개있는지 체크
                        }
                    }
                    bRework = true;
                    iCalRptCnt++; //Cal 편차 확인 작업 몇번 했는 지 카운트 한다.


                    m_tmDelay.Clear();
                    m_iCycle ++;
                    return false;

                case 51:
                    if (!m_tmDelay.OnDelay(OM.DressyInfo.iCalDeleteDelay)) return false;
                    if(OM.DressyInfo.iCalRptCnt > 0 && iCalRptCnt >= OM.DressyInfo.iCalRptCnt)
                    {
                        iCalRptCnt = 0;
                        sErrMsg = "Calibration 편차 반복 횟수를 초과하였습니다.";
                    }

                    m_iCycle = 0;
                    return true;

                case 60:
                    sCalPath = OM.DressyInfo.sCalPath + "\\CAL";
                    if (System.IO.Directory.Exists(sCalPath))
                    {
                        System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(sCalPath);
                        foreach (var item in di.GetFiles())
                        {
                            if (item.Name.IndexOf("X") == 0 && !CalBfImgFile.Contains(item.Name)) CalBfImgFile.Add(item.Name);
                        }
                    }
                    
                    iCalTypeCnt = 0;
                    CalCrntImgFile.Clear();
                    bPass = true;
                    bRework = false;
                    m_iCycle = 0;
                    return true;

            }
        }

        /// <summary>
        /// Calibration Generate Click (Gain0x_00xx.raw)
        /// Setting.sPath1
        /// </summary>
        /// <returns></returns>
        public bool Cycle5() //Calibration Generate Click (Gain00_00xx.raw) 
        {
            String sTemp;
            if (m_tmCycle.OnDelay(m_iCycle != 0 && m_iCycle == m_iPreCycle, 20000))
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                if (sErrMsg == "") sErrMsg = "ERR100_"+m_sPartName + "Macro Time Out Error " + sTemp;
                Trace(sErrMsg);
                m_iCycle = 0;
                return true;
            }

            if (m_iCycle != m_iPreCycle)
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                Trace(sTemp);
                m_tmDelay.Clear();
            }

            m_iHwnd1 = IntPtr.Zero;
            m_iHwnd2 = IntPtr.Zero;
            m_iHwnd3 = IntPtr.Zero;
            m_iHwnd4 = IntPtr.Zero;

            string sClassName1 = "#32770";
            string sPath       = "";
            string sPath1      = "";
            string sPath2      = "";
            string sPath3      = "";
            string sName       = "";
            string sName1      = "";
            string sName2      = "";

            //Tab Order
            int Dev = 0;
            int Acq = 1;
            int Cal = 2;
            int Man = 3;

            m_iPreCycle = m_iCycle;

            int iDly = 200;

            switch (m_iCycle)
            {

                default:
                    sTemp = string.Format("Cycle_Default_Clear_m_iCycle={0:000}", m_iCycle);
                    sErrMsg = sTemp;
                    return true;


                case 0:
                    return true;
                    
                case 10: //프로그램 실행되어 있는지 확인하고
                    //Cal 공정에서 사용하는 리스트인데
                    //Cal 공정이 전부 완료되면 클리어해야해서
                    //여기서 클리어함.
                    CalBfImgFile.Clear();
                    InitStart();
                    m_iCycle++;
                    return false;

                case 11: //프로그램 실행하기
                    if (!Cycle0()) return false;
                    m_iCycle = 20;
                    return false;

                case 20: //제너레이트 버튼 클릭
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", Cal);
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    SetTabControl(m_iHwnd1,Cal);
                    m_iHwnd3 = FindWindowChild(m_iHwnd2, IntPtr.Zero, "Button", "Generate");
                    if (m_iHwnd3 == IntPtr.Zero) return false;
                    Click(m_iHwnd3);
                    if (OM.MstOptn.bUseSwTrg)
                    {
                        //S/W 트리거 할때 조건 다르게 파일 생성이 안되서 가라로 파일 하나 만듬 진섭
                        File.Create(OM.DressyInfo.sCalPath + "\\Cal\\gain01_00000.raw");
                    }
                    m_iCycle++;
                    return false;

                case 21:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    string sSourceFilePath;
                    string sDestFilePath;
                    List<string> FileList = new List<string>();
                    string sCalPath = OM.DressyInfo.sCalPath + "\\Cal";
                    if (Directory.Exists(sCalPath))
                    {
                        DirectoryInfo di = new DirectoryInfo(sCalPath);
                        foreach (var item in di.GetFiles())
                        {
                            if (item.Name.IndexOf("gain") == 0)
                            {
                                FileList.Add(item.Name);
                            }
                        }
                    }

                    sPath1 = OM.DressyInfo.sCalPath + "\\Cal\\offset.raw";

                    if (FileList.Count == 0) { sErrMsg = "ERR006_Gain0X_XXXXX.raw 파일 생성 실패"; return true; } //리스트에 아무것도 없으면 파일 못찾은거니까 에러.
                    if (!File.Exists(sPath1)) { sErrMsg = "ERR006_Offset.raw 파일 생성 실패"; return true; }

                    string[] aFileName = new string[FileList.Count];

                    for(int i = 0; i< FileList.Count; i++)
                    {
                        aFileName[i] = FileList[i].ToString();
                    }

                    sPath = OM.DressyInfo.sCalPath;
                    
                    DirectoryInfo di1 = new DirectoryInfo(sPath+"\\Cal\\"+sSerial);
                    if(!di1.Exists) di1.Create();

                    if (!OM.MstOptn.bUseSwTrg)
                    {
                        for (int i = 0; i < FileList.Count; i++)
                        {
                            sSourceFilePath = OM.DressyInfo.sCalPath + "\\Cal\\" + FileList[i].ToString();
                            sDestFilePath   = sPath + "\\Cal\\" + sSerial + "\\" + FileList[i].ToString();
                            if (!File.Exists(sPath + "\\Cal\\" + sSerial + "\\" + FileList[i].ToString()))
                            {
                                File.Copy(sSourceFilePath, sDestFilePath);
                            }
                            File.Delete(sSourceFilePath);
                        }
                       
                        if (!File.Exists(sPath + "\\Cal\\" + sSerial + "\\offset.raw"))
                        {
                            File.Copy(sPath1, sPath + "\\Cal\\" + sSerial + "\\offset.raw");
                        }
                    }
                    
                    //Calibration 파일 받은거 지우는거
                    if (System.IO.Directory.Exists(sCalPath))
                    {
                        System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(sCalPath);
                        foreach (var item in di.GetFiles())
                        {
                            if (item.Name.IndexOf("X") >= 0) 
                            {
                                sName1 = item.Name;
                                File.Delete(sCalPath + "\\" + sName1);
                            }
                        }
                    }

                    
                    //File.Delete(sPath3);

                    m_iCycle = 0;
                    return true;
            }
        }

        /// <summary>
        /// Acquisition 4 Point
        /// Setting.iAcq1
        /// Setting.iAcq2
        /// Setting.iAcq3
        /// Setting.iAcq4
        /// Setting.iAcq5
        /// Setting.iAcq6
        /// Setting.iAcq7
        /// Setting.sFileName1
        /// Setting.sKvp
        /// Setting.smA
        /// Setting.sTime
        /// </summary>
        /// <returns></returns>
        public bool Cycle6() //Acquisition 4 Point Brnu_Raw
        {
            String sTemp;
            if (m_tmCycle.OnDelay(m_iCycle != 0 && m_iCycle == m_iPreCycle, 30000))
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                if (sErrMsg == "") sErrMsg = "ERR100_"+m_sPartName + "Macro Time Out Error " + sTemp;
                Trace(sErrMsg);
                m_iCycle = 0;
                return true;
            }

            if (m_iCycle != m_iPreCycle)
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                Trace(sTemp);
                m_tmDelay.Clear();
            }

            m_iHwnd1 = IntPtr.Zero;
            m_iHwnd2 = IntPtr.Zero;
            m_iHwnd3 = IntPtr.Zero;
            m_iHwnd4 = IntPtr.Zero;

            string sClassName1 = "#32770";
            string sPath       = "";
            string sName       = "";

            //Tab Order
            int Dev = 0;
            int Acq = 1;
            int Cal = 2;
            int Man = 3;

            int i1,i2;
            m_iPreCycle = m_iCycle;

            int iDly = 700;

            string sFileNameRaw = Path.GetFileName(@Setting.sFileName1);
            int iLength = sFileNameRaw.Length;
            string sFileName = sFileNameRaw.Substring(0, iLength - 4);

            switch (m_iCycle)
            {

                default:
                    sTemp = string.Format("Cycle_Default_Clear_m_iCycle={0:000}", m_iCycle);
                    sErrMsg = sTemp;
                    return true;


                case 0:
                    return true;
                    
                case 10: //프로그램 실행되어 있는지 확인하고
                    InitStart();
                    m_iCycle++;
                    return false;

                case 11: //프로그램 실행하기
                    if (!Cycle0()) return false;
                    m_iCycle = 20;
                    return false;

                case 20: //Cal Options Setting
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", Acq);
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    SetTabControl(m_iHwnd1,Acq);

                    SetWindow(m_iHwnd2, Tools.Button, 3, Setting.iAcq1);
                    SetWindow(m_iHwnd2, Tools.Button, 4, Setting.iAcq2);
                    SetWindow(m_iHwnd2, Tools.Button, 5, Setting.iAcq3);
                    SetWindow(m_iHwnd2, Tools.Button, 6, Setting.iAcq4);
                    SetWindow(m_iHwnd2, Tools.Button, 7, Setting.iAcq5);
                    SetWindow(m_iHwnd2, Tools.Button, 10, Setting.iAcq6);
                    SetWindow(m_iHwnd2, Tools.Button, 8, Setting.iAcq7);

                    SetWindowText(m_iHwnd2, Tools.Edit, 4, sFileName);

                    m_iCycle++;
                    return false;

                case 21://Cal Options Check
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", Acq);
                    if (m_iHwnd2 == IntPtr.Zero) return false;

                    if (GetWindow(m_iHwnd2, Tools.Button, 3 ) != Setting.iAcq1) { sErrMsg = "ERR000_offset Calibration 셋팅 실패"    ; return true;}
                    if (GetWindow(m_iHwnd2, Tools.Button, 4 ) != Setting.iAcq2) { sErrMsg = "ERR000_Gain Calibration 셋팅 실패"      ; return true; }
                    if (GetWindow(m_iHwnd2, Tools.Button, 5 ) != Setting.iAcq3) { sErrMsg = "ERR000_Bpm Correction 셋팅 실패"        ; return true; }
                    if (GetWindow(m_iHwnd2, Tools.Button, 6 ) != Setting.iAcq4) { sErrMsg = "ERR000_Direct Hit Filtering 셋팅 실패"  ; return true; }
                    if (GetWindow(m_iHwnd2, Tools.Button, 7 ) != Setting.iAcq5) { sErrMsg = "ERR000_Gamma Correction 셋팅 실패"      ; return true; }
                    if (GetWindow(m_iHwnd2, Tools.Button, 10) != Setting.iAcq6) { sErrMsg = "ERR000_Char. Curve 셋팅 실패"           ; return true; }
                    if (GetWindow(m_iHwnd2, Tools.Button, 8 ) != Setting.iAcq7) { sErrMsg = "ERR000_Continuous acquisition 셋팅 실패"; return true; }

                    if (GetWindowText(m_iHwnd2, Tools.Edit, 4) != sFileName) { sErrMsg = "ERR000_File Name 셋팅 실패"; return true; }

                    m_iHwnd3 = FindWindowChild(m_iHwnd2, IntPtr.Zero, "Button", "Get Image");

                    if (m_iHwnd3 == IntPtr.Zero) return false;
                    Click(m_iHwnd3);
                    m_iCycle++;
                    return false;

                case 22: //X ray 조사
                    if (!m_tmDelay.OnDelay(OM.DressyInfo.iBfXrayDelay)) return false;
                    m_iHwnd1 = FindWindowL("#32770",null);
                    if (IsWindowVisible(m_iHwnd1) == 0) return false;
                    //엑스레이 조사
                    if (OM.MstOptn.bUseSwTrg)
                    {
                        m_iHwnd3 = FindWindowChild(m_iHwndM, IntPtr.Zero, "Button", "S/W Trigger");
                        if (m_iHwnd3 == IntPtr.Zero) return false;
                        Click(m_iHwnd3);
                    }
                    else
                    {
                        SEQ.XrayCom.SetXrayPara(Setting.sKvp, Setting.smA, Setting.sTime);
                    }
                    
                    //dInspDely = CConfig.StrToDoubleDef(Setting.sTime,0) * 30000 ;
                    dInspDely = 5000;
                    //SEQ.Xray.Run();
                    m_iCycle++;
                    return false;

                case 23: //
                    if (!m_tmDelay.OnDelay((int)dInspDely)) return false;
                    m_iHwnd1 = FindWindowL("#32770",null);
                    if (IsWindowVisible(m_iHwnd1) == 1)
                    {
                        if (iRptCnt == OM.CmnOptn.iXrayRptCnt)
                        {
                            iRptCnt = 0;
                            sErrMsg = "Acquisition 4포인트 이미지 파일 생성에 실패했습니다.";
                            m_iCycle = 0;
                            return true;
                        }
                        if (OM.CmnOptn.iXrayRptCnt > 0)
                        {
                            iRptCnt++;
                            m_tmDelay.Clear();
                            m_iCycle = 22;
                            return false;
                        }
                        else if (OM.CmnOptn.iXrayRptCnt <= 0)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        iRptCnt = 0;
                    }
                    sPath = OM.DressyInfo.sCalPath + "\\Temp\\" + sFileName + ".raw";
                    if(!File.Exists(sPath))
                    {
                        sErrMsg = "ERR007_Acquisition Get Image 실패";
                        return true;
                    }
                    m_iCycle = 30;
                    return false;

                case 30: //프로그램 실행되어 있는지 확인하고
                    if (!ExistProces(OM.DressyInfo.sAppName3) || m_iHwndM == IntPtr.Zero)
                    {
                        m_iCycle++;
                        return false;
                    }
                    m_iCycle++;
                    return false;

                case 31: //프로그램 실행하기
                    if (!ExitProcess(OM.DressyInfo.sAppName3)) return false; //
                    InitProcess2(OM.DressyInfo.sAppPath3);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 32: //
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwndS = FindWindowL("#32770", "Noise Evaluation for Factory [ver 1.0]");
                    if (!ExistProces(OM.DressyInfo.sAppName3) || m_iHwndM == IntPtr.Zero)
                    {
                        sErrMsg = "ERR002_프로그램이 실행되지 않았습니다.";
                        return true;
                    }
                    SetWindowPos(m_iHwndS, IntPtr.Zero, OM.DressyInfo.iPosX3, OM.DressyInfo.iPosY3, 0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);
                    m_iCycle = 40;
                    return false;

                case 40://시리얼 넘버 클릭하고 확인
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "#32770", "ElectronicNoisePerformance");
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    SetWindowText(m_iHwnd1, Tools.Edit, 1, OM.DressyInfo.sC);
                    SetWindowText(m_iHwnd1, Tools.Edit, 2, OM.DressyInfo.sD);
                    m_iCycle++;
                    return false;

                case 41:
                    if (!m_tmDelay.OnDelay(true, iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "#32770", "ElectronicNoisePerformance");
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    sPath = OM.DressyInfo.sCalPath + "\\Temp\\";// + sFileName + ".raw";
                    SetWindowText(m_iHwnd1, Tools.Edit, 7, sPath);
                    m_iCycle++;
                    return false;

                case 42:
                    if (!m_tmDelay.OnDelay(true, iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "#32770", "ElectronicNoisePerformance");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "RUN");
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    Click(m_iHwnd2);
                    m_iCycle++;
                    return false;

                case 43:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770", "ENP_Inspection");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Static", "확인");
                    if (m_tmDelay.OnDelay(GetWindowText(m_iHwnd1, Tools.Static, 0).IndexOf("Complete") == -1, 12000))
                    {
                        sErrMsg = "ERR008_ENP_Inspection 프로그램에서 결과데이터를 가져오지 못했습니다.";
                        Trace(sErrMsg);
                        return true;
                    }
                    if (GetWindowText(m_iHwnd1, Tools.Static, 0).IndexOf("Complete") == -1) return false;
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 44:
                    if (!m_tmDelay.OnDelay(true, iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770", "ENP_Inspection");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "확인");
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    Click(m_iHwnd2);
                    m_iCycle++;
                    return false;

                case 45: //확인 취소창이 없어졋는지 확인.
                    if (!m_tmDelay.OnDelay(true, iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770", "ENP_Inspection");
                    if (m_iHwnd1 != IntPtr.Zero) return false;
                    m_iCycle++;
                    return false;

                case 46:
                    if (!m_tmDelay.OnDelay(true, iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "#32770", "ElectronicNoisePerformance");
                    if (m_iHwnd1 == IntPtr.Zero) return false;

                    i1 = CConfig.StrToIntDef(GetWindowText(m_iHwnd1, Tools.Edit, 14),0);
                    i2 = CConfig.StrToIntDef(GetWindowText(m_iHwnd1, Tools.Edit, 14),0);
                    if (i1 < Setting.iMin) { sErrMsg = "ERR009_(설정치보다 Median값이 작음)"; return true; }
                    if (i2 > Setting.iMax) { sErrMsg = "ERR00A_(설정치보다 Median값이 큼)"; return true; }
                    
                    sPath = OM.DressyInfo.sCalPath + "\\Temp\\" + Path.GetFileName(@Setting.sFileName1) ;
                    sName = OM.DressyInfo.sCalPath + "\\Cal\\" + sSerial + "\\" + @Setting.sFileName1;

                    DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(sName));
                    if(!di.Exists) di.Create();
                    if (File.Exists(sName))
                    {
                        File.Delete(sName);
                    }
                    File.Copy(sPath,sName);
                    sName = OM.DressyInfo.sCalPath + "\\Cal\\" + Path.GetFileName(@Setting.sFileName1);
                    if (File.Exists(sName))
                    {
                        File.Delete(sName);
                    }
                    File.Copy(sPath,sName);

                    if(File.Exists(sPath)) File.Delete(sPath);
                    m_iCycle++;
                    return false;

                case 47:
                    if (!m_tmDelay.OnDelay(true, iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "Button", "취소");
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    Click(m_iHwnd1);
                    m_iCycle = 0;
                    return true;
            }
        }

        /// <summary>
        /// Cal Update //주의사항 마지막에 시간이 많이 필요함.
        /// </summary>
        /// <returns></returns>
        public bool Cycle7() 
        {
            String sTemp;
            if (m_tmCycle.OnDelay(m_iCycle != 0 && m_iCycle == m_iPreCycle, 200000))
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                if (sErrMsg == "") sErrMsg = "ERR100_"+m_sPartName + "Macro Time Out Error " + sTemp;
                Trace(sErrMsg);
                m_iCycle = 0;
                return true;
            }

            if (m_iCycle != m_iPreCycle)
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                Trace(sTemp);
                m_tmDelay.Clear();
            }

            m_iHwnd1 = IntPtr.Zero;
            m_iHwnd2 = IntPtr.Zero;
            m_iHwnd3 = IntPtr.Zero;
            m_iHwnd4 = IntPtr.Zero;

            string sClassName1 = "#32770";
            string sPath       = "";
            string sPath1      = "";
            string sPath2      = "";
            string sName       = "";
            string sName1      = "";
            string sName2      = "";                
            string sName3      = "";
            string sName4      = "";
            //Tab Order
            int Dev = 0;
            int Acq = 1;
            int Cal = 2;
            int Man = 3;

            int i1,i2;
            m_iPreCycle = m_iCycle;

            int iDly = 700;
            
            switch (m_iCycle)
            {

                default:
                    sTemp = string.Format("Cycle_Default_Clear_m_iCycle={0:000}", m_iCycle);
                    sErrMsg = sTemp;
                    return true;


                case 0:
                    return true;
                    
                case 10: //프로그램 실행되어 있는지 확인하고
                    InitStart();
                    m_iCycle++;
                    return false;

                case 11: //프로그램 실행하기
                    if (!Cycle0()) return false;
                    m_iCycle = 20;
                    return false;

                case 20: //Cal Options Setting
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", Cal);
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    SetTabControl(m_iHwnd1,Cal);
                    sName1 = "" ;
                    sName2 = "" ;
                    sName3 = "" ;
                    sName4 = "" ;
                    if (System.IO.Directory.Exists(OM.DressyInfo.sCalPath + "\\Cal"))
                    {
                        System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(OM.DressyInfo.sCalPath + "\\Cal");
                        foreach (var item in di.GetFiles())
                        {
                            if (sName1 == "")
                            {
                                if (item.Name.IndexOf("X") >= 0)
                                {
                                    sName1 = item.Name;
                                }
                            }
                            else if (sName2 == "")
                            {
                                if (item.Name.IndexOf("X") >= 0)
                                {
                                    sName2 = item.Name;
                                }
                            }
                            else if (sName3 == "")
                            {
                                if (item.Name.IndexOf("X") >= 0)
                                {
                                    sName3 = item.Name;
                                }
                            }
                            else if (sName4 == "")
                            {
                                if (item.Name.IndexOf("X") >= 0)
                                {
                                    sName4 = item.Name;
                                }
                            }
                        }
                    }
                    if (!File.Exists(OM.DressyInfo.sCalPath + "\\Cal\\" + sName1)) { sErrMsg = "ERR00D_Xxxxx.raw 파일을(1) 찾을수 없습니다."; return true; }
                    if (!File.Exists(OM.DressyInfo.sCalPath + "\\Cal\\" + sName2)) { sErrMsg = "ERR00D_Xxxxx.raw 파일을(2) 찾을수 없습니다."; return true; }
                    if (!File.Exists(OM.DressyInfo.sCalPath + "\\Cal\\" + sName3)) { sErrMsg = "ERR00D_Xxxxx.raw 파일을(3) 찾을수 없습니다."; return true; }
                    if (!File.Exists(OM.DressyInfo.sCalPath + "\\Cal\\" + sName4)) { sErrMsg = "ERR00D_Xxxxx.raw 파일을(4) 찾을수 없습니다."; return true; }

                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", Cal);
                    m_iHwnd3 = FindWindowChild(m_iHwnd2, IntPtr.Zero, "Button", "Generate");
                    if (m_iHwnd3 == IntPtr.Zero) return false;
                    Click(m_iHwnd3); //제너레이트 버튼 클릭

                    m_iCycle++;
                    return false;

                case 21://BPM Generate
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", Cal);
                    m_iHwnd3 = FindWindowChild(m_iHwnd2, IntPtr.Zero, "Button", "BPM Generate");
                    if (m_iHwnd3 == IntPtr.Zero) return false;
                    Click(m_iHwnd3); //
                    m_iCycle++;
                    return false;

                case 22: //
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    sPath1 = OM.DressyInfo.sCalPath + "\\Cal\\bpm.raw"    ;
                    sPath2 = OM.DressyInfo.sCalPath + "\\Cal\\" + sSerial + "\\bpm.raw"    ;
                    if (!File.Exists(sPath1)) { sErrMsg = "ERR00C_bpm.raw 파일을 찾을수 없습니다."; return true; }
                    if (!File.Exists(sPath2)) File.Copy(sPath1,sPath2);

                    sTemp = GetWindowText(m_iHwndM, Tools.RichEdit, 0);

                    i1 = sTemp.LastIndexOf("(Size : ") + 8;
                    Result.Blemish_amount = sTemp.Substring(i1, 1);

                    i1 = sTemp.LastIndexOf("pixels (")+8 ;
                    i2 = sTemp.LastIndexOf("%)") ;
                    Result.Total_blemish_count = "";
                    string sTempSub2 = sTemp.Substring(i1, i2 - i1);
                    Result.Total_blemish_count = sTemp.Substring(i1,i2-i1);
                    if(Result.Total_blemish_count == "")
                    {
                        sErrMsg = "ERR00E_BPMGenerate 결과값을 받아올수 없습니다.";
                        return true;
                    }

                    sName1 = "" ;
                    sName2 = "" ;
                    sName3 = "" ;
                    sName4 = "" ;
                    if (System.IO.Directory.Exists(OM.DressyInfo.sCalPath + "\\Cal\\"+ sSerial))
                    {
                        System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(OM.DressyInfo.sCalPath + "\\Cal\\" + sSerial);
                        foreach (var item in di.GetFiles())
                        {
                            if (item.Name.IndexOf("gain00") >= 0)
                            {
                                sName1 = item.Name;
                            }
                            if (item.Name.IndexOf("gain01") >= 0)
                            {
                                sName2 = item.Name;
                            }
                            if (item.Name.IndexOf("offset.raw") >= 0)
                            {
                                sName3 = item.Name;
                            }
                            if (item.Name.IndexOf("bpm.raw") >= 0)
                            {
                                sName4 = item.Name;
                            }
                        }
                    }
                    sPath  = OM.DressyInfo.sCalPath;
                    sPath1 = OM.DressyInfo.sCalPath + "\\Cal\\" + sSerial + "\\" + sName1;
                    sPath2 = OM.DressyInfo.sCalPath + "\\Cal\\" + sSerial + "\\" + sName2;
                    if (!File.Exists(sPath1)) { sErrMsg = "ERR006_Gain00_00xx.raw 파일을 찾을수 없습니다."; return true; }
                    if (!File.Exists(sPath2)) { sErrMsg = "ERR006_Gain01_0xxx.raw 파일을 찾을수 없습니다."; return true; }
                    sPath1 = OM.DressyInfo.sCalPath + "\\Cal\\" + sSerial + "\\" + sName3;
                    sPath2 = OM.DressyInfo.sCalPath + "\\Cal\\" + sSerial + "\\" + sName4;
                    if (!File.Exists(sPath1)) { sErrMsg = "ERR00B_offset.raw 파일을 찾을수 없습니다."; return true; }
                    if (!File.Exists(sPath2)) { sErrMsg = "ERR00C_bpm.raw 파일을 찾을수 없습니다."   ; return true; }

                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", Cal);
                    m_iHwnd3 = FindWindowChild(m_iHwnd2, IntPtr.Zero, "Button", "Cal. Update");
                    if (m_iHwnd3 == IntPtr.Zero) return false;
                    Click(m_iHwnd3);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 23: //
                    m_iHwnd1 = IntPtr.Zero;
                    m_iHwnd2 = IntPtr.Zero;
                    m_iHwnd3 = IntPtr.Zero;
                    string sTemp1 = "Update the calibration files...";

                    m_iHwnd1 = FindWindowL("#32770", "");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "msctls_progress32", sTemp1);
                    if (IsWindowVisible(m_iHwnd1) == 0 )return false;
                    m_iCycle++;
                    return false;

                case 24: //
                    if (!m_tmDelay.OnDelay((int)dInspDely)) return false;
                    m_iHwnd1 = IntPtr.Zero;
                    m_iHwnd1 = FindWindowL("#32770", "");
                    if (IsWindowVisible(m_iHwnd1) == 1) return false;
                    m_iCycle++;
                    return false;

                case 25: //
                    if (!m_tmDelay.OnDelay((int)dInspDely)) return false;
                    m_iHwnd1 = FindWindowL("#32770", null);
                    m_iHwnd2 = FindWindowL("#32770", null);
                    
                    m_iCycle = 0;
                    return true;
            }
        }

        /// <summary>
        /// Acquisition
        /// Setting.iAcq1
        /// Setting.iAcq2
        /// Setting.iAcq3
        /// Setting.iAcq4
        /// Setting.iAcq5
        /// Setting.iAcq6
        /// Setting.iAcq7
        /// Setting.sFileName1
        /// Setting.sKvp
        /// Setting.smA
        /// Setting.sTime
        /// </summary>
        /// <returns></returns>
        public bool Cycle8() //Acquisition 여기까지 Work 이거 마지막에 SNR, MTF 요딴 파일 만들때 쓴다.
        {
            String sTemp;
            if (m_tmCycle.OnDelay(m_iCycle != 0 && m_iCycle == m_iPreCycle, 20000))
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                if (sErrMsg == "") sErrMsg = m_sPartName + "Macro Time Out Error " + sTemp;
                Trace(sErrMsg);
                m_iCycle = 0;
                return true;
            }

            if (m_iCycle != m_iPreCycle)
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                Trace(sTemp);
                m_tmDelay.Clear();
            }

            m_iHwnd1 = IntPtr.Zero;
            m_iHwnd2 = IntPtr.Zero;
            m_iHwnd3 = IntPtr.Zero;
            m_iHwnd4 = IntPtr.Zero;

            string sClassName1 = "#32770";
            string sPath       = "";
            string sName       = "";

            //Tab Order
            int Dev = 0;
            int Acq = 1;
            int Cal = 2;
            int Man = 3;

            m_iPreCycle = m_iCycle;

            int iDly = 700;

            string sFileNameRaw = Path.GetFileName(@Setting.sFileName1);
            int iLength = sFileNameRaw.Length;
            string sFileName = sFileNameRaw.Substring(0, iLength - 4);

            switch (m_iCycle)
            {

                default:
                    sTemp = string.Format("Cycle_Default_Clear_m_iCycle={0:000}", m_iCycle);
                    sErrMsg = sTemp;
                    return true;


                case 0:
                    return true;
                    
                case 10: //프로그램 실행되어 있는지 확인하고
                    InitStart();
                    m_iCycle++;
                    return false;

                case 11: //프로그램 실행하기
                    if (!Cycle0()) return false;
                    m_iCycle = 20;
                    return false;

                case 20: //Cal Options Setting
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", Acq);
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    SetTabControl(m_iHwnd1,Acq);

                    SetWindow(m_iHwnd2,Tools.Button,3 ,Setting.iAcq1);
                    SetWindow(m_iHwnd2,Tools.Button,4 ,Setting.iAcq2);
                    SetWindow(m_iHwnd2,Tools.Button,5 ,Setting.iAcq3);
                    SetWindow(m_iHwnd2,Tools.Button,6 ,Setting.iAcq4);
                    SetWindow(m_iHwnd2,Tools.Button,7 ,Setting.iAcq5);
                    SetWindow(m_iHwnd2,Tools.Button,10,Setting.iAcq6);
                    SetWindow(m_iHwnd2,Tools.Button,8 ,Setting.iAcq7);

                    SetWindowText(m_iHwnd2, Tools.Edit, 4, sFileName);

                    m_iCycle++;
                    return false;

                case 21://Cal Options Check
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", Acq);
                    if (m_iHwnd2 == IntPtr.Zero) return false;

                    if(GetWindow(m_iHwnd2,Tools.Button,3 ) != Setting.iAcq1) { sErrMsg = "ERR000_offset Calibration 셋팅 실패"    ; return true;}
                    if(GetWindow(m_iHwnd2,Tools.Button,4 ) != Setting.iAcq2) { sErrMsg = "ERR000_Gain Calibration 셋팅 실패"      ; return true;}
                    if(GetWindow(m_iHwnd2,Tools.Button,5 ) != Setting.iAcq3) { sErrMsg = "ERR000_Bpm Correction 셋팅 실패"        ; return true;}
                    if(GetWindow(m_iHwnd2,Tools.Button,6 ) != Setting.iAcq4) { sErrMsg = "ERR000_Direct Hit Filtering 셋팅 실패"  ; return true;}
                    if(GetWindow(m_iHwnd2,Tools.Button,7 ) != Setting.iAcq5) { sErrMsg = "ERR000_Gamma Correction 셋팅 실패"      ; return true;}
                    if(GetWindow(m_iHwnd2,Tools.Button,10) != Setting.iAcq6) { sErrMsg = "ERR000_Char. Curve 셋팅 실패"           ; return true;}
                    if(GetWindow(m_iHwnd2,Tools.Button,8 ) != Setting.iAcq7) { sErrMsg = "ERR000_Continuous acquisition 셋팅 실패"; return true;}

                    if(GetWindowText(m_iHwnd2,Tools.Edit,4 ) != sFileName) { sErrMsg = "ERR000_File Name 셋팅 실패"; return true;}

                    m_iHwnd3 = FindWindowChild(m_iHwnd2, IntPtr.Zero, "Button", "Get Image");
                    if (m_iHwnd3 == IntPtr.Zero) return false;
                    Click(m_iHwnd3);
                    m_iCycle++;
                    return false;

                case 22: //X ray 조사
                    if (!m_tmDelay.OnDelay(OM.DressyInfo.iBfXrayDelay)) return false;
                    m_iHwnd1 = FindWindowL("#32770",null);
                    if (IsWindowVisible(m_iHwnd1) == 0) return false;
                    //엑스레이 조사
                    if (OM.MstOptn.bUseSwTrg)
                    {
                        m_iHwnd3 = FindWindowChild(m_iHwndM, IntPtr.Zero, "Button", "S/W Trigger");
                        if (m_iHwnd3 == IntPtr.Zero) return false;
                        Click(m_iHwnd3);
                    }
                    else
                    {
                        SEQ.XrayCom.SetXrayPara(Setting.sKvp, Setting.smA, Setting.sTime);
                    }
                    
                    //dInspDely = CConfig.StrToDoubleDef(Setting.sTime,0) * 30000 ;
                    dInspDely = 5000;
                    m_iCycle++;
                    return false;

                case 23: //
                    if (!m_tmDelay.OnDelay((int)dInspDely)) return false;
                    m_iHwnd1 = FindWindowL("#32770",null);
                    if (IsWindowVisible(m_iHwnd1) == 1)
                    {
                        if (iRptCnt == OM.CmnOptn.iXrayRptCnt)
                        {
                            iRptCnt = 0;
                            sErrMsg = "Acquisition 이미지 파일 생성에 실패했습니다.";
                            m_iCycle = 0;
                            return true;
                        }
                        if (OM.CmnOptn.iXrayRptCnt > 0)
                        {
                            iRptCnt++;
                            m_tmDelay.Clear();
                            m_iCycle = 22;
                            return false;
                        }
                        else if (OM.CmnOptn.iXrayRptCnt <= 0)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        iRptCnt = 0;
                    }
                    sPath = OM.DressyInfo.sCalPath + "\\Temp\\" + Path.GetFileName(@Setting.sFileName1) ;
                    if(!File.Exists(sPath))
                    {
                        sErrMsg = "ERR007_Acquisition Get Image 실패";
                        return true;
                    }

                    string sDir = OM.DressyInfo.sCalPath + "\\Cal\\" + sSerial + "\\" + @Setting.sFileName1;
                    string sSubPath = "";
                    int iDirLength = sDir.Length;
                    for (int i = iDirLength; i >= 0; i--)
                    {
                        int iIndex = sDir.LastIndexOf("\\");
                        if (i == iIndex)
                        {
                            sSubPath = sDir.Substring(0, i);
                        }
                    }


                    
                    sName = OM.DressyInfo.sCalPath + "\\Cal\\" + sSerial + "\\" + @Setting.sFileName1;
                    if (!System.IO.Directory.Exists(sSubPath))
                    {
                        Directory.CreateDirectory(sSubPath);
                    }

                    if (File.Exists(sName))
                    {
                        File.Delete(sName);
                    }
                    File.Copy(sPath,sName);
                    File.Delete(sPath);
                    m_iCycle = 0;
                    return true;
            }
        }

        /// <summary>
        /// VIET 평가 //주의사항 Props 오래 걸림 약 5초
        /// </summary>
        /// <returns></returns>
        public bool Cycle9() //
        {
            String sTemp;
            if (m_tmCycle.OnDelay(m_iCycle != 0 && m_iCycle == m_iPreCycle, 12000))
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                if (sErrMsg == "") sErrMsg = m_sPartName + "Macro Time Out Error " + sTemp;
                Trace(sErrMsg);
                m_iCycle = 0;
                return true;
            }

            if (m_iCycle != m_iPreCycle)
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                Trace(sTemp);
                m_tmDelay.Clear();
            }

            m_iHwnd1 = IntPtr.Zero;
            m_iHwnd2 = IntPtr.Zero;
            m_iHwnd3 = IntPtr.Zero;
            m_iHwnd4 = IntPtr.Zero;

            string sClassName1 = "#32770";
            string sPath       = "";
            string sName       = "";

            //Tab Order
            int Set = 0;
            int Pro = 1;
            int Nps = 2;
            int Mtf = 3;
            int Dqe = 4;
            int Lin = 5;

            double d0,d1,d2,d3,d4,d5,d6,d7;
            m_iPreCycle = m_iCycle;

            int iDly  = 1500 ;
            int iDly1 = 5000;
            int iDly2 = 1000;

            sSerial = OM.EqpStat.sCrntSerial;

            switch (m_iCycle)
            {

                default:
                    sTemp = string.Format("Cycle_Default_Clear_m_iCycle={0:000}", m_iCycle);
                    sErrMsg = sTemp;
                    return true;


                case 0:
                    return true;
                    
                case 10: //INI 디폴트로 덮어 씌우기
                    
                    sPath = OM.DressyInfo.sIniPath2 + "\\";
                    sName = Path.GetFileName(OM.DressyInfo.sIniPath1);
                    if (File.Exists(sPath + sName))
                    {
                        File.Delete(sPath + sName);

                    }
                    File.Copy(OM.DressyInfo.sIniPath1,sPath + sName);
                    m_iCycle++;
                    return false;

                case 11:
                    if (!ExitProcess(OM.DressyInfo.sAppName2)) return false; //TODO :: 잠시 삭제
                    InitProcess1(OM.DressyInfo.sAppPath2);
                    m_iCycle++;
                    return false;

                case 12:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwndM = FindWindowL("#32770", "VIET ver3008");
                    if (!ExistProces(OM.DressyInfo.sAppName2) || m_iHwndM == IntPtr.Zero)
                    {
                        sErrMsg = "ERR002_프로그램이 실행되지 않았습니다.";
                        return true;
                    }
                    SetWindowPos(m_iHwndM, IntPtr.Zero, OM.DressyInfo.iPosX2, OM.DressyInfo.iPosY2, 0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);
                    m_iCycle = 20;
                    return false;

                case 20: //
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", Set);
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    sPath = OM.DressyInfo.sCalPath + "\\Cal\\" + sSerial + "\\1x1" ;
                    SetWindowText(m_iHwnd2,Tools.Edit,0,sPath);
                    SetTabControl(m_iHwnd1,Pro);
                    m_iCycle++;
                    return false;

                case 21: //
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", Pro);
                    m_iHwnd3 = FindWindowChild(m_iHwnd2, IntPtr.Zero, "Button", "Calculate Props");
                    Click(m_iHwnd3);

                    m_iCycle++;
                    return false;

                case 22://iDly1 로 대기 (오래 걸림)
                    if (!m_tmDelay.OnDelay(iDly1)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", Pro);
                    m_iHwnd3 = FindWindowChild(m_iHwnd2, IntPtr.Zero, "Button", "Save Result");
                    Click(m_iHwnd3);

                    m_iCycle++;
                    return false;

                case 23://
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770","다른 이름으로 저장");
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "DUIViewWndClassName", null);
                    m_iHwnd3 = FindWindowChild(m_iHwnd2, IntPtr.Zero, "DirectUIHWND"       , null);
                    m_iHwnd4 = FindWindowChild(m_iHwnd3, IntPtr.Zero, "FloatNotifySink"    , null);
                    if (m_iHwnd4 == IntPtr.Zero) return false;
                    Setting.sPath1 = OM.DressyInfo.sRsltPath;
                    DirectoryInfo di1 = new DirectoryInfo(GetRsltFd());
                    if(!di1.Exists)di1.Create();
                    SetWindowText(m_iHwnd4,Tools.ComboBox,0,GetRsltFd() + "Props");
                    m_iCycle++;
                    return false;

                case 24://
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770","다른 이름으로 저장");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "저장(&S)");
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    Click(m_iHwnd2);
                    m_iCycle++;
                    return false;

                case 25://
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770","다른 이름으로 저장");
                    if (m_iHwnd1 != IntPtr.Zero) return false;
                    m_iCycle = 30;
                    return false;

                case 30://NPS 2
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    SetTabControl(m_iHwnd1,Nps);
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", Nps);
                    SetWindowText(m_iHwnd2, Tools.Edit, 0, OM.DressyInfo.iNPSTop1 .ToString());
                    SetWindowText(m_iHwnd2, Tools.Edit, 1, OM.DressyInfo.iNPSLeft1.ToString());
                    SetWindowText(m_iHwnd2, Tools.Edit, 2, OM.DressyInfo.iNPSArea1.ToString());
                    SetWindowText(m_iHwnd2, Tools.Edit, 3, OM.DressyInfo.iNPSSub1 .ToString());
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 31:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", Nps);
                    m_iHwnd3 = FindWindowChild(m_iHwnd2, IntPtr.Zero, "Button", "Calculate NPS");
                    if (m_iHwnd3 == IntPtr.Zero) return false;
                    Click(m_iHwnd3);
                    m_iCycle++;
                    return false;

                case 32://NPS 2
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", Nps);
                    m_iHwnd3 = FindWindowChild(m_iHwnd2, IntPtr.Zero, "Button", "Save Result");
                    if (m_iHwnd3 == IntPtr.Zero) return false;
                    Click(m_iHwnd3);
                    m_iCycle++;
                    return false;

                case 33://
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770","다른 이름으로 저장");
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "DUIViewWndClassName", null);
                    m_iHwnd3 = FindWindowChild(m_iHwnd2, IntPtr.Zero, "DirectUIHWND"       , null);
                    m_iHwnd4 = FindWindowChild(m_iHwnd3, IntPtr.Zero, "FloatNotifySink"    , null);
                    if (m_iHwnd4 == IntPtr.Zero) return false;
                    SetWindowText(m_iHwnd4,Tools.ComboBox,0,GetRsltFd() + "NPS");
                    m_iCycle++;
                    return false;

                case 34://
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770","다른 이름으로 저장");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "저장(&S)");
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    Click(m_iHwnd2);
                    m_iCycle++;
                    return false;

                case 35://
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770","다른 이름으로 저장");
                    if (m_iHwnd1 != IntPtr.Zero) return false;
                    m_iCycle = 40;
                    return false;

                case 40://MTF 3
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    SetTabControl(m_iHwnd1,Mtf);
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", Mtf);
                    SetWindowText(m_iHwnd2, Tools.Edit, 0, OM.DressyInfo.iMTFROITop1 .ToString());
                    SetWindowText(m_iHwnd2, Tools.Edit, 2, OM.DressyInfo.iMTFROINum1 .ToString());
                    SetWindowText(m_iHwnd2, Tools.Edit, 1, OM.DressyInfo.iMTFROILeft1.ToString());
                    SetWindowText(m_iHwnd2, Tools.Edit, 3, OM.DressyInfo.iMTFROILen1 .ToString());

                    SetWindowText(m_iHwnd2, Tools.Edit, 5, OM.DressyInfo.iMTFEgTop1  .ToString());
                    SetWindowText(m_iHwnd2, Tools.Edit, 7, OM.DressyInfo.iMTFEgHght1 .ToString());
                    SetWindowText(m_iHwnd2, Tools.Edit, 6, OM.DressyInfo.iMTFEgLeft1 .ToString());
                    SetWindowText(m_iHwnd2, Tools.Edit, 8, OM.DressyInfo.iMTFEgWidth1.ToString());
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 41:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", Mtf);
                    m_iHwnd3 = FindWindowChild(m_iHwnd2, IntPtr.Zero, "Button", "Auto sampling");
                    if (m_iHwnd3 == IntPtr.Zero) return false;
                    Click(m_iHwnd3);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 42://MTF 3
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", Mtf);
                    m_iHwnd3 = FindWindowChild(m_iHwnd2, IntPtr.Zero, "Button", "Calculate MTF");
                    if (m_iHwnd3 == IntPtr.Zero) return false;
                    Click(m_iHwnd3);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 43://
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", Mtf);
                    m_iHwnd3 = FindWindowChild(m_iHwnd2, IntPtr.Zero, "Button", "Save Result");
                    if (m_iHwnd3 == IntPtr.Zero) return false;
                    Click(m_iHwnd3);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 44://
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770","다른 이름으로 저장");
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "DUIViewWndClassName", null);
                    m_iHwnd3 = FindWindowChild(m_iHwnd2, IntPtr.Zero, "DirectUIHWND"       , null);
                    m_iHwnd4 = FindWindowChild(m_iHwnd3, IntPtr.Zero, "FloatNotifySink"    , null);
                    if (m_iHwnd4 == IntPtr.Zero) return false;
                    SetWindowText(m_iHwnd4,Tools.ComboBox,0,GetRsltFd() + "MTF");
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 45://
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770","다른 이름으로 저장");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "저장(&S)");
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    Click(m_iHwnd2);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 46://
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770","다른 이름으로 저장");
                    if (m_iHwnd1 != IntPtr.Zero) return false;
                    m_tmDelay.Clear();
                    m_iCycle = 50;
                    return false;

                case 50://DQE
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    SetTabControl(m_iHwnd1,Dqe);
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", Dqe);
                    m_iHwnd3 = FindWindowChild(m_iHwnd2, IntPtr.Zero, "Button", "Calculate DQE");
                    if (m_iHwnd3 == IntPtr.Zero) return false;
                    Click(m_iHwnd3);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 51://
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", Dqe);
                    m_iHwnd3 = FindWindowChild(m_iHwnd2, IntPtr.Zero, "Button", "Save Result");
                    if (m_iHwnd3 == IntPtr.Zero) return false;
                    Click(m_iHwnd3);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 52://
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770","다른 이름으로 저장");
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "DUIViewWndClassName", null);
                    m_iHwnd3 = FindWindowChild(m_iHwnd2, IntPtr.Zero, "DirectUIHWND"       , null);
                    m_iHwnd4 = FindWindowChild(m_iHwnd3, IntPtr.Zero, "FloatNotifySink"    , null);
                    if (m_iHwnd4 == IntPtr.Zero) return false;
                    SetWindowText(m_iHwnd4,Tools.ComboBox,0,GetRsltFd() + "DQE");
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 53://
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770","다른 이름으로 저장");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "저장(&S)");
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    Click(m_iHwnd2);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 54://
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770","다른 이름으로 저장");
                    if (m_iHwnd1 != IntPtr.Zero) return false;
                    m_tmDelay.Clear();
                    m_iCycle = 60;
                    return false;

                case 60://Linearity
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    SetTabControl(m_iHwnd1,Lin);
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", Lin);
                    m_iHwnd3 = FindWindowChild(m_iHwnd2, IntPtr.Zero, "Button", "Calculate Linearity");
                    if (m_iHwnd3 == IntPtr.Zero) return false;
                    Click(m_iHwnd3);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 61://
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", Lin);
                    m_iHwnd3 = FindWindowChild(m_iHwnd2, IntPtr.Zero, "Button", "Save Result");
                    if (m_iHwnd3 == IntPtr.Zero) return false;
                    Click(m_iHwnd3);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 62://
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770","다른 이름으로 저장");
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "DUIViewWndClassName", null);
                    m_iHwnd3 = FindWindowChild(m_iHwnd2, IntPtr.Zero, "DirectUIHWND"       , null);
                    m_iHwnd4 = FindWindowChild(m_iHwnd3, IntPtr.Zero, "FloatNotifySink"    , null);
                    if (m_iHwnd4 == IntPtr.Zero) return false;
                    SetWindowText(m_iHwnd4,Tools.ComboBox,0,GetRsltFd() + "Linearity");
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 63://
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770","다른 이름으로 저장");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "저장(&S)");
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    Click(m_iHwnd2);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 64://
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770","다른 이름으로 저장");
                    if (m_iHwnd1 != IntPtr.Zero) return false;
                    m_tmDelay.Clear();
                    m_iCycle = 70;
                    return false;

                case 70://데이터 취합.
                    if (!m_tmDelay.OnDelay(iDly)) return false;

                    Result.Row_defects         = "0";
                    Result.Column_defects      = "0";
                    Result.Adjacent_columns    = "0";
                    Result.Adjacent_rows       = "0";
                    if (Result.Blemish_amount == "")
                    {
                        sErrMsg = "ERR00E_BPMGenerate 결과값을 받아올수 없습니다.";
                        return true;
                    }
                    if (Result.Total_blemish_count == "")
                    {
                        sErrMsg = "ERR00E_BPMGenerate 결과값을 받아올수 없습니다.";
                        return true;
                    }
                    if (Result.Offset == "")
                    {
                        sErrMsg = "ERR00E_BPMGenerate 결과값을 받아올수 없습니다.";
                        return true;
                    }
                    
                    Result.Trigger_sensitivity = "4.2" ;
                    //진섭
                    //Props 1x1
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", Pro);
                    m_iHwnd3 = FindWindowIndex(m_iHwnd2, "SysListView32" , 0);
                    if (m_iHwnd3 == IntPtr.Zero) return false;
                    Result.SNR_R60_100 = GetItemText(m_iHwnd3,0,1);
                    Result.SNR_R70_100 = GetItemText(m_iHwnd3,2,1);
                    Result.SNR_R60_400 = GetItemText(m_iHwnd3,1,1);
                    Result.SNR_R70_400 = GetItemText(m_iHwnd3,3,1);
                    Result.SNR_C60_100 = GetItemText(m_iHwnd3,0,2);
                    Result.SNR_C70_100 = GetItemText(m_iHwnd3,2,2);
                    Result.SNR_C60_400 = GetItemText(m_iHwnd3,1,2);
                    Result.SNR_C70_400 = GetItemText(m_iHwnd3,3,2);

                    Result.Signal_1x1_mode = GetItemText(m_iHwnd3,2,3);
                    Result.Noise_1x1_mode  = GetItemText(m_iHwnd3,2,5);
                    double dSense          = double.Parse(Result.Signal_1x1_mode)/100;
                    Result.Sensitivity     = dSense.ToString("N1") ;

                    //PRUN & BRNU
                    m_iHwnd3 = FindWindowIndex(m_iHwnd2, "SysListView32" , 1);
                    if (m_iHwnd3 == IntPtr.Zero) return false;
                    //일단 최대값으로 넣습니다. 100일단 곱해줌.
                    d0 = 0 ;
                    d1 = CConfig.StrToDoubleDef(GetItemText(m_iHwnd3,0,1),0); if(d1 > d0) d0 = d1 ;
                    d2 = CConfig.StrToDoubleDef(GetItemText(m_iHwnd3,1,1),0); if(d2 > d0) d0 = d2 ;
                    d3 = CConfig.StrToDoubleDef(GetItemText(m_iHwnd3,2,1),0); if(d3 > d0) d0 = d3 ;
                    d4 = CConfig.StrToDoubleDef(GetItemText(m_iHwnd3,3,1),0); if(d4 > d0) d0 = d4 ;
                    Result.Low_frequency_non_uniformity1 = (d0 * 100).ToString();;
                    //일단 최대값으로 넣습니다. 100일단 곱해줌.
                    d0 = 0 ;
                    d1 = CConfig.StrToDoubleDef(GetItemText(m_iHwnd3,0,2),0); if(d1 > d0) d0 = d1 ;
                    d2 = CConfig.StrToDoubleDef(GetItemText(m_iHwnd3,1,2),0); if(d2 > d0) d0 = d2 ;
                    d3 = CConfig.StrToDoubleDef(GetItemText(m_iHwnd3,2,2),0); if(d3 > d0) d0 = d3 ;
                    d4 = CConfig.StrToDoubleDef(GetItemText(m_iHwnd3,3,2),0); if(d4 > d0) d0 = d4 ;
                    Result.Uniformity = (d0 * 100).ToString();;

                    //PRUN in Raw images
                    m_iHwnd3 = FindWindowIndex(m_iHwnd2, "SysListView32" , 2);
                    if (m_iHwnd3 == IntPtr.Zero) return false;
                    //일단 최대값으로 넣습니다. 100일단 곱해줌.
                    d0 = 0 ;
                    d1 = CConfig.StrToDoubleDef(GetItemText(m_iHwnd3,0,1),0); if(d1 > d0) d0 = d1 ;
                    d2 = CConfig.StrToDoubleDef(GetItemText(m_iHwnd3,1,1),0); if(d2 > d0) d0 = d2 ;
                    d3 = CConfig.StrToDoubleDef(GetItemText(m_iHwnd3,2,1),0); if(d3 > d0) d0 = d3 ;
                    d4 = CConfig.StrToDoubleDef(GetItemText(m_iHwnd3,3,1),0); if(d4 > d0) d0 = d4 ;
                    Result.Low_frequency_non_uniformity2 = (d0 * 100).ToString();

                    //MTF 1x1 100일단 곱해줌.
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", Mtf);
                    m_iHwnd3 = FindWindowIndex(m_iHwnd2, "SysListView32" , 0);
                    if (m_iHwnd3 == IntPtr.Zero) return false;
                    
                    Result.MTF_mode_1x1_at_5_lp_mm  = (CConfig.StrToDoubleDef(GetItemText(m_iHwnd3,5 ,1),0) * (double)100).ToString();
                    Result.MTF_mode_1x1_at_8_lp_mm  = (CConfig.StrToDoubleDef(GetItemText(m_iHwnd3,8 ,1),0) * (double)100).ToString();
                    Result.MTF_mode_1x1_at_12_lp_mm = (CConfig.StrToDoubleDef(GetItemText(m_iHwnd3,12,1),0) * (double)100).ToString();

                    //DQE 1x1
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", Dqe);
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    m_iHwnd3 = FindWindowIndex(m_iHwnd2, "SysListView32", 0);
                    if (m_iHwnd3 == IntPtr.Zero) return false;

                    d1 = CConfig.StrToDoubleDef(GetItemText(m_iHwnd3,0,1),0);
                    Result.DQE = (d1 * 100).ToString();

                    //Linearity 1x1
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", Lin);
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    
                    sTemp = GetWindowText(m_iHwnd2, "RICHEDIT", 0);
                    string sTemp1 = sTemp.Substring(sTemp.IndexOf("R2= ") + 4, sTemp.IndexOf("\r\n") - (sTemp.IndexOf("R2= ") + 4));
                    //double dTemp = (1 - double.Parse(sTemp1)) * 1000;
                    double dTemp = (1 - double.Parse(sTemp1)) * 100;
                    double dTemp1 = Math.Ceiling(dTemp); //string.Format("{0:0}", dTemp);
                    Result.Linearity_error = dTemp1.ToString(); ;
                    Result.Saturation_dose = sTemp.Substring(sTemp.IndexOf("Max Dose= ")+10,sTemp.IndexOf("uGy")-(sTemp.IndexOf("Max Dose= ")+10));

                    m_iCycle = 80;
                    return false;

                case 80: //2x2
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    SetTabControl(m_iHwnd1, Set);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 81:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", Set);
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    sPath = OM.DressyInfo.sCalPath + "\\Cal\\" + sSerial + "\\2x2";
                    SetWindowText(m_iHwnd2,Tools.Edit,0,sPath);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 82:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    SetTabControl(m_iHwnd1,Pro);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 83: //
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", Pro);
                    m_iHwnd3 = FindWindowChild(m_iHwnd2, IntPtr.Zero, "Button", "Calculate Props");
                    Click(m_iHwnd3);
                    m_tmDelay.Clear();

                    m_iCycle++;
                    return false;

                case 84://iDly2 로 대기 (오래 걸리진 않음)
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", Pro);
                    m_iHwnd3 = FindWindowChild(m_iHwnd2, IntPtr.Zero, "Button", "Save Result");
                    Click(m_iHwnd3);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 85://
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770","다른 이름으로 저장");
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "DUIViewWndClassName", null);
                    m_iHwnd3 = FindWindowChild(m_iHwnd2, IntPtr.Zero, "DirectUIHWND"       , null);
                    m_iHwnd4 = FindWindowChild(m_iHwnd3, IntPtr.Zero, "FloatNotifySink"    , null);
                    if (m_iHwnd4 == IntPtr.Zero) return false;
                    SetWindowText(m_iHwnd4,Tools.ComboBox,0,GetRsltFd() + "Props_2x2");
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 86://
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770","다른 이름으로 저장");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "저장(&S)");
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    Click(m_iHwnd2);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 87://
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770","다른 이름으로 저장");
                    if (m_iHwnd1 != IntPtr.Zero) return false;
                    m_tmDelay.Clear();
                    m_iCycle = 90;
                    return false;

                case 90://MTF 3
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    SetTabControl(m_iHwnd1,Mtf);
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", Mtf);
                    SetWindowText(m_iHwnd2, Tools.Edit, 0, OM.DressyInfo.iMTFROITop2 .ToString());
                    SetWindowText(m_iHwnd2, Tools.Edit, 2, OM.DressyInfo.iMTFROINum2 .ToString());
                    SetWindowText(m_iHwnd2, Tools.Edit, 1, OM.DressyInfo.iMTFROILeft2.ToString());
                    SetWindowText(m_iHwnd2, Tools.Edit, 3, OM.DressyInfo.iMTFROILen2 .ToString());

                    SetWindowText(m_iHwnd2, Tools.Edit, 5, OM.DressyInfo.iMTFEgTop2  .ToString());
                    SetWindowText(m_iHwnd2, Tools.Edit, 7, OM.DressyInfo.iMTFEgHght2 .ToString());
                    SetWindowText(m_iHwnd2, Tools.Edit, 6, OM.DressyInfo.iMTFEgLeft2 .ToString());
                    SetWindowText(m_iHwnd2, Tools.Edit, 8, OM.DressyInfo.iMTFEgWidth2.ToString());

                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 91:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", Mtf);
                    m_iHwnd3 = FindWindowChild(m_iHwnd2, IntPtr.Zero, "Button", "Auto sampling");
                    if (m_iHwnd3 == IntPtr.Zero) return false;
                    Click(m_iHwnd3);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 92://MTF 3
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", Mtf);
                    m_iHwnd3 = FindWindowChild(m_iHwnd2, IntPtr.Zero, "Button", "Calculate MTF");
                    if (m_iHwnd3 == IntPtr.Zero) return false;
                    Click(m_iHwnd3);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 93://
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", Mtf);
                    m_iHwnd3 = FindWindowChild(m_iHwnd2, IntPtr.Zero, "Button", "Save Result");
                    if (m_iHwnd3 == IntPtr.Zero) return false;
                    Click(m_iHwnd3);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 94://
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770","다른 이름으로 저장");
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "DUIViewWndClassName", null);
                    m_iHwnd3 = FindWindowChild(m_iHwnd2, IntPtr.Zero, "DirectUIHWND"       , null);
                    m_iHwnd4 = FindWindowChild(m_iHwnd3, IntPtr.Zero, "FloatNotifySink"    , null);
                    if (m_iHwnd4 == IntPtr.Zero) return false;
                    SetWindowText(m_iHwnd4,Tools.ComboBox,0,GetRsltFd() + "MTF_2x2");
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 95://
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770","다른 이름으로 저장");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "저장(&S)");
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    Click(m_iHwnd2);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 96://
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770","다른 이름으로 저장");
                    if (m_iHwnd1 != IntPtr.Zero) return false;
                    m_tmDelay.Clear();
                    m_iCycle = 100;
                    return false;

                case 100://데이터 취합 2x2
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", Pro);
                    m_iHwnd3 = FindWindowIndex(m_iHwnd2, "SysListView32" , 0);
                    if (m_iHwnd3 == IntPtr.Zero) return false;

                    //Props 2x2
                    Result.SNR_2x2_mode    = GetItemText(m_iHwnd3,0,1);
                    Result.Signal_2x2_mode = GetItemText(m_iHwnd3,0,3);
                    Result.Noise_2x2_mode  = GetItemText(m_iHwnd3,0,5);

                    //MTF 2x2 100일단 곱해줌.
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", Mtf);
                    m_iHwnd3 = FindWindowIndex(m_iHwnd2, "SysListView32" , 0);
                    if (m_iHwnd3 == IntPtr.Zero) return false;

                    Result.MTF_mode_2x2_at_8_lp_mm  = (CConfig.StrToDoubleDef(GetItemText(m_iHwnd3,8 ,1),0) * (double)100).ToString();

                    m_tmDelay.Clear();
                    m_iCycle = 110;
                    return false;

                case 110://종료
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "Button", "Close");
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    Click(m_iHwnd1);
                    m_iCycle = 0;
                    return true;


            }
        }

        /// <summary>
        /// Write 쓰기
        /// </summary>
        /// <returns></returns>
        public bool Cycle10() //
        {
            String sTemp;
            if (m_tmCycle.OnDelay(m_iCycle != 0 && m_iCycle == m_iPreCycle, 20000))
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                if (sErrMsg == "") sErrMsg = m_sPartName + "Macro Time Out Error " + sTemp;
                Trace(sErrMsg);
                m_iCycle = 0;
                return true;
            }

            if (m_iCycle != m_iPreCycle)
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                Trace(sTemp);
                m_tmDelay.Clear();
            }

            m_iHwnd1 = IntPtr.Zero;
            m_iHwnd2 = IntPtr.Zero;
            m_iHwnd3 = IntPtr.Zero;
            m_iHwnd4 = IntPtr.Zero;

            string sClassName1 = "#32770";
            string sPath       = "";
            string sPath1      = "";
            string sPath2      = "";
            string sName       = "";
            string sName1      = "";
            string sName2      = "";

            //Tab Order
            int Dev = 0;
            int Acq = 1;
            int Cal = 2;
            int Man = 3;

            m_iPreCycle = m_iCycle;

            int iDly = 200;

            switch (m_iCycle)
            {

                default:
                    sTemp = string.Format("Cycle_Default_Clear_m_iCycle={0:000}", m_iCycle);
                    sErrMsg = sTemp;
                    return true;


                case 0:
                    return true;
                    
                case 10: //프로그램 실행되어 있는지 확인하고
                    //소수점 첫째자리까지 표시 되도록 변환 
                    double dTrigger     = double.Parse(SEQ.Mcr.Dr1.Result.Trigger_sensitivity           );
                    double dS_R60100    = double.Parse(SEQ.Mcr.Dr1.Result.SNR_R60_100                   );
                    double dS_R70100    = double.Parse(SEQ.Mcr.Dr1.Result.SNR_R70_100                   );
                    double dS_R60400    = double.Parse(SEQ.Mcr.Dr1.Result.SNR_R60_400                   );
                    double dS_R70400    = double.Parse(SEQ.Mcr.Dr1.Result.SNR_R70_400                   );
                    double dS_C60100    = double.Parse(SEQ.Mcr.Dr1.Result.SNR_C60_100                   );
                    double dS_C70100    = double.Parse(SEQ.Mcr.Dr1.Result.SNR_C70_100                   );
                    double dS_C60400    = double.Parse(SEQ.Mcr.Dr1.Result.SNR_C60_400                   );
                    double dS_C70400    = double.Parse(SEQ.Mcr.Dr1.Result.SNR_C70_400                   );
                    double dSignal1x1   = double.Parse(SEQ.Mcr.Dr1.Result.Signal_1x1_mode               );
                    double dNoise1x1    = double.Parse(SEQ.Mcr.Dr1.Result.Noise_1x1_mode                );
                    double dSens        = double.Parse(SEQ.Mcr.Dr1.Result.Sensitivity                   );
                    double dLf_non_uni1 = double.Parse(SEQ.Mcr.Dr1.Result.Low_frequency_non_uniformity1 );
                    double dUniform     = double.Parse(SEQ.Mcr.Dr1.Result.Uniformity                    );
                    double dLf_non_uni2 = double.Parse(SEQ.Mcr.Dr1.Result.Low_frequency_non_uniformity2 );
                    double dMTF1x1_5lp  = double.Parse(SEQ.Mcr.Dr1.Result.MTF_mode_1x1_at_5_lp_mm       );
                    double dMTF1x1_8lp  = double.Parse(SEQ.Mcr.Dr1.Result.MTF_mode_1x1_at_8_lp_mm       );
                    double dMTF1x1_12lp = double.Parse(SEQ.Mcr.Dr1.Result.MTF_mode_1x1_at_12_lp_mm      );
                    double dDQE         = double.Parse(SEQ.Mcr.Dr1.Result.DQE                           );
                    double dS_Dose      = double.Parse(SEQ.Mcr.Dr1.Result.Saturation_dose               );
                    double dSNR2x2      = double.Parse(SEQ.Mcr.Dr1.Result.SNR_2x2_mode                  );
                    double dSignal2x2   = double.Parse(SEQ.Mcr.Dr1.Result.Signal_2x2_mode               );
                    double dNoise2x2    = double.Parse(SEQ.Mcr.Dr1.Result.Noise_2x2_mode                );
                    double dMTF2x2_8lp  = double.Parse(SEQ.Mcr.Dr1.Result.MTF_mode_2x2_at_8_lp_mm       );
                    
                    Result.Trigger_sensitivity            = string.Format("{0:###0.0}", dTrigger    );
                    Result.SNR_R60_100                    = string.Format("{0:###0.0}", dS_R60100   );
                    Result.SNR_R70_100                    = string.Format("{0:###0.0}", dS_R70100   );
                    Result.SNR_R60_400                    = string.Format("{0:###0.0}", dS_R60400   );
                    Result.SNR_R70_400                    = string.Format("{0:###0.0}", dS_R70400   );
                    Result.SNR_C60_100                    = string.Format("{0:###0.0}", dS_C60100   );
                    Result.SNR_C70_100                    = string.Format("{0:###0.0}", dS_C70100   );
                    Result.SNR_C60_400                    = string.Format("{0:###0.0}", dS_C60400   );
                    Result.SNR_C70_400                    = string.Format("{0:###0.0}", dS_C70400   );
                    Result.Signal_1x1_mode                = string.Format("{0:###0.0}", dSignal1x1  );
                    Result.Noise_1x1_mode                 = string.Format("{0:###0.0}", dNoise1x1   );
                    Result.Sensitivity                    = string.Format("{0:###0.0}", dSens       );
                    Result.Low_frequency_non_uniformity1  = string.Format("{0:###0.0}", dLf_non_uni1);
                    Result.Uniformity                     = string.Format("{0:###0.0}", dUniform    );
                    Result.Low_frequency_non_uniformity2  = string.Format("{0:###0.0}", dLf_non_uni2);
                    Result.MTF_mode_1x1_at_5_lp_mm        = string.Format("{0:###0.0}", dMTF1x1_5lp );
                    Result.MTF_mode_1x1_at_8_lp_mm        = string.Format("{0:###0.0}", dMTF1x1_8lp );
                    Result.MTF_mode_1x1_at_12_lp_mm       = string.Format("{0:###0.0}", dMTF1x1_12lp);
                    Result.DQE                            = string.Format("{0:###0.0}", dDQE        );
                    Result.Saturation_dose                = string.Format("{0:###0.0}", dS_Dose     );
                    Result.SNR_2x2_mode                   = string.Format("{0:###0.0}", dSNR2x2     );
                    Result.Signal_2x2_mode                = string.Format("{0:###0.0}", dSignal2x2  );
                    Result.Noise_2x2_mode                 = string.Format("{0:###0.0}", dNoise2x2   );
                    Result.MTF_mode_2x2_at_8_lp_mm        = string.Format("{0:###0.0}", dMTF2x2_8lp );

                    InitStart();
                    m_iCycle++;
                    return false;

                case 11: //프로그램 실행하기
                    if (!Cycle0()) return false;
                    m_iCycle = 20;
                    return false;

                case 20: //Cal Options Setting
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", Man);
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    SetTabControl(m_iHwnd1,Man);

                    m_iHwnd3 = FindWindowChild(m_iHwnd2, IntPtr.Zero, "Button", "Write");
                    if (m_iHwnd3 == IntPtr.Zero) return false;
                    Click(m_iHwnd3);

                    m_iCycle++;
                    return false;

                case 21://첫번째 페이지
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770","Inspection Sheet");
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", 0);
                    if (m_iHwnd2 == IntPtr.Zero) return false;

                    SetWindowText(m_iHwnd2,Tools.Edit,0 ,OM.DressyInfo.sProductCode);
                    SetWindowText(m_iHwnd2,Tools.Edit,1 ,OM.DressyInfo.sProductVer );
                    SetWindowText(m_iHwnd2,Tools.Edit,2 ,OM.EqpStat.sSerialList    );
                    SetWindowText(m_iHwnd2,Tools.Edit,10,OM.DressyInfo.sFPGAVer    );
                    SetWindowText(m_iHwnd2,Tools.Edit,11,OM.DressyInfo.sAcqSW      );
                    SetWindowText(m_iHwnd2,Tools.Edit,12,OM.DressyInfo.sEvalSW     );
                    SetWindowText(m_iHwnd2,Tools.Edit,13,OM.EqpStat.sTemp          );
                    SetWindowText(m_iHwnd2,Tools.Edit,14,OM.EqpStat.sHumid         );

                    m_iHwnd3 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button" , "Next >");
                    if (m_iHwnd3 == IntPtr.Zero) return false;
                    Click(m_iHwnd3);
                    m_iCycle++;
                    return false;

                case 22://두번째 페이지
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770","Inspection Sheet");
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", 1);
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    SetWindowText(m_iHwnd2,Tools.Edit,0 ,OM.EqpStat.sVolt );//이거 어디에 입력?
                    m_iHwnd3 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button" , "Next >");
                    if (m_iHwnd3 == IntPtr.Zero) return false;
                    Click(m_iHwnd3);
                    m_iCycle++;
                    return false;

                case 23://3번째 페이지
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770","Inspection Sheet");
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", 2);
                    if (m_iHwnd2 == IntPtr.Zero) return false;

                    SetWindowText(m_iHwnd2,Tools.Edit,0,Result.Trigger_sensitivity          );
                    SetWindowText(m_iHwnd2,Tools.Edit,1,Result.Low_frequency_non_uniformity2);
                    SetWindowText(m_iHwnd2,Tools.Edit,2,Result.Blemish_amount               );
                    SetWindowText(m_iHwnd2,Tools.Edit,3,Result.Row_defects                  );
                    SetWindowText(m_iHwnd2,Tools.Edit,4,Result.Column_defects               );
                    SetWindowText(m_iHwnd2,Tools.Edit,5,Result.Adjacent_columns             );
                    SetWindowText(m_iHwnd2,Tools.Edit,6,Result.Adjacent_rows                );
                    SetWindowText(m_iHwnd2,Tools.Edit,7,Result.Total_blemish_count          );
                    SetWindowText(m_iHwnd2,Tools.Edit,8,Result.Saturation_dose              );
                    SetWindowText(m_iHwnd2,Tools.Edit,9,Result.Offset                       );

                    m_iHwnd3 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button" , "Next >");
                    if (m_iHwnd3 == IntPtr.Zero) return false;
                    Click(m_iHwnd3);
                    m_iCycle++;
                    return false;

                case 24://4번째 페이지
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770","Inspection Sheet");
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", 3);
                    if (m_iHwnd2 == IntPtr.Zero) return false;

                    SetWindowText(m_iHwnd2,Tools.Edit, 0,Result.Low_frequency_non_uniformity1);
                    SetWindowText(m_iHwnd2,Tools.Edit, 1,Result.SNR_R60_100                  );
                    SetWindowText(m_iHwnd2,Tools.Edit, 2,Result.SNR_R70_100                  );
                    SetWindowText(m_iHwnd2,Tools.Edit, 3,Result.SNR_R60_400                  );
                    SetWindowText(m_iHwnd2,Tools.Edit, 4,Result.SNR_R70_400                  );
                    SetWindowText(m_iHwnd2,Tools.Edit, 5,Result.SNR_2x2_mode                 );
                    SetWindowText(m_iHwnd2,Tools.Edit, 6,Result.MTF_mode_1x1_at_5_lp_mm      );
                    SetWindowText(m_iHwnd2,Tools.Edit, 7,Result.MTF_mode_1x1_at_8_lp_mm      );
                    SetWindowText(m_iHwnd2,Tools.Edit, 8,Result.MTF_mode_1x1_at_12_lp_mm     );
                    SetWindowText(m_iHwnd2,Tools.Edit, 9,Result.Uniformity                   );
                    SetWindowText(m_iHwnd2,Tools.Edit,10,Result.Linearity_error              );
                    SetWindowText(m_iHwnd2,Tools.Edit,11,Result.DQE                          );
                    SetWindowText(m_iHwnd2,Tools.Edit,12,Result.Sensitivity                  );
                    SetWindowText(m_iHwnd2,Tools.Edit,13,Result.Signal_1x1_mode              );
                    SetWindowText(m_iHwnd2,Tools.Edit,14,Result.Signal_2x2_mode              );
                    SetWindowText(m_iHwnd2,Tools.Edit,15,Result.Noise_1x1_mode               );
                    SetWindowText(m_iHwnd2,Tools.Edit,16,Result.Noise_2x2_mode               );

                    m_iHwnd3 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button" , "Next >");
                    if (m_iHwnd3 == IntPtr.Zero) return false;
                    Click(m_iHwnd3);
                    m_iCycle++;
                    return false;

                case 25://5번째 페이지
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770","Inspection Sheet");
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", 4);
                    if (m_iHwnd2 == IntPtr.Zero) return false;

                    SetWindowText(m_iHwnd2,Tools.Edit, 0,Result.SNR_C60_100);
                    SetWindowText(m_iHwnd2,Tools.Edit, 1,Result.SNR_C70_100);
                    SetWindowText(m_iHwnd2,Tools.Edit, 2,Result.SNR_C60_400);
                    SetWindowText(m_iHwnd2,Tools.Edit, 3,Result.SNR_C70_400);
                    SetWindowText(m_iHwnd2,Tools.Edit, 5,Result.MTF_mode_2x2_at_8_lp_mm);
                    SetWindowText(m_iHwnd2, Tools.Edit, 7, OM.DressyInfo.sPerform);

                    m_iCycle++;
                    return false;

                case 26:
                    m_iHwnd1 = FindWindowL("#32770", "Inspection Sheet");
                    m_iHwnd3 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button" , "Finish");
                    if (m_iHwnd3 == IntPtr.Zero) return false;
                    Click(m_iHwnd3);
                    m_iCycle++;
                    return false;

                case 27://파일 복사하기 INI 파일
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770","Inspection Sheet");
                    if (m_iHwnd1 != IntPtr.Zero)
                    {
                        m_iCycle = 26;
                        return false;
                    }
                    
                    m_iCycle = 0;
                    return true;
            }
        }

        /// <summary>
        /// Write 쓰기 이후에 이미지 저장된 폴더 Report 폴더로 복사
        /// </summary>
        /// <returns></returns>
        public bool Cycle11() //
        {
            String sTemp;
            if (m_tmCycle.OnDelay(m_iCycle != 0 && m_iCycle == m_iPreCycle, 20000))
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                if (sErrMsg == "") sErrMsg = m_sPartName + "Macro Time Out Error " + sTemp;
                Trace(sErrMsg);
                m_iCycle = 0;
                return true;
            }

            if (m_iCycle != m_iPreCycle)
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                Trace(sTemp);
                m_tmDelay.Clear();
            }

            m_iHwnd1 = IntPtr.Zero;
            m_iHwnd2 = IntPtr.Zero;
            m_iHwnd3 = IntPtr.Zero;
            m_iHwnd4 = IntPtr.Zero;

            string sPath1 = "";
            string sPath2 = "";

            //Tab Order
            int Man = 3;

            m_iPreCycle = m_iCycle;

            int iDly = 200;

            switch (m_iCycle)
            {

                default:
                    sTemp = string.Format("Cycle_Default_Clear_m_iCycle={0:000}", m_iCycle);
                    sErrMsg = sTemp;
                    return true;


                case 0:
                    return true;

                case 10: //프로그램 실행되어 있는지 확인하고
                    InitStart();
                    m_iCycle++;
                    return false;

                case 11: //프로그램 실행하기
                    if (!Cycle0()) return false;
                    m_iCycle = 20;
                    return false;

                case 20://파일 복사하기 전부 지정된 폴더로 카피
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    sPath1 = OM.DressyInfo.sCalPath + "\\Cal\\" + sSerial;
                    sPath2 = GetRsltFd();

                    DirectoryInfo dir = new DirectoryInfo(sPath2);
                    if (dir.Exists) //읽기전용 파일들 찾아서 노멀로 바꿔주면 삭제 가능
                    {
                        FileInfo[] files = dir.GetFiles("*.*",

                        SearchOption.AllDirectories);

                        foreach (FileInfo file in files)
                        {
                            file.Attributes = FileAttributes.Normal;
                        }
                    }

                    CopyFolder(sPath1, sPath2);
                    m_iCycle++;
                    return false;

                case 21://
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwndM = FindWindowL("#32770", "Dressy I/O Sensor Manufacturer Tool");
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "Button", "Close");
                    if (m_iHwnd1 != IntPtr.Zero)
                    {
                        Click(m_iHwnd1);
                    }

                    m_iCycle = 0;
                    return true;
            }
        }

        /// <summary>
        /// File Check (ReConnect)
        /// </summary>
        /// <returns></returns>
        public bool Cycle12() //
        {
            String sTemp;
            if (m_tmCycle.OnDelay(m_iCycle != 0 && m_iCycle == m_iPreCycle, 20000))
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                if (sErrMsg == "") sErrMsg = m_sPartName + "Macro Time Out Error " + sTemp;
                Trace(sErrMsg);
                m_iCycle = 0;
                return true;
            }

            if (m_iCycle != m_iPreCycle)
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                Trace(sTemp);
                m_tmDelay.Clear();
            }

            m_iHwnd1 = IntPtr.Zero;
            m_iHwnd2 = IntPtr.Zero;
            m_iHwnd3 = IntPtr.Zero;
            m_iHwnd4 = IntPtr.Zero;

            string sClassName1 = "#32770";
            string sPath       = "";
            string sPath1      = "";
            string sPath2      = "";
            string sName       = "";
            string sName1      = "";
            string sName2      = "";

            //Tab Order
            int Dev = 0;
            int Acq = 1;
            int Cal = 2;
            int Man = 3;

            m_iPreCycle = m_iCycle;

            int iDly  = 700 ;
            int iDly1 = 2000;
            int iTemp = 0;
            switch (m_iCycle)
            {

                default:
                    sTemp = string.Format("Cycle_Default_Clear_m_iCycle={0:000}", m_iCycle);
                    sErrMsg = sTemp;
                    return true;


                case 0:
                    return true;
                    
                case 10: //프로그램 실행되어 있는지 확인하고
                    sPath = OM.DressyInfo.sCalPath + "\\Cal\\" + sSerial;
                    DeleteFolder(sPath);
                    InitStart();
                    m_iCycle++;
                    return false;

                case 11: //프로그램 실행하기
                    if (!Cycle0(true)) return false;
                    m_iCycle = 20;
                    return false;

                case 20://
                    //프로그램 켜져있으면 파일 목록 못읽어와서 미리 끔 진섭
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "Button" , "Close");
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    Click(m_iHwnd1);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 21:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    iTemp = 0;
                    sPath = OM.DressyInfo.sCalPath + "\\Cal\\" + sSerial;

                    if (Directory.Exists(sPath))
                    {
                        DirectoryInfo di1 = new DirectoryInfo(sPath);
                        
                        foreach (var item in di1.GetFiles())
                        {
                            if (item.Name.IndexOf("bpm") == 0)
                            {
                                iTemp++;
                            }
                            else if (item.Name.IndexOf("gain00") == 0)
                            {
                                iTemp++;
                            }
                            else if (item.Name.IndexOf("gain01") == 0)
                            {
                                iTemp++;
                            }
                            else if (item.Name.IndexOf("offset") == 0)
                            {
                                iTemp++;
                            }
                        }
                    }

                    if(iTemp != 4)
                    {
                        sErrMsg = "ERR00F_센서를 재접속하엿으나 파일 4개가 모두 생성 안됨";
                        return true;
                    }

                    m_iCycle = 0;
                    return true;
            }
        }

        /// <summary>
        /// 1.0.1.2 (20180530)
        /// Aging
        /// Aging 버튼 사용하기 위해 사이클 추가
        /// </summary>
        /// <returns></returns>
        public bool Cycle13() 
        {
            String sTemp;
            if (m_tmCycle.OnDelay(m_iCycle != 0 && m_iCycle == m_iPreCycle, 30000))
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                if (sErrMsg == "") sErrMsg = "ERR100_" + m_sPartName + "Macro Time Out Error " + sTemp;
                Trace(sErrMsg);
                m_iCycle = 0;
                return true;
            }

            if (m_iCycle != m_iPreCycle)
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                Trace(sTemp);
                m_tmDelay.Clear();
            }

            m_iHwnd1 = IntPtr.Zero;
            m_iHwnd2 = IntPtr.Zero;
            m_iHwnd3 = IntPtr.Zero;
            m_iHwnd4 = IntPtr.Zero;

            string sClassName1 = "#32770";
            string sPath = "";
            string sName = "";

            //Tab Order
            int Dev = 0;
            int Acq = 1;
            int Cal = 2;
            int Man = 3;

            m_iPreCycle = m_iCycle;

            int iDly = 1000;

            switch (m_iCycle)
            {

                default:
                    sTemp = string.Format("Cycle_Default_Clear_m_iCycle={0:000}", m_iCycle);
                    sErrMsg = sTemp;
                    return true;


                case 0:
                    return true;

                case 10: //프로그램 실행되어 있는지 확인하고
                    InitStart();
                    m_iCycle++;
                    return false;

                case 11: //프로그램 실행하기
                    if (!Cycle0()) return false;
                    m_iCycle = 20;
                    return false;

                case 20: //겟 다크 클릭
                    sPath = Path.GetDirectoryName(OM.DressyInfo.sCalPath);
                    sName = Path.GetDirectoryName(OM.DressyInfo.sCalPath) + "\\Cal\\offset.raw";
                    if (File.Exists(sName))
                    {
                        File.Delete(sName);
                    }
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", Cal);
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    SetTabControl(m_iHwnd1, Cal);
                    m_iHwnd3 = FindWindowChild(m_iHwnd2, IntPtr.Zero, "Button", "Aging");
                    if (m_iHwnd3 == IntPtr.Zero) return false;

                    Click(m_iHwnd3);
                    m_iCycle++;
                    return false;

                case 21://트리거 파일 체크하고 겟 브라이트 클릭
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwndS = FindWindowL("#32770", "Aging Test");
                    if (m_iHwndS == IntPtr.Zero) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "Button", "Start");
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    Click(m_iHwnd1);

                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 22:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    string sCount = GetWindowText(m_iHwndS, Tools.Edit, 1);
                    int iCount = int.Parse(sCount);

                    if (Directory.Exists(OM.DressyInfo.sCalPath + "\\Cal"))
                    {
                        DirectoryInfo di = new DirectoryInfo(OM.DressyInfo.sCalPath + "\\Cal");

                        foreach (var item in di.GetFiles())
                        {
                            if (item.Name.IndexOf("D000") == 0 && !AllAgingFile.Contains(item.Name)) CrntAgingFile.Add(item.Name);
                        }
                    }
                    if (AllAgingFile.Count < iCount)
                    {
                        AllAgingFile = CrntAgingFile;
                        return false;
                    }
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 23:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    sCount = GetWindowText(m_iHwndS, Tools.Edit, 1);
                    iCount = int.Parse(sCount);
                    if (AllAgingFile.Count == iCount - 1)
                    {
                        if (!File.Exists(sName)) //트리거 불량
                        {
                            sErrMsg = "ERR004_Offset.raw 파일 생성 실패(Aging)";
                            return true;
                        }
                    }
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 24:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "Button", "Close");
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    Click(m_iHwnd1);

                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 25: //D000으로 시작하는 파일들 모두 삭제
                    string sDelFile = "";

                    if (Directory.Exists(OM.DressyInfo.sCalPath + "\\Cal"))
                    {
                        DirectoryInfo di2 = new DirectoryInfo(OM.DressyInfo.sCalPath + "\\Cal");

                        foreach (var item in di2.GetFiles())
                        {
                            if (item.Name.IndexOf("D000") == 0)
                            {
                                sDelFile = item.Name;
                            }
                            FileInfo[] files = di2.GetFiles(sDelFile, SearchOption.AllDirectories);

                            foreach (FileInfo file in files)
                            {
                                file.Attributes = FileAttributes.Normal;
                                file.Delete();
                            }
                        }

                    }
                    AllAgingFile.Clear();
                    CrntAgingFile.Clear();
                    m_iCycle = 0;
                    return true;
            }
        }

        public void Log_Trace(string sLog_Trace, [CallerLineNumber] int sourceLineNumber = 0)
        {
            Debug.WriteLine(DateTime.Now.ToString("MM-dd hh:mm:ss:fff_") + sLog_Trace + "_" + sourceLineNumber.ToString());
        }

    }

    public class EzSensor : MacroCmd
    {
        private int    m_iCycle     ;
        private int    m_iPreCycle  ;
        private double dInspDely = 0;

        public string sErrMsg              ;
        public bool   bErrDevice           ; //Xray 재조사 카운트까지 쐈는데 트리거 안터지면 에러
        public bool   bDetectSerial = false; //시리얼 넘버 받아왔는지 알 길이 없어서 태그 두고 확인 함
        public int    iRptCnt       = 0    ; //Xray 재조사 카운트
        

        private const string  m_sPartName = "EzSensor ";
        protected CDelayTimer m_tmCycle = new CDelayTimer();
        protected CDelayTimer m_tmDelay = new CDelayTimer();

        //Get Bright 할때 필요한 것들
        public bool bReWork = false;//false이면 GetBright 재작업 안하고 True이면 재작업 한다.
        public int  iGetBrightRslt; //Get Bright 3회 하고 편차 구했을때 편차 결과값
        public int  iGetBrightMax;
        public int  iGetBrightMin;
        public int  iGetBrightAvr;

        public struct SResult
        {
            public string LineNoise1x1   ;
            public string DataNoise1x1   ;
            public string Median1x1      ;
            public string Fluc1x1        ;
            public string Min1x1         ;
            public string Max1x1         ;
            public string Dispersion1x1  ;
            public string DynamicRange1x1;
            public string TotalNoise1x1  ;
            public string LineNoise2x2   ;
            public string DataNoise2x2   ;
            public string Median2x2      ;
            public string Fluc2x2        ;
            public string Min2x2         ;
            public string Max2x2         ;
            public string Dispersion2x2  ;
            public string DynamicRange2x2;
            public string TotalNoise2x2  ;
            //DQE 결과값
            public string SNR1x1         ;
            public string Doze1x1        ;
            public string Sens1x1        ;
            public string s3lpmm1x1      ;//얘네 나중에 확인해서 붙여야함 진섭
            public string s6lpmm1x1      ;//얘네 나중에 확인해서 붙여야함 진섭
            public string s8lpmm1x1      ;//얘네 나중에 확인해서 붙여야함 진섭
            public string SNR2x2         ;
            public string Doze2x2        ;
            public string Sens2x2        ;
            public string s3lpmm2x2      ;//얘네 나중에 확인해서 붙여야함 진섭
            public string s6lpmm2x2      ;//얘네 나중에 확인해서 붙여야함 진섭
            public string s8lpmm2x2      ;//얘네 나중에 확인해서 붙여야함 진섭

            public void Clear()
            {
                LineNoise1x1    = "" ;
                DataNoise1x1    = "" ;
                Median1x1       = "" ;
                Fluc1x1         = "" ;
                Min1x1          = "" ;
                Max1x1          = "" ;
                Dispersion1x1   = "" ;
                DynamicRange1x1 = "" ;
                TotalNoise1x1   = "" ;
                LineNoise2x2    = "" ;
                DataNoise2x2    = "" ;
                Median2x2       = "" ;
                Fluc2x2         = "" ;
                Min2x2          = "" ;
                Max2x2          = "" ;
                Dispersion2x2   = "" ;
                DynamicRange2x2 = "" ;
                TotalNoise2x2   = "" ;
                
                SNR1x1          = "" ;
                Doze1x1         = "" ;
                Sens1x1         = "" ;
                SNR2x2          = "" ;
                Doze2x2         = "" ;
                Sens2x2         = "" ;
            }
        };

        public struct SDQE
        {
            
        };

        public struct SSetting
        {
            public string smA          ;
            public string sKvp         ;
            public string sTime        ;
                                     
            public int    iBinning     ;
                                     
            public int    iWidth       ;//영상 사이즈 , 1x1, 2x2일때 달라서 쪼개 쓴다.
            public int    iHeight      ;//영상 사이즈 , 1x1, 2x2일때 달라서 쪼개 쓴다.
            public string sFileName    ; //Get Image 할때 파일 이름 넣는놈
            public string sGetImgFdName; //Get Image 하고 폴더 Rename 하는 것을 Copy & Delete로 대체해서 Copy할 이름 넣는 놈
            public string sFlatPath    ;//특성 검사에서 Flat Frame에 넣는 시리얼넘버 다음 폴더 이름
            public string sObjtPath    ;//특성 검사에서 Object Frame에 넣는 시리얼넘버 다음 폴더 이름
            public string sCalFolder   ;//1x1, 2x2에 따라 CAL, CAL_A에 저장 되서 셋팅함
                                        //CAL_A 폴더는 EzSensor 프로그램에서 인터락 걸려있어서 못쓰고 최종 저장 폴더에서만 CAL_A로 변경해서 쓴다.
            public string sDarkImgPath ;//1x1, 2x2에 따라 CAL, CAL_A에 저장 되서 셋팅함, ENP_Inspection 프로그램에 셋팅됨
            public string sSaveFolder  ;//1x1에 저장할지 2x2에 저장할지 셋팅하는 부분
            public int    iDimWidth    ;
            public int    iDimHght     ;
            public double dPixelPitch  ;
            public int    iCutoffbevel ;

            public int    iNPSLeft     ;
            public int    iNPSTop      ;
            public int    iNPSW        ;
            public int    iNPSH        ;
                                       
            public int    iMTFLeft     ;
            public int    iMTFTop      ;
            public int    iMTFW        ;
            public int    iMTFH        ;

            public int    iAcqMaxFrame ;
            public int    iAcqInterval ;
    
        };

        public void Clear()
        {

        }

        public SResult Result;
        public SSetting Setting;

        public string GetErrCode() { return sErrMsg; }

        public string GetRsltFd()
        {
            string s1 = "";
            string s2 = OM.EqpStat.sYear + "-" + OM.EqpStat.sMonth + "-" + OM.EqpStat.sDay;
            s1 = OM.EzSensorInfo.sRsltPath + "\\" + s2 + "\\" + OM.EqpStat.sSerialList + "\\";
            return s1;
        }

        public void InitCycle()
        {
            m_iCycle = 10;
            m_iPreCycle = 0;
            bErrDevice = false;
            sErrMsg = "";
        }

        public void Stop()
        {
            m_iCycle = 0;
        }

        public void Reset() { InitCycle(); }

        IntPtr m_iHwndM = IntPtr.Zero;
        IntPtr m_iHwnd1 = IntPtr.Zero; //휘발성
        IntPtr m_iHwnd2 = IntPtr.Zero; //휘발성
        IntPtr m_iHwnd3 = IntPtr.Zero; //휘발성
        IntPtr m_iHwnd4 = IntPtr.Zero; //휘발성

        IntPtr m_iHwndS = IntPtr.Zero;

        private string sSerial;

        //Settings에있는 캡션 찾는 Structure
        public struct CFindSetCptn             
        {
            //Macro Option
            public string sProductID    ;
            public int    iDescramble   ;//콤보박스 저장될때 item 순서로 되어있음
            public int    iDarkOffset   ;
            public int    iGain         ;
            public int    iBinning      ;
        } ;
        public static CFindSetCptn   FindSetCptn  ;

        public static void LoadSetCaption(string _sPath)
        {
            //TODO:
            //ini 파일 로드하는 부분 전부 싹 옵션으로 빼야함
            string sFindCaptionPath = _sPath;//"C:\\EzSensor" + "\\EzSensor.ini";
            
            CIniFile IniCaption = new CIniFile(sFindCaptionPath);

            IniCaption.Load("Settings", "PID"       , out FindSetCptn.sProductID );
            IniCaption.Load("Settings", "Descramble", out FindSetCptn.iDescramble);
            IniCaption.Load("Settings", "VReset"    , out FindSetCptn.iDarkOffset);
            IniCaption.Load("Settings", "GainMode"  , out FindSetCptn.iGain      );
            IniCaption.Load("Settings", "BinMode"   , out FindSetCptn.iBinning   );
        }
        
        //Ez, BI 공용으로 사용
        //입고 공정
        //시작 전 설정
        //입고 영상
        public string sCopyIniPath = "";
        public bool CycleEntering() //Setting
        {
            String sTemp;
            if (m_tmCycle.OnDelay(m_iCycle != 0 && m_iCycle == m_iPreCycle, 20000))
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                if (sErrMsg == "") sErrMsg = m_sPartName + "Macro Time Out Error " + sTemp;
                Trace(sErrMsg);
                m_iCycle = 0;
                return true;
            }

            if (m_iCycle != m_iPreCycle)
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                Trace(sTemp);
                m_tmDelay.Clear();
            }

            m_iHwnd1 = IntPtr.Zero;
            m_iHwnd2 = IntPtr.Zero;
            m_iHwnd3 = IntPtr.Zero;
            m_iHwnd4 = IntPtr.Zero;

            //EzSensor Class name
            string sClassName1 = "#32770";

            //EzSensor Tab Order
            int Settings     = 0;
            int Acquisition  = 1;
            int Calibration  = 2;
            int Preprocess   = 3;
            int ImageProcess = 4;
            int Records      = 5;
            int Aging        = 6;
            int Log_Traces   = 7;

            m_iPreCycle = m_iCycle;

            int  iDly = 700;
        
            switch (m_iCycle)
            {
        
                default:
                    sTemp = string.Format("Cycle_Default_Clear_m_iCycle={0:000}", m_iCycle);
                    sErrMsg = sTemp;
                    return true;
        
        
                case 0:
                    return true;
        
                case 10:
                    //INI 파일 체크
                    sCopyIniPath = "";
                    if (!File.Exists(OM.EzSensorInfo.sIniPath1))
                    {
                        sErrMsg = "ERR001_Ini파일을 찾을 수 없습니다.(원본)";
                        Trace(sErrMsg);
                        return true;
                    }
                    //INI 파일 지우기
                    string sPath = OM.EzSensorInfo.sIniPath2 + "\\";
                    string sName = Path.GetFileName(OM.EzSensorInfo.sIniPath1);
                    sCopyIniPath = sPath + sName;
                    if (File.Exists(sCopyIniPath))
                    {
                        File.Delete(sCopyIniPath);
                    }
                    //INI 파일 복사
                    File.Copy(OM.EzSensorInfo.sIniPath1, sCopyIniPath);
                    if ( OM.EzSensorInfo.iImgSize == 0 || OM.EzSensorInfo.iImgSize == 1 ||
                        (OM.EzSensorInfo.iImgSize == 2 && SEQ.XRYE.iWorkStat == cs.Entering1x1))
                    {
                        Result.Clear();
                    }

                    string sPath2 = Path.GetDirectoryName(OM.EzSensorInfo.sAppPath1) + "\\CAL" ;
                    DeleteFolder(sPath2);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 11:
                    //프로그램 종료
                    if (!ExitProcess(OM.EzSensorInfo.sAppName1)) return false; //TODO :: 잠시 삭제
                    
                    //프로그램 시작
                    InitProcess1(OM.EzSensorInfo.sAppPath1);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;
        
                case 12:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    if(sCopyIniPath == "")
                    {
                        sErrMsg = "ERR001_Ini파일을 찾을 수 없습니다.(사본)";
                        Trace(sErrMsg);
                        return true;
                    }
                    LoadSetCaption(sCopyIniPath); //INI 파일 읽기
                    //메인 핸들 받아오기
                    string sCaption = "IntraOral Detector " + "(PID " + FindSetCptn.sProductID + ")";
                    m_iHwndM = FindWindowL("#32770", sCaption);
                    if (!ExistProces(OM.EzSensorInfo.sAppName1) || m_iHwndM == IntPtr.Zero)
                    {
                        sErrMsg = "ERR002_프로그램이 실행되지 않았습니다.";
                        Trace(sErrMsg);
                        return true;
                    }
                    SetWindowPos(m_iHwndM, IntPtr.Zero, OM.EzSensorInfo.iPosX1, OM.EzSensorInfo.iPosY1, 0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);

                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 13: //시리얼 넘버 확인하는 부분
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    SetTabControl(m_iHwnd1, Records);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 14://Get Dark
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, sClassName1, "Records");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, Tools.Button, "Read");
                    Click(m_iHwnd2);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 15:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, sClassName1, "Records");
                    if(!OM.EzSensorInfo.bUseIOSPgm) OM.EqpStat.sCrntSerial = GetWindowText(m_iHwnd1, Tools.Edit, 2); //원래 사용하던 EzSensor 프로그램 시리얼넘버 에디트박스 순번
                    else                            OM.EqpStat.sCrntSerial = GetWindowText(m_iHwnd1, Tools.Edit, 4); //IOS폴더에서 사용하는 EzSensor 프로그램 시리얼넘버 에디트박스 순번
                    if (OM.EqpStat.sCrntSerial == "")
                    {
                        sErrMsg = "ERR003_시리얼 번호 확인 안됨.";
                        return true;
                    }
                    bDetectSerial = true;
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 16:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    if (!SEQ.XRYE.bFindSerialEnd) return false; //Ver 2019.10.23.6 시리얼넘버 매칭되기 전에 입력하는 경우도 있어서 FindSerialNo() 다 탈때까지 기다린다.
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, sClassName1, "Records");

                    if (!OM.EzSensorInfo.bUseIOSPgm) SetWindowText(m_iHwnd1, Tools.Edit, 1, OM.EqpStat.sSerialList);
                    else                             SetWindowText(m_iHwnd1, Tools.Edit, 3, OM.EqpStat.sSerialList);

                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 17:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, sClassName1, "Records");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, Tools.Button, "Record");
                    Click(m_iHwnd2);
                    m_tmDelay.Clear();
                    m_iCycle = 20;
                    return false;
        
                case 20: //Setting Tab에서 Full Frame 에디트 박스 가져옴
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, sClassName1, "Settings");
                    SetWindowText(m_iHwnd1, Tools.Edit, 2, Setting.iWidth .ToString());
                    SetWindowText(m_iHwnd1, Tools.Edit, 3, Setting.iHeight.ToString());
     
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 21:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, Tools.Button, "확인");
                    if (m_iHwnd1 == IntPtr.Zero) return false;

                    Click(m_iHwnd1); //Post 두번 보내는거 테스트 필요.
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;
        
                case 22:
                    if (!m_tmDelay.OnDelay(iDly)) return false;

                    InitProcess1(OM.EzSensorInfo.sAppPath1);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 23:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    if (sCopyIniPath == "")
                    {
                        sErrMsg = "ERR001_Ini파일을 찾을 수 없습니다.(사본)";
                        Trace(sErrMsg);
                        return true;
                    }
                    LoadSetCaption(sCopyIniPath); //INI 파일 읽기
                    //메인 핸들 받아오기
                    sCaption = "IntraOral Detector " + "(PID " + FindSetCptn.sProductID + ")";
                    m_iHwndM = FindWindowL("#32770", sCaption);
                    if (!ExistProces(OM.EzSensorInfo.sAppName1) || m_iHwndM == IntPtr.Zero)
                    {
                        sErrMsg = "ERR002_프로그램이 실행되지 않았습니다.";
                        Trace(sErrMsg);
                        return true;
                    }
                    SetWindowPos(m_iHwndM, IntPtr.Zero, OM.EzSensorInfo.iPosX1, OM.EzSensorInfo.iPosY1, 0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);

                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 24:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, sClassName1, "Settings"           );
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button"   , "Detector's settings");
                    if (m_iHwnd2 == IntPtr.Zero) return false;

                    Click(m_iHwnd2); //Post 두번 보내는거 테스트 필요.
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 25:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770", "Detector settings");
                    if (m_iHwnd1 == IntPtr.Zero) return false;

                    //SetComboBox
                    SetWindow    (m_iHwnd1, Tools.ComboBox, 0, OM.EzSensorInfo.iProductID );
                    SetWindow    (m_iHwnd1, Tools.ComboBox, 1, OM.EzSensorInfo.iDescramble);
                    SetWindow    (m_iHwnd1, Tools.ComboBox, 2, OM.EzSensorInfo.iVRest     );
                    SetWindow    (m_iHwnd1, Tools.ComboBox, 3, Setting        .iBinning   );//제어사이클에서 1x1, 2x2 분기한다. 진섭
                    SetWindow    (m_iHwnd1, Tools.ComboBox, 4, OM.EzSensorInfo.iMode      );
                    SetWindowText(m_iHwnd1, Tools.ComboBox, 5, OM.EqpStat     .sSerialList); 
                    SetWindow    (m_iHwnd1, Tools.ComboBox, 6, OM.EzSensorInfo.iPattern   );

                    //SetButton
                    SetWindow    (m_iHwnd1, Tools.Button, 4, OM.EzSensorInfo.bInvertacq ? 1 : 0);
                    SetWindow    (m_iHwnd1, Tools.Button, 7, OM.EzSensorInfo.bBright61  ? 1 : 0);
                    SetWindow    (m_iHwnd1, Tools.Button, 8, OM.EzSensorInfo.bDebugdump ? 1 : 0);

                    //SetEdit
                    //2019.11.19 신규 IOS 프로그램에서 On-the-fly-offset day<s> 항목 없어져서 옵션에 따라 따로 셋팅
                    if (!OM.EzSensorInfo.bUseIOSPgm)//기존 EzSensor 프로그램으로 사용 시
                    {
                        SetWindowText(m_iHwnd1, Tools.Edit, 0, OM.EzSensorInfo.iTimeout    .ToString());
                        SetWindowText(m_iHwnd1, Tools.Edit, 1, OM.EzSensorInfo.iOntheFly   .ToString());
                        SetWindowText(m_iHwnd1, Tools.Edit, 2, OM.EqpStat     .sDarkOfs    .ToString());
                        SetWindowText(m_iHwnd1, Tools.Edit, 3, OM.EzSensorInfo.iGain       .ToString());
                        SetWindowText(m_iHwnd1, Tools.Edit, 4, Setting        .iCutoffbevel.ToString());//제어사이클에서 1x1, 2x2 분기한다. 진섭
                        SetWindowText(m_iHwnd1, Tools.Edit, 5, OM.EzSensorInfo.iCutoffR    .ToString());
                    }
                    else //신규 IOS 프로그램으로 사용 시
                    {
                        SetWindowText(m_iHwnd1, Tools.Edit, 0, OM.EzSensorInfo.iTimeout    .ToString());
                        //SetWindowText(m_iHwnd1, Tools.Edit, 1, OM.EzSensorInfo.iOntheFly   .ToString());
                        SetWindowText(m_iHwnd1, Tools.Edit, 1, OM.EqpStat     .sDarkOfs    .ToString());
                        SetWindowText(m_iHwnd1, Tools.Edit, 2, OM.EzSensorInfo.iGain       .ToString());
                        SetWindowText(m_iHwnd1, Tools.Edit, 3, Setting        .iCutoffbevel.ToString());//제어사이클에서 1x1, 2x2 분기한다. 진섭
                        SetWindowText(m_iHwnd1, Tools.Edit, 4, OM.EzSensorInfo.iCutoffR    .ToString());
                    }

                    m_iCycle++;
                    return false;

                case 26: //셋팅하고 셋팅값 확인하기.
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770", "Detector settings");
                    if (m_iHwnd1 == IntPtr.Zero) return false;

                    //SetComboBox
                    if(GetWindow    (m_iHwnd1, Tools.ComboBox, 0) !=  OM.EzSensorInfo.iProductID    ) sErrMsg = "ProductID Setting Fail" ;
                    if(GetWindow    (m_iHwnd1, Tools.ComboBox, 1) !=  OM.EzSensorInfo.iDescramble   ) sErrMsg = "Descramble Setting Fail";
                    if(GetWindow    (m_iHwnd1, Tools.ComboBox, 2) !=  OM.EzSensorInfo.iVRest        ) sErrMsg = "VRest Setting Fail"     ;
                    if(GetWindow    (m_iHwnd1, Tools.ComboBox, 3) !=  Setting        .iBinning      ) sErrMsg = "Binning Setting Fail"   ;
                    if(GetWindow    (m_iHwnd1, Tools.ComboBox, 4) !=  OM.EzSensorInfo.iMode         ) sErrMsg = "Mode Setting Fail"      ;
                    if(GetWindowText(m_iHwnd1, Tools.ComboBox, 5) !=  OM.EqpStat     .sSerialList   ) sErrMsg = "SerialID Setting Fail"  ;
                    if(GetWindow    (m_iHwnd1, Tools.ComboBox, 6) !=  OM.EzSensorInfo.iPattern      ) sErrMsg = "Pattern Setting Fail"   ;

                    //SetButton
                    if(GetWindow(m_iHwnd1, Tools.Button, 4) !=  (OM.EzSensorInfo.bInvertacq ? 1 : 0)) sErrMsg = "Invertacq Setting Fail";
                    if(GetWindow(m_iHwnd1, Tools.Button, 7) !=  (OM.EzSensorInfo.bBright61  ? 1 : 0)) sErrMsg = "Bright61 Setting Fail" ;
                    if(GetWindow(m_iHwnd1, Tools.Button, 8) !=  (OM.EzSensorInfo.bDebugdump ? 1 : 0)) sErrMsg = "Debugdump Setting Fail";

                    //SetEdit
                    //2019.11.19 신규 IOS 프로그램에서 On-the-fly-offset day<s> 항목 없어져서 옵션에 따라 따로 셋팅
                    if (!OM.EzSensorInfo.bUseIOSPgm)//기존 EzSensor 프로그램으로 사용 시
                    {
                        if(GetWindowText(m_iHwnd1, Tools.Edit, 0) !=  OM.EzSensorInfo.iTimeout    .ToString()) sErrMsg = "Timeout Setting Fail"    ;
                        if(GetWindowText(m_iHwnd1, Tools.Edit, 1) !=  OM.EzSensorInfo.iOntheFly   .ToString()) sErrMsg = "OntheFly Setting Fail"   ;
                        if(GetWindowText(m_iHwnd1, Tools.Edit, 2) !=  OM.EqpStat     .sDarkOfs    .ToString()) sErrMsg = "DarkOffset Setting Fail" ;
                        if(GetWindowText(m_iHwnd1, Tools.Edit, 3) !=  OM.EzSensorInfo.iGain       .ToString()) sErrMsg = "Gain Setting Fail"       ;
                        if(GetWindowText(m_iHwnd1, Tools.Edit, 4) !=  Setting        .iCutoffbevel.ToString()) sErrMsg = "Cutoffbevel Setting Fail";
                        if(GetWindowText(m_iHwnd1, Tools.Edit, 5) !=  OM.EzSensorInfo.iCutoffR    .ToString()) sErrMsg = "CutoffR Setting Fail"    ;
                    }
                    else
                    {
                        if(GetWindowText(m_iHwnd1, Tools.Edit, 0) !=  OM.EzSensorInfo.iTimeout    .ToString()) sErrMsg = "Timeout Setting Fail"    ;
                        //if(GetWindowText(m_iHwnd1, Tools.Edit, 1) !=  OM.EzSensorInfo.iOntheFly   .ToString()) sErrMsg = "OntheFly Setting Fail"   ;
                        if(GetWindowText(m_iHwnd1, Tools.Edit, 1) !=  OM.EqpStat     .sDarkOfs    .ToString()) sErrMsg = "DarkOffset Setting Fail" ;
                        if(GetWindowText(m_iHwnd1, Tools.Edit, 2) !=  OM.EzSensorInfo.iGain       .ToString()) sErrMsg = "Gain Setting Fail"       ;
                        if(GetWindowText(m_iHwnd1, Tools.Edit, 3) !=  Setting        .iCutoffbevel.ToString()) sErrMsg = "Cutoffbevel Setting Fail";
                        if(GetWindowText(m_iHwnd1, Tools.Edit, 4) !=  OM.EzSensorInfo.iCutoffR    .ToString()) sErrMsg = "CutoffR Setting Fail"    ;
                    }
                    
                    if(sErrMsg != "")
                    {
                        Trace(sErrMsg);
                        return true;
                    }
                    m_iCycle++;
                    return false;

                case 27:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL(sClassName1, "Detector settings");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, Tools.Button, "OK");
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    Click(m_iHwnd2);
                    m_tmDelay.Clear();
                    m_iCycle = 30;
                    return false;

                case 30:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, Tools.Button, "확인");
                    if (m_iHwnd1 == IntPtr.Zero) return false;

                    Click(m_iHwnd1); //Post 두번 보내는거 테스트 필요.
                    m_tmDelay.Clear();
                    m_iCycle = 40;
                    return false;

                case 40:
                    sPath = Path.GetDirectoryName(OM.EzSensorInfo.sAppPath1) + "\\CAL" ;
                    DirectoryInfo dir = new DirectoryInfo(sPath);
                    if (dir.Exists) //읽기전용 파일들 찾아서 노멀로 바꿔주면 삭제 가능
                    {
                        FileInfo[] files = dir.GetFiles("*.*",

                        SearchOption.AllDirectories);

                        foreach (FileInfo file in files)
                        {
                            file.Attributes = FileAttributes.Normal;
                            file.Delete();
                        }
                    }
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 41:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    if (!ExitProcess(OM.EzSensorInfo.sAppName1)) return false; //TODO :: 잠시 삭제
                    InitProcess1(OM.EzSensorInfo.sAppPath1);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 42:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    if (sCopyIniPath == "")
                    {
                        sErrMsg = "ERR001_Ini파일을 찾을 수 없습니다.(사본)";
                        Trace(sErrMsg);
                        return true;
                    }
                    LoadSetCaption(sCopyIniPath);
                    sCaption = "IntraOral Detector " + "(PID " + FindSetCptn.sProductID + ")";
                    m_iHwndM = FindWindowL("#32770", sCaption);
                    if (!ExistProces(OM.EzSensorInfo.sAppName1) || m_iHwndM == IntPtr.Zero)
                    {
                        sErrMsg = "ERR002_프로그램이 실행되지 않았습니다.";
                        Trace(sErrMsg);
                        return true;
                    }
                    SetWindowPos(m_iHwndM, IntPtr.Zero, OM.EzSensorInfo.iPosX1, OM.EzSensorInfo.iPosY1, 0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);

                    m_tmDelay.Clear();
                    m_iCycle = 50;
                    return false;

                case 50://Calibration Tab 이동
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    SetTabControl(m_iHwnd1, Calibration);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 51://Get Dark
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "#32770", "Calibration");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "Get &Dark");

                    Click(m_iHwnd2);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 52:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwndS = FindWindowL("#32770", "Acquiring dark frame"); //캡션에 퍼센테이지가 올라가서 끝까지 유지가 안됨.
                    if (m_iHwndS == IntPtr.Zero) return false;
                    m_iCycle++;
                    return false;

                case 53:
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "button", null); //cancel 버튼이 하나 있음
                    if (m_iHwnd1 != IntPtr.Zero) return false;
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;


                case 54:
                    if (!m_tmDelay.OnDelay(2000)) return false;
                    sPath = Path.GetDirectoryName(OM.EzSensorInfo.sAppPath1) + "\\CAL" ;
                    sName = sPath + "\\dark.raw";
                    if (!File.Exists(sName)) //트리거 불량
                    {
                        sErrMsg = "ERR004_dark.raw 파일 생성 실패(Get Dark)";
                        Trace(sErrMsg);
                        return true;
                    }

                    m_tmDelay.Clear();
                    m_iCycle = 60;
                    return false;

                case 60: //Get Bright
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "#32770", "Calibration");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, Tools.Button, "Get &Bright");
                    Click(m_iHwnd2);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 61://지금 여기까지만 확인
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwndS = FindWindowL("#32770", "Acquiring bright frame");
                    if (m_iHwndS == IntPtr.Zero) return false;
                    m_iCycle++;
                    return false;

                case 62:
                    if (!m_tmDelay.OnDelay(1500)) return false;
                
                    SEQ.XrayCom.SetXrayPara(Setting.sKvp, Setting.smA, Setting.sTime);

                    m_iCycle++;
                    return false;

                case 63: //
                    if (!m_tmDelay.OnDelay(OM.EzSensorInfo.iGbDelay)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "button", null); //cancel 버튼이 하나 있음
                    if (m_iHwnd1 != IntPtr.Zero)
                    {
                        if (iRptCnt == OM.CmnOptn.iXrayRptCnt)
                        {
                            iRptCnt = 0;
                            sErrMsg = "트리거 신호가 나오지 않습니다.";
                            m_iCycle = 0;
                            return true;
                        }
                        if (OM.CmnOptn.iXrayRptCnt > 0)
                        {
                            iRptCnt++;
                            m_tmDelay.Clear();
                            m_iCycle = 62;
                            return false;
                        }
                        else if (OM.CmnOptn.iXrayRptCnt <= 0)
                        {
                            return false;
                        }
                    }
                    else if (m_iHwnd1 == IntPtr.Zero)
                    {
                        bErrDevice = false;
                        iRptCnt = 0;
                    }
                    
                    m_iCycle = 70;
                    return false;

                case 70:
                    string sOldName = "";
                    string sNewName = OM.EqpStat.sSerialList + ".raw";
                    sPath = Path.GetDirectoryName(OM.EzSensorInfo.sAppPath1) + "\\CAL" ;
                    if (Directory.Exists(sPath))
                    {
                        DirectoryInfo di1 = new DirectoryInfo(sPath);

                        foreach (var item in di1.GetFiles())
                        {
                            if (item.Name.IndexOf("x") == 0)
                            {
                                sOldName = item.Name;
                            }
                        }
                    }
                    if (!File.Exists(sPath + "\\" + sOldName))
                    {
                        sErrMsg = "ERR00D_Xxxxx.raw 파일을 찾을수 없습니다.";
                        Trace(sErrMsg);
                        return true; 
                    }
                    
                    string sOld = sPath + "\\" + sOldName;
                    string sNew = sPath + "\\" + sNewName;

                    File.Move(sOld, sNew);

                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 71:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    sPath = Path.GetDirectoryName(OM.EzSensorInfo.sAppPath1) + "\\CAL" ;
                    sNewName = sPath + "\\" + OM.EqpStat.sSerialList + ".raw";
                    if (!File.Exists(sNewName))
                    { 
                        sErrMsg = "ERR010_파일 이름이 변경되지 않았습니다.";
                        Trace(sErrMsg);
                        return true; 
                    }

                    m_iCycle++;
                    return false;

                case 72:
                    string sTemp1 = OM.EqpStat.sSerialList;
                    string sPath1 = "";
                    string sCalPath = Path.GetDirectoryName(OM.EzSensorInfo.sAppPath1) + "\\CAL";//CAL, CAL_A 원래 폴더
                    sPath1 = Path.GetDirectoryName(OM.EzSensorInfo.sAppPath1) + "\\" + sTemp1 + "\\" + Setting.sSaveFolder + "\\" + Setting.sCalFolder; //SerialNo 폴더

                    DirectoryInfo di = new DirectoryInfo(sPath1);
                    if (!di.Exists)
                    {
                        di.Create();
                    }
                    else
                    {
                        DeleteFolder(sPath1);
                        di.Create();
                    }
                    
                    CopyFolder(sCalPath, sPath1);
                    DeleteFolder(sCalPath);

                    m_iCycle = 0;
                    return true;
                
            }
        }

        //Ez, BI 공용으로 사용
        //Aging 공정
        public bool CycleAging() 
        {
            String sTemp;
            if (m_tmCycle.OnDelay(m_iCycle != 0 && m_iCycle == m_iPreCycle, 15000))
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                if (sErrMsg == "") sErrMsg = m_sPartName + "Macro Time Out Error " + sTemp;
                Trace(sErrMsg);
                m_iCycle = 0;
                return true;
            }

            if (m_iCycle != m_iPreCycle)
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                Trace(sTemp);
                m_tmDelay.Clear();
            }

            m_iHwnd1 = IntPtr.Zero;
            m_iHwnd2 = IntPtr.Zero;
            m_iHwnd3 = IntPtr.Zero;
            m_iHwnd4 = IntPtr.Zero;

            //EzSensor Class name
            string sClassName1 = "#32770";

            //EzSensor Tab Order
            int Settings     = 0;
            int Acquisition  = 1;
            int Calibration  = 2;
            int Preprocess   = 3;
            int ImageProcess = 4;
            int Records      = 5;
            int Aging        = 6;
            int Log_Traces   = 7;

            m_iPreCycle = m_iCycle;

            int iDly = 700;

            switch (m_iCycle)
            {

                default:
                    sTemp = string.Format("Cycle_Default_Clear_m_iCycle={0:000}", m_iCycle);
                    sErrMsg = sTemp;
                    return true;


                case 0:
                    return true;

                //입고영상
                case 10:
                    //INI파일 읽어오기
                    if (sCopyIniPath == "")
                    {
                        sErrMsg = "ERR001_Ini파일을 찾을 수 없습니다.(사본)";
                        Trace(sErrMsg);
                        return true;
                    }
                    LoadSetCaption(sCopyIniPath);
                    //메인 핸들 얻어오기
                    string sCaption = "IntraOral Detector " + "(PID " + FindSetCptn.sProductID + ")";
                    m_iHwndM = FindWindowL("#32770", sCaption);
                    if (ExistProces(OM.EzSensorInfo.sAppName1) || m_iHwndM != IntPtr.Zero)
                    {
                        m_iCycle = 20;
                        return false;
                    }

                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 11:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    if (!ExitProcess(OM.EzSensorInfo.sAppName1)) return false; //TODO :: 잠시 삭제
                    InitProcess1(OM.EzSensorInfo.sAppPath1);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 12:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    if (sCopyIniPath == "")
                    {
                        sErrMsg = "ERR001_Ini파일을 찾을 수 없습니다.(사본)";
                        Trace(sErrMsg);
                        return true;
                    }
                    LoadSetCaption(sCopyIniPath);
                    sCaption = "IntraOral Detector " + "(PID " + FindSetCptn.sProductID + ")";
                    m_iHwndM = FindWindowL("#32770", sCaption);
                    if (!ExistProces(OM.EzSensorInfo.sAppName1) || m_iHwndM == IntPtr.Zero)
                    {
                        sErrMsg = "ERR002_프로그램이 실행되지 않았습니다.";
                        Trace(sErrMsg);
                        return true;
                    }
                    SetWindowPos(m_iHwndM, IntPtr.Zero, OM.EzSensorInfo.iPosX1, OM.EzSensorInfo.iPosY1, 0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);

                    m_tmDelay.Clear();
                    m_iCycle = 20;
                    return false;

                case 20://Calibration Tab 이동
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    SetTabControl(m_iHwnd1, Settings);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 21://Get Dark
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "#32770", "Settings");
                    if (m_iHwnd1 == IntPtr.Zero) return false;

                    SetWindowText(m_iHwnd1, Tools.Edit    , 2 , Setting        .iWidth  .ToString()); //제어쪽에서 2x2 이면 바꿀수 있도록
                    SetWindowText(m_iHwnd1, Tools.Edit    , 3 , Setting        .iHeight .ToString()); //제어쪽에서 2x2 이면 바꿀수 있도록
                    SetWindow    (m_iHwnd1, Tools.ComboBox, 1 , OM.EzSensorInfo.iAgRotate              );
                    SetWindow    (m_iHwnd1, Tools.Button  , 9 , OM.EzSensorInfo.bAgFlipHorz ? 1 : 0    );
                    SetWindow    (m_iHwnd1, Tools.Button  , 10, OM.EzSensorInfo.bAgFlipVert ? 1 : 0    );
                    SetWindowText(m_iHwnd1, Tools.Edit    , 4 , OM.EzSensorInfo.iAgCropTop  .ToString());
                    SetWindowText(m_iHwnd1, Tools.Edit    , 5 , OM.EzSensorInfo.iAgCropLeft .ToString());
                    SetWindowText(m_iHwnd1, Tools.Edit    , 6 , OM.EzSensorInfo.iAgCropRight.ToString());
                    SetWindowText(m_iHwnd1, Tools.Edit    , 7 , OM.EzSensorInfo.iAgCropBtm  .ToString());

                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 22: 
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "#32770", "Settings");
                    if (m_iHwnd1 == IntPtr.Zero) return false;

                    if(GetWindowText(m_iHwnd1, Tools.Edit    , 2 ) !=  Setting        .iWidth  .ToString()) sErrMsg = "Full Frame Width Setting Fail" ;
                    if(GetWindowText(m_iHwnd1, Tools.Edit    , 3 ) !=  Setting        .iHeight .ToString()) sErrMsg = "Full Frame Height Setting Fail";
                    if(GetWindow    (m_iHwnd1, Tools.ComboBox, 1 ) !=  OM.EzSensorInfo.iAgRotate              ) sErrMsg = "Rotate Setting Fail"           ;
                    if(GetWindow    (m_iHwnd1, Tools.Button  , 9 ) != (OM.EzSensorInfo.bAgFlipHorz ? 1 : 0   )) sErrMsg = "Flip Horz Setting Fail"        ;
                    if(GetWindow    (m_iHwnd1, Tools.Button  , 10) != (OM.EzSensorInfo.bAgFlipVert ? 1 : 0   )) sErrMsg = "Flip Vert Setting Fail"        ;
                    if(GetWindowText(m_iHwnd1, Tools.Edit    , 4 ) !=  OM.EzSensorInfo.iAgCropTop  .ToString()) sErrMsg = "Crop Top Setting Fail"         ;
                    if(GetWindowText(m_iHwnd1, Tools.Edit    , 5 ) !=  OM.EzSensorInfo.iAgCropLeft .ToString()) sErrMsg = "Crop Left Setting Fail"        ;
                    if(GetWindowText(m_iHwnd1, Tools.Edit    , 6 ) !=  OM.EzSensorInfo.iAgCropRight.ToString()) sErrMsg = "Crop Right Setting Fail"       ;
                    if(GetWindowText(m_iHwnd1, Tools.Edit    , 7 ) !=  OM.EzSensorInfo.iAgCropBtm  .ToString()) sErrMsg = "Crop Bottom Setting Fail"      ;
                     
                    if(sErrMsg != "")
                    {
                        Trace(sErrMsg);
                        return true;
                    }
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 23:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "#32770", "Settings");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button"   , "Detector's settings");
                    if (m_iHwnd2 == IntPtr.Zero) return false;

                    Click(m_iHwnd2); //Post 두번 보내는거 테스트 필요.
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 24:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770", "Detector settings");
                    if (m_iHwnd1 == IntPtr.Zero) return false;

                    SetWindow(m_iHwnd1, Tools.ComboBox, 3, Setting.iBinning);//제어사이클에서 1x1, 2x2 분기한다. 진섭

                    m_iCycle++;
                    return false;

                case 25: //셋팅하고 셋팅값 확인하기.
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770", "Detector settings");
                    if (m_iHwnd1 == IntPtr.Zero) return false;

                    if(GetWindow    (m_iHwnd1, Tools.ComboBox, 3) !=  Setting.iBinning) sErrMsg = "Binning Setting Fail"   ;

                    if(sErrMsg != "")
                    {
                        Trace(sErrMsg);
                        return true;
                    }
                    m_iCycle++;
                    return false;

                case 26:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL(sClassName1, "Detector settings");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, Tools.Button, "OK");
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    Click(m_iHwnd2);
                    m_tmDelay.Clear();
                    m_iCycle = 30;
                    return false;

                case 30: // Get Dark
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    SetTabControl(m_iHwnd1, Calibration);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 31://Get Dark
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, sClassName1, "Calibration");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, Tools.Button, "Get &Dark");
                    Click(m_iHwnd2);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 32:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwndS = FindWindowL("#32770", "Acquiring dark frame"); //캡션에 퍼센테이지가 올라가서 끝까지 유지가 안됨.
                    if (m_iHwndS == IntPtr.Zero) return false;
                    m_iCycle++;
                    return false;

                case 33:
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "button", null); //cancel 버튼이 하나 있음
                    if (m_iHwnd1 != IntPtr.Zero) return false;
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;


                case 34:
                    if (!m_tmDelay.OnDelay(2000)) return false;
                    string sPath = Path.GetDirectoryName(OM.EzSensorInfo.sAppPath1) + "\\CAL" ;
                    string sName = sPath + "\\dark.raw";
                    if (!File.Exists(sName)) //트리거 불량
                    {
                        sErrMsg = "ERR004_dark.raw 파일 생성 실패(Get Dark)";
                        Trace(sErrMsg);
                        return true;
                    }
                    m_tmDelay.Clear();
                    m_iCycle = 40;
                    return false;

                case 40:
                    if (!ExitProcess(OM.EzSensorInfo.sAppName2)) return false; //
                    InitProcess2(OM.EzSensorInfo.sAppPath2);

                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 41: //
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwndS = FindWindowL("#32770", "Noise Evaluation for Factory [ver 1.0]");
                    if (!ExistProces(OM.EzSensorInfo.sAppName2) || m_iHwndS == IntPtr.Zero)
                    {
                        sErrMsg = "ERR002_프로그램이 실행되지 않았습니다.";
                        Trace(sErrMsg);
                        return true;
                    }
                    SetWindowPos(m_iHwndS, IntPtr.Zero, OM.EzSensorInfo.iPosX2, OM.EzSensorInfo.iPosY2, 0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);

                    m_iCycle++;
                    return false;

                case 42://
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "#32770", "ElectronicNoisePerformance");
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    SetWindowText(m_iHwnd1, Tools.Edit, 1, Setting.iWidth .ToString());
                    SetWindowText(m_iHwnd1, Tools.Edit, 2, Setting.iHeight.ToString());

                    m_iCycle++;
                    return false;

                case 43:
                    if (!m_tmDelay.OnDelay(true, iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "#32770", "ElectronicNoisePerformance");
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    sPath = Path.GetDirectoryName(OM.EzSensorInfo.sAppPath1) + "\\CAL" ;

                    SetWindowText(m_iHwnd1, Tools.Edit, 7, sPath);
                    m_iCycle++;
                    return false;

                case 44:
                    if (!m_tmDelay.OnDelay(true, iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "#32770", "ElectronicNoisePerformance");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "RUN");
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    Click(m_iHwnd2);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 45:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    //m_iHwnd1 = FindWindowL("#32770", "ENP_Inspection");
                    //m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Static", "확인");
                    //if (GetWindowText(m_iHwnd1, Tools.Static, 0).IndexOf("Complete") == -1)
                    //{
                    //    sErrMsg = "ERR008_ENP_Inspection 프로그램에서 결과데이터를 가져오지 못했습니다.";
                    //    Trace(sErrMsg);
                    //    return true;
                    //}

                    m_iCycle++;
                    return false;

                case 46:
                    m_iHwnd1 = FindWindowL("#32770", "ENP_Inspection");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Static", "확인");
                    if (m_tmDelay.OnDelay(GetWindowText(m_iHwnd1, Tools.Static, 0).IndexOf("Complete") == -1, 12000))
                    {
                        sErrMsg = "ERR008_ENP_Inspection 프로그램에서 결과데이터를 가져오지 못했습니다.";
                        Trace(sErrMsg);
                        return true;
                    }
                    if (GetWindowText(m_iHwnd1, Tools.Static, 0).IndexOf("Complete") == -1) return false;
                    
                    m_iCycle++;
                    return false;

                case 47:
                    if (!m_tmDelay.OnDelay(true, iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770", "ENP_Inspection");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "확인");
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    Click(m_iHwnd2);
                    m_iCycle++;
                    return false;

                case 48: //확인 취소창이 없어졋는지 확인.
                    if (!m_tmDelay.OnDelay(true, iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770", "ENP_Inspection");
                    if (m_iHwnd1 != IntPtr.Zero) return false;
                    m_iCycle++;
                    return false;

                case 49:
                    if (!m_tmDelay.OnDelay(true, iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "#32770", "ElectronicNoisePerformance");
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    string sTemp1 = GetWindowText(m_iHwnd1, Tools.Edit, 13); //메디안 가져오는부분

                    if (sErrMsg != "")
                    {
                        Trace(sErrMsg);
                        return true;
                    }

                    m_iCycle++;
                    return false;

                case 50:
                    if (!m_tmDelay.OnDelay(true, iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "Button", "취소");
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    Click(m_iHwnd1);
                    m_tmDelay.Clear();
                    m_iCycle = 60; //60 진섭
                    return false;

                case 60:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    SetTabControl(m_iHwnd1, Aging);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 61:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "#32770", "Aging");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button"   , "Purge");
                    Click(m_iHwnd2);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 62:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770", "IntraOral Detector");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "확인");
                    Click(m_iHwnd2);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 63://Max Frame Interval 넣는 부분
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "#32770", "Aging");
                    SetWindowText(m_iHwnd1, Tools.Edit, 0, Setting.iAcqMaxFrame.ToString());
                    SetWindowText(m_iHwnd1, Tools.Edit, 1, Setting.iAcqInterval.ToString());

                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 64:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "#32770", "Aging");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button"   , "Active");
                    Click(m_iHwnd2, 1);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 65:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "#32770", "Aging");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Static", "not active");
                    m_iHwnd3 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Static", "connecting");
                    m_iHwnd4 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Static", "acquiring");
                    if (m_iHwnd2 != IntPtr.Zero)
                    {
                        m_tmDelay.Clear();
                        m_iCycle = 64;
                        return false;
                    }
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 66:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "#32770", "Aging");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Static", "not active");
                    m_iHwnd3 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Static", "connecting");
                    m_iHwnd4 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Static", "acquiring");
                    if (m_iHwnd3 != IntPtr.Zero || m_iHwnd4 != IntPtr.Zero)
                    {
                        m_tmDelay.Clear();
                        m_iCycle++;
                        return false;
                    }
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 67:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "#32770", "Aging");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Static", "not active");
                    m_iHwnd3 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Static", "connecting");
                    m_iHwnd4 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Static", "acquiring");
                    if (m_iHwnd3 != IntPtr.Zero || m_iHwnd4 != IntPtr.Zero)
                    {
                        m_tmDelay.Clear();
                        m_iCycle = 66;
                        return false;
                    }
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    m_tmDelay.Clear();
                    m_iCycle = 70;
                    return false;

                case 70:
                    if (!ExitProcess(OM.EzSensorInfo.sAppName2)) return false; //
                    InitProcess2(OM.EzSensorInfo.sAppPath2);

                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 71: //
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwndS = FindWindowL("#32770", "Noise Evaluation for Factory [ver 1.0]");
                    if (!ExistProces(OM.EzSensorInfo.sAppName2) || m_iHwndS == IntPtr.Zero)
                    {
                        sErrMsg = "ERR002_프로그램이 실행되지 않았습니다.";
                        Trace(sErrMsg);
                        return true;
                    }
                    SetWindowPos(m_iHwndS, IntPtr.Zero, OM.EzSensorInfo.iPosX2, OM.EzSensorInfo.iPosY2, 0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);
                    m_tmDelay.Clear();
                    m_iCycle = 80;
                    return false;

                case 80: //Noise 탭핸들에서 Width Height 변경
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "#32770", "ElectronicNoisePerformance");
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    SetWindowText(m_iHwnd1, Tools.Edit, 1, Setting.iWidth .ToString());
                    SetWindowText(m_iHwnd1, Tools.Edit, 2, Setting.iHeight.ToString());
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 81:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "#32770", "ElectronicNoisePerformance");
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    SetWindowText(m_iHwnd1, Tools.Edit, 7, Setting.sDarkImgPath);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 82:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "#32770", "ElectronicNoisePerformance");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "RUN");
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    Click(m_iHwnd2);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 83:
                    if (!m_tmDelay.OnDelay(6000)) return false;
                    m_iHwnd1 = FindWindowL("#32770", "ENP_Inspection");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Static", "확인");
                    if (m_tmDelay.OnDelay(GetWindowText(m_iHwnd1, Tools.Static, 0).IndexOf("Complete") == -1, 12000))
                    {
                        sErrMsg = "ERR008_ENP_Inspection 프로그램에서 결과데이터를 가져오지 못했습니다.";
                        Trace(sErrMsg);
                        return true;
                    }
                    if (GetWindowText(m_iHwnd1, Tools.Static, 0).IndexOf("Complete") == -1) return false;
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 84:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770", "ENP_Inspection");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "확인");
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    Click(m_iHwnd2);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 85: //확인 취소창이 없어졋는지 확인.
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770", "ENP_Inspection");
                    if (m_iHwnd1 != IntPtr.Zero) return false;
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 86:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "#32770", "ElectronicNoisePerformance");
                    if (m_iHwnd1 == IntPtr.Zero) return false;

                    if (SEQ.XRYE.iWorkStat == cs.Aging1x1)
                    {
                        Result.LineNoise1x1    = GetWindowText(m_iHwnd1, Tools.Edit, 11);
                        Result.DataNoise1x1    = GetWindowText(m_iHwnd1, Tools.Edit, 12);
                        Result.Median1x1       = GetWindowText(m_iHwnd1, Tools.Edit, 13);
                        Result.Fluc1x1         = GetWindowText(m_iHwnd1, Tools.Edit, 14);
                        Result.Min1x1          = GetWindowText(m_iHwnd1, Tools.Edit, 15);
                        Result.Max1x1          = GetWindowText(m_iHwnd1, Tools.Edit, 16);
                        Result.Dispersion1x1   = GetWindowText(m_iHwnd1, Tools.Edit, 17);
                        Result.DynamicRange1x1 = GetWindowText(m_iHwnd1, Tools.Edit, 19);
                        Result.TotalNoise1x1   = GetWindowText(m_iHwnd1, Tools.Edit, 8 );

                        //소수점 둘째자리까지 표시 되도록 변환 
                        double dLineNoise1x1   ;
                        double dDataNoise1x1   ;
                        double dMedian1x1      ;
                        double dFluc1x1        ;
                        double dMin1x1         ;
                        double dMax1x1         ;
                        double dDispersion1x1  ;
                        double dDynamicRange1x1;
                        double dTotalNoise1x1  ;

                        if (double.TryParse(Result.LineNoise1x1, out dLineNoise1x1))
                        {
                            Result.LineNoise1x1 = string.Format("{0:####.##}", dLineNoise1x1);
                        }
                        if (double.TryParse(Result.DataNoise1x1, out dDataNoise1x1))
                        {
                            Result.DataNoise1x1 = string.Format("{0:####.##}", dDataNoise1x1);
                        }
                        if (double.TryParse(Result.Median1x1, out dMedian1x1))
                        {
                            Result.Median1x1 = string.Format("{0:####.##}", dMedian1x1);
                        }
                        if (double.TryParse(Result.Fluc1x1, out dFluc1x1))
                        {
                            Result.Fluc1x1 = string.Format("{0:####.##}", dFluc1x1);
                        }
                        if (double.TryParse(Result.Min1x1, out dMin1x1))
                        {
                            Result.Min1x1 = string.Format("{0:####.##}", dMin1x1);
                        }
                        if (double.TryParse(Result.Max1x1, out dMax1x1))
                        {
                            Result.Max1x1 = string.Format("{0:####.##}", dMax1x1);
                        }
                        if (double.TryParse(Result.Dispersion1x1, out dDispersion1x1))
                        {
                            Result.Dispersion1x1 = string.Format("{0:####.##}", dDispersion1x1);
                        }
                        if (double.TryParse(Result.DynamicRange1x1, out dDynamicRange1x1))
                        {
                            Result.DynamicRange1x1 = string.Format("{0:####.##}", dDynamicRange1x1);
                        }
                        if (double.TryParse(Result.TotalNoise1x1, out dTotalNoise1x1))
                        {
                            Result.TotalNoise1x1 = string.Format("{0:####.##}", dTotalNoise1x1);
                        }
                    }
                    else if (SEQ.XRYE.iWorkStat == cs.Aging2x2)
                    {
                        Result.LineNoise2x2    = GetWindowText(m_iHwnd1, Tools.Edit, 11);
                        Result.DataNoise2x2    = GetWindowText(m_iHwnd1, Tools.Edit, 12);
                        Result.Median2x2       = GetWindowText(m_iHwnd1, Tools.Edit, 13);
                        Result.Fluc2x2         = GetWindowText(m_iHwnd1, Tools.Edit, 14);
                        Result.Min2x2          = GetWindowText(m_iHwnd1, Tools.Edit, 15);
                        Result.Max2x2          = GetWindowText(m_iHwnd1, Tools.Edit, 16);
                        Result.Dispersion2x2   = GetWindowText(m_iHwnd1, Tools.Edit, 17);
                        Result.DynamicRange2x2 = GetWindowText(m_iHwnd1, Tools.Edit, 19);
                        Result.TotalNoise2x2   = GetWindowText(m_iHwnd1, Tools.Edit, 8 );

                        //소수점 둘째자리까지 표시 되도록 변환 
                        double dLineNoise2x2;
                        double dDataNoise2x2;
                        double dMedian2x2;
                        double dFluc2x2;
                        double dMin2x2;
                        double dMax2x2;
                        double dDispersion2x2;
                        double dDynamicRange2x2;
                        double dTotalNoise2x2;

                        if (double.TryParse(Result.LineNoise2x2, out dLineNoise2x2))
                        {
                            Result.LineNoise2x2 = string.Format("{0:####.##}", dLineNoise2x2);
                        }
                        if (double.TryParse(Result.DataNoise2x2, out dDataNoise2x2))
                        {
                            Result.DataNoise2x2 = string.Format("{0:####.##}", dDataNoise2x2);
                        }
                        if (double.TryParse(Result.Median2x2, out dMedian2x2))
                        {
                            Result.Median2x2 = string.Format("{0:####.##}", dMedian2x2);
                        }
                        if (double.TryParse(Result.Fluc2x2, out dFluc2x2))
                        {
                            Result.Fluc2x2 = string.Format("{0:####.##}", dFluc2x2);
                        }
                        if (double.TryParse(Result.Min2x2, out dMin2x2))
                        {
                            Result.Min2x2 = string.Format("{0:####.##}", dMin2x2);
                        }
                        if (double.TryParse(Result.Max2x2, out dMax2x2))
                        {
                            Result.Max2x2 = string.Format("{0:####.##}", dMax2x2);
                        }
                        if (double.TryParse(Result.Dispersion2x2, out dDispersion2x2))
                        {
                            Result.Dispersion2x2 = string.Format("{0:####.##}", dDispersion2x2);
                        }
                        if (double.TryParse(Result.DynamicRange2x2, out dDynamicRange2x2))
                        {
                            Result.DynamicRange2x2 = string.Format("{0:####.##}", dDynamicRange2x2);
                        }
                        if (double.TryParse(Result.TotalNoise2x2, out dTotalNoise2x2))
                        {
                            Result.TotalNoise2x2 = string.Format("{0:####.##}", dTotalNoise2x2);
                        }
                    }

                    //SaveCsv();
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 87:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "Button", "확인");
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    Click(m_iHwnd1);
                    m_tmDelay.Clear();
                    m_iCycle = 0;
                    return true;
            }
        }

        //Ez, BI 공용으로 사용
        //특성검사
        //Calibration
        //Get Bright
        public bool CycleGetBright()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(m_iCycle != 0 && m_iCycle == m_iPreCycle, 20000))
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                if (sErrMsg == "") sErrMsg = m_sPartName + "Macro Time Out Error " + sTemp;
                Trace(sErrMsg);
                m_iCycle = 0;
                return true;
            }

            if (m_iCycle != m_iPreCycle)
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                Trace(sTemp);
                m_tmDelay.Clear();
            }

            m_iHwnd1 = IntPtr.Zero;
            m_iHwnd2 = IntPtr.Zero;
            m_iHwnd3 = IntPtr.Zero;
            m_iHwnd4 = IntPtr.Zero;

            //EzSensor Class name
            string sClassName1 = "#32770";

            //EzSensor Tab Order
            int Settings     = 0;
            int Acquisition  = 1;
            int Calibration  = 2;
            int Preprocess   = 3;
            int ImageProcess = 4;
            int Records      = 5;
            int Aging        = 6;
            int Log_Traces   = 7;

            m_iPreCycle = m_iCycle;

            int iDly = 700;
            //double dInspDely ;

            switch (m_iCycle)
            {

                default:
                    sTemp = string.Format("Cycle_Default_Clear_m_iCycle={0:000}", m_iCycle);
                    sErrMsg = sTemp;
                    return true;


                case 0:
                    return true;

                //입고영상
                case 10:
                    string sCalPath = Path.GetDirectoryName(OM.EzSensorInfo.sAppPath1) + "\\CAL" ;

                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 11:
                    if (sCopyIniPath == "")
                    {
                        sErrMsg = "ERR001_Ini파일을 찾을 수 없습니다.(사본)";
                        Trace(sErrMsg);
                        return true;
                    }
                    LoadSetCaption(sCopyIniPath);
                    string sCaption = "IntraOral Detector " + "(PID " + FindSetCptn.sProductID + ")";
                    m_iHwndM = FindWindowL("#32770", sCaption);
                    if (ExistProces(OM.EzSensorInfo.sAppName1) || m_iHwndM != IntPtr.Zero)
                    {
                        m_iCycle = 20;
                        return false;
                    }

                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 12:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    if (!ExitProcess(OM.EzSensorInfo.sAppName1)) return false; //TODO :: 잠시 삭제
                    InitProcess1(OM.EzSensorInfo.sAppPath1);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 13:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    LoadSetCaption(sCopyIniPath);
                    sCaption = "IntraOral Detector " + "(PID " + FindSetCptn.sProductID + ")";
                    m_iHwndM = FindWindowL("#32770", sCaption);
                    if (!ExistProces(OM.EzSensorInfo.sAppName1) || m_iHwndM == IntPtr.Zero)
                    {
                        sErrMsg = "ERR002_프로그램이 실행되지 않았습니다.";
                        Trace(sErrMsg);
                        return true;
                    }
                    
                    SetWindowPos(m_iHwndM, IntPtr.Zero, OM.EzSensorInfo.iPosX1, OM.EzSensorInfo.iPosY1, 0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);

                    m_tmDelay.Clear();
                    m_iCycle = 20;
                    return false;

                case 20://Calibration Tab 이동
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    SetTabControl(m_iHwnd1, Calibration);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 21://Get Dark
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    string sPath = Path.GetDirectoryName(OM.EzSensorInfo.sAppPath1) + "\\CAL" ;
                    if (Directory.Exists(sPath))
                    {
                        DirectoryInfo di1 = new DirectoryInfo(sPath);

                        foreach (var item in di1.GetFiles())
                        {
                            if (item.Name.IndexOf("dark") == 0)
                            {
                                m_tmDelay.Clear();
                                m_iCycle = 30;
                                return false;
                            }
                        }
                    }

                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, sClassName1, "Calibration");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, Tools.Button, "Get &Dark");
                    Click(m_iHwnd2);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 22:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwndS = FindWindowL("#32770", "Acquiring dark frame"); //캡션에 퍼센테이지가 올라가서 끝까지 유지가 안됨.
                    if (m_iHwndS == IntPtr.Zero) return false;
                    m_iCycle++;
                    return false;

                case 23:
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "button", null); //cancel 버튼이 하나 있음
                    if (m_iHwnd1 != IntPtr.Zero) return false;
                    m_tmDelay.Clear();
                    m_iCycle = 30;
                    return false;

                case 30: //Get Bright
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "#32770", "Calibration");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, Tools.Button, "Get &Bright");
                    Click(m_iHwnd2);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 31://지금 여기까지만 확인
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwndS = FindWindowL("#32770", "Acquiring bright frame");
                    if (m_iHwndS == IntPtr.Zero) return false;
                    m_iCycle++;
                    return false;

                case 32:
                    if (!m_tmDelay.OnDelay(1500)) return false;

                    SEQ.XrayCom.SetXrayPara(Setting.sKvp, Setting.smA, Setting.sTime);

                    m_iCycle++;
                    return false;

                case 33: //
                    if (!m_tmDelay.OnDelay(OM.EzSensorInfo.iGbDelay)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "button", null); //cancel 버튼이 하나 있음
                    if (m_iHwnd1 != IntPtr.Zero)
                    {
                        if (iRptCnt == OM.CmnOptn.iXrayRptCnt)
                        {
                            iRptCnt = 0;
                            sErrMsg = "트리거 신호가 나오지 않습니다.";
                            m_iCycle = 0;
                            return true;
                        }
                        if (OM.CmnOptn.iXrayRptCnt > 0)
                        {
                            iRptCnt++;
                            m_tmDelay.Clear();
                            m_iCycle = 32;
                            return false;
                        }
                        else if (OM.CmnOptn.iXrayRptCnt <= 0)
                        {
                            return false;
                        }
                    }
                    else if (m_iHwnd1 == IntPtr.Zero)
                    {
                        bErrDevice = false;
                        iRptCnt = 0;
                    }

                    m_iCycle = 70;
                    return false;

                case 70:
                    string sFileName = "";
                    sPath = Path.GetDirectoryName(OM.EzSensorInfo.sAppPath1) + "\\CAL" ;
                    if (Directory.Exists(sPath))
                    {
                        DirectoryInfo di1 = new DirectoryInfo(sPath);

                        foreach (var item in di1.GetFiles())
                        {
                            if (item.Name.IndexOf("x") == 0)
                            {
                                sFileName = item.Name;
                            }
                        }
                    }
                    if (!File.Exists(sPath + "\\" + sFileName)) 
                    {
                        sErrMsg = "ERR00D_Xxxxx.raw 파일을 찾을수 없습니다.";
                        Trace(sErrMsg);
                        return true; 
                    }

                    m_iCycle++;
                    return false;

                case 71: //영상 받을때마다 ini 파일 복사
                    string sTemp1 = OM.EqpStat.sSerialList;
                    string sPath2 = Path.GetDirectoryName(OM.EzSensorInfo.sAppPath1) + "\\" + sTemp1 + "\\" + Setting.sSaveFolder + "\\" ; //SerialNo 폴더
                    
                    //INI 파일 지우기
                    sPath = OM.EzSensorInfo.sIniPath2 + "\\";           //원본 파일이고 시리얼넘버 폴더에 넣는다.
                    string sName = Path.GetFileName(OM.EzSensorInfo.sIniPath1);//원본 파일이고 시리얼넘버 폴더에 넣는다.
                    if (!File.Exists(OM.EzSensorInfo.sIniPath2 + "\\" + sName))
                    {
                        sErrMsg = "ERR001_Ini파일을 찾을 수 없습니다.(사본)";
                        Trace(sErrMsg);
                        return true;
                    }
                    if (File.Exists(sPath2 + "\\" + sName))
                    {
                        File.Delete(sPath2 + "\\" + sName);
                    }
                    //INI 파일 복사
                    File.Copy(sPath + sName, sPath2 + "\\" + sName);

                    m_iCycle++;
                    return false;

                case 72:
                     

                    m_iCycle = 0;
                    return true;
            }
        }

        //Ez, BI 공용으로 사용
        //특성검사
        //Acquisition Tab Get Image
        public bool CycleGetImage()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(m_iCycle != 0 && m_iCycle == m_iPreCycle, 20000))
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                if (sErrMsg == "") sErrMsg = m_sPartName + "Macro Time Out Error " + sTemp;
                Trace(sErrMsg);
                m_iCycle = 0;
                return true;
            }

            if (m_iCycle != m_iPreCycle)
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                Trace(sTemp);
                m_tmDelay.Clear();
            }

            m_iHwnd1 = IntPtr.Zero;
            m_iHwnd2 = IntPtr.Zero;
            m_iHwnd3 = IntPtr.Zero;
            m_iHwnd4 = IntPtr.Zero;

            //EzSensor Class name
            string sClassName1 = "#32770";

            //EzSensor Tab Order
            int Settings = 0;
            int Acquisition = 1;
            int Calibration = 2;
            int Preprocess = 3;
            int ImageProcess = 4;
            int Records = 5;
            int Aging = 6;
            int Log_Traces = 7;

            m_iPreCycle = m_iCycle;

            int iDly = 700;
            //double dInspDely;

            switch (m_iCycle)
            {

                default:
                    sTemp = string.Format("Cycle_Default_Clear_m_iCycle={0:000}", m_iCycle);
                    sErrMsg = sTemp;
                    return true;


                case 0:
                    return true;

                //입고영상
                case 10:
                    if (sCopyIniPath == "")
                    {
                        sErrMsg = "ERR001_Ini파일을 찾을 수 없습니다.(사본)";
                        Trace(sErrMsg);
                        return true;
                    }
                    LoadSetCaption(sCopyIniPath);
                    string sCaption = "IntraOral Detector " + "(PID " + FindSetCptn.sProductID + ")";
                    m_iHwndM = FindWindowL("#32770", sCaption);
                    if (ExistProces(OM.EzSensorInfo.sAppName1) || m_iHwndM != IntPtr.Zero)
                    {
                        m_iCycle = 20;
                        return false;
                    }

                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 11:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    if (!ExitProcess(OM.EzSensorInfo.sAppName1)) return false; //TODO :: 잠시 삭제
                    InitProcess1(OM.EzSensorInfo.sAppPath1);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 12:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    if (sCopyIniPath == "")
                    {
                        sErrMsg = "ERR001_Ini파일을 찾을 수 없습니다.(사본)";
                        Trace(sErrMsg);
                        return true;
                    }
                    LoadSetCaption(sCopyIniPath);
                    sCaption = "IntraOral Detector " + "(PID " + FindSetCptn.sProductID + ")";
                    m_iHwndM = FindWindowL("#32770", sCaption);
                    if (!ExistProces(OM.EzSensorInfo.sAppName1) || m_iHwndM == IntPtr.Zero)
                    {
                        sErrMsg = "ERR002_프로그램이 실행되지 않았습니다.";
                        Trace(sErrMsg);
                        return true;
                    }
                    SetWindowPos(m_iHwndM, IntPtr.Zero, OM.EzSensorInfo.iPosX1, OM.EzSensorInfo.iPosY1, 0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);

                    m_tmDelay.Clear();
                    m_iCycle = 20;
                    return false;

                case 20://Calibration Tab 이동
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    SetTabControl(m_iHwnd1, Acquisition);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 21:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, sClassName1, "Acquisition");
                    SetWindowText(m_iHwnd1, Tools.Edit, 4, Setting.sFileName);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 22:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, sClassName1, "Acquisition");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, Tools.Button, "&Get Image"); //동작중일때 버튼 캡션이 Generating...으로 바뀜
                    Click(m_iHwnd2);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 23:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwndS = FindWindowL("#32770", "Acquiring bright frame"); //캡션에 퍼센테이지가 올라가서 끝까지 유지가 안됨.
                    if (m_iHwndS == IntPtr.Zero) return false;
                    m_iCycle++;
                    return false;

                case 24://Xray 조사하는거 넣어야함
                    if (!m_tmDelay.OnDelay(1500)) return false;

                    SEQ.XrayCom.SetXrayPara(Setting.sKvp, Setting.smA, Setting.sTime);

                    m_iCycle++;
                    return false;

                case 25: //
                    if (!m_tmDelay.OnDelay(OM.EzSensorInfo.iGbDelay)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "button", null); //cancel 버튼이 하나 있음
                    if (m_iHwnd1 != IntPtr.Zero)
                    {
                        if (iRptCnt == OM.CmnOptn.iXrayRptCnt)
                        {
                            iRptCnt = 0;
                            sErrMsg = "트리거 신호가 나오지 않습니다.";
                            m_iCycle = 0;
                            return true;
                        }
                        if (OM.CmnOptn.iXrayRptCnt > 0)
                        {
                            iRptCnt++;
                            m_tmDelay.Clear();
                            m_iCycle = 24;
                            return false;
                        }
                        else if (OM.CmnOptn.iXrayRptCnt <= 0)
                        {
                            return false;
                        }
                    }
                    else if (m_iHwnd1 == IntPtr.Zero)
                    {
                        bErrDevice = false;
                        iRptCnt = 0;
                    }

                    m_tmDelay.Clear();
                    m_iCycle = 30;
                    return false;

                case 30://폴더 카피하는 부분
                    string sFdName = ""; //원래 저장되어있는 폴더
                    int iWidthCrop = OM.EzSensorInfo.iAgCropLeft + OM.EzSensorInfo.iAgCropRight;
                    int iHghtCrop  = OM.EzSensorInfo.iAgCropTop  + OM.EzSensorInfo.iAgCropBtm  ;
                    if (OM.EzSensorInfo.iAgRotate == 0) //이미지 안돌림
                    {
                        sFdName = "I" + string.Format("{0:0000}", Setting.iWidth - iWidthCrop) + "X" + string.Format("{0:0000}", Setting.iHeight - iHghtCrop);
                    }
                    else if (OM.EzSensorInfo.iAgRotate == 1 || OM.EzSensorInfo.iAgRotate == 2) //90도 돌려서
                    {
                        sFdName = "I" + string.Format("{0:0000}", Setting.iHeight - iHghtCrop) + "X" + string.Format("{0:0000}", Setting.iWidth - iWidthCrop);
                    }
            
                    string sPath1 = Path.GetDirectoryName(OM.EzSensorInfo.sAppPath1) + "\\" + sFdName; //원래 폴더
                    string sPath2 = Path.GetDirectoryName(OM.EzSensorInfo.sAppPath1) + "\\" + OM.EqpStat.sSerialList + "\\" + Setting.sGetImgFdName; //변경할 폴더 이름
                    DirectoryInfo di1 = new DirectoryInfo(sPath1);
                    DirectoryInfo di2 = new DirectoryInfo(sPath2);
                    
                    if (!di2.Exists)
                    {
                        di2.Create();
                    }
                    if (di1.Exists) 
                    {
                        System.IO.FileInfo[] files = di1.GetFiles("*.*",
                    
                        System.IO.SearchOption.AllDirectories);
                    
                        foreach (System.IO.FileInfo file in files)
                        {
                            file.Attributes = System.IO.FileAttributes.Normal;
                        }
                        
                    }
                    string sFileName = "\\" + Setting.sFileName + ".raw";
                    FileInfo Fi1 = new System.IO.FileInfo(sPath2 + sFileName); //시리얼넘버 폴더에 파일 있으면 지운다
                    if (Fi1.Exists)
                    {
                        Fi1.Delete();
                    }
                    File.Copy(sPath1 + sFileName, sPath2 + sFileName);//파일 카피
                    DeleteFolder(sPath1);

                    m_iCycle = 0;
                    return true;
            }
        }

        //Ez, BI 공용으로 사용
        //특성검사
        //Calibration Generate 버튼 클릭
        //원본 CAL폴더 C:\\EzSensor\\시리얼넘버\\CAL 카피하고 지운다.
        public bool CycleCalGen()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(m_iCycle != 0 && m_iCycle == m_iPreCycle, 7000))
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                if (sErrMsg == "") sErrMsg = m_sPartName + "Macro Time Out Error " + sTemp;
                Trace(sErrMsg);
                m_iCycle = 0;
                return true;
            }

            if (m_iCycle != m_iPreCycle)
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                Trace(sTemp);
                m_tmDelay.Clear();
            }

            m_iHwnd1 = IntPtr.Zero;
            m_iHwnd2 = IntPtr.Zero;
            m_iHwnd3 = IntPtr.Zero;
            m_iHwnd4 = IntPtr.Zero;

            //EzSensor Class name
            string sClassName1 = "#32770";

            //EzSensor Tab Order
            int Settings = 0;
            int Acquisition = 1;
            int Calibration = 2;
            int Preprocess = 3;
            int ImageProcess = 4;
            int Records = 5;
            int Aging = 6;
            int Log_Traces = 7;

            m_iPreCycle = m_iCycle;



            int iDly = 700;

            switch (m_iCycle)
            {

                default:
                    sTemp = string.Format("Cycle_Default_Clear_m_iCycle={0:000}", m_iCycle);
                    sErrMsg = sTemp;
                    return true;


                case 0:
                    return true;


                case 10:
                    if (sCopyIniPath == "")
                    {
                        sErrMsg = "ERR001_Ini파일을 찾을 수 없습니다.(사본)";
                        Trace(sErrMsg);
                        return true;
                    }
                    LoadSetCaption(sCopyIniPath);
                    string sCaption = "IntraOral Detector " + "(PID " + FindSetCptn.sProductID + ")";
                    m_iHwndM = FindWindowL("#32770", sCaption);
                    if (ExistProces(OM.EzSensorInfo.sAppName1) || m_iHwndM != IntPtr.Zero)
                    {
                        m_iCycle = 20;
                        return false;
                    }

                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 11:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    if (!ExitProcess(OM.EzSensorInfo.sAppName1)) return false; //TODO :: 잠시 삭제
                    InitProcess1(OM.EzSensorInfo.sAppPath1);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 12:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    if (sCopyIniPath == "")
                    {
                        sErrMsg = "ERR001_Ini파일을 찾을 수 없습니다.(사본)";
                        Trace(sErrMsg);
                        return true;
                    }
                    LoadSetCaption(sCopyIniPath);
                    sCaption = "IntraOral Detector " + "(PID " + FindSetCptn.sProductID + ")";
                    m_iHwndM = FindWindowL("#32770", sCaption);
                    if (!ExistProces(OM.EzSensorInfo.sAppName1) || m_iHwndM == IntPtr.Zero)
                    {
                        sErrMsg = "ERR002_프로그램이 실행되지 않았습니다.";
                        Trace(sErrMsg);
                        return true;
                    }
                    SetWindowPos(m_iHwndM, IntPtr.Zero, OM.EzSensorInfo.iPosX1, OM.EzSensorInfo.iPosY1, 0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);

                    m_tmDelay.Clear();
                    m_iCycle = 20;
                    return false;

                case 20://Calibration Tab 이동
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    SetTabControl(m_iHwnd1, Calibration);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;


                case 21:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, sClassName1, "Calibration");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, Tools.Button, "&Generate");
                    Click(m_iHwnd2);

                    //이 뒤에는 Generate 버튼이 활성화가 안되서 가서 해야할듯. 진섭
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 22:

                    m_tmDelay.Clear();
                    m_iCycle = 0;
                    return true;
            }
        }

        //Ez 사용
        //특성검사
        //PreProcess Tab Generate Auto BPM 클릭
        public bool CyclePreGen()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(m_iCycle != 0 && m_iCycle == m_iPreCycle, 7000))
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                if (sErrMsg == "") sErrMsg = m_sPartName + "Macro Time Out Error " + sTemp;
                Trace(sErrMsg);
                m_iCycle = 0;
                return true;
            }

            if (m_iCycle != m_iPreCycle)
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                Trace(sTemp);
                m_tmDelay.Clear();
            }

            m_iHwnd1 = IntPtr.Zero;
            m_iHwnd2 = IntPtr.Zero;
            m_iHwnd3 = IntPtr.Zero;
            m_iHwnd4 = IntPtr.Zero;

            //EzSensor Class name
            string sClassName1 = "#32770";

            //EzSensor Tab Order
            int Settings     = 0;
            int Acquisition  = 1;
            int Calibration  = 2;
            int Preprocess   = 3;
            int ImageProcess = 4;
            int Records      = 5;
            int Aging        = 6;
            int Log_Traces   = 7;

            m_iPreCycle = m_iCycle;

            int iDly = 700;

            switch (m_iCycle)
            {

                default:
                    sTemp = string.Format("Cycle_Default_Clear_m_iCycle={0:000}", m_iCycle);
                    sErrMsg = sTemp;
                    return true;


                case 0:
                    return true;

                //입고영상
                case 10:
                    if (sCopyIniPath == "")
                    {
                        sErrMsg = "ERR001_Ini파일을 찾을 수 없습니다.(사본)";
                        Trace(sErrMsg);
                        return true;
                    }
                    LoadSetCaption(sCopyIniPath);
                    string sCaption = "IntraOral Detector " + "(PID " + FindSetCptn.sProductID + ")";
                    m_iHwndM = FindWindowL("#32770", sCaption);
                    if (ExistProces(OM.EzSensorInfo.sAppName1) || m_iHwndM != IntPtr.Zero)
                    {
                        m_iCycle = 20;
                        return false;
                    }

                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 11:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    if (!ExitProcess(OM.EzSensorInfo.sAppName1)) return false; //TODO :: 잠시 삭제
                    InitProcess1(OM.EzSensorInfo.sAppPath1);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 12:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    if (sCopyIniPath == "")
                    {
                        sErrMsg = "ERR001_Ini파일을 찾을 수 없습니다.(사본)";
                        Trace(sErrMsg);
                        return true;
                    }
                    LoadSetCaption(sCopyIniPath);
                    sCaption = "IntraOral Detector " + "(PID " + FindSetCptn.sProductID + ")";
                    m_iHwndM = FindWindowL("#32770", sCaption);
                    if (!ExistProces(OM.EzSensorInfo.sAppName1) || m_iHwndM == IntPtr.Zero)
                    {
                        sErrMsg = "ERR002_프로그램이 실행되지 않았습니다.";
                        Trace(sErrMsg);
                        return true;
                    }
                    SetWindowPos(m_iHwndM, IntPtr.Zero, OM.EzSensorInfo.iPosX1, OM.EzSensorInfo.iPosY1, 0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);

                    m_tmDelay.Clear();
                    m_iCycle = 20;
                    return false;

                case 20://Calibration Tab 이동
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    SetTabControl(m_iHwnd1, Preprocess);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 21://Get Dark
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, sClassName1, "Preprocess");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, Tools.Button, "&Generate BPM");
                    Click(m_iHwnd2);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 22:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, sClassName1, "Preprocess");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, Tools.Button, "Generating..."); //동작중일때 버튼 캡션이 Generating...으로 바뀜
                    if (m_iHwnd2 != IntPtr.Zero)
                    {
                        m_tmDelay.Clear();
                        m_iCycle++;
                        return false;
                    }
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 23:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, sClassName1, "Preprocess");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, Tools.Button, "Generating..."); //동작중일때 버튼 캡션이 Generating...으로 바뀜
                    m_iHwnd3 = FindWindowChild(m_iHwnd1, IntPtr.Zero, Tools.Button, "&Generate BPM"); //동작중일때 버튼 캡션이 Generating...으로 바뀜
                    if (m_iHwnd2 != IntPtr.Zero)
                    {
                        m_tmDelay.Clear();
                        m_iCycle = 22;
                        return false;
                    }
                    if (m_iHwnd3 == IntPtr.Zero) return false;
                    m_tmDelay.Clear();

                    m_iCycle++;
                    return false;

                case 24:
                    string sTemp1 = "";
                    
                    sTemp1 = OM.EqpStat.sSerialList;
                    string sFileName1 = "";

                    string sCalPath = Path.GetDirectoryName(OM.EzSensorInfo.sAppPath1) + "\\CAL" ;//CAL, CAL_A 원래 폴더
                    string sSerialPath = System.IO.Path.GetDirectoryName(OM.EzSensorInfo.sAppPath1) + "\\" + sTemp1 + "\\" + Setting.sSaveFolder + "\\" + Setting.sCalFolder; //SerialNo 폴더
                    string sDelPath = "";

                    DirectoryInfo di = new DirectoryInfo(sSerialPath);
                    if (!di.Exists)
                    {
                        di.Create();
                    }

                    CopyFolder(sCalPath, sSerialPath); //폴더에 있는거 일단 카피 해놓고 지운다.

                    
                    
                    m_iCycle = 0;
                    return true;
            }
        }
        
        //Ez, BI 공용
        //특성검사
        //DQE by IEC62220-1 프로그램 실행
        //처음 Setting Tab에서 MTF Tab까지 하는 동작
        public bool CycleDQE1()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(m_iCycle != 0 && m_iCycle == m_iPreCycle, 7000))
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                if (sErrMsg == "") sErrMsg = m_sPartName + "Macro Time Out Error " + sTemp;
                Trace(sErrMsg);
                m_iCycle = 0;
                return true;
            }

            if (m_iCycle != m_iPreCycle)
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                Trace(sTemp);
                m_tmDelay.Clear();
            }

            m_iHwnd1 = IntPtr.Zero;
            m_iHwnd2 = IntPtr.Zero;
            m_iHwnd3 = IntPtr.Zero;
            m_iHwnd4 = IntPtr.Zero;

            //EzSensor Class name
            string sClassName1 = "#32770";

            //DQE by IEC 62220-1 Tab Order
            int Settings = 0;
            int NPS      = 1;
            int MTF1     = 2;
            int MTF2     = 3;
            int DQE      = 4;
            int About    = 5;

            m_iPreCycle = m_iCycle;

            int iDly = 700;

            switch (m_iCycle)
            {

                default:
                    sTemp = string.Format("Cycle_Default_Clear_m_iCycle={0:000}", m_iCycle);
                    sErrMsg = sTemp;
                    return true;

                case 0:
                    return true;

                //입고영상
                case 10:
                    if (!ExitProcess(OM.EzSensorInfo.sAppName3)) return false; //
                    InitProcess2(OM.EzSensorInfo.sAppPath3);

                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 11: //
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwndS = FindWindowL("#32770", "DQE by IEC 62220-1");
                    if (!ExistProces(OM.EzSensorInfo.sAppName3) || m_iHwndS == IntPtr.Zero)
                    {
                        sErrMsg = "ERR002_프로그램이 실행되지 않았습니다.";
                        Trace(sErrMsg);
                        return true;
                    }
                    SetWindowPos(m_iHwndS, IntPtr.Zero, OM.EzSensorInfo.iPosX3, OM.EzSensorInfo.iPosY3, 0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);
                    m_tmDelay.Clear();
                    m_iCycle = 20;
                    return false;

                case 20: //Noise 탭핸들에서 Width Height 변경
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "SysTabControl32", null);
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    SetTabControl(m_iHwnd1, Settings);
                    
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 21: //Settings Tab
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "#32770", "Settings");
                    if (m_iHwnd1 == IntPtr.Zero) return false;

                    string sPath1 = Path.GetDirectoryName(OM.EzSensorInfo.sAppPath1) + "\\" + OM.EqpStat.sSerialList + "\\" + Setting.sFlatPath; //변경할 폴더 이름
                    string sPath2 = Path.GetDirectoryName(OM.EzSensorInfo.sAppPath1) + "\\" + OM.EqpStat.sSerialList + "\\" + Setting.sObjtPath; //변경할 폴더 이름
                    //Ver 2019.10.23.7 원래 레이언스에서 셋팅해주기로 한 경로인데 영상획득 프로그램 이원화 되면서 직접 셋팅해줘야 정상동작 해서 넣어줌
                    string sPath3 = Path.GetDirectoryName(OM.EzSensorInfo.sAppPath1) + "\\chart";

                    SetWindowText(m_iHwnd1, Tools.Edit, 1 , sPath1);
                    SetWindowText(m_iHwnd1, Tools.Edit, 2 , sPath2);
                    //Ver 2019.10.23.7 원래 레이언스에서 셋팅해주기로 한 경로인데 영상획득 프로그램 이원화 되면서 직접 셋팅해줘야 정상동작 해서 넣어줌
                    SetWindowText(m_iHwnd1, Tools.Edit, 4 , sPath3);

                    int iCropWidth = OM.EzSensorInfo.iAgCropLeft + OM.EzSensorInfo.iAgCropRight;
                    int iCropHght  = OM.EzSensorInfo.iAgCropTop  + OM.EzSensorInfo.iAgCropBtm  ;

                    SetWindowText(m_iHwnd1, Tools.Edit, 9 , Setting.iDimWidth.ToString());
                    SetWindowText(m_iHwnd1, Tools.Edit, 10, Setting.iDimHght .ToString());
                    
                    SetWindowText(m_iHwnd1, Tools.Edit, 11, Setting.dPixelPitch.ToString());
                 
                    m_tmDelay.Clear();
                    m_iCycle = 30;
                    return false;

                case 30: //NPS (Flat Frames) tab
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "SysTabControl32", null);
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    SetTabControl(m_iHwnd1, NPS);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;
                
                case 31:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "#32770", "NPS (Flat Frames)");
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    SetWindowText(m_iHwnd1, Tools.Edit, 1 , Setting.iNPSLeft.ToString());
                    SetWindowText(m_iHwnd1, Tools.Edit, 2 , Setting.iNPSTop .ToString());
                    SetWindowText(m_iHwnd1, Tools.Edit, 3 , Setting.iNPSW   .ToString());
                    SetWindowText(m_iHwnd1, Tools.Edit, 4 , Setting.iNPSH   .ToString());

                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;
                
                case 32:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "#32770", "NPS (Flat Frames)");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "Calculate NPS");
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    Click(m_iHwnd2);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;
                
                case 33: //Calculate NPS 누르면 대화상자 뜬다는데 실체를 확인할길이 없어서 나중에 확인하면서 한다. 진섭
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770", "DQE by IEC 62220-1");
                    if (m_iHwnd1 == m_iHwndS) return false;
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "확인");
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    Click(m_iHwnd2, 1);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;
                
                case 34:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "#32770", "NPS (Flat Frames)");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "Save Text");
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    Click(m_iHwnd2, 1);
                    m_tmDelay.Clear();
                    m_iCycle = 40;
                    return false;

                case 40: //MTF (Object) tab
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "SysTabControl32", null);
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    SetTabControl(m_iHwnd1, MTF1);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;
                
                case 41:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "#32770", "");
                    SetWindowText(m_iHwnd1, Tools.Edit, 1 , Setting.iMTFLeft.ToString());
                    SetWindowText(m_iHwnd1, Tools.Edit, 2 , Setting.iMTFTop .ToString());
                    SetWindowText(m_iHwnd1, Tools.Edit, 3 , Setting.iMTFW   .ToString());
                    SetWindowText(m_iHwnd1, Tools.Edit, 4 , Setting.iMTFH   .ToString());

                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 42:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "#32770", "");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "Calculate MTF");
                   if (m_iHwnd2 == IntPtr.Zero) return false;
                    Click(m_iHwnd2, 1);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;
                
                case 43: //Calculate NPS 누르면 대화상자 뜬다는데 실체를 확인할길이 없어서 나중에 확인하면서 한다. 진섭
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770", "DQE by IEC 62220-1");
                    if (m_iHwnd1 == m_iHwndS) return false;
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "확인");
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    Click(m_iHwnd2, 1);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;
                
                case 44:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "#32770", "");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "Save Text");
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    Click(m_iHwnd2, 1);
                    m_tmDelay.Clear();
                    m_iCycle = 0;
                    return true;
            }
        }

        //Ez 사용
        //특성검사
        //DQE by IEC62220-1 프로그램 실행
        //마지막 DQE Tab에서 하는 동작들
        public bool CycleDQE2()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(m_iCycle != 0 && m_iCycle == m_iPreCycle, 7000))
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                if (sErrMsg == "") sErrMsg = m_sPartName + "Macro Time Out Error " + sTemp;
                Trace(sErrMsg);
                m_iCycle = 0;
                return true;
            }

            if (m_iCycle != m_iPreCycle)
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                Trace(sTemp);
                m_tmDelay.Clear();
            }

            m_iHwnd1 = IntPtr.Zero;
            m_iHwnd2 = IntPtr.Zero;
            m_iHwnd3 = IntPtr.Zero;
            m_iHwnd4 = IntPtr.Zero;

            //EzSensor Class name
            string sClassName1 = "#32770";

            //DQE by IEC 62220-1 Tab Order
            int Settings = 0;
            int NPS      = 1;
            int MTF1     = 2;
            int MTF2     = 3;
            int DQE      = 4;
            int About    = 5;

            m_iPreCycle = m_iCycle;

            int iDly = 700;

            switch (m_iCycle)
            {

                default:
                    sTemp = string.Format("Cycle_Default_Clear_m_iCycle={0:000}", m_iCycle);
                    sErrMsg = sTemp;
                    return true;


                case 0:
                    return true;

                //입고영상
                case 10:
                    m_iHwndS = FindWindowL("#32770", "DQE by IEC 62220-1");
                    if (ExistProces(OM.EzSensorInfo.sAppName3) || m_iHwndS != IntPtr.Zero)
                    {
                        m_iCycle = 20;
                        return false;
                    }

                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 11:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    if (!ExitProcess(OM.EzSensorInfo.sAppName3)) return false; //TODO :: 잠시 삭제
                    InitProcess2(OM.EzSensorInfo.sAppPath3);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 12:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwndS = FindWindowL("#32770", "DQE by IEC 62220-1");
                    if (!ExistProces(OM.EzSensorInfo.sAppName3) || m_iHwndS == IntPtr.Zero)
                    {
                        sErrMsg = "ERR002_프로그램이 실행되지 않았습니다.";
                        Trace(sErrMsg);
                        return true;
                    }
                    SetWindowPos(m_iHwndM, IntPtr.Zero, OM.EzSensorInfo.iPosX3, OM.EzSensorInfo.iPosY3, 0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);

                    m_tmDelay.Clear();
                    m_iCycle = 20;
                    return false;

                case 20: //MTF (Object) tab
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "SysTabControl32", null);
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    SetTabControl(m_iHwnd1, DQE);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 21:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "#32770", "DQE");
                   if (m_iHwnd1 == IntPtr.Zero) return false;
                    SetWindowText(m_iHwnd1,Tools.Edit,2,OM.EzSensorInfo.dDoze.ToString());

                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 22:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "#32770", "DQE");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "Calculate DQE");
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    Click(m_iHwnd2, 1);
                    m_tmDelay.Clear();
                    m_iCycle = 30;
                    return false;
                
                case 30:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "#32770", "DQE");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "Save Text");
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    Click(m_iHwnd2, 1);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 31:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "Button", "확인");
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    Click(m_iHwnd1);
                    m_tmDelay.Clear();
                    m_iCycle = 40;
                    return false;

                case 40:
                    string sPath = Path.GetDirectoryName(OM.EzSensorInfo.sAppPath1) + "\\chart\\DQEchart.txt";
                    string textValue = File.ReadAllText(sPath);

                    //여기 나중에 확인해봐야함 진섭
                    if (SEQ.XRYE.iWorkStat == cs.MTFNPS1x1)
                    {
                        //SNR
                        int iTemp1 = textValue.IndexOf("(") + 1;
                        Result.SNR1x1 = textValue.Substring(iTemp1, 4);
                        //Doze
                        int iTemp2  = textValue.IndexOf("Doze") + 5;
                        Result.Doze1x1 = textValue.Substring(iTemp2, 4);
                        //Sens
                        int iTemp3  = textValue.IndexOf("Sens") + 5;
                        Result.Sens1x1 = textValue.Substring(iTemp3, 4);
                    }
                    else if (SEQ.XRYE.iWorkStat == cs.MTFNPS2x2)
                    {
                        //SNR
                        int iTemp1 = textValue.IndexOf("(") + 1;
                        Result.SNR2x2 = textValue.Substring(iTemp1, 4);
                        //Doze
                        int iTemp2  = textValue.IndexOf("Doze") + 5;
                        Result.Doze2x2 = textValue.Substring(iTemp2, 4);
                        //Sens
                        int iTemp3  = textValue.IndexOf("Sens") + 5;
                        Result.Sens2x2 = textValue.Substring(iTemp3, 4);
                    }

                    m_iCycle++;
                    return false;

                case 41:
                    sPath = System.IO.Path.GetDirectoryName(OM.EzSensorInfo.sAppPath1) + "\\chart\\MTFchart.txt";//GetRsltFd() + "CHART\\DQE.txt";
                    textValue = System.IO.File.ReadAllText(sPath);

                    double dTemp1 = 0.0;
                    double dTemp2 = 0.0;
                    double dTemp3 = 0.0;

                    if (SEQ.XRYE.iWorkStat == cs.MTFNPS1x1)
                    {
                        //3lpmm
                        int    iTemp1    = textValue.IndexOf("func value:") + 55;       //해당 데이터 텍스트 시작 위치
                        string s3lpmm1x1 = textValue.Substring(iTemp1, 5);              //서브스트링으로 데이터 자른다.
                        if (double.TryParse(s3lpmm1x1, out dTemp1))                     //string -> double
                        {
                            double d3lpmm1x1 = dTemp1 * 100;                            //변환한 double형 데이터에 곱하기 100(백분위로 변경)
                            Result.s3lpmm1x1 = string.Format("{0:####.##}", d3lpmm1x1); //결과값에 string 형으로 변환해서 넣는다.
                        }
                        //6lpmm
                        int    iTemp2    = iTemp1 + 48;
                        string s6lpmm1x1 = textValue.Substring(iTemp2, 5);        //서브스트링으로 데이터 자른다.
                        if (double.TryParse(s6lpmm1x1, out dTemp2))
                        {
                            double d6lpmm1x1 = dTemp2 * 100;
                            Result.s6lpmm1x1 = string.Format("{0:####.##}", d6lpmm1x1);
                        }
                        //8lpmm
                        int    iTemp3    = iTemp2 + 32;
                        string s8lpmm1x1 = textValue.Substring(iTemp3, 5);        //서브스트링으로 데이터 자른다.
                        if (double.TryParse(s8lpmm1x1, out dTemp3))
                        {
                            double d8lpmm1x1 = dTemp3 * 100;
                            Result.s8lpmm1x1 = string.Format("{0:####.##}", d8lpmm1x1);
                        }
                    }
                    else if (SEQ.XRYE.iWorkStat == cs.MTFNPS2x2)
                    {
                        //3lpmm
                        int    iTemp1    = textValue.IndexOf("func value:") + 55;
                        string s3lpmm2x2 = textValue.Substring(iTemp1, 5);        //서브스트링으로 데이터 자른다.
                        if (double.TryParse(s3lpmm2x2, out dTemp1))
                        {
                            double d3lpmm2x2 = dTemp1 * 100;
                            Result.s3lpmm2x2 = string.Format("{0:####.##}", d3lpmm2x2);
                        }
                        //6lpmm
                        int    iTemp2    = iTemp1 + 48;
                        string s6lpmm2x2 = textValue.Substring(iTemp2, 5);        //서브스트링으로 데이터 자른다.
                        if (double.TryParse(s6lpmm2x2, out dTemp2))
                        {
                            double d6lpmm2x2 = dTemp2 * 100;
                            Result.s6lpmm2x2 = string.Format("{0:####.##}", d6lpmm2x2);
                        }
                        //8lpmm
                        int    iTemp3    = iTemp2 + 32;
                        string s8lpmm2x2 = textValue.Substring(iTemp3, 5);        //서브스트링으로 데이터 자른다.
                        if (double.TryParse(s8lpmm2x2, out dTemp3))
                        {
                            double d8lpmm2x2 = dTemp3 * 100;
                            Result.s8lpmm2x2 = string.Format("{0:####.##}", d8lpmm2x2);
                        }
                    }
                    //if (SEQ.XRYE.iWorkStat == cs.MTFNPS1x1)
                    //{
                    //    //3lpmm
                    //    int    iTemp1    = textValue.IndexOf("func value:") + 55;
                    //    Result.s3lpmm1x1 = textValue.Substring(iTemp1, 5);
                    //    //6lpmm
                    //    int    iTemp2    = iTemp1 + 48;
                    //    Result.s6lpmm1x1 = textValue.Substring(iTemp2, 5);
                    //    //8lpmm
                    //    int    iTemp3    = iTemp2 + 32;
                    //    Result.s8lpmm1x1 = textValue.Substring(iTemp3, 5);
                    //}
                    //else if (SEQ.XRYE.iWorkStat == cs.MTFNPS2x2)
                    //{
                    //    //3lpmm
                    //    int    iTemp1    = textValue.IndexOf("func value:") + 55;
                    //    Result.s3lpmm2x2 = textValue.Substring(iTemp1, 5);
                    //    //6lpmm
                    //    int    iTemp2    = iTemp1 + 48;
                    //    Result.s6lpmm2x2 = textValue.Substring(iTemp2, 5);
                    //    //8lpmm
                    //    int    iTemp3    = iTemp2 + 32;
                    //    Result.s8lpmm2x2 = textValue.Substring(iTemp3, 5);
                    //}

                    m_iCycle++;
                    return false;

                case 42: //chart 폴더 Serial 폴더에 옮김
                    sPath = Path.GetDirectoryName(OM.EzSensorInfo.sAppPath1) + "\\chart";
                    string sTemp1 = OM.EqpStat.sSerialList;
                    string sSerialPath = Path.GetDirectoryName(OM.EzSensorInfo.sAppPath1) + "\\" + sTemp1 + "\\" + Setting.sSaveFolder +"\\chart";
                    CopyFolder(sPath, sSerialPath);

                    DirectoryInfo dir = new DirectoryInfo(sPath);
                    if (dir.Exists) //읽기전용 파일들 찾아서 노멀로 바꿔주면 삭제 가능
                    {
                        FileInfo[] files = dir.GetFiles("*.*",
                    
                        SearchOption.AllDirectories);

                        foreach (FileInfo file in files)
                        {
                            file.Attributes = FileAttributes.Normal;
                            file.Delete();
                        }
                    }
                    m_iCycle = 0;
                    return true;
            }
        }

        //Ez 사용
        //Calibration Trigger
        public bool CycleTrigger()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(m_iCycle != 0 && m_iCycle == m_iPreCycle, 20000))
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                if (sErrMsg == "") sErrMsg = m_sPartName + "Macro Time Out Error " + sTemp;
                Trace(sErrMsg);
                m_iCycle = 0;
                return true;
            }

            if (m_iCycle != m_iPreCycle)
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                Trace(sTemp);
                m_tmDelay.Clear();
            }

            m_iHwnd1 = IntPtr.Zero;
            m_iHwnd2 = IntPtr.Zero;
            m_iHwnd3 = IntPtr.Zero;
            m_iHwnd4 = IntPtr.Zero;

            //EzSensor Class name
            string sClassName1 = "#32770";

            //EzSensor Tab Order
            int Settings     = 0;
            int Acquisition  = 1;
            int Calibration  = 2;
            int Preprocess   = 3;
            int ImageProcess = 4;
            int Records      = 5;
            int Aging        = 6;
            int Log_Traces   = 7;

            m_iPreCycle = m_iCycle;

            int iDly = 700;
            //double dInspDely = 0 ;

            switch (m_iCycle)
            {

                default:
                    sTemp = string.Format("Cycle_Default_Clear_m_iCycle={0:000}", m_iCycle);
                    sErrMsg = sTemp;
                    return true;


                case 0:
                    return true;

                //입고영상
                case 10:
                    if (sCopyIniPath == "")
                    {
                        sErrMsg = "ERR001_Ini파일을 찾을 수 없습니다.(사본)";
                        Trace(sErrMsg);
                        return true;
                    }
                    LoadSetCaption(sCopyIniPath);
                    string sCaption = "IntraOral Detector " + "(PID " + FindSetCptn.sProductID + ")";
                    m_iHwndM = FindWindowL("#32770", sCaption);
                    if (ExistProces(OM.EzSensorInfo.sAppName1) || m_iHwndM != IntPtr.Zero)
                    {
                        m_iCycle = 20;
                        return false;
                    }

                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 11:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    if (!ExitProcess(OM.EzSensorInfo.sAppName1)) return false; //TODO :: 잠시 삭제
                    InitProcess1(OM.EzSensorInfo.sAppPath1);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 12:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    if (sCopyIniPath == "")
                    {
                        sErrMsg = "ERR001_Ini파일을 찾을 수 없습니다.(사본)";
                        Trace(sErrMsg);
                        return true;
                    }
                    LoadSetCaption(sCopyIniPath);
                    sCaption = "IntraOral Detector " + "(PID " + FindSetCptn.sProductID + ")";
                    m_iHwndM = FindWindowL("#32770", sCaption);
                    if (!ExistProces(OM.EzSensorInfo.sAppName1) || m_iHwndM == IntPtr.Zero)
                    {
                        sErrMsg = "ERR002_프로그램이 실행되지 않았습니다.";
                        Trace(sErrMsg);
                        return true;
                    }
                    
                    SetWindowPos(m_iHwndM, IntPtr.Zero, OM.EzSensorInfo.iPosX1, OM.EzSensorInfo.iPosY1, 0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);

                    m_tmDelay.Clear();
                    m_iCycle = 20;
                    return false;

                case 20://Calibration Tab 이동
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    SetTabControl(m_iHwnd1, Calibration);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 21://Get Dark
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, sClassName1, "Calibration");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, Tools.Button, "Get &Dark");
                    Click(m_iHwnd2);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 22:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwndS = FindWindowL("#32770", "Acquiring dark frame"); //캡션에 퍼센테이지가 올라가서 끝까지 유지가 안됨.
                    if (m_iHwndS == IntPtr.Zero) return false;
                    m_iCycle++;
                    return false;

                case 23:
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "button", null); //cancel 버튼이 하나 있음
                    if (m_iHwnd1 != IntPtr.Zero) return false;
                    m_tmDelay.Clear();
                    m_iCycle = 30;
                    return false;

                case 30: //Get Bright
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "#32770", "Calibration");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, Tools.Button, "Get &Bright");
                    Click(m_iHwnd2);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 31://지금 여기까지만 확인
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwndS = FindWindowL("#32770", "Acquiring bright frame");
                    if (m_iHwndS == IntPtr.Zero) return false;
                    m_iCycle++;
                    return false;

                case 32:
                    if (!m_tmDelay.OnDelay(1500)) return false;

                    SEQ.XrayCom.SetXrayPara(Setting.sKvp, Setting.smA, Setting.sTime);

                    m_iCycle++;
                    return false;

                case 33: //
                    if (!m_tmDelay.OnDelay(OM.EzSensorInfo.iGbDelay)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "button", null); //cancel 버튼이 하나 있음
                    if (m_iHwnd1 != IntPtr.Zero)
                    {
                        if (iRptCnt == OM.CmnOptn.iXrayRptCnt)
                        {
                            iRptCnt = 0;
                            bErrDevice = true;
                            m_iCycle = 0;
                            return true;
                        }
                        if (OM.CmnOptn.iXrayRptCnt > 0)
                        {
                            iRptCnt++;
                            m_tmDelay.Clear();
                            m_iCycle = 32;
                            return false;
                        }
                        else if (OM.CmnOptn.iXrayRptCnt <= 0)
                        {
                            return false;
                        }
                    }
                    else if (m_iHwnd1 == IntPtr.Zero)
                    {
                        bErrDevice = false;
                        iRptCnt = 0;
                    }

                    m_iCycle = 70;
                    return false;

                case 70:
                    string sFileName = "";
                    string sPath = Path.GetDirectoryName(OM.EzSensorInfo.sAppPath1) + "\\CAL" ;
                    if (Directory.Exists(sPath))
                    {
                        DirectoryInfo di1 = new DirectoryInfo(sPath);

                        foreach (var item in di1.GetFiles())
                        {
                            if (item.Name.IndexOf("x") == 0)
                            {
                                sFileName = item.Name;
                            }
                        }
                    }
                    if (!File.Exists(sPath + "\\" + sFileName)) 
                    {
                        sErrMsg = "ERR00D_Xxxxx.raw 파일을 찾을수 없습니다.";
                        Trace(sErrMsg);
                        return true; 
                    }

                    m_iCycle++;
                    return false;

                case 71:
                    string sTemp1 = OM.EqpStat.sSerialList;
                   
                    string sCalPath = Path.GetDirectoryName(OM.EzSensorInfo.sAppPath1) + "\\CAL" ;//CAL, CAL_A 원래 폴더
                    string sPath1   = Path.GetDirectoryName(OM.EzSensorInfo.sAppPath1) + "\\" + sTemp1 + "\\" + Setting.sSaveFolder + "\\" + Setting.sCalFolder + "\\Trigger"; //SerialNo 폴더

                    DirectoryInfo di = new DirectoryInfo(sPath1);
                    if (!di.Exists)
                    {
                        di.Create();
                    }
                    
                    CopyFolder(sCalPath, sPath1);
                    DeleteFolder(sCalPath);

                    m_iCycle = 0;
                    return true;
            }
        }

        //Ez, BI 공용으로 사용
        //Calibration1
        //GetBright 받는 부분
        //처음에 CAL 폴더 전체 삭제해야해서 GetBright와 쪼개서 씀
        public bool CycleCalibration1()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(m_iCycle != 0 && m_iCycle == m_iPreCycle, 20000))
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                if (sErrMsg == "") sErrMsg = m_sPartName + "Macro Time Out Error " + sTemp;
                Trace(sErrMsg);
                m_iCycle = 0;
                return true;
            }

            if (m_iCycle != m_iPreCycle)
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                Trace(sTemp);
                m_tmDelay.Clear();
            }

            m_iHwnd1 = IntPtr.Zero;
            m_iHwnd2 = IntPtr.Zero;
            m_iHwnd3 = IntPtr.Zero;
            m_iHwnd4 = IntPtr.Zero;

            //EzSensor Class name
            string sClassName1 = "#32770";

            //EzSensor Tab Order
            int Settings     = 0;
            int Acquisition  = 1;
            int Calibration  = 2;
            int Preprocess   = 3;
            int ImageProcess = 4;
            int Records      = 5;
            int Aging        = 6;
            int Log_Traces   = 7;

            m_iPreCycle = m_iCycle;

            int iDly = 700;

            switch (m_iCycle)
            {

                default:
                    sTemp = string.Format("Cycle_Default_Clear_m_iCycle={0:000}", m_iCycle);
                    sErrMsg = sTemp;
                    return true;


                case 0:
                    return true;

                //입고영상
                case 10:
                    //Skull 할때 Calibration에서 Generate 한 파일들이 필요하고
                    //MTF/NPS 영상 획득때 찍은 거 Generate한 파일 지워야 하는데
                    //집어 넣을 곳이 마땅치않아서 Calibration 시작 전에 지운다.
                    string sTemp1 = "";
                    
                    sTemp1 = OM.EqpStat.sSerialList;
                    string sFileName1 = "";

                    string sSerialPath = System.IO.Path.GetDirectoryName(OM.EzSensorInfo.sAppPath1) + "\\" + sTemp1 + "\\" + Setting.sSaveFolder + "\\" + Setting.sCalFolder; //SerialNo 폴더
                    string sDelPath = "";
                    if (Directory.Exists(sSerialPath))
                    {
                        DirectoryInfo di3 = new DirectoryInfo(sSerialPath);

                        foreach (var item in di3.GetFiles())
                        {
                            if (item.Name.IndexOf("x") == 0)
                            {
                                sFileName1 = item.Name;
                                sDelPath = sSerialPath + "\\" + sFileName1;
                                File.Delete(sDelPath);
                            }
                            if (item.Name.IndexOf("A") == 0) 
                            {
                                sFileName1 = item.Name;
                                sDelPath = sSerialPath + "\\" + sFileName1;
                                File.Delete(sDelPath);
                            }
                        }
                    }

                    //EzSensor 프로그램에서 영상 저장하는 폴더를 Calibration
                    //작업 전에 통째로 삭제
                    if (SEQ.XRYE.iWorkStep == 0 && SEQ.XRYE.iGetImgStep == 0)
                    {
                        Crntlst.Clear();
                        string sCalPath = Path.GetDirectoryName(OM.EzSensorInfo.sAppPath1) + "\\CAL" ;

                        DeleteFolder(sCalPath);
                    }
                    
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 11:
                    if (sCopyIniPath == "")
                    {
                        sErrMsg = "ERR001_Ini파일을 찾을 수 없습니다.(사본)";
                        Trace(sErrMsg);
                        return true;
                    }
                    LoadSetCaption(sCopyIniPath);
                    string sCaption = "IntraOral Detector " + "(PID " + FindSetCptn.sProductID + ")";
                    m_iHwndM = FindWindowL("#32770", sCaption);
                    if (ExistProces(OM.EzSensorInfo.sAppName1) || m_iHwndM != IntPtr.Zero)
                    {
                        m_iCycle = 20;
                        return false;
                    }

                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 12:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    if (!ExitProcess(OM.EzSensorInfo.sAppName1)) return false; //TODO :: 잠시 삭제
                    InitProcess1(OM.EzSensorInfo.sAppPath1);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 13:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    if (sCopyIniPath == "")
                    {
                        sErrMsg = "ERR001_Ini파일을 찾을 수 없습니다.(사본)";
                        Trace(sErrMsg);
                        return true;
                    }
                    LoadSetCaption(sCopyIniPath);
                    sCaption = "IntraOral Detector " + "(PID " + FindSetCptn.sProductID + ")";
                    m_iHwndM = FindWindowL("#32770", sCaption);
                    if (!ExistProces(OM.EzSensorInfo.sAppName1) || m_iHwndM == IntPtr.Zero)
                    {
                        sErrMsg = "ERR002_프로그램이 실행되지 않았습니다.";
                        Trace(sErrMsg);
                        return true;
                    }
                    
                    SetWindowPos(m_iHwndM, IntPtr.Zero, OM.EzSensorInfo.iPosX1, OM.EzSensorInfo.iPosY1, 0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);

                    m_tmDelay.Clear();
                    m_iCycle = 20;
                    return false;

                case 20://Calibration Tab 이동
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    SetTabControl(m_iHwnd1, Calibration);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 21://Get Dark
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    string sPath = Path.GetDirectoryName(OM.EzSensorInfo.sAppPath1) + "\\CAL" ;
                    if (Directory.Exists(sPath))
                    {
                        DirectoryInfo di1 = new DirectoryInfo(sPath);

                        foreach (var item in di1.GetFiles())
                        {
                            if (item.Name.IndexOf("dark") == 0)
                            {
                                m_tmDelay.Clear();
                                m_iCycle = 30;
                                return false;
                            }
                        }
                    }

                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, sClassName1, "Calibration");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, Tools.Button, "Get &Dark");
                    Click(m_iHwnd2);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 22:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwndS = FindWindowL("#32770", "Acquiring dark frame"); //캡션에 퍼센테이지가 올라가서 끝까지 유지가 안됨.
                    if (m_iHwndS == IntPtr.Zero) return false;
                    m_iCycle++;
                    return false;

                case 23:
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "button", null); //cancel 버튼이 하나 있음
                    if (m_iHwnd1 != IntPtr.Zero) return false;
                    m_tmDelay.Clear();
                    m_iCycle = 30;
                    return false;

                case 30: //Get Bright
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "#32770", "Calibration");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, Tools.Button, "Get &Bright");
                    Click(m_iHwnd2);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 31://지금 여기까지만 확인
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwndS = FindWindowL("#32770", "Acquiring bright frame");
                    if (m_iHwndS == IntPtr.Zero) return false;
                    m_iCycle++;
                    return false;

                case 32:
                    if (!m_tmDelay.OnDelay(1500)) return false;

                    SEQ.XrayCom.SetXrayPara(Setting.sKvp, Setting.smA, Setting.sTime);

                    m_iCycle++;
                    return false;

                case 33: //
                    if (!m_tmDelay.OnDelay(OM.EzSensorInfo.iGbDelay)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "button", null); //cancel 버튼이 하나 있음
                    if (m_iHwnd1 != IntPtr.Zero)
                    {
                        if (iRptCnt == OM.CmnOptn.iXrayRptCnt)
                        {
                            iRptCnt = 0;
                            sErrMsg = "트리거 신호가 나오지 않습니다.";
                            m_iCycle = 0;
                            return true;
                        }
                        if (OM.CmnOptn.iXrayRptCnt > 0)
                        {
                            iRptCnt++;
                            m_tmDelay.Clear();
                            m_iCycle = 32;
                            return false;
                        }
                        else if (OM.CmnOptn.iXrayRptCnt <= 0)
                        {
                            return false;
                        }
                    }
                    else if (m_iHwnd1 == IntPtr.Zero)
                    {
                        bErrDevice = false;
                        iRptCnt = 0;
                    }

                    m_iCycle = 0;
                    return true;

            }
        }

        //Ez, BI 공용으로 사용
        //Calibration2
        //폴더에 파일 검색해서 지울거 지우고 다시 GetBright 하는 부분
        List<string> AllFilelst = new List<string>(); //Cal 폴더 안에 x로 시작하는 폴더를 다 넣는 List
        List<string> Crntlst    = new List<string>(); //현재 작업중인 파일 리스트
        public bool CycleCalibration2()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(m_iCycle != 0 && m_iCycle == m_iPreCycle, 7000))
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                if (sErrMsg == "") sErrMsg = m_sPartName + "Macro Time Out Error " + sTemp;
                Trace(sErrMsg);
                m_iCycle = 0;
                return true;
            }

            if (m_iCycle != m_iPreCycle)
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                Trace(sTemp);
                m_tmDelay.Clear();
            }

            m_iHwnd1 = IntPtr.Zero;
            m_iHwnd2 = IntPtr.Zero;
            m_iHwnd3 = IntPtr.Zero;
            m_iHwnd4 = IntPtr.Zero;

            //EzSensor Class name
            string sClassName1 = "#32770";

            //EzSensor Tab Order
            int Settings     = 0;
            int Acquisition  = 1;
            int Calibration  = 2;
            int Preprocess   = 3;
            int ImageProcess = 4;
            int Records      = 5;
            int Aging        = 6;
            int Log_Traces   = 7;

            m_iPreCycle = m_iCycle;

            int iDly = 700;

            switch (m_iCycle)
            {

                default:
                    sTemp = string.Format("Cycle_Default_Clear_m_iCycle={0:000}", m_iCycle);
                    sErrMsg = sTemp;
                    return true;


                case 0:
                    return true;

                //입고영상
                case 10: //폴더 안에 파일 리스트 가져온다
                    Crntlst.Clear();
                    string sCalPath = Path.GetDirectoryName(OM.EzSensorInfo.sAppPath1) + "\\CAL" ;
                    int iMax = 0;
                    int iMin = 0;
                    int iAvr = 0;
                    int iSum = 0;

                    if (Directory.Exists(sCalPath))
                    {
                        DirectoryInfo di = new DirectoryInfo(sCalPath);
                        foreach (var item in di.GetFiles())
                        {
                            if (item.Name.IndexOf("x") == 0 && !AllFilelst.Contains(item.Name)) Crntlst.Add(item.Name);
                            Trace(item.Name);
                        }
                    }

                    int[] iTemp = new int[Crntlst.Count];
                    for (int i = 0; i < Crntlst.Count; i++)
                    {
                        iTemp[i] = CConfig.StrToIntDef(Crntlst[i].Substring(1, 5), 0);

                        //iMax = 0;
                        if (iTemp[i] > iMax) iMax = iTemp[i];
                    } 

                    iMin = iMax;
                    for (int i = 0; i < Crntlst.Count; i++)
                    {
                        if (iTemp[i] < iMin) iMin = iTemp[i];
                    }

                    
                    for (int i = 0; i < Crntlst.Count; i++)
                    {
                        iSum = iSum + iTemp[i];
                        iAvr = iSum / Crntlst.Count;
                    }

                    iGetBrightMax = iMax;
                    iGetBrightMin = iMin;
                    iGetBrightAvr = iAvr;
                        
                    iGetBrightRslt = iGetBrightMax - iGetBrightMin;

                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 11: //Tolerance보다 크면 1번 더 작업
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    //string sCaption = "IntraOral Detector " + "(PID 2009)";
                    //m_iHwndM = FindWindowL("#32770", sCaption);
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, sClassName1, "Calibration");
                    string sTol = GetWindowText(m_iHwnd1, Tools.Edit, 0);
                    int iTol = CConfig.StrToIntDef(sTol, 0);

                    string sDelFile = "";
                    sCalPath = Path.GetDirectoryName(OM.EzSensorInfo.sAppPath1) + "\\CAL" ;

                    int iAvrDev1 = Math.Abs(iGetBrightMax - iGetBrightAvr);
                    int iAvrDev2 = Math.Abs(iGetBrightMin - iGetBrightAvr);
               
                    if (iGetBrightRslt > iTol)
                    {
                        if (System.IO.Directory.Exists(sCalPath))
                        {
                            if(iAvrDev1 < iAvrDev2 || iAvrDev2 == iAvrDev1)
                            {
                                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(sCalPath);
                                foreach (var item in di.GetFiles())
                                {
                                    if (item.Name.IndexOf("x" + string.Format("{0:00000}", iGetBrightMin) + "A.raw") == 0)
                                    {
                                        sDelFile = item.Name;
                                        Trace(sDelFile);
                                    }
                                }
                                Crntlst.Remove("X" + string.Format("{0:0000}", iGetBrightMin) + "A.raw");
                                FileInfo[] files = di.GetFiles(sDelFile, SearchOption.AllDirectories);

                                foreach (FileInfo file in files)
                                {
                                    file.Attributes = FileAttributes.Normal;
                                    file.Delete();
                                }
                            }
                            else if(iAvrDev1 > iAvrDev2)
                            {
                                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(sCalPath);
                                foreach (var item in di.GetFiles())
                                {
                                    if (item.Name.IndexOf("x" + string.Format("{0:00000}", iGetBrightMax) + "A.raw") == 0)
                                    {
                                        sDelFile = item.Name;
                                        Trace(sDelFile);
                                    }
                                }
                                Crntlst.Remove("X" + string.Format("{0:0000}", iGetBrightMax) + "A.raw");
                                FileInfo[] files = di.GetFiles(sDelFile, SearchOption.AllDirectories);

                                foreach (FileInfo file in files)
                                {
                                    file.Attributes = FileAttributes.Normal;
                                    file.Delete();
                                }
                            }
                            
                        }
                 
                        bReWork = true;
                        m_iCycle = 0;
                        return true;
                    }

                    m_iCycle++;
                    return false;

                case 12:
                    sCalPath = Path.GetDirectoryName(OM.EzSensorInfo.sAppPath1) + "\\CAL" ;
                    if (System.IO.Directory.Exists(sCalPath))
                    {
                        System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(sCalPath);
                        foreach (var item in di.GetFiles())
                        {
                            if (item.Name.IndexOf("x") == 0) AllFilelst.Add(item.Name);
                        }
                        
                    }
                    Crntlst.Clear();
                    m_iCycle++;
                    return false;
                    

                case 13: //영상 받을때마다 ini 파일 복사
                    string sTemp1 = OM.EqpStat.sSerialList;
                    string sPath2 = Path.GetDirectoryName(OM.EzSensorInfo.sAppPath1) + "\\" + sTemp1 + "\\" + Setting.sSaveFolder + "\\" ; //SerialNo 폴더
                    
                    //INI 파일 지우기
                    string sPath = OM.EzSensorInfo.sIniPath2 + "\\";           //원본 파일이고 시리얼넘버 폴더에 넣는다.
                    string sName = Path.GetFileName(OM.EzSensorInfo.sIniPath1);//원본 파일이고 시리얼넘버 폴더에 넣는다.
                    if (!File.Exists(OM.EzSensorInfo.sIniPath2 + "\\" + sName))
                    {
                        sErrMsg = "ERR001_Ini파일을 찾을 수 없습니다.(사본)";
                        Trace(sErrMsg);
                        return true;
                    }
                    if (File.Exists(sPath2 + "\\" + sName))
                    {
                        File.Delete(sPath2 + "\\" + sName);
                    }
                    //INI 파일 복사
                    File.Copy(sPath + sName, sPath2 + "\\" + sName);

                    m_iCycle = 0;
                    return true;

            }
        }

        //Ez, BI 공용으로 사용
        //특성검사
        //Acquisition Tab Get Image
        public bool CycleSkull()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(m_iCycle != 0 && m_iCycle == m_iPreCycle, 20000))
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                if (sErrMsg == "") sErrMsg = m_sPartName + "Macro Time Out Error " + sTemp;
                Trace(sErrMsg);
                m_iCycle = 0;
                return true;
            }

            if (m_iCycle != m_iPreCycle)
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                Trace(sTemp);
                m_tmDelay.Clear();
            }

            m_iHwnd1 = IntPtr.Zero;
            m_iHwnd2 = IntPtr.Zero;
            m_iHwnd3 = IntPtr.Zero;
            m_iHwnd4 = IntPtr.Zero;

            //EzSensor Class name
            string sClassName1 = "#32770";

            //EzSensor Tab Order
            int Settings = 0;
            int Acquisition = 1;
            int Calibration = 2;
            int Preprocess = 3;
            int ImageProcess = 4;
            int Records = 5;
            int Aging = 6;
            int Log_Traces = 7;

            m_iPreCycle = m_iCycle;

            int iDly = 700;

            switch (m_iCycle)
            {

                default:
                    sTemp = string.Format("Cycle_Default_Clear_m_iCycle={0:000}", m_iCycle);
                    sErrMsg = sTemp;
                    return true;


                case 0:
                    return true;

                //입고영상
                case 10:
                    if (sCopyIniPath == "")
                    {
                        sErrMsg = "ERR001_Ini파일을 찾을 수 없습니다.(사본)";
                        Trace(sErrMsg);
                        return true;
                    }
                    LoadSetCaption(sCopyIniPath);
                    string sCaption = "IntraOral Detector " + "(PID " + FindSetCptn.sProductID + ")";
                    m_iHwndM = FindWindowL("#32770", sCaption);
                    if (ExistProces(OM.EzSensorInfo.sAppName1) || m_iHwndM != IntPtr.Zero)
                    {
                        m_iCycle = 20;
                        return false;
                    }

                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 11:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    if (!ExitProcess(OM.EzSensorInfo.sAppName1)) return false; //TODO :: 잠시 삭제
                    InitProcess1(OM.EzSensorInfo.sAppPath1);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 12:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    if (sCopyIniPath == "")
                    {
                        sErrMsg = "ERR001_Ini파일을 찾을 수 없습니다.(사본)";
                        Trace(sErrMsg);
                        return true;
                    }
                    LoadSetCaption(sCopyIniPath);
                    sCaption = "IntraOral Detector " + "(PID " + FindSetCptn.sProductID + ")";
                    m_iHwndM = FindWindowL("#32770", sCaption);
                    if (!ExistProces(OM.EzSensorInfo.sAppName1) || m_iHwndM == IntPtr.Zero)
                    {
                        sErrMsg = "ERR002_프로그램이 실행되지 않았습니다.";
                        Trace(sErrMsg);
                        return true;
                    }
                    SetWindowPos(m_iHwndM, IntPtr.Zero, OM.EzSensorInfo.iPosX1, OM.EzSensorInfo.iPosY1, 0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);

                    m_tmDelay.Clear();
                    m_iCycle = 20;
                    return false;

                case 20://Setting Tab 이동
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    SetTabControl(m_iHwnd1, Settings);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 21://Get Dark
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "#32770", "Settings");
                    if (m_iHwnd1 == IntPtr.Zero) return false;

                    SetWindow    (m_iHwnd1, Tools.ComboBox, 1 , OM.EzSensorInfo.iSkRotate              );
                    SetWindow    (m_iHwnd1, Tools.Button  , 9 , OM.EzSensorInfo.bSkFlipHorz ? 1 : 0    );
                    SetWindow    (m_iHwnd1, Tools.Button  , 10, OM.EzSensorInfo.bSkFlipVert ? 1 : 0    );
                    SetWindowText(m_iHwnd1, Tools.Edit    , 4 , OM.EzSensorInfo.iSkCropTop  .ToString());
                    SetWindowText(m_iHwnd1, Tools.Edit    , 5 , OM.EzSensorInfo.iSkCropLeft .ToString());
                    SetWindowText(m_iHwnd1, Tools.Edit    , 6 , OM.EzSensorInfo.iSkCropRight.ToString());
                    SetWindowText(m_iHwnd1, Tools.Edit    , 7 , OM.EzSensorInfo.iSkCropBtm  .ToString());

                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 22: 
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "#32770", "Settings");
                    if (m_iHwnd1 == IntPtr.Zero) return false;

                    if(GetWindow    (m_iHwnd1, Tools.ComboBox, 1 ) !=  OM.EzSensorInfo.iSkRotate              ) sErrMsg = "Rotate Setting Fail"           ;
                    if(GetWindow    (m_iHwnd1, Tools.Button  , 9 ) != (OM.EzSensorInfo.bSkFlipHorz ? 1 : 0   )) sErrMsg = "Flip Horz Setting Fail"        ;
                    if(GetWindow    (m_iHwnd1, Tools.Button  , 10) != (OM.EzSensorInfo.bSkFlipVert ? 1 : 0   )) sErrMsg = "Flip Vert Setting Fail"        ;
                    if(GetWindowText(m_iHwnd1, Tools.Edit    , 4 ) !=  OM.EzSensorInfo.iSkCropTop  .ToString()) sErrMsg = "Crop Top Setting Fail"         ;
                    if(GetWindowText(m_iHwnd1, Tools.Edit    , 5 ) !=  OM.EzSensorInfo.iSkCropLeft .ToString()) sErrMsg = "Crop Left Setting Fail"        ;
                    if(GetWindowText(m_iHwnd1, Tools.Edit    , 6 ) !=  OM.EzSensorInfo.iSkCropRight.ToString()) sErrMsg = "Crop Right Setting Fail"       ;
                    if(GetWindowText(m_iHwnd1, Tools.Edit    , 7 ) !=  OM.EzSensorInfo.iSkCropBtm  .ToString()) sErrMsg = "Crop Bottom Setting Fail"      ;
                     
                    if(sErrMsg != "")
                    {
                        Trace(sErrMsg);
                        return true;
                    }
                    m_tmDelay.Clear();
                    m_iCycle = 30;
                    return false;

                case 30://Acqiusition Tab 이동
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    if (m_iHwnd1 == IntPtr.Zero) return false;
                    SetTabControl(m_iHwnd1, Acquisition);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 31:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, sClassName1, "Acquisition");
                    SetWindowText(m_iHwnd1, Tools.Edit, 4, Setting.sFileName);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 32:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, sClassName1, "Acquisition");
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, Tools.Button, "&Get Image"); //동작중일때 버튼 캡션이 Generating...으로 바뀜
                    Click(m_iHwnd2);
                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 33:
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwndS = FindWindowL("#32770", "Acquiring bright frame"); //캡션에 퍼센테이지가 올라가서 끝까지 유지가 안됨.
                    if (m_iHwndS == IntPtr.Zero) return false;
                    m_iCycle++;
                    return false;

                case 34://Xray 조사하는거 넣어야함
                    if (!m_tmDelay.OnDelay(1500)) return false;

                    SEQ.XrayCom.SetXrayPara(Setting.sKvp, Setting.smA, Setting.sTime);

                    m_iCycle++;
                    return false;

                case 35: //
                    if (!m_tmDelay.OnDelay(OM.EzSensorInfo.iGbDelay)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "button", null); //cancel 버튼이 하나 있음
                    if (m_iHwnd1 != IntPtr.Zero)
                    {
                        if (iRptCnt == OM.CmnOptn.iXrayRptCnt)
                        {
                            iRptCnt = 0;
                            sErrMsg = "트리거 신호가 나오지 않습니다.";
                            m_iCycle = 0;
                            return true;
                        }
                        if (OM.CmnOptn.iXrayRptCnt > 0)
                        {
                            iRptCnt++;
                            m_tmDelay.Clear();
                            m_iCycle = 34;
                            return false;
                        }
                        else if (OM.CmnOptn.iXrayRptCnt <= 0)
                        {
                            return false;
                        }
                    }
                    else if (m_iHwnd1 == IntPtr.Zero)
                    {
                        bErrDevice = false;
                        iRptCnt = 0;
                    }

                    m_tmDelay.Clear();
                    m_iCycle++;
                    return false;

                case 36://폴더 카피하는 부분
                    string sFdName = ""; //원래 저장되어있는 폴더
                    int iWidthCrop = OM.EzSensorInfo.iSkCropLeft + OM.EzSensorInfo.iSkCropRight;
                    int iHghtCrop  = OM.EzSensorInfo.iSkCropTop  + OM.EzSensorInfo.iSkCropBtm  ;
                    if (OM.EzSensorInfo.iSkRotate == 0) //이미지 안돌림
                    {
                        sFdName = "I" + string.Format("{0:0000}", Setting.iWidth - iWidthCrop) + "X" + string.Format("{0:0000}", Setting.iHeight - iHghtCrop);
                    }
                    else if (OM.EzSensorInfo.iSkRotate == 1 || OM.EzSensorInfo.iSkRotate == 2) //90도 돌려서
                    {
                        sFdName = "I" + string.Format("{0:0000}", Setting.iHeight - iHghtCrop) + "X" + string.Format("{0:0000}", Setting.iWidth - iWidthCrop);
                    }
            
                    string sPath1 = Path.GetDirectoryName(OM.EzSensorInfo.sAppPath1) + "\\" + sFdName; //원래 폴더
                    string sPath2 = Path.GetDirectoryName(OM.EzSensorInfo.sAppPath1) + "\\" + OM.EqpStat.sSerialList + "\\" + Setting.sGetImgFdName; //변경할 폴더 이름
                    DirectoryInfo di1 = new DirectoryInfo(sPath1);
                    DirectoryInfo di2 = new DirectoryInfo(sPath2);
                    
                    if (!di2.Exists)
                    {
                        di2.Create();
                    }
                    if (di1.Exists) 
                    {
                        System.IO.FileInfo[] files = di1.GetFiles("*.*",
                    
                        System.IO.SearchOption.AllDirectories);
                    
                        foreach (System.IO.FileInfo file in files)
                        {
                            file.Attributes = System.IO.FileAttributes.Normal;
                        }
                        
                    }
                    string sFileName = "\\" + Setting.sFileName + ".raw";
                    FileInfo Fi1 = new System.IO.FileInfo(sPath2 + sFileName); //시리얼넘버 폴더에 파일 있으면 지운다
                    if (Fi1.Exists)
                    {
                        Fi1.Delete();
                    }
                    File.Copy(sPath1 + sFileName, sPath2 + sFileName);//파일 카피
                    DeleteFolder(sPath1);

                    m_iCycle++;
                    return false;

                case 37:
                    //1x1, 2x2(시리얼넘버 폴더 내의 시리얼넘버 폴더)에 Cal 폴더에 있는 A로 시작하는 Generate 파일 복사 생성
                    string sTemp1 = "";

                    sTemp1 = OM.EqpStat.sSerialList;
                    string sFileName1 = "";

                    //복사 원본 파일 경로
                    string sSourcePath = System.IO.Path.GetDirectoryName(OM.EzSensorInfo.sAppPath1) + "\\" + sTemp1 + "\\2x2\\" + Setting.sCalFolder;
                    string sDestPath = System.IO.Path.GetDirectoryName(OM.EzSensorInfo.sAppPath1) + "\\" + sTemp1 + "\\" + sTemp1;
                    string sCopyFile = "";
                    string sDestFile = "";
                    if (!Directory.Exists(sDestPath))
                    {
                        Directory.CreateDirectory(sDestPath);
                    }
                    if (Directory.Exists(sSourcePath))
                    {
                        DirectoryInfo di3 = new DirectoryInfo(sSourcePath);

                        foreach (var item in di3.GetFiles())
                        {
                            if (item.Name.IndexOf("A") == 0)
                            {
                                sFileName1 = item.Name;
                                sCopyFile = sSourcePath + "\\" + sFileName1;
                                sDestFile = sDestPath   + "\\" + sFileName1;
                                File.Copy(sCopyFile, sDestFile);
                            }

                            if(item.Name.IndexOf("BPM.raw") == 0)
                            {
                                sFileName1 = item.Name;
                                sCopyFile = sSourcePath + "\\" + sFileName1;
                                sDestFile = sDestPath + "\\" + sFileName1;
                                File.Copy(sCopyFile, sDestFile);
                            }
                            if (item.Name.IndexOf("BPMU.raw") == 0)
                            {
                                sFileName1 = item.Name;
                                sCopyFile = sSourcePath + "\\" + sFileName1;
                                sDestFile = sDestPath + "\\" + sFileName1;
                                File.Copy(sCopyFile, sDestFile);
                            }
                            if (item.Name.IndexOf("dark.raw") == 0)
                            {
                                sFileName1 = item.Name;
                                sCopyFile = sSourcePath + "\\" + sFileName1;
                                sDestFile = sDestPath + "\\" + sFileName1;
                                File.Copy(sCopyFile, sDestFile);
                            }
                        }
                    }

                    m_iCycle++;
                    return false;

                case 38:
                    sTemp1      = OM.EqpStat.sSerialList;
                    string sCopyPath1 = Path.GetDirectoryName(OM.EzSensorInfo.sAppPath1) + "\\" + sTemp1 + "\\" + Setting.sSaveFolder; //SerialNo 폴더
                    string sCopyPath2 = Path.GetDirectoryName(OM.EzSensorInfo.sAppPath1) + "\\" + sTemp1 + "\\" + sTemp1;
                    //INI 파일 체크

                    //INI 파일 지우기
                    string sPath = OM.EzSensorInfo.sIniPath2 + "\\";           //원본 파일이고 시리얼넘버 폴더에 넣는다.
                    string sName = Path.GetFileName(OM.EzSensorInfo.sIniPath1);//원본 파일이고 시리얼넘버 폴더에 넣는다.
                    if (!File.Exists(OM.EzSensorInfo.sIniPath2 + "\\" + sName))
                    {
                        sErrMsg = "ERR001_Ini파일을 찾을 수 없습니다.(사본)";
                        Trace(sErrMsg);
                        return true;
                    }
                    if (File.Exists(sCopyPath1 + "\\" + sName))
                    {
                        File.Delete(sCopyPath1 + "\\" + sName);
                    }
                    if (File.Exists(sCopyPath2 + "\\" + sName))
                    {
                        File.Delete(sCopyPath2 + "\\" + sName);
                    }
                    //INI 파일 복사
                    File.Copy(sPath + sName, sCopyPath1 + "\\" + sName);
                    File.Copy(sPath + sName, sCopyPath2 + "\\" + sName);

                    m_iCycle = 0;
                    return true;

                //CAL 폴더 통째로 결과 폴더에 복사 및 삭제
                case 39:
                    sTemp1 = OM.EqpStat.sSerialList;
                    //C:\\EzSensor\\시리얼넘버\\ 폴더를 결과 저장 디렉토리로 카피 후 지운다.
                    sPath2 = Path.GetDirectoryName(OM.EzSensorInfo.sAppPath1) + "\\" + sTemp1 ; //SerialNo 폴더
                    CopyFolder(sPath2, GetRsltFd());
                    DeleteFolder(sPath2);
                   
                    m_iCycle = 0;
                    return true;

            }
        }

        //Ez, BI 공용으로 사용
        //공정 끝나고 시리얼넘버 폴더 Report 폴더에 카피하는 사이클
        //Skull공정 Skip 하게되면 복사를 못해서 따로 분리
        public bool CycleRsltCopy()
        {
            String sTemp;
            if (m_tmCycle.OnDelay(m_iCycle != 0 && m_iCycle == m_iPreCycle, 20000))
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                if (sErrMsg == "") sErrMsg = m_sPartName + "Macro Time Out Error " + sTemp;
                Trace(sErrMsg);
                m_iCycle = 0;
                return true;
            }

            if (m_iCycle != m_iPreCycle)
            {
                sTemp = string.Format("m_iCycle={0:00}", m_iCycle);
                Trace(sTemp);
                m_tmDelay.Clear();
            }

            m_iHwnd1 = IntPtr.Zero;
            m_iHwnd2 = IntPtr.Zero;
            m_iHwnd3 = IntPtr.Zero;
            m_iHwnd4 = IntPtr.Zero;

            //EzSensor Class name
            string sClassName1 = "#32770";

            //EzSensor Tab Order
            int Settings = 0;
            int Acquisition = 1;
            int Calibration = 2;
            int Preprocess = 3;
            int ImageProcess = 4;
            int Records = 5;
            int Aging = 6;
            int Log_Traces = 7;

            m_iPreCycle = m_iCycle;

            int iDly = 700;

            switch (m_iCycle)
            {

                default:
                    sTemp = string.Format("Cycle_Default_Clear_m_iCycle={0:000}", m_iCycle);
                    sErrMsg = sTemp;
                    return true;


                case 0:
                    return true;

                //입고영상
                case 10:
                //CAL 폴더 통째로 결과 폴더에 복사 및 삭제
                    string sTemp1 = OM.EqpStat.sSerialList;
                    //C:\\EzSensor\\시리얼넘버\\ 폴더를 결과 저장 디렉토리로 카피 후 지운다.
                    string sPath = Path.GetDirectoryName(OM.EzSensorInfo.sAppPath1) + "\\" + sTemp1; //SerialNo 폴더
                    CopyFolder(sPath, GetRsltFd());
                    DeleteFolder(sPath);

                    m_iCycle = 0;
                    return true;

            }
        }



        public void Log_Trace(string sLog_Trace, [CallerLineNumber] int sourceLineNumber = 0)
        {
            Debug.WriteLine(DateTime.Now.ToString("MM-dd hh:mm:ss:fff_") + sLog_Trace + "_" + sourceLineNumber.ToString());
        }

    }

    public class MacroCmd
    {
        public static int m_iPartId;
        public MacroCmd(int _iPartId = 0)
        {
            m_iPartId = _iPartId;
        }
        //=========================================================================================
        [DllImport("user32")]
        public static extern int IsWindowVisible(IntPtr hwnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        protected static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int  SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, string lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SendMessage(IntPtr hWnd, uint Msg, int wParam, StringBuilder lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int SendMessage(IntPtr window, int message, int wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, string lParam);
        
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int IsWindowEnabled(IntPtr hwnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, UInt32 uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hwnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("kernel32", CharSet = CharSet.Auto)]
        public static extern IntPtr OpenProcess(Int32 Access, Boolean InheritHandle, Int32 ProcessId);


        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint dwFreeType);
        
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, int nSize, int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out()] IntPtr lpBuffer, int dwSize, int lpNumberOfBytesRead);

        [StructLayoutAttribute(LayoutKind.Sequential)]
        struct LV_ITEM
        {
            public UInt32 mask;
            public Int32 iItem;
            public Int32 iSubItem;
            public UInt32 state;
            public UInt32 stateMask;
            public IntPtr pszText;
            public Int32 cchTextMax;
            public Int32 iImage;
            public IntPtr lParam;
        }
        //=========================================================================================
        //사용하는 윈도우 메시지
        protected const UInt32 WM_LBUTTONDOWN   = 0x0201;//0x0002
        protected const UInt32 WM_LBUTTONUP     = 0x0202;//0x0004
        protected const UInt32 WM_RBUTTONDOWN   = 0x0204;
        protected const UInt32 WM_RBUTTONUP     = 0x0205;
        protected const UInt32 BM_CLICK         = 0x00F5;
        protected const UInt32 WM_GETTEXT       = 0x000D;
        protected const UInt32 WM_GETTEXTLENGTH = 0x000E;
        protected const UInt32 WM_COPYDATA      = 0x4A  ;
        protected const UInt32 WM_CLOSE         = 0x0010;
        protected const UInt32 WM_SETTEXT       = 0x000C;
        protected const UInt32 CB_GETCURSEL     = 0x0147;
        protected const UInt32 CB_SELECTSTRING  = 0x014D;
        protected const UInt32 TCM_SETCURFOCUS  = 0x1330;
        protected const UInt32 CB_FINDSTRING    = 0x014C;
        protected const UInt32 TCM_SETCURSEL    = 0x1312;
        protected const UInt32 WM_COMMAND       = 0x111 ;
        protected const UInt32 BM_SETSTATE      = 0x00F3;
        protected const UInt32 CB_SETCURSEL     = 0x014E;
        protected const UInt32 BM_SETCHECK      = 0x00F1;
        protected const UInt32 BM_GETCHECK      = 0x00F0;
        
        protected const UInt32 SWP_NOSIZE       = 0x0001;
        protected const UInt32 SWP_NOZORDER     = 0x0004;
        protected const UInt32 SWP_SHOWWINDOW   = 0x0040;

        protected const int LVM_FIRST           = 0x1000;
        protected const int LVM_GETITEMCOUNT    = LVM_FIRST + 4;
        protected const int LVM_GETITEM         = LVM_FIRST + 75;
        protected const int LVIF_TEXT           = 0x0001;

        public const int WM_ENABLE   = 0x000A;
        public const int WM_KEYDOWN  = 0x0100;
        public const int WM_CHAR     = 0x0102;
        public const int VK_RETURN   = 0x0D  ;
        public const int WM_SETFOCUS = 0x0007;
        public const int WM_KEYUP    = 0x101 ;
        public const int PROCESS_ALL_ACCESS = 0x1F0FFF;

        //=========================================================================================
        public static IntPtr MakeLParam(int LoWord, int HiWord)
        {
            return (IntPtr) ((HiWord >> 16) | (LoWord & 0xffff));
        }
        private static int HiWord(int number) 
        {
            if((number & 0x80000000) == 0x80000000) return (number >> 16); 
            else                                    return (number >> 16) & 0xffff ;
        }
        private static int LoWord(int number) 
        {
            return number & 0xffff;
        }
        //=========================================================================================
        //구형
        public struct Tools
        {
            public const string ComboBox = "ComboBox";
            public const string Button   = "Button";
            public const string Edit     = "Edit";
            public const string Static   = "Static";
            public const string RichEdit = "RichEdit20W";
        }

        static System.Diagnostics.Process Proc1 = new System.Diagnostics.Process();
        static System.Diagnostics.Process Proc2 = new System.Diagnostics.Process();

        protected void InitProcess1(string _sAppPath,int x = 0, int y = 0)
        {
            Proc1.StartInfo.FileName = _sAppPath;
            //Proc1.StartInfo.Verb = "runas";
            try
            {
                Trace("Before Process Start");
                Proc1.Start();
                Trace("Process Start");
                //Proc1.WaitForInputIdle();
                //if(x != 0 && y != 0) SetWindowPos(Proc1.MainWindowHandle,IntPtr.Zero,x,y,0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);
                Trace("Before Wait");
                Proc1.WaitForExit(1000);
                Trace("1Sec Wait");
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
        
        protected void InitProcess2(string _sAppPath,int x = 0, int y = 0)
        {
            Proc2.StartInfo.FileName = _sAppPath;
            //Proc2.StartInfo.Verb = "runas";
            try
            {
                Proc2.Start();
                //Proc2.WaitForInputIdle();
                //if (x != 0 && y != 0) SetWindowPos(Proc2.MainWindowHandle, IntPtr.Zero, x, y, 0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);
                Proc2.WaitForExit(1000);
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        protected bool ExitProcess(string _sAppCaption = "")
        {
            bool bRet = true;
            Process[] pList = Process.GetProcessesByName(_sAppCaption);

            if (_sAppCaption != "" && pList.Length > 0)
            {
                for (int i = 0; i < pList.Length; i++) pList[i].Kill();
            }
            if (pList.Length > 0) bRet = false;
            return bRet;
        }

        protected bool ExistProces(string _sAppCaption = "")
        {
            bool bRet = false;
            Process[] pList = Process.GetProcessesByName(_sAppCaption);
            if (pList.Length > 0) bRet = true;
            return bRet;
        }

        protected IntPtr FindWindowL(string strClassName, string strWindowName) 
        {
            IntPtr iRet = IntPtr.Zero;
            try
            {
                iRet = FindWindow(strClassName, strWindowName);
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return iRet;

        }
        //프로그램에 종속된 컨트롤 핸들 찾는부분
        protected IntPtr FindWindowChild(IntPtr _ipParent , IntPtr _ipFrom , string _sClass , string _sCaption)
        {
            //Delay(50);
            IntPtr iRet = IntPtr.Zero;
            try
            {
                iRet = FindWindowEx(_ipParent, _ipFrom, _sClass, _sCaption);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return iRet;        
        }

        protected IntPtr FindWindowIndex(IntPtr _ipParent, string Tool , int _iIdx = 0) 
        {
            int iIdx = _iIdx;
            IntPtr iRet = FindWindowChild(_ipParent, IntPtr.Zero, Tool, null);
            if (iRet != IntPtr.Zero)
            {
                for (int i = 0; i < iIdx; i++)
                {
                    iRet = FindWindowChild(_ipParent, iRet, Tool, null);
                    if (iRet == IntPtr.Zero) break;
                }
            }
            return iRet;
        }

        protected IntPtr SetWindow(IntPtr _ipParent, string Tool , int _iIdx = 0 , int _iValue = 0) 
        {
            IntPtr iRet = FindWindowIndex(_ipParent, Tool, _iIdx);

            if(iRet != IntPtr.Zero)
            {
                if(Tool == Tools.ComboBox   ) PostMessage(iRet, CB_SETCURSEL, _iValue, 0); //0 ~
                else if(Tool == Tools.Button) PostMessage(iRet, BM_SETCHECK , _iValue, 0); // 0 - uncheck 1 - check
            }
            return iRet;
        }

        protected IntPtr SetWindowText(IntPtr _ipParent, string Tool , int _iIdx = 0 , string  _sValue = "") 
        {
            IntPtr iRet = FindWindowIndex(_ipParent, Tool, _iIdx);

            if(iRet != IntPtr.Zero)
            {
                SendMessage(iRet, WM_SETTEXT, IntPtr.Zero, _sValue); //need to be delivered synchronously
            }
            return iRet;
        }

        protected int GetWindow(IntPtr _ipParent, string Tool , int _iIdx = 0) 
        {
            int    iText = 0;
            IntPtr iRet  = FindWindowIndex(_ipParent, Tool, _iIdx);
            
            if(iRet != IntPtr.Zero)
            {
                if(Tool == Tools.ComboBox   ) iText = SendMessage(iRet, CB_GETCURSEL, 0, 0); //0 ~
                else if(Tool == Tools.Button) iText = SendMessage(iRet, BM_GETCHECK , 0, 0); //0 - uncheck 1 - check
            }
            return iText;
        }

        protected string GetWindowText(IntPtr _ipParent, string Tool , int _iIdx = 0 ) 
        {
            IntPtr textPtr = IntPtr.Zero;
            string sText   = "";
            IntPtr iRet    = FindWindowIndex(_ipParent, Tool, _iIdx);

            if(iRet != IntPtr.Zero)
            {
                StringBuilder title = new StringBuilder();
                int size = SendMessage(iRet, WM_GETTEXTLENGTH, 0, 0);
                if (size > 0)
                {
                    title = new StringBuilder(size + 1);
                    SendMessage(iRet, WM_GETTEXT, title.Capacity, title);
                    sText = title.ToString();
                }
            }
            return sText;
        }

        //윈도우에 해당하는 텍스트를 호출자가 제공 한 버퍼로 복사합니다.
        protected string GetWindowText(IntPtr _ipWnd)
        {
            //Delay(500);
            IntPtr textPtr = IntPtr.Zero;
            string sTemp = "";
            try
            {
                StringBuilder title = new StringBuilder();

                // Get the size of the string required to hold the window title. 
                int size = SendMessage(_ipWnd, WM_GETTEXTLENGTH, 0, 0);

                // If the return is 0, there is no title. 
                if (size > 0)
                {
                    title = new StringBuilder(size + 1);
                    SendMessage(_ipWnd, WM_GETTEXT, title.Capacity, title);
                    sTemp = title.ToString();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return sTemp ;
        }

        //Listview 아이템 텍스트 가져올때 사용
        
        protected string GetItemText(IntPtr handle, int index, int subIndex)
        {
            int pid;

            //핸들을 이용하여 프로세스 id를 얻어 온다.
            GetWindowThreadProcessId(handle, out pid);


            //해당 프로세스의 핸들을 얻어 옵니다.
            IntPtr hProcess = OpenProcess(0x0008 | 0x0010 | 0x0020 | 0x0400, false, pid);
         

            //해당 프로세스 영역에 메모리를 할당 합니다.

            IntPtr vPtr = VirtualAllocEx(hProcess, IntPtr.Zero, Marshal.SizeOf(typeof(LV_ITEM)), 0x1000, 0x04);


            //우리가 원하는 리스트뷰의 항목 구조체를 할당 합니다.

            LV_ITEM item = new LV_ITEM();

            //텍스트를 얻어오겠다는 마스크 입니다.
            item.mask = LVIF_TEXT;

            //텍스트의 최대 크기를 지정 합니다.
            item.cchTextMax = 512;
            //원하는 항목의 인덱스 입니다.
            item.iItem = index;
            //원하는 서브 항목의 인덱스 입니다.
            item.iSubItem = subIndex;

            //이부분에 항목문자열이 저장 됩니다. 역시 해당 프로세스 영역에 할당 합니다.
            item.pszText = VirtualAllocEx(hProcess, IntPtr.Zero, 512, 0x1000, 0x04);
           

            //방금만든 구조체를 비관리 영역에 할당 합니다.
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(LV_ITEM)));

            //할당한 포인터에 복사 합니다.
            Marshal.StructureToPtr(item, ptr, true);


            //비관리 영역에 할당한 포인터를 프로세스 영역에 할당한 포인터에 씁니다.

            WriteProcessMemory(hProcess, vPtr, ptr, Marshal.SizeOf(typeof(LV_ITEM)), 0);

            //쓰고나면 포인터는 해제 합니다.
            Marshal.FreeHGlobal(ptr);


            //항목 포인터를 얻어 옵니다.

            SendMessage(handle, LVM_GETITEM, 0, vPtr);


            //얻어온 항목은 프로세스 영역에 할당되어 있기 때문에 읽거나 쓸 수 없습니다.

            //다시 비관리 영역으로 읽어 들여야 합니다. 이를위해 다시 할당 합니다.

            ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(LV_ITEM)));


            //해당 프로세스 영역에서 비관리 영역으로 읽어 들입니다.
            ReadProcessMemory(hProcess, vPtr, ptr, Marshal.SizeOf(typeof(LV_ITEM)), 0);

            //읽어들인 포인터를 구조체로 변환 합니다.
            item = (LV_ITEM)Marshal.PtrToStructure(ptr, typeof(LV_ITEM));

            //쓰고난 포인터 해제
            Marshal.FreeHGlobal(ptr);


            //이제 다왔습니다. 문자열을 얻어오기 위한 작업 입니다. 위의 작업과 거의 동일하니 주석 생략 합니다.

            ptr = Marshal.AllocHGlobal(512);
            ReadProcessMemory(hProcess, item.pszText, ptr, 512, 0);
            string text = Marshal.PtrToStringUni(ptr);
            Marshal.FreeHGlobal(ptr);


            //메모리 해제

            VirtualFreeEx(hProcess, item.pszText, 0, 0x8000);
            VirtualFreeEx(hProcess, ptr, 0, 0x8000);


            return text;
        }

        //콤보박스 인덱스 가져올때 사용
        protected Int32 GetComboIndex(IntPtr _ipWnd)
        {
            //Delay(500);
            Int32 iTemp = 0;
            try
            {
                iTemp = SendMessage(_ipWnd, CB_GETCURSEL, 0, 0);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return iTemp;
        }
       
        //콤보박스 내용 변경할때 사용
        protected void SetComboBox(IntPtr _ipWnd, string _cNewCaption) 
        {
            try
            {
                PostMessage(_ipWnd, CB_SELECTSTRING, IntPtr.Zero, _cNewCaption);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        protected void SetComboBoxIndex(IntPtr _ipWnd, int _Index)
        {
            try
            {
                PostMessage(_ipWnd, CB_SETCURSEL, _Index, 0);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        //탭컨트롤 변경할때 사용
        protected void SetTabControl(IntPtr _ipWnd, int _iTabNumber)
        {
            try
            {
                PostMessage(_ipWnd, TCM_SETCURFOCUS, _iTabNumber, 0);
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        //핸들 찾아서 해당 좌표에 마우스 좌클릭 실행
        protected void Click(IntPtr _ipHwnd , int iTimes = 2/*, int _iX, int _iY*/)
        {
            for(int i=0; i< iTimes; i++)
            {
                PostMessage(_ipHwnd, BM_CLICK, 0, 0);
            }
        }
        public void RClick(IntPtr _ipHwnd, int iTimes = 2/*, int _iX, int _iY*/)
        {
            for (int i = 0; i < iTimes; i++)
            {
                PostMessage(_ipHwnd, WM_RBUTTONUP  , 0, 0);
            }
        }


        //Folder Copy하는 함수
        public void CopyFolder(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
            {
                Directory.CreateDirectory(destFolder);
            }

            string[] files = Directory.GetFiles(sourceFolder);
            string[] folders = Directory.GetDirectories(sourceFolder);

            foreach (string file in files)
            {
                string name = Path.GetFileName(file);
                string dest = Path.Combine(destFolder, name);
                File.Copy(file, dest, true);
            }

            foreach (string folder in folders)
            {
                string name = Path.GetFileName(folder); 
                string dest = Path.Combine(destFolder, name);
                CopyFolder(folder, dest);
            }
        }

        /// <summary>
        /// FOLDER DELETE
        /// </summary>
        /// <param name="path">삭제할 폴더 경로</param>
        /// <returns>true,false반환</returns>
        public static bool DeleteFolder(string path)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                FileInfo[] files = dir.GetFiles("*.*", SearchOption.AllDirectories);
                foreach (FileInfo file in files)
                    file.Attributes = FileAttributes.Normal;
                Directory.Delete(path, true);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool TestMoveWindow(IntPtr _hwnd, int _X, int _Y, int _nWidth, int _nHeight, bool _bRepaint)
        {
            return MoveWindow(_hwnd, _X, _Y, _nWidth, _nHeight, _bRepaint);
        }
        public IntPtr TestFindWindowL(string strClassName, string strWindowName)
        {
            IntPtr iRet = IntPtr.Zero;
            try
            {
                iRet = FindWindow(strClassName, strWindowName);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return iRet;

        }

        public void Delay(int ms)
        {
            DateTime dateTimeNow = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, ms);
            DateTime dateTimeAdd = dateTimeNow.Add(duration);
            while (dateTimeAdd >= dateTimeNow)
            {
                System.Windows.Forms.Application.DoEvents();
                dateTimeNow = DateTime.Now;
            }
            return;
        }
//=========================================================================================
        public void Trace(string _sMsg, [CallerLineNumber] int sourceLineNumber = 0, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "")
        {
            string sHdr = "Macro";
            string sMsg = _sMsg.Replace(",", "");
            string sTag = string.Format("{0:00}", m_iPartId);
            string sFullMsg = string.Format("{0}, {1} ,{2},{3},{4}", sTag, sHdr + " " + sMsg, sourceLineNumber, memberName, sourceFilePath);
            Log.SendMessage(sFullMsg);
        }

    }
 
        
}
                    