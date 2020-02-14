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
using System.Windows.Shapes;
using System.Windows.Threading;
using COMMON;

namespace Machine.View.Operations
{
    public partial class LogView : UserControl
    {
        //Dispatcher dispatcher = new Dispatcher()
        public LogView()
        {
            InitializeComponent();

            Log.SetSendMsgCallback(ListMsg);

        }
        //~LogViewer()
        //{
        //
        //}

        
        private void ListMsg(string _sMsg)
        {
            string sCrntTime = DateTime.Now.ToString("hh:mm:ss.fff-");
            if(Dispatcher.CurrentDispatcher != Application.Current.Dispatcher)
            {
                //비동기 방식 lvLogView.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
                lvLogView.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                {
                    lvLogView.Items.Add(sCrntTime+_sMsg);
                    if (lvLogView.Items.Count > 200)
                    {
                        lvLogView.Items.RemoveAt(0);
                    }
        
                    lvLogView.SelectedIndex = lvLogView.Items.Count - 1;
                    lvLogView.ScrollIntoView(lvLogView.SelectedItem);
        
                }));
            }
            else
            {
                lvLogView.Items.Add(sCrntTime+_sMsg);
                if (lvLogView.Items.Count > 200)
                {
                    lvLogView.Items.RemoveAt(0);
                }
                lvLogView.SelectedIndex = lvLogView.Items.Count - 1;
                lvLogView.ScrollIntoView(lvLogView.SelectedItem);
            }            
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
