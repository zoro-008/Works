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
    public partial class YesNo : Window
    {
        public YesNo()
        {
            InitializeComponent();
        }

        private void BtYes_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true ;
        }

        private void BtNo_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        public void ShowForm(string _sTitle, string _sContens = "")
        {
            if (!this.Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                {
                	lbTitle   .Text    = _sTitle  ;
                	lbContents.Text    = "THIS WINDOW IS NEED TO UI THREAD";
                    //ShowDialog();
                    Show();
                    
                }));
                //return System.Windows.Forms.DialogResult.Abort;
            }
            else
            {
                lbTitle   .Text    = _sTitle  ;
                lbContents.Text    = _sContens;
                ShowDialog();
                //bool ? bRst = ShowDialog().Value;
                //if(bRst == true) return System.Windows.Forms.DialogResult.Yes; 
                //else             return System.Windows.Forms.DialogResult.No ; 
            }
        }

        /*
public string ResponseText {
get { return ResponseTextBox.Text; }
set { ResponseTextBox.Text = value; }
}


private void OKButton_Click(object sender, RoutedEventArgs e)
{
DialogResult = true;
}

//Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
//	{
//		//사용할 메서드 및 동작
//		ScrollGraph_001.AddStep(LastData);
//	}));
//InputDialog();
var dialog = new Window1();
if (dialog.ShowDialog() == true) {
MessageBox.Show("You said: " + dialog.ResponseText);

DialogCoordinator.Instance.ShowInputAsync(this, "MahApps Dialog", "Using Material Design Themes", metroDialogSettings);

*/


    }
}
