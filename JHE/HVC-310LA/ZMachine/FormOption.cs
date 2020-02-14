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

            tmUpdate.Enabled = true;

        }

      

        private void btSave_Click(object sender, EventArgs e)
        {
            //Check Running Status.
            if (SEQ._bRun) 
            {
                Log.ShowMessage("Warning", "Can't Save during Autorunning!");
                return;
            }

            if (cbSkipFrnt.Checked && cbSkipRear.Checked)
            {
                Log.ShowMessage("Warning", "You can not use both functions at the same time");
                cbSkipFrnt.Checked = false;
                cbSkipRear.Checked = false;
                return;
            }

            if (cbIgnrLeftPck.Checked && cbIgnrRightPck.Checked)
            {
                Log.ShowMessage("Warning", "You can not use both functions at the same time");
                cbIgnrLeftPck .Checked = false;
                cbIgnrRightPck.Checked = false;
                return;
            }

            if (cbSkipFrnt.Checked) DM.ARAY[(int)ri.FRNT].SetStat(cs.None);
            if (cbSkipRear.Checked) DM.ARAY[(int)ri.REAR].SetStat(cs.None);

            if (cbIgnrLeftPck.Checked && !cbIgnrRightPck.Checked)
            {
                DM.ARAY[(int)ri.PICK].SetStat(0, 0, cs.None);
                DM.ARAY[(int)ri.PICK].SetStat(1, 0, cs.Empty);
                OM.CmnOptn.bUseMultiHldr = false;
                cbUseMultiHldr.Checked = false;
            }

            if (!cbIgnrLeftPck.Checked && cbIgnrRightPck.Checked)
            {
                DM.ARAY[(int)ri.PICK].SetStat(0, 0, cs.Empty);
                DM.ARAY[(int)ri.PICK].SetStat(1, 0, cs.None);
                OM.CmnOptn.bUseMultiHldr = false;
                cbUseMultiHldr.Checked = false;
            }

            if (!cbIgnrLeftPck.Checked && !cbIgnrRightPck.Checked)
            {
                DM.ARAY[(int)ri.PICK].SetStat(0, 0, cs.Empty);
                DM.ARAY[(int)ri.PICK].SetStat(1, 0, cs.Empty);
            }

            if (Log.ShowMessageModal("Confirm", "Are you Sure?") != DialogResult.Yes) return;

            
            UpdateComOptn(false);
            OM.SaveCmnOptn();

        } 
        
        public void UpdateComOptn(bool _bToTable)
        {                                                                                   
            if (_bToTable == true) 
            {
                tbVisnBfDelay   .Text    = OM.CmnOptn.iVisnBfDelay  .ToString();
                //cbUnlock        .Checked = OM.CmnOptn.bUnlock                  ;
                cbSkipFrnt      .Checked = OM.CmnOptn.bSkipFrnt                ;
                cbSkipRear      .Checked = OM.CmnOptn.bSkipRear                ;
                cbUseMultiHldr  .Checked = OM.CmnOptn.bUseMultiHldr            ;
                cbIgnrLeftPck   .Checked = OM.CmnOptn.bIgnrLeftPck             ;
                cbIgnrRightPck  .Checked = OM.CmnOptn.bIgnrRightPck            ;
                cbTorqChck      .Checked = OM.CmnOptn.bTorqChck                ;
                //cb1PntRpt       .Checked = OM.CmnOptn.b1PntRpt                 ;
                //토크 컨버팅하는 옵션
                tbSetMotrTorq1  .Text    = OM.CmnOptn.dSetMotrTorq1.ToString() ;
                tbSetMotrTorq2  .Text    = OM.CmnOptn.dSetMotrTorq2.ToString() ;
                tbGaugeTorq1    .Text    = OM.CmnOptn.dGaugeTorq1  .ToString() ;
                tbGaugeTorq2    .Text    = OM.CmnOptn.dGaugeTorq2  .ToString() ;



                //Ver1.0.1.0
                //렌즈 체결 후 비젼 인스펙션 사용 옵션 처리
                cbUseAtInsp.Checked = OM.CmnOptn.bUseAtInsp;

                cbPickVisnRetryCnt .SelectedIndex = OM.CmnOptn.iPickVisnRetryCnt ;
                cbPickRetryCnt     .SelectedIndex = OM.CmnOptn.iPickRetryCnt     ;
                cbPlaceVisnRetryCnt.SelectedIndex = OM.CmnOptn.iPlaceVisnRetryCnt;
                cbPlaceRetryCnt    .SelectedIndex = OM.CmnOptn.iPlaceRetryCnt    ;


            }
            else 
            {
                OM.CmnOptn.iVisnBfDelay    = CConfig.StrToIntDef(tbVisnBfDelay  .Text, 0  );
                //OM.CmnOptn.bUnlock         = cbUnlock      .Checked ;
                OM.CmnOptn.bSkipFrnt       = cbSkipFrnt     .Checked ;
                OM.CmnOptn.bSkipRear       = cbSkipRear     .Checked ;
                OM.CmnOptn.bUseMultiHldr   = cbUseMultiHldr .Checked ;
                OM.CmnOptn.bIgnrLeftPck    = cbIgnrLeftPck  .Checked ;
                OM.CmnOptn.bIgnrRightPck   = cbIgnrRightPck .Checked ;
                OM.CmnOptn.bTorqChck       = cbTorqChck     .Checked ;
                //OM.CmnOptn.b1PntRpt        = cb1PntRpt      .Checked ;
                //토크 컨버팅하는 옵션
                OM.CmnOptn.dSetMotrTorq1 = CConfig.StrToDoubleDef(tbSetMotrTorq1.Text, 0.0);
                OM.CmnOptn.dSetMotrTorq2 = CConfig.StrToDoubleDef(tbSetMotrTorq2.Text, 0.0);
                OM.CmnOptn.dGaugeTorq1   = CConfig.StrToDoubleDef(tbGaugeTorq1  .Text, 0.0);
                OM.CmnOptn.dGaugeTorq2   = CConfig.StrToDoubleDef(tbGaugeTorq2  .Text, 0.0);

                
                //Ver1.0.1.0
                //렌즈 체결 후 비젼 인스펙션 사용 옵션 처리
                OM.CmnOptn.bUseAtInsp = cbUseAtInsp.Checked;
                    
                    
                OM.CmnOptn.iPickVisnRetryCnt  = cbPickVisnRetryCnt .SelectedIndex ;
                OM.CmnOptn.iPickRetryCnt      = cbPickRetryCnt     .SelectedIndex ;
                OM.CmnOptn.iPlaceVisnRetryCnt = cbPlaceVisnRetryCnt.SelectedIndex ;
                OM.CmnOptn.iPlaceRetryCnt     = cbPlaceRetryCnt    .SelectedIndex ;


                //COptnMan.CmnOptn.iTipCleanDelay = CConfig.StrToIntDef(tbTipCleanDelay.Text, COptnMan.CmnOptn.iTipCleanDelay);

                UpdateComOptn(true);
            }
        }

        

        private void FormOption_VisibleChanged(object sender, EventArgs e)
        {
            tmUpdate.Enabled = Visible;
        }

        public bool bUpdate = false;
        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            tmUpdate.Enabled = false;

            //UpdateLookUpTable();

            tmUpdate.Enabled = true;
        }
    }
}
