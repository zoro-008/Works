using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
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

        public int m_iId;
        public int m_iType;

        public TPara Para;






        public void SetIdType(mi _iId, MOTION_DIR _iType)
        {
            m_iId = (int)_iId;
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
                btNeg.Text = "RIGHT(+)";
                btPos.Text = "LEFT(-)";
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

            LbStat1.BackColor = SM.MTR.GetNLimSnsr  (m_iId) ? Color.Lime : Color.Silver;
            LbStat2.BackColor = SM.MTR.GetHomeSnsr  (m_iId) ? Color.Lime : Color.Silver;
            LbStat3.BackColor = SM.MTR.GetPLimSnsr  (m_iId) ? Color.Lime : Color.Silver;
            LbStat4.BackColor = SM.MTR.GetAlarmSgnl (m_iId) ? Color.Lime : Color.Silver;
            LbStat5.BackColor = SM.MTR.GetServo     (m_iId) ? Color.Lime : Color.Silver;
            LbStat6.BackColor = SM.MTR.GetStop      (m_iId) ? Color.Lime : Color.Silver;
            LbStat7.BackColor = SM.MTR.GetHomeDone  (m_iId) ? Color.Lime : Color.Silver;

            LbCmdPos.Text = SM.MTR.GetCmdPos(m_iId).ToString();
            LbEncPos.Text = SM.MTR.GetEncPos(m_iId).ToString();

            tmUpdate.Enabled = true;
        }

        private void btPos_MouseUp(object sender, MouseEventArgs e)
        {
            if (Para.iUnitType == (int)EN_UNIT_TYPE.utJog) SM.MTR.Stop(m_iId);
        }

        private void btNeg_MouseUp(object sender, MouseEventArgs e)
        {
            if (Para.iUnitType == (int)EN_UNIT_TYPE.utJog) SM.MTR.Stop(m_iId);
        }

        private void btNeg_MouseDown(object sender, MouseEventArgs e)
        {
            string sTemp;
            //if (!CheckSafe(m_iMotrNo)) return;

            SM.MTR.Stop(m_iId);

            sTemp = m_iId.ToString();

            Log.Trace("Operator", ("FrameMotr Form_" + sTemp + "Motor Pos Move Button Click"));

            if (!cbSlow.Checked)
            {
                     if (Para.iUnitType == (int)EN_UNIT_TYPE.utJog  ) SM.MTR.JogN    (m_iId);
                else if (Para.iUnitType == (int)EN_UNIT_TYPE.utMove ) SM.MTR.GoIncMan(m_iId, -(Para.dUnit));
                else if (Para.iUnitType == (int)EN_UNIT_TYPE.utPitch) SM.MTR.GoIncMan(m_iId, -(Para.dUnit));
                else return;
            }
            else
            {
                     if (Para.iUnitType == (int)EN_UNIT_TYPE.utJog  ) SM.MTR.JogN     (m_iId);
                else if (Para.iUnitType == (int)EN_UNIT_TYPE.utMove ) SM.MTR.GoIncSlow(m_iId, -(Para.dUnit));
                else if (Para.iUnitType == (int)EN_UNIT_TYPE.utPitch) SM.MTR.GoIncSlow(m_iId, -(Para.dUnit));
                else return;
            }
        }

        private void btPos_MouseDown(object sender, MouseEventArgs e)
        {
            string sTemp;
            //if (!CheckSafe(m_iMotrNo)) return;

            SM.MTR.Stop(m_iId);

            sTemp = m_iId.ToString();

            Log.Trace("Operator", ("FrameMotr Form_" + sTemp + "Motor Pos Move Button Click"));

            if (!cbSlow.Checked)
            {
                     if (Para.iUnitType == (int)EN_UNIT_TYPE.utJog  ) SM.MTR.JogP    (m_iId);
                else if (Para.iUnitType == (int)EN_UNIT_TYPE.utMove ) SM.MTR.GoIncMan(m_iId, Para.dUnit);
                else if (Para.iUnitType == (int)EN_UNIT_TYPE.utPitch) SM.MTR.GoIncMan(m_iId, Para.dUnit);
                else return;
            }
            else
            {
                     if (Para.iUnitType == (int)EN_UNIT_TYPE.utJog  ) SM.MTR.JogP     (m_iId);
                else if (Para.iUnitType == (int)EN_UNIT_TYPE.utMove ) SM.MTR.GoIncSlow(m_iId, Para.dUnit);
                else if (Para.iUnitType == (int)EN_UNIT_TYPE.utPitch) SM.MTR.GoIncSlow(m_iId, Para.dUnit);
                else return;
            }
        }


        private void cbSlow_Click(object sender, EventArgs e)
        {
            if (cbSlow.Checked)
            {
                Log.ShowMessageFunc(string.Format("상대이동값이 10%로 감속됩니다.", m_iId));
            }
            else
            {
                Log.ShowMessageFunc(string.Format("상대이동값이 Manual Speed로 변경됩니다.", m_iId));
            }
        }

        private void btStop_Click(object sender, EventArgs e)
        {
            SM.MTR.Stop(m_iId);
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


 