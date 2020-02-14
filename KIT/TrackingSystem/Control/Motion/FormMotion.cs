using COMMON;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Control
{
    public partial class FormMotion : Form
    {
        private int   iMotrSel;

        public FormMotion()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            iMotrSel = 0;

            pgMotrPara   .SelectedObject = Util.MT.Para[0];
            S.SelectedObject = Util.MT.Mtr [0].GetPara();
        }

        private void btJogN_MouseDown(object sender, MouseEventArgs e)
        {
            Util.MT.JogN(iMotrSel);
        }

        private void btJogN_MouseUp(object sender, MouseEventArgs e)
        {
            Util.MT.Stop(iMotrSel);
        }

        private void btJogP_MouseDown(object sender, MouseEventArgs e)
        {
            Util.MT.JogP(iMotrSel);
        }

        private void btJogP_MouseUp(object sender, MouseEventArgs e)
        {
            Util.MT.Stop(iMotrSel);
        }

        private void btStop_Click(object sender, EventArgs e)
        {
            Util.MT.Stop(iMotrSel);
        }

        private void btReset_MouseDown(object sender, MouseEventArgs e)
        {
            Util.MT.SetReset(iMotrSel, true);
        }

        private void btReset_MouseUp(object sender, MouseEventArgs e)
        {
            Util.MT.SetReset(iMotrSel, false);
        }

        private void btClearPos_Click(object sender, EventArgs e)
        {
            Util.MT.SetPos(iMotrSel, 0.0);
        }

        private void btHome_Click(object sender, EventArgs e)
        {
            Util.MT.GoHome(iMotrSel);
        }

        private void btServoOn_Click(object sender, EventArgs e)
        {
            Util.MT.SetServo(iMotrSel,true);
        }

        private void btServoOff_Click(object sender, EventArgs e)
        {
            Util.MT.SetServo(iMotrSel, false);
        }

        private void btAllStop_Click(object sender, EventArgs e)
        {
            Util.MT.Stop(iMotrSel);
        }

        private void btServoOnAll_Click(object sender, EventArgs e)
        {
            Util.MT.SetServo(iMotrSel,true);
        }

        private void btServoOffAll_Click(object sender, EventArgs e)
        {
            Util.MT.SetServo(iMotrSel,false);
        }

        private void btSaveMotr_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace("Form Motion " + sText + " Button Click", 1);

            Util.MT.ApplyParaAll();
            Util.MT.LoadSaveAll(false);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;

            int iMotrSel = 0;

            lbStop    .BackColor = Util.MT.GetStop      (iMotrSel) ? Color.Lime:Color.Silver;
            lbInpos   .BackColor = Util.MT.GetInPosSgnl (iMotrSel) ? Color.Lime:Color.Silver;
            lbServo   .BackColor = Util.MT.GetServo     (iMotrSel) ? Color.Lime:Color.Silver;
            lbHomeDone.BackColor = Util.MT.GetHomeDone  (iMotrSel) ? Color.Lime:Color.Silver;
            //lbBreakOff.BackColor = Util.MT.GetBreakOff  (iMotrSel) ? Color.Lime:Color.Silver;
            lbAlram   .BackColor = Util.MT.GetAlarmSgnl (iMotrSel) ? Color.Lime:Color.Silver;
            lbDirP    .BackColor = Util.MT.Stat[iMotrSel].bMovingP ? Color.Lime:Color.Silver;
            lbDirN    .BackColor = Util.MT.Stat[iMotrSel].bMovingN ? Color.Lime:Color.Silver;
            lbZPhase  .BackColor = Util.MT.GetZphaseSgnl(iMotrSel) ? Color.Lime:Color.Silver;
            lbLimitP  .BackColor = Util.MT.GetPLimSnsr  (iMotrSel) ? Color.Lime:Color.Silver;
            lbHome    .BackColor = Util.MT.GetHomeSnsr  (iMotrSel) ? Color.Lime:Color.Silver;
            lbLimitN  .BackColor = Util.MT.GetNLimSnsr  (iMotrSel) ? Color.Lime:Color.Silver;

            lbCmdPos  .Text      = Util.MT.GetCmdPos(iMotrSel).ToString();
            lbEncPos  .Text      = Util.MT.GetEncPos(iMotrSel).ToString();
            lbTrgPos  .Text      = Util.MT.GetTrgPos(iMotrSel).ToString();


            lbX1.BackColor = Util.MT.GetX(iMotrSel,0) ? Color.Lime : Color.Silver;
            lbX2.BackColor = Util.MT.GetX(iMotrSel,1) ? Color.Lime : Color.Silver;
            lbX3.BackColor = Util.MT.GetX(iMotrSel,2) ? Color.Lime : Color.Silver;
            lbX4.BackColor = Util.MT.GetX(iMotrSel,3) ? Color.Lime : Color.Silver;
            lbX5.BackColor = Util.MT.GetX(iMotrSel,4) ? Color.Lime : Color.Silver;

            lbY1.BackColor = Util.MT.GetY(iMotrSel,0) ? Color.Lime : Color.Silver;
            lbY2.BackColor = Util.MT.GetY(iMotrSel,1) ? Color.Lime : Color.Silver;
            lbY3.BackColor = Util.MT.GetY(iMotrSel,2) ? Color.Lime : Color.Silver;
            lbY4.BackColor = Util.MT.GetY(iMotrSel,3) ? Color.Lime : Color.Silver;
            lbY5.BackColor = Util.MT.GetY(iMotrSel,4) ? Color.Lime : Color.Silver;

            if (!this.Visible)
            {
                timer1.Enabled = false;
                return;
            }

            timer1.Enabled = true;
        }

        private void FormMotion_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing) e.Cancel = true;
            this.Hide();
        }

        private void FormMotion_VisibleChanged(object sender, EventArgs e)
        {
            if(this.Visible) timer1.Enabled = true;
        }

        private void lbY1_DoubleClick(object sender, EventArgs e)
        {
            string sText = ((Label)sender).Text;
            Log.Trace("Form Motion " + sText + " Label Clicked", 1);

            string sTag = (string)((Label)sender).Tag;
            int iTag = CConfig.StrToIntDef(sTag,0);
            int iMotrSel = 0;
            bool bRet = Util.MT.GetY(iMotrSel,iTag);
            Util.MT.SetY(iMotrSel,iTag,!bRet);
        }

        private void btGo1stPos_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace("Form Motion " + sText + " Button Clicked", 1);

            int iMotrSel = 0;
            Util.MT.GoAbsRepeatFst(iMotrSel);
        }

        private void btGo2ndPos_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace("Form Motion " + sText + " Button Clicked", 1);

            int iMotrSel = 0;
            Util.MT.GoAbsRepeatScd(iMotrSel);
        }

        private void btStop2_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace("Form Motion " + sText + " Button Clicked", 1);

            int iMotrSel = 0;
            Util.MT.Stop(iMotrSel);
        }

        private void btRepeat_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace("Form Motion " + sText + " Button Clicked", 1);

            int iMotrSel = 0;
            Util.MT.StartRepeat(iMotrSel);
        }

        private void lbY1_Click(object sender, EventArgs e)
        {

        }

        private void btJogN_Click(object sender, EventArgs e)
        {

        }
    }
}
