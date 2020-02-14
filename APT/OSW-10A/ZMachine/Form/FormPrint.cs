using System;
using System.Drawing;
using System.Windows.Forms;

namespace Machine
{
    public partial class FormPrint : Form
    {
        //RS232_110Xi4 Zebra = new RS232_110Xi4(0,"BarPrinter");
        public FormPrint()
        {
            InitializeComponent();
            tmUpdate.Enabled = true;
        }

        private void buttonSendData_Click(object sender, EventArgs e)
        {
            string strTray_ID, strType, strMatNo, strLotId, strQty, strBinNo, strDesc, strToff;
            int iYOffset;

            if(!int.TryParse(tbYOffset.Text, out iYOffset)){
                iYOffset = 48;
            }
            strTray_ID = textBoxTrayId.Text;
            strType    = textBoxType.Text;
            strMatNo   = textBoxMatNo.Text;
            strLotId   = textBoxLotId.Text;
            strQty     = textBoxQty.Text;
            strBinNo   = string.Format("{0:000}", int.Parse(textBoxBinNo.Text));
            strDesc    = textBoxDesc.Text;
            strToff    = string.Format("{0:000}", numericUpDown1Toff.Value);
            SEQ.BarcordPrnt.PrintBar(iYOffset , strTray_ID, strType, strMatNo, strLotId, strQty, strBinNo, strDesc, strToff);

        }

        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            lbStep.Text = SEQ.BarcordPrnt.iStep.ToString() ; 

            // 화면 Update
            if (SEQ.BarcordPrnt.Stat.isPauseFlag    ) lblPause.BackColor         = Color.Green; else lblPause.BackColor         = Color.Red ;
            if (SEQ.BarcordPrnt.Stat.isPaperout     ) lblPaperOut.BackColor      = Color.Green; else lblPaperOut.BackColor      = Color.Red ;
            if (SEQ.BarcordPrnt.Stat.isRibonoutFlag ) lblRibionOut.BackColor     = Color.Green; else lblRibionOut.BackColor     = Color.Red ;
            if (SEQ.BarcordPrnt.Stat.isHeadupFlag   ) lblHeadUp.BackColor        = Color.Green; else lblHeadUp.BackColor        = Color.Red ;
            if (SEQ.BarcordPrnt.Stat.isLabelWaitFlag) lblLabelWaitFlag.BackColor = Color.Green; else lblLabelWaitFlag.BackColor = Color.Red ;
        }
    }
}
