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
            [CategoryAttribute("UPara" ), DisplayNameAttribute("Match Scale"        )] public double                MatchScale         {get;set;} = 1 ; //1.5
            [CategoryAttribute("UPara" ), DisplayNameAttribute("Rotation Range"     )] public uint                  RotationRange      {get;set;} = 2 ; //20
            [CategoryAttribute("UPara" ), DisplayNameAttribute("Rotation Graduation")] public double                RotationGraduation {get;set;} = 1 ; //20
            [CategoryAttribute("UPara" ), DisplayNameAttribute("Mathode"            )] public TemplateMatchingType  Mathode            {get;set;} = TemplateMatchingType.CcorrNormed ; //20
        }

        public struct TRslt
        {
            public double dImgAngle ;
            public double dX ;
            public double dY ;
            public double dScore ;
            public string sError ;
        }

        //템플릿을 이용한 기본 메치
        static public bool Find (Mat _mtImage , Mat _mtTemplate , CUPara _upPara , ref TRslt _tRslt)
        {
            /// Source image to display
            Mat mtResult = new Mat();
           
            double  dFindImgAngle = 0 ;
            double  dFindImgVal =  0 ;
            int     iFindX      =  0 ;
            int     iFindY      =  0 ;


            if(_upPara.Mathode == TemplateMatchingType.SqdiffNormed || _upPara.Mathode == TemplateMatchingType.Sqdiff)
            {
                dFindImgVal = double.MaxValue;
            }
            else
            {
                dFindImgVal = double.MinValue;
            }

            //-레인지에서 +레인지까지 세팅 하고 0도가 가운데 배치되기 위해 이렇게 함.
            double dAngGrd = _upPara.RotationGraduation ;
            double dRotRng = _upPara.RotationRange ;
            if(dAngGrd <= 0 || dRotRng <= 0)
            {
                dAngGrd = 1 ;
                dRotRng = 0 ;
            }
            //if(dAngGrd == 0)
            //여기부터 dAngGrd 0으로 입력 됐을때 예외 처리. 검사 1번만 해야함.
            int iImgCnt = (int)(dRotRng / dAngGrd) ;
            double dStartAngle = -_upPara.RotationGraduation * iImgCnt ;
            double dEndAngle   =  _upPara.RotationGraduation * iImgCnt ;

            using (Mat RotMap      = new Mat()          )
            using (Mat RotTemplate = _mtTemplate.Clone())//회전한 템플릿.
            using (Mat Mask        = _mtTemplate.Clone())//마스크는 탬플릿 이미지와 똑같은 규격이여야 함.
            using (Mat RotMask     = _mtTemplate.Clone())//회전한 마스크.
            {
                //마스크적용은 2가지 종류의 검사에서만 됌.
                bool bCanUseMask = (_upPara.Mathode == TemplateMatchingType.Sqdiff|| _upPara.Mathode == TemplateMatchingType.CcorrNormed);
                
                for (double dAngle = dStartAngle ; dAngle <= dEndAngle ; dAngle += dAngGrd )
                {                         
                    try
                    {
                        //0도 아닌놈들 이미지 돌리기. <= 이렇게 하니 검사가 안됌;;
                        //if (dAngle != 0)
                        {
                            //마스크는 미리 색칠하고 돌리면 무효영역은 0이 된다.
                            Mask   .SetTo(new MCvScalar(255));
                            RotMask.SetTo(new MCvScalar(0  ));

                            PointF pt = new PointF((float)(_mtTemplate.Cols / 2.0), (float)(_mtTemplate.Rows / 2.0));
                            CvInvoke.GetRotationMatrix2D(pt, dAngle, 1.0, RotMap);
                            CvInvoke.WarpAffine(_mtTemplate, RotTemplate, RotMap, new Size(_mtTemplate.Cols, _mtTemplate.Rows));
                        }
                        //else
                        //{
                        //    //마스크 폼으로 띄울때 전에루프꺼 띄워서 헛갈려서 칠해놓음.
                        //    RotMask.SetTo(new MCvScalar(255));
                        //}

                        //가장 확율높은 곳의 좌표를 뽑기 위해 리절트맵을 만듬.5
                        int iRsltCol = _mtImage.Cols - RotTemplate.Cols + 1;
                        int iRsltRow = _mtImage.Rows - RotTemplate.Rows + 1;
                        mtResult.Create(iRsltRow, iRsltCol, DepthType.Cv32F, 1); //32비트float , 1chanel


                        if (bCanUseMask)
                        {
                            if(dAngle != 0)
                            {
                                //마스크 돌리기.
                                CvInvoke.WarpAffine(Mask, RotMask, RotMap, new Size(Mask.Cols, Mask.Rows));
                                //메칭.
                                CvInvoke.MatchTemplate(_mtImage, RotTemplate, mtResult, _upPara.Mathode, RotMask);
                            }
                            else
                            {
                                CvInvoke.MatchTemplate(_mtImage, RotTemplate, mtResult, _upPara.Mathode);
                            }                            
                        }
                        else
                        {
                            CvInvoke.MatchTemplate(_mtImage, RotTemplate, mtResult, _upPara.Mathode);
                        }


                        CvInvoke.Imshow(dAngle.ToString(), RotTemplate);

                        double dMinVal=0.0 ,  dMaxVal=0.0 ;
                        Point MinPos = new Point();
                        Point MaxPos = new Point();
                        CvInvoke.MinMaxLoc(mtResult, ref dMinVal, ref dMaxVal, ref MinPos, ref MaxPos);

                        //Sqdiff 는 적을수록 좋음.
                        if (_upPara.Mathode == TemplateMatchingType.SqdiffNormed || _upPara.Mathode == TemplateMatchingType.Sqdiff)
                        {
                            if (dMinVal < dFindImgVal)//가장점수 높은놈 등록
                            {
                                dFindImgAngle = dAngle;
                                dFindImgVal = dMinVal;
                                //dFindImgVal = dMaxValues[0] ;
                                iFindX = MinPos.X;
                                iFindY = MinPos.Y;
                            }
                        }
                        else
                        {
                            if (dMaxVal > dFindImgVal)//가장점수 높은놈 등록
                            {
                                dFindImgAngle = dAngle;
                                dFindImgVal = dMaxVal;
                                //dFindImgVal = dMaxValues[0] ;
                                iFindX = MaxPos.X;
                                iFindY = MaxPos.Y;
                            }
                        }
                        //각도 세팅 안하면 증가가 안되어 무한 루프 돌게 되어 강제 브레이크
                        //if(_upPara.RotationGraduation ==0) break ;
                    }
                    catch (Exception _e)
                    {
                        _tRslt.sError = _e.Message ;
                        return false ;
                    }
                
                }
            }            

            _tRslt.dImgAngle = dFindImgAngle ;
            _tRslt.dScore    = dFindImgVal ;
            _tRslt.dX        = iFindX ;
            _tRslt.dY        = iFindY ;

            return true ;
        }
    }
}
