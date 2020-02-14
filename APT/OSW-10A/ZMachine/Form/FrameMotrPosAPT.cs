using COMMON;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Machine
{
    public partial class FrameMotrPosAPT : Form
    {
        //FormMain FrmMain;
        public FrameMotrPosAPT()
        {
            InitializeComponent();

            SM.PM_SetGetCmdPos(mi.LODR_ZLift);
            SM.PM_SetGetCmdPos(mi.TOOL_XRjct);
            SM.PM_SetGetCmdPos(mi.IDXR_XRear);
            SM.PM_SetGetCmdPos(mi.IDXF_XFrnt);
            SM.PM_SetGetCmdPos(mi.TOOL_YTool);
            SM.PM_SetGetCmdPos(mi.TOOL_ZPckr);
            SM.PM_SetGetCmdPos(mi.BARZ_XPckr);
            SM.PM_SetGetCmdPos(mi.BARZ_ZPckr);
            SM.PM_SetGetCmdPos(mi.STCK_ZStck);
            SM.PM_SetGetCmdPos(mi.TOOL_ZVisn);

            PM.Load(OM.GetCrntDev().ToString());

            PM.UpdatePstn(true);
            tmUpdate.Enabled = false; 
          
        }

        public mi   m_eId   ;

        //폼 가져다 붙이는 함수
        public void SetWindow(int _iPageIdx, Control _wcParent)
        {
            switch(_iPageIdx)
            {
                case 0: m_eId = (mi)_iPageIdx;
                        lbAxisNo.Text = "Axis " + _iPageIdx.ToString();
                        lbAxisName.Text = "[" + _iPageIdx + "] " + SM.MT_GetName((mi)_iPageIdx);
                        PM.SetWindow(pnMotr0, _iPageIdx); break;
                
                case 1: m_eId = (mi)_iPageIdx;
                        lbAxisNo.Text = "Axis " + _iPageIdx.ToString();
                        lbAxisName.Text = "[" + _iPageIdx + "] " + SM.MT_GetName((mi)_iPageIdx);
                        PM.SetWindow(pnMotr0, _iPageIdx); break;
                
                case 2: m_eId = (mi)_iPageIdx;
                        lbAxisNo.Text = "Axis " + _iPageIdx.ToString();
                        lbAxisName.Text = "[" + _iPageIdx + "] " + SM.MT_GetName((mi)_iPageIdx);
                        PM.SetWindow(pnMotr0, _iPageIdx); break;
                
                case 3: m_eId = (mi)_iPageIdx;
                        lbAxisNo.Text = "Axis " + _iPageIdx.ToString();
                        lbAxisName.Text = "[" + _iPageIdx + "] " + SM.MT_GetName((mi)_iPageIdx);
                        PM.SetWindow(pnMotr0, _iPageIdx); break;
                
                case 4: m_eId = (mi)_iPageIdx;
                        lbAxisNo.Text = "Axis " + _iPageIdx.ToString();
                        lbAxisName.Text = "[" + _iPageIdx + "] " + SM.MT_GetName((mi)_iPageIdx);
                        PM.SetWindow(pnMotr0, _iPageIdx); break;
                
                case 5: m_eId = (mi)_iPageIdx;
                        lbAxisNo.Text = "Axis " + _iPageIdx.ToString();
                        lbAxisName.Text = "[" + _iPageIdx + "] " + SM.MT_GetName((mi)_iPageIdx);
                        PM.SetWindow(pnMotr0, _iPageIdx); break;
                
                case 6: m_eId = (mi)_iPageIdx;
                        lbAxisNo.Text = "Axis " + _iPageIdx.ToString();
                        lbAxisName.Text = "[" + _iPageIdx + "] " + SM.MT_GetName((mi)_iPageIdx);
                        PM.SetWindow(pnMotr0, _iPageIdx); break;
                
                case 7: m_eId = (mi)_iPageIdx;
                        lbAxisNo.Text = "Axis " + _iPageIdx.ToString();
                        lbAxisName.Text = "[" + _iPageIdx + "] " + SM.MT_GetName((mi)_iPageIdx);
                        PM.SetWindow(pnMotr0, _iPageIdx); break;
                
                case 8: m_eId = (mi)_iPageIdx;
                        lbAxisNo.Text = "Axis " + _iPageIdx.ToString();
                        lbAxisName.Text = "[" + _iPageIdx + "] " + SM.MT_GetName((mi)_iPageIdx);
                        PM.SetWindow(pnMotr0, _iPageIdx); break;
                
                case 9: m_eId = (mi)_iPageIdx;
                        lbAxisNo.Text = "Axis " + _iPageIdx.ToString();
                        lbAxisName.Text = "[" + _iPageIdx + "] " + SM.MT_GetName((mi)_iPageIdx);
                        PM.SetWindow(pnMotr0, _iPageIdx); break;
            }
             
            this.Parent = _wcParent;
            tmUpdate.Enabled = true;
        }

        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            string sTemp ; ;
            tmUpdate.Enabled = false;

            lbStat1.ForeColor = SM.MT_GetNLimSnsr  (m_eId) ? Color.Red : Color.Silver;
            lbStat2.ForeColor = SM.MT_GetHomeSnsr  (m_eId) ? Color.Red : Color.Silver;
            lbStat3.ForeColor = SM.MT_GetPLimSnsr  (m_eId) ? Color.Red : Color.Silver;
            lbStat4.ForeColor = SM.MT_GetHomeDone  (m_eId) ? Color.Red : Color.Silver;
            lbStat5.ForeColor = SM.MT_GetAlarmSgnl (m_eId) ? Color.Red : Color.Silver;
            lbStat6.ForeColor = SM.MT_GetStop      (m_eId) ? Color.Red : Color.Silver;
            lbStat7.ForeColor = SM.MT_GetInPosSgnl (m_eId) ? Color.Red : Color.Silver;  
            lbStat8.ForeColor = SM.MT_GetServo     (m_eId) ? Color.Red : Color.Silver;
            
            sTemp = string.Format("{0:0.0000}", SM.MT_GetCmdPos(m_eId));
            lbCmdPos.Text     = sTemp;

            if (SM.MT_GetHoming(m_eId))
            {
                btHome.ForeColor = SEQ._bFlick ? Color.Lime : Color.Black ;
            }
            else
            {
                btHome.ForeColor = Color.Black;
            }
            tmUpdate.Enabled = true;
        }

        private void btServoOn_Click(object sender, EventArgs e)
        {
            SM.MT_SetServo(m_eId, true);
        }

        private void btServoOff_Click(object sender, EventArgs e)
        {
            SM.MT_SetServo(m_eId, false);
        }

        private void btHome_Click(object sender, EventArgs e)
        {
            if(Log.ShowMessageModal(m_eId.ToString() + " Axis" , "Do you want to Axis Homing?")!= System.Windows.Forms.DialogResult.Yes) return ;
            SM.MT_GoHome(m_eId);
            
        }


    
     
        //private void FraMotr_FormClosing(object sender, FormClosingEventArgs e)
        //{
        //    tmUpdate.Enabled = false;
        //}
    }
}
