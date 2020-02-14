using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace Machine
{
    public partial class FormOracle : Form
    {
        
        public FormOracle()
        {
            InitializeComponent();
            //SEQ.Oracle.SetSendMsgFunc(SendListMsg);

            //gvTable.DataSource = SEQ.Oracle.Table ;
            //gvPanelIdTable.DataSource = SEQ.Oracle.PanelIDTable ;
        }
        public void SendListMsg(string _sMsg)
        {
            lvLog.Invoke(new OracleBase.FSendMsg(ListMsg), new string[]{_sMsg});
        }
        private void ListMsg(string _sMsg)
        {
            if (!lvLog.GridLines)
            {
                lvLog.View = View.Details;
                lvLog.GridLines = true;
                lvLog.Columns.Add("Message", lvLog.Size.Width, HorizontalAlignment.Left);
                var PropError = lvLog.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
                PropError.SetValue(lvLog, true, null);

            }

            lvLog.Items.Add(_sMsg);
            if(lvLog.Items.Count > 100 ){
                lvLog.Items.RemoveAt(0);
            }
            lvLog.Items[lvLog.Items.Count - 1].EnsureVisible();
        }


        ~FormOracle()
        {
            
        }



        private void button1_Click_1(object sender, EventArgs e)
        {


        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }


        private void btGetRecipe_Click(object sender, EventArgs e)
        {
            string sRecipe ="";
            if (!SEQ.Oracle.ProcessLotOpen(tbLotNumber.Text , tbMeterialNo.Text, tbLotAlias.Text))
            {
                MessageBox.Show("ProcessLotOpen return false-"+SEQ.Oracle.GetLastMsg());
            }

            //if(SEQ.Oracle.LoadTable())
        }



        private void btCheckTrayLabel_Click(object sender, EventArgs e)
        {
            //tbGroupingNo  .Text = SEQ.Oracle.GetGroupingNo  (tbTrayLabel.Text);
            //tbMeterialNo2 .Text = SEQ.Oracle.GetMeterialNo  (tbTrayLabel.Text);
            //tbBatchNo     .Text = SEQ.Oracle.GetBatchNo     (tbTrayLabel.Text);
            //tbSecurityCode.Text = SEQ.Oracle.GetSecurityCode(tbTrayLabel.Text);

            //if (!SEQ.Oracle.CheckTrayLabel(tbTrayLabel.Text , tbLotAlias.Text))
            //{
            //    MessageBox.Show("CheckTrayLabel return false");

            //}

        }

        private void btCheckUnitID_Click(object sender, EventArgs e)
        {
            //if (!SEQ.Oracle.CheckUnitID(tbUnitID.Text))
            //{
            //    MessageBox.Show("Failed!");
            //}
            //else
            //{
            //    MessageBox.Show("OK!");
            //}
        }

        private void btMakePanelIDTable_Click(object sender, EventArgs e)
        {

            //if (!SEQ.Oracle.MakeListPanelID(tbTrayLabel.Text , tbLotAlias.Text))
            //{
                
            //    MessageBox.Show("Failed!");
            //}
        }

        private void btMakeSupplyList_Click(object sender, EventArgs e)
        {
            //if (!SEQ.Oracle.MakeListDMC1(tbMeterialNo2.Text))
            //{
                

            //}
        }

        private void btCheckDMC1_Click(object sender, EventArgs e)
        {
            //if (!SEQ.Oracle.CheckDMC1(tbDMC1.Text))
            //{
            //    MessageBox.Show("Not Exist!");

            //}
            //else
            //{
            //    MessageBox.Show("Exist!");
            //}
        }

        private void btCheckDMC1Exist_Click(object sender, EventArgs e)
        {
            //string [] sDMC1 = {tbDMC1Exist.Text};
            //List<string> sDMC1Exist = new List<string>() ;
            //if (!SEQ.Oracle.GetExistDMC1(sDMC1 , ref sDMC1Exist))
            //{
            //    MessageBox.Show("GetExistDMC1 Failed!");

            //}
            //else
            //{
            //    if(sDMC1Exist.Count!=0) MessageBox.Show(sDMC1Exist[0] + " Exist!");
            //    else                    MessageBox.Show("Not Exist!");
            //}
        }

        private void btDeviceNoList_Click(object sender, EventArgs e)
        {
            //if (!SEQ.Oracle.MakeListDeviceNumber(tbMeterialNo2.Text))
            //{
            //    MessageBox.Show("MakeListDeviceNumber Failed!");
            //}
        }

        private void btCharacterCompare_Click(object sender, EventArgs e)
        {
            //if (!SEQ.Oracle.CheckDMC2CharacterCompare(tbDMC2_1.Text))
            //{
            //    MessageBox.Show("CheckDMC2CharacterCompare Failed!");

            //}
            //else
            //{
            //    MessageBox.Show("CheckDMC2CharacterCompare Pass!");
            //}
        }       

        

        private void btMakeDMC2Spec_Click(object sender, EventArgs e)
        {
            //if (!SEQ.Oracle.MakeDMC2Spec(tbT2_11SERIES.Text))
            //{
            //    MessageBox.Show("MakeListDeviceNumber Failed!");
            //}
        }
        private void btDMC2Type2_Click(object sender, EventArgs e)
        {
            //if (!SEQ.Oracle.CheckDMC2_Spec(tbDMC2_2.Text))
            //{
            //    MessageBox.Show("CheckDMC2_Spec Failed!");
            //}
            //else
            //{
            //    MessageBox.Show("CheckDMC2_Spec Passed!");
            //}
        }

        private void btAddQuery_Click(object sender, EventArgs e)
        {
            ////if(!SEQ.Oracle.AddUnitInspectionQuery(true , string _sLotNo , string _sTrayLabel, int _iPocketID , string _sUnitID , string _sDMC1 , string _sDMC2 , cs _eStat ,string _sMachineID , string _sT2_11_Series ))
            //cs Stat = cs.Good;
            //     if(cbStat_UI.Text == "Good")Stat=cs.Good;
            //else if(cbStat_UI.Text == "NG0" )Stat=cs.NG0 ;
            //else if(cbStat_UI.Text == "NG1" )Stat=cs.NG1 ;
            //else if(cbStat_UI.Text == "NG2" )Stat=cs.NG2 ;
            //else if(cbStat_UI.Text == "NG3" )Stat=cs.NG3 ;
            //else if(cbStat_UI.Text == "NG4" )Stat=cs.NG4 ;
            //else if(cbStat_UI.Text == "NG5" )Stat=cs.NG5 ;
            //else if(cbStat_UI.Text == "NG6" )Stat=cs.NG6 ;
            //else if(cbStat_UI.Text == "NG7" )Stat=cs.NG7 ;
            //else if(cbStat_UI.Text == "NG8" )Stat=cs.NG8 ;
            //else if(cbStat_UI.Text == "NG9" )Stat=cs.NG9 ;
            //else if(cbStat_UI.Text == "NG10")Stat=cs.NG10;
            //if(!SEQ.Oracle.AddUnitInspectionQuery(
            //                                      tbLotNumber_UI   .Text , 
            //                                      tbTrayLabel_UI   .Text , 
            //                                      tbPocketID_UI    .Text , 
            //                                      tbUnitID_UI      .Text ,
            //                                      tbDmc1_UI        .Text ,
            //                                      tbDmc2_UI        .Text ,
            //                                      Stat                   ,
            //                                      tbMachineID_UI   .Text ,
            //                                      tbT2_11_Series_UI.Text ))
            //{
            //    MessageBox.Show("AddUnitInspectionQuery Failed!");
            //}
            //else
            //{
            //    //MessageBox.Show("AddUnitInspectionQuery Success!");
            //}
        }

        private void btSendQuery_Click(object sender, EventArgs e)
        {
            //if (!SEQ.Oracle.SendUnitInspectionQuery())
            //{
            //    MessageBox.Show("SendUnitInspectionQuery Failed!");
            //}
            //else
            //{
            //    MessageBox.Show("SendUnitInspectionQuery Success!");
            //}

        }

        private void btClearQuery_Click(object sender, EventArgs e)
        {
            SEQ.Oracle.ClearUnitInspectionQuery();
        }

        private void button3_Click(object sender, EventArgs e)
        {            
            if (!SEQ.Oracle.GetNLS_DATABASE_PARAMETERS())
            {
                MessageBox.Show("SendUnitInspectionQuery Failed!");
            }
        }

        private void btInsertVIT_Click(object sender, EventArgs e)
        {
            //if(!SEQ.Oracle.InsertVIT(tbLotNumber_VIT  .Text , 
            //                         tbTrayLabel_VIT  .Text , 
            //                         tbMachinID_VIT   .Text , 
            //                         tbOperID_VIT     .Text ,
            //                         tbStartTime_VIT  .Text ,
            //                         tbStopTime_VIT   .Text ,
            //                         tbT2_11_Series_UI.Text ,
            //                         tbTrayQty_VIT    .Text ))
            //{
            //    MessageBox.Show("InsertVIT Failed!");
            //}
            //else
            //{
            //    MessageBox.Show("InsertVIT Success!");
            //}
        }



    }
}
