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
using SMDll2;

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

            //DM.ARAY[(int)ri.PICK].SetStat(cs.Empty);
            if (OM.CmnOptn.bIgnrLeftPck)
            {
                DM.ARAY[(int)ri.PICK].SetStat(0, 0, cs.None);
                DM.ARAY[(int)ri.PICK].SetStat(1, 0, cs.Empty);
            }
            else if (OM.CmnOptn.bIgnrRightPck)
            {
                DM.ARAY[(int)ri.PICK].SetStat(0, 0, cs.Empty);
                DM.ARAY[(int)ri.PICK].SetStat(1, 0, cs.None);
            }
            else
            {
                DM.ARAY[(int)ri.PICK].SetStat(cs.Empty);
            }

            DM.ARAY[(int)ri.LENS].SetStat(cs.Unkwn);
            if(!OM.CmnOptn.bSkipRear)
            {
                DM.ARAY[(int)ri.REAR].SetStat(cs.Unkwn);
            }
            else
            {
                DM.ARAY[(int)ri.REAR].SetStat(cs.None);
            }

            if (!OM.CmnOptn.bSkipFrnt)
            {
                DM.ARAY[(int)ri.FRNT].SetStat(cs.Unkwn);
            }
            else
            {
                DM.ARAY[(int)ri.FRNT].SetStat(cs.None);
            }

            SPC.LOT.LoadSaveLotIni(true);

            SEQ.Reset();
            //DM.ARAY[(int)ri.PICK].SetStat(cs.Empty);
            //if (DM.ARAY[(int)ri.LENS].CheckAllStat(cs.Empty))                         DM.ARAY[(int)ri.LENS].SetStat(cs.Unkwn);
            //
            //if (DM.ARAY[(int)ri.REAR].CheckAllStat(cs.Work))                          DM.ARAY[(int)ri.REAR].SetStat(cs.Unkwn);
            //if (OM.CmnOptn.bSkipRear)                                                 DM.ARAY[(int)ri.REAR].SetStat(cs.None );
            //if (!OM.CmnOptn.bSkipRear && DM.ARAY[(int)ri.REAR].CheckAllStat(cs.None)) DM.ARAY[(int)ri.REAR].SetStat(cs.Unkwn);
            //
            //if (DM.ARAY[(int)ri.FRNT].CheckAllStat(cs.Work))                          DM.ARAY[(int)ri.FRNT].SetStat(cs.Unkwn);                                        
            //if ( OM.CmnOptn.bSkipFrnt)                                                DM.ARAY[(int)ri.FRNT].SetStat(cs.None );
            //if (!OM.CmnOptn.bSkipFrnt && DM.ARAY[(int)ri.FRNT].CheckAllStat(cs.None)) DM.ARAY[(int)ri.FRNT].SetStat(cs.Unkwn);             

            LOT.LotOpen(LotNo, Device);
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
