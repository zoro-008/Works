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

            //모터 축 수에 맞춰 FrameMotr 생성
            FraMotr = new FraMotr[(int)mi.MAX_MOTR];
            for (int m = 0; m < (int)mi.MAX_MOTR; m++)
            {
                Control[] Ctrl = tcDeviceSet.Controls.Find("pnMotrJog" +  m.ToString(), true);

                MOTION_DIR eDir = ML.MT_GetDirType((mi)m);
                FraMotr[m] = new FraMotr();
                FraMotr[m].SetIdType((mi)m, eDir);
                FraMotr[m].TopLevel = false;
                FraMotr[m].Parent = Ctrl[0];
                FraMotr[m].Show();
                FraMotr[m].SetUnit(EN_UNIT_TYPE.utJog, 0);
            }
            
            FraCylAPT = new FrameCylinderAPT[(int)ci.MAX_ACTR];
            for (int i = 0; i < (int)ci.MAX_ACTR; i++)
            {
                Control[] CtrlAP = tcDeviceSet.Controls.Find("C" + i.ToString(), true);

                //int iCylCtrl = Convert.ToInt32(CtrlAP[0].Tag);
                int iCylCtrl = Convert.ToInt32(i);
                FraCylAPT[i] = new FrameCylinderAPT();
                FraCylAPT[i].TopLevel = false;
                FraCylAPT[i].SetConfig((ci)iCylCtrl, ML.CL_GetName(iCylCtrl).ToString(), ML.CL_GetDirType((ci)iCylCtrl), CtrlAP[0]);
                FraCylAPT[i].Show();
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

            //Input Status 생성 AP텍꺼
            const int iInputBtnCnt  = 26;
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
            const int iOutputBtnCnt = 48;
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

            //Rail 포지션 탭페이지 제거
            tabControl3.TabPages.Remove(tabPage19);
            tabControl12.TabPages.Remove(tabPage20);
        }

        public void PstnDisp()
        {
            //PRER_X
            PM.SetProp((uint)mi.PRER_X   , (uint)pv.PRER_XWait      , "Wait     ", false, false, false  );
            //PSTR_X
            PM.SetProp((uint)mi.PSTR_X   , (uint)pv.PSTR_XWait      , "Wait     ", false, false, false  );
            //PCKL_Y
            PM.SetProp((uint)mi.LPCK_Y   , (uint)pv.LPCK_YWait      , "Wait     ", false, false, false  );
            PM.SetProp((uint)mi.LPCK_Y   , (uint)pv.LPCK_YPick      , "Pick     ", false, false, false  );
            PM.SetProp((uint)mi.LPCK_Y   , (uint)pv.LPCK_YCleanStt  , "CleanStt ", false, false, false  );
            PM.SetProp((uint)mi.LPCK_Y   , (uint)pv.LPCK_YCleanEnd  , "CleanEnd ", false, false, false  );
            PM.SetProp((uint)mi.LPCK_Y   , (uint)pv.LPCK_YPlace     , "Place    ", false, false, false  );
            //PCKR_Y
            PM.SetProp((uint)mi.RPCK_Y   , (uint)pv.RPCK_YWait      , "Wait     ", false, false, false  );
            PM.SetProp((uint)mi.RPCK_Y   , (uint)pv.RPCK_YPick      , "Pick     ", false, false, false  );
            PM.SetProp((uint)mi.RPCK_Y   , (uint)pv.RPCK_YPlace     , "Place    ", false, false, false  );
            //VSTG_X
            PM.SetProp((uint)mi.VSTG_X   , (uint)pv.VSTG_XWait      , "Wait     ", false, false, false  );
            PM.SetProp((uint)mi.VSTG_X   , (uint)pv.VSTG_XWorkStt   , "WorkStt  ", false, false, false  );
            PM.SetProp((uint)mi.VSTG_X   , (uint)pv.VSTG_XCleanStt  , "CleanStt ", false, false, false  );
            PM.SetProp((uint)mi.VSTG_X   , (uint)pv.VSTG_XCleanEnd  , "CleanEnd ", false, false, false  );
            PM.SetProp((uint)mi.VSTG_X   , (uint)pv.VSTG_XWorkEnd   , "WorkEnd  ", false, false, false  );
            
            PM.SetCheckSafe((uint)mi.PRER_X,SEQ.PRER.CheckSafe);
            PM.SetCheckSafe((uint)mi.PSTR_X,SEQ.PSTR.CheckSafe);
            PM.SetCheckSafe((uint)mi.LPCK_Y,SEQ.LPCK.CheckSafe);
            PM.SetCheckSafe((uint)mi.RPCK_Y,SEQ.RPCK.CheckSafe);
            PM.SetCheckSafe((uint)mi.VSTG_X,SEQ.VSTG.CheckSafe);
        }
             
        private void tcDeviceSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            int iSeletedIndex;
            iSeletedIndex = tcDeviceSet.SelectedIndex;
            
            switch (iSeletedIndex)
            {
                default : break;
                case 0  : gbJogUnit.Parent = pnJog0; break;
                case 1  : gbJogUnit.Parent = pnJog1; break;
                case 2  : gbJogUnit.Parent = pnJog2; break;
            }

            PM.UpdatePstn(true);
            PM.Load(OM.GetCrntDev());
        }

        public void UpdateDevInfo(bool bToTable)
        {
            if (bToTable)
            {
                CConfig.ValToCon(dRailClnSpeed , ref OM.DevInfo.dRailClnSpeed );
                CConfig.ValToCon(dLPCKClnSpeed , ref OM.DevInfo.dLPCKClnSpeed );
                CConfig.ValToCon(iLPCKBfPickDly, ref OM.DevInfo.iLPCKBfPickDly);
                CConfig.ValToCon(iLPCKPickDly  , ref OM.DevInfo.iLPCKPickDly  );
                CConfig.ValToCon(iLPCKPlaceDly , ref OM.DevInfo.iLPCKPlaceDly );
                CConfig.ValToCon(iRPCKPickDly  , ref OM.DevInfo.iRPCKPickDly  ); 
                CConfig.ValToCon(iRPCKPlaceDly , ref OM.DevInfo.iRPCKPlaceDly );
                CConfig.ValToCon(dVSTGClnSpeed , ref OM.DevInfo.dVSTGClnSpeed );
                CConfig.ValToCon(iPSTR_OutDelay, ref OM.DevInfo.iPSTR_OutDelay);
            }
            else 
            {
                OM.CDevInfo DevInfo = OM.DevInfo;

                CConfig.ConToVal(dRailClnSpeed , ref OM.DevInfo.dRailClnSpeed );
                CConfig.ConToVal(dLPCKClnSpeed , ref OM.DevInfo.dLPCKClnSpeed );
                CConfig.ConToVal(iLPCKBfPickDly, ref OM.DevInfo.iLPCKBfPickDly);
                CConfig.ConToVal(iLPCKPickDly  , ref OM.DevInfo.iLPCKPickDly  );
                CConfig.ConToVal(iLPCKPlaceDly , ref OM.DevInfo.iLPCKPlaceDly );
                CConfig.ConToVal(iRPCKPickDly  , ref OM.DevInfo.iRPCKPickDly  );
                CConfig.ConToVal(iRPCKPlaceDly , ref OM.DevInfo.iRPCKPlaceDly );
                CConfig.ConToVal(dVSTGClnSpeed , ref OM.DevInfo.dVSTGClnSpeed );
                CConfig.ConToVal(iPSTR_OutDelay, ref OM.DevInfo.iPSTR_OutDelay);

                //Auto Log
                Type type = DevInfo.GetType();
                FieldInfo[] f = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                for (int i = 0; i < f.Length; i++)
                {
                    Trace(f[i].Name, f[i].GetValue(DevInfo).ToString(), f[i].GetValue(OM.DevInfo).ToString());
                }

                UpdateDevInfo(true);
            }
        
        }

        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            tmUpdate.Enabled = false;

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
            //if (!double.TryParse(tbUserUnit.Text, out dUserDefine)) dUserDefine = 0.0;

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
            //if (!rbUserUnit.Checked) return;
            //
            //double dUserDefine = 0.0;
            //if (!double.TryParse(tbUserUnit.Text, out dUserDefine)) dUserDefine = 0.0;
            //
            //for (int m = 0; m < (int)mi.MAX_MOTR; m++)
            //{
            //
            //    FraMotr[m].SetUnit(EN_UNIT_TYPE.utMove, dUserDefine);
            //
            //}
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

        private void btSavePosition_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            if (Log.ShowMessageModal("Confirm", "Are you Sure?") != DialogResult.Yes) return;

            PM.UpdatePstn(false);
            PM.Save(OM.GetCrntDev());
            PM.UpdatePstn(true);

            Refresh();
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
        
        private void btSaveDevice_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            if (Log.ShowMessageModal("Confirm", "Are you Sure?") != DialogResult.Yes) return;


            UpdateDevInfo(false);

            OM.SaveJobFile(OM.GetCrntDev().ToString());

            ////Loader.
            //DM.ARAY[ri.LODR].SetMaxColRow(1 , OM.DevInfo.iMgzSlotCnt);
            //
            ////Prebuffer
            //DM.ARAY[ri.PREB].SetMaxColRow(OM.DevInfo.iColCnt, OM.DevInfo.iRowCnt );
            //
            ////Vision1
            //DM.ARAY[ri.VSN1].SetMaxColRow(OM.DevInfo.iColCnt, OM.DevInfo.iRowCnt );
            //DM.ARAY[ri.RLT1].SetMaxColRow(OM.DevInfo.iColCnt, OM.DevInfo.iRowCnt );
            //DM.ARAY[ri.WRK1].SetMaxColRow(OM.DevInfo.iColCnt, OM.DevInfo.iRowCnt );
            //
            ////Vision2
            //DM.ARAY[ri.VSN2].SetMaxColRow(OM.DevInfo.iColCnt, OM.DevInfo.iRowCnt );
            //DM.ARAY[ri.RLT2].SetMaxColRow(OM.DevInfo.iColCnt, OM.DevInfo.iRowCnt );
            //DM.ARAY[ri.WRK2].SetMaxColRow(OM.DevInfo.iColCnt, OM.DevInfo.iRowCnt );
            //
            ////Vision3
            //DM.ARAY[ri.VSN3].SetMaxColRow(OM.DevInfo.iColCnt, OM.DevInfo.iRowCnt );
            //DM.ARAY[ri.RLT3].SetMaxColRow(OM.DevInfo.iColCnt, OM.DevInfo.iRowCnt );
            //DM.ARAY[ri.WRK3].SetMaxColRow(OM.DevInfo.iColCnt, OM.DevInfo.iRowCnt );
            //
            ////PostBuffer
            //DM.ARAY[ri.PSTB].SetMaxColRow(OM.DevInfo.iColCnt, OM.DevInfo.iRowCnt );
            //
            ////Unloader
            //DM.ARAY[ri.ULDR].SetMaxColRow(1 , OM.DevInfo.iMgzSlotCnt);

            
            Refresh();
        }

        private void label25_Click(object sender, EventArgs e)
        {

        }

        private void label40_Click(object sender, EventArgs e)
        {

        }

        private void label41_Click(object sender, EventArgs e)
        {

        }

        private void C10_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

        }

        private void btBreakOnOff_Click(object sender, EventArgs e)
        {
            const int iBreakAdd = 3 ;
            //bool bBreakStat = ML.  MT_GetY(mi.LODR_ZClmp , iBreakAdd) ;
            //ML.MT_SetY(mi.LODR_ZClmp , iBreakAdd , !bBreakStat);
        }

        private void btULDBreakOnOff_Click(object sender, EventArgs e)
        {
            const int iBreakAdd = 3 ;
            //bool bBreakStat = ML.  MT_GetY(mi.ULDR_ZClmp , iBreakAdd) ;
            //ML.MT_SetY(mi.ULDR_ZClmp , iBreakAdd , !bBreakStat);
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
