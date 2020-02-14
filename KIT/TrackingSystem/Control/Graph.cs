using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Control
{
    class CGraph
    {
        private Chart chart1;
        private int iSeriseCnt ;
        private Series[] sSeries ;

        //public List<string> lst;
        public CGraph(Panel _pnBase)
        {
            chart1        = new Chart();
            chart1.Parent = _pnBase;
            chart1.Dock   = DockStyle.Fill;

            iSeriseCnt = 1 ;

            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();

            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(0, 50);
            this.chart1.Name = "chart1";
            // this.chart1.Size = new System.Drawing.Size(284, 212);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
        }

        public void AddSerises(List<string> _sName)
        {
            chart1.Series.Clear();

            Series ee = chart1.Series.Add("sin"); //새로운 series 생성
            ee.ChartType = SeriesChartType.Line; //그래프 모양을 '선'으로 지정
                                                 //데이터 포인트 저장
            for (double k = 0; k < 2 * Math.PI; k += 0.1)
            {
                ee.Points.AddXY(k, Math.Sin(k));
            }

            iSeriseCnt = _sName.Count ;
            sSeries = new Series[iSeriseCnt];
            
            for(int i=0; i<iSeriseCnt; i++)
            {
                sSeries[i] = new Series {
                    Name = _sName[i],
                    Color = System.Drawing.Color.Green,
                    IsVisibleInLegend = false,
                    IsXValueIndexed = true,
                    ChartType = SeriesChartType.Line
                };//(_sName[i]);
                chart1.Series.Add(sSeries[i]);
                sSeries[i].ChartType = SeriesChartType.Line;
            }
        }

        public void AddPoints(int _iNo, double _dX, double _dY)
        {
            if(_iNo >= iSeriseCnt) return;
            sSeries[_iNo].Points.AddXY(_dX,_dY);

        }

    }
}
