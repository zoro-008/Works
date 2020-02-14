using System;
using System.Drawing;
using System.Windows.Forms;

namespace Machine
{
    public partial class FrameInputAPT : Form
    {
        public FrameInputAPT()
        {
            InitializeComponent();
        }

        public xi   m_iXadd;
        public xi   m_iXHexadd;
        //public bool m_bPreDone;

        public void SetConfig(xi _iXadd, string _sTitle, Control _wcParent)
        {
            string sInputName;
            sInputName = _sTitle;
            if (sInputName == "") return;
            //sInputName = sInputName.Substring(5, sInputName.Length - 5);

            m_iXadd = _iXadd;
            lbAdd.Text = "NO : " + ((int)m_iXadd).ToString();
            lbHexAdd.Text = string.Format("X{0:X2}", (int)m_iXadd);
            lbTitle.Text = sInputName;
            this.Parent = _wcParent;
            this.Dock   = DockStyle.Fill;

            //m_bPreDone = false;
            //tmUpdate.Enabled = true;
        }

        public void SetSize()
        {
            //btAction.Top = lbTitle.Top + lbTitle.Height + 1;
            //btAction.Height = this.Height - btAction.Top;
            //btAction.Width = this.Width;
            //btAction.Left = 0;
        }

        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            tmUpdate.Enabled = false;

            bool bIn = ML.IO_GetX(m_iXadd);

            if (bIn)
            {
                lbState.BackColor = Color.Red;
                lbState.Text = "ON";
            }
            else
            {
                lbState.BackColor = SystemColors.Control;//Color.FloralWhite;//
                lbState.Text = "OFF";
            }

            tmUpdate.Enabled = true;
        }

        private void btAction_Click(object sender, EventArgs e)
        {
            //SM.IO_SetY(m_iYadd, !SM.IO_GetY(m_iYadd));
            //
            //string sMsg;
            //sMsg = lbTitle.Text + " Button Click " + (SM.IO_GetY(m_iYadd) ? "(ON)" : "(OFF)").ToString();
            //
            //Log.Trace("Operator", sMsg);
        }

        private void FraOutput_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmUpdate.Enabled = false;
        }

        private void FrameInputAPT_Shown(object sender, EventArgs e)
        {
            tmUpdate.Enabled = true;
        }

        private void FrameInputAPT_VisibleChanged(object sender, EventArgs e)
        {
            tmUpdate.Enabled = this.Visible;
        }
    }
}
