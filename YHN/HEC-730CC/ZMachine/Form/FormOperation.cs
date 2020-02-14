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
        public static FormLotOpen FrmLotOpen;
        public static FormMain FrmMain;

        public int LotInfoCnt = 6;
        public int DayInfoCnt = 4;

        private const string sFormText = "Form Operation ";
        
        [DllImport("Kernel32.dll")]
        public static extern bool IsWow64Process(System.IntPtr hProcess, out bool lpSystemInfo);
        public FormOperation(Panel _pnBase)
        {
            InitializeComponent();

            this.TopLevel = false;
            this.Parent = _pnBase;

            //DispLotInfo();
            DispDayList();

            MakeDoubleBuffered(pnPRER,true);
            MakeDoubleBuffered(pnPSTR,true);
            MakeDoubleBuffered(pnPCKL,true);
            MakeDoubleBuffered(pnPCKR,true);
            MakeDoubleBuffered(pnVSTG,true);

            MakeDoubleBuffered(pnTray1,true);
            MakeDoubleBuffered(pnTray2,true);
            MakeDoubleBuffered(pnTray3,true);
            MakeDoubleBuffered(pnTray4,true);

            MakeDoubleBuffered(pnStg ,true);
            MakeDoubleBuffered(pnPck1,true);
            MakeDoubleBuffered(pnPck2,true);
            


            DM.ARAY[ri.LODR].SetParent(pnLODR); DM.ARAY[ri.LODR].Name = "LODR";
            DM.ARAY[ri.PRER].SetParent(pnPRER); DM.ARAY[ri.PRER].Name = "PRER";
            DM.ARAY[ri.PSTR].SetParent(pnPSTR); DM.ARAY[ri.PSTR].Name = "PSTR";
            DM.ARAY[ri.LPCK].SetParent(pnPCKL); DM.ARAY[ri.LPCK].Name = "PCKL";
            DM.ARAY[ri.RPCK].SetParent(pnPCKR); DM.ARAY[ri.RPCK].Name = "PCKR";
            DM.ARAY[ri.VSTG].SetParent(pnVSTG); DM.ARAY[ri.VSTG].Name = "VSTG";
            DM.ARAY[ri.ULDR].SetParent(pnULDR); DM.ARAY[ri.ULDR].Name = "ULDR";

            //Loading
            DM.ARAY[ri.LODR].SetDisp(cs.None     , "None"     ,Color.White     );
            DM.ARAY[ri.LODR].SetDisp(cs.Unknown  , "Unknown"  ,Color.Aqua      );
            //PreRail                                         
            DM.ARAY[ri.PRER].SetDisp(cs.None     , "None"     ,Color.White     );
            DM.ARAY[ri.PRER].SetDisp(cs.Exist    , "Exist"    ,Color.DarkViolet     );
            DM.ARAY[ri.PRER].SetDisp(cs.LiftUp   , "LiftUp"   ,Color.Green     );
            DM.ARAY[ri.PRER].SetDisp(cs.Empty    , "Empty"    ,Color.Silver    );
            DM.ARAY[ri.PRER].SetDisp(cs.Cleaning , "Cleaning" ,Color.Yellow    );
            //PostRail                                                        
            DM.ARAY[ri.PSTR].SetDisp(cs.None     , "None"     ,Color.White     );
            DM.ARAY[ri.PSTR].SetDisp(cs.Empty    , "Empty"    ,Color.Silver    );
            DM.ARAY[ri.PSTR].SetDisp(cs.LiftUp   , "LiftUp"   ,Color.Green     );
            DM.ARAY[ri.PSTR].SetDisp(cs.Clean    , "Clean"    ,Color.Lime      );
            DM.ARAY[ri.PSTR].SetDisp(cs.WorkEnd  , "WorkEnd"  ,Color.Blue      );                                                                 
            //Left Picker                                                          
            DM.ARAY[ri.LPCK].SetDisp(cs.None     , "None"     ,Color.White     );
            DM.ARAY[ri.LPCK].SetDisp(cs.Pick     , "Pick"     ,Color.Orange    );
            DM.ARAY[ri.LPCK].SetDisp(cs.Cleaning , "Cleaning" ,Color.Yellow    );
            DM.ARAY[ri.LPCK].SetDisp(cs.Place    , "Place"    ,Color.Brown     );
            DM.ARAY[ri.LPCK].SetDisp(cs.Pckrclean, "PckrClean",Color.Crimson     );
            //Right Picker
            DM.ARAY[ri.RPCK].SetDisp(cs.None     , "None"     ,Color.White     );
            DM.ARAY[ri.RPCK].SetDisp(cs.Pick     , "Pick"     ,Color.Orange    );
            DM.ARAY[ri.RPCK].SetDisp(cs.Move     , "Move"     ,Color.LightCoral);
            DM.ARAY[ri.RPCK].SetDisp(cs.Place    , "Place"    ,Color.Brown     );
            //Vacuum Stage                                   
            //DM.ARAY[ri.VSTG].SetDisp(cs.None    , "None"     ,Color.White     );
            DM.ARAY[ri.VSTG].SetDisp(cs.Empty    , "Empty"    ,Color.Silver    );
            DM.ARAY[ri.VSTG].SetDisp(cs.Cleaning , "Cleaning" ,Color.Yellow    );
            DM.ARAY[ri.VSTG].SetDisp(cs.Clean    , "Clean"    ,Color.Lime      );
            DM.ARAY[ri.VSTG].SetDisp(cs.WorkEnd  , "WorkEnd"  ,Color.Blue      );
            //Unloading                                       
            DM.ARAY[ri.ULDR].SetDisp(cs.None     , "None"     ,Color.White     );
            DM.ARAY[ri.ULDR].SetDisp(cs.WorkEnd  , "WorkEnd"  ,Color.Blue      );
            
            DM.LoadMap();

            var PropLotInfo = pbLeftPicker.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropLotInfo.SetValue(pbLeftPicker, true, null);
            
            PropLotInfo = pbRightPicker.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropLotInfo.SetValue(pbRightPicker, true, null);
            
            PropLotInfo = pbStage.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropLotInfo.SetValue(pbStage, true, null);
        }
        
        private void PanelRefresh()
        {
            pnLODR.Refresh();
            pnPRER.Refresh();
            pnPSTR.Refresh();
            pnPCKL.Refresh();
            pnPCKR.Refresh();
            pnVSTG.Refresh();
            pnULDR.Refresh();

            for (int i = 0 ; i < ri.MAX_ARAY ; i++)
            {
                DM.ARAY[i].UpdateAray();
            }
        }

        private void btLotEnd_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            LOT.LotEnd();
        }

        private static bool bPreLotOpen = false;

        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            tmUpdate.Enabled = false;
            //로그인/로그아웃 방식
            if (SM.FrmLogOn.GetLevel() == (int)EN_LEVEL.LogOff)
            {
                btLogIn.Text = "  LOG IN";

                pnManual .Enabled = false;
            }
            else
            {
                btLogIn.Text = "  " + SM.FrmLogOn.GetLevel().ToString();

                pnManual .Enabled = true;
            }
                        
            //if (SML.FrmLogOn.GetLevel() != (int)EN_LEVEL.LogOff)
            //{
            //    btStart.Enabled = LOT.GetLotOpen();
            //}
            
            //SPC.LOT.DispLotInfo(lvLotInfo);
            SPC.DAY.DispDayInfo(lvDayInfo);

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

            //if(!ML.MT_GetHomeDoneAll()){
            //    btAllHome.ForeColor = SEQ._bFlick ? Color.Black : Color.Red;
            //}
            //else {
            //    btAllHome.ForeColor = Color.Black  ;
            //}

            //if (LOT.GetLotOpen() && !SEQ._bRun)
            //{
            //    btLotOpen.Text = "WORK ING";
            //    //btLotOpen.Enabled = true;
            //    btLotEnd .Enabled = true ;
            //}
            //else
            //{
            //    btLotOpen.Text = "WORK STT";
            //    //btLotOpen.Enabled = true ;
            //    btLotEnd .Enabled = false;
            //}

            //현재 구동 상태 확인
            //Pre Rail
            switch (SEQ.PRER.GetSeqStep())
            {
                default: break;
                case 0:
                    pnR1.BackColor = SystemColors.ControlDark;
                    pnR2.BackColor = SystemColors.ControlDark;
                    pnR3.BackColor = SystemColors.ControlDark;
                    break;
                case 2:
                    pnR1.BackColor = SEQ._bFlick ? Color.DarkKhaki : SystemColors.ControlDark;
                    pnR2.BackColor = SystemColors.ControlDark;
                    pnR3.BackColor = SystemColors.ControlDark;
                    break;
                case 3:
                    pnR1.BackColor = Color.DarkKhaki;
                    pnR2.BackColor = SEQ._bFlick ? Color.DarkKhaki : SystemColors.ControlDark;
                    pnR3.BackColor = SystemColors.ControlDark;
                    break;
                case 4:
                    pnR1.BackColor = Color.DarkKhaki;
                    pnR2.BackColor = Color.DarkKhaki;
                    pnR3.BackColor = SEQ._bFlick ? Color.DarkKhaki : SystemColors.ControlDark;
                    break;
            }
            //Post Rail
            switch (SEQ.PSTR.GetSeqStep())
            {
                default: break;
                case 0:
                    pnR4.BackColor = SystemColors.ControlDark;
                    pnR5.BackColor = SystemColors.ControlDark;
                    pnR6.BackColor = SystemColors.ControlDark;
                    pnR7.BackColor = SystemColors.ControlDark;
                    break;
                case 2:
                    pnR4.BackColor = SEQ._bFlick ? Color.DarkKhaki : SystemColors.ControlDark;
                    pnR5.BackColor = SystemColors.ControlDark;
                    pnR6.BackColor = SystemColors.ControlDark;
                    pnR7.BackColor = SystemColors.ControlDark;
                    break;
                case 3:
                    pnR4.BackColor = Color.DarkKhaki;
                    pnR5.BackColor = SEQ._bFlick ? Color.DarkKhaki : SystemColors.ControlDark;
                    pnR6.BackColor = SystemColors.ControlDark;
                    pnR7.BackColor = SystemColors.ControlDark;
                    break;
                case 4:
                    pnR4.BackColor = Color.DarkKhaki;
                    pnR5.BackColor = Color.DarkKhaki;
                    pnR6.BackColor = SEQ._bFlick ? Color.DarkKhaki : SystemColors.ControlDark;
                    pnR7.BackColor = SystemColors.ControlDark;
                    break;
                case 5:
                    pnR4.BackColor = Color.DarkKhaki;
                    pnR5.BackColor = Color.DarkKhaki;
                    pnR6.BackColor = Color.DarkKhaki;
                    pnR7.BackColor = SEQ._bFlick ? Color.DarkKhaki : SystemColors.ControlDark;
                    break;
            }
            //Left Picker
            switch (SEQ.LPCK.GetSeqStep())
            {
                default: break;
                case 0:
                    pnLP1.BackColor = SystemColors.ControlDark;
                    pnLP2.BackColor = SystemColors.ControlDark;
                    pnLP3.BackColor = SystemColors.ControlDark;
                    pnLP4.BackColor = SystemColors.ControlDark;
                    break;
                case 1:
                    pnLP1.BackColor = SEQ._bFlick ? Color.DarkKhaki : SystemColors.ControlDark;
                    pnLP2.BackColor = SystemColors.ControlDark;
                    pnLP3.BackColor = SystemColors.ControlDark;
                    pnLP4.BackColor = SystemColors.ControlDark;
                    break;
                case 2:
                    pnLP1.BackColor = Color.DarkKhaki;
                    pnLP2.BackColor = SEQ._bFlick ? Color.DarkKhaki : SystemColors.ControlDark;
                    pnLP3.BackColor = SystemColors.ControlDark;
                    pnLP4.BackColor = SystemColors.ControlDark;
                    break;
                case 3:
                    pnLP1.BackColor = Color.DarkKhaki;
                    pnLP2.BackColor = Color.DarkKhaki;
                    pnLP3.BackColor = SEQ._bFlick ? Color.DarkKhaki : SystemColors.ControlDark;
                    pnLP4.BackColor = SystemColors.ControlDark;
                    break;
                case 4:
                    pnLP1.BackColor = Color.DarkKhaki;
                    pnLP2.BackColor = Color.DarkKhaki;
                    pnLP3.BackColor = Color.DarkKhaki;
                    pnLP4.BackColor = SEQ._bFlick ? Color.DarkKhaki : SystemColors.ControlDark; 
                    break;
            }
            //Right Picker
            switch (SEQ.RPCK.GetSeqStep())
            {
                default: break;
                case 0:
                    pnRP1.BackColor = SystemColors.ControlDark;
                    pnRP2.BackColor = SystemColors.ControlDark;
                    pnRP3.BackColor = SystemColors.ControlDark;
                    break;
                case 1:
                    pnRP1.BackColor = SEQ._bFlick ? Color.DarkKhaki : SystemColors.ControlDark;
                    pnRP2.BackColor = SystemColors.ControlDark;
                    pnRP3.BackColor = SystemColors.ControlDark;
                    break;
                case 2:
                    pnRP1.BackColor = Color.DarkKhaki;
                    pnRP2.BackColor = SEQ._bFlick ? Color.DarkKhaki : SystemColors.ControlDark;
                    pnRP3.BackColor = SystemColors.ControlDark;
                    break;
                case 3:
                    pnRP1.BackColor = Color.DarkKhaki;
                    pnRP2.BackColor = Color.DarkKhaki;
                    pnRP3.BackColor = SEQ._bFlick ? Color.DarkKhaki : SystemColors.ControlDark;
                    break;
            }
            //Vacuum Stage
            switch (SEQ.VSTG.GetSeqStep())
            {
                default: break;
                case 0:
                    pnS1.BackColor = SystemColors.ControlDark;
                    pnS2.BackColor = SystemColors.ControlDark;
                    break;
                case 1:
                    pnS1.BackColor = SEQ._bFlick ? Color.DarkKhaki : SystemColors.ControlDark;
                    pnS2.BackColor = SystemColors.ControlDark;
                    break;
                case 2:
                    pnS1.BackColor = Color.DarkKhaki;
                    pnS2.BackColor = SEQ._bFlick ? Color.DarkKhaki : SystemColors.ControlDark;
                    break;
            }

            //Ionizer
            lbPRER_TopIon.BackColor = ML.IO_GetX(xi.PSTR_IonBlwrTop) ? Color.Lime : Color.Gray;
            lbPRER_TopIon.Text      = ML.IO_GetX(xi.PSTR_IonBlwrTop) ? "ON" : "OFF";
            lbPRER_BtmIon.BackColor = ML.IO_GetX(xi.PSTR_IonBlwrBtm) ? Color.Lime : Color.Gray;
            lbPRER_BtmIon.Text      = ML.IO_GetX(xi.PSTR_IonBlwrBtm) ? "ON" : "OFF";
            lbLPCK_BtmIon.BackColor = ML.IO_GetX(xi.LPCK_IonBlwrBtm) ? Color.Lime : Color.Gray;
            lbLPCK_BtmIon.Text      = ML.IO_GetX(xi.LPCK_IonBlwrBtm) ? "ON" : "OFF";
            lbVSTG_TopIon.BackColor = ML.IO_GetX(xi.VSTG_IonBlwrTop) ? Color.Lime : Color.Gray;
            lbVSTG_TopIon.Text      = ML.IO_GetX(xi.VSTG_IonBlwrTop) ? "ON" : "OFF";

            lbPRER_TopVtg.BackColor = ML.IO_GetX(xi.PSTR_HighVtgTop) ? Color.Lime : Color.Gray;
            lbPRER_TopVtg.Text      = ML.IO_GetX(xi.PSTR_HighVtgTop) ? "ON" : "OFF";
            lbPRER_BtmVtg.BackColor = ML.IO_GetX(xi.PSTR_HighVtgBtm) ? Color.Lime : Color.Gray;
            lbPRER_BtmVtg.Text      = ML.IO_GetX(xi.PSTR_HighVtgBtm) ? "ON" : "OFF";
            lbLPCK_BtmVtg.BackColor = ML.IO_GetX(xi.LPCK_HighVtgBtm) ? Color.Lime : Color.Gray;
            lbLPCK_BtmVtg.Text      = ML.IO_GetX(xi.LPCK_HighVtgBtm) ? "ON" : "OFF";
            lbVSTG_TopVtg.BackColor = ML.IO_GetX(xi.VSTG_HighVtgTop) ? Color.Lime : Color.Gray;
            lbVSTG_TopVtg.Text      = ML.IO_GetX(xi.VSTG_HighVtgTop) ? "ON" : "OFF";

            //Ionizer
            lbPSTR_TopIonStat.BackColor = OM.CmnOptn.bPSTR_IonTopOff ? Color.Red : Color.Lime ;
            lbPSTR_TopIonStat.Text      = OM.CmnOptn.bPSTR_IonTopOff ? "NOT USE" : "USE"      ; 
            lbPSTR_BtmIonStat.BackColor = OM.CmnOptn.bPSTR_IonBtmOff ? Color.Red : Color.Lime ;
            lbPSTR_BtmIonStat.Text      = OM.CmnOptn.bPSTR_IonBtmOff ? "NOT USE" : "USE"      ;      
            lbLPCK_BtmIonStat.BackColor = OM.CmnOptn.bLPCK_IonOff    ? Color.Red : Color.Lime ;
            lbLPCK_BtmIonStat.Text      = OM.CmnOptn.bLPCK_IonOff    ? "NOT USE" : "USE"      ;      
            lbVSTG_TopIonStat.BackColor = OM.CmnOptn.bVSTG_IonOff    ? Color.Red : Color.Lime ;
            lbVSTG_TopIonStat.Text      = OM.CmnOptn.bVSTG_IonOff    ? "NOT USE" : "USE"      ;      
                                                                                              
            lbPSTR_TopAir1Stat.BackColor = OM.CmnOptn.bPSTR_Air1TopOff ? Color.Red : Color.Lime ;
            lbPSTR_TopAir1Stat.Text      = OM.CmnOptn.bPSTR_Air1TopOff ? "1 NOT USE" : "1 USE"      ;      
            lbPSTR_BtmAir1Stat.BackColor = OM.CmnOptn.bPSTR_Air1BtmOff ? Color.Red : Color.Lime ;
            lbPSTR_BtmAir1Stat.Text      = OM.CmnOptn.bPSTR_Air1BtmOff ? "1 NOT USE" : "1 USE"      ;      
            lbLPCK_BtmAir1Stat.BackColor = OM.CmnOptn.bLPCK_Air1Off    ? Color.Red : Color.Lime ;
            lbLPCK_BtmAir1Stat.Text      = OM.CmnOptn.bLPCK_Air1Off    ? "1 NOT USE" : "1 USE"      ;      
            lbVSTG_TopAir1Stat.BackColor = OM.CmnOptn.bVSTG_Air1Off    ? Color.Red : Color.Lime ;
            lbVSTG_TopAir1Stat.Text      = OM.CmnOptn.bVSTG_Air1Off    ? "1 NOT USE" : "1 USE"      ;

            lbPSTR_TopAir2Stat.BackColor = OM.CmnOptn.bPSTR_Air2TopOff ? Color.Red : Color.Lime ;
            lbPSTR_TopAir2Stat.Text      = OM.CmnOptn.bPSTR_Air2TopOff ? "2 NOT USE" : "2 USE"      ;      
            lbPSTR_BtmAir2Stat.BackColor = OM.CmnOptn.bPSTR_Air2BtmOff ? Color.Red : Color.Lime ;
            lbPSTR_BtmAir2Stat.Text      = OM.CmnOptn.bPSTR_Air2BtmOff ? "2 NOT USE" : "2 USE"      ;      
            lbLPCK_BtmAir2Stat.BackColor = OM.CmnOptn.bLPCK_Air2Off    ? Color.Red : Color.Lime ;
            lbLPCK_BtmAir2Stat.Text      = OM.CmnOptn.bLPCK_Air2Off    ? "2 NOT USE" : "2 USE"      ;      
            lbVSTG_TopAir2Stat.BackColor = OM.CmnOptn.bVSTG_Air2Off    ? Color.Red : Color.Lime ;
            lbVSTG_TopAir2Stat.Text      = OM.CmnOptn.bVSTG_Air2Off    ? "2 NOT USE" : "2 USE"      ;

            PanelRefresh();

            //Image Move LeftPicker
            //105, 60 ~250
            double dLPbZero = 312;//
            double dLPbMax  = 77 ;//
            double dLPos = ML.MT_GetCmdPos((int)mi.LPCK_Y); //dTemp;//
            //double dHeight = 312 - (dPos / 567 * 240);
            double dLHeight = dLPbZero - (dLPos / ML.MT_GetMaxPosition((int)mi.LPCK_Y) * (dLPbZero - dLPbMax));
            //pbLeftPicker.Location = new Point(396, (int)dLHeight);
            pnPck1.Location = new Point(396, (int)dLHeight);

            //Image Move RightPicker
            //105, 60 ~250
            double dRPbZero = 312;//
            double dRPbMax = 77;//
            double dRPos = ML.MT_GetCmdPos((int)mi.RPCK_Y); //dTemp;//
            //double dHeight = 312 - (dPos / 567 * 240);
            double dRHeight = dRPbZero - (dRPos / ML.MT_GetMaxPosition((int)mi.RPCK_Y) * (dRPbZero - dRPbMax));
            //pbRightPicker.Location = new Point(687, (int)dRHeight);
            pnPck2.Location = new Point(687, (int)dRHeight);

            //Image Move Stage
            //105, 60 ~250
            double dSPbZero = 692;//
            double dSPbMax = 363;//
            double dSPos = ML.MT_GetCmdPos((int)mi.VSTG_X); //dTemp;//
            //double dHeight = 312 - (dPos / 567 * 240);
            double dSWidth = dSPbZero - (dSPos / ML.MT_GetMaxPosition((int)mi.VSTG_X) * (dSPbZero - dSPbMax));
            //pbStage.Location = new Point((int)dSWidth, 298);
            pnStg.Location = new Point((int)dSWidth, 298);

            bool s1 = !DM.ARAY[ri.LODR].CheckAllStat(cs.None ); //bool r1 = ML.IO_GetX(xi.PREB_StrpDetect);
            bool s2 = !DM.ARAY[ri.PRER].CheckAllStat(cs.None ); //bool r2 = ML.IO_GetX(xi.RAIL_Vsn1Detect);
            bool s3 = !DM.ARAY[ri.LPCK].CheckAllStat(cs.None ); //bool r3 = ML.IO_GetX(xi.RAIL_Vsn2Detect);
            bool s4 = !DM.ARAY[ri.VSTG].CheckAllStat(cs.Empty); //bool r4 = ML.IO_GetX(xi.RAIL_Vsn3Detect);
            bool s5 = !DM.ARAY[ri.RPCK].CheckAllStat(cs.None ); //bool r5 = ML.IO_GetX(xi.PSTB_MarkDetect);
            bool s6 = !DM.ARAY[ri.PSTR].CheckAllStat(cs.None ); //bool r6 = ML.IO_GetX(xi.PSTB_MarkDetect);
            bool s7 = !DM.ARAY[ri.ULDR].CheckAllStat(cs.None ); //bool r7 = ML.IO_GetX(xi.PSTB_MarkDetect);
            bool s8 = !DM.ARAY[ri.VSTG].CheckAllStat(cs.WorkEnd); //bool r4 = ML.IO_GetX(xi.RAIL_Vsn3Detect);
            bool s9 = !DM.ARAY[ri.LPCK].CheckAllStat(cs.Pckrclean); //bool r4 = ML.IO_GetX(xi.RAIL_Vsn3Detect);

            pnTray1.Visible = s1 ;
            pnTray2.Visible = s2 ;
            pnTray3.Visible = s6 ;
            pnTray4.Visible = s7 ;
            
            pbLeftPicker  .Visible = !s3 || !s9;
            pbRightPicker .Visible = !s5 ;
            pbStage       .Visible = !s4 || !s8;

            P1.BackColor = SEQ.PSTR.m_bVacSkip1 ? Color.Red : Color.Lime;
            P2.BackColor = SEQ.PSTR.m_bVacSkip2 ? Color.Red : Color.Lime;
            P3.BackColor = SEQ.PSTR.m_bVacSkip3 ? Color.Red : Color.Lime;
            P4.BackColor = SEQ.PSTR.m_bVacSkip4 ? Color.Red : Color.Lime;
            P5.BackColor = SEQ.PSTR.m_bVacSkip5 ? Color.Red : Color.Lime;
            P6.BackColor = SEQ.PSTR.m_bVacSkip6 ? Color.Red : Color.Lime;

            P1.Text = SEQ.PSTR.m_bVacSkip1 ? "Skip" : "";
            P2.Text = SEQ.PSTR.m_bVacSkip2 ? "Skip" : "";
            P3.Text = SEQ.PSTR.m_bVacSkip3 ? "Skip" : "";
            P4.Text = SEQ.PSTR.m_bVacSkip4 ? "Skip" : "";
            P5.Text = SEQ.PSTR.m_bVacSkip5 ? "Skip" : "";
            P6.Text = SEQ.PSTR.m_bVacSkip6 ? "Skip" : "";

            //Left Picker Vacuum 켜져있을때
            if(ML.IO_GetY(yi.LPCK_Vacuum1) && ML.IO_GetY(yi.LPCK_Vacuum2) &&
               ML.IO_GetY(yi.LPCK_Vacuum3) && ML.IO_GetY(yi.LPCK_Vacuum4) &&
               ML.IO_GetY(yi.LPCK_Vacuum5) && ML.IO_GetY(yi.LPCK_Vacuum6))
            {
                b63.ForeColor = Color.Lime;
            }
            else if (!ML.IO_GetY(yi.LPCK_Vacuum1) && !ML.IO_GetY(yi.LPCK_Vacuum2) &&
                     !ML.IO_GetY(yi.LPCK_Vacuum3) && !ML.IO_GetY(yi.LPCK_Vacuum4) &&
                     !ML.IO_GetY(yi.LPCK_Vacuum5) && !ML.IO_GetY(yi.LPCK_Vacuum6))
            {
                b63.ForeColor = SystemColors.ControlText;
            }
            else
            {
                b63.ForeColor = Color.Red;
            }

            //Right Picker Vacuum 켜져있을때
            if(ML.IO_GetY(yi.RPCK_Vacuum1) && ML.IO_GetY(yi.RPCK_Vacuum2) &&
               ML.IO_GetY(yi.RPCK_Vacuum3) && ML.IO_GetY(yi.RPCK_Vacuum4) &&
               ML.IO_GetY(yi.RPCK_Vacuum5) && ML.IO_GetY(yi.RPCK_Vacuum6))
            {
                b65.ForeColor = Color.Lime;
            }
            else if (!ML.IO_GetY(yi.RPCK_Vacuum1) && !ML.IO_GetY(yi.RPCK_Vacuum2) &&
                     !ML.IO_GetY(yi.RPCK_Vacuum3) && !ML.IO_GetY(yi.RPCK_Vacuum4) &&
                     !ML.IO_GetY(yi.RPCK_Vacuum5) && !ML.IO_GetY(yi.RPCK_Vacuum6))
            {
                b65.ForeColor = SystemColors.ControlText;
            }
            else
            {
                b65.ForeColor = Color.Red;
            }

            //Stage Vacuum 켜져있을때
            if(ML.IO_GetY(yi.VSTG_Vacuum1) && ML.IO_GetY(yi.VSTG_Vacuum2) &&
               ML.IO_GetY(yi.VSTG_Vacuum3) && ML.IO_GetY(yi.VSTG_Vacuum4) &&
               ML.IO_GetY(yi.VSTG_Vacuum5) && ML.IO_GetY(yi.VSTG_Vacuum6))
            {
                b67.ForeColor = Color.Lime;
            }
            else if (!ML.IO_GetY(yi.VSTG_Vacuum1) && !ML.IO_GetY(yi.VSTG_Vacuum2) &&
                     !ML.IO_GetY(yi.VSTG_Vacuum3) && !ML.IO_GetY(yi.VSTG_Vacuum4) &&
                     !ML.IO_GetY(yi.VSTG_Vacuum5) && !ML.IO_GetY(yi.VSTG_Vacuum6))
            {
                b67.ForeColor = SystemColors.ControlText;
            }
            else
            {
                b67.ForeColor = Color.Red;
            }

            //Left Picker Ejector 켜져있을때
            if(ML.IO_GetY(yi.LPCK_Eject1) && ML.IO_GetY(yi.LPCK_Eject2) &&
               ML.IO_GetY(yi.LPCK_Eject3) && ML.IO_GetY(yi.LPCK_Eject4) &&
               ML.IO_GetY(yi.LPCK_Eject5) && ML.IO_GetY(yi.LPCK_Eject6))
            {
                b64.ForeColor = Color.Lime;
            }
            else if (!ML.IO_GetY(yi.LPCK_Eject1) && !ML.IO_GetY(yi.LPCK_Eject2) &&
                     !ML.IO_GetY(yi.LPCK_Eject3) && !ML.IO_GetY(yi.LPCK_Eject4) &&
                     !ML.IO_GetY(yi.LPCK_Eject5) && !ML.IO_GetY(yi.LPCK_Eject6))
            {
                b64.ForeColor = SystemColors.ControlText;
            }
            else
            {
                b64.ForeColor = Color.Red;
            }

            //Right Picker Ejector 켜져있을때
            if(ML.IO_GetY(yi.RPCK_Eject1) && ML.IO_GetY(yi.RPCK_Eject2) &&
               ML.IO_GetY(yi.RPCK_Eject3) && ML.IO_GetY(yi.RPCK_Eject4) &&
               ML.IO_GetY(yi.RPCK_Eject5) && ML.IO_GetY(yi.RPCK_Eject6))
            {
                b66.ForeColor = Color.Lime;
            }
            else if (!ML.IO_GetY(yi.RPCK_Eject1) && !ML.IO_GetY(yi.RPCK_Eject2) &&
                     !ML.IO_GetY(yi.RPCK_Eject3) && !ML.IO_GetY(yi.RPCK_Eject4) &&
                     !ML.IO_GetY(yi.RPCK_Eject5) && !ML.IO_GetY(yi.RPCK_Eject6))
            {
                b66.ForeColor = SystemColors.ControlText;
            }
            else
            {
                b66.ForeColor = Color.Red;
            }

            //Stage Ejector 켜져있을때
            if(ML.IO_GetY(yi.VSTG_Eject1) && ML.IO_GetY(yi.VSTG_Eject2) &&
               ML.IO_GetY(yi.VSTG_Eject3) && ML.IO_GetY(yi.VSTG_Eject4) &&
               ML.IO_GetY(yi.VSTG_Eject5) && ML.IO_GetY(yi.VSTG_Eject6))
            {
                b68.ForeColor = Color.Lime;
            }
            else if (!ML.IO_GetY(yi.VSTG_Eject1) && !ML.IO_GetY(yi.VSTG_Eject2) &&
                     !ML.IO_GetY(yi.VSTG_Eject3) && !ML.IO_GetY(yi.VSTG_Eject4) &&
                     !ML.IO_GetY(yi.VSTG_Eject5) && !ML.IO_GetY(yi.VSTG_Eject6))
            {
                b68.ForeColor = SystemColors.ControlText;
            }
            else
            {
                b68.ForeColor = Color.Red;
            }

            //Left Picker Cleaner 켜져있을때
            if(ML.IO_GetY(yi.LPCK_IonBlwrBtm) && ML.IO_GetY(yi.LPCK_AirBlwrBtm1) && ML.IO_GetY(yi.LPCK_AirBlwrBtm2))
            {
                b61.ForeColor = Color.Lime;
            }
            else if (!ML.IO_GetY(yi.LPCK_IonBlwrBtm) && !ML.IO_GetY(yi.LPCK_AirBlwrBtm1) && !ML.IO_GetY(yi.LPCK_AirBlwrBtm2))
            {
                b61.ForeColor = SystemColors.ControlText;
            }
            else
            {
                b61.ForeColor = Color.Red;
            }

            //Pre Rail Cleaner 켜져있을때
            if(ML.IO_GetY(yi.PSTR_IonBlwrBtm) && ML.IO_GetY(yi.PSTR_AirBlwrBtm1) && ML.IO_GetY(yi.PSTR_AirBlwrBtm2) &&
               ML.IO_GetY(yi.PSTR_IonBlwrTop) && ML.IO_GetY(yi.PSTR_AirBlwrTop1) && ML.IO_GetY(yi.PSTR_AirBlwrTop2))
            {
                b60.ForeColor = Color.Lime;
            }
            else if (!ML.IO_GetY(yi.PSTR_IonBlwrBtm) && !ML.IO_GetY(yi.PSTR_AirBlwrBtm1) && !ML.IO_GetY(yi.PSTR_AirBlwrBtm2) &&
                     !ML.IO_GetY(yi.PSTR_IonBlwrTop) && !ML.IO_GetY(yi.PSTR_AirBlwrTop1) && !ML.IO_GetY(yi.PSTR_AirBlwrTop2))
            {
                b60.ForeColor = SystemColors.ControlText;
            }
            else
            {
                b60.ForeColor = Color.Red;
            }

            //Stage Cleaner 켜져있을때
            if(ML.IO_GetY(yi.VSTG_IonBlwrTop) && ML.IO_GetY(yi.VSTG_AirBlwrTop1) && ML.IO_GetY(yi.VSTG_AirBlwrTop2))
            {
                b62.ForeColor = Color.Lime;
            }
            else if (!ML.IO_GetY(yi.VSTG_IonBlwrTop) && !ML.IO_GetY(yi.VSTG_AirBlwrTop1) && !ML.IO_GetY(yi.VSTG_AirBlwrTop2))
            {
                b62.ForeColor = SystemColors.ControlText;
            }
            else
            {
                b62.ForeColor = Color.Red;
            }

           
            if(!ML.MT_GetStop(mi.PRER_X) && !ML.MT_GetStop(mi.PSTR_X))
            {
                button2.ForeColor = Color.Lime;
            }
            else if(ML.MT_GetStop(mi.PRER_X) && ML.MT_GetStop(mi.PSTR_X))
            {
                button2.ForeColor = SystemColors.ControlText;
            }
            else
            {
                button2.ForeColor = Color.Red;
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
        //public void DispLotInfo() //오퍼레이션 창용.
        //{
        //    lvLotInfo.Clear();
        //    lvLotInfo.View = View.Details;
        //    lvLotInfo.LabelEdit = true;
        //    lvLotInfo.AllowColumnReorder = true;
        //    lvLotInfo.FullRowSelect = true;
        //    lvLotInfo.GridLines = true;
        //    //lvLotInfo.Sorting = SortOrder.Descending;
        //    lvLotInfo.Scrollable = true;
        //    lvLotInfo.Enabled = true;
        //    //lvLotInfo.TileSize = new Size(lvLotInfo.Width,lvLotInfo.Height);
        //    //lvLotInfo.Columns.Add("", 200, HorizontalAlignment.Left);     //1280*1024는 109, 1024*768은 78
        //    //lvLotInfo.Columns.Add("", 300, HorizontalAlignment.Left);     //1280*1024는 109, 1024*768은 78
        //    lvLotInfo.Columns.Add("", 175, HorizontalAlignment.Left);     //1280*1024는 109, 1024*768은 78
        //    //lvLotInfo.Columns.Add("", 100, HorizontalAlignment.Left);     //1280*1024는 109, 1024*768은 78
        //
        //    ListViewItem[] liLotInfo = new ListViewItem[LotInfoCnt*2];
        //
        //    for (int i = 0; i < LotInfoCnt*2; i++)
        //    {
        //        liLotInfo[i] = new ListViewItem();
        //        liLotInfo[i].SubItems.Add("");
        //
        //
        //        liLotInfo[i].UseItemStyleForSubItems = false;
        //        liLotInfo[i].UseItemStyleForSubItems = false;
        //
        //        //liLotInfo[i].BackColor = Color.White;
        //        lvLotInfo.Items.Add(liLotInfo[i]);
        //      
        //    }
        //
        //    var PropLotInfo = lvLotInfo.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
        //    PropLotInfo.SetValue(lvLotInfo, true, null);
        //}

        public void DispDayList() //오퍼레이션 창용.
        {
            lvDayInfo.Clear();
            lvDayInfo.View = View.Details;
            lvDayInfo.LabelEdit = true;
            lvDayInfo.AllowColumnReorder = true;
            lvDayInfo.FullRowSelect = true;
            lvDayInfo.GridLines = true;
            //lvDayInfo.Sorting = SortOrder.Descending;
            lvDayInfo.Scrollable = true;
            lvDayInfo.Enabled = true;
            //lvDayInfo.TileSize = new Size(lvDayInfo.Width - 22,lvDayInfo.Height/DayInfoCnt);

            lvDayInfo.Columns.Add("", 175, HorizontalAlignment.Left);    //1280*1024는 109, 1024*768은 78
            //lvDayInfo.Columns.Add("", 100, HorizontalAlignment.Left);    //1280*1024는 109, 1024*768은 78

            ListViewItem[] liDayInfo = new ListViewItem[DayInfoCnt*2];

            for (int i = 0; i < DayInfoCnt*2; i++)
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
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            SEQ._bBtnStart = true;
        }

        private void btStop_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            SEQ._bBtnStop = true;
        }

        private void btReset_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            SEQ._bBtnReset = true;
        }

        private void FormOperation_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmUpdate.Enabled = false;
            DM.SaveMap();

            //VL.Close();
        }

        private void btManual1_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            if (Log.ShowMessageModal("Confirm", "Do you want to perform manual actions?") != DialogResult.Yes) return;

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

        private void btOperator_Click(object sender, EventArgs e)
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

        private void btAllHome_Click(object sender, EventArgs e)
        {
            if (!ML.IO_GetY(yi.ETC_MainAirOnOff))
            {
                ML.ER_SetErr(ei.ETC_MainAir, "메인 에어를 ON 해주세요.");
                return;
            }

            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

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

        private void FormOperation_Shown(object sender, EventArgs e)
        {
            tmUpdate.Enabled = true;
        }

        private void FormOperation_VisibleChanged(object sender, EventArgs e)
        {
            tmUpdate.Enabled = this.Visible;
        }

        public void MakeDoubleBuffered(Control control, bool setting)
        {
            Type controlType = control.GetType();
            PropertyInfo pi = controlType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(control, setting, null);
        }

        private void btRemoveStrip_Click(object sender, EventArgs e)
        {

        }


        private void button26_Click(object sender, EventArgs e)
        {
            //LOT.LotOpen(tbLotNo.Text);
        }

        private void button28_Click(object sender, EventArgs e)
        {

        }

        private void button27_Click(object sender, EventArgs e)
        {
            LOT.LotEnd();
        }

        private void btLightOnOff_Click(object sender, EventArgs e)
        {
            ML.IO_SetY(yi.ETC_LightOnOff,!ML.IO_GetY(yi.ETC_LightOnOff));
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btCyl_Click(object sender, EventArgs e)
        {
            string sText = ((Label)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            Label iSelLabel = sender as Label ;

            if(iSelLabel == null) return ;

            if(SEQ._bRun) return ;


            int iSel = Convert.ToInt32(iSelLabel.Tag);
             
            ML.CL_Move(iSel , ML.CL_GetCmd(iSel)==fb.Fwd ? fb.Bwd : fb.Fwd ) ;
        }

        private void btOutput_Click(object sender, EventArgs e)
        {
            Label iSelLabel = sender as Label ;

            if(iSelLabel == null) return ;

            if (SEQ._bRun) return;

            int iSel = Convert.ToInt32(iSelLabel.Tag);
             
            ML.IO_SetY(iSel, !ML.IO_GetY(iSel)) ;
        }

        private void button34_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            if (SEQ._bRun) return;

            const int iBreakAdd = 3 ;
            //bool bBreakStat = ML.  MT_GetY(mi.LODR_ZClmp , iBreakAdd) ;
            //ML.MT_SetY(mi.LODR_ZClmp , iBreakAdd , !bBreakStat);

        }

        private void btULDBreakOnOff_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            if (SEQ._bRun) return;

            const int iBreakAdd = 3 ;
            //bool bBreakStat = ML.  MT_GetY(mi.ULDR_ZClmp , iBreakAdd) ;
            //ML.MT_SetY(mi.ULDR_ZClmp , iBreakAdd , !bBreakStat);

        }

        private void tcVision_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if(tcVision.SelectedIndex == 1)
            //{
            //    DM.ARAY[ri.LODR].SetParent(pnLODR);
            //    DM.ARAY[ri.ULDR].SetParent(pnULDR);
            //}
            //else
            //{
            //    DM.ARAY[ri.LODR].SetParent(pnLODR1);
            //    DM.ARAY[ri.ULDR].SetParent(pnULDR1);
            //}
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

        private void btGetLotNo_Click(object sender, EventArgs e)
        {

            

            //tbLotNo.Text = LOT.PopMgz();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            OM.EqpStat.iDayWorkCnt += 1;
        }

        private void btHome_Click(object sender, EventArgs e)
        {
            
            MM.SetManCycle(mc.AllHome);
        }

        private void pnTray1_DoubleClick(object sender, EventArgs e)
        {
            if (Log.ShowMessageModal("Confirm", "Do you want to remove the strip?") != DialogResult.Yes) return;
            DM.ARAY[ri.LODR].SetStat(cs.None);
        }

        private void pnTray2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pnTray2_DoubleClick(object sender, EventArgs e)
        {
            if (Log.ShowMessageModal("Confirm", "Do you want to remove the strip?") != DialogResult.Yes) return;
            DM.ARAY[ri.PRER].SetStat(cs.None);
        }

        private void pnTray3_DoubleClick(object sender, EventArgs e)
        {
            if (Log.ShowMessageModal("Confirm", "Do you want to remove the strip?") != DialogResult.Yes) return;
            DM.ARAY[ri.PSTR].SetStat(cs.None);
        }

        private void pnTray4_DoubleClick(object sender, EventArgs e)
        {
            if (Log.ShowMessageModal("Confirm", "Do you want to remove the strip?") != DialogResult.Yes) return;
            DM.ARAY[ri.ULDR].SetStat(cs.None);
        }

        private void pnPck1_DoubleClick(object sender, EventArgs e)
        {
            if (Log.ShowMessageModal("Confirm", "Do you want to remove the strip?") != DialogResult.Yes) return;
            DM.ARAY[ri.LPCK].SetStat(cs.None);
        }

        private void pnPck2_DoubleClick(object sender, EventArgs e)
        {
            if (Log.ShowMessageModal("Confirm", "Do you want to remove the strip?") != DialogResult.Yes) return;
            DM.ARAY[ri.RPCK].SetStat(cs.None);
        }

        private void pnStg_DoubleClick(object sender, EventArgs e)
        {
            if (Log.ShowMessageModal("Confirm", "Do you want to remove the strip?") != DialogResult.Yes) return;
            DM.ARAY[ri.VSTG].SetStat(cs.Empty);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Log.ShowMessageModal("Notice", "Are you sure you want to clear DataMap?") == DialogResult.Yes)
            {
                DM.ARAY[ri.LODR].SetStat(cs.Unknown);
                DM.ARAY[ri.PRER].SetStat(cs.None);
                DM.ARAY[ri.PSTR].SetStat(cs.None);
                DM.ARAY[ri.ULDR].SetStat(cs.None);
                DM.ARAY[ri.LPCK].SetStat(cs.None);
                DM.ARAY[ri.RPCK].SetStat(cs.None);
                DM.ARAY[ri.VSTG].SetStat(cs.Empty);
            }
        }

        
        private void P1_Click(object sender, EventArgs e)
        {
            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);
            if(iBtnTag == 1) SEQ.PSTR.m_bVacSkip1 = !SEQ.PSTR.m_bVacSkip1 ? true : false;
            if(iBtnTag == 2) SEQ.PSTR.m_bVacSkip2 = !SEQ.PSTR.m_bVacSkip2 ? true : false;
            if(iBtnTag == 3) SEQ.PSTR.m_bVacSkip3 = !SEQ.PSTR.m_bVacSkip3 ? true : false;
            if(iBtnTag == 4) SEQ.PSTR.m_bVacSkip4 = !SEQ.PSTR.m_bVacSkip4 ? true : false;
            if(iBtnTag == 5) SEQ.PSTR.m_bVacSkip5 = !SEQ.PSTR.m_bVacSkip5 ? true : false;
            if(iBtnTag == 6) SEQ.PSTR.m_bVacSkip6 = !SEQ.PSTR.m_bVacSkip6 ? true : false;

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