using SML;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using COMMON;
using System.Management;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using MetroFramework;
using System.Threading;

namespace Machine
{
    public partial class FormMain : Form
    {
        public FormOperation      FrmOperation      ;
        public FormManual         FrmManual         ;
        public FormDevice         FrmDevice         ;
        public FormDeviceSet      FrmDeviceSet      ;
        public FormSPC            FrmSPC            ;
        public FomOption         FrmOption         ;
        public FormPassword       FrmPassword       ;
        public FormMaster         FrmMaster         ;
        public FormVersion        FrmVersion        ;

        public int m_iActivePage = 0;

        private const string sFormText = "Form Main ";

        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern System.IntPtr CreateRoundRectRgn
        (
             int nLeftRect, // x-coordinate of upper-left corner
             int nTopRect, // y-coordinate of upper-left corner
             int nRightRect, // x-coordinate of lower-right corner
             int nBottomRect, // y-coordinate of lower-right corner
             int nWidthEllipse, // height of ellipse
             int nHeightEllipse // width of ellipse
        );

        


        public FormMain()
        {
            //this.TopMost = true;
            //SEQ.Init(pnBase.Width,pnBase.Height);
            //OM.LoadLastInfo();

            InitializeComponent();

            //Round
            //TopPanel2.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, TopPanel2.Width, TopPanel2.Height, 15, 15));


            SEQ.Init(pnBase.Width,pnBase.Height);
            OM.LoadLastInfo();
            //UsbNotification.RegisterUsbDeviceNotification(this.Handle);
            m_iActivePage = 0;



            //lbDevice.Text = OM.GetCrntDev().ToString();

            //lbLotNo.Text  = LOT.GetLotNo();
            //lbName.Text = Eqp.sEqpName;
            
            btOperation.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btOperation.Width, btOperation.Height, 5, 5));
            btManual   .Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btManual   .Width, btManual   .Height, 5, 5));
            btDevice   .Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btDevice   .Width, btDevice   .Height, 5, 5));
            btOption   .Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btOption   .Width, btOption   .Height, 5, 5));
            btReport   .Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btReport   .Width, btReport   .Height, 5, 5));
            btExit     .Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btExit     .Width, btExit     .Height, 5, 5));

            ActiveControl = btOperation;
            

        }
        ~FormMain()
        {
            //MessageBox.Show("123", "123");
            
        }

        MetroFramework.Components.MetroStyleManager smMain = new MetroFramework.Components.MetroStyleManager();
        private void FormMain_Load(object sender, EventArgs e)
        {
            //파일 버전 보여주는 부분
            string sFileVersion = System.Windows.Forms.Application.ProductVersion;
            lbVer.Text = "Version " + sFileVersion;

            

            //this.Size = new Size(1920 , 1024) ;

            //MainSol
            //ML.IO.SetY((int)yi.ETC_MainAirSol, true);
            
            tmUpdate.Enabled = true;

            FrmOperation      = new FormOperation     (pnBase);
            FrmManual         = new FormManual        (pnBase);
            FrmDevice         = new FormDevice        (this  ,pnBase);
            FrmDeviceSet      = new FormDeviceSet     (pnBase);
            FrmSPC            = new FormSPC           (pnBase);
            FrmOption         = new FomOption         (pnBase);
            FrmMaster         = new FormMaster        (      );
            FrmVersion        = new FormVersion       (      );
            //FrmDevice.TopLevel = false;
            //FrmDevice.Parent = pnBase;

            smMain.Style = MetroFramework.MetroColorStyle.Green ;
            smMain.Theme = MetroFramework.MetroThemeStyle.Dark  ;
            smMain.Owner = this ;

            pnMain.StyleManager = smMain ;
            smMain.Style = (MetroColorStyle)OM.EqpStat.iSkin ;


            ShowPage(0);//Operation.
            
        }

        
        //void ChangeColorStyle()
        //{
        //    foreach (Control c in Controls)
        //    {
        //       
        //        UpdateColorControls(c);
        //    }
        //}



        //public void UpdateColorControls(Control myControl)
        //{
        //    //metroStyleManager1.Style = (MetroFramework.MetroColorStyle)cbColor.SelectedIndex;
        //    if(myControl is MetroFramework.Interfaces.IMetroControl)
        //    {
        //        myControl.BackColor = Colors.Black;
        //        myControl.ForeColor = Colors.White;
        //    }
        //    
        //    
        //    foreach (Control subC in myControl.Controls) 
        //    {
        //        UpdateColorControls(subC);
        //    } 
        //}

        public void ShowPage(int _iPageIdx)
        {
            FrmDeviceSet.UpdateDevInfo(true);
            PM.UpdatePstn(true);
            //PM.Load(OM.GetCrntDev());

            Panel pnTemp = pnBase as Panel ;

            switch (_iPageIdx)
            {
                case 0: FrmOperation.Show(); break;
                case 1: FrmManual   .Show(); break;
                case 2: FrmDevice   .Show(); break;
                case 3: FrmOption   .Show(); break;
                case 4: FrmSPC      .Show(); break;
                case 5: SM.SetDllMainWin(pnTemp); break;
                default: FrmOperation.Show(); break;
            }

        }
        private void HidePage(int _iPageIdx)
        {
            switch (_iPageIdx)
            {
                case 0: FrmOperation.Hide(); break;
                case 1: FrmManual   .Hide(); break;
                case 2: FrmDevice   .Hide();
                        FrmDeviceSet.Hide(); break;
                
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
                case EN_SEQ_STAT.Stop   : sStatText = "STOP"    ; StatColor = Color.Gray; break;
                case EN_SEQ_STAT.RunWarn: sStatText = "RUNWARN" ; StatColor = SEQ._bFlick ? Color.Gold : Color.Lime; break;
                case EN_SEQ_STAT.WorkEnd: sStatText = "LOTEND"  ; StatColor = Color.DarkGray; break;
                case EN_SEQ_STAT.Manual : sStatText = "MANUAL"  ; StatColor = Color.Blue; break;
                case EN_SEQ_STAT.ToStart: sStatText = "STARTING"; StatColor = SEQ._bFlick ? Color.Gold : Color.Lime; break;
                case EN_SEQ_STAT.ToStop : sStatText = "STOPING" ; StatColor = SEQ._bFlick ? Color.Gold : Color.Lime; break;
            }
            //if (MM.GetManSetting()) sStatText = "MANUAL"; StatColor = Color.Lime; 

            if (OM.MstOptn.bDebugMode) {
                sStatText = "DEBUG " + sStatText;
            }
            if (OM.MstOptn.bIdleRun) {
                sStatText = "IDLE " + sStatText;
            }

            lbStat.Text = sStatText;
            lbStat.ForeColor = StatColor;

            lbDate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            //pnIdleRunning.Visible  = SEQ._bFlick && OM.MstOptn.bIdleRun    ;
            //접근 레벨 Operator에서 Option/Util 버튼 비활성화(패스워드 방식)
            ////if (SM.FrmLogOn.GetLevel() == (int)EN_LEVEL.LogOff)
            ////{
            ////    btDevice.Enabled = false;
            ////    btReport.Enabled = false;
            ////    btUtil  .Enabled = false;
            ////    
            ////    btOption.Enabled = false;
            ////    //btExit.Enabled = false;
            ////}
            ////
            ////else
            ////{
            ////    btDevice.Enabled = !SEQ._bRun;
            ////    btReport.Enabled = !SEQ._bRun;
            ////    //btUtil  .Enabled = !SEQ._bRun;
            ////    btUtil.Enabled = true;
            ////    btOption.Enabled = !SEQ._bRun;
            ////    btExit  .Enabled = !SEQ._bRun;
            ////}

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


            //장비명 표기
            lbName.Text = OM.EqpOptn.sEquipName ;
            //lbName.Text = Eqp.sEqpName ;


            int iStyle = (int)smMain.Style ;
            iStyle++;
            if(iStyle > 14)iStyle = 1 ;
            //smMain.Style = (MetroColorStyle)iStyle ;

            tmUpdate.Enabled = true;

        }

        private void btExit_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.TraceListView(sFormText + sText + " Button Clicked", ForContext.Frm);

            //OM.EqpStat.dLastIDXRPos = ML.MT_GetEncPos(mi.IDXR_XRear);
            //OM.EqpStat.dLastIDXFPos = ML.MT_GetEncPos(mi.IDXF_XFrnt);

            if (Log.ShowMessageModal("Confirm", "Do you want to exit the program ?") != DialogResult.Yes) return;

            //Part.IO_SetY(yi.ETC_MainPumpOn, false , true);
            Thread.Sleep(200);

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
            Log.TraceListView(sFormText + sText + " Button Clicked", ForContext.Frm);

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

        private void FormMain_Shown(object sender, EventArgs e)
        {
            try 
            {
                //프로그램 Lock 설정된 PC에서만 동작하게 함.
                //실행파일루트에 db.lock 파일에 sEncrypt 내용이 있어야 동작 (바이오스 시리얼번호)
                if(Eqp.bBiosLock) 
                {
                    string mbInfo = String.Empty;
            
                    ManagementScope scope = new ManagementScope("\\\\" + Environment.MachineName + "\\root\\cimv2");
                    scope.Connect();
                    ManagementObject wmiClass = new ManagementObject(scope, new ManagementPath("Win32_BaseBoard.Tag=\"Base Board\""), new ObjectGetOptions());
                    
                    foreach (PropertyData propData in wmiClass.Properties)
                    {
                        if (propData.Name == "SerialNumber")
                            //mbInfo = String.Format("{0,-25}{1}", propData.Name, Convert.ToString(propData.Value));
                            mbInfo = Convert.ToString(propData.Value);
                    }
                    string sEncrypt = Encrypt(mbInfo,"Hanra");

                    string sPath    = Path.GetPathRoot(System.Environment.CurrentDirectory) + sEncrypt + ".lock";
                    if(!File.Exists(sPath)) { 
                        Log.ShowMessage("Error","Need license key");
                        Close();
                    }
                    //string sPath    = Path.GetPathRoot(System.Environment.CurrentDirectory) + "db.lock";
                    //string stext    = System.IO.File.ReadAllText(sPath);
                    //string sDecrypt = Decrypt(stext,"Hanra");
                    //if (mbInfo != sDecrypt) {
                    //    Log.ShowMessage("Error","Need license key");
                    //    Close();
                    //}
                }
            }
            catch
            {
                Log.ShowMessage("Error","Need license key");
                Close();
            }
            
            

            
        }


        public static string Decrypt(string textToDecrypt, string key)

        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.PKCS7;
            rijndaelCipher.KeySize = 128;
            rijndaelCipher.BlockSize = 128;

            byte[] encryptedData = Convert.FromBase64String(textToDecrypt);
            byte[] pwdBytes = Encoding.UTF8.GetBytes(key);
            byte[] keyBytes = new byte[16];

            int len = pwdBytes.Length;
            if (len > keyBytes.Length)
            {
                len = keyBytes.Length;
            }

            Array.Copy(pwdBytes, keyBytes, len);
            rijndaelCipher.Key = keyBytes;
            rijndaelCipher.IV = keyBytes;
            byte[] plainText = rijndaelCipher.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);
            return Encoding.UTF8.GetString(plainText);
        }

        public static string Encrypt(string textToEncrypt, string key)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.PKCS7;
            rijndaelCipher.KeySize = 128;
            rijndaelCipher.BlockSize = 128;

            byte[] pwdBytes = Encoding.UTF8.GetBytes(key);
            byte[] keyBytes = new byte[16];
            int len = pwdBytes.Length;
            if (len > keyBytes.Length)
            {
                len = keyBytes.Length;
            }
            Array.Copy(pwdBytes, keyBytes, len);
            rijndaelCipher.Key = keyBytes;
            rijndaelCipher.IV = keyBytes;
            ICryptoTransform transform = rijndaelCipher.CreateEncryptor();
            byte[] plainText = Encoding.UTF8.GetBytes(textToEncrypt);
            return Convert.ToBase64String(transform.TransformFinalBlock(plainText, 0, plainText.Length));
        }



        private void metroButton1_Click(object sender, EventArgs e)
        {
            
        }


        private void btMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void panel3_DoubleClick(object sender, EventArgs e)
        {
            SM.SetDllMainWin(null);
        }

        private void lbMainThreadTime_Click(object sender, EventArgs e)
        {
            lbMainThreadTime.Enabled = false;
            OM.EqpStat.iSkin++;
            if(OM.EqpStat.iSkin > 14)OM.EqpStat.iSkin = 1 ;
            smMain.Style = (MetroColorStyle)OM.EqpStat.iSkin ;
            lbMainThreadTime.Enabled = true;
        }

        //TODO :: 삭제
        NewOptics client = new NewOptics();
        private void button1_Click(object sender, EventArgs e)
        {
            //client = new NewOptics();
            client.Connect("192.168.1.6",8080);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //client.SendMsg(DateTime.Now.ToString());
            client.SendMsg(NewOptics.test.FCM_3_TEST,"A1234567890");
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lbVer_Click(object sender, EventArgs e)
        {

        }

        private void lbDate_Click(object sender, EventArgs e)
        {

        }

        private void lbName_Click(object sender, EventArgs e)
        {

        }

        private void panel1_DoubleClick(object sender, EventArgs e)
        {
            SM.SetDllMainWin(null);
        }
    }
}

