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

namespace Common.View
{
    /// <summary>
    /// UserControl1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Ok : Window
    {
        public DispatcherTimer Timer = new DispatcherTimer();

        public Ok()
        {
            InitializeComponent();

            //UI 타이머
            Timer.Interval = TimeSpan.FromMilliseconds(100);
            Timer.Tick += new EventHandler(Timer_Tick);
            Timer.Start();
        }

        private void BtYes_Click(object sender, RoutedEventArgs e)
        {
            //DialogResult = true;
            Hide();
        }

        public void ShowForm(string _sTitle, string _sContens , int _iTime = 0)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                {
                    lbTitle.Text    = _sTitle;
                    lbContents.Text = _sContens;
                    Show();
                    
                }));
            }
            else
            {
                lbTitle.Text    = _sTitle;
                lbContents.Text = _sContens;
                if (_iTime != 0)
                {
                    Timer.Interval = TimeSpan.FromMilliseconds(_iTime);
                    Timer.Start();
                }
                Show();
            }
        }

        public void HideForm()
        {
            if (!this.Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                {
                    Hide();
                }));
            }
            else
            {
                Hide();
            }
        }

        public void CloseForm()
        {
            HideForm();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            HideForm();
            Timer.Stop();
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //if(IsVisible) Timer.Start();
            //else          Timer.Stop ();
        }
    }
}
