using System;
using System.Drawing;
using System.Windows.Forms;
using COMMON;
using System.IO;
using System.Reflection;
using SML;
using System.Collections.Generic;

namespace Machine
{
    public partial class FormOption : Form
    {
        //FormMain FrmMain;
        private const string sFormText = "Form Option ";
        public int iPreWorkMode  = 0;
        public int iCrntWorkMode = 0;

        public FormOption(Panel _pnBase)
        {
            InitializeComponent();

            this.TopLevel = false;
            this.Parent = _pnBase;

            UpdateComOptn(true);
            UpdateEqpOptn(true);

            OM.LoadCmnOptn();

            edMagnetCheck.Minimum = (int)ML.MT_GetMinPosition(mi.MagnetX);
            edMagnetCheck.Maximum = (int)ML.MT_GetMaxPosition(mi.MagnetX);

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
            ((Button)sender).Enabled = false;
            
            UpdateComOptn(false);
            OM.SaveCmnOptn();

            UpdateEqpOptn(false);
            OM.SaveEqpOptn();
            
            
            ((Button)sender).Enabled = true;
        } 

        public void UpdateComOptn(bool _bToTable)
        {                                                                                   
            if (_bToTable == true) 
            {
                CConfig.ValToCon(cbIgnrDoor   , ref OM.CmnOptn.bIgnrDoor   );
                CConfig.ValToCon(LsrMaxDelay  , ref OM.CmnOptn.iLsrMaxDelay);
                CConfig.ValToCon(edMagnetCheck, ref OM.CmnOptn.dMagnetCheck);
                CConfig.ValToCon(edLaserCheck , ref OM.CmnOptn.dLaserCheck );
                CConfig.ValToCon(LsrSttPos    , ref OM.CmnOptn.dLsrSttPos  );
                
                
            }
            else 
            {
                OM.CCmnOptn PreCmnOptn = OM.CmnOptn;
                CConfig.ConToVal(cbIgnrDoor   , ref OM.CmnOptn.bIgnrDoor   );
                CConfig.ConToVal(LsrMaxDelay  , ref OM.CmnOptn.iLsrMaxDelay);
                CConfig.ConToVal(edMagnetCheck, ref OM.CmnOptn.dMagnetCheck);
                CConfig.ConToVal(edLaserCheck , ref OM.CmnOptn.dLaserCheck );
                CConfig.ConToVal(LsrSttPos    , ref OM.CmnOptn.dLsrSttPos  );

                //Auto Log
                Type type = PreCmnOptn.GetType();
                FieldInfo[] f = type.GetFields(BindingFlags.Public|BindingFlags.NonPublic | BindingFlags.Instance);
                for(int i = 0 ; i < f.Length ; i++)
                {
                    if(f[i].GetValue(PreCmnOptn) != f[i].GetValue(OM.CmnOptn))Trace(f[i].Name + " Changed", f[i].GetValue(PreCmnOptn).ToString() , f[i].GetValue(OM.CmnOptn).ToString());
                    else                                                      Trace(f[i].Name             , f[i].GetValue(PreCmnOptn).ToString() , f[i].GetValue(OM.CmnOptn).ToString());
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
                CConfig.ValToCon(cbIgnrSptr       , ref OM.EqpOptn.bIgnrSptr      );
                



            }
            else 
            {
                OM.CEqpOptn EqpOptn = OM.EqpOptn;

                CConfig.ConToVal(tbEquipName      , ref OM.EqpOptn.sEquipName     );
                CConfig.ConToVal(tbEquipSerial    , ref OM.EqpOptn.sEquipSerial   );
                CConfig.ConToVal(cbIgnrSptr       , ref OM.EqpOptn.bIgnrSptr      );

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

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void FormOption_VisibleChanged(object sender, EventArgs e)
        {
            timer1.Enabled = Visible;
            if (Visible)
            {
                UpdateComOptn(true);
                UpdateEqpOptn(true);
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

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
