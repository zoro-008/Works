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

            int i = 0 ;
            i = this.Left ;
            i = this.Top ;
            i = this.Width ;
            i = this.Height ;

            this.TopLevel = false;
            this.Parent = _pnBase;


            PstnDisp();
            PM.Load(OM.GetCrntDev());


            this.Left = 0 ;
            this.Top = 0 ;
            
            //매뉴얼 버튼 라운딩 처리
            //로더
            metroTile4 .Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, metroTile4 .Width, metroTile4 .Height, 5, 5));
            metroTile5 .Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, metroTile5 .Width, metroTile5 .Height, 5, 5));
            metroTile23.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, metroTile23.Width, metroTile23.Height, 5, 5));
            metroTile24.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, metroTile24.Width, metroTile24.Height, 5, 5));
            metroTile25.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, metroTile25.Width, metroTile25.Height, 5, 5));
            //metroTile26.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, metroTile26.Width, metroTile26.Height, 5, 5));
            //로더픽커
            metroTile30.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, metroTile30.Width, metroTile30.Height, 5, 5));
            metroTile29.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, metroTile29.Width, metroTile29.Height, 5, 5));
            metroTile28.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, metroTile28.Width, metroTile28.Height, 5, 5));
            metroTile27.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, metroTile27.Width, metroTile27.Height, 5, 5));
            metroTile9 .Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, metroTile9 .Width, metroTile9 .Height, 5, 5));
            metroTile8 .Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, metroTile8 .Width, metroTile8 .Height, 5, 5));
            metroTile42.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, metroTile42.Width, metroTile42.Height, 5, 5));
            //AUTO QC
            metroTile43.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, metroTile43.Width, metroTile43.Height, 5, 5));
            metroTile34.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, metroTile34.Width, metroTile34.Height, 5, 5));
            metroTile33.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, metroTile33.Width, metroTile33.Height, 5, 5));
            metroTile32.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, metroTile32.Width, metroTile32.Height, 5, 5));
            metroTile31.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, metroTile31.Width, metroTile31.Height, 5, 5));
            metroTile12.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, metroTile12.Width, metroTile12.Height, 5, 5));
            metroTile11.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, metroTile11.Width, metroTile11.Height, 5, 5));
            metroTile44.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, metroTile44.Width, metroTile44.Height, 5, 5));
            metroTile45.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, metroTile45.Width, metroTile45.Height, 5, 5));
            metroTile46.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, metroTile46.Width, metroTile46.Height, 5, 5));
            //실린지
            metroTile38.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, metroTile38.Width, metroTile38.Height, 5, 5));
            metroTile37.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, metroTile37.Width, metroTile37.Height, 5, 5));
            metroTile36.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, metroTile36.Width, metroTile36.Height, 5, 5));
            metroTile35.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, metroTile35.Width, metroTile35.Height, 5, 5));
            //챔버
            metroTile41.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, metroTile41.Width, metroTile41.Height, 5, 5));
            metroTile40.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, metroTile40.Width, metroTile40.Height, 5, 5));
            metroTile39.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, metroTile39.Width, metroTile39.Height, 5, 5));
            metroTile22.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, metroTile22.Width, metroTile22.Height, 5, 5));
            metroTile20.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, metroTile20.Width, metroTile20.Height, 5, 5));


        }

        private void FormDeviceSet_Load(object sender, EventArgs e)
        {
            tbUserUnit.Text = 0.01.ToString();

            

            PM.UpdatePstn(true);
            UpdateDevInfo(true);

            



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
            
            FraCylAPT = new FrameCylinderAPT[(int)ci.MAX_ACTR];
            for (int i = 0; i < (int)ci.MAX_ACTR; i++)
            {
                Control[] CtrlAP = tcPosition.Controls.Find("C" + i.ToString(), true);

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
                Control[] Ctrl = tcPosition.Controls.Find("pnMotrPos" + i.ToString(), true);
            
                FraMotrPosAPT[i] = new FrameMotrPosAPT();
                FraMotrPosAPT[i].TopLevel = false;
                FraMotrPosAPT[i].SetWindow(i, Ctrl[0]);
                FraMotrPosAPT[i].Show();
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
            //const int iOutputBtnCnt = 8;
            //FraOutputAPT = new FrameOutputAPT[iOutputBtnCnt];
            //for (int i = 0; i < iOutputBtnCnt; i++)
            //{
            //    Control[] Ctrl = tcDeviceSet.Controls.Find("Y" + i.ToString(), true);
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

            tcMain.TabPages.Remove(tpDeviceInfo);
        }

        public void PstnDisp()
        {

            PM.SetProp((uint)mi.LDR_XBelt   , (uint)pv.LDR_XBeltPitch        , "Pitch       ", true , false, true  );
                                                                                            
            PM.SetProp((uint)mi.PKR_YSutl   , (uint)pv.PKR_YSutlSyinge       , "Syinge      ", false, false, true  );
            PM.SetProp((uint)mi.PKR_YSutl   , (uint)pv.PKR_YSutlBuffer       , "Buffer      ", false, false, true  );
            PM.SetProp((uint)mi.PKR_YSutl   , (uint)pv.PKR_YSutlPicker       , "Picker      ", false, false, true  );
                                                                                            
            PM.SetProp((uint)mi.PKR_ZPckr   , (uint)pv.PKR_ZPckrShake        , "Shake       ", false, false, true  );
            PM.SetProp((uint)mi.PKR_ZPckr   , (uint)pv.PKR_ZPckrRackPlce     , "RackPlace   ", false, false, true  );
            PM.SetProp((uint)mi.PKR_ZPckr   , (uint)pv.PKR_ZPckrRackPick     , "RackPick    ", false, false, true  );
            PM.SetProp((uint)mi.PKR_ZPckr   , (uint)pv.PKR_ZPckrSuttlePlce   , "SuttlePlace ", false, false, true  );
            PM.SetProp((uint)mi.PKR_ZPckr   , (uint)pv.PKR_ZPckrSuttlePick   , "SuttlePick  ", false, false, true  );

            PM.SetProp((uint)mi.LDR_YBelt   , (uint)pv.LDR_YBeltWorkStart    , "WorkStart   ", false, false, true  );
            PM.SetProp((uint)mi.LDR_YBelt   , (uint)pv.LDR_YBeltWorkEnd      , "WorkEnd     ", false, false, true  );

            PM.SetProp((uint)mi.SYR_XSyrg   , (uint)pv.SYR_XSyrgSuttle       , "Suttle      ", false, false, true  );
            PM.SetProp((uint)mi.SYR_XSyrg   , (uint)pv.SYR_XSyrgClean        , "Clean       ", false, false, true  );
            PM.SetProp((uint)mi.SYR_XSyrg   , (uint)pv.SYR_XSyrgChamberStt   , "ChamberStt  ", false, false, true  );
            PM.SetProp((uint)mi.SYR_XSyrg   , (uint)pv.SYR_XSyrgChamberPitch , "ChamberPitch", true , false, true  );
                                                                                            
            PM.SetProp((uint)mi.SYR_ZSyrg   , (uint)pv.SYR_ZSyrgMove         , "Move        ", false, false, true  );
            PM.SetProp((uint)mi.SYR_ZSyrg   , (uint)pv.SYR_ZSyrgSuttleLid    , "SuttleLid   ", false, false, true  );
            PM.SetProp((uint)mi.SYR_ZSyrg   , (uint)pv.SYR_ZSyrgSuttle       , "Suttle      ", false, false, true  );
            PM.SetProp((uint)mi.SYR_ZSyrg   , (uint)pv.SYR_ZSyrgChamber      , "Chamber     ", false, false, true  );

            
            
            PM.SetCheckSafe((uint)mi.SYR_XSyrg ,SEQ.SYR.CheckSafe);  //sun]
            PM.SetCheckSafe((uint)mi.PKR_YSutl ,SEQ.PKR.CheckSafe);
            //조그는 프레임에 있음.

            //PM.SetCheckSafe((uint)mi.PKR_YSutl ,)

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
                CConfig.ValToCon(tbShakeCnt             , ref OM.DevInfo.iShakeCnt         );

                CConfig.ValToCon(tbBloodChamberSpd      , ref OM.DevInfo.iBloodChamberSpd  );
                CConfig.ValToCon(tbDeadVolSpd           , ref OM.DevInfo.iDeadVolSpd       );

                CConfig.ValToCon(tbBloodReadyCnt        , ref OM.DevInfo.iBloodReadyCnt    );
                CConfig.ValToCon(tbDCReadyCnt           , ref OM.DevInfo.iDCReadyCnt       );
                CConfig.ValToCon(tbFCMReadyCnt          , ref OM.DevInfo.iFCMReadyCnt      );
                CConfig.ValToCon(tbNRReadyCnt           , ref OM.DevInfo.iNRReadyCnt       );
                CConfig.ValToCon(tbRETReadyCnt          , ref OM.DevInfo.iRETReadyCnt      );
                CConfig.ValToCon(tb4DLReadyCnt          , ref OM.DevInfo.i4DLReadyCnt      );
                                                     
                CConfig.ValToCon(tbCmbEmptyTime         , ref OM.DevInfo.iCmbEmptyTime     ); 
                CConfig.ValToCon(tbWasteToExtTime       , ref OM.DevInfo.iWasteToExtTime   ); 
                CConfig.ValToCon(tbFcmWastToWastTime    , ref OM.DevInfo.iFcmWastToWastTime); 
                CConfig.ValToCon(tbHgbToWastTime        , ref OM.DevInfo.iHgbToWastTime    ); 
                CConfig.ValToCon(tbBloodPreCutVol       , ref OM.DevInfo.iBloodPreCutVol   ); 

                CConfig.ValToCon(tbFCMTestStartDelay    , ref OM.DevInfo.iFCMTestStartDelay); 
                CConfig.ValToCon(tbFCMTestEndDelay      , ref OM.DevInfo.iFCMTestEndDelay  ); 
                CConfig.ValToCon(tbFCMTestDelayTime     , ref OM.DevInfo.iFCMTestDelayTime ); 











                CConfig.ValToCon(tbCmb1BloodVol         , ref OM.DevInfo.iCmb1BloodVol     );
                CConfig.ValToCon(tbCmb1Cp2Time          , ref OM.DevInfo.iCmb1Cp2Time      );
              //CConfig.ValToCon(tbCmb1TankTime         , ref OM.DevInfo.iCmb1TankTime     );
              //CConfig.ValToCon(tbCmb1SylnPos          , ref OM.DevInfo.iCmb1SylnPos      );
              //CConfig.ValToCon(tbCmb1SylSpdCode       , ref OM.DevInfo.iCmb1SylSpdCode   );
                CConfig.ValToCon(tbCmb1Cp3Time          , ref OM.DevInfo.iCmb1Cp3Time      );                
                CConfig.ValToCon(tbCmb1DCSylPos         , ref OM.DevInfo.iCmb1DCSylPos     );
                CConfig.ValToCon(tbCmb1DCSylSpdCode     , ref OM.DevInfo.iCmb1DCSylSpdCode );     
                CConfig.ValToCon(tbCmb1CleanRvsPos      , ref OM.DevInfo.iCmb1CleanRvsPos  );     
                CConfig.ValToCon(tbCmb1DeadVol          , ref OM.DevInfo.iCmb1DeadVol      );    
                CConfig.ValToCon(tbCmb1DeadTimes        , ref OM.DevInfo.iCmb1DeadTimes    );    
                CConfig.ValToCon(tbDccCleanTime         , ref OM.DevInfo.iDccCleanTime     ); 
                CConfig.ValToCon(tbDccInspDelayTime     , ref OM.DevInfo.iDccInspDelayTime ); 
                CConfig.ValToCon(tbCmb1ToInter          , ref OM.DevInfo.iCmb1ToInter      ); 


                CConfig.ValToCon(tbCmb2BloodVol         , ref OM.DevInfo.iCmb2BloodVol     );
                CConfig.ValToCon(tbCmb2Cp2Time          , ref OM.DevInfo.iCmb2Cp2Time      );
                CConfig.ValToCon(tbCmb2TankTime         , ref OM.DevInfo.iCmb2TankTime     );
              //CConfig.ValToCon(tbCmb2SylnPos          , ref OM.DevInfo.iCmb2SylnPos      );
              //CConfig.ValToCon(tbCmb2SylSpdCode       , ref OM.DevInfo.iCmb2SylSpdCode   );
                CConfig.ValToCon(tbCmb2Cp3Time          , ref OM.DevInfo.iCmb2Cp3Time      );                
              //CConfig.ValToCon(tbCmb2DCSylPos         , ref OM.DevInfo.iCmb2DCSylPos     );
              //CConfig.ValToCon(tbCmb2DCSylSpdCode     , ref OM.DevInfo.iCmb2DCSylSpdCode );    
                CConfig.ValToCon(tbCmb2CleanRvsPos      , ref OM.DevInfo.iCmb2CleanRvsPos  );     
                CConfig.ValToCon(tbCmb2BubbleTime       , ref OM.DevInfo.iCmb2BubbleTime   );     
                CConfig.ValToCon(tbCmb2Blk              , ref OM.DevInfo.iCmb2Blk          ); 
                CConfig.ValToCon(tbCmb2SpecAngle        , ref OM.DevInfo.dCmb2SpecAngle    ); 



                CConfig.ValToCon(tbCmb3BloodVol         , ref OM.DevInfo.iCmb3BloodVol     );
              //CConfig.ValToCon(tbCmb3Cp2Time          , ref OM.DevInfo.iCmb3Cp2Time      );
                CConfig.ValToCon(tbCmb3TankTime         , ref OM.DevInfo.iCmb3TankTime     );
                CConfig.ValToCon(tbCmb3SylnPos          , ref OM.DevInfo.iCmb3SylnPos      );
                CConfig.ValToCon(tbCmb3SylSpdCode       , ref OM.DevInfo.iCmb3SylSpdCode   );
                CConfig.ValToCon(tbCmb3Cp3Time          , ref OM.DevInfo.iCmb3Cp3Time      );                
                CConfig.ValToCon(tbCmb3FCMSylPos        , ref OM.DevInfo.iCmb3FCMSylPos    );
                CConfig.ValToCon(tbCmb3FCMSylSpdCode    , ref OM.DevInfo.iCmb3FCMSylSpdCode);   
                CConfig.ValToCon(tbCmb3CleanRvsPos      , ref OM.DevInfo.iCmb3CleanRvsPos  );   
                CConfig.ValToCon(tbCmb3DeadVol          , ref OM.DevInfo.iCmb3DeadVol      );
                CConfig.ValToCon(tbCmb3DeadTimes        , ref OM.DevInfo.iCmb3DeadTimes    );    
                CConfig.ValToCon(tbCmb3ToInter          , ref OM.DevInfo.iCmb3ToInter      ); 
                

                CConfig.ValToCon(tbCmb4BloodVol         , ref OM.DevInfo.iCmb4BloodVol     );
              //CConfig.ValToCon(tbCmb4Cp2Time          , ref OM.DevInfo.iCmb4Cp2Time      );
                CConfig.ValToCon(tbCmb4TankTime         , ref OM.DevInfo.iCmb4TankTime     );
                CConfig.ValToCon(tbCmb4SylnPos          , ref OM.DevInfo.iCmb4SylnPos      );
                CConfig.ValToCon(tbCmb4SylSpdCode       , ref OM.DevInfo.iCmb4SylSpdCode   );
                CConfig.ValToCon(tbCmb4Cp3Time          , ref OM.DevInfo.iCmb4Cp3Time      );                
                CConfig.ValToCon(tbCmb4FCMSylPos        , ref OM.DevInfo.iCmb4FCMSylPos    );
                CConfig.ValToCon(tbCmb4FCMSylSpdCode    , ref OM.DevInfo.iCmb4FCMSylSpdCode); 
                CConfig.ValToCon(tbCmb4CleanRvsPos      , ref OM.DevInfo.iCmb4CleanRvsPos  );     
                CConfig.ValToCon(tbCmb4DeadVol          , ref OM.DevInfo.iCmb4DeadVol      );
                CConfig.ValToCon(tbCmb4DeadTimes        , ref OM.DevInfo.iCmb4DeadTimes    );    
                CConfig.ValToCon(tbCmb4ToInter          , ref OM.DevInfo.iCmb4ToInter      ); 
                

                CConfig.ValToCon(tbCmb5BloodVol         , ref OM.DevInfo.iCmb5BloodVol     );
              //CConfig.ValToCon(tbCmb5Cp2Time          , ref OM.DevInfo.iCmb5Cp2Time      );
                CConfig.ValToCon(tbCmb5TankTime         , ref OM.DevInfo.iCmb5TankTime     );
                CConfig.ValToCon(tbCmb5SylnPos          , ref OM.DevInfo.iCmb5SylnPos      );
                CConfig.ValToCon(tbCmb5SylSpdCode       , ref OM.DevInfo.iCmb5SylSpdCode   );
                CConfig.ValToCon(tbCmb5Cp3Time          , ref OM.DevInfo.iCmb5Cp3Time      );                
                CConfig.ValToCon(tbCmb5FCMSylPos        , ref OM.DevInfo.iCmb5FCMSylPos    );
                CConfig.ValToCon(tbCmb5FCMSylSpdCode    , ref OM.DevInfo.iCmb5FCMSylSpdCode); 
                CConfig.ValToCon(tbCmb5CleanRvsPos      , ref OM.DevInfo.iCmb5CleanRvsPos  );     
                CConfig.ValToCon(tbCmb5DeadVol          , ref OM.DevInfo.iCmb5DeadVol      );
                CConfig.ValToCon(tbCmb5DeadTimes        , ref OM.DevInfo.iCmb5DeadTimes    );    

                CConfig.ValToCon(tbCmb5ToInter          , ref OM.DevInfo.iCmb5ToInter      ); 
                

                CConfig.ValToCon(tbCmb6BloodVol         , ref OM.DevInfo.iCmb6BloodVol     );//iCmb6ToInter
              //CConfig.ValToCon(tbCmb6Cp2Time          , ref OM.DevInfo.iCmb6Cp2Time      );
                CConfig.ValToCon(tbCmb6TankTime         , ref OM.DevInfo.iCmb6TankTime     );
                CConfig.ValToCon(tbCmb6SylnPos          , ref OM.DevInfo.iCmb6SylnPos      );
                CConfig.ValToCon(tbCmb6SylSpdCode       , ref OM.DevInfo.iCmb6SylSpdCode   );
                CConfig.ValToCon(tbCmb6Cp3Time          , ref OM.DevInfo.iCmb6Cp3Time      );                
                CConfig.ValToCon(tbCmb6FCMSylPos        , ref OM.DevInfo.iCmb6FCMSylPos    );
                CConfig.ValToCon(tbCmb6FCMSylSpdCode    , ref OM.DevInfo.iCmb6FCMSylSpdCode);
                CConfig.ValToCon(tbCmb6CleanRvsPos      , ref OM.DevInfo.iCmb6CleanRvsPos  );     
                CConfig.ValToCon(tbCmb6DeadVol          , ref OM.DevInfo.iCmb6DeadVol      );
                CConfig.ValToCon(tbCmb6DeadTimes        , ref OM.DevInfo.iCmb6DeadTimes    );    
                CConfig.ValToCon(tbCmb6ToInter          , ref OM.DevInfo.iCmb6ToInter      ); 
            }
            else 
            {
                OM.CDevInfo DevInfo = OM.DevInfo;

                CConfig.ConToVal(tbShakeCnt             , ref OM.DevInfo.iShakeCnt         );

                CConfig.ConToVal(tbBloodChamberSpd      , ref OM.DevInfo.iBloodChamberSpd  );
                CConfig.ConToVal(tbDeadVolSpd           , ref OM.DevInfo.iDeadVolSpd       );

                CConfig.ConToVal(tbBloodReadyCnt        , ref OM.DevInfo.iBloodReadyCnt    );
                CConfig.ConToVal(tbDCReadyCnt           , ref OM.DevInfo.iDCReadyCnt       );
                CConfig.ConToVal(tbFCMReadyCnt          , ref OM.DevInfo.iFCMReadyCnt      );
                CConfig.ConToVal(tbNRReadyCnt           , ref OM.DevInfo.iNRReadyCnt       );
                CConfig.ConToVal(tbRETReadyCnt          , ref OM.DevInfo.iRETReadyCnt      );
                CConfig.ConToVal(tb4DLReadyCnt          , ref OM.DevInfo.i4DLReadyCnt      );
                                                     
                CConfig.ConToVal(tbCmbEmptyTime         , ref OM.DevInfo.iCmbEmptyTime     );
                CConfig.ConToVal(tbWasteToExtTime       , ref OM.DevInfo.iWasteToExtTime   ); 
                CConfig.ConToVal(tbFcmWastToWastTime    , ref OM.DevInfo.iFcmWastToWastTime); 
                CConfig.ConToVal(tbHgbToWastTime        , ref OM.DevInfo.iHgbToWastTime    ); 
                CConfig.ConToVal(tbBloodPreCutVol       , ref OM.DevInfo.iBloodPreCutVol   ); 

                CConfig.ConToVal(tbFCMTestStartDelay    , ref OM.DevInfo.iFCMTestStartDelay); 
                CConfig.ConToVal(tbFCMTestEndDelay      , ref OM.DevInfo.iFCMTestEndDelay  ); 
                CConfig.ConToVal(tbFCMTestDelayTime     , ref OM.DevInfo.iFCMTestDelayTime ); 


                CConfig.ConToVal(tbCmb1BloodVol         , ref OM.DevInfo.iCmb1BloodVol     );
                CConfig.ConToVal(tbCmb1Cp2Time          , ref OM.DevInfo.iCmb1Cp2Time      );
              //CConfig.ConToVal(tbCmb1TankTime         , ref OM.DevInfo.iCmb1TankTime     );
              //CConfig.ConToVal(tbCmb1SylnPos          , ref OM.DevInfo.iCmb1SylnPos      );
              //CConfig.ConToVal(tbCmb1SylSpdCode       , ref OM.DevInfo.iCmb1SylSpdCode   );
                CConfig.ConToVal(tbCmb1Cp3Time          , ref OM.DevInfo.iCmb1Cp3Time      );                
                CConfig.ConToVal(tbCmb1DCSylPos         , ref OM.DevInfo.iCmb1DCSylPos     );
                CConfig.ConToVal(tbCmb1DCSylSpdCode     , ref OM.DevInfo.iCmb1DCSylSpdCode );    
                CConfig.ConToVal(tbCmb1CleanRvsPos      , ref OM.DevInfo.iCmb1CleanRvsPos  ); 
                CConfig.ConToVal(tbCmb1DeadVol          , ref OM.DevInfo.iCmb1DeadVol      );
                CConfig.ConToVal(tbCmb1DeadTimes        , ref OM.DevInfo.iCmb1DeadTimes    ); 
                CConfig.ConToVal(tbDccInspDelayTime     , ref OM.DevInfo.iDccInspDelayTime ); 
                CConfig.ConToVal(tbDccCleanTime         , ref OM.DevInfo.iDccCleanTime     );
                CConfig.ConToVal(tbCmb1ToInter          , ref OM.DevInfo.iCmb1ToInter      ); 
                

                CConfig.ConToVal(tbCmb2BloodVol         , ref OM.DevInfo.iCmb2BloodVol     );
                CConfig.ConToVal(tbCmb2Cp2Time          , ref OM.DevInfo.iCmb2Cp2Time      );
                CConfig.ConToVal(tbCmb2TankTime         , ref OM.DevInfo.iCmb2TankTime     );
              //CConfig.ConToVal(tbCmb2SylnPos          , ref OM.DevInfo.iCmb2SylnPos      );
              //CConfig.ConToVal(tbCmb2SylSpdCode       , ref OM.DevInfo.iCmb2SylSpdCode   );
                CConfig.ConToVal(tbCmb2Cp3Time          , ref OM.DevInfo.iCmb2Cp3Time      );                
              //CConfig.ConToVal(tbCmb2DCSylPos         , ref OM.DevInfo.iCmb2DCSylPos     );
              //CConfig.ConToVal(tbCmb2DCSylSpdCode     , ref OM.DevInfo.iCmb2DCSylSpdCode );  
                CConfig.ConToVal(tbCmb2CleanRvsPos      , ref OM.DevInfo.iCmb2CleanRvsPos  );     
                CConfig.ConToVal(tbCmb2BubbleTime       , ref OM.DevInfo.iCmb2BubbleTime   );  
                CConfig.ConToVal(tbCmb2Blk              , ref OM.DevInfo.iCmb2Blk          );     
                CConfig.ConToVal(tbCmb2SpecAngle        , ref OM.DevInfo.dCmb2SpecAngle    ); 
                
                CConfig.ConToVal(tbCmb3BloodVol         , ref OM.DevInfo.iCmb3BloodVol     );
              //CConfig.ConToVal(tbCmb3Cp2Time          , ref OM.DevInfo.iCmb3Cp2Time      );
                CConfig.ConToVal(tbCmb3TankTime         , ref OM.DevInfo.iCmb3TankTime     );
                CConfig.ConToVal(tbCmb3SylnPos          , ref OM.DevInfo.iCmb3SylnPos      );
                CConfig.ConToVal(tbCmb3SylSpdCode       , ref OM.DevInfo.iCmb3SylSpdCode   );
                CConfig.ConToVal(tbCmb3Cp3Time          , ref OM.DevInfo.iCmb3Cp3Time      );                
                CConfig.ConToVal(tbCmb3FCMSylPos        , ref OM.DevInfo.iCmb3FCMSylPos    );
                CConfig.ConToVal(tbCmb3FCMSylSpdCode    , ref OM.DevInfo.iCmb3FCMSylSpdCode);    
                CConfig.ConToVal(tbCmb3CleanRvsPos      , ref OM.DevInfo.iCmb3CleanRvsPos  );   
                CConfig.ConToVal(tbCmb3DeadVol          , ref OM.DevInfo.iCmb3DeadVol      );
                CConfig.ConToVal(tbCmb3DeadTimes        , ref OM.DevInfo.iCmb3DeadTimes    );    
                CConfig.ConToVal(tbCmb3ToInter          , ref OM.DevInfo.iCmb3ToInter      ); 

                CConfig.ConToVal(tbCmb4BloodVol         , ref OM.DevInfo.iCmb4BloodVol     );
              //CConfig.ConToVal(tbCmb4Cp2Time          , ref OM.DevInfo.iCmb4Cp2Time      );
                CConfig.ConToVal(tbCmb4TankTime         , ref OM.DevInfo.iCmb4TankTime     );
                CConfig.ConToVal(tbCmb4SylnPos          , ref OM.DevInfo.iCmb4SylnPos      );
                CConfig.ConToVal(tbCmb4SylSpdCode       , ref OM.DevInfo.iCmb4SylSpdCode   );
                CConfig.ConToVal(tbCmb4Cp3Time          , ref OM.DevInfo.iCmb4Cp3Time      );                
                CConfig.ConToVal(tbCmb4FCMSylPos        , ref OM.DevInfo.iCmb4FCMSylPos    );
                CConfig.ConToVal(tbCmb4FCMSylSpdCode    , ref OM.DevInfo.iCmb4FCMSylSpdCode);        
                CConfig.ConToVal(tbCmb4CleanRvsPos      , ref OM.DevInfo.iCmb4CleanRvsPos  );   
                CConfig.ConToVal(tbCmb4DeadVol          , ref OM.DevInfo.iCmb4DeadVol      );
                CConfig.ConToVal(tbCmb4DeadTimes        , ref OM.DevInfo.iCmb4DeadTimes    );    
                CConfig.ConToVal(tbCmb4ToInter          , ref OM.DevInfo.iCmb4ToInter      ); 
                

                CConfig.ConToVal(tbCmb5BloodVol         , ref OM.DevInfo.iCmb5BloodVol     );
              //CConfig.ConToVal(tbCmb5Cp2Time          , ref OM.DevInfo.iCmb5Cp2Time      );
                CConfig.ConToVal(tbCmb5TankTime         , ref OM.DevInfo.iCmb5TankTime     );
                CConfig.ConToVal(tbCmb5SylnPos          , ref OM.DevInfo.iCmb5SylnPos      );
                CConfig.ConToVal(tbCmb5SylSpdCode       , ref OM.DevInfo.iCmb5SylSpdCode   );
                CConfig.ConToVal(tbCmb5Cp3Time          , ref OM.DevInfo.iCmb5Cp3Time      );                
                CConfig.ConToVal(tbCmb5FCMSylPos        , ref OM.DevInfo.iCmb5FCMSylPos    );
                CConfig.ConToVal(tbCmb5FCMSylSpdCode    , ref OM.DevInfo.iCmb5FCMSylSpdCode);   
                CConfig.ConToVal(tbCmb5CleanRvsPos      , ref OM.DevInfo.iCmb5CleanRvsPos  );   
                CConfig.ConToVal(tbCmb5DeadVol          , ref OM.DevInfo.iCmb5DeadVol      );
                CConfig.ConToVal(tbCmb5DeadTimes        , ref OM.DevInfo.iCmb5DeadTimes    );    
                CConfig.ConToVal(tbCmb5ToInter          , ref OM.DevInfo.iCmb5ToInter      ); 
                

                CConfig.ConToVal(tbCmb6BloodVol         , ref OM.DevInfo.iCmb6BloodVol     );
              //CConfig.ConToVal(tbCmb6Cp2Time          , ref OM.DevInfo.iCmb6Cp2Time      );
                CConfig.ConToVal(tbCmb6TankTime         , ref OM.DevInfo.iCmb6TankTime     );
                CConfig.ConToVal(tbCmb6SylnPos          , ref OM.DevInfo.iCmb6SylnPos      );
                CConfig.ConToVal(tbCmb6SylSpdCode       , ref OM.DevInfo.iCmb6SylSpdCode   );
                CConfig.ConToVal(tbCmb6Cp3Time          , ref OM.DevInfo.iCmb6Cp3Time      );                
                CConfig.ConToVal(tbCmb6FCMSylPos        , ref OM.DevInfo.iCmb6FCMSylPos    );
                CConfig.ConToVal(tbCmb6FCMSylSpdCode    , ref OM.DevInfo.iCmb6FCMSylSpdCode);
                CConfig.ConToVal(tbCmb6CleanRvsPos      , ref OM.DevInfo.iCmb6CleanRvsPos  );   
                CConfig.ConToVal(tbCmb6DeadVol          , ref OM.DevInfo.iCmb6DeadVol      );
                CConfig.ConToVal(tbCmb6DeadTimes        , ref OM.DevInfo.iCmb6DeadTimes    );    
                CConfig.ConToVal(tbCmb6ToInter          , ref OM.DevInfo.iCmb6ToInter      ); 


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
            //metroTile7.Text = tcPosition.Left.ToString() + " " + tcPosition.Width.ToString() + " " + this.Left.ToString() + " W" + this.Width.ToString() + " H" + this.Height.ToString();
            //btSave.Text = btSave.Left.ToString() ;
            if (!this.Visible)
            {
                tmUpdate.Enabled = false;
                return;
            }
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
            if (this.Visible) tmUpdate.Enabled = true;
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

            PM.UpdatePstn(false);
            PM.Save(OM.GetCrntDev());
            PM.UpdatePstn(true);

            ////Loader.
            //DM.ARAY[ri.LODR].SetMaxColRow(1 , OM.DevInfo.iMgzSlotCnt);
            //


            //ArrayPos.TPara PosPara ;//= new ArrayPos.TPara();
            //PosPara.dColGrGap  = OM.DevInfo.dColGrGap  ;            
            //if(!OM .StripPos.SetPara(PosPara))
            //{
            //    Log.ShowMessage("Strip Position Err" , OM .StripPos.Error);
            //}

            Refresh();
        }




    }
}
