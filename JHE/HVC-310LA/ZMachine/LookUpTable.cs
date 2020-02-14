using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Machine
{
    public static class LookUpTable
    {
        public static double GetLookUpTableLeftX(double _dDegree)
        {
            double dRet = 0.0;

            if (_dDegree < 0) _dDegree += 360;

            //편심보정 한눈금 분해능.
            double dDegreeRes = 360 / (double)OM.MAX_TABLE;
            
            //편심보정의 마스터 보정치 인덱스.
            int iIndexMst = (int)(_dDegree / dDegreeRes) ;

            //마스터 보정치 빼고 비율뽑아야하는 양.
            double dDegreeSub = _dDegree - iIndexMst * dDegreeRes ;
            double dVal1 = OM.LeftTable[iIndexMst].dX;
            double dVal2 = (iIndexMst+1) < OM.MAX_TABLE ? OM.LeftTable[iIndexMst+1].dX : OM.LeftTable[0].dX ;

            //
            double dVal1Ratio = (dDegreeRes - dDegreeSub) /dDegreeRes;
            double dVal2Ratio = dDegreeSub / dDegreeRes;



            dRet = dVal1Ratio * dVal1 + dVal2Ratio * dVal2;


            return dRet;
        }

        public static double GetLookUpTableLeftY(double _dDegree)
        {
            double dRet = 0.0;

            if (_dDegree < 0) _dDegree += 360;

            //편심보정 한눈금 분해능.
            double dDegreeRes = 360 / (double)OM.MAX_TABLE;
            
            //편심보정의 마스터 보정치 인덱스.
            int iIndexMst = (int)(_dDegree / dDegreeRes) ;

            //마스터 보정치 빼고 비율뽑아야하는 양.
            double dDegreeSub = _dDegree - iIndexMst * dDegreeRes ;
            double dVal1 = OM.LeftTable[iIndexMst].dY;
            double dVal2 = (iIndexMst+1) < OM.MAX_TABLE ? OM.LeftTable[iIndexMst+1].dY : OM.LeftTable[0].dY ;

            //
            double dVal1Ratio = (dDegreeRes - dDegreeSub) /dDegreeRes;
            double dVal2Ratio = dDegreeSub / dDegreeRes;

            dRet = dVal1Ratio * dVal1 + dVal2Ratio * dVal2;

            return dRet;
        }

        public static double GetLookUpTableLeftT(double _dDegree)
        {
            double dRet = 0.0;

            if (_dDegree < 0) _dDegree += 360;

            //편심보정 한눈금 분해능.
            double dDegreeRes = 360 / (double)OM.MAX_TABLE;
            
            //편심보정의 마스터 보정치 인덱스.
            int iIndexMst = (int)(_dDegree / dDegreeRes) ;

            //마스터 보정치 빼고 비율뽑아야하는 양.
            double dDegreeSub = _dDegree - iIndexMst * dDegreeRes ;
            double dVal1 = OM.LeftTable[iIndexMst].dT;
            double dVal2 = (iIndexMst+1) < OM.MAX_TABLE ? OM.LeftTable[iIndexMst+1].dT : OM.LeftTable[0].dT ;

            //
            double dVal1Ratio = (dDegreeRes - dDegreeSub) /dDegreeRes;
            double dVal2Ratio = dDegreeSub / dDegreeRes;

            dRet = dVal1Ratio * dVal1 + dVal2Ratio * dVal2;

            return dRet;
        }

        public static double GetLookUpTableRightX(double _dDegree)
        {
            double dRet = 0.0;


            if (_dDegree < 0) _dDegree += 360;

            //편심보정 한눈금 분해능.
            double dDegreeRes = 360 / (double)OM.MAX_TABLE;
            
            //편심보정의 마스터 보정치 인덱스.
            int iIndexMst = (int)(_dDegree / dDegreeRes) ;

            //마스터 보정치 빼고 비율뽑아야하는 양.
            double dDegreeSub = _dDegree - iIndexMst * dDegreeRes ;
            double dVal1 = OM.RightTable[iIndexMst].dX;
            double dVal2 = (iIndexMst+1) < OM.MAX_TABLE ? OM.RightTable[iIndexMst+1].dX : OM.RightTable[0].dX ;

            //
            double dVal1Ratio = (dDegreeRes - dDegreeSub) /dDegreeRes;
            double dVal2Ratio = dDegreeSub / dDegreeRes;



            dRet = dVal1Ratio * dVal1 + dVal2Ratio * dVal2;


            return dRet;
        }

        public static double GetLookUpTableRightY(double _dDegree)
        {
            double dRet = 0.0;

            if (_dDegree < 0) _dDegree += 360;

            //편심보정 한눈금 분해능.
            double dDegreeRes = 360 / (double)OM.MAX_TABLE;
            
            //편심보정의 마스터 보정치 인덱스.
            int iIndexMst = (int)(_dDegree / dDegreeRes) ;

            //마스터 보정치 빼고 비율뽑아야하는 양.
            double dDegreeSub = _dDegree - iIndexMst * dDegreeRes ;
            double dVal1 = OM.RightTable[iIndexMst].dY;
            double dVal2 = (iIndexMst+1) < OM.MAX_TABLE ? OM.RightTable[iIndexMst+1].dY : OM.RightTable[0].dY ;

            //
            double dVal1Ratio = (dDegreeRes - dDegreeSub) /dDegreeRes;
            double dVal2Ratio = dDegreeSub / dDegreeRes;

            dRet = dVal1Ratio * dVal1 + dVal2Ratio * dVal2;

            return dRet;
        }

        public static double GetLookUpTableRightT(double _dDegree)
        {
            double dRet = 0.0;

            if (_dDegree < 0) _dDegree += 360;

            //편심보정 한눈금 분해능.
            double dDegreeRes = 360 / (double)OM.MAX_TABLE;
            
            //편심보정의 마스터 보정치 인덱스.
            int iIndexMst = (int)(_dDegree / dDegreeRes) ;

            //마스터 보정치 빼고 비율뽑아야하는 양.
            double dDegreeSub = _dDegree - iIndexMst * dDegreeRes ;
            double dVal1 = OM.RightTable[iIndexMst].dT;
            double dVal2 = (iIndexMst+1) < OM.MAX_TABLE ? OM.RightTable[iIndexMst+1].dT : OM.RightTable[0].dT ;

            //
            double dVal1Ratio = (dDegreeRes - dDegreeSub) /dDegreeRes;
            double dVal2Ratio = dDegreeSub / dDegreeRes;

            dRet = dVal1Ratio * dVal1 + dVal2Ratio * dVal2;

            return dRet;
        }
    }
}
