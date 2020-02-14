using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Reflection;
using COMMON;
using SML;

namespace Machine
{
    public partial class FormMaster : Form
    {
        //요건 나중에 파트 여러개 있을때 조정 하자.
        const int iLvHeadHeight = 25 ;
        const int iLvCellHeight = 17 ;
        CheckBox [] cbPart = new CheckBox[(int)pi.MAX_PART];
        private const string sFormText = "Form Master ";

        public FormMaster()
        {
            InitializeComponent();

            //Input ListView
            lvSequence.Clear();
            lvSequence.View = View.Details;
            lvSequence.FullRowSelect = true;
            lvSequence.MultiSelect   = true;
            lvSequence.GridLines     = true;
            

            lvSequence.Columns.Add("Eabled"    , 60 ,HorizontalAlignment.Left);
            lvSequence.Columns.Add("Part Name" , 90 ,HorizontalAlignment.Left);
            lvSequence.Columns.Add("ToSTart"   , 60 ,HorizontalAlignment.Left);
            lvSequence.Columns.Add("Seq"       , 60 ,HorizontalAlignment.Left);
            lvSequence.Columns.Add("Cycle"     , 60, HorizontalAlignment.Left);
            lvSequence.Columns.Add("ToStop"    , 60, HorizontalAlignment.Left);            
            lvSequence.Columns.Add("Home"      , 60, HorizontalAlignment.Left);            

            ListViewItem [] liInput = new ListViewItem [(int)pi.MAX_PART];
            for (int i = 0; i < (int)pi.MAX_PART; i++)
            {
                liInput[i] = new ListViewItem("");
                liInput[i].SubItems.Add("");//SEQ.m_Part[i].GetPartName());
                liInput[i].SubItems.Add("");
                liInput[i].SubItems.Add("");
                liInput[i].SubItems.Add("");
                liInput[i].SubItems.Add("");
                liInput[i].SubItems.Add("");

                liInput[i].UseItemStyleForSubItems = false;
                lvSequence.Items.Add(liInput[i]);
                
                cbPart[i] = new CheckBox(); 
                cbPart[i].Text = "";
                cbPart[i].Size= new System.Drawing.Size(13,13);
                lvSequence.Controls.Add(cbPart[i]);
                cbPart[i].Left = 15  ;
                cbPart[i].Top  = iLvHeadHeight + iLvCellHeight * i ;



            }

            var PropInput = lvSequence.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropInput.SetValue(lvSequence, true, null);

            UpdateMstOptn(true);
            OM.LoadMstOptn();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            tmUpdate.Enabled = false;
            for (int i = 0; i < (int)pi.MAX_PART; i++)
            {
                //lvSequence.Items[i].SubItems[2].Text = SEQ.m_Part[i].GetToStartStep  ().ToString() ;lvSequence.Items[i].SubItems[2].BackColor = SEQ.m_Part[i].GetToStartStep() == 0 ? Color.White : Color.Yellow ;
                //lvSequence.Items[i].SubItems[3].Text = SEQ.m_Part[i].GetCrntCycleName()            ;lvSequence.Items[i].SubItems[3].BackColor = SEQ.m_Part[i].GetSeqStep    () == 0 ? Color.White : Color.Yellow ;
                //lvSequence.Items[i].SubItems[4].Text = SEQ.m_Part[i].GetCycleStep    ().ToString() ;lvSequence.Items[i].SubItems[4].BackColor = SEQ.m_Part[i].GetCycleStep  () == 0 ? Color.White : Color.Yellow ;
                //lvSequence.Items[i].SubItems[5].Text = SEQ.m_Part[i].GetToStopStep   ().ToString() ;lvSequence.Items[i].SubItems[5].BackColor = SEQ.m_Part[i].GetToStopStep () == 0 ? Color.White : Color.Yellow ;
                //lvSequence.Items[i].SubItems[6].Text = SEQ.m_Part[i].GetHomeStep     ().ToString() ;lvSequence.Items[i].SubItems[6].BackColor = SEQ.m_Part[i].GetHomeStep   () == 0 ? Color.White : Color.Yellow ;
            }

            tbRcvDataId.Text = VC.GetVisnRecvViewMsgId().ToString();
            tbRcvData  .Text = VC.GetVisnRecvViewMsg();
            if (!this.Visible)
            {
                tmUpdate.Enabled = false;
                return;
            }
            tmUpdate.Enabled = true;
        }

        private void FormMaster_Shown(object sender, EventArgs e)
        {
            tmUpdate.Enabled = true ;
        }

        private void btPartReset_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            for (int i = 0; i < (int)pi.MAX_PART; i++)
            {
                //if(cbPart[i].Checked) SEQ.m_Part[i].Reset();
                
            }
        }

        private void btPartAutorun_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            for (int i = 0; i < (int)pi.MAX_PART; i++)
            {
                //if(cbPart[i].Checked) SEQ.m_Part[i].Autorun();
                
            }
        }

        private void btAllReset_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            SEQ.Reset();
        }

        private void btAllCheck_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            bool bChecked = cbPart[0].Checked ;
            
            for (int i = 0; i < (int)pi.MAX_PART; i++)
            {
                cbPart[i].Checked = !bChecked ;
                
            }
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            UpdateMstOptn(false);
            OM.SaveMstOptn();
        }

        public void UpdateMstOptn(bool bTable)
        {
            if(bTable)
            {
                CConfig.ValToCon(cbDebug       , ref OM.MstOptn.bDebugMode  );
                CConfig.ValToCon(cbIdlerun     , ref OM.MstOptn.bIdleRun    );
                CConfig.ValToCon(tbTrgOfs      , ref OM.MstOptn.dTrgOfs     );
                CConfig.ValToCon(cbMarkAlign   , ref OM.MstOptn.bMarkAlgin  );
                //cbDebug        .Checked = OM.MstOptn.bDebugMode   ;
                //cbIdlerun      .Checked = OM.MstOptn.bIdleRun     ;
                //               
                //tbTrgOfs       .Text    = OM.MstOptn.dTrgOfs.ToString()  ;
            }
            else{
                CConfig.ConToVal(cbDebug       , ref OM.MstOptn.bDebugMode  );
                CConfig.ConToVal(cbIdlerun     , ref OM.MstOptn.bIdleRun    );
                CConfig.ConToVal(tbTrgOfs      , ref OM.MstOptn.dTrgOfs     );
                CConfig.ConToVal(cbMarkAlign   , ref OM.MstOptn.bMarkAlgin  );
                //OM.MstOptn.bDebugMode     = cbDebug  .Checked   ;
                //OM.MstOptn.bIdleRun       = cbIdlerun.Checked   ;
                //                         
                //OM.MstOptn.dTrgOfs        = CConfig.StrToDoubleDef(tbTrgOfs.Text, 0.0);
                
            }
        }

        private void FormMaster_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmUpdate.Enabled = false;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            int iOneshotTime = CConfig.StrToIntDef(tbOneShotTime.Text, 0);
            //ML.MT_OneShotTrg(mi.TOOL_YTool, true, iOneshotTime);
            //SML.MT.OneShotTrg((int)mi.PCK_Y, true, iOneshotTime);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            //VC.ClearRecvData();
        }

        private void btMsgSend_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            //if(cbCSSignal.SelectedIndex == 0) VC.SendVisnMsg(VC.ML.Busy                  );
            //if(cbCSSignal.SelectedIndex == 1) VC.SendVisnMsg(VC.ML.Ready                 );
            //if(cbCSSignal.SelectedIndex == 2) VC.SendVisnMsg(VC.ML.Reset                 );
            //if(cbCSSignal.SelectedIndex == 3) VC.SendVisnMsg(VC.ML.Change, tbMsgData.Text);
            //if(cbCSSignal.SelectedIndex == 4) VC.SendVisnMsg(VC.ML.Insp                  );
            //if(cbCSSignal.SelectedIndex == 5) VC.SendVisnMsg(VC.ML.End                   );
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            if(tbMsgData.Text != "") VC.SendMsgTest(int.Parse(tbMsgData.Text));
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void btWeightCheck_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);
            //SEQ.LoadCell.WeightCheck();
        }

        private void btSetHoldOn_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);
            //SEQ.LoadCell.SetHold(true);
        }

        private void btSetHoldOff_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);
            //SEQ.LoadCell.SetHold(false);
        }

        private void btSetZero_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);
            //SEQ.LoadCell.SetZero();
        }

        private void btShowWeight_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);
            //double dWeight = SEQ.LoadCell.GetLoadCellData();

            //MessageBox.Show(dWeight.ToString());
        }

        private void btSetLoadCh_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);
            //int iCh = 0 ; 
            //int.TryParse(tbCh.Text , out iCh);

            //SEQ.Dispr.SetLoadCh(iCh);
        }

        private void btCheckHeight_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);
            //SEQ.HeightSnsr.SendCheckHeight();
        }

        private void btGetHeight_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);
            //double dHeight = SEQ.HeightSnsr.GetHeight();
            //MessageBox.Show(dHeight.ToString());
        }

        private void btReZero_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);
            //SEQ.HeightSnsr.SendRezero();
        }

        private void btScanOn_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);
            //SEQ.BarcordSSTG.SendScanOn();
        }

        private void btScanOff_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);
            //SEQ.BarcordSSTG.SendScanOff();
        }

        private void btShowMessage_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);
            //string sMsg = SEQ.BarcordSSTG.GetText();
            //MessageBox.Show(sMsg);


        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            
        }

        private void btSetTemp_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);
            //int iTemp = int.Parse(tbTemp.Text);
            //SEQ.Temp.SetTemp(0, iTemp);
            //SEQ.Temp.SendSetTemp(0, iTemp);
        }

        private void button3_Click_2(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);
            //SEQ.Temp.GetCrntTemp(0);
        }

        private void btForcedHomeEnd_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            ML.MT_SetServoAll(true);

            Thread.Sleep(100);

            for(int i = 0 ; i < (int)mi.MAX_MOTR; i++){
                ML.MT_SetHomeDone(i,true);
            }
        }

        private void btImgSave_Click(object sender,EventArgs e) {
            UpdateMstOptn(false);
        }

        private void button5_Click_1(object sender,EventArgs e) {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);
        }

        private void FormMaster_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible) tmUpdate.Enabled = true;
        }
    }
}
