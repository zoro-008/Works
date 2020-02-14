using System;
using System.Drawing;
using System.Windows.Forms;
using COMMON;
using SML;
using System.Reflection;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Configuration;
using Script;
using System.Collections.Generic;
//using VDll;

//TODO :: 
//LotInfoCnt , DayInfoCnt 갯수 지정해 주기 및 화면 컴파일후 배치 맞추기
namespace Machine
{
    public partial class FormOperation : Form
    {
        public static FormOperation FrmOperation;
        public static FormLotOpen FrmLotOpen;
        public static FormMain FrmMain;

        public int LotInfoCnt = 6;
        public int DayInfoCnt = 4;

        private const string sFormText = "Form Operation ";
        
        [DllImport("Kernel32.dll")]
        public static extern bool IsWow64Process(System.IntPtr hProcess, out bool lpSystemInfo);
        public FormOperation(Panel _pnBase)
        {
            InitializeComponent();

            this.TopLevel = false;
            this.Parent = _pnBase;

            //string sTemp = Script.Interpreter.AutoRun.ToString();

            DispLotInfo();
            DispDayList();

            MakeDoubleBuffered(pnLODR,true); MakeDoubleBuffered(pnLODR1, true);
            MakeDoubleBuffered(pnPREB,true);
            MakeDoubleBuffered(pnVSN1,true);MakeDoubleBuffered(pnWRK1,true);MakeDoubleBuffered(pnRST1,true);
            MakeDoubleBuffered(pnVSN2,true);MakeDoubleBuffered(pnWRK2,true);MakeDoubleBuffered(pnRST2,true);
            MakeDoubleBuffered(pnVSN3,true);MakeDoubleBuffered(pnWRK3,true);MakeDoubleBuffered(pnRST3,true);
            MakeDoubleBuffered(pnPSTB,true);
            MakeDoubleBuffered(pnULDR,true); MakeDoubleBuffered(pnULDR1, true);


            DM.ARAY[ri.LODR].SetParent(pnLODR1); DM.ARAY[ri.LODR].Name = "LODR";
            DM.ARAY[ri.PREB].SetParent(pnPREB); DM.ARAY[ri.PREB].Name = "PREB";

            DM.ARAY[ri.VSN1].SetParent(pnVSN1); DM.ARAY[ri.VSN1].Name = "VSN1";
            DM.ARAY[ri.VSN2].SetParent(pnVSN2); DM.ARAY[ri.VSN2].Name = "VSN2";
            DM.ARAY[ri.VSN3].SetParent(pnVSN3); DM.ARAY[ri.VSN3].Name = "VSN3";

            DM.ARAY[ri.WRK1].SetParent(pnRST1); DM.ARAY[ri.WRK1].Name = "WRK1";
            DM.ARAY[ri.WRK2].SetParent(pnRST2); DM.ARAY[ri.WRK2].Name = "WRK2";
            DM.ARAY[ri.WRK3].SetParent(pnRST3); DM.ARAY[ri.WRK3].Name = "WRK3";

            DM.ARAY[ri.RLT1].SetParent(pnWRK1); DM.ARAY[ri.RLT1].Name = "RST1";
            DM.ARAY[ri.RLT2].SetParent(pnWRK2); DM.ARAY[ri.RLT2].Name = "RST2";
            DM.ARAY[ri.RLT3].SetParent(pnWRK3); DM.ARAY[ri.RLT3].Name = "RST3";

            DM.ARAY[ri.PSTB].SetParent(pnPSTB ); DM.ARAY[ri.PSTB].Name = "PSTB";
            DM.ARAY[ri.ULDR].SetParent(pnULDR1); DM.ARAY[ri.ULDR].Name = "ULDR";

            DM.ARAY[ri.SPC ].Name = "SPC"; //SPC 저장용.

            //Loader
            DM.ARAY[ri.LODR].SetDisp(cs.Empty   , "Empty"   ,Color.Silver    );
            DM.ARAY[ri.LODR].SetDisp(cs.Unknown , "Unknown" ,Color.Aqua      );
            DM.ARAY[ri.LODR].SetDisp(cs.None    , "None"    ,Color.White     );
                                                                             
            //PreBuffer                                                      
            DM.ARAY[ri.PREB].SetDisp(cs.None    , "None"    ,Color.White     );
            DM.ARAY[ri.PREB].SetDisp(cs.Unknown , "Unknown" ,Color.Aqua      );
                                                                             
            //Vision1                                                        
            DM.ARAY[ri.VSN1].SetDisp(cs.None    , "None"    ,Color.White     );
            DM.ARAY[ri.VSN1].SetDisp(cs.Unknown , "Unknown" ,Color.Aqua      );
            DM.ARAY[ri.VSN1].SetDisp(cs.Work    , "Work"    ,Color.Blue      );
            DM.ARAY[ri.VSN1].SetDisp(cs.Wait    , "Wait"    ,Color.Yellow    );
                                                                             
            DM.ARAY[ri.WRK1].SetDisp(cs.None    , "None"    ,Color.White     );
            DM.ARAY[ri.WRK1].SetDisp(cs.Unknown , "Unknown" ,Color.Aqua      );
            DM.ARAY[ri.WRK1].SetDisp(cs.Good    , "Good"    ,Color.Lime      );
            //DM.ARAY[ri.WRK1].SetDisp(cs.ToBuf   , "ToBuff"  ,Color.Brown     );
            //DM.ARAY[ri.WRK1].SetDisp(cs.FromBuf , "FromBuff",Color.DarkOrange);
            SetDisp(ri.WRK1);    
            
            DM.ARAY[ri.RLT1].SetDisp(cs.None    , "None"    ,Color.White     );
            DM.ARAY[ri.RLT1].SetDisp(cs.Unknown , "Unknown" ,Color.Aqua      );
            DM.ARAY[ri.RLT1].SetDisp(cs.Good    , "Good"    ,Color.Lime      );
            //DM.ARAY[ri.RLT1].SetDisp(cs.ToBuf   , "ToBuff"  ,Color.Brown     );
            //DM.ARAY[ri.RLT1].SetDisp(cs.FromBuf , "FromBuff",Color.DarkOrange);
            SetDisp(ri.RLT1);
                                                                                                                                           
            //Vision2
            DM.ARAY[ri.VSN2].SetDisp(cs.None    , "None"    ,Color.White     );
            DM.ARAY[ri.VSN2].SetDisp(cs.Unknown , "Unknown" ,Color.Aqua      );
            DM.ARAY[ri.VSN2].SetDisp(cs.Work    , "Work"    ,Color.Blue      );
            DM.ARAY[ri.VSN2].SetDisp(cs.Wait    , "Wait"    ,Color.Yellow    );
                                                                             
            DM.ARAY[ri.WRK2].SetDisp(cs.None    , "None"    ,Color.White     );
            DM.ARAY[ri.WRK2].SetDisp(cs.Unknown , "Unknown" ,Color.Aqua      );
            DM.ARAY[ri.WRK2].SetDisp(cs.Good    , "Good"    ,Color.Lime      );
            //DM.ARAY[ri.WRK2].SetDisp(cs.ToBuf   , "ToBuff"  ,Color.Brown     );
            //DM.ARAY[ri.WRK2].SetDisp(cs.FromBuf , "FromBuff",Color.DarkOrange);
            SetDisp(ri.WRK2);                                              

            DM.ARAY[ri.RLT2].SetDisp(cs.None    , "None"    ,Color.White     );
            DM.ARAY[ri.RLT2].SetDisp(cs.Unknown , "Unknown" ,Color.Aqua      );
            DM.ARAY[ri.RLT2].SetDisp(cs.Good    , "Good"    ,Color.Lime      );
            //DM.ARAY[ri.RLT2].SetDisp(cs.ToBuf   , "ToBuff"  ,Color.Brown     );
            //DM.ARAY[ri.RLT2].SetDisp(cs.FromBuf , "FromBuff",Color.DarkOrange);
            SetDisp(ri.RLT2);
                                                                               
            //Vision3
            DM.ARAY[ri.VSN3].SetDisp(cs.None    , "None"    ,Color.White     );
            DM.ARAY[ri.VSN3].SetDisp(cs.Unknown , "Unknown" ,Color.Aqua      );
            DM.ARAY[ri.VSN3].SetDisp(cs.Work    , "Work"    ,Color.Blue      );
            DM.ARAY[ri.VSN3].SetDisp(cs.Wait    , "Wait"    ,Color.Yellow    );
                                                                                
            DM.ARAY[ri.WRK3].SetDisp(cs.None    , "None"    ,Color.White     );
            DM.ARAY[ri.WRK3].SetDisp(cs.Unknown , "Unknown" ,Color.Aqua      );
            DM.ARAY[ri.WRK3].SetDisp(cs.Good    , "Good"    ,Color.Lime      );
            //DM.ARAY[ri.WRK3].SetDisp(cs.ToBuf   , "ToBuff"  ,Color.Brown     );
            //DM.ARAY[ri.WRK3].SetDisp(cs.FromBuf , "FromBuff",Color.DarkOrange);
            SetDisp(ri.WRK3); 
            
            DM.ARAY[ri.RLT3].SetDisp(cs.None    , "None"    ,Color.White     );
            DM.ARAY[ri.RLT3].SetDisp(cs.Unknown , "Unknown" ,Color.Aqua      );
            DM.ARAY[ri.RLT3].SetDisp(cs.Good    , "Good"    ,Color.Lime      );
            //DM.ARAY[ri.RLT3].SetDisp(cs.ToBuf   , "ToBuff"  ,Color.Brown     );
            //DM.ARAY[ri.RLT3].SetDisp(cs.FromBuf , "FromBuff",Color.DarkOrange);
            SetDisp(ri.RLT3);
            
            //Post Buffer
            DM.ARAY[ri.PSTB].SetDisp(cs.None    , "None"    ,Color.White   );
            DM.ARAY[ri.PSTB].SetDisp(cs.Work    , "Work"    ,Color.Blue    );
            DM.ARAY[ri.PSTB].SetDisp(cs.Good    , "Good"    ,Color.Lime    );
            SetDisp(ri.PSTB);
                       
            //Unloader 
            DM.ARAY[ri.ULDR].SetDisp(cs.Empty   , "Empty"   ,Color.Silver  );
            DM.ARAY[ri.ULDR].SetDisp(cs.Work    , "Work"    ,Color.Blue    );
            DM.ARAY[ri.ULDR].SetDisp(cs.None    , "None"    ,Color.White   );
            
            DM.LoadMap();
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

        private void PanelRefresh()
        {
            pnLODR.Refresh(); pnLODR1.Refresh();
            pnPREB.Refresh();
            pnVSN1.Refresh();
            pnVSN2.Refresh();
            pnVSN3.Refresh();
            
            pnWRK1.Refresh();
            pnWRK2.Refresh();
            pnWRK3.Refresh();
            
            pnRST1.Refresh();
            pnRST2.Refresh();
            pnRST3.Refresh();
            
            pnPSTB.Refresh();
            pnULDR.Refresh(); pnULDR1.Refresh();

            for (int i = 0 ; i < ri.MAX_ARAY ; i++)
            {
                DM.ARAY[i].UpdateAray();
            }
        }

        private void btLotEnd_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            LOT.LotEnd();
        }

        private static bool bPreLotOpen = false;

        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            tmUpdate.Enabled = false;
            //로그인/로그아웃 방식
            if (SM.FrmLogOn.GetLevel() == (int)EN_LEVEL.LogOff)
            {
                btOperator.Text = "  LOG IN";

                pnManual .Enabled = false;
            }
            else
            {
                btOperator.Text = "  " + SM.FrmLogOn.GetLevel().ToString();

                pnManual .Enabled = true;
            }
                        
            //if (SML.FrmLogOn.GetLevel() != (int)EN_LEVEL.LogOff)
            //{
            //    btStart.Enabled = LOT.GetLotOpen();
            //}
            
            SPC.LOT.DispLotInfo(lvLotInfo);
            SPC.DAY.DispDayInfo(lvDayInfo);

            string Str      ;
            int iPreErrCnt  = 0;
            int iCrntErrCnt = 0;
            for (int i = 0 ; i < ML.ER_MaxCount() ; i++) 
            {
                if (ML.ER_GetErr((ei)i)) iCrntErrCnt++;
            }
            if (iPreErrCnt != iCrntErrCnt ) 
            {
                lbErr.Items.Clear();
                int iErrNo = ML.ER_GetLastErr();
                for (int i = 0; i < ML.ER_MaxCount(); i++) 
                {
                    if (ML.ER_GetErr((ei)i))
                    {
                        Str = string.Format("[ERR{0:000}]" , i) ;
                        Str += ML.ER_GetErrName(i) + " " + ML.ER_GetErrSubMsg((ei)i);
                        lbErr.Items.Add(Str);
                    }
                }
            }
            if (SEQ._iSeqStat != EN_SEQ_STAT.Error)
            {
                lbErr.Items.Clear();
            }
            iPreErrCnt = iCrntErrCnt ;

            if(!ML.MT_GetHomeDoneAll()){
                btAllHome.ForeColor = SEQ._bFlick ? Color.Black : Color.Red;
            }
            else {
                btAllHome.ForeColor = Color.Black  ;
            }

            if (LOT.GetLotOpen() && !SEQ._bRun)
            {
                btLotOpen.Text = "WORK ING";
                //btLotOpen.Enabled = true;
                btLotEnd .Enabled = true ;
            }
            else
            {
                btLotOpen.Text = "WORK STT";
                //btLotOpen.Enabled = true ;
                btLotEnd .Enabled = false;
            }

            //결과 색 이름 표기
            pnC0.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColor0); lbC0.Text = OM.CmnOptn.sRsltName0; lbCnt0.Text = OM.EqpStat.iRsltCnts[(int)cs.Rslt0].ToString();
            pnC1.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColor1); lbC1.Text = OM.CmnOptn.sRsltName1; lbCnt1.Text = OM.EqpStat.iRsltCnts[(int)cs.Rslt1].ToString();
            pnC2.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColor2); lbC2.Text = OM.CmnOptn.sRsltName2; lbCnt2.Text = OM.EqpStat.iRsltCnts[(int)cs.Rslt2].ToString();
            pnC3.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColor3); lbC3.Text = OM.CmnOptn.sRsltName3; lbCnt3.Text = OM.EqpStat.iRsltCnts[(int)cs.Rslt3].ToString();
            pnC4.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColor4); lbC4.Text = OM.CmnOptn.sRsltName4; lbCnt4.Text = OM.EqpStat.iRsltCnts[(int)cs.Rslt4].ToString();
            pnC5.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColor5); lbC5.Text = OM.CmnOptn.sRsltName5; lbCnt5.Text = OM.EqpStat.iRsltCnts[(int)cs.Rslt5].ToString();
            pnC6.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColor6); lbC6.Text = OM.CmnOptn.sRsltName6; lbCnt6.Text = OM.EqpStat.iRsltCnts[(int)cs.Rslt6].ToString();
            pnC7.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColor7); lbC7.Text = OM.CmnOptn.sRsltName7; lbCnt7.Text = OM.EqpStat.iRsltCnts[(int)cs.Rslt7].ToString();
            pnC8.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColor8); lbC8.Text = OM.CmnOptn.sRsltName8; lbCnt8.Text = OM.EqpStat.iRsltCnts[(int)cs.Rslt8].ToString();
            pnC9.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColor9); lbC9.Text = OM.CmnOptn.sRsltName9; lbCnt9.Text = OM.EqpStat.iRsltCnts[(int)cs.Rslt9].ToString();
            pnCA.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorA); lbCA.Text = OM.CmnOptn.sRsltNameA; lbCntA.Text = OM.EqpStat.iRsltCnts[(int)cs.RsltA].ToString();
            pnCB.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorB); lbCB.Text = OM.CmnOptn.sRsltNameB; lbCntB.Text = OM.EqpStat.iRsltCnts[(int)cs.RsltB].ToString();
            pnCC.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorC); lbCC.Text = OM.CmnOptn.sRsltNameC; lbCntC.Text = OM.EqpStat.iRsltCnts[(int)cs.RsltC].ToString();
            pnCD.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorD); lbCD.Text = OM.CmnOptn.sRsltNameD; lbCntD.Text = OM.EqpStat.iRsltCnts[(int)cs.RsltD].ToString();
            pnCE.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorE); lbCE.Text = OM.CmnOptn.sRsltNameE; lbCntE.Text = OM.EqpStat.iRsltCnts[(int)cs.RsltE].ToString();
            pnCF.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorF); lbCF.Text = OM.CmnOptn.sRsltNameF; lbCntF.Text = OM.EqpStat.iRsltCnts[(int)cs.RsltF].ToString();
            pnCG.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorG); lbCG.Text = OM.CmnOptn.sRsltNameG; lbCntG.Text = OM.EqpStat.iRsltCnts[(int)cs.RsltG].ToString();
            pnCH.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorH); lbCH.Text = OM.CmnOptn.sRsltNameH; lbCntH.Text = OM.EqpStat.iRsltCnts[(int)cs.RsltH].ToString();
            pnCI.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorI); lbCI.Text = OM.CmnOptn.sRsltNameI; lbCntI.Text = OM.EqpStat.iRsltCnts[(int)cs.RsltI].ToString(); 
            pnCJ.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorJ); lbCJ.Text = OM.CmnOptn.sRsltNameJ; lbCntJ.Text = OM.EqpStat.iRsltCnts[(int)cs.RsltJ].ToString();
            pnCK.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorK); lbCK.Text = OM.CmnOptn.sRsltNameK; lbCntK.Text = OM.EqpStat.iRsltCnts[(int)cs.RsltK].ToString();
            pnCL.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorL); lbCL.Text = OM.CmnOptn.sRsltNameL; lbCntL.Text = OM.EqpStat.iRsltCnts[(int)cs.RsltL].ToString();

              C0.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColor0);   T0.Text = OM.CmnOptn.sRsltName0;     N0.Text = OM.EqpStat.iPreRsltCnts[(int)cs.Rslt0].ToString();
              C1.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColor1);   T1.Text = OM.CmnOptn.sRsltName1;     N1.Text = OM.EqpStat.iPreRsltCnts[(int)cs.Rslt1].ToString();
              C2.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColor2);   T2.Text = OM.CmnOptn.sRsltName2;     N2.Text = OM.EqpStat.iPreRsltCnts[(int)cs.Rslt2].ToString();
              C3.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColor3);   T3.Text = OM.CmnOptn.sRsltName3;     N3.Text = OM.EqpStat.iPreRsltCnts[(int)cs.Rslt3].ToString();
              C4.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColor4);   T4.Text = OM.CmnOptn.sRsltName4;     N4.Text = OM.EqpStat.iPreRsltCnts[(int)cs.Rslt4].ToString();
              C5.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColor5);   T5.Text = OM.CmnOptn.sRsltName5;     N5.Text = OM.EqpStat.iPreRsltCnts[(int)cs.Rslt5].ToString();
              C6.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColor6);   T6.Text = OM.CmnOptn.sRsltName6;     N6.Text = OM.EqpStat.iPreRsltCnts[(int)cs.Rslt6].ToString();
              C7.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColor7);   T7.Text = OM.CmnOptn.sRsltName7;     N7.Text = OM.EqpStat.iPreRsltCnts[(int)cs.Rslt7].ToString();
              C8.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColor8);   T8.Text = OM.CmnOptn.sRsltName8;     N8.Text = OM.EqpStat.iPreRsltCnts[(int)cs.Rslt8].ToString();
              C9.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColor9);   T9.Text = OM.CmnOptn.sRsltName9;     N9.Text = OM.EqpStat.iPreRsltCnts[(int)cs.Rslt9].ToString();
              CA.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorA);   TA.Text = OM.CmnOptn.sRsltNameA;     NA.Text = OM.EqpStat.iPreRsltCnts[(int)cs.RsltA].ToString();
              CB.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorB);   TB.Text = OM.CmnOptn.sRsltNameB;     NB.Text = OM.EqpStat.iPreRsltCnts[(int)cs.RsltB].ToString();
              CC.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorC);   TC.Text = OM.CmnOptn.sRsltNameC;     NC.Text = OM.EqpStat.iPreRsltCnts[(int)cs.RsltC].ToString();
              CD.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorD);   TD.Text = OM.CmnOptn.sRsltNameD;     ND.Text = OM.EqpStat.iPreRsltCnts[(int)cs.RsltD].ToString();
              CE.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorE);   TE.Text = OM.CmnOptn.sRsltNameE;     NE.Text = OM.EqpStat.iPreRsltCnts[(int)cs.RsltE].ToString();
              CF.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorF);   TF.Text = OM.CmnOptn.sRsltNameF;     NF.Text = OM.EqpStat.iPreRsltCnts[(int)cs.RsltF].ToString();
              CG.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorG);   TG.Text = OM.CmnOptn.sRsltNameG;     NG.Text = OM.EqpStat.iPreRsltCnts[(int)cs.RsltG].ToString();
              CH.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorH);   TH.Text = OM.CmnOptn.sRsltNameH;     NH.Text = OM.EqpStat.iPreRsltCnts[(int)cs.RsltH].ToString();
              CI.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorI);   TI.Text = OM.CmnOptn.sRsltNameI;     NI.Text = OM.EqpStat.iPreRsltCnts[(int)cs.RsltI].ToString(); 
              CJ.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorJ);   TJ.Text = OM.CmnOptn.sRsltNameJ;     NJ.Text = OM.EqpStat.iPreRsltCnts[(int)cs.RsltJ].ToString();
              CK.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorK);   TK.Text = OM.CmnOptn.sRsltNameK;     NK.Text = OM.EqpStat.iPreRsltCnts[(int)cs.RsltK].ToString();
              CL.BackColor = Color.FromArgb(OM.CmnOptn.iRsltColorL);   TL.Text = OM.CmnOptn.sRsltNameL;     NL.Text = OM.EqpStat.iPreRsltCnts[(int)cs.RsltL].ToString();


            //인풋
            lbPREB_PkgInDetect.BackColor = ML.IO_GetX(xi.PREB_PkgInDetect) ? Color.Lime : Color.Gray ; lbPREB_PkgInDetect.Text = ML.IO_GetX(xi.PREB_PkgInDetect) ? "ON" : "OFF" ;
            lbPREB_StrpDetect .BackColor = ML.IO_GetX(xi.PREB_StrpDetect ) ? Color.Lime : Color.Gray ; lbPREB_StrpDetect .Text = ML.IO_GetX(xi.PREB_StrpDetect ) ? "ON" : "OFF" ;
            lbRAIL_Vsn1Detect .BackColor = ML.IO_GetX(xi.RAIL_Vsn1Detect ) ? Color.Lime : Color.Gray ; lbRAIL_Vsn1Detect .Text = ML.IO_GetX(xi.RAIL_Vsn1Detect ) ? "ON" : "OFF" ;
            lbRAIL_Vsn2Detect .BackColor = ML.IO_GetX(xi.RAIL_Vsn2Detect ) ? Color.Lime : Color.Gray ; lbRAIL_Vsn2Detect .Text = ML.IO_GetX(xi.RAIL_Vsn2Detect ) ? "ON" : "OFF" ;
            lbRAIL_Vsn3Detect .BackColor = ML.IO_GetX(xi.RAIL_Vsn3Detect ) ? Color.Lime : Color.Gray ; lbRAIL_Vsn3Detect .Text = ML.IO_GetX(xi.RAIL_Vsn3Detect ) ? "ON" : "OFF" ;
            lbPSTB_MarkDetect .BackColor = ML.IO_GetX(xi.PSTB_MarkDetect ) ? Color.Lime : Color.Gray ; lbPSTB_MarkDetect .Text = ML.IO_GetX(xi.PSTB_MarkDetect ) ? "ON" : "OFF" ;
            lbPSTB_PkgDetect1 .BackColor = ML.IO_GetX(xi.PSTB_PkgDetect1 ) ? Color.Lime : Color.Gray ; lbPSTB_PkgDetect1 .Text = ML.IO_GetX(xi.PSTB_PkgDetect1 ) ? "ON" : "OFF" ;
            lbPSTB_PkgDetect2 .BackColor = ML.IO_GetX(xi.PSTB_PkgDetect2 ) ? Color.Lime : Color.Gray ; lbPSTB_PkgDetect2 .Text = ML.IO_GetX(xi.PSTB_PkgDetect2 ) ? "ON" : "OFF" ;

            //실린더
            lbPREB_StprUpDn     .BackColor = ML.CL_GetCmd(ci.PREB_StprUpDn     ) == fb.Fwd ? Color.Aqua : Color.Yellow ; lbPREB_StprUpDn     .Text = ML.CL_GetCmd(ci.PREB_StprUpDn     ) == fb.Fwd ? "UP" : "DN" ;
            lbRAIL_Vsn1StprUpDn .BackColor = ML.CL_GetCmd(ci.RAIL_Vsn1StprUpDn ) == fb.Fwd ? Color.Aqua : Color.Yellow ; lbRAIL_Vsn1StprUpDn .Text = ML.CL_GetCmd(ci.RAIL_Vsn1StprUpDn ) == fb.Fwd ? "UP" : "DN" ;
            lbRAIL_Vsn2StprUpDn .BackColor = ML.CL_GetCmd(ci.RAIL_Vsn2StprUpDn ) == fb.Fwd ? Color.Aqua : Color.Yellow ; lbRAIL_Vsn2StprUpDn .Text = ML.CL_GetCmd(ci.RAIL_Vsn2StprUpDn ) == fb.Fwd ? "UP" : "DN" ;
            lbRAIL_Vsn3StprUpDn .BackColor = ML.CL_GetCmd(ci.RAIL_Vsn3StprUpDn ) == fb.Fwd ? Color.Aqua : Color.Yellow ; lbRAIL_Vsn3StprUpDn .Text = ML.CL_GetCmd(ci.RAIL_Vsn3StprUpDn ) == fb.Fwd ? "UP" : "DN" ;
            lbPSTB_MarkStprUpDn .BackColor = ML.CL_GetCmd(ci.PSTB_MarkStprUpDn ) == fb.Fwd ? Color.Aqua : Color.Yellow ; lbPSTB_MarkStprUpDn .Text = ML.CL_GetCmd(ci.PSTB_MarkStprUpDn ) == fb.Fwd ? "UP" : "DN" ;
                                                                                                                                                                                       
            lbPSTB_PusherFwBw   .BackColor = ML.CL_GetCmd(ci.PSTB_PusherFwBw   ) == fb.Fwd ? Color.Aqua : Color.Yellow ; lbPSTB_PusherFwBw   .Text = ML.CL_GetCmd(ci.PSTB_PusherFwBw   ) == fb.Fwd ? "FW" : "BW" ;

            lbRAIL_Vsn1AlignFwBw.BackColor = ML.CL_GetCmd(ci.RAIL_Vsn1AlignFwBw) == fb.Fwd ? Color.Aqua : Color.Yellow ; lbRAIL_Vsn1AlignFwBw.Text = ML.CL_GetCmd(ci.RAIL_Vsn1AlignFwBw) == fb.Fwd ? "FW" : "BW" ;
            lbRAIL_Vsn2AlignFwBw.BackColor = ML.CL_GetCmd(ci.RAIL_Vsn2AlignFwBw) == fb.Fwd ? Color.Aqua : Color.Yellow ; lbRAIL_Vsn2AlignFwBw.Text = ML.CL_GetCmd(ci.RAIL_Vsn2AlignFwBw) == fb.Fwd ? "FW" : "BW" ;
            lbRAIL_Vsn3AlignFwBw.BackColor = ML.CL_GetCmd(ci.RAIL_Vsn3AlignFwBw) == fb.Fwd ? Color.Aqua : Color.Yellow ; lbRAIL_Vsn3AlignFwBw.Text = ML.CL_GetCmd(ci.RAIL_Vsn3AlignFwBw) == fb.Fwd ? "FW" : "BW" ;
            lbPSTB_MarkAlignFWBw.BackColor = ML.CL_GetCmd(ci.PSTB_MarkAlignFWBw) == fb.Fwd ? Color.Aqua : Color.Yellow ; lbPSTB_MarkAlignFWBw.Text = ML.CL_GetCmd(ci.PSTB_MarkAlignFWBw) == fb.Fwd ? "FW" : "BW" ;

            lbRAIL_Vsn1SttnUpDn .BackColor = ML.CL_GetCmd(ci.RAIL_Vsn1SttnUpDn ) == fb.Fwd ? Color.Aqua : Color.Yellow ; lbRAIL_Vsn1SttnUpDn .Text = ML.CL_GetCmd(ci.RAIL_Vsn1SttnUpDn ) == fb.Fwd ? "UP" : "DN" ;
            lbRAIL_Vsn2SttnUpDn .BackColor = ML.CL_GetCmd(ci.RAIL_Vsn2SttnUpDn ) == fb.Fwd ? Color.Aqua : Color.Yellow ; lbRAIL_Vsn2SttnUpDn .Text = ML.CL_GetCmd(ci.RAIL_Vsn2SttnUpDn ) == fb.Fwd ? "UP" : "DN" ;
            lbRAIL_Vsn3SttnUpDn .BackColor = ML.CL_GetCmd(ci.RAIL_Vsn3SttnUpDn ) == fb.Fwd ? Color.Aqua : Color.Yellow ; lbRAIL_Vsn3SttnUpDn .Text = ML.CL_GetCmd(ci.RAIL_Vsn3SttnUpDn ) == fb.Fwd ? "UP" : "DN" ;
            lbPSTB_MarkSttnUpDn .BackColor = ML.CL_GetCmd(ci.PSTB_MarkSttnUpDn ) == fb.Fwd ? Color.Aqua : Color.Yellow ; lbPSTB_MarkSttnUpDn .Text = ML.CL_GetCmd(ci.PSTB_MarkSttnUpDn ) == fb.Fwd ? "UP" : "DN" ;

            //아웃풋.
            lbRAIL_FeedingAC1 .BackColor = ML.IO_GetY(yi.RAIL_FeedingAC1 ) ? Color.Lime : Color.Gray ; lbRAIL_FeedingAC1 .Text = ML.IO_GetY(yi.RAIL_FeedingAC1 ) ? "ON" : "OFF" ;
            lbRAIL_FeedingAC2 .BackColor = ML.IO_GetY(yi.RAIL_FeedingAC2 ) ? Color.Lime : Color.Gray ; lbRAIL_FeedingAC2 .Text = ML.IO_GetY(yi.RAIL_FeedingAC2 ) ? "ON" : "OFF" ;
            lbRAIL_FeedingAC3 .BackColor = ML.IO_GetY(yi.RAIL_FeedingAC3 ) ? Color.Lime : Color.Gray ; lbRAIL_FeedingAC3 .Text = ML.IO_GetY(yi.RAIL_FeedingAC3 ) ? "ON" : "OFF" ;
            lbMainAir         .BackColor = ML.IO_GetY(yi.ETC_MainAirOnOff) ? Color.Lime : Color.Gray ; lbMainAir         .Text = ML.IO_GetY(yi.ETC_MainAirOnOff) ? "ON" : "OFF" ;

            PanelRefresh();

            //Strip Check
            bool Strip1 = !DM.ARAY[ri.PREB].CheckAllStat(cs.None); bool Sensor1 = ML.IO_GetX(xi.PREB_StrpDetect);
            bool Strip2 = !DM.ARAY[ri.VSN1].CheckAllStat(cs.None); bool Sensor2 = ML.IO_GetX(xi.RAIL_Vsn1Detect);
            bool Strip3 = !DM.ARAY[ri.VSN2].CheckAllStat(cs.None); bool Sensor3 = ML.IO_GetX(xi.RAIL_Vsn2Detect);
            bool Strip4 = !DM.ARAY[ri.VSN3].CheckAllStat(cs.None); bool Sensor4 = ML.IO_GetX(xi.RAIL_Vsn3Detect);
            bool Strip5 = !DM.ARAY[ri.PSTB].CheckAllStat(cs.None); bool Sensor5 = ML.IO_GetX(xi.PSTB_MarkDetect) || ML.IO_GetX(xi.PSTB_PkgDetect1) || ML.IO_GetX(xi.PSTB_PkgDetect2);

            bool Lift1Dn = ML.CL_Complete(ci.RAIL_Vsn1SttnUpDn,fb.Bwd);
            bool Lift2Dn = ML.CL_Complete(ci.RAIL_Vsn2SttnUpDn,fb.Bwd);
            bool Lift3Dn = ML.CL_Complete(ci.RAIL_Vsn3SttnUpDn,fb.Bwd);
            bool Lift4Dn = ML.CL_Complete(ci.PSTB_MarkSttnUpDn,fb.Bwd);

            if (Strip1 && Sensor1) s1.Visible = true; else if(!Strip1 && !Sensor1) s1.Visible = false; else s1.Visible = SEQ._bFlick ? true : false;
            if (Strip2 && Sensor2) s2.Visible = true; else if( Strip2 && !Sensor2 && !Lift1Dn) s2.Visible = true; else if(!Strip2 && !Sensor2) s2.Visible = false; else s2.Visible = SEQ._bFlick ? true : false;
            if (Strip3 && Sensor3) s3.Visible = true; else if( Strip3 && !Sensor3 && !Lift2Dn) s3.Visible = true; else if(!Strip3 && !Sensor3) s3.Visible = false; else s3.Visible = SEQ._bFlick ? true : false;
            if (Strip4 && Sensor4) s4.Visible = true; else if( Strip4 && !Sensor4 && !Lift3Dn) s4.Visible = true; else if(!Strip4 && !Sensor4) s4.Visible = false; else s4.Visible = SEQ._bFlick ? true : false;
            if (Strip5 && Sensor5) s5.Visible = true; else if( Strip5 && !Sensor5 && !Lift4Dn) s5.Visible = true; else if(!Strip5 && !Sensor5) s5.Visible = false; else s5.Visible = SEQ._bFlick ? true : false;

            
            
            

            //Sensor
            RA1.BackColor = ML.IO_GetX(xi.PREB_PkgInDetect) ? Color.Lime : SystemColors.ActiveCaption;
            RA2.BackColor = ML.IO_GetX(xi.PREB_StrpDetect ) ? Color.Lime : SystemColors.ActiveCaption;
            RB1.BackColor = ML.IO_GetX(xi.RAIL_Vsn1Detect ) ? Color.Lime : SystemColors.ActiveCaption;
            RC1.BackColor = ML.IO_GetX(xi.RAIL_Vsn2Detect ) ? Color.Lime : SystemColors.ActiveCaption;
            RD1.BackColor = ML.IO_GetX(xi.RAIL_Vsn3Detect ) ? Color.Lime : SystemColors.ActiveCaption;
            RE1.BackColor = ML.IO_GetX(xi.PSTB_MarkDetect ) ? Color.Lime : SystemColors.ActiveCaption;
            RF1.BackColor = ML.IO_GetX(xi.PSTB_PkgDetect1 ) ? Color.Lime : SystemColors.ActiveCaption;
            RF2.BackColor = ML.IO_GetX(xi.PSTB_PkgDetect2 ) ? Color.Lime : SystemColors.ActiveCaption;

            tmUpdate.Enabled = true;
        }

        private void btLotOpen_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            FrmLotOpen = new FormLotOpen();
            FrmLotOpen.Show();
        }

        //public void DispLotInfo(ListView _lvLotInfo) //오퍼레이션 창용.
        public void DispLotInfo() //오퍼레이션 창용.
        {
            lvLotInfo.Clear();
            lvLotInfo.View = View.Details;
            lvLotInfo.LabelEdit = true;
            lvLotInfo.AllowColumnReorder = true;
            lvLotInfo.FullRowSelect = true;
            lvLotInfo.GridLines = true;
            //lvLotInfo.Sorting = SortOrder.Descending;
            lvLotInfo.Scrollable = true;
            lvLotInfo.Enabled = true;
            //lvLotInfo.TileSize = new Size(lvLotInfo.Width,lvLotInfo.Height);
            //lvLotInfo.Columns.Add("", 200, HorizontalAlignment.Left);     //1280*1024는 109, 1024*768은 78
            //lvLotInfo.Columns.Add("", 300, HorizontalAlignment.Left);     //1280*1024는 109, 1024*768은 78
            lvLotInfo.Columns.Add("", 175, HorizontalAlignment.Left);     //1280*1024는 109, 1024*768은 78
            //lvLotInfo.Columns.Add("", 100, HorizontalAlignment.Left);     //1280*1024는 109, 1024*768은 78

            ListViewItem[] liLotInfo = new ListViewItem[LotInfoCnt*2];
        
            for (int i = 0; i < LotInfoCnt*2; i++)
            {
                liLotInfo[i] = new ListViewItem();
                liLotInfo[i].SubItems.Add("");
        
        
                liLotInfo[i].UseItemStyleForSubItems = false;
                liLotInfo[i].UseItemStyleForSubItems = false;

                //liLotInfo[i].BackColor = Color.White;
                lvLotInfo.Items.Add(liLotInfo[i]);
              
            }
        
            var PropLotInfo = lvLotInfo.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropLotInfo.SetValue(lvLotInfo, true, null);
        }

        public void DispDayList() //오퍼레이션 창용.
        {
            lvDayInfo.Clear();
            lvDayInfo.View = View.Details;
            lvDayInfo.LabelEdit = true;
            lvDayInfo.AllowColumnReorder = true;
            lvDayInfo.FullRowSelect = true;
            lvDayInfo.GridLines = true;
            //lvDayInfo.Sorting = SortOrder.Descending;
            lvDayInfo.Scrollable = true;
            lvDayInfo.Enabled = true;
            //lvDayInfo.TileSize = new Size(lvDayInfo.Width - 22,lvDayInfo.Height/DayInfoCnt);

            lvDayInfo.Columns.Add("", 175, HorizontalAlignment.Left);    //1280*1024는 109, 1024*768은 78
            //lvDayInfo.Columns.Add("", 100, HorizontalAlignment.Left);    //1280*1024는 109, 1024*768은 78

            ListViewItem[] liDayInfo = new ListViewItem[DayInfoCnt*2];

            for (int i = 0; i < DayInfoCnt*2; i++)
            {
                liDayInfo[i] = new ListViewItem();
                liDayInfo[i].SubItems.Add("");


                liDayInfo[i].UseItemStyleForSubItems = false;
                liDayInfo[i].UseItemStyleForSubItems = false;

                lvDayInfo.Items.Add(liDayInfo[i]);

            }

            var PropDayInfo = lvDayInfo.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropDayInfo.SetValue(lvDayInfo, true, null);

            if (lvDayInfo == null) return;
        }

        private void btStart_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            SEQ._bBtnStart = true;
        }

        private void btStop_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            SEQ._bBtnStop = true;
        }

        private void btReset_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            SEQ._bBtnReset = true;
        }

        private void FormOperation_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmUpdate.Enabled = false;
            DM.SaveMap();

            //VL.Close();
        }

        private void btManual1_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            if (Log.ShowMessageModal("Confirm", "Do you want to perform manual actions?") != DialogResult.Yes) return;

            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);
            MM.SetManCycle((mc)iBtnTag);
        }
		
        private void btCylinder1_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);
            ML.CL_Move((ci)iBtnTag, ML.CL_GetCmd((ci)iBtnTag) == fb.Bwd ? fb.Fwd : fb.Bwd);
        }

        private void btOperator_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            //pnPassWord.Visible = true;
            //if (FrmLogOn.m_iCrntLogIn == (int)EN_LOGIN.LogOut)
            //{
            //    FrmLogOn.ShowPage();
            //}
            SM.FrmLogOn.Show();
        }

        private void btAllHome_Click(object sender, EventArgs e)
        {
            if (!ML.IO_GetY(yi.ETC_MainAirOnOff))
            {
                ML.ER_SetErr(ei.ETC_MainAir, "메인 에어를 ON 해주세요.");
                return;
            }

            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            if (!OM.MstOptn.bDebugMode)
            {
                if (Log.ShowMessageModal("Confirm", "Do you want to perform manual actions?") != DialogResult.Yes) return;
                MM.SetManCycle(mc.AllHome);
            }
            else
            {
                DialogResult Rslt;
                Rslt = MessageBox.Show("홈동작을 생략 하겠습니까?", "Confirm", MessageBoxButtons.YesNoCancel);
                if (Rslt == DialogResult.Yes)
                {
                    ML.MT_SetServoAll(true);
                    Thread.Sleep(100);
                    for (mi i = 0; i < mi.MAX_MOTR; i++)
                    {
                        ML.MT_SetHomeDone(i, true);
                    }
                }
                else if (Rslt == DialogResult.No)
                {
                    MM.SetManCycle(mc.AllHome);
                }

            }
        }

        private void FormOperation_Shown(object sender, EventArgs e)
        {
            tmUpdate.Enabled = true;
        }

        private void FormOperation_VisibleChanged(object sender, EventArgs e)
        {
            tmUpdate.Enabled = this.Visible;
        }

        public void MakeDoubleBuffered(Control control, bool setting)
        {
            Type controlType = control.GetType();
            PropertyInfo pi = controlType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(control, setting, null);
        }

        private void btRemoveStrip_Click(object sender, EventArgs e)
        {
            if (Log.ShowMessageModal("Confirm", "Do you want to remove the strip?") != DialogResult.Yes) return;

            string sTag = ((Button)sender).Tag.ToString() ;
                 if(sTag == "0") { DM.ARAY[ri.PREB].ClearMap(); }
            else if(sTag == "1") { DM.ARAY[ri.VSN1].ClearMap(); DM.ARAY[ri.RLT1].ClearMap(); DM.ARAY[ri.WRK1].ClearMap();}
            else if(sTag == "2") { DM.ARAY[ri.VSN2].ClearMap(); DM.ARAY[ri.RLT2].ClearMap(); DM.ARAY[ri.WRK2].ClearMap();}
            else if(sTag == "3") { DM.ARAY[ri.VSN3].ClearMap(); DM.ARAY[ri.RLT3].ClearMap(); DM.ARAY[ri.WRK3].ClearMap();}
            else if(sTag == "4") { DM.ARAY[ri.PSTB].ClearMap(); }

        }


        private void button26_Click(object sender, EventArgs e)
        {
            LOT.LotOpen(tbLotNo.Text);
        }

        private void button28_Click(object sender, EventArgs e)
        {
            DM.ARAY[ri.PSTB].LotNo = tbLotNo.Text ;
            DM.ARAY[ri.PSTB].ID    = tbID.Text ;

            SPC.MAP.SaveDataMap(ri.PSTB);
            SPC.MAP.SaveDataCnt(SPC.LOT.Data.StartedAt , DM.ARAY[ri.PSTB].LotNo , OM.EqpStat.iRsltCnts);


        }

        private void button27_Click(object sender, EventArgs e)
        {
            LOT.LotEnd();
        }

        private void btLightOnOff_Click(object sender, EventArgs e)
        {
            ML.IO_SetY(yi.ETC_Light,!ML.IO_GetY(yi.ETC_Light));
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btCyl_Click(object sender, EventArgs e)
        {
            string sText = ((Label)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            Label iSelLabel = sender as Label ;

            if(iSelLabel == null) return ;

            if(SEQ._bRun) return ;


            int iSel = Convert.ToInt32(iSelLabel.Tag);
             
            ML.CL_Move(iSel , ML.CL_GetCmd(iSel)==fb.Fwd ? fb.Bwd : fb.Fwd ) ;
        }

        private void btOutput_Click(object sender, EventArgs e)
        {
            Label iSelLabel = sender as Label ;

            if(iSelLabel == null) return ;

            if (SEQ._bRun) return;

            int iSel = Convert.ToInt32(iSelLabel.Tag);
             
            ML.IO_SetY(iSel, !ML.IO_GetY(iSel)) ;
        }

        private void button34_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            if (SEQ._bRun) return;

            const int iBreakAdd = 3 ;
            bool bBreakStat = ML.  MT_GetY(mi.LODR_ZClmp , iBreakAdd) ;
            ML.MT_SetY(mi.LODR_ZClmp , iBreakAdd , !bBreakStat);

        }

        private void btULDBreakOnOff_Click(object sender, EventArgs e)
        {
            string sText = ((Button)sender).Text;
            Log.Trace(sFormText + sText + " Button Clicked", ti.Frm);

            if (SEQ._bRun) return;

            const int iBreakAdd = 3 ;
            bool bBreakStat = ML.  MT_GetY(mi.ULDR_ZClmp , iBreakAdd) ;
            ML.MT_SetY(mi.ULDR_ZClmp , iBreakAdd , !bBreakStat);

        }

        private void tcVision_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(tcVision.SelectedIndex == 1)
            {
                DM.ARAY[ri.LODR].SetParent(pnLODR);
                DM.ARAY[ri.ULDR].SetParent(pnULDR);
            }
            else
            {
                DM.ARAY[ri.LODR].SetParent(pnLODR1);
                DM.ARAY[ri.ULDR].SetParent(pnULDR1);
            }
        }

        private void s1_DoubleClick(object sender, EventArgs e)
        {
            if (Log.ShowMessageModal("Confirm", "Do you want to remove the strip?") != DialogResult.Yes) return;
            string sTag = ((PictureBox)sender).Tag.ToString();

            if (sTag == "1") DM.ARAY[ri.PREB].SetStat(cs.None);
            if (sTag == "2") DM.ARAY[ri.VSN1].SetStat(cs.None);
            if (sTag == "3") DM.ARAY[ri.VSN2].SetStat(cs.None);
            if (sTag == "4") DM.ARAY[ri.VSN3].SetStat(cs.None);
            if (sTag == "5") DM.ARAY[ri.PSTB].SetStat(cs.None);
        }

        private void btGetLotNo_Click(object sender, EventArgs e)
        {

            

            tbLotNo.Text = LOT.PopMgz();
        }


        //---------------------------------------------------------------------------------------------------------------
        //Script

        private string m_filename = null;
        private string m_baseTitle = null;
        private bool m_runOnReturn = false;
        //Script.Interpreter interpreter = new Script.Interpreter();
        private void button35_Click(object sender, EventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = "cscs";
            openFileDialog.Filter = "cscs files (*.cscs)|*.cscs|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 0;
            openFileDialog.RestoreDirectory = true;
            
            if (openFileDialog.ShowDialog() == DialogResult.OK )
            {
                try
                {
                    m_filename = openFileDialog.FileName;
                    string[] readText = File.ReadAllLines(m_filename);
                    textSource.Text = string.Join("\n", readText);
                    //textResult.Text = string.Join("\n", readText);
                    //textSource.Text = readText.ToString();
                    
                    //this.Title = m_baseTitle + openFileDialog.SafeFileName;
                    SaveFilename(m_filename);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Couldn't read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void SaveFilename(string filename)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["lastFilename"].Value = filename;
            config.Save(ConfigurationSaveMode.Minimal);
            //ConfigurationManager.RefreshSection("appSettings");
        }

        private void button36_Click(object sender, EventArgs e)
        {
            string text = textResult.Text;
            if (string.IsNullOrWhiteSpace(text))
            {
                textSource.Text = string.Empty;
            }
            else
            {
                textResult.Clear();
            }
        }
        //Script script = new Script();
        private void button37_Click(object sender, EventArgs e)
        {
            Constants.LOG = true;
            //Parser.Verbose = true;

            //string script = "if(2<=1){print(2+2-(3-1));if(4>2){set(www,500);set(ww,50);print(3*www-ww*2-(-3+sin(pi/2)));print(\"IF DONE\");}}else{print(\"ELSE\");set(www,env(path));print(www);print(\"I am in ELSE\");}print(\"We are DONE.\")";
            //string script = "set(b,env(path));set(env1,\"www.ilanguage.ch\");set(env2,\"Skype\");if(indexOf(b,env2)<0){append(b,env1);}print(b);";
            //string script = "set(i,1);set(b,\"Argument\");unless(i>5){append(b,2*i);print(i);print(b);set(i,i+1);}print(\"Done!\");";
            Stopwatch sw = new Stopwatch();
            sw.Reset();
            sw.Restart();
            Interpreter it = Script.Interpreter.Instance;

                    //int step = (int)EN_SEQ_STEP.Run;
                    //Interpreter.Instance.Process("set(seq.Step,"+step.ToString()+")");

            //string sTemp1 = Script.Interpreter.AutoRun.ToString();
            //string sTemp2 = nameof(ri.PSTB);
            //string sTemp3 = nameof(Script.Interpreter.AutoRun);

            string script = textSource.SelectedText;
            if (m_runOnReturn)
            {
                script = textSource.Lines[textSource.GetLineFromCharIndex(textSource.SelectionStart - 1)];
            }
            else
            {
                textResult.Clear();//Document.Blocks.Clear();
            }

            if (string.IsNullOrWhiteSpace(script))
            {
                script = textSource.Text;
            }
            string errorMsg = null;
            try
            {
                //script = @"if(0<=1){print(2+2-(3-1));set(www,500);set(ww,50);}print(3*www-ww*2-(-3+sin(pi/2)));";
                //Script.Interpreter.Instance.Process(script);
                bool bRet = Interpreter.Instance.Process(script);
            }
            catch(ArgumentException exc)
            {
                errorMsg = exc.Message;
            }

            textResult.ForeColor = Color.Green;//Brushes.Green;
                  
            //string output = Script.Interpreter.Instance.Output;
            string output = Interpreter.Instance.Output + "\r\n";
            if (!string.IsNullOrWhiteSpace(output))
            {
                //textResult.Document.Blocks.Add(new Paragraph(new Run(output)));
                textResult.AppendText(output);
            }
            if (!string.IsNullOrWhiteSpace(errorMsg))
            {
                //Paragraph p = new Paragraph(new Run(errorMsg));
                //p.Foreground = Brushes.Red;
                //textResult.Document.Blocks.Add(p);

                textResult.SelectionStart = textResult.TextLength;
                textResult.SelectionLength = 0;

                textResult.SelectionColor = Color.Red;
                //textResult.SelectionFont = font;
                textResult.AppendText(errorMsg);
                textResult.SelectionColor = textResult.ForeColor;
                //textResult.AppendText(errorMsg);
                
            }

            //ParserFunction.GetFunction("set");
            double dTemp = sw.ElapsedMilliseconds;
            
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            m_runOnReturn = checkBox1.Checked == true;
            //textSource.
        }

        private void textSource_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void button39_Click(object sender, EventArgs e)
        {
            
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = "cscs";
            saveFileDialog.Filter = "cscs files (*.cscs)|*.cscs|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                m_filename = saveFileDialog.FileName;
                SaveFile();

                //this.Title = m_baseTitle + saveFileDialog.SafeFileName;
                SaveFilename(m_filename);
            }
            //Interpreter.Instance.ReLoad();
            
        }

        private void SaveFile()
        {
            try
            {
                File.WriteAllText(m_filename, textSource.Text);
            
                //Paragraph p = new Paragraph(new Run("File " + m_filename + " saved."));
                //p.Foreground = Brushes.Black;
                //textResult.Document.Blocks.Add(p);
                textResult.AppendText("File " + m_filename + " saved.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Couldn't write file to disk. Original error: " + ex.Message);
            }
            //Interpreter.Instance.ReLoad();
        }

        private void textSource_KeyDown(object sender, KeyEventArgs e)
        {
            RichTextBox rtAct = sender as RichTextBox;
            if (lbAutoComp.Visible)
            {
                if(e.KeyCode == Keys.Up)
                {
                    if(0 < lbAutoComp.SelectedIndex-1)lbAutoComp.SelectedIndex-- ;
                    e.Handled = true ;
                }
                else if(e.KeyCode == Keys.Down)
                {
                    if(lbAutoComp.Items.Count > lbAutoComp.SelectedIndex+1)lbAutoComp.SelectedIndex++ ;      
                    e.Handled = true ;
                }
                else if(e.KeyCode == Keys.Enter) 
                {
                    int    iSel  = rtAct.SelectionStart ;
                    string sTemp = rtAct.Text ;
                    e.Handled = true ;
                }
                else if(e.Control && e.KeyCode == Keys.Space)
                {
                    lbAutoComp.Visible = false;
                    rtAct.Focus();
                    e.Handled = true;
                }
            }
            else
            {
                
            }

            //undo.
            if(e.KeyCode == Keys.Z && (e.Control))
            {
                string sPop = "" ;

                if(rtAct.CanUndo)
                {
                    rtAct.Undo();
                }


                //switch(tcSequence.SelectedIndex)
                //{
                //    case 0: if(UndoListRun    .Count>0)sPop = UndoListRun    .Pop(); break ;
                //    case 1: if(UndoListCool   .Count>0)sPop = UndoListCool   .Pop(); break ;
                //    case 2: if(UndoListExample.Count>0)sPop = UndoListExample.Pop(); break ;
                //    case 3: if(UndoListTest   .Count>0)sPop = UndoListTest   .Pop(); break ;
                //}
                //switch(tcSequence.SelectedIndex)
                //{
                //    case 0: rtRun     .Text = sPop; break ;
                //    case 1: rtCool    .Text = sPop; break ;
                //    case 2: rtExample .Text = sPop; break ;
                //    case 3: rtTest    .Text = sPop; break ;
                //}
            }
            //AddLineNumbers();
        }

        private void textSource_KeyPress(object sender, KeyPressEventArgs e)
        {
            
            RichTextBox rtAct = sender as RichTextBox ;
            //int pos = rtAct.GetLineFromCharIndex(rtAct.SelectionStart); 

            if (!lbAutoComp.Visible)
            {
                //밑에서 코딩할때 TextBox에 가려져서 Parent를 페이지컨트롤상단 Panel로 한다.
                //lbAutoComp.Parent = rtAct ;
                if (Control.ModifierKeys == Keys.Control && e.KeyChar == ' ') //Space
                {
                    keyword="";
                    char cCharictor ;
                    for(int i = 1 ; i < rtAct.SelectionStart ; i++)
                    {
                        cCharictor = rtAct.Text[rtAct.SelectionStart-i];
                             if(cCharictor >= '0' && cCharictor <= '9') keyword = cCharictor + keyword;
                        else if(cCharictor >= 'a' && cCharictor <= 'z') keyword = cCharictor + keyword;
                        else if(cCharictor >= 'A' && cCharictor <= 'Z') keyword = cCharictor + keyword;
                        else if(cCharictor == '_'                     ) keyword = cCharictor + keyword;
                        else                                            break ;
                    }
                    //List<string> typeNames = GetContainList(Utils.GetList(typeof(Constants),"string"), keyword);
                    List<string> typeNames = GetContainList(ParserFunction.GetFunction(), keyword);
                    lbAutoComp.Items.Clear();
                    if(typeNames != null)
                    {
                        foreach (string typeName in typeNames)
                        {
                            lbAutoComp.Items.Add(typeName);
                        }
                        lbAutoComp.SelectedIndex = lbAutoComp.FindString(keyword);
                        Point point = rtAct.GetPositionFromCharIndex(rtAct.SelectionStart);
                        point.Y += (int)Math.Ceiling(rtAct.Font.GetHeight()) + 5; //10 is the .y postion of the richtectbox
                        point.X  = point.X + ((int)Math.Ceiling(rtAct.Font.GetHeight()/2) * keyword.Length); //105 is the .x postion of the richtectbox
                        point.Y += 60 ; //이건 페어런트가 누구냐에따라 조절... 원래는 글로벌로 좌표 따서 해야 될듯.
                        //point.X += rtAct.Left ;
                        //point.Y += 
                        Point p = FindLocation(rtAct);
                        p.X += point.X;
                        p.Y += point.Y - 165;
                        lbAutoComp.Location = p;     
                        
                        lbAutoComp.Show();
                    }
                    
                    rtAct.Focus();
                    e.Handled = true;            
                }
            }
            else
            {
                if (e.KeyChar == '\b') //backspace시엔 키워드 줄인다.
                {
                    if(keyword.Length > 0)
                    { 
                        keyword = keyword.Remove(keyword.Length - 1);
                    }
                    else
                    {
                        lbAutoComp.Visible = false;
                        rtAct.Focus();
                        e.Handled = true;

                    }
                    //List<string> typeNames = GetContainList(CFunction.GetFunctions() , keyword);

                    //List<string> typeNames = GetContainList(Utils.GetList(typeof(Constants),"string"), keyword);
                    List<string> typeNames = GetContainList(ParserFunction.GetFunction(), keyword);
                    lbAutoComp.Items.Clear();
                    if(typeNames != null)
                    {
                        foreach (string typeName in typeNames)
                        {
                            lbAutoComp.Items.Add(typeName);
                        }
                        lbAutoComp.SelectedIndex = lbAutoComp.FindString(keyword);
                    }
                    
                }
                else if (e.KeyChar == '\r')//Enter시에는 리스트박스에 선택되어진 글짜를 리치텍스트박스에 완성시킨다.
                {
                    lbAutoComp.Visible = false;

                    //rtAct.Text.TrimEnd('\n');

                    string functionName = lbAutoComp.Text;
                    string AddingText   = "";
                    
                    
                    
                    //여기부터
                    rtAct.Focus();
                    int iStartCursorPos = rtAct.SelectionStart ;
                    //if(keyword.Length>0) 
                    {
                        rtAct.Text = rtAct.Text.Remove(iStartCursorPos - keyword.Length , keyword.Length);//-1은 엔터시에 \n이 이미 들어있음.
                        iStartCursorPos -= keyword.Length ;
                    }
                    

                    //if(keyword.Length>0) rtAct.Text = rtAct.Text.Remove(rtAct.Text.Length - keyword.Length-1);//-1은 엔터시에 \n이 이미 들어있음.

                    AddingText += functionName;
                    
                    int ?iParaCnt   = null ;//Utils.GetParaCount(functionName);
                    int iCursorPos = iStartCursorPos + AddingText.Length + 1;//int iCursorPos =  iStartCursorPos + AddingText.Length; //rtAct.Text.Length + AddingText.Length;
                    iParaCnt = Utils.GetParaCount(functionName);
                    if(iParaCnt != null){// !AddingText.Contains(".")) {
                        
                        AddingText += "(";
                        for(int i = 1 ; i < iParaCnt ; i++) AddingText += "," ;
                        AddingText += ");";
                        //AddingText += CFunction.GetComment(functionName);
                        AddingText += "//" + Utils.GetComment(functionName);

                    }
                    
                    int iTextLength = rtAct.Text.Length ;
                    rtAct.Text = rtAct.Text.Insert(iStartCursorPos , AddingText);
                    //if(iTextLength <= iStartCursorPos) rtAct.Text+=AddingText;
                    //else                               rtAct.Text = rtAct.Text.Insert(iStartCursorPos , AddingText);

                    rtAct.Focus();//커서
                    if(iParaCnt==0) iCursorPos = iStartCursorPos + AddingText.Length ;
                    rtAct.SelectionStart = iCursorPos ;
                    e.Handled = true;
                }
                else if (e.KeyChar == '\u001b') //Esc시엔 취소
                {
                    lbAutoComp.Visible = false;
                    rtAct.Focus();
                    e.Handled = true;
                }
                else//나머지는 그냥 글짜를 붙인다.
                {
                    keyword += e.KeyChar;
                    //List<string> typeNames = GetContainList(CFunction.GetFunctions() , keyword);
                    //List<string> typeNames = GetContainList(Utils.GetList(typeof(Constants),"string"), keyword);
                    List<string> typeNames = GetContainList(ParserFunction.GetFunction(), keyword);
                    lbAutoComp.Items.Clear();
                    if(typeNames != null)
                    {
                        foreach (string typeName in typeNames)
                        {
                            lbAutoComp.Items.Add(typeName);
                        }
                        lbAutoComp.SelectedIndex = lbAutoComp.FindString(keyword);
                    }
                    rtAct.Focus();
                }
            }
            
        }

        List<string> GetContainList(List<string> types , string _sWord)
        {
            if(types == null) return null ;
            List<string> lsTypeNames = new List<string>();
            foreach (string type in types)
            {
                //소문자로 변환하여 소문자 소문자 구분없이 비교.
                if(type.ToLower().Contains(_sWord.ToLower())) lsTypeNames.Add(type);
            }
            if(lsTypeNames.Count == 0)
            {
                foreach (string type in types)
                {
                    lsTypeNames.Add(type);
                }
            }
            return lsTypeNames ;
        }

        string keyword = "";

        private static Point FindLocation(Control ctrl)
        {
            Point p;
            for (p = ctrl.Location; ctrl.Parent != null; ctrl = ctrl.Parent)
                p.Offset(ctrl.Parent.Location);
            return p;
        }

        private void button38_Click(object sender, EventArgs e)
        {
            SaveFile();
        }
    }

    public class DoubleBuffer : Panel
    {
        public DoubleBuffer()
        {
            this.SetStyle(ControlStyles.ResizeRedraw         , true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint            , true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint , true);

            this.UpdateStyles();
        }
    }
    public class DoubleBufferP : PictureBox
    {
        public DoubleBufferP()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.UserPaint |
              ControlStyles.AllPaintingInWmPaint  |
              ControlStyles.ResizeRedraw          |
              ControlStyles.ContainerControl      |
              ControlStyles.OptimizedDoubleBuffer |
              ControlStyles.SupportsTransparentBackColor
              , true);
        }
    }



}