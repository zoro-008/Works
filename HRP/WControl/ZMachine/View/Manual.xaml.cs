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
using System.ComponentModel;

namespace Machine.View
{
    /// <summary>
    /// OperationManual.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Manual : UserControl
    {
        
        public Manual()
        {
            InitializeComponent();

            btPart0_0.Tag = mc.LODR_Home;


        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void BtManual_Click(object sender, RoutedEventArgs e)
        {
            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);
            MM.SetManCycle((mc)iBtnTag);
        }
    }
}
