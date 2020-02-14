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
using SMDll2;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Machine
{
    public partial class FormDeviceSet : Form
    {
        public static FormDeviceSet FrmDeviceSet ;
        public        FraMotr    [] FraMotr      ;
        public        FraCylOneBt[] FraCylinder  ;
        public        FraOutput  [] FraOutput    ;
        public        FormMain      FrmMain      ;

        //FrameMotr 폼 갯수
        const int FRAMEMOTR_COUNT = 4;
        const int CYLINDER_COUNT  = 6;
        const int OUTPUT_COUNT    = 16;
        
        //CPstnMan PstnCnt;

        public  FormDeviceSet(Panel _pnBase)
        {
            InitializeComponent();
            
//            InitNodePosView();
            
            this.Width = 1272;
            this.Height = 866;

            this.TopLevel = false;
            this.Parent = _pnBase;

            tbUserUnit.Text = 0.01.ToString();
            PstnDisp();

            

            //모터 축에 대한 포지션 디스플레이
            PM.SetWindow(pnMotrPos0, (int)mi.LDR_Z);
            PM.SetWindow(pnMotrPos1, (int)mi.IDX_X);
            PM.SetWindow(pnMotrPos3, (int)mi.PCK_Z);
            PM.SetWindow(pnMotrPos2, (int)mi.PCK_Y);

            PM.SetGetCmdPos((int)mi.LDR_Z, SM.MT.GetCmdPos);
            PM.SetGetCmdPos((int)mi.IDX_X, SM.MT.GetCmdPos);
            PM.SetGetCmdPos((int)mi.PCK_Z, SM.MT.GetCmdPos);
            PM.SetGetCmdPos((int)mi.PCK_Y, SM.MT.GetCmdPos);

            OM.LoadLastInfo();
            PM.Load(OM.GetCrntDev().ToString());

            PM.UpdatePstn(true);
            UpdateDevInfo(true);
            UpdateDevOptn(true);
//            UpdateNodePos(true);


            FraMotr     = new FraMotr    [FRAMEMOTR_COUNT];
            FraCylinder = new FraCylOneBt[CYLINDER_COUNT] ;
            FraOutput   = new FraOutput  [OUTPUT_COUNT]   ;

            //모터 축 수에 맞춰 FrameMotr 생성
            for (int m = 0; m < (int)FRAMEMOTR_COUNT; m++)
            {
                Control[] Ctrl = tcDeviceSet.Controls.Find("pnMotrJog" +  m.ToString(), true);

                FraMotr[m] = new FraMotr();
                FraMotr[m].SetIdType((mi)m, SM.MT.GetDirType(m));
                FraMotr[m].TopLevel = false;
                FraMotr[m].Parent   = Ctrl[0];
                FraMotr[m].Show();
                FraMotr[m].SetUnit(EN_UNIT_TYPE.utJog, 0);
            }

            //실린더 수에 맞춰 FrameCylinder 생성
            for (int i = 0; i < (int)CYLINDER_COUNT; i++)
            {
                Control[] Ctrl          = tcDeviceSet.Controls.Find("pnAtcr" + i.ToString(), true);

                FraCylinder[i]          = new FraCylOneBt();
                FraCylinder[i].TopLevel = false;

                switch (i)
                {
                    default                               : break;
                    case (int)ai.LDR_GripClOp   : FraCylinder[i].SetConfig((ai)i, SM.CL.GetName(i).ToString(), SM.CL.GetDirType(i), Ctrl[0]); break;
                    case (int)ai.LDR_GripDnUp   : FraCylinder[i].SetConfig((ai)i, SM.CL.GetName(i).ToString(), SM.CL.GetDirType(i), Ctrl[0]); break;
                    case (int)ai.LDR_GripRtLt   : FraCylinder[i].SetConfig((ai)i, SM.CL.GetName(i).ToString(), SM.CL.GetDirType(i), Ctrl[0]); break;
                    case (int)ai.LDR_TrayFixClOp: FraCylinder[i].SetConfig((ai)i, SM.CL.GetName(i).ToString(), SM.CL.GetDirType(i), Ctrl[0]); break;
                    case (int)ai.IDX_IdxUpDn    : FraCylinder[i].SetConfig((ai)i, SM.CL.GetName(i).ToString(), SM.CL.GetDirType(i), Ctrl[0]); break;
                    case (int)ai.IDX_StockUpDn  : FraCylinder[i].SetConfig((ai)i, SM.CL.GetName(i).ToString(), SM.CL.GetDirType(i), Ctrl[0]); break;
                }
                FraCylinder[i].Show();
            }

            //Output 버튼 생성
            for (int i = 0; i < (int)OUTPUT_COUNT; i++)
            {
                Control[] Ctrl = tcDeviceSet.Controls.Find("pnIO" + i.ToString(), true);
                
                int iIOCtrl = Convert.ToInt32(Ctrl[0].Tag);
                
                FraOutput[i] = new FraOutput();
                FraOutput[i].TopLevel = false;

                switch (iIOCtrl)
                {  
                    default                  :                                                                                        break;
                    case (int)yi.PCK_Vcc1    : FraOutput[i].SetConfig(yi.PCK_Vcc1   , SM.IO.GetYName((int)yi.PCK_Vcc1  )  , Ctrl[0]); break;
                    case (int)yi.PCK_Vcc2    : FraOutput[i].SetConfig(yi.PCK_Vcc2   , SM.IO.GetYName((int)yi.PCK_Vcc2  )  , Ctrl[0]); break;
                    case (int)yi.PCK_Vcc3    : FraOutput[i].SetConfig(yi.PCK_Vcc3   , SM.IO.GetYName((int)yi.PCK_Vcc3  )  , Ctrl[0]); break;
                    case (int)yi.PCK_Vcc4    : FraOutput[i].SetConfig(yi.PCK_Vcc4   , SM.IO.GetYName((int)yi.PCK_Vcc4  )  , Ctrl[0]); break;
                    case (int)yi.PCK_Vcc5    : FraOutput[i].SetConfig(yi.PCK_Vcc5   , SM.IO.GetYName((int)yi.PCK_Vcc5  )  , Ctrl[0]); break;
                    case (int)yi.PCK_Vcc6    : FraOutput[i].SetConfig(yi.PCK_Vcc6   , SM.IO.GetYName((int)yi.PCK_Vcc6  )  , Ctrl[0]); break;
                    case (int)yi.PCK_Vcc7    : FraOutput[i].SetConfig(yi.PCK_Vcc7   , SM.IO.GetYName((int)yi.PCK_Vcc7  )  , Ctrl[0]); break;
                    case (int)yi.PCK_Vcc8    : FraOutput[i].SetConfig(yi.PCK_Vcc8   , SM.IO.GetYName((int)yi.PCK_Vcc8  )  , Ctrl[0]); break;
                    case (int)yi.PCK_Ejt1    : FraOutput[i].SetConfig(yi.PCK_Ejt1   , SM.IO.GetYName((int)yi.PCK_Ejt1  )  , Ctrl[0]); break;
                    case (int)yi.PCK_Ejt2    : FraOutput[i].SetConfig(yi.PCK_Ejt2   , SM.IO.GetYName((int)yi.PCK_Ejt2  )  , Ctrl[0]); break;
                    case (int)yi.PCK_Ejt3    : FraOutput[i].SetConfig(yi.PCK_Ejt3   , SM.IO.GetYName((int)yi.PCK_Ejt3  )  , Ctrl[0]); break;
                    case (int)yi.PCK_Ejt4    : FraOutput[i].SetConfig(yi.PCK_Ejt4   , SM.IO.GetYName((int)yi.PCK_Ejt4  )  , Ctrl[0]); break;
                    case (int)yi.PCK_Ejt5    : FraOutput[i].SetConfig(yi.PCK_Ejt5   , SM.IO.GetYName((int)yi.PCK_Ejt5  )  , Ctrl[0]); break;
                    case (int)yi.PCK_Ejt6    : FraOutput[i].SetConfig(yi.PCK_Ejt6   , SM.IO.GetYName((int)yi.PCK_Ejt6  )  , Ctrl[0]); break;
                    case (int)yi.PCK_Ejt7    : FraOutput[i].SetConfig(yi.PCK_Ejt7   , SM.IO.GetYName((int)yi.PCK_Ejt7  )  , Ctrl[0]); break;
                    case (int)yi.PCK_Ejt8    : FraOutput[i].SetConfig(yi.PCK_Ejt8   , SM.IO.GetYName((int)yi.PCK_Ejt8  )  , Ctrl[0]); break;
                }
              
                FraOutput[i].Show();
            }
        }

//        private void InitNodePosView()
//        {
//            //NodePositions ListView==========================
//            btZWaitMove = new Button[(int)OM.MAX_NODE_POS];
//            btMove      = new Button[(int)OM.MAX_NODE_POS];
//            btInput     = new Button[(int)OM.MAX_NODE_POS];
//
//            const int iHeadHeight = 23 ;
//            const int iCellPitch  = 21 ;
//            const int iCellWidth  = 75 ;
//            const int iXYZWidth   = 72 ;
//            const int iButtonWidth = 50 ;
//            const int iButtonOffset = 10 ;
//            
//            lvNodePos.Clear();
//            lvNodePos.View = View.Details;
//            lvNodePos.FullRowSelect = true;
//            lvNodePos.MultiSelect   = true;
//            lvNodePos.GridLines     = true; 
//            
//
//            lvNodePos.Columns.Add("No"               , 35         ,HorizontalAlignment.Left);
//            lvNodePos.Columns.Add("Bf Feed (ms)"     , iCellWidth ,HorizontalAlignment.Left);
//            lvNodePos.Columns.Add("X (mm)"           , iXYZWidth  ,HorizontalAlignment.Left);
//            lvNodePos.Columns.Add("Y (mm)"           , iXYZWidth  ,HorizontalAlignment.Left);
//            lvNodePos.Columns.Add("Z (mm)"           , iXYZWidth  , HorizontalAlignment.Left);
//            lvNodePos.Columns.Add("Vel (mm/s)"       , iCellWidth , HorizontalAlignment.Left);            
//            lvNodePos.Columns.Add("Move Wait (ms)"   , iCellWidth , HorizontalAlignment.Left);       
//            lvNodePos.Columns.Add("Move Feed (ms)"   , iCellWidth , HorizontalAlignment.Left);    
//            lvNodePos.Columns.Add("At Move (ms)"     , iCellWidth , HorizontalAlignment.Left);    
//
//            lvNodePos.Columns.Add("Z WaitMove"       , iButtonWidth, HorizontalAlignment.Left);    
//            lvNodePos.Columns.Add("Move"             , iButtonWidth, HorizontalAlignment.Left);    
//            lvNodePos.Columns.Add("Input"            , iButtonWidth, HorizontalAlignment.Left);         

//            ListViewItem [] liInput = new ListViewItem [(int)OM.MAX_NODE_POS];
//            for (int i = 0; i < (int)OM.MAX_NODE_POS; i++)
//            {
//                btZWaitMove[i] = new Button();  
//                btZWaitMove[i].Tag = i ;
//                btZWaitMove[i].Width = iButtonWidth ; btZWaitMove[i].Height=iCellPitch; btZWaitMove[i].Text = "ZMove";
//                btZWaitMove[i].Click += btNodeZMoveClick ;
//                lvNodePos.Controls.Add(btZWaitMove[i]); btZWaitMove[i].Left = lvNodePos.Width - iButtonWidth*3 -iButtonOffset; btZWaitMove[i].Top = iHeadHeight + i*iCellPitch ;
//
//                btMove     [i] = new Button();  
//                btMove     [i].Tag = i ;
//                btMove     [i].Width = iButtonWidth ; btMove     [i].Height=iCellPitch; btMove     [i].Text = "Move";
//                btMove     [i].Click += btNodeMoveClick ;
//                lvNodePos.Controls.Add(btMove     [i]); btMove     [i].Left = lvNodePos.Width - iButtonWidth*2 -iButtonOffset; btMove     [i].Top = iHeadHeight + i*iCellPitch ;
//
//                btInput    [i] = new Button();  
//                btInput    [i].Tag = i ;
//                btInput    [i].Width = iButtonWidth ; btInput    [i].Height=iCellPitch; btInput    [i].Text = "Input";
//                btInput    [i].Click += btNodeInputClick ;
//                lvNodePos.Controls.Add(btInput    [i]); btInput    [i].Left = lvNodePos.Width - iButtonWidth   -iButtonOffset; btInput    [i].Top = iHeadHeight + i*iCellPitch ;
//
//
//                liInput[i] = new ListViewItem(i.ToString());
//                for (int j = 1; j < lvNodePos.Columns.Count; j++) {  
//                    liInput[i].SubItems.Add("");
//                }
//                
//                liInput[i].UseItemStyleForSubItems = false;
//                lvNodePos.Items.Add(liInput[i]);
//            }
//
//            var PropInput = lvNodePos.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
//            PropInput.SetValue(lvNodePos, true, null);
//        }

//        private void btNodeZMoveClick(object sender, EventArgs e)
//        {
//            Log.Trace("btNodeZMoveClick", "Clicked");       
//            int iBtnTag = Convert.ToInt32(((Button)sender).Tag) ;
//            MM.m_iNodeNo = iBtnTag;
//            MM.SetManCycle(mc.SLD_ZWaitMove);
//            
//        }
//        private void btNodeMoveClick(object sender, EventArgs e)
//        {
//            Log.Trace("btNodeMoveClick", "Clicked"); 
//            int iBtnTag = Convert.ToInt32(((Button)sender).Tag) ;
//            MM.m_iNodeNo = iBtnTag;
//            MM.SetManCycle(mc.SLD_Move);      
//        }
//        private void btNodeInputClick(object sender, EventArgs e)
//        {
//            Log.Trace("btNodeMoveClick", "Clicked");    
//            int iBtnTag = Convert.ToInt32(((Button)sender).Tag) ;
//            MM.m_iNodeNo = iBtnTag;
//
//            double dPosX = SM.MT.GetCmdPos((int)mi.SLD_XMotr) - PM.GetValue(mi.SLD_XMotr, pv.SLD_XWorkStt);
//            double dPosY = SM.MT.GetCmdPos((int)mi.SLD_YMotr) - PM.GetValue(mi.SLD_YMotr, pv.SLD_YWorkStt);
//            double dPosZ = SM.MT.GetCmdPos((int)mi.SLD_ZMotr) - PM.GetValue(mi.SLD_ZMotr, pv.SLD_ZWorkStt);
//
//            lvNodePos.Items[iBtnTag].SubItems[2].Text = dPosX.ToString();
//            lvNodePos.Items[iBtnTag].SubItems[3].Text = dPosY.ToString();
//            lvNodePos.Items[iBtnTag].SubItems[4].Text = dPosZ.ToString();
//        }

        public void PstnDisp()
        {
            //Loader Z축 포지션 Property
            PM.SetProp((uint)mi.LDR_Z, (uint)pv.LDR_ZWait    , "Wait    ", false, false, false);
            PM.SetProp((uint)mi.LDR_Z, (uint)pv.LDR_ZWorkStt , "WorkStt ", false, false, false);
                                                                        
            //Index X축 포지션 Property                                 
            PM.SetProp((uint)mi.IDX_X, (uint)pv.IDX_XWait    , "Wait    ", false, false, false);
            PM.SetProp((uint)mi.IDX_X, (uint)pv.IDX_XPickStt , "PickStt ", false, false, false);
            PM.SetProp((uint)mi.IDX_X, (uint)pv.IDX_XOut     , "Out     ", false, false, false);
            
            //Picker Y축 포지션 Property
            PM.SetProp((uint)mi.PCK_Y, (uint)pv.PCK_YWait    , "YWait   ", false, false, false);
            PM.SetProp((uint)mi.PCK_Y, (uint)pv.PCK_YPick    , "YPick   ", false, false, false);
            PM.SetProp((uint)mi.PCK_Y, (uint)pv.PCK_YVisn    , "YVision ", false, false, false);
            PM.SetProp((uint)mi.PCK_Y, (uint)pv.PCK_YPrnt    , "YPrint  ", false, false, false);
            PM.SetProp((uint)mi.PCK_Y, (uint)pv.PCK_YPlce    , "YPlace  ", false, false, false);

            //Picker Z축 포지션 Property
            PM.SetProp((uint)mi.PCK_Z, (uint)pv.PCK_ZWait, "ZWait   ", false, false, false);
            PM.SetProp((uint)mi.PCK_Z, (uint)pv.PCK_ZMove, "ZMove   ", false, false, false);
            PM.SetProp((uint)mi.PCK_Z, (uint)pv.PCK_ZPick, "ZPick   ", false, false, false);
            PM.SetProp((uint)mi.PCK_Z, (uint)pv.PCK_ZVisn, "ZVision ", false, false, false);
            PM.SetProp((uint)mi.PCK_Z, (uint)pv.PCK_ZPrnt, "ZPrint  ", false, false, false);
            PM.SetProp((uint)mi.PCK_Z, (uint)pv.PCK_ZPlce, "ZPlace  ", false, false, false);
            
        }

        private void tcDeviceSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            int iSeletedIndex;
            iSeletedIndex = tcDeviceSet.SelectedIndex;
            
            switch (iSeletedIndex)
            {
                default : break;
                case 1  : gbJogUnit.Parent = pnJog1; break;
                case 2  : gbJogUnit.Parent = pnJog2; break;
                case 3  : gbJogUnit.Parent = pnJog3; break;
                case 4  : EmbededExe.SetCamParent(pnVisn.Handle);break;
            }

            UpdateDevInfo(true);
            UpdateDevOptn(true);
//            UpdateNodePos(true);
            PM.UpdatePstn(true);
        }

        public IntPtr GetCamPnHandle()
        {
            return pnVisn.Handle;
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            Log.Trace("SAVE", "Clicked");

            if (double.Parse(tbPckShakeDistance.Text) >= 2)
            {
                if(Log.ShowMessageModal("Warning", "Picker Shake Distance is High, Do you want to save?") != DialogResult.Yes) return;
            }
            if (Log.ShowMessageModal("Confirm", "Are you Sure?") != DialogResult.Yes) return;

            UpdateDevInfo(false);
            UpdateDevOptn(false);

            //Picker Shake 기능 사용 여부에 따라서 거리 텍스트박스 사용 할지 안할지 결정
            //if (OM.DevOptn.bUsePckShake)
            //{
            //    tbPckShakeDistance.Enabled = true;
            //    tbPckShakeCnt     .Enabled = true;
            //    tbPckShakeZOfs    .Enabled = true;
            //}
            //else
            //{
            //    tbPckShakeDistance.Enabled = false;
            //    tbPckShakeCnt     .Enabled = false;
            //    tbPckShakeZOfs    .Enabled = false;
            //}

            
//            UpdateNodePos(false);

            OM.SaveDevInfo(OM.GetCrntDev().ToString());
            OM.SaveDevOptn(OM.GetCrntDev().ToString());

            

            PM.UpdatePstn(false);
            PM.Save(OM.GetCrntDev());

//            DM.ARAY[(int)ri.SLD].SetMaxColRow(OM.DevInfo.iSTGColCnt, OM.DevInfo.iSTGRowCnt);
            pbSTG.Refresh();
            OM.SaveEqpOptn();

            SEQ.Com[1].PrintSendMsg();

            SEQ.Com[1].PrintSendMsg();
            

            //Thread.Sleep(100);
            
            //SEQ.Com[1].SendMsg(OM.DevInfo.sMrkData);
            Refresh();

        }

        
        public void UpdateDevInfo(bool bToTable)
        {
            if (bToTable)
            {
                tbTrayColCnt  .Text = OM.DevInfo.iTrayColCnt  .ToString();
                tbTrayRowCnt  .Text = OM.DevInfo.iTrayRowCnt  .ToString();
                tbTrayColPitch.Text = OM.DevInfo.dTrayColPitch.ToString();
                tbTrayRowPitch.Text = OM.DevInfo.dTrayRowPitch.ToString();

                tbTrayHigtPitch.Text = OM.DevInfo.dTrayHeight.ToString();

                tbVsnInspColCnt.Text = OM.DevInfo.iVsnInspColCnt.ToString();
                tbVsnInspRowCnt.Text = OM.DevInfo.iVsnInspRowCnt.ToString();

                tbVsnIndx      .Text = OM.DevInfo.sVsnIndx;
                tbMrkData      .Text = OM.DevInfo.sMrkData;

                tbReptDelay    .Text = OM.DevInfo.iMrkReptDelay.ToString();
                

                if (CConfig.StrToIntDef(tbVsnInspColCnt.Text, 1) <= 0) { tbVsnInspColCnt.Text = 1.ToString(); }
                if (CConfig.StrToIntDef(tbVsnInspRowCnt.Text, 1) <= 0) { tbVsnInspRowCnt.Text = 1.ToString(); }

                
            }
            else 
            {
                if (CConfig.StrToIntDef(tbTrayColCnt.Text, 1) <= 0) { tbTrayColCnt.Text = 1.ToString(); }
                if (CConfig.StrToIntDef(tbTrayRowCnt.Text, 1) <= 0) { tbTrayRowCnt.Text = 1.ToString(); }

                if (CConfig.StrToIntDef(tbVsnInspColCnt.Text, 1) <= 0) { tbVsnInspColCnt.Text = 1.ToString(); }
                if (CConfig.StrToIntDef(tbVsnInspRowCnt.Text, 1) <= 0) { tbVsnInspRowCnt.Text = 1.ToString(); }

                OM.DevInfo.iTrayColCnt   =  CConfig.StrToIntDef  (tbTrayColCnt  . Text, 1  ) ;
                OM.DevInfo.iTrayRowCnt   =  CConfig.StrToIntDef  (tbTrayRowCnt  . Text, 1  ) ;
                OM.DevInfo.dTrayColPitch =  CConfig.StrToIntDef  (tbTrayColPitch. Text, 1  ) ;
                OM.DevInfo.dTrayRowPitch =  CConfig.StrToIntDef  (tbTrayRowPitch. Text, 1  ) ;

                OM.DevInfo.dTrayHeight   = CConfig.StrToIntDef   (tbTrayHigtPitch.Text, 1);

                OM.DevInfo.iVsnInspColCnt = CConfig.StrToIntDef(tbVsnInspColCnt.Text, 1);
                OM.DevInfo.iVsnInspRowCnt = CConfig.StrToIntDef(tbVsnInspRowCnt.Text, 1);

                OM.DevInfo.sVsnIndx = tbVsnIndx.Text;
                OM.DevInfo.sMrkData = tbMrkData.Text;

                OM.DevInfo.iMrkReptDelay = CConfig.StrToIntDef(tbReptDelay.Text , 0);

                
                
                UpdateDevInfo(true);
            }
        
        }

        public void UpdateDevOptn(bool bToTable)
        {

            if (bToTable)
            {
                tbLDRDetectTime     .Text    = OM.DevOptn.iLDRTrayCheckTime.ToString();
                tbPckWaitTime       .Text    = OM.DevOptn.iPickWaitTime    .ToString();
                tbAlgnWaitTime      .Text    = OM.DevOptn.iAlgnWaitTime    .ToString();
                tbPlceWaitTime      .Text    = OM.DevOptn.iPlceWaitTime    .ToString();
                                             
                tbPckShakeDistance  .Text    = OM.DevOptn.dPckShakeDistance.ToString();
                tbPckShakeZOfs      .Text    = OM.DevOptn.dPckShakeZOfs    .ToString();
                tbPckShakeCnt       .Text    = OM.DevOptn.iPckShakeCnt     .ToString();
                cbUsePckShake       .Checked = OM.DevOptn.bUsePckShake;


                if (OM.DevOptn.bUsePckShake)
                {
                    //cbUsePckShake.Checked = true;
                    tbPckShakeDistance.Enabled = true;
                    tbPckShakeCnt.Enabled = true;
                    tbPckShakeZOfs.Enabled = true;
                }
                else
                {
                    //cbUsePckShake.Checked = false;
                    tbPckShakeDistance.Enabled = false;
                    tbPckShakeCnt.Enabled = false;
                    tbPckShakeZOfs.Enabled = false;
                }
            }
            else
            {
                OM.DevOptn.iLDRTrayCheckTime = CConfig.StrToIntDef(tbLDRDetectTime.Text, 0);
                OM.DevOptn.iPickWaitTime     = CConfig.StrToIntDef(tbPckWaitTime  .Text, 0);
                OM.DevOptn.iAlgnWaitTime     = CConfig.StrToIntDef(tbAlgnWaitTime .Text, 0);
                OM.DevOptn.iPlceWaitTime     = CConfig.StrToIntDef(tbPlceWaitTime .Text, 0);

                OM.DevOptn.dPckShakeDistance = CConfig.StrToDoubleDef(tbPckShakeDistance.Text, 0.0);
                OM.DevOptn.dPckShakeZOfs     = CConfig.StrToDoubleDef(tbPckShakeZOfs    .Text, 0.0);
                OM.DevOptn.iPckShakeCnt      = CConfig.StrToIntDef   (tbPckShakeCnt     .Text, 0  );
                OM.DevOptn.bUsePckShake      = cbUsePckShake.Checked;
                UpdateDevOptn(true);
            }
        }

//        public void UpdateNodePos(bool bToTable)
//        {
//            if(bToTable)
//            {
//                for (int i = 0; i < OptnList.MAX_NODE_POS; i++) { 
//                    lvNodePos.Items[i].SubItems[0].Text = i.ToString();
//                    lvNodePos.Items[i].SubItems[1].Text = OM.NodePos[i].BfFeed .ToString();
//                    lvNodePos.Items[i].SubItems[2].Text = OM.NodePos[i].X      .ToString();
//                    lvNodePos.Items[i].SubItems[3].Text = OM.NodePos[i].Y      .ToString();
//                    lvNodePos.Items[i].SubItems[4].Text = OM.NodePos[i].Z      .ToString();
//                    lvNodePos.Items[i].SubItems[5].Text = OM.NodePos[i].Vel    .ToString();
//                    lvNodePos.Items[i].SubItems[6].Text = OM.NodePos[i].MvWait .ToString();
//                    lvNodePos.Items[i].SubItems[7].Text = OM.NodePos[i].MvFeed .ToString();
//                    lvNodePos.Items[i].SubItems[8].Text = OM.NodePos[i].AtWait .ToString();
//                }
//            }
//            else
//            {
//                for (int i = 0; i < OptnList.MAX_NODE_POS; i++) { 
//                    int   .TryParse(lvNodePos.Items[i].SubItems[1].Text,out OM.NodePos[i].BfFeed);
//                    double.TryParse(lvNodePos.Items[i].SubItems[2].Text,out OM.NodePos[i].X     );
//                    double.TryParse(lvNodePos.Items[i].SubItems[3].Text,out OM.NodePos[i].Y     );
//                    double.TryParse(lvNodePos.Items[i].SubItems[4].Text,out OM.NodePos[i].Z     );
//                    double.TryParse(lvNodePos.Items[i].SubItems[5].Text,out OM.NodePos[i].Vel   );
//                    int   .TryParse(lvNodePos.Items[i].SubItems[6].Text,out OM.NodePos[i].MvWait);
//                    int   .TryParse(lvNodePos.Items[i].SubItems[7].Text,out OM.NodePos[i].MvFeed);
//                    int   .TryParse(lvNodePos.Items[i].SubItems[8].Text,out OM.NodePos[i].AtWait);
//                }
//            }
//        }

        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            tmUpdate.Enabled = false;
            btSaveDevice.Enabled = !SEQ._bRun;

            bool bBackColor = false;

//            for(int i = 0; i < (int)OptnList.MAX_NODE_POS; i++)
//            {
//                if(i!=0){
//                    bBackColor = CConfig.StrToDoubleDef(lvNodePos.Items[i].SubItems[2].Text, 0.0) != 0 ||
//                                 CConfig.StrToDoubleDef(lvNodePos.Items[i].SubItems[3].Text, 0.0) != 0 ||
//                                 CConfig.StrToDoubleDef(lvNodePos.Items[i].SubItems[4].Text, 0.0) != 0 ;
//                }
//                else{
//                    bBackColor = true;
//                }
//                
//                if(bBackColor)
//                {
//                    for(int j = 0; j < lvNodePos.Columns.Count; j++)
//                    {
//                        lvNodePos.Items[i].SubItems[j].BackColor = bBackColor ? Color.Aqua : Color.White;    
//                    }
//                    
//                }
//                
//            }

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


        private void pbSTG_Paint(object sender, PaintEventArgs e)
        {
            SolidBrush Brush = new SolidBrush(Color.Black);

            Pen Pen = new Pen(Color.Black);

            Graphics gSTG = pbSTG.CreateGraphics();


            double dX1, dX2, dY1, dY2;

            int iSTGColCnt, iSTGRowCnt;

            iSTGColCnt = OM.DevInfo.iTrayColCnt;
            iSTGRowCnt = OM.DevInfo.iTrayRowCnt;

            int iGetWidth = pbSTG.Width;
            int iGetHeight = pbSTG.Height;

            double iSetWidth = 0, iSetHeight = 0;

            double uGw = (double)iGetWidth  / (double)(iSTGColCnt);
            double uGh = (double)iGetHeight / (double)(iSTGRowCnt);
            double dWOff = (double)(iGetWidth - uGw * (iSTGColCnt)) / 2.0;
            double dHOff = (double)(iGetHeight - uGh * (iSTGRowCnt)) / 2.0;

            Graphics g = e.Graphics;

            Pen.Color = Color.Black;

            Brush.Color = Color.HotPink;
            

            for (int r = 0; r < iSTGRowCnt; r++)
            {
                for (int c = 0; c < iSTGColCnt; c++)
                {
                    
                    dY1 = dHOff + r * uGh -1;
                    dY2 = dHOff + r * uGh + uGh ;
                    dX1 = dWOff + c * uGw -1;
                    dX2 = dWOff + c * uGw + uGw ;

                    g.FillRectangle(Brush, (float)dX1, (float)dY1, (float)dX2, (float)dY2);
                    g.DrawRectangle(Pen  , (float)dX1, (float)dY1, (float)dX2, (float)dY2);

                    iSetWidth += dY2;
                    iSetHeight += dX2;
                }

            }
            
        }

        private void pbVSNInsp_Paint(object sender, PaintEventArgs e)
        {
            SolidBrush Brush = new SolidBrush(Color.Black);

            Pen Pen = new Pen(Color.Black);

            Graphics gVSNInsp = pbVSNInsp.CreateGraphics();


            double dX1, dX2, dY1, dY2;

            int iVSNInspColCnt, iVSNInspRowCnt;

            iVSNInspColCnt = OM.DevInfo.iVsnInspColCnt;
            iVSNInspRowCnt = OM.DevInfo.iVsnInspRowCnt;

            int iGetWidth = pbVSNInsp.Width;
            int iGetHeight = pbVSNInsp.Height;

            double iSetWidth = 0, iSetHeight = 0;

            double uGw = (double)iGetWidth / (double)(iVSNInspColCnt);
            double uGh = (double)iGetHeight / (double)(iVSNInspRowCnt);
            double dWOff = (double)(iGetWidth - uGw * (iVSNInspColCnt)) / 2.0;
            double dHOff = (double)(iGetHeight - uGh * (iVSNInspRowCnt)) / 2.0;

            Graphics g = e.Graphics;

            Pen.Color = Color.Black;

            Brush.Color = Color.HotPink;


            for (int r = 0; r < iVSNInspRowCnt; r++)
            {
                for (int c = 0; c < iVSNInspColCnt; c++)
                {

                    dY1 = dHOff + r * uGh - 1;
                    dY2 = dHOff + r * uGh + uGh;
                    dX1 = dWOff + c * uGw - 1;
                    dX2 = dWOff + c * uGw + uGw;

                    g.FillRectangle(Brush, (float)dX1, (float)dY1, (float)dX2, (float)dY2);
                    g.DrawRectangle(Pen, (float)dX1, (float)dY1, (float)dX2, (float)dY2);

                    iSetWidth += dY2;
                    iSetHeight += dX2;
                }

            }
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

        private void btApply_Click(object sender, EventArgs e)
        {
//            if( lvNodePos.SelectedItems.Count==0)return;  //선택된 것이 없으면 그냥 나감
//            int sel=lvNodePos.SelectedItems[0].Index;   //현재 선택된것..
//
//            int    BfFeed ;
//            double X      ;
//            double Y      ;
//            double Z      ;
//            double Vel    ;
//            int    MvWait ;
//            int    MvFeed ;
//            int    AtWait ;
//
//            if(int   .TryParse(textBox1.Text,out BfFeed))lvNodePos.Items[sel].SubItems[1].Text = BfFeed.ToString() ;
//            if(double.TryParse(textBox2.Text,out X     ))lvNodePos.Items[sel].SubItems[2].Text = X     .ToString() ;
//            if(double.TryParse(textBox3.Text,out Y     ))lvNodePos.Items[sel].SubItems[3].Text = Y     .ToString() ;
//            if(double.TryParse(textBox4.Text,out Z     ))lvNodePos.Items[sel].SubItems[4].Text = Z     .ToString() ;
//            if(double.TryParse(textBox5.Text,out Vel   ))lvNodePos.Items[sel].SubItems[5].Text = Vel   .ToString() ;
//            if(int   .TryParse(textBox6.Text,out MvWait))lvNodePos.Items[sel].SubItems[6].Text = MvWait.ToString() ;
//            if(int   .TryParse(textBox7.Text,out MvFeed))lvNodePos.Items[sel].SubItems[7].Text = MvFeed.ToString() ;
//            if(int   .TryParse(textBox8.Text,out AtWait))lvNodePos.Items[sel].SubItems[8].Text = AtWait.ToString() ;
        }


        private void FormDeviceSet_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmUpdate.Enabled = false;
        }

        private void FormDeviceSet_Shown(object sender, EventArgs e)
        {
//            EmbededExe.SetCamParent(pnCamera.Handle);
        }

//        public IntPtr GetCamPnHandle()
//        {
//            return pnCamera.Handle;
//        }

        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //싸이즈가 찐따나서 한번 더 해준다.
//            EmbededExe.SetCamParent(pnCamera.Handle);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label54_Click(object sender, EventArgs e)
        {

        }
    }
}
