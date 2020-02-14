﻿using COMMON;
using SML;
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
using System.Xml.Serialization;

namespace Machine
{
    public partial class FormGrid : Form
    {
        private const string sFormText = "Form Grid ";

        public FormGrid()
        {


            InitializeComponent();


            InitGridView();

            //옵션 메니저에게 접 붙여놓는다.
            //시퀜스에서 짧게쓸수 있게 그리고 화면 에디터 사용 할 수 있는 양빵방법.
            //gvPara.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText ;
            OM.SeasoningOptnView =  gvPara ; 

            //gvPara.ColumnHeadersDefaultCellStyle.BackColor = Color.Purple;
            this.DoubleBuffered = true;
            typeof(DataGridView).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, gvPara, new object[] { true });  

            foreach (DataGridViewColumn i in gvPara.Columns)
            {
                i.SortMode = DataGridViewColumnSortMode.NotSortable;
                //i.Frozen   = true;
            }

            //Add Line
            //string[] row = { "1", "", "0", 
            //                 "전압유지" , "0", "0", "0", "0", "0", "0", //Anode
            //                 "전압유지" , "0", "0", "0", "0", "0", "0", //Gate
            //                 "전압유지" , "0", "0", "0", "0",           //Focus
            //                 "SWITCHING", "0", "0",                     //Cathod
            //                 "0", "0", "0" };                           //Current Limit
            //
            //gvPara.Rows.Add(row);

            //Getvalue
            //string sTemp  = gvPara.GetString(tbCol.Text , int.Parse(tbRow.Text));
            //MessageBox.Show(sTemp);

            //Load Init
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevInfoPath = sExeFolder + "Recipe\\" + OM.GetCrntDev() + "\\RecipeRead.csv";
            OM.SeasoningOptnView.Load(sDevInfoPath);
            SetList();
        }

        

        private void InitGridView()
        {

            DataGridViewComboBoxColumn clNo          = new DataGridViewComboBoxColumn(); clNo         .Name = "clNo"         ; clNo         .HeaderText = "NO"              ; clNo   .Items.AddRange(Aging.clNo);// "1","2","3","4","5","6","7","8","9","10","11","12","13","14","15","16","17","18","19","20");
            DataGridViewTextBoxColumn  clName        = new DataGridViewTextBoxColumn (); clName       .Name = "clName"       ; clName       .HeaderText = "NAME"            ; 
            DataGridViewTextBoxColumn  clProcessTime = new DataGridViewTextBoxColumn (); clProcessTime.Name = "clProcessTime"; clProcessTime.HeaderText = "진행시간\n(s)"   ;

            //에노드
            DataGridViewComboBoxColumn clAMode       = new DataGridViewComboBoxColumn(); clAMode      .Name = "clAMode"      ; clAMode      .HeaderText = "MODE"            ; clAMode.Items.AddRange(Aging.clAMode);
            DataGridViewTextBoxColumn  clAStartKV    = new DataGridViewTextBoxColumn (); clAStartKV   .Name = "clAStartKV"   ; clAStartKV   .HeaderText = "START\n(kV)"     ;
            DataGridViewTextBoxColumn  clAStopKV     = new DataGridViewTextBoxColumn (); clAStopKV    .Name = "clAStopKV"    ; clAStopKV    .HeaderText = "STOP\n(kV)"      ;
            DataGridViewTextBoxColumn  clAStepKV     = new DataGridViewTextBoxColumn (); clAStepKV    .Name = "clAStepKV"    ; clAStepKV    .HeaderText = "증가전압\n(kV)"  ;
            DataGridViewTextBoxColumn  clAStepTime   = new DataGridViewTextBoxColumn (); clAStepTime  .Name = "clAStepTime"  ; clAStepTime  .HeaderText = "증가시간\n(s)"   ;
            DataGridViewTextBoxColumn  clAOnTime     = new DataGridViewTextBoxColumn (); clAOnTime    .Name = "clAOnTime"    ; clAOnTime    .HeaderText = "전압\nON T(s)"   ;
            DataGridViewTextBoxColumn  clAOffTime    = new DataGridViewTextBoxColumn (); clAOffTime   .Name = "clAOffTime"   ; clAOffTime   .HeaderText = "전압\nOFF T(s)"  ;
            DataGridViewTextBoxColumn  clRepeatCnt   = new DataGridViewTextBoxColumn (); clRepeatCnt  .Name = "clRepeatCnt"  ; clRepeatCnt  .HeaderText = "전압\n반복횟수"  ;

            //포커스
            DataGridViewComboBoxColumn clFMode       = new DataGridViewComboBoxColumn(); clFMode      .Name = "clFMode"      ; clFMode      .HeaderText = "MODE"            ; clFMode.Items.AddRange(Aging.clFMode);
            DataGridViewTextBoxColumn  clFStartKV    = new DataGridViewTextBoxColumn (); clFStartKV   .Name = "clFStartKV"   ; clFStartKV   .HeaderText = "START\n(V)"      ;
            DataGridViewTextBoxColumn  clFStopKV     = new DataGridViewTextBoxColumn (); clFStopKV    .Name = "clFStopKV"    ; clFStopKV    .HeaderText = "STOP\n(V)"       ;
            DataGridViewTextBoxColumn  clFStepKV     = new DataGridViewTextBoxColumn (); clFStepKV    .Name = "clFStepKV"    ; clFStepKV    .HeaderText = "증가전압\n(V)"   ;
            DataGridViewTextBoxColumn  clFStepTime   = new DataGridViewTextBoxColumn (); clFStepTime  .Name = "clFStepTime"  ; clFStepTime  .HeaderText = "증가시간\n(V)"   ;
            //DataGridViewTextBoxColumn  clFKeepA      = new DataGridViewTextBoxColumn (); clFKeepA     .Name = "clGKeepA"     ; clFKeepA     .HeaderText = "전압\nON T(s)"   ;
            //DataGridViewTextBoxColumn  clFEndA       = new DataGridViewTextBoxColumn (); clFEndA      .Name = "clGEndA"      ; clFEndA      .HeaderText = "전압\nOFF T(s)"  ;

            //게이트
            DataGridViewComboBoxColumn clGMode       = new DataGridViewComboBoxColumn(); clGMode      .Name = "clGMode"      ; clGMode      .HeaderText = "MODE"            ; clGMode.Items.AddRange(Aging.clGMode);
            DataGridViewTextBoxColumn  clGStartKV    = new DataGridViewTextBoxColumn (); clGStartKV   .Name = "clGStartKV"   ; clGStartKV   .HeaderText = "START\n(V)"      ;
            DataGridViewTextBoxColumn  clGStopKV     = new DataGridViewTextBoxColumn (); clGStopKV    .Name = "clGStopKV"    ; clGStopKV    .HeaderText = "STOP\n(V)"       ;
            DataGridViewTextBoxColumn  clGStepKV     = new DataGridViewTextBoxColumn (); clGStepKV    .Name = "clGStepKV"    ; clGStepKV    .HeaderText = "증가전압\n(V)"   ;
            DataGridViewTextBoxColumn  clGStepTime   = new DataGridViewTextBoxColumn (); clGStepTime  .Name = "clGStepTime"  ; clGStepTime  .HeaderText = "증가시간\n(s)"   ;
            DataGridViewTextBoxColumn  clGKeepA      = new DataGridViewTextBoxColumn (); clGKeepA     .Name = "clGKeepA"     ; clGKeepA     .HeaderText = "유지전류\n(mA)"  ;
            DataGridViewTextBoxColumn  clGEndA       = new DataGridViewTextBoxColumn (); clGEndA      .Name = "clGEndA"      ; clGEndA      .HeaderText = "종료전류\n(mA)"  ;

            //캐소드
            DataGridViewComboBoxColumn clCMode       = new DataGridViewComboBoxColumn(); clCMode      .Name = "clCMode"      ; clCMode      .HeaderText = "MODE"           ; clCMode.Items.AddRange(Aging.clCMode);
            DataGridViewTextBoxColumn  clCOnTime     = new DataGridViewTextBoxColumn (); clCOnTime    .Name = "clCOnTime"    ; clCOnTime    .HeaderText = "ON T(ms)"       ;
            DataGridViewTextBoxColumn  clCOffTime    = new DataGridViewTextBoxColumn (); clCOffTime   .Name = "clCOffTime"   ; clCOffTime   .HeaderText = "OFF T(ms)"      ;

            DataGridViewTextBoxColumn  clCLMin       = new DataGridViewTextBoxColumn (); clCLMin      .Name = "clCLMin"      ; clCLMin      .HeaderText = "MIN\n(mA)"      ;
            DataGridViewTextBoxColumn  clCLMax       = new DataGridViewTextBoxColumn (); clCLMax      .Name = "clCLMax"      ; clCLMax      .HeaderText = "MAX\n(mA)"      ;
                                                                                                                                                                           
            //마무리                                                                                                                                                       
            //DataGridViewTextBoxColumn  clRepeatCnt   = new DataGridViewTextBoxColumn (); clRepeatCnt  .Name = "clRepeatCnt"  ; clRepeatCnt  .HeaderText = "반복횟수"       ;

            gvPara.Columns.Add(clNo         ); gvPara.Columns[0 ].Width = 50 ;
            gvPara.Columns.Add(clName       ); gvPara.Columns[1 ].Width = 115;
            gvPara.Columns.Add(clProcessTime); gvPara.Columns[2 ].Width = 65 ;
                                                                              
            gvPara.Columns.Add(clAMode      ); gvPara.Columns[3 ].Width = 75 ;
            gvPara.Columns.Add(clAStartKV   ); gvPara.Columns[4 ].Width = 65 ;
            gvPara.Columns.Add(clAStopKV    ); gvPara.Columns[5 ].Width = 65 ;
            gvPara.Columns.Add(clAStepKV    ); gvPara.Columns[6 ].Width = 65 ;
            gvPara.Columns.Add(clAStepTime  ); gvPara.Columns[7 ].Width = 65 ;
            gvPara.Columns.Add(clAOnTime    ); gvPara.Columns[8 ].Width = 65 ;
            gvPara.Columns.Add(clAOffTime   ); gvPara.Columns[9 ].Width = 65 ;
            gvPara.Columns.Add(clRepeatCnt  ); gvPara.Columns[10].Width = 65 ;
                                                                                                           
            gvPara.Columns.Add(clFMode      ); gvPara.Columns[11].Width = 75 ;
            gvPara.Columns.Add(clFStartKV   ); gvPara.Columns[12].Width = 65 ;
            gvPara.Columns.Add(clFStopKV    ); gvPara.Columns[13].Width = 65 ;
            gvPara.Columns.Add(clFStepKV    ); gvPara.Columns[14].Width = 65 ;
            gvPara.Columns.Add(clFStepTime  ); gvPara.Columns[15].Width = 65 ;
            //gvPara.Columns.Add(clFKeepA     ); gvPara.Columns[22].Width = 65 ;
            //gvPara.Columns.Add(clFEndA      ); gvPara.Columns[23].Width = 65 ;
                                             
            gvPara.Columns.Add(clGMode      ); gvPara.Columns[16].Width = 75 ;
            gvPara.Columns.Add(clGStartKV   ); gvPara.Columns[17].Width = 65 ;
            gvPara.Columns.Add(clGStopKV    ); gvPara.Columns[18].Width = 65 ;
            gvPara.Columns.Add(clGStepKV    ); gvPara.Columns[19].Width = 65 ;
            gvPara.Columns.Add(clGStepTime  ); gvPara.Columns[20].Width = 65 ;
            gvPara.Columns.Add(clGKeepA     ); gvPara.Columns[21].Width = 65 ;
            gvPara.Columns.Add(clGEndA      ); gvPara.Columns[22].Width = 65 ;
            
            gvPara.Columns.Add(clCMode      ); gvPara.Columns[23].Width = 75 ;
            gvPara.Columns.Add(clCOnTime    ); gvPara.Columns[24].Width = 50 ;
            gvPara.Columns.Add(clCOffTime   ); gvPara.Columns[25].Width = 50 ;
                                                              
            gvPara.Columns.Add(clCLMin      ); gvPara.Columns[26].Width = 50 ;
            gvPara.Columns.Add(clCLMax      ); gvPara.Columns[27].Width = 50 ;
                                                              
            //gvPara.Columns.Add(clRepeatCnt  ); gvPara.Columns[27].Width = 65 ;

            //UpdateCellSize();
        }

        private void UpdateCellSize()
        {
            // DataGridView에 생성된 3개의 Column 등록 및 너비 설정
            //gvPara.Columns[0 ].Width = 60 ; //gvPara.Columns.Add(clName       );          
            //gvPara.Columns[0 ].Width = 60 ; //gvPara.Columns.Add(clName       );          
            //gvPara.Columns[1 ].Width = 50 ; //gvPara.Columns.Add(clProcessTime);         
                                            //                                  
            //Change cell font
            foreach (DataGridViewColumn c in gvPara.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 10F, GraphicsUnit.Pixel);
                c.HeaderCell.Style.Font = new Font("Arial", 10F, GraphicsUnit.Pixel);
            }

            //gvPara.AutoResizeColumns(); 
            //gvPara.AutoResizeRows();

        }

        private void gvPara_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            DataGridView grid = sender as DataGridView;
            //String rowIdx = (e.RowIndex + 1).ToString("X4");
            String rowIdx = (e.RowIndex + 1).ToString();
            
            StringFormat centerFormat = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            
            Rectangle headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top,grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.ControlText,headerBounds,centerFormat);
        }

        private void gvPara_Paint(object sender, PaintEventArgs e)
        {
            DataGridView gv = (DataGridView)sender;
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            string[] sHeader = { "ANODE", "FOCUS" , "GATE" , "CATHODE", "CURRENT LIMIT"};
            Point [] pRange  = { new Point(3,10) , new Point(11,15) , new Point(16,22) , new Point(23,25) , new Point(26,27)};

            //e.Graphics.D

            // Category Painting
            {
                //여기부터 header
                Rectangle Rect = new Rectangle();
                
                
                for(int i = 0 ; i < pRange.Length ; i++)
                {
                    int iWidth = 0 ;
                    Rect.X = 0 ;
                    Rect.Y = 0 ;
                    Rect.Width = 0 ;
                    Rect.Height= 0 ;
                    for(int j = pRange[i].X ; j <= pRange[i].Y ; j++ )
                    {
                        if(Rect.X==0 && Rect.Y==0 && Rect.Width==0 && Rect.Height==0) Rect = gv.GetCellDisplayRectangle(j, -1, false) ; 
                        iWidth += gv.GetCellDisplayRectangle(j, -1, false).Width;
                    }

                    Rect.X += 1;
                    Rect.Y += 2;
                    Rect.Width = iWidth - 1 ;
                    Rect.Height = (Rect.Height / 3) - 1;

                    //e.Graphics.DrawRectangle(new Pen(gv.BackgroundColor), Rect);
                    //e.Graphics.DrawRectangle(new Pen(Color.LightBlue), Rect);
                    e.Graphics.DrawRectangle(new Pen(Color.White), Rect);
                    e.Graphics.FillRectangle(new SolidBrush(gv.ColumnHeadersDefaultCellStyle.BackColor), Rect);
                    e.Graphics.DrawString(sHeader[i],gv.ColumnHeadersDefaultCellStyle.Font,new SolidBrush(gv.ColumnHeadersDefaultCellStyle.ForeColor),Rect,format);
                }
            }
        }

        private void gvPara_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            DataGridView gv = (DataGridView)sender;
            Rectangle rtHeader = gv.DisplayRectangle;
            rtHeader.Height = gv.ColumnHeadersHeight / 2;
            gv.Invalidate(rtHeader);
        }

        private void gvPara_Scroll(object sender, ScrollEventArgs e)
        {
            DataGridView gv = (DataGridView)sender;
            Rectangle rtHeader = gv.DisplayRectangle;
            rtHeader.Height = gv.ColumnHeadersHeight / 2;
            gv.Invalidate(rtHeader);
        }

        //private string GetName(int iNo)
        //{
        //    string sName = "";
        //    if(gvPara.GetString("clNo" , iNo ) == "1"  ) sName = OM.CmnOptn.sName1 ;
        //    if(gvPara.GetString("clNo" , iNo ) == "2"  ) sName = OM.CmnOptn.sName2 ;
        //    if(gvPara.GetString("clNo" , iNo ) == "3"  ) sName = OM.CmnOptn.sName3 ;
        //    if(gvPara.GetString("clNo" , iNo ) == "4"  ) sName = OM.CmnOptn.sName4 ;
        //    if(gvPara.GetString("clNo" , iNo ) == "5"  ) sName = OM.CmnOptn.sName5 ;
        //    if(gvPara.GetString("clNo" , iNo ) == "6"  ) sName = OM.CmnOptn.sName6 ;
        //    if(gvPara.GetString("clNo" , iNo ) == "7"  ) sName = OM.CmnOptn.sName7 ;
        //    if(gvPara.GetString("clNo" , iNo ) == "8"  ) sName = OM.CmnOptn.sName8 ;
        //    if(gvPara.GetString("clNo" , iNo ) == "9"  ) sName = OM.CmnOptn.sName9 ;
        //    if(gvPara.GetString("clNo" , iNo ) == "10" ) sName = OM.CmnOptn.sName10;
        //    if(gvPara.GetString("clNo" , iNo ) == "11" ) sName = OM.CmnOptn.sName11;
        //    if(gvPara.GetString("clNo" , iNo ) == "12" ) sName = OM.CmnOptn.sName12;
        //    if(gvPara.GetString("clNo" , iNo ) == "13" ) sName = OM.CmnOptn.sName13;
        //    if(gvPara.GetString("clNo" , iNo ) == "14" ) sName = OM.CmnOptn.sName14;
        //    if(gvPara.GetString("clNo" , iNo ) == "15" ) sName = OM.CmnOptn.sName15;
        //    if(gvPara.GetString("clNo" , iNo ) == "16" ) sName = OM.CmnOptn.sName16;
        //    if(gvPara.GetString("clNo" , iNo ) == "17" ) sName = OM.CmnOptn.sName17;
        //    if(gvPara.GetString("clNo" , iNo ) == "18" ) sName = OM.CmnOptn.sName18;
        //    if(gvPara.GetString("clNo" , iNo ) == "19" ) sName = OM.CmnOptn.sName19;
        //    if(gvPara.GetString("clNo" , iNo ) == "20" ) sName = OM.CmnOptn.sName20;
            
        //    return sName;
        //}

        public bool SetList()
        {
            lock(SEQ.lockObject)
            {             
            SEQ.aging.lst.Clear();
            for(int i=0; i<gvPara.RowCount-1; i++)
            {
                try { 
                    //gvPara.SetString("clName" , i , GetName(i));

                    Data.TTotal dt = new Data.TTotal();
                    //dt.No        = gvPara.GetString ("clNo"     , i);                             gvPara.SetString("clNo"         ,i,dt.No                  );
                    dt.No        = gvPara.GetString ("clNo"         , i);  gvPara.SetString("clNo"         ,i,dt.No       .ToString());
                    dt.Name      = gvPara.GetString ("clName"       , i);  gvPara.SetString("clName"       ,i,dt.Name     .ToString());
                    dt.Time      = CConfig.StrToIntDef(gvPara.GetString ("clProcessTime", i),0);  gvPara.SetString("clProcessTime",i,dt.Time     .ToString());
                    
                    Data.TAnode da = new Data.TAnode();
                    da.Mode    = gvPara.GetString("clAMode"     , i);                             gvPara.SetString("clAMode"      ,i,da.Mode              );                                       
                    da.StartkV = CConfig.StrToDoubleDef(gvPara.GetString("clAStartKV"   , i),0);  gvPara.SetString("clAStartKV"   ,i,da.StartkV.ToString());
                    da.StopkV  = CConfig.StrToDoubleDef(gvPara.GetString("clAStopKV"    , i),0);  gvPara.SetString("clAStopKV"    ,i,da.StopkV .ToString());
                    da.InckV   = CConfig.StrToDoubleDef(gvPara.GetString("clAStepKV"    , i),0);  gvPara.SetString("clAStepKV"    ,i,da.InckV  .ToString());
                    da.IncTime = CConfig.StrToDoubleDef(gvPara.GetString("clAStepTime"  , i),0);  gvPara.SetString("clAStepTime"  ,i,da.IncTime.ToString());
                    da.OnTime  = CConfig.StrToDoubleDef(gvPara.GetString("clAOnTime"    , i),0);  gvPara.SetString("clAOnTime"    ,i,da.OnTime .ToString());
                    da.OffTime = CConfig.StrToDoubleDef(gvPara.GetString("clAOffTime"   , i),0);  gvPara.SetString("clAOffTime"   ,i,da.OffTime.ToString());
                    da.RptCnt  = CConfig.StrToIntDef   (gvPara.GetString ("clRepeatCnt" , i),0);  gvPara.SetString("clRepeatCnt"  ,i,da.RptCnt .ToString());
                                                                                                                                  
                    Data.TFocus df = new Data.TFocus();                                                                           
                    df.Mode    = gvPara.GetString("clFMode"    , i);                              gvPara.SetString("clFMode"      ,i,df.Mode              );                                      
                    df.StartV  = CConfig.StrToDoubleDef(gvPara.GetString("clFStartKV"   , i),0);  gvPara.SetString("clFStartKV"   ,i,df.StartV .ToString());
                    df.StopV   = CConfig.StrToDoubleDef(gvPara.GetString("clFStopKV"    , i),0);  gvPara.SetString("clFStopKV"    ,i,df.StopV  .ToString());
                    df.IncV    = CConfig.StrToDoubleDef(gvPara.GetString("clFStepKV"    , i),0);  gvPara.SetString("clFStepKV"    ,i,df.IncV   .ToString());
                    df.IncTime = CConfig.StrToDoubleDef(gvPara.GetString("clFStepTime"  , i),0);  gvPara.SetString("clFStepTime"  ,i,df.IncTime.ToString());

                    Data.TGate dg = new Data.TGate();                                                                             
                    dg.Mode    = gvPara.GetString("clGMode"     , i);                             gvPara.SetString("clGMode"      ,i,dg.Mode              );                                
                    dg.StartV  = CConfig.StrToDoubleDef(gvPara.GetString("clGStartKV"   , i),0);  gvPara.SetString("clGStartKV"   ,i,dg.StartV .ToString());
                    dg.StopV   = CConfig.StrToDoubleDef(gvPara.GetString("clGStopKV"    , i),0);  gvPara.SetString("clGStopKV"    ,i,dg.StopV  .ToString());
                    dg.IncV    = CConfig.StrToDoubleDef(gvPara.GetString("clGStepKV"    , i),0);  gvPara.SetString("clGStepKV"    ,i,dg.IncV   .ToString());
                    dg.IncTime = CConfig.StrToDoubleDef(gvPara.GetString("clGStepTime"  , i),0);  gvPara.SetString("clGStepTime"  ,i,dg.IncTime.ToString());
                    dg.KeepmA  = CConfig.StrToDoubleDef(gvPara.GetString("clGKeepA"     , i),0);  gvPara.SetString("clGKeepA"     ,i,dg.KeepmA .ToString());
                    dg.EndmA   = CConfig.StrToDoubleDef(gvPara.GetString("clGEndA"      , i),0);  gvPara.SetString("clGEndA"      ,i,dg.EndmA  .ToString());
                                                                                                                                  
                    Data.TCathode dc = new Data.TCathode();                                                                       
                    dc.Mode    = gvPara.GetString("clCMode"   , i);                               gvPara.SetString("clCMode"      ,i,dc.Mode              );                                
                    dc.OnTime  = CConfig.StrToDoubleDef(gvPara.GetString("clCOnTime"    , i),0);  gvPara.SetString("clCOnTime"    ,i,dc.OnTime .ToString());
                    dc.OffTime = CConfig.StrToDoubleDef(gvPara.GetString("clCOffTime"   , i),0);  gvPara.SetString("clCOffTime"   ,i,dc.OffTime.ToString());
                    dc.MinmA   = CConfig.StrToDoubleDef(gvPara.GetString("clCLMin"      , i),0);  gvPara.SetString("clCLMin"      ,i,dc.MinmA  .ToString());   
                    dc.MaxmA   = CConfig.StrToDoubleDef(gvPara.GetString("clCLMax"      , i),0);  gvPara.SetString("clCLMax"      ,i,dc.MaxmA  .ToString());

                    SEQ.aging.lst.Add(new Data(dt,da,dg,df,dc));

                }
                catch (Exception exceptionObject)
                {
                    MessageBox.Show(exceptionObject.ToString());
                    return false;
                }
            }
            }

            //Add List Log 
            SEQ.aging.ListLog();
            return true;
        }

        private void gvPara_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //if(e.ColumnIndex != 0) return;
            //gvPara.SetString("clName" , e.RowIndex , GetName(e.RowIndex));
        }

        private void btDeleteLine_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            if(gvPara.Rows.Count-2 > 0) gvPara.Rows.Remove(gvPara.Rows[gvPara.Rows.Count-2]);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;

            //lbRecipe.Text  =  OM.GetCrntDev();
            btSave.Enabled = !SEQ._bRun;

            timer1.Enabled = true;
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            if (Log.ShowMessageModal("Confirm", "Are you Sure?") != DialogResult.Yes) return;

            btSave.Enabled = false;

            SetList();
            
            SEQ.aging.ListLog();

            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sDevInfoPath = sExeFolder + "Recipe\\" + OM.GetCrntDev() + "\\RecipeRead.csv";
            File.Delete(sDevInfoPath);
            gvPara.Save(sDevInfoPath);

            //데이터 그리드뷰 불러와서 셋팅하기
            FormDevice.FrmDeviceSet.RecipeSetting();
            //FormDeviceSet.RecipeSetting();
            //FrmMain.FrmDeviceSet.RecipeSetting();

            btSave.Enabled = true ;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(gvPara.GetClipboardContent());
        }

        private void gvPara_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Modifiers == Keys.Control)
            {
                switch(e.KeyCode)
                {
                    case Keys.C:
                        gvPara.CopyToClipboard();
                        break;
                    case Keys.V:
                        gvPara.PasteClipboard();
                        break;
                }
            }
        }

    }
    //컬럼 이름으로 값을 가져오기 위한 Extention Class
    public static class DataGridViewExtenstions
    {
        //public static object GetCellValueFromColumnHeader(this DataGridViewCellCollection CellCollection, string HeaderText)
        //{
        //    //return CellCollection.Cast<DataGridViewCell>().First(c => c.OwningColumn.HeaderText == HeaderText).Value;            
        //
        //    foreach(DataGridViewColumn Col in )
        //
        //
        //
        //}

        //public static object GetDataByName(this DataGridViewCellCollection CellCollection, string HeaderText)
        //{
        //    //return CellCollection.Cast<DataGridViewCell>().First(c => c.OwningColumn.HeaderText == HeaderText).Value;            
        //
        //    foreach(DataGridViewColumn Col in )
        //
        //
        //
        //}

        public static string GetString(this DataGridView GridView ,  string _sName , int _iRow)
        {
            string sRet = "";
            for(int c = 0 ; c < GridView.ColumnCount ; c++)
            {
                if(GridView.Columns[c].Name == _sName)
                {
                    try
                    {
                        sRet = GridView[c,_iRow].Value.ToString();
                    }
                    catch
                    {
                        sRet = "";
                    }
                    return sRet ;
                    
                }
            }
            return sRet;
        }

        //이거 되려나....
        public static bool SetString(this DataGridView GridView , string _sName , int _iRow , string _sValue)
        {
            for(int c = 0 ; c < GridView.ColumnCount ; c++)
            {
                if(GridView.Columns[c].Name == _sName) {
                    GridView[c,_iRow].Value = _sValue ;
                    return true ;
                }
            }
            return false ;

        }

        public static void CopyToClipboard(this DataGridView GridView)
        {
            DataObject obj = GridView.GetClipboardContent();
            if(obj != null)
            {
                Clipboard.SetDataObject(obj);
            }
        }

        public static void PasteClipboard(this DataGridView GridView)
        {
            string s = Clipboard.GetText();
            string[] lines = s.Split('\n');
            int iRow = GridView.CurrentCell.RowIndex   ;
            int iCol = GridView.CurrentCell.ColumnIndex;
            DataGridViewCell oCell;
            try//if(true)//iRow + lines.Length > gvPara.Rows.Count-1)
            {
                bool bFlag = false;
                foreach(string sEmpty in lines) 
                { 
                    if(sEmpty == "")
                    {
                        bFlag = true;
                    }
                }
                int iNewRows = iRow + lines.Length - GridView.Rows.Count;
                if(iNewRows >=0)
                {
                    if(bFlag) GridView.Rows.Add(iNewRows);
                    else      GridView.Rows.Add(iNewRows+1);
                }
                else
                {
                    //gvPara.Rows.Add(iNewRows+1);
                }
                foreach(string line in lines)
                {
                    if(iRow < GridView.RowCount && line.Length > 0)
                    {
                        string[] sCells = line.Split('\t');
                        for(int i=0;i<sCells.GetLength(0);++i)
                        {
                            if(iCol+i<GridView.ColumnCount)
                            {
                                oCell = GridView[iCol+i,iRow];
                                oCell.Value = Convert.ChangeType(sCells[i].Replace("\r",""),oCell.ValueType);
                            }
                            else
                            {
                                break;
                            }
                        }
                            iRow++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch(FormatException)
            {
                return;
            }
            //Clipboard.Clear();
        }

        [Serializable]
        public sealed class ColumnInfo
        {
            public string Name { get; set; }
            public int DisplayIndex { get; set; }
            public int Width { get; set; }
            public bool Visible { get; set; }
        }
        public static bool Load(this DataGridView GridView, string fileName)
        {
            GridView.Rows.Clear();

            string [] sDatas ;
            try
            {
                sDatas= System.IO.File.ReadAllLines(fileName, Encoding.Default);
            }
            catch(Exception _e)
            {
                //MessageBox.Show(_e.Message);
                return false ;
            }
                

            //해더만 있어도 리턴.
            if(sDatas.Length < 2) return false ;

            //0번은 해더.
            GridView.RowCount = sDatas.Length ;
            for (int r = 1; r < sDatas.Length ; r++)
            {
                string [] RowData = sDatas[r].Split(',');
                if (RowData.Length != GridView.ColumnCount) return false;
                for(int c = 0 ; c < RowData.Length ; c++)
                {
                    GridView[c,r-1].Value = RowData[c];
                }

                //GridView.Rows.Add(RowData);
            }
            return true ;
        }

        public static bool Save(this DataGridView GridView, string fileName)
        {

            try
            {
                if (GridView.ColumnCount <= 0) return false;
                using (System.IO.FileStream csvFileWriter = new FileStream(fileName,FileMode.OpenOrCreate))
                {
                    //Header
                    string columnHeaderText = "";
                    for (int c = 0; c < GridView.ColumnCount; c++)
                    {
                        columnHeaderText += GridView.Columns[c].Name;
                        if (c != GridView.ColumnCount - 1) columnHeaderText += ',';
                    }
                    Byte[] Bytes = Encoding.GetEncoding("ks_c_5601-1987").GetBytes(columnHeaderText+"\n");
                    csvFileWriter.Write(Bytes, 0, Bytes.Length);

                    if (GridView.RowCount <= 0) return false;
                    //Data
                    string RowData = "";
                    for (int r = 0; r < GridView.RowCount - 1; r++)
                    {
                        RowData = "";
                        for (int c = 0; c < GridView.ColumnCount; c++)
                        {
                            //if(GridView.Columns[c] is DataGridViewComboBoxColumn)
                            //{
                            //    RowData +=uint.Parse(GridView.Rows[r].Cells[c].Value);
                            //}
                            //else
                            //{
                            //    RowData += GridView[c, r].Value.ToString();
                            //}

                            if(GridView[c, r].Value == null) RowData +=  "" ;
                            else                             RowData += GridView[c, r].Value.ToString();

                            if (c != GridView.ColumnCount - 1) RowData += ",";

                        }
                        Bytes = Encoding.GetEncoding("ks_c_5601-1987").GetBytes(RowData+"\n");
                        //csvFileWriter.Write(Bytes, 0, Bytes.Length);
                        //csvFileWriter.wr
                        csvFileWriter.Write(Bytes, 0, Bytes.Length);
                    }
                    csvFileWriter.Close();
                }


            }
            catch (Exception exceptionObject)
            {
                MessageBox.Show(exceptionObject.ToString());
                return false;
            }
            return true;

        }
        

        /*
         * public static bool Save(this DataGridView GridView, string fileName)
        {

            try
            {
                if (GridView.ColumnCount <= 0) return false ;
                using (System.IO.StreamWriter csvFileWriter = new StreamWriter(fileName, false))
                { 
                    //Header
                    string columnHeaderText = "";
                    for (int c = 0; c < GridView.ColumnCount; c++)
                    {
                        columnHeaderText += GridView.Columns[c].Name;
                        if(c != GridView.ColumnCount-1) columnHeaderText += ',' ;
                    }
                    csvFileWriter.WriteLine(columnHeaderText);
                    
                    if(GridView.RowCount <= 0) return false ;
                    //Data
                    string RowData = "";
                    for(int r = 0 ; r < GridView.RowCount-1 ; r++)
                    {
                        RowData = "";
                        for(int c = 0 ; c < GridView.ColumnCount ; c++ )
                        {
                            RowData += GridView[c,r].Value.ToString() ;
                            if(c != GridView.ColumnCount-1) RowData +=",";
                    
                        }                    
                        csvFileWriter.WriteLine(RowData);
                    }
                    csvFileWriter.Close();
                }

                
            }
            catch (Exception exceptionObject)
            {
                MessageBox.Show(exceptionObject.ToString());
                return false ;
            }
            return true ;

        }*/
    }
}
