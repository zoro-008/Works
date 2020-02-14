using SMDll2;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;



namespace Machine
{
    
    public partial class FormMain : Form
    {
        public FormOperation FrmOperation ;
        public FormVision    FrmVision    ;
        public FormDevice    FrmDevice    ;
        public FormDeviceSet FrmDeviceSet ;
        public FormSPC       FrmSPC       ;
        public FormOption    FrmOption    ;
        public FormPassword  FrmPassword  ; 
        public FormMaster    FrmMaster    ;
         
        public int m_iActivePage = 0;
        public FormMain()
        {
            //this.TopMost = true;
            SEQ.Init();
            OM.LoadLastInfo();

            InitializeComponent();

            m_iActivePage = 0;
            

            FrmOperation = new FormOperation(pnBase);
            FrmVision    = new FormVision   (pnBase);
            FrmDevice    = new FormDevice   (this  );
            FrmDeviceSet = new FormDeviceSet(pnBase);
            FrmSPC       = new FormSPC      (pnBase);
            FrmOption    = new FormOption   (pnBase);

            FrmMaster    = new FormMaster();

            FrmDevice.TopLevel = false;
            FrmDevice.Parent = pnBase;
   
            lbDevice.Text = OM.GetCrntDev().ToString();

            lbLotNo.Text  = LOT.GetLotNo();
            lbName.Text   = OM.EqpOptn.sModelName;

            //USB 현미경 프로그램 키는거            
            EmbededExe.CameraInit();
            EmbededExe.SetCamParent(FrmVision.Handle);

            ShowPage(0);//Operation.

            //MainSol
            SM.IO.SetY((int)yi.ETC_MainAirSol, true);
            //SM.IO.SetY((int)yi.PCK_VacSol    , true);

            tmUpdate.Enabled = true;

            

        }
         ~FormMain()
        {
            MessageBox.Show("123", "123");
        }

        public void ShowPage(int _iPageIdx)
        {
            FrmDeviceSet.UpdateDevInfo(true);
            FrmDeviceSet.UpdateDevOptn(true);
            PM.UpdatePstn(true);

            switch (_iPageIdx)
            {
                case 0: FrmOperation.Show();          break;
                case 1: FrmVision.Show();
                        EmbededExe.SetCamParent(FrmVision.Handle);
                        break;
                case 2: FrmDevice   .Show();          break;
                case 3: FrmOption   .Show();          
                        FrmOption.bUpdate = true;     break;
                case 4: FrmSPC      .Show();          
                        FrmSPC      .ShowUpdate();    break;
                case 5: SM.SetDllMainWin(ref pnBase); break;
                default: FrmOperation.Show();         break;
            }
          
        }
        private void HidePage(int _iPageIdx)
        {
            switch (_iPageIdx)
            {
                case 0: FrmOperation.Hide();          break;
                case 1: FrmVision   .Hide();          break;
                case 2: FrmDevice   .Hide();          
                        FrmDeviceSet.Hide();          break;
                case 3: FrmOption   .Hide();          break;
                case 4: FrmSPC      .Hide();          break;
                case 5: SM.HideDllMainWin();          break;
                default: FrmOperation.Hide();         break;
            }
        }

        bool bMsgEnd = false;
        bool bRecvEnd = false;
        int iExitCnt = 0;
        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            tmUpdate.Enabled = false ;

            lbMainThreadTime.Text = string.Format("{0:0.000ms}", SEQ._dMainThreadCycleTime);
            lbDevice.Text = OM.GetCrntDev().ToString();
            lbLotNo.Text = LOT.GetLotNo();
                 
            
            btExit.Enabled = !SEQ._bRun ;
            
            //Set Sequence State.
            if(!OM.MstOptn.bDebugMode){
            
                switch(SEQ._iSeqStat) {
                    default        :
                    break ;
                    case EN_SEQ_STAT.ssInit    : lbStat.Text = "INIT"      ; lbStat.ForeColor = Color.Blue   ;
                    break ;
                    case EN_SEQ_STAT.ssError   : lbStat.Text = "ERROR"     ; lbStat.ForeColor = SEQ._bFlick ? Color.Yellow : Color.Red ;
                    break ;
                    case EN_SEQ_STAT.ssRunning : lbStat.Text = "RUNNING"   ; lbStat.ForeColor = Color.Lime   ;
                    break ;
                    case EN_SEQ_STAT.ssStop    : lbStat.Text = "STOP"      ; lbStat.ForeColor = Color.Black  ;
                    break ;
                    case EN_SEQ_STAT.ssWorkEnd : lbStat.Text = "LOTEND"    ; lbStat.ForeColor = Color.Gray   ;
                    break ;
                }
            }
            else {
                switch(SEQ._iSeqStat) {
                    default                    :                                                                                                  break;
                    case EN_SEQ_STAT.ssInit    : lbStat.Text = "DEBUG INIT"    ; lbStat.ForeColor = Color.Blue                                  ; break;
                    case EN_SEQ_STAT.ssError   : lbStat.Text = "DEBUG ERROR"   ; lbStat.ForeColor = SEQ._bFlick ? Color.Yellow : Color.Red      ; break;
                    case EN_SEQ_STAT.ssRunning : lbStat.Text = "DEBUG RUNNING" ; lbStat.ForeColor = Color.Lime                                  ; break;
                    case EN_SEQ_STAT.ssStop    : lbStat.Text = "DEBUG STOP"    ; lbStat.ForeColor = Color.Black                                 ; break;
                    case EN_SEQ_STAT.ssWorkEnd : lbStat.Text = "DEBUG LOTEND"  ; lbStat.ForeColor = Color.Gray                                  ; break;
                }
            }
            
            //접근 레벨 Operator에서 Option/Util 버튼 비활성화
            if (FormPassword.GetLevel() == EN_LEVEL.lvOperator)
            {
                btSpc.Enabled    = false;
                btUtil.Enabled   = false;
                btOption.Enabled = false;
            }

            else
            {
                btSpc.Enabled    = true;
                btUtil.Enabled   = true;
                btOption.Enabled = true;
            }

            //Vision Program End.
            if (bMsgEnd && VC.GetVisnSendMsg() == "OK") bRecvEnd = true;
            if (iExitCnt >= 2) bRecvEnd = true;

            tmUpdate.Enabled = true  ;
            
        }

        

        private void btExit_Click(object sender, EventArgs e)
        {
            if (EmbededExe.GetHPSHandle() > 0)
            {
                VC.SendVisnMsg(VC.sm.End);
                bMsgEnd = true;
                iExitCnt++;
                if (!bRecvEnd) return;
            }
            
            SEQ.Close();
            FrmOperation.Close();
            FrmDevice.Close();
            FrmDeviceSet.Close();
            FrmMaster.Close();
            FrmOption.Close();
            
            Close();
        }

        private void btOperation_Click(object sender, EventArgs e)
        {
            HidePage(m_iActivePage);
            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);
            m_iActivePage = iBtnTag;
            ShowPage(m_iActivePage);
        }




        private void lbName_Click(object sender, EventArgs e)
        {
            if (FrmMaster.IsDisposed)
            {
                FrmMaster = new FormMaster();
            }
            FrmMaster.Show();
            
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmUpdate.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }

        public const int WM_COPYDATA = 0x4A;


        ///SendMessage Receive
        protected override void WndProc(ref Message m)
        {
            const int WM_COPYDATA = 0x4A;
            try
            {
                switch (m.Msg)
                {
                    default:
                        base.WndProc(ref m);
                        break;

                    case WM_COPYDATA:
                        VC.SetReceivedMsg((VC.COPYDATASTRUCT)m.GetLParam(typeof(VC.COPYDATASTRUCT)));
                        return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        } 
    }
}
