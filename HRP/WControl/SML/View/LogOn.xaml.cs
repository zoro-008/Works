using COMMON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SML.View
{
    /// <summary>
    /// LogOn.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LogOn : Window
    {
        public DispatcherTimer Timer = new DispatcherTimer();

        public struct IdData
        {
            public string   ID ;
            public string   Password ;
            public EN_LEVEL Level ;
        }

        private IdData LogedOnData ;
        private string ControlId = "1";
        private string ControlPassword = "1";

        public LogOn()
        {
            InitializeComponent();

            LogedOnData.ID       = "LogOff";
            LogedOnData.Password = "1";
            LogedOnData.Level    = EN_LEVEL.LogOff ;

            EDIT.Visibility           = Visibility.Hidden  ;
            PassWordChange.Visibility = Visibility.Hidden  ;
            Login.Visibility          = Visibility.Visible ;

            if (LogedOnData.Level == EN_LEVEL.LogOff)
            {
                tbLoginId.Text = ""    ;//"ID" ;
                tbLoginPassword.Clear();//"Password" ;
            }
            tbLoginId.Focus();

            //UI 타이머
            Timer.Interval = TimeSpan.FromMilliseconds(100);
            Timer.Tick += new EventHandler(Timer_Tick);

        }
               
        private void Timer_Tick(object sender, EventArgs e)
        {
            tbLoginId.IsReadOnly       = LogedOnData.Level!=EN_LEVEL.LogOff ;
            tbLoginPassword.IsEnabled  = LogedOnData.Level==EN_LEVEL.LogOff ;
            btChange.Visibility        = LogedOnData.Level!=EN_LEVEL.LogOff ? Visibility.Visible : Visibility.Hidden;
            tbLogout.Text              = LogedOnData.Level==EN_LEVEL.LogOff ? " LOG IN" : " LOG OUT";

            if (LogedOnData.Level >= EN_LEVEL.Master) { 
                btEdit.Visibility      = Visibility.Visible;
                btDeleteAll.Visibility = Visibility.Visible;
            }
            else
            {
                btEdit.Visibility      = Visibility.Hidden;
                btDeleteAll.Visibility = Visibility.Hidden;
            }
        }
        private void BtChange_Click(object sender, RoutedEventArgs e)
        {
            string sText = tbChange.Text;
            Log.Trace(this.GetType().Name + " " + sText + " Button Clicked", ForContext.Frm);

            if (tbChange.Text == " CHANGE")//  m_iCurrentMode == Change.iLogOn)
            {
                if (tbEdit.Text == " LOG ON") tbEdit.Text = " EDIT ID";
                EDIT.Visibility           = Visibility.Hidden  ;
                PassWordChange.Visibility = Visibility.Visible ;
                Login.Visibility          = Visibility.Hidden  ;

                tbChange.Text = " LOG IN";

                tbOldPass.Clear();
                tbNewPass.Clear();
                tbOldPass.Focus();
            }
            else 
            {
                EDIT.Visibility           = Visibility.Hidden  ;
                PassWordChange.Visibility = Visibility.Hidden  ;
                Login.Visibility          = Visibility.Visible ;

                tbChange.Text = " CHANGE";
            }
        }

        private void BtEdit_Click(object sender, RoutedEventArgs e)
        {
            string sText = tbEdit.Text;
            Log.Trace(this.GetType().Name + " " + sText + " Button Clicked", ForContext.Frm);

            if (tbEdit.Text == " EDIT ID")
            {
                if (tbChange.Text == "LOG IN") tbChange.Text = " CHANGE";
                EDIT.Visibility           = Visibility.Visible ;
                PassWordChange.Visibility = Visibility.Hidden  ; 
                Login.Visibility          = Visibility.Hidden  ;
                tbEdit.Text = " LOG IN";
                
            }
            else 
            {
                EDIT.Visibility           = Visibility.Hidden  ;
                PassWordChange.Visibility = Visibility.Hidden  ;
                Login.Visibility          = Visibility.Visible ;
                tbEdit.Text = " EDIT ID";
            }
        }

        private void BtEsc_Click(object sender, RoutedEventArgs e)
        {
            string sText = tbEsc.Text;
            Log.Trace(this.GetType().Name + " " + sText + " Button Clicked", ForContext.Frm);

            //DialogResult = false;
            Hide();
        }

        private void BtEnter_Click(object sender, RoutedEventArgs e)
        {
            if (tbOldPass.Password == LogedOnData.Password)//예전 비번 확인됐을때
            {
                LogedOnData.Password = tbNewPass.Password;
                SaveId(LogedOnData);
                tbOldPass.Clear();
                tbNewPass.Clear();
                //btEnter.DialogResult = DialogResult.OK;
                //pnInput.BringToFront();
                EDIT.Visibility           = Visibility.Hidden  ;
                PassWordChange.Visibility = Visibility.Hidden  ;
                Login.Visibility          = Visibility.Visible ;

                tbChange.Text = "ChangePW";
                Hide();
            }
            else //예전 비번 확인 안되면
            {
                Log.ShowMessage("Check", "Wrong Old Password");
                tbOldPass.Clear();
                tbNewPass.Clear();
                tbOldPass.Focus();
            }
        }




        public void SetLevel(EN_LEVEL _eLevel)
        {
            LogedOnData.Level = _eLevel ;
        }

        public EN_LEVEL GetLevel()
        {
            return LogedOnData.Level ;
        }

        public string GetId()
        {
            return LogedOnData.ID ;
        }

        public void LoadId(string _sId , ref IdData Data)
        {
            //Make Dir.
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sPath = sExeFolder + "Util\\LogOn.ini";
            CIniFile Ini = new CIniFile(sPath);
            int iTemp = 0 ;
            
            //Load Device.

            Ini.Load(_sId, "Password", ref Data.Password);
            Ini.Load(_sId, "Level"   , ref iTemp        ); Data.Level = (EN_LEVEL)iTemp ;
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

        private void BtLogOut_Click(object sender, RoutedEventArgs e)
        {
            string sText = tbLogout.Text;
            Log.Trace(this.GetType().Name + " " + sText + " Button Clicked", ForContext.Frm);

            if (tbLogout.Text == " LOG OUT")
            {
                LogedOnData.Password = "1";
                LogedOnData.Level    = EN_LEVEL.LogOff ;
                tbLoginId.IsEnabled  = true;//tbID에 포커스 올라가는 타이밍이 타이머에서 tbID Enabled 하는 타이밍보다 빠름
                tbLoginId.Focus();
                return ;
            }

            string Id = tbLoginId.Text          ;
            string Pw = tbLoginPassword.Password;

            //if (Id == "ID" || Id == "")
            if (Id == "")
            {
                Log.ShowMessage("Check", "Please Input ID");
                return;
            }

            if (Id == ControlId && Pw == ControlPassword)
            {
                LogedOnData.ID       = ControlId ;
                LogedOnData.Password = ControlPassword  ;
                LogedOnData.Level    = EN_LEVEL.Control ;
                Log.Trace("LogOn ID : " + LogedOnData.ID + " . Level : " + LogedOnData.Level, ForContext.Frm);
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
            else
            {
                Log.Trace("LogOn ID : " + CrntData.ID + " . Level : " + CrntData.Level , ForContext.Frm);
            }
            if (CrntData.Password != Pw )
            {
                Log.ShowMessage("Check", "Wrong Password!");
                tbLoginPassword.Clear();
                return;
            }

            LogedOnData = CrntData ;

            Hide();
        }

        private void BtEditNew_Click(object sender, RoutedEventArgs e)
        {
            if (!Log.ShowMessageModal("Confirm", "Do you want to Create a New ID?")) return;

            string Id = tbEditID.Text     ;
            string Pw = tbEditPW.Password ;
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
            CrntData.Password = tbEditPW.Password;
            CrntData.Level    = (EN_LEVEL)(cbEditLevel.SelectedIndex + 1);
            SaveId(CrntData);

        }

        private void BtEditChange_Click(object sender, RoutedEventArgs e)
        {
            if (!Log.ShowMessageModal("Confirm", "Do you want to Change the Id Data?")) return;

            string Id = tbEditID.Text     ;
            string Pw = tbEditPW.Password ;
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


        public void DeleteId(string _sId)
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sPath = sExeFolder + "Util\\LogOn.ini";
            CIniFile Ini = new CIniFile(sPath);

            Ini.DeleteSection(_sId);
        }

        private void BtDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!Log.ShowMessageModal("Confirm", "Do you want to Delete the Id Data?")) return;

            string Id = tbEditID.Text     ;
            string Pw = tbEditPW.Password ;
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

        private void BtDeleteAll_Click(object sender, RoutedEventArgs e)
        {
            string sPath;

            //sPath = Application.StartupPath + "\\Util\\LogOn.ini";
            sPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\Util\\LogOn.ini";
            

            if (File.Exists(sPath))
            {
                if (!Log.ShowMessageModal("Confirm", "Do you want to delete the All ID?")) return;
                File.Delete(sPath);
                
                if(LogedOnData.Level != EN_LEVEL.Control)SaveId(LogedOnData);
                
            }
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(IsVisible) Timer.Start();
            else          Timer.Stop ();
        }
    }
}
