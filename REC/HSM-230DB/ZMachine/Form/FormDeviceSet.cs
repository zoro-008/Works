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
using System.Drawing.Drawing2D;
using Microsoft.VisualBasic;
using System.Threading;

namespace Machine
{
    public partial class FormDeviceSet : Form
    {
        //public static FormDeviceSet       FrmDeviceSet ;
        public        FraMotr    []       FraMotr      ;
        public        FraCylOneBt[]       FraCylinder  ;
        public        FraOutput  []       FraOutput    ;
        public        FormMain            FrmMain      ;
        //public        Pattern             PTT  = new Pattern();
        //public        RS232_SuperSigmaCM2 Disp = new RS232_SuperSigmaCM2(1, "Dispensor");

        //FrameMotr 폼 갯수
        //const int FRAMEMOTR_COUNT = 30;
        //const int CYLINDER_COUNT  = 6;
        //const int OUTPUT_COUNT    = 16;
        
        //CPstnMan PstnCnt;

        public bool bInit = false;
        public  FormDeviceSet(Panel _pnBase)
        {
            InitializeComponent();
            
//            InitNodePosView();
            
            this.Width = 1272;
            this.Height = 866;

            this.TopLevel = false;
            this.Parent = _pnBase;

            // 이미지 초기화
            Point SttPnt = new Point(0,0);
            pbDieAttach.Location = SttPnt ;
            pbDieAttach.Width  = PANEL_DRAW_WIDTH ;
            pbDieAttach.Height = PANEL_DRAW_WIDTH ;

            tbUserUnit.Text = 0.01.ToString();
            PstnDisp();
            SEQ.DispPtrn.InitNodePosView(pnDisprNode,pbDieAttach);
            SEQ.HghtPtrn.InitHghtPosView(pnHghtNode ,pbDieAttach);
            SEQ.BltPtrn .InitNodePosView(pnBLTNode  ,pbDieAttach);
            UpdatePattern(true);

            

            //모터 축에 대한 포지션 디스플레이
            PM.SetWindow(pnMotrPos0 , (int)mi.WLDR_ZElev);
            PM.SetWindow(pnMotrPos1 , (int)mi.SLDR_ZElev);
            PM.SetWindow(pnMotrPos2 , (int)mi.WSTG_YGrpr);
            PM.SetWindow(pnMotrPos3 , (int)mi.SSTG_YGrpr);
            PM.SetWindow(pnMotrPos4 , (int)mi.WSTG_TTble);
            PM.SetWindow(pnMotrPos5 , (int)mi.TOOL_YEjtr);
            PM.SetWindow(pnMotrPos6 , (int)mi.TOOL_ZEjtr);
            PM.SetWindow(pnMotrPos7 , (int)mi.TOOL_ZPckr);
            PM.SetWindow(pnMotrPos8 , (int)mi.TOOL_YGent);
            //PM.SetWindow(pnMotrPos9 , (int)mi.TOOL_YRsub);
            PM.SetWindow(pnMotrPos10, (int)mi.TOOL_XRght);
            PM.SetWindow(pnMotrPos11, (int)mi.TOOL_ZDisp);
            PM.SetWindow(pnMotrPos12, (int)mi.TOOL_XLeft);
            PM.SetWindow(pnMotrPos13, (int)mi.TOOL_XEjtL);
            PM.SetWindow(pnMotrPos14, (int)mi.TOOL_XEjtR);
            PM.SetWindow(pnMotrPos15, (int)mi.TOOL_YVisn);
            PM.SetWindow(pnMotrPos16, (int)mi.SSTG_XRail);
            //PM.SetWindow(pnMotrPos17, (int)mi.Spare17   );
            PM.SetWindow(pnMotrPos18, (int)mi.WSTG_ZExpd);
            PM.SetWindow(pnMotrPos19, (int)mi.SSTG_ZRail);
            PM.SetWindow(pnMotrPos20, (int)mi.SSTG_YLeft);
            PM.SetWindow(pnMotrPos21, (int)mi.SSTG_YRght);
            PM.SetWindow(pnMotrPos22, (int)mi.SSTG_XFrnt);
            PM.SetWindow(pnMotrPos23, (int)mi.TOOL_ZVisn);
            

            PM.SetGetCmdPos((int)mi.WLDR_ZElev, SML.MT.GetCmdPos);
            PM.SetGetCmdPos((int)mi.SLDR_ZElev, SML.MT.GetCmdPos);
            PM.SetGetCmdPos((int)mi.WSTG_YGrpr, SML.MT.GetCmdPos);
            PM.SetGetCmdPos((int)mi.SSTG_YGrpr, SML.MT.GetCmdPos);
            PM.SetGetCmdPos((int)mi.WSTG_TTble, SML.MT.GetCmdPos);
            PM.SetGetCmdPos((int)mi.TOOL_YEjtr, SML.MT.GetCmdPos);
            PM.SetGetCmdPos((int)mi.TOOL_ZEjtr, SML.MT.GetCmdPos);
            PM.SetGetCmdPos((int)mi.TOOL_ZPckr, SML.MT.GetCmdPos);
            PM.SetGetCmdPos((int)mi.TOOL_YGent, SML.MT.GetCmdPos);
            PM.SetGetCmdPos((int)mi.TOOL_YRsub, SML.MT.GetCmdPos);
            PM.SetGetCmdPos((int)mi.TOOL_XRght, SML.MT.GetCmdPos);
            PM.SetGetCmdPos((int)mi.TOOL_ZDisp, SML.MT.GetCmdPos);
            PM.SetGetCmdPos((int)mi.TOOL_XLeft, SML.MT.GetCmdPos);
            PM.SetGetCmdPos((int)mi.TOOL_XEjtL, SML.MT.GetCmdPos);
            PM.SetGetCmdPos((int)mi.TOOL_XEjtR, SML.MT.GetCmdPos);
            PM.SetGetCmdPos((int)mi.TOOL_YVisn, SML.MT.GetCmdPos);
            PM.SetGetCmdPos((int)mi.SSTG_XRail, SML.MT.GetCmdPos);
            //PM.SetGetCmdPos((int)mi.Spare17   , SM.MT.GetCmdPos);
            PM.SetGetCmdPos((int)mi.WSTG_ZExpd, SML.MT.GetCmdPos);
            PM.SetGetCmdPos((int)mi.SSTG_ZRail, SML.MT.GetCmdPos);
            PM.SetGetCmdPos((int)mi.SSTG_YLeft, SML.MT.GetCmdPos);
            PM.SetGetCmdPos((int)mi.SSTG_YRght, SML.MT.GetCmdPos);
            PM.SetGetCmdPos((int)mi.SSTG_XFrnt, SML.MT.GetCmdPos);
            PM.SetGetCmdPos((int)mi.TOOL_ZVisn, SML.MT.GetCmdPos);
            

            OM.LoadLastInfo();
            PM.Load(OM.GetCrntDev().ToString());

            PM.UpdatePstn(true);
            UpdateDevInfo(true);
            UpdateDevOptn(true);
//            UpdateNodePos(true);


            FraMotr     = new FraMotr    [(int)mi.MAX_MOTR  ];
            FraCylinder = new FraCylOneBt[(int)ci.MAX_ACTR  ];
            //FraOutput   = new FraOutput  [(int)yi.MAX_OUTPUT];

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

            //겐트리 서브축은 안보이게.
            FraMotr[(int)mi.TOOL_YRsub].Visible = false;

            //실린더 수에 맞춰 FrameCylinder 생성
            for (int i = 0; i < (int)ci.MAX_ACTR; i++)
            {
                Control[] Ctrl          = tcDeviceSet.Controls.Find("pnActr" + i.ToString(), true);

                FraCylinder[i]          = new FraCylOneBt();
                FraCylinder[i].TopLevel = false;

                switch (i)
                {
                    default                               : break;
                    case (int)ci.TOOL_GuidFtDwUp     : FraCylinder[i].SetConfig((ci)i, SML.CL.GetName(i).ToString(), SML.CL.GetDirType(i), Ctrl[0]); break;
                    case (int)ci.TOOL_GuidRrDwUp     : FraCylinder[i].SetConfig((ci)i, SML.CL.GetName(i).ToString(), SML.CL.GetDirType(i), Ctrl[0]); break;
                    case (int)ci.TOOL_DispCvFwBw     : FraCylinder[i].SetConfig((ci)i, SML.CL.GetName(i).ToString(), SML.CL.GetDirType(i), Ctrl[0]); break;
                    case (int)ci.WSTG_RailCvsLtFwBw  : FraCylinder[i].SetConfig((ci)i, SML.CL.GetName(i).ToString(), SML.CL.GetDirType(i), Ctrl[0]); break;
                    case (int)ci.WSTG_RailCvsRtFwBw  : FraCylinder[i].SetConfig((ci)i, SML.CL.GetName(i).ToString(), SML.CL.GetDirType(i), Ctrl[0]); break;
                    case (int)ci.WSTG_GrprClOp       : FraCylinder[i].SetConfig((ci)i, SML.CL.GetName(i).ToString(), SML.CL.GetDirType(i), Ctrl[0]); break;
                    case (int)ci.SSTG_GrprClOp       : FraCylinder[i].SetConfig((ci)i, SML.CL.GetName(i).ToString(), SML.CL.GetDirType(i), Ctrl[0]); break;
                    case (int)ci.SSTG_BoatClampClOp  : FraCylinder[i].SetConfig((ci)i, SML.CL.GetName(i).ToString(), SML.CL.GetDirType(i), Ctrl[0]); break;
                }

                FraCylinder[i].Show();
            }

            //Output 버튼 생성
            const int iOutputBtnCnt = 3;
            FraOutput   = new FraOutput  [iOutputBtnCnt];
            for (int i = 0; i < iOutputBtnCnt; i++)
            {
                Control[] Ctrl = tcDeviceSet.Controls.Find("pnIO" + i.ToString(), true);
                
                int iIOCtrl = Convert.ToInt32(Ctrl[0].Tag);
                
                FraOutput[i] = new FraOutput();
                FraOutput[i].TopLevel = false;

                switch (iIOCtrl)
                {  
                    default : break;
                    case (int)yi.TOOL_PckrVac   : FraOutput[i].SetConfig(yi.TOOL_PckrVac   , SML.IO.GetYName((int)yi.TOOL_PckrVac  )  , Ctrl[0]); break;
                    case (int)yi.SSTG_SubsVac   : FraOutput[i].SetConfig(yi.SSTG_SubsVac   , SML.IO.GetYName((int)yi.SSTG_SubsVac  )  , Ctrl[0]); break;
                    case (int)yi.TOOL_DispOnOff : FraOutput[i].SetConfig(yi.TOOL_DispOnOff , SML.IO.GetYName((int)yi.TOOL_DispOnOff)  , Ctrl[0]); break;
                    //case (int)yi.PCK_Vcc4    : FraOutput[i].SetConfig(yi.PCK_Vcc4   , SM.IO.GetYName((int)yi.PCK_Vcc4  )  , Ctrl[0]); break;
                    //case (int)yi.PCK_Vcc5    : FraOutput[i].SetConfig(yi.PCK_Vcc5   , SM.IO.GetYName((int)yi.PCK_Vcc5  )  , Ctrl[0]); break;
                    //case (int)yi.PCK_Vcc6    : FraOutput[i].SetConfig(yi.PCK_Vcc6   , SM.IO.GetYName((int)yi.PCK_Vcc6  )  , Ctrl[0]); break;
                    //case (int)yi.PCK_Vcc7    : FraOutput[i].SetConfig(yi.PCK_Vcc7   , SM.IO.GetYName((int)yi.PCK_Vcc7  )  , Ctrl[0]); break;
                    //case (int)yi.PCK_Vcc8    : FraOutput[i].SetConfig(yi.PCK_Vcc8   , SM.IO.GetYName((int)yi.PCK_Vcc8  )  , Ctrl[0]); break;
                    //case (int)yi.PCK_Ejt1    : FraOutput[i].SetConfig(yi.PCK_Ejt1   , SM.IO.GetYName((int)yi.PCK_Ejt1  )  , Ctrl[0]); break;
                    //case (int)yi.PCK_Ejt2    : FraOutput[i].SetConfig(yi.PCK_Ejt2   , SM.IO.GetYName((int)yi.PCK_Ejt2  )  , Ctrl[0]); break;
                    //case (int)yi.PCK_Ejt3    : FraOutput[i].SetConfig(yi.PCK_Ejt3   , SM.IO.GetYName((int)yi.PCK_Ejt3  )  , Ctrl[0]); break;
                    //case (int)yi.PCK_Ejt4    : FraOutput[i].SetConfig(yi.PCK_Ejt4   , SM.IO.GetYName((int)yi.PCK_Ejt4  )  , Ctrl[0]); break;
                    //case (int)yi.PCK_Ejt5    : FraOutput[i].SetConfig(yi.PCK_Ejt5   , SM.IO.GetYName((int)yi.PCK_Ejt5  )  , Ctrl[0]); break;
                    //case (int)yi.PCK_Ejt6    : FraOutput[i].SetConfig(yi.PCK_Ejt6   , SM.IO.GetYName((int)yi.PCK_Ejt6  )  , Ctrl[0]); break;
                    //case (int)yi.PCK_Ejt7    : FraOutput[i].SetConfig(yi.PCK_Ejt7   , SM.IO.GetYName((int)yi.PCK_Ejt7  )  , Ctrl[0]); break;
                    //case (int)yi.PCK_Ejt8    : FraOutput[i].SetConfig(yi.PCK_Ejt8   , SM.IO.GetYName((int)yi.PCK_Ejt8  )  , Ctrl[0]); break;
                }
              
                FraOutput[i].Show();
            }
            bInit = true;
        }

        public void PstnDisp()
        {
            //WLDR_ZElev                                 
            PM.SetProp((uint)mi.WLDR_ZElev, (uint)pv.WLDR_ZElevWait           , "Wait             ", false, false, false);
            PM.SetProp((uint)mi.WLDR_ZElev, (uint)pv.WLDR_ZElevTopWorkStt     , "Top Work Stt     ", false, false, false);
            PM.SetProp((uint)mi.WLDR_ZElev, (uint)pv.WLDR_ZElevBtmWorkStt     , "Bottom Work Stt  ", false, false, false);
                                                                                                  
            //SLDR_ZElev                                                                          
            PM.SetProp((uint)mi.SLDR_ZElev, (uint)pv.SLDR_ZElevWait           , "Wait             ", false, false, false);
            PM.SetProp((uint)mi.SLDR_ZElev, (uint)pv.SLDR_ZElevTopWorkStt     , "Top Work Stt     ", false, false, false);
            PM.SetProp((uint)mi.SLDR_ZElev, (uint)pv.SLDR_ZElevBtmWorkStt     , "Bottom Work Stt  ", false, false, false);
                                                                                                  
            //WSTG_YGrpr                                                                          
            PM.SetProp((uint)mi.WSTG_YGrpr, (uint)pv.WSTG_YGrprWait           , "Wait             ", false, false, false);
            PM.SetProp((uint)mi.WSTG_YGrpr, (uint)pv.WSTG_YGrprPick           , "Pick             ", false, false, false);
            PM.SetProp((uint)mi.WSTG_YGrpr, (uint)pv.WSTG_YGrprPickWait       , "Pick Wait        ", false, false, false);
            PM.SetProp((uint)mi.WSTG_YGrpr, (uint)pv.WSTG_YGrprBarcode        , "Barcode          ", false, false, false);
            PM.SetProp((uint)mi.WSTG_YGrpr, (uint)pv.WSTG_YGrprPlace          , "Place            ", false, false, false);
                                                                                                  
            //SSTG_YGrpr                                                                          
            PM.SetProp((uint)mi.SSTG_YGrpr, (uint)pv.SSTG_YGrprWait           , "Wait             ", false, false, false);
            PM.SetProp((uint)mi.SSTG_YGrpr, (uint)pv.SSTG_YGrprPick           , "Pick             ", false, false, false);
            PM.SetProp((uint)mi.SSTG_YGrpr, (uint)pv.SSTG_YGrprPickWait       , "Pick Wait        ", false, false, false);
            PM.SetProp((uint)mi.SSTG_YGrpr, (uint)pv.SSTG_YGrprBarcode        , "Barcode          ", false, false, false);
            PM.SetProp((uint)mi.SSTG_YGrpr, (uint)pv.SSTG_YGrprWorkStt        , "Work Stt         ", false, false, false);
                                                                                                  
            //WSTG_TTble                                                                          
            PM.SetProp((uint)mi.WSTG_TTble, (uint)pv.WSTG_TTbleWait           , "Wait             ", false, false, false);
            PM.SetProp((uint)mi.WSTG_TTble, (uint)pv.WSTG_TTbleWork           , "Work             ", false, false, false);
            PM.SetProp((uint)mi.WSTG_TTble, (uint)pv.WSTG_TTbleWfrWork        , "Wafer Work       ", false, false, false);
                                                                                                  
            //WSTG_YEjtr                                                                          
            PM.SetProp((uint)mi.TOOL_YEjtr, (uint)pv.TOOL_YEjtrWait           , "Wait             ", false, false, false);
            PM.SetProp((uint)mi.TOOL_YEjtr, (uint)pv.TOOL_YEjtrWorkStt        , "Work Stt         ", false, false, false);
                                                                                                  
            //WSTG_ZEjtr                                                                          
            PM.SetProp((uint)mi.TOOL_ZEjtr, (uint)pv.TOOL_ZEjtrWait           , "Wait             ", false, false, false);
            PM.SetProp((uint)mi.TOOL_ZEjtr, (uint)pv.TOOL_ZEjtrWork           , "Work             ", false, false, false);
                                                                                                  
            //TOOL_ZPckr                                                                          
            PM.SetProp((uint)mi.TOOL_ZPckr, (uint)pv.TOOL_ZPckrWait           , "Wait             ", false, false, true );
            PM.SetProp((uint)mi.TOOL_ZPckr, (uint)pv.TOOL_ZPckrPick           , "Pick             ", false, false, false);
            PM.SetProp((uint)mi.TOOL_ZPckr, (uint)pv.TOOL_ZPckrBVisn          , "Bottom Visn      ", false, false, false);
            PM.SetProp((uint)mi.TOOL_ZPckr, (uint)pv.TOOL_ZPckrPlce           , "Place            ", false, false, false);
            PM.SetProp((uint)mi.TOOL_ZPckr, (uint)pv.TOOL_ZPckrCheck          , "Check            ", false, false, false);
                                                                                                  
            //TOOL_YLeft                                                                          
            PM.SetProp((uint)mi.TOOL_YGent, (uint)pv.TOOL_YGentWait           , "Wait             ", false, false, true );
            PM.SetProp((uint)mi.TOOL_YGent, (uint)pv.TOOL_YGentPkPickStt      , "Picker Pick Stt  ", false, false, false);
            PM.SetProp((uint)mi.TOOL_YGent, (uint)pv.TOOL_YGentPkBVsnM        , "Picker Btm Visn M", false, false, false);
            PM.SetProp((uint)mi.TOOL_YGent, (uint)pv.TOOL_YGentPkBVsnS        , "Picker Btm Visn S", false, false, false);
            PM.SetProp((uint)mi.TOOL_YGent, (uint)pv.TOOL_YGentPkPlce         , "Picker Place     ", false, false, false);
            PM.SetProp((uint)mi.TOOL_YGent, (uint)pv.TOOL_YGentVsWStgVsnMStt  , "Visn WStg M Stt  ", false, false, false);
            PM.SetProp((uint)mi.TOOL_YGent, (uint)pv.TOOL_YGentVsWStgVsnSStt  , "Visn WStg S Stt  ", false, false, false);
            PM.SetProp((uint)mi.TOOL_YGent, (uint)pv.TOOL_YGentDispStt        , "Dispensor Stt    ", false, false, false);
            PM.SetProp((uint)mi.TOOL_YGent, (uint)pv.TOOL_YGentHghtStt        , "Height Stt       ", false, false, false);
            PM.SetProp((uint)mi.TOOL_YGent, (uint)pv.TOOL_YGentBltStt         , "Blt Stt          ", false, false, false);
            PM.SetProp((uint)mi.TOOL_YGent, (uint)pv.TOOL_YGentDispVisn1      , "Dispense Vision1 ", false, false, false);
            PM.SetProp((uint)mi.TOOL_YGent, (uint)pv.TOOL_YGentDispVisn2      , "Dispense Vision2 ", false, false, false);
            PM.SetProp((uint)mi.TOOL_YGent, (uint)pv.TOOL_YGentTVsnCheck      , "Top Vision Check ", false, false, true );
            PM.SetProp((uint)mi.TOOL_YGent, (uint)pv.TOOL_YGentDispCheck      , "Dispensor Check  ", false, false, true );
            PM.SetProp((uint)mi.TOOL_YGent, (uint)pv.TOOL_YGentHghtCheck      , "Height Check     ", false, false, true );
            PM.SetProp((uint)mi.TOOL_YGent, (uint)pv.TOOL_YGentPckrCheck      , "Picker Check     ", false, false, false);
            PM.SetProp((uint)mi.TOOL_YGent, (uint)pv.TOOL_YGentVsWStgCtr      , "Visn WStg Ctr    ", false, false, false);
            PM.SetProp((uint)mi.TOOL_YGent, (uint)pv.TOOL_YGentVsSStgCtr      , "Visn SStg Ctr    ", false, false, false);
            PM.SetProp((uint)mi.TOOL_YGent, (uint)pv.TOOL_YGentConv           , "Picker Conversion", false, false, true );
                                                                                                  
            //TOOL_YRght                                                                          
            PM.SetProp((uint)mi.TOOL_YRsub, (uint)pv.TOOL_YRghtWait           , "Wait             ", false, false, false);
                                                                                                  
            //TOOL_XRght                                                                          
            PM.SetProp((uint)mi.TOOL_XRght, (uint)pv.TOOL_XRghtWait           , "Wait             ", false, false, true );
            PM.SetProp((uint)mi.TOOL_XRght, (uint)pv.TOOL_XRghtVsSStgVsnRtM   , "Visn SStg Rt M   ", false, false, false);
            PM.SetProp((uint)mi.TOOL_XRght, (uint)pv.TOOL_XRghtVsSStgVsnRtS   , "Visn SStg Rt S   ", false, false, false);
            PM.SetProp((uint)mi.TOOL_XRght, (uint)pv.TOOL_XRghtVsSStgVsnLtM   , "Visn SStg Lt M   ", false, false, false);
            PM.SetProp((uint)mi.TOOL_XRght, (uint)pv.TOOL_XRghtVsSStgVsnLtS   , "Visn SStg Lt S   ", false, false, false);
            PM.SetProp((uint)mi.TOOL_XRght, (uint)pv.TOOL_XRghtVsWStgVsnMStt  , "Visn WStg M Stt  ", false, false, false);
            PM.SetProp((uint)mi.TOOL_XRght, (uint)pv.TOOL_XRghtVsWStgVsnSStt  , "Visn WStg S Stt  ", false, false, false);
            PM.SetProp((uint)mi.TOOL_XRght, (uint)pv.TOOL_XRghtDispStt        , "Dispensor Stt    ", false, false, false);
            PM.SetProp((uint)mi.TOOL_XRght, (uint)pv.TOOL_XRghtHghtStt        , "Height Stt       ", false, false, false);
            PM.SetProp((uint)mi.TOOL_XRght, (uint)pv.TOOL_XRghtBltStt         , "Blt Stt          ", false, false, false);
            PM.SetProp((uint)mi.TOOL_XRght, (uint)pv.TOOL_XRghtDispVisn1      , "Dispense Vision1 ", false, false, false);
            PM.SetProp((uint)mi.TOOL_XRght, (uint)pv.TOOL_XRghtDispVisn2      , "Dispense Vision2 ", false, false, false);
            PM.SetProp((uint)mi.TOOL_XRght, (uint)pv.TOOL_XRghtTVsnCheck      , "Top Vision Check ", false, false, true );
            PM.SetProp((uint)mi.TOOL_XRght, (uint)pv.TOOL_XRghtDispCheck      , "Dispensor Check  ", false, false, true );
            PM.SetProp((uint)mi.TOOL_XRght, (uint)pv.TOOL_XRghtHghtCheck      , "Height Check     ", false, false, true );
            PM.SetProp((uint)mi.TOOL_XRght, (uint)pv.TOOL_XRghtVsWStgCtr      , "Visn WStg Ctr    ", false, false, false);
            PM.SetProp((uint)mi.TOOL_XRght, (uint)pv.TOOL_XRghtVsSStgCtr      , "Visn SStg Ctr    ", false, false, false);

                                                                                                  
            //TOOL_ZDisp                                                                          
            PM.SetProp((uint)mi.TOOL_ZDisp, (uint)pv.TOOL_ZDispWait           , "Wait             ", false, false, true );
            PM.SetProp((uint)mi.TOOL_ZDisp, (uint)pv.TOOL_ZDispMove           , "Move             ", false, false, true );
            PM.SetProp((uint)mi.TOOL_ZDisp, (uint)pv.TOOL_ZDispWork           , "Work             ", false, false, false);
            PM.SetProp((uint)mi.TOOL_ZDisp, (uint)pv.TOOL_ZDispCheck          , "Check            ", false, false, true );//61.565
                                                                                                  
            //TOOL_XLeft                                                                          
            PM.SetProp((uint)mi.TOOL_XLeft, (uint)pv.TOOL_XLeftWait           , "Pckr Wait        ", false, false, false);
            PM.SetProp((uint)mi.TOOL_XLeft, (uint)pv.TOOL_XLeftPkPickStt      , "Pckr Pick Stt    ", false, false, false);
            PM.SetProp((uint)mi.TOOL_XLeft, (uint)pv.TOOL_XLeftPkBVsnM        , "Pckr Btm Visn M  ", false, false, false);
            PM.SetProp((uint)mi.TOOL_XLeft, (uint)pv.TOOL_XLeftPkBVsnS        , "Pckr Btm Visn S  ", false, false, false);
            PM.SetProp((uint)mi.TOOL_XLeft, (uint)pv.TOOL_XLeftPkPlce         , "Pckr Place       ", false, false, false);
            PM.SetProp((uint)mi.TOOL_XLeft, (uint)pv.TOOL_XLeftPkCheck        , "Pckr Check       ", false, false, false);
            PM.SetProp((uint)mi.TOOL_XLeft, (uint)pv.TOOL_XLeftConv           , "Pckr Conversion  ", false, false, true );
                                                                                                  
            //WSTG_XEjtL                                                                          
            PM.SetProp((uint)mi.TOOL_XEjtL, (uint)pv.WSTG_XEjtLWait           , "Wait             ", false, false, false);
            PM.SetProp((uint)mi.TOOL_XEjtL, (uint)pv.WSTG_XEjtLStart          , "Start            ", false, false, false);
            PM.SetProp((uint)mi.TOOL_XEjtL, (uint)pv.WSTG_XEjtLEnd            , "End              ", false, false, false);
                                                                                                  
            //WSTG_XEjtR                                                                          
            PM.SetProp((uint)mi.TOOL_XEjtR, (uint)pv.WSTG_XEjtRWait           , "Wait             ", false, false, false);
            PM.SetProp((uint)mi.TOOL_XEjtR, (uint)pv.WSTG_XEjtRStart          , "Start            ", false, false, false);
            PM.SetProp((uint)mi.TOOL_XEjtR, (uint)pv.WSTG_XEjtREnd            , "End              ", false, false, false);
                                                                                                  
            //TOOL_YVisn                                                                          
            PM.SetProp((uint)mi.TOOL_YVisn, (uint)pv.TOOL_YVisnWait           , "Wait             ", false, false, true );
            PM.SetProp((uint)mi.TOOL_YVisn, (uint)pv.TOOL_YVisnWork           , "Work             ", false, false, false);
            PM.SetProp((uint)mi.TOOL_YVisn, (uint)pv.TOOL_YVisnSStgVsnRtM     , "SStg Visn Rt M   ", false, false, false);
            PM.SetProp((uint)mi.TOOL_YVisn, (uint)pv.TOOL_YVisnSStgVsnRtS     , "SStg Visn Rt S   ", false, false, false);
            PM.SetProp((uint)mi.TOOL_YVisn, (uint)pv.TOOL_YVisnSStgVsnLtM     , "SStg Visn Lt M   ", false, false, false);
            PM.SetProp((uint)mi.TOOL_YVisn, (uint)pv.TOOL_YVisnSStgVsnLtS     , "SStg Visn Lt S   ", false, false, false);
                                                                                                  
            //SSTG_XRail                                                                          
            PM.SetProp((uint)mi.SSTG_XRail, (uint)pv.SSTG_XRailWait           , "Wait             ", false, false, false);
            PM.SetProp((uint)mi.SSTG_XRail, (uint)pv.SSTG_XRailWork           , "Work             ", false, false, false);
                       
            //Spare17 -> 세이브할때 뻑나서 추가함. 진섭 
            PM.SetProp((uint)mi.Spare17   , (uint)pv.Spare17Wait              , "Wait             ", false, false, false);
                                                                                                  
            //WSTG_ZExpd                                                                          
            PM.SetProp((uint)mi.WSTG_ZExpd, (uint)pv.WSTG_ZExpdWait           , "Wait             ", false, false, false);
            PM.SetProp((uint)mi.WSTG_ZExpd, (uint)pv.WSTG_ZExpdWork           , "Work             ", false, false, false);
                                                                                                  
            //SSTG_ZRail                                                                          
            PM.SetProp((uint)mi.SSTG_ZRail, (uint)pv.SSTG_ZRailWait           , "Wait             ", false, false, false);
            PM.SetProp((uint)mi.SSTG_ZRail, (uint)pv.SSTG_ZRailDown           , "Down             ", false, false, false);
                                                                                                  
            //SSTG_YLeft                                                                          
            PM.SetProp((uint)mi.SSTG_YLeft, (uint)pv.SSTG_YLeftWait           , "Wait             ", false, false, false);
                                                                                                  
            //SSTG_YRght                                                                          
            PM.SetProp((uint)mi.SSTG_YRght, (uint)pv.SSTG_YRghtWait           , "Wait             ", false, false, false);
                                                                                                  
            //SSTG_XFrnt                                                                          
            PM.SetProp((uint)mi.SSTG_XFrnt, (uint)pv.SSTG_XFrntWait           , "Wait             ", false, false, false);
                                                                                                  
            //TOOL_ZVisn                                                                          
            PM.SetProp((uint)mi.TOOL_ZVisn, (uint)pv.TOOL_ZVisnWait           , "Wait             ", false, false, false);
            PM.SetProp((uint)mi.TOOL_ZVisn, (uint)pv.TOOL_ZVisnSStgWfrWork    , "SStg Wfr Work    ", false, false, false);
            PM.SetProp((uint)mi.TOOL_ZVisn, (uint)pv.TOOL_ZVisnSStgSbsWork    , "SStg Sbs Work    ", false, false, false);
            PM.SetProp((uint)mi.TOOL_ZVisn, (uint)pv.TOOL_ZVisnSStgSbsEndWork , "SStg Sbs End Work", false, false, false);
            PM.SetProp((uint)mi.TOOL_ZVisn, (uint)pv.TOOL_ZVisnWStgWork       , "WStg Work        ", false, false, false);

            
        }

        private void tcDeviceSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            int iSeletedIndex;
            iSeletedIndex = tcDeviceSet.SelectedIndex;
            
            switch (iSeletedIndex)
            {
                default : break;
                case 2  : gbJogUnit.Parent = pnJog1;                       break;
                case 3  : gbJogUnit.Parent = pnJog2;                       break;
                case 4  : gbJogUnit.Parent = pnJog3;             
                          FraMotr[8].Parent = pnMotrJog8;                  
                          PM.SetWindow(pnMotrPos8   , (int)mi.TOOL_YGent); break;
                case 5  : gbJogUnit.Parent = pnJog4; 
                          FraMotr[8].Parent = pnMotrJog8_1;
                          PM.SetWindow(pnMotrPos8_1 , (int)mi.TOOL_YGent); break;

            }
            
            PM.Load(OM.GetCrntDev());
            OM.LoadDevInfo(OM.GetCrntDev());
            OM.LoadDevOptn(OM.GetCrntDev());



            UpdatePattern(true);

            UpdateDevInfo(true);
            UpdateDevOptn(true);

            PM.UpdatePstn(true);
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

            UpdatePattern(false);
            SEQ.DispPtrn.Save(OM.GetCrntDev());
            SEQ.HghtPtrn.Save(OM.GetCrntDev());
            SEQ.BltPtrn .Save(OM.GetCrntDev());
            SEQ.DispPtrn.SavePttColor(OM.GetCrntDev());
            SEQ.HghtPtrn.SavePttColor(OM.GetCrntDev());

            pbWLDR.Refresh();
            pbSLDR.Refresh();
            pbWSTG.Refresh();
            pbSSTG.Refresh();

            OM.SaveEqpOptn();

            DM.ARAY[(int)ri.WLDT].SetMaxColRow(1                       , OM.DevInfo.iWLDR_SlotCnt);
            DM.ARAY[(int)ri.WLDB].SetMaxColRow(1                       , OM.DevInfo.iWLDR_SlotCnt);
            DM.ARAY[(int)ri.SLDT].SetMaxColRow(1                       , OM.DevInfo.iSLDR_SlotCnt);
            DM.ARAY[(int)ri.SLDB].SetMaxColRow(1                       , OM.DevInfo.iSLDR_SlotCnt);
            DM.ARAY[(int)ri.WSTG].SetMaxColRow(OM.DevInfo.iWFER_DieCntX, OM.DevInfo.iWFER_DieCntY);
            DM.ARAY[(int)ri.SSTG].SetMaxColRow(1                       , OM.DevInfo.iSBOT_PcktCnt);
            DM.ARAY[(int)ri.PCKR].SetMaxColRow(1                       , 1                       );

            Refresh();
        }

        public void UpdateDevInfo(bool bToTable)
        {
            if (bToTable)
            {
                tbWLDR_SlotCnt  .Text     = OM.DevInfo.iWLDR_SlotCnt  .ToString();
                tbWLDR_SlotPitch.Text     = OM.DevInfo.dWLDR_SlotPitch.ToString();
                tbWFER_DieCntX  .Text     = OM.DevInfo.iWFER_DieCntX  .ToString();
                tbWFER_DieCntY  .Text     = OM.DevInfo.iWFER_DieCntY  .ToString();
                tbWFER_DiePitchX.Text     = OM.DevInfo.dWFER_DiePitchX.ToString();
                tbWFER_DiePitchY.Text     = OM.DevInfo.dWFER_DiePitchY.ToString();

                                          
                tbSLDR_SlotCnt  .Text     = OM.DevInfo.iSLDR_SlotCnt  .ToString();
                tbSLDR_SlotPitch.Text     = OM.DevInfo.dSLDR_SlotPitch.ToString();
                tbSBOT_PcktCnt  .Text     = OM.DevInfo.iSBOT_PcktCnt  .ToString();
                tbSBOT_PcktPitch.Text     = OM.DevInfo.dSBOT_PcktPitch.ToString();

                cbWaferSize.SelectedIndex = OM.DevInfo.iWaferSize;        //콤보박스     
                                          
                tbDieWidth      .Text     = OM.DevInfo.dDieWidth      .ToString();
                tbDieHeight     .Text     = OM.DevInfo.dDieHeight     .ToString();
                                          
                tbSubWidth      .Text     = OM.DevInfo.dSubWidth      .ToString();
                tbSubHeight     .Text     = OM.DevInfo.dSubHeight     .ToString();

                tbWFER_Tickness .Text     = OM.DevInfo.dWFER_Tickness .ToString();
                                          
                tbSubWidth      .Text     = OM.DevInfo.dSubWidth      .ToString();
                tbSubHeight     .Text     = OM.DevInfo.dSubHeight     .ToString();
                                                                      
                tbDieWidth      .Text     = OM.DevInfo.dDieWidth      .ToString();
                tbDieHeight     .Text     = OM.DevInfo.dDieHeight     .ToString();


               
                if (CConfig.StrToIntDef(tbSubWidth     .Text, 1) <= 0) { tbSubWidth     .Text = 1.ToString(); }
                if (CConfig.StrToIntDef(tbSubHeight    .Text, 1) <= 0) { tbSubHeight    .Text = 1.ToString(); }
                                                                                           
                if (CConfig.StrToIntDef(tbDieWidth     .Text, 1) <= 0) { tbDieWidth     .Text = 1.ToString(); }
                if (CConfig.StrToIntDef(tbDieHeight    .Text, 1) <= 0) { tbDieHeight    .Text = 1.ToString(); }

                if (CConfig.StrToIntDef(tbWLDR_SlotCnt .Text, 1) <= 0) { tbWLDR_SlotCnt .Text = 1.ToString(); }
                if (CConfig.StrToIntDef(tbSLDR_SlotCnt .Text, 1) <= 0) { tbSLDR_SlotCnt .Text = 1.ToString(); }

                if (CConfig.StrToIntDef(tbWFER_DieCntX  .Text, 1) <= 0) { tbWFER_DieCntX  .Text = 1.ToString(); }
                if (CConfig.StrToIntDef(tbSBOT_PcktCnt .Text, 1) <= 0) { tbSBOT_PcktCnt .Text = 1.ToString(); }
            }
            else 
            {
                OM.DevInfo.iWLDR_SlotCnt    = CConfig.StrToIntDef   (tbWLDR_SlotCnt  .Text, 1  );
                OM.DevInfo.dWLDR_SlotPitch  = CConfig.StrToDoubleDef(tbWLDR_SlotPitch.Text, 0.0);
                OM.DevInfo.iWFER_DieCntX    = CConfig.StrToIntDef   (tbWFER_DieCntX   .Text, 1  );
                OM.DevInfo.iWFER_DieCntY    = CConfig.StrToIntDef   (tbWFER_DieCntY   .Text, 1  );
                OM.DevInfo.dWFER_DiePitchX  = CConfig.StrToDoubleDef(tbWFER_DiePitchX .Text, 0.0);
                OM.DevInfo.dWFER_DiePitchY  = CConfig.StrToDoubleDef(tbWFER_DiePitchY .Text, 0.0);

                OM.DevInfo.iSLDR_SlotCnt   = CConfig.StrToIntDef   (tbSLDR_SlotCnt  .Text, 1  );
                OM.DevInfo.dSLDR_SlotPitch = CConfig.StrToDoubleDef(tbSLDR_SlotPitch.Text, 0.0);
                OM.DevInfo.iSBOT_PcktCnt   = CConfig.StrToIntDef   (tbSBOT_PcktCnt  .Text, 1  );
                OM.DevInfo.dSBOT_PcktPitch = CConfig.StrToDoubleDef(tbSBOT_PcktPitch.Text, 0.0);

                OM.DevInfo.iWaferSize      = cbWaferSize.SelectedIndex ;     

                OM.DevInfo.dDieWidth       = CConfig.StrToDoubleDef(tbDieWidth      .Text, 0.0);
                OM.DevInfo.dDieHeight      = CConfig.StrToDoubleDef(tbDieHeight     .Text, 0.0);

                OM.DevInfo.dSubWidth       = CConfig.StrToDoubleDef(tbSubWidth      .Text, 0.0);
                OM.DevInfo.dSubHeight      = CConfig.StrToDoubleDef(tbSubHeight     .Text, 0.0);

                OM.DevInfo.dWFER_Tickness  = CConfig.StrToDoubleDef(tbWFER_Tickness .Text, 0.0);

                OM.DevInfo.dSubWidth       = CConfig.StrToDoubleDef(tbSubWidth      .Text, 0.0);
                OM.DevInfo.dSubHeight      = CConfig.StrToDoubleDef(tbSubHeight     .Text, 0.0);

                OM.DevInfo.dDieWidth       = CConfig.StrToDoubleDef(tbDieWidth      .Text, 0.0);
                OM.DevInfo.dDieHeight      = CConfig.StrToDoubleDef(tbDieHeight     .Text, 0.0);
                
                UpdateDevInfo(true);
            }
        
        }

        public void UpdateDevOptn(bool bToTable)
        {

            if (bToTable)
            {
                //Pick Option
                tbPickDelay        .Text          = OM.DevOptn.iPickDelay        .ToString();  
                    
                //Vision Tolerance Option
                tbVisnTolXY        .Text          = OM.DevOptn.dVisnTolXY        .ToString();      
                tbVisnTolAng       .Text          = OM.DevOptn.dVisnTolAng       .ToString(); 
                tbRghtEndVisnTolXY .Text          = OM.DevOptn.dRghtEndVisnTolXY .ToString();    
                tbLeftEndVisnTolXY .Text          = OM.DevOptn.dLeftEndVisnTolXY .ToString();  
                                               
                tbShakeOffset      .Text          = OM.DevOptn.dShakeOffset      .ToString();    
                tbShakeRange       .Text          = OM.DevOptn.dShakeRange       .ToString();    
                tbAttachSpeed1     .Text          = OM.DevOptn.dAttachSpeed1     .ToString(); 
                tbAttachForce      .Text          = OM.DevOptn.dAttachForce      .ToString();
                tbAttachForceOfs   .Text          = OM.DevOptn.dAttachForceOfs   .ToString();
                tbAttachDelay      .Text          = OM.DevOptn.iAttachDelay      .ToString();
                tbAtAttachDelay    .Text          = OM.DevOptn.iAtAttachDelay    .ToString();
                                                                                 
                tbSStgTemp         .Text          = OM.DevOptn.iSStgTemp         .ToString();    
                
                //tbBoutClampOfs     .Text          = OM.DevOptn.dBoutClampOfs     .ToString();
                                                                                                
                //패턴 탭                                                        
                tbDieXOfs          .Text          = OM.DevOptn.dDieXOfs          .ToString();
                tbDieYOfs          .Text          = OM.DevOptn.dDieYOfs          .ToString();
                                                                                 
                tbMainVsnX         .Text          = OM.DevOptn.dMstVsnX          .ToString();
                tbMainVsnY         .Text          = OM.DevOptn.dMstVsnY          .ToString();
                tbSubVsnX          .Text          = OM.DevOptn.dSlvVsnX          .ToString();
                tbSubVsnY          .Text          = OM.DevOptn.dSlvVsnY          .ToString();
                                                                                 
                cbDspCh            .SelectedIndex = OM.DevOptn.iDspCh;           
                tbDspZOfs          .Text          = OM.DevOptn.dDspZOfs          .ToString();
                tbDspMinAmount     .Text          = OM.DevOptn.iDspMinAmount     .ToString();
                tbDispShotDelay    .Text          = OM.DevOptn.iDispShotDelay    .ToString();
                tbDispAtShotDelay  .Text          = OM.DevOptn.iDispAtShotDelay  .ToString();
                cdVsn.Color = Color.FromArgb(OM.DevOptn.iVsnRgb);
                cdSub.Color = Color.FromArgb(OM.DevOptn.iSubRgb);
                cdDie.Color = Color.FromArgb(OM.DevOptn.iDieRgb);

                tbToolCrashDist   .Text          = OM.DevOptn.dToolCrashDist     .ToString();

                tbUVWTOfs         .Text          = OM.DevOptn.dUVWTOfs           .ToString();

                tbEjectAtUpDelay  .Text          = OM.DevOptn.iEjectAtUpDelay    .ToString();
                tbEjectSpeed      .Text          = OM.DevOptn.dEjectSpeed        .ToString();
                tbPickUpFrstOfs   .Text          = OM.DevOptn.dPickUpFrstOfs     .ToString();
                tbPickUpFrstSpeed .Text          = OM.DevOptn.dPickUpFrstSpeed   .ToString();
                tbAtPlaceUpSpeed  .Text          = OM.DevOptn.dAtPlaceUpSpeed    .ToString();
                tbAtPlaceUpOfs    .Text          = OM.DevOptn.dAtPlaceUpOfs      .ToString();

                tbGetHeatDelay    .Text          = OM.DevOptn.iGetHeatDelay      .ToString();
                tbOutHeatDelay    .Text          = OM.DevOptn.iOutHeatDelay      .ToString();

                tbSubMinFlat      .Text          = OM.DevOptn.dSubMinFlat        .ToString();
                tbEndMinFlat      .Text          = OM.DevOptn.dEndMinFlat        .ToString();

                cbRvsWafer        .Checked       = OM.DevOptn.bRvsWafer                     ;

            }
            else
            {
                //Pick Option
                OM.DevOptn.iPickDelay        = CConfig.StrToIntDef   (tbPickDelay        .Text, OM.DevOptn.iPickDelay       );  
                    
                //Vision Tolerance Option
                OM.DevOptn.dVisnTolXY        = CConfig.StrToDoubleDef(tbVisnTolXY        .Text, OM.DevOptn.dVisnTolXY       );      
                OM.DevOptn.dVisnTolAng       = CConfig.StrToDoubleDef(tbVisnTolAng       .Text, OM.DevOptn.dVisnTolAng      ); 
                OM.DevOptn.dRghtEndVisnTolXY = CConfig.StrToDoubleDef(tbRghtEndVisnTolXY .Text, OM.DevOptn.dRghtEndVisnTolXY);  
                OM.DevOptn.dLeftEndVisnTolXY = CConfig.StrToDoubleDef(tbLeftEndVisnTolXY .Text, OM.DevOptn.dLeftEndVisnTolXY);    

                OM.DevOptn.dShakeOffset      = CConfig.StrToDoubleDef(tbShakeOffset      .Text, OM.DevOptn.dShakeOffset     );    
                OM.DevOptn.dShakeRange       = CConfig.StrToDoubleDef(tbShakeRange       .Text, OM.DevOptn.dShakeRange      );    
                OM.DevOptn.dAttachSpeed1     = CConfig.StrToDoubleDef(tbAttachSpeed1     .Text, OM.DevOptn.dAttachSpeed1    ); 
                OM.DevOptn.dAttachForce      = CConfig.StrToDoubleDef(tbAttachForce      .Text, OM.DevOptn.dAttachForce     );
                OM.DevOptn.dAttachForceOfs   = CConfig.StrToDoubleDef(tbAttachForceOfs   .Text, OM.DevOptn.dAttachForceOfs  );
                OM.DevOptn.iAttachDelay      = CConfig.StrToIntDef   (tbAttachDelay      .Text, OM.DevOptn.iAttachDelay     );
                OM.DevOptn.iAtAttachDelay    = CConfig.StrToIntDef   (tbAtAttachDelay    .Text, OM.DevOptn.iAtAttachDelay   );
                                                                                                                            
                OM.DevOptn.iSStgTemp         = CConfig.StrToIntDef   (tbSStgTemp         .Text, OM.DevOptn.iSStgTemp        );
                                                             
                //패턴 탭                                                                
                OM.DevOptn.dDieXOfs          = CConfig.StrToDoubleDef(tbDieXOfs          .Text, OM.DevOptn.dDieXOfs         );
                OM.DevOptn.dDieYOfs          = CConfig.StrToDoubleDef(tbDieYOfs          .Text, OM.DevOptn.dDieYOfs         );
                                                                                                                            
                OM.DevOptn.dMstVsnX          = CConfig.StrToDoubleDef(tbMainVsnX         .Text, OM.DevOptn.dMstVsnX         );
                OM.DevOptn.dMstVsnY          = CConfig.StrToDoubleDef(tbMainVsnY         .Text, OM.DevOptn.dMstVsnY         );
                OM.DevOptn.dSlvVsnX          = CConfig.StrToDoubleDef(tbSubVsnX          .Text, OM.DevOptn.dSlvVsnX         );
                OM.DevOptn.dSlvVsnY          = CConfig.StrToDoubleDef(tbSubVsnY          .Text, OM.DevOptn.dSlvVsnY         );
                                                                                                                            
                OM.DevOptn.iDspCh            = cbDspCh.SelectedIndex;                                                       
                OM.DevOptn.dDspZOfs          = CConfig.StrToDoubleDef(tbDspZOfs          .Text, OM.DevOptn.dDspZOfs         );
                OM.DevOptn.iDspMinAmount     = CConfig.StrToIntDef   (tbDspMinAmount     .Text, OM.DevOptn.iDspMinAmount    );
                OM.DevOptn.iDispShotDelay    = CConfig.StrToIntDef   (tbDispShotDelay    .Text, OM.DevOptn.iDispShotDelay   );
                OM.DevOptn.iDispAtShotDelay  = CConfig.StrToIntDef   (tbDispAtShotDelay  .Text, OM.DevOptn.iDispAtShotDelay );
                                            
                OM.DevOptn.iSubRgb           = cdSub      .Color.ToArgb(); 
                OM.DevOptn.iDieRgb           = cdDie      .Color.ToArgb();
                OM.DevOptn.iVsnRgb           = cdVsn      .Color.ToArgb();
                
                OM.DevOptn.dToolCrashDist    = CConfig.StrToDoubleDef(tbToolCrashDist    .Text, OM.DevOptn.dToolCrashDist   );                                                                                                                            
                OM.DevOptn.dUVWTOfs          = CConfig.StrToDoubleDef(tbUVWTOfs          .Text, OM.DevOptn.dUVWTOfs         );

                OM.DevOptn.iEjectAtUpDelay   = CConfig.StrToIntDef   (tbEjectAtUpDelay   .Text ,OM.DevOptn.iEjectAtUpDelay  );
                OM.DevOptn.dEjectSpeed       = CConfig.StrToDoubleDef(tbEjectSpeed       .Text ,OM.DevOptn.dEjectSpeed      );
                OM.DevOptn.dPickUpFrstOfs    = CConfig.StrToDoubleDef(tbPickUpFrstOfs    .Text ,OM.DevOptn.dPickUpFrstOfs   );
                OM.DevOptn.dPickUpFrstSpeed  = CConfig.StrToDoubleDef(tbPickUpFrstSpeed  .Text ,OM.DevOptn.dPickUpFrstSpeed );
                OM.DevOptn.dAtPlaceUpSpeed   = CConfig.StrToDoubleDef(tbAtPlaceUpSpeed   .Text ,OM.DevOptn.dAtPlaceUpSpeed  );
                OM.DevOptn.dAtPlaceUpOfs     = CConfig.StrToDoubleDef(tbAtPlaceUpOfs     .Text ,OM.DevOptn.dAtPlaceUpOfs    );

                OM.DevOptn.iGetHeatDelay     = CConfig.StrToIntDef   (tbGetHeatDelay     .Text ,OM.DevOptn.iGetHeatDelay    );
                OM.DevOptn.iOutHeatDelay     = CConfig.StrToIntDef   (tbOutHeatDelay     .Text ,OM.DevOptn.iOutHeatDelay    );

                OM.DevOptn.dSubMinFlat       = CConfig.StrToDoubleDef(tbSubMinFlat       .Text ,OM.DevOptn.dSubMinFlat      );
                OM.DevOptn.dEndMinFlat       = CConfig.StrToDoubleDef(tbEndMinFlat       .Text ,OM.DevOptn.dEndMinFlat      );

                OM.DevOptn.bRvsWafer         = cbRvsWafer        .Checked                  ;

                UpdateDevOptn(true);
            }
        }

        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            tmUpdate.Enabled = false;
            btSaveDevice.Enabled = !SEQ._bRun;
            tmUpdate.Enabled = true;
        }


        private void pbSTG_Paint(object sender, PaintEventArgs e)
        {
            int iTag = Convert.ToInt32(((PictureBox)sender).Tag);

            SolidBrush Brush = new SolidBrush(Color.Black);

            Pen Pen = new Pen(Color.Black);

            Graphics gSTG = pbWLDR.CreateGraphics();


            double dX1, dX2, dY1, dY2;

            int iWSTGColCnt, iWSTGRowCnt, iSSTGColCnt, iSSTGRowCnt, iWLDRColCnt, iWLDRRowCnt, iSLDRColCnt, iSLDRRowCnt;
                                                                    
            //DM.ARAY[(int)ri.WLDR].SetMaxColRow(1                       , OM.DevInfo.iWLDR_SlotCnt);
            //DM.ARAY[(int)ri.SLDR].SetMaxColRow(1                       , OM.DevInfo.iSLDR_SlotCnt);
            //DM.ARAY[(int)ri.WSTG].SetMaxColRow(OM.DevInfo.iWFER_DieCnt , 1                       );
            //DM.ARAY[(int)ri.SSTG].SetMaxColRow(OM.DevInfo.iSBOT_PcktCnt, 1                       );
            //DM.ARAY[(int)ri.PCKR].SetMaxColRow(1                       , 1                       );


            Graphics g = e.Graphics;

            switch (iTag)
            {
                default:  break;
                case  1:  iWSTGColCnt = OM.DevInfo.iWFER_DieCntX;
                          iWSTGRowCnt = OM.DevInfo.iWFER_DieCntY;            

                          int iGetWSTGWidth  = pbWSTG.Width;
                          int iGetWSTGHeight = pbWSTG.Height;
                          
                          double iSetWSTGWidth = 0, iSetWSTGHeight = 0;
                          
                          double uWSTGGw   = (double)iGetWSTGWidth  / (double)(iWSTGColCnt);
                          double uWSTGGh   = (double)iGetWSTGHeight / (double)(iWSTGRowCnt);
                          double dWSTGWOff = (double)(iGetWSTGWidth  - uWSTGGw * (iWSTGColCnt)) / 2.0;
                          double dWSTGHOff = (double)(iGetWSTGHeight - uWSTGGh * (iWSTGRowCnt)) / 2.0;
                          
                          Pen.Color = Color.Black;
                          
                          Brush.Color = Color.HotPink;
                          
                          
                          for (int r = 0; r < iWSTGRowCnt; r++)
                          {
                              for (int c = 0; c < iWSTGColCnt; c++)
                              {
                                  
                                  dY1 = dWSTGHOff + r * uWSTGGh -1;
                                  dY2 = dWSTGHOff + r * uWSTGGh + uWSTGGh ;
                                  dX1 = dWSTGWOff + c * uWSTGGw -1;
                                  dX2 = dWSTGWOff + c * uWSTGGw + uWSTGGw ;
                          
                                  g.FillRectangle(Brush, (float)dX1, (float)dY1, (float)dX2, (float)dY2);
                                  g.DrawRectangle(Pen  , (float)dX1, (float)dY1, (float)dX2, (float)dY2);
                          
                                  iSetWSTGWidth += dY2;
                                  iSetWSTGHeight += dX2;
                              }
                          
                          }

                          break;


                case  2:  iSSTGColCnt = 1;
                          iSSTGRowCnt = OM.DevInfo.iSBOT_PcktCnt;

                          int iGetSSTGWidth  = pbSSTG.Width;
                          int iGetSSTGHeight = pbSSTG.Height;

                          double iSetSSTGWidth = 0, iSetSSTGHeight = 0;

                          double uSSTGGw   = (double) iGetSSTGWidth  / (double)(iSSTGColCnt);
                          double uSSTGGh   = (double) iGetSSTGHeight / (double)(iSSTGRowCnt);
                          double dSSTGWOff = (double)(iGetSSTGWidth  - uSSTGGw * (iSSTGColCnt)) / 2.0;
                          double dSSTGHOff = (double)(iGetSSTGHeight - uSSTGGh * (iSSTGRowCnt)) / 2.0;

                          Pen.Color = Color.Black;

                          Brush.Color = Color.HotPink;


                          for (int r = 0; r < iSSTGRowCnt; r++)
                          {
                              for (int c = 0; c < iSSTGColCnt; c++)
                              {

                                  dY1 = dSSTGHOff + r * uSSTGGh - 1;
                                  dY2 = dSSTGHOff + r * uSSTGGh + uSSTGGh;
                                  dX1 = dSSTGWOff + c * uSSTGGw - 1;
                                  dX2 = dSSTGWOff + c * uSSTGGw + uSSTGGw;

                                  g.FillRectangle(Brush, (float)dX1, (float)dY1, (float)dX2, (float)dY2);
                                  g.DrawRectangle(Pen, (float)dX1, (float)dY1, (float)dX2, (float)dY2);

                                  iSetSSTGWidth += dY2;
                                  iSetSSTGHeight += dX2;
                              }

                          }

                          break;

                case  3:  iWLDRColCnt = 1;
                          iWLDRRowCnt = OM.DevInfo.iWLDR_SlotCnt;

                          int iGetWLDRWidth  = pbWLDR.Width;
                          int iGetWLDRHeight = pbWLDR.Height;

                          double iSetWLDRWidth = 0, iSetWLDRHeight = 0;

                          double uWLDRGw = (double)iGetWLDRWidth  / (double)(iWLDRColCnt);
                          double uWLDRGh = (double)iGetWLDRHeight / (double)(iWLDRRowCnt);
                          double dWLDRWOff = (double)(iGetWLDRWidth  - uWLDRGw * (iWLDRColCnt)) / 2.0;
                          double dWLDRHOff = (double)(iGetWLDRHeight - uWLDRGh * (iWLDRRowCnt)) / 2.0;

                          Pen.Color = Color.Black;

                          Brush.Color = Color.HotPink;


                          for (int r = 0; r < iWLDRRowCnt; r++)
                          {
                              for (int c = 0; c < iWLDRColCnt; c++)
                              {

                                  dY1 = dWLDRHOff + r * uWLDRGh - 1;
                                  dY2 = dWLDRHOff + r * uWLDRGh + uWLDRGh;
                                  dX1 = dWLDRWOff + c * uWLDRGw - 1;
                                  dX2 = dWLDRWOff + c * uWLDRGw + uWLDRGw;

                                  g.FillRectangle(Brush, (float)dX1, (float)dY1, (float)dX2, (float)dY2);
                                  g.DrawRectangle(Pen, (float)dX1, (float)dY1, (float)dX2, (float)dY2);

                                  iSetWLDRWidth += dY2;
                                  iSetWLDRHeight += dX2;
                              }

                          }

                          break;

                case  4:  iSLDRColCnt = 1;
                          iSLDRRowCnt = OM.DevInfo.iSLDR_SlotCnt;

                          int iGetSLDRWidth  = pbSLDR.Width;
                          int iGetSLDRHeight = pbSLDR.Height;

                          double iSetSLDRWidth = 0, iSetSLDRHeight = 0;

                          double uSLDRGw = (double)iGetSLDRWidth  / (double)(iSLDRColCnt);
                          double uSLDRGh = (double)iGetSLDRHeight / (double)(iSLDRRowCnt);
                          double dSLDRWOff = (double)(iGetSLDRWidth  - uSLDRGw * (iSLDRColCnt)) / 2.0;
                          double dSLDRHOff = (double)(iGetSLDRHeight - uSLDRGh * (iSLDRRowCnt)) / 2.0;

                          Pen.Color = Color.Black;

                          Brush.Color = Color.HotPink;


                          for (int r = 0; r < iSLDRRowCnt; r++)
                          {
                              for (int c = 0; c < iSLDRColCnt; c++)
                              {

                                  dY1 = dSLDRHOff + r * uSLDRGh - 1;
                                  dY2 = dSLDRHOff + r * uSLDRGh + uSLDRGh;
                                  dX1 = dSLDRWOff + c * uSLDRGw - 1;
                                  dX2 = dSLDRWOff + c * uSLDRGw + uSLDRGw;

                                  g.FillRectangle(Brush, (float)dX1, (float)dY1, (float)dX2, (float)dY2);
                                  g.DrawRectangle(Pen, (float)dX1, (float)dY1, (float)dX2, (float)dY2);

                                  iSetSLDRWidth += dY2;
                                  iSetSLDRHeight += dX2;
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
            UpdateDevInfo(false);
            UpdateDevOptn(false);


            PM.UpdatePstn(false);
            UpdatePattern(false);

        }
        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            

        }

        private void btAction_Click(object sender, EventArgs e)
        {
            
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }

        private void btFanMotr_Click(object sender, EventArgs e)
        {

        }

        private void btDoorLock_Click(object sender, EventArgs e)
        {
            
        }

//여기부터 DieAttach 그림 그리는거. 진섭===================================================================
        public void UpdatePattern(bool bToTable)
        {
            if (bToTable)
            {
                //디스펜서 업데이트
                SEQ.DispPtrn.SetListViewItem();                
                tbDispScalePatternX.Text = (SEQ.DispPtrn.GetScaleX() * 100).ToString();
                tbDispScalePatternY.Text = (SEQ.DispPtrn.GetScaleY() * 100).ToString();
                tbHghtScalePatternX.Text = (SEQ.HghtPtrn.GetScaleX() * 100).ToString();
                tbHghtScalePatternY.Text = (SEQ.HghtPtrn.GetScaleY() * 100).ToString();
                tbDisprNodeCnt     .Text =  SEQ.DispPtrn.GetDispPosCnt().ToString() ;
                tbDisprAcc         .Text =  SEQ.DispPtrn.GetAcc().ToString();
                tbDisprDec         .Text =  SEQ.DispPtrn.GetDec().ToString();
                int iDispPosCnt = SEQ.DispPtrn.GetDispPosCnt();
                for(int i = 0 ; i < iDispPosCnt ; i++) 
                {
                    SEQ.DispPtrn.lvNodePos.Items[i ].SubItems[1].Text = SEQ.DispPtrn.GetDispPosX     (i).ToString();
                    SEQ.DispPtrn.lvNodePos.Items[i ].SubItems[2].Text = SEQ.DispPtrn.GetDispPosY     (i).ToString();
                    SEQ.DispPtrn.lvNodePos.Items[i ].SubItems[3].Text = SEQ.DispPtrn.GetDispPosZ     (i).ToString();
                    SEQ.DispPtrn.lvNodePos.Items[i ].SubItems[4].Text = SEQ.DispPtrn.GetScaleDispPosX(i).ToString();
                    SEQ.DispPtrn.lvNodePos.Items[i ].SubItems[5].Text = SEQ.DispPtrn.GetScaleDispPosY(i).ToString();
                    SEQ.DispPtrn.lvNodePos.Items[i ].SubItems[6].Text = SEQ.DispPtrn.GetSpeed        (i).ToString();
                    SEQ.DispPtrn.lvNodePos.Items[i ].SubItems[7].Text = SEQ.DispPtrn.GetBfDelay      (i).ToString();
                    SEQ.DispPtrn.lvNodePos.Items[i ].SubItems[8].Text = SEQ.DispPtrn.GetDispOn       (i) ? "1":"0" ;
                    SEQ.DispPtrn.lvNodePos.Items[i ].SubItems[9].Text = SEQ.DispPtrn.GetAtDelay      (i).ToString();
                }
                cdDisprLine.Color = Color.FromArgb(SEQ.DispPtrn.m_iDisprLineRgb);
                cdDisprSlct.Color = Color.FromArgb(SEQ.DispPtrn.m_iDisprSlctRgb);

                //높이측정 업데이트
                SEQ.HghtPtrn.SetHghtItem();
                tbHghtNodeCnt.Text = SEQ.HghtPtrn.GetHghtPosCnt().ToString();
                int iHghtPosCnt = SEQ.HghtPtrn.GetHghtPosCnt();
                for (int i = 0; i < iHghtPosCnt; i++)
                {
                    SEQ.HghtPtrn.lvHghtPos.Items[i].SubItems[1].Text = SEQ.HghtPtrn.GetHghtPosX     (i).ToString();
                    SEQ.HghtPtrn.lvHghtPos.Items[i].SubItems[2].Text = SEQ.HghtPtrn.GetHghtPosY     (i).ToString();
                    SEQ.HghtPtrn.lvHghtPos.Items[i].SubItems[3].Text = SEQ.HghtPtrn.GetHghtPosZ     (i).ToString();
                    SEQ.HghtPtrn.lvHghtPos.Items[i].SubItems[4].Text = SEQ.HghtPtrn.GetScaleHghtPosX(i).ToString();
                    SEQ.HghtPtrn.lvHghtPos.Items[i].SubItems[5].Text = SEQ.HghtPtrn.GetScaleHghtPosY(i).ToString();
                }        
                cdHght.Color = Color.FromArgb(SEQ.HghtPtrn.m_iHghtRgb );

                //비엘티 업데이트
                SEQ.BltPtrn.SetListViewItem();                
                int iPosCnt = SEQ.BltPtrn.GetBltPosCnt();
                tbBLTNodeCnt.Text = SEQ.BltPtrn.GetBltPosCnt().ToString();
                for(int i = 0 ; i < iPosCnt ; i++) 
                {
                    SEQ.BltPtrn.lvBltPos.Items[i].SubItems[1].Text = SEQ.BltPtrn.GetBltPos(i).dSubPosX.ToString();
                    SEQ.BltPtrn.lvBltPos.Items[i].SubItems[2].Text = SEQ.BltPtrn.GetBltPos(i).dSubPosY.ToString();
                    SEQ.BltPtrn.lvBltPos.Items[i].SubItems[3].Text = SEQ.BltPtrn.GetBltPos(i).dDiePosX.ToString();
                    SEQ.BltPtrn.lvBltPos.Items[i].SubItems[4].Text = SEQ.BltPtrn.GetBltPos(i).dDiePosY.ToString();
                }
            }
            else
            {
                int iNodeCnt = CConfig.StrToIntDef(tbDisprNodeCnt.Text, SEQ.DispPtrn.GetDispPosCnt());
                     if (iNodeCnt >= DispensePattern.MAX_DSP_CMD) SEQ.DispPtrn.SetDispPosCnt(DispensePattern.MAX_DSP_CMD-1);
                else if (iNodeCnt < 1                           ) SEQ.DispPtrn.SetDispPosCnt(1                            );
                else                                              SEQ.DispPtrn.SetDispPosCnt(iNodeCnt                     );
                
                SEQ.DispPtrn.SetListViewItem();
                
                int iDispScalePatternX = Int32.Parse(tbDispScalePatternX.Text);
                int iDispScalePatternY = Int32.Parse(tbDispScalePatternY.Text);

                SEQ.DispPtrn.SetScale(CConfig.StrToDoubleDef((iDispScalePatternX * 0.01).ToString(), SEQ.DispPtrn.GetScaleX()), CConfig.StrToDoubleDef((iDispScalePatternY * 0.01).ToString(), SEQ.DispPtrn.GetScaleY()));
                SEQ.DispPtrn.SetAccDec(CConfig.StrToIntDef(tbDisprAcc.Text, (int)SEQ.DispPtrn.GetAcc()), CConfig.StrToIntDef(tbDisprDec.Text, (int)SEQ.DispPtrn.GetDec()));

                
                
                int iDispPosCnt = SEQ.DispPtrn.GetDispPosCnt();
                for(int i = 0 ; i < iDispPosCnt ; i++) 
                {
                    SEQ.DispPtrn.SetDispPosX(i, CConfig.StrToDoubleDef(SEQ.DispPtrn.lvNodePos.Items[i].SubItems[1].Text, SEQ.DispPtrn.GetDispPosX(i)));
                    SEQ.DispPtrn.SetDispPosY(i, CConfig.StrToDoubleDef(SEQ.DispPtrn.lvNodePos.Items[i].SubItems[2].Text, SEQ.DispPtrn.GetDispPosY(i)));
                    SEQ.DispPtrn.SetDispPosZ(i, CConfig.StrToDoubleDef(SEQ.DispPtrn.lvNodePos.Items[i].SubItems[3].Text, SEQ.DispPtrn.GetDispPosZ(i)));
                    
                    SEQ.DispPtrn.SetSpeed   (i, CConfig.StrToDoubleDef(SEQ.DispPtrn.lvNodePos.Items[i].SubItems[6].Text, SEQ.DispPtrn.GetSpeed  (i)));
                    SEQ.DispPtrn.SetBfDelay (i, CConfig.StrToIntDef(SEQ.DispPtrn.lvNodePos.Items[i].SubItems[7].Text, SEQ.DispPtrn.GetBfDelay(i)));
                    SEQ.DispPtrn.SetDispOn  (i, SEQ.DispPtrn.lvNodePos.Items[i].SubItems[8].Text != "0");
                    SEQ.DispPtrn.SetAtDelay (i, CConfig.StrToIntDef(SEQ.DispPtrn.lvNodePos.Items[i].SubItems[9].Text, SEQ.DispPtrn.GetAtDelay(i)));
                }

                SEQ.DispPtrn.m_iDisprLineRgb = cdDisprLine.Color.ToArgb();
                SEQ.DispPtrn.m_iDisprSlctRgb = cdDisprSlct.Color.ToArgb();



                //높이측정기 업데이트
                iNodeCnt = CConfig.StrToIntDef(tbHghtNodeCnt.Text, SEQ.HghtPtrn.GetHghtPosCnt()); 
                     if (iNodeCnt >= HeightPattern.MAX_HGHT_CMD) SEQ.HghtPtrn.SetHghtPosCnt(HeightPattern.MAX_HGHT_CMD-1);
                else if (iNodeCnt < 1                          ) SEQ.HghtPtrn.SetHghtPosCnt(1);
                else                                             SEQ.HghtPtrn.SetHghtPosCnt(iNodeCnt);
                SEQ.HghtPtrn.SetHghtItem();
                int iHghtScalePatternX = Int32.Parse(tbHghtScalePatternX.Text);
                int iHghtScalePatternY = Int32.Parse(tbHghtScalePatternY.Text);
                SEQ.HghtPtrn.SetScale(CConfig.StrToDoubleDef((iHghtScalePatternX * 0.01).ToString(), SEQ.HghtPtrn.GetScaleX()), CConfig.StrToDoubleDef((iHghtScalePatternY * 0.01).ToString(), SEQ.HghtPtrn.GetScaleY()));
                int iHghtPosCnt = SEQ.HghtPtrn.GetHghtPosCnt();
                for (int i = 0; i < iHghtPosCnt; i++)
                {
                    SEQ.HghtPtrn.SetHghtPosX(i, CConfig.StrToDoubleDef(SEQ.HghtPtrn.lvHghtPos.Items[i].SubItems[1].Text, SEQ.HghtPtrn.GetHghtPosX(i)));
                    SEQ.HghtPtrn.SetHghtPosY(i, CConfig.StrToDoubleDef(SEQ.HghtPtrn.lvHghtPos.Items[i].SubItems[2].Text, SEQ.HghtPtrn.GetHghtPosY(i)));
                    SEQ.HghtPtrn.SetHghtPosZ(i, CConfig.StrToDoubleDef(SEQ.HghtPtrn.lvHghtPos.Items[i].SubItems[3].Text, SEQ.HghtPtrn.GetHghtPosY(i)));
                }
                SEQ.HghtPtrn.m_iHghtRgb = cdHght.Color.ToArgb();

                //여기 뻑나는것 부터.
                iNodeCnt = CConfig.StrToIntDef(tbBLTNodeCnt.Text, SEQ.BltPtrn.GetBltPosCnt()); 
                     if (iNodeCnt >= BltPattern.MAX_BLT_CMD) SEQ.BltPtrn.SetBltPosCnt(BltPattern.MAX_BLT_CMD-1);
                else if (iNodeCnt < 1                      ) SEQ.BltPtrn.SetBltPosCnt(1);
                else                                         SEQ.BltPtrn.SetBltPosCnt(iNodeCnt);         
                SEQ.BltPtrn.SetListViewItem();
                int iPosCnt = SEQ.BltPtrn.GetBltPosCnt();
                for(int i = 0 ; i < iPosCnt ; i++) 
                {
                    SEQ.BltPtrn.BltPos[i].dSubPosX = CConfig.StrToDoubleDef(SEQ.BltPtrn.lvBltPos.Items[i].SubItems[1].Text,SEQ.BltPtrn.BltPos[i].dSubPosX);// 
                    SEQ.BltPtrn.BltPos[i].dSubPosY = CConfig.StrToDoubleDef(SEQ.BltPtrn.lvBltPos.Items[i].SubItems[2].Text,SEQ.BltPtrn.BltPos[i].dSubPosY);// 
                    SEQ.BltPtrn.BltPos[i].dDiePosX = CConfig.StrToDoubleDef(SEQ.BltPtrn.lvBltPos.Items[i].SubItems[3].Text,SEQ.BltPtrn.BltPos[i].dDiePosX);// 
                    SEQ.BltPtrn.BltPos[i].dDiePosY = CConfig.StrToDoubleDef(SEQ.BltPtrn.lvBltPos.Items[i].SubItems[4].Text,SEQ.BltPtrn.BltPos[i].dDiePosY);// 
                }
        
                UpdatePattern(true);
            }
        }

        struct TRect
        {
            public double dleft   ;// = r.left;
            public double dtop    ;// = r.top;
            public double dright  ;// = r.right;
            public double dbottom ;// = r.bottom;
          
        };

        //int cd = cdSub.Color.ToArgb(); 

        public const int PANEL_DRAW_WIDTH = 500;
        public const int REAL_DRAW_WIDTH  = 250;

        public struct TGraphics
        {
            public SolidBrush   Brush ;
            public PointF[]     aPnts ;
            public PointF       Pnts  ;
            public GraphicsPath gPath ;
            //Graphics     g     ;
            //public ColorDialog  Color ;
            public Pen          Pen   ;
        }

        TGraphics    gSub     ;
        TGraphics    gDie     ;
        TGraphics    gDispr   ;
        TGraphics    gCntrLine;
        TGraphics    gArrow   ;
        TGraphics    gString  ;
        TGraphics    gVsnMst  ;
        TGraphics    gVsnSlv  ;
        TGraphics    gCircle  ;
        TGraphics    gHghtLine;
        TGraphics    gBltLine ;
        Graphics     g        ;


        ColorDialog cdSub       = new ColorDialog();
        ColorDialog cdDie       = new ColorDialog();
        ColorDialog cdDisprLine = new ColorDialog();
        ColorDialog cdDisprSlct = new ColorDialog();
        ColorDialog cdVsn       = new ColorDialog();
        ColorDialog cdHght      = new ColorDialog();
        
      
        private void pbDieAttach_Paint(object sender, PaintEventArgs e)
        {
            //Pattern.m_iSubRgb      
            //Pattern.m_iDieRgb      
            //Pattern.m_iDisprLineRgb
            //Pattern.m_iDisprSlctRgb
            //Pattern.m_iVsnRgb      
            //Pattern.m_iHghtRgb     

            gSub     .Brush = new SolidBrush(Color.FromArgb(OM.DevOptn.iSubRgb));
            gSub     .Pen   = new Pen       (Color.FromArgb(OM.DevOptn.iSubRgb));

            gDie     .Brush = new SolidBrush(Color.FromArgb(OM.DevOptn.iDieRgb));
            gDie     .Pen   = new Pen       (Color.FromArgb(OM.DevOptn.iDieRgb));

            //gCntrLine.Brush = new SolidBrush(Color.Lime);
            gCntrLine.Pen   = new Pen(Color.Lime);

            gArrow   .Brush = new SolidBrush(Color.Black);
            gArrow   .Pen   = new Pen(Color.Black);
                           
            //gDispr   .Brush = new SolidBrush(Color.Yellow);
            gDispr   .Pen   = new Pen(Color.FromArgb(SEQ.DispPtrn.m_iDisprLineRgb));

            gString  .Brush = new SolidBrush(Color.Red);
            gString  .Pen   = new Pen(Color.Black);

            gVsnMst  .Pen   = new Pen(Color.FromArgb(OM.DevOptn.iVsnRgb));
            gVsnSlv  .Pen   = new Pen(Color.FromArgb(OM.DevOptn.iVsnRgb));

            gCircle  .Pen   = new Pen       (Color.FromArgb(SEQ.HghtPtrn.m_iHghtRgb));
            gCircle  .Brush = new SolidBrush(Color.FromArgb(SEQ.HghtPtrn.m_iHghtRgb));

            gHghtLine.Pen   = new Pen(Color.FromArgb(SEQ.HghtPtrn.m_iHghtRgb));

            gBltLine .Pen   = new Pen(Color.Brown);



            g = e.Graphics;



            TRect DrawRect ;
            DrawRect.dleft   = 0 ;
            DrawRect.dtop    = 0 ;
            DrawRect.dright  = pbDieAttach.Width  ;
            DrawRect.dbottom = pbDieAttach.Height ;
            
            pbDieAttach.BackColor = Color.Gray ;
            




            gSub.Brush.Color   = Color.Gray;
            g.FillRectangle(gSub.Brush , 0 , 0 ,pbDieAttach.Width,pbDieAttach.Height) ;
            
            //각 레졸루션.
            double dPxFromMm = pbDieAttach.Width / (double)REAL_DRAW_WIDTH  ;
            const double dMmFromPx = REAL_DRAW_WIDTH  / (double)PANEL_DRAW_WIDTH ;
            
            
            
            // 이미지 센터 좌표 값 구하기
            double dCenterXPx = (double)pbDieAttach.Width  / 2.0 ;
            double dCenterYPx = (double)pbDieAttach.Height / 2.0 ;
            
            double dCenterXMm = (double)pbDieAttach.Width  / 2.0 ;
            double dCenterYMm = (double)pbDieAttach.Height / 2.0 ;
            
            
            
            //자제 그림 Substrate=============================================================
            double dSubSttX = (REAL_DRAW_WIDTH - OM.DevInfo.dSubWidth )/2.0 ;
            double dSubSttY = (REAL_DRAW_WIDTH - OM.DevInfo.dSubHeight)/2.0 ;
            double dSubEndX = dSubSttX + OM.DevInfo.dSubWidth;
            double dSubEndY = dSubSttY + OM.DevInfo.dSubHeight;

            
            const int iPolygonSubPnt = 5 ;
            Point[] aSubPnts = new Point[iPolygonSubPnt];
            //왼쪽위.
            aSubPnts[0].X = (int)(dPxFromMm * dSubSttX) ;
            aSubPnts[0].Y = (int)(dPxFromMm * dSubSttY) ;
            aSubPnts[1].X = (int)(dPxFromMm * dSubEndX) ;
            aSubPnts[1].Y = (int)(dPxFromMm * dSubSttY) ;
            aSubPnts[2].X = (int)(dPxFromMm * dSubEndX) ;
            aSubPnts[2].Y = (int)(dPxFromMm * dSubEndY) ;
            aSubPnts[3].X = (int)(dPxFromMm * dSubSttX) ;
            aSubPnts[3].Y = (int)(dPxFromMm * dSubEndY) ;
            aSubPnts[4].X = (int)(dPxFromMm * dSubSttX) ;
            aSubPnts[4].Y = (int)(dPxFromMm * dSubSttY) ;

            gSub.Pen.Color     = cdSub.Color;
            gSub.Brush.Color   = cdSub.Color;
            gSub.Pen.Width     = 1;
            gSub.Pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;

            g.DrawPolygon(gSub.Pen  , aSubPnts);
            g.FillPolygon(gSub.Brush, aSubPnts);

            
            //==========================================================================
            
            
            //자제 그림 DIE=============================================================
            
            double dDieSttX = (REAL_DRAW_WIDTH - OM.DevInfo.dDieWidth )/2.0 + OM.DevOptn.dDieXOfs ;
            double dDieSttY = (REAL_DRAW_WIDTH - OM.DevInfo.dDieHeight)/2.0 + OM.DevOptn.dDieYOfs ;
            double dDieEndX = dDieSttX + OM.DevInfo.dDieWidth  ;
            double dDieEndY = dDieSttY + OM.DevInfo.dDieHeight ;

            const int iPolygonDiePnt = 5;
            Point[] aDiePnts = new Point[iPolygonDiePnt];
            //왼쪽위.

            aDiePnts[0].X = (int)(dPxFromMm * dDieSttX) ;
            aDiePnts[0].Y = (int)(dPxFromMm * dDieSttY) ;
            aDiePnts[1].X = (int)(dPxFromMm * dDieEndX) ;
            aDiePnts[1].Y = (int)(dPxFromMm * dDieSttY) ;
            aDiePnts[2].X = (int)(dPxFromMm * dDieEndX) ;
            aDiePnts[2].Y = (int)(dPxFromMm * dDieEndY) ;
            aDiePnts[3].X = (int)(dPxFromMm * dDieSttX) ;
            aDiePnts[3].Y = (int)(dPxFromMm * dDieEndY) ;
            aDiePnts[4].X = (int)(dPxFromMm * dDieSttX) ;
            aDiePnts[4].Y = (int)(dPxFromMm * dDieSttY) ;

            gDie.Pen.Color     = cdDie.Color;
            gDie.Brush.Color   = cdDie.Color;
            gDie.Pen.Width     = 1;
            gDie.Pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;

            g.DrawPolygon(gDie.Pen  , aDiePnts);
            g.FillPolygon(gDie.Brush, aDiePnts);
            
            // Dispensor 실제 Line 그리는 부분
            if (tcPattern.SelectedTab == tpDispr)
            {
                PointF BfDrawPnt;
                PointF AtDrawPnt;
                double dDrawX, dDrawY;

                gDispr.Pen.Width = 1;
                dDrawX = dPxFromMm * (SEQ.DispPtrn.GetScaleDispPosX(0) + REAL_DRAW_WIDTH/2.0);// + PTT.GetSttOffsetY());
                dDrawY = dPxFromMm * (SEQ.DispPtrn.GetScaleDispPosY(0) + REAL_DRAW_WIDTH/2.0);// + PTT.GetSttOffsetX());

                g.DrawEllipse(gCircle.Pen  , (float)(dDrawX - 1), (float)(dDrawY - 1), 2, 2);
                g.FillEllipse(gCircle.Brush, (float)(dDrawX - 1), (float)(dDrawY - 1), 2, 2);
             
                BfDrawPnt = new PointF((float)dDrawX, (float)dDrawY);
                
                int iDispPosCnt = SEQ.DispPtrn.GetDispPosCnt();
                
                for (int i = 0; i < iDispPosCnt; i++)
                {
                    gDispr.Pen.Color = Color.Lime;
                    gDispr.Pen.DashStyle = SEQ.DispPtrn.GetDispOn(i) ? DashStyle.Solid : DashStyle.Dot; // : clYellow ;
                    //dDrawX = dPxFromMm * (SEQ.DispPtrn.GetScaleDispPosX(i)+ dCenterXMm); //+ PTT.GetSttOffsetY()
                    //dDrawY = dPxFromMm * (SEQ.DispPtrn.GetScaleDispPosY(i)+ dCenterYMm); //+ PTT.GetSttOffsetX()

                    dDrawX = dPxFromMm * (SEQ.DispPtrn.GetScaleDispPosX(i)+ REAL_DRAW_WIDTH/2.0);
                    dDrawY = dPxFromMm * (SEQ.DispPtrn.GetScaleDispPosY(i)+ REAL_DRAW_WIDTH/2.0);
                   
                    AtDrawPnt = new PointF((float)dDrawX, (float)dDrawY);
                
                    //리스트뷰 아이템 선택했을때 선 색깔 바꾸는거
                    if (SEQ.DispPtrn.lvNodePos.Items[i].Selected)
                    {
                        gDispr.Pen.Width = 2;
                        gDispr.Pen.Color = Color.Red;
                        g.DrawLine(gDispr.Pen, BfDrawPnt, AtDrawPnt);
                    }
                    gDispr.Pen.Width = 1;
                    g.DrawLine(gDispr.Pen, BfDrawPnt, AtDrawPnt);
                
                    BfDrawPnt = AtDrawPnt;
                }
            }
            
            //Center 점...
            Point XPntStt = new Point((int)dCenterXPx, 0);
            Point XPntEnd = new Point((int)dCenterXPx, pbDieAttach.Height);

            Point YPntStt = new Point(0, (int)dCenterYPx);
            Point YPntEnd = new Point(pbDieAttach.Width, (int)dCenterYPx );

            gCntrLine.Pen.Color = Color.Lime; // 실제 디바이스 크기의 사각형 그릴 때 필요한 부분, Device 가 여러개라 수정해야함...
            gCntrLine.Pen.Width = 1;
            gCntrLine.Pen.DashStyle = DashStyle.DashDot;

            g.DrawLine(gCntrLine.Pen, XPntStt, XPntEnd);
            g.DrawLine(gCntrLine.Pen, YPntStt, YPntEnd);
            
            //그냥 바깥으로 뱀.
            //// 자재 X, Y 화살표 방향 그리기 부분
            //gArrow.Pen.DashStyle = DashStyle.Solid;
            //gArrow.Pen.Color = Color.Black;
            //gArrow.Pen.Width = 8;
            //gArrow.Pen.StartCap = LineCap.AnchorMask;
            //gArrow.Pen.EndCap = LineCap.ArrowAnchor;

            ////X방향 화살표
            //Point XArrowSttPnt = new Point(40 , 10);
            //Point XArrowEndPnt = new Point(180, 10);

            //g.DrawLine(gArrow.Pen, XArrowSttPnt, XArrowEndPnt);

            ////Y방향 화살표
            //Point YArrowSttPnt = new Point(10, 40 );
            //Point YArrowEndPnt = new Point(10, 180);

            //g.DrawLine(gArrow.Pen, YArrowSttPnt, YArrowEndPnt);

            ////X, Y 화살표 글자 쓰기
            //gString.Brush.Color = Color.Red;
            //FontFamily familyName = new FontFamily("굴림");                   // FontFamily
            //Font myFont = new System.Drawing.Font(familyName, 20, FontStyle.Bold, GraphicsUnit.Pixel);  // 폰트생성
            //PointF X = new PointF(20, 2);                           // 글자 시작위치
            //PointF Y = new PointF(2, 17);
            //g.DrawString("X", myFont, gString.Brush, X);     // 글자 쓰기
            //g.DrawString("Y", myFont, gString.Brush, Y);

            //비젼 마스터/슬레이브 포지션 타겟 그리는부분
            //비젼 마스터
            

            //높이측정기 포지션 타겟 그리는부분
            if (tcPattern.SelectedTab == tpHghtSnsr)
            {
                PointF BfDrawPnt;
                PointF AtDrawPnt;

                gCircle .Pen.Color    = cdHght.Color;
                gCircle .Brush.Color  = cdHght.Color;
                gHghtLine.Pen.Color   = cdHght.Color;

                double dDrawX, dDrawY;

                dDrawX = dPxFromMm * (SEQ.HghtPtrn.GetScaleHghtPosX(0) + REAL_DRAW_WIDTH/2.0);// + PTT.GetSttOffsetY());
                dDrawY = dPxFromMm * (SEQ.HghtPtrn.GetScaleHghtPosY(0) + REAL_DRAW_WIDTH/2.0);// + PTT.GetSttOffsetX());
                BfDrawPnt = new PointF((float)dDrawX, (float)dDrawY);
                g.DrawEllipse(gCircle.Pen  , (float)(dDrawX - 1), (float)(dDrawY - 1), 2, 2);
                g.FillEllipse(gCircle.Brush, (float)(dDrawX - 1), (float)(dDrawY - 1), 2, 2);
                int iHghtPosCnt = SEQ.HghtPtrn.GetHghtPosCnt();
                for (int i = 0; i < iHghtPosCnt; i++)
                {
                    //선 엔드캡 부분 커스터마이징 하는 부분
                    //AdjustableArrowCap cusCap;
                    //cusCap = new AdjustableArrowCap(3, 3);

                    gHghtLine.Pen.Width = 1;
                    gHghtLine.Pen.DashStyle    = DashStyle.Solid;
                    //gHghtLine.Pen.StartCap     = LineCap.AnchorMask;
                    //gHghtLine.Pen.EndCap       = LineCap.Custom;
                    //gHghtLine.Pen.CustomEndCap = cusCap;

                    dDrawX = dPxFromMm * (SEQ.HghtPtrn.GetScaleHghtPosX(i) + REAL_DRAW_WIDTH/2.0); //+ PTT.GetSttOffsetY()
                    dDrawY = dPxFromMm * (SEQ.HghtPtrn.GetScaleHghtPosY(i) + REAL_DRAW_WIDTH/2.0); //+ PTT.GetSttOffsetX()
                    AtDrawPnt = new PointF((float)dDrawX, (float)dDrawY);
                    g.DrawLine(gHghtLine.Pen, BfDrawPnt, AtDrawPnt);
                    //g.DrawEllipse(gHghtDot.Pen, (float)(dDrawX - 3), (float)(dDrawY - 3), 6, 6);
                    //g.FillEllipse(gHghtDot.Brush, (float)(dDrawX - 3), (float)(dDrawY - 3), 6, 6);
                    //리스트뷰 아이템 선택했을때 선 색깔 바꾸는거
                    if (SEQ.HghtPtrn.lvHghtPos.Items[i].Selected)
                    {
                        gCircle .Pen.Width = 2;
                        gCircle .Pen.Color = Color.Red;
                        gHghtLine.Pen.Width = 2;
                        gHghtLine.Pen.Color = Color.Red;

                        g.DrawLine(gHghtLine.Pen, BfDrawPnt, AtDrawPnt);
                    }
                    

                    BfDrawPnt = AtDrawPnt;
                }
            }

            if (tcPattern.SelectedTab == tpBlt)
            {
                PointF SttPnt = new PointF(0,0);
                PointF EndPnt = new PointF(0,0);

                

                int iPosCnt = SEQ.BltPtrn.GetBltPosCnt();
                for (int i = 0; i < iPosCnt; i++)
                {
                    gBltLine.Pen.Width = 1;
                    gBltLine.Pen.Color   = Color.Lime;
                    gBltLine.Pen.DashStyle = DashStyle.Solid;

                    SttPnt.X = (float)(dPxFromMm*(SEQ.BltPtrn.GetBltPos(i).dSubPosX + REAL_DRAW_WIDTH/2.0)) ;
                    SttPnt.Y = (float)(dPxFromMm*(SEQ.BltPtrn.GetBltPos(i).dSubPosY + REAL_DRAW_WIDTH/2.0)) ;
                    EndPnt.X = (float)(dPxFromMm*(SEQ.BltPtrn.GetBltPos(i).dDiePosX + REAL_DRAW_WIDTH/2.0)) ;
                    EndPnt.Y = (float)(dPxFromMm*(SEQ.BltPtrn.GetBltPos(i).dDiePosY + REAL_DRAW_WIDTH/2.0)) ;

                    g.DrawLine(gBltLine.Pen, SttPnt, EndPnt);
                    //g.DrawEllipse(gHghtDot.Pen, (float)(dDrawX - 3), (float)(dDrawY - 3), 6, 6);
                    //g.FillEllipse(gHghtDot.Brush, (float)(dDrawX - 3), (float)(dDrawY - 3), 6, 6);
                    //리스트뷰 아이템 선택했을때 선 색깔 바꾸는거
                    if (SEQ.BltPtrn.lvBltPos.Items[i].Selected)
                    {
                        gBltLine.Pen.Width = 2;
                        gBltLine.Pen.Color = Color.Red;
                        g.DrawLine(gBltLine.Pen, SttPnt, EndPnt);
                    }
                    
                }
            }

            gDispr   .Pen.Dispose();
            gCntrLine.Pen.Dispose();
            gArrow   .Pen.Dispose();
            gString  .Pen.Dispose();            
            gString  .Brush.Dispose();
        }

        
        private void btSubColor_Click(object sender, EventArgs e)
        {
            //gSub.Color = new ColorDialog();
            if (cdSub.ShowDialog() == DialogResult.OK)
            {
                gSub.Pen.Color   = cdSub.Color;
                gSub.Brush.Color = cdSub.Color;
            }

            cdSub.Dispose();
        }

        private void btDieColor_Click(object sender, EventArgs e)
        {
            //gDie.Color = new ColorDialog();
            if (cdDie.ShowDialog() == DialogResult.OK)
            {
                gDie.Pen.Color   = cdDie.Color;
                gDie.Brush.Color = cdDie.Color;
            }
            
            cdDie.Dispose();
        }
     
        private void button2_Click(object sender, EventArgs e)
        {
            if (cdVsn.ShowDialog() == DialogResult.OK)
            {
                gVsnMst.Pen.Color   = cdVsn.Color;
                //gVsnSlv.Pen.Color   = cdVsn.Color;
                //gDie.Brush.Color = cdDie.Color;
            }
            cdVsn.Dispose();
        }

        private void btHghtColor_Click(object sender, EventArgs e)
        {
            if (cdHght.ShowDialog() == DialogResult.OK)
            {
                gCircle .Pen.Color = cdHght.Color;
                gHghtLine.Pen.Color = cdHght.Color;
            }
            cdHght.Dispose();
        }

        private void btDispChSet_Click(object sender, EventArgs e)
        {
            FormDispCh FrmDispCh = new FormDispCh();
            FrmDispCh.Show();
        }

        private void cbDspCh_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            double dPosX = 0.0;
            double dPosY = 0.0;
            double dPosT = 0.0;
            
            if(!double.TryParse(ebUvwX1.Text, out dPosX)){
                dPosX = SEQ.SSTG.UVW.GetTrgX() ;
            }

            if(!double.TryParse(ebUvwY1.Text, out dPosY)){
                dPosY = SEQ.SSTG.UVW.GetTrgX() ;
            }

            if(!double.TryParse(ebUvwT1.Text, out dPosT)){
                dPosT = SEQ.SSTG.UVW.GetTrgX() ;
            }

            SEQ.SSTG.UVW.GoAbs(dPosX, dPosY, dPosT);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            double dPosX = 0.0;
            double dPosY = 0.0;
            double dPosT = 0.0;

            if (!double.TryParse(ebUvwX2.Text, out dPosX))
            {
                dPosX = SEQ.SSTG.UVW.GetTrgX();
            }

            if (!double.TryParse(ebUvwY2.Text, out dPosY))
            {
                dPosY = SEQ.SSTG.UVW.GetTrgX();
            }

            if (!double.TryParse(ebUvwT2.Text, out dPosT))
            {
                dPosT = SEQ.SSTG.UVW.GetTrgX();
            }

            SEQ.SSTG.UVW.GoAbs(dPosX, dPosY, dPosT);
        }

        private void timer1_Tick(object sender,EventArgs e) {

        }

        private void cbDspCh_SelectedValueChanged(object sender,EventArgs e) {
            SEQ.Dispr.SetLoadCh(cbDspCh.SelectedIndex+1);
            //if(bInit) this.btSave_Click(sender, e);
        }

        private void btOri_Click(object sender, EventArgs e)
        {
            Point SttPnt = new Point(0,0);
            pbDieAttach.Location = SttPnt ;
            pbDieAttach.Width  = PANEL_DRAW_WIDTH ;
            pbDieAttach.Height = PANEL_DRAW_WIDTH ;
            pbDieAttach.Invalidate();
        }

        private void btMinimize_Click(object sender, EventArgs e)
        {
            int iAddedWidth  = (int)(pbDieAttach.Width  - pbDieAttach.Width   * 0.8) ;
            int iAddedHeight = (int)(pbDieAttach.Height - pbDieAttach.Height  * 0.8) ;

            pbDieAttach.Width  = (int)(pbDieAttach.Width   * 0.8) ;
            pbDieAttach.Height = (int)(pbDieAttach.Height  * 0.8) ;
            Point SttPnt = new Point(pbDieAttach.Location.X,pbDieAttach.Location.Y);

            if(pbDieAttach.Width > pnDieAttach.Width) {
                SttPnt.X += (int)(iAddedWidth  /2.0) ;
                SttPnt.Y += (int)(iAddedHeight /2.0) ;
            }
            else {
                SttPnt.X = (int)((pnDieAttach.Width  - pbDieAttach.Width )/2.0) ;
                SttPnt.Y = (int)((pnDieAttach.Height - pbDieAttach.Height)/2.0) ;
            }
            pbDieAttach.Location = SttPnt ;
            pbDieAttach.Invalidate();

        }

        private void btMaximize_Click(object sender, EventArgs e)
        {
            int iAddedWidth  = (int)(pbDieAttach.Width  - pbDieAttach.Width   * 1.2) ;
            int iAddedHeight = (int)(pbDieAttach.Height - pbDieAttach.Height  * 1.2) ;

            pbDieAttach.Width  = (int)(pbDieAttach.Width   * 1.2) ;
            pbDieAttach.Height = (int)(pbDieAttach.Height  * 1.2) ;
            Point SttPnt = new Point(pbDieAttach.Location.X,pbDieAttach.Location.Y);
            SttPnt.X += (int)(iAddedWidth  /2.0) ;
            SttPnt.Y += (int)(iAddedHeight /2.0) ;

            pbDieAttach.Location = SttPnt ;
            pbDieAttach.Invalidate();
        }

        int m_iDownX = 0 ;
        int m_iDownY = 0 ;
        bool m_bDowned  = false ;
        private void pbDieAttach_MouseDown(object sender, MouseEventArgs e)
        {
            m_bDowned = false ;
            if(pbDieAttach.Width <= pnDieAttach.Width) return ;
            m_bDowned = true ;
            m_iDownX = e.X ;
            m_iDownY = e.Y ;
        }

        private void pbDieAttach_MouseMove(object sender, MouseEventArgs e)
        {
            //if(!m_bDowned) return ;
            //int iMoveX = e.X - m_iDownX ;
            //int iMoveY = e.Y - m_iDownY ;

            //Point Pos = new Point(pbDieAttach.Location.X + iMoveX , pbDieAttach.Location.Y + iMoveY);
            //pbDieAttach.Location = Pos ;
            //m_iDownX = e.X ;
            //m_iDownY = e.Y ;
        }

        private void pbDieAttach_MouseUp(object sender, MouseEventArgs e)
        {
            if(!m_bDowned) return ;
            int iMoveX = e.X - m_iDownX ;
            int iMoveY = e.Y - m_iDownY ;

            Point Pos = new Point(pbDieAttach.Location.X + iMoveX , pbDieAttach.Location.Y + iMoveY);
            pbDieAttach.Location = Pos ;
            m_iDownX = e.X ;
            m_iDownY = e.Y ;
            m_bDowned = false ;
        }

        private void tcPattern_SelectedIndexChanged(object sender, EventArgs e)
        {
            pbDieAttach.Invalidate();
        }

        private void btDispMove_Click(object sender, EventArgs e)
        {
           
            //SEQ.DispPtrn.GetScaleDispPosX(0)
            double dDist = CConfig.StrToDoubleDef( tbDispMove.Text , 0.0);
            tbDispMove.Text = dDist.ToString() ;

            double dVal = 0 ;
            int iNodeCnt = SEQ.DispPtrn.GetDispPosCnt() ;
            if(cbDispMove.SelectedIndex == 0){
                for(int i = 0 ; i < iNodeCnt ; i++){
                    dVal =  SEQ.DispPtrn.GetDispPosX(i);
                    dVal += dDist ;
                    SEQ.DispPtrn.SetDispPosX(i,dVal);
                }
            }
            else if(cbDispMove.SelectedIndex == 1){
                for(int i = 0 ; i < iNodeCnt ; i++){
                    dVal =  SEQ.DispPtrn.GetDispPosY(i);
                    dVal += dDist ;
                    SEQ.DispPtrn.SetDispPosY(i,dVal);
                }
            }
            else {
                for(int i = 0 ; i < iNodeCnt ; i++){
                    dVal =  SEQ.DispPtrn.GetDispPosZ(i);
                    dVal += dDist ;
                    SEQ.DispPtrn.SetDispPosZ(i,dVal);
                }
            }
            UpdatePattern(true);
            pbDieAttach.Invalidate();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            double dDist = CConfig.StrToDoubleDef( tbHghtMove.Text , 0.0);
            tbHghtMove.Text = dDist.ToString() ;

            double dVal = 0 ;
            int iNodeCnt = SEQ.HghtPtrn.GetHghtPosCnt() ;
            if(cbHghtMove.SelectedIndex == 0){
                for(int i = 0 ; i < iNodeCnt ; i++){
                    dVal =  SEQ.HghtPtrn.GetHghtPosX(i);
                    dVal += dDist ;
                    SEQ.HghtPtrn.SetHghtPosX(i,dVal);
                }
            }
            else if(cbHghtMove.SelectedIndex == 1){
                for(int i = 0 ; i < iNodeCnt ; i++){
                    dVal =  SEQ.HghtPtrn.GetHghtPosY(i);
                    dVal += dDist ;
                    SEQ.HghtPtrn.SetHghtPosY(i,dVal);
                }
            }
            else {
                for(int i = 0 ; i < iNodeCnt ; i++){
                    dVal =  SEQ.HghtPtrn.GetHghtPosZ(i);
                    dVal += dDist ;
                    SEQ.HghtPtrn.SetHghtPosZ(i,dVal);
                }
            }
            UpdatePattern(true);
            pbDieAttach.Invalidate();
        }

        private void btLeftMove_Click(object sender, EventArgs e)
        {
            if (Log.ShowMessageModal("Confirm", "픽,웨이퍼비전,이젝터의 위치를 바꿉니다.") != DialogResult.Yes) return;

            double dMoveX = CConfig.StrToDoubleDef(tbLeftMoveX.Text , 0.0);
            double dMoveY = CConfig.StrToDoubleDef(tbLeftMoveY.Text , 0.0);

            tbLeftMoveX.Text = "0.0" ;
            tbLeftMoveY.Text = "0.0" ;


            double dValX = 0.0 ;
            double dValY = 0.0 ;

            //Pick 포지션.
            dValX = PM.GetValue(mi.TOOL_XLeft , pv.TOOL_XLeftPkPickStt);
            dValY = PM.GetValue(mi.TOOL_YGent , pv.TOOL_YGentPkPickStt);
            dValX += dMoveX ;
            dValY += dMoveY ;
            PM.SetValue(mi.TOOL_XLeft , pv.TOOL_XLeftPkPickStt , dValX);
            PM.SetValue(mi.TOOL_YGent , pv.TOOL_YGentPkPickStt , dValY);

            //웨이퍼 마스터 비전 포지션.
            dValX = PM.GetValue(mi.TOOL_XRght , pv.TOOL_XRghtVsWStgVsnMStt);
            dValY = PM.GetValue(mi.TOOL_YGent , pv.TOOL_YGentVsWStgVsnMStt);
            dValX -= dMoveX ;
            dValY += dMoveY ;
            PM.SetValue(mi.TOOL_XRght , pv.TOOL_XRghtVsWStgVsnMStt , dValX);
            PM.SetValue(mi.TOOL_YGent , pv.TOOL_YGentVsWStgVsnMStt , dValY);

            //웨이퍼 슬레이브 비전 포지션.
            dValX = PM.GetValue(mi.TOOL_XRght , pv.TOOL_XRghtVsWStgVsnSStt);
            dValY = PM.GetValue(mi.TOOL_YGent , pv.TOOL_YGentVsWStgVsnSStt);
            dValX -= dMoveX ;
            dValY += dMoveY ;
            PM.SetValue(mi.TOOL_XRght , pv.TOOL_XRghtVsWStgVsnSStt , dValX);
            PM.SetValue(mi.TOOL_YGent , pv.TOOL_YGentVsWStgVsnSStt , dValY);

            //이젝터Y
            dValY = PM.GetValue(mi.TOOL_YEjtr , pv.TOOL_YEjtrWorkStt);
            dValY += dMoveY ;
            PM.SetValue(mi.TOOL_YEjtr , pv.TOOL_YEjtrWorkStt , dValY);

            //이젝터X 레프트
            dValX = PM.GetValue(mi.TOOL_XEjtL , pv.WSTG_XEjtLStart);
            dValX -= dMoveX ;
            PM.SetValue(mi.TOOL_XEjtL , pv.WSTG_XEjtLStart  , dValX);

            //이젝터X 라이트
            dValX = PM.GetValue(mi.TOOL_XEjtR , pv.WSTG_XEjtRStart);
            dValX += dMoveX ;
            PM.SetValue(mi.TOOL_XEjtR , pv.WSTG_XEjtRStart  , dValX);

            PM.Save(OM.GetCrntDev());
        }

        private void btRghtDieMove_Click(object sender, EventArgs e)
        {
            if (Log.ShowMessageModal("Confirm", "플레이스 ,UVW스테이지 상의 다이비전의 위치를 바꿉니다.") != DialogResult.Yes) return;

            double dMoveX = CConfig.StrToDoubleDef(tbRghtDieMoveX.Text , 0.0);
            double dMoveY = CConfig.StrToDoubleDef(tbRghtDieMoveY.Text , 0.0);

            tbRghtDieMoveX.Text = "0.0" ;
            tbRghtDieMoveY.Text = "0.0" ;

            double dValX = 0.0 ;
            double dValY = 0.0 ;

            //Place 포지션.
            dValX = PM.GetValue(mi.TOOL_XLeft , pv.TOOL_XLeftPkPlce);
            dValY = PM.GetValue(mi.TOOL_YGent , pv.TOOL_YGentPkPlce);
            dValX += dMoveX ;
            dValY += dMoveY ;
            PM.SetValue(mi.TOOL_XLeft , pv.TOOL_XLeftPkPlce , dValX);
            PM.SetValue(mi.TOOL_YGent , pv.TOOL_YGentPkPlce , dValY);

            //플레이스 다이 왼쪽 비전 포지션 
            dValX = PM.GetValue(mi.TOOL_XRght , pv.TOOL_XRghtVsSStgVsnLtS);
            dValX -= dMoveX ;
            PM.SetValue(mi.TOOL_XRght , pv.TOOL_XRghtVsSStgVsnLtS , dValX);
            //Y는 플레이스 포지션하고 엮여 있어서 서브스트레이트를 반대로 해줘야함.
            dValY = PM.GetValue(mi.TOOL_YVisn , pv.TOOL_YVisnSStgVsnLtM);
            dValY -= dMoveY ;
            PM.SetValue(mi.TOOL_YVisn , pv.TOOL_YVisnSStgVsnLtM , dValY);

            //플레이스 다이 오른쪽 비전 포지션 
            dValX = PM.GetValue(mi.TOOL_XRght , pv.TOOL_XRghtVsSStgVsnRtS);
            dValX -= dMoveX ;
            PM.SetValue(mi.TOOL_XRght , pv.TOOL_XRghtVsSStgVsnRtS , dValX);
            //Y는 플레이스 포지션하고 엮여 있어서 서브스트레이트를 반대로 해줘야함.
            dValY = PM.GetValue(mi.TOOL_YVisn , pv.TOOL_YVisnSStgVsnRtM);
            dValY -= dMoveY ;
            PM.SetValue(mi.TOOL_YVisn , pv.TOOL_YVisnSStgVsnRtM , dValY);

            PM.Save(OM.GetCrntDev());

        }

        private void btRghtSstMove_Click(object sender, EventArgs e)
        {
            if (Log.ShowMessageModal("Confirm", "디스펜스관련, 높이측정, UVW스테이지 상의 서브스트레이트 비전의 위치를 바꿉니다.") != DialogResult.Yes) return;

            double dMoveX = CConfig.StrToDoubleDef(tbRghtSstMoveX.Text , 0.0);
            double dMoveY = CConfig.StrToDoubleDef(tbRghtSstMoveY.Text , 0.0);

            tbRghtSstMoveX.Text = "0.0" ;
            tbRghtSstMoveY.Text = "0.0" ;

            double dValX = 0.0 ;
            double dValY = 0.0 ;

            //플레이스 서브스트레이트 왼쪽 비전 포지션 
            dValX = PM.GetValue(mi.TOOL_XRght , pv.TOOL_XRghtVsSStgVsnLtM);
            dValY = PM.GetValue(mi.TOOL_YVisn , pv.TOOL_YVisnSStgVsnLtM  );
            dValX -= dMoveX ;
            dValY += dMoveY ;
            PM.SetValue(mi.TOOL_XRght , pv.TOOL_XRghtVsSStgVsnLtM , dValX);
            PM.SetValue(mi.TOOL_YVisn , pv.TOOL_YVisnSStgVsnLtM   , dValY);

            //플레이스 서브스트레이트 오른쪽 비전 포지션 
            dValX = PM.GetValue(mi.TOOL_XRght , pv.TOOL_XRghtVsSStgVsnRtM);
            dValY = PM.GetValue(mi.TOOL_YVisn , pv.TOOL_YVisnSStgVsnRtM  );
            dValX -= dMoveX ;
            dValY += dMoveY ;
            PM.SetValue(mi.TOOL_XRght , pv.TOOL_XRghtVsSStgVsnRtM , dValX);
            PM.SetValue(mi.TOOL_YVisn , pv.TOOL_YVisnSStgVsnRtM   , dValY);

            //디스펜서
            dValX = PM.GetValue(mi.TOOL_XRght , pv.TOOL_XRghtDispStt);
            dValY = PM.GetValue(mi.TOOL_YGent , pv.TOOL_YGentDispStt);
            dValX -= dMoveX ;
            dValY += dMoveY ;
            PM.SetValue(mi.TOOL_XRght , pv.TOOL_XRghtDispStt , dValX);
            PM.SetValue(mi.TOOL_YGent , pv.TOOL_YGentDispStt , dValY);

            //디스펜서 비전1
            dValX = PM.GetValue(mi.TOOL_XRght , pv.TOOL_XRghtDispVisn1);
            dValY = PM.GetValue(mi.TOOL_YGent , pv.TOOL_YGentDispVisn1);
            dValX -= dMoveX ;
            dValY += dMoveY ;
            PM.SetValue(mi.TOOL_XRght , pv.TOOL_XRghtDispVisn1 , dValX);
            PM.SetValue(mi.TOOL_YGent , pv.TOOL_YGentDispVisn1 , dValY);

            //디스펜서 비전2
            dValX = PM.GetValue(mi.TOOL_XRght , pv.TOOL_XRghtDispVisn2);
            dValY = PM.GetValue(mi.TOOL_YGent , pv.TOOL_YGentDispVisn2);
            dValX -= dMoveX ;
            dValY += dMoveY ;
            PM.SetValue(mi.TOOL_XRght , pv.TOOL_XRghtDispVisn2 , dValX);
            PM.SetValue(mi.TOOL_YGent , pv.TOOL_YGentDispVisn2 , dValY);

            //높이 측정 위치
            dValX = PM.GetValue(mi.TOOL_XRght , pv.TOOL_XRghtHghtStt);
            dValY = PM.GetValue(mi.TOOL_YGent , pv.TOOL_YGentHghtStt);
            dValX -= dMoveX ;
            dValY += dMoveY ;
            PM.SetValue(mi.TOOL_XRght , pv.TOOL_XRghtHghtStt , dValX);
            PM.SetValue(mi.TOOL_YGent , pv.TOOL_YGentHghtStt , dValY);

            //BLT 측정 위치
            dValX = PM.GetValue(mi.TOOL_XRght , pv.TOOL_XRghtBltStt);
            dValY = PM.GetValue(mi.TOOL_YGent , pv.TOOL_YGentBltStt);
            dValX -= dMoveX ;
            dValY += dMoveY ;
            PM.SetValue(mi.TOOL_XRght , pv.TOOL_XRghtBltStt , dValX);
            PM.SetValue(mi.TOOL_YGent , pv.TOOL_YGentBltStt , dValY);

            PM.Save(OM.GetCrntDev());
        }

        private void groupBox9_Enter(object sender, EventArgs e)
        {

        }

        private void btSaveDispPtrn_Click(object sender, EventArgs e)
        {
            SaveFileDialog sdSaveDispPtrn = new SaveFileDialog();
            sdSaveDispPtrn.Filter = "csv File|*.csv";
            sdSaveDispPtrn.Title = "Save";
            sdSaveDispPtrn.ShowDialog();

            if(sdSaveDispPtrn.FileName != ""){
                 SEQ.DispPtrn.SaveToCsv(sdSaveDispPtrn.FileName);

            }

        }

        private void btLoadDispPtrn_Click(object sender, EventArgs e)
        {
            OpenFileDialog odLoadDispPtrn = new OpenFileDialog();
            odLoadDispPtrn.Filter = "csv File|*.csv";
            odLoadDispPtrn.Title = "Open";
            odLoadDispPtrn.ShowDialog();

            if(odLoadDispPtrn.FileName != ""){
                SEQ.DispPtrn.LoadFromCsv(odLoadDispPtrn.FileName);
                tbDisprNodeCnt.Text = SEQ.DispPtrn.GetDispPosCnt().ToString() ;
                UpdatePattern(false);
            }
        }

        private void btSaveHghtPtrn_Click(object sender, EventArgs e)
        {
            SaveFileDialog sdSaveHghtPtrn = new SaveFileDialog();
            sdSaveHghtPtrn.Filter = "csv File|*.csv";
            sdSaveHghtPtrn.Title = "Save";
            sdSaveHghtPtrn.ShowDialog();

            if(sdSaveHghtPtrn.FileName != ""){
                 SEQ.HghtPtrn.SaveToCsv(sdSaveHghtPtrn.FileName);

            }
        }

        private void btLoadHghtPtrn_Click(object sender, EventArgs e)
        {
            OpenFileDialog odLoadHghtPtrn = new OpenFileDialog();
            odLoadHghtPtrn.Filter = "csv File|*.csv";
            odLoadHghtPtrn.Title = "Open";
            odLoadHghtPtrn.ShowDialog();

            if(odLoadHghtPtrn.FileName != ""){
                SEQ.HghtPtrn.LoadFromCsv(odLoadHghtPtrn.FileName);
                tbHghtNodeCnt.Text = SEQ.HghtPtrn.GetHghtPosCnt().ToString() ;
                UpdatePattern(false);
            }
        }

        private void btBLTSave_Click(object sender,EventArgs e) {
            SaveFileDialog sdSavePtrn = new SaveFileDialog();
            sdSavePtrn.Filter = "csv File|*.csv";
            sdSavePtrn.Title = "Save";
            sdSavePtrn.ShowDialog();

            if(sdSavePtrn.FileName != ""){
                 SEQ.BltPtrn.SaveToCsv(sdSavePtrn.FileName);

            }
        }

        private void btBLTLoad_Click(object sender,EventArgs e) {
            OpenFileDialog odLoadPtrn = new OpenFileDialog();
            odLoadPtrn.Filter = "csv File|*.csv";
            odLoadPtrn.Title = "Open";
            odLoadPtrn.ShowDialog();

            if(odLoadPtrn.FileName != ""){
                SEQ.HghtPtrn.LoadFromCsv(odLoadPtrn.FileName);
                tbHghtNodeCnt.Text = SEQ.HghtPtrn.GetHghtPosCnt().ToString() ;
                UpdatePattern(false);
            }
        }

        private void btBLTMove_Click(object sender,EventArgs e) {
            double dDist = CConfig.StrToDoubleDef( tbBLTMove.Text , 0.0);
            tbBLTMove.Text = dDist.ToString() ;

            double dVal = 0 ;
            int iNodeCnt = SEQ.BltPtrn.GetBltPosCnt() ;
            if(cbBLTMove.SelectedIndex == 0){
                for(int i = 0 ; i < iNodeCnt ; i++){
                    dVal =  SEQ.BltPtrn.BltPos[i].dDiePosX;
                    dVal += dDist ;
                    SEQ.BltPtrn.BltPos[i].dDiePosX = dVal;

                    dVal =  SEQ.BltPtrn.BltPos[i].dSubPosX;
                    dVal += dDist ;
                    SEQ.BltPtrn.BltPos[i].dSubPosX = dVal;
                }
            }
            else if(cbBLTMove.SelectedIndex == 1){
                for(int i = 0 ; i < iNodeCnt ; i++){
                    dVal =  SEQ.BltPtrn.BltPos[i].dDiePosY;
                    dVal += dDist ;
                    SEQ.BltPtrn.BltPos[i].dDiePosY = dVal;

                    dVal =  SEQ.BltPtrn.BltPos[i].dSubPosY;
                    dVal += dDist ;
                    SEQ.BltPtrn.BltPos[i].dSubPosY = dVal;
                }
            }

            UpdatePattern(true);
            pbDieAttach.Invalidate();
        }

        private void tpOptn_Click(object sender, EventArgs e)
        {

        }
    }    
}
