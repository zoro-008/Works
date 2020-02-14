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

using static Machine.View.MotorMove;

namespace Machine.View
{
    /// <summary>
    /// MotorUnit.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MotorUnit : UserControl
    {
        private static double  dUnit;
        public  static double _dUnit { get => dUnit; set => dUnit = value; }

        private static EN_UNIT_TYPE  iUnitType;
        public  static EN_UNIT_TYPE _iUnitType { get => iUnitType; set => iUnitType = value; }

        public MotorUnit()
        {
            InitializeComponent();
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            //int iUnit = Convert.ToInt32(((RadioButton)sender).Tag);
            RadioButton_Changed();
        }

        private void RadioButton_Changed()//int _iUnit)
        {
            double dUserDefine = 0.0;
            if (!double.TryParse(tbUserUnit.Text, out dUserDefine)) dUserDefine = 0.0;
            
                 if(rbUnit0   .IsChecked == true) { iUnitType = EN_UNIT_TYPE.utJog ; dUnit = 0d         ; }
            else if(rbUnit1   .IsChecked == true) { iUnitType = EN_UNIT_TYPE.utMove; dUnit = 1d         ; }
            else if(rbUnit2   .IsChecked == true) { iUnitType = EN_UNIT_TYPE.utMove; dUnit = 0.5d       ; }
            else if(rbUnit3   .IsChecked == true) { iUnitType = EN_UNIT_TYPE.utMove; dUnit = 0.1d       ; }
            else if(rbUnit4   .IsChecked == true) { iUnitType = EN_UNIT_TYPE.utMove; dUnit = 0.05d      ; }
            else if(rbUserUnit.IsChecked == true) { iUnitType = EN_UNIT_TYPE.utMove; dUnit = dUserDefine; }

            //switch (_iUnit)
            //{
            //    default: iUnitType = EN_UNIT_TYPE.utJog ; dUnit = 0d         ; break;
            //    case 1 : iUnitType = EN_UNIT_TYPE.utMove; dUnit = 1d         ; break;
            //    case 2 : iUnitType = EN_UNIT_TYPE.utMove; dUnit = 0.5d       ; break;
            //    case 3 : iUnitType = EN_UNIT_TYPE.utMove; dUnit = 0.1d       ; break;
            //    case 4 : iUnitType = EN_UNIT_TYPE.utMove; dUnit = 0.05d      ; break;
            //    case 5 : iUnitType = EN_UNIT_TYPE.utMove; dUnit = dUserDefine; break;
            //}
        }

        private void TbUserUnit_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (rbUserUnit.IsChecked == false) return;
            
            double dUserDefine = 0.0;
            if (!double.TryParse(tbUserUnit.Text, out dUserDefine)) dUserDefine = 0.0;
            
            iUnitType = EN_UNIT_TYPE.utMove; 
            dUnit = dUserDefine;
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            RadioButton_Changed();
        }
    }
}
