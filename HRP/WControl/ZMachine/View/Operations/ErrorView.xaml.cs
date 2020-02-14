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
using SML;
using COMMON;

namespace Machine.View.Operations
{
    /// <summary>
    /// Error.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ErrorView : UserControl
    {
        public DispatcherTimer Timer = new DispatcherTimer();

        private int iPreErrCnt = 0;
        private bool bLoad = false;

        public ErrorView()
        {
            InitializeComponent();

            //bLoad = true;

            //UI 타이머
            Timer.Interval = TimeSpan.FromMilliseconds(100);
            Timer.Tick += new EventHandler(Timer_Tick);
            //Timer.Stop();
        }
        ~ErrorView()
        {
            bLoad = false;
            //Timer.Stop();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            //if(!bLoad) return;
            //if(!Timer.IsEnabled) return;
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
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
                        //liError = new ListViewItem(sErr);
                        //liError = new ListViewItem();
                        //liError.
                        lvError.Items.Add(sErr);
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
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(IsVisible) Timer.Start();
            else           Timer.Stop();

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //bLoad = true;
        }
    }
}
