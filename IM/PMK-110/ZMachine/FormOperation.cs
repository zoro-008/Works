using System;
using System.Drawing;
using System.Windows.Forms;
using COMMON;
using SML;
using System.Reflection;

namespace Machine
{
    public partial class FormOperation : Form
    {
        public static FormOperation FrmOperation;
        public static FormLotOpen FrmLotOpen;
        //public FormPassword FrmPassword;
        public static FormMain FrmMain;

        public int LotInfoCnt = 6;
        public int DayInfoCnt = 6;

        //public EN_SEQ_STAT m_iSeqStat;

        public FormOperation(Panel _pnBase)
        {
            InitializeComponent();

            //FrmPassword = new FormPassword();

            this.TopLevel = false;
            this.Parent = _pnBase;

            DispDayList();
            DispLotInfo();

            pnPassWord.Visible = false;
            tmUpdate.Enabled = true;

            //btStart.Enabled = LotUnit.GetLotOpen();

            DM.ARAY[(int)ri.PCK].SetParent(pnPCK); DM.ARAY[(int)ri.PCK].Name = "PCK";
            DM.ARAY[(int)ri.PRI].SetParent(pnPRI); DM.ARAY[(int)ri.PRI].Name = "PRI";
            DM.ARAY[(int)ri.MRK].SetParent(pnMRK); DM.ARAY[(int)ri.MRK].Name = "WRK";
            DM.ARAY[(int)ri.BAR].SetParent(pnBAR); DM.ARAY[(int)ri.BAR].Name = "BAR";


            //Loader Array
            DM.ARAY[(int)ri.PCK].SetDispColor(cs.None   , Color.White  ); DM.ARAY[(int)ri.PCK].SetDispName(cs.None   , "Not Exsist"); DM.ARAY[(int)ri.PCK].SetVisible(cs.None    , true);
            DM.ARAY[(int)ri.PCK].SetDispColor(cs.Unkwn  , Color.Aqua   ); DM.ARAY[(int)ri.PCK].SetDispName(cs.Unkwn  , "UnKnown"   ); DM.ARAY[(int)ri.PCK].SetVisible(cs.Unkwn   , true);
                                                                                                                                                                                 
            //PreBuffer Array                                                                                                                                                    
            DM.ARAY[(int)ri.PRI].SetDispColor(cs.None   , Color.White  ); DM.ARAY[(int)ri.PRI].SetDispName(cs.None   , "Not Exsist"); DM.ARAY[(int)ri.PRI].SetVisible(cs.None    , true);
            DM.ARAY[(int)ri.PRI].SetDispColor(cs.Unkwn  , Color.Aqua   ); DM.ARAY[(int)ri.PRI].SetDispName(cs.Unkwn  , "UnKnown"   ); DM.ARAY[(int)ri.PRI].SetVisible(cs.Unkwn   , true);
                                                                                                                                                                                 
            //MARK Array                                                                                                                                                         
            DM.ARAY[(int)ri.MRK].SetDispColor(cs.None   , Color.White  ); DM.ARAY[(int)ri.MRK].SetDispName(cs.None   , "Not Exsist"); DM.ARAY[(int)ri.MRK].SetVisible(cs.None    , true);
            DM.ARAY[(int)ri.MRK].SetDispColor(cs.Unkwn  , Color.Aqua   ); DM.ARAY[(int)ri.MRK].SetDispName(cs.Unkwn  , "UnKnown"   ); DM.ARAY[(int)ri.MRK].SetVisible(cs.Unkwn   , true);
            DM.ARAY[(int)ri.MRK].SetDispColor(cs.Mark   , Color.Green  ); DM.ARAY[(int)ri.MRK].SetDispName(cs.Mark   , "Mark"      ); DM.ARAY[(int)ri.MRK].SetVisible(cs.Mark    , true);

            //Barcode Array                                                                                                                                                         
            DM.ARAY[(int)ri.BAR].SetDispColor(cs.None   , Color.White  ); DM.ARAY[(int)ri.BAR].SetDispName(cs.None   , "Not Exsist"); DM.ARAY[(int)ri.BAR].SetVisible(cs.None    , true);
            DM.ARAY[(int)ri.BAR].SetDispColor(cs.Mark   , Color.Green  ); DM.ARAY[(int)ri.BAR].SetDispName(cs.Mark   , "Mark"      ); DM.ARAY[(int)ri.BAR].SetVisible(cs.Mark    , true);                                                                                                           
            DM.ARAY[(int)ri.BAR].SetDispColor(cs.Good   , Color.Lime   ); DM.ARAY[(int)ri.BAR].SetDispName(cs.Good   , "Good"      ); DM.ARAY[(int)ri.BAR].SetVisible(cs.Good    , true);
                                                                                                                                                                                
                                                                                                                                                                                 
            DM.ARAY[(int)ri.PCK].SetMaxColRow(1, 1);
            DM.ARAY[(int)ri.PRI].SetMaxColRow(1, 1);
            DM.ARAY[(int)ri.MRK].SetMaxColRow(1, 1);
            DM.ARAY[(int)ri.BAR].SetMaxColRow(1, 1);
        
            DM.LoadMap();
            //DM.ARAY[(int)ri.SLD].ChangeStat(cs.None , cs.Unkwn);
        }

        public int m_iLevel;

        private void btOperator_Click(object sender, EventArgs e)
        {
            pnPassWord.Visible = true;
        }

        //사용자 레벨 버튼 클릭 이벤트
        private void btOper_Click(object sender, EventArgs e)
        {
            SM.FrmPassword.SetLevel(EN_LEVEL.Operator);
            pnPassWord.Visible = false;
        }

        private void btEngr_Click(object sender, EventArgs e)
        {
            SM.FrmPassword.ShowPage(EN_LEVEL.Engineer);
            SM.FrmPassword.Show();

            pnPassWord.Visible = false;
        }

        private void btMast_Click(object sender, EventArgs e)
        {
            SM.FrmPassword.ShowPage(EN_LEVEL.Master);
            SM.FrmPassword.Show();

            pnPassWord.Visible = false;
        }

        private void btPasswordClose_Click(object sender, EventArgs e)
        {
            pnPassWord.Visible = false;
        }

        private void btLotEnd_Click(object sender, EventArgs e)
        {
            SPC.LOT.SaveDataIni();
            LOT.LotEnd();
        }


        private static bool bPreLotOpen = false;
        private static int iTrayImg = 0;

        private void timer1_Tick(object sender, EventArgs e)
        {
            tmUpdate.Enabled = false;
            //pnLDR.Refresh();

            lbLaserCom.Visible = !SEQ.Laser.GetCycleEnded();

            btLotOpen.Enabled = !LOT.GetLotOpen() && SEQ.Laser.GetCycleEnded();
            btLotEnd.Enabled  =  LOT.GetLotOpen();

            btStart.Enabled = LOT.GetLotOpen() && SM.MTR.GetHomeDoneAll() && SEQ.Laser.GetCycleEnded();

            tbBarcode.Text = SEQ.Barcode.GetReadingText();

            int iSolder = 1;
            int iClean  = 2;
//            double dSolderTime = SEQ.SLD.GetCycleTime(iSolder);
//            double dClean      = SEQ.SLD.GetCycleTime(iClean);

            int iLevel = (int)SM.FrmPassword.GetLevel();
            //어플리케이션 개조 하면서 신형 DLL에 로그오프가 추가 되서 않맞음.
            if(SM.FrmPassword.GetLevel() == EN_LEVEL.LogOff) SM.FrmPassword.SetLevel(EN_LEVEL.Operator);
            switch (iLevel)
            {
                case (int)EN_LEVEL.Operator: btOperator.Text = "OPERATOR"; break;
                case (int)EN_LEVEL.Engineer: btOperator.Text = "ENGINEER"; break;
                case (int)EN_LEVEL.Master  : btOperator.Text = " ADMIN  "; break;
                default                    : btOperator.Text = " ERROR  "; break;
            }
            
            if (bPreLotOpen != LOT.GetLotOpen())
            {
                btStart.Enabled = LOT.GetLotOpen();
                bPreLotOpen = LOT.GetLotOpen();
            }


            SPC.LOT.DispLotInfo(lvLotInfo);
            SPC.DAY.DispDayInfo(lvDayInfo);

            string Str      ;
            int iPreErrCnt  = 0;
            int iCrntErrCnt = 0;
            for (int i = 0 ; i < SM.ERR._iMaxErrCnt ; i++) 
            {
                if (SM.ERR.GetErr(i)) iCrntErrCnt++;
            }
            if (iPreErrCnt != iCrntErrCnt ) 
            {
                lbErr.Items.Clear();
                int iErrNo = SM.ERR.GetLastErr();
                for (int i = 0; i < SM.ERR._iMaxErrCnt; i++) 
                {
                    if (SM.ERR.GetErr(i))
                    {
                        Str = string.Format("[ERR{0:000}]" , i) ;
                        Str += SM.ERR.GetErrName(i) + " " + SM.ERR.GetErrMsg(i);
                        lbErr.Items.Add(Str);
                    }
                }
            }
            if (SEQ._iSeqStat != EN_SEQ_STAT.Error)
            {
                lbErr.Items.Clear();
            }
            iPreErrCnt = iCrntErrCnt ;

            
            //Door Sensor.  나중에 찾아보자
            //bool isAllCloseDoor = SM.IO.GetX((int)EN_INPUT_ID.xETC_DoorFt) &&
            //                      SM.IO.GetX((int)EN_INPUT_ID.xETC_DoorLt) &&
            //                      SM.IO.GetX((int)EN_INPUT_ID.xETC_DoorRt) &&
            //                      SM.IO.GetX((int)EN_INPUT_ID.xETC_DoorRr) ;
            //if (FormPassword.GetLevel() != EN_LEVEL.lvOperator && isAllCloseDoor && CMachine._bRun)
            //{
            //    //FM_SetLevel(lvOperator);
            //}
            
            if(!SM.MTR.GetHomeDoneAll()){
                btAllHome.ForeColor = SEQ._bFlick ? Color.Black : Color.Red;
            }
            else {
                btAllHome.ForeColor = Color.Black  ;
            }


            //Option View
            if(OM.CmnOptn.bIgnrDoor   ) {pnIgnrDoor.BackColor = Color.Lime; lbIgnrDoor.Text = "ON"; }
            else                        {pnIgnrDoor.BackColor = Color.Red ; lbIgnrDoor.Text = "OFF";}
            if(OM.CmnOptn.bMarkSkip   ) {pnSkipMark.BackColor = Color.Lime; lbSkipMark.Text = "ON"; }
            else                        {pnSkipMark.BackColor = Color.Red ; lbSkipMark.Text = "OFF";}
            //if(OM.CmnOptn.bAirBlwrSkip) {pnSkipAir .BackColor = Color.Lime; lbSkipAir .Text = "ON"; }
            //else                        {pnSkipAir .BackColor = Color.Red ; lbSkipAir .Text = "OFF";}
            if(OM.CmnOptn.bBarSkip    ) {pnSkipBar .BackColor = Color.Lime; lbSkipBar .Text = "ON"; }
            else                        {pnSkipBar .BackColor = Color.Red ; lbSkipBar .Text = "OFF";}

            tbMarkNo.Text = OM.EqpStat.iSerialNo.ToString();
            if(tbChangeMarkNo.Text == "") tbChangeMarkNo.Text = "0";

            Refresh();
            tmUpdate.Enabled = true;
        }

        private void btLotOpen_Click(object sender, EventArgs e)
        {
            FrmLotOpen = new FormLotOpen();
            FrmLotOpen.Show();
        }

        //public void DispLotInfo(ListView _lvLotInfo) //오퍼레이션 창용.
        public void DispLotInfo() //오퍼레이션 창용.
        {
            lvLotInfo.Clear();
            lvLotInfo.View = View.Details;
            lvLotInfo.LabelEdit = true;
            lvLotInfo.AllowColumnReorder = true;
            lvLotInfo.FullRowSelect = true;
            lvLotInfo.GridLines = true;
            lvLotInfo.Sorting = SortOrder.Descending;
            lvLotInfo.Scrollable = true;

            lvLotInfo.Columns.Add("", 105, HorizontalAlignment.Left);     //1280*1024는 109, 1024*768은 78
            lvLotInfo.Columns.Add("", 105, HorizontalAlignment.Left);     //1280*1024는 109, 1024*768은 78
        
            ListViewItem[] liLotInfo = new ListViewItem[LotInfoCnt];
        
            for (int i = 0; i < LotInfoCnt; i++)
            {
                liLotInfo[i] = new ListViewItem();
                liLotInfo[i].SubItems.Add("");
        
        
                liLotInfo[i].UseItemStyleForSubItems = false;
                liLotInfo[i].UseItemStyleForSubItems = false;

                lvLotInfo.Items.Add(liLotInfo[i]);
                
            }
        
            var PropLotInfo = lvLotInfo.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropLotInfo.SetValue(lvLotInfo, true, null);
        }

         
        public void DispDayList() //오퍼레이션 창용.
        {
            lvDayInfo.Clear();
            lvDayInfo.View = View.Details;
            lvDayInfo.LabelEdit = true;
            lvDayInfo.AllowColumnReorder = true;
            lvDayInfo.FullRowSelect = true;
            lvDayInfo.GridLines = true;
            lvDayInfo.Sorting = SortOrder.Descending;
            lvDayInfo.Scrollable = true;

            lvDayInfo.Columns.Add("", 105, HorizontalAlignment.Left);    //1280*1024는 109, 1024*768은 78
            lvDayInfo.Columns.Add("", 105, HorizontalAlignment.Left);    //1280*1024는 109, 1024*768은 78
            
            ListViewItem[] liDayInfo = new ListViewItem[DayInfoCnt];
            
            for (int i = 0; i < DayInfoCnt; i++)
            {
                liDayInfo[i] = new ListViewItem();
                liDayInfo[i].SubItems.Add("");


                liDayInfo[i].UseItemStyleForSubItems = false;
                liDayInfo[i].UseItemStyleForSubItems = false;

                lvDayInfo.Items.Add(liDayInfo[i]);
                
            }
            
            var PropDayInfo = lvDayInfo.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropDayInfo.SetValue(lvDayInfo, true, null);
            
            if (lvDayInfo == null) return;
        }

        private void btStart_Click(object sender, EventArgs e)
        {
            SEQ._bBtnStart = true;
            
        }

        private void btStop_Click(object sender, EventArgs e)
        {
            SEQ._bBtnStop = true;
            //for(int i = 0 ; i < 100 ; i++)
            //{
            //    Log.TraceListView("buttonStart"+ i.ToString());
            //}
        }

        private void btReset_Click(object sender, EventArgs e)
        {
            SEQ._bBtnReset = true;
            
        }

        private void lvDayInfo_MouseDoubleClick(object sender, MouseEventArgs e)  //요거는 확인 해봐야 함 진섭
        {
            if(SM.FrmPassword.GetLevel() != EN_LEVEL.Master) return ;

            if (Log.ShowMessageModal("Confirm", "Clear Day Info?") != DialogResult.Yes) return;
            
            DayData.ClearData() ;
        }

        private bool bHomeClick = false;
        private void btAllHome_Click(object sender, EventArgs e)
        {
            //    if(IO_GetX(xSTG_Vccm)){
        //        FM_MsgOk("Error" , "작업 Stage에 자제를 제거하여 주십시오");
        //        return ;
        //    }
            //if (MessageBox.Show(new Form{TopMost = true}, "전체 홈을 잡으시겠습니까?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question)!=DialogResult.Yes) return;

            if (Log.ShowMessageModal("Confirm", "Do you want to All Homming?") != DialogResult.Yes) return;

            MM.SetManCycle(mc.AllHome);
            bHomeClick = true;
        }
        bool bRepeat = false;
        private void lbWorkNo_Click(object sender, EventArgs e)
        {
            //bRepeat = !bRepeat; 
        }

        private void FormOperation_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmUpdate.Enabled = false;
            DM.SaveMap();
        }

        private void FormOperation_Shown(object sender, EventArgs e)
        {
//            EmbededExe.SetCamParent(pnCamera.Handle);
            //Refresh();
        }

        private void pnOption1_DoubleClick(object sender, EventArgs e)
        {
            int iBtnTag = Convert.ToInt32(((Panel)sender).Tag);
            switch (iBtnTag)
            {
                default : break;
                case 1: OM.CmnOptn.bIgnrDoor    = !OM.CmnOptn.bIgnrDoor   ; break;
                //case 2: OM.CmnOptn.bAirBlwrSkip = !OM.CmnOptn.bAirBlwrSkip; break;
                case 3: OM.CmnOptn.bMarkSkip    = !OM.CmnOptn.bMarkSkip   ; break;
                case 4: OM.CmnOptn.bBarSkip     = !OM.CmnOptn.bBarSkip    ; break;
            }
            OM.SaveCmnOptn();
        }

        private void lbOption1_DoubleClick(object sender, EventArgs e)
        {
            int iBtnTag = Convert.ToInt32(((Label)sender).Tag);
            switch (iBtnTag)
            {
                default : break;
                case 1: OM.CmnOptn.bIgnrDoor    = !OM.CmnOptn.bIgnrDoor   ; break;
                //case 2: OM.CmnOptn.bAirBlwrSkip = !OM.CmnOptn.bAirBlwrSkip; break;
                case 3: OM.CmnOptn.bMarkSkip    = !OM.CmnOptn.bMarkSkip   ; break;
                case 4: OM.CmnOptn.bBarSkip     = !OM.CmnOptn.bBarSkip    ; break;
            }
            OM.SaveCmnOptn();
        }

        private void btManual1_Click(object sender, EventArgs e)
        {
            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);
            MM.SetManCycle((mc)iBtnTag);
        }

        private void FormOperation_Load(object sender, EventArgs e)
        {
            Log.SetMessageListBox(lvInfo);
            Log.TraceListView("Program Started");

            //레이저 마킹기 껐다 키면 이거 한번 보내줘야 해서 보냄.
            SEQ.Laser.SetCycle(RS232_DominoDynamark3.Cycle.ProjectLoad, true); //에러는 안에서 띄움.
        }

        private void btManual2_Click(object sender, EventArgs e)
        {
            if (Log.ShowMessageModal("Confirm", "Do you want to Init Tray Cnt?") != DialogResult.Yes) return;
            Loader.m_iTrayWorkCnt = 0 ; 
            Log.TraceListView("Tray Cnt Init");
            SEQ.LDR.MoveMotr(mi.LDR_Z , PM.GetValue(mi.LDR_Z, pv.LDR_ZWait));
        }

        private void btBarOff_Click(object sender, EventArgs e)
        {
            SEQ.Barcode.Stop();
        }

        private void btBarOn_Click(object sender, EventArgs e)
        {
            SEQ.Barcode.Read();
        }

        private void label6_Click(object sender, EventArgs e)
        {
            //SPC.LOT.AddGoodCntData(1);
            //SPC.LOT.AddWorkCntData(2);
            //
            //SPC.LOT.SaveDataIni(DateTime.Now.ToOADate());
        }

        private void lbSkipBar_Click(object sender, EventArgs e)
        {
            
        }

        private void btMarkNoSet_Click(object sender, EventArgs e)
        {
            int iTemp = 0;
            if (!int.TryParse(tbChangeMarkNo.Text, out iTemp))
            {
                Log.ShowMessage("Error", "Please enter numbers only in Change Marking No.");
                return;
            }
            OM.EqpStat.iSerialNo = iTemp;
        }

        private void lvInfo_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //Log.Clear();
        }
    }

    public class DoubleBuffer : Panel
    {
        public DoubleBuffer()
        {
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            this.UpdateStyles();
        }
    }
    public class DoubleBufferP : PictureBox
    {
        public DoubleBufferP()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.UserPaint |
              ControlStyles.AllPaintingInWmPaint |
              ControlStyles.ResizeRedraw |
              ControlStyles.ContainerControl |
              ControlStyles.OptimizedDoubleBuffer |
              ControlStyles.SupportsTransparentBackColor
              , true);
        }
    }



}
