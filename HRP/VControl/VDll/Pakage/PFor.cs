using COMMON;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace VDll.Pakage
{
    class PFor : CPkgObject , IPkg
    {
        readonly TProp Prop = new TProp{bImgGrabber = false ,
                                        bHaveDialog = false ,
                                        bHaveImage  = false ,
                                        bHavePosRef = false };

        public class CMstPara 
        {                         
            [CategoryAttribute("MPara"), DisplayNameAttribute("PkgName of Ok Result")] public string   PkgNameOfOk       {get;set;} public IPkg PkgOk    ; 
            [CategoryAttribute("MPara"), DisplayNameAttribute("PkgName of Ng Result")] public string   PkgNameOfNg       {get;set;} public IPkg PkgNg    ; 
            [CategoryAttribute("MPara"), DisplayNameAttribute("Process Count"       )] public int      ProcessCount      {get;set;} //토탈 이 PKG를 몇번을 수행 하는 건지에 대한 갯수.
        }        

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class CUsrPara 
        {
            [CategoryAttribute("UPara"), DisplayNameAttribute("Threshold"           )] public uint         Threshold         {get;set;}        
        }

        private CMultiObjectSelector.CustomObjectType oRet;

        //저장 값들....
        CMstPara MstPara = new CMstPara();
        CUsrPara UsrPara = new CUsrPara(); 
        Object   AlgPara ;                 

        //====================================
        List<CTracker> lsTracker = new List<CTracker>();
        TPos Pos = new TPos{dRefX   = 0.0 , //카메라는 기준포지션 없음.
                            dRefY   = 0.0 ,
                            dRefT   = 0.0 ,
                            dInspX  = 0.0 ,
                            dInspY  = 0.0 ,
                            dInspT  = 0.0 };

        int iProcessCnt = 0 ;
        class TRslt
        {
            //public List<PointF> Peaks = new List<PointF>();
            public PointF[]     Peaks = new PointF[1];
            public int          WorkTimems ;
        }
        TRslt Rslt = new TRslt();

        public PFor(string _sName , int _iVisionID):base(_sName , _iVisionID)
        {
            //현재 세팅 패키지와 가장 가까운 이미지를 가지고 있는 PKG로 초기화 세팅.
            foreach(IPkg Pkg in GV.Visions[iVisionID].Pkgs)
            {
                TProp Prop =  Pkg.GetProp() ;
            }
        }

        #region IPkg
        public bool Init()
        {
            //CTracker Tracker = new CTracker();
            //Tracker.Init();
            //
            //
            //lsTracker.Add(Tracker);


            //나중에 알고리즘 바꿀때는 AlgPara = Activator.CreateInstance( /*여기 생각좀 하자...*/); //sun
            //AlgPara = Activator.CreateInstance(typeof(Algorithm.CPeak.CUPara));

            return true ;
        }

        public bool Close()
        {
            return true ;
        }

        //외부에서 보는 현재 패키지 특성.
        public TProp GetProp()
        {            
            return Prop ;
        }

        public object MPara 
        {
            get {return MstPara;} 
            set {MstPara = value as CMstPara;}
        }
        public object UPara 
        {
            get {
                //sun 나중에 여기 널일때 문제 없는지 확인.
                //타입이 다를때 기존 메모리 CreateInstance로 생성 된것 관리가 되는지 확인.
                //타입 바꾸는 것이 능사인지 모르겠음.

                if(oRet == null)
                {
                    List<CCustomProperty> lsPara = new List<CCustomProperty>();
                    CMultiObjectSelector.GetData(ref AlgPara, ref lsPara , "Peak" );
                    CMultiObjectSelector.GetData(ref UsrPara, ref lsPara , "User" );

                    oRet = new CMultiObjectSelector.CustomObjectType { props = lsPara };
                }
                return oRet;
            }

            set {
                CMultiObjectSelector.SetProperty(ref AlgPara, value , "Peak");
                CMultiObjectSelector.SetProperty(ref UsrPara, value , "User");
            }
        }

        public string GetName ()  {return sName;  }
        public string GetError()  {return sError; }

        //트레커 외부에서 클릭 처리 해야함.
        public List<CTracker> Trackers {get { return lsTracker ;}}
        
        public Mat GetImage ()
        {
            return null;
        }

        public Mat GetTrainFormImage ()
        {
            return GetImage ();
        }

        public Mat GetTrainImage ()
        {
            return null ;
        }

        public TPos GetPos()
        {
            return Pos ;
        }

        public bool Train()
        {
            return true ;       
        }

      

        public bool Run(TRunPara _tPara , ref TRunRet _tRet)
        {
            _tRet.bEnded = false ;
            _tRet.NextPkg = this ;
            sError = "";

            Stopwatch WorkTime = new Stopwatch() ;
            //WorkTime.Reset();
            WorkTime.Restart();


            WorkTime.Stop();

            Rslt.WorkTimems = (int)WorkTime.ElapsedMilliseconds ;


            //OK NG에 따라 분기문....
            //PkgOk PkgNG
            //_tRet.NextPkg = PkgOk ;
            _tRet.bEnded  = true ;
            _tRet.NextPkg = null ;

            iProcessCnt++;

            if(MstPara.ProcessCount > iProcessCnt)
            {
                _tRet.NextPkg = MstPara.PkgNg ;
            }
            else 
            {
                _tRet.NextPkg = MstPara.PkgOk ;
            }

            return true ;
        }

        public bool Paint(Graphics _g , bool _bTrain)// , TScaleOffset _ScaleOffset)//매개변수 화면 핸들 혹은 ImageBox포인터.
        {
            return true ;
        }

        //public bool PaintTrain (Graphics _g)
        //{
        //    if(!Paint(_g)) return false ;  
        //    return true;
        //}

        public TScaleOffset ScaleOffset
        {
            get {return tScaleOffset ;} 
            set {tScaleOffset = value;}
        }

        public bool ShowDialog() //bHaveDialog 가 on인 페키지만 Show 해줌.
        {
            return false ;
        }

        //오토런 전환시 수행할 것들함.
        public bool Autorun(bool _bAutorun)
        {
            iProcessCnt = 0;
            return true;
        }

        //오토런중에 뭔가 미리 준비 해야 될 것들 있으면 한다.
        public bool Ready()
        {
            iProcessCnt = 0;
            return true;
        }

        public bool MouseDown(MouseEventArgs _e)
        {
            foreach( CTracker Tracker in lsTracker)
            {
                if(Tracker.MouseDown(Control.ModifierKeys , _e , tScaleOffset))
                {
                    return true ;
                }
            }
            return false ;
        }
        public bool MouseMove(MouseEventArgs _e)
        {
            foreach( CTracker Tracker in lsTracker)
            {
                if(Tracker.MouseMove(Control.ModifierKeys , _e , tScaleOffset))
                {
                    return true ;
                }
            }
            
            return false ;
        }
        public bool MouseUp(MouseEventArgs _e)
        {
            foreach( CTracker Tracker in lsTracker)
            {
                if(Tracker.MouseUp(Control.ModifierKeys , _e))
                {
                    return true ;
                }
            }
            return false ;
        }

        //VisionJobFile//VisionName//PkgName 폴더까지 들어와야함.
        //한비전단위로 로드세이브 되기에 세이브시에 PKG이름이나 갯수 변경사항 찌꺼기 남는 것을 방지 하기 위해.
        //해당 비전을 한번 다 지우고 해야 함.
        public bool LoadSave (bool _bLoad , string _sPath)
        {
            string FilePath = _sPath+"\\"+sName+"_"+GetType().Name +".ini";

            if (_bLoad)
            {
                CAutoIniFile.LoadStruct(FilePath,"MasterPara",ref MstPara ); 
                CAutoIniFile.LoadStruct(FilePath,"UserPara"  ,ref UsrPara ); 
                CAutoIniFile.LoadStruct(FilePath,"AlgoPara"  ,ref AlgPara ); 
            }
            else
            {
                //Vision에서 폴더 지움.if(File.Exists(_sPath)) File.Delete(_sPath);
                CAutoIniFile.SaveStruct(FilePath,"MasterPara",ref MstPara); 
                CAutoIniFile.SaveStruct(FilePath,"UserPara"  ,ref UsrPara);   
                CAutoIniFile.SaveStruct(FilePath,"AlgoPara"  ,ref AlgPara);   
                
            }

            CIniFile Ini = new CIniFile(FilePath);
            foreach (CTracker Tracker in lsTracker)
            {
                Tracker.LoadSave(_bLoad, "Tracker1" , Ini);
            }

            //Apply
            MstPara.PkgOk    = null ;
            MstPara.PkgNg    = null ;
            foreach(IPkg Pkg in GV.Visions[iVisionID].Pkgs)
            {
                if(Pkg.GetName() == MstPara.PkgNameOfOk    ) MstPara.PkgOk    = Pkg ;
                if(Pkg.GetName() == MstPara.PkgNameOfNg    ) MstPara.PkgNg    = Pkg ;
            }

            int iCrntPkgIdx = 0 ;
            for(int i = 0 ; i < GV.Visions[iVisionID].Pkgs.Count ; i++)
            {
                if(GV.Visions[iVisionID].Pkgs[i] == this)
                {
                    iCrntPkgIdx = i ;
                }
            }

            if(iCrntPkgIdx < GV.Visions[iVisionID].Pkgs.Count -1) 
            {
                if(MstPara.PkgOk == null) MstPara.PkgOk = GV.Visions[iVisionID].Pkgs[iCrntPkgIdx+1];
                if(MstPara.PkgNg == null) MstPara.PkgNg = GV.Visions[iVisionID].Pkgs[iCrntPkgIdx+1];
            }
            

            return true ;
        }
        #endregion

    }
}
