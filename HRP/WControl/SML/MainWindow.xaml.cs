using COMMON;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using MaterialDesignThemes.Wpf;

namespace SML
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window //,INotifyPropertyChanged
    {
        private struct TErrTracker
        {
            public const int iThickness     =  4 ;
            public const int iDefaultWidth  = 30 ;
            public const int iDefaultHeight = 30 ;
            public System.Drawing.Rectangle Rect      ;
            public bool                     bDown     ;
            public EClickPos                eClickPos ;
             
            public int       iPosX     ;
            public int       iPosY     ;
            public int       iPrePosX  ;
            public int       iPrePosY  ;
        }

        TErrTracker m_tErrTracker ;

        private const string sFormText = "Dll MainWindow";


        public MainWindow()
        {
            InitializeComponent();

            lvError   .ItemsSource = Errordata    .GetInstance();
            lvInput   .ItemsSource = IOdata       .GetInput   ();
            lvOutput  .ItemsSource = IOdata       .GetOutput  ();
            lvAInput  .ItemsSource = AIOdata      .GetInput   ();
            lvAOutput .ItemsSource = AIOdata      .GetOutput  ();
            lvTLamp   .ItemsSource = TowerLampData.Getinstance();
            lvCylinder.ItemsSource = CylinderData .Getinstance();
            cbMotrSel .ItemsSource = MotorData    .Getinstance();
            

            lvError   .SelectedIndex = 0;
            lvInput   .SelectedIndex = 0;
            lvOutput  .SelectedIndex = 0;
            lvAInput  .SelectedIndex = 0; 
            lvAOutput .SelectedIndex = 0;
            lvTLamp   .SelectedIndex = 0;
            lvCylinder.SelectedIndex = 0;
            cbMotrSel .SelectedIndex = 0;

            //UI 타이머
            Timer.Interval = TimeSpan.FromMilliseconds(100);
            Timer.Tick += new EventHandler(Timer_Tick);

        }

        //---------------------------------------------------------------------------------------------
        //Timer
        //---------------------------------------------------------------------------------------------
        public DispatcherTimer Timer = new DispatcherTimer();
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (tabInput.IsSelected)
            {
                for (int i = 0; i < SM.DIO._iMaxIn; i++)
                {
                    IOdata.GetInput()[i].VVal = SM.DIO.m_aIn[i].Stat.bVtrVal ? " ON" : " OFF";
                    IOdata.GetInput()[i].AVal = SM.DIO.GetX(i, false)        ? " ON" : " OFF";
                    IOdata.GetInput()[i].VValBackground = SM.DIO.m_aIn[i].Stat.bVtrVal ?  Brushes.Lime : Brushes.Silver; 
                    IOdata.GetInput()[i].AValBackground = SM.DIO.GetX(i, false)        ?  Brushes.Lime : Brushes.Silver; 
                }
            }

            if (tabOutput.IsSelected)
            {
                for (int i = 0; i < SM.DIO._iMaxOut; i++)
                {
                    IOdata.GetOutput()[i].VVal = SM.DIO.m_aOut[i].Stat.bVtrVal ? " ON" : " OFF";
                    IOdata.GetOutput()[i].AVal = SM.DIO.GetY(i, false)         ? " ON" : " OFF";
                    IOdata.GetOutput()[i].VValBackground = SM.DIO.m_aOut[i].Stat.bVtrVal ? Brushes.Lime : Brushes.Silver; 
                    IOdata.GetOutput()[i].AValBackground = SM.DIO.GetY(i, false)         ? Brushes.Lime : Brushes.Silver; 
                }            
            }
            
            if (tabAIO.IsSelected)
            {
                for (int i = 0; i < SM.AIO._iMaxIn; i++)
                {
                    double dAVal = SM.AIO.GetX(i);
                    double dDVal = SM.AIO.GetX(i,true);

                    AIOdata.GetInput()[i].AVal = dAVal.ToString();
                    AIOdata.GetInput()[i].DVal = dDVal.ToString();
                }            
                for (int i = 0; i < SM.AIO._iMaxOut; i++)
                {
                    double dVal = SM.AIO.GetY(i);
                    AIOdata.GetOutput()[i].Val  = dVal.ToString();
                    //AIOdata.GetOutput()[i].Val  = dVal.ToString();
                }            
            }

            if (tabCylinder.IsSelected)
            {
                if (tbCylNo.Text != "")
                {
                    int iSelNo = CConfig.StrToIntDef(tbCylNo.Text);
                    SM.CYL.WpfDisplayStatus(iSelNo, lbFwdStat, lbBwdStat, lbAlarm);
                }
            }

            if (tabMotor.IsSelected)
            {
                if (m_iPreCrntLevel != SM.FrmLogOn.GetLevel()) DispMotr();

                int iMotrSel = cbMotrSel.SelectedIndex;

                lbStop    .Background = SM.MTR.GetStop       (iMotrSel) ? Brushes.Lime : Brushes.Silver;
                lbInpos   .Background = SM.MTR.GetInPosSgnl  (iMotrSel) ? Brushes.Lime : Brushes.Silver;
                lbServo   .Background = SM.MTR.GetServo      (iMotrSel) ? Brushes.Lime : Brushes.Silver;
                lbHomeDone.Background = SM.MTR.GetHomeDone   (iMotrSel) ? Brushes.Lime : Brushes.Silver;
                lbBrakeOff.Background = SM.MTR.GetBreakOff   (iMotrSel) ? Brushes.Lime : Brushes.Silver;
                lbMtAlarm .Background = SM.MTR.GetAlarmSgnl  (iMotrSel) ? Brushes.Lime : Brushes.Silver;
                lbDirP    .Background = SM.MTR.Stat[iMotrSel].bMovingP  ? Brushes.Lime : Brushes.Silver;
                lbDirN    .Background = SM.MTR.Stat[iMotrSel].bMovingN  ? Brushes.Lime : Brushes.Silver;
                lbZPhase  .Background = SM.MTR.GetZphaseSgnl (iMotrSel) ? Brushes.Lime : Brushes.Silver;
                lbLimitP  .Background = SM.MTR.GetPLimSnsr   (iMotrSel) ? Brushes.Lime : Brushes.Silver;
                lbHome    .Background = SM.MTR.GetHomeSnsr   (iMotrSel) ? Brushes.Lime : Brushes.Silver;
                lbLimitN  .Background = SM.MTR.GetNLimSnsr   (iMotrSel) ? Brushes.Lime : Brushes.Silver;

                tbCmdPos.Text = SM.MTR.GetCmdPos(iMotrSel).ToString();
                tbEncPos.Text = SM.MTR.GetEncPos(iMotrSel).ToString();
                tbTrgPos.Text = SM.MTR.GetTrgPos(iMotrSel).ToString();

                lbX0.Background = SM.MTR.GetX(iMotrSel,0) ? Brushes.Lime : Brushes.Silver;
                lbX1.Background = SM.MTR.GetX(iMotrSel,1) ? Brushes.Lime : Brushes.Silver;
                lbX2.Background = SM.MTR.GetX(iMotrSel,2) ? Brushes.Lime : Brushes.Silver;
                lbX3.Background = SM.MTR.GetX(iMotrSel,3) ? Brushes.Lime : Brushes.Silver;
                lbX4.Background = SM.MTR.GetX(iMotrSel,4) ? Brushes.Lime : Brushes.Silver;
                                      
                lbY0.Background = SM.MTR.GetY(iMotrSel,0) ? Brushes.Lime : Brushes.Silver;
                lbY1.Background = SM.MTR.GetY(iMotrSel,1) ? Brushes.Lime : Brushes.Silver;
                lbY2.Background = SM.MTR.GetY(iMotrSel,2) ? Brushes.Lime : Brushes.Silver;
                lbY3.Background = SM.MTR.GetY(iMotrSel,3) ? Brushes.Lime : Brushes.Silver;
                lbY4.Background = SM.MTR.GetY(iMotrSel,4) ? Brushes.Lime : Brushes.Silver;

                //표기 추가
                double dRPM = SM.MTR.Para[iMotrSel].dRepeatSpeed / SM.MTR.Para[iMotrSel].dUnitPerRev * 60 ;
                lbRPM.Text = "REPEAT SPEED = " + dRPM.ToString("N0") + " RPM";
            }

        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(IsVisible) Timer.Start();
            else          Timer.Stop ();

            DefaultMode();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl) DefaultMode();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(Keyboard.IsKeyDown(Key.LeftAlt) && Keyboard.IsKeyDown(Key.A))
            {
                string sHeader = ((System.Windows.Controls.HeaderedContentControl)tabControl.SelectedItem).Header.ToString() ;
                if(sHeader == "ERROR"     ) BtErrApply_Click  (this,null);//btErrApply  .PerformClick();
                if(sHeader == "INPUT"     ) BtDiApply_Click   (this,null);//btErrApply  .PerformClick();
                if(sHeader == "OUTPUT"    ) BtDoApply_Click   (this,null);//btErrApply  .PerformClick();
                if(sHeader == "AIO"       ) BtAioApply_Click  (this,null);//btErrApply  .PerformClick();
                if(sHeader == "TOWER LAMP") BtTLampApply_Click(this,null);//btErrApply  .PerformClick();
                if(sHeader == "CYLINDER"  ) BtCylApply_Click  (this,null);//btErrApply  .PerformClick();
            }
            if(Keyboard.IsKeyDown(Key.LeftAlt) && Keyboard.IsKeyDown(Key.S))
            {
                string sHeader = ((System.Windows.Controls.HeaderedContentControl)tabControl.SelectedItem).Header.ToString() ;
                if(sHeader == "ERROR"     ) BtSaveErr_Click  (this,null);//btErrApply  .PerformClick();
                if(sHeader == "INPUT"     ) BtDiSave_Click   (this,null);//btErrApply  .PerformClick();
                if(sHeader == "OUTPUT"    ) BtDoSave_Click   (this,null);//btErrApply  .PerformClick();
                if(sHeader == "AIO"       ) BtAioSave_Click  (this,null);//btErrApply  .PerformClick();
                if(sHeader == "TOWER LAMP") BtSaveTLamp_Click(this,null);//btErrApply  .PerformClick();
                if(sHeader == "CYLINDER"  ) BtSaveCyl_Click  (this,null);//btErrApply  .PerformClick();
            }
        }

        private void DefaultMode()
        {
            //cbDiDirect  .IsChecked = false;
            cbDiTestMode.IsChecked = false;
            cbDoDirect  .IsChecked = false;
            cbDoTestMode.IsChecked = false;
            cbTLTestMode.IsChecked = false;

            SM.DIO.SetTestMode(false);
            SM.AIO.SetTestMode(false);
            SM.TWL.SetTestMode(false);

            SM.CYL.StopRpt();
        }

        public void TabHide(int _iIndex)
        {

        }

        //---------------------------------------------------------------------------------------------
        //Error
        //---------------------------------------------------------------------------------------------
        #region Error
        private void BtOpenImg_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = "*.*";
            //dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif|ALL Files (*.*)|*.*";
            dlg.Filter = "Images Files(*.jpg; *.jpeg; *.gif; *.bmp; *.png)|*.jpg;*.jpeg;*.gif;*.bmp;*.png|All Files|*.*";

            // Display OpenFileDialog by calling ShowDialog method 
            bool ? result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                string fileFullName = dlg.FileName;
                tbErrImg.Text = fileFullName;
                
                //BitmapImage bitmap = new BitmapImage();
                //bitmap.BeginInit();
                //bitmap.UriSource = new Uri(fileFullName);
                //bitmap.EndInit();
                
                //Using Image 
                //pbErrImg.Source = bitmap;
                //this.pbErrImg.Stretch = Stretch.Uniform ;

                //MediaElement
                pbErrImg.UnloadedBehavior = MediaState.Manual;
                pbErrImg.LoadedBehavior   = MediaState.Play;
                //pbErrImg1.Source = new Uri("file://" + tbErrImg.Text);
                pbErrImg.Source = new Uri(tbErrImg.Text);
            }
        }

        private void BtErrApply_Click(object sender, RoutedEventArgs e)
        {
            btErrApply.IsEnabled = false;

            Log.Trace(this.GetType().Name + " Apply Button Clicked", ForContext.Frm);
            
            if (tbErrNo.Text != "")
            {
                int iSelNo = CConfig.StrToIntDef(tbErrNo.Text);

                CErrMan.TPara Para = SM.ERR.GetErrConfig(iSelNo);
                Para.sEnum    = tbErrEnum.Text;
                Para.sName    = tbErrName.Text;
                Para.sAction  = tbErrAct .Text;
                Para.sImgPath = tbErrImg .Text;

                Para.dRectLeft   = m_tErrTracker.Rect.X     ;
                Para.dRectTop    = m_tErrTracker.Rect.Y     ;
                Para.dRectWidth  = m_tErrTracker.Rect.Width ;
                Para.dRectHeight = m_tErrTracker.Rect.Height;

                if(rbError.IsChecked == true) Para.iErrorLevel = (int)EN_ERR_LEVEL.Error;
                else                          Para.iErrorLevel = (int)EN_ERR_LEVEL.Info ;

                SM.ERR.SetErrConfig(iSelNo, Para);

                Errordata.New();
            }

            btErrApply.IsEnabled = true;
        }

        private void BtSaveErr_Click(object sender, RoutedEventArgs e)
        {
            btSaveErr.IsEnabled = false;

            Log.Trace(this.GetType().Name + " Save Button Clicked", ForContext.Frm);

            SM.ERR.LoadSave(false);
            btSaveErr.IsEnabled = true;
        }

        private void LvError_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvError.SelectedIndex == -1) return;
            
            int iSelNo = lvError.SelectedIndex ; 

            tbErrNo.Text   = iSelNo.ToString();
            tbErrEnum.Text = SM.ERR.GetErrConfig(iSelNo).sEnum;
            tbErrName.Text = SM.ERR.GetErrConfig(iSelNo).sName;
            tbErrAct.Text  = SM.ERR.GetErrConfig(iSelNo).sAction;
            tbErrImg.Text  = SM.ERR.GetErrConfig(iSelNo).sImgPath;

            m_tErrTracker.Rect.X      = (int)SM.ERR.GetErrConfig(iSelNo).dRectLeft;
            m_tErrTracker.Rect.Y      = (int)SM.ERR.GetErrConfig(iSelNo).dRectTop;
            m_tErrTracker.Rect.Width  = (int)SM.ERR.GetErrConfig(iSelNo).dRectWidth;
            m_tErrTracker.Rect.Height = (int)SM.ERR.GetErrConfig(iSelNo).dRectHeight;

            if(SM.ERR.GetErrLevel(iSelNo) == EN_ERR_LEVEL.Error) rbError.IsChecked = true;
            else                                                 rbInfo .IsChecked = true;

            if (tbErrImg.Text.Trim() == "")
            {
                pbErrImg.Source = null;
                return;
            }

            FileInfo Info = new FileInfo(tbErrImg.Text);

            if (!Info.Exists)
            {
                pbErrImg.Source = null;
                //Log.ShowMessage("File Not Exist", tbErrImg.Text);
                return;
            }

            try
            {
                //BitmapImage bitmap = new BitmapImage();
                //bitmap.BeginInit();
                //bitmap.UriSource = new Uri(tbErrImg.Text);
                //bitmap.EndInit();
                //MediaElement
                pbErrImg.UnloadedBehavior = MediaState.Manual;
                pbErrImg.LoadedBehavior   = MediaState.Play;
                //pbErrImg1.Source = new Uri("file://" + tbErrImg.Text);
                pbErrImg.Source = new Uri(tbErrImg.Text);

                DrawRect();

            }
            catch(Exception ex)
            {
                Log.ShowMessage("Exception", ex.Message);
                throw new FileNotFoundException(ex.Message);
            }

        }

        private void PbErrImg_MediaEnded(object sender, RoutedEventArgs e)
        {
            pbErrImg.Position = new TimeSpan(0, 0, 1);
            pbErrImg.Play();
        }

        bool CheckRectIn(int _iLeftX, int _iTopY,int _iRightX, int _iBottomY, int _iX, int _iY) //사각포인트 중간크기를 넘김.
        {
            return _iX > _iLeftX && _iX <= _iRightX &&
                   _iY > _iTopY  && _iY <= _iBottomY;
        }

        private void DrawRect()
        {
            //사각형 그리기 연습
            try
            {
                cvsErrImg.Children.Clear();
                //cvsErrImg.BeginInit();

                
                
                Rectangle rectangle = new Rectangle();
                rectangle.Width  = m_tErrTracker.Rect.Width ; //넓이 설정
                rectangle.Height = m_tErrTracker.Rect.Height; //높이 설정
                rectangle.Stroke = Brushes.Blue; //테두리 색 설정
                rectangle.StrokeThickness = TErrTracker.iThickness ;
                 
                //위치 설정
                Canvas.SetTop (rectangle, m_tErrTracker.Rect.Y);
                Canvas.SetLeft(rectangle, m_tErrTracker.Rect.X); 

                //Canvas에 추가
                cvsErrImg.Children.Add(rectangle);
                //cvsErrImg.EndInit();      
            }
            catch(Exception ex)
            {
                Log.Trace(ex.ToString());
            }


        }

        private void TabItem_MouseMove(object sender, MouseEventArgs e)
        {

            int X = (int)e.GetPosition(cvsErrImg).X ;
            int Y = (int)e.GetPosition(cvsErrImg).Y ;

            int x = X - m_tErrTracker.iPrePosX;
            int y = Y - m_tErrTracker.iPrePosY;

            if(m_tErrTracker.bDown)
            {
                if(m_tErrTracker.eClickPos == EClickPos.Move)
                {
                    m_tErrTracker.Rect.X = m_tErrTracker.Rect.X + x;
                    m_tErrTracker.Rect.Y = m_tErrTracker.Rect.Y + y;

                    m_tErrTracker.iPrePosX = X;
                    m_tErrTracker.iPrePosY = Y;
                }

                else if (m_tErrTracker.eClickPos == EClickPos.LeftTop)
                {
                    m_tErrTracker.Rect.X = m_tErrTracker.Rect.X + x;
                    m_tErrTracker.Rect.Y = m_tErrTracker.Rect.Y + y;

                    m_tErrTracker.Rect.Width  = m_tErrTracker.Rect.Width  - x;
                    m_tErrTracker.Rect.Height = m_tErrTracker.Rect.Height - y;

                    m_tErrTracker.iPrePosX = X;
                    m_tErrTracker.iPrePosY = Y;
                }

                else if (m_tErrTracker.eClickPos == EClickPos.Top)
                {
                    m_tErrTracker.Rect.Y = m_tErrTracker.Rect.Y + y;

                    m_tErrTracker.Rect.Height = m_tErrTracker.Rect.Height - y;

                    m_tErrTracker.iPrePosX = X;
                    m_tErrTracker.iPrePosY = Y;
                }

                else if (m_tErrTracker.eClickPos == EClickPos.RightTop)
                {
                    m_tErrTracker.Rect.Y = m_tErrTracker.Rect.Y + y;

                    m_tErrTracker.Rect.Width = m_tErrTracker.Rect.Width + x;
                    m_tErrTracker.Rect.Height = m_tErrTracker.Rect.Height - y;

                    m_tErrTracker.iPrePosX = X;
                    m_tErrTracker.iPrePosY = Y;
                }

                else if (m_tErrTracker.eClickPos == EClickPos.Right)
                {
                    m_tErrTracker.Rect.Width = m_tErrTracker.Rect.Width + x;
                  

                    m_tErrTracker.iPrePosX = X;
                    m_tErrTracker.iPrePosY = Y;
                }

                else if (m_tErrTracker.eClickPos == EClickPos.RightBottom)
                {
                    m_tErrTracker.Rect.Width = m_tErrTracker.Rect.Width + x;
                    m_tErrTracker.Rect.Height = m_tErrTracker.Rect.Height + y;

                    m_tErrTracker.iPrePosX = X;
                    m_tErrTracker.iPrePosY = Y;
                }

                else if (m_tErrTracker.eClickPos == EClickPos.Bottom)
                {
                    m_tErrTracker.Rect.Height = m_tErrTracker.Rect.Height + y;

                    m_tErrTracker.iPrePosX = X;
                    m_tErrTracker.iPrePosY = Y;
                }

                else if (m_tErrTracker.eClickPos == EClickPos.LeftBottom)
                {
                    m_tErrTracker.Rect.X = m_tErrTracker.Rect.X + x;


                    m_tErrTracker.Rect.Width  = m_tErrTracker.Rect.Width  - x;
                    m_tErrTracker.Rect.Height = m_tErrTracker.Rect.Height + y;

                    m_tErrTracker.iPrePosX = X;
                    m_tErrTracker.iPrePosY = Y;
                }

                else if (m_tErrTracker.eClickPos == EClickPos.Left)
                {
                    m_tErrTracker.Rect.X = m_tErrTracker.Rect.X + x;


                    m_tErrTracker.Rect.Width = m_tErrTracker.Rect.Width - x;


                    m_tErrTracker.iPrePosX = X;
                    m_tErrTracker.iPrePosY = Y;
                }
            }
            
            DrawRect();
        }

        private void TabItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                m_tErrTracker.Rect.X      = 0  ;
                m_tErrTracker.Rect.Y      = 0  ;
                m_tErrTracker.Rect.Width  = TErrTracker.iDefaultWidth  ;
                m_tErrTracker.Rect.Height = TErrTracker.iDefaultHeight ;

                DrawRect();
                return;
            }

            m_tErrTracker.bDown = true;

            int X = (int)e.GetPosition(cvsErrImg).X ;
            int Y = (int)e.GetPosition(cvsErrImg).Y ;

            if (m_tErrTracker.Rect.IsEmpty)
            {
                m_tErrTracker.Rect.X = X ;
                m_tErrTracker.Rect.Y = Y ;
            }

            int iCntX = (m_tErrTracker.Rect.Left+m_tErrTracker.Rect.Right )/2 ;
            int iCntY = (m_tErrTracker.Rect.Top +m_tErrTracker.Rect.Bottom)/2 ;
            const int iThick = TErrTracker.iThickness + 30;

                 if (CheckRectIn(m_tErrTracker.Rect.Right - iThick, m_tErrTracker.Rect.Bottom - iThick, m_tErrTracker.Rect.Right + iThick, m_tErrTracker.Rect.Bottom + iThick, X, Y)) m_tErrTracker.eClickPos = EClickPos.RightBottom;
            else if (CheckRectIn(m_tErrTracker.Rect.Right - iThick, iCntY                     - iThick, m_tErrTracker.Rect.Right + iThick, iCntY                     + iThick, X, Y)) m_tErrTracker.eClickPos = EClickPos.Right;
            else if (CheckRectIn(iCntX                    - iThick, m_tErrTracker.Rect.Bottom - iThick, iCntX                    + iThick, m_tErrTracker.Rect.Bottom + iThick, X, Y)) m_tErrTracker.eClickPos = EClickPos.Bottom;
            else if (CheckRectIn(m_tErrTracker.Rect.Left  - iThick, m_tErrTracker.Rect.Bottom - iThick, m_tErrTracker.Rect.Left  + iThick, m_tErrTracker.Rect.Bottom + iThick, X, Y)) m_tErrTracker.eClickPos = EClickPos.LeftBottom;
            else if (CheckRectIn(m_tErrTracker.Rect.Left  - iThick, iCntY                     - iThick, m_tErrTracker.Rect.Left  + iThick, iCntY                     + iThick, X, Y)) m_tErrTracker.eClickPos = EClickPos.Left;
            else if (CheckRectIn(m_tErrTracker.Rect.Left  - iThick, m_tErrTracker.Rect.Top    - iThick, m_tErrTracker.Rect.Left  + iThick, m_tErrTracker.Rect.Top    + iThick, X, Y)) m_tErrTracker.eClickPos = EClickPos.LeftTop;
            else if (CheckRectIn(iCntX                    - iThick, m_tErrTracker.Rect.Top    - iThick, iCntX                    + iThick, m_tErrTracker.Rect.Top    + iThick, X, Y)) m_tErrTracker.eClickPos = EClickPos.Top;
            else if (CheckRectIn(m_tErrTracker.Rect.Right - iThick, m_tErrTracker.Rect.Top    - iThick, m_tErrTracker.Rect.Right + iThick, m_tErrTracker.Rect.Top    + iThick, X, Y)) m_tErrTracker.eClickPos = EClickPos.RightTop;
            

            else     m_tErrTracker.eClickPos = EClickPos.Move;
            //else if (CheckRectShapeIn(m_tErrTracker.Rect, e.X, e.Y, TErrTracker.iThickness  + m_tErrTracker.Rect.Width )) m_tErrTracker.eClickPos = EClickPos.Move;
            //else if (!CheckRectShapeIn(m_tErrTracker.Rect, e.X, e.Y, TErrTracker.iThickness + m_tErrTracker.Rect.Width )) m_tErrTracker.eClickPos = EClickPos.None;

            m_tErrTracker.iPrePosX = X ;
            m_tErrTracker.iPrePosY = Y ;

        }

        private void TabItem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            m_tErrTracker.bDown = false;
        }

        private void TabItem_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
        }
        #endregion
        //---------------------------------------------------------------------------------------------
        //Error
        //---------------------------------------------------------------------------------------------

        //---------------------------------------------------------------------------------------------
        //Input
        //---------------------------------------------------------------------------------------------
        #region Input
        private void LvInput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvInput.SelectedIndex == -1) return;
            int iSelNo = lvInput.SelectedIndex ; 

            tbDiNo.Text       = iSelNo.ToString();
            tbDiEnum.Text     = SM.DIO.GetInputPara(iSelNo).sEnum;
            tbDiName.Text     = SM.DIO.GetInputPara(iSelNo).sName;
            tbDiAdd.Text      = SM.DIO.GetInputPara(iSelNo).iAdd.ToString();
            //tbDiComment.Text  = SM.DIO.GetInputPara(iSelNo).SComt;
            tbDiOnDelay.Text  = SM.DIO.GetInputPara(iSelNo).iOnDelay.ToString();
            tbDiOffDelay.Text = SM.DIO.GetInputPara(iSelNo).iOffDelay.ToString();
            cbDiInv.IsChecked = SM.DIO.GetInputPara(iSelNo).bInv;
            cbDiLog.IsChecked = !SM.DIO.GetInputPara(iSelNo).bNotLog;
        }


        private void BtDiApply_Click(object sender, RoutedEventArgs e)
        {
            btDiApply.IsEnabled = false;
            Log.Trace(this.GetType().Name + " APPLY Button Clicked", ForContext.Frm);

            if (tbDiNo.Text != "")
            {
                int iSelNo = CConfig.StrToIntDef(tbDiNo.Text);

                //Para.iAdd = CIniFile.StrToIntDef(tbDioAdd.Text, 0);
                CDioMan.TPara InPara = SM.DIO.GetInputPara(iSelNo);
                InPara.iAdd          = CConfig.StrToIntDef(tbDiAdd     .Text);
                InPara.iOffDelay     = CConfig.StrToIntDef(tbDiOffDelay.Text);
                InPara.iOnDelay      = CConfig.StrToIntDef(tbDiOnDelay .Text);
                //InPara.SComt         = tbDioComment.Text;
                InPara.sName         = tbDiName.Text;
                InPara.bInv          = cbDiInv.IsChecked == true;
                InPara.bNotLog       = cbDiLog.IsChecked != true;
                

                SM.DIO.SetInputPara(iSelNo, InPara);

                IOdata.NewInput();
                //inputdata.
            }
            btDiApply.IsEnabled = true;
        }

        private void BtDiSave_Click(object sender, RoutedEventArgs e)
        {
            btDiSave.IsEnabled = false;
            Log.Trace(this.GetType().Name + " SAVE Button Clicked", ForContext.Frm);

            SM.DIO.LoadSave(false);
            btDiSave.IsEnabled = true;
        }

        private void BtDiInit_Click(object sender, RoutedEventArgs e)
        {
            if (!Log.ShowMessageModal("Confirm", "ENUM, ADDRESS INITIALIZATION ?")) return;
            SM.DIO.SetAllInputName();
            SM.DIO.SetAllInputAdd ();

            IOdata.NewInput();
        }
        #endregion
        //---------------------------------------------------------------------------------------------
        //Input
        //---------------------------------------------------------------------------------------------

        //---------------------------------------------------------------------------------------------
        //Output
        //---------------------------------------------------------------------------------------------
        #region Output
        private void BtDoApply_Click(object sender, RoutedEventArgs e)
        {
            btDoApply.IsEnabled = false;

            Log.Trace(this.GetType().Name + " APPLY Button Clicked", ForContext.Frm);

            if (tbDoNo.Text != "")
            {
                int iSelNo = CConfig.StrToIntDef(tbDoNo.Text);

                //Para.iAdd = CIniFile.StrToIntDef(tbDioAdd.Text, 0);
                CDioMan.TPara Para = SM.DIO.GetOutputPara(iSelNo);
                Para.iAdd          = CConfig.StrToIntDef(tbDoAdd     .Text);
                Para.iOffDelay     = CConfig.StrToIntDef(tbDoOffDelay.Text);
                Para.iOnDelay      = CConfig.StrToIntDef(tbDoOnDelay .Text);
                //Para.SComt         = tbDoComment.Text;
                Para.sName         = tbDoName.Text;
                Para.bInv          = cbDoInv.IsChecked == true;
                Para.bNotLog       = cbDoLog.IsChecked != true;
                

                SM.DIO.SetOutputPara(iSelNo, Para);

                IOdata.NewOutput(); //여기가 타이머에서 갱신중인데 접근하는 부분이라 타이머 중간에 갱신할거 같은데 괜찮네;;;
            }

            btDoApply.IsEnabled = true;
        }

        private void BtDoSave_Click(object sender, RoutedEventArgs e)
        {
            btDoSave.IsEnabled = false;
            Log.Trace(this.GetType().Name + " SAVE Button Clicked", ForContext.Frm);

            SM.DIO.LoadSave(false);
            btDoSave.IsEnabled = true;
        }

        private void LvOutput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvOutput.SelectedIndex == -1) return;
            int iSelNo = lvOutput.SelectedIndex ; 

            tbDoNo.Text       =  iSelNo.ToString();
            tbDoEnum.Text     =  SM.DIO.GetOutputPara(iSelNo).sEnum;
            tbDoName.Text     =  SM.DIO.GetOutputPara(iSelNo).sName;
            tbDoAdd.Text      =  SM.DIO.GetOutputPara(iSelNo).iAdd.ToString();
            //tbDoComment.Text  = SM.DIO.GetOutputPara(iSelNo).SComt;
            tbDoOnDelay.Text  =  SM.DIO.GetOutputPara(iSelNo).iOnDelay.ToString();
            tbDoOffDelay.Text =  SM.DIO.GetOutputPara(iSelNo).iOffDelay.ToString();
            cbDoInv.IsChecked =  SM.DIO.GetOutputPara(iSelNo).bInv;
            cbDoLog.IsChecked = !SM.DIO.GetOutputPara(iSelNo).bNotLog;
        }

        private void LvOutput_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lvAOutput.SelectedIndex == -1) return;
            //int iSelNo = lvAOutput.SelectedIndex ; 
            int iSelNo = CConfig.StrToIntDef(tbDoNo.Text);

            if (SM.DIO.GetTestMode()) SM.DIO.SetYTestMode(iSelNo, !SM.DIO.GetY(iSelNo));
            else                      SM.DIO.SetY        (iSelNo, !SM.DIO.GetY(iSelNo), cbDoDirect.IsChecked == true);
        }

        private void CbDoDirect_Click(object sender, RoutedEventArgs e)
        {
            SM.DIO.SetTestMode(cbDoTestMode.IsChecked == true);
        }

        private void BtDoInit_Click(object sender, RoutedEventArgs e)
        {
            if (!Log.ShowMessageModal("Confirm", "ENUM, ADDRESS INITIALIZATION ?")) return;
            SM.DIO.SetAllOutputName();
            SM.DIO.SetAllOutputAdd ();

            IOdata.NewOutput();
        }
        #endregion
        //---------------------------------------------------------------------------------------------
        //AIO
        //---------------------------------------------------------------------------------------------
        #region AIO
        private void BtAioApply_Click(object sender, RoutedEventArgs e)
        {
            btAioApply.IsEnabled = false;

            Log.Trace(this.GetType().Name + " APPLY Button Clicked", ForContext.Frm);

            if (tbAiNo.Text != "")
            {
                int iSelNo = CConfig.StrToIntDef(tbAiNo.Text);

                CAioMan.TPara InPara = SM.AIO.GetInputPara(iSelNo);
                InPara.iAdd = CConfig.StrToIntDef(tbAiAdd.Text);
                //InPara.SComt = tbAiComment.Text;
                InPara.sName = tbAiName.Text;

                SM.AIO.SetInputPara(iSelNo, InPara);

                AIOdata.NewInput();
            }
            if (tbAoNo.Text != "")
            {
                int iSelNo = CConfig.StrToIntDef(tbAoNo.Text,0);

                CAioMan.TPara OutPara = SM.AIO.GetOutputPara(iSelNo);
                OutPara.iAdd = CConfig.StrToIntDef(tbAoAdd.Text);
                //OutPara.SComt = tbAoComment.Text;
                OutPara.sName = tbAoName.Text;

                SM.AIO.SetOutputPara(iSelNo, OutPara);

                AIOdata.NewOutput();
            }

            btAioApply.IsEnabled = true;
        }

        private void BtAioSave_Click(object sender, RoutedEventArgs e)
        {
            btAioSave.IsEnabled = false;
            Log.Trace(this.GetType().Name + " SAVE Button Clicked", ForContext.Frm);

            SM.AIO.LoadSave(false);
            btAioSave.IsEnabled = true;
        }

        private void LvAInput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvAInput.SelectedIndex == -1) return;
            int iSelNo = lvAInput.SelectedIndex ; 

            tbAiNo.Text       = iSelNo.ToString();
            tbAiEnum.Text     = SM.AIO.GetInputPara(iSelNo).sEnum;
            tbAiName.Text     = SM.AIO.GetInputPara(iSelNo).sName;
            tbAiAdd.Text      = SM.AIO.GetInputPara(iSelNo).iAdd.ToString();
        }

        private void LvAOutput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvAOutput.SelectedIndex == -1) return;
            int iSelNo = lvAOutput.SelectedIndex ; 

            tbAoNo.Text       = iSelNo.ToString();
            tbAoEnum.Text     = SM.AIO.GetOutputPara(iSelNo).sEnum;
            tbAoName.Text     = SM.AIO.GetOutputPara(iSelNo).sName;
            tbAoAdd.Text      = SM.AIO.GetOutputPara(iSelNo).iAdd.ToString();
        }

        private void BtAioSetOut_Click(object sender, RoutedEventArgs e)
        {
            if (lvAOutput.SelectedIndex == -1) return;
            int iSelNo = lvAOutput.SelectedIndex ; 

            Log.Trace(this.GetType().Name + " SET OUT Button Clicked", ForContext.Frm);

            double dVol = CConfig.StrToDoubleDef(tbAoTest.Text);
            SM.AIO.SetTestMode(true);
            SM.AIO.SetYTestMode(iSelNo,dVol);
        }

        #endregion
        //---------------------------------------------------------------------------------------------
        //AIO
        //---------------------------------------------------------------------------------------------


        //---------------------------------------------------------------------------------------------
        //Towerlamp
        //---------------------------------------------------------------------------------------------
        #region TowerLamp
        private void BtTLampApply_Click(object sender, RoutedEventArgs e)
        {
            if (lvTLamp.SelectedIndex == -1) return;
            int iSelNo = lvTLamp.SelectedIndex ; 

            btTLampApply.IsEnabled = false;
            Log.Trace(this.GetType().Name + " APPLY Button Clicked", ForContext.Frm);

            if (tbRedAdd.Text != "")
            {
                EN_SEQ_STAT eStat = (EN_SEQ_STAT)iSelNo;

                CTowerLampMan.TPara     Para = SM.TWL.GetTLampPara();
                CTowerLampMan.TLampInfo Info = SM.TWL.GetTLampInfo(eStat);

                Para.iRedAdd = CConfig.StrToIntDef(tbRedAdd.Text);
                Para.iYelAdd = CConfig.StrToIntDef(tbYelAdd.Text);
                Para.iGrnAdd = CConfig.StrToIntDef(tbGrnAdd.Text);
                Para.iSndAdd = CConfig.StrToIntDef(tbSntAdd.Text);

                SM.TWL.SetTLampPara(Para);

                Info.iRed  = (CTowerLampMan.EN_LAMP_OPER)cbRedSeq.SelectedIndex;
                Info.iYel  = (CTowerLampMan.EN_LAMP_OPER)cbYelSeq.SelectedIndex;
                Info.iGrn  = (CTowerLampMan.EN_LAMP_OPER)cbGrnSeq.SelectedIndex;
                Info.iBuzz = (CTowerLampMan.EN_LAMP_OPER)cbSndSeq.SelectedIndex;

                SM.TWL.SetTLampInfo(eStat, Info);

                TowerLampData.New();

            }
            btTLampApply.IsEnabled = true;
        }

        private void BtSaveTLamp_Click(object sender, RoutedEventArgs e)
        {
            btSaveTLamp.IsEnabled = false;
            Log.Trace(this.GetType().Name + "SAVE Button Clicked", ForContext.Frm);

            SM.TWL.LoadSave(false);
            btSaveTLamp.IsEnabled = true;
        }

        private void CbTLTestMode_Click(object sender, RoutedEventArgs e)
        {
            SM.TWL.SetTestMode(true);
        }

        private void LvTLamp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvTLamp.SelectedIndex == -1) return;
            int iSelNo = lvTLamp.SelectedIndex ; 

            EN_SEQ_STAT eStat = (EN_SEQ_STAT)iSelNo;
            
            cbRedSeq.SelectedIndex = (int)SM.TWL.GetTLampInfo(eStat).iRed ;
            cbYelSeq.SelectedIndex = (int)SM.TWL.GetTLampInfo(eStat).iYel ;
            cbGrnSeq.SelectedIndex = (int)SM.TWL.GetTLampInfo(eStat).iGrn ;
            cbSndSeq.SelectedIndex = (int)SM.TWL.GetTLampInfo(eStat).iBuzz;
        
            tbRedAdd.Text = SM.TWL.GetTLampPara().iRedAdd.ToString();
            tbYelAdd.Text = SM.TWL.GetTLampPara().iYelAdd.ToString();
            tbGrnAdd.Text = SM.TWL.GetTLampPara().iGrnAdd.ToString();
            tbSntAdd.Text = SM.TWL.GetTLampPara().iSndAdd.ToString();
        }
        #endregion
        //---------------------------------------------------------------------------------------------
        //Towerlamp
        //---------------------------------------------------------------------------------------------

        //---------------------------------------------------------------------------------------------
        //Cylinder
        //---------------------------------------------------------------------------------------------
        #region Cylinder
        private void LvCylinder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvCylinder.SelectedIndex == -1) return;
            int iSelNo = lvCylinder.SelectedIndex ; 

            tbCylNo      .Text   = iSelNo.ToString();
            tbCylEnum    .Text   = SM.CYL.GetCylinderPara(iSelNo).sEnum;
            tbCylName    .Text   = SM.CYL.GetCylinderPara(iSelNo).sName;
            //tbCylComt    .Text   = SM.CYL.GetCylinderPara(iSelNo).sComment;
            tbCylAddrIF  .Text   = SM.CYL.GetCylinderPara(iSelNo).iFwdXAdd.ToString();
            tbCylAddrIB  .Text   = SM.CYL.GetCylinderPara(iSelNo).iBwdXAdd.ToString();
            tbCylAddrOF  .Text   = SM.CYL.GetCylinderPara(iSelNo).iFwdYAdd   .ToString();
            tbCylAddrOB  .Text   = SM.CYL.GetCylinderPara(iSelNo).iBwdYAdd   .ToString();
            tbCylOnDelayF.Text   = SM.CYL.GetCylinderPara(iSelNo).iFwdOnDelay.ToString();
            tbCylOnDelayB.Text   = SM.CYL.GetCylinderPara(iSelNo).iBwdOnDelay.ToString();
            tbCylTimeOutF.Text   = SM.CYL.GetCylinderPara(iSelNo).iFwdTimeOut.ToString();
            tbCylTimeOutB.Text   = SM.CYL.GetCylinderPara(iSelNo).iBwdTimeOut.ToString();
            cbDirc.SelectedIndex = (int)SM.CYL.GetCylinderPara(iSelNo).eDirType;
            cbActrSync.IsChecked = SM.CYL.GetCylinderPara(iSelNo).bActrSync;
            tbActrSync.Text      = SM.CYL.GetCylinderPara(iSelNo).iActrSync .ToString();
            tbRptDelay.Text      = SM.CYL.GetCylRptPara  (iSelNo).iDelay.ToString();

            SetCntrlPn();
        }

        private void BtCylApply_Click(object sender, RoutedEventArgs e)
        {

            btCylApply.IsEnabled = false;
            Log.Trace(this.GetType().Name + " APPLY Button Clicked", ForContext.Frm);

            //string sXAdd = "";
            if (tbCylNo.Text != "")
            {
                int iSelNo = CConfig.StrToIntDef(tbCylNo.Text);

                CCylinder.TPara Para = SM.CYL.GetCylinderPara(iSelNo);
                Para.sEnum           = tbCylEnum.Text;
                Para.sName           = tbCylName.Text;
                //Para.sComment      = tbCylComt.Text;
                Para.iFwdXAdd        = CConfig.StrToIntDef(tbCylAddrIF.Text);
                Para.iBwdXAdd        = CConfig.StrToIntDef(tbCylAddrIB.Text);
                Para.iFwdYAdd        = CConfig.StrToIntDef(tbCylAddrOF.Text);
                Para.iBwdYAdd        = CConfig.StrToIntDef(tbCylAddrOB.Text);
                Para.iFwdOnDelay     = CConfig.StrToIntDef(tbCylOnDelayF.Text);
                Para.iBwdOnDelay     = CConfig.StrToIntDef(tbCylOnDelayB.Text);
                Para.iFwdTimeOut     = CConfig.StrToIntDef(tbCylTimeOutF.Text);
                Para.iBwdTimeOut     = CConfig.StrToIntDef(tbCylTimeOutB.Text);
                Para.eDirType        = (EN_MOVE_DIRECTION)cbDirc.SelectedIndex;
                Para.bActrSync       = cbActrSync.IsChecked == true;
                Para.iActrSync       = CConfig.StrToIntDef(tbActrSync.Text);
                SM.CYL.Repeat.iDelay = CConfig.StrToIntDef(tbRptDelay.Text);


                SM.CYL.SetCylinderPara(iSelNo, Para);

                CylinderData.New();
                SetCntrlPn();

            }
            btCylApply.IsEnabled = true;   
        }

        private void BtSaveCyl_Click(object sender, RoutedEventArgs e)
        {
            btSaveCyl.IsEnabled = false;
            Log.Trace(this.GetType().Name + " SAVE Button Clicked", ForContext.Frm);

            SM.CYL.LoadSave(false);
            btSaveCyl.IsEnabled = true;
        }

        private void BtActrRpt_Click(object sender, RoutedEventArgs e)
        {
            Log.Trace(this.GetType().Name + " REPEAT Button Clicked", ForContext.Frm);

            if (tbCylNo.Text != "")
            {
                int iSelNo = CConfig.StrToIntDef(tbCylNo.Text);
                //CCylinder.TPara Para = SM.CL.GetCylinderPara(iSelNo);

                if (cbActrSync.IsChecked == true) SM.CYL.GoRpt(CConfig.StrToIntDef(tbRptDelay.Text, 0), iSelNo, CConfig.StrToIntDef(tbActrSync.Text, -1));
                else                              SM.CYL.GoRpt(CConfig.StrToIntDef(tbRptDelay.Text, 0), iSelNo                                         );  
            }
        }

        private void BtActrStop_Click(object sender, RoutedEventArgs e)
        {
            Log.Trace(this.GetType().Name + " STOP Button Clicked", ForContext.Frm);

            SM.CYL.StopRpt();
        }

        private void BtActrReset_Click(object sender, RoutedEventArgs e)
        {
            Log.Trace(this.GetType().Name + " RESET Button Clicked", ForContext.Frm);

            if (tbCylNo.Text != "")
            {
                int iSelNo = CConfig.StrToIntDef(tbCylNo.Text);
                CCylinder.TPara Para = SM.CYL.GetCylinderPara(iSelNo);

                Log.Trace(Para.sName, "Clicked");
                SM.CYL.Reset();
            }
        }

        public void SetCntrlPn()
        {
            
            if (tbCylNo.Text != "")
            {
                int iSelNo = CConfig.StrToIntDef(tbCylNo.Text); //으흠???
                CCylinder.TPara Para = SM.CYL.GetCylinderPara(iSelNo);

                
                //Left  = Bwd버튼
                //Right = Fwd버튼
                //Down  = Bwd버튼
                //Up    = Fwd버튼
                //Bwd(왼쪽에 위치) 버튼 동적 생성
                Button     btBwd = new Button    (); //Button 생성
                StackPanel pnBwd = new StackPanel(); //Button Text, Icon 정렬할 StackPanel 생성
                TextBlock  tbBwd = new TextBlock (); //Button Text 생성
                PackIcon   piBwd = new PackIcon  (); //Button Icon 생성
                //텍스트박스 속성 셋팅
                tbBwd.VerticalAlignment = VerticalAlignment.Center;
                tbBwd.Margin            =  new Thickness(0, 0, 0, 3);
                tbBwd.FontSize          = 18;
                //PackIcon 속성 셋팅
                piBwd.VerticalAlignment = VerticalAlignment.Center;
                piBwd.Width             = 25;
                piBwd.Height            = 25;
                piBwd.Margin            = new Thickness(0);
                //StackPanel 속성 셋팅
                pnBwd.Width               = double.NaN;
                pnBwd.Height              = double.NaN;
                pnBwd.Orientation         = Orientation.Horizontal;
                pnBwd.HorizontalAlignment = HorizontalAlignment.Left;
                //Button 속성 셋팅
                btBwd.Background          = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0x8B, 0x8B));
                btBwd.Content             = pnBwd;
                btBwd.Width               = double.NaN;
                btBwd.Height              = double.NaN;
                btBwd.VerticalAlignment   = VerticalAlignment  .Stretch;
                btBwd.HorizontalAlignment = HorizontalAlignment.Stretch;
                //실린더 방향에 따라 달라져야하는 녀석들
                tbBwd.Text = "BWD";
                piBwd.Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowLeft;
                btBwd.Margin = new Thickness(2, 2, 1, 2);
                Grid.SetColumn    (btBwd, 0);
                Grid.SetRow       (btBwd, 1);
                Grid.SetColumnSpan(btBwd, 1);
                Grid.SetRowSpan   (btBwd, 2);
                //Grid에 버튼 추가
                gCylTest.Children.Add(btBwd);

                //Fwd(오른쪽에 위치) 버튼 동적 생성
                Button     btFwd = new Button    (); //Button 생성
                StackPanel pnFwd = new StackPanel(); //Button Text, Icon 정렬할 StackPanel 생성
                TextBlock  tbFwd = new TextBlock (); //Button Text 생성
                PackIcon   piFwd = new PackIcon  (); //Button Icon 생성
                //텍스트박스 속성 셋팅
                tbFwd.VerticalAlignment = VerticalAlignment.Center;
                tbFwd.Margin            =  new Thickness(0, 0, 0, 3);
                tbFwd.FontSize          = 18;
                //PackIcon 속성 셋팅
                piFwd.VerticalAlignment = VerticalAlignment.Center;
                piFwd.Width             = 25;
                piFwd.Height            = 25;
                piFwd.Margin            = new Thickness(0);
                //StackPanel 속성 셋팅
                pnFwd.Width               = double.NaN;
                pnFwd.Height              = double.NaN;
                pnFwd.Orientation         = Orientation.Horizontal;
                pnFwd.HorizontalAlignment = HorizontalAlignment.Left;
                //Button 속성 셋팅
                btFwd.Background          = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0x8B, 0x8B));
                btFwd.Content             = pnFwd;
                btFwd.Width               = double.NaN;
                btFwd.Height              = double.NaN;
                btFwd.VerticalAlignment   = VerticalAlignment  .Stretch;
                btFwd.HorizontalAlignment = HorizontalAlignment.Stretch;
                //실린더 방향에 따라 달라져야하는 녀석들
                tbFwd.Text = "FWD";
                piFwd.Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowRight;
                btFwd.Margin = new Thickness(1, 2, 2, 2);
                Grid.SetColumn    (btFwd, 1);
                Grid.SetRow       (btFwd, 1);
                Grid.SetColumnSpan(btFwd, 1);
                Grid.SetRowSpan   (btFwd, 2);
                
                //Grid에 버튼 추가
                gCylTest.Children.Add(btFwd);

                pnBwd.Children.Clear();
                pnFwd.Children.Clear();
                gCylTest.Children.Clear();
                
                //gCylTest 클리어 하는 것 때문에 그리드 위에 있는 TextBlock도 Clear 되어 마지막에 add 해줘야 실린더 이름이 보임.
                tbStCaption.Text = Para.sName;
                gCylTest.Children.Add(tbStCaption);

                btBwd.AddHandler(Button.ClickEvent, new RoutedEventHandler(btBwd_Click));
                btFwd.AddHandler(Button.ClickEvent, new RoutedEventHandler(btFwd_Click));

                switch (Para.eDirType) 
                { 
                    case EN_MOVE_DIRECTION.LR : tbBwd.Text   = "BWD";
                                                piBwd.Kind   = MaterialDesignThemes.Wpf.PackIconKind.ArrowLeft;
                                                btBwd.Margin = new Thickness(2, 2, 1, 2);
                                                pnBwd.Children.Add(piBwd);
                                                pnBwd.Children.Add(tbBwd);
                                                Grid.SetColumn    (btBwd, 0);
                                                Grid.SetRow       (btBwd, 1);
                                                Grid.SetColumnSpan(btBwd, 1);
                                                Grid.SetRowSpan   (btBwd, 2);
                                                
                                                tbFwd.Text   = "FWD";
                                                piFwd.Kind   = MaterialDesignThemes.Wpf.PackIconKind.ArrowRight;
                                                btFwd.Margin = new Thickness(1, 2, 2, 2);
                                                pnFwd.Children.Add(tbFwd); 
                                                pnFwd.Children.Add(piFwd); 
                                                Grid.SetColumn    (btFwd, 1);
                                                Grid.SetRow       (btFwd, 1);
                                                Grid.SetColumnSpan(btFwd, 1);
                                                Grid.SetRowSpan   (btFwd, 2);

                                                gCylTest.Children.Add(btBwd);
                                                gCylTest.Children.Add(btFwd);
                                                
                                                break ;
                
                    case EN_MOVE_DIRECTION.RL : tbBwd.Text   = "BWD";
                                                piBwd.Kind   = MaterialDesignThemes.Wpf.PackIconKind.ArrowRight;
                                                btBwd.Margin = new Thickness(2, 2, 1, 2);
                                                pnBwd.Children.Add(tbBwd);
                                                pnBwd.Children.Add(piBwd); 
                                                Grid.SetColumn    (btBwd, 0);
                                                Grid.SetRow       (btBwd, 1);
                                                Grid.SetColumnSpan(btBwd, 1);
                                                Grid.SetRowSpan   (btBwd, 2);
                                                
                                                tbFwd.Text   = "FWD";
                                                piFwd.Kind   = MaterialDesignThemes.Wpf.PackIconKind.ArrowLeft;
                                                btFwd.Margin = new Thickness(1, 2, 2, 2);
                                                pnFwd.Children.Add(piFwd);
                                                pnFwd.Children.Add(tbFwd);
                                                Grid.SetColumn    (btFwd, 1);
                                                Grid.SetRow       (btFwd, 1);
                                                Grid.SetColumnSpan(btFwd, 1);
                                                Grid.SetRowSpan   (btFwd, 2);

                                                gCylTest.Children.Add(btBwd);
                                                gCylTest.Children.Add(btFwd);
                                                  
                                                break ;
                
                    case EN_MOVE_DIRECTION.BF : tbBwd.Text   = "BWD";
                                                piBwd.Kind   = MaterialDesignThemes.Wpf.PackIconKind.ArrowDown;
                                                btBwd.Margin = new Thickness(2, 1, 2, 2);
                                                pnBwd.Children.Add(piBwd);
                                                pnBwd.Children.Add(tbBwd);
                                                Grid.SetColumn    (btBwd, 0);
                                                Grid.SetRow       (btBwd, 2);
                                                Grid.SetColumnSpan(btBwd, 2);
                                                Grid.SetRowSpan   (btBwd, 1);
                                                
                                                tbFwd.Text   = "FWD";
                                                piFwd.Kind   = MaterialDesignThemes.Wpf.PackIconKind.ArrowUp;
                                                btFwd.Margin = new Thickness(2, 2, 2, 1);
                                                pnFwd.Children.Add(piFwd);
                                                pnFwd.Children.Add(tbFwd);
                                                Grid.SetColumn    (btFwd, 0);
                                                Grid.SetRow       (btFwd, 1);
                                                Grid.SetColumnSpan(btFwd, 2);
                                                Grid.SetRowSpan   (btFwd, 1);

                                                gCylTest.Children.Add(btBwd);
                                                gCylTest.Children.Add(btFwd);
                                                  
                                                break ;
                    
                    case EN_MOVE_DIRECTION.FB : tbBwd.Text   = "FWD";
                                                piBwd.Kind   = MaterialDesignThemes.Wpf.PackIconKind.ArrowUp;
                                                btBwd.Margin = new Thickness(2, 1, 2, 2);
                                                pnBwd.Children.Add(piBwd);
                                                pnBwd.Children.Add(tbBwd);
                                                Grid.SetColumn    (btBwd, 0);
                                                Grid.SetRow       (btBwd, 2);
                                                Grid.SetColumnSpan(btBwd, 2);
                                                Grid.SetRowSpan   (btBwd, 1);
                                                
                                                tbFwd.Text   = "BWD";
                                                piFwd.Kind   = MaterialDesignThemes.Wpf.PackIconKind.ArrowDown;
                                                btFwd.Margin = new Thickness(2, 2, 2, 1);
                                                pnFwd.Children.Add(piFwd);
                                                pnFwd.Children.Add(tbFwd);
                                                Grid.SetColumn    (btFwd, 0);
                                                Grid.SetRow       (btFwd, 1);
                                                Grid.SetColumnSpan(btFwd, 2);
                                                Grid.SetRowSpan   (btFwd, 1);

                                                gCylTest.Children.Add(btBwd);
                                                gCylTest.Children.Add(btFwd);
                                                
                                                break ;
                    
                    case EN_MOVE_DIRECTION.UD : tbBwd.Text   = "UP";
                                                piBwd.Kind   = MaterialDesignThemes.Wpf.PackIconKind.ArrowUp;
                                                btBwd.Margin = new Thickness(2, 1, 2, 2);
                                                pnBwd.Children.Add(piBwd);
                                                pnBwd.Children.Add(tbBwd);
                                                Grid.SetColumn    (btBwd, 0);
                                                Grid.SetRow       (btBwd, 2);
                                                Grid.SetColumnSpan(btBwd, 2);
                                                Grid.SetRowSpan   (btBwd, 1);
                                                
                                                tbFwd.Text   = "DOWN";
                                                piFwd.Kind   = MaterialDesignThemes.Wpf.PackIconKind.ArrowDown;
                                                btFwd.Margin = new Thickness(2, 2, 2, 1);
                                                pnFwd.Children.Add(piFwd);
                                                pnFwd.Children.Add(tbFwd);
                                                Grid.SetColumn    (btFwd, 0);
                                                Grid.SetRow       (btFwd, 1);
                                                Grid.SetColumnSpan(btFwd, 2);
                                                Grid.SetRowSpan   (btFwd, 1);

                                                gCylTest.Children.Add(btBwd);
                                                gCylTest.Children.Add(btFwd);

                                                break ;
                    
                    case EN_MOVE_DIRECTION.DU : tbBwd.Text   = "DOWN";
                                                piBwd.Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowDown;
                                                btBwd.Margin = new Thickness(2, 1, 2, 2);
                                                pnBwd.Children.Add(piBwd);
                                                pnBwd.Children.Add(tbBwd);
                                                Grid.SetColumn    (btBwd, 0);
                                                Grid.SetRow       (btBwd, 2);
                                                Grid.SetColumnSpan(btBwd, 2);
                                                Grid.SetRowSpan   (btBwd, 1);                        
                                                
                                                tbFwd.Text   = "UP";
                                                piFwd.Kind   = MaterialDesignThemes.Wpf.PackIconKind.ArrowUp;
                                                btFwd.Margin = new Thickness(2, 2, 2, 1);
                                                pnFwd.Children.Add(piFwd);
                                                pnFwd.Children.Add(tbFwd);
                                                Grid.SetColumn    (btFwd, 0);
                                                Grid.SetRow       (btFwd, 1);
                                                Grid.SetColumnSpan(btFwd, 2);
                                                Grid.SetRowSpan   (btFwd, 1);

                                                gCylTest.Children.Add(btBwd);
                                                gCylTest.Children.Add(btFwd);
                         
                                                break ;
                    
                    case EN_MOVE_DIRECTION.CA : tbBwd.Text   = "CW";
                                                piBwd.Kind   = MaterialDesignThemes.Wpf.PackIconKind.Clockwise;
                                                btBwd.Margin = new Thickness(2, 2, 1, 2);
                                                pnBwd.Children.Add(piBwd);
                                                pnBwd.Children.Add(tbBwd);
                                                Grid.SetColumn    (btBwd, 0);
                                                Grid.SetRow       (btBwd, 1);
                                                Grid.SetColumnSpan(btBwd, 1);
                                                Grid.SetRowSpan   (btBwd, 2);
                                                
                                                tbFwd.Text   = "CCW";
                                                piFwd.Kind   = MaterialDesignThemes.Wpf.PackIconKind.RestoreClock;
                                                btFwd.Margin = new Thickness(1, 2, 2, 2);
                                                pnFwd.Children.Add(tbFwd); 
                                                pnFwd.Children.Add(piFwd); 
                                                Grid.SetColumn    (btFwd, 1);
                                                Grid.SetRow       (btFwd, 1);
                                                Grid.SetColumnSpan(btFwd, 1);
                                                Grid.SetRowSpan   (btFwd, 2);

                                                gCylTest.Children.Add(btBwd);
                                                gCylTest.Children.Add(btFwd);

                                                break ;
                    
                    case EN_MOVE_DIRECTION.AC : tbBwd.Text   = "CCW";
                                                piBwd.Kind   = MaterialDesignThemes.Wpf.PackIconKind.RestoreClock;
                                                btBwd.Margin = new Thickness(2, 2, 1, 2);
                                                pnBwd.Children.Add(tbBwd);
                                                pnBwd.Children.Add(piBwd); 
                                                Grid.SetColumn    (btBwd, 0);
                                                Grid.SetRow       (btBwd, 1);
                                                Grid.SetColumnSpan(btBwd, 1);
                                                Grid.SetRowSpan   (btBwd, 2);
                                                
                                                tbFwd.Text   = "CW";
                                                piFwd.Kind   = MaterialDesignThemes.Wpf.PackIconKind.Clockwise;
                                                btFwd.Margin = new Thickness(1, 2, 2, 2);
                                                pnFwd.Children.Add(piFwd);
                                                pnFwd.Children.Add(tbFwd);
                                                Grid.SetColumn    (btFwd, 1);
                                                Grid.SetRow       (btFwd, 1);
                                                Grid.SetColumnSpan(btFwd, 1);
                                                Grid.SetRowSpan   (btFwd, 2);

                                                gCylTest.Children.Add(btBwd);
                                                gCylTest.Children.Add(btFwd);
                         
                                                break ;
                    
                    case EN_MOVE_DIRECTION.CO : tbBwd.Text   = "CLOSE";
                                                piBwd.Kind   = MaterialDesignThemes.Wpf.PackIconKind.ArrowCollapse;
                                                btBwd.Margin = new Thickness(2, 1, 2, 2);
                                                pnBwd.Children.Add(piBwd);
                                                pnBwd.Children.Add(tbBwd);
                                                Grid.SetColumn    (btBwd, 0);
                                                Grid.SetRow       (btBwd, 2);
                                                Grid.SetColumnSpan(btBwd, 2);
                                                Grid.SetRowSpan   (btBwd, 1);
                                                
                                                tbFwd.Text   = "OPEN";
                                                piFwd.Kind   = MaterialDesignThemes.Wpf.PackIconKind.ArrowExpand;
                                                btFwd.Margin = new Thickness(2, 2, 2, 1);
                                                pnFwd.Children.Add(piFwd);
                                                pnFwd.Children.Add(tbFwd);
                                                Grid.SetColumn    (btFwd, 0);
                                                Grid.SetRow       (btFwd, 1);
                                                Grid.SetColumnSpan(btFwd, 2);
                                                Grid.SetRowSpan   (btFwd, 1);

                                                gCylTest.Children.Add(btBwd);
                                                gCylTest.Children.Add(btFwd);
                                                  
                                                break ;
                    
                    case EN_MOVE_DIRECTION.OC : tbBwd.Text   = "OPEN";
                                                piBwd.Kind   = MaterialDesignThemes.Wpf.PackIconKind.ArrowExpand;
                                                btBwd.Margin = new Thickness(2, 1, 2, 2);
                                                pnBwd.Children.Add(piBwd);
                                                pnBwd.Children.Add(tbBwd);
                                                Grid.SetColumn    (btBwd, 0);
                                                Grid.SetRow       (btBwd, 2);
                                                Grid.SetColumnSpan(btBwd, 2);
                                                Grid.SetRowSpan   (btBwd, 1);
                                                
                                                tbFwd.Text   = "CLOSE";
                                                piFwd.Kind   = MaterialDesignThemes.Wpf.PackIconKind.ArrowCollapse;
                                                btFwd.Margin = new Thickness(2, 2, 2, 1);
                                                pnFwd.Children.Add(piFwd);
                                                pnFwd.Children.Add(tbFwd);
                                                Grid.SetColumn    (btFwd, 0);
                                                Grid.SetRow       (btFwd, 1);
                                                Grid.SetColumnSpan(btFwd, 2);
                                                Grid.SetRowSpan   (btFwd, 1);

                                                gCylTest.Children.Add(btBwd);
                                                gCylTest.Children.Add(btFwd);

                                                break ;
                }
                btBwd.BorderThickness = new Thickness(0);
                btFwd.BorderThickness = new Thickness(0);
                //m_bSetCntrlPn = true ;

            }
            
        }

        public void btBwd_Click(object sender, RoutedEventArgs e)
        {
            if (tbCylNo.Text != "")
            {
                int iSelNo = CConfig.StrToIntDef(tbCylNo.Text);
                CCylinder.TPara Para = SM.CYL.GetCylinderPara(iSelNo);

                Log.Trace(Para.sName, "Clicked");
                SM.CYL.Move(iSelNo, EN_CYL_POS.Bwd);
            }
        }

        public void btFwd_Click(object sender, RoutedEventArgs e)
        {
            if (tbCylNo.Text != "")
            {
                int iSelNo = CConfig.StrToIntDef(tbCylNo.Text);
                CCylinder.TPara Para = SM.CYL.GetCylinderPara(iSelNo);

                Log.Trace(Para.sName, "Clicked");
                SM.CYL.Move(iSelNo, EN_CYL_POS.Fwd);
            }
        }



        #endregion
        //---------------------------------------------------------------------------------------------
        //Cylinder
        //---------------------------------------------------------------------------------------------

        //---------------------------------------------------------------------------------------------
        //Motor
        //---------------------------------------------------------------------------------------------
        #region Motor
        private EN_LEVEL m_iPreCrntLevel = EN_LEVEL.Operator;

        private void DispMotr()
        {
            if (SM.MTR._iMaxMotr < 1) return;

            int iMotrSel = cbMotrSel.SelectedIndex;

            pgMotrPara   .SelectedObject = SM.MTR.Para[iMotrSel];
            pgMotrParaSub.SelectedObject = SM.MTR.Mtr [iMotrSel].GetPara();

            if (SM.FrmLogOn.GetLevel() >= EN_LEVEL.Master)
            {
                pgMotrPara   .IsEnabled = true;
                pgMotrParaSub.IsEnabled = true;
            }
            else
            {
                pgMotrPara   .IsEnabled = false;
                pgMotrParaSub.IsEnabled = false;
            }

            m_iPreCrntLevel = SM.FrmLogOn.GetLevel();
        }
        private void DispJogIcon()
        {
            int iMotrSel = cbMotrSel.SelectedIndex;
            pnJogN.Children.Clear();
            pnJogP.Children.Clear();
            switch (SM.MTR.Para[iMotrSel].iDirType) //왼쪽에 있는게 -, 오른쪽에 있는게 +
                { 
                    case MOTION_DIR.BwdFwd : tbJogN.Text   = "BWD";
                                             piJogN.Kind   = MaterialDesignThemes.Wpf.PackIconKind.ArrowDown;
                                             pnJogN.Children.Add(piJogN);
                                             pnJogN.Children.Add(tbJogN);
                                             
                                             tbJogP.Text   = "FWD";
                                             piJogP.Kind   = MaterialDesignThemes.Wpf.PackIconKind.ArrowUp;
                                             pnJogP.Children.Add(tbJogP); 
                                             pnJogP.Children.Add(piJogP); 
                                             
                                             break ;
                
                    case MOTION_DIR.FwdBwd : tbJogN.Text   = "FWD";
                                             piJogN.Kind   = MaterialDesignThemes.Wpf.PackIconKind.ArrowUp;
                                             pnJogN.Children.Add(piJogN);
                                             pnJogN.Children.Add(tbJogN); 
                                             
                                             tbJogP.Text   = "BWD";
                                             piJogP.Kind   = MaterialDesignThemes.Wpf.PackIconKind.ArrowDown;
                                             pnJogP.Children.Add(tbJogP);
                                             pnJogP.Children.Add(piJogP);
                                            
                                             break ;
                
                    case MOTION_DIR.CcwCw : tbJogN.Text   = "CCW";
                                            piJogN.Kind   = MaterialDesignThemes.Wpf.PackIconKind.RestoreClock;
                                            pnJogN.Children.Add(piJogN);
                                            pnJogN.Children.Add(tbJogN);
                                            
                                            tbJogP.Text   = "CW";
                                            piJogP.Kind   = MaterialDesignThemes.Wpf.PackIconKind.Clockwise;
                                            pnJogP.Children.Add(tbJogP);
                                            pnJogP.Children.Add(piJogP);
                                           
                                            break ;
                    
                    case MOTION_DIR.CwCcw : tbJogN.Text   = "CW";
                                            piJogN.Kind   = MaterialDesignThemes.Wpf.PackIconKind.Clockwise;
                                            pnJogN.Children.Add(piJogN);
                                            pnJogN.Children.Add(tbJogN);
                                            
                                            tbJogP.Text   = "CCW";
                                            piJogP.Kind   = MaterialDesignThemes.Wpf.PackIconKind.RestoreClock;
                                            pnJogP.Children.Add(tbJogP);
                                            pnJogP.Children.Add(piJogP);
                                           
                                            break ;
                    
                    case MOTION_DIR.DownUp : tbJogN.Text   = "DOWN";
                                             piJogN.Kind   = MaterialDesignThemes.Wpf.PackIconKind.ArrowDown;
                                             pnJogN.Children.Add(piJogN);
                                             pnJogN.Children.Add(tbJogN);
                                             
                                             tbJogP.Text   = "UP";
                                             piJogP.Kind   = MaterialDesignThemes.Wpf.PackIconKind.ArrowUp;
                                             pnJogP.Children.Add(tbJogP);
                                             pnJogP.Children.Add(piJogP);
                                            
                                             break ;
                    
                    case MOTION_DIR.UpDown : tbJogN.Text   = "UP";
                                             piJogN.Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowUp;
                                             pnJogN.Children.Add(piJogN);
                                             pnJogN.Children.Add(tbJogN);
                                             
                                             tbJogP.Text   = "DOWN";
                                             piJogP.Kind   = MaterialDesignThemes.Wpf.PackIconKind.ArrowDown;
                                             pnJogP.Children.Add(tbJogP);
                                             pnJogP.Children.Add(piJogP);
                                             
                                             break ;
                    
                    case MOTION_DIR.LeftRight : tbJogN.Text   = "LEFT";
                                                piJogN.Kind   = MaterialDesignThemes.Wpf.PackIconKind.ArrowLeft;
                                                pnJogN.Children.Add(piJogN);
                                                pnJogN.Children.Add(tbJogN);
                                                
                                                tbJogP.Text   = "RIGHT";
                                                piJogP.Kind   = MaterialDesignThemes.Wpf.PackIconKind.ArrowRight;
                                                pnJogP.Children.Add(tbJogP); 
                                                pnJogP.Children.Add(piJogP); 
                                               
                                                break ;
                    
                    case MOTION_DIR.RightLeft : tbJogN.Text   = "RIGHT";
                                                piJogN.Kind   = MaterialDesignThemes.Wpf.PackIconKind.ArrowRight;
                                                pnJogN.Children.Add(piJogN);
                                                pnJogN.Children.Add(tbJogN); 
                                                
                                                tbJogP.Text   = "LEFT";
                                                piJogP.Kind   = MaterialDesignThemes.Wpf.PackIconKind.ArrowLeft;
                                                pnJogP.Children.Add(tbJogP);
                                                pnJogP.Children.Add(piJogP);
                                               
                                                break ;
                    
                    
                }
        }

        private void CbMotrSel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DispMotr();
            DispJogIcon();
        }

        private void BtSaveMotr_Click(object sender, RoutedEventArgs e)
        {
            btSaveMotr.IsEnabled = false;
            Log.Trace(this.GetType().Name + " " + "SAVE Button Clicked", ForContext.Frm);

            SM.MTR.ApplyParaAll();
            SM.MTR.LoadSaveAll(false);
            DispJogIcon();
            
            MotorData.Update();
            //MotorData.New();

            btSaveMotr.IsEnabled = true;
        }
        
        private void BtJogN_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            string sText = tbJogN.Text;
            Log.Trace(sFormText + sText + " Mouse Down", ForContext.Frm);

            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MTR.JogN(iMotrSel);
        }

        private void BtJogN_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            string sText = tbJogN.Text;
            Log.Trace(sFormText + sText + " Mouse Up", ForContext.Frm);

            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MTR.Stop(iMotrSel);
        }

        private void BtJogP_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            string sText = tbJogP.Text;
            Log.Trace(sFormText + sText + " Mouse Down", ForContext.Frm);

            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MTR.JogP(iMotrSel);
        }

        private void BtJogP_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            string sText = tbJogP.Text;
            Log.Trace(sFormText + sText + " Mouse Up", ForContext.Frm);

            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MTR.Stop(iMotrSel);
        }

        private void BtStop_Click(object sender, RoutedEventArgs e)
        {
            Log.Trace(this.GetType().Name + " MT STOP Button Clicked", ForContext.Frm);

            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MTR.Stop(iMotrSel);
        }

        private void BtReset_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Log.Trace(sFormText + " MT RESET Mouse Down", ForContext.Frm);

            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MTR.SetReset(iMotrSel, true);
        }

        private void BtReset_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Log.Trace(sFormText + " MT RESET Mouse Up", ForContext.Frm);

            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MTR.SetReset(iMotrSel, false);
        }

        private void BtClearPos_Click(object sender, RoutedEventArgs e)
        {
            Log.Trace(this.GetType().Name + " CLEAR POS Button Clicked", ForContext.Frm);

            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MTR.SetPos(iMotrSel, 0.0);
        }

        private void BtHome_Click(object sender, RoutedEventArgs e)
        {
            Log.Trace(this.GetType().Name + " HOME Button Clicked", ForContext.Frm);

            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MTR.GoHome(iMotrSel);
        }

        private void BtServoOn_Click(object sender, RoutedEventArgs e)
        {
            Log.Trace(this.GetType().Name + " SERVO ON Button Clicked", ForContext.Frm);

            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MTR.SetServo(iMotrSel, true);
        }

        private void BtServoOff_Click(object sender, RoutedEventArgs e)
        {
            Log.Trace(this.GetType().Name + " SERVO OFF Button Clicked", ForContext.Frm);

            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MTR.SetServo(iMotrSel, false);
        }

        private void BtAllStop_Click(object sender, RoutedEventArgs e)
        {
            Log.Trace(this.GetType().Name + " ALL STOP Button Clicked", ForContext.Frm);

            for (int i = 0; i < SM.MTR._iMaxMotr; i++)
                SM.MTR.Stop(i);
        }

        private void BtServoOnAll_Click(object sender, RoutedEventArgs e)
        {
            Log.Trace(this.GetType().Name + " SERVO ON ALL Button Clicked", ForContext.Frm);

            for (int i = 0; i < SM.MTR._iMaxMotr; i++)
                SM.MTR.SetServo(i, true);
        }

        private void BtServoOffAll_Click(object sender, RoutedEventArgs e)
        {
            Log.Trace(this.GetType().Name + " SERVO OFF ALL Button Clicked", ForContext.Frm);

            for (int i = 0; i < SM.MTR._iMaxMotr; i++)
                SM.MTR.SetServo(i, false);
        }

        private void BtGo1stPos_Click(object sender, RoutedEventArgs e)
        {
            Log.Trace(this.GetType().Name + " GO 1st POS Button Clicked", ForContext.Frm);

            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MTR.GoAbsRepeatFst(iMotrSel);
        }

        private void BtGo2ndPos_Click(object sender, RoutedEventArgs e)
        {
            Log.Trace(this.GetType().Name + " GO 2nd POS Button Clicked", ForContext.Frm);

            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MTR.GoAbsRepeatScd(iMotrSel);
        }

        private void BtRepeat_Click(object sender, RoutedEventArgs e)
        {
            Log.Trace(this.GetType().Name + " REPEAT Button Clicked", ForContext.Frm);

            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MTR.StartRepeat(iMotrSel);
        }

        private void BtRpStop_Click(object sender, RoutedEventArgs e)
        {
            Log.Trace(this.GetType().Name + " REPEAT STOP Button Clicked", ForContext.Frm);

            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MTR.Stop(iMotrSel);
        }

        private void LbHomeDone_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int iMotrSel = cbMotrSel.SelectedIndex;
            SM.MTR.SetHomeDone(iMotrSel, !SM.MTR.GetHomeDone(iMotrSel));
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
        }


        #endregion
        //---------------------------------------------------------------------------------------------
        //Motor
        //---------------------------------------------------------------------------------------------
    }

    //---------------------------------------------------------------------------------------------
    //Data ListVIew ItemsSource ObservableCollection
    //---------------------------------------------------------------------------------------------

    #region Errordata
    //---------------------------------------------------------------------------------------------
    //Errordata
    //---------------------------------------------------------------------------------------------
    public class Errordata
    {
        public string ErrorNo     { get; set; }
        public string ErrorEnum   { get; set; }
        public string ErrorName   { get; set; }
        public string ErrorAction { get; set; }
 
        //private static List<Listdata> instance;
        private static ObservableCollection<Errordata> instance;
 
        //public static List<Listdata> GetInstance()
        public static ObservableCollection<Errordata> GetInstance()
        {
            //if (instance == null)
            //    instance = new List<Listdata>();
            if (instance == null) {
                instance = new ObservableCollection<Errordata>();
                New(); 
            }

            return instance;
        }

        public static void New()
        {
            instance.Clear();
            for (int i = 0; i < SM.ERR._iMaxErrCnt; i++)
                instance.Add(new Errordata() { ErrorNo = i.ToString(), ErrorEnum = SM.ERR.GetErrConfig(i).sEnum , ErrorName = SM.ERR.GetErrConfig(i).sName , ErrorAction = SM.ERR.GetErrConfig(i).sAction});                                           
                                                           
        }
    }
    #endregion
    //---------------------------------------------------------------------------------------------
    //IOdata 
    //---------------------------------------------------------------------------------------------
    #region IOdata
    public class IOdata : INotifyPropertyChanged
    {
        private string aVal           ; //타이머에서 갱신중이라 속성변경 알림 구현
        private Brush  aValBackground ; //타이머에서 갱신중이라 속성변경 알림 구현
        private string vVal           ; //타이머에서 갱신중이라 속성변경 알림 구현
        private Brush  vValBackground ; //타이머에서 갱신중이라 속성변경 알림 구현

        public string No             { get; set; }
        public string Hex            { get; set; }
        public string Name           { get; set; }
        public string Comment        { get; set; }
        public string Inv            { get; set; }
        public string Log            { get; set; }
        public string Add            { get; set; }
        public string OnDelay        { get; set; }
        public string OffDelay       { get; set; }
        public string AVal           { get{ return aVal           ;} set{ aVal           = value; OnPropertyChanged("AVal");          } }
        public Brush  AValBackground { get{ return aValBackground ;} set{ aValBackground = value; OnPropertyChanged("AValBackground");} }
        public string VVal           { get{ return vVal           ;} set{ vVal           = value; OnPropertyChanged("VVal");          } }
        public Brush  VValBackground { get{ return vValBackground ;} set{ vValBackground = value; OnPropertyChanged("VValBackground");} }

        private static ObservableCollection<IOdata> Input ;
        private static ObservableCollection<IOdata> Output;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            if(PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public static ObservableCollection<IOdata> GetInput()
        {
            if (Input  == null) { 
                Input = new ObservableCollection<IOdata>();  
                NewInput();  }
            return Input;
        }
        public static ObservableCollection<IOdata> GetOutput()
        {
            if (Output == null) { 
                Output = new ObservableCollection<IOdata>(); 
                NewOutput(); }
            return Output;
        }

        public static void NewInput()
        {
            Input.Clear();
            for (int i = 0; i < SM.DIO._iMaxIn; i++)
                Input.Add(new IOdata() { 
                    No       = i.ToString(), 
                    Hex      = string.Format("X{0:X3}" , SM.DIO.GetInputPara(i).iAdd),//SM.DIO.GetInputPara(i).sHex, 
                    Name     = SM.DIO.GetInputPara(i).sName, 
                    //Comment  = SM.DIO.GetInputPara(i).SComt, 
                    Inv      = SM.DIO.GetInputPara(i).bInv    ? "O" : "X", 
                    Log      = SM.DIO.GetInputPara(i).bNotLog ? "X" : "O", 
                    Add      = string.Format("X{0:000}", SM.DIO.GetInputPara(i).iAdd      ), 
                    OnDelay  = string.Format("{0:0}"   , SM.DIO.GetInputPara(i).iOnDelay  ),
                    OffDelay = string.Format("{0:0}"   , SM.DIO.GetInputPara(i).iOffDelay )});
                                                           
        }

        public static void NewOutput()
        {
            Output.Clear();
            for (int i = 0; i < SM.DIO._iMaxOut; i++)
                Output.Add(new IOdata() { 
                    No       = i.ToString(), 
                    Hex      = string.Format("X{0:X3}" , SM.DIO.GetOutputPara(i).iAdd),//SM.DIO.GetInputPara(i).sHex, 
                    Name     = SM.DIO.GetOutputPara(i).sName, 
                    Comment  = SM.DIO.GetOutputPara(i).SComt, 
                    Inv      = SM.DIO.GetOutputPara(i).bInv    ? "O" : "X", 
                    Log      = SM.DIO.GetOutputPara(i).bNotLog ? "X" : "O", 
                    Add      = string.Format("X{0:000}", SM.DIO.GetOutputPara(i).iAdd      ), 
                    OnDelay  = string.Format("{0:0}"   , SM.DIO.GetOutputPara(i).iOnDelay  ),
                    OffDelay = string.Format("{0:0}"   , SM.DIO.GetOutputPara(i).iOffDelay )});
                                                           
        }

        //클리어 하는게 좀 꺼림직해서 밑에껄로 할까 하다 괜찮은거 같아서 그냥 함
        //public void ReOutput()
        //{
        //    for (int i = 0; i < SM.DIO._iMaxOut; i++)
        //    { 
        //        Output[i].No       = i.ToString(); 
        //        Output[i].Hex      = string.Format("X{0:X3}" , SM.DIO.GetOutputPara(i).iAdd);//SM.DIO.GetInputPara(i).sHex, 
        //        Output[i].Name     = SM.DIO.GetOutputPara(i).sName; 
        //        Output[i].Comment  = SM.DIO.GetOutputPara(i).SComt; 
        //        Output[i].Inv      = SM.DIO.GetOutputPara(i).bInv    ? "O" : "X"; 
        //        Output[i].Log      = SM.DIO.GetOutputPara(i).bNotLog ? "X" : "O"; 
        //        Output[i].Add      = string.Format("X{0:000}", SM.DIO.GetOutputPara(i).iAdd      ); 
        //        Output[i].OnDelay  = string.Format("{0:0}"   , SM.DIO.GetOutputPara(i).iOnDelay  );
        //        Output[i].OffDelay = string.Format("{0:0}"   , SM.DIO.GetOutputPara(i).iOffDelay );
        //    }                                             
        //}
    }
    #endregion
    //---------------------------------------------------------------------------------------------
    //AIOdata 
    //---------------------------------------------------------------------------------------------
    #region AIOdata
    public class AIOdata : INotifyPropertyChanged
    {
        private string aVal           ; //타이머에서 갱신중이라 속성변경 알림 구현
        private string dVal           ; //타이머에서 갱신중이라 속성변경 알림 구현
        private string val            ; //타이머에서 갱신중이라 속성변경 알림 구현

        public string No             { get; set; }
        public string Hex            { get; set; }
        public string Name           { get; set; }
        public string Comment        { get; set; }
        public string Add            { get; set; }
        public string AVal           { get{ return aVal           ;} set{ aVal           = value; OnPropertyChanged("AVal");          } } //AI Analog
        public string DVal           { get{ return dVal           ;} set{ dVal           = value; OnPropertyChanged("DVal");          } } //AI Digit
        public string Val            { get{ return val            ;} set{ val            = value; OnPropertyChanged("Val");           } } //AO GetY

        private static ObservableCollection<AIOdata> Input ;
        private static ObservableCollection<AIOdata> Output;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            if(PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public static ObservableCollection<AIOdata> GetInput()
        {
            if (Input  == null) { Input = new ObservableCollection<AIOdata>(); NewInput(); }
            return Input;
        }
        public static ObservableCollection<AIOdata> GetOutput()
        {
            if (Output == null) { Output = new ObservableCollection<AIOdata>(); NewOutput(); }
            return Output;
        }

        public static void NewInput()
        {
            Input.Clear();
            for (int i = 0; i < SM.AIO._iMaxIn; i++)
                Input.Add(new AIOdata() { 
                    No       = i.ToString(), 
                    Hex      = string.Format("X{0:X3}" , SM.AIO.GetInputPara(i).iAdd),//SM.DIO.GetInputPara(i).sHex, 
                    Name     = SM.AIO.GetInputPara(i).sName, 
                    //Comment  = SM.AIO.GetInputPara(i).SComt, 
                    Add      = string.Format("X{0:000}", SM.AIO.GetInputPara(i).iAdd      )});
            
        }

        public static void NewOutput()
        {
            Output.Clear();
            for (int i = 0; i < SM.AIO._iMaxOut; i++)
                Output.Add(new AIOdata() { 
                    No       = i.ToString(), 
                    Hex      = string.Format("X{0:X3}" , SM.AIO.GetOutputPara(i).iAdd),//SM.DIO.GetInputPara(i).sHex, 
                    Name     = SM.AIO.GetOutputPara(i).sName, 
                    //Comment  = SM.AIO.GetOutputPara(i).SComt, 
                    Add      = string.Format("X{0:000}", SM.AIO.GetOutputPara(i).iAdd      )});
        }

    }
    #endregion

    //---------------------------------------------------------------------------------------------
    //TowerLampData 
    //---------------------------------------------------------------------------------------------
    #region TowerLampData
    public class TowerLampData 
    {
        public string STATUS        { get; set; }
        public string RED           { get; set; }
        public string YELLOW        { get; set; }
        public string GREEN         { get; set; }
        public string SOUND         { get; set; }

        private static ObservableCollection<TowerLampData> instance ;

        public static ObservableCollection<TowerLampData> Getinstance()
        {
            if (instance  == null) { instance = new ObservableCollection<TowerLampData>(); New(); }
            return instance;
        }

        public static void New()
        {
            instance.Clear();
            for (int i = 0; i < (int)EN_SEQ_STAT.MAX_SEQ_STAT; i++)
            {
                instance.Add(new TowerLampData() { 
                    STATUS = ((EN_SEQ_STAT)i).ToString(), 
                    RED    = SM.TWL.GetTLampInfo((EN_SEQ_STAT)i).iRed .ToString(),
                    YELLOW = SM.TWL.GetTLampInfo((EN_SEQ_STAT)i).iYel .ToString(),
                    GREEN  = SM.TWL.GetTLampInfo((EN_SEQ_STAT)i).iGrn .ToString(),
                    SOUND  = SM.TWL.GetTLampInfo((EN_SEQ_STAT)i).iBuzz.ToString() });

            }
            
        }

    }
    #endregion

    //---------------------------------------------------------------------------------------------
    //CylinderData 
    //---------------------------------------------------------------------------------------------
    #region CylinderData
    public class CylinderData 
    {
        public string NO            { get; set; }
        public string NAME          { get; set; }
        public string FwdXAdd       { get; set; }
        public string BwdXAdd       { get; set; }
        public string FwdYAdd       { get; set; }
        public string BwdYAdd       { get; set; }
        public string FwdOnDelay    { get; set; }
        public string BwdOnDelay    { get; set; }
        public string FwdTimeOut    { get; set; }
        public string BwdTimeOut    { get; set; }

        private static ObservableCollection<CylinderData> instance ;

        public static ObservableCollection<CylinderData> Getinstance()
        {
            if (instance  == null) { instance = new ObservableCollection<CylinderData>(); New(); }
            return instance;
        }

        public static void New()
        {
            instance.Clear();
            for (int i = 0; i < SM.CYL._iMaxCylinder; i++)
            {
                instance.Add(new CylinderData() { 
                    NO         = i.ToString(), 
                    NAME       = SM.CYL.GetCylinderPara(i).sName,
                    FwdXAdd    = string.Format("{0:0}", SM.CYL.GetCylinderPara(i).iFwdXAdd   ),
                    BwdXAdd    = string.Format("{0:0}", SM.CYL.GetCylinderPara(i).iBwdXAdd   ),
                    FwdYAdd    = string.Format("{0:0}", SM.CYL.GetCylinderPara(i).iFwdYAdd   ),
                    BwdYAdd    = string.Format("{0:0}", SM.CYL.GetCylinderPara(i).iBwdYAdd   ),
                    FwdOnDelay = string.Format("{0:0}", SM.CYL.GetCylinderPara(i).iFwdOnDelay),
                    BwdOnDelay = string.Format("{0:0}", SM.CYL.GetCylinderPara(i).iBwdOnDelay),
                    FwdTimeOut = string.Format("{0:0}", SM.CYL.GetCylinderPara(i).iFwdTimeOut),
                    BwdTimeOut = string.Format("{0:0}", SM.CYL.GetCylinderPara(i).iBwdTimeOut) });
            }
            
        }

    }
    #endregion

    //---------------------------------------------------------------------------------------------
    //MotorData 
    //---------------------------------------------------------------------------------------------
    #region MotorData
    public class MotorData : INotifyPropertyChanged 
    {
        //public string Items            { get; set; }
        //public string NAME          { get; set; }
        private string items;
        public string  Items            { get{ return items            ;} set{ items            = value; OnPropertyChanged("Items");           } }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            if(PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        private static ObservableCollection<MotorData> instance ;

        public static ObservableCollection<MotorData> Getinstance()
        {
            if (instance  == null) { instance = new ObservableCollection<MotorData>(); New(); }
            return instance;
        }

        public static void New()
        {
            instance.Clear();
            for (int i = 0; i < SM.MTR._iMaxMotr; i++)
            {
                string sTemp = SM.MTR.Para[i].sMotrName;

                if(sTemp == null) sTemp = "NULL";
                instance.Add(new MotorData() {
                    Items = sTemp
                });
            }
            
        }

        public static void Update()
        {
            if(instance.Count != SM.MTR._iMaxMotr) return;

            for (int i = 0; i < SM.MTR._iMaxMotr; i++)
            {
                string sTemp = SM.MTR.Para[i].sMotrName;
                if(sTemp == null) sTemp = "NULL";

                instance[i].Items = sTemp;
            }
            
        }

    }
    #endregion

    //---------------------------------------------------------------------------------------------
    //Data
    //---------------------------------------------------------------------------------------------
}
