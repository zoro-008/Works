using SML;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using COMMON;
using System.Management;

namespace Machine
{
    public partial class FormMain : Form
    {
        public FormOperation      FrmOperation      ;
        public FormDevice         FrmDevice         ;
        public FormDeviceSet      FrmDeviceSet      ;
        public FormSPC            FrmSPC            ;
        public FormOption         FrmOption         ;
        public FormPassword       FrmPassword       ;
        public FormMaster         FrmMaster         ;
        public FormVersion        FrmVersion        ;

        public int m_iActivePage = 0;

        private const string sFormText = "Form Main ";

        public FormMain()
        {
            //this.TopMost = true;
            InitializeComponent();

            SEQ.Init(pnBase.Width,pnBase.Height);
            OM.LoadLastInfo();

            //UsbNotification.RegisterUsbDeviceNotification(this.Handle);
            m_iActivePage = 0;

            FrmOperation      = new FormOperation     (pnBase);
            FrmDevice         = new FormDevice        (this,pnBase  );
            FrmDeviceSet      = new FormDeviceSet     (pnBase);
            FrmSPC            = new FormSPC           (pnBase);
            FrmOption         = new FormOption        (pnBase);
            FrmMaster         = new FormMaster        (      );
            FrmVersion        = new FormVersion       (      );
            FrmDevice.TopLevel = false;
            FrmDevice.Parent = pnBase;

            //lbDevice.Text = OM.GetCrntDev().ToString();

            //lbLotNo.Text  = LOT.GetLotNo();
            lbName.Text = Eqp.sEqpName;

            //파일 버전 보여주는 부분
            string sFileVersion = System.Windows.Forms.Application.ProductVersion;
            lbVer.Text = "Version " + sFileVersion;

            ShowPage(0);//Operation.

            this.Size = new Size(1280 , 1024) ;

            //MainSol
            //ML.IO.SetY((int)yi.ETC_MainAirSol, true);
            
            tmUpdate.Enabled = true;

        }
        ~FormMain()
        {
            //MessageBox.Show("123", "123");
        }

        public void ShowPage(int _iPageIdx)
        {
            FrmDeviceSet.UpdateDevInfo(true);
            PM.UpdatePstn(true);
            //PM.Load(OM.GetCrntDev());

            switch (_iPageIdx)
            {
                case 0: FrmOperation.Show(); break;
                case 1: FrmDevice   .Show(); break;
                case 2: /*FrmMacro    .Show();*/          break;
                case 3: FrmOption   .Show(); break;
                case 4: FrmSPC      .Show(); break;
                //FrmSPC      .ShowUpdate();    break;
                case 5: SM.SetDllMainWin(pnBase); break;
                default: FrmOperation.Show(); break;
            }

        }
        private void HidePage(int _iPageIdx)
        {
            switch (_iPageIdx)
            {
                case 0: FrmOperation.Hide(); break;
                case 1: FrmDevice   .Hide();
                        FrmDeviceSet.Hide(); break;
                case 2: /*FrmMacro    .Hide();*/          break;
                case 3: FrmOption   .Hide(); break;
                case 4: FrmSPC      .Hide(); break;
                case 5: SM.HideDllMainWin(); break;
                default: FrmOperation.Hide(); break;
            }
        }

        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            tmUpdate.Enabled = false;

            lbMainThreadTime.Text = string.Format("{0:0.000ms}", SEQ._dMainThreadCycleTime);
            //lbDevice.Text = "DEVICE : " + OM.GetCrntDev().ToString();
            //lbDevice.ForeColor = Color.Yellow;
            //lbLotNo.Text = "LotNo : " + LOT.GetLotNo();
            //lbLotNo.ForeColor = Color.White;

            //btExit.Enabled = !SEQ._bRun ;

            //Set Sequence State.
            string sStatText = "";
            Color StatColor = Color.Black;

            switch (SEQ._iSeqStat) {
                default: break;
                case EN_SEQ_STAT.Init   : sStatText = "INIT"    ; StatColor = Color.Blue; break;
                case EN_SEQ_STAT.Warning: sStatText = "WARNING" ; StatColor = Color.Gold; break;
                case EN_SEQ_STAT.Error  : sStatText = "ERROR"   ; StatColor = SEQ._bFlick ? Color.Gold : Color.Red; break;
                case EN_SEQ_STAT.Running: sStatText = "RUNNING" ; StatColor = Color.Lime; break;
                case EN_SEQ_STAT.Stop   : sStatText = "STOP"    ; StatColor = Color.Black; break;
                case EN_SEQ_STAT.RunWarn: sStatText = "RUNWARN" ; StatColor = SEQ._bFlick ? Color.Gold : Color.Lime; break;
                case EN_SEQ_STAT.WorkEnd: sStatText = "LOTEND"  ; StatColor = Color.Gray; break;
                case EN_SEQ_STAT.Manual : sStatText = "MANUAL"  ; StatColor = Color.Blue; break;
                case EN_SEQ_STAT.ToStart: sStatText = "STARTING"; StatColor = SEQ._bFlick ? Color.Gold : Color.Lime; break;
                case EN_SEQ_STAT.ToStop : sStatText = "STOPING" ; StatColor = SEQ._bFlick ? Color.Gold : Color.Lime; break;
            }
            //if (MM.GetManSetting()) sStatText = "MANUAL"; StatColor = Color.Lime; 

            if (OM.MstOptn.bDebugMode) {
                sStatText = "DEBUG " + sStatText;
            }

            lbStat.Text = sStatText;
            lbStat.ForeColor = StatColor;

            lbDate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            //pnIdleRunning.Visible  = SEQ._bFlick && OM.MstOptn.bIdleRun    ;
            //접근 레벨 Operator에서 Option/Util 버튼 비활성화(패스워드 방식)
            if (SM.FrmLogOn.GetLevel() == (int)EN_LEVEL.LogOff)
            {
                btDevice.Enabled = false;
                btSpc   .Enabled = false;
                btUtil  .Enabled = false;
                
                btOption.Enabled = false;
                //btExit.Enabled = false;
            }

            else
            {
                btDevice.Enabled = !SEQ._bRun;
                btSpc   .Enabled = !SEQ._bRun;
                //btUtil  .Enabled = !SEQ._bRun;
                btUtil.Enabled = true;
                btOption.Enabled = !SEQ._bRun;
                btExit  .Enabled = !SEQ._bRun;
            }

            //if (SM.FrmLogOn.GetLevel()<= EN_LEVEL.Operator)
            //{
            //    btSpc.Enabled    = false;
            //    btUtil.Enabled   = false;
            //    btOption.Enabled = false;
            //}

            //else
            //{
            //    btSpc.Enabled    = true;
            //    btUtil.Enabled   = true;
            //    btOption.Enabled = true;
            //}

            //btDevice.Enabled = SM.FrmLogOn.GetLevel() != EN_LEVEL.LogOff ;

            //Panel Option 
            bool bSkip1 = false;
            bool bSkip2 = false;
            if(OM.DevInfo.bVs1_Skip   || OM.DevInfo.bVs2_Skip || OM.DevInfo.bVs3_Skip ||
               OM.DevInfo.bVsL_NotUse || OM.DevInfo.bVsR_NotUse ) bSkip1 = true;

            if(/*OM.CmnOptn.bNotMark0 || OM.CmnOptn.bNotMark1 ||*/ OM.CmnOptn.bNotMark2 || OM.CmnOptn.bNotMark3 || OM.CmnOptn.bNotMark4 ||
               OM.CmnOptn.bNotMark5 || OM.CmnOptn.bNotMark6 || OM.CmnOptn.bNotMark7 || OM.CmnOptn.bNotMark8 || OM.CmnOptn.bNotMark9 ||
               OM.CmnOptn.bNotMarkA || OM.CmnOptn.bNotMarkB || OM.CmnOptn.bNotMarkC || OM.CmnOptn.bNotMarkD || OM.CmnOptn.bNotMarkE ||
               OM.CmnOptn.bNotMarkF || OM.CmnOptn.bNotMarkG || OM.CmnOptn.bNotMarkH || OM.CmnOptn.bNotMarkI || OM.CmnOptn.bNotMarkJ ||
               OM.CmnOptn.bNotMarkK || OM.CmnOptn.bNotMarkL) bSkip2 = true;
               
            pnOptn1.Visible = SEQ._bFlick && OM.CmnOptn.bLoadStop;
            pnOptn2.Visible = SEQ._bFlick && bSkip1              ;
            pnOptn3.Visible = SEQ._bFlick && bSkip2              ;

            //장비명 표기
            lbName.Text = OM.EqpOptn.sEquipName ;

            tmUpdate.Enabled = true;

        }

        private void btExit_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ForContext.Frm);

            //OM.EqpStat.dLastIDXRPos = ML.MT_GetEncPos(mi.IDXR_XRear);
            //OM.EqpStat.dLastIDXFPos = ML.MT_GetEncPos(mi.IDXF_XFrnt);

            if (Log.ShowMessageModal("Confirm", "Do you want to exit the program ?") != DialogResult.Yes) return;
            Log.Trace("Program End", ForContext.Sts);

            SEQ.Close();
            FrmOperation .Close();
            FrmDevice    .Close();
            FrmDeviceSet .Close();
            FrmMaster    .Close();
            FrmOption    .Close();

            Close();
        }

        private void btOperation_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ForContext.Frm);

            HidePage(m_iActivePage);
            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);
            m_iActivePage = iBtnTag;
            ShowPage(m_iActivePage);
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmUpdate.Enabled = false;
        }

        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }

        private void lbName_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string sText = ((Label)sender).Text;
            Log.Trace(sFormText + sText + " Label Double Clicked", ForContext.Frm);

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

        private void lbVer_DoubleClick(object sender, EventArgs e)
        {
            if (FrmVersion.IsDisposed)
            {
                FrmVersion = new FormVersion();
            }
            FrmVersion.Owner = this;
            FrmVersion.Show();
        }
    }
}

