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
using SML2;
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
        
        //FormErr FrmErr;

        public int LotInfoCnt = 7;
        public int DayInfoCnt = 6;


        //public EN_SEQ_STAT m_iSeqStat;

        public enum MovePos
        {
            LensVisnPos = 0,
            RearVisnPos    ,
            FrntVisnPos    ,
            LensPickPos    ,
            RearPickPos    ,
            FrntPickPos    ,

        }

        

        public FormOperation(Panel _pnBase)
        {
            InitializeComponent();

            FrmPassword = new FormPassword();
            FrmLotOpen  = new FormLotOpen ();

            this.TopLevel = false;
            this.Parent = _pnBase;
            
            //FrmMain = _FrmMain;
            DayInfoList();
            LotInfoList();

            pnPassWord.Visible = false;
            tmUpdate.Enabled = true;

            btLotEnd.Enabled  =  LOT.GetLotOpen();
            btStart.Enabled   =  LOT.GetLotOpen();
            btLotOpen.Enabled = !LOT.GetLotOpen();
            //
            DM.ARAY[(int)ri.IDX].SetParent(pnIdx);
            DM.ARAY[(int)ri.IDX].Name = "riIDX";
            DM.ARAY[(int)ri.IDX].SetDispColor(cs.None , Color.White ); DM.ARAY[(int)ri.IDX].SetDispName(cs.None , "NotExsist" ); DM.ARAY[(int)ri.IDX].SetVisible(cs.None , true);
            DM.ARAY[(int)ri.IDX].SetDispColor(cs.Empty, Color.Gray  ); DM.ARAY[(int)ri.IDX].SetDispName(cs.Empty, "Empty"     ); DM.ARAY[(int)ri.IDX].SetVisible(cs.Empty, true);
            DM.ARAY[(int)ri.IDX].SetDispColor(cs.Unkwn, Color.Aqua  ); DM.ARAY[(int)ri.IDX].SetDispName(cs.Unkwn, "Unknown"   ); DM.ARAY[(int)ri.IDX].SetVisible(cs.Unkwn, true);
            DM.ARAY[(int)ri.IDX].SetDispColor(cs.Move , Color.Yellow); DM.ARAY[(int)ri.IDX].SetDispName(cs.Move , "Move"      ); DM.ARAY[(int)ri.IDX].SetVisible(cs.Move , true);
            DM.ARAY[(int)ri.IDX].SetDispColor(cs.Work , Color.Blue  ); DM.ARAY[(int)ri.IDX].SetDispName(cs.Work , "Work"      ); DM.ARAY[(int)ri.IDX].SetVisible(cs.Work , true);
            DM.ARAY[(int)ri.IDX].SetMaxColRow(1, 1);

            //
            //DM.ARAY[(int)ri.PST_IDX].SetParent(pnPst);
            //DM.ARAY[(int)ri.PST_IDX].Name = "riPst";
            //DM.ARAY[(int)ri.PST_IDX].SetDispColor(cs.None , Color.White ); DM.ARAY[(int)ri.PST_IDX].SetDispName(cs.None , "NotExsist" ); DM.ARAY[(int)ri.PST_IDX].SetVisible(cs.None , true);
            //DM.ARAY[(int)ri.PST_IDX].SetDispColor(cs.Empty, Color.Gray  ); DM.ARAY[(int)ri.PST_IDX].SetDispName(cs.Empty, "Empty"     ); DM.ARAY[(int)ri.PST_IDX].SetVisible(cs.Empty, true);
            //DM.ARAY[(int)ri.PST_IDX].SetDispColor(cs.Work , Color.Blue  ); DM.ARAY[(int)ri.PST_IDX].SetDispName(cs.Work , "Work"      ); DM.ARAY[(int)ri.PST_IDX].SetVisible(cs.Work , true);
            //DM.ARAY[(int)ri.PST_IDX].SetMaxColRow(1,  5);

            DM.LoadMap();
        }

        public int m_iLevel;

        private void btOperator_Click(object sender, EventArgs e)
        {
            pnPassWord.Visible = true;
        }

        //사용자 레벨 버튼 클릭 이벤트
        private void btOper_Click(object sender, EventArgs e)
        {
            FormPassword.SetLevel(EN_LEVEL.Operator);
            pnPassWord.Visible = false;
        }

        private void btEngr_Click(object sender, EventArgs e)
        {
            FrmPassword.ShowPage(EN_LEVEL.Engineer);
            FrmPassword.Show();

            pnPassWord.Visible = false;
        }

        private void btMast_Click(object sender, EventArgs e)
        {
            FrmPassword.ShowPage(EN_LEVEL.Master);
            FrmPassword.Show();

            pnPassWord.Visible = false;
        }

        private void btPasswordClose_Click(object sender, EventArgs e)
        {
            pnPassWord.Visible = false;
        }

        private void btLotEnd_Click(object sender, EventArgs e)
        {
            if (Log.ShowMessageModal("Confirm", "Do you want to Lot End?") != DialogResult.Yes) return;
            SPC.Close();
            LOT.LotEnd();
            DM.ARAY[ri.IDX].SetStat(cs.None);
        }


        static bool bPreLotOpen = false; 

        private void timer1_Tick(object sender, EventArgs e)
        {
            tmUpdate.Enabled = false;
            pnIdx.Refresh();
            //pnPst.Refresh();

            tbTargetCnt.Text = OM.DevOptn.iTargetCnt.ToString();
            tbCrntCnt  .Text = OM.EqpStat.iWorkCnt  .ToString();

            tbNodeCnt  .Text = OM.EqpStat.iNodeCnt.ToString();
            tbCttrCnt  .Text = OM.EqpStat.iCttrCnt.ToString();
            tbDegree   .Text = OM.NodePos[SEQ.IDX.iNodeCnt].dDegree.ToString();

            int iLevel = (int)FormPassword.GetLevel();
            switch (iLevel)
            {
                case (int)EN_LEVEL.Operator: btOperator.Text = "OPERATOR"; break;
                case (int)EN_LEVEL.Engineer: btOperator.Text = "ENGINEER"; break;
                case (int)EN_LEVEL.Master  : btOperator.Text = " ADMIN  "; break;
                default                    : btOperator.Text = " ERROR  "; break;
            }

            if (bPreLotOpen != LOT.GetLotOpen())
            {
                btLotEnd .Enabled =  LOT.GetLotOpen();
                btStart  .Enabled =  LOT.GetLotOpen();
                btLotOpen.Enabled = !LOT.GetLotOpen();
                bPreLotOpen       =  LOT.GetLotOpen();
            }


            SPC.DAY.DispDayInfo(lvDayInfo);
            SPC.LOT.DispLotInfo(lvLotInfo);

            string Str      ;
            int iPreErrCnt  = 0;
            int iCrntErrCnt = 0;
            for (int i = 0 ; i < SML.ER._iMaxErrCnt ; i++) 
            {
                if (SML.ER.GetErr(i)) iCrntErrCnt++;
            }
            if (iPreErrCnt != iCrntErrCnt ) 
            {
                lbErr.Items.Clear();
                int iErrNo = SML.ER.GetLastErr();
                for (int i = 0; i < SML.ER._iMaxErrCnt; i++) 
                {
                    if (SML.ER.GetErr(i))
                    {
                        Str = string.Format("[ERR{0:000}]" , i) ;
                        Str += SML.ER.GetErrName(i) + " " + SML.ER.GetErrMsg(i);
                        lbErr.Items.Add(Str);
                    }
                }
            }
            if (SEQ._iSeqStat != EN_SEQ_STAT.Error)
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
            
            if(!SM.MT_GetHomeDoneAll()){
                btAllHome.ForeColor = SEQ._bFlick ? Color.Black : Color.Red;
            }
            else {
                btAllHome.ForeColor = Color.Black  ;
            }

            //DM.ARAY[(int)ri.LENS ].SetMaxColRow(OM.DevInfo.iLensColCnt , OM.DevInfo.iLensRowCnt);
            //DM.ARAY[(int)ri.REAR ].SetMaxColRow(OM.DevInfo.iRearColCnt , OM.DevInfo.iRearRowCnt);
            //DM.ARAY[(int)ri.FRNT ].SetMaxColRow(OM.DevInfo.iFrntColCnt , OM.DevInfo.iFrntRowCnt);
            //DM.ARAY[(int)ri.PICK ].SetMaxColRow(2, 1);

            pnIDXDetect1.BackColor = !SM.IO_GetX(xi.IDX_Detect1) ? Color.Lime : Color.Red;
            pnIDXDetect2.BackColor = !SM.IO_GetX(xi.IDX_Detect2) ? Color.Lime : Color.Red;
            pnIDXDetect3.BackColor = !SM.IO_GetX(xi.IDX_Detect3) ? Color.Lime : Color.Red;
            pnIDXDetect4.BackColor = !SM.IO_GetX(xi.IDX_Detect4) ? Color.Lime : Color.Red;
            pnIDXDetect5.BackColor = !SM.IO_GetX(xi.IDX_Detect5) ? Color.Lime : Color.Red;

            pnULDDetect1.BackColor = SM.IO_GetX(xi.ULD_Detect1) ? Color.Lime : Color.Red;
            pnULDDetect2.BackColor = SM.IO_GetX(xi.ULD_Detect2) ? Color.Lime : Color.Red;
            pnULDDetect3.BackColor = SM.IO_GetX(xi.ULD_Detect3) ? Color.Lime : Color.Red;
            pnULDDetect4.BackColor = SM.IO_GetX(xi.ULD_Detect4) ? Color.Lime : Color.Red;
            pnULDDetect5.BackColor = SM.IO_GetX(xi.ULD_Detect5) ? Color.Lime : Color.Red;

            //Option View
            if(OM.CmnOptn.bUsedLine1) {pnOption1.BackColor = Color.Lime; lbOption1.Text = "ON"; }
            else                      {pnOption1.BackColor = Color.Red ; lbOption1.Text = "OFF";}
            if(OM.CmnOptn.bUsedLine2) {pnOption2.BackColor = Color.Lime; lbOption2.Text = "ON"; }
            else                      {pnOption2.BackColor = Color.Red ; lbOption2.Text = "OFF";}
            if(OM.CmnOptn.bUsedLine3) {pnOption3.BackColor = Color.Lime; lbOption3.Text = "ON"; }
            else                      {pnOption3.BackColor = Color.Red ; lbOption3.Text = "OFF";}
            if(OM.CmnOptn.bUsedLine4) {pnOption4.BackColor = Color.Lime; lbOption4.Text = "ON"; }
            else                      {pnOption4.BackColor = Color.Red ; lbOption4.Text = "OFF";}
            if(OM.CmnOptn.bUsedLine5) {pnOption5.BackColor = Color.Lime; lbOption5.Text = "ON"; }
            else                      {pnOption5.BackColor = Color.Red ; lbOption5.Text = "OFF";}
            if(OM.CmnOptn.bIgnrWork ) {pnOption6.BackColor = Color.Lime; lbOption6.Text = "ON"; }
            else                      {pnOption6.BackColor = Color.Red ; lbOption6.Text = "OFF";}

            btCyl1 .Text      = SML.CL.GetCmd((int)ci.IDX_Hold1UpDn ) != 0 ?  "FWD"      : "BWD"      ;
            btCyl1 .ForeColor = SML.CL.GetCmd((int)ci.IDX_Hold1UpDn ) != 0 ?  Color.Lime : Color.Black;

            btCyl2 .Text      = SML.CL.GetCmd((int)ci.IDX_CutLtFwBw ) != 0 ?  "FWD"      : "BWD"      ;
            btCyl2 .ForeColor = SML.CL.GetCmd((int)ci.IDX_CutLtFwBw ) != 0 ?  Color.Lime : Color.Black;

            btCyl3 .Text      = SML.CL.GetCmd((int)ci.IDX_CutRtFwBw ) != 0 ?  "FWD"      : "BWD"      ;
            btCyl3 .ForeColor = SML.CL.GetCmd((int)ci.IDX_CutRtFwBw ) != 0 ?  Color.Lime : Color.Black;
            
            btCyl4 .Text      = SML.CL.GetCmd((int)ci.IDX_TwstLtDnUp) != 0 ?  "FWD"      : "BWD"      ;
            btCyl4 .ForeColor = SML.CL.GetCmd((int)ci.IDX_TwstLtDnUp) != 0 ?  Color.Lime : Color.Black;

            //btCyl5 .Text      = SM.CL.GetCmd((int)ai.IDX_TwstRtDnUp) != 0 ?  "FWD"      : "BWD"      ;
            //btCyl5 .ForeColor = SM.CL.GetCmd((int)ai.IDX_TwstRtDnUp) != 0 ?  Color.Lime : Color.Black;

            btCyl6 .Text      = SML.CL.GetCmd((int)ci.IDX_Hold2UpDn ) != 0 ?  "FWD"      : "BWD"      ;
            btCyl6 .ForeColor = SML.CL.GetCmd((int)ci.IDX_Hold2UpDn ) != 0 ?  Color.Lime : Color.Black;

            btCyl7.Text       = SML.CL.GetCmd((int)ci.IDX_CutBaseUpDn)  != 0 ? "FWD"       : "BWD"      ;
            btCyl7 .ForeColor = SML.CL.GetCmd((int)ci.IDX_CutBaseUpDn ) != 0 ?  Color.Lime : Color.Black;
            
            //btCyl7 .Text      = SM.CL.GetCmd((int)ai.IDX_ShiftFwBw ) != 0 ?  "FWD"      : "BWD"      ;
            //btCyl7 .ForeColor = SM.CL.GetCmd((int)ai.IDX_ShiftFwBw ) != 0 ?  Color.Lime : Color.Black;

            //btCyl8 .Text      = SM.CL.GetCmd((int)ai.IDX_ShiftUpDn ) != 0 ?  "FWD"      : "BWD"      ;
            //btCyl8 .ForeColor = SM.CL.GetCmd((int)ai.IDX_ShiftUpDn ) != 0 ?  Color.Lime : Color.Black;

            btCyl8 .Text      = SML.CL.GetCmd((int)ci.IDX_OutDnUp   ) != 0 ?  "FWD"      : "BWD"      ;
            btCyl8 .ForeColor = SML.CL.GetCmd((int)ci.IDX_OutDnUp   ) != 0 ?  Color.Lime : Color.Black;

            btCyl9.Text      = SML.CL.GetCmd((int)ci.IDX_CutterDnUp) != 0 ?  "FWD"      : "BWD"      ;
            btCyl9.ForeColor = SML.CL.GetCmd((int)ci.IDX_CutterDnUp) != 0 ?  Color.Lime : Color.Black;

            //if (CMachine._iSeqStat == EN_SEQ_STAT.ssWorkEnd || CMachine._iSeqStat == EN_SEQ_STAT.ssStop)
            //{
            //    CMachine.Reset();
            //    if (bRepeat) CMachine._bBtnStart = true;
            //}
            tmUpdate.Enabled = true;
        }

        private void btLotOpen_Click(object sender, EventArgs e)
        {
            if (FrmLotOpen.IsDisposed)
            {
                FrmLotOpen = new FormLotOpen();
            }
            
            FrmLotOpen.Show();
        }

        public void LotInfoList() //오퍼레이션 창용.
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
        
            //LotCount확인
            //lvLotInfo.Items.Clear();
        
            //DirectoryInfo Info = new DirectoryInfo(sPath);
            //int FileCount = Info.GetFiles().Length;
        
            ListViewItem[] liLotInfo = new ListViewItem[LotInfoCnt];
        
            for (int i = 0; i < LotInfoCnt; i++)
            {
                liLotInfo[i] = new ListViewItem();
                liLotInfo[i].SubItems.Add("");
        
        
                liLotInfo[i].UseItemStyleForSubItems = false;
                liLotInfo[i].UseItemStyleForSubItems = false;
        
                lvLotInfo.Items.Add(liLotInfo[i]);
                
            }

            var PropDayInfo = lvLotInfo.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropDayInfo.SetValue(lvLotInfo, true, null);

            if (lvDayInfo == null) return;
        }

         
        public void DayInfoList() //오퍼레이션 창용.
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
            
            //LotCount확인
            //_lvDayInfo.Items.Clear();
            
            //DirectoryInfo Info = new DirectoryInfo(sPath);
            //int FileCount = Info.GetFiles().Length;

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
        
            //double dYield = m_tData.iWorkCnt ? (m_tData.iWorkCnt - m_tData.iFailCnt) * 100 / (double)m_tData.iWorkCnt : 0.0;
            //String sYield = sYield.sprintf("%.2f%%",dYield);
        
            
        }

        private void btStart_Click(object sender, EventArgs e)
        {
            SEQ._bBtnReset = true;
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
            if(FormPassword.GetLevel() != EN_LEVEL.Master) return ;

            if (Log.ShowMessageModal("Confirm", "Clear Day Info?") != DialogResult.Yes) return;
            
            SPC.DAY.ClearData() ;
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

        //매뉴얼 버튼 클릭 이벤트
        private void btClean_Click(object sender, EventArgs e)
        {
            int iBtnTag = Convert.ToInt32(((Button)sender).Tag) ;
            MM.SetManCycle((mc)iBtnTag);
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

        private void btTimeReset_Click(object sender, EventArgs e)
        {
            DayData.m_tData.TimeClear();
          
        }

        private void btUPHReset_Click(object sender, EventArgs e)
        {
            DayData.m_tData.UPHClear();
        }

        private void btWrkCntReset_Click(object sender, EventArgs e)
        {
            DayData.m_tData.CntClear();
        }

        private void label7_Click(object sender, EventArgs e)
        {
            
        }

        private void FormOperation_Shown(object sender, EventArgs e)
        {
//            EmbededVision.SetVisionParent(pnCamera.Handle);
        }

        private void pnOption1_DoubleClick(object sender, EventArgs e)
        {
            int iBtnTag = Convert.ToInt32(((Panel)sender).Tag);
            switch (iBtnTag)
            {
                default: break;
                case 1: OM.CmnOptn.bUsedLine1 = !OM.CmnOptn.bUsedLine1; break;
                case 2: OM.CmnOptn.bUsedLine2 = !OM.CmnOptn.bUsedLine2; break;
                case 3: OM.CmnOptn.bUsedLine3 = !OM.CmnOptn.bUsedLine3; break;
                case 4: OM.CmnOptn.bUsedLine4 = !OM.CmnOptn.bUsedLine4; break;
                case 5: OM.CmnOptn.bUsedLine5 = !OM.CmnOptn.bUsedLine5; break;
                case 6: OM.CmnOptn.bIgnrWork  = !OM.CmnOptn.bIgnrWork ; break;
            }
            OM.SaveCmnOptn();
        }

        private void btManual1_Click(object sender, EventArgs e)
        {
            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);
            MM.SetManCycle((mc)iBtnTag);
        }

        private void lbOption1_DoubleClick(object sender, EventArgs e)
        {
            int iBtnTag = Convert.ToInt32(((Label)sender).Tag);
            switch (iBtnTag)
            {
                default: break;
                case 1: OM.CmnOptn.bUsedLine1 = !OM.CmnOptn.bUsedLine1; break;
                case 2: OM.CmnOptn.bUsedLine2 = !OM.CmnOptn.bUsedLine2; break;
                case 3: OM.CmnOptn.bUsedLine3 = !OM.CmnOptn.bUsedLine3; break;
                case 4: OM.CmnOptn.bUsedLine4 = !OM.CmnOptn.bUsedLine4; break;
                case 5: OM.CmnOptn.bUsedLine5 = !OM.CmnOptn.bUsedLine5; break;
                case 6: OM.CmnOptn.bIgnrWork  = !OM.CmnOptn.bIgnrWork ; break;
            }
            OM.SaveCmnOptn();
        }

        private void btLotEnd_Click_1(object sender, EventArgs e)
        {

        }

        private void btCyl1_Click(object sender, EventArgs e)
        {
            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);

            if (SML.CL.GetCmd(iBtnTag) == 0) SML.CL.Move(iBtnTag, EN_CYLINDER_POS.Fwd);
            else                             SML.CL.Move(iBtnTag, EN_CYLINDER_POS.Bwd);
                                                
        }

        private void btCyl9_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //textBox1.Text = SEQ.IDX.LengthToDegree(5).ToString();
            //textBox1.Text = SEQ.IDX.WorkCount(OM.NodePos[0].dWrkPitch).ToString();
        }


//        public IntPtr GetCamPnHandle()
//        {
//            return pnCamera.Handle;
//        }



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
