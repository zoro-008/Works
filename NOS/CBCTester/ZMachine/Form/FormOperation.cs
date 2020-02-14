using System;
using System.Drawing;
using System.Windows.Forms;
using COMMON;
using SML;
using System.Reflection;
using System.Threading;
using System.Runtime.InteropServices;
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

        public int LotInfoCnt = 6;
        public int DayInfoCnt = 4;

        private const string sFormText = "Form Operation ";
        
        [DllImport("Kernel32.dll")]
        public static extern bool IsWow64Process(System.IntPtr hProcess, out bool lpSystemInfo);

        ValveDisplay ValveDisp;

        public FormOperation(Panel _pnBase)
        {
            InitializeComponent();

            this.TopLevel = false;
            this.Parent = _pnBase;

            this.Dock = DockStyle.Fill;

            //Scable Setting
            //int  _iWidth  = _pnBase.Width;
            //int  _iHeight = _pnBase.Height;
            //
            //const int  iWidth  = 1280;
            //const int  iHeight = 863;
            //
            //float widthRatio  = _iWidth   / (float)iWidth;// this.ClientSize.Width;//1280f;
            //float heightRatio = _iHeight  / (float)iHeight;//.ClientSize.Height; //863f ;
            //
            //SizeF scale = new SizeF(widthRatio, heightRatio);

            DispLotInfo();         

            MakeDoubleBuffered(pnLDR,true);
            MakeDoubleBuffered(pnPKR,true);
            MakeDoubleBuffered(pnSUT,true);
            MakeDoubleBuffered(pnSYR,true);
            MakeDoubleBuffered(pnCHA,true);
            MakeDoubleBuffered(pnFRG,true);
            MakeDoubleBuffered(pnBFR,true);                             
            MakeDoubleBuffered(pnRPK,true);                             
            MakeDoubleBuffered(pnLPK,true);                             

            DM.ARAY[ri.LDR].SetParent(pnLDR); DM.ARAY[ri.LDR].Name = "LDR";
            DM.ARAY[ri.PKR].SetParent(pnPKR); DM.ARAY[ri.PKR].Name = "PKR";
            DM.ARAY[ri.SUT].SetParent(pnSUT); DM.ARAY[ri.SUT].Name = "SUT";
            DM.ARAY[ri.SYR].SetParent(pnSYR); DM.ARAY[ri.SYR].Name = "SYR";
            DM.ARAY[ri.CHA].SetParent(pnCHA); DM.ARAY[ri.CHA].Name = "CHA";
            DM.ARAY[ri.FRG].SetParent(pnFRG); DM.ARAY[ri.FRG].Name = "FRG";
            DM.ARAY[ri.BFR].SetParent(pnBFR); DM.ARAY[ri.BFR].Name = "BFR";
            DM.ARAY[ri.RPK].SetParent(pnRPK); DM.ARAY[ri.RPK].Name = "RPK";
            DM.ARAY[ri.LPK].SetParent(pnLPK); DM.ARAY[ri.LPK].Name = "LPK";

            //Loader
            DM.ARAY[ri.LDR].SetDisp(cs.None    , "None"    ,Color.White     );
            DM.ARAY[ri.LDR].SetDisp(cs.Mask    , "Mask"    ,Color.Gray      );
            DM.ARAY[ri.LDR].SetDisp(cs.Shake   , "Shake"   ,Color.Aqua      );
            DM.ARAY[ri.LDR].SetDisp(cs.Empty   , "Empty"   ,Color.DarkGray  );
            DM.ARAY[ri.LDR].SetDisp(cs.WorkEnd , "WorkEnd" ,Color.Purple    );
                                                                             
            //Picker                                                      
            DM.ARAY[ri.PKR].SetDisp(cs.None    , "None"    ,Color.White     );
            DM.ARAY[ri.PKR].SetDisp(cs.Shake   , "Shake"   ,Color.Aqua      );
            DM.ARAY[ri.PKR].SetDisp(cs.Barcode , "Barcode" ,Color.Brown     );
            DM.ARAY[ri.PKR].SetDisp(cs.WorkEnd , "WorkEnd" ,Color.Purple    );

            //Suttle                                                        
            DM.ARAY[ri.SUT].SetDisp(cs.None    , "None"    ,Color.White     );
            DM.ARAY[ri.SUT].SetDisp(cs.Barcode , "Barcode" ,Color.Brown     );
            DM.ARAY[ri.SUT].SetDisp(cs.Work    , "Work"    ,Color.Blue      );
            DM.ARAY[ri.SUT].SetDisp(cs.WorkEnd , "WorkEnd" ,Color.Purple    );
            DM.ARAY[ri.SUT].SetDisp(cs.Shake   , "Shake"   ,Color.Aqua      );

            //SYR
            DM.ARAY[ri.SYR].SetDisp(cs.None    , "None"    ,Color.White     );
            DM.ARAY[ri.SYR].SetDisp(cs.Clean   , "Clean"   ,Color.Lime      );
            DM.ARAY[ri.SYR].SetDisp(cs.Work    , "Work"    ,Color.Blue      );
            //DM.ARAY[ri.SYR].SetDisp(cs.Waste   , "Waste"   ,Color.Violet    );

            //CHA
            DM.ARAY[ri.CHA].SetDisp(cs.None    , "None"    ,Color.White     );
            DM.ARAY[ri.CHA].SetDisp(cs.Ready   , "Ready"   ,Color.Yellow    );
            DM.ARAY[ri.CHA].SetDisp(cs.Fill    , "Fill"    ,Color.Violet    );
            DM.ARAY[ri.CHA].SetDisp(cs.Work    , "Work"    ,Color.Blue      );
            DM.ARAY[ri.CHA].SetDisp(cs.WorkEnd , "WorkEnd" ,Color.Purple    );
            DM.ARAY[ri.CHA].SetDisp(cs.Clean   , "Clean"   ,Color.Lime      );


            //FRG
            DM.ARAY[ri.FRG].SetDisp(cs.None    , "None"    ,Color.White     );
            DM.ARAY[ri.FRG].SetDisp(cs.Shake   , "Shake"   ,Color.Aqua      );
            DM.ARAY[ri.FRG].SetDisp(cs.Freeze  , "Freeze"  ,Color.PowderBlue);
            DM.ARAY[ri.FRG].SetDisp(cs.WorkEnd , "WorkEnd" ,Color.Purple    );

            //BFR
            DM.ARAY[ri.BFR].SetDisp(cs.None    , "None"    ,Color.White     );
            DM.ARAY[ri.BFR].SetDisp(cs.Freeze  , "Freeze"  ,Color.PowderBlue);
            DM.ARAY[ri.BFR].SetDisp(cs.Shake   , "Shake"   ,Color.Aqua      );
            DM.ARAY[ri.BFR].SetDisp(cs.WorkEnd , "WorkEnd" ,Color.Purple    );
            
            //RPK
            DM.ARAY[ri.RPK].SetDisp(cs.None    , "None"    ,Color.White     );
            DM.ARAY[ri.RPK].SetDisp(cs.Freeze  , "Freeze"  ,Color.PowderBlue);
            DM.ARAY[ri.RPK].SetDisp(cs.WorkEnd , "WorkEnd" ,Color.Purple    );
            
            //LPK
            DM.ARAY[ri.LPK].SetDisp(cs.None    , "None"    ,Color.White     );
            DM.ARAY[ri.LPK].SetDisp(cs.Shake   , "Shake"   ,Color.Aqua      );
            DM.ARAY[ri.LPK].SetDisp(cs.WorkEnd , "WorkEnd" ,Color.Purple    );


            DM.LoadMap();

            DM.ARAY[ri.LDR].SetMaxColRow(1,1);
            DM.ARAY[ri.PKR].SetMaxColRow(1,1);
            DM.ARAY[ri.SUT].SetMaxColRow(1,1);
            DM.ARAY[ri.SYR].SetMaxColRow(1,1);
            DM.ARAY[ri.CHA].SetMaxColRow(6,1);
            DM.ARAY[ri.FRG].SetMaxColRow(1,3);
            DM.ARAY[ri.BFR].SetMaxColRow(1,3);
            DM.ARAY[ri.RPK].SetMaxColRow(1,1);
            DM.ARAY[ri.LPK].SetMaxColRow(1,1);
            
            ValveDisp = new ValveDisplay(pnValves);

            MakeDoubleBuffered(pnValves, true);
        }


        private void FormOperation_Load(object sender, EventArgs e)
        {
            //시간없어서 그냥 
            //나중에한번 보자. DOCK으로 하면 이상하게 폼이 안붙음.....
            pnOperMain.Dock = DockStyle.None ;
            pnOperMain.Left = 0 ;
            pnOperMain.Top  = 0 ;
            pnOperMain.Width = Parent.Width ;
            pnOperMain.Height = Parent.Height ;

            Log.SetMessageListBox(lvInfo);
            Log.TraceListView("Program Started");

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

         public void DispLotInfo() //오퍼레이션 창용.
        {
            lvLotInfo.Clear();
            lvLotInfo.View = View.Details;
            lvLotInfo.LabelEdit = true;
            lvLotInfo.AllowColumnReorder = true;
            lvLotInfo.FullRowSelect = true;
            lvLotInfo.GridLines = true;
            lvLotInfo.Scrollable = true;
            lvLotInfo.Enabled = true;

            lvLotInfo.Columns.Add("", 200, HorizontalAlignment.Left);     //1280*1024는 109, 1024*768은 78
            lvLotInfo.Columns.Add("", lvLotInfo.Width - 200, HorizontalAlignment.Left);     //1280*1024는 109, 1024*768은 78

            ListViewItem[] liLotInfo = new ListViewItem[LotInfoCnt*2];
        
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

            cbQC.Checked = OM.CmnOptn.bAutoQc;
        }

        private void btStart_Click(object sender, EventArgs e)
        {
            if(OM.CmnOptn.bAutoQc)
            {
                if(Log.ShowMessageModal("WARNING" , "오토QC 대기 상태인데 START 하시겠습니까?") != DialogResult.Yes) return ;
                Part.IO_SetY(yi.ETC_MainPumpOff, false);
            }
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ForContext.Frm);

            SEQ._bBtnStart = true;
        }

        private void btStop_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ForContext.Frm);

            SEQ._bBtnStop = true;
        }

        private void btReset_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ForContext.Frm);

            SEQ._bBtnReset = true;
        }

        private void btLogIn_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ForContext.Frm);

            SM.FrmLogOn.Show();
        }

        private void btHome_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ForContext.Frm);

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

            pnLDR.Refresh();
            pnPKR.Refresh();
            pnSUT.Refresh();
            pnSYR.Refresh();
            pnCHA.Refresh();
            pnFRG.Refresh();
            pnBFR.Refresh();
            pnRPK.Refresh();
            pnLPK.Refresh();














        }


        int iPreErrCnt  = 0;
        //Random Ran = new Random();
        //int iRan = Ran.Next(10, 100);
        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            

            if (SEQ.CHA.GetSeqStep() == (int)Chamber.sc.FillTank    ) lbWorkInfo.Text = "탱크 채우는 중" ;
            else if (SEQ.CHA.GetSeqStep() == (int)Chamber.sc.FillChamber ) lbWorkInfo.Text = "챔버 채우는 중" ;
            else if (SEQ.CHA.GetSeqStep() == (int)Chamber.sc.InspChamber ) 
            {
                string sTemp = "챔버 검사 중";
                string sSub  = "" ;
                if(SEQ.CHA.GetStep().iCycleSubDC  > 10) sSub += "DC "  ;
                if(SEQ.CHA.GetStep().iCycleSubFcm > 10) 
                {
                         if(SEQ.CHA.GetStep().iCycleSubFcm / 100 == 3) sSub += "FCM(FB) "  ;
                    else if(SEQ.CHA.GetStep().iCycleSubFcm / 100 == 4) sSub += "FCM(4DLS) ";
                    else if(SEQ.CHA.GetStep().iCycleSubFcm / 100 == 5) sSub += "FCM(RET) " ;
                    else if(SEQ.CHA.GetStep().iCycleSubFcm / 100 == 6) sSub += "FCM(NR) "  ;
                }
                if(SEQ.CHA.GetStep().iCycleSubHgb > 10) sSub += "HGB " ;
                lbWorkInfo.Text = sTemp + "-" + sSub.Trim();

            }
            else if (SEQ.CHA.GetSeqStep() == (int)Chamber.sc.EmptyChamber) lbWorkInfo.Text = "챔버 비우는 중" ;
            else if (SEQ.CHA.GetSeqStep() == (int)Chamber.sc.CleanChamber) lbWorkInfo.Text = "챔버 닦는 중"   ;
            else lbWorkInfo.Text = "";

            



            btWaitStop.ForeColor = OM.EqpStat.bWorkOneStop ? (SEQ._bFlick ? Color.Red : Color.White ) : Color.White ;

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
                      
            lvLotInfo.Items.Clear();
            SPC.LOT.DispLotInfo(lvLotInfo);
            SPC.DAY.DispDayInfo(lvLotInfo);

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

            if(!ML.MT_GetHomeDoneAll()){
                btHome.ForeColor = SEQ._bFlick ? Color.White : Color.Red;
            }
            else {
                btHome.ForeColor = Color.White  ;
            }

            pnValves.Invalidate();

            
            pbInsp.Value = OM.EqpStat.iInspProgress ;
            pbInsp.Visible = pbInsp.Value != 0 ;
        }


        public void MakeDoubleBuffered(Control control, bool setting)
        {
            Type controlType = control.GetType();
            PropertyInfo pi = controlType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(control, setting, null);
        }

        private void button34_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ForContext.Frm);

            if (SEQ._bRun) return;

            const int iBreakAdd = 3 ;
            //bool bBreakStat = ML.  MT_GetY(mi.LODR_ZClmp , iBreakAdd) ;
            //ML.MT_SetY(mi.LODR_ZClmp , iBreakAdd , !bBreakStat);

        }

        private void btULDBreakOnOff_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ForContext.Frm);

            if (SEQ._bRun) return;

            const int iBreakAdd = 3 ;
            //bool bBreakStat = ML.  MT_GetY(mi.ULDR_ZClmp , iBreakAdd) ;
            //ML.MT_SetY(mi.ULDR_ZClmp , iBreakAdd , !bBreakStat);

        }

        private void s1_DoubleClick(object sender, EventArgs e)
        {
            if (Log.ShowMessageModal("Confirm", "Do you want to remove the strip?") != DialogResult.Yes) return;
            string sTag = ((PictureBox)sender).Tag.ToString();

            //if (sTag == "1") DM.ARAY[ri.PREB].SetStat(cs.None);
            //if (sTag == "2") DM.ARAY[ri.VSN1].SetStat(cs.None);
            //if (sTag == "3") DM.ARAY[ri.VSN2].SetStat(cs.None);
            //if (sTag == "4") DM.ARAY[ri.VSN3].SetStat(cs.None);
            //if (sTag == "5") DM.ARAY[ri.PSTB].SetStat(cs.None);
        }



        private void cbQC_CheckedChanged(object sender, EventArgs e)
        {
            OM.CmnOptn.bAutoQc = cbQC.Checked ; 

            if(cbQC.Checked )
            {
                Log.ShowMessage("INFO" , "오토QC모드 진입 - 냉동고에 샘플3개를 꼭 넣어주세요.");
                
                Part.IO_SetY(yi.ETC_MainPumpOff , true );
                DM.ARAY[ri.LDR].SetStat(cs.None);
                DM.ARAY[ri.PKR].SetStat(cs.None);
                DM.ARAY[ri.SUT].SetStat(cs.None);
                DM.ARAY[ri.SYR].SetStat(cs.None);
                DM.ARAY[ri.CHA].SetStat(cs.None);
                DM.ARAY[ri.FRG].SetStat(cs.Freeze);
                DM.ARAY[ri.BFR].SetStat(cs.None);
                DM.ARAY[ri.RPK].SetStat(cs.None); 
                DM.ARAY[ri.LPK].SetStat(cs.None);

            }
            else 
            {
                Log.ShowMessage("INFO" , "검사모드 진입");
                Part.IO_SetY(yi.ETC_MainPumpOff , false );
            }

            OM.SaveCmnOptn();
        }

        //메뉴얼.
        //==========================================================================================================
        private void button2_Click(object sender, EventArgs e)
        {
            fb FwdBwd = ML.CL_GetCmd(ci.CHA_MixCoverOpCl) == fb.Fwd ? fb.Bwd : fb.Fwd ;
            ML.CL_Move(ci.CHA_MixCoverOpCl, FwdBwd );
        }

        private void btManual_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            //Log.Trace(sFormText + sText + " Button Clicked", ForContext.Frm);

            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);
            MM.SetManCycle((mc)iBtnTag);
        }

        private void btTankEmpty_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button ;    
            if(btn == null)return ;//fghfgh

            //Log.Trace(sFormText + sText + " Button Clicked", ForContext.Frm);

            int iTag = Convert.ToInt32(btn.Tag);

            if (iTag == 0)
            {
                SEQ.CHA.TankCP1toFCMWST(btn.BackColor != Color.Aqua);
                btn.BackColor = btTankCP1toFCMWST.BackColor != Color.Aqua ? Color.Aqua : Color.LightGray; 
            }
            else if(iTag == 1)
            {
                SEQ.CHA.FCMWSTtoMAINWST(btn.BackColor != Color.Aqua);
                btn.BackColor = btn.BackColor != Color.Aqua ? Color.Aqua : Color.LightGray;
            }
            else if(iTag == 2)
            {
                SEQ.CHA.MainWtoExtW(btn.BackColor != Color.Aqua);
                btn.BackColor = btn.BackColor != Color.Aqua ? Color.Aqua : Color.LightGray;
            }
            else if(iTag == 10)
            {
                SEQ.CHA.TankCP2toMainWST(btn.BackColor != Color.Aqua);
                btn.BackColor = btn.BackColor != Color.Aqua ? Color.Aqua : Color.LightGray;
            
            }
            else if(iTag == 20)
            {
                
                SEQ.CHA.TankCP3toMainWST(btn.BackColor != Color.Aqua);
                btn.BackColor = btn.BackColor != Color.Aqua ? Color.Aqua : Color.LightGray;
            }
            else if(iTag == 30)
            {
                
                SEQ.CHA.TankCSFtoMainWST(btn.BackColor != Color.Aqua);
                btn.BackColor = btn.BackColor != Color.Aqua ? Color.Aqua : Color.LightGray;
            }
            else if(iTag == 40)
            {
                
                SEQ.CHA.TankCSRtoMainWST(btn.BackColor != Color.Aqua);
                btn.BackColor = btn.BackColor != Color.Aqua ? Color.Aqua : Color.LightGray;
            }
            else if(iTag == 50)
            {
                
                SEQ.CHA.TankSULFtoMainWST(btn.BackColor != Color.Aqua);
                btn.BackColor = btn.BackColor != Color.Aqua ? Color.Aqua : Color.LightGray;
            }
            else if(iTag == 60)
            {
                
                SEQ.CHA.TankFBtoMainWST(btn.BackColor != Color.Aqua);
                btn.BackColor = btn.BackColor != Color.Aqua ? Color.Aqua : Color.LightGray;
            }
            else if(iTag == 70)
            {
                
                SEQ.CHA.Tank4DLtoMainWST(btn.BackColor != Color.Aqua);
                btn.BackColor = btn.BackColor != Color.Aqua ? Color.Aqua : Color.LightGray;
            }
            else if(iTag == 80)
            {
                
                SEQ.CHA.TankRETtoMainWST(btn.BackColor != Color.Aqua);
                btn.BackColor = btn.BackColor != Color.Aqua ? Color.Aqua : Color.LightGray;
            }
            else if(iTag == 90)
            {
                
                SEQ.CHA.TankNRtoMainWST(btn.BackColor != Color.Aqua);
                btn.BackColor = btn.BackColor != Color.Aqua ? Color.Aqua : Color.LightGray;
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SEQ.CHA.InitValve();
        }

        private void pnValves_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            ValveDisp.Paint(g);
        }

        private void pnValves_MouseMove(object sender, MouseEventArgs e)
        {
            //lbMousePoint.Text = e.X.ToString() + " : " + e.Y.ToString() + " W:" + Width.ToString() + " H:" + Height.ToString();

            string sTip = ValveDisp.Move(e.X, e.Y) ;

            if(sTip != "")
            {
                ttValve.Active = true ;
                ttValve.SetToolTip(pnValves , sTip);
            }
            else
            {
                 ttValve.Active = false ;
            }
        }

        private void pnValves_MouseClick(object sender, MouseEventArgs e)
        {
            ValveDisp.Click(e.X, e.Y);
        }

        private void btWaitStop_Click(object sender, EventArgs e)
        {
            OM.EqpStat.bWorkOneStop = !OM.EqpStat.bWorkOneStop;
        }


        private void metroTile7_Click(object sender, EventArgs e)
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