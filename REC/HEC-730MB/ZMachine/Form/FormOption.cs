using System;
using System.Drawing;
using System.Windows.Forms;
using COMMON;
using System.IO;
using System.Reflection;

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
            lbVer.Text          = "VERSION " + sFileVersion;
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
            SEQ.LEFT.MoveKg(ax.ETC_LoadCell1,0);                                                                               
            if (_bToTable == true) 
            {
                CConfig.ValToCon(bIgnrDoor    , ref OM.CmnOptn.bIgnrDoor   );
                CConfig.ValToCon(bIgnrCam     , ref OM.CmnOptn.bIgnrCam    );
                CConfig.ValToCon(cb1          , ref OM.CmnOptn.bUse_L_Cort );
                CConfig.ValToCon(cb2          , ref OM.CmnOptn.bUse_R_Cort );
                CConfig.ValToCon(cb3          , ref OM.CmnOptn.bUse_L_Part );
                CConfig.ValToCon(cb4          , ref OM.CmnOptn.bUse_R_Part );
                CConfig.ValToCon(tb1          , ref OM.CmnOptn.sLeftFolder );
                CConfig.ValToCon(tb2          , ref OM.CmnOptn.sRighFolder );
                CConfig.ValToCon(cb5          , ref OM.CmnOptn.bUse_L_Dark );
                CConfig.ValToCon(cb6          , ref OM.CmnOptn.bUse_R_Dark );
                CConfig.ValToCon(tbL1         , ref OM.CmnOptn.dLoadOfs1   );
                CConfig.ValToCon(tbL2         , ref OM.CmnOptn.dLoadOfs2   );
                CConfig.ValToCon(tbL3         , ref OM.CmnOptn.dLoadOfs3   );
                CConfig.ValToCon(tbL4         , ref OM.CmnOptn.iLoadTime   );
                CConfig.ValToCon(tbL5         , ref OM.CmnOptn.dLoadRange  );

                CConfig.ValToCon(tbLS1        , ref OM.CmnOptn.dLS1        );
                CConfig.ValToCon(tbLS2        , ref OM.CmnOptn.dLS2        );
                CConfig.ValToCon(tbLS3        , ref OM.CmnOptn.dLS3        );
                CConfig.ValToCon(tbLS4        , ref OM.CmnOptn.dLS4        );
                CConfig.ValToCon(tbLS5        , ref OM.CmnOptn.dLS5        );
                CConfig.ValToCon(tbLF1        , ref OM.CmnOptn.dLF1        );
                CConfig.ValToCon(tbLF2        , ref OM.CmnOptn.dLF2        );
                CConfig.ValToCon(tbLF3        , ref OM.CmnOptn.dLF3        );
                CConfig.ValToCon(tbLF4        , ref OM.CmnOptn.dLF4        );
                CConfig.ValToCon(tbLF5        , ref OM.CmnOptn.dLF5        );
                
                


            }
            else 
            {
                OM.CCmnOptn CmnOptn = OM.CmnOptn;
                CConfig.ConToVal(bIgnrDoor    , ref OM.CmnOptn.bIgnrDoor   );
                CConfig.ConToVal(bIgnrCam     , ref OM.CmnOptn.bIgnrCam    );
                CConfig.ConToVal(cb1          , ref OM.CmnOptn.bUse_L_Cort );
                CConfig.ConToVal(cb2          , ref OM.CmnOptn.bUse_R_Cort );
                CConfig.ConToVal(cb3          , ref OM.CmnOptn.bUse_L_Part );
                CConfig.ConToVal(cb4          , ref OM.CmnOptn.bUse_R_Part );
                CConfig.ConToVal(tb1          , ref OM.CmnOptn.sLeftFolder );
                CConfig.ConToVal(tb2          , ref OM.CmnOptn.sRighFolder );
                CConfig.ConToVal(cb5          , ref OM.CmnOptn.bUse_L_Dark );
                CConfig.ConToVal(cb6          , ref OM.CmnOptn.bUse_R_Dark );
                CConfig.ConToVal(tbL1         , ref OM.CmnOptn.dLoadOfs1   ); SEQ.CheckValue   (ref OM.CmnOptn.dLoadOfs1      , -1, 1    );
                CConfig.ConToVal(tbL2         , ref OM.CmnOptn.dLoadOfs2   ); SEQ.CheckValue   (ref OM.CmnOptn.dLoadOfs2      , -1, 1    );
                CConfig.ConToVal(tbL3         , ref OM.CmnOptn.dLoadOfs3   ); SEQ.CheckValue   (ref OM.CmnOptn.dLoadOfs3      , -1, 1    );
                CConfig.ConToVal(tbL4         , ref OM.CmnOptn.iLoadTime   ); SEQ.CheckValue   (ref OM.CmnOptn.iLoadTime      , 20, 100  );
                CConfig.ConToVal(tbL5         , ref OM.CmnOptn.dLoadRange  ); SEQ.CheckValue   (ref OM.CmnOptn.dLoadRange     , 0.1, 0.9 );
                                                                                               
                CConfig.ConToVal(tbLS1        , ref OM.CmnOptn.dLS1        ); SEQ.CheckValue   (ref OM.CmnOptn.dLS1      , 0.0001, 0.1 );
                CConfig.ConToVal(tbLS2        , ref OM.CmnOptn.dLS2        ); SEQ.CheckValue   (ref OM.CmnOptn.dLS2      , 20  , 1000  );
                CConfig.ConToVal(tbLS3        , ref OM.CmnOptn.dLS3        ); SEQ.CheckValue   (ref OM.CmnOptn.dLS3      , 30  , 1000  );
                CConfig.ConToVal(tbLS4        , ref OM.CmnOptn.dLS4        ); SEQ.CheckValue   (ref OM.CmnOptn.dLS4      , 40  , 1000  );
                CConfig.ConToVal(tbLS5        , ref OM.CmnOptn.dLS5        ); SEQ.CheckValue   (ref OM.CmnOptn.dLS5      , 50  , 1000  );
                CConfig.ConToVal(tbLF1        , ref OM.CmnOptn.dLF1        ); SEQ.CheckValueMax(ref OM.CmnOptn.dLF1      , 0.0001, 0.1 );
                CConfig.ConToVal(tbLF2        , ref OM.CmnOptn.dLF2        ); SEQ.CheckValueMax(ref OM.CmnOptn.dLF2      , 0.0001, 0.1 );
                CConfig.ConToVal(tbLF3        , ref OM.CmnOptn.dLF3        ); SEQ.CheckValueMax(ref OM.CmnOptn.dLF3      , 0.0001, 0.1 );
                CConfig.ConToVal(tbLF4        , ref OM.CmnOptn.dLF4        ); SEQ.CheckValueMax(ref OM.CmnOptn.dLF4      , 0.0001, 0.1 );
                CConfig.ConToVal(tbLF5        , ref OM.CmnOptn.dLF5        ); SEQ.CheckValueMax(ref OM.CmnOptn.dLF5      , 0.0001, 0.1 );

                //Auto Log
                Type type = CmnOptn.GetType();
                FieldInfo[] f = type.GetFields(BindingFlags.Public|BindingFlags.NonPublic | BindingFlags.Instance);
                for(int i = 0 ; i < f.Length ; i++){
                    Trace(f[i].Name, f[i].GetValue(CmnOptn).ToString(), f[i].GetValue(OM.CmnOptn).ToString());
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

        private void bt1_Click(object sender, EventArgs e)
        {
            //FolderBrowserDialog Dialog1 = new FolderBrowserDialog();
            DialogResult Rslt = folderBrowserDialog1.ShowDialog();
            if (Rslt != DialogResult.OK) return;

            tb1.Text = folderBrowserDialog1.SelectedPath ;

        }

        private void bt2_Click(object sender, EventArgs e)
        {
            //FolderBrowserDialog Dialog1 = new FolderBrowserDialog();
            DialogResult Rslt = folderBrowserDialog1.ShowDialog();
            if (Rslt != DialogResult.OK) return;

            tb2.Text = folderBrowserDialog1.SelectedPath ;

        }
    }
}
