using System;
using System.Drawing;
using System.Windows.Forms;
using COMMON;
using SML;
using System.Reflection;
using System.Threading;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Machine
{
    public partial class FormOperation : Form
    {
        //public static Recorder      Recorder;
        public static FormOperation FrmOperation;
        public static FormLotOpen   FrmLotOpen;
        //public static FormMain FrmMain;

        public int LotInfoCnt = 8;
        public int DayInfoCnt = 6;        

        protected CDelayTimer m_tmStartBt ;

        private const string sFormText = "Form Operation ";
        //public EN_SEQ_STAT m_iSeqStat;

        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern System.IntPtr CreateRoundRectRgn
        (
             int nLeftRect, // x-coordinate of upper-left corner
             int nTopRect, // y-coordinate of upper-left corner
             int nRightRect, // x-coordinate of lower-right corner
             int nBottomRect, // y-coordinate of lower-right corner
             int nWidthEllipse, // height of ellipse
             int nHeightEllipse // width of ellipse
        );

        public FormOperation(Panel _pnBase)
        {
            InitializeComponent();

            //Recorder = new Recorder();

            this.TopLevel = false;
            this.Parent = _pnBase;

            pnImg.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, pnImg.Width, pnImg.Height, 15, 15));
            //var path = new System.Drawing.Drawing2D.GraphicsPath();
            //path.aAddEllipse(0, 0, pnImg.Width, pnImg.Height);
            //pnImg.Region = new Region(path);

            //DispDayList();
            DispLotInfo();

            //MakeDoubleBuffered(groupBox5,true);
            //MakeDoubleBuffered(7/groupBox4, true);
            MakeDoubleBuffered(groupBox3,true);
            //MakeDoubleBuffered(groupBox2,true);
            //MakeDoubleBuffered(groupBox1,true);

            MakeDoubleBuffered(pnImg, true);

            MakeDoubleBuffered(pnI1, true);
            MakeDoubleBuffered(pnI2, true);
            MakeDoubleBuffered(pnI3, true);
            MakeDoubleBuffered(pnI4, true);
            MakeDoubleBuffered(pnS1, true);
            MakeDoubleBuffered(pnS2, true);
            MakeDoubleBuffered(pnS3, true);
            MakeDoubleBuffered(pnS4, true);
            MakeDoubleBuffered(pnS5, true);
            //tmUpdate.Enabled = true;

            //btStart.Enabled = LOT.GetLotOpen();

            m_tmStartBt = new CDelayTimer();

            DM.ARAY[ri.LODR].SetParent(pnLODR); DM.ARAY[ri.LODR].Name = "LODR";
            DM.ARAY[ri.INDX].SetParent(pnINDX); DM.ARAY[ri.INDX].Name = "INDX";
            
            //Loader           
            DM.ARAY[ri.LODR].SetDisp(cs.None     , "None"     , Color.White        );
            DM.ARAY[ri.LODR].SetDisp(cs.Unknown  , "UnKnown"  , Color.Aqua         );
            DM.ARAY[ri.LODR].SetDisp(cs.Mask     , "Mask"     , Color.SkyBlue      );
            DM.ARAY[ri.LODR].SetDisp(cs.Empty    , "Empty"    , Color.Silver       ); 
            DM.ARAY[ri.LODR].SetDisp(cs.WorkEnd  , "WorkEnd"  , Color.Blue         );

            //Dressy Index 
            DM.ARAY[ri.INDX].SetDisp(cs.None     , "None"     , Color.White        );
            DM.ARAY[ri.INDX].SetDisp(cs.Empty    , "Empty"    , Color.Silver       );
            DM.ARAY[ri.INDX].SetDisp(cs.Barcode  , "Barcode"  , Color.Tan          );
            DM.ARAY[ri.INDX].SetDisp(cs.Unknown  , "UnKnown"  , Color.Aqua         );
            DM.ARAY[ri.INDX].SetDisp(cs.Ready    , "Ready  "  , Color.Yellow       );
            DM.ARAY[ri.INDX].SetDisp(cs.Work     , "Work   "  , Color.Lime         );
            DM.ARAY[ri.INDX].SetDisp(cs.Analyze  , "Analyze"  , Color.LightCoral   );
            DM.ARAY[ri.INDX].SetDisp(cs.Check    , "Check  "  , Color.MistyRose    );
            DM.ARAY[ri.INDX].SetDisp(cs.WorkEnd  , "WorkEnd"  , Color.Blue         );

            //EzSensor Index
            DM.ARAY[ri.INDX].SetDisp(cs.None           , "None"          , Color.White        );
            DM.ARAY[ri.INDX].SetDisp(cs.Empty          , "Empty"         , Color.Silver       );
            DM.ARAY[ri.INDX].SetDisp(cs.Barcode        , "Barcode"       , Color.Tan          );
            DM.ARAY[ri.INDX].SetDisp(cs.Unknown        , "UnKnown"       , Color.Aqua         );
            DM.ARAY[ri.INDX].SetDisp(cs.Entering1x1    , "1x1Entering"   , Color.Yellow       );
            DM.ARAY[ri.INDX].SetDisp(cs.Aging1x1       , "1x1Aging"      , Color.Lime         );
            DM.ARAY[ri.INDX].SetDisp(cs.MTFNPS1x1      , "1x1MTF/NPS"    , Color.LightCoral   );
            DM.ARAY[ri.INDX].SetDisp(cs.Calibration1x1 , "1x1Calibration", Color.MistyRose    );
            DM.ARAY[ri.INDX].SetDisp(cs.Skull1x1       , "1x1Skull"      , Color.DarkOrange   );
            DM.ARAY[ri.INDX].SetDisp(cs.Entering2x2    , "2x2Entering"   , Color.YellowGreen  );
            DM.ARAY[ri.INDX].SetDisp(cs.Aging2x2       , "2x2Aging"      , Color.LimeGreen    );
            DM.ARAY[ri.INDX].SetDisp(cs.MTFNPS2x2      , "2x2MTF/NPS"    , Color.IndianRed    );
            DM.ARAY[ri.INDX].SetDisp(cs.Calibration2x2 , "2x2Calibration", Color.SaddleBrown  );
            DM.ARAY[ri.INDX].SetDisp(cs.Skull2x2       , "2x2Skull"      , Color.Olive        );
            DM.ARAY[ri.INDX].SetDisp(cs.WorkEnd        , "WorkEnd"       , Color.Blue         );
            
            if (OM.DevInfo.iMacroType == 0)
            {
                //EzSensor Index
                DM.ARAY[ri.INDX].SetVisible(cs.Entering1x1    , false);
                DM.ARAY[ri.INDX].SetVisible(cs.Aging1x1       , false);
                DM.ARAY[ri.INDX].SetVisible(cs.MTFNPS1x1      , false);
                DM.ARAY[ri.INDX].SetVisible(cs.Calibration1x1 , false);
                DM.ARAY[ri.INDX].SetVisible(cs.Skull1x1       , false);
                DM.ARAY[ri.INDX].SetVisible(cs.Entering2x2    , false);
                DM.ARAY[ri.INDX].SetVisible(cs.Aging2x2       , false);
                DM.ARAY[ri.INDX].SetVisible(cs.MTFNPS2x2      , false);
                DM.ARAY[ri.INDX].SetVisible(cs.Calibration2x2 , false);
                DM.ARAY[ri.INDX].SetVisible(cs.Skull2x2       , false);
                
                //Dressy Index 
                DM.ARAY[ri.INDX].SetVisible(cs.Ready    , true);
                DM.ARAY[ri.INDX].SetVisible(cs.Work     , true);
                DM.ARAY[ri.INDX].SetVisible(cs.Analyze  , true);
                DM.ARAY[ri.INDX].SetVisible(cs.Check    , true);
           
            }
            else if (OM.DevInfo.iMacroType == 1)
            {
                //Dressy Index 
                DM.ARAY[ri.INDX].SetVisible(cs.Ready    , false);
                DM.ARAY[ri.INDX].SetVisible(cs.Work     , false);
                DM.ARAY[ri.INDX].SetVisible(cs.Analyze  , false);
                DM.ARAY[ri.INDX].SetVisible(cs.Check    , false);
                
                //EzSensor Index
                DM.ARAY[ri.INDX].SetVisible(cs.Entering1x1    , true);
                DM.ARAY[ri.INDX].SetVisible(cs.Aging1x1       , true);
                DM.ARAY[ri.INDX].SetVisible(cs.MTFNPS1x1      , true);
                DM.ARAY[ri.INDX].SetVisible(cs.Calibration1x1 , true);
                DM.ARAY[ri.INDX].SetVisible(cs.Skull1x1       , true);
                DM.ARAY[ri.INDX].SetVisible(cs.Entering2x2    , true);
                DM.ARAY[ri.INDX].SetVisible(cs.Aging2x2       , true);
                DM.ARAY[ri.INDX].SetVisible(cs.MTFNPS2x2      , true);
                DM.ARAY[ri.INDX].SetVisible(cs.Calibration2x2 , true);
                DM.ARAY[ri.INDX].SetVisible(cs.Skull2x2       , true);
            }
          
            DM.ARAY[ri.LODR].SetMaxColRow(1                        , OM.DevInfo.iLODR_SlotCnt    );
            DM.ARAY[ri.INDX].SetMaxColRow(OM.DevInfo.iTRAY_PcktCntX, 1                           );

            DM.LoadMap();

            //tbUpSide.TabPages.Remove(tbUpside1);//Chart 탭 제거

            tbAmp .Text = OM.EqpStat.dAmp .ToString();
            tbVolt.Text = OM.EqpStat.iVolt.ToString();
            tbTime.Text = OM.EqpStat.dTime.ToString();
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
            DM.ARAY[ri.LODR].SetStat(cs.None);
            DM.ARAY[ri.INDX].SetStat(cs.None);
            
            btStart.Enabled = false;
        }


        private static bool bPreLotOpen = false;
        private static int iTrayImg = 0;

        [DllImport("Kernel32.dll")]
        public static extern bool IsWow64Process(System.IntPtr hProcess, out bool lpSystemInfo);


        private void timer1_Tick(object sender, EventArgs e)
        {
            tmUpdate.Enabled = false;

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
                btOperator  .Text    = "  LOG IN";
                pnLODR      .Enabled = false     ;
                pnImg       .Enabled = false     ;
                pnLotInfo   .Enabled = false     ;
                pnError     .Enabled = false     ;
                pnOperMan   .Enabled = false     ;
                pnLotOpen   .Enabled = false     ;
                btStart     .Enabled = false     ;
                btStop      .Enabled = false     ;
                btReset     .Enabled = false     ;
                btLightOnOff.Enabled = false     ;
                btOperator  .Enabled = true      ;
                panel4      .Enabled = false     ;
            }
            else
            {
                btOperator  .Text    = "  " + SM.FrmLogOn.GetLevel().ToString();
                pnLODR      .Enabled = true                                    ;
                pnImg       .Enabled = true                                    ;
                pnLotInfo   .Enabled = true                                    ;
                pnError     .Enabled = true                                    ;
                pnOperMan   .Enabled = true                                    ;
                pnLotOpen   .Enabled = true                                    ;
                btStop      .Enabled = true                                    ;
                btReset     .Enabled = true                                    ;
                btLightOnOff.Enabled = true                                    ;
                panel4      .Enabled = true                                    ;
                btStart     .Enabled =  LOT.GetLotOpen()                       ;
                btLotEnd    .Enabled =  LOT.GetLotOpen()                       ;
                btLotOpen   .Enabled = !LOT.GetLotOpen()                       ;
            }

            if (SEQ._iSeqStat == EN_SEQ_STAT.Running)
            {
                btLotEnd.Enabled = false;
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

          
            string sCycleTimeSec ;
            int iCycleTimeMs ;
            
            if(!ML.MT_GetHomeDoneAll()){
                btAllHome.ForeColor = SEQ._bFlick ? Color.Black : Color.Red;
            }
            else {
                btAllHome.ForeColor = Color.Black  ;
            }

            SPC.LOT.DispLotInfo(lvLotInfo);

            pnLODR.Refresh();
            pnINDX.Refresh();
            //Refresh();

            if (!this.Visible)
            {
                tmUpdate.Enabled = false;
                return;
            }
           
            //필터 업다운 오퍼레이션 창에서 확인
            if (!ML.IO_GetX(xi.XRAY_Filter1Dn))
            {
                lbFilter1.BackColor = Color.Red;
            }
            else if (ML.IO_GetX(xi.XRAY_Filter1Dn))
            {
                lbFilter1.BackColor = Color.Lime;
            }

            if (!ML.IO_GetX(xi.XRAY_Filter2Dn)) 
            {
               
                lbFilter2.BackColor = Color.Red ;
            }
            else if (ML.IO_GetX(xi.XRAY_Filter2Dn))
            {
               
                lbFilter2.BackColor = Color.Lime;
            }
        
            if (!ML.IO_GetX(xi.XRAY_Filter3Dn))
            {
              
                lbFilter3.BackColor = Color.Red ;
            }
            else if (ML.IO_GetX(xi.XRAY_Filter3Dn))
            {
               
                lbFilter3.BackColor = Color.Lime;
            }
            
            if (!ML.IO_GetX(xi.XRAY_Filter4Dn))
            {
                lbFilter4.BackColor = Color.Red ;
            }
            else if (ML.IO_GetX(xi.XRAY_Filter4Dn))
            {
                lbFilter4.BackColor = Color.Lime;
            }
            
            if (!ML.IO_GetX(xi.XRAY_Filter5Dn))
            {
                lbFilter5.BackColor = Color.Red ;
            }
            else if (ML.IO_GetX(xi.XRAY_Filter5Dn))
            {
                lbFilter5.BackColor = Color.Lime;
            }
            
            if (!ML.IO_GetX(xi.XRAY_Filter6Dn))
            {
                lbFilter6.BackColor = Color.Red ;
            }
            else if (ML.IO_GetX(xi.XRAY_Filter6Dn))
            {
                lbFilter6.BackColor = Color.Lime;
            }
            
            if (!ML.IO_GetX(xi.XRAY_Filter7Dn))
            {
                lbFilter7.BackColor = Color.Red ;
            }
            else if (ML.IO_GetX(xi.XRAY_Filter7Dn))
            {
                lbFilter7.BackColor = Color.Lime;
            }

            //USB 연결
                 if (ML.IO_GetX(xi.XRAY_LeftUSBFw)) pnLeftUSB.BackColor = Color.Lime;
            else if (ML.IO_GetX(xi.XRAY_LeftUSBBw)) pnLeftUSB.BackColor = Color.Red;

  
                 if (ML.IO_GetX(xi.XRAY_RightUSBFw)) pnRightUSB.BackColor = Color.Lime;
            else if (ML.IO_GetX(xi.XRAY_RightUSBBw)) pnRightUSB.BackColor = Color.Red;

            if (SEQ.XRYD.bLeftCnct || SEQ.XRYD.bRightCnct ||
                SEQ.XRYE.bLeftCnct || SEQ.XRYE.bRightCnct)
            {
                lbCnct.BackColor = Color.Aqua;
                lbCnct.Text = "O";
            }
            else if ((!SEQ.XRYD.bLeftCnct && !SEQ.XRYD.bRightCnct) ||
                     (!SEQ.XRYD.bLeftCnct && !SEQ.XRYD.bRightCnct))
            {
                lbCnct.BackColor = Color.DarkGray;
                lbCnct.Text = "X";
            }

            if(OM.DevInfo.iMacroType == 0) 
            {
                lbWorkNo.Text = (SEQ.XRYD.iWorkStep + 1).ToString();
                lbVolt.Text = OM.Dressy[OM.EqpStat.iLastWorkStep].dXKvp .ToString();
                lbAmp .Text = OM.Dressy[OM.EqpStat.iLastWorkStep].dXmA  .ToString();
                lbTime.Text = OM.Dressy[OM.EqpStat.iLastWorkStep].dXTime.ToString();
            } 
            else if(OM.DevInfo.iMacroType == 1) 
            {
                lbWorkNo.Text = (SEQ.XRYE.iWorkStep + 1).ToString();
                lbVolt.Text = SEQ.XRYE.MacroSet.sKvp ;
                lbAmp .Text = SEQ.XRYE.MacroSet.smA  ;
                lbTime.Text = SEQ.XRYE.MacroSet.sTime;
            }
            

            
            lbDeviceName.Text = OM.GetCrntDev();

            if(OM.DevInfo.iMacroType == 0)
            {
                lbRepeat.Text = SEQ.Mcr.Dr1.iRptCnt.ToString();
                panel4 .Visible = false;
                panel13.Visible = true ;

                switch (SEQ.XRYD.GetSeqStep())
                {
                    default: break;
                    case 0:
                        pnS1.BackColor = SystemColors.ControlDark;
                        pnS2.BackColor = SystemColors.ControlDark;
                        pnS3.BackColor = SystemColors.ControlDark;
                        pnS4.BackColor = SystemColors.ControlDark;
                        pnS5.BackColor = SystemColors.ControlDark;
                        break;
                    case 1:
                        pnS1.BackColor = SEQ._bFlick ? Color.DarkKhaki : SystemColors.ControlDark;
                        pnS2.BackColor = SystemColors.ControlDark;
                        pnS3.BackColor = SystemColors.ControlDark;
                        pnS4.BackColor = SystemColors.ControlDark;
                        pnS5.BackColor = SystemColors.ControlDark;
                        break;
                    case 2:
                        pnS1.BackColor = Color.DarkKhaki;
                        pnS2.BackColor = SEQ._bFlick ? Color.DarkKhaki : SystemColors.ControlDark;
                        pnS3.BackColor = SystemColors.ControlDark;
                        pnS4.BackColor = SystemColors.ControlDark;
                        pnS5.BackColor = SystemColors.ControlDark;
                        break;
                    case 3:
                        pnS1.BackColor = Color.DarkKhaki;
                        pnS2.BackColor = Color.DarkKhaki;
                        pnS3.BackColor = SEQ._bFlick ? Color.DarkKhaki : SystemColors.ControlDark;
                        pnS4.BackColor = SystemColors.ControlDark;
                        pnS5.BackColor = SystemColors.ControlDark;
                        break;
                    case 4:
                        pnS1.BackColor = Color.DarkKhaki;
                        pnS2.BackColor = Color.DarkKhaki;
                        pnS3.BackColor = Color.DarkKhaki;
                        pnS4.BackColor = SEQ._bFlick ? Color.DarkKhaki : SystemColors.ControlDark;
                        pnS5.BackColor = SystemColors.ControlDark;
                        break;
                    case 5:
                        pnS1.BackColor = Color.DarkKhaki;
                        pnS2.BackColor = Color.DarkKhaki;
                        pnS3.BackColor = Color.DarkKhaki;
                        pnS4.BackColor = Color.DarkKhaki;
                        pnS5.BackColor = SEQ._bFlick ? Color.DarkKhaki : SystemColors.ControlDark;
                        break;
                }
            }
            else if (OM.DevInfo.iMacroType == 1)
            {
                lbRepeat.Text = SEQ.Mcr.Ez1.iRptCnt.ToString();
                panel4 .Visible = true ;
                panel13.Visible = false;

                switch (SEQ.XRYE.GetSeqStep())
                {
                    default: break;
                    case 0:
                        pnE1.BackColor = SystemColors.ControlDark;
                        pnE2.BackColor = SystemColors.ControlDark;
                        pnE3.BackColor = SystemColors.ControlDark;
                        pnE4.BackColor = SystemColors.ControlDark;
                        pnE5.BackColor = SystemColors.ControlDark;
                        pnE6.BackColor = SystemColors.ControlDark;
                        break;
                    case 1:
                        pnE1.BackColor = SEQ._bFlick ? Color.DarkKhaki : SystemColors.ControlDark;
                        pnE2.BackColor = SystemColors.ControlDark;
                        pnE3.BackColor = SystemColors.ControlDark;
                        pnE4.BackColor = SystemColors.ControlDark;
                        pnE5.BackColor = SystemColors.ControlDark;
                        pnE6.BackColor = SystemColors.ControlDark;
                        break;
                    case 2:
                        pnE1.BackColor = Color.DarkKhaki;
                        pnE2.BackColor = SEQ._bFlick ? Color.DarkKhaki : SystemColors.ControlDark;
                        pnE3.BackColor = SystemColors.ControlDark;
                        pnE4.BackColor = SystemColors.ControlDark;
                        pnE5.BackColor = SystemColors.ControlDark;
                        pnE6.BackColor = SystemColors.ControlDark;
                        break;
                    case 3:
                        pnE1.BackColor = Color.DarkKhaki;
                        pnE2.BackColor = Color.DarkKhaki;
                        pnE3.BackColor = SEQ._bFlick ? Color.DarkKhaki : SystemColors.ControlDark;
                        pnE4.BackColor = SystemColors.ControlDark;
                        pnE5.BackColor = SystemColors.ControlDark;
                        pnE6.BackColor = SystemColors.ControlDark;
                        break;
                    case 4:
                        pnE1.BackColor = Color.DarkKhaki;
                        pnE2.BackColor = Color.DarkKhaki;
                        pnE3.BackColor = Color.DarkKhaki;
                        pnE4.BackColor = SEQ._bFlick ? Color.DarkKhaki : SystemColors.ControlDark;
                        pnE5.BackColor = SystemColors.ControlDark;
                        pnE6.BackColor = SystemColors.ControlDark;
                        break;
                    case 5:
                        pnE1.BackColor = Color.DarkKhaki;
                        pnE2.BackColor = Color.DarkKhaki;
                        pnE3.BackColor = Color.DarkKhaki;
                        pnE4.BackColor = Color.DarkKhaki;
                        pnE5.BackColor = SEQ._bFlick ? Color.DarkKhaki : SystemColors.ControlDark;
                        pnE6.BackColor = SystemColors.ControlDark;
                        break;
                    case 6:
                        pnE1.BackColor = Color.DarkKhaki;
                        pnE2.BackColor = Color.DarkKhaki;
                        pnE3.BackColor = Color.DarkKhaki;
                        pnE4.BackColor = Color.DarkKhaki;
                        pnE5.BackColor = Color.DarkKhaki;
                        pnE6.BackColor = SEQ._bFlick ? Color.DarkKhaki : SystemColors.ControlDark;
                        break;
                }
            }
            

            

            pnD1.Visible = ML.IO_GetX(xi.INDX_DoorOp );
            pnD2.Visible = !ML.IO_GetX(xi.ETC_RrInDoor);
            pnD3.Visible = !ML.IO_GetX(xi.ETC_FtInDoor);
            pnD4.Visible = !ML.IO_GetX(xi.ETC_RtInDoor);

            if (ML.IO_GetX(xi.INDX_DoorCl))  { pnC2.BackColor = Color.Silver; lbC2.Text = "CLOSE"; }
            else                             { pnC2.BackColor = Color.Red   ; lbC2.Text = "OPEN" ; }
            if (ML.IO_GetX(xi.ETC_RrInDoor)) { pnC4.BackColor = Color.Silver; lbC4.Text = "CLOSE"; }
            else                             { pnC4.BackColor = Color.Red   ; lbC4.Text = "OPEN" ; }
            if (ML.IO_GetX(xi.ETC_FtInDoor)) { pnC1.BackColor = Color.Silver; lbC1.Text = "CLOSE"; }
            else                             { pnC1.BackColor = Color.Red   ; lbC1.Text = "OPEN" ; }
            if (ML.IO_GetX(xi.ETC_RtInDoor)) { pnC3.BackColor = Color.Silver; lbC3.Text = "CLOSE"; }
            else                             { pnC3.BackColor = Color.Red   ; lbC3.Text = "OPEN" ; }

            if (!ML.IO_GetY(yi.ETC_LightOnOff))
            {
                button12.Image = global::Machine.Properties.Resources.LightOff;
            }
            else
            {
                button12.Image = global::Machine.Properties.Resources.LightOn;
            }  
  
            if(ML.MT_GetCmdPos(mi.XRAY_ZXRay) == ML.PM_GetValue(mi.XRAY_ZXRay, pv.XRAY_ZXRayWait))
            {
                lbXrayZAxis.Text = "안전";
                lbXrayZAxis.BackColor = Color.Lime;
            }

            else if (ML.MT_GetCmdPos(mi.XRAY_ZXRay) <= ML.PM_GetValue(mi.XRAY_ZXRay, pv.XRAY_ZXRayFltrMove))
            {
                lbXrayZAxis.Text = "안전";
                lbXrayZAxis.BackColor = Color.Lime;
            }

            else if (ML.MT_GetCmdPos(mi.XRAY_ZXRay) >= ML.PM_GetValue(mi.XRAY_ZXRay, pv.XRAY_ZXRayWork) ||
                     (ML.MT_GetCmdPos(mi.XRAY_ZXRay) <= ML.PM_GetValue(mi.XRAY_ZXRay, pv.XRAY_ZXRayWork) &&
                      ML.MT_GetCmdPos(mi.XRAY_ZXRay) >= ML.PM_GetValue(mi.XRAY_ZXRay, pv.XRAY_ZXRayFltrMove)))
            {
                lbXrayZAxis.Text = "위험";
                lbXrayZAxis.BackColor = Color.Red;
            }

            if (bUnlock)
            {
                btUnlock.Image = global::Machine.Properties.Resources.Unlock32;
            }
            else
            {
                btUnlock.Image = global::Machine.Properties.Resources.Lock32;
            }

            lbSerial.Text = OM.EqpStat.sSerialList;

            if(OM.CmnOptn.bSkipEntr ) 
            {
                pnE2.BackColor = SystemColors.ControlDark;
                lbE2.Text = "입고 공정 SKIP";
            }
            if(OM.CmnOptn.bSkipAging) 
            {
                pnE3.BackColor = SystemColors.ControlDark;
                lbE3.Text = "Aging 공정 SKIP";
            }
            if(OM.CmnOptn.bSkipMTF  ) 
            {
                pnE4.BackColor = SystemColors.ControlDark;
                lbE4.Text = "MTF/NPS 공정 SKIP";
            }
            if(OM.CmnOptn.bSkipCalib) 
            {
                pnE5.BackColor = SystemColors.ControlDark;
                lbE5.Text = "Calibration 공정 SKIP";
            }
            if(OM.CmnOptn.bSkipSkull) 
            {
                pnE6.BackColor = SystemColors.ControlDark;
                lbE6.Text = "Skull 공정 SKIP";
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
            lvLotInfo.Columns.Add("", 115, HorizontalAlignment.Left);     //1280*1024는 109, 1024*768은 78
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
        //
        //    lvDayInfo.Columns.Add("", 105, HorizontalAlignment.Left);    //1280*1024는 109, 1024*768은 78
        //    lvDayInfo.Columns.Add("", 105, HorizontalAlignment.Left);    //1280*1024는 109, 1024*768은 78
        //
        //    ListViewItem[] liDayInfo = new ListViewItem[DayInfoCnt];
        //
        //    for (int i = 0; i < DayInfoCnt; i++)
        //    {
        //        liDayInfo[i] = new ListViewItem();
        //        liDayInfo[i].SubItems.Add("");
        //
        //
        //        liDayInfo[i].UseItemStyleForSubItems = false;
        //        liDayInfo[i].UseItemStyleForSubItems = false;
        //
        //        lvDayInfo.Items.Add(liDayInfo[i]);
        //
        //    }
        //
        //    var PropDayInfo = lvDayInfo.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
        //    PropDayInfo.SetValue(lvDayInfo, true, null);
        //
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
            DM.SaveMap();
        }

        private void btManual1_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);
            MM.SetManCycle((mc)iBtnTag);
        }
		
        private void btFwd_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);
            if (SEQ.INDX.CheckSafe((ci)iBtnTag, fb.Fwd, true) &&
                SEQ.XRYD.CheckSafe((ci)iBtnTag, fb.Fwd, true))
            {
                ML.CL_Move((ci)iBtnTag, fb.Fwd);
            }
            
        }

        private void btBwd_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);
            if (SEQ.INDX.CheckSafe((ci)iBtnTag, fb.Fwd, true) &&
                SEQ.XRYD.CheckSafe((ci)iBtnTag, fb.Fwd, true))
            {
                ML.CL_Move((ci)iBtnTag, fb.Bwd);
            }
        }

        private void btCylinder4_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);
            if (SEQ.INDX.CheckSafe((ci)iBtnTag, fb.Fwd, true) &&
                SEQ.XRYD.CheckSafe((ci)iBtnTag, fb.Fwd, true))
            {
                ML.CL_Move((ci)iBtnTag, ML.CL_GetCmd((ci)iBtnTag) == fb.Bwd ? fb.Fwd : fb.Bwd);
            }
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
            DM.ARAY[ri.LODR].SetStat(cs.None);
            DM.ARAY[ri.INDX].SetStat(cs.None);

            btStart.Enabled = false;
        }

        private void btStart_Click_1(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;

            //if (OM.CmnOptn.bUseRecord)
            //{
            //    SEQ.ScreenRecord.StartRecording();
            //}

            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            SEQ._bBtnStart = true;
            bUnlock = false;
        }

        private void btStop_Click_1(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);
            SEQ.Mcr.Stop();
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
            if(!ML.IO_GetY(yi.ETC_MainAirSol) && !OM.MstOptn.bDebugMode)
            {
                ML.ER_SetErr(ei.ETC_MainAir, "메인 에어를 ON 해주세요.");
                return;
            }
            
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
            if (SEQ._iSeqStat == EN_SEQ_STAT.Running)
            {
                Log.ShowMessage("Warning", "장비가 가동중입니다.");
                return;
            }

            LOT.LotEnd();
            DM.ARAY[ri.LODR].SetStat(cs.None   );
            DM.ARAY[ri.INDX].SetStat(cs.None   );
            ML.IO_SetY(yi.ETC_ColdGunOnOff, false);
           
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

        private void button12_Click(object sender, EventArgs e)
        {
            ML.IO_SetY(yi.ETC_LightOnOff, !ML.IO_GetY(yi.ETC_LightOnOff));
            //if (!ML.IO_GetY(yi.ETC_LightOnOff))
            //{
            //    button12.Image = global::Machine.Properties.Resources.LightOff;
            //}
            //else
            //{
            //    button12.Image = global::Machine.Properties.Resources.LightOn;
            //}
        }

        private void button13_Click_1(object sender, EventArgs e)
        {
            string sTemp = "";
            sTemp = SEQ.Mcr.Dr1.Test();

        }

        private void btXray_Click(object sender, EventArgs e)
        {
            ML.IO_SetY(yi.XRAY_XRayOn, true);
            if (tbAmp.Text == "" || tbAmp.Text == "" || tbAmp.Text == "")
            {
                Log.ShowMessage("Warning", "X-Ray 조사 조건이 누락되었습니다.");
                return;
            }

            double dAmp  = CConfig.StrToDoubleDef(tbAmp.Text , 0);
            int    iVolt = CConfig.StrToIntDef   (tbVolt.Text, 0);
            double dTime = CConfig.StrToDoubleDef(tbTime.Text, 0);


            if (!double.TryParse(tbAmp.Text, out dAmp))
            {
                Log.ShowMessage("Warning", "관전류에 숫자 이외의 텍스트가 입력되어있습니다.");
                return;
            }
            if (!int   .TryParse(tbVolt.Text, out iVolt)) 
            {
                Log.ShowMessage("Warning", "관전압에 숫자 이외의 텍스트가 입력되어있습니다.");
                return;
            }
            if (!double.TryParse(tbTime.Text, out dTime))
            {
                Log.ShowMessage("Warning", "시간에 숫자 이외의 텍스트가 입력되어있습니다.");
                return;
            }
           
            if (Log.ShowMessageModal("Confirm", "X-Ray를 조사 하시겠습니까?") != DialogResult.Yes) return;

            OM.EqpStat.dAmp  = dAmp;
            OM.EqpStat.iVolt = iVolt;
            OM.EqpStat.dTime = dTime;

            SEQ.XrayCom.SetXrayPara(iVolt, dAmp, dTime);

            Delay(5000);
            ML.IO_SetY(yi.XRAY_XRayOn, false);
        }

        private void lbFilter1_Click(object sender, EventArgs e)
        {
            if (!bUnlock) return;
            string sText = ((Label)sender).Text;
            Log.Trace(sFormText + sText + " Filter Label Clicked", ti.Frm);

            int iLbTag = Convert.ToInt32(((Label)sender).Tag);
            ML.CL_Move((ci)iLbTag, ML.CL_GetCmd((ci)iLbTag) == fb.Bwd ? fb.Fwd : fb.Bwd);
        }

        bool bUnlock = false;
        private void btLock_Click(object sender, EventArgs e)
        {
            if (bUnlock) //Unlock 상태일때 Lock으로 바꿈
            {
                btUnlock.Image = global::Machine.Properties.Resources.Lock32;
                bUnlock = false;
                
            }
            else //Lock 상태일때 Unlock으로 바꿈
            {
                btUnlock.Image = global::Machine.Properties.Resources.Unlock32;
                bUnlock = true;
                
            }
        }

        private void pnLeftUSB_Click(object sender, EventArgs e)
        {
            if (!bUnlock) return;
            string sText = ((Panel)sender).Text;
            Log.Trace(sFormText + sText + " Filter Label Clicked", ti.Frm);

            int iPnTag = Convert.ToInt32(((Panel)sender).Tag);
            ML.CL_Move((ci)iPnTag, ML.CL_GetCmd((ci)iPnTag) == fb.Bwd ? fb.Fwd : fb.Bwd);
        }
        
        double dEndTime;
        private void button32_Click(object sender, EventArgs e)
        {
            //SEQ.XRAY.SaveCsv("Test");
            dEndTime = DateTime.Now.ToOADate();
            OM.EqpStat.dEndTime = dEndTime;

            string sTemp = DateTime.FromOADate(OM.EqpStat.dEndTime).ToString("HHmmss");
        }

        private void btLdrRefill_Click(object sender, EventArgs e)
        {
            if (!bUnlock) return;
            DM.ARAY[ri.LODR].SetStat(cs.Unknown);
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
            return;// DateTime.Now;
        }

        private void button15_Click_1(object sender, EventArgs e)
        {
            IntPtr m_iHwndM = SEQ.Mcr.TestFindWindowL("#32770", "Dressy I/O Sensor Manufacturer Tool");
            //Trace("Handle Check");
            //if (m_iHwndM == IntPtr.Zero)
            //{
            //    sErrMsg = "ERR002_프로그램이 실행되지 않았습니다.(핸들)";
            //    return true;
            //}
            //Trace("Handle Check End");
            //Trace("Window Position Move");
            SEQ.Mcr.TestMoveWindow(m_iHwndM, OM.DressyInfo.iPosX1, OM.DressyInfo.iPosY1, 825, 490, true);
        }

        private void button15_Click(object sender, EventArgs e)
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