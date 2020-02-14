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
            tmUpdateArray.Enabled = true;

            btLotEnd.Enabled  =  LOT.GetLotOpen();
            //btStart.Enabled   =  LOT.GetLotOpen();
            btLotOpen.Enabled = !LOT.GetLotOpen();
            //Lens Stage
            DM.ARAY[(int)ri.LENS].SetParent(pnLENS);
            DM.ARAY[(int)ri.LENS].Name = "riLENS";
            DM.ARAY[(int)ri.LENS].SetDispColor(cs.None     , Color.White ); DM.ARAY[(int)ri.LENS].SetDispName(cs.None     , "NotExsist" ); DM.ARAY[(int)ri.LENS].SetVisible(cs.None     , true);
            DM.ARAY[(int)ri.LENS].SetDispColor(cs.Unkwn    , Color.Aqua  ); DM.ARAY[(int)ri.LENS].SetDispName(cs.Unkwn    , "Unknown"   ); DM.ARAY[(int)ri.LENS].SetVisible(cs.Unkwn    , true);
            DM.ARAY[(int)ri.LENS].SetDispColor(cs.Visn     , Color.Yellow); DM.ARAY[(int)ri.LENS].SetDispName(cs.Visn     , "Vision"    ); DM.ARAY[(int)ri.LENS].SetVisible(cs.Visn     , true);
            DM.ARAY[(int)ri.LENS].SetDispColor(cs.Align    , Color.Green ); DM.ARAY[(int)ri.LENS].SetDispName(cs.Align    , "Align"     ); DM.ARAY[(int)ri.LENS].SetVisible(cs.Align    , true);
            DM.ARAY[(int)ri.LENS].SetDispColor(cs.Empty    , Color.Gray  ); DM.ARAY[(int)ri.LENS].SetDispName(cs.Empty    , "Empty"     ); DM.ARAY[(int)ri.LENS].SetVisible(cs.Empty    , true);
            DM.ARAY[(int)ri.LENS].SetDispColor(cs.Fail     , Color.Red   ); DM.ARAY[(int)ri.LENS].SetDispName(cs.Fail     , "Fail"      ); DM.ARAY[(int)ri.LENS].SetVisible(cs.Fail     , true);
            //DM.ARAY[(int)ri.LENS].SetDispColor(cs.PickFail , Color.Pink  ); DM.ARAY[(int)ri.LENS].SetDispName(cs.PickFail , "PickFail"  ); DM.ARAY[(int)ri.LENS].SetVisible(cs.PickFail , true);
            DM.ARAY[(int)ri.LENS].SetMaxColRow(OM.DevInfo.iLensColCnt, OM.DevInfo.iLensRowCnt);

            //Picker Array
            DM.ARAY[(int)ri.PICK].SetParent(pnPICK);
            DM.ARAY[(int)ri.PICK].Name = "riPICK";
            DM.ARAY[(int)ri.PICK].SetDispColor(cs.None , Color.White ); DM.ARAY[(int)ri.PICK].SetDispName(cs.None , "NotExsist" ); DM.ARAY[(int)ri.PICK].SetVisible(cs.None , false);
            DM.ARAY[(int)ri.PICK].SetDispColor(cs.Visn , Color.Yellow); DM.ARAY[(int)ri.PICK].SetDispName(cs.Visn , "Vision"    ); DM.ARAY[(int)ri.PICK].SetVisible(cs.Visn , true );
            DM.ARAY[(int)ri.PICK].SetDispColor(cs.Align, Color.Green ); DM.ARAY[(int)ri.PICK].SetDispName(cs.Align, "Align"     ); DM.ARAY[(int)ri.PICK].SetVisible(cs.Align, true );
            DM.ARAY[(int)ri.PICK].SetDispColor(cs.Empty, Color.Gray  ); DM.ARAY[(int)ri.PICK].SetDispName(cs.Empty, "Empty"     ); DM.ARAY[(int)ri.PICK].SetVisible(cs.Empty, true );
            DM.ARAY[(int)ri.PICK].SetDispColor(cs.Fail , Color.Red   ); DM.ARAY[(int)ri.PICK].SetDispName(cs.Fail , "Fail"      ); DM.ARAY[(int)ri.PICK].SetVisible(cs.Fail , true );
            DM.ARAY[(int)ri.PICK].SetMaxColRow(2, 1);

            //Rear Housing Stage
            DM.ARAY[(int)ri.REAR].SetParent(pnREAR);
            DM.ARAY[(int)ri.REAR].Name = "riREAR";
            DM.ARAY[(int)ri.REAR].SetDispColor(cs.None         , Color.White    ); DM.ARAY[(int)ri.REAR].SetDispName(cs.None         , "NotExsist"   ); DM.ARAY[(int)ri.REAR].SetVisible(cs.None       , true);
            DM.ARAY[(int)ri.REAR].SetDispColor(cs.Unkwn        , Color.Aqua     ); DM.ARAY[(int)ri.REAR].SetDispName(cs.Unkwn        , "Unknown"     ); DM.ARAY[(int)ri.REAR].SetVisible(cs.Unkwn      , true);
            DM.ARAY[(int)ri.REAR].SetDispColor(cs.Visn         , Color.Yellow   ); DM.ARAY[(int)ri.REAR].SetDispName(cs.Visn         , "Vision"      ); DM.ARAY[(int)ri.REAR].SetVisible(cs.Visn       , true);
            DM.ARAY[(int)ri.REAR].SetDispColor(cs.Work         , Color.Blue     ); DM.ARAY[(int)ri.REAR].SetDispName(cs.Work         , "Work"        ); DM.ARAY[(int)ri.REAR].SetVisible(cs.Work       , true);
            DM.ARAY[(int)ri.REAR].SetDispColor(cs.TorqueFail   , Color.Orange   ); DM.ARAY[(int)ri.REAR].SetDispName(cs.TorqueFail   , "TorqueFail"  ); DM.ARAY[(int)ri.REAR].SetVisible(cs.TorqueFail , true);
            DM.ARAY[(int)ri.REAR].SetDispColor(cs.SensorFail   , Color.Olive    ); DM.ARAY[(int)ri.REAR].SetDispName(cs.SensorFail   , "SensorFail"  ); DM.ARAY[(int)ri.REAR].SetVisible(cs.SensorFail , true);
            DM.ARAY[(int)ri.REAR].SetDispColor(cs.LastVisn     , Color.Brown    ); DM.ARAY[(int)ri.REAR].SetDispName(cs.LastVisn     , "LastVisn"    ); DM.ARAY[(int)ri.REAR].SetVisible(cs.LastVisn     , true);
            DM.ARAY[(int)ri.REAR].SetDispColor(cs.LastVisnFail , Color.Pink     ); DM.ARAY[(int)ri.REAR].SetDispName(cs.LastVisnFail , "LastVisnFail"); DM.ARAY[(int)ri.REAR].SetVisible(cs.LastVisnFail , true);
            DM.ARAY[(int)ri.REAR].SetDispColor(cs.Fail         , Color.Red      ); DM.ARAY[(int)ri.REAR].SetDispName(cs.Fail         , "Fail"        ); DM.ARAY[(int)ri.REAR].SetVisible(cs.Fail         , true);
            DM.ARAY[(int)ri.REAR].SetMaxColRow(OM.DevInfo.iRearColCnt, OM.DevInfo.iRearRowCnt);

            //Front Housing Stage
            DM.ARAY[(int)ri.FRNT].SetParent(pnFRNT);
            DM.ARAY[(int)ri.FRNT].Name = "riFRNT";
            DM.ARAY[(int)ri.FRNT].SetDispColor(cs.None         , Color.White    ); DM.ARAY[(int)ri.FRNT].SetDispName(cs.None         , "NotExsist"   ); DM.ARAY[(int)ri.FRNT].SetVisible(cs.None       , true);
            DM.ARAY[(int)ri.FRNT].SetDispColor(cs.Unkwn        , Color.Aqua     ); DM.ARAY[(int)ri.FRNT].SetDispName(cs.Unkwn        , "Unknown"     ); DM.ARAY[(int)ri.FRNT].SetVisible(cs.Unkwn      , true);
            DM.ARAY[(int)ri.FRNT].SetDispColor(cs.Visn         , Color.Yellow   ); DM.ARAY[(int)ri.FRNT].SetDispName(cs.Visn         , "Vision"      ); DM.ARAY[(int)ri.FRNT].SetVisible(cs.Visn       , true);
            DM.ARAY[(int)ri.FRNT].SetDispColor(cs.Work         , Color.Blue     ); DM.ARAY[(int)ri.FRNT].SetDispName(cs.Work         , "Work"        ); DM.ARAY[(int)ri.FRNT].SetVisible(cs.Work       , true);
            DM.ARAY[(int)ri.FRNT].SetDispColor(cs.TorqueFail   , Color.Orange   ); DM.ARAY[(int)ri.FRNT].SetDispName(cs.TorqueFail   , "TorqueFail"  ); DM.ARAY[(int)ri.FRNT].SetVisible(cs.TorqueFail , true);
            DM.ARAY[(int)ri.FRNT].SetDispColor(cs.SensorFail   , Color.Olive    ); DM.ARAY[(int)ri.FRNT].SetDispName(cs.SensorFail   , "SensorFail"  ); DM.ARAY[(int)ri.FRNT].SetVisible(cs.SensorFail , true);
            DM.ARAY[(int)ri.FRNT].SetDispColor(cs.LastVisn     , Color.Brown    ); DM.ARAY[(int)ri.FRNT].SetDispName(cs.LastVisn     , "LastVisn"    ); DM.ARAY[(int)ri.FRNT].SetVisible(cs.LastVisn     , true);
            DM.ARAY[(int)ri.FRNT].SetDispColor(cs.LastVisnFail , Color.Pink     ); DM.ARAY[(int)ri.FRNT].SetDispName(cs.LastVisnFail , "LastVisnFail"); DM.ARAY[(int)ri.FRNT].SetVisible(cs.LastVisnFail , true);
            DM.ARAY[(int)ri.FRNT].SetDispColor(cs.Fail         , Color.Red      ); DM.ARAY[(int)ri.FRNT].SetDispName(cs.Fail         , "Fail"        ); DM.ARAY[(int)ri.FRNT].SetVisible(cs.Fail         , true);
            DM.ARAY[(int)ri.FRNT].SetMaxColRow(OM.DevInfo.iFrntColCnt, OM.DevInfo.iFrntRowCnt);

            DM.LoadMap();

            cbZWork.SelectedIndex = 0;
            cbInsp .SelectedIndex = 0;

            //DM.ARAY[(int)ri.PICK ].ChangeStat(cs.None, cs.Unkwn);
            //DM.ARAY[(int)ri.LENS ].ChangeStat(cs.None, cs.Unkwn);
            //DM.ARAY[(int)ri.REAR ].ChangeStat(cs.None, cs.Unkwn);
            //DM.ARAY[(int)ri.FRNT ].ChangeStat(cs.None, cs.Unkwn);
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

        static bool bPreLotOpen = false; 

        private void timer1_Tick(object sender, EventArgs e)
        {
            //tmUpdate.Enabled = false;

            lbUnLock.Visible = OM.MstOptn.bUnlock;

            lbUnLock.ForeColor = SEQ._bFlick ? Color.Red : Color.Black;



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
                btLotEnd .Enabled =  LOT.GetLotOpen();
                //btStart  .Enabled =  LOT.GetLotOpen();
                btLotOpen.Enabled = !LOT.GetLotOpen();
                bPreLotOpen       =  LOT.GetLotOpen();
            }

            if (OM.CmnOptn.bSkipFrnt) btFrntTrayIn.Enabled = false;
            else                      btFrntTrayIn.Enabled = true ;

            if (OM.CmnOptn.bSkipRear) btRearTrayIn.Enabled = false;
            else                      btRearTrayIn.Enabled = true ;
            

            SPC.DAY.DispDayInfo(lvDayInfo);
            SPC.LOT.DispLotInfo(lvLotInfo);

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


            //Door Sensor.  1호기는 도어센서 확인해야함.
            bool isAllCloseDoor = SM.IO.GetX((int)xi.ETC_DoorRr);

            if (FormPassword.GetLevel() != EN_LEVEL.lvOperator && isAllCloseDoor && SEQ._bRun)
            {
                //FM_SetLevel(lvOperator);
            }


            //bool isAllCloseDoor = SM.IO.GetX((int)EN_INPUT_ID.xETC_DoorFt) &&
            //                      SM.IO.GetX((int)EN_INPUT_ID.xETC_DoorLt) &&
            //                      SM.IO.GetX((int)EN_INPUT_ID.xETC_DoorRt) &&
            //                      SM.IO.GetX((int)EN_INPUT_ID.xETC_DoorRr) ;
            //if (FormPassword.GetLevel() != EN_LEVEL.lvOperator && isAllCloseDoor && CMachine._bRun)
            //{
            //    //FM_SetLevel(lvOperator);
            //}

            if (!SM.MT.GetHomeDoneAll()){
                btAllHome.ForeColor = SEQ._bFlick ? Color.Black : Color.Red;
            }
            else {
                btAllHome.ForeColor = Color.Black  ;
            }

            

            if (SEQ._iSeqStat != EN_SEQ_STAT.ssRunning)
            {
                btSTGVacmOnOff.ForeColor = SM.IO.GetY((int)yi.STG_VacLenOn) ? Color.Lime : Color.Black;
            }

            if (SEQ._iSeqStat != EN_SEQ_STAT.ssRunning)
            {
                btLightOnOff.ForeColor = SM.IO.GetY((int)yi.ETC_LightOnOff) ? Color.Lime : Color.Black;
            }

            //Option View
            if(OM.CmnOptn.bUseMultiHldr) {pnOption1.BackColor = Color.Lime; lbOption1.Text = "ON"; }
            else                         {pnOption1.BackColor = Color.Red ; lbOption1.Text = "OFF";}
            if(OM.CmnOptn.bTorqChck    ) {pnOption2.BackColor = Color.Lime; lbOption2.Text = "ON"; }
            else                         {pnOption2.BackColor = Color.Red ; lbOption2.Text = "OFF";}

            //if (CMachine._iSeqStat == EN_SEQ_STAT.ssWorkEnd || CMachine._iSeqStat == EN_SEQ_STAT.ssStop)
            //{
            //    CMachine.Reset();
            //    if (bRepeat) CMachine._bBtnStart = true;
            //}
            //tmUpdate.Enabled = true;
        }

        private void tmUpdateArray_Tick(object sender, EventArgs e)
        {
            pnLENS.Refresh();
            pnREAR.Refresh();
            pnFRNT.Refresh();
            pnPICK.Refresh();

            DM.ARAY[(int)ri.LENS].SetMaxColRow(OM.DevInfo.iLensColCnt, OM.DevInfo.iLensRowCnt);
            DM.ARAY[(int)ri.REAR].SetMaxColRow(OM.DevInfo.iRearColCnt, OM.DevInfo.iRearRowCnt);
            DM.ARAY[(int)ri.FRNT].SetMaxColRow(OM.DevInfo.iFrntColCnt, OM.DevInfo.iFrntRowCnt);
            DM.ARAY[(int)ri.PICK].SetMaxColRow(2, 1);
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
            lvLotInfo.View               = View.Details;
            lvLotInfo.LabelEdit          = true;
            lvLotInfo.AllowColumnReorder = true;
            lvLotInfo.FullRowSelect      = true;
            lvLotInfo.GridLines          = true;
            lvLotInfo.Sorting            = SortOrder.Descending;
            lvLotInfo.Scrollable         = true;
        
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
            lvDayInfo.View               = View.Details;
            lvDayInfo.LabelEdit          = true;
            lvDayInfo.AllowColumnReorder = true;
            lvDayInfo.FullRowSelect      = true;
            lvDayInfo.GridLines          = true;
            lvDayInfo.Sorting            = SortOrder.Descending;
            lvDayInfo.Scrollable         = true;

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
            if (!OM.MstOptn.bUnlock)
            {
                if (!LOT.GetLotOpen())
                {
                    Log.ShowMessage("Lot Open", "Not a Lot Open");
                    return;
                }
            }

            SEQ.ASY.iRptCnt = 0;

            //if (OM.CmnOptn.bUnlock)
            //{
            //    if (Log.ShowMessageModal("Confirm", "Do you Want to Unlock Work?") != DialogResult.Yes) return;
            //}
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

            SPC.DAY.ClearData();
            
            //CDayData.ClearData() ;
        }

        private bool bHomeClick = false;
        private void btAllHome_Click(object sender, EventArgs e)
        {
            //    if(IO_GetX(xSTG_Vccm)){
        //        FM_MsgOk("Error" , "작업 Stage에 자제를 제거하여 주십시오");
        //        return ;
        //    }
            //if (MessageBox.Show(new Form{TopMost = true}, "전체 홈을 잡으시겠습니까?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question)!=DialogResult.Yes) return;

            //업체측에서 빼달라고 해서 뺌 진섭.
            //if (Log.ShowMessageModal("Confirm", "Do you want to All Homming?") != DialogResult.Yes) return;

            MM.SetManCycle(mc.AllHome);
            bHomeClick = true;
        }

        //매뉴얼 버튼 클릭 이벤트
        private void btClean_Click(object sender, EventArgs e)
        {
            //Inspection Type
            const int iOneLens = 0;
            const int iOneRear = 1;
            const int iOneFrnt = 2;
            const int iConLens = 3;
            const int iConRear = 4;
            const int iConFrnt = 5;

            int iBtnTag = Convert.ToInt32(((Button)sender).Tag) ;

            //Ready Cal.
            if (iBtnTag == 50)
            {
                if (!OM.CmnOptn.bIgnrLeftPck)
                {
                    SM.MT.GoAbsMan((int)mi.PCK_ZL, 0);

                    if (SM.IO.GetY((int)yi.PCK_VacLtOn, true))
                    {
                        SM.IO.SetY((int)yi.PCK_VacLtOn, false);
                        SM.IO.SetY((int)yi.PCK_EjtLtOn, false);
                    }
                    else
                    {
                        SM.IO.SetY((int)yi.PCK_VacLtOn, true);
                        SM.IO.SetY((int)yi.PCK_EjtLtOn, false);
                    }
                }

                if (!OM.CmnOptn.bIgnrRightPck)
                {
                    SM.MT.GoAbsMan((int)mi.PCK_ZR, 0);

                    if (SM.IO.GetY((int)yi.PCK_VacRtOn, true))
                    {
                        SM.IO.SetY((int)yi.PCK_VacRtOn, false);
                        SM.IO.SetY((int)yi.PCK_EjtRtOn, false);
                    }
                    else
                    {
                        SM.IO.SetY((int)yi.PCK_VacRtOn, true);
                        SM.IO.SetY((int)yi.PCK_EjtRtOn, false);
                    }
                }
                
                //if (SM.IO.GetY((int)yi.PCK_VacLtOn) && SM.IO.GetY((int)yi.PCK_VacRtOn))
                //{
                //    SM.IO.SetY((int)yi.PCK_VacLtOn, false);
                //    SM.IO.SetY((int)yi.PCK_VacRtOn, false);
                //
                //    SM.IO.SetY((int)yi.PCK_EjtLtOn, false);
                //    SM.IO.SetY((int)yi.PCK_EjtRtOn, false);
                //}
                //else
                //{
                //    SM.IO.SetY((int)yi.PCK_VacLtOn, true);
                //    SM.IO.SetY((int)yi.PCK_VacRtOn, true);
                //
                //    SM.IO.SetY((int)yi.PCK_EjtLtOn, false);
                //    SM.IO.SetY((int)yi.PCK_EjtRtOn, false);
                //}
            }

            //Stage Vacuum On/Off
            else if (iBtnTag == 51)
            {
                if (SM.IO.GetY((int)yi.STG_VacLenOn))
                {
                    SM.IO.SetY((int)yi.STG_VacLenOn, false);
                }
                else
                {
                    SM.IO.SetY((int)yi.STG_VacLenOn, true);

                }
            }

            //Light OnOff
            else if (iBtnTag == 52)
            {
                if (SM.IO.GetY((int)yi.ETC_LightOnOff))
                {
                    SM.IO.SetY((int)yi.ETC_LightOnOff, false);
                }
                else
                {
                    SM.IO.SetY((int)yi.ETC_LightOnOff, true);

                }
            }

            else if (iBtnTag == 12)
            {
                if (cbZWork.SelectedIndex == 0) SEQ.ASY.bLeftTorque = true;
                else                            SEQ.ASY.bLeftTorque = false;

                MM.SetManCycle((mc)iBtnTag);
            }

            else if (iBtnTag == 13)
            {
                //if(Log.ShowMessageModal("STAGE", "Do you want To Tray Change?") != DialogResult.Yes) return;

                MM.SetManCycle((mc)iBtnTag);
            }

            else if (iBtnTag == 14)
            {
                //if (Log.ShowMessageModal("STAGE", "Do you want To Stage In?") != DialogResult.Yes) return; 
                MM.SetManCycle((mc)iBtnTag);
            }

            //Insp
            else if (iBtnTag == 20)
            {
                if (cbInsp.SelectedIndex == iOneLens)
                {
                    MM.m_iInspType = 0;
                }
                else if (cbInsp.SelectedIndex == iOneRear)
                {
                    MM.m_iInspType = 1;
                }
                else if (cbInsp.SelectedIndex == iOneFrnt)
                {
                    MM.m_iInspType = 2;
                }
                else if (cbInsp.SelectedIndex == iConLens)
                {
                    MM.m_iInspType = 0;
                    SEQ.ASY.bconsecutively = true; // !SEQ.ASY.bconsecutively;
                }
                else if (cbInsp.SelectedIndex == iConRear)
                {
                    MM.m_iInspType = 1;
                    SEQ.ASY.bconsecutively = true; // !SEQ.ASY.bconsecutively;
                }
                else if (cbInsp.SelectedIndex == iConFrnt)
                {
                    MM.m_iInspType = 2;
                    SEQ.ASY.bconsecutively = true;// !SEQ.ASY.bconsecutively;
                }
               
                MM.SetManCycle((mc)iBtnTag);
            }

            else if (iBtnTag == 60)
            {
                //if (Log.ShowMessageModal("STAGE", "Do you want To Stage In?") != DialogResult.Yes) return; 
                DM.ARAY[(int)ri.LENS].SetStat(cs.Unkwn);
                MM.SetManCycle(mc.ASY_CycleTrayIn);
            }

            else if (iBtnTag == 61)
            {
                //if (Log.ShowMessageModal("STAGE", "Do you want To Stage In?") != DialogResult.Yes) return;
                DM.ARAY[(int)ri.REAR].SetStat(cs.Unkwn);
                MM.SetManCycle(mc.ASY_CycleTrayIn);
            }

            else if (iBtnTag == 62)
            {
                //if (Log.ShowMessageModal("STAGE", "Do you want To Stage In?") != DialogResult.Yes) return;
                DM.ARAY[(int)ri.FRNT].SetStat(cs.Unkwn);
                MM.SetManCycle(mc.ASY_CycleTrayIn);
            }

            else if (iBtnTag == 11)//toolcalibration
            {
                if(!OM.CmnOptn.bIgnrLeftPck && !SM.IO.GetX((int)xi.PCK_VacLt))
                {
                    Log.ShowMessage("Error","There is no Lens on LeftTool");
                    return ;
                }
                if(!OM.CmnOptn.bIgnrRightPck && !SM.IO.GetX((int)xi.PCK_VacRt))
                {
                    Log.ShowMessage("Error","There is no Lens on RightTool");
                    return ;
                }
                MM.SetManCycle((mc)iBtnTag);
            }

            else if (iBtnTag == 15)//toolcalibration
            {
                if (Log.ShowMessageModal("Calibration", "Press Yes after put the Assay on the first Pocket for calibration") != DialogResult.Yes) return;
                MM.SetManCycle((mc)iBtnTag);
            }

            else
            {
                MM.SetManCycle((mc)iBtnTag);
            }
            
        }

        bool bRepeat = false;
        private void lbWorkNo_Click(object sender, EventArgs e)
        {
            //bRepeat = !bRepeat;
        }

        private void FormOperation_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmUpdate.Enabled = false;
            tmUpdateArray.Enabled = false;
            DM.SaveMap();
        }

        private void btTimeReset_Click(object sender, EventArgs e)
        {
            
          
        }

        private void btUPHReset_Click(object sender, EventArgs e)
        {
            
        }

        private void btWrkCntReset_Click(object sender, EventArgs e)
        {
            
        }

        private void FormOperation_Shown(object sender, EventArgs e)
        {
//            EmbededVision.SetVisionParent(pnCamera.Handle);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //if(SM.IO.GetY((int)yi.PCK_VacLtOn) && SM.IO.GetY((int)yi.PCK_VacRtOn))
            //{
            //    SM.IO.SetY((int)yi.PCK_VacLtOn, false);
            //    SM.IO.SetY((int)yi.PCK_VacRtOn, false);
            //    
            //    SM.IO.SetY((int)yi.PCK_EjtLtOn, false);
            //    SM.IO.SetY((int)yi.PCK_EjtRtOn, false);
            //}
            //else
            //{
            //    SM.IO.SetY((int)yi.PCK_VacLtOn, true);
            //    SM.IO.SetY((int)yi.PCK_VacRtOn, true);
            //
            //    SM.IO.SetY((int)yi.PCK_EjtLtOn, false);
            //    SM.IO.SetY((int)yi.PCK_EjtRtOn, false);
            //}

            
        }

        private void btManLensInsp_Click(object sender, EventArgs e)
        {
            //int iBtnTag = Convert.ToInt32(((Button)sender).Tag);
            //MM.m_bLens = true;
            //MM.SetManCycle(mc.ASY_CycleInsp);
        }

        private void btManHoldInsp_Click(object sender, EventArgs e)
        {
            //int iBtnTag = Convert.ToInt32(((Button)sender).Tag);
            //MM.m_bLens = false;
            //MM.SetManCycle(mc.ASY_CycleInsp);
        }

        private void cbConsecutively_CheckedChanged(object sender, EventArgs e)
        {
            //SEQ.ASY.bconsecutively = cbConsecutively.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DM.ARAY[(int)ri.LENS].SetStat(cs.Empty);
            DM.ARAY[(int)ri.FRNT].SetStat(cs.Work );
            DM.ARAY[(int)ri.LENS].SetStat(0, 0, cs.Unkwn);
            DM.ARAY[(int)ri.FRNT].SetStat(0, 0, cs.Unkwn);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            DM.ARAY[(int)ri.FRNT].SetStat(0, 0, cs.Unkwn);
            DM.ARAY[(int)ri.FRNT].SetStat(0, 1, cs.Work );
            DM.ARAY[(int)ri.FRNT].SetStat(0, 2, cs.Work );
            DM.ARAY[(int)ri.FRNT].SetStat(0, 3, cs.Work );
            DM.ARAY[(int)ri.FRNT].SetStat(0, 4, cs.Work );
            DM.ARAY[(int)ri.FRNT].SetStat(0, 5, cs.Work );
        }

        private void lbOption1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (FormPassword.GetLevel() == EN_LEVEL.lvMaster || FormPassword.GetLevel() == EN_LEVEL.lvEngineer)
            {
                int iBtnTag = Convert.ToInt32(((Label)sender).Tag);
                switch (iBtnTag)
                {
                    default: break;
                    case 1: OM.CmnOptn.bUseMultiHldr = !OM.CmnOptn.bUseMultiHldr; break;
                    case 2: OM.CmnOptn.bTorqChck     = !OM.CmnOptn.bTorqChck    ; break;
                }
                OM.SaveCmnOptn();
            }
        }

        private void pnOption1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (FormPassword.GetLevel() == EN_LEVEL.lvMaster || FormPassword.GetLevel() == EN_LEVEL.lvEngineer)
            {
                int iBtnTag = Convert.ToInt32(((Panel)sender).Tag);
                switch (iBtnTag)
                {
                    default: break;
                    case 1: OM.CmnOptn.bUseMultiHldr = !OM.CmnOptn.bUseMultiHldr; break;
                    case 2: OM.CmnOptn.bTorqChck     = !OM.CmnOptn.bTorqChck    ; break;
                }
                OM.SaveCmnOptn();
            }
        }

        private void panel10_VisibleChanged(object sender, EventArgs e)
        {

        }

        private void FormOperation_VisibleChanged(object sender, EventArgs e)
        {
            tmUpdate.Enabled = Visible;
            tmUpdateArray.Enabled = Visible;
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
