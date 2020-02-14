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

            UpdateMstOptn(true);
            OM.LoadMstOptn();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < (int)pi.MAX_PART; i++)
            {
                lvSequence.Items[i].SubItems[2].Text = SEQ.m_Part[i].GetToStartStep().ToString() ;lvSequence.Items[i].SubItems[2].BackColor = SEQ.m_Part[i].GetToStartStep() == 0 ? Color.White : Color.Yellow ;
                lvSequence.Items[i].SubItems[3].Text = SEQ.m_Part[i].GetSeqStep    ().ToString() ;lvSequence.Items[i].SubItems[3].BackColor = SEQ.m_Part[i].GetSeqStep    () == 0 ? Color.White : Color.Yellow ;
                lvSequence.Items[i].SubItems[4].Text = SEQ.m_Part[i].GetCycleStep  ().ToString() ;lvSequence.Items[i].SubItems[4].BackColor = SEQ.m_Part[i].GetCycleStep  () == 0 ? Color.White : Color.Yellow ;
                lvSequence.Items[i].SubItems[5].Text = SEQ.m_Part[i].GetToStopStep ().ToString() ;lvSequence.Items[i].SubItems[5].BackColor = SEQ.m_Part[i].GetToStopStep () == 0 ? Color.White : Color.Yellow ;
                lvSequence.Items[i].SubItems[6].Text = SEQ.m_Part[i].GetHomeStep   ().ToString() ;lvSequence.Items[i].SubItems[6].BackColor = SEQ.m_Part[i].GetHomeStep   () == 0 ? Color.White : Color.Yellow ;
            }

            tbRcvDataId.Text = VC.GetVisnRecvViewMsgId().ToString();
            tbRcvData  .Text = VC.GetVisnRecvViewMsg();

            tbWorkCnt.Text = Loader.iTrayWorkCnt.ToString();
            
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
                if(cbPart[i].Checked) SEQ.m_Part[i].Autorun();
                
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
                cbDebug      .Checked = OM.MstOptn.bDebugMode   ;
                cbIdlerun    .Checked = OM.MstOptn.bIdleRun     ;

                tbTrgOfs     .Text    = OM.MstOptn.dTrgOfs.ToString()  ;
                
                
        
            }
            else{
                OM.MstOptn.bDebugMode    = cbDebug  .Checked   ;
                OM.MstOptn.bIdleRun      = cbIdlerun.Checked   ;

                OM.MstOptn.dTrgOfs       = CConfig.StrToDoubleDef(tbTrgOfs.Text, 0.0);
                
            }
        }

        private void FormMaster_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmUpdate.Enabled = false;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            int iOneshotTime = CConfig.StrToIntDef(tbOneShotTime.Text, 0);
            SM.MT.OneShotTrg((int)mi.PCK_Y, true, iOneshotTime);
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
            if(cbCSSignal.SelectedIndex == 4) VC.SendVisnMsg(VC.sm.Insp                  );
            if(cbCSSignal.SelectedIndex == 5) VC.SendVisnMsg(VC.sm.End                   );
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string sMsg = "";

            //S001 = S   -> 프린트 할 내용. 
            //       001 -> 디바이스 이름.
            sMsg = "S001" + tbSendMsg.Text;
            SEQ.Com[1].SendMsg(sMsg);

            string sRepCnt = "";
            string sRepDelay = "";

            int iRepCnt   = 0;
            int iRepDelay = 0;

            iRepCnt = CConfig.StrToIntDef(tbReptCnt.Text, 0);
            sRepCnt = iRepCnt.ToString("D2");

            iRepDelay = CConfig.StrToIntDef(tbReptDelay.Text, 0);
            sRepDelay = iRepDelay.ToString("D4");
            
            
            //G1 = G         -> Repeat관련 메시지 전송.
            //     1         -> Head No.
            //     sRepCnt   -> Repeat Count.
            //     sRepDelay -> Repeat Delay.
            sMsg = "G1" + sRepCnt + sRepDelay;
            
            SEQ.Com[1].SendMsg(sMsg);

            //P1001 = P  -> 전송 메시지 입력.
            //        1  -> Head No.
            //        001-> 디바이스 이름.
            sMsg = "P1001";
            SEQ.Com[1].SendMsg(sMsg);
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            if(tbMsgData.Text != "") VC.SendMsgTest(int.Parse(tbMsgData.Text));
        }

        private void button5_Click(object sender, EventArgs e)
        {
            
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            Loader.iTrayWorkCnt = 0;
        }

    }
}
