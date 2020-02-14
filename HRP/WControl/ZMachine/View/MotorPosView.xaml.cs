using COMMON;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Machine.View
{
    /// <summary>
    /// MotorPosView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MotorPosView : UserControl
    {
        public DispatcherTimer Timer = new DispatcherTimer();
        public int iAxisNo { get; set; } = -1;

        public MotorPosView()
        {
            InitializeComponent();
            
            //UI 타이머
            Timer.Interval = TimeSpan.FromMilliseconds(100);
            Timer.Tick += new EventHandler(Timer_Tick);

            //listView.ItemsSource = null;
            //listView.ItemsSource = PM.Display[0];
            
            //GridView gView = listView.View as GridView;
            //gView.Columns[0].Width = workingWidth*col1;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if(iAxisNo < 0) return;
            //tbDate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //throw new NotImplementedException();
            tbStat1.Background = ML.MT_GetNLimSnsr  (iAxisNo) ? Brushes.Red : Brushes.Silver;
            tbStat2.Background = ML.MT_GetHomeSnsr  (iAxisNo) ? Brushes.Red : Brushes.Silver;
            tbStat3.Background = ML.MT_GetPLimSnsr  (iAxisNo) ? Brushes.Red : Brushes.Silver;
            tbStat4.Background = ML.MT_GetHomeDone  (iAxisNo) ? Brushes.Red : Brushes.Silver;
            tbStat5.Background = ML.MT_GetAlarmSgnl (iAxisNo) ? Brushes.Red : Brushes.Silver;
            tbStat6.Background = ML.MT_GetStop      (iAxisNo) ? Brushes.Red : Brushes.Silver;
            tbStat7.Background = ML.MT_GetInPosSgnl (iAxisNo) ? Brushes.Red : Brushes.Silver;  
            tbStat8.Background = ML.MT_GetServo     (iAxisNo) ? Brushes.Red : Brushes.Silver;
            
            tbAxisPos.Text     = string.Format("{0:0.0000}", ML.MT_GetCmdPos(iAxisNo));
        }

        private void BtGo_Click(object sender, RoutedEventArgs e)
        {
            if(!ML.MT_GetStop(iAxisNo)) ML.MT_Stop(iAxisNo);

            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);
            int iPstnNo = iBtnTag;
            
            bool bRet = true;
            if (Log.ShowMessageModal("Confirm", "Do you want to Part Move?") != System.Windows.Forms.DialogResult.Yes) return;
            //if (MessageBox.Show(new Form{TopMost = true},"Part를 이동하시겠습니까?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

            //TODO :: if (CheckSafe != null)
            //TODO :: {
            //TODO ::     if(!CheckSafe((mi)m_uMotrId , (pv)uPstnNo)) bRet = false ;
            //TODO :: }
            //if(m_uMotrId == (int)mi.IDX_X) bRet = SEQ.IDX.CheckSafe((mi)m_uMotrId, uPstnNo);

            if (!bRet) return;
            ML.MT_GoAbsRun(iAxisNo, PM.Para[iAxisNo][iPstnNo].dPos, PM.Para[iAxisNo][iPstnNo].iSpeed );
        }

        private void BtInput_Click(object sender, RoutedEventArgs e)
        {
            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);
            int iPstnNo = iBtnTag;

            bool   bServo = ML.MT_GetServo(iAxisNo);
            
            double dPos, dInputPos;
            string sPos;

            if (bServo)
            {
                dPos      = ML.MT_GetCmdPos(iAxisNo);
                sPos      = dPos.ToString("N4");
                dInputPos = double.Parse(sPos);
            }
            else
            {
                dPos      = ML.MT_GetEncPos(iAxisNo);
                sPos      = dPos.ToString("N4");
                dInputPos = double.Parse(sPos);

            }

            Log.Trace("Form DeviceSet Input Button Clicked ("  + ML.MT_GetName(iAxisNo) + " " + PM.Para[iAxisNo][iPstnNo].sName + " " +
                       PM.Para[iAxisNo][iPstnNo].dPos + " -> " + sPos + ")", ForContext.Frm);

            //SetValue(uPstnNo , GetCmdPos((int)m_uMotrId));
            //SetValue(uPstnNo, dInputPos);            
            PM.Para[iAxisNo][iPstnNo].dPos = dInputPos;
        }

        public void SetConfig(uint _uAxisNo, Frame _frame)
        {
            iAxisNo = (int)_uAxisNo ;
            listView.ItemsSource = null;
            listView.ItemsSource = PM.Display[iAxisNo];

            if(_frame != null) _frame.Content = this;
            //uAxisNo = _uAxisNo ;
            //PM.Display[uAxisNo].Add(new PositionPara() { sName = _sName, uPstnNo = _uPstnNo, bGo = _bUseGo, bInput = _bUseInput, bCommon = _bCommon});
        }

        private void ListView_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateColumnsWidth(sender as ListView);
        }

        private void UpdateColumnsWidth(ListView listView)
        {
            //ListView listView = sender as ListView;
            GridView gView = listView.View as GridView;

            var workingWidth = listView.ActualWidth - SystemParameters.VerticalScrollBarWidth - 18; // take into account vertical scrollbar
            var col1 = 0.20;
            var col2 = 0.23;
            var col3 = 0.10;
            var col4 = 0.15;
            var col5 = 0.10;
            var col6 = 0.10;
            var col7 = 0.12;
            
            gView.Columns[0].Width = workingWidth*col1; 
            gView.Columns[1].Width = workingWidth*col2; 
            gView.Columns[2].Width = workingWidth*col3; 
            gView.Columns[3].Width = workingWidth*col4; 
            gView.Columns[4].Width = workingWidth*col5; 
            gView.Columns[5].Width = workingWidth*col6; 
            gView.Columns[6].Width = workingWidth*col7; 
            //gView.Columns[7].Width = workingWidth*col4;

        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(IsVisible) Timer.Start();
            else          Timer.Stop ();
        }

        private void BtServoOn_Click(object sender, RoutedEventArgs e)
        {
            string sText = ((Button)sender).Content.ToString();
            Log.Trace(((mi)iAxisNo).ToString() + " Axis " + sText + " Button Clicked", ForContext.Frm);
            ML.MT_SetServo(iAxisNo, true);
        }

        private void BtServoOff_Click(object sender, RoutedEventArgs e)
        {
            string sText = ((Button)sender).Content.ToString();
            Log.Trace(((mi)iAxisNo).ToString() + " Axis " + sText + " Button Clicked", ForContext.Frm);
            ML.MT_SetServo(iAxisNo, false);
        }

        private void BtHome_Click(object sender, RoutedEventArgs e)
        {
            string sText = ((Button)sender).Content.ToString();
            Log.Trace(((mi)iAxisNo).ToString() + " Axis " + sText + " Button Clicked", ForContext.Frm);

            if (Log.ShowMessageModal(((mi)iAxisNo).ToString() + " Axis" , "Do you want to Axis Homing?")!= System.Windows.Forms.DialogResult.Yes) return ;
            ML.MT_GoHome(iAxisNo);
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PM.iClickedAxis = (uint)iAxisNo ;
        }
    }

    public static class PM
    {
        public delegate bool dgCheckSafe(mi _eMotr, pv _ePstn ,  double _dOfsPos=0);
        public static dgCheckSafe[] CheckSafe;
        //WPF DISPLAY
        public static ObservableCollection<PositionPara>[] Display ;//= new List<PositionPara>();
        public static List                <PositionPara>[] Para    ;//= new List<PositionPara>();
        public static List                <PositionPara>[] Para1   ;//= new List<PositionPara>();

        public static uint iClickedAxis = 0;

        public static void Init()
        {
            CheckSafe = new dgCheckSafe                       [(int)mi.MAX_MOTR];
            Display   = new ObservableCollection<PositionPara>[(int)mi.MAX_MOTR];
            Para      = new List                <PositionPara>[(int)mi.MAX_MOTR];
            Para1     = new List                <PositionPara>[(int)mi.MAX_MOTR];
            for (int i = 0; i < (int)mi.MAX_MOTR; i++)
            {
                Display[i] = new ObservableCollection<PositionPara>();
                Para   [i] = new List                <PositionPara>();
                Para1  [i] = new List                <PositionPara>();
            }
        }

        public static void SetPara(uint _uAxisNo, uint _uPstnNo, string _sName, bool _bUseGo, bool _bUseInput, bool _bCommon)
        {
            Para[_uAxisNo].Add(new PositionPara() { sName = _sName, uPstnNo = _uPstnNo, bGo = _bUseGo, bInput = _bUseInput, bCommon = _bCommon});
        }

        public static void SetCheckSafe(uint _uAxisNo, dgCheckSafe _dgCheckSafe)
        {
            CheckSafe[_uAxisNo] = _dgCheckSafe;
        }
        //public static void SetGetCmdPos(uint _uAxisNo, dgGetCmdPos _dgGetCmdPos)
        //{
        //    MotrPstn[_uAxisNo].SetGetCmdPos(_dgGetCmdPos);
        //}

        public static double GetValue(uint _uAxisNo, uint _uPstnNo)
        {
            return Para[_uAxisNo][(int)_uPstnNo].dPos;
        }

        public static double GetValue(mi _eAxisNo, pv _ePstnNo)
        {
            return Para[(uint)_eAxisNo][(int)_ePstnNo].dPos;
        }

        public static int GetValueSpdPer(uint _uAxisNo, uint _uPstnNo)
        {
            return Para[(uint)_uAxisNo][(int)_uPstnNo].iSpeed;
        }

        public static int GetValueSpdPer(mi _eAxisNo, pv _ePstnNo)
        {
            return Para[(uint)_eAxisNo][(int)_ePstnNo].iSpeed;
        }

        public static void SetValue(uint _uAxisNo, uint _uPstnNo, double _dValue)
        {
            Para[_uAxisNo][(int)_uPstnNo].dPos = _dValue;
        }

        public static void SetValue(mi _uAxisNo, pv _uPstnNo, double _dValue)
        {
            Para[(uint)_uAxisNo][(int)_uPstnNo].dPos = _dValue;
        }

        public static void Load(string _sJobName)
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevicePath = sExeFolder + "JobFile\\" + _sJobName + "\\MotrPstn.ini";
            string sCommonPath = sExeFolder + "Util\\CommonPstn.ini";
            string sMinMaxPath = sExeFolder + "Util\\LimitPstn.ini";

            CIniFile IniDevice = new CIniFile(sDevicePath);
            CIniFile IniCommon = new CIniFile(sCommonPath);
            CIniFile IniMinMax = new CIniFile(sMinMaxPath);

            string sSection;

            //Save Device.
            int iMaxMotr = (int)mi.MAX_MOTR;
            for (int m = 0; m < iMaxMotr; m++)
            {
                //Set Dir.
                sSection = "Motor" + m.ToString();
                for (int i = 0; i < Para[m].Count; i++)
                {
                    if (Para[m][i].bCommon)
                    {
                        double dPos = 0;
                        IniCommon.Load(sSection, Para[m][i].sName, ref dPos); Para[m][i].dPos = dPos;
                    }
                    else
                    {
                        double dPos = 0;
                        IniDevice.Load(sSection, Para[m][i].sName, ref dPos); Para[m][i].dPos = dPos;
                    }
                    double dMin   = 0; 
                    double dMax   = 0;
                    int    iSpeed = 0;
                    IniMinMax.Load(sSection, Para[m][i].sName.Trim() + "_MIN", ref dMin  ); Para[m][i].dMin   = dMin   ;
                    IniMinMax.Load(sSection, Para[m][i].sName.Trim() + "_MAX", ref dMax  ); Para[m][i].dMax   = dMax   ;
                    IniMinMax.Load(sSection, Para[m][i].sName.Trim() + "_SPD", ref iSpeed); Para[m][i].iSpeed = iSpeed ;
                    
                    if (Para[m][i].dMax   <= 0  ) Para[m][i].dMax   = ML.MT_GetMaxPosition(m);
                    if (Para[m][i].iSpeed <= 0  ) Para[m][i].iSpeed = 100;
                    if (Para[m][i].iSpeed  > 100) Para[m][i].iSpeed = 100;

                    //if (MotrPstn[m].PstnValue[i].dMax    <= 0 )
                    //ML.MT_GetInPosSgnl

                }
            }
        }
        public static void Save(string _sJobName)
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevicePath = sExeFolder + "JobFile\\" + _sJobName + "\\MotrPstn.ini";
            string sCommonPath = sExeFolder + "Util\\CommonPstn.ini";
            string sMinMaxPath = sExeFolder + "Util\\LimitPstn.ini";

            CIniFile IniDevice = new CIniFile(sDevicePath);
            CIniFile IniCommon = new CIniFile(sCommonPath);
            CIniFile IniMinMax = new CIniFile(sMinMaxPath);

            string sSection;

            //Save Device.
            int iMaxMotr = (int)mi.MAX_MOTR;
            for (int m = 0; m < iMaxMotr; m++)
            {
                //Set Dir.
                sSection = "Motor" + m.ToString();
                for (int i = 0; i < Para[m].Count; i++)
                {
                    if (Para[m][i].bCommon)
                    {
                        IniCommon.Save(sSection, Para[m][i].sName, Para[m][i].dPos);
                    }
                    else
                    {
                        IniDevice.Save(sSection, Para[m][i].sName, Para[m][i].dPos);
                    }
                    if (Para[m][i].dMax <= 0) Para[m][i].dMax = ML.MT_GetMaxPosition(m);
                    IniMinMax.Save(sSection, Para[m][i].sName.Trim() + "_MIN", Para[m][i].dMin);
                    IniMinMax.Save(sSection, Para[m][i].sName.Trim() + "_MAX", Para[m][i].dMax);

                    if (Para[m][i].iSpeed <= 0  ) Para[m][i].iSpeed = 100;
                    if (Para[m][i].iSpeed  > 100) Para[m][i].iSpeed = 100;
                    IniMinMax.Save(sSection, Para[m][i].sName.Trim() + "_SPD", Para[m][i].iSpeed);
                }
            }
        }

        //private static ObservableCollection<T> DeepCopy<T>(IEnumerable<T> list) where T : ICloneable
        //{
        //    return new ObservableCollection<T>(list.Select(x => x.Clone()).Cast<T>());
        //}

        public static void UpdatePstn(bool _bToTable)
        {
            if (_bToTable)
            {
                //Display = new ObservableCollection<PositionPara>(Para);
                for (int i = 0; i < (int)mi.MAX_MOTR; i++)
                {
                    //foreach (var item in Para[i])
                    //Display[i] = new ObservableCollection<PositionPara>(Para[i]);
                    //당췌 모르겟다 이게 위에 처럼 하면 깊은 복사가 안되서 밑에처럼 수작업 함
                    Display[i].Clear();
                    for (int j = 0; j < Para[i].Count; j++) 
                    {
                        Display[i].Add(new PositionPara(){ 
                            bCommon = Para[i][j].bCommon ,
                            bGo     = Para[i][j].bGo     ,
                            bInput  = Para[i][j].bInput  ,
                            dMax    = Para[i][j].dMax    ,
                            dMin    = Para[i][j].dMin    ,
                            dPos    = Para[i][j].dPos    ,
                            iSpeed  = Para[i][j].iSpeed  ,
                            sName   = Para[i][j].sName   ,
                            uPstnNo = Para[i][j].uPstnNo 
                        });
                    }
                }
            }
            else
            {
                for (int i = 0; i < (int)mi.MAX_MOTR; i++)
                {
                    //Para[i] = new List<PositionPara>(Display[i]);
                    //Para[i].Clear();
                    //foreach (var item in Display[i])
                    //{
                    //    Para[i].Add(item);
                    //}
                    Para[i].Clear();
                    for (int j = 0; j < Display[i].Count; j++) 
                    {
                        Para[i].Add(new PositionPara(){ 
                            bCommon = Display[i][j].bCommon ,
                            bGo     = Display[i][j].bGo     ,
                            bInput  = Display[i][j].bInput  ,
                            dMax    = Display[i][j].dMax    ,
                            dMin    = Display[i][j].dMin    ,
                            dPos    = Display[i][j].dPos    ,
                            iSpeed  = Display[i][j].iSpeed  ,
                            sName   = Display[i][j].sName   ,
                            uPstnNo = Display[i][j].uPstnNo 
                        });
                    }

                }
            }
        }

    }

    public class PositionPara
    {
        //public int       iAxis    { get; set; }
        public string    sName    { get; set; }
        public uint      uPstnNo  { get; set; }
        public double    dPos     { get; set; }
        //public string    Go 
        //public string    Input
        //public string    Min     
        public double    dMin     { get; set; }
        //public string    Max     
        public double    dMax     { get; set; }
        //public string    Speed   { get; set; }
        public int       iSpeed   { get; set; }
 
        public bool      bGo      { get; set; }  
        public bool      bInput   { get; set; }

        public bool      bCommon  { get; set; }

        //private static List<Positiondata> instance;
        //
        //public static List<Positiondata> GetInstance()
        //{
        //    if (instance == null)
        //        instance = new List<Positiondata>();
        //
        //    return instance;
        //}
    }

    //public class MotorPosConverter : IValueConverter
    internal class MotorPosConverter : MarkupExtension, IValueConverter //IMultiValueConverter
    {
        private static MotorPosConverter converter;
        public MotorPosConverter()
        {
        }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return converter ?? (converter = new MotorPosConverter());
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //convert the int to a string:
            return value.ToString();
        }
    
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(parameter != null && parameter.ToString() == "SpeedVal")
            {
                int iRet;
                if(!int.TryParse(value.ToString(),out iRet)) iRet = 0 ; 

                     if(iRet > 100) iRet = 100;
                else if(iRet < 0  ) iRet =   0;

                return iRet;
            }
            else if(parameter != null && parameter.ToString() == "MinMax")
            {
                double dRet;
                if(!double.TryParse(value.ToString(),out dRet)) dRet = 0 ; 

                int iAxis = (int)PM.iClickedAxis;
                //if(!int.TryParse(parameter.ToString(),out iAxis)) iAxis = 0 ; 

                     if (dRet < ML.MT_GetMinPosition(iAxis)) dRet = ML.MT_GetMinPosition(iAxis);
                else if (dRet > ML.MT_GetMaxPosition(iAxis)) dRet = ML.MT_GetMaxPosition(iAxis);

                return dRet;
            }
            else if(targetType.Name == "Double")
            {
                if(double.TryParse(value.ToString(),out double dRet)) return dRet;
                return 0.0;
            }

            return value.ToString();
            //throw new NotImplementedException();
            //convert the string back to an int here
            //return int.Parse(value.ToString());
        }

    }
}
