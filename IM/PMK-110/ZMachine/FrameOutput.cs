﻿using System;
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
            sOutputName = sOutputName.Substring(4, sOutputName.Length -4);

            m_iYadd      = _iYadd;
            lbTitle.Text = sOutputName;
            this.Parent  = _wcParent;

            m_bPreDone       = false;
            tmUpdate.Enabled = true;
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

            bool bOut = SM.DIO.GetY((int)m_iYadd, false);

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

            tmUpdate.Enabled = true;
        }

        private void btAction_Click(object sender, EventArgs e)
        {
            SM.DIO.SetY((int)m_iYadd, !SM.DIO.GetY((int)m_iYadd));

            string sMsg;
            sMsg = lbTitle.Text + " Button Click " + (SM.DIO.GetY((int)m_iYadd) ? "(ON)" : "(OFF)").ToString();
            
            Log.Trace("Operator", sMsg);
        }

        private void FraOutput_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmUpdate.Enabled = false;
        }
    }
}
