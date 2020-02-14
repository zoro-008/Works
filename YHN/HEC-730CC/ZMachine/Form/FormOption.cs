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
            
            //비전 결과 색 이름 표기
            //SetDisp(ri.RLT1);SetDisp(ri.RLT2);SetDisp(ri.RLT3);
            //SetDisp(ri.WRK1);SetDisp(ri.WRK2);SetDisp(ri.WRK3);
            //SetDisp(ri.PSTB);
            //SetDisp(ri.SPC );
            
            ((Button)sender).Enabled = true;
        } 
               
        public void UpdateComOptn(bool _bToTable)
        {                                                                                   
            if (_bToTable == true) 
            {
                CConfig.ValToCon(cbIgnrDoor       , ref OM.CmnOptn.bIgnrDoor       );
                CConfig.ValToCon(cbLPCK_IonOff    , ref OM.CmnOptn.bLPCK_IonOff    );
                CConfig.ValToCon(cbPSTR_IonBtmOff , ref OM.CmnOptn.bPSTR_IonBtmOff );
                CConfig.ValToCon(cbPSTR_IonTopOff , ref OM.CmnOptn.bPSTR_IonTopOff );
                CConfig.ValToCon(cbVSTG_IonOff    , ref OM.CmnOptn.bVSTG_IonOff    );
                CConfig.ValToCon(cbLPCK_Air1Off   , ref OM.CmnOptn.bLPCK_Air1Off   );
                CConfig.ValToCon(cbPSTR_Air1BtmOff, ref OM.CmnOptn.bPSTR_Air1BtmOff);
                CConfig.ValToCon(cbPSTR_Air1TopOff, ref OM.CmnOptn.bPSTR_Air1TopOff);
                CConfig.ValToCon(cbVSTG_Air1Off   , ref OM.CmnOptn.bVSTG_Air1Off   );
                CConfig.ValToCon(cbLPCK_Air2Off   , ref OM.CmnOptn.bLPCK_Air2Off   );
                CConfig.ValToCon(cbPSTR_Air2BtmOff, ref OM.CmnOptn.bPSTR_Air2BtmOff);
                CConfig.ValToCon(cbPSTR_Air2TopOff, ref OM.CmnOptn.bPSTR_Air2TopOff);
                CConfig.ValToCon(cbVSTG_Air2Off   , ref OM.CmnOptn.bVSTG_Air2Off   );
            }
            else 
            {
                OM.CCmnOptn PreCmnOptn = OM.CmnOptn;
                CConfig.ConToVal(cbIgnrDoor       , ref OM.CmnOptn.bIgnrDoor       );
                CConfig.ConToVal(cbLPCK_IonOff    , ref OM.CmnOptn.bLPCK_IonOff    );
                CConfig.ConToVal(cbPSTR_IonBtmOff , ref OM.CmnOptn.bPSTR_IonBtmOff );
                CConfig.ConToVal(cbPSTR_IonTopOff , ref OM.CmnOptn.bPSTR_IonTopOff );
                CConfig.ConToVal(cbVSTG_IonOff    , ref OM.CmnOptn.bVSTG_IonOff    );
                CConfig.ConToVal(cbLPCK_Air1Off   , ref OM.CmnOptn.bLPCK_Air1Off   );
                CConfig.ConToVal(cbPSTR_Air1BtmOff, ref OM.CmnOptn.bPSTR_Air1BtmOff);
                CConfig.ConToVal(cbPSTR_Air1TopOff, ref OM.CmnOptn.bPSTR_Air1TopOff);
                CConfig.ConToVal(cbVSTG_Air1Off   , ref OM.CmnOptn.bVSTG_Air1Off   );
                CConfig.ConToVal(cbLPCK_Air2Off   , ref OM.CmnOptn.bLPCK_Air2Off   );
                CConfig.ConToVal(cbPSTR_Air2BtmOff, ref OM.CmnOptn.bPSTR_Air2BtmOff);
                CConfig.ConToVal(cbPSTR_Air2TopOff, ref OM.CmnOptn.bPSTR_Air2TopOff);
                CConfig.ConToVal(cbVSTG_Air2Off   , ref OM.CmnOptn.bVSTG_Air2Off   );

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
                CConfig.ValToCon(sVisnPath        , ref OM.EqpOptn.sVisnPath      );
                
                

            }
            else 
            {
                OM.CEqpOptn EqpOptn = OM.EqpOptn;

                CConfig.ConToVal(tbEquipName      , ref OM.EqpOptn.sEquipName     );
                CConfig.ConToVal(tbEquipSerial    , ref OM.EqpOptn.sEquipSerial   );
                CConfig.ConToVal(sVisnPath        , ref OM.EqpOptn.sVisnPath      );

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

        private void lbDate_Click(object sender, EventArgs e)
        {

        }

        private void lbVer_Click(object sender, EventArgs e)
        {

        }

        private void pnColor0_Click(object sender, EventArgs e)
        {
            Panel pnSel = sender as Panel ;
            
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                pnSel.BackColor = cd.Color ; 
            }
        }
    }
}
