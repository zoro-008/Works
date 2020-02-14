using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using System.ComponentModel;
using System.Drawing;

namespace VDll.Algorithm
{
    class CEdge
    {
        [Serializable ,TypeConverter(typeof(ExpandableObjectConverter))]
        public class CUPara
        {                                                  
            //[CategoryAttribute("UPara"), DisplayNameAttribute("Raw"            )] public uint     Raw            {get;set;}        
            //[CategoryAttribute("UPara"), DisplayNameAttribute("Smoothed Data"  )] public uint     SmoothedData   {get;set;}      
            //[CategoryAttribute("UPara"), DisplayNameAttribute("Difference Data")] public uint     DifferenceData {get;set;}      
        }

        public struct TRslt
        {
            public int    RisingX  ;
            public int    RisingY  ;
            public int    FallingX ;
            public int    FallingY ;
            public string sError   ;
        }

        static public int iPxCnt = 0;//배열 크기 지정해줘야해서 만든다.

        //static public int[] rawData        = new int[iPxCnt];
        //static public int[] smoothedData   = new int[iPxCnt];
        //static public int[] differenceData = new int[iPxCnt];

        static public List<int> rawData        = new List<int>();
        static public List<int> smoothedData   = new List<int>();
        static public List<int> differenceData = new List<int>();

        static public int iRawDataCnt = 0;//리스트뷰에 픽셀값 띄워볼라고 잠깐 쓴다.

        public int GetPixelGray(Mat mat, int x, int y)//, int cols, int elementSize)
        {
            byte gray = 0;

            int c = mat.Cols;
            int e = mat.ElementSize;
            unsafe
            {
                byte* Data = (byte*)mat.DataPointer;
                
                gray = *(Data + (y * c + x) * e);
            }
            return gray;
        }

        static public bool FindEdge (Mat _mtImage , int _iStartX , int _iEndX , int _iStartY , int _iEndY , CUPara _upPara , ref TRslt _tRslt)
        {
            _tRslt.RisingX  = 0 ;
            _tRslt.RisingY  = 0 ;
            _tRslt.FallingX = 0 ;
            _tRslt.FallingY = 0 ;
            rawData       .Clear();
            smoothedData  .Clear();
            differenceData.Clear();
            iRawDataCnt = 0;
            int stride              = _mtImage.Step ;
            int CrntPx              = 0 ; //현재 픽셀 밝기.
            int e = _mtImage.ElementSize;
            int iMaxDif = 0;
            int iMinDif = 256;
            int iSum = 0; //smoothed data 추가 전 rawdata 3개 더한 값을 여기에 넣는다.
            int iSumCount = 0;//raw data 더한 갯수 카운팅하여 평균 나눌때 분모로 쓴다.
            int iAvr = 0; //iSum 변수를 나누기3한 값을 여기에 넣는다.
            int iDif = 0;

            Point StartPoint = new Point();
            Point EndPoint   = new Point();
            StartPoint.X = _iStartX; StartPoint.Y = _iStartY;
            EndPoint  .X = _iEndX  ; EndPoint  .Y = _iEndY  ;

            bool IsPat = Math.Abs(EndPoint.X - StartPoint.X) >= Math.Abs(EndPoint.Y - StartPoint.Y);
            
            //Start -> End 방향으로 트랙커 위치시 문제 없는데 End -> Start 방향일때 Start에서 증가시키기 때문에 트랙커 방향과 반대로 픽셀값을 읽어서 뒤집어야함
            Point Temp;
            if (EndPoint.X < StartPoint.X)
            { 
                Temp       = EndPoint  ;
                EndPoint   = StartPoint;
                StartPoint = Temp      ;
            }

            int StartX  = StartPoint.X;
            int EndX    = EndPoint  .X;
            int StartY  = StartPoint.Y;
            int EndY    = EndPoint  .Y;
            
            //Set Fomula of Line.기울기
            double a;
            double b;
            if (EndX - StartX > 0) a = (EndY - StartY) / (double)(EndX - StartX);
            else                       a = (EndY - StartY) / 0.0000000000001;
            b = StartY - a * StartX;

            unsafe
            {
                CrntPx = 0;
                byte* Data = (byte *)_mtImage.DataPointer;

                for(int i = StartX; i <= EndX; i++)
                {
                    int y = (int)(a * i + b);//직선방정식
                    CrntPx = *(Data + (y * stride) + i) ; //*(Data + (row * mat.Cols + col)
                    rawData.Add(CrntPx);
                    iRawDataCnt++;
                }

                //3개씩 묶어서 평균 내면 1칸씩 땡겨져서 처음 2픽셀만 따로 평균 내고 이후부터 3픽셀씩 평균 낸다.
                iSum = rawData[0] + rawData[1];
                iAvr = iSum/2;
                smoothedData.Add(iAvr);

                for(int j = 0; j < rawData.Count; j++)
                {
                    iSum = 0;
                    iSumCount = 0;
                    iAvr = 0;
                    for(int k = j; k < j + 3; k++)
                    {
                        if(k >= rawData.Count) break;
                        iSum += rawData[k];
                        iSumCount++;
                    }
                    iAvr = iSum/iSumCount;
                    smoothedData.Add(iAvr);
                }

                for(int l = 0; l < smoothedData.Count; l++)
                {
                    if(l+2 >= smoothedData.Count) 
                    {
                        differenceData.Add(0);
                        continue;
                    }
                    iDif = smoothedData[l+2] - smoothedData[l];
                    differenceData.Add(iDif);

                    if (iMaxDif < differenceData[l])
                    {
                        iMaxDif = differenceData[l]; 
                        _tRslt.RisingX = StartX + l;
                        _tRslt.RisingY = (int)(a * _tRslt.RisingX + b);//직선방정식
                    }
                    if (iMinDif > differenceData[l]) 
                    {
                        iMinDif = differenceData[l];
                        _tRslt.FallingX = StartX + l;
                        _tRslt.FallingY = (int)(a * _tRslt.FallingX + b);//직선방정식
                    }
                }

                
                
               
            }
            return true ;
        }
    }
}
