using COMMON;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VDll.Algorithm;
//using VDll.Algorithm;

namespace VDll.Pakage
{
    public class PMatch : CPkgObject , IPkg
    {
        readonly TProp Prop = new TProp{bHaveTrain  = true  ,
                                        bHaveDialog = true  ,
                                        bHaveImage  = false ,
                                        bHavePosRef = true  };

        public enum EMatchMode
        {
            Shape     = 0 ,
            Template      ,
            Search         
        }

        public class CMstPara 
        {
            [CategoryAttribute("MPara"), DisplayNameAttribute("Match Mode"          )] public EMatchMode   MatchMode         {get;set;}             
            [CategoryAttribute("MPara"), DisplayNameAttribute("PkgName of Image"    )] public string       PkgNameOfImage    {get;set;} public IPkg PkgImage ;            
            [CategoryAttribute("MPara"), DisplayNameAttribute("PkgName of Pos"      )] public string       PkgNameOfPos      {get;set;} public IPkg PkgPos   ; 
            [CategoryAttribute("MPara"), DisplayNameAttribute("PkgName of Ok Result")] public string       PkgNameOfOk       {get;set;} public IPkg PkgOk    ; 
            [CategoryAttribute("MPara"), DisplayNameAttribute("PkgName of Ng Result")] public string       PkgNameOfNg       {get;set;} public IPkg PkgNg    ; 
        }        

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class CUsrPara 
        {
            [CategoryAttribute("UPara"), DisplayNameAttribute("Threshold"  )] public uint         Threshold  {get;set;}        
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

        public PMatch(string _sName , int _iVisionID):base(_sName , _iVisionID)
        {
            //현재 세팅 패키지와 가장 가까운 이미지를 가지고 있는 PKG로 초기화 세팅.
            foreach(IPkg Pkg in GV.Visions[iVisionID].Pkgs)
            {
                TProp Prop =  Pkg.GetProp() ;
                if(Prop.bHaveImage) MstPara.PkgNameOfImage = Pkg.GetName() ;
            }
        }

        #region IPkg
        public bool Init()
        {
            CTracker Tracker = new CTracker();
            Tracker.Init();


            lsTracker.Add(Tracker);


            //나중에 알고리즘 바꿀때는 AlgPara = Activator.CreateInstance( /*여기 생각좀 하자...*/); //sun
            AlgPara = Activator.CreateInstance(typeof(CTempMatch.CUPara));

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
                    CMultiObjectSelector.GetData(ref AlgPara, ref lsPara , "User");
                    CMultiObjectSelector.GetData(ref UsrPara, ref lsPara , "User");

                    oRet = new CMultiObjectSelector.CustomObjectType { props = lsPara };
                }
                return oRet;
            }

            set {
                CMultiObjectSelector.SetProperty(ref AlgPara, value , "User");
                CMultiObjectSelector.SetProperty(ref UsrPara, value , "User");
            }
        }

        public string GetName ()  {return sName;  }
        public string GetError()  {return sError; }

        //트레커 외부에서 클릭 처리 해야함.
        public List<CTracker> Trackers {get { return lsTracker ;}}        
        public Mat GetImage ()
        {
            if (MstPara.PkgImage == null) return null;
            return MstPara.PkgImage.GetImage()  ;
        }

        public Mat GetTrainFormImage ()
        {
            if (MstPara.PkgImage == null) return null;
            return MstPara.PkgImage.GetTrainImage()  ;
        }

        public Mat GetTrainImage ()
        {
            return null;
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
            _tRet.bEnded  = false ;
            _tRet.NextPkg = this  ;

            //if(GV.Trainings[iVisionID])
            //{
            //    Img = new Mat(MstPara.PkgImage.GetTrainImage() , Rect);
            //}
            //else 
            //{
            //    Img = new Mat(MstPara.PkgImage.GetImage() , Rect);
            //}

            //이미지 프로세싱.
            _tRet.bEnded  = true ;
            _tRet.NextPkg = null ;
            for(int i = 0 ; i < GV.Visions[iVisionID].Pkgs.Count ; i++)
            {
                if(GV.Visions[iVisionID].Pkgs[i] == this)
                {
                    if(GV.Visions[iVisionID].Pkgs.Count > i+1)
                    {
                        _tRet.NextPkg = GV.Visions[iVisionID].Pkgs[i+1];
                    }
                }
            }
            return true ;
        }

        //GetImage() 에서 가져오는 이미지가 항상 쓰레드안전하지 않은 상태이므로 
        //모든 상황을 잘 따져보자.. GetImage 할때 버벅일 수 있음.
        public bool Paint(Graphics _g)// , TScaleOffset _ScaleOffset)//매개변수 화면 핸들 혹은 ImageBox포인터.
        {
            //찾은곳 네모 그리기...
            //매치율 표시.
            return true;
        }

        public bool PaintTrain (Graphics _g)
        {
            try
            {
                Mat TrainImg = MstPara.PkgImage.GetImage();
                //Rectangle Rect = new Rectangle(0, 0, (int)_g.ClipBounds.Width, (int)_g.ClipBounds.Height);

                if(tScaleOffset.fScaleX == 0 && tScaleOffset.fScaleY ==0)
                {
                    SetFitScaleOffset(_g.ClipBounds.Width , _g.ClipBounds.Height , MstPara.PkgImage.GetImage().Width , MstPara.PkgImage.GetImage().Height);
                }

                TrainImg.Paint(_g, tScaleOffset);

                foreach (CTracker Tracker in lsTracker)
                {
                    Tracker.Paint(_g, tScaleOffset);
                }
            }
            catch (Exception _e)
            {
                sError = _e.ToString();
                return false;
            }

            _g.DrawString("Match 결과", new Font("헤움봄비152", 20), Brushes.Black, 0, 0);
            return true;
        }

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
            return true;
        }

        //오토런중에 뭔가 미리 준비 해야 될 것들 있으면 한다.
        public bool Ready()
        {
            return true;
        }

        public bool MouseDown(MouseEventArgs _e)
        {
            bool bRlst = false;
            bool bRet = false;
            foreach (CTracker Tracker in lsTracker)
            {
                bRlst = Tracker.MouseDown(Control.ModifierKeys, _e, tScaleOffset);
                if (bRlst == true) bRet = true;
            }
            return bRet ;
        }
        public bool MouseMove(MouseEventArgs _e)
        {
            bool bRlst = false;
            bool bRet = false;
            foreach (CTracker Tracker in lsTracker)
            {
                bRlst = Tracker.MouseMove(Control.ModifierKeys, _e, tScaleOffset);
                if (bRlst == true) bRet = true;
            }
            return bRet;
        }
        public bool MouseUp  (MouseEventArgs _e)
        {
            bool bRlst = false;
            bool bRet = false;
            foreach (CTracker Tracker in lsTracker)
            {
                bRlst = Tracker.MouseUp(Control.ModifierKeys, _e);
                if (bRlst == true) bRet = true;
            }
            return bRet;
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
            }
            else
            {
                //Vision에서 폴더 지움.if(File.Exists(_sPath)) File.Delete(_sPath);
                CAutoIniFile.SaveStruct(FilePath,"MasterPara",ref MstPara); 
                CAutoIniFile.SaveStruct(FilePath,"UserPara"  ,ref UsrPara);    
                
                foreach(CTracker Tracker in lsTracker)
                {
                    //Tracker.

                }
            }




            //Apply
            MstPara.PkgImage = null ;
            MstPara.PkgPos   = null ;
            foreach(IPkg Pkg in GV.Visions[iVisionID].Pkgs)
            {
                if(Pkg.GetName() == MstPara.PkgNameOfImage ) MstPara.PkgImage = Pkg ;
                if(Pkg.GetName() == MstPara.PkgNameOfPos   ) MstPara.PkgPos   = Pkg ;
            }





            return true ;
        }
        #endregion
    }
}
