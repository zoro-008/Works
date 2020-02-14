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

namespace SMDll2
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

            //Control Scale 변경
            float widthRatio = 1024f / 1280f;
            float heightRatio = 607f / 863f;
            SizeF scale = new SizeF(widthRatio, heightRatio);
            this.Scale(scale);

            foreach (Control control in this.Controls)
            {
                control.Font = new Font("Verdana", control.Font.SizeInPoints * heightRatio * widthRatio);
            }


           
            this.Width = 244;
            this.Height = 143;
           

        }

        public struct TPara
        {
            public double dUnit;
            public int iUnitType;
            public double dPitch;
            public int iMotrNo;
            public int iType;
        }

        public TPara Para;






        public void SetIdType(int _iMotrNo, MOTION_DIR _iType)
        {
            Para.iMotrNo = _iMotrNo;
            Para.iType = (int)_iType;

            if (_iType == MOTION_DIR.LeftRight)
            {
                btNeg.Image = global::SMDll2.Properties.Resources.LEFT;
                btPos.Image = global::SMDll2.Properties.Resources.RIGHT;
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
                btNeg.Image = global::SMDll2.Properties.Resources.RIGHT;
                btPos.Image = global::SMDll2.Properties.Resources.LEFT;
                btNeg.Text = "RIGHT(+)";
                btPos.Text = "LEFT(-)";
            }

            if (_iType == MOTION_DIR.BwdFwd)
            {
                btNeg.Image = global::SMDll2.Properties.Resources.UP;
                btPos.Image = global::SMDll2.Properties.Resources.DN;
                btNeg.Text = "BWD(-)";
                btPos.Text = "FWD(+)";
            }

            if (_iType == MOTION_DIR.FwdBwd)
            {
                btNeg.Image = global::SMDll2.Properties.Resources.DN;
                btPos.Image = global::SMDll2.Properties.Resources.UP;
                btNeg.Text = "FWD(-)";
                btPos.Text = "BWD(+)";
            }

            if (_iType == MOTION_DIR.DownUp)
            {
                btNeg.Image = global::SMDll2.Properties.Resources.DN;
                btPos.Image = global::SMDll2.Properties.Resources.UP;
                btNeg.Text = "DN(-)";
                btPos.Text = "UP(+)";
            }

            if (_iType == MOTION_DIR.UpDown)
            {
                btNeg.Image = global::SMDll2.Properties.Resources.UP;
                btPos.Image = global::SMDll2.Properties.Resources.DN;
                btNeg.Text = "UP(-)";
                btPos.Text = "DN(+)";
            }

            if (_iType == MOTION_DIR.CcwCw)
            {
                btNeg.Image = global::SMDll2.Properties.Resources.CCW;
                btPos.Image = global::SMDll2.Properties.Resources.CW;
                btNeg.Text = "CCW(-)";
                btPos.Text = "CW(+)";
            }

            if (_iType == MOTION_DIR.CwCcw)
            {
                btNeg.Image = global::SMDll2.Properties.Resources.CW;
                btPos.Image = global::SMDll2.Properties.Resources.CCW;
                btNeg.Text = "CW(-)";
                btPos.Text = "CCW(+)";
            }
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

            LbStat1.BackColor = SM.MT.GetNLimSnsr  (Para.iMotrNo) ? Color.Lime : Color.Silver;
            LbStat2.BackColor = SM.MT.GetHomeSnsr  (Para.iMotrNo) ? Color.Lime : Color.Silver;
            LbStat3.BackColor = SM.MT.GetPLimSnsr  (Para.iMotrNo) ? Color.Lime : Color.Silver;
            LbStat4.BackColor = SM.MT.GetAlarmSgnl (Para.iMotrNo) ? Color.Lime : Color.Silver;
            LbStat5.BackColor = SM.MT.GetServo     (Para.iMotrNo) ? Color.Lime : Color.Silver;
            LbStat6.BackColor = SM.MT.GetStop      (Para.iMotrNo) ? Color.Lime : Color.Silver;
            LbStat7.BackColor = SM.MT.GetHomeDone  (Para.iMotrNo) ? Color.Lime : Color.Silver;

            LbCmdPos.Text = SM.MT.GetCmdPos(Para.iMotrNo).ToString();
            LbEncPos.Text = SM.MT.GetEncPos(Para.iMotrNo).ToString();

            tmUpdate.Enabled = true;
        }

        private void btPos_MouseUp(object sender, MouseEventArgs e)
        {
            if (Para.iUnitType == (int)EN_UNIT_TYPE.utJog) SM.MT.Stop(Para.iMotrNo);
        }

        private void btNeg_MouseUp(object sender, MouseEventArgs e)
        {
            if (Para.iUnitType == (int)EN_UNIT_TYPE.utJog) SM.MT.Stop(Para.iMotrNo);
        }

        private void btNeg_MouseDown(object sender, MouseEventArgs e)
        {
            string sTemp;
            //if (!CheckSafe(m_iMotrNo)) return;

            SM.MT.Stop(Para.iMotrNo);

            sTemp = Para.iMotrNo.ToString();

            Log.Trace("Operator", ("FrameMotr Form_" + sTemp + "Motor Pos Move Button Click"));

            if (!cbSlow.Checked)
            {
                     if (Para.iUnitType == (int)EN_UNIT_TYPE.utJog  ) SM.MT.JogP    (Para.iMotrNo);
                else if (Para.iUnitType == (int)EN_UNIT_TYPE.utMove ) SM.MT.GoIncMan(Para.iMotrNo, Para.dUnit);
                else if (Para.iUnitType == (int)EN_UNIT_TYPE.utPitch) SM.MT.GoIncMan(Para.iMotrNo, Para.dUnit);
                else return;
            }
            else
            {
                     if (Para.iUnitType == (int)EN_UNIT_TYPE.utJog  ) SM.MT.JogP    (Para.iMotrNo);
                else if (Para.iUnitType == (int)EN_UNIT_TYPE.utMove ) SM.MT.GoIncMan(Para.iMotrNo, Para.dUnit);
                else if (Para.iUnitType == (int)EN_UNIT_TYPE.utPitch) SM.MT.GoIncMan(Para.iMotrNo, Para.dUnit);
                else return;
            }
        }

        private void btPos_MouseDown(object sender, MouseEventArgs e)
        {
            string sTemp;
            //if (!CheckSafe(m_iMotrNo)) return;

            SM.MT.Stop(Para.iMotrNo);

            sTemp = Para.iMotrNo.ToString();

            Log.Trace("Operator", ("FrameMotr Form_" + sTemp + "Motor Pos Move Button Click"));

            if (!cbSlow.Checked)
            {
                     if (Para.iUnitType == (int)EN_UNIT_TYPE.utJog  ) SM.MT.JogP    (Para.iMotrNo);
                else if (Para.iUnitType == (int)EN_UNIT_TYPE.utMove ) SM.MT.GoIncMan(Para.iMotrNo, Para.dUnit);
                else if (Para.iUnitType == (int)EN_UNIT_TYPE.utPitch) SM.MT.GoIncMan(Para.iMotrNo, Para.dUnit);
                else return;
            }
            else
            {
                     if (Para.iUnitType == (int)EN_UNIT_TYPE.utJog  ) SM.MT.JogP    (Para.iMotrNo);
                else if (Para.iUnitType == (int)EN_UNIT_TYPE.utMove ) SM.MT.GoIncVel(Para.iMotrNo, Para.dUnit, 0.01);
                else if (Para.iUnitType == (int)EN_UNIT_TYPE.utPitch) SM.MT.GoIncVel(Para.iMotrNo, Para.dUnit, 0.01);
                else return;
            }
        }


        private void cbSlow_Click(object sender, EventArgs e)
        {
            if (cbSlow.Checked)
            {
                Log.ShowMessageFunc(string.Format("상대이동값이 10%로 감속됩니다.", Para.iMotrNo));
            }
            else
            {
                Log.ShowMessageFunc(string.Format("상대이동값이 Manual Speed로 변경됩니다.", Para.iMotrNo));
            }
        }

        private void btStop_Click(object sender, EventArgs e)
        {
            SM.MT.Stop(Para.iMotrNo);
        }
    }
}


 