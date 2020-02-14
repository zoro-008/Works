using System;
using System.Drawing;
using System.Windows.Forms;
using COMMON;
using SML;
using System.Reflection;

namespace Machine
{
    public partial class FormDeviceSet : Form
    {

        private const string sFormText = "Form DeviceSet ";

        public FormDeviceSet(Panel _pnBase)
        {
            InitializeComponent();            
            //this.Width = 1272;
            //this.Height = 866;

            this.Left     = 0;
            this.Top      = 0;

            this.TopLevel = false  ;
            this.Parent   = _pnBase;
                      
            PM.Load(OM.GetCrntDev());

        }

        private void FormDeviceSet_Load(object sender, EventArgs e)
        {

            PM.UpdatePstn(true);
            UpdateDevInfo(true);
            UpdateEqpOptn(true);

            tcMain.SelectedIndex = 0;

        }

        public void UpdateDevInfo(bool bToTable)
        {
            if (bToTable)
            {
                CConfig.ValToCon(tbBoltPitch     , ref OM.DevInfo.dBoltPitch      );
                CConfig.ValToCon(tbChannelNo     , ref OM.DevInfo.iChannelNo      );
                CConfig.ValToCon(cbUseAdjTransfer, ref OM.DevInfo.bUseAdjTransfer );
                CConfig.ValToCon(tbCheckTolerance, ref OM.DevInfo.dCheckTolerance );
                CConfig.ValToCon(tbNutWorkOptn1  , ref OM.DevInfo.dNutWorkOptn1   );
                CConfig.ValToCon(tbNutWorkOptn2  , ref OM.DevInfo.dNutWorkOptn2   );
                CConfig.ValToCon(tbNutWorkOptn3  , ref OM.DevInfo.dNutWorkOptn3   );
                CConfig.ValToCon(tbNutLastMotn   , ref OM.DevInfo.dNutLastMotn    );
                CConfig.ValToCon(cbFStgOptn      , ref OM.DevInfo.iBoltFindOptn   );
                CConfig.ValToCon(cbAddWork       , ref OM.DevInfo.bAddWork        );
                
                CConfig.ValToCon(tbWorkOfs       , ref OM.DevInfo.dWorkOfs        );
                CConfig.ValToCon(tbBfMaxTq       , ref OM.DevInfo.dBfMaxTq        );
                CConfig.ValToCon(tbMaxTq         , ref OM.DevInfo.dMaxTq          );
                CConfig.ValToCon(tbMinTq         , ref OM.DevInfo.dMinTq          );

                CConfig.ValToCon(tbImgPath       , ref OM.DevInfo.sImgPath        );
                
                
            }
            else 
            {
                OM.CDevInfo DevInfo = OM.DevInfo;

                CConfig.ConToVal(tbBoltPitch     , ref OM.DevInfo.dBoltPitch      );
                CConfig.ConToVal(tbChannelNo     , ref OM.DevInfo.iChannelNo      );
                CConfig.ConToVal(cbUseAdjTransfer, ref OM.DevInfo.bUseAdjTransfer );
                CConfig.ConToVal(tbCheckTolerance, ref OM.DevInfo.dCheckTolerance );
                CConfig.ConToVal(tbNutWorkOptn1  , ref OM.DevInfo.dNutWorkOptn1   );
                CConfig.ConToVal(tbNutWorkOptn2  , ref OM.DevInfo.dNutWorkOptn2   );
                CConfig.ConToVal(tbNutWorkOptn3  , ref OM.DevInfo.dNutWorkOptn3   );
                CConfig.ConToVal(tbNutLastMotn   , ref OM.DevInfo.dNutLastMotn    );
                CConfig.ConToVal(cbFStgOptn      , ref OM.DevInfo.iBoltFindOptn   );
                CConfig.ConToVal(cbAddWork       , ref OM.DevInfo.bAddWork        );

                CConfig.ConToVal(tbWorkOfs       , ref OM.DevInfo.dWorkOfs        );
                CConfig.ConToVal(tbBfMaxTq       , ref OM.DevInfo.dBfMaxTq        );
                CConfig.ConToVal(tbMaxTq         , ref OM.DevInfo.dMaxTq          );
                CConfig.ConToVal(tbMinTq         , ref OM.DevInfo.dMinTq          );

                CConfig.ConToVal(tbImgPath       , ref OM.DevInfo.sImgPath        );

                //Auto Log
                Type type = DevInfo.GetType();
                FieldInfo[] f = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                for (int i = 0; i < f.Length; i++)
                {
                    Trace(f[i].Name, f[i].GetValue(DevInfo).ToString(), f[i].GetValue(OM.DevInfo).ToString());
                }

                UpdateDevInfo(true);
            }
        
        }


        public void UpdateEqpOptn(bool _bToTable)
        {                                                                                   
            if (_bToTable == true) 
            {
                //CConfig.ValToCon(edetc2  , ref OM.EqpOptn.sEquipName     );
                //CConfig.ValToCon(edetc1  , ref OM.EqpOptn.sEquipSerial   );
            }
            else 
            {
                OM.CEqpOptn EqpOptn = OM.EqpOptn;

                //CConfig.ConToVal(edetc2  , ref OM.EqpOptn.sEquipName     );
                //CConfig.ConToVal(edetc1  , ref OM.EqpOptn.sEquipSerial   );

                //Auto Log
                Type type = EqpOptn.GetType();
                FieldInfo[] f = type.GetFields(BindingFlags.Public|BindingFlags.NonPublic | BindingFlags.Instance);
                for(int i = 0 ; i < f.Length ; i++){
                    Trace(f[i].Name, f[i].GetValue(EqpOptn).ToString(), f[i].GetValue(OM.EqpOptn).ToString());
                }

                UpdateEqpOptn(true);
            }
        }

        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            tmUpdate.Enabled = false;
            //센서 보여주기
            bool bSsr = false;

            lbTq0.Text = (OM.DevInfo.dBfMaxTq / 10.0).ToString("N1") + "N.m";
            lbTq1.Text = (OM.DevInfo.dMaxTq   / 10.0).ToString("N1") + "N.m";
            lbTq2.Text = (OM.DevInfo.dMinTq   / 10.0).ToString("N1") + "N.m";

            tmUpdate.Enabled = true;
        }

        private void FormDeviceSet_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmUpdate.Enabled = false;
        }


        private void FormDeviceSet_Shown(object sender, EventArgs e)
        {
            tmUpdate.Enabled = true;
        }

        private void FormDeviceSet_VisibleChanged(object sender, EventArgs e)
        {
            tmUpdate.Enabled = this.Visible;
        }
    
        private void Trace(string sText, string _s1, string _s2)
        {
            Log.Trace(sText.Trim() + " : " + _s1 + " -> " + _s2,ForContext.Dev);
        }
        private void Trace(string sText, int _s1, int _s2)
        {
            Log.Trace(sText.Trim() + " : " + _s1.ToString() + " -> " + _s2.ToString(),ForContext.Dev);
        }
        private void Trace(string sText, double _s1, double _s2)
        {
            Log.Trace(sText.Trim() + " : " + _s1.ToString() + " -> " + _s2.ToString(),ForContext.Dev);
        }
        private void Trace(string sText, bool _s1, bool _s2)
        {
            Log.Trace(sText.Trim() + " : " + _s1.ToString() + " -> " + _s2.ToString(),ForContext.Dev);
        }
        
        private void btSaveDevice_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.TraceListView(sFormText + sText + " Button Clicked", ForContext.Frm);

            if (Log.ShowMessageModal("Confirm", "Are you Sure?") != DialogResult.Yes) return;


            UpdateDevInfo(false);
            UpdateEqpOptn(false);

            OM.SaveJobFile(OM.GetCrntDev());
            OM.SaveEqpOptn();

            PM.UpdatePstn(false);
            PM.Save(OM.GetCrntDev());
            PM.UpdatePstn(true);

            SEQ.rsNut.SetCh(OM.DevInfo.iChannelNo);

            Refresh();
        }

        private void btImgPath_Click(object sender, EventArgs e)
        {
            OpenFileDialog OpenImg = new OpenFileDialog();
            OpenImg.InitialDirectory = @"./";
            OpenImg.Filter = "그림 파일 (*.jpg, *.gif, *.bmp) | *.jpg; *.gif; *.bmp; | 모든 파일 (*.*) | *.*";
            OpenImg.FilterIndex = 1;
            OpenImg.RestoreDirectory = true;
            if (OpenImg.ShowDialog() == DialogResult.OK)
            {
                string fileFullName = OpenImg.FileName;
                tbImgPath.Text = fileFullName;

                //pbErrImg.ImageLocation = fileFullName;

                //this.pbErrImg.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }
    }
}
