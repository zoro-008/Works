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
                cbUsedLine1.Checked = OM.CmnOptn.bUsedLine1;
                cbUsedLine2.Checked = OM.CmnOptn.bUsedLine2;
                cbUsedLine3.Checked = OM.CmnOptn.bUsedLine3;
                cbUsedLine4.Checked = OM.CmnOptn.bUsedLine4;
                cbUsedLine5.Checked = OM.CmnOptn.bUsedLine5;
                cbIgnrWork .Checked = OM.CmnOptn.bIgnrWork ;
                cbUseAbsPos.Checked = OM.CmnOptn.bUseAbsPos;
            }
            else 
            {
                //COptnMan.CmnOptn.iTipCleanDelay = CConfig.StrToIntDef(tbTipCleanDelay.Text, COptnMan.CmnOptn.iTipCleanDelay);
                OM.CmnOptn.bUsedLine1 = cbUsedLine1.Checked;
                OM.CmnOptn.bUsedLine2 = cbUsedLine2.Checked;
                OM.CmnOptn.bUsedLine3 = cbUsedLine3.Checked;
                OM.CmnOptn.bUsedLine4 = cbUsedLine4.Checked;
                OM.CmnOptn.bUsedLine5 = cbUsedLine5.Checked;
                OM.CmnOptn.bIgnrWork  = cbIgnrWork .Checked;
                OM.CmnOptn.bUseAbsPos = cbUseAbsPos.Checked;

                UpdateComOptn(true);
            }
        }

    }
}
