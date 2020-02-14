using System;
using System.Drawing;
using System.Windows.Forms;
using COMMON;
using SML;
using System.Reflection;
using System.Threading;
using System.Runtime.InteropServices;

namespace Machine
{
    public partial class FormOperation : Form
    {
        public static FormOperation FrmOperation;
        public static FormLotOpen FrmLotOpen;
        public static FormMain FrmMain;

        public int LotInfoCnt = 6;
        public int DayInfoCnt = 6;        

        protected CDelayTimer m_tmStartBt ;

        //private string sFormText ;
        private const string sFormText = "Form Operation ";
        //public EN_SEQ_STAT m_iSeqStat;

        public FormOperation(Panel _pnBase)
        {
            InitializeComponent();

            //sFormText = this.GetType().Name;

            this.TopLevel = false;
            this.Parent = _pnBase;
            
            //DispDayList();
            DispLotInfo();

            MakeDoubleBuffered(pnULDR,true);
            MakeDoubleBuffered(pnPSHR,true);
            MakeDoubleBuffered(pnPICK,true);
            MakeDoubleBuffered(pnPULD,true);
            MakeDoubleBuffered(pnTULD,true);
            MakeDoubleBuffered(pnTMRK,true);
            MakeDoubleBuffered(pnTRJM,true);
            MakeDoubleBuffered(pnTVSN,true);
            MakeDoubleBuffered(pnTLDR,true);
            MakeDoubleBuffered(pnTRJV,true);
            MakeDoubleBuffered(pnPLDR,true);
            MakeDoubleBuffered(pnLODR,true);
            //tmUpdate.Enabled = true;

            //btStart.Enabled = LOT.GetLotOpen();

            m_tmStartBt = new CDelayTimer();

            DM.ARAY[ri.LODR].SetParent(pnLODR); DM.ARAY[ri.LODR].Name = "LODR";
            DM.ARAY[ri.PLDR].SetParent(pnPLDR); DM.ARAY[ri.PLDR].Name = "PLDR";
            DM.ARAY[ri.TLDR].SetParent(pnTLDR); DM.ARAY[ri.TLDR].Name = "TLDR";
            DM.ARAY[ri.TVSN].SetParent(pnTVSN); DM.ARAY[ri.TVSN].Name = "TVSN";
            DM.ARAY[ri.TMRK].SetParent(pnTMRK); DM.ARAY[ri.TMRK].Name = "TMRK";
            DM.ARAY[ri.TULD].SetParent(pnTULD); DM.ARAY[ri.TULD].Name = "TULD";
            DM.ARAY[ri.TRJM].SetParent(pnTRJM); DM.ARAY[ri.TRJM].Name = "TRJM";
            DM.ARAY[ri.TRJV].SetParent(pnTRJV); DM.ARAY[ri.TRJV].Name = "TRJV";
            DM.ARAY[ri.PULD].SetParent(pnPULD); DM.ARAY[ri.PULD].Name = "PULD";
            DM.ARAY[ri.ULDR].SetParent(pnULDR); DM.ARAY[ri.ULDR].Name = "ULDR";
            DM.ARAY[ri.PICK].SetParent(pnPICK); DM.ARAY[ri.PICK].Name = "PICK";
            DM.ARAY[ri.PSHR].SetParent(pnPSHR); DM.ARAY[ri.PSHR].Name = "PSHR";
            //DM.ARAY[ri.MASK].SetParent(pnSTCK); DM.ARAY[ri.PSTC].Name = "PSTC";
                        
            //Loader           
            DM.ARAY[ri.LODR].SetDisp(cs.None      , "None"            ,Color.White        );
            DM.ARAY[ri.LODR].SetDisp(cs.Work      , "Work"            ,Color.Yellow       );
            DM.ARAY[ri.LODR].SetDisp(cs.Unknown   , "UnKnown"         ,Color.Aqua         );
            DM.ARAY[ri.LODR].SetDisp(cs.Empty     , "Empty"           ,Color.Silver       ); 
                                                                                     
            //Index Rear                                                             
            DM.ARAY[ri.PLDR].SetDisp(cs.None      , "None"            ,Color.White        );
            DM.ARAY[ri.PLDR].SetDisp(cs.Unknown   , "UnKnown"         ,Color.Aqua         );

            //Index Rear                                                             
            DM.ARAY[ri.TLDR].SetDisp(cs.None      , "None"            ,Color.White        );
            DM.ARAY[ri.TLDR].SetDisp(cs.Unknown   , "UnKnown"         ,Color.Aqua         );

            //Index Front                                                           
            DM.ARAY[ri.TVSN].SetDisp(cs.None      , "None"            ,Color.White        );
            DM.ARAY[ri.TVSN].SetDisp(cs.Unknown   , "UnKnown"         ,Color.Aqua         );
            DM.ARAY[ri.TVSN].SetDisp(cs.Good      , "Good"            ,Color.Green        );
            DM.ARAY[ri.TVSN].SetDisp(cs.NGVisn    , "Visn Fail"       ,Color.Coral        );
      
            //Picker                                               
            DM.ARAY[ri.TMRK].SetDisp(cs.None      , "None"            ,Color.White        );
            DM.ARAY[ri.TMRK].SetDisp(cs.Unknown   , "UnKnown"         ,Color.Aqua         );
            DM.ARAY[ri.TMRK].SetDisp(cs.Good      , "Good"            ,Color.Green        );
            DM.ARAY[ri.TMRK].SetDisp(cs.NGVisn    , "Visn Fail"       ,Color.Coral        );
            DM.ARAY[ri.TMRK].SetDisp(cs.NGMark    , "Mark Fail"       ,Color.Red          );
                                                                                    
            //Fail Tray                                                           
            DM.ARAY[ri.TULD].SetDisp(cs.None      , "None"            ,Color.White        );
            DM.ARAY[ri.TULD].SetDisp(cs.Good      , "Good"            ,Color.Lime         );
            DM.ARAY[ri.TULD].SetDisp(cs.NGVisn    , "Visn Fail"       ,Color.Coral        );
            DM.ARAY[ri.TULD].SetDisp(cs.NGMark    , "Mark Fail"       ,Color.Red          );
            
            //Good Tray                                                               
            DM.ARAY[ri.TRJM].SetDisp(cs.None      , "None"            ,Color.White        );
            DM.ARAY[ri.TRJM].SetDisp(cs.NGVisn    , "Visn Fail"       ,Color.Coral        );
            DM.ARAY[ri.TRJM].SetDisp(cs.NGMark    , "Mark Fail"       ,Color.Red          );
                                                                                          
            //Out Zone                                                                   
            DM.ARAY[ri.TRJV].SetDisp(cs.None      , "None"            ,Color.White        );
            DM.ARAY[ri.TRJV].SetDisp(cs.NGVisn    , "Visn Fail"       ,Color.Coral        );
                                                                      
            //Out Zone                                                                   
            DM.ARAY[ri.PULD].SetDisp(cs.None      , "None"            ,Color.White        );
            DM.ARAY[ri.PULD].SetDisp(cs.Good      , "Good"            ,Color.Lime         );

            //Pre Stack Zone                                                                    
            DM.ARAY[ri.ULDR].SetDisp(cs.None      , "None"            ,Color.White        ); 
            DM.ARAY[ri.ULDR].SetDisp(cs.Work      , "Work"            ,Color.Yellow       );
            DM.ARAY[ri.ULDR].SetDisp(cs.WorkEnd   , "WorkEnd"         ,Color.Blue         );
            DM.ARAY[ri.ULDR].SetDisp(cs.Empty     , "Empty "          ,Color.Silver       );

            DM.ARAY[ri.PICK].SetDisp(cs.None      , "None"            ,Color.White        ); 
            DM.ARAY[ri.PICK].SetDisp(cs.Empty     , "Empty "          ,Color.Silver       );
            DM.ARAY[ri.PICK].SetDisp(cs.Unknown   , "Unknown "        ,Color.Aqua         );
            
            DM.ARAY[ri.PSHR].SetDisp(cs.None      , "None"            ,Color.White        ); 
            DM.ARAY[ri.PSHR].SetDisp(cs.Good      , "Good  "          ,Color.Lime         );
            


            DM.LoadMap();


            var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(0, 0, pnTMRK.Width, pnTMRK.Height);
            pnTMRK.Region = new Region(path);
            pnTULD.Region = new Region(path);
            pnTRJM.Region = new Region(path);
            pnTVSN.Region = new Region(path);
            pnTLDR.Region = new Region(path);
            pnTRJV.Region = new Region(path);
            pnPLDR.Region = new Region(path);
            pnPSHR.Region = new Region(path);
            pnPICK.Region = new Region(path);
            pnPULD.Region = new Region(path);


        }


        private void btOperator_Click(object sender, EventArgs e)
        {
            //pnPassWord.Visible = true;
            //if (FrmLogOn.m_iCrntLogIn == (int)EN_LOGIN.LogOut)
            //{
            //    FrmLogOn.ShowPage();
            //}
            SM.FrmLogOn.Show();
        }

        private void btLotEnd_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            LOT.LotEnd();
            DM.ARAY[ri.LODR].SetStat(cs.Unknown);
            DM.ARAY[ri.PLDR].SetStat(cs.None   );
            DM.ARAY[ri.TLDR].SetStat(cs.None   );
            DM.ARAY[ri.TVSN].SetStat(cs.None   );
            DM.ARAY[ri.TMRK].SetStat(cs.None   );
            DM.ARAY[ri.TULD].SetStat(cs.None   );
            DM.ARAY[ri.TRJM].SetStat(cs.None   );
            DM.ARAY[ri.TRJV].SetStat(cs.None   );
            DM.ARAY[ri.PULD].SetStat(cs.None   );
            DM.ARAY[ri.ULDR].SetStat(cs.Empty  );
            DM.ARAY[ri.PICK].SetStat(cs.None   );
            DM.ARAY[ri.PSHR].SetStat(cs.None   );
            btStart.Enabled = false;
        }


        private static bool bPreLotOpen = false;
        private static int iTrayImg = 0;

        [DllImport("Kernel32.dll")]
        public static extern bool IsWow64Process(System.IntPtr hProcess, out bool lpSystemInfo);


        private void timer1_Tick(object sender, EventArgs e)
        {
            tmUpdate.Enabled = false;

            //Information
            lbDevice.Text = OM.GetCrntDev().ToString();
            lbLotNo.Text  = LOT.GetLotNo();

            btStart.Enabled = LOT.GetLotOpen();
            btLotEnd.Enabled = LOT.GetLotOpen();

            bool bRet = false;
            IsWow64Process(System.Diagnostics.Process.GetCurrentProcess().Handle, out bRet);
            //if (bRet) label3.Text = "32BIt";
            //else label3.Text = "64BIt";

            //패스워드만 쓰는 방식
            //int iLevel = (int)FormPassword.GetLevel();
            //switch (iLevel)
            //{
            //    case (int)EN_LEVEL.Operator: btOperator.Text = "OPERATOR"; break;
            //    case (int)EN_LEVEL.Engineer: btOperator.Text = "ENGINEER"; break;
            //    case (int)EN_LEVEL.Master  : btOperator.Text = " ADMIN  "; break;
            //    default                    : btOperator.Text = " ERROR  "; break;
            //}

            //로그인/로그아웃 방식
            if (SM.FrmLogOn.GetLevel() == (int)EN_LEVEL.LogOff)
            {
                btOperator.Text = "  LOG IN";

                pnULDR.Enabled = false;
                pnPSHR.Enabled = false;
                pnPICK.Enabled = false;
                pnPULD.Enabled = false;
                pnTULD.Enabled = false;
                pnTMRK.Enabled = false;
                pnTRJM.Enabled = false;
                pnTVSN.Enabled = false;
                pnTLDR.Enabled = false;
                pnTRJV.Enabled = false;
                pnPLDR.Enabled = false;
                pnLODR.Enabled = false;

                //pnDataMap .Enabled = false;
                //pnDayInfo .Enabled = false;
                pnLotInfo .Enabled = false;
                pnError   .Enabled = false;
                pnOperMan .Enabled = false;
                //pnWorkInfo.Enabled = false;
                pnLotOpen .Enabled = false;
                //btStart   .Enabled = LOT.LotList.Count == 0 || !LOT.LotOpened;
                btStart   .Enabled = false;
                btStop    .Enabled = false;
                btReset   .Enabled = false;
                btHome .Enabled = false;
                btOperator.Enabled = true;
            }
            else
            {
                pnULDR.Enabled = true;
                pnPSHR.Enabled = true;
                pnPICK.Enabled = true;
                pnPULD.Enabled = true;
                pnTULD.Enabled = true;
                pnTMRK.Enabled = true;
                pnTRJM.Enabled = true;
                pnTVSN.Enabled = true;
                pnTLDR.Enabled = true;
                pnTRJV.Enabled = true;
                pnPLDR.Enabled = true;
                pnLODR.Enabled = true;

                btOperator.Text = "  " + SM.FrmLogOn.GetLevel().ToString();

                //pnDataMap .Enabled = true;
                //pnDayInfo .Enabled = true;
                pnLotInfo .Enabled = true;
                pnError   .Enabled = true;
                pnOperMan .Enabled = true;
                //pnWorkInfo.Enabled = true;
                pnLotOpen .Enabled = true;
                //btStart   .Enabled = LOT.LotList.Count != 0 || LOT.LotOpened;
                //btStart   .Enabled = true;
                btStop    .Enabled = true;
                btReset   .Enabled = true;
                btHome .Enabled = true;

            }
                        
            //if (SML.FrmLogOn.GetLevel() != (int)EN_LEVEL.LogOff)
            //{
            //    btStart.Enabled = LOT.GetLotOpen();
            //}

            TimeSpan Span ;
            try{
                    Span = TimeSpan.FromMilliseconds(SPC.LOT.Data.RunTime);
                }
                catch(Exception ex){          
                    Span = TimeSpan.FromMilliseconds(0);
                }
            
            //SPC.LOT.DispLotInfo(lvLotInfo);
            //SPC.DAY.DispDayInfo(lvDayInfo);

            string Str      ;
            int iPreErrCnt  = 0;
            int iCrntErrCnt = 0;
            for (int i = 0 ; i < ML.ER_MaxCount() ; i++) 
            {
                if (ML.ER_GetErr((ei)i)) iCrntErrCnt++;
            }
            if (iPreErrCnt != iCrntErrCnt ) 
            {
                lbErr.Items.Clear();
                int iErrNo = ML.ER_GetLastErr();
                for (int i = 0; i < ML.ER_MaxCount(); i++) 
                {
                    if (ML.ER_GetErr((ei)i))
                    {
                        Str = string.Format("[ERR{0:000}]" , i) ;
                        Str += ML.ER_GetErrName(i) + " " + ML.ER_GetErrSubMsg((ei)i);
                        lbErr.Items.Add(Str);
                    }
                }
            }
            if (SEQ._iSeqStat != EN_SEQ_STAT.Error)
            {
                lbErr.Items.Clear();
            }
            iPreErrCnt = iCrntErrCnt ;


            WorkInfo();
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
            
            if(!ML.MT_GetHomeDoneAll()){
                btAllHome.ForeColor = SEQ._bFlick ? Color.Black : Color.Red;
            }
            else {
                btAllHome.ForeColor = Color.Black  ;
            }

            SPC.LOT.DispLotInfo(lvLotInfo);

            //Refresh();
            //Invalidate(true);
            pnULDR.Invalidate(true);
            pnPSHR.Invalidate(true);
            pnPICK.Invalidate(true);
            pnPULD.Invalidate(true);
            pnTULD.Invalidate(true);
            pnTMRK.Invalidate(true);
            pnTRJM.Invalidate(true);
            pnTVSN.Invalidate(true);
            pnTLDR.Invalidate(true);
            pnTRJV.Invalidate(true);
            pnPLDR.Invalidate(true);
            pnLODR.Invalidate(true);
            //pnLODR.Update();


            if(ML.IO_GetY(yi.ETC_LightOn)) {btlightOn.Text     = " LIGHT ON " ; }//btlightOn.BackColor = Color.Lime;}
            else                           {btlightOn.Text     = " LIGHT OFF" ; }//btlightOn.BackColor = Color.Red ;}

            if(ML.IO_GetY(yi.MARK_Light )) {btMarklightOn.Text = " MARK LIGHT ON " ; }//btlightOn.BackColor = Color.Lime;}
            else                           {btMarklightOn.Text = " MARK LIGHT OFF" ; }//btlightOn.BackColor = Color.Red ;}

            
            if (!this.Visible)
            {
                tmUpdate.Enabled = false;
                return;
            }
            tmUpdate.Enabled = true;
            
        }

        private void btLotOpen_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

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
            lvLotInfo.Enabled = false;

            //lvLotInfo.Columns.Add("", 200, HorizontalAlignment.Left);     //1280*1024는 109, 1024*768은 78
            //lvLotInfo.Columns.Add("", 300, HorizontalAlignment.Left);     //1280*1024는 109, 1024*768은 78
            lvLotInfo.Columns.Add("", 125, HorizontalAlignment.Left);     //1280*1024는 109, 1024*768은 78
            lvLotInfo.Columns.Add("", 90, HorizontalAlignment.Left);     //1280*1024는 109, 1024*768은 78

            ListViewItem[] liLotInfo = new ListViewItem[LotInfoCnt];
        
            for (int i = 0; i < LotInfoCnt; i++)
            {
                liLotInfo[i] = new ListViewItem();
                liLotInfo[i].SubItems.Add("");
        
        
                liLotInfo[i].UseItemStyleForSubItems = false;
                liLotInfo[i].UseItemStyleForSubItems = false;

                liLotInfo[i].BackColor = Color.White;
                lvLotInfo.Items.Add(liLotInfo[i]);
                
            }
        
            var PropLotInfo = lvLotInfo.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropLotInfo.SetValue(lvLotInfo, true, null);
        }

        //public void DispDayList() //오퍼레이션 창용.
        //{
        //    lvDayInfo.Clear();
        //    lvDayInfo.View = View.Details;
        //    lvDayInfo.LabelEdit = true;
        //    lvDayInfo.AllowColumnReorder = true;
        //    lvDayInfo.FullRowSelect = true;
        //    lvDayInfo.GridLines = true;
        //    lvDayInfo.Sorting = SortOrder.Descending;
        //    lvDayInfo.Scrollable = true;

        //    lvDayInfo.Columns.Add("", 105, HorizontalAlignment.Left);    //1280*1024는 109, 1024*768은 78
        //    lvDayInfo.Columns.Add("", 105, HorizontalAlignment.Left);    //1280*1024는 109, 1024*768은 78

        //    ListViewItem[] liDayInfo = new ListViewItem[DayInfoCnt];

        //    for (int i = 0; i < DayInfoCnt; i++)
        //    {
        //        liDayInfo[i] = new ListViewItem();
        //        liDayInfo[i].SubItems.Add("");


        //        liDayInfo[i].UseItemStyleForSubItems = false;
        //        liDayInfo[i].UseItemStyleForSubItems = false;

        //        lvDayInfo.Items.Add(liDayInfo[i]);

        //    }

        //    var PropDayInfo = lvDayInfo.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
        //    PropDayInfo.SetValue(lvDayInfo, true, null);

        //    if (lvDayInfo == null) return;
        //}

        private void btStop_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            //OM.EqpStat.dLastIDXRPos = ML.MT_GetEncPos(mi.IDXR_XRear);
            //OM.EqpStat.dLastIDXFPos = ML.MT_GetEncPos(mi.IDXF_XFrnt);
            //OM.EqpStat.iLDRSplyCnt  = SEQ.LODR.iLDRSplyCnt;
            SEQ._bBtnStop = true;
        }

        private void btReset_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            SEQ._bBtnReset = true;
        }

        private void lvDayInfo_MouseDoubleClick(object sender, MouseEventArgs e)  //요거는 확인 해봐야 함 진섭
        {

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
                Rslt = MessageBox.Show(new Form(){TopMost = true}, "홈동작을 생략 하겠습니까?", "Confirm", MessageBoxButtons.YesNoCancel);
                if (Rslt == DialogResult.Yes)
                {
                    ML.MT_SetServoAll(true);
                    Thread.Sleep(100);
                    for (int i = 0; i < (int)mi.MAX_MOTR; i++)
                    {
                        ML.MT_SetHomeDone(i, true);
                    }
                }
                else if(Rslt == DialogResult.No)
                {
                    MM.SetManCycle(mc.AllHome);
                }

            }

            
        }

        private void FormOperation_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmUpdate.Enabled = false;
            DM.SaveMap();
        }

        private void btManual1_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);
            MM.SetManCycle((mc)iBtnTag);
        }
		
        private void btCylinder1_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);
            ML.CL_Move((ci)iBtnTag, ML.CL_GetCmd((ci)iBtnTag) == fb.Bwd ? fb.Fwd : fb.Bwd);
        }

        private void btOperator_Click_1(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            //pnPassWord.Visible = true;
            //if (FrmLogOn.m_iCrntLogIn == (int)EN_LOGIN.LogOut)
            //{
            //    FrmLogOn.ShowPage();
            //}
            SM.FrmLogOn.Show();
        }

        private void btLotOpen_Click_1(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            FrmLotOpen = new FormLotOpen();
            FrmLotOpen.Show();
        }

        private void btLotEnd_Click_1(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            LOT.LotEnd();
            DM.ARAY[ri.LODR].SetStat(cs.Unknown);
            DM.ARAY[ri.PLDR].SetStat(cs.None   );
            DM.ARAY[ri.TLDR].SetStat(cs.None   );
            DM.ARAY[ri.TVSN].SetStat(cs.None   );
            DM.ARAY[ri.TMRK].SetStat(cs.None   );
            DM.ARAY[ri.TULD].SetStat(cs.None   );
            DM.ARAY[ri.TRJM].SetStat(cs.None   );
            DM.ARAY[ri.TRJV].SetStat(cs.None   );
            DM.ARAY[ri.PULD].SetStat(cs.None   );
            DM.ARAY[ri.ULDR].SetStat(cs.Empty  );
            DM.ARAY[ri.PICK].SetStat(cs.None   );
            DM.ARAY[ri.PSHR].SetStat(cs.None   );

            btStart.Enabled = false;
        }

        private void btStart_Click_1(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            SEQ._bBtnStart = true;
        }

        private void btStop_Click_1(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            //OM.EqpStat.dLastIDXRPos = ML.MT_GetEncPos(mi.IDXR_XRear);
            //OM.EqpStat.dLastIDXFPos = ML.MT_GetEncPos(mi.IDXF_XFrnt);
            SEQ._bBtnStop = true;
        }

        private void btReset_Click_1(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            SEQ._bBtnReset = true;
        }

        private void lvDayInfo_MouseDoubleClick_1(object sender, MouseEventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            if (FormPassword.GetLevel() != EN_LEVEL.Master) return;

            if (Log.ShowMessageModal("Confirm", "Clear Day Info?") != DialogResult.Yes) return;
        }

        private void btAllHome_Click_1(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            if (!OM.MstOptn.bDebugMode)
            {
                if (Log.ShowMessageModal("Confirm", "Do you want to All Homming?") != DialogResult.Yes) return;
                MM.SetManCycle(mc.AllHome);
            }
            else
            {
                DialogResult Rslt;
                Rslt = MessageBox.Show("홈동작을 생략 하겠습니까?", "Confirm", MessageBoxButtons.YesNoCancel);
                if (Rslt == DialogResult.Yes)
                {
                    ML.MT_SetServoAll(true);
                    Thread.Sleep(100);
                    for (int i = 0; i < (int)mi.MAX_MOTR; i++)
                    {
                        ML.MT_SetHomeDone(i, true);
                    }
                }
                else if (Rslt == DialogResult.No)
                {
                    MM.SetManCycle(mc.AllHome);
                }

            }
        }

        private void btManual1_Click_1(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);

            MM.SetManCycle((mc)iBtnTag);
        }

        private void FormOperation_Shown(object sender, EventArgs e)
        {
            tmUpdate.Enabled = true;
        }

        private void btLotOpen_Click_2(object sender, EventArgs e)
        {
            FrmLotOpen = new FormLotOpen();
            FrmLotOpen.Show();
        }

        private void btLotEnd_Click_2(object sender, EventArgs e)
        {
            if (Log.ShowMessageModal("Confirm", "Do you want to LotEnd?") != DialogResult.Yes) return;

            LOT.LotEnd();
            DM.ARAY[ri.LODR].SetStat(cs.Unknown);
            DM.ARAY[ri.PLDR].SetStat(cs.None   );
            DM.ARAY[ri.TLDR].SetStat(cs.None   );
            DM.ARAY[ri.TVSN].SetStat(cs.None   );
            DM.ARAY[ri.TMRK].SetStat(cs.None   );
            DM.ARAY[ri.TULD].SetStat(cs.None   );
            DM.ARAY[ri.TRJM].SetStat(cs.None   );
            DM.ARAY[ri.TRJV].SetStat(cs.None   );
            DM.ARAY[ri.PULD].SetStat(cs.None   );
            DM.ARAY[ri.ULDR].SetStat(cs.Empty  );
            DM.ARAY[ri.PICK].SetStat(cs.None   );
            DM.ARAY[ri.PSHR].SetStat(cs.None   );

            btStart.Enabled = false;

        }
       
        private void FormOperation_VisibleChanged(object sender, EventArgs e)
        {
            if(this.Visible) tmUpdate.Enabled = true;
        }

        public void MakeDoubleBuffered(Control control, bool setting)
        {
            Type controlType = control.GetType();
            PropertyInfo pi = controlType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(control, setting, null);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ML.IO_SetY(yi.ETC_LightOn , !ML.IO_GetY(yi.ETC_LightOn));
        }

        private void WorkInfo()
        {
            lbWorkInfo0.Text = OM.EqpStat.iULDRCnt.ToString();
            lbWorkInfo1.Text = OM.EqpStat.iRJCMCnt.ToString();
            lbWorkInfo2.Text = OM.EqpStat.iRJCVCnt.ToString();

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            ML.IO_SetY(yi.MARK_Light , !ML.IO_GetY(yi.MARK_Light));
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            DM.ARAY[ri.LODR].SetMaxColRow(20,30);
            DM.ARAY[ri.LODR].Trace(1);
        }
    }

    public class DoubleBuffer : Panel
    {
        public DoubleBuffer()
        {
            this.SetStyle(ControlStyles.ResizeRedraw         , true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint            , true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint , true);

            this.UpdateStyles();
        }
    }
    public class DoubleBufferP : PictureBox
    {
        public DoubleBufferP()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.UserPaint |
              ControlStyles.AllPaintingInWmPaint  |
              ControlStyles.ResizeRedraw          |
              ControlStyles.ContainerControl      |
              ControlStyles.OptimizedDoubleBuffer |
              ControlStyles.SupportsTransparentBackColor
              , true);
        }
    }



}