using COMMON;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
            if(e.Delta > 0)
            {
                chart1.ChartAreas[0].AxisX.ScaleView.ZoomReset();
            }
            if(e.Delta < 0)
            {
                chart1.ChartAreas[0].AxisX.ScrollBar.Enabled  = true;
                chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;

                double xMin = chart1.ChartAreas[0].AxisX.ScaleView.ViewMinimum ;
                double xMax = chart1.ChartAreas[0].AxisX.ScaleView.ViewMaximum ;

                if(e.Location.X > 0)
                { 
                    int posXStart  = (int)chart1.ChartAreas[0].AxisX.PixelPositionToValue(e.Location.X) - (int)(xMax - xMin) / 2 ;
                    int posXFinish = (int)chart1.ChartAreas[0].AxisX.PixelPositionToValue(e.Location.X) + (int)(xMax - xMin) / 2 ;

                    chart1.ChartAreas[0].AxisX.ScaleView.Zoom(posXStart,posXFinish);
                }
            }
        }

        private void FormGraph_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing) e.Cancel = true;
            this.Hide();
            
        }

        public void Chart_AddPoints(string _dX, string _dY)
        {
            if (chart1.InvokeRequired)
            {
                chart1.Invoke(new MethodInvoker(delegate () {
                    

                    int    i1 ; int.TryParse(_dX,out i1); i1 += 5000;
                    double i2 ; double.TryParse(_dY,out i2);

                    if(bSpecial) chart1.Series[0].Points[i1].SetValueY(i2);
                    else         chart1.Series[0].Points.AddXY(_dX,_dY);
                }));
            }
            else
            {
                //chart1.Visible = false;
                int i1 ; int.TryParse(_dX,out i1); i1 += 5000;
                int i2 ; int.TryParse(_dY,out i2);
                       
                if(bSpecial) chart1.Series[0].Points[i1].SetValueY(i2);
                else         chart1.Series[0].Points.AddXY(_dX,_dY);
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
                                    
                    if(bSpecial)
                    {
                        for(int i=0; i<10000; i++)
                        {
                            //chart1.Series[0].Points.AddY(-7000);
                            chart1.Series[0].Points.AddXY(-5000 + i ,-7000);
                        }

                        chart1.ChartAreas[0].AxisX.Minimum = -5000 ;
                        chart1.ChartAreas[0].AxisX.Maximum =  5000 ;
                        //chart1.ChartAreas[0].AxisX.RoundAxisValues = true;

                        chart1.ChartAreas[0].AxisY.Minimum = -5000 ;
                        chart1.ChartAreas[0].AxisY.Maximum =  5000 ;

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

                if(bSpecial)
                {
                    for(int i=0; i<10000; i++)
                    {
                        //chart1.Series[0].Points.AddY(-7000);
                        chart1.Series[0].Points.AddXY(-5000 + i ,-7000);
                    }

                    chart1.ChartAreas[0].AxisX.Minimum = -5000 ;
                    chart1.ChartAreas[0].AxisX.Maximum =  5000 ;
                    //chart1.ChartAreas[0].AxisX.RoundAxisValues = true;

                    chart1.ChartAreas[0].AxisY.Minimum = -5000 ;
                    chart1.ChartAreas[0].AxisY.Maximum =  5000 ;

                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Random rd = new Random();
            Begin();
            for(int i=0; i<3600; i++)
            {
                
                double d1 = rd.Next(0,50);
                Chart_AddPoints(i.ToString(),d1.ToString());
                //CAddPoints1(i.ToString(),d1.ToString());
            }
            End();

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
            //OpenFileDialog fd = new OpenFileDialog();
            //DialogResult dr = fd.ShowDialog();
            //if(dr != DialogResult.OK) return;
            //chart1.SaveImage("d:\\Chart.jpg",ChartImageFormat.Jpeg);
            if (Log.ShowMessageModal("Confirm", "Do you want to Clear?") != DialogResult.Yes) return;
            Chart_Clear();

        }

        private void chart1_Click(object sender, EventArgs e)
        {
            
        }
    }
}
