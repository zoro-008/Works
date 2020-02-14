using COMMON;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Threading;
//using System.Windows.Forms;

namespace SML.View
{
    /// <summary>
    /// Error.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Error : Window
    {
        public DispatcherTimer Timer = new DispatcherTimer();

        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern System.IntPtr CreateRoundRectRgn
        (
             int nLeftRect      , // x-coordinate of upper-left corner
             int nTopRect       , // y-coordinate of upper-left corner
             int nRightRect     , // x-coordinate of lower-right corner
             int nBottomRect    , // y-coordinate of lower-right corner
             int nWidthEllipse  , // height of ellipse
             int nHeightEllipse   // width of ellipse
        );
        int m_iActiveErrNo;
        public Error()
        {
            InitializeComponent();

            Picture.Visibility = SM.ERR._bUseErrPic ? Visibility.Visible : Visibility.Hidden;

            int iWidthHalfSize  = (int)(FormError.Width  / 2);
            int iHeightHalfSize = (int)(FormError.Height / 2);

            if (!SM.ERR._bUseErrPic)
            {
                iHeightHalfSize -= (int)(Picture.Height / 2);
            }

            Point pP = new Point();

            pP.X = (int)((System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width  / 2)) - (iWidthHalfSize );
            pP.Y = (int)((System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height / 2)) - (iHeightHalfSize);

            //this.Location = pP;

            this.Left = pP.X;
            this.Top  = pP.Y;

            //요주 부위.
            //페어런트로 FormMain 부터 설정 해하고   TopLevel=false로 다 설정 하고 정상적으로 ParentTree  만들면 Invoke까지 문제 없이 되는데.
            //실제 창이 안뜸 안뜨는것인지 다른 폼 밑에 있는 것인지 모르겠지만 안뜸.
            //다시 페어런트 트리 다 끊고 그냥 핸들만 만들었을 경우 정상 동작 하나 야매 같음...
            //문제 될 소지 있음.
            //WPF 컨트롤에 자체 핸들이 없어 CreateHandle을 실행할수없음
            //다른 스레드에서 UI 호출/업데이트는 디스패처를 통해서 한다.
            //Application.Current.Dispatcher
            //if (!this.IsHandleCreated)
            //{
            //    this.CreateHandle();
            //}

            //UI 타이머
            Timer.Interval = TimeSpan.FromMilliseconds(100);
            Timer.Tick += new EventHandler(Timer_Tick);
        }

        //Show
        delegate void VoidDelegate();

        public void ShowErr(int _iNo)
        {
            m_iActiveErrNo = _iNo;
            
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new VoidDelegate(delegate () { SM.ERR.WPF_DisplayErrForm(Name, Enum, Action, Msg, pbErr, Tracker, pbErr.Width, pbErr.Height); }));
                Dispatcher.Invoke(new VoidDelegate(this.Show));
            }
            else
            {
                SM.ERR.WPF_DisplayErrForm(Name, Enum, Action, Msg, pbErr, Tracker, pbErr.Width, pbErr.Height);
                this.Show();
            
            }
        }

        public void HideErr()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new VoidDelegate(this.Hide));
            }
            else
            {
                this.Hide();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            
        }

        private void BtClose_Click_1(object sender, RoutedEventArgs e)
        {
            HideErr();
        }

        private void pbErr_MediaEnded(object sender, RoutedEventArgs e)
        {
            pbErr.Position = new TimeSpan(0, 0, 1);
            pbErr.Play();
        }
    }
}
