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
using System.IO;
using System.Reflection;
using SML2;
using DWORD = System.UInt32;
using System.Diagnostics;

namespace Machine
{
    public partial class FormVision : Form
    {
        public FormVision(Panel _pnBase)
        {
            InitializeComponent();

            this.TopLevel = false;
            this.Parent = _pnBase;

            

        }

        private void FormVision_Shown(object sender, EventArgs e)
        {
            
        }


    }
}
