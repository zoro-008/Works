using System;
using System.Drawing;
using System.Windows.Forms;
using COMMON;
using SML;

namespace Machine
{
    public partial class FraCylOneBt : Form
    {
        public FraCylOneBt()
        {
            InitializeComponent();

            this.Width = 103;
            this.Height = 127;
        }

        public ci         m_iActrId;
        public EN_MOVE_DIRECTION  m_iType     ;
        public bool               m_bPreCmd   ;
        public string             m_sCaptionFw;
        public string             m_sCaptionBw;
        public dgCheckSafe        m_CheckSafe;

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

        private void FraCylOneBt_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible) tmUpdate.Enabled = true;
        }

        private void FraCylOneBt_Shown(object sender, EventArgs e)
        {
            tmUpdate.Enabled = true;
        }

        public void SetConfig(ci _iActrId, string _sTitle, EN_MOVE_DIRECTION _iActrType, Control _wcParent /*, dgCheckSafe _CheckSafe*/)
        {
            string sActrName; 
            sActrName = _sTitle;
            if(sActrName == null) return;
            sActrName = sActrName.Replace("_", "");

            
        
            m_iActrId = _iActrId;
            lbTitle.Text = sActrName;
            m_iType = _iActrType;
            this.Parent  = _wcParent;
            //m_CheckSafe = _CheckSafe;
        
            m_bPreCmd = true;
            //btAction.ImageIndex = 1;
            //tmUpdate.Enabled = true;



            switch (m_iType)
            {
                default: iFwd = Right; sFwd = "RIGHT"; iBwd = Left; sBwd = "LEFT"; break;

                case EN_MOVE_DIRECTION.LR: iFwd = Right ; sFwd = "RIGHT" ; 
                                             iBwd = Left  ; sBwd = "LEFT"  ; break;
                case EN_MOVE_DIRECTION.RL: iFwd = Left  ; sFwd = "LEFT"  ;
                                             iBwd = Right ; sBwd = "RIGHT" ; break;
                case EN_MOVE_DIRECTION.BF: iFwd = Left  ; sFwd = "Fwd"   ;
                                             iBwd = Right ; sBwd = "Bwd"   ; break;
                case EN_MOVE_DIRECTION.FB: iFwd = Right ; sFwd = "Bwd"   ;
                                             iBwd = Left  ; sBwd = "Fwd"   ; break;
                case EN_MOVE_DIRECTION.UD: iFwd = Down  ; sFwd = "DN"    ;
                                             iBwd = Up    ; sBwd = "Up"    ; break;
                case EN_MOVE_DIRECTION.DU: iFwd = Up    ; sFwd = "UP"    ;
                                             iBwd = Down  ; sBwd = "DN"    ; break;
                case EN_MOVE_DIRECTION.CA: iFwd = CCW   ; sFwd = "CCW"   ;
                                             iBwd = CW    ; sBwd = "CW"    ; break;
                case EN_MOVE_DIRECTION.AC: iFwd = CW    ; sFwd = "CW"    ;
                                             iBwd = CCW   ; sBwd = "CCW"   ; break;
            }

            if ((int)ML.CL_GetCmd(m_iActrId) == 0) { btAction.ImageIndex = iFwd; btAction.Text = sFwd; }
            else                                   { btAction.ImageIndex = iBwd; btAction.Text = sBwd; }


            //btAction.ImageIndex = Right;
            //ImgLstBt.Images.SetKeyName(1, "Right");
        
        
        
        
        
            
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

            bool bCmd  = ML.CL_GetCmd  (m_iActrId) == 0 ? true : false;
            bool bErr  = ML.CL_Err     (m_iActrId);
            bool bDone = ML.CL_Complete(m_iActrId);

                 if (bDone) btAction.ForeColor = Color.Lime;
            else if (bErr)  btAction.ForeColor = Color.Red;
            else            btAction.ForeColor = Color.Black;

            if (bCmd != m_bPreCmd)
            {
                if ((int)ML.CL_GetCmd(m_iActrId) == 0) { btAction.ImageIndex = iFwd; btAction.Text = sFwd; }
                else                                   { btAction.ImageIndex = iBwd; btAction.Text = sBwd; }
            }

            m_bPreCmd = bCmd;

            if (!this.Visible)
            {
                tmUpdate.Enabled = false;
                return;
            }
            tmUpdate.Enabled = true;
        }

        private void btAction_Click(object sender, EventArgs e)
        {
            fb sCylderPos = 0;

            if (ML.CL_GetCmd(m_iActrId) == 0) sCylderPos = fb.Fwd;
            else                              sCylderPos = fb.Bwd;

            ML.CL_Move(m_iActrId, sCylderPos);

            string sMsg = "FrameCyl Form_Cylinder Move Button Click" + ML.CL_GetName(m_iActrId).ToString();
            sMsg += ML.CL_GetCmd(m_iActrId) == 0 ? "(Fwd)" : "(Bwd)";

            //Log.Trace("Operator", sMsg);
            Log.Trace(sMsg,ForContext.Frm);
        }

        private void FraCylOneBt_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmUpdate.Enabled = false;
        }
    }
}
