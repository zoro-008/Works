using System;
using System.Drawing;
using System.Windows.Forms;
using COMMON;
using SML;

namespace Machine
{
    public enum EN_UNIT_TYPE
    {
        utJog = 0,
        utMove,
        utPitch,

        MAX_UNIT_TYPE
    };

    public partial class FraMotr : Form
    {
        public FraMotr()
        {
            InitializeComponent();

            //this.Width = 254;
            //this.Height = 143;

            tmUpdate.Enabled = false;

        }

        public struct TPara
        {
            public double dUnit;
            public int iUnitType;
            public double dPitch;
            
        }

        public mi m_eId;
        public int m_iType;

        public TPara Para;






        public void SetIdType(mi _eId, MOTION_DIR _iType)
        {
            lbName.Text = ML.MT_GetName(_eId);

            m_eId = _eId;
            m_iType = (int)_iType;

            if (_iType == MOTION_DIR.LeftRight)
            {
                btNeg.Image = global::Machine.Properties.Resources.LEFT;
                btPos.Image = global::Machine.Properties.Resources.RIGHT;
                //ImgLstBt.Images.SetKeyName(0, "left");
                //ImgLstBt.Images.SetKeyName(1, "Right");
                //
                //btNeg.ImageIndex = 0;
                //btPos.ImageIndex = 0;
                btNeg.Text = "LEFT(-)";
                btPos.Text = "RIGHT(+)";
            }

            if (_iType == MOTION_DIR.RightLeft)
            {
                btNeg.Image = global::Machine.Properties.Resources.RIGHT;
                btPos.Image = global::Machine.Properties.Resources.LEFT ;
                btNeg.Text = "RIGHT(-)";
                btPos.Text = "LEFT(+)";
            }

            if (_iType == MOTION_DIR.BwdFwd)
            {
                btNeg.Image = global::Machine.Properties.Resources.DN;
                btPos.Image = global::Machine.Properties.Resources.UP;
                btNeg.Text = "BWD(-)";
                btPos.Text = "FWD(+)";
            }

            if (_iType == MOTION_DIR.FwdBwd)
            {
                btNeg.Image = global::Machine.Properties.Resources.UP;
                btPos.Image = global::Machine.Properties.Resources.DN;
                btNeg.Text = "FWD(-)";
                btPos.Text = "BWD(+)";
            }

            if (_iType == MOTION_DIR.DownUp)
            {
                btNeg.Image = global::Machine.Properties.Resources.DN;
                btPos.Image = global::Machine.Properties.Resources.UP;
                btNeg.Text = "DN(-)";
                btPos.Text = "UP(+)";
            }

            if (_iType == MOTION_DIR.UpDown)
            {
                btNeg.Image = global::Machine.Properties.Resources.UP;
                btPos.Image = global::Machine.Properties.Resources.DN;
                btNeg.Text = "UP(-)";
                btPos.Text = "DN(+)";
            }

            if (_iType == MOTION_DIR.CcwCw)
            {
                btNeg.Image = global::Machine.Properties.Resources.CCW;
                btPos.Image = global::Machine.Properties.Resources.CW;
                btNeg.Text = "CCW(-)";
                btPos.Text = "CW(+)";
            }

            if (_iType == MOTION_DIR.CwCcw)
            {
                btNeg.Image = global::Machine.Properties.Resources.CW;
                btPos.Image = global::Machine.Properties.Resources.CCW;
                btNeg.Text = "CW(-)";
                btPos.Text = "CCW(+)";
            }

            //tmUpdate.Enabled = true;
        }

        public void SetPitch(double _dUnit)
        {
            Para.dPitch = _dUnit;
        }

        public void SetUnit(EN_UNIT_TYPE _iUnitType, double _dUnit)
        {
            Para.dUnit = _dUnit;
            Para.iUnitType = (int)_iUnitType;
        }

        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            tmUpdate.Enabled = false;

            LbStat1.BackColor = ML.MT_GetNLimSnsr  (m_eId) ? Color.Lime : Color.Silver;
            LbStat2.BackColor = ML.MT_GetHomeSnsr  (m_eId) ? Color.Lime : Color.Silver;
            LbStat3.BackColor = ML.MT_GetPLimSnsr  (m_eId) ? Color.Lime : Color.Silver;
            LbStat4.BackColor = ML.MT_GetAlarmSgnl (m_eId) ? Color.Lime : Color.Silver;
            LbStat5.BackColor = ML.MT_GetServo     (m_eId) ? Color.Lime : Color.Silver;
            LbStat6.BackColor = ML.MT_GetStop      (m_eId) ? Color.Lime : Color.Silver;
            LbStat7.BackColor = ML.MT_GetHomeDone  (m_eId) ? Color.Lime : Color.Silver;

            LbCmdPos.Text = ML.MT_GetCmdPos(m_eId).ToString();
            LbEncPos.Text = ML.MT_GetEncPos(m_eId).ToString();

            if (SEQ._bRun)
            {
                btNeg.Enabled = false;
                btPos.Enabled = false;
            }

            else
            {
                btNeg.Enabled = true;
                btPos.Enabled = true;
            }


            tmUpdate.Enabled = true;
        }

        private void btPos_MouseUp(object sender, MouseEventArgs e)
        {
            if (Para.iUnitType == (int)EN_UNIT_TYPE.utJog) ML.MT_Stop(m_eId);
        }

        private void btNeg_MouseUp(object sender, MouseEventArgs e)
        {
            if (Para.iUnitType == (int)EN_UNIT_TYPE.utJog) ML.MT_Stop(m_eId);
        }

        private void btNeg_MouseDown(object sender, MouseEventArgs e)
        {
            string sTemp;
            string sText = ((Button)sender).Text;
            bool bRet = true;
            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);
            uint uPstnNo = (uint)iBtnTag;
            //if (!CheckSafe(m_iMotrNo)) return;
            if (!OM.MstOptn.bDebugMode)
            {
                //if (m_eId == mi.PRER_X) bRet = SEQ.PRER.JogCheckSafe((mi)m_eId, EN_JOG_DIRECTION.Neg, (EN_UNIT_TYPE)Para.iUnitType, Para.dUnit);
                //if (m_eId == mi.PSTR_X) bRet = SEQ.PSTR.JogCheckSafe((mi)m_eId, EN_JOG_DIRECTION.Neg, (EN_UNIT_TYPE)Para.iUnitType, Para.dUnit);
                //if (m_eId == mi.LDR_X) bRet = SEQ.LDR.JogCheckSafe((mi)m_eId, EN_JOG_DIRECTION.Neg, (EN_UNIT_TYPE)Para.iUnitType, Para.dUnit);
                //if (m_eId == mi.RPCK_Y) bRet = SEQ.PCKR.JogCheckSafe((mi)m_eId, EN_JOG_DIRECTION.Neg, (EN_UNIT_TYPE)Para.iUnitType, Para.dUnit);
                //if (m_eId == mi.VSTG_X) bRet = SEQ.VSTG.JogCheckSafe((mi)m_eId, EN_JOG_DIRECTION.Neg, (EN_UNIT_TYPE)Para.iUnitType, Para.dUnit);
            }
            
            
           
            
            
            

            if (!bRet) return;

            ML.MT_Stop(m_eId);

            sTemp = m_eId.ToString();
            
            if (SEQ._iSeqStat == EN_SEQ_STAT.Manual)
            {
                Log.ShowMessage("ERROR", "Doing Manual Cycle"); 
                return;
            }
            else
            {
                Log.Trace("FrameMotr Form_" + sTemp + " " + sText + " Button Clicked",ForContext.Frm);

                if (!cbSlow.Checked)
                {
                         if (Para.iUnitType == (int)EN_UNIT_TYPE.utJog  ) ML.MT_JogN    (m_eId);
                    else if (Para.iUnitType == (int)EN_UNIT_TYPE.utMove ) ML.MT_GoIncMan(m_eId, -(Para.dUnit));
                    else if (Para.iUnitType == (int)EN_UNIT_TYPE.utPitch) ML.MT_GoIncMan(m_eId, -(Para.dUnit));
                    else return;
                }
                else
                {
                         if (Para.iUnitType == (int)EN_UNIT_TYPE.utJog  ) ML.MT_JogN     (m_eId);
                    else if (Para.iUnitType == (int)EN_UNIT_TYPE.utMove ) ML.MT_GoIncSlow(m_eId, -(Para.dUnit));
                    else if (Para.iUnitType == (int)EN_UNIT_TYPE.utPitch) ML.MT_GoIncSlow(m_eId, -(Para.dUnit));
                    else return;
                }
            }
            
        }

        private void btPos_MouseDown(object sender, MouseEventArgs e)
        {
            string sTemp;
            string sText = ((Button)sender).Text;
            bool bRet = true;
            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);
            uint uPstnNo = (uint)iBtnTag;
            //if (!CheckSafe(m_iMotrNo)) return;

            if (!OM.MstOptn.bDebugMode)
            {
                //if (m_eId == mi.PRER_X) bRet = SEQ.PRER.JogCheckSafe((mi)m_eId, EN_JOG_DIRECTION.Pos, (EN_UNIT_TYPE)Para.iUnitType, Para.dUnit);
                //if (m_eId == mi.PSTR_X) bRet = SEQ.PSTR.JogCheckSafe((mi)m_eId, EN_JOG_DIRECTION.Pos, (EN_UNIT_TYPE)Para.iUnitType, Para.dUnit);
                //if (m_eId == mi.LDR_X ) bRet = SEQ.LDR.JogCheckSafe((mi)m_eId, EN_JOG_DIRECTION.Pos, (EN_UNIT_TYPE)Para.iUnitType, Para.dUnit);
                //if (m_eId == mi.RPCK_Y) bRet = SEQ.PCKR.JogCheckSafe((mi)m_eId, EN_JOG_DIRECTION.Pos, (EN_UNIT_TYPE)Para.iUnitType, Para.dUnit);
                //if (m_eId == mi.VSTG_X) bRet = SEQ.VSTG.JogCheckSafe((mi)m_eId, EN_JOG_DIRECTION.Pos, (EN_UNIT_TYPE)Para.iUnitType, Para.dUnit);

            }
            

            


            if (!bRet) return;
            ML.MT_Stop(m_eId);

            

            sTemp = m_eId.ToString();

            if (SEQ._iSeqStat == EN_SEQ_STAT.Manual)
            {
                Log.ShowMessage("ERROR", "Doing Manual Cycle"); 
                return;
            }
            else
            {
                Log.Trace("FrameMotr Form_" + sTemp + " " + sText + " Button Clicked",ForContext.Frm);

                if (!cbSlow.Checked)
                {
                         if (Para.iUnitType == (int)EN_UNIT_TYPE.utJog  ) ML.MT_JogP    (m_eId);
                    else if (Para.iUnitType == (int)EN_UNIT_TYPE.utMove ) ML.MT_GoIncMan(m_eId, Para.dUnit);
                    else if (Para.iUnitType == (int)EN_UNIT_TYPE.utPitch) ML.MT_GoIncMan(m_eId, Para.dUnit);
                    else return;
                }
                else
                {
                         if (Para.iUnitType == (int)EN_UNIT_TYPE.utJog  ) ML.MT_JogP     (m_eId);
                    else if (Para.iUnitType == (int)EN_UNIT_TYPE.utMove ) ML.MT_GoIncSlow(m_eId, Para.dUnit);
                    else if (Para.iUnitType == (int)EN_UNIT_TYPE.utPitch) ML.MT_GoIncSlow(m_eId, Para.dUnit);
                    else return;
                }
            }
        }


        private void cbSlow_Click(object sender, EventArgs e)
        {
            if (cbSlow.Checked)
            {
                Log.ShowMessageFunc(string.Format("Relative movement value is decelerated to 10%.", m_eId));
            }
            else
            {
                Log.ShowMessageFunc(string.Format("The relative shift value changes to Manual Speed.", m_eId));
            }
        }

        private void btStop_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace("FrameMotr Form_" + m_eId.ToString() + " Stop Button Clicked", ForContext.Frm);
            ML.MT_Stop(m_eId);
        }

        private void FraMotr_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmUpdate.Enabled = false;
        }

        private void FraMotr_Shown(object sender, EventArgs e)
        {
            tmUpdate.Enabled = true;
        }

        private void FraMotr_VisibleChanged(object sender, EventArgs e)
        {
            tmUpdate.Enabled = this.Visible;
        }

        private void btNeg_Click(object sender, EventArgs e)
        {

        }
    }
}


 