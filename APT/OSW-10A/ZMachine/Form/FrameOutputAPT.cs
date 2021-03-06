﻿using COMMON;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Machine
{
    public partial class FrameOutputAPT : Form
    {
        public FrameOutputAPT()
        {
            InitializeComponent();
        }

        public yi   m_iYadd;
        public yi   m_iYHexadd;
        public bool m_bPreDone;

        public void SetConfig(yi _iYadd, string _sTitle, Control _wcParent)
        {
            string sInputName;
            sInputName = _sTitle;
            if (sInputName == "") return;
            //sInputName = sInputName.Substring(5, sInputName.Length - 5);

            m_iYadd = _iYadd;
            lbAdd.Text = "NO : " + ((int)m_iYadd).ToString();
            lbHexAdd.Text = string.Format("Y{0:X2}", (int)m_iYadd);
            lbTitle.Text = sInputName;
            this.Parent = _wcParent;

            m_bPreDone = false;
            tmUpdate.Enabled = true;
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

            bool bOut = SM.IO_GetY(m_iYadd, false);

            if (bOut)
            {
                lbState.BackColor = Color.Red;
                lbState.Text = "ON";
            }
            else
            {
                lbState.BackColor = SystemColors.Control;
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

        private void lbState_DoubleClick(object sender, EventArgs e)
        {
            SM.IO_SetY(m_iYadd, !SM.IO_GetY(m_iYadd));

            string sMsg;
            sMsg = lbTitle.Text + " Button Click " + (SM.IO_GetY(m_iYadd) ? "(ON)" : "(OFF)").ToString();

            Log.Trace("Operator", sMsg);
        }


    }
}
