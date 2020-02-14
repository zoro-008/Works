using System;
using System.Drawing;
using System.Windows.Forms;
using COMMON;
using SML;
using System.Reflection;

namespace Machine
{
    public partial class FormDeviceSet : Form
    {
        public        FraMotr    []       FraMotr      ;
        //public        FraCylOneBt[]       FraCylinder  ;
        //public        FraOutput  []       FraOutput    ;
        //public        FormMain            FrmMain      ;
        //여기 AP텍꺼
        public        FrameCylinderAPT []  FraCylAPT    ;
        public        FrameInputAPT    []  FraInputAPT  ;
        public        FrameOutputAPT   []  FraOutputAPT ;
        public        FrameMotrPosAPT  []  FraMotrPosAPT;

        private const string sFormText = "Form DeviceSet ";

        public FormDeviceSet(Panel _pnBase)
        {
            InitializeComponent();            
            this.Width = 1272;
            this.Height = 866;

            this.TopLevel = false;
            this.Parent = _pnBase;

            tbUserUnit.Text = 0.01.ToString();
            PstnDisp();

            OM.LoadLastInfo();
            PM.Load(OM.GetCrntDev().ToString());

            PM.UpdatePstn(true);
            UpdateDevInfo(true);
            
            //DM.ARAY[ri.MASK].SetParent(pnTrayMask); DM.ARAY[ri.MASK].Name = "MASK";
            //LoadMask(OM.GetCrntDev().ToString());            
            //DM.ARAY[ri.MASK].SetDisp(cs.Empty,"Empty",Color.Silver);
            //DM.ARAY[ri.MASK].SetDisp(cs.None ,"None" ,Color.White );            
           
            FraMotr     = new FraMotr    [(int)mi.MAX_MOTR];
            //FraCylinder = new FraCylOneBt[(int)ci.MAX_ACTR  ];

            //모터 축 수에 맞춰 FrameMotr 생성
            for (int m = 0; m < (int)mi.MAX_MOTR; m++)
            {
                Control[] Ctrl = tcDeviceSet.Controls.Find("pnMotrJog" +  m.ToString(), true);

                MOTION_DIR eDir = ML.MT_GetDirType(m);
                FraMotr[m] = new FraMotr();
                FraMotr[m].SetIdType((mi)m, eDir);
                FraMotr[m].TopLevel = false;
                FraMotr[m].Parent = Ctrl[0];
                FraMotr[m].Show();
                FraMotr[m].SetUnit(EN_UNIT_TYPE.utJog, 0);
            }

            //여기 AP텍에서만 쓰는거
            //실린더 버튼 AP텍꺼
            const int iCylinderCnt = 30;
            FraCylAPT = new FrameCylinderAPT[iCylinderCnt];
            for (int i = 0; i < iCylinderCnt; i++)
            {
                Control[] CtrlAP = tcDeviceSet.Controls.Find("C" + i.ToString(), true);

                int iCylCtrl = Convert.ToInt32(CtrlAP[0].Tag);
                FraCylAPT[i] = new FrameCylinderAPT();
                FraCylAPT[i].TopLevel = false;
                FraCylAPT[i].SetConfig((ci)iCylCtrl, SM.CYL.GetName(iCylCtrl).ToString(), ML.CL_GetDirType((ci)iCylCtrl), CtrlAP[0]);
                FraCylAPT[i].Show();
            }

            //Input Status 생성 AP텍꺼
            const int iInputBtnCnt  = 12;
            FraInputAPT = new FrameInputAPT[iInputBtnCnt];
            for (int i = 0; i < iInputBtnCnt; i++)
            {
                Control[] Ctrl = tcDeviceSet.Controls.Find("X" + i.ToString(), true);
                
                int iIOCtrl = Convert.ToInt32(Ctrl[0].Tag);

                FraInputAPT[i] = new FrameInputAPT();
                FraInputAPT[i].TopLevel = false;
                FraInputAPT[i].SetConfig((xi)iIOCtrl, ML.IO_GetXName((xi)iIOCtrl), Ctrl[0]);
                FraInputAPT[i].Show();
            }

            //Output Status 생성 AP텍꺼
            const int iOutputBtnCnt = 1;
            FraOutputAPT = new FrameOutputAPT[iOutputBtnCnt];
            for (int i = 0; i < iOutputBtnCnt; i++)
            {
                Control[] Ctrl = tcDeviceSet.Controls.Find("Y" + i.ToString(), true);
            
                int iIOCtrl = Convert.ToInt32(Ctrl[0].Tag);

                FraOutputAPT[i] = new FrameOutputAPT();
                FraOutputAPT[i].TopLevel = false;
                FraOutputAPT[i].SetConfig((yi)iIOCtrl, ML.IO_GetYName((yi)iIOCtrl), Ctrl[0]);
                FraOutputAPT[i].Show();
            
               // FraOutputAPT[i].Show();
            }

            //모터 포지션 AP텍꺼
            FraMotrPosAPT = new FrameMotrPosAPT[(int)mi.MAX_MOTR];
            for (int i = 0; i < (int)mi.MAX_MOTR; i++)
            {
                Control[] Ctrl = tcDeviceSet.Controls.Find("pnMotrPos" + i.ToString(), true);

                FraMotrPosAPT[i] = new FrameMotrPosAPT();
                FraMotrPosAPT[i].TopLevel = false;
                FraMotrPosAPT[i].SetWindow(i, Ctrl[0]);
                FraMotrPosAPT[i].Show();
            }
        }

        public void PstnDisp()
        {
            //TBLE_TTlbe
            PM.SetProp((uint)mi.TBLE_TTble, (uint)pv.TBLE_TTbleWait        , "Wait            ", false, false, false  );
            PM.SetProp((uint)mi.TBLE_TTble, (uint)pv.TBLE_TTbleWorkPitch   , "WorkPitch       ", true , false, true   );
            
            //LODR_XPckr
            PM.SetProp((uint)mi.LODR_XPckr, (uint)pv.LODR_XPckrWait        , "Wait           ", false , false, false );
            PM.SetProp((uint)mi.LODR_XPckr, (uint)pv.LODR_XPckrPick        , "Pick           ", false , false, false );
            PM.SetProp((uint)mi.LODR_XPckr, (uint)pv.LODR_XPckrPickRtt     , "PickRtt        ", false , false, false );
            PM.SetProp((uint)mi.LODR_XPckr, (uint)pv.LODR_XPckrPlce        , "Place          ", false , false, false );
            PM.SetProp((uint)mi.LODR_XPckr, (uint)pv.LODR_XPckrPlceRtt     , "PlaceRtt       ", false , false, false );
                                                                          
            //VISN_ZGrpr                     
            PM.SetProp((uint)mi.VISN_ZGrpr, (uint)pv.VISN_ZGrprWait        , "Wait           ", false , false, false );
            PM.SetProp((uint)mi.VISN_ZGrpr, (uint)pv.VISN_ZGrprWork        , "Work           ", false , false, false );
             
            //ULDR_ZPckr                      
            PM.SetProp((uint)mi.ULDR_ZPckr, (uint)pv.ULDR_ZPckrWait        , "Wait           ", false , false, false );
            PM.SetProp((uint)mi.ULDR_ZPckr, (uint)pv.ULDR_ZPckrPick        , "Pick           ", false , false, false );
            PM.SetProp((uint)mi.ULDR_ZPckr, (uint)pv.ULDR_ZPckrPlace       , "Place          ", false , false, false );
              
            //ULDR_XGrpr                     
            PM.SetProp((uint)mi.ULDR_XGrpr, (uint)pv.ULDR_XGrprWait        , "Wait           ", false , false, false );
            PM.SetProp((uint)mi.ULDR_XGrpr, (uint)pv.ULDR_XGrprPick        , "Pick           ", false , false, false );
            PM.SetProp((uint)mi.ULDR_XGrpr, (uint)pv.ULDR_XGrprPlace       , "Place          ", false , false, false );
             
            //LODR_YIndx                      
            PM.SetProp((uint)mi.LODR_YIndx, (uint)pv.LODR_YIndxWait        , "Wait           ", false , false, false );
            PM.SetProp((uint)mi.LODR_YIndx, (uint)pv.LODR_YIndxWork        , "Work           ", false , false, false );

             
            //LODR_XPshr                  
            PM.SetProp((uint)mi.LODR_XPshr, (uint)pv.LODR_XPshrWait        , "Wait           ", false , false, false );
            PM.SetProp((uint)mi.LODR_XPshr, (uint)pv.LODR_XPshrWorkStt     , "WorkStt        ", false , false, false );
            PM.SetProp((uint)mi.LODR_XPshr, (uint)pv.LODR_XPshrWorkEnd     , "WorkEnd        ", false , false, false );
            PM.SetProp((uint)mi.LODR_XPshr, (uint)pv.LODR_XPshrBackOfs     , "BackOfs        ", true  , false, false );
            
            //ULDR_YIndx                               
            PM.SetProp((uint)mi.ULDR_YIndx, (uint)pv.ULDR_YIndxWait        , "Wait           ", false , false, false );
            PM.SetProp((uint)mi.ULDR_YIndx, (uint)pv.ULDR_YIndxWork        , "Work           ", false , false, false );
               
            //ULDR_ZNzzl              
            PM.SetProp((uint)mi.ULDR_ZNzzl, (uint)pv.ULDR_ZNzzlWait        , "Wait           ", false , false, false );
            PM.SetProp((uint)mi.ULDR_ZNzzl, (uint)pv.ULDR_ZNzzlWork        , "Work           ", false , false, false );

            
            //ULDR_XNzzl
            PM.SetProp((uint)mi.ULDR_XNzzl, (uint)pv.ULDR_XNzzlWait        , "Wait           ", false , false, false );
            PM.SetProp((uint)mi.ULDR_XNzzl, (uint)pv.ULDR_XNzzlWorkStt     , "WorkStt        ", false , false, false );
            PM.SetProp((uint)mi.ULDR_XNzzl, (uint)pv.ULDR_XNzzlWorkEnd     , "WorkEnd        ", false , false, false );
        }
             
        private void tcDeviceSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            int iSeletedIndex;
            iSeletedIndex = tcDeviceSet.SelectedIndex;
            
            switch (iSeletedIndex)
            {
                default : break;
                case 0  : gbJogUnit.Parent = pnJog1;                       break;
                case 1  : gbJogUnit.Parent = pnJog2;                       break;
                case 2  : gbJogUnit.Parent = pnJog3;                       break;
                case 3  : gbJogUnit.Parent = pnJog4;                       break;
                case 4  : gbJogUnit.Parent = pnJog5;                       break;
                case 5  : gbJogUnit.Parent = pnJog6;                       break;
            }

            PM.UpdatePstn(true);
            //PM.Load(OM.GetCrntDev());
        }

        public void UpdateDevInfo(bool bToTable)
        {
            if (bToTable)
            {
                CConfig.ValToCon(tbLODR_BarCntY     , ref OM.DevInfo.iLODR_BarCnt   );
                CConfig.ValToCon(tbULDR_BarCntY     , ref OM.DevInfo.iULDR_BarCnt   );
                CConfig.ValToCon(tbLODR_BarPitchY   , ref OM.DevInfo.dLODR_BarPitch );
                CConfig.ValToCon(tbULDR_BarPitchY   , ref OM.DevInfo.dULDR_BarPitch );
                CConfig.ValToCon(tbWorkEndCnt       , ref OM.DevInfo.iWorkEndCnt    );
                CConfig.ValToCon(tbPickDelay        , ref OM.DevInfo.iPickDelay     );                
                CConfig.ValToCon(tbPickMissCnt      , ref OM.DevInfo.iPickMissCnt   );                
                CConfig.ValToCon(nrLSR_ProgramNo    , ref OM.DevInfo.iProgramNo     );
            }
            else 
            {
                OM.CDevInfo DevInfo = OM.DevInfo;
                CConfig.ConToVal(tbLODR_BarCntY     , ref OM.DevInfo.iLODR_BarCnt   );
                CConfig.ConToVal(tbULDR_BarCntY     , ref OM.DevInfo.iULDR_BarCnt   );
                CConfig.ConToVal(tbLODR_BarPitchY   , ref OM.DevInfo.dLODR_BarPitch );
                CConfig.ConToVal(tbULDR_BarPitchY   , ref OM.DevInfo.dULDR_BarPitch );
                CConfig.ConToVal(tbWorkEndCnt       , ref OM.DevInfo.iWorkEndCnt    );
                CConfig.ConToVal(tbPickDelay        , ref OM.DevInfo.iPickDelay     );
                CConfig.ConToVal(tbPickMissCnt      , ref OM.DevInfo.iPickMissCnt   );                
                CConfig.ConToVal(nrLSR_ProgramNo    , ref OM.DevInfo.iProgramNo     );

                //Auto Log
                Type type = DevInfo.GetType();
                FieldInfo[] f = type.GetFields(BindingFlags.Public|BindingFlags.NonPublic | BindingFlags.Instance);
                for(int i = 0 ; i < f.Length ; i++){
                    Trace(f[i].Name, f[i].GetValue(DevInfo).ToString(), f[i].GetValue(OM.DevInfo).ToString());
                }

                UpdateDevInfo(true);
            }
        
        }




        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            tmUpdate.Enabled = false;
            btSaveDevice.Enabled = !SEQ._bRun;

            if(rbPaintNo1.Text != OM.CmnOptn.sPaintName1){ rbPaintNo1.Text = OM.CmnOptn.sPaintName1; }
            if(rbPaintNo2.Text != OM.CmnOptn.sPaintName2){ rbPaintNo2.Text = OM.CmnOptn.sPaintName2; }
            if(rbPaintNo3.Text != OM.CmnOptn.sPaintName3){ rbPaintNo3.Text = OM.CmnOptn.sPaintName3; }
            if(rbPaintNo4.Text != OM.CmnOptn.sPaintName4){ rbPaintNo4.Text = OM.CmnOptn.sPaintName4; }
            if(rbPaintNo5.Text != OM.CmnOptn.sPaintName5){ rbPaintNo5.Text = OM.CmnOptn.sPaintName5; }


            //pnTrayMask.Refresh();

            if (!this.Visible)
            {
                tmUpdate.Enabled = false;
                return;
            }
            tmUpdate.Enabled = true;
        }

        private void rbJog_Click(object sender, EventArgs e)
        {
            int iUnit = Convert.ToInt32(((RadioButton)sender).Tag);

            double dUserDefine = 0.0;
            if (!double.TryParse(tbUserUnit.Text, out dUserDefine)) dUserDefine = 0.0;

            for (int m = 0; m < (int)mi.MAX_MOTR; m++)
            {
                switch (iUnit)
                {
                    default: FraMotr[m].SetUnit(EN_UNIT_TYPE.utJog , 0d         ); break;
                    case 1 : FraMotr[m].SetUnit(EN_UNIT_TYPE.utMove, 1d         ); break;
                    case 2 : FraMotr[m].SetUnit(EN_UNIT_TYPE.utMove, 0.5d       ); break;
                    case 3 : FraMotr[m].SetUnit(EN_UNIT_TYPE.utMove, 0.1d       ); break;
                    case 4 : FraMotr[m].SetUnit(EN_UNIT_TYPE.utMove, 0.05d      ); break;
                    case 5 : FraMotr[m].SetUnit(EN_UNIT_TYPE.utMove, dUserDefine); break;
                }
            }
        }

        private void tbUserUnit_TextChanged(object sender, EventArgs e)
        {
            if (!rbUserUnit.Checked) return;

            double dUserDefine = 0.0;
            if (!double.TryParse(tbUserUnit.Text, out dUserDefine)) dUserDefine = 0.0;

            for (int m = 0; m < (int)mi.MAX_MOTR; m++)
            {

                FraMotr[m].SetUnit(EN_UNIT_TYPE.utMove, dUserDefine);

            }
        }

        private void FormDeviceSet_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmUpdate.Enabled = false;
        }
        
        private void btManual1_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);
            MM.SetManCycle((mc)iBtnTag);
        }

        private void FormDeviceSet_Shown(object sender, EventArgs e)
        {
            tmUpdate.Enabled = true;
        }
        
        private void btSaveDevice_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            if (Log.ShowMessageModal("Confirm", "Are you Sure?") != DialogResult.Yes) return;

            UpdateDevInfo(false);

            OM.SaveDevInfo(OM.GetCrntDev().ToString());
            OM.SaveEqpOptn();

            //SaveMask(OM.GetCrntDev());

            DM.ARAY[ri.LODR].SetMaxColRow(1, OM.DevInfo.iLODR_BarCnt);
            DM.ARAY[ri.PLDR].SetMaxColRow(1, 1);
            DM.ARAY[ri.TLDR].SetMaxColRow(1, 1);
            DM.ARAY[ri.TVSN].SetMaxColRow(1, 1);
            DM.ARAY[ri.TMRK].SetMaxColRow(1, 1);
            DM.ARAY[ri.TULD].SetMaxColRow(1, 1);
            DM.ARAY[ri.TRJM].SetMaxColRow(1, 1);
            DM.ARAY[ri.TRJV].SetMaxColRow(1, 1);
            DM.ARAY[ri.PULD].SetMaxColRow(1, 1);
            DM.ARAY[ri.ULDR].SetMaxColRow(1, OM.DevInfo.iULDR_BarCnt);
            DM.ARAY[ri.PICK].SetMaxColRow(1, 1);
            DM.ARAY[ri.PSHR].SetMaxColRow(1, 1);
            //           DM.ARAY[ri.BPCK].SetMaxColRow(1                        , 1                           );

            MM.SetManCycle(mc.MARK_CycleManChage);
            //Refresh();
            Invalidate(true);
        }

        private void btSavePosition_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            if (Log.ShowMessageModal("Confirm", "Are you Sure?") != DialogResult.Yes) return;

            PM.UpdatePstn(false);
            PM.Save(OM.GetCrntDev());
            PM.UpdatePstn(true);

            //Refresh();
            Invalidate(true);
        }

        private void FormDeviceSet_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible) tmUpdate.Enabled = true;
        }
    
        private void Trace(string sText, string _s1, string _s2)
        {
            Log.Trace(sText.Trim() + " : " + _s1 + " -> " + _s2,ti.Dev);
        }
        private void Trace(string sText, int _s1, int _s2)
        {
            Log.Trace(sText.Trim() + " : " + _s1.ToString() + " -> " + _s2.ToString(),ti.Dev);
        }
        private void Trace(string sText, double _s1, double _s2)
        {
            Log.Trace(sText.Trim() + " : " + _s1.ToString() + " -> " + _s2.ToString(),ti.Dev);
        }
        private void Trace(string sText, bool _s1, bool _s2)
        {
            Log.Trace(sText.Trim() + " : " + _s1.ToString() + " -> " + _s2.ToString(),ti.Dev);
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void rbPaintNo_Click(object sender, EventArgs e)
        {
            int iUnit = Convert.ToInt32(((RadioButton)sender).Tag);

            switch (iUnit)
            {
                default: OM.DevInfo.iPaintColorNo = 0 ; break;
                case 1 : OM.DevInfo.iPaintColorNo = 1 ; break;
                case 2 : OM.DevInfo.iPaintColorNo = 2 ; break;
                case 3 : OM.DevInfo.iPaintColorNo = 3 ; break;
                case 4 : OM.DevInfo.iPaintColorNo = 4 ; break;
            }

        }

        private void groupBox12_Enter(object sender, EventArgs e)
        {

        }


        //private void pbSTG_Paint(object sender, PaintEventArgs e)
        //{
        //    int iTag = Convert.ToInt32(((PictureBox)sender).Tag);

        //    SolidBrush Brush = new SolidBrush(Color.Black);

        //    Pen Pen = new Pen(Color.Black);

        //    Graphics gSTG = pbTRAY.CreateGraphics();


        //    double dX1, dX2, dY1, dY2;

        //    int iTrayColCnt, iTrayRowCnt;
           
        //    Graphics g = e.Graphics;

        //    switch (iTag)
        //    {
        //        default: break;
        //        case 1: 
        //            iTrayColCnt = OM.DevInfo.iTRAY_PcktCntX;
        //            iTrayRowCnt = OM.DevInfo.iTRAY_PcktCntY;

        //            int iGetWSTGWidth = pbTRAY.Width;
        //            int iGetWSTGHeight = pbTRAY.Height;

        //            double iSetWSTGWidth = 0, iSetWSTGHeight = 0;

        //            double uWSTGGw = (double)iGetWSTGWidth  / (double)(iTrayColCnt);
        //            double uWSTGGh = (double)iGetWSTGHeight / (double)(iTrayRowCnt);
        //            double dWSTGWOff = (double)(iGetWSTGWidth - uWSTGGw * (iTrayColCnt)) / 2.0;
        //            double dWSTGHOff = (double)(iGetWSTGHeight - uWSTGGh * (iTrayRowCnt)) / 2.0;

        //            Pen.Color = Color.Black;

        //            Brush.Color = Color.HotPink;


        //            for (int r = 0; r < iTrayRowCnt; r++)
        //            {
        //                for (int c = 0; c < iTrayColCnt; c++)
        //                {

        //                    dY1 = dWSTGHOff + r * uWSTGGh - 1;
        //                    dY2 = dWSTGHOff + r * uWSTGGh + uWSTGGh;
        //                    dX1 = dWSTGWOff + c * uWSTGGw - 1;
        //                    dX2 = dWSTGWOff + c * uWSTGGw + uWSTGGw;

        //                    g.FillRectangle(Brush, (float)dX1, (float)dY1, (float)dX2, (float)dY2);
        //                    g.DrawRectangle(Pen, (float)dX1, (float)dY1, (float)dX2, (float)dY2);

        //                    iSetWSTGWidth += dY2;
        //                    iSetWSTGHeight += dX2;
        //                }

        //            }

        //            break;


        //    }
        //}

        //public static void LoadMask(string _sJobName)
        //{
        //    CConfig Config = new CConfig();
        //    string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
        //    string sDevOptnPath = sExeFolder + "JobFile\\" + _sJobName + "\\TrayMask.ini";

        //    Config.Load(sDevOptnPath, CConfig.EN_CONFIG_FILE_TYPE.ftIni);
        //    DM.ARAY[ri.MASK].Load(Config, true);
        //}        
        //public static void SaveMask(string _sJobName)
        //{
        //    CConfig Config = new CConfig();
        //    string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
        //    string sDevOptnPath = sExeFolder + "JobFile\\" + _sJobName + "\\TrayMask.ini";

        //    DM.ARAY[ri.MASK].Load(Config, false);
        //    Config.Save(sDevOptnPath, CConfig.EN_CONFIG_FILE_TYPE.ftIni);
        //}
    }
}
