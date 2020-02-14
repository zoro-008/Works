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

namespace Machine.View
{
    /// <summary>
    /// Input.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Input : UserControl
    {
        public DispatcherTimer Timer = new DispatcherTimer();
        public xi   m_iXadd   ;
        public xi   m_iXHexadd;
        
        private bool bLoad = false;

        public Input()
        {
            InitializeComponent();
        }

        public void SetConfig(xi _iXadd, string _sTitle, Frame _frame)
        {
            string sInputName;
            sInputName = _sTitle;
            if (sInputName == "") return;
            //sInputName = sInputName.Substring(5, sInputName.Length - 5);
            bLoad = true;

            m_iXadd = _iXadd;
            tbAdd.Text = "NO : " + ((int)m_iXadd).ToString();
            tbHexAdd.Text = string.Format("X{0:X2}", (int)m_iXadd);
            tbTitle.Text = sInputName;

            if(_frame != null) _frame.Content = this;

            //UI 타이머
            Timer.Interval = TimeSpan.FromMilliseconds(100);
            Timer.Tick += new EventHandler(Timer_Tick);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if(!bLoad) return;

            bool bIn = ML.IO_GetX(m_iXadd);

            if (bIn)
            {
                btState.Background = Brushes.Red;
                btState.Content    = "ON";
            }
            else
            {
                btState.Background = Brushes.RosyBrown;
                btState.Content    = "OFF";
            }

        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(IsVisible) Timer.Start();
            else          Timer.Stop ();
        }
    }
}
