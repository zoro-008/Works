using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using COMMON;
using SML;

namespace Machine
{
    public partial class FormSPC : Form
    {
        FormRepair FrmRepair = new FormRepair() ;

        Random ran = new Random();

        private const string sFormText = "Form Device ";

        public FormSPC(Panel _pnBase)
        {
            InitializeComponent();

            //string sDate = lvLotDate.Items[Convert.ToInt32(lvLotDate.Items)].SubItems[1].ToString() ;
            this.TopLevel = false;
            this.Parent = _pnBase;
            dpSttTime.Value = DateTime.Now;
            dpEndTime.Value = DateTime.Now;

            pbChart.Image = new Bitmap(pbChart.Width, pbChart.Height);

            tcData.TabPages.Remove(tabPage4);//Chart 탭 제거

            //tmUpdate.Enabled = true;
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

        //private void GetRejectList(string _sRejectName , ref List<int> _lsReject)
        //{
        //    /*
        //    WorkCnt      
        //    MixDevice    
        //    UnitID       
        //    UnitDMC1     
        //    UnitDMC2     
        //    GlobtopLeft  
        //    GlobtopTop   
        //    GlobtopRight 
        //    GlobtopBottom
        //    Empty        
        //    */
        //    _lsReject.Clear();
        //    if(lvLot.Items.Count < 1) return ;
        //    int iColReject = -1 ;
        //    int iColStartedAt = -1 ;
        //    for (int c = 0; c < lvLot.Columns.Count; c++)
        //    {
        //        if (lvLot.Columns[c].Text == _sRejectName)
        //        {
        //            iColReject = c ;
        //        }
        //        if (lvLot.Columns[c].Text == "StartedAt")
        //        {
        //            iColStartedAt = c ;
        //        }
        //    }
        //    //간격을 보는 거라 한번 덜 한다.
        //    int iRslt = 0; 
        //    int iDaySum = 0;
        //    DateTime iDate = new DateTime();
        //    string sDate = "";
        //    for (int r = 0; r < lvLot.Items.Count; r++)
        //    {
        //        if(!DateTime.TryParse (lvFailure.Items[r].SubItems[iColStartedAt].Text ,out iDate)) return ;

        //        if(!int.TryParse (lvLot.Items[r].SubItems[iColReject].Text ,out iRslt)) {
        //            _lsReject.Add(0) ;   
        //        }
        //        _lsReject.Add(iRslt) ;         
        //    }
             
        //}

        Chart TrendChart = new Chart();
        public void PaintTrendChart(DateTime _dtStart , DateTime _dtEnd) 
        {
            if(lvLot.Items.Count < 1) return ;
            
            int iColStartedAt = -1 ;                                              
            Chart.Item lsWorkCnt       = new Chart.Item(); lsWorkCnt      .Name = "WorkCnt"      ; lsWorkCnt      .XUnit = 1 ; lsWorkCnt      .LineColor = Color.Black        ; int iColWorkCnt       = -1 ; int iDayWorkCnt       = 0 ;
            Chart.Item lsMixDevice     = new Chart.Item(); lsMixDevice    .Name = "MixDevice"    ; lsMixDevice    .XUnit = 1 ; lsMixDevice    .LineColor = Color.Coral        ; int iColMixDevice     = -1 ; int iDayMixDevice     = 0 ;
            Chart.Item lsUnitID        = new Chart.Item(); lsUnitID       .Name = "UnitID"       ; lsUnitID       .XUnit = 1 ; lsUnitID       .LineColor = Color.DarkOrchid   ; int iColUnitID        = -1 ; int iDayUnitID        = 0 ;
            Chart.Item lsUnitDMC1      = new Chart.Item(); lsUnitDMC1     .Name = "UnitDMC1"     ; lsUnitDMC1     .XUnit = 1 ; lsUnitDMC1     .LineColor = Color.DarkTurquoise; int iColUnitDMC1      = -1 ; int iDayUnitDMC1      = 0 ;
            Chart.Item lsUnitDMC2      = new Chart.Item(); lsUnitDMC2     .Name = "UnitDMC2"     ; lsUnitDMC2     .XUnit = 1 ; lsUnitDMC2     .LineColor = Color.Olive        ; int iColUnitDMC2      = -1 ; int iDayUnitDMC2      = 0 ;
            Chart.Item lsGlobtopLeft   = new Chart.Item(); lsGlobtopLeft  .Name = "GlobtopLeft"  ; lsGlobtopLeft  .XUnit = 1 ; lsGlobtopLeft  .LineColor = Color.DeepSkyBlue  ; int iColGlobtopLeft   = -1 ; int iDayGlobtopLeft   = 0 ;
            Chart.Item lsGlobtopTop    = new Chart.Item(); lsGlobtopTop   .Name = "GlobtopTop"   ; lsGlobtopTop   .XUnit = 1 ; lsGlobtopTop   .LineColor = Color.Crimson      ; int iColGlobtopTop    = -1 ; int iDayGlobtopTop    = 0 ;
            Chart.Item lsGlobtopRight  = new Chart.Item(); lsGlobtopRight .Name = "GlobtopRight" ; lsGlobtopRight .XUnit = 1 ; lsGlobtopRight .LineColor = Color.SlateBlue    ; int iColGlobtopRight  = -1 ; int iDayGlobtopRight  = 0 ;
            Chart.Item lsGlobtopBottom = new Chart.Item(); lsGlobtopBottom.Name = "GlobtopBottom"; lsGlobtopBottom.XUnit = 1 ; lsGlobtopBottom.LineColor = Color.DarkCyan     ; int iColGlobtopBottom = -1 ; int iDayGlobtopBottom = 0 ;
            Chart.Item lsEmpty         = new Chart.Item(); lsEmpty        .Name = "Empty"        ; lsEmpty        .XUnit = 1 ; lsEmpty        .LineColor = Color.Orange       ; int iColEmpty         = -1 ; int iDayEmpty         = 0 ;
            Chart.Item lsMatchingError = new Chart.Item(); lsMatchingError.Name = "MatchingError"; lsMatchingError.XUnit = 1 ; lsMatchingError.LineColor = Color.DarkKhaki    ; int iColMatchingError = -1 ; int iDayMatchingError = 0 ;
            Chart.Item lsUserDefine    = new Chart.Item(); lsUserDefine   .Name = "UserDefine"   ; lsUserDefine   .XUnit = 1 ; lsUserDefine   .LineColor = Color.DarkGoldenrod; int iColUserDefine    = -1 ; int iDayUserDefine    = 0 ;
                  
            for (int c = 0; c < lvLot.Columns.Count; c++)
            {
                if (lvLot.Columns[c].Text == "StartedAt"    ) iColStartedAt     = c ;
                if (lvLot.Columns[c].Text == "WorkCnt"      ) iColWorkCnt       = c ;
                if (lvLot.Columns[c].Text == "MixDevice"    ) iColMixDevice     = c ;
                if (lvLot.Columns[c].Text == "UnitID"       ) iColUnitID        = c ;
                if (lvLot.Columns[c].Text == "UnitDMC1"     ) iColUnitDMC1      = c ;
                if (lvLot.Columns[c].Text == "UnitDMC2"     ) iColUnitDMC2      = c ;
                if (lvLot.Columns[c].Text == "GlobtopLeft"  ) iColGlobtopLeft   = c ;
                if (lvLot.Columns[c].Text == "GlobtopTop"   ) iColGlobtopTop    = c ;
                if (lvLot.Columns[c].Text == "GlobtopRight" ) iColGlobtopRight  = c ;
                if (lvLot.Columns[c].Text == "GlobtopBottom") iColGlobtopBottom = c ;
                if (lvLot.Columns[c].Text == "Empty"        ) iColEmpty         = c ;
                if (lvLot.Columns[c].Text == "MatchingError") iColMatchingError = c ;
                if (lvLot.Columns[c].Text == "UserDefine"   ) iColUserDefine    = c ;
            }

            //int iCrntRow = 0 ;
            DateTime Date = new DateTime();
            int iBinCnt = 0 ;
            double dMin = 999;
            double dMax = -1 ;

            if(!DateTime.TryParse (lvLot.Items[0].SubItems[iColStartedAt].Text ,out Date)) return ;

            for (DateTime day = _dtStart; day <= _dtEnd; day = day.AddDays(1))
            {                
                for(int r = 0 ; r < lvLot.Items.Count ; r++ )
                {
                    if(!DateTime.TryParse (lvLot.Items[r].SubItems[iColStartedAt].Text ,out Date)) return ;
                    if((int)day.ToOADate() == (int)Date.ToOADate()) {
                        if(int.TryParse(lvLot.Items[r].SubItems[iColWorkCnt      ].Text , out iBinCnt)) iDayWorkCnt       += iBinCnt ;
                        if(int.TryParse(lvLot.Items[r].SubItems[iColMixDevice    ].Text , out iBinCnt)) iDayMixDevice     += iBinCnt ;
                        if(int.TryParse(lvLot.Items[r].SubItems[iColUnitID       ].Text , out iBinCnt)) iDayUnitID        += iBinCnt ;
                        if(int.TryParse(lvLot.Items[r].SubItems[iColUnitDMC1     ].Text , out iBinCnt)) iDayUnitDMC1      += iBinCnt ;
                        if(int.TryParse(lvLot.Items[r].SubItems[iColUnitDMC2     ].Text , out iBinCnt)) iDayUnitDMC2      += iBinCnt ;
                        if(int.TryParse(lvLot.Items[r].SubItems[iColGlobtopLeft  ].Text , out iBinCnt)) iDayGlobtopLeft   += iBinCnt ;
                        if(int.TryParse(lvLot.Items[r].SubItems[iColGlobtopTop   ].Text , out iBinCnt)) iDayGlobtopTop    += iBinCnt ;
                        if(int.TryParse(lvLot.Items[r].SubItems[iColGlobtopRight ].Text , out iBinCnt)) iDayGlobtopRight  += iBinCnt ;
                        if(int.TryParse(lvLot.Items[r].SubItems[iColGlobtopBottom].Text , out iBinCnt)) iDayGlobtopBottom += iBinCnt ;
                        if(int.TryParse(lvLot.Items[r].SubItems[iColEmpty        ].Text , out iBinCnt)) iDayEmpty         += iBinCnt ;
                        if(int.TryParse(lvLot.Items[r].SubItems[iColMatchingError].Text , out iBinCnt)) iDayMatchingError += iBinCnt ;
                        if(int.TryParse(lvLot.Items[r].SubItems[iColUserDefine   ].Text , out iBinCnt)) iDayUserDefine    += iBinCnt ;
                    }
                }
                int iTotalCnt = 0 ; 
                iTotalCnt += iDayWorkCnt       ;
                iTotalCnt += iDayMixDevice     ;
                iTotalCnt += iDayUnitID        ;
                iTotalCnt += iDayUnitDMC1      ;
                iTotalCnt += iDayUnitDMC2      ;
                iTotalCnt += iDayGlobtopLeft   ;
                iTotalCnt += iDayGlobtopTop    ;
                iTotalCnt += iDayGlobtopRight  ;
                iTotalCnt += iDayGlobtopBottom ;
                iTotalCnt += iDayEmpty         ;    
                iTotalCnt += iDayMatchingError ;    
                iTotalCnt += iDayUserDefine    ;    

                double dPerWorkCnt       = 0 ;
                double dPerMixDevice     = 0 ;
                double dPerUnitID        = 0 ;
                double dPerUnitDMC1      = 0 ;
                double dPerUnitDMC2      = 0 ;
                double dPerGlobtopLeft   = 0 ;
                double dPerGlobtopTop    = 0 ;
                double dPerGlobtopRight  = 0 ;
                double dPerGlobtopBottom = 0 ;
                double dPerEmpty         = 0 ;
                double dPerMatchingError = 0 ;
                double dPerUserDefine    = 0 ;

                if(iTotalCnt > 0) {
                    dPerWorkCnt      = iDayWorkCnt      * 100/(double)iTotalCnt;
                    dPerMixDevice    = iDayMixDevice    * 100/(double)iTotalCnt;
                    dPerUnitID       = iDayUnitID       * 100/(double)iTotalCnt;
                    dPerUnitDMC1     = iDayUnitDMC1     * 100/(double)iTotalCnt;
                    dPerUnitDMC2     = iDayUnitDMC2     * 100/(double)iTotalCnt;
                    dPerGlobtopLeft  = iDayGlobtopLeft  * 100/(double)iTotalCnt;
                    dPerGlobtopTop   = iDayGlobtopTop   * 100/(double)iTotalCnt;
                    dPerGlobtopRight = iDayGlobtopRight * 100/(double)iTotalCnt;
                    dPerGlobtopBottom= iDayGlobtopBottom* 100/(double)iTotalCnt;
                    dPerEmpty        = iDayEmpty        * 100/(double)iTotalCnt;
                    dPerMatchingError= iDayMatchingError* 100/(double)iTotalCnt;
                    dPerUserDefine   = iDayUserDefine   * 100/(double)iTotalCnt;
                }

                //if(dMin > dPerWorkCnt      ) dMin = dPerWorkCnt       ; if(dMax < dPerWorkCnt      ) dMax = dPerWorkCnt       ;
                if(dMin > dPerMixDevice    ) dMin = dPerMixDevice     ; if(dMax < dPerMixDevice    ) dMax = dPerMixDevice     ;
                if(dMin > dPerUnitID       ) dMin = dPerUnitID        ; if(dMax < dPerUnitID       ) dMax = dPerUnitID        ;
                if(dMin > dPerUnitDMC1     ) dMin = dPerUnitDMC1      ; if(dMax < dPerUnitDMC1     ) dMax = dPerUnitDMC1      ;
                if(dMin > dPerUnitDMC2     ) dMin = dPerUnitDMC2      ; if(dMax < dPerUnitDMC2     ) dMax = dPerUnitDMC2      ;
                if(dMin > dPerGlobtopLeft  ) dMin = dPerGlobtopLeft   ; if(dMax < dPerGlobtopLeft  ) dMax = dPerGlobtopLeft   ;
                if(dMin > dPerGlobtopTop   ) dMin = dPerGlobtopTop    ; if(dMax < dPerGlobtopTop   ) dMax = dPerGlobtopTop    ;
                if(dMin > dPerGlobtopRight ) dMin = dPerGlobtopRight  ; if(dMax < dPerGlobtopRight ) dMax = dPerGlobtopRight  ;
                if(dMin > dPerGlobtopBottom) dMin = dPerGlobtopBottom ; if(dMax < dPerGlobtopBottom) dMax = dPerGlobtopBottom ;
                if(dMin > dPerEmpty        ) dMin = dPerEmpty         ; if(dMax < dPerEmpty        ) dMax = dPerEmpty         ;
                if(dMin > dPerMatchingError) dMin = dPerMatchingError ; if(dMax < dPerMatchingError) dMax = dPerMatchingError ;
                if(dMin > dPerUserDefine   ) dMin = dPerUserDefine    ; if(dMax < dPerUserDefine   ) dMax = dPerUserDefine    ;

                /*lsWorkCnt      .YDatas.Add(dPerWorkCnt      );*/iDayWorkCnt       = 0 ;
                lsMixDevice    .YDatas.Add(dPerMixDevice    );iDayMixDevice     = 0 ;
                lsUnitID       .YDatas.Add(dPerUnitID       );iDayUnitID        = 0 ;
                lsUnitDMC1     .YDatas.Add(dPerUnitDMC1     );iDayUnitDMC1      = 0 ;
                lsUnitDMC2     .YDatas.Add(dPerUnitDMC2     );iDayUnitDMC2      = 0 ;
                lsGlobtopLeft  .YDatas.Add(dPerGlobtopLeft  );iDayGlobtopLeft   = 0 ;
                lsGlobtopTop   .YDatas.Add(dPerGlobtopTop   );iDayGlobtopTop    = 0 ;
                lsGlobtopRight .YDatas.Add(dPerGlobtopRight );iDayGlobtopRight  = 0 ;
                lsGlobtopBottom.YDatas.Add(dPerGlobtopBottom);iDayGlobtopBottom = 0 ;
                lsEmpty        .YDatas.Add(dPerEmpty        );iDayEmpty         = 0 ;
                lsMatchingError.YDatas.Add(dPerMatchingError);iDayMatchingError = 0 ;
                lsUserDefine   .YDatas.Add(dPerUserDefine   );iDayUserDefine    = 0 ;
            }              
         

            
            int iTemp = 0 ;
            int iYUnit = 5 ;
            if(dMax-dMin < 30)iYUnit = 1 ;
            iTemp = (int)(dMin /iYUnit);
            iTemp = iTemp * iYUnit ;
            dMin = iTemp; //범위를 약간 넓히기 위해.

            iTemp = (int)(dMax  /iYUnit );
            iTemp = (iTemp+1)*iYUnit;
            dMax  = iTemp;
            int iXUnit = 1 ;
            if (_dtEnd.ToOADate() - _dtStart.ToOADate() > 50)
            {
                iXUnit = (int)(_dtEnd.ToOADate() - _dtStart.ToOADate()) / 50 ;
            }
            TrendChart.Clear();
            
            TrendChart.SetBase(_dtStart.ToOADate() , _dtEnd.ToOADate() , iXUnit , dMin , dMax , iYUnit , Chart.XValue.DateTime);

            //TrendChart.AddItem(ref lsWorkCnt      );
            TrendChart.AddItem(ref lsMixDevice    );
            TrendChart.AddItem(ref lsUnitID       );
            TrendChart.AddItem(ref lsUnitDMC1     );
            TrendChart.AddItem(ref lsUnitDMC2     );
            TrendChart.AddItem(ref lsGlobtopLeft  );
            TrendChart.AddItem(ref lsGlobtopTop   );
            TrendChart.AddItem(ref lsGlobtopRight );
            TrendChart.AddItem(ref lsGlobtopBottom);
            TrendChart.AddItem(ref lsEmpty        );
            TrendChart.AddItem(ref lsMatchingError);
            TrendChart.AddItem(ref lsUserDefine   );
            TrendChart.Paint  (pbChart);
        }

        private void btDataView_Click(object sender, EventArgs e)
        {
            if(dpSttTime.Value > dpEndTime.Value) {
                Log.ShowMessage("ERROR" , "The StartTime Must be earlyer than EndTime!");
                return ;
            }
            SPC.LOT.DispData(dpSttTime.Value, dpEndTime.Value, lvLot);
            lbUptime.Text = string.Format("Uptime={0:0.000}%",GetLotUptime()) ;

            SPC.ERR.DispData(dpSttTime.Value, dpEndTime.Value, lvError);
            lbMTBA.Text = string.Format("MTBA={0:0.000}min",GetMTBA()) ;
            lbMTTA.Text = string.Format("MTTA={0:0.000}sec",GetMTTA()) ;

            SPC.FLR.DispData(dpSttTime.Value, dpEndTime.Value, lvFailure);
            lbMTBF.Text = string.Format("MTBF={0:0.000}hour",GetMTBF()) ;
            lbMTTR.Text = string.Format("MTTR={0:0.000}min" ,GetMTTR()) ;

            PaintTrendChart(dpSttTime.Value, dpEndTime.Value) ;
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

            //SPC.LOT.Data.LotNo         = GetRandomStr(8);
            //                            
            //SPC.LOT.Data.TrayNo        = GetRandomStr(6);
            //SPC.LOT.Data.Device        = GetRandomStr(8);
            //SPC.LOT.Data.StartedAt     = DateTime.Now.AddSeconds(-10).ToOADate();
            //SPC.LOT.Data.EndedAt       = DateTime.Now.ToOADate();
            //
            //SPC.LOT.Data.RunTime       = DateTime.Now.ToOADate() - DateTime.Now.AddHours  (-7.3333333111 ).ToOADate();
//          //  SPC.LOT.Data.RunTime       = 
            //SPC.LOT.Data.DownTime      = DateTime.Now.ToOADate() - DateTime.Now.AddHours  (-3 ).ToOADate();
            //SPC.LOT.Data.IdleTime      = DateTime.Now.ToOADate() - DateTime.Now.AddHours  (-2 ).ToOADate();
            //SPC.LOT.Data.FailureTime   = DateTime.Now.ToOADate() - DateTime.Now.AddHours  (-10).ToOADate();
            //                           
            //SPC.LOT.Data.WorkCnt       = 60;
            //SPC.LOT.Data.MixDevice     = GetRandomInt(1);
            //SPC.LOT.Data.UnitID        = GetRandomInt(1);
            //SPC.LOT.Data.UnitDMC1      = GetRandomInt(1);
            //SPC.LOT.Data.UnitDMC2      = GetRandomInt(1);
            //SPC.LOT.Data.GlobtopLeft   = GetRandomInt(1);
            //SPC.LOT.Data.GlobtopTop    = GetRandomInt(1);
            //SPC.LOT.Data.GlobtopRight  = GetRandomInt(1);
            //SPC.LOT.Data.GlobtopBottom = GetRandomInt(1);
            //SPC.LOT.Data.Empty         = GetRandomInt(1);
            //SPC.LOT.Data.MatchingError = GetRandomInt(1);
            //SPC.LOT.Data.UserDefine    = GetRandomInt(1);
            //
            //SPC.LOT.SaveDataIni();

        }
        
        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            tmUpdate.Enabled = false;

            btLotDelete .Enabled = FormPassword.GetLevel() > EN_LEVEL.Operator ;
            btErrDelete .Enabled = FormPassword.GetLevel() > EN_LEVEL.Operator ;
            btFailDelete.Enabled = FormPassword.GetLevel() > EN_LEVEL.Operator ;

            btSetRepair .Enabled = FormPassword.GetLevel() > EN_LEVEL.Operator ;

            if (!this.Visible)
            {
                tmUpdate.Enabled = false;
                return;
            }
            tmUpdate.Enabled = true;
        }


        private void btDataView_Click_1(object sender, EventArgs e)
        {
            if (dpSttTime.Value > dpEndTime.Value)
            {
                Log.ShowMessage("ERROR", "The StartTime Must be earlyer than EndTime!");
                return;
            }
            SPC.LOT.DispData(dpSttTime.Value, dpEndTime.Value, lvLot);
            lbUptime.Text = string.Format("Uptime={0:0.000}%", GetLotUptime());

            SPC.ERR.DispData(dpSttTime.Value, dpEndTime.Value, lvError);
            lbMTBA.Text = string.Format("MTBA={0:0.000}min", GetMTBA());
            lbMTTA.Text = string.Format("MTTA={0:0.000}sec", GetMTTA());

            SPC.FLR.DispData(dpSttTime.Value, dpEndTime.Value, lvFailure);
            lbMTBF.Text = string.Format("MTBF={0:0.000}hour", GetMTBF());
            lbMTTR.Text = string.Format("MTTR={0:0.000}min", GetMTTR());

            PaintTrendChart(dpSttTime.Value, dpEndTime.Value);
        }

        private void btLotSave_Click_1(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            SPC.LOT.SaveCsv(lvLot);
        }

        private void btErrSave_Click_1(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            SPC.ERR.SaveCsv(lvError);
        }

        private void btFailSave_Click_1(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            SPC.FLR.SaveCsv(lvFailure);
        }

        private void btLotFind_Click_1(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

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
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

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
            ////SPC.LOT.Data.Device = RandomStr().toString();
            //
            //SPC.LOT.Data.LotNo = GetRandomStr(8);
            //
            //SPC.LOT.Data.TrayNo = GetRandomStr(6);
            //SPC.LOT.Data.Device = GetRandomStr(8);
            //SPC.LOT.Data.StartedAt = DateTime.Now.AddSeconds(-10).ToOADate();
            //SPC.LOT.Data.EndedAt = DateTime.Now.ToOADate();
            //
            //SPC.LOT.Data.RunTime = DateTime.Now.ToOADate() - DateTime.Now.AddHours(-7.3333333111).ToOADate();
            ////            SPC.LOT.Data.RunTime       = 
            //SPC.LOT.Data.DownTime = DateTime.Now.ToOADate() - DateTime.Now.AddHours(-3).ToOADate();
            //SPC.LOT.Data.IdleTime = DateTime.Now.ToOADate() - DateTime.Now.AddHours(-2).ToOADate();
            //SPC.LOT.Data.FailureTime = DateTime.Now.ToOADate() - DateTime.Now.AddHours(-10).ToOADate();
            //
            //SPC.LOT.Data.WorkCnt = 60;
            //SPC.LOT.Data.MixDevice = GetRandomInt(1);
            //SPC.LOT.Data.UnitID = GetRandomInt(1);
            //SPC.LOT.Data.UnitDMC1 = GetRandomInt(1);
            //SPC.LOT.Data.UnitDMC2 = GetRandomInt(1);
            //SPC.LOT.Data.GlobtopLeft = GetRandomInt(1);
            //SPC.LOT.Data.GlobtopTop = GetRandomInt(1);
            //SPC.LOT.Data.GlobtopRight = GetRandomInt(1);
            //SPC.LOT.Data.GlobtopBottom = GetRandomInt(1);
            //SPC.LOT.Data.Empty = GetRandomInt(1);
            //
            //SPC.LOT.SaveDataIni();
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
            if (this.Visible) tmUpdate.Enabled = true;
        }
    }
}





/*
 
 private void dtTolistView(DataTable dt, ListView lvw) { // 테이블 Row가 없을때 if (dt.Rows.Count <= 0) { lvw.Clear(); } else { foreach (DataRow dr in dt.Rows) { ListViewItem lvwi = new ListViewItem(); lvwi.Text = dr[0].ToString(); for (int i = 1; i < dt.Columns.Count; i++) { lvwi.SubItems.Add(dr[i].ToString()); } lvw.Items.Add(lvwi); } } }

출처: http://sociophobia.tistory.com/8 [Devch]
 */