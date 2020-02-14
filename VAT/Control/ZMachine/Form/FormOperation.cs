using System;
using System.Drawing;
using System.Windows.Forms;
using COMMON;
using SML;
using System.Reflection;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;
using System.Text;

namespace Machine
{
    public partial class FormOperation : Form
    {
        public static FormOperation FrmOperation;
        public static FormLotOpen   FrmLotOpen  ;
        public static FormMain      FrmMain     ;
        public static FormCam_XNB   FrmCam_XNB  ;
        public static FormGrid      FrmGrid     ;

        //Graph
        public static FormGraph     FrmGraph1   ;
        public static FormGraph     FrmGraph2   ;
        public static FormGraph     FrmGraph3   ;
        public static FormGraph     FrmGraph4   ;
        public static FormGraph     FrmGraph5   ;
        public static FormGraph     FrmGraph6   ;

        public int LotInfoCnt = 6;
        public int DayInfoCnt = 6;        

        //Output 0~5
        FrameOutputAPT[] FraOutputAPT;
        //private string sFormText ;
        private const string sFormText = "Form Operation ";

        //Rs 
        public RS485_ConverTech.TStat[] Stat;
        public RS232_Daegyum_Seasoning.TStat StatD;

        public FormOperation(Panel _pnBase)
        {
            InitializeComponent();

            //sFormText = this.GetType().Name;

            this.TopLevel = false;
            this.Parent = _pnBase;

            //From Cam Setting
            if(!Eqp.bIgnrCam)
            {
                FrmCam_XNB = new FormCam_XNB();
                FrmCam_XNB.TopLevel = false;
                FrmCam_XNB.Parent   = pnCam;
                FrmCam_XNB.Dock     = DockStyle.Fill;
                FrmCam_XNB.Show();
            }

            //Form Grid Setting
            FrmGrid = new FormGrid();
            FrmGrid.TopLevel = false   ;
            FrmGrid.Parent   = pnRecipe;
            FrmGrid.Dock     = DockStyle.Fill;
            FrmGrid.Show();

            #region Form Graph Setting
            //Form Graph
            FrmGraph1 = new FormGraph(SeriesChartType.FastLine, "ANODE");
            FrmGraph1.TopLevel = false;
            FrmGraph1.Parent   = pnAnode1;
            FrmGraph1.Dock     = DockStyle.Fill;
            FrmGraph1.Show();

            FrmGraph2 = new FormGraph(SeriesChartType.FastLine, "CATHOD");
            FrmGraph2.TopLevel = false;
            FrmGraph2.Parent   = pnAnode2;
            FrmGraph2.Dock     = DockStyle.Fill;
            FrmGraph2.Show();

            FrmGraph3 = new FormGraph(SeriesChartType.FastLine, "GATE V");
            FrmGraph3.TopLevel = false;
            FrmGraph3.Parent   = pnGate1;
            FrmGraph3.Dock     = DockStyle.Fill;
            FrmGraph3.Show();

            FrmGraph4 = new FormGraph(SeriesChartType.FastLine, "GATE I");
            FrmGraph4.TopLevel = false;
            FrmGraph4.Parent   = pnGate2;
            FrmGraph4.Dock     = DockStyle.Fill;
            FrmGraph4.Show();

            FrmGraph5 = new FormGraph(SeriesChartType.FastLine, "FOCUS V");
            FrmGraph5.TopLevel = false;
            FrmGraph5.Parent   = pnFocus1;
            FrmGraph5.Dock     = DockStyle.Fill;
            FrmGraph5.Show();

            FrmGraph6 = new FormGraph(SeriesChartType.FastLine, "FOCUS I");
            FrmGraph6.TopLevel = false;
            FrmGraph6.Parent   = pnFocus2;
            FrmGraph6.Dock     = DockStyle.Fill;
            FrmGraph6.Show();

            SEQ.aging.CAddPoints1  += new Aging.Chart_AddPoints1 (FrmGraph1.Chart_AddPoints);
            SEQ.aging.CAddPoints2  += new Aging.Chart_AddPoints2 (FrmGraph2.Chart_AddPoints);
            SEQ.aging.CAddPoints3  += new Aging.Chart_AddPoints3 (FrmGraph3.Chart_AddPoints);
            SEQ.aging.CAddPoints4  += new Aging.Chart_AddPoints4 (FrmGraph4.Chart_AddPoints);
            SEQ.aging.CAddPoints5  += new Aging.Chart_AddPoints5 (FrmGraph5.Chart_AddPoints);
            SEQ.aging.CAddPoints6  += new Aging.Chart_AddPoints6 (FrmGraph6.Chart_AddPoints);

            SEQ.aging.CSave1       += new Aging.Chart_Save1      (FrmGraph1.Save           );
            SEQ.aging.CSave2       += new Aging.Chart_Save2      (FrmGraph2.Save           );
            SEQ.aging.CSave3       += new Aging.Chart_Save3      (FrmGraph3.Save           );
            SEQ.aging.CSave4       += new Aging.Chart_Save4      (FrmGraph4.Save           );
            SEQ.aging.CSave5       += new Aging.Chart_Save5      (FrmGraph5.Save           );
            SEQ.aging.CSave6       += new Aging.Chart_Save6      (FrmGraph6.Save           );

            SEQ.aging.CClear       += new Aging.Chart_Clear      (FrmGraph1.Chart_Clear    );
            SEQ.aging.CClear       += new Aging.Chart_Clear      (FrmGraph2.Chart_Clear    );
            SEQ.aging.CClear       += new Aging.Chart_Clear      (FrmGraph3.Chart_Clear    );
            SEQ.aging.CClear       += new Aging.Chart_Clear      (FrmGraph4.Chart_Clear    );
            SEQ.aging.CClear       += new Aging.Chart_Clear      (FrmGraph5.Chart_Clear    );
            SEQ.aging.CClear       += new Aging.Chart_Clear      (FrmGraph6.Chart_Clear    );

            SEQ.aging.CBegin       += new Aging.Chart_Begin      (FrmGraph1.Begin          );
            SEQ.aging.CBegin       += new Aging.Chart_Begin      (FrmGraph2.Begin          );
            SEQ.aging.CBegin       += new Aging.Chart_Begin      (FrmGraph3.Begin          );
            SEQ.aging.CBegin       += new Aging.Chart_Begin      (FrmGraph4.Begin          );
            SEQ.aging.CBegin       += new Aging.Chart_Begin      (FrmGraph5.Begin          );
            SEQ.aging.CBegin       += new Aging.Chart_Begin      (FrmGraph6.Begin          );

            SEQ.aging.CEnd         += new Aging.Chart_End        (FrmGraph1.End            );
            SEQ.aging.CEnd         += new Aging.Chart_End        (FrmGraph2.End            );
            SEQ.aging.CEnd         += new Aging.Chart_End        (FrmGraph3.End            );
            SEQ.aging.CEnd         += new Aging.Chart_End        (FrmGraph4.End            );
            SEQ.aging.CEnd         += new Aging.Chart_End        (FrmGraph5.End            );
            SEQ.aging.CEnd         += new Aging.Chart_End        (FrmGraph6.End            );
            #endregion

            //DispDayList();
            //DispLotInfo();

            InitLsv ();
            InitLsv1();

            var PropLotInfo = LsvDisp.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropLotInfo.SetValue(LsvDisp, true, null);

            var PropLotInfo1 = LsvDisp1.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropLotInfo.SetValue(LsvDisp1, true, null);

            FraOutputAPT = new FrameOutputAPT[6];
            FraOutputAPT[0] = new FrameOutputAPT(); FraOutputAPT[0].TopLevel = false;
            FraOutputAPT[0].SetConfig(yi.CathodeSWT, ML.IO_GetYName(yi.CathodeSWT), pnA1);
            FraOutputAPT[0].Show();

            FraOutputAPT[1] = new FrameOutputAPT(); FraOutputAPT[1].TopLevel = false;
            FraOutputAPT[1].SetConfig(yi.CathodeGND, ML.IO_GetYName(yi.CathodeGND), pnA2);
            FraOutputAPT[1].Show();

            FraOutputAPT[2] = new FrameOutputAPT(); FraOutputAPT[2].TopLevel = false;
            FraOutputAPT[2].SetConfig(yi.GatePower, ML.IO_GetYName(yi.GatePower), pnG1);
            FraOutputAPT[2].Show();

            FraOutputAPT[3] = new FrameOutputAPT(); FraOutputAPT[3].TopLevel = false;
            FraOutputAPT[3].SetConfig(yi.GateGND, ML.IO_GetYName(yi.GateGND), pnG2);
            FraOutputAPT[3].Show();

            FraOutputAPT[4] = new FrameOutputAPT(); FraOutputAPT[4].TopLevel = false;
            FraOutputAPT[4].SetConfig(yi.FocusPower, ML.IO_GetYName(yi.FocusPower), pnF1);
            FraOutputAPT[4].Show();

            FraOutputAPT[5] = new FrameOutputAPT(); FraOutputAPT[5].TopLevel = false;
            FraOutputAPT[5].SetConfig(yi.FocusGND, ML.IO_GetYName(yi.FocusGND), pnF2);
            FraOutputAPT[5].Show();

            //    FraOutputAPT[i] = new FrameOutputAPT();
            //    FraOutputAPT[i].TopLevel = false;
            //    FraOutputAPT[i].SetConfig((yi)iIOCtrl, ML.IO_GetYName((yi)iIOCtrl), Ctrl[0]);
            //    FraOutputAPT[i].Show();
            //
            //   // FraOutputAPT[i].Show();

            //MakeDoubleBuffered(pnULDR,true);
            //tmUpdate.Enabled = true;

            //DM.ARAY[ri.LODR].SetParent(pnLODR); DM.ARAY[ri.LODR].Name = "LODR";
            
            //Loader           
            //DM.ARAY[ri.LODR].SetDisp(cs.None      , "None"            ,Color.White        );
            //DM.ARAY[ri.LODR].SetDisp(cs.Work      , "Work"            ,Color.Yellow       );
            //DM.ARAY[ri.LODR].SetDisp(cs.Unknown   , "UnKnown"         ,Color.Aqua         );
            //DM.ARAY[ri.LODR].SetDisp(cs.Empty     , "Empty"           ,Color.Silver       ); 
                                                                                     
            //DM.LoadMap();


            //var path = new System.Drawing.Drawing2D.GraphicsPath();
            //path.AddEllipse(0, 0, pnTMRK.Width, pnTMRK.Height);
            //pnTMRK.Region = new Region(path);


            //전에 데이터 저장용
            Stat  = new RS485_ConverTech.TStat[RS485_ConverTech.MAX_ARRAY];
            StatD = new RS232_Daegyum_Seasoning.TStat();

        }

        private void InitLsv()
        {
            LsvDisp.Clear();
            LsvDisp.View = View.Details;
            LsvDisp.FullRowSelect = true;
            LsvDisp.GridLines = true;
            LsvDisp.MultiSelect = false;
            LsvDisp.Columns.Add("1" , lbD1.Width, HorizontalAlignment.Left); //
            LsvDisp.Columns.Add("2" , lbD2.Width, HorizontalAlignment.Left); //
            LsvDisp.Columns.Add("3" , lbD3.Width, HorizontalAlignment.Left); //
            LsvDisp.Columns.Add("4" , lbD4.Width, HorizontalAlignment.Left); //
            LsvDisp.Columns.Add("5" , lbD5.Width, HorizontalAlignment.Left); //
            LsvDisp.Columns.Add("6" , lbD6.Width, HorizontalAlignment.Left); //
            LsvDisp.Columns.Add("7" , lbD7.Width, HorizontalAlignment.Left); //
            LsvDisp.Columns.Add("8" , lbD8.Width, HorizontalAlignment.Left); //
            LsvDisp.Columns.Add("9" , lbD9.Width, HorizontalAlignment.Left); //
            LsvDisp.Columns.Add("10", lbDA.Width, HorizontalAlignment.Left); //
            LsvDisp.Columns.Add("11", lbDB.Width, HorizontalAlignment.Left); //
            LsvDisp.HeaderStyle = ColumnHeaderStyle.None;
        }

        private void InitLsv1()
        {
            LsvDisp1.Clear();
            LsvDisp1.View = View.Details;
            LsvDisp1.FullRowSelect = true;
            LsvDisp1.GridLines = true;
            LsvDisp1.MultiSelect = false;
            LsvDisp1.Columns.Add("1" , lbD1.Width, HorizontalAlignment.Left); //
            LsvDisp1.Columns.Add("2" , lbD2.Width, HorizontalAlignment.Left); //
            LsvDisp1.Columns.Add("3" , lbD3.Width, HorizontalAlignment.Left); //
            LsvDisp1.Columns.Add("4" , lbD4.Width, HorizontalAlignment.Left); //
            LsvDisp1.Columns.Add("5" , lbD5.Width, HorizontalAlignment.Left); //
            LsvDisp1.Columns.Add("6" , lbD6.Width, HorizontalAlignment.Left); //
            LsvDisp1.Columns.Add("7" , lbD7.Width, HorizontalAlignment.Left); //
            LsvDisp1.Columns.Add("8" , lbD8.Width, HorizontalAlignment.Left); //
            LsvDisp1.Columns.Add("9" , lbD9.Width, HorizontalAlignment.Left); //
            LsvDisp1.Columns.Add("10", lbDA.Width, HorizontalAlignment.Left); //
            LsvDisp1.Columns.Add("11", lbDB.Width, HorizontalAlignment.Left); //
            LsvDisp1.HeaderStyle = ColumnHeaderStyle.None;
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

            if (Log.ShowMessageModal("Confirm", "Do you want to Lot End?") != DialogResult.Yes) return;

            LOT.LotEnd();
            //DM.ARAY[ri.LODR].SetStat(cs.Unknown);
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
            lbDevice.Text       = OM.GetCrntDev().ToString();
            lbLotNo.Text        = LOT.GetLotNo();
            
            DateTime tDateTime  = DateTime.FromOADate(SPC.LOT.Data.StartedAt);
            lbLotStartTime.Text = tDateTime.ToString("HH:mm:ss");

            //btStart.Enabled  = LOT.GetLotOpen();
            btLotEnd.Enabled = LOT.GetLotOpen();

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

            }
            else
            {
                btOperator.Text = "  " + SM.FrmLogOn.GetLevel().ToString();
                

            }

            //Auto Manual 시에 조작 막기
            bool bDoorClose1 = !ML.IO_GetX(xi.Door1)          && !ML.IO_GetX(xi.Door2)           ;
            bool bDoorClose2 = !ML.IO_GetX(xi.ShieldDoorOpen) &&  ML.IO_GetX(xi.ShieldDoorClose) ;
            if(bDoorClose1 && bDoorClose2)
            { 
                if(ML.IO_GetX(xi.ManualSw))
                {
                    pnM1.Enabled = true ;
                    pnM2.Enabled = true ;
                    pnM3.Enabled = true ;
                    pnM4.Enabled = true ;
                    
                    //pnM5.Enabled = true ;
                    
                }
                else
                {
                    btStart   .Enabled = LOT.GetLotOpen();
                }
            }
            else
            {
                //pnM1.Enabled = false;
                //pnM2.Enabled = false;
                //pnM3.Enabled = false;
                //pnM4.Enabled = false;
                //pnM5.Enabled = false;
                //btStart.Enabled = true;
                btStart.Enabled = false;
            }
                        
            //if (SML.FrmLogOn.GetLevel() != (int)EN_LEVEL.LogOff)
            //{
            //    btStart.Enabled = LOT.GetLotOpen();
            //}

            //TimeSpan Span ;
            //try{
            //        Span = TimeSpan.FromMilliseconds(SPC.LOT.Data.RunTime);
            //    }
            //    catch(Exception ex){          
            //        Span = TimeSpan.FromMilliseconds(0);
            //    }
            
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


            //string sCycleTimeSec ;
            //int iCycleTimeMs ;
            
            
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
            //    btAllHome.ForeColor = SEQ._bFlick ? Color.Black : Color.Red;
            //}
            //else {
            //    btAllHome.ForeColor = Color.Black  ;
            //}

            //SPC.LOT.DispLotInfo(lvLotInfo);

            //Refresh();
            //Invalidate(true);
            //pnULDR.Invalidate(true);
            //pnLODR.Update();


            //Power Supply Status Check
            //if(tabControl1.SelectedIndex != 2 && tabControl1.SelectedIndex != 3)
            //{ 
                //string sConnected = "";
                //bool b1,b2;
                ////Anode
                //b1 = ML.IO_GetX(xi.CathodeGND);
                //b2 = ML.IO_GetX(xi.CathodeSWT);
                //if(!b1 &&  b2) sConnected = "SWT" ;
                //if( b1 && !b2) sConnected = "GND" ;
                //if(!b1 && !b2) sConnected = "OPEN";
                //lbA1.Text = ML.IO_GetX(xi.PSAnode)     ? "ON" : "OFF";
                //lbA2.Text = SEQ.aging.Stat[0].bOutput  ? "ON" : "OFF";
                //lbA3.Text = SEQ.aging.Stat[0].bRemote  ? "ON" : "OFF";
                //lbA4.Text = sConnected;                
                //lbA5.Text = SEQ.aging.Stat[0].dVoltage.ToString();
                //lbA6.Text = SEQ.aging.Stat[0].dCurrent.ToString();
                ////Focus
                //b1 = ML.IO_GetX(xi.FocusGND  );
                //b2 = ML.IO_GetX(xi.FocusPower);
                //if(!b1 &&  b2) sConnected = "PWR" ;
                //if( b1 && !b2) sConnected = "GND" ;
                //if(!b1 && !b2) sConnected = "OPEN";
                //lbF1.Text = ML.IO_GetX(xi.PSFocus)     ? "ON" : "OFF";
                //lbF2.Text = SEQ.aging.Stat[1].bOutput  ? "ON" : "OFF";
                //lbF3.Text = SEQ.aging.Stat[1].bRemote  ? "ON" : "OFF";
                //lbF4.Text = sConnected;                
                //lbF5.Text = SEQ.aging.Stat[1].dVoltage.ToString();
                //lbF6.Text = SEQ.aging.Stat[1].dCurrent.ToString();
                ////Gate
                //b1 = ML.IO_GetX(xi.GateGND  );
                //b2 = ML.IO_GetX(xi.GatePower);
                //if(!b1 &&  b2) sConnected = "PWR" ;
                //if( b1 && !b2) sConnected = "GND" ;
                //if(!b1 && !b2) sConnected = "OPEN";
                //lbG1.Text = ML.IO_GetX(xi.PSGate)      ? "ON" : "OFF";
                //lbG2.Text = SEQ.aging.Stat[2].bOutput  ? "ON" : "OFF";
                //lbG3.Text = SEQ.aging.Stat[2].bRemote  ? "ON" : "OFF";
                //lbG4.Text = sConnected;                
                //lbG5.Text = SEQ.aging.Stat[2].dVoltage.ToString();
                //lbG6.Text = SEQ.aging.Stat[2].dCurrent.ToString();

            //}
            
            //if(tabControl1.SelectedIndex == 2)
            //{ 
                //Anode
                bool b1 = ML.IO_GetX(xi.CathodeGND);
                bool b2 = ML.IO_GetX(xi.CathodeSWT);
                string sConnected = "";
                if(!b1 &&  b2) sConnected = "SWT" ;
                if( b1 && !b2) sConnected = "GND" ;
                if(!b1 && !b2) sConnected = "OPEN";
                lbA1.Text = ML.IO_GetX(xi.PSAnode)          ? "ON" : "OFF";
                lbA2.Text = SEQ.ConverTech.Stat[0].bOutput  ? "ON" : "OFF";
                lbA3.Text = SEQ.ConverTech.Stat[0].bRemote  ? "ON" : "OFF";
                lbA4.Text = sConnected;                
                lbA5.Text = SEQ.ConverTech.Stat[0].dVoltage.ToString();
                lbA6.Text = SEQ.ConverTech.Stat[0].dCurrent.ToString();
                //Focus
                b1 = ML.IO_GetX(xi.FocusGND  );
                b2 = ML.IO_GetX(xi.FocusPower);
                if(!b1 &&  b2) sConnected = "PWR" ;
                if( b1 && !b2) sConnected = "GND" ;
                if(!b1 && !b2) sConnected = "OPEN";
                lbF1.Text = ML.IO_GetX(xi.PSFocus)          ? "ON" : "OFF";
                lbF2.Text = SEQ.ConverTech.Stat[1].bOutput  ? "ON" : "OFF";
                lbF3.Text = SEQ.ConverTech.Stat[1].bRemote  ? "ON" : "OFF";
                lbF4.Text = sConnected;                
                lbF5.Text = SEQ.ConverTech.Stat[1].dVoltage.ToString();
                lbF6.Text = SEQ.ConverTech.Stat[1].dCurrent.ToString();
                //Gate
                b1 = ML.IO_GetX(xi.GateGND  );
                b2 = ML.IO_GetX(xi.GatePower);
                if(!b1 &&  b2) sConnected = "PWR" ;
                if( b1 && !b2) sConnected = "GND" ;
                if(!b1 && !b2) sConnected = "OPEN";
                lbG1.Text = ML.IO_GetX(xi.PSGate)           ? "ON" : "OFF";
                lbG2.Text = SEQ.ConverTech.Stat[2].bOutput  ? "ON" : "OFF";
                lbG3.Text = SEQ.ConverTech.Stat[2].bRemote  ? "ON" : "OFF";
                lbG4.Text = sConnected;                
                lbG5.Text = SEQ.ConverTech.Stat[2].dVoltage.ToString();
                lbG6.Text = SEQ.ConverTech.Stat[2].dCurrent.ToString();

                lbR0.Text = SEQ.Daegyum.bOut ? "ON" : "OFF"; 
            //}

            //if(tabControl1.SelectedIndex == 3)
            //{ 
                //Reader Read
                lbR4.Text = SEQ.Daegyum.bOut ? "ON" : "OFF";
                lbR1.Text = SEQ.Daegyum.Stat.dCathod.ToString();
                lbR2.Text = SEQ.Daegyum.Stat.dGate  .ToString();
                lbR3.Text = SEQ.Daegyum.Stat.dFocus .ToString();
                lbR4.Text = SEQ.Daegyum.Stat.iArc   .ToString();
                lbR5.Text = (SEQ.Daegyum.dOnTime/10).ToString() + " " + (SEQ.Daegyum.dOffTime/10).ToString();
            //}



            //Data ListView
            lock(SEQ.lockObject)
            { 
                bool bStat1 = !SEQ.aging.Stat[0].Equals(Stat[0]);
                bool bStat2 = !SEQ.aging.Stat[1].Equals(Stat[1]);
                bool bStat3 = !SEQ.aging.Stat[2].Equals(Stat[2]);
                bool bStat4 = !SEQ.aging.StatD  .Equals(StatD  );

                if (bStat1 || bStat2 || bStat3 || bStat4) 
                {
                    double onTime  = 0;
                    double offTime = 0;
                    string sPath1 = "d:\\SpcLog\\"+ Eqp.sEqpName + "\\LotLog\\" + DateTime.Now.ToString("yyyyMMdd") + "\\" + LOT.GetLotNo() + "\\ALL.csv";
                    string sPath2 = "d:\\SpcLog\\"+ Eqp.sEqpName + "\\LotLog\\" + DateTime.Now.ToString("yyyyMMdd") + "\\" + LOT.GetLotNo() + "\\100.csv";
                    if(SEQ.aging.iStep < SEQ.aging.lst.Count) { 
                        onTime  = SEQ.aging.lst[SEQ.aging.iStep].Cathode.OnTime ;
                        offTime = SEQ.aging.lst[SEQ.aging.iStep].Cathode.OffTime;
                        SetSubItems(LsvDisp,onTime,offTime);
                        SaveCsv(0,sPath1,onTime,offTime);
                        if(onTime >= 100) {
                            SetSubItems(LsvDisp1,onTime,offTime);
                            SaveCsv(1,sPath2,onTime,offTime);
                        }
                    }
                }
                Stat[0] = SEQ.aging.Stat[0] ;
                Stat[1] = SEQ.aging.Stat[1] ;
                Stat[2] = SEQ.aging.Stat[2] ;
                StatD   = SEQ.aging.StatD   ;
            }
            

            //liInput[i].UseItemStyleForSubItems = false; //요건 셀별 색깔 넣을때.
            //liInput[i].UseItemStyleForSubItems = false;

            

            bool bClear1 = false;
            bool bClear2 = false;
            if(LsvDisp.Items.Count  > 3000) bClear1  = true;
            if(LsvDisp1.Items.Count > 3000) bClear2  = true;

            bool bLotOpen = LOT.GetLotOpen();
            if(bLotOpen && !bPreLotOpen) { 
                bClear1  = true; bClear2  = true; 
                FrmGraph1.Chart_Clear(); FrmGraph4.Chart_Clear();
                FrmGraph2.Chart_Clear(); FrmGraph5.Chart_Clear();
                FrmGraph3.Chart_Clear(); FrmGraph6.Chart_Clear();
                FrmGrid.SetList();
                //아킹카운트 초기화
                SEQ.Daegyum.SendArcReset();
                OM.CmnOptn.iDetectCount = 0;
                OM.CmnOptn.iArcCount1   = 0;
                OM.CmnOptn.iArcCount2   = 0;
            }
            bPreLotOpen = LOT.GetLotOpen();

            if(bClear1) { LsvDisp .Clear(); InitLsv ();}
            if(bClear2) { LsvDisp1.Clear(); InitLsv1();}

            //작업시간 
            //TODO :: 한번만 하기
            if(tabControl1.SelectedIndex == 0) //Manual
            { 
                double dTime1 = 0;
                double dTime2 = 0;
                int    iStep  = 0;
                int    iCnt   = 0;
                double dStepTime = 0;
                double dWorkTime = 0;
                for(int i=0; i<SEQ.aging.lst.Count; i++)
                {
                    dTime1 += SEQ.aging.lst[i].Total.Time;
                }
                iStep     = SEQ.aging.iStep ;
                if(SEQ.aging.lst.Count > 0) {
                    dStepTime = SEQ.aging.lst[iStep].Total.Time;
                    dWorkTime = SEQ.aging.lst[iStep].Total.msec;
                    iCnt      = SEQ.aging.lst.Count            ;
                }
                for(int i=0; i<iStep; i++)
                {
                    dTime2 += SEQ.aging.lst[i].Total.Time;
                }
                
                lbTotalTime.Text      =  TimeSpan.FromSeconds(dTime1).ToString(); //남은시간    
                lbRemainTime.Text     = (TimeSpan.FromSeconds(dTime2) + TimeSpan.FromMilliseconds(dWorkTime)).ToString(); 
                
                lbStepNo.Text         = (iStep + 1).ToString() + "/" + iCnt.ToString() + "_" + GetName(iStep + 1); //스텝진행위치
                lbStepWorkTime.Text   =  TimeSpan.FromSeconds(dStepTime).ToString();       //스텝작업시간 
                lbStepRemainTime.Text = (TimeSpan.FromSeconds(dStepTime) - TimeSpan.FromMilliseconds(dWorkTime)).ToString(); //스텝남은시간 

                lbArc.Text  = SEQ.aging.StatD.iArc.ToString();
                lbArcV.Text = OM.CmnOptn.iDetectCount.ToString();

            }
                
            if(SEQ.Daegyum.IsReceiveEnd())
            {
                btR7.Enabled = true;
                btR9.Enabled = true;
                btRB.Enabled = true;
                btR5.Enabled = true;
                btRC.Enabled = true;
                btR6.Enabled = true;

            }

            if(OM.EqpOptn.bTestMode) Test();

            //대겸 연속 누르면 먹통 되는거 방지용으로 넣음
            //btR7.Enabled = SEQ.Daegyum.IsReceiveEnd();
            //btR9.Enabled = SEQ.Daegyum.IsReceiveEnd();
            //btRB.Enabled = SEQ.Daegyum.IsReceiveEnd();
            //btR5.Enabled = SEQ.Daegyum.IsReceiveEnd();
            //btRC.Enabled = SEQ.Daegyum.IsReceiveEnd();
            //btR6.Enabled = SEQ.Daegyum.IsReceiveEnd();
           

            //if (!this.Visible)
            //{
            //    tmUpdate.Enabled = false;
            //    return;
            //}
            tmUpdate.Enabled = true;
            
        }
        private double dT1 = 0;
        private double dT2 = 0;
        private double dT3 = 0;
        private void Test()
        {
            ML.IO_SetY(yi.CathodeGND,false); ML.IO_SetY(yi.GatePower ,false); ML.IO_SetY(yi.FocusGND  ,false);
            ML.IO_SetY(yi.CathodeSWT,false); ML.IO_SetY(yi.GateGND   ,false); ML.IO_SetY(yi.FocusPower,false);

            SEQ.ConverTech.SendB(0,dT1);
            SEQ.ConverTech.SendB(0,dT2);
            SEQ.ConverTech.SendB(0,dT3);
            Delay(30);
            SEQ.ConverTech.SendA(0);
            SEQ.ConverTech.SendA(1);
            SEQ.ConverTech.SendA(2);
            Delay(30);
            ListViewItem liInput = new ListViewItem(GetName(SEQ.aging.iStep));
            liInput.SubItems.Add(DateTime.Now.ToString("hh:mm:ss")); //Time
            liInput.SubItems.Add(SEQ.ConverTech.Stat[0].dVoltage.ToString()); //Anode
            liInput.SubItems.Add(SEQ.ConverTech.Stat[0].dCurrent.ToString()); //Anode ma
            liInput.SubItems.Add(SEQ.ConverTech.Stat[2].dVoltage.ToString()); //Gate
            liInput.SubItems.Add(SEQ.ConverTech.Stat[2].dCurrent.ToString()); //Gate ma
            liInput.SubItems.Add(SEQ.ConverTech.Stat[1].dVoltage.ToString()); //Focus
            liInput.SubItems.Add(SEQ.ConverTech.Stat[1].dCurrent.ToString()); //Focus ma
            liInput.SubItems.Add(SEQ.Daegyum.Stat.dCathod       .ToString()); //Cathod
            liInput.SubItems.Add(SEQ.Daegyum.dOnTime            .ToString()); //Cathod On Time
            liInput.SubItems.Add(SEQ.Daegyum.dOffTime           .ToString()); //Cathod Off Time
            LsvDisp.Items.Add(liInput);
            LsvDisp.EnsureVisible(LsvDisp.Items.Count - 1);
            dT1 += 0.1 ;
            dT2 += 0.1 ;
            dT3 += 0.1 ;
            if(dT1 > 100) dT1 = 0;
            if(dT2 >   6) dT2 = 0;
            if(dT3 >   6) dT3 = 0;

            using (TextWriter tWriter = new StreamWriter(@"d:\TestLog.csv"))
            {
                // ListView의 Item을 하나씩 가져와서..
                foreach (ListViewItem item in LsvDisp.Items)
                {
                    // 원하는 형태의 문자열로 한줄씩 기록합니다.
                    tWriter.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}"
                        , item.SubItems[0].Text
                        , item.SubItems[1].Text
                        , item.SubItems[2].Text
                        , item.SubItems[3].Text
                        , item.SubItems[4].Text
                        , item.SubItems[5].Text
                        , item.SubItems[6].Text
                        , item.SubItems[7].Text
                        , item.SubItems[8].Text
                        , item.SubItems[9].Text
                        ));
                }
            }                


        }

        private void SetSubItems(ListView _listView, double _dOn, double _dOff)
        {

            ListViewItem liInput = new ListViewItem(GetName(SEQ.aging.iStep));
            liInput.SubItems.Add(DateTime.Now.ToString("hh:mm:ss")); //Time
            liInput.SubItems.Add(SEQ.aging.Stat[0].dVoltage.ToString()); //Anode
            liInput.SubItems.Add(SEQ.aging.Stat[0].dCurrent.ToString()); //Anode ma
            liInput.SubItems.Add(SEQ.aging.Stat[1].dVoltage.ToString()); //Gate
            liInput.SubItems.Add(SEQ.aging.StatD.dGate     .ToString()); //Gate ma
            liInput.SubItems.Add(SEQ.aging.Stat[2].dVoltage.ToString()); //Focus
            liInput.SubItems.Add(SEQ.aging.StatD.dFocus    .ToString()); //Focus ma
            liInput.SubItems.Add(SEQ.aging.StatD.dCathod   .ToString()); //Cathod
            liInput.SubItems.Add(_dOn                      .ToString()); //Cathod On Time
            liInput.SubItems.Add(_dOff                     .ToString()); //Cathod Off Time
            //liInput.SubItems.Add(DateTime.Now.ToString("hh:mm:ss")); //Time
            //liInput.SubItems.Add("100.1"); //Anode
            //liInput.SubItems.Add("56.56"); //Anode ma
            //liInput.SubItems.Add("56.56"); //Gate
            //liInput.SubItems.Add("56.56"); //Gate ma
            //liInput.SubItems.Add("56.56"); //Focus
            //liInput.SubItems.Add("56.56"); //Focus ma
            //liInput.SubItems.Add("56.56"); //Cathod
            //liInput.SubItems.Add("56.56"); //Cathod On Time
            //liInput.SubItems.Add("56.56"); //Cathod Off Time

            _listView.Items.Add(liInput);
            _listView.EnsureVisible(_listView.Items.Count - 1);
        }

        private string GetName(int iNo)
        {
            string sName = "";
            if(iNo == 1  ) sName = OM.CmnOptn.sName1 ;
            if(iNo == 2  ) sName = OM.CmnOptn.sName2 ;
            if(iNo == 3  ) sName = OM.CmnOptn.sName3 ;
            if(iNo == 4  ) sName = OM.CmnOptn.sName4 ;
            if(iNo == 5  ) sName = OM.CmnOptn.sName5 ;
            if(iNo == 6  ) sName = OM.CmnOptn.sName6 ;
            if(iNo == 7  ) sName = OM.CmnOptn.sName7 ;
            if(iNo == 8  ) sName = OM.CmnOptn.sName8 ;
            if(iNo == 9  ) sName = OM.CmnOptn.sName9 ;
            if(iNo == 10 ) sName = OM.CmnOptn.sName10;
            if(iNo == 11 ) sName = OM.CmnOptn.sName11;
            if(iNo == 12 ) sName = OM.CmnOptn.sName12;
            if(iNo == 13 ) sName = OM.CmnOptn.sName13;
            if(iNo == 14 ) sName = OM.CmnOptn.sName14;
            if(iNo == 15 ) sName = OM.CmnOptn.sName15;
            if(iNo == 16 ) sName = OM.CmnOptn.sName16;
            if(iNo == 17 ) sName = OM.CmnOptn.sName17;
            if(iNo == 18 ) sName = OM.CmnOptn.sName18;
            if(iNo == 19 ) sName = OM.CmnOptn.sName19;
            if(iNo == 20 ) sName = OM.CmnOptn.sName20;
            
            return sName;
        }

        private void btLotOpen_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            FrmLotOpen = new FormLotOpen();
            FrmLotOpen.Show();
        }

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

            //if(fs != null) fs.Close();
            //if(sw != null) sw.Close();

            DM.SaveMap();

            if(!Eqp.bIgnrCam) FrmCam_XNB.Close();
            FrmGrid.Close();

            FrmGraph1.Close();
            FrmGraph2.Close();
            FrmGraph3.Close();
            FrmGraph4.Close();
            FrmGraph5.Close();
            FrmGraph6.Close();

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

        }

        private void btLotEnd_Click_1(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            LOT.LotEnd();
            //DM.ARAY[ri.LODR].SetStat(cs.Unknown);

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

        }

        private void btLotEnd_Click_2(object sender, EventArgs e)
        {
            if (Log.ShowMessageModal("Confirm", "Do you want to LotEnd?") != DialogResult.Yes) return;

            LOT.LotEnd();
            //DM.ARAY[ri.LODR].SetStat(cs.Unknown);

            btStart.Enabled = false;

        }
       
        private void FormOperation_VisibleChanged(object sender, EventArgs e)
        {
            //if(this.Visible) tmUpdate.Enabled = true;
            if(this.Visible)
            {
                FrmGrid.Parent   = pnRecipe;
            }
            
        }

        public void MakeDoubleBuffered(Control control, bool setting)
        {
            Type controlType = control.GetType();
            PropertyInfo pi = controlType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(control, setting, null);
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //FormTest FrmTest = new FormTest();

 //           FrmTest.Show();
        }

        private void button1_Click_3(object sender, EventArgs e)
        {
        }

        private void btStart_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            SEQ._bBtnStart = true;
        }

        private void pnAnode1_DoubleClick(object sender, EventArgs e)
        {


        }

        private void label11_DoubleClick(object sender, EventArgs e)
        {
            string sText = ((Label)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "jpg File|*.jpg";
            saveFileDialog1.Title = "Save";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName == "") return;

            FrmGraph1.Save(saveFileDialog1.FileName);
        }

        private void label6_DoubleClick(object sender, EventArgs e)
        {
            string sText = ((Label)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "jpg File|*.jpg";
            saveFileDialog1.Title = "Save";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName == "") return;

            FrmGraph2.Save(saveFileDialog1.FileName);
        }

        private void label9_DoubleClick(object sender, EventArgs e)
        {
            string sText = ((Label)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "jpg File|*.jpg";
            saveFileDialog1.Title = "Save";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName == "") return;

            FrmGraph3.Save(saveFileDialog1.FileName);
        }

        private void label3_DoubleClick(object sender, EventArgs e)
        {
            string sText = ((Label)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "jpg File|*.jpg";
            saveFileDialog1.Title = "Save";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName == "") return;

            FrmGraph4.Save(saveFileDialog1.FileName);
        }

        private void label5_DoubleClick(object sender, EventArgs e)
        {
            string sText = ((Label)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "jpg File|*.jpg";
            saveFileDialog1.Title = "Save";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName == "") return;

            FrmGraph5.Save(saveFileDialog1.FileName);
        }

        private void label8_DoubleClick(object sender, EventArgs e)
        {
            string sText = ((Label)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "jpg File|*.jpg";
            saveFileDialog1.Title = "Save";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName == "") return;

            FrmGraph6.Save(saveFileDialog1.FileName);
        }

        private void label71_Click(object sender, EventArgs e)
        {

        }

        private void panel115_Paint(object sender, PaintEventArgs e)
        {

        }

        //출력온
        private void btA1_Click(object sender, EventArgs e)
        {
            string sTag = ((Button)sender).Tag.ToString();
            int iTag = Convert.ToInt32(sTag);
            SEQ.ConverTech.SendD(iTag);
        }
        //출력오프
        private void btA2_Click(object sender, EventArgs e)
        {
            string sTag = ((Button)sender).Tag.ToString();
            int iTag = Convert.ToInt32(sTag);
            SEQ.ConverTech.SendE(iTag);
        }
        //전압설정
        private void btA3_Click(object sender, EventArgs e)
        {
            string sTag = ((Button)sender).Tag.ToString();
            int iTag = Convert.ToInt32(sTag);

            double dValue = 0;
                 if(iTag == 0) dValue = (double)edA1.Value;
            else if(iTag == 1) dValue = (double)edF1.Value;
            else if(iTag == 2) dValue = (double)edG1.Value;

            SEQ.ConverTech.SendB(iTag,dValue);
            Delay(100);
            SEQ.ConverTech.SendC(iTag,dValue);
        }
        public void Delay(int ms)
        {
            DateTime dateTimeNow = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, ms);
            DateTime dateTimeAdd = dateTimeNow.Add(duration);
            while (dateTimeAdd >= dateTimeNow)
            {
                System.Windows.Forms.Application.DoEvents();
                dateTimeNow = DateTime.Now;
            }
            return;
        }
        //상태읽기
        private void btA5_Click(object sender, EventArgs e)
        {
            string sTag = ((Button)sender).Tag.ToString();
            int iTag = Convert.ToInt32(sTag);
            SEQ.ConverTech.SendA(iTag);
        }
        //대겸리더기 셋팅
        private void btR1_Click(object sender, EventArgs e)
        {
            //((Button)sender).Enabled = false;
            string sTag = ((Button)sender).Tag.ToString();
            int iTag = Convert.ToInt32(sTag);

            
            double dValue1 = (double)edR1.Value * 10; 
            double dValue2 = (double)edR2.Value * 10;

                 if(iTag ==  0) { SEQ.Daegyum.SendOutOnOff(true   ); }
            else if(iTag ==  1) { SEQ.Daegyum.SendOutOnOff(false  ); }
            else if(iTag ==  2) { SEQ.Daegyum.SendOnTime  (dValue1); }
            else if(iTag ==  3) { SEQ.Daegyum.SendOffTime (dValue2); }
            else if(iTag ==  4) { SEQ.Daegyum.SendArc     (       ); ((Button)sender).Enabled = false;}  
            else if(iTag ==  5) { SEQ.Daegyum.SendArcReset(       ); }
            else if(iTag ==  6) { SEQ.Daegyum.SendCathod  (       ); ((Button)sender).Enabled = false;}
            else if(iTag ==  7) { SEQ.Daegyum.SendDisplay (false  ); ((Button)sender).Enabled = false;}
            else if(iTag ==  8) { SEQ.Daegyum.SendFocus   (       ); ((Button)sender).Enabled = false;}
            else if(iTag ==  9) { SEQ.Daegyum.SendDisplay (true   ); }
            else if(iTag == 10) { SEQ.Daegyum.SendGate    (       ); ((Button)sender).Enabled = false;}
            else if(iTag == 11) { SEQ.Daegyum.SendAll();             ((Button)sender).Enabled = false;}

            SEQ.ConverTech.SendA(iTag);
            //((Button)sender).Enabled = true ;
        }


        FileStream   fs1,fs2 ;
        StreamWriter sw1,sw2 ;
        string sPrePath1 = "";

        string sPrePath2 = "";
        public void SaveCsv(int _iId, string _sPath, double _OnTime,double _OffTime)
        {
            string sPath = _sPath;// @"D:\Data\" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + ".csv";
            string sDir  = Path.GetDirectoryName(sPath + "\\");
            string sName = Path.GetFileName(sPath);
            if (!CIniFile.MakeFilePathFolder(sDir)) return;
            
            bool b1 = false;
            bool b2 = false;
            if(_iId == 0)
            { 
                if(fs1 != null && sPrePath1 != sPath) {
                    sw1.Close();
                    fs1.Close();
                    b1 = true;
                }
                if(fs1 == null || b1)
                {
                    fs1 = new FileStream(sPath, FileMode.Append, FileAccess.Write);
                    sw1 = new StreamWriter(fs1, Encoding.UTF8);
                }
                sPrePath1 = sPath;
            }
            else
            {
                if(fs2 != null && sPrePath2 != sPath) {
                    sw2.Close();
                    fs2.Close();
                    b2 = true;
                }
                if(fs2 == null || b2)
                {
                    fs2 = new FileStream(sPath, FileMode.Append, FileAccess.Write);
                    sw2 = new StreamWriter(fs2, Encoding.UTF8);
                }
                sPrePath2 = sPath;
            }

            string line = "";
            if (b1 || b2) //!File.Exists(sPath))
            {
                line  = "Name,Time,";
                line += "Anode(kV),Anode(mA),Gate(kV),Gate(mA),Focus(kV),Focus(mA),Cathod(mA),Cathod(On),Cathod(Off),ArcCnt,ArcCnt(V)";
                line += "\r\n";
            }
            
            //FileStream fs   = new FileStream(sPath, FileMode.Append, FileAccess.Write);
            //StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
            line += SEQ.aging.iStep.ToString() + "_" + GetName(SEQ.aging.iStep) + ",";
            line += DateTime.Now.ToString("hh:mm:ss:fff")  + ",";
            line += SEQ.aging.Stat[0].dVoltage .ToString() + ",";
            line += SEQ.aging.Stat[0].dCurrent .ToString() + ",";
            line += SEQ.aging.Stat[1].dVoltage .ToString() + ",";
            line += SEQ.aging.StatD.dGate      .ToString() + ",";
            line += SEQ.aging.Stat[2].dVoltage .ToString() + ",";
            line += SEQ.aging.StatD.dFocus     .ToString() + ",";
            line += SEQ.aging.StatD.dCathod    .ToString() + ",";
            line += _OnTime                    .ToString() + ",";
            line += _OffTime                   .ToString() + ",";
            line += SEQ.aging.StatD.iArc       .ToString() + ",";
            line += OM.CmnOptn.iDetectCount    .ToString() + ",";

            line += "\r\n";

            //sw.WriteLine(line);
            if(_iId == 0) sw1.Write(line);
            else          sw2.Write(line);
            //sw.Close();
            //fs.Close();
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