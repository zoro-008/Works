using System;
using System.Drawing;
using System.Windows.Forms;
using COMMON;
using SML;
using System.Reflection;
using System.Text;
using System.Collections.Generic;
using System.IO;

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

            //var PropLotInfo = pnM4.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            //PropLotInfo.SetValue(pnM4, true, null);
            //cb1.Focus = false;

            //tbUserUnit.Text = 0.01.ToString();
            PstnDisp();

            OM.LoadLastInfo();
            PM.Load(OM.GetCrntDev().ToString());

            PM.UpdatePstn(true);
            UpdateDevInfo(true);
            
            //DM.ARAY[ri.MASK].SetParent(pnTrayMask); DM.ARAY[ri.MASK].Name = "MASK";
            //LoadMask(OM.GetCrntDev().ToString());            
            //DM.ARAY[ri.MASK].SetDisp(cs.Empty,"Empty",Color.Silver);
            //DM.ARAY[ri.MASK].SetDisp(cs.None ,"None" ,Color.White );            
           
            //FraMotr     = new FraMotr    [(int)mi.MAX_MOTR];
            //FraCylinder = new FraCylOneBt[(int)ci.MAX_ACTR  ];

            //모터 축 수에 맞춰 FrameMotr 생성
            //for (int m = 0; m < (int)mi.MAX_MOTR; m++)
            //{
            //    Control[] Ctrl = tcDeviceSet.Controls.Find("pnMotrJog" +  m.ToString(), true);
            //
            //    MOTION_DIR eDir = ML.MT_GetDirType(m);
            //    FraMotr[m] = new FraMotr();
            //    FraMotr[m].SetIdType((mi)m, eDir);
            //    FraMotr[m].TopLevel = false;
            //    FraMotr[m].Parent = Ctrl[0];
            //    FraMotr[m].Show();
            //    FraMotr[m].SetUnit(EN_UNIT_TYPE.utJog, 0);
            //}

            //여기 AP텍에서만 쓰는거
            //실린더 버튼 AP텍꺼
            //const int iCylinderCnt = (int)ci.MAX_ACTR;//30;
            //FraCylAPT = new FrameCylinderAPT[iCylinderCnt];
            //for (int i = 0; i < iCylinderCnt; i++)
            //{
            //    Control[] CtrlAP = tcDeviceSet.Controls.Find("C" + i.ToString(), true);
            //
            //    //int iCylCtrl = Convert.ToInt32(CtrlAP[0].Tag);
            //    FraCylAPT[i] = new FrameCylinderAPT();
            //    FraCylAPT[i].TopLevel = false;
            //    FraCylAPT[i].SetConfig((ci)iCylCtrl, SM.CYL.GetName(iCylCtrl).ToString(), ML.CL_GetDirType((ci)iCylCtrl), CtrlAP[0]);
            //    FraCylAPT[i].Show();
            //}

            //Input Status 생성 AP텍꺼
            //const int iInputBtnCnt  = 12;
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
            //
            ////Output Status 생성 AP텍꺼
            //const int iOutputBtnCnt = 1;
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

            //모터 포지션 AP텍꺼
            //FraMotrPosAPT = new FrameMotrPosAPT[(int)mi.MAX_MOTR];
            //for (int i = 0; i < (int)mi.MAX_MOTR; i++)
            //{
            //    Control[] Ctrl = tcDeviceSet.Controls.Find("pnMotrPos" + i.ToString(), true);
            //
            //    FraMotrPosAPT[i] = new FrameMotrPosAPT();
            //    FraMotrPosAPT[i].TopLevel = false;
            //    FraMotrPosAPT[i].SetWindow(i, Ctrl[0]);
            //    FraMotrPosAPT[i].Show();
            //}
        }

        public void PstnDisp()
        {
            //LODR_XPshr                  
            //PM.SetProp((uint)mi.LODR_XPshr, (uint)pv.LODR_XPshrWait        , "Wait           ", false , false, false );
            //PM.SetProp((uint)mi.LODR_XPshr, (uint)pv.LODR_XPshrWorkStt     , "WorkStt        ", false , false, false );
            //PM.SetProp((uint)mi.LODR_XPshr, (uint)pv.LODR_XPshrWorkEnd     , "WorkEnd        ", false , false, false );
            //PM.SetProp((uint)mi.LODR_XPshr, (uint)pv.LODR_XPshrBackOfs     , "BackOfs        ", true  , false, false );
            //
            //PM.SetProp((uint)mi.LODR_YIndx, (uint)pv.LODR_YIndxWait        , "BackOfs        ", true  , false, false );
            //PM.SetProp((uint)mi.LODR_YIndx, (uint)pv.LODR_YIndxWork        , "BackOfs        ", true  , false, false );

            
        }
             
        private void tcDeviceSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            //int iSeletedIndex;
            //iSeletedIndex = tcDeviceSet.SelectedIndex;
            
            //switch (iSeletedIndex)
            //{
            //    default : break;
            //    case 0  : gbJogUnit.Parent = pnJog1;                       break;
            //    case 1  : gbJogUnit.Parent = pnJog2;                       break;
            //    case 2  : gbJogUnit.Parent = pnJog3;                       break;
            //    case 3  : gbJogUnit.Parent = pnJog4;                       break;
            //    case 4  : gbJogUnit.Parent = pnJog5;                       break;
            //    case 5  : gbJogUnit.Parent = pnJog6;                       break;
            //}

            //PM.UpdatePstn(true);
            ////PM.Load(OM.GetCrntDev());
        }

        public void UpdateDevInfo(bool bToTable)
        {
            if (bToTable)
            {
                CConfig.ValToCon(cb1 , ref OM.DevInfo.iNo1 ); tb1 .Text = GetName(OM.DevInfo.iNo1 ); 
                CConfig.ValToCon(cb2 , ref OM.DevInfo.iNo2 ); tb2 .Text = GetName(OM.DevInfo.iNo2 );
                CConfig.ValToCon(cb3 , ref OM.DevInfo.iNo3 ); tb3 .Text = GetName(OM.DevInfo.iNo3 );
                CConfig.ValToCon(cb4 , ref OM.DevInfo.iNo4 ); tb4 .Text = GetName(OM.DevInfo.iNo4 );
                CConfig.ValToCon(cb5 , ref OM.DevInfo.iNo5 ); tb5 .Text = GetName(OM.DevInfo.iNo5 );
                CConfig.ValToCon(cb6 , ref OM.DevInfo.iNo6 ); tb6 .Text = GetName(OM.DevInfo.iNo6 );
                CConfig.ValToCon(cb7 , ref OM.DevInfo.iNo7 ); tb7 .Text = GetName(OM.DevInfo.iNo7 );
                CConfig.ValToCon(cb8 , ref OM.DevInfo.iNo8 ); tb8 .Text = GetName(OM.DevInfo.iNo8 );
                CConfig.ValToCon(cb9 , ref OM.DevInfo.iNo9 ); tb9 .Text = GetName(OM.DevInfo.iNo9 );
                CConfig.ValToCon(cb10, ref OM.DevInfo.iNo10); tb10.Text = GetName(OM.DevInfo.iNo10);
                CConfig.ValToCon(cb11, ref OM.DevInfo.iNo11); tb11.Text = GetName(OM.DevInfo.iNo11);
                CConfig.ValToCon(cb12, ref OM.DevInfo.iNo12); tb12.Text = GetName(OM.DevInfo.iNo12);
                CConfig.ValToCon(cb13, ref OM.DevInfo.iNo13); tb13.Text = GetName(OM.DevInfo.iNo13);
                CConfig.ValToCon(cb14, ref OM.DevInfo.iNo14); tb14.Text = GetName(OM.DevInfo.iNo14);
                CConfig.ValToCon(cb15, ref OM.DevInfo.iNo15); tb15.Text = GetName(OM.DevInfo.iNo15);
                CConfig.ValToCon(cb16, ref OM.DevInfo.iNo16); tb16.Text = GetName(OM.DevInfo.iNo16);
                CConfig.ValToCon(cb17, ref OM.DevInfo.iNo17); tb17.Text = GetName(OM.DevInfo.iNo17);
                CConfig.ValToCon(cb18, ref OM.DevInfo.iNo18); tb18.Text = GetName(OM.DevInfo.iNo18);
                CConfig.ValToCon(cb19, ref OM.DevInfo.iNo19); tb19.Text = GetName(OM.DevInfo.iNo19);
                CConfig.ValToCon(cb20, ref OM.DevInfo.iNo20); tb20.Text = GetName(OM.DevInfo.iNo20);
                CConfig.ValToCon(cb21, ref OM.DevInfo.iNo21); tb21.Text = GetName(OM.DevInfo.iNo21);
                CConfig.ValToCon(cb22, ref OM.DevInfo.iNo22); tb22.Text = GetName(OM.DevInfo.iNo22);
                CConfig.ValToCon(cb23, ref OM.DevInfo.iNo23); tb23.Text = GetName(OM.DevInfo.iNo23);
                CConfig.ValToCon(cb24, ref OM.DevInfo.iNo24); tb24.Text = GetName(OM.DevInfo.iNo24);
                
            }
            else 
            {
                OM.CDevInfo DevInfo = OM.DevInfo;

                CConfig.ConToVal(cb1 , ref OM.DevInfo.iNo1 ); CConfig.ValToCon(cb11, ref OM.DevInfo.iNo11);
                CConfig.ConToVal(cb2 , ref OM.DevInfo.iNo2 ); CConfig.ValToCon(cb12, ref OM.DevInfo.iNo12);
                CConfig.ConToVal(cb3 , ref OM.DevInfo.iNo3 ); CConfig.ValToCon(cb13, ref OM.DevInfo.iNo13);
                CConfig.ConToVal(cb4 , ref OM.DevInfo.iNo4 ); CConfig.ValToCon(cb14, ref OM.DevInfo.iNo14);
                CConfig.ConToVal(cb5 , ref OM.DevInfo.iNo5 ); CConfig.ValToCon(cb15, ref OM.DevInfo.iNo15);
                CConfig.ConToVal(cb6 , ref OM.DevInfo.iNo6 ); CConfig.ValToCon(cb16, ref OM.DevInfo.iNo16);
                CConfig.ConToVal(cb7 , ref OM.DevInfo.iNo7 ); CConfig.ValToCon(cb17, ref OM.DevInfo.iNo17);
                CConfig.ConToVal(cb8 , ref OM.DevInfo.iNo8 ); CConfig.ValToCon(cb18, ref OM.DevInfo.iNo18);
                CConfig.ConToVal(cb9 , ref OM.DevInfo.iNo9 ); CConfig.ValToCon(cb19, ref OM.DevInfo.iNo19);
                CConfig.ConToVal(cb10, ref OM.DevInfo.iNo10); CConfig.ValToCon(cb20, ref OM.DevInfo.iNo20);

                CConfig.ConToVal(cb21, ref OM.DevInfo.iNo21);
                CConfig.ConToVal(cb22, ref OM.DevInfo.iNo22);
                CConfig.ConToVal(cb23, ref OM.DevInfo.iNo23);
                CConfig.ConToVal(cb24, ref OM.DevInfo.iNo24);

                //Auto Log
                Type type = DevInfo.GetType();
                FieldInfo[] f = type.GetFields(BindingFlags.Public|BindingFlags.NonPublic | BindingFlags.Instance);
                for(int i = 0 ; i < f.Length ; i++){
                    Trace(f[i].Name, f[i].GetValue(DevInfo).ToString(), f[i].GetValue(OM.DevInfo).ToString());
                }

                UpdateDevInfo(true);
            }
        
        }




        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            tmUpdate.Enabled = false;
            btSaveDevice.Enabled = !SEQ._bRun;

            //if(rbPaintNo1.Text != OM.CmnOptn.sPaintName1){ rbPaintNo1.Text = OM.CmnOptn.sPaintName1; }
            //if(rbPaintNo2.Text != OM.CmnOptn.sPaintName2){ rbPaintNo2.Text = OM.CmnOptn.sPaintName2; }
            //if(rbPaintNo3.Text != OM.CmnOptn.sPaintName3){ rbPaintNo3.Text = OM.CmnOptn.sPaintName3; }
            //if(rbPaintNo4.Text != OM.CmnOptn.sPaintName4){ rbPaintNo4.Text = OM.CmnOptn.sPaintName4; }
            //if(rbPaintNo5.Text != OM.CmnOptn.sPaintName5){ rbPaintNo5.Text = OM.CmnOptn.sPaintName5; }


            //pnTrayMask.Refresh();

            if (!this.Visible)
            {
                tmUpdate.Enabled = false;
                return;
            }
            tmUpdate.Enabled = true;
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

            UpdateDevInfo(false);

            OM.SaveDevInfo(OM.GetCrntDev().ToString());
            OM.SaveEqpOptn();

            RecipeSetting();
            //SaveMask(OM.GetCrntDev());

            //DM.ARAY[ri.LODR].SetMaxColRow(1, OM.DevInfo.iLODR_BarCnt);

            //MM.SetManCycle(mc.MARK_CycleManChage);
            //Refresh();
            Invalidate(true);
        }

        private void btSavePosition_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            if (Log.ShowMessageModal("Confirm", "Are you Sure?") != DialogResult.Yes) return;

            PM.UpdatePstn(false);
            PM.Save(OM.GetCrntDev());
            PM.UpdatePstn(true);

            //Refresh();
            Invalidate(true);
        }

        private void FormDeviceSet_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible) {
                FormOperation.FrmGrid.Parent = pnRecipe;
                tmUpdate.Enabled = true;
            }
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

        private void label5_Click(object sender, EventArgs e)
        {

        }

        static public void RecipeSetting()
        {
          
            List<string> lst = new List<string>();

            for(int i=1; i<25; i++) //아 너무 고정이네 
            {
                
                if(i == 1 ) { if(OM.DevInfo.iNo1  != 0 ) Load(lst,OM.DevInfo.iNo1 ); }
                if(i == 2 ) { if(OM.DevInfo.iNo2  != 0 ) Load(lst,OM.DevInfo.iNo2 ); }
                if(i == 3 ) { if(OM.DevInfo.iNo3  != 0 ) Load(lst,OM.DevInfo.iNo3 ); }
                if(i == 4 ) { if(OM.DevInfo.iNo4  != 0 ) Load(lst,OM.DevInfo.iNo4 ); }
                if(i == 5 ) { if(OM.DevInfo.iNo5  != 0 ) Load(lst,OM.DevInfo.iNo5 ); }
                if(i == 6 ) { if(OM.DevInfo.iNo6  != 0 ) Load(lst,OM.DevInfo.iNo6 ); }
                if(i == 7 ) { if(OM.DevInfo.iNo7  != 0 ) Load(lst,OM.DevInfo.iNo7 ); }
                if(i == 8 ) { if(OM.DevInfo.iNo8  != 0 ) Load(lst,OM.DevInfo.iNo8 ); }
                if(i == 9 ) { if(OM.DevInfo.iNo9  != 0 ) Load(lst,OM.DevInfo.iNo9 ); }
                if(i == 10) { if(OM.DevInfo.iNo10 != 0 ) Load(lst,OM.DevInfo.iNo10); }
                if(i == 11) { if(OM.DevInfo.iNo11 != 0 ) Load(lst,OM.DevInfo.iNo11); }
                if(i == 12) { if(OM.DevInfo.iNo12 != 0 ) Load(lst,OM.DevInfo.iNo12); }
                if(i == 13) { if(OM.DevInfo.iNo13 != 0 ) Load(lst,OM.DevInfo.iNo13); }
                if(i == 14) { if(OM.DevInfo.iNo14 != 0 ) Load(lst,OM.DevInfo.iNo14); }
                if(i == 15) { if(OM.DevInfo.iNo15 != 0 ) Load(lst,OM.DevInfo.iNo15); }
                if(i == 16) { if(OM.DevInfo.iNo16 != 0 ) Load(lst,OM.DevInfo.iNo16); }
                if(i == 17) { if(OM.DevInfo.iNo17 != 0 ) Load(lst,OM.DevInfo.iNo17); }
                if(i == 18) { if(OM.DevInfo.iNo18 != 0 ) Load(lst,OM.DevInfo.iNo18); }
                if(i == 19) { if(OM.DevInfo.iNo19 != 0 ) Load(lst,OM.DevInfo.iNo19); }
                if(i == 20) { if(OM.DevInfo.iNo20 != 0 ) Load(lst,OM.DevInfo.iNo20); }
                if(i == 21) { if(OM.DevInfo.iNo21 != 0 ) Load(lst,OM.DevInfo.iNo21); }
                if(i == 22) { if(OM.DevInfo.iNo22 != 0 ) Load(lst,OM.DevInfo.iNo22); }
                if(i == 23) { if(OM.DevInfo.iNo23 != 0 ) Load(lst,OM.DevInfo.iNo23); }
                if(i == 24) { if(OM.DevInfo.iNo24 != 0 ) Load(lst,OM.DevInfo.iNo24); }
            }

            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevInfoPath = sExeFolder + "JobFile\\" + OM.GetCrntDev() + "\\Recipe.csv";
            string sCopyPath    = sExeFolder + "JobFile\\" + OM.GetCrntDev() + "\\RecipeRead.csv";
            Save(lst,sDevInfoPath);

            if(File.Exists(sCopyPath   )) File.Delete(sCopyPath);
            if(File.Exists(sDevInfoPath)) File.Copy(sDevInfoPath,sCopyPath);

            //데이터 그리드뷰 불러와서 셋팅하기
            OM.SeasoningOptnView.Load(sDevInfoPath);
            FormOperation.FrmGrid.SetList();
        }

        static public bool Load(List<string> _lst, int _iNo)
        {
            string fileName ;
            fileName = System.AppDomain.CurrentDomain.BaseDirectory + "Util\\" + _iNo.ToString() +".csv";

            if(!File.Exists(fileName)) return false;

            string [] sDatas ;
            try
            {
                sDatas= System.IO.File.ReadAllLines(fileName, Encoding.Default);
            }
            catch(Exception _e)
            {
                //MessageBox.Show(_e.Message);
                return false ;
            }

            //해더만 있어도 리턴.
            if(sDatas.Length < 2) return false ;

            if(_lst.Count == 0) _lst.Add(sDatas[0]);
            for(int i=1; i<sDatas.Length; i++)
            {
                _lst.Add(sDatas[i]);
            }

            return true ;
        }


        static public bool Save(List<string> _lst, string fileName)
        {
            try
            {
                if (_lst.Count <= 0) return false;
                using (System.IO.FileStream csvFileWriter = new FileStream(fileName,FileMode.OpenOrCreate))
                {
                    string RowData = "";
                    for (int r = 0; r < _lst.Count; r++)
                    {
                        RowData =  _lst[r] ;
                        Byte[] Bytes = Encoding.GetEncoding("ks_c_5601-1987").GetBytes(RowData+"\n");
                        csvFileWriter.Write(Bytes, 0, Bytes.Length);
                    }
                    csvFileWriter.Close();
                }


            }
            catch (Exception exceptionObject)
            {
                MessageBox.Show(exceptionObject.ToString());
                return false;
            }
            return true;

        }

        private void cb1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //int  iNo = ((ComboBox)sender).SelectedIndex;
            ////그래 이름으로 접근해서 하면 되는데 안함요 개귀찮
            //     if(iNo == 1 ) tb1 .Text = GetName(iNo);
            //else if(iNo == 2 ) tb2 .Text = GetName(iNo);
            //else if(iNo == 3 ) tb3 .Text = GetName(iNo);
            //else if(iNo == 4 ) tb4 .Text = GetName(iNo);
            //else if(iNo == 5 ) tb5 .Text = GetName(iNo);
            //else if(iNo == 6 ) tb6 .Text = GetName(iNo);
            //else if(iNo == 7 ) tb7 .Text = GetName(iNo);
            //else if(iNo == 8 ) tb8 .Text = GetName(iNo);
            //else if(iNo == 9 ) tb9 .Text = GetName(iNo);
            //else if(iNo == 10) tb10.Text = GetName(iNo);
            //else if(iNo == 11) tb11.Text = GetName(iNo);
            //else if(iNo == 12) tb12.Text = GetName(iNo);
            //else if(iNo == 13) tb13.Text = GetName(iNo);
            //else if(iNo == 14) tb14.Text = GetName(iNo);
            //else if(iNo == 15) tb15.Text = GetName(iNo);
            //else if(iNo == 16) tb16.Text = GetName(iNo);
            //else if(iNo == 17) tb17.Text = GetName(iNo);
            //else if(iNo == 18) tb18.Text = GetName(iNo);
            //else if(iNo == 19) tb19.Text = GetName(iNo);
            //else if(iNo == 20) tb20.Text = GetName(iNo);
            //else if(iNo == 21) tb21.Text = GetName(iNo);
            //else if(iNo == 22) tb22.Text = GetName(iNo);
            //else if(iNo == 23) tb23.Text = GetName(iNo);
            //else if(iNo == 24) tb24.Text = GetName(iNo);

        }


        private string GetName(int iNo)
        {
            string sName = "";
            if(iNo == 1  ) sName = OM.CmnOptn.sName1 ;
            if(iNo == 2  ) sName = OM.CmnOptn.sName2 ;
            if(iNo == 3  ) sName = OM.CmnOptn.sName3 ;
            if(iNo == 4  ) sName = OM.CmnOptn.sName4 ;
            if(iNo == 5  ) sName = OM.CmnOptn.sName5 ;
            if(iNo == 6  ) sName = OM.CmnOptn.sName6 ;
            if(iNo == 7  ) sName = OM.CmnOptn.sName7 ;
            if(iNo == 8  ) sName = OM.CmnOptn.sName8 ;
            if(iNo == 9  ) sName = OM.CmnOptn.sName9 ;
            if(iNo == 10 ) sName = OM.CmnOptn.sName10;
            if(iNo == 11 ) sName = OM.CmnOptn.sName11;
            if(iNo == 12 ) sName = OM.CmnOptn.sName12;
            if(iNo == 13 ) sName = OM.CmnOptn.sName13;
            if(iNo == 14 ) sName = OM.CmnOptn.sName14;
            if(iNo == 15 ) sName = OM.CmnOptn.sName15;
            if(iNo == 16 ) sName = OM.CmnOptn.sName16;
            if(iNo == 17 ) sName = OM.CmnOptn.sName17;
            if(iNo == 18 ) sName = OM.CmnOptn.sName18;
            if(iNo == 19 ) sName = OM.CmnOptn.sName19;
            if(iNo == 20 ) sName = OM.CmnOptn.sName20;
            
            return sName;
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
