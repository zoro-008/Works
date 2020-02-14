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
using COMMON;
using System.Runtime.CompilerServices;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace Machine.View.DataMan
{
    public partial class DataArray : UserControl
    {
        DispatcherTimer Timer = new DispatcherTimer();

        public enum EN_STATUS_POPUP_ID : uint
        {
            spOne  = 0 ,
            spCol      ,
            spRow      ,
            spAll      ,
            spDrag     ,

            MAX_STATUS_POPUP_ID
        }

        private struct TClickStat
        {
            public bool bMouseDown;
            public double dSelX1, dSelY1, dSelX2, dSelY2;
            public int    iSelR1, iSelR2, iSelC1, iSelC2;
        }
        TClickStat ClickStat;

        private int         m_iCellMargin = 0 ; //디스플레이 및 클릭시에 마진.
        private bool        m_bSameCellSize; //셀사이즈 관련.

        public int          CellMargin   { get { return m_iCellMargin  ;} set { m_iCellMargin   = value; } }
        public bool         SameCellSize { get { return m_bSameCellSize;} set { m_bSameCellSize = value; } } //셀사이즈 관련.
        
        struct TStatProp
        {
            public Visibility Visible  ;
            public Brush      ChipColor;
            public string     Caption  ;
            public bool       Enable   ;
        }

        private TStatProp[] StatProp = new TStatProp[(int)cs.MAX_CHIP_STAT];
        public Point lpPoint;

        private ContextMenu[] cmArray = new ContextMenu[(int)EN_STATUS_POPUP_ID.MAX_STATUS_POPUP_ID];

        Rectangle DragRect = new Rectangle(); //드래그할때 마우스 무브 범위 표현하는 사각형 틀

        public int iArrayId { get; set; } = -1;

        public DataArray()
        {
            InitializeComponent();

            for (int i = 0; i < (int)EN_STATUS_POPUP_ID.MAX_STATUS_POPUP_ID; i++)
            {
                cmArray[i] = new ContextMenu();
                cmArray[i].ContextMenuClosing += new ContextMenuEventHandler(Popup_Closing);
            }

            //UI 타이머
            Timer.Interval = TimeSpan.FromMilliseconds(1000);
            Timer.Tick += new EventHandler(Timer_Tick);

            DataContext = this;
            //cvArray

        }

        public void SetConfig(int _iId, string _sName)//, int _iMaxCol, int _iMaxRow)
        {
            iArrayId = _iId;
            DM.ARAY[iArrayId].Name   = _sName;
            DM.ARAY[iArrayId].Resize = SetResize;
            //DM.SetMaxColRow(iArrayId, _iMaxCol, _iMaxRow);

            gbArray.Header = _sName;

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            PaintAray();
            //DM.SetMaxColRow(iArrayId,50,30);
            //cvArray.row
        }

        private void Popup_Closing(object sender, ContextMenuEventArgs e)
        {
            ClickStat.dSelX1 = ClickStat.dSelX2 = ClickStat.dSelY1 = ClickStat.dSelY2 = 0;
        }

        private void GetChipCoord(int _iCol , int _iRow , ref double _iX , ref double _iY , ref double _iW , ref double _iH)
        {
            int iTag = Convert.ToInt32(this.Tag);
            
            //그림그릴때 1픽셀 오른쪽으로 그리는 것 때문에 
            const int iPaintSttOft = 0;
            int iRow = DM.ARAY[iArrayId].MaxRow;
            int iCol = DM.ARAY[iArrayId].MaxCol;
                        
            //똑같은 크기로 셀을 그릴때 그릴수 있는 총넓이.
            //셀들이 작아지면 똑같이 안그리면 삐뚤빼뚤 하다.
            double    DrawWidth  = cvArray.ActualWidth  / iCol * iCol ;
            double    DrawHeight = cvArray.ActualHeight / iRow * iRow ;

            //셀들의 넓이 높이.
            double dCellWidth  = DrawWidth  / (double)iCol;
            double dCellHeight = DrawHeight / (double)iRow;

            //전체 그림을 그릴때 시작 오프셑.
            double dXSttOfs = (cvArray.ActualWidth  - DrawWidth ) / 2;
            double dYSttOfs = (cvArray.ActualHeight - DrawHeight) / 2;

            _iX = dXSttOfs + iPaintSttOft + (dCellWidth  * _iCol) + m_iCellMargin;
            _iY = dYSttOfs + iPaintSttOft + (dCellHeight * _iRow) + m_iCellMargin;

            _iW = dCellWidth  - m_iCellMargin*2 + iPaintSttOft;
            _iH = dCellHeight - m_iCellMargin*2 + iPaintSttOft;
            
        }

        private bool GetChipCR(double _iX, double _iY, ref int _iC, ref int _iR)
        {
            int iTag = Convert.ToInt32(this.Tag);
            //그림그릴때 1픽셀 오른쪽으로 그리는 것 때문에 
            const int iPaintSttOft = 0;
            int iRow = DM.ARAY[iArrayId].MaxRow;
            int iCol = DM.ARAY[iArrayId].MaxCol;

            //똑같은 크기로 셀을 그릴때 그릴수 있는 총넓이.
            //셀들이 작아지면 똑같이 안그리면 삐뚤빼뚤 하다.
            double    DrawWidth  = cvArray.ActualWidth  / iCol * iCol;
            double    DrawHeight = cvArray.ActualHeight / iRow * iRow;

            //셀들의 넓이 높이.
            double dCellWidth  = DrawWidth  / (double)iCol;
            double dCellHeight = DrawHeight / (double)iRow;

            //전체 그림을 그릴때 시작 오프셑.
            double dXSttOfs = (cvArray.ActualWidth  - DrawWidth ) / 2;
            double dYSttOfs = (cvArray.ActualHeight - DrawHeight) / 2;
            
            _iC = (int)((_iX - dXSttOfs - iPaintSttOft) / dCellWidth ) ;
            _iR = (int)((_iY - dYSttOfs - iPaintSttOft) / dCellHeight);

            if (_iR < 0) return false;
            if (_iC < 0) return false;

            if (_iR >= iRow) return false;
            if (_iC >= iCol) return false;

            return true;
        }
        
        private void SetPopupMenuStat()
        {
            MenuItem miStatusOne ;
            MenuItem miStatusCol ;
            MenuItem miStatusRow ;
            MenuItem miStatusAll ;
            MenuItem miStatusDrag;
        
        
            cmArray[(int)EN_STATUS_POPUP_ID.spOne ].Items.Clear();
            cmArray[(int)EN_STATUS_POPUP_ID.spCol ].Items.Clear();
            cmArray[(int)EN_STATUS_POPUP_ID.spRow ].Items.Clear();
            cmArray[(int)EN_STATUS_POPUP_ID.spAll ].Items.Clear();
            cmArray[(int)EN_STATUS_POPUP_ID.spDrag].Items.Clear();
        
            for (int s = 0; s < (int)cs.MAX_CHIP_STAT; s++)
            {
                miStatusOne = new MenuItem();
                miStatusOne.Background = SystemColors.ControlDarkBrush;
                miStatusOne.Visibility = StatProp[s].Visible ;
                miStatusOne.IsEnabled  = StatProp[s].Enable  ;

                miStatusOne.Header = "ONE__" + StatProp[s].Caption;
                miStatusOne.Tag = s;
                miStatusOne.Click += new RoutedEventHandler(this.OneClick);
                miStatusOne.Icon = ItemPaint(s);
                if(StatProp[s].Caption != null)
                    cmArray[(int)EN_STATUS_POPUP_ID.spOne].Items.Add(miStatusOne);
        
                miStatusCol = new MenuItem();
                miStatusCol.Background = SystemColors.ControlDarkBrush;
                miStatusCol.Visibility = StatProp[s].Visible;
                miStatusCol.IsEnabled  = StatProp[s].Enable ;
                miStatusCol.Header = "COL__" + StatProp[s].Caption;
                miStatusCol.Tag = s;
                miStatusCol.Click += new RoutedEventHandler(this.ColClick);
                miStatusCol.Icon = ItemPaint(s);
                if(StatProp[s].Caption != null)
                    cmArray[(int)EN_STATUS_POPUP_ID.spCol].Items.Add(miStatusCol);
        
                miStatusRow = new MenuItem();
                miStatusRow.Background = SystemColors.ControlDarkBrush;
                miStatusRow.Visibility = StatProp[s].Visible;
                miStatusRow.IsEnabled  = StatProp[s].Enable ;
                miStatusRow.Header = "ROW__" + StatProp[s].Caption;
                miStatusRow.Tag = s;
                miStatusRow.Click += new RoutedEventHandler(this.RowClick);
                miStatusRow.Icon = ItemPaint(s);
                if(StatProp[s].Caption != null)
                    cmArray[(int)EN_STATUS_POPUP_ID.spRow].Items.Add(miStatusRow);
        
                miStatusAll = new MenuItem();
                miStatusAll.Background = SystemColors.ControlDarkBrush;
                miStatusAll.Visibility = StatProp[s].Visible;
                miStatusAll.IsEnabled  = StatProp[s].Enable ;
                miStatusAll.Header = "ALL__" + StatProp[s].Caption;
                miStatusAll.Tag = s;
                miStatusAll.Click += new RoutedEventHandler(this.AllClick);
                miStatusAll.Icon = ItemPaint(s);
                if(StatProp[s].Caption != null)
                    cmArray[(int)EN_STATUS_POPUP_ID.spAll].Items.Add(miStatusAll);
        
                miStatusDrag = new MenuItem();
                miStatusDrag.Background = SystemColors.ControlDarkBrush;
                miStatusDrag.Visibility = StatProp[s].Visible;
                miStatusDrag.IsEnabled  = StatProp[s].Enable ;
                miStatusDrag.Header = "DRG__" + StatProp[s].Caption;
                miStatusDrag.Tag = s;
                miStatusDrag.Click += new RoutedEventHandler(this.DragClick);
                miStatusDrag.Icon = ItemPaint(s);
                if(StatProp[s].Caption != null)
                    cmArray[(int)EN_STATUS_POPUP_ID.spDrag].Items.Add(miStatusDrag);
            }
        }
        
        public void SetDisplay(cs _eStat, String _sName, Brush _cColor, Visibility _vVisible = Visibility.Visible, bool _bEnable = true)
        {
            StatProp[(int)_eStat].Caption   = _sName    ; 
            StatProp[(int)_eStat].Visible   = _vVisible ;
            StatProp[(int)_eStat].Enable    = _bEnable  ;
            StatProp[(int)_eStat].ChipColor = _cColor   ;
            SetPopupMenuStat();
            
        }

        public void SetPopupName(cs _eStat, String _sName)
        {
            StatProp[(int)_eStat].Caption = _sName;
            SetPopupMenuStat();
            //OnChange();
        }

        public void SetPopupVisible(cs _eStat, Visibility _bVisible)
        {
            StatProp[(int)_eStat].Visible = _bVisible;
            SetPopupMenuStat();
            //OnChange();
        }
        public void SetPopupColor(cs _eStat, Brush _cColor)
        {
            StatProp[(int)_eStat].ChipColor = _cColor;
            SetPopupMenuStat();
            //OnChange();
        }
        //PopupMenu
        public void SetPopupEnable(cs _eStat, bool _bState)
        {
            //for (int i = 0; i < (int)EN_STATUS_POPUP_ID.MAX_STATUS_POPUP_ID; i++)
            StatProp[(int)_eStat].Enable = _bState;
            SetPopupMenuStat();
        }

        private void OneClick(object sender, RoutedEventArgs e)
        {
            //Local Var.
            int Tag = (int)((MenuItem)sender).Tag;
        
            String sTemp;
            sTemp = "One(C=" + ClickStat.iSelR2.ToString() + ",R=" + ClickStat.iSelC2.ToString() + ") Stat Change " + (cs)DM.GetStat(iArrayId, ClickStat.iSelC2, ClickStat.iSelR2) + " To " + (cs)Tag;
            String sHead = "One StatChange " + Name;
            Log.Trace(sHead, sTemp);
        
            ClickStat.dSelX1 = ClickStat.dSelX2 = ClickStat.dSelY1 = ClickStat.dSelY2 = 0;
        
            //Local Var.
            DM.SetStat(iArrayId, ClickStat.iSelC2, ClickStat.iSelR2, (cs)Tag);

            PaintAray();
        }
        private void AllClick(object sender, RoutedEventArgs e)
        {
            //Local Var.
            int Tag = (int)((MenuItem)sender).Tag;
        
            String sTemp;
            sTemp = "All Stat Change To " + (cs)Tag;
            String sHead = "All StatChange " + Name;
            Log.Trace(sHead, sTemp);
        
            ClickStat.dSelX1 = ClickStat.dSelX2 = ClickStat.dSelY1 = ClickStat.dSelY2 = 0;
        
            //Get RC.
            DM.SetStat(iArrayId, (cs)Tag);
            PaintAray();
        }
        private void RowClick(object sender, RoutedEventArgs e)
        {
            //Local Var.
            int Tag = (int)((MenuItem)sender).Tag;
        
            String sTemp;
            sTemp = "Row(R=" + ClickStat.iSelC2.ToString() + ") Stat Change " + (cs)DM.GetStat(iArrayId, ClickStat.iSelC2, ClickStat.iSelR2) + " To " + (cs)Tag;
            String sHead = "Row StatChange " + Name;
            Log.Trace(sHead, sTemp);
        
            ClickStat.dSelX1 = ClickStat.dSelX2 = ClickStat.dSelY1 = ClickStat.dSelY2 = 0;
        
            //Get RC.
            for (int c = 0; c < DM.ARAY[iArrayId].MaxCol; c++)
            {
                DM.SetStat(iArrayId, c, ClickStat.iSelR2, (cs)Tag);
            }
            PaintAray();
        }
        private void ColClick(object sender, RoutedEventArgs e)
        {
            //Local Var.
            int Tag = (int)((MenuItem)sender).Tag;
        
            String sTemp;
            sTemp = "Col(C=" + ClickStat.iSelC2.ToString() + ") Stat Change " + (cs)DM.GetStat(iArrayId, ClickStat.iSelC2, ClickStat.iSelR2) + " To " + (cs)Tag;
            String sHead = "Col StatChange " + Name;
            Log.Trace(sHead, sTemp);
        
            ClickStat.dSelX1 = ClickStat.dSelX2 = ClickStat.dSelY1 = ClickStat.dSelY2 = 0;
        
            for (int r = 0; r < DM.ARAY[iArrayId].MaxRow; r++)
            {
                DM.SetStat(iArrayId, ClickStat.iSelC2, r, (cs)Tag);
            }
            PaintAray();
        }
        private void DragClick(object sender, RoutedEventArgs e)
        {
            //Local Var.
            int Tag = (int)((MenuItem)sender).Tag;
        
            String sTemp;
            sTemp = "Drag(C1=" + ClickStat.iSelC1.ToString() + ",R1=" + ClickStat.iSelR1.ToString() + " C2=" + ClickStat.iSelC2.ToString() + ",R2=" + ClickStat.iSelR2.ToString() + "Stat Change " + (cs)DM.GetStat(iArrayId, ClickStat.iSelC2, ClickStat.iSelR2) + " To " + (cs)Tag;
            String sHead = "Drag StatChange " + Name;
            Log.Trace(sHead, sTemp);
        
            int r1, r2, c1, c2;
            if (ClickStat.iSelR1 < ClickStat.iSelR2) { r1 = ClickStat.iSelR1; r2 = ClickStat.iSelR2; }
            else                                     { r1 = ClickStat.iSelR2; r2 = ClickStat.iSelR1; }
            if (ClickStat.iSelC1 < ClickStat.iSelC2) { c1 = ClickStat.iSelC1; c2 = ClickStat.iSelC2; }
            else                                     { c1 = ClickStat.iSelC2; c2 = ClickStat.iSelC1; }
        
            ClickStat.dSelX1 = ClickStat.dSelX2 = ClickStat.dSelY1 = ClickStat.dSelY2 = 0;
        
            DM.RangeSetStat(iArrayId, c1, r1, c2, r2, (cs)Tag);
            PaintAray();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            //PaintAray();
            //base.OnRender(drawingContext);
        }
        public void PaintAray()
        {
            int iRow = DM.ARAY[iArrayId].MaxRow;
            int iCol = DM.ARAY[iArrayId].MaxCol;

            for (int r = 0; r < iRow; r++)
            {
                for (int c = 0; c < iCol; c++)
                {
                    cs eStat = DM.GetStat(iArrayId, c, r);
                    borders[r][c].Background = StatProp[(int)eStat].ChipColor;
                }
            }

        }
        
        private object ItemPaint(int _iStat)
        {
            Rectangle rect = new Rectangle();
            rect.Width  = 20;
            rect.Height = 20;
            rect.Fill = StatProp[_iStat].ChipColor;

            return rect;
        }
        
        public List<Border>[] borders;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetResize();
            PaintAray();
        }

        private void SetResize()
        {
            this.Dispatcher.Invoke(() =>
            {
                if(!cvArray.IsLoaded) return;
                
                //PaintAray();
                int iRow = DM.ARAY[iArrayId].MaxRow;
                int iCol = DM.ARAY[iArrayId].MaxCol;

                if (iRow == 0) iRow = 1;
                if (iCol == 0) iCol = 1;
        
                cvArray.BeginInit();
                cvArray.Children.Clear();
                cvArray.Rows    = iRow ;
                cvArray.Columns = iCol ;
        
                borders = new List<Border>[iRow];
                for (int r = 0; r < iRow; r++)
                {
                    borders[r] = new List<Border>();
                    for (int c = 0; c < iCol; c++)
                    {
                        Border bd = new Border();
                        //bd.Width  = (cvArray.ActualWidth  - 1)/iCol;
                        //bd.Height = (cvArray.ActualHeight - 1)/iRow;
                        bd.Width  = (cvArray.ActualWidth  )/iCol;
                        bd.Height = (cvArray.ActualHeight )/iRow;
                        bd.BorderThickness = new Thickness(0.5);
                        bd.BorderBrush     = Brushes.Black;
                        bd.Margin = new Thickness(1,1,1,1);
                        //bd.Padding         = new Thickness(0.1);

                        borders[r].Add(bd);
                        cvArray.Children.Add(borders[r][c]);
                    }
                }
                cvArray.EndInit();
            });

        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(IsVisible) Timer.Start();
            else          Timer.Stop ();
        }

        private void GridBackGround_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!ClickStat.bMouseDown) return;
            
            ClickStat.bMouseDown = false;
            gridBackGround.Children.Remove(DragRect);
            //cvArray.Children.Remove(DragRect);

            //cmArray[(int)EN_STATUS_POPUP_ID.spOne].Items.Clear();

            if (e.LeftButton == MouseButtonState.Released)
            {
                if (ClickStat.dSelX1 == ClickStat.dSelX2 && ClickStat.dSelY1 == ClickStat.dSelY2)
                {
                  
                    if (Keyboard.IsKeyDown(Key.LeftShift) && Keyboard.IsKeyDown(Key.LeftCtrl))
                    {
                        cvArray.ContextMenu = cmArray[(int)EN_STATUS_POPUP_ID.spAll];
                        cmArray[(int)EN_STATUS_POPUP_ID.spAll].IsOpen = true;
                    }
                    else if (Keyboard.IsKeyDown(Key.LeftCtrl))
                    {
                        cvArray.ContextMenu = cmArray[(int)EN_STATUS_POPUP_ID.spCol];
                        cmArray[(int)EN_STATUS_POPUP_ID.spCol].IsOpen = true;
                    }
                    else if (Keyboard.IsKeyDown(Key.LeftShift))
                    {
                        cvArray.ContextMenu = cmArray[(int)EN_STATUS_POPUP_ID.spRow];
                        cmArray[(int)EN_STATUS_POPUP_ID.spRow].IsOpen = true;
                    }
                    else
                    {
                        
                        int C=0 , R=0 ;
                        if (GetChipCR(ClickStat.dSelX1, ClickStat.dSelY1, ref C, ref R))
                        {
                            cmArray[(int)EN_STATUS_POPUP_ID.spOne].Items.Clear();
                            for (int s = 0; s < (int)cs.MAX_CHIP_STAT; s++)
                            {
                                MenuItem ArrayMenu = new MenuItem();
                                ArrayMenu.Background = SystemColors.ControlDarkBrush;
                                ArrayMenu.Header = C.ToString() + "__" + R.ToString() + "__" + StatProp[s].Caption; //숫자 앞뒤로 언더바가 표시가 안되서 1개 더 추가해서 보이도록 해놓음. 진섭
                                ArrayMenu.Tag = s;
                                ArrayMenu.Click += new RoutedEventHandler(this.OneClick);
                                ArrayMenu.Icon = ItemPaint(s);

                                ArrayMenu.Visibility = StatProp[s].Visible ;
                                ArrayMenu.IsEnabled  = StatProp[s].Enable  ;

                                if (StatProp[s].Caption != null) cmArray[(int)EN_STATUS_POPUP_ID.spOne].Items.Add(ArrayMenu);
                            }
                        }
                    
                        cvArray.ContextMenu = cmArray[(int)EN_STATUS_POPUP_ID.spOne];
                        cmArray[(int)EN_STATUS_POPUP_ID.spOne].IsOpen = true;
                    }
                }
            
                else
                {
                    cvArray.ContextMenu = cmArray[(int)EN_STATUS_POPUP_ID.spDrag];
                    cmArray[(int)EN_STATUS_POPUP_ID.spDrag].IsOpen = true;
                }
            }
            else if (e.RightButton == MouseButtonState.Released && Keyboard.IsKeyDown(Key.LeftCtrl))//Control.ModifierKeys == Keys.Control)
            {
                
            }
        }

        private void GridBackGround_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePoint = e.GetPosition(cvArray);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ClickStat.bMouseDown = true;

                ClickStat.dSelX1 = mousePoint.X;
                ClickStat.dSelY1 = mousePoint.Y;
                ClickStat.dSelX2 = mousePoint.X;
                ClickStat.dSelY2 = mousePoint.Y;

                int C = 0, R = 0;
                if (GetChipCR(ClickStat.dSelX1, ClickStat.dSelY1, ref C, ref R))
                {
                    ClickStat.iSelC1 = C;
                    ClickStat.iSelR1 = R;

                    ClickStat.iSelC2 = C;
                    ClickStat.iSelR2 = R;
                }
            }
        }

        private void GridBackGround_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!ClickStat.bMouseDown) return;

            Point mousePoint      = e.GetPosition(cvArray);
            Point BackGroundPoint = e.GetPosition(gridBackGround);
            ClickStat.dSelX2 = mousePoint.X;
            ClickStat.dSelY2 = mousePoint.Y;

            int R = 0, C = 0;
            if (GetChipCR(ClickStat.dSelX2, ClickStat.dSelY2, ref C, ref R))
            {
                ClickStat.iSelC2 = C;
                ClickStat.iSelR2 = R;
            }

            //마우스 드래그할때 마우스 동선 범위 보여주는 사각형
            gridBackGround.BeginInit();
            if (ClickStat.dSelX1 != 0 || ClickStat.dSelX2 != 0 || ClickStat.dSelY1 != 0 || ClickStat.dSelY2 != 0)
            {
                double dxGap = BackGroundPoint.X - mousePoint.X ;
                double dyGap = BackGroundPoint.Y - mousePoint.Y ;
                DragRect.Stroke = Brushes.Black;
                DragRect.StrokeDashArray = new DoubleCollection() { 2 };
                
                double dLeft   = ClickStat.dSelX2 + dxGap< ClickStat.dSelX1 + dxGap? ClickStat.dSelX2 + dxGap : ClickStat.dSelX1 + dxGap;
                double dTop    = ClickStat.dSelY2 + dyGap< ClickStat.dSelY1 + dyGap? ClickStat.dSelY2 + dyGap : ClickStat.dSelY1 + dyGap;
                double dRight  = ClickStat.dSelX2 + dxGap< ClickStat.dSelX1 + dxGap? ClickStat.dSelX1 + dxGap : ClickStat.dSelX2 + dxGap;
                double dBottom = ClickStat.dSelY2 + dyGap< ClickStat.dSelY1 + dyGap? ClickStat.dSelY1 + dyGap : ClickStat.dSelY2 + dyGap;
                
                double dWidth  = dRight  - dLeft;
                double dHeight = dBottom - dTop;
                
                DragRect.Width  = dWidth;
                DragRect.Height = dHeight;

                DragRect.Margin = new Thickness(dLeft,dTop,gridBackGround.ActualWidth - dRight,gridBackGround.ActualHeight - dBottom);
                
                gridBackGround.Children.Remove(DragRect);
                gridBackGround.Children.Add(DragRect);

            }
            gridBackGround.EndInit();
        }
    }

}
