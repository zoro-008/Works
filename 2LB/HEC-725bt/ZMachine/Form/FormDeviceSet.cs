using System;
using System.Drawing;
using System.Windows.Forms;
using COMMON;
using SML;
using System.Reflection;

/*
 
         OUT_YGuid = 0 , //벨트방식 지름 25.87mm 1바퀴당 81.273mm 3.14159265358979
        OUT_TRelT = 1 , //Reel1 360도 
        OUT_TRelB = 2 , //Reel2 360도
        WRK_TFeed = 3 , //Roller Feeding Motor. 3.14159265358979 62.832
        WRK_YRott = 4 , //실 돌리는 모터.
        WRK_ZTabl = 5 , //실 써는 도마테이블 업다운.2.0mm
        WRK_TBlad = 6 , //칼날각도.
        WRK_TSwit = 7 , //칼날 써는 방향 선택
        WRK_YBlad = 8 , //칼날 In Out
 */
namespace Machine
{
    public partial class FormDeviceSet : Form
    {
        public        FraMotr    []       FraMotr      ;

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

            tbUserUnit.Text = "0.01";
            PstnDisp();

            OM.LoadLastInfo();
            PM.Load(OM.GetCrntDev().ToString());

            PM.UpdatePstn(true);
            UpdateDevInfo(true);

            SEQ.WRK.MakeWorkList();
            //Macro
            InitListView();
            UpdateWorkInfo();
            
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
                MOTION_DIR eDir = ML.MT_GetDirType((mi)m);
                FraMotr[m] = new FraMotr();
                FraMotr[m].SetIdType((mi)m, eDir);
                FraMotr[m].TopLevel = false;
                FraMotr[m].Parent = Ctrl[0];
                FraMotr[m].Show();
                FraMotr[m].SetUnit(EN_UNIT_TYPE.utJog, 0);

                //약간 위험.
                Ctrl[0].Parent.Text = ML.MT_GetName((mi)m);
            }

            //여기 AP텍에서만 쓰는거
            //실린더 버튼 AP텍꺼
            FraCylAPT = new FrameCylinderAPT[(int)ci.MAX_ACTR];
            for (int i = 0; i < (int)ci.MAX_ACTR; i++)
            {
                Control[] CtrlAP = tcDeviceSet.Controls.Find("pnActr" + i.ToString(), true);

                FraCylAPT[i] = new FrameCylinderAPT();
                FraCylAPT[i].TopLevel = false;
                FraCylAPT[i].SetConfig((ci)i, SM.CL.GetName(i).ToString(), ML.CL_GetDirType((ci)i), CtrlAP[0]);
                FraCylAPT[i].Show();
            }

            //Input Status 생성 AP텍꺼
            const int iInputBtnCnt  = 0;
            FraInputAPT = new FrameInputAPT[iInputBtnCnt];
            for (int i = 0; i < iInputBtnCnt; i++)
            {
                Control[] Ctrl = tcDeviceSet.Controls.Find("pnInput" + i.ToString(), true);
                
                int iIOCtrl = Convert.ToInt32(Ctrl[0].Tag);

                FraInputAPT[i] = new FrameInputAPT();
                FraInputAPT[i].TopLevel = false;
                FraInputAPT[i].SetConfig((xi)i, ML.IO_GetXName((xi)i), Ctrl[0]);
                FraInputAPT[i].Show();
            }

            //Output Status 생성 AP텍꺼
            const int iOutputBtnCnt = 0;
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
                    //case (int)yi.ETC_IonizerPower: FraOutputAPT[i].SetConfig(yi.ETC_IonizerPower, ML.IO_GetYName(yi.ETC_IonizerPower), Ctrl[0]); break;
                    //case (int)yi.ETC_IonizerSol  : FraOutputAPT[i].SetConfig(yi.ETC_IonizerSol  , ML.IO_GetYName(yi.ETC_IonizerSol  ), Ctrl[0]); break;
                    //case (int)yi.BARZ_PckrVac: FraOutputAPT[i].SetConfig(yi.BARZ_PckrVac, ML.IO_GetYName(yi.BARZ_PckrVac), Ctrl[0]); break;
                    //case (int)yi.BARZ_BrcdAC : FraOutputAPT[i].SetConfig(yi.BARZ_BrcdAC , ML.IO_GetYName(yi.BARZ_BrcdAC) , Ctrl[0]); break;
                    
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
                FraMotrPosAPT[i].SetWindow(i, Ctrl[0]);
                FraMotrPosAPT[i].Show();

            }
        }


        private void InitListView()
        {
            lvWorkInfo.Clear();
            lvWorkInfo.View = View.Details;
            //Lsv.LabelEdit = true;
            //Lsv.AllowColumnReorder = true;
            lvWorkInfo.FullRowSelect = true;
            lvWorkInfo.GridLines = true;
            //Lsv.Sorting = SortOrder.Descending;
            lvWorkInfo.Scrollable = true;
            //Need to Find
            lvWorkInfo.MultiSelect = false;
            //Lsv.HideSelection = false;

            lvWorkInfo.Columns.Add("NO"            , 50,  HorizontalAlignment.Left); //Day Time
            lvWorkInfo.Columns.Add("Abs피딩"       , 110, HorizontalAlignment.Left); //Day Time
            lvWorkInfo.Columns.Add("로테이션"      , 110, HorizontalAlignment.Left); //Day Time
            lvWorkInfo.Columns.Add("칼날진입각도"  , 110, HorizontalAlignment.Left); //Day Time
            lvWorkInfo.Columns.Add("가시세움각도"  , 110, HorizontalAlignment.Left); //Day Time
            lvWorkInfo.Columns.Add("스테이지높이"  , 110, HorizontalAlignment.Left); //Day Time
            lvWorkInfo.Columns.Add("스테이지대기"  , 110, HorizontalAlignment.Left); //Day Time
            lvWorkInfo.Columns.Add("방향선택"      , 110, HorizontalAlignment.Left); //Day Time
            lvWorkInfo.Columns.Add("칼날대기위치"  , 110, HorizontalAlignment.Left); //Day Time
            lvWorkInfo.Columns.Add("칼날작업위치"  , 110, HorizontalAlignment.Left); //Day Time

            
        }

        



        public void UpdateWorkInfo()
        {
            
            lvWorkInfo.BeginUpdate();
            lvWorkInfo.Items.Clear ();
            ListViewItem item;
            /*
            Lsv1.Columns.Add("NO"            , 50,  HorizontalAlignment.Left); //Day Time
            Lsv1.Columns.Add("Abs피딩"       , 100, HorizontalAlignment.Left); //Day Time double dAbsFeed    ;
            Lsv1.Columns.Add("로테이션"      , 100, HorizontalAlignment.Left); //Day Time double dRotPos     ;
            Lsv1.Columns.Add("칼날진입각도"  , 100, HorizontalAlignment.Left); //Day Time double dCog        ;
            Lsv1.Columns.Add("가시세움각도"  , 100, HorizontalAlignment.Left); //Day Time double dCogUp      ;
            Lsv1.Columns.Add("스테이지높이"  , 100, HorizontalAlignment.Left); //Day Time double dTableWork  ;
            Lsv1.Columns.Add("방향선택"      , 100, HorizontalAlignment.Left); //Day Time double dSwitchWork ;
             */
            for (int j = 0; j < SEQ.WRK.lsWorkList.Count ; j++)
            {
                item = new ListViewItem(j.ToString());
                item.SubItems.Add(SEQ.WRK.lsWorkList[j].dAbsFeed   .ToString());
                item.SubItems.Add(SEQ.WRK.lsWorkList[j].dRotPos    .ToString());
                item.SubItems.Add(SEQ.WRK.lsWorkList[j].dCog       .ToString());
                item.SubItems.Add(SEQ.WRK.lsWorkList[j].dCogUp     .ToString());
                item.SubItems.Add(SEQ.WRK.lsWorkList[j].dTableWork .ToString());
                item.SubItems.Add(SEQ.WRK.lsWorkList[j].dTableWait .ToString());
                item.SubItems.Add(SEQ.WRK.lsWorkList[j].dSwitchWork.ToString());   
                item.SubItems.Add(SEQ.WRK.lsWorkList[j].dBladBfWork.ToString());   
                item.SubItems.Add(SEQ.WRK.lsWorkList[j].dBladWork  .ToString());   

                lvWorkInfo.Items.Add(item);
            }
            lvWorkInfo.EndUpdate();

        }

        public void PstnDisp()
        {  
            //OUT_YGuid                                 
            PM.SetProp((uint)mi.OUT_YGuid , (uint)pv.OUT_YGuidWait      , "Wait          ", false, false, true );
            PM.SetProp((uint)mi.OUT_YGuid , (uint)pv.OUT_YGuidFrnt      , "FrontEnd      ", false, false, true );
            PM.SetProp((uint)mi.OUT_YGuid , (uint)pv.OUT_YGuidRear      , "RearEnd       ", false, false, true );
            PM.SetCheckSafe((uint)mi.OUT_YGuid , SEQ.OUT.CheckSafe);    
                                                                        
            //OUT_TRelT                                                                     
            PM.SetProp((uint)mi.OUT_TRelT, (uint)pv.OUT_TRelTWait       , "Wait          ", false, false, true );
            PM.SetCheckSafe((uint)mi.OUT_TRelT, SEQ.OUT.CheckSafe);     
                                                                        
            //OUT_TRelB                                                                      
            PM.SetProp((uint)mi.OUT_TRelB  , (uint)pv.OUT_TRelBWait     , "Wait          ", false, false, true );
            PM.SetCheckSafe((uint)mi.OUT_TRelB, SEQ.OUT.CheckSafe);

            //WRK_TFeed                                                                       
            PM.SetProp((uint)mi.WRK_TFeed, (uint)pv.WRK_TFeedWait       , "Wait          ", false, false, true );
            PM.SetProp((uint)mi.WRK_TFeed, (uint)pv.WRK_TFeed0SpareOfs  , "0 스페어 간격 ", true , false, false);
            PM.SetProp((uint)mi.WRK_TFeed, (uint)pv.WRK_TFeed0CogLengOfs, "0 코그 길이   ", true , false, false);
            PM.SetProp((uint)mi.WRK_TFeed, (uint)pv.WRK_TFeed0CogGapOfs , "0 코그 간격   ", true , false, false);
            PM.SetProp((uint)mi.WRK_TFeed, (uint)pv.WRK_TFeed1SpareOfs  , "1 스페어 간격 ", true , false, false);
            PM.SetProp((uint)mi.WRK_TFeed, (uint)pv.WRK_TFeed1CogLengOfs, "1 코그 길이   ", true , false, false);
            PM.SetProp((uint)mi.WRK_TFeed, (uint)pv.WRK_TFeed1CogGapOfs , "1 코그 간격   ", true , false, false);
            PM.SetProp((uint)mi.WRK_TFeed, (uint)pv.WRK_TFeed2SpareOfs  , "2 스페어 간격 ", true , false, false);
            PM.SetProp((uint)mi.WRK_TFeed, (uint)pv.WRK_TFeed2CogLengOfs, "2 코그 길이   ", true , false, false);
            PM.SetProp((uint)mi.WRK_TFeed, (uint)pv.WRK_TFeed2CogGapOfs , "2 코그 간격   ", true , false, false);
            PM.SetProp((uint)mi.WRK_TFeed, (uint)pv.WRK_TFeed3SpareOfs  , "3 스페어 간격 ", true , false, false);
            PM.SetProp((uint)mi.WRK_TFeed, (uint)pv.WRK_TFeed3CogLengOfs, "3 코그 길이   ", true , false, false);
            PM.SetProp((uint)mi.WRK_TFeed, (uint)pv.WRK_TFeed3CogGapOfs , "3 코그 간격   ", true , false, false);
            PM.SetProp((uint)mi.WRK_TFeed, (uint)pv.WRK_TFeed4SpareOfs  , "4 스페어 간격 ", true , false, false);
            PM.SetProp((uint)mi.WRK_TFeed, (uint)pv.WRK_TFeed4CogLengOfs, "4 코그 길이   ", true , false, false);
            PM.SetProp((uint)mi.WRK_TFeed, (uint)pv.WRK_TFeed4CogGapOfs , "4 코그 간격   ", true , false, false);     
            PM.SetProp((uint)mi.WRK_TFeed, (uint)pv.WRK_TFeed5SpareOfs  , "5 스페어 간격 ", true , false, false);
            PM.SetProp((uint)mi.WRK_TFeed, (uint)pv.WRK_TFeed5CogLengOfs, "5 코그 길이   ", true , false, false);
            PM.SetProp((uint)mi.WRK_TFeed, (uint)pv.WRK_TFeed5CogGapOfs , "5 코그 간격   ", true , false, false);
            PM.SetProp((uint)mi.WRK_TFeed, (uint)pv.WRK_TFeed6SpareOfs  , "6 스페어 간격 ", true , false, false);
            PM.SetProp((uint)mi.WRK_TFeed, (uint)pv.WRK_TFeed6CogLengOfs, "6 코그 길이   ", true , false, false);
            PM.SetProp((uint)mi.WRK_TFeed, (uint)pv.WRK_TFeed6CogGapOfs , "6 코그 간격   ", true , false, false);
            PM.SetProp((uint)mi.WRK_TFeed, (uint)pv.WRK_TFeed7SpareOfs  , "7 스페어 간격 ", true , false, false);
            PM.SetProp((uint)mi.WRK_TFeed, (uint)pv.WRK_TFeed7CogLengOfs, "7 코그 길이   ", true , false, false);
            PM.SetProp((uint)mi.WRK_TFeed, (uint)pv.WRK_TFeed7CogGapOfs , "7 코그 간격   ", true , false, false);
            PM.SetProp((uint)mi.WRK_TFeed, (uint)pv.WRK_TFeed8SpareOfs  , "8 스페어 간격 ", true , false, false);
            PM.SetProp((uint)mi.WRK_TFeed, (uint)pv.WRK_TFeed8CogLengOfs, "8 코그 길이   ", true , false, false);
            PM.SetProp((uint)mi.WRK_TFeed, (uint)pv.WRK_TFeed8CogGapOfs , "8 코그 간격   ", true , false, false);
            PM.SetProp((uint)mi.WRK_TFeed, (uint)pv.WRK_TFeed9SpareOfs  , "9 스페어 간격 ", true , false, false);
            PM.SetProp((uint)mi.WRK_TFeed, (uint)pv.WRK_TFeed9CogLengOfs, "9 코그 길이   ", true , false, false);
            PM.SetProp((uint)mi.WRK_TFeed, (uint)pv.WRK_TFeed9CogGapOfs , "9 코그 간격   ", true , false, false);

            PM.SetProp((uint)mi.WRK_TFeed, (uint)pv.WRK_TFeedRepeatOfs  , "반복간 간격   ", true , false, false);
            PM.SetProp((uint)mi.WRK_TFeed, (uint)pv.WRK_TFeedEndOfs     , "제품간 간격   ", true , false, false);
            PM.SetCheckSafe((uint)mi.WRK_TFeed, SEQ.WRK.CheckSafe);

			
			//WRK_YRott 
			PM.SetProp((uint)mi.WRK_YRott, (uint)pv.WRK_YRottWait       , "Wait          ", false, false, true );
            PM.SetProp((uint)mi.WRK_YRott, (uint)pv.WRK_YRottMoveOfs    , "상/하 이동량  ", true , false, false);
            PM.SetProp((uint)mi.WRK_YRott, (uint)pv.WRK_YRottMoveEnd    , "하 이동제한   ", false, false, false);
            PM.SetCheckSafe((uint)mi.WRK_YRott, SEQ.WRK.CheckSafe);     
                                                                        
            //WRK_ZTabl                                                 
            PM.SetProp((uint)mi.WRK_ZTabl, (uint)pv.WRK_ZTablWait       , "Wait          ", false, false, true );
            PM.SetProp((uint)mi.WRK_ZTabl, (uint)pv.WRK_ZTabl0Wait      , "0 스테이지대기", false, false, false);
            PM.SetProp((uint)mi.WRK_ZTabl, (uint)pv.WRK_ZTabl1Wait      , "1 스테이지대기", false, false, false);
            PM.SetProp((uint)mi.WRK_ZTabl, (uint)pv.WRK_ZTabl2Wait      , "2 스테이지대기", false, false, false);
            PM.SetProp((uint)mi.WRK_ZTabl, (uint)pv.WRK_ZTabl3Wait      , "3 스테이지대기", false, false, false);
            PM.SetProp((uint)mi.WRK_ZTabl, (uint)pv.WRK_ZTabl4Wait      , "4 스테이지대기", false, false, false);
            PM.SetProp((uint)mi.WRK_ZTabl, (uint)pv.WRK_ZTabl5Wait      , "5 스테이지대기", false, false, false);
            PM.SetProp((uint)mi.WRK_ZTabl, (uint)pv.WRK_ZTabl6Wait      , "6 스테이지대기", false, false, false);
            PM.SetProp((uint)mi.WRK_ZTabl, (uint)pv.WRK_ZTabl7Wait      , "7 스테이지대기", false, false, false);
            PM.SetProp((uint)mi.WRK_ZTabl, (uint)pv.WRK_ZTabl8Wait      , "8 스테이지대기", false, false, false);
            PM.SetProp((uint)mi.WRK_ZTabl, (uint)pv.WRK_ZTabl9Wait      , "9 스테이지대기", false, false, false);
            PM.SetProp((uint)mi.WRK_ZTabl, (uint)pv.WRK_ZTabl0Work      , "0 스테이지높이", false, false, false);
            PM.SetProp((uint)mi.WRK_ZTabl, (uint)pv.WRK_ZTabl1Work      , "1 스테이지높이", false, false, false);
            PM.SetProp((uint)mi.WRK_ZTabl, (uint)pv.WRK_ZTabl2Work      , "2 스테이지높이", false, false, false);
            PM.SetProp((uint)mi.WRK_ZTabl, (uint)pv.WRK_ZTabl3Work      , "3 스테이지높이", false, false, false);
            PM.SetProp((uint)mi.WRK_ZTabl, (uint)pv.WRK_ZTabl4Work      , "4 스테이지높이", false, false, false);
            PM.SetProp((uint)mi.WRK_ZTabl, (uint)pv.WRK_ZTabl5Work      , "5 스테이지높이", false, false, false);
            PM.SetProp((uint)mi.WRK_ZTabl, (uint)pv.WRK_ZTabl6Work      , "6 스테이지높이", false, false, false);
            PM.SetProp((uint)mi.WRK_ZTabl, (uint)pv.WRK_ZTabl7Work      , "7 스테이지높이", false, false, false);
            PM.SetProp((uint)mi.WRK_ZTabl, (uint)pv.WRK_ZTabl8Work      , "8 스테이지높이", false, false, false);
            PM.SetProp((uint)mi.WRK_ZTabl, (uint)pv.WRK_ZTabl9Work      , "9 스테이지높이", false, false, false);
            PM.SetCheckSafe((uint)mi.WRK_ZTabl, SEQ.WRK.CheckSafe);     
                                                                        
            //WRK_TBlad                                                 
			PM.SetProp((uint)mi.WRK_TBlad, (uint)pv.WRK_TBladWait       , "Wait          ", false, false, true );       
            PM.SetProp((uint)mi.WRK_TBlad, (uint)pv.WRK_TBlad0Cog       , "0 칼날세움각도", false, false, false);       
            PM.SetProp((uint)mi.WRK_TBlad, (uint)pv.WRK_TBlad0CogUp     , "0 칼날높힘각도", false, false, false);       
            PM.SetProp((uint)mi.WRK_TBlad, (uint)pv.WRK_TBlad1Cog       , "1 칼날세움각도", false, false, false);       
            PM.SetProp((uint)mi.WRK_TBlad, (uint)pv.WRK_TBlad1CogUp     , "1 칼날높힘각도", false, false, false);       
            PM.SetProp((uint)mi.WRK_TBlad, (uint)pv.WRK_TBlad2Cog       , "2 칼날세움각도", false, false, false);       
            PM.SetProp((uint)mi.WRK_TBlad, (uint)pv.WRK_TBlad2CogUp     , "2 칼날높힘각도", false, false, false);
            PM.SetProp((uint)mi.WRK_TBlad, (uint)pv.WRK_TBlad3Cog       , "3 칼날세움각도", false, false, false);       
            PM.SetProp((uint)mi.WRK_TBlad, (uint)pv.WRK_TBlad3CogUp     , "3 칼날높힘각도", false, false, false);       
            PM.SetProp((uint)mi.WRK_TBlad, (uint)pv.WRK_TBlad4Cog       , "4 칼날세움각도", false, false, false);       
            PM.SetProp((uint)mi.WRK_TBlad, (uint)pv.WRK_TBlad4CogUp     , "4 칼날높힘각도", false, false, false);
            PM.SetProp((uint)mi.WRK_TBlad, (uint)pv.WRK_TBlad5Cog       , "5 칼날세움각도", false, false, false);       
            PM.SetProp((uint)mi.WRK_TBlad, (uint)pv.WRK_TBlad5CogUp     , "5 칼날높힘각도", false, false, false);
            PM.SetProp((uint)mi.WRK_TBlad, (uint)pv.WRK_TBlad6Cog       , "6 칼날세움각도", false, false, false);       
            PM.SetProp((uint)mi.WRK_TBlad, (uint)pv.WRK_TBlad6CogUp     , "6 칼날높힘각도", false, false, false);
            PM.SetProp((uint)mi.WRK_TBlad, (uint)pv.WRK_TBlad7Cog       , "7 칼날세움각도", false, false, false);       
            PM.SetProp((uint)mi.WRK_TBlad, (uint)pv.WRK_TBlad7CogUp     , "7 칼날높힘각도", false, false, false);
            PM.SetProp((uint)mi.WRK_TBlad, (uint)pv.WRK_TBlad8Cog       , "8 칼날세움각도", false, false, false);       
            PM.SetProp((uint)mi.WRK_TBlad, (uint)pv.WRK_TBlad8CogUp     , "8 칼날높힘각도", false, false, false);
            PM.SetProp((uint)mi.WRK_TBlad, (uint)pv.WRK_TBlad9Cog       , "9 칼날세움각도", false, false, false);       
            PM.SetProp((uint)mi.WRK_TBlad, (uint)pv.WRK_TBlad9CogUp     , "9 칼날높힘각도", false, false, false);
            PM.SetCheckSafe((uint)mi.WRK_TBlad, SEQ.WRK.CheckSafe);     
                                                                        
            //WRK_TSwit                                                 
			PM.SetProp((uint)mi.WRK_TSwit, (uint)pv.WRK_TSwitWait       , "Wait          ", false, false, true );
            PM.SetProp((uint)mi.WRK_TSwit, (uint)pv.WRK_TSwitLWork      , "L Work        ", false, false, true );
            PM.SetProp((uint)mi.WRK_TSwit, (uint)pv.WRK_TSwitRWork      , "R Work        ", false, false, true );
            PM.SetCheckSafe((uint)mi.WRK_TSwit, SEQ.WRK.CheckSafe);     
                                                                        
                                                                        
            //WRK_YBlad                                                 
			PM.SetProp((uint)mi.WRK_YBlad, (uint)pv.WRK_YBladWait       , "Wait          ", false, false, true );
            PM.SetProp((uint)mi.WRK_YBlad, (uint)pv.WRK_YBlad0BfWork    , "0 커팅 대기   ", false, false, true );
            PM.SetProp((uint)mi.WRK_YBlad, (uint)pv.WRK_YBlad1BfWork    , "1 커팅 대기   ", false, false, true );
            PM.SetProp((uint)mi.WRK_YBlad, (uint)pv.WRK_YBlad2BfWork    , "2 커팅 대기   ", false, false, true );
            PM.SetProp((uint)mi.WRK_YBlad, (uint)pv.WRK_YBlad3BfWork    , "3 커팅 대기   ", false, false, true );
            PM.SetProp((uint)mi.WRK_YBlad, (uint)pv.WRK_YBlad4BfWork    , "4 커팅 대기   ", false, false, true );
            PM.SetProp((uint)mi.WRK_YBlad, (uint)pv.WRK_YBlad5BfWork    , "5 커팅 대기   ", false, false, true );
            PM.SetProp((uint)mi.WRK_YBlad, (uint)pv.WRK_YBlad6BfWork    , "6 커팅 대기   ", false, false, true );
            PM.SetProp((uint)mi.WRK_YBlad, (uint)pv.WRK_YBlad7BfWork    , "7 커팅 대기   ", false, false, true );
            PM.SetProp((uint)mi.WRK_YBlad, (uint)pv.WRK_YBlad8BfWork    , "8 커팅 대기   ", false, false, true );
            PM.SetProp((uint)mi.WRK_YBlad, (uint)pv.WRK_YBlad9BfWork    , "9 커팅 대기   ", false, false, true );

            PM.SetProp((uint)mi.WRK_YBlad, (uint)pv.WRK_YBlad0Work      , "0 커팅        ", false, false, false);
            PM.SetProp((uint)mi.WRK_YBlad, (uint)pv.WRK_YBlad1Work      , "1 커팅        ", false, false, false);
            PM.SetProp((uint)mi.WRK_YBlad, (uint)pv.WRK_YBlad2Work      , "2 커팅        ", false, false, false);
            PM.SetProp((uint)mi.WRK_YBlad, (uint)pv.WRK_YBlad3Work      , "3 커팅        ", false, false, false);
            PM.SetProp((uint)mi.WRK_YBlad, (uint)pv.WRK_YBlad4Work      , "4 커팅        ", false, false, false);
            PM.SetProp((uint)mi.WRK_YBlad, (uint)pv.WRK_YBlad5Work      , "5 커팅        ", false, false, false);
            PM.SetProp((uint)mi.WRK_YBlad, (uint)pv.WRK_YBlad6Work      , "6 커팅        ", false, false, false);
            PM.SetProp((uint)mi.WRK_YBlad, (uint)pv.WRK_YBlad7Work      , "7 커팅        ", false, false, false);
            PM.SetProp((uint)mi.WRK_YBlad, (uint)pv.WRK_YBlad8Work      , "8 커팅        ", false, false, false);
            PM.SetProp((uint)mi.WRK_YBlad, (uint)pv.WRK_YBlad9Work      , "9 커팅        ", false, false, false);

            PM.SetCheckSafe((uint)mi.WRK_YBlad, SEQ.WRK.CheckSafe);     
        }

        public void UpdateDevInfo(bool bToTable)
        {
            double dTemp = 0.0 ;
            if (bToTable)
            {
                //DeviceInfo
                CConfig.ValToCon(tbPattern       , ref OM.DevOptn.sPattern       );
                CConfig.ValToCon(tbRepeatCnt     , ref OM.DevOptn.iRepeatCnt     );
                CConfig.ValToCon(cbCogRotation   , ref OM.DevOptn.iCogRotation   );
                CConfig.ValToCon(tbLeftRightDist , ref OM.DevOptn.dLeftRightDist );
                CConfig.ValToCon(tbLotWorkCount  , ref OM.DevOptn.iLotWorkCount  );
                CConfig.ValToCon(tbGuidStopDelay , ref OM.DevOptn.iGuidStopDealy );

                //인덱스 옵션
                dTemp=PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed0SpareOfs  ); CConfig.ValToCon(tbWRK_TFeed0SpareOfs   , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed0CogLengOfs); CConfig.ValToCon(tbWRK_TFeed0CogLengOfs , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed0CogGapOfs ); CConfig.ValToCon(tbWRK_TFeed0CogGapOfs  , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_TBlad , pv.WRK_TBlad0Cog       ); CConfig.ValToCon(tbWRK_TBlad0Cog        , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_TBlad , pv.WRK_TBlad0CogUp     ); CConfig.ValToCon(tbWRK_TBlad0CogUp      , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_ZTabl , pv.WRK_ZTabl0Work      ); CConfig.ValToCon(tbWRK_ZTabl0Work       , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_ZTabl , pv.WRK_ZTabl0Wait      ); CConfig.ValToCon(tbWRK_ZTabl0Wait       , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_YBlad , pv.WRK_YBlad0Work      ); CConfig.ValToCon(tbWRK_YBlad0Work       , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_YBlad , pv.WRK_YBlad0BfWork    ); CConfig.ValToCon(tbWRK_YBlad0BfWork     , ref dTemp);
                CConfig.ValToCon(cbWRK_TSwit0Work, ref OM.DevOptn.iWRK_TSwit0Work );

                dTemp=PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed1SpareOfs  ); CConfig.ValToCon(tbWRK_TFeed1SpareOfs   , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed1CogLengOfs); CConfig.ValToCon(tbWRK_TFeed1CogLengOfs , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed1CogGapOfs ); CConfig.ValToCon(tbWRK_TFeed1CogGapOfs  , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_TBlad , pv.WRK_TBlad1Cog       ); CConfig.ValToCon(tbWRK_TBlad1Cog        , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_TBlad , pv.WRK_TBlad1CogUp     ); CConfig.ValToCon(tbWRK_TBlad1CogUp      , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_ZTabl , pv.WRK_ZTabl1Work      ); CConfig.ValToCon(tbWRK_ZTabl1Work       , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_ZTabl , pv.WRK_ZTabl1Wait      ); CConfig.ValToCon(tbWRK_ZTabl1Wait       , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_YBlad , pv.WRK_YBlad1Work      ); CConfig.ValToCon(tbWRK_YBlad1Work       , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_YBlad , pv.WRK_YBlad1BfWork    ); CConfig.ValToCon(tbWRK_YBlad1BfWork     , ref dTemp);
                CConfig.ValToCon(cbWRK_TSwit1Work, ref OM.DevOptn.iWRK_TSwit1Work );
                                                                     
                dTemp=PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed2SpareOfs  ); CConfig.ValToCon(tbWRK_TFeed2SpareOfs   , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed2CogLengOfs); CConfig.ValToCon(tbWRK_TFeed2CogLengOfs , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed2CogGapOfs ); CConfig.ValToCon(tbWRK_TFeed2CogGapOfs  , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_TBlad , pv.WRK_TBlad2Cog       ); CConfig.ValToCon(tbWRK_TBlad2Cog        , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_TBlad , pv.WRK_TBlad2CogUp     ); CConfig.ValToCon(tbWRK_TBlad2CogUp      , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_ZTabl , pv.WRK_ZTabl2Work      ); CConfig.ValToCon(tbWRK_ZTabl2Work       , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_ZTabl , pv.WRK_ZTabl2Wait      ); CConfig.ValToCon(tbWRK_ZTabl2Wait       , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_YBlad , pv.WRK_YBlad2Work      ); CConfig.ValToCon(tbWRK_YBlad2Work       , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_YBlad , pv.WRK_YBlad2BfWork    ); CConfig.ValToCon(tbWRK_YBlad2BfWork     , ref dTemp);
                CConfig.ValToCon(cbWRK_TSwit2Work, ref OM.DevOptn.iWRK_TSwit2Work );

                dTemp=PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed3SpareOfs  ); CConfig.ValToCon(tbWRK_TFeed3SpareOfs   , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed3CogLengOfs); CConfig.ValToCon(tbWRK_TFeed3CogLengOfs , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed3CogGapOfs ); CConfig.ValToCon(tbWRK_TFeed3CogGapOfs  , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_TBlad , pv.WRK_TBlad3Cog       ); CConfig.ValToCon(tbWRK_TBlad3Cog        , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_TBlad , pv.WRK_TBlad3CogUp     ); CConfig.ValToCon(tbWRK_TBlad3CogUp      , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_ZTabl , pv.WRK_ZTabl3Work      ); CConfig.ValToCon(tbWRK_ZTabl3Work       , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_ZTabl , pv.WRK_ZTabl3Wait      ); CConfig.ValToCon(tbWRK_ZTabl3Wait       , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_YBlad , pv.WRK_YBlad3Work      ); CConfig.ValToCon(tbWRK_YBlad3Work       , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_YBlad , pv.WRK_YBlad3BfWork    ); CConfig.ValToCon(tbWRK_YBlad3BfWork     , ref dTemp);
                CConfig.ValToCon(cbWRK_TSwit3Work, ref OM.DevOptn.iWRK_TSwit3Work );

                dTemp=PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed4SpareOfs  ); CConfig.ValToCon(tbWRK_TFeed4SpareOfs   , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed4CogLengOfs); CConfig.ValToCon(tbWRK_TFeed4CogLengOfs , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed4CogGapOfs ); CConfig.ValToCon(tbWRK_TFeed4CogGapOfs  , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_TBlad , pv.WRK_TBlad4Cog       ); CConfig.ValToCon(tbWRK_TBlad4Cog        , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_TBlad , pv.WRK_TBlad4CogUp     ); CConfig.ValToCon(tbWRK_TBlad4CogUp      , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_ZTabl , pv.WRK_ZTabl4Work      ); CConfig.ValToCon(tbWRK_ZTabl4Work       , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_ZTabl , pv.WRK_ZTabl4Wait      ); CConfig.ValToCon(tbWRK_ZTabl4Wait       , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_YBlad , pv.WRK_YBlad4Work      ); CConfig.ValToCon(tbWRK_YBlad4Work       , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_YBlad , pv.WRK_YBlad4BfWork    ); CConfig.ValToCon(tbWRK_YBlad4BfWork     , ref dTemp);
                CConfig.ValToCon(cbWRK_TSwit4Work, ref OM.DevOptn.iWRK_TSwit4Work );


                dTemp=PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed5SpareOfs  ); CConfig.ValToCon(tbWRK_TFeed5SpareOfs   , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed5CogLengOfs); CConfig.ValToCon(tbWRK_TFeed5CogLengOfs , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed5CogGapOfs ); CConfig.ValToCon(tbWRK_TFeed5CogGapOfs  , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_TBlad , pv.WRK_TBlad5Cog       ); CConfig.ValToCon(tbWRK_TBlad5Cog        , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_TBlad , pv.WRK_TBlad5CogUp     ); CConfig.ValToCon(tbWRK_TBlad5CogUp      , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_ZTabl , pv.WRK_ZTabl5Work      ); CConfig.ValToCon(tbWRK_ZTabl5Work       , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_ZTabl , pv.WRK_ZTabl5Wait      ); CConfig.ValToCon(tbWRK_ZTabl5Wait       , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_YBlad , pv.WRK_YBlad5Work      ); CConfig.ValToCon(tbWRK_YBlad5Work       , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_YBlad , pv.WRK_YBlad5BfWork    ); CConfig.ValToCon(tbWRK_YBlad5BfWork     , ref dTemp);
                CConfig.ValToCon(cbWRK_TSwit5Work, ref OM.DevOptn.iWRK_TSwit5Work );


                dTemp=PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed6SpareOfs  ); CConfig.ValToCon(tbWRK_TFeed6SpareOfs   , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed6CogLengOfs); CConfig.ValToCon(tbWRK_TFeed6CogLengOfs , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed6CogGapOfs ); CConfig.ValToCon(tbWRK_TFeed6CogGapOfs  , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_TBlad , pv.WRK_TBlad6Cog       ); CConfig.ValToCon(tbWRK_TBlad6Cog        , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_TBlad , pv.WRK_TBlad6CogUp     ); CConfig.ValToCon(tbWRK_TBlad6CogUp      , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_ZTabl , pv.WRK_ZTabl6Work      ); CConfig.ValToCon(tbWRK_ZTabl6Work       , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_ZTabl , pv.WRK_ZTabl6Wait      ); CConfig.ValToCon(tbWRK_ZTabl6Wait       , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_YBlad , pv.WRK_YBlad6Work      ); CConfig.ValToCon(tbWRK_YBlad6Work       , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_YBlad , pv.WRK_YBlad6BfWork    ); CConfig.ValToCon(tbWRK_YBlad6BfWork     , ref dTemp);
                CConfig.ValToCon(cbWRK_TSwit6Work, ref OM.DevOptn.iWRK_TSwit6Work );

                dTemp=PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed7SpareOfs  ); CConfig.ValToCon(tbWRK_TFeed7SpareOfs   , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed7CogLengOfs); CConfig.ValToCon(tbWRK_TFeed7CogLengOfs , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed7CogGapOfs ); CConfig.ValToCon(tbWRK_TFeed7CogGapOfs  , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_TBlad , pv.WRK_TBlad7Cog       ); CConfig.ValToCon(tbWRK_TBlad7Cog        , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_TBlad , pv.WRK_TBlad7CogUp     ); CConfig.ValToCon(tbWRK_TBlad7CogUp      , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_ZTabl , pv.WRK_ZTabl7Work      ); CConfig.ValToCon(tbWRK_ZTabl7Work       , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_ZTabl , pv.WRK_ZTabl7Wait      ); CConfig.ValToCon(tbWRK_ZTabl7Wait       , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_YBlad , pv.WRK_YBlad7Work      ); CConfig.ValToCon(tbWRK_YBlad7Work       , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_YBlad , pv.WRK_YBlad7BfWork    ); CConfig.ValToCon(tbWRK_YBlad7BfWork     , ref dTemp);
                CConfig.ValToCon(cbWRK_TSwit7Work, ref OM.DevOptn.iWRK_TSwit7Work );


                dTemp=PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed8SpareOfs  ); CConfig.ValToCon(tbWRK_TFeed8SpareOfs   , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed8CogLengOfs); CConfig.ValToCon(tbWRK_TFeed8CogLengOfs , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed8CogGapOfs ); CConfig.ValToCon(tbWRK_TFeed8CogGapOfs  , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_TBlad , pv.WRK_TBlad8Cog       ); CConfig.ValToCon(tbWRK_TBlad8Cog        , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_TBlad , pv.WRK_TBlad8CogUp     ); CConfig.ValToCon(tbWRK_TBlad8CogUp      , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_ZTabl , pv.WRK_ZTabl8Work      ); CConfig.ValToCon(tbWRK_ZTabl8Work       , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_ZTabl , pv.WRK_ZTabl8Wait      ); CConfig.ValToCon(tbWRK_ZTabl8Wait       , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_YBlad , pv.WRK_YBlad8Work      ); CConfig.ValToCon(tbWRK_YBlad8Work       , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_YBlad , pv.WRK_YBlad8BfWork    ); CConfig.ValToCon(tbWRK_YBlad8BfWork     , ref dTemp);
                CConfig.ValToCon(cbWRK_TSwit8Work, ref OM.DevOptn.iWRK_TSwit8Work );

                dTemp=PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed9SpareOfs  ); CConfig.ValToCon(tbWRK_TFeed9SpareOfs   , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed9CogLengOfs); CConfig.ValToCon(tbWRK_TFeed9CogLengOfs , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeed9CogGapOfs ); CConfig.ValToCon(tbWRK_TFeed9CogGapOfs  , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_TBlad , pv.WRK_TBlad9Cog       ); CConfig.ValToCon(tbWRK_TBlad9Cog        , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_TBlad , pv.WRK_TBlad9CogUp     ); CConfig.ValToCon(tbWRK_TBlad9CogUp      , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_ZTabl , pv.WRK_ZTabl9Work      ); CConfig.ValToCon(tbWRK_ZTabl9Work       , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_ZTabl , pv.WRK_ZTabl9Wait      ); CConfig.ValToCon(tbWRK_ZTabl9Wait       , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_YBlad , pv.WRK_YBlad9Work      ); CConfig.ValToCon(tbWRK_YBlad9Work       , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_YBlad , pv.WRK_YBlad9BfWork    ); CConfig.ValToCon(tbWRK_YBlad9BfWork     , ref dTemp);
                CConfig.ValToCon(cbWRK_TSwit9Work, ref OM.DevOptn.iWRK_TSwit9Work );



                dTemp=PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeedRepeatOfs  ); CConfig.ValToCon(tbWRK_TFeedRepeatOfs   , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_TFeed , pv.WRK_TFeedEndOfs     ); CConfig.ValToCon(tbWRK_TFeedEndOfs      , ref dTemp);

                
                

                dTemp=PM.GetValue(mi.WRK_YRott , pv.WRK_YRottMoveOfs    ); CConfig.ValToCon(tbWRK_YRottMoveOfs     , ref dTemp);
                dTemp=PM.GetValue(mi.WRK_YRott , pv.WRK_YRottMoveEnd    ); CConfig.ValToCon(tbWRK_YRottMoveEnd     , ref dTemp);               
            }
            else 
            {
                //DeviceInfo
                OM.CDevInfo DevInfo = OM.DevInfo;
                
                CConfig.ConToVal(tbPattern       , ref OM.DevOptn.sPattern       );
                CConfig.ConToVal(tbRepeatCnt     , ref OM.DevOptn.iRepeatCnt     );                
                CConfig.ConToVal(cbCogRotation   , ref OM.DevOptn.iCogRotation   );
                CConfig.ConToVal(tbLeftRightDist , ref OM.DevOptn.dLeftRightDist );
                CConfig.ConToVal(tbLotWorkCount  , ref OM.DevOptn.iLotWorkCount  );
                CConfig.ConToVal(tbGuidStopDelay , ref OM.DevOptn.iGuidStopDealy );

                CConfig.ConToVal(tbWRK_TFeed0SpareOfs   , ref dTemp); PM.SetValue(mi.WRK_TFeed , pv.WRK_TFeed0SpareOfs   , dTemp);
                CConfig.ConToVal(tbWRK_TFeed0CogLengOfs , ref dTemp); PM.SetValue(mi.WRK_TFeed , pv.WRK_TFeed0CogLengOfs , dTemp);
                CConfig.ConToVal(tbWRK_TFeed0CogGapOfs  , ref dTemp); PM.SetValue(mi.WRK_TFeed , pv.WRK_TFeed0CogGapOfs  , dTemp);
                CConfig.ConToVal(tbWRK_TBlad0Cog        , ref dTemp); PM.SetValue(mi.WRK_TBlad , pv.WRK_TBlad0Cog        , dTemp);
                CConfig.ConToVal(tbWRK_TBlad0CogUp      , ref dTemp); PM.SetValue(mi.WRK_TBlad , pv.WRK_TBlad0CogUp      , dTemp);
                CConfig.ConToVal(tbWRK_ZTabl0Work       , ref dTemp); PM.SetValue(mi.WRK_ZTabl , pv.WRK_ZTabl0Work       , dTemp);
                CConfig.ConToVal(tbWRK_ZTabl0Wait       , ref dTemp); PM.SetValue(mi.WRK_ZTabl , pv.WRK_ZTabl0Wait       , dTemp);
                CConfig.ConToVal(tbWRK_YBlad0Work       , ref dTemp); PM.SetValue(mi.WRK_YBlad , pv.WRK_YBlad0Work       , dTemp);
                CConfig.ConToVal(tbWRK_YBlad0BfWork     , ref dTemp); PM.SetValue(mi.WRK_YBlad , pv.WRK_YBlad0BfWork     , dTemp);
                CConfig.ConToVal(cbWRK_TSwit0Work                                 , ref OM.DevOptn.iWRK_TSwit0Work );

                CConfig.ConToVal(tbWRK_TFeed1SpareOfs   , ref dTemp); PM.SetValue(mi.WRK_TFeed , pv.WRK_TFeed1SpareOfs   , dTemp);
                CConfig.ConToVal(tbWRK_TFeed1CogLengOfs , ref dTemp); PM.SetValue(mi.WRK_TFeed , pv.WRK_TFeed1CogLengOfs , dTemp);
                CConfig.ConToVal(tbWRK_TFeed1CogGapOfs  , ref dTemp); PM.SetValue(mi.WRK_TFeed , pv.WRK_TFeed1CogGapOfs  , dTemp);
                CConfig.ConToVal(tbWRK_TBlad1Cog        , ref dTemp); PM.SetValue(mi.WRK_TBlad , pv.WRK_TBlad1Cog        , dTemp);
                CConfig.ConToVal(tbWRK_TBlad1CogUp      , ref dTemp); PM.SetValue(mi.WRK_TBlad , pv.WRK_TBlad1CogUp      , dTemp);
                CConfig.ConToVal(tbWRK_ZTabl1Work       , ref dTemp); PM.SetValue(mi.WRK_ZTabl , pv.WRK_ZTabl1Work       , dTemp);
                CConfig.ConToVal(tbWRK_ZTabl1Wait       , ref dTemp); PM.SetValue(mi.WRK_ZTabl , pv.WRK_ZTabl1Wait       , dTemp);
                CConfig.ConToVal(tbWRK_YBlad1Work       , ref dTemp); PM.SetValue(mi.WRK_YBlad , pv.WRK_YBlad1Work       , dTemp);
                CConfig.ConToVal(tbWRK_YBlad1BfWork     , ref dTemp); PM.SetValue(mi.WRK_YBlad , pv.WRK_YBlad1BfWork     , dTemp);
                CConfig.ConToVal(cbWRK_TSwit1Work, ref OM.DevOptn.iWRK_TSwit1Work );

                CConfig.ConToVal(tbWRK_TFeed2SpareOfs   , ref dTemp); PM.SetValue(mi.WRK_TFeed , pv.WRK_TFeed2SpareOfs   , dTemp);
                CConfig.ConToVal(tbWRK_TFeed2CogLengOfs , ref dTemp); PM.SetValue(mi.WRK_TFeed , pv.WRK_TFeed2CogLengOfs , dTemp);
                CConfig.ConToVal(tbWRK_TFeed2CogGapOfs  , ref dTemp); PM.SetValue(mi.WRK_TFeed , pv.WRK_TFeed2CogGapOfs  , dTemp);
                CConfig.ConToVal(tbWRK_TBlad2Cog        , ref dTemp); PM.SetValue(mi.WRK_TBlad , pv.WRK_TBlad2Cog        , dTemp);
                CConfig.ConToVal(tbWRK_TBlad2CogUp      , ref dTemp); PM.SetValue(mi.WRK_TBlad , pv.WRK_TBlad2CogUp      , dTemp);
                CConfig.ConToVal(tbWRK_ZTabl2Work       , ref dTemp); PM.SetValue(mi.WRK_ZTabl , pv.WRK_ZTabl2Work       , dTemp);
                CConfig.ConToVal(tbWRK_ZTabl2Wait       , ref dTemp); PM.SetValue(mi.WRK_ZTabl , pv.WRK_ZTabl2Wait       , dTemp);
                CConfig.ConToVal(tbWRK_YBlad2Work       , ref dTemp); PM.SetValue(mi.WRK_YBlad , pv.WRK_YBlad2Work       , dTemp);
                CConfig.ConToVal(tbWRK_YBlad2BfWork     , ref dTemp); PM.SetValue(mi.WRK_YBlad , pv.WRK_YBlad2BfWork     , dTemp);
                CConfig.ConToVal(cbWRK_TSwit2Work, ref OM.DevOptn.iWRK_TSwit2Work );

                CConfig.ConToVal(tbWRK_TFeed3SpareOfs   , ref dTemp); PM.SetValue(mi.WRK_TFeed , pv.WRK_TFeed3SpareOfs   , dTemp);
                CConfig.ConToVal(tbWRK_TFeed3CogLengOfs , ref dTemp); PM.SetValue(mi.WRK_TFeed , pv.WRK_TFeed3CogLengOfs , dTemp);
                CConfig.ConToVal(tbWRK_TFeed3CogGapOfs  , ref dTemp); PM.SetValue(mi.WRK_TFeed , pv.WRK_TFeed3CogGapOfs  , dTemp);
                CConfig.ConToVal(tbWRK_TBlad3Cog        , ref dTemp); PM.SetValue(mi.WRK_TBlad , pv.WRK_TBlad3Cog        , dTemp);
                CConfig.ConToVal(tbWRK_TBlad3CogUp      , ref dTemp); PM.SetValue(mi.WRK_TBlad , pv.WRK_TBlad3CogUp      , dTemp);
                CConfig.ConToVal(tbWRK_ZTabl3Work       , ref dTemp); PM.SetValue(mi.WRK_ZTabl , pv.WRK_ZTabl3Work       , dTemp);
                CConfig.ConToVal(tbWRK_ZTabl3Wait       , ref dTemp); PM.SetValue(mi.WRK_ZTabl , pv.WRK_ZTabl3Wait       , dTemp);
                CConfig.ConToVal(tbWRK_YBlad3Work       , ref dTemp); PM.SetValue(mi.WRK_YBlad , pv.WRK_YBlad3Work       , dTemp);
                CConfig.ConToVal(tbWRK_YBlad3BfWork     , ref dTemp); PM.SetValue(mi.WRK_YBlad , pv.WRK_YBlad3BfWork     , dTemp);
                CConfig.ConToVal(cbWRK_TSwit3Work, ref OM.DevOptn.iWRK_TSwit3Work );

                CConfig.ConToVal(tbWRK_TFeed4SpareOfs   , ref dTemp); PM.SetValue(mi.WRK_TFeed , pv.WRK_TFeed4SpareOfs   , dTemp);
                CConfig.ConToVal(tbWRK_TFeed4CogLengOfs , ref dTemp); PM.SetValue(mi.WRK_TFeed , pv.WRK_TFeed4CogLengOfs , dTemp);
                CConfig.ConToVal(tbWRK_TFeed4CogGapOfs  , ref dTemp); PM.SetValue(mi.WRK_TFeed , pv.WRK_TFeed4CogGapOfs  , dTemp);
                CConfig.ConToVal(tbWRK_TBlad4Cog        , ref dTemp); PM.SetValue(mi.WRK_TBlad , pv.WRK_TBlad4Cog        , dTemp);
                CConfig.ConToVal(tbWRK_TBlad4CogUp      , ref dTemp); PM.SetValue(mi.WRK_TBlad , pv.WRK_TBlad4CogUp      , dTemp);
                CConfig.ConToVal(tbWRK_ZTabl4Work       , ref dTemp); PM.SetValue(mi.WRK_ZTabl , pv.WRK_ZTabl4Work       , dTemp);
                CConfig.ConToVal(tbWRK_ZTabl4Wait       , ref dTemp); PM.SetValue(mi.WRK_ZTabl , pv.WRK_ZTabl4Wait       , dTemp);
                CConfig.ConToVal(tbWRK_YBlad4Work       , ref dTemp); PM.SetValue(mi.WRK_YBlad , pv.WRK_YBlad4Work       , dTemp);
                CConfig.ConToVal(tbWRK_YBlad4BfWork     , ref dTemp); PM.SetValue(mi.WRK_YBlad , pv.WRK_YBlad4BfWork     , dTemp);
                CConfig.ConToVal(cbWRK_TSwit4Work, ref OM.DevOptn.iWRK_TSwit4Work );

                CConfig.ConToVal(tbWRK_TFeed5SpareOfs   , ref dTemp); PM.SetValue(mi.WRK_TFeed , pv.WRK_TFeed5SpareOfs   , dTemp);
                CConfig.ConToVal(tbWRK_TFeed5CogLengOfs , ref dTemp); PM.SetValue(mi.WRK_TFeed , pv.WRK_TFeed5CogLengOfs , dTemp);
                CConfig.ConToVal(tbWRK_TFeed5CogGapOfs  , ref dTemp); PM.SetValue(mi.WRK_TFeed , pv.WRK_TFeed5CogGapOfs  , dTemp);
                CConfig.ConToVal(tbWRK_TBlad5Cog        , ref dTemp); PM.SetValue(mi.WRK_TBlad , pv.WRK_TBlad5Cog        , dTemp);
                CConfig.ConToVal(tbWRK_TBlad5CogUp      , ref dTemp); PM.SetValue(mi.WRK_TBlad , pv.WRK_TBlad5CogUp      , dTemp);
                CConfig.ConToVal(tbWRK_ZTabl5Work       , ref dTemp); PM.SetValue(mi.WRK_ZTabl , pv.WRK_ZTabl5Work       , dTemp);
                CConfig.ConToVal(tbWRK_ZTabl5Wait       , ref dTemp); PM.SetValue(mi.WRK_ZTabl , pv.WRK_ZTabl5Wait       , dTemp);
                CConfig.ConToVal(tbWRK_YBlad5Work       , ref dTemp); PM.SetValue(mi.WRK_YBlad , pv.WRK_YBlad5Work       , dTemp);
                CConfig.ConToVal(tbWRK_YBlad5BfWork     , ref dTemp); PM.SetValue(mi.WRK_YBlad , pv.WRK_YBlad5BfWork     , dTemp);
                CConfig.ConToVal(cbWRK_TSwit5Work                                 , ref OM.DevOptn.iWRK_TSwit5Work );

                CConfig.ConToVal(tbWRK_TFeed6SpareOfs   , ref dTemp); PM.SetValue(mi.WRK_TFeed , pv.WRK_TFeed6SpareOfs   , dTemp);
                CConfig.ConToVal(tbWRK_TFeed6CogLengOfs , ref dTemp); PM.SetValue(mi.WRK_TFeed , pv.WRK_TFeed6CogLengOfs , dTemp);
                CConfig.ConToVal(tbWRK_TFeed6CogGapOfs  , ref dTemp); PM.SetValue(mi.WRK_TFeed , pv.WRK_TFeed6CogGapOfs  , dTemp);
                CConfig.ConToVal(tbWRK_TBlad6Cog        , ref dTemp); PM.SetValue(mi.WRK_TBlad , pv.WRK_TBlad6Cog        , dTemp);
                CConfig.ConToVal(tbWRK_TBlad6CogUp      , ref dTemp); PM.SetValue(mi.WRK_TBlad , pv.WRK_TBlad6CogUp      , dTemp);
                CConfig.ConToVal(tbWRK_ZTabl6Work       , ref dTemp); PM.SetValue(mi.WRK_ZTabl , pv.WRK_ZTabl6Work       , dTemp);
                CConfig.ConToVal(tbWRK_ZTabl6Wait       , ref dTemp); PM.SetValue(mi.WRK_ZTabl , pv.WRK_ZTabl6Wait       , dTemp);
                CConfig.ConToVal(tbWRK_YBlad6Work       , ref dTemp); PM.SetValue(mi.WRK_YBlad , pv.WRK_YBlad6Work       , dTemp);
                CConfig.ConToVal(tbWRK_YBlad6BfWork     , ref dTemp); PM.SetValue(mi.WRK_YBlad , pv.WRK_YBlad6BfWork     , dTemp);
                CConfig.ConToVal(cbWRK_TSwit6Work                                 , ref OM.DevOptn.iWRK_TSwit6Work );

                CConfig.ConToVal(tbWRK_TFeed7SpareOfs   , ref dTemp); PM.SetValue(mi.WRK_TFeed , pv.WRK_TFeed7SpareOfs   , dTemp);
                CConfig.ConToVal(tbWRK_TFeed7CogLengOfs , ref dTemp); PM.SetValue(mi.WRK_TFeed , pv.WRK_TFeed7CogLengOfs , dTemp);
                CConfig.ConToVal(tbWRK_TFeed7CogGapOfs  , ref dTemp); PM.SetValue(mi.WRK_TFeed , pv.WRK_TFeed7CogGapOfs  , dTemp);
                CConfig.ConToVal(tbWRK_TBlad7Cog        , ref dTemp); PM.SetValue(mi.WRK_TBlad , pv.WRK_TBlad7Cog        , dTemp);
                CConfig.ConToVal(tbWRK_TBlad7CogUp      , ref dTemp); PM.SetValue(mi.WRK_TBlad , pv.WRK_TBlad7CogUp      , dTemp);
                CConfig.ConToVal(tbWRK_ZTabl7Work       , ref dTemp); PM.SetValue(mi.WRK_ZTabl , pv.WRK_ZTabl7Work       , dTemp);
                CConfig.ConToVal(tbWRK_ZTabl7Wait       , ref dTemp); PM.SetValue(mi.WRK_ZTabl , pv.WRK_ZTabl7Wait       , dTemp);
                CConfig.ConToVal(tbWRK_YBlad7Work       , ref dTemp); PM.SetValue(mi.WRK_YBlad , pv.WRK_YBlad7Work       , dTemp);
                CConfig.ConToVal(tbWRK_YBlad7BfWork     , ref dTemp); PM.SetValue(mi.WRK_YBlad , pv.WRK_YBlad7BfWork     , dTemp);
                CConfig.ConToVal(cbWRK_TSwit7Work                                 , ref OM.DevOptn.iWRK_TSwit7Work );

                CConfig.ConToVal(tbWRK_TFeed8SpareOfs   , ref dTemp); PM.SetValue(mi.WRK_TFeed , pv.WRK_TFeed8SpareOfs   , dTemp);
                CConfig.ConToVal(tbWRK_TFeed8CogLengOfs , ref dTemp); PM.SetValue(mi.WRK_TFeed , pv.WRK_TFeed8CogLengOfs , dTemp);
                CConfig.ConToVal(tbWRK_TFeed8CogGapOfs  , ref dTemp); PM.SetValue(mi.WRK_TFeed , pv.WRK_TFeed8CogGapOfs  , dTemp);
                CConfig.ConToVal(tbWRK_TBlad8Cog        , ref dTemp); PM.SetValue(mi.WRK_TBlad , pv.WRK_TBlad8Cog        , dTemp);
                CConfig.ConToVal(tbWRK_TBlad8CogUp      , ref dTemp); PM.SetValue(mi.WRK_TBlad , pv.WRK_TBlad8CogUp      , dTemp);
                CConfig.ConToVal(tbWRK_ZTabl8Work       , ref dTemp); PM.SetValue(mi.WRK_ZTabl , pv.WRK_ZTabl8Work       , dTemp);
                CConfig.ConToVal(tbWRK_ZTabl8Wait       , ref dTemp); PM.SetValue(mi.WRK_ZTabl , pv.WRK_ZTabl8Wait       , dTemp);
                CConfig.ConToVal(tbWRK_YBlad8Work       , ref dTemp); PM.SetValue(mi.WRK_YBlad , pv.WRK_YBlad8Work       , dTemp);
                CConfig.ConToVal(tbWRK_YBlad8BfWork     , ref dTemp); PM.SetValue(mi.WRK_YBlad , pv.WRK_YBlad8BfWork     , dTemp);
                CConfig.ConToVal(cbWRK_TSwit8Work                                 , ref OM.DevOptn.iWRK_TSwit8Work );

                CConfig.ConToVal(tbWRK_TFeed9SpareOfs   , ref dTemp); PM.SetValue(mi.WRK_TFeed , pv.WRK_TFeed9SpareOfs   , dTemp);
                CConfig.ConToVal(tbWRK_TFeed9CogLengOfs , ref dTemp); PM.SetValue(mi.WRK_TFeed , pv.WRK_TFeed9CogLengOfs , dTemp);
                CConfig.ConToVal(tbWRK_TFeed9CogGapOfs  , ref dTemp); PM.SetValue(mi.WRK_TFeed , pv.WRK_TFeed9CogGapOfs  , dTemp);
                CConfig.ConToVal(tbWRK_TBlad9Cog        , ref dTemp); PM.SetValue(mi.WRK_TBlad , pv.WRK_TBlad9Cog        , dTemp);
                CConfig.ConToVal(tbWRK_TBlad9CogUp      , ref dTemp); PM.SetValue(mi.WRK_TBlad , pv.WRK_TBlad9CogUp      , dTemp);
                CConfig.ConToVal(tbWRK_ZTabl9Work       , ref dTemp); PM.SetValue(mi.WRK_ZTabl , pv.WRK_ZTabl9Work       , dTemp);
                CConfig.ConToVal(tbWRK_ZTabl9Wait       , ref dTemp); PM.SetValue(mi.WRK_ZTabl , pv.WRK_ZTabl9Wait       , dTemp);
                CConfig.ConToVal(tbWRK_YBlad9Work       , ref dTemp); PM.SetValue(mi.WRK_YBlad , pv.WRK_YBlad9Work       , dTemp);
                CConfig.ConToVal(tbWRK_YBlad9BfWork     , ref dTemp); PM.SetValue(mi.WRK_YBlad , pv.WRK_YBlad9BfWork     , dTemp);
                CConfig.ConToVal(cbWRK_TSwit9Work                                 , ref OM.DevOptn.iWRK_TSwit9Work );

                CConfig.ConToVal(tbWRK_TFeedRepeatOfs   , ref dTemp); PM.SetValue(mi.WRK_TFeed , pv.WRK_TFeedRepeatOfs   , dTemp);
                CConfig.ConToVal(tbWRK_TFeedEndOfs      , ref dTemp); PM.SetValue(mi.WRK_TFeed , pv.WRK_TFeedEndOfs      , dTemp);

                

                CConfig.ConToVal(tbWRK_YRottMoveOfs     , ref dTemp); PM.SetValue(mi.WRK_YRott , pv.WRK_YRottMoveOfs     , dTemp);
                CConfig.ConToVal(tbWRK_YRottMoveEnd     , ref dTemp); PM.SetValue(mi.WRK_YRott , pv.WRK_YRottMoveEnd     , dTemp);

                //Auto Log
                Type type = DevInfo.GetType();
                FieldInfo[] f = type.GetFields(BindingFlags.Public|BindingFlags.NonPublic | BindingFlags.Instance);
                for(int i = 0 ; i < f.Length ; i++){
                    Trace(f[i].Name, f[i].GetValue(DevInfo).ToString(), f[i].GetValue(OM.DevInfo).ToString());
                }

                //인덱스 옵션
                UpdateDevInfo(true);
            }
        
        }
        
        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            tmUpdate.Enabled = false;
            btSaveDevice.Enabled = !SEQ._bRun;

            //pnTrayMask.Refresh();
            //lbFeedT.Text = ML.MT_GetCmdPos(mi.WRK_TFeed).ToString();
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
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            if (Log.ShowMessageModal("Confirm", "Are you Sure?") != DialogResult.Yes) return;

            if(tcDeviceSet.SelectedIndex == 0) //디바이스 인포일때는  
            {
                UpdateDevInfo(false);
                PM.Save(OM.GetCrntDev());
                OM.SaveJobFile(OM.GetCrntDev().ToString());
            }
            else 
            {
                PM.Save(OM.GetCrntDev());
                UpdateDevInfo(true);
                OM.SaveJobFile(OM.GetCrntDev().ToString());
            }

            SEQ.WRK.MakeWorkList();
            UpdateWorkInfo();

            
            if (SEQ.WRK.lsWorkList.Count <= OM.EqpStat.iWorkStep)
            {
                Log.ShowMessage("WorkStep Reset", "코그 갯수가 변경되어 작업 순서가 넘어가서 리셑됩니다.");
                OM.EqpStat.iWorkStep = 0;
            }
            if(SEQ.WRK.lsWorkList.Count!=0)   ML.MT_SetPos(mi.WRK_TFeed, SEQ.WRK.lsWorkList[OM.EqpStat.iWorkStep].dAbsFeed);

            //OM.SaveDevInfo(OM.GetCrntDev().ToString());
            //OM.SaveDevOptn(OM.GetCrntDev().ToString());
            //OM.SaveEqpOptn();

            //DM.ARAY[ri.LODR].SetMaxColRow(1                         , OM.DevInfo.iLODR_SlotCnt);
            //DM.ARAY[ri.INDX].SetMaxColRow(OM.DevInfo.iTRAY_PcktCntX , 1                       );
           
            Refresh();
        }



        private void Trace(string sText, string _s1, string _s2)
        {
            Log.Trace(sText + " : " + _s1 + " -> " + _s2,ti.Dev);
        }

        private void FormDeviceSet_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible) tmUpdate.Enabled = true;
        }
    

        private void tpDeviceSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            int iSeletedIndex;
            iSeletedIndex = tcDeviceSet.SelectedIndex;
            
            switch (iSeletedIndex)
            {
                default : break;
                case 1  : gbJogUnit.Parent = pnJog1;                       break;
                case 2  : gbJogUnit.Parent = pnJog2;                       break;
                case 3  : gbJogUnit.Parent = pnJog3;                       break;

                //case 4  : gbJogUnit.Parent = pnJog5;                       break;
            }

            PM.UpdatePstn(true);
            PM.Load(OM.GetCrntDev());
        }

        private void pbLine_Paint(object sender, PaintEventArgs e)
        {
            Pen pen = new Pen(Color.Black);
            pen.Width = 2;

            Graphics g = e.Graphics;

            if(SEQ.WRK.lsWorkList.Count <= 0 ) return ;
            
            double dTotalLength = SEQ.WRK.lsWorkList[SEQ.WRK.lsWorkList.Count-1].dAbsFeed ;
            dTotalLength += ML.PM_GetValue(mi.WRK_TFeed , pv.WRK_TFeedEndOfs);
            dTotalLength += OM.DevOptn.dLeftRightDist / 2.0;
            

            if (dTotalLength < 1) dTotalLength = 1;
            double dWidthScale = pbLine.Size.Width / dTotalLength ;
            double dCenterPos  = dTotalLength / 2;

            double dX1 = 0;
            double dX2 = pbLine.Size.Width - 0;
            double dY = (pbLine.Size.Height) / 2.0;

            g.DrawLine(pen, (int)dX1, (int)dY, (int)dX2, (int)dY);

            bool bLeft = false ;
            double dXPos = 0 ;
            double dCogLeng = 2 * dWidthScale ;
            double dSwitchOffset = OM.DevOptn.dLeftRightDist / 2.0 ;
            for (int i = 0; i < SEQ.WRK.lsWorkList.Count; i++)
            {
                bLeft = SEQ.WRK.lsWorkList[i].dSwitchWork == PM.GetValue(mi.WRK_TSwit , pv.WRK_TSwitLWork);//OM.DevOptn.iWRK_TSwit1Work==0 ?  PM.GetValue(mi.WRK_TSwit , pv.WRK_TSwitLWork ) : PM.GetValue(mi.WRK_TSwit , pv.WRK_TSwitRWork );
                dXPos =(SEQ.WRK.lsWorkList[i].dAbsFeed + (bLeft ? -dSwitchOffset : dSwitchOffset))  * dWidthScale ;
                g.DrawLine(pen, (int)dXPos, (int)dY, (int)(dXPos + (bLeft ? -dCogLeng : dCogLeng))  , (int)(dY -dCogLeng));
            }

            //
            //{
            //    double dCenter = pbLine.Size.Width / 2;
            //
            //    for (int c = 0; c < OM.NodePos[n].iWrkCnt; c++)
            //    {
            //        if (OM.NodePos[n].iDirection == 0)
            //        {
            //            dX1 = OM.NodePos[n].dWrkSttPos + OM.DevOptn.dSttEmpty;
            //            dY1 = iY;
            //            dX2 = dX1;
            //
            //            dX1Ret = (dX1 + (c * OM.NodePos[n].dWrkPitch)) * dWidthScale;
            //            dX2Ret = (dX2 + (c * OM.NodePos[n].dWrkPitch) - OM.DevOptn.dLeftCutMovePitch) * dWidthScale;
            //
            //            if (OM.NodePos[n].dDegree == 0)
            //            {
            //                dY2 = iY - 10;
            //            }
            //            else
            //            {
            //                dY2 = iY + 10;
            //            }
            //        }
            //        else
            //        {
            //            dX1 = OM.NodePos[n].dWrkSttPos + OM.DevOptn.dSttEmpty;
            //            //dX1 = ((dNodeMaxPos + dNodeMinPos) / 2) + OM.NodePos[n].dWrkSttPos;//(OM.NodePos[n].dWrkSttPos - dNodeMinPos) + ((dNodeMaxPos + dNodeMinPos) / 2);
            //            dY1 = iY;
            //            dX2 = dX1;
            //
            //            dX1Ret = (dX1 + (c * OM.NodePos[n].dWrkPitch)) * dWidthScale;
            //            dX2Ret = (dX2 + (c * OM.NodePos[n].dWrkPitch) + OM.DevOptn.dRightCutMovePitch) * dWidthScale;
            //
            //            if (OM.NodePos[n].dDegree == 0)
            //            {
            //                dY2 = iY - 10;
            //            }
            //            else
            //            {
            //                dY2 = iY + 10;
            //            }
            //        }
            //
            //        g.DrawLine(Pen, (float)dX1Ret, (float)dY1, (float)dX2Ret, (float)dY2);
            //    }
            //}
            pen.Dispose();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label21_Click(object sender, EventArgs e)
        {

        }

        private void textBox40_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
