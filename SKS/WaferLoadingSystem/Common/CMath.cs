using System;
using System.Drawing;

namespace COMMON
{
    public class CMath
    {
        static public double GetLineAngle(double x1, double y1, double x2, double y2)
        {
            double rad, deg;
            double dx, dy, dl;
            double dTemp ;
        
            dx = x2 - x1;
            dy = y2 - y1;
            dl = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
        
            if (dl==0) return 0.0; //직선의 길이가 0이라 판별 못함.
        
            dTemp = dy/dl ;
            if(dTemp < 0) dTemp = dTemp * -1 ;
            rad = Math.Asin(dTemp);
        
            if (dx >= 0 && dy >= 0) rad = Math.PI *2 - rad;
            if (dx  < 0 && dy >= 0) rad = Math.PI    + rad;
            if (dx  < 0 && dy  < 0) rad = Math.PI    - rad;
            //if (dx >= 0 && dy  < 0) rad =             rad;
        
            deg = (rad*180)/Math.PI;
        
            return deg;
        }
        
        //비전 좌표계와 같이 4사분면. CCW+
        static public void GetRotPnt(double _dT     , //대상물체의 각도
                                     double _dCntrX , //현재 모터위치에서 스테이지 센터에서의 모터위치 까지의 거리.
                                     double _dCntrY ,
                                     out double _dMoveX ,
                                     out double _dMoveY )
        {
            _dMoveX = 0;
            _dMoveY = 0; 
            double dRadT =  _dT*Math.PI/180.0;
            double dX = -_dCntrX;
            double dY = -_dCntrY;
            double dMoveX = dX*Math.Cos(dRadT) - dY*Math.Sin(dRadT) ;
            double dMoveY = dX*Math.Sin(dRadT) + dY*Math.Cos(dRadT) ;
            dMoveX += _dCntrX ;
            dMoveY += _dCntrY ;
            //4사분면일때.
            _dMoveX = -dMoveX ;
            _dMoveY = -dMoveY ;
        }

        //여기부터 나중에 추가한거 20180219 진섭
        static public bool GetCircleInPoint(double _dCntrX, double _dCntrY, double _dRadius, double _pdX, double _pdY)
        {
            double _dLeng = Math.Sqrt((_pdX - _dCntrX) * (_pdX - _dCntrX) + (_pdY - _dCntrY) * (_pdY - _dCntrY));

            if (_dLeng <= _dRadius) return true;

            return false;
        }

        static public bool IsPntInPolygon(Point[] _pPoints, int _iPntCnt, Point _tPnt)
        {

            Rectangle r = new Rectangle();
            Point[] ppt = new Point[_iPntCnt+1];
            int i;
            Point pt1, pt2;
            int wnumintsct = 0;

            if (!IsPntInPolygonOutRect(_pPoints, _iPntCnt, _tPnt)) return false; //이거 문제

            pt1 = pt2 = _tPnt;
            pt2.X = r.Right + 50;

            // Now go through each of the lines in the polygon and see if it
            // intersects
            for (i = 0; i < _iPntCnt - 1; i++)
            {
                ppt = _pPoints;
                if (GetSegmentIntersect(_tPnt, pt2, ppt[i], ppt[i + 1])) { wnumintsct++; }
            }

            // And the last line
            if (GetSegmentIntersect(_tPnt, pt2, ppt[0], _pPoints[i])) { wnumintsct++; }
                

            //return (wnumintsct & 1) ;
            if ((wnumintsct & 1) == 0) return false;
            else                       return true;

        }

        //시작점에서 부터 해당각도와 길이만큼 떨어진 점.
        static public void GetPntFromPntByAngLen(double _dX, double _dY, //입력 시작점.
                                   double _dAngle,         //입력 각도.
                                   double _dLength,         //입력 길이.
                                   ref double _pX, ref double _pY) //출력 포인트.
        {
            //double sss = cos(_dAngle*MATH_PI/180.0);
            _pX =  _dLength * Math.Cos(_dAngle * Math.PI / 180.0) + _dX;
            _pY = -_dLength * Math.Sin(_dAngle * Math.PI / 180.0) + _dY;
        }

        //3포인트를 순서대로 이동했을때 CCW방향이면 1, 아니면 -1
        static public int GetPntCCW(Point _tPnt1, Point _tPnt2, Point _tPnt3)

        {

            double dx1, dx2;//long dx1, dx2 ;
            double dy1, dy2;//long dy1, dy2 ;

            dx1 = _tPnt2.X - _tPnt1.X; dx2 = _tPnt3.X - _tPnt1.X;
            dy1 = _tPnt2.Y - _tPnt1.Y; dy2 = _tPnt3.Y - _tPnt1.Y;

            /* This is basically a slope comparison: we don't do divisions because
        
             * of divide by zero possibilities with pure horizontal and pure
             * vertical lines.
             */
            double lTemp1 = dx1 * dy2;
            double lTemp2 = dy1 * dx2;
            int iRet = (lTemp1 > lTemp2) ? 1 : -1;

            return iRet;

            //return ((dx1 * dy2 > dy1 * dx2) ? 1 : -1);

        }

        //2개의 선분이 교차되면 true
        static public bool GetSegmentIntersect(Point _tPntStt1, Point _tPntEnd1, Point _tPntStt2, Point _tPntEnd2)
        {
            int iTemp1 = GetPntCCW(_tPntStt1, _tPntEnd1, _tPntStt2);
            int iTemp2 = GetPntCCW(_tPntStt1, _tPntEnd1, _tPntEnd2);
            int iTemp3 = GetPntCCW(_tPntStt2, _tPntEnd2, _tPntStt1);
            int iTemp4 = GetPntCCW(_tPntStt2, _tPntEnd2, _tPntEnd1);

            bool bRet = ((iTemp1 * iTemp2 <= 0) && (iTemp3 * iTemp4 <= 0));


            //bool bRet = (((GetPntCCW(_tPntStt1, _tPntEnd1, _tPntStt2) * GetPntCCW(_tPntStt1, _tPntEnd1, _tPntEnd2)) <= 0) &&
            //            ((GetPntCCW(_tPntStt2, _tPntEnd2, _tPntStt1) * GetPntCCW(_tPntStt2, _tPntEnd2, _tPntEnd1) <= 0)));
            return bRet;

        }

        //포인트가 구성하는 가장 작은 사각형 안에 해당 포인트가 있는지 확인.
        static public bool IsPntInPolygonOutRect(Point[] _pPoints, int _iPntCnt, Point _tPnt)
        {
            Rectangle tOutRect = new Rectangle();
            // If a bounding rect has not been passed in, calculate it

            int iXMin, iXMax, iYMin, iYMax;
            Point[] ppt = new Point[_iPntCnt];
            int i;

            iXMin = iYMin =  int.MaxValue;//int.MaxValue;//INT_MAX ;
            iXMax = iYMax = -int.MaxValue;//int.MinValue;//-INT_MAX ;

            for (i = 0; i < _iPntCnt; i++)
            {
                ppt[i] = _pPoints[i];
                if (ppt[i].X < iXMin) iXMin = ppt[i].X;
                if (ppt[i].X > iXMax) iXMax = ppt[i].X;
                if (ppt[i].Y < iYMin) iYMin = ppt[i].Y;
                if (ppt[i].Y > iYMax) iYMax = ppt[i].Y;
            }

            int iLeft, iTop, iRight, iBottom;

            iLeft   = iXMin;
            iTop    = iYMin;
            iRight  = iXMax;
            iBottom = iYMax;

            tOutRect.X      = iXMin;
            tOutRect.Y      = iYMin;
            tOutRect.Width  = iXMax - tOutRect.X;
            tOutRect.Height = iYMax - tOutRect.Y;

            return (tOutRect.Left <= _tPnt.X && _tPnt.X <= tOutRect.Right &&
                    tOutRect.Top <= _tPnt.Y && _tPnt.Y <= tOutRect.Bottom);
        }
    }    
}
