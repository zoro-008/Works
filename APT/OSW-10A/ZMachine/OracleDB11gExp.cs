using COMMON;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.IO;
using System.Threading;

namespace Machine
{
    //D:\Works\APT\래핑머신\소스\OSW-10A\Doc 여기보면 오라클관련 문서 있음. 
    //서버에도 있는데 이것은 초기 테스트 환경 세팅 방법

    //(1)  Accessible IP Address to data base of OSRAM Oracle Server
    //- This is a local server and IP address is 192.168.1.1 
    //(2)   User ID for Data Base
    //     - The user name is SYSTEM
    //(3)   Pass ward for accessible Data Base 
    //     - The password is 12345
    //(4)   Data Base Table structure and detailed documentation to get it from DB by using Tray Label.
    //     - 자료 첨부하니 확인 하시기 바랍니다.  

    public class OracleBase //DB11gExp
    {
        #region SendMsg
        string LastMsg = "";
        public string GetLastMsg(){return LastMsg;}
        public delegate void FSendMsg(string _sMsg);
        FSendMsg Msg = null; 
        public void SetSendMsgFunc(FSendMsg _fpSendMsg)
        {
            Msg = _fpSendMsg ;
        }
        public void SendMsg(string _sMsg , [CallerMemberName] string _sFuncName = "")
        {
            //string sMsg = "<FUNC:" + _sFuncName + ">   " + _sMsg ;
            string sMsg = _sMsg ;
            LastMsg = sMsg ;

            Log.Trace("Oracle",sMsg);

            if(Msg==null)return ;
            
            Msg(sMsg);
        }
        #endregion

        protected OracleConnection OracleCon ;
        OracleCommand    OracleCmd ;
        public DataTable Table = new DataTable();
        private string sConStr = "";

        public bool OpenDB(string _sIP, string _sPort , string _sID, string _sPW , string _sSID)
        {   
            this.sConStr = "";
            this.sConStr = "DATA SOURCE = (DESCRIPTION = (ADDRESS_LIST = (ADDRESS = (PROTOCOL = TCP)(HOST = " + _sIP + ")(PORT = " + _sPort + ")))";
            //this.sConStr += "(CONNECT_DATA = (SERVICE_NAME = xe)));";
            this.sConStr += "(CONNECT_DATA = (SID = " + _sSID + ")));";
            this.sConStr += "USER ID=" + _sID + ";PASSWORD="+_sPW;
            if (_sIP == "") {
                SendMsg("IP is NULL");
                return false;
            }
            if (_sPort == "") {
                SendMsg("PORT is NULL");
                return false;
            }
            if (_sID == "") {
                SendMsg("ID is NULL");
                return false;
            }
            if (_sPW == "") {
                SendMsg("PW is NULL");
                return false;
            }
            if(!Connect()) {
                return false;
            }

            return true;
        }

        private bool Connect()
        {
            try
        	{
        		this.OracleCon = new OracleConnection(sConStr);
        		this.OracleCon.Open();
        		this.OracleCmd = OracleCon.CreateCommand();
        		
        	}
        	catch (Exception ex)
        	{
                SendMsg("Connection Failed!");
                SendMsg(ex.Message);
        		return false;
        	}

            if (this.OracleCon.State != ConnectionState.Open){
                SendMsg("Oracle Not Connected");
                return false ;
            }
            SendMsg("Connection Success!");
            return true ;
        }

        public bool CloseDB()
        {
            if(this.OracleCon.State != ConnectionState.Open) {
                //창이 없어져있어서 보내면 안됌.
                //SendMsg("Not Connected");
                return false;
            }
            if(this.OracleCmd != null)
            { 
                this.OracleCmd.Dispose();
            
                this.OracleCon.Close();
            }
            return true ;
        }
        
        //쿼리문으로 선택한 데이터만 DataTable에 담음.
        public bool Select(string _sQuery , ref DataTable _Table)
        {
            if(this.OracleCon.State != ConnectionState.Open) {
                 SendMsg("Try ReConnect");
                 Connect();
            }
            if(this.OracleCon.State != ConnectionState.Open) {
                SendMsg("Not Connected");
                return false;
            }

            bool bRet = true ;
            DateTime Time =  DateTime.Now ;
            
            _Table.Columns.Clear();
            _Table.Clear();
        
            this.OracleCmd.CommandText = _sQuery;
            SendMsg(this.OracleCmd.CommandText);
            using (OracleDataAdapter da = new OracleDataAdapter(OracleCmd))
            {
                try{
             	    da.Fill(_Table);
                    SendMsg("Selected Data is " + _Table.Rows.Count + "ea");
                }
                catch(Exception ex){      
                    SendMsg(ex.Message); 
                    if (ex.Message.Contains("ORA-03114") || ex.Message.Contains("ORA-03135") || ex.Message.Contains("ORA-12570"))
                    {
                        Connect();
                    }
                    bRet = false ;
                }
                finally{

                }
            }

            TimeSpan Span =  DateTime.Now - Time ;
            double dMs = Span.TotalMilliseconds ;
            return bRet;
        }

        ////이건거의 안씀 밑에꺼만 씀.
        //public bool Insert(string _sTableName , string _sValues)//_sValues는 'HZ63700A99_001_160926_212440P_29','3KFC7C1S','2122654747' 이런형식.
        //{
        //     if(this.OracleCon.State != ConnectionState.Open) {
        //         SendMsg("Not Connected");
        //         return false;
        //     }          
        //     OracleTransaction STrans=null;  //오라클 트랜젝션                     
        //     STrans =OracleCon.BeginTransaction();           
        //     bool bRet =true ;
            
        //     try{                  
        //         this.OracleCmd.Transaction = STrans;  //커맨드에 트랜젝션 명시        
        //         string sValueString = "Insert Into " + _sTableName + " VALUES(" + _sValues + ")";
        //         this.OracleCmd.CommandText = sValueString ;
        //         SendMsg(this.OracleCmd.CommandText);
        //         this.OracleCmd.ExecuteNonQuery() ;           
        //         this.OracleCmd.Transaction.Commit();   //커밋          
        //     }
        //     catch(Exception ex){          
        //         this.OracleCmd.Transaction.Rollback();   //롤백
        //         SendMsg(ex.Message);
        //         bRet = false ;
        //     }
        //     finally{
        //         //this.OracleCmd.Dispose();
        //         //this.OracleCon.Close();
             
        //     }
        //     return bRet ;
        //}

        //함수 이름은 인서트 이나 트렌젝션 기능 있는 쿼리함수 라고 볼 수 있음.
        public bool Insert(string _sQuery)
        {
             if(this.OracleCon.State != ConnectionState.Open) {
                 SendMsg("Try ReConnect");
                 Connect();
             }
             if(this.OracleCon.State != ConnectionState.Open) {
                 SendMsg("Not Connected");
                 return false;
             }          
             OracleTransaction STrans=null;  //오라클 트랜젝션          
             //OracleCon.Open();             
             STrans =OracleCon.BeginTransaction();           
             //this.OracleCmd = new OracleCommand("",OracleCon);
             bool bRet =true ;
            
             try{                  
                 this.OracleCmd.Transaction = STrans;  //커맨드에 트랜젝션 명시        
                 //string sValueString = string.Join("','", (from Val in _lsValues select Val).ToArray());
                 this.OracleCmd.CommandText = _sQuery ;
                 SendMsg(this.OracleCmd.CommandText);
                 this.OracleCmd.ExecuteNonQuery() ;           
                 this.OracleCmd.Transaction.Commit();   //커밋       
                 SendMsg("Insert is Success!" );
             }
             catch(Exception ex){  
                 SendMsg(ex.Message);
                 try{ 
                     this.OracleCmd.Transaction.Rollback();   //롤백
                 }
                 catch(Exception ex2){
                     SendMsg(ex2.Message);
                 }

                 if (ex.Message.Contains("ORA-03114") || ex.Message.Contains("ORA-03135") || ex.Message.Contains("ORA-12570"))
                 {
                     Connect();
                 }

                 bRet = false ;
             }
             finally{
                 //this.OracleCmd.Dispose();
                 //this.OracleCon.Close();
             
             }
             return bRet ;
        }
    }


    public class PointD
    {

        public double X { get; set; }
        public double Y { get; set; }
        public PointD(double x, double y)
        {
            X = x;
            Y = y;
        }     
        public override string ToString()
        {
            return string.Format("X={0},Y={1}", X, Y);
        }
    }

    public class COracle : OracleBase
    {
        static Thread SubThread = new Thread(new ThreadStart(Update));
        //DMC1 중복검사를 위한 리스트를 랏오픈 혹은 피씨껐다 켰을때 만둔다.
        static public bool bMakingDMC1List  = false ;
        static public bool bMakeDMC1ListRet = false ;

        //PanelID 포함검사를 위한 리스트를 트레이라벨리딩 혹은 피씨껐다 켰을때 만둔다.
        static public bool bMakingPanelList  = false ;
        static public bool bMakePanelListRet = false ;

        public bool Init(string _sIP, string _sPort , string _sID, string _sPW , string _sSID)
        {
            bool bRet = OpenDB( _sIP,  _sPort ,  _sID,  _sPW ,  _sSID);
            if(!bRet) Log.ShowMessage("Oracle DB Open" , GetLastMsg());


            LoadSave(true);

            SubThread.Priority = ThreadPriority.Normal;
            SubThread.Start();

            if (bRet)
            {
                ThreadMakeUnitIDDMC1List();
                ThreadMakePanelIDList();
            }

            return bRet ;
        }
        public bool Close()
        {
            LoadSave(false);

            SubThread.Abort();
            SubThread.Join();

            return CloseDB();
        }
        public void ThreadMakeUnitIDDMC1List()
        {
            bMakingDMC1List = true ;
        }
        public void ThreadMakePanelIDList()
        {
            bMakingPanelList = true ;
            
        }
        static void Update()
        {
            while (true)
            {
                Thread.Sleep(0);
                if (bMakingDMC1List) { 
                    if(!SEQ.Oracle.MakeUnitIDDMC1List()) {
                        bMakeDMC1ListRet = false ;
                        //Log.ShowMessage("Oracle Err","Make DMC1 List Failed!-"+SEQ.Oracle.GetLastMsg());
                    }
                    else {  
                        bMakeDMC1ListRet = true ;
                        //Log.ShowMessage("Oracle","Make DMC1 List Success!");
                    }
                    bMakingDMC1List = false ;
                }

                if (bMakingPanelList) { 
                    if(!SEQ.Oracle.MakePanelList()) {
                        bMakePanelListRet = false ;
                        //Log.ShowMessage("Oracle Err","ProcessTrayLabel Failed!-"+SEQ.Oracle.GetLastMsg());
                    }
                    else {  
                        bMakePanelListRet = true ;
                        //Log.ShowMessage("Oracle","Make DMC1 List Success!");
                    }
                    bMakingPanelList = false ;
                }
            }
        }

        
  


        public bool GetDBOpen()
        {
            return this.OracleCon.State == ConnectionState.Open ;
        }

        public string GetLotNumberWithoutDot(string _sLotNo)//"HZ6200A9.01" 이런식인데 서버에 저장할때는 "."을빼고 "HZ6200A9.01" 저장된다.
        {
            string sRet = _sLotNo.Replace(".","");
            return sRet ; 
        }
        public class CStat
        {
            //랏 트레블러에서 온것.
            public string sLotTraveler_LotNo     ;
            public string sLotTraveler_MaterialNo;// ==T2_11Series ==  BIGMatrial No == 
            public string sLotTraveler_LotAlias  ;

            //비전 레서피 테이블
            public string sVisionRecipe_RecipeName ;

            //Unit Inspection 테이블
            public List<string>lsUnitInspection_UnitID = new List<string>();//이미 지금 랏 조건으로 작업 했었던 유닛 리스트.하나 하나 검사 하여 작업 했었던 유닛이 나오면 에러.
            public List<string>lsUnitInspection_DMC1   = new List<string>();//Have to be not contained in TBL_UNIT_INSPECTION

            //TrayLabel
            public string sTrayLabel_TrayLabel   ;
            public string sTrayLabel_GroupingNo  ;
            public string sTrayLabel_MaterialNo  ;   
            public string sTrayLabel_BatchNo     ;
            public string sTrayLabel_SecurityCode;

            //TrayInfomation            
            public List<string>lsTrayInfomation_SmallMaterialNumber = new List<string>();
            public string sTrayInfomation_DMC1                ;
            public string sTrayInfomation_DeviceNumber        ; 
            public string sTrayInfomation_Bin                 ; 
            public string sTrayInfomation_BigTrayQty          ; 
            public List<string>lsProbeFile_ProbeFile = new List<string>();

            //DMC2 Value
            public int    iDmc2Value_InspectionType ;       
            public double dDmc2Value_Pmin           ;       
            public double dDmc2Value_Pmax           ;       
            public double dDmc2Value_LMin           ;       
            public double dDmc2Value_LMax           ;       
            public double dDmc2Value_Cx1            ;       
            public double dDmc2Value_Cy1            ;       
            public double dDmc2Value_Cx2            ;       
            public double dDmc2Value_Cy2            ;       
            public double dDmc2Value_Cx3            ;       
            public double dDmc2Value_Cy3            ;       
            public double dDmc2Value_Cx4            ;       
            public double dDmc2Value_Cy4            ;       

            //검사결과 쿼리.
            public List<string> lsUnitInspection_Values = new List<string>();
            

            public void Clear()
            {
                sLotTraveler_LotNo       = "";
                sLotTraveler_MaterialNo  = "";
                sLotTraveler_LotAlias    = "";

                sVisionRecipe_RecipeName = "";

                lsUnitInspection_UnitID.Clear();
                lsUnitInspection_DMC1  .Clear();

                sTrayLabel_TrayLabel     = "";
                sTrayLabel_GroupingNo    = "";
                sTrayLabel_MaterialNo    = "";
                sTrayLabel_BatchNo       = "";
                sTrayLabel_SecurityCode  = "";

                lsTrayInfomation_SmallMaterialNumber.Clear();
                sTrayInfomation_DMC1                ="";
                sTrayInfomation_DeviceNumber        ="";
                sTrayInfomation_Bin                 ="";
                sTrayInfomation_BigTrayQty          ="";                
                lsProbeFile_ProbeFile.Clear();

                iDmc2Value_InspectionType = 0 ;
                dDmc2Value_Pmin           = 0 ;
                dDmc2Value_Pmax           = 0 ;
                dDmc2Value_LMin           = 0 ;
                dDmc2Value_LMax           = 0 ;
                dDmc2Value_Cx1            = 0 ;
                dDmc2Value_Cy1            = 0 ;
                dDmc2Value_Cx2            = 0 ;
                dDmc2Value_Cy2            = 0 ;
                dDmc2Value_Cx3            = 0 ;
                dDmc2Value_Cy3            = 0 ;
                dDmc2Value_Cx4            = 0 ;
                dDmc2Value_Cy4            = 0 ;
                
                lsUnitInspection_Values.Clear();
            }
        }
        public CStat Stat = new CStat();

        //서버의 NLS파라미터 조회.
        //select * from NLS_DATABASE_PARAMETERS;
        //윈도우키+R => cmd치고 => 실행창에서 'regedit' 치면 => 검색으로'NLS_LANG'치면 데이터에 클라이언트 NLS_LANGUAGE를 확인 할 수 있다.
        public bool GetNLS_DATABASE_PARAMETERS()
        {
            return Select("select * from NLS_DATABASE_PARAMETERS" , ref Table);
        }

        public bool ProcessLotOpen(string _sLotNo , string _sMaterialNo , string _sLotAlias)
        {                   
            Stat.Clear();

            Stat.sLotTraveler_LotNo         = _sLotNo      ;
            Stat.sLotTraveler_MaterialNo    = _sMaterialNo ;
            Stat.sLotTraveler_LotAlias      = _sLotAlias   ;

            string sQuery = "";
            //============
            //Get Recipe When you lotopen.
            //============
            sQuery = "select RECIPE_NAME from TBL_VISION_RECIPE_AP where BIG_MATERIAL_NO='"+Stat.sLotTraveler_MaterialNo+"'";
            
            //DataTable Table = new DataTable();
            if (!Select(sQuery,ref Table)) {
                SendMsg(sQuery + " Select Query Failed!");
                return false;
            }
            if(Table.Rows.Count == 0){
                SendMsg(sQuery + " Select Query No Recipe!");
                return false;
            }
            if(Table.Rows.Count > 1){
                SendMsg(sQuery + " Recipe is more than 1ea!");
                return false;
            }
            Stat.sVisionRecipe_RecipeName = Table.Rows[0]["RECIPE_NAME"].ToString();

            return true ;
            
        }
        public bool MakeUnitIDDMC1List()
        {
            //==============검사 하면서 보면 느리니깐 작업할 자제의 조건으로 미리 랏오픈때 리스트를 만들고 하다.
            //Make List unitid and dmc1 When you lotopen.
            //==============
            //bMakingUnitIDList = true ;
            Stat.lsUnitInspection_UnitID.Clear();
            Stat.lsUnitInspection_DMC1  .Clear();

            DateTime CutOffDate = DateTime.Now.AddHours((OM.DevOptn.iDMC1MonthLimit * 30 * -24)); // max 2 years
            string sQuery = "";
            sQuery = "select UNIT_ID, DMC1 from TBL_UNIT_INSPECTION WHERE T2_11_SERIES = '" + Stat.sLotTraveler_MaterialNo +"'";

            if(OM.DevOptn.iDMC1MonthLimit != 0)sQuery += " AND trunc(DATE_TIME) >= to_date('" + CutOffDate.ToString("yyyy-MM-dd") + "', 'YYYY-MM-DD')";
            if (!Select(sQuery,ref Table)) {
                SendMsg(sQuery + " Select Query Failed!");
                //bMakingUnitIDList = false ;
                return false;
            }
            if(Table.Rows.Count > 0){
                for(int r = 0 ; r < Table.Rows.Count ; r++){
                    Stat.lsUnitInspection_UnitID.Add(Table.Rows[r]["UNIT_ID"].ToString());
                    Stat.lsUnitInspection_DMC1  .Add(Table.Rows[r]["DMC1"   ].ToString());
                }
            }
            //bMakingUnitIDList = false ;
            return true ;
        }

        public bool ProcessTrayLabel(string _sTrayLabel)
        {
            if (_sTrayLabel.Length < 25)
            {
                SendMsg("Label:"+_sTrayLabel + " is less than 25 char");
                return false ;
            }
            Stat.lsTrayInfomation_SmallMaterialNumber.Clear();
            

            //메트리얼 , 빈 , LotAlias 같이 확인======================================
            Stat.sTrayLabel_TrayLabel     = _sTrayLabel ;
            Stat.sTrayLabel_MaterialNo    = _sTrayLabel.Substring(4,8);
            Stat.sTrayLabel_GroupingNo    = _sTrayLabel.Substring(0,4);
            Stat.sTrayLabel_BatchNo       = _sTrayLabel.Substring(12,10);
            Stat.sTrayLabel_SecurityCode  = _sTrayLabel.Substring(22,3);

            if (Stat.sTrayLabel_MaterialNo != Stat.sLotTraveler_MaterialNo) {
                SendMsg("LotTravler And TrayLabel has Different MatrialNo!");
                return false;
            }

            int iGroupingNo = 0 ;
            if (!int.TryParse(Stat.sTrayLabel_GroupingNo, out iGroupingNo))
            {
                SendMsg("GroupingNo:"+ Stat.sTrayLabel_GroupingNo + " Pharsing Failed!");
                return false ;
            }
            Stat.sTrayLabel_GroupingNo = iGroupingNo.ToString() ;


            //==========
            //get tray information When you TrayBarcode Reading.
            //==========
            string sQuery = "" ;
            //sQuery = "select * from TBL_TRAY_INFORMATION where BIG_MATERIAL_NO='"+Stat.sLotTraveler_MaterialNo+"' and BIN=" + Stat.sTrayLabel_GroupingNo + " and LOT_TRAV_GROUP='"+_sLotAlias+"'" ;
            sQuery = "select * from TBL_TRAY_INFORMATION WHERE BIG_MATERIAL_NO = '" + Stat.sLotTraveler_MaterialNo + "' AND  LOT_TRAV_GROUP = '" + Stat.sLotTraveler_LotAlias+ "' AND  BIN = '" + Stat.sTrayLabel_GroupingNo + "'";
            if (!Select(sQuery,ref Table)) {
                SendMsg(sQuery + " Select Query Failed!");
                return false;
            }
            if(Table.Rows.Count == 0){
                SendMsg(sQuery + " Select Query No Data!");
                return false;
            }

            //char[] cComma={','};
            //string[] sItems ;
            //sItems = Table.Rows[0]["DMC_1"].ToString().Split(cComma);
            //foreach (string sItem in sItems)
            //{
            //    if (!lsDMC1.Contains(sItem))
            //    {
            //        Stat.lsTrayInfomation_DMC1.Add(sItem);
            //    }
            //}       
            Stat.sTrayInfomation_DMC1                = Table.Rows[0]["DMC_1"                ].ToString();
            Stat.sTrayInfomation_DeviceNumber        = Table.Rows[0]["DEVICE_NUMBER"        ].ToString();
            Stat.sTrayInfomation_Bin                 = Table.Rows[0]["BIN"                  ].ToString();
            Stat.sTrayInfomation_BigTrayQty          = Table.Rows[0]["BIG_TRAY_QTY"         ].ToString();

            for (int r = 0; r < Table.Rows.Count; r++)
            {   
                if(!Stat.lsTrayInfomation_SmallMaterialNumber.Contains(Table.Rows[r]["SMALL_MATERIAL_NUMBER"].ToString())) {
                    Stat.lsTrayInfomation_SmallMaterialNumber.Add(Table.Rows[r]["SMALL_MATERIAL_NUMBER"].ToString());
                    SendMsg("Small MatNo"+r.ToString() + "-" + Table.Rows[r]["SMALL_MATERIAL_NUMBER"].ToString());
                }
            }   

            //오래 걸려서 쓰레드로.
            ////=============
            ////get probe file When you TrayBarcode Reading.
            ////============
            //DateTime CutOffDate = DateTime.Now.AddHours((OM.DevOptn.iDMC1MonthLimit * 30 * -24)); // max 2 years
            //string sWhereT1 = "";
            //for(int i = 0 ; i < Stat.lsTrayInfomation_SmallMaterialNumber.Count ; i++){
            //    sWhereT1 += "(T1_11_SERIES='"+Stat.lsTrayInfomation_SmallMaterialNumber[i]+"'";
            //    if(Stat.lsTrayInfomation_SmallMaterialNumber.Count-1 != i) sWhereT1+=" or ";
            //}
            //sWhereT1 +=")";

            //string sDateCutoff = "";            
            //if(OM.DevOptn.iDMC1MonthLimit != 0) sDateCutoff = " AND trunc(PROBE_FILE_DATE_TIME) >= to_date('" + CutOffDate.ToString("yyyy-MM-dd") + "', 'YYYY-MM-DD')";

            //if (OM.DevOptn.bUnitID) { 
            //    sQuery = "select distinct PROBE_FILE from TBL_PROBE_FILE where " + sWhereT1 + sDateCutoff ;
            //    if (!Select(sQuery,ref Table)) {
            //        SendMsg(sQuery + " Select Query Failed!");
            //        return false;
            //    }
            //    if(Table.Rows.Count == 0){
            //        SendMsg(sQuery + " Select Query No Data!");
            //        return false;
            //    }           
            

            //    string sItem ;
            //    string sItems = "" ; 
            //    for (int i = 0; i < Table.Rows.Count; i++)
            //    {
            //        sItem = Table.Rows[i]["PROBE_FILE"].ToString();
            //        sItems += sItem + " ";
            //        if (!Stat.lsProbeFile_ProbeFile.Contains(sItem))
            //        {
            //            Stat.lsProbeFile_ProbeFile.Add(sItem);
            //        }
            //    }
            //}
            //SendMsg(sItems);

            //================
            //Set dmc 2 value Spec When you  TrayBarcode Reading.
            //=================
            //여기가 이상하네.......2개
            //
            if (OM.DevOptn.iDMC2CheckMathod == 2) { //Device Spec Compare optn
                string sWhereT1 = "";
                sWhereT1 = "(";
                for(int i = 0 ; i < Stat.lsTrayInfomation_SmallMaterialNumber.Count ; i++){
                    sWhereT1 += "T1_11_SERIES='"+Stat.lsTrayInfomation_SmallMaterialNumber[i]+"'";
                    if(Stat.lsTrayInfomation_SmallMaterialNumber.Count-1 != i) sWhereT1+=" or ";
                }
                sWhereT1 +=")";
                sQuery = "select * from TBL_DMC2_VALUE WHERE " + sWhereT1 + " AND T2_11_SERIES = '" + Stat.sLotTraveler_MaterialNo + "' AND DEVICE = '" + Stat.sTrayInfomation_DeviceNumber + "' AND GROUPING = '" + Stat.sLotTraveler_LotAlias + "'";
                if (!Select(sQuery,ref Table)) {
                    SendMsg(sQuery + " Select Query Failed!");
                    return false;
                }
                if(Table.Rows.Count == 0){
                    SendMsg(sQuery + " Select Query No Data!");
                    return false;
                } 
                
                try{
                    Stat.iDmc2Value_InspectionType = Convert.ToInt32 (Table.Rows[0]["INSPECTION_TYPE"].ToString());
                    Stat.dDmc2Value_Pmin           = Convert.ToDouble(Table.Rows[0]["PMIN"           ].ToString());
                    Stat.dDmc2Value_Pmax           = Convert.ToDouble(Table.Rows[0]["PMAX"           ].ToString());
                    Stat.dDmc2Value_LMin           = Convert.ToDouble(Table.Rows[0]["LMIN"           ].ToString());
                    Stat.dDmc2Value_LMax           = Convert.ToDouble(Table.Rows[0]["LMAX"           ].ToString());
                    Stat.dDmc2Value_Cx1            = Convert.ToDouble(Table.Rows[0]["CX1"            ].ToString());
                    Stat.dDmc2Value_Cy1            = Convert.ToDouble(Table.Rows[0]["CY1"            ].ToString());
                    Stat.dDmc2Value_Cx2            = Convert.ToDouble(Table.Rows[0]["CX2"            ].ToString());
                    Stat.dDmc2Value_Cy2            = Convert.ToDouble(Table.Rows[0]["CY2"            ].ToString());
                    Stat.dDmc2Value_Cx3            = Convert.ToDouble(Table.Rows[0]["CX3"            ].ToString());
                    Stat.dDmc2Value_Cy3            = Convert.ToDouble(Table.Rows[0]["CY3"            ].ToString());
                    Stat.dDmc2Value_Cx4            = Convert.ToDouble(Table.Rows[0]["CX4"            ].ToString());
                    Stat.dDmc2Value_Cy4            = Convert.ToDouble(Table.Rows[0]["CY4"            ].ToString());
                }
                catch(Exception ex)
                {
                    SendMsg("Dmc2value Spec Convert Error-" + ex.Message);
                    return false;
                }
            }
            return true ;
        }

        //오래걸려서 
        public bool MakePanelList()
        {
            //=============
            //get probe file When you TrayBarcode Reading.
            //============
            Stat.lsProbeFile_ProbeFile.Clear();
            DateTime CutOffDate = DateTime.Now.AddHours((OM.DevOptn.iDMC1MonthLimit * 30 * -24)); // max 2 years
            string sWhereT1 = "";
            for(int i = 0 ; i < Stat.lsTrayInfomation_SmallMaterialNumber.Count ; i++){
                sWhereT1 += "(T1_11_SERIES='"+Stat.lsTrayInfomation_SmallMaterialNumber[i]+"'";
                if(Stat.lsTrayInfomation_SmallMaterialNumber.Count-1 != i) sWhereT1+=" or ";
            }
            sWhereT1 +=")";

            string sDateCutoff = "";            
            if(OM.DevOptn.iDMC1MonthLimit != 0) sDateCutoff = " AND trunc(PROBE_FILE_DATE_TIME) >= to_date('" + CutOffDate.ToString("yyyy-MM-dd") + "', 'YYYY-MM-DD')";

            if (OM.DevOptn.bUnitID) { 
                string sQuery = "select distinct PROBE_FILE from TBL_PROBE_FILE where " + sWhereT1 + sDateCutoff ;
                if (!Select(sQuery,ref Table)) {
                    SendMsg(sQuery + " Select Query Failed!");
                    return false;
                }
                if(Table.Rows.Count == 0){
                    SendMsg(sQuery + " Select Query No Data!");
                    return false;
                }           
            

                string sItem ;
                string sItems = "" ; 
                for (int i = 0; i < Table.Rows.Count; i++)
                {
                    sItem = Table.Rows[i]["PROBE_FILE"].ToString();
                    sItems += sItem + " ";
                    if (!Stat.lsProbeFile_ProbeFile.Contains(sItem))
                    {
                        Stat.lsProbeFile_ProbeFile.Add(sItem);
                    }
                }
            }
            return true ;
        }

        //into TBL_TEST Values ('sun','kye') 형태로 넣어야 한다.
        //"UNIT_INSPECTION_ID"	"UNIT_ID"	"DMC1"	"DMC2"	"DATE_TIME"	"RESULT"	"VIT_NO"	"TRAY_LABEL"	"LOT_NO"	"AVITTRK_AVIT"	"T2_11_SERIES"	"OLD_LOT_NO"	"NG_REMARK"
        public bool AddUnitInspectionQuery(string _sPocketID , string _sUnitID , string _sDMC1 , string _sDMC2 , cs _eStat ,string _sMachineID)
        {

            //비전 엠프티는 안 넣는다.
            //요건 까먹을 까봐 일단 넣었는데 원래 밖에서 처리해야함.
            if(_eStat == cs.NG0) { 
                return true ;
            }
            //if (sUnitInspectionQuery != "")
            //{
            //    sUnitInspectionQuery += " ";
            //}
            //=====================UnitInspection ID
            //HZ6200A9.01은 Lot number로 lottraveler로 프린트 되나,  서버에 저장 시 “.”는 삭제되어 HZ6200A901로 저장
            string sLOT_NOSub        = Stat.sLotTraveler_LotNo.Replace(".","");          
            string sSecurityCode = Stat.sTrayLabel_SecurityCode ;
            string sDateTime     = DateTime.Now.ToString("yyMMdd_HHmmss");
            //HZ63700A99_001_160926_212440P_29
            string sUNIT_INSPECTION_ID = "'"+sLOT_NOSub + "_" + sSecurityCode + "_" + sDateTime + "P_" + _sPocketID+"'" ;

            //=====================UnitID
            string sUNIT_ID = "'" +_sUnitID +"'";

            //=====================DMC1
            string sDMC1 = "'" +_sDMC1 +"'";

            //=====================DMC2
            string sDMC2 = "'" +_sDMC2 +"'";

            //=====================DATE_TIME//'01-OCT-19 10.21.45.078000000 AM','DD-MON-RR HH.MI.SSXFF AM','NLS_DATE_LANGUAGE=american'
            string sDATE_TIME = "CURRENT_TIMESTAMP"; //"to_timestamp('06-SEP-17 05.29.28.252000000 AM','DD-MON-RR HH.MI.SSXFF AM','NLS_DATE_LANGUAGE=american')" ;

            //=====================RESULT
            //cs.NG0  V_Empty        //자제없음.5
            //cs.NG1  V_MixDevice    //4chip 5chip 비전에서 설정하기 나름인데 바뀔수도 있음.
            //cs.NG2  V_UnitID       
            //cs.NG3  V_UnitDMC1     
            //cs.NG4  V_UnitDMC2     
            //cs.NG5  V_GlobtopLeft  
            //cs.NG6  V_GlobtopTop   
            //cs.NG7  V_GlobtopRight 
            //cs.NG8  V_GlobtopBottom
            //cs.NG9  V_MatchingError
            //cs.NG10 V_UserDefine      

            //이상하게 RESULT에 bottom이 없고 Gloptop Left Right Top만 있음.
            //string sRESULT = "'" ;
            //sRESULT += "Unit Locator:"           + (                           "1##") ;
            //sRESULT += "Unit ID:"                + (_eStat == cs.NG2 ? "0##" : "1##") ;
            //sRESULT += "DMC 1:"                  + (_eStat == cs.NG3 ? "0##" : "1##") ;
            //sRESULT += "DMC 2:"                  + (_eStat == cs.NG4 ? "0##" : "1##") ;
            //sRESULT += "Glob Top Epoxy (LEFT):"  + (                           "1##") ;
            //sRESULT += "Glob Top Epoxy (RIGHT):" + (                           "1##") ;
            //sRESULT += "Unit Pattern (LEFT):"    + (                           "1##") ;
            //sRESULT += "Unit Pattern (RIGHT):"   + (_eStat == cs.NG9 ? "0##" : "1##") ;
            //sRESULT += "Glob Top (LEFT):"        + (_eStat == cs.NG5 ? "0##" : "1##") ;
            //sRESULT += "Glob Top (RIGHT):"       + (_eStat == cs.NG7 ? "0##" : "1##") ;
            //sRESULT += "Glob Top (TOP):"         + (_eStat == cs.NG6 ? "0##" : "1##") ;
            //sRESULT += "'" ;
            string sRESULT = "'" ;
            sRESULT += "MixDevice:"     + (_eStat == cs.NG1  ? "0##" : "1##") ;   
            sRESULT += "UnitID:"        + (_eStat == cs.NG2  ? "0##" : "1##") ;
            sRESULT += "UnitDMC1:"      + (_eStat == cs.NG3  ? "0##" : "1##") ;
            sRESULT += "UnitDMC2:"      + (_eStat == cs.NG4  ? "0##" : "1##") ;
            sRESULT += "GlobtopLeft:"   + (_eStat == cs.NG5  ? "0##" : "1##") ;   
            sRESULT += "GlobtopTop:"    + (_eStat == cs.NG6  ? "0##" : "1##") ;   
            sRESULT += "GlobtopRight:"  + (_eStat == cs.NG7  ? "0##" : "1##") ;   
            sRESULT += "GlobtopBottom:" + (_eStat == cs.NG8  ? "0##" : "1##") ;
            sRESULT += "MatchingError:" + (_eStat == cs.NG9  ? "0##" : "1##") ;
            sRESULT += "UserDefine:"    + (_eStat == cs.NG10 ? "0##" : "1##") ;
            sRESULT += "'" ;

            //=====================VIT_NO
            //HZ63700A99_001_160926_212440P
            string sVIT_NO = "'" + sLOT_NOSub + "_" + sSecurityCode + "_" + sDateTime + "P" + "'";

            //=====================Tray_Label
            //0044110826121008274930001
            string sTRAY_LABEL = "'"+ Stat.sTrayLabel_TrayLabel +"'" ;

            //=====================LotNo
            //
            string sLOT_NO = "'"+ sLOT_NOSub+"'" ;

            //=====================AVITTRK_AVIT
            string sAVITTRK_AVIT = "'" +_sMachineID + "'";

            //=====================T2_11_SERIES
            string sT2_11_SERIES = "'"+Stat.sLotTraveler_MaterialNo+"'" ;

            //======================OLD_LOT_NO
            string sOLD_LOT_NO = "null";

            //======================NG_REMARK
            string sNG_REMARK = "";
                 if(_eStat == cs.NG1 )sNG_REMARK = "MixDevice"     ;
            else if(_eStat == cs.NG2 )sNG_REMARK = "UnitID"        ;
            else if(_eStat == cs.NG3 )sNG_REMARK = "UnitDMC1"      ;
            else if(_eStat == cs.NG4 )sNG_REMARK = "UnitDMC2"      ;
            else if(_eStat == cs.NG5 )sNG_REMARK = "GlobtopLeft"   ;
            else if(_eStat == cs.NG6 )sNG_REMARK = "GlobtopTop"    ;
            else if(_eStat == cs.NG7 )sNG_REMARK = "GlobtopRight"  ;
            else if(_eStat == cs.NG8 )sNG_REMARK = "GlobtopBottom" ;
            else if(_eStat == cs.NG9 )sNG_REMARK = "MatchingError" ;
            else if(_eStat == cs.NG10)sNG_REMARK = "UserDefine"    ;    

            if(sNG_REMARK =="")sNG_REMARK = "'PASS'" ;
            else               sNG_REMARK = "'" + sNG_REMARK + "'";

            string sTableName = "";

            if (OM.CmnOptn.bUseApTestTable) { 
                if(_eStat != cs.Good) sTableName = "TBL_NG_UNIT_INSPECTION_NEW_AP";
                else                  sTableName = "TBL_UNIT_INSPECTION_AP";
            }
            else
            {
                if(_eStat != cs.Good) sTableName = "TBL_NG_UNIT_INSPECTION_NEW";
                else                  sTableName = "TBL_UNIT_INSPECTION";
            }


            //""	"UNIT_ID"	"DMC1"	"DMC2"	"DATE_TIME"	"RESULT"	"VIT_NO"	"TRAY_LABEL"	"LOT_NO"	"AVITTRK_AVIT"	"T2_11_SERIES"	"OLD_LOT_NO"	"NG_REMARK"
            int iCnt = 0 ;
            string sQuery = " Into " + sTableName + " Values" ;
            sQuery += "(";
            sQuery += sUNIT_INSPECTION_ID +"," ; //iCnt = sUNIT_INSPECTION_ID.Length;
            sQuery += sUNIT_ID            +"," ; //iCnt = sUNIT_ID           .Length;
            sQuery += sDMC1               +"," ; //iCnt = sDMC1              .Length;
            sQuery += sDMC2               +"," ; //iCnt = sDMC2              .Length;
            sQuery += sDATE_TIME          +"," ; //iCnt = sDATE_TIME         .Length;
            sQuery += sRESULT             +"," ; //iCnt = sRESULT            .Length;
            sQuery += sVIT_NO             +"," ; //iCnt = sVIT_NO            .Length;
            sQuery += sTRAY_LABEL         +"," ; //iCnt = sTRAY_LABEL        .Length;
            sQuery += sLOT_NO             +"," ; //iCnt = sLOT_NO            .Length;
            sQuery += sAVITTRK_AVIT       +"," ; //iCnt = sAVITTRK_AVIT      .Length;
            sQuery += sT2_11_SERIES       +"," ; //iCnt = sT2_11_SERIES      .Length;
            sQuery += sOLD_LOT_NO         ;
            
            //" Into TBL_UNIT_INSPECTION_AP Values('HZ7170CU98_009_171020_153217P_30','3LEC1AES','2124065264','04 5R ebxD68 127 01',CURRENT_TIMESTAMP,'MixDevice1##UnitID1##UnitDMC11##UnitDMC21##GlobtopLeft1##GlobtopTop1##GlobtopRight1##GlobtopBottom1##MatchingError1##UserDefine1##','HZ7170CU98_009_171020_153217P','0023110826111008822905009','HZ7170CU98','OSW-10a-01','11082611',null)"
            if(_eStat == cs.Good) sQuery+= ")";
            else                  sQuery+= ","+sNG_REMARK+") ";

            Stat.lsUnitInspection_Values. Add(sQuery);
            
            //lsUnitInspectionValues  //sUnitInspectionQuery += sQuery ;

            return true ;
        }

        public bool GetUnitInspectionQuery(string _sPocketID , string _sUnitID , string _sDMC1 , string _sDMC2 , cs _eStat ,string _sMachineID , ref string _sQuery)
        {

            //비전 엠프티는 안 넣는다.
            //요건 까먹을 까봐 일단 넣었는데 원래 밖에서 처리해야함.
            if(_eStat == cs.NG0) { 
                return true ;
            }
            //if (sUnitInspectionQuery != "")
            //{
            //    sUnitInspectionQuery += " ";
            //}
            //=====================UnitInspection ID
            //HZ6200A9.01은 Lot number로 lottraveler로 프린트 되나,  서버에 저장 시 “.”는 삭제되어 HZ6200A901로 저장
            string sLOT_NOSub        = Stat.sLotTraveler_LotNo.Replace(".","");          
            string sSecurityCode = Stat.sTrayLabel_SecurityCode ;
            string sDateTime     = DateTime.Now.ToString("yyMMdd_HHmmss");
            //HZ63700A99_001_160926_212440P_29
            string sUNIT_INSPECTION_ID = "'"+sLOT_NOSub + "_" + sSecurityCode + "_" + sDateTime + "P_" + _sPocketID+"'" ;

            //=====================UnitID
            string sUNIT_ID = "'" +_sUnitID +"'";

            //=====================DMC1
            string sDMC1 = "'" +_sDMC1 +"'";

            //=====================DMC2
            string sDMC2 = "'" +_sDMC2 +"'";

            //=====================DATE_TIME//'01-OCT-19 10.21.45.078000000 AM','DD-MON-RR HH.MI.SSXFF AM','NLS_DATE_LANGUAGE=american'
            string sDATE_TIME = "CURRENT_TIMESTAMP"; //"to_timestamp('06-SEP-17 05.29.28.252000000 AM','DD-MON-RR HH.MI.SSXFF AM','NLS_DATE_LANGUAGE=american')" ;

            //=====================RESULT
            //cs.NG0  V_Empty        //자제없음.5
            //cs.NG1  V_MixDevice    //4chip 5chip 비전에서 설정하기 나름인데 바뀔수도 있음.
            //cs.NG2  V_UnitID       
            //cs.NG3  V_UnitDMC1     
            //cs.NG4  V_UnitDMC2     
            //cs.NG5  V_GlobtopLeft  
            //cs.NG6  V_GlobtopTop   
            //cs.NG7  V_GlobtopRight 
            //cs.NG8  V_GlobtopBottom
            //cs.NG9  V_MatchingError
            //cs.NG10 V_UserDefine      

            //이상하게 RESULT에 bottom이 없고 Gloptop Left Right Top만 있음.
            //string sRESULT = "'" ;
            //sRESULT += "Unit Locator:"           + (                           "1##") ;
            //sRESULT += "Unit ID:"                + (_eStat == cs.NG2 ? "0##" : "1##") ;
            //sRESULT += "DMC 1:"                  + (_eStat == cs.NG3 ? "0##" : "1##") ;
            //sRESULT += "DMC 2:"                  + (_eStat == cs.NG4 ? "0##" : "1##") ;
            //sRESULT += "Glob Top Epoxy (LEFT):"  + (                           "1##") ;
            //sRESULT += "Glob Top Epoxy (RIGHT):" + (                           "1##") ;
            //sRESULT += "Unit Pattern (LEFT):"    + (                           "1##") ;
            //sRESULT += "Unit Pattern (RIGHT):"   + (_eStat == cs.NG9 ? "0##" : "1##") ;
            //sRESULT += "Glob Top (LEFT):"        + (_eStat == cs.NG5 ? "0##" : "1##") ;
            //sRESULT += "Glob Top (RIGHT):"       + (_eStat == cs.NG7 ? "0##" : "1##") ;
            //sRESULT += "Glob Top (TOP):"         + (_eStat == cs.NG6 ? "0##" : "1##") ;
            //sRESULT += "'" ;
            string sRESULT = "'" ;
            sRESULT += "MixDevice:"     + (_eStat == cs.NG1  ? "0##" : "1##") ;   
            sRESULT += "UnitID:"        + (_eStat == cs.NG2  ? "0##" : "1##") ;
            sRESULT += "UnitDMC1:"      + (_eStat == cs.NG3  ? "0##" : "1##") ;
            sRESULT += "UnitDMC2:"      + (_eStat == cs.NG4  ? "0##" : "1##") ;
            sRESULT += "GlobtopLeft:"   + (_eStat == cs.NG5  ? "0##" : "1##") ;   
            sRESULT += "GlobtopTop:"    + (_eStat == cs.NG6  ? "0##" : "1##") ;   
            sRESULT += "GlobtopRight:"  + (_eStat == cs.NG7  ? "0##" : "1##") ;   
            sRESULT += "GlobtopBottom:" + (_eStat == cs.NG8  ? "0##" : "1##") ;
            sRESULT += "MatchingError:" + (_eStat == cs.NG9  ? "0##" : "1##") ;
            sRESULT += "UserDefine:"    + (_eStat == cs.NG10 ? "0##" : "1##") ;
            sRESULT += "'" ;

            //=====================VIT_NO
            //HZ63700A99_001_160926_212440P
            string sVIT_NO = "'" + sLOT_NOSub + "_" + sSecurityCode + "_" + sDateTime + "P" + "'";

            //=====================Tray_Label
            //0044110826121008274930001
            string sTRAY_LABEL = "'"+ Stat.sTrayLabel_TrayLabel +"'" ;

            //=====================LotNo
            //
            string sLOT_NO = "'"+ sLOT_NOSub+"'" ;

            //=====================AVITTRK_AVIT
            string sAVITTRK_AVIT = "'" +_sMachineID + "'";

            //=====================T2_11_SERIES
            string sT2_11_SERIES = "'"+Stat.sLotTraveler_MaterialNo+"'" ;

            //======================OLD_LOT_NO
            string sOLD_LOT_NO = "null";

            //======================NG_REMARK
            string sNG_REMARK = "";
                 if(_eStat == cs.NG1 )sNG_REMARK = "MixDevice"     ;
            else if(_eStat == cs.NG2 )sNG_REMARK = "UnitID"        ;
            else if(_eStat == cs.NG3 )sNG_REMARK = "UnitDMC1"      ;
            else if(_eStat == cs.NG4 )sNG_REMARK = "UnitDMC2"      ;
            else if(_eStat == cs.NG5 )sNG_REMARK = "GlobtopLeft"   ;
            else if(_eStat == cs.NG6 )sNG_REMARK = "GlobtopTop"    ;
            else if(_eStat == cs.NG7 )sNG_REMARK = "GlobtopRight"  ;
            else if(_eStat == cs.NG8 )sNG_REMARK = "GlobtopBottom" ;
            else if(_eStat == cs.NG9 )sNG_REMARK = "MatchingError" ;
            else if(_eStat == cs.NG10)sNG_REMARK = "UserDefine"    ;    

            if(sNG_REMARK =="")sNG_REMARK = "'PASS'" ;
            else               sNG_REMARK = "'" + sNG_REMARK + "'";

            string sTableName = "";

            if (OM.CmnOptn.bUseApTestTable) { 
                if(_eStat != cs.Good) sTableName = "TBL_NG_UNIT_INSPECTION_NEW_AP";
                else                  sTableName = "TBL_UNIT_INSPECTION_AP";
            }
            else
            {
                if(_eStat != cs.Good) sTableName = "TBL_NG_UNIT_INSPECTION_NEW";
                else                  sTableName = "TBL_UNIT_INSPECTION";
            }


            //""	"UNIT_ID"	"DMC1"	"DMC2"	"DATE_TIME"	"RESULT"	"VIT_NO"	"TRAY_LABEL"	"LOT_NO"	"AVITTRK_AVIT"	"T2_11_SERIES"	"OLD_LOT_NO"	"NG_REMARK"
            int iCnt = 0 ;
            string sQuery = " Into " + sTableName + " Values" ;
            sQuery += "(";
            sQuery += sUNIT_INSPECTION_ID +"," ; //iCnt = sUNIT_INSPECTION_ID.Length;
            sQuery += sUNIT_ID            +"," ; //iCnt = sUNIT_ID           .Length;
            sQuery += sDMC1               +"," ; //iCnt = sDMC1              .Length;
            sQuery += sDMC2               +"," ; //iCnt = sDMC2              .Length;
            sQuery += sDATE_TIME          +"," ; //iCnt = sDATE_TIME         .Length;
            sQuery += sRESULT             +"," ; //iCnt = sRESULT            .Length;
            sQuery += sVIT_NO             +"," ; //iCnt = sVIT_NO            .Length;
            sQuery += sTRAY_LABEL         +"," ; //iCnt = sTRAY_LABEL        .Length;
            sQuery += sLOT_NO             +"," ; //iCnt = sLOT_NO            .Length;
            sQuery += sAVITTRK_AVIT       +"," ; //iCnt = sAVITTRK_AVIT      .Length;
            sQuery += sT2_11_SERIES       +"," ; //iCnt = sT2_11_SERIES      .Length;
            sQuery += sOLD_LOT_NO         ;
            
            //" Into TBL_UNIT_INSPECTION_AP Values('HZ7170CU98_009_171020_153217P_30','3LEC1AES','2124065264','04 5R ebxD68 127 01',CURRENT_TIMESTAMP,'MixDevice1##UnitID1##UnitDMC11##UnitDMC21##GlobtopLeft1##GlobtopTop1##GlobtopRight1##GlobtopBottom1##MatchingError1##UserDefine1##','HZ7170CU98_009_171020_153217P','0023110826111008822905009','HZ7170CU98','OSW-10a-01','11082611',null)"
            if(_eStat == cs.Good) sQuery+= ")";
            else                  sQuery+= ","+sNG_REMARK+") ";


            _sQuery = sQuery ;

            return true ;
        }

        public bool InsertUnitInspectionQuery(string _sPocketID , string _sUnitID , string _sDMC1 , string _sDMC2 , cs _eStat ,string _sMachineID)
        {

            //비전 엠프티는 안 넣는다.
            //요건 까먹을 까봐 일단 넣었는데 원래 밖에서 처리해야함.
            if(_eStat == cs.NG0) { 
                return true ;
            }
            //if (sUnitInspectionQuery != "")
            //{
            //    sUnitInspectionQuery += " ";
            //}
            //=====================UnitInspection ID
            //HZ6200A9.01은 Lot number로 lottraveler로 프린트 되나,  서버에 저장 시 “.”는 삭제되어 HZ6200A901로 저장
            string sLOT_NOSub        = Stat.sLotTraveler_LotNo.Replace(".","");          
            string sSecurityCode = Stat.sTrayLabel_SecurityCode ;
            string sDateTime     = DateTime.Now.ToString("yyMMdd_HHmmss");
            //HZ63700A99_001_160926_212440P_29
            string sUNIT_INSPECTION_ID = "'"+sLOT_NOSub + "_" + sSecurityCode + "_" + sDateTime + "P_" + _sPocketID+"'" ;

            //=====================UnitID
            string sUNIT_ID = "'" +_sUnitID +"'";

            //=====================DMC1
            string sDMC1 = "'" +_sDMC1 +"'";

            //=====================DMC2
            string sDMC2 = "'" +_sDMC2 +"'";

            //=====================DATE_TIME//'01-OCT-19 10.21.45.078000000 AM','DD-MON-RR HH.MI.SSXFF AM','NLS_DATE_LANGUAGE=american'
            string sDATE_TIME = "CURRENT_TIMESTAMP"; //"to_timestamp('06-SEP-17 05.29.28.252000000 AM','DD-MON-RR HH.MI.SSXFF AM','NLS_DATE_LANGUAGE=american')" ;

            //=====================RESULT
            //cs.NG0  V_Empty        //자제없음.5
            //cs.NG1  V_MixDevice    //4chip 5chip 비전에서 설정하기 나름인데 바뀔수도 있음.
            //cs.NG2  V_UnitID       
            //cs.NG3  V_UnitDMC1     
            //cs.NG4  V_UnitDMC2     
            //cs.NG5  V_GlobtopLeft  
            //cs.NG6  V_GlobtopTop   
            //cs.NG7  V_GlobtopRight 
            //cs.NG8  V_GlobtopBottom
            //cs.NG9  V_MatchingError
            //cs.NG10 V_UserDefine      

            //이상하게 RESULT에 bottom이 없고 Gloptop Left Right Top만 있음.
            //string sRESULT = "'" ;
            //sRESULT += "Unit Locator:"           + (                           "1##") ;
            //sRESULT += "Unit ID:"                + (_eStat == cs.NG2 ? "0##" : "1##") ;
            //sRESULT += "DMC 1:"                  + (_eStat == cs.NG3 ? "0##" : "1##") ;
            //sRESULT += "DMC 2:"                  + (_eStat == cs.NG4 ? "0##" : "1##") ;
            //sRESULT += "Glob Top Epoxy (LEFT):"  + (                           "1##") ;
            //sRESULT += "Glob Top Epoxy (RIGHT):" + (                           "1##") ;
            //sRESULT += "Unit Pattern (LEFT):"    + (                           "1##") ;
            //sRESULT += "Unit Pattern (RIGHT):"   + (_eStat == cs.NG9 ? "0##" : "1##") ;
            //sRESULT += "Glob Top (LEFT):"        + (_eStat == cs.NG5 ? "0##" : "1##") ;
            //sRESULT += "Glob Top (RIGHT):"       + (_eStat == cs.NG7 ? "0##" : "1##") ;
            //sRESULT += "Glob Top (TOP):"         + (_eStat == cs.NG6 ? "0##" : "1##") ;
            //sRESULT += "'" ;
            string sRESULT = "'" ;
            sRESULT += "MixDevice:"     + (_eStat == cs.NG1  ? "0##" : "1##") ;   
            sRESULT += "UnitID:"        + (_eStat == cs.NG2  ? "0##" : "1##") ;
            sRESULT += "UnitDMC1:"      + (_eStat == cs.NG3  ? "0##" : "1##") ;
            sRESULT += "UnitDMC2:"      + (_eStat == cs.NG4  ? "0##" : "1##") ;
            sRESULT += "GlobtopLeft:"   + (_eStat == cs.NG5  ? "0##" : "1##") ;   
            sRESULT += "GlobtopTop:"    + (_eStat == cs.NG6  ? "0##" : "1##") ;   
            sRESULT += "GlobtopRight:"  + (_eStat == cs.NG7  ? "0##" : "1##") ;   
            sRESULT += "GlobtopBottom:" + (_eStat == cs.NG8  ? "0##" : "1##") ;
            sRESULT += "MatchingError:" + (_eStat == cs.NG9  ? "0##" : "1##") ;
            sRESULT += "UserDefine:"    + (_eStat == cs.NG10 ? "0##" : "1##") ;
            sRESULT += "'" ;

            //=====================VIT_NO
            //HZ63700A99_001_160926_212440P
            string sVIT_NO = "'" + sLOT_NOSub + "_" + sSecurityCode + "_" + sDateTime + "P" + "'";

            //=====================Tray_Label
            //0044110826121008274930001
            string sTRAY_LABEL = "'"+ Stat.sTrayLabel_TrayLabel +"'" ;

            //=====================LotNo
            //
            string sLOT_NO = "'"+ sLOT_NOSub+"'" ;

            //=====================AVITTRK_AVIT
            string sAVITTRK_AVIT = "'" +_sMachineID + "'";

            //=====================T2_11_SERIES
            string sT2_11_SERIES = "'"+Stat.sLotTraveler_MaterialNo+"'" ;

            //======================OLD_LOT_NO
            string sOLD_LOT_NO = "null";

            //======================NG_REMARK
            string sNG_REMARK = "";
                 if(_eStat == cs.NG1 )sNG_REMARK = "MixDevice"     ;
            else if(_eStat == cs.NG2 )sNG_REMARK = "UnitID"        ;
            else if(_eStat == cs.NG3 )sNG_REMARK = "UnitDMC1"      ;
            else if(_eStat == cs.NG4 )sNG_REMARK = "UnitDMC2"      ;
            else if(_eStat == cs.NG5 )sNG_REMARK = "GlobtopLeft"   ;
            else if(_eStat == cs.NG6 )sNG_REMARK = "GlobtopTop"    ;
            else if(_eStat == cs.NG7 )sNG_REMARK = "GlobtopRight"  ;
            else if(_eStat == cs.NG8 )sNG_REMARK = "GlobtopBottom" ;
            else if(_eStat == cs.NG9 )sNG_REMARK = "MatchingError" ;
            else if(_eStat == cs.NG10)sNG_REMARK = "UserDefine"    ;    

            if(sNG_REMARK =="")sNG_REMARK = "'PASS'" ;
            else               sNG_REMARK = "'" + sNG_REMARK + "'";

            string sTableName = "";

            if (OM.CmnOptn.bUseApTestTable) { 
                if(_eStat != cs.Good) sTableName = "TBL_NG_UNIT_INSPECTION_NEW_AP";
                else                  sTableName = "TBL_UNIT_INSPECTION_AP";
            }
            else
            {
                if(_eStat != cs.Good) sTableName = "TBL_NG_UNIT_INSPECTION_NEW";
                else                  sTableName = "TBL_UNIT_INSPECTION";
            }


            //""	"UNIT_ID"	"DMC1"	"DMC2"	"DATE_TIME"	"RESULT"	"VIT_NO"	"TRAY_LABEL"	"LOT_NO"	"AVITTRK_AVIT"	"T2_11_SERIES"	"OLD_LOT_NO"	"NG_REMARK"
            int iCnt = 0 ;
            string sQuery = "Insert Into " + sTableName + " Values" ;
            sQuery += "(";
            sQuery += sUNIT_INSPECTION_ID +"," ; //iCnt = sUNIT_INSPECTION_ID.Length;
            sQuery += sUNIT_ID            +"," ; //iCnt = sUNIT_ID           .Length;
            sQuery += sDMC1               +"," ; //iCnt = sDMC1              .Length;
            sQuery += sDMC2               +"," ; //iCnt = sDMC2              .Length;
            sQuery += sDATE_TIME          +"," ; //iCnt = sDATE_TIME         .Length;
            sQuery += sRESULT             +"," ; //iCnt = sRESULT            .Length;
            sQuery += sVIT_NO             +"," ; //iCnt = sVIT_NO            .Length;
            sQuery += sTRAY_LABEL         +"," ; //iCnt = sTRAY_LABEL        .Length;
            sQuery += sLOT_NO             +"," ; //iCnt = sLOT_NO            .Length;
            sQuery += sAVITTRK_AVIT       +"," ; //iCnt = sAVITTRK_AVIT      .Length;
            sQuery += sT2_11_SERIES       +"," ; //iCnt = sT2_11_SERIES      .Length;
            sQuery += sOLD_LOT_NO         ;
            
            //" Into TBL_UNIT_INSPECTION_AP Values('HZ7170CU98_009_171020_153217P_30','3LEC1AES','2124065264','04 5R ebxD68 127 01',CURRENT_TIMESTAMP,'MixDevice1##UnitID1##UnitDMC11##UnitDMC21##GlobtopLeft1##GlobtopTop1##GlobtopRight1##GlobtopBottom1##MatchingError1##UserDefine1##','HZ7170CU98_009_171020_153217P','0023110826111008822905009','HZ7170CU98','OSW-10a-01','11082611',null)"
            if(_eStat == cs.Good) sQuery+= ")";
            else                  sQuery+= ","+sNG_REMARK+") ";

            return Insert(sQuery);
        }

        public bool SendUnitInspectionQueryOneByOne()
        {
            //sUnitInspectionQuery = "Insert all " ;
            //sUnitInspectionQuery+= "into TBL_TEST Values ('sun','kye') ";
            //sUnitInspectionQuery+= "into TBL_TEST Values ('ddd','vvv') ";
            //sUnitInspectionQuery+= "into TBL_TEST Values ('fff','sss') ";
            //sUnitInspectionQuery+= "select * from dual";
            if (Stat.lsUnitInspection_Values.Count == 0)
            {
                SendMsg("lsUnitInspectionValues's count is 0");
                return false ;
            }

            string sQuery = "";
            
            bool bRet = true ;
            foreach(string sValue in Stat.lsUnitInspection_Values)
            {
                sQuery = "Insert " + sValue ;
                bRet = Insert(sQuery);
            }
            Stat.lsUnitInspection_Values.Clear();
            
            return bRet ;
        }

        public bool SendUnitInspectionQuery()
        {
            //sUnitInspectionQuery = "Insert all " ;
            //sUnitInspectionQuery+= "into TBL_TEST Values ('sun','kye') ";
            //sUnitInspectionQuery+= "into TBL_TEST Values ('ddd','vvv') ";
            //sUnitInspectionQuery+= "into TBL_TEST Values ('fff','sss') ";
            //sUnitInspectionQuery+= "select * from dual";
            if (Stat.lsUnitInspection_Values.Count == 0)
            {
                SendMsg("lsUnitInspectionValues's count is 0");
                return false ;
            }

            string sQuery = "";
            sQuery += "Insert all " ;
            foreach(string sValue in Stat.lsUnitInspection_Values)
            {
                sQuery += sValue ;
            }

            sQuery += "select * from dual";

            
            //SendMsg("Query length=" + sQuery.Length.ToString());
            bool bRet = Insert(sQuery);
            Stat.lsUnitInspection_Values.Clear();
            return bRet ;
        }
        public void ClearUnitInspectionQuery()
        {
            Stat.lsUnitInspection_Values.Clear();
        }

        public bool InsertVIT(string _sMachineID    , 
                              string _sOperID       , 
                              string _sSttTime      , 
                              string _sStpTime      )
        {
            //=====================VIT_NO HZ65118E78_009_161225_153133P            //0044110826121008274930001 
            //HZ6200A9.01은 Lot number로 lottraveler로 프린트 되나,  서버에 저장 시 “.”는 삭제되어 HZ6200A901로 저장
            string sLOT_NOSub    = Stat.sLotTraveler_LotNo.Replace(".","");     
            string sSecurityCode = Stat.sTrayLabel_SecurityCode;
            string sDateTime     = DateTime.Now.ToString("yyMMdd_HHmmss");
            string sVIT_NO = "'"+sLOT_NOSub + "_" + sSecurityCode + "_" + sDateTime + "P'" ;
            //=====================VIT_DATE 25/12/2016
            string sVIT_DATE = "'" + DateTime.Now.ToString("dd\\/MM\\/yyyy") +"'";

            //=====================AVITTRK_AVIT AOSF03
            string sAVITTRK_AVIT = "'" +_sMachineID +"'";

            //=====================OPT OS26021
            string sOPT = "'" +_sOperID +"'";

            //=====================START_TIME
            string sSTART_TIME = "'" +_sSttTime +"'";

            //=====================STOP_TIME
            string sSTOP_TIME = "'" +_sStpTime +"'";

            //=====================BATCH_NO
            string sBATCH_NO = "'" + Stat.sTrayLabel_BatchNo + "'" ;//여기부터

            //=====================LOT_NO
            string sLOT_NO = "'" + sLOT_NOSub + "'" ;

            //=====================DC
            string sDC = "null" ;

            //=====================PROD_NO
            string sPROD_NO = "'" + Stat.sLotTraveler_MaterialNo + "'";

            //=====================VIT_GROUP
            int    iVIT_GROUP = 0 ;
            string sVIT_GROUP = Stat.sTrayLabel_GroupingNo ;
            if(!int.TryParse(sVIT_GROUP , out iVIT_GROUP)){
                SendMsg("VIT_GROUP:" + sVIT_GROUP +" Pharsing Failed!");
                return false ;
            }            
            sVIT_GROUP = "'" + iVIT_GROUP.ToString() + "'";

            //=====================SC
            string sSC = "'" + sSecurityCode + "'" ;

            //=====================QTY
            string sQTY = "'" + Stat.sTrayInfomation_BigTrayQty + "'" ;

            //=====================FAIL
            string sFAIL = "'0'" ;

            //=====================PASS
            string sPASS = "'" + Stat.sTrayInfomation_BigTrayQty + "'" ;

            //=====================STATUS
            string sSTATUS = "'PASS'";

            //=====================TRAY_LABEL
            string sTRAY_LABEL = "'" + Stat.sTrayLabel_TrayLabel + "'";

            //=====================OLD_LOT_NO
            string sOLD_LOT_NO = "null";

            string sQuery ;
            sQuery  = "insert into TBL_VIT_AP values(" ;
            sQuery += sVIT_NO       + "," ;
            sQuery += sVIT_DATE     + "," ;
            sQuery += sAVITTRK_AVIT + "," ;
            sQuery += sOPT          + "," ;
            sQuery += sSTART_TIME   + "," ;
            sQuery += sSTOP_TIME    + "," ;
            sQuery += sBATCH_NO     + "," ;
            sQuery += sLOT_NO       + "," ;
            sQuery += sDC           + "," ;
            sQuery += sPROD_NO      + "," ;
            sQuery += sVIT_GROUP    + "," ;
            sQuery += sSC           + "," ;
            sQuery += sQTY          + "," ;
            sQuery += sFAIL         + "," ;
            sQuery += sPASS         + "," ;
            sQuery += sSTATUS       + "," ;
            sQuery += sTRAY_LABEL   + "," ;
            sQuery += sOLD_LOT_NO   + ")" ;

            bool bRet = Insert(sQuery);
            return bRet ;
        }

        public bool CheckDMC2CharacterCompare(string _sDMC2)
        {
            if(OM.DevOptn.iDMC2CheckMathod != 1) return true ;
            

            //A3 b 1800 == 9ea  
            if(_sDMC2.Length < OM.DevOptn.iCompareDmc2Cnt) 
            {
                SendMsg("DMC2 is under " + OM.DevOptn.iCompareDmc2Cnt + " character!");
                return false ;
            }
            

            if(OM.DevOptn.bUseDmc2CharLimit)
            {
                //A3 b 1800 == 9ea  
                if(_sDMC2.Length > OM.DevOptn.iDmc2CharLimit) 
                {
                    SendMsg("DMC2 is Over " + OM.DevOptn.iDmc2CharLimit + " character!");
                    return false ;
                }
            }

            string sGrouping = _sDMC2.Substring(0,OM.DevOptn.iCompareDmc2Cnt);

            //bool bRet =  ;

            if (Stat.sTrayInfomation_DeviceNumber!=sGrouping)
            {
                SendMsg("Grouping:"+sGrouping + " is Not Same as DeviceNo:" + Stat.sTrayInfomation_DeviceNumber);
                return false ;
            }
            return true ;
        }

        //다각형 내부 여부 확인.
        //다각형이 4각형이면 데이터는 총5개로 마지막 인자는 첫째 인자와 같은 위치여야 한다.
        bool IsPointInPolygon(List<PointD> _lsPolygon, PointD _pdPoint)
        {
            bool result = false;
         
            int j = _lsPolygon.Count - 1;
         
            for(int i = 0; i < _lsPolygon.Count; i++)
            {
                if(_lsPolygon[i].Y <  _pdPoint.Y &&
                   _lsPolygon[j].Y >= _pdPoint.Y ||
                   _lsPolygon[j].Y <  _pdPoint.Y &&
                   _lsPolygon[i].Y >= _pdPoint.Y)
                {
                    double value = _lsPolygon[i].X + (_pdPoint.Y - _lsPolygon[i].Y) / (_lsPolygon[j].Y - _lsPolygon[i].Y) *
                        (_lsPolygon[j].X - _lsPolygon[i].X);
         
                    if(value < _pdPoint.X)
                    {
                        result =! result;
                    }
                }
         
                j = i;
            }         
            return result;
        }

        public bool CheckDMC2_Spec(string _sDMC2)
        {
            if(OM.DevOptn.iDMC2CheckMathod != 2) return true ;
            char [] cSeperator = {' '};
            string[] sItems = _sDMC2.Split(cSeperator);

            if(sItems.Count() != 5) {
                SendMsg("DMC2 is Under 5 Words!");
                return false ;
            }           

            double dBrightness = 0 ;
            double dLdom       = 0 ;
            double dCX         = 0 ;
            double dCY         = 0 ;

            if(!double.TryParse(sItems[0] , out dBrightness)) return false ;
            if(!double.TryParse(sItems[2] , out dLdom      )) return false ;
            if(!double.TryParse(sItems[3] , out dCX        )) return false ;
            if(!double.TryParse(sItems[4] , out dCY        )) return false ;

            if(OM.DevOptn.bBrightnessCheck) {
                if(dBrightness < Stat.dDmc2Value_Pmin || dBrightness > Stat.dDmc2Value_Pmax) {
                    SendMsg("Mix Bin - PMIN:"+Stat.dDmc2Value_Pmin + " PMAX:"+Stat.dDmc2Value_Pmax + " Brightness=" + dBrightness );
                    return false ;
                }
            }
            //1로 설정된경우는 brightness와 Ldom만을 비교.  
            //설정이 2 인 경우 brightness, Cx 및 Cy를 비교하는 것이다. 
            //Brightness 판독은 PMIN 및 PMAX 범위 내에 있어야 하며,  Out of the spec시 "Mix Bin"과 함께 오류 메시지를 띄운다.
            if (Stat.iDmc2Value_InspectionType == 1) { 
                if(OM.DevOptn.bLDOMCheck) {
                    if(dLdom < Stat.dDmc2Value_LMin || dLdom > Stat.dDmc2Value_LMax) {
                        //아웃오브스펙.
                        SendMsg("Mix Bin - LMIN:"+Stat.dDmc2Value_LMin + " LMAX:"+Stat.dDmc2Value_LMax + " Ldom=" + dLdom );
                        return false ;
                    }
                }
                //Ldom판독은 LMIN and LMAX범위 내에 있어야 하며, Out of the spec시 “Mix Bin”과 함께 오류메시지를 띄운다.
            }
            else if(Stat.iDmc2Value_InspectionType == 2){
                if(OM.DevOptn.bCxCy) {
                    List<PointD> lsPoly = new List<PointD>();
                    
                    lsPoly.Add(new PointD(Stat.dDmc2Value_Cx1 , Stat.dDmc2Value_Cy1));
                    lsPoly.Add(new PointD(Stat.dDmc2Value_Cx2 , Stat.dDmc2Value_Cy2));
                    lsPoly.Add(new PointD(Stat.dDmc2Value_Cx3 , Stat.dDmc2Value_Cy3));
                    lsPoly.Add(new PointD(Stat.dDmc2Value_Cx4 , Stat.dDmc2Value_Cy4));
                    lsPoly.Add(new PointD(Stat.dDmc2Value_Cx1 , Stat.dDmc2Value_Cy1));
                    if(!IsPointInPolygon(lsPoly , new PointD(dCX , dCY))) {
                        SendMsg("Mix Bin - Cx:"+dCX + " Cy:"+dCY );
                        return false ;
                    }
                }
                //Cx 및 Cy  스펙은 사각형으로 구성되어야 하며 DMC 2의 값은 사각형 내에 있어야 한다. 
                //샘플은 다음과 같다. 요점은  사각형 밖에 있을 시, 이는 자재가 섞여 있음을  뜻하고,  오류메시지를 띄워야 한다.
            }
            
            return true ;
        }

        public void LoadSave(bool _bLoad) 
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string sPath = sExeFolder + "SeqData\\OracleStat.ini";

            if(!_bLoad) System.IO.File.Delete(sPath);

            CIniFile Ini = new CIniFile(sPath);

            //this one has lot of Datas  over 100000 So INI too Late 
            //Reconnect is better.너무 데이터가 많아서 그냥 다시 디비에서 가져오는것이 빠름.
            //LoadSaveList(_bLoad, "lsUnitInspection_UnitID"               , ref Ini , ref  Stat.lsUnitInspection_UnitID             );
            //LoadSaveList(_bLoad, "lsUnitInspection_DMC1"                 , ref Ini , ref  Stat.lsUnitInspection_DMC1               );
            //LoadSaveList(_bLoad, "lsProbeFile_ProbeFile"                 , ref Ini , ref  Stat.lsProbeFile_ProbeFile               );

            LoadSaveList(_bLoad, "lsTrayInfomation_SmallMaterialNumber"  , ref Ini , ref  Stat.lsTrayInfomation_SmallMaterialNumber);
            LoadSaveList(_bLoad, "lsUnitInspection_Values"               , ref Ini , ref  Stat.lsUnitInspection_Values             );

            if(_bLoad){
                Ini.Load("Stat" ,"sLotTraveler_LotNo          ",out Stat.sLotTraveler_LotNo          );
                Ini.Load("Stat" ,"sLotTraveler_MaterialNo     ",out Stat.sLotTraveler_MaterialNo     );
                Ini.Load("Stat" ,"sLotTraveler_LotAlias       ",out Stat.sLotTraveler_LotAlias       );
                Ini.Load("Stat" ,"sVisionRecipe_RecipeName    ",out Stat.sVisionRecipe_RecipeName    );
                Ini.Load("Stat" ,"sTrayLabel_TrayLabel        ",out Stat.sTrayLabel_TrayLabel        );
                Ini.Load("Stat" ,"sTrayLabel_GroupingNo       ",out Stat.sTrayLabel_GroupingNo       );
                Ini.Load("Stat" ,"sTrayLabel_MaterialNo       ",out Stat.sTrayLabel_MaterialNo       );
                Ini.Load("Stat" ,"sTrayLabel_BatchNo          ",out Stat.sTrayLabel_BatchNo          );
                Ini.Load("Stat" ,"sTrayLabel_SecurityCode     ",out Stat.sTrayLabel_SecurityCode     );
                Ini.Load("Stat" ,"sTrayInfomation_DMC1        ",out Stat.sTrayInfomation_DMC1        );
                Ini.Load("Stat" ,"sTrayInfomation_DeviceNumber",out Stat.sTrayInfomation_DeviceNumber);
                Ini.Load("Stat" ,"sTrayInfomation_Bin         ",out Stat.sTrayInfomation_Bin         );
                Ini.Load("Stat" ,"sTrayInfomation_BigTrayQty  ",out Stat.sTrayInfomation_BigTrayQty  );
                Ini.Load("Stat" ,"iDmc2Value_InspectionType   ",out Stat.iDmc2Value_InspectionType   );
                Ini.Load("Stat" ,"dDmc2Value_Pmin             ",out Stat.dDmc2Value_Pmin             );
                Ini.Load("Stat" ,"dDmc2Value_Pmax             ",out Stat.dDmc2Value_Pmax             );
                Ini.Load("Stat" ,"dDmc2Value_LMin             ",out Stat.dDmc2Value_LMin             );
                Ini.Load("Stat" ,"dDmc2Value_LMax             ",out Stat.dDmc2Value_LMax             );
                Ini.Load("Stat" ,"dDmc2Value_Cx1              ",out Stat.dDmc2Value_Cx1              );
                Ini.Load("Stat" ,"dDmc2Value_Cy1              ",out Stat.dDmc2Value_Cy1              );
                Ini.Load("Stat" ,"dDmc2Value_Cx2              ",out Stat.dDmc2Value_Cx2              );
                Ini.Load("Stat" ,"dDmc2Value_Cy2              ",out Stat.dDmc2Value_Cy2              );
                Ini.Load("Stat" ,"dDmc2Value_Cx3              ",out Stat.dDmc2Value_Cx3              );
                Ini.Load("Stat" ,"dDmc2Value_Cy3              ",out Stat.dDmc2Value_Cy3              );
                Ini.Load("Stat" ,"dDmc2Value_Cx4              ",out Stat.dDmc2Value_Cx4              );
                Ini.Load("Stat" ,"dDmc2Value_Cy4              ",out Stat.dDmc2Value_Cy4              );
            }
            else {
                Ini.Save("Stat" ,"sLotTraveler_LotNo          ",    Stat.sLotTraveler_LotNo          );
                Ini.Save("Stat" ,"sLotTraveler_MaterialNo     ",    Stat.sLotTraveler_MaterialNo     );
                Ini.Save("Stat" ,"sLotTraveler_LotAlias       ",    Stat.sLotTraveler_LotAlias       );
                Ini.Save("Stat" ,"sVisionRecipe_RecipeName    ",    Stat.sVisionRecipe_RecipeName    );
                Ini.Save("Stat" ,"sTrayLabel_TrayLabel        ",    Stat.sTrayLabel_TrayLabel        );
                Ini.Save("Stat" ,"sTrayLabel_GroupingNo       ",    Stat.sTrayLabel_GroupingNo       );
                Ini.Save("Stat" ,"sTrayLabel_MaterialNo       ",    Stat.sTrayLabel_MaterialNo       );
                Ini.Save("Stat" ,"sTrayLabel_BatchNo          ",    Stat.sTrayLabel_BatchNo          );
                Ini.Save("Stat" ,"sTrayLabel_SecurityCode     ",    Stat.sTrayLabel_SecurityCode     );
                Ini.Save("Stat" ,"sTrayInfomation_DMC1        ",    Stat.sTrayInfomation_DMC1        );
                Ini.Save("Stat" ,"sTrayInfomation_DeviceNumber",    Stat.sTrayInfomation_DeviceNumber);
                Ini.Save("Stat" ,"sTrayInfomation_Bin         ",    Stat.sTrayInfomation_Bin         );
                Ini.Save("Stat" ,"sTrayInfomation_BigTrayQty  ",    Stat.sTrayInfomation_BigTrayQty  );
                Ini.Save("Stat" ,"iDmc2Value_InspectionType   ",    Stat.iDmc2Value_InspectionType   );
                Ini.Save("Stat" ,"dDmc2Value_Pmin             ",    Stat.dDmc2Value_Pmin             );
                Ini.Save("Stat" ,"dDmc2Value_Pmax             ",    Stat.dDmc2Value_Pmax             );
                Ini.Save("Stat" ,"dDmc2Value_LMin             ",    Stat.dDmc2Value_LMin             );
                Ini.Save("Stat" ,"dDmc2Value_LMax             ",    Stat.dDmc2Value_LMax             );
                Ini.Save("Stat" ,"dDmc2Value_Cx1              ",    Stat.dDmc2Value_Cx1              );
                Ini.Save("Stat" ,"dDmc2Value_Cy1              ",    Stat.dDmc2Value_Cy1              );
                Ini.Save("Stat" ,"dDmc2Value_Cx2              ",    Stat.dDmc2Value_Cx2              );
                Ini.Save("Stat" ,"dDmc2Value_Cy2              ",    Stat.dDmc2Value_Cy2              );
                Ini.Save("Stat" ,"dDmc2Value_Cx3              ",    Stat.dDmc2Value_Cx3              );
                Ini.Save("Stat" ,"dDmc2Value_Cy3              ",    Stat.dDmc2Value_Cy3              );
                Ini.Save("Stat" ,"dDmc2Value_Cx4              ",    Stat.dDmc2Value_Cx4              );
                Ini.Save("Stat" ,"dDmc2Value_Cy4              ",    Stat.dDmc2Value_Cy4              );
            }
        }

        public void LoadSaveList(bool _bLoad, string _sName ,ref CIniFile _cIni , ref List<string> _lsList)
        {
            if(!_bLoad)
            {
                _cIni.Save(_sName,"Count" , _lsList.Count);
                for (int i = 0; i < _lsList.Count; i++)
                {
                    _cIni.Save(_sName,i.ToString() , _lsList[i]);
                }
            }
            else
            {
                int iCount = 0 ;
                _cIni.Load(_sName,"Count" , out iCount);
                string sValue = "";
                for (int i = 0; i < iCount; i++)
                {
                    _cIni.Load(_sName,i.ToString() , out sValue);
                    _lsList.Add(sValue);
                }
            }
        }     
   
        public bool WriteVIT(string _sFolderPath  , 
                             //string _sFileName    ,//HZ7290XH98_006_170928_143116P
                             string _sDate        ,//9/28/2017
                             string _sAVIT        ,//AOSF01
                             string _sOPT         ,//OS35963
                             string _sSTART_TIME  ,//14:29:33
                             string _sSTOP_TIME   ,//14:31:16
                             string _sQty         )//600
        {
            //            
            string sLOT_NOSub    = Stat.sLotTraveler_LotNo.Replace(".","");          
            string sSecurityCode = Stat.sTrayLabel_SecurityCode ;
            string sDateTime     = DateTime.Now.ToString("yyMMdd_HHmmss");
            string sFilePath = _sFolderPath + "\\"+sLOT_NOSub + "_" + "999" + "_" + sDateTime + "P.txt"  ;
            try
            {
                if (!Directory.Exists(_sFolderPath))
                {
                    Directory.CreateDirectory(_sFolderPath);
                }
                if (!Directory.Exists(_sFolderPath)) {
                    SendMsg("Make VIT Folder Failed! ");
                    return false ;
                }

                using (StreamWriter sw = new StreamWriter(sFilePath))
                {
                    //DATE		AVITTRK	AVIT	OPT	START_TIME	STOP_TIME	BATCH_NO	LOT_NO		DC	PROD_NO		GROUP	SC	QTY	FAIL	PASS	STATUS
                    //9/28/2017		AOSF01	OS35963	14:29:33	14:31:16	1009219812	HZ7290XH98		11085106	15	006	60	0	60	PASS
                    string sBATCH_NO = Stat.sTrayLabel_BatchNo                ;
                    string sLOT_NO   = Stat.sLotTraveler_LotNo.Replace(".","");
                    string sDC       = "" ;
                    string sPROD_NO  = Stat.sLotTraveler_MaterialNo ;                    
                    int    iVIT_GROUP = 0 ;
                    string sVIT_GROUP = Stat.sTrayLabel_GroupingNo ;
                    if(!int.TryParse(sVIT_GROUP , out iVIT_GROUP)){
                        SendMsg("VIT_GROUP:" + sVIT_GROUP +" Pharsing Failed!");
                        return false ;
                    }            
                    sVIT_GROUP = iVIT_GROUP.ToString() ;
                    string sSC = Stat.sTrayLabel_SecurityCode ;
                    
                    string sHeader = "";
                    string sValue = "";

                    sHeader += "DATE\t"         ; sValue += _sDate         + "\t"  ;        
                    sHeader += "AVITTRK\t"      ; sValue += ""             + "\t"  ;
                    sHeader += "AVIT\t"         ; sValue += _sAVIT         + "\t"  ;
                    sHeader += "OPT\t"          ; sValue += _sOPT          + "\t"  ;
                    sHeader += "START_TIME\t"   ; sValue += _sSTART_TIME   + "\t"  ;
                    sHeader += "STOP_TIME\t"    ; sValue += _sSTOP_TIME    + "\t"  ;
                    sHeader += "BATCH_NO\t"     ; sValue += sBATCH_NO      + "\t"  ;
                    sHeader += "LOT_NO\t"       ; sValue += sLOT_NO        + "\t"  ;
                    sHeader += "DC\t"           ; sValue += sDC            + "\t"  ;
                    sHeader += "PROD_NO\t"      ; sValue += sPROD_NO       + "\t"  ;
                    sHeader += "GROUP\t"        ; sValue += sVIT_GROUP     + "\t"  ;
                    sHeader += "SC\t"           ; sValue += sSC            + "\t"  ;
                    sHeader += "QTY\t"          ; sValue += _sQty          + "\t"  ;
                    sHeader += "FAIL\t"         ; sValue += "0"            + "\t"  ;
                    sHeader += "PASS\t"         ; sValue += _sQty          + "\t"  ;
                    sHeader += "STATUS\r\n"     ; sValue += "PASS"         + "\r\n";
                    sw.WriteLine(sHeader + sValue);
                }        
            }
            catch (System.Exception e)
            {
                Log.ShowMessage("VIT File Write Error" , e.Message);
                return false ;
            }

            return true ;
        }

    }
}





/*



        //이밑은 테스트용.
        //============================================================================


        //랏카드에서 찍어서 랏오픈할때 이함수 써서 
        //레서피 네임 확인 해서 현재 레서피랑 같으면 OK
        //다르면 물어보고 다운로드.
        public bool GetRecipeName(string _sMeterialNr , ref string _sRecipeName )
        {
            string sQuery = "select RECIPE_NAME from TBL_VISION_RECIPE_AP where BIG_MATERIAL_NO='"+_sMeterialNr+"'";
            //DataTable Table = new DataTable();
            if (!Select(sQuery,ref Table)) {
                SendMsg(sQuery + " Select Query Failed!");
                return false;
            }
            if(Table.Rows.Count == 0){
                SendMsg(sQuery + " Select Query No Recipe!");
                return false;

            }
            if(Table.Rows.Count > 1){
                SendMsg(sQuery + " Recipe is more than 1ea!");
                return false;
            }

            _sRecipeName = Table.Rows[0]["RECIPE_NAME"].ToString();
            return true ;
        }

        public string GetGroupingNo(string _sTrayLabel)//0044
        {
            string sRet = _sTrayLabel.Substring(0,4);
            return sRet ;
        }

        public string GetMeterialNo(string _sTrayLabel)//11082612
        {
            string sRet = _sTrayLabel.Substring(4,8);
            return sRet ;
        }

        public string GetBatchNo(string _sTrayLabel)//1008642640
        {
            string sRet = _sTrayLabel.Substring(12,10);
            return sRet ;
        }

        public string GetSecurityCode(string _sTrayLabel)//009
        {
            string sRet = _sTrayLabel.Substring(22,3);
            return sRet ;
        }

        

        //Tray lebel에서 material 번호를 가지고 와와 TBL_TRAY_INFORMATION의  BIG_MATERIAL_NO에서  material number를 검색한다. 
        //Tray label에서 grouping number를 가져온 후, TBL_TRAY_INFORMATION 와 BIN 칼럼 아래의 material number를 검색한다. Grouping number은 BIN 칼럼에서 찾는다.
        //랏트레블러의 Grouping(Lot Alias)는 LOT_TRAV_GROUP에서 찾을 수 있어야 한다. 
        public bool CheckTrayLabel(string _sTrayLabel , string _sLotAlias)
        {
            string sMetNo ;
            string sQuery ;
            string sGroupingNo ;
            int    iGroupingNo ;           

            //메트리얼 , 빈 , LotAlias 같이 확인======================================
            sMetNo = GetMeterialNo(_sTrayLabel);
            sGroupingNo = GetGroupingNo(_sTrayLabel);
            if (!int.TryParse(sGroupingNo, out iGroupingNo))
            {
                SendMsg("GroupingNo:"+ sGroupingNo + " Pharsing Failed!");
                return false ;
            }
            sGroupingNo = iGroupingNo.ToString() ;
            sQuery = "select * from TBL_TRAY_INFORMATION where BIG_MATERIAL_NO='"+sMetNo+"' and BIN=" + sGroupingNo + " and LOT_TRAV_GROUP='"+_sLotAlias+"'" ;
            //sQuery = "select * from TBL_TRAY_INFORMATION where BIG_MATERIAL_NO='"+sMetNo+"'";
            //sQuery = "select * from TBL_TRAY_INFORMATION where BIG_MATERIAL_NO='"+sMetNo+"' and BIN=" + sGroupingNo ;
            if (!Select(sQuery,ref Table)) {
                SendMsg(sQuery + " Select Query Failed!");
                return false;
            }
            if(Table.Rows.Count == 0){
                SendMsg(sQuery + " Select Query No Data!");
                return false;
            }

            return true ;
        }

        //트레이 라벨 읽을때 한번씩 트레이 라벨에서 메트리얼 넘버를 넣어서.
        //리스트을 미리 만들어 놓음.
        //public DataTable PanelIDTable = new DataTable();
        private List<string> lsPanelID = new List<string> ();
        public bool MakeListPanelID(string _sTrayLabel , string _sLotAlias)
        {
            lsPanelID.Clear();

            string sMetNo ;
            string sQuery ;
            string sGroupingNo ;
            int    iGroupingNo ;           

            ////메트리얼 , 빈 , LotAlias 같이 확인======================================
            sMetNo = GetMeterialNo(_sTrayLabel);
            sGroupingNo = GetGroupingNo(_sTrayLabel);
            if (!int.TryParse(sGroupingNo, out iGroupingNo))
            {
                SendMsg("GroupingNo:"+ sGroupingNo + " Pharsing Failed!");
                return false ;
            }
            sGroupingNo = iGroupingNo.ToString() ;
            //TBL_TRAY_INFORMATION 테이블에서 BIG_MATERIAL_NO가 일치하는 SMALL_MATERIAL_NUMBER를 가져와서
            sQuery = "select distinct SMALL_MATERIAL_NUMBER from TBL_TRAY_INFORMATION where BIG_MATERIAL_NO='"+sMetNo+"' and BIN=" + sGroupingNo + " and LOT_TRAV_GROUP='"+_sLotAlias+"'" ;
            if (!Select(sQuery,ref Table)) {
                SendMsg(sQuery + " Select Query Failed!");
                return false;
            }
            if(Table.Rows.Count == 0){
                SendMsg(sQuery + " Select Query No Data!");
                return false;
            }

            DateTime CutOffDate = DateTime.Now.AddHours((OM.DevOptn.iDMC1MonthLimit * 30 * -24)); // max 2 years
            string sWhereT1 = "";
            for(int i = 0 ; i < Table.Rows.Count ; i++){
                sWhereT1 += "(T1_11_SERIES='"+Table.Rows[i]["SMALL_MATERIAL_NUMBER"]+"'";
                if(Table.Rows.Count-1 != i) sWhereT1+=" or ";
            }
            sWhereT1 +=")";

            string sDateCutoff = "";
            
            if(OM.DevOptn.iDMC1MonthLimit != 0) sDateCutoff = " AND trunc(PROBE_FILE_DATE_TIME) >= to_date('" + CutOffDate.ToString("yyyy-MM-dd") + "', 'YYYY-MM-DD')";


            sQuery = "select distinct PROBE_FILE from TBL_PROBE_FILE where " + sWhereT1 + sDateCutoff ;
            if (!Select(sQuery,ref Table)) {
                SendMsg(sQuery + " Select Query Failed!");
                return false;
            }
            if(Table.Rows.Count == 0){
                SendMsg(sQuery + " Select Query No Data!");
                return false;
            }

            string sItem ;
            string sItems = "" ; 
            for (int i = 0; i < Table.Rows.Count; i++)
            {
                sItem = Table.Rows[i]["PROBE_FILE"].ToString();
                sItems += sItem + " ";
                if (!lsPanelID.Contains(sItem))
                {
                    lsPanelID.Add(sItem);
                }
            }
            SendMsg(sItems);
            return true ;

        }

        //* Unit ID (Refer to Probe File)===========================================>(옵션기능)
        //자제의 UnitID를 읽고 미리 만들어 놓은 리스트에 자재가 부합하는지 확인한다        
        public bool CheckUnitID(string _sUnitID)
        {
            if(!OM.DevOptn.bUnitID) return true ;
            if(_sUnitID.Length < 8) return false ;

            string sProbeFile = _sUnitID.Substring(0,3) + _sUnitID.Substring(5,3);

            if (!lsPanelID.Contains(sProbeFile))
            {
                SendMsg(_sUnitID + " is Not Exist in PanelID List!");
                return false ;
            }
            return true ;
        }

        //2123580108은 DMC1을 decode한 후의 번호이며, 첫 번째 두 문자(21)를 가져온다. 
        //이 공급자코드(supplier code)는 TBL_TRAY_INFORMATION 에서 DMC_1열 아래 찾을 수 있어야 한다. 
        //공급자 코드는DMC_1에 기재되어 있어야 하며, 그렇지 못할 경우 에러메시지를 띄운다.
        //매 트레이마다 DMC1리스트를 만들고 자제별로 모두 비교 한다.
        private List<string> lsDMC1 = new List<string> ();
        public bool MakeListDMC1(string _sMeterialNr)
        {
            lsDMC1.Clear();
            //TBL_TRAY_INFORMATION 테이블에서 BIG_MATERIAL_NO가 일치하는 DMC_1
            string sQuery = "select distinct DMC_1 from TBL_TRAY_INFORMATION where BIG_MATERIAL_NO='"+_sMeterialNr+"'";
            if (!Select(sQuery,ref Table)) {
                SendMsg(sQuery + " Select Query Failed!");
                return false;
            }

            if (Table.Rows.Count == 0)
            {
                SendMsg(sQuery + " Select Query No Data!");
                return false;
            }

            char[] cComma={','};
            string[] sItems ;
            for (int i = 0; i < Table.Rows.Count; i++)
            {
                sItems = Table.Rows[i]["DMC_1"].ToString().Split(cComma);
                foreach (string sItem in sItems)
                {
                    if (!lsDMC1.Contains(sItem))
                    {
                        lsDMC1.Add(sItem);
                    }
                }
            }


            return true ;
        }

        //* DMC1 Grouping(E.g : 21 , 31 , etc)===========================================>(옵션기능)
        //2123580108은 DMC1을 decode한 후의 번호이며, 첫 번째 두 문자(21)를 가져온다. 
        //이 공급자코드(supplier code)는 TBL_TRAY_INFORMATION 에서 DMC_1열 아래 찾을 수 있어야 한다. 
        //공급자 코드는DMC_1에 기재되어 있어야 하며, 그렇지 못할 경우 에러메시지를 띄운다.
        public bool CheckDMC1(string _sDMC1)
        {
            if(!OM.DevOptn.bDMC1Grouping) return true ;
            if(_sDMC1.Length < 2) {
                SendMsg("DMC1 is Under 2 character!");
                return false ;
            }

            string sSupplyerCode = _sDMC1.Substring(0,2);
            if (!lsDMC1.Contains(sSupplyerCode))
            {
                SendMsg(_sDMC1 + " is Not Exist in DMC1 List!");
                return false ;
            }
            return true ;

        }

        //* Duplicate DMC1(Fresh Lot Only)===========================================>(옵션기능)
        //DMC 1은 TBL_UNIT_INSPECTION에서 중복되지 않으며, 데이터베이스에 중복될 경우 에러메시지를 띄운다.
        //return은 끝까지 수행 여부.
        public bool GetExistDMC1(string [] _sDMC1 , ref List<string> _lsExistDMC1)
        {
            if(!OM.DevOptn.bDuplicateDMC1) return true ;

            _lsExistDMC1.Clear();
            //TBL_TRAY_INFORMATION 테이블에서 BIG_MATERIAL_NO가 일치하는 DMC_1
            string sQuery = "select DMC1 from TBL_UNIT_INSPECTION_AP where ";
            for(int i = 0 ; i < _sDMC1.Length ; i++) {
                if(i!=0) sQuery += " or " ;
                sQuery += "DMC1='" +_sDMC1[i]+"'";
            }

            if (!Select(sQuery,ref Table)) {
                SendMsg(sQuery + " Select Query Failed!");
                return false;
            }

            for (int i = 0; i < Table.Rows.Count; i++)
            {
                _lsExistDMC1.Add(Table.Rows[i]["DMC1"].ToString());
            }

            //기존에 테이블에 데이터가 있으면 에러 띄워야함.
            return true ;
        }



        
//=================================
//                                                                                                       TrayInfomation SmallMaterial                            TrayLabel                                         TrayInfomation DeviceNo                               LotTraveller LotAlias
//GlobalVar.m_OracleCommand.CommandText = "select * from DANIEL.TBL_DMC2_VALUE WHERE T1_11_SERIES = '" + GlobalVar.m_strT111Series + "' AND  T2_11_SERIES = '" + GlobalVar.m_strT211Series + "' AND  DEVICE = '" + GlobalVar.m_strDeviceNumber + "' AND  GROUPING = '" + GlobalVar.m_strGroupBin + "'";
//GlobalVar.m_OracleCommand.CommandType = CommandType.Text;
//GlobalVar.m_OracleDataReader = GlobalVar.m_OracleCommand.ExecuteReader();
//


        //* DMC2 Character Compare ===========================================>(옵션기능)
        //DMC2 확인 방법 2가지 중에 첫번째 방식.
        //Grouping (05 7R ebxD68)를 가져오기 위해 DMC2를 decode한 후, 
        //TBL_TRAY_INFORMATION 테이블의 DEVICE_NUMBER 컬럼을 조회하여 비교한다.  
        //데이터베이스에  몇 개의 문자가 기재되어 있는지는 physical 자재와 반드시 동일해야 한다.
        //트레이 라벨 읽을때 확인 해놓는다.

        //  => 여기서 DMC2를 찾는 기준은 빅메트리얼넘버로 하면 되는지.?????????????????
        // 일단 Big으로함.
        private List<string> lsDeviceNumber = new List<string> ();
        public bool MakeListDeviceNumber(string _sMeterialNr , string _s)
        {
            lsDeviceNumber.Clear();
            //TBL_TRAY_INFORMATION 테이블에서 BIG_MATERIAL_NO가 일치하는 DEVICE_NUMBER
            //                                                               TrayInfomation SmallMaterial                            TrayLabel                                         TrayInfomation DeviceNo                               LotTraveller LotAlias
            //"select * from DANIEL.TBL_DMC2_VALUE WHERE T1_11_SERIES = '" + GlobalVar.m_strT111Series + "' AND  T2_11_SERIES = '" + GlobalVar.m_strT211Series + "' AND  DEVICE = '" + GlobalVar.m_strDeviceNumber + "' AND  GROUPING = '" + GlobalVar.m_strGroupBin + "'";
            //string sQuery = "select distinct DEVICE_NUMBER from TBL_TRAY_INFORMATION where BIG_MATERIAL_NO='"+_sMeterialNr+"'";

            string sQuery = "select * from TBL_DMC2_VALUE where T1_11_SERIES='"+_sMeterialNr+"'";
            if (!Select(sQuery,ref Table)) {
                SendMsg(sQuery + " Select Query Failed!");
                return false;
            }

            if (Table.Rows.Count == 0)
            {
                SendMsg(sQuery + " Select Query No Data!");
                return false;
            }

            string sItem ;
            for (int i = 0; i < Table.Rows.Count; i++)
            {
                sItem=Table.Rows[i]["DEVICE_NUMBER"].ToString();
                if (!lsDeviceNumber.Contains(sItem))
                {
                    lsDeviceNumber.Add(sItem);
                }
            }
            return true ;

        }

        public bool CheckDMC2CharacterCompare(string _sDMC2)
        {
            if(OM.DevOptn.iDMC2CheckMathod != 0) return true ;
            if(_sDMC2.Length < 12) {
                SendMsg("DMC2 is Under 12 character!");
                return false ;
            }
            string sDMC2 = _sDMC2.Substring(0,12);

         //                                                                                                       TrayInfomation SmallMaterial                            TrayLabel                                         TrayInfomation DeviceNo                               LotTraveller LotAlias
         //GlobalVar.m_OracleCommand.CommandText = "select * from DANIEL.TBL_DMC2_VALUE WHERE T1_11_SERIES = '" + GlobalVar.m_strT111Series + "' AND  T2_11_SERIES = '" + GlobalVar.m_strT211Series + "' AND  DEVICE = '" + GlobalVar.m_strDeviceNumber + "' AND  GROUPING = '" + GlobalVar.m_strGroupBin + "'";
            
            bool bRet = lsDeviceNumber.Contains(sDMC2) ;

            if (!lsDeviceNumber.Contains(sDMC2))
            {
                SendMsg(sDMC2 + " is Not Exist in DeviceNumber List!");
                return false ;
            }
            return true ;

        }

        //아마도 트레이마다 만들면 될듯.
        //T2_11_SERIES를 바탕으로 grouping한 시스템은 밝기(PMIN, PMAX), Ldom (LMIN, LMAX), Cx , Cy의 스펙을 검색할 수 있어야 한다.
        //몇 몇 의 자재는 brightness 와 Ldom만을 비교한다. 이 밖의 다른 자재는 brightness, Cx, Cy를 비교한다  이는 the INSPECTION_TYPE에 따라 추적가능하며, 
        int    iSpecINSPTYPE = 0 ;
        double dSpecPMIN =0;
        double dSpecPMAX =0;
        double dSpecLMIN =0;
        double dSpecLMAX =0;
        double dSpecCX1  =0;
        double dSpecCY1  =0;
        double dSpecCX2  =0;
        double dSpecCY2  =0;
        double dSpecCX3  =0;
        double dSpecCY3  =0;
        double dSpecCX4  =0;
        double dSpecCY4  =0;
        public bool MakeDMC2Spec(string _sMeterialNr)
        {
            //TBL_TRAY_INFORMATION 테이블에서 BIG_MATERIAL_NO가 일치하는 DEVICE_NUMBER
            string sQuery = "select distinct * from TBL_DMC2_VALUE where T2_11_SERIES='"+_sMeterialNr+"'";
            if (!Select(sQuery,ref Table)) {
                SendMsg(sQuery + " Select Query Failed!");
                return false;
            }

            if (Table.Rows.Count == 0)            {
                SendMsg(sQuery + " Select Query No Data!");
                return false;
            }

            bool bRet = true ;
            try{
                if(!int   .TryParse(Table.Rows[0]["INSPECTION_TYPE"].ToString(),out iSpecINSPTYPE)){bRet = false ; SendMsg("Parsing Filed! iSpecINSPTYPE");}
                if(!double.TryParse(Table.Rows[0]["PMIN"           ].ToString(),out dSpecPMIN    )){bRet = false ; SendMsg("Parsing Filed! dSpecPMIN"    );}
                if(!double.TryParse(Table.Rows[0]["PMAX"           ].ToString(),out dSpecPMAX    )){bRet = false ; SendMsg("Parsing Filed! dSpecPMAX"    );}
                if(!double.TryParse(Table.Rows[0]["LMIN"           ].ToString(),out dSpecLMIN    )){bRet = false ; SendMsg("Parsing Filed! dSpecLMIN"    );}
                if(!double.TryParse(Table.Rows[0]["LMAX"           ].ToString(),out dSpecLMAX    )){bRet = false ; SendMsg("Parsing Filed! dSpecLMAX"    );}
                if(!double.TryParse(Table.Rows[0]["CX1"            ].ToString(),out dSpecCX1     )){bRet = false ; SendMsg("Parsing Filed! dSpecCX1 "    );}
                if(!double.TryParse(Table.Rows[0]["CY1"            ].ToString(),out dSpecCY1     )){bRet = false ; SendMsg("Parsing Filed! dSpecCY1 "    );}
                if(!double.TryParse(Table.Rows[0]["CX2"            ].ToString(),out dSpecCX2     )){bRet = false ; SendMsg("Parsing Filed! dSpecCX2 "    );}
                if(!double.TryParse(Table.Rows[0]["CY2"            ].ToString(),out dSpecCY2     )){bRet = false ; SendMsg("Parsing Filed! dSpecCY2 "    );}
                if(!double.TryParse(Table.Rows[0]["CX3"            ].ToString(),out dSpecCX3     )){bRet = false ; SendMsg("Parsing Filed! dSpecCX3 "    );}
                if(!double.TryParse(Table.Rows[0]["CY3"            ].ToString(),out dSpecCY3     )){bRet = false ; SendMsg("Parsing Filed! dSpecCY3 "    );}
                if(!double.TryParse(Table.Rows[0]["CX4"            ].ToString(),out dSpecCX4     )){bRet = false ; SendMsg("Parsing Filed! dSpecCX4 "    );}
                if(!double.TryParse(Table.Rows[0]["CY4"            ].ToString(),out dSpecCY4     )){bRet = false ; SendMsg("Parsing Filed! dSpecCY4 "    );}          
            }
            catch(Exception ex){
                SendMsg("Spec Parsing Failed : " + ex.Message);
                bRet = false ;
            }

            return bRet ;         
        }
        

        



        public bool CheckDMC2_Spec(string _sDMC2)
        {
            if(OM.DevOptn.iDMC2CheckMathod != 1) return true ;
            char [] cSeperator = {' '};
            string[] sItems = _sDMC2.Split(cSeperator);

            if(sItems.Count() != 5) {
                SendMsg("DMC2 is Under 5 Words!");
                return false ;
            }
            

            double dBrightness = 0 ;
            double dLdom       = 0 ;
            double dCX         = 0 ;
            double dCY         = 0 ;

            if(!double.TryParse(sItems[0] , out dBrightness)) return false ;
            if(!double.TryParse(sItems[2] , out dLdom      )) return false ;
            if(!double.TryParse(sItems[3] , out dCX        )) return false ;
            if(!double.TryParse(sItems[4] , out dCY        )) return false ;

            if(OM.DevOptn.bBrightnessCheck) {
                if(dBrightness < this.dSpecPMIN || dBrightness > this.dSpecPMAX) {
                    SendMsg("Mix Bin - PMIN:"+this.dSpecPMIN + " PMAX:"+this.dSpecPMAX + " Brightness=" + dBrightness );
                    return false ;
                }
            }
            //1로 설정된경우는 brightness와 Ldom만을 비교.  
            //설정이 2 인 경우 brightness, Cx 및 Cy를 비교하는 것이다. 
            //Brightness 판독은 PMIN 및 PMAX 범위 내에 있어야 하며,  Out of the spec시 "Mix Bin"과 함께 오류 메시지를 띄운다.
            if (iSpecINSPTYPE == 1) { 
                if(OM.DevOptn.bLDOMCheck) {
                    if(dLdom < this.dSpecLMIN || dLdom > this.dSpecLMAX) {
                        //아웃오브스펙.
                        SendMsg("Mix Bin - LMIN:"+this.dSpecLMIN + " LMAX:"+this.dSpecLMAX + " Ldom=" + dLdom );
                        return false ;
                    }
                }
                //Ldom판독은 LMIN and LMAX범위 내에 있어야 하며, Out of the spec시 “Mix Bin”과 함께 오류메시지를 띄운다.
            }
            else if(iSpecINSPTYPE == 2){
                if(OM.DevOptn.bCxCy) {
                    List<PointD> lsPoly = new List<PointD>();
                    
                    lsPoly.Add(new PointD(this.dSpecCX1 , this.dSpecCY1));
                    lsPoly.Add(new PointD(this.dSpecCX2 , this.dSpecCY2));
                    lsPoly.Add(new PointD(this.dSpecCX3 , this.dSpecCY3));
                    lsPoly.Add(new PointD(this.dSpecCX4 , this.dSpecCY4));
                    lsPoly.Add(new PointD(this.dSpecCX1 , this.dSpecCY1));
                    if(!IsPointInPolygon(lsPoly , new PointD(dCX , dCY))) {
                        SendMsg("Mix Bin - Cx:"+dCX + " Cy:"+dCY );
                        return false ;
                    }
                }
                //Cx 및 Cy  스펙은 사각형으로 구성되어야 하며 DMC 2의 값은 사각형 내에 있어야 한다. 
                //샘플은 다음과 같다. 요점은  사각형 밖에 있을 시, 이는 자재가 섞여 있음을  뜻하고,  오류메시지를 띄워야 한다.
            }
            
            return true ;
        }


        //Data1
        //RESULT            => Unit Locator:1##Unit ID:1##DMC 1:1##DMC 2:1##Glob Top Epoxy (LEFT):1##Glob Top Epoxy (RIGHT):1##Unit Pattern (LEFT):1##
        //                     Unit Pattern (RIGHT):1##Glob Top (LEFT):1##Glob Top (RIGHT):1##Glob Top (TOP):1##
        //NG_REMARK         => DMC 1 Grouping Reject!

        //Data2
        //RESULT            => Unit Locator:1##Unit ID:1##DMC 1:1##DMC 2:1##Glob Top Epoxy (LEFT):1##Glob Top Epoxy (RIGHT):1##Unit Pattern (LEFT):1##
        //                     Unit Pattern (RIGHT):1##Glob Top (LEFT):1##Glob Top (RIGHT):1##Glob Top (TOP):1##
        //NG_REMARK         => Reject!

        //Data3
        //RESULT            => Unit Locator:1##Unit ID:1##DMC 1:1##DMC 2:1##Glob Top Epoxy (LEFT):1##Glob Top Epoxy (RIGHT):1##Unit Pattern (LEFT):1##
        //                     Unit Pattern (RIGHT):0##Glob Top (LEFT):1##Glob Top (RIGHT):1##Glob Top (TOP):1##
        //NG_REMARK         => Reject!


        //UNIT_INSPECTION_ID=> HZ63700A99_001_160926_212440P_29
        //UNIT_ID           => 3KFC7C1S
        //DMC1              => 2122654747
        //DMC2              => 05 7R ebxD68 158 01
        //DATE_TIME         => 26-SEP-16 09.23.17.790351000 PM
        //RESULT            => Unit Locator:1##Unit ID:1##DMC 1:1##DMC 2:1##Glob Top Epoxy (LEFT):1##Glob Top Epoxy (RIGHT):1##Unit Pattern (LEFT):1##
        //                     Unit Pattern (RIGHT):1##Glob Top (LEFT):1##Glob Top (RIGHT):1##Glob Top (TOP):1##
        //VIT_NO            => HZ63700A99_001_160926_212440P
        //TRAY_LABEL        => 0044110826121008274930001
        //LOT_NO            => HZ63700A99
        //AVITTRRK_AVIT     => AOSF03
        //T2_11_SERIES      => 11082612
        //OLD_LOT_NO        => 
        //NG_REMARK         => DMC 1 Grouping Reject!




        //string sUnitInspectionQuery = "";
        

        
        

        

 */