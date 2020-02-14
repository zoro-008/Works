using COMMON;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using Machine;
using System.IO;
using Microsoft.Win32;
using System.Text;
using LiveCharts.Configurations;
using LiveCharts.Defaults;

namespace Machine.View
{
    /// <summary>
    /// Spc.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Spc : UserControl
    {
        //Basic Stacked
        public SeriesCollection ColumnCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }

        //Doughnut Stack Chart
        public SeriesCollection DoughnutSeries  { get; set; }
        public SeriesCollection StackCollection { get; set; }
        
        //DataGrid Data
        ObservableCollection<TLotData    > lstLot  = new ObservableCollection<TLotData    >();
        ObservableCollection<TErrData    > lstErr  = new ObservableCollection<TErrData    >();
        ObservableCollection<TFailureData> lstFail = new ObservableCollection<TFailureData>();

        
        public Spc()
        {
            InitializeComponent();

            //DataGrid ItemSource
            DgLot .ItemsSource = lstLot;
            DgErr .ItemsSource = lstErr;
            DgFail.ItemsSource = lstFail;

            //Doughnut Chart
            //https://lvcharts.net/App/examples/v1/wpf/Doughnut%20Chart
            DoughnutSeries = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "ErrNo 0",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(8) },
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "ErrNo 1",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(6) },
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "ErrNo 2",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(10) },
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "ErrNo 3",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(4) },
                    DataLabels = true
                }
            };

            //Basic Column
            //https://lvcharts.net/App/examples/v1/wpf/Basic%20Column
            //ColumnCollection = new SeriesCollection
            ColumnCollection = new SeriesCollection
            {
                //new ColumnSeries
                new ColumnSeries
                {
                    Title = "ErrTime",
                    //Values = new ChartValues<double> { 10, 50, 39, 50 }
                    Values = new ChartValues<double> { 10, 50, 39, 50 }
                }
            };
 
            //adding series will update and animate the chart automatically
            //ColumnCollection.Add(new ColumnSeries
            //{
            //    Title = "2016",
            //    Values = new ChartValues<double> { 11, 56, 42 }
            //});
 
            //also adding values updates and animates the chart automatically
            ColumnCollection[0].Values.Add(48d);
 
            //Labels = new[] {"Maria", "Susan", "Charles", "Frida"};
            //Formatter = value => value.ToString("N");
 
            DataContext = this;

            DpSttTime.SelectedDate = DateTime.Now;
            DpEndTime.SelectedDate = DateTime.Now;
        }


        private void BtDataView_Click(object sender, RoutedEventArgs e)
        {
            if(DpSttTime.SelectedDate == null) return;
            if(DpEndTime.SelectedDate == null) return;

                 if(TcData.SelectedIndex == 0) SPC.LOT.LoadDataList(DpSttTime.SelectedDate.Value,DpEndTime.SelectedDate.Value, ref lstLot );
            else if(TcData.SelectedIndex == 1) SPC.ERR.LoadDataList(DpSttTime.SelectedDate.Value,DpEndTime.SelectedDate.Value, ref lstErr );
            else if(TcData.SelectedIndex == 2) SPC.FLR.LoadDataList(DpSttTime.SelectedDate.Value,DpEndTime.SelectedDate.Value, ref lstFail);
            
            UptimeText();
            
            UpdateErrChart();
            //DateTime.FromOADate(dValue).ToString("yyyy-MM-dd HH:mm:ss");
            //Formatter = value => DateTime.FromOADate(value).ToString("HH:mm:ss");
        }
        private void UpdateErrChart()
        {

            //DoughnutSeriesUpdate();
            DoughnutSeries.Clear(); 
            for (int i = 0; i < lstErr.Count; i++)
            {
                bool bExist = false;
                string sTitle = "ErrNo " + lstErr[i].ErrNo.ToString();
                foreach (var series in DoughnutSeries)
                {
                    if(series.Title == sTitle)
                    {
                        foreach (var observable in series.Values.Cast<ObservableValue>())
                        {
                            observable.Value += 1;
                            bExist = true;
                        }
                    }
                }
                if (!bExist)
                {
                    DoughnutSeries.Add(
                        new PieSeries
                        {
                            Title = sTitle,
                            Values = new ChartValues<ObservableValue> { new ObservableValue(1) },
                            DataLabels = true
                        });
                }
            }
            
            //TimeChartUpdate();
            List<TErrData    > lstBuff  = new List<TErrData    >();
            
            foreach (var err in lstErr)
            {
                bool bHave = false;
                foreach (var buff in lstBuff)
                {
                    if(err.ErrNo == buff.ErrNo)
                    {
                        int index = lstBuff.IndexOf(buff);
                        TErrData terr = lstBuff[index];
                        terr.ErrTime += err.ErrTime;
                        lstBuff[index] = terr ; 
                        bHave = true;
                        break;
                    }
                }
                if (!bHave)
                {
                    lstBuff.Add(err);
                }
            }
            
            ColumnCollection.Clear();
            foreach (var buff in lstBuff)
            {
                ColumnCollection.Add(new ColumnSeries { Title = "ErrNo "+buff.ErrNo.ToString(), Values = new ChartValues<double> { buff.ErrTime } });
            }
            if(ColumnCollection.Chart.AxisX != null)
                ColumnCollection.Chart.AxisX[0].ShowLabels = false;
            
            Formatter = value => TimeSpan.FromMilliseconds(value).ToString();
            ErrAxisY.LabelFormatter = Formatter;
            ColumnCollection.Chart.AxisX[0].Title = "";
            DataContext = this;

            

        }

        private void UptimeText()
        {
                 if(TcData.SelectedIndex == 0) LbUptime.Text = string.Format("UPTIME={0:0.000}%" ,GetLotUptime()) ;
            else if(TcData.SelectedIndex == 1) LbUptime.Text = string.Format("MTBA={0:0.000}min" ,GetMTBA     ()) + " " + string.Format("MTTA={0:0.000}sec" ,GetMTTA()) ;
            else if(TcData.SelectedIndex == 2) LbUptime.Text = string.Format("MTBF={0:0.000}hour",GetMTBF     ()) + " " + string.Format("MTTR={0:0.000}min" ,GetMTTR()) ;
            else LbUptime.Text = "";
        }

        private double GetLotUptime()
        {
            TimeSpan Span        = new TimeSpan();
            TimeSpan RunTime     = new TimeSpan();
            TimeSpan DownTime    = new TimeSpan();
            TimeSpan FailureTime = new TimeSpan();
            for (int r = 0; r < lstLot.Count; r++)
            {
                if(!SPC.LOT.TryParse(lstLot[r].RunTime.ToString(),    out Span)) return 0.0;
                RunTime  += Span ;                                           
                if(!SPC.LOT.TryParse(lstLot[r].DownTime.ToString(),   out Span)) return 0.0;
                DownTime += Span ;
                if(!SPC.LOT.TryParse(lstLot[r].FailureTime.ToString(),out Span)) return 0.0;
                FailureTime += Span ;
            }
            double dRetTime  = 0 ;
            double dTotalSec = 0 ;
            try{
                dTotalSec =(RunTime+DownTime+FailureTime).TotalSeconds ;
                if(dTotalSec != 0) {dRetTime = RunTime.TotalSeconds*100/dTotalSec ;}
            }
            catch(Exception e){
                dRetTime = 0 ;
            }
            
            return dRetTime;
        }

        //error 탭. 잼간 평균간격.Mean Time Between Assist
        private double GetMTBA()
        {
            if(lstErr.Count < 2) return 0.0 ; //2개이상 되어야 간격을 계산 할 수 있다.

            //간격을 보는 거라 한번 덜 한다.
            DateTime Time1 ;
            DateTime Time2 ;
            TimeSpan Span = new TimeSpan() ;

            if(!DateTime.TryParse (lstErr[0].StartedAt.ToString()             , out Time1)) return 0.0;
            if(!DateTime.TryParse (lstErr[lstErr.Count-1].StartedAt.ToString(), out Time2)) return 0.0;
            Span  = Time2 - Time1 ;  
            double dRet = 0 ;
            
            if((lstErr.Count-1) != 0){dRet = Span.TotalMinutes / (lstErr.Count-1) ;}
            return dRet ;
        }

        //Mean Time to Assist 쨈푸는 평균 시간.
        private double GetMTTA()
        {
            if(lstErr.Count < 1) return 0.0 ; //1개이상 되어야 간격을 계산 할 수 있다.
            //간격을 보는 거라 한번 덜 한다.
            TimeSpan Span = new TimeSpan() ;
            double dRet = 0.0 ;
            for (int r = 0; r < lstErr.Count; r++)
            {
                if(!SPC.ERR.TryParse (lstErr[r].ErrTime.ToString(), out Span)) return 0.0;
                dRet += Span.TotalSeconds ;            
            }
            
            if(lstErr.Count != 0) {dRet = dRet/(double)lstErr.Count ;}
            else                  {dRet = 0.0 ; }
            return dRet ;
        }

        //평균 수리간격.Mean Time Between Assist
        private double GetMTBF()
        {
            if(lstFail.Count < 2) return 0.0 ; //2개이상 되어야 간격을 계산 할 수 있다.
            //간격을 보는 거라 한번 덜 한다.
            DateTime Time1 ;
            DateTime Time2 ;
            TimeSpan Span = new TimeSpan() ;

            if(!DateTime.TryParse (lstFail[0].StartedAt.ToString()              , out Time1)) return 0.0;
            if(!DateTime.TryParse (lstFail[lstFail.Count-1].StartedAt.ToString(), out Time2)) return 0.0;
            Span = Time2 - Time1 ;  

            double dRet = 0.0;
            if((lstFail.Count-1) != 0 ) {dRet = Span.TotalHours / (lstFail.Count-1) ;}

            return dRet ;
        }

        //Mean Time to Repair 평균수리시간.
        private double GetMTTR()
        {
            if(lstFail.Count < 1) return 0.0 ;
            //간격을 보는 거라 한번 덜 한다.
            TimeSpan Span = new TimeSpan() ;
            double dRet = 0.0 ;
            for (int r = 0; r < lstFail.Count; r++)
            {
                if(!SPC.ERR.TryParse (lstFail[r].FailureTime.ToString(), out Span)) return 0.0;
                dRet += Span.TotalMinutes ;            
            }

            if(lstFail.Count!=0) {dRet = dRet/(double)lstFail.Count ;}
            else                 {dRet = 0;}
            return dRet ;
        }

        private void TcData_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
 
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
            Log.TraceListView(this.GetType().Name + " " + sText + " Button Clicked", ForContext.Frm);

            var btn = sender as Button;
            var Tag = btn.Tag;
            
            DataGrid dg = null;

            string sToFind = ""   ;
            int  ? iIndex  = null ;

                 if(TcData.SelectedIndex == 0) { sToFind = TbLotFind .Text; dg = DgLot ; for (int i = 0; i < lstLot .Count; i++) if(Find(sToFind,lstLot [i])) {iIndex = i; break;} }
            else if(TcData.SelectedIndex == 1) { sToFind = TbErrFind .Text; dg = DgErr ; for (int i = 0; i < lstErr .Count; i++) if(Find(sToFind,lstErr [i])) {iIndex = i; break;}}
            else if(TcData.SelectedIndex == 2) { sToFind = TbFailFind.Text; dg = DgFail; for (int i = 0; i < lstFail.Count; i++) if(Find(sToFind,lstFail[i])) {iIndex = i; break;}}
            
            if(iIndex.HasValue) dg.SelectedIndex = iIndex.Value ;

            //Datas.FindIndex(x => x.LotNo.IndexOf(sToFind) >= 0);
            //DgLot.SelectedIndex = 10 ;
        }

        private bool Find(string _sFind, object _oData )
        {
            string sToFind = _sFind;

            Type type2 = _oData.GetType();
            FieldInfo[] f = type2.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            for(int j = 0 ; j < f.Length ; j++){
                if(f[j].GetValue(_oData).ToString().IndexOf(sToFind) >= 0) return true;
            }

            return false;

        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string sText = ((Button)sender).Content.ToString();
            Log.TraceListView(this.GetType().Name + " " + sText + " Button Clicked", ForContext.Frm);

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "csv File|*.csv";
            saveFileDialog1.Title = "Save";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName == "") return;
                
            //Machine.spSpcUnit.
            DataGrid dg = null;

                 if(TcData.SelectedIndex == 0)   { dg = DgLot ; }
            else if(TcData.SelectedIndex == 1)   { dg = DgErr ; }
            else if(TcData.SelectedIndex == 2)   { dg = DgFail; }

            if(dg == null) return;

            dg.SelectAllCells();
            dg.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
            ApplicationCommands.Copy.Execute(null, dg);
            dg.UnselectAllCells();
            String result = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);
            File.AppendAllText(saveFileDialog1.FileName, result, UnicodeEncoding.UTF8);
        }

        private void Repair_Click(object sender, RoutedEventArgs e)
        {
            string sText = ((Button)sender).Content.ToString();
            Log.TraceListView(this.GetType().Name + " " + sText + " Button Clicked", ForContext.Frm);
            new Repair().ShowDialog();
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

    public class DateModel
    {
        public System.DateTime DateTime { get; set; }
        public double Value { get; set; }
    }
}
