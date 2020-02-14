using COMMON;
using SML2;
using System;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace Machine
{
    public partial class FormLotOpen : Form
    {
        public static FormLotOpen FrmLotOpen;

        public FormLotOpen()
        {
            InitializeComponent();

            
            tbSelDevice.Text = OM.GetCrntDev();
            tbLotNo     .Text = "";
            tbEmployeeID.Text = ""; //20180125 SML.FrmLogOn.GetId();
            tbMaterialNo.Text = "";
            tbLotAlias  .Text = "";

            tbLotNo.Focus();
            
        }

        private void UpdateLotList()
        {
            lvLot.Clear();
            lvLot.View = View.Details;
            lvLot.LabelEdit = true;
            lvLot.AllowColumnReorder = true;
            lvLot.FullRowSelect = true;
            lvLot.GridLines = true;
            //lvLot.Sorting = SortOrder.Descending;
            lvLot.Scrollable = true;

            Type type = typeof(LOT.TLot);
            int iCntOfItem = type.GetProperties().Length;
            FieldInfo[] f = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            //컬럼추가 하고 이름을 넣는다.
            lvLot.Columns.Add("No", 30, HorizontalAlignment.Left);
            for (int c = 0; c < f.Length; c++)
            {
                lvLot.Columns.Add(f[c].Name, 90, HorizontalAlignment.Left);
            }


            lvLot.Items.Clear();
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

        private bool DeviceChange(string _sDevName)
        {
            bool bRet = true ;
            if (!OM.LoadJobFile(_sDevName))
            {
                return false ;
            }
            SEQ.Visn.SendJobChange(_sDevName);                                   
            PM.Load(_sDevName);                                                  
            CConfig Config = new CConfig();
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevOptnPath = sExeFolder + "JobFile\\" + _sDevName + "\\TrayMask.ini";
            Config.Load(sDevOptnPath, CConfig.EN_CONFIG_FILE_TYPE.ftIni);
            DM.ARAY[ri.MASK].Load(Config, true);

            CDelayTimer TimeOut = new CDelayTimer();
            TimeOut.Clear();
            while(!SEQ.Visn.GetSendCycleEnd(VisnCom.vs.JobChange  )){
                Thread.Sleep(1);
                if(TimeOut.OnDelay(8000)) { 
                    Log.ShowMessage("Vision" , "Device Change TimeOut");
                    bRet = false ;
                    break;
                }
            }

            PM.UpdatePstn(true);

            DM.ARAY[ri.SPLR].SetMaxColRow(1                        , 1                           );
            DM.ARAY[ri.IDXR].SetMaxColRow(OM.DevInfo.iTRAY_PcktCntX, OM.DevInfo.iTRAY_PcktCntY   );
            DM.ARAY[ri.IDXF].SetMaxColRow(OM.DevInfo.iTRAY_PcktCntX, OM.DevInfo.iTRAY_PcktCntY   );
            DM.ARAY[ri.PCKR].SetMaxColRow(1                        , 1                           );
            DM.ARAY[ri.TRYF].SetMaxColRow(OM.DevInfo.iTRAY_PcktCntX, OM.DevInfo.iTRAY_PcktCntY   );
            DM.ARAY[ri.TRYG].SetMaxColRow(OM.DevInfo.iTRAY_PcktCntX, OM.DevInfo.iTRAY_PcktCntY   );
            DM.ARAY[ri.OUTZ].SetMaxColRow(1                        , 1                           );
            DM.ARAY[ri.STCK].SetMaxColRow(1                        , OM.DevInfo.iTRAY_StackingCnt);
            DM.ARAY[ri.BARZ].SetMaxColRow(1                        , 1                           );
            DM.ARAY[ri.INSP].SetMaxColRow(1                        , OM.DevInfo.iTRAY_PcktCntY   );
            DM.ARAY[ri.PSTC].SetMaxColRow(1                        , 1                           );
            DM.ARAY[ri.MASK].SetMaxColRow(OM.DevInfo.iTRAY_PcktCntX, OM.DevInfo.iTRAY_PcktCntY   );

            DM.ARAY[ri.TRYF].SetStat(cs.Empty);
            DM.ARAY[ri.TRYG].SetStat(cs.Good );
            DM.ARAY[ri.STCK].SetStat(cs.Empty);
            DM.ARAY[ri.INSP].SetStat(cs.Good );

            DM.ARAY[ri.IDXR].SetMask(DM.ARAY[ri.MASK]);
            DM.ARAY[ri.IDXF].SetMask(DM.ARAY[ri.MASK]);
            DM.ARAY[ri.TRYF].SetMask(DM.ARAY[ri.MASK]);
            DM.ARAY[ri.TRYG].SetMask(DM.ARAY[ri.MASK]);

            return bRet ; 
        }

        private void btLotOpen_Click(object sender, EventArgs e)
        {
            if (tbLotNo.Text == "") {
                Log.ShowMessage("Error", "Lot No is Empty" );
                return;// tbLotId.Text = DateTime.Now.ToString("HHmmss");
            }
            if (tbMaterialNo.Text == "") {
                Log.ShowMessage("Error", "Material No is Empty" );
                return;// tbLotId.Text = DateTime.Now.ToString("HHmmss");
            }
            if (tbLotAlias.Text == "") {
                Log.ShowMessage("Error", "Lot Alias is Empty" );
                return;// tbLotId.Text = DateTime.Now.ToString("HHmmss");
            }
            if (tbEmployeeID.Text == "") {
                Log.ShowMessage("Error", "EmployeeID is Empty" );
                return;// tbLotId.Text = DateTime.Now.ToString("HHmmss");
            }





            if (!SM.IO_GetX(xi.VISN_Ready)) {
                Log.ShowMessage("Vision", SM.IO_GetXName(xi.VISN_Ready) + "Vision Ready IO is not On" );
                return ;
            }
            OM.EqpStat.bWrapingEnd = false ;

            Log.Trace("LotOpen", "Try");

            string LotNo  = tbLotNo.Text.Trim();
            string Device = tbSelDevice.Text.Trim();

            CDelayTimer TimeOut = new CDelayTimer();
            TimeOut.Clear();
            SEQ.Visn.SendLotStart(LotNo);
            while(!SEQ.Visn.GetSendCycleEnd(VisnCom.vs.LotStart  )){
                Thread.Sleep(1);
                if(TimeOut.OnDelay(5000)) { 
                    Log.ShowMessage("Vision" , "Lot Start TimeOut");                    
                    return ;
                }
            }

            OM.EqpStat.iWorkBundle = 0 ;

            LOT.TLot Lot ;
            Lot.sEmployeeID = tbEmployeeID.Text.Trim();
            Lot.sLotNo      = tbLotNo     .Text.Trim();
            Lot.sMaterialNo = tbMaterialNo.Text.Trim();
            Lot.sLotAlias   = tbLotAlias  .Text.Trim();
            LOT.LotOpen(Lot);            

            OM.EqpStat.sLotSttTime = DateTime.Now.ToString("HH:mm:ss");
            if (!OM.CmnOptn.bOracleNotUse && !OM.CmnOptn.bIdleRun)
            {                                         
                if (SEQ.Oracle.ProcessLotOpen(Lot.sLotNo , Lot.sMaterialNo, Lot.sLotAlias))
                {                    
                    if (!DeviceChange(SEQ.Oracle.Stat.sVisionRecipe_RecipeName))
                    {
                        Log.ShowMessage("Device" , "'"+SEQ.Oracle.Stat.sVisionRecipe_RecipeName+"'" + "dosn't exist!" );
                    }
                }
                else
                {                 
                    Log.ShowMessage("Oracle" , SEQ.Oracle.GetLastMsg());
                    return ;
                }        
                //유닛아이디 리스트 만들기 시간 오래 걸려서 
                //별도 스레드 만듬.
                SEQ.Oracle.ThreadMakeUnitIDDMC1List();
            }
            else
            {
                //Device Change에서 함.
                DM.ARAY[ri.SPLR].SetStat(cs.None );
                DM.ARAY[ri.IDXR].SetStat(cs.None );
                DM.ARAY[ri.IDXF].SetStat(cs.None );
                DM.ARAY[ri.PCKR].SetStat(cs.None );
                DM.ARAY[ri.TRYF].SetStat(cs.None );
                DM.ARAY[ri.TRYG].SetStat(cs.Good );
                DM.ARAY[ri.OUTZ].SetStat(cs.None );
                DM.ARAY[ri.STCK].SetStat(cs.Empty);
                DM.ARAY[ri.BARZ].SetStat(cs.None );
                DM.ARAY[ri.INSP].SetStat(cs.Good );
                DM.ARAY[ri.PSTC].SetStat(cs.None );
                Close();
            }    

            //if (Log.ShowMessageModal("Confirm", "Do you want to All Homming?") != DialogResult.Yes) ;
            
            //20180305 오스람 요청... 랏오픈시에 올홈.
            MM.SetManCycle(mc.AllHome);
            
        }

        private void tmMakeDMC1_Tick(object sender, EventArgs e)
        {
            if (lbConDB.Visible != COracle.bMakingDMC1List) {
                if (COracle.bMakingDMC1List)//보내기 시작.
                {
                    
                }
                else//보내기 끝.
                {
                    if(!COracle.bMakeDMC1ListRet){
                        Log.ShowMessage("Oracle MakeUnitID_DMC1List Err ",SEQ.Oracle.GetLastMsg());
                    }
                    else//성공.
                    {
                        Log.ShowMessage("Oracle MakeUnitID_DMC1List Success!",SEQ.Oracle.GetLastMsg());
                        Close();
                    }
                }
                
                lbConDB.Visible = COracle.bMakingDMC1List ;
                
            }
            lbConDB.ForeColor = SEQ._bFlick ? System.Drawing.Color.Black : System.Drawing.Color.Lime;
        }

        //static bool bDBReading = false ;
        //public static void MakeUnitID()
        //{
        //    bDBReading = true ;
        //    if(!SEQ.Oracle.MakeUnitID_DMC1List()) {
        //        Log.ShowMessage("Oracle Err","MakeUnitID_DMC1List Failed!-"+SEQ.Oracle.GetLastMsg());
        //    }
        //    else
        //    {

        //    }
        //        //Log.ShowMessage("Oracle","MakeUnitIDList OK!");


        //    bDBReading = false ;         
        //}

        private void btCancel_Click(object sender, EventArgs e)
        {
            Close();   
        }

        private void FormLotOpen_Shown(object sender, EventArgs e)
        {
            UpdateLotList();
            tbLotNo.Focus();
        }

        private void btAdd_Click(object sender, EventArgs e)
        {
            LOT.TLot Lot ;

            Lot.sMaterialNo = tbMaterialNo.Text.Trim();
            Lot.sEmployeeID = tbEmployeeID.Text.Trim();
            Lot.sLotNo      = tbLotNo     .Text.Trim();
            Lot.sLotAlias   = tbLotAlias  .Text.Trim();

            //LOT.LotList.Add(Lot);

            UpdateLotList();

            Log.Trace("Lot Add", Lot.sLotNo );

            tbMaterialNo.Text = "";
            tbLotNo     .Text = "";
            tbLotAlias  .Text = "";

            tbLotNo.Focus();



        }

        private void btDelete_Click(object sender, EventArgs e)
        {
            //if (LOT.LotList.Count != 0 && lvLot.SelectedIndices[0] != 0)
            //{
            //    LOT.LotList.Remove(LOT.LotList[lvLot.SelectedIndices[0]]);
            //}
            
            
            UpdateLotList();
        }

        private void tbLotNo_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                tbMaterialNo.Focus();
            }
        }

        private void tbMaterialNo_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                tbLotAlias.Focus();
            }
        }

        private void tbLotAlias_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                tbEmployeeID.Focus();
            }
        }

        private void tbEmployeeID_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                btLotOpen.Focus();
            }
        }

        private void FormLotOpen_VisibleChanged(object sender, EventArgs e)
        {
            this.Left = 0 ;
            this.Top = 0 ;
            tbSelDevice .Text = OM.GetCrntDev();
            tbEmployeeID.Text = "";//LOT.CrntLotData.sEmployeeID ; //20180125 SML.FrmLogOn.GetId();
            tbLotNo     .Text = "";//"HZ7170CU.98";            
            tbMaterialNo.Text = "";//"11082611";
            tbLotAlias  .Text = "";//"5R-ebxD68-0";

            tbLotNo.Focus();

            lbConDB .Visible = false ;
        }

        private void btClearLotNo_Click(object sender, EventArgs e)
        {
            tbLotNo.Text = "";
        }

        private void btClearMaterialNo_Click(object sender, EventArgs e)
        {
            tbMaterialNo.Text = "";
        }

        private void btLotAlias_Click(object sender, EventArgs e)
        {
            tbLotAlias.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tbEmployeeID.Text = "";
        }

        

       

        
    }
}
