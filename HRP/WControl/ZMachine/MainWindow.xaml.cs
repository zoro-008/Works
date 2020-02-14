using COMMON;
using SML;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Machine.View.DataMan;

namespace Machine
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public DispatcherTimer Timer = new DispatcherTimer();
        //View.Operation operation = new View.Operation();
        //View.Device    device    = new View.Device   ();
        //View.Option    optione   = new View.Option   ();
        //View.Spc       spc       = new View.Spc      ();
        //View.DeviceSet deviceSet = new View.DeviceSet();
        //View.Master    master    = new View.Master   ();
        View.Operations.Operation operation ;
        View.Device     device    ;
        View.Option     optione   ;
        View.Spc        spc       ;
        View.DeviceSet  deviceSet ;
        View.Master     master    ;
        View.Version    version   ;
        SML.MainWindow  SMDLL;

        public MainWindow()
        {
            InitializeComponent();

            SEQ.Init();

            operation = new View.Operations.Operation();
            device    = new View.Device   ();
            optione   = new View.Option   ();
            spc       = new View.Spc      ();
            deviceSet = new View.DeviceSet();
            master    = new View.Master   ();
            version   = new View.Version  ();

            //UI 타이머
            Timer.Interval = TimeSpan.FromMilliseconds(100);
            Timer.Tick += new EventHandler(Timer_Tick);
            //Timer.Start();

            //파일 버전 보여주는 부분
            var fvi = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            var fv  = fvi.FileVersion; // or fvi.ProductVersion
            tbVersion.Text = "Version " + fv;

            SMDLL = new SML.MainWindow();

            Frame.Content = operation;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            tbDate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            tbThreadTime.Text = string.Format("{0:0.000ms}", SEQ._dMainThreadCycleTime);
            if (Environment.Is64BitOperatingSystem) textBlock.Text = "64bit";
            else                                    textBlock.Text = "32bit";  

            //Set Sequence State.
            string sStatText = "";
            SolidColorBrush StatColor = Brushes.Black;

            switch (SEQ._iSeqStat) {
                default: break;
                case EN_SEQ_STAT.Init   : sStatText = "INIT"    ; StatColor = Brushes.Blue;                              break;
                case EN_SEQ_STAT.Warning: sStatText = "WARNING" ; StatColor = Brushes.Gold;                              break;
                case EN_SEQ_STAT.Error  : sStatText = "ERROR"   ; StatColor = SEQ._bFlick ? Brushes.Gold : Brushes.Red;  break;
                case EN_SEQ_STAT.Running: sStatText = "RUNNING" ; StatColor = Brushes.Lime;                              break;
                case EN_SEQ_STAT.Stop   : sStatText = "STOP"    ; StatColor = Brushes.Gray;                              break;
                case EN_SEQ_STAT.RunWarn: sStatText = "RUNWARN" ; StatColor = SEQ._bFlick ? Brushes.Gold : Brushes.Lime; break;
                case EN_SEQ_STAT.WorkEnd: sStatText = "LOTEND"  ; StatColor = Brushes.DarkGray;                          break;
                case EN_SEQ_STAT.Manual : sStatText = "MANUAL"  ; StatColor = Brushes.Blue;                              break;
                case EN_SEQ_STAT.ToStart: sStatText = "STARTING"; StatColor = SEQ._bFlick ? Brushes.Gold : Brushes.Lime; break;
                case EN_SEQ_STAT.ToStop : sStatText = "STOPING" ; StatColor = SEQ._bFlick ? Brushes.Gold : Brushes.Lime; break;
            }
            //if (MM.GetManSetting()) sStatText = "MANUAL"; StatColor = Color.Lime; 

            if (OM.MstOptn.bDebugMode) {
                sStatText = "DEBUG " + sStatText;
            }
            if (OM.MstOptn.bIdleRun) {
                sStatText = "IDLE " + sStatText;
            }

            tbStat.Text       = sStatText;
            tbStat.Foreground = StatColor;
            //throw new NotImplementedException();
        }

        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = sender as ListView;
            var idx  = item.SelectedIndex;
                 if(idx == 0) Frame.Content = operation;
            else if(idx == 1) Frame.Content = device   ; 
            else if(idx == 2) Frame.Content = deviceSet; 
            else if(idx == 3) Frame.Content = optione  ;
            else if(idx == 4) Frame.Content = spc      ;
            else 
            {
                 //Frame.Content = device;
            }
            //ItemHome.
            //{Frame.Content = SM.GetDllMainWin(); SM.ShowForm(); }
        }

        //이미지에 마우스 더블클릭 이벤트가 없어서 이렇게 쓴다.
        private void Image_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {

            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch { }
        }

        #region PopUpMenu
        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            Log.TraceListView("MainWindow Menu Open Button Clicked", ForContext.Frm);
            ButtonOpenMenu.Visibility  = Visibility.Collapsed;
            ButtonCloseMenu.Visibility = Visibility.Visible  ;
        }

        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            Log.TraceListView("MainWindow Menu Close Button Clicked", ForContext.Frm);
            ButtonOpenMenu.Visibility  = Visibility.Visible  ;
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
        }

        private void btMinimum_Click(object sender, RoutedEventArgs e)
        {
            Log.TraceListView("MainWindow Minimum Button Clicked", ForContext.Frm);
            WindowState = WindowState.Minimized;
        }

        private void btVersion_Click(object sender, RoutedEventArgs e)
        {
            Log.TraceListView("MainWindow Version Button Clicked", ForContext.Frm);
            if(!version.IsLoaded) version = new View.Version();
            version.Show();
        }

        private void btSetting_Click(object sender, RoutedEventArgs e)
        {
            Log.TraceListView("MainWindow Setting Button Clicked", ForContext.Frm);
            SMDLL.Show();
        }

        private void btDebug_Click(object sender, RoutedEventArgs e)
        {
            Log.TraceListView("MainWindow Debug Button Clicked", ForContext.Frm);

            if(!master.IsLoaded) master = new View.Master();
            master.Show();
        }

        private void btHelp_Click(object sender, RoutedEventArgs e)
        {
            Log.TraceListView("MainWindow Help Button Clicked", ForContext.Frm);
        }

        private void ButtonPopUpLogout_Click(object sender, RoutedEventArgs e)
        {
            //Common.View.YesNo yesno = new Common.View.YesNo();
            //yesno.ShowForm("Notice","Do you want to Exit ?");

            //Common.View.Ok ok = new Common.View.Ok();
            //ok.ShowForm("Notice","Do you want to Exit ?");

            if(!Log.ShowMessageModal("Notice","Do you want to Exit ?"))return ;
            //Log.ShowMessage("Notice","Do you want to Exit ?");
            //return;
            DM.SaveMap();
            DM.SaveData();
            SEQ.Close();

            Application.Current.Shutdown();
        }
        #endregion


    }
}
