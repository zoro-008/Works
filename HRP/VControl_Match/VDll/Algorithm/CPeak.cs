using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDll.Algorithm
{
    class CPeak
    {
        [Serializable ,TypeConverter(typeof(ExpandableObjectConverter))]
        public class CUPara
        {                                                  
            [CategoryAttribute("UPara"), DisplayNameAttribute("Low Threshold"       )] public uint     LowThreshold      {get;set;}        
            [CategoryAttribute("UPara"), DisplayNameAttribute("High Threshold"      )] public uint     HighThreshold     {get;set;}      
        }

        public struct TRslt
        {
            public string sError ;
            public float  fPos   ;
        }

        static public bool FindPeakVertical (Mat _mtImage , int _iX , int _iStartY , int _iEndY , CUPara _upPara , ref TRslt _tRslt)
        {
            _tRslt.fPos = 0 ;

            bool bPrePxOn = false ;
            bool bCrntPxOn = false ;
            int  stride = _mtImage.Step ;
            byte CrntPx = 0 ;  //현재 픽셀 밝기.
            int  iPxSum = 0 ;  //도수값의 합.
            int  iPxYSum = 0 ; //계급값 * 도수값의 합.
            int  iMaxPxValue = 0 ; //현재까지 가장 밝았던 픽셀의 밝기값. 나중에 피크 선택 할 때 쓴다.
            int  iCrntAreaMaxPxValue = 0 ; //현재 관찰 영역의 맥스 값을 넣어둔다.
            int  iMaxAreaSize = 0 ; //현재까지 가장 넓은 넓이 세팅.

            unsafe
            {
                byte* Data = (byte *)_mtImage.DataPointer;               

                for(int y = _iStartY ; y <= _iEndY ; y++)
                {
                    CrntPx = *(Data + (y * stride) + _iX) ;
                    bPrePxOn = bCrntPxOn ;
                    bCrntPxOn = _upPara.LowThreshold <= CrntPx && CrntPx <= _upPara.HighThreshold ;

                    //관찰영역 진입.
                    if(bCrntPxOn)
                    {
                        if(!bPrePxOn)
                        {
                            iPxSum = 0 ;
                            iPxYSum = 0 ;
                            iCrntAreaMaxPxValue = 0 ;
                        }

                        if(iCrntAreaMaxPxValue < CrntPx) //혹시 가장밝은 픽셀이 세로 나타났으면 갱신함.
                        {
                            iCrntAreaMaxPxValue = CrntPx ; 
                        }
                        iPxYSum += CrntPx * y ;
                        iPxSum  += CrntPx  ;

                        //관찰영역이 끝나거나 다음 영역이 OnPx이 아닐경우 최고점인지 비교해서 크면 갱신.
                        if(y == _iEndY || _upPara.LowThreshold > *(Data + ((y+1) * stride) + _iX) || *(Data + ((y+1) * stride) + _iX) > _upPara.HighThreshold )
                        {
                            //이건 혹시 255만땅 찰경우가 많을듯 해서 
                            //만땅짜리 들이 다수일 경우 그중에 가장 면적넓은것을 등록하게 함.
                            if(iCrntAreaMaxPxValue == iMaxPxValue) 
                            {
                                if(iMaxAreaSize < iPxSum)
                                {
                                    iMaxPxValue  = iCrntAreaMaxPxValue ;
                                    iMaxAreaSize = iPxSum ;
                                    _tRslt.fPos = iPxYSum / (float)iPxSum ;
                                }
                            }
                            else if(iCrntAreaMaxPxValue > iMaxPxValue)
                            {
                                iMaxPxValue  = iCrntAreaMaxPxValue ;
                                iMaxAreaSize = iPxSum ;
                                _tRslt.fPos = iPxYSum / (float)iPxSum ;
                            }
                        }
                    }
                }
            }
            return true ;
        }
    }
}
