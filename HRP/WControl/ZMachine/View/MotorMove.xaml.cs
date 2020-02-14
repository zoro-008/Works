//using COMMON;
//using SML;
using COMMON;
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
    public enum EN_UNIT_TYPE
    {
        utJog = 0,
        utMove,
        utPitch,

        MAX_UNIT_TYPE
    };
    /// <summary>
    /// MotorMove.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MotorMove : UserControl
    {
        public DispatcherTimer Timer = new DispatcherTimer();

        public struct TPara
        {
            public double dUnit;
            public int iUnitType;
            public double dPitch;
            
        }

        public enum EN_UNIT_TYPE
        {
            utJog = 0,
            utMove,
            utPitch,

            MAX_UNIT_TYPE
        };

        public mi m_eId = 0;
        public int m_iType;
        public TPara Para;

        private bool bLoad = false;

        public MotorMove()
        {
            InitializeComponent();

            //UI 타이머
            Timer.Interval = TimeSpan.FromMilliseconds(100);
            Timer.Tick += new EventHandler(Timer_Tick);

        }

        
        
        private void Timer_Tick(object sender, EventArgs e)
        {
            if(!bLoad) return;
            //BdStat1.Background = true  ? Brushes.Lime : Brushes.Silver;
            BdStat1.Background = ML.MT_GetNLimSnsr  (m_eId) ? Brushes.Lime : Brushes.Silver;
            BdStat2.Background = ML.MT_GetHomeSnsr  (m_eId) ? Brushes.Lime : Brushes.Silver;
            BdStat3.Background = ML.MT_GetPLimSnsr  (m_eId) ? Brushes.Lime : Brushes.Silver;
            BdStat4.Background = ML.MT_GetAlarmSgnl (m_eId) ? Brushes.Lime : Brushes.Silver;
            BdStat5.Background = ML.MT_GetServo     (m_eId) ? Brushes.Lime : Brushes.Silver;
            BdStat6.Background = ML.MT_GetStop      (m_eId) ? Brushes.Lime : Brushes.Silver;
            BdStat7.Background = ML.MT_GetHomeDone  (m_eId) ? Brushes.Lime : Brushes.Silver;
            
            LbCmdPos.Text = ML.MT_GetCmdPos(m_eId).ToString();
            LbEncPos.Text = ML.MT_GetEncPos(m_eId).ToString();
            
            if (SEQ._bRun)
            {
                BtNeg.IsEnabled = false;
                BtPos.IsEnabled = false;
            }
            else
            {
                BtNeg.IsEnabled = true;
                BtPos.IsEnabled = true;
            }
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                //Timer.Start();
                if(IsVisible) Timer.Start();
                //else          Timer.Stop ();
            }
            else
            {
                Timer.Stop ();
            }

        }

        //public void SetIdType(mi _eId, MOTION_DIR _iType)
        public void SetConfig(mi _eId, SML.MOTION_DIR _iType, Frame _frame)
        {
            m_eId = _eId;
            m_iType = (int)_iType;
            tbMotorName.Header  = ML.MT_GetName(_eId);
            //tbMotorName.Text = _eId.ToString();
            bLoad = true;

            if (_iType == SML.MOTION_DIR.LeftRight)
            {
                BtNegIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowLeftBold ;
                BtPosIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowRightBold;
                
                
                //btNeg.Text = "LEFT(-)";
                //btPos.Text = "RIGHT(+)";
            }

            if (_iType == SML.MOTION_DIR.RightLeft)
            {
                BtNegIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowRightBold;
                BtPosIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowLeftBold ;
                //BtNeg.Image = global::Machine.Properties.Resources.RIGHT;
                //BtPos.Image = global::Machine.Properties.Resources.LEFT ;
                //BtNeg.Text = "RIGHT(-)";
                //BtPos.Text = "LEFT(+)";
            }

            if (_iType == SML.MOTION_DIR.BwdFwd)
            {
                BtNegIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowDownBold;
                BtPosIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowUpBold  ;
                //btNeg.Image = global::Machine.Properties.Resources.DN;
                //btPos.Image = global::Machine.Properties.Resources.UP;
                //btNeg.Text = "BWD(-)";
                //btPos.Text = "FWD(+)";
            }

            if (_iType == SML.MOTION_DIR.FwdBwd)
            {
                BtNegIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowUpBold  ;
                BtPosIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowDownBold;
                //btNeg.Image = global::Machine.Properties.Resources.UP;
                //btPos.Image = global::Machine.Properties.Resources.DN;
                //btNeg.Text = "FWD(-)";
                //btPos.Text = "BWD(+)";
            }

            if (_iType == SML.MOTION_DIR.DownUp)
            {
                BtNegIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowDownBold;
                BtPosIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowUpBold  ;
                //btNeg.Image = global::Machine.Properties.Resources.DN;
                //btPos.Image = global::Machine.Properties.Resources.UP;
                //btNeg.Text = "DN(-)";
                //btPos.Text = "UP(+)";
            }

            if (_iType == SML.MOTION_DIR.UpDown)
            {
                BtNegIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowUpBold;
                BtPosIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowDownBold  ;
                //btNeg.Image = global::Machine.Properties.Resources.UP;
                //btPos.Image = global::Machine.Properties.Resources.DN;
                //btNeg.Text = "UP(-)";
                //btPos.Text = "DN(+)";
            }

            if (_iType == SML.MOTION_DIR.CcwCw)
            {
                BtNegIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowRotateLeft ;
                BtPosIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowRotateRight;
                //btNeg.Image = global::Machine.Properties.Resources.CCW;
                //btPos.Image = global::Machine.Properties.Resources.CW;
                //btNeg.Text = "CCW(-)";
                //btPos.Text = "CW(+)";
            }

            if (_iType == SML.MOTION_DIR.CwCcw)
            {
                BtNegIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowRotateRight;
                BtPosIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowRotateLeft ;
                //btNeg.Image = global::Machine.Properties.Resources.CW;
                //btPos.Image = global::Machine.Properties.Resources.CCW;
                //btNeg.Text = "CW(-)";
                //btPos.Text = "CCW(+)";
            }

            if(_frame != null) _frame.Content = this;
            //tmUpdate.Enabled = true;
        }

        public void SetPitch(double _dUnit)
        {
            Para.dPitch = _dUnit;
        }

        public void SetUnit(EN_UNIT_TYPE _iUnitType, double _dUnit)
        {
            Para.dUnit = _dUnit;
            Para.iUnitType = (int)_iUnitType;
        }

        private void BtNeg_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Para.iUnitType == (int)EN_UNIT_TYPE.utJog) ML.MT_Stop(m_eId);
        }

        private void BtNeg_MouseDown(object sender, MouseButtonEventArgs e)
        {

            string sTemp;
            string sText = ((Button)sender).Name;
            bool bRet = true;
            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);
            uint uPstnNo = (uint)iBtnTag;
            //if (!CheckSafe(m_iMotrNo)) return;
            if (!OM.MstOptn.bDebugMode)
            {
                //if (m_eId == mi.HEAD_XVisn) bRet = SEQ.VSNZ.JogCheckSafe((mi)m_eId, EN_JOG_DIRECTION.Neg, (EN_UNIT_TYPE)Para.iUnitType, Para.dUnit);
                //if (m_eId == mi.HEAD_XVisn) bRet = SEQ.VSNZ.CheckSafe((mi)m_eId, (pv)uPstnNo);
            }

            if (!bRet) return;

            //SetUnit(EN_UNIT_TYPE _iUnitType, double _dUnit)
            //기존에 DeviceSet에서 하던거 일루 옮겨봄
            Para.dUnit     = MotorUnit. _dUnit;
            Para.iUnitType = (int)MotorUnit._iUnitType;

            ML.MT_Stop(m_eId);

            sTemp = m_eId.ToString();
            
            if (SEQ._iSeqStat == SML.EN_SEQ_STAT.Manual)
            {
                Log.ShowMessage("ERROR", "Doing Manual Cycle"); 
                return;
            }
            else
            {
                Log.TraceListView(this.GetType().Name + " " + sTemp + " " + sText + " Button Clicked",ForContext.Frm);

                if (CbSlow.IsChecked == false)
                {
                         if (Para.iUnitType == (int)EN_UNIT_TYPE.utJog  ) ML.MT_JogN    (m_eId);
                    else if (Para.iUnitType == (int)EN_UNIT_TYPE.utMove ) ML.MT_GoIncMan(m_eId, -(Para.dUnit));
                    else if (Para.iUnitType == (int)EN_UNIT_TYPE.utPitch) ML.MT_GoIncMan(m_eId, -(Para.dUnit));
                    else return;
                }
                else
                {
                         if (Para.iUnitType == (int)EN_UNIT_TYPE.utJog  ) ML.MT_JogN     (m_eId);
                    else if (Para.iUnitType == (int)EN_UNIT_TYPE.utMove ) ML.MT_GoIncSlow(m_eId, -(Para.dUnit));
                    else if (Para.iUnitType == (int)EN_UNIT_TYPE.utPitch) ML.MT_GoIncSlow(m_eId, -(Para.dUnit));
                    else return;
                }
            }
        }

        private void BtStop_Click(object sender, RoutedEventArgs e)
        {
            Log.TraceListView(this.GetType().Name + " " + m_eId.ToString() + " Stop Button Clicked", ForContext.Frm);
            ML.MT_Stop(m_eId);
        }

        private void BtPos_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Para.iUnitType == (int)EN_UNIT_TYPE.utJog) ML.MT_Stop(m_eId);
        }

        private void BtPos_MouseDown(object sender, MouseButtonEventArgs e)
        {

            string sTemp;
            string sText = ((Button)sender).Name;
            bool bRet = true;
            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);
            uint uPstnNo = (uint)iBtnTag;
            //if (!CheckSafe(m_iMotrNo)) return;

            if (!OM.MstOptn.bDebugMode)
            {
                //if (m_eId == mi.HEAD_XVisn) bRet = SEQ.VSNZ.JogCheckSafe((mi)m_eId, EN_JOG_DIRECTION.Pos, (EN_UNIT_TYPE)Para.iUnitType, Para.dUnit);
                //if (m_eId == mi.HEAD_XVisn) bRet = SEQ.VSNZ.CheckSafe((mi)m_eId, (pv)uPstnNo);
            }

            if (!bRet) return;

            //SetUnit(EN_UNIT_TYPE _iUnitType, double _dUnit)
            //기존에 DeviceSet에서 하던거 일루 옮겨봄
            Para.dUnit     = MotorUnit. _dUnit;
            Para.iUnitType = (int)MotorUnit._iUnitType;

            ML.MT_Stop(m_eId);

            sTemp = m_eId.ToString();

            if (SEQ._iSeqStat == SML.EN_SEQ_STAT.Manual)
            {
                Log.ShowMessage("ERROR", "Doing Manual Cycle"); 
                return;
            }
            else
            {
                Log.TraceListView(this.GetType().Name + " " + sTemp + " " + sText + " Button Clicked",ForContext.Frm);

                if (CbSlow.IsChecked == false)
                {
                         if (Para.iUnitType == (int)EN_UNIT_TYPE.utJog  ) ML.MT_JogP    (m_eId);
                    else if (Para.iUnitType == (int)EN_UNIT_TYPE.utMove ) ML.MT_GoIncMan(m_eId, Para.dUnit);
                    else if (Para.iUnitType == (int)EN_UNIT_TYPE.utPitch) ML.MT_GoIncMan(m_eId, Para.dUnit);
                    else return;
                }
                else
                {
                         if (Para.iUnitType == (int)EN_UNIT_TYPE.utJog  ) ML.MT_JogP     (m_eId);
                    else if (Para.iUnitType == (int)EN_UNIT_TYPE.utMove ) ML.MT_GoIncSlow(m_eId, Para.dUnit);
                    else if (Para.iUnitType == (int)EN_UNIT_TYPE.utPitch) ML.MT_GoIncSlow(m_eId, Para.dUnit);
                    else return;
                }
            }
        }
        private void CbSlow_Click(object sender, RoutedEventArgs e)
        {
            if (CbSlow.IsChecked == true)
            {
                Log.ShowMessageFunc(string.Format("Relative movement value is decelerated to 10%.", m_eId));
            }
            else
            {
                Log.ShowMessageFunc(string.Format("The relative shift value changes to Manual Speed.", m_eId));
            }
        }

    }
}
