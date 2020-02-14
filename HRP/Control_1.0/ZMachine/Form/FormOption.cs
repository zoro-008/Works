using System;
using System.Drawing;
using System.Windows.Forms;
using COMMON;
using System.IO;
using System.Reflection;
using SML;

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

            ////파일 버전, 수정한날짜 보여줄때 필요한 부분
            //string sExeFolder = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase;
            //string FileName = Path.GetFileName(sExeFolder);
            //FileInfo File = new FileInfo(FileName);
            ////파일 버전 보여주는 부분
            //string sFileVersion = System.Windows.Forms.Application.ProductVersion;  
            //lbVer.Text          = "Version " + sFileVersion;
            ////수정한 날짜 보여주는 부분
            //double Age  = File.LastWriteTime.ToOADate();
            ////string Date = DateTime.FromOADate(Age).ToString("''yyyy'_ 'M'_ 'd'_ 'tt' 'h': 'm''");
            //string Date = DateTime.FromOADate(Age).ToString("yyyy-MM-dd HH:mm:ss");
            //lbDate.Text = Date;

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
                CConfig.ValToCon(cbLoadingStop    , ref OM.CmnOptn.bLoadingStop   );
                CConfig.ValToCon(cbVISNSkip       , ref OM.CmnOptn.bVisnSkip      );
                CConfig.ValToCon(cbMARKSkip       , ref OM.CmnOptn.bMarkSkip      );

                CConfig.ValToCon(tbPaintName1     , ref OM.CmnOptn.sPaintName1    );
                CConfig.ValToCon(tbPaintName2     , ref OM.CmnOptn.sPaintName2    );
                CConfig.ValToCon(tbPaintName3     , ref OM.CmnOptn.sPaintName3    );
                CConfig.ValToCon(tbPaintName4     , ref OM.CmnOptn.sPaintName4    );
                CConfig.ValToCon(tbPaintName5     , ref OM.CmnOptn.sPaintName5    );

            }
            else 
            {
                OM.CCmnOptn CmnOptn = OM.CmnOptn;
                CConfig.ConToVal(cbLoadingStop    , ref OM.CmnOptn.bLoadingStop   );
                CConfig.ConToVal(cbVISNSkip       , ref OM.CmnOptn.bVisnSkip      );
                CConfig.ConToVal(cbMARKSkip       , ref OM.CmnOptn.bMarkSkip      );

                CConfig.ConToVal(tbPaintName1     , ref OM.CmnOptn.sPaintName1    );
                CConfig.ConToVal(tbPaintName2     , ref OM.CmnOptn.sPaintName2    );
                CConfig.ConToVal(tbPaintName3     , ref OM.CmnOptn.sPaintName3    );
                CConfig.ConToVal(tbPaintName4     , ref OM.CmnOptn.sPaintName4    );
                CConfig.ConToVal(tbPaintName5     , ref OM.CmnOptn.sPaintName5    );

                //Auto Log
                Type type = CmnOptn.GetType();
                FieldInfo[] f = type.GetFields(BindingFlags.Public|BindingFlags.NonPublic | BindingFlags.Instance);
                for(int i = 0 ; i < f.Length ; i++){
                    Trace(f[i].Name, f[i].GetValue(CmnOptn).ToString(), f[i].GetValue(OM.CmnOptn).ToString());
                }

                UpdateComOptn(true);
            }
        }

        public void UpdateEqpOptn(bool _bToTable)
        {                                                                                   
            if (_bToTable == true) 
            {
                
                CConfig.ValToCon(tbEquipName      , ref OM.EqpOptn.sEquipName     );
                CConfig.ValToCon(tbEquipSerial    , ref OM.EqpOptn.sEquipSerial   );

            }
            else 
            {
                OM.CEqpOptn EqpOptn = OM.EqpOptn;

                CConfig.ConToVal(tbEquipName      , ref OM.EqpOptn.sEquipName     );
                CConfig.ConToVal(tbEquipSerial    , ref OM.EqpOptn.sEquipSerial   );

                //Auto Log
                Type type = EqpOptn.GetType();
                FieldInfo[] f = type.GetFields(BindingFlags.Public|BindingFlags.NonPublic | BindingFlags.Instance);
                for(int i = 0 ; i < f.Length ; i++){
                    Trace(f[i].Name, f[i].GetValue(EqpOptn).ToString(), f[i].GetValue(OM.EqpOptn).ToString());
                }

                UpdateComOptn(true);
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

        private void FormOption_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible) timer1.Enabled = true;
        }

        private void Trace(string sText, string _s1, string _s2)
        {
            Log.Trace(sText.Trim() + " : " + _s1 + " -> " + _s2,ti.Dev);
        }
        private void Trace(string sText, int _s1, int _s2)
        {
            Log.Trace(sText.Trim() + " : " + _s1.ToString() + " -> " + _s2.ToString(),ti.Dev);
        }
        private void Trace(string sText, double _s1, double _s2)
        {
            Log.Trace(sText.Trim() + " : " + _s1.ToString() + " -> " + _s2.ToString(),ti.Dev);
        }
        private void Trace(string sText, bool _s1, bool _s2)
        {
            Log.Trace(sText.Trim() + " : " + _s1.ToString() + " -> " + _s2.ToString(),ti.Dev);
        }


    }
}
