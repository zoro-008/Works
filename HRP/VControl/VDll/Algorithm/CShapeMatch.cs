using Emgu.CV;
using Emgu.CV.Cuda;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.XFeatures2D;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDll.Algorithm
{
    class CShapeMatch
    {
        [Serializable ,TypeConverter(typeof(ExpandableObjectConverter))]
        public class CUPara
        {                                                  
            [CategoryAttribute("UPara" ), DisplayNameAttribute("K"                   )] public int     K                     {get;set;} = 2   ;
            [CategoryAttribute("UPara" ), DisplayNameAttribute("Uniqueness Threshold")] public double  UniquenessThreshold   {get;set;} = 0.8 ;
            [CategoryAttribute("UPara" ), DisplayNameAttribute("Hessian Thresh"      )] public double  HessianThresh         {get;set;} = 300 ;
        }

        public struct TRslt
        {
            public double dImgAngle ;
            public double dX ;
            public double dY ;
            public double dScore ;
            public string sError ;
        }

        public static bool Find(Mat _mtImage , Mat _mtModel , CUPara _upPara , ref TRslt _tRslt , out Mat _mtResult)
        {
            Mat homography;
            VectorOfKeyPoint modelKeyPoints;
            VectorOfKeyPoint observedKeyPoints;
            using (VectorOfVectorOfDMatch matches = new VectorOfVectorOfDMatch())
            {
                Mat mask;
                FindShape(_mtModel, _mtImage, _upPara , out modelKeyPoints, out observedKeyPoints, matches, out mask, out homography);
                
                _mtResult = new Mat();
                Features2DToolbox.DrawMatches(_mtModel, modelKeyPoints, _mtImage, observedKeyPoints,
                   matches, _mtResult, new MCvScalar(255, 255, 255), new MCvScalar(255, 255, 255), mask);

                if (homography != null)
                {
                    //draw a rectangle along the projected model
                    Rectangle rect = new Rectangle(Point.Empty, _mtModel.Size);
                    PointF[] pts = new PointF[]
                    {
                         new PointF(rect.Left, rect.Bottom),
                         new PointF(rect.Right, rect.Bottom),
                         new PointF(rect.Right, rect.Top),
                         new PointF(rect.Left, rect.Top)
                    };
                    pts = CvInvoke.PerspectiveTransform(pts, homography);
                    _tRslt.dX = pts[3].X ;
                    _tRslt.dY = pts[3].Y ;


                    Point[] points = Array.ConvertAll<PointF, Point>(pts, Point.Round);
                    using (VectorOfPoint vp = new VectorOfPoint(points))
                    {
                        CvInvoke.Polylines(_mtResult, vp, true, new MCvScalar(255, 0, 0, 255), 5);
                    }

                }

                //return result;
                return true;

            }


        }


        //static public bool FindMatch (Mat _mtImage , Mat _mtTemplate , CUPara _upPara , ref TRslt _tRslt)
        public static bool FindShape(Mat modelImage, Mat observedImage , CUPara _upPara ,  out VectorOfKeyPoint modelKeyPoints, out VectorOfKeyPoint observedKeyPoints, VectorOfVectorOfDMatch matches, out Mat mask, out Mat homography)
        {
            //int k = 2;
            //double uniquenessThreshold = 0.8;
            //double hessianThresh = 300;

            //int k = 2;
            //double uniquenessThreshold = 0.8;
            //double hessianThresh = 300;

            homography = null;

            modelKeyPoints = new VectorOfKeyPoint();
            observedKeyPoints = new VectorOfKeyPoint();

#if !__IOS__
            if (CudaInvoke.HasCuda)
            {
                CudaSURF surfCuda = new CudaSURF((float)_upPara.HessianThresh);
                using (GpuMat gpuModelImage = new GpuMat(modelImage))
                //extract features from the object image
                using (GpuMat gpuModelKeyPoints = surfCuda.DetectKeyPointsRaw(gpuModelImage, null))
                using (GpuMat gpuModelDescriptors = surfCuda.ComputeDescriptorsRaw(gpuModelImage, null, gpuModelKeyPoints))
                using (CudaBFMatcher matcher = new CudaBFMatcher(DistanceType.L2))
                {
                    surfCuda.DownloadKeypoints(gpuModelKeyPoints, modelKeyPoints);

                    // extract features from the observed image
                    using (GpuMat gpuObservedImage = new GpuMat(observedImage))
                    using (GpuMat gpuObservedKeyPoints = surfCuda.DetectKeyPointsRaw(gpuObservedImage, null))
                    using (GpuMat gpuObservedDescriptors = surfCuda.ComputeDescriptorsRaw(gpuObservedImage, null, gpuObservedKeyPoints))
                    //using (GpuMat tmp = new GpuMat())
                    //using (Stream stream = new Stream())
                    {
                        matcher.KnnMatch(gpuObservedDescriptors, gpuModelDescriptors, matches, _upPara.K);

                        surfCuda.DownloadKeypoints(gpuObservedKeyPoints, observedKeyPoints);

                        mask = new Mat(matches.Size, 1, DepthType.Cv8U, 1);
                        mask.SetTo(new MCvScalar(255));
                        Features2DToolbox.VoteForUniqueness(matches, _upPara.UniquenessThreshold , mask);

                        int nonZeroCount = CvInvoke.CountNonZero(mask);
                        if (nonZeroCount >= 4)
                        {
                            nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints, observedKeyPoints,
                               matches, mask, 1.5, 20);
                            if (nonZeroCount >= 4)
                                homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(modelKeyPoints,
                                   observedKeyPoints, matches, mask, 2);
                        }
                    }
                }
            }
            else
#endif
            {
                using (UMat uModelImage = modelImage.GetUMat(AccessType.Read))
                using (UMat uObservedImage = observedImage.GetUMat(AccessType.Read))
                {
                    SURF surfCPU = new SURF(_upPara.HessianThresh);
                    //extract features from the object image
                    UMat modelDescriptors = new UMat();
                    surfCPU.DetectAndCompute(uModelImage, null, modelKeyPoints, modelDescriptors, false);

                    //watch = Stopwatch.StartNew();

                    // extract features from the observed image
                    UMat observedDescriptors = new UMat();
                    surfCPU.DetectAndCompute(uObservedImage, null, observedKeyPoints, observedDescriptors, false);
                    BFMatcher matcher = new BFMatcher(DistanceType.L2);
                    matcher.Add(modelDescriptors);

                    matcher.KnnMatch(observedDescriptors, matches, _upPara.K , null);
                    mask = new Mat(matches.Size, 1, DepthType.Cv8U, 1);
                    mask.SetTo(new MCvScalar(255));
                    Features2DToolbox.VoteForUniqueness(matches, _upPara.UniquenessThreshold , mask);

                    int nonZeroCount = CvInvoke.CountNonZero(mask);
                    if (nonZeroCount >= 4)
                    {
                        nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints, observedKeyPoints,
                           matches, mask, 1.5, 20);
                        if (nonZeroCount >= 4)
                            homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(modelKeyPoints,
                               observedKeyPoints, matches, mask, 2);
                    }

                    //watch.Stop();
                }
            }
            return true ;
        }    
    }
}
