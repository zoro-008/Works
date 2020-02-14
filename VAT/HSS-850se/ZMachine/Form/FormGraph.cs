using COMMON;
using SML;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Machine
{
    public partial class FormGraph : Form
    {
        SeriesChartType Sct      ;
        string          sName    ;
        bool            bSpecial ;
        public FormGraph(SeriesChartType _Sct, string _sName, bool _bSpecial = false)
        {
            InitializeComponent();

            Sct      = _Sct      ;
            sName    = _sName    ;
            bSpecial = _bSpecial ;
            chart1.MouseWheel += new MouseEventHandler(evMouseWheel1);
            
            //chart1.Series[0].IsValueShownAsLabel = false;
            chart1.Series[0].IsVisibleInLegend = false;
            chart1.Legends.Clear();

            //chart1.ChartAreas[0]
            //chrtarea.CursorX.IsUserEnabled = true;
            //chrtarea.CursorX.IsUserSelectionEnabled = true;
            //chrtarea.CursorY.IsUserEnabled = true;
            //chrtarea.CursorY.IsUserSelectionEnabled = true;
            //
            //chrtarea.AxisX.ScaleView.Zoomable = true;
            //chrtarea.AxisY.ScaleView.Zoomable = true;
            //chrtarea.AxisY2.ScaleView.Zoomable = true;

            //chart1.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            //chart1.ChartAreas.FirstOrDefault().AxisX.IntervalType = DateTimeIntervalType.Milliseconds;
            //chart1.ChartAreas.FirstOrDefault().AxisX.Interval = 1;

            // Define X axis interval type (seconds). Width 200s, interval = 10s, label every 30s
            //chart1.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Seconds;
            //chart1.ChartAreas[0].AxisX.Interval = 0.1;   // Add a tick every 10s interval on X axis (careful, it's different from the grid !!!)
            //chart1.ChartAreas[0].AxisX.Title = "Time (s)";
            //chart1.ChartAreas[0].AxisX.IntervalOffsetType = DateTimeIntervalType.Seconds;
            //chartArea1->AxisX->LabelStyle->Format = L"HH:mm:ss.fffffff";
            //chart1.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Milliseconds;
            //chart1.ChartAreas[0].AxisX.LabelStyle.Format = "ss.fff";
            //chart1.ChartAreas[0].AxisX.LabelStyle.IntervalOffsetType = DateTimeIntervalType.Seconds;
            //chart1.ChartAreas[0].AxisX.Interval = 1;    // Add a label every 30s
            //chart1.ChartAreas[0].AxisX.LabelStyle.IntervalOffset = 0D;
            //chart1.ChartAreas[0].AxisX.LabelStyle.Format = @"hh\:mm\:ss\.fff";
            //chart1.ChartAreas[0].AxisX.Minimum = 0; // X axis unit is a day ! (don't know why )
            //chart1.ChartAreas[0].AxisX.Maximum = 200.0/(24.0*60.0*60.0); // Unit is a day so convert second in day unit =>  200s => 200s / (24h*60m*60s)


            //chart1.Series[0].XValueType = ChartValueType.DateTime; 


     //chart1.ChartAreas[0].AxisX.Maximum = 0;//timelist.Max().ToOADate(); 
     //chart1.ChartAreas[0].AxisX.Minimum = timelist.Min().ToOADate(); 
            
            //chart1.ChartAreas[0].AxisX.LabelStyle.Format = "mm:ss.fff";// chartArea1->AxisX->LabelStyle->Format = L"HH:mm:ss.fffffff";
            //chart1.ChartAreas[0].AxisX.LabelStyle.IntervalType = DateTimeIntervalType.Milliseconds;
            //chart1.ChartAreas[0].AxisX.Interval = 1000;
            //chart1.

            
            //chart1.ChartAreas[0].AxisX.LabelStyle.IntervalType = DateTimeIntervalType.Number;
            ////chart1.ChartAreas[0].AxisX.LabelStyle.Format = "hh:mm:ss.fff";

            //chart1.Series[0].Points.AddXY(x.AddMilliseconds(100), 34);
            //chart1.Series[0].Points.AddXY(x.AddMilliseconds(300), 334);
            //System.DateTime dt = System.DateTime.FromOADate(chart1.Series[0].Points[0].XValue);

            //----------------
            //System.DateTime dt = new System.DateTime(1983,11,08,0,0,0);
            //chart1.Series[0].XValueType = ChartValueType.DateTime;
            ////chart1.ChartAreas[0].AxisX.LabelStyle.Format = "HH:mm:ss.fff";
            //chart1.ChartAreas[0].AxisX.LabelStyle.Format = "H:mm:ss";
            ////chart1.ChartAreas[0].AxisX.LabelStyle.Enabled = true;
            ////chart1.ChartAreas[0].AxisX.LabelStyle.
            ////chart1.ChartAreas[0].AxisX.Interval = 
            //chart1.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Milliseconds;
            //chart1.ChartAreas[0].AxisX.Minimum = dt.ToOADate();
            ////chart1.ChartAreas[0].AxisX.IntervalOffset = 1 ;
            ////chart1.ChartAreas[0].AxisX.ScaleView.

            //chart1.ChartAreas[0].CursorX.IsUserEnabled = true;
            //chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            //chart1.ChartAreas[0].CursorX.IntervalType = DateTimeIntervalType.Milliseconds;
            //chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            //chart1.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;

            //chart1.ChartAreas[0].CursorX.IsUserEnabled = true;
            //chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
        //chart1.ChartAreas[0].CursorX.IntervalType = DateTimeIntervalType.Seconds;
        //chart1.ChartAreas[0].CursorX.Interval = 1D;
            //chart1.ChartAreas[0].CursorX.IntervalType = DateTimeIntervalType.Seconds ;
            //chart1.ChartAreas[0].CursorX.Interval = 1D;
            //chart1.ChartAreas[0].AxisX.IsMarginVisible = false;
            //chart1.ChartAreas[0].AxisY.IsMarginVisible = false;
            //chart1.ChartAreas[0].InnerPlotPosition = new ElementPosition(0,0,0,0);

            
            //데이터 포인트 저장
            //for (double k=0;k<2*Math.PI;k+=0.1)
            //{
            //    chart1.Series[0].Points.AddXY(k,Math.Sin(k));
            //}
            Chart_Clear();
            //End();

            var PropLotInfo1 = chart1.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropLotInfo1.SetValue(chart1, true, null);
            //

        }

        public void Show(int _iW, int _iH)
        {
            this.Width  = _iW;
            this.Height = _iH;
            Show();
        }
        private void evMouseWheel1(object sender, MouseEventArgs e)
        {
            chart1.ChartAreas[0].AxisX.ScaleView.ZoomReset();
            chart1.ChartAreas[0].AxisY.ScaleView.ZoomReset();
            //if(e.Delta > 0)
            //{
            //    chart1.ChartAreas[0].AxisX.ScaleView.ZoomReset();
            //}
            //if(e.Delta < 0)
            //{
            //    chart1.ChartAreas[0].AxisX.ScrollBar.Enabled  = true;
            //    chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            //
            //    double xMin = chart1.ChartAreas[0].AxisX.ScaleView.ViewMinimum ;
            //    double xMax = chart1.ChartAreas[0].AxisX.ScaleView.ViewMaximum ;
            //
            //    if(e.Location.X > 0)
            //    { 
            //        int posXStart  = (int)chart1.ChartAreas[0].AxisX.PixelPositionToValue(e.Location.X) - (int)(xMax - xMin) / 2 ;
            //        int posXFinish = (int)chart1.ChartAreas[0].AxisX.PixelPositionToValue(e.Location.X) + (int)(xMax - xMin) / 2 ;
            //
            //        chart1.ChartAreas[0].AxisX.ScaleView.Zoom(posXStart,posXFinish);
            //    }
            //}
        }

        private void FormGraph_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing) e.Cancel = true;
            this.Hide();
            
        }

        /*
        public void Chart_AddPoints(string _dX, string _dY)
        {
            DateTime dt = DateTime.Now;
            if (chart1.InvokeRequired)
            {
                chart1.Invoke(new MethodInvoker(delegate () {
                    

                    int    i1 ; int.TryParse(_dX,out i1); i1 += 5000;
                    double i2 ; double.TryParse(_dY,out i2);

                    //double d1 = CConfig.StrToDoubleDef(_dX,0);
                    if(bSpecial) chart1.Series[0].Points[i1].SetValueY(i2);
                    else         chart1.Series[0].Points.AddXY(_dX,_dY);
                    //else         chart1.Series[0].Points.AddXY(dt,_dY);
                    
                }));
            }
            else
            {
                //chart1.Visible = false;
                int i1 ; int.TryParse(_dX,out i1); i1 += 5000;
                int i2 ; int.TryParse(_dY,out i2);

                //double d1 = CConfig.StrToDoubleDef(_dX,0);       
                if(bSpecial) chart1.Series[0].Points[i1].SetValueY(i2);
                else         chart1.Series[0].Points.AddXY(_dX,_dY);
                //else         chart1.Series[0].Points.AddXY(dt,_dY);
                
            }
        }
        */


        public void Chart_AddPoints(DateTime _dX, string _dY)
        {
            if (chart1.InvokeRequired)
            {
                chart1.Invoke(new MethodInvoker(delegate () {
                    chart1.Series[0].Points.AddXY(_dX,_dY);
                }));
            }
            else
            {
                chart1.Series[0].Points.AddXY(_dX,_dY);
            }
        }

        public void Begin()
        {
            if (chart1.InvokeRequired)
            {
                chart1.Invoke(new MethodInvoker(delegate () {
                    //chart1.Visible = false;
                    chart1.Series.SuspendUpdates();
                    chart1.ChartAreas.SuspendUpdates();
                    Chart_Clear();
                }));
            }
            else
            {
                //chart1.Visible = false;
                chart1.Series.SuspendUpdates();
                chart1.ChartAreas.SuspendUpdates();
                Chart_Clear();
            }
        }
        public void End()
        {
            if (chart1.InvokeRequired)
            {
                chart1.Invoke(new MethodInvoker(delegate () {
                    //Visible1();
                    chart1.Series.ResumeUpdates();
                    chart1.ChartAreas.ResumeUpdates();
                }));
            }
            else
            {
                //Visible1();
                chart1.Series.ResumeUpdates();
                chart1.ChartAreas.ResumeUpdates();
            }
        }

        public void Save(string _sFileName)
        {
            if (chart1.InvokeRequired)
            {
                chart1.Invoke(new MethodInvoker(delegate () {
                    //Visible1();
                    if(File.Exists(_sFileName)) return;
                    chart1.SaveImage(_sFileName,ChartImageFormat.Jpeg);
                }));
            }
            else
            {
                //Visible1();
                if(File.Exists(_sFileName)) return;
                chart1.SaveImage(_sFileName,ChartImageFormat.Jpeg);
            }
            
            
        }
        public void Chart_Clear()
        {
            
            if (chart1.InvokeRequired)
            {
                chart1.Invoke(new MethodInvoker(delegate () {
                    chart1.Series.Clear();
                    Series Out1    = new Series();
                    Out1.ChartType = Sct  ;//SeriesChartType.FastLine;
                    Out1.Name      = sName; 
                    chart1.Series.Add(Out1); 
                    Out1.MarkerSize = 2;

                    System.DateTime dt = new System.DateTime(1983,11,08,0,0,0);
                    chart1.Series[0].XValueType = ChartValueType.DateTime;
                    chart1.ChartAreas[0].AxisX.LabelStyle.Format = "H:mm:ss";
                    chart1.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Milliseconds;
                    chart1.ChartAreas[0].AxisX.Minimum = dt.ToOADate();
                    
                    chart1.ChartAreas[0].CursorX.IsUserEnabled = true;
                    chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
                    chart1.ChartAreas[0].CursorX.IntervalType = DateTimeIntervalType.Milliseconds;
                    chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
                    chart1.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;

                    chart1.ChartAreas[0].CursorY.IsUserEnabled = true;
                    chart1.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
                    chart1.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
                    chart1.ChartAreas[0].AxisY.ScrollBar.IsPositionedInside = true;
                                    
                    if(bSpecial)
                    {
                        for(int i=0; i<10000; i++)
                        {
                            //chart1.Series[0].Points.AddY(-7000);
                            chart1.Series[0].Points.AddXY(-5000 + i ,-7000);
                        }

                        //chart1.ChartAreas[0].AxisX.Minimum = -5000 ;
                        //chart1.ChartAreas[0].AxisX.Maximum =  5000 ;
                        //chart1.ChartAreas[0].AxisX.RoundAxisValues = true;

                        //chart1.ChartAreas[0].AxisY.Minimum = -5000 ;
                        //chart1.ChartAreas[0].AxisY.Maximum =  5000 ;

                    }                                                        
                }));
            }
            else
            {
                chart1.Series.Clear();
                Series Out1    = new Series();
                Out1.ChartType = Sct  ;//SeriesChartType.FastLine;
                Out1.Name      = sName; 
                chart1.Series.Add(Out1); 
                Out1.MarkerSize = 2;

                System.DateTime dt = new System.DateTime(1983,11,08,0,0,0);
                chart1.Series[0].XValueType = ChartValueType.DateTime;
                chart1.ChartAreas[0].AxisX.LabelStyle.Format = "H:mm:ss";
                chart1.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Milliseconds;
                chart1.ChartAreas[0].AxisX.Minimum = dt.ToOADate();
                
                chart1.ChartAreas[0].CursorX.IsUserEnabled = true;
                chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
                chart1.ChartAreas[0].CursorX.IntervalType = DateTimeIntervalType.Milliseconds;
                chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
                chart1.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;

                chart1.ChartAreas[0].CursorY.IsUserEnabled = true;
                chart1.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
                chart1.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
                chart1.ChartAreas[0].AxisY.ScrollBar.IsPositionedInside = true;

                if(bSpecial)
                {
                    for(int i=0; i<10000; i++)
                    {
                        //chart1.Series[0].Points.AddY(-7000);
                        chart1.Series[0].Points.AddXY(-5000 + i ,-7000);
                    }

                    //chart1.ChartAreas[0].AxisX.Minimum = -5000 ;
                    //chart1.ChartAreas[0].AxisX.Maximum =  5000 ;
                    //chart1.ChartAreas[0].AxisX.RoundAxisValues = true;

                    //chart1.ChartAreas[0].AxisY.Minimum = -5000 ;
                    //chart1.ChartAreas[0].AxisY.Maximum =  5000 ;

                }
            }
        }

        private int iCnt = 0;
        System.DateTime dt ;
        Stopwatch sw = new Stopwatch();
        private void button1_Click(object sender, EventArgs e)
        {
            
            Random rd = new Random();
            if(iCnt == 0) {
                dt = new System.DateTime(1983,11,08,0,0,0);
                sw.Start();
            }
            iCnt++;
            Begin();
            
            for(int i=0; i<60; i++)
            {
                
                double d1 = rd.Next(0,50);
                //System.DateTime dt = new System.DateTime(1983,11,08,0,0,0);
                Chart_AddPoints(dt.AddMilliseconds(sw.ElapsedMilliseconds),d1.ToString());
                End();
                //CAddPoints1(i.ToString(),d1.ToString());
            }
            //iCnt += 60 ;
            //End();

            //Begin();
            ////Chart_Clear();
            //Random rd = new Random();
            //
            //double d1 = rd.Next(-2000,2000);
            //double d2 = rd.Next(-2000,2000);
            //
            //Chart_AddPoints(d1.ToString(),d2.ToString());
            //
            //End();
        }

        private void FormGraph_DoubleClick(object sender, EventArgs e)
        {
        
        }

        private void chart1_DoubleClick(object sender, EventArgs e)
        {
            //Log.Trace("FormGraph Chart Double Clicked", ti.Frm);

            //SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            //saveFileDialog1.Filter = "jpg File|*.jpg";
            //saveFileDialog1.Title = "Save";
            //saveFileDialog1.ShowDialog();

            //if (saveFileDialog1.FileName == "") return;

            //Save(saveFileDialog1.FileName);    


        }

        private void chart1_Click(object sender, EventArgs e)
        {
        
        }

        private void FormGraph_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void chart1_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                //if (Log.ShowMessageModal("Confirm", "Do you want to Clear?") != DialogResult.Yes) return;
                //Chart_Clear();
                if (Log.ShowMessageModal("Confirm", "Do you want to Save?") != DialogResult.Yes) return;
                
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "jpg File|*.jpg";
                saveFileDialog1.Title = "Save";
                saveFileDialog1.ShowDialog();
                
                if (saveFileDialog1.FileName == "") return;
                
                Save(saveFileDialog1.FileName);    
            }
            
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;

            if(chart1.ChartAreas[0].AxisX.ScaleView.IsZoomed)
            {
                chart1.ChartAreas[0].AxisX.LabelStyle.Format = "H:mm:ss.fff";
                if(!bInside) {
                    chart1.ChartAreas[0].AxisX.ScaleView.Position = chart1.ChartAreas[0].AxisX.Maximum - chart1.ChartAreas[0].AxisX.ScaleView.Size;
                }
            }
            else
            {
                chart1.ChartAreas[0].AxisX.LabelStyle.Format = "H:mm:ss";
            }    

            /* 
            //Manual Test
            Random rd = new Random();
            if(iCnt == 0) {
                dt = new System.DateTime(1983,11,08,0,0,0);
                sw.Start();
                Begin();
            }
            iCnt++;
            
            double d1 = rd.Next(0,50);
            //System.DateTime dt = new System.DateTime(1983,11,08,0,0,0);
            Chart_AddPoints(dt.AddMilliseconds(sw.ElapsedMilliseconds),d1.ToString());
            End();
            */

            timer1.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            chart1.ChartAreas[0].AxisX.ScaleView.Position = chart1.ChartAreas[0].AxisX.Maximum - chart1.ChartAreas[0].AxisX.ScaleView.Size;
        }

        private bool bInside = false;
        private void chart1_MouseHover(object sender, EventArgs e)
        {
            bInside = true ;
        }

        private void chart1_MouseLeave(object sender, EventArgs e)
        {
            bInside = false;
        }
    }
}
