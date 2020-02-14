using System;
using System.Drawing;
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
                lvSequence.Items[i].SubItems[3].Text = SEQ.m_Part[i].GetSeqName    ()            ;lvSequence.Items[i].SubItems[3].BackColor = SEQ.m_Part[i].GetSeqStep    () == 0 ? Color.White : Color.Yellow ;
                lvSequence.Items[i].SubItems[4].Text = SEQ.m_Part[i].GetCycleStep  ().ToString() ;lvSequence.Items[i].SubItems[4].BackColor = SEQ.m_Part[i].GetCycleStep  () == 0 ? Color.White : Color.Yellow ;
                lvSequence.Items[i].SubItems[5].Text = SEQ.m_Part[i].GetToStopStep ().ToString() ;lvSequence.Items[i].SubItems[5].BackColor = SEQ.m_Part[i].GetToStopStep () == 0 ? Color.White : Color.Yellow ;
                lvSequence.Items[i].SubItems[6].Text = SEQ.m_Part[i].GetHomeStep   ().ToString() ;lvSequence.Items[i].SubItems[6].BackColor = SEQ.m_Part[i].GetHomeStep   () == 0 ? Color.White : Color.Yellow ;
            }
            tbWorkCnt.Text = Loader.m_iTrayWorkCnt.ToString();

            lbBarReadingText.Text = SEQ.Barcode.GetReadingText();
            tbRecive.Text = SEQ.Laser.GetReadingText();


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

        }

        public void UpdateMstOptn(bool bTable)
        {
            if(bTable){
                cbDebug      .Checked = OM.MstOptn.bDebugMode   ;
                cbIdlerun    .Checked = OM.MstOptn.bIdleRun     ;

        
            }
            else{
                OM.MstOptn.bDebugMode    = cbDebug  .Checked   ;
                OM.MstOptn.bIdleRun      = cbIdlerun.Checked   ;

                
            }
        }

        private void FormMaster_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmUpdate.Enabled = false;
        }


        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void btMsgSend_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            
            SEQ.Laser.SendMsg(tbSendMsg.Text );
        }

        private void button4_Click_1(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            Loader.m_iTrayWorkCnt = 0;
        }

        private void tbSendMsg_TextChanged(object sender, EventArgs e)
        {

        }

        private void btBarOn_Click(object sender, EventArgs e)
        {
            SEQ.Barcode.Read();
        }

        private void btBarOff_Click(object sender, EventArgs e)
        {
            SEQ.Barcode.Stop();
        }

        private void btSendCombobox_Click(object sender, EventArgs e)
        {
            SEQ.Laser.SendMsg(cbSendMsg.Text );
        }

        private void btSendCycle_Click(object sender, EventArgs e)
        {
            SEQ.Laser.SetCycle((RS232_DominoDynamark3.Cycle)cbCycle.SelectedIndex , true);
        }
    }
}
