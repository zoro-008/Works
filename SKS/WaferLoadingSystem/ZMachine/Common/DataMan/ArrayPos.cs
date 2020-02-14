using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Machine
{
    public class ArrayPos
    {
        public struct TPara
        {
            //public WorkStartCorner eWorkStartConner ; //어디부터 작업 시작 하는지.
            //public double dWorkStartX ; //eWorkStartConner와 iWorkXCnt iWorkYCnt 가 감안된 워크 스타트포지션 
            //public double dWorkStartY ; //비전으로 생각하면 한번에 칩을 X3,Y2개 검사할때 X1.5 , Y1의 위치. 즉 6개의 칩의 가운데.
            
            

            //public int    iWorkXCnt   ; //한번 작업시에 커버하는 X카운트 작업을 1개씩 하면 1이라고 하면 됨.
            //public int    iWorkYCnt   ; //한번 작업시에 커버하는 Y카운트

            public int    iColCnt     ; //전체 X갯수.
            public int    iRowCnt     ; //전체 Y갯수.
            public double dColPitch   ; //기본 X피치
            public double dRowPitch   ; //기본 Y피치
            public int    iColGrCnt   ; //스트립에서 X 그룹의 갯수. 없을시는 0
            public int    iRowGrCnt   ; //스트립에서 Y 그풉의 갯수.
            public double dColGrGap   ; //1그룹마지막 자제 와 2그룹첫 자제 간의 피치.
            public double dRowGrGap   ; //1그룹마지막 자제 와 2그룹첫 자제 간의 피치.
            

            //딱히 필요는 없음...
            //public double dWidth      ; //한칩의 넓이.
            //public double dHeight     ; //한칩의 높이.

            //1그룹안에서 또 그룹이 나눠져 있을때 사용.===================================================
            public int    iColSbGrCnt ; //서브그룹이 있을때 1그룹안에 서부그룹이 총몇개 있는지..사용 안할시에는 iRowSubGroupCount==0
            public int    iRowSbGrCnt ; 

            public double dRowSbGrGap ; //서브그룹이 있을때 1번써브그룹 마지막 자제 시작지점과 2번써브그룹 첫번째 자제 시작지점간의 거리.
            public double dColSbGrGap ; 
        }
        TPara Para ;

        string sError ; public string Error {get{return sError; } }
        public ArrayPos()
        {
            
        }
        public bool SetPara(TPara _tPara)
        {
            
            //if(_tPara.iWorkXCnt < 1)
            //{
            //    sError = "iWorkXCnt have to be bigger than 0" ;
            //    return false ;
            //}
            //if(_tPara.iWorkYCnt < 1)
            //{
            //    sError = "iWorkYCnt have to be bigger than 0" ;
            //    return false ;
            //}
            if(_tPara.iColCnt < 1)
            {
                sError = "iColCnt have to be bigger than 0" ;
                return false ;
            }
            if(_tPara.iRowCnt < 1)
            {
                sError = "iRowCnt have to be bigger than 0" ;
                return false ;
            }
            if(_tPara.iColGrCnt >= _tPara.iColCnt)
            {
                sError = "iColGrCnt have to be smaller than iColCnt" ;
                return false ;
            }
            if(_tPara.iRowGrCnt >= _tPara.iRowCnt)
            {
                sError = "iRowGrCnt have to be smaller than iRowCnt" ;
                return false ;
            }
            if(_tPara.iColSbGrCnt!=0 && _tPara.iColGrCnt==0) //서브그룹카운트 설정전에 그룹카운트를 설정해야 함.
            {
                sError = "Set iColGrCnt first" ;
                return false ;
            }
            if(_tPara.iRowSbGrCnt!=0 && _tPara.iRowGrCnt==0)
            {
                sError = "Set iRowGrCnt first" ;
                return false ;
            }

            //그룹카운트 체크.
            int iColCntInGroup = _tPara.iColCnt ;
            if(_tPara.iColGrCnt!=0)
            { 
                if(iColCntInGroup % _tPara.iColGrCnt != 0)
                {
                    sError = "iColCnt divided by iColGrCnt have to be integer" ; //그룹카운트로 나눴을때 딱 떨어져야 함.
                    return false ;
                }
                iColCntInGroup =  iColCntInGroup / _tPara.iColGrCnt ;
            }
            if(_tPara.iColSbGrCnt!=0)
            {
                if(iColCntInGroup % _tPara.iColSbGrCnt != 0)
                {
                    sError = "iColCnt divided by iColGrCnt divided by iColCntInGroup have to be integer" ; //서브그룹카운트로 나눴을때 딱 떨어져야 함.
                    return false ;
                }
            }

            //그룹카운트 체크.
            int iRowCntInGroup = _tPara.iRowCnt ;
            if(_tPara.iRowGrCnt!=0)
            { 
                if(iRowCntInGroup % _tPara.iRowGrCnt != 0)
                {
                    sError = "iRowCnt divided by iRowGrCnt have to be integer" ; //그룹카운트로 나눴을때 딱 떨어져야 함.
                    return false ;
                }
                iRowCntInGroup =  iRowCntInGroup / _tPara.iRowGrCnt ;
            }
            if(_tPara.iRowSbGrCnt!=0)
            {
                if(iRowCntInGroup % _tPara.iRowSbGrCnt != 0)
                {
                    sError = "iRowCnt divided by iRowGrCnt divided by iRowCntInGroup have to be integer" ; //서브그룹카운트로 나눴을때 딱 떨어져야 함.
                    return false ;
                }
            }

            Para = _tPara ;
            return true ;
        }

        private double GetPos(int _iIdx , int _iMaxIdx , double _dPitch , int _iGrCnt, double _dGrPitch , int _iSbGrCnt , double _dSbGrPitch)
        {
            //그룹과 서브그룹의 칩갯수.
            int iGroupChipCnt    = _iGrCnt  !=0 ? (_iMaxIdx     / _iGrCnt  ) : 0 ;
            int iSubGroupChipCnt = _iSbGrCnt!=0 ? (iGroupChipCnt / _iSbGrCnt) : 0 ;

            //그룹간의 거리에서 피치를 뺀 나머지 거리를 계산.
            //double dColGroupOfs    = Para.dColGrGap   - Para.dColPitch ; //무조건 0보다 켜야함.
            //double dColSubGroupOfs = Para.dColSbGrGap - Para.dColPitch ; //무조건 0보다 켜야함.

            //dX = _iC * Para.dColPitch ;

            double dPos = 0 ;
            // 칩 , 칩그룹 , 칩서브그룹 피치개념이여서 1부터 시작.
            for(int c = 1 , cg = 1 , csg = 1 ; c<=_iIdx ; c++ , cg++ , csg++) 
            {
                if(iGroupChipCnt!=0 && cg==iGroupChipCnt)//그룹의 칩카운트와 같다면 그룹갭을 더함.
                {
                    dPos += _dGrPitch ;
                    cg  = 0 ;//그룹 카운트 리셑.
                    csg = 0 ;//그룹 카운트가 상위여서 서브그룹카운트도 리셑.
                }
                else if(iSubGroupChipCnt != 0 && cg==iSubGroupChipCnt)//서브그룹의 칩카운트와 같다면.
                {
                    dPos += _dSbGrPitch ;
                    csg = 0 ; //서브그룹카운트 리셑.
                }
                else 
                {
                    dPos += _dPitch ;
                }
            }

            return dPos ;
        }

        /// <summary>
        ///하기내용 일단 너무복잡해 고려하지 않음===========
        ///성용이 말로는 중간에 서브그룹의 칩갯수가 3개일때 2개씩검사해서 1번은 2개검사 2번은 1개검사 3번은 2개검사 4번은 1개검사 이렇게도 된다고함.
        ///그래서 포지션은 검사세트의 센터로 하면 안되고 첫자재의 포지션이 세트첫번째 포지션과 일치하게 검사해야함. 
        /// </summary>
        /// <param name="_iC">Col</param>
        /// <param name="_iR">Row</param>
        /// <param name="_iQuadrant">하부가 고정된 스테이지고 상부가 움직일때 4사분면중 사용사분면 설정</param>
        /// <param name="_dX">결과값 X</param>
        /// <param name="_dY">결과값 Y</param>
        /// <returns></returns>
        public bool GetPos(int _iC , int _iR , int _iQuadrant ,out double _dX , out double _dY) //Array와 같이 왼쪽 위가 기준 포지션.
        {
            _dX = 0 ;
            _dY = 0 ;
            if(_iQuadrant < 1 || _iQuadrant > 4)
            {
                sError = "Quadrant have to be between 1 and 4" ;
                return false ;
            }
            if(_iC >= Para.iColCnt)
            {
                sError = "_iC have to be smaller than iColCnt" ;
                return false ;
            }
            if(_iR >= Para.iRowCnt)
            {
                sError = "_iR have to be smaller than iRowCnt" ;
                return false ;
            }
            if(_iC < 0)
            {
                sError = "_iC have to be bigger than 0" ;
                return false ;
            }
            if(_iR < 0)
            {
                sError = "_iR have to be bigger than 0" ;
                return false ;
            }

            //double dMaxX = GetPos(Para.iColCnt-1, Para.iColCnt, Para.dColPitch, Para.iColGrCnt , Para.dColGrGap , Para.iColSbGrCnt , Para.dColSbGrGap);
            //double dMaxY = GetPos(Para.iRowCnt-1, Para.iRowCnt, Para.dRowPitch, Para.iRowGrCnt , Para.dRowGrGap , Para.iRowSbGrCnt , Para.dRowSbGrGap);

            double dRetX = GetPos(_iC, Para.iColCnt, Para.dColPitch, Para.iColGrCnt , Para.dColGrGap , Para.iColSbGrCnt , Para.dColSbGrGap);
            double dRetY = GetPos(_iR, Para.iRowCnt, Para.dRowPitch, Para.iRowGrCnt , Para.dRowGrGap , Para.iRowSbGrCnt , Para.dRowSbGrGap);

            if(_iQuadrant == 1)//1사분면기준인 상태의 값.
            {
                _dX =  dRetX ;
                _dY = -dRetY ;                
            }
            else if(_iQuadrant == 2)//2사분면기준인 상태의 값.
            {
                _dX = -dRetX ;
                _dY = -dRetY ;   
            }
            else if(_iQuadrant == 3)//3사분면기준인 상태의 값.
            {
                _dX = -dRetX ;
                _dY =  dRetY ;
            }
            else //4사분면기준인 상태의 값.
            {
                _dX = dRetX  ;
                _dY = dRetY  ;
            }

            return true ;
        }
    }
}
