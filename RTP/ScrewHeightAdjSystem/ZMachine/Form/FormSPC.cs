using System;
using System.Drawing;
using System.Windows.Forms;
using COMMON;
using SML;
using System.Windows.Forms.DataVisualization.Charting;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace Machine
{
    public partial class FormSPC : Form
    {
        FormRepair FrmRepair = new FormRepair() ;
        FormCsv    FrmCsv    = new FormCsv();
        Random ran = new Random();

        private const string sFormText = "Form Device ";

        string sPath1 , sPath2 ;

        public FormSPC(Panel _pnBase)
        {
            InitializeComponent();

            //string sDate = lvLotDate.Items[Convert.ToInt32(lvLotDate.Items)].SubItems[1].ToString() ;
            this.TopLevel = false;
            this.Parent = _pnBase;
            dpSttTime.Value = DateTime.Now;
            dpEndTime.Value = DateTime.Now;

            //pbChart.Image = new Bitmap(pbChart.Width, pbChart.Height);

            //Chart 
            chartErr1.MouseWheel += new MouseEventHandler(evMouseWheel1);
            //chartLot1.MouseWheel += new MouseEventHandler(evMouseWheel1);
            //chartTrend.MouseWheel += new MouseEventHandler(evMouseWheel1);
            //tmUpdate.Enabled = true;

            

            tcData.TabPages.Remove(tabPage4);
        }

        private void evMouseWheel1(object sender, MouseEventArgs e)
        {
            System.Windows.Forms.DataVisualization.Charting.Chart ct = (System.Windows.Forms.DataVisualization.Charting.Chart)sender ;
            
            if(e.Delta < 0)
            {
                ct.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                ct.ChartAreas[0].AxisY.ScaleView.ZoomReset();
            }
            if(e.Delta > 0)
            {
                ct.Enabled  = false;
                ct.ChartAreas[0].AxisX.ScrollBar.Enabled  = true;
                //ct.ChartAreas[0].AxisX.ScrollBar.BackColor = Color.Silver;
                ct.ChartAreas[0].AxisX.ScaleView.Zoomable = true;

                double xMin = ct.ChartAreas[0].AxisX.ScaleView.ViewMinimum ;
                double xMax = ct.ChartAreas[0].AxisX.ScaleView.ViewMaximum ;
                double yMin = ct.ChartAreas[0].AxisY.ScaleView.ViewMinimum ;
                double yMax = ct.ChartAreas[0].AxisY.ScaleView.ViewMaximum ;

                //if(e.Location.X > 0)
                //{ 
                    double posXStart  = (ct.ChartAreas[0].AxisX.PixelPositionToValue(e.Location.X) - (xMax - xMin) / 2 );
                    double posXFinish = (ct.ChartAreas[0].AxisX.PixelPositionToValue(e.Location.X) + (xMax - xMin) / 2 );
                    double x1 = posXStart ;//Convert.ToDouble(posXStart.ToString("N2"));
                    double x2 = posXFinish;//Convert.ToDouble(posXFinish.ToString("N2"));
                    if(x1 != 0 && x2 != 0) ct.ChartAreas[0].AxisX.ScaleView.Zoom(x1,x2);
                    Log.Trace(posXStart.ToString());
                    Log.Trace(posXFinish.ToString());
                //}
                //if(e.Location.Y > 0)
                //{ 
                    double posYStart  = (ct.ChartAreas[0].AxisY.PixelPositionToValue(e.Location.Y) - (yMax - yMin) / 2 );
                    double posYFinish = (ct.ChartAreas[0].AxisY.PixelPositionToValue(e.Location.Y) + (yMax - yMin) / 2 );
                    double y1 = posYStart ;//Convert.ToDouble(posYStart.ToString("N2"));
                    double y2 = posYFinish;//Convert.ToDouble(posYFinish.ToString("N2"));
                    if(y1 != 0 && y2 != 0) ct.ChartAreas[0].AxisY.ScaleView.Zoom(y1,y2);
                    Log.Trace(posYStart.ToString());
                    Log.Trace(posYFinish.ToString());
                //}
                ct.Enabled  = true;
            }
        }


        private void FormSPC_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmUpdate.Enabled = false;
        }

        //원래 이걸 데이터 단에서 하면 좋은데.
        //로딩시에 몇번해야 하는 문제 있어.
        //바꿈.
        private double GetLotUptime()
        {
            int iColRunTime     = -1 ;
            int iColDownTime    = -1 ;
            int iColFailureTime = -1 ;

            for (int c = 0; c < lvLot.Columns.Count; c++)
            {
                if (lvLot.Columns[c].Text == "RunTime")
                {
                    iColRunTime = c ;
                }
                if (lvLot.Columns[c].Text == "DownTime")
                {
                    iColDownTime = c ;
                }
                if (lvLot.Columns[c].Text == "FailureTime")
                {
                    iColFailureTime = c ;
                }
            }

            TimeSpan Span        = new TimeSpan();
            TimeSpan RunTime     = new TimeSpan();
            TimeSpan DownTime    = new TimeSpan();
            TimeSpan FailureTime = new TimeSpan();
            for (int r = 0; r < lvLot.Items.Count; r++)
            {
                if(!SPC.LOT.TryParse (lvLot.Items[r].SubItems[iColRunTime    ].Text ,out Span)) return 0.0;
                RunTime  += Span ;                                           
                if(!SPC.LOT.TryParse (lvLot.Items[r].SubItems[iColDownTime   ].Text ,out Span)) return 0.0;
                DownTime += Span ;
                if(!SPC.LOT.TryParse (lvLot.Items[r].SubItems[iColFailureTime].Text ,out Span)) return 0.0;
                FailureTime += Span ;
            }
            double dRetTime = 0 ;
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
            if(lvError.Items.Count < 2) return 0.0 ; //2개이상 되어야 간격을 계산 할 수 있다.
            //TErrData[] Datas = new TErrData[iDatsCnt];
            //LoadDataIni(_tSttData, _tEndData, Datas);
            int iColStartedAt = -1 ;
            for (int c = 0; c < lvError.Columns.Count; c++)
            {
                if (lvError.Columns[c].Text == "StartedAt")
                {
                    iColStartedAt = c ;
                }
            }
            //간격을 보는 거라 한번 덜 한다.
            DateTime Time1 ;
            DateTime Time2 ;
            TimeSpan Span = new TimeSpan() ;

            if(!DateTime.TryParse (lvError.Items[0                    ].SubItems[iColStartedAt].Text ,out Time1)) return 0.0;
            if(!DateTime.TryParse (lvError.Items[lvError.Items.Count-1].SubItems[iColStartedAt].Text ,out Time2)) return 0.0;
            Span  = Time2 - Time1 ;  
            double dRet = 0 ;
            
            if((lvError.Items.Count-1) != 0){dRet = Span.TotalMinutes / (lvError.Items.Count-1) ;}
            return dRet ;
        }

        //Mean Time to Assist 쨈푸는 평균 시간.
        private double GetMTTA()
        {
            if(lvError.Items.Count < 1) return 0.0 ; //1개이상 되어야 간격을 계산 할 수 있다.
            //TErrData[] Datas = new TErrData[iDatsCnt];
            //LoadDataIni(_tSttData, _tEndData, Datas);
            int iColErrTime = -1 ;
            for (int c = 0; c < lvError.Columns.Count; c++)
            {
                if (lvError.Columns[c].Text == "ErrTime")
                {
                    iColErrTime = c ;
                }
            }
            //간격을 보는 거라 한번 덜 한다.
            TimeSpan Span = new TimeSpan() ;
            double dRet = 0.0 ;
            for (int r = 0; r < lvError.Items.Count; r++)
            {
                if(!SPC.ERR.TryParse (lvError.Items[r].SubItems[iColErrTime].Text ,out Span)) return 0.0;
                dRet += Span.TotalSeconds ;            
            }
            
            if(lvError.Items.Count != 0) {dRet = dRet/(double)lvError.Items.Count ;}
            else                         {dRet = 0.0 ; }
            return dRet ;
        }

        //평균 수리간격.Mean Time Between Assist
        private double GetMTBF()
        {
            
            if(lvFailure.Items.Count < 2) return 0.0 ; //2개이상 되어야 간격을 계산 할 수 있다.
            //TErrData[] Datas = new TErrData[iDatsCnt];
            //LoadDataIni(_tSttData, _tEndData, Datas);
            int iColStartedAt = -1 ;
            for (int c = 0; c < lvFailure.Columns.Count; c++)
            {
                if (lvFailure.Columns[c].Text == "StartedAt")
                {
                    iColStartedAt = c ;
                }
            }
            //간격을 보는 거라 한번 덜 한다.
            DateTime Time1 ;
            DateTime Time2 ;
            TimeSpan Span = new TimeSpan() ;

            if(!DateTime.TryParse (lvFailure.Items[0                      ].SubItems[iColStartedAt].Text ,out Time1)) return 0.0;
            if(!DateTime.TryParse (lvFailure.Items[lvFailure.Items.Count-1].SubItems[iColStartedAt].Text ,out Time2)) return 0.0;
            Span = Time2 - Time1 ;  

            double dRet = 0.0;
            if((lvFailure.Items.Count-1) != 0 ) {dRet = Span.TotalHours / (lvFailure.Items.Count-1) ;}

            return dRet ;
        }

        //Mean Time to Repair 평균수리시간.
        private double GetMTTR()
        {
            if(lvFailure.Items.Count < 1) return 0.0 ;
            //TErrData[] Datas = new TErrData[iDatsCnt];
            //LoadDataIni(_tSttData, _tEndData, Datas);
            int iColErrTime = -1 ;
            for (int c = 0; c < lvFailure.Columns.Count; c++)
            {
                if (lvFailure.Columns[c].Text == "FailureTime")
                {
                    iColErrTime = c ;
                }
            }
            //간격을 보는 거라 한번 덜 한다.
            TimeSpan Span = new TimeSpan() ;
            double dRet = 0.0 ;
            for (int r = 0; r < lvFailure.Items.Count; r++)
            {
                if(!SPC.ERR.TryParse (lvFailure.Items[r].SubItems[iColErrTime].Text ,out Span)) return 0.0;
                dRet += Span.TotalMinutes ;            
            }

            if(lvFailure.Items.Count!=0) {dRet = dRet/(double)lvFailure.Items.Count ;}
            else                         {dRet = 0;}
            return dRet ;
        }
        
        //Chart TrendChart = new Chart();
        //잘 안되네 하다 매번 귀찮을거 같아서 포기.
        public void PaintTrendChart(DateTime _dtStart , DateTime _dtEnd) 
        {
            return;
            /*
            if(lvLot.Items.Count < 1) return ;
            //chartTrend.Series[0].Enabled = false;
            List<int> lst1 = new List<int>();
            List<DateTime> lst2 = new List<DateTime>();
            string sElement1 = "RejectMark" ; List<int> lst3 = new List<int>();
            string sElement2 = "RejectVisn" ; List<int> lst4 = new List<int>();

            chartTrend.Series.Clear();

            for (int c = 0; c < lvLot.Columns.Count; c++)
            {
                if (lvLot.Columns[c].Text == sElement1 ) { lst1.Add(c); chartTrend.Series.Add(sElement1); };
                if (lvLot.Columns[c].Text == sElement2 ) { lst1.Add(c); chartTrend.Series.Add(sElement2); };
            }
 
            for(int i=0; i<chartTrend.Series.Count; i++)
            {
                chartTrend.Series[i].ChartType = SeriesChartType.Column;
                chartTrend.Series[i].ChartArea = "ChartArea1";
                chartTrend.Series[i].Enabled = true;
                chartTrend.Series[i].Points.Clear();
                chartTrend.Series[i].XValueType = ChartValueType.DateTime;
            }

            DateTime Date = new DateTime();
            int iCnt = 0;
            for (int r = 0; r < lvLot.Items.Count; r++)
            {
                if (DateTime.TryParse (lvLot.Items[r].SubItems[3].Text , out Date)) lst2.Add(Date);
                if (int.TryParse(lvLot.Items[r].SubItems[lst1[0]].Text , out iCnt)) lst3.Add(iCnt);
                if (int.TryParse(lvLot.Items[r].SubItems[lst1[1]].Text , out iCnt)) lst4.Add(iCnt);
            }
            
            chartTrend.ChartAreas.SuspendUpdates();
            chartLot1.ChartAreas[0].AxisX.LabelStyle.Format = "yyyy-MM-dd";
            chartLot1.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Days;
            //chartLot1.ChartAreas[0].AxisX.Interval = 1;

            chartTrend.ChartAreas[0].AxisX.Minimum = _dtStart.ToOADate();
            chartTrend.ChartAreas[0].AxisX.Maximum = _dtEnd.ToOADate();

            chartLot1.ChartAreas[0].AxisX.Interval = 1;
            for(int i=0; i<lst3.Count; i++) {
                if(chartTrend.Series[0].Points.Count > i) chartTrend.Series[0].Points.AddXY(lst2[i],chartTrend.Series[0].Points[i].YValues[0] + lst3[i]);
                else                                      chartTrend.Series[0].Points.AddXY(lst2[i],lst3[i]);
                if(chartTrend.Series[1].Points.Count > i) chartTrend.Series[1].Points.AddXY(lst2[i],chartTrend.Series[1].Points[i].YValues[0] + lst4[i]);
                else                                      chartTrend.Series[1].Points.AddXY(lst2[i],lst4[i]);
            }

            chartTrend.ChartAreas.ResumeUpdates();
            
            //chartLot.ChartAreas[0].AxisY.IntervalOffsetType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Seconds;
            //chartLot.ChartAreas[0].AxisY.Interval = 1 ;
            //chartLot.ChartAreas[0].AxisY.IntervalOffset = 0d;
            
            //chartLot1.ChartAreas[0].AxisX.Interval = 1 ;
                  
            */ 

        }

        public enum CLot
        {
            LotNo       ,
            Device      ,
            StartedAt   ,
            EndedAt     ,
            RunTime     ,
            DownTime    ,
            IdleTime    ,
            FailureTime ,
            
        }

        public void UpdateChartLot() 
        {
            //chartLot1 - Column Lot Time
            //chartLot2 - Circle Total Time
        
            //Setting
            chartLot1.Series[0].Name = "RunTime"    ;
            chartLot1.Series[1].Name = "DownTime"   ;
            chartLot1.Series[2].Name = "IdleTime"   ;
            chartLot1.Series[3].Name = "FailureTime";
        
            for(int i=0; i<4; i++) chartLot1.Series[i].YValueType = ChartValueType.Time; 
            
            chartLot1.ChartAreas[0].AxisY.IntervalType = DateTimeIntervalType.Seconds;
            //chartLot.ChartAreas[0].AxisY.IntervalOffsetType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Seconds;
            //chartLot.ChartAreas[0].AxisY.Interval = 1 ;
            //chartLot.ChartAreas[0].AxisY.IntervalOffset = 0d;
            chartLot1.ChartAreas[0].AxisY.LabelStyle.Format = "HH:mm:ss";
            chartLot1.ChartAreas[0].AxisX.Interval = 1 ;
        
            //Default 
            if(lvLot.Items.Count < 1) {
                for(int i=0; i<4; i++) chartLot1.Series[i].Points.Clear();
                chartLot2.Series[0].Points.Clear();
                
                DateTime dt ;
                for(int i=0; i<4; i++)
                {
                    DateTime.TryParse("04:05:06",out dt);
                    chartLot1.Series[0].Points.AddXY(i,dt);
                    chartLot1.Series[1].Points.AddXY(i,dt);
                    chartLot1.Series[2].Points.AddXY(i,dt);
                    chartLot1.Series[3].Points.AddXY(i,dt);
                
                    chartLot2.Series[0].Points.AddXY("LOT NO " + i.ToString(),10);
                }
                return ;
            }
        
            //Update
            chartLot1.ChartAreas.SuspendUpdates();
            chartLot2.ChartAreas.SuspendUpdates();
            //chartLot.Series[0].Points.SuspendUpdates();
            for(int i=0; i<4; i++) chartLot1.Series[i].Points.Clear();
            chartLot2.Series[0].Points.Clear();
        
            List<string> lst1 = new List<string>();
            List<string> lst2 = new List<string>();
            
            for (int r = 0; r < lvLot.Items.Count; r++)
            {
                //lst1.Add(lvLot.Items[r].SubItems[1].Text); //LotNo
                lst1.Clear();
                lst2.Clear();
                lst1.Add(lvLot.Items[r].SubItems[5].Text); //RunTime
                lst1.Add(lvLot.Items[r].SubItems[6].Text); //DownTime
                lst1.Add(lvLot.Items[r].SubItems[7].Text); //IdleTime
                lst1.Add(lvLot.Items[r].SubItems[8].Text); //FailureTime
                                                    
                lst2.Add(lvLot.Items[r].SubItems[3].Text); //StartedAt
                lst2.Add(lvLot.Items[r].SubItems[4].Text); //EndedAt
        
                DateTime dt ;
                DateTime dt1, dt2 ;
                int i = 0;
                lst1.ForEach(delegate(String name)
                {
                    dt = Convert.ToDateTime(name.Substring(3,name.Length-3)); //앞에 dd. 이있는데 변환이 안되서
                    chartLot1.Series[i++].Points.AddXY(r.ToString(),dt); 
        
                });
                dt1 = Convert.ToDateTime(lst2[0].Substring(3,lst2[0].Length-3));
                dt2 = Convert.ToDateTime(lst2[1].Substring(3,lst2[1].Length-3));
        
                TimeSpan ts = dt2 - dt1 ;
                //chartLot2.Series[0].Points.AddXY(r.ToString(),ts.TotalSeconds); 
                chartLot2.Series[0].Points.AddXY(r,ts.TotalSeconds); 
                chartLot2.Series[0].Points[r].LegendText = r.ToString() + " " + ts.ToString();//.ToString("HH:mm:ss");
            }
        
            chartLot1.ChartAreas.ResumeUpdates();
            chartLot2.ChartAreas.ResumeUpdates();
        }

        public void UpdateChartErr()
        {
            //chartErr1 - Column Lot Time
            //chartErr2 - Circle Total Time
            List<DateTime> lst1 = new List<DateTime>();
            List<int> lst2 = new List<int>();

            string sDateTime = "2000-01-01 00:00:00";

            for (int i=0; i<(int)ei.MAX_ERR; i++)
            {
                lst1.Add(Convert.ToDateTime(sDateTime));
                lst2.Add(0);
            }

            chartErr1.Series[0].Name = "Time"    ;
            chartErr1.ChartAreas[0].AxisY.IntervalType = DateTimeIntervalType.Seconds;
            chartErr1.ChartAreas[0].AxisY.Minimum  = Convert.ToDateTime(sDateTime).ToOADate() ;
            chartErr1.ChartAreas[0].AxisY.LabelStyle.Format = "HH:mm:ss";
            chartErr1.ChartAreas[0].AxisX.Interval = 1 ;

            chartErr1.Series[0].Points.Clear();
            chartErr2.Series[0].Points.Clear();

            //Default 
            if(lvError.Items.Count < 1) {
                for(int i=1; i<5; i++)
                {
                    chartErr1.Series[0].Points.AddXY(i.ToString(),Convert.ToDateTime(sDateTime).AddHours(i));
                    //chartErr1.Series[0].Points[i].LegendText = "Err No : " + i.ToString() + " Err Cnt : " +  (i*10).ToString() ;
                    //string sDateTime1 = "2018-01-16 17:37:10";7//
                    chartErr2.Series[0].Points.AddXY("ErrNo"+i.ToString(),i*10);
                }
                return ;
            }

            //chartErr1.ChartAreas[0].AxisY.Minimum  = Convert.ToDateTime(sDateTime).ToOADate() ;

            //Update
            chartErr1.ChartAreas.SuspendUpdates();
            chartErr2.ChartAreas.SuspendUpdates();
          
            DateTime dt;
            DateTime dt1;
            for (int r = 0; r < lvError.Items.Count; r++)
            {
                int iNo ;
                if (!int.TryParse(lvError.Items[r].SubItems[1].Text , out iNo)) return;
                
                string sTime = lvError.Items[r].SubItems[5].Text; //Error Time
                dt  = Convert.ToDateTime(sTime.Substring(3,sTime.Length-3));
                dt1 = lst1[iNo] ;

                lst1[iNo] = dt1.AddHours  (dt.Hour   );
                lst1[iNo] = dt1.AddMinutes(dt.Minute );
                lst1[iNo] = dt1.AddSeconds(dt.Second );

                lst2[iNo]++;
            }

            int iLT1 = 0;
            int iLT2 = 0;
            for(int i=0; i<(int)ei.MAX_ERR; i++)
            {
                if(lst2[i] != 0) {
                    //chartErr2.Series[0].Points.AddXY(i.ToString(),lst2[i]);
                    chartErr2.Series[0].Points.AddXY(i,lst2[i]);
                    chartErr2.Series[0].Points[iLT1++].LegendText = "ErrNo : " + string.Format("{0:000}",i) + " ErrCnt : " + string.Format("{0:000}",lst2[i]) ;
                    //chartErr1.Series[0].Points.AddXY(i,lst2[i]);
                }
                if(lst1[i].Hour != 0 || lst1[i].Minute!= 0 || lst1[i].Second != 0 )
                {
                    chartErr1.Series[0].Points.AddXY(i.ToString(),lst1[i]);
                    //chartErr1.Series[0].Points[iLT2].LegendText = i.ToString() + " " + lst1[i] ;
                    //chartErr1.Legends[0].CellColumns.Add((i.ToString() + " " + lst1[i]),LegendCellColumnType.Text,"");
                    iLT2++;
                }
            }

            chartErr1.ChartAreas.ResumeUpdates();
            chartErr2.ChartAreas.ResumeUpdates();
        }

        public string GetRandomStr(int _iLength)
        {
            const string sChar = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            string sRet = "";
            for(int i = 0 ; i < _iLength ; i++){
                sRet += sChar[ran.Next(0, sChar.Length-1)];
            }
             
            return sRet ;
        }
        
        public int GetRandomInt(int _iLength)
        {
            const string sChar = "0123456789";
            

            string sRet = "";
            for(int i = 0 ; i < _iLength ; i++){
                sRet += sChar[ran.Next(0, sChar.Length-1)];
            }
             
            int iRet = 0;
            int.TryParse(sRet ,out iRet);
            return iRet  ;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //SPC.LOT.Data.Device = RandomStr().toString();

            SPC.LOT.Data.LotNo         = GetRandomStr(8);
                                        
            //SPC.LOT.Data.TrayNo        = GetRandomStr(6);
            SPC.LOT.Data.Device        = GetRandomStr(8);
            SPC.LOT.Data.StartedAt     = DateTime.Now.AddSeconds(-10).ToOADate();
            SPC.LOT.Data.EndedAt       = DateTime.Now.ToOADate();

            SPC.LOT.Data.RunTime       = DateTime.Now.ToOADate() - DateTime.Now.AddHours  (-7.3333333111 ).ToOADate();
//            SPC.LOT.Data.RunTime       = 
            SPC.LOT.Data.DownTime      = DateTime.Now.ToOADate() - DateTime.Now.AddHours  (-3 ).ToOADate();
            SPC.LOT.Data.IdleTime      = DateTime.Now.ToOADate() - DateTime.Now.AddHours  (-2 ).ToOADate();
            SPC.LOT.Data.FailureTime   = DateTime.Now.ToOADate() - DateTime.Now.AddHours  (-10).ToOADate();
                                       
            //SPC.LOT.Data.WorkCnt1       = 7860;
            //SPC.LOT.Data.WorkCnt2       = 5620;

            SPC.LOT.SaveDataIni();

        }
        
        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            tmUpdate.Enabled = false;
            
            btLotDelete .Enabled = SM.FrmLogOn.GetLevel() > EN_LEVEL.Operator ;
            btErrDelete .Enabled = SM.FrmLogOn.GetLevel() > EN_LEVEL.Operator ;
            btFailDelete.Enabled = SM.FrmLogOn.GetLevel() > EN_LEVEL.Operator ;

            btSetRepair .Enabled = SM.FrmLogOn.GetLevel() > EN_LEVEL.Operator ;

            if (!this.Visible)
            {
                tmUpdate.Enabled = false;
                return;
            }
            tmUpdate.Enabled = true;
        }


        private void btDataView_Click_1()
        {
            if (dpSttTime.Value > dpEndTime.Value)
            {
                Log.ShowMessage("ERROR", "The StartTime Must be earlyer than EndTime!");
                return;
            }
            SPC.LOT.DispData(dpSttTime.Value, dpEndTime.Value, lvLot);
            //lbUptime.Text = string.Format("Uptime={0:0.000}%", GetLotUptime());

            SPC.ERR.DispData(dpSttTime.Value, dpEndTime.Value, lvError);
            //lbMTBA.Text = string.Format("MTBA={0:0.000}min", GetMTBA());
            //lbMTTA.Text = string.Format("MTTA={0:0.000}sec", GetMTTA());

            SPC.FLR.DispData(dpSttTime.Value, dpEndTime.Value, lvFailure);
            //lbMTBF.Text = string.Format("MTBF={0:0.000}hour", GetMTBF());
            //lbMTTR.Text = string.Format("MTTR={0:0.000}min", GetMTTR());

            PaintTrendChart(dpSttTime.Value, dpEndTime.Value);

            ChangeText();
            //UpdateChartLot();
            UpdateChartErr();
            
        }
        private void btDataView_Click_1(object sender, EventArgs e)
        {
            btDataView_Click_1();
        }

        private void btLotSave_Click_1(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ForContext.Frm);

            SPC.LOT.SaveCsv(lvLot);
        }

        private void btErrSave_Click_1(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ForContext.Frm);

            SPC.ERR.SaveCsv(lvError);
        }

        private void btFailSave_Click_1(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ForContext.Frm);

            SPC.FLR.SaveCsv(lvFailure);
        }

        private void btLotFind_Click_1(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ForContext.Frm);

            int iSel = Convert.ToInt32(((Button)sender).Tag);

            string sFindText;
            ListView lvListView;
            if (iSel == 1)//Lot
            {
                sFindText = tbLotFind.Text;
                lvListView = lvLot;
            }
            else if (iSel == 2)
            {
                sFindText = tbErrFind.Text;
                lvListView = lvError;
            }
            else
            {
                sFindText = tbFailFind.Text;
                lvListView = lvFailure;
            }

            lvListView.Focus();
            for (int r = 0; r < lvListView.Items.Count; r++)
            {
                for (int c = 0; c < lvListView.Columns.Count; c++)
                {
                    if (lvListView.Items[r].SubItems[c].Text.IndexOf(sFindText) >= 0)
                    {
                        lvListView.Items[r].Selected = true;
                        break;
                    }
                    else
                    {
                        lvListView.Items[r].Selected = false;
                    }
                }
            }
        }

        private void btLotDelete_Click_1(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ForContext.Frm);

            int iSel = Convert.ToInt32(((Button)sender).Tag);

            if (iSel == 1)//Lot
            {
                if (lvLot.SelectedItems.Count == 0) return;
                if (Log.ShowMessageModal("Confirm", "Would you like to delete selected " + lvLot.SelectedItems.Count + "Items?") != DialogResult.Yes) return;
                SPC.LOT.DeleteSelItems(lvLot);
                SPC.LOT.DispData(dpSttTime.Value, dpEndTime.Value, lvLot);
            }
            else if (iSel == 2)
            {
                if (lvError.SelectedItems.Count == 0) return;
                if (Log.ShowMessageModal("Confirm", "Would you like to delete selected " + lvError.SelectedItems.Count + "Items?") != DialogResult.Yes) return;
                SPC.ERR.DeleteSelItems(lvError);
                SPC.ERR.DispData(dpSttTime.Value, dpEndTime.Value, lvError);
            }
            else
            {
                if (lvFailure.SelectedItems.Count == 0) return;
                if (Log.ShowMessageModal("Confirm", "Would you like to delete selected " + lvFailure.SelectedItems.Count + "Items?") != DialogResult.Yes) return;
                SPC.FLR.DeleteSelItems(lvFailure);
                SPC.FLR.DispData(dpSttTime.Value, dpEndTime.Value, lvFailure);
            }
        }

        private void btSetRepair_Click_1(object sender, EventArgs e)
        {
            FrmRepair.ShowDialog(this);
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            Random rd = new Random();
            //SPC.LOT.Data.Device = RandomStr().toString();

            SPC.LOT.Data.LotNo = GetRandomStr(8);

            //SPC.LOT.Data.TrayNo = GetRandomStr(6);
            SPC.LOT.Data.Device = GetRandomStr(8);
            SPC.LOT.Data.StartedAt = DateTime.Now.AddSeconds(-rd.Next(0,60)).ToOADate();
            SPC.LOT.Data.EndedAt = DateTime.Now.ToOADate();
            
            
            TimeSpan Span ;
            Span = TimeSpan.FromHours(DateTime.Now.AddHours(-rd.Next(0,10)).ToOADate());

            DateTime dt = DateTime.Now.AddHours(-rd.Next(0,10));


            SPC.LOT.Data.RunTime     = DateTime.Now.AddHours(-rd.Next(0,10)).Hour *1000000 ;        
            SPC.LOT.Data.DownTime    = DateTime.Now.AddHours(-rd.Next(0,10)).Hour *1000000 ;        
            SPC.LOT.Data.IdleTime    = DateTime.Now.AddHours(-rd.Next(0,10)).Hour *1000000 ;        
            SPC.LOT.Data.FailureTime = DateTime.Now.AddHours(-rd.Next(0,10)).Hour *1000000 ;        

            //SPC.LOT.Data.WorkCnt1 = 60;
            //SPC.LOT.Data.WorkCnt2 = 260;

            //SPC.LOT.Data.LotNo.
            //SPC.LOT.Data.MixDevice = GetRandomInt(1);
            //SPC.LOT.Data.UnitID = GetRandomInt(1);
            //SPC.LOT.Data.UnitDMC1 = GetRandomInt(1);
            //SPC.LOT.Data.UnitDMC2 = GetRandomInt(1);
            //SPC.LOT.Data.GlobtopLeft = GetRandomInt(1);
            //SPC.LOT.Data.GlobtopTop = GetRandomInt(1);
            //SPC.LOT.Data.GlobtopRight = GetRandomInt(1);
            //SPC.LOT.Data.GlobtopBottom = GetRandomInt(1);
            //SPC.LOT.Data.Empty = GetRandomInt(1);

            SPC.LOT.SaveDataIni();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            //SPC.LOT.Data.Device = RandomStr().toString();

            //SPC.ERR.Data.LotNo         = GetRandomStr(8);

            Random ran = new Random();

            ML.ER_SetErr((ei)(ran.Next(0, (int)ei.MAX_ERR - 1)), "sunsunsunsun");


            //SPC.LOT.SaveDataIni();
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            SEQ.Reset();
        }

        private void FormSPC_Shown(object sender, EventArgs e)
        {
            tmUpdate.Enabled = true;

            
        }

        private void FormSPC_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible) {
                tmUpdate.Enabled = true;
                btDataView_Click_1();
            }
        }

        private void tbLotFind_TextChanged(object sender, EventArgs e)
        {

        }

        private void ChangeText()
        {
                 if(tcData.SelectedIndex == 0) lbUptime.Text = string.Format("UPTIME={0:0.000}%",GetLotUptime()) ;
            else if(tcData.SelectedIndex == 1) lbUptime.Text = string.Format("MTBA={0:0.000}min",GetMTBA()) + " " + string.Format("MTTA={0:0.000}sec",GetMTTA()) ;
            else if(tcData.SelectedIndex == 2) lbUptime.Text = string.Format("MTBF={0:0.000}hour",GetMTBF()) + " " + string.Format("MTTR={0:0.000}min" ,GetMTTR()) ;
            else lbUptime.Text = "";

        }
        private void tcData_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeText();
        }

        private void lvLot_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void lvLot_Click(object sender, EventArgs e)
        {
           
        }

        private void lvLot_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(lvLot.SelectedItems.Count <= 0) return;
            ListViewItem item = lvLot.SelectedItems[lvLot.SelectedItems.Count-1];
            
            DateTime tDateTime = DateTime.Parse(item.SubItems[3].Text);
            double dSttTime = tDateTime.ToOADate();
            string sLotNo   = item.SubItems[1].Text;
            
        }

        private void lvLot_MouseUp(object sender, MouseEventArgs e)
        {

            ListViewHitTestInfo i = lvLot.HitTest(e.X, e.Y);
            ListViewItem.ListViewSubItem SelectedLSI = i.SubItem;
            if (SelectedLSI == null) return;
            
            int row = i.Item.Index;
            int col = i.Item.SubItems.IndexOf(i.SubItem);

            ListViewItem item = lvLot.Items[row];
            DateTime tDateTime;

            if (item.SubItems.Count > 4)
            {
                sPath2 = item.SubItems[1].Text;
                tDateTime = DateTime.Parse(item.SubItems[3].Text);
                sPath1 = tDateTime.ToString("yyyyMMdd");

                string sPath = System.IO.Directory.GetParent(SPC.LOT.Folder) + "\\" + sPath1 + "\\" + sPath2 +".ini" ;
                if(!File.Exists(sPath)) {
                    lbLotDataID.Text = "LOT DATA";
                    lvLotData.Clear();
                    return ;
                }

                lbLotDataID.Text = "LOT DATA ( " + sPath2 + " )" ;

                SPC.SUB.DispLotData(sPath,lvLotData);

                
            }

            //if(e.Button == MouseButtons.Left)
            //{
            //    FrmSPC_Sub.InitDirTreeView(sPath1, sPath2);
            //    FrmSPC_Sub.Show();
            //    FrmSPC_Sub.Focus();
            //}

            if(e.Button == MouseButtons.Right)
            {
                //기존에 탐색기에서 열기.
                string sRootPath = System.IO.Directory.GetParent(SPC.LOT.Folder).Parent.FullName.ToString() + "\\DataMap\\";
                DirectoryInfo di = new DirectoryInfo(sRootPath + sPath1 + "\\" + sPath2);
                if (di.Exists == false) return;
                Process.Start(di.FullName);
            }
            //    OpenFileDialog fd = new OpenFileDialog();
            //    fd.InitialDirectory = SPC.LOT.Folder + sPath1 + "\\" + sPath2;
            //    fd.Filter = "csv File|*.csv";
            //    DialogResult Rslt = fd.ShowDialog();
            //    if (Rslt != DialogResult.OK) return;
            //    //FormCsv f = new FormCsv();
            //    FrmCsv.Load(fd.FileName);
            //    if(FrmCsv.IsDisposed) FrmCsv.Show();
            //}
        }

    }
}





/*
 
 private void dtTolistView(DataTable dt, ListView lvw) { // 테이블 Row가 없을때 if (dt.Rows.Count <= 0) { lvw.Clear(); } else { foreach (DataRow dr in dt.Rows) { ListViewItem lvwi = new ListViewItem(); lvwi.Text = dr[0].ToString(); for (int i = 1; i < dt.Columns.Count; i++) { lvwi.SubItems.Add(dr[i].ToString()); } lvw.Items.Add(lvwi); } } }

출처: http://sociophobia.tistory.com/8 [Devch]
 */