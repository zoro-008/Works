using System.Windows.Forms;
using COMMON;
using System;

namespace SML
{
    
    /*public sealed class SM
    {
        //single tone 패턴.
        private static volatile SM inst ;
        private static object sync = new Object();
        private SM() {}
        public static SM Inst
        {
            get
            {
                if (inst == null)
                {
                    lock(sync)//멀티쓰레이드 일경우.
                    {
                        if (inst == null)
                        {
                            inst = new SM();
                        }
                    }
                }
                return inst;
            }
        }



        public static CErrors EM = CErrors.Inst; //이름 짧게 하려고 한번 치환.
        public void Update()
        {
            EM.Update();

        }


    }*/

    public class SM
    {
        static public CErrMan       ERR;
        static public CDioMan       DIO;
        static public CAioMan       AIO;
        static public CCylinderMan  CYL;
        static public CMotrMan      MTR;
        static public CTowerLampMan TWL;
        
        //폼쪽은 억세스 못한다.
        public static FormLogOn FrmLogOn;
        protected static FormDllMain FrmDllMain ;//= new FormDllMain();
        

        public struct TPara
        {
            public EN_LAN_SEL    eLanSel     ;
            public string        sParaFolderPath;
            public int           iWidth ;
            public int           iHeight;
            public bool[]        bTabHides;
            public TParaErr      Err ;
            public TParaDio      Dio ;
            public TParaAio      Aio ;
            public TParaMtr      Mtr ;
            public TParaCyl      Cyl ;
        }

        public struct TParaErr
        {
            public Enum          eErr        ;
            public bool          bUseErrPic;
        }

        public struct TParaDio
        {
            public EN_DIO_SEL    eDioSel     ;
            public Enum          eX          ;
            public Enum          eY          ;
        }

        public struct TParaAio
        {
            public EN_AIO_SEL    eAioSel     ;
            public Enum          eX          ;
            public Enum          eY          ;
            public int           iRangeAMin  ;
            public int           iRangeAMax  ;
        }

        public struct TParaMtr
        {
            public EN_MTR_SEL[]  eMtrSel     ;
            public Enum          eMtr        ;
        }

        public struct TParaCyl
        {
            public Enum          eCyl        ;
        }

        public static void Init(TPara _tPara)
        {
            ERR = new CErrMan();
            ERR.Init(_tPara.eLanSel             , 
                     _tPara.sParaFolderPath     , 
                     _tPara.Err.eErr            ,
                     _tPara.Err.bUseErrPic      );

            DIO = new CDioMan();
            DIO.Init(_tPara.eLanSel             ,
                      _tPara.sParaFolderPath,
                     _tPara.Dio.eDioSel         ,
                     _tPara.Dio.eX              ,
                     _tPara.Dio.eY              );

            AIO = new CAioMan();
            AIO.Init(_tPara.eLanSel             ,
                     _tPara.sParaFolderPath     ,
                     _tPara.Aio.eAioSel         ,
                     _tPara.Aio.eX              ,
                     _tPara.Aio.eY              ,
                     _tPara.Aio.iRangeAMin      ,
                     _tPara.Aio.iRangeAMax      );

            CYL = new CCylinderMan();
            CYL.Init(_tPara.eLanSel             , 
                     _tPara.sParaFolderPath     , 
                     _tPara.Cyl.eCyl            , 
                     DIO                        );

            MTR = new CMotrMan();
            MTR.Init(_tPara.eLanSel             , 
                     _tPara.sParaFolderPath     , 
                     _tPara.Mtr.eMtrSel         ,
                     _tPara.Mtr.eMtr            , 
                     DIO                        );
                                                
            TWL = new CTowerLampMan();          
            TWL.Init(_tPara.eLanSel             , 
                     _tPara.sParaFolderPath     , 
                     DIO                        ,!_tPara.bTabHides[3]);

            FrmLogOn   = new FormLogOn();
            FrmDllMain = new FormDllMain(_tPara.iWidth , _tPara.iHeight , _tPara.eLanSel , _tPara.bTabHides);

            Log.StartLogMan();
        }


        public static void Close()
        {
           

            ERR.Close();
            DIO.Close();
            AIO.Close();
            CYL.Close();
            MTR.Close();
            TWL.Close();

            Log.EndLogMan();      

        }



        public static void SetDllMainWin(ref Panel _ctBase) 
        {
            if (_ctBase != null)
            {
                FrmDllMain.TopLevel = false;
                FrmDllMain.FormBorderStyle = FormBorderStyle.None;
                _ctBase.Controls.Add(FrmDllMain);
            }
            FrmDllMain.Visible = true;
            //FrmDllMain.Show();
            
        }
        public static void HideDllMainWin()
        {
            FrmDllMain.Visible = false;
            //FrmDllMain.Hide();

        }

        public static void Reset()
        {
            CYL.Reset();
            MTR.ResetAll();
        }


        public static void Update(EN_SEQ_STAT _eStat)
        {
            DIO.Update();
            AIO.Update();
            ERR.Update();
            CYL.Update();
            TWL.Update(_eStat);
            MTR.Update();
        }
    }

}
