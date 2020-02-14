using COMMON;
using System;
using System.Drawing;
using System.Windows.Forms;


namespace SML
{
    public partial class FormErr : Form
    {
        /*
         이 문제를 해결하는 방법은 위에서 결론으로 말한 Invoke method를 이용하는 것이다. msdn에서는 Invoke를 다음과 같이 설명한다.

           # public Object Invoke( Delegate Method, param Object[] args)
           # WPF에서는 DispatcherObject를 만든 thread만 해당 객체에 access할 수 있습니다. 예를 들어 기본 UI thread에서 분리
           # 된 background thread는 UI thread에서 작성된 Button의 콘텐츠를 업데이트 할 수 없습니다.
           # background thread가 Button의 Content 속성에 access 하려면 background thread에서 UI thread에 연결된 Dispatcher에
           # 작업을 위임(delegate)해야 합니다. 이 작업은 Invoke 또는 BeginInvoke를 사용하여 수행할 수 있습니다. Invoke는 동기
           # 적이고 BeginInvoke는 비동기적 입니다. 작업은 지정된 DispatcherPriority에서 Dispatcher의 이벤트 큐에 추가됩니다.
           
           즉, 다른 thread에서는 UI thread(Main thread)에 직접 접근을 할 수 없기 때문에 UI thread의 Invoke method를 이용하여, 
         * 다른 Thread에서 호출할 메소드를 delegate로 정의하여 넘겨줘야 한다. 호출될 메소드의 parameter가 있다면 이는 object 객체로 넘겨주면 된다. 
         * 이렇게 해주면 Main thread에게 "너 볼일 다보고 나서 이것도 좀 해줘" 라는 식으로 전달된다.
         */
        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern System.IntPtr CreateRoundRectRgn
        (
             int nLeftRect, // x-coordinate of upper-left corner
             int nTopRect, // y-coordinate of upper-left corner
             int nRightRect, // x-coordinate of lower-right corner
             int nBottomRect, // y-coordinate of lower-right corner
             int nWidthEllipse, // height of ellipse
             int nHeightEllipse // width of ellipse
        );
        int m_iActiveErrNo;
        public FormErr()
        {
            //switch(SM.ERR._eLangSel)
            //{
            //    default:                 CLanguage.ChangeLanguage("en"     ); break;
            //    case EN_LAN_SEL.English: CLanguage.ChangeLanguage("en"     ); break;
            //    case EN_LAN_SEL.Korean : CLanguage.ChangeLanguage("ko"     ); break;
            //    case EN_LAN_SEL.Chinese: CLanguage.ChangeLanguage("zh-Hans"); break;
            //}

            this.TopMost = true;
            InitializeComponent();
            //this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, this.Width, this.Height, 15, 15));

            pnErrPic.Visible = SM.ERR._bUseErrPic ;
            pnErr.Visible    = SM.ERR._bUseErrPic ;

            int iWidthHalfSize  = this.Width  / 2;
            int iHeightHalfSize = this.Height / 2;

            if(!SM.ERR._bUseErrPic)
            {
                iHeightHalfSize -= pnErrPic.Height / 2;
                iHeightHalfSize -= pnErr.Height    / 2;
            }
            
            Point pP = new Point();

            pP.X = (int)((Screen.PrimaryScreen.Bounds.Width  / 2)) - (iWidthHalfSize ) ;
            pP.Y = (int)((Screen.PrimaryScreen.Bounds.Height / 2)) - (iHeightHalfSize) ;
            
            this.Location = pP;

            //요주 부위.
            //페어런트로 FormMain 부터 설정 해하고   TopLevel=false로 다 설정 하고 정상적으로 ParentTree  만들면 Invoke까지 문제 없이 되는데.
            //실제 창이 안뜸 안뜨는것인지 다른 폼 밑에 있는 것인지 모르겠지만 안뜸.
            //다시 페어런트 트리 다 끊고 그냥 핸들만 만들었을 경우 정상 동작 하나 야매 같음...
            //문제 될 소지 있음.
            if (!this.IsHandleCreated)
            {
                this.CreateHandle();
            }
            
        }

        //Show
        delegate void VoidDelegate();

        public void ShowErr(int _iNo)
        {
            
            m_iActiveErrNo = _iNo;

            if (this.InvokeRequired)
            {
                this.Invoke(new VoidDelegate(delegate() { SM.ERR.DisplayErrForm(tbName, tbEnum, tbAction, tbErrMsg, pbErr); this.Show(); }));
                //this.Invoke(new VoidDelegate(this.Show));
            }
            else
            {
                SM.ERR.DisplayErrForm(tbName, tbEnum, tbAction, tbErrMsg, pbErr);
                this.Show();
                
            }
            
        }
        
        public void HideErr()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new VoidDelegate(this.Hide));
            }
            else
            {
                this.Hide();
            }
        }

        private struct TErrTracker
        {
            public const int iThickness = 6;
            public Rectangle Rect     ;
            public EClickPos eClickPos;
            public bool bDown;

            public int iPosX   ;
            public int iPosY   ;
            public int iPrePosX;
            public int iPrePosY;
        }

        TErrTracker m_tErrTracker;

        private void pbErr_Paint(object sender, PaintEventArgs e)
        {
            int iErrNo;
            iErrNo = SM.ERR.GetLastErr();

            double dOfsScale = 0.0;

            if (pbErr.Image == null) return;

            //int iImgErrWidth = pbErr.Image.Width;
            //int iImgErrHeight = pbErr.Image.Height;

            int iImgErrWidth  = SM.ERR.GetPanelSize().iWidth ;
            int iImgErrHeight = SM.ERR.GetPanelSize().iHeight;

            double dWidthSc  = pbErr.Width  / (double)iImgErrWidth  ;
            double dHeightSc = pbErr.Height / (double)iImgErrHeight ;

            double dXPos = (double)SM.ERR.GetErrConfig(iErrNo).dRectLeft * (double)(dWidthSc + dOfsScale);
            double dYpos = (double)SM.ERR.GetErrConfig(iErrNo).dRectTop * (double)(dHeightSc + dOfsScale);

            m_tErrTracker.Rect.X = (int)dXPos;
            m_tErrTracker.Rect.Y = (int)dYpos;
            m_tErrTracker.Rect.Width  = (int)SM.ERR.GetErrConfig(iErrNo).dRectWidth  ;
            m_tErrTracker.Rect.Height = (int)SM.ERR.GetErrConfig(iErrNo).dRectHeight ;

            double dRectWidthSc  = m_tErrTracker.Rect.Width  * dWidthSc ;
            double dRectHeightSc = m_tErrTracker.Rect.Height * dHeightSc;

            Graphics g = e.Graphics;

            Pen RP = new Pen(Color.Red, 6);
            g.DrawRectangle(RP, m_tErrTracker.Rect.X, m_tErrTracker.Rect.Y, (float)dRectWidthSc, (float)dRectHeightSc);
            RP.Dispose();
            //g.Dispose();
            pbErr.SizeMode = PictureBoxSizeMode.StretchImage;
        }


        private void btClose_Click(object sender, EventArgs e)
        {
            HideErr();
        }

        private void FormErr_Load(object sender, EventArgs e)
        {

        }


        /*
         * private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        */

    }
}
