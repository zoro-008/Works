using System;
using System.Drawing;
using System.Windows.Forms;
using COMMON;
using SML;

namespace Machine
{
    public partial class FrameCylinderAPT : Form
    {
        public FrameCylinderAPT()
        {
            InitializeComponent();

           
            //this.Width = 378;
            //this.Height = 127;
            //Control Scale 변경
            //float widthRatio =  180f  / 378f ;
            //float heightRatio = 58f   / 127f ;
            //SizeF scale = new SizeF(widthRatio, heightRatio);
            //this.Scale(scale);
            //
            //foreach (Control control in this.Controls)
            //{
            //    control.Font = new Font("Verdana", control.Font.SizeInPoints * heightRatio * widthRatio);
            //}

        }

        public ci                 m_iActrId   ;
        public string             m_sActrName ;
        public EN_MOVE_DIRECTION  m_iType     ;
        public bool               m_bPreCmd   ;
        public string             m_sCaptionFw;
        public string             m_sCaptionBw;
        public dgCheckSafe        m_CheckSafe;
        public bool[] bActivate = new bool[(int)ci.MAX_ACTR];

        public int iFwd = 0;
        public int iBwd = 0;

        public string sFwd = "";
        public string sBwd = "";


        public const int Up    = 0,
                         Down  = 1,
                         Left  = 2,
                         Right = 3,
                         FWD   = 4,
                         BWD   = 5,
                         CW    = 6,
                         CCW   = 7,
                         OPEN  = 8,
                         CLOSE = 9;

        public void SetConfig(ci _iActrId, string _sTitle, EN_MOVE_DIRECTION _iActrType, Control _wcParent /*, dgCheckSafe _CheckSafe*/)
        {
            m_sActrName = _sTitle;
            //bActivate = new bool [SML.CL]
            if(m_sActrName == null) return;
            m_sActrName = m_sActrName.Replace("_", " ");
        
            m_iActrId = _iActrId;      //실린더 넘버
            lbCylNo.Text = ((int)m_iActrId).ToString();
            lbCylName.Text = m_sActrName;//실린더 이름
            m_iType = _iActrType;      
            this.Parent  = _wcParent;
            this.Dock    = DockStyle.Fill;
        
            m_bPreCmd = true;
            //tmUpdate.Enabled = true;

            switch (m_iType)
            {
                default: iFwd = Left; sFwd = "LEFT"; iBwd = Right; sBwd = "RIGHT"; break;

                case EN_MOVE_DIRECTION.LR: sBwd = "LEFT"  ; btBwd.Click += new EventHandler(evBwd_Click);
                                           sFwd = "RIGHT" ; btFwd.Click += new EventHandler(evFwd_Click); break;
                case EN_MOVE_DIRECTION.RL: sBwd = "RIGHT" ; btBwd.Click += new EventHandler(evBwd_Click);
                                           sFwd = "LEFT"  ; btFwd.Click += new EventHandler(evFwd_Click); break; 
                case EN_MOVE_DIRECTION.BF: sBwd = "BWD"   ; btBwd.Click += new EventHandler(evBwd_Click);
                                           sFwd = "FWD"   ; btFwd.Click += new EventHandler(evFwd_Click); break;
                case EN_MOVE_DIRECTION.FB: sBwd = "BWD"   ; btBwd.Click += new EventHandler(evBwd_Click);
                                           sFwd = "FWD"   ; btFwd.Click += new EventHandler(evFwd_Click); break;
                case EN_MOVE_DIRECTION.UD: sBwd = "UP"    ; btBwd.Click += new EventHandler(evBwd_Click);
                                           sFwd = "DN"    ; btFwd.Click += new EventHandler(evFwd_Click); break;
                case EN_MOVE_DIRECTION.DU: sBwd = "DN"    ; btBwd.Click += new EventHandler(evBwd_Click);
                                           sFwd = "UP"    ; btFwd.Click += new EventHandler(evFwd_Click); break;
                case EN_MOVE_DIRECTION.CA: sBwd = "CW"    ; btBwd.Click += new EventHandler(evBwd_Click);
                                           sFwd = "CCW"   ; btFwd.Click += new EventHandler(evFwd_Click); break;
                case EN_MOVE_DIRECTION.AC: sBwd = "CCW"   ; btBwd.Click += new EventHandler(evBwd_Click);
                                           sFwd = "CW"    ; btFwd.Click += new EventHandler(evFwd_Click); break;
                case EN_MOVE_DIRECTION.OC: sBwd = "OPEN"  ; btBwd.Click += new EventHandler(evBwd_Click);
                                           sFwd = "CLOSE" ; btFwd.Click += new EventHandler(evFwd_Click); break;
                case EN_MOVE_DIRECTION.CO: sBwd = "CLOSE" ; btBwd.Click += new EventHandler(evBwd_Click);
                                           sFwd = "OPEN"  ; btFwd.Click += new EventHandler(evFwd_Click); break;
            }

            btFwd.Text = sFwd;
            btBwd.Text = sBwd;
            //lbFwd.Text = sFwd; 
            //lbBwd.Text = sBwd;
        }

        private void FrameCylinderAPT_Shown(object sender, EventArgs e)
        {
            tmUpdate.Enabled = true;
        }

        private void FrameCylinderAPT_VisibleChanged(object sender, EventArgs e)
        {
            tmUpdate.Enabled = this.Visible;
        }

        private void FrameCylinderAPT_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmUpdate.Enabled = false;
        }

        public void SetSize()
        {
            //btAction.Top    = lbTitle.Top + lbTitle.Height + 1;
            //btAction.Height = this.Height - btAction.Top;
            //btAction.Width  = this.Width;
            //btAction.Left   = 0;
        }

        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            tmUpdate.Enabled = false;

            bool bCmd  = ML.CL_GetCmd  (m_iActrId) == 0 ? true : false;
            bool bErr  = ML.CL_Err     (m_iActrId);
            bool bDone = ML.CL_Complete(m_iActrId);
            bool bDoneFwd = ML.CL_Complete(m_iActrId, fb.Fwd);
            bool bDoneBwd = ML.CL_Complete(m_iActrId, fb.Bwd);

            //if (bDone)
            //{
            //    lbFwd.BackColor = bCmd ? Color.ForestGreen    : SystemColors.Control;
            //    lbBwd.BackColor = bCmd ? SystemColors.Control : Color.ForestGreen   ;
            //}
            if (bErr)
            {
                btBwd.BackColor = Color.Red;
                btFwd.BackColor = Color.Red;
            }
            

            if (bCmd != m_bPreCmd)
            {
                if ((int)ML.CL_GetCmd(m_iActrId) == 0) { btBwd.BackColor = Color.ForestGreen; btFwd.BackColor = SystemColors.Control; }
                else                                   { btFwd.BackColor = Color.ForestGreen; btBwd.BackColor = SystemColors.Control; }
            }

            m_bPreCmd = bCmd;

            tmUpdate.Enabled = true;
        }

        private void btAction_Click(object sender, EventArgs e)
        {
            //fb sCylderPos = 0;
            //
            //if (ML.CL_GetCmd(m_iActrId) == 0) sCylderPos = fb.Fwd;
            //else                              sCylderPos = fb.Bwd;
            //
            //ML.CL_Move(m_iActrId, sCylderPos);
            //
            //string sMsg = "FrameCyl Form_Cylinder Move Button Click" + ML.CL_GetName(m_iActrId).ToString();
            //sMsg += ML.CL_GetCmd(m_iActrId) == 0 ? "(Fwd)" : "(Bwd)";
            //
            //Log.Trace("Operator", sMsg);
        }
        
        private void btRepeat_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(m_sActrName + "_" + sText + " Button Clicked", ForContext.Frm);

            int iSelNo = Convert.ToInt32(lbCylNo.Text);
            int iRptTm = CConfig.StrToIntDef(textBox1.Text);
            if (lbCylNo.Text != "" && !bActivate[iSelNo])
            {
                ML.CL_GoRpt(iRptTm, iSelNo);
                bActivate[iSelNo] = true;
            }
            else if(bActivate[iSelNo])
            {
                ML.CL_StopRpt();
                bActivate[iSelNo] = false;
            }
        }

        private void evFwd_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(m_sActrName + "_" + sText + " Button Clicked",ForContext.Frm);
            if (SEQ.LDR.CheckSafe(m_iActrId, fb.Fwd , true)) ML.CL_Move(m_iActrId, fb.Fwd);
        }

        private void evBwd_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;

            Log.Trace(m_sActrName + "_" + sText + " Button Clicked", ForContext.Frm);

            if (SEQ.LDR.CheckSafe(m_iActrId, fb.Bwd , true)) ML.CL_Move(m_iActrId, fb.Bwd);
        }

        private void btFwd_Click(object sender, EventArgs e)
        {

        }
    }
}
