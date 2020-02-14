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
using System.IO;
using System.Xml.Serialization;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Collections;
using System.Runtime.InteropServices;

namespace Machine
{
    public partial class FormSubErr : Form
    {
        public FormSubErr()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);
            
            SEQ.Reset();
            
            int c,r;
            int Array ;
            SEQ.SSTG.FindChip(out Array , out c, out r );
            
            switch(iBtnTag)
            {
                    //다시 디스펜싱.
                case 1: DM.ARAY[Array].SetStat(c,r,cs.Disp);                         
                        break;
                    //스킵하고 진행.
                case 2: DM.ARAY[Array].SetStat(c,r,cs.Attach); 
                        
                        break;
                    //비전 재검사.
                case 3: break;
            }
            //SEQ.PCK.bPickMiss = false;
            SEQ.Reset();
            this.Hide();
            
        }
    }
}
