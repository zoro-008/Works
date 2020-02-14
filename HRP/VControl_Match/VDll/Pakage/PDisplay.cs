﻿using COMMON;
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

namespace VDll.Pakage
{
    public class PDisplay : CPkgObject , IPkg
    {
        readonly TProp Prop = new TProp{bHaveTrain  = false ,
                                        bHaveDialog = false ,
                                        bHaveImage  = false ,
                                        bHavePosRef = false };
        public enum EMathode
        {
            One   ,
            Two   ,
            Three 
        }

        public class CMstPara 
        {
            [CategoryAttribute("MPara"), DisplayNameAttribute("PkgName of Image"    )] public string   PkgNameOfImage    {get;set;} public IPkg PkgImage ;            
            [CategoryAttribute("MPara"), DisplayNameAttribute("PkgName of Result1"  )] public string   PkgNameOfRslt1    {get;set;} public IPkg PkgRslt1 ; 
            [CategoryAttribute("MPara"), DisplayNameAttribute("PkgName of Result2"  )] public string   PkgNameOfRslt2    {get;set;} public IPkg PkgRslt2 ; 
            [CategoryAttribute("MPara"), DisplayNameAttribute("PkgName of Result3"  )] public string   PkgNameOfRslt3    {get;set;} public IPkg PkgRslt3 ; 
            [CategoryAttribute("MPara"), DisplayNameAttribute("Mathode"             )] public EMathode Mathode           {get;set;} 

            public IPkg PkgNext    ; 
        }        

        

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class CUsrPara 
        {
            [CategoryAttribute("UPara"), DisplayNameAttribute("Threshold"  )] public uint         Threshold  {get;set;}   
            [CategoryAttribute("UPara"), DisplayNameAttribute("Mathode"    )] public EMathode     Mathode    {get;set;}   
        }

        private CMultiObjectSelector.CustomObjectType oRet;

        //저장 값들....
        CMstPara MstPara = new CMstPara();
        CUsrPara UsrPara = new CUsrPara(); //public TObj[] oUsrPara;
        //Object   AlgPara ;                 public TObj[] oAlgPara;

        //====================================
        List<CTracker> lsTracker = new List<CTracker>();
        TPos Pos = new TPos{dRefX   = 0.0 , //카메라는 기준포지션 없음.
                            dRefY   = 0.0 ,
                            dRefT   = 0.0 ,
                            dInspX  = 0.0 ,
                            dInspY  = 0.0 ,
                            dInspT  = 0.0 };

        public PDisplay(string _sName , int _iVisionID):base(_sName , _iVisionID)
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
            lsTracker.Add(new CTracker());

            //AlgPara = Activator.CreateInstance(typeof(CUPara));

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
                    CMultiObjectSelector.GetData(ref UsrPara, ref lsPara, "User"  );

                    oRet = new CMultiObjectSelector.CustomObjectType { props = lsPara };
                }
                return oRet;
            }
            
            set {
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

        public Mat GetTrainFormImage()
        {
            return MstPara.PkgImage.GetTrainFormImage()  ;
        }

        public Mat GetTrainImage ()
        {
            if (MstPara.PkgImage == null) return null;
            return MstPara.PkgImage.GetTrainImage()  ;
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



            //이건 그림만 그리면 됨...페인트에서 실직적인 일을 다함.
            GV.PaintCallbacks[iVisionID]();

            _tRet.bEnded  = true ;
            _tRet.NextPkg = MstPara.PkgNext ;

            return true ;
        }

        //GetImage() 에서 가져오는 이미지가 항상 쓰레드안전하지 않은 상태이므로 
        //모든 상황을 잘 따져보자.. GetImage 할때 버벅일 수 있음.
        public bool Paint(Graphics _g)// , TScaleOffset _ScaleOffset)//매개변수 화면 핸들 혹은 ImageBox포인터.
        {
            TScaleOffset OriSO ;
            try
            {
                if (MstPara.PkgImage != null)
                {
                    if(tScaleOffset.fScaleX == 0 && tScaleOffset.fScaleY ==0)
                    {
                        SetFitScaleOffset(_g.ClipBounds.Width , _g.ClipBounds.Height , MstPara.PkgImage.GetImage().Width , MstPara.PkgImage.GetImage().Height);
                    }
                    MstPara.PkgImage.GetImage().Paint(_g, tScaleOffset);
                }
                if (MstPara.PkgRslt1 != null)
                {
                    OriSO = MstPara.PkgRslt1.ScaleOffset ;
                    MstPara.PkgRslt1.ScaleOffset = tScaleOffset ;
                    MstPara.PkgRslt1.Paint(_g);
                    MstPara.PkgRslt1.ScaleOffset = OriSO ;
                }
                if (MstPara.PkgRslt2 != null)
                {
                    OriSO = MstPara.PkgRslt2.ScaleOffset ;
                    MstPara.PkgRslt2.ScaleOffset = tScaleOffset ;
                    MstPara.PkgRslt2.Paint(_g);
                    MstPara.PkgRslt2.ScaleOffset = OriSO ;
                }
                if (MstPara.PkgRslt3 != null)
                {
                    OriSO = MstPara.PkgRslt3.ScaleOffset ;
                    MstPara.PkgRslt3.ScaleOffset = tScaleOffset ;
                    MstPara.PkgRslt3.Paint(_g);
                    MstPara.PkgRslt3.ScaleOffset = OriSO ;
                }
            }
            catch (Exception _e)
            {
                sError = _e.ToString();
                return false;
            }
            return true;
        }

        public bool PaintTrain (Graphics _g)
        {
            TScaleOffset OriSO ;
            try
            {
                if (MstPara.PkgImage != null)
                {
                    if(tScaleOffset.fScaleX == 0 && tScaleOffset.fScaleY ==0)
                    {
                        SetFitScaleOffset(_g.ClipBounds.Width , _g.ClipBounds.Height , MstPara.PkgImage.GetTrainImage().Width , MstPara.PkgImage.GetTrainImage().Height);
                    }
                    MstPara.PkgImage.GetTrainImage().Paint(_g, tScaleOffset);
                }
                if (MstPara.PkgRslt1 != null)
                {
                    OriSO = MstPara.PkgRslt1.ScaleOffset ;
                    MstPara.PkgRslt1.ScaleOffset = tScaleOffset ;
                    MstPara.PkgRslt1.PaintTrain(_g);
                    MstPara.PkgRslt1.ScaleOffset = OriSO ;
                }
                if (MstPara.PkgRslt2 != null)
                {
                    OriSO = MstPara.PkgRslt2.ScaleOffset ;
                    MstPara.PkgRslt2.ScaleOffset = tScaleOffset ;
                    MstPara.PkgRslt2.PaintTrain(_g);
                    MstPara.PkgRslt2.ScaleOffset = OriSO ;
                }
                if (MstPara.PkgRslt3 != null)
                {
                    OriSO = MstPara.PkgRslt3.ScaleOffset ;
                    MstPara.PkgRslt3.ScaleOffset = tScaleOffset ;
                    MstPara.PkgRslt3.PaintTrain(_g);
                    MstPara.PkgRslt3.ScaleOffset = OriSO ;
                }
            }
            catch (Exception _e)
            {
                sError = _e.ToString();
                return false;
            }
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
            return false ;
        }
        public bool MouseMove(MouseEventArgs _e)
        {
            return false ;
        }
        public bool MouseUp  (MouseEventArgs _e)
        {
            return false ;
        }

        //VisionJobFile//VisionName//PkgName 폴더까지 들어와야함.
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
            }

            //다음 자제 뽑아놓기.
            MstPara.PkgNext = null ;
            for(int i = 0 ; i < GV.Visions[iVisionID].Pkgs.Count - 1 ; i++)
            {
                if(GV.Visions[iVisionID].Pkgs[i] == this)
                {
                    MstPara.PkgNext = GV.Visions[iVisionID].Pkgs[i+1];
                    break ;
                }
            }



            MstPara.PkgImage = null ;
            MstPara.PkgRslt1 = null ;
            MstPara.PkgRslt2 = null ;
            MstPara.PkgRslt3 = null ;
            foreach(IPkg Pkg in GV.Visions[iVisionID].Pkgs)
            {
                if(Pkg.GetName() == MstPara.PkgNameOfImage ) MstPara.PkgImage = Pkg ;
                if(Pkg.GetName() == MstPara.PkgNameOfRslt1 ) MstPara.PkgRslt1 = Pkg ;
                if(Pkg.GetName() == MstPara.PkgNameOfRslt2 ) MstPara.PkgRslt2 = Pkg ;
                if(Pkg.GetName() == MstPara.PkgNameOfRslt3 ) MstPara.PkgRslt3 = Pkg ;
            }

            return true ;
        }
        #endregion
    }
}
