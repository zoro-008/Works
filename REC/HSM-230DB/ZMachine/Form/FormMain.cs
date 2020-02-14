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
        public FormSubErr    FrmSubErr    ;
         
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
            FrmSubErr    = new FormSubErr();
            FrmDevice.TopLevel = false;
            FrmDevice.Parent = pnBase;
            
            lbDevice.Text = OM.GetCrntDev().ToString();

            lbLotNo.Text  = LOT.GetLotNo();
            lbName.Text = Eqp.sEqpName;


            ShowPage(0);//Operation.

            //MainSol
            //SM.IO.SetY((int)yi.ETC_MainAirSol, true);

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
                case 1: FrmVision   .Show();          break;
                case 2: FrmDevice   .Show();          break;
                case 3: FrmOption   .Show();          
                        FrmOption.bUpdate = true;     break;
                case 4: FrmSPC      .Show();          
                        FrmSPC      .ShowUpdate();    break;
                case 5: SML.SetDllMainWin(ref pnBase); break;
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
                case 5: SML.HideDllMainWin();        break;
                default: FrmOperation.Hide();         break;
            }
        }

        bool bMsgEnd = false;
        bool bRecvEnd = false;
        int iExitCnt = 0;
        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            tmUpdate.Enabled = false ;

            //lbMainThreadTime.Text = string.Format("{0:0.000ms}", SEQ._dMainThreadCycleTime);
            lbDevice.Text = OM.GetCrntDev().ToString();
            //lbDevice.ForeColor = Color.Yellow;
            label3.Text = "LotNo";
            lbLotNo.Text = LOT.GetLotNo();
            //lbLotNo.ForeColor = Color.White;
            
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

            
             //ER_SetErr(ei.VSN_InspNG , "End Dispense 검사 NG");
            if(SM.ER_GetErrOn(ei.VSN_InspNG)){
                if(SM.ER_GetErrSubMsg(ei.VSN_InspNG) == "End Dispense 검사 NG"){
                    if(!FrmSubErr.Visible){
                        FrmSubErr.Show();
                    }
                }
            }
             
            lbHght.Text = OM.EqpStat.dSubstrateHeight.ToString();
             

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
                        //VC.SetReceivedMsg((VC.COPYDATASTRUCT)m.GetLParam(typeof(VC.COPYDATASTRUCT)));
                        return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btExit_Click_1(object sender, EventArgs e)
        {

        }

        private void lbName_DoubleClick(object sender, EventArgs e)
        {

            
        }
        
        //private void lbName_Click_1(object sender, EventArgs e)
        //{

        //}

        //private void lbName_Click(object sender, EventArgs e)
        //{

        //}

        private void lbName_DoubleClick_1(object sender, EventArgs e)
        {
            if(FormPassword.m_iCrntLevel != EN_LEVEL.Master) return ;

            if (FrmMaster.IsDisposed)
            {
                FrmMaster = new FormMaster();
            }
            FrmMaster.Show();
            //if (!bOpen)
            //{
            //    Temp.PortOpen();
            //    bOpen = true;
            //}
            //Temp.GetCrntTemp(1);     //GetCrnSetHeatTimer(1,60);
            //Temp.

            //byte [] babo = {0x02, 0x04, 0x02, 0x00, 0x00, 0x00, 0x00};
            //ushort usRet = Temp.CRC16(babo , 5);
            //string sTemp = usRet.ToString("X4");
            //int a =0; 
            //a++;
        }

        private void lbName_Click(object sender,EventArgs e) {
            
        } 
    }
}
