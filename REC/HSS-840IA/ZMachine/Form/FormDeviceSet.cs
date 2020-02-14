using System;
using System.Drawing;
using System.Windows.Forms;
using COMMON;
using SML;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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
        //Dressy
        ListViewItem.ListViewSubItem SelectedLSI;
        //EzSensor
        ListViewItem.ListViewSubItem EzLSI0;
        ListViewItem.ListViewSubItem EzLSI1;
        ListViewItem.ListViewSubItem EzLSI2;
        ListViewItem.ListViewSubItem EzLSI3;
        ListViewItem.ListViewSubItem EzLSI4;
        ListViewItem.ListViewSubItem EzLSI5;
        ListViewItem.ListViewSubItem EzLSI6;
        ListViewItem.ListViewSubItem EzLSI7;

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
            //Macro Dressy
            InitListView();
            
            //EzSensor
            InitEzListView(Lv0 , 1 );
            InitEzListView(Lv1 , 1 );
            InitEzListView(Lv2 , 1 );
            InitEzListView(Lv3 , 1 );
            InitEzListView(Lv4 , 1 );
            InitEzListView(Lv5 , 1 );
            InitEzListView(Lv6 , 10);
            InitEzListView(Lv7 , 1 );
           
            UpdateLsv(true);
            UpdateEzSensor(true);

            tabControl7.TabPages.Remove(tabPage9);//Chart 탭 제거

            EzToDressyChange(); //이지센서일때는 이지센서 탭만 보이고 드레시일때는 드레시만 보이게 한다.

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
            }

            //여기 AP텍에서만 쓰는거
            //실린더 버튼 AP텍꺼
            FraCylAPT = new FrameCylinderAPT[(int)ci.MAX_ACTR];
            for (int i = 0; i < (int)ci.MAX_ACTR; i++)
            {
                Control[] CtrlAP = tcDeviceSet.Controls.Find("C" + i.ToString(), true);

                int iCylCtrl = Convert.ToInt32(CtrlAP[0].Tag);
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
                Control[] Ctrl = tcDeviceSet.Controls.Find("X" + i.ToString(), true);
                
                int iIOCtrl = Convert.ToInt32(Ctrl[0].Tag);

                FraInputAPT[i] = new FrameInputAPT();
                FraInputAPT[i].TopLevel = false;
                FraInputAPT[i].SetConfig((xi)i, ML.IO_GetXName((xi)i), Ctrl[0]);
                FraInputAPT[i].Show();
            }

            //Output Status 생성 AP텍꺼
            const int iOutputBtnCnt = 2;
            FraOutputAPT = new FrameOutputAPT[iOutputBtnCnt];
            for (int i = 0; i < iOutputBtnCnt; i++)
            {
                Control[] Ctrl = tcDeviceSet.Controls.Find("Y" + i.ToString(), true);
            
                int iIOCtrl = Convert.ToInt32(Ctrl[0].Tag);

                FraOutputAPT[i] = new FrameOutputAPT();
                FraOutputAPT[i].TopLevel = false;
                FraOutputAPT[i].SetConfig((yi)iIOCtrl, ML.IO_GetYName((yi)iIOCtrl), Ctrl[0]);
                //FraInputAPT[i].Show();
                //switch (iIOCtrl)
                //{
                //    default: break;
                //    case (int)yi.ETC_IonizerPower: FraOutputAPT[i].SetConfig(yi.ETC_IonizerPower, ML.IO_GetYName(yi.ETC_IonizerPower), Ctrl[0]); break;
                //    case (int)yi.ETC_IonizerSol  : FraOutputAPT[i].SetConfig(yi.ETC_IonizerSol  , ML.IO_GetYName(yi.ETC_IonizerSol  ), Ctrl[0]); break;
                //    //case (int)yi.BARZ_PckrVac: FraOutputAPT[i].SetConfig(yi.BARZ_PckrVac, ML.IO_GetYName(yi.BARZ_PckrVac), Ctrl[0]); break;
                //    //case (int)yi.BARZ_BrcdAC : FraOutputAPT[i].SetConfig(yi.BARZ_BrcdAC , ML.IO_GetYName(yi.BARZ_BrcdAC) , Ctrl[0]); break;
                    
                //}

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
            Lsv1.Clear();
            Lsv1.View = View.Details;
            //Lsv.LabelEdit = true;
            //Lsv.AllowColumnReorder = true;
            Lsv1.FullRowSelect = true;
            Lsv1.GridLines = true;
            //Lsv.Sorting = SortOrder.Descending;
            //Lsv.Scrollable = true;
            //Need to Find
            Lsv1.MultiSelect = false;
            //Lsv.HideSelection = false;

            Lsv1.Columns.Add("NO"               , 50,  HorizontalAlignment.Left); //Day Time
            Lsv1.Columns.Add("관전류(mA)"       , 100, HorizontalAlignment.Left); //Day Time
            Lsv1.Columns.Add("관전압(Kvp)"      , 100, HorizontalAlignment.Left); //Day Time
            Lsv1.Columns.Add("시간(sec)"        , 100, HorizontalAlignment.Left); //Day Time
            Lsv1.Columns.Add("File name(.raw)"  , 300, HorizontalAlignment.Left); //Day Time
            Lsv1.Columns.Add("Type"             , 110, HorizontalAlignment.Left); //Day Time
            Lsv1.Columns.Add("적용필터"         , 100, HorizontalAlignment.Left); //Day Time
            Lsv1.Columns.Add("바인딩"           ,  90, HorizontalAlignment.Left); //Day Time

            Lsv1.MouseWheel += new MouseEventHandler(Lsv1_MouseWheel);

            //lvD1.Items.Clear();
            Lsv1.BeginUpdate();
            ListViewItem item;
            for (int j = 99; j >= 0; j--)
            //for (int j = 0; j < 100; j++)
            {
                item = new ListViewItem((j).ToString());
                item.SubItems.Add("");
                item.SubItems.Add("");
                item.SubItems.Add("");
                item.SubItems.Add("");
                item.SubItems.Add("");
                item.SubItems.Add("");
                item.SubItems.Add("");

                Lsv1.Items.Insert(0, item);
            }
            Lsv1.EndUpdate();

            //TxtEdit = new TextBox();
            //TxtEdit.Leave += new EventHandler   (TxtEdit_Leave);
            //TxtEdit.KeyUp += new KeyEventHandler(TxtEdit_KeyUp);
            ////TxtEdit.SendToBack();
            //TxtEdit.BringToFront();
        }

        public void InitEzListView(ListView _Lv, int _iItem)
        {
            _Lv.Clear();
            _Lv.View          = View.Details;
            _Lv.FullRowSelect = true;
            _Lv.GridLines     = true;
            _Lv.MultiSelect   = false;

            _Lv.Columns.Add("NO"         , 50 , HorizontalAlignment.Left); //Day Time
            _Lv.Columns.Add("관전류(mA)" , 75, HorizontalAlignment.Left); //Day Time
            _Lv.Columns.Add("관전압(Kvp)", 75, HorizontalAlignment.Left); //Day Time
            _Lv.Columns.Add("시간(sec)"  , 75, HorizontalAlignment.Left); //Day Time
            _Lv.Columns.Add("Filter"     , 75, HorizontalAlignment.Left); //Day Time

            _Lv.BeginUpdate();
            ListViewItem item;
            for (int j = _iItem; j >= 1; j--)
            {
                item = new ListViewItem((j).ToString());
                item.SubItems.Add("");
                item.SubItems.Add("");
                item.SubItems.Add("");
                item.SubItems.Add("");

                _Lv.Items.Insert(0, item);
            }
            _Lv.EndUpdate();
        }

        public void UpdateLsv(bool _bToTable)
        {
            string sTemp = "";
            if (_bToTable)
            {
                Lsv1.Items.Clear();
                Lsv1.BeginUpdate();
                ListViewItem item;
                for (int j = 99; j >= 0; j--)
                //for (int j = OM.m_iMaxArray - 1; j >= 0; j--)
                {
                    item = new ListViewItem((j + 1).ToString());
                    item.SubItems.Add(OM.Dressy[j].dXmA     .ToString());
                    item.SubItems.Add(OM.Dressy[j].dXKvp    .ToString());
                    item.SubItems.Add(OM.Dressy[j].dXTime   .ToString());
                    if(OM.Dressy[j].sFileName != null)item.SubItems.Add(OM.Dressy[j].sFileName.ToString());
                    if(OM.Dressy[j].sType     != null) sTemp = OM.Dressy[j].sType    .ToString() ;
                    if(OM.Dressy[j].sAcq1.ToString() != "") sTemp += "_" + OM.Dressy[j].sAcq1.ToString() + "," ;
                    if(OM.Dressy[j].sAcq2.ToString() != "") sTemp += OM.Dressy[j].sAcq2.ToString() + "," ;
                    if(OM.Dressy[j].sAcq3.ToString() != "") sTemp += OM.Dressy[j].sAcq3.ToString() + "," ;
                    if(OM.Dressy[j].sAcq4.ToString() != "") sTemp += OM.Dressy[j].sAcq4.ToString() + "," ;
                    if(OM.Dressy[j].sAcq5.ToString() != "") sTemp += OM.Dressy[j].sAcq5.ToString() + "," ;
                    if(OM.Dressy[j].sAcq6.ToString() != "") sTemp += OM.Dressy[j].sAcq6.ToString() + "," ;
                    if(OM.Dressy[j].sAcq7.ToString() != "") sTemp += OM.Dressy[j].sAcq7.ToString() + "," ;
                    if(OM.Dressy[j].sStep.ToString() != "") sTemp += OM.Dressy[j].sStep.ToString();
                    item.SubItems.Add(sTemp);
                    item.SubItems.Add(OM.Dressy[j].iFilter  .ToString());
                    item.SubItems.Add(OM.Dressy[j].iBind    .ToString());
                                                   
                    Lsv1.Items.Insert(0, item);
                }
                Lsv1.EndUpdate();
            }
            else
            {
                for (int i = 0; i < Lsv1.Items.Count; i++)
                {
                    ListViewItem item = Lsv1.Items[i];
                    OM.Dressy[i].dXmA      = CConfig.StrToDoubleDef(item.SubItems[1].Text,0);
                    OM.Dressy[i].dXKvp     = CConfig.StrToDoubleDef(item.SubItems[2].Text,0);
                    OM.Dressy[i].dXTime    = CConfig.StrToDoubleDef(item.SubItems[3].Text,0);
                    OM.Dressy[i].sFileName = item.SubItems[4].Text;

                    String[] sSplit = item.SubItems[5].Text.Split(',');
                    if(0 < sSplit.Length && sSplit[0] != "")  OM.Dressy[i].sType     = sSplit[0].Substring(0,1);
                    if(0 < sSplit.Length && sSplit[0] == "")  OM.Dressy[i].sType     = sSplit[0];
                    if(1 < sSplit.Length)  OM.Dressy[i].sAcq1     = sSplit[0].Substring(2, 1);
                    else                   OM.Dressy[i].sAcq1     = "";
                    if(2 < sSplit.Length)  OM.Dressy[i].sAcq2     = sSplit[1];
                    else                   OM.Dressy[i].sAcq2     = "";
                    if(3 < sSplit.Length)  OM.Dressy[i].sAcq3     = sSplit[2];
                    else                   OM.Dressy[i].sAcq3     = "";
                    if(4 < sSplit.Length)  OM.Dressy[i].sAcq4     = sSplit[3];
                    else                   OM.Dressy[i].sAcq4     = "";
                    if(5 < sSplit.Length)  OM.Dressy[i].sAcq5     = sSplit[4];
                    else                   OM.Dressy[i].sAcq5     = "";
                    if(6 < sSplit.Length)  OM.Dressy[i].sAcq6     = sSplit[5];
                    else                   OM.Dressy[i].sAcq6     = "";
                    if(7 <= sSplit.Length) OM.Dressy[i].sAcq7     = sSplit[6];
                    else                   OM.Dressy[i].sAcq7     = "";
                    if(8 <= sSplit.Length) OM.Dressy[i].sStep     = sSplit[7];
                    else                   OM.Dressy[i].sStep     = "";

                    OM.Dressy[i].iFilter   = CConfig.StrToIntDef(item.SubItems[6].Text,0);
                    OM.Dressy[i].iBind     = CConfig.StrToIntDef(item.SubItems[7].Text,0);
                }
                UpdateLsv(true);
            }      

        }

        public void UpdateEzSensor(bool _bToTable)
        {
            string sTemp = "";
            if (_bToTable)
            {
                //입고 영상 공용
                Lv0.Items.Clear();
                Lv0.BeginUpdate();
                ListViewItem item0;
                for (int j = 0; j >= 0; j--)
                {
                    item0 = new ListViewItem((j + 1).ToString());
                    item0.SubItems.Add(OM.EzSensor[j].dEzEntrXmA  .ToString());
                    item0.SubItems.Add(OM.EzSensor[j].dEzEntrXKvp .ToString());
                    item0.SubItems.Add(OM.EzSensor[j].dEzEntrXTime.ToString());
                    item0.SubItems.Add(OM.EzSensor[j].iEzEntrFltr .ToString());
                    if(OM.EzSensor[j].iEzEntrFltr <=0)
                    {
                        OM.EzSensor[j].iEzEntrFltr = 1;
                    }
                    Lv0.Items.Insert(0, item0);
                }
                Lv0.EndUpdate();
                //EzSensor
                //특성 Get Bright1
                Lv1.Items.Clear();
                Lv1.BeginUpdate();
                ListViewItem item1;
                for (int j = 0; j >= 0; j--)
                {
                    item1 = new ListViewItem((j + 1).ToString());
                    item1.SubItems.Add(OM.EzSensor[j].dEzGbXmA1  .ToString());
                    item1.SubItems.Add(OM.EzSensor[j].dEzGbXKvp1 .ToString());
                    item1.SubItems.Add(OM.EzSensor[j].dEzGbXTime1.ToString());
                    item1.SubItems.Add(OM.EzSensor[j].iEzGbFltr1 .ToString());
                    if(OM.EzSensor[j].iEzGbFltr1 <=0)
                    {
                        OM.EzSensor[j].iEzGbFltr1 = 1;
                    }

                    Lv1.Items.Insert(0, item1);
                }
                Lv1.EndUpdate();

                //특성 Get Image1
                Lv2.Items.Clear();
                Lv2.BeginUpdate();
                ListViewItem item2;
                for (int j = 0; j >= 0; j--)
                {
                    item2 = new ListViewItem((j + 1).ToString());
                    item2.SubItems.Add(OM.EzSensor[j].dEzGiXmA1  .ToString());
                    item2.SubItems.Add(OM.EzSensor[j].dEzGiXKvp1 .ToString());
                    item2.SubItems.Add(OM.EzSensor[j].dEzGiXTime1.ToString());
                    item2.SubItems.Add(OM.EzSensor[j].iEzGiFltr1 .ToString());
                    if(OM.EzSensor[j].iEzGiFltr1 <=0)
                    {
                        OM.EzSensor[j].iEzGiFltr1 = 1;
                    }

                    Lv2.Items.Insert(0, item2);
                }
                Lv2.EndUpdate();

                //특성 Get Bright2
                Lv3.Items.Clear();
                Lv3.BeginUpdate();
                ListViewItem item3;
                for (int j = 0; j >= 0; j--)
                {
                    item3 = new ListViewItem((j + 1).ToString());
                    item3.SubItems.Add(OM.EzSensor[j].dEzGbXmA2  .ToString());
                    item3.SubItems.Add(OM.EzSensor[j].dEzGbXKvp2 .ToString());
                    item3.SubItems.Add(OM.EzSensor[j].dEzGbXTime2.ToString());
                    item3.SubItems.Add(OM.EzSensor[j].iEzGbFltr2 .ToString());
                    if(OM.EzSensor[j].iEzGbFltr2 <=0)
                    {
                        OM.EzSensor[j].iEzGbFltr2 = 1;
                    }

                    Lv3.Items.Insert(0, item3);
                }
                Lv3.EndUpdate();

                //특성 Get Image2
                Lv4.Items.Clear();
                Lv4.BeginUpdate();
                ListViewItem item4;
                for (int j = 0; j >= 0; j--)
                {
                    item4 = new ListViewItem((j + 1).ToString());
                    item4.SubItems.Add(OM.EzSensor[j].dEzGiXmA2  .ToString());
                    item4.SubItems.Add(OM.EzSensor[j].dEzGiXKvp2 .ToString());
                    item4.SubItems.Add(OM.EzSensor[j].dEzGiXTime2.ToString());
                    item4.SubItems.Add(OM.EzSensor[j].iEzGiFltr2 .ToString());
                    if(OM.EzSensor[j].iEzGiFltr2 <=0)
                    {
                        OM.EzSensor[j].iEzGiFltr2 = 1;
                    }

                    Lv4.Items.Insert(0, item4);
                }
                Lv4.EndUpdate();

                //Cal Trigger  
                Lv5.Items.Clear();
                Lv5.BeginUpdate();
                ListViewItem item5;
                for (int j = 0; j >= 0; j--)
                {
                    item5 = new ListViewItem((j + 1).ToString());
                    item5.SubItems.Add(OM.EzSensor[j].dEzTrXmA  .ToString());
                    item5.SubItems.Add(OM.EzSensor[j].dEzTrXKvp .ToString());
                    item5.SubItems.Add(OM.EzSensor[j].dEzTrXTime.ToString());
                    item5.SubItems.Add(OM.EzSensor[j].iEzTrFltr .ToString());
                    if(OM.EzSensor[j].iEzTrFltr <=0)
                    {
                        OM.EzSensor[j].iEzTrFltr = 1;
                    }

                    Lv5.Items.Insert(0, item5);
                }
                Lv5.EndUpdate();

                //Cal Get Bright3  8개  
                Lv6.Items.Clear();
                Lv6.BeginUpdate();
                ListViewItem item6;
                for (int j = 9; j >= 0; j--)
                {
                    item6 = new ListViewItem((j + 1).ToString());
                    item6.SubItems.Add(OM.EzSensor[j].dEzGbXmA3  .ToString());
                    item6.SubItems.Add(OM.EzSensor[j].dEzGbXKvp3 .ToString());
                    item6.SubItems.Add(OM.EzSensor[j].dEzGbXTime3.ToString());
                    item6.SubItems.Add(OM.EzSensor[j].iEzGbFltr3 .ToString());
                    if(OM.EzSensor[j].iEzGbFltr3 <=0)
                    {
                        OM.EzSensor[j].iEzGbFltr3 = 1;
                    }

                    Lv6.Items.Insert(0, item6);
                }
                Lv6.EndUpdate();

                //Skull  
                Lv7.Items.Clear();
                Lv7.BeginUpdate();
                ListViewItem item7;
                for (int j = 0; j >= 0; j--)
                {
                    item7 = new ListViewItem((j + 1).ToString());
                    item7.SubItems.Add(OM.EzSensor[j].dEzSkXmA  .ToString());
                    item7.SubItems.Add(OM.EzSensor[j].dEzSkXKvp .ToString());
                    item7.SubItems.Add(OM.EzSensor[j].dEzSkXTime.ToString());
                    item7.SubItems.Add(OM.EzSensor[j].iEzSkFltr .ToString());
                    if(OM.EzSensor[j].iEzSkFltr <=0)
                    {
                        OM.EzSensor[j].iEzSkFltr = 1;
                    }

                    Lv7.Items.Insert(0, item7);
                }
                Lv7.EndUpdate();
            }
            else
            {
                //입고영상 공용
                for (int i = 0; i < Lv0.Items.Count; i++)
                {
                    ListViewItem item0   = Lv0.Items[i];
                    OM.EzSensor[i].dEzEntrXmA   = CConfig.StrToDoubleDef(item0.SubItems[1].Text, 0);
                    OM.EzSensor[i].dEzEntrXKvp  = CConfig.StrToDoubleDef(item0.SubItems[2].Text, 0);
                    OM.EzSensor[i].dEzEntrXTime = CConfig.StrToDoubleDef(item0.SubItems[3].Text, 0);
                    OM.EzSensor[i].iEzEntrFltr  = CConfig.StrToIntDef   (item0.SubItems[4].Text, 0);
                }

                //EzSensor
                //특성 Get Bright1
                for (int i = 0; i < Lv1.Items.Count; i++)
                {
                    ListViewItem item1   = Lv1.Items[i];
                    OM.EzSensor[i].dEzGbXmA1   = CConfig.StrToDoubleDef(item1.SubItems[1].Text, 0);
                    OM.EzSensor[i].dEzGbXKvp1  = CConfig.StrToDoubleDef(item1.SubItems[2].Text, 0);
                    OM.EzSensor[i].dEzGbXTime1 = CConfig.StrToDoubleDef(item1.SubItems[3].Text, 0);
                    OM.EzSensor[i].iEzGbFltr1  = CConfig.StrToIntDef   (item1.SubItems[4].Text, 0);
                }

                //특성 Get Image1
                for (int i = 0; i < Lv2.Items.Count; i++)
                {
                    ListViewItem item2   = Lv2.Items[i];
                    OM.EzSensor[i].dEzGiXmA1   = CConfig.StrToDoubleDef(item2.SubItems[1].Text, 0);
                    OM.EzSensor[i].dEzGiXKvp1  = CConfig.StrToDoubleDef(item2.SubItems[2].Text, 0);
                    OM.EzSensor[i].dEzGiXTime1 = CConfig.StrToDoubleDef(item2.SubItems[3].Text, 0);
                    OM.EzSensor[i].iEzGiFltr1  = CConfig.StrToIntDef   (item2.SubItems[4].Text, 0);
                }

                //특성 Get Bright2
                for (int i = 0; i < Lv3.Items.Count; i++)
                {
                    ListViewItem item3   = Lv3.Items[i];
                    OM.EzSensor[i].dEzGbXmA2   = CConfig.StrToDoubleDef(item3.SubItems[1].Text, 0);
                    OM.EzSensor[i].dEzGbXKvp2  = CConfig.StrToDoubleDef(item3.SubItems[2].Text, 0);
                    OM.EzSensor[i].dEzGbXTime2 = CConfig.StrToDoubleDef(item3.SubItems[3].Text, 0);
                    OM.EzSensor[i].iEzGbFltr2  = CConfig.StrToIntDef   (item3.SubItems[4].Text, 0);
                }

                //특성 Get Image2         
                for (int i = 0; i < Lv4.Items.Count; i++)
                {
                    ListViewItem item4   = Lv4.Items[i];
                    OM.EzSensor[i].dEzGiXmA2   = CConfig.StrToDoubleDef(item4.SubItems[1].Text, 0);
                    OM.EzSensor[i].dEzGiXKvp2  = CConfig.StrToDoubleDef(item4.SubItems[2].Text, 0);
                    OM.EzSensor[i].dEzGiXTime2 = CConfig.StrToDoubleDef(item4.SubItems[3].Text, 0);
                    OM.EzSensor[i].iEzGiFltr2  = CConfig.StrToIntDef   (item4.SubItems[4].Text, 0);
                }

                //Cal Trigger             
                for (int i = 0; i < Lv5.Items.Count; i++)
                {
                    ListViewItem item5   = Lv5.Items[i];
                    OM.EzSensor[i].dEzTrXmA   = CConfig.StrToDoubleDef(item5.SubItems[1].Text, 0);
                    OM.EzSensor[i].dEzTrXKvp  = CConfig.StrToDoubleDef(item5.SubItems[2].Text, 0);
                    OM.EzSensor[i].dEzTrXTime = CConfig.StrToDoubleDef(item5.SubItems[3].Text, 0);
                    OM.EzSensor[i].iEzTrFltr  = CConfig.StrToIntDef   (item5.SubItems[4].Text, 0);
                }

                //Cal Get Bright3     
                for (int i = 0; i < Lv6.Items.Count; i++)
                {
                    ListViewItem item6   = Lv6.Items[i];
                    OM.EzSensor[i].dEzGbXmA3   = CConfig.StrToDoubleDef(item6.SubItems[1].Text, 0);
                    OM.EzSensor[i].dEzGbXKvp3  = CConfig.StrToDoubleDef(item6.SubItems[2].Text, 0);
                    OM.EzSensor[i].dEzGbXTime3 = CConfig.StrToDoubleDef(item6.SubItems[3].Text, 0);
                    OM.EzSensor[i].iEzGbFltr3  = CConfig.StrToIntDef   (item6.SubItems[4].Text, 0);
                }

                //Skull                   
                for (int i = 0; i < Lv7.Items.Count; i++)
                {
                    ListViewItem item7   = Lv7.Items[i];
                    OM.EzSensor[i].dEzSkXmA   = CConfig.StrToDoubleDef(item7.SubItems[1].Text, 0);
                    OM.EzSensor[i].dEzSkXKvp  = CConfig.StrToDoubleDef(item7.SubItems[2].Text, 0);
                    OM.EzSensor[i].dEzSkXTime = CConfig.StrToDoubleDef(item7.SubItems[3].Text, 0);
                    OM.EzSensor[i].iEzSkFltr  = CConfig.StrToIntDef   (item7.SubItems[4].Text, 0);
                }

                //BI
                //특성 Get Bright1
                //for (int i = 0; i < Lv8.Items.Count; i++)
                //{
                //    ListViewItem item8   = Lv8.Items[i];
                //    OM.EzSensor[i].dBiGiXmA1   = CConfig.StrToDoubleDef(item8.SubItems[1].Text, 0);
                //    OM.EzSensor[i].dBiGiXKvp1  = CConfig.StrToDoubleDef(item8.SubItems[2].Text, 0);
                //    OM.EzSensor[i].dBiGiXTime1 = CConfig.StrToDoubleDef(item8.SubItems[3].Text, 0);
                //    OM.EzSensor[i].iBiGiFltr1  = CConfig.StrToIntDef   (item8.SubItems[4].Text, 0);
                //}
                //
                ////특성 Get Bright2
                //for (int i = 0; i < Lv9.Items.Count; i++)
                //{
                //    ListViewItem item9   = Lv9.Items[i];
                //    OM.EzSensor[i].dBiGiXmA2   = CConfig.StrToDoubleDef(item9.SubItems[1].Text, 0);
                //    OM.EzSensor[i].dBiGiXKvp2  = CConfig.StrToDoubleDef(item9.SubItems[2].Text, 0);
                //    OM.EzSensor[i].dBiGiXTime2 = CConfig.StrToDoubleDef(item9.SubItems[3].Text, 0);
                //    OM.EzSensor[i].iBiGiFltr2  = CConfig.StrToIntDef   (item9.SubItems[4].Text, 0);
                //}
                //
                ////Cal Get Bright 3개
                //for (int i = 0; i < Lv10.Items.Count; i++)
                //{
                //    ListViewItem item10   = Lv10.Items[i];
                //    OM.EzSensor[i].dBiGbXmA   = CConfig.StrToDoubleDef(item10.SubItems[1].Text, 0);
                //    OM.EzSensor[i].dBiGbXKvp  = CConfig.StrToDoubleDef(item10.SubItems[2].Text, 0);
                //    OM.EzSensor[i].dBiGbXTime = CConfig.StrToDoubleDef(item10.SubItems[3].Text, 0);
                //    OM.EzSensor[i].iBiGbFltr  = CConfig.StrToIntDef   (item10.SubItems[4].Text, 0);
                //}
                //
                ////Skull
                //for (int i = 0; i < Lv11.Items.Count; i++)
                //{
                //    ListViewItem item11   = Lv11.Items[i];
                //    OM.EzSensor[i].dBiSkXmA   = CConfig.StrToDoubleDef(item11.SubItems[1].Text, 0);
                //    OM.EzSensor[i].dBiSkXKvp  = CConfig.StrToDoubleDef(item11.SubItems[2].Text, 0);
                //    OM.EzSensor[i].dBiSkXTime = CConfig.StrToDoubleDef(item11.SubItems[3].Text, 0);
                //    OM.EzSensor[i].iBiSkFltr  = CConfig.StrToIntDef   (item11.SubItems[4].Text, 0);
                //}

                UpdateEzSensor(true);
            }

        }

        

        public void PstnDisp()
        {
            //LODR_ZElev                                 
            PM.SetProp((uint)mi.LODR_ZElev, (uint)pv.LODR_ZElevWait      , "Wait                ", false, false, false);
            PM.SetProp((uint)mi.LODR_ZElev, (uint)pv.LODR_ZElevWorkStt   , "Work Start          ", false, false, false);
            PM.SetProp((uint)mi.LODR_ZElev, (uint)pv.LODR_ZElevRefill    , "Refill              ", false, false, false);
            PM.SetCheckSafe((uint)mi.LODR_ZElev, SEQ.INDX.CheckSafe);

            //INDX_XRail                                                                             
            PM.SetProp((uint)mi.INDX_XRail, (uint)pv.INDX_XRailWait      , "Wait                ", false, false, false);
            PM.SetProp((uint)mi.INDX_XRail, (uint)pv.INDX_XRailGet       , "Get                 ", false, false, false);
            PM.SetProp((uint)mi.INDX_XRail, (uint)pv.INDX_XRailBarcode   , "Barcode             ", false, false, false);
            PM.SetProp((uint)mi.INDX_XRail, (uint)pv.INDX_XRailWorkStt   , "Work Start          ", false, false, false);
            PM.SetCheckSafe((uint)mi.INDX_XRail, SEQ.INDX.CheckSafe);

            //XRAY_XFltr                                                                             
            PM.SetProp((uint)mi.XRAY_XFltr, (uint)pv.XRAY_XFltrWait      , "Wait                ", false, false, false);
            PM.SetProp((uint)mi.XRAY_XFltr, (uint)pv.XRAY_XFltr1Work     , "Filter1 Work        ", false, false, false);
            PM.SetProp((uint)mi.XRAY_XFltr, (uint)pv.XRAY_XFltr2Work     , "Filter2 Work        ", false, false, false);
            PM.SetProp((uint)mi.XRAY_XFltr, (uint)pv.XRAY_XFltr3Work     , "Filter3 Work        ", false, false, false);
            PM.SetProp((uint)mi.XRAY_XFltr, (uint)pv.XRAY_XFltr4Work     , "Filter4 Work        ", false, false, false);
            PM.SetProp((uint)mi.XRAY_XFltr, (uint)pv.XRAY_XFltr5Work     , "Filter5 Work        ", false, false, false);
            PM.SetProp((uint)mi.XRAY_XFltr, (uint)pv.XRAY_XFltr6Work     , "Filter6 Work        ", false, false, false);
            PM.SetProp((uint)mi.XRAY_XFltr, (uint)pv.XRAY_XFltr7Work     , "Filter7 Work        ", false, false, false);
                 if(OM.DevInfo.iMacroType == 0) PM.SetCheckSafe((uint)mi.XRAY_XFltr, SEQ.XRYD.CheckSafe);
            else if(OM.DevInfo.iMacroType == 1) PM.SetCheckSafe((uint)mi.XRAY_XFltr, SEQ.XRYE.CheckSafe);

            //XRAY_ZXRay                                                                             
            PM.SetProp((uint)mi.XRAY_ZXRay, (uint)pv.XRAY_ZXRayWait      , "Wait                ", false, false, false);
            PM.SetProp((uint)mi.XRAY_ZXRay, (uint)pv.XRAY_ZXRayFltrMove  , "Filter Move         ", false, false, false);
            PM.SetProp((uint)mi.XRAY_ZXRay, (uint)pv.XRAY_ZXRayWork      , "Work                ", false, false, false);
                 if (OM.DevInfo.iMacroType == 0) PM.SetCheckSafe((uint)mi.XRAY_ZXRay, SEQ.XRYD.CheckSafe);
            else if (OM.DevInfo.iMacroType == 1) PM.SetCheckSafe((uint)mi.XRAY_ZXRay, SEQ.XRYE.CheckSafe);
			//XRAY_XCnct
			PM.SetProp((uint)mi.XRAY_XCnct, (uint)pv.XRAY_XCnctWait      , "Wait                ", false, false, false);
            PM.SetProp((uint)mi.XRAY_XCnct, (uint)pv.XRAY_XCnctLeftWork  , "Left Work           ", false, false, false);
            PM.SetProp((uint)mi.XRAY_XCnct, (uint)pv.XRAY_XCnctRightWork , "Right Work          ", false, false, false);
                 if(OM.DevInfo.iMacroType == 0) PM.SetCheckSafe((uint)mi.XRAY_XCnct, SEQ.XRYD.CheckSafe);
            else if(OM.DevInfo.iMacroType == 1) PM.SetCheckSafe((uint)mi.XRAY_XCnct, SEQ.XRYE.CheckSafe);
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
                //case 4  : gbJogUnit.Parent = pnJog5;                       break;
            }

            PM.UpdatePstn(true);
            PM.Load(OM.GetCrntDev());
        }

        public void UpdateDevInfo(bool bToTable)
        {
            if (bToTable)
            {
                //DeviceInfo Dressy
                CConfig.ValToCon(tbTRAY_PcktCntX   , ref OM.DevInfo.iTRAY_PcktCntX   );
                CConfig.ValToCon(tbTRAY_PcktPitchX , ref OM.DevInfo.dTRAY_PcktPitchX );
                CConfig.ValToCon(tbLODR_SlotCnt    , ref OM.DevInfo.iLODR_SlotCnt    );
                CConfig.ValToCon(tbLODR_SlotPitch  , ref OM.DevInfo.dLODR_SlotPitch  );
                
                
                ////XRAY 옵션
                CConfig.ValToCon(cbUseUSBOptn      , ref OM.DevInfo.iUseUSBOptn      );
                CConfig.ValToCon(tbCoolingTime     , ref OM.DevInfo.iCoolingTime     );

                //Macro
                CConfig.ValToCon(cbMacroType       , ref OM.DevInfo.iMacroType       );

                //Dressy 공용 옵션
                CConfig.ValToCon(cbIgnrCycleAnalyze, ref OM.DressyInfo.bIgnrCycleAnalyze);
                CConfig.ValToCon(cbIgnrCycleCheck  , ref OM.DressyInfo.bIgnrCycleCheck  );
                CConfig.ValToCon(tbBfXrayDelay     , ref OM.DressyInfo.iBfXrayDelay     );
                CConfig.ValToCon(cbSelGetDarkBtn   , ref OM.DressyInfo.iSelGetDarkBtn   );
                CConfig.ValToCon(tbTolerance       , ref OM.DressyInfo.iTolerance       );
                CConfig.ValToCon(tbCalDeleteDelay  , ref OM.DressyInfo.iCalDeleteDelay  );
                //1.0.1.7 옵션 추가
                CConfig.ValToCon(cbTrgErrProc      , ref OM.DressyInfo.iTrgErrProc      );
                CConfig.ValToCon(cbCalRptErrProc   , ref OM.DressyInfo.iCalRptErrProc   );
                CConfig.ValToCon(tbCalRptCnt       , ref OM.DressyInfo.iCalRptCnt       );

                //DressyInfo
                CConfig.ValToCon(b1                , ref OM.DressyInfo.s1            );
                CConfig.ValToCon(b2                , ref OM.DressyInfo.s2            );
                CConfig.ValToCon(b3                , ref OM.DressyInfo.s3            );
                CConfig.ValToCon(b4                , ref OM.DressyInfo.s4            );
                CConfig.ValToCon(b5                , ref OM.DressyInfo.s5            );
                CConfig.ValToCon(b6                , ref OM.DressyInfo.s6            );
                CConfig.ValToCon(b7                , ref OM.DressyInfo.s7            );
                CConfig.ValToCon(b8                , ref OM.DressyInfo.s8            );
                CConfig.ValToCon(b9                , ref OM.DressyInfo.s9            );
                CConfig.ValToCon(bA                , ref OM.DressyInfo.sA            );
                CConfig.ValToCon(bB                , ref OM.DressyInfo.sB            );
                CConfig.ValToCon(bC                , ref OM.DressyInfo.sC            );
                CConfig.ValToCon(bD                , ref OM.DressyInfo.sD            );


                CConfig.ValToCon(b11               , ref OM.DressyInfo.sAppPath1     );
                CConfig.ValToCon(b12               , ref OM.DressyInfo.sAppName1     );
                CConfig.ValToCon(b13               , ref OM.DressyInfo.iPosX1        );
                CConfig.ValToCon(b14               , ref OM.DressyInfo.iPosY1        );
                CConfig.ValToCon(b15               , ref OM.DressyInfo.sAppPath2     );
                CConfig.ValToCon(b16               , ref OM.DressyInfo.sAppName2     );
                CConfig.ValToCon(b17               , ref OM.DressyInfo.iPosX2        );
                CConfig.ValToCon(b18               , ref OM.DressyInfo.iPosY2        );
                CConfig.ValToCon(b19               , ref OM.DressyInfo.sAppPath3     );
                CConfig.ValToCon(b20               , ref OM.DressyInfo.sAppName3     );
                CConfig.ValToCon(b21               , ref OM.DressyInfo.iPosX3        );
                CConfig.ValToCon(b22               , ref OM.DressyInfo.iPosY3        );
                CConfig.ValToCon(b23               , ref OM.DressyInfo.sIniPath1     );
                CConfig.ValToCon(b24               , ref OM.DressyInfo.sIniPath2     );
                CConfig.ValToCon(b25               , ref OM.DressyInfo.sRsltPath     );
                CConfig.ValToCon(b26               , ref OM.DressyInfo.sCalPath      );

                //NPS ROI 1x1 Setting
                CConfig.ValToCon(tbNPSArea1        , ref OM.DressyInfo.iNPSArea1     );
                CConfig.ValToCon(tbNPSLeft1        , ref OM.DressyInfo.iNPSLeft1     );
                CConfig.ValToCon(tbNPSSub1         , ref OM.DressyInfo.iNPSSub1      );
                CConfig.ValToCon(tbNPSTop1         , ref OM.DressyInfo.iNPSTop1      );

                //NPS ROI 2x2
                CConfig.ValToCon(tbNPSArea2        , ref OM.DressyInfo.iNPSArea2     );
                CConfig.ValToCon(tbNPSLeft2        , ref OM.DressyInfo.iNPSLeft2     );
                CConfig.ValToCon(tbNPSSub2         , ref OM.DressyInfo.iNPSSub2      );
                CConfig.ValToCon(tbNPSTop2         , ref OM.DressyInfo.iNPSTop2      );

                //1x1 MTF ROI
                CConfig.ValToCon(tbMTFROILeft1     , ref OM.DressyInfo.iMTFROILeft1  );
                CConfig.ValToCon(tbMTFROILen1      , ref OM.DressyInfo.iMTFROILen1   );
                CConfig.ValToCon(tbMTFROINum1      , ref OM.DressyInfo.iMTFROINum1   );
                CConfig.ValToCon(tbMTFROITop1      , ref OM.DressyInfo.iMTFROITop1   );

                //1x1 MTF Edge
                CConfig.ValToCon(tbMTFEgHght1      , ref OM.DressyInfo.iMTFEgHght1   );
                CConfig.ValToCon(tbMTFEgLeft1      , ref OM.DressyInfo.iMTFEgLeft1   );
                CConfig.ValToCon(tbMTFEgTop1       , ref OM.DressyInfo.iMTFEgTop1    );
                CConfig.ValToCon(tbMTFEgWidth1     , ref OM.DressyInfo.iMTFEgWidth1  );

                //2x2 MTF ROI
                CConfig.ValToCon(tbMTFROILeft2     , ref OM.DressyInfo.iMTFROILeft2  );
                CConfig.ValToCon(tbMTFROILen2      , ref OM.DressyInfo.iMTFROILen2   );
                CConfig.ValToCon(tbMTFROINum2      , ref OM.DressyInfo.iMTFROINum2   );
                CConfig.ValToCon(tbMTFROITop2      , ref OM.DressyInfo.iMTFROITop2   );

                //2x2 MTF Edge
                CConfig.ValToCon(tbMTFEgHght2      , ref OM.DressyInfo.iMTFEgHght2   );
                CConfig.ValToCon(tbMTFEgLeft2      , ref OM.DressyInfo.iMTFEgLeft2   );
                CConfig.ValToCon(tbMTFEgTop2       , ref OM.DressyInfo.iMTFEgTop2    );
                CConfig.ValToCon(tbMTFEgWidth2     , ref OM.DressyInfo.iMTFEgWidth2  );

                //Write 할때 쑤셔넣는놈들
                CConfig.ValToCon(tbCode            , ref OM.DressyInfo.sProductCode  );
                CConfig.ValToCon(tbVer             , ref OM.DressyInfo.sProductVer   );
                CConfig.ValToCon(tbFPGA            , ref OM.DressyInfo.sFPGAVer      );
                CConfig.ValToCon(tbAcqSW           , ref OM.DressyInfo.sAcqSW        );
                CConfig.ValToCon(tbEvalSW          , ref OM.DressyInfo.sEvalSW       );
                CConfig.ValToCon(tbPerform         , ref OM.DressyInfo.sPerform      );
          
				
				//EzSensor Info
                //Detector's settings & App Path
                CConfig.ValToCon(cbEzType          , ref OM.EzSensorInfo.iEzType         );
                CConfig.ValToCon(cbImgSize         , ref OM.EzSensorInfo.iImgSize        );
                CConfig.ValToCon(tbEzGbCnt         , ref OM.EzSensorInfo.iEzGbCnt        );
                CConfig.ValToCon(tbGbDelay         , ref OM.EzSensorInfo.iGbDelay        );
                CConfig.ValToCon(tbBfMacDelay      , ref OM.EzSensorInfo.iBfMacDelay     );
                CConfig.ValToCon(tbAtMacDelay      , ref OM.EzSensorInfo.iAtMacDelay     );
                CConfig.ValToCon(cbUseIOSPgm       , ref OM.EzSensorInfo.bUseIOSPgm      );
                CConfig.ValToCon(a1                , ref OM.EzSensorInfo.iProductID      );
                CConfig.ValToCon(a2                , ref OM.EzSensorInfo.bInvertacq      );
                CConfig.ValToCon(a3                , ref OM.EzSensorInfo.iTimeout        );
                CConfig.ValToCon(a4                , ref OM.EzSensorInfo.iOntheFly       );
                CConfig.ValToCon(a5                , ref OM.EzSensorInfo.bEnableSerial   );
                CConfig.ValToCon(a6                , ref OM.EzSensorInfo.iDescramble     );
                CConfig.ValToCon(a7                , ref OM.EzSensorInfo.iVRest          );
                CConfig.ValToCon(a9                , ref OM.EzSensorInfo.iBinning1x1     );
                CConfig.ValToCon(a10               , ref OM.EzSensorInfo.iBinning2x2     );
                CConfig.ValToCon(a11               , ref OM.EzSensorInfo.iCutoffbevel1x1 );
                CConfig.ValToCon(a12               , ref OM.EzSensorInfo.iCutoffbevel2x2 );
                CConfig.ValToCon(a13               , ref OM.EzSensorInfo.bBright61       );
                CConfig.ValToCon(a14               , ref OM.EzSensorInfo.iGain           );
                CConfig.ValToCon(a15               , ref OM.EzSensorInfo.iMode           );
                CConfig.ValToCon(a16               , ref OM.EzSensorInfo.iCutoffR        );
                CConfig.ValToCon(a17               , ref OM.EzSensorInfo.iPattern        );
                CConfig.ValToCon(a18               , ref OM.EzSensorInfo.bDebugdump      );
                CConfig.ValToCon(a19               , ref OM.EzSensorInfo.iNsWidth        );
                CConfig.ValToCon(a20               , ref OM.EzSensorInfo.iNsHeight       );
                CConfig.ValToCon(a21               , ref OM.EzSensorInfo.sAppPath1       );
                CConfig.ValToCon(a22               , ref OM.EzSensorInfo.sAppName1       );
                CConfig.ValToCon(a25               , ref OM.EzSensorInfo.sAppPath2       );
                CConfig.ValToCon(a26               , ref OM.EzSensorInfo.sAppName2       );
                CConfig.ValToCon(a29               , ref OM.EzSensorInfo.sAppPath3       );
                CConfig.ValToCon(a30               , ref OM.EzSensorInfo.sAppName3       );
                CConfig.ValToCon(a23               , ref OM.EzSensorInfo.iPosX1          );
                CConfig.ValToCon(a24               , ref OM.EzSensorInfo.iPosY1          );
                CConfig.ValToCon(a27               , ref OM.EzSensorInfo.iPosX2          );
                CConfig.ValToCon(a28               , ref OM.EzSensorInfo.iPosY2          );
                CConfig.ValToCon(a31               , ref OM.EzSensorInfo.iPosX3          );
                CConfig.ValToCon(a32               , ref OM.EzSensorInfo.iPosY3          );
                CConfig.ValToCon(a33               , ref OM.EzSensorInfo.sIniPath1       );
                CConfig.ValToCon(a34               , ref OM.EzSensorInfo.sIniPath2       );
                CConfig.ValToCon(a35               , ref OM.EzSensorInfo.sRsltPath       );
                CConfig.ValToCon(tbDarkImg1x1      , ref OM.EzSensorInfo.sDarkImgPath1x1 );
                CConfig.ValToCon(tbDarkImg2x2      , ref OM.EzSensorInfo.sDarkImgPath2x2 );

                //입고공정~Skull Setting
                //입고공정
                CConfig.ValToCon(tbEntr1x1Width    , ref OM.EzSensorInfo.iEntr1x1Width   );
                CConfig.ValToCon(tbEntr1x1Hght     , ref OM.EzSensorInfo.iEntr1x1Hght    );
                CConfig.ValToCon(tbEntr2x2Width    , ref OM.EzSensorInfo.iEntr2x2Width   );
                CConfig.ValToCon(tbEntr2x2Hght     , ref OM.EzSensorInfo.iEntr2x2Hght    );

                //Dark Aging
                CConfig.ValToCon(tbAg1x1Width      , ref OM.EzSensorInfo.iAg1x1Width     );
                CConfig.ValToCon(tbAg1x1Hght       , ref OM.EzSensorInfo.iAg1x1Hght      );
                CConfig.ValToCon(tbAg2x2Width      , ref OM.EzSensorInfo.iAg2x2Width     );
                CConfig.ValToCon(tbAg2x2Hght       , ref OM.EzSensorInfo.iAg2x2Hght      );
                CConfig.ValToCon(cbAgRotate        , ref OM.EzSensorInfo.iAgRotate       );
                CConfig.ValToCon(cbAgFlipHorz      , ref OM.EzSensorInfo.bAgFlipHorz     );
                CConfig.ValToCon(cbAgFlipVert      , ref OM.EzSensorInfo.bAgFlipVert     );
                CConfig.ValToCon(tbAgCropTop       , ref OM.EzSensorInfo.iAgCropTop      );
                CConfig.ValToCon(tbAgCropLeft      , ref OM.EzSensorInfo.iAgCropLeft     );
                CConfig.ValToCon(tbAgCropRight     , ref OM.EzSensorInfo.iAgCropRight    );
                CConfig.ValToCon(tbAgCropBtm       , ref OM.EzSensorInfo.iAgCropBtm      );
                CConfig.ValToCon(tbAcqMaxFrame1x1  , ref OM.EzSensorInfo.iAcqMaxFrame1x1 );
                CConfig.ValToCon(tbAcqInterval1x1  , ref OM.EzSensorInfo.iAcqInterval1x1 );

                CConfig.ValToCon(tbAcqMaxFrame2x2  , ref OM.EzSensorInfo.iAcqMaxFrame2x2 );
                CConfig.ValToCon(tbAcqInterval2x2  , ref OM.EzSensorInfo.iAcqInterval2x2 );

                //특성검사(평가)
                CConfig.ValToCon(tbDimWidth1x1        , ref OM.EzSensorInfo.iDimWidth1x1       );
                CConfig.ValToCon(tbDimHght1x1         , ref OM.EzSensorInfo.iDimHght1x1        );
                CConfig.ValToCon(tbPixelPitch1x1      , ref OM.EzSensorInfo.dPixelPitch1x1     );
                CConfig.ValToCon(tbDimWidth2x2        , ref OM.EzSensorInfo.iDimWidth2x2       );
                CConfig.ValToCon(tbDimHght2x2         , ref OM.EzSensorInfo.iDimHght2x2        );
                CConfig.ValToCon(tbPixelPitch2x2      , ref OM.EzSensorInfo.dPixelPitch2x2     );

                CConfig.ValToCon(tbNPSLeft1x1         , ref OM.EzSensorInfo.iNPSLeft1x1        );
                CConfig.ValToCon(tbNPSTop1x1          , ref OM.EzSensorInfo.iNPSTop1x1         );
                CConfig.ValToCon(tbNPSW1x1            , ref OM.EzSensorInfo.iNPSW1x1           );
                CConfig.ValToCon(tbNPSH1x1            , ref OM.EzSensorInfo.iNPSH1x1           );

                CConfig.ValToCon(tbMTFLeft1x1         , ref OM.EzSensorInfo.iMTFLeft1x1        );
                CConfig.ValToCon(tbMTFTop1x1          , ref OM.EzSensorInfo.iMTFTop1x1         );
                CConfig.ValToCon(tbMTFW1x1            , ref OM.EzSensorInfo.iMTFW1x1           );
                CConfig.ValToCon(tbMTFH1x1            , ref OM.EzSensorInfo.iMTFH1x1           );

                CConfig.ValToCon(tbNPSLeft2x2         , ref OM.EzSensorInfo.iNPSLeft2x2        );
                CConfig.ValToCon(tbNPSTop2x2          , ref OM.EzSensorInfo.iNPSTop2x2         );
                CConfig.ValToCon(tbNPSW2x2            , ref OM.EzSensorInfo.iNPSW2x2           );
                CConfig.ValToCon(tbNPSH2x2            , ref OM.EzSensorInfo.iNPSH2x2           );

                CConfig.ValToCon(tbMTFLeft2x2         , ref OM.EzSensorInfo.iMTFLeft2x2        );
                CConfig.ValToCon(tbMTFTop2x2          , ref OM.EzSensorInfo.iMTFTop2x2         );
                CConfig.ValToCon(tbMTFW2x2            , ref OM.EzSensorInfo.iMTFW2x2           );
                CConfig.ValToCon(tbMTFH2x2            , ref OM.EzSensorInfo.iMTFH2x2           );

                CConfig.ValToCon(tbDoze            , ref OM.EzSensorInfo.dDoze           );

                CConfig.ValToCon(cbSkRotate        , ref OM.EzSensorInfo.iSkRotate       );
                CConfig.ValToCon(cbSkFlipHorz      , ref OM.EzSensorInfo.bSkFlipHorz     );
                CConfig.ValToCon(cbSkFlipVert      , ref OM.EzSensorInfo.bSkFlipVert     );
                CConfig.ValToCon(tbSkCropTop       , ref OM.EzSensorInfo.iSkCropTop      );
                CConfig.ValToCon(tbSkCropLeft      , ref OM.EzSensorInfo.iSkCropLeft     );
                CConfig.ValToCon(tbSkCropRight     , ref OM.EzSensorInfo.iSkCropRight    );
                CConfig.ValToCon(tbSkCropBtm       , ref OM.EzSensorInfo.iSkCropBtm      );

                if (OM.DevInfo.iMacroType == 1)
                {
                    //Ver 2019.10.23.1 
                    //EzSensor 콤보박스 item 옵션에 따라 달리 보여줘야해서 바인딩 함수 추가
                    DescrableDataBinding();
                }
            }
            else 
            {
                //DeviceInfo
                OM.CDevInfo    DevInfo    = OM.DevInfo;
                OM.CDressyInfo DressyInfo = OM.DressyInfo;
                OM.CEzSensorInfo   EzSensorInfo   = OM.EzSensorInfo  ;
                CConfig.ConToVal(tbTRAY_PcktCntX   , ref OM.DevInfo.iTRAY_PcktCntX   );
                CConfig.ConToVal(tbTRAY_PcktPitchX , ref OM.DevInfo.dTRAY_PcktPitchX );
                CConfig.ConToVal(tbLODR_SlotCnt    , ref OM.DevInfo.iLODR_SlotCnt    );
                CConfig.ConToVal(tbLODR_SlotPitch  , ref OM.DevInfo.dLODR_SlotPitch  );
               
                //XRAY 옵션
                CConfig.ConToVal(cbUseUSBOptn      , ref OM.DevInfo.iUseUSBOptn      );
                
                CConfig.ConToVal(tbCoolingTime     , ref OM.DevInfo.iCoolingTime     );

                //Macro
                CConfig.ConToVal(cbMacroType       , ref OM.DevInfo.iMacroType       );
                
                //Dressy 공용 옵션
                CConfig.ConToVal(cbIgnrCycleAnalyze, ref OM.DressyInfo.bIgnrCycleAnalyze);
                CConfig.ConToVal(cbIgnrCycleCheck  , ref OM.DressyInfo.bIgnrCycleCheck  );
                CConfig.ConToVal(tbBfXrayDelay     , ref OM.DressyInfo.iBfXrayDelay     );
                CConfig.ConToVal(cbSelGetDarkBtn   , ref OM.DressyInfo.iSelGetDarkBtn   );
                CConfig.ConToVal(tbTolerance       , ref OM.DressyInfo.iTolerance       );
                if (int.Parse(tbCalDeleteDelay.Text) >= 30000)
                {
                    tbCalDeleteDelay.Text = "30000";
                    Log.ShowMessage("Warning", "딜레이 30초 이상 설정 시 Macro Cycle Time Out Error가 발생 할 수 있습니다.");
                }
                CConfig.ConToVal(tbCalDeleteDelay  , ref OM.DressyInfo.iCalDeleteDelay  );
                //1.0.1.7 옵션 추가
                CConfig.ConToVal(cbTrgErrProc      , ref OM.DressyInfo.iTrgErrProc      );
                CConfig.ConToVal(cbCalRptErrProc   , ref OM.DressyInfo.iCalRptErrProc   );
                CConfig.ConToVal(tbCalRptCnt       , ref OM.DressyInfo.iCalRptCnt       );

                //DressyInfo
                CConfig.ConToVal(b1                , ref OM.DressyInfo.s1            );
                CConfig.ConToVal(b2                , ref OM.DressyInfo.s2            );
                CConfig.ConToVal(b3                , ref OM.DressyInfo.s3            );
                CConfig.ConToVal(b4                , ref OM.DressyInfo.s4            );
                CConfig.ConToVal(b5                , ref OM.DressyInfo.s5            );
                CConfig.ConToVal(b6                , ref OM.DressyInfo.s6            );
                CConfig.ConToVal(b7                , ref OM.DressyInfo.s7            );
                CConfig.ConToVal(b8                , ref OM.DressyInfo.s8            );
                CConfig.ConToVal(b9                , ref OM.DressyInfo.s9            );
                CConfig.ConToVal(bA                , ref OM.DressyInfo.sA            );
                CConfig.ConToVal(bB                , ref OM.DressyInfo.sB            );
                CConfig.ConToVal(bC                , ref OM.DressyInfo.sC            );
                CConfig.ConToVal(bD                , ref OM.DressyInfo.sD            );
                
                CConfig.ConToVal(b11               , ref OM.DressyInfo.sAppPath1     );
                CConfig.ConToVal(b12               , ref OM.DressyInfo.sAppName1     );
                CConfig.ConToVal(b13               , ref OM.DressyInfo.iPosX1        );
                CConfig.ConToVal(b14               , ref OM.DressyInfo.iPosY1        );
                CConfig.ConToVal(b15               , ref OM.DressyInfo.sAppPath2     );
                CConfig.ConToVal(b16               , ref OM.DressyInfo.sAppName2     );
                CConfig.ConToVal(b17               , ref OM.DressyInfo.iPosX2        );
                CConfig.ConToVal(b18               , ref OM.DressyInfo.iPosY2        );
                CConfig.ConToVal(b19               , ref OM.DressyInfo.sAppPath3     );
                CConfig.ConToVal(b20               , ref OM.DressyInfo.sAppName3     );
                CConfig.ConToVal(b21               , ref OM.DressyInfo.iPosX3        );
                CConfig.ConToVal(b22               , ref OM.DressyInfo.iPosY3        );
                CConfig.ConToVal(b23               , ref OM.DressyInfo.sIniPath1     );
                CConfig.ConToVal(b24               , ref OM.DressyInfo.sIniPath2     );
                CConfig.ConToVal(b25               , ref OM.DressyInfo.sRsltPath     );
                CConfig.ConToVal(b26               , ref OM.DressyInfo.sCalPath      );

                //NPS ROI 1x1 Setting
                CConfig.ConToVal(tbNPSArea1        , ref OM.DressyInfo.iNPSArea1     );
                CConfig.ConToVal(tbNPSLeft1        , ref OM.DressyInfo.iNPSLeft1     );
                CConfig.ConToVal(tbNPSSub1         , ref OM.DressyInfo.iNPSSub1      );
                CConfig.ConToVal(tbNPSTop1         , ref OM.DressyInfo.iNPSTop1      );

                //NPS ROI 2x2
                CConfig.ConToVal(tbNPSArea2        , ref OM.DressyInfo.iNPSArea2     );
                CConfig.ConToVal(tbNPSLeft2        , ref OM.DressyInfo.iNPSLeft2     );
                CConfig.ConToVal(tbNPSSub2         , ref OM.DressyInfo.iNPSSub2      );
                CConfig.ConToVal(tbNPSTop2         , ref OM.DressyInfo.iNPSTop2      );

                //1x1 MTF ROI
                CConfig.ConToVal(tbMTFROILeft1     , ref OM.DressyInfo.iMTFROILeft1  );
                CConfig.ConToVal(tbMTFROILen1      , ref OM.DressyInfo.iMTFROILen1   );
                CConfig.ConToVal(tbMTFROINum1      , ref OM.DressyInfo.iMTFROINum1   );
                CConfig.ConToVal(tbMTFROITop1      , ref OM.DressyInfo.iMTFROITop1   );

                //1x1 MTF Edge
                CConfig.ConToVal(tbMTFEgHght1      , ref OM.DressyInfo.iMTFEgHght1   );
                CConfig.ConToVal(tbMTFEgLeft1      , ref OM.DressyInfo.iMTFEgLeft1   );
                CConfig.ConToVal(tbMTFEgTop1       , ref OM.DressyInfo.iMTFEgTop1    );
                CConfig.ConToVal(tbMTFEgWidth1     , ref OM.DressyInfo.iMTFEgWidth1  );

                //2x2 MTF ROI
                CConfig.ConToVal(tbMTFROILeft2     , ref OM.DressyInfo.iMTFROILeft2  );
                CConfig.ConToVal(tbMTFROILen2      , ref OM.DressyInfo.iMTFROILen2   );
                CConfig.ConToVal(tbMTFROINum2      , ref OM.DressyInfo.iMTFROINum2   );
                CConfig.ConToVal(tbMTFROITop2      , ref OM.DressyInfo.iMTFROITop2   );

                //2x2 MTF Edge
                CConfig.ConToVal(tbMTFEgHght2      , ref OM.DressyInfo.iMTFEgHght2   );
                CConfig.ConToVal(tbMTFEgLeft2      , ref OM.DressyInfo.iMTFEgLeft2   );
                CConfig.ConToVal(tbMTFEgTop2       , ref OM.DressyInfo.iMTFEgTop2    );
                CConfig.ConToVal(tbMTFEgWidth2     , ref OM.DressyInfo.iMTFEgWidth2  );

                //Write 할때 쑤셔넣는놈들
                CConfig.ConToVal(tbCode            , ref OM.DressyInfo.sProductCode  );
                CConfig.ConToVal(tbVer             , ref OM.DressyInfo.sProductVer   );
                CConfig.ConToVal(tbFPGA            , ref OM.DressyInfo.sFPGAVer      );
                CConfig.ConToVal(tbAcqSW           , ref OM.DressyInfo.sAcqSW        );
                CConfig.ConToVal(tbEvalSW          , ref OM.DressyInfo.sEvalSW       );
                CConfig.ConToVal(tbPerform         , ref OM.DressyInfo.sPerform      );
                
                //EzSensor Info
                CConfig.ConToVal(cbEzType          , ref OM.EzSensorInfo.iEzType         );
                CConfig.ConToVal(cbImgSize         , ref OM.EzSensorInfo.iImgSize        );
                CConfig.ConToVal(tbEzGbCnt         , ref OM.EzSensorInfo.iEzGbCnt        );
                CConfig.ConToVal(tbGbDelay         , ref OM.EzSensorInfo.iGbDelay        );
                CConfig.ConToVal(tbBfMacDelay      , ref OM.EzSensorInfo.iBfMacDelay     );
                CConfig.ConToVal(tbAtMacDelay      , ref OM.EzSensorInfo.iAtMacDelay     );
                CConfig.ConToVal(cbUseIOSPgm       , ref OM.EzSensorInfo.bUseIOSPgm      );
                CConfig.ConToVal(a1                , ref OM.EzSensorInfo.iProductID      );
                CConfig.ConToVal(a2                , ref OM.EzSensorInfo.bInvertacq      );
                CConfig.ConToVal(a3                , ref OM.EzSensorInfo.iTimeout        );
                CConfig.ConToVal(a4                , ref OM.EzSensorInfo.iOntheFly       );
                CConfig.ConToVal(a5                , ref OM.EzSensorInfo.bEnableSerial   );
                CConfig.ConToVal(a6                , ref OM.EzSensorInfo.iDescramble     );
                CConfig.ConToVal(a7                , ref OM.EzSensorInfo.iVRest          );
                CConfig.ConToVal(a9                , ref OM.EzSensorInfo.iBinning1x1     );
                CConfig.ConToVal(a10               , ref OM.EzSensorInfo.iBinning2x2     );
                CConfig.ConToVal(a11               , ref OM.EzSensorInfo.iCutoffbevel1x1 );
                CConfig.ConToVal(a12               , ref OM.EzSensorInfo.iCutoffbevel2x2 );
                CConfig.ConToVal(a13               , ref OM.EzSensorInfo.bBright61       );
                CConfig.ConToVal(a14               , ref OM.EzSensorInfo.iGain           );
                CConfig.ConToVal(a15               , ref OM.EzSensorInfo.iMode           );
                CConfig.ConToVal(a16               , ref OM.EzSensorInfo.iCutoffR        );
                CConfig.ConToVal(a17               , ref OM.EzSensorInfo.iPattern        );
                CConfig.ConToVal(a18               , ref OM.EzSensorInfo.bDebugdump      );
                CConfig.ConToVal(a19               , ref OM.EzSensorInfo.iNsWidth        );
                CConfig.ConToVal(a20               , ref OM.EzSensorInfo.iNsHeight       );
                
                CConfig.ConToVal(a21               , ref OM.EzSensorInfo.sAppPath1       );
                CConfig.ConToVal(a22               , ref OM.EzSensorInfo.sAppName1       );
                CConfig.ConToVal(a25               , ref OM.EzSensorInfo.sAppPath2       );
                CConfig.ConToVal(a26               , ref OM.EzSensorInfo.sAppName2       );
                CConfig.ConToVal(a29               , ref OM.EzSensorInfo.sAppPath3       );
                CConfig.ConToVal(a30               , ref OM.EzSensorInfo.sAppName3       );
                CConfig.ConToVal(a23               , ref OM.EzSensorInfo.iPosX1          );
                CConfig.ConToVal(a24               , ref OM.EzSensorInfo.iPosY1          );
                CConfig.ConToVal(a27               , ref OM.EzSensorInfo.iPosX2          );
                CConfig.ConToVal(a28               , ref OM.EzSensorInfo.iPosY2          );
                CConfig.ConToVal(a31               , ref OM.EzSensorInfo.iPosX3          );
                CConfig.ConToVal(a32               , ref OM.EzSensorInfo.iPosY3          );
                CConfig.ConToVal(a33               , ref OM.EzSensorInfo.sIniPath1       );
                CConfig.ConToVal(a34               , ref OM.EzSensorInfo.sIniPath2       );
                CConfig.ConToVal(a35               , ref OM.EzSensorInfo.sRsltPath       );
                CConfig.ConToVal(tbDarkImg1x1      , ref OM.EzSensorInfo.sDarkImgPath1x1 );
                CConfig.ConToVal(tbDarkImg2x2      , ref OM.EzSensorInfo.sDarkImgPath2x2 );

                //입고공정~Skull Setting
                //입고공정
                CConfig.ConToVal(tbEntr1x1Width    , ref OM.EzSensorInfo.iEntr1x1Width   );
                CConfig.ConToVal(tbEntr1x1Hght     , ref OM.EzSensorInfo.iEntr1x1Hght    );
                CConfig.ConToVal(tbEntr2x2Width    , ref OM.EzSensorInfo.iEntr2x2Width   );
                CConfig.ConToVal(tbEntr2x2Hght     , ref OM.EzSensorInfo.iEntr2x2Hght    );

                //Dark Aging
                CConfig.ConToVal(tbAg1x1Width      , ref OM.EzSensorInfo.iAg1x1Width     );
                CConfig.ConToVal(tbAg1x1Hght       , ref OM.EzSensorInfo.iAg1x1Hght      );
                CConfig.ConToVal(tbAg2x2Width      , ref OM.EzSensorInfo.iAg2x2Width     );
                CConfig.ConToVal(tbAg2x2Hght       , ref OM.EzSensorInfo.iAg2x2Hght      );
                CConfig.ConToVal(cbAgRotate        , ref OM.EzSensorInfo.iAgRotate       );
                CConfig.ConToVal(cbAgFlipHorz      , ref OM.EzSensorInfo.bAgFlipHorz     );
                CConfig.ConToVal(cbAgFlipVert      , ref OM.EzSensorInfo.bAgFlipVert     );
                CConfig.ConToVal(tbAgCropTop       , ref OM.EzSensorInfo.iAgCropTop      );
                CConfig.ConToVal(tbAgCropLeft      , ref OM.EzSensorInfo.iAgCropLeft     );
                CConfig.ConToVal(tbAgCropRight     , ref OM.EzSensorInfo.iAgCropRight    );
                CConfig.ConToVal(tbAgCropBtm       , ref OM.EzSensorInfo.iAgCropBtm      );
                CConfig.ConToVal(tbAcqMaxFrame1x1  , ref OM.EzSensorInfo.iAcqMaxFrame1x1 );
                CConfig.ConToVal(tbAcqInterval1x1  , ref OM.EzSensorInfo.iAcqInterval1x1 );

                CConfig.ConToVal(tbAcqMaxFrame2x2  , ref OM.EzSensorInfo.iAcqMaxFrame2x2 );
                CConfig.ConToVal(tbAcqInterval2x2  , ref OM.EzSensorInfo.iAcqInterval2x2 );

                //특성검사(평가)
                CConfig.ConToVal(tbDimWidth1x1        , ref OM.EzSensorInfo.iDimWidth1x1       );
                CConfig.ConToVal(tbDimHght1x1         , ref OM.EzSensorInfo.iDimHght1x1        );
                CConfig.ConToVal(tbPixelPitch1x1      , ref OM.EzSensorInfo.dPixelPitch1x1     );
                CConfig.ConToVal(tbDimWidth2x2        , ref OM.EzSensorInfo.iDimWidth2x2       );
                CConfig.ConToVal(tbDimHght2x2         , ref OM.EzSensorInfo.iDimHght2x2        );
                CConfig.ConToVal(tbPixelPitch2x2      , ref OM.EzSensorInfo.dPixelPitch2x2     );

                CConfig.ConToVal(tbNPSLeft1x1         , ref OM.EzSensorInfo.iNPSLeft1x1        );
                CConfig.ConToVal(tbNPSTop1x1          , ref OM.EzSensorInfo.iNPSTop1x1         );
                CConfig.ConToVal(tbNPSW1x1            , ref OM.EzSensorInfo.iNPSW1x1           );
                CConfig.ConToVal(tbNPSH1x1            , ref OM.EzSensorInfo.iNPSH1x1           );

                CConfig.ConToVal(tbMTFLeft1x1         , ref OM.EzSensorInfo.iMTFLeft1x1        );
                CConfig.ConToVal(tbMTFTop1x1          , ref OM.EzSensorInfo.iMTFTop1x1         );
                CConfig.ConToVal(tbMTFW1x1            , ref OM.EzSensorInfo.iMTFW1x1           );
                CConfig.ConToVal(tbMTFH1x1            , ref OM.EzSensorInfo.iMTFH1x1           );

                CConfig.ConToVal(tbNPSLeft2x2         , ref OM.EzSensorInfo.iNPSLeft2x2        );
                CConfig.ConToVal(tbNPSTop2x2          , ref OM.EzSensorInfo.iNPSTop2x2         );
                CConfig.ConToVal(tbNPSW2x2            , ref OM.EzSensorInfo.iNPSW2x2           );
                CConfig.ConToVal(tbNPSH2x2            , ref OM.EzSensorInfo.iNPSH2x2           );

                CConfig.ConToVal(tbMTFLeft2x2         , ref OM.EzSensorInfo.iMTFLeft2x2        );
                CConfig.ConToVal(tbMTFTop2x2          , ref OM.EzSensorInfo.iMTFTop2x2         );
                CConfig.ConToVal(tbMTFW2x2            , ref OM.EzSensorInfo.iMTFW2x2           );
                CConfig.ConToVal(tbMTFH2x2            , ref OM.EzSensorInfo.iMTFH2x2           );

                CConfig.ConToVal(tbDoze            , ref OM.EzSensorInfo.dDoze           );

                CConfig.ConToVal(cbSkRotate        , ref OM.EzSensorInfo.iSkRotate       );
                CConfig.ConToVal(cbSkFlipHorz      , ref OM.EzSensorInfo.bSkFlipHorz     );
                CConfig.ConToVal(cbSkFlipVert      , ref OM.EzSensorInfo.bSkFlipVert     );
                CConfig.ConToVal(tbSkCropTop       , ref OM.EzSensorInfo.iSkCropTop      );
                CConfig.ConToVal(tbSkCropLeft      , ref OM.EzSensorInfo.iSkCropLeft     );
                CConfig.ConToVal(tbSkCropRight     , ref OM.EzSensorInfo.iSkCropRight    );
                CConfig.ConToVal(tbSkCropBtm       , ref OM.EzSensorInfo.iSkCropBtm      );

                //Auto Log
                Type type = DevInfo.GetType();
                FieldInfo[] f = type.GetFields(BindingFlags.Public|BindingFlags.NonPublic | BindingFlags.Instance);
                for(int i = 0 ; i < f.Length ; i++)
                {
                    Trace(f[i].Name, f[i].GetValue(DevInfo).ToString(), f[i].GetValue(OM.DevInfo).ToString());
                }

                //Macro Dressy
                if (OM.DevInfo.iMacroType == 0)
                {
                    Type Dressytype = DressyInfo.GetType();
                    FieldInfo[] Dressyf = Dressytype.GetFields(BindingFlags.Public|BindingFlags.NonPublic | BindingFlags.Instance);
                    for(int i = 0 ; i < Dressyf.Length ; i++)
                    {
                        Trace(Dressyf[i].Name, Dressyf[i].GetValue(DressyInfo).ToString(), Dressyf[i].GetValue(OM.DressyInfo).ToString());
                    }
                }
                //Macro EzSensor
                else if (OM.DevInfo.iMacroType == 1)
                {
                    Type Eztype = EzSensorInfo.GetType();
                    FieldInfo[] Ezf = Eztype.GetFields(BindingFlags.Public|BindingFlags.NonPublic | BindingFlags.Instance);
                    for(int i = 0 ; i < Ezf.Length ; i++)
                    {
                        Trace(Ezf[i].Name, Ezf[i].GetValue(EzSensorInfo).ToString(), Ezf[i].GetValue(OM.EzSensorInfo).ToString());
                    }
                }

                /*
                Trace(lbTrayPcktCnt  .Text, DevInfo.iTRAY_PcktCntX  .ToString(), OM.DevInfo.iTRAY_PcktCntX  .ToString());
                Trace(lbTrayPcktPitch.Text, DevInfo.dTRAY_PcktPitchX.ToString(), OM.DevInfo.dTRAY_PcktPitchX.ToString());
                Trace(lbLdrSlotCnt   .Text, DevInfo.iLODR_SlotCnt   .ToString(), OM.DevInfo.iLODR_SlotCnt   .ToString());
                Trace(lbLdrSlotPitch .Text, DevInfo.dLODR_SlotPitch .ToString(), OM.DevInfo.dLODR_SlotPitch .ToString());
                
                //X-Ray 옵션
                
                Trace(label3         .Text, DevInfo.iUseUSBOptn     .ToString(), OM.DevInfo.iUseUSBOptn     .ToString());
                Trace(cbUseCycleCheck.Text, DevInfo.bUseCycleCheck  .ToString(), OM.DevInfo.bUseCycleCheck  .ToString());
                Trace(label198       .Text, DevInfo.iCoolingTime    .ToString(), OM.DevInfo.iCoolingTime    .ToString());

                //Macro Dressy
                if (OM.DevInfo.iMacroType == 0)
                {
                    Trace(label90.Text, DressyInfo.s1.ToString(), OM.DressyInfo.s1.ToString());
                    Trace(label91.Text, DressyInfo.s2.ToString(), OM.DressyInfo.s2.ToString());
                    Trace(label92.Text, DressyInfo.s3.ToString(), OM.DressyInfo.s3.ToString());
                    Trace(label93.Text, DressyInfo.s4.ToString(), OM.DressyInfo.s4.ToString());
                    Trace(label87.Text, DressyInfo.s5.ToString(), OM.DressyInfo.s5.ToString());
                    Trace(label94.Text, DressyInfo.s6.ToString(), OM.DressyInfo.s6.ToString());
                    Trace(label95.Text, DressyInfo.s7.ToString(), OM.DressyInfo.s7.ToString());
                    Trace(label16.Text, DressyInfo.s8.ToString(), OM.DressyInfo.s8.ToString());
                    Trace(label17.Text, DressyInfo.s9.ToString(), OM.DressyInfo.s9.ToString());
                    Trace(label21.Text, DressyInfo.sA.ToString(), OM.DressyInfo.sA.ToString());
                    Trace(label22.Text, DressyInfo.sB.ToString(), OM.DressyInfo.sB.ToString());
                    Trace(label36.Text, DressyInfo.sC.ToString(), OM.DressyInfo.sC.ToString());
                    Trace(label35.Text, DressyInfo.sD.ToString(), OM.DressyInfo.sD.ToString());

                    Trace(groupBox28.Text + label80.Text, DressyInfo.sAppPath1.ToString(), OM.DressyInfo.sAppPath1.ToString());
                    Trace(groupBox28.Text + label81.Text, DressyInfo.sAppName1.ToString(), OM.DressyInfo.sAppName1.ToString());
                    Trace(groupBox28.Text + label82.Text, DressyInfo.iPosX1   .ToString(), OM.DressyInfo.iPosX1   .ToString());
                    Trace(groupBox28.Text + label83.Text, DressyInfo.iPosY1   .ToString(), OM.DressyInfo.iPosY1   .ToString());
                    Trace(groupBox27.Text + label76.Text, DressyInfo.sAppPath2.ToString(), OM.DressyInfo.sAppPath2.ToString());
                    Trace(groupBox27.Text + label77.Text, DressyInfo.sAppName2.ToString(), OM.DressyInfo.sAppName2.ToString());
                    Trace(groupBox27.Text + label78.Text, DressyInfo.iPosX2   .ToString(), OM.DressyInfo.iPosX2   .ToString());
                    Trace(groupBox27.Text + label79.Text, DressyInfo.iPosY2   .ToString(), OM.DressyInfo.iPosY2   .ToString());
                    Trace(groupBox30.Text + label96.Text, DressyInfo.sAppPath3.ToString(), OM.DressyInfo.sAppPath3.ToString());
                    Trace(groupBox30.Text + label97.Text, DressyInfo.sAppName3.ToString(), OM.DressyInfo.sAppName3.ToString());
                    Trace(groupBox30.Text + label98.Text, DressyInfo.iPosX3   .ToString(), OM.DressyInfo.iPosX3   .ToString());
                    Trace(groupBox30.Text + label99.Text, DressyInfo.iPosY3   .ToString(), OM.DressyInfo.iPosY3   .ToString());
                    Trace(                  label37.Text, DressyInfo.sIniPath1.ToString(), OM.DressyInfo.sIniPath1.ToString());
                    Trace(                  label6.Text , DressyInfo.sIniPath2.ToString(), OM.DressyInfo.sIniPath2.ToString());
                    Trace(                  label33.Text, DressyInfo.sRsltPath.ToString(), OM.DressyInfo.sRsltPath.ToString());
                    Trace(                  label38.Text, DressyInfo.sCalPath .ToString(), OM.DressyInfo.sCalPath .ToString());

                    //NPS ROI
                    Trace(groupBox7.Text + label39.Text, DressyInfo.iNPSArea1.ToString(), OM.DressyInfo.iNPSArea1.ToString());
                    Trace(groupBox7.Text + label40.Text, DressyInfo.iNPSLeft1.ToString(), OM.DressyInfo.iNPSLeft1.ToString());
                    Trace(groupBox7.Text + label42.Text, DressyInfo.iNPSSub1 .ToString(), OM.DressyInfo.iNPSSub1 .ToString());
                    Trace(groupBox7.Text + label43.Text, DressyInfo.iNPSTop1 .ToString(), OM.DressyInfo.iNPSTop1 .ToString());

                    Trace(groupBox9.Text + label47.Text, DressyInfo.iNPSArea2.ToString(), OM.DressyInfo.iNPSArea2.ToString());
                    Trace(groupBox9.Text + label49.Text, DressyInfo.iNPSLeft2.ToString(), OM.DressyInfo.iNPSLeft2.ToString());
                    Trace(groupBox9.Text + label50.Text, DressyInfo.iNPSSub2 .ToString(), OM.DressyInfo.iNPSSub2 .ToString());
                    Trace(groupBox9.Text + label54.Text, DressyInfo.iNPSTop2 .ToString(), OM.DressyInfo.iNPSTop2 .ToString());

                    //1x1 MTF ROI
                    Trace(groupBox12.Text + label55.Text, DressyInfo.iMTFROILeft1.ToString(), OM.DressyInfo.iMTFROILeft1.ToString());
                    Trace(groupBox12.Text + label56.Text, DressyInfo.iMTFROILen1 .ToString(), OM.DressyInfo.iMTFROILen1 .ToString());
                    Trace(groupBox12.Text + label57.Text, DressyInfo.iMTFROINum1 .ToString(), OM.DressyInfo.iMTFROINum1 .ToString());
                    Trace(groupBox12.Text + label58.Text, DressyInfo.iMTFROITop1 .ToString(), OM.DressyInfo.iMTFROITop1 .ToString());

                    Trace(groupBox14.Text + label59.Text, DressyInfo.iMTFEgHght1 .ToString(), OM.DressyInfo.iMTFEgHght1 .ToString());
                    Trace(groupBox14.Text + label64.Text, DressyInfo.iMTFEgLeft1 .ToString(), OM.DressyInfo.iMTFEgLeft1 .ToString());
                    Trace(groupBox14.Text + label65.Text, DressyInfo.iMTFEgTop1  .ToString(), OM.DressyInfo.iMTFEgTop1  .ToString());
                    Trace(groupBox14.Text + label86.Text, DressyInfo.iMTFEgWidth1.ToString(), OM.DressyInfo.iMTFEgWidth1.ToString());

                    //2x2 MTF ROI
                    Trace(groupBox19.Text + label104.Text, DressyInfo.iMTFROILeft2.ToString(), OM.DressyInfo.iMTFROILeft2.ToString());
                    Trace(groupBox19.Text + label105.Text, DressyInfo.iMTFROILen2 .ToString(), OM.DressyInfo.iMTFROILen2 .ToString());
                    Trace(groupBox19.Text + label106.Text, DressyInfo.iMTFROINum2 .ToString(), OM.DressyInfo.iMTFROINum2 .ToString());
                    Trace(groupBox19.Text + label107.Text, DressyInfo.iMTFROITop2 .ToString(), OM.DressyInfo.iMTFROITop2 .ToString());

                    Trace(groupBox18.Text + label100.Text, DressyInfo.iMTFEgHght2 .ToString(), OM.DressyInfo.iMTFEgHght2 .ToString());
                    Trace(groupBox18.Text + label101.Text, DressyInfo.iMTFEgLeft2 .ToString(), OM.DressyInfo.iMTFEgLeft2 .ToString());
                    Trace(groupBox18.Text + label102.Text, DressyInfo.iMTFEgTop2  .ToString(), OM.DressyInfo.iMTFEgTop2  .ToString());
                    Trace(groupBox18.Text + label103.Text, DressyInfo.iMTFEgWidth2.ToString(), OM.DressyInfo.iMTFEgWidth2.ToString());

                    //Write 할때 쑤셔넣는 놈들
                    Trace(groupBox10.Text + label135.Text, DressyInfo.sProductCode   , OM.DressyInfo.sProductCode);
                    Trace(groupBox10.Text + label136.Text, DressyInfo.sProductVer    , OM.DressyInfo.sProductVer );
                    Trace(groupBox10.Text + label137.Text, DressyInfo.sFPGAVer       , OM.DressyInfo.sFPGAVer    );
                    Trace(groupBox10.Text + label138.Text, DressyInfo.sAcqSW         , OM.DressyInfo.sAcqSW      );
                    Trace(groupBox10.Text + label139.Text, DressyInfo.sEvalSW        , OM.DressyInfo.sEvalSW     );
                    Trace(groupBox10.Text + label181.Text, DressyInfo.sPerform       , OM.DressyInfo.sPerform    );
                }
                else if (OM.DevInfo.iMacroType == 1)//EzSensorInfo
                {
                    Trace(groupBox43.Text + label166.Text, EzSensorInfo.iEzType        .ToString(), OM.EzSensorInfo.iEzType        .ToString());
                    Trace(groupBox24.Text + label46 .Text, EzSensorInfo.iProductID     .ToString(), OM.EzSensorInfo.iProductID     .ToString());
                    Trace(groupBox24.Text + label60 .Text, EzSensorInfo.bInvertacq     .ToString(), OM.EzSensorInfo.bInvertacq     .ToString());
                    Trace(groupBox24.Text + label61 .Text, EzSensorInfo.iTimeout       .ToString(), OM.EzSensorInfo.iTimeout       .ToString());
                    Trace(groupBox24.Text + label62 .Text, EzSensorInfo.iOntheFly      .ToString(), OM.EzSensorInfo.iOntheFly      .ToString());
                    Trace(groupBox24.Text + label63 .Text, EzSensorInfo.bEnableSerial  .ToString(), OM.EzSensorInfo.bEnableSerial  .ToString());
                    Trace(groupBox24.Text + label66 .Text, EzSensorInfo.iDescramble    .ToString(), OM.EzSensorInfo.iDescramble    .ToString());
                    Trace(groupBox24.Text + label67 .Text, EzSensorInfo.iVRest         .ToString(), OM.EzSensorInfo.iVRest         .ToString());
                    Trace(groupBox23.Text + label29 .Text, EzSensorInfo.iBinning1x1    .ToString(), OM.EzSensorInfo.iBinning1x1    .ToString());
                    Trace(groupBox23.Text + label34 .Text, EzSensorInfo.iBinning2x2    .ToString(), OM.EzSensorInfo.iBinning2x2    .ToString());
                    Trace(groupBox23.Text + label32 .Text, EzSensorInfo.iCutoffbevel1x1.ToString(), OM.EzSensorInfo.iCutoffbevel1x1.ToString());
                    Trace(groupBox23.Text + label215.Text, EzSensorInfo.iCutoffbevel2x2.ToString(), OM.EzSensorInfo.iCutoffbevel2x2.ToString());
                    Trace(groupBox23.Text + label41 .Text, EzSensorInfo.bBright61      .ToString(), OM.EzSensorInfo.bBright61      .ToString());
                    Trace(groupBox23.Text + label5  .Text, EzSensorInfo.iGain          .ToString(), OM.EzSensorInfo.iGain          .ToString());
                    Trace(groupBox23.Text + label19 .Text, EzSensorInfo.iMode          .ToString(), OM.EzSensorInfo.iMode          .ToString());
                    Trace(groupBox23.Text + label24 .Text, EzSensorInfo.iCutoffR       .ToString(), OM.EzSensorInfo.iCutoffR       .ToString());
                    Trace(groupBox23.Text + label26 .Text, EzSensorInfo.iPattern       .ToString(), OM.EzSensorInfo.iPattern       .ToString());
                    Trace(                  a18     .Text, EzSensorInfo.bDebugdump     .ToString(), OM.EzSensorInfo.bDebugdump     .ToString());
                    Trace(groupBox22.Text + label30 .Text, EzSensorInfo.iNsWidth       .ToString(), OM.EzSensorInfo.iNsWidth       .ToString());
                    Trace(groupBox22.Text + label31 .Text, EzSensorInfo.iNsHeight      .ToString(), OM.EzSensorInfo.iNsHeight      .ToString());
                    Trace(groupBox22.Text + label4  .Text, EzSensorInfo.sDarkImgPath1x1           , OM.EzSensorInfo.sDarkImgPath1x1           );
                    Trace(groupBox22.Text + label94 .Text, EzSensorInfo.sDarkImgPath2x2           , OM.EzSensorInfo.sDarkImgPath2x2           );
                    
                    Trace(groupBox26.Text + label72 .Text, EzSensorInfo.sAppPath1                 , OM.EzSensorInfo.sAppPath1                 );
                    Trace(groupBox26.Text + label73 .Text, EzSensorInfo.sAppName1                 , OM.EzSensorInfo.sAppName1                 );
                    Trace(groupBox25.Text + label68 .Text, EzSensorInfo.sAppPath2                 , OM.EzSensorInfo.sAppPath2                 );
                    Trace(groupBox25.Text + label69 .Text, EzSensorInfo.sAppName2                 , OM.EzSensorInfo.sAppName2                 );
                    Trace(groupBox26.Text + label74 .Text, EzSensorInfo.iPosX1         .ToString(), OM.EzSensorInfo.iPosX1         .ToString());
                    Trace(groupBox26.Text + label75 .Text, EzSensorInfo.iPosY1         .ToString(), OM.EzSensorInfo.iPosY1         .ToString());
                    Trace(groupBox25.Text + label70 .Text, EzSensorInfo.iPosX2         .ToString(), OM.EzSensorInfo.iPosX2         .ToString());
                    Trace(groupBox25.Text + label71 .Text, EzSensorInfo.iPosY2         .ToString(), OM.EzSensorInfo.iPosY2         .ToString());

                    Trace(groupBox17.Text + label143.Text, EzSensorInfo.iEntr1x1Width  .ToString(), OM.EzSensorInfo.iEntr1x1Width  .ToString());
                    Trace(groupBox17.Text + label148.Text, EzSensorInfo.iEntr1x1Hght   .ToString(), OM.EzSensorInfo.iEntr1x1Hght   .ToString());
                    Trace(groupBox17.Text + label178.Text, EzSensorInfo.iEntr2x2Width  .ToString(), OM.EzSensorInfo.iEntr2x2Width  .ToString());
                    Trace(groupBox17.Text + label179.Text, EzSensorInfo.iEntr2x2Hght   .ToString(), OM.EzSensorInfo.iEntr2x2Hght   .ToString());
                    Trace(groupBox20.Text + label149.Text, EzSensorInfo.iAg1x1Width    .ToString(), OM.EzSensorInfo.iAg1x1Width    .ToString());
                    Trace(groupBox20.Text + label150.Text, EzSensorInfo.iAg1x1Hght     .ToString(), OM.EzSensorInfo.iAg1x1Hght     .ToString());
                    Trace(groupBox20.Text + label151.Text, EzSensorInfo.iAg2x2Width    .ToString(), OM.EzSensorInfo.iAg2x2Width    .ToString());
                    Trace(groupBox20.Text + label152.Text, EzSensorInfo.iAg2x2Hght     .ToString(), OM.EzSensorInfo.iAg2x2Hght     .ToString());
                    Trace(groupBox31.Text + label153.Text, EzSensorInfo.iAgRotate      .ToString(), OM.EzSensorInfo.iAgRotate      .ToString());
                    Trace(groupBox31.Text + label154.Text, EzSensorInfo.bAgFlipHorz    .ToString(), OM.EzSensorInfo.bAgFlipHorz    .ToString());
                    Trace(groupBox31.Text + label159.Text, EzSensorInfo.bAgFlipVert    .ToString(), OM.EzSensorInfo.bAgFlipVert    .ToString());
                    Trace(groupBox21.Text + label155.Text, EzSensorInfo.iAgCropTop     .ToString(), OM.EzSensorInfo.iAgCropTop     .ToString());
                    Trace(groupBox21.Text + label156.Text, EzSensorInfo.iAgCropLeft    .ToString(), OM.EzSensorInfo.iAgCropLeft    .ToString());
                    Trace(groupBox21.Text + label157.Text, EzSensorInfo.iAgCropRight   .ToString(), OM.EzSensorInfo.iAgCropRight   .ToString());
                    Trace(groupBox21.Text + label158.Text, EzSensorInfo.iAgCropBtm     .ToString(), OM.EzSensorInfo.iAgCropBtm     .ToString());
                    Trace(groupBox32.Text + label160.Text, EzSensorInfo.iAcqMaxFrame1x1.ToString(), OM.EzSensorInfo.iAcqMaxFrame1x1.ToString());
                    Trace(groupBox32.Text + label161.Text, EzSensorInfo.iAcqInterval1x1.ToString(), OM.EzSensorInfo.iAcqInterval1x1.ToString());

                    Trace(groupBox50.Text + label213.Text, EzSensorInfo.iAcqMaxFrame2x2.ToString(), OM.EzSensorInfo.iAcqMaxFrame2x2.ToString());
                    Trace(groupBox50.Text + label214.Text, EzSensorInfo.iAcqInterval2x2.ToString(), OM.EzSensorInfo.iAcqInterval2x2.ToString());

                    Trace(groupBox45.Text + label183.Text, EzSensorInfo.iDimWidth1x1   .ToString(), OM.EzSensorInfo.iDimWidth1x1   .ToString());
                    Trace(groupBox45.Text + label184.Text, EzSensorInfo.iDimHght1x1    .ToString(), OM.EzSensorInfo.iDimHght1x1    .ToString());
                    Trace(groupBox45.Text + label185.Text, EzSensorInfo.dPixelPitch1x1 .ToString(), OM.EzSensorInfo.dPixelPitch1x1 .ToString());

                    Trace(groupBox47.Text + label201.Text, EzSensorInfo.iDimWidth2x2   .ToString(), OM.EzSensorInfo.iDimWidth2x2   .ToString());
                    Trace(groupBox47.Text + label203.Text, EzSensorInfo.iDimHght2x2    .ToString(), OM.EzSensorInfo.iDimHght2x2    .ToString());
                    Trace(groupBox47.Text + label204.Text, EzSensorInfo.dPixelPitch2x2 .ToString(), OM.EzSensorInfo.dPixelPitch2x2 .ToString());

                    Trace(groupBox46.Text + label209.Text, EzSensorInfo.iNPSLeft1x1    .ToString(), OM.EzSensorInfo.iNPSLeft1x1    .ToString());
                    Trace(groupBox46.Text + label210.Text, EzSensorInfo.iNPSTop1x1     .ToString(), OM.EzSensorInfo.iNPSTop1x1     .ToString());
                    Trace(groupBox46.Text + label211.Text, EzSensorInfo.iNPSW1x1       .ToString(), OM.EzSensorInfo.iNPSW1x1       .ToString());
                    Trace(groupBox46.Text + label212.Text, EzSensorInfo.iNPSH1x1       .ToString(), OM.EzSensorInfo.iNPSH1x1       .ToString());
                                                                                       
                    Trace(groupBox34.Text + label205.Text, EzSensorInfo.iMTFLeft1x1    .ToString(), OM.EzSensorInfo.iMTFLeft1x1    .ToString());
                    Trace(groupBox34.Text + label206.Text, EzSensorInfo.iMTFTop1x1     .ToString(), OM.EzSensorInfo.iMTFTop1x1     .ToString());
                    Trace(groupBox34.Text + label207.Text, EzSensorInfo.iMTFW1x1       .ToString(), OM.EzSensorInfo.iMTFW1x1       .ToString());
                    Trace(groupBox34.Text + label208.Text, EzSensorInfo.iMTFH1x1       .ToString(), OM.EzSensorInfo.iMTFH1x1       .ToString());
                          
                    Trace(groupBox49.Text + label183.Text, EzSensorInfo.iNPSLeft2x2    .ToString(), OM.EzSensorInfo.iNPSLeft2x2    .ToString());
                    Trace(groupBox49.Text + label184.Text, EzSensorInfo.iNPSTop2x2     .ToString(), OM.EzSensorInfo.iNPSTop2x2     .ToString());
                    Trace(groupBox49.Text + label185.Text, EzSensorInfo.iNPSW2x2       .ToString(), OM.EzSensorInfo.iNPSW2x2       .ToString());
                    Trace(groupBox49.Text + label186.Text, EzSensorInfo.iNPSH2x2       .ToString(), OM.EzSensorInfo.iNPSH2x2       .ToString());
                                                                                                                               
                    Trace(groupBox48.Text + label168.Text, EzSensorInfo.iMTFLeft2x2    .ToString(), OM.EzSensorInfo.iMTFLeft2x2  .ToString());
                    Trace(groupBox48.Text + label169.Text, EzSensorInfo.iMTFTop2x2     .ToString(), OM.EzSensorInfo.iMTFTop2x2   .ToString());
                    Trace(groupBox48.Text + label172.Text, EzSensorInfo.iMTFW2x2       .ToString(), OM.EzSensorInfo.iMTFW2x2     .ToString());
                    Trace(groupBox48.Text + label173.Text, EzSensorInfo.iMTFH2x2       .ToString(), OM.EzSensorInfo.iMTFH2x2     .ToString());
                                                                          
                    Trace(groupBox39.Text + label175.Text, EzSensorInfo.dDoze          .ToString(), OM.EzSensorInfo.dDoze        .ToString());
                                                                                       
                                                                                       
                    Trace(groupBox44.Text + label191.Text, EzSensorInfo.iSkRotate      .ToString(), OM.EzSensorInfo.iSkRotate    .ToString());
                    Trace(groupBox44.Text + label192.Text, EzSensorInfo.bSkFlipHorz    .ToString(), OM.EzSensorInfo.bSkFlipHorz  .ToString());
                    Trace(groupBox44.Text + label193.Text, EzSensorInfo.bSkFlipVert    .ToString(), OM.EzSensorInfo.bSkFlipVert  .ToString());
                    Trace(groupBox40.Text + label187.Text, EzSensorInfo.iSkCropTop     .ToString(), OM.EzSensorInfo.iSkCropTop   .ToString());
                    Trace(groupBox40.Text + label188.Text, EzSensorInfo.iSkCropLeft    .ToString(), OM.EzSensorInfo.iSkCropLeft  .ToString());
                    Trace(groupBox40.Text + label189.Text, EzSensorInfo.iSkCropRight   .ToString(), OM.EzSensorInfo.iSkCropRight .ToString());
                    Trace(groupBox40.Text + label190.Text, EzSensorInfo.iSkCropBtm     .ToString(), OM.EzSensorInfo.iSkCropBtm   .ToString());

                    Trace(                  label199.Text, EzSensorInfo.iGbDelay       .ToString(), OM.EzSensorInfo.iGbDelay     .ToString());
                                                                                                                                 
                    Trace(                  label218.Text, EzSensorInfo.iBfMacDelay    .ToString(), OM.EzSensorInfo.iBfMacDelay  .ToString());
                    Trace(                  label217.Text, EzSensorInfo.iAtMacDelay    .ToString(), OM.EzSensorInfo.iAtMacDelay  .ToString());
                }
                */
                
                UpdateDevInfo(true);
            }
        
        }

        static Dictionary<int, String> dict_newitems = new Dictionary<int, string>();
        public void DescrableDataBinding()
        {
            //Ver 2019.10.15.1
            //Ezsensor 마스터 ini 파일 별로 Descramble 디스플레이 달리 해줘야해서 여기에 넣음
            if (!OM.EzSensorInfo.bUseIOSPgm)
            {
                dict_newitems[0] = "None"            ;
                dict_newitems[1] = "HDS - APS"       ;
                dict_newitems[2] = "HDS - 100"       ;
                dict_newitems[3] = "RC AnySen"       ;
                dict_newitems[4] = "RC2 EzSen"       ;
                dict_newitems[5] = "EzSS10 1352X2028";
                dict_newitems[6] = "EzSS15 1620x2230";
                dict_newitems[7] = "EzSS20 1756x2432";
            }
            else
            {
                dict_newitems[0] = "None"                    ;
                dict_newitems[1] = "HDS - APS"               ;
                dict_newitems[2] = "HDS - 100"               ;
                dict_newitems[3] = "RC AnySen"               ;
                dict_newitems[4] = "RC2 EzSen"               ;
                dict_newitems[5] = "EzSS10 altered 1110X1666";
                dict_newitems[6] = "EzSS15 altered 1332X1666";
                dict_newitems[7] = "EzSS20 altered 1442X1998";
            }
            //a6.DisplayMember = "Value";
            a6.ValueMember = "Value";
            a6.DataSource = new BindingSource(dict_newitems, null);
        }
        
        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            tmUpdate.Enabled = false;
            btSaveDevice.Enabled = !SEQ._bRun;

            //UpdateLsv(true);
            //pnTrayMask.Refresh();

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

            if (cbIgnrCycleAnalyze.Checked)
            {
                Log.ShowMessage("Warning", "CycleCheck 무시 옵션을 같이 사용합니다.");
                cbIgnrCycleCheck.Checked = true;
            }
            else
            {
                if (cbIgnrCycleCheck.Checked)
                {
                    if (Log.ShowMessageModal("Confirm", "CycleCheck 무시 옵션을 사용 하시겠습니까?") != DialogResult.Yes)
                    {
                        cbIgnrCycleCheck.Checked = false;
                    }
                }
            }

            UpdateDevInfo(false);
            UpdateLsv(false);
            UpdateEzSensor(false);

            OM.SaveJobFile(OM.GetCrntDev().ToString());
            //OM.SaveDevInfo(OM.GetCrntDev().ToString());
            //OM.SaveDevOptn(OM.GetCrntDev().ToString());
            //OM.SaveEqpOptn();

            DM.ARAY[ri.LODR].SetMaxColRow(1                         , OM.DevInfo.iLODR_SlotCnt);
            DM.ARAY[ri.INDX].SetMaxColRow(OM.DevInfo.iTRAY_PcktCntX , 1                       );

            EzToDressyChange();

            Refresh();
        }

        private void btSavePosition_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            if (Log.ShowMessageModal("Confirm", "Are you Sure?") != DialogResult.Yes) return;

            PM.UpdatePstn(false);
            PM.Save(OM.GetCrntDev());
            PM.UpdatePstn(true);

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
    

        private void pbSTG_Paint(object sender, PaintEventArgs e)
        {
            int iTag = Convert.ToInt32(((PictureBox)sender).Tag);

            SolidBrush Brush = new SolidBrush(Color.Black);

            Pen Pen = new Pen(Color.Black);

            


            double dX1, dX2, dY1, dY2;

            int iTrayColCnt, iTrayRowCnt, iLdrColCnt, iLdrRowCnt;
         
            Graphics g = e.Graphics;

            switch (iTag)
            {
                default: break;
                case 1: 
                    iTrayColCnt = OM.DevInfo.iTRAY_PcktCntX;
                    iTrayRowCnt = 1;

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

                case 2:
                    iLdrColCnt = 1;
                    iLdrRowCnt = OM.DevInfo.iLODR_SlotCnt;

                    int iGetLdrWidth = pbLODR.Width;
                    int iGetLdrHeight = pbLODR.Height;

                    double iSetLdrWidth = 0, iSetLdrHeight = 0;

                    double uLdrGw = (double)iGetLdrWidth  / (double)(iLdrColCnt);
                    double uLdrGh = (double)iGetLdrHeight / (double)(iLdrRowCnt);
                    double dLdrWOff = (double)(iGetLdrWidth  - uLdrGw * (iLdrColCnt)) / 2.0;
                    double dLdrHOff = (double)(iGetLdrHeight - uLdrGh * (iLdrRowCnt)) / 2.0;


                    Pen.Color = Color.Black;

                    Brush.Color = Color.HotPink;

                    for (int r = 0; r < iLdrRowCnt; r++)
                    {
                        for (int c = 0; c < iLdrColCnt; c++)
                        {
                            dY1 = dLdrHOff + r * uLdrGh - 1;
                            dY2 = dLdrHOff + r * uLdrGh + uLdrGh;
                            dX1 = dLdrWOff + c * uLdrGw - 1;
                            dX2 = dLdrWOff + c * uLdrGw + uLdrGw;

                            g.FillRectangle(Brush, (float)dX1, (float)dY1, (float)dX2, (float)dY2);
                            g.DrawRectangle(Pen, (float)dX1, (float)dY1, (float)dX2, (float)dY2);

                            iSetLdrWidth += dY2;
                            iSetLdrHeight += dX2;
                        }
                    }

                    break;


            }
        }

        private void HideTextEditor()
        {
            TxtEdit.Visible = false;
            if (SelectedLSI != null)
            {
                SelectedLSI.Text = TxtEdit.Text;
            }
            SelectedLSI = null;
            TxtEdit.Text = "";
        }

        private void TxtEdit_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
                HideTextEditor();
        }

        private void TxtEdit_Leave(object sender, EventArgs e)
        {
            HideTextEditor();
        }

        private void Lsv1_MouseDown(object sender, MouseEventArgs e)
        {
            HideTextEditor();
        }
        private void Lsv1_MouseWheel(object sender, MouseEventArgs e)
        {
            HideTextEditor();
        }

        private void Lsv1_MouseUp(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo i = Lsv1.HitTest(e.X, e.Y);

            SelectedLSI = i.SubItem;
            if (SelectedLSI == null)
                return;

            int row = i.Item.Index;
            int col = i.Item.SubItems.IndexOf(i.SubItem);
            if (col == 0) {
                ListViewItem item = Lsv1.Items[row];
                TxtEdit.Text = item.SubItems[col].Text ;
                return;
            }

            int border = 0;
            switch (Lsv1.BorderStyle)
            {
                case BorderStyle.FixedSingle:
                    border = 1;
                    break;
                case BorderStyle.Fixed3D:
                    border = 2;
                    break;
            }

            int CellWidth = SelectedLSI.Bounds.Width;
            int CellHeight = SelectedLSI.Bounds.Height;
            int CellLeft = border + Lsv1.Left + i.SubItem.Bounds.Left;
            int CellTop = Lsv1.Top + i.SubItem.Bounds.Top;
            // First Column
            if (i.SubItem == i.Item.SubItems[0])
                CellWidth = Lsv1.Columns[0].Width;
            TxtEdit.Location = new Point(CellLeft, CellTop);
            TxtEdit.Size = new Size(CellWidth, CellHeight);
            TxtEdit.Visible = true;
            TxtEdit.BringToFront();
            TxtEdit.Text = i.SubItem.Text;
            TxtEdit.Select();
            TxtEdit.SelectAll();
        }

        private void HideTextEditor0()
        {
            T0.Visible = false;
            if (EzLSI0 != null)
            {
                EzLSI0.Text = T0.Text;
            }
            EzLSI0 = null;
            T0.Text = "";
        }
        private void HideTextEditor1()
        {
            T1.Visible = false;
            if (EzLSI1 != null)
            {
                EzLSI1.Text = T1.Text;
            }
            EzLSI1 = null;
            T1.Text = "";
        }
        private void HideTextEditor2()
        {
            T2.Visible = false;
            if (EzLSI2 != null)
            {
                EzLSI2.Text = T2.Text;
            }
            EzLSI2 = null;
            T2.Text = "";
        }
        private void HideTextEditor3()
        {
            T3.Visible = false;
            if (EzLSI3 != null)
            {
                EzLSI3.Text = T3.Text;
            }
            EzLSI3 = null;
            T3.Text = "";
        }
        private void HideTextEditor4()
        {
            T4.Visible = false;
            if (EzLSI4 != null)
            {
                EzLSI4.Text = T4.Text;
            }
            EzLSI4 = null;
            T4.Text = "";
        }
        private void HideTextEditor5()
        {
            T5.Visible = false;
            if (EzLSI5 != null)
            {
                EzLSI5.Text = T5.Text;
            }
            EzLSI5 = null;
            T5.Text = "";
        }
        private void HideTextEditor6()
        {
            T6.Visible = false;
            if (EzLSI6 != null)
            {
                EzLSI6.Text = T6.Text;
            }
            EzLSI6 = null;
            T6.Text = "";
        }
        private void HideTextEditor7()
        {
            T7.Visible = false;
            if (EzLSI7 != null)
            {
                EzLSI7.Text = T7.Text;
            }
            EzLSI7 = null;
            T7.Text = "";
        }
        
        private void Lv0_MouseDown(object sender, MouseEventArgs e)
        {
            int iTag = Convert.ToInt32(((ListView)sender).Tag);
            switch (iTag)
            {
                //공용
                case  0: HideTextEditor0 (); break;
                case  1: HideTextEditor1 (); break;
                case  2: HideTextEditor2 (); break;
                case  3: HideTextEditor3 (); break;
                case  4: HideTextEditor4 (); break;
                case  5: HideTextEditor5 (); break;
                case  6: HideTextEditor6 (); break;
                case  7: HideTextEditor7 (); break;
            }
            
        }

        private void Lv0_MouseUp(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo i = Lv0.HitTest(e.X, e.Y);

            EzLSI0 = i.SubItem;
            if (EzLSI0 == null)
                return;

            int row = i.Item.Index;
            int col = i.Item.SubItems.IndexOf(i.SubItem);

            if (col == 0)
            {
                ListViewItem item = Lv0.Items[row];
                T0.Text = item.SubItems[col].Text;
                return;
            }

            int border = 0;
            switch (Lv0.BorderStyle)
            {
                case BorderStyle.FixedSingle:
                    border = 1;
                    break;
                case BorderStyle.Fixed3D:
                    border = 2;
                    break;
            }

            int CellWidth = EzLSI0.Bounds.Width;
            int CellHeight = EzLSI0.Bounds.Height;
            int CellLeft = border + Lv0.Left + i.SubItem.Bounds.Left;
            int CellTop = Lv0.Top + i.SubItem.Bounds.Top;
            // First Column
            if (i.SubItem == i.Item.SubItems[0])
                CellWidth = Lv0.Columns[0].Width;
            T0.Location = new Point(CellLeft, CellTop);
            T0.Size = new Size(CellWidth, CellHeight);
            T0.Visible = true;
            T0.BringToFront();
            T0.Text = i.SubItem.Text;
            T0.Select();
            T0.SelectAll();
        }

        private void Lv0_MouseLeave(object sender, EventArgs e)
        {
            
        }

        private void T0_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                HideTextEditor0 ();
                HideTextEditor1 ();
                HideTextEditor2 ();
                HideTextEditor3 ();
                HideTextEditor4 ();
                HideTextEditor5 ();
                HideTextEditor6 ();
                HideTextEditor7 ();
            }
                
        }

        private void T0_Leave(object sender, EventArgs e)
        {
            HideTextEditor0();
            HideTextEditor1();
            HideTextEditor2();
            HideTextEditor3();
            HideTextEditor4();
            HideTextEditor5();
            HideTextEditor6();
            HideTextEditor7();
        }

        private void bt1_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            DialogResult Rslt = fd.ShowDialog();
            if (Rslt != DialogResult.OK) return;
            b11.Text = fd.FileName;            
        }

        private void bt2_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            DialogResult Rslt = fd.ShowDialog();
            if (Rslt != DialogResult.OK) return;
            b15.Text = fd.FileName;     
        }

        private void bt3_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            DialogResult Rslt = fd.ShowDialog();
            if (Rslt != DialogResult.OK) return;
            b19.Text = fd.FileName; 
        }

        private void bt11_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            DialogResult Rslt = fd.ShowDialog();
            if (Rslt != DialogResult.OK) return;
            a21.Text = fd.FileName; 
        }

        private void bt12_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            DialogResult Rslt = fd.ShowDialog();
            if (Rslt != DialogResult.OK) return;
            a25.Text = fd.FileName; 
        }

        private void bt4_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            DialogResult Rslt = fd.ShowDialog();
            if (Rslt != DialogResult.OK) return;
            b23.Text = fd.FileName; 
        }

        private void bt5_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            DialogResult Rslt = fd.ShowDialog();
            if (Rslt != DialogResult.OK) return;
            b24.Text = fd.SelectedPath;
        }

        private void bt6_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            DialogResult Rslt = fd.ShowDialog();
            if (Rslt != DialogResult.OK) return;
            b25.Text = fd.SelectedPath;
        }

        private void bt7_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            DialogResult Rslt = fd.ShowDialog();
            if (Rslt != DialogResult.OK) return;
            b26.Text = fd.SelectedPath;
        }

        private void bt13_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            DialogResult Rslt = fd.ShowDialog();
            if (Rslt != DialogResult.OK) return;
            a29.Text = fd.FileName;
        }

        private void bt14_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            DialogResult Rslt = fd.ShowDialog();
            if (Rslt != DialogResult.OK) return;
            a33.Text = fd.FileName;
        }

        private void bt15_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            DialogResult Rslt = fd.ShowDialog();
            if (Rslt != DialogResult.OK) return;
            a34.Text = fd.SelectedPath;
        }

        private void bt16_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            DialogResult Rslt = fd.ShowDialog();
            if (Rslt != DialogResult.OK) return;
            a35.Text = fd.SelectedPath;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            DialogResult Rslt = fd.ShowDialog();
            if (Rslt != DialogResult.OK) return;
            tbDarkImg1x1.Text = fd.SelectedPath;
        }

        public void EzToDressyChange()
        {
            if (OM.DevInfo.iMacroType == 0)
            {
                tabControl13.TabPages.Remove(tabPage23);//Chart 탭 제거
                if (tabControl13.TabPages.Contains(tabPage26))
                {
                    tabControl13.TabPages.Add(tabPage26);//Chart 탭 제거
                }
                tabControl13.SuspendLayout();
                tabControl13.TabPages.Clear();
                tabControl13.TabPages.Add(tabPage26);
                //tabControl13.TabPages.Add(_tabPage2);
                tabControl13.ResumeLayout();
                
            }
            else if (OM.DevInfo.iMacroType == 1)
            {
                tabControl13.TabPages.Remove(tabPage26);//Chart 탭 제거
                if (tabControl13.TabPages.Contains(tabPage23))
                {
                    tabControl13.TabPages.Add(tabPage23);//Chart 탭 제거
                }
                tabControl13.SuspendLayout();
                tabControl13.TabPages.Clear();
                tabControl13.TabPages.Add(tabPage23);
                //tabControl13.TabPages.Add(_tabPage2);
                tabControl13.ResumeLayout();

                //Ver 2019.10.23.1
                //a6 콤보박스 item 바인딩 후에 텍스트 변경이 안되서 추가
                //UpdateDevInfo에서 같이 하려 했더니 UpdateDevInfo 이후에 탭 페이지 추가/삭제하고있어서
                //텍스트가 안들어가서 여기에서 텍스트 변경
                a6.Text = a6.GetItemText(dict_newitems[OM.EzSensorInfo.iDescramble]);
            }
        }

        private void Lv1_MouseUp(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo i = Lv1.HitTest(e.X, e.Y);

            EzLSI1 = i.SubItem;
            if (EzLSI1 == null)
                return;

            int row = i.Item.Index;
            int col = i.Item.SubItems.IndexOf(i.SubItem);

            if (col == 0)
            {
                ListViewItem item = Lv1.Items[row];
                T1.Text = item.SubItems[col].Text;
                return;
            }

            int border = 0;
            switch (Lv1.BorderStyle)
            {
                case BorderStyle.FixedSingle:
                    border = 1;
                    break;
                case BorderStyle.Fixed3D:
                    border = 2;
                    break;
            }

            int CellWidth = EzLSI1.Bounds.Width;
            int CellHeight = EzLSI1.Bounds.Height;
            int CellLeft = border + Lv1.Left + i.SubItem.Bounds.Left;
            int CellTop = Lv1.Top + i.SubItem.Bounds.Top;
            // First Column
            if (i.SubItem == i.Item.SubItems[0])
                CellWidth = Lv1.Columns[0].Width;
            T1.Location = new Point(CellLeft, CellTop);
            T1.Size = new Size(CellWidth, CellHeight);
            T1.Visible = true;
            T1.BringToFront();
            T1.Text = i.SubItem.Text;
            T1.Select();
            T1.SelectAll();
        }

        private void Lv2_MouseUp(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo i = Lv2.HitTest(e.X, e.Y);

            EzLSI2 = i.SubItem;
            if (EzLSI2 == null)
                return;

            int row = i.Item.Index;
            int col = i.Item.SubItems.IndexOf(i.SubItem);

            if (col == 0)
            {
                ListViewItem item = Lv2.Items[row];
                T2.Text = item.SubItems[col].Text;
                return;
            }

            int border = 0;
            switch (Lv2.BorderStyle)
            {
                case BorderStyle.FixedSingle:
                    border = 1;
                    break;
                case BorderStyle.Fixed3D:
                    border = 2;
                    break;
            }

            int CellWidth = EzLSI2.Bounds.Width;
            int CellHeight = EzLSI2.Bounds.Height;
            int CellLeft = border + Lv2.Left + i.SubItem.Bounds.Left;
            int CellTop = Lv2.Top + i.SubItem.Bounds.Top;
            // First Column
            if (i.SubItem == i.Item.SubItems[0])
                CellWidth = Lv2.Columns[0].Width;
            T2.Location = new Point(CellLeft, CellTop);
            T2.Size = new Size(CellWidth, CellHeight);
            T2.Visible = true;
            T2.BringToFront();
            T2.Text = i.SubItem.Text;
            T2.Select();
            T2.SelectAll();
        }

        private void Lv3_MouseUp(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo i = Lv3.HitTest(e.X, e.Y);

            EzLSI3 = i.SubItem;
            if (EzLSI3 == null)
                return;

            int row = i.Item.Index;
            int col = i.Item.SubItems.IndexOf(i.SubItem);

            if (col == 0)
            {
                ListViewItem item = Lv3.Items[row];
                T3.Text = item.SubItems[col].Text;
                return;
            }

            int border = 0;
            switch (Lv3.BorderStyle)
            {
                case BorderStyle.FixedSingle:
                    border = 1;
                    break;
                case BorderStyle.Fixed3D:
                    border = 2;
                    break;
            }

            int CellWidth = EzLSI3.Bounds.Width;
            int CellHeight = EzLSI3.Bounds.Height;
            int CellLeft = border + Lv3.Left + i.SubItem.Bounds.Left;
            int CellTop = Lv3.Top + i.SubItem.Bounds.Top;
            // First Column
            if (i.SubItem == i.Item.SubItems[0])
                CellWidth = Lv3.Columns[0].Width;
            T3.Location = new Point(CellLeft, CellTop);
            T3.Size = new Size(CellWidth, CellHeight);
            T3.Visible = true;
            T3.BringToFront();
            T3.Text = i.SubItem.Text;
            T3.Select();
            T3.SelectAll();
        }

        private void Lv4_MouseUp(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo i = Lv4.HitTest(e.X, e.Y);

            EzLSI4 = i.SubItem;
            if (EzLSI4 == null)
                return;

            int row = i.Item.Index;
            int col = i.Item.SubItems.IndexOf(i.SubItem);

            if (col == 0)
            {
                ListViewItem item = Lv4.Items[row];
                T4.Text = item.SubItems[col].Text;
                return;
            }

            int border = 0;
            switch (Lv4.BorderStyle)
            {
                case BorderStyle.FixedSingle:
                    border = 1;
                    break;
                case BorderStyle.Fixed3D:
                    border = 2;
                    break;
            }

            int CellWidth = EzLSI4.Bounds.Width;
            int CellHeight = EzLSI4.Bounds.Height;
            int CellLeft = border + Lv4.Left + i.SubItem.Bounds.Left;
            int CellTop = Lv4.Top + i.SubItem.Bounds.Top;
            // First Column
            if (i.SubItem == i.Item.SubItems[0])
                CellWidth = Lv4.Columns[0].Width;
            T4.Location = new Point(CellLeft, CellTop);
            T4.Size = new Size(CellWidth, CellHeight);
            T4.Visible = true;
            T4.BringToFront();
            T4.Text = i.SubItem.Text;
            T4.Select();
            T4.SelectAll();
        }

        private void Lv5_MouseUp(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo i = Lv5.HitTest(e.X, e.Y);

            EzLSI5 = i.SubItem;
            if (EzLSI5 == null)
                return;

            int row = i.Item.Index;
            int col = i.Item.SubItems.IndexOf(i.SubItem);

            if (col == 0)
            {
                ListViewItem item = Lv5.Items[row];
                T5.Text = item.SubItems[col].Text;
                return;
            }

            int border = 0;
            switch (Lv5.BorderStyle)
            {
                case BorderStyle.FixedSingle:
                    border = 1;
                    break;
                case BorderStyle.Fixed3D:
                    border = 2;
                    break;
            }

            int CellWidth = EzLSI5.Bounds.Width;
            int CellHeight = EzLSI5.Bounds.Height;
            int CellLeft = border + Lv5.Left + i.SubItem.Bounds.Left;
            int CellTop = Lv5.Top + i.SubItem.Bounds.Top;
            // First Column
            if (i.SubItem == i.Item.SubItems[0])
                CellWidth = Lv5.Columns[0].Width;
            T5.Location = new Point(CellLeft, CellTop);
            T5.Size = new Size(CellWidth, CellHeight);
            T5.Visible = true;
            T5.BringToFront();
            T5.Text = i.SubItem.Text;
            T5.Select();
            T5.SelectAll();
        }

        private void Lv6_MouseUp(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo i = Lv6.HitTest(e.X, e.Y);

            EzLSI6 = i.SubItem;
            if (EzLSI6 == null)
                return;

            int row = i.Item.Index;
            int col = i.Item.SubItems.IndexOf(i.SubItem);

            if (col == 0)
            {
                ListViewItem item = Lv6.Items[row];
                T6.Text = item.SubItems[col].Text;
                return;
            }

            int border = 0;
            switch (Lv6.BorderStyle)
            {
                case BorderStyle.FixedSingle:
                    border = 1;
                    break;
                case BorderStyle.Fixed3D:
                    border = 2;
                    break;
            }

            int CellWidth = EzLSI6.Bounds.Width;
            int CellHeight = EzLSI6.Bounds.Height;
            int CellLeft = border + Lv6.Left + i.SubItem.Bounds.Left;
            int CellTop = Lv6.Top + i.SubItem.Bounds.Top;
            // First Column
            if (i.SubItem == i.Item.SubItems[0])
                CellWidth = Lv6.Columns[0].Width;
            T6.Location = new Point(CellLeft, CellTop);
            T6.Size = new Size(CellWidth, CellHeight);
            T6.Visible = true;
            T6.BringToFront();
            T6.Text = i.SubItem.Text;
            T6.Select();
            T6.SelectAll();
        }

        private void Lv7_MouseUp(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo i = Lv7.HitTest(e.X, e.Y);

            EzLSI7 = i.SubItem;
            if (EzLSI7 == null)
                return;

            int row = i.Item.Index;
            int col = i.Item.SubItems.IndexOf(i.SubItem);

            if (col == 0)
            {
                ListViewItem item = Lv7.Items[row];
                T7.Text = item.SubItems[col].Text;
                return;
            }

            int border = 0;
            switch (Lv7.BorderStyle)
            {
                case BorderStyle.FixedSingle:
                    border = 1;
                    break;
                case BorderStyle.Fixed3D:
                    border = 2;
                    break;
            }

            int CellWidth = EzLSI7.Bounds.Width;
            int CellHeight = EzLSI7.Bounds.Height;
            int CellLeft = border + Lv7.Left + i.SubItem.Bounds.Left;
            int CellTop = Lv7.Top + i.SubItem.Bounds.Top;
            // First Column
            if (i.SubItem == i.Item.SubItems[0])
                CellWidth = Lv7.Columns[0].Width;
            T7.Location = new Point(CellLeft, CellTop);
            T7.Size = new Size(CellWidth, CellHeight);
            T7.Visible = true;
            T7.BringToFront();
            T7.Text = i.SubItem.Text;
            T7.Select();
            T7.SelectAll();
        }

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
