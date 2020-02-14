using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Machine
{
    public partial class FormOkNg : Form
    {
        int    iResult = 0 ;
        string sText   = "";
        public FormOkNg()
        {
            InitializeComponent();
        }

        public void SetResult(int _iResult, string _sText = "")
        {
            iResult = _iResult;
            sText   = _sText  ;
        }

        private void FormOkNg_VisibleChanged(object sender, EventArgs e)
        {
            if (iResult == 0)
            {
                lbOkNg.Text = "OK";
                lbText.Text = sText;
                lbOkNg.BackColor = Color.Lime;
                BackColor = Color.Lime;
            }
            else if (iResult == 1)
            {
                lbOkNg.Text = "측정 NG";
                lbText.Text = sText;
                lbOkNg.BackColor = Color.Red;
                BackColor = Color.Red;
            }
            else
            {
                lbOkNg.Text = "체결 NG";
                lbText.Text = sText;
                lbOkNg.BackColor = Color.Red;
                BackColor = Color.Red;
            }

        }
    }
}
