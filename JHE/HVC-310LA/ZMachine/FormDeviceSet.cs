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

namespace Machine
{
    public partial class FormDeviceSet : Form
    {
        public static FormDeviceSet FrmDeviceSet ;
        public        FraMotr    [] FraMotr      ;
        public        FraCylOneBt[] FraCylinder  ;
        public        FraOutput  [] FraOutput    ;
        public        FormMain      FrmMain      ;

        //CPstnMan PstnCnt;

        public  FormDeviceSet(Panel _pnBase)
        {
            InitializeComponent();

            this.Width = 1272;
            this.Height = 866;

            this.TopLevel = false;
            this.Parent = _pnBase;

            tbUserUnit.Text = 0.01.ToString();
            PstnDisp();

            

            //모터 축에 대한 포지션 디스플레이
            PM.SetWindow(pnMotrPos0, (int)mi.PCK_X);
            PM.SetWindow(pnMotrPos1, (int)mi.STG_Y);
            PM.SetWindow(pnMotrPos2, (int)mi.PCK_ZL);
            PM.SetWindow(pnMotrPos3, (int)mi.PCK_ZR);
            PM.SetWindow(pnMotrPos4, (int)mi.PCK_TL);
            PM.SetWindow(pnMotrPos5, (int)mi.PCK_TR);

            PM.SetGetCmdPos((int)mi.PCK_X , SM.MT.GetCmdPos);
            PM.SetGetCmdPos((int)mi.STG_Y , SM.MT.GetCmdPos);
            PM.SetGetCmdPos((int)mi.PCK_ZL, SM.MT.GetCmdPos);
            PM.SetGetCmdPos((int)mi.PCK_ZR, SM.MT.GetCmdPos);
            PM.SetGetCmdPos((int)mi.PCK_TL, SM.MT.GetCmdPos);
            PM.SetGetCmdPos((int)mi.PCK_TR, SM.MT.GetCmdPos);

            PM.SetCheckSafe((int)mi.PCK_X , SEQ.ASY.CheckSafe);
            PM.SetCheckSafe((int)mi.STG_Y , SEQ.ASY.CheckSafe);
            PM.SetCheckSafe((int)mi.PCK_ZL, SEQ.ASY.CheckSafe);
            PM.SetCheckSafe((int)mi.PCK_ZR, SEQ.ASY.CheckSafe);
            PM.SetCheckSafe((int)mi.PCK_TL, SEQ.ASY.CheckSafe);
            PM.SetCheckSafe((int)mi.PCK_TR, SEQ.ASY.CheckSafe);





            OM.LoadLastInfo();
            PM.Load(OM.GetCrntDev().ToString());

            PM.UpdatePstn(true);
            UpdateDevInfo(true);
            UpdateDevOptn(true);

            FraMotr     = new FraMotr    [SM.MT._iMaxMotr    ];
            FraCylinder = new FraCylOneBt[SM.CL._iMaxCylinder];
            FraOutput   = new FraOutput  [SM.IO._iMaxOut     ];

            //모터 축 수에 맞춰 FrameMotr 생성

            for (int m = 0; m < (int)mi.MAX_MOTR ; m++)
            {
                Control[] Ctrl = tcDeviceSet.Controls.Find("pnMotrJog" +  m.ToString(), true);

                FraMotr[m] = new FraMotr();
                FraMotr[m].SetIdType((mi)m, SM.MT.GetDirType(m));
                FraMotr[m].TopLevel = false;
                FraMotr[m].Parent   = Ctrl[0];
                FraMotr[m].Show();
                FraMotr[m].SetUnit(EN_UNIT_TYPE.utJog, 0);
                FraMotr[m].SetCheckSafeJog(SEQ.ASY.CheckSafeJog); //public delegate bool FCheckSafeJog(int _iAxis, bool _bPos);
               
            }

            //실린더 수에 맞춰 FrameCylinder 생성
            /*
            for (int i = 0; i < (int)EN_ACTR_ID.MAX_ACTR; i++)
            {
                Control[] Ctrl          = tcDeviceSet.Controls.Find("pnAtcr" + i.ToString(), true);

                FraCylinder[i]          = new FraCylOneBt();
                FraCylinder[i].TopLevel = false;

                switch (i)
                {
                    default                               : break;
                    case (int)EN_ACTR_ID.aiLDR_Actr : FraCylinder[i].SetConfig((EN_ACTR_ID)i, SM.CL.GetName(i).ToString(), SM.CL.GetDirType(i), Ctrl[0]); break;
                    //case (int)EN_ACTR_ID.aiULD_Actr : FraCylinder[i].SetConfig((EN_ACTR_ID)i, SM.CL.GetName(i).ToString(), SM.CL.GetDirType(i), Ctrl[0]); break;
                }
                FraCylinder[i].Show();
            }
            */

            //Output 버튼 생성
            for (int i = 0; i < (int)yi.MAX_OUTPUT; i++)
            {
                FraOutput[i] = new FraOutput();
                FraOutput[i].TopLevel = false;
            
                switch (i)
                {
                    default                       :                                                                                       break;
                    case (int)yi.PCK_VacLtOn      : FraOutput[i].SetConfig(yi.PCK_VacLtOn  , SM.IO.GetYName((int)yi.PCK_VacLtOn), pnIO0); break;
                    case (int)yi.PCK_VacRtOn      : FraOutput[i].SetConfig(yi.PCK_VacRtOn  , SM.IO.GetYName((int)yi.PCK_VacRtOn), pnIO1); break;
                    case (int)yi.PCK_EjtLtOn      : FraOutput[i].SetConfig(yi.PCK_EjtLtOn  , SM.IO.GetYName((int)yi.PCK_EjtLtOn), pnIO2); break;
                    case (int)yi.PCK_EjtRtOn      : FraOutput[i].SetConfig(yi.PCK_EjtRtOn  , SM.IO.GetYName((int)yi.PCK_EjtRtOn), pnIO3); break;
                }
                
                FraOutput[i].Show();
            }
        }

        public void PstnDisp()
        {
            //PCK_X Property        
            PM.SetProp((uint)mi.PCK_X, (uint)pv.PCK_XWait           , "Wait          ", false, false, false);            
            PM.SetProp((uint)mi.PCK_X, (uint)pv.PCK_XVisnLensStt    , "VisnLensStt   ", false, false, false);            
            PM.SetProp((uint)mi.PCK_X, (uint)pv.PCK_XVisnRearStt    , "VisnRearStt   ", false, false, false);            
            PM.SetProp((uint)mi.PCK_X, (uint)pv.PCK_XVisnFrntStt    , "VisnFrntStt   ", false, false, false);            
            PM.SetProp((uint)mi.PCK_X, (uint)pv.PCK_XAlgn           , "Align         ", false, false, false);            
            PM.SetProp((uint)mi.PCK_X, (uint)pv.PCK_XVisnPck1Ofs    , "VisnPck1Ofs   ", true , false, false);            
            PM.SetProp((uint)mi.PCK_X, (uint)pv.PCK_XVisnPck2Ofs    , "VisnPck2Ofs   ", true , false, false); 
                                                                                     
            //STG_Y Property                                                         
            PM.SetProp((uint)mi.STG_Y, (uint)pv.STG_YWait           , "Wait          ", false, false, false);                  
            PM.SetProp((uint)mi.STG_Y, (uint)pv.STG_YVisnLensStt    , "VisnLensStt   ", false, false, false);                  
            PM.SetProp((uint)mi.STG_Y, (uint)pv.STG_YVisnRearStt    , "VisnRearStt   ", false, false, false);                  
            PM.SetProp((uint)mi.STG_Y, (uint)pv.STG_YVisnFrntStt    , "VisnFrntStt   ", false, false, false);                  
            PM.SetProp((uint)mi.STG_Y, (uint)pv.STG_YAlgn           , "Align         ", false, false, false);
            PM.SetProp((uint)mi.STG_Y, (uint)pv.STG_YChange         , "Lens Change   ", false, false, false);
            PM.SetProp((uint)mi.STG_Y, (uint)pv.STG_YVisnPck1Ofs    , "VisnPck1Ofs   ", true , false, false);                  
            PM.SetProp((uint)mi.STG_Y, (uint)pv.STG_YVisnPck2Ofs    , "VisnPck2Ofs   ", true , false, false); 
                                                                                                                   
            //PCK_Z Left Property                                                                                  
            PM.SetProp((uint)mi.PCK_ZL, (uint)pv.PCK_ZLWait         , "Wait          ", false, false, false);             
            PM.SetProp((uint)mi.PCK_ZL, (uint)pv.PCK_ZLPick         , "Pick          ", false, false, false);             
            PM.SetProp((uint)mi.PCK_ZL, (uint)pv.PCK_ZLPlce         , "Plce          ", false, false, false);               
            PM.SetProp((uint)mi.PCK_ZL, (uint)pv.PCK_ZLAlgnPick     , "AlignPick     ", false, false, false);             
            PM.SetProp((uint)mi.PCK_ZL, (uint)pv.PCK_ZLAlgnPlce     , "AlignPlace    ", false, false, false);
            PM.SetProp((uint)mi.PCK_ZL, (uint)pv.PCK_ZLUnlock       , "Unlock        ", false, false, false);
                                                                                                                        
            //PCK_Z Right Property                                                                                      
            PM.SetProp((uint)mi.PCK_ZR, (uint)pv.PCK_ZRWait         , "Wait          ", false, false, false);          
            PM.SetProp((uint)mi.PCK_ZR, (uint)pv.PCK_ZRPick         , "Pick          ", false, false, false);              
            PM.SetProp((uint)mi.PCK_ZR, (uint)pv.PCK_ZRPlce         , "Plce          ", false, false, false);                               
            PM.SetProp((uint)mi.PCK_ZR, (uint)pv.PCK_ZRAlgnPick     , "AlignPick     ", false, false, false);                
            PM.SetProp((uint)mi.PCK_ZR, (uint)pv.PCK_ZRAlgnPlce     , "AlignPlace    ", false, false, false);
            PM.SetProp((uint)mi.PCK_ZR, (uint)pv.PCK_ZRUnlock       , "Unlock        ", false, false, false);
                                                                                                                     
            //PCK_T Left Property                                                                                   
            PM.SetProp((uint)mi.PCK_TL, (uint)pv.PCK_TLWait         , "Wait          ", false, false, false);            
            PM.SetProp((uint)mi.PCK_TL, (uint)pv.PCK_TLVisnZero     , "VisnZero      ", false, false, false);
            PM.SetProp((uint)mi.PCK_TL, (uint)pv.PCK_TLRvrsWork     , "ReverseWork   ", true , false, false); 
            PM.SetProp((uint)mi.PCK_TL, (uint)pv.PCK_TLWorkOfs      , "WorkOfs       ", true , false, false); 
            PM.SetProp((uint)mi.PCK_TL, (uint)pv.PCK_TLUnlockWork   , "UnlockWork    ", true , false, false);
            PM.SetProp((uint)mi.PCK_TL, (uint)pv.PCK_TLHolderPutOfs , "HolderPutOfs  ", true , false, false); //틈새에 끼우는 디바이스용. 보통 체결방식은 그냥 0으로 놓으면됌.
                                                                                                                  
            //PCK_T Right Property                                                                                         
            PM.SetProp((uint)mi.PCK_TR, (uint)pv.PCK_TRWait         , "Wait          ", false, false, false);            
            PM.SetProp((uint)mi.PCK_TR, (uint)pv.PCK_TRVisnZero     , "VisnZero      ", false, false, false);
            PM.SetProp((uint)mi.PCK_TR, (uint)pv.PCK_TRRvrsWork     , "ReverseWork   ", true , false, false); 
            PM.SetProp((uint)mi.PCK_TR, (uint)pv.PCK_TRWorkOfs      , "WorkOfs       ", true , false, false);
            PM.SetProp((uint)mi.PCK_TR, (uint)pv.PCK_TRUnlockWork   , "UnlockWork    ", true , false, false);
            PM.SetProp((uint)mi.PCK_TR, (uint)pv.PCK_TRHolderPutOfs , "HolderPutOfs  ", true , false, false); //틈새에 끼우는 디바이스용. 보통 체결방식은 그냥 0으로 놓으면됌.
        }

        private void tcDeviceSet_SelectedIndexChanged(object sender, EventArgs e)                                      
        {                                                                                                               
            int iSeletedIndex;                                                                                          
            iSeletedIndex = tcDeviceSet.SelectedIndex;                                                                  
            
            switch (iSeletedIndex)
            {
                default : break;
                case 1  : gbJogUnit  .Parent = pnJog1 ; 
                          tabControl1.Parent = pnMotr1; break;
                case 2  : gbJogUnit  .Parent = pnJog2 ;
                          tabControl1.Parent = pnMotr2; break;
            }

            //EmbededExe.SetCamParent(pnVisn.Handle);

            UpdateDevInfo(true);
            UpdateDevOptn(true);
            PM.UpdatePstn(true);
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            Log.Trace("SAVE", "Clicked");

            if (Log.ShowMessageModal("Confirm", "Are you Sure?") != DialogResult.Yes) return;

            

            UpdateDevInfo(false);
            UpdateDevOptn(false);

            //if (OM.CmnOptn.bTorqChck && OM.DevOptn.iFailRptCnt > 1)
            //{
            //    Log.ShowMessage("Warning", "Torque Check Option is On.");
            //    OM.DevOptn.iFailRptCnt = 0;
            //    UpdateDevOptn(true);
            //}

            OM.SaveDevInfo(OM.GetCrntDev().ToString());
            OM.SaveDevOptn(OM.GetCrntDev().ToString());

            PM.UpdatePstn(false);
            PM.Save(OM.GetCrntDev());

            DM.ARAY[(int)ri.REAR ].SetMaxColRow(OM.DevInfo.iRearColCnt , OM.DevInfo.iRearRowCnt );
            DM.ARAY[(int)ri.FRNT ].SetMaxColRow(OM.DevInfo.iFrntColCnt , OM.DevInfo.iFrntRowCnt );
            DM.ARAY[(int)ri.LENS].SetMaxColRow(OM.DevInfo.iLensColCnt, OM.DevInfo.iLensRowCnt);

            pbHousingR.Refresh();
            pbHousingF.Refresh();
            pbLens.Refresh();
            OM.SaveEqpOptn();

        }

        public void UpdateDevInfo(bool bToTable)
        {
            if (bToTable)
            {
                //Rear Housing Stage
                tbRearColCnt   .Text = OM.DevInfo.iRearColCnt   .ToString();
                tbRearRowCnt   .Text = OM.DevInfo.iRearRowCnt   .ToString();
                tbRearColPitch .Text = OM.DevInfo.dRearColPitch .ToString();
                tbRearRowPitch .Text = OM.DevInfo.dRearRowPitch .ToString();
                        
                //Front Housing Stage                    
                tbFrntColCnt   .Text = OM.DevInfo.iFrntColCnt   .ToString();
                tbFrntRowCnt   .Text = OM.DevInfo.iFrntRowCnt   .ToString();
                tbFrntColPitch .Text = OM.DevInfo.dFrntColPitch .ToString();
                tbFrntRowPitch .Text = OM.DevInfo.dFrntRowPitch .ToString();
                             
                //Lens Stage
                tbLensColCnt  .Text = OM.DevInfo.iLensColCnt  .ToString();
                tbLensRowCnt  .Text = OM.DevInfo.iLensRowCnt  .ToString();
                tbLensColPitch.Text = OM.DevInfo.dLensColPitch.ToString();
                tbLensRowPitch.Text = OM.DevInfo.dLensRowPitch.ToString();


            }
            else 
            {
                if (CConfig.StrToIntDef(tbRearColCnt .Text, 1) <= 0) { tbRearColCnt .Text = 1.ToString(); }
                if (CConfig.StrToIntDef(tbRearRowCnt .Text, 1) <= 0) { tbRearRowCnt .Text = 1.ToString(); }
                if (CConfig.StrToIntDef(tbFrntColCnt .Text, 1) <= 0) { tbFrntColCnt .Text = 1.ToString(); }
                if (CConfig.StrToIntDef(tbFrntRowCnt .Text, 1) <= 0) { tbFrntRowCnt .Text = 1.ToString(); }
                if (CConfig.StrToIntDef(tbLensColCnt .Text, 1) <= 0) { tbLensColCnt .Text = 1.ToString(); }
                if (CConfig.StrToIntDef(tbLensRowCnt .Text, 1) <= 0) { tbLensRowCnt .Text = 1.ToString(); }

                //Rear Housing Stage
                OM.DevInfo.iRearColCnt    =  CConfig.StrToIntDef     (tbRearColCnt   . Text, 1  ) ;
                OM.DevInfo.iRearRowCnt    =  CConfig.StrToIntDef     (tbRearRowCnt   . Text, 1  ) ;
                OM.DevInfo.dRearColPitch  =  CConfig.StrToDoubleDef  (tbRearColPitch . Text, 1  ) ;
                OM.DevInfo.dRearRowPitch  =  CConfig.StrToDoubleDef  (tbRearRowPitch . Text, 1  ) ;
                                                                              
                //Front Housing Stage                                         
                OM.DevInfo.iFrntColCnt    =  CConfig.StrToIntDef     (tbFrntColCnt   . Text, 1  ) ;
                OM.DevInfo.iFrntRowCnt    =  CConfig.StrToIntDef     (tbFrntRowCnt   . Text, 1  ) ;
                OM.DevInfo.dFrntColPitch  =  CConfig.StrToDoubleDef  (tbFrntColPitch . Text, 1  ) ;
                OM.DevInfo.dFrntRowPitch  =  CConfig.StrToDoubleDef  (tbFrntRowPitch . Text, 1  ) ;

                //Lens Stage
                OM.DevInfo.iLensColCnt   =  CConfig.StrToIntDef     (tbLensColCnt  . Text, 1  ) ;
                OM.DevInfo.iLensRowCnt   =  CConfig.StrToIntDef     (tbLensRowCnt  . Text, 1  ) ;
                OM.DevInfo.dLensColPitch =  CConfig.StrToDoubleDef  (tbLensColPitch. Text, 1  ) ;
                OM.DevInfo.dLensRowPitch =  CConfig.StrToDoubleDef  (tbLensRowPitch. Text, 1  ) ;

                UpdateDevInfo(true);
            }
        
        }

        public void UpdateDevOptn(bool bToTable)
        {

            if (bToTable)
            {
                tbPCKGapCnt      .Text    = OM.DevOptn.iPCKGapCnt     .ToString();
                //cbUseMultiHldr   .Checked = OM.DevOptn.bUseMultiHldr             ;
                tbHldrPitch      .Text    = OM.DevOptn.dHldrPitch     .ToString();
                tbThetaWorkSpeed .Text    = OM.DevOptn.dThetaWorkSpeed.ToString();
                tbThetaWorkAcc   .Text    = OM.DevOptn.dThetaWorkAcc  .ToString();
                tbThetaBackPos   .Text    = OM.DevOptn.dThetaBackPos  .ToString();
                tbPlceDelay      .Text    = OM.DevOptn.iPlceDelay     .ToString();
                tbRptCnt         .Text    = OM.DevOptn.iWrkRptCnt     .ToString();

                //비젼 옵션
                tbLensVisnXTol       .Text    = OM.DevOptn.dLensVisnXTol      .ToString();
                tbLensVisnYTol       .Text    = OM.DevOptn.dLensVisnYTol      .ToString();
                tbLensVisnXTol       .Text    = OM.DevOptn.dLensVisnXTol      .ToString();
                tbLensVisnYTol       .Text    = OM.DevOptn.dLensVisnYTol      .ToString();
                tbHldrVisnXTol       .Text    = OM.DevOptn.dHldrVisnXTol      .ToString();
                tbHldrVisnYTol       .Text    = OM.DevOptn.dHldrVisnYTol      .ToString();
                tbAtVisnTTol         .Text    = OM.DevOptn.dAtVisnTTol        .ToString();

                //토크 옵션
                tbMaxTorque          .Text    = OM.DevOptn.dTorqueMax  .ToString();
                tbTorqueLimit        .Text    = OM.DevOptn.dTorqueLimit.ToString();
                tbTorqueTime         .Text    = OM.DevOptn.dTorqueTime .ToString();

                                            
                //Ver1.0.1.0
                cbWorkMode.SelectedIndex = OM.DevOptn.iWorkMode;
                

            }
            else
            {
                OM.DevOptn.iPCKGapCnt      = CConfig.StrToIntDef   (tbPCKGapCnt.Text, 0)       ;
                //OM.DevOptn.bUseMultiHldr   = cbUseMultiHldr.Checked ;
                OM.DevOptn.dHldrPitch      = CConfig.StrToDoubleDef(tbHldrPitch     .Text, 0.0);
                OM.DevOptn.dThetaWorkSpeed = CConfig.StrToDoubleDef(tbThetaWorkSpeed.Text, 0.0);
                OM.DevOptn.dThetaWorkAcc   = CConfig.StrToDoubleDef(tbThetaWorkAcc  .Text, 0.0);
                OM.DevOptn.dThetaBackPos   = CConfig.StrToDoubleDef(tbThetaBackPos  .Text, 0.0);
                OM.DevOptn.iPlceDelay      = CConfig.StrToIntDef   (tbPlceDelay     .Text, 0  );
                OM.DevOptn.iWrkRptCnt      = CConfig.StrToIntDef   (tbRptCnt        .Text, 0  );

                OM.DevOptn.dLensVisnYTol       = CConfig.StrToDoubleDef(tbLensVisnYTol      .Text, 0.0);
                OM.DevOptn.dLensVisnXTol       = CConfig.StrToDoubleDef(tbLensVisnXTol      .Text, 0.0);
                OM.DevOptn.dHldrVisnXTol       = CConfig.StrToDoubleDef(tbHldrVisnXTol      .Text, 0.0);
                OM.DevOptn.dHldrVisnYTol       = CConfig.StrToDoubleDef(tbHldrVisnYTol      .Text, 0.0);
                OM.DevOptn.dAtVisnTTol         = CConfig.StrToDoubleDef(tbAtVisnTTol        .Text, 0.0);

                //토크 옵션
                OM.DevOptn.dTorqueMax   = CConfig.StrToDoubleDef(tbMaxTorque  .Text, 0.0);
                OM.DevOptn.dTorqueLimit = CConfig.StrToDoubleDef(tbTorqueLimit.Text, 0.0);
                OM.DevOptn.dTorqueTime  = CConfig.StrToDoubleDef(tbTorqueTime .Text, 0.0);

                //Ver1.0.1.0
                //2호기는 포지션으로 체결해야해서 Z축 센서 or 포지션 체결 옵션 처리
                OM.DevOptn.iWorkMode = cbWorkMode.SelectedIndex ;

                UpdateDevOptn(true);
            }
        }

        

        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            //tm tmUpdate.Enabled = false;
            btSaveDevice.Enabled = !SEQ._bRun;

            double RsltMaxTorq   = 0.0;
            double RsltLimitTorq = 0.0;
            double RsltRatio = 0.0;
            double SetMotrTorq1 = OM.CmnOptn.dSetMotrTorq1 ;
            double SetMotrTorq2 = OM.CmnOptn.dSetMotrTorq2 ;
            double GaugeTorq1   = OM.CmnOptn.dGaugeTorq1   ;
            double GaugeTorq2   = OM.CmnOptn.dGaugeTorq2   ;
            if (SetMotrTorq1 > GaugeTorq1)
            {
                if (GaugeTorq1 != 0 && GaugeTorq2 != 0)
                {
                    RsltRatio = ((SetMotrTorq1 / GaugeTorq1) + (SetMotrTorq2 / GaugeTorq2)) / 2;
                }

                RsltMaxTorq   = OM.DevOptn.dTorqueMax   / RsltRatio;
                RsltLimitTorq = OM.DevOptn.dTorqueLimit / RsltRatio;
            }

            else if (SetMotrTorq1 < GaugeTorq1)
            {
                if (SetMotrTorq1 != 0 && SetMotrTorq2 != 0)
                {
                    RsltRatio = ((GaugeTorq1 / SetMotrTorq1) + (GaugeTorq2 / SetMotrTorq2)) / 2;
                }

                RsltMaxTorq   = OM.DevOptn.dTorqueMax   * RsltRatio;
                RsltLimitTorq = OM.DevOptn.dTorqueLimit * RsltRatio;
            } 

            tbGaugeMax.Text  = RsltMaxTorq  .ToString();
            tbGaugeTorq.Text = RsltLimitTorq.ToString();

            

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

            //tm tmUpdate.Enabled = true;
        }


        private void pbSTG_Paint(object sender, PaintEventArgs e)
        {
            //pbSTG.Padding = new Control(10, 10, 10, 10);
            int iTag = Convert.ToInt32(((PictureBox)sender).Tag);

            SolidBrush Brush = new SolidBrush(Color.Black);

            Pen Pen = new Pen(Color.Black);

            //Graphics gSTG = pbHousingR.CreateGraphics();
            Graphics g    = e.Graphics;

            double dX1, dX2, dY1, dY2;

            int iRearColCnt, iRearRowCnt, iFrntColCnt, iFrntRowCnt, iLensColCnt, iLensRowCnt;

            switch (iTag)
            {
                default:                              break;
                case  1: iRearColCnt = OM.DevInfo.iRearColCnt;
                         iRearRowCnt = OM.DevInfo.iRearRowCnt;

                         int iGetRWidth  = pbHousingR.Width;
                         int iGetRHeight = pbHousingR.Height;
                         
                         double iSetRWidth = 0, iSetRHeight = 0;
                         
                         double uGRw   = (double) iGetRWidth  / (double)(iRearColCnt);
                         double uGRh   = (double) iGetRHeight / (double)(iRearRowCnt);
                         double dWROff = (double)(iGetRWidth  - uGRw * (iRearColCnt)) / 2.0;
                         double dHROff = (double)(iGetRHeight - uGRh * (iRearRowCnt)) / 2.0;
                         
                         Pen.Color = Color.Black;
                         Brush.Color = Color.HotPink;

                         for (int r = 0; r < iRearRowCnt; r++)
                         {
                             for (int c = 0; c < iRearColCnt; c++)
                             {

                                 dY1 = dHROff + r * uGRh - 1;
                                 dY2 = dHROff + r * uGRh + uGRh;
                                 dX1 = dWROff + c * uGRw - 1;
                                 dX2 = dWROff + c * uGRw + uGRw;

                                 g.FillRectangle(Brush, (float)dX1, (float)dY1, (float)dX2, (float)dY2);
                                 g.DrawRectangle(Pen, (float)dX1, (float)dY1, (float)dX2, (float)dY2);

                                 iSetRWidth += dY2;
                                 iSetRHeight += dX2;
                             }

                         }
                         
                         break;
                
                case  2: iFrntColCnt = OM.DevInfo.iFrntColCnt;
                         iFrntRowCnt = OM.DevInfo.iFrntRowCnt;

                         int iGetFWidth = pbHousingF.Width;
                         int iGetFHeight = pbHousingF.Height;
                         
                         double iSetFWidth = 0, iSetFHeight = 0;
                         
                         double uGFw   = (double) iGetFWidth  / (double)(iFrntColCnt);
                         double uGFh   = (double) iGetFHeight / (double)(iFrntRowCnt);
                         double dWFOff = (double)(iGetFWidth  - uGFw * (iFrntColCnt)) / 2.0;
                         double dHFOff = (double)(iGetFHeight - uGFh * (iFrntRowCnt)) / 2.0;
                         
                         Pen.Color = Color.Black;
                         Brush.Color = Color.HotPink;

                         for (int r = 0; r < iFrntRowCnt; r++)
                         {
                             for (int c = 0; c < iFrntColCnt; c++)
                             {

                                 dY1 = dHFOff + r * uGFh - 1;
                                 dY2 = dHFOff + r * uGFh + uGFh;
                                 dX1 = dWFOff + c * uGFw - 1;
                                 dX2 = dWFOff + c * uGFw + uGFw;

                                 g.FillRectangle(Brush, (float)dX1, (float)dY1, (float)dX2, (float)dY2);
                                 g.DrawRectangle(Pen, (float)dX1, (float)dY1, (float)dX2, (float)dY2);

                                 iSetFWidth += dY2;
                                 iSetFHeight += dX2;
                             }

                         }
                         
                         break;
                
                case  3: iLensColCnt = OM.DevInfo.iLensColCnt;
                         iLensRowCnt = OM.DevInfo.iLensRowCnt;

                         int iGetLWidth = pbLens.Width;
                         int iGetLHeight = pbLens.Height;
                         
                         double iSetLWidth = 0, iSetLHeight = 0;
                         
                         double uGLw   = (double) iGetLWidth  / (double)(iLensColCnt);
                         double uGLh   = (double) iGetLHeight / (double)(iLensRowCnt);
                         double dWLOff = (double)(iGetLWidth  - uGLw * (iLensColCnt)) / 2.0;
                         double dHLOff = (double)(iGetLHeight - uGLh * (iLensRowCnt)) / 2.0;
                         
                         Pen.Color = Color.Black;
                         Brush.Color = Color.HotPink;

                         for (int r = 0; r < iLensRowCnt; r++)
                         {
                             for (int c = 0; c < iLensColCnt; c++)
                             {

                                 dY1 = dHLOff + r * uGLh - 1;
                                 dY2 = dHLOff + r * uGLh + uGLh;
                                 dX1 = dWLOff + c * uGLw - 1;
                                 dX2 = dWLOff + c * uGLw + uGLw;

                                 g.FillRectangle(Brush, (float)dX1, (float)dY1, (float)dX2, (float)dY2);
                                 g.DrawRectangle(Pen, (float)dX1, (float)dY1, (float)dX2, (float)dY2);

                                 iSetLWidth += dY2;
                                 iSetLHeight += dX2;
                             }

                         }
                         
                         break;
            }

            Brush.Dispose();
            Pen.Dispose();
            //g.Dispose();
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

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void FormDeviceSet_VisibleChanged(object sender, EventArgs e)
        {
            EmbededExe.SetCamParent(pnVisn.Handle);
            tmUpdate.Enabled = Visible;
        }
    }
}
