using System;
using System.Drawing;
using System.Windows.Forms;
using COMMON;
using SML;
using System.Reflection;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;

namespace Machine
{
    public partial class FormOperation : Form
    {
        public static FormOperation FrmOperation;
        public static FormLotOpen FrmLotOpen;
        public static FormMain FrmMain;

        public int LotInfoCnt = 7;
        public int DayInfoCnt = 6;

        public int  iTickCnt = 0;
        public double dLoad1 = 0;
        public double dLoad2 = 0;
        public double dLoad3 = 0;
        protected CDelayTimer m_tmStartBt ;

        private const string sFormText = "Form Operation ";
        public EN_SEQ_STAT m_iPreStat;

        public FormCam FrmCam;
        EN_SEQ_STAT PreStat ;
        public FormOperation(Panel _pnBase)
        {
            InitializeComponent();

            this.TopLevel = false;
            this.Parent = _pnBase;
            this.Width  = _pnBase.Width;
            this.Height = _pnBase.Height;
            
            //Usb Cam
            FrmCam = new FormCam(pnCam);
            FrmCam.Show();
            
            SEQ.LEFT.CStart += new Machine.Left.CamVoid  (FrmCam.Start); 
            SEQ.LEFT.CStop  += new Machine.Left.CamVoid  (FrmCam.Stop );
            //SEQ.LEFT.CRec   += new Machine.Left.CamString(FrmCam.Rec  );

            SEQ.RIGH.CStart += new Machine.Righ.CamVoid  (FrmCam.Start);
            SEQ.RIGH.CStop  += new Machine.Righ.CamVoid  (FrmCam.Stop );
            //SEQ.RIGH.CRec   += new Machine.Righ.CamString(FrmCam.Rec  );

            //DispDayList();
            DispLotInfo();

            UpdateDevInfo(true);
            //MakeDoubleBuffered(groupBox5,true);

            //tmUpdate.Enabled = true;

            //btStart.Enabled = LOT.GetLotOpen();

            m_tmStartBt = new CDelayTimer();

            tabControl3.TabPages.Remove(tabPage6);

            //DM.ARAY[ri.ARAY].SetParent(pnSPLR); DM.ARAY[ri.ARAY].Name = "ARAY1";   
                        
            //Loader          
            //DM.ARAY[ri.ARAY].SetDisp(cs.None      , "None"            ,Color.White        );
            //DM.ARAY[ri.ARAY].SetDisp(cs.Unknown   , "UnKnown"         ,Color.Aqua         );
            //DM.ARAY[ri.ARAY].SetDisp(cs.Empty     , "Empty"           ,Color.Silver       ); 

            //DM.LoadMap();
        }

        public void Rec  (string _sPath = "") {
            string sPath = _sPath ;

            if (sPath == "")
            {
                sPath = @OM.CmnOptn.sLeftFolder + @"\" + DateTime.Now.ToString("yyyyMMdd") + @"\" + LOT.GetLotNo() + @"\" + 
                        LOT.GetLotNo() + DateTime.Now.ToString("(yyyyMMdd-hhmmss)") + ".avi";
            }

            FrmCam.Rec(sPath, pnCam.Width, pnCam.Height);
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
            DM.ARAY[ri.ARAY].SetStat(cs.None );
            btStart.Enabled = false;
        }


        private static bool bPreLotOpen = false;
        private static int iTrayImg = 0;

        [DllImport("Kernel32.dll")]
        public static extern bool IsWow64Process(System.IntPtr hProcess, out bool lpSystemInfo);


        private void timer1_Tick(object sender, EventArgs e)
        {
            tmUpdate.Enabled = false;

            //bool bRet = false;
            //IsWow64Process(System.Diagnostics.Process.GetCurrentProcess().Handle, out bRet);
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
                //pnDataMap .Enabled = false;
                //pnDayInfo .Enabled = false;
                btSaveDevice.Enabled  = false;
                //btSaveDevice1.Enabled = false;
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
                lbOptn1.Enabled = false;
                lbOptn2.Enabled = false;
            }
            else
            {
                btOperator.Text = "  " + SM.FrmLogOn.GetLevel().ToString();
                //pnDataMap .Enabled = true;
                //pnDayInfo .Enabled = true;
                btSaveDevice.Enabled = !SEQ._bRun;
                //btSaveDevice1.Enabled = !SEQ._bRun;
                pnLotInfo .Enabled = true;
                pnError   .Enabled = true;
                pnOperMan .Enabled = true;
                //pnWorkInfo.Enabled = true;
                pnLotOpen .Enabled = true;
                //btStart   .Enabled = LOT.LotList.Count != 0 || LOT.LotOpened;
                btStart   .Enabled = true;
                btStop    .Enabled = true;
                btReset   .Enabled = true;
                btHome .Enabled = true;
                lbOptn1.Enabled = !SEQ._bRun;
                lbOptn2.Enabled = !SEQ._bRun;

            }
            
            button6.Enabled = !SEQ._bRun && MM.GetManNo() == (int)mc.NoneCycle;
            button7.Enabled = !SEQ._bRun && MM.GetManNo() == (int)mc.NoneCycle;
            button8.Enabled = !SEQ._bRun && MM.GetManNo() == (int)mc.NoneCycle;

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

            if (OM.CmnOptn.bUse_L_Part)
            {
                pnOptn1.BackColor = Color.DimGray;
                lbOptn1.Text      = "USE BENDING";
            }
            else
            {
                pnOptn1.BackColor = Color.Red;
                lbOptn1.Text      = "IGNORE";
            }

            if (OM.CmnOptn.bUse_R_Part)
            {
                pnOptn2.BackColor = Color.DimGray;
                lbOptn2.Text      = "USE PULLING,BITING";
            }
            else
            {
                pnOptn2.BackColor = Color.Red;
                lbOptn2.Text      = "IGNORE";
            }
          
            
            //Door Sensor.  나중에 찾아보자
            //bool isAllCloseDoor = SM.IO.GetX((int)EN_INPUT_ID.xETC_DoorFt) &&
            //                      SM.IO.GetX((int)EN_INPUT_ID.xETC_DoorLt) &&
            //                      SM.IO.GetX((int)EN_INPUT_ID.xETC_DoorRt) &&
            //                      SM.IO.GetX((int)EN_INPUT_ID.xETC_DoorRr) ;
            //if (FormPassword.GetLevel() != EN_LEVEL.lvOperator && isAllCloseDoor && CMachine._bRun)
            //{
            //    //FM_SetLevel(lvOperator);
            //}
            
            //if(!ML.MT_GetHomeDoneAll()){
            //    btm1.ForeColor = SEQ._bFlick ? Color.Black : Color.Red;
            //}
            //else {
            //    btm1.ForeColor = Color.Black  ;
            //}

            SPC.LOT.DispLotInfo(lvLotInfo);




            //iTickCnt++;
            //dLoad1 += SEQ.AIO_GetX(ax.ETC_LoadCell1);
            //dLoad2 += SEQ.AIO_GetX(ax.ETC_LoadCell2);
            //dLoad3 += SEQ.AIO_GetX(ax.ETC_LoadCell3);

            //if (iTickCnt > 4)
            //{
            //    dLoad1 /= iTickCnt;
            //    dLoad2 /= iTickCnt;
            //    dLoad3 /= iTickCnt;
            
            //    lbl3.Text = dLoad1.ToString();//ML.AIO_GetX(ax.ETC_LoadCell1).ToString();
            //    lbl4.Text = dLoad2.ToString();//ML.AIO_GetX(ax.ETC_LoadCell2).ToString();
            //    lbr3.Text = dLoad3.ToString();//ML.AIO_GetX(ax.ETC_LoadCell3).ToString();
            //    iTickCnt = 0;
            //    dLoad1   = 0;
            //    dLoad2   = 0;
            //    dLoad3   = 0;
            //}
            if (OM.DevInfo.iL_Mode == (int)Mode.Height)
            {
                lbl1.Text = OM.DevInfo.iL_H_Count.ToString() ;
                lbl2.Text = SEQ.LEFT.iWorkCnt.ToString() ;
            }
            else if (OM.DevInfo.iL_Mode == (int)Mode.Pull_Dest)
            {
                lbl1.Text = "파괴모드";
                lbl2.Text = SEQ.LEFT.iDestCnt.ToString();
            }
            else if (OM.DevInfo.iL_Mode == (int)Mode.Weight)
            {
                lbl1.Text = OM.DevInfo.iL_W_Count.ToString();
                lbl2.Text = SEQ.LEFT.iWorkCnt.ToString();
            }
            else{
                lbl1.Text = OM.DevInfo.iL_G_Count.ToString();
                lbl2.Text = SEQ.LEFT.iWorkCnt.ToString();
            }


            if(OM.DevInfo.iR_Mode == (int)Mode.Height)
            {
                lbr1.Text = OM.DevInfo.iR_H_Count.ToString() ;
                lbr2.Text = SEQ.RIGH.iWorkCnt.ToString() ;
            }
            else if (OM.DevInfo.iR_Mode == (int)Mode.Weight)
            {
                lbr1.Text = OM.DevInfo.iR_W_Count.ToString();
                lbr2.Text = SEQ.RIGH.iWorkCnt.ToString();
            }
            else
            {
                lbr1.Text = OM.DevInfo.iR_P_Count.ToString();
                lbr2.Text = SEQ.RIGH.iWorkCnt.ToString();
            }
            
            
            //녹화하기
            EN_SEQ_STAT NowStat = SEQ._iSeqStat ;
            //FrmCam.Start();
            if (NowStat != m_iPreStat && NowStat == EN_SEQ_STAT.Running)
            {
                Log.Trace("REC",6);
                //FrmCam.RecStop();
                //FrmCam.Start();
                Rec();
            }
            if (NowStat != m_iPreStat && NowStat != EN_SEQ_STAT.Running)
            {
                Log.Trace("RECSTOP",6);
                //FrmCam.Stop();
                FrmCam.RecStop();
            }
            m_iPreStat = NowStat;

            double dN1 = CConfig.StrToDoubleDef(tbL_10.Text, 0) * 9.8;
            double dN2 = CConfig.StrToDoubleDef(tbL_21.Text, 0) * 9.8;
            double dN3 = CConfig.StrToDoubleDef(tbL_31.Text, 0) * 9.8;
            double dN4 = CConfig.StrToDoubleDef(tbR_10.Text, 0) * 9.8;
            double dN5 = CConfig.StrToDoubleDef(tbR_21.Text, 0) * 9.8;
            
            lbN1.Text = string.Format("{0:0.000}",dN1) + " N";
            lbN2.Text = string.Format("{0:0.000}",dN2) + " N";
            lbN3.Text = string.Format("{0:0.000}",dN3) + " N";
            lbN4.Text = string.Format("{0:0.000}",dN4) + " N";
            lbN5.Text = string.Format("{0:0.000}",dN5) + " N";
            
            if(OM.DevInfo.iL_Mode == 0) { lbMode1.Text = "변위모드";  lbMode3.Text = "변위모드"; }
            if(OM.DevInfo.iL_Mode == 1) { lbMode1.Text = "하중모드";  lbMode3.Text = "변위모드"; }
            if(OM.DevInfo.iL_Mode == 2) { lbMode1.Text = "파괴모드";  lbMode3.Text = "변위모드"; }
            if(OM.DevInfo.iL_Mode == 3) { lbMode1.Text = "GRIP 변위모드";  lbMode3.Text = "GRIP 변위모드"; }
        
            if(OM.DevInfo.iR_Mode == 0) { lbMode2.Text = "BITING 변위모드" ; lbMode4.Text = "BITING 변위모드" ; }
            if(OM.DevInfo.iR_Mode == 1) { lbMode2.Text = "BITING 하중모드" ; lbMode4.Text = "BITING 하중모드" ; }
            if(OM.DevInfo.iR_Mode == 2) { lbMode2.Text = "PULLING 변위모드"; lbMode4.Text = "PULLING 변위모드"; }
            //if(OM.DevInfo.iR_Mode == 3) { lbMode2.Text = "GRIP 변위모드";    lbMode4.Text = "GRIP 변위모드";    }
            //Refresh();

            if(ML.ER_IsErr())
            {
                btReset.ForeColor = SEQ._bFlick ? Color.Gold : Color.Red ;
            }
            else
            {
                btReset.ForeColor = Color.Gold ;
            }
            

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
            lvLotInfo.Columns.Add("", 90, HorizontalAlignment.Left);     //1280*1024는 109, 1024*768은 78
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
                    for (mi i = 0; i < mi.MAX_MOTR; i++)
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
            timer1.Enabled = false;
            DM.SaveMap();

            FrmCam.Close();
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
            DM.ARAY[ri.ARAY].SetStat(cs.None);

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



        private void btManual1_Click_1(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);

            if(iBtnTag >= (int)mc.LEFT_Home && iBtnTag < (int)mc.LEFT_Rest)
            {
                if (Log.ShowMessageModal("Confirm", "Do you want to manual work? \n (좌측에 자재가 그립되어 있으며 제거후 YES 버튼을 클릭해 주세요)") != DialogResult.Yes) return;
            }
            

            MM.SetManCycle((mc)iBtnTag);
        }

        private void FormOperation_Shown(object sender, EventArgs e)
        {
            tmUpdate.Enabled = true;
            timer1.Enabled = true;
        }

        private void btLotOpen_Click_2(object sender, EventArgs e)
        {
            FrmLotOpen = new FormLotOpen();
            FrmLotOpen.Show();
        }

        private void btLotEnd_Click_2(object sender, EventArgs e)
        {
            if (Log.ShowMessageModal("Confirm", "Do you want to Clear Lot?") != DialogResult.Yes) return;
            LOT.LotEnd();
            DM.ARAY[ri.ARAY].SetStat(cs.None);

            btStart.Enabled = false;

        }
       
        private void FormOperation_VisibleChanged(object sender, EventArgs e)
        {
            if(this.Visible) {
                tmUpdate.Enabled = true;
                timer1.Enabled = true;
                UpdateDevInfo(true);
            }

        }

        public void MakeDoubleBuffered(Control control, bool setting)
        {
            Type controlType = control.GetType();
            PropertyInfo pi = controlType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(control, setting, null);
        }

        private void btLightOnOff_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            if (!OM.MstOptn.bDebugMode)
            {
                if (Log.ShowMessageModal("Confirm", "Do you want to All Homming? \n (좌측에 자재가 그립되어 있으며 제거후 YES 버튼을 클릭해 주세요)") != DialogResult.Yes) return;
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
                    for (mi i = 0; i < mi.MAX_MOTR; i++)
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

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void btSaveDevice_Click(object sender, EventArgs e)
        {
            btSaveDevice.Enabled = false;
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            if (Log.ShowMessageModal("Confirm", "Are you Sure?") != DialogResult.Yes) return;
            PM.SetValue(mi.L_UP_ZLift, pv.L_UP_ZLiftTchA, 0);
            PM.SetValue(mi.L_DN_ZLift, pv.L_DN_ZLiftTchA, 0);
            PM.SetValue(mi.R_UP_ZLift, pv.R_UP_ZLiftTchA, 0);

            PM.UpdatePstn(false);
            PM.Save(OM.GetCrntDev());
            PM.UpdatePstn(true);

            UpdateDevInfo(false);

            OM.SaveDevInfo(OM.GetCrntDev().ToString());
            OM.SaveEqpOptn();

            //SaveMask(OM.GetCrntDev());

            //DM.ARAY[ri.ARAY].SetMaxColRow(1, 1);

            //Refresh();
            btSaveDevice.Enabled = true;
        }


        public void UpdateDevInfo(bool bToTable , int iMode = 0)
        {
            if (bToTable)
            {
                CConfig.ValToCon(tbL_10   , ref OM.DevInfo.dL_H_Weight      );
                CConfig.ValToCon(tbL_11   , ref OM.DevInfo.dL_H_Height      );
                CConfig.ValToCon(tbL_12   , ref OM.DevInfo.dL_H_Acc         );
                CConfig.ValToCon(tbL_13   , ref OM.DevInfo.dL_H_Vel         );
                CConfig.ValToCon(tbL_14   , ref OM.DevInfo.dL_H_Dcc         );
                CConfig.ValToCon(tbL_15   , ref OM.DevInfo.iL_H_Time        );
                CConfig.ValToCon(tbL_16   , ref OM.DevInfo.iL_H_Count       );
                CConfig.ValToCon(tbL_17   , ref OM.DevInfo.dL_H_Over        );
                CConfig.ValToCon(tbL_18   , ref OM.DevInfo.iL_H_Wait        );
                CConfig.ValToCon(tbL_19   , ref OM.DevInfo.dL_H_ZeroOfs1    );
                CConfig.ValToCon(tbL_1A   , ref OM.DevInfo.dL_H_ZeroOfs2    );

                CConfig.ValToCon(tbL_21   , ref OM.DevInfo.dL_W_Weight      );
                CConfig.ValToCon(tbL_22   , ref OM.DevInfo.dL_W_Acc         );
                CConfig.ValToCon(tbL_23   , ref OM.DevInfo.dL_W_Vel         );
                CConfig.ValToCon(tbL_24   , ref OM.DevInfo.dL_W_Dcc         );
                CConfig.ValToCon(tbL_25   , ref OM.DevInfo.iL_W_Time        );
                CConfig.ValToCon(tbL_26   , ref OM.DevInfo.iL_W_Count       );
                CConfig.ValToCon(tbL_27   , ref OM.DevInfo.dL_W_Over        );
                CConfig.ValToCon(tbL_28   , ref OM.DevInfo.iL_W_Wait        );
                                            
                CConfig.ValToCon(tbL_31   , ref OM.DevInfo.dL_D_Weight      );
                CConfig.ValToCon(tbL_32   , ref OM.DevInfo.dL_D_Height      );
                CConfig.ValToCon(tbL_33   , ref OM.DevInfo.iL_D_Time        );
                CConfig.ValToCon(tbL_34   , ref OM.DevInfo.dL_D_Over        );                                    

                CConfig.ValToCon(tbL_40   , ref OM.DevInfo.dL_G_Height1     );
                CConfig.ValToCon(tbL_41   , ref OM.DevInfo.dL_G_Height2     );
                CConfig.ValToCon(tbL_42   , ref OM.DevInfo.dL_G_Acc         );
                CConfig.ValToCon(tbL_43   , ref OM.DevInfo.dL_G_Vel         );
                CConfig.ValToCon(tbL_44   , ref OM.DevInfo.dL_G_Dcc         );
                CConfig.ValToCon(tbL_45   , ref OM.DevInfo.iL_G_Time        );
                CConfig.ValToCon(tbL_46   , ref OM.DevInfo.iL_G_Count       );
                CConfig.ValToCon(tbL_47   , ref OM.DevInfo.dL_G_Over        );
                CConfig.ValToCon(tbL_48   , ref OM.DevInfo.iL_G_Wait1       );
                CConfig.ValToCon(tbL_49   , ref OM.DevInfo.iL_G_Wait2       );

                CConfig.ValToCon(tbR_10   , ref OM.DevInfo.dR_H_Weight      );
                CConfig.ValToCon(tbR_11   , ref OM.DevInfo.dR_H_Height      );
                CConfig.ValToCon(tbR_12   , ref OM.DevInfo.dR_H_Acc         );
                CConfig.ValToCon(tbR_13   , ref OM.DevInfo.dR_H_Vel         );
                CConfig.ValToCon(tbR_14   , ref OM.DevInfo.dR_H_Dcc         );
                CConfig.ValToCon(tbR_15   , ref OM.DevInfo.iR_H_Time        );
                CConfig.ValToCon(tbR_16   , ref OM.DevInfo.iR_H_Count       );
                CConfig.ValToCon(tbR_17   , ref OM.DevInfo.dR_H_Over        );
                CConfig.ValToCon(tbR_18   , ref OM.DevInfo.iR_H_Manual      );
                CConfig.ValToCon(tbR_19   , ref OM.DevInfo.iR_H_Wait        );
                CConfig.ValToCon(tbR_1A   , ref OM.DevInfo.dR_H_ZeroOfs1    );
                                   
                CConfig.ValToCon(tbR_21   , ref OM.DevInfo.dR_W_Weight      );
                CConfig.ValToCon(tbR_22   , ref OM.DevInfo.dR_W_Acc         );
                CConfig.ValToCon(tbR_23   , ref OM.DevInfo.dR_W_Vel         );
                CConfig.ValToCon(tbR_24   , ref OM.DevInfo.dR_W_Dcc         );
                CConfig.ValToCon(tbR_25   , ref OM.DevInfo.iR_W_Time        );
                CConfig.ValToCon(tbR_26   , ref OM.DevInfo.iR_W_Count       );
                CConfig.ValToCon(tbR_27   , ref OM.DevInfo.dR_W_Over        );
                CConfig.ValToCon(tbR_28   , ref OM.DevInfo.iR_W_Manual      );
                CConfig.ValToCon(tbR_29   , ref OM.DevInfo.iR_W_Wait        );
                                                              
                CConfig.ValToCon(tbR_31   , ref OM.DevInfo.dR_P_Height      );
                CConfig.ValToCon(tbR_32   , ref OM.DevInfo.dR_P_Acc         );
                CConfig.ValToCon(tbR_33   , ref OM.DevInfo.dR_P_Vel         );
                CConfig.ValToCon(tbR_34   , ref OM.DevInfo.dR_P_Dcc         );
                CConfig.ValToCon(tbR_35   , ref OM.DevInfo.iR_P_Time        );
                CConfig.ValToCon(tbR_36   , ref OM.DevInfo.iR_P_Count       );
                CConfig.ValToCon(tbR_37   , ref OM.DevInfo.dR_P_Over        );
                CConfig.ValToCon(tbR_38   , ref OM.DevInfo.iR_P_Wait        );

                //CConfig.ValToCon(tbR_41   , ref OM.DevInfo.dR_G_Height      );
                //CConfig.ValToCon(tbR_42   , ref OM.DevInfo.dR_G_Acc         );
                //CConfig.ValToCon(tbR_43   , ref OM.DevInfo.dR_G_Vel         );
                //CConfig.ValToCon(tbR_44   , ref OM.DevInfo.dR_G_Dcc         );
                //CConfig.ValToCon(tbR_45   , ref OM.DevInfo.iR_G_Time        );
                //CConfig.ValToCon(tbR_46   , ref OM.DevInfo.iR_G_Count       );
                //CConfig.ValToCon(tbR_47   , ref OM.DevInfo.dR_G_Over        );
                //CConfig.ValToCon(tbR_48   , ref OM.DevInfo.iR_G_Wait        );

                CConfig.ValToCon(cbL1     , ref OM.DevInfo.iL_Mode          );
                CConfig.ValToCon(cbL2     , ref OM.DevInfo.iL_Motr          );
                CConfig.ValToCon(cbL3     , ref OM.DevInfo.iR_Mode          );

                CConfig.ValToCon(tbL_01   , ref OM.DevInfo.sL_Name          );
                CConfig.ValToCon(tbL_02   , ref OM.DevInfo.iL_UsbCnt        );
                CConfig.ValToCon(tbL_03   , ref OM.DevInfo.sR_Name          );
                CConfig.ValToCon(tbL_04   , ref OM.DevInfo.iR_UsbCnt        );
                
            }
            else 
            {
                OM.CDevInfo DevInfo = OM.DevInfo;
                if(iMode == 0 || iMode == 1)
                { 
                CConfig.ConToVal(tbL_10   , ref OM.DevInfo.dL_H_Weight      ); SEQ.CheckValue(ref OM.DevInfo.dL_H_Weight      , 1, 25    );
                CConfig.ConToVal(tbL_11   , ref OM.DevInfo.dL_H_Height      ); SEQ.CheckValue(ref OM.DevInfo.dL_H_Height      , 0, 15    );
                CConfig.ConToVal(tbL_12   , ref OM.DevInfo.dL_H_Acc         ); SEQ.CheckValue(ref OM.DevInfo.dL_H_Acc         , 1, 2000  );
                CConfig.ConToVal(tbL_13   , ref OM.DevInfo.dL_H_Vel         ); SEQ.CheckValue(ref OM.DevInfo.dL_H_Vel         , 1, 150   );
                CConfig.ConToVal(tbL_14   , ref OM.DevInfo.dL_H_Dcc         ); SEQ.CheckValue(ref OM.DevInfo.dL_H_Dcc         , 1, 2000  );
                CConfig.ConToVal(tbL_15   , ref OM.DevInfo.iL_H_Time        ); SEQ.CheckValue(ref OM.DevInfo.iL_H_Time        , 0, 5000  );
                CConfig.ConToVal(tbL_16   , ref OM.DevInfo.iL_H_Count       ); SEQ.CheckValue(ref OM.DevInfo.iL_H_Count       , 1, 200000);
                CConfig.ConToVal(tbL_17   , ref OM.DevInfo.dL_H_Over        ); SEQ.CheckValue(ref OM.DevInfo.dL_H_Over        , 1, 25    );
                CConfig.ConToVal(tbL_18   , ref OM.DevInfo.iL_H_Wait        ); SEQ.CheckValue(ref OM.DevInfo.iL_H_Wait        , 0, 5000  );
                CConfig.ConToVal(tbL_19   , ref OM.DevInfo.dL_H_ZeroOfs1    ); SEQ.CheckValue(ref OM.DevInfo.dL_H_ZeroOfs1    , 0, 2     );
                CConfig.ConToVal(tbL_1A   , ref OM.DevInfo.dL_H_ZeroOfs2    ); SEQ.CheckValue(ref OM.DevInfo.dL_H_ZeroOfs2    , 0, 2     );

                CConfig.ConToVal(tbL_21   , ref OM.DevInfo.dL_W_Weight      ); SEQ.CheckValue(ref OM.DevInfo.dL_W_Weight      , 1, 25    );
                CConfig.ConToVal(tbL_22   , ref OM.DevInfo.dL_W_Acc         ); SEQ.CheckValue(ref OM.DevInfo.dL_W_Acc         , 1, 2000  );
                CConfig.ConToVal(tbL_23   , ref OM.DevInfo.dL_W_Vel         ); SEQ.CheckValue(ref OM.DevInfo.dL_W_Vel         , 1, 150   );
                CConfig.ConToVal(tbL_24   , ref OM.DevInfo.dL_W_Dcc         ); SEQ.CheckValue(ref OM.DevInfo.dL_W_Dcc         , 1, 2000  );
                CConfig.ConToVal(tbL_25   , ref OM.DevInfo.iL_W_Time        ); SEQ.CheckValue(ref OM.DevInfo.iL_W_Time        , 0, 5000  );
                CConfig.ConToVal(tbL_26   , ref OM.DevInfo.iL_W_Count       ); SEQ.CheckValue(ref OM.DevInfo.iL_W_Count       , 1, 200000);
                CConfig.ConToVal(tbL_27   , ref OM.DevInfo.dL_W_Over        ); SEQ.CheckValue(ref OM.DevInfo.dL_W_Over        , 1, 25);
                CConfig.ConToVal(tbL_28   , ref OM.DevInfo.iL_W_Wait        ); SEQ.CheckValue(ref OM.DevInfo.iL_W_Wait        , 0, 5000  );

                CConfig.ConToVal(tbL_31   , ref OM.DevInfo.dL_D_Weight      ); SEQ.CheckValue(ref OM.DevInfo.dL_D_Weight      , 1, 25);
                CConfig.ConToVal(tbL_32   , ref OM.DevInfo.dL_D_Height      ); SEQ.CheckValue(ref OM.DevInfo.dL_D_Height      , 0.01, 0.1);
                CConfig.ConToVal(tbL_33   , ref OM.DevInfo.iL_D_Time        ); SEQ.CheckValue(ref OM.DevInfo.iL_D_Time        , 0, 5000);
                CConfig.ConToVal(tbL_34   , ref OM.DevInfo.dL_D_Over        ); SEQ.CheckValue(ref OM.DevInfo.dL_D_Over        , 0, 25);

                CConfig.ConToVal(tbL_40   , ref OM.DevInfo.dL_G_Height1     ); SEQ.CheckValue(ref OM.DevInfo.dL_G_Height1     , 0, 10    );
                CConfig.ConToVal(tbL_41   , ref OM.DevInfo.dL_G_Height2     ); SEQ.CheckValue(ref OM.DevInfo.dL_G_Height2     , 0, 10    );
                CConfig.ConToVal(tbL_42   , ref OM.DevInfo.dL_G_Acc         ); SEQ.CheckValue(ref OM.DevInfo.dL_G_Acc         , 1, 2000  );
                CConfig.ConToVal(tbL_43   , ref OM.DevInfo.dL_G_Vel         ); SEQ.CheckValue(ref OM.DevInfo.dL_G_Vel         , 1, 150   );
                CConfig.ConToVal(tbL_44   , ref OM.DevInfo.dL_G_Dcc         ); SEQ.CheckValue(ref OM.DevInfo.dL_G_Dcc         , 1, 2000  );
                CConfig.ConToVal(tbL_45   , ref OM.DevInfo.iL_G_Time        ); SEQ.CheckValue(ref OM.DevInfo.iL_G_Time        , 0, 5000  );
                CConfig.ConToVal(tbL_46   , ref OM.DevInfo.iL_G_Count       ); SEQ.CheckValue(ref OM.DevInfo.iL_G_Count       , 1, 200000);
                CConfig.ConToVal(tbL_47   , ref OM.DevInfo.dL_G_Over        ); SEQ.CheckValue(ref OM.DevInfo.dL_G_Over        , 1, 25    );
                CConfig.ConToVal(tbL_48   , ref OM.DevInfo.iL_G_Wait1       ); SEQ.CheckValue(ref OM.DevInfo.iL_G_Wait1       , 0, 5000  );
                CConfig.ConToVal(tbL_49   , ref OM.DevInfo.iL_G_Wait2       ); SEQ.CheckValue(ref OM.DevInfo.iL_G_Wait2       , 0, 5000  );

                CConfig.ConToVal(cbL1     , ref OM.DevInfo.iL_Mode          );
                CConfig.ConToVal(cbL2     , ref OM.DevInfo.iL_Motr          );
                CConfig.ConToVal(tbL_01   , ref OM.DevInfo.sL_Name          );
                CConfig.ConToVal(tbL_02   , ref OM.DevInfo.iL_UsbCnt        );
                }
                if(iMode == 0 || iMode == 2)
                { 
                CConfig.ConToVal(tbR_10   , ref OM.DevInfo.dR_H_Weight      ); SEQ.CheckValue(ref OM.DevInfo.dR_H_Weight      , 1, 25    );
                CConfig.ConToVal(tbR_11   , ref OM.DevInfo.dR_H_Height      ); SEQ.CheckValue(ref OM.DevInfo.dR_H_Height      , 0, 15    );
                CConfig.ConToVal(tbR_12   , ref OM.DevInfo.dR_H_Acc         ); SEQ.CheckValue(ref OM.DevInfo.dR_H_Acc         , 1, 2000  );
                CConfig.ConToVal(tbR_13   , ref OM.DevInfo.dR_H_Vel         ); SEQ.CheckValue(ref OM.DevInfo.dR_H_Vel         , 1, 150   );
                CConfig.ConToVal(tbR_14   , ref OM.DevInfo.dR_H_Dcc         ); SEQ.CheckValue(ref OM.DevInfo.dR_H_Dcc         , 1, 2000  );
                CConfig.ConToVal(tbR_15   , ref OM.DevInfo.iR_H_Time        ); SEQ.CheckValue(ref OM.DevInfo.iR_H_Time        , 0, 5000  );
                CConfig.ConToVal(tbR_16   , ref OM.DevInfo.iR_H_Count       ); SEQ.CheckValue(ref OM.DevInfo.iR_H_Count       , 1, 200000);
                CConfig.ConToVal(tbR_17   , ref OM.DevInfo.dR_H_Over        ); SEQ.CheckValue(ref OM.DevInfo.dR_H_Over        , 1, 25    );
                CConfig.ConToVal(tbR_18   , ref OM.DevInfo.iR_H_Manual      );
                CConfig.ConToVal(tbR_19   , ref OM.DevInfo.iR_H_Wait        ); SEQ.CheckValue(ref OM.DevInfo.iR_H_Wait        , 0, 5000  );
                CConfig.ConToVal(tbR_1A   , ref OM.DevInfo.dR_H_ZeroOfs1    ); SEQ.CheckValue(ref OM.DevInfo.dR_H_ZeroOfs1    , 0, 2     );

                CConfig.ConToVal(tbR_21   , ref OM.DevInfo.dR_W_Weight      ); SEQ.CheckValue(ref OM.DevInfo.dR_W_Weight      , 1, 25    );
                CConfig.ConToVal(tbR_22   , ref OM.DevInfo.dR_W_Acc         ); SEQ.CheckValue(ref OM.DevInfo.dR_W_Acc         , 1, 2000  );
                CConfig.ConToVal(tbR_23   , ref OM.DevInfo.dR_W_Vel         ); SEQ.CheckValue(ref OM.DevInfo.dR_W_Vel         , 1, 150   );
                CConfig.ConToVal(tbR_24   , ref OM.DevInfo.dR_W_Dcc         ); SEQ.CheckValue(ref OM.DevInfo.dR_W_Dcc         , 1, 2000  );
                CConfig.ConToVal(tbR_25   , ref OM.DevInfo.iR_W_Time        ); SEQ.CheckValue(ref OM.DevInfo.iR_W_Time        , 0, 5000  );
                CConfig.ConToVal(tbR_26   , ref OM.DevInfo.iR_W_Count       ); SEQ.CheckValue(ref OM.DevInfo.iR_W_Count       , 1, 200000);
                CConfig.ConToVal(tbR_27   , ref OM.DevInfo.dR_W_Over        ); SEQ.CheckValue(ref OM.DevInfo.dR_W_Over        , 1, 25    );
                CConfig.ConToVal(tbR_28   , ref OM.DevInfo.iR_W_Manual      );
                CConfig.ConToVal(tbR_29   , ref OM.DevInfo.iR_W_Wait        ); SEQ.CheckValue(ref OM.DevInfo.iR_W_Wait        , 0, 5000  );

                CConfig.ConToVal(tbR_31   , ref OM.DevInfo.dR_P_Height      ); SEQ.CheckValue(ref OM.DevInfo.dR_P_Height      , 1, 40    );
                CConfig.ConToVal(tbR_32   , ref OM.DevInfo.dR_P_Acc         ); SEQ.CheckValue(ref OM.DevInfo.dR_P_Acc         , 1, 2000  );
                CConfig.ConToVal(tbR_33   , ref OM.DevInfo.dR_P_Vel         ); SEQ.CheckValue(ref OM.DevInfo.dR_P_Vel         , 1, 150   );
                CConfig.ConToVal(tbR_34   , ref OM.DevInfo.dR_P_Dcc         ); SEQ.CheckValue(ref OM.DevInfo.dR_P_Dcc         , 1, 2000  );
                CConfig.ConToVal(tbR_35   , ref OM.DevInfo.iR_P_Time        ); SEQ.CheckValue(ref OM.DevInfo.iR_P_Time        , 0, 5000  );
                CConfig.ConToVal(tbR_36   , ref OM.DevInfo.iR_P_Count       ); SEQ.CheckValue(ref OM.DevInfo.iR_P_Count       , 1, 200000);
                CConfig.ConToVal(tbR_37   , ref OM.DevInfo.dR_P_Over        ); SEQ.CheckValue(ref OM.DevInfo.dR_P_Over        , 1, 25    );
                CConfig.ConToVal(tbR_38   , ref OM.DevInfo.iR_P_Wait        ); SEQ.CheckValue(ref OM.DevInfo.iR_P_Wait        , 0, 5000  );

                CConfig.ConToVal(cbL3     , ref OM.DevInfo.iR_Mode          );
                CConfig.ConToVal(tbL_03   , ref OM.DevInfo.sR_Name          );
                CConfig.ConToVal(tbL_04   , ref OM.DevInfo.iR_UsbCnt        );

                }

                //Auto Log
                Type type = DevInfo.GetType();
                FieldInfo[] f = type.GetFields(BindingFlags.Public|BindingFlags.NonPublic | BindingFlags.Instance);
                for(int i = 0 ; i < f.Length ; i++){
                    Trace(f[i].Name, f[i].GetValue(DevInfo).ToString(), f[i].GetValue(OM.DevInfo).ToString());
                }
                //CConfig.ConToVal(tbR_41   , ref OM.DevInfo.dR_G_Height      ); SEQ.CheckValue(ref OM.DevInfo.dR_G_Height      , 0, 2     );
                //CConfig.ConToVal(tbR_42   , ref OM.DevInfo.dR_G_Acc         ); SEQ.CheckValue(ref OM.DevInfo.dR_G_Acc         , 1, 2000  );
                //CConfig.ConToVal(tbR_43   , ref OM.DevInfo.dR_G_Vel         ); SEQ.CheckValue(ref OM.DevInfo.dR_G_Vel         , 1, 150   );
                //CConfig.ConToVal(tbR_44   , ref OM.DevInfo.dR_G_Dcc         ); SEQ.CheckValue(ref OM.DevInfo.dR_G_Dcc         , 1, 2000  );
                //CConfig.ConToVal(tbR_45   , ref OM.DevInfo.iR_G_Time        ); SEQ.CheckValue(ref OM.DevInfo.iR_G_Time        , 0, 5000  );
                //CConfig.ConToVal(tbR_46   , ref OM.DevInfo.iR_G_Count       ); SEQ.CheckValue(ref OM.DevInfo.iR_G_Count       , 1, 200000);
                //CConfig.ConToVal(tbR_47   , ref OM.DevInfo.dR_G_Over        ); SEQ.CheckValue(ref OM.DevInfo.dR_G_Over        , 1, 25    );
                //CConfig.ConToVal(tbR_48   , ref OM.DevInfo.iR_G_Wait        ); SEQ.CheckValue(ref OM.DevInfo.iR_G_Wait        , 0, 5000  );




                
                //Trace("접촉한 뒤에 하중 (1 ~ 25 kg)            ", DevInfo.dL_H_Weight, OM.DevInfo.dL_H_Weight);
                //Trace("접촉한 뒤에 내려갈 높이 (0 ~ 2)         ", DevInfo.dL_H_Height, OM.DevInfo.dL_H_Height);
                //Trace("가속도 (초당 이동거리 1~2000)           ", DevInfo.dL_H_Acc   , OM.DevInfo.dL_H_Acc   );
                //Trace("속도 (1 ~ 150)                          ", DevInfo.dL_H_Vel   , OM.DevInfo.dL_H_Vel   );
                //Trace("감속도 (초당 이동거리 1~2000)           ", DevInfo.dL_H_Dcc   , OM.DevInfo.dL_H_Dcc   );
                //Trace("유지시간 (0 ~ 5000)                     ", DevInfo.iL_H_Time  , OM.DevInfo.iL_H_Time  );
                //Trace("횟수 (1 ~ 200000)                       ", DevInfo.iL_H_Count , OM.DevInfo.iL_H_Count );
                //Trace("오버로드 (0 ~ 25)                       ", DevInfo.dL_H_Over  , OM.DevInfo.dL_H_Over  );
                //Trace("대기시간 (0 ~ 5000)                     ", DevInfo.iL_H_Wait  , OM.DevInfo.iL_H_Wait  );
                //Trace("대기시간 (0 ~ 5000)                     ", DevInfo.iL_H_Wait  , OM.DevInfo.iL_H_Wait  );
                //Trace("대기시간 (0 ~ 5000)                     ", DevInfo.iL_H_Wait  , OM.DevInfo.iL_H_Wait  );
                
                //Trace("접촉한 뒤에 하중 (1 ~ 25 kg)            ", DevInfo.dL_W_Weight, OM.DevInfo.dL_W_Weight);
                //Trace("가속도 (초당 이동거리 1~2000)           ", DevInfo.dL_W_Acc   , OM.DevInfo.dL_W_Acc   );
                //Trace("속도 (1 ~ 150)                          ", DevInfo.dL_W_Vel   , OM.DevInfo.dL_W_Vel   );
                //Trace("감속도 (초당 이동거리 1~2000)           ", DevInfo.dL_W_Dcc   , OM.DevInfo.dL_W_Dcc   );
                //Trace("유지시간 (0 ~ 5000)                     ", DevInfo.iL_W_Time  , OM.DevInfo.iL_W_Time  );
                //Trace("횟수 (1 ~ 200000)                       ", DevInfo.iL_W_Count , OM.DevInfo.iL_W_Count );
                //Trace("오버로드 (0 ~ 25)                       ", DevInfo.dL_W_Over  , OM.DevInfo.dL_W_Over  );
                //Trace("대기시간 (0 ~ 5000)                     ", DevInfo.iL_W_Wait  , OM.DevInfo.iL_W_Wait  );

                //Trace("최대 하중 (1~25 kg)                     ", DevInfo.dL_D_Weight, OM.DevInfo.dL_D_Weight);
                //Trace("단계별 높이 변화값 (0.01~0.1)           ", DevInfo.dL_D_Height, OM.DevInfo.dL_D_Height);
                //Trace("단계별 높이 변화값 (0.01~0.1)           ", DevInfo.dL_D_Height, OM.DevInfo.dL_D_Height);
                //Trace("유지시간 (0 ~ 5000)                     ", DevInfo.iL_D_Time  , OM.DevInfo.iL_D_Time  );
                //Trace("오버로드 (0 ~ 25)                       ", DevInfo.dL_D_Over  , OM.DevInfo.dL_D_Over  ); 
                
                //Trace("접촉한 뒤에 하중 (1 ~ 25 kg)            ", DevInfo.dR_H_Weight, OM.DevInfo.dR_H_Weight);
                //Trace("접촉한 뒤에 내려갈 높이 (0 ~ 2)         ", DevInfo.dR_H_Height, OM.DevInfo.dR_H_Height);
                //Trace("가속도 (초당 이동거리 1~2000)           ", DevInfo.dR_H_Acc   , OM.DevInfo.dR_H_Acc   );
                //Trace("속도 (1 ~ 150)                          ", DevInfo.dR_H_Vel   , OM.DevInfo.dR_H_Vel   );
                //Trace("감속도 (초당 이동거리 1~2000)           ", DevInfo.dR_H_Dcc   , OM.DevInfo.dR_H_Dcc   );
                //Trace("유지시간 (0 ~ 5000)                     ", DevInfo.iR_H_Time  , OM.DevInfo.iR_H_Time  );
                //Trace("횟수 (1 ~ 200000)                       ", DevInfo.iR_H_Count , OM.DevInfo.iR_H_Count );
                //Trace("오버로드 (0 ~ 25)                       ", DevInfo.dR_H_Over  , OM.DevInfo.dR_H_Over  );
                //Trace("운영모드 (0 - 오토, 1 - 메뉴얼)         ", DevInfo.iR_H_Manual, OM.DevInfo.iR_H_Manual);
                //Trace("대기시간 (0 ~ 5000)                     ", DevInfo.iR_H_Wait  , OM.DevInfo.iR_H_Wait  );

                //Trace("접촉한 뒤에 하중 (1 ~ 25 kg)            ", DevInfo.dR_W_Weight, OM.DevInfo.dR_W_Weight);
                //Trace("가속도 (초당 이동거리 1~2000)           ", DevInfo.dR_W_Acc   , OM.DevInfo.dR_W_Acc   );
                //Trace("속도 (1 ~ 150)                          ", DevInfo.dR_W_Vel   , OM.DevInfo.dR_W_Vel   );
                //Trace("감속도 (초당 이동거리 1~2000)           ", DevInfo.dR_W_Dcc   , OM.DevInfo.dR_W_Dcc   );
                //Trace("유지시간 (0 ~ 5000)                     ", DevInfo.iR_W_Time  , OM.DevInfo.iR_W_Time  );
                //Trace("횟수 (1 ~ 200000)                       ", DevInfo.iR_W_Count , OM.DevInfo.iR_W_Count );
                //Trace("오버로드 (0 ~ 25)                       ", DevInfo.dR_W_Over  , OM.DevInfo.dR_W_Over  );
                //Trace("운영모드 (0 - 오토, 1 - 메뉴얼)         ", DevInfo.iR_W_Manual, OM.DevInfo.iR_W_Manual);
                //Trace("대기시간 (0 ~ 5000)                     ", DevInfo.iR_W_Wait  , OM.DevInfo.iR_W_Wait  );
                    
                //Trace("이동할 높이 (0 ~ 40)                    ", DevInfo.dR_P_Height, OM.DevInfo.dR_P_Height);
                //Trace("가속도 (초당 이동거리 1~2000)           ", DevInfo.dR_P_Acc   , OM.DevInfo.dR_P_Acc   );
                //Trace("속도 (1 ~ 150)                          ", DevInfo.dR_P_Vel   , OM.DevInfo.dR_P_Vel   );
                //Trace("감속도 (초당 이동거리 1~2000)           ", DevInfo.dR_P_Dcc   , OM.DevInfo.dR_P_Dcc   );
                //Trace("유지시간 (0 ~ 5000)                     ", DevInfo.iR_P_Time  , OM.DevInfo.iR_P_Time  );
                //Trace("횟수 (1 ~ 200000)                       ", DevInfo.iR_P_Count , OM.DevInfo.iR_P_Count );
                //Trace("오버로드 (0 ~ 25)                       ", DevInfo.dR_P_Over  , OM.DevInfo.dR_P_Over  );
                                   
                //Trace("이동할 높이 (0 ~ 2)                     ", DevInfo.dL_G_Height, OM.DevInfo.dL_G_Height);
                //Trace("가속도 (초당 이동거리 1~2000)           ", DevInfo.dL_G_Acc   , OM.DevInfo.dL_G_Acc   );
                //Trace("속도 (1 ~ 150)                          ", DevInfo.dL_G_Vel   , OM.DevInfo.dL_G_Vel   );
                //Trace("감속도 (초당 이동거리 1~2000)           ", DevInfo.dL_G_Dcc   , OM.DevInfo.dL_G_Dcc   );
                //Trace("유지시간 (0 ~ 5000)                     ", DevInfo.iL_G_Time  , OM.DevInfo.iL_G_Time  );
                //Trace("횟수 (1 ~ 200000)                       ", DevInfo.iL_G_Count , OM.DevInfo.iL_G_Count );
                //Trace("오버로드 (0 ~ 25)                       ", DevInfo.dL_G_Over  , OM.DevInfo.dL_G_Over  );
                //Trace("대기시간 (0 ~ 5000)                     ", DevInfo.iL_G_Wait  , OM.DevInfo.iL_G_Wait  );
                                                                                                        
                //Trace("사용할 모드 선택(0-변위모드.1-하중모드.2-파괴모드)                               ", DevInfo.iL_Mode    , OM.DevInfo.iL_Mode    );
                //Trace("사용할 모터 선택(0-위아래 모터 모두 사용.1-위쪽 모터만 사용.2-아래쪽 모터만 사용)", DevInfo.iL_Motr    , OM.DevInfo.iL_Motr    );
                
                //Trace("사용할 모드 선택(0-변위모드.1-하중모드.2-PULLING변위모드)                        ", DevInfo.iR_Mode    , OM.DevInfo.iR_Mode    );
                                                                                                           
                //Trace("장치설명(LEFT)                              ", DevInfo.sL_Name    , OM.DevInfo.sL_Name    );
                //Trace("장치갯수(LEFT)                              ", DevInfo.iL_UsbCnt  , OM.DevInfo.iL_UsbCnt  );
                //Trace("장치설명(RIGHT)                             ", DevInfo.sR_Name    , OM.DevInfo.sR_Name    );
                //Trace("장치갯수(RIGHT)                             ", DevInfo.iR_UsbCnt  , OM.DevInfo.iR_UsbCnt  );

                UpdateDevInfo(true);
            }
        
        }

        private void lbOptn1_Click(object sender, EventArgs e)
        {
            if (Log.ShowMessageModal("Confirm", "Are you Sure?") != DialogResult.Yes) return;
            OM.CmnOptn.bUse_L_Part = !OM.CmnOptn.bUse_L_Part ; 
        }

        private void lbOptn2_Click(object sender, EventArgs e)
        {
            if (Log.ShowMessageModal("Confirm", "Are you Sure?") != DialogResult.Yes) return;
            OM.CmnOptn.bUse_R_Part = !OM.CmnOptn.bUse_R_Part ; 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SEQ.LEFT.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SEQ.LEFT.Stop();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //SEQ.LEFT.Rec(@"d:\Temp\asdf.avi");
            //FrmCam.Rec(@"d:\Temp\asdf.avi", pnCam.Width, pnCam.Height);
            Rec();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //SEQ.LEFT.Rec(@"");
            SEQ.MCR.CycleInit(1,"1");
            SEQ.MCR.Cycle(1);
        }

        private void btLight_Click(object sender, EventArgs e)
        {
            ML.IO_SetY(yi.ETC_LightOnOff,!ML.IO_GetY(yi.ETC_LightOnOff));
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            //label122.Text = FrmCam.m_dMainThreadCycleTime.ToString();
            //Display
            lbDevice.Text = OM.GetCrntDev().ToString();
            lbLotNo.Text = LOT.GetLotNo();
            lbUsbCnt.Text = Eqp.iUsbCnt.ToString();

            double dNow1 = SEQ.AIO_GetX(ax.ETC_LoadCell1);
            double dNow2 = SEQ.AIO_GetX(ax.ETC_LoadCell2);
            double dNow3 = SEQ.AIO_GetX(ax.ETC_LoadCell3);

            //if (dNow1 < dLoad1) lbl3.Text = string.Format("{0:0.00}", dLoad1);
            //else                lbl3.Text = string.Format("{0:0.00}", dNow1 );
                                                                            
            //if (dNow2 < dLoad2) lbl4.Text = string.Format("{0:0.00}", dLoad2);
            //else                lbl4.Text = string.Format("{0:0.00}", dNow2 );
                                                                            
            //if (dNow3 < dLoad3) lbr3.Text = string.Format("{0:0.00}", dLoad3);
            //else                lbr3.Text = string.Format("{0:0.00}", dNow3 );

            //lbl3.Text = string.Format("{0:0.00}", dLoad1);
            lbl3.Text = string.Format("{0:0.00}", dNow1);

            //lbl4.Text = string.Format("{0:0.00}", dLoad2);
            lbl4.Text = string.Format("{0:0.00}", dNow2);

            //lbr3.Text = string.Format("{0:0.00}", dLoad3);
            lbr3.Text = string.Format("{0:0.00}", dNow3);

            //lbl3.Text = dNow1.ToString();
            //lbl4.Text = dNow2.ToString();
            //lbr3.Text = dNow3.ToString();

            dLoad1 = dNow1;
            dLoad2 = dNow2;
            dLoad3 = dNow3;

            if (!this.Visible)
            {
                timer1.Enabled = false;
                return;
            }
            timer1.Enabled = true;
        }

        private void Trace(string sText, string _s1, string _s2)
        {
            Log.Trace(sText.Trim() + " : " + _s1 + " -> " + _s2, ti.Dev);
        }
        private void Trace(string sText, int _s1, int _s2)
        {
            Log.Trace(sText.Trim() + " : " + _s1.ToString() + " -> " + _s2.ToString(), ti.Dev);
        }
        private void Trace(string sText, double _s1, double _s2)
        {
            Log.Trace(sText.Trim() + " : " + _s1.ToString() + " -> " + _s2.ToString(), ti.Dev);
        }
        private void Trace(string sText, bool _s1, bool _s2)
        {
            Log.Trace(sText.Trim() + " : " + _s1.ToString() + " -> " + _s2.ToString(), ti.Dev);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ML.IO_SetY(yi.ETC_LoadZero1, true);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            ML.IO_SetY(yi.ETC_LoadZero2, true);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            ML.IO_SetY(yi.ETC_LoadZero3, true);
        }

        private void groupBox8_Enter(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (Log.ShowMessageModal("Confirm", "Do you want to manual work? \n (우측에 자재가 그립되어 있으며 제거후 YES 버튼을 클릭해 주세요)") != DialogResult.Yes) return;
            //SEQ.RIGH.MoveMotr(mi.R_UP_ZLift,pv.R_UP_ZLiftWaitG);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //if (Log.ShowMessageModal("Confirm", "Do you want to manual work? \n (우측에 자재가 그립되어 있으며 제거후 YES 버튼을 클릭해 주세요)") != DialogResult.Yes) return;
            SEQ.RIGH.MoveMotr(mi.R_UP_ZLift,pv.R_UP_ZLiftWaitP);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (Log.ShowMessageModal("Confirm", "Do you want to manual work? \n (좌측에 자재가 그립되어 있으며 제거후 YES 버튼을 클릭해 주세요)") != DialogResult.Yes) return;
            SEQ.LEFT.MoveMotr(mi.L_UP_ZLift,pv.L_UP_ZLiftWaitG);
        }

        private void btSaveDevice1_Click(object sender, EventArgs e)
        {
            btSaveDevice.Enabled = false;
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            if (Log.ShowMessageModal("Confirm", "Are you Sure?") != DialogResult.Yes) return;

            if(SEQ._bRun) UpdateDevInfo(false,2);
            else { 
                PM.SetValue(mi.L_UP_ZLift, pv.L_UP_ZLiftTchA, 0);
                PM.SetValue(mi.L_DN_ZLift, pv.L_DN_ZLiftTchA, 0);
                PM.SetValue(mi.R_UP_ZLift, pv.R_UP_ZLiftTchA, 0);

                PM.UpdatePstn(false);
                PM.Save(OM.GetCrntDev());
                PM.UpdatePstn(true);

                UpdateDevInfo(false);

                OM.SaveDevInfo(OM.GetCrntDev().ToString());
                OM.SaveEqpOptn();
            }
            //SaveMask(OM.GetCrntDev());

            //DM.ARAY[ri.ARAY].SetMaxColRow(1, 1);

            //Refresh();
            btSaveDevice.Enabled = true;

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