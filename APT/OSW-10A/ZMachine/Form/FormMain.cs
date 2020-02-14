using COMMON;
using SML2;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace Machine
{
    
    public partial class FormMain : Form
    {


        public FormOperation FrmOperation ;
        public FormDevice    FrmDevice    ;
        public FormDeviceSet FrmDeviceSet ;
        public FormSPC       FrmSPC       ;
        public FormOption    FrmOption    ;
        public FormPassword  FrmPassword  ; 
        public FormMaster    FrmMaster    ;
        public FormVccOption FrmVccOption ;
        public FormVer       FrmVer       ;
         
        public int m_iActivePage = 0;
        public FormMain()
        {
            
            //this.TopMost = true;
            SEQ.Init();
            OM.LoadLastInfo();

            InitializeComponent();

            m_iActivePage = 0;
            

            FrmOperation = new FormOperation(pnBase);
            FrmDevice    = new FormDevice   (this  );
            FrmDeviceSet = new FormDeviceSet(pnBase);
            FrmSPC       = new FormSPC      (pnBase);
            FrmOption    = new FormOption   (pnBase);

            FrmMaster    = new FormMaster();
            FrmVccOption = new FormVccOption();
            
            FrmVer       = new FormVer();
            FrmVer.TopLevel = true ;

            FrmDevice.TopLevel = false;
            FrmDevice.Parent = pnBase;
            
            lbDevice.Text = OM.GetCrntDev().ToString();

            lbLotNo.Text  = LOT.GetLotNo();
            lbName.Text = Eqp.sEqpName;

            //파일 버전 보여주는 부분
            string sFileVersion = System.Windows.Forms.Application.ProductVersion;
            lbVer.Text = "Ver " + sFileVersion;

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
            PM.Load(OM.GetCrntDev());

            switch (_iPageIdx)
            {
                case 0: FrmOperation.Show();          break;
                case 1:                               break;
                case 2: FrmDevice   .Show();          break;
                case 3: FrmOption   .Show();          break;
                case 4: FrmSPC      .FormInit();
                        FrmSPC      .Show();          break;        
                case 5: SML.SetDllMainWin(ref pnBase); break;
                default: FrmOperation.Show();         break;
            }
          
        }
        private void HidePage(int _iPageIdx)
        {
            switch (_iPageIdx)
            {
                case 0: FrmOperation.Hide();          break;
                case 1:                               break;
                case 2: FrmDevice   .Hide();          
                        FrmDeviceSet.Hide();          break;
                case 3: FrmOption   .Hide();          break;
                case 4: FrmSPC      .Hide();          break;
                case 5: SML.HideDllMainWin();        break;
                default: FrmOperation.Hide();         break;
            }
        }
        
        public void CopyFolder(string _sSourceFolder , string _sDestFolder)
        {
            //DirectoryInfo DestDirecy = new DirectoryInfo(_sDestFolder);
            if (!Directory.Exists(_sDestFolder))
            {
                Directory.CreateDirectory(_sDestFolder);
            }

            string[] Files   = Directory.GetFiles(_sSourceFolder);
            string[] Folders = Directory.GetDirectories(_sSourceFolder);
            try { 
                foreach (string file in Files)
                {
                    string sName = Path.GetFileName(file);
                    string sDest = Path.Combine(_sDestFolder , sName);
                    File.Copy(file,sDest);
                }
                
                foreach (string folder in Folders)
                {
                    string sName = Path.GetFileName(folder);
                    string sDest = Path.Combine(_sDestFolder , sName);
                    CopyFolder(folder,sDest);
                }
            }
            catch(Exception e)
            {

            }
        }



        DateTime CrntTime = new DateTime(DateTime.Now.Ticks);
        DateTime PreTime  = new DateTime(DateTime.Now.Ticks);
        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            tmUpdate.Enabled = false ;

            CrntTime = DateTime.Now ;
            if (CrntTime.Date != PreTime.Date)//하루지났다 백업해라.
            {
                //옵션끄려면 공백.
                if (OM.CmnOptn.sBackupFolder != "") { 
                    string sExeFolder   = System.AppDomain.CurrentDomain.BaseDirectory;
                    string sUtilPath    = sExeFolder + "Util";
                    string sJobFilePath = sExeFolder + "JobFile";
                    string sToday       = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    CopyFolder(sUtilPath    , OM.CmnOptn.sBackupFolder + "\\" + sToday + "\\Util");
                    CopyFolder(sJobFilePath , OM.CmnOptn.sBackupFolder + "\\" + sToday + "\\JobFile");
                    
                    //기존에 있던것들 지우기.
                    DirectoryInfo di = new DirectoryInfo(OM.CmnOptn.sBackupFolder);
                    if (!di.Exists) di.Create();
                    foreach (FileInfo fi in di.GetFiles())
                    {
                        //if (fi.Extension != ".log") continue;
                        // 1개월 이전 네디터를 삭제합니다.
                        if (fi.CreationTime <= DateTime.Now.AddDays(-1))
                        {
                            fi.Delete();
                        }
                    }
                }                
            }
            PreTime = CrntTime ;

            lbMainThreadTime.Text = string.Format("{0:0.000ms}", SEQ._dMainThreadCycleTime);
            lbDevice.Text = OM.GetCrntDev().ToString();
            //lbDevice.ForeColor = Color.Yellow;
            label3.Text = "LotNo";
            lbLotNo.Text = LOT.GetLotNo();
            //lbLotNo.ForeColor = Color.White;
            
            btExit.Enabled = !SEQ._bRun ;
            
            //Set Sequence State.
            string sStatText = "";
            Color  StatColor = Color.Black ;




            //if(!OM.MstOptn.bDebugMode){
            
            //    switch(SEQ._iSeqStat) {
            //        default        :
            //        break ;
            //        case EN_SEQ_STAT.Init    : sStatText = "INIT"      ; StatColor = Color.Blue   ;
            //        break ;
            //        case EN_SEQ_STAT.Error   : sStatText = "ERROR"     ; StatColor = SEQ._bFlick ? Color.Yellow : Color.Red ;
            //        break ;
            //        case EN_SEQ_STAT.Running : sStatText = "RUNNING"   ; StatColor = Color.Lime   ;
            //        break ;
            //        case EN_SEQ_STAT.Stop    : sStatText = "STOP"      ; StatColor = Color.Black  ;
            //        break ;
            //        case EN_SEQ_STAT.WorkEnd : sStatText = "LOTEND"    ; StatColor = Color.Gray   ;
            //        break ;
            //        case EN_SEQ_STAT.RunWarn : sStatText = "Run End"   ; StatColor = Color.Gray   ;
            //        break;
            //    }
            //}
            //else {
            //    switch(SEQ._iSeqStat) {
            //        default                  :                                                                                         break;
            //        case EN_SEQ_STAT.Init    : sStatText = "DEBUG INIT"    ; StatColor = Color.Blue                                  ; break;
            //        case EN_SEQ_STAT.Error   : sStatText = "DEBUG ERROR"   ; StatColor = SEQ._bFlick ? Color.Yellow : Color.Red      ; break;
            //        case EN_SEQ_STAT.Running : sStatText = "DEBUG RUNNING" ; StatColor = Color.Lime                                  ; break;
            //        case EN_SEQ_STAT.Stop    : sStatText = "DEBUG STOP"    ; StatColor = Color.Black                                 ; break;
            //        case EN_SEQ_STAT.WorkEnd : sStatText = "DEBUG LOTEND"  ; StatColor = Color.Gray                                  ; break;
            //    }
            //}

            switch(SEQ._iSeqStat) {
                default                  :                                                                                      break;
                case EN_SEQ_STAT.Init    : sStatText = "INIT"       ; StatColor = Color.Blue                                  ; break;
                case EN_SEQ_STAT.Warning : sStatText = "WARNING"    ; StatColor = Color.Yellow                                ; break;
                case EN_SEQ_STAT.Error   : sStatText = "ERROR"      ; StatColor = SEQ._bFlick ? Color.Yellow : Color.Red      ; break;
                case EN_SEQ_STAT.Running : sStatText = "RUNNING"    ; StatColor = Color.Lime                                  ; break;
                case EN_SEQ_STAT.Stop    : sStatText = "STOP"       ; StatColor = Color.Black                                 ; break;
                case EN_SEQ_STAT.RunWarn : sStatText = "RUNWARN"    ; StatColor = SEQ._bFlick ? Color.Yellow : Color.Lime     ; break;
                case EN_SEQ_STAT.WorkEnd : sStatText = "LOTEND"     ; StatColor = Color.Gray                                  ; break;
                case EN_SEQ_STAT.Manual  : sStatText = "MANUAL"     ; StatColor = Color.Blue                                  ; break;
            }
            //if (MM.GetManSetting()) sStatText = "MANUAL"; StatColor = Color.Lime; 

            if(OM.MstOptn.bDebugMode){
                sStatText = "DEBUG " + sStatText ;
            }

            

            lbStat.Text = sStatText ;
            lbStat.ForeColor = StatColor ;

            pnUnderRepair.Visible  = SEQ._bFlick && OM.EqpStat.bMaint      ;
            pnIdleRunning.Visible  = SEQ._bFlick && OM.CmnOptn.bIdleRun    ;
            pnGoldenDevice.Visible = SEQ._bFlick && OM.CmnOptn.bGoldenTray ;
            pnDBNotConnect.Visible = SEQ._bFlick && !SEQ.Oracle.GetDBOpen() ;    
            //접근 레벨 Operator에서 Option/Util 버튼 비활성화(패스워드 방식)
            //if (FormPassword.GetLevel() == EN_LEVEL.Operator)
            //{
            //    btSpc.Enabled    = false;
            //    btUtil.Enabled   = false;
            //    btOption.Enabled = false;
            //}
            //
            //else
            //{
            //    btSpc.Enabled    = true;
            //    btUtil.Enabled   = true;
            //    btOption.Enabled = true;
            //}

            //btSpc   .Enabled = SML.FrmLogOn.GetLevel()>= EN_LEVEL.Engineer ;
            btUtil  .Enabled = SML.FrmLogOn.GetLevel()>= EN_LEVEL.Master   ;
            btOption.Enabled = SML.FrmLogOn.GetLevel()>= EN_LEVEL.Engineer ;
            btDevice.Enabled = SML.FrmLogOn.GetLevel()!= EN_LEVEL.LogOff   ;


            




            tmUpdate.Enabled = true  ;
            
        }

        

        private void btExit_Click(object sender, EventArgs e)
        {
            if (COracle.bMakingDMC1List)
            {
                Log.ShowMessage("OracleDB" , "Please Wait for Finish Making DMC1 List!");
                return ;
            }
            if (COracle.bMakingPanelList)
            {
                Log.ShowMessage("OracleDB" , "Please Wait for Finish Making PanelID List!");
                return ;
            }


            
            //if (Log.ShowMessageModal("Confirm", "프로그램을 종료하시겠습니까?") != DialogResult.Yes) return;
            if (Log.ShowMessageModal("Confirm", "Are you sure you want to exit the program?") != DialogResult.Yes) return;
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

        private void lbName_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {
            string sExeFolder   = System.AppDomain.CurrentDomain.BaseDirectory;
            string sUtilPath    = sExeFolder + "Util";
            string sJobFilePath = sExeFolder + "JobFile";
            string sToday       = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            CopyFolder(sUtilPath    , OM.CmnOptn.sBackupFolder + "\\" + sToday + "\\Util");
            CopyFolder(sJobFilePath , OM.CmnOptn.sBackupFolder + "\\" + sToday + "\\JobFile");

            //기존에 있던것들 지우기.
            DirectoryInfo di = new DirectoryInfo(OM.CmnOptn.sBackupFolder);
            if (!di.Exists) di.Create();
            foreach (FileInfo fi in di.GetFiles())
            {
                //if (fi.Extension != ".log") continue;
                // 12개월 이전 로그를 삭제합니다.
                if (fi.CreationTime <= DateTime.Now.AddDays(-1))
                {
                    fi.Delete();
                }
            }
        }

        private void lbVer_DoubleClick(object sender, EventArgs e)
        {
            if (FrmVer.IsDisposed)
            {
                FrmVer = new FormVer();
            }
            FrmVer.Show();
        } 
        
    }
}
