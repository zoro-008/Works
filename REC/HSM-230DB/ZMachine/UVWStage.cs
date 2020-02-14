using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SML2;

namespace Machine
{
    
    public enum UVW_ANG
    {
        AngCW0    = 0 ,
        AngCW90   = 1 ,
        AngCW180  = 2 ,
        AngCW270  = 3 
    }
    public struct TUVWPara
    {
        //모터 아이디는 MMT카다로그 상에서 그림에 맞게 넣어주고 
        //명령 내릴때 혹은 포지션 값 받아올때 스테이지 돌아가있는 각도로 계산하여 값을 알려준다.
        //카달로그상
        public UVW_ANG eStgAng ;

        public int iX1 ; //뒤쪽 X축 모터 카달로그 형태일때 왼쪽이 - 오른쪽 +
        public int iX2 ; //앞쪽 X축 모터 카달로그 형태일때 왼쪽이 - 오른쪽 +
        public int iY  ; //오른쪽 Y축 모터 카달로그 형태일때 뒷쪽이 - 앞쪽 +

        public double dX1Ang ; //X1에 연결되어 있는 Ang  정우측이 0도고 반시계방향이 +
        public double dX2Ang ; //X2에 연결되어 있는 Ang
        public double dYAng  ; //Y에  연결되어 있는 Ang

        public double dRotRad ; //각축에 연결되어 있는 베어링 회전 반지름.

    }

    //20170516 MMT 모델 AA-400-3S 사용.카달로그 도면에서 
    //eStgAng : 1 , dX1Ang :135 , dX2Ang : 225 , dYAbg : 45 , dRotRad : 212.13203435596425732
    //카달로그와 똑같은 AngCW0 옵션이라고 했을때.
    //오른쪽,전면,반시계방향 이 + 이다.
    //공식설명에서는 오른쪽,후면,반시계방향이 +로 나와있는데 실제 제품은 Y축이 반대이다. 그래서 공식에도 Y축이동량은 반전함.
    //즉 공식은 1사분면 실제품은 4사분면 사용한다.
    public class UVWStage
    {
        TUVWPara Para ;

        double m_dTrgX ;
        double m_dTrgY ;
        double m_dTrgT ;
        bool   m_bBusy ;

        public UVWStage(TUVWPara _tUVWPara) 
        {
            Para = _tUVWPara ;
        }

        public double GetTrgX(){return m_dTrgX;}
        public double GetTrgY(){return m_dTrgY;}
        public double GetTrgT(){return m_dTrgT;}


        public bool GoAbs(double _dX , double _dY , double _dT)
        {
            //double dRadT =  _dAng*3.14159265358979323846/180.0;
            //카달로그상의 좌표로 변환.
            m_dTrgX = _dX ;
            m_dTrgY = _dY ;
            m_dTrgT = _dT ;

            double dX , dY , dT ;

            double dMoveX1 ;
            double dMoveX2 ;
            double dMoveY  ;


            //다이어테치 적용 케이스
                 if (Para.eStgAng == UVW_ANG.AngCW90 ) {dX =  _dY ; dY = -_dX ; dT =  _dT ;}
            else if (Para.eStgAng == UVW_ANG.AngCW180) {dX = -_dX ; dY = -_dY ; dT =  _dT ;}
            else if (Para.eStgAng == UVW_ANG.AngCW270) {dX = -_dY ; dY =  _dX ; dT =  _dT ;}
            else                                       {dX =  _dX ; dY =  _dY ; dT =  _dT ;}
            
            //X,Y 이동.
            dMoveX1 = dX;
            dMoveX2 = dX;
            dMoveY  = dY;

            //여기에 T 더함.      double dbCosTheta21 = Math.Cos (21 * (2 * Math.PI / 360));          // 0.9335804
            const double dAngToRad =  (Math.PI / 180.0) ;
            dMoveX1 += Para.dRotRad * Math.Cos((dT+Para.dX1Ang)*dAngToRad) - Para.dRotRad * Math.Cos(Para.dX1Ang*dAngToRad) ;
            dMoveX2 += Para.dRotRad * Math.Cos((dT+Para.dX2Ang)*dAngToRad) - Para.dRotRad * Math.Cos(Para.dX2Ang*dAngToRad) ;
            dMoveY  += Para.dRotRad * Math.Sin((dT+Para.dYAng )*dAngToRad) - Para.dRotRad * Math.Sin(Para.dYAng *dAngToRad) ;

            //카달로그 공식과 실제품의 Y축 방향이 반대로 되어 있음.
            dMoveY = -dMoveY ;

            SML.MT.GoAbsRun(Para.iX1 , dMoveX1) ;
            SML.MT.GoAbsRun(Para.iX2 , dMoveX2) ;
            SML.MT.GoAbsRun(Para.iY  , dMoveY ) ;

            return true ;
        }

        public bool GoAbsXXY(double _dX1, double _dX2, double _dY)
        {
            SML.MT.GoAbsRun(Para.iX1, _dX1);
            SML.MT.GoAbsRun(Para.iX2, _dX2);
            SML.MT.GoAbsRun(Para.iY , _dY );

            return true;
        }


        public bool GoInc(double _dX , double _dY , double _dT)
        {
            return GoAbs(m_dTrgX + _dX , m_dTrgY + _dY , m_dTrgT + _dT) ;
        }

        public bool GetStop()
        {
            bool bRet = true ;
            bRet &= SML.MT.GetStop(Para.iX1);
            bRet &= SML.MT.GetStop(Para.iX2);
            bRet &= SML.MT.GetStop(Para.iY );
            return bRet ;
        }      

        public bool GoHome()
        {
            bool bRet = true ;

            bRet &= SML.MT.GoHome(Para.iX1);
            bRet &= SML.MT.GoHome(Para.iX2);
            bRet &= SML.MT.GoHome(Para.iY );

            m_dTrgX = 0 ;
            m_dTrgY = 0 ;
            m_dTrgT = 0 ;

            return bRet ;
        }

        public bool GetHomeDone()
        {
            bool bRet = true ;

            bRet &= SML.MT.GetHomeDone(Para.iX1);
            bRet &= SML.MT.GetHomeDone(Para.iX2);
            bRet &= SML.MT.GetHomeDone(Para.iY );

            return bRet ;
        }
    }
}
