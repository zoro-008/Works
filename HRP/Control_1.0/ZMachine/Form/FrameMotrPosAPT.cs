using System;
using System.Drawing;
using COMMON;
using System.Windows.Forms;
using SML;

namespace Machine
{
    public partial class FrameMotrPosAPT : Form
    {
        //FormMain FrmMain;
        public FrameMotrPosAPT()
        {
            InitializeComponent();

            //ML.PM_SetGetCmdPos(mi.LODR_ZLift);
            //ML.PM_SetGetCmdPos(mi.TOOL_XRjct);
            //ML.PM_SetGetCmdPos(mi.IDXR_XRear);
            //ML.PM_SetGetCmdPos(mi.IDXF_XFrnt);
            //ML.PM_SetGetCmdPos(mi.TOOL_YTool);
            //ML.PM_SetGetCmdPos(mi.TOOL_ZPckr);
            //ML.PM_SetGetCmdPos(mi.BARZ_XPckr);
            //ML.PM_SetGetCmdPos(mi.BARZ_ZPckr);
            //ML.PM_SetGetCmdPos(mi.STCK_ZStck);
            //ML.PM_SetGetCmdPos(mi.TOOL_ZVisn);

            //PM.Load(OM.GetCrntDev().ToString());

            //PM.UpdatePstn(true);
            tmUpdate.Enabled = false; 
          
        }

        public mi m_eId;

        //폼 가져다 붙이는 함수
        public void SetWindow(int _Idx, Control _wcParent )
        {
            m_eId = (mi)_Idx;
            lbAxisNo.Text = "Axis " + _Idx.ToString();
            lbAxisName.Text = "[" + _Idx + "] " + ML.MT_GetName(_Idx);
            PM.SetWindow(pnMotr0, _Idx);
            //ML.PM_SetGetCmdPos(m_eId);

            //switch(_iPageIdx)
            //{
            //    case 0: m_eId = (mi)_iPageIdx;
            //            lbAxisNo.Text = "Axis " + _iPageIdx.ToString();
            //            lbAxisName.Text = "[" + _iPageIdx + "] " + ML.MT_GetName((mi)_iPageIdx);
            //            PM.SetWindow(pnMotr0, _iPageIdx); break;

            //    case 1: m_eId = (mi)_iPageIdx;
            //            lbAxisNo.Text = "Axis " + _iPageIdx.ToString();
            //            lbAxisName.Text = "[" + _iPageIdx + "] " + ML.MT_GetName((mi)_iPageIdx);
            //            PM.SetWindow(pnMotr0, _iPageIdx); break;

            //    case 2: m_eId = (mi)_iPageIdx;
            //            lbAxisNo.Text = "Axis " + _iPageIdx.ToString();
            //            lbAxisName.Text = "[" + _iPageIdx + "] " + ML.MT_GetName((mi)_iPageIdx);
            //            PM.SetWindow(pnMotr0, _iPageIdx); break;

            //    case 3: m_eId = (mi)_iPageIdx;
            //            lbAxisNo.Text = "Axis " + _iPageIdx.ToString();
            //            lbAxisName.Text = "[" + _iPageIdx + "] " + ML.MT_GetName((mi)_iPageIdx);
            //            PM.SetWindow(pnMotr0, _iPageIdx); break;

            //    case 4: m_eId = (mi)_iPageIdx;
            //            lbAxisNo.Text = "Axis " + _iPageIdx.ToString();
            //            lbAxisName.Text = "[" + _iPageIdx + "] " + ML.MT_GetName((mi)_iPageIdx);
            //            PM.SetWindow(pnMotr0, _iPageIdx); break;

            //    case 5: m_eId = (mi)_iPageIdx;
            //            lbAxisNo.Text = "Axis " + _iPageIdx.ToString();
            //            lbAxisName.Text = "[" + _iPageIdx + "] " + ML.MT_GetName((mi)_iPageIdx);
            //            PM.SetWindow(pnMotr0, _iPageIdx); break;

            //    case 6: m_eId = (mi)_iPageIdx;
            //            lbAxisNo.Text = "Axis " + _iPageIdx.ToString();
            //            lbAxisName.Text = "[" + _iPageIdx + "] " + ML.MT_GetName((mi)_iPageIdx);
            //            PM.SetWindow(pnMotr0, _iPageIdx); break;

            //    case 7: m_eId = (mi)_iPageIdx;
            //            lbAxisNo.Text = "Axis " + _iPageIdx.ToString();
            //            lbAxisName.Text = "[" + _iPageIdx + "] " + ML.MT_GetName((mi)_iPageIdx);
            //            PM.SetWindow(pnMotr0, _iPageIdx); break;

            //    case 8: m_eId = (mi)_iPageIdx;
            //            lbAxisNo.Text = "Axis " + _iPageIdx.ToString();
            //            lbAxisName.Text = "[" + _iPageIdx + "] " + ML.MT_GetName((mi)_iPageIdx);
            //            PM.SetWindow(pnMotr0, _iPageIdx); break;

            //    case 9: m_eId = (mi)_iPageIdx;
            //            lbAxisNo.Text = "Axis " + _iPageIdx.ToString();
            //            lbAxisName.Text = "[" + _iPageIdx + "] " + ML.MT_GetName((mi)_iPageIdx);
            //            PM.SetWindow(pnMotr0, _iPageIdx); break;
            //}

            this.Parent = _wcParent;
            //tmUpdate.Enabled = true;
        }

        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            string sTemp ; ;
            tmUpdate.Enabled = false;

            lbStat1.ForeColor = ML.MT_GetNLimSnsr  (m_eId) ? Color.Red : Color.Silver;
            lbStat2.ForeColor = ML.MT_GetHomeSnsr  (m_eId) ? Color.Red : Color.Silver;
            lbStat3.ForeColor = ML.MT_GetPLimSnsr  (m_eId) ? Color.Red : Color.Silver;
            lbStat4.ForeColor = ML.MT_GetHomeDone  (m_eId) ? Color.Red : Color.Silver;
            lbStat5.ForeColor = ML.MT_GetAlarmSgnl (m_eId) ? Color.Red : Color.Silver;
            lbStat6.ForeColor = ML.MT_GetStop      (m_eId) ? Color.Red : Color.Silver;
            lbStat7.ForeColor = ML.MT_GetInPosSgnl (m_eId) ? Color.Red : Color.Silver;  
            lbStat8.ForeColor = ML.MT_GetServo     (m_eId) ? Color.Red : Color.Silver;
            
            sTemp = string.Format("{0:0.0000}", ML.MT_GetCmdPos(m_eId));
            lbCmdPos.Text     = sTemp;

            if (!this.Visible)
            {
                tmUpdate.Enabled = false;
                return;
            }
            tmUpdate.Enabled = true;
        }

        private void btServoOn_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(m_eId.ToString() + " Axis " + sText + " Button Clicked", ti.Frm);
            ML.MT_SetServo(m_eId, true);
        }

        private void btServoOff_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(m_eId.ToString() + " Axis " + sText + " Button Clicked", ti.Frm);

            ML.MT_SetServo(m_eId, false);
        }

        private void btHome_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(m_eId.ToString() + " Axis " + sText + " Button Clicked", ti.Frm);

            if (Log.ShowMessageModal(m_eId.ToString() + " Axis" , "Do you want to Axis Homing?")!= System.Windows.Forms.DialogResult.Yes) return ;
            ML.MT_GoHome(m_eId);
        }

        private void FrameMotrPosAPT_Shown(object sender, EventArgs e)
        {
            tmUpdate.Enabled = true;
        }

        private void FrameMotrPosAPT_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible) tmUpdate.Enabled = true;
        }




        //private void FraMotr_FormClosing(object sender, FormClosingEventArgs e)
        //{
        //    tmUpdate.Enabled = false;
        //}
    }
}
