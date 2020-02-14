using COMMON;
using SML2;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Machine
{
    public partial class FormDeviceSet : Form
    {
        public        FraMotr    []       FraMotr      ;
        public        FraCylOneBt[]       FraCylinder  ;
        public        FraOutput  []       FraOutput    ;
        public        FormMain            FrmMain      ;
        //여기 AP텍꺼
        public        FrameCylinderAPT []  FraCylAPT    ;
        public        FrameInputAPT    []  FraInputAPT  ;
        public        FrameOutputAPT   []  FraOutputAPT ;
        public        FrameMotrPosAPT  []  FraMotrPosAPT;

        public static void LoadTrayMask(string _sJobName)
        {
            //CConfig Config = new CConfig();
            //string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            //string sDevOptnPath = sExeFolder + "JobFile\\" + _sJobName + "\\TrayMask.ini";
            //DM.ARAY[ri.MASK].Load(Config, true);        

            CConfig Config = new CConfig();
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevOptnPath = sExeFolder + "JobFile\\" + _sJobName + "\\TrayMask.ini";

            Config.Load(sDevOptnPath, CConfig.EN_CONFIG_FILE_TYPE.ftIni);
            DM.ARAY[ri.MASK].Load(Config, true);


        }        
        public static void SaveTrayMask(string _sJobName)
        {
            //Read&Write.
            //CConfig Config = new CConfig();
            //string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            //string sDevOptnPath = sExeFolder + "JobFile\\" + _sJobName + "\\TrayMask.ini";
            //DM.ARAY[ri.MASK].Load(Config, false);



            CConfig Config = new CConfig();
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevOptnPath = sExeFolder + "JobFile\\" + _sJobName + "\\TrayMask.ini";

            DM.ARAY[ri.MASK].Load(Config, false);
            Config.Save(sDevOptnPath, CConfig.EN_CONFIG_FILE_TYPE.ftIni);
            

        }
       
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
            UpdateDevOptn(true);            
            
            DM.ARAY[ri.MASK].SetParent(pnTrayMask); DM.ARAY[ri.MASK].Name = "MASK";
            LoadTrayMask(OM.GetCrntDev().ToString());            
            DM.ARAY[ri.MASK].SetDisp(cs.Empty,"Empty",Color.Silver);
            DM.ARAY[ri.MASK].SetDisp(cs.None ,"None" ,Color.White );            
           
            FraMotr     = new FraMotr    [(int)mi.MAX_MOTR  ];
            FraCylinder = new FraCylOneBt[(int)ci.MAX_ACTR  ];

            //모터 축 수에 맞춰 FrameMotr 생성
            for (int m = 0; m < (int)mi.MAX_MOTR; m++)
            {
                Control[] Ctrl = tcDeviceSet.Controls.Find("pnMotrJog" +  m.ToString(), true);

                MOTION_DIR eDir = SM.MT_GetDirType((mi)m);
                FraMotr[m] = new FraMotr();
                FraMotr[m].SetIdType((mi)m, eDir);
                FraMotr[m].TopLevel = false;
                FraMotr[m].Parent = Ctrl[0];
                FraMotr[m].Show();
                FraMotr[m].SetUnit(EN_UNIT_TYPE.utJog, 0);
            }

            for (int i = 0; i < (int)mi.MAX_MOTR; i++)
            {
                Control[] Ctrl = tcDeviceSet.Controls.Find("pnMotrPos" + i.ToString(), true);

                switch (i)
                {
                    default: break;
                    case (int)mi.LODR_ZLift: SetMotrPanel((int)pv.MAX_PSTN_MOTR0, Ctrl[0]); break;
                    case (int)mi.TOOL_XRjct: SetMotrPanel((int)pv.MAX_PSTN_MOTR1, Ctrl[0]); break;
                    case (int)mi.IDXR_XRear: SetMotrPanel((int)pv.MAX_PSTN_MOTR2, Ctrl[0]); break;
                    case (int)mi.IDXF_XFrnt: SetMotrPanel((int)pv.MAX_PSTN_MOTR3, Ctrl[0]); break;
                    case (int)mi.TOOL_YTool: SetMotrPanel((int)pv.MAX_PSTN_MOTR4, Ctrl[0]); break;
                    case (int)mi.TOOL_ZPckr: SetMotrPanel((int)pv.MAX_PSTN_MOTR5, Ctrl[0]); break;
                    case (int)mi.BARZ_XPckr: SetMotrPanel((int)pv.MAX_PSTN_MOTR6, Ctrl[0]); break;
                    case (int)mi.BARZ_ZPckr: SetMotrPanel((int)pv.MAX_PSTN_MOTR7, Ctrl[0]); break;
                    case (int)mi.STCK_ZStck: SetMotrPanel((int)pv.MAX_PSTN_MOTR8, Ctrl[0]); break;
                    case (int)mi.TOOL_ZVisn: SetMotrPanel((int)pv.MAX_PSTN_MOTR9, Ctrl[0]); break;

                }
            }

            //여기 AP텍에서만 쓰는거
            
            
            FraCylAPT = new FrameCylinderAPT[(int)ci.MAX_ACTR];
            //실린더 버튼 AP텍꺼
            for (int i = 0; i < (int)ci.MAX_ACTR; i++)
            {
                Control[] CtrlAP = tcDeviceSet.Controls.Find("pnActrAP" + i.ToString(), true);

                FraCylAPT[i] = new FrameCylinderAPT();
                FraCylAPT[i].TopLevel = false;

                

                switch (i)
                {
                    default: break;
                    case (int)ci.LODR_ClampClOp    : FraCylAPT[i].SetConfig((ci)i, SML.CL.GetName(i).ToString(), SML.CL.GetDirType(i), CtrlAP[0]); break;
                    case (int)ci.LODR_SperatorUpDn : FraCylAPT[i].SetConfig((ci)i, SML.CL.GetName(i).ToString(), SML.CL.GetDirType(i), CtrlAP[0]); break;
                    case (int)ci.STCK_RailClOp     : FraCylAPT[i].SetConfig((ci)i, SML.CL.GetName(i).ToString(), SML.CL.GetDirType(i), CtrlAP[0]); break;
                    case (int)ci.IDXR_ClampUpDn    : FraCylAPT[i].SetConfig((ci)i, SML.CL.GetName(i).ToString(), SML.CL.GetDirType(i), CtrlAP[0]); break;
                    case (int)ci.IDXF_ClampUpDn    : FraCylAPT[i].SetConfig((ci)i, SML.CL.GetName(i).ToString(), SML.CL.GetDirType(i), CtrlAP[0]); break;
                    case (int)ci.IDXR_ClampClOp    : FraCylAPT[i].SetConfig((ci)i, SML.CL.GetName(i).ToString(), SML.CL.GetDirType(i), CtrlAP[0]); break;
                    case (int)ci.IDXF_ClampClOp    : FraCylAPT[i].SetConfig((ci)i, SML.CL.GetName(i).ToString(), SML.CL.GetDirType(i), CtrlAP[0]); break;
                    case (int)ci.STCK_RailTrayUpDn : FraCylAPT[i].SetConfig((ci)i, SML.CL.GetName(i).ToString(), SML.CL.GetDirType(i), CtrlAP[0]); break;
                    case (int)ci.STCK_StackStprUpDn: FraCylAPT[i].SetConfig((ci)i, SML.CL.GetName(i).ToString(), SML.CL.GetDirType(i), CtrlAP[0]); break;
                    case (int)ci.STCK_StackOpCl    : FraCylAPT[i].SetConfig((ci)i, SML.CL.GetName(i).ToString(), SML.CL.GetDirType(i), CtrlAP[0]); break;
                    case (int)ci.BARZ_BrcdStprUpDn : FraCylAPT[i].SetConfig((ci)i, SML.CL.GetName(i).ToString(), SML.CL.GetDirType(i), CtrlAP[0]); break;
                    case (int)ci.BARZ_BrcdTrayUpDn : FraCylAPT[i].SetConfig((ci)i, SML.CL.GetName(i).ToString(), SML.CL.GetDirType(i), CtrlAP[0]); break;
                    case (int)ci.BARZ_YPckrFwBw    : FraCylAPT[i].SetConfig((ci)i, SML.CL.GetName(i).ToString(), SML.CL.GetDirType(i), CtrlAP[0]); break;
                }
              
                FraCylAPT[i].Show();
            }

            //Input Status 생성 AP텍꺼
            const int iInputBtnCnt  = 11;
            FraInputAPT = new FrameInputAPT[iInputBtnCnt];
            for (int i = 0; i < iInputBtnCnt; i++)
            {
                Control[] Ctrl = tcDeviceSet.Controls.Find("pnInput" + i.ToString(), true);
                
                int iIOCtrl = Convert.ToInt32(Ctrl[0].Tag);

                FraInputAPT[i] = new FrameInputAPT();
                FraInputAPT[i].TopLevel = false;
            
                switch (iIOCtrl)
                {  
                    default : break;
                    case (int)xi.LODR_TrayDtct      : FraInputAPT[i].SetConfig(xi.LODR_TrayDtct     , SML.IO.GetXName((int)xi.LODR_TrayDtct     )  , Ctrl[0]); break;
                    case (int)xi.RAIL_TrayDtct1     : FraInputAPT[i].SetConfig(xi.RAIL_TrayDtct1    , SML.IO.GetXName((int)xi.RAIL_TrayDtct1    )  , Ctrl[0]); break;
                    case (int)xi.IDXR_TrayDtct      : FraInputAPT[i].SetConfig(xi.IDXR_TrayDtct     , SML.IO.GetXName((int)xi.IDXR_TrayDtct     )  , Ctrl[0]); break;
                    case (int)xi.IDXF_TrayDtct      : FraInputAPT[i].SetConfig(xi.IDXF_TrayDtct     , SML.IO.GetXName((int)xi.IDXF_TrayDtct     )  , Ctrl[0]); break;
                    case (int)xi.TOOL_PckrVac       : FraInputAPT[i].SetConfig(xi.TOOL_PckrVac      , SML.IO.GetXName((int)xi.TOOL_PckrVac      )  , Ctrl[0]); break;
                    case (int)xi.STCK_StackTrayDtct : FraInputAPT[i].SetConfig(xi.STCK_StackTrayDtct, SML.IO.GetXName((int)xi.STCK_StackTrayDtct)  , Ctrl[0]); break;
                    case (int)xi.STCK_StackUpDtct   : FraInputAPT[i].SetConfig(xi.STCK_StackUpDtct  , SML.IO.GetXName((int)xi.STCK_StackUpDtct  )  , Ctrl[0]); break;
                    case (int)xi.BARZ_BrcdTrayDtct  : FraInputAPT[i].SetConfig(xi.BARZ_BrcdTrayDtct , SML.IO.GetXName((int)xi.BARZ_BrcdTrayDtct )  , Ctrl[0]); break;
                    case (int)xi.BARZ_PckrBrcdDtct  : FraInputAPT[i].SetConfig(xi.BARZ_PckrBrcdDtct , SML.IO.GetXName((int)xi.BARZ_PckrBrcdDtct )  , Ctrl[0]); break;
                    case (int)xi.BARZ_TrayOutDtct   : FraInputAPT[i].SetConfig(xi.BARZ_TrayOutDtct  , SML.IO.GetXName((int)xi.BARZ_TrayOutDtct  )  , Ctrl[0]); break;
                    case (int)xi.BARZ_PckrVac       : FraInputAPT[i].SetConfig(xi.BARZ_PckrVac      , SML.IO.GetXName((int)xi.BARZ_PckrVac      )  , Ctrl[0]); break;
                }

                FraInputAPT[i].Show();
            }

            //Output Status 생성 AP텍꺼
            const int iOutputBtnCnt = 4;
            FraOutputAPT = new FrameOutputAPT[iOutputBtnCnt];
            for (int i = 0; i < iOutputBtnCnt; i++)
            {
                Control[] Ctrl = tcDeviceSet.Controls.Find("pnOutput" + i.ToString(), true);
            
                int iIOCtrl = Convert.ToInt32(Ctrl[0].Tag);

                FraOutputAPT[i] = new FrameOutputAPT();
                FraOutputAPT[i].TopLevel = false;
            
                switch (iIOCtrl)
                {
                    default: break;
                    case (int)yi.TOOL_PckrVac: FraOutputAPT[i].SetConfig(yi.TOOL_PckrVac, SML.IO.GetYName((int)yi.TOOL_PckrVac), Ctrl[0]); break;
                    case (int)yi.STCK_StackAC: FraOutputAPT[i].SetConfig(yi.STCK_StackAC, SML.IO.GetYName((int)yi.STCK_StackAC), Ctrl[0]); break;
                    case (int)yi.BARZ_PckrVac: FraOutputAPT[i].SetConfig(yi.BARZ_PckrVac, SML.IO.GetYName((int)yi.BARZ_PckrVac), Ctrl[0]); break;
                    case (int)yi.BARZ_BrcdAC : FraOutputAPT[i].SetConfig(yi.BARZ_BrcdAC , SML.IO.GetYName((int)yi.BARZ_BrcdAC) , Ctrl[0]); break;
                    
                }

                FraOutputAPT[i].Show();
            }

            //모터 포지션 AP텍꺼
            FraMotrPosAPT = new FrameMotrPosAPT[(int)mi.MAX_MOTR];
            for (int i = 0; i < (int)mi.MAX_MOTR; i++)
            {
                Control[] Ctrl = tcDeviceSet.Controls.Find("pnMotrPos" + i.ToString(), true);

                FraMotrPosAPT[i] = new FrameMotrPosAPT();
                FraMotrPosAPT[i].TopLevel = false;
                switch (i)
                {
                    default: break;
                    case (int)mi.LODR_ZLift: FraMotrPosAPT[i].SetWindow((int)mi.LODR_ZLift, Ctrl[0]); break;
                    case (int)mi.TOOL_XRjct: FraMotrPosAPT[i].SetWindow((int)mi.TOOL_XRjct, Ctrl[0]); break;
                    case (int)mi.IDXR_XRear: FraMotrPosAPT[i].SetWindow((int)mi.IDXR_XRear, Ctrl[0]); break;
                    case (int)mi.IDXF_XFrnt: FraMotrPosAPT[i].SetWindow((int)mi.IDXF_XFrnt, Ctrl[0]); break;
                    case (int)mi.TOOL_YTool: FraMotrPosAPT[i].SetWindow((int)mi.TOOL_YTool, Ctrl[0]); break;
                    case (int)mi.TOOL_ZPckr: FraMotrPosAPT[i].SetWindow((int)mi.TOOL_ZPckr, Ctrl[0]); break;
                    case (int)mi.BARZ_XPckr: FraMotrPosAPT[i].SetWindow((int)mi.BARZ_XPckr, Ctrl[0]); break;
                    case (int)mi.BARZ_ZPckr: FraMotrPosAPT[i].SetWindow((int)mi.BARZ_ZPckr, Ctrl[0]); break;
                    case (int)mi.STCK_ZStck: FraMotrPosAPT[i].SetWindow((int)mi.STCK_ZStck, Ctrl[0]); break;
                    case (int)mi.TOOL_ZVisn: FraMotrPosAPT[i].SetWindow((int)mi.TOOL_ZVisn, Ctrl[0]); break;

                }
                FraMotrPosAPT[i].Show();
            }
        }

        public void PstnDisp()
        {
            //LODR_ZLift                                 
            PM.SetProp((uint)mi.LODR_ZLift, (uint)pv.LODR_ZLiftWait        , "Wait                ", false, false, false);
            PM.SetProp((uint)mi.LODR_ZLift, (uint)pv.LODR_ZLiftPick        , "Tray Pick           ", false, false, false);
            PM.SetProp((uint)mi.LODR_ZLift, (uint)pv.LODR_ZLiftSperate     , "Sperate             ", false, false, false);
            PM.SetProp((uint)mi.LODR_ZLift, (uint)pv.LODR_ZLiftPlace       , "Tray Place          ", false, false, false);

            //TOOL_XRjct                                                                             
            PM.SetProp((uint)mi.TOOL_XRjct, (uint)pv.TOOL_XRjctWait        , "Wait                ", false, false, false);
            PM.SetProp((uint)mi.TOOL_XRjct, (uint)pv.TOOL_XRjctWrkStt      , "Index Work Start    ", false, false, false);

            //IDXR_XRear                                                                             
            PM.SetProp((uint)mi.IDXR_XRear, (uint)pv.IDXR_XRearWait        , "Wait                ", false, false, false);
            PM.SetProp((uint)mi.IDXR_XRear, (uint)pv.IDXR_XRearClamp       , "Tray Clamp          ", false, false, false);
            PM.SetProp((uint)mi.IDXR_XRear, (uint)pv.IDXR_XRearBarcode     , "Barcode Scan        ", false, false, false);
            PM.SetProp((uint)mi.IDXR_XRear, (uint)pv.IDXR_XRearVsnStt1     , "Vision 1st Start    ", false, false, false);
            PM.SetProp((uint)mi.IDXR_XRear, (uint)pv.IDXR_XRearVsnStt2     , "Vision 2nd Start    ", false, false, false);
            PM.SetProp((uint)mi.IDXR_XRear, (uint)pv.IDXR_XRearVsnStt3     , "Vision 3rd Start    ", false, false, false);
            PM.SetProp((uint)mi.IDXR_XRear, (uint)pv.IDXR_XRearVsnStt4     , "Vision 4th Start    ", false, false, false);
            PM.SetProp((uint)mi.IDXR_XRear, (uint)pv.IDXR_XRearWorkStt     , "Pick Start          ", false, false, false);
            PM.SetProp((uint)mi.IDXR_XRear, (uint)pv.IDXR_XRearUld         , "Unload              ", false, false, false);

            //IDXF_XFrnt                                                                             
            PM.SetProp((uint)mi.IDXF_XFrnt, (uint)pv.IDXF_XFrntWait        , "Wait                ", false, false, false);
            PM.SetProp((uint)mi.IDXF_XFrnt, (uint)pv.IDXF_XFrntClamp       , "Tray Clamp          ", false, false, false);
            PM.SetProp((uint)mi.IDXF_XFrnt, (uint)pv.IDXF_XFrntBarcode     , "Barcode Scan        ", false, false, false);
            PM.SetProp((uint)mi.IDXF_XFrnt, (uint)pv.IDXF_XFrntVsnStt1     , "Vision 1st Start    ", false, false, false);
            PM.SetProp((uint)mi.IDXF_XFrnt, (uint)pv.IDXF_XFrntVsnStt2     , "Vision 2nd Start    ", false, false, false);
            PM.SetProp((uint)mi.IDXF_XFrnt, (uint)pv.IDXF_XFrntVsnStt3     , "Vision 3rd Start    ", false, false, false);
            PM.SetProp((uint)mi.IDXF_XFrnt, (uint)pv.IDXF_XFrntVsnStt4     , "Vision 4th Start    ", false, false, false);
            PM.SetProp((uint)mi.IDXF_XFrnt, (uint)pv.IDXF_XFrntWorkStt     , "Pick Start          ", false, false, false);
            PM.SetProp((uint)mi.IDXF_XFrnt, (uint)pv.IDXF_XFrntUld         , "Unload              ", false, false, false);

            //TOOL_YTool                                                                         
            PM.SetProp((uint)mi.TOOL_YTool, (uint)pv.TOOL_YToolWait        , "Wait                ", false, false, false);
            PM.SetProp((uint)mi.TOOL_YTool, (uint)pv.TOOL_YToolVsnStt1     , "Vision 1st Start    ", false, false, false);
            PM.SetProp((uint)mi.TOOL_YTool, (uint)pv.TOOL_YToolVsnStt2     , "Vision 2nd Start    ", false, false, false);
            PM.SetProp((uint)mi.TOOL_YTool, (uint)pv.TOOL_YToolVsnStt3     , "Vision 3rd Start    ", false, false, false);
            PM.SetProp((uint)mi.TOOL_YTool, (uint)pv.TOOL_YToolVsnStt4     , "Vision 4th Start    ", false, false, false);
            PM.SetProp((uint)mi.TOOL_YTool, (uint)pv.TOOL_YToolIdxWorkStt  , "Rail Pick Start     ", false, false, false);
            PM.SetProp((uint)mi.TOOL_YTool, (uint)pv.TOOL_YToolNgTWorkStt  , "NG Tray Place Start ", false, false, false);
            PM.SetProp((uint)mi.TOOL_YTool, (uint)pv.TOOL_YToolGdTWorkStt  , "Good Tray Pick Start", false, false, false);

            //TOOL_ZPckr                                                                          
            PM.SetProp((uint)mi.TOOL_ZPckr, (uint)pv.TOOL_ZPckrWait        , "Wait                ", false, false, false);
            PM.SetProp((uint)mi.TOOL_ZPckr, (uint)pv.TOOL_ZPckrIdxPick     , "Rail Pick           ", false, false, false);
            PM.SetProp((uint)mi.TOOL_ZPckr, (uint)pv.TOOL_ZPckrIdxPlace    , "Rail Place          ", false, false, false);
            PM.SetProp((uint)mi.TOOL_ZPckr, (uint)pv.TOOL_ZPckrMove        , "Move                ", false, false, false);
            PM.SetProp((uint)mi.TOOL_ZPckr, (uint)pv.TOOL_ZPckrNgTWork     , "NG Tray Place       ", false, false, false);
            PM.SetProp((uint)mi.TOOL_ZPckr, (uint)pv.TOOL_ZPckrGdTWork     , "Good Tray Pick      ", false, false, false);

            //BARZ_XPckr                                                                       
            PM.SetProp((uint)mi.BARZ_XPckr, (uint)pv.BARZ_XPckrWait        , "Wait                ", false, false, true );
            PM.SetProp((uint)mi.BARZ_XPckr, (uint)pv.BARZ_XPckrPick        , "Pick                ", false, false, true );
            PM.SetProp((uint)mi.BARZ_XPckr, (uint)pv.BARZ_XPckrPlace       , "Place               ", false, false, true );
            PM.SetProp((uint)mi.BARZ_XPckr, (uint)pv.BARZ_XPckrBarc        , "Barcode Scan        ", false, false, true );
            PM.SetProp((uint)mi.BARZ_XPckr, (uint)pv.BARZ_XPckrRemove      , "Barcode ReMove      ", false, false, true );

            //BARZ_ZPckr                                                                          
            PM.SetProp((uint)mi.BARZ_ZPckr, (uint)pv.BARZ_ZPckrWait        , "Wait                ", false, false, true );
            PM.SetProp((uint)mi.BARZ_ZPckr, (uint)pv.BARZ_ZPckrCylFw       , "Picker Y Cyl Fwd    ", false, false, true );
            PM.SetProp((uint)mi.BARZ_ZPckr, (uint)pv.BARZ_ZPckrPick        , "Pick                ", false, false, true );
            PM.SetProp((uint)mi.BARZ_ZPckr, (uint)pv.BARZ_ZPckrMove        , "Move                ", false, false, true );
            PM.SetProp((uint)mi.BARZ_ZPckr, (uint)pv.BARZ_ZPckrPlaceCheck  , "Place Check         ", false, false, true );
            PM.SetProp((uint)mi.BARZ_ZPckr, (uint)pv.BARZ_ZPckrBarc        , "Barcode Scan        ", false, false, true );
            PM.SetProp((uint)mi.BARZ_ZPckr, (uint)pv.BARZ_ZPckrPlaceOfs    , "Place Offset        ", true , false, false);
            PM.SetProp((uint)mi.BARZ_ZPckr, (uint)pv.BARZ_ZPckrRemove      , "Barcode ReMove      ", false, false, true );

            //STCK_ZStck                                                                             
            PM.SetProp((uint)mi.STCK_ZStck, (uint)pv.STCK_ZStckWait        , "Wait                ", false, false, false);
            PM.SetProp((uint)mi.STCK_ZStck, (uint)pv.STCK_ZStckWork        , "Work                ", false, false, false);

            //TOOL_ZVisn                                                                             
            PM.SetProp((uint)mi.TOOL_ZVisn, (uint)pv.TOOL_ZVisnWait        , "Wait                ", false, false, false);
            PM.SetProp((uint)mi.TOOL_ZVisn, (uint)pv.TOOL_ZVisnWork1       , "1st Work            ", false, false, false);
            PM.SetProp((uint)mi.TOOL_ZVisn, (uint)pv.TOOL_ZVisnWork2       , "2nd Work            ", false, false, false);
            PM.SetProp((uint)mi.TOOL_ZVisn, (uint)pv.TOOL_ZVisnWork3       , "3rd Work            ", false, false, false);
            PM.SetProp((uint)mi.TOOL_ZVisn, (uint)pv.TOOL_ZVisnWork4       , "4th Work            ", false, false, false);
        }

        //모터 포지션 패널 사이즈 변경하는 함수 한라 전용
        //public void SetMotrPanel(int _iMaxPos, Control _wcParent)
        //{
        //    int iGroupBoxHghtSize = 28; //패널 사이즈 제외한 그룹박스 사이즈
        //    int iCellHeight = iGroupBoxHghtSize + ((PM.GetMotrPosHeight()+1) * _iMaxPos)+1 ;
        //    _wcParent.Height = iCellHeight;
        //
        //    int iGroupBoxWidthSize = 10;
        //    int iCellWidth = iGroupBoxWidthSize + PM.GetMotrPosWidth();
        //    _wcParent.Width = iCellWidth;
        //}

        public void SetMotrPanel(int _iMaxPos, Control _wcParent)
        {
            int iGroupBoxHghtSize = 46; //패널 사이즈 제외한 그룹박스 사이즈
            int iCellHeight = iGroupBoxHghtSize + ((PM.GetMotrPosHeight() + 1) * _iMaxPos) + 1;
            _wcParent.Height = iCellHeight;

            //int iGroupBoxWidthSize = 0;
            //int iCellWidth = iGroupBoxWidthSize + PM.GetMotrPosWidth();
            //_wcParent.Width = iCellWidth;
        }

        private void tcDeviceSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            int iSeletedIndex;
            iSeletedIndex = tcDeviceSet.SelectedIndex;
            
            switch (iSeletedIndex)
            {
                default : break;
                case 1  : gbJogUnit.Parent = pnJog1;                       break;
                case 2  : gbJogUnit.Parent = pnJog2;                       break;
                case 3  : gbJogUnit.Parent = pnJog3;                       break;
                case 4  : gbJogUnit.Parent = pnJog4;                       break;
                case 5  : gbJogUnit.Parent = pnJog5;                       break;
            }

            UpdateDevInfo(true);
            UpdateDevOptn(true);
            //LoadTrayMask(OM.GetCrntDev());

            PM.UpdatePstn(true);
            PM.Load(OM.GetCrntDev());
        }

        CDelayTimer TimeOut = new CDelayTimer();
        private void btSave_Click(object sender, EventArgs e)
        {
            Log.Trace("SAVE", "Clicked");

            if (Log.ShowMessageModal("Confirm", "Are you Sure?") != DialogResult.Yes) return;

            UpdateDevInfo(false);
            UpdateDevOptn(false);

            OM.SaveDevInfo(OM.GetCrntDev().ToString());
            OM.SaveDevOptn(OM.GetCrntDev().ToString());

            

            PM.UpdatePstn(false);
            PM.Save(OM.GetCrntDev());

            //SEQ.DispPtrn.Save(OM.GetCrntDev());
            //SEQ.HghtPtrn.Save(OM.GetCrntDev());
            //SEQ.DispPtrn.SavePttColor(OM.GetCrntDev());
            //SEQ.HghtPtrn.SavePttColor(OM.GetCrntDev());

            

            OM.SaveEqpOptn();

            //OM.TrayMask.SetMaxColRow(OM.DevInfo.iTRAY_PcktCntX, OM.DevInfo.iTRAY_PcktCntY);
            SaveTrayMask(OM.GetCrntDev());

            DM.ARAY[ri.SPLR].SetMaxColRow(1                        , 1                           );
            DM.ARAY[ri.IDXR].SetMaxColRow(OM.DevInfo.iTRAY_PcktCntX, OM.DevInfo.iTRAY_PcktCntY   );
            DM.ARAY[ri.IDXF].SetMaxColRow(OM.DevInfo.iTRAY_PcktCntX, OM.DevInfo.iTRAY_PcktCntY   );
            DM.ARAY[ri.PCKR].SetMaxColRow(1                        , 1                           );
            DM.ARAY[ri.TRYF].SetMaxColRow(OM.DevInfo.iTRAY_PcktCntX, OM.DevInfo.iTRAY_PcktCntY   );
            DM.ARAY[ri.TRYG].SetMaxColRow(OM.DevInfo.iTRAY_PcktCntX, OM.DevInfo.iTRAY_PcktCntY   );
            DM.ARAY[ri.OUTZ].SetMaxColRow(1                        , 1                           );
            
            //여기부터 하면 됨.
            int iPreGoodCnt = DM.ARAY[ri.STCK].GetCntStat(cs.Good) ;
            DM.ARAY[ri.STCK].SetMaxColRow(1                        , OM.DevInfo.iTRAY_StackingCnt);            
            DM.ARAY[ri.STCK].SetStat(cs.Empty);
            for (int r = DM.ARAY[ri.STCK].GetMaxRow() - 1 ; r >= 0  ; r--)
            {
                if(iPreGoodCnt>0){
                    DM.ARAY[ri.STCK].SetStat(0,r,cs.Good);
                    iPreGoodCnt--;
                }
            }


            DM.ARAY[ri.BARZ].SetMaxColRow(1                        , 1                           );
            DM.ARAY[ri.INSP].SetMaxColRow(1                        , OM.DevInfo.iTRAY_PcktCntY   );
            DM.ARAY[ri.PSTC].SetMaxColRow(1                        , 1                           );
            DM.ARAY[ri.MASK].SetMaxColRow(OM.DevInfo.iTRAY_PcktCntX, OM.DevInfo.iTRAY_PcktCntY   );
 //         DM.ARAY[ri.BPCK].SetMaxColRow(1                        , 1                           );

            DM.ARAY[ri.IDXR].SetMask(DM.ARAY[ri.MASK]);
            DM.ARAY[ri.IDXF].SetMask(DM.ARAY[ri.MASK]);
            DM.ARAY[ri.TRYF].SetMask(DM.ARAY[ri.MASK]);
            DM.ARAY[ri.TRYG].SetMask(DM.ARAY[ri.MASK]);
            

            //SEQ.Com[1].SendMsg(OM.DevInfo.sMrkData);
            //SetComboItem();

            Refresh();

        }

        public void UpdateDevInfo(bool bToTable)
        {
            if (bToTable)
            {
                CConfig.ValToCon(tbTRAY_PcktCntX     , ref OM.DevInfo.iTRAY_PcktCntX   );
                CConfig.ValToCon(tbTRAY_PcktCntY     , ref OM.DevInfo.iTRAY_PcktCntY   );
                CConfig.ValToCon(tbTRAY_PcktPitchX   , ref OM.DevInfo.dTRAY_PcktPitchX );
                CConfig.ValToCon(tbTRAY_PcktPitchY   , ref OM.DevInfo.dTRAY_PcktPitchY );
                CConfig.ValToCon(tbTRAY_StackingCnt  , ref OM.DevInfo.iTRAY_StackingCnt);
                CConfig.ValToCon(tbTRAY_BundleCnt    , ref OM.DevInfo.iTRAY_BundleCnt  );
            }
            else 
            {
                CConfig.ConToVal(tbTRAY_PcktCntX   , ref OM.DevInfo.iTRAY_PcktCntX   );
                CConfig.ConToVal(tbTRAY_PcktCntY   , ref OM.DevInfo.iTRAY_PcktCntY   );
                CConfig.ConToVal(tbTRAY_PcktPitchX , ref OM.DevInfo.dTRAY_PcktPitchX );
                CConfig.ConToVal(tbTRAY_PcktPitchY , ref OM.DevInfo.dTRAY_PcktPitchY );
                CConfig.ConToVal(tbTRAY_StackingCnt, ref OM.DevInfo.iTRAY_StackingCnt);
                CConfig.ConToVal(tbTRAY_BundleCnt  , ref OM.DevInfo.iTRAY_BundleCnt  );

                if(OM.DevInfo.iTRAY_PcktCntX    < 1) OM.DevInfo.iTRAY_PcktCntX    = 1 ;
                if(OM.DevInfo.iTRAY_PcktCntY    < 1) OM.DevInfo.iTRAY_PcktCntY    = 1 ;
                if(OM.DevInfo.dTRAY_PcktPitchX  <=0) OM.DevInfo.dTRAY_PcktPitchX  = 0.1 ;
                if(OM.DevInfo.dTRAY_PcktPitchY  <=0) OM.DevInfo.dTRAY_PcktPitchY  = 0.1 ;
                if(OM.DevInfo.iTRAY_StackingCnt < 2) OM.DevInfo.iTRAY_StackingCnt = 2 ;
                if(OM.DevInfo.iTRAY_BundleCnt   < 1) OM.DevInfo.iTRAY_BundleCnt   = 1 ;


                UpdateDevInfo(true);
            }
        
        }

        public void UpdateDevOptn(bool bToTable)
        {
            
            if (bToTable)
            {
                CConfig.ValToCon(tbPickDelay         , ref OM.DevOptn.iPickDelay         );
                CConfig.ValToCon(tbPlceDelay         , ref OM.DevOptn.iPlceDelay         );
                CConfig.ValToCon(tbInspSpeed         , ref OM.DevOptn.iInspSpeed         );
                CConfig.ValToCon(cbTotalInspCnt      , ref OM.DevOptn.iTotalInspCnt      );
                CConfig.ValToCon(cbNgInspCnt         , ref OM.DevOptn.iNgInspCnt         ); 
                CConfig.ValToCon(cbUseBtmCover       , ref OM.DevOptn.bUseBtmCover       ); 
                CConfig.ValToCon(tbTrgOfs            , ref OM.DevOptn.dTrgOfs            );
                CConfig.ValToCon(cbVisnNGCntErr      , ref OM.DevOptn.iVisnNGCntErr      );
                                                                                         
                                                                                         
                CConfig.ValToCon(cbUnitID            , ref OM.DevOptn.bUnitID            );
                CConfig.ValToCon(cbDuplicateDMC1     , ref OM.DevOptn.bDuplicateDMC1     );
                CConfig.ValToCon(cbDMC1Grouping      , ref OM.DevOptn.bDMC1Grouping      );
                CConfig.ValToCon(cbDMC1MonthLimit    , ref OM.DevOptn.iDMC1MonthLimit    );
                CConfig.ValToCon(cbDMC2              , ref OM.DevOptn.iDMC2CheckMathod   );//DMC2검사는 2가지 방법이 있어서 그거 설정.
                CConfig.ValToCon(cbBrightnessCheck   , ref OM.DevOptn.bBrightnessCheck   );
                CConfig.ValToCon(cbLDOMCheck         , ref OM.DevOptn.bLDOMCheck         );
                CConfig.ValToCon(cbCxCy              , ref OM.DevOptn.bCxCy              );
                CConfig.ValToCon(tbCompareDmc2Cnt    , ref OM.DevOptn.iCompareDmc2Cnt    );//DMC2검사일때 String Compare 검사시에 검사하는 캐릭터 카운트 수를 지정. 
                CConfig.ValToCon(cbUseDmc2CharLimit  , ref OM.DevOptn.bUseDmc2CharLimit  );
                CConfig.ValToCon(tbUseDmc2CharLimit  , ref OM.DevOptn.iDmc2CharLimit     );

                CConfig.ValToCon(cbUseBarcCyl        , ref OM.DevOptn.bUseBarcCyl        );
                



                 
            }
            else
            {
                CConfig.ConToVal(tbPickDelay         , ref OM.DevOptn.iPickDelay         );
                CConfig.ConToVal(tbPlceDelay         , ref OM.DevOptn.iPlceDelay         );
                CConfig.ConToVal(tbInspSpeed         , ref OM.DevOptn.iInspSpeed         );
                                                                                         
                                                                                         
                CConfig.ConToVal(cbTotalInspCnt      , ref OM.DevOptn.iTotalInspCnt      );
                CConfig.ConToVal(cbNgInspCnt         , ref OM.DevOptn.iNgInspCnt         );
                                                                                         
                CConfig.ConToVal(cbUseBtmCover       , ref OM.DevOptn.bUseBtmCover       ); 
                                                                                         
                CConfig.ConToVal(tbTrgOfs            , ref OM.DevOptn.dTrgOfs            ); 
                CConfig.ConToVal(cbVisnNGCntErr      , ref OM.DevOptn.iVisnNGCntErr      );
                                                                                         
                CConfig.ConToVal(cbUnitID            , ref OM.DevOptn.bUnitID            );
                CConfig.ConToVal(cbDuplicateDMC1     , ref OM.DevOptn.bDuplicateDMC1     );
                CConfig.ConToVal(cbDMC1Grouping      , ref OM.DevOptn.bDMC1Grouping      );
                CConfig.ConToVal(cbDMC1MonthLimit    , ref OM.DevOptn.iDMC1MonthLimit    );
                CConfig.ConToVal(cbDMC2              , ref OM.DevOptn.iDMC2CheckMathod   );//DMC2검사는 2가지 방법이 있어서 그거 설정.
                CConfig.ConToVal(cbBrightnessCheck   , ref OM.DevOptn.bBrightnessCheck   );
                CConfig.ConToVal(cbLDOMCheck         , ref OM.DevOptn.bLDOMCheck         );
                CConfig.ConToVal(cbCxCy              , ref OM.DevOptn.bCxCy              );
                CConfig.ConToVal(tbCompareDmc2Cnt    , ref OM.DevOptn.iCompareDmc2Cnt    );//DMC2검사일때 String Compare 검사시에 검사하는 캐릭터 카운트 수를 지정. 
                CConfig.ConToVal(cbUseDmc2CharLimit  , ref OM.DevOptn.bUseDmc2CharLimit  );
                CConfig.ConToVal(tbUseDmc2CharLimit  , ref OM.DevOptn.iDmc2CharLimit     );

                CConfig.ConToVal(cbUseBarcCyl        , ref OM.DevOptn.bUseBarcCyl        );
                UpdateDevOptn(true);
            }
        }


        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            tmUpdate.Enabled = false;
            btSaveDevice.Enabled = !SEQ._bRun;

            cbBrightnessCheck.Enabled = cbDMC2.SelectedIndex == 2 ;
            cbLDOMCheck      .Enabled = cbDMC2.SelectedIndex == 2 ;
            cbCxCy           .Enabled = cbDMC2.SelectedIndex == 2 ;

            tbCompareDmc2Cnt  .Enabled = cbDMC2.SelectedIndex == 1 ;
            lbCompareLimit    .Enabled = cbDMC2.SelectedIndex == 1 ;
            cbUseDmc2CharLimit.Enabled = cbDMC2.SelectedIndex == 1 ;
            tbUseDmc2CharLimit.Enabled = cbDMC2.SelectedIndex == 1 ;
            lbUseDmc2CharLimit.Enabled = cbDMC2.SelectedIndex == 1 ;

            //pbTRAY.Refresh();
            //pbStackTray.Refresh();
            pnTrayMask.Refresh();
            tmUpdate.Enabled = true;
        }


        private void pbSTG_Paint(object sender, PaintEventArgs e)
        {
            int iTag = Convert.ToInt32(((PictureBox)sender).Tag);

            SolidBrush Brush = new SolidBrush(Color.Black);

            Pen Pen = new Pen(Color.Black);

            Graphics gSTG = pbTRAY.CreateGraphics();


            double dX1, dX2, dY1, dY2;

            int iTrayColCnt, iTrayRowCnt;
           
            Graphics g = e.Graphics;

            switch (iTag)
            {
                default: break;
                case 1: 
                    iTrayColCnt = OM.DevInfo.iTRAY_PcktCntX;
                    iTrayRowCnt = OM.DevInfo.iTRAY_PcktCntY;

                    int iGetWSTGWidth = pbTRAY.Width;
                    int iGetWSTGHeight = pbTRAY.Height;

                    double iSetWSTGWidth = 0, iSetWSTGHeight = 0;

                    double uWSTGGw = (double)iGetWSTGWidth  / (double)(iTrayColCnt);
                    double uWSTGGh = (double)iGetWSTGHeight / (double)(iTrayRowCnt);
                    double dWSTGWOff = (double)(iGetWSTGWidth - uWSTGGw * (iTrayColCnt)) / 2.0;
                    double dWSTGHOff = (double)(iGetWSTGHeight - uWSTGGh * (iTrayRowCnt)) / 2.0;

                    Pen.Color = Color.Black;

                    Brush.Color = Color.HotPink;


                    for (int r = 0; r < iTrayRowCnt; r++)
                    {
                        for (int c = 0; c < iTrayColCnt; c++)
                        {

                            dY1 = dWSTGHOff + r * uWSTGGh - 1;
                            dY2 = dWSTGHOff + r * uWSTGGh + uWSTGGh;
                            dX1 = dWSTGWOff + c * uWSTGGw - 1;
                            dX2 = dWSTGWOff + c * uWSTGGw + uWSTGGw;

                            g.FillRectangle(Brush, (float)dX1, (float)dY1, (float)dX2, (float)dY2);
                            g.DrawRectangle(Pen, (float)dX1, (float)dY1, (float)dX2, (float)dY2);

                            iSetWSTGWidth += dY2;
                            iSetWSTGHeight += dX2;
                        }

                    }

                    break;


            }
        }

        private void pbVSNInsp_Paint(object sender, PaintEventArgs e)
        {
            
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

        private void pbStackTray_Paint(object sender, PaintEventArgs e)
        {
            SolidBrush Brush = new SolidBrush(Color.Black);

            Pen Pen = new Pen(Color.Black);

            Graphics gSTG = pbStackTray.CreateGraphics();

            double dX1, dX2, dY1, dY2;

            int iTrayColCnt, iTrayRowCnt;

            Graphics g = e.Graphics;

            //switch (iTag)
            //{
            //    default: break;
            //    case 1:
                    iTrayColCnt = 1;
                    iTrayRowCnt = OM.DevInfo.iTRAY_StackingCnt;

                    int iGetWSTGWidth = pbStackTray.Width;
                    int iGetWSTGHeight = pbStackTray.Height;

                    double iSetWSTGWidth = 0, iSetWSTGHeight = 0;

                    double uWSTGGw = (double)iGetWSTGWidth / (double)(iTrayColCnt);
                    double uWSTGGh = (double)iGetWSTGHeight / (double)(iTrayRowCnt);
                    double dWSTGWOff = (double)(iGetWSTGWidth - uWSTGGw * (iTrayColCnt)) / 2.0;
                    double dWSTGHOff = (double)(iGetWSTGHeight - uWSTGGh * (iTrayRowCnt)) / 2.0;

                    Pen.Color = Color.Black;

                    Brush.Color = Color.HotPink;


                    for (int r = 0; r < iTrayRowCnt; r++)
                    {
                        for (int c = 0; c <= iTrayColCnt; c++)
                        {

                            dY1 = dWSTGHOff + r * uWSTGGh - 1;
                            dY2 = dWSTGHOff + r * uWSTGGh + uWSTGGh;
                            dX1 = dWSTGWOff + c * uWSTGGw - 1;
                            dX2 = dWSTGWOff + c * uWSTGGw + uWSTGGw;

                            g.FillRectangle(Brush, (float)dX1, (float)dY1, (float)dX2, (float)dY2);
                            g.DrawRectangle(Pen, (float)dX1, (float)dY1, (float)dX2, (float)dY2);

                            iSetWSTGWidth += dY2;
                            iSetWSTGHeight += dX2;
                        }

                    }

            //        break;
            //
            //}
        }

        private void btManual1_Click(object sender, EventArgs e)
        {
            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);
            MM.SetManCycle((mc)iBtnTag);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }
    }    
}
