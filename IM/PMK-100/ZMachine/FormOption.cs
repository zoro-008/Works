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
using SMDll2;
using DWORD = System.UInt32;
using System.Diagnostics;

namespace Machine
{
    public partial class FormOption : Form
    {
        FormMain FrmMain;

        public FormOption(Panel _pnBase)
        {
            InitializeComponent();

            this.TopLevel = false;
            this.Parent = _pnBase;

            //FrmMain = _FrmMain;

            //파일 버전, 수정한날짜 보여줄때 필요한 부분
            string sExeFolder = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase;
            string FileName = Path.GetFileName(sExeFolder);
            FileInfo File = new FileInfo(FileName);
            //파일 버전 보여주는 부분
            string sFileVersion = System.Windows.Forms.Application.ProductVersion;  
            lbVer.Text          = "Ver " + sFileVersion;
            //수정한 날짜 보여주는 부분
            double Age  = File.LastWriteTime.ToOADate();
            string Date = DateTime.FromOADate(Age).ToString("''yyyy'_ 'M'_ 'd'_ 'tt' 'h': 'm''");
            lbDate.Text = Date;

            UpdateComOptn(true);
            OM.LoadCmnOptn();

        }

        private void btSave_Click(object sender, EventArgs e)
        {
            //Check Running Status.
            if (SEQ._bRun) 
            {
                Log.ShowMessage("Warning", "Can't Save during Autorunning!");
                return;
            }

            if (Log.ShowMessageModal("Confirm", "Are you Sure?") != DialogResult.Yes) return;
        
            UpdateComOptn(false);
            OM.SaveCmnOptn();

        } 
        
        public void UpdateComOptn(bool _bToTable)
        {                                                                                   
            if (_bToTable == true) 
            {
                cbIgnrDoor   .Checked = OM.CmnOptn.bIgnrDoor;
                             
                cbVisnSkip   .Checked = OM.CmnOptn.bVisnSkip;
                cbAirBlwrSkip.Checked = OM.CmnOptn.bAirBlwrSkip;
                //tbTipCleanDelay.Text = COptnMan.CmnOptn.iTipCleanDelay.ToString();
               
            }
            else 
            {
                OM.CmnOptn.bIgnrDoor    = cbIgnrDoor.Checked;
                                        
                OM.CmnOptn.bVisnSkip    = cbVisnSkip.Checked;
                OM.CmnOptn.bAirBlwrSkip = cbAirBlwrSkip.Checked;
                
                //OM.SaveCmnOptn();
                //UpdateComOptn(true);
            }
        }

        private void FormOption_Shown(object sender, EventArgs e)
        {
            timer1.Enabled = true;
        }

        public bool bUpdate = false;
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            if(bUpdate) {
                UpdateComOptn(true);
                bUpdate = false;
            }
            
            timer1.Enabled = true;
        }
    }
}
