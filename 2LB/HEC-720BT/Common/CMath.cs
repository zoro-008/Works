using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
