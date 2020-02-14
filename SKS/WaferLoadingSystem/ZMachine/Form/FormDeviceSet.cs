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
        //public        FrameCylinderAPT []  FraCylAPT    ;
        //public        FrameInputAPT    []  FraInputAPT  ;
        public        FrameOutputAPT   []  FraOutputAPT ;
        public        FrameMotrPosAPT  []  FraMotrPosAPT;

        private const string sFormText = "Form DeviceSet ";

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

        public FormDeviceSet(Panel _pnBase)
        {
            InitializeComponent();            
            //this.Width = 1272;
            //this.Height = 866;

            this.Left     = 0;
            this.Top      = 0;

            this.TopLevel = false  ;
            this.Parent   = _pnBase;
                      

            PstnDisp();
            PM.Load(OM.GetCrntDev());

        }

        private void FormDeviceSet_Load(object sender, EventArgs e)
        {
            tbUserUnit.Text = 0.01.ToString();

            Log.Trace(sFormText + " FormDeviceSet_Load", ForContext.Frm);

            PM.UpdatePstn(true);
            UpdateDevInfo(true);
            UpdateEqpOptn(true);

            //모터 축 수에 맞춰 FrameMotr 생성
            FraMotr = new FraMotr[(int)mi.MAX_MOTR];
            for (int m = 0; m < (int)mi.MAX_MOTR; m++)
            {
                Control[] CtrlPanel    = tcPosition.Controls.Find("pnMotrJog" +  m.ToString(), true);
                //Control[] CtrlGroupBox = tcDeviceSet.Controls.Find("gbMotrJog" +  m.ToString(), true);

                MOTION_DIR eDir = ML.MT_GetDirType((mi)m);
                FraMotr[m] = new FraMotr();
                FraMotr[m].SetIdType((mi)m, eDir);
                FraMotr[m].TopLevel = false;
                FraMotr[m].Parent = CtrlPanel[0];

                FraMotr[m].Show();
                FraMotr[m].SetUnit(EN_UNIT_TYPE.utJog, 0);

                FraMotr[m].Dock = DockStyle.Fill ;
            }

            //Z축은 조그 막음
            FraMotr[(int)mi.LDR_ZPck].JogDisable();
            FraMotr[(int)mi.LDR_ZStg].JogDisable();

            //FraMotr[0].SetJogCheckSafe(mi.LDR_XPck,SEQ.LDR.JogCheckSafe);
            //FraCylAPT = new FrameCylinderAPT[(int)ci.MAX_ACTR];
            //for (int i = 0; i < (int)ci.MAX_ACTR; i++)
            //{
            //    Control[] CtrlAP = tcPosition.Controls.Find("C" + i.ToString(), true);
            //
            //    //int iCylCtrl = Convert.ToInt32(CtrlAP[0].Tag);
            //    int iCylCtrl = Convert.ToInt32(i);
            //    FraCylAPT[i] = new FrameCylinderAPT();
            //    FraCylAPT[i].TopLevel = false;
            //    FraCylAPT[i].SetConfig((ci)iCylCtrl, ML.CL_GetName(iCylCtrl).ToString(), ML.CL_GetDirType((ci)iCylCtrl), CtrlAP[0]);
            //    FraCylAPT[i].Show();
            //}

            //모터 포지션 AP텍꺼
            FraMotrPosAPT = new FrameMotrPosAPT[(int)mi.MAX_MOTR];
            for (int i = 0; i < (int)mi.MAX_MOTR; i++)
            {
                Control[] Ctrl = tcPosition.Controls.Find("pnMotrPos" + i.ToString(), true);
            
                FraMotrPosAPT[i] = new FrameMotrPosAPT();
                FraMotrPosAPT[i].TopLevel = false;
                FraMotrPosAPT[i].SetWindow(i, Ctrl[0]);
                FraMotrPosAPT[i].Show();
                //FraMotrPosAPT[i].SetWindow(i, Ctrl[0]);
            }
            
            //Input Status 생성 AP텍꺼
            //const int iInputBtnCnt  = 17;
            //FraInputAPT = new FrameInputAPT[iInputBtnCnt];
            //for (int i = 0; i < iInputBtnCnt; i++)
            //{
            //    Control[] Ctrl = tcDeviceSet.Controls.Find("X" + i.ToString(), true);
            //    
            //    int iIOCtrl = Convert.ToInt32(Ctrl[0].Tag);
            //
            //    FraInputAPT[i] = new FrameInputAPT();
            //    FraInputAPT[i].TopLevel = false;
            //    FraInputAPT[i].SetConfig((xi)iIOCtrl, ML.IO_GetXName((xi)iIOCtrl), Ctrl[0]);
            //    FraInputAPT[i].Show();
            //}

            //Output Status 생성 AP텍꺼
            //const int iOutputBtnCnt = 2;
            //FraOutputAPT = new FrameOutputAPT[iOutputBtnCnt];
            //for (int i = 0; i < iOutputBtnCnt; i++)
            //{
            //    Control[] Ctrl = tcPosition.Controls.Find("Y" + i.ToString(), true);
            //
            //    int iIOCtrl = Convert.ToInt32(Ctrl[0].Tag);
            //
            //    FraOutputAPT[i] = new FrameOutputAPT();
            //    FraOutputAPT[i].TopLevel = false;
            //    FraOutputAPT[i].SetConfig((yi)iIOCtrl, ML.IO_GetYName((yi)iIOCtrl), Ctrl[0]);
            //    FraOutputAPT[i].Show();
            //
            //   // FraOutputAPT[i].Show();
            //}

            tcPosition.SelectedIndex = 0 ;
            for (int i = 0; i < tcPosition.TabPages.Count ; i++)
            {
                Control[] Ctrl = tcPosition.Controls.Find("tcControl" + i.ToString(), true);
                TabControl TabCon = Ctrl[0] as TabControl ;
                if(TabCon == null)continue ;
                TabCon.SelectedIndex = 0 ;
            }

            //Scable Setting
           // this.Dock = DockStyle.Fill;

            tcMain.SelectedIndex = 0;
            //tcMain.TabPages.Remove(tpDeviceInfo);
            tcControl0.TabPages.Remove(metroTabPage9 ); //CYL
            tcControl0.TabPages.Remove(metroTabPage10); //IO
        }

        public void PstnDisp()
        {
            //PM.SetProp((uint)mi.LDR_ZStg    , (uint)pv.LDR_ZStgWait        , "Wait                ", false , false, true  );
            ////PM.SetProp((uint)mi.LDR_ZStg    , (uint)pv.LDR_ZStgBeforeAlign , "Before Align(Recive)", false , false, true  );
            //PM.SetProp((uint)mi.LDR_ZStg    , (uint)pv.LDR_ZStgAlign       , "Align               ", false , false, true  );
            //PM.SetProp((uint)mi.LDR_ZStg    , (uint)pv.LDR_ZStgBeforeStage , "Before Stage        ", false , false, true  );
            //PM.SetProp((uint)mi.LDR_ZStg    , (uint)pv.LDR_ZStgStage       , "Stage               ", false , false, true  );
            //PM.SetProp((uint)mi.LDR_ZStg    , (uint)pv.LDR_ZStgStageDown   , "Stage Down(Rotable) ", false , false, true  );
            
                                                                                                  
            //PM.SetProp((uint)mi.LDR_XPck    , (uint)pv.LDR_XPckWait        , "Wait                ", false , false, true  );
            //PM.SetProp((uint)mi.LDR_XPck    , (uint)pv.LDR_XPckCst         , "Cassette            ", false , false, true  );
            //PM.SetProp((uint)mi.LDR_XPck    , (uint)pv.LDR_XPckStg         , "Stage               ", false , false, true  );
                                                                                                  
            //PM.SetProp((uint)mi.LDR_YPck    , (uint)pv.LDR_YPckWait        , "Wait                ", false , false, true  );
            //PM.SetProp((uint)mi.LDR_YPck    , (uint)pv.LDR_YPckBwd         , "Backward            ", false , false, true  );
            //PM.SetProp((uint)mi.LDR_YPck    , (uint)pv.LDR_YPckFwdCst      , "Forward Cassette    ", false , false, true  );
            //PM.SetProp((uint)mi.LDR_YPck    , (uint)pv.LDR_YPckFwdStg      , "Forward Stage       ", false , false, true  );
            //PM.SetProp((uint)mi.LDR_YPck    , (uint)pv.LDR_YPckStgMoveAble , "Stage & Wafer Moveable down", false , false, true  );
            

            //PM.SetProp((uint)mi.LDR_ZPck    , (uint)pv.LDR_ZPckWait            , "Wait            ", false , false, true  );
            //PM.SetProp((uint)mi.LDR_ZPck    , (uint)pv.LDR_ZPck1stWaferBtm     , "1stWaferBottom  ", false , false, true  );
            //PM.SetProp((uint)mi.LDR_ZPck    , (uint)pv.LDR_ZPck1stWafer        , "1stWafer        ", false , false, true  );
            //PM.SetProp((uint)mi.LDR_ZPck    , (uint)pv.LDR_ZPckStgAlignUp  , "Align Up        ", false , false, true  );
            //PM.SetProp((uint)mi.LDR_ZPck    , (uint)pv.LDR_ZPckStgAlign        , "Align           ", false , false, true  );
            //PM.SetProp((uint)mi.LDR_ZPck    , (uint)pv.LDR_ZPckStgAlignDown    , "Align Down      ", false , false, true  );            
            
            PM.SetProp((uint)mi.LDR_ZStg    , (uint)pv.LDR_ZStgWait        , "대기위치(Wait)                        ", false , false, true  );
            //PM.SetProp((uint)mi.LDR_ZStg    , (uint)pv.LDR_ZStgBeforeAlign , "Before Align(Recive)                ", false , false, true  );
            PM.SetProp((uint)mi.LDR_ZStg    , (uint)pv.LDR_ZStgAlign       , "자재 받는 위치(Align)                 ", false , false, true  );
            PM.SetProp((uint)mi.LDR_ZStg    , (uint)pv.LDR_ZStgBeforeStage , "스테이지 전 위치(Before Stage)        ", false , false, true  );
            PM.SetProp((uint)mi.LDR_ZStg    , (uint)pv.LDR_ZStgStage       , "스테이지 위치(Stage)                  ", false , false, true  );
            PM.SetProp((uint)mi.LDR_ZStg    , (uint)pv.LDR_ZStgStageDown   , "스테이지 밑 위치(Stage Down(Rotable)) ", false , false, true  );
            
                                                                                                  
            PM.SetProp((uint)mi.LDR_XPck    , (uint)pv.LDR_XPckWait        , "대기위치(Wait)          ", false , false, true  );
            PM.SetProp((uint)mi.LDR_XPck    , (uint)pv.LDR_XPckCst         , "카세트쪽 위치(Cassette) ", false , false, true  );
            PM.SetProp((uint)mi.LDR_XPck    , (uint)pv.LDR_XPckStg         , "스테이지쪽 위치(Stage)  ", false , false, true  );
                                                                                                  
            PM.SetProp((uint)mi.LDR_YPck    , (uint)pv.LDR_YPckWait        , "대기위치(Wait)                                    ", false , false, true  );
            PM.SetProp((uint)mi.LDR_YPck    , (uint)pv.LDR_YPckBwd         , "후진위치(Backward)                                ", false , false, true  );
            PM.SetProp((uint)mi.LDR_YPck    , (uint)pv.LDR_YPckFwdCst      , "카세트쪽 전진위치(Forward Cassette)               ", false , false, true  );
            PM.SetProp((uint)mi.LDR_YPck    , (uint)pv.LDR_YPckFwdStg      , "스테이지쪽 전진위치(Forward Stage)                ", false , false, true  );
            PM.SetProp((uint)mi.LDR_YPck    , (uint)pv.LDR_YPckStgMoveAble , "스테이지간섭없는 위치(Stage & Wafer Moveable down)", false , false, true  );
            

            PM.SetProp((uint)mi.LDR_ZPck    , (uint)pv.LDR_ZPckWait            , "대기위치(Wait)                              ", false , false, true  );
            PM.SetProp((uint)mi.LDR_ZPck    , (uint)pv.LDR_ZPck1stWaferBtm     , "카세트 첫번째웨이퍼 하단위치(1stWaferBottom)", false , false, true  );
            PM.SetProp((uint)mi.LDR_ZPck    , (uint)pv.LDR_ZPck1stWafer        , "카세트 첫번째웨이퍼 위치(1stWafer)          ", false , false, true  );
            PM.SetProp((uint)mi.LDR_ZPck    , (uint)pv.LDR_ZPckStgAlignUp      , "스테이지 받는위치 전 위치(Align Up)         ", false , false, true  );
            PM.SetProp((uint)mi.LDR_ZPck    , (uint)pv.LDR_ZPckStgAlign        , "스테이지 받는위치(Align)                    ", false , false, true  );
            PM.SetProp((uint)mi.LDR_ZPck    , (uint)pv.LDR_ZPckStgAlignDown    , "스테이지 받는위치 밑 위치(Align Down)       ", false , false, true  );            

            PM.SetCheckSafe((uint)mi.LDR_XPck,SEQ.LDR.CheckSafe); 
            PM.SetCheckSafe((uint)mi.LDR_YPck,SEQ.LDR.CheckSafe); 
            PM.SetCheckSafe((uint)mi.LDR_ZPck,SEQ.LDR.CheckSafe); 
            PM.SetCheckSafe((uint)mi.LDR_ZStg,SEQ.LDR.CheckSafe); 


        }
             
        private void tcDeviceSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            int iSeletedIndex;
            iSeletedIndex = tcPosition.SelectedIndex;
            Control[] JogPanel    = tcPosition.Controls.Find("pnJog" +  iSeletedIndex.ToString() , true);
            if(JogPanel.Length <= 0) return ; 
            
            pnJog.Parent = JogPanel[0] ;
            
            PM.UpdatePstn(true);
            PM.Load(OM.GetCrntDev());
        }

        public void UpdateDevInfo(bool bToTable)
        {
            if (bToTable)
            {
                CConfig.ValToCon(ed1             , ref OM.DevInfo.dRowPitch );
                CConfig.ValToCon(ed2             , ref OM.DevInfo.iRowCount );
                CConfig.ValToCon(ed3             , ref OM.DevInfo.iVacuumOn );
                CConfig.ValToCon(ed4             , ref OM.DevInfo.iVacuumOff);
            }
            else 
            {
                OM.CDevInfo DevInfo = OM.DevInfo;

                CConfig.ConToVal(ed1             , ref OM.DevInfo.dRowPitch );
                CConfig.ConToVal(ed2             , ref OM.DevInfo.iRowCount );
                CConfig.ConToVal(ed3             , ref OM.DevInfo.iVacuumOn );
                CConfig.ConToVal(ed4             , ref OM.DevInfo.iVacuumOff);

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


        public void UpdateEqpOptn(bool _bToTable)
        {                                                                                   
            if (_bToTable == true) 
            {
                
                CConfig.ValToCon(edetc2  , ref OM.EqpOptn.sEquipName     );
                CConfig.ValToCon(edetc1  , ref OM.EqpOptn.sEquipSerial   );
            }
            else 
            {
                OM.CEqpOptn EqpOptn = OM.EqpOptn;

                CConfig.ConToVal(edetc2  , ref OM.EqpOptn.sEquipName     );
                CConfig.ConToVal(edetc1  , ref OM.EqpOptn.sEquipSerial   );

                //Auto Log
                Type type = EqpOptn.GetType();
                FieldInfo[] f = type.GetFields(BindingFlags.Public|BindingFlags.NonPublic | BindingFlags.Instance);
                for(int i = 0 ; i < f.Length ; i++){
                    Trace(f[i].Name, f[i].GetValue(EqpOptn).ToString(), f[i].GetValue(OM.EqpOptn).ToString());
                }

                UpdateEqpOptn(true);
            }
        }

        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            tmUpdate.Enabled = false;
            //센서 보여주기
            bool bSsr = false;
            bSsr = ML.IO_GetX(xi.CassetteLeft    ); l1.BackColor = bSsr ? Color.Lime : Color.Gray ; l1.Text = bSsr ? "ON" : "OFF" ;
            bSsr = ML.IO_GetX(xi.CassetteRight   ); l2.BackColor = bSsr ? Color.Lime : Color.Gray ; l2.Text = bSsr ? "ON" : "OFF" ;
            bSsr = ML.IO_GetX(xi.PickerVacuum    ); l3.BackColor = bSsr ? Color.Lime : Color.Gray ; l3.Text = bSsr ? "ON" : "OFF" ;
            bSsr = ML.IO_GetX(xi.WaferOverload   ); l4.BackColor = bSsr ? Color.Lime : Color.Gray ; l4.Text = bSsr ? "ON" : "OFF" ;
            bSsr = ML.IO_GetX(xi.StageVacuum     ); l5.BackColor = bSsr ? Color.Lime : Color.Gray ; l5.Text = bSsr ? "ON" : "OFF" ;
            bSsr = ML.IO_GetX(xi.WaferDtSsr      ); l6.BackColor = bSsr ? Color.Lime : Color.Gray ; l6.Text = bSsr ? "ON" : "OFF" ;
            bSsr = ML.IO_GetX(xi.ManualInspLimit ); l7.BackColor = bSsr ? Color.Lime : Color.Gray ; l7.Text = bSsr ? "ON" : "OFF" ;

            bSsr = ML.IO_GetY(yi.PickerVacuum);    metroTile9 .BackColor = bSsr ? Color.Lime : Color.Gray ; metroTile9 .Text = bSsr ? "PICKER VACUUM ON"   : "PICKER VACUUM OFF" ;
            bSsr = ML.IO_GetY(yi.StageVacuum);    metroTile10.BackColor = bSsr ? Color.Lime : Color.Gray ; metroTile10.Text = bSsr ? "STAGE VACUUM ON"    : "STAGE VACUUM OFF"  ;
            bSsr = ML.IO_GetY(yi.StageZBreakOff); metroTile11.BackColor = bSsr ? Color.Lime : Color.Gray ; metroTile11.Text = bSsr ? "PICKER_Z BREAK OFF" : "PICKER_Z BREAK ON" ;
            //btSave.Text = btSave.Left.ToString() ;
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
        
        private void btManual_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.TraceListView(sFormText + sText + " Button Clicked", ForContext.Frm);

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
            Log.TraceListView(sFormText + sText + " Button Clicked", ForContext.Frm);

            if (Log.ShowMessageModal("Confirm", "Are you Sure?") != DialogResult.Yes) return;


            UpdateDevInfo(false);
            UpdateEqpOptn(false);

            OM.SaveJobFile(OM.GetCrntDev());
            OM.SaveEqpOptn();

            PM.UpdatePstn(false);
            PM.Save(OM.GetCrntDev());
            PM.UpdatePstn(true);

            ////Loader.
            DM.ARAY[ri.CST].SetMaxColRow(1 , OM.DevInfo.iRowCount);
            //


            //ArrayPos.TPara PosPara ;//= new ArrayPos.TPara();
            //PosPara.dColGrGap  = OM.DevInfo.dColGrGap  ;            
            //if(!OM .StripPos.SetPara(PosPara))
            //{
            //    Log.ShowMessage("Strip Position Err" , OM .StripPos.Error);
            //}

            Refresh();
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }

        private void metroTile9_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.TraceListView(sFormText + sText + " Button Clicked", ForContext.Frm);

            if (Log.ShowMessageModal("Confirm", "Are you Sure?") != DialogResult.Yes) return;

            sText = ((Button)sender).Tag.ToString();
            if(sText == "A") ML.IO_SetY(yi.PickerVacuum   ,!ML.IO_GetY(yi.PickerVacuum   ));
            if(sText == "B") ML.IO_SetY(yi.StageVacuum   ,!ML.IO_GetY(yi.StageVacuum   ));
            if(sText == "C") ML.IO_SetY(yi.StageZBreakOff,!ML.IO_GetY(yi.StageZBreakOff));

        }
    }
}
