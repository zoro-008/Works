using System;
using System.Drawing;
using System.Windows.Forms;
using COMMON;
using SML;
using System.Reflection;
using System.Threading;
using System.Runtime.InteropServices;
//using VDll;

//TODO :: 
//LotInfoCnt , DayInfoCnt 갯수 지정해 주기 및 화면 컴파일후 배치 맞추기
namespace Machine
{
    public partial class FormOperation : Form
    {
        public static FormOperation FrmOperation;
        //public static FormLotOpen FrmLotOpen;
        public static FormMain FrmMain;

        public int LotInfoCnt = 6;
        public int DayInfoCnt = 4;

        private const string sFormText = "Form Operation ";
        
        [DllImport("Kernel32.dll")]
        public static extern bool IsWow64Process(System.IntPtr hProcess, out bool lpSystemInfo);
        public FormOperation(Panel _pnBase)
        {
            InitializeComponent();

            this.TopLevel = false;
            this.Parent = _pnBase;

            this.Dock = DockStyle.Fill;

            //Scable Setting
            //int  _iWidth  = _pnBase.Width;
            //int  _iHeight = _pnBase.Height;
            //
            //const int  iWidth  = 1280;
            //const int  iHeight = 863;
            //
            //float widthRatio  = _iWidth   / (float)iWidth;// this.ClientSize.Width;//1280f;
            //float heightRatio = _iHeight  / (float)iHeight;//.ClientSize.Height; //863f ;
            //
            //SizeF scale = new SizeF(widthRatio, heightRatio);

            DispLotInfo();

            //Form Option이 열려있지 않아서 Common Option을 Operation 창에서 저장
            UpdateComOptn(true);

            OM.LoadCmnOptn();

            MakeDoubleBuffered(pnCST,true);
            MakeDoubleBuffered(pnPCK,true);
            MakeDoubleBuffered(pnSTG,true);

            MakeDoubleBuffered(pnInfoImg ,true);
            MakeDoubleBuffered(pnPckWafer,true);
            MakeDoubleBuffered(pnStgWafer,true);
            MakeDoubleBuffered(pnCassette,true);

            DM.ARAY[ri.CST].SetParent(pnCST); DM.ARAY[ri.CST].Name = "CASSETTE";
            DM.ARAY[ri.PCK].SetParent(pnPCK); DM.ARAY[ri.PCK].Name = "PICKER"  ;
            DM.ARAY[ri.STG].SetParent(pnSTG); DM.ARAY[ri.STG].Name = "STAGE"   ;

            //CASSETTE
            DM.ARAY[ri.CST].SetDisp(cs.None    , "None"        ,Color.White     );
            DM.ARAY[ri.CST].SetDisp(cs.Unkn    , "Before Work" ,Color.Aquamarine);
            DM.ARAY[ri.CST].SetDisp(cs.Mask    , "Mask"        ,Color.Yellow    );
            DM.ARAY[ri.CST].SetDisp(cs.Work    , "Work"        ,Color.BlueViolet);
            DM.ARAY[ri.CST].SetDisp(cs.Empty   , "Empty"       ,Color.DimGray   );
                                                                              
            //Picker                                                       
            DM.ARAY[ri.PCK].SetDisp(cs.None    , "None"        ,Color.White     );
            DM.ARAY[ri.PCK].SetDisp(cs.Unkn    , "BeforeWork"  ,Color.Aquamarine);
            DM.ARAY[ri.PCK].SetDisp(cs.Work    , "Work"        ,Color.BlueViolet);

            //Suttle                                                        
            DM.ARAY[ri.STG].SetDisp(cs.None    , "None"        ,Color.White     );
            DM.ARAY[ri.STG].SetDisp(cs.Unkn    , "Before Work" ,Color.Aquamarine);
            DM.ARAY[ri.STG].SetDisp(cs.Work    , "Work"        ,Color.BlueViolet);

            DM.LoadMap();

            DM.ARAY[ri.CST].SetMaxColRow(1,25);
            DM.ARAY[ri.PCK].SetMaxColRow(1,1);
            DM.ARAY[ri.STG].SetMaxColRow(1,1);
        }

        private void FormOperation_Load(object sender, EventArgs e)
        {
            //시간없어서 그냥 
            //나중에한번 보자. DOCK으로 하면 이상하게 폼이 안붙음.....
            pnOperMain.Dock = DockStyle.None ;
            pnOperMain.Left = 0 ;
            pnOperMain.Top  = 0 ;
            pnOperMain.Width = Parent.Width ;
            pnOperMain.Height = Parent.Height ;

            Log.SetMessageListBox(lvInfo);
            Log.TraceListView("Program Started");

            //lvError.OwnerDraw = true ; 
            //var PropError = lvError.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            //PropError.SetValue(lvError, true, null);

            lvError.View = View.Details ;
            lvError.GridLines = false;
            lvError.HeaderStyle = ColumnHeaderStyle.None ;
            lvError.Columns.Add("err", lvError.Size.Width, HorizontalAlignment.Left);
            var PropError = lvError.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropError.SetValue(lvError, true, null);
            
        }

         public void DispLotInfo() //오퍼레이션 창용.
        {
            /*
            lvLotInfo.Clear();
            lvLotInfo.View = View.Details;
            lvLotInfo.LabelEdit = true;
            lvLotInfo.AllowColumnReorder = true;
            lvLotInfo.FullRowSelect = true;
            lvLotInfo.GridLines = true;
            lvLotInfo.Scrollable = true;
            lvLotInfo.Enabled = true;

            lvLotInfo.Columns.Add("", 200, HorizontalAlignment.Left);     //1280*1024는 109, 1024*768은 78
            lvLotInfo.Columns.Add("", lvLotInfo.Width - 200, HorizontalAlignment.Left);     //1280*1024는 109, 1024*768은 78

            ListViewItem[] liLotInfo = new ListViewItem[LotInfoCnt*2];
        
            for (int i = 0; i < LotInfoCnt; i++)
            {
                liLotInfo[i] = new ListViewItem();
                liLotInfo[i].SubItems.Add("");
        
                liLotInfo[i].UseItemStyleForSubItems = false;
                liLotInfo[i].UseItemStyleForSubItems = false;

                lvLotInfo.Items.Add(liLotInfo[i]);
              
            }
        
            var PropLotInfo = lvLotInfo.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropLotInfo.SetValue(lvLotInfo, true, null);
            */
        }

        private void FormOperation_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmUpdate.Enabled = false;
            DM.SaveMap();
        }

        private void FormOperation_Shown(object sender, EventArgs e)
        {
            tmUpdate.Enabled = true;
        }

        private void FormOperation_VisibleChanged(object sender, EventArgs e)
        {
            tmUpdate.Enabled = this.Visible;
        }

        private void btStart_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ForContext.Frm);

            SEQ._bBtnStart = true;
        }

        private void btStop_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.TraceListView(sFormText + sText + " Button Clicked", ForContext.Frm);

            SEQ._bBtnStop = true;
        }

        private void btReset_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.TraceListView(sFormText + sText + " Button Clicked", ForContext.Frm);

            SEQ._bBtnReset = true;
        }

        private void btLogIn_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.TraceListView(sFormText + sText + " Button Clicked", ForContext.Frm);

            SM.FrmLogOn.Show();
        }

        private void btHome_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.TraceListView(sFormText + sText + " Button Clicked", ForContext.Frm);

            if (!OM.MstOptn.bDebugMode)
            {
                if (Log.ShowMessageModal("Confirm", "Do you want to perform manual actions?") != DialogResult.Yes) return;
                MM.SetManCycle(mc.AllHome);
            }
            else
            {
                DialogResult Rslt;
                Rslt = MessageBox.Show("홈동작을 생략 하겠습니까?", "Confirm", MessageBoxButtons.YesNoCancel);
                if (Rslt == DialogResult.Yes)
                {
                    ML.MT_SetServoAll(true);
                    Thread.Sleep(100);
                    for (mi i = 0; i < mi.MAX_MOTR; i++)
                    {
                        ML.MT_SetHomeDone(i, true);
                    }
                }
                else if (Rslt == DialogResult.No)
                {
                    MM.SetManCycle(mc.AllHome);
                }
            }
        }


        private void PanelRefresh()
        {
            for (int i = 0 ; i < ri.MAX_ARAY ; i++)
            {
                DM.ARAY[i].UpdateAray();
            }

            pnCST.Refresh();
            pnPCK.Refresh();
            pnSTG.Refresh();

        }


        int iPreErrCnt  = 0;
        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            
            PanelRefresh();

            pnData.Enabled = MM.GetManNo() == mc.NoneCycle ;
            btAllWafer.Enabled = MM.GetManNo() == mc.NoneCycle;

            //로그인/로그아웃 방식
            if (SM.FrmLogOn.GetLevel() == (int)EN_LEVEL.LogOff)
            {
                btLogIn.Text = "  LOG IN";
            }
            else
            {
                btLogIn.Text = "  " + SM.FrmLogOn.GetLevel().ToString();
            }

            
                      
            //lvLotInfo.Items.Clear();
            //SPC.LOT.DispLotInfo(lvLotInfo);
            //SPC.DAY.DispDayInfo(lvLotInfo);

            int iCrntErrCnt = 0;
            for (int i = 0 ; i < ML.ER_MaxCount() ; i++) 
            {
                if (ML.ER_GetErr((ei)i)) iCrntErrCnt++;
            }
            if (iPreErrCnt != iCrntErrCnt ) 
            {
                lvError.Items.Clear();
                int iErrNo = ML.ER_GetLastErr();
                ListViewItem liError ;
                string sErr ;
                for (int i = 0; i < ML.ER_MaxCount(); i++)
                {
                    if (ML.ER_GetErr(i))
                    {
                        sErr = string.Format("{0:000} - ", i) + ML.ER_GetErrName(i) + " _ " + ML.ER_GetErrSubMsg(i);
                        liError = new ListViewItem(sErr);
                        lvError.Items.Add(liError);
                    }
                    else
                    {
                        //sErr = string.Format("{0:000} - ", i) + ML.ER_GetErrName(i) + " _ " + ML.ER_GetErrSubMsg(i);
                        //liError = new ListViewItem(sErr);
                        //lvError.Items.Add(liError);
                    }
                }
                //lvError.Columns[0].Width = lvError.Width - 100 ;
            }
            if (SEQ._iSeqStat != EN_SEQ_STAT.Error && lvError.Items.Count != 0)
            {
                lvError.Items.Clear();
            }
            iPreErrCnt = iCrntErrCnt ;

            if(!ML.MT_GetHomeDoneAll()){
                btHome.ForeColor = SEQ._bFlick ? Color.White : Color.Red;
            }
            else {
                btHome.ForeColor = Color.White  ;
            }

            //자재 보여주기
            bool bPckNone =  DM.ARAY[ri.PCK].CheckAllStat(cs.None) ;//&& !ML.IO_GetX(xi.WaferVacuum); 
            bool bStgNone =  DM.ARAY[ri.STG].CheckAllStat(cs.None) ;//&& !ML.IO_GetX(xi.WaferDtSsr ); 
            bool bCstNone =  DM.ARAY[ri.CST].CheckAllStat(cs.None) ;//ML.IO_GetX(xi.CassetteLeft) &&  ML.IO_GetX(xi.CassetteRight); 

            bool bDisp1 = !bPckNone && !ML.IO_GetX(xi.PickerVacuum) ;
            bool bUnkn1 =  bPckNone &&  ML.IO_GetX(xi.PickerVacuum) ;

            bool bDisp2 = !bStgNone && !ML.IO_GetX(xi.WaferDtSsr ) ;
            bool bUnkn2 =  bStgNone &&  ML.IO_GetX(xi.WaferDtSsr ) ;

            //bool bDisp3 = !bCstNone && (!ML.IO_GetX(xi.CassetteLeft) || !ML.IO_GetX(xi.CassetteLeft)) ;
            //bool bUnkn3 =  bCstNone && ( ML.IO_GetX(xi.CassetteLeft) ||  ML.IO_GetX(xi.CassetteLeft)) ;
            
            bool bCst =  ML.IO_GetX(xi.CassetteLeft) &&  ML.IO_GetX(xi.CassetteRight);

            if (bDisp1 || bUnkn1) pnPckWafer.Visible = SEQ._bFlick ;
            else                  pnPckWafer.Visible = !bPckNone   ;

            if(bDisp2 || bUnkn2) pnStgWafer.Visible = SEQ._bFlick ;
            else                 pnStgWafer.Visible = !bStgNone   ;

            //if(bDisp3 || bUnkn3) pnCassette.Visible = bCst;
            //else                 pnCassette.Visible = !bCstNone   ;
            pnCassette.Visible = bCst;

            //센서 보여주기
            bool bSsr = false;
            bSsr = ML.IO_GetX(xi.CassetteLeft    ); l1.BackColor = bSsr ? Color.Lime : Color.Gray ; l1.Text = bSsr ? "ON" : "OFF" ;
            bSsr = ML.IO_GetX(xi.CassetteRight   ); l2.BackColor = bSsr ? Color.Lime : Color.Gray ; l2.Text = bSsr ? "ON" : "OFF" ;
            bSsr = ML.IO_GetX(xi.PickerVacuum    ); l3.BackColor = bSsr ? Color.Lime : Color.Gray ; l3.Text = bSsr ? "ON" : "OFF" ;
            bSsr = ML.IO_GetX(xi.WaferOverload   ); l4.BackColor = bSsr ? Color.Lime : Color.Gray ; l4.Text = bSsr ? "ON" : "OFF" ;
            bSsr = ML.IO_GetX(xi.StageVacuum     ); l5.BackColor = bSsr ? Color.Lime : Color.Gray ; l5.Text = bSsr ? "ON" : "OFF" ;
            bSsr = ML.IO_GetX(xi.WaferDtSsr      ); l6.BackColor = bSsr ? Color.Lime : Color.Gray ; l6.Text = bSsr ? "ON" : "OFF" ;
            bSsr = ML.IO_GetX(xi.ManualInspLimit ); l7.BackColor = bSsr ? Color.Lime : Color.Gray ; l7.Text = bSsr ? "ON" : "OFF" ;

            //출력 보여주기
            bSsr = ML.IO_GetY(yi.PickerVacuum     ); l10.BackColor = bSsr ? Color.Red  : Color.Gray ; l10.Text = bSsr ? "ON" : "OFF" ;
            bSsr = ML.IO_GetY(yi.StageVacuum     ); l11.BackColor = bSsr ? Color.Red  : Color.Gray ; l11.Text = bSsr ? "ON" : "OFF" ;
            
            //그냥 상태 색깔 표기용
            pnS1.BackColor = Color.Aquamarine ; //작업전
            pnS2.BackColor = Color.BlueViolet ; //작업후
            pnS3.BackColor = Color.DimGray    ; //빈슬롯
            pnS4.BackColor = Color.Yellow; //작업중인슬롯

            //if(cbLoadAtUnload.Checked) label32.Text = " cbLoadAtUnload.Checked";
            //else                       label32.Text = "!cbLoadAtUnload.Checked";
            //
            //if (cbLoadAtUnload.CheckState == CheckState.Checked) label31.Text = " cbLoadAtUnload.CheckState";
            //else                                                label31.Text = "!cbLoadAtUnload.CheckState";

            //Check Running Status.
            cbLoadAtUnload.Enabled = !SEQ._bRun ;

            //if(!ML.IO_GetX(xi.CassetteLeft) || !ML.IO_GetX(xi.CassetteRight)) //&& 조건에서 안될 것 같아서 일단 ||조건으로 본다.안되면 바꿈
            //{
            //    if(ML.IO_GetX(xi.CassetteLeft) && ML.IO_GetX(xi.CassetteRight))
            //    {
            //        DM.ARAY[ri.CST].ChangeStat(cs.Work, cs.Unkn);
            //    }
            //}
        }


        public void MakeDoubleBuffered(Control control, bool setting)
        {
            Type controlType = control.GetType();
            PropertyInfo pi = controlType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(control, setting, null);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text ;
            Log.TraceListView(sFormText + sText + " Button Clicked", ForContext.Frm);

            string sTag = ((Button)sender).Tag.ToString();
            if(sTag.Contains("W"))
            {
                sTag = sTag.Remove(0,1);
                int.TryParse(sTag,out int iTag);
                DM.ARAY[ri.CST].SetStat(0, OM.DevInfo.iRowCount - iTag, cs.Unkn);
            }
            else
            {
                sTag = sTag.Remove(0, 1);
                int.TryParse(sTag, out int iTag);
                DM.ARAY[ri.CST].SetStat(0, OM.DevInfo.iRowCount - iTag, cs.Empty);
            }
        }

        private void btManualClick(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text ;
            Log.TraceListView(sFormText + sText + " Button Clicked", ForContext.Frm);
 
            if (Log.ShowMessageModal("Confirm", "Do you want to perform manual actions?") != DialogResult.Yes) return;

            string sTag = ((Button)sender).Tag.ToString();
            if(!int.TryParse(sTag, out int iTag)) return ;
            MM.SetManCycle((mc)iTag);
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text ;
            Log.TraceListView(sFormText + sText + " Button Clicked", ForContext.Frm);

            DM.ARAY[ri.CST].SetStat(cs.Unkn);
        }

        private void btAllWafer_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text ;
            Log.TraceListView(sFormText + sText + " Button Clicked", ForContext.Frm);

            DM.ARAY[ri.CST].SetStat(cs.Unkn);
        }

        private void btRemoveStg_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text ;
            Log.TraceListView(sFormText + sText + " Button Clicked", ForContext.Frm);

            if (Log.ShowMessageModal("Confirm", "스테이지 자재 데이터를 제거 하시겠습니까?") != DialogResult.Yes) return;
            DM.ARAY[ri.STG].SetStat(cs.None);

        }

        private void btRemovePck_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text ;
            Log.TraceListView(sFormText + sText + " Button Clicked", ForContext.Frm);

            if (Log.ShowMessageModal("Confirm", "스테이지 자재 데이터를 제거 하시겠습니까?") != DialogResult.Yes) return;
            DM.ARAY[ri.PCK].SetStat(cs.None);

        }

        private void pnPckWafer_Click(object sender, EventArgs e)
        {
            btRemovePck.PerformClick();
        }

        private void pnStgWafer_Click(object sender, EventArgs e)
        {
            btRemoveStg.PerformClick();
        }

        private void label42_Click(object sender, EventArgs e)
        {
            ML.IO_SetY(yi.PickerVacuum , !ML.IO_GetY(yi.PickerVacuum));
        }

        private void l11_Click(object sender, EventArgs e)
        {
            ML.IO_SetY(yi.StageVacuum, !ML.IO_GetY(yi.StageVacuum));
        }

        private void btAllEmpty_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.TraceListView(sFormText + sText + " Button Clicked", ForContext.Frm);

            DM.ARAY[ri.CST].SetStat(cs.Empty);
        }

        //Form Option이 열려있지 않아서 Common Option을 Operation 창에서 저장
        private void btSave_Click(object sender, EventArgs e)
        {
            //언로딩 작업 후 로딩작업 옵션 셋팅해야하는데
            //Common Option이 열려있지 않아서 Operation 창에서 한다.
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ForContext.Frm);



            if (Log.ShowMessageModal("Confirm", "Are you Sure?") != DialogResult.Yes) return;
            ((Button)sender).Enabled = false;

            UpdateComOptn(false);
            OM.SaveCmnOptn();

            //비전 결과 색 이름 표기
            //SetDisp(ri.RLT1);SetDisp(ri.RLT2);SetDisp(ri.RLT3);
            //SetDisp(ri.WRK1);SetDisp(ri.WRK2);SetDisp(ri.WRK3);
            //SetDisp(ri.PSTB);
            //SetDisp(ri.SPC );

            ((Button)sender).Enabled = true;
        }

        //언로딩 작업 후 로딩작업 옵션 셋팅해야하는데
        //Common Option이 열려있지 않아서 Operation 창에서 한다.
        public void UpdateComOptn(bool _bToTable)
        {                                                                                   
            if (_bToTable == true) 
            {
                CConfig.ValToCon(cbLoadAtUnload , ref OM.CmnOptn.bLoadAtUnload);
            }
            else 
            {
                OM.CCmnOptn PreCmnOptn = OM.CmnOptn;
                CConfig.ConToVal(cbLoadAtUnload , ref OM.CmnOptn.bLoadAtUnload);

                //Auto Log
                Type type = PreCmnOptn.GetType();
                FieldInfo[] f = type.GetFields(BindingFlags.Public|BindingFlags.NonPublic | BindingFlags.Instance);
                for(int i = 0 ; i < f.Length ; i++)
                {
                    if(f[i].GetValue(PreCmnOptn) != f[i].GetValue(OM.CmnOptn))Trace(f[i].Name + " Changed", f[i].GetValue(PreCmnOptn).ToString() , f[i].GetValue(OM.CmnOptn).ToString());
                    else                                                      Trace(f[i].Name             , f[i].GetValue(PreCmnOptn).ToString() , f[i].GetValue(OM.CmnOptn).ToString());
                }

                UpdateComOptn(true);
            }
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

        private void cbLoadAtUnload_CheckedChanged(object sender, EventArgs e)
        {
            string sText = ((CheckBox)sender).Text;
            Log.Trace(sFormText + sText + " CheckBox Changed", ForContext.Frm);

            //Check Running Status.
            //if (SEQ._bRun)
            //{
            // Log.ShowMessage("Warning", "Can't Save during Autorunning!");
            //    return;
            //}

            //if (Log.ShowMessageModal("Confirm", "Are you Sure?") != DialogResult.Yes) return;
            //((Button)sender).Enabled = false;

            UpdateComOptn(false);
            OM.SaveCmnOptn();
            
        }

        private void cbLoadAtUnload_CheckStateChanged(object sender, EventArgs e)
        {

        }
    }

    public class DoubleBuffer : Panel
    {
        public DoubleBuffer()
        {
            this.SetStyle(ControlStyles.ResizeRedraw         , true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint            , true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint , true);

            this.UpdateStyles();
        }
    }
    public class DoubleBufferP : PictureBox
    {
        public DoubleBufferP()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.UserPaint |
              ControlStyles.AllPaintingInWmPaint  |
              ControlStyles.ResizeRedraw          |
              ControlStyles.ContainerControl      |
              ControlStyles.OptimizedDoubleBuffer |
              ControlStyles.SupportsTransparentBackColor
              , true);
        }
    }



}