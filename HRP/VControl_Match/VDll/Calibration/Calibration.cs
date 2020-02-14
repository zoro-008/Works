using COMMON;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VDll.Pakage;

/*
1. 캘리브레이션 -> 호모그래피 순서로 만들기. 커몬옵션에 옵션처리.
2. PKG List 제거 하고 한번에 캘리브레이션과 호모그래피 변환 같이 하게.
3. 처음에 찾은 도트 센터 표시 Red+    
4. 실제좌표계 상의 pobject Yellow+ 디스플레이.
5. 보정후에 찾은 도트 센터 표시 Lime+ 디스플레이.
6. 보정후 센터로 와야 하는것은 보정전 센터의 보정후 값이 되어야 한다.
7. homography.para.Width , homography.para.Height 는 0이면 오토로 현재 찾은 도트배열의 X값평균과 Y값평균을 이용 , 
   더좋은 방법은 보정전과 보정후의 도트의 거리차가 가장 적게 인데 방법은 찾아봐야함.
8. 보정전과 보정후의 도트간의 거리차(X, Y 개별이 아닌 그냥거리) 평균을 Display.
9. X,Y는 0,0 일때 6번 항목 적용 되면됨. 
 
 
 */

namespace VDll
{
    public class Calibration
    {
		//public static Calibration Instance { 
		//	get { return instance == null ? (instance = new Calibration()) : instance; } 
		//}
		//private static Calibration instance = null;

        public delegate void FInvalidate(Mat mat);
        public event FInvalidate PaintCallbacks ;//= new FInvalidate;

        public enum ORIGIN_METHOD : uint
        {
            LeftTop      = 0,
            RightTop     = 1,
            LeftBottom   = 2,
            RightBottom  = 3
        }
        public enum USE_METHOD : uint
        {
            NotUse       = 0,
            Use          = 1
        }

        public class Common
        {
            public class Para
            {
                int xCnt, yCnt;
                [CategoryAttribute("Common"), DisplayNameAttribute("X(count)"            ),DescriptionAttribute("0 ~ 999 Count \nColumn Count")]
                    public  int XCnt  {//get;set;}
                        get {
                            if (xCnt > 999) xCnt = 999 ;
                            if (xCnt <   0) xCnt =   0 ;
                            return xCnt;
                        }
                        set {
                            xCnt = value;
                        }
                    }
                [CategoryAttribute("Common"), DisplayNameAttribute("Y(count)"            ),DescriptionAttribute("0 ~ 999 Count \nRow Count")]
                    public int           YCnt      {//get;set;}
                        get {
                            if (yCnt > 999) yCnt = 999 ;
                            if (yCnt <   0) yCnt =   0 ;
                            return yCnt;
                        }
                        set {
                            yCnt = value;
                        }
                    }
            }
            public Para para = new Para();

            public CTracker Tracker = new CTracker();
            public bool bTrackerDisplay ;
        }

        public class Homography
        {
            public class Para
            {
                int    width,height,xPos,yPos;
                double low,high;
                [CategoryAttribute("Homography"), DisplayNameAttribute("Whether to Use"      ),DescriptionAttribute("Use, Not Use \n(If used, the result is used as the initial calibration image)")] public USE_METHOD    Use       {get;set;} 
                [CategoryAttribute("Homography"), DisplayNameAttribute("Width"               ),DescriptionAttribute("Point to Point Width(1 ~ 1000 Pixel)" )]
                    public int           Width     {//get;set;} 
                        get {
                            if (width > 1000) width = 1000 ;
                            if (width <    0) width =    1 ;
                            return width;
                        }
                        set {
                            width = value;
                        }
                    }
                [CategoryAttribute("Homography"), DisplayNameAttribute("Height"              ),DescriptionAttribute("Point to Point Height(1 ~ 1000 Pixel)")]
                    public int           Height    {//get;set;} 
                        get {
                            if (height > 1000) height = 1000 ;
                            if (height <    1) height =    1 ;
                            return height;
                        }
                        set {
                            height = value;
                        }
                    }
                [CategoryAttribute("Homography"), DisplayNameAttribute("Origin Method"       ),DescriptionAttribute("Start Location"              )] public ORIGIN_METHOD Origin    {get;set;} 
                [CategoryAttribute("Homography"), DisplayNameAttribute("X Position"          ),DescriptionAttribute("Start Location Offset X (0 ~ 100 Pixel)")]
                    public int           XPos      {//get;set;} 
                        get {
                            if (xPos > 100) xPos = 100 ;
                            if (xPos <   0) xPos =   0 ;
                            return xPos;
                        }
                        set {
                            xPos = value;
                        }
                    }
                [CategoryAttribute("Homography"), DisplayNameAttribute("Y Position"          ),DescriptionAttribute("Start Location Offset Y (0 ~ 100 Pixel)")]
                    public int           YPos      {//get;set;} 
                        get {
                            if (yPos > 100) yPos = 100 ;
                            if (yPos <   0) yPos =   0 ;
                            return yPos;
                        }
                        set {
                            yPos = value;
                        }
                    }
                [CategoryAttribute("Homography"), DisplayNameAttribute("Low boundary value"  ),DescriptionAttribute("Canny Edge Low Value (0 ~ 255)"        )]
                    public double        Low       {//get;set;} 
                        get {
                            if (low > 255) low = 255 ;
                            if (low <   0) low =   0 ;
                            return low;
                        }
                        set {
                            low = value;
                        }
                    }
                [CategoryAttribute("Homography"), DisplayNameAttribute("High boundary value" ),DescriptionAttribute("Canny Edge High Value (0 ~ 255)"       )]
                    public double        High      {//get;set;} 
                        get {
                            if (high > 255) high = 255 ;
                            if (high <   0) high =   0 ;
                            return high;
                        }
                        set {
                            high = value;
                        }
                    }
                [CategoryAttribute("Homography"), DisplayNameAttribute("Image Path         " ),DescriptionAttribute("Image Location"              )] public string        Path      {get;set;} 
            }
            public Para para = new Para();

            public Mat ori  = new Mat();
            public Mat rst1 = new Mat();
            public Mat rst2 = new Mat();

            public Mat findHomography; //결과값
            public CTracker Tracker = new CTracker();
        }  

        public class Calibrate
        {
            public class Para
            {
                double low,high;
                [CategoryAttribute("Calibration"), DisplayNameAttribute("Whether to Use"      ),DescriptionAttribute("Use, Not Use"                )] public USE_METHOD    Use       {get;set;} 
                [CategoryAttribute("Calibration"), DisplayNameAttribute("Low boundary value"  ),DescriptionAttribute("Canny Edge Low Value"        )] public double        Low       {get;set;} 
                [CategoryAttribute("Calibration"), DisplayNameAttribute("High boundary value" ),DescriptionAttribute("Canny Edge High Value"       )] public double        High      {get;set;} 
                [CategoryAttribute("Calibration"), DisplayNameAttribute("Image Path         " ),DescriptionAttribute("Image Location"              )] public string        Path      {get;set;} 
            }
            public Para para = new Para();

            public Mat ori  = new Mat();
            public Mat rst1 = new Mat();
            public Mat rst2 = new Mat();

            public readonly Mat cameraMatrix = new Mat(3, 3, Emgu.CV.CvEnum.DepthType.Cv64F, 1);
            public readonly Mat distCoeffs   = new Mat(8, 1, Emgu.CV.CvEnum.DepthType.Cv64F, 1);
            public Mat Map1 = new Mat();
            public Mat Map2 = new Mat();
            public Mat[] rvecs, tvecs;

            public CTracker Tracker = new CTracker();
        }  

        public Common      common      = new Common     ();
        public Homography  homography  = new Homography ();
        public Calibrate   calibrate   = new Calibrate  ();

        public Mat display = new Mat();
        public string sErr = "";
        public int id ;

        public Calibration(int id)
        {
            homography.Tracker.Init();
            calibrate.Tracker.Init();
            common.Tracker.Init();

            this.id  = id ;
            Load();
        }

        public void InitPaint(int iNo = 0)
        {
            if(iNo == 0) PaintCallbacks(homography.ori);
            else         PaintCallbacks(calibrate.ori); 
        }
        
        public PointF[] FindFitEllipse(Mat original, ref Mat result, CTracker Tracker)
        {
            sErr = "";
            result = original.Clone();

            Mat rst = new Mat();            

            Rectangle trkRect = Tracker.GetRectangle();
            Mat Img = new Mat(original , trkRect);

            CvInvoke.CvtColor(Img, rst, ColorConversion.Bgr2Gray);        
            CvInvoke.Canny(rst, rst, homography.para.Low, homography.para.High);

            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(rst, contours, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);
            //CvInvoke.FindContours(rst, contours, null, RetrType.Tree, ChainApproxMethod.ChainApproxSimple);

            //MCvScalar mcvs = new MCvScalar(0,255,0);
            //CvInvoke.DrawContours(rst,contours,-1,mcvs);
            List<PointF> lst1 = new List<PointF>();
            
            for (int i = 0; i < contours.Size; i++)
            {
                if(contours[i].Size < 5) continue;
                RotatedRect rt = CvInvoke.FitEllipse(contours[i]);
                if(rt.Size.Width < 3|| rt.Size.Height <3) continue;
                float fwh = rt.Size.Width > rt.Size.Height ? rt.Size.Width : rt.Size.Height ;
                PointF plefttop      = new PointF(rt.Center.X - fwh/2f , rt.Center.Y - fwh/2f);
                PointF prighttop     = new PointF(rt.Center.X + fwh/2f , rt.Center.Y - fwh/2f);
                PointF prightbottom  = new PointF(rt.Center.X + fwh/2f , rt.Center.Y + fwh/2f);
                PointF pleftbottom   = new PointF(rt.Center.X - fwh/2f , rt.Center.Y + fwh/2f);
                bool   blefttop      = (plefttop.X     >= 0             && plefttop.Y     >= 0             );
                bool   brighttop     = (prighttop.X    <  trkRect.Width && prighttop.Y    >= 0             );
                bool   brightbottom  = (prightbottom.X <  trkRect.Width && prightbottom.Y <  trkRect.Height);
                bool   bleftbottom   = (pleftbottom.X  >= 0             && pleftbottom.Y  <  trkRect.Height);
                if(blefttop && brighttop && brightbottom && bleftbottom)
                {
                    lst1.Add(new PointF(trkRect.Left + rt.Center.X, trkRect.Top + rt.Center.Y));
                }
                else
                {

                }
                
            }

            //Fail
            if(lst1.Count != common.para.XCnt*common.para.YCnt) return null;

            lst1.Sort(delegate(PointF f1, PointF f2){
                if(f1.Y > f2.Y) return 1;
                else if(f1.Y < f2.Y) return -1;
                return 0;
            });

            int lstCount =0;
            int XCnt     =1;
            List<List<PointF>> lst2 = new List<List<PointF>>();
            if(lst1.Count > 0) { lst2.Add(new List<PointF>()); lst2[0].Add(lst1[0]); }
            for(int i=1; i< lst1.Count ; i++)
            {
                if(XCnt < common.para.XCnt) {
                    lst2[lstCount].Add(lst1[i]);
                    XCnt++;
                }
                else
                {
                    XCnt=1;
                    lstCount++;
                    lst2.Add(new List<PointF>());
                    lst2[lstCount].Add(lst1[i]);
                }
            }

            for(int i=0; i< lst2.Count ; i++)
            {
                lst2[i].Sort(delegate(PointF f1, PointF f2){
                    if(f1.X > f2.X) return 1;
                    else if(f1.X < f2.X) return -1;
                    return 0;
                });
            }

            
            
            PointF[] pf = new PointF[lst1.Count];
            lst1.Clear(); lstCount = 0;
            for(int i=0; i< lst2.Count ; i++)
            {
                for(int j=0; j< lst2[i].Count ; j++)
                {
                    lst1.Add(lst2[i][j]);
                    pf[lstCount] = new PointF(lst2[i][j].X,lst2[i][j].Y);
                    lstCount++;
                }
            }

            //Find Circle Display
            int iNo = 0;
            foreach (var i in lst1)
            {
                int x = (int)i.X ;//+ trkRect.Left;
                int y = (int)i.Y ;//+ trkRect.Top ;

                Point pt1 = new Point(x-3,y  );
                Point pt2 = new Point(x+3,y  );
                Point pt3 = new Point(x  ,y-3);
                Point pt4 = new Point(x  ,y+3);
                CvInvoke.Line(result,pt1,pt2,new MCvScalar(0, 0, 255),1);
                CvInvoke.Line(result,pt3,pt4,new MCvScalar(0, 0, 255),1);
            
                Point pt5 = new Point((int)i.X +5 ,(int)i.Y+5);
                var textOrigin = pt5;
                string msg = iNo.ToString();
                iNo++;
                CvInvoke.PutText(result, msg, textOrigin, FontFace.HersheyPlain, 1, new MCvScalar(0, 0, 255), 1);                
            }

            return pf;
        }

        public bool FindHomography(Mat mat)
        {
            PointF[] pf = FindFitEllipse(mat, ref homography.rst1 , common.Tracker);
            if(pf == null) {
                sErr = "원 검사 실패(설정 갯수와 검사 갯수 불일치)";
                return false;
            }
            //WarpPerspective
            float fw = (common.para.XCnt-1) * homography.para.Width ;
            float fh = (common.para.YCnt-1) * homography.para.Height;
            int   ix = (int)common.para.XCnt;
            int   iy = (int)common.para.YCnt;
            PointF   porigin = new PointF();
            PointF[] pobject = new PointF[common.para.XCnt*common.para.YCnt];
            //if(homography.para.Origin == ORIGIN_METHOD.LeftTop    ) porigin = new PointF(lst2[0   ][0   ].X     ,lst2[0   ][0   ].Y     );
            //if(homography.para.Origin == ORIGIN_METHOD.RightTop   ) porigin = new PointF(lst2[0   ][ix-1].X - fw,lst2[0   ][ix-1].Y     );
            //if(homography.para.Origin == ORIGIN_METHOD.LeftBottom ) porigin = new PointF(lst2[iy-1][0   ].X     ,lst2[iy-1][0   ].Y - fh);
            //if(homography.para.Origin == ORIGIN_METHOD.RightBottom) porigin = new PointF(lst2[iy-1][ix-1].X - fw,lst2[iy-1][ix-1].Y - fh);
            if(homography.para.Origin == ORIGIN_METHOD.LeftTop    ) porigin = new PointF(pf[0             ].X     ,pf[0             ].Y     );
            if(homography.para.Origin == ORIGIN_METHOD.RightTop   ) porigin = new PointF(pf[ix-1          ].X - fw,pf[ix-1          ].Y     );
            if(homography.para.Origin == ORIGIN_METHOD.LeftBottom ) porigin = new PointF(pf[(iy-1)*(ix)     ].X     ,pf[(iy-1)*(ix)     ].Y - fh);
            if(homography.para.Origin == ORIGIN_METHOD.RightBottom) porigin = new PointF(pf[(iy-1)*(ix)+ix-1].X - fw,pf[(iy-1)*(ix)+ix-1].Y - fh);
            
            porigin.X += homography.para.XPos ;
            porigin.Y += homography.para.YPos ;

            fw = homography.para.Width ;
            fh = homography.para.Height;
            int lstCount = 0;
            for (int i = 0; i < common.para.YCnt; i++)
            {
                for (int j = 0; j < common.para.XCnt; j++)
                {
                    pobject[lstCount] = new PointF(porigin.X + j * fw, porigin.Y + i * fh); lstCount++;
                }
            }

            //Mat warpPerspective = new Mat();
            homography.findHomography = CvInvoke.FindHomography(pf,pobject,HomographyMethod.Ransac);
            //CvInvoke.WarpPerspective(mat,warpPerspective,homography.findHomography,mat.Size);
            //GetWarpMat(mat,warpPerspective);
            WarpPerspective(mat,homography.rst2);

            //homography.rst2 = warpPerspective.Clone();
            
            
            //Test
            //homography.rst2 = homography.rst1 ;




            //this.display = homography.rst1.Clone() ;
            PaintCallbacks(homography.rst1);

            return true;
        }

        public void WarpPerspective(Mat Input , Mat Output)
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            CvInvoke.WarpPerspective(Input, Output, homography.findHomography, Input.Size);
            //sw.Stop();
            //Debug.WriteLine(sw.ElapsedMilliseconds);
        }

        public PointF[] PerspectiveTransform(PointF[] Input)
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            PointF[] output = CvInvoke.PerspectiveTransform(Input, homography.findHomography);
            //sw.Stop();
            //Debug.WriteLine(sw.ElapsedMilliseconds);
            return output;

        }

        public bool FindCalibrate(Mat mat)
        {
            PointF[] pf = FindFitEllipse(mat, ref calibrate.rst1 , common.Tracker);
            if(pf == null) {
                sErr = "원 검사 실패(설정 갯수와 검사 갯수 불일치)";
                return false;
            }

            PointF[][] point_list = new PointF[1][];
            point_list[0] = pf.ToArray();

            //Object
            List<MCvPoint3D32f> object_list = new List<MCvPoint3D32f>();
            for (int i = 0; i < common.para.YCnt; i++)
            {
                for (int j = 0; j < common.para.XCnt; j++)
                {
                    object_list.Add(new MCvPoint3D32f(j*20.0F, i*20.0F, 0.0F));
                }
            }
            MCvPoint3D32f[][] object_point = new MCvPoint3D32f[1][];
            object_point[0] = object_list.ToArray();

            //Calibration
            double error = CvInvoke.CalibrateCamera(object_point, 
                                                    point_list, 
                                                    calibrate.ori.Size,
                                                    calibrate.cameraMatrix, 
                                                    calibrate.distCoeffs, 
                                                    CalibType.RationalModel, 
                                                    new MCvTermCriteria(30,0.1), 
                                                    out calibrate.rvecs, 
                                                    out calibrate.tvecs);

            //CvInvoke.Undistort(calibration.ori, calibration.rst2, calibration.cameraMatrix, calibration.distCoeffs);
            CvInvoke.InitUndistortRectifyMap(calibrate.cameraMatrix,calibrate.distCoeffs,null,calibrate.cameraMatrix,calibrate.ori.Size,DepthType.Cv32F,calibrate.Map1,calibrate.Map2);
            //CvInvoke.Remap(calibration.ori,calibration.rst2,map1,map2,Inter.Linear);
            Undistort(calibrate.ori,ref calibrate.rst2);

            //MCvPoint3D32f[] object_point1 = new MCvPoint3D32f[1];
            //object_point1 = object_list.ToArray(); 
            //object_point1[0].X = 10;
            //object_point1[0].Y = 10;
            //object_point1[0].Z = 0;
            //Mat r = calibration.rvecs[0];
            //Mat t = calibration.tvecs[0];
            
            //bool b1 = CvInvoke.CheckRange(calibration.cameraMatrix);
            //PointF[] pfCal ;
            //pfCal = CvInvoke.ProjectPoints(object_point1,r,t,calibration.cameraMatrix,calibration.distCoeffs);
            //Undistort(calibration.ori, calibration.rst2);
            //CvInvoke.ProjectPoints()
            
            //public PointF[] Undistort(PointF[] src, Matrix<double> R = null, Matrix<double> P = null)

            //VectorOfPointF vop = new VectorOfPointF();
            //PointF[] pf1 = new PointF[1];
            //pf1[0].X = 89 ;
            //pf1[0].Y = 96 ;

            //PointF[] pf2 = UndistortPoints(pf1);
            //vop.Push(pf1);
            //vop[0] = new VectorOfPoint();
            //vop[0].Push(new PointF[](10,10));
            //VectorOfPointF vop2 = new VectorOfPointF();
            //
            //var v1 = GetValue(map1,89,96); //79
            //var v2 = GetValue(map2,89,96);
            //
            //CvInvoke.UndistortPoints(vop,vop2,calibration.cameraMatrix,calibration.distCoeffs);
            //FormCalibration
            //this.display = calibrate.rst1.Clone();
            PaintCallbacks(calibrate.rst1);

            return true;
        }

        public PointF[] UndistortPoints(PointF[] src, Matrix<double> R = null, Matrix<double> P = null)
        {
            PointF[] dst = new PointF[src.Length]; 
            GCHandle srcHandle = GCHandle.Alloc(src, GCHandleType.Pinned);
            GCHandle dstHandle = GCHandle.Alloc(dst, GCHandleType.Pinned);
            using (Matrix<float> srcPointMatrix = new Matrix<float>(src.Length, 1, 2, srcHandle.AddrOfPinnedObject(), 2 * sizeof(float)))
            using (Matrix<float> dstPointMatrix = new Matrix<float>(dst.Length, 1, 2, dstHandle.AddrOfPinnedObject(), 2 * sizeof(float)))
            {
               CvInvoke.UndistortPoints(srcPointMatrix, dstPointMatrix,calibrate.cameraMatrix,
                   calibrate.distCoeffs,
                   R,
                   calibrate.cameraMatrix);
            }
            srcHandle.Free();
            dstHandle.Free();
            return dst;
        }

        public void Undistort(Mat Input , ref Mat Output)
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            //CvInvoke.Undistort(Input,Output,calibration.cameraMatrix,calibration.distCoeffs);
            if(calibrate.Map1.Size.Width > 0 && calibrate.Map2.Size.Width > 0)
            CvInvoke.Remap(Input,Output,calibrate.Map1,calibrate.Map2,Inter.Linear);
            //sw.Stop();
            //Debug.WriteLine(sw.ElapsedMilliseconds);


            //return Output;

        }


        public bool Load(bool _bLoad = true, string _sPath = "")
        {
            //string FilePath = _sPath +"Calibration.ini";
            string sCommon              = System.AppDomain.CurrentDomain.BaseDirectory + "VisnUtil\\Calibration" + id.ToString() + "\\Common.xml"             ;
            string sHomography          = System.AppDomain.CurrentDomain.BaseDirectory + "VisnUtil\\Calibration" + id.ToString() + "\\Homography.xml"         ;
            string sCalibrate           = System.AppDomain.CurrentDomain.BaseDirectory + "VisnUtil\\Calibration" + id.ToString() + "\\Calibrate.xml"          ;
            string sTracker_Homography  = System.AppDomain.CurrentDomain.BaseDirectory + "VisnUtil\\Calibration" + id.ToString() + "\\Tracker_Homography.xml" ;
            string sTracker_Calibrate   = System.AppDomain.CurrentDomain.BaseDirectory + "VisnUtil\\Calibration" + id.ToString() + "\\Tracker_Calibrate.xml"  ;
            string sHomographyPara      = System.AppDomain.CurrentDomain.BaseDirectory + "VisnUtil\\Calibration" + id.ToString() + "\\FindHomography.xml"     ;
            string sCameraMatrix        = System.AppDomain.CurrentDomain.BaseDirectory + "VisnUtil\\Calibration" + id.ToString() + "\\CameraMatrix.xml"       ;
            string sDistCoeffs          = System.AppDomain.CurrentDomain.BaseDirectory + "VisnUtil\\Calibration" + id.ToString() + "\\DistCoeffs.xml"         ;
            
                
            //object oParaMotrSub = Para ;
            if (_bLoad)
            {
                //Para
                CXml.LoadXml(sCommon     , ref common.para     ) ;
                CXml.LoadXml(sHomography , ref homography.para ) ;
                CXml.LoadXml(sCalibrate  , ref calibrate.para  ) ;
                //if (!CXml.LoadXml(sHomographyPara, ref homography.findHomography)) { return false; }
                                
                //Tracker
                homography.Tracker.LoadSave (true,sTracker_Homography );
                calibrate.Tracker.LoadSave(true,sTracker_Calibrate);
                common.Tracker = homography.Tracker ;

                //Mat
                ReadMat(homography.findHomography,sHomographyPara,"findHomography");
                ReadMat(calibrate.cameraMatrix ,sCameraMatrix  ,"cameraMatrix"  );
                ReadMat(calibrate.distCoeffs   ,sDistCoeffs    ,"distCoeffs"    );

                if(File.Exists(homography.para.Path )) homography.ori  = new Mat(homography.para.Path ); 
                if(File.Exists(calibrate.para.Path  )) calibrate.ori = new Mat(calibrate.para.Path); 
                //display = homography.ori;  
                //PaintCallbacks(homography.ori);

            }
            else
            {
                //Para
                CXml.SaveXml(sCommon     , ref common.para     ) ;
                CXml.SaveXml(sHomography , ref homography.para ) ;
                CXml.SaveXml(sCalibrate  , ref calibrate.para  ) ;
                //if (!CXml.SaveXml(sHomographyPara, ref homography.findHomography)) { return false; }

                //Tracker
                homography.Tracker.LoadSave (false,sTracker_Homography );
                calibrate.Tracker.LoadSave(false,sTracker_Calibrate);

                //Mat
                SaveMat(homography.findHomography,sHomographyPara,"findHomography");
                SaveMat(calibrate.cameraMatrix   ,sCameraMatrix  ,"cameraMatrix"  );
                SaveMat(calibrate.distCoeffs     ,sDistCoeffs    ,"distCoeffs"    );

            }

            return true ;
        }

        private void SaveMat(Mat mat, string file, string node)
        {
            if(!File.Exists(file)) return;

            var fs = new FileStorage(file, FileStorage.Mode.Write);
            fs.Write(mat, node);
        }

        private Mat ReadMat(Mat mat, string file, string node)
        {
            if(!File.Exists(file)) return null;

            mat = new Mat();
            try
            {
                var fs = new FileStorage(file, FileStorage.Mode.Read);
                fs.GetNode(node).ReadMat(mat);
            }
            catch
            {

            }

            return mat;
        }
    
        public PointF[] GetPoints(PointF[] src)
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            PointF[] dst = new PointF[src.Length]; 
            if(homography.para.Use == USE_METHOD.Use) { dst = PerspectiveTransform(src); }
            if(calibrate.para.Use  == USE_METHOD.Use) { dst = UndistortPoints(dst);      }
            //sw.Stop();
            //Debug.WriteLine("GetPoints"+sw.ElapsedMilliseconds);
            return dst;
        }

        public void ReMap(Mat Input , ref Mat Output)
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            //CvInvoke.Undistort(Input,Output,calibration.cameraMatrix,calibration.distCoeffs);
            if(homography.para.Use  == USE_METHOD.Use) WarpPerspective(Input,Output);
            if(calibrate.para.Use   == USE_METHOD.Use) CvInvoke.Remap(Input,Output,calibrate.Map1,calibrate.Map2,Inter.Linear);
            //sw.Stop();
            //Debug.WriteLine("ReMap"+sw.ElapsedMilliseconds);


            //return Output;

        }
    }
}
