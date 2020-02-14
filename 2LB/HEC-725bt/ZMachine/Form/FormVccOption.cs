using System;
using System.Windows.Forms;

namespace Machine
{
    public partial class FormVccOption : Form
    {
        public FormVccOption()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);
            
            SEQ.Reset();
            
            
            switch(iBtnTag)
            {
                //case 1: DM.ARAY[(int)ri.PCK].SetStat(cs.None); 
                //        DM.ARAY[(int)ri.IDX1].ChangeStat(cs.Working, cs.Unkwn);
                //        DM.ARAY[(int)ri.IDX2].ChangeStat(cs.Working, cs.Unkwn);
                //        DM.ARAY[(int)ri.IDX3].ChangeStat(cs.Working, cs.Unkwn);
                //        SEQ._bBtnStart = true; break;

                //case 2: SEQ._bBtnStart = true; break;

                //case 3: if (DM.ARAY[(int)ri.IDX1].GetCntStat(cs.Working) != 0) DM.ARAY[(int)ri.IDX1].ChangeStat(cs.Unkwn, cs.Empty);
                //        if (DM.ARAY[(int)ri.IDX2].GetCntStat(cs.Working) != 0) DM.ARAY[(int)ri.IDX2].ChangeStat(cs.Unkwn, cs.Empty);
                //        if (DM.ARAY[(int)ri.IDX3].GetCntStat(cs.Working) != 0) DM.ARAY[(int)ri.IDX3].ChangeStat(cs.Unkwn, cs.Empty);
                //        SEQ._bBtnStart = true; break;

                //case 4: DM.ARAY[(int)ri.IDX1].ChangeStat(cs.Unkwn, cs.Empty);
                //        DM.ARAY[(int)ri.IDX2].ChangeStat(cs.Unkwn, cs.Empty);
                //        DM.ARAY[(int)ri.IDX3].ChangeStat(cs.Unkwn, cs.Empty); 
                //        SEQ._bBtnStart = true; break;

                case 5: this.Hide(); break;
            }
            //SEQ.PCK.bPickMiss = false;
            this.Hide();
        }
    }
}
