using COMMON;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Machine
{
    public partial class FormOption : Form
    {
        //FormMain FrmMain;
        private const string sFormText = "Form Option ";

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
            lbVer.Text          = "Version " + sFileVersion;
            //수정한 날짜 보여주는 부분
            double Age  = File.LastWriteTime.ToOADate();
            //string Date = DateTime.FromOADate(Age).ToString("''yyyy'_ 'M'_ 'd'_ 'tt' 'h': 'm''");
            string Date = DateTime.FromOADate(Age).ToString("yyyy-MM-dd HH:mm:ss");
            lbDate.Text = Date;

            UpdateComOptn(true);
            OM.LoadCmnOptn();

        }

        private void btSave_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

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
                
                CConfig.ValToCon(cbRewindMode, ref OM.CmnOptn.bRewindMode);
                CConfig.ValToCon(cbUseRear   , ref OM.CmnOptn.bUseRear   );
                CConfig.ValToCon(cbUseFrnt   , ref OM.CmnOptn.bUseFrnt   );
                CConfig.ValToCon(cbUseYBfPos , ref OM.CmnOptn.bUseYBfPos );
                CConfig.ValToCon(tbRewindRVel, ref OM.CmnOptn.dRewindRVel);
                CConfig.ValToCon(tbRewindYVel, ref OM.CmnOptn.dRewindYVel);
                
                

            }
            else 
            {
                OM.CCmnOptn CmnOptn = OM.CmnOptn;
                
                CConfig.ConToVal(cbRewindMode, ref OM.CmnOptn.bRewindMode);
                CConfig.ConToVal(cbUseRear   , ref OM.CmnOptn.bUseRear   );
                CConfig.ConToVal(cbUseFrnt   , ref OM.CmnOptn.bUseFrnt   );
                CConfig.ConToVal(cbUseYBfPos , ref OM.CmnOptn.bUseYBfPos );
                CConfig.ConToVal(tbRewindRVel, ref OM.CmnOptn.dRewindRVel);
                CConfig.ValToCon(tbRewindYVel, ref OM.CmnOptn.dRewindYVel);
                if(OM.CmnOptn.dRewindRVel <= 0 ) OM.CmnOptn.dRewindRVel = 1 ;
                if(OM.CmnOptn.dRewindYVel <= 0 ) OM.CmnOptn.dRewindYVel = 0.001 ;

                //Auto Log
                Type type = CmnOptn.GetType();
                FieldInfo[] f = type.GetFields(BindingFlags.Public|BindingFlags.NonPublic | BindingFlags.Instance);
                for(int i = 0 ; i < f.Length ; i++){
                    Trace(f[i].Name, f[i].GetValue(CmnOptn).ToString(), f[i].GetValue(OM.CmnOptn).ToString());
                }

                UpdateComOptn(true);
                //Trace(cbRewindMode.Checked, CmnOptn.iSetLvNo .ToString(), OM.CmnOptn.iSetLvNo .ToString());
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

        private void Trace(string sText, string _s1, string _s2)
        {
            Log.Trace(sText + " : " + _s1 + " -> " + _s2, ti.Dev);
        }

        private void FormOption_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible) timer1.Enabled = true;
        }
    }
}
