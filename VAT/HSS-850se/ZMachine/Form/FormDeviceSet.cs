using System;
using System.Drawing;
using System.Windows.Forms;
using COMMON;
using SML;
using System.Reflection;
using System.Text;
using System.Collections.Generic;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;

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
            //this.Width = 1272;
            //this.Height = 866;

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
            //UpdateDevInfo(true);
            
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
                //CConfig.ValToCon(tb1 , ref OM.DevInfo.sNo1 );  
                //CConfig.ValToCon(tb2 , ref OM.DevInfo.sNo2 ); 
                //CConfig.ValToCon(tb3 , ref OM.DevInfo.sNo3 ); 
                //CConfig.ValToCon(tb4 , ref OM.DevInfo.sNo4 ); 
                //CConfig.ValToCon(tb5 , ref OM.DevInfo.sNo5 ); 
                //CConfig.ValToCon(tb6 , ref OM.DevInfo.sNo6 ); 
                //CConfig.ValToCon(tb7 , ref OM.DevInfo.sNo7 ); 
                //CConfig.ValToCon(tb8 , ref OM.DevInfo.sNo8 ); 
                //CConfig.ValToCon(tb9 , ref OM.DevInfo.sNo9 ); 
                //CConfig.ValToCon(tb10, ref OM.DevInfo.sNo10); 
                //CConfig.ValToCon(tb11, ref OM.DevInfo.sNo11); 
                //CConfig.ValToCon(tb12, ref OM.DevInfo.sNo12); 
                //CConfig.ValToCon(tb13, ref OM.DevInfo.sNo13); 
                //CConfig.ValToCon(tb14, ref OM.DevInfo.sNo14); 
                //CConfig.ValToCon(tb15, ref OM.DevInfo.sNo15); 
                //CConfig.ValToCon(tb16, ref OM.DevInfo.sNo16); 
                //CConfig.ValToCon(tb17, ref OM.DevInfo.sNo17); 
                //CConfig.ValToCon(tb18, ref OM.DevInfo.sNo18); 
                //CConfig.ValToCon(tb19, ref OM.DevInfo.sNo19); 
                //CConfig.ValToCon(tb20, ref OM.DevInfo.sNo20); 
                
            }
            else 
            {
                OM.CDevInfo DevInfo = OM.DevInfo;

                //CConfig.ConToVal(tb1 , ref OM.DevInfo.sNo1 );  
                //CConfig.ConToVal(tb2 , ref OM.DevInfo.sNo2 ); 
                //CConfig.ConToVal(tb3 , ref OM.DevInfo.sNo3 ); 
                //CConfig.ConToVal(tb4 , ref OM.DevInfo.sNo4 ); 
                //CConfig.ConToVal(tb5 , ref OM.DevInfo.sNo5 ); 
                //CConfig.ConToVal(tb6 , ref OM.DevInfo.sNo6 ); 
                //CConfig.ConToVal(tb7 , ref OM.DevInfo.sNo7 ); 
                //CConfig.ConToVal(tb8 , ref OM.DevInfo.sNo8 ); 
                //CConfig.ConToVal(tb9 , ref OM.DevInfo.sNo9 ); 
                //CConfig.ConToVal(tb10, ref OM.DevInfo.sNo10); 
                //CConfig.ConToVal(tb11, ref OM.DevInfo.sNo11); 
                //CConfig.ConToVal(tb12, ref OM.DevInfo.sNo12); 
                //CConfig.ConToVal(tb13, ref OM.DevInfo.sNo13); 
                //CConfig.ConToVal(tb14, ref OM.DevInfo.sNo14); 
                //CConfig.ConToVal(tb15, ref OM.DevInfo.sNo15); 
                //CConfig.ConToVal(tb16, ref OM.DevInfo.sNo16); 
                //CConfig.ConToVal(tb17, ref OM.DevInfo.sNo17); 
                //CConfig.ConToVal(tb18, ref OM.DevInfo.sNo18); 
                //CConfig.ConToVal(tb19, ref OM.DevInfo.sNo19); 
                //CConfig.ConToVal(tb20, ref OM.DevInfo.sNo20); 

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
            
            btSaveDevice.Enabled = !SEQ._bRun && !LOT.LotOpened;
            btSaveAsDevice.Enabled = !SEQ._bRun && !LOT.LotOpened;
            lbRecipe.Text  =  OM.GetCrntDev();
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

            if (LOT.GetLotOpen())
            {
                Log.ShowMessage("Error", "Please check the status of the Lot(Need to Lot End).");
                return;
            }

            if (Log.ShowMessageModal("Confirm", "Are you Sure?") != DialogResult.Yes) return;

            //UpdateDevInfo(false);

            //OM.SaveDevInfo(OM.GetCrntDev().ToString());
            //OM.SaveEqpOptn();

            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevInfoPath = sExeFolder + "Recipe\\" + OM.GetCrntDev() + "\\RecipeSetting.csv";
            File.Delete(sDevInfoPath);
            dataGridView1.Save(sDevInfoPath);

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
                FormOperation.FrmGrid.Dock   = DockStyle.Fill;
                FormOperation.FrmGrid.Show();
                //FormOperation.FrmGrid.TopLevel = false   ;
                //FormOperation.FrmGrid.Parent   = pnRecipe;
                //
                string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
                string sDevInfoPath = sExeFolder + "Recipe\\" + OM.GetCrntDev() + "\\RecipeSetting.csv";

                dataGridView1.Load(sDevInfoPath);
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
        public bool bComplete = false;
        public bool bOk       = false;
        public bool RecipeCheck()
        {
            List<string> lst = new List<string>();
            bOk = true;

            //if (dataGridView1.Parent != null)
            //{
            //    //UI에 접근 하기 위한 인보크
            //    if (dataGridView1.InvokeRequired)
            //    {
            //        dataGridView1.Invoke(new MethodInvoker(delegate ()
            //        {
            //            for(int i=0; i<dataGridView1.RowCount-1; i++)
            //            {
            //                if(!Load(lst,dataGridView1.GetString("PATH",i) ,i+1 )) {
            //                    bComplete = true ;
            //                    bOk       = false;
            //                    return; 
            //                }
            //            }
            //        }));
            //    }

            //    else
            //    {
                    for(int i=0; i<dataGridView1.RowCount-1; i++)
                    {
                        if(!Load(lst,dataGridView1.GetString("PATH",i) ,i+1 )) {
                            bComplete = true ;
                            bOk       = false;
                            return false; 
                        }
                    }
                //}
            //}

            //if(OM.DevInfo.sNo1  != "" ) { if(!Load(lst,OM.DevInfo.sNo1 ,1 )) return false; }
            //if(OM.DevInfo.sNo2  != "" ) { if(!Load(lst,OM.DevInfo.sNo2 ,2 )) return false; }
            //if(OM.DevInfo.sNo3  != "" ) { if(!Load(lst,OM.DevInfo.sNo3 ,3 )) return false; }
            //if(OM.DevInfo.sNo4  != "" ) { if(!Load(lst,OM.DevInfo.sNo4 ,4 )) return false; }
            //if(OM.DevInfo.sNo5  != "" ) { if(!Load(lst,OM.DevInfo.sNo5 ,5 )) return false; }
            //if(OM.DevInfo.sNo6  != "" ) { if(!Load(lst,OM.DevInfo.sNo6 ,6 )) return false; }
            //if(OM.DevInfo.sNo7  != "" ) { if(!Load(lst,OM.DevInfo.sNo7 ,7 )) return false; }
            //if(OM.DevInfo.sNo8  != "" ) { if(!Load(lst,OM.DevInfo.sNo8 ,8 )) return false; }
            //if(OM.DevInfo.sNo9  != "" ) { if(!Load(lst,OM.DevInfo.sNo9 ,9 )) return false; }
            //if(OM.DevInfo.sNo10 != "" ) { if(!Load(lst,OM.DevInfo.sNo10,10)) return false; }
            //if(OM.DevInfo.sNo11 != "" ) { if(!Load(lst,OM.DevInfo.sNo11,11)) return false; }
            //if(OM.DevInfo.sNo12 != "" ) { if(!Load(lst,OM.DevInfo.sNo12,12)) return false; }
            //if(OM.DevInfo.sNo13 != "" ) { if(!Load(lst,OM.DevInfo.sNo13,13)) return false; }
            //if(OM.DevInfo.sNo14 != "" ) { if(!Load(lst,OM.DevInfo.sNo14,14)) return false; }
            //if(OM.DevInfo.sNo15 != "" ) { if(!Load(lst,OM.DevInfo.sNo15,15)) return false; }
            //if(OM.DevInfo.sNo16 != "" ) { if(!Load(lst,OM.DevInfo.sNo16,16)) return false; }
            //if(OM.DevInfo.sNo17 != "" ) { if(!Load(lst,OM.DevInfo.sNo17,17)) return false; }
            //if(OM.DevInfo.sNo18 != "" ) { if(!Load(lst,OM.DevInfo.sNo18,18)) return false; }
            //if(OM.DevInfo.sNo19 != "" ) { if(!Load(lst,OM.DevInfo.sNo19,19)) return false; }
            //if(OM.DevInfo.sNo20 != "" ) { if(!Load(lst,OM.DevInfo.sNo20,20)) return false; }

            return true;
        }

        public bool RecipeNameExist(string sName)
        {
            bool bRet = false;
            for(int i=0; i<dataGridView1.RowCount-1; i++)
            {
                if(dataGridView1.GetString("PATH",i) == sName) return true ;
            }
            return bRet ;
        }
        public bool RecipeSetting(string sPath = "")
        {
          
            List<string> lst = new List<string>();
            bool bRet = true;
            bOk = true;

            //if (dataGridView1.Parent != null)
            //{
            //    //UI에 접근 하기 위한 인보크
            //    if (dataGridView1.InvokeRequired)
            //    {
            //        dataGridView1.Invoke(new MethodInvoker(delegate ()
            //        {
            //            for(int i=0; i<dataGridView1.RowCount-1; i++)
            //            {
            //                if(!Load(lst,dataGridView1.GetString("PATH",i) ,i+1 )) {
            //                    bComplete = true ;
            //                    bOk       = false;
            //                    bRet = false; 
            //                }
            //            }
            //        }));
            //    }

            //    else
            //    {
                    for(int i=0; i<dataGridView1.RowCount-1; i++)
                    {
                        if(!Load(lst,dataGridView1.GetString("PATH",i) ,i+1 )) {
                            bComplete = true ;
                            bOk       = false;
                            bRet = false; 
                        }
                    }
            //    }
            //}

            //for(int i=0; i<dataGridView1.RowCount-1; i++)
            //{
            //    if(!Load(lst,OM.DevInfo.sNo1 ,i+1 )) return false; 
            //}
            //for(int i=1; i<25; i++) //아 너무 고정이네 
            //{
                //if(OM.DevInfo.sNo1  != "" ) { if(!Load(lst,OM.DevInfo.sNo1 ,1 )) bRet = false; }
                //if(OM.DevInfo.sNo2  != "" ) { if(!Load(lst,OM.DevInfo.sNo2 ,2 )) bRet = false; }
                //if(OM.DevInfo.sNo3  != "" ) { if(!Load(lst,OM.DevInfo.sNo3 ,3 )) bRet = false; }
                //if(OM.DevInfo.sNo4  != "" ) { if(!Load(lst,OM.DevInfo.sNo4 ,4 )) bRet = false; }
                //if(OM.DevInfo.sNo5  != "" ) { if(!Load(lst,OM.DevInfo.sNo5 ,5 )) bRet = false; }
                //if(OM.DevInfo.sNo6  != "" ) { if(!Load(lst,OM.DevInfo.sNo6 ,6 )) bRet = false; }
                //if(OM.DevInfo.sNo7  != "" ) { if(!Load(lst,OM.DevInfo.sNo7 ,7 )) bRet = false; }
                //if(OM.DevInfo.sNo8  != "" ) { if(!Load(lst,OM.DevInfo.sNo8 ,8 )) bRet = false; }
                //if(OM.DevInfo.sNo9  != "" ) { if(!Load(lst,OM.DevInfo.sNo9 ,9 )) bRet = false; }
                //if(OM.DevInfo.sNo10 != "" ) { if(!Load(lst,OM.DevInfo.sNo10,10)) bRet = false; }
                //if(OM.DevInfo.sNo11 != "" ) { if(!Load(lst,OM.DevInfo.sNo11,11)) bRet = false; }
                //if(OM.DevInfo.sNo12 != "" ) { if(!Load(lst,OM.DevInfo.sNo12,12)) bRet = false; }
                //if(OM.DevInfo.sNo13 != "" ) { if(!Load(lst,OM.DevInfo.sNo13,13)) bRet = false; }
                //if(OM.DevInfo.sNo14 != "" ) { if(!Load(lst,OM.DevInfo.sNo14,14)) bRet = false; }
                //if(OM.DevInfo.sNo15 != "" ) { if(!Load(lst,OM.DevInfo.sNo15,15)) bRet = false; }
                //if(OM.DevInfo.sNo16 != "" ) { if(!Load(lst,OM.DevInfo.sNo16,16)) bRet = false; }
                //if(OM.DevInfo.sNo17 != "" ) { if(!Load(lst,OM.DevInfo.sNo17,17)) bRet = false; }
                //if(OM.DevInfo.sNo18 != "" ) { if(!Load(lst,OM.DevInfo.sNo18,18)) bRet = false; }
                //if(OM.DevInfo.sNo19 != "" ) { if(!Load(lst,OM.DevInfo.sNo19,19)) bRet = false; }
                //if(OM.DevInfo.sNo20 != "" ) { if(!Load(lst,OM.DevInfo.sNo20,20)) bRet = false; }
            //}

            if(!bRet)
            {
                Log.ShowMessage("Confirm","File not found.");
                return false;
            }

            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevInfoPath = sExeFolder + "Recipe\\" + OM.GetCrntDev() + "\\Recipe.csv";
            string sCopyPath    = sExeFolder + "Recipe\\" + OM.GetCrntDev() + "\\RecipeRead.csv";
            
            if(sPath != "")
            {
                sDevInfoPath = Path.GetDirectoryName(sPath) + "\\Recipe.csv";
                sCopyPath    = Path.GetDirectoryName(sPath) + "\\RecipeRead.csv";
            }

            if(File.Exists(sDevInfoPath)) File.Delete(sDevInfoPath);
            Save(lst,sDevInfoPath);

            if(File.Exists(sCopyPath   )) File.Delete(sCopyPath   );
            if(File.Exists(sDevInfoPath)) File.Copy(sDevInfoPath,sCopyPath);

            //데이터 그리드뷰 불러와서 셋팅하기
            OM.SeasoningOptnView.Load(sDevInfoPath);
            FormOperation.FrmGrid.SetList();

            return true;
        }

        static public bool Load(List<string> _lst, string _sName, int _iNo)
        {
            string fileName = _sName;//System.AppDomain.CurrentDomain.BaseDirectory + "Util\\" + _iNo.ToString() +".csv";
            string sName    = Path.GetFileNameWithoutExtension(fileName);
            //fileName = _sName;//System.AppDomain.CurrentDomain.BaseDirectory + "Util\\" + _iNo.ToString() +".csv";
            string sNo = _iNo.ToString();
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

            if(_lst.Count == 0) _lst.Add("clNo,clName," + sDatas[0]);
            for(int i=1; i<sDatas.Length; i++)
            {
                _lst.Add(sNo + "," + sName + "," + sDatas[i]);
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

        private void button1_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            string sTag  = ((Button)sender).Tag.ToString();
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevInfoPath = sExeFolder + "RecipeSub\\" ;
            OpenFileDialog fd   = new OpenFileDialog();
            fd.InitialDirectory = sDevInfoPath;
            fd.Filter           = "csv files (*.csv)|*.csv";
            
            DialogResult dr = fd.ShowDialog();
            if(dr != DialogResult.OK) return;

                 if(sTag == "1" ) tb1 .Text = fd.FileName ;
            else if(sTag == "2" ) tb2 .Text = fd.FileName ;
            else if(sTag == "3" ) tb3 .Text = fd.FileName ;
            else if(sTag == "4" ) tb4 .Text = fd.FileName ;
            else if(sTag == "5" ) tb5 .Text = fd.FileName ;
            else if(sTag == "6" ) tb6 .Text = fd.FileName ;
            else if(sTag == "7" ) tb7 .Text = fd.FileName ;
            else if(sTag == "8" ) tb8 .Text = fd.FileName ;
            else if(sTag == "9" ) tb9 .Text = fd.FileName ;
            else if(sTag == "10") tb10.Text = fd.FileName ;
            else if(sTag == "11") tb11.Text = fd.FileName ;
            else if(sTag == "12") tb12.Text = fd.FileName ;
            else if(sTag == "13") tb13.Text = fd.FileName ;
            else if(sTag == "14") tb14.Text = fd.FileName ;
            else if(sTag == "15") tb15.Text = fd.FileName ;
            else if(sTag == "16") tb16.Text = fd.FileName ;
            else if(sTag == "17") tb17.Text = fd.FileName ;
            else if(sTag == "18") tb18.Text = fd.FileName ;
            else if(sTag == "19") tb19.Text = fd.FileName ;
            else if(sTag == "20") tb20.Text = fd.FileName ;
        }

        private void btClear_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            string sTag  = ((Button)sender).Tag.ToString();
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            if (Log.ShowMessageModal("Confirm", "Do you want to clear?") != DialogResult.Yes) return;

                 if(sTag == "1" ) tb1 .Text = "" ;
            else if(sTag == "2" ) tb2 .Text = "" ;
            else if(sTag == "3" ) tb3 .Text = "" ;
            else if(sTag == "4" ) tb4 .Text = "" ;
            else if(sTag == "5" ) tb5 .Text = "" ;
            else if(sTag == "6" ) tb6 .Text = "" ;
            else if(sTag == "7" ) tb7 .Text = "" ;
            else if(sTag == "8" ) tb8 .Text = "" ;
            else if(sTag == "9" ) tb9 .Text = "" ;
            else if(sTag == "10") tb10.Text = "" ;
            else if(sTag == "11") tb11.Text = "" ;
            else if(sTag == "12") tb12.Text = "" ;
            else if(sTag == "13") tb13.Text = "" ;
            else if(sTag == "14") tb14.Text = "" ;
            else if(sTag == "15") tb15.Text = "" ;
            else if(sTag == "16") tb16.Text = "" ;
            else if(sTag == "17") tb17.Text = "" ;
            else if(sTag == "18") tb18.Text = "" ;
            else if(sTag == "19") tb19.Text = "" ;
            else if(sTag == "20") tb20.Text = "" ;

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
            {
                if(e.RowIndex == senderGrid.Rows.Count - 1)
                {
                    senderGrid.Rows.Add();
                }
                if(e.ColumnIndex == 1 && e.RowIndex < senderGrid.Rows.Count - 1) senderGrid.Rows.RemoveAt(e.RowIndex);
                if(e.ColumnIndex == 2 && e.RowIndex < senderGrid.Rows.Count - 1) 
                {
                    string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
                    string sDevInfoPath = sExeFolder + "RecipeSub\\" ;
                    OpenFileDialog fd   = new OpenFileDialog();
                    //fd.InitialDirectory = sDevInfoPath;
                    fd.Filter           = "csv files (*.csv)|*.csv";
                    
                    DialogResult dr = fd.ShowDialog();
                    if(dr != DialogResult.OK) return;

                    senderGrid.SetString("PATH",e.RowIndex,fd.FileName);
                }
            }
        }

        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            DataGridView grid = sender as DataGridView;
            //String rowIdx = (e.RowIndex + 1).ToString("X4");
            String rowIdx = (e.RowIndex + 1).ToString();
            
            StringFormat centerFormat = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            
            Rectangle headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top,grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.ControlText,headerBounds,centerFormat);
        }

        private void btSaveAsDevice_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            if (LOT.GetLotOpen())
            {
                Log.ShowMessage("Error", "Please check the status of the Lot(Need to Lot End).");
                return;
            }

            //if (Log.ShowMessageModal("Confirm", "Are you Sure?") != DialogResult.Yes) return;

            //UpdateDevInfo(false);

            //OM.SaveDevInfo(OM.GetCrntDev().ToString());
            //OM.SaveEqpOptn();

            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevInfoPath = sExeFolder + "Recipe\\" ;//+ OM.GetCrntDev() + "\\RecipeSetting.csv";

            //FolderBrowserDialog dialog = new FolderBrowserDialog();
            //dialog.RootFolderr = Environment.SpecialFolder;
            //sDevInfoPath ;
            //if(dialog.ShowDialog() != DialogResult.OK) return ;

            var dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = sDevInfoPath ;
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() != CommonFileDialogResult.Ok) return ;
            //this.Path = dialog.FileName;
            
            string select_path = dialog.FileName;    //선택한 다이얼로그 경로 저장
            sDevInfoPath = select_path + "\\RecipeSetting.csv";

            
            if(File.Exists(sDevInfoPath)) File.Delete(sDevInfoPath);
            dataGridView1.Save(sDevInfoPath);
            RecipeSetting(sDevInfoPath);
            Invalidate(true);
        }

        private void btDel_Click(object sender, EventArgs e)
        {
            if(dataGridView1.CurrentCell == null) return ;
            int idx = dataGridView1.CurrentCell.RowIndex;
            if(idx < dataGridView1.Rows.Count - 1) dataGridView1.Rows.RemoveAt(idx);
        }

        private void btAdd_Click(object sender, EventArgs e)
        {
            if(dataGridView1.CurrentCell == null) return ;
            int idx = dataGridView1.CurrentCell.RowIndex;
            dataGridView1.Rows.Insert(idx);
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
