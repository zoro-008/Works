using COMMON;
using SML2;
using System;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace Machine
{
    public partial class FormOperation : Form
    {
        public static FormOperation FrmOperation;
        public static FormLotOpen FrmLotOpen;
        public static FormMain FrmMain;
        public static FormPrint FrmPrint; 
        //public static FormOracle FrmOracle ; //For Send Msg 

           

        protected CDelayTimer m_tmStartBt ;



        public void SendListMsg(string _sMsg)
        {
            lvLog.Invoke(new OracleBase.FSendMsg(ListMsg), new string[]{_sMsg});
        }
        private void ListMsg(string _sMsg)
        {
            if (!lvLog.GridLines)
            {
                lvLog.View = View.Details;
                lvLog.GridLines = true;
                lvLog.Columns.Add("Message", lvLog.Size.Width, HorizontalAlignment.Left);
                var PropError = lvLog.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
                PropError.SetValue(lvLog, true, null);

            }

            lvLog.Items.Add(_sMsg);
            if(lvLog.Items.Count > 500 ){
                lvLog.Items.RemoveAt(0);
            }
            lvLog.Items[lvLog.Items.Count - 1].EnsureVisible();
        }

        //public EN_SEQ_STAT m_iSeqStat;

        public FormOperation(Panel _pnBase)
        {
            InitializeComponent();

            this.TopLevel = false;
            this.Parent = _pnBase;

            //DispDayList();
            DispLotInfo();

            tmUpdate.Enabled = true;

            //btStart.Enabled = LOT.GetLotOpen();

            m_tmStartBt = new CDelayTimer();

            DM.ARAY[ri.SPLR].SetParent(pnSPLR); DM.ARAY[ri.SPLR].Name = "SPLR";   
            DM.ARAY[ri.IDXR].SetParent(pnIDXR); DM.ARAY[ri.IDXR].Name = "IDXR";
            DM.ARAY[ri.IDXF].SetParent(pnIDXF); DM.ARAY[ri.IDXF].Name = "IDXF";
            DM.ARAY[ri.PCKR].SetParent(pnPCKR); DM.ARAY[ri.PCKR].Name = "PCKR";
            DM.ARAY[ri.TRYF].SetParent(pnTRYF); DM.ARAY[ri.TRYF].Name = "TRYF";
            DM.ARAY[ri.TRYG].SetParent(pnTRYG); DM.ARAY[ri.TRYG].Name = "TRYG";
            DM.ARAY[ri.OUTZ].SetParent(pnOUTZ); DM.ARAY[ri.OUTZ].Name = "OUTZ";
            DM.ARAY[ri.STCK].SetParent(pnSTCK); DM.ARAY[ri.STCK].Name = "STCK";
            DM.ARAY[ri.BARZ].SetParent(pnBARC); DM.ARAY[ri.BARZ].Name = "BARC";
            DM.ARAY[ri.INSP].SetParent(pnINSP); DM.ARAY[ri.INSP].Name = "INSP";
            DM.ARAY[ri.PSTC].SetParent(pnPSTC); DM.ARAY[ri.PSTC].Name = "PSTC";
            //DM.ARAY[ri.MASK].SetParent(pnSTCK); DM.ARAY[ri.PSTC].Name = "PSTC";
                        
            //Loader           
            DM.ARAY[ri.SPLR].SetDisp(cs.None      , "None"            ,Color.White        );
            DM.ARAY[ri.SPLR].SetDisp(cs.Unknown   , "UnKnown"         ,Color.Aqua         );
            DM.ARAY[ri.SPLR].SetDisp(cs.Empty     , "Empty"           ,Color.Silver       ); 
                                                                                     
            //Index Rear                                                             
            DM.ARAY[ri.IDXR].SetDisp(cs.None      , "None"            ,Color.White        );
            DM.ARAY[ri.IDXR].SetDisp(cs.Unknown   , "UnKnown"         ,Color.Aqua         );
            DM.ARAY[ri.IDXR].SetDisp(cs.Empty     , "Empty"           ,Color.Silver       );
            DM.ARAY[ri.IDXR].SetDisp(cs.Vision    , "Vision"          ,Color.Yellow       );
            DM.ARAY[ri.IDXR].SetDisp(cs.Good      , "Good"            ,Color.Green        );
            DM.ARAY[ri.IDXR].SetDisp(cs.NG0       , "V_Empty"         ,Color.Orange       );
            DM.ARAY[ri.IDXR].SetDisp(cs.NG1       , "V_MixDevice"     ,Color.Coral        );
            DM.ARAY[ri.IDXR].SetDisp(cs.NG2       , "V_UnitID"        ,Color.DarkOrchid   );
            DM.ARAY[ri.IDXR].SetDisp(cs.NG3       , "V_UnitDMC1"      ,Color.DarkTurquoise);
            DM.ARAY[ri.IDXR].SetDisp(cs.NG4       , "V_UnitDMC2"      ,Color.Olive        );
            DM.ARAY[ri.IDXR].SetDisp(cs.NG5       , "V_GlobtopLeft"   ,Color.DeepSkyBlue  );
            DM.ARAY[ri.IDXR].SetDisp(cs.NG6       , "V_GlobtopTop"    ,Color.Crimson      );
            DM.ARAY[ri.IDXR].SetDisp(cs.NG7       , "V_GlobtopRight"  ,Color.SlateBlue    );
            DM.ARAY[ri.IDXR].SetDisp(cs.NG8       , "V_GlobtopBottom" ,Color.DarkCyan     );
            DM.ARAY[ri.IDXR].SetDisp(cs.NG9       , "V_MatchingError" ,Color.DarkKhaki    );
            DM.ARAY[ri.IDXR].SetDisp(cs.NG10      , "V_UserDefine"    ,Color.DarkGoldenrod);                                                                                    
                                                                                    
            //Index Front                                                           
            DM.ARAY[ri.IDXF].SetDisp(cs.None      , "None"            ,Color.White        );
            DM.ARAY[ri.IDXF].SetDisp(cs.Unknown   , "UnKnown"         ,Color.Aqua         );
            DM.ARAY[ri.IDXF].SetDisp(cs.Empty     , "Empty"           ,Color.Silver       );
            DM.ARAY[ri.IDXF].SetDisp(cs.Vision    , "Vision"          ,Color.Yellow       );
            DM.ARAY[ri.IDXF].SetDisp(cs.Good      , "Good"            ,Color.Green        );
            DM.ARAY[ri.IDXF].SetDisp(cs.NG0       , "V_Empty"         ,Color.Orange       );
            DM.ARAY[ri.IDXF].SetDisp(cs.NG1       , "V_MixDevice"     ,Color.Coral        );
            DM.ARAY[ri.IDXF].SetDisp(cs.NG2       , "V_UnitID"        ,Color.DarkOrchid   );
            DM.ARAY[ri.IDXF].SetDisp(cs.NG3       , "V_UnitDMC1"      ,Color.DarkTurquoise);
            DM.ARAY[ri.IDXF].SetDisp(cs.NG4       , "V_UnitDMC2"      ,Color.Olive        );
            DM.ARAY[ri.IDXF].SetDisp(cs.NG5       , "V_GlobtopLeft"   ,Color.DeepSkyBlue  );
            DM.ARAY[ri.IDXF].SetDisp(cs.NG6       , "V_GlobtopTop"    ,Color.Crimson      );
            DM.ARAY[ri.IDXF].SetDisp(cs.NG7       , "V_GlobtopRight"  ,Color.SlateBlue    );
            DM.ARAY[ri.IDXF].SetDisp(cs.NG8       , "V_GlobtopBottom" ,Color.DarkCyan     );
            DM.ARAY[ri.IDXF].SetDisp(cs.NG9       , "V_MatchingError" ,Color.DarkKhaki    );
            DM.ARAY[ri.IDXF].SetDisp(cs.NG10      , "V_UserDefine"    ,Color.DarkGoldenrod);
      
            //Picker                                               
            DM.ARAY[ri.PCKR].SetDisp(cs.None      , "None"            ,Color.White        );
            DM.ARAY[ri.PCKR].SetDisp(cs.Unknown   , "UnKnown"         ,Color.Aqua         );
            DM.ARAY[ri.PCKR].SetDisp(cs.Empty     , "Empty"           ,Color.Silver       );
            DM.ARAY[ri.PCKR].SetDisp(cs.Good      , "Good"            ,Color.Green        );
            DM.ARAY[ri.PCKR].SetDisp(cs.NG0       , "V_Empty"         ,Color.Orange       );
            DM.ARAY[ri.PCKR].SetDisp(cs.NG1       , "V_MixDevice"     ,Color.Coral        );
            DM.ARAY[ri.PCKR].SetDisp(cs.NG2       , "V_UnitID"        ,Color.DarkOrchid   );
            DM.ARAY[ri.PCKR].SetDisp(cs.NG3       , "V_UnitDMC1"      ,Color.DarkTurquoise);
            DM.ARAY[ri.PCKR].SetDisp(cs.NG4       , "V_UnitDMC2"      ,Color.Olive        );
            DM.ARAY[ri.PCKR].SetDisp(cs.NG5       , "V_GlobtopLeft"   ,Color.DeepSkyBlue  );
            DM.ARAY[ri.PCKR].SetDisp(cs.NG6       , "V_GlobtopTop"    ,Color.Crimson      );
            DM.ARAY[ri.PCKR].SetDisp(cs.NG7       , "V_GlobtopRight"  ,Color.SlateBlue    );
            DM.ARAY[ri.PCKR].SetDisp(cs.NG8       , "V_GlobtopBottom" ,Color.DarkCyan     );
            DM.ARAY[ri.PCKR].SetDisp(cs.NG9       , "V_MatchingError" ,Color.DarkKhaki    );
            DM.ARAY[ri.PCKR].SetDisp(cs.NG10      , "V_UserDefine"    ,Color.DarkGoldenrod);
                                                                                    
            //Fail Tray                                                           
            DM.ARAY[ri.TRYF].SetDisp(cs.None      , "None"            ,Color.White        );
            DM.ARAY[ri.TRYF].SetDisp(cs.Empty     , "Empty"           ,Color.Silver       );
            DM.ARAY[ri.TRYF].SetDisp(cs.NG0       , "V_Empty"         ,Color.Orange       );
            DM.ARAY[ri.TRYF].SetDisp(cs.NG1       , "V_MixDevice"     ,Color.Coral        );
            DM.ARAY[ri.TRYF].SetDisp(cs.NG2       , "V_UnitID"        ,Color.DarkOrchid   );
            DM.ARAY[ri.TRYF].SetDisp(cs.NG3       , "V_UnitDMC1"      ,Color.DarkTurquoise);
            DM.ARAY[ri.TRYF].SetDisp(cs.NG4       , "V_UnitDMC2"      ,Color.Olive        );
            DM.ARAY[ri.TRYF].SetDisp(cs.NG5       , "V_GlobtopLeft"   ,Color.DeepSkyBlue  );
            DM.ARAY[ri.TRYF].SetDisp(cs.NG6       , "V_GlobtopTop"    ,Color.Crimson      );
            DM.ARAY[ri.TRYF].SetDisp(cs.NG7       , "V_GlobtopRight"  ,Color.SlateBlue    );
            DM.ARAY[ri.TRYF].SetDisp(cs.NG8       , "V_GlobtopBottom" ,Color.DarkCyan     );
            DM.ARAY[ri.TRYF].SetDisp(cs.NG9       , "V_MatchingError" ,Color.DarkKhaki    );
            DM.ARAY[ri.TRYF].SetDisp(cs.NG10      , "V_UserDefine"    ,Color.DarkGoldenrod);
                                                                                    
            //Good Tray                                                               
            DM.ARAY[ri.TRYG].SetDisp(cs.None      , "None"            ,Color.White        );
            DM.ARAY[ri.TRYG].SetDisp(cs.Empty     , "Empty"           ,Color.Silver       );
            DM.ARAY[ri.TRYG].SetDisp(cs.Good      , "Good"            ,Color.Green        );
                                                                                          
            //Out Zone                                                                   
            DM.ARAY[ri.OUTZ].SetDisp(cs.None      , "None"            ,Color.White        );
            DM.ARAY[ri.OUTZ].SetDisp(cs.Good      , "Good"            ,Color.Green        );
                                                                      
            //Pre Stack Zone                                                                    
            DM.ARAY[ri.PSTC].SetDisp(cs.None      , "None"            ,Color.White        ); 
            DM.ARAY[ri.PSTC].SetDisp(cs.Good      , "Good"            ,Color.Green        ); 
                                                                                          
            //Stack Zone                                                                    
            DM.ARAY[ri.STCK].SetDisp(cs.None      , "None"            ,Color.White        ); 
            DM.ARAY[ri.STCK].SetDisp(cs.Empty     , "Empty"           ,Color.Silver       );
            DM.ARAY[ri.STCK].SetDisp(cs.Good      , "Good"            ,Color.Green        ); 
                                                                                          
            //Barcode Zone                                                               
            DM.ARAY[ri.BARZ].SetDisp(cs.None      , "None"            ,Color.White        );
            DM.ARAY[ri.BARZ].SetDisp(cs.Unknown   , "UnKnown"         ,Color.Aqua         );
            DM.ARAY[ri.BARZ].SetDisp(cs.Barcode   , "Barcode"         ,Color.Tan          );
            DM.ARAY[ri.BARZ].SetDisp(cs.BarRead   , "BarRead"         ,Color.Fuchsia      );
            DM.ARAY[ri.BARZ].SetDisp(cs.WorkEnd   , "WorkEnd"         ,Color.Blue         );

            DM.ARAY[ri.INSP].SetDisp(cs.NG0       , "V_Empty"         ,Color.Orange       );
            DM.ARAY[ri.INSP].SetDisp(cs.NG1       , "V_MixDevice"     ,Color.Coral        );
            DM.ARAY[ri.INSP].SetDisp(cs.NG2       , "V_UnitID"        ,Color.DarkOrchid   );
            DM.ARAY[ri.INSP].SetDisp(cs.NG3       , "V_UnitDMC1"      ,Color.DarkTurquoise);
            DM.ARAY[ri.INSP].SetDisp(cs.NG4       , "V_UnitDMC2"      ,Color.Olive        );
            DM.ARAY[ri.INSP].SetDisp(cs.NG5       , "V_GlobtopLeft"   ,Color.DeepSkyBlue  );
            DM.ARAY[ri.INSP].SetDisp(cs.NG6       , "V_GlobtopTop"    ,Color.Crimson      );
            DM.ARAY[ri.INSP].SetDisp(cs.NG7       , "V_GlobtopRight"  ,Color.SlateBlue    );
            DM.ARAY[ri.INSP].SetDisp(cs.NG8       , "V_GlobtopBottom" ,Color.DarkCyan     );
            DM.ARAY[ri.INSP].SetDisp(cs.NG9       , "V_MatchingError" ,Color.DarkKhaki    );
            DM.ARAY[ri.INSP].SetDisp(cs.NG10      , "V_UserDefine"    ,Color.DarkGoldenrod);
            DM.ARAY[ri.INSP].SetDisp(cs.Good      , "Good"            ,Color.Green        );

                                                                                   
            DM.ARAY[ri.SPLR].SetMaxColRow(1                        , 1                           );
            DM.ARAY[ri.IDXR].SetMaxColRow(OM.DevInfo.iTRAY_PcktCntX, OM.DevInfo.iTRAY_PcktCntY   );
            DM.ARAY[ri.IDXF].SetMaxColRow(OM.DevInfo.iTRAY_PcktCntX, OM.DevInfo.iTRAY_PcktCntY   );
            DM.ARAY[ri.PCKR].SetMaxColRow(1                        , 1                           );
            DM.ARAY[ri.TRYF].SetMaxColRow(OM.DevInfo.iTRAY_PcktCntX, OM.DevInfo.iTRAY_PcktCntY   );
            DM.ARAY[ri.TRYG].SetMaxColRow(OM.DevInfo.iTRAY_PcktCntX, OM.DevInfo.iTRAY_PcktCntY   );
            DM.ARAY[ri.OUTZ].SetMaxColRow(1                        , 1                           );
            DM.ARAY[ri.PSTC].SetMaxColRow(1                        , 1                           );
            DM.ARAY[ri.STCK].SetMaxColRow(1                        , OM.DevInfo.iTRAY_StackingCnt);
            DM.ARAY[ri.BARZ].SetMaxColRow(1                        , 1                           );
            DM.ARAY[ri.INSP].SetMaxColRow(1                        , OM.DevInfo.iTRAY_PcktCntY   );
            DM.ARAY[ri.PSTC].SetMaxColRow(1                        , 1                           );
            DM.ARAY[ri.MASK].SetMaxColRow(OM.DevInfo.iTRAY_PcktCntX, OM.DevInfo.iTRAY_PcktCntY   );


            DM.ARAY[ri.IDXR].SetMask(DM.ARAY[ri.MASK]);
            DM.ARAY[ri.IDXF].SetMask(DM.ARAY[ri.MASK]);
            DM.ARAY[ri.TRYF].SetMask(DM.ARAY[ri.MASK]);
            DM.ARAY[ri.TRYG].SetMask(DM.ARAY[ri.MASK]);


            
            
            FrmPrint  = new FormPrint();
            //FrmOracle = new FormOracle();

            SEQ.Oracle.SetSendMsgFunc(SendListMsg);

            DM.LoadMap();
        }


        private void btOperator_Click(object sender, EventArgs e)
        {
            //pnPassWord.Visible = true;
            //if (FrmLogOn.m_iCrntLogIn == (int)EN_LOGIN.LogOut)
            //{
            //    FrmLogOn.ShowPage();
            //}
            SML.FrmLogOn.Show();
        }

        private void btLotEnd_Click(object sender, EventArgs e)
        {
            if (Log.ShowMessageModal("Confirm", "Do you want to Clear Lot?") != DialogResult.Yes) return;
            LOT.LotEnd();
            DM.ARAY[ri.SPLR].SetStat(cs.None );
            DM.ARAY[ri.IDXR].SetStat(cs.None );
            DM.ARAY[ri.IDXF].SetStat(cs.None );
            DM.ARAY[ri.PCKR].SetStat(cs.None );
            //DM.ARAY[ri.TRYF].SetStat(cs.None );
            //DM.ARAY[ri.TRYG].SetStat(cs.None );
            DM.ARAY[ri.OUTZ].SetStat(cs.None );
            DM.ARAY[ri.STCK].SetStat(cs.Empty);
            DM.ARAY[ri.BARZ].SetStat(cs.None );
            DM.ARAY[ri.INSP].SetStat(cs.Good );
            DM.ARAY[ri.PSTC].SetStat(cs.None );
            btStart.Enabled = false;

            if (Log.ShowMessageModal("Confirm", "Do you want to make VIT file and Local Report?") != DialogResult.Yes) return;

            LOT.LotEnd();
            Log.Trace("SEQ",LOT.GetLotNo() + "LotEnd");

            //HZ7290XH98_006_170928_143116P
            int iRealTrayCnt = OM.DevInfo.iTRAY_StackingCnt;
            iRealTrayCnt-- ; //Top CoverTray 
            if(OM.DevOptn.bUseBtmCover)iRealTrayCnt--;
            int iQty = OM.DevInfo.iTRAY_PcktCntX * OM.DevInfo.iTRAY_PcktCntY * iRealTrayCnt ;
            iQty *= OM.EqpStat.iWorkBundle ;
            if (!OM.CmnOptn.bOracleNotWriteVITFile && OM.CmnOptn.sVITFolder != "") { 
                SEQ.Oracle.WriteVIT(OM.CmnOptn.sVITFolder                  , 
                                    DateTime.Now.ToString("MM\\/dd\\/yyyy"),
                                    OM.CmnOptn.sMachinID                   ,
                                    LOT.CrntLotData.sEmployeeID            ,  //20180125 SML.FrmLogOn.GetId()                   ,
                                    OM.EqpStat.sLotSttTime                 ,
                                    DateTime.Now.ToString("HH:mm:ss")      ,
                                    iQty.ToString());
            }
        }


        private static bool bPreLotOpen = false;
        private static int iTrayImg = 0;

        private void timer1_Tick(object sender, EventArgs e)
        {
            tmUpdate.Enabled = false;

            //로그인/로그아웃 방식
            if (SML.FrmLogOn.GetLevel() == (int)EN_LEVEL.LogOff)
            {
                btOperator.Text = "LOG IN";
                pnDataMap .Enabled = false;
                //pnDayInfo .Enabled = false;
                pnLotInfo .Enabled = false;
                pnError   .Enabled = false;
                pnOperMan .Enabled = false;
                //pnWorkInfo.Enabled = false;
                pnLotOpen .Enabled = false;
                //btStart   .Enabled = LOT.LotList.Count == 0 || !LOT.LotOpened;
                btStart   .Enabled = false;
                btStop    .Enabled = false;
                btReset   .Enabled = false;
                btLightOnOff .Enabled = false;
                btOperator.Enabled = true;
            }
            else
            {
                btOperator.Text = SML.FrmLogOn.GetLevel().ToString();
                pnDataMap .Enabled = true;
                //pnDayInfo .Enabled = true;
                pnLotInfo .Enabled = true;
                pnError   .Enabled = true;
                pnOperMan .Enabled = true;
                //pnWorkInfo.Enabled = true;
                pnLotOpen .Enabled = true;
                //btStart   .Enabled = LOT.LotList.Count != 0 || LOT.LotOpened;
                btStart   .Enabled = true;
                btStop    .Enabled = true;
                btReset   .Enabled = true;
                btLightOnOff .Enabled = true;
                
            }



            btLotOpen.Enabled = !LOT.GetLotOpen() ;
            

            lbDate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            
            TimeSpan Span ;
            try{
                    Span = TimeSpan.FromMilliseconds(SPC.LOT.Data.RunTime);
                }
                catch(Exception ex){          
                    Span = TimeSpan.FromMilliseconds(0);
                }
            //label11.Text = OM.EqpStat.iLDRSplyCnt.ToString() ; 

            string Str      ;
            int iPreErrCnt  = 0;
            int iCrntErrCnt = 0;
            for (int i = 0 ; i < SML.ER._iMaxErrCnt ; i++) 
            {
                if (SML.ER.GetErr(i)) iCrntErrCnt++;
            }
            if (iPreErrCnt != iCrntErrCnt ) 
            {
                lbErr.Items.Clear();
                int iErrNo = SM.ER_GetLastErr();
                for (int i = 0; i < SML.ER._iMaxErrCnt; i++) 
                {
                    if (SML.ER.GetErr(i))
                    {
                        Str = string.Format("[ERR{0:000}]" , i) ;
                        Str += SML.ER.GetErrName(i) + " " + SML.ER.GetErrSubMsg(i);
                        lbErr.Items.Add(Str);
                    }
                }
            }
            if (SEQ._iSeqStat != EN_SEQ_STAT.Error)
            {
                lbErr.Items.Clear();
            }
            iPreErrCnt = iCrntErrCnt ;
          
            
            if(!SM.MT_GetHomeDoneAll()){
                btAllHome.ForeColor = SEQ._bFlick ? Color.Black : Color.Red;
            }
            else {
                btAllHome.ForeColor = Color.Black  ;
            }

            SPC.LOT.DispLotInfo(lvLotInfo);

            btInputTrayF  .Enabled = !SEQ._bRun;
            btInputTrayG  .Enabled = !SEQ._bRun;
            btAllEmptyTray.Enabled = !SEQ._bRun;
            btBarCodeReTry.Enabled = !SEQ._bRun;

            if (SM.IO_GetX(xi.BARZ_PckrVac)) lbBarVac.BackColor = Color.Lime;
            else                             lbBarVac.BackColor = Color.Red ;

            //JS
            //Manual Button Text.
            if(SM.CL_GetCmd(ci.LODR_ClampClOp    ) == fb.Bwd) btCylinder1 .Text = "Loader Clamp OPEN";
            else                                              btCylinder1 .Text = "Loader Clamp CLOSE";
            btCylinder1.ForeColor =  SM.CL_Complete(ci.LODR_ClampClOp) ? Color.Black : Color.Lime ;
                                                                          
            if(SM.CL_GetCmd(ci.LODR_SperatorUpDn ) == fb.Bwd) btCylinder2 .Text = "Loader Sperator DOWN";
            else                                              btCylinder2 .Text = "Loader Sperator Up";
            btCylinder2.ForeColor =  SM.CL_Complete(ci.LODR_SperatorUpDn) ? Color.Black : Color.Lime ;
                                                                          
            if(SM.CL_GetCmd(ci.IDXR_ClampClOp    ) == fb.Bwd) btCylinder4 .Text = "Index Rear Clamp OPEN";
            else                                              btCylinder4 .Text = "Index Rear Clamp CLOSE";
            btCylinder4.ForeColor =  SM.CL_Complete(ci.IDXR_ClampClOp) ? Color.Black : Color.Lime ;
                                                                          
            if(SM.CL_GetCmd(ci.IDXR_ClampUpDn    ) == fb.Bwd) btCylinder5 .Text = "Index Rear Clamp DOWN";
            else                                              btCylinder5 .Text = "Index Rear Clamp UP";
            btCylinder5.ForeColor =  SM.CL_Complete(ci.IDXR_ClampUpDn) ? Color.Black : Color.Lime ;
                                                                          
            if(SM.CL_GetCmd(ci.IDXF_ClampClOp    ) == fb.Bwd) btCylinder6 .Text = "Index Front Clamp OPEN";
            else                                              btCylinder6 .Text = "Index Front Clamp CLOSE";
            btCylinder6.ForeColor =  SM.CL_Complete(ci.IDXF_ClampClOp) ? Color.Black : Color.Lime ;
                                                                          
            if(SM.CL_GetCmd(ci.IDXF_ClampUpDn    ) == fb.Bwd) btCylinder3 .Text = "Index Front Clamp DOWN";
            else                                              btCylinder3 .Text = "Index Front Clamp UP";
            btCylinder3.ForeColor =  SM.CL_Complete(ci.IDXF_ClampUpDn) ? Color.Black : Color.Lime ;
                                                                          
            if(SM.CL_GetCmd(ci.STCK_RailClOp     ) == fb.Bwd) btCylinder7 .Text = "Tray Rail      OPEN";
            else                                              btCylinder7 .Text = "Tray Rail      CLOSE";
            btCylinder7.ForeColor =  SM.CL_Complete(ci.STCK_RailClOp) ? Color.Black : Color.Lime ;
                                                                          
            if(SM.CL_GetCmd(ci.STCK_RailTrayUpDn ) == fb.Bwd) btCylinder8 .Text = "Stacker Rail Tray DOWN";
            else                                              btCylinder8 .Text = "Stacker Rail Tray UP";
            btCylinder8.ForeColor =  SM.CL_Complete(ci.STCK_RailTrayUpDn) ? Color.Black : Color.Lime ;
                                                                          
            if(SM.CL_GetCmd(ci.STCK_StackStprUpDn) == fb.Bwd) btCylinder9 .Text = "Stacker Stopper DOWN";
            else                                              btCylinder9 .Text = "Stacker Stopper UP";
            btCylinder9.ForeColor =  SM.CL_Complete(ci.STCK_StackStprUpDn) ? Color.Black : Color.Lime ;

            if(SM.CL_GetCmd(ci.STCK_StackOpCl    ) == fb.Bwd) btCylinder10.Text = "Stacker Rail     CLOSE";
            else                                              btCylinder10.Text = "Stacker Rail     OPEN";
            btCylinder10.ForeColor =  SM.CL_Complete(ci.STCK_StackOpCl) ? Color.Black : Color.Lime ;

            if(SM.CL_GetCmd(ci.BARZ_BrcdStprUpDn ) == fb.Bwd) btCylinder11.Text = "Barcode Stopper DOWN";
            else                                              btCylinder11.Text = "Barcode Stopper UP";
            btCylinder11.ForeColor =  SM.CL_Complete(ci.BARZ_BrcdStprUpDn) ? Color.Black : Color.Lime ;

            if(SM.CL_GetCmd(ci.BARZ_BrcdTrayUpDn ) == fb.Bwd) btCylinder12.Text = "Barcode Tray DOWN";
            else                                              btCylinder12.Text = "Barcode Tray UP";
            btCylinder12.ForeColor =  SM.CL_Complete(ci.BARZ_BrcdTrayUpDn) ? Color.Black : Color.Lime ;

            if(SM.CL_GetCmd(ci.BARZ_YPckrFwBw    ) == fb.Bwd) btCylinder13.Text = "Barcode Picker BWD";
            else                                              btCylinder13.Text = "Barcode Picker FWD";
            btCylinder13.ForeColor =  SM.CL_Complete(ci.BARZ_YPckrFwBw)    ? Color.Black : Color.Lime ;

            btManual1.ForeColor=(MM.GetManNo() == mc.LODR_Home) ? Color.Lime : Color.Black ;
            btManual2.ForeColor=(MM.GetManNo() == mc.TOOL_Home) ? Color.Lime : Color.Black ;
            btManual3.ForeColor=(MM.GetManNo() == mc.BARZ_Home) ? Color.Lime : Color.Black ;
            btManual4.ForeColor=(MM.GetManNo() == mc.IDXR_Home) ? Color.Lime : Color.Black ;
            btManual5.ForeColor=(MM.GetManNo() == mc.IDXF_Home) ? Color.Lime : Color.Black ;
            btManual6.ForeColor=(MM.GetManNo() == mc.STCK_Home) ? Color.Lime : Color.Black ;
                    
            Refresh();
            tmUpdate.Enabled = true;
        }

        private void btLotOpen_Click(object sender, EventArgs e)
        {
            if (COracle.bMakingDMC1List)
            {
                Log.ShowMessage("OracleDB" , "Please Wait for Finish Making DMC1 List!");
                return ;
            }
            if (COracle.bMakingPanelList)
            {
                Log.ShowMessage("OracleDB" , "Please Wait for Finish Making PanelID List!");
                return ;
            }
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
            lvLotInfo.Sorting = SortOrder.Descending;
            lvLotInfo.Scrollable = true;

            lvLotInfo.Columns.Add("", 200, HorizontalAlignment.Left);     //1280*1024는 109, 1024*768은 78
            lvLotInfo.Columns.Add("", 300, HorizontalAlignment.Left);     //1280*1024는 109, 1024*768은 78
        
            const int LotInfoCnt = 7;   
            ListViewItem[] liLotInfo = new ListViewItem[LotInfoCnt];
        
            for (int i = 0; i < LotInfoCnt; i++)
            {
                liLotInfo[i] = new ListViewItem();
                liLotInfo[i].SubItems.Add("");
        
        
                liLotInfo[i].UseItemStyleForSubItems = false;
                liLotInfo[i].UseItemStyleForSubItems = false;

                liLotInfo[i].BackColor = Color.White;
                lvLotInfo.Items.Add(liLotInfo[i]);
                
            }
        
            var PropLotInfo = lvLotInfo.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            PropLotInfo.SetValue(lvLotInfo, true, null);
        }

         
        //public void DispDayList() //오퍼레이션 창용.
        //{
        //    lvDayInfo.Clear();
        //    lvDayInfo.View = View.Details;
        //    lvDayInfo.LabelEdit = true;
        //    lvDayInfo.AllowColumnReorder = true;
        //    lvDayInfo.FullRowSelect = true;
        //    lvDayInfo.GridLines = true;
        //    lvDayInfo.Sorting = SortOrder.Descending;
        //    lvDayInfo.Scrollable = true;

        //    lvDayInfo.Columns.Add("", 105, HorizontalAlignment.Left);    //1280*1024는 109, 1024*768은 78
        //    lvDayInfo.Columns.Add("", 105, HorizontalAlignment.Left);    //1280*1024는 109, 1024*768은 78
            
        //    ListViewItem[] liDayInfo = new ListViewItem[DayInfoCnt];
            
        //    for (int i = 0; i < DayInfoCnt; i++)
        //    {
        //        liDayInfo[i] = new ListViewItem();
        //        liDayInfo[i].SubItems.Add("");


        //        liDayInfo[i].UseItemStyleForSubItems = false;
        //        liDayInfo[i].UseItemStyleForSubItems = false;

        //        lvDayInfo.Items.Add(liDayInfo[i]);
                
        //    }
            
        //    var PropDayInfo = lvDayInfo.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
        //    PropDayInfo.SetValue(lvDayInfo, true, null);
            
        //    if (lvDayInfo == null) return;
        //}

        private void btStart_Click(object sender, EventArgs e)
        {
            
        }

        private void btStop_Click(object sender, EventArgs e)
        {
            
            //OM.EqpStat.iLDRSplyCnt  = SEQ.LODR.iLDRSplyCnt;
            SEQ._bBtnStop = true;
        }

        private void btReset_Click(object sender, EventArgs e)
        {
           
            SEQ._bBtnReset = true;
            
            
        }

        private void lvDayInfo_MouseDoubleClick(object sender, MouseEventArgs e)  //요거는 확인 해봐야 함 진섭
        {
            if(FormPassword.GetLevel() != EN_LEVEL.Master) return ;

            if (Log.ShowMessageModal("Confirm", "Clear Day Info?") != DialogResult.Yes) return;
            
            //DayData.ClearData() ;
        }

        private void btAllHome_Click(object sender, EventArgs e)
        {
            if (!OM.MstOptn.bDebugMode)
            {
                if (Log.ShowMessageModal("Confirm", "Do you want to All Homming?") != DialogResult.Yes) return;
                MM.SetManCycle(mc.AllHome);
            }
            else
            {
                DialogResult Rslt ;
                Rslt = MessageBox.Show(new Form(){TopMost = true}, "홈동작을 생략 하겠습니까?", "Confirm", MessageBoxButtons.YesNoCancel);
                if (Rslt == DialogResult.Yes)
                {
                    SM.MT_SetServoAll(true);
                    Thread.Sleep(100);
                    for (mi i = 0; i < mi.MAX_MOTR; i++)
                    {
                        SM.MT_SetHomeDone(i, true);
                    }
                }
                else if(Rslt == DialogResult.No)
                {
                    MM.SetManCycle(mc.AllHome);
                }

            }

            
        }
        bool bRepeat = false;
        private void lbWorkNo_Click(object sender, EventArgs e)
        {
            //bRepeat = !bRepeat;
        }

        private void FormOperation_FormClosing(object sender, FormClosingEventArgs e)
        {
            SEQ.Oracle.SetSendMsgFunc(null);
            tmUpdate.Enabled = false;
            DM.SaveMap();
        }

        private void FormOperation_Shown(object sender, EventArgs e)
        {
        }

        private void pnOption1_DoubleClick(object sender, EventArgs e)
        {
            int iBtnTag = Convert.ToInt32(((Panel)sender).Tag);
            switch (iBtnTag)
            {
                default : break;
                //case 1: OM.CmnOptn.bIgnrDoor    = !OM.CmnOptn.bIgnrDoor   ; break;
                //case 2: OM.CmnOptn.bVisnSkip    = !OM.CmnOptn.bVisnSkip   ; break;
                //case 3: OM.CmnOptn.bMrkingSkip  = !OM.CmnOptn.bMrkingSkip ; break;
            }
            OM.SaveCmnOptn();
        }

        private void lbOption1_DoubleClick(object sender, EventArgs e)
        {
            int iBtnTag = Convert.ToInt32(((Label)sender).Tag);
            switch (iBtnTag)
            {
                default : break;
                //case 1: OM.CmnOptn.bIgnrDoor    = !OM.CmnOptn.bIgnrDoor   ; break;
                //case 2: OM.CmnOptn.bVisnSkip    = !OM.CmnOptn.bVisnSkip   ; break;
                //case 3: OM.CmnOptn.bMrkingSkip  = !OM.CmnOptn.bMrkingSkip ; break;
            }
            OM.SaveCmnOptn();
        }

        private void btManual1_Click(object sender, EventArgs e)
        {
            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);
            MM.SetManCycle((mc)iBtnTag);

        }

        private void panel11_Paint(object sender, PaintEventArgs e)
        {
        }

        int iActionPreStep = -1 ;
        int iActionStep = 0 ;
        int iActionID  = 0 ;
        int iPreActionID = 0 ;
        CDelayTimer m_tmCycle = new CDelayTimer();
        private void tmRot_Tick(object sender, EventArgs e)
        {
            if (iActionID != 0 && iActionID != iPreActionID)
            {
                iActionStep = 10 ;
                m_tmCycle.Clear();
            }
            iPreActionID = iActionID ;

            if(iActionID == 0 )return ;

            string sTemp ="";
            if (m_tmCycle.OnDelay(iActionStep != 0 && iActionStep == iActionPreStep, 4000))
            {
                sTemp = string.Format("iActionStep={0:00}", iActionStep);
                Log.Trace("OperAction TimeOver", sTemp);
                iActionID = 0 ;
            }
            
            iActionPreStep = iActionStep ;            
            switch (iActionStep) {

                default: 
                    sTemp = string.Format("Cycle Default iActionStep={0:00}", iActionStep);
                    if(iActionPreStep == iActionStep)Log.Trace("OperAction", sTemp);
                    iActionID = 0 ;
                    return ;
            
                case 10:
                    if (iActionID == 1) {iActionStep = 100; }
                    if (iActionID == 2) {iActionStep = 200; }
                    if (iActionID == 3) {iActionStep = 300; }
                    if (iActionID == 4) {iActionStep = 400; }
                    return ;

                //Unclamp IDXF
                case 100:
                    SEQ.IDXF.MoveCyl(ci.IDXF_ClampClOp , fb.Bwd);
                    iActionStep++;
                    return ;

                case 101:
                    if(!SM.CL_Complete(ci.IDXF_ClampClOp , fb.Bwd))return ;
                    SEQ.IDXF.MoveCyl(ci.IDXF_ClampUpDn , fb.Bwd);
                    iActionStep++;
                    return ;

                case 102:
                    if(!SM.CL_Complete(ci.IDXF_ClampUpDn , fb.Bwd))return ;
                    iActionID = 0 ;
                    return ;

                //Unclamp IDXR
                case 200:
                    SEQ.IDXR.MoveCyl(ci.IDXR_ClampClOp , fb.Bwd);
                    iActionStep++;
                    return ;

                case 201:
                    if(!SM.CL_Complete(ci.IDXR_ClampClOp , fb.Bwd))return ;
                    SEQ.IDXR.MoveCyl(ci.IDXR_ClampUpDn , fb.Bwd);
                    iActionStep++;
                    return ;

                case 202:
                    if(!SM.CL_Complete(ci.IDXR_ClampUpDn , fb.Bwd))return ;
                    iActionID = 0 ;
                    return ;

                //clamp IDXF
                case 300:
                    SEQ.IDXF.MoveCyl(ci.IDXF_ClampUpDn , fb.Fwd);
                    iActionStep++;
                    return ;

                case 301:
                    if(!SM.CL_Complete(ci.IDXF_ClampUpDn , fb.Fwd))return ;
                    SEQ.IDXF.MoveCyl(ci.IDXF_ClampClOp , fb.Fwd);
                    iActionStep++;
                    return ;

                case 302:
                    if(!SM.CL_Complete(ci.IDXF_ClampClOp , fb.Fwd))return ;
                    iActionID = 0 ;
                    return ;


                //clamp IDXR
                case 400:
                    SEQ.IDXR.MoveCyl(ci.IDXR_ClampUpDn , fb.Fwd);
                    iActionStep++;
                    return ;

                case 401:
                    if(!SM.CL_Complete(ci.IDXR_ClampUpDn , fb.Fwd))return ;
                    SEQ.IDXR.MoveCyl(ci.IDXR_ClampClOp , fb.Fwd);
                    iActionStep++;
                    return ;

                case 402:
                    if(!SM.CL_Complete(ci.IDXR_ClampClOp , fb.Fwd))return ;
                    iActionID = 0 ;
                    return ;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            
        }

        private void btInputTrayG_Click(object sender, EventArgs e)
        {
            DM.ARAY[ri.TRYG].SetStat(cs.Good);
            //for (int c = 0; c < OM.DevInfo.iTRAY_PcktCntX; c++)
            //{
            //    for (int r = 0; r < OM.DevInfo.iTRAY_PcktCntY; r++)
            //    {
            //        if (DM.ARAY[ri.MASK].GetStat(c, r) == cs.None)
            //        {
            //            DM.ARAY[ri.TRYG].SetStat(c, r, cs.None);
            //        }
            //    }
            //}
        }

        private void btInputTrayF_Click(object sender, EventArgs e)
        {
            DM.ARAY[ri.TRYF].SetStat(cs.Empty);
            //for (int c = 0; c < OM.DevInfo.iTRAY_PcktCntX; c++)
            //{
            //    for (int r = 0; r < OM.DevInfo.iTRAY_PcktCntY; r++)
            //    {
            //        if (DM.ARAY[ri.MASK].GetStat(c, r) == cs.None)
            //        {
            //            DM.ARAY[ri.TRYF].SetStat(c, r, cs.None);
            //        }
            //    }
            //}
        }

        private void btLightOnOff_Click(object sender, EventArgs e)
        {
            SM.IO_SetY(yi.ETC_LightOnOff, !SM.IO_GetY(yi.ETC_LightOnOff));
        }
		
		
        private void btCylinder1_Click(object sender, EventArgs e)
        {
            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);

            if(iBtnTag == (int)ci.LODR_ClampClOp      ){SEQ.LODR.MoveCyl((ci)iBtnTag ,SM.CL_GetCmd((ci)iBtnTag) == fb.Bwd ? fb.Fwd : fb.Bwd);}
            if(iBtnTag == (int)ci.LODR_SperatorUpDn   ){SEQ.LODR.MoveCyl((ci)iBtnTag ,SM.CL_GetCmd((ci)iBtnTag) == fb.Bwd ? fb.Fwd : fb.Bwd);}
            if(iBtnTag == (int)ci.STCK_RailClOp       ){SEQ.STCK.MoveCyl((ci)iBtnTag ,SM.CL_GetCmd((ci)iBtnTag) == fb.Bwd ? fb.Fwd : fb.Bwd);}
            if(iBtnTag == (int)ci.IDXR_ClampUpDn      ){SEQ.IDXR.MoveCyl((ci)iBtnTag ,SM.CL_GetCmd((ci)iBtnTag) == fb.Bwd ? fb.Fwd : fb.Bwd);}
            if(iBtnTag == (int)ci.IDXF_ClampUpDn      ){SEQ.IDXF.MoveCyl((ci)iBtnTag ,SM.CL_GetCmd((ci)iBtnTag) == fb.Bwd ? fb.Fwd : fb.Bwd);}
            if(iBtnTag == (int)ci.IDXR_ClampClOp      ){SEQ.IDXR.MoveCyl((ci)iBtnTag ,SM.CL_GetCmd((ci)iBtnTag) == fb.Bwd ? fb.Fwd : fb.Bwd);}
            if(iBtnTag == (int)ci.IDXF_ClampClOp      ){SEQ.IDXF.MoveCyl((ci)iBtnTag ,SM.CL_GetCmd((ci)iBtnTag) == fb.Bwd ? fb.Fwd : fb.Bwd);}
            if(iBtnTag == (int)ci.STCK_RailTrayUpDn   ){SEQ.STCK.MoveCyl((ci)iBtnTag ,SM.CL_GetCmd((ci)iBtnTag) == fb.Bwd ? fb.Fwd : fb.Bwd);}
            if(iBtnTag == (int)ci.STCK_StackStprUpDn  ){SEQ.STCK.MoveCyl((ci)iBtnTag ,SM.CL_GetCmd((ci)iBtnTag) == fb.Bwd ? fb.Fwd : fb.Bwd);}
            if(iBtnTag == (int)ci.STCK_StackOpCl      ){SEQ.STCK.MoveCyl((ci)iBtnTag ,SM.CL_GetCmd((ci)iBtnTag) == fb.Bwd ? fb.Fwd : fb.Bwd);}
            if(iBtnTag == (int)ci.BARZ_BrcdStprUpDn   ){SEQ.BARZ.MoveCyl((ci)iBtnTag ,SM.CL_GetCmd((ci)iBtnTag) == fb.Bwd ? fb.Fwd : fb.Bwd);}
            if(iBtnTag == (int)ci.BARZ_BrcdTrayUpDn   ){SEQ.BARZ.MoveCyl((ci)iBtnTag ,SM.CL_GetCmd((ci)iBtnTag) == fb.Bwd ? fb.Fwd : fb.Bwd);}
            if(iBtnTag == (int)ci.BARZ_YPckrFwBw      ){SEQ.BARZ.MoveCyl((ci)iBtnTag ,SM.CL_GetCmd((ci)iBtnTag) == fb.Bwd ? fb.Fwd : fb.Bwd);}
            
        }

        private void btLODRBarcodeOn_Click(object sender, EventArgs e)
        {
            int iBtnTag = Convert.ToInt32(((Button)sender).Tag);

            switch (iBtnTag)
            {
                case 1: SEQ.BarcordLODR.SendScanOn (); break;
                case 2: SEQ.BarcordLODR.SendScanOff(); break;
                case 3: SEQ.BarcordBARZ.SendScanOn (); break;
                case 4: SEQ.BarcordBARZ.SendScanOff(); break;
            }
        }

        private void btFrmPrint_Click(object sender, EventArgs e)
        {
            if (FrmPrint.IsDisposed)
            {
                FrmPrint = new FormPrint();
            }
            FrmPrint.Show();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            //if (FrmOracle.IsDisposed)
            //{
            //    FrmOracle = new FormOracle();
            //}
            //FrmOracle.Show();

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
             VisnCom.TRslt Rslt = new VisnCom.TRslt();
             int iIndex = 0;
             for(int i = 0 ; i < OM.DevInfo.iTRAY_PcktCntY ; i++){
             
                     iIndex = i;
                     SEQ.Visn.LoadRslt(i , ref Rslt);
             
                      if(Rslt.Empty        ==0   ){DM.ARAY[ri.INSP].SetStat(0,iIndex,cs.NG0 );} 
                 else if(Rslt.MixDevice    ==0   ){DM.ARAY[ri.INSP].SetStat(0,iIndex,cs.NG1 );}
                 else if(Rslt.UnitID       =="0" ){DM.ARAY[ri.INSP].SetStat(0,iIndex,cs.NG2 );}//바코드들은 "0"이면 있는데 못읽음. "1"바코드가 검사에 없음. "바코드문자" 읽기 성공.
                 else if(Rslt.UnitDMC1     =="0" ){DM.ARAY[ri.INSP].SetStat(0,iIndex,cs.NG3 );}
                 else if(Rslt.UnitDMC2     =="0" ){DM.ARAY[ri.INSP].SetStat(0,iIndex,cs.NG4 );}
                 else if(Rslt.GlobtopLeft  ==0   ){DM.ARAY[ri.INSP].SetStat(0,iIndex,cs.NG5 );}
                 else if(Rslt.GlobtopTop   ==0   ){DM.ARAY[ri.INSP].SetStat(0,iIndex,cs.NG6 );}
                 else if(Rslt.GlobtopRight ==0   ){DM.ARAY[ri.INSP].SetStat(0,iIndex,cs.NG7 );}
                 else if(Rslt.GlobtopBottom==0   ){DM.ARAY[ri.INSP].SetStat(0,iIndex,cs.NG8 );} 
                 else if(Rslt.MatchingError==0   ){DM.ARAY[ri.INSP].SetStat(0,iIndex,cs.NG9 );} 
                 else if(Rslt.UserDefine   ==0   ){DM.ARAY[ri.INSP].SetStat(0,iIndex,cs.NG10);} 
             
             
             }
        }
        private void btAllEmptyTray_Click(object sender, EventArgs e)
        {
            //OM.EqpStat.iLDRSplyCnt = 2;
            if(!DM.ARAY[ri.IDXF].CheckAllStat(cs.None) && !DM.ARAY[ri.IDXF].CheckAllStat(cs.Unknown))DM.ARAY[ri.IDXF].SetStat(cs.Unknown);
            if(!DM.ARAY[ri.IDXR].CheckAllStat(cs.None) && !DM.ARAY[ri.IDXR].CheckAllStat(cs.Unknown))DM.ARAY[ri.IDXR].SetStat(cs.Unknown);
            DM.ARAY[ri.OUTZ].SetStat(cs.Good);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DM.ARAY[ri.BARZ].SetStat(cs.Unknown);
        }

        private void btStart_MouseDown(object sender, MouseEventArgs e)
        {
            tmStart.Enabled = true;
            m_tmStartBt.Clear();
            lbStartDn.Visible = true ;
            
        }

        private void btStart_MouseUp(object sender, MouseEventArgs e)
        {
            tmStart.Enabled = false;
            lbStartDn.Visible = false ;
        }

        private void tmStart_Tick(object sender, EventArgs e)
        {
            lbStartDn.Text = m_tmStartBt.GetCrntGapTime().ToString();
            if(m_tmStartBt.OnDelay(800)){
                SEQ._bBtnStart = true;
                tmStart.Enabled = false;
            }
            

        }

        private void btStart_Click_1(object sender, EventArgs e)
        {
            //SM.ER_SetErrDisp(false);
            //SM.ER_SetErr(ei.ETC_001);
        }

        private void lvLog_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //if (Log.ShowMessageModal("Check", "Do you want to clear lists?") != DialogResult.Yes) { return ;}
            //
            //lvLog.Clear();
        }

        
        private void btIDXRUnClamp_Click(object sender, EventArgs e)
        {
            if (Log.ShowMessageModal("Confirm", "Do you want to Move?") != DialogResult.Yes) return;
            iActionID  = 2  ;
      
        }

        private void btIDXFUnClamp_Click(object sender, EventArgs e)
        {
            if (Log.ShowMessageModal("Confirm", "Do you want to Move?") != DialogResult.Yes) return;
            iActionID = 1 ;
        }

        private void btIDXFClamp_Click(object sender, EventArgs e)
        {
            if (Log.ShowMessageModal("Confirm", "Do you want to Move?") != DialogResult.Yes) return;
            iActionID = 3 ;
        }

        private void btIDXRClamp_Click(object sender, EventArgs e)
        {
            if (Log.ShowMessageModal("Confirm", "Do you want to Move?") != DialogResult.Yes) return;
            iActionID = 4 ;
        }

        private void btToolYGoWait_Click(object sender, EventArgs e)
        {
            if (Log.ShowMessageModal("Confirm", "Do you want to Move?") != DialogResult.Yes) return;
            SEQ.TOOL.MoveMotrSlow(mi.TOOL_YTool , pv.TOOL_YToolWait);
        }

        private void btToolYGoNG_Click(object sender, EventArgs e)
        {
            if (Log.ShowMessageModal("Confirm", "Do you want to Move?") != DialogResult.Yes) return;
            SEQ.TOOL.MoveMotrSlow(mi.TOOL_YTool , pv.TOOL_YToolNgTWorkStt);
        }

        private void lbStartDn_Click(object sender, EventArgs e)
        {
            
        }

        private void lbBarVac_Click(object sender, EventArgs e)
        { 
            //SEQ.Oracle.WriteVIT(OM.CmnOptn.sVITFolder                  , 
            //                            DateTime.Now.ToString("MM\\/dd\\/yyyy"),
            //                            OM.CmnOptn.sMachinID                   ,
            //                            SML.FrmLogOn.GetId()                   ,
            //                            OM.EqpStat.sLotSttTime                 ,
            //                            DateTime.Now.ToString("HH:mm:ss")      ,
            //                            (OM.DevInfo.iTRAY_PcktCntX * OM.DevInfo.iTRAY_PcktCntY * OM.DevInfo.iTRAY_StackingCnt).ToString());
            //
        }

        private void label3_Click(object sender, EventArgs e)
        {
                    //SEQ.Visn.FindRsltFile();
                    
                    
                    ////여기 나중에 미리 보내는 것 넣자. 진섭아.
                    ////if(iVisnIndx>=iTotalInspCnt) 
                    //VisnCom.TRslt Rslt = new VisnCom.TRslt();
                    //int iIndex = 0;
                    //for(int i = 0 ; i < OM.DevInfo.iTRAY_PcktCntY ; i++){
                    //    if(true) {
                    //        iIndex = i;
                    //        SEQ.Visn.LoadRslt(i , ref Rslt);
                    //    }
                    //    else { //비전쪽은 첨찍은게 인덱스가 0이다.
                    //        iIndex = OM.DevInfo.iTRAY_PcktCntY - i - 1;
                    //        SEQ.Visn.LoadRslt(i , ref Rslt);
                    //    }
                    //         if(Rslt.Empty        ==0   ){DM.ARAY[ri.INSP].SetStat(0,iIndex,cs.NG0 );} 
                    //    //else if(Rslt.MixDevice    ==0   ){DM.ARAY[ri.INSP].SetStat(0,iIndex,cs.NG1 );} //이것은 비전에서 쓰는 것이 아니고 핸들러에서 오라클 관련 에러 일때 쓰는 것이다.
                    //    else if(Rslt.UnitID       =="0" ){DM.ARAY[ri.INSP].SetStat(0,iIndex,cs.NG2 );}//바코드들은 "0"이면 있는데 못읽음. "1"바코드가 검사에 없음. "바코드문자" 읽기 성공.
                    //    else if(Rslt.UnitDMC1     =="0" ){DM.ARAY[ri.INSP].SetStat(0,iIndex,cs.NG3 );}
                    //    else if(Rslt.UnitDMC2     =="0" ){DM.ARAY[ri.INSP].SetStat(0,iIndex,cs.NG4 );}
                    //    else if(Rslt.GlobtopLeft  ==0   ){DM.ARAY[ri.INSP].SetStat(0,iIndex,cs.NG5 );}
                    //    else if(Rslt.GlobtopTop   ==0   ){DM.ARAY[ri.INSP].SetStat(0,iIndex,cs.NG6 );}
                    //    else if(Rslt.GlobtopRight ==0   ){DM.ARAY[ri.INSP].SetStat(0,iIndex,cs.NG7 );}
                    //    else if(Rslt.GlobtopBottom==0   ){DM.ARAY[ri.INSP].SetStat(0,iIndex,cs.NG8 );} 
                    //    else if(Rslt.MatchingError==0   ){DM.ARAY[ri.INSP].SetStat(0,iIndex,cs.NG9 );} 
                    //    else if(Rslt.UserDefine   ==0   ){DM.ARAY[ri.INSP].SetStat(0,iIndex,cs.NG10);}

                    //    if (!OM.CmnOptn.bIdleRun)
                    //    {
                    //        if (!OM.CmnOptn.bOracleNotUse)//칩에
                    //        {
                    //            if (Rslt.UnitID   != "1")DM.ARAY[ri.INSP].Chip[0,iIndex].sUnitID = Rslt.UnitID   ; 
                    //            if (Rslt.UnitDMC1 != "1")DM.ARAY[ri.INSP].Chip[0,iIndex].sDMC1   = Rslt.UnitDMC1 ; 
                    //            if (Rslt.UnitDMC2 != "1")DM.ARAY[ri.INSP].Chip[0,iIndex].sDMC2   = Rslt.UnitDMC2 ; 
                    //        }
                    //    }

                    //    if(OM.CmnOptn.bIdleRun){
                    //        DM.ARAY[ri.INSP].SetStat(0,iIndex,cs.Good);
                    //    }
  

                    
                    //}
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