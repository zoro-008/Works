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
using COMMON;
using SML;

namespace Machine.View
{
    /// <summary>
    /// OutputView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class OutputView : UserControl
    {
        public DispatcherTimer Timer = new DispatcherTimer();
        public yi   m_iYadd;
        public yi   m_iYHexadd;

        public OutputView()
        {
            InitializeComponent();
        }

        public void SetConfig(yi _iYadd, string _sTitle, Frame _frame)
        {
            string sInputName;
            sInputName = _sTitle;
            if (sInputName == "") return;
            //sInputName = sInputName.Substring(5, sInputName.Length - 5);

            m_iYadd = _iYadd;
            tbAdd.Text = "NO : " + ((int)m_iYadd).ToString();
            tbHexAdd.Text = string.Format("Y{0:X2}", (int)m_iYadd);
            tbTitle.Text = sInputName;
                      
            if(_frame != null) _frame.Content = this;

            //UI 타이머
            Timer.Interval = TimeSpan.FromMilliseconds(100);
            Timer.Tick += new EventHandler(Timer_Tick);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            bool bOut = ML.IO_GetY(m_iYadd, false);

            if (bOut)
            {
                btState.Background = Brushes.Red;
                btState.Content    = "ON";
            }
            else
            {
                btState.Background = Brushes.RosyBrown;
                btState.Content    = "OFF";
            }

            //throw new NotImplementedException();
        }

        private void BtState_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            bool b1 = Environment.Is64BitProcess;
            //TODO :: SM.DIO.SetY((int)m_iYadd, !SM.DIO.GetY((int)m_iYadd));
            //TODO :: //ML.IO_SetY(m_iYadd, !ML.IO_GetY(m_iYadd));
            //TODO :: 
            //TODO :: string sMsg;
            //TODO :: sMsg = btState.Content + " Button Click " + (ML.IO_GetY(m_iYadd) ? "(ON)" : "(OFF)").ToString();
            //TODO :: 
            //TODO :: Log.Trace(sMsg,ForContext.Frm);
        }

        private void BtState_Click(object sender, RoutedEventArgs e)
        {
            SM.DIO.SetY((int)m_iYadd, !SM.DIO.GetY((int)m_iYadd));
            //ML.IO_SetY(m_iYadd, !ML.IO_GetY(m_iYadd));
            
            string sMsg;
            sMsg = btState.Content + " Button Click " + (ML.IO_GetY(m_iYadd) ? "(ON)" : "(OFF)").ToString();
            
            Log.Trace(sMsg,ForContext.Frm);
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(IsVisible) Timer.Start();
            else          Timer.Stop ();
        }
    }
}
