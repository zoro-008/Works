using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using COMMON;
using System.Globalization;
using System.IO;

using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Machine
{
    public class Macro : MacroCmd
    {
        public EzSensor Ez1;
        public Dressy   Dr1;
        public int m_iNo = 0;

        //public int m_iPartId;
        public bool CycleEnd;
        public System.Windows.Forms.Timer tmUpdate;
        public ListView lvSampleListView = new ListView();

        public Macro(int _iPartId = 0)
        {
            Ez1 = new EzSensor();
            Dr1 = new Dressy  ();
            m_iPartId = _iPartId;
            CycleEnd = false;

            tmUpdate = new System.Windows.Forms.Timer();
            tmUpdate.Interval = 100;
            tmUpdate.Tick += new EventHandler(tmUpdate_Tick); //
            tmUpdate.Enabled = true;
        }

        public string GetErrCode()
        {
            string sErr = "";
            //if (OM.DevInfo.iMacroType == 0)
            //{
                sErr = Dr1.GetErrCode();
            //}
            //else if (OM.DevInfo.iMacroType == 0)
            //{
                //sErr = Ez1.GetErrCode();
            //}

            if(sErr != "")
            {
                m_iNo = 0;
            }
            return sErr;
        }

        public void Reset()
        {
            //if (OM.DevInfo.iMacroType == 0)
            //{
                Dr1.Reset();
            //}
            tmUpdate = new System.Windows.Forms.Timer();
            tmUpdate.Interval = 100;
            tmUpdate.Tick += new EventHandler(tmUpdate_Tick); //
            tmUpdate.Enabled = true;

        }
        
        //드레시용임 다른거할때는 하나더 만들~
        public void CycleInit(int _iNo,string _sFileName)
        {
            //if (OM.DevInfo.iMacroType == 0)
            //{
                Dr1.InitCycle(_sFileName);
                
                CycleEnd = false;
                m_iNo = _iNo;
                tmUpdate.Enabled = true;
            //}
        }

        public bool Cycle(int _iNo)
        {
            return CycleEnd;
        }

        private void tmUpdate_Tick(object sender, EventArgs e)
        {
           // tmUpdate.Enabled = false;
            bool bRet = false;
            //if (OM.DevInfo.iMacroType == 0 && m_iNo != 0)
            //{
                //Delay(100);
                if(m_iNo ==  1) bRet = Dr1.Cycle1();

                if (bRet)
                {
                    CycleEnd = true;
                    m_iNo = 0;
                    //tmUpdate.Enabled = false;
                    return;
                }
            //}

            
            //else if (OM.DevInfo.iMacroType == 1)
            //{
            //    //Delay(100);
            //    bRet = Ez1.CycleMacro();
            //    if (bRet)
            //    {
            //        CycleEnd = true;
            //        tmUpdate.Enabled = false;
            //        return;
            //    }
            //}
            
            //tmUpdate.Enabled = true;
            
        }
    }

    public class Dressy : MacroCmd
    {
        private int m_iCycle;
        //private int m_iClass;

        private int m_iPreCycle;
        public string sErrMsg;
        private const string m_sPartName = "Dressy ";
        protected CDelayTimer m_tmCycle = new CDelayTimer();
        protected CDelayTimer m_tmDelay = new CDelayTimer();

        public string sFileName;
        private string sSerial  ;
        
        public string GetErrCode() { return sErrMsg; }

        public void InitCycle(string _sFileName)
        {
            //m_iClass    =  0;
            m_iCycle    = 10;
            m_iPreCycle =  0;
            sErrMsg = "";
            sFileName = _sFileName;
        }

        public void Reset() { InitCycle(""); }

        IntPtr m_iHwndM = IntPtr.Zero;
        IntPtr m_iHwnd1 = IntPtr.Zero; //휘발성
        IntPtr m_iHwnd2 = IntPtr.Zero; //휘발성
        IntPtr m_iHwnd3 = IntPtr.Zero; //휘발성
        IntPtr m_iHwnd4 = IntPtr.Zero; //휘발성

        IntPtr m_iHwndS = IntPtr.Zero;

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public bool Cycle1() //Tigger
        {
            String sTemp;
            if (m_tmCycle.OnDelay(m_iCycle != 0 && m_iCycle == m_iPreCycle, 5000))
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
                    //Log_Trace.ShowMessage("Macro", sTemp);
                    return true;


                case 0:
                    return true;

                case 10: //프로그램 실행되어 있는지 확인하고

                    if (!ExistProces(OM.DressyInfo.sAppName1))// || m_iHwndM == IntPtr.Zero)
                    {
                        m_iCycle++;
                        return false;
                    }
                    m_iHwndM = FindWindowL("#32770", "Dressy I/O Sensor Manufacturer Tool");
                    if (m_iHwndM == IntPtr.Zero)
                    {
                        sErrMsg = "ERR002_The program can not be executed";
                        return true;
                    }
                    m_iCycle = 12;
                    return false;

                case 11: //프로그램 실행하기
                    if (!File.Exists(OM.DressyInfo.sAppPath1))
                    {
                        sErrMsg = "ERR002_The program can not be executed";
                        return true;
                    }
                    m_iHwndM = InitProcess1(OM.DressyInfo.sAppPath1, OM.DressyInfo.iPosX1, OM.DressyInfo.iPosY1);
                    m_iCycle++;
                    return false;

                case 12: //시리얼 넘버 확인하기
                    if (!ExistProces(OM.DressyInfo.sAppName1) || m_iHwndM == IntPtr.Zero)
                    {
                        sErrMsg = "ERR002_The program can not be executed";
                        return true;
                    }
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "#32770", null);
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    //m_iHwnd3 = FindWindowIndex(m_iHwnd2, "Static", 9);
                    sSerial = GetWindowText(m_iHwnd2, Tools.Static, 9);
                    if (sSerial == "-")
                    {
                        m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                        m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "#32770", null);
                        m_iHwnd3 = FindWindowChild(m_iHwnd2, IntPtr.Zero, "ComboBox", null);
                        PostMessage(m_iHwnd3, WM_KEYDOWN, 53, 0); //5
                        m_iCycle++;
                        return false;
                    }

                    m_iCycle = 20;
                    return false;

                case 13://시리얼 넘버 클릭하고 확인
                    if (!m_tmDelay.OnDelay(6000)) return false;
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "#32770", null);
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    //m_iHwnd3 = FindWindowIndex(m_iHwnd2, "Static", 9);
                    sSerial = GetWindowText(m_iHwnd2, Tools.Static, 9);
                    if (sSerial == "-")
                    {
                        sErrMsg = "ERR003_디바이스 리스트를 클릭하였으나 시리얼 번호가 여전히 - 로 변경이 안됨.";
                        return true;
                    }

                    m_iCycle = 20;
                    return false;

                case 20: //겟 다크 클릭
                    m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                    m_iHwnd2 = FindWindowIndex(m_iHwnd1, "#32770", Cal);
                    if (m_iHwnd2 == IntPtr.Zero) return false;
                    SetTabControl(m_iHwnd1, Cal);
                    //m_iHwnd3 = FindWindowIndex(m_iHwnd1, "#32770", 2);
                    m_iHwnd3 = FindWindowChild(m_iHwnd2, IntPtr.Zero, "Button", "Get Dark");
                    if (m_iHwnd3 == IntPtr.Zero) return false;

                    Click(m_iHwnd3);
                    m_iCycle++;
                    return false;

                case 21://트리거 파일 체크하고 겟 브라이트 클릭
                    if (!m_tmDelay.OnDelay(iDly)) return false;
                    m_iHwnd1 = FindWindowL("#32770", null);
                    if (IsWindowVisible(m_iHwnd1) == 1) return false;


                    sName = Path.GetDirectoryName(OM.DressyInfo.sAppPath1) + "\\Cal\\offset.raw";
                    if (!File.Exists(sName)) //트리거 불량
                    {
                        sErrMsg = "ERR004_Offset.raw 파일 생성 실패(Get Dark)";
                        return true;
                    }
                    
                    sPath = @OM.CmnOptn.sLeftFolder + @"\" + DateTime.Now.ToString("yyyyMMdd") + @"\" + LOT.GetLotNo() + @"\" + sFileName + ".raw";
                    if( File.Exists(sPath)) File.Delete(sPath);

                    if (!Directory.Exists(Path.GetDirectoryName(sPath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(sPath));
                    }
                    if(!File.Exists(sPath)) File.Copy(sName, sPath);
                    File.Delete(sName);
                    
                    m_iCycle = 0;
                    return true;
            }
        }

        public void SaveCsv()
        {
            string sPath = "";//@"D:\Macro\" + Eqp.sEqpName + @"\" + DateTime.Now.ToString("yyyyMMdd") + @"\" + OM.DressyInfo.sSerialID + ".csv";
            string sDir = Path.GetDirectoryName(sPath + "\\");
            string sName = Path.GetFileName(sPath);
            if (!CIniFile.MakeFilePathFolder(sDir)) return;

            FileStream fs = new FileStream(sPath, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
            string line;
            line =
            DateTime.Now.ToString("yyyyMMdd-hhmmss") + "\r\n" ;
            //"LineNoise   " + "," + Noise.LineNoise + "\r\n" +
            //"DataNoise   " + "," + Noise.LineNoise + "\r\n" +
            //"Median      " + "," + Noise.LineNoise + "\r\n" +
            //"Fluc        " + "," + Noise.LineNoise + "\r\n" +
            //"Min         " + "," + Noise.LineNoise + "\r\n" +
            //"Max         " + "," + Noise.LineNoise + "\r\n" +
            //"Dispersion  " + "," + Noise.LineNoise + "\r\n" +
            //"DynamicRange" + "," + Noise.LineNoise + "\r\n" +
            //"TotalNoise  " + "," + Noise.LineNoise;

            sw.WriteLine(line);
            sw.Close();
            fs.Close();



        }

        public void Log_Trace(string sLog_Trace, [CallerLineNumber] int sourceLineNumber = 0)
        {
            Debug.WriteLine(DateTime.Now.ToString("MM-dd hh:mm:ss:fff_") + sLog_Trace + "_" + sourceLineNumber.ToString());
        }

    }

    public class EzSensor : MacroCmd
    {
        private int m_iCycle;
        private int m_iPreCycle;
        public string sErrMsg;
        private const string m_sPartName = "EzSensor ";
        protected CDelayTimer m_tmCycle = new CDelayTimer();
        protected CDelayTimer m_tmDelay = new CDelayTimer();

        public struct SNoise
        {
            public string LineNoise;
            public string DataNoise;
            public string Median;
            public string Fluc;
            public string Min;
            public string Max;
            public string Dispersion;
            public string DynamicRange;
            public string TotalNoise;
        };
        SNoise Noise;

        public string GetErrCode() { return sErrMsg; }

        public void InitCycle()
        {
            m_iCycle = 10;
            m_iPreCycle = 0;
            sErrMsg = "";
        }

        public void Reset() { InitCycle(); }

        IntPtr m_iHwndM = IntPtr.Zero;
        IntPtr m_iHwnd1 = IntPtr.Zero; //휘발성
        IntPtr m_iHwnd2 = IntPtr.Zero; //휘발성
        IntPtr m_iHwnd3 = IntPtr.Zero; //휘발성
        IntPtr m_iHwnd4 = IntPtr.Zero; //휘발성

        IntPtr m_iHwndS = IntPtr.Zero;

        public bool CycleMacro()
        {
            /*
                        String sTemp;
                        if (m_tmCycle.OnDelay(m_iCycle != 0 && m_iCycle == m_iPreCycle, 5000))
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

                        int iDly = 200;

                        switch (m_iCycle)
                        {

                            default:
                                sTemp = string.Format("Cycle_Default_Clear_m_iCycle={0:000}", m_iCycle);
                                sErrMsg = sTemp;
                                //Log_Trace.ShowMessage("Macro", sTemp);
                                return true;


                            case 0:
                                return true;

                            case 10: //EZSENSOR 프로그램 시작
                                m_iCycle++;
                                return false;

                            case 11:
                                //if (!ExitProcess(OM.EzSensor.sAppName1)) return false; //TODO :: 잠시 삭제
                                m_iHwndM = InitProcess1(OM.EzSensor.sAppPath1,OM.EzSensor.iPosX1,OM.EzSensor.iPosY1);
                                m_iCycle++;
                                return false;

                            case 12:
                                if (!ExistProces(OM.EzSensor.sAppName1) || m_iHwndM == IntPtr.Zero)
                                {
                                    sErrMsg = "The program can not be executed";
                                    Trace(sErrMsg);
                                    return true;
                                }
                                m_iCycle = 20;
                                return false;

                            case 20: //Settings Detector's settings 클릭후 셋팅값 셋팅후 OK 클릭
                                m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, sClassName1, "Settings");
                                m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "Detector's settings");
                                if (m_iHwnd2 == IntPtr.Zero) return false;

                                Click(m_iHwnd2); //Post 두번 보내는거 테스트 필요.
                                m_iCycle++;
                                return false;

                            case 21:
                                if (!m_tmDelay.OnDelay(iDly)) return false;
                                m_iHwnd1 = FindWindowL("#32770", "Detector settings");
                                if (m_iHwnd1 == IntPtr.Zero) return false;

                                //SetComboBox
                                SetWindow(m_iHwnd1, Tools.ComboBox, 0, OM.EzSensor.iProductID);
                                SetWindow(m_iHwnd1, Tools.ComboBox, 1, OM.EzSensor.iDescramble);
                                SetWindow(m_iHwnd1, Tools.ComboBox, 2, OM.EzSensor.iVRest);
                                SetWindow(m_iHwnd1, Tools.ComboBox, 3, OM.EzSensor.iBinning);
                                SetWindow(m_iHwnd1, Tools.ComboBox, 4, OM.EzSensor.iMode);
                                SetWindowText(m_iHwnd1, Tools.ComboBox, 5, OM.EzSensor.sSerialID); //TODO :: 확인 필요
                                SetWindow(m_iHwnd1, Tools.ComboBox, 6, OM.EzSensor.iPattern);

                                //SetButton
                                SetWindow(m_iHwnd1, Tools.Button, 4, OM.EzSensor.bInvertacq ? 1 : 0);
                                //SetWindow(m_iHwnd1, Tools.Button, 5, OM.EzSensor.bEnableSerial ? 1:0);
                                SetWindow(m_iHwnd1, Tools.Button, 7, OM.EzSensor.bBright61 ? 1 : 0);
                                SetWindow(m_iHwnd1, Tools.Button, 8, OM.EzSensor.bDebugdump ? 1 : 0);

                                //SetEdit
                                SetWindowText(m_iHwnd1, Tools.Edit, 0, OM.EzSensor.iTimeout.ToString());
                                SetWindowText(m_iHwnd1, Tools.Edit, 1, OM.EzSensor.iOntheFly.ToString());
                                SetWindowText(m_iHwnd1, Tools.Edit, 2, OM.EzSensor.iDarkOffset.ToString());
                                SetWindowText(m_iHwnd1, Tools.Edit, 3, OM.EzSensor.iGain.ToString());
                                SetWindowText(m_iHwnd1, Tools.Edit, 4, OM.EzSensor.iCutoffbevel.ToString());
                                SetWindowText(m_iHwnd1, Tools.Edit, 5, OM.EzSensor.iCutoffR.ToString());

                                m_iCycle++;
                                return false;

                            case 22: //셋팅하고 셋팅값 확인하기.
                                if (!m_tmDelay.OnDelay(iDly)) return false;
                                m_iHwnd1 = FindWindowL("#32770", "Detector settings");
                                if (m_iHwnd1 == IntPtr.Zero) return false;

                                //SetComboBox
                                if(GetWindow(m_iHwnd1, Tools.ComboBox, 0) !=  OM.EzSensor.iProductID    ) sErrMsg = "ProductID Setting Fail";
                                if(GetWindow(m_iHwnd1, Tools.ComboBox, 1) !=  OM.EzSensor.iDescramble   ) sErrMsg = "Descramble Setting Fail";
                                if(GetWindow(m_iHwnd1, Tools.ComboBox, 2) !=  OM.EzSensor.iVRest        ) sErrMsg = "VRest Setting Fail";
                                if(GetWindow(m_iHwnd1, Tools.ComboBox, 3) !=  OM.EzSensor.iBinning      ) sErrMsg = "Binning Setting Fail";
                                if(GetWindow(m_iHwnd1, Tools.ComboBox, 4) !=  OM.EzSensor.iMode         ) sErrMsg = "Mode Setting Fail";
                                if(GetWindowText(m_iHwnd1, Tools.ComboBox, 5) !=  OM.EzSensor.sSerialID ) sErrMsg = "SerialID Setting Fail"; //TODO :: 확인 필요
                                if(GetWindow(m_iHwnd1, Tools.ComboBox, 6) !=  OM.EzSensor.iPattern      ) sErrMsg = "Pattern Setting Fail";

                                //SetButton
                                if(GetWindow(m_iHwnd1, Tools.Button, 4) !=  (OM.EzSensor.bInvertacq ? 1 : 0)) sErrMsg = "Invertacq Setting Fail";
                                //if(GetWindow(m_iHwnd1, Tools.Button, 5) !=  OM.EzSensor.bEnableSerial ? 1:0) sErrMsg = "EnableSerial Setting Fail";
                                if(GetWindow(m_iHwnd1, Tools.Button, 7) !=  (OM.EzSensor.bBright61  ? 1 : 0)) sErrMsg = "Bright61 Setting Fail";
                                if(GetWindow(m_iHwnd1, Tools.Button, 8) !=  (OM.EzSensor.bDebugdump ? 1 : 0)) sErrMsg = "Debugdump Setting Fail";

                                //SetEdit
                                if(GetWindowText(m_iHwnd1, Tools.Edit, 0) !=  OM.EzSensor.iTimeout.ToString()    ) sErrMsg = "Timeout Setting Fail";
                                if(GetWindowText(m_iHwnd1, Tools.Edit, 1) !=  OM.EzSensor.iOntheFly.ToString()   ) sErrMsg = "OntheFly Setting Fail";
                                if(GetWindowText(m_iHwnd1, Tools.Edit, 2) !=  OM.EzSensor.iDarkOffset.ToString() ) sErrMsg = "DarkOffset Setting Fail";
                                if(GetWindowText(m_iHwnd1, Tools.Edit, 3) !=  OM.EzSensor.iGain.ToString()       ) sErrMsg = "Gain Setting Fail";
                                if(GetWindowText(m_iHwnd1, Tools.Edit, 4) !=  OM.EzSensor.iCutoffbevel.ToString()) sErrMsg = "Cutoffbevel Setting Fail";
                                if(GetWindowText(m_iHwnd1, Tools.Edit, 5) !=  OM.EzSensor.iCutoffR.ToString()    ) sErrMsg = "CutoffR Setting Fail";

                                if(sErrMsg != "")
                                {
                                    Trace(sErrMsg);
                                    return true;
                                }
                                m_iCycle++;
                                return false;

                            case 23:
                                if (!m_tmDelay.OnDelay(iDly)) return false;
                                m_iHwnd1 = FindWindowL(sClassName1, "Detector settings");
                                m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "OK");
                                if (m_iHwnd2 == IntPtr.Zero) return false;
                                Click(m_iHwnd2);
                                m_iCycle = 30;
                                return false;

                            case 30: //Records Serial에는 아마 위에서 셋팅해서 들어가 있을거임 , Record 버튼 클릭후 Read클릭후 비교.
                                if (!m_tmDelay.OnDelay(iDly)) return false;
                                m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                                if (m_iHwnd1 == IntPtr.Zero) return false;
                                SetTabControl(m_iHwnd1, Records);
                                m_iCycle++;
                                return false;

                            case 31: //Serial 을 넣고 Record 클릭후 Read 하여 확인하면 될듯.
                                if (!m_tmDelay.OnDelay(iDly)) return false;
                                m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "#32770", "Records");
                                m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "Record");
                                if (m_iHwnd2 == IntPtr.Zero) return false;
                                SetWindowText(m_iHwnd1, Tools.Edit, 1, OM.EzSensor.sSerialID);
                                Click(m_iHwnd2);
                                m_iCycle++;
                                return false;

                            case 32:
                                if (!m_tmDelay.OnDelay(500)) return false;
                                m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "#32770", "Records");
                                m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "Record");
                                m_iHwnd3 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "Read");
                                if (m_iHwnd2 == IntPtr.Zero) return false;
                                if (IsWindowEnabled(m_iHwnd2) != 1) return false;
                                if (IsWindowEnabled(m_iHwnd3) != 1) return false;
                                m_iCycle++;
                                return false;

                            case 33:
                                if (!m_tmDelay.OnDelay(iDly)) return false;
                                m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "#32770", "Records");
                                m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "Read");
                                if (m_iHwnd2 == IntPtr.Zero) return false;
                                Click(m_iHwnd2);
                                m_iCycle++;
                                return false;

                            case 34:
                                if (!m_tmDelay.OnDelay(iDly)) return false;
                                m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "#32770", "Records");
                                m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "Read");
                                if (m_iHwnd2 == IntPtr.Zero) return false;
                                if (IsWindowEnabled(m_iHwnd2) != 1) return false;
                                m_iCycle++;
                                return false;

                            case 35:
                                if (!m_tmDelay.OnDelay(iDly)) return false;
                                m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "#32770", "Records");
                                m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "Record");
                                if (m_iHwnd2 == IntPtr.Zero) return false;
                                if (GetWindowText(m_iHwnd1, Tools.Edit, 2) != OM.EzSensor.sSerialID)
                                {
                                    sErrMsg = "Recorded Serial is Differant";
                                    Trace(sErrMsg);
                                    return true;
                                }
                                //여기서 쓴거랑 비교ㄱㄱ.
                                m_iCycle = 40;
                                return false;

                            case 40: //Calibration Get Dark 클릭후 Acquiring dark frame 창 나타낫다 없어져야함.
                                if (!m_tmDelay.OnDelay(iDly)) return false;
                                m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                                if (m_iHwnd1 == IntPtr.Zero) return false;
                                SetTabControl(m_iHwnd1, Calibration);
                                m_iCycle++;
                                return false;

                            case 41:
                                if (!m_tmDelay.OnDelay(iDly)) return false;
                                m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "#32770", "Calibration");
                                m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "Get &Dark");
                                if (m_iHwnd2 == IntPtr.Zero) return false;
                                Click(m_iHwnd2);
                                m_iCycle++;
                                return false;

                            case 42:
                                m_iHwndS = FindWindowL("#32770", "Acquiring dark frame"); //캡션에 퍼센테이지가 올라가서 끝까지 유지가 안됨.
                                if (m_iHwndS == IntPtr.Zero) return false;
                                m_iCycle++;
                                return false;

                            case 43:
                                m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "button", null); //cancel 버튼이 하나 있음
                                if (m_iHwnd1 != IntPtr.Zero) return false;
                                m_iCycle = 50;
                                return false;

                            case 50: //Aging Purge클릭후 Yes 클릭후 Active 후 50장 
                                if (!m_tmDelay.OnDelay(iDly)) return false;
                                m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "SysTabControl32", null);
                                if (m_iHwnd1 == IntPtr.Zero) return false;
                                SetTabControl(m_iHwnd1, Aging);
                                m_iCycle++;
                                return false;

                            case 51: //Aging Purge Button Click
                                if (!m_tmDelay.OnDelay(iDly)) return false;
                                m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "#32770", "Aging");
                                m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "Purge");
                                if (m_iHwnd2 == IntPtr.Zero) return false;
                                Click(m_iHwnd2);
                                m_iCycle++;
                                return false;

                            case 52: //Purge 후 확인 취소창이 에서 확인 클릭.
                                if (!m_tmDelay.OnDelay(iDly)) return false;
                                m_iHwnd1 = FindWindowL("#32770", "IntraOral Detector");
                                m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "확인");
                                if (m_iHwnd2 == IntPtr.Zero) return false;
                                Click(m_iHwnd2);
                                m_iCycle++;
                                return false;

                            case 53: //확인 취소창이 없어졋는지 확인.
                                if (!m_tmDelay.OnDelay(true, iDly)) return false;
                                m_iHwnd1 = FindWindowL("#32770", "IntraOral Detector");
                                if (m_iHwnd1 != IntPtr.Zero) return false;
                                m_iCycle++;
                                return false;

                            case 54: //Aging Active Button Click
                                if (!m_tmDelay.OnDelay(true, iDly)) return false;
                                m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "#32770", "Aging");
                                m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "Active");
                                if (m_iHwnd2 == IntPtr.Zero) return false;
                                Click(m_iHwnd2,1);
                                m_iCycle++;
                                return false;

                            case 55: //Aging Active 클릭후 Not Active 아닌거 체크.
                                if (!m_tmDelay.OnDelay(true, iDly)) return false;
                                m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "#32770", "Aging");
                                m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Static", "not active");
                                if (m_iHwnd2 != IntPtr.Zero) return false;
                                m_iCycle = 60;
                                return false;

                            //오래 걸리니깐 50장 해야 해서 다음프로그램 먼저 하고 보자.
                            case 60:
                                if (!ExitProcess(OM.EzSensor.sAppName2)) return false;
                                m_iHwndS = InitProcess2(OM.EzSensor.sAppPath2,OM.EzSensor.iPosX2,OM.EzSensor.iPosY2);
                                m_iCycle++;
                                return false;

                            case 61:
                                if (!ExistProces(OM.EzSensor.sAppName2) || m_iHwndS == IntPtr.Zero)
                                {
                                    sErrMsg = "The program can not be executed";
                                    Trace(sErrMsg);
                                    return true;
                                }
                                m_iCycle = 70;
                                return false;

                            case 70: //Noise 탭핸들에서 Width Height 변경
                                if (!m_tmDelay.OnDelay(true, iDly)) return false;
                                m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "#32770", "ElectronicNoisePerformance");
                                if (m_iHwnd1 == IntPtr.Zero) return false;
                                SetWindowText(m_iHwnd1, Tools.Edit, 1, OM.EzSensor.iNsWidth.ToString());
                                SetWindowText(m_iHwnd1, Tools.Edit, 2, OM.EzSensor.iNsHeight.ToString());
                                m_iCycle++;
                                return false;

                            case 71:
                                if (!m_tmDelay.OnDelay(true, iDly)) return false;
                                m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "#32770", "ElectronicNoisePerformance");
                                if (m_iHwnd1 == IntPtr.Zero) return false;
                                SetWindowText(m_iHwnd1, Tools.Edit, 7, OM.EzSensor.sDarkImgPath);
                                m_iCycle++;
                                return false;

                            case 72:
                                if (!m_tmDelay.OnDelay(true, iDly)) return false;
                                m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "#32770", "ElectronicNoisePerformance");
                                m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "RUN");
                                if (m_iHwnd2 == IntPtr.Zero) return false;
                                Click(m_iHwnd2);
                                m_iCycle++;
                                return false;

                            case 73:
                                if (!m_tmDelay.OnDelay(iDly)) return false;
                                m_iHwnd1 = FindWindowL("#32770", "ENP_Inspection");
                                m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Static", "확인");
                                if (GetWindowText(m_iHwnd1, Tools.Static, 0).IndexOf("Complete") == -1)
                                {
                                    sErrMsg = "Noise Program Run Result Error";
                                    Trace(sErrMsg);
                                    return true;
                                }
                                m_iCycle++;
                                return false;

                            case 74:
                                if (!m_tmDelay.OnDelay(true, iDly)) return false;
                                m_iHwnd1 = FindWindowL("#32770", "ENP_Inspection");
                                m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Button", "확인");
                                if (m_iHwnd2 == IntPtr.Zero) return false;
                                Click(m_iHwnd2);
                                m_iCycle++;
                                return false;

                            case 75: //확인 취소창이 없어졋는지 확인.
                                if (!m_tmDelay.OnDelay(true, iDly)) return false;
                                m_iHwnd1 = FindWindowL("#32770", "ENP_Inspection");
                                if (m_iHwnd1 != IntPtr.Zero) return false;
                                m_iCycle++;
                                return false;

                            case 76:
                                if (!m_tmDelay.OnDelay(true, iDly)) return false;
                                m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "#32770", "ElectronicNoisePerformance");
                                if (m_iHwnd1 == IntPtr.Zero) return false;

                                Noise.LineNoise = GetWindowText(m_iHwnd1, Tools.Edit, 11);
                                Noise.DataNoise = GetWindowText(m_iHwnd1, Tools.Edit, 12);
                                Noise.Median = GetWindowText(m_iHwnd1, Tools.Edit, 13);
                                Noise.Fluc = GetWindowText(m_iHwnd1, Tools.Edit, 14);
                                Noise.Min = GetWindowText(m_iHwnd1, Tools.Edit, 15);
                                Noise.Max = GetWindowText(m_iHwnd1, Tools.Edit, 16);
                                Noise.Dispersion = GetWindowText(m_iHwnd1, Tools.Edit, 17);
                                Noise.DynamicRange = GetWindowText(m_iHwnd1, Tools.Edit, 19);
                                Noise.TotalNoise = GetWindowText(m_iHwnd1, Tools.Edit, 8);

                                SaveCsv();
                                m_iCycle++;
                                return false;

                            case 77:
                                if (!m_tmDelay.OnDelay(true, iDly)) return false;
                                m_iHwnd1 = FindWindowChild(m_iHwndS, IntPtr.Zero, "Button", "확인");
                                if (m_iHwnd1 == IntPtr.Zero) return false;
                                Click(m_iHwnd1);
                                m_iCycle = 80;
                                return false;

                            case 80:
                                if (!m_tmDelay.OnDelay(true, iDly)) return false;
                                m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "#32770", "Aging");
                                m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Static", "not active");
                                m_iHwnd3 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Static", "connecting");
                                m_iHwnd4 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Static", "acquiring");
                                if (m_iHwnd3 != IntPtr.Zero || m_iHwnd4 != IntPtr.Zero)
                                {
                                    m_iCycle++;
                                    return false;
                                }
                                m_iCycle++;
                                return false;

                            case 81:
                                if (!m_tmDelay.OnDelay(true, iDly)) return false;
                                m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "#32770", "Aging");
                                m_iHwnd2 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Static", "not active");
                                m_iHwnd3 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Static", "connecting");
                                m_iHwnd4 = FindWindowChild(m_iHwnd1, IntPtr.Zero, "Static", "acquiring");
                                if (m_iHwnd3 != IntPtr.Zero || m_iHwnd4 != IntPtr.Zero)
                                {
                                    m_iCycle = 80;
                                    return false;
                                }
                                if (m_iHwnd2 == IntPtr.Zero) return false;
                                m_iCycle++;
                                return false;

                            case 82:
                                if (!m_tmDelay.OnDelay(true, iDly)) return false;
                                m_iHwnd1 = FindWindowChild(m_iHwndM, IntPtr.Zero, "Button", "확인");
                                if (m_iHwnd1 == IntPtr.Zero) return false;
                                Click(m_iHwnd1);
                                m_iCycle = 0;
                                return true;


                        }
            */
            return true;
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
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, UInt32 uFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr OpenProcess(UInt32 dwDesiredAccess, Int32 bInheritHandle, UInt32 dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint dwFreeType);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, int nSize, int lpNumberOfBytesWritten);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
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

        protected IntPtr InitProcess1(string _sAppPath,int x = 0, int y = 0)
        {
            Proc1.StartInfo.FileName = _sAppPath;
            try
            {
                Proc1.Start();
                //Proc1.WaitForInputIdle();
                Proc1.WaitForExit(1000);
                //SetWindowPos(Proc1.MainWindowHandle,IntPtr.Zero,x,y,0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);
                //Proc1.WaitForExit(1000);
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            //SetWindowPos(Proc1.MainWindowHandle,IntPtr.Zero,x,y,0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);
            return Proc1.MainWindowHandle;
        }

        protected IntPtr InitProcess2(string _sAppPath,int x = 0, int y = 0)
        {
            Proc2.StartInfo.FileName = _sAppPath;
            try
            {
                Proc2.Start();
                Proc2.WaitForInputIdle(); 
                SetWindowPos(Proc2.MainWindowHandle,IntPtr.Zero,x,y,0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);
                Proc2.WaitForExit(1000);
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            //SetWindowPos(Proc2.MainWindowHandle,IntPtr.Zero,x,y,0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);
            return Proc2.MainWindowHandle;
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

        public void ProcessExited(object source, EventArgs e)
        {
            //Proc.Close();
        }

        
        //프로그램 종료
        //protected void CloseProcess(IntPtr _ipHandle)
        //{
            //SendMessage(_ipHandle , WM_CLOSE , IntPtr.Zero ,IntPtr.Zero);
        //}
        protected IntPtr FindWindowL(string strClassName, string strWindowName) 
        //protected IntPtr FindWindowL(IntPtr _ipParent, IntPtr _ipFrom, string _sClass, string _sCaption)
        {
            //Delay(50);
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
            //IntPtr hCombo = FindWindowChild(_ipParent, IntPtr.Zero, "ComboBox", null);
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
                //if(Tool == Tools.Edit       )
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
            uint pid;

            //핸들을 이용하여 프로세스 id를 얻어 온다.
            GetWindowThreadProcessId(handle, out pid);


            //해당 프로세스의 핸들을 얻어 옵니다.
            IntPtr hProcess = OpenProcess(0x0008 | 0x0010 | 0x0020 | 0x0400, 0, pid);

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
            //Delay(500);
            try
            {
                //SendMessage(_ipWnd, CB_SELECTSTRING, IntPtr.Zero, _cNewCaption);
                PostMessage(_ipWnd, CB_SELECTSTRING, IntPtr.Zero, _cNewCaption);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        protected void SetComboBoxIndex(IntPtr _ipWnd, int _Index)
        {
            //Delay(500);
            try
            {
                //SendMessage(_ipWnd, CB_SETCURSEL, _Index, 0);
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
            //Delay(500);
            //SetForegroundWindow(_ipWnd);
            try
            {
                //SendMessage(_ipWnd, TCM_SETCURFOCUS, _iTabNumber, 0);
                PostMessage(_ipWnd, TCM_SETCURFOCUS, _iTabNumber, 0);
            }
            catch(Exception e)
            {
                //string sMsg = e.Message;
                Debug.WriteLine(e.Message);
            }
        }

        //핸들 찾아서 해당 좌표에 마우스 좌클릭 실행
        protected void Click(IntPtr _ipHwnd , int iTimes = 2/*, int _iX, int _iY*/)
        {
            //SetForegroundWindow(_ipHwnd);
            //Delay(500);
            for(int i=0; i< iTimes; i++)
            {
                PostMessage(_ipHwnd, BM_CLICK, 0, 0);
            }
            //SetForegroundWindow(_ipHwnd);
            //PostMessage(_ipHwnd, WM_LBUTTONDOWN, 0, 0);
            //Delay(500);
            //PostMessage(_ipHwnd, WM_LBUTTONUP, 0, 0);
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
                File.Copy(file, dest);
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
            return;// DateTime.Now;
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
                    