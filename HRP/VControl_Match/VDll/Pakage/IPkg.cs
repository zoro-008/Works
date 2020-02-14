using System.Collections.Generic;
using System.Drawing;
using Emgu.CV;
using System.Windows.Forms;
using Emgu.CV.CvEnum;
using System.Runtime.InteropServices;
using System;

namespace VDll.Pakage
{
    public struct TProp
    {
        public bool bHaveTrain  ; //트레인이 필요한 sd
        public bool bHaveDialog ; //별도 다이얼로그를 가지고 있는
        public bool bHaveImage  ; //이미지 컨테이너 PKG
        public bool bHavePosRef ; //포지션 참조 가능한.
    }

    public struct TPos
    {
        public double dRefX     ; //티칭상의 X
        public double dRefY     ; //티칭상의 Y
        public double dRefT     ; //티칭상의 T

        public double dInspX    ; //검사상의 X
        public double dInspY    ; //검사상의 Y
        public double dInspT    ; //검사상의 T 
    }

    public struct TScaleOffset
    {

        public float fScaleX ;
        public float fScaleY ;

        public float fOffsetX ;
        public float fOffsetY ;
    }

    public struct TRunPara //런함수의 리턴 값.
    {
        public bool bNeedPopImg  ;
    }

    public struct TRunRet //런함수의 리턴 값.
    {
        public IPkg NextPkg ;
        public bool bEnded  ;
    }

    public static class MatExtension
    {
        public static dynamic GetValue(this Mat mat, int col ,int row)
        {
            var value = CreateElement(mat.Depth);
            Marshal.Copy(mat.DataPointer + (row * mat.Cols + col) * mat.ElementSize, value, 0, 1);
            return value[0];
        }

        public static void SetValue(this Mat mat, int col, int row, dynamic value)
        {
            var target = CreateElement(mat.Depth, value);
            Marshal.Copy(target, 0, mat.DataPointer + (row * mat.Cols + col) * mat.ElementSize, 1);
        }

        private static dynamic CreateElement(DepthType depthType, dynamic value)
        {
            var element = CreateElement(depthType);
            element[0] = value;
            return element;
        }

        private static dynamic CreateElement(DepthType depthType)
        {
            if (depthType == DepthType.Cv8S)
            {
                return new sbyte[1];
            }
            if (depthType == DepthType.Cv8U)
            {
                return new byte[1];
            }
            if (depthType == DepthType.Cv16S)
            {
                return new short[1];
            }
            if (depthType == DepthType.Cv16U)
            {
                return new ushort[1];
            }
            if (depthType == DepthType.Cv32S)
            {
                return new int[1];
            }
            if (depthType == DepthType.Cv32F)
            {
                return new float[1];
            }
            if (depthType == DepthType.Cv64F)
            {
                return new double[1];
            }
            return new float[1];
        }

        public static bool Paint(this Mat _Mat , Graphics _g , TScaleOffset _ScaleOffset , float _fMin = 0f, float _fMax = 0f )//매개변수 화면 핸들 혹은 ImageBox포인터.
        {
            Rectangle Rect = new Rectangle(0,0,(int)_g.ClipBounds.Width , (int)_g.ClipBounds.Height) ;

            
            if(_Mat.Depth == DepthType.Cv8U)
            {
                try
                {
                    _g.DrawImage(_Mat.Bitmap , 
                                 Rect , 
                                 _ScaleOffset.fOffsetX , 
                                 _ScaleOffset.fOffsetY , 
                                 _g.ClipBounds.Width  / _ScaleOffset.fScaleX , 
                                 _g.ClipBounds.Height / _ScaleOffset.fScaleY , 
                                 GraphicsUnit.Pixel);
                }
                catch(Exception _e)
                {
                    return false ;
                }
            }
            else if(_Mat.Depth == DepthType.Cv32F)
            {
                double min = _fMin;
                double max = _fMax;
                int[] minIdx = null;
                int[] maxIdx = null;
                
                if(min == 0f && max == 0f)
                {
                    CvInvoke.MinMaxIdx(_Mat, out min, out max, minIdx, maxIdx);
                }

                if(min == max) 
                {
                    return false ;
                }

                using(Mat ScaledImg = new Mat(_Mat.Rows, _Mat.Cols, DepthType.Cv8U, 4))
                {
                    double scale = 255.0 / (max - min);
                    _Mat.ConvertTo(ScaledImg, DepthType.Cv8U, scale, -min * scale);

                    try
                    {
                        _g.DrawImage(ScaledImg.Bitmap , 
                                     Rect , 
                                     _ScaleOffset.fOffsetX , 
                                     _ScaleOffset.fOffsetY , 
                                     _g.ClipBounds.Width  / _ScaleOffset.fScaleX , 
                                     _g.ClipBounds.Height / _ScaleOffset.fScaleY , 
                                     GraphicsUnit.Pixel);
                    }
                    catch(Exception _e)
                    {
                        return false ;
                    }
                }
                
            }
            else 
            {
                return false ;
            }
            

            return true ;

        }
    }

    public class CPkgObject : CErrorObject
    {
        protected TScaleOffset tScaleOffset  = new TScaleOffset() ;
        public CPkgObject(string _sName , int _iVisionID):base(_sName)
        {    
            iVisionID = _iVisionID ;
        }

        protected int iVisionID ;
        public int VisionID 
        {
            get{ return iVisionID ;}
        }

        protected void SetFitScaleOffset(float _fPanelWidth , float _fPanelHeight , int _iImgWidth , int _iImgHeight)
        {
            float WidthScale  = _fPanelWidth  / (float)_iImgWidth  ;
            float HeightScale = _fPanelHeight / (float)_iImgHeight ;
            TScaleOffset ScaleOffset = new TScaleOffset();
            ScaleOffset.fScaleX  = WidthScale < HeightScale ? WidthScale : HeightScale ;
            ScaleOffset.fScaleY  = WidthScale < HeightScale ? WidthScale : HeightScale ;

            float fMaxOfsX =  _iImgWidth   - _fPanelWidth  / ScaleOffset.fScaleX ;
            float fMaxOfsY =  _iImgHeight  - _fPanelHeight / ScaleOffset.fScaleY ;
            if(fMaxOfsX < 0) fMaxOfsX /= 2.0f ;
            if(fMaxOfsY < 0) fMaxOfsY /= 2.0f ;
            if(ScaleOffset.fOffsetX < 0       ) ScaleOffset.fOffsetX = 0 ;
            if(ScaleOffset.fOffsetY < 0       ) ScaleOffset.fOffsetY = 0 ;
            if(ScaleOffset.fOffsetX > fMaxOfsX) ScaleOffset.fOffsetX = fMaxOfsX ;
            if(ScaleOffset.fOffsetY > fMaxOfsY) ScaleOffset.fOffsetY = fMaxOfsY ;

            tScaleOffset = ScaleOffset;
        }

        

    }
    //트레커 별 검사는 하지 않는것이 좋을듯.
    //다만 서치 매치 같은경우 다각형이 지원이 되면서 한트레커 안에서 여러개의 새끼 트레커가 존재 할 수 있게 만듬.=>다시 그냥 꾸러미 개념으로 ㄱ ㄱ ㄱ 
    //일이 너무 커져서 만약 검사 세팅이 달라졌을경우.
    public interface IPkg
    {
        bool           Init();
        bool           Close();
        //외부에서 보는 현재 패키지 특성.
        TProp          GetProp  ();
        
        //마스터 파라 및 유저파라.
        object         MPara  {get ;set;}
        object         UPara  {get ;set;}
                       
        string         GetName ();
        string         GetError();

        //트레커 외부에서 클릭 처리 해야함.
        List<CTracker> Trackers {get ;}
        
        Mat            GetImage   ();
        Mat            GetTrainFormImage(); //현재 패키지의 Display 
        Mat            GetTrainImage();
        TPos           GetPos     ();
                       
        bool           Train      ();
        bool           Run        (TRunPara _tPara , ref TRunRet _tRet);
        bool           Paint      (Graphics _g);// , TScaleOffset _ScaleOffset);//매개변수 화면 핸들 혹은 ImageBox포인터.
        bool           PaintTrain (Graphics _g);// , TScaleOffset _ScaleOffset);//매개변수 화면 핸들 혹은 ImageBox포인터.
        TScaleOffset   ScaleOffset{get ; set;}

        bool           ShowDialog (); //bHaveDialog 가 on인 페키지만 Show 해줌.

        //일단 삭제 따져보니 Vision까지만 Autorun함수 있으면 될듯.
        bool           Autorun    (bool _bAutorun); //오토런 전환시 수행할 것들함.
        bool           Ready      ();  //오토런중에 뭔가 미리 준비 해야 될 것들 있으면 한다.
                      
        bool           MouseDown  (MouseEventArgs _e);// , TScaleOffset _ScaleOffset);
        bool           MouseMove  (MouseEventArgs _e);// , TScaleOffset _ScaleOffset);
        bool           MouseUp    (MouseEventArgs _e);// , TScaleOffset _ScaleOffset);
                                  
        bool           LoadSave   (bool _bLoad , string _sPath);

        
    }
    //첫번째 패키지는 카메라를 놓도록 하고.
    //일단 외부에서 소프트 트리거든 하드웨어 트리거든. 검사는 쓰레드에서 이미지리스트 보고 있다가 이미지 리스트에 이미지가 있으면 하는 걸로
}
