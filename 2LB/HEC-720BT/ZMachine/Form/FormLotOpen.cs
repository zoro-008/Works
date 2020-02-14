using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using COMMON;
using SML2;

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

            DM.ARAY[(int)ri.IDX].SetStat(cs.Empty);

            SPC.LOT.ClearData();

            if (!SM.IO_GetX(xi.IDX_Detect1)) DM.ARAY[ri.IDX].SetStat(0, 0, cs.Unkwn);
            else                             DM.ARAY[ri.IDX].SetStat(0, 0, cs.Empty);
            if (!SM.IO_GetX(xi.IDX_Detect2)) DM.ARAY[ri.IDX].SetStat(0, 1, cs.Unkwn);
            else                             DM.ARAY[ri.IDX].SetStat(0, 1, cs.Empty);
            if (!SM.IO_GetX(xi.IDX_Detect3)) DM.ARAY[ri.IDX].SetStat(0, 2, cs.Unkwn);
            else                             DM.ARAY[ri.IDX].SetStat(0, 2, cs.Empty);
            if (!SM.IO_GetX(xi.IDX_Detect4)) DM.ARAY[ri.IDX].SetStat(0, 3, cs.Unkwn);
            else                             DM.ARAY[ri.IDX].SetStat(0, 3, cs.Empty);
            if (!SM.IO_GetX(xi.IDX_Detect5)) DM.ARAY[ri.IDX].SetStat(0, 4, cs.Unkwn);
            else                             DM.ARAY[ri.IDX].SetStat(0, 4, cs.Empty);
            
            LOT.LotOpen(LotNo, Device);
            Log.Trace("LotOpen", "Try");
            OM.EqpStat.iWorkCnt = 0;
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
