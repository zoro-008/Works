using System;
using System.Collections.Generic;
using System.Linq;
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
using System.ComponentModel;
using SML;
using COMMON;

namespace Machine.View.Operations
{
    /// <summary>
    /// DayInfo.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DayInfo : UserControl
    {
        public DispatcherTimer Timer = new DispatcherTimer();
        //bool bAppStart = false;

        public DayInfo()
        {
            InitializeComponent();

            //bAppStart = true;
            
            //UI 타이머
            Timer.Interval = TimeSpan.FromMilliseconds(1000);
            Timer.Tick += new EventHandler(Timer_Tick);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            //if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            tbData0.Text = TimeSpan.FromMilliseconds(SPC.DAY.Data.RunTime    ).ToString(@"hh\:mm\:ss");//DateTime.FromOADate(SPC.DAY.Data.RunTime    ).ToString("HH:mm:ss");
            tbData1.Text = TimeSpan.FromMilliseconds(SPC.DAY.Data.DownTime   ).ToString(@"hh\:mm\:ss");//DateTime.FromOADate(SPC.DAY.Data.IdleTime   ).ToString("HH:mm:ss");
            tbData2.Text = TimeSpan.FromMilliseconds(SPC.DAY.Data.IdleTime   ).ToString(@"hh\:mm\:ss");//DateTime.FromOADate(SPC.DAY.Data.FailureTime).ToString("HH:mm:ss");
            tbData3.Text = TimeSpan.FromMilliseconds(SPC.DAY.Data.FailureTime).ToString(@"hh\:mm\:ss");//DateTime.FromOADate(SPC.DAY.Data.DownTime   ).ToString("HH:mm:ss");

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            /*
            if (bAppStart)
            {
                bAppStart = false;
                //폰트 비율 계산 적용
                //UserControl 형태로 붙일때 FontSize는 지정된 상태로 붙어서
                //UserControl이 축소/확대 될때 Font는 그대로 있어 비율 계산해서 곱해줌. 진섭
                double dActualWidth  = this.ActualWidth ;
                double dActualHeight = this.ActualHeight;
                double dDesignWidth  = 400;
                double dDesignHeight = 450;
                double dAreaRatio = (dActualHeight * dActualWidth) / (dDesignHeight * dDesignWidth);
                double dTemp = tbName0.FontSize;
                
                tbName0.FontSize *= dAreaRatio;
                tbData0.FontSize *= dAreaRatio;
                tbName1.FontSize *= dAreaRatio;
                tbData1.FontSize *= dAreaRatio;
                tbName2.FontSize *= dAreaRatio;
                tbData2.FontSize *= dAreaRatio;
                tbName3.FontSize *= dAreaRatio;
                tbData3.FontSize *= dAreaRatio;
            }
            */
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(IsVisible) Timer.Start();
            else          Timer.Stop ();
        }
    }
}
