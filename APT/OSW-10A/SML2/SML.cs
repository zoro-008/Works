using COMMON;
using System.Windows.Forms;


namespace SML2
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

    public class SML
    {
        static public CErrMan       ER;
        static public CDioMan       IO;
        static public CCylinderMan  CL;
        static public CMotrMan      MT;
        static public CTowerLampMan TL;
        
        //폼쪽은 억세스 못한다.
        public static FormLogOn FrmLogOn;
        protected static FormDllMain FrmDllMain ;//= new FormDllMain();
        

        public struct TPara
        {
            public string        sParaFolderPath;
            public int           iWidth ;
            public int           iHeight;
            public bool[]        bTabHides;
            public bool          bUseErrPic;
            public int           iCntErr;
            public int           iCntDIn;
            public int           iCntDOut;
            public int           iCntCylinder;
            public int           iCntMotr;
            public EN_LAN_SEL    eLanSel;
            public EN_DIO_SEL    eDio   ;
            public EN_MOTR_SEL[] eMotors;
            
        }

        public static void Init(TPara _tPara)
        {

            ER = new CErrMan();
            ER.Init(_tPara.eLanSel         , 
                    _tPara.sParaFolderPath , 
                    _tPara.iCntErr         ,
                    _tPara.bUseErrPic      );


            IO = new CDioMan();
            IO.Init(_tPara.eLanSel         ,
                    _tPara.sParaFolderPath ,
                    _tPara.iCntDIn         ,
                    _tPara.iCntDOut        ,
                    _tPara.eDio           );

            CL = new CCylinderMan();
            CL.Init(_tPara.eLanSel         , 
                    _tPara.sParaFolderPath , 
                    _tPara.iCntCylinder    ,
                    IO                     );

            MT = new CMotrMan();
            MT.Init(_tPara.eLanSel         , 
                    _tPara.sParaFolderPath , 
                    _tPara.iCntMotr        ,
                    _tPara.eMotors         ,
                    IO                    );

            TL = new CTowerLampMan();
            TL.Init(_tPara.sParaFolderPath , 
                    IO                 );

            FrmLogOn = new FormLogOn();
            FrmDllMain = new FormDllMain(_tPara.iWidth , _tPara.iHeight , _tPara.bTabHides);
            

            Log.StartLogMan();

        }

        public static void Close()
        {
           

            ER.Close();
            IO.Close();
            CL.Close();
            MT.Close();
            TL.Close();

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
            CL.Reset();
            MT.ResetAll();
        }


        public static void Update(EN_SEQ_STAT _eStat)
        {
            IO.Update();
            ER.Update();
            CL.Update();
            TL.Update(_eStat);
            MT.Update();
        }
    }

}
