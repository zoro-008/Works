using SML2;
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
            //VC.Init();

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
            lbName.Text   = Eqp.sEqpName;//OM.EqpOptn.sModelName;

            ShowPage(0);//Operation.

            tmUpdate.Enabled = true;

            //SM.IO.SetY((int)yi.ETC_MainAirSol, true);

            //if (SM.CL.GetCmd((int)ai.IDX_Hold1UpDn) == EN_CYLINDER_POS.cpFwd)
            //{
            //    SM.CL.Move((int)ai.IDX_Hold1UpDn, EN_CYLINDER_POS.cpFwd);
            //}
            //if (SM.CL.GetCmd((int)ai.IDX_Hold2UpDn) != 0)
            //{
            //    SM.CL.Move((int)ai.IDX_Hold2UpDn, EN_CYLINDER_POS.cpFwd);
            //}

                

            

        }

        public void ShowPage(int _iPageIdx)
        {
            FrmDeviceSet.UpdateDevInfo(true);
            FrmDeviceSet.UpdateDevOptn(true);
            PM.UpdatePstn(true);

            switch (_iPageIdx)
            {
                case 0 : FrmOperation.Show();          break;        
                case 1 : FrmVision   .Show();          break;
                case 2 : FrmDevice   .Show();          break;
                case 3 : FrmOption   .Show();          break;
                case 4 : FrmSPC      .Show();
                         FrmSPC      .ShowUpdate();    break;
                case 5 : SML.SetDllMainWin(ref pnBase);break;
                default: FrmOperation.Show();          break;
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
                case 5: SML.HideDllMainWin();          break;
                default: FrmOperation.Hide();         break;
            }
        }

        bool bMsgEnd = false;
        bool bRecvEnd = false;
        
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
                    case EN_SEQ_STAT.Init    : lbStat.Text = "INIT"      ; lbStat.ForeColor = Color.Blue   ;
                    break ;
                    case EN_SEQ_STAT.Error   : lbStat.Text = "ERROR"     ; lbStat.ForeColor = SEQ._bFlick ? Color.Yellow : Color.Red ;
                    break ;
                    case EN_SEQ_STAT.Running : lbStat.Text = "RUNNING"   ; lbStat.ForeColor = Color.Lime   ;
                    break ;
                    case EN_SEQ_STAT.Stop    : lbStat.Text = "STOP"      ; lbStat.ForeColor = Color.Black  ;
                    break ;
                    case EN_SEQ_STAT.WorkEnd : lbStat.Text = "LOTEND"    ; lbStat.ForeColor = Color.Gray   ;
                    break ;
                    case EN_SEQ_STAT.RunWarn : lbStat.Text = "Run End"   ; lbStat.ForeColor = Color.Gray   ;
                    break;
                }
            }
            else {
                switch(SEQ._iSeqStat) {
                    default                    :                                                                                                  break;
                    case EN_SEQ_STAT.Init    : lbStat.Text = "DEBUG INIT"    ; lbStat.ForeColor = Color.Blue                                  ; break;
                    case EN_SEQ_STAT.Error   : lbStat.Text = "DEBUG ERROR"   ; lbStat.ForeColor = SEQ._bFlick ? Color.Yellow : Color.Red      ; break;
                    case EN_SEQ_STAT.Running : lbStat.Text = "DEBUG RUNNING" ; lbStat.ForeColor = Color.Lime                                  ; break;
                    case EN_SEQ_STAT.Stop    : lbStat.Text = "DEBUG STOP"    ; lbStat.ForeColor = Color.Black                                 ; break;
                    case EN_SEQ_STAT.WorkEnd : lbStat.Text = "DEBUG LOTEND"  ; lbStat.ForeColor = Color.Gray                                  ; break;
                }
            }
            
            //접근 레벨 Operator에서 Option/Util 버튼 비활성화
            if (FormPassword.GetLevel() == EN_LEVEL.Operator)
            {
                //btSpc.Enabled    = false;
                btUtil.Enabled      = false;
                btOption.Enabled = false;
            }

            else
            {
                //btSpc.Enabled    = true;
                btUtil.Enabled      = true;
                btOption.Enabled = true;
            }

            tmUpdate.Enabled = true  ;
            
        }

        private void btExit_Click(object sender, EventArgs e)
        {
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
            if (FormPassword.GetLevel() == EN_LEVEL.Master)
            {
                if (FrmMaster.IsDisposed)
                {
                    FrmMaster = new FormMaster();
                }
                FrmMaster.Show();
            }
            
            
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmUpdate.Enabled = false;
        }

    }
}
