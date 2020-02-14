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

        public static FormGraph     FrmGraph1C   ;
        public static FormGraph     FrmGraph2C   ;
        public static FormGraph     FrmGraph3C   ;
        public static FormGraph     FrmGraph4C   ;
        public static FormGraph     FrmGraph5C   ;
        public static FormGraph     FrmGraph6C   ;

        public int LotInfoCnt = 6;
        public int DayInfoCnt = 6;        

        //Output 0~5
        FrameOutputAPT[] FraOutputAPT;
        //private string sFormText ;
        private const string sFormText = "Form Operation ";

        //Rs 
        public RS485_ConverTech.TStat[] Stat;
        public RS232_Daegyum_Seasoning.TStat StatD;

        public RS485_ConverTech.TStat[] StatT;
        public RS232_Daegyum_Seasoning.TStat StatDT;

        public int iArk1;
        public int iArk2;

        public FormOperation(Panel _pnBase)
        {
            InitializeComponent();

            //sFormText = this.GetType().Name;
            
            this.TopLevel = false;
            this.Parent = _pnBase;
            //SM.FrmLogOn.

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
            //FrmGraph1.Parent   = pnAnode;
            FrmGraph1.Dock     = DockStyle.Fill;
            FrmGraph1.Show();

            FrmGraph2 = new FormGraph(SeriesChartType.FastLine, "ANODE");
            FrmGraph2.TopLevel = false;
            FrmGraph2.Parent   = pnAnode;
            FrmGraph2.Dock     = DockStyle.Fill;
            FrmGraph2.Show();

            FrmGraph3 = new FormGraph(SeriesChartType.FastLine, "FOCUS V");
            FrmGraph3.TopLevel = false;
            //FrmGraph3.Parent   = pnFocus1;
            FrmGraph3.Dock     = DockStyle.Fill;
            FrmGraph3.Show();

            FrmGraph4 = new FormGraph(SeriesChartType.FastLine, "FOCUS V");
            FrmGraph4.TopLevel = false;
            FrmGraph4.Parent   = pnFocus;
            FrmGraph4.Dock     = DockStyle.Fill;
            FrmGraph4.Show();

            FrmGraph5 = new FormGraph(SeriesChartType.FastLine, "GATE V");
            FrmGraph5.TopLevel = false;
            //FrmGraph5.Parent   = pnGate;
            FrmGraph5.Dock     = DockStyle.Fill;
            FrmGraph5.Show();

            FrmGraph6 = new FormGraph(SeriesChartType.FastLine, "GATE V");
            FrmGraph6.TopLevel = false;
            FrmGraph6.Parent   = pnGate;
            FrmGraph6.Dock     = DockStyle.Fill;
            FrmGraph6.Show();

            //Current
            FrmGraph1C = new FormGraph(SeriesChartType.FastLine, "ANODE I");
            FrmGraph1C.TopLevel = false;
            FrmGraph1C.Parent   = pnAnode;
            FrmGraph1C.Dock     = DockStyle.Fill;
            FrmGraph1C.Visible  = false;
            FrmGraph1C.Show();
            
                
            FrmGraph2C = new FormGraph(SeriesChartType.FastLine, "ANODE I");
            FrmGraph2C.TopLevel = false;
            FrmGraph2C.Parent   = pnCathod;
            FrmGraph2C.Dock     = DockStyle.Fill;
            FrmGraph2C.Visible  = false;
            FrmGraph2C.Show();
                
            FrmGraph3C = new FormGraph(SeriesChartType.FastLine, "FOCUS I");
            FrmGraph3C.TopLevel = false;
            //FrmGraph3C.Parent   = pnFocus1;
            FrmGraph3C.Dock     = DockStyle.Fill;
            FrmGraph3C.Visible  = false;
            FrmGraph3C.Show();
                
            FrmGraph4C = new FormGraph(SeriesChartType.FastLine, "FOCUS I");
            FrmGraph4C.TopLevel = false;
            FrmGraph4C.Parent   = pnFocus;
            FrmGraph4C.Dock     = DockStyle.Fill;
            FrmGraph4C.Visible  = false;
            FrmGraph4C.Show();
                
            FrmGraph5C = new FormGraph(SeriesChartType.FastLine, "GATE I");
            FrmGraph5C.TopLevel = false;
            //FrmGraph5C.Parent   = pnGate;
            FrmGraph5C.Dock     = DockStyle.Fill;
            FrmGraph5C.Visible  = false;
            FrmGraph5C.Show();

            FrmGraph6C = new FormGraph(SeriesChartType.FastLine, "GATE I");
            FrmGraph6C.TopLevel = false;
            FrmGraph6C.Parent   = pnGate;
            FrmGraph6C.Dock     = DockStyle.Fill;
            FrmGraph6C.Visible  = false;
            FrmGraph6C.Show();


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
            SEQ.aging.CClear       += new Aging.Chart_Clear      (FrmGraph1C.Chart_Clear   );
            SEQ.aging.CClear       += new Aging.Chart_Clear      (FrmGraph2C.Chart_Clear   );
            SEQ.aging.CClear       += new Aging.Chart_Clear      (FrmGraph3C.Chart_Clear   );
            SEQ.aging.CClear       += new Aging.Chart_Clear      (FrmGraph4C.Chart_Clear   );
            SEQ.aging.CClear       += new Aging.Chart_Clear      (FrmGraph5C.Chart_Clear   );
            SEQ.aging.CClear       += new Aging.Chart_Clear      (FrmGraph6C.Chart_Clear   );

            SEQ.aging.CBegin       += new Aging.Chart_Begin      (FrmGraph1.Begin          );
            SEQ.aging.CBegin       += new Aging.Chart_Begin      (FrmGraph2.Begin          );
            SEQ.aging.CBegin       += new Aging.Chart_Begin      (FrmGraph3.Begin          );
            SEQ.aging.CBegin       += new Aging.Chart_Begin      (FrmGraph4.Begin          );
            SEQ.aging.CBegin       += new Aging.Chart_Begin      (FrmGraph5.Begin          );
            SEQ.aging.CBegin       += new Aging.Chart_Begin      (FrmGraph6.Begin          );
            SEQ.aging.CBegin       += new Aging.Chart_Begin      (FrmGraph1C.Begin         );
            SEQ.aging.CBegin       += new Aging.Chart_Begin      (FrmGraph2C.Begin         );
            SEQ.aging.CBegin       += new Aging.Chart_Begin      (FrmGraph3C.Begin         );
            SEQ.aging.CBegin       += new Aging.Chart_Begin      (FrmGraph4C.Begin         );
            SEQ.aging.CBegin       += new Aging.Chart_Begin      (FrmGraph5C.Begin         );
            SEQ.aging.CBegin       += new Aging.Chart_Begin      (FrmGraph6C.Begin         );

            SEQ.aging.CEnd         += new Aging.Chart_End        (FrmGraph1.End            );
            SEQ.aging.CEnd         += new Aging.Chart_End        (FrmGraph2.End            );
            SEQ.aging.CEnd         += new Aging.Chart_End        (FrmGraph3.End            );
            SEQ.aging.CEnd         += new Aging.Chart_End        (FrmGraph4.End            );
            SEQ.aging.CEnd         += new Aging.Chart_End        (FrmGraph5.End            );
            SEQ.aging.CEnd         += new Aging.Chart_End        (FrmGraph6.End            );
            SEQ.aging.CEnd         += new Aging.Chart_End        (FrmGraph1C.End           );
            SEQ.aging.CEnd         += new Aging.Chart_End        (FrmGraph2C.End           );
            SEQ.aging.CEnd         += new Aging.Chart_End        (FrmGraph3C.End           );
            SEQ.aging.CEnd         += new Aging.Chart_End        (FrmGraph4C.End           );
            SEQ.aging.CEnd         += new Aging.Chart_End        (FrmGraph5C.End           );
            SEQ.aging.CEnd         += new Aging.Chart_End        (FrmGraph6C.End           );

            SEQ.aging.CAddPoints1C += new Aging.Chart_AddPoints1C(FrmGraph1C.Chart_AddPoints);
            SEQ.aging.CAddPoints2C += new Aging.Chart_AddPoints2C(FrmGraph2C.Chart_AddPoints);
            SEQ.aging.CAddPoints3C += new Aging.Chart_AddPoints3C(FrmGraph3C.Chart_AddPoints);
            SEQ.aging.CAddPoints4C += new Aging.Chart_AddPoints4C(FrmGraph4C.Chart_AddPoints);
            SEQ.aging.CAddPoints5C += new Aging.Chart_AddPoints5C(FrmGraph5C.Chart_AddPoints);
            SEQ.aging.CAddPoints6C += new Aging.Chart_AddPoints6C(FrmGraph6C.Chart_AddPoints);
                                                                           
            SEQ.aging.CSave1C      += new Aging.Chart_Save1C     (FrmGraph1C.Save           );
            SEQ.aging.CSave2C      += new Aging.Chart_Save2C     (FrmGraph2C.Save           );
            SEQ.aging.CSave3C      += new Aging.Chart_Save3C     (FrmGraph3C.Save           );
            SEQ.aging.CSave4C      += new Aging.Chart_Save4C     (FrmGraph4C.Save           );
            SEQ.aging.CSave5C      += new Aging.Chart_Save5C     (FrmGraph5C.Save           );
            SEQ.aging.CSave6C      += new Aging.Chart_Save6C     (FrmGraph6C.Save           );
                                                                          

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

            iArk1 = 0;
            iArk2 = 0;

            
            


        }

        private void InitLsv()
        {
            LsvDisp.Clear();
            LsvDisp.View = View.Details;
            LsvDisp.FullRowSelect = true;
            LsvDisp.GridLines = true;
            LsvDisp.MultiSelect = false;
            LsvDisp.Columns.Add("1" , lbD1.Width,   HorizontalAlignment.Left); //
            LsvDisp.Columns.Add("2" , lbD2.Width,   HorizontalAlignment.Left); //
            LsvDisp.Columns.Add("3" , lbD3.Width,   HorizontalAlignment.Left); //
            LsvDisp.Columns.Add("4" , lbD4.Width,   HorizontalAlignment.Left); //
            LsvDisp.Columns.Add("5" , lbD5.Width,   HorizontalAlignment.Left); //
            LsvDisp.Columns.Add("6" , lbD6.Width,   HorizontalAlignment.Left); //
            LsvDisp.Columns.Add("7" , lbD7.Width,   HorizontalAlignment.Left); //
            LsvDisp.Columns.Add("8" , lbD8.Width,   HorizontalAlignment.Left); //
            LsvDisp.Columns.Add("9" , lbD9.Width,   HorizontalAlignment.Left); //
            LsvDisp.Columns.Add("10", lbD10.Width,  HorizontalAlignment.Left); //
            LsvDisp.Columns.Add("11", lbD1a.Width,  HorizontalAlignment.Left); //
            LsvDisp.Columns.Add("12", lbD2a.Width,  HorizontalAlignment.Left); //
            LsvDisp.Columns.Add("13", lbD3a.Width,  HorizontalAlignment.Left); //
            LsvDisp.Columns.Add("14", lbD4a.Width,  HorizontalAlignment.Left); //
            LsvDisp.Columns.Add("15", lbD5a.Width,  HorizontalAlignment.Left); //
            LsvDisp.Columns.Add("16", lbD6a.Width,  HorizontalAlignment.Left); //
            LsvDisp.Columns.Add("17", lbD7a.Width,  HorizontalAlignment.Left); //
            LsvDisp.Columns.Add("18", lbD8a.Width,  HorizontalAlignment.Left); //
            LsvDisp.Columns.Add("19", lbD9a.Width,  HorizontalAlignment.Left); //
            LsvDisp.Columns.Add("20", lbD10a.Width, HorizontalAlignment.Left); //
            LsvDisp.HeaderStyle = ColumnHeaderStyle.None;
        }

        private void InitLsv1()
        {
            LsvDisp1.Clear();
            LsvDisp1.View = View.Details;
            LsvDisp1.FullRowSelect = true;
            LsvDisp1.GridLines = true;
            LsvDisp1.MultiSelect = false;
            LsvDisp1.Columns.Add("1" , lbD1.Width,   HorizontalAlignment.Left); //
            LsvDisp1.Columns.Add("2" , lbD2.Width,   HorizontalAlignment.Left); //
            LsvDisp1.Columns.Add("3" , lbD3.Width,   HorizontalAlignment.Left); //
            LsvDisp1.Columns.Add("4" , lbD4.Width,   HorizontalAlignment.Left); //
            LsvDisp1.Columns.Add("5" , lbD5.Width,   HorizontalAlignment.Left); //
            LsvDisp1.Columns.Add("6" , lbD6.Width,   HorizontalAlignment.Left); //
            LsvDisp1.Columns.Add("7" , lbD7.Width,   HorizontalAlignment.Left); //
            LsvDisp1.Columns.Add("8" , lbD8.Width,   HorizontalAlignment.Left); //
            LsvDisp1.Columns.Add("9" , lbD9.Width,   HorizontalAlignment.Left); //
            LsvDisp1.Columns.Add("10", lbD10.Width,  HorizontalAlignment.Left); //
            LsvDisp1.Columns.Add("11", lbD1a.Width,  HorizontalAlignment.Left); //
            LsvDisp1.Columns.Add("12", lbD2a.Width,  HorizontalAlignment.Left); //
            LsvDisp1.Columns.Add("13", lbD3a.Width,  HorizontalAlignment.Left); //
            LsvDisp1.Columns.Add("14", lbD4a.Width,  HorizontalAlignment.Left); //
            LsvDisp1.Columns.Add("15", lbD5a.Width,  HorizontalAlignment.Left); //
            LsvDisp1.Columns.Add("16", lbD6a.Width,  HorizontalAlignment.Left); //
            LsvDisp1.Columns.Add("17", lbD7a.Width,  HorizontalAlignment.Left); //
            LsvDisp1.Columns.Add("18", lbD8a.Width,  HorizontalAlignment.Left); //
            LsvDisp1.Columns.Add("19", lbD9a.Width,  HorizontalAlignment.Left); //
            LsvDisp1.Columns.Add("20", lbD10a.Width, HorizontalAlignment.Left); //
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

            //tbLotNo.Text = "";
            LOT.LotEnd();
            //DM.ARAY[ri.LODR].SetStat(cs.Unknown);

            //if(sw1 != null) sw1.Close();
            //if(fs1 != null) fs1.Close();

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
            //lbDevice.Text       = OM.GetCrntDev().ToString();
            if(LOT.GetLotNo() != "") lbLotName.Text = "LOT : " + LOT.GetLotNo();
            else                     lbLotName.Text = "LOT" ;
            //lbLotName.Text = "LOT : " + LOT.GetLotNo();
            lbLotNo.Text        = LOT.GetLotNo();

            btLotOpen.Enabled = !LOT.GetLotOpen();
            if (LOT.GetLotOpen())
            {
                btLotOpen.Text = "작업 중";
            }
            else
            {
                btLotOpen.Text = "작업 시작";
            }
            btLotEnd.Enabled  =  LOT.GetLotOpen() && !SEQ._bRun;

            DateTime tDateTime  = DateTime.FromOADate(SPC.LOT.Data.StartedAt);
            lbLotStartTime.Text = tDateTime.ToString("HH:mm:ss");

            //btStart.Enabled  = LOT.GetLotOpen();
            //btLotEnd.Enabled = LOT.GetLotOpen();

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
                btOperator.Text = " LOG IN";

            }
            else
            {
                btOperator.Text = " " + SM.FrmLogOn.GetLevel().ToString();
                

            }

            //Auto Manual 시에 조작 막기
            //bool bDoorClose1 = !ML.IO_GetX(xi.Door1)          && !ML.IO_GetX(xi.Door2)           ;
            bool bDoorClose2 = !ML.IO_GetX(xi.ShieldDoorOpen) &&  ML.IO_GetX(xi.ShieldDoorClose) ;
            //if(bDoorClose1 && bDoorClose2)
            if(bDoorClose2)
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

            if(SEQ._bRun)
            {
                StatT  = SEQ.aging.Stat ;
                StatDT = SEQ.aging.StatD;
            }
            else
            {
                StatT  = SEQ.ConverTech.Stat ;
                StatDT = SEQ.Daegyum.Stat    ;
            }
                //Anode
                bool b1 = ML.IO_GetX(xi.CathodeGND);
                bool b2 = ML.IO_GetX(xi.CathodeSWT);
                string sConnected = "";
                if(!b1 &&  b2) sConnected = "SWT" ;
                if( b1 && !b2) sConnected = "GND" ;
                if(!b1 && !b2) sConnected = "OPEN";
                lbA1.Text      = ML.IO_GetX(xi.PSAnode) ? "ON" : "OFF";
                lbA1.ForeColor = ML.IO_GetX(xi.PSAnode) ? Color.Red : Color.Black ;

                lbA2.Text      = StatT[0].bOutput  ? "ON" : "OFF";
                lbA2.ForeColor = StatT[0].bOutput  ? Color.Red : Color.Black ;

                lbA3.Text      = StatT[0].bRemote  ? "ON" : "OFF";
                lbA3.ForeColor = StatT[0].bRemote  ? Color.Red : Color.Black ;

                lbA4.Text = sConnected;                
                lbA5.Text = StatT[0].dVoltage.ToString();
                lbA6.Text = StatT[0].dCurrent.ToString();
                //Focus
                b1 = ML.IO_GetX(xi.FocusGND  );
                b2 = ML.IO_GetX(xi.FocusPower);
                if(!b1 &&  b2) sConnected = "PWR" ;
                if( b1 && !b2) sConnected = "GND" ;
                if(!b1 && !b2) sConnected = "OPEN";
                lbF1.Text      = ML.IO_GetX(xi.PSFocus) ? "ON" : "OFF";
                lbF1.ForeColor = ML.IO_GetX(xi.PSFocus) ? Color.Red : Color.Black ;

                lbF2.Text      = StatT[1].bOutput  ? "ON" : "OFF";
                lbF2.ForeColor = StatT[1].bOutput  ? Color.Red : Color.Black ;

                lbF3.Text      = StatT[1].bRemote  ? "ON" : "OFF";
                lbF3.ForeColor = StatT[1].bRemote  ? Color.Red : Color.Black ;

                lbF4.Text = sConnected;                
                lbF5.Text = StatT[1].dVoltage.ToString();
                lbF6.Text = StatT[1].dCurrent.ToString();
                //Gate
                b1 = ML.IO_GetX(xi.GateGND  );
                b2 = ML.IO_GetX(xi.GatePower);
                if(!b1 &&  b2) sConnected = "PWR" ;
                if( b1 && !b2) sConnected = "GND" ;
                if(!b1 && !b2) sConnected = "OPEN";
                lbG1.Text      = ML.IO_GetX(xi.PSGate) ? "ON" : "OFF";
                lbG1.ForeColor = ML.IO_GetX(xi.PSGate) ? Color.Red : Color.Black ;

                lbG2.Text      = StatT[2].bOutput  ? "ON" : "OFF";
                lbG2.ForeColor = StatT[2].bOutput  ? Color.Red : Color.Black ;

                lbG3.Text      = StatT[2].bRemote  ? "ON" : "OFF";
                lbG3.ForeColor = StatT[2].bRemote  ? Color.Red : Color.Black ;

                lbG4.Text = sConnected;                
                lbG5.Text = StatT[2].dVoltage.ToString();
                lbG6.Text = StatT[2].dCurrent.ToString();

                lbR0.Text = SEQ.Daegyum.bOut ? "ON" : "OFF"; 
            //}

            //if(tabControl1.SelectedIndex == 3)
            //{ 
                //Reader Read
                lbR4.Text = SEQ.Daegyum.bOut ? "ON" : "OFF";
                lbR1.Text = StatDT.dCathod.ToString();
                lbR2.Text = StatDT.dGate  .ToString();
                lbR3.Text = StatDT.dFocus .ToString();
                //lbR4.Text = StatDT.iArc   .ToString();
                lbR5.Text = (SEQ.Daegyum.dOnTime/10).ToString() ;
                lbR4.Text = (SEQ.Daegyum.dOffTime/10).ToString();
            //}

            //파워 에러 상태 표기
                 if(StatT[0].iError == 1) lbErr1.Text = "OVER VOLTAGE PROTECTION STATUS";
            else if(StatT[0].iError == 2) lbErr1.Text = "OVER CURRENT PROTECTION STATUS";
            else if(StatT[0].iError == 3) lbErr1.Text = "OVER TEMPERATURE PROTECTION STATUS";
            else                          lbErr1.Text = "";                                                    

                 if(StatT[1].iError == 1) lbErr2.Text = "OVER VOLTAGE PROTECTION STATUS";
            else if(StatT[1].iError == 2) lbErr2.Text = "OVER CURRENT PROTECTION STATUS";
            else if(StatT[1].iError == 3) lbErr2.Text = "OVER TEMPERATURE PROTECTION STATUS";
            else                          lbErr2.Text = "";                                                    

                 if(StatT[2].iError == 1) lbErr3.Text = "OVER VOLTAGE PROTECTION STATUS";
            else if(StatT[2].iError == 2) lbErr3.Text = "OVER CURRENT PROTECTION STATUS";
            else if(StatT[2].iError == 3) lbErr3.Text = "OVER TEMPERATURE PROTECTION STATUS";
            else                          lbErr3.Text = "";                                                    


            //Data ListView
            bool bTest = false;
            if(SEQ._bRun)// && !SEQ.aging.bReading)
            {
                lock(SEQ.lockObject)
                { 
                    int iStep = SEQ.aging.iStep ;
                    string sName = (iStep + 1).ToString() + "/" + SEQ.aging.lst.Count.ToString() + " " + SEQ.aging.lst[iStep].Total.Name ;
         
                    //bool bStat1 = !SEQ.aging.Stat[0].Equals(Stat[0]);
                    //bool bStat2 = !SEQ.aging.Stat[1].Equals(Stat[1]);
                    //bool bStat3 = !SEQ.aging.Stat[2].Equals(Stat[2]);
                    //bool bStat4 = !SEQ.aging.StatD  .Equals(StatD  );
                    bool bStat1 =  SEQ.aging.Stat[0].dSetVoltage != Stat[0].dSetVoltage ;
                    bool bStat2 =  SEQ.aging.Stat[1].dSetVoltage != Stat[1].dSetVoltage ;
                    bool bStat3 =  SEQ.aging.Stat[2].dSetVoltage != Stat[2].dSetVoltage ;
                    bool bStat4 =  false;
                    bool bStat5 =  iArk1 != OM.CmnOptn.iArcCount1 || iArk2 != OM.CmnOptn.iDetectCount ;
                    if(SEQ.aging.lst[SEQ.aging.iStep].Cathode.Mode == Aging.clCMode[0]) //SWITCHING
                    {
                        if(SEQ.aging.lst[iStep].Cathode.OnTime >= 100)
                        {
                            //bStat4 = SEQ.Daegyum.bFocusEnd && SEQ.Daegyum.bGateEnd ;
                            if(OM.CmnOptn.bRdShotGate ) bStat4 = SEQ.Daegyum.bGateEnd                          ; //
                            if(OM.CmnOptn.bRdShotFouce) bStat4 = SEQ.Daegyum.bFocusEnd                         ; //
                            else                        bStat4 = SEQ.Daegyum.bFocusEnd && SEQ.Daegyum.bGateEnd ; //

                            SEQ.Daegyum.bFocusEnd = false;
                            SEQ.Daegyum.bGateEnd  = false;
                        }
                        else
                        {
                            bStat4 = SEQ.aging.StatD.dCathod != StatD.dCathod ;// SEQ.aging.StatD.dFocus != StatD.dFocus || SEQ.aging.StatD.dGate != StatD.dGate ;
                        }

                    }
                    else if(SEQ.aging.lst[SEQ.aging.iStep].Cathode.Mode == Aging.clCMode[1]) //GND
                    {
                        bStat4 = SEQ.aging.Stat[0].dCurrent != Stat[0].dCurrent || SEQ.aging.Stat[1].dCurrent != Stat[1].dCurrent || SEQ.aging.Stat[2].dCurrent != Stat[2].dCurrent  ;
                    } 
                    
                    if (bStat1 || bStat2 || bStat3 || bStat4 || bStat5) 
                    {
                        double onTime  = 0;
                        double offTime = 0;
                        bool   bSwt    = false;
                        string sPath1 = "d:\\SpcLog\\"+ Eqp.sEqpName + "\\LotLog\\" + DateTime.Now.ToString("yyyyMMdd") + "\\" + LOT.GetLotNo() + "\\ALL.csv";
                        string sPath2 = "d:\\SpcLog\\"+ Eqp.sEqpName + "\\LotLog\\" + DateTime.Now.ToString("yyyyMMdd") + "\\" + LOT.GetLotNo() + "\\100.csv";
                        if(SEQ.aging.iStep < SEQ.aging.lst.Count) { 
                            onTime  = SEQ.aging.lst[SEQ.aging.iStep].Cathode.OnTime ;
                            offTime = SEQ.aging.lst[SEQ.aging.iStep].Cathode.OffTime;
                            bSwt    = SEQ.aging.lst[SEQ.aging.iStep].Cathode.Mode == Aging.clCMode[0]; //SWITCHING
                            SetSubItems(LsvDisp,onTime,offTime,sName,bSwt);
                            SaveCsv(0,sPath1,onTime,offTime,sName,bSwt);
                            if(onTime >= 100) {
                                SetSubItems(LsvDisp1,onTime,offTime,sName,bSwt);
                                SaveCsv(1,sPath2,onTime,offTime,sName,bSwt);
                            }
                        }
                    }
                    Stat[0] = SEQ.aging.Stat[0] ;
                    Stat[1] = SEQ.aging.Stat[1] ;
                    Stat[2] = SEQ.aging.Stat[2] ;
                    StatD   = SEQ.aging.StatD   ;

                    iArk1   = OM.CmnOptn.iArcCount1  ;
                    iArk2   = OM.CmnOptn.iDetectCount;
                }
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
                FrmGraph1 .Chart_Clear(); FrmGraph4 .Chart_Clear();
                FrmGraph2 .Chart_Clear(); FrmGraph5 .Chart_Clear();
                FrmGraph3 .Chart_Clear(); FrmGraph6 .Chart_Clear();
                FrmGraph1C.Chart_Clear(); FrmGraph4C.Chart_Clear();
                FrmGraph2C.Chart_Clear(); FrmGraph5C.Chart_Clear();
                FrmGraph3C.Chart_Clear(); FrmGraph6C.Chart_Clear();
                
                FrmGrid.SetList();

                SEQ.Daegyum.SendReset (); //데이터 초기화용
                SEQ.Daegyum.SendReset1(); //데이터 초기화용
                SEQ.Daegyum.SendReset2(); //데이터 초기화용

                SEQ.aging.ClearStep(false);
                //아킹카운트 초기화
                //SEQ.Daegyum.SendArcReset();
                OM.CmnOptn.iDetectCount = 0;
                OM.CmnOptn.iArcCount1   = 0;
                OM.CmnOptn.iArcCount2   = 0;

                //비전 그냥 검사 보기용
                FrmCam_XNB.ArkCnt = 0;

                //그래프 시작
                FrmGraph1.Begin(); FrmGraph1C.Begin();
                FrmGraph2.Begin(); FrmGraph2C.Begin();
                FrmGraph3.Begin(); FrmGraph3C.Begin();
                FrmGraph4.Begin(); FrmGraph4C.Begin();
                FrmGraph5.Begin(); FrmGraph5C.Begin();
                FrmGraph6.Begin(); FrmGraph6C.Begin();

                Stat[0].Clear();
                Stat[1].Clear();
                Stat[2].Clear();
                StatD  .Clear();

                iArk1 = 0 ;
                iArk2 = 0 ;

                iPreArcCount1    = 0 ; //저장시에 아킹 생길때로 체크 하려고 하나 씀.
                iPreDetectCount  = 0 ; //저장시에 아킹 생길때로 체크 하려고 하나 씀.

                SEQ.Daegyum.bFocusEnd = false; //완료 되면 받는거라 여기서 초기화
                SEQ.Daegyum.bGateEnd  = false;

                //SEQ.Daegyum.Stat.Clear(); //통신 보내기 전에 초기화 하는 부분을 없애서 여기서 한번만 하고 있음
                //SEQ.ConverTech.Stat[0].Clear();
                //SEQ.ConverTech.Stat[1].Clear();
                //SEQ.ConverTech.Stat[2].Clear();


                if (!FormDevice.FrmDeviceSet.RecipeCheck())
                {
                    ML.ER_SetErr(ei.AGG_CheckRecipe);
                }

            }
            bPreLotOpen = LOT.GetLotOpen();

            if(bClear1) { LsvDisp .Clear(); InitLsv ();}
            if(bClear2) { LsvDisp1.Clear(); InitLsv1();}

            //작업시간 
            //TODO :: 한번만 하기
            //if(tabControl1.SelectedIndex == 0) //Manual
            { 
                double dTime1 = 0;
                double dTime2 = 0;
                int    iStep  = 0;
                int    iCnt   = 0;
                int    iNo    = 0;
                double dStepTime  = 0;
                double dWorkTime  = 0;
                string sStepName  = "";
                double dOnTime    = 0;
                double dOffTime   = 0;
                for(int i=0; i<SEQ.aging.lst.Count; i++)
                {
                    //dTime1 += SEQ.aging.lst[i].Total.Time;
                    dTime1 += SEQ.aging.GetTime(i);
                }
                iStep     = SEQ.aging.iStep ;
                if(SEQ.aging.lst.Count > iStep) {
                    //dStepTime = SEQ.aging.lst[iStep].Total.Time;
                    dStepTime = SEQ.aging.GetTime(iStep);
                    dWorkTime = SEQ.aging.lst[0].Total.msec;
                    iCnt      = SEQ.aging.lst.Count            ;
                    iNo       = CConfig.StrToIntDef(SEQ.aging.lst[iStep].Total.No,0);
                    sStepName = SEQ.aging.lst[iStep].Total.Name;
                    dOnTime   = SEQ.aging.lst[iStep].Cathode.OnTime ;
                    dOffTime  = SEQ.aging.lst[iStep].Cathode.OffTime;

                    //for(int i=0; i<=iStep; i++) dTime2 += SEQ.aging.lst[i].Total.Time;
                    for(int i=0; i<=iStep; i++) dTime2 += SEQ.aging.GetTime(i);
                }
                
                lbTotalTime.Text      =  TimeSpan.FromSeconds(dTime1).ToString(); //남은시간    
                //lbRemainTime.Text     =  TimeSpan.FromMilliseconds(dWorkTime).ToString(@"hh\:mm\:ss\.fff"); 

                lbRemainTime.Text     =  TimeSpan.FromMilliseconds(dTime1*1000 -dWorkTime).ToString(@"hh\:mm\:ss");  ;
                
                lbStepNo.Text         = (iStep + 1).ToString() + "/" + iCnt.ToString() + "\n" + iNo.ToString() + "(" + sStepName + ")"; //스텝진행위치
                //lbStepNo.Text         = (iStep + 1).ToString() + "/" + iCnt.ToString() + "_" + sStep; //스텝진행위치
                lbStepWorkTime.Text   =  TimeSpan.FromSeconds(dStepTime).ToString();       //스텝작업시간 
                //lbStepRemainTime.Text = (TimeSpan.FromSeconds(dStepTime) - TimeSpan.FromMilliseconds(dWorkTime)).ToString(); //스텝남은시간 
                
                int      sp = TimeSpan.FromSeconds(dTime2).Milliseconds;
                lbStepRemainTime.Text = TimeSpan.FromMilliseconds(dTime2*1000 - dWorkTime).ToString(@"hh\:mm\:ss");

                
                
                lbArc.Text  = OM.CmnOptn.iArcCount1.ToString  () + " / " + OM.CmnOptn.iDetectCount.ToString();//SEQ.aging.StatD.iArc.ToString() + "_" + OM.CmnOptn.iArcCount2;
                double dOnOffTime  = (dOnTime + dOffTime)/1000.0 ;
                if(dOnOffTime > 0 && SEQ.aging.lst.Count > iStep)
                {
                    double dTime       =  SEQ.aging.lst[0].Total.msec / 1000.0 ;
                    double dPreStepTime = 0;
                    for(int i=0; i<iStep; i++) dPreStepTime += SEQ.aging.GetTime(i);
                    //double dShotCount1 =  SEQ.aging.lst[iStep].Total.Time / dOnOffTime ;
                    //double dShotCount2 =  dTime / dOnOffTime ;
                    double dShotCount1 =  SEQ.aging.lst[iStep].Total.Time / dOnOffTime ;
                    double dShotCount2 =  (dTime-dPreStepTime) / dOnOffTime ;

                    lbShot.Text = ((int)dShotCount2).ToString() + " / " + ((int)dShotCount1).ToString();//OM.CmnOptn.iDetectCount.ToString();
                }
                else
                {
                    lbShot.Text = "";
                }
                
                
                

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

            //if(OM.EqpOptn.bTestMode) Test();

            if(tabControl1.SelectedIndex == 3) //절연유 탭일때
            {
                double dNow = DateTime.Now.ToOADate();
                DateTime dt0 = DateTime.FromOADate(OM.Oil.dReplaceTime);
                TimeSpan Sp1 = DateTime.Now - dt0; //TimeSpan.FromMilliseconds(dNow - OM.Oil.dReplaceTime);
                TimeSpan Sp2 = TimeSpan.FromMilliseconds(OM.Oil.dWorkTime    - OM.Oil.dReplaceTime);
                //DateTime dt1 = Span.ToString()
                //DateTime dt2 = DateTime.FromOADate(OM.Oil.dWorkTime    - OM.Oil.dReplaceTime);
                
                DateTime dt3 = DateTime.FromOADate(OM.Oil.dReplaceTime).AddHours(OM.CmnOptn.dProcessTimeLim);
                DateTime dt4 = DateTime.FromOADate(OM.Oil.dReplaceTime).AddHours(OM.CmnOptn.dWorkTimeLim   );
                
                lbO1.Text = dt0.ToString("yyyy/MM/dd hh:mm:ss");
                lbO2.Text = Sp1.ToString(@"dd\/hh\:mm\:ss");
                lbO3.Text = Sp2.ToString(@"dd\/hh\:mm\:ss");
                lbO4.Text = dt3.ToString("yyyy/MM/dd hh:mm:ss");
                lbO5.Text = dt4.ToString("yyyy/MM/dd hh:mm:ss");
            }

            tbPErr1.Text = SEQ.ConverTech.i1Pass.ToString();
            tbPErr2.Text = SEQ.ConverTech.i2Pass.ToString();
            tbPErr3.Text = SEQ.ConverTech.i3Pass.ToString();

            tbRErr1.Text = SEQ.Daegyum.i1Pass.ToString();
            tbRErr2.Text = SEQ.Daegyum.i2Pass.ToString();
            tbRErr3.Text = SEQ.Daegyum.i3Pass.ToString();

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
        private static double dT1 = 0;
        private static double dT2 = 0;
        private static double dT3 = 0;
        public static void TestClear()
        {
            dT1 = 0 ; dT2 = 0 ; dT3 = 0 ;
        }
        private void Test()
        {
            //return;

            //ML.IO_SetY(yi.CathodeGND,false); ML.IO_SetY(yi.GatePower ,false); ML.IO_SetY(yi.FocusGND  ,false);
            //ML.IO_SetY(yi.CathodeSWT,false); ML.IO_SetY(yi.GateGND   ,false); ML.IO_SetY(yi.FocusPower,false);

            if(dT1 == 0 && dT2 == 0 && dT3 == 0)
            {
                SEQ.ConverTech.SendB(0,1);
                Delay(50);
                SEQ.ConverTech.SendB(1,1);
                Delay(50);
                SEQ.ConverTech.SendB(2,1);
                Delay(50);
                SEQ.Daegyum.SendOutOnOff(true);
            }
            SEQ.ConverTech.SendC(0,dT1);
            Delay(500);
            SEQ.ConverTech.SendC(1,dT2);
            Delay(500);
            SEQ.ConverTech.SendC(2,dT3);
            Delay(500);
            SEQ.ConverTech.SendA(0);
            Delay(500);
            SEQ.ConverTech.SendA(1);
            Delay(500);
            SEQ.ConverTech.SendA(2);
            Delay(500);
            SEQ.Daegyum.SendAll();
            Delay(100);
            
            //ListViewItem liInput = new ListViewItem(GetName(SEQ.aging.iStep));
            ListViewItem liInput = new ListViewItem(SEQ.aging.lst[SEQ.aging.iStep].Total.Name);
            liInput.SubItems.Add(DateTime.Now.ToString("hh:mm:ss")); //Time

            //var time = TimeSpan.FromMilliseconds(SEQ.aging.lst[0].Total.msec);
            //liInput.SubItems.Add(time.ToString(@"hh\:mm\:ss\.fff"));

            //liInput.SubItems.Add(SEQ.ConverTech.Stat[0].dSetVoltage.ToString()); //Anode ma
            liInput.SubItems.Add(dT1.ToString()); //Anode ma
            liInput.SubItems.Add(SEQ.ConverTech.Stat[0].dVoltage   .ToString()); //Anode
            liInput.SubItems.Add(SEQ.ConverTech.Stat[0].dCurrent   .ToString()); //Anode
            
            //liInput.SubItems.Add(SEQ.ConverTech.Stat[1].dSetVoltage.ToString()); //Focus
            liInput.SubItems.Add(dT2.ToString()); //F
            liInput.SubItems.Add(SEQ.ConverTech.Stat[1].dVoltage   .ToString()); //Focus
            liInput.SubItems.Add(SEQ.ConverTech.Stat[1].dCurrent   .ToString()); //Focus

            //liInput.SubItems.Add(SEQ.ConverTech.Stat[2].dSetVoltage.ToString()); //Gate
            liInput.SubItems.Add(dT3.ToString()); //F
            liInput.SubItems.Add(SEQ.ConverTech.Stat[2].dVoltage   .ToString()); //Gate
            liInput.SubItems.Add(SEQ.ConverTech.Stat[2].dCurrent   .ToString()); //Gate
            
            liInput.SubItems.Add(SEQ.Daegyum.Stat.dCathod      .ToString()); //Cathod
            liInput.SubItems.Add(SEQ.Daegyum.Stat.dFocus       .ToString()); //Cathod
            liInput.SubItems.Add(SEQ.Daegyum.Stat.dGate        .ToString()); //Cathod
            liInput.SubItems.Add(SEQ.Daegyum.dOnTime            .ToString()); //Cathod On Time
            liInput.SubItems.Add(SEQ.Daegyum.dOffTime           .ToString()); //Cathod Off Time

            LsvDisp.Items.Add(liInput);
            
            if(!bClicked) LsvDisp.EnsureVisible(LsvDisp.Items.Count - 1);
            dT1 += 0.1  ; dT1 = Math.Round((Double)dT1, 1);  
            dT2 += 0.1  ; dT2 = Math.Round((Double)dT2, 2);  
            dT3 += 0.1  ; dT3 = Math.Round((Double)dT3, 2);  
            if(dT1 >  60) dT1 = 0;
            if(dT2 >  50) dT2 = 0;
            if(dT3 >  50) dT3 = 0;
            
            using (TextWriter tWriter = new StreamWriter(@"d:\TestLog.csv"))
            {
                // ListView의 Item을 하나씩 가져와서..
                foreach (ListViewItem item in LsvDisp.Items)
                {
                    // 원하는 형태의 문자열로 한줄씩 기록합니다.
                    tWriter.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15}"
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
                        , item.SubItems[10].Text
                        , item.SubItems[11].Text
                        , item.SubItems[12].Text
                        , item.SubItems[13].Text
                        , item.SubItems[14].Text
                        , item.SubItems[15].Text
                        ));
                }
            }                


        }

        private void SetSubItems(ListView _listView, double _dOn, double _dOff, string _sName, bool _bSwt)
        {
            int iStep = SEQ.aging.iStep;
            int iNo   = CConfig.StrToIntDef(SEQ.aging.lst[iStep].Total.No,0);

            //ListViewItem liInput = new ListViewItem(GetName(SEQ.aging.iStep));
            //ListViewItem liInput = new ListViewItem(GetName(iNo));
            //ListViewItem liInput = new ListViewItem(SEQ.aging.lst[iStep].Total.Name);
            ListViewItem liInput = new ListViewItem(_sName);
            ;
            //liInput.SubItems.Add(DateTime.Now.ToString("hh:mm:ss")); //Time
            var time = TimeSpan.FromMilliseconds(SEQ.aging.lst[0].Total.msec);
            liInput.SubItems.Add(time.ToString(@"hh\:mm\:ss\.fff"));

            //liInput.SubItems.Add("60.00"); //Anode ma
            //liInput.SubItems.Add("60.00"); //Anode
            //liInput.SubItems.Add("60.00"); //Anode
            //liInput.SubItems.Add("60.00"); //Focus
            //liInput.SubItems.Add("60.00"); //Focus
            //liInput.SubItems.Add("60.00"); //Focus
            //liInput.SubItems.Add("60.00"); //Gate
            //liInput.SubItems.Add("60.00"); //Gate
            //liInput.SubItems.Add("60.00"); //Gate
            //liInput.SubItems.Add("60.00"); //Cathod
            //liInput.SubItems.Add("60.00"); //Cathod
            //liInput.SubItems.Add("60.00"); //Cathod
            //liInput.SubItems.Add(_dOn                         .ToString()); //Cathod On Time
            //liInput.SubItems.Add(_dOff                        .ToString()); //Cathod Off Time
            
            liInput.SubItems.Add(SEQ.aging.Stat[0].dSetVoltage.ToString("N1")); //Anode ma
            liInput.SubItems.Add(SEQ.aging.Stat[0].dVoltage   .ToString("N1")); //Anode
            liInput.SubItems.Add(SEQ.aging.Stat[0].dCurrent   .ToString("N2")); //Anode
            
            liInput.SubItems.Add(SEQ.aging.Stat[1].dSetVoltage.ToString("N3")); //Focus
            liInput.SubItems.Add(SEQ.aging.Stat[1].dVoltage   .ToString("N3")); //Focus
            liInput.SubItems.Add(SEQ.aging.Stat[1].dCurrent   .ToString("N2")); //Focus

            liInput.SubItems.Add(SEQ.aging.Stat[2].dSetVoltage.ToString("N3")); //Gate
            liInput.SubItems.Add(SEQ.aging.Stat[2].dVoltage   .ToString("N3")); //Gate
            liInput.SubItems.Add(SEQ.aging.Stat[2].dCurrent   .ToString("N2")); //Gate
            
            double d1 = 0 ;
            double d2 = 0 ;
            double d3 = 0 ;
            if(_bSwt)
            { 
                d1 = SEQ.aging.StatD.dCathod  ;
                d2 = SEQ.aging.StatD.dGate    ;
                d3 = d2 / (d1) * 100.0   ;
            }
            else
            {
                d1 = SEQ.aging.Stat[0].dCurrent ;
                d2 = SEQ.aging.Stat[2].dCurrent ;
                d3 = d2 / (d1 + d2) * 100.0   ;
            }

            //if(lst[iNo].Cathode.Mode == clCMode[0]) //SWITCHING

            liInput.SubItems.Add(d3                           .ToString("N1")); //Gate 누설
            
            liInput.SubItems.Add(SEQ.aging.StatD.dCathod      .ToString("N2")); //Cathod
            liInput.SubItems.Add(SEQ.aging.StatD.dFocus       .ToString("N2")); //Cathod
            liInput.SubItems.Add(SEQ.aging.StatD.dGate        .ToString("N2")); //Cathod
            liInput.SubItems.Add(_dOn                         .ToString(    )); //Cathod On Time
            liInput.SubItems.Add(_dOff                        .ToString(    )); //Cathod Off Time

            liInput.SubItems.Add(OM.CmnOptn.iArcCount1        .ToString(    )); //ARK
            liInput.SubItems.Add(OM.CmnOptn.iDetectCount      .ToString(    )); //ARK

            _listView.Items.Add(liInput);
            if(!bClicked) _listView.EnsureVisible(_listView.Items.Count - 1);
        }

        //private string GetName(int iNo)
        //{
        //    string sName = "";
        //    if(iNo == 1  ) sName = OM.CmnOptn.sName1 ;
        //    if(iNo == 2  ) sName = OM.CmnOptn.sName2 ;
        //    if(iNo == 3  ) sName = OM.CmnOptn.sName3 ;
        //    if(iNo == 4  ) sName = OM.CmnOptn.sName4 ;
        //    if(iNo == 5  ) sName = OM.CmnOptn.sName5 ;
        //    if(iNo == 6  ) sName = OM.CmnOptn.sName6 ;
        //    if(iNo == 7  ) sName = OM.CmnOptn.sName7 ;
        //    if(iNo == 8  ) sName = OM.CmnOptn.sName8 ;
        //    if(iNo == 9  ) sName = OM.CmnOptn.sName9 ;
        //    if(iNo == 10 ) sName = OM.CmnOptn.sName10;
        //    if(iNo == 11 ) sName = OM.CmnOptn.sName11;
        //    if(iNo == 12 ) sName = OM.CmnOptn.sName12;
        //    if(iNo == 13 ) sName = OM.CmnOptn.sName13;
        //    if(iNo == 14 ) sName = OM.CmnOptn.sName14;
        //    if(iNo == 15 ) sName = OM.CmnOptn.sName15;
        //    if(iNo == 16 ) sName = OM.CmnOptn.sName16;
        //    if(iNo == 17 ) sName = OM.CmnOptn.sName17;
        //    if(iNo == 18 ) sName = OM.CmnOptn.sName18;
        //    if(iNo == 19 ) sName = OM.CmnOptn.sName19;
        //    if(iNo == 20 ) sName = OM.CmnOptn.sName20;
            
        //    return sName;
        //}

        private void btLotOpen_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            DateTime dt3 = DateTime.FromOADate(OM.Oil.dReplaceTime).AddHours(OM.CmnOptn.dProcessTimeLim);
            DateTime dt4 = DateTime.FromOADate(OM.Oil.dReplaceTime).AddHours(OM.CmnOptn.dWorkTimeLim   );

            if(OM.CmnOptn.bUsePrecessTime && DateTime.Now.ToOADate() > dt3.ToOADate())
            {
                SM.ERR.SetErr((int)ei.ETC_OilChange,"경과 시간 리밋 경과");
                return;
            }
            if(OM.CmnOptn.bUseWorkTime && DateTime.Now.ToOADate() > dt4.ToOADate())
            {
                SM.ERR.SetErr((int)ei.ETC_OilChange,"작업 시간 리밋 경과");
                return;
            }

            //FrmLotOpen = new FormLotOpen();
            //FrmLotOpen.Show();

            if (tbLotNo.Text == "") {
                Log.ShowMessage("Confirm" , "LOT ID를 입력해 주세요."); 
                return;
            }

            string LotNo  = tbLotNo.Text.Trim();

            LOT.TLot Lot ;
            Lot.sLotNo      = tbLotNo     .Text.Trim() +  DateTime.Now.ToString("_hh.mm.ss");
            LOT.LotOpen(Lot);

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

            if(!Eqp.bIgnrCam) {
                //FrmCam_XNB.Tracker.Close();
                FrmCam_XNB.Close();
            }
            FrmGrid.Close();

            FrmGraph1.Close(); FrmGraph1C.Close();
            FrmGraph2.Close(); FrmGraph2C.Close();
            FrmGraph3.Close(); FrmGraph3C.Close();
            FrmGraph4.Close(); FrmGraph4C.Close();
            FrmGraph5.Close(); FrmGraph5C.Close();
            FrmGraph6.Close(); FrmGraph6C.Close();

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
            timer1.Enabled = this.Visible;
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

        }

        private void label6_DoubleClick(object sender, EventArgs e)
        {

        }

        private void label9_DoubleClick(object sender, EventArgs e)
        {

        }

        private void label3_DoubleClick(object sender, EventArgs e)
        {

        }

        private void label5_DoubleClick(object sender, EventArgs e)
        {

        }

        private void label8_DoubleClick(object sender, EventArgs e)
        {

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
            SEQ.ConverTech.SendA(iTag);            
            //SEQ.ConverTech.CheckReceiveEnd();
        }
        //출력오프
        private void btA2_Click(object sender, EventArgs e)
        {
            string sTag = ((Button)sender).Tag.ToString();
            int iTag = Convert.ToInt32(sTag);
            SEQ.ConverTech.SendE(iTag);
            SEQ.ConverTech.SendA(iTag);
            //SEQ.ConverTech.CheckReceiveEnd();
        }
        //전압설정
        private void btA3_Click(object sender, EventArgs e)
        {
            string sTag = ((Button)sender).Tag.ToString();
            int iTag = Convert.ToInt32(sTag);

            double dValue = 0;
                 if(iTag == 0) {dValue = (double)edA1.Value; if(dValue > 60) {SEQ.ConverTech.SendC(iTag,60);} else {SEQ.ConverTech.SendC(iTag,dValue);} } //100kv,60ma맥스로 넣는다.
            else if(iTag == 1) {dValue = (double)edF1.Value; if(dValue > 50) {SEQ.ConverTech.SendC(iTag,50);} else {SEQ.ConverTech.SendC(iTag,dValue);} }//100kv,60ma맥스로 넣는다.
            else if(iTag == 2) {dValue = (double)edG1.Value; if(dValue > 50) {SEQ.ConverTech.SendC(iTag,50);} else {SEQ.ConverTech.SendC(iTag,dValue);} }//100kv,60ma맥스로 넣는다.
            //SEQ.ConverTech.SendC(iTag,10); //6kv,50ma
            //SEQ.ConverTech.CheckReceiveEnd();
            SEQ.ConverTech.SendB(iTag,dValue);
            //SEQ.ConverTech.SendA(iTag);
            //SEQ.ConverTech.CheckReceiveEnd();
            //Delay(100);
            
        }
        public void Delay(int ms)
        {
            return;
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
            //SEQ.ConverTech.CheckReceiveEnd();
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
            else if(iTag ==  4) { SEQ.Daegyum.SendArc     (       ); }// (Button)sender).Enabled = false;}  
            else if(iTag ==  5) { SEQ.Daegyum.SendArcReset(       ); }
            else if(iTag ==  6) { SEQ.Daegyum.SendCathod  (       ); }//((Button)sender).Enabled = false;}
            else if(iTag ==  7) { SEQ.Daegyum.SendDisplay (false  ); }
            else if(iTag ==  8) { SEQ.Daegyum.SendFocus   (       ); }//((Button)sender).Enabled = false;}
            else if(iTag ==  9) { SEQ.Daegyum.SendDisplay (true   ); }
            else if(iTag == 10) { SEQ.Daegyum.SendGate    (       ); }//((Button)sender).Enabled = false;}
            else if(iTag == 11) { SEQ.Daegyum.SendAll();             }//((Button)sender).Enabled = false;}

            //SEQ.ConverTech.SendA(iTag);
            //((Button)sender).Enabled = true ;
        }


        //FileStream   fs1,fs2 ;
        //StreamWriter sw1,sw2 ;
        string sPrePath1 = "";

        private void b1_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text.ToString();
            string sTag  = ((Button)sender).Tag.ToString();
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            if(sTag == "0") {FrmGraph1C.Hide();FrmGraph2.Visible = true;}//Parent = null; FrmGraph1.Parent   = pnAnode1; }
            if(sTag == "1") {FrmGraph4C.Hide();FrmGraph4.Visible = true;}//Parent = null; FrmGraph2.Parent   = pnAnode2; }
            //if(sTag == "2") {FrmGraph3C.Hide();FrmGraph3.Visible = true;}//Parent = null; FrmGraph3.Parent   = pnFocus1; }
            //if(sTag == "3") {FrmGraph4C.Hide();FrmGraph4.Visible = true;}//Parent = null; FrmGraph4.Parent   = pnFocus2; }
            if(sTag == "4") {FrmGraph6C.Hide();FrmGraph6.Visible = true;}//Parent = null; FrmGraph5.Parent   = pnGate1 ; }
            //if(sTag == "5") {FrmGraph6C.Hide();FrmGraph6.Visible = true;}//Parent = null; FrmGraph6.Parent   = pnGate2 ; }

        }

        private void b2_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text.ToString();
            string sTag  = ((Button)sender).Tag.ToString();
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            if(sTag == "0") {FrmGraph2.Hide();FrmGraph1C.Visible = true;}//Parent = null; FrmGraph1C.Parent   = pnAnode1; }
            if(sTag == "1") {FrmGraph4.Hide();FrmGraph4C.Visible = true;}//Parent = null; FrmGraph2C.Parent   = pnAnode2; }
            //if(sTag == "2") {FrmGraph3.Hide();FrmGraph3C.Visible = true;}//Parent = null; FrmGraph3C.Parent   = pnFocus1; }
            //if(sTag == "3") {FrmGraph4.Hide();FrmGraph4C.Visible = true;}//Parent = null; FrmGraph4C.Parent   = pnFocus2; }
            if(sTag == "4") {FrmGraph6.Hide();FrmGraph6C.Visible = true;}//Parent = null; FrmGraph5C.Parent   = pnGate1 ; }
            //if(sTag == "5") {FrmGraph6.Hide();FrmGraph6C.Visible = true;}//Parent = null; FrmGraph6C.Parent   = pnGate2 ; }

        }

        private void btRst_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            if (Log.ShowMessageModal("Confirm", "Do you want to replace the oil??") != DialogResult.Yes) return;

            OM.Oil.dReplaceTime = DateTime.Now.ToOADate();
            OM.Oil.dProcessTime = DateTime.Now.ToOADate();
            OM.Oil.dWorkTime    = DateTime.Now.ToOADate();
            OM.SaveOilInfo();
        }

        private bool bClicked = false;
        private void LsvDisp_MouseDown(object sender, MouseEventArgs e)
        {
            //bClicked = true;
        }

        private void LsvDisp_MouseUp(object sender, MouseEventArgs e)
        {
            //bClicked = false;
        }

        private void LsvDisp_MouseHover(object sender, EventArgs e)
        {
            //bClicked = true;
        }

        private void LsvDisp_MouseLeave(object sender, EventArgs e)
        {
            //bClicked = false;
        }

        private void LsvDisp1_MouseHover(object sender, EventArgs e)
        {
            //bClicked = true;
        }

        private void LsvDisp1_MouseLeave(object sender, EventArgs e)
        {
            //bClicked = false;
        }


        private int iPreArcCount1   = 0;
        private int iPreDetectCount = 0;
        string sPrePath2 = "";
        public void SaveCsv(int _iId, string _sPath, double _OnTime,double _OffTime, string _sName, bool _bSwt)
        {
            string sPath = _sPath;// @"D:\Data\" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + ".csv";
            string sDir  = Path.GetDirectoryName(sPath + "\\");
            string sName = Path.GetFileName(sPath);
            if (!CIniFile.MakeFilePathFolder(sDir)) return;
            
            //bool b1 = false;
            //bool b2 = false;
            //FileStream   fs = new FileStream(sPath, FileMode.Append, FileAccess.Write);
            //StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
            /*
            if(_iId == 0)
            { 
                //if(fs1 != null && sPrePath1 != sPath) {
                //    sw1.Close();
                //    fs1.Close();
                //    b1 = true;
                //}
                //if(fs1 == null || b1)
                //{
                    fs = new FileStream(sPath, FileMode.Append, FileAccess.Write);
                    sw = new StreamWriter(fs, Encoding.UTF8);
                //}
                //sPrePath1 = sPath;
            }
            else
            {
                //if(fs2 != null && sPrePath2 != sPath) {
                //    sw2.Close();
                //    fs2.Close();
                //    b2 = true;
                //}
                //if(fs2 == null || b2)
                //{
                    fs = new FileStream(sPath, FileMode.Append, FileAccess.Write);
                    sw = new StreamWriter(fs, Encoding.UTF8);
                //}
                //sPrePath2 = sPath;
            }
            */
            
            string line = "";
            //if (b1 || b2) //!File.Exists(sPath))
            if (!File.Exists(sPath))
            {
                line  = "Name,Time(Now),Time(Process),";
                line += "Anode(Set kV),Anode(kV),Anode(mA),Focus(Set kV),Focus(kV),Focus(mA),Gate(Set kV),Gate(kV),Gate(mA),누설 %,Cathod(mA),Focus(mA),Gate(mA),Cathod(On),Cathod(Off),ArcCnt(P),ArcCnt(V),ArcCheck(P),ArcCheck(V)";
                line += "\r\n";
            }

            FileStream   fs ;
            StreamWriter sw ;
            try
            {
                fs = new FileStream(sPath, FileMode.Append, FileAccess.Write);
                sw = new StreamWriter(fs, Encoding.UTF8);

            //FileStream fs   = new FileStream(sPath, FileMode.Append, FileAccess.Write);
            //StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
            int iStep     = SEQ.aging.iStep ;
            int iNo       = CConfig.StrToIntDef(SEQ.aging.lst[iStep].Total.No,0);

            //line += SEQ.aging.iStep.ToString() + "_" + GetName(iNo) + ",";
            //line += SEQ.aging.iStep.ToString() + "_" + SEQ.aging.lst[SEQ.aging.iStep].Total.Name + ",";
            line += _sName + "," ;
            
            double d1 = 0 ;
            double d2 = 0 ;
            double d3 = 0 ;
            if(_bSwt)
            { 
                d1 = SEQ.aging.StatD.dCathod  ;
                d2 = SEQ.aging.StatD.dGate    ;
                d3 = d2 / (d1) * 100.0   ;
            }
            else
            {
                d1 = SEQ.aging.Stat[0].dCurrent ;
                d2 = SEQ.aging.Stat[2].dCurrent ;
                d3 = d2 / (d1 + d2) * 100.0   ;
            }

            var time = TimeSpan.FromMilliseconds(SEQ.aging.lst[0].Total.msec);
            //liInput.SubItems.Add(time.ToString(@"hh\:mm\:ss\.fff"));
            line += DateTime.Now.ToString("hh:mm:ss:fff")  + ",";
            line += time.ToString(@"hh\:mm\:ss\.fff")  + ",";
            line += SEQ.aging.Stat[0].dSetVoltage.ToString("N1") + ",";
            line += SEQ.aging.Stat[0].dVoltage   .ToString("N1") + ",";
            line += SEQ.aging.Stat[0].dCurrent   .ToString("N2") + ",";
            line += SEQ.aging.Stat[1].dSetVoltage.ToString("N3") + ",";
            line += SEQ.aging.Stat[1].dVoltage   .ToString("N3") + ",";
            line += SEQ.aging.Stat[1].dCurrent   .ToString("N2") + ",";
            line += SEQ.aging.Stat[2].dSetVoltage.ToString("N3") + ",";
            line += SEQ.aging.Stat[2].dVoltage   .ToString("N3") + ",";
            line += SEQ.aging.Stat[2].dCurrent   .ToString("N2") + ",";
            line += d3                           .ToString("N1") + ",";
            line += SEQ.aging.StatD.dCathod      .ToString("N2") + ",";
            line += SEQ.aging.StatD.dFocus       .ToString("N2") + ",";
            line += SEQ.aging.StatD.dGate        .ToString("N2") + ",";
            line += _OnTime                      .ToString() + ",";
            line += _OffTime                     .ToString() + ",";
            line += OM.CmnOptn.iArcCount1        .ToString() + ",";
            line += OM.CmnOptn.iDetectCount      .ToString() + ",";

            if(OM.CmnOptn.iArcCount1   != iPreArcCount1  ) line += "Arc,";
            if(OM.CmnOptn.iDetectCount != iPreDetectCount) line += "Arc,";

            iPreArcCount1   = OM.CmnOptn.iArcCount1   ;
            iPreDetectCount = OM.CmnOptn.iDetectCount ;


            line += "\r\n";

            //sw.WriteLine(line);
            //if(_iId == 0) {sw1.Write(line); fs1.Close(); }
            //else          {sw2.Write(line); }

            sw.Write(line);
            sw.Close();
            fs.Close();
            }
            catch
            {
                Log.ShowMessage("File Opened", sPath);
            }

        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            timer1.Enabled = false;

            Point p = FindLocation(LsvDisp);
            int width  = LsvDisp.Width  ;
            int height = LsvDisp.Height ; 
            bool bX1 = p.X          <= Control.MousePosition.X ; 
            bool bX2 = p.X + width  >= Control.MousePosition.X ; 
            bool bY1 = p.Y          <= Control.MousePosition.Y ; 
            bool bY2 = p.Y + height >= Control.MousePosition.Y ; 

            if(bX1 && bX2 && bY1 && bY2)
            {
                bClicked = true;
            }
            else
            {
                bClicked = false;
            }

            //TODO :: TEST
            //if(SEQ.aging.lst.Count > 0) label88.Text = SEQ.aging.lst[0].Gate.EndTime.ToString();

            timer1.Enabled = true;
        }

        private static Point FindLocation(Control ctrl)
        {
            Point p;
            for (p = ctrl.Location; ctrl.Parent != null; ctrl = ctrl.Parent)
                p.Offset(ctrl.Parent.Location);
            return p;
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void tbPErr2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click_4(object sender, EventArgs e)
        {
        }

        private void button1_Click_5(object sender, EventArgs e)
        {
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