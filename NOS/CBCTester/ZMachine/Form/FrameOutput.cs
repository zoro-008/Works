using System;
using System.Drawing;
using System.Windows.Forms;
using COMMON;
using SML;

namespace Machine
{
    public partial class FraOutput : Form
    {
        public FraOutput()
        {
            InitializeComponent();
        }

        public        yi m_iYadd;
        public bool   m_bPreDone;

        public void SetConfig(yi _iYadd, string _sTitle, Control _wcParent)
        {
            string sOutputName;
            sOutputName = _sTitle;
            if (sOutputName == "") return;
            sOutputName = sOutputName.Substring(5, sOutputName.Length -5);

            m_iYadd      = _iYadd;
            lbTitle.Text = sOutputName;
            this.Parent  = _wcParent;

            m_bPreDone       = false;
            //tmUpdate.Enabled = true;
        }

        public void SetSize()
        {
            btAction.Top    = lbTitle.Top + lbTitle.Height + 1;
            btAction.Height = this.Height - btAction.Top;
            btAction.Width  = this.Width;
            btAction.Left   = 0;
        }

        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            tmUpdate.Enabled = false;

            bool bOut = ML.IO_GetY(m_iYadd, false);

            if (bOut)
            {
                btAction.ForeColor = Color.Lime;
                btAction.Text      = "ON";
            }
            else
            {
                btAction.ForeColor = Color.Black;
                btAction.Text      = "OFF";
            }

            if (!this.Visible)
            {
                tmUpdate.Enabled = false;
                return;
            }
            tmUpdate.Enabled = true;
        }

        private void btAction_Click(object sender, EventArgs e)
        {
            ML.IO_SetY(m_iYadd, !ML.IO_GetY(m_iYadd));

            string sMsg;
            sMsg = lbTitle.Text + " Button Click " + (ML.IO_GetY(m_iYadd) ? "(ON)" : "(OFF)").ToString();
            
            Log.Trace(sMsg,ForContext.Frm);
        }

        private void FraOutput_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmUpdate.Enabled = false;
        }

        private void FraOutput_Shown(object sender, EventArgs e)
        {
            tmUpdate.Enabled = true;
        }

        private void FraOutput_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible) tmUpdate.Enabled = true;
        }
    }
}
