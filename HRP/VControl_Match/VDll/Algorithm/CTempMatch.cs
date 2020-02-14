using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Flann;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.ComponentModel;
using System.Drawing;

namespace VDll.Algorithm
{
    class CTempMatch
    {

        [Serializable ,TypeConverter(typeof(ExpandableObjectConverter))]
        public class CUPara
        {                                                  
            [CategoryAttribute("UPara" ), DisplayNameAttribute("Uniq Threshold"   )] public double  UniqThreshold    {get;set;}  //0.8
            [CategoryAttribute("UPara" ), DisplayNameAttribute("Count Non Zero"   )] public uint    CountNonZero     {get;set;}  //4
            [CategoryAttribute("UPara" ), DisplayNameAttribute("Scale Increment"  )] public double  ScaleIncrement   {get;set;}  //1.5
            [CategoryAttribute("UPara" ), DisplayNameAttribute("Rotation Bins"    )] public int     RotationBins     {get;set;}  //20
            [CategoryAttribute("UPara" ), DisplayNameAttribute("Ransac Threashold")] public int     RansacThreashold {get;set;}  //2  
        }

        public struct TRslt
        {
            public double dCntX ;
            public double dCntY ;
        }

        static public bool FindMatch (Mat _mtImage , Mat _mtTrainImage , CUPara _upPara , ref TRslt _tRslt)
        {
            Mat mtHomography;
            VectorOfKeyPoint TrainKeyPoint; //트레인 특징점변수.
            VectorOfKeyPoint ImageKeyPoint; //입력영상 특징점 변수
            using (VectorOfVectorOfDMatch Matches = new VectorOfVectorOfDMatch())
            {
                Mat Mask;//대응점 저장용.
                int k = 2;
                
                mtHomography = null;
                
                TrainKeyPoint = new VectorOfKeyPoint();
                ImageKeyPoint = new VectorOfKeyPoint();
                
                using (UMat uModelImage = _mtTrainImage.GetUMat(AccessType.Read))
                using (UMat uObservedImage = _mtImage.GetUMat(AccessType.Read))
                {
                    KAZE featureDetector = new KAZE();
                
                    //extract features from the object image
                    //트레인 특징 뽑자 이거 나중에 밖에서 이미지 대신에 트레인 특징을 저장 하는 수도 있어야 할 지 모름.
                    //sun 
                    Mat mtModelDescriptors = new Mat();
                    featureDetector.DetectAndCompute(uModelImage, null, TrainKeyPoint, mtModelDescriptors, false);
                
                    // extract features from the observed image
                    //특징뽑고.
                    Mat mtObservedDescriptors = new Mat();
                    featureDetector.DetectAndCompute(uObservedImage, null, ImageKeyPoint, mtObservedDescriptors, false);
                
                    // Bruteforce, slower but more accurate
                    // You can use KDTree for faster matching with slight loss in accuracy
                    using (Emgu.CV.Flann.LinearIndexParams ip = new Emgu.CV.Flann.LinearIndexParams()) 
                    using (Emgu.CV.Flann.SearchParams sp = new SearchParams())
                    using (DescriptorMatcher Matcher = new FlannBasedMatcher(ip, sp))
                    {
                        Matcher.Add(mtModelDescriptors);
                
                        Matcher.KnnMatch(mtObservedDescriptors, Matches, k, null);
                        Mask = new Mat(Matches.Size, 1, DepthType.Cv8U, 1);
                        Mask.SetTo(new MCvScalar(255));
                        Features2DToolbox.VoteForUniqueness(Matches, _upPara.UniqThreshold , Mask);//거리비를 지정한 쓰레숄을 따져서 대응점을 선정합니다.
                
                        int nonZeroCount = CvInvoke.CountNonZero(Mask);//논제로값이 존재하면 대응점 추출됨 -> 이미지에 표시
                        if (nonZeroCount >= _upPara.CountNonZero)  //일단 4로 세팅.
                        {
                            //사이즈와 회전으로 적용 하는듯??
                            nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(TrainKeyPoint, ImageKeyPoint, Matches, Mask, _upPara.ScaleIncrement, _upPara.RotationBins);
                            if (nonZeroCount >= _upPara.CountNonZero)
                            {
                                mtHomography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(TrainKeyPoint, ImageKeyPoint, Matches, Mask, _upPara.RansacThreashold);
                            }
                        }
                    }
                }
            }

            if (mtHomography == null) return false ;

            //draw a rectangle along the projected model
            Rectangle rect = new Rectangle(Point.Empty, _mtTrainImage.Size);
            PointF[] pts = new PointF[]
            {
                new PointF(rect.Left, rect.Bottom),
                new PointF(rect.Right, rect.Bottom),
                new PointF(rect.Right, rect.Top),
                new PointF(rect.Left, rect.Top)
            };
            pts = CvInvoke.PerspectiveTransform(pts, mtHomography);

            double dSumX = 0.0 ;
            double dSumY = 0.0 ;
            foreach(PointF Pnt in pts)
            {
                dSumX += Pnt.X ;
                dSumY += Pnt.Y ;
            }

            _tRslt.dCntX = dSumX /4.0 ;
            _tRslt.dCntY = dSumY /4.0 ;

            return true ;
        }

        static public bool FindMatch2 (Mat _mtImage , Mat [] _mtTrainImages , CUPara _upPara , ref TRslt _tRslt)
        {
            /// Source image to display
            Mat mtResult = new Mat();
           
            for(int i = 0 ; i < _mtTrainImages.Length ; i++)
            {
                //가장 확율높은 곳의 좌표를 뽑기 위해 리절트맵을 만듬.
                int iRsltCol =  _mtImage.Cols - _mtTrainImages[i].Cols + 1;
                int iRsltRow =  _mtImage.Rows - _mtTrainImages[i].Rows + 1;

                mtResult.Create( iRsltRow, iRsltCol, DepthType.Cv32F , 1); //32비트float , 1chanel
                Emgu.CV.CvInvoke.MatchTemplate(_mtImage , _mtTrainImages[i] , mtResult ,TemplateMatchingType.)

            }
            /// Create the result matrix
            
            
           
            
           
            /// Do the Matching and Normalize
            matchTemplate( img, templ, result, match_method );
            normalize( result, result, 0, 1, NORM_MINMAX, -1, Mat() );
           
            /// Localizing the best match with minMaxLoc
            double minVal; double maxVal; Point minLoc; Point maxLoc;
            Point matchLoc;
           
            minMaxLoc( result, &minVal, &maxVal, &minLoc, &maxLoc, Mat() );
           
            /// For SQDIFF and SQDIFF_NORMED, the best matches are lower values. For all the other methods, the higher the better
            if( match_method  == CV_TM_SQDIFF || match_method == CV_TM_SQDIFF_NORMED )
              { matchLoc = minLoc; }
            else
              { matchLoc = maxLoc; }
           
            /// Show me what you got
            rectangle( img_display, matchLoc, Point( matchLoc.x + templ.cols , matchLoc.y + templ.rows ), Scalar::all(0), 2, 8, 0 );
            rectangle( result, matchLoc, Point( matchLoc.x + templ.cols , matchLoc.y + templ.rows ), Scalar::all(0), 2, 8, 0 );
           
            imshow( image_window, img_display );
            imshow( result_window, result );

            //_tRslt.dCntX = dSumX /4.0 ;
            //_tRslt.dCntY = dSumY /4.0 ;

            return true ;
        }

//        static public void Paint()
//        {

//                            //Draw the matched keypoints
//                Mat result = new Mat();
//                Features2DToolbox.DrawMatches(modelImage, modelKeyPoints, observedImage, observedKeyPoints,
//                   matches, result, new MCvScalar(255, 255, 255), new MCvScalar(255, 255, 255), mask);

//#region draw the projected region on the image

//                if (homography != null)
//                {
//                    //draw a rectangle along the projected model
//                    Rectangle rect = new Rectangle(Point.Empty, modelImage.Size);
//                    PointF[] pts = new PointF[]
//                    {
//                        new PointF(rect.Left, rect.Bottom),
//                        new PointF(rect.Right, rect.Bottom),
//                        new PointF(rect.Right, rect.Top),
//                        new PointF(rect.Left, rect.Top)
//                    };
//                    pts = CvInvoke.PerspectiveTransform(pts, homography);

//#if NETFX_CORE
//               Point[] points = Extensions.ConvertAll<PointF, Point>(pts, Point.Round);
//#else
//                    Point[] points = Array.ConvertAll<PointF, Point>(pts, Point.Round);
//#endif
//                    using (VectorOfPoint vp = new VectorOfPoint(points))
//                    {
//                        CvInvoke.Polylines(result, vp, true, new MCvScalar(255, 0, 0, 255), 5);
//                    }
//                }
//#endregion

//                return true;
//        }
    }
}
