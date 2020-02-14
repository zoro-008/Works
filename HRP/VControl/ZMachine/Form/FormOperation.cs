using System;
using System.Drawing;
using System.Windows.Forms;
using COMMON;
using SML;
using System.Reflection;
using System.Threading;
using System.Runtime.InteropServices;
using VDll;

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

        private const string sFormText = "Form Operation ";
        //public EN_SEQ_STAT m_iSeqStat;
        
        [DllImport("Kernel32.dll")]
        public static extern bool IsWow64Process(System.IntPtr hProcess, out bool lpSystemInfo);
        public FormOperation(Panel _pnBase)
        {
            InitializeComponent();

            this.TopLevel = false;
            this.Parent = _pnBase;
            
            DispLotInfo();
            m_tmStartBt = new CDelayTimer();

            //Loader           
            DM.ARAY[ri.MOVE].SetDisp(cs.None      , "None"            ,Color.White        );
            DM.ARAY[ri.MOVE].SetDisp(cs.Work      , "Work"            ,Color.Yellow       );
            DM.ARAY[ri.MOVE].SetDisp(cs.Unknown   , "UnKnown"         ,Color.Aqua         );
                                                                                     

            


            DM.LoadMap();


            var path = new System.Drawing.Drawing2D.GraphicsPath();



            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            bool bRet = false;
            IsWow64Process(System.Diagnostics.Process.GetCurrentProcess().Handle, out bRet);
            if(bRet) MessageBox.Show("32bit");
            VL.TPara Prop ;
            Prop.UtilFolder   = sExeFolder + "VisnUtil\\";
            Prop.DeviceFolder = sExeFolder + "JobFile\\";
            Prop.VisionCnt = 2 ;
            Prop.VisionNames = new string[Prop.VisionCnt];
            Prop.VisionNames[0] = "Main";
            Prop.VisionNames[1] = "Sub" ;

            tcVision.TabPages[0].Text = Prop.VisionNames[0] ;
            tcVision.TabPages[1].Text = Prop.VisionNames[1] ;

            VL.Init(Prop);      
      

            VL.SetVisonForm(0 , ref  pnVision1);
            VL.SetVisonForm(1 , ref  pnVision2);

            ML.MT_SetServoAll(true);
                    //Thread.Sleep(100);
                    for (mi i = 0; i < mi.MAX_MOTR; i++)
                    {
                        ML.MT_SetHomeDone(i, true);
                    }
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

        }


        private static bool bPreLotOpen = false;
        private static int iTrayImg = 0;



        private bool bPreTrainning = false ;
        private void timer1_Tick(object sender, EventArgs e)
        {
            bool bTrainning = VL.GetTraining(0) ;
            if(!bPreTrainning && bTrainning)
            {
                ML.MT_SetY(mi.MOVE_X, 3, true);
            }
            else if(bPreTrainning && !bTrainning)
            {
                ML.MT_SetY(mi.MOVE_X, 3, false);
            }
            

             
            if (!this.Visible)
            {
                return;
            }


            //로그인/로그아웃 방식
            if (SM.FrmLogOn.GetLevel() == (int)EN_LEVEL.LogOff)
            {
                btOperator.Text = "  LOG IN";


                pnOperMan .Enabled = false;
                //pnWorkInfo.Enabled = false;
                //pnLotOpen .Enabled = true;
                //btStart   .Enabled = LOT.LotList.Count == 0 || !LOT.LotOpened;
                btStart   .Enabled = true;
                btStop    .Enabled = true;
                btReset   .Enabled = true;
                btHome .Enabled    = true;
                btOperator.Enabled = true;
            }
            else
            {


                btOperator.Text = "  " + SM.FrmLogOn.GetLevel().ToString();

                //pnDataMap .Enabled = true;
                //pnDayInfo .Enabled = true;
                //pnLotInfo .Enabled = true;
                //pnError   .Enabled = true;
                pnOperMan .Enabled = true;
                //pnWorkInfo.Enabled = true;
                //pnLotOpen .Enabled = true;
                //btStart   .Enabled = LOT.LotList.Count != 0 || LOT.LotOpened;
                btStart   .Enabled = true;
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
            //if (iPreErrCnt != iCrntErrCnt ) 
            //{
            //    lbErr.Items.Clear();
            //    int iErrNo = ML.ER_GetLastErr();
            //    for (int i = 0; i < ML.ER_MaxCount(); i++) 
            //    {
            //        if (ML.ER_GetErr((ei)i))
            //        {
            //            Str = string.Format("[ERR{0:000}]" , i) ;
            //            Str += ML.ER_GetErrName(i) + " " + ML.ER_GetErrSubMsg((ei)i);
            //            lbErr.Items.Add(Str);
            //        }
            //    }
            //}
            //if (SEQ._iSeqStat != EN_SEQ_STAT.Error)
            //{
            //    lbErr.Items.Clear();
            //}
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

            //SPC.LOT.DispLotInfo(lvLotInfo);

            ////Refresh();
            //pnULDR.Refresh();
            //pnPSHR.Refresh();
            //pnPICK.Refresh();
            //pnPULD.Refresh();
            //pnTULD.Refresh();
            //pnTMRK.Refresh();
            //pnTRJM.Refresh();
            //pnTVSN.Refresh();
            //pnTLDR.Refresh();
            //pnTRJV.Refresh();
            //pnMOVE.Refresh();
            //pnLODR.Refresh();


            //if(ML.IO_GetY(yi.ETC_LightOn)) {btlightOn.Text     = " LIGHT ON " ; }//btlightOn.BackColor = Color.Lime;}
            //else                           {btlightOn.Text     = " LIGHT OFF" ; }//btlightOn.BackColor = Color.Red ;}
            //
            //if(ML.IO_GetY(yi.MARK_Light )) {btMarklightOn.Text = " MARK LIGHT ON " ; }//btlightOn.BackColor = Color.Lime;}
            //else                           {btMarklightOn.Text = " MARK LIGHT OFF" ; }//btlightOn.BackColor = Color.Red ;}

            
            
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
            //lvLotInfo.Clear();
            //lvLotInfo.View = View.Details;
            //lvLotInfo.LabelEdit = true;
            //lvLotInfo.AllowColumnReorder = true;
            //lvLotInfo.FullRowSelect = true;
            //lvLotInfo.GridLines = true;
            //lvLotInfo.Sorting = SortOrder.Descending;
            //lvLotInfo.Scrollable = true;
            //lvLotInfo.Enabled = false;

            ////lvLotInfo.Columns.Add("", 200, HorizontalAlignment.Left);     //1280*1024는 109, 1024*768은 78
            ////lvLotInfo.Columns.Add("", 300, HorizontalAlignment.Left);     //1280*1024는 109, 1024*768은 78
            //lvLotInfo.Columns.Add("", 125, HorizontalAlignment.Left);     //1280*1024는 109, 1024*768은 78
            //lvLotInfo.Columns.Add("", 90, HorizontalAlignment.Left);     //1280*1024는 109, 1024*768은 78

            //ListViewItem[] liLotInfo = new ListViewItem[LotInfoCnt];
        
            //for (int i = 0; i < LotInfoCnt; i++)
            //{
            //    liLotInfo[i] = new ListViewItem();
            //    liLotInfo[i].SubItems.Add("");
        
        
            //    liLotInfo[i].UseItemStyleForSubItems = false;
            //    liLotInfo[i].UseItemStyleForSubItems = false;

            //    liLotInfo[i].BackColor = Color.White;
            //    lvLotInfo.Items.Add(liLotInfo[i]);
                
            //}
        
            //var PropLotInfo = lvLotInfo.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            //PropLotInfo.SetValue(lvLotInfo, true, null);
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

        }

        private void FormOperation_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmUpdate.Enabled = false;
            DM.SaveMap();

            VL.Close();
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
            DM.ARAY[ri.MOVE].SetStat(cs.Unknown);
            //DM.ARAY[ri.PLDR].SetStat(cs.None   );
            //DM.ARAY[ri.TLDR].SetStat(cs.None   );
            //DM.ARAY[ri.TVSN].SetStat(cs.None   );
            //DM.ARAY[ri.TMRK].SetStat(cs.None   );
            //DM.ARAY[ri.TULD].SetStat(cs.None   );
            //DM.ARAY[ri.TRJM].SetStat(cs.None   );
            //DM.ARAY[ri.TRJV].SetStat(cs.None   );
            //DM.ARAY[ri.PULD].SetStat(cs.None   );
            //DM.ARAY[ri.ULDR].SetStat(cs.Empty  );
            //DM.ARAY[ri.PICK].SetStat(cs.None   );
            //DM.ARAY[ri.PSHR].SetStat(cs.None   );

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

            //if (!OM.MstOptn.bDebugMode)
            //{
            //    if (Log.ShowMessageModal("Confirm", "Do you want to All Homming?") != DialogResult.Yes) return;
            //    MM.SetManCycle(mc.AllHome);
            //}
            //else
            //{

            MM.SetManCycle(mc.AllHome);
                //DialogResult Rslt;
                //Rslt = MessageBox.Show("홈동작을 생략 하겠습니까?", "Confirm", MessageBoxButtons.YesNoCancel);
                //if (Rslt == DialogResult.Yes)
                //{
                //    
                //}
                //else if (Rslt == DialogResult.No)
                //{
                //    
                //}

            //}
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
            DM.ARAY[ri.MOVE].SetStat(cs.Unknown);
            //DM.ARAY[ri.PLDR].SetStat(cs.None   );
            //DM.ARAY[ri.TLDR].SetStat(cs.None   );
            //DM.ARAY[ri.TVSN].SetStat(cs.None   );
            //DM.ARAY[ri.TMRK].SetStat(cs.None   );
            //DM.ARAY[ri.TULD].SetStat(cs.None   );
            //DM.ARAY[ri.TRJM].SetStat(cs.None   );
            //DM.ARAY[ri.TRJV].SetStat(cs.None   );
            //DM.ARAY[ri.PULD].SetStat(cs.None   );
            //DM.ARAY[ri.ULDR].SetStat(cs.Empty  );
            //DM.ARAY[ri.PICK].SetStat(cs.None   );
            //DM.ARAY[ri.PSHR].SetStat(cs.None   );

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
            //ML.IO_SetY(yi.ETC_LightOn , !ML.IO_GetY(yi.ETC_LightOn));
        }

        private void WorkInfo()
        {
            //lbWorkInfo0.Text = OM.EqpStat.iULDRCnt.ToString();
            //lbWorkInfo1.Text = OM.EqpStat.iRJCMCnt.ToString();
            //lbWorkInfo2.Text = OM.EqpStat.iRJCVCnt.ToString();

        }

        public bool bTrgTest = false;
        private void button1_Click_1(object sender, EventArgs e)
        {
            bTrgTest = !bTrgTest;
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            timer1.Enabled = false;

            if(bTrgTest) SM.MTR.OneShotTrg((int)mi.MOVE_X, true, 100);

            timer1.Enabled = true;


        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            
        }

        private void button1_MouseDown(object sender, MouseEventArgs e)
        {
            ML.MT_JogN(mi.MOVE_X);
        }

        private void button1_MouseUp(object sender, MouseEventArgs e)
        {
            ML.MT_Stop(mi.MOVE_X);
        }

        private void button2_MouseDown(object sender, MouseEventArgs e)
        {
            ML.MT_JogP(mi.MOVE_X);
        }

        private void button2_MouseUp(object sender, MouseEventArgs e)
        {
            ML.MT_Stop(mi.MOVE_X);
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            //ML.MT_ResetTrgPos(mi.MOVE_X);
            ML.MT_GoInc(mi.MOVE_X,10,100,1000,1000);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ML.MT_GoInc(mi.MOVE_X,-10,100,1000,1000);
            //double[] dPos = { 111.2 };
            //
            //VL.Autorun(true);
            //VL.Ready(0);
            ////ML.MT_SetTrgPos(mi.MOVE_X,dPos,2,false,true);
            //SM.MTR.SetTrgAbs(0,true,50,2,false,true);
            ////ML.mt_set

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