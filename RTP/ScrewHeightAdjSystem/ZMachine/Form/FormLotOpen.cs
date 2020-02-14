using System;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;
using COMMON;
using SML;

namespace Machine
{
    public partial class FormLotOpen : Form
    {
        public static FormLotOpen FrmLotOpen;
        private const string sFormText = "Form Operation ";

        public FormLotOpen()
        {
            InitializeComponent();
            
            tbSelDevice.Text = OM.GetCrntDev();
            tbLotNo     .Text = "";

            tbLotNo.Focus();
            
        }

        private void UpdateLotList()
        {

            Type type = typeof(LOT.TLot);
            int iCntOfItem = type.GetProperties().Length;
            FieldInfo[] f = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            //컬럼추가 하고 이름을 넣는다.
            string sValue = "";
            string sName = "";
            //ListViewItem[] liitem = new ListViewItem[LOT.LotList.Count];
            //for (int r = 0; r < LOT.LotList.Count; r++)
            //{
            //    liitem[r] = new ListViewItem(string.Format("{0}", r));
            //    for (int c = 0; c < f.Length; c++)
            //    {
            //        sName = f[c].Name;
            //        sValue = f[c].GetValue(LOT.LotList[r]).ToString();
            //        liitem[r].SubItems.Add(sValue);
            //    }
            //    liitem[r].UseItemStyleForSubItems = false;
            //    lvLot.Items.Add(liitem[r]);
            //}
            //lvLot.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

        }

        private void btLotOpen_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ForContext.Frm);
            
            if (tbLotNo.Text == "") return;// tbLotId.Text = DateTime.Now.ToString("HHmmss");

            string LotNo  = tbLotNo.Text.Trim();
            string Device = tbSelDevice.Text.Trim();

            LOT.TLot Lot ;
            
            Lot.sLotNo      = tbLotNo     .Text.Trim();
            if(!int.TryParse(tbWorkCount.Text, out OM.EqpStat.iTotalWorkCount))
            {
                ML.ER_SetErr(ei.ETC_LotOpen, "워크 카운트 숫자입력이 잘못되었습니다.");
                return;
            }

            LOT.LotOpen(Lot);

            //DM.ARAY[ri.STT].SetStat(cs.None);
            //DM.ARAY[ri.PRE].SetStat(cs.None);
            //DM.ARAY[ri.ADJ].SetStat(cs.None);
            //DM.ARAY[ri.PST].SetStat(cs.None);



            //FrmOperation.


            //Log.Trace("LotOpen", "Try");

            //CDelayTimer TimeOut = new CDelayTimer();
            //TimeOut.Clear();
            //SEQ.Visn.SendLotStart(Lot.sLotNo);
            //while(!SEQ.Visn.GetSendCycleEnd(VisnCom.vs.LotStart  )){
            //    Thread.Sleep(1);
            //    if(TimeOut.OnDelay(5000)) { 
            //        SM.ER_SetErr(ei.VSN_ComErr,"Lot Start TimeOut");
            //        break;
            //    }
            //}
            //DM.ARAY[ri.LODR].SetStat(cs.Unknown);



            //            DM.ARAY[ri.BPCK].SetStat(cs.None );      

            Close();
            
    
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ForContext.Frm);

            Close();   
        }

        private void FormLotOpen_Shown(object sender, EventArgs e)
        {
            UpdateLotList();
        }

        private void btAdd_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ForContext.Frm);

            LOT.TLot Lot ;
            Lot.sLotNo      = tbLotNo     .Text.Trim();

            //LOT.LotList.Add(Lot);

            UpdateLotList();

            Log.Trace("Lot Add : " + Lot.sLotNo , ForContext.Frm);

            tbLotNo     .Text = "";
        }

        private void btDelete_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ForContext.Frm);

            //if (LOT.LotList.Count != 0 && lvLot.SelectedIndices[0] != 0)
            //{
            //    LOT.LotList.Remove(LOT.LotList[lvLot.SelectedIndices[0]]);
            //}


            UpdateLotList();
        }

        private void tbEmployeeID_KeyDown(object sender, KeyEventArgs e)
        {
           
        }

        private void tbMaterialNo_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                tbLotNo.Focus();
            }
        }

        private void tbLotNo_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                btLotOpen.Focus();
            }
        }

        private void tbTargetBin_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                //btAdd_Click(sender , e);
                //tbMaterialNo.Focus();
                btLotOpen.Focus();
            }
        }

        private void FormLotOpen_Load(object sender, EventArgs e)
        {

        }

        private void FormLotOpen_VisibleChanged(object sender, EventArgs e)
        {
            if (LOT.GetLotOpen())
            {
                tbLotNo.Text = LOT.GetLotNo();
                tbWorkCount.Text = OM.EqpStat.iTotalWorkCount.ToString();
                tbLotNo    .Enabled = false;
                tbWorkCount.Enabled = false;
                btLotOpen  .Enabled = false;
                btRework   .Enabled = false;
            }
            else
            {
                tbLotNo    .Enabled = true;
                tbWorkCount.Enabled = true;
                btLotOpen  .Enabled = true;
                btRework   .Enabled = true;
            }
        }

        private void btRework_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ForContext.Frm);

            if (tbLotNo.Text != "") return;// tbLotId.Text = DateTime.Now.ToString("HHmmss");

            string LotNo = tbLotNo.Text.Trim();
            string Device = tbSelDevice.Text.Trim();

            LOT.TLot Lot;

            tbLotNo.Text = DateTime.Now.ToString("yyyyMMdd") + "_Rework_" + OM.EqpStat.iReworkCount.ToString();
            Lot.sLotNo = tbLotNo.Text.Trim();
            tbWorkCount.Text = 999.ToString();
            if (!int.TryParse(tbWorkCount.Text, out OM.EqpStat.iTotalWorkCount))
            {
                ML.ER_SetErr(ei.ETC_LotOpen, "워크 카운트 숫자입력이 잘못되었습니다.");
                return;
            }
            OM.EqpStat.iCrntWorkCount = 0;
            OM.EqpStat.iHghtNG = 0;
            OM.EqpStat.iGoodCount = 0;

            LOT.LotOpen(Lot);

            DM.ARAY[ri.STT].SetStat(cs.None);
            DM.ARAY[ri.PRE].SetStat(cs.None);
            DM.ARAY[ri.ADJ].SetStat(cs.None);
            DM.ARAY[ri.PST].SetStat(cs.None);

            OM.EqpStat.iReworkCount++;

            //FrmOperation.


            //Log.Trace("LotOpen", "Try");

            //CDelayTimer TimeOut = new CDelayTimer();
            //TimeOut.Clear();
            //SEQ.Visn.SendLotStart(Lot.sLotNo);
            //while(!SEQ.Visn.GetSendCycleEnd(VisnCom.vs.LotStart  )){
            //    Thread.Sleep(1);
            //    if(TimeOut.OnDelay(5000)) { 
            //        SM.ER_SetErr(ei.VSN_ComErr,"Lot Start TimeOut");
            //        break;
            //    }
            //}
            //DM.ARAY[ri.LODR].SetStat(cs.Unknown);



            //            DM.ARAY[ri.BPCK].SetStat(cs.None );      

            Close();
        }
    }
}
