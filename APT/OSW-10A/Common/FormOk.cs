using System;
using System.Drawing;
using System.Windows.Forms;

namespace COMMON
{
    public partial class FormOk : Form
    {
        public FormOk()
        {
            InitializeComponent();

            this.TopMost = true;

            int iWidthHalfSize  = (int)(this.Width  / 2);
            int iHeightHalfSize = (int)(this.Height / 2);
            
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
        public void ShowForm(string _sCaption , string _sMessage , int _iTime)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new VoidDelegate(delegate() { lbCaption.Text = _sCaption; lbMsg.Text = _sMessage; if (_iTime != 0) { tmHide.Interval = _iTime; tmHide.Enabled = true; } }));
                this.Invoke(new VoidDelegate(this.Show));
            }
            else
            {
                lbCaption.Text = _sCaption; 
                lbMsg.Text = _sMessage;
                this.Show();
            }
        }
        public void CloseForm()
        {
            HideForm();
        }


        private void btOk_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        
        public void HideForm()
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

        private void tmHide_Tick(object sender, EventArgs e)
        {
            this.Hide();
        }


    }
}
