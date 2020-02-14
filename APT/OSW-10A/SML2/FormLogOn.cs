using COMMON;
using System;
using System.IO;
using System.Windows.Forms;


namespace SML2
{
    public partial class FormLogOn : Form
    {        
        //public enum Stat
        //{
        //    LogOn          = 0,
        //    PasswordChange = 1,
        //    Edit           = 2
        //}
        //public Stat CrntStat; 

        public struct IdData
        {
            public string   ID ;
            public string   Password ;
            public EN_LEVEL Level ;
        }

        private IdData LogedOnData ;
        private string ControlId = "1";
        private string ControlPassword = "1";

        public FormLogOn()
        {
            InitializeComponent();
            LogedOnData.ID       = "LogOff";
            LogedOnData.Password = "1";
            LogedOnData.Level    = EN_LEVEL.LogOff ;
            tmUpdate.Enabled = true ;

        }

        private void FormLogOn_Shown(object sender, EventArgs e)
        {
            pnInput.BringToFront();
            btEdit.Text = "Edit ID";
            if (LogedOnData.Level == EN_LEVEL.LogOff)
            {
                tbID.Text = "ID" ;
                tbPassword.Text = "Password" ;
            }
            tbID.Focus();
        }

        private void FormLogOn_VisibleChanged(object sender, EventArgs e)
        {
            pnInput.BringToFront();
            btEdit.Text = "Edit ID";
            if (LogedOnData.Level == EN_LEVEL.LogOff)
            {
                tbID.Text = "ID" ;
            }
            tbPassword.Text = "Password" ;
            tbID.Focus();
        }

        

        //Main Btm Panel
        //==============================================================
        private void btChangePW_Click(object sender, EventArgs e)
        {
            if (btChangePW.Text == "Change PW")//  m_iCurrentMode == Change.iLogOn)
            {
                if (btEdit.Text == "Log On") btEdit.Text = "Edit ID";
                pnChange.BringToFront();
                btChangePW.Text = "Log On";
                tbOldPass.Clear();
                tbNewPass.Clear();
                tbOldPass.Focus();
            }
            else 
            {
                pnInput.BringToFront();
                btChangePW.Text = "Change PW";
            }
        }

        private void btEdit_Click(object sender, EventArgs e)
        {
            if (btEdit.Text == "Edit ID")
            {
                if (btChangePW.Text == "Log On") btChangePW.Text = "Change PW";
                pnEdit.BringToFront();
                btEdit.Text = "Log On";
                
            }
            else 
            {
                
                pnInput.BringToFront();
                btEdit.Text = "Edit ID";
            }
        }

        //LogOn Panel
        //==============================================================
        
        private void btLogIn_Click(object sender, EventArgs e)
        {
            if (btLogIn.Text == "LogOut")
            {
                LogedOnData.Password = "1";
                LogedOnData.Level    = EN_LEVEL.LogOff ;
                tbID.Enabled = true;//tbID에 포커스 올라가는 타이밍이 타이머에서 tbID Enabled 하는 타이밍보다 빠름
                tbID.Focus();
                return ;
            }

            string Id = tbID.Text ;
            string Pw = tbPassword.Text ;

            if (Id == "ID" || Id == "")
            {
                Log.ShowMessage("Check", "Please Input ID");
                return;
            }

            if (Id == ControlId && Pw == ControlPassword)
            {
                LogedOnData.ID       = ControlId ;
                LogedOnData.Password = ControlPassword  ;
                LogedOnData.Level    = EN_LEVEL.Control ;
                Hide();
                return ;
            }

            IdData CrntData = new IdData();
            LoadId(Id , ref CrntData);           

            if (CrntData.Level == EN_LEVEL.LogOff)
            {
                Log.ShowMessage("Check", "ID does not Exist!");
                return ;
            }
            if (CrntData.Password != Pw )
            {
                Log.ShowMessage("Check", "Wrong Password!");
                tbPassword.Clear();
                return;
            }

            LogedOnData = CrntData ;

            Hide();
        }

        private void tbPassword_Click(object sender, EventArgs e)
        {
            if (tbID.Text == "") tbID.Text = "ID";
            tbPassword.Text = "";
            tbPassword.PasswordChar = '*';
        }

        private void tbID_Click(object sender, EventArgs e)
        {
            if (tbPassword.Text == "")
            {
                tbPassword.PasswordChar = Char.MinValue;
                tbPassword.Text = "Password";
            }
            tbID.Text = "";
        }

        //Edit Panel
        //==============================================================
        private void btEditNew_Click(object sender, EventArgs e)
        {
            if (Log.ShowMessageModal("Confirm", "Do you want to Create a New ID?") != DialogResult.Yes) return;

            string Id = tbEditID.Text ;
            string Pw = tbEditPW.Text ;
            int    Lv = cbEditLevel.SelectedIndex + 1 ;
            if (Id == "")
            {
                Log.ShowMessage("Check", "Please Input ID");
                return;
            }

            IdData CrntData = new IdData();
            LoadId(Id , ref CrntData);        
            if (CrntData.Level != EN_LEVEL.LogOff)
            {
                Log.ShowMessage("Check", "ID Exist Already!");
                return ;
            }

            CrntData.ID       = tbEditID.Text;
            CrntData.Password = tbEditPW.Text;
            CrntData.Level    = (EN_LEVEL)(cbEditLevel.SelectedIndex + 1);
            SaveId(CrntData);
        }

        private void btEditChange_Click(object sender, EventArgs e)
        {
            if (Log.ShowMessageModal("Confirm", "Do you want to Change the Id Data?") != DialogResult.Yes) return;

            string Id = tbEditID.Text ;
            string Pw = tbEditPW.Text ;
            int    Lv = cbEditLevel.SelectedIndex + 1 ;

            if (Id == "")
            {
                Log.ShowMessage("Check", "Please Input ID");
                return;
            }

            IdData CrntData = new IdData();
            LoadId(Id , ref CrntData);        
            if (CrntData.Level == EN_LEVEL.LogOff)
            {
                Log.ShowMessage("Check", "ID does not Exist!");
                return ;
            }

            CrntData.Password = Pw ;
            CrntData.Level    = (EN_LEVEL)Lv ;
            SaveId(CrntData);
        }

        private void btDelete_Click(object sender, EventArgs e)
        {
            if (Log.ShowMessageModal("Confirm", "Do you want to Delete the Id Data?") != DialogResult.Yes) return;

            string Id = tbEditID.Text ;
            string Pw = tbEditPW.Text ;
            int    Lv = cbEditLevel.SelectedIndex + 1 ;

            if (Id == "")
            {
                Log.ShowMessage("Check", "Please Input ID");
                return;
            }

            IdData CrntData = new IdData();
            LoadId(Id , ref CrntData);        
            if (CrntData.Level == EN_LEVEL.LogOff)
            {
                Log.ShowMessage("Check", "ID does not Exist!");
                return ;
            }

            DeleteId(CrntData.ID);           
        }        

        public void LoadId(string _sId , ref IdData Data)
        {
            //Make Dir.
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sPath = sExeFolder + "Util\\LogOn.ini";
            CIniFile Ini = new CIniFile(sPath);
            int iTemp = 0 ;
            
            //Load Device.

            Ini.Load(_sId, "Password", out Data.Password);
            Ini.Load(_sId, "Level"   , out iTemp        ); Data.Level = (EN_LEVEL)iTemp ;
            Data.ID = _sId ;

        }
        public void SaveId(IdData Data)
        {
            //Make Dir.
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sPath = sExeFolder + "Util\\LogOn.ini";
            CIniFile Ini = new CIniFile(sPath);
            int iTemp = 0 ;
            
            Ini.Save(Data.ID, "Password", Data.Password);
            iTemp = (int)Data.Level ;
            Ini.Save(Data.ID, "Level"   , iTemp        );             
        }

        public void DeleteId(string _sId)
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sPath = sExeFolder + "Util\\LogOn.ini";
            CIniFile Ini = new CIniFile(sPath);

            Ini.DeleteSection(_sId);
        }

        private void btDeleteAll_Click(object sender, EventArgs e)
        {
            string sPath;

            sPath = Application.StartupPath + "\\Util\\LogOn.ini";

            if (File.Exists(sPath))
            {
                if (Log.ShowMessageModal("Confirm", "Do you want to delete the All ID?") != DialogResult.Yes) return;
                File.Delete(sPath);
                
                if(LogedOnData.Level != EN_LEVEL.Control)SaveId(LogedOnData);
                
            }
        }

        public EN_LEVEL GetLevel()
        {
            return LogedOnData.Level ;
        }

        public string GetId()
        {
            return LogedOnData.ID ;
        }

        private void btEsc_Click(object sender, EventArgs e)
        {
            btEsc.DialogResult = DialogResult.Cancel;
            Hide();
        }

        //Change Panel
        //==============================================================
        private void btEnter_Click(object sender, EventArgs e)    //Password 변경
        {
      
            if (tbOldPass.Text == LogedOnData.Password)//예전 비번 확인됐을때
            {
                LogedOnData.Password = tbNewPass.Text;
                SaveId(LogedOnData);
                tbOldPass.Clear();
                tbNewPass.Clear();
                btEnter.DialogResult = DialogResult.OK;
                pnInput.BringToFront();
                btChangePW.Text = "ChangePW";
                //Hide();
            }
            else //예전 비번 확인 안되면
            {
                Log.ShowMessage("Check", "Wrong Old Password");
                tbOldPass.Clear();
                tbNewPass.Clear();
                tbOldPass.Focus();
            }
        }

        //private void tbInputPass_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Enter)
        //    {
        //        btEnter_Click(sender, e);
        //    }

        //    else if (e.KeyCode == Keys.Escape)
        //    {
        //        btEsc_Click(sender, e);
        //    }
        //} 

        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            tbID.Enabled       = LogedOnData.Level==EN_LEVEL.LogOff ;
            tbPassword.Enabled = LogedOnData.Level==EN_LEVEL.LogOff ;
            btChangePW.Visible = LogedOnData.Level!=EN_LEVEL.LogOff ;
            btLogIn   .Text    = LogedOnData.Level==EN_LEVEL.LogOff ? "LogIn" : "LogOut";

            if (LogedOnData.Level >= EN_LEVEL.Master) { 
                btEdit.Visible      = true ;
                btDeleteAll.Visible = true ;
            }
            else
            {
                btEdit.Visible      = false ;
                btDeleteAll.Visible = false ;
            }
            if (tbPassword.Focused)
            {
                tbPassword.PasswordChar = '*';
            }

        }

        private void tbPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                btLogIn_Click(sender, e);
            }

            else if (e.KeyCode == Keys.Escape)
            {
                btEsc_Click(sender, e);
            }
        }

        private void tbPassword_CursorChanged(object sender, EventArgs e)
        {

        }

        
    }
}
