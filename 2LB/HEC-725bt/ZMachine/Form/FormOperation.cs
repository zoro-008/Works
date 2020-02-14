using System;
using System.Drawing;
using System.Windows.Forms;
using COMMON;
using SML;
using System.Reflection;
using System.Threading;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Machine
{
    public partial class FormOperation : Form
    {
        public static FormOperation FrmOperation;
        public static FormLotOpen FrmLotOpen;
        //public static FormMain FrmMain;

        public int LotInfoCnt = 6;
        public int DayInfoCnt = 6;        

        protected CDelayTimer m_tmStartBt ;

        private const string sFormText = "Form Operation ";
        //public EN_SEQ_STAT m_iSeqStat;

        private FormCam FrmCam ;

        public FormOperation(Panel _pnBase)
        {   
            InitializeComponent();

            this.TopLevel = false;
            this.Parent = _pnBase;
            
            //DispDayList();
            DispLotInfo();

            FrmCam = new FormCam(pnCam);
            FrmCam.Show();

            //Loader           
            //DM.ARAY[ri.LODR].SetDisp     (cs.None     , "None"     , Color.White        );
            //DM.ARAY[ri.USBR].SetMaxColRow(1           , 1                           );
            //DM.ARAY[ri.USBL].SetMaxColRow(1           , 1                           );

            DM.LoadMap();
        }


        private void btOperator_Click(object sender, EventArgs e)
        {
            SM.FrmLogOn.Show();
        }

        private void btLotEnd_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            LOT.LotEnd();
            
            btStart.Enabled = false;
        }

        [DllImport("Kernel32.dll")]
        public static extern bool IsWow64Process(System.IntPtr hProcess, out bool lpSystemInfo);


        private void timer1_Tick(object sender, EventArgs e)
        {
            tmUpdate.Enabled = false;

            bool bRet = false;
            IsWow64Process(System.Diagnostics.Process.GetCurrentProcess().Handle, out bRet);
            //if (bRet) label3.Text = "32BIt";
            //else label3.Text = "64BIt";

            //패스워드만 쓰는 방식
            //int iLevel = (int)FormPassword.GetLevel();
            //switch (iLevel)
            //{
            //    case (int)EN_LEVEL.Operator: btOperator.Text = "OPERATOR"; break;
            //    case (int)EN_LEVEL.Engineer: btOperator.Text = "ENGINEER"; break;
            //    case (int)EN_LEVEL.Master  : btOperator.Text = " ADMIN  "; break;
            //    default                    : btOperator.Text = " ERROR  "; break;
            //}
            btBldHeat.ForeColor = ML.IO_GetY(yi.ETC_HeaterOnBlade) ? Color.Lime : Color.Gray;
            btStgHeat.ForeColor = ML.IO_GetY(yi.ETC_HeaterOnStage) ? Color.Lime : Color.Gray;


            //로그인/로그아웃 방식
            if (SM.FrmLogOn.GetLevel() == (int)EN_LEVEL.LogOff)
            {
                btOperator.Text = "  LOG IN";
                pnCamBase .Enabled = false;
                //pnDayInfo .Enabled = false;
                pnLotInfo .Enabled = false;
                pnError   .Enabled = false;
                pnOperMan .Enabled = false;
                //pnWorkInfo.Enabled = false;
                pnLotOpen .Enabled = false;
                //btStart   .Enabled = LOT.LotList.Count == 0 || !LOT.LotOpened;
                btStart   .Enabled = false;
                btStop    .Enabled = false;
                btReset   .Enabled = false;
                btAllHome .Enabled = false;
                btOperator.Enabled = true;
            }
            else
            {
                btOperator.Text = "  " + SM.FrmLogOn.GetLevel().ToString();
                pnCamBase .Enabled = true;
                //pnDayInfo .Enabled = true;
                pnLotInfo .Enabled = true;
                pnError   .Enabled = true;
                pnOperMan .Enabled = true;
                //pnWorkInfo.Enabled = true;
                pnLotOpen .Enabled = true;
                //btStart   .Enabled = LOT.LotList.Count != 0 || LOT.LotOpened;
                btStart   .Enabled = true;
                btStop    .Enabled = true;
                btReset   .Enabled = true;
                btAllHome .Enabled = true;

            }
                        
            //if (SML.FrmLogOn.GetLevel() != (int)EN_LEVEL.LogOff)
            //{
            //    btStart.Enabled = LOT.GetLotOpen();
            //}

            TimeSpan Span ;
            try{
                    Span = TimeSpan.FromMilliseconds(SPC.LOT.Data.RunTime);
                }
                catch(Exception ex){          
                    Span = TimeSpan.FromMilliseconds(0);
                }
            
            //SPC.LOT.DispLotInfo(lvLotInfo);
            //SPC.DAY.DispDayInfo(lvDayInfo);

            string Str      ;
            int iPreErrCnt  = 0;
            int iCrntErrCnt = 0;
            for (int i = 0 ; i < ML.ER_MaxCount() ; i++) 
            {
                if (ML.ER_GetErr((ei)i)) iCrntErrCnt++;
            }
            if (iPreErrCnt != iCrntErrCnt ) 
            {
                lbErr.Items.Clear();
                int iErrNo = ML.ER_GetLastErr();
                for (int i = 0; i < ML.ER_MaxCount(); i++) 
                {
                    if (ML.ER_GetErr((ei)i))
                    {
                        Str = string.Format("[ERR{0:000}]" , i) ;
                        Str += ML.ER_GetErrName(i) + " " + ML.ER_GetErrSubMsg((ei)i);
                        lbErr.Items.Add(Str);
                    }
                }
            }
            if (SEQ._iSeqStat != EN_SEQ_STAT.Error)
            {
                lbErr.Items.Clear();
            }
            iPreErrCnt = iCrntErrCnt ;
            
            //Door Sensor.  나중에 찾아보자
            //bool isAllCloseDoor = SM.IO.GetX((int)EN_INPUT_ID.xETC_DoorFt) &&
            //                      SM.IO.GetX((int)EN_INPUT_ID.xETC_DoorLt) &&
            //                      SM.IO.GetX((int)EN_INPUT_ID.xETC_DoorRt) &&
            //                      SM.IO.GetX((int)EN_INPUT_ID.xETC_DoorRr) ;
            //if (FormPassword.GetLevel() != EN_LEVEL.lvOperator && isAllCloseDoor && CMachine._bRun)
            //{
            //    //FM_SetLevel(lvOperator);
            //}
            
            if(!ML.MT_GetHomeDoneAll()){
                btAllHome.ForeColor = SEQ._bFlick ? Color.Black : Color.Red;
            }
            else {
                btAllHome.ForeColor = Color.Black  ;
            }

            SPC.LOT.DispLotInfo(lvLot);

            Refresh();

            if (!this.Visible)
            {
                tmUpdate.Enabled = false;
                return;
            }
            
            //lbWorkInfo0.Text = OM.EqpStat.iLastWorkStep.ToString();            

            btLotOpen.Enabled = !LOT.LotOpened ;
            btLotEnd .Enabled =  LOT.LotOpened ;

            tmUpdate.Enabled = true;
        }

        private void btLotOpen_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            FrmLotOpen = new FormLotOpen();
            FrmLotOpen.Show();
        }

        //public void DispLotInfo(ListView _lvLotInfo) //오퍼레이션 창용.
        public void DispLotInfo() //오퍼레이션 창용.
        {
            lvLot.Clear();
            lvLot.View = View.Details;
            lvLot.LabelEdit = true;
            lvLot.AllowColumnReorder = true;
            lvLot.FullRowSelect = true;
            lvLot.GridLines = true;
            lvLot.Sorting = SortOrder.Descending;
            lvLot.Scrollable = true;
            lvLot.Enabled = false;

            //lvLotInfo.Columns.Add("", 200, HorizontalAlignment.Left);     //1280*1024는 109, 1024*768은 78
            //lvLotInfo.Columns.Add("", 300, HorizontalAlignment.Left);     //1280*1024는 109, 1024*768은 78
            lvLot.Columns.Add("", 100, HorizontalAlignment.Left);     //1280*1024는 109, 1024*768은 78
            lvLot.Columns.Add("", 100, HorizontalAlignment.Left);     //1280*1024는 109, 1024*768은 78

            ListViewItem[] liLotInfo = new ListViewItem[LotInfoCnt];
        
            for (int i = 0; i < LotInfoCnt; i++)
            {
                liLotInfo[i] = new ListViewItem();
                liLotInfo[i].SubItems.Add("");
        
        
                liLotInfo[i].UseItemStyleForSubItems = false;
                liLotInfo[i].UseItemStyleForSubItems = false;

                liLotInfo[i].BackColor = Color.White;
                lvLot.Items.Add(liLotInfo[i]);
                
            }
        
            var PropLotInfo = lvLot.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropLotInfo.SetValue(lvLot, true, null);
        }

        private void btStop_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);
            SEQ._bBtnStop = true;
        }

        private void btStart_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);
            
            SEQ._bBtnStart = true;
        }

        private void btReset_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            SEQ._bBtnReset = true;
        }

        private void lvDayInfo_MouseDoubleClick(object sender, MouseEventArgs e)  //요거는 확인 해봐야 함 진섭
        {

        }

        private void btAllHome_Click(object sender, EventArgs e)
        {
            if (!OM.MstOptn.bDebugMode)
            {
                if (Log.ShowMessageModal("Confirm", "Do you want to All Homming?") != DialogResult.Yes) return;
                MM.SetManCycle(mc.AllHome);
            }
            else
            {
                DialogResult Rslt ;
                Rslt = MessageBox.Show(new Form(){TopMost = true}, "홈동작을 생략 하겠습니까?", "Confirm", MessageBoxButtons.YesNoCancel);
                if (Rslt == DialogResult.Yes)
                {
                    ML.MT_SetServoAll(true);
                    Thread.Sleep(100);
                    for (mi i = 0; i < mi.MAX_MOTR; i++)
                    {
                        ML.MT_SetHomeDone(i, true);
                    }
                }
                else if(Rslt == DialogResult.No)
                {
                    MM.SetManCycle(mc.AllHome);
                }
            }            
        }

        private void FormOperation_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmUpdate.Enabled = false;
            DM.SaveMap();
        }

        private void btManual1_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);
            MM.SetManCycle((mc)iBtnTag);
        }
		
        private void btCylinder1_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);
            ML.CL_Move((ci)iBtnTag, ML.CL_GetCmd((ci)iBtnTag) == fb.Bwd ? fb.Fwd : fb.Bwd);
        }

        private void FormOperation_Shown(object sender, EventArgs e)
        {
            tmUpdate.Enabled = true;
        }
       
        private void FormOperation_VisibleChanged(object sender, EventArgs e)
        {
            if(this.Visible) tmUpdate.Enabled = true;
        }

        public void MakeDoubleBuffered(Control control, bool setting)
        {
            Type controlType = control.GetType();
            PropertyInfo pi = controlType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(control, setting, null);
        }

        private void btOperator_Click_1(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            //pnPassWord.Visible = true;
            //if (FrmLogOn.m_iCrntLogIn == (int)EN_LOGIN.LogOut)
            //{
            //    FrmLogOn.ShowPage();
            //}
            SM.FrmLogOn.Show();
        }

        private void btMan1_Click(object sender, EventArgs e)
        {
            Program.SendListMsg("Mansul");
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);

            MM.SetManCycle((mc)iBtnTag);
        }


        delegate void SendMsg(string _sMsg);
        public void SendListMsg(string _sMsg)
        {
            lvLog.Invoke(new SendMsg(ListMsg), new string[]{_sMsg});
        }
        private void ListMsg(string _sMsg)
        {
            if (!lvLog.GridLines)
            {
                lvLog.View = View.Details;
                lvLog.GridLines = true;
                lvLog.Columns.Add("Message", lvLog.Size.Width-5, HorizontalAlignment.Left);
                var PropError = lvLog.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
                PropError.SetValue(lvLog, true, null);

            }

            lvLog.Items.Add(_sMsg);
            if(lvLog.Items.Count > 100 ){
                lvLog.Items.RemoveAt(0);
            }
            lvLog.Items[lvLog.Items.Count - 1].EnsureVisible();
        }


        private void lvLog_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lvLog_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            lvLog.Items.Clear();
        }

        private void btManClick(object sender, EventArgs e)
        {
            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);
            MM.SetManCycle((mc)iBtnTag);
        }

        private void btBldHeat_Click(object sender, EventArgs e)
        {
            ML.IO_SetY(yi.ETC_HeaterOnBlade, !ML.IO_GetY(yi.ETC_HeaterOnBlade));
        }

        private void btStgHeat_Click(object sender, EventArgs e)
        {
            ML.IO_SetY(yi.ETC_HeaterOnStage, !ML.IO_GetY(yi.ETC_HeaterOnStage));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (Log.ShowMessageModal("Confirm", "작업 갯수 및 스탭 갯수를 초기화 하겠습니까 ?") != DialogResult.Yes) return;
            OM.EqpStat.iWorkCount = 0 ;
            OM.EqpStat.iWorkStep  = 0 ;
        }

        private void btLotEnd_Click_1(object sender, EventArgs e)
        {
            LOT.LotEnd();
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