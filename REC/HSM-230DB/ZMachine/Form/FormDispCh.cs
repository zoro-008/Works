using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using COMMON;

namespace Machine
{
    public partial class FormDispCh : Form
    {
        public FormDispCh()
        {
            InitializeComponent();

            this.Text = (OM.DevOptn.iDspCh + 1).ToString() + "Ch Setting";

            UpdateDispData(true);

            tmUpdate.Enabled = true;
        }

        private void btSigmaMode_Click(object sender, EventArgs e)
        {
            if (!SEQ.Dispr.GetDispData().bSigmaMode)
            {
                Log.ShowMessageModal("시그마모드 진입", "최고용량 실린지로 교체해주세요.");
                SEQ.Dispr.SetSigmaModeOn();
                Thread.Sleep(20000); //바로 클릭 못하게 그냥 화면 잡고 있자.
            }
            else
            {
                Log.ShowMessageModal("시그마모드 해제", "시그마 모드를 해제 합니다.");
                SEQ.Dispr.SetSigmaModeOff(OM.DevOptn.iDspCh + 1);
                Thread.Sleep(500);
            
            }
            CDelayTimer TimeOut = new CDelayTimer();
            TimeOut.Clear();
            while (!SEQ.Dispr.GetMsgEnd())
            { //메세지 다 주고 받을때까지 기다림.
                Thread.Sleep(1);
                if (TimeOut.OnDelay(true, 3000))
                {
                    break;
                }
            }

           

            UpdateDispData(true);
        }



        public void UpdateDispData(bool _toTable)
        {
            if(_toTable) {
                SEQ.Dispr.GetDispData(OM.DevOptn.iDspCh + 1);
                CDelayTimer TimeOut = new CDelayTimer();
                TimeOut.Clear();
                while (!SEQ.Dispr.GetMsgEnd())
                { //메세지 다 주고 받을때까지 기다림.
                    Thread.Sleep(1);
                    if(TimeOut.OnDelay(true , 3000)){
                        break ;
                    }
                }
                
                OM.DevOptn.dDspVacPres = SEQ.Dispr.GetDispData().dVacPres;
                OM.DevOptn.dDspPrsPres = SEQ.Dispr.GetDispData().dPrsPres;
                //cbDspCh              -> ItemIndex = OM.DevOptn.iDspCh            ;
                tbDspVacPres. Text    = OM.DevOptn.dDspVacPres.ToString()   ;//
                tbDspPrsPres. Text    = OM.DevOptn.dDspPrsPres.ToString()   ;//
            
            
            
            
            }
            else { //ToControllor.
                //OM.DevOptn.dDspVacPres       = CConfig.StrToDoubleDef  (tbDspVacPres. Text , OM.DevOptn.dDspVacPres) ;
                OM.DevOptn.dDspVacPres       = CConfig.StrToDoubleDef  (tbDspVacPres. Text , 0.0) ;
                if( OM.DevOptn.dDspVacPres < 0) {
                    OM.DevOptn.dDspVacPres = 0;
                }
                if( OM.DevOptn.dDspVacPres > 20) {
                    OM.DevOptn.dDspVacPres = 20;
                }
                //OM.DevOptn.dDspPrsPres       = CConfig.StrToDoubleDef  (tbDspPrsPres. Text , OM.DevOptn.dDspPrsPres) ;
                OM.DevOptn.dDspPrsPres       = CConfig.StrToDoubleDef  (tbDspPrsPres. Text , 0.0) ;
                if( OM.DevOptn.dDspPrsPres < 30) {
                    OM.DevOptn.dDspPrsPres = 30;
                }
                if( OM.DevOptn.dDspPrsPres > 400) {
                    OM.DevOptn.dDspPrsPres = 400;
                }

                
            
                SEQ.Dispr.SetPTV(OM.DevOptn.dDspPrsPres, 10, OM.DevOptn.dDspVacPres);
                CDelayTimer TimeOut = new CDelayTimer();
                TimeOut.Clear();
                while (!SEQ.Dispr.GetMsgEnd())
                { //메세지 다 주고 받을때까지 기다림.
                    Thread.Sleep(1);
                    if(TimeOut.OnDelay(true , 1000)){
                        break ;
                    }
                }
            
                UpdateDispData(true);
            }
        
            OM.SaveDevOptn(OM.GetCrntDev());
        }

        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            tmUpdate.Enabled = false;
            tbDspPrsPres.Enabled   = !SEQ.Dispr.GetDispData().bSigmaMode ;
            tbDspPrsPres.BackColor = !SEQ.Dispr.GetDispData().bSigmaMode ? SystemColors.Window : Color.Gray;
            tbDspVacPres.Enabled   = !SEQ.Dispr.GetDispData().bSigmaMode ;
            tbDspVacPres.BackColor = !SEQ.Dispr.GetDispData().bSigmaMode ? SystemColors.Window : Color.Gray ;
            btSetDspData.Enabled   = !SEQ.Dispr.GetDispData().bSigmaMode ;
            
            btSigmaMode .ForeColor = SEQ.Dispr.GetDispData().bSigmaMode ? Color.Lime : Color.Black ;
            btSigmaMode .Text      = SEQ.Dispr.GetDispData().bSigmaMode ? "SIGMA ON" : "SIGMA OFF" ;
            tmUpdate.Enabled = true;
        }

        private void btSetDspData_Click(object sender, EventArgs e)
        {
            UpdateDispData(false);
        }

        private void btCheckAmount_Click(object sender, EventArgs e)
        {
            SEQ.Dispr.GetSylVolm(OM.DevOptn.iDspCh + 1);
            CDelayTimer TimeOut = new CDelayTimer() ;
            TimeOut.Clear();
            while (!SEQ.Dispr.GetMsgEnd())
            { //메세지 다 주고 받을때까지 기다림.
                Thread.Sleep(1);
                if(TimeOut.OnDelay(true , 3000)){
                    break ;
                }
            }
            
            lbAmount.Text = (SEQ.Dispr.GetSylFill()) + "%";
        }
    }
}
