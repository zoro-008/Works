using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using COMMON;
using System.IO;
using System.Reflection;
using SML2;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Machine
{
    public partial class FormDeviceSet : Form
    {
        public static FormDeviceSet FrmDeviceSet ;
        public        FraMotr    [] FraMotr      ;
        public        FraCylOneBt[] FraCylinder  ;
        public        FraOutput  [] FraOutput    ;
        public        FormMain      FrmMain      ;
        public        ListViewEx    lvNodePos    ;

        private       ComboBox      cbDirctn     ; //Node ComboBox
        private       TextBox       tbText       ; //Node ComboBox

        private       int           ilvNodeCnt = OM.MAX_NODE_POS;

        //CPstnMan PstnCnt;

        public  FormDeviceSet(Panel _pnBase)
        {
            InitializeComponent();

            InitNodePosView(pnLvBase);

            this.Width = 1272;
            this.Height = 866;

            this.TopLevel = false;
            this.Parent = _pnBase;

            tbUserUnit.Text = 0.01.ToString();
            PstnDisp();

            

            //모터 축에 대한 포지션 디스플레이
            PM.SetWindow(pnMotrPos0, (int)mi.IDX_XCUT);
            PM.SetWindow(pnMotrPos1, (int)mi.IDX_XOUT);
            PM.SetWindow(pnMotrPos2, (int)mi.IDX_TTRN);


            PM.SetGetCmdPos((int)mi.IDX_XCUT, SML.MT.GetCmdPos);
            PM.SetGetCmdPos((int)mi.IDX_XOUT, SML.MT.GetCmdPos);
            PM.SetGetCmdPos((int)mi.IDX_TTRN, SML.MT.GetCmdPos);
            

            OM.LoadLastInfo();
            PM.Load(OM.GetCrntDev().ToString());

            PM.UpdatePstn(true);
            UpdateDevInfo(true);
            UpdateDevOptn(true);

            FraMotr     = new FraMotr    [(int)mi.MAX_MOTR];
            FraCylinder = new FraCylOneBt[(int)ci.MAX_ACTR];
            //FraOutput   = new FraOutput  [SM.IO._iMaxOut     ];

            //모터 축 수에 맞춰 FrameMotr 생성

            for (int m = 0; m < (int)mi.MAX_MOTR ; m++)
            {
                Control[] Ctrl = tcDeviceSet.Controls.Find("pnMotrJog" +  m.ToString(), true);

                MOTION_DIR eDir = SM.MT_GetDirType((mi)m);
                FraMotr[m] = new FraMotr();
                FraMotr[m].SetIdType((mi)m, eDir);
                FraMotr[m].TopLevel = false;
                FraMotr[m].Parent   = Ctrl[0];
                FraMotr[m].Show();
                FraMotr[m].SetUnit(EN_UNIT_TYPE.utJog, 0);
            }

            //실린더 수에 맞춰 FrameCylinder 생성
            for (int i = 0; i < (int)ci.MAX_ACTR; i++)
            {
                Control[] Ctrl          = tcDeviceSet.Controls.Find("pnAtcr" + i.ToString(), true);

                FraCylinder[i]          = new FraCylOneBt();
                FraCylinder[i].TopLevel = false;

                switch (i)
                {
                    default                     :                                                                                               break;
                    case (int)ci.IDX_Hold1UpDn  : FraCylinder[i].SetConfig((ci)i, SML.CL.GetName(i).ToString(), SML.CL.GetDirType(i), Ctrl[0]); break;
                    case (int)ci.IDX_CutLtFwBw  : FraCylinder[i].SetConfig((ci)i, SML.CL.GetName(i).ToString(), SML.CL.GetDirType(i), Ctrl[0]); break;
                    case (int)ci.IDX_CutRtFwBw  : FraCylinder[i].SetConfig((ci)i, SML.CL.GetName(i).ToString(), SML.CL.GetDirType(i), Ctrl[0]); break;
                    case (int)ci.IDX_TwstLtDnUp : FraCylinder[i].SetConfig((ci)i, SML.CL.GetName(i).ToString(), SML.CL.GetDirType(i), Ctrl[0]); break;
                    case (int)ci.IDX_TwstRtDnUp : FraCylinder[i].SetConfig((ci)i, SML.CL.GetName(i).ToString(), SML.CL.GetDirType(i), Ctrl[0]); break;
                    case (int)ci.IDX_Hold2UpDn  : FraCylinder[i].SetConfig((ci)i, SML.CL.GetName(i).ToString(), SML.CL.GetDirType(i), Ctrl[0]); break;
                    case (int)ci.IDX_CutBaseUpDn: FraCylinder[i].SetConfig((ci)i, SML.CL.GetName(i).ToString(), SML.CL.GetDirType(i), Ctrl[0]); break;
                    case (int)ci.IDX_OutDnUp    : FraCylinder[i].SetConfig((ci)i, SML.CL.GetName(i).ToString(), SML.CL.GetDirType(i), Ctrl[0]); break;
                    case (int)ci.IDX_CutterDnUp : FraCylinder[i].SetConfig((ci)i, SML.CL.GetName(i).ToString(), SML.CL.GetDirType(i), Ctrl[0]); break;
                    
                }
                FraCylinder[i].Show();
            }

            //Output 버튼 생성
            const int iOutputBtnCnt = 3;
            FraOutput = new FraOutput[iOutputBtnCnt];
            for (int i = 0; i < iOutputBtnCnt; i++)
            {
                FraOutput[i] = new FraOutput();
                FraOutput[i].TopLevel = false;
            
                switch (i)
                {
                    default                                : break;
//                    case (int)yi.ySLD_Soldering    : FraOutput[i].SetConfig(yi.ySLD_Soldering    , SM.IO.GetYName((int)yi.ySLD_Soldering    )  , pnIO0); break;
//                    case (int)yi.ySLD_AirCleanOnOff: FraOutput[i].SetConfig(yi.ySLD_AirCleanOnOff, SM.IO.GetYName((int)yi.ySLD_AirCleanOnOff), pnIO1); break;
                }
                
                FraOutput[i].Show();
            }
            UpdateNodePos(true);

            pbLine.Refresh();
        }

        private void InitNodePosView(Panel _pnBase)
        {
            lvNodePos = new ListViewEx();

            

            lvNodePos.Parent = _pnBase;
            lvNodePos.Dock   = DockStyle.Fill;

            //NodePositions ListView==========================
            const int iCellWidth    = 103;
            const int icbWidth      = 103;
            string [] sCBData = {"Left", "Right"};

            cbDirctn = new ComboBox();//[(int)OM.MAX_NODE_POS];
            cbDirctn.Items.AddRange(sCBData);
            cbDirctn.Visible = false;

            tbText = new TextBox();

            tbText.Visible = false;
            tbText.BorderStyle = BorderStyle.FixedSingle;
            tbText.Tag = 0;
            tbText.Leave += tbText_Leave;
            tbText.KeyDown += tbText_KeyDown;

            lvNodePos.Clear();
            lvNodePos.View = View.Details;
            lvNodePos.FullRowSelect = true;
            lvNodePos.MultiSelect   = true;
            lvNodePos.GridLines     = true;

            //셀높이 조절하기 편법.
            ImageList dummyImage = new ImageList();
            dummyImage.ImageSize = new System.Drawing.Size(1, 23);
            lvNodePos.SmallImageList = dummyImage;


            lvNodePos.Columns.Add("No"        , 40 , HorizontalAlignment.Left);
            lvNodePos.Columns.Add("Start Pos" , iCellWidth, HorizontalAlignment.Left);
            lvNodePos.Columns.Add("Direction" , icbWidth  , HorizontalAlignment.Left);
            lvNodePos.Columns.Add("Degree"    , iCellWidth, HorizontalAlignment.Left);
            lvNodePos.Columns.Add("WorkCount" , iCellWidth, HorizontalAlignment.Left);
            lvNodePos.Columns.Add("WorkPitch" , iCellWidth, HorizontalAlignment.Left);

            ListViewItem[] liInput = new ListViewItem[ilvNodeCnt];
            for (int i = 0; i < ilvNodeCnt; i++)
            {
                liInput[i] = new ListViewItem(i.ToString());
                for (int j = 1; j < lvNodePos.Columns.Count; j++)
                {
                    liInput[i].SubItems.Add("0");
                }

                liInput[i].UseItemStyleForSubItems = false;
                lvNodePos.Items.Add(liInput[i]);

                //cbDirctn[i].Text = "Input";
                lvNodePos.AddEmbeddedControl(cbDirctn, 2, i);
                
                
            }
            for (int i = 0; i < ilvNodeCnt; i++)
            { 
                for (int j = 1; j < lvNodePos.Columns.Count; j++)
                {
                    lvNodePos.AddEmbeddedControl(tbText, j, i);
                }
            }

            lvNodePos.Click += new EventHandler(lvNodePos_Click);
            tbText.KeyDown += new KeyEventHandler(tbText_KeyDown);
            cbDirctn.SelectedIndexChanged += new EventHandler(cbDirctn_SelectedIndexChanged);
            //lvNodePos.SelectedIndexChanged += new EventHandler(lvNodePos_SelectedIndexChanged);

            var PropInput = lvNodePos.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropInput.SetValue(lvNodePos, true, null);
            PropInput = cbDirctn.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropInput.SetValue(cbDirctn, true, null);
            PropInput = tbText.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropInput.SetValue(tbText, true, null);
            
        }

        int iLvrow = 0;
        int iLvcol = 0;
        
        private void lvNodePos_Click(object sender, EventArgs e)
        {
            const int iHeadHeight = 23;
            const int iHeadWidth  = 35;
            const int iItemHeight = 23;
            const int iItemWidth  = 103;

            if (tbText.Visible && lvNodePos.Items[iLvrow].SubItems[iLvcol].Text != tbText.Text && iLvcol != 0) lvNodePos.Items[iLvrow].SubItems[iLvcol].Text = tbText.Text;

            Point mousePos = lvNodePos.PointToClient(Control.MousePosition);
            ListViewHitTestInfo hitTest = lvNodePos.HitTest(mousePos);
            if (hitTest.Item == null) return;
            iLvrow = hitTest.Item.Index;
            iLvcol = hitTest.Item.SubItems.IndexOf(hitTest.SubItem);
            mousePos.X = iHeadWidth + ((iLvcol - 1) * iItemWidth);
            mousePos.Y = iHeadHeight + (iLvrow * iItemHeight);
            
            if (iLvcol != 2 && iLvcol != 0)
            {
                Font sFont = new System.Drawing.Font("굴림", 11);

                lvNodePos.MoveEmbeddedControl(tbText, iLvcol, iLvrow);
                tbText.Font = sFont;

                tbText.Text = lvNodePos.Items[iLvrow].SubItems[iLvcol].Text;
                tbText.Visible = true;
                cbDirctn.Visible = false;
                tbText.Select(); ;//Focus();
                //tbText.Tag = row;
            }
            if (iLvcol == 2)
            {
                lvNodePos.MoveEmbeddedControl(cbDirctn, iLvcol, iLvrow);
                                
                cbDirctn.Visible = true;
                tbText.Visible = false;
                //tbText.Tag = row;
            }
        }

        private void tbText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                lvNodePos.Items[iLvrow].SubItems[iLvcol].Text = tbText.Text;

                tbText.Visible = false;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                tbText.Text = "";
                tbText.Visible = false;
            }
        }
        private void tbText_Leave(object sender, EventArgs e)
        {

        }
        private void cbDirctn_SelectedIndexChanged(object sender, EventArgs e)
        {
            const int iHeadHeight = 23;
            const int iHeadWidth  = 35;
            const int iItemHeight = 23;
            const int iItemWidth  = 103;

            //Point mousePos = lvNodePos.PointToClient(Control.MousePosition);
            //ListViewHitTestInfo hitTest = lvNodePos.HitTest(mousePos);
            //if (hitTest.Item == null) return;
            //iLvrow = hitTest.Item.Index;
            //iLvcol = hitTest.Item.SubItems.IndexOf(hitTest.SubItem);
            //mousePos.X = iHeadWidth + ((iLvcol - 1) * iItemWidth);
            //mousePos.Y = iHeadHeight + (iLvrow * iItemHeight);

            int iDirctn = cbDirctn.SelectedIndex;

            if(iDirctn == 0) lvNodePos.Items[iLvrow].SubItems[iLvcol].Text = "Left";
            else             lvNodePos.Items[iLvrow].SubItems[iLvcol].Text = "Right";

            cbDirctn.Visible = false;

        }
        
        public void PstnDisp()
        {
            //IDX_XCUT Property
            PM.SetProp((uint)mi.IDX_XCUT, (uint)pv.IDX_XCUTWait     , "Wait"       , false, false, false);
            PM.SetProp((uint)mi.IDX_XCUT, (uint)pv.IDX_XCUTWorkStt  , "Work Stt"   , false, false, false);

            //IDX_TTRN Property        
            PM.SetProp((uint)mi.IDX_TTRN, (uint)pv.IDX_TTRNWait     , "Wait     "  , false, false, false);

            //IDX_XOUT Left Property                                                                                  
            PM.SetProp((uint)mi.IDX_XOUT, (uint)pv.IDX_XOUTWait     , "Wait"                  , false, false, false);
            PM.SetProp((uint)mi.IDX_XOUT, (uint)pv.IDX_XOUTStopWait , "StopWait"              , false, false, false);
            PM.SetProp((uint)mi.IDX_XOUT, (uint)pv.IDX_XOUTClamp    , "Clamp"                 , false, false, false);
            PM.SetProp((uint)mi.IDX_XOUT, (uint)pv.IDX_XOUTBin      , "IDX_XOUTBin"           , false, false, false);
            PM.SetProp((uint)mi.IDX_XOUT, (uint)pv.IDX_XOUTTensnOfs , "Tension Offset"        , true , false, false);
            PM.SetProp((uint)mi.IDX_XOUT, (uint)pv.IDX_XOUTRvrsOfs  , "Tension Reverse Offset", true , false, false);
            //PM.SetProp((uint)mi.IDX_XOUT, (uint)pv.IDX_XOUTOut      , "Out"      , false, false, false); //느낌이 불길하다.. Pull포지션 만들어야 할지도...
        }                                                                                                            
                                                                                                                                      
        private void tcDeviceSet_SelectedIndexChanged(object sender, EventArgs e)                                      
        {                                                                                                               
            int iSeletedIndex;                                                                                          
            iSeletedIndex = tcDeviceSet.SelectedIndex;                                                                  
            
            switch (iSeletedIndex)
            {
                default : break;
                case 0  : gbJogUnit.Parent = pnJog1; 
                          tabControl1.Parent = panel1; break;
                case 1  : gbJogUnit.Parent = pnJog2; 
                          tabControl1.Parent = panel2;break;
                //case 2  : gbJogUnit.Parent = pnJog2; 
                //    pnMotrJog0.Parent = pnMotrJog3;break;
            }

            UpdateDevInfo(true);
            UpdateDevOptn(true);
            UpdateNodePos(true);
            PM.UpdatePstn(true);
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            Log.Trace("SAVE", "Clicked");

            if (Log.ShowMessageModal("Confirm", "Are you Sure?") != DialogResult.Yes) return;

            UpdateDevInfo(false);
            UpdateNodePos(false);
            UpdateDevOptn(false);
            if (WorkDistanceCal() > 200)
            {
                Log.ShowMessage("Warring", "작업길이 최대치를 넘어갔습니다."); 
                return ;
            }
            if (WorkDistanceCal() > OM.MstOptn.dMAXWorkDist && !OM.DevOptn.bShiftWork) {
                if (Log.ShowMessageModal("Warring", "Used ShiftWork ?") != DialogResult.Yes)
                {
                    return;
                }
                cbshiftWork.Checked = true;
                Refresh();
            }
            

            
            OM.SaveDevInfo(OM.GetCrntDev().ToString());

            
            OM.SaveNodePos(OM.GetCrntDev().ToString());

            
            OM.SaveDevOptn(OM.GetCrntDev().ToString());
            
            pbLine.Refresh();

            PM.UpdatePstn(false);
            PM.Save(OM.GetCrntDev());

            tbWorkDist.Text = OM.DevOptn.dWorkDist.ToString();
            tbWorkDist2.Text = OM.DevOptn.dWorkDist.ToString();

            //DM.ARAY[(int)ri.REAR ].SetMaxColRow(OM.DevInfo.iRearColCnt , OM.DevInfo.iRearRowCnt );
            //DM.ARAY[(int)ri.FRNT ].SetMaxColRow(OM.DevInfo.iFrntColCnt , OM.DevInfo.iFrntRowCnt );

            OM.SaveEqpOptn();

        }

        public void UpdateDevInfo(bool bToTable)
        {
            if (bToTable)
            {
                //Rear Housing Stage
                //tbLtWrkCnt   .Text = OM.DevInfo.iRearColCnt   .ToString();
            }
            else 
            {
                //if (CConfig.StrToIntDef(tbLtWrkCnt .Text, 1) <= 0) { tbLtWrkCnt .Text = 1.ToString(); }
                UpdateDevInfo(true);
            }
        
        }

        public void UpdateDevOptn(bool bToTable)
        {

            if (bToTable)
            {
                tbSttEmpty    .Text = OM.DevOptn.dSttEmpty    .ToString();
                tbEndEmpty    .Text = OM.DevOptn.dEndEmpty    .ToString();
                tbWorkDist    .Text = OM.DevOptn.dWorkDist    .ToString();
                tbLeftCutMovePitch .Text = OM.DevOptn.dLeftCutMovePitch .ToString();
                tbRightCutMovePitch.Text = OM.DevOptn.dRightCutMovePitch.ToString();
                //tbTensnBwd    .Text = OM.DevOptn.dTensnBwd    .ToString();
                tbCutLtRtDist .Text = OM.DevOptn.dCutLtRtDist .ToString();

                tbWorkDist2   .Text = OM.DevOptn.dWorkDist    .ToString();
                tbShiftWorkPos.Text = OM.DevOptn.dShiftWorkPos.ToString();

                tbWorkDelay   .Text = OM.DevOptn.iWorkDelay   .ToString();

                cbshiftWork   .Checked = OM.DevOptn.bShiftWork;

                tbShiftOfs    .Text    = OM.DevOptn.dShiftOfs.ToString();
                tbCutBwdOfs   .Text    = OM.DevOptn.dCutBwdOfs.ToString();

                tbTargetCnt   .Text    = OM.DevOptn.iTargetCnt.ToString();

                tbWorkDiameter.Text    = OM.DevOptn.dWorkDiameter.ToString();
                
            }
            else
            {
                OM.DevOptn.dSttEmpty     = CConfig.StrToDoubleDef(tbSttEmpty    .Text, 0);
                OM.DevOptn.dEndEmpty     = CConfig.StrToDoubleDef(tbEndEmpty    .Text, 0);
                OM.DevOptn.dWorkDist     = WorkDistanceCal();//CConfig.StrToIntDef   (tbWorkDist    .Text, 0);
                OM.DevOptn.dLeftCutMovePitch  = CConfig.StrToDoubleDef(tbLeftCutMovePitch .Text, 0);
                OM.DevOptn.dRightCutMovePitch = CConfig.StrToDoubleDef(tbRightCutMovePitch.Text, 0);
                //OM.DevOptn.dTensnBwd     = CConfig.StrToDoubleDef(tbTensnBwd    .Text, 0);
                OM.DevOptn.dCutLtRtDist  = CConfig.StrToDoubleDef(tbCutLtRtDist .Text, 0);
                
                OM.DevOptn.bShiftWork    = cbshiftWork.Checked;
                OM.DevOptn.dShiftWorkPos = CConfig.StrToDoubleDef(tbShiftWorkPos.Text, 0);

                OM.DevOptn.iWorkDelay    = CConfig.StrToIntDef   (tbWorkDelay   .Text, 0);

                OM.DevOptn.dShiftOfs     = CConfig.StrToDoubleDef   (tbShiftOfs    .Text, 0.0);
                OM.DevOptn.dCutBwdOfs    = CConfig.StrToDoubleDef   (tbCutBwdOfs   .Text, 0.0);

                OM.DevOptn.iTargetCnt    = CConfig.StrToIntDef      (tbTargetCnt   .Text, 0  );

                OM.DevOptn.dWorkDiameter = CConfig.StrToDoubleDef(tbWorkDiameter.Text, 0.0);

                UpdateDevOptn(true);
            }
        }

        public void UpdateNodePos(bool bToTable)
        {
            int iDirection = 0;
            if (bToTable)
            {
                for (int i = 0; i < OM.MAX_NODE_POS; i++)
                {
                    lvNodePos.Items[i].SubItems[0].Text = i.ToString();
                    lvNodePos.Items[i].SubItems[1].Text = OM.NodePos[i].dWrkSttPos .ToString();
                    iDirection = OM.NodePos[i].iDirection;
                    if(iDirection == 0)lvNodePos.Items[i].SubItems[2].Text = "Left";
                    else               lvNodePos.Items[i].SubItems[2].Text = "Right";
                    lvNodePos.Items[i].SubItems[3].Text = OM.NodePos[i].dDegree   .ToString();
                    lvNodePos.Items[i].SubItems[4].Text = OM.NodePos[i].iWrkCnt   .ToString();
                    lvNodePos.Items[i].SubItems[5].Text = OM.NodePos[i].dWrkPitch .ToString();
                }
            }
            else
            {
                for (int i = 0; i < OM.MAX_NODE_POS; i++)
                {
                    double.TryParse(lvNodePos.Items[i].SubItems[1].Text, out OM.NodePos[i].dWrkSttPos );
                    if (lvNodePos.Items[i].SubItems[2].Text == "Left") OM.NodePos[i].iDirection = 0;
                    else                                               OM.NodePos[i].iDirection = 1;
                    //int   .TryParse(lvNodePos.Items[i].SubItems[2].Text, out OM.NodePos[i].iDirection);
                    double.TryParse(lvNodePos.Items[i].SubItems[3].Text, out OM.NodePos[i].dDegree   );
                    int   .TryParse(lvNodePos.Items[i].SubItems[4].Text, out OM.NodePos[i].iWrkCnt   );
                    double.TryParse(lvNodePos.Items[i].SubItems[5].Text, out OM.NodePos[i].dWrkPitch );


                    //UpdateNodePos(true);
                }
            }
        }

        

        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            tmUpdate.Enabled = false;
            btSaveDevice.Enabled = !SEQ._bRun;

            pbLine.Refresh();

            //UpdateDevInfo(true);
            //UpdateDevOptn(true);
            

            //tbSTGColCnt            
            //edHghMsg2     -> Text = Rs232Keyence.GetMsg();

            //MakeMidBlkImg();
            //MakeLDRFImg();
            //MakeLDRRImg()  ;
            //MakeULDImg()   ;
            
            //pnScreen -> Visible  = FM_GetLevel() != lvMaster  ;
            //
            //if(FM_GetLevel() != lvMaster && rgJogUnit -> ItemIndex == 5)
            //{
            //    rgJogUnit -> ItemIndex = 0 ;
            //}
            
            tmUpdate.Enabled = true;
        }

        private void rbJog_Click(object sender, EventArgs e)
        {

            int iUnit = Convert.ToInt32(((RadioButton)sender).Tag) ;

            double dUserDefine = 0.0 ;
            if(!double.TryParse(tbUserUnit.Text  , out dUserDefine))dUserDefine=0.0;

            for (int m = 0; m < (int)mi.MAX_MOTR; m++)
            {
                switch(iUnit)
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
            if (!rbUserUnit.Checked)return ;

            double dUserDefine = 0.0 ;
            if(!double.TryParse(tbUserUnit.Text  , out dUserDefine))dUserDefine=0.0;
            
            for (int m = 0; m < (int)mi.MAX_MOTR; m++)
            {

                FraMotr[m].SetUnit(EN_UNIT_TYPE.utMove, dUserDefine);
                         
            }


        }

        private void FormDeviceSet_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmUpdate.Enabled = false;
        }

        private void FormDeviceSet_Shown(object sender, EventArgs e)
        {
            //EmbededVision.SetVisionParent(pnCamera.Handle);
        }

        //public IntPtr GetCamPnHandle()
        //{
        //    //return pnCamera.Handle;
        //}

        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //싸이즈가 찐따나서 한번 더 해준다.
            //EmbededVision.SetVisionParent(pnCamera.Handle);
        }

        double dNodeMaxPos = 0;
        double dNodeMinPos = 0;
        
        private void pbLine_Paint(object sender, PaintEventArgs e)
        {
            Pen Pen = new Pen(Color.Black);

            Graphics g = e.Graphics;
            WorkDistanceCal();
            double dWidthScale  = (pbLine.Size.Width - 6) / OM.DevOptn.dWorkDist;
            double dCenterPos = OM.DevOptn.dWorkDist / 2;

            int iX1 = 3;
            int iX2 = pbLine.Size.Width - 3;
            int iY = (pbLine.Size.Height) / 2;

            double dX1 = 0.0;
            double dX2 = 0.0;
            double dY1 = 0.0;
            double dY2 = 0.0;
            double dX1Ret = 0.0;
            double dX2Ret = 0.0;

            g.DrawLine(Pen, iX1, iY, iX2, iY);

            for (int n = 0; n < OM.MAX_NODE_POS; n++)
            {
                double dCenter = pbLine.Size.Width / 2;
           
                for (int c = 0; c < OM.NodePos[n].iWrkCnt; c++)
                {
                    if (OM.NodePos[n].iDirection == 0)
                    {
                        dX1 = OM.NodePos[n].dWrkSttPos + OM.DevOptn.dSttEmpty;
                        dY1 = iY;
                        dX2 = dX1;

                        dX1Ret = (dX1 + (c * OM.NodePos[n].dWrkPitch)) * dWidthScale;
                        dX2Ret = (dX2 + (c * OM.NodePos[n].dWrkPitch) - OM.DevOptn.dLeftCutMovePitch) * dWidthScale;

                        if (OM.NodePos[n].dDegree == 0)
                        {
                            dY2 = iY - 10;
                        }
                        else
                        {
                            dY2 = iY + 10;
                        }

                        //if (OM.NodePos[n].dDegree == 0)
                        //{
                        //    //dCenterPos - (OM.NodePos[n].dWrkSttPos - dNodeMinPos) + ((dNodeMaxPos - dNodeMinPos) / 2);
                        //    dX1 = OM.NodePos[n].dWrkSttPos;
                        //    dY1 = iY;
                        //    dX2 = dX1;
                        //    dY2 = iY - 10;
                            
                        //    dX1Ret = (dX1 - (c * OM.NodePos[n].dWrkPitch)) * dWidthScale;
                        //    dX2Ret = (dX2 - (c * OM.NodePos[n].dWrkPitch) - OM.DevOptn.dCutMovePitch) * dWidthScale;
                        //}
                        //else
                        //{
                        //    dX1 = dCenterPos - (OM.NodePos[n].dWrkSttPos - dNodeMinPos) + ((dNodeMaxPos - dNodeMinPos) / 2);
                        //    dY1 = iY;
                        //    dX2 = dX1;
                        //    dY2 = iY + 10;

                        //    dX1Ret = (dX1 - (c * OM.NodePos[n].dWrkPitch)) * dWidthScale;
                        //    dX2Ret = (dX2 - (c * OM.NodePos[n].dWrkPitch) - OM.DevOptn.dCutMovePitch) * dWidthScale;
                        //}
                    }
                    else
                    {
                        dX1 = OM.NodePos[n].dWrkSttPos + OM.DevOptn.dSttEmpty;
                        //dX1 = ((dNodeMaxPos + dNodeMinPos) / 2) + OM.NodePos[n].dWrkSttPos;//(OM.NodePos[n].dWrkSttPos - dNodeMinPos) + ((dNodeMaxPos + dNodeMinPos) / 2);
                        dY1 = iY;
                        dX2 = dX1;

                        dX1Ret = (dX1 + (c * OM.NodePos[n].dWrkPitch)) * dWidthScale;
                        dX2Ret = (dX2 + (c * OM.NodePos[n].dWrkPitch) + OM.DevOptn.dRightCutMovePitch) * dWidthScale;

                        if (OM.NodePos[n].dDegree == 0)
                        {
                            dY2 = iY - 10;
                        }
                        else
                        {
                            dY2 = iY + 10;
                        }
                        //if (OM.NodePos[n].dDegree == 0)
                        //{
                        //    dX1 = dCenterPos + (OM.NodePos[n].dWrkSttPos - dNodeMinPos) + ((dNodeMaxPos + dNodeMinPos) / 2);
                        //    dY1 = iY;
                        //    dX2 = dX1;
                        //    dY2 = iY - 10;

                        //    dX1Ret = (dX1 + (c * OM.NodePos[n].dWrkPitch)) * dWidthScale;
                        //    dX2Ret = (dX2 + (c * OM.NodePos[n].dWrkPitch) + OM.DevOptn.dCutMovePitch) * dWidthScale;
                        //}
                        //else
                        //{
                        //    dX1 = dCenterPos + (OM.NodePos[n].dWrkSttPos - dNodeMinPos) + ((dNodeMaxPos + dNodeMinPos) / 2);
                        //    dY1 = iY;
                        //    dX2 = dX1;
                        //    dY2 = iY + 10;

                        //    dX1Ret = (dX1 + (c * OM.NodePos[n].dWrkPitch)) * dWidthScale;
                        //    dX2Ret = (dX2 + (c * OM.NodePos[n].dWrkPitch) + OM.DevOptn.dCutMovePitch) * dWidthScale;
                        //}
                    }

                    g.DrawLine(Pen, (float)dX1Ret, (float)dY1, (float)dX2Ret, (float)dY2);
                }
            }
            Pen.Dispose();
        }

        
        public double WorkDistanceCal()
        {
            double dWorkDist = 0.0;
            dNodeMaxPos = 0;
            dNodeMinPos = 1000;


            for (int i = 0; i < OM.MAX_NODE_POS; i++)
            {
                if (OM.NodePos[i].dWrkSttPos != 0 || OM.NodePos[i].dWrkPitch != 0 || OM.NodePos[i].iWrkCnt != 0)
                {
                    dWorkDist = OM.NodePos[i].dWrkSttPos + ((OM.NodePos[i].iWrkCnt - 1) * OM.NodePos[i].dWrkPitch);
                    if (dNodeMaxPos < dWorkDist) dNodeMaxPos = dWorkDist ;
                    if (dNodeMinPos > dWorkDist) dNodeMinPos = dWorkDist; 
                }
            }

            dWorkDist = OM.DevOptn.dSttEmpty + dNodeMaxPos + OM.DevOptn.dEndEmpty;

            return dWorkDist;
        }









    }
}
