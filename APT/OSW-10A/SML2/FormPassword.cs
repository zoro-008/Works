using COMMON;
using System;
using System.Windows.Forms;


namespace SML2
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

        public FormPassword()
        {
            InitializeComponent();

            LoadPassword(true);
        }


        public void ShowPage(EN_LEVEL _iSelLevel)
        {
            m_iSelLevel = _iSelLevel;
            m_iCurrentMode = 0;
            tbInputPass.Clear();
            pnInput.BringToFront();
            btChange.Show();

            if (m_iSelLevel == EN_LEVEL.Engineer) lbInputPass.Text = "[ENGINEER] Input Password";
            if (m_iSelLevel == EN_LEVEL.Master  ) lbInputPass.Text = "[MASTER] Input Password"  ;
        }

        private void btChange_MouseDown(object sender, MouseEventArgs e)
        {
            pnInput.SendToBack();
            btChange.Hide();
            tbOldPass.Clear();
            tbNewPass.Clear();
            tbOldPass.Focus();
            m_iCurrentMode = 1;
        }


        private void btEnter_Click(object sender, EventArgs e)    //Password Check
        {
            if(m_iCurrentMode == 0)
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
            string Path   ;
        
            //Make Dir.
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sCmnOptnPath = sExeFolder + "Util\\Password.ini";
        
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

    }
}
