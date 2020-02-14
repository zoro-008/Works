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
using SML;
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
        //const int FRAMEMOTR_COUNT = 4;
        //const int CYLINDER_COUNT  = 6;
        //
        
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


            PM.SetGetCmdPos((int)mi.LDR_Z, SM.MTR.GetCmdPos);
            PM.SetGetCmdPos((int)mi.IDX_X, SM.MTR.GetCmdPos);


            OM.LoadLastInfo();
            PM.Load(OM.GetCrntDev().ToString());

            PM.UpdatePstn(true);
            UpdateDevInfo(true);
            UpdateDevOptn(true);
//            UpdateNodePos(true);


            
            
            

            //모터 축 수에 맞춰 FrameMotr 생성
            FraMotr = new FraMotr[(int)mi.MAX_MOTR];
            for (int m = 0; m < (int)mi.MAX_MOTR; m++)
            {
                Control[] Ctrl = tcDeviceSet.Controls.Find("pnMotrJog" +  m.ToString(), true);

                FraMotr[m] = new FraMotr();
                FraMotr[m].SetIdType((mi)m, SM.MTR.GetDirType(m));
                FraMotr[m].TopLevel = false;
                FraMotr[m].Parent   = Ctrl[0];
                FraMotr[m].Show();
                FraMotr[m].SetUnit(EN_UNIT_TYPE.utJog, 0);
            }

            //실린더 수에 맞춰 FrameCylinder 생성
            FraCylinder = new FraCylOneBt[(int)ci.MAX_CYL ] ;
            for (int i = 0; i < (int)ci.MAX_CYL; i++)
            {
                Control[] Ctrl          = tcDeviceSet.Controls.Find("pnAtcr" + i.ToString(), true);

                FraCylinder[i]          = new FraCylOneBt();
                FraCylinder[i].TopLevel = false;

                switch (i)
                {
                    default                               : break;
                    case (int)ci.LDR_GripClOp   : FraCylinder[i].SetConfig((ci)i, SM.CYL.GetName(i).ToString(), SM.CYL.GetDirType(i), Ctrl[0],SEQ.LDR.CheckSafe); break;
                    case (int)ci.LDR_GripDnUp   : FraCylinder[i].SetConfig((ci)i, SM.CYL.GetName(i).ToString(), SM.CYL.GetDirType(i), Ctrl[0],SEQ.LDR.CheckSafe); break;
                    case (int)ci.LDR_GripRtLt   : FraCylinder[i].SetConfig((ci)i, SM.CYL.GetName(i).ToString(), SM.CYL.GetDirType(i), Ctrl[0],SEQ.LDR.CheckSafe); break;
                    case (int)ci.LDR_TrayFixClOp: FraCylinder[i].SetConfig((ci)i, SM.CYL.GetName(i).ToString(), SM.CYL.GetDirType(i), Ctrl[0],SEQ.LDR.CheckSafe); break;
                    case (int)ci.IDX_IdxUpDn    : FraCylinder[i].SetConfig((ci)i, SM.CYL.GetName(i).ToString(), SM.CYL.GetDirType(i), Ctrl[0],SEQ.IDX.CheckSafe); break;
                    case (int)ci.IDX_StockUpDn  : FraCylinder[i].SetConfig((ci)i, SM.CYL.GetName(i).ToString(), SM.CYL.GetDirType(i), Ctrl[0],SEQ.IDX.CheckSafe); break;
                }
                FraCylinder[i].Show();
            }

            //Output 버튼 생성
            const int OUTPUT_COUNT    = 0;
            FraOutput   = new FraOutput  [OUTPUT_COUNT]   ;
            for (int i = 0; i < (int)OUTPUT_COUNT; i++)
            {
                Control[] Ctrl = tcDeviceSet.Controls.Find("pnIO" + i.ToString(), true);
                
                int iIOCtrl = Convert.ToInt32(Ctrl[0].Tag);
                
                FraOutput[i] = new FraOutput();
                FraOutput[i].TopLevel = false;
                FraOutput[i].Show();
            }
        }

        public void PstnDisp()
        {
            //Loader Z축 포지션 Property
            PM.SetProp((uint)mi.LDR_Z, (uint)pv.LDR_ZWait    , "Wait    ", false, false, false);
            PM.SetProp((uint)mi.LDR_Z, (uint)pv.LDR_ZWorkStt , "WorkStt ", false, false, false);
                                                                        
            //Index X축 포지션 Property                                 
            PM.SetProp((uint)mi.IDX_X, (uint)pv.IDX_XWait    , "Wait    ", false, false, false);
            PM.SetProp((uint)mi.IDX_X, (uint)pv.IDX_XMark    , "Mark    ", false, false, false);
            PM.SetProp((uint)mi.IDX_X, (uint)pv.IDX_XBarcode , "Barcode ", false, false, false);
            PM.SetProp((uint)mi.IDX_X, (uint)pv.IDX_XOut     , "Out     ", false, false, false);
            
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
            }

            UpdateDevInfo(true);
            UpdateDevOptn(true);
            PM.UpdatePstn(true);
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            Log.Trace("SAVE", "Clicked");

            if (Log.ShowMessageModal("Confirm", "Are you Sure?") != DialogResult.Yes) return;
            
            string sPreLaser = OM.DevOptn.sLaserProject ;

            UpdateDevInfo(false);
            UpdateDevOptn(false);


            OM.SaveDevInfo(OM.GetCrntDev().ToString());
            OM.SaveDevOptn(OM.GetCrntDev().ToString());

            PM.UpdatePstn(false);
            PM.Save(OM.GetCrntDev());

            if(sPreLaser != OM.DevOptn.sLaserProject) SEQ.Laser.SetCycle(RS232_DominoDynamark3.Cycle.ProjectLoad, true); //에러는 안에서 띄움.

            pbSTG.Refresh();
            OM.SaveEqpOptn();
            OM.SaveEqpStat();

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

                tbTrayMaxLoading.Text = OM.DevInfo.iTrayMaxLoading.ToString();


                
            }
            else 
            {
                if (CConfig.StrToIntDef(tbTrayColCnt.Text, 1) <= 0) { tbTrayColCnt.Text = 1.ToString(); }
                if (CConfig.StrToIntDef(tbTrayRowCnt.Text, 1) <= 0) { tbTrayRowCnt.Text = 1.ToString(); }


                OM.DevInfo.iTrayColCnt   =  CConfig.StrToIntDef  (tbTrayColCnt  . Text, 1  ) ;
                OM.DevInfo.iTrayRowCnt   =  CConfig.StrToIntDef  (tbTrayRowCnt  . Text, 1  ) ;
                OM.DevInfo.dTrayColPitch =  CConfig.StrToDoubleDef  (tbTrayColPitch. Text, 1  ) ;
                OM.DevInfo.dTrayRowPitch =  CConfig.StrToDoubleDef  (tbTrayRowPitch. Text, 1  ) ;

                OM.DevInfo.dTrayHeight   = CConfig.StrToDoubleDef   (tbTrayHigtPitch.Text, 18.6);
                OM.DevInfo.iTrayMaxLoading=  CConfig.StrToIntDef  (tbTrayMaxLoading. Text, 9  ) ;
                
                
                UpdateDevInfo(true);
            }
        
        }

        public void UpdateDevOptn(bool bToTable)
        {

            if (bToTable)
            {
                tbLDRDetectTime     .Text    = OM.DevOptn.iLDRTrayCheckTime.ToString();
                tbLaserProject      .Text    = OM.DevOptn.sLaserProject ;
                tbStartSerial       .Text    = OM.DevOptn.iStartSerial.ToString();
                cbUseSerialDMC      .Checked = OM.DevOptn.bUseSerialDMC ;
            }
            else
            {
                OM.DevOptn.iLDRTrayCheckTime = CConfig.StrToIntDef(tbLDRDetectTime.Text, OM.DevOptn.iLDRTrayCheckTime);
                OM.DevOptn.sLaserProject     = tbLaserProject.Text  ;
                OM.DevOptn.iStartSerial      = CConfig.StrToIntDef(tbStartSerial  .Text, OM.DevOptn.iStartSerial);
                OM.DevOptn.bUseSerialDMC     = cbUseSerialDMC.Checked  ;
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
