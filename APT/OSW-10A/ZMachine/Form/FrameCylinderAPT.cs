using COMMON;
using SML2;
using System;
using System.Drawing;
using System.Windows.Forms;

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
                         CCW   = 7;

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
        
            m_bPreCmd = true;
            tmUpdate.Enabled = true;

            switch (m_iType)
            {
                default: iFwd = Left; sFwd = "LEFT"; iBwd = Right; sBwd = "RIGHT"; break;

                case EN_MOVE_DIRECTION.LR: sFwd = "RIGHT" ; 
                                           sBwd = "LEFT"  ; break;
                case EN_MOVE_DIRECTION.RL: sFwd = "LEFT"  ;
                                           sBwd = "RIGHT" ; break;
                case EN_MOVE_DIRECTION.BF: sFwd = "Fwd"   ;
                                           sBwd = "Bwd"   ; break;
                case EN_MOVE_DIRECTION.FB: sFwd = "Bwd"   ;
                                           sBwd = "Fwd"   ; break;
                case EN_MOVE_DIRECTION.UD: sFwd = "DN"    ;
                                           sBwd = "Up"    ; break;
                case EN_MOVE_DIRECTION.DU: sFwd = "UP"    ;
                                           sBwd = "DN"    ; break;
                case EN_MOVE_DIRECTION.CA: sFwd = "CCW"   ;
                                           sBwd = "CW"    ; break;
                case EN_MOVE_DIRECTION.AC: sFwd = "CW"    ;
                                           sBwd = "CCW"   ; break;
                case EN_MOVE_DIRECTION.CO: sFwd = "OPEN"  ; 
                                           sBwd = "CLOSE" ; break;
                case EN_MOVE_DIRECTION.OC: sFwd = "CLOSE" ; 
                                           sBwd = "OPEN"  ; break;
            }

            lbBwd.BackColor = Color.ForestGreen; 
            lbFwd.BackColor = SystemColors.Control;

            if ((int)SM.CL_GetCmd(m_iActrId) == 0) { btFwd.Text = sFwd; btBwd.Text = sBwd; }
            else                                   { btBwd.Text = sBwd; btFwd.Text = sFwd; }
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

            bool bCmd  = SM.CL_GetCmd  (m_iActrId) == fb.Bwd ? true : false;
            bool bErr  = SM.CL_Err     (m_iActrId);
            bool bDone = SM.CL_Complete(m_iActrId);
            bool bDoneFwd = SM.CL_Complete(m_iActrId, fb.Fwd);
            bool bDoneBwd = SM.CL_Complete(m_iActrId, fb.Bwd);

            //if (bDone)
            //{
            //    lbFwd.BackColor = bCmd ? Color.ForestGreen    : SystemColors.Control;
            //    lbBwd.BackColor = bCmd ? SystemColors.Control : Color.ForestGreen   ;
            //}
            if (bErr)
            {
                lbFwd.BackColor = Color.Red;
                lbBwd.BackColor = Color.Red;
            }
            

            if (bCmd != m_bPreCmd)
            {
                if (SM.CL_GetCmd(m_iActrId) == fb.Bwd) { lbBwd.BackColor = Color.ForestGreen; lbFwd.BackColor = SystemColors.Control; }
                else                                   { lbFwd.BackColor = Color.ForestGreen; lbBwd.BackColor = SystemColors.Control; }
            }

            m_bPreCmd = bCmd;
            tmUpdate.Enabled = true;
        }

        private void btAction_Click(object sender, EventArgs e)
        {
            //fb sCylderPos = 0;
            //
            //if (SM.CL_GetCmd(m_iActrId) == 0) sCylderPos = fb.Fwd;
            //else                              sCylderPos = fb.Bwd;
            //
            //SM.CL_Move(m_iActrId, sCylderPos);
            //
            //string sMsg = "FrameCyl Form_Cylinder Move Button Click" + SM.CL_GetName(m_iActrId).ToString();
            //sMsg += SM.CL_GetCmd(m_iActrId) == 0 ? "(Fwd)" : "(Bwd)";
            //
            //Log.Trace("Operator", sMsg);
        }

        private void FraCylOneBt_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmUpdate.Enabled = false;
        }

        
        private void btRepeat_Click(object sender, EventArgs e)
        {
            
            int iSelNo = Convert.ToInt32(lbCylNo.Text);

            if (lbCylNo.Text != "" && !bActivate[iSelNo])
            {
                SML.CL.GoRpt(1000, iSelNo);
                bActivate[iSelNo] = true;
            }
            else if(bActivate[iSelNo])
            {
                SML.CL.StopRpt();
                bActivate[iSelNo] = false;
            }
        }

        private void btFwd_Click(object sender, EventArgs e)
        {
            Log.Trace(m_sActrName, "Fwd Clicked");

            if(m_iActrId == ci.LODR_ClampClOp      ){SEQ.LODR.MoveCyl(m_iActrId ,SM.CL_GetCmd(m_iActrId) == fb.Bwd ? fb.Fwd : fb.Bwd);}
            if(m_iActrId == ci.LODR_SperatorUpDn   ){SEQ.LODR.MoveCyl(m_iActrId ,SM.CL_GetCmd(m_iActrId) == fb.Bwd ? fb.Fwd : fb.Bwd);}
            if(m_iActrId == ci.STCK_RailClOp       ){SEQ.STCK.MoveCyl(m_iActrId ,SM.CL_GetCmd(m_iActrId) == fb.Bwd ? fb.Fwd : fb.Bwd);}
            if(m_iActrId == ci.IDXR_ClampUpDn      ){SEQ.IDXR.MoveCyl(m_iActrId ,SM.CL_GetCmd(m_iActrId) == fb.Bwd ? fb.Fwd : fb.Bwd);}
            if(m_iActrId == ci.IDXF_ClampUpDn      ){SEQ.IDXF.MoveCyl(m_iActrId ,SM.CL_GetCmd(m_iActrId) == fb.Bwd ? fb.Fwd : fb.Bwd);}
            if(m_iActrId == ci.IDXR_ClampClOp      ){SEQ.IDXR.MoveCyl(m_iActrId ,SM.CL_GetCmd(m_iActrId) == fb.Bwd ? fb.Fwd : fb.Bwd);}
            if(m_iActrId == ci.IDXF_ClampClOp      ){SEQ.IDXF.MoveCyl(m_iActrId ,SM.CL_GetCmd(m_iActrId) == fb.Bwd ? fb.Fwd : fb.Bwd);}
            if(m_iActrId == ci.STCK_RailTrayUpDn   ){SEQ.STCK.MoveCyl(m_iActrId ,SM.CL_GetCmd(m_iActrId) == fb.Bwd ? fb.Fwd : fb.Bwd);}
            if(m_iActrId == ci.STCK_StackStprUpDn  ){SEQ.STCK.MoveCyl(m_iActrId ,SM.CL_GetCmd(m_iActrId) == fb.Bwd ? fb.Fwd : fb.Bwd);}
            if(m_iActrId == ci.STCK_StackOpCl      ){SEQ.STCK.MoveCyl(m_iActrId ,SM.CL_GetCmd(m_iActrId) == fb.Bwd ? fb.Fwd : fb.Bwd);}
            if(m_iActrId == ci.BARZ_BrcdStprUpDn   ){SEQ.BARZ.MoveCyl(m_iActrId ,SM.CL_GetCmd(m_iActrId) == fb.Bwd ? fb.Fwd : fb.Bwd);}
            if(m_iActrId == ci.BARZ_BrcdTrayUpDn   ){SEQ.BARZ.MoveCyl(m_iActrId ,SM.CL_GetCmd(m_iActrId) == fb.Bwd ? fb.Fwd : fb.Bwd);}
            if(m_iActrId == ci.BARZ_YPckrFwBw      ){SEQ.BARZ.MoveCyl(m_iActrId ,SM.CL_GetCmd(m_iActrId) == fb.Bwd ? fb.Fwd : fb.Bwd);}


            //SM.CL_Move(m_iActrId, fb.Fwd);
        }

        private void btBwd_Click(object sender, EventArgs e)
        {
            Log.Trace(m_sActrName, "Fwd Clicked");
            if(m_iActrId == ci.LODR_ClampClOp      ){SEQ.LODR.MoveCyl(m_iActrId ,SM.CL_GetCmd(m_iActrId) == fb.Bwd ? fb.Fwd : fb.Bwd);}
            if(m_iActrId == ci.LODR_SperatorUpDn   ){SEQ.LODR.MoveCyl(m_iActrId ,SM.CL_GetCmd(m_iActrId) == fb.Bwd ? fb.Fwd : fb.Bwd);}
            if(m_iActrId == ci.STCK_RailClOp       ){SEQ.STCK.MoveCyl(m_iActrId ,SM.CL_GetCmd(m_iActrId) == fb.Bwd ? fb.Fwd : fb.Bwd);}
            if(m_iActrId == ci.IDXR_ClampUpDn      ){SEQ.IDXR.MoveCyl(m_iActrId ,SM.CL_GetCmd(m_iActrId) == fb.Bwd ? fb.Fwd : fb.Bwd);}
            if(m_iActrId == ci.IDXF_ClampUpDn      ){SEQ.IDXF.MoveCyl(m_iActrId ,SM.CL_GetCmd(m_iActrId) == fb.Bwd ? fb.Fwd : fb.Bwd);}
            if(m_iActrId == ci.IDXR_ClampClOp      ){SEQ.IDXR.MoveCyl(m_iActrId ,SM.CL_GetCmd(m_iActrId) == fb.Bwd ? fb.Fwd : fb.Bwd);}
            if(m_iActrId == ci.IDXF_ClampClOp      ){SEQ.IDXF.MoveCyl(m_iActrId ,SM.CL_GetCmd(m_iActrId) == fb.Bwd ? fb.Fwd : fb.Bwd);}
            if(m_iActrId == ci.STCK_RailTrayUpDn   ){SEQ.STCK.MoveCyl(m_iActrId ,SM.CL_GetCmd(m_iActrId) == fb.Bwd ? fb.Fwd : fb.Bwd);}
            if(m_iActrId == ci.STCK_StackStprUpDn  ){SEQ.STCK.MoveCyl(m_iActrId ,SM.CL_GetCmd(m_iActrId) == fb.Bwd ? fb.Fwd : fb.Bwd);}
            if(m_iActrId == ci.STCK_StackOpCl      ){SEQ.STCK.MoveCyl(m_iActrId ,SM.CL_GetCmd(m_iActrId) == fb.Bwd ? fb.Fwd : fb.Bwd);}
            if(m_iActrId == ci.BARZ_BrcdStprUpDn   ){SEQ.BARZ.MoveCyl(m_iActrId ,SM.CL_GetCmd(m_iActrId) == fb.Bwd ? fb.Fwd : fb.Bwd);}
            if(m_iActrId == ci.BARZ_BrcdTrayUpDn   ){SEQ.BARZ.MoveCyl(m_iActrId ,SM.CL_GetCmd(m_iActrId) == fb.Bwd ? fb.Fwd : fb.Bwd);}
            if(m_iActrId == ci.BARZ_YPckrFwBw      ){SEQ.BARZ.MoveCyl(m_iActrId ,SM.CL_GetCmd(m_iActrId) == fb.Bwd ? fb.Fwd : fb.Bwd);}
        }
    }
}
