using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using Machine;
using COMMON;
using SMDll2;

namespace Machine
{
    public partial class FormMaster : Form
    {
        //요건 나중에 파트 여러개 있을때 조정 하자.
        const int iLvHeadHeight = 25 ;
        const int iLvCellHeight = 17 ;
        CheckBox [] cbPart = new CheckBox[(int)pi.MAX_PART];

        EN_SEQ_CYCLE eSeq;

        public FormMaster()
        {
            //this.TopMost = true;
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
                liInput[i].SubItems.Add(SEQ.m_Part[i].GetPartName().ToString());
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

            InitLookUpTable();

            UpdateMstOptn(true);
            OM.LoadMstOptn();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < (int)pi.MAX_PART; i++)
            {
                lvSequence.Items[i].SubItems[2].Text = SEQ.m_Part[i].GetToStartStep().ToString()           ;lvSequence.Items[i].SubItems[2].BackColor = SEQ.m_Part[i].GetToStartStep() == 0 ? Color.White : Color.Yellow ;
                //lvSequence.Items[i].SubItems[3].Text = SEQ.m_Part[i].GetSeqStep    ().ToString()           ; lvSequence.Items[i].SubItems[3].BackColor = SEQ.m_Part[i].GetSeqStep() == 0 ? Color.White : Color.Yellow;
                int iTemp = SEQ.m_Part[i].GetSeqStep();
                string sTemp = SEQ.m_Part[i].GetCycleName(SEQ.m_Part[i].GetSeqStep());
                lvSequence.Items[i].SubItems[3].Text = SEQ.m_Part[i].GetCycleName(SEQ.m_Part[i].GetSeqStep()) ; lvSequence.Items[i].SubItems[4].BackColor = SEQ.m_Part[i].GetCycleName(SEQ.m_Part[i].GetSeqStep()) == "Idle" ? Color.White : Color.Yellow;
                lvSequence.Items[i].SubItems[4].Text = SEQ.m_Part[i].GetCycleStep  ().ToString()           ;lvSequence.Items[i].SubItems[4].BackColor = SEQ.m_Part[i].GetCycleStep  () == 0 ? Color.White : Color.Yellow ;
                lvSequence.Items[i].SubItems[5].Text = SEQ.m_Part[i].GetToStopStep ().ToString()           ;lvSequence.Items[i].SubItems[5].BackColor = SEQ.m_Part[i].GetToStopStep () == 0 ? Color.White : Color.Yellow ;
                lvSequence.Items[i].SubItems[6].Text = SEQ.m_Part[i].GetHomeStep   ().ToString()           ;lvSequence.Items[i].SubItems[6].BackColor = SEQ.m_Part[i].GetHomeStep   () == 0 ? Color.White : Color.Yellow ;
            }

            tbRcvDataId.Text = VC.GetVisnRecvViewMsgId().ToString();
            tbRcvData  .Text = VC.GetVisnRecvViewMsg();

            textBox2.Text = SM.MT.GetAlarmSgnl((int)mi.PCK_TL).ToString() + SM.MT.GetAlarmSgnl((int)mi.PCK_TR).ToString();

            //tcMaster.SelectedIndex == 
            UpdateLookUpTable();
        }

        private void FormMaster_Shown(object sender, EventArgs e)
        {
            tmUpdate.Enabled = true ;
        }

        private void btPartReset_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < (int)pi.MAX_PART; i++)
            {
                if(cbPart[i].Checked) SEQ.m_Part[i].Reset();
                
            }
        }

        private void btPartAutorun_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < (int)pi.MAX_PART; i++)
            {
                if (cbPart[i].Checked) SEQ.m_Part[i].SetManAutorun(true);
                
            }
        }

        private void btAllReset_Click(object sender, EventArgs e)
        {
            SEQ.Reset();
        }

        private void btAllCheck_Click(object sender, EventArgs e)
        {
            bool bChecked = cbPart[0].Checked ;
            
            for (int i = 0; i < (int)pi.MAX_PART; i++)
            {
                cbPart[i].Checked = !bChecked ;
                
            }
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            UpdateMstOptn(false);
            OM.SaveMstOptn();
        }

        public void UpdateMstOptn(bool bTable)
        {
            if(bTable){
                cbDebug         .Checked = OM.MstOptn.bDebugMode   ;
                tbTrgOfs        .Text    = OM.MstOptn.dTrgOfs        .ToString()  ;
                tbMaxTorqueMax  .Text    = OM.MstOptn.iMaxTorqueMax  .ToString()  ;
                tbMaxTorqueLimit.Text    = OM.MstOptn.iMaxTorqueLimit.ToString()  ;
                tbMaxTorqueTime .Text    = OM.MstOptn.iMaxTorqueTime .ToString()  ;
                cbUseUnlock     .Checked = OM.MstOptn.bUnlock                     ;
                cbUseEccntrCorr .Checked = OM.MstOptn.bUseEccntrCorr;

                UpdateLookUpTable();



            }
            else{
                OM.MstOptn.bDebugMode      = cbDebug.Checked   ;
                OM.MstOptn.dTrgOfs         = CConfig.StrToDoubleDef(tbTrgOfs        .Text, 0.0);
                OM.MstOptn.iMaxTorqueMax   = CConfig.StrToIntDef   (tbMaxTorqueMax  .Text, 0  );
                OM.MstOptn.iMaxTorqueLimit = CConfig.StrToIntDef   (tbMaxTorqueLimit.Text, 0  );
                OM.MstOptn.iMaxTorqueTime  = CConfig.StrToIntDef   (tbMaxTorqueTime .Text, 0  );
                OM.MstOptn.bUnlock         = cbUseUnlock.Checked;
                OM.MstOptn.bUseEccntrCorr  = cbUseEccntrCorr.Checked;
            }
        }

        private void FormMaster_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmUpdate.Enabled = false;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            int iOneshotTime = CConfig.StrToIntDef(tbOneShotTime.Text, 0);
            //SM.MT.OneShotTrg((int)mi.PCK_Y, true, iOneshotTime);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            VC.ClearRecvData();
        }

        private void btMsgSend_Click(object sender, EventArgs e)
        {
            if(cbCSSignal.SelectedIndex == 0) VC.SendVisnMsg(VC.sm.Busy                  );
            if(cbCSSignal.SelectedIndex == 1) VC.SendVisnMsg(VC.sm.Ready                 );
            if(cbCSSignal.SelectedIndex == 2) VC.SendVisnMsg(VC.sm.Reset                 );
            if(cbCSSignal.SelectedIndex == 3) VC.SendVisnMsg(VC.sm.Change, tbMsgData.Text);
            if(cbCSSignal.SelectedIndex == 4) VC.SendVisnMsg(VC.sm.Insp  , tbMsgData.Text);
            if(cbCSSignal.SelectedIndex == 5) VC.SendVisnMsg(VC.sm.End                   );
        }

        public void InitLookUpTable()
        {
            lvTable.Dock = DockStyle.Fill;

            //Device ListView
            lvTable.Clear();
            lvTable.Sorting = SortOrder.None;
            lvTable.View = View.Details;
            lvTable.FullRowSelect = true;

            lvTable.Columns.Add("No"     , 30 , HorizontalAlignment.Left);
            lvTable.Columns.Add("Degree" , 55 , HorizontalAlignment.Left);
            lvTable.Columns.Add("Left X" , 54 , HorizontalAlignment.Left);
            lvTable.Columns.Add("Left Y" , 54 , HorizontalAlignment.Left);
            lvTable.Columns.Add("Left T" , 54 , HorizontalAlignment.Left);
            lvTable.Columns.Add("Right X", 54 , HorizontalAlignment.Left);
            lvTable.Columns.Add("Right Y", 54 , HorizontalAlignment.Left);
            lvTable.Columns.Add("Right T", 54 , HorizontalAlignment.Left);

            int iNo = 1;

            for (int i = 0; i < OM.MAX_TABLE; i++)
            {
                ListViewItem item = new ListViewItem(string.Format("{0}", iNo));
                item.SubItems.Add("");
                item.SubItems.Add("");
                item.SubItems.Add("");
                item.SubItems.Add("");
                item.SubItems.Add("");
                item.SubItems.Add("");
                item.SubItems.Add("");
                lvTable.Items.Add(item);
            }

            var PropDevice = lvTable.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropDevice.SetValue(lvTable, true, null);

            UpdateLookUpTable();
        }

        public void UpdateLookUpTable()
        {                                                                                   
            lvTable.Items.Clear();
            lvTable.BeginUpdate();
            ListViewItem item0;
            for (int j = OM.MAX_TABLE-1; j >= 0; j--)
            {
                item0 = new ListViewItem((j + 1).ToString());
                item0.SubItems.Add((j * 20).ToString());
                item0.SubItems.Add(OM.LeftTable [j].dX.ToString());
                item0.SubItems.Add(OM.LeftTable [j].dY.ToString());
                item0.SubItems.Add(OM.LeftTable [j].dT.ToString());
                item0.SubItems.Add(OM.RightTable[j].dX.ToString());
                item0.SubItems.Add(OM.RightTable[j].dY.ToString());
                item0.SubItems.Add(OM.RightTable[j].dT.ToString());

                lvTable.Items.Insert(0, item0);
            }
            lvTable.EndUpdate();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //SEQ.Com[1].SendMsg(tbSendMsg.Text);
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            //if(tbMsgData.Text != "") VC.SendMsgTest(int.Parse(tbMsgData.Text));
        }

        private void button5_Click(object sender, EventArgs e)
        {
            
        }

        private void FormMaster_VisibleChanged(object sender, EventArgs e)
        {
            tmUpdate.Enabled = Visible;
        }

        private void btEccentricInsp_Click(object sender, EventArgs e)
        {
            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);
            MM.SetManCycle((mc)iBtnTag);
        }
    }
}
