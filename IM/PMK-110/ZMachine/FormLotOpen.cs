using System;
using System.Windows.Forms;
using COMMON;

namespace Machine
{
    public partial class FormLotOpen : Form
    {
        public static FormLotOpen FrmLotOpen;

        public FormLotOpen()
        {
            InitializeComponent();

            ////Control Scale 변경
            //float widthRatio = 1024f / 1280f;
            //float heightRatio = 607f / 863f;
            //SizeF scale = new SizeF(widthRatio, heightRatio);
            //this.Scale(scale);
            
            //foreach (Control control in this.Controls)
            //{
            //    control.Font = new Font("Verdana", control.Font.SizeInPoints * heightRatio * widthRatio);
            //}
            
            tbSelDevice.Text = OM.GetCrntDev();
            tbLotId.Text = "";

            tbLotId.Focus();
            
        }

        private void btLotOpen_Click(object sender, EventArgs e)
        {
            if(tbLotId.Text == "") tbLotId.Text = DateTime.Now.ToString("HHmmss");

            string LotNo  = tbLotId.Text.Trim();
            string Device = tbSelDevice.Text.Trim();
            
            LOT.LotOpen(LotNo, Device);

            OM.EqpStat.iSerialNo = 0 ;
            //에러는 내부에서 띄운다.
            SEQ.Laser.SetCycle(RS232_DominoDynamark3.Cycle.SetLotNo , true);//맨뒤에 인자 내부에서 에러 띄우는 옵션.

            Log.Trace("LotOpen", "Try");
            Close();
            
    
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            Close();   
        }

        private void FormLotOpen_Shown(object sender, EventArgs e)
        {
            tbLotId.Focus();
        }


    }
}
