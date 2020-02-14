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
using System.Threading;

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

            btStart.Enabled = LOT.GetLotOpen();

            DM.ARAY[ri.WLDT].SetParent(pnWLDT); DM.ARAY[ri.WLDT].Name = "WLDT";
            DM.ARAY[ri.WLDB].SetParent(pnWLDB); DM.ARAY[ri.WLDB].Name = "WLDB";
            DM.ARAY[ri.SLDT].SetParent(pnSLDT); DM.ARAY[ri.SLDT].Name = "SLDT";
            DM.ARAY[ri.SLDB].SetParent(pnSLDB); DM.ARAY[ri.SLDB].Name = "SLDB";
            DM.ARAY[ri.WSTG].SetParent(pnWSTG); DM.ARAY[ri.WSTG].Name = "WSTG";
            DM.ARAY[ri.SSTG].SetParent(pnSSTG); DM.ARAY[ri.SSTG].Name = "SSTG";
            DM.ARAY[ri.PCKR].SetParent(pnPCKR); DM.ARAY[ri.PCKR].Name = "PCKR";

            //Wafer Loader Top Array            
            DM.ARAY[ri.WLDT].SetDisp(cs.None      , "None"      ,Color.White       );
            DM.ARAY[ri.WLDT].SetDisp(cs.Unknown   , "UnKnown"   ,Color.Aqua        );
            DM.ARAY[ri.WLDT].SetDisp(cs.Mask      , "Mask"      ,Color.Gray        );
            DM.ARAY[ri.WLDT].SetDisp(cs.Empty     , "Empty"     ,Color.Silver      );      
            DM.ARAY[ri.WLDT].SetDisp(cs.WorkEnd   , "WorkEnd"   ,Color.Blue        );   
                                                                                   
            //Wafer Loader Btm Array                                               
            DM.ARAY[ri.WLDB].SetDisp(cs.None      , "None"      ,Color.White       );
            DM.ARAY[ri.WLDB].SetDisp(cs.Unknown   , "UnKnown"   ,Color.Aqua        );
            DM.ARAY[ri.WLDB].SetDisp(cs.Mask      , "Mask"      ,Color.Gray        );
            DM.ARAY[ri.WLDB].SetDisp(cs.Empty     , "Empty"     ,Color.Silver      );      
            DM.ARAY[ri.WLDB].SetDisp(cs.WorkEnd   , "WorkEnd"   ,Color.Blue        );  
                                                                                   
            //Substrate Loader Top Array                                               
            DM.ARAY[ri.SLDT].SetDisp(cs.None      , "None"      ,Color.White       );
            DM.ARAY[ri.SLDT].SetDisp(cs.Unknown   , "UnKnown"   ,Color.Aqua        );
            DM.ARAY[ri.SLDT].SetDisp(cs.Mask      , "Mask"      ,Color.Gray        );
            DM.ARAY[ri.SLDT].SetDisp(cs.Empty     , "Empty"     ,Color.Silver      );   
            DM.ARAY[ri.SLDT].SetDisp(cs.WorkEnd   , "WorkEnd"   ,Color.Blue        );
                                                                                   
            //Substrate Loader Btm Array                                               
            DM.ARAY[ri.SLDB].SetDisp(cs.None      , "None"      ,Color.White       );
            DM.ARAY[ri.SLDB].SetDisp(cs.Unknown   , "UnKnown"   ,Color.Aqua        );
            DM.ARAY[ri.SLDB].SetDisp(cs.Mask      , "Mask"      ,Color.Gray        );
            DM.ARAY[ri.SLDB].SetDisp(cs.Empty     , "Empty"     ,Color.Silver      );   
            DM.ARAY[ri.SLDB].SetDisp(cs.WorkEnd   , "WorkEnd"   ,Color.Blue        );
                                                                                   
            //Wafer Stage Array                                                    
            DM.ARAY[ri.WSTG].SetDisp(cs.None      , "None"      ,Color.White       ); 
            DM.ARAY[ri.WSTG].SetDisp(cs.EjectAlign, "EjectAlign",Color.LightSkyBlue);
            DM.ARAY[ri.WSTG].SetDisp(cs.Eject     , "Eject"     ,Color.Aqua        ); 
            DM.ARAY[ri.WSTG].SetDisp(cs.Align     , "Align"     ,Color.Yellow      ); 
            DM.ARAY[ri.WSTG].SetDisp(cs.Pick      , "Pick"      ,Color.Lime        ); 
            DM.ARAY[ri.WSTG].SetDisp(cs.Empty     , "Empty"     ,Color.Silver      ); 
            DM.ARAY[ri.WSTG].SetDisp(cs.Fail      , "Fail"      ,Color.Red         ); 
                                                                                   
                                   


            //Substrate Stage Array                                                                                                  
            DM.ARAY[ri.SSTG].SetDisp(cs.None      , "None"      ,Color.White       ); 
            DM.ARAY[ri.SSTG].SetDisp(cs.Align     , "Align"     ,Color.Yellow      ); 
            DM.ARAY[ri.SSTG].SetDisp(cs.Empty     , "Empty"     ,Color.Silver      ); 
            DM.ARAY[ri.SSTG].SetDisp(cs.SubHeight , "SubHeight" ,Color.Green       ); 
            DM.ARAY[ri.SSTG].SetDisp(cs.Disp      , "Disp"      ,Color.Linen       ); 
            DM.ARAY[ri.SSTG].SetDisp(cs.DispVisn  , "DispVisn"  ,Color.Plum        ); 
            DM.ARAY[ri.SSTG].SetDisp(cs.Attach    , "Attach"    ,Color.Orange      ); 
            DM.ARAY[ri.SSTG].SetDisp(cs.EndVisn   , "EndVisn"   ,Color.SkyBlue     ); 
            DM.ARAY[ri.SSTG].SetDisp(cs.EndHeight , "EndHeight" ,Color.Violet      ); 
            DM.ARAY[ri.SSTG].SetDisp(cs.BltHeight , "BltHeight" ,Color.Maroon      );
            DM.ARAY[ri.SSTG].SetDisp(cs.WorkEnd   , "WorkEnd"   ,Color.Blue        ); 
                                                                                   
            //Tool Array                                                                                                  
            DM.ARAY[ri.PCKR].SetDisp(cs.None      , "None"      ,Color.White       ); 
            DM.ARAY[ri.PCKR].SetDisp(cs.Align     , "Align"     ,Color.Yellow      ); 
            DM.ARAY[ri.PCKR].SetDisp(cs.BtmVisn   , "BtmVisn"   ,Color.Purple      ); 
            DM.ARAY[ri.PCKR].SetDisp(cs.Attach    , "Attach"    ,Color.Orange      ); 
            DM.ARAY[ri.PCKR].SetDisp(cs.Distance  , "Distance"  ,Color.Salmon      ); 
                                                                                                                                                                            
            DM.ARAY[ri.WLDT].SetMaxColRow(1                        , OM.DevInfo.iWLDR_SlotCnt);
            DM.ARAY[ri.WLDB].SetMaxColRow(1                        , OM.DevInfo.iWLDR_SlotCnt);
            DM.ARAY[ri.SLDT].SetMaxColRow(1                        , OM.DevInfo.iSLDR_SlotCnt);
            DM.ARAY[ri.SLDB].SetMaxColRow(1                        , OM.DevInfo.iSLDR_SlotCnt);
            DM.ARAY[ri.WSTG].SetMaxColRow(OM.DevInfo.iWFER_DieCntX , OM.DevInfo.iWFER_DieCntY);
            DM.ARAY[ri.SSTG].SetMaxColRow(1                        , OM.DevInfo.iSBOT_PcktCnt);
            DM.ARAY[ri.PCKR].SetMaxColRow(1                        , 1                       );
        
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
            LOT.LotEnd();
            DM.ARAY[ri.WLDT].SetStat(cs.None);
            DM.ARAY[ri.WLDB].SetStat(cs.None);
            DM.ARAY[ri.SLDT].SetStat(cs.None);
            DM.ARAY[ri.SLDB].SetStat(cs.None);
            DM.ARAY[ri.WSTG].SetStat(cs.None);
            DM.ARAY[ri.SSTG].SetStat(cs.None);
            DM.ARAY[ri.PCKR].SetStat(cs.None);

            btStart.Enabled = false;
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
                case (int)EN_LEVEL.Operator: btOperator.Text = "OPERATOR"; break;
                case (int)EN_LEVEL.Engineer: btOperator.Text = "ENGINEER"; break;
                case (int)EN_LEVEL.Master  : btOperator.Text = " ADMIN  "; break;
                default                    : btOperator.Text = " ERROR  "; break;
            }
            
            lbSstBarcode.Text = "Substrate: "  + OM.EqpStat.sSSTGBarcode;
            lbWfrBarcode.Text = "Wafer: " + OM.EqpStat.sWSTGBarcode;


            //if (bPreLotOpen != LOT.GetLotOpen())
            //{
            
                btStart.Enabled = LOT.GetLotOpen();
                //bPreLotOpen = LOT.GetLotOpen();
            //}


            //SPC.LOT.DispLotInfo(lvLotInfo);
            //SPC.DAY.DispDayInfo(lvDayInfo);

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

            if(SM.IO_GetY(yi.ETC_LightOn)){
                btLightOn.Text = "Light On";
                btLightOn.BackColor = Color.Lime ;
            }
            else {
                btLightOn.Text = "Light Off";
                btLightOn.BackColor = Color.Silver ;
            }
            
            if(!SML.MT.GetHomeDoneAll()){
                btAllHome.ForeColor = SEQ._bFlick ? Color.Black : Color.Red;
            }
            else {
                btAllHome.ForeColor = Color.Black  ;
            }

            lbWorkInfo1.Text = SEQ.Dispr.GetDispData().dVacPres.ToString();
            lbWorkInfo0.Text = SEQ.Dispr.GetDispData().dPrsPres.ToString();

            lbTWfrWork.Text = SM.PM_GetValue(mi.WSTG_TTble,pv.WSTG_TTbleWfrWork).ToString();


            //Ver 
            DispDayList();
            DispLotInfo();

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

            lvLotInfo.Columns.Add("", 185, HorizontalAlignment.Left);     //1280*1024는 109, 1024*768은 78
            lvLotInfo.Columns.Add("", 255, HorizontalAlignment.Left);     //1280*1024는 109, 1024*768은 78
        
            //Ver 1.0.4.2
            ListViewItem[] liLotInfo = new ListViewItem[7];
            liLotInfo[0] = new ListViewItem("Lot No");
            liLotInfo[0].SubItems.Add(LOT.m_sLotNo);
            liLotInfo[1] = new ListViewItem("Job File");
            liLotInfo[1].SubItems.Add(LOT.m_sJobName);

            //Ver 1.0.5.0
            //Day WorkTime, Day StopTime, Day ErrorTime, CycleTime, Day WorkCount 추가
            DateTime tWorkTime  = DateTime.FromOADate(SPC.DAY.Data.dWorkTime );
            DateTime tStopTime  = DateTime.FromOADate(SPC.DAY.Data.dStopTime );
            DateTime tErrTime   = DateTime.FromOADate(SPC.DAY.Data.dErrTime  );
            DateTime tCycleTime = DateTime.FromOADate(SPC.LOT.Data.dCycleTime);

            liLotInfo[2] = new ListViewItem("Day WorkTime");
            liLotInfo[2].SubItems.Add(tWorkTime.ToString("HH:mm:ss")); 
            liLotInfo[3] = new ListViewItem("Day StopTime");
            liLotInfo[3].SubItems.Add(tStopTime.ToString("HH:mm:ss"));
            liLotInfo[4] = new ListViewItem("Day ErrorTime");
            liLotInfo[4].SubItems.Add(tErrTime.ToString("HH:mm:ss"));
            liLotInfo[5] = new ListViewItem("1Cycle Time");
            liLotInfo[5].SubItems.Add(tCycleTime.ToString("mm:ss"));
            liLotInfo[6] = new ListViewItem("Day WorkCount");
            liLotInfo[6].SubItems.Add(SPC.DAY.Data.iWorkCnt.ToString());
            //DateTime.FromOADate(Data.dWorkTime);

            lvLotInfo.Items.Add(liLotInfo[0]);
            lvLotInfo.Items.Add(liLotInfo[1]);
            lvLotInfo.Items.Add(liLotInfo[2]);
            lvLotInfo.Items.Add(liLotInfo[3]);
            lvLotInfo.Items.Add(liLotInfo[4]);
            lvLotInfo.Items.Add(liLotInfo[5]);
            lvLotInfo.Items.Add(liLotInfo[6]);


            //            for (int i = 0; i < LotInfoCnt; i++)
            //            {
            //                liLotInfo[i] = new ListViewItem();
            //                liLotInfo[i].SubItems.Add(LOT.m_sLotNo  );
            //                liLotInfo[i].SubItems.Add(LOT.m_sJobName);
            //        
            //        
            //                liLotInfo[i].UseItemStyleForSubItems = false;
            //                liLotInfo[i].UseItemStyleForSubItems = false;
            //
            //                lvLotInfo.Items.Add(liLotInfo[i]);
            //                
            //            }

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
            if(FormPassword.GetLevel() != EN_LEVEL.Master) return ;

            if (Log.ShowMessageModal("Confirm", "Clear Day Info?") != DialogResult.Yes) return;
            
            SPC.DAY.ClearData() ;
        }

        private void btAllHome_Click(object sender, EventArgs e)
        {
            if (!OM.MstOptn.bDebugMode)
            {
                if (Log.ShowMessageModal("Confirm", "Do you want to All Homming?") != DialogResult.Yes) return;
                MM.SetManCycle(mc.AllHome);
            }
            else
            {
                DialogResult Rslt ;
                Rslt = MessageBox.Show("홈동작을 생략 하겠습니까?", "Confirm", MessageBoxButtons.YesNoCancel);
                if (Rslt == DialogResult.Yes)
                {
                    SM.MT_SetServoAll(true);
                    Thread.Sleep(100);
                    for (mi i = 0; i < mi.MAX_MOTR; i++)
                    {
                        SM.MT_SetHomeDone(i, true);
                    }
                }
                else if(Rslt == DialogResult.No)
                {
                    MM.SetManCycle(mc.AllHome);
                }

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
            DM.SaveMap();
        }

        private void FormOperation_Shown(object sender, EventArgs e)
        {
            
        }

        private void pnOption1_DoubleClick(object sender, EventArgs e)
        {
            int iBtnTag = Convert.ToInt32(((Panel)sender).Tag);
            switch (iBtnTag)
            {
                default : break;
                case 1: OM.CmnOptn.bIgnrDoor    = !OM.CmnOptn.bIgnrDoor   ; break;
                //case 2: OM.CmnOptn.bVisnSkip    = !OM.CmnOptn.bVisnSkip   ; break;
                //case 3: OM.CmnOptn.bMrkingSkip  = !OM.CmnOptn.bMrkingSkip ; break;
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
                //case 2: OM.CmnOptn.bVisnSkip    = !OM.CmnOptn.bVisnSkip   ; break;
                //case 3: OM.CmnOptn.bMrkingSkip  = !OM.CmnOptn.bMrkingSkip ; break;
            }
            OM.SaveCmnOptn();
        }

        private void btManual1_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace("FormOperation" ,  sText + " Button Clicked");

            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);
            MM.SetManCycle((mc)iBtnTag);
        }

        private void panel11_Paint(object sender, PaintEventArgs e)
        {
        }

        int iStep = -6;
        bool bPos = true;
        private void tmRot_Tick(object sender, EventArgs e)
        {
            if(SM.MT_GetStop(mi.SSTG_YRght )&&
               SM.MT_GetStop(mi.SSTG_YLeft )&&
               SM.MT_GetStop(mi.SSTG_XFrnt )){
                if(bPos)
                {
                    iStep++;
                    if (iStep > 6)
                    {
                        bPos = false;
                        iStep -= 2;
                    }
                    
                }
                else
                {
                    iStep--;
                    if (iStep < -6)
                    {
                        bPos = true;
                        iStep += 2;
                    }
                }

                SM.MT_GoAbsRun(mi.SSTG_YRght, -iStep);
                SM.MT_GoAbsRun(mi.SSTG_YLeft, iStep );
                SM.MT_GoAbsRun(mi.SSTG_XFrnt, -iStep);


            }


        }

        private void button1_Click(object sender, EventArgs e)
        {

            SEQ.WSTG.SetMapFile(ri.WSTG , tbWaferBar.Text);
            //SPC.WRK.Data.LotNo      = "gggg";
            //SPC.WRK.Data.Device     = "dddd";
            //SPC.WRK.Data.StartTime  = DateTime.Now.ToOADate()-0.1;
            //SPC.WRK.Data.EndTime    = DateTime.Now.ToOADate();
            //SPC.WRK.Data.LeftGapX   = 3.0;
            //SPC.WRK.Data.LeftGapY   = 4.0;
            //SPC.WRK.Data.RghtGapX   = 5.0;
            //SPC.WRK.Data.RghtGapY   = 6.0;
            //SPC.WRK.Data.SubHeight  = "sdfsdf";
            //SPC.WRK.Data.EndHeight  = "sdsddf";
            //SPC.WRK.Data.GapHeight  = "sdgg"  ;
            //SPC.WRK.Data.GapHeight  = "sdsagd";
            //SPC.WRK.Data.BltHeight  = "dfsdgsgd";

            //SPC.WRK.SaveDataIni();

            //tmRot.EnablsBltHght ed = !tmRot.Enabled;
            //SEQ.Dispr.SetPTV(OM.DevOptn.dDspPrsPres, 10, OM.DevOptn.dDspVacPres);
        }

        delegate void SendMsg(string _sMsg);
        public void SendListMsg(string _sMsg)
        {
            lvLog.Invoke(new SendMsg(ListMsg), new string[]{_sMsg});
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

        private void btClear_Click(object sender, EventArgs e)
        {
            lvLog.Items.Clear();
        }

        private void btLightOn_Click(object sender, EventArgs e)
        {
            SM.IO_SetY(yi.ETC_LightOn , !SM.IO_GetY(yi.ETC_LightOn));
        }

        private void button4_Click(object sender,EventArgs e) 
        {
            string sText = ((Button)sender).Text;
            Log.Trace("FormOperation" ,  sText + " Button Clicked");

            SEQ.SSTG.MoveMotr(mi.SLDR_ZElev, pv.SLDR_ZElevWait);
            SEQ.WSTG.MoveMotr(mi.WLDR_ZElev , pv.WLDR_ZElevWait);
        }

        private void btTWfrWork_Click(object sender, EventArgs e)
        {
            double dVal = CConfig.StrToDoubleDef(tbTWfrWork.Text , SM.PM_GetValue(mi.WSTG_TTble,pv.WSTG_TTbleWfrWork));

            PM.SetValue(mi.WSTG_TTble,pv.WSTG_TTbleWfrWork,dVal);
        }

        private void label1_Click(object sender, EventArgs e)
        {
            SEQ.TOOL.Test();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            int iWorkC ;
            int iWorkR ;

            int.TryParse(tbWorkCol.Text , out iWorkC);
            int.TryParse(tbWorkRow.Text , out iWorkR);


            SEQ.TOOL.SetWorkMapFile(ri.WSTG, iWorkC , iWorkR , tbWaferBar.Text);
        }

        private void cbContiWork_CheckedChanged(object sender, EventArgs e)
        {
            SEQ.TOOL.m_bContiWork = cbContiWork.Checked ;
        }

        private void label5_Click(object sender, EventArgs e)
        {

            //            if(!DM.ARAY[ri.SSTG].CheckAllStat(cs.None)) DM.ARAY[ri.SSTG].SetStat(cs.WorkEnd); //서브스트레이트 스테이지에 있는 자제 다 워크엔드 시키고.
            //            DM.ARAY[ri.SLDB].ChangeStat(cs.Unknown , cs.WorkEnd);  //로더에 언노운 있던것을 다 WorkEnd시킨다.
            //            DM.ARAY[ri.SLDT].ChangeStat(cs.Unknown , cs.WorkEnd);
            //
            //
            //
            //if(!DM.ARAY[ri.WSTG].CheckAllStat(cs.None)) DM.ARAY[ri.WSTG].SetStat(cs.Empty); //서브스트레이트 스테이지에 있는 자제 다 워크엔드 시키고.
            //            DM.ARAY[ri.WLDB].ChangeStat(cs.Unknown , cs.WorkEnd);  //로더에 언노운 있던것을 다 WorkEnd시킨다.
            //            DM.ARAY[ri.WLDT].ChangeStat(cs.Unknown , cs.WorkEnd);
        }

        private void label11_Click(object sender, EventArgs e)
        {
            //int iAray = 0 ;
            //int iC = 0 ; 
            //int iR = 0 ;
            //
            //if (SEQ.SSTG.FindChip(out iAray , out iC , out iR, cs.Unknown) && iAray == ri.SLDT)
            //{
            //    MoveMotr(mi.SLDR_ZElev, pv.SLDR_ZElevTopWorkStt, iR * OM.DevInfo.dSLDR_SlotPitch);
            //    
            //}
            //else if (SEQ.SSTG.FindChip(out iAray , out iC , out iR, cs.Unknown) && iAray == ri.SLDB)
            //{
            //    MoveMotr(mi.SLDR_ZElev, pv.SLDR_ZElevBtmWorkStt, iR * OM.DevInfo.dSLDR_SlotPitch);    
            //}

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
