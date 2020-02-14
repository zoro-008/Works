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

        public FormLotOpen()
        {
            InitializeComponent();

            
            
            tbSelDevice.Text = OM.GetCrntDev();
            tbLotNo     .Text = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            tbEmployeeID.Text = SM.FrmLogOn.GetId();

            tbLotNo.Focus();
            
        }

        //private void UpdateLotList()
        //{
        //    lvLot.Clear();
        //    lvLot.View = View.Details;
        //    lvLot.LabelEdit = true;
        //    lvLot.AllowColumnReorder = true;
        //    lvLot.FullRowSelect = true;
        //    lvLot.GridLines = true;
        //    //lvLot.Sorting = SortOrder.Descending;
        //    lvLot.Scrollable = true;
        //
        //    Type type = typeof(LOT.TLot);
        //    int iCntOfItem = type.GetProperties().Length;
        //    FieldInfo[] f = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        //
        //    //컬럼추가 하고 이름을 넣는다.
        //    lvLot.Columns.Add("No", 30, HorizontalAlignment.Left);
        //    for (int c = 0; c < f.Length; c++)
        //    {
        //        lvLot.Columns.Add(f[c].Name, 90, HorizontalAlignment.Left);
        //    }
        //
        //
        //    lvLot.Items.Clear();
        //    string sValue = "";
        //    string sName = "";
        //    //ListViewItem[] liitem = new ListViewItem[LOT.LotList.Count];
        //    //for (int r = 0; r < LOT.LotList.Count; r++)
        //    //{
        //    //    liitem[r] = new ListViewItem(string.Format("{0}", r));
        //    //    for (int c = 0; c < f.Length; c++)
        //    //    {
        //    //        sName = f[c].Name;
        //    //        sValue = f[c].GetValue(LOT.LotList[r]).ToString();
        //    //        liitem[r].SubItems.Add(sValue);
        //    //    }
        //    //    liitem[r].UseItemStyleForSubItems = false;
        //    //    lvLot.Items.Add(liitem[r]);
        //    //}
        //    //lvLot.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        //
        //}

        private void btLotOpen_Click(object sender, EventArgs e)
        {
            if (tbLotNo.Text == "") return;// tbLotId.Text = DateTime.Now.ToString("HHmmss");

            if (OM.DevInfo.iMacroType == 0 && OM.CmnOptn.sDressyPath == "")
            {
                ML.ER_SetErr(ei.ETC_OptnSet, "Dressy 시리얼 넘버 csv 파일 경로가 설정되지 않았습니다.");
                return;
            }
            else if (OM.DevInfo.iMacroType == 1 && OM.CmnOptn.sEzSensorPath == "")
            {
                ML.ER_SetErr(ei.ETC_OptnSet, "EzSensor 시리얼 넘버 csv 파일 경로가 설정되지 않았습니다.");
                return;
            }

            //SEQ.XRAY.GetSerialNo();
            //OM.EqpStat.iFrstSerialCnt = 0;
            //OM.EqpStat.iLastSerialCnt = 0;
            //SEQ.XRAY.iSerialCnt = 0;
            //int iCnt = 300;
            //if (SEQ.XRAY.iSerialCnt >= iCnt)
            //{
            //    ML.ER_SetErr(ei.ETC_LotOpen, "시리얼넘버가 자재 수량보다 많습니다.");
            //    return;
            //}

            string LotNo  = tbLotNo.Text.Trim();
            string Device = tbSelDevice.Text.Trim();

            LOT.TLot Lot ;

            Lot.sEmployeeID = tbEmployeeID.Text.Trim();
            Lot.sLotNo      = tbLotNo     .Text.Trim();

            LOT.LotOpen(Lot);

            OM.EqpStat.iLDRSplyCnt = 0 ;



            Log.Trace("LotOpen", "Try");

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
            DM.ARAY[ri.INDX].SetStat(cs.None   );
            DM.ARAY[ri.LODR].SetStat(cs.Unknown);

            ML.IO_SetY(yi.ETC_ColdGunOnOff, true);

            OM.EqpStat.sYear  = DateTime.Now.ToString("yyyy");
            OM.EqpStat.sMonth = DateTime.Now.ToString("MM"  );
            OM.EqpStat.sDay   = DateTime.Now.ToString("dd"  );
            


            Close();
            
    
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            Close();   
        }

        private void FormLotOpen_Shown(object sender, EventArgs e)
        {
            //UpdateLotList();
            tbLotNo.Focus();
        }

        private void btAdd_Click(object sender, EventArgs e)
        {
            LOT.TLot Lot ;

            Lot.sEmployeeID = tbEmployeeID.Text.Trim();
            Lot.sLotNo      = tbLotNo     .Text.Trim();

            //LOT.LotList.Add(Lot);

            //UpdateLotList();

            Log.Trace("Lot Add", Lot.sLotNo );

            tbLotNo     .Text = "";

            tbLotNo     .Focus();



        }

        private void btDelete_Click(object sender, EventArgs e)
        {
            //if (LOT.LotList.Count != 0 && lvLot.SelectedIndices[0] != 0)
            //{
            //    LOT.LotList.Remove(LOT.LotList[lvLot.SelectedIndices[0]]);
            //}
            
            
            //UpdateLotList();
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
                tbLotNo.Focus();
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



    }
}
