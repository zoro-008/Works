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
            //OM.EqpStat.iLastWorkStep = int.Parse(tbSetLvNo.Text);
            //SEQ.XRAY.iWorkStep = int.Parse(tbSetLvNo.Text);
            //OM.EqpStat.iLastWorkStep = SEQ.XRAY.iWorkStep;

            UpdateComOptn(false);
            OM.SaveCmnOptn();
            
        } 
        
        public void UpdateComOptn(bool _bToTable)
        {                                                                                   
            if (_bToTable == true) 
            {
                CConfig.ValToCon(tbSetLvNo      , ref OM.CmnOptn.iSetLvNo       );
                //CConfig.ValToCon(tbCrntLvNo   , ref OM.CmnOptn.iCrntLvNo);  
                //인덱스 옵션                                                 
                CConfig.ValToCon(cbBarcodeSkip  , ref OM.CmnOptn.bBarcodeSkip   );
                                                                              
                CConfig.ValToCon(cbIgnrDoor     , ref OM.CmnOptn.bIgnrDoor      );
                CConfig.ValToCon(cbIgnrCnctErr  , ref OM.CmnOptn.bIgnrCnctErr   );
                CConfig.ValToCon(cbIgnrSerialErr , ref OM.CmnOptn.bIgnrSerialErr);
                                                                              
                CConfig.ValToCon(tbXrayRptCnt   , ref OM.CmnOptn.iXrayRptCnt    );
                CConfig.ValToCon(tbDressyPath   , ref OM.CmnOptn.sDressyPath    );
                CConfig.ValToCon(tbEzSensorPath , ref OM.CmnOptn.sEzSensorPath  );

                CConfig.ValToCon(cbSkipEntr     , ref OM.CmnOptn.bSkipEntr      );
                CConfig.ValToCon(cbSkipAging    , ref OM.CmnOptn.bSkipAging     );
                CConfig.ValToCon(cbSkipMTF      , ref OM.CmnOptn.bSkipMTF       );
                CConfig.ValToCon(cbSkipCalib    , ref OM.CmnOptn.bSkipCalib     );
                CConfig.ValToCon(cbSkipSkull    , ref OM.CmnOptn.bSkipSkull     );
                CConfig.ValToCon(tbBuzzOffTime  , ref OM.CmnOptn.iBuzzOffTime   );

                //Ver 1.0.3.0
                //Record Option 프로그램 문제 발생 시 화면 녹화해서 확인 가능하도록 추가
                CConfig.ValToCon(cbUseRecord    , ref OM.CmnOptn.bUseRecord     );
                CConfig.ValToCon(tbRecordPath   , ref OM.CmnOptn.sRecordPath    );
            }
            else 
            {
                OM.CCmnOptn CmnOptn = OM.CmnOptn;
                CConfig.ConToVal(tbSetLvNo      , ref OM.CmnOptn.iSetLvNo      );
                //CConfig.ConToVal(tbCrntLvNo     , ref OM.CmnOptn.iCrntLvNo); 
                                                                               
                CConfig.ConToVal(cbBarcodeSkip  , ref OM.CmnOptn.bBarcodeSkip  );
                                                                               
                CConfig.ConToVal(cbIgnrDoor     , ref OM.CmnOptn.bIgnrDoor     );
                CConfig.ConToVal(cbIgnrCnctErr  , ref OM.CmnOptn.bIgnrCnctErr  );
                CConfig.ConToVal(cbIgnrSerialErr, ref OM.CmnOptn.bIgnrSerialErr);
                                                
                CConfig.ConToVal(tbXrayRptCnt   , ref OM.CmnOptn.iXrayRptCnt   );
                CConfig.ConToVal(tbDressyPath   , ref OM.CmnOptn.sDressyPath   );
                CConfig.ConToVal(tbEzSensorPath , ref OM.CmnOptn.sEzSensorPath );

                CConfig.ConToVal(cbSkipEntr     , ref OM.CmnOptn.bSkipEntr     );
                CConfig.ConToVal(cbSkipAging    , ref OM.CmnOptn.bSkipAging    );
                CConfig.ConToVal(cbSkipMTF      , ref OM.CmnOptn.bSkipMTF      );
                CConfig.ConToVal(cbSkipCalib    , ref OM.CmnOptn.bSkipCalib    );
                CConfig.ConToVal(cbSkipSkull    , ref OM.CmnOptn.bSkipSkull    );

                CConfig.ConToVal(tbBuzzOffTime  , ref OM.CmnOptn.iBuzzOffTime  );

                //Ver 1.0.3.0
                //Record Option 프로그램 문제 발생 시 화면 녹화해서 확인 가능하도록 추가
                CConfig.ConToVal(cbUseRecord    , ref OM.CmnOptn.bUseRecord    );
                CConfig.ConToVal(tbRecordPath   , ref OM.CmnOptn.sRecordPath   );

                //Auto Log
                Type type = CmnOptn.GetType();
                FieldInfo[] f = type.GetFields(BindingFlags.Public|BindingFlags.NonPublic | BindingFlags.Instance);
                for(int i = 0 ; i < f.Length ; i++)
                {
                    Trace(f[i].Name, f[i].GetValue(CmnOptn).ToString(), f[i].GetValue(OM.CmnOptn).ToString());
                }

                /*
                Trace(label26.Text, CmnOptn.iSetLvNo .ToString(), OM.CmnOptn.iSetLvNo .ToString());
                Trace(label25.Text, CmnOptn.iCrntLvNo.ToString(), OM.CmnOptn.iCrntLvNo.ToString());

                //인덱스 옵션
                Trace(cbBarcodeSkip  .Text, CmnOptn.bBarcodeSkip  .ToString(), OM.CmnOptn.bBarcodeSkip  .ToString());
                                     
                Trace(cbIgnrDoor     .Text, CmnOptn.bIgnrDoor     .ToString(), OM.CmnOptn.bIgnrDoor     .ToString());
                Trace(cbIgnrCnctErr  .Text, CmnOptn.bIgnrCnctErr  .ToString(), OM.CmnOptn.bIgnrCnctErr  .ToString());
                Trace(cbIgnrSerialErr.Text, CmnOptn.bIgnrSerialErr.ToString(), OM.CmnOptn.bIgnrSerialErr.ToString());

                Trace(label5         .Text, CmnOptn.iXrayRptCnt   .ToString(), OM.CmnOptn.iXrayRptCnt   .ToString());
                Trace(label7         .Text, CmnOptn.sDressyPath   .ToString(), OM.CmnOptn.sDressyPath   .ToString());
                Trace(label8         .Text, CmnOptn.sEzSensorPath .ToString(), OM.CmnOptn.sEzSensorPath .ToString());

                Trace(cbSkipEntr     .Text, CmnOptn.bSkipEntr     .ToString(), OM.CmnOptn.bSkipEntr     .ToString());
                Trace(cbSkipAging    .Text, CmnOptn.bSkipAging    .ToString(), OM.CmnOptn.bSkipAging    .ToString());
                Trace(cbSkipMTF      .Text, CmnOptn.bSkipMTF      .ToString(), OM.CmnOptn.bSkipMTF      .ToString());
                Trace(cbSkipCalib    .Text, CmnOptn.bSkipCalib    .ToString(), OM.CmnOptn.bSkipCalib    .ToString());
                Trace(cbSkipSkull    .Text, CmnOptn.bSkipSkull    .ToString(), OM.CmnOptn.bSkipSkull    .ToString());
                */
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
                 if(OM.DevInfo.iMacroType == 0) tbCrntLvNo.Text = (SEQ.XRYD.iWorkStep + 1).ToString();
            else if(OM.DevInfo.iMacroType == 1) tbCrntLvNo.Text = (SEQ.XRYE.iWorkStep + 1).ToString();
            
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

        private void button1_Click(object sender, EventArgs e)
        {
            //1.0.1.5
            //워크리스트 옮길때 Cal쪽 데이터들 초기화한다.
            SEQ.Mcr.Dr1.CalDataClear();
            if (int.Parse(tbSetLvNo.Text) < 1) tbSetLvNo.Text = "1";
                 if(OM.DevInfo.iMacroType == 0) SEQ.XRYD.iWorkStep = int.Parse(tbSetLvNo.Text) - 1;
            else if(OM.DevInfo.iMacroType == 1) SEQ.XRYE.iWorkStep = int.Parse(tbSetLvNo.Text) - 1;
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            DialogResult Rslt = fd.ShowDialog();
            if (Rslt != DialogResult.OK) return;
            tbDressyPath.Text = fd.FileName;  
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            DialogResult Rslt = fd.ShowDialog();
            if (Rslt != DialogResult.OK) return;
            tbEzSensorPath.Text = fd.FileName; 
        }

        private void btRecordPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            DialogResult Rslt = fd.ShowDialog();
            if (Rslt != DialogResult.OK) return;
            tbRecordPath.Text = fd.SelectedPath + "\\Record";
        }
    }
}
