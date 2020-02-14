using COMMON;
using System;
using System.IO;
using System.Windows.Forms;

namespace Machine
{
    public partial class FormOption : Form
    {
        //FormMain FrmMain;

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

        }

        private void btSave_Click(object sender, EventArgs e)
        {
            //Check Running Status.
            if (SEQ._bRun) 
            {
                Log.ShowMessage("Warning", "Can't Save during Autorunning!");
                return;
            }

            if (Log.ShowMessageModal("Confirm", "Are you Sure?") != DialogResult.Yes) return;
        
            UpdateComOptn(false);
            OM.SaveCmnOptn();

            //Index Rear                                                           
            //DM.ARAY[ri.IDXR].SetDisp(cs.NG0       , "MixDevice"    ,Color.Coral        );//OM.CmnOptn.sNG0 ,Color.Coral        );
            //DM.ARAY[ri.IDXR].SetDisp(cs.NG1       , "UnitID"       ,Color.DarkOrchid   );//OM.CmnOptn.sNG1 ,Color.DarkOrchid   );
            //DM.ARAY[ri.IDXR].SetDisp(cs.NG2       , "UnitDMC1"     ,Color.DarkTurquoise);//OM.CmnOptn.sNG2 ,Color.DarkTurquoise);
            //DM.ARAY[ri.IDXR].SetDisp(cs.NG3       , "UnitDMC2"     ,Color.Olive        );//OM.CmnOptn.sNG3 ,Color.Olive        );
            //DM.ARAY[ri.IDXR].SetDisp(cs.NG4       , "GlobtopLeft"  ,Color.DeepSkyBlue  );//OM.CmnOptn.sNG4 ,Color.DeepSkyBlue  );
            //DM.ARAY[ri.IDXR].SetDisp(cs.NG5       , "GlobtopTop"   ,Color.Crimson      );//OM.CmnOptn.sNG5 ,Color.Crimson      );
            //DM.ARAY[ri.IDXR].SetDisp(cs.NG6       , "GlobtopRight" ,Color.SlateBlue    );//OM.CmnOptn.sNG6 ,Color.SlateBlue    );
            //DM.ARAY[ri.IDXR].SetDisp(cs.NG7       , "GlobtopBottom",Color.DarkCyan     );//OM.CmnOptn.sNG7 ,Color.DarkCyan     );
            //DM.ARAY[ri.IDXR].SetDisp(cs.NG8       , "Empty"        ,Color.Orange       );//OM.CmnOptn.sNG8 ,Color.Orange       );
            //DM.ARAY[ri.IDXR].SetDisp(cs.NG9       , "MatchingError",Color.DarkKhaki    );//OM.CmnOptn.sNG9 ,Color.DarkKhaki    );
            //DM.ARAY[ri.IDXR].SetDisp(cs.NG10      , "UserDefine"   ,Color.DarkGoldenrod);//OM.CmnOptn.sNG9 ,Color.DarkKhaki    );                                                                                    
                                                                                    
            //Index Front                                                           
            //DM.ARAY[ri.IDXF].SetDisp(cs.NG0       , "MixDevice"    ,Color.Coral        );//OM.CmnOptn.sNG0 ,Color.Coral        );
            //DM.ARAY[ri.IDXF].SetDisp(cs.NG1       , "UnitID"       ,Color.DarkOrchid   );//OM.CmnOptn.sNG1 ,Color.DarkOrchid   );
            //DM.ARAY[ri.IDXF].SetDisp(cs.NG2       , "UnitDMC1"     ,Color.DarkTurquoise);//OM.CmnOptn.sNG2 ,Color.DarkTurquoise);
            //DM.ARAY[ri.IDXF].SetDisp(cs.NG3       , "UnitDMC2"     ,Color.Olive        );//OM.CmnOptn.sNG3 ,Color.Olive        );
            //DM.ARAY[ri.IDXF].SetDisp(cs.NG4       , "GlobtopLeft"  ,Color.DeepSkyBlue  );//OM.CmnOptn.sNG4 ,Color.DeepSkyBlue  );
            //DM.ARAY[ri.IDXF].SetDisp(cs.NG5       , "GlobtopTop"   ,Color.Crimson      );//OM.CmnOptn.sNG5 ,Color.Crimson      );
            //DM.ARAY[ri.IDXF].SetDisp(cs.NG6       , "GlobtopRight" ,Color.SlateBlue    );//OM.CmnOptn.sNG6 ,Color.SlateBlue    );
            //DM.ARAY[ri.IDXF].SetDisp(cs.NG7       , "GlobtopBottom",Color.DarkCyan     );//OM.CmnOptn.sNG7 ,Color.DarkCyan     );
            //DM.ARAY[ri.IDXF].SetDisp(cs.NG8       , "Empty"        ,Color.Orange       );//OM.CmnOptn.sNG8 ,Color.Orange       );
            //DM.ARAY[ri.IDXF].SetDisp(cs.NG9       , "MatchingError",Color.DarkKhaki    );//OM.CmnOptn.sNG9 ,Color.DarkKhaki    );
            //DM.ARAY[ri.IDXF].SetDisp(cs.NG10      , "UserDefine"   ,Color.DarkGoldenrod);//
            //Picker                                               
            //DM.ARAY[ri.PCKR].SetDisp(cs.NG0       , "MixDevice"    ,Color.Coral        );//OM.CmnOptn.sNG0 ,Color.Coral        );
            //DM.ARAY[ri.PCKR].SetDisp(cs.NG1       , "UnitID"       ,Color.DarkOrchid   );//OM.CmnOptn.sNG1 ,Color.DarkOrchid   );
            //DM.ARAY[ri.PCKR].SetDisp(cs.NG2       , "UnitDMC1"     ,Color.DarkTurquoise);//OM.CmnOptn.sNG2 ,Color.DarkTurquoise);
            //DM.ARAY[ri.PCKR].SetDisp(cs.NG3       , "UnitDMC2"     ,Color.Olive        );//OM.CmnOptn.sNG3 ,Color.Olive        );
            //DM.ARAY[ri.PCKR].SetDisp(cs.NG4       , "GlobtopLeft"  ,Color.DeepSkyBlue  );//OM.CmnOptn.sNG4 ,Color.DeepSkyBlue  );
            //DM.ARAY[ri.PCKR].SetDisp(cs.NG5       , "GlobtopTop"   ,Color.Crimson      );//OM.CmnOptn.sNG5 ,Color.Crimson      );
            //DM.ARAY[ri.PCKR].SetDisp(cs.NG6       , "GlobtopRight" ,Color.SlateBlue    );//OM.CmnOptn.sNG6 ,Color.SlateBlue    );
            //DM.ARAY[ri.PCKR].SetDisp(cs.NG7       , "GlobtopBottom",Color.DarkCyan     );//OM.CmnOptn.sNG7 ,Color.DarkCyan     );
            //DM.ARAY[ri.PCKR].SetDisp(cs.NG8       , "Empty"        ,Color.Orange       );//OM.CmnOptn.sNG8 ,Color.Orange       );
            //DM.ARAY[ri.PCKR].SetDisp(cs.NG9       , "MatchingError",Color.DarkKhaki    );//OM.CmnOptn.sNG9 ,Color.DarkKhaki    );
            //DM.ARAY[ri.PCKR].SetDisp(cs.NG10      , "UserDefine"   ,Color.DarkGoldenrod);
                                                                                    
            //Fail Tray                                                           
            //DM.ARAY[ri.TRYF].SetDisp(cs.NG0       , "MixDevice"    ,Color.Coral        );//OM.CmnOptn.sNG0 ,Color.Coral        );
            //DM.ARAY[ri.TRYF].SetDisp(cs.NG1       , "UnitID"       ,Color.DarkOrchid   );//OM.CmnOptn.sNG1 ,Color.DarkOrchid   );
            //DM.ARAY[ri.TRYF].SetDisp(cs.NG2       , "UnitDMC1"     ,Color.DarkTurquoise);//OM.CmnOptn.sNG2 ,Color.DarkTurquoise);
            //DM.ARAY[ri.TRYF].SetDisp(cs.NG3       , "UnitDMC2"     ,Color.Olive        );//OM.CmnOptn.sNG3 ,Color.Olive        );
            //DM.ARAY[ri.TRYF].SetDisp(cs.NG4       , "GlobtopLeft"  ,Color.DeepSkyBlue  );//OM.CmnOptn.sNG4 ,Color.DeepSkyBlue  );
            //DM.ARAY[ri.TRYF].SetDisp(cs.NG5       , "GlobtopTop"   ,Color.Crimson      );//OM.CmnOptn.sNG5 ,Color.Crimson      );
            //DM.ARAY[ri.TRYF].SetDisp(cs.NG6       , "GlobtopRight" ,Color.SlateBlue    );//OM.CmnOptn.sNG6 ,Color.SlateBlue    );
            //DM.ARAY[ri.TRYF].SetDisp(cs.NG7       , "GlobtopBottom",Color.DarkCyan     );//OM.CmnOptn.sNG7 ,Color.DarkCyan     );
            //DM.ARAY[ri.TRYF].SetDisp(cs.NG8       , "Empty"        ,Color.Orange       );//OM.CmnOptn.sNG8 ,Color.Orange       );
            //DM.ARAY[ri.TRYF].SetDisp(cs.NG9       , "MatchingError",Color.DarkKhaki    );//OM.CmnOptn.sNG9 ,Color.DarkKhaki    );
            //DM.ARAY[ri.TRYF].SetDisp(cs.NG10      , "UserDefine"   ,Color.DarkGoldenrod);                                                                                  

            //DM.ARAY[ri.INSP].SetDisp(cs.NG0       , "MixDevice"    ,Color.Coral        );//OM.CmnOptn.sNG0 ,Color.Coral        );
            //DM.ARAY[ri.INSP].SetDisp(cs.NG1       , "UnitID"       ,Color.DarkOrchid   );//OM.CmnOptn.sNG1 ,Color.DarkOrchid   );
            //DM.ARAY[ri.INSP].SetDisp(cs.NG2       , "UnitDMC1"     ,Color.DarkTurquoise);//OM.CmnOptn.sNG2 ,Color.DarkTurquoise);
            //DM.ARAY[ri.INSP].SetDisp(cs.NG3       , "UnitDMC2"     ,Color.Olive        );//OM.CmnOptn.sNG3 ,Color.Olive        );
            //DM.ARAY[ri.INSP].SetDisp(cs.NG4       , "GlobtopLeft"  ,Color.DeepSkyBlue  );//OM.CmnOptn.sNG4 ,Color.DeepSkyBlue  );
            //DM.ARAY[ri.INSP].SetDisp(cs.NG5       , "GlobtopTop"   ,Color.Crimson      );//OM.CmnOptn.sNG5 ,Color.Crimson      );
            //DM.ARAY[ri.INSP].SetDisp(cs.NG6       , "GlobtopRight" ,Color.SlateBlue    );//OM.CmnOptn.sNG6 ,Color.SlateBlue    );
            //DM.ARAY[ri.INSP].SetDisp(cs.NG7       , "GlobtopBottom",Color.DarkCyan     );//OM.CmnOptn.sNG7 ,Color.DarkCyan     );
            //DM.ARAY[ri.INSP].SetDisp(cs.NG8       , "Empty"        ,Color.Orange       );//OM.CmnOptn.sNG8 ,Color.Orange       );
            //DM.ARAY[ri.INSP].SetDisp(cs.NG9       , "MatchingError",Color.DarkKhaki    );//OM.CmnOptn.sNG9 ,Color.DarkKhaki    );
            //DM.ARAY[ri.INSP].SetDisp(cs.NG10      , "UserDefine"   ,Color.DarkGoldenrod);  
        } 
        
        public void UpdateComOptn(bool _bToTable)
        {                                                                                   
            if (_bToTable == true) 
            {
                CConfig.ValToCon(cbLoadingStop       , ref OM.CmnOptn.bLoadingStop           );
                CConfig.ValToCon(cbIdleRun           , ref OM.CmnOptn.bIdleRun               );
                CConfig.ValToCon(cbEmptyCheck        , ref OM.CmnOptn.iEmptyCheckPrcs        );
                CConfig.ValToCon(tbNG0               , ref OM.CmnOptn.sNG0                   );
                CConfig.ValToCon(tbNG1               , ref OM.CmnOptn.sNG1                   );
                CConfig.ValToCon(tbNG2               , ref OM.CmnOptn.sNG2                   );
                CConfig.ValToCon(tbNG3               , ref OM.CmnOptn.sNG3                   );
                CConfig.ValToCon(tbNG4               , ref OM.CmnOptn.sNG4                   );
                CConfig.ValToCon(tbNG5               , ref OM.CmnOptn.sNG5                   );
                CConfig.ValToCon(tbNG6               , ref OM.CmnOptn.sNG6                   );
                CConfig.ValToCon(tbNG7               , ref OM.CmnOptn.sNG7                   );
                CConfig.ValToCon(tbNG8               , ref OM.CmnOptn.sNG8                   );
                CConfig.ValToCon(tbNG9               , ref OM.CmnOptn.sNG9                   );    
                CConfig.ValToCon(tbNG10              , ref OM.CmnOptn.sNG10                  );    
                                                                                             
                CConfig.ValToCon(cbOracleNotUse      , ref OM.CmnOptn.bOracleNotUse          );    
                CConfig.ValToCon(tbOracleIP          , ref OM.CmnOptn.sOracleIP              );    
                CConfig.ValToCon(tbOraclePort        , ref OM.CmnOptn.sOraclePort            );    
                CConfig.ValToCon(tbOracleID          , ref OM.CmnOptn.sOracleID              );    
                CConfig.ValToCon(tbOraclePassword    , ref OM.CmnOptn.sOraclePassword        );
                CConfig.ValToCon(tbOracleSID         , ref OM.CmnOptn.sOracleSID             );
                                                     
                CConfig.ValToCon(cbNotWriteVIT       , ref OM.CmnOptn.bOracleNotWriteVIT     );
                CConfig.ValToCon(cbNotWriteUnitInsp  , ref OM.CmnOptn.bOracleNotWriteInsp    );
                                                     
                CConfig.ValToCon(cbNotWriteVITFile   , ref OM.CmnOptn.bOracleNotWriteVITFile );
                CConfig.ValToCon(cbUseApTestTable    , ref OM.CmnOptn.bUseApTestTable        );
                                                                                             
                CConfig.ValToCon(tbIdxFSplyPos       , ref OM.CmnOptn.dIdxFSplyPos           );
                CConfig.ValToCon(tbIdxRSplyPos       , ref OM.CmnOptn.dIdxRSplyPos           );
                                                                                             
                                                                                             
                CConfig.ValToCon(tbYOffset           , ref OM.CmnOptn.iBarcYOffset           );
                CConfig.ValToCon(tbToff              , ref OM.CmnOptn.iBarcToff              );
                CConfig.ValToCon(tbBrcdPickDelay     , ref OM.CmnOptn.iBrcdPickDelay         );
                                                                                             
                CConfig.ValToCon(cbIdxFSkip          , ref OM.CmnOptn.bIdxFSkip              );
                CConfig.ValToCon(cbIdxRSkip          , ref OM.CmnOptn.bIdxRSkip              );
                                                                                             
                CConfig.ValToCon(tbGoodPickMissCnt   , ref OM.CmnOptn.iGoodPickMissCnt       );
                                                                                             
                CConfig.ValToCon(cbUsedGoldenTray    , ref OM.CmnOptn.bGoldenTray            );
                CConfig.ValToCon(cbInspCrvrTray      , ref OM.CmnOptn.iInspCrvrTray          );
                                                                                             
                CConfig.ValToCon(tbPickRtryCnt       , ref OM.CmnOptn.iPickRtryCnt           );
                CConfig.ValToCon(tbLotEndDelay       , ref OM.CmnOptn.iLotEndDelay           );
                CConfig.ValToCon(tbBackupFolder      , ref OM.CmnOptn.sBackupFolder          );
                CConfig.ValToCon(tbOracleMachinID    , ref OM.CmnOptn.sMachinID              );
                CConfig.ValToCon(tbVITFolder         , ref OM.CmnOptn.sVITFolder             ); 
                CConfig.ValToCon(cbSkipBarAttach     , ref OM.CmnOptn.bSkipBarAttach         ); 
                CConfig.ValToCon(cbIdxDetectVisnZone , ref OM.CmnOptn.bIdxDetectVisnZone     ); 
 
            }
            else 
            {
                CConfig.ConToVal(cbLoadingStop       , ref OM.CmnOptn.bLoadingStop             );
                CConfig.ConToVal(cbIdleRun           , ref OM.CmnOptn.bIdleRun                 );
                CConfig.ConToVal(cbEmptyCheck        , ref OM.CmnOptn.iEmptyCheckPrcs          );
                CConfig.ConToVal(tbNG0               , ref OM.CmnOptn.sNG0                     );
                CConfig.ConToVal(tbNG1               , ref OM.CmnOptn.sNG1                     );
                CConfig.ConToVal(tbNG2               , ref OM.CmnOptn.sNG2                     );
                CConfig.ConToVal(tbNG3               , ref OM.CmnOptn.sNG3                     );
                CConfig.ConToVal(tbNG4               , ref OM.CmnOptn.sNG4                     );
                CConfig.ConToVal(tbNG5               , ref OM.CmnOptn.sNG5                     );
                CConfig.ConToVal(tbNG6               , ref OM.CmnOptn.sNG6                     );
                CConfig.ConToVal(tbNG7               , ref OM.CmnOptn.sNG7                     );
                CConfig.ConToVal(tbNG8               , ref OM.CmnOptn.sNG8                     );
                CConfig.ConToVal(tbNG9               , ref OM.CmnOptn.sNG9                     );
                CConfig.ConToVal(tbNG10              , ref OM.CmnOptn.sNG10                    );    
                                                                                               
                CConfig.ConToVal(cbOracleNotUse      , ref OM.CmnOptn.bOracleNotUse            );    
                CConfig.ConToVal(tbOracleIP          , ref OM.CmnOptn.sOracleIP                );    
                CConfig.ConToVal(tbOraclePort        , ref OM.CmnOptn.sOraclePort              );    
                CConfig.ConToVal(tbOracleID          , ref OM.CmnOptn.sOracleID                );    
                CConfig.ConToVal(tbOraclePassword    , ref OM.CmnOptn.sOraclePassword          );  
                CConfig.ConToVal(tbOracleSID         , ref OM.CmnOptn.sOracleSID               );
                CConfig.ConToVal(tbOracleMachinID    , ref OM.CmnOptn.sMachinID                );
                                                                                               
                CConfig.ConToVal(cbNotWriteVIT       , ref OM.CmnOptn.bOracleNotWriteVIT       );    
                CConfig.ConToVal(cbNotWriteUnitInsp  , ref OM.CmnOptn.bOracleNotWriteInsp      );  
                                                     
                CConfig.ConToVal(cbNotWriteVITFile   , ref OM.CmnOptn.bOracleNotWriteVITFile   );
                CConfig.ConToVal(cbUseApTestTable    , ref OM.CmnOptn.bUseApTestTable          );
                                                                                               
                CConfig.ConToVal(tbIdxFSplyPos       , ref OM.CmnOptn.dIdxFSplyPos             );
                CConfig.ConToVal(tbIdxRSplyPos       , ref OM.CmnOptn.dIdxRSplyPos             ); 
                                                                                               
                                                                                               
                CConfig.ConToVal(tbYOffset           , ref OM.CmnOptn.iBarcYOffset             );
                CConfig.ConToVal(tbToff              , ref OM.CmnOptn.iBarcToff                );
                CConfig.ConToVal(tbBrcdPickDelay     , ref OM.CmnOptn.iBrcdPickDelay           ); 
                                                                                               
                CConfig.ConToVal(cbIdxFSkip          , ref OM.CmnOptn.bIdxFSkip                ); 
                CConfig.ConToVal(cbIdxRSkip          , ref OM.CmnOptn.bIdxRSkip                ); 
                                                     
                CConfig.ConToVal(tbGoodPickMissCnt   , ref OM.CmnOptn.iGoodPickMissCnt         );
                                                                                               
                CConfig.ConToVal(cbUsedGoldenTray    , ref OM.CmnOptn.bGoldenTray              );
                CConfig.ConToVal(cbInspCrvrTray      , ref OM.CmnOptn.iInspCrvrTray            );
                                                                                               
                CConfig.ConToVal(tbPickRtryCnt       , ref OM.CmnOptn.iPickRtryCnt             );
                CConfig.ConToVal(tbLotEndDelay       , ref OM.CmnOptn.iLotEndDelay             );
                                                                                               
                CConfig.ConToVal(tbBackupFolder      , ref OM.CmnOptn.sBackupFolder            );
                CConfig.ConToVal(tbVITFolder         , ref OM.CmnOptn.sVITFolder               );
                CConfig.ConToVal(cbSkipBarAttach     , ref OM.CmnOptn.bSkipBarAttach           ); 
                CConfig.ConToVal(cbIdxDetectVisnZone , ref OM.CmnOptn.bIdxDetectVisnZone       ); 
            }
        }

        private void FormOption_Shown(object sender, EventArgs e)
        {
            UpdateComOptn(true);
            timer1.Enabled = true;
        }

        //public bool bUpdate = false;
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;

            
            timer1.Enabled = true;
        }

        private void btBackupFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog Dlg = new FolderBrowserDialog();
            Dlg.ShowDialog();

            if(Dlg.SelectedPath == "") return ;

            tbBackupFolder.Text = Dlg.SelectedPath ;

        }

        private void btVITFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog Dlg = new FolderBrowserDialog();
            Dlg.ShowDialog();

            if(Dlg.SelectedPath == "") return ;

            tbVITFolder.Text = Dlg.SelectedPath ;
        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
/*
 * SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "csv File|*.csv";
            saveFileDialog1.Title = "Save";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();

                switch (saveFileDialog1.FilterIndex)
                {
                    case 1:

                        string sTmp = "";
                        for (int j = 0; j < _lvTable.Columns.Count ; j++)
                            sTmp += _lvTable.Columns[j].Text + ", ";
                        sTmp += "\n";
                        Byte[] Bytes = Encoding.UTF8.GetBytes(sTmp);
                        fs.Write(Bytes, 0, Bytes.Length);


                        for (int i = 0; i < _lvTable.Items.Count; i++)
                        {
                            sTmp = "";
                            for (int j = 0; j < _lvTable.Items[i].SubItems.Count; j++)
                                sTmp += _lvTable.Items[i].SubItems[j].Text + ", ";
                            sTmp += "\n";
                            Bytes = Encoding.UTF8.GetBytes(sTmp);
                            fs.Write(Bytes, 0, Bytes.Length);
                        }

                        break;
                }

                fs.Close();
            }
 */
