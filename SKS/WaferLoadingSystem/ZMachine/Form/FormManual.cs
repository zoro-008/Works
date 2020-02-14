using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Machine
{
    public partial class FormManual : Form
    {
        public FormManual(Panel _pnBase)
        {
            InitializeComponent();

            this.TopLevel = false;
            this.Parent = _pnBase;
            this.Left = 0 ;
            this.Top  = 0 ;

            //this.Dock = DockStyle.Fill;
            

            //Scable Setting
            //this.Dock = DockStyle.Fill;
            //int  _iWidth  = _pnBase.Width;
            //int  _iHeight = _pnBase.Height;
            //
            //const int  iWidth  = 1920;
            //const int  iHeight = 920;
            //
            //float widthRatio  = _iWidth   / (float)iWidth;// this.ClientSize.Width;//1280f;
            //float heightRatio = _iHeight  / (float)iHeight;//.ClientSize.Height; //863f ;
            //
            //SizeF scale = new SizeF(widthRatio, heightRatio);
            //this.Scale(scale);

            //foreach (Control control in this.Controls)
            //{
            //control.Scale(scale); 
            //control.Font = new Font("Verdana", control.Font.SizeInPoints * heightRatio * widthRatio);
            //}

        }

        private void FormManual_Load(object sender, EventArgs e)
        {
            //pnManualMain.Dock = DockStyle.None ;
            //pnManualMain.Left = 0 ;
            //pnManualMain.Top  = 0 ;
            //pnManualMain.Width = Parent.Width ;
            //pnManualMain.Height = Parent.Height ;
        }

        private void tmUpdate_Tick(object sender, EventArgs e)
        {

        }
    }
}
