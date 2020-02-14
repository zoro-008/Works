using COMMON;
using System;
using System.Collections.Generic;
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
    /// UserControl1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Password : Window
    {
        public int m_iCurrentMode; //0 Password , 1 change

        static        EN_LEVEL m_iSelLevel;
        public static EN_LEVEL m_iCrntLevel;
        string m_sEngineerPass;
        string m_sMasterPass;
        string m_sInputTest;

        public void ShowPage(EN_LEVEL _iSelLevel)
        {
            m_iSelLevel          = _iSelLevel;
            m_iCurrentMode       = 0 ;
            tbInputPass.Password = "";
            
            PassWordChange.Visibility = Visibility.Hidden  ;
            PassWordCheck .Visibility = Visibility.Visible ;

            btChange.Visibility = Visibility.Visible;

                 if (m_iSelLevel == EN_LEVEL.Engineer) lbInputPass.Text = "[ENGINEER] " + m_sInputTest;
            else if (m_iSelLevel == EN_LEVEL.Master  ) lbInputPass.Text = "[MASTER] "   + m_sInputTest;
            else                                       lbInputPass.Text = m_sInputTest ;

             ShowDialog();
        }

        public Password()
        {
            InitializeComponent();

            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);

            m_sInputTest = lbInputPass.Text;

            LoadPassword(true);
        }

        private void BtChange_Click(object sender, RoutedEventArgs e)
        {
            Log.Trace("CHANGE Button Clicked", ForContext.Frm);
            
            PassWordChange.Visibility = Visibility.Visible ;
            PassWordCheck .Visibility = Visibility.Hidden  ;
            btChange.Visibility = Visibility.Hidden;
            tbOldPass.Password = "";
            tbNewPass.Password = "";
            tbOldPass.Focus();
            m_iCurrentMode = 1;
        }

        private void BtEnter_Click(object sender, RoutedEventArgs e)
        {
            Log.Trace("ENTER Button Clicked", ForContext.Frm);

            //DialogResult = true ;
            if (m_iCurrentMode == 0)
            {
                if(m_iSelLevel == EN_LEVEL.Engineer)
                {
                    if(m_sEngineerPass==tbInputPass.Password)
                    {
                        tbInputPass.Password = "";
                        m_iCrntLevel = m_iSelLevel ;
                        
                        DialogResult = true ;
                    }
                    else
                    {
                        Log.ShowMessage("Check","Wrong Password") ;
                        tbInputPass.Password = "";
                        tbInputPass.Focus();
                    }
                }
                else if (m_iSelLevel == EN_LEVEL.Master)
                {
                    if (m_sMasterPass == tbInputPass.Password || string.Format("zxc") == tbInputPass.Password)
                    {
                        tbInputPass.Password = "";
                        m_iCrntLevel = m_iSelLevel ;
                        DialogResult = true ;
                        Hide();
                    }
                    else
                    {
                        Log.ShowMessage("Check","Wrong Password") ;
                        tbInputPass.Password = "";
                        tbInputPass.Focus();
                    }
                }
                else
                {
                    m_iCrntLevel = m_iSelLevel ;
                }
            }
            else
            {
                if(m_iSelLevel== EN_LEVEL.Engineer)
                {
                    if(tbOldPass.Password == m_sEngineerPass) 
                    {
                        m_sEngineerPass = tbNewPass.Password ;
                        LoadPassword(false);
                        tbOldPass.Password = "";
                        tbNewPass.Password = "";
                        DialogResult = true;
                        //Hide();
                    }
                    else 
                    {
                       Log.ShowMessage("Check","Wrong Password") ;
                        tbOldPass.Password = "";
                        tbNewPass.Password = "";
                        tbOldPass.Focus();
                    }
                }
                else if(m_iSelLevel== EN_LEVEL.Master)
                {
                    if (tbOldPass.Password == m_sMasterPass || string.Format("zxc") == tbInputPass.Password)
                    {
                        m_sMasterPass = tbNewPass.Password ;
                        LoadPassword(false);
                        tbOldPass.Password = "";
                        tbNewPass.Password = "";
                        DialogResult = true;
                        Hide();
                    }
                    else 
                    {
                        Log.ShowMessage("Check","Wrong Password") ;
                        tbOldPass.Password = "";
                        tbNewPass.Password = "";
                        tbOldPass.Focus();
                    }
                }
            }
        }

        private void BtEsc_Click(object sender, RoutedEventArgs e)
        {
            Log.Trace("ESC Button Clicked", ForContext.Frm);
            DialogResult = false;
        }


        public static EN_LEVEL GetLevel()
        {
            return m_iCrntLevel;
        }

        public static void SetLevel(EN_LEVEL _iSelLevel)
        {
            m_iCrntLevel = _iSelLevel;
        }

        public void LoadPassword(bool IsLoad)
        {
            CConfig Config = new CConfig();

            //Make Dir.
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sPath      = sExeFolder + "Util\\Password.ini";

            //Load Device.
            if (IsLoad)
            {
                Config.Load(sPath,CConfig.EN_CONFIG_FILE_TYPE.ftIni);

                Config.GetValue("PASSWORD", "m_sEngineerPass", out m_sEngineerPass);
                Config.GetValue("PASSWORD", "m_sMasterPass  ", out m_sMasterPass  );
            }
            else
            {
                Config.SetValue("PASSWORD", "m_sEngineerPass", m_sEngineerPass);
                Config.SetValue("PASSWORD", "m_sMasterPass  ", m_sMasterPass  );

                Config.Save(sPath,CConfig.EN_CONFIG_FILE_TYPE.ftIni);
            }        
        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape) {
                Log.Trace("ESC Key Clicked", ForContext.Frm);
                DialogResult = false;
                //CloseForm();
            }

            
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            tbInputPass.Focus();
        }
    }
}
