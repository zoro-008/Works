using SML;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using COMMON;
using System.Management;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Machine
{
    public partial class FormMain : Form
    {
        public FormOperation    FrmOperation   ;
        public FormDevice       FrmDevice      ;
        //public FormPassword     FrmPassword  ; 
        public FormMaster       FrmMaster      ;
        public FormSpectroMain  FrmSpectroMain ;
        public FormVersion      FrmVersion     ;
        public FormOption       FrmOption      ;


        public int m_iActivePage = 0;
        public EN_SEQ_STAT m_iPreStat;

        public EmbededExe SpectroMeter = new EmbededExe("SpectraSuite","SunAwtFrame",@"C:\Program Files\Ocean Optics\SpectraSuite\spectrasuite\bin\SpectraSuite.exe");

        private const string sFormText = "Form Main ";

        public FormMain()
        {
            //this.TopMost = true;
            SEQ.Init();
            OM.LoadLastInfo();
            
            InitializeComponent();
            //UsbNotification.RegisterUsbDeviceNotification(this.Handle);
            m_iActivePage = 0;

            FrmOperation   = new FormOperation  (pnBase);
            FrmDevice      = new FormDevice     (this  );
            FrmMaster      = new FormMaster     ();
            if(!OM.EqpOptn.bIgnrSptr) FrmSpectroMain = new FormSpectroMain();
            FrmVersion     = new FormVersion    ();
            FrmOption      = new FormOption     (pnBase);



            FrmDevice.TopLevel = false ;
            FrmDevice.Parent   = pnBase;
            
            //lbLotNo.Text  = LOT.GetLotNo();
            lbName.Text = Eqp.sEqpName;

            //파일 버전 보여주는 부분
            string sFileVersion = System.Windows.Forms.Application.ProductVersion;
            lbVer.Text = "Version " + sFileVersion;

            ShowPage(0);//Operation.

            //SM.FrmLogOn.SetLevel(EN_LEVEL.Master);

            //SpectroMeter.Init();




            tmUpdate.Enabled = true ;

        }
         ~FormMain()
        {
            //SpectroMeter.Close();



            //MessageBox.Show("123", "123");
        }

        private void FormMain_VisibleChanged(object sender, EventArgs e)
        {
            if(!OM.EqpOptn.bIgnrSptr && Visible)
            {
                FrmSpectroMain.Show();
                FrmSpectroMain.Left   = 1281 ;
                FrmSpectroMain.Top    = 0    ;
                FrmSpectroMain.Width  = 1280 ;
                FrmSpectroMain.Height = 1024 ;
            }
        }

        public void ShowPage(int _iPageIdx)
        {
            //FrmDeviceSet.UpdateDevInfo(true);
            //PM.UpdatePstn(true);
            //PM.Load(OM.GetCrntDev());

            switch (_iPageIdx)
            {
                case 0: FrmOperation.Show();          break;
                case 1: FrmDevice   .Show();          break;
                case 3: FrmOption   .Show();          break;
                case 5: SM.SetDllMainWin(ref pnBase); break;
                default: FrmOperation.Show();         break;
            }
          
        }
        private void HidePage(int _iPageIdx)
        {
            switch (_iPageIdx)
            {
                case 0: FrmOperation.Hide();          break;
                case 1: FrmDevice   .Hide();          break;
                case 3: FrmOption   .Hide();          break;
                case 5: SM.HideDllMainWin();          break;
                default: FrmOperation.Hide();         break;
            }
        }

        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            tmUpdate.Enabled = false ;
            lbMainThreadTime.Text = string.Format("M:{0:0.000ms}", SEQ._dMainThreadCycleTime);
            lbRecThreadTime .Text = string.Format("C:{0:0.000ms}", ThreadRecorder.m_dThreadCycleTime);
            
            //Set Sequence State.
            string sStatText = "";
            Color  StatColor = Color.Black ;

            switch(SEQ._iSeqStat) {
                default                  :                                                                                      break;
                case EN_SEQ_STAT.Init    : sStatText = "INIT"       ; StatColor = Color.Blue                                  ; break;
                case EN_SEQ_STAT.Warning : sStatText = "WARNING"    ; StatColor = Color.Gold                                  ; break;
                case EN_SEQ_STAT.Error   : sStatText = "ERROR"      ; StatColor = SEQ._bFlick ? Color.Gold : Color.Red        ; break;
                case EN_SEQ_STAT.Running : sStatText = "RUNNING"    ; StatColor = Color.Lime                                  ; break;
                case EN_SEQ_STAT.Stop    : sStatText = "STOP"       ; StatColor = Color.Black                                 ; break;
                case EN_SEQ_STAT.RunWarn : sStatText = "WORK END"   ; StatColor = SEQ._bFlick ? Color.Gray : Color.Black      ; break;
                case EN_SEQ_STAT.WorkEnd : sStatText = "LOTEND"     ; StatColor = Color.Gray                                  ; break;
                case EN_SEQ_STAT.Manual  : sStatText = "MANUAL"     ; StatColor = Color.Blue                                  ; break;
                case EN_SEQ_STAT.ToStart : sStatText = "TO START"   ; StatColor = Color.Gold                                  ; break;
                case EN_SEQ_STAT.ToStop  : sStatText = "TO STOP"    ; StatColor = Color.Gold                                  ; break;
            }

            if(OM.MstOptn.bDebugMode){
                sStatText = "DEBUG " + sStatText ;
            }

            lbStat.Text = sStatText ;
            lbStat.ForeColor = StatColor ;
            lbDate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            //접근 레벨 Operator에서 Option/Util 버튼 비활성화(패스워드 방식)
            btDevice.Enabled = !SEQ._bRun;
            btUtil.Enabled   = SM.FrmLogOn.GetLevel() != (int)EN_LEVEL.LogOff ;
            btExit.Enabled   = !SEQ._bRun;

            //레이저 보호용, LED 보호용
            if (ML.IO_GetY(yi.LaserOnOff  )) {if(!sw1.IsRunning) sw1.Restart(); }
            else sw1.Stop();
            if (sw1.ElapsedMilliseconds > OM.CmnOptn.iLsrMaxDelay) { ML.IO_SetY(yi.LaserOnOff   ,false); sw1.Stop(); }

            //레이저 바닥 보호용
            if(!ML.MT_GetHomeDone(mi.LaserX) || ML.MT_GetCmdPos(mi.LaserX) < OM.CmnOptn.dLaserCheck)
            {
                ML.IO_SetY(yi.LaserOnOff   ,false);
            }

            //if (ML.IO_GetY(yi.SepctroLight)) {if(!sw2.IsRunning) sw2.Restart(); }
            //else sw2.Stop();
            //if (sw2.ElapsedMilliseconds > 5000) { ML.IO_SetY(yi.SepctroLight, false); sw2.Stop(); }

            //장비명 표기
            lbName.Text = OM.EqpOptn.sEquipName ;
            
            //현재 디바이스 표기
            lbDevice.Text = OM.GetCrntDev().ToString();

            //옵션 표기
            pnOptn3.Visible = OM.CmnOptn.bIgnrDoor && SEQ._bFlick ;



            if (OM.Info.bTempSave)
            {
                if(!tmTemp.Enabled) {
                    tmTemp.Interval = OM.Info.iTempSaveInterval;
                    tmTemp.Enabled  = true;
                }
            }



            tmUpdate.Enabled = true  ;

            
        }
        Stopwatch sw1 = new Stopwatch();
        Stopwatch sw2 = new Stopwatch();


        private void btExit_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            //OM.EqpStat.dLastIDXRPos = ML.MT_GetEncPos(mi.IDXR_XRear);
            //OM.EqpStat.dLastIDXFPos = ML.MT_GetEncPos(mi.IDXF_XFrnt);
            
            if (Log.ShowMessageModal("Confirm", "Do you want to exit the program ?") != DialogResult.Yes) return;
            Log.Trace("Program End", ti.Sts);

            if(FrmSpectroMain != null) FrmSpectroMain.Close();
            //if (!OM.EqpOptn.bIgnrSptr) FrmSpectroMain.Close();
            FrmOperation.Close();
            FrmDevice   .Close();
            FrmMaster   .Close(); 
            



            SEQ.Close();
                   
            
            Close();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmUpdate.Enabled = false;
            tmTemp.Enabled   = false;
            FrmOperation.Close();
            FrmDevice   .Close();
            //FrmPassword .Close();
            FrmMaster   .Close();
        }

        private void lbName_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string sText = ((Label)sender).Text;
            Log.Trace(sFormText + sText + " Label Double Clicked", ti.Frm);

            if (FrmMaster.IsDisposed)
            {
                FrmMaster = new FormMaster();
            }
            FrmMaster.Show();
        }

        private void btMinimization_Click_1(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        //private void button1_Click(object sender, EventArgs e)
        //{
        //}

        //private void lbVer_DoubleClick(object sender, EventArgs e)
        //{

        //}

        private void btOperation_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            HidePage(m_iActivePage);
            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);
            m_iActivePage = iBtnTag;
            ShowPage(m_iActivePage);
        }

        private void panel1_DoubleClick(object sender, EventArgs e)
        {
            //FrmSpectroMain.Left   = 1281 ;
            //FrmSpectroMain.Top    = 0    ;
            //FrmSpectroMain.Width  = 1280 ;
            //FrmSpectroMain.Height = 1024 ;
            //FrmSpectroMain.Show();
        }

        private void lbVer_DoubleClick(object sender, EventArgs e)
        {
            if (FrmVersion.IsDisposed)
            {
                FrmVersion = new FormVersion();
            }
            FrmVersion.Owner = this;
            FrmVersion.Show();
        }

        private void lbDate_DoubleClick(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void panel1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string sText = ((Panel)sender).Name;
            Log.Trace(sFormText + sText + " Clicked", ti.Frm);

            SM.FrmLogOn.Show();
        }

        private void tmTemp_Tick(object sender, EventArgs e)
        {
            tmTemp.Enabled = false;

            SaveCsv("d:\\Heater\\"+OM.Info.sTempFileName+".csv",SEQ.Heater.GetCrntTemp(0));

            tmTemp.Enabled = OM.Info.bTempSave ;
        }


        public void SaveCsv(string _sPath,int iTemp)
        {
            string sPath = _sPath;// @"D:\Data\" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + ".csv";
            string sDir  = Path.GetDirectoryName(sPath + "\\");
            string sName = Path.GetFileName(sPath);
            if (!CIniFile.MakeFilePathFolder(sDir)) return;

            string line = "";
            if (!File.Exists(sPath))
            {
                line = "Time,Temp";
                line += "\r\n";
            }

            try
            {
                FileStream   fs = new FileStream(sPath, FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                
                line += DateTime.Now.ToString("yyyyMMdd_HHmmss") + ",";
                line += iTemp.ToString()                         + ",";
                line += "\r\n";

                //sw.WriteLine(line);
                sw.Write(line);
                sw.Close();
                fs.Close();
            }
            catch (Exception _e)
            {
                Log.Trace(_e.Message);
            }
        }
    }
}

