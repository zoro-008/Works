using COMMON;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;
using SML;
using Machine.View.Operations;
using Machine.View.DataMan;

namespace Machine.View.Operations
{
    /// <summary>
    /// Operation.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Operation : UserControl
    {
        public DispatcherTimer Timer = new DispatcherTimer();

        private const string sFormText = "Form Operation ";
        //protected CDelayTimer m_tmStartBt;

        public Operation()
        {
            InitializeComponent();

            LODR.SetConfig(ri.LODR, "LOADER");//, OM.DevInfo.iColCount, OM.DevInfo.iRowCount);
            PLDR.SetConfig(ri.PLDR, "PART");//, OM.DevInfo.iColCount, OM.DevInfo.iRowCount);

            //Loader           
            LODR.SetDisplay(cs.None     , "None"     , Brushes.White        );
            LODR.SetDisplay(cs.Unknown  , "UnKnown"  , Brushes.Aqua         );
            LODR.SetDisplay(cs.Empty    , "Empty"    , Brushes.Silver       ); 
            LODR.SetDisplay(cs.WorkEnd  , "WorkEnd"  , Brushes.Blue         );

            //LODR.SetPopupVisible(cs.None , Visibility.Hidden);
            //LODR.SetPopupEnable(false);
            
            //Dressy Index 
            PLDR.SetDisplay(cs.None     , "None"     , Brushes.White        );
            PLDR.SetDisplay(cs.Empty    , "Empty"    , Brushes.Silver       );
            PLDR.SetDisplay(cs.Barcode  , "Barcode"  , Brushes.Tan          );
            PLDR.SetDisplay(cs.Unknown  , "UnKnown"  , Brushes.Aqua         );
            PLDR.SetDisplay(cs.Work     , "Work   "  , Brushes.Lime         );
            PLDR.SetDisplay(cs.WorkEnd  , "WorkEnd"  , Brushes.Blue         );
            
            DM.LoadMap();
            DM.LoadData();
            //tbUpSide.TabPages.Remove(tbUpside1);//Chart 탭 제거
            
            //tbAmp .Text = OM.EqpStat.dAmp .ToString();
            //tbVolt.Text = OM.EqpStat.iVolt.ToString();
            //tbTime.Text = OM.EqpStat.dTime.ToString();

            DataContext = this;

            //UI 타이머
            Timer.Interval = TimeSpan.FromMilliseconds(100);
            Timer.Tick += new EventHandler(Timer_Tick);


        }

        private void PanelRefresh()
        {
            LODR.InvalidateVisual();
            PLDR.InvalidateVisual();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;

            //로그인/로그아웃 방식
            if (SM.FrmLogOn.GetLevel() == (int)EN_LEVEL.LogOff)
            {
                icLogIn.Kind = MaterialDesignThemes.Wpf.PackIconKind.Login;
                tbLogIn.Text = "  LOG IN";
            }
            else
            {
                icLogIn.Kind = MaterialDesignThemes.Wpf.PackIconKind.Contact;
                tbLogIn.Text = "  " + SM.FrmLogOn.GetLevel().ToString();
            }

            //lvLotInfo.Items.Clear();
            //SPC.LOT.DispLotInfo(lvLotInfo);
            //SPC.DAY.DispDayInfo(lvLotInfo);

            if(!ML.MT_GetHomeDoneAll()){
                btHome.Foreground = SEQ._bFlick ? Brushes.White : Brushes.Red;
                tbHome.Foreground = SEQ._bFlick ? Brushes.White : Brushes.Red;
            }
            else {
                btHome.Foreground = Brushes.White  ;
                tbHome.Foreground = Brushes.White  ;
            }

            if (LOT.GetLotOpen() && !SEQ._bRun)
            {
                tbLotOpen.Text = "WORK ING";
                //btLotOpen.Enabled = true;
                btLotEnd.IsEnabled = true ;
            }
            else
            {
                tbLotOpen.Text = "WORK START";
                //btLotOpen.Enabled = true ;
                btLotEnd.IsEnabled = false;
            }

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //ov = new OutputView();
            //ov.SetConfig(yi.ETC_058,yi.ETC_058.ToString(), frame);

            //mv = new MotorView();
            //mv.SetConfig(mi.LODR_XPckr,SML.MOTION_DIR.CwCcw,frame_Copy);

            //iv = new InputView();
            //iv.SetConfig(xi.ETC_91,xi.ETC_DoorRt.ToString(), frame_Copy1);

            //cylinderview = new CylinderView();
            //cylinder.SetConfig(ci.TBLE_Grpr1FwBw,"TEST cylinderview",ML.CL_GetDirType((int)ci.TBLE_Grpr1FwBw), null);

            //cv = new CylinderView();
            //cv.SetConfig(ci.LODR_GrpRtrCwCCw,ML.CL_GetName(ci.LODR_GrpRtrCwCCw),ML.CL_GetDirType(ci.LODR_GrpRtrCwCCw), frame_Copy2);

            //mpv

            PM.Init();
            //PM.Para[(int)mi.TBL_XMove].Add(new PositionPara() { 
            //    sName     = "aswedf" ,
            //    uPstnNo   = 0 ,
            //    dPos      = 123.1  ,
            //    dMin      = 12.1   ,
            //    dMax      = 12.2   ,
            //    iSpeed    = 24     ,
            //    bGo       = false  ,
            //    bInput    = false  ,
            //    bCommon   = false
            //});
            //PM.Para[(int)mi.TBL_YMove].Add(new PositionPara() { 
            //    sName     = "as213df" ,
            //    uPstnNo   = 1 ,
            //    dPos      = 123.1  ,
            //    dMin      = 12.1   ,
            //    dMax      = 12.2   ,
            //    iSpeed    = 24     ,
            //    bGo       = false  ,
            //    bInput    = false  ,
            //    bCommon   = false
            //});
            //PM.Save("NONE");
            //PM.UpdatePstn(true);
            //mpv.SetConfig(0,frame_Copy3); //SetConfig(0);
            //frame_Copy3.Content = mpv;
            
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(IsVisible) Timer.Start();
            else          Timer.Stop ();
        }

        private void BtLotOpen_Click(object sender, RoutedEventArgs e)
        {
            new LotOpen().Show();
        }

        private void BtLotEnd_Click(object sender, RoutedEventArgs e)
        {
            if (SEQ._iSeqStat == EN_SEQ_STAT.Running)
            {
                Log.ShowMessage("Confirm", "Machine is running.");
                return;
            }

            LOT.LotEnd();
        }

        private void BtVersion_Click(object sender, RoutedEventArgs e)
        {
            new Version().Show();
        }
        
        private void BtStart_Click(object sender, RoutedEventArgs e)
        {
            Log.TraceListView(sFormText + "Start Button Clicked", ForContext.Frm);

            SEQ._bBtnStart = true;
        }

        private void BtStop_Click(object sender, RoutedEventArgs e)
        {
            Log.TraceListView(sFormText + "Stop Button Clicked", ForContext.Frm);

            SEQ._bBtnStop = true;
        }

        private void BtReset_Click(object sender, RoutedEventArgs e)
        {
            Log.TraceListView(sFormText + "Reset Button Clicked", ForContext.Frm);

            SEQ._bBtnReset = true;
        }

        private void BtHome_Click(object sender, RoutedEventArgs e)
        {
            Log.TraceListView(sFormText + "All Home Button Clicked", ForContext.Frm);

            if (!OM.MstOptn.bDebugMode)
            {
                if (!Log.ShowMessageModal("Confirm", "Do you want to All Homming?")) return;
                MM.SetManCycle(mc.AllHome);
            }
            else
            {
                if (Log.ShowMessageModal("Confirm", "홈동작을 생략 하겠습니까?"))
                {
                    ML.MT_SetServoAll(true);
                    System.Threading.Thread.Sleep(100);
                    for (mi i = 0; i < mi.MAX_MOTR; i++)
                    {
                        ML.MT_SetHomeDone(i, true);
                    }
                }
                else
                {
                    MM.SetManCycle(mc.AllHome);
                }
            }
        }

        private void BtLogIn_Click(object sender, RoutedEventArgs e)
        {
            SM.FrmLogOn.Show();
        }

        private void UserControl_ContextMenuClosing(object sender, ContextMenuEventArgs e)
        {
            Timer.Stop();
            DM.SaveMap();
            DM.SaveData();
        }
    }
}
