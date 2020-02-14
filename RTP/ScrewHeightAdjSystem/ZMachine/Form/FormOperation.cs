using System;
using System.Drawing;
using System.Windows.Forms;
using COMMON;
using SML;
using System.Reflection;
using System.Threading;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
//using VDll;

//TODO :: 
//LotInfoCnt , DayInfoCnt 갯수 지정해 주기 및 화면 컴파일후 배치 맞추기
namespace Machine
{
    public partial class FormOperation : Form
    {
        public static FormOperation FrmOperation;
        //public static FormLotOpen FrmLotOpen;
        public static FormMain FrmMain;

        public int LotInfoCnt = 9;
        public int DayInfoCnt = 4;

        FrameCylinderAPT [] FraCylAPT ;

        private const string sFormText = "Form Operation ";
        
        [DllImport("Kernel32.dll")]
        public static extern bool IsWow64Process(System.IntPtr hProcess, out bool lpSystemInfo);
        public FormOperation(Panel _pnBase)
        {
            InitializeComponent();

            this.TopLevel = false;
            this.Parent = _pnBase;

            this.Dock = DockStyle.Fill;

            //Scable Setting
            int  _iWidth  = _pnBase.Width;
            int  _iHeight = _pnBase.Height;
            
            const int  iWidth  = 1280;
            const int  iHeight = 863;
            
            float widthRatio  = _iWidth   / (float)iWidth;// this.ClientSize.Width;//1280f;
            float heightRatio = _iHeight  / (float)iHeight;//.ClientSize.Height; //863f ;
            
            SizeF scale = new SizeF(widthRatio, heightRatio);

            InitLotInfo();

            MakeDoubleBuffered(pnSTT,true);
            MakeDoubleBuffered(pnPRE,true);
            MakeDoubleBuffered(pnADJ,true);
            MakeDoubleBuffered(pnPST,true);

            MakeDoubleBuffered(pnImage,true);
            pb12.Location = pb11.Location; pb13.Location = pb11.Location;
            pb22.Location = pb21.Location; pb23.Location = pb21.Location;
            pb32.Location = pb31.Location; pb33.Location = pb31.Location;
            pb42.Location = pb41.Location; pb43.Location = pb41.Location;
            //MakeDoubleBuffered(pb11,true);MakeDoubleBuffered(pb21,true);MakeDoubleBuffered(pb31,true);MakeDoubleBuffered(pb41,true);
            //MakeDoubleBuffered(pb12,true);MakeDoubleBuffered(pb22,true);MakeDoubleBuffered(pb32,true);MakeDoubleBuffered(pb42,true);
            //MakeDoubleBuffered(pb13,true);MakeDoubleBuffered(pb23,true);MakeDoubleBuffered(pb33,true);MakeDoubleBuffered(pb43,true);


            DM.ARAY[ri.STT].SetParent(pnSTT); DM.ARAY[ri.STT].Name = "START"     ;
            DM.ARAY[ri.PRE].SetParent(pnPRE); DM.ARAY[ri.PRE].Name = "PRECHECK"  ;
            DM.ARAY[ri.ADJ].SetParent(pnADJ); DM.ARAY[ri.ADJ].Name = "ADJUST"    ;
            DM.ARAY[ri.PST].SetParent(pnPST); DM.ARAY[ri.PST].Name = "POSTCHECK" ;

            //START 작업 시작전 및 작업 끝                                        
            DM.ARAY[ri.STT].SetDisp(cs.None     , "자재없음"    ,Color.White     );
            DM.ARAY[ri.STT].SetDisp(cs.Unkn     , "작업전"      ,Color.Aquamarine);
            DM.ARAY[ri.STT].SetDisp(cs.Good     , "Good"        ,Color.Lime      );
            DM.ARAY[ri.STT].SetDisp(cs.NG1      , "높이측정 NG" ,Color.Red       );
            DM.ARAY[ri.STT].SetDisp(cs.NG2      , "상한토크 NG" ,Color.OrangeRed );
            DM.ARAY[ri.STT].SetDisp(cs.NG3      , "하한토크 NG" ,Color.IndianRed );

            //PRECHECK_작업전높이측정
            DM.ARAY[ri.PRE].SetDisp(cs.None     , "자재없음"    ,Color.White     );
            DM.ARAY[ri.PRE].SetDisp(cs.Unkn     , "작업전"      ,Color.Aquamarine);
            DM.ARAY[ri.PRE].SetDisp(cs.Work     , "작업완료"    ,Color.BlueViolet);
                                                                              
            //ADJUST_너트런너                                                       
            DM.ARAY[ri.ADJ].SetDisp(cs.None     , "자재없음"    ,Color.White     );
            DM.ARAY[ri.ADJ].SetDisp(cs.Unkn     , "작업전"      ,Color.Aquamarine);
            DM.ARAY[ri.ADJ].SetDisp(cs.Work     , "작업완료"    ,Color.BlueViolet);
            DM.ARAY[ri.ADJ].SetDisp(cs.NG1      , "높이측정 NG" ,Color.Red       );
            DM.ARAY[ri.ADJ].SetDisp(cs.NG2      , "상한토크 NG" ,Color.OrangeRed );
            DM.ARAY[ri.ADJ].SetDisp(cs.NG3      , "하한토크 NG" ,Color.IndianRed );

            //POSTCHECK_작업후높이측정                                        
            DM.ARAY[ri.PST].SetDisp(cs.None     , "자재없음"    ,Color.White     );
            DM.ARAY[ri.PST].SetDisp(cs.Unkn     , "작업전"      ,Color.Aquamarine);
            DM.ARAY[ri.PST].SetDisp(cs.Good     , "Good"        ,Color.Lime      );
            DM.ARAY[ri.PST].SetDisp(cs.NG1      , "높이측정 NG" ,Color.Red       );
            DM.ARAY[ri.PST].SetDisp(cs.NG2      , "상한토크 NG" ,Color.OrangeRed );
            DM.ARAY[ri.PST].SetDisp(cs.NG3      , "하한토크 NG" ,Color.IndianRed );

            pnS0.BackColor = Color.White      ;
            pnS1.BackColor = Color.Aquamarine ;
            pnS2.BackColor = Color.BlueViolet ;
            pnS3.BackColor = Color.Lime       ;
            pnS4.BackColor = Color.Red        ;
            pnS5.BackColor = Color.OrangeRed  ;
            pnS6.BackColor = Color.IndianRed  ;
            

            DM.LoadMap();

            DM.ARAY[ri.STT].SetMaxColRow(1, 1 );
            DM.ARAY[ri.PRE].SetMaxColRow(1, 1 );
            DM.ARAY[ri.ADJ].SetMaxColRow(1, 1 );
            DM.ARAY[ri.PST].SetMaxColRow(1, 1 );

            Log.SetMessageListBox(listView2);
        }

        private void FormOperation_Load(object sender, EventArgs e)
        {

            FraCylAPT = new FrameCylinderAPT[(int)ci.MAX_ACTR];
            for (int i = 0; i < (int)ci.MAX_ACTR; i++)
            {
                Control[] CtrlAP = Controls.Find("C" + i.ToString(), true);
            
                //int iCylCtrl = Convert.ToInt32(CtrlAP[0].Tag);
                int iCylCtrl = Convert.ToInt32(i);
                FraCylAPT[i] = new FrameCylinderAPT();
                FraCylAPT[i].TopLevel = false;
                FraCylAPT[i].SetConfig((ci)iCylCtrl, ML.CL_GetName(iCylCtrl).ToString(), ML.CL_GetDirType((ci)iCylCtrl), CtrlAP[0]);
                FraCylAPT[i].Show();
            }







            //시간없어서 그냥 
            //나중에한번 보자. DOCK으로 하면 이상하게 폼이 안붙음.....
            //pnOperMain.Dock = DockStyle.None ;
            //pnOperMain.Left = 0 ;
            //pnOperMain.Top  = 0 ;
            //pnOperMain.Width = Parent.Width ;
            //pnOperMain.Height = Parent.Height ;
            //
            //Log.SetMessageListBox(lvInfo);
            //Log.TraceListView("Program Started");
            //
            //lvError.OwnerDraw = true ; 
            //var PropError = lvError.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            //PropError.SetValue(lvError, true, null);
            
            lvError.View = View.Details ;
            lvError.GridLines = false;
            lvError.HeaderStyle = ColumnHeaderStyle.None ;
            lvError.Columns.Add("err", lvError.Size.Width, HorizontalAlignment.Left);
            var PropError = lvError.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropError.SetValue(lvError, true, null);

        }

        public void InitLotInfo() //오퍼레이션 창용.
        {
            lvLotInfo.Clear();
            lvLotInfo.View = View.Details;
            lvLotInfo.LabelEdit = true;
            lvLotInfo.AllowColumnReorder = true;
            lvLotInfo.FullRowSelect = true;
            lvLotInfo.GridLines = true;
            //lvLotInfo.Sorting = SortOrder.Descending;
            lvLotInfo.Scrollable = true;
            lvLotInfo.Enabled = true;
            //lvLotInfo.TileSize = new Size(lvLotInfo.Width,lvLotInfo.Height);
            //lvLotInfo.Columns.Add("", 200, HorizontalAlignment.Left);     //1280*1024는 109, 1024*768은 78
            //lvLotInfo.Columns.Add("", 300, HorizontalAlignment.Left);     //1280*1024는 109, 1024*768은 78
            lvLotInfo.Columns.Add("", 120, HorizontalAlignment.Left);     //1280*1024는 109, 1024*768은 78
            lvLotInfo.Columns.Add("",  80, HorizontalAlignment.Left);     //1280*1024는 109, 1024*768은 78

            ListViewItem[] liLotInfo = new ListViewItem[LotInfoCnt ];

            for (int i = 0; i < LotInfoCnt ; i++)
            {
                liLotInfo[i] = new ListViewItem();
                liLotInfo[i].SubItems.Add("");


                liLotInfo[i].UseItemStyleForSubItems = false;
                liLotInfo[i].UseItemStyleForSubItems = false;

                //liLotInfo[i].BackColor = Color.White;
                lvLotInfo.Items.Add(liLotInfo[i]);
            }

            var PropLotInfo = lvLotInfo.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropLotInfo.SetValue(lvLotInfo, true, null);
        }
        /*
        public void InitTorqueInfo() //오퍼레이션 창용.
        {
            lvTorqueInfo.Clear();
            lvTorqueInfo.View = View.Details;
            lvTorqueInfo.LabelEdit = true;
            lvTorqueInfo.AllowColumnReorder = true;
            lvTorqueInfo.FullRowSelect = true;
            lvTorqueInfo.GridLines = true;
            //lvTorqueInfo.Sorting = SortOrder.Descending;
            lvTorqueInfo.Scrollable = true;
            lvTorqueInfo.Enabled = true;
            //lvTorqueInfo.TileSize = new Size(lvTorqueInfo.Width,lvTorqueInfo.Height);
            //lvTorqueInfo.Columns.Add("", 200, HorizontalAlignment.Left);     //1280*1024는 109, 1024*768은 78
            //lvTorqueInfo.Columns.Add("", 300, HorizontalAlignment.Left);     //1280*1024는 109, 1024*768은 78
            lvTorqueInfo.Columns.Add("", 175, HorizontalAlignment.Left);     //1280*1024는 109, 1024*768은 78
            //lvTorqueInfo.Columns.Add("", 100, HorizontalAlignment.Left);     //1280*1024는 109, 1024*768은 78

            ListViewItem[] liLotInfo = new ListViewItem[LotInfoCnt * 2];

            for (int i = 0; i < LotInfoCnt * 2; i++)
            {
                liLotInfo[i] = new ListViewItem();
                liLotInfo[i].SubItems.Add("");


                liLotInfo[i].UseItemStyleForSubItems = false;
                liLotInfo[i].UseItemStyleForSubItems = false;

                //liLotInfo[i].BackColor = Color.White;
                lvTorqueInfo.Items.Add(liLotInfo[i]);

            }

            var PropLotInfo = lvTorqueInfo.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropLotInfo.SetValue(lvTorqueInfo, true, null);
        }
        */

        private void FormOperation_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmUpdate.Enabled = false;
            DM.SaveMap();
        }

        private void FormOperation_Shown(object sender, EventArgs e)
        {
            tmUpdate.Enabled = true;
        }

        private void FormOperation_VisibleChanged(object sender, EventArgs e)
        {
            tmUpdate.Enabled = this.Visible;

            //현재 가동중인 제품 디스플레이
            pbDevice.ImageLocation = OM.DevInfo.sImgPath;
            pbDevice.SizeMode = PictureBoxSizeMode.StretchImage;
            

        }

        private void btStart_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ForContext.Frm);

            SEQ._bBtnStart = true;
        }

        //private void btStop_Click(object sender, EventArgs e)
        //{
        //    string sText = ((Button)sender).Text;
        //    Log.TraceListView(sFormText + sText + " Button Clicked", ForContext.Frm);
        //
        //    SEQ._bBtnStop = true;
        //}

        private void btReset_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.TraceListView(sFormText + sText + " Button Clicked", ForContext.Frm);

            SEQ._bBtnReset = true;
        }

        private void btLogIn_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.TraceListView(sFormText + sText + " Button Clicked", ForContext.Frm);

            SM.FrmLogOn.Show();
        }

        private void btHome_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.TraceListView(sFormText + sText + " Button Clicked", ForContext.Frm);

            if (!OM.MstOptn.bDebugMode)
            {
                if (Log.ShowMessageModal("Confirm", "Do you want to perform manual actions?") != DialogResult.Yes) return;
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


        private void PanelRefresh()
        {
            for (int i = 0 ; i < ri.MAX_ARAY ; i++)
            {
                DM.ARAY[i].UpdateAray();
            }

            pnSTT.Refresh();
            pnPRE.Refresh();
            pnADJ.Refresh();
            pnPST.Refresh();

        }


        int iPreErrCnt  = 0;
        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            
            PanelRefresh();

            //로그인/로그아웃 방식
            if (SM.FrmLogOn.GetLevel() == (int)EN_LEVEL.LogOff)
            {
                btLogIn.Text = "  LOG IN";
            }
            else
            {
                btLogIn.Text = "  " + SM.FrmLogOn.GetLevel().ToString();
            }
                      
            //lvLotInfo.Items.Clear();
            SPC.LOT.DispLotInfo(lvLotInfo);
            //SPC.DAY.DispDayInfo(lvLotInfo);

            int iCrntErrCnt = 0;
            for (int i = 0 ; i < ML.ER_MaxCount() ; i++) 
            {
                if (ML.ER_GetErr((ei)i)) iCrntErrCnt++;
            }
            if (iPreErrCnt != iCrntErrCnt ) 
            {
                lvError.Items.Clear();
                int iErrNo = ML.ER_GetLastErr();
                ListViewItem liError ;
                string sErr ;
                for (int i = 0; i < ML.ER_MaxCount(); i++)
                {
                    if (ML.ER_GetErr(i))
                    {
                        sErr = string.Format("{0:000} - ", i) + ML.ER_GetErrName(i) + " _ " + ML.ER_GetErrSubMsg(i);
                        liError = new ListViewItem(sErr);
                        lvError.Items.Add(liError);
                    }
                    else
                    {
                        //sErr = string.Format("{0:000} - ", i) + ML.ER_GetErrName(i) + " _ " + ML.ER_GetErrSubMsg(i);
                        //liError = new ListViewItem(sErr);
                        //lvError.Items.Add(liError);
                    }
                }
                //lvError.Columns[0].Width = lvError.Width - 100 ;
            }
            if (SEQ._iSeqStat != EN_SEQ_STAT.Error && lvError.Items.Count != 0)
            {
                lvError.Items.Clear();
            }
            iPreErrCnt = iCrntErrCnt ;

            if(SM.FrmLogOn.GetLevel() <= EN_LEVEL.Operator)
            {
                tabControl1.TabPages.Remove(tabPage2);
                tabControl2.TabPages.Remove(tpStatus);
                tabControl3.TabPages.Remove(tpData  );
            }
            else
            {
                if(!tabControl1.TabPages.Contains(tabPage2))
                {
                    tabControl1.TabPages.Add(tabPage2);
                }
                if (!tabControl2.TabPages.Contains(tpStatus))
                {
                    tabControl2.TabPages.Add(tpStatus);
                }
                if (!tabControl3.TabPages.Contains(tpData))
                {
                    tabControl3.TabPages.Add(tpData);
                }
            }

            if(!ML.MT_GetHomeDoneAll()){
                btHome.ForeColor = SEQ._bFlick ? Color.White : Color.Red;
            }
            

            //센서 보여주기
            bool bSsr = false;
            //bSsr = ML.IO_GetX(xi.CassetteLeft    ); l1.BackColor = bSsr ? Color.Lime : Color.Gray ; l1.Text = bSsr ? "ON" : "OFF" ;
            //bSsr = ML.IO_GetX(xi.CassetteRight   ); l2.BackColor = bSsr ? Color.Lime : Color.Gray ; l2.Text = bSsr ? "ON" : "OFF" ;
            //bSsr = ML.IO_GetX(xi.WaferVacuum     ); l3.BackColor = bSsr ? Color.Lime : Color.Gray ; l3.Text = bSsr ? "ON" : "OFF" ;
            //bSsr = ML.IO_GetX(xi.WaferOverload   ); l4.BackColor = bSsr ? Color.Lime : Color.Gray ; l4.Text = bSsr ? "ON" : "OFF" ;
            //bSsr = ML.IO_GetX(xi.StageVacuum     ); l5.BackColor = bSsr ? Color.Lime : Color.Gray ; l5.Text = bSsr ? "ON" : "OFF" ;
            //bSsr = ML.IO_GetX(xi.WaferDtSsr      ); l6.BackColor = bSsr ? Color.Lime : Color.Gray ; l6.Text = bSsr ? "ON" : "OFF" ;
            //bSsr = ML.IO_GetX(xi.ManualInspLimit ); l7.BackColor = bSsr ? Color.Lime : Color.Gray ; l7.Text = bSsr ? "ON" : "OFF" ;


            //그냥 상태 색깔 표기용
            //            pnS1.BackColor = Color.Aquamarine ; //작업전
            //            pnS2.BackColor = Color.BlueViolet ; //작업후
            //            pnS3.BackColor = Color.DimGray    ; //빈슬롯
            //            pnS4.BackColor = Color.Yellow     ; //작업중인슬롯

            //전자 마이크로 미터 측정 결과 표기
            lbPreData1.Text = DM.ARAY[ri.PRE].Data.dData1.ToString();
            lbPreData2.Text = DM.ARAY[ri.PRE].Data.dData2.ToString();
            lbPreDataL.Text = (((DM.ARAY[ri.PRE].Data.dData1 - DM.ARAY[ri.PRE].Data.dData2) + OM.DevInfo.dWorkOfs) * -1).ToString("N3") ;

            lbAdjData1.Text = DM.ARAY[ri.ADJ].Data.dData1.ToString();
            lbAdjData2.Text = DM.ARAY[ri.ADJ].Data.dData2.ToString();
            lbAdjDataL.Text = (((DM.ARAY[ri.ADJ].Data.dData1 - DM.ARAY[ri.ADJ].Data.dData2) + OM.DevInfo.dWorkOfs) * -1).ToString("N3") ;

            lbPstData1.Text = DM.ARAY[ri.PST].Data.dData1.ToString();
            lbPstData2.Text = DM.ARAY[ri.PST].Data.dData2.ToString();
            lbPstData3.Text = DM.ARAY[ri.PST].Data.dData3.ToString();
            lbPstData4.Text = DM.ARAY[ri.PST].Data.dData4.ToString();
            lbPstDataL.Text = (((DM.ARAY[ri.PST].Data.dData1 - DM.ARAY[ri.PST].Data.dData2) + OM.DevInfo.dWorkOfs) * -1).ToString("N3") ;
            lbPstDataR.Text = (DM.ARAY[ri.PST].Data.dData3 - DM.ARAY[ri.PST].Data.dData4).ToString("N3") ;
            if (Math.Abs(DM.ARAY[ri.PST].Data.dData3 - DM.ARAY[ri.PST].Data.dData4) <= OM.DevInfo.dCheckTolerance)
            {
                lbPstDataR.ForeColor = Color.Black;
            }
            else
            {
                lbPstDataR.ForeColor = Color.Red;
            }

            lbSttData1.Text = DM.ARAY[ri.STT].Data.dData1.ToString();
            lbSttData2.Text = DM.ARAY[ri.STT].Data.dData2.ToString();
            lbSttData3.Text = DM.ARAY[ri.STT].Data.dData3.ToString();
            lbSttData4.Text = DM.ARAY[ri.STT].Data.dData4.ToString();
            lbSttDataL.Text = (((DM.ARAY[ri.STT].Data.dData1 - DM.ARAY[ri.STT].Data.dData2) + OM.DevInfo.dWorkOfs) * -1).ToString("N3") ;
            lbSttDataR.Text = (DM.ARAY[ri.STT].Data.dData3 - DM.ARAY[ri.STT].Data.dData4).ToString("N3") ;
            if (Math.Abs(DM.ARAY[ri.STT].Data.dData3 - DM.ARAY[ri.STT].Data.dData4) <= OM.DevInfo.dCheckTolerance)
            {
                lbSttDataR.ForeColor = Color.Black;
            }
            else
            {
                lbSttDataR.ForeColor = Color.Red;
            }

            //최대 토크 최소 토크
            List<double> Datas = new List<double>(); 

            Datas.Clear();
            //Datas.Add(DM.ARAY[ri.ADJ].Data.dMaxTq0); Datas.Add(DM.ARAY[ri.ADJ].Data.dMaxTq1); Datas.Add(DM.ARAY[ri.ADJ].Data.dMaxTq2); Datas.Add(DM.ARAY[ri.ADJ].Data.dMaxTq3);
            Datas.Add(DM.ARAY[ri.ADJ].Data.dMaxTq1); Datas.Add(DM.ARAY[ri.ADJ].Data.dMaxTq2); Datas.Add(DM.ARAY[ri.ADJ].Data.dMaxTq3);
            //터치에서 보이는게 N / cm 셋팅
            lbTqA0.Text  = DM.ARAY[ri.ADJ].Data.dLowTq == 0 ? (DM.ARAY[ri.ADJ].Data.dTgtTq3 / 1000).ToString("N3") : (DM.ARAY[ri.ADJ].Data.dLowTq / 1000).ToString("N3");
            lbTqA1.Text  = (DM.ARAY[ri.ADJ].Data.dTgtTq0 / 1000).ToString("N3");
            lbTqA2.Text  = (Datas.Max()                  / 1000).ToString("N3");
            //너트런너 토크 디스플레이 단위가 N/cm 이고 날려주는 데이터가 N.m/1000으로 날려서 환산 위해 /100한다.
            //if (DM.ARAY[ri.ADJ].Data.dTgtTq0/100 >= OM.DevInfo.dMinTq)
            if(DM.ARAY[ri.ADJ].Data.dLowTq == 0)
            {
                lbTqA0.ForeColor = Color.Black;
            }
            else
            {
                lbTqA0.ForeColor = Color.Red;
            }
            if (DM.ARAY[ri.ADJ].Data.dTgtTq0 / 100 > (double)OM.DevInfo.dMinTq)
            {
                lbTqA1.ForeColor = Color.Black;
            }
            else
            {
                lbTqA1.ForeColor = Color.Red;
            }
            if (DM.ARAY[ri.ADJ].Data.dTgtTq0 / 100 < (double)OM.DevInfo.dBfMaxTq)
            {
                lbTqA1.ForeColor = Color.Black;
            }
            else
            {
                lbTqA1.ForeColor = Color.Red;
            }
            if (Datas.Max()/100 < (double)OM.DevInfo.dMaxTq)
            {
                lbTqA2.ForeColor = Color.Black;
            }
            else
            {
                lbTqA2.ForeColor = Color.Red;
            }

            Datas.Clear();
            Datas.Add(DM.ARAY[ri.PST].Data.dMaxTq1); Datas.Add(DM.ARAY[ri.PST].Data.dMaxTq2); Datas.Add(DM.ARAY[ri.PST].Data.dMaxTq3);
            lbTqP0.Text  = DM.ARAY[ri.PST].Data.dLowTq == 0 ? (DM.ARAY[ri.PST].Data.dTgtTq3 / 1000).ToString("N3") : (DM.ARAY[ri.PST].Data.dLowTq / 1000).ToString("N3");
            lbTqP1.Text  = (DM.ARAY[ri.PST].Data.dTgtTq0 / 1000).ToString("N3"); 
            lbTqP2.Text  = (Datas.Max()                  / 1000).ToString("N3");
            //너트런너 토크 디스플레이 단위가 N/cm 이고 날려주는 데이터가 N.m/1000으로 날려서 환산 위해 /100한다.
            //if (DM.ARAY[ri.PST].Data.dTgtTq0/100 >= OM.DevInfo.dMinTq)
            if(DM.ARAY[ri.PST].Data.dLowTq == 0)
            {
                lbTqP0.ForeColor = Color.Black;
            }
            else
            {
                lbTqP0.ForeColor = Color.Red;
            }
            if (DM.ARAY[ri.PST].Data.dTgtTq0 / 100 > (double)OM.DevInfo.dMinTq)
            {
                lbTqP1.ForeColor = Color.Black;
            }
            else
            {
                lbTqP1.ForeColor = Color.Red;
            }
            if (DM.ARAY[ri.PST].Data.dTgtTq0 / 100 < (double)OM.DevInfo.dBfMaxTq)
            {
                lbTqP1.ForeColor = Color.Black;
            }
            else
            {
                lbTqP1.ForeColor = Color.Red;
            }
            if (Datas.Max()/100 < (double)OM.DevInfo.dMaxTq)
            {
                lbTqP2.ForeColor = Color.Black;
            }
            else
            {
                lbTqP2.ForeColor = Color.Red;

            }

            Datas.Clear();
            Datas.Add(DM.ARAY[ri.STT].Data.dMaxTq1); Datas.Add(DM.ARAY[ri.STT].Data.dMaxTq2); Datas.Add(DM.ARAY[ri.STT].Data.dMaxTq3);
            lbTqS0.Text  = DM.ARAY[ri.STT].Data.dLowTq == 0 ? (DM.ARAY[ri.STT].Data.dTgtTq3 / 1000).ToString("N3") : (DM.ARAY[ri.STT].Data.dLowTq / 1000).ToString("N3");
            lbTqS1.Text  = (DM.ARAY[ri.STT].Data.dTgtTq0 / 1000).ToString("N3"); 
            lbTqS2.Text  = (Datas.Max()                  / 1000).ToString("N3");
            //너트런너 토크 디스플레이 단위가 N/cm 이고 날려주는 데이터가 N.m/1000으로 날려서 환산 위해 /100한다.
            //if (DM.ARAY[ri.STT].Data.dTgtTq0/100 >= OM.DevInfo.dMinTq)
            if(DM.ARAY[ri.STT].Data.dLowTq == 0)
            {
                lbTqS0.ForeColor = Color.Black;
            }
            else
            {
                lbTqS0.ForeColor = Color.Red;
            }
            if (DM.ARAY[ri.STT].Data.dTgtTq0 / 100 > (double)OM.DevInfo.dMinTq)
            {
                lbTqS1.ForeColor = Color.Black;
            }
            else
            {
                lbTqS1.ForeColor = Color.Red;
            }
            if (DM.ARAY[ri.STT].Data.dTgtTq0 / 100 < (double)OM.DevInfo.dBfMaxTq)
            {
                lbTqS1.ForeColor = Color.Black;
            }
            else
            {
                lbTqS1.ForeColor = Color.Red;
            }
            if (Datas.Max()/100 < (double)OM.DevInfo.dMaxTq)
            {
                lbTqS2.ForeColor = Color.Black;
            }
            else
            {
                lbTqS2.ForeColor = Color.Red;
            }

            p31.Visible = SM.FrmLogOn.GetLevel() >= EN_LEVEL.Master;
            p32.Visible = SM.FrmLogOn.GetLevel() >= EN_LEVEL.Master;
            p33.Visible = SM.FrmLogOn.GetLevel() >= EN_LEVEL.Master;

            p47.Visible = SM.FrmLogOn.GetLevel() >= EN_LEVEL.Master;
            p48.Visible = SM.FrmLogOn.GetLevel() >= EN_LEVEL.Master;
            p49.Visible = SM.FrmLogOn.GetLevel() >= EN_LEVEL.Master;

            p51.Visible = SM.FrmLogOn.GetLevel() >= EN_LEVEL.Master;
            p52.Visible = SM.FrmLogOn.GetLevel() >= EN_LEVEL.Master;
            p53.Visible = SM.FrmLogOn.GetLevel() >= EN_LEVEL.Master;

            
            lb1.Text = ((((DM.ARAY[ri.ADJ].Data.dData1 - DM.ARAY[ri.ADJ].Data.dData2) + OM.DevInfo.dWorkOfs) * -1) * 360.0).ToString();
            lb2.Text = ((((DM.ARAY[ri.PST].Data.dData1 - DM.ARAY[ri.PST].Data.dData2) + OM.DevInfo.dWorkOfs) * -1) * 360.0).ToString();
            lb3.Text = ((((DM.ARAY[ri.STT].Data.dData1 - DM.ARAY[ri.STT].Data.dData2) + OM.DevInfo.dWorkOfs) * -1) * 360.0).ToString();


            //LotOpen Button
            if (LOT.GetLotOpen() && !SEQ._bRun)
            {
                btLotOpen.Text = "WORK ING";
                //btLotOpen.Enabled = true;
                btLotEnd.Enabled = true;
            }
            else
            {
                btLotOpen.Text = "WORK STT";
                //btLotOpen.Enabled = true ;
                btLotEnd.Enabled = false;
            }

            //
            bool bNone1 = DM.ARAY[ri.STT].CheckAllStat(cs.None); bool bNg1 = DM.ARAY[ri.STT].CheckAllStat(cs.NG1) || DM.ARAY[ri.STT].CheckAllStat(cs.NG2) || DM.ARAY[ri.STT].CheckAllStat(cs.NG3); bool bGood1 = DM.ARAY[ri.STT].CheckAllStat(cs.Good);
            bool bNone2 = DM.ARAY[ri.PRE].CheckAllStat(cs.None);                                                 
            bool bNone3 = DM.ARAY[ri.ADJ].CheckAllStat(cs.None); bool bNg3 = DM.ARAY[ri.ADJ].CheckAllStat(cs.NG1) || DM.ARAY[ri.ADJ].CheckAllStat(cs.NG2) || DM.ARAY[ri.ADJ].CheckAllStat(cs.NG3); bool bGood3 = DM.ARAY[ri.ADJ].CheckAllStat(cs.Good);
            bool bNone4 = DM.ARAY[ri.PST].CheckAllStat(cs.None); bool bNg4 = DM.ARAY[ri.PST].CheckAllStat(cs.NG1) || DM.ARAY[ri.PST].CheckAllStat(cs.NG2) || DM.ARAY[ri.PST].CheckAllStat(cs.NG3); bool bGood4 = DM.ARAY[ri.PST].CheckAllStat(cs.Good);

                 if (bGood1) { pb11.Visible = false ; pb12.Visible = true    ; pb13.Visible = false   ; }
            else if (bNg1  ) { pb11.Visible = false ; pb12.Visible = false   ; pb13.Visible = true    ; }
            else             { pb11.Visible = !bNone1;pb12.Visible = false   ; pb13.Visible = false   ; }   

            pb21.Visible = !bNone2; pb22.Visible = false; pb23.Visible = false; 

                 if (bGood3) { pb31.Visible = false ; pb32.Visible = true    ; pb33.Visible = false   ; }
            else if (bNg3  ) { pb31.Visible = false ; pb32.Visible = false   ; pb33.Visible = true    ; }
            else             { pb31.Visible = !bNone3;pb32.Visible = false   ; pb33.Visible = false   ; }   

                 if (bGood4) { pb41.Visible = false ; pb42.Visible = true    ; pb43.Visible = false   ; }
            else if (bNg4  ) { pb41.Visible = false ; pb42.Visible = false   ; pb43.Visible = true    ; }
            else             { pb41.Visible = !bNone4;pb42.Visible = false   ; pb43.Visible = false   ; }   

            //
            if(ML.IO_GetX(xi.HghtNGBoxDtct)) lbX14.Text = "X14 : ON"  ;
            else                          lbX14.Text = "X14 : OFF" ;

            //OK,NG 판정
            switch(DM.ARAY[ri.STT].GetStat(0, 0))
            {
                default      : lbJudge.Text = ""           ; pnJudge.BackColor = Color.Aqua     ; lbData.Text = ""                                                                    ; break;
                case cs.Good : lbJudge.Text = "OK"         ; pnJudge.BackColor = Color.Lime     ; lbData.Text = (DM.ARAY[ri.STT].Data.dData3 - DM.ARAY[ri.STT].Data.dData4).ToString(); break;
                case cs.NG1  : lbJudge.Text = "높이측정 NG"; pnJudge.BackColor = Color.Red       ; lbData.Text = (DM.ARAY[ri.STT].Data.dData3 - DM.ARAY[ri.STT].Data.dData4).ToString(); break;
                case cs.NG2  : lbJudge.Text = "상한토크 NG"; pnJudge.BackColor = Color.OrangeRed ; lbData.Text = lbTqS2.Text                                                           ; break;
                case cs.NG3  : lbJudge.Text = "하한토크 NG"; pnJudge.BackColor = Color.IndianRed ; lbData.Text = (DM.ARAY[ri.STT].Data.dLowTq /1000).ToString("N3")                    ; break;
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
            //string sText = ((Button)sender).Text ;
            //Log.TraceListView(sFormText + sText + " Button Clicked", ForContext.Frm);
            //
            //string sTag = ((Button)sender).Tag.ToString();
            //if(sTag.Contains("W"))
            //{
            //    sTag = sTag.Remove(0,1);
            //    int.TryParse(sTag,out int iTag);
            //    DM.ARAY[ri.CST].SetStat(0, OM.DevInfo.iRowCount - iTag, cs.Unkn);
            //}
            //else
            //{
            //    sTag = sTag.Remove(0, 1);
            //    int.TryParse(sTag, out int iTag);
            //    DM.ARAY[ri.CST].SetStat(0, OM.DevInfo.iRowCount - iTag, cs.Empty);
            //}
        }

        private void btManualClick(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text ;
            Log.TraceListView(sFormText + sText + " Button Clicked", ForContext.Frm);
 
            if (Log.ShowMessageModal("Confirm", "Do you want to perform manual actions?") != DialogResult.Yes) return;

            
            string sTag = ((Button)sender).Tag.ToString();
            int.TryParse(sTag, out int iTag);
            //Log.SetMessageListBox(listView2);
            MM.SetManCycle((mc)iTag);
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            //string sText = ((Button)sender).Text ;
            //Log.TraceListView(sFormText + sText + " Button Clicked", ForContext.Frm);
            //
            //DM.ARAY[ri.CST].SetStat(cs.Unkn);
        }

        private void btAllWafer_Click(object sender, EventArgs e)
        {
            //string sText = ((Button)sender).Text ;
            //Log.TraceListView(sFormText + sText + " Button Clicked", ForContext.Frm);
            //
            //DM.ARAY[ri.CST].SetStat(cs.Unkn);
        }

        private void btRemoveStg_Click(object sender, EventArgs e)
        {
            //string sText = ((Button)sender).Text ;
            //Log.TraceListView(sFormText + sText + " Button Clicked", ForContext.Frm);
            //
            //if (Log.ShowMessageModal("Confirm", "스테이지 자재 데이터를 제거 하시겠습니까?") != DialogResult.Yes) return;
            //DM.ARAY[ri.STG].SetStat(cs.None);

        }

        private void btRemovePck_Click(object sender, EventArgs e)
        {
            //string sText = ((Button)sender).Text ;
            //Log.TraceListView(sFormText + sText + " Button Clicked", ForContext.Frm);
            //
            //if (Log.ShowMessageModal("Confirm", "스테이지 자재 데이터를 제거 하시겠습니까?") != DialogResult.Yes) return;
            //DM.ARAY[ri.PCK].SetStat(cs.None);

        }

        private void pnPckWafer_Click(object sender, EventArgs e)
        {
//            btRemovePck.PerformClick();
        }

        private void pnStgWafer_Click(object sender, EventArgs e)
        {
//            btRemoveStg.PerformClick();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            SEQ.rsNut.SendReport();
            //SEQ.rsNut.SendRead(10);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SEQ.rsNut.SendWrite(129, 1);//) return true;//시계방향
            //SEQ.rsNut.SendWrite(149,5);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
        }

        private void btLotOpen_Click(object sender, EventArgs e)
        {
            new FormLotOpen().Show();
        }

        private void btLotEnd_Click(object sender, EventArgs e)
        {
            LOT.LotEnd();
        }

        private void btReset_MouseDown(object sender, MouseEventArgs e)
        {
            SEQ.rsNut.SetReset(yi.NutReset, true);
        }

        private void btReset_MouseUp(object sender, MouseEventArgs e)
        {
            SEQ.rsNut.SetReset(yi.NutReset, false);
        }

        private void button6_MouseDown(object sender, MouseEventArgs e)
        {
            ML.IO_SetY(yi.HSensorStart,true);
        }

        private void button6_MouseUp(object sender, MouseEventArgs e)
        {
            ML.IO_SetY(yi.HSensorStart, false);
        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void pnDataMap_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pb1_DoubleClick(object sender, EventArgs e)
        {
            if (Log.ShowMessageModal("Confirm", "자재 데이터를 제거 하시겠습니까 ?") != DialogResult.Yes) return;
            string sTag = ((PictureBox)sender).Tag.ToString();

            if (sTag == "1") DM.ARAY[ri.STT].SetStat(cs.None);
            if (sTag == "2") DM.ARAY[ri.PRE].SetStat(cs.None);
            if (sTag == "3") DM.ARAY[ri.ADJ].SetStat(cs.None);
            if (sTag == "4") DM.ARAY[ri.PST].SetStat(cs.None);

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
