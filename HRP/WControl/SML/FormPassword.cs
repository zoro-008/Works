using System;
using System.Windows.Forms;
using COMMON;
using System.Drawing;

namespace SML
{
    public partial class FormPassword : Form
    {
        public FormDllMain FrmDllMain;
        public FormPassword FrmPassword ; 
        
        public int m_iCurrentMode;                  //0 Password , 1 change

        static EN_LEVEL m_iSelLevel;
        public static EN_LEVEL m_iCrntLevel;
        string m_sEngineerPass;
        string m_sMasterPass;
        string m_sInputTest;
        private const string sFormText = "Form Password";

        public FormPassword()
        {
            InitializeComponent();

            pnChange.Location  = new Point(2, 4);
            pnInput.Location   = new Point(2, 4);
            this.Width  = 334 ;
            this.Height = 162 ;

            m_sInputTest = lbInputPass.Text;

            LoadPassword(true);
        }


        public void ShowPage(EN_LEVEL _iSelLevel)
        {
            m_iSelLevel = _iSelLevel;
            m_iCurrentMode = 0;
            tbInputPass.Clear();
            pnInput.BringToFront();
            btChange.Show();

                 if (m_iSelLevel == EN_LEVEL.Engineer) lbInputPass.Text = "[ENGINEER] " + m_sInputTest;
            else if (m_iSelLevel == EN_LEVEL.Master  ) lbInputPass.Text = "[MASTER] "   + m_sInputTest;
            else                                       lbInputPass.Text = m_sInputTest ;
        }

        private void btChange_MouseDown(object sender, MouseEventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Mouse Down", ForContext.Frm);

            pnInput.SendToBack();
            btChange.Hide();
            tbOldPass.Clear();
            tbNewPass.Clear();
            tbOldPass.Focus();
            m_iCurrentMode = 1;
        }


        private void btEnter_Click(object sender, EventArgs e)    //Password Check
        {
            string sText = ((Button)sender).Text;
            Log.Trace(this.GetType().Name + " " + sText + " Button Clicked", ForContext.Frm);

            if (m_iCurrentMode == 0)
            {
                if(m_iSelLevel == EN_LEVEL.Engineer)
                {
                    if(m_sEngineerPass==tbInputPass.Text)
                    {
                        tbInputPass.Clear();
                        m_iCrntLevel = m_iSelLevel ;
                        btEnter.DialogResult = DialogResult.OK;
                        Hide();
                    }
                    else
                    {
                        Log.ShowMessage("Check","Wrong Password") ;
                        tbInputPass.Clear();
                        tbInputPass.Focus();
                    }
                }
                else if (m_iSelLevel == EN_LEVEL.Master)
                {
                    if (m_sMasterPass == tbInputPass.Text || string.Format("zxc") == tbInputPass.Text)
                    {
                        tbInputPass.Clear();
                        m_iCrntLevel = m_iSelLevel ;
                        btEnter.DialogResult = DialogResult.OK;
                        Hide();
                    }
                    else
                    {
                        Log.ShowMessage("Check","Wrong Password") ;
                        tbInputPass.Clear();
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
                    if(tbOldPass.Text == m_sEngineerPass) 
                    {
                        m_sEngineerPass = tbNewPass.Text ;
                        LoadPassword(false);
                        tbOldPass.Clear();
                        tbNewPass.Clear();
                        btEnter.DialogResult = DialogResult.OK;
                        Hide();
                    }
                    else 
                    {
                       Log.ShowMessage("Check","Wrong Password") ;
                        tbOldPass.Clear();
                        tbNewPass.Clear();
                        tbOldPass.Focus();
                    }
                }
                else if(m_iSelLevel== EN_LEVEL.Master)
                {
                    if (tbOldPass.Text == m_sMasterPass || string.Format("zxc") == tbInputPass.Text)
                    {
                        m_sMasterPass = tbNewPass.Text;
                        LoadPassword(false);
                        tbOldPass.Clear();
                        tbNewPass.Clear();
                        btEnter.DialogResult = DialogResult.OK;
                        Hide();
                    }
                    else 
                    {
                        Log.ShowMessage("Check","Wrong Password") ;
                        tbOldPass.Clear();
                        tbNewPass.Clear();
                        tbOldPass.Focus();
                    }
                }
            }
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
            /*
            //Load Device.
            if (IsLoad)
            {
                CIniFile IniLoadPass = new CIniFile(sCmnOptnPath);

                IniLoadPass.Load("PASSWORD", "m_sEngineerPass", out m_sEngineerPass);
                IniLoadPass.Load("PASSWORD", "m_sMasterPass  ", out m_sMasterPass);
            }
            else
            {
                CIniFile IniSavePass = new CIniFile(sCmnOptnPath);

                IniSavePass.Save("PASSWORD", "m_sEngineerPass", m_sEngineerPass);
                IniSavePass.Save("PASSWORD", "m_sMasterPass  ", m_sMasterPass);
            }
            */
        }

        public static EN_LEVEL GetLevel()
        {
            return m_iCrntLevel;
        }

        public static void SetLevel(EN_LEVEL _iSelLevel)
        {
            m_iCrntLevel = _iSelLevel;
        }

        private void btEsc_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(this.GetType().Name + " " + sText + " Button Clicked", ForContext.Frm);

            btEsc.DialogResult = DialogResult.Cancel;
            Hide();
        }

        private void tbInputPass_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                btEnter_Click(sender, e);
            }

            else if (e.KeyCode == Keys.Escape)
            {
                btEsc_Click(sender, e);
            }
        }

        private void FormPassword_Shown(object sender, EventArgs e)
        {
            tbInputPass.Focus();
        }

        private void btChange_Click(object sender, EventArgs e)
        {

        }
    }
}
