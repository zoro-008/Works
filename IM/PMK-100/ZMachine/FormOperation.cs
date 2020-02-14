using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using COMMON;
using SMDll2;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Collections;
using System.Runtime.InteropServices;

namespace Machine
{
    public partial class FormOperation : Form
    {
        public static FormOperation FrmOperation;
        public static FormLotOpen FrmLotOpen;
        public FormPassword FrmPassword;
        public static FormMain FrmMain;

        public int LotInfoCnt = 6;
        public int DayInfoCnt = 6;

        //public EN_SEQ_STAT m_iSeqStat;

        public FormOperation(Panel _pnBase)
        {
            InitializeComponent();

            FrmPassword = new FormPassword();

            this.TopLevel = false;
            this.Parent = _pnBase;

            DispDayList();
            DispLotInfo();

            pnPassWord.Visible = false;
            tmUpdate.Enabled = true;

            //btStart.Enabled = LotUnit.GetLotOpen();

            DM.ARAY[(int)ri.LDR].SetParent(pnLDR); DM.ARAY[(int)ri.LDR].Name = "LDR";
            DM.ARAY[(int)ri.PRI].SetParent(pnPRI); DM.ARAY[(int)ri.PRI].Name = "PRI";
            DM.ARAY[(int)ri.WRK].SetParent(pnWRK); DM.ARAY[(int)ri.WRK].Name = "WRK";
            DM.ARAY[(int)ri.PCK].SetParent(pnPCK); DM.ARAY[(int)ri.PCK].Name = "PCK";
            DM.ARAY[(int)ri.ALN].SetParent(pnALN); DM.ARAY[(int)ri.ALN].Name = "ALN";

            //Loader Array
            DM.ARAY[(int)ri.LDR].SetDispColor(cs.None   , Color.White  ); DM.ARAY[(int)ri.LDR].SetDispName(cs.None   , "Not Exsist"); DM.ARAY[(int)ri.LDR].SetVisible(cs.None    , true);
            DM.ARAY[(int)ri.LDR].SetDispColor(cs.Unkwn  , Color.Aqua   ); DM.ARAY[(int)ri.LDR].SetDispName(cs.Unkwn  , "UnKnown"   ); DM.ARAY[(int)ri.LDR].SetVisible(cs.Unkwn   , true);
            DM.ARAY[(int)ri.LDR].SetDispColor(cs.Empty  , Color.Gray   ); DM.ARAY[(int)ri.LDR].SetDispName(cs.Empty  , "Empty"     ); DM.ARAY[(int)ri.LDR].SetVisible(cs.Empty   , true);
                                                                                                                                                                                 
            //PreBuffer Array                                                                                                                                                    
            DM.ARAY[(int)ri.PRI].SetDispColor(cs.None   , Color.White  ); DM.ARAY[(int)ri.PRI].SetDispName(cs.None   , "Not Exsist"); DM.ARAY[(int)ri.PRI].SetVisible(cs.None    , true);
            DM.ARAY[(int)ri.PRI].SetDispColor(cs.Unkwn  , Color.Aqua   ); DM.ARAY[(int)ri.PRI].SetDispName(cs.Unkwn  , "UnKnown"   ); DM.ARAY[(int)ri.PRI].SetVisible(cs.Unkwn   , true);
            DM.ARAY[(int)ri.PRI].SetDispColor(cs.Empty  , Color.Gray   ); DM.ARAY[(int)ri.PRI].SetDispName(cs.Empty  , "Empty"     ); DM.ARAY[(int)ri.PRI].SetVisible(cs.Empty   , true);
                                                                                                                                                                                 
            //Work Array                                                                                                                                                         
            DM.ARAY[(int)ri.WRK].SetDispColor(cs.None   , Color.White  ); DM.ARAY[(int)ri.WRK].SetDispName(cs.None   , "Not Exsist"); DM.ARAY[(int)ri.WRK].SetVisible(cs.None    , true);
            DM.ARAY[(int)ri.WRK].SetDispColor(cs.Unkwn  , Color.Aqua   ); DM.ARAY[(int)ri.WRK].SetDispName(cs.Unkwn  , "UnKnown"   ); DM.ARAY[(int)ri.WRK].SetVisible(cs.Unkwn   , true);
            DM.ARAY[(int)ri.WRK].SetDispColor(cs.Empty  , Color.Gray   ); DM.ARAY[(int)ri.WRK].SetDispName(cs.Empty  , "Empty"     ); DM.ARAY[(int)ri.WRK].SetVisible(cs.Empty   , true);
            DM.ARAY[(int)ri.WRK].SetDispColor(cs.Good   , Color.Green  ); DM.ARAY[(int)ri.WRK].SetDispName(cs.Good   , "Good"      ); DM.ARAY[(int)ri.WRK].SetVisible(cs.Good    , true);
            DM.ARAY[(int)ri.WRK].SetDispColor(cs.Fail   , Color.Red    ); DM.ARAY[(int)ri.WRK].SetDispName(cs.Fail   , "Fail"      ); DM.ARAY[(int)ri.WRK].SetVisible(cs.Fail    , true);
            DM.ARAY[(int)ri.WRK].SetDispColor(cs.Working, Color.Yellow ); DM.ARAY[(int)ri.WRK].SetDispName(cs.Working, "Working"   ); DM.ARAY[(int)ri.WRK].SetVisible(cs.Working , true);
                                                                                                                     
            //Picker Array                                                                                           
            DM.ARAY[(int)ri.PCK].SetDispColor(cs.None   , Color.White  ); DM.ARAY[(int)ri.PCK].SetDispName(cs.None   , "Not Exsist"); DM.ARAY[(int)ri.PCK].SetVisible(cs.None    , true);
            DM.ARAY[(int)ri.PCK].SetDispColor(cs.Unkwn  , Color.Aqua   ); DM.ARAY[(int)ri.PCK].SetDispName(cs.Unkwn  , "UnKnown"   ); DM.ARAY[(int)ri.PCK].SetVisible(cs.Unkwn   , true);
            DM.ARAY[(int)ri.PCK].SetDispColor(cs.Empty  , Color.Gray   ); DM.ARAY[(int)ri.PCK].SetDispName(cs.Empty  , "Empty"     ); DM.ARAY[(int)ri.PCK].SetVisible(cs.Empty   , true);
            DM.ARAY[(int)ri.PCK].SetDispColor(cs.Align  , Color.Pink   ); DM.ARAY[(int)ri.PCK].SetDispName(cs.Align  , "Align"     ); DM.ARAY[(int)ri.PCK].SetVisible(cs.Align   , true);
            DM.ARAY[(int)ri.PCK].SetDispColor(cs.Good   , Color.Green  ); DM.ARAY[(int)ri.PCK].SetDispName(cs.Good   , "Good"      ); DM.ARAY[(int)ri.PCK].SetVisible(cs.Good    , true);
            DM.ARAY[(int)ri.PCK].SetDispColor(cs.Fail   , Color.Red    ); DM.ARAY[(int)ri.PCK].SetDispName(cs.Fail   , "Fail"      ); DM.ARAY[(int)ri.PCK].SetVisible(cs.Fail    , true);
            DM.ARAY[(int)ri.PCK].SetDispColor(cs.Marking, Color.Blue   ); DM.ARAY[(int)ri.PCK].SetDispName(cs.Marking, "Marking"   ); DM.ARAY[(int)ri.PCK].SetVisible(cs.Marking , true);
                                                                                                                                                                                 
            //Aligner Array                                                                                                                                                      
            DM.ARAY[(int)ri.ALN].SetDispColor(cs.None   , Color.White  ); DM.ARAY[(int)ri.ALN].SetDispName(cs.None   , "Not Exsist"); DM.ARAY[(int)ri.ALN].SetVisible(cs.None    , true);
            DM.ARAY[(int)ri.ALN].SetDispColor(cs.Unkwn  , Color.Aqua   ); DM.ARAY[(int)ri.ALN].SetDispName(cs.Unkwn  , "UnKnown"   ); DM.ARAY[(int)ri.ALN].SetVisible(cs.Unkwn   , true);
            DM.ARAY[(int)ri.ALN].SetDispColor(cs.Empty  , Color.Gray   ); DM.ARAY[(int)ri.ALN].SetDispName(cs.Empty  , "Empty"     ); DM.ARAY[(int)ri.ALN].SetVisible(cs.Empty   , true);
            DM.ARAY[(int)ri.ALN].SetDispColor(cs.Align  , Color.Gold   ); DM.ARAY[(int)ri.ALN].SetDispName(cs.Align  , "Align"     ); DM.ARAY[(int)ri.ALN].SetVisible(cs.Align   , true);
                                                                                                                                                                                 
            DM.ARAY[(int)ri.LDR].SetMaxColRow(1                     , 1                     );
            DM.ARAY[(int)ri.PRI].SetMaxColRow(OM.DevInfo.iTrayColCnt, OM.DevInfo.iTrayRowCnt);
            DM.ARAY[(int)ri.WRK].SetMaxColRow(OM.DevInfo.iTrayColCnt, OM.DevInfo.iTrayRowCnt);
            DM.ARAY[(int)ri.PCK].SetMaxColRow(1                     , OM.DevInfo.iTrayRowCnt);
            DM.ARAY[(int)ri.ALN].SetMaxColRow(1                     , OM.DevInfo.iTrayRowCnt);
        
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
            FormPassword.SetLevel(EN_LEVEL.lvOperator);
            pnPassWord.Visible = false;
        }

        private void btEngr_Click(object sender, EventArgs e)
        {
            FrmPassword.ShowPage(EN_LEVEL.lvEngineer);
            FrmPassword.Show();

            pnPassWord.Visible = false;
        }

        private void btMast_Click(object sender, EventArgs e)
        {
            FrmPassword.ShowPage(EN_LEVEL.lvMaster);
            FrmPassword.Show();

            pnPassWord.Visible = false;
        }

        private void btPasswordClose_Click(object sender, EventArgs e)
        {
            pnPassWord.Visible = false;
        }

        private void btLotEnd_Click(object sender, EventArgs e)
        {
            LOT.LotEnd();
        }


        private static bool bPreLotOpen = false;
        private static int iTrayImg = 0;

        private void timer1_Tick(object sender, EventArgs e)
        {
            tmUpdate.Enabled = false;
            //pnLDR.Refresh();

            int iSolder = 1;
            int iClean  = 2;
//            double dSolderTime = SEQ.SLD.GetCycleTime(iSolder);
//            double dClean      = SEQ.SLD.GetCycleTime(iClean);

            int iLevel = (int)FormPassword.GetLevel();
            switch (iLevel)
            {
                case (int)EN_LEVEL.lvOperator: btOperator.Text = "OPERATOR"; break;
                case (int)EN_LEVEL.lvEngineer: btOperator.Text = "ENGINEER"; break;
                case (int)EN_LEVEL.lvMaster  : btOperator.Text = " ADMIN  "; break;
                default                      : btOperator.Text = " ERROR  "; break;
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
            for (int i = 0 ; i < SM.ER._iMaxErrCnt ; i++) 
            {
                if (SM.ER.GetErr(i)) iCrntErrCnt++;
            }
            if (iPreErrCnt != iCrntErrCnt ) 
            {
                lbErr.Items.Clear();
                int iErrNo = SM.ER.GetLastErr();
                for (int i = 0; i < SM.ER._iMaxErrCnt; i++) 
                {
                    if (SM.ER.GetErr(i))
                    {
                        Str = string.Format("[ERR{0:000}]" , i) ;
                        Str += SM.ER.GetErrName(i) + " " + SM.ER.GetErrMsg(i);
                        lbErr.Items.Add(Str);
                    }
                }
            }
            if (SEQ._iSeqStat != EN_SEQ_STAT.ssError)
            {
                lbErr.Items.Clear();
            }
            iPreErrCnt = iCrntErrCnt ;

          
            string sCycleTimeSec ;
            int iCycleTimeMs ;
            
            
            //Door Sensor.  나중에 찾아보자
            //bool isAllCloseDoor = SM.IO.GetX((int)EN_INPUT_ID.xETC_DoorFt) &&
            //                      SM.IO.GetX((int)EN_INPUT_ID.xETC_DoorLt) &&
            //                      SM.IO.GetX((int)EN_INPUT_ID.xETC_DoorRt) &&
            //                      SM.IO.GetX((int)EN_INPUT_ID.xETC_DoorRr) ;
            //if (FormPassword.GetLevel() != EN_LEVEL.lvOperator && isAllCloseDoor && CMachine._bRun)
            //{
            //    //FM_SetLevel(lvOperator);
            //}
            
            if(!SM.MT.GetHomeDoneAll()){
                btAllHome.ForeColor = SEQ._bFlick ? Color.Black : Color.Red;
            }
            else {
                btAllHome.ForeColor = Color.Black  ;
            }

            //if (iTrayImg != DM.ARAY[(int)ri.WRK].SubStep) { 
                if(DM.ARAY[(int)ri.WRK].SubStep == 0) pbTrayImg.Image = global::Machine.Properties.Resources.Tray1;
                else                                  pbTrayImg.Image = global::Machine.Properties.Resources.Tray2; 
                iTrayImg = DM.ARAY[(int)ri.WRK].SubStep;

            //}

            //Option View
            if(OM.CmnOptn.bIgnrDoor   ) {pnOption1.BackColor = Color.Lime; lbOption1.Text = "ON"; }
            else                        {pnOption1.BackColor = Color.Red ; lbOption1.Text = "OFF";}
            if(OM.CmnOptn.bVisnSkip   ) {pnOption2.BackColor = Color.Lime; lbOption2.Text = "ON"; }
            else                        {pnOption2.BackColor = Color.Red ; lbOption2.Text = "OFF";}
            if(OM.CmnOptn.bAirBlwrSkip) {pnOption3.BackColor = Color.Lime; lbOption3.Text = "ON"; }
            else                        {pnOption3.BackColor = Color.Red ; lbOption3.Text = "OFF";}


            //DM.ARAY[(int)ri.LDR].SetMaxColRow(1                     , 1                     );
            //DM.ARAY[(int)ri.PRI].SetMaxColRow(OM.DevInfo.iTrayColCnt, OM.DevInfo.iTrayRowCnt);
            //DM.ARAY[(int)ri.WRK].SetMaxColRow(OM.DevInfo.iTrayColCnt, OM.DevInfo.iTrayRowCnt);
            //DM.ARAY[(int)ri.PCK].SetMaxColRow(1                     , OM.DevInfo.iTrayRowCnt);
            //DM.ARAY[(int)ri.ALN].SetMaxColRow(1                     , OM.DevInfo.iTrayRowCnt);
            //DM.ARAY[(int)ri.ULD].SetMaxColRow(1                     , 1                     );

            //if (SEQ._iSeqStat == EN_SEQ_STAT.ssWorkEnd || SEQ._iSeqStat == EN_SEQ_STAT.ssStop)
            //{
            //    SEQ.Reset();
            //    if (bRepeat) SEQ._bBtnStart = true;
            //}
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
        }

        private void btReset_Click(object sender, EventArgs e)
        {
            SEQ._bBtnReset = true;
            
        }

        private void lvDayInfo_MouseDoubleClick(object sender, MouseEventArgs e)  //요거는 확인 해봐야 함 진섭
        {
            if(FormPassword.GetLevel() != EN_LEVEL.lvMaster) return ;

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
                case 2: OM.CmnOptn.bVisnSkip    = !OM.CmnOptn.bVisnSkip   ; break;
                case 3: OM.CmnOptn.bAirBlwrSkip = !OM.CmnOptn.bAirBlwrSkip; break;
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
                case 2: OM.CmnOptn.bVisnSkip    = !OM.CmnOptn.bVisnSkip   ; break;
                case 3: OM.CmnOptn.bAirBlwrSkip = !OM.CmnOptn.bAirBlwrSkip; break;
            }
            OM.SaveCmnOptn();
        }

        private void btManual1_Click(object sender, EventArgs e)
        {
            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);
            MM.SetManCycle((mc)iBtnTag);
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
