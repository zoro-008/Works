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
            const int iInputBtnCnt  = 17;
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
            const int iOutputBtnCnt = 8;
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
        }

        public void PstnDisp()
        {
            //HEAD_XVisn
            PM.SetProp((uint)mi.HEAD_XVisn   , (uint)pv.HEAD_XVisnWait       , "Wait      ", false, false, false  );
            PM.SetProp((uint)mi.HEAD_XVisn   , (uint)pv.HEAD_XVisnWorkStart  , "WorkStart ", false, false, false  );
            PM.SetProp((uint)mi.HEAD_XVisn   , (uint)pv.HEAD_XVisnRWorkStart , "RWorkStart", false, false, false  );
            //PM.SetProp((uint)mi.HEAD_XVisn   , (uint)pv.HEAD_XVisnLWorkEnd   , "LWorkEnd  ", false, false, false  );
            //HEAD_YVisn
            PM.SetProp((uint)mi.HEAD_YVisn   , (uint)pv.HEAD_YVisnWait       , "Wait      ", false, false, false  );
            PM.SetProp((uint)mi.HEAD_YVisn   , (uint)pv.HEAD_YVisnWorkStart  , "WorkStart ", false, false, false  );
            //PSTB_XMark
            PM.SetProp((uint)mi.PSTB_XMark   , (uint)pv.PSTB_XMarkWait       , "Wait      ", false, false, false  );
            PM.SetProp((uint)mi.PSTB_XMark   , (uint)pv.PSTB_XMarkWorkStart  , "WorkStart ", false, false, false  );
            PM.SetProp((uint)mi.PSTB_XMark   , (uint)pv.PSTB_XReplace        , "Replace   ", false, false, false  );
            //PSTB_YMark
            PM.SetProp((uint)mi.PSTB_YMark   , (uint)pv.PSTB_YMarkWait       , "Wait      ", false, false, false  );
            PM.SetProp((uint)mi.PSTB_YMark   , (uint)pv.PSTB_YMarkWorkStart  , "WorkStart ", false, false, false  );
            PM.SetProp((uint)mi.PSTB_YMark   , (uint)pv.PSTB_YReplace        , "Replace   ", false, false, false  );
            //HEAD_XCvr1
            PM.SetProp((uint)mi.HEAD_XCvr1   , (uint)pv.HEAD_XCvr1Wait       , "Wait      ", false, false, false  );
            PM.SetProp((uint)mi.HEAD_XCvr1   , (uint)pv.HEAD_XCvr1Work       , "Work      ", false, false, false  );
            //HEAD_XCvr2
            PM.SetProp((uint)mi.HEAD_XCvr2   , (uint)pv.HEAD_XCvr2Wait       , "Wait      ", false, false, false  );
            PM.SetProp((uint)mi.HEAD_XCvr2   , (uint)pv.HEAD_XCvr2Work       , "Work      ", false, false, false  );
            //HEAD_XCvr3
            PM.SetProp((uint)mi.HEAD_XCvr3   , (uint)pv.HEAD_XCvr3Wait       , "Wait      ", false, false, false  );
            PM.SetProp((uint)mi.HEAD_XCvr3   , (uint)pv.HEAD_XCvr3Work       , "Work      ", false, false, false  );
            //LODR_YClmp
            PM.SetProp((uint)mi.LODR_YClmp   , (uint)pv.LODR_YClmpWait       , "Wait      ", false, false, false  );
            PM.SetProp((uint)mi.LODR_YClmp   , (uint)pv.LODR_YClmpWork       , "Work      ", false, false, false  );
            PM.SetProp((uint)mi.LODR_YClmp   , (uint)pv.LODR_YClmpPick       , "Pick      ", false, false, false  );
            PM.SetProp((uint)mi.LODR_YClmp   , (uint)pv.LODR_YClmpPlace      , "Place     ", false, false, false  );
            //LODR_ZClmp
            PM.SetProp((uint)mi.LODR_ZClmp   , (uint)pv.LODR_ZClmpWait       , "Wait      ", false, false, false  );
            PM.SetProp((uint)mi.LODR_ZClmp   , (uint)pv.LODR_ZClmpPickFwd    , "PickFwd   ", false, false, false  );
            PM.SetProp((uint)mi.LODR_ZClmp   , (uint)pv.LODR_ZClmpClampOn    , "ClampOn   ", false, false, false  );
            PM.SetProp((uint)mi.LODR_ZClmp   , (uint)pv.LODR_ZClmpPickBwd    , "PickBwd   ", false, false, false  );
            PM.SetProp((uint)mi.LODR_ZClmp   , (uint)pv.LODR_ZClmpWorkStart  , "WorkStart ", false, false, false  );
            PM.SetProp((uint)mi.LODR_ZClmp   , (uint)pv.LODR_ZClmpPlaceFwd   , "PlaceFwd  ", false, false, false  );
            PM.SetProp((uint)mi.LODR_ZClmp   , (uint)pv.LODR_ZClmpClampOff   , "ClampOff  ", false, false, false  );
            PM.SetProp((uint)mi.LODR_ZClmp   , (uint)pv.LODR_ZClmpPlaceBwd   , "PlaceBwd  ", false, false, false  );
            //ULDR_YClmp
            PM.SetProp((uint)mi.ULDR_YClmp   , (uint)pv.ULDR_YClmpWait       , "Wait      ", false, false, false  );
            PM.SetProp((uint)mi.ULDR_YClmp   , (uint)pv.ULDR_YClmpWork       , "Work      ", false, false, false  );
            PM.SetProp((uint)mi.ULDR_YClmp   , (uint)pv.ULDR_YClmpPick       , "Pick      ", false, false, false  );
            PM.SetProp((uint)mi.ULDR_YClmp   , (uint)pv.ULDR_YClmpPlace      , "Place     ", false, false, false  );
            //ULDR_ZClmp
            PM.SetProp((uint)mi.ULDR_ZClmp   , (uint)pv.ULDR_ZClmpWait       , "Wait      ", false, false, false  );
            PM.SetProp((uint)mi.ULDR_ZClmp   , (uint)pv.ULDR_ZClmpPickFwd    , "PickFwd   ", false, false, false  );
            PM.SetProp((uint)mi.ULDR_ZClmp   , (uint)pv.ULDR_ZClmpClampOn    , "ClampOn   ", false, false, false  );
            PM.SetProp((uint)mi.ULDR_ZClmp   , (uint)pv.ULDR_ZClmpPickBwd    , "PickBwd   ", false, false, false  );
            PM.SetProp((uint)mi.ULDR_ZClmp   , (uint)pv.ULDR_ZClmpWorkStart  , "WorkStart ", false, false, false  );
            PM.SetProp((uint)mi.ULDR_ZClmp   , (uint)pv.ULDR_ZClmpPlaceFwd   , "PlaceFwd  ", false, false, false  );
            PM.SetProp((uint)mi.ULDR_ZClmp   , (uint)pv.ULDR_ZClmpClampOff   , "ClampOff  ", false, false, false  );
            PM.SetProp((uint)mi.ULDR_ZClmp   , (uint)pv.ULDR_ZClmpPlaceBwd   , "PlaceBwd  ", false, false, false  );


            PM.SetCheckSafe((uint)mi.HEAD_XVisn,SEQ.VSNZ.CheckSafe);
            PM.SetCheckSafe((uint)mi.HEAD_YVisn,SEQ.VSNZ.CheckSafe);
            PM.SetCheckSafe((uint)mi.PSTB_XMark,SEQ.PSTB.CheckSafe);
            PM.SetCheckSafe((uint)mi.PSTB_YMark,SEQ.PSTB.CheckSafe);
            PM.SetCheckSafe((uint)mi.HEAD_XCvr1,SEQ.VSNZ.CheckSafe);
            PM.SetCheckSafe((uint)mi.HEAD_XCvr2,SEQ.VSNZ.CheckSafe);
            PM.SetCheckSafe((uint)mi.HEAD_XCvr3,SEQ.VSNZ.CheckSafe);
            PM.SetCheckSafe((uint)mi.LODR_YClmp,SEQ.LODR.CheckSafe);
            PM.SetCheckSafe((uint)mi.LODR_ZClmp,SEQ.LODR.CheckSafe);
            PM.SetCheckSafe((uint)mi.ULDR_YClmp,SEQ.ULDR.CheckSafe);
            PM.SetCheckSafe((uint)mi.ULDR_ZClmp,SEQ.ULDR.CheckSafe);

        }
             
        private void tcDeviceSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            int iSeletedIndex;
            iSeletedIndex = tcDeviceSet.SelectedIndex;
            
            switch (iSeletedIndex)
            {
                default : break;
                case 0  : gbJogUnit.Parent = pnJog1; break;
                case 1  : gbJogUnit.Parent = pnJog2; break;
                case 2  : gbJogUnit.Parent = pnJog3; break;
                case 3  : gbJogUnit.Parent = pnJog4; break;
            }

            PM.UpdatePstn(true);
            PM.Load(OM.GetCrntDev());
        }

        public void UpdateDevInfo(bool bToTable)
        {
            if (bToTable)
            {
                CConfig.ValToCon(dColPitch      , ref OM.DevInfo.dColPitch       );
                CConfig.ValToCon(dRowPitch      , ref OM.DevInfo.dRowPitch       );
                CConfig.ValToCon(iColGrCnt      , ref OM.DevInfo.iColGrCnt       );
                CConfig.ValToCon(iRowGrCnt      , ref OM.DevInfo.iRowGrCnt       );
                CConfig.ValToCon(dColGrGap      , ref OM.DevInfo.dColGrGap       ); 
                CConfig.ValToCon(dRowGrGap      , ref OM.DevInfo.dRowGrGap       );
                CConfig.ValToCon(iColCnt        , ref OM.DevInfo.iColCnt         );
                CConfig.ValToCon(iRowCnt        , ref OM.DevInfo.iRowCnt         );

                CConfig.ValToCon(iColSbGrCnt    , ref OM.DevInfo.iColSbGrCnt     );
                CConfig.ValToCon(dColSbGrGap    , ref OM.DevInfo.dColSbGrGap     );
                CConfig.ValToCon(iRowSbGrCnt    , ref OM.DevInfo.iRowSbGrCnt     );
                CConfig.ValToCon(dRowSbGrGap    , ref OM.DevInfo.dRowSbGrGap     );

                CConfig.ValToCon(sVisnIndexId   , ref OM.DevInfo.sVisnIndexId    );
                CConfig.ValToCon(iColInspCnt    , ref OM.DevInfo.iColInspCnt     );
                CConfig.ValToCon(iRowInspCnt    , ref OM.DevInfo.iRowInspCnt     );

                CConfig.ValToCon(iMgzCntPerLot  , ref OM.DevInfo.iMgzCntPerLot   );
                CConfig.ValToCon(dMgzPitch      , ref OM.DevInfo.dMgzPitch       );
                CConfig.ValToCon(iMgzSlotCnt    , ref OM.DevInfo.iMgzSlotCnt     );
                CConfig.ValToCon(iLdrOutDelay   , ref OM.DevInfo.iLdrOutDelay    );
                CConfig.ValToCon(iUdrOutDelay   , ref OM.DevInfo.iUdrOutDelay    );
                CConfig.ValToCon(bVs1_Skip      , ref OM.DevInfo.bVs1_Skip       );
                CConfig.ValToCon(bVs2_Skip      , ref OM.DevInfo.bVs2_Skip       );
                CConfig.ValToCon(bVs3_Skip      , ref OM.DevInfo.bVs3_Skip       );
                CConfig.ValToCon(bVsL_NotUse    , ref OM.DevInfo.bVsL_NotUse     );
                CConfig.ValToCon(bVsR_NotUse    , ref OM.DevInfo.bVsR_NotUse     );
                
                CConfig.ValToCon(iStprDnDelay   , ref OM.DevInfo.iStprDnDelay    );
                

            }
            else 
            {
                OM.CDevInfo DevInfo = OM.DevInfo;

                CConfig.ConToVal(dColPitch      , ref OM.DevInfo.dColPitch       ,0.1,300);
                CConfig.ConToVal(dRowPitch      , ref OM.DevInfo.dRowPitch       ,0.1,300);
                CConfig.ConToVal(iColGrCnt      , ref OM.DevInfo.iColGrCnt       ,  1, 99);
                CConfig.ConToVal(iRowGrCnt      , ref OM.DevInfo.iRowGrCnt       ,  1, 99);
                CConfig.ConToVal(dColGrGap      , ref OM.DevInfo.dColGrGap       ,  0,300); 
                CConfig.ConToVal(dRowGrGap      , ref OM.DevInfo.dRowGrGap       ,  0,300);
                CConfig.ConToVal(iColCnt        , ref OM.DevInfo.iColCnt         ,  1, 99);
                CConfig.ConToVal(iRowCnt        , ref OM.DevInfo.iRowCnt         ,  1, 99);

                CConfig.ConToVal(iColSbGrCnt    , ref OM.DevInfo.iColSbGrCnt     ,  0, 99);
                CConfig.ConToVal(dColSbGrGap    , ref OM.DevInfo.dColSbGrGap     ,0.0,300);
                CConfig.ConToVal(iRowSbGrCnt    , ref OM.DevInfo.iRowSbGrCnt     ,  0, 99);
                CConfig.ConToVal(dRowSbGrGap    , ref OM.DevInfo.dRowSbGrGap     ,0.0,300);

                CConfig.ConToVal(sVisnIndexId   , ref OM.DevInfo.sVisnIndexId    );
                CConfig.ConToVal(iColInspCnt    , ref OM.DevInfo.iColInspCnt     ,  1, 99);
                CConfig.ConToVal(iRowInspCnt    , ref OM.DevInfo.iRowInspCnt     ,  1, 99);

                CConfig.ConToVal(iMgzCntPerLot  , ref OM.DevInfo.iMgzCntPerLot   ,  1, 99);
                CConfig.ConToVal(dMgzPitch      , ref OM.DevInfo.dMgzPitch       ,0.1,300);
                CConfig.ConToVal(iMgzSlotCnt    , ref OM.DevInfo.iMgzSlotCnt     ,  1, 99);
                CConfig.ConToVal(iLdrOutDelay   , ref OM.DevInfo.iLdrOutDelay    ,  0,5500);
                CConfig.ConToVal(iUdrOutDelay   , ref OM.DevInfo.iUdrOutDelay    ,  0,5500);

                CConfig.ConToVal(bVs1_Skip      , ref OM.DevInfo.bVs1_Skip       );
                CConfig.ConToVal(bVs2_Skip      , ref OM.DevInfo.bVs2_Skip       );
                CConfig.ConToVal(bVs3_Skip      , ref OM.DevInfo.bVs3_Skip       );
                CConfig.ConToVal(bVsL_NotUse    , ref OM.DevInfo.bVsL_NotUse     );
                CConfig.ConToVal(bVsR_NotUse    , ref OM.DevInfo.bVsR_NotUse     );

                CConfig.ConToVal(iStprDnDelay   , ref OM.DevInfo.iStprDnDelay    );

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
            Log.Trace(sFormText + sText + " Button Clicked", ForContext.Frm);

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
            Log.Trace(sFormText + sText + " Button Clicked", ForContext.Frm);

            if (Log.ShowMessageModal("Confirm", "Are you Sure?") != DialogResult.Yes) return;

            PM.UpdatePstn(false);
            PM.Save(OM.GetCrntDev());
            PM.UpdatePstn(true);

            Refresh();
        }

        private void FormDeviceSet_VisibleChanged(object sender, EventArgs e)
        {
            tmUpdate.Enabled = this.Visible;
        }
    
        private void Trace(string sText, string _s1, string _s2)
        {
            Log.Trace(sText.Trim() + " : " + _s1 + " -> " + _s2,ForContext.Dev);
        }
        private void Trace(string sText, int _s1, int _s2)
        {
            Log.Trace(sText.Trim() + " : " + _s1.ToString() + " -> " + _s2.ToString(),ForContext.Dev);
        }
        private void Trace(string sText, double _s1, double _s2)
        {
            Log.Trace(sText.Trim() + " : " + _s1.ToString() + " -> " + _s2.ToString(),ForContext.Dev);
        }
        private void Trace(string sText, bool _s1, bool _s2)
        {
            Log.Trace(sText.Trim() + " : " + _s1.ToString() + " -> " + _s2.ToString(),ForContext.Dev);
        }
        
        private void btSaveDevice_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ForContext.Frm);

            if (Log.ShowMessageModal("Confirm", "Are you Sure?") != DialogResult.Yes) return;


            UpdateDevInfo(false);

            OM.SaveJobFile(OM.GetCrntDev());

            //Loader.
            DM.ARAY[ri.LODR].SetMaxColRow(1 , OM.DevInfo.iMgzSlotCnt);

            //Prebuffer
            DM.ARAY[ri.PREB].SetMaxColRow(OM.DevInfo.iColCnt, OM.DevInfo.iRowCnt );

            //Vision1
            DM.ARAY[ri.VSN1].SetMaxColRow(OM.DevInfo.iColCnt, OM.DevInfo.iRowCnt );
            DM.ARAY[ri.RLT1].SetMaxColRow(OM.DevInfo.iColCnt, OM.DevInfo.iRowCnt );
            DM.ARAY[ri.WRK1].SetMaxColRow(OM.DevInfo.iColCnt, OM.DevInfo.iRowCnt );

            //Vision2
            DM.ARAY[ri.VSN2].SetMaxColRow(OM.DevInfo.iColCnt, OM.DevInfo.iRowCnt );
            DM.ARAY[ri.RLT2].SetMaxColRow(OM.DevInfo.iColCnt, OM.DevInfo.iRowCnt );
            DM.ARAY[ri.WRK2].SetMaxColRow(OM.DevInfo.iColCnt, OM.DevInfo.iRowCnt );

            //Vision3
            DM.ARAY[ri.VSN3].SetMaxColRow(OM.DevInfo.iColCnt, OM.DevInfo.iRowCnt );
            DM.ARAY[ri.RLT3].SetMaxColRow(OM.DevInfo.iColCnt, OM.DevInfo.iRowCnt );
            DM.ARAY[ri.WRK3].SetMaxColRow(OM.DevInfo.iColCnt, OM.DevInfo.iRowCnt );

            //PostBuffer
            DM.ARAY[ri.PSTB].SetMaxColRow(OM.DevInfo.iColCnt, OM.DevInfo.iRowCnt );

            //Unloader
            DM.ARAY[ri.ULDR].SetMaxColRow(1 , OM.DevInfo.iMgzSlotCnt);



            ArrayPos.TPara PosPara ;//= new ArrayPos.TPara();
            PosPara.dColGrGap  = OM.DevInfo.dColGrGap  ;
            PosPara.iColCnt    = OM.DevInfo.iColCnt    ;
            PosPara.iRowCnt    = OM.DevInfo.iRowCnt    ;
            PosPara.dColPitch  = OM.DevInfo.dColPitch  ;
            PosPara.dRowPitch  = OM.DevInfo.dRowPitch  ;
            PosPara.iColGrCnt  = OM.DevInfo.iColGrCnt  ;
            PosPara.iRowGrCnt  = OM.DevInfo.iRowGrCnt  ;
            PosPara.dColGrGap  = OM.DevInfo.dColGrGap  ;
            PosPara.dRowGrGap  = OM.DevInfo.dRowGrGap  ;
            PosPara.iColSbGrCnt= OM.DevInfo.iColSbGrCnt;
            PosPara.iRowSbGrCnt= OM.DevInfo.iRowSbGrCnt;            
            PosPara.dRowSbGrGap= OM.DevInfo.dRowSbGrGap;
            PosPara.dColSbGrGap= OM.DevInfo.dColSbGrGap;
            if(!OM .StripPos.SetPara(PosPara))
            {
                Log.ShowMessage("Strip Position Err" , OM .StripPos.Error);
            }

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
            bool bBreakStat = ML.  MT_GetY(mi.LODR_ZClmp , iBreakAdd) ;
            ML.MT_SetY(mi.LODR_ZClmp , iBreakAdd , !bBreakStat);
        }

        private void btULDBreakOnOff_Click(object sender, EventArgs e)
        {
            const int iBreakAdd = 3 ;
            bool bBreakStat = ML.  MT_GetY(mi.ULDR_ZClmp , iBreakAdd) ;
            ML.MT_SetY(mi.ULDR_ZClmp , iBreakAdd , !bBreakStat);
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
