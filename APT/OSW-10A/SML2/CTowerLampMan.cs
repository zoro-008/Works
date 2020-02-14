using COMMON;

namespace SML2
{
    public class CTowerLampMan
    {
        public CTowerLampMan() { }

        public enum EN_LAMP_OPER : uint
        {
            //loLampOff   = 0 ,
            //loLampOn        ,
            //loLampFlick     ,
            
            ON = 0,
            OFF,
            FLICK,
        
            MAX_LAMP_OPER
        };

        public struct TLampInfo 
        {
            public EN_LAMP_OPER  iRed    ;
            public EN_LAMP_OPER  iYel    ;
            public EN_LAMP_OPER  iGrn    ;
            public EN_LAMP_OPER  iBuzz   ;
        };

        public struct TStat
        {
            public bool bRedOn;
            public bool bYelOn;
            public bool bGrnOn;
            public bool bBuzOn;
        }
        
        public struct TPara 
        {
            public int  iRedAdd ;
            public int  iYelAdd ;
            public int  iGrnAdd ;
            public int  iSndAdd ;
            public TLampInfo[] LampInfo;
        };

        public TPara  Para;
        TStat         Stat;

        //TPara[] m_aAdd = null;
        

        string        m_sParaFolderPath;
        bool          m_bFlick;

        bool          m_bTestMode;
        bool          m_bTestLampBuss;
        EN_SEQ_STAT   m_iSeqStat;
        EN_SEQ_STAT   m_eTestStat;

        CDioMan       DIO;

        CDelayTimer   FlickTimer = new CDelayTimer();

        //public int _ListViewCnt = 8;

        bool          m_bBuzzOff;
 
        public bool Init(string _sParaFolderPath,CDioMan _Dio)
        {

            m_sParaFolderPath = _sParaFolderPath;
            DIO = _Dio;

            Para.LampInfo = new TLampInfo[(int)EN_SEQ_STAT.MAX_SEQ_STAT];

            //m_aAdd = new TPara[(int)EN_SEQ_STAT.MAX_SEQ_STAT];

            LoadSave(true);

            return true;

            
        }

        public bool Close()
        {
            return true;
        }

        public void Update(EN_SEQ_STAT _eStat)
        {
            //m_iSeqStat = _eStat;

            //Local Var.
            EN_SEQ_STAT eStat = m_bTestLampBuss ? m_eTestStat : _eStat;
            int         iStat = (int)eStat;

            if(FlickTimer.OnDelay(500))
            {
                m_bFlick = !m_bFlick;
                FlickTimer.Clear();
            }

            if (DIO.GetTestMode()) return;



            
            //switch(Para.LampInfo[iStat].iRed)
            //{
            //    case EN_LAMP_OPER.loLampOff  : DIO.SetY(Para.iRedAdd,false   ); break;
            //    case EN_LAMP_OPER.loLampOn   : DIO.SetY(Para.iRedAdd,true    ); break;
            //    case EN_LAMP_OPER.loLampFlick: DIO.SetY(Para.iRedAdd,m_bFlick); break;
            //}
            //
            //switch(Para.LampInfo[iStat].iGrn)
            //{
            //    case EN_LAMP_OPER.loLampOff  : DIO.SetY(Para.iGrnAdd,false   ); break;
            //    case EN_LAMP_OPER.loLampOn   : DIO.SetY(Para.iGrnAdd,true    ); break;
            //    case EN_LAMP_OPER.loLampFlick: DIO.SetY(Para.iGrnAdd,m_bFlick); break;
            //}
            //
            //switch(Para.LampInfo[iStat].iYel)
            //{
            //    case EN_LAMP_OPER.loLampOff  : DIO.SetY(Para.iRedAdd,false   ); break;
            //    case EN_LAMP_OPER.loLampOn   : DIO.SetY(Para.iRedAdd,true    ); break;
            //    case EN_LAMP_OPER.loLampFlick: DIO.SetY(Para.iRedAdd,m_bFlick); break;
            //}
            //
            //switch(Para.LampInfo[iStat].iBuzz)
            //{
            //    case EN_LAMP_OPER.loLampOff  : DIO.SetY(Para.iSndAdd,false   ); break;
            //    case EN_LAMP_OPER.loLampOn   : DIO.SetY(Para.iSndAdd,true    ); break;
            //    case EN_LAMP_OPER.loLampFlick: DIO.SetY(Para.iSndAdd,m_bFlick); break;
            //}

            switch (Para.LampInfo[iStat].iRed)
            {
                case EN_LAMP_OPER.OFF  : if(Para.iRedAdd != -1) {DIO.SetY(Para.iRedAdd, false   );} break;
                case EN_LAMP_OPER.ON   : if(Para.iRedAdd != -1) {DIO.SetY(Para.iRedAdd, true    );} break;
                case EN_LAMP_OPER.FLICK: if(Para.iRedAdd != -1) {DIO.SetY(Para.iRedAdd, m_bFlick);} break;
            }

            switch (Para.LampInfo[iStat].iGrn)
            {
                case EN_LAMP_OPER.OFF  : if(Para.iGrnAdd != -1) {DIO.SetY(Para.iGrnAdd, false   );} break;
                case EN_LAMP_OPER.ON   : if(Para.iGrnAdd != -1) {DIO.SetY(Para.iGrnAdd, true    );} break;
                case EN_LAMP_OPER.FLICK: if(Para.iGrnAdd != -1) {DIO.SetY(Para.iGrnAdd, m_bFlick);} break;
            }                                                                                     
                                                                                                  
            switch (Para.LampInfo[iStat].iYel)                                                    
            {                                                                                     
                case EN_LAMP_OPER.OFF  : if(Para.iYelAdd != -1) {DIO.SetY(Para.iYelAdd, false   );} break;
                case EN_LAMP_OPER.ON   : if(Para.iYelAdd != -1) {DIO.SetY(Para.iYelAdd, true    );} break;
                case EN_LAMP_OPER.FLICK: if(Para.iYelAdd != -1) {DIO.SetY(Para.iYelAdd, m_bFlick);} break;
            }                                                                                     
                                                                                                  
            if(!m_bBuzzOff){                                               
                switch (Para.LampInfo[iStat].iBuzz)                                                   
                {                                                                                     
                    case EN_LAMP_OPER.OFF  : if(Para.iSndAdd != -1) {DIO.SetY(Para.iSndAdd, false   );} break;
                    case EN_LAMP_OPER.ON   : if(Para.iSndAdd != -1) {DIO.SetY(Para.iSndAdd, true    );} break;
                    case EN_LAMP_OPER.FLICK: if(Para.iSndAdd != -1) {DIO.SetY(Para.iSndAdd, m_bFlick);} break;
                }
            }
            else {
                DIO.SetY( Para.iSndAdd, false   );
            }

        }

        public TLampInfo GetTLampInfo(EN_SEQ_STAT _eStat)
        {
            return Para.LampInfo[(int)_eStat];
        }

        public TPara GetTLampPara()
        {
            //return m_aAdd[(int)_eStat];
            return Para;

        }

        public void SetTLampInfo(EN_SEQ_STAT _eStat, TLampInfo _tLampInfo)
        {
            Para.LampInfo[(int)_eStat] = _tLampInfo;
        }

        public void SetTLampPara(TPara _tLampPara)
        {
            //m_aAdd[(int)_eStat] = _tLampPara;
            Para.iRedAdd = _tLampPara.iRedAdd;
            Para.iYelAdd = _tLampPara.iYelAdd;
            Para.iGrnAdd = _tLampPara.iGrnAdd;
            Para.iSndAdd = _tLampPara.iSndAdd;

        }

        public void SetTestMode (bool IsTest) 
        { 
            m_bTestMode = IsTest; 
        }

        public bool LoadSave(bool _bLoad)
        {
           
            


            //if(_bLoad)
            //{
            //    CConfig Config = new CConfig();
            //    Config.Load(m_sParaFolderPath + "TowerLamp.ini",CConfig.EN_CONFIG_FILE_TYPE.ftIni);
            //
            //    sIndex = "Para";
            //    Config.GetValue("iRedAdd",sIndex,out Para.iRedAdd);
            //    Config.GetValue("iYelAdd",sIndex,out Para.iYelAdd);
            //    Config.GetValue("iGrnAdd",sIndex,out Para.iGrnAdd);
            //    Config.GetValue("iSndAdd",sIndex,out Para.iSndAdd);
            //
            //    int iTemp;
            //    sIndex = "LampInfo";
            //    for(int i = 0 ; i < (int)EN_SEQ_STAT.MAX_SEQ_STAT ; i++)
            //    {
            //        Config.GetValue("iRed ",sIndex,out iTemp);Para.LampInfo[i].iRed = (EN_LAMP_OPER)iTemp ;
            //        Config.GetValue("iYel ",sIndex,out iTemp);Para.LampInfo[i].iYel = (EN_LAMP_OPER)iTemp ;
            //        Config.GetValue("iGrn ",sIndex,out iTemp);Para.LampInfo[i].iGrn = (EN_LAMP_OPER)iTemp ;
            //        Config.GetValue("iBuzz",sIndex,out iTemp);Para.LampInfo[i].iBuzz= (EN_LAMP_OPER)iTemp ;
            //    }
            //}
            //else
            //{
            //    CConfig Config = new CConfig();
            //
            //    sIndex = "Para";
            //    Config.SetValue("iRedAdd",sIndex, Para.iRedAdd);
            //    Config.SetValue("iYelAdd",sIndex, Para.iYelAdd);
            //    Config.SetValue("iGrnAdd",sIndex, Para.iGrnAdd);
            //    Config.SetValue("iSndAdd",sIndex, Para.iSndAdd);
            //
            //    sIndex = "LampInfo";
            //    for(int i = 0;i < (int)EN_SEQ_STAT.MAX_SEQ_STAT;i++)
            //    {
            //        Config.SetValue("iRed ",sIndex,(int)Para.LampInfo[i].iRed);
            //        Config.SetValue("iYel ",sIndex,(int)Para.LampInfo[i].iYel);
            //        Config.SetValue("iGrn ",sIndex,(int)Para.LampInfo[i].iGrn);
            //        Config.SetValue("iBuzz",sIndex,(int)Para.LampInfo[i].iBuzz); 
            //    }
            //    Config.Save(m_sParaFolderPath + "TowerLamp.ini",CConfig.EN_CONFIG_FILE_TYPE.ftIni);
            //}
            string sIndex;
            CConfig Config = new CConfig();

           




            if (_bLoad)
            {
                CIniFile TLampConfig = new CIniFile(m_sParaFolderPath + "TowerLamp.ini");


                int iTemp;
                
                for (int i = 0; i < (int)EN_SEQ_STAT.MAX_SEQ_STAT; i++)
                {
                    sIndex = string.Format("TOWER_LAMP({0:0000})", i);
                    
                    TLampConfig.Load("iRed ", sIndex, out iTemp); Para.LampInfo[i].iRed  = (EN_LAMP_OPER)iTemp;
                    TLampConfig.Load("iYel ", sIndex, out iTemp); Para.LampInfo[i].iYel  = (EN_LAMP_OPER)iTemp;
                    TLampConfig.Load("iGrn ", sIndex, out iTemp); Para.LampInfo[i].iGrn  = (EN_LAMP_OPER)iTemp;
                    TLampConfig.Load("iBuzz", sIndex, out iTemp); Para.LampInfo[i].iBuzz = (EN_LAMP_OPER)iTemp;
                }

                sIndex = "m_aAdd";
                TLampConfig.Load(sIndex, "iRedAdd", out Para.iRedAdd);
                TLampConfig.Load(sIndex, "iYelAdd", out Para.iYelAdd);
                TLampConfig.Load(sIndex, "iGrnAdd", out Para.iGrnAdd);
                TLampConfig.Load(sIndex, "iSndAdd", out Para.iSndAdd);

            }
            else
            {
                CIniFile TLampConfig = new CIniFile(m_sParaFolderPath + "TowerLamp.ini");

                for (int i = 0; i < (int)EN_SEQ_STAT.MAX_SEQ_STAT; i++)
                {
                    sIndex = string.Format("TOWER_LAMP({0:0000})", i);
                    TLampConfig.Save("iRed ", sIndex, (int)Para.LampInfo[i].iRed);
                    TLampConfig.Save("iYel ", sIndex, (int)Para.LampInfo[i].iYel);
                    TLampConfig.Save("iGrn ", sIndex, (int)Para.LampInfo[i].iGrn);
                    TLampConfig.Save("iBuzz", sIndex, (int)Para.LampInfo[i].iBuzz);
                }

                sIndex = "m_aAdd";
                TLampConfig.Save(sIndex, "iRedAdd", Para.iRedAdd);
                TLampConfig.Save(sIndex, "iYelAdd", Para.iYelAdd);
                TLampConfig.Save(sIndex, "iGrnAdd", Para.iGrnAdd);
                TLampConfig.Save(sIndex, "iSndAdd", Para.iSndAdd);
            }
            return true;
        }

        public void SetBuzzOff(bool _bValue)
        {
            m_bBuzzOff = _bValue ;
        }
    }
}
