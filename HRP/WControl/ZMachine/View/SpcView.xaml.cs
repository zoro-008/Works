using COMMON;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;

namespace Machine.View
{
    /// <summary>
    /// SpcView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SpcView : UserControl
    {
        //Basic Stacked
        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }

        public SpcView()
        {
            InitializeComponent();

            //Pie Chart
            PieChartExample();

            //Basic Stacked
            //https://lvcharts.net/App/examples/v1/wpf/Basic%20Stacked
            SeriesCollection = new SeriesCollection
            {
                new StackedColumnSeries
                {
                    Values = new ChartValues<double> {4, 5, 6, 8},
                    StackMode = StackMode.Values, // this is not necessary, values is the default stack mode
                    DataLabels = true
                },
                new StackedColumnSeries
                {
                    Values = new ChartValues<double> {2, 5, 6, 7},
                    StackMode = StackMode.Values,
                    DataLabels = true
                }
            };
 
            //adding series updates and animates the chart
            SeriesCollection.Add(new StackedColumnSeries
            {
                Values = new ChartValues<double> {6, 2, 7},
                StackMode = StackMode.Values
            });
 
            //adding values also updates and animates
            SeriesCollection[2].Values.Add(4d);
 
            Labels = new[] {"Chrome", "Mozilla", "Opera", "IE"};
            Formatter = value => value + " Mill";
 
            DataContext = this;
        }

        List<TLotData> Datas;
        private void BtDataView_Click(object sender, RoutedEventArgs e)
        {
            Datas = new List<TLotData>();
            if(DpSttTime.SelectedDate == null) return;
            if(DpEndTime.SelectedDate == null) return;
            SPC.LOT.LoadDataList(DpSttTime.SelectedDate.Value,DpEndTime.SelectedDate.Value, ref Datas);
            
            DgLot.ItemsSource = null;
            DgLot.ItemsSource = Datas;
            
            //SPC.LOT.DispData(DpSttTime.SelectedDate.Value,DpEndTime.SelectedDate.Value);

        }

        private void TcData_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        //Pie Chart
        //https://lvcharts.net/App/examples/v1/wpf/Pie%20Chart
        public void PieChartExample()
        {
            InitializeComponent();
 
            PointLabel = chartPoint =>
                string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);
 
            DataContext = this;
        }
 
        public Func<ChartPoint, string> PointLabel { get; set; }
 
        private void Chart_OnDataClick(object sender, ChartPoint chartpoint)
        {
            var chart = (LiveCharts.Wpf.PieChart) chartpoint.ChartView;
            
            //clear selected slice.
            foreach (PieSeries series in chart.Series)
                series.PushOut = 0;
 
            var selectedSeries = (PieSeries) chartpoint.SeriesView;
            selectedSeries.PushOut = 8;
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {

        }

        private void DgLot_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex()+1).ToString(); 
        }

        private void Find_Click(object sender, RoutedEventArgs e)
        {
            string sText = ((Button)sender).Content.ToString();
            Log.Trace(this.GetType().Name + " " + sText + " Button Clicked", ForContext.Frm);

            string sToFind = TbLotFind.Text;
            int    iIndex  = 0;

            for (int i = 0; i < Datas.Count; i++)
            {
                Type type = Datas[i].GetType();
                FieldInfo[] f = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                for(int j = 0 ; j < f.Length ; j++){
                    string sTemp = f[j].GetValue(Datas[i]).ToString();
                    if(f[j].GetValue(Datas[i]).ToString().IndexOf(sToFind) >= 0)
                    {
                        iIndex = i;
                    }
                }
            }
            
            DgLot.SelectedIndex = iIndex ;
            //Datas.FindIndex(x => x.LotNo.IndexOf(sToFind) >= 0);
            //DgLot.SelectedIndex = 10 ;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            string sText = ((Button)sender).Content.ToString();
            Log.Trace(this.GetType().Name + " " + sText + " Button Clicked", ForContext.Frm);


        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string sText = ((Button)sender).Content.ToString();
            Log.Trace(this.GetType().Name + " " + sText + " Button Clicked", ForContext.Frm);


        }
    }


    internal class SpcConverter : MarkupExtension, IValueConverter //IMultiValueConverter
    {
        private static SpcConverter converter;
        public SpcConverter()
        {
        }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return converter ?? (converter = new SpcConverter());
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(parameter != null && parameter.ToString() == "AT")
            {
                double.TryParse(value.ToString(),out double dValue);
                string sValue = DateTime.FromOADate(dValue).ToString("yyyy-MM-dd HH:mm:ss");
                return sValue;
            }
            else if(parameter != null && parameter.ToString() == "TIME")
            {
                double.TryParse(value.ToString(),out double dValue);
                TimeSpan Span ;
                try  { Span = TimeSpan.FromMilliseconds(dValue); }
                catch{ Span = TimeSpan.FromMilliseconds(0     ); }
                string sValue = string.Format("{0:00}.{1:00}:{2:00}:{3:00}" , Span.Days , Span.Hours , Span.Minutes , Span.Seconds);
                return sValue;
            }
            return value.ToString();
        }
    
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new NotImplementedException();
        }

    }
}
