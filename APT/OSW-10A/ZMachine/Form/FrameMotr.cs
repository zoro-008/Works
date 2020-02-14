using COMMON;
using SML2;
using System;
using System.Drawing;
using System.Windows.Forms;

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

            this.Width = 244;
            this.Height = 143;

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
                btPos.Image = global::Machine.Properties.Resources.LEFT;
                btNeg.Text = "RIGHT(-)";
                btPos.Text = "LEFT(+)";
            }

            if (_iType == MOTION_DIR.BwdFwd)
            {
                btNeg.Image = global::Machine.Properties.Resources.UP;
                btPos.Image = global::Machine.Properties.Resources.DN;
                btNeg.Text = "BWD(-)";
                btPos.Text = "FWD(+)";
            }

            if (_iType == MOTION_DIR.FwdBwd)
            {
                btNeg.Image = global::Machine.Properties.Resources.DN;
                btPos.Image = global::Machine.Properties.Resources.UP;
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

            tmUpdate.Enabled = true;
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

            LbStat1.BackColor = SM.MT_GetNLimSnsr  (m_eId) ? Color.Lime : Color.Silver;
            LbStat2.BackColor = SM.MT_GetHomeSnsr  (m_eId) ? Color.Lime : Color.Silver;
            LbStat3.BackColor = SM.MT_GetPLimSnsr  (m_eId) ? Color.Lime : Color.Silver;
            LbStat4.BackColor = SM.MT_GetAlarmSgnl (m_eId) ? Color.Lime : Color.Silver;
            LbStat5.BackColor = SM.MT_GetServo     (m_eId) ? Color.Lime : Color.Silver;
            LbStat6.BackColor = SM.MT_GetStop      (m_eId) ? Color.Lime : Color.Silver;
            LbStat7.BackColor = SM.MT_GetHomeDone  (m_eId) ? Color.Lime : Color.Silver;

            LbCmdPos.Text = string.Format("{0:0.0000}", SM.MT_GetCmdPos(m_eId));   
            LbEncPos.Text = string.Format("{0:0.0000}", SM.MT_GetEncPos(m_eId));   

            if (SEQ._bRun || MM.GetManNo() != mc.NoneCycle)
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
            if (Para.iUnitType == (int)EN_UNIT_TYPE.utJog) SM.MT_Stop(m_eId);
        }

        private void btNeg_MouseUp(object sender, MouseEventArgs e)
        {
            if (Para.iUnitType == (int)EN_UNIT_TYPE.utJog) SM.MT_Stop(m_eId);
        }

        private void btNeg_MouseDown(object sender, MouseEventArgs e)
        {
            string sTemp;
            bool bRet = true;
            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);
            uint uPstnNo = (uint)iBtnTag;
            //if (!CheckSafe(m_iMotrNo)) return;
            //if (m_iId == (int)mi.IDX_X) bRet = SEQ.IDX.CheckSafe((mi)m_iId, uPstnNo);
            //if (m_iId == (int)mi.PCK_Y) bRet = SEQ.PCK.CheckSafe((mi)m_iId, uPstnNo);
            //if (m_iId == (int)mi.PCK_Z) bRet = SEQ.PCK.CheckSafe((mi)m_iId, uPstnNo);

            if (!bRet) return;

            SM.MT_Stop(m_eId);

            sTemp = m_eId.ToString();

            
            if (SEQ._iSeqStat == EN_SEQ_STAT.Manual)
            {
                Log.ShowMessage("ERROR", "Doing Manual Cycle"); 
                return;
            }
            else
            {
                Log.Trace("Operator", ("FrameMotr Form_" + sTemp + "Motor Pos Move Button Click"));

                if (!cbSlow.Checked)
                {
                         if (Para.iUnitType == (int)EN_UNIT_TYPE.utJog  ) SM.MT_JogN    (m_eId);
                    else if (Para.iUnitType == (int)EN_UNIT_TYPE.utMove ) SM.MT_GoIncMan(m_eId, -(Para.dUnit));
                    else if (Para.iUnitType == (int)EN_UNIT_TYPE.utPitch) SM.MT_GoIncMan(m_eId, -(Para.dUnit));
                    else return;
                }
                else
                {
                         if (Para.iUnitType == (int)EN_UNIT_TYPE.utJog  ) SM.MT_JogN     (m_eId);
                    else if (Para.iUnitType == (int)EN_UNIT_TYPE.utMove ) SM.MT_GoIncSlow(m_eId, -(Para.dUnit));
                    else if (Para.iUnitType == (int)EN_UNIT_TYPE.utPitch) SM.MT_GoIncSlow(m_eId, -(Para.dUnit));
                    else return;
                }
            }
            
        }

        private void btPos_MouseDown(object sender, MouseEventArgs e)
        {
            string sTemp;
            bool bRet = true;
            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);
            uint uPstnNo = (uint)iBtnTag;
            //if (!CheckSafe(m_iMotrNo)) return;
            //if (m_iId == (int)mi.IDX_X) bRet = SEQ.IDX.CheckSafe((mi)m_iId, uPstnNo);
            //if (m_iId == (int)mi.PCK_Y) bRet = SEQ.PCK.CheckSafe((mi)m_iId, uPstnNo);
            //if (m_iId == (int)mi.PCK_Z) bRet = SEQ.PCK.CheckSafe((mi)m_iId, uPstnNo);

            if (!bRet) return;
            SM.MT_Stop(m_eId);

            sTemp = m_eId.ToString();

            
            if (SEQ._iSeqStat == EN_SEQ_STAT.Manual)
            {
                Log.ShowMessage("ERROR", "Doing Manual Cycle"); 
                return;
            }
            else
            {
                Log.Trace("Operator", ("FrameMotr Form_" + sTemp + "Motor Pos Move Button Click"));

                if (!cbSlow.Checked)
                {
                         if (Para.iUnitType == (int)EN_UNIT_TYPE.utJog  ) SM.MT_JogP    (m_eId);
                    else if (Para.iUnitType == (int)EN_UNIT_TYPE.utMove ) SM.MT_GoIncMan(m_eId, Para.dUnit);
                    else if (Para.iUnitType == (int)EN_UNIT_TYPE.utPitch) SM.MT_GoIncMan(m_eId, Para.dUnit);
                    else return;
                }
                else
                {
                         if (Para.iUnitType == (int)EN_UNIT_TYPE.utJog  ) SM.MT_JogP     (m_eId);
                    else if (Para.iUnitType == (int)EN_UNIT_TYPE.utMove ) SM.MT_GoIncSlow(m_eId, Para.dUnit);
                    else if (Para.iUnitType == (int)EN_UNIT_TYPE.utPitch) SM.MT_GoIncSlow(m_eId, Para.dUnit);
                    else return;
                }
            }
        }


        private void cbSlow_Click(object sender, EventArgs e)
        {
            if (cbSlow.Checked)
            {
                Log.ShowMessageFunc(string.Format("상대이동값이 10%로 감속됩니다.", m_eId));
            }
            else
            {
                Log.ShowMessageFunc(string.Format("상대이동값이 Manual Speed로 변경됩니다.", m_eId));
            }
        }

        private void btStop_Click(object sender, EventArgs e)
        {
            SM.MT_Stop(m_eId);
        }

        private void btNeg_Click(object sender, EventArgs e)
        {

        }

        private void FraMotr_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmUpdate.Enabled = false;
        }
    }
}


 