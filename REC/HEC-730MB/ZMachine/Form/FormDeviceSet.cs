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

            tabControl6.TabPages.Remove(tabPage6);

            tbUserUnit.Text = "0.01";
            PstnDisp();

            OM.LoadLastInfo();
            PM.Load(OM.GetCrntDev().ToString());

            PM.UpdatePstn(true);
            //UpdateDevInfo(true);
            
            //DM.ARAY[ri.MASK].SetParent(pnTrayMask); DM.ARAY[ri.MASK].Name = "MASK";
            //LoadMask(OM.GetCrntDev().ToString());            
            //DM.ARAY[ri.MASK].SetDisp(cs.Empty,"Empty",Color.Silver);
            //DM.ARAY[ri.MASK].SetDisp(cs.None ,"None" ,Color.White );            
           
            FraMotr     = new FraMotr    [(int)mi.MAX_MOTR  ];
            //FraCylinder = new FraCylOneBt[(int)ci.MAX_ACTR  ];

            //모터 축 수에 맞춰 FrameMotr 생성
            for (int m = 0; m < (int)mi.MAX_MOTR; m++)
            {
                Control[] Ctrl = tcDeviceSet.Controls.Find("pnMotrJog" +  m.ToString(), true);
                if(Ctrl.Length == 0) break;
                MOTION_DIR eDir = ML.MT_GetDirType((mi)m);
                FraMotr[m] = new FraMotr();
                FraMotr[m].SetIdType((mi)m, eDir);
                FraMotr[m].TopLevel = false;
                FraMotr[m].Parent = Ctrl[0];
                FraMotr[m].Show();
                FraMotr[m].SetUnit(EN_UNIT_TYPE.utJog, 0);
            }

            //여기 AP텍에서만 쓰는거
            //실린더 버튼 AP텍꺼
            FraCylAPT = new FrameCylinderAPT[(int)ci.MAX_ACTR];
            for (int i = 0; i < (int)ci.MAX_ACTR; i++)
            {
                Control[] CtrlAP = tcDeviceSet.Controls.Find("pnActrAP" + i.ToString(), true);
                if(CtrlAP.Length == 0) break;
                FraCylAPT[i] = new FrameCylinderAPT();
                FraCylAPT[i].TopLevel = false;
                FraCylAPT[i].SetConfig((ci)i, SM.CL.GetName(i).ToString(), ML.CL_GetDirType((ci)i), CtrlAP[0]);
                FraCylAPT[i].Show();
            }

            //Input Status 생성 AP텍꺼
            const int iInputBtnCnt  = 11;
            FraInputAPT = new FrameInputAPT[iInputBtnCnt];
            for (int i = 0; i < iInputBtnCnt; i++)
            {
                Control[] Ctrl = tcDeviceSet.Controls.Find("pnInput" + i.ToString(), true);
                if(Ctrl.Length == 0) break;
                int iIOCtrl = Convert.ToInt32(Ctrl[0].Tag);

                FraInputAPT[i] = new FrameInputAPT();
                FraInputAPT[i].TopLevel = false;
                FraInputAPT[i].SetConfig((xi)i, ML.IO_GetXName((xi)i), Ctrl[0]);
                FraInputAPT[i].Show();
            }

            //Output Status 생성 AP텍꺼
            const int iOutputBtnCnt = 4;
            FraOutputAPT = new FrameOutputAPT[iOutputBtnCnt];
            for (int i = 0; i < iOutputBtnCnt; i++)
            {
                Control[] Ctrl = tcDeviceSet.Controls.Find("pnOutput" + i.ToString(), true);
                if(Ctrl.Length == 0) break;
                int iIOCtrl = Convert.ToInt32(Ctrl[0].Tag);

                FraOutputAPT[i] = new FrameOutputAPT();
                FraOutputAPT[i].TopLevel = false;
            
                switch (iIOCtrl)
                {
                    default: break;
                    //case (int)yi.TOOL_PckrVac: FraOutputAPT[i].SetConfig(yi.TOOL_PckrVac, ML.IO_GetYName(yi.TOOL_PckrVac), Ctrl[0]); break;
                    
                }

                FraOutputAPT[i].Show();
            }

            //모터 포지션 AP텍꺼
            FraMotrPosAPT = new FrameMotrPosAPT[(int)mi.MAX_MOTR];
            for (int i = 0; i < (int)mi.MAX_MOTR; i++)
            {
                Control[] Ctrl = tcDeviceSet.Controls.Find("pnMotrPos" + i.ToString(), true);
                if(Ctrl.Length == 0) break;
                FraMotrPosAPT[i] = new FrameMotrPosAPT();
                FraMotrPosAPT[i].TopLevel = false;
                FraMotrPosAPT[i].SetWindow(i, Ctrl[0]);
                FraMotrPosAPT[i].Show();
            }
        }
       
        public void PstnDisp()
        {
            /*
            PM.SetProp((uint)mi.L_UP_ZLift, (uint)pv.L_UP_ZLiftWait        , "Wait                 ", false, false, true );
            PM.SetProp((uint)mi.L_UP_ZLift, (uint)pv.L_UP_ZLiftWaitG       , "Wait(Grip)           ", false, false, false);
            PM.SetProp((uint)mi.L_UP_ZLift, (uint)pv.L_UP_ZLiftTchB        , "Before Touch         ", false, false, true );
            //PM.SetProp((uint)mi.L_UP_ZLift, (uint)pv.L_UP_ZLiftTchH        , "Touch(변위모드)      ", false, false, false);
            //PM.SetProp((uint)mi.L_UP_ZLift, (uint)pv.L_UP_ZLiftTchW        , "Touch Setting(Weight)", false, false, false);
            PM.SetProp((uint)mi.L_UP_ZLift, (uint)pv.L_UP_ZLiftTchA        , "Touch Measure(Auto)  ", true , true , true );
            PM.SetCheckSafe((uint)mi.L_UP_ZLift, SEQ.LEFT.CheckSafe);                           
                                                                                                
            PM.SetProp((uint)mi.L_DN_ZLift, (uint)pv.L_DN_ZLiftWait        , "Wait                 ", false, false, true );
            PM.SetProp((uint)mi.L_DN_ZLift, (uint)pv.L_DN_ZLiftTchB        , "Before Touch         ", false, false, true );
            //PM.SetProp((uint)mi.L_DN_ZLift, (uint)pv.L_DN_ZLiftTchH        , "Touch(변위모드)      ", false, false, false);
            //PM.SetProp((uint)mi.L_DN_ZLift, (uint)pv.L_DN_ZLiftTchW        , "Touch Setting(Weight)", false, false, false);
            PM.SetProp((uint)mi.L_DN_ZLift, (uint)pv.L_DN_ZLiftTchA        , "Touch Measure(Auto)  ", true , true , true );
            PM.SetCheckSafe((uint)mi.L_DN_ZLift, SEQ.LEFT.CheckSafe);                           
                                                                                                
            PM.SetProp((uint)mi.R_UP_ZLift, (uint)pv.R_UP_ZLiftWait        , "Wait(Biting)         ", false, false, true );
            PM.SetProp((uint)mi.R_UP_ZLift, (uint)pv.R_UP_ZLiftWaitP       , "Wait(Pulling)        ", false, false, true );
            //PM.SetProp((uint)mi.R_UP_ZLift, (uint)pv.R_UP_ZLiftWaitG       , "Wait(Grip)           ", false, false, false);
            PM.SetProp((uint)mi.R_UP_ZLift, (uint)pv.R_UP_ZLiftTchB        , "Before Touch         ", false, false, true );
            
            //PM.SetProp((uint)mi.R_UP_ZLift, (uint)pv.R_UP_ZLiftTchH        , "Touch(변위모드)      ", false, false, false);
            //PM.SetProp((uint)mi.R_UP_ZLift, (uint)pv.R_UP_ZLiftTchW        , "Touch Setting(Weight)", false, false, false);
            PM.SetProp((uint)mi.R_UP_ZLift, (uint)pv.R_UP_ZLiftTchA        , "Touch Measure(Auto)  ", true , true , true );
            PM.SetCheckSafe((uint)mi.R_UP_ZLift, SEQ.RIGH.CheckSafe);
            */
            PM.SetProp((uint)mi.L_UP_ZLift, (uint)pv.L_UP_ZLiftWait        , "Wait                 ", false, false, false );
            PM.SetProp((uint)mi.L_UP_ZLift, (uint)pv.L_UP_ZLiftWaitG       , "Wait(Grip)           ", false, false, false);
            PM.SetProp((uint)mi.L_UP_ZLift, (uint)pv.L_UP_ZLiftTchB        , "Before Touch         ", false, false, false );
            //PM.SetProp((uint)mi.L_UP_ZLift, (uint)pv.L_UP_ZLiftTchH        , "Touch(변위모드)      ", false, false, false);
            //PM.SetProp((uint)mi.L_UP_ZLift, (uint)pv.L_UP_ZLiftTchW        , "Touch Setting(Weight)", false, false, false);
            PM.SetProp((uint)mi.L_UP_ZLift, (uint)pv.L_UP_ZLiftTchA        , "Touch Measure(Auto)  ", true , true , true );
            PM.SetCheckSafe((uint)mi.L_UP_ZLift, SEQ.LEFT.CheckSafe);                           
                                                                                                
            PM.SetProp((uint)mi.L_DN_ZLift, (uint)pv.L_DN_ZLiftWait        , "Wait                 ", false, false, false );
            PM.SetProp((uint)mi.L_DN_ZLift, (uint)pv.L_DN_ZLiftTchB        , "Before Touch         ", false, false, false );
            //PM.SetProp((uint)mi.L_DN_ZLift, (uint)pv.L_DN_ZLiftTchH        , "Touch(변위모드)      ", false, false, false);
            //PM.SetProp((uint)mi.L_DN_ZLift, (uint)pv.L_DN_ZLiftTchW        , "Touch Setting(Weight)", false, false, false);
            PM.SetProp((uint)mi.L_DN_ZLift, (uint)pv.L_DN_ZLiftTchA        , "Touch Measure(Auto)  ", true , true , true );
            PM.SetCheckSafe((uint)mi.L_DN_ZLift, SEQ.LEFT.CheckSafe);                           
                                                                                                
            PM.SetProp((uint)mi.R_UP_ZLift, (uint)pv.R_UP_ZLiftWait        , "Wait(Biting)         ", false, false, false );
            PM.SetProp((uint)mi.R_UP_ZLift, (uint)pv.R_UP_ZLiftWaitP       , "Wait(Pulling)        ", false, false, false );
            //PM.SetProp((uint)mi.R_UP_ZLift, (uint)pv.R_UP_ZLiftWaitG       , "Wait(Grip)           ", false, false, false);
            PM.SetProp((uint)mi.R_UP_ZLift, (uint)pv.R_UP_ZLiftTchB        , "Before Touch         ", false, false, false );
            
            //PM.SetProp((uint)mi.R_UP_ZLift, (uint)pv.R_UP_ZLiftTchH        , "Touch(변위모드)      ", false, false, false);
            //PM.SetProp((uint)mi.R_UP_ZLift, (uint)pv.R_UP_ZLiftTchW        , "Touch Setting(Weight)", false, false, false);
            PM.SetProp((uint)mi.R_UP_ZLift, (uint)pv.R_UP_ZLiftTchA        , "Touch Measure(Auto)  ", true , true , true );
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
                //case 2  : gbJogUnit.Parent = pnJog3;                       break;
                //case 3  : gbJogUnit.Parent = pnJog4;                       break;
                //case 4  : gbJogUnit.Parent = pnJog5;                       break;
            }

            PM.UpdatePstn(true);
            //PM.Load(OM.GetCrntDev());
        }

        public void UpdateDevInfo(bool bToTable)
        {
            if (bToTable)
            {
                CConfig.ValToCon(tbL_11   , ref OM.DevInfo.dL_H_Height      );
                CConfig.ValToCon(tbL_12   , ref OM.DevInfo.dL_H_Acc         );
                CConfig.ValToCon(tbL_13   , ref OM.DevInfo.dL_H_Vel         );
                CConfig.ValToCon(tbL_14   , ref OM.DevInfo.dL_H_Dcc         );
                CConfig.ValToCon(tbL_15   , ref OM.DevInfo.iL_H_Time        );
                CConfig.ValToCon(tbL_16   , ref OM.DevInfo.iL_H_Count       );
                CConfig.ValToCon(tbL_17   , ref OM.DevInfo.dL_H_Over        );

                CConfig.ValToCon(tbL_21   , ref OM.DevInfo.dL_W_Weight      );
                CConfig.ValToCon(tbL_22   , ref OM.DevInfo.dL_W_Acc         );
                CConfig.ValToCon(tbL_23   , ref OM.DevInfo.dL_W_Vel         );
                CConfig.ValToCon(tbL_24   , ref OM.DevInfo.dL_W_Dcc         );
                CConfig.ValToCon(tbL_25   , ref OM.DevInfo.iL_W_Time        );
                CConfig.ValToCon(tbL_26   , ref OM.DevInfo.iL_W_Count       );
                CConfig.ValToCon(tbL_27   , ref OM.DevInfo.dL_W_Over        );

                CConfig.ValToCon(tbL_31   , ref OM.DevInfo.dL_D_Weight      );
                CConfig.ValToCon(tbL_32   , ref OM.DevInfo.dL_D_Height      );
                CConfig.ValToCon(tbL_33   , ref OM.DevInfo.iL_D_Time        );
                CConfig.ValToCon(tbL_34   , ref OM.DevInfo.dL_D_Over        );
                                                                
                CConfig.ValToCon(tbR_11   , ref OM.DevInfo.dR_H_Height      );
                CConfig.ValToCon(tbR_12   , ref OM.DevInfo.dR_H_Acc         );
                CConfig.ValToCon(tbR_13   , ref OM.DevInfo.dR_H_Vel         );
                CConfig.ValToCon(tbR_14   , ref OM.DevInfo.dR_H_Dcc         );
                CConfig.ValToCon(tbR_15   , ref OM.DevInfo.iR_H_Time        );
                CConfig.ValToCon(tbR_16   , ref OM.DevInfo.iR_H_Count       );
                CConfig.ValToCon(tbR_17   , ref OM.DevInfo.dR_H_Over        );
                CConfig.ValToCon(tbR_18   , ref OM.DevInfo.iR_H_Manual      );
                                   
                CConfig.ValToCon(tbR_21   , ref OM.DevInfo.dR_W_Weight      );
                CConfig.ValToCon(tbR_22   , ref OM.DevInfo.dR_W_Acc         );
                CConfig.ValToCon(tbR_23   , ref OM.DevInfo.dR_W_Vel         );
                CConfig.ValToCon(tbR_24   , ref OM.DevInfo.dR_W_Dcc         );
                CConfig.ValToCon(tbR_25   , ref OM.DevInfo.iR_W_Time        );
                CConfig.ValToCon(tbR_26   , ref OM.DevInfo.iR_W_Count       );
                CConfig.ValToCon(tbR_27   , ref OM.DevInfo.dR_W_Over        );
                CConfig.ValToCon(tbR_28   , ref OM.DevInfo.iR_W_Manual      );
                                                              
                CConfig.ValToCon(tbR_31   , ref OM.DevInfo.dR_P_Height      );
                CConfig.ValToCon(tbR_32   , ref OM.DevInfo.dR_P_Acc         );
                CConfig.ValToCon(tbR_33   , ref OM.DevInfo.dR_P_Vel         );
                CConfig.ValToCon(tbR_34   , ref OM.DevInfo.dR_P_Dcc         );
                CConfig.ValToCon(tbR_35   , ref OM.DevInfo.iR_P_Time        );
                CConfig.ValToCon(tbR_36   , ref OM.DevInfo.iR_P_Count       );
                CConfig.ValToCon(tbR_37   , ref OM.DevInfo.dR_P_Over        );
                //cbL1.Items.Count = 4; 
                CConfig.ValToCon(cbL1     , ref OM.DevInfo.iL_Mode          );
                CConfig.ValToCon(cbL2     , ref OM.DevInfo.iL_Motr          );
                CConfig.ValToCon(cbL3     , ref OM.DevInfo.iR_Mode          );

                CConfig.ValToCon(tbL_01   , ref OM.DevInfo.sL_Name          );
                CConfig.ValToCon(tbL_02   , ref OM.DevInfo.iL_UsbCnt        );
                CConfig.ValToCon(tbL_03   , ref OM.DevInfo.sR_Name          );
                CConfig.ValToCon(tbL_04   , ref OM.DevInfo.iR_UsbCnt        );
                
            }
            else 
            {
                OM.CDevInfo DevInfo = OM.DevInfo;

                CConfig.ConToVal(tbL_11   , ref OM.DevInfo.dL_H_Height      ); SEQ.CheckValue(ref OM.DevInfo.dL_H_Height      , 0, 2     );
                CConfig.ConToVal(tbL_12   , ref OM.DevInfo.dL_H_Acc         ); SEQ.CheckValue(ref OM.DevInfo.dL_H_Acc         , 1, 2000  );
                CConfig.ConToVal(tbL_13   , ref OM.DevInfo.dL_H_Vel         ); SEQ.CheckValue(ref OM.DevInfo.dL_H_Vel         , 1, 150   );
                CConfig.ConToVal(tbL_14   , ref OM.DevInfo.dL_H_Dcc         ); SEQ.CheckValue(ref OM.DevInfo.dL_H_Dcc         , 1, 2000  );
                CConfig.ConToVal(tbL_15   , ref OM.DevInfo.iL_H_Time        ); SEQ.CheckValue(ref OM.DevInfo.iL_H_Time        , 0, 5000  );
                CConfig.ConToVal(tbL_16   , ref OM.DevInfo.iL_H_Count       ); SEQ.CheckValue(ref OM.DevInfo.iL_H_Count       , 1, 200000);
                CConfig.ConToVal(tbL_17   , ref OM.DevInfo.dL_H_Over        ); SEQ.CheckValue(ref OM.DevInfo.dL_H_Over        , 1, 25    );
                                                                                                                              
                CConfig.ConToVal(tbL_21   , ref OM.DevInfo.dL_W_Weight      ); SEQ.CheckValue(ref OM.DevInfo.dL_W_Weight      , 0, 25    );
                CConfig.ConToVal(tbL_22   , ref OM.DevInfo.dL_W_Acc         ); SEQ.CheckValue(ref OM.DevInfo.dL_W_Acc         , 1, 2000  );
                CConfig.ConToVal(tbL_23   , ref OM.DevInfo.dL_W_Vel         ); SEQ.CheckValue(ref OM.DevInfo.dL_W_Vel         , 1, 150   );
                CConfig.ConToVal(tbL_24   , ref OM.DevInfo.dL_W_Dcc         ); SEQ.CheckValue(ref OM.DevInfo.dL_W_Dcc         , 1, 2000  );
                CConfig.ConToVal(tbL_25   , ref OM.DevInfo.iL_W_Time        ); SEQ.CheckValue(ref OM.DevInfo.iL_W_Time        , 0, 5000  );
                CConfig.ConToVal(tbL_26   , ref OM.DevInfo.iL_W_Count       ); SEQ.CheckValue(ref OM.DevInfo.iL_W_Count       , 1, 200000);
                CConfig.ConToVal(tbL_27   , ref OM.DevInfo.dL_W_Over        ); SEQ.CheckValue(ref OM.DevInfo.dL_W_Over        , 1, 25);
                                          
                CConfig.ValToCon(tbL_31   , ref OM.DevInfo.dL_D_Weight      ); SEQ.CheckValue(ref OM.DevInfo.dL_D_Weight, 1   , 25  );
                CConfig.ValToCon(tbL_32   , ref OM.DevInfo.dL_D_Height      ); SEQ.CheckValue(ref OM.DevInfo.dL_D_Height, 0.01, 0.1 );
                CConfig.ValToCon(tbL_33   , ref OM.DevInfo.iL_D_Time        ); SEQ.CheckValue(ref OM.DevInfo.iL_D_Time  , 0   , 5000);
                CConfig.ValToCon(tbL_34   , ref OM.DevInfo.dL_D_Over        ); SEQ.CheckValue(ref OM.DevInfo.dL_D_Over  , 0   , 25  );

                CConfig.ConToVal(tbR_11   , ref OM.DevInfo.dR_H_Height      ); SEQ.CheckValue(ref OM.DevInfo.dR_H_Height      , 0, 2     );
                CConfig.ConToVal(tbR_12   , ref OM.DevInfo.dR_H_Acc         ); SEQ.CheckValue(ref OM.DevInfo.dR_H_Acc         , 1, 2000  );
                CConfig.ConToVal(tbR_13   , ref OM.DevInfo.dR_H_Vel         ); SEQ.CheckValue(ref OM.DevInfo.dR_H_Vel         , 1, 150   );
                CConfig.ConToVal(tbR_14   , ref OM.DevInfo.dR_H_Dcc         ); SEQ.CheckValue(ref OM.DevInfo.dR_H_Dcc         , 1, 2000  );
                CConfig.ConToVal(tbR_15   , ref OM.DevInfo.iR_H_Time        ); SEQ.CheckValue(ref OM.DevInfo.iR_H_Time        , 0, 5000  );
                CConfig.ConToVal(tbR_16   , ref OM.DevInfo.iR_H_Count       ); SEQ.CheckValue(ref OM.DevInfo.iR_H_Count       , 1, 200000);
                CConfig.ConToVal(tbR_17   , ref OM.DevInfo.dR_H_Over        ); SEQ.CheckValue(ref OM.DevInfo.dR_H_Over        , 1, 25    );
                CConfig.ConToVal(tbR_18   , ref OM.DevInfo.iR_H_Manual      );
                                   
                CConfig.ConToVal(tbR_21   , ref OM.DevInfo.dR_W_Weight      ); SEQ.CheckValue(ref OM.DevInfo.dR_W_Weight      , 0, 25    );
                CConfig.ConToVal(tbR_22   , ref OM.DevInfo.dR_W_Acc         ); SEQ.CheckValue(ref OM.DevInfo.dR_W_Acc         , 1, 2000  );
                CConfig.ConToVal(tbR_23   , ref OM.DevInfo.dR_W_Vel         ); SEQ.CheckValue(ref OM.DevInfo.dR_W_Vel         , 1, 150   );
                CConfig.ConToVal(tbR_24   , ref OM.DevInfo.dR_W_Dcc         ); SEQ.CheckValue(ref OM.DevInfo.dR_W_Dcc         , 1, 2000  );
                CConfig.ConToVal(tbR_25   , ref OM.DevInfo.iR_W_Time        ); SEQ.CheckValue(ref OM.DevInfo.iR_W_Time        , 0, 5000  );
                CConfig.ConToVal(tbR_26   , ref OM.DevInfo.iR_W_Count       ); SEQ.CheckValue(ref OM.DevInfo.iR_W_Count       , 1, 200000);
                CConfig.ConToVal(tbR_27   , ref OM.DevInfo.dR_W_Over        ); SEQ.CheckValue(ref OM.DevInfo.dR_W_Over        , 1, 25    );
                CConfig.ConToVal(tbR_28   , ref OM.DevInfo.iR_W_Manual      );
                                                                                                                
                CConfig.ConToVal(tbR_31   , ref OM.DevInfo.dR_P_Height      ); SEQ.CheckValue(ref OM.DevInfo.dR_P_Height      , 1, 40    );
                CConfig.ConToVal(tbR_32   , ref OM.DevInfo.dR_P_Acc         ); SEQ.CheckValue(ref OM.DevInfo.dR_P_Acc         , 1, 2000  );
                CConfig.ConToVal(tbR_33   , ref OM.DevInfo.dR_P_Vel         ); SEQ.CheckValue(ref OM.DevInfo.dR_P_Vel         , 1, 150   );
                CConfig.ConToVal(tbR_34   , ref OM.DevInfo.dR_P_Dcc         ); SEQ.CheckValue(ref OM.DevInfo.dR_P_Dcc         , 1, 2000  );
                CConfig.ConToVal(tbR_35   , ref OM.DevInfo.iR_P_Time        ); SEQ.CheckValue(ref OM.DevInfo.iR_P_Time        , 0, 5000  );
                CConfig.ConToVal(tbR_36   , ref OM.DevInfo.iR_P_Count       ); SEQ.CheckValue(ref OM.DevInfo.iR_P_Count       , 1, 200000);
                CConfig.ConToVal(tbR_37   , ref OM.DevInfo.dR_P_Over        ); SEQ.CheckValue(ref OM.DevInfo.dR_P_Over        , 1, 25    );

                CConfig.ConToVal(cbL1     , ref OM.DevInfo.iL_Mode          );
                CConfig.ConToVal(cbL2     , ref OM.DevInfo.iL_Motr          );
                CConfig.ConToVal(cbL3     , ref OM.DevInfo.iR_Mode          );

                CConfig.ConToVal(tbL_01   , ref OM.DevInfo.sL_Name          );
                CConfig.ConToVal(tbL_02   , ref OM.DevInfo.iL_UsbCnt        );
                CConfig.ConToVal(tbL_03   , ref OM.DevInfo.sR_Name          );
                CConfig.ConToVal(tbL_04   , ref OM.DevInfo.iR_UsbCnt        );
                

                Trace("접촉한 뒤에 내려갈 높이 (0 ~ 2)         ", DevInfo.dL_H_Height, OM.DevInfo.dL_H_Height);
                Trace("가속도 (초당 이동거리 1~2000)           ", DevInfo.dL_H_Acc   , OM.DevInfo.dL_H_Acc   );
                Trace("속도 (1 ~ 150)                          ", DevInfo.dL_H_Vel   , OM.DevInfo.dL_H_Vel   );
                Trace("감속도 (초당 이동거리 1~2000)           ", DevInfo.dL_H_Dcc   , OM.DevInfo.dL_H_Dcc   );
                Trace("유지시간 (0 ~ 5000)                     ", DevInfo.iL_H_Time  , OM.DevInfo.iL_H_Time  );
                Trace("횟수 (1 ~ 200000)                       ", DevInfo.iL_H_Count , OM.DevInfo.iL_H_Count );
                Trace("오버로드 (0 ~ 25)                       ", DevInfo.dL_H_Over  , OM.DevInfo.dL_H_Over  );
                
                Trace("가속도 (초당 이동거리 1~2000)           ", DevInfo.dL_W_Acc   , OM.DevInfo.dL_W_Acc   );
                Trace("속도 (1 ~ 150)                          ", DevInfo.dL_W_Vel   , OM.DevInfo.dL_W_Vel   );
                Trace("감속도 (초당 이동거리 1~2000)           ", DevInfo.dL_W_Dcc   , OM.DevInfo.dL_W_Dcc   );
                Trace("유지시간 (0 ~ 5000)                     ", DevInfo.iL_W_Time  , OM.DevInfo.iL_W_Time  );
                Trace("횟수 (1 ~ 200000)                       ", DevInfo.iL_W_Count , OM.DevInfo.iL_W_Count );
                Trace("오버로드 (0 ~ 25)                       ", DevInfo.dL_W_Over  , OM.DevInfo.dL_W_Over  );
                
                Trace("단계별 높이 변화값 (0.01~0.1)           ", DevInfo.dL_D_Height, OM.DevInfo.dL_D_Height);
                Trace("유지시간 (0 ~ 5000)                     ", DevInfo.iL_D_Time  , OM.DevInfo.iL_D_Time  );
                Trace("오버로드 (0 ~ 25)                       ", DevInfo.dL_D_Over  , OM.DevInfo.dL_D_Over  ); 
                
                Trace("가속도 (초당 이동거리 1~2000)           ", DevInfo.dR_H_Acc   , OM.DevInfo.dR_H_Acc   );
                Trace("속도 (1 ~ 150)                          ", DevInfo.dR_H_Vel   , OM.DevInfo.dR_H_Vel   );
                Trace("감속도 (초당 이동거리 1~2000)           ", DevInfo.dR_H_Dcc   , OM.DevInfo.dR_H_Dcc   );
                Trace("유지시간 (0 ~ 5000)                     ", DevInfo.iR_H_Time  , OM.DevInfo.iR_H_Time  );
                Trace("횟수 (1 ~ 200000)                       ", DevInfo.iR_H_Count , OM.DevInfo.iR_H_Count );
                Trace("오버로드 (0 ~ 25)                       ", DevInfo.dR_H_Over  , OM.DevInfo.dR_H_Over  );
                Trace("운영모드 (0 - 오토, 1 - 메뉴얼)         ", DevInfo.iR_H_Manual, OM.DevInfo.iR_H_Manual);

                Trace("가속도 (초당 이동거리 1~2000)           ", DevInfo.dR_W_Acc   , OM.DevInfo.dR_W_Acc   );
                Trace("속도 (1 ~ 150)                          ", DevInfo.dR_W_Vel   , OM.DevInfo.dR_W_Vel   );
                Trace("감속도 (초당 이동거리 1~2000)           ", DevInfo.dR_W_Dcc   , OM.DevInfo.dR_W_Dcc   );
                Trace("유지시간 (0 ~ 5000)                     ", DevInfo.iR_W_Time  , OM.DevInfo.iR_W_Time  );
                Trace("횟수 (1 ~ 200000)                       ", DevInfo.iR_W_Count , OM.DevInfo.iR_W_Count );
                Trace("오버로드 (0 ~ 25)                       ", DevInfo.dR_W_Over  , OM.DevInfo.dR_W_Over  );
                Trace("운영모드 (0 - 오토, 1 - 메뉴얼)         ", DevInfo.iR_W_Manual, OM.DevInfo.iR_W_Manual);

                Trace("가속도 (초당 이동거리 1~2000)           ", DevInfo.dR_P_Acc   , OM.DevInfo.dR_P_Acc   );
                Trace("속도 (1 ~ 150)                          ", DevInfo.dR_P_Vel   , OM.DevInfo.dR_P_Vel   );
                Trace("감속도 (초당 이동거리 1~2000)           ", DevInfo.dR_P_Dcc   , OM.DevInfo.dR_P_Dcc   );
                Trace("유지시간 (0 ~ 5000)                     ", DevInfo.iR_P_Time  , OM.DevInfo.iR_P_Time  );
                Trace("횟수 (1 ~ 200000)                       ", DevInfo.iR_P_Count , OM.DevInfo.iR_P_Count );
                Trace("오버로드 (0 ~ 25)                       ", DevInfo.dR_P_Over  , OM.DevInfo.dR_P_Over  );
                                                                                                           
                Trace("사용할 모드 선택(0-변위모드.1-하중모드.2-파괴모드)                               ", DevInfo.iL_Mode    , OM.DevInfo.iL_Mode    );
                Trace("사용할 모터 선택(0-위아래 모터 모두 사용.1-위쪽 모터만 사용.2-아래쪽 모터만 사용)", DevInfo.iL_Motr    , OM.DevInfo.iL_Motr    );
                
                Trace("사용할 모드 선택(0-변위모드.1-하중모드.2-PULLING변위모드)                        ", DevInfo.iR_Mode    , OM.DevInfo.iR_Mode    );
                                                                                                           
                Trace("장치설명(LEFT)                              ", DevInfo.sL_Name    , OM.DevInfo.sL_Name    );
                Trace("장치갯수(LEFT)                              ", DevInfo.iL_UsbCnt  , OM.DevInfo.iL_UsbCnt  );
                Trace("장치설명(RIGHT)                             ", DevInfo.sR_Name    , OM.DevInfo.sR_Name    );
                Trace("장치갯수(RIGHT)                             ", DevInfo.iR_UsbCnt  , OM.DevInfo.iR_UsbCnt  );


                UpdateDevInfo(true);
            }
        }




        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            tmUpdate.Enabled = false;
            btSaveDevice.Enabled = !SEQ._bRun;

            //pnTrayMask.Refresh();
            lbL_1.Text = SEQ.AIO_GetX(ax.ETC_LoadCell1).ToString();
            lbL_2.Text = SEQ.AIO_GetX(ax.ETC_LoadCell2).ToString();
            lbL_3.Text = SEQ.AIO_GetX(ax.ETC_LoadCell3).ToString();

            lbR_1.Text = SEQ.AIO_GetX(ax.ETC_LoadCell1).ToString();
            lbR_2.Text = SEQ.AIO_GetX(ax.ETC_LoadCell2).ToString();
            lbR_3.Text = SEQ.AIO_GetX(ax.ETC_LoadCell3).ToString();


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
            btSaveDevice.Enabled = false;
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            if (Log.ShowMessageModal("Confirm", "Are you Sure?") != DialogResult.Yes) return;

            //UpdateDevInfo(false);

            OM.SaveDevInfo(OM.GetCrntDev().ToString());
            OM.SaveEqpOptn();

            //SaveMask(OM.GetCrntDev());

            DM.ARAY[ri.ARAY].SetMaxColRow(1, 1);

            //Refresh();
            btSaveDevice.Enabled = true;
        }

        private void btSavePosition_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            if (Log.ShowMessageModal("Confirm", "Are you Sure?") != DialogResult.Yes) return;

            PM.SetValue(mi.L_UP_ZLift, pv.L_UP_ZLiftTchA, 0);
            PM.SetValue(mi.L_DN_ZLift, pv.L_DN_ZLiftTchA, 0);
            PM.SetValue(mi.R_UP_ZLift, pv.R_UP_ZLiftTchA, 0);

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

        private void button1_Click(object sender, EventArgs e)
        {
            ML.IO_SetY(yi.ETC_LoadZero1, true);
            ML.IO_SetY(yi.ETC_LoadZero1, false);
            //double dTemp1 = ML.MT_GetEncPos(mi.L_UP_ZLift);
            //PM.SetValue(mi.L_UP_ZLift, pv.L_UP_ZLiftTchA, dTemp1);
            
            //double dTemp2 = PM.GetValue(mi.L_UP_ZLift, pv.L_UP_ZLiftTchA);
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
