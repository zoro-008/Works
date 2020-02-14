using COMMON;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Machine
{
    public partial class FormSpectroMain : Form
    {
        static public FormSpectrometer  frmSpectrometer ;
        public FormSpectroMain()
        {
            InitializeComponent();

            Log.ShowMessage("스펙트로미터초기화 중" , "여기서 정지 되면 스펙트로미터USB를 뽑아주세요." , 500);
            Application.DoEvents();

            frmSpectrometer = new FormSpectrometer();
            frmSpectrometer.TopLevel = false ;
            SEQ.Reset();

        }

        private void FormSpectroMain_VisibleChanged(object sender, EventArgs e)
        {
            if(Visible)
            {
                
                frmSpectrometer.Parent = pnBase ;
                frmSpectrometer.Show();
            }
        }

        private void btMinimization_Click(object sender, EventArgs e)
        {

        }
    }
}
