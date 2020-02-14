using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Management;
using System.IO;
using COMMON;
using System.Security.Cryptography;
using System.Text;
using SML;

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

            FrmDevice.TopLevel = false;
            FrmDevice.Parent = pnBase;
   
            lbDevice.Text = OM.GetCrntDev().ToString();

            lbLotNo.Text  = LOT.GetLotNo();
            lbName.Text   = OM.EqpOptn.sModelName;

            ShowPage(0);//Operation.

            //MainSol
            SM.DIO.SetY((int)yi.ETC_MainAirSol, true);
            //SM.IO.SetY((int)yi.PCK_VacSol    , true);

            tmUpdate.Enabled = true;

            

        }
         ~FormMain()
        {
            MessageBox.Show("123", "123");
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            tmLock.Enabled = Eqp.bBiosLock ;
            
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

        public void ShowPage(int _iPageIdx)
        {
            FrmDeviceSet.UpdateDevInfo(true);
            FrmDeviceSet.UpdateDevOptn(true);
            PM.UpdatePstn(true);

            switch (_iPageIdx)
            {
                case 0: FrmOperation.Show();          break;
                case 1: 
                        break;
                case 2: FrmDevice   .Show();          break;
                case 3: FrmOption   .Show();          
                        FrmOption.bUpdate = true;     break;
                case 4: FrmSPC      .Show();          
                        FrmSPC      .ShowUpdate();    break;
                case 5: SM.SetDllMainWin(pnBase);     break;
                default: FrmOperation.Show();         break;
            }
          
        }
        private void HidePage(int _iPageIdx)
        {
            switch (_iPageIdx)
            {
                case 0: FrmOperation.Hide();          break;
                case 1:           break;
                case 2: FrmDevice   .Hide();          
                        FrmDeviceSet.Hide();          break;
                case 3: FrmOption   .Hide();          break;
                case 4: FrmSPC      .Hide();          break;
                case 5: SM.HideDllMainWin();          break;
                default: FrmOperation.Hide();         break;
            }
        }

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
            if (SM.FrmPassword.GetLevel() == EN_LEVEL.Operator)
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
                        return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void tmLock_Tick(object sender, EventArgs e)
        {
            tmLock.Enabled = false ;
            try
            {
                //프로그램 Lock 설정된 PC에서만 동작하게 함.
                //실행파일루트에 db.lock 파일에 sEncrypt 내용이 있어야 동작 (바이오스 시리얼번호)
                   
                
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

                string sEncrypt = Encrypt(mbInfo, "Hanra");

                string value = System.Text.RegularExpressions.Regex.Replace(sEncrypt, "[/*?\"<>|]", "");//특수문자 제거.

                string sPath = Path.GetPathRoot(System.Environment.CurrentDirectory) + value + ".txt";
                //sPath = Path.GetPathRoot(System.Environment.CurrentDirectory) + "sun.txt";
                string [] sFiles = Directory.GetFiles(Path.GetPathRoot(System.Environment.CurrentDirectory));
                //D:\\10oWVZ7aAoMXejNp4kiSA==.txt
                //D:\\10oWV/Z7aAoMXejNp4kiSA==.txt

          
                if (!File.Exists(sPath))
                {
                    Log.ShowMessageModal("Error", "Need license key - Code : "+ mbInfo);
                    SEQ.Close();
                    FrmOperation.Close();
                    FrmDevice.Close();
                    FrmDeviceSet.Close();
                    FrmMaster.Close();
                    FrmOption.Close();

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
            catch
            {
                Log.ShowMessageModal("Error", "Error on verifying license key");
                SEQ.Close();
                FrmOperation.Close();
                FrmDevice.Close();
                FrmDeviceSet.Close();
                FrmMaster.Close();
                FrmOption.Close();

                Close();
            }
        }
    }
}
