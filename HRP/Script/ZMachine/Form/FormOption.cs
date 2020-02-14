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
            SetDisp(ri.RLT1);SetDisp(ri.RLT2);SetDisp(ri.RLT3);
            SetDisp(ri.WRK1);SetDisp(ri.WRK2);SetDisp(ri.WRK3);
            SetDisp(ri.PSTB);
            SetDisp(ri.SPC );
            
            ((Button)sender).Enabled = true;
        } 
        
        private void SetDisp(int _ri)
        {
            DM.ARAY[_ri].SetDisp(cs.Rslt0, OM.CmnOptn.sRsltName0, Color.FromArgb(OM.CmnOptn.iRsltColor0)); 
            DM.ARAY[_ri].SetDisp(cs.Rslt1, OM.CmnOptn.sRsltName1, Color.FromArgb(OM.CmnOptn.iRsltColor1)); 
            DM.ARAY[_ri].SetDisp(cs.Rslt2, OM.CmnOptn.sRsltName2, Color.FromArgb(OM.CmnOptn.iRsltColor2)); 
            DM.ARAY[_ri].SetDisp(cs.Rslt3, OM.CmnOptn.sRsltName3, Color.FromArgb(OM.CmnOptn.iRsltColor3)); 
            DM.ARAY[_ri].SetDisp(cs.Rslt4, OM.CmnOptn.sRsltName4, Color.FromArgb(OM.CmnOptn.iRsltColor4)); 
            DM.ARAY[_ri].SetDisp(cs.Rslt5, OM.CmnOptn.sRsltName5, Color.FromArgb(OM.CmnOptn.iRsltColor5)); 
            DM.ARAY[_ri].SetDisp(cs.Rslt6, OM.CmnOptn.sRsltName6, Color.FromArgb(OM.CmnOptn.iRsltColor6)); 
            DM.ARAY[_ri].SetDisp(cs.Rslt7, OM.CmnOptn.sRsltName7, Color.FromArgb(OM.CmnOptn.iRsltColor7)); 
            DM.ARAY[_ri].SetDisp(cs.Rslt8, OM.CmnOptn.sRsltName8, Color.FromArgb(OM.CmnOptn.iRsltColor8)); 
            DM.ARAY[_ri].SetDisp(cs.Rslt9, OM.CmnOptn.sRsltName9, Color.FromArgb(OM.CmnOptn.iRsltColor9)); 
            DM.ARAY[_ri].SetDisp(cs.RsltA, OM.CmnOptn.sRsltNameA, Color.FromArgb(OM.CmnOptn.iRsltColorA)); 
            DM.ARAY[_ri].SetDisp(cs.RsltB, OM.CmnOptn.sRsltNameB, Color.FromArgb(OM.CmnOptn.iRsltColorB)); 
            DM.ARAY[_ri].SetDisp(cs.RsltC, OM.CmnOptn.sRsltNameC, Color.FromArgb(OM.CmnOptn.iRsltColorC)); 
            DM.ARAY[_ri].SetDisp(cs.RsltD, OM.CmnOptn.sRsltNameD, Color.FromArgb(OM.CmnOptn.iRsltColorD)); 
            DM.ARAY[_ri].SetDisp(cs.RsltE, OM.CmnOptn.sRsltNameE, Color.FromArgb(OM.CmnOptn.iRsltColorE)); 
            DM.ARAY[_ri].SetDisp(cs.RsltF, OM.CmnOptn.sRsltNameF, Color.FromArgb(OM.CmnOptn.iRsltColorF)); 
            DM.ARAY[_ri].SetDisp(cs.RsltG, OM.CmnOptn.sRsltNameG, Color.FromArgb(OM.CmnOptn.iRsltColorG)); 
            DM.ARAY[_ri].SetDisp(cs.RsltH, OM.CmnOptn.sRsltNameH, Color.FromArgb(OM.CmnOptn.iRsltColorH)); 
            DM.ARAY[_ri].SetDisp(cs.RsltI, OM.CmnOptn.sRsltNameI, Color.FromArgb(OM.CmnOptn.iRsltColorI)); 
            DM.ARAY[_ri].SetDisp(cs.RsltJ, OM.CmnOptn.sRsltNameJ, Color.FromArgb(OM.CmnOptn.iRsltColorJ)); 
            DM.ARAY[_ri].SetDisp(cs.RsltK, OM.CmnOptn.sRsltNameK, Color.FromArgb(OM.CmnOptn.iRsltColorK)); 
            DM.ARAY[_ri].SetDisp(cs.RsltL, OM.CmnOptn.sRsltNameL, Color.FromArgb(OM.CmnOptn.iRsltColorL)); 
            
        }
                    
        public void UpdateComOptn(bool _bToTable)
        {                                                                                   
            if (_bToTable == true) 
            {
                CConfig.ValToCon(cbLoadingStop , ref OM.CmnOptn.bLoadStop  );
                CConfig.ValToCon(cbIgnrDoor    , ref OM.CmnOptn.bIgnrDoor  );
                CConfig.ValToCon(cbIgnrVisn    , ref OM.CmnOptn.bIgnrVisn  );
                CConfig.ValToCon(cbIgnrMark    , ref OM.CmnOptn.bIgnrMark  );
                CConfig.ValToCon(tbTrgOfs      , ref OM.CmnOptn.dTrgOfs    );

                CConfig.ValToCon(tbName0       , ref OM.CmnOptn.sRsltName0 ); CConfig.ValToCon(tbLevel0 , ref OM.CmnOptn.iRsltLevel0 ); CConfig.ValToCon(tbLim0 , ref OM.CmnOptn.iVsLim0 ); CConfig.ValToCon(cbNotMark0 , ref OM.CmnOptn.bNotMark0 );
                CConfig.ValToCon(tbName1       , ref OM.CmnOptn.sRsltName1 ); CConfig.ValToCon(tbLevel1 , ref OM.CmnOptn.iRsltLevel1 ); CConfig.ValToCon(tbLim1 , ref OM.CmnOptn.iVsLim1 ); CConfig.ValToCon(cbNotMark1 , ref OM.CmnOptn.bNotMark1 );
                CConfig.ValToCon(tbName2       , ref OM.CmnOptn.sRsltName2 ); CConfig.ValToCon(tbLevel2 , ref OM.CmnOptn.iRsltLevel2 ); CConfig.ValToCon(tbLim2 , ref OM.CmnOptn.iVsLim2 ); CConfig.ValToCon(cbNotMark2 , ref OM.CmnOptn.bNotMark2 );
                CConfig.ValToCon(tbName3       , ref OM.CmnOptn.sRsltName3 ); CConfig.ValToCon(tbLevel3 , ref OM.CmnOptn.iRsltLevel3 ); CConfig.ValToCon(tbLim3 , ref OM.CmnOptn.iVsLim3 ); CConfig.ValToCon(cbNotMark3 , ref OM.CmnOptn.bNotMark3 );
                CConfig.ValToCon(tbName4       , ref OM.CmnOptn.sRsltName4 ); CConfig.ValToCon(tbLevel4 , ref OM.CmnOptn.iRsltLevel4 ); CConfig.ValToCon(tbLim4 , ref OM.CmnOptn.iVsLim4 ); CConfig.ValToCon(cbNotMark4 , ref OM.CmnOptn.bNotMark4 );
                CConfig.ValToCon(tbName5       , ref OM.CmnOptn.sRsltName5 ); CConfig.ValToCon(tbLevel5 , ref OM.CmnOptn.iRsltLevel5 ); CConfig.ValToCon(tbLim5 , ref OM.CmnOptn.iVsLim5 ); CConfig.ValToCon(cbNotMark5 , ref OM.CmnOptn.bNotMark5 );
                CConfig.ValToCon(tbName6       , ref OM.CmnOptn.sRsltName6 ); CConfig.ValToCon(tbLevel6 , ref OM.CmnOptn.iRsltLevel6 ); CConfig.ValToCon(tbLim6 , ref OM.CmnOptn.iVsLim6 ); CConfig.ValToCon(cbNotMark6 , ref OM.CmnOptn.bNotMark6 );
                CConfig.ValToCon(tbName7       , ref OM.CmnOptn.sRsltName7 ); CConfig.ValToCon(tbLevel7 , ref OM.CmnOptn.iRsltLevel7 ); CConfig.ValToCon(tbLim7 , ref OM.CmnOptn.iVsLim7 ); CConfig.ValToCon(cbNotMark7 , ref OM.CmnOptn.bNotMark7 );
                CConfig.ValToCon(tbName8       , ref OM.CmnOptn.sRsltName8 ); CConfig.ValToCon(tbLevel8 , ref OM.CmnOptn.iRsltLevel8 ); CConfig.ValToCon(tbLim8 , ref OM.CmnOptn.iVsLim8 ); CConfig.ValToCon(cbNotMark8 , ref OM.CmnOptn.bNotMark8 );
                CConfig.ValToCon(tbName9       , ref OM.CmnOptn.sRsltName9 ); CConfig.ValToCon(tbLevel9 , ref OM.CmnOptn.iRsltLevel9 ); CConfig.ValToCon(tbLim9 , ref OM.CmnOptn.iVsLim9 ); CConfig.ValToCon(cbNotMark9 , ref OM.CmnOptn.bNotMark9 );
                CConfig.ValToCon(tbNameA       , ref OM.CmnOptn.sRsltNameA ); CConfig.ValToCon(tbLevelA , ref OM.CmnOptn.iRsltLevelA ); CConfig.ValToCon(tbLimA , ref OM.CmnOptn.iVsLimA ); CConfig.ValToCon(cbNotMarkA , ref OM.CmnOptn.bNotMarkA );
                CConfig.ValToCon(tbNameB       , ref OM.CmnOptn.sRsltNameB ); CConfig.ValToCon(tbLevelB , ref OM.CmnOptn.iRsltLevelB ); CConfig.ValToCon(tbLimB , ref OM.CmnOptn.iVsLimB ); CConfig.ValToCon(cbNotMarkB , ref OM.CmnOptn.bNotMarkB );
                CConfig.ValToCon(tbNameC       , ref OM.CmnOptn.sRsltNameC ); CConfig.ValToCon(tbLevelC , ref OM.CmnOptn.iRsltLevelC ); CConfig.ValToCon(tbLimC , ref OM.CmnOptn.iVsLimC ); CConfig.ValToCon(cbNotMarkC , ref OM.CmnOptn.bNotMarkC );
                CConfig.ValToCon(tbNameD       , ref OM.CmnOptn.sRsltNameD ); CConfig.ValToCon(tbLevelD , ref OM.CmnOptn.iRsltLevelD ); CConfig.ValToCon(tbLimD , ref OM.CmnOptn.iVsLimD ); CConfig.ValToCon(cbNotMarkD , ref OM.CmnOptn.bNotMarkD );
                CConfig.ValToCon(tbNameE       , ref OM.CmnOptn.sRsltNameE ); CConfig.ValToCon(tbLevelE , ref OM.CmnOptn.iRsltLevelE ); CConfig.ValToCon(tbLimE , ref OM.CmnOptn.iVsLimE ); CConfig.ValToCon(cbNotMarkE , ref OM.CmnOptn.bNotMarkE );
                CConfig.ValToCon(tbNameF       , ref OM.CmnOptn.sRsltNameF ); CConfig.ValToCon(tbLevelF , ref OM.CmnOptn.iRsltLevelF ); CConfig.ValToCon(tbLimF , ref OM.CmnOptn.iVsLimF ); CConfig.ValToCon(cbNotMarkF , ref OM.CmnOptn.bNotMarkF );
                CConfig.ValToCon(tbNameG       , ref OM.CmnOptn.sRsltNameG ); CConfig.ValToCon(tbLevelG , ref OM.CmnOptn.iRsltLevelG ); CConfig.ValToCon(tbLimG , ref OM.CmnOptn.iVsLimG ); CConfig.ValToCon(cbNotMarkG , ref OM.CmnOptn.bNotMarkG );
                CConfig.ValToCon(tbNameH       , ref OM.CmnOptn.sRsltNameH ); CConfig.ValToCon(tbLevelH , ref OM.CmnOptn.iRsltLevelH ); CConfig.ValToCon(tbLimH , ref OM.CmnOptn.iVsLimH ); CConfig.ValToCon(cbNotMarkH , ref OM.CmnOptn.bNotMarkH );
                CConfig.ValToCon(tbNameI       , ref OM.CmnOptn.sRsltNameI ); CConfig.ValToCon(tbLevelI , ref OM.CmnOptn.iRsltLevelI ); CConfig.ValToCon(tbLimI , ref OM.CmnOptn.iVsLimI ); CConfig.ValToCon(cbNotMarkI , ref OM.CmnOptn.bNotMarkI );
                CConfig.ValToCon(tbNameJ       , ref OM.CmnOptn.sRsltNameJ ); CConfig.ValToCon(tbLevelJ , ref OM.CmnOptn.iRsltLevelJ ); CConfig.ValToCon(tbLimJ , ref OM.CmnOptn.iVsLimJ ); CConfig.ValToCon(cbNotMarkJ , ref OM.CmnOptn.bNotMarkJ );
                CConfig.ValToCon(tbNameK       , ref OM.CmnOptn.sRsltNameK ); CConfig.ValToCon(tbLevelK , ref OM.CmnOptn.iRsltLevelK ); CConfig.ValToCon(tbLimK , ref OM.CmnOptn.iVsLimK ); CConfig.ValToCon(cbNotMarkK , ref OM.CmnOptn.bNotMarkK );
                CConfig.ValToCon(tbNameL       , ref OM.CmnOptn.sRsltNameL ); CConfig.ValToCon(tbLevelL , ref OM.CmnOptn.iRsltLevelL ); CConfig.ValToCon(tbLimL , ref OM.CmnOptn.iVsLimL ); CConfig.ValToCon(cbNotMarkL , ref OM.CmnOptn.bNotMarkL );
                CConfig.ValToCon(tbLimT        , ref OM.CmnOptn.iVsLimT    ); 

                pnColor0.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColor0);
                pnColor1.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColor1);
                pnColor2.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColor2);
                pnColor3.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColor3);
                pnColor4.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColor4);
                pnColor5.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColor5);
                pnColor6.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColor6);
                pnColor7.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColor7);
                pnColor8.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColor8);
                pnColor9.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColor9);
                pnColorA.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorA);
                pnColorB.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorB);
                pnColorC.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorC);
                pnColorD.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorD);
                pnColorE.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorE);
                pnColorF.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorF);
                pnColorG.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorG);
                pnColorH.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorH);
                pnColorI.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorI);
                pnColorJ.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorJ);
                pnColorK.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorK);
                pnColorL.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorL);
            }
            else 
            {
                OM.CCmnOptn PreCmnOptn = OM.CmnOptn;
                CConfig.ConToVal(cbLoadingStop , ref OM.CmnOptn.bLoadStop  );
                CConfig.ConToVal(cbIgnrDoor    , ref OM.CmnOptn.bIgnrDoor  );
                CConfig.ConToVal(cbIgnrVisn    , ref OM.CmnOptn.bIgnrVisn  );
                CConfig.ConToVal(cbIgnrMark    , ref OM.CmnOptn.bIgnrMark  );
                CConfig.ConToVal(tbTrgOfs      , ref OM.CmnOptn.dTrgOfs    );

                CConfig.ConToVal(tbName0       , ref OM.CmnOptn.sRsltName0 ); CConfig.ConToVal(tbLevel0 , ref OM.CmnOptn.iRsltLevel0 ); CConfig.ConToVal(tbLim0 , ref OM.CmnOptn.iVsLim0 ); CConfig.ConToVal(cbNotMark0 , ref OM.CmnOptn.bNotMark0 );
                CConfig.ConToVal(tbName1       , ref OM.CmnOptn.sRsltName1 ); CConfig.ConToVal(tbLevel1 , ref OM.CmnOptn.iRsltLevel1 ); CConfig.ConToVal(tbLim1 , ref OM.CmnOptn.iVsLim1 ); CConfig.ConToVal(cbNotMark1 , ref OM.CmnOptn.bNotMark1 );
                CConfig.ConToVal(tbName2       , ref OM.CmnOptn.sRsltName2 ); CConfig.ConToVal(tbLevel2 , ref OM.CmnOptn.iRsltLevel2 ); CConfig.ConToVal(tbLim2 , ref OM.CmnOptn.iVsLim2 ); CConfig.ConToVal(cbNotMark2 , ref OM.CmnOptn.bNotMark2 );
                CConfig.ConToVal(tbName3       , ref OM.CmnOptn.sRsltName3 ); CConfig.ConToVal(tbLevel3 , ref OM.CmnOptn.iRsltLevel3 ); CConfig.ConToVal(tbLim3 , ref OM.CmnOptn.iVsLim3 ); CConfig.ConToVal(cbNotMark3 , ref OM.CmnOptn.bNotMark3 );
                CConfig.ConToVal(tbName4       , ref OM.CmnOptn.sRsltName4 ); CConfig.ConToVal(tbLevel4 , ref OM.CmnOptn.iRsltLevel4 ); CConfig.ConToVal(tbLim4 , ref OM.CmnOptn.iVsLim4 ); CConfig.ConToVal(cbNotMark4 , ref OM.CmnOptn.bNotMark4 );
                CConfig.ConToVal(tbName5       , ref OM.CmnOptn.sRsltName5 ); CConfig.ConToVal(tbLevel5 , ref OM.CmnOptn.iRsltLevel5 ); CConfig.ConToVal(tbLim5 , ref OM.CmnOptn.iVsLim5 ); CConfig.ConToVal(cbNotMark5 , ref OM.CmnOptn.bNotMark5 );
                CConfig.ConToVal(tbName6       , ref OM.CmnOptn.sRsltName6 ); CConfig.ConToVal(tbLevel6 , ref OM.CmnOptn.iRsltLevel6 ); CConfig.ConToVal(tbLim6 , ref OM.CmnOptn.iVsLim6 ); CConfig.ConToVal(cbNotMark6 , ref OM.CmnOptn.bNotMark6 );
                CConfig.ConToVal(tbName7       , ref OM.CmnOptn.sRsltName7 ); CConfig.ConToVal(tbLevel7 , ref OM.CmnOptn.iRsltLevel7 ); CConfig.ConToVal(tbLim7 , ref OM.CmnOptn.iVsLim7 ); CConfig.ConToVal(cbNotMark7 , ref OM.CmnOptn.bNotMark7 );
                CConfig.ConToVal(tbName8       , ref OM.CmnOptn.sRsltName8 ); CConfig.ConToVal(tbLevel8 , ref OM.CmnOptn.iRsltLevel8 ); CConfig.ConToVal(tbLim8 , ref OM.CmnOptn.iVsLim8 ); CConfig.ConToVal(cbNotMark8 , ref OM.CmnOptn.bNotMark8 );
                CConfig.ConToVal(tbName9       , ref OM.CmnOptn.sRsltName9 ); CConfig.ConToVal(tbLevel9 , ref OM.CmnOptn.iRsltLevel9 ); CConfig.ConToVal(tbLim9 , ref OM.CmnOptn.iVsLim9 ); CConfig.ConToVal(cbNotMark9 , ref OM.CmnOptn.bNotMark9 );
                CConfig.ConToVal(tbNameA       , ref OM.CmnOptn.sRsltNameA ); CConfig.ConToVal(tbLevelA , ref OM.CmnOptn.iRsltLevelA ); CConfig.ConToVal(tbLimA , ref OM.CmnOptn.iVsLimA ); CConfig.ConToVal(cbNotMarkA , ref OM.CmnOptn.bNotMarkA );
                CConfig.ConToVal(tbNameB       , ref OM.CmnOptn.sRsltNameB ); CConfig.ConToVal(tbLevelB , ref OM.CmnOptn.iRsltLevelB ); CConfig.ConToVal(tbLimB , ref OM.CmnOptn.iVsLimB ); CConfig.ConToVal(cbNotMarkB , ref OM.CmnOptn.bNotMarkB );
                CConfig.ConToVal(tbNameC       , ref OM.CmnOptn.sRsltNameC ); CConfig.ConToVal(tbLevelC , ref OM.CmnOptn.iRsltLevelC ); CConfig.ConToVal(tbLimC , ref OM.CmnOptn.iVsLimC ); CConfig.ConToVal(cbNotMarkC , ref OM.CmnOptn.bNotMarkC );
                CConfig.ConToVal(tbNameD       , ref OM.CmnOptn.sRsltNameD ); CConfig.ConToVal(tbLevelD , ref OM.CmnOptn.iRsltLevelD ); CConfig.ConToVal(tbLimD , ref OM.CmnOptn.iVsLimD ); CConfig.ConToVal(cbNotMarkD , ref OM.CmnOptn.bNotMarkD );
                CConfig.ConToVal(tbNameE       , ref OM.CmnOptn.sRsltNameE ); CConfig.ConToVal(tbLevelE , ref OM.CmnOptn.iRsltLevelE ); CConfig.ConToVal(tbLimE , ref OM.CmnOptn.iVsLimE ); CConfig.ConToVal(cbNotMarkE , ref OM.CmnOptn.bNotMarkE );
                CConfig.ConToVal(tbNameF       , ref OM.CmnOptn.sRsltNameF ); CConfig.ConToVal(tbLevelF , ref OM.CmnOptn.iRsltLevelF ); CConfig.ConToVal(tbLimF , ref OM.CmnOptn.iVsLimF ); CConfig.ConToVal(cbNotMarkF , ref OM.CmnOptn.bNotMarkF );
                CConfig.ConToVal(tbNameG       , ref OM.CmnOptn.sRsltNameG ); CConfig.ConToVal(tbLevelG , ref OM.CmnOptn.iRsltLevelG ); CConfig.ConToVal(tbLimG , ref OM.CmnOptn.iVsLimG ); CConfig.ConToVal(cbNotMarkG , ref OM.CmnOptn.bNotMarkG );
                CConfig.ConToVal(tbNameH       , ref OM.CmnOptn.sRsltNameH ); CConfig.ConToVal(tbLevelH , ref OM.CmnOptn.iRsltLevelH ); CConfig.ConToVal(tbLimH , ref OM.CmnOptn.iVsLimH ); CConfig.ConToVal(cbNotMarkH , ref OM.CmnOptn.bNotMarkH );
                CConfig.ConToVal(tbNameI       , ref OM.CmnOptn.sRsltNameI ); CConfig.ConToVal(tbLevelI , ref OM.CmnOptn.iRsltLevelI ); CConfig.ConToVal(tbLimI , ref OM.CmnOptn.iVsLimI ); CConfig.ConToVal(cbNotMarkI , ref OM.CmnOptn.bNotMarkI );
                CConfig.ConToVal(tbNameJ       , ref OM.CmnOptn.sRsltNameJ ); CConfig.ConToVal(tbLevelJ , ref OM.CmnOptn.iRsltLevelJ ); CConfig.ConToVal(tbLimJ , ref OM.CmnOptn.iVsLimJ ); CConfig.ConToVal(cbNotMarkJ , ref OM.CmnOptn.bNotMarkJ );
                CConfig.ConToVal(tbNameK       , ref OM.CmnOptn.sRsltNameK ); CConfig.ConToVal(tbLevelK , ref OM.CmnOptn.iRsltLevelK ); CConfig.ConToVal(tbLimK , ref OM.CmnOptn.iVsLimK ); CConfig.ConToVal(cbNotMarkK , ref OM.CmnOptn.bNotMarkK );
                CConfig.ConToVal(tbNameL       , ref OM.CmnOptn.sRsltNameL ); CConfig.ConToVal(tbLevelL , ref OM.CmnOptn.iRsltLevelL ); CConfig.ConToVal(tbLimL , ref OM.CmnOptn.iVsLimL ); CConfig.ConToVal(cbNotMarkL , ref OM.CmnOptn.bNotMarkL );
                CConfig.ConToVal(tbLimT        , ref OM.CmnOptn.iVsLimT    ); 

                OM.CmnOptn.iRsltColor0 = pnColor0.BackColor.ToArgb(); 
                OM.CmnOptn.iRsltColor1 = pnColor1.BackColor.ToArgb(); 
                OM.CmnOptn.iRsltColor2 = pnColor2.BackColor.ToArgb(); 
                OM.CmnOptn.iRsltColor3 = pnColor3.BackColor.ToArgb(); 
                OM.CmnOptn.iRsltColor4 = pnColor4.BackColor.ToArgb(); 
                OM.CmnOptn.iRsltColor5 = pnColor5.BackColor.ToArgb(); 
                OM.CmnOptn.iRsltColor6 = pnColor6.BackColor.ToArgb(); 
                OM.CmnOptn.iRsltColor7 = pnColor7.BackColor.ToArgb(); 
                OM.CmnOptn.iRsltColor8 = pnColor8.BackColor.ToArgb(); 
                OM.CmnOptn.iRsltColor9 = pnColor9.BackColor.ToArgb(); 
                OM.CmnOptn.iRsltColorA = pnColorA.BackColor.ToArgb(); 
                OM.CmnOptn.iRsltColorB = pnColorB.BackColor.ToArgb(); 
                OM.CmnOptn.iRsltColorC = pnColorC.BackColor.ToArgb(); 
                OM.CmnOptn.iRsltColorD = pnColorD.BackColor.ToArgb(); 
                OM.CmnOptn.iRsltColorE = pnColorE.BackColor.ToArgb(); 
                OM.CmnOptn.iRsltColorF = pnColorF.BackColor.ToArgb(); 
                OM.CmnOptn.iRsltColorG = pnColorG.BackColor.ToArgb(); 
                OM.CmnOptn.iRsltColorH = pnColorH.BackColor.ToArgb(); 
                OM.CmnOptn.iRsltColorI = pnColorI.BackColor.ToArgb(); 
                OM.CmnOptn.iRsltColorJ = pnColorJ.BackColor.ToArgb(); 
                OM.CmnOptn.iRsltColorK = pnColorK.BackColor.ToArgb(); 
                OM.CmnOptn.iRsltColorL = pnColorL.BackColor.ToArgb(); 


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
