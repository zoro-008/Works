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

        //public static FormGridSub   FrmGridSub  ;

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
                        
            //OM.LoadCmnOptn();
            UpdateComOptn(true);

            //OM.LoadEqpOptn();
            UpdateEqpOptn(true);

            //Form GridSub Setting
            //FrmGridSub = new FormGridSub();
            //FrmGridSub.TopLevel = false   ;
            //FrmGridSub.Parent   = pnRecipe;
            //FrmGridSub.Dock     = DockStyle.Fill;
            //FrmGridSub.Show();

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
            btSave.Enabled = false;
            UpdateComOptn(false);
            OM.SaveCmnOptn();
            btSave.Enabled = true ;
        } 
        
        public void UpdateComOptn(bool _bToTable)
        {                                                                                   
            if (_bToTable == true) 
            {
                CConfig.ValToCon(tbName1 , ref OM.CmnOptn.sName1   );
                CConfig.ValToCon(tbName2 , ref OM.CmnOptn.sName2   );
                CConfig.ValToCon(tbName3 , ref OM.CmnOptn.sName3   );
                CConfig.ValToCon(tbName4 , ref OM.CmnOptn.sName4   );
                CConfig.ValToCon(tbName5 , ref OM.CmnOptn.sName5   );
                CConfig.ValToCon(tbName6 , ref OM.CmnOptn.sName6   );
                CConfig.ValToCon(tbName7 , ref OM.CmnOptn.sName7   );
                CConfig.ValToCon(tbName8 , ref OM.CmnOptn.sName8   );
                CConfig.ValToCon(tbName9 , ref OM.CmnOptn.sName9   );
                CConfig.ValToCon(tbName10, ref OM.CmnOptn.sName10  );
                CConfig.ValToCon(tbName11, ref OM.CmnOptn.sName11  );
                CConfig.ValToCon(tbName12, ref OM.CmnOptn.sName12  );
                CConfig.ValToCon(tbName13, ref OM.CmnOptn.sName13  );
                CConfig.ValToCon(tbName14, ref OM.CmnOptn.sName14  );
                CConfig.ValToCon(tbName15, ref OM.CmnOptn.sName15  );
                CConfig.ValToCon(tbName16, ref OM.CmnOptn.sName16  );
                CConfig.ValToCon(tbName17, ref OM.CmnOptn.sName17  );
                CConfig.ValToCon(tbName18, ref OM.CmnOptn.sName18  );
                CConfig.ValToCon(tbName19, ref OM.CmnOptn.sName19  );
                CConfig.ValToCon(tbName20, ref OM.CmnOptn.sName20  );

                CConfig.ValToCon(min11   , ref OM.CmnOptn.min11    ); CConfig.ValToCon(max11   , ref OM.CmnOptn.max11    );
                CConfig.ValToCon(min12   , ref OM.CmnOptn.min12    ); CConfig.ValToCon(max12   , ref OM.CmnOptn.max12    );
                                                                                        
                CConfig.ValToCon(min21   , ref OM.CmnOptn.min21    ); CConfig.ValToCon(max21   , ref OM.CmnOptn.max21    );
                CConfig.ValToCon(min22   , ref OM.CmnOptn.min22    ); CConfig.ValToCon(max22   , ref OM.CmnOptn.max22    );
                CConfig.ValToCon(min23   , ref OM.CmnOptn.min23    ); CConfig.ValToCon(max23   , ref OM.CmnOptn.max23    );
                CConfig.ValToCon(min24   , ref OM.CmnOptn.min24    ); CConfig.ValToCon(max24   , ref OM.CmnOptn.max24    );
                CConfig.ValToCon(min25   , ref OM.CmnOptn.min25    ); CConfig.ValToCon(max25   , ref OM.CmnOptn.max25    );
                CConfig.ValToCon(min26   , ref OM.CmnOptn.min26    ); CConfig.ValToCon(max26   , ref OM.CmnOptn.max26    );
                                                                                        
                CConfig.ValToCon(min31   , ref OM.CmnOptn.min31    ); CConfig.ValToCon(max31   , ref OM.CmnOptn.max31    );
                CConfig.ValToCon(min32   , ref OM.CmnOptn.min32    ); CConfig.ValToCon(max32   , ref OM.CmnOptn.max32    );
                CConfig.ValToCon(min33   , ref OM.CmnOptn.min33    ); CConfig.ValToCon(max33   , ref OM.CmnOptn.max33    );
                CConfig.ValToCon(min34   , ref OM.CmnOptn.min34    ); CConfig.ValToCon(max34   , ref OM.CmnOptn.max34    );
                CConfig.ValToCon(min35   , ref OM.CmnOptn.min35    ); CConfig.ValToCon(max35   , ref OM.CmnOptn.max35    );
                CConfig.ValToCon(min36   , ref OM.CmnOptn.min36    ); CConfig.ValToCon(max36   , ref OM.CmnOptn.max36    );
                                                                                        
                CConfig.ValToCon(min41   , ref OM.CmnOptn.min41    ); CConfig.ValToCon(max41   , ref OM.CmnOptn.max41    );
                CConfig.ValToCon(min42   , ref OM.CmnOptn.min42    ); CConfig.ValToCon(max42   , ref OM.CmnOptn.max42    );
                CConfig.ValToCon(min43   , ref OM.CmnOptn.min43    ); CConfig.ValToCon(max43   , ref OM.CmnOptn.max43    );
                CConfig.ValToCon(min44   , ref OM.CmnOptn.min44    ); CConfig.ValToCon(max44   , ref OM.CmnOptn.max44    );
                                                                                        
                CConfig.ValToCon(min51   , ref OM.CmnOptn.min51    ); CConfig.ValToCon(max51   , ref OM.CmnOptn.max51    );
                CConfig.ValToCon(min52   , ref OM.CmnOptn.min52    ); CConfig.ValToCon(max52   , ref OM.CmnOptn.max52    );
                CConfig.ValToCon(min53   , ref OM.CmnOptn.min53    ); CConfig.ValToCon(max53   , ref OM.CmnOptn.max53    );
                CConfig.ValToCon(min54   , ref OM.CmnOptn.min54    ); CConfig.ValToCon(max54   , ref OM.CmnOptn.max54    );

                CConfig.ValToCon(tbStepDelay , ref OM.CmnOptn.iStepDelay ); 

                //CConfig.ValToCon(tbCA, ref OM.CmnOptn.dCMul ); CConfig.ValToCon(tbCB, ref OM.CmnOptn.dCAdd ); 
                //CConfig.ValToCon(tbFA, ref OM.CmnOptn.dFMul ); CConfig.ValToCon(tbFB, ref OM.CmnOptn.dFAdd ); 
                //CConfig.ValToCon(tbGA, ref OM.CmnOptn.dGMul ); CConfig.ValToCon(tbGB, ref OM.CmnOptn.dGAdd ); 

                CConfig.ValToCon(tbArcVoltage , ref OM.CmnOptn.dArcVoltage ); 
                CConfig.ValToCon(tbArcVoltage1, ref OM.CmnOptn.dArcVoltage1); 
                CConfig.ValToCon(tbArcVoltage2, ref OM.CmnOptn.dArcVoltage2); 
                CConfig.ValToCon(tbArcMin    , ref OM.CmnOptn.iMinA      ); 
                CConfig.ValToCon(tbArcMax    , ref OM.CmnOptn.iCheckGap  ); 
                CConfig.ValToCon(tbKeepGateV_P , ref OM.CmnOptn.dKeepGateV_P ); 
                CConfig.ValToCon(tbKeepGateV_M , ref OM.CmnOptn.dKeepGateV_M ); 
                CConfig.ValToCon(tbKeepGateV_A , ref OM.CmnOptn.dKeepGateV_A ); 

                CConfig.ValToCon(tbPTHour    , ref OM.CmnOptn.dProcessTimeLim ); 
                CConfig.ValToCon(tbWTHour    , ref OM.CmnOptn.dWorkTimeLim    ); 
                CConfig.ValToCon(cbPT        , ref OM.CmnOptn.bUsePrecessTime ); 
                CConfig.ValToCon(cbWT        , ref OM.CmnOptn.bUseWorkTime    ); 

                CConfig.ValToCon(tbCA   , ref OM.CmnOptn.dCA   ); 
                CConfig.ValToCon(tbCB   , ref OM.CmnOptn.dCB   ); 
                CConfig.ValToCon(tbCC   , ref OM.CmnOptn.dCC   ); 
                CConfig.ValToCon(tbCUse , ref OM.CmnOptn.bCUse ); 

                CConfig.ValToCon(tbFA   , ref OM.CmnOptn.dFA   ); 
                CConfig.ValToCon(tbFB   , ref OM.CmnOptn.dFB   ); 
                CConfig.ValToCon(tbFC   , ref OM.CmnOptn.dFC   ); 
                CConfig.ValToCon(tbFUse , ref OM.CmnOptn.bFUse ); 
                
                CConfig.ValToCon(tbGA   , ref OM.CmnOptn.dGA   ); 
                CConfig.ValToCon(tbGB   , ref OM.CmnOptn.dGB   ); 
                CConfig.ValToCon(tbGC   , ref OM.CmnOptn.dGC   ); 
                CConfig.ValToCon(tbGUse , ref OM.CmnOptn.bGUse ); 

                CConfig.ValToCon(tbPwErr1 , ref OM.CmnOptn.iPwErr1 ); 
                CConfig.ValToCon(tbPwErr2 , ref OM.CmnOptn.iPwErr2 ); 
                CConfig.ValToCon(tbPwErr3 , ref OM.CmnOptn.iPwErr3 ); 

                CConfig.ValToCon(tbRdErr1 , ref OM.CmnOptn.iRdErr1 ); 
                CConfig.ValToCon(tbRdErr2 , ref OM.CmnOptn.iRdErr2 ); 
                CConfig.ValToCon(tbRdErr3 , ref OM.CmnOptn.iRdErr3 ); 

                rb1.Checked = OM.CmnOptn.bRdShotGate  ;
                rb2.Checked = OM.CmnOptn.bRdShotFouce ;
                rb3.Checked = OM.CmnOptn.bRdShotAll   ;
                    
                
            }
            else 
            {
                OM.CCmnOptn CmnOptn = OM.CmnOptn;
                //CConfig.ConToVal(cbLoadingStop    , ref OM.CmnOptn.bLoadingStop   );
                CConfig.ConToVal(tbName1 , ref OM.CmnOptn.sName1   );
                CConfig.ConToVal(tbName2 , ref OM.CmnOptn.sName2   );
                CConfig.ConToVal(tbName3 , ref OM.CmnOptn.sName3   );
                CConfig.ConToVal(tbName4 , ref OM.CmnOptn.sName4   );
                CConfig.ConToVal(tbName5 , ref OM.CmnOptn.sName5   );
                CConfig.ConToVal(tbName6 , ref OM.CmnOptn.sName6   );
                CConfig.ConToVal(tbName7 , ref OM.CmnOptn.sName7   );
                CConfig.ConToVal(tbName8 , ref OM.CmnOptn.sName8   );
                CConfig.ConToVal(tbName9 , ref OM.CmnOptn.sName9   );
                CConfig.ConToVal(tbName10, ref OM.CmnOptn.sName10  );
                CConfig.ConToVal(tbName11, ref OM.CmnOptn.sName11  );
                CConfig.ConToVal(tbName12, ref OM.CmnOptn.sName12  );
                CConfig.ConToVal(tbName13, ref OM.CmnOptn.sName13  );
                CConfig.ConToVal(tbName14, ref OM.CmnOptn.sName14  );
                CConfig.ConToVal(tbName15, ref OM.CmnOptn.sName15  );
                CConfig.ConToVal(tbName16, ref OM.CmnOptn.sName16  );
                CConfig.ConToVal(tbName17, ref OM.CmnOptn.sName17  );
                CConfig.ConToVal(tbName18, ref OM.CmnOptn.sName18  );
                CConfig.ConToVal(tbName19, ref OM.CmnOptn.sName19  );
                CConfig.ConToVal(tbName20, ref OM.CmnOptn.sName20  );

                CConfig.ConToVal(min11   , ref OM.CmnOptn.min11    ); CConfig.ConToVal(max11   , ref OM.CmnOptn.max11    );
                CConfig.ConToVal(min12   , ref OM.CmnOptn.min12    ); CConfig.ConToVal(max12   , ref OM.CmnOptn.max12    );
                                                                                        
                CConfig.ConToVal(min21   , ref OM.CmnOptn.min21    ); CConfig.ConToVal(max21   , ref OM.CmnOptn.max21    );
                CConfig.ConToVal(min22   , ref OM.CmnOptn.min22    ); CConfig.ConToVal(max22   , ref OM.CmnOptn.max22    );
                CConfig.ConToVal(min23   , ref OM.CmnOptn.min23    ); CConfig.ConToVal(max23   , ref OM.CmnOptn.max23    );
                CConfig.ConToVal(min24   , ref OM.CmnOptn.min24    ); CConfig.ConToVal(max24   , ref OM.CmnOptn.max24    );
                CConfig.ConToVal(min25   , ref OM.CmnOptn.min25    ); CConfig.ConToVal(max25   , ref OM.CmnOptn.max25    );
                CConfig.ConToVal(min26   , ref OM.CmnOptn.min26    ); CConfig.ConToVal(max26   , ref OM.CmnOptn.max26    );
                                                                                        
                CConfig.ConToVal(min31   , ref OM.CmnOptn.min31    ); CConfig.ConToVal(max31   , ref OM.CmnOptn.max31    );
                CConfig.ConToVal(min32   , ref OM.CmnOptn.min32    ); CConfig.ConToVal(max32   , ref OM.CmnOptn.max32    );
                CConfig.ConToVal(min33   , ref OM.CmnOptn.min33    ); CConfig.ConToVal(max33   , ref OM.CmnOptn.max33    );
                CConfig.ConToVal(min34   , ref OM.CmnOptn.min34    ); CConfig.ConToVal(max34   , ref OM.CmnOptn.max34    );
                CConfig.ConToVal(min35   , ref OM.CmnOptn.min35    ); CConfig.ConToVal(max35   , ref OM.CmnOptn.max35    );
                CConfig.ConToVal(min36   , ref OM.CmnOptn.min36    ); CConfig.ConToVal(max36   , ref OM.CmnOptn.max36    );
                                                                                        
                CConfig.ConToVal(min41   , ref OM.CmnOptn.min41    ); CConfig.ConToVal(max41   , ref OM.CmnOptn.max41    );
                CConfig.ConToVal(min42   , ref OM.CmnOptn.min42    ); CConfig.ConToVal(max42   , ref OM.CmnOptn.max42    );
                CConfig.ConToVal(min43   , ref OM.CmnOptn.min43    ); CConfig.ConToVal(max43   , ref OM.CmnOptn.max43    );
                CConfig.ConToVal(min44   , ref OM.CmnOptn.min44    ); CConfig.ConToVal(max44   , ref OM.CmnOptn.max44    );
                                                                                        
                CConfig.ConToVal(min51   , ref OM.CmnOptn.min51    ); CConfig.ConToVal(max51   , ref OM.CmnOptn.max51    );
                CConfig.ConToVal(min52   , ref OM.CmnOptn.min52    ); CConfig.ConToVal(max52   , ref OM.CmnOptn.max52    );
                CConfig.ConToVal(min53   , ref OM.CmnOptn.min53    ); CConfig.ConToVal(max53   , ref OM.CmnOptn.max53    );
                CConfig.ConToVal(min54   , ref OM.CmnOptn.min54    ); CConfig.ConToVal(max54   , ref OM.CmnOptn.max54    );

                CConfig.ConToVal(tbStepDelay , ref OM.CmnOptn.iStepDelay,0,60000); 

                //CConfig.ConToVal(tbCA, ref OM.CmnOptn.dCMul ,0,1.5); CConfig.ConToVal(tbCB, ref OM.CmnOptn.dCAdd ,0,0.05); 
                //CConfig.ConToVal(tbFA, ref OM.CmnOptn.dFMul ,0,1.5); CConfig.ConToVal(tbFB, ref OM.CmnOptn.dFAdd ,0,0.05); 
                //CConfig.ConToVal(tbGA, ref OM.CmnOptn.dGMul ,0,1.5); CConfig.ConToVal(tbGB, ref OM.CmnOptn.dGAdd ,0,0.05); 

                CConfig.ConToVal(tbArcVoltage , ref OM.CmnOptn.dArcVoltage ,0,100   ); 
                CConfig.ConToVal(tbArcVoltage1, ref OM.CmnOptn.dArcVoltage1,0,6     ); 
                CConfig.ConToVal(tbArcVoltage2, ref OM.CmnOptn.dArcVoltage2,0,6     ); 
                CConfig.ConToVal(tbArcMin     , ref OM.CmnOptn.iMinA       ,0,255   ); 
                CConfig.ConToVal(tbArcMax     , ref OM.CmnOptn.iCheckGap   ,0,FormCam_XNB.iAvrQueMax-1); 

                CConfig.ConToVal(tbKeepGateV_P , ref OM.CmnOptn.dKeepGateV_P ,0,100); 
                CConfig.ConToVal(tbKeepGateV_M , ref OM.CmnOptn.dKeepGateV_M ,0,100); 
                CConfig.ConToVal(tbKeepGateV_A , ref OM.CmnOptn.dKeepGateV_A ,0,10 ); 



                CConfig.ConToVal(tbPTHour    , ref OM.CmnOptn.dProcessTimeLim ); 
                CConfig.ConToVal(tbWTHour    , ref OM.CmnOptn.dWorkTimeLim    ); 
                CConfig.ConToVal(cbPT        , ref OM.CmnOptn.bUsePrecessTime ); 
                CConfig.ConToVal(cbWT        , ref OM.CmnOptn.bUseWorkTime    ); 

                CConfig.ConToVal(tbCA   , ref OM.CmnOptn.dCA   ); 
                CConfig.ConToVal(tbCB   , ref OM.CmnOptn.dCB   ); 
                CConfig.ConToVal(tbCC   , ref OM.CmnOptn.dCC   ); 
                CConfig.ConToVal(tbCUse , ref OM.CmnOptn.bCUse ); 

                CConfig.ConToVal(tbFA   , ref OM.CmnOptn.dFA   ); 
                CConfig.ConToVal(tbFB   , ref OM.CmnOptn.dFB   ); 
                CConfig.ConToVal(tbFC   , ref OM.CmnOptn.dFC   ); 
                CConfig.ConToVal(tbFUse , ref OM.CmnOptn.bFUse ); 
                
                CConfig.ConToVal(tbGA   , ref OM.CmnOptn.dGA   ); 
                CConfig.ConToVal(tbGB   , ref OM.CmnOptn.dGB   ); 
                CConfig.ConToVal(tbGC   , ref OM.CmnOptn.dGC   ); 
                CConfig.ConToVal(tbGUse , ref OM.CmnOptn.bGUse ); 

                CConfig.ConToVal(tbPwErr1 , ref OM.CmnOptn.iPwErr1 ,0,10); 
                CConfig.ConToVal(tbPwErr2 , ref OM.CmnOptn.iPwErr2 ,0,10); 
                CConfig.ConToVal(tbPwErr3 , ref OM.CmnOptn.iPwErr3 ,0,10); 

                CConfig.ConToVal(tbRdErr1 , ref OM.CmnOptn.iRdErr1 ,0,10); 
                CConfig.ConToVal(tbRdErr2 , ref OM.CmnOptn.iRdErr2 ,0,10); 
                CConfig.ConToVal(tbRdErr3 , ref OM.CmnOptn.iRdErr3 ,0,10); 

                OM.CmnOptn.bRdShotGate  = rb1.Checked ;
                OM.CmnOptn.bRdShotFouce = rb2.Checked ;
                OM.CmnOptn.bRdShotAll   = rb3.Checked ;

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
                CConfig.ValToCon(cbTestMode       , ref OM.EqpOptn.bTestMode      );

                CConfig.ValToCon(cbTestMode       , ref OM.EqpOptn.bTestMode      );
                CConfig.ValToCon(cbClearStop      , ref OM.EqpOptn.bClearStop     );
                

            }
            else 
            {
                OM.CEqpOptn EqpOptn = OM.EqpOptn;

                CConfig.ConToVal(tbEquipName      , ref OM.EqpOptn.sEquipName     );
                CConfig.ConToVal(tbEquipSerial    , ref OM.EqpOptn.sEquipSerial   );
                CConfig.ConToVal(cbTestMode       , ref OM.EqpOptn.bTestMode      );
                CConfig.ConToVal(cbClearStop      , ref OM.EqpOptn.bClearStop     );

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
                
                bUpdate = false;
            }
            
            timer1.Enabled = true;
        }

        private void FormOption_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible) {
                UpdateComOptn(true);
                //timer1.Enabled = true;
            }
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

        private void textBox9_TextChanged(object sender, EventArgs e)
        {

        }

        private void label34_Click(object sender, EventArgs e)
        {

        }

        private void btSaveE_Click(object sender, EventArgs e)
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
            FormOperation.TestClear();
            btSave.Enabled = false;
            UpdateEqpOptn(false);
            OM.SaveEqpOptn();
            btSave.Enabled = true ;
        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void panel10_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
