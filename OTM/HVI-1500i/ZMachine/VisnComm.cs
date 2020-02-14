using COMMON;
using SML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Machine
{

    public class VisnCom
    {
        public enum vc : uint
        {
            None      = 0 ,
            Reset     = 1 ,
            LotStart  = 2 ,
            JobChange = 3
        }

        public struct TPara
        {
            public vi Id          ;
            
            public yi yiLotStart  ;
            public yi yiReset     ;
            public yi yiJobStart  ;
            
            public xi xiVisnReady ;
            public xi xiVisnBusy  ;
            public xi xiVisnEnd   ;
        }
        TPara Para ;

        //보내는중인 커멘트 세팅.
        vc  eSendingCmd    ;
        int iPreCycleStep  ;
        int iCycleStep     ;
        string sErrMsg     ;
        string sPara       ;

        CDelayTimer tmTimeOut = new CDelayTimer();
        CDelayTimer tmDelay   = new CDelayTimer();

        public VisnCom(TPara _tPara) 
        {
            Para = _tPara ;
            sErrMsg = "";
        }

        public void Update()
        {
            bool bRet = false;

                 if(eSendingCmd == vc.Reset    ) { if(CycleReset    ()) bRet = true; }
            else if(eSendingCmd == vc.LotStart ) { if(CycleLotStart ()) bRet = true; }
            else if(eSendingCmd == vc.JobChange) { if(CycleJobChange()) bRet = true; }

            if(bRet) {
                string sTemp = Para.Id.ToString() + " End Cycle - " + eSendingCmd.ToString() ;
                if(sErrMsg != "") sTemp += " with err : " + sErrMsg ;
                Log.Trace(sTemp); 
                eSendingCmd = vc.None; 
                iCycleStep = 0 ;
            }
        }
        
        //비전쪽에서 클리어 잘 안되는 문제 있어서 2번 보낸다.
        int iResetSendCnt = 0 ;
        private bool CycleReset()
        {
            if (tmTimeOut.OnDelay(iCycleStep != 0 && iCycleStep == iPreCycleStep , 2000 )) 
            {
                sErrMsg = string.Format(Para.Id.ToString() + " " +eSendingCmd.ToString() + " Cycle TimeOut iCycleStep={0:00}", iCycleStep);
                Log.Trace(sErrMsg);
                ML.IO_SetY(Para.yiReset , false) ;
                ML.ER_SetErr(ei.VSN_ComErr , sErrMsg );
                return true;
            }

            if(iPreCycleStep != iCycleStep)
            {
                string sTemp = string.Format(Para.Id.ToString() + " " +eSendingCmd.ToString() + " Cycle iCycleStep={0:00}", iCycleStep);
                Log.Trace(sTemp);
            }

            iPreCycleStep = iCycleStep ;

            switch (iCycleStep)
            {

                default:
                    sErrMsg = string.Format(Para.Id.ToString() + " " +eSendingCmd.ToString() + " Cycle Default Clear at iCycleStep={0:00}", iCycleStep);
                    Log.Trace(sErrMsg);
                    ML.IO_SetY(Para.yiReset , false) ;
                    ML.ER_SetErr(ei.VSN_ComErr , sErrMsg );
                    return true;

                case 10:
                    iResetSendCnt = 0 ;
                    ML.IO_SetY(Para.yiReset    , false) ;
                    ML.IO_SetY(Para.yiJobStart , false) ;
                    ML.IO_SetY(Para.yiLotStart , false) ;

                    iCycleStep++;
                    return false;

                //여기서 레디 안들어와 있으면.
                case 11:
                    if(!ML.IO_GetX(Para.xiVisnReady))
                    {
                        ML.IO_SetY(Para.yiReset , true) ;
                        tmDelay.Clear();
                        iCycleStep++;
                        return false ;
                    }
                    iCycleStep=14;
                    return false;

                //레디 안들어왔으면 리셑을 켜서 레디를 먼저 한다.
                case 12:
                    if(!tmDelay.OnDelay(50)) return false ;
                    //if(!ML.IO_GetX(Para.xiVisnReady)) return false ;
                    ML.IO_SetY(Para.yiReset , false) ;
                   
                    iCycleStep++;
                    return false ;

                case 13:
                    
                    //if(!tmDelay.OnDelay(500)) return false ;
                    iCycleStep++;
                    return false;


                //여기부터 세팅.
                //밑에서 씀.
                case 14:
                    ML.IO_SetY(Para.yiReset , true) ;
                    iCycleStep++;
                    return false ;

                case 15:
                    if(ML.IO_GetX(Para.xiVisnReady)) return false ; //이게 맞는건가???
                    ML.IO_SetY(Para.yiReset , false) ;
                    iCycleStep++;
                    return false ;

                case 16:
                    if(!ML.IO_GetX(Para.xiVisnReady)) return false ;
                    tmDelay.Clear();
                    iCycleStep++;
                    return false ;

                case 17:
                    if(!tmDelay.OnDelay(100)) return false ;
                    iResetSendCnt++;
                    if(iResetSendCnt == 1) 
                    {
                        //if(!ML.IO_GetX(Para.xiVisnBusy))
                        //{
                            iCycleStep=14 ;
                            return false ;
                        //}
                    }
                    if(iResetSendCnt > 1)
                    {
                        if(ML.IO_GetX(Para.xiVisnBusy))
                        {
                            sErrMsg = "Vision Reset Failed!" ;
                            return true ;
                        }
                    }
                    
                    return true ;
            }
        }
        //LotData1~6
        private bool WriteLot(string _sLotNo)
        {
            //Local Var.
            string     sPath  ;
            string     sData  ;
        
            //Set Path.
            sPath = OM.EqpOptn.sVisnPath ;
            sData = _sLotNo+";" ;
        
            if (!Directory.Exists(sPath)) Directory.CreateDirectory(sPath);
            sPath = OM.EqpOptn.sVisnPath + "\\" + "LotData" + ((int)Para.Id+1).ToString() +".dat";
            if (File.Exists(sPath))File.Delete(sPath) ;

            try
            {
                using (FileStream fs = new FileStream(sPath, FileMode.Append))
                {
                    StreamWriter sw = new StreamWriter(fs, Encoding.Default);
                    sw.WriteLine(sData);
                    sw.Close();
                }
            }
            catch(Exception _e)
            {
                sErrMsg = _e.Message ;
                return false ;
            }

            return true ;
        }
        private bool CycleLotStart()
        {
            if (tmTimeOut.OnDelay(iCycleStep != 0 && iCycleStep == iPreCycleStep, 2000))
            {
                sErrMsg = string.Format(Para.Id.ToString() + " " + eSendingCmd.ToString() + " Cycle TimeOut iCycleStep={0:00}", iCycleStep);
                Log.Trace(sErrMsg);
                ML.IO_SetY(Para.yiLotStart , false) ;
                ML.ER_SetErr(ei.VSN_ComErr, sErrMsg);
                return true;
            }

            if(iPreCycleStep != iCycleStep)
            {
                string sTemp = string.Format(Para.Id.ToString() + " " +eSendingCmd.ToString() + " Cycle iCycleStep={0:00}", iCycleStep);
                Log.Trace(sTemp);
            }

            iPreCycleStep = iCycleStep ;

            switch (iCycleStep)
            {

                default:
                    sErrMsg = string.Format(Para.Id.ToString() + " " +eSendingCmd.ToString() + " Cycle Default Clear at iCycleStep={0:00}", iCycleStep);
                    Log.Trace(sErrMsg);
                    ML.ER_SetErr(ei.VSN_ComErr , sErrMsg );
                    ML.IO_SetY(Para.yiLotStart , false) ;
                    return true;

                case 10:
                    if(!ML.IO_GetX(Para.xiVisnReady))
                    {
                        sErrMsg = Para.Id.ToString() + " Vision Not Ready!";
                        ML.ER_SetErr(ei.VSN_ComErr , sErrMsg );
                        return true ;
                    }
                    if(ML.IO_GetX(Para.xiVisnBusy))
                    {
                        sErrMsg = Para.Id.ToString() + " Vision is Busy!";
                        ML.ER_SetErr(ei.VSN_ComErr , sErrMsg );
                        return true ;
                    }

                    if(!WriteLot(sPara))
                    {
                        //쓰기페일.
                        sErrMsg = Para.Id.ToString() + " Vision WriteLot Failed!";
                        ML.ER_SetErr(ei.VSN_ComErr , sErrMsg );
                        return true ;
                    }
                    ML.IO_SetY(Para.yiLotStart , true) ;
                    iCycleStep++;
                    return false;

                case 11:
                    if(!ML.IO_GetX(Para.xiVisnBusy))return false ;
                    ML.IO_SetY(Para.yiLotStart , false) ;
                    iCycleStep++;
                    return false;

                case 12:
                    if(ML.IO_GetX(Para.xiVisnBusy))return false ;
                    iCycleStep++;
                    return false ;

                case 13:
                    
                    return true ;
            }
        }

        //Change1~6
        private bool WriteJob(string _sJob)
        {
            //Local Var.
            string     sPath  ;
            string     sData  ;
        
            //Set Path.
            sPath = OM.EqpOptn.sVisnPath ;
            sData = _sJob+";" ; //sData = OM.DevInfo.sVisnIndexId + ";" + OM.m_sCrntDev + ";";

            if (!Directory.Exists(sPath)) Directory.CreateDirectory(sPath);

            sPath = OM.EqpOptn.sVisnPath + "\\" + "Change" +  ((int)Para.Id+1).ToString() + ".dat"; //이건 협의 된건가???
            if (File.Exists(sPath))File.Delete(sPath) ;
            //FileInfo fileinfo = new FileInfo(sPath);
            try
            {
                using (FileStream fs = new FileStream(sPath, FileMode.Append))
                {
                    StreamWriter sw = new StreamWriter(fs, Encoding.Default);
                    sw.WriteLine(sData);
                    sw.Close();
                }
            }
            catch (Exception _e)
            {
                sErrMsg = _e.Message;
                return false;
            }


            return true ;
        }
        private bool CycleJobChange()
        {
            if (tmTimeOut.OnDelay(iCycleStep != 0 && iCycleStep == iPreCycleStep , 3000 )) 
            {
                sErrMsg = string.Format(Para.Id.ToString() + " " +eSendingCmd.ToString() + " Cycle TimeOut iCycleStep={0:00}", iCycleStep);
                Log.Trace(sErrMsg);
                ML.ER_SetErr(ei.VSN_ComErr , sErrMsg );
                ML.IO_SetY(Para.yiJobStart , false) ;
                return true;
            }

            if(iPreCycleStep != iCycleStep)
            {
                string sTemp = string.Format(Para.Id.ToString() + " " +eSendingCmd.ToString() + " Cycle iCycleStep={0:00}", iCycleStep);
                Log.Trace(sTemp);
            }

            iPreCycleStep = iCycleStep ;

            switch (iCycleStep)
            {

                default:
                    sErrMsg = string.Format(Para.Id.ToString() + " " +eSendingCmd.ToString() + " Cycle Default Clear at iCycleStep={0:00}", iCycleStep);
                    Log.Trace(sErrMsg);
                    ML.ER_SetErr(ei.VSN_ComErr , sErrMsg );
                    ML.IO_SetY(Para.yiJobStart , false) ;
                    return true;

                case 10:
                    if(!ML.IO_GetX(Para.xiVisnReady))
                    {
                        sErrMsg = Para.Id.ToString() + " Vision Not Ready!";
                        ML.ER_SetErr(ei.VSN_ComErr , sErrMsg );
                        return true ;

                    }
                    if(ML.IO_GetX(Para.xiVisnBusy))
                    {
                        sErrMsg = Para.Id.ToString() + " Vision is Busy!";
                        ML.ER_SetErr(ei.VSN_ComErr , sErrMsg );
                        return true ;
                    }

                    if(!WriteJob(sPara))
                    {
                        //쓰기 실패.
                        sErrMsg = Para.Id.ToString() + " Vision WriteJob Failed!";
                        ML.ER_SetErr(ei.VSN_ComErr , sErrMsg );
                        return true ;
                    }
                    ML.IO_SetY(Para.yiJobStart , true) ;
                    iCycleStep++;
                    return false;

                case 11:
                    if(!ML.IO_GetX(Para.xiVisnBusy))return false ;
                    ML.IO_SetY(Para.yiJobStart , false) ;
                    iCycleStep++;
                    return false;

                case 12:
                    if(ML.IO_GetX(Para.xiVisnBusy))return false ;
                    iCycleStep++;
                    return false ;

                case 13:
                    
                    return true ;
            }
        }


        //=====================================================================================================================================================================================
        //퍼블릭....
        //밖에서 Good마스킹 시켜서 돌려야 함.
        //좌,우 비전 같이 한번씩 태워야 한다. 그래서 그럼.
        //Result1~6
        public bool ReadResult(CArray _arAray)
        {
            //Local Var.
            string     sPath = "" ;
            string     sData = "" ;
        
            //Set Path.
            sPath = OM.EqpOptn.sVisnPath ;
            if (!Directory.Exists(sPath)) Directory.CreateDirectory(sPath);

            sPath = sPath + "\\Result"+((int)Para.Id+1).ToString()+".dat";

            //switch(Para.Id)
            //{
            //    case vi.Vs1L:  break ;
            //    case vi.Vs1R: sPath = sPath + "\\Result"+((int)Para.Id+1).ToString(); break ;
            //    case vi.Vs2L: sPath = sPath + "\\Result"+((int)Para.Id+1).ToString(); break ;
            //    case vi.Vs2R: sPath = sPath + "\\Result"+((int)Para.Id+1).ToString(); break ;
            //    case vi.Vs3L: sPath = sPath + "\\Result"+((int)Para.Id+1).ToString(); break ;
            //    case vi.Vs3R: sPath = sPath + "\\Result"+((int)Para.Id+1).ToString(); break ;
            //}
            if(!File.Exists(sPath))
            {
                Log.Trace("Result Reading Err" , sPath + " - is not exist");
                return false ;
            }

            
            string sLine = "";
            try
            {
                using (FileStream fs = new FileStream(sPath, FileMode.Open))
                {
                    StreamReader sr = new StreamReader(fs, Encoding.Default);
                    while(true)    
                    {
                        sLine = sr.ReadLine();
                        if(sLine == null) break;
                        sData+= sLine;
                    }
                    sr.Close();
                }
            }
            catch(Exception _e)
            {
                sErrMsg = _e.Message ;
                return false ;
            }

            string sFailCnt = sData.Substring(0,3);
            if(!int.TryParse(sFailCnt , out int iFailCnt))
            {
                sErrMsg = Para.Id.ToString() + " : '" + sFailCnt + "'is can't changed to int" ;
                return false ;
            }
            sData = sData.Remove(0,3);

            string sRow ="";
            string sCol ="";
            string sFail ="";
            cs Fail = cs.None ;
            for (int i = 0; i < iFailCnt; i++)
            {
                sCol = sData.Substring(0, 2);
                sData = sData.Remove(0, 2);
                if (!int.TryParse(sCol, out int iCol))
                {
                    sErrMsg =Para.Id.ToString() + " : '" + sCol + "'is can't changed to int" ;
                    return false ;
                }
                if (_arAray.GetMaxCol() < iCol)
                {
                    sErrMsg = Para.Id.ToString() + " : '" + sCol + "'is RangeOver" ;
                    return false ;
                }
                if (0 >= iCol)
                {
                    sErrMsg = Para.Id.ToString() + " : '" + sCol + "'is RangeOver" ;
                    return false ;
                }


                sRow = sData.Substring(0, 2);
                sData = sData.Remove(0, 2);
                if (!int.TryParse(sRow, out int iRow))
                {
                    sErrMsg = Para.Id.ToString() + " : '" + sRow + "'is can't changed to int" ;
                    return false ;
                }
                if (_arAray.GetMaxRow() < iRow)
                {
                    sErrMsg = Para.Id.ToString() + " : '" + sRow + "'is RangeOver" ;
                    return false ;
                }
                if (0 >= iRow)
                {
                    sErrMsg = Para.Id.ToString() + " : '" + sRow + "'is RangeOver" ;
                    return false ;
                }

                sFail = sData.Substring(0, 1);
                sData = sData.Remove(0, 1);

                     if(sFail == "0") Fail = cs.Rslt0 ;
                else if(sFail == "1") Fail = cs.Rslt1 ;
                else if(sFail == "2") Fail = cs.Rslt2 ;
                else if(sFail == "3") Fail = cs.Rslt3 ;
                else if(sFail == "4") Fail = cs.Rslt4 ;
                else if(sFail == "5") Fail = cs.Rslt5 ;
                else if(sFail == "6") Fail = cs.Rslt6 ;
                else if(sFail == "7") Fail = cs.Rslt7 ;
                else if(sFail == "8") Fail = cs.Rslt8 ;
                else if(sFail == "9") Fail = cs.Rslt9 ;
                else if(sFail == "A") Fail = cs.RsltA ;
                else if(sFail == "B") Fail = cs.RsltB ;
                else if(sFail == "C") Fail = cs.RsltC ;
                else if(sFail == "D") Fail = cs.RsltD ;
                else if(sFail == "E") Fail = cs.RsltE ;
                else if(sFail == "F") Fail = cs.RsltF ;
                else if(sFail == "G") Fail = cs.RsltG ;
                else if(sFail == "H") Fail = cs.RsltH ;
                else if(sFail == "I") Fail = cs.RsltI ;
                else if(sFail == "J") Fail = cs.RsltJ ;
                else if(sFail == "K") Fail = cs.RsltK ;
                else if(sFail == "L") Fail = cs.RsltL ;
                else
                {
                    sErrMsg = Para.Id.ToString() + " : '" + sFail + "' Result is RangeOver" ;
                    return false ;
                }
                _arAray.SetStat(iCol-1 ,iRow-1 , Fail) ;
            }

            File.Delete(sPath) ;
            return true ;
        }

        public void Reset()
        {
            eSendingCmd = vc.None;
            iCycleStep = 0;
            iPreCycleStep = 0;
            sErrMsg = "";
            Log.Trace(Para.Id.ToString() + " Reset");
        }

        public void SendCmd(vc _eCmd , string _sPara = "") 
        {
            eSendingCmd = _eCmd ;
            iCycleStep = 10 ;
            iPreCycleStep = 0 ;
            sPara= _sPara ;
            sErrMsg = "";
            Log.Trace(Para.Id.ToString() + " Start Cycle - " + _eCmd.ToString()) ; 
        }

        public string GetErrMsg()
        {
            return sErrMsg ;
        }

        public bool EndCmd()
        {
            return eSendingCmd == vc.None ;
        }

        public bool GetIOReady()
        {
            return ML.IO_GetX(Para.xiVisnReady) ;
        }
        public bool GetIOEnd()
        {
            return ML.IO_GetX(Para.xiVisnEnd) ;
        }

    }
}
