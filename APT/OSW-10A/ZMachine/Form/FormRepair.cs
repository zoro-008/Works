using SML2;
using System;
using System.Windows.Forms;

namespace Machine
{
    public partial class FormRepair : Form
    {
        public FormRepair()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (cbUnderRepair.Checked)
            {
                SPC.FLR.Data.EngineerID = SML.FrmLogOn.GetId() ; //나중에 진섭이 머지 할때 바꾸자.
                SPC.FLR.Data.Purpose    = tbPurpose.Text ; 
            }

            OM.EqpStat.bMaint = cbUnderRepair.Checked ;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void FormRepair_Shown(object sender, EventArgs e)
        {
            cbUnderRepair.Checked = OM.EqpStat.bMaint ; 
        }

        private void FormRepair_VisibleChanged(object sender, EventArgs e)
        {
            //cbUnderRepair.Checked = OM.EqpStat.bMaint ; 
        }
    }
}
