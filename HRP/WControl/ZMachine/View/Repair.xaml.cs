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
using SML;
using COMMON;

namespace Machine.View
{
    /// <summary>
    /// Repair.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Repair : Window
    {
        public Repair()
        {
            InitializeComponent();
        }

        private void BtApply_Click(object sender, RoutedEventArgs e)
        {
            if (cbUnderRepair.IsChecked == true)
            {
                SPC.FLR.Data.EngineerID = SM.FrmLogOn.GetId(); //나중에 진섭이 머지 할때 바꾸자.
                SPC.FLR.Data.Purpose = tbPurpose.Text;
            }

            OM.EqpStat.bMaint = cbUnderRepair.IsChecked == true;
        }

        private void BtClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbUnderRepair.IsChecked = OM.EqpStat.bMaint;
        }
    }
}
