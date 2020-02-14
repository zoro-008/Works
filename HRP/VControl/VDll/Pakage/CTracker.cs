﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using COMMON;

namespace VDll.Pakage
{
    public class CTracker
    {    
        bool m_bSelected  ;
        bool m_bMoved     ; //움직이고 세이브 안한상태.
        bool m_bInRange   ; //라인 선택 범위 안에 있음.
        bool m_bPreInRange;
        int m_iPointCount ;
        PointF[] m_aPoints;
        PointF[] CalLeft  ; //Calibration에서 격자형 포인트 위치
        PointF[] CalRight ; //Calibration에서 격자형 포인트 위치
        PointF[] CalTop   ; //Calibration에서 격자형 포인트 위치
        PointF[] CalBottom; //Calibration에서 격자형 포인트 위치
        List<string> lsMsg = new List<string>(); //Message Tracker에서 사용하는 리스트
        const int    m_iMsgMaxCnt = 10;

        //프로그램에서 선택되는 것들.
        protected int  m_iTag    ; //테그로 쓴다.
        protected bool m_bVisible; //보이지 않음.
        protected bool m_bEnabled; //클릭이 안되고 포인트들이 안보임.

        //User UI Setting Var.
        protected ETrackerType m_eTrackerType    ; //트렉커의 타입 선정.
        protected int          m_dRoiWidth       ; 
        protected Color        m_cLineColor      ; //선의 색깔.
        protected Color        m_cRectPointColor ; //점의 색깔.
        protected Color        m_cPointColor     ; //점의 색깔.
        protected int          m_iLineWidth      ;
        protected int          m_iPointSize      ;
        protected int          m_iRectPointSize  ;
        protected String       m_sCaption        ;
        protected string       m_sMessage        ;
        
        //프로그램에서 선택되는 것들.
        public bool visible { get { return m_bVisible ; } set { m_bVisible = value ; } }
        public int  tag     { get { return m_iTag     ; } set { m_iTag     = value ; } }
        public bool enabled { get { return m_bEnabled ; } set { m_bEnabled = value ; } }

        //User UI Setting Var.
        public ETrackerType trackerType    { get { return m_eTrackerType   ; } set { m_eTrackerType    = value; } }

        public int          roiWidth       { get { return m_dRoiWidth      ; } set { m_dRoiWidth       = value; } }
        public Color        lineColor      { get { return m_cLineColor     ; } set { m_cLineColor      = value; } }
        public Color        rectPointColor { get { return m_cRectPointColor; } set { m_cRectPointColor = value; } }
        public Color        pointColor     { get { return m_cPointColor    ; } set { m_cPointColor     = value; } }
        public int          lineWidth      { get { return m_iLineWidth     ; } set { m_iLineWidth      = value; } }
        public int          pointSize      { get { return m_iPointSize     ; } set { m_iPointSize      = value; } }
        public int          rectPointSize  { get { return m_iRectPointSize ; } set { m_iRectPointSize  = value; } }
        private double dPntHfSize     ;
        private double dRectPntHfSize ;
        private float  fPntHfSize     ;
        private float  fRectPntHfSize ;

        public string       caption        { get { return m_sCaption       ; } set { m_sCaption        = value; } }
        public string       message        { 
            get { return m_sMessage       ; } 
            set { 
                lock(lsMsg)
                {
                    lsMsg.Clear();
                }
                m_sMessage = value; } 
        }

        public enum ETrackerType
        {
            ttNone = 0      , //로딩 하지 않고 픽스로 사용 한다.
            ttRect          , //사각형
            ttLine          , //라인
            ttCircle        , //원형
            ttEllipse       , //타원
            ttPolygon       , //다각형 트레커.
            ttPolyline      , //나중에 필요할때 하자..여러포인트를 가지고 있는 라인 트레커.
            ttCalibration   , //Calibration용
            ttMessage       ,
            MAX_TRAKCER_TYPE
        }
        ETrackerType eTrackerType;

        public enum EDownType
        {
            //이것은 dtLeftTop 이렇게 풀어서 재정의.
            dtLeftTop    = -100, dtCenterTop    = -101, dtRightTop    = -102,
            dtLeftCenter = -103, dtMove         = -104, dtRightCenter = -105,
            dtLeftBottom = -106, dtCenterBottom = -107, dtRightBottom = -108,

            dtStart = -3,
            dtEnd = -2,

            dtNone = -1,
        }

        public enum EPointId
        {
            //플어서 쓰기.
            //Rect, Circle, Ellipse Line용      
            piRectLeftTop     = 0,  piLineStart = 0, 
            piRectRightTop    = 1,  piLineEnd   = 1,  
            piRectRightBottom = 2,
            piRectLeftBottom  = 3,
        };


        protected struct TMoveStat //protected
        {
            public EDownType eDnType    ; //클릭다운된 아이디.
            public float     fX, fY     ; //클릭하고 이동한 양.
            public EDownType eLastDnType; //마지막으로 클릭다운된 아이디. 팝업띄우기 위해 존재.
            public bool      bRight     ;    //오른클릭으로 팝업을 띄우는데 자꾸 포지션이 바뀌어서 삽입.

            public void Clear()
            {
                eDnType     = EDownType.dtNone;
                fX = fY     = 0.0f            ;
                eLastDnType = EDownType.dtNone;
                bRight      = false           ;
            }
        }
        TMoveStat MoveStat;

        public void Init(ETrackerType _trackerType = ETrackerType.ttNone, bool _bVisible = false)
        {
            if(_trackerType == ETrackerType.ttNone) trackerType = ETrackerType.ttRect;
            trackerType = _trackerType;

            m_bMoved       = false;
            m_bInRange     = false;
            m_bPreInRange  = false;
            m_bSelected    = false;
                           
            m_aPoints      = null;
            CalLeft        = null;
            CalRight       = null;
            CalTop         = null;
            CalBottom      = null;
            m_iPointCount  = 0;
                           
            //visible        = true;
            visible        = _bVisible;
            enabled        = true     ; 
            
            lineColor      = Color.Lime;
            roiWidth       = 20    ;
            rectPointColor = Color.Red;
            pointColor     = Color.Green;
            lineWidth      = 1;
            
            //Point Size
            rectPointSize  = 6;
            pointSize      = 6;
            dPntHfSize     = pointSize     / 2.0 ; 
            dRectPntHfSize = rectPointSize / 2.0 ; 
            fPntHfSize     = pointSize     / 2.0f;
            fRectPntHfSize = rectPointSize / 2.0f;

            caption        = "";
            MoveStat.Clear();

            //SetTrackerType(trackerType);
            SetTrackerType();
        }

        public void Reset()
        {
            SetTrackerType();
        }

        public void SetTrackerType()//ETrackerType _eTrackerType = ETrackerType.ttNone, bool _bInit = false)
        {
            //if(eTrackerType == _eTrackerType) return ;
            ETrackerType _eTrackerType = eTrackerType;
        
            if(_eTrackerType == ETrackerType.ttLine){
                m_iPointCount = 1 ;
                m_aPoints = new PointF[m_iPointCount] ;

                m_aPoints[(int)EPointId.piLineStart].X = 60 ;
                m_aPoints[(int)EPointId.piLineStart].Y = 60 ;
                m_aPoints[(int)EPointId.piLineEnd  ].X = 100;
                m_aPoints[(int)EPointId.piLineEnd  ].Y = 60 ;
            }
            else if(_eTrackerType == ETrackerType.ttCalibration)
            {
                CalLeft   = new PointF[5];
                CalRight  = new PointF[5];
                CalTop    = new PointF[5];
                CalBottom = new PointF[5];
            }
            else //if(_eTrackerType == ETrackerType.ttRect || _eTrackerType == ETrackerType.ttCircle){
            { 
                m_iPointCount = 4 ;
                m_aPoints = new PointF[m_iPointCount] ;
        
                m_aPoints[(int)EPointId.piRectLeftTop].X = 60;
                m_aPoints[(int)EPointId.piRectLeftTop].Y = 60;

                m_aPoints[(int)EPointId.piRectRightTop].X = 140;
                m_aPoints[(int)EPointId.piRectRightTop].Y = 60;
                
                m_aPoints[(int)EPointId.piRectRightBottom].X = 140;
                m_aPoints[(int)EPointId.piRectRightBottom].Y = 140;

                m_aPoints[(int)EPointId.piRectLeftBottom].X = 60;
                m_aPoints[(int)EPointId.piRectLeftBottom].Y = 140;
            }
        
            eTrackerType = _eTrackerType ;
        }

        public void Close()
        {
            if (m_aPoints != null)
            {
                m_aPoints = null;
            }
        }

        //맴버.
        protected bool CheckRectIn(double _dPntX, double _dPntY, double _dX, double _dY, double _dPntHfSize) //사각포인트 중간크기를 넘김.
        {
            return _dX > _dPntX - _dPntHfSize && _dX <= _dPntX + _dPntHfSize &&
                   _dY > _dPntY - _dPntHfSize && _dY <= _dPntY + _dPntHfSize;
        }

        protected bool CheckLineIn(PointF[] _aPoints, int _iPointCount, float _dX, float _dY, double _dMargin) //라인 클릭으로 판단할 마진 넘김.
        {
            if (_iPointCount < 2) return false;

            PointF P1 = new PointF();
            PointF P2 = new PointF();
            Point Pnt = new Point();
            Point[] aRectPnt = new Point[4];

            P1 = _aPoints[0];
            P2 = _aPoints[_iPointCount - 1];

            double dRectPntHfSize = rectPointSize / 2.0;
            for (int i = 1; i <= _iPointCount; i++)
            {
                GetLineBandPnts(P1,P2,dRectPntHfSize, aRectPnt);
                Pnt.X = (int)_dX;
                Pnt.Y = (int)_dY;
                if(CMath.IsPntInPolygon(aRectPnt , 4 , Pnt))return true ; //이거 문제
                P2 = P1;
                P1.X = (int)(_aPoints[i % _iPointCount].X);
                P1.Y = (int)(_aPoints[i % _iPointCount].Y);
            }

            return false;
        }

        protected void GetLineBandPnts(PointF _tPnt1, PointF _tPnt2, double _dMargin, Point[] _a4Points) //_aPoints 4개 포인트 배열 넘겨야함.
        {
            double   dAngle = CMath.GetLineAngle(_tPnt1.X, _tPnt1.Y, _tPnt2.X, _tPnt2.Y);
            double[] dPntX  = new double[4];
            double[] dPntY  = new double[4];
                        
            CMath.GetPntFromPntByAngLen(_tPnt1.X, _tPnt1.Y,           //입력 시작점.
                                         dAngle + 90.0,               //입력 각도.
                                        _dMargin,                     //입력 길이.
                                        ref dPntX[0], ref dPntY[0]);  //출력 포인트.

            CMath.GetPntFromPntByAngLen(_tPnt2.X, _tPnt2.Y,           //입력 시작점.
                                         dAngle + 90.0,               //입력 각도.
                                        _dMargin,                     //입력 길이.
                                        ref dPntX[1], ref dPntY[1]);  //출력 포인트.

            CMath.GetPntFromPntByAngLen(_tPnt2.X, _tPnt2.Y,           //입력 시작점.
                                         dAngle - 90.0,               //입력 각도.
                                        _dMargin,                     //입력 길이.
                                        ref dPntX[2], ref dPntY[2]);  //출력 포인트.

            CMath.GetPntFromPntByAngLen(_tPnt1.X, _tPnt1.Y,           //입력 시작점.
                                         dAngle - 90.0,               //입력 각도.
                                        _dMargin,                     //입력 길이.
                                        ref dPntX[3], ref dPntY[3]);  //출력 포인트.

            _a4Points[0].X = (int)dPntX[0];
            _a4Points[0].Y = (int)dPntY[0];
            _a4Points[1].X = (int)dPntX[1];
            _a4Points[1].Y = (int)dPntY[1];
            _a4Points[2].X = (int)dPntX[2];
            _a4Points[2].Y = (int)dPntY[2];
            _a4Points[3].X = (int)dPntX[3];
            _a4Points[3].Y = (int)dPntY[3];
        }

        public float GetPointX(int _iPointId)
        {
            return m_aPoints[_iPointId].X;
        }
        public float GetPointY(int _iPointId)
        {
            return m_aPoints[_iPointId].Y;
        }

        public Rectangle GetRectangle()
        {
            int iLeft   = int.MaxValue;
            int iRight  = int.MinValue;
            int iTop    = int.MaxValue;
            int iBottom = int.MinValue;
            for (int i = 0; i < m_iPointCount; i++)
            {
                if (iLeft   > m_aPoints[i].X) iLeft   = (int)m_aPoints[i].X;
                if (iRight  < m_aPoints[i].X) iRight  = (int)m_aPoints[i].X;
                if (iTop    > m_aPoints[i].Y) iTop    = (int)m_aPoints[i].Y;
                if (iBottom < m_aPoints[i].Y) iBottom = (int)m_aPoints[i].Y;
            }

            return new Rectangle(iLeft, iTop, iRight - iLeft, iBottom - iTop);
        }

        public void AddMessage(string _sMsg)
        {            
            if(_sMsg =="") return ;
            m_sMessage += "\r\n" ;
            m_sMessage += _sMsg  ;

            //lsMsg.Add(_sMsg);
            lock(lsMsg)
            {
                lsMsg.Insert(0,_sMsg);                
                while(lsMsg.Count >= m_iMsgMaxCnt)
                {
                    lsMsg.Remove(lsMsg[lsMsg.Count-1]);
                }
            }
        }
        
        //public List<string> AddMessage(Rectangle _Rect, Font _Font)
        //{
        //    
        //    string sMsg = message;
        //   
        //    if (sMsg != null)
        //    {
        //        lsMsg.Add(sMsg);
        //        message = null;
        //    }
        //    
        //    if ((lsMsg.Count * _Font.GetHeight()) > _Rect.Height)
        //    {
        //        lsMsg.Remove(lsMsg[0]);
        //    }
        //    
        //    return lsMsg;
        //}


        private bool InsertPoint(int _iNo) //private
        {
            if (_iNo < 0) _iNo = 0;
            if (_iNo >= m_iPointCount + 1) _iNo = m_iPointCount + 1;

            m_iPointCount++;
            PointF[] aTempPnt = new PointF[m_iPointCount];
            int iInserted = 0;
            for (int i = 0; i < m_iPointCount; i++)
            {
                if (i == _iNo)
                {
                    aTempPnt[i].X = m_aPoints[i].X + pointSize;
                    aTempPnt[i].Y = m_aPoints[i].Y + pointSize;
                    iInserted = 1;
                    continue;
                }
                aTempPnt[i].X = m_aPoints[i - iInserted].X;
                aTempPnt[i].Y = m_aPoints[i - iInserted].Y;
            }

            if (m_aPoints != null) m_aPoints = null;
            m_aPoints = new PointF[m_iPointCount];

            for (int i = 0; i < m_iPointCount; i++)
            {
                m_aPoints[i] = aTempPnt[i];
            }

            aTempPnt = null;

            return true;
        }

        private bool DeletePoint(int _iNo) //private
        {
            if (m_iPointCount < 3) return false;
            if (_iNo < 0 || m_iPointCount <= _iNo) return false;

            m_iPointCount--;
            PointF[] aTempPnt = new PointF[m_iPointCount];
            int iDeleted = 0;

            for (int i = 0; i < m_iPointCount; i++)
            {
                if (i == _iNo)
                {
                    iDeleted = 1;
                }
                aTempPnt[i].X = m_aPoints[i + iDeleted].X;
                aTempPnt[i].Y = m_aPoints[i + iDeleted].Y;
            }

            if (m_aPoints != null) m_aPoints = null;
            m_aPoints = new PointF[m_iPointCount];

            for (int i = 0; i < m_iPointCount; i++)
            {
                m_aPoints[i] = aTempPnt[i];
            }

            aTempPnt = null;

            return true;
        }
        
        float GetScaleOffsetValX(float _fOriX , TScaleOffset _ScaleOffset)
        {
            float fRet = (_fOriX - _ScaleOffset.fOffsetX)  * _ScaleOffset.fScaleX ;
            return fRet ;
        }
        float GetOriginalValX(int _iViewX , TScaleOffset _ScaleOffset)
        {
            float fRet = (_iViewX / _ScaleOffset.fScaleX) + _ScaleOffset.fOffsetX  ;
            return fRet ;
        }

        float GetScaleOffsetValY(float _fOriY , TScaleOffset _ScaleOffset)
        {
            float fRet = (_fOriY - _ScaleOffset.fOffsetY)  * _ScaleOffset.fScaleY ;
            return fRet ;
        }
        float GetOriginalValY(int _iViewY , TScaleOffset _ScaleOffset)
        {
            float fRet = (_iViewY / _ScaleOffset.fScaleY) + _ScaleOffset.fOffsetY  ;
            return fRet ;
        }
        
        public void Paint(Graphics _g, TScaleOffset _ScaleOffset)
        {
            if (!visible) return;

            if (trackerType == ETrackerType.ttRect) //Rectangle Tracker
            {
                RectangleF Rect = GetRectangle();

                float fLeft   = GetScaleOffsetValX(Rect.Left  , _ScaleOffset)  ;
                float fTop    = GetScaleOffsetValY(Rect.Top   , _ScaleOffset)  ;
                float fRight  = GetScaleOffsetValX(Rect.Right , _ScaleOffset)  ;
                float fBottom = GetScaleOffsetValY(Rect.Bottom, _ScaleOffset)  ;
                float fWidth  = fRight  - fLeft ;
                float fHeight = fBottom - fTop  ;

                float fCx = fLeft + fWidth  / 2.0f;
                float fCy = fTop  + fHeight / 2.0f;

                using (Pen pen = new Pen(lineColor, 1))
                {
                    pen.DashStyle = DashStyle.Solid;
                    pen.Color = lineColor;
                    pen.Width = m_bInRange ? lineWidth + 1 : lineWidth;

                    _g.DrawRectangle(pen, fLeft, fTop, fWidth, fHeight);

                    if (m_bSelected)
                    {
                        using (SolidBrush brush = new SolidBrush(pointColor))
                        {
                            _g.FillEllipse(brush, fLeft  - fPntHfSize, fTop    - fPntHfSize, pointSize, pointSize);
                            _g.FillEllipse(brush, fRight - fPntHfSize, fTop    - fPntHfSize, pointSize, pointSize);
                            _g.FillEllipse(brush, fRight - fPntHfSize, fBottom - fPntHfSize, pointSize, pointSize);
                            _g.FillEllipse(brush, fLeft  - fPntHfSize, fBottom - fPntHfSize, pointSize, pointSize);
                        }
                        using (SolidBrush brush = new SolidBrush(rectPointColor))
                        {
                            _g.FillRectangle(brush, fCx    - fRectPntHfSize, fTop    - fRectPntHfSize, rectPointSize, rectPointSize);
                            _g.FillRectangle(brush, fRight - fRectPntHfSize, fCy     - fRectPntHfSize, rectPointSize, rectPointSize);
                            _g.FillRectangle(brush, fCx    - fRectPntHfSize, fBottom - fRectPntHfSize, rectPointSize, rectPointSize);
                            _g.FillRectangle(brush, fLeft  - fRectPntHfSize, fCy     - fRectPntHfSize, rectPointSize, rectPointSize);
                        }
                    }
                    using (SolidBrush brush = new SolidBrush(Color.Blue))
                    {
                        using (Font font = new Font("Arial", 10)) 
                        {
                            float sx = 1 * _ScaleOffset.fScaleX ;
                            float sy = 1 * _ScaleOffset.fScaleY ;
                            string sHeaderMsg = string.Format("L:{0:0.00} T:{1:0.00} W:{2:0.00} H:{3:0.00}" , Rect.Left, Rect.Top, Rect.Width, Rect.Height);
                            _g.DrawString(sHeaderMsg, font, brush, fLeft, fTop - font.GetHeight() - sy);
                            lock(lsMsg)
                            {
                                for(int i = 0 ; i < lsMsg.Count ; i++) 
                                {
                                    _g.DrawString(lsMsg[i], font, brush, fLeft, fTop + (i * font.GetHeight()));
                                }
                            }
                        }
                    }

                }
            }

            else if (trackerType == ETrackerType.ttLine) //Rectangle Tracker
            {
                //Display Position.
                float fStartX = GetScaleOffsetValX(m_aPoints[0].X , _ScaleOffset);
                float fStartY = GetScaleOffsetValY(m_aPoints[0].Y , _ScaleOffset);
                float fEndX   = GetScaleOffsetValX(m_aPoints[1].X , _ScaleOffset);
                float fEndY   = GetScaleOffsetValY(m_aPoints[1].Y , _ScaleOffset);

                using (Pen pen = new Pen(lineColor, 1))
                {
                    using (AdjustableArrowCap cusCap = new AdjustableArrowCap(3 * _ScaleOffset.fScaleX, 5 * _ScaleOffset.fScaleY))
                    {
                        pen.Width        = m_bInRange ? lineWidth + 1 : lineWidth;
                        pen.Color        = lineColor;
                        pen.StartCap     = LineCap.AnchorMask;
                        pen.EndCap       = LineCap.Custom;
                        pen.CustomEndCap = cusCap;

                        _g.DrawLine(pen, fStartX, fStartY, fEndX, fEndY);

                        using (SolidBrush brush = new SolidBrush(pointColor))
                        {
                            if (m_bSelected)
                            {
                                _g.FillEllipse(brush, fEndX   - fPntHfSize, fEndY   - fPntHfSize, pointSize, pointSize);
                                _g.FillEllipse(brush, fStartX - fPntHfSize, fStartY - fPntHfSize, pointSize, pointSize);
                            }
                        }
                    }
                    using (SolidBrush brush = new SolidBrush(Color.Blue))
                    {
                        using (Font font = new Font("Arial", 10)) 
                        {
                            float sx = 1 * _ScaleOffset.fScaleX ;
                            float sy = 1 * _ScaleOffset.fScaleY ;
                            string sHeaderMsg = string.Format("S:{0:0.00},{1:0.00} E:{2:0.00},{3:0.00} A:{4:0.00} L:{5:0.00}" , 
                                fStartX,fStartY,fEndX,fEndY,
                                CMath.GetLineAngle(fStartX, fStartY, fEndX, fEndY),
                                CMath.GetLineLenth(fStartX, fStartY, fEndX, fEndY));
                            _g.DrawString(sHeaderMsg, font, brush, fStartX, fStartY - font.GetHeight() - sy);
                            lock(lsMsg)
                            {
                                for(int i = 0 ; i < lsMsg.Count ; i++) 
                                {
                                    _g.DrawString(lsMsg[i], font, brush, fStartX, fStartY + (i * font.GetHeight()));
                                }
                            }
                        }
                    }
                    
                }
            }

            else if (trackerType == ETrackerType.ttEllipse || trackerType == ETrackerType.ttCircle) //Rectangle Tracker
            {
                RectangleF Rect = GetRectangle();

                float fViewLeft   = GetScaleOffsetValX(Rect.Left  , _ScaleOffset)  ;
                float fViewTop    = GetScaleOffsetValY(Rect.Top   , _ScaleOffset)  ;
                float fViewRight  = GetScaleOffsetValX(Rect.Right , _ScaleOffset)  ;
                float fViewBottom = GetScaleOffsetValY(Rect.Bottom, _ScaleOffset)  ;
                float fViewWidth  = fViewRight  - fViewLeft ;
                float fViewHeight = fViewBottom - fViewTop  ;

                float fViewCx = fViewLeft + fViewWidth  / 2.0f;
                float fViewCy = fViewTop  + fViewHeight / 2.0f;

                float fCx     = Rect.Left + Rect.Width  / 2.0f;
                float fCy     = Rect.Top  + Rect.Height / 2.0f;
                float fWidth  = Rect.Right  - Rect.Left       ;
                float fHeight = Rect.Bottom - Rect.Top        ;

                using (Pen pen = new Pen(lineColor, 1))
                {
                    pen.DashStyle = DashStyle.Solid;
                    pen.Color = lineColor;
                    pen.Width = m_bInRange ? lineWidth + 1 : lineWidth;

                    _g.DrawEllipse(pen, fViewLeft, fViewTop, fViewWidth, fViewHeight);

                    float sx = 1 * _ScaleOffset.fScaleX ;
                    float sy = 1 * _ScaleOffset.fScaleY ;

                    _g.DrawLine(pen, fViewCx-sx, fViewCy     , fViewCx+sx, fViewCy     );
                    _g.DrawLine(pen, fViewCx   , fViewCy - sy, fViewCx   , fViewCy + sy);
                    
                    if (m_bSelected)
                    {
                        using (SolidBrush brush = new SolidBrush(rectPointColor))
                        {
                            _g.FillRectangle(brush, fViewCx    - fRectPntHfSize , fViewTop    - fRectPntHfSize , rectPointSize , rectPointSize);
                            _g.FillRectangle(brush, fViewCx    - fRectPntHfSize , fViewBottom - fRectPntHfSize , rectPointSize , rectPointSize);

                            _g.FillRectangle(brush, fViewLeft  - fRectPntHfSize , fViewCy     - fRectPntHfSize , rectPointSize , rectPointSize);
                            _g.FillRectangle(brush, fViewRight - fRectPntHfSize , fViewCy     - fRectPntHfSize , rectPointSize , rectPointSize);
                        }
                    }
                    using (SolidBrush brush = new SolidBrush(Color.Blue))
                    {
                        using (Font font = new Font("Arial", 10)) 
                        {
                            string sHeaderMsg = string.Format("CX:{0:0.00} CY:{1:0.00} W:{2:0.00} H:{3:0.00}" , fCx, fCy, fWidth, fHeight);
                            _g.DrawString(sHeaderMsg, font, brush, fViewLeft, fViewTop - font.GetHeight());
                            lock(lsMsg)
                            {
                                for(int i = 0 ; i < lsMsg.Count ; i++) 
                                {
                                    _g.DrawString(lsMsg[i], font, brush, fViewLeft, fViewTop + (i * font.GetHeight()));
                                }
                            }
                        }
                    }

                }
            }

            else if (trackerType == ETrackerType.ttPolygon || trackerType == ETrackerType.ttPolyline) //Rectangle Tracker
            {
                RectangleF Rect = GetRectangle();

                float fViewLeft   = GetScaleOffsetValX(Rect.Left  , _ScaleOffset)  ;
                float fViewTop    = GetScaleOffsetValY(Rect.Top   , _ScaleOffset)  ;
                float fViewRight  = GetScaleOffsetValX(Rect.Right , _ScaleOffset)  ;
                float fViewBottom = GetScaleOffsetValY(Rect.Bottom, _ScaleOffset)  ;
                float fViewWidth  = fViewRight  - fViewLeft ;
                float fViewHeight = fViewBottom - fViewTop  ;

                float fViewCx = fViewLeft + fViewWidth  / 2.0f;
                float fViewCy = fViewTop  + fViewHeight / 2.0f;

                float fCx     = Rect.Left + Rect.Width  / 2.0f;
                float fCy     = Rect.Top  + Rect.Height / 2.0f;
                float fWidth  = Rect.Right  - Rect.Left       ;
                float fHeight = Rect.Bottom - Rect.Top        ;

                using (Pen pen = new Pen(lineColor, 1))
                {

                    pen.DashStyle = DashStyle.Solid;
                    pen.Color = lineColor;
                    pen.Width = m_bInRange ? lineWidth + 1 : lineWidth;

                    for(int i = 0 ; i < m_iPointCount ; i++) {
                        if(i >= m_iPointCount - 1) {
                            if(trackerType == ETrackerType.ttPolygon)
                            {
                                float fx0 = GetScaleOffsetValX(m_aPoints[0].X, _ScaleOffset)  ;
                                float fy0 = GetScaleOffsetValY(m_aPoints[0].Y, _ScaleOffset)  ;
                                float fxi = GetScaleOffsetValX(m_aPoints[i].X, _ScaleOffset)  ;
                                float fyi = GetScaleOffsetValY(m_aPoints[i].Y, _ScaleOffset)  ;
                                _g.DrawLine(pen, fx0, fy0, fxi, fyi );
                            }
                            continue ;
                        }
                            float fx1 = GetScaleOffsetValX(m_aPoints[i].X, _ScaleOffset)  ;
                            float fy1 = GetScaleOffsetValY(m_aPoints[i].Y, _ScaleOffset)  ;
                            float fx2 = GetScaleOffsetValX(m_aPoints[(i+1)%m_iPointCount].X, _ScaleOffset)  ;
                            float fy2 = GetScaleOffsetValY(m_aPoints[(i+1)%m_iPointCount].Y, _ScaleOffset)  ;

                            _g.DrawLine(pen, fx1, fy1, fx2, fy2 );
                            //Gdi -> Line(m_aPoints[i].x , m_aPoints[i].y , m_aPoints[(i+1)%m_iPointCount].x , m_aPoints[(i+1)%m_iPointCount].y);
                    }
                    
                    if (m_bSelected)
                    {
                        float sx = 1 * _ScaleOffset.fScaleX ;
                        float sy = 1 * _ScaleOffset.fScaleY ;
                        _g.DrawRectangle(pen, fViewLeft - 1, fViewTop - 1, (fViewWidth + sx), (fViewHeight + sy));
                        using (SolidBrush brush = new SolidBrush(rectPointColor))
                        {
                            for(int i = 0 ; i < m_iPointCount ; i++) {

                                float fx = GetScaleOffsetValX(m_aPoints[i].X, _ScaleOffset)  ;
                                float fy = GetScaleOffsetValY(m_aPoints[i].Y, _ScaleOffset)  ;
                                //if(m_bSelected && enabled) Gdi -> Circle(true , m_aPoints[i].X , m_aPoints[i].Y , dPntHfSize ,0);
                                if(m_bSelected) _g.FillEllipse(brush, fx - fPntHfSize, fy - fPntHfSize, pointSize, pointSize);
                            }
                        }
                    }
                    using (SolidBrush brush = new SolidBrush(Color.Blue))
                    {
                        using (Font font = new Font("Arial", 10)) 
                        {
                            float sx = 1 * _ScaleOffset.fScaleX ;
                            float sy = 1 * _ScaleOffset.fScaleY ;

                            string sMsg = string.Format("SX:{0:0.00} SY:{1:0.00}" , Rect.Left, Rect.Top);
                            _g.DrawString(sMsg, font, brush, fViewLeft     , fViewTop - font.GetHeight() - sy);
                            
                            sMsg = string.Format("EX:{0:0.00} EY:{1:0.00}" , Rect.Right, Rect.Bottom);
                            _g.DrawString(sMsg, font, brush, fViewRight    , fViewBottom + sy);
                            
                            sMsg = string.Format("CX:{0:0.00} CY:{1:0.00}" , fCx, fCy );
                            _g.DrawString(sMsg, font, brush, fViewCx       , fViewCy );

                            //sMsg = string.Format("W:{0:0.00} H:{1:0.00}" , fWidth, fHeight);
                            //_g.DrawString(sMsg, font, brush, fViewLeft + font.GetHeight(), fViewTop + font.GetHeight());

                            lock(lsMsg)
                            {
                                for(int i = 0 ; i < lsMsg.Count ; i++) 
                                {
                                    _g.DrawString(lsMsg[i], font, brush, fViewLeft, fViewTop + (i * font.GetHeight()));
                                }
                            }
                        }
                    }

                }
            }

            else if (trackerType == ETrackerType.ttCalibration) //Calibration Tracker
            {
                using (Pen pen = new Pen(lineColor))
                {
                    //격자형 라인 그리는 부분
                    for (int i = 0; i < 5; i++)
                    {
                        CalTop   [i].X = (m_aPoints[1].X * (i + 1) + m_aPoints[0].X * (5 - i)) / 6;
                        CalBottom[i].X = (m_aPoints[2].X * (i + 1) + m_aPoints[3].X * (5 - i)) / 6;
                        CalTop   [i].Y = (m_aPoints[1].Y * (i + 1) + m_aPoints[0].Y * (5 - i)) / 6;
                        CalBottom[i].Y = (m_aPoints[2].Y * (i + 1) + m_aPoints[3].Y * (5 - i)) / 6;

                        _g.DrawLine(pen, CalTop[i].X, CalTop[i].Y, CalBottom[i].X, CalBottom[i].Y);

                        CalLeft [i].X = (m_aPoints[3].X * (i + 1) + m_aPoints[0].X * (5 - i)) / 6;
                        CalRight[i].X = (m_aPoints[2].X * (i + 1) + m_aPoints[1].X * (5 - i)) / 6;
                        CalLeft [i].Y = (m_aPoints[3].Y * (i + 1) + m_aPoints[0].Y * (5 - i)) / 6;
                        CalRight[i].Y = (m_aPoints[2].Y * (i + 1) + m_aPoints[1].Y * (5 - i)) / 6;

                        _g.DrawLine(pen, CalLeft[i].X, CalLeft[i].Y, CalRight[i].X, CalRight[i].Y);
                    }

                    using (SolidBrush brush = new SolidBrush(Color.Purple))
                    {
                        using (Font font = new Font("Arial", 7)) 
                        {
                            brush.Color = Color.Purple;
                            _g.DrawString("LT", font, brush, m_aPoints[0].X - 10, m_aPoints[0].Y - 12);
                            _g.DrawString("RT", font, brush, m_aPoints[1].X     , m_aPoints[1].Y - 12);
                            _g.DrawString("RB", font, brush, m_aPoints[2].X     , m_aPoints[2].Y + 5 );
                            _g.DrawString("LB", font, brush, m_aPoints[3].X - 10, m_aPoints[3].Y + 5 );
                        }

                        pen.DashStyle = DashStyle.Solid;
                        pen.Color = lineColor;
                        pen.Width = m_bInRange ? lineWidth + 1 : lineWidth;

                        _g.DrawPolygon(pen, m_aPoints);

                        pen.Width = lineWidth;

                        pen.Color = rectPointColor;
                        brush.Color = rectPointColor;

                        for (int i = 0; i < m_iPointCount; i++)
                        {
                            if (m_bSelected && enabled) _g.FillEllipse(brush, m_aPoints[i].X - fPntHfSize, m_aPoints[i].Y - fPntHfSize, pointSize, pointSize);
                        }
                    }
                }
            }

            
        }

        public bool MouseDown(Keys _Shift, MouseEventArgs _e, TScaleOffset _ScaleOffset)
        {
            if (!visible || !enabled) return false ;

            float fImgX     = GetOriginalValX((int)_e.X , _ScaleOffset);
            float fImgY     = GetOriginalValY((int)_e.Y , _ScaleOffset);

            //Point[] pt = new Point[m_iPointCount];
            
            //for (int i = 0; i < m_iPointCount; i++)
            //{
            //    pt[i] = new Point((int)m_aPoints[i].X,(int)m_aPoints[i].Y);
            //}
            //bool bIs = CMath.IsPntInPolygon(pt, m_iPointCount, new Point((int)fImgX,(int)fImgY));

            MoveStat.fX = _e.X ;
            MoveStat.fY = _e.Y ;


            if (trackerType == ETrackerType.ttRect)
            {
                RectangleF Rect = GetRectangle();
                
                float fViewLeft   = GetScaleOffsetValX(Rect.Left  , _ScaleOffset)  ;
                float fViewTop    = GetScaleOffsetValY(Rect.Top   , _ScaleOffset)  ;
                float fViewRight  = GetScaleOffsetValX(Rect.Right , _ScaleOffset)  ;
                float fViewBottom = GetScaleOffsetValY(Rect.Bottom, _ScaleOffset)  ;
                float fViewWidth  = fViewRight - fViewLeft ;
                float fViewHeight = fViewBottom - fViewTop ;
                
                float fViewCx = fViewLeft + fViewWidth  / 2.0f;
                float fViewCy = fViewTop  + fViewHeight / 2.0f;

                     if (CMath.GetCircleInPoint(fViewLeft , fViewTop   , _e.X , _e.Y, fPntHfSize)) { MoveStat.eDnType = MoveStat.eLastDnType = EDownType.dtLeftTop     ; }
                else if (CMath.GetCircleInPoint(fViewRight, fViewTop   , _e.X , _e.Y, fPntHfSize)) { MoveStat.eDnType = MoveStat.eLastDnType = EDownType.dtRightTop    ; }
                else if (CMath.GetCircleInPoint(fViewRight, fViewBottom, _e.X , _e.Y, fPntHfSize)) { MoveStat.eDnType = MoveStat.eLastDnType = EDownType.dtRightBottom ; }
                else if (CMath.GetCircleInPoint(fViewLeft , fViewBottom, _e.X , _e.Y, fPntHfSize)) { MoveStat.eDnType = MoveStat.eLastDnType = EDownType.dtLeftBottom  ; }
                
                else if (CheckRectIn(fViewCx   , fViewTop    , _e.X , _e.Y, fRectPntHfSize)) { MoveStat.eDnType = MoveStat.eLastDnType = EDownType.dtCenterTop   ; }
                else if (CheckRectIn(fViewCx   , fViewBottom , _e.X , _e.Y, fRectPntHfSize)) { MoveStat.eDnType = MoveStat.eLastDnType = EDownType.dtCenterBottom; }
                else if (CheckRectIn(fViewLeft , fViewCy     , _e.X , _e.Y, fRectPntHfSize)) { MoveStat.eDnType = MoveStat.eLastDnType = EDownType.dtLeftCenter  ; }
                else if (CheckRectIn(fViewRight, fViewCy     , _e.X , _e.Y, fRectPntHfSize)) { MoveStat.eDnType = MoveStat.eLastDnType = EDownType.dtRightCenter ; }
                
                else if (CheckLineIn(m_aPoints, m_iPointCount, fImgX, fImgY, fRectPntHfSize)) MoveStat.eDnType = MoveStat.eLastDnType = EDownType.dtMove;
                else                                                                          MoveStat.eDnType = MoveStat.eLastDnType = EDownType.dtNone;
            }
            else if (trackerType == ETrackerType.ttLine)
            {
                float fViewStartX = GetScaleOffsetValX(m_aPoints[0].X , _ScaleOffset)  ;
                float fViewStartY = GetScaleOffsetValY(m_aPoints[0].Y , _ScaleOffset)  ;
                float fViewEndX   = GetScaleOffsetValX(m_aPoints[1].X , _ScaleOffset)  ;
                float fViewEndY   = GetScaleOffsetValY(m_aPoints[1].Y , _ScaleOffset)  ;

                     if (CMath.GetCircleInPoint(fViewStartX, fViewStartY, _e.X , _e.Y , fPntHfSize)) { MoveStat.eDnType=MoveStat.eLastDnType= EDownType.dtStart ; }
                else if (CMath.GetCircleInPoint(fViewEndX  , fViewEndY  , _e.X , _e.Y , fPntHfSize)) { MoveStat.eDnType=MoveStat.eLastDnType= EDownType.dtEnd   ; }
                
                else if (CheckLineIn  (m_aPoints , m_iPointCount ,fImgX ,fImgY , fRectPntHfSize)) MoveStat.eDnType =MoveStat.eLastDnType= EDownType.dtMove ;
                else                                                                              MoveStat.eDnType =MoveStat.eLastDnType= EDownType.dtNone ;
            }
            else if (trackerType == ETrackerType.ttCircle || trackerType == ETrackerType.ttEllipse)
            {
                RectangleF Rect = GetRectangle();
                
                float fViewLeft   = GetScaleOffsetValX(Rect.Left  , _ScaleOffset)  ;
                float fViewTop    = GetScaleOffsetValY(Rect.Top   , _ScaleOffset)  ;
                float fViewRight  = GetScaleOffsetValX(Rect.Right , _ScaleOffset)  ;
                float fViewBottom = GetScaleOffsetValY(Rect.Bottom, _ScaleOffset)  ;
                float fViewWidth  = fViewRight  - fViewLeft ;
                float fViewHeight = fViewBottom - fViewTop  ;
                
                float fViewCx = fViewLeft + fViewWidth  / 2.0f;
                float fViewCy = fViewTop  + fViewHeight / 2.0f;

                double dCx = Rect.Left + Rect.Width  / 2.0;  
                double dCy = Rect.Top  + Rect.Height / 2.0;

                     if (CheckRectIn  (fViewCx     , fViewTop    , _e.X , _e.Y , fRectPntHfSize)){MoveStat.eDnType=MoveStat.eLastDnType= EDownType.dtCenterTop    ; }
                else if (CheckRectIn  (fViewCx     , fViewBottom , _e.X , _e.Y , fRectPntHfSize)){MoveStat.eDnType=MoveStat.eLastDnType= EDownType.dtCenterBottom ; }
                else if (CheckRectIn  (fViewLeft   , fViewCy     , _e.X , _e.Y , fRectPntHfSize)){MoveStat.eDnType=MoveStat.eLastDnType= EDownType.dtLeftCenter   ; }
                else if (CheckRectIn  (fViewRight  , fViewCy     , _e.X , _e.Y , fRectPntHfSize)){MoveStat.eDnType=MoveStat.eLastDnType= EDownType.dtRightCenter  ; }
                
                else if(CMath.GetCircleInPoint(dCx, dCy , fImgX, fImgY, Rect.Width + fRectPntHfSize, Rect.Height + fRectPntHfSize )&&
                       !CMath.GetCircleInPoint(dCx, dCy , fImgX, fImgY, Rect.Width - fRectPntHfSize, Rect.Height - fRectPntHfSize )){MoveStat.eDnType=MoveStat.eLastDnType= EDownType.dtMove ;} 

            }
            else if (trackerType == ETrackerType.ttPolygon || trackerType == ETrackerType.ttPolyline)
            {
                for(int i = 0 ; i < m_iPointCount ; i++) 
                {
                    float fx = m_aPoints[i].X;//GetScaleOffsetValX(m_aPoints[i].X , _ScaleOffset)  ;
                    float fy = m_aPoints[i].Y;//GetScaleOffsetValY(m_aPoints[i].Y , _ScaleOffset)  ;
                    if (CMath.GetCircleInPoint(fx, fy, fImgX, fImgY, dPntHfSize)) 
                    {
                        if(_Shift == (Keys.Shift | Keys.Control)) 
                        {
                            DeletePoint(i);
                            break ;
                        }
                        else if(_Shift == Keys.Control) 
                        {
                            InsertPoint(i);
                            break ;
                        }
                    }
                }

                for(int i = 0 ; i < m_iPointCount ; i++){
                    float fx = m_aPoints[i].X;//GetScaleOffsetValX(m_aPoints[i].X , _ScaleOffset)  ;
                    float fy = m_aPoints[i].Y;//GetScaleOffsetValY(m_aPoints[i].Y , _ScaleOffset)  ;
                    if(CMath.GetCircleInPoint(fx, fy, fImgX, fImgY, dPntHfSize)){MoveStat.eDnType=MoveStat.eLastDnType= (EDownType)i ; }
                }
                if(MoveStat.eDnType < (EDownType)0)
                {
                    //나중에 하자..
                    //     if (CheckRectIn  (dLeft  , dTop    , dX , dY , dRectPntHfSize)){MoveStat.eDnType = dtLT   ; }
                    //else if (CheckRectIn  (dRight , dTop    , dX , dY , dRectPntHfSize)){MoveStat.eDnType = dtRT   ; }
                    //else if (CheckRectIn  (dRight , dBottom , dX , dY , dRectPntHfSize)){MoveStat.eDnType = dtRB   ; }
                    //else if (CheckRectIn  (dLeft  , dBottom , dX , dY , dRectPntHfSize)){MoveStat.eDnType = dtLB   ; }
                    //else if (CheckRectIn  (dCx    , dTop    , dX , dY , dRectPntHfSize)){MoveStat.eDnType = dtCT   ; }
                    //else if (CheckRectIn  (dCx    , dBottom , dX , dY , dRectPntHfSize)){MoveStat.eDnType = dtCB   ; }
                    //else if (CheckRectIn  (dLeft  , dCy     , dX , dY , dRectPntHfSize)){MoveStat.eDnType = dtLC   ; }
                    //else if (CheckRectIn  (dRight , dCy     , dX , dY , dRectPntHfSize)){MoveStat.eDnType = dtRC   ; }
                    //else

                    //ssuunn 폴리곤인 테스트용.
                    //Point[] pnts = new Point[m_iPointCount];
                    //for(int i = 0 ; i < m_aPoints.Length ; i++)
                    //{
                    //    pnts[i].X = (int)m_aPoints[i].X;
                    //    pnts[i].Y = (int)m_aPoints[i].Y;
                    //}
                    //if (CMath.IsPntInPolygon(pnts, m_iPointCount, new Point((int)fImgX, (int)fImgY))) MoveStat.eDnType =MoveStat.eLastDnType= EDownType.dtMove ;
                    
                    if (CheckLineIn  (m_aPoints, m_iPointCount, fImgX, fImgY, dPntHfSize)) MoveStat.eDnType =MoveStat.eLastDnType= EDownType.dtMove ;
                    else                                                                   MoveStat.eDnType =MoveStat.eLastDnType= EDownType.dtNone ;
                }
            }

            else if (trackerType == ETrackerType.ttCalibration)
            {
                RectangleF Rect = GetRectangle();
                
                float fViewLeft   = GetScaleOffsetValX(Rect.Left  , _ScaleOffset)  ;
                float fViewTop    = GetScaleOffsetValY(Rect.Top   , _ScaleOffset)  ;
                float fViewRight  = GetScaleOffsetValX(Rect.Right , _ScaleOffset)  ;
                float fViewBottom = GetScaleOffsetValY(Rect.Bottom, _ScaleOffset)  ;
                float fViewWidth  = fViewRight - fViewLeft ;
                float fViewHeight = fViewBottom - fViewTop ;
                
                float fViewCx = fViewLeft + fViewWidth  / 2.0f;
                float fViewCy = fViewTop  + fViewHeight / 2.0f;

                MoveStat.eDnType = MoveStat.eLastDnType = EDownType.dtNone;
                int iPntCnt = m_iPointCount;
                
                
                for (int i = 0; i < m_iPointCount; i++)
                {
                    if (CMath.GetCircleInPoint(m_aPoints[i].X, m_aPoints[i].Y, _e.X, _e.Y, fPntHfSize)) { MoveStat.eDnType = MoveStat.eLastDnType = (EDownType)i; }
                }
                if (MoveStat.eDnType < (EDownType)0)
                {
                         if (CheckRectIn(fViewLeft , fViewTop   , _e.X, _e.Y, fRectPntHfSize)) { MoveStat.eDnType = EDownType.dtLeftTop    ; }
                    else if (CheckRectIn(fViewRight, fViewTop   , _e.X, _e.Y, fRectPntHfSize)) { MoveStat.eDnType = EDownType.dtRightTop   ; }
                    else if (CheckRectIn(fViewRight, fViewBottom, _e.X, _e.Y, fRectPntHfSize)) { MoveStat.eDnType = EDownType.dtRightBottom; }
                    else if (CheckRectIn(fViewLeft , fViewBottom, _e.X, _e.Y, fRectPntHfSize)) { MoveStat.eDnType = EDownType.dtLeftBottom ; }
                    
                    if (CheckLineIn(m_aPoints, m_iPointCount, fImgX, fImgY, fPntHfSize)) MoveStat.eDnType = MoveStat.eLastDnType = EDownType.dtMove;
                    else                                                                 MoveStat.eDnType = MoveStat.eLastDnType = EDownType.dtNone;
                } 
            }
            else if(trackerType == ETrackerType.ttMessage)
            {
                RectangleF Rect = GetRectangle();
                
                float fViewLeft   = GetScaleOffsetValX(Rect.Left  , _ScaleOffset)  ;
                float fViewTop    = GetScaleOffsetValY(Rect.Top   , _ScaleOffset)  ;
                float fViewRight  = GetScaleOffsetValX(Rect.Right , _ScaleOffset)  ;
                float fViewBottom = GetScaleOffsetValY(Rect.Bottom, _ScaleOffset)  ;
                float fViewWidth  = fViewRight - fViewLeft ;
                float fViewHeight = fViewBottom - fViewTop ;
                
                float fViewCx = fViewLeft + fViewWidth  / 2.0f;
                float fViewCy = fViewTop  + fViewHeight / 2.0f;

                     if (CMath.GetCircleInPoint(fViewLeft , fViewTop   , _e.X , _e.Y, fPntHfSize)) { MoveStat.eDnType = MoveStat.eLastDnType = EDownType.dtLeftTop     ; }
                else if (CMath.GetCircleInPoint(fViewRight, fViewTop   , _e.X , _e.Y, fPntHfSize)) { MoveStat.eDnType = MoveStat.eLastDnType = EDownType.dtRightTop    ; }
                else if (CMath.GetCircleInPoint(fViewRight, fViewBottom, _e.X , _e.Y, fPntHfSize)) { MoveStat.eDnType = MoveStat.eLastDnType = EDownType.dtRightBottom ; }
                else if (CMath.GetCircleInPoint(fViewLeft , fViewBottom, _e.X , _e.Y, fPntHfSize)) { MoveStat.eDnType = MoveStat.eLastDnType = EDownType.dtLeftBottom  ; }
                
                else if (CheckRectIn(fViewCx   , fViewTop    , _e.X , _e.Y, fRectPntHfSize)) { MoveStat.eDnType = MoveStat.eLastDnType = EDownType.dtCenterTop   ; }
                else if (CheckRectIn(fViewCx   , fViewBottom , _e.X , _e.Y, fRectPntHfSize)) { MoveStat.eDnType = MoveStat.eLastDnType = EDownType.dtCenterBottom; }
                else if (CheckRectIn(fViewLeft , fViewCy     , _e.X , _e.Y, fRectPntHfSize)) { MoveStat.eDnType = MoveStat.eLastDnType = EDownType.dtLeftCenter  ; }
                else if (CheckRectIn(fViewRight, fViewCy     , _e.X , _e.Y, fRectPntHfSize)) { MoveStat.eDnType = MoveStat.eLastDnType = EDownType.dtRightCenter ; }
                
                else if (CheckLineIn(m_aPoints, m_iPointCount, fImgX, fImgY, fRectPntHfSize)) MoveStat.eDnType = MoveStat.eLastDnType = EDownType.dtMove;
                else                                                                          MoveStat.eDnType = MoveStat.eLastDnType = EDownType.dtNone;
            }
            m_bSelected = MoveStat.eDnType != EDownType.dtNone;
            return m_bSelected;
        }

        public bool MouseMove(Keys Shift, MouseEventArgs _e, TScaleOffset _ScaleOffset)
        {
            if (!visible || !enabled) return false;
            if (MoveStat.bRight) return false;

            float fImgMoveX = GetOriginalValX((int)_e.X , _ScaleOffset) - GetOriginalValX((int)MoveStat.fX , _ScaleOffset) ;
            float fImgMoveY = GetOriginalValY((int)_e.Y , _ScaleOffset) - GetOriginalValY((int)MoveStat.fY , _ScaleOffset) ;

            float fImgX     = GetOriginalValX((int)_e.X , _ScaleOffset);
            float fImgY     = GetOriginalValY((int)_e.Y , _ScaleOffset);

            MoveStat.fX = _e.X ;
            MoveStat.fY = _e.Y ;

            m_bPreInRange = m_bInRange;
            m_bInRange = false;
            if (MoveStat.eDnType == EDownType.dtNone)
            {
                if (trackerType == ETrackerType.ttCircle || trackerType == ETrackerType.ttEllipse)
                {
                    RectangleF Rect = GetRectangle();

                    float fViewLeft   = GetScaleOffsetValX(Rect.Left  , _ScaleOffset)  ;
                    float fViewTop    = GetScaleOffsetValY(Rect.Top   , _ScaleOffset)  ;
                    float fViewRight  = GetScaleOffsetValX(Rect.Right , _ScaleOffset)  ;
                    float fViewBottom = GetScaleOffsetValY(Rect.Bottom, _ScaleOffset)  ;
                    float fViewWidth  = fViewRight  - fViewLeft ;
                    float fViewHeight = fViewBottom - fViewTop ;

                    float fViewCx = fViewLeft + fViewWidth  / 2.0f;
                    float fViewCy = fViewTop  + fViewHeight / 2.0f;

                    double dCx = Rect.Left + Rect.Width /2.0;  
                    double dCy = Rect.Top  + Rect.Height/2.0;  

                    m_bInRange |= (CMath.GetCircleInPoint(dCx, dCy, fImgX, fImgY, Rect.Width + dRectPntHfSize, Rect.Height + dRectPntHfSize)&&
                                  !CMath.GetCircleInPoint(dCx, dCy, fImgX, fImgY, Rect.Width - dRectPntHfSize, Rect.Height - dRectPntHfSize)) ;
                    return m_bPreInRange != m_bInRange;
                }
                else { 
                    m_bInRange |= CheckLineIn(m_aPoints, m_iPointCount, fImgX, fImgY, dPntHfSize + 50);
                    return m_bPreInRange != m_bInRange;
                }
            }

            m_bMoved = true;

            const int iSM = 5 ; //최소크기마진.
            if (trackerType == ETrackerType.ttRect || trackerType == ETrackerType.ttMessage)
            {
                if (MoveStat.eDnType == EDownType.dtLeftTop)
                {
                    if (fImgX >= m_aPoints[(int)EPointId.piRectRightBottom].X - iSM) fImgX = m_aPoints[(int)EPointId.piRectRightBottom].X - iSM;
                    if (fImgY >= m_aPoints[(int)EPointId.piRectRightBottom].Y - iSM) fImgY = m_aPoints[(int)EPointId.piRectRightBottom].Y - iSM;
                    if (fImgX < 0) fImgX = 0; if (fImgY < 0) fImgY = 0;

                    m_aPoints[(int)EPointId.piRectLeftTop   ].X = fImgX; m_aPoints[(int)EPointId.piRectLeftTop].Y = fImgY;
                    m_aPoints[(int)EPointId.piRectLeftBottom].X = fImgX;
                    m_aPoints[(int)EPointId.piRectRightTop  ].Y = fImgY;
                }
                else if (MoveStat.eDnType == EDownType.dtRightTop)
                {
                    if (fImgX <  m_aPoints[(int)EPointId.piRectLeftTop    ].X + iSM) fImgX = m_aPoints[(int)EPointId.piRectLeftTop    ].X + iSM;
                    if (fImgY >= m_aPoints[(int)EPointId.piRectRightBottom].Y - iSM) fImgY = m_aPoints[(int)EPointId.piRectRightBottom].Y - iSM;
                    if (fImgY < 0) fImgY = 0;

                    m_aPoints[(int)EPointId.piRectRightTop   ].X = fImgX; m_aPoints[(int)EPointId.piRectRightTop].Y = fImgY;

                    m_aPoints[(int)EPointId.piRectRightBottom].X = fImgX;
                    m_aPoints[(int)EPointId.piRectLeftTop    ].Y = fImgY;
                }
                else if (MoveStat.eDnType == EDownType.dtLeftBottom)
                {
                    if (fImgX >= m_aPoints[(int)EPointId.piRectRightBottom].X - iSM) fImgX = m_aPoints[(int)EPointId.piRectRightBottom].X - iSM;
                    if (fImgX < 0) fImgX = 0;
                    if (fImgY < m_aPoints[(int)EPointId.piRectLeftTop     ].Y + iSM) fImgY = m_aPoints[(int)EPointId.piRectLeftTop    ].Y + iSM;

                    m_aPoints[(int)EPointId.piRectLeftBottom ].X = fImgX; m_aPoints[(int)EPointId.piRectLeftBottom].Y = fImgY;

                    m_aPoints[(int)EPointId.piRectLeftTop    ].X = fImgX;
                    m_aPoints[(int)EPointId.piRectRightBottom].Y = fImgY;
                }
                else if (MoveStat.eDnType == EDownType.dtRightBottom)
                {
                    if (fImgX < m_aPoints[(int)EPointId.piRectLeftTop].X + iSM) fImgX = m_aPoints[(int)EPointId.piRectLeftTop].X + iSM;
                    if (fImgY < m_aPoints[(int)EPointId.piRectLeftTop].Y + iSM) fImgY = m_aPoints[(int)EPointId.piRectLeftTop].Y + iSM;

                    m_aPoints[(int)EPointId.piRectRightBottom].X = fImgX; m_aPoints[(int)EPointId.piRectRightBottom].Y = fImgY;

                    m_aPoints[(int)EPointId.piRectRightTop   ].X = fImgX;
                    m_aPoints[(int)EPointId.piRectLeftBottom ].Y = fImgY;
                }
                else if (MoveStat.eDnType == EDownType.dtCenterTop)
                {
                    if (fImgY > m_aPoints[(int)EPointId.piRectRightBottom].Y - iSM) fImgY = m_aPoints[(int)EPointId.piRectRightBottom].Y - iSM;
                    if (fImgY < 0) fImgY = 0;
                     
                    m_aPoints[(int)EPointId.piRectLeftTop ].Y = fImgY;
                    m_aPoints[(int)EPointId.piRectRightTop].Y = fImgY;
                }
                else if (MoveStat.eDnType == EDownType.dtCenterBottom)
                {
                    if (fImgY < m_aPoints[(int)EPointId.piRectLeftTop].Y + iSM) fImgY = m_aPoints[(int)EPointId.piRectLeftTop].Y + iSM;

                    m_aPoints[(int)EPointId.piRectLeftBottom ].Y = fImgY;
                    m_aPoints[(int)EPointId.piRectRightBottom].Y = fImgY;
                }
                else if (MoveStat.eDnType == EDownType.dtLeftCenter)
                {
                    if (fImgX > m_aPoints[(int)EPointId.piRectRightBottom].X - iSM) fImgX = m_aPoints[(int)EPointId.piRectRightBottom].X - iSM;
                    if (fImgX < 0) fImgX = 0;

                    m_aPoints[(int)EPointId.piRectLeftTop   ].X = fImgX;
                    m_aPoints[(int)EPointId.piRectLeftBottom].X = fImgX;
                }
                else if (MoveStat.eDnType == EDownType.dtRightCenter)
                {
                    if (fImgX < m_aPoints[(int)EPointId.piRectLeftTop].X + iSM) fImgX = m_aPoints[(int)EPointId.piRectLeftTop].X + iSM;

                    m_aPoints[(int)EPointId.piRectRightTop   ].X = fImgX;
                    m_aPoints[(int)EPointId.piRectRightBottom].X = fImgX;
                }
                else if (MoveStat.eDnType == EDownType.dtMove)
                {
                    m_aPoints[(int)EPointId.piRectLeftTop    ].X += fImgMoveX ; m_aPoints[(int)EPointId.piRectLeftTop    ].Y += fImgMoveY ;
                    m_aPoints[(int)EPointId.piRectRightTop   ].X += fImgMoveX ; m_aPoints[(int)EPointId.piRectRightTop   ].Y += fImgMoveY ;
                    m_aPoints[(int)EPointId.piRectRightBottom].X += fImgMoveX ; m_aPoints[(int)EPointId.piRectRightBottom].Y += fImgMoveY ;
                    m_aPoints[(int)EPointId.piRectLeftBottom ].X += fImgMoveX ; m_aPoints[(int)EPointId.piRectLeftBottom ].Y += fImgMoveY ;

                }
            }
            else if (trackerType == ETrackerType.ttLine)
            {
                if (fImgX < 0) fImgX = 0;
                if (fImgY < 0) fImgY = 0;
                if (MoveStat.eDnType == EDownType.dtStart)
                {
                    m_aPoints[(int)EPointId.piLineStart].X = fImgX;
                    m_aPoints[(int)EPointId.piLineStart].Y = fImgY;
                }
                else if (MoveStat.eDnType == EDownType.dtEnd)
                {
                    m_aPoints[(int)EPointId.piLineEnd].X = fImgX;
                    m_aPoints[(int)EPointId.piLineEnd].Y = fImgY;
                }
                else if (MoveStat.eDnType == EDownType.dtMove)
                {
                    m_aPoints[(int)EPointId.piLineStart].X += fImgMoveX; m_aPoints[(int)EPointId.piLineStart].Y += fImgMoveY;
                    m_aPoints[(int)EPointId.piLineEnd  ].X += fImgMoveX; m_aPoints[(int)EPointId.piLineEnd  ].Y += fImgMoveY;
                }
            }
            else if (trackerType == ETrackerType.ttCircle || trackerType == ETrackerType.ttEllipse)
            {     
                float fWidth  = m_aPoints[(int)EPointId.piRectRightBottom].X - m_aPoints[(int)EPointId.piRectLeftTop].X ;
                float fHeight = m_aPoints[(int)EPointId.piRectRightBottom].Y - m_aPoints[(int)EPointId.piRectLeftTop].Y ;

                float fCx = m_aPoints[(int)EPointId.piRectLeftTop].X + fWidth  / 2.0f;
                float fCy = m_aPoints[(int)EPointId.piRectLeftTop].Y + fHeight / 2.0f;

                if (MoveStat.eDnType == EDownType.dtCenterTop)
                {
                    if (fImgY > m_aPoints[(int)EPointId.piRectRightBottom].Y - iSM) fImgY = m_aPoints[(int)EPointId.piRectRightBottom].Y - iSM;
                    if (fImgY < 0) fImgY = 0;

                    if(trackerType == ETrackerType.ttEllipse)
                    {
                        m_aPoints[(int)EPointId.piRectLeftTop ].Y = fImgY;
                        m_aPoints[(int)EPointId.piRectRightTop].Y = fImgY;
                    }
                    else
                    {
                        float RadiusX = Math.Abs(fCy - fImgY) ;
                        float RadiusY = Math.Abs(fCy - fImgY) ;

                        m_aPoints[(int)EPointId.piRectLeftTop    ].X = fCx - RadiusX; m_aPoints[(int)EPointId.piRectLeftTop    ].Y = fCy - RadiusY;
                        m_aPoints[(int)EPointId.piRectRightTop   ].X = fCx + RadiusX; m_aPoints[(int)EPointId.piRectRightTop   ].Y = fCy - RadiusY;
                        m_aPoints[(int)EPointId.piRectRightBottom].X = fCx + RadiusX; m_aPoints[(int)EPointId.piRectRightBottom].Y = fCy + RadiusY;
                        m_aPoints[(int)EPointId.piRectLeftBottom ].X = fCx - RadiusX; m_aPoints[(int)EPointId.piRectLeftBottom ].Y = fCy + RadiusY;
                    }
                    
                    
                }
                else if (MoveStat.eDnType == EDownType.dtCenterBottom)
                {
                    if (fImgY < m_aPoints[(int)EPointId.piRectLeftTop].Y + iSM) fImgY = m_aPoints[(int)EPointId.piRectLeftTop].Y + iSM;

                    if(trackerType == ETrackerType.ttEllipse)
                    {
                        m_aPoints[(int)EPointId.piRectLeftBottom ].Y = fImgY;
                        m_aPoints[(int)EPointId.piRectRightBottom].Y = fImgY;
                    }
                    else
                    {
                        float RadiusX = Math.Abs(fCy - fImgY) ;
                        float RadiusY = Math.Abs(fCy - fImgY) ;

                        m_aPoints[(int)EPointId.piRectLeftTop    ].X = fCx - RadiusX; m_aPoints[(int)EPointId.piRectLeftTop    ].Y = fCy - RadiusY;
                        m_aPoints[(int)EPointId.piRectRightTop   ].X = fCx + RadiusX; m_aPoints[(int)EPointId.piRectRightTop   ].Y = fCy - RadiusY;
                        m_aPoints[(int)EPointId.piRectRightBottom].X = fCx + RadiusX; m_aPoints[(int)EPointId.piRectRightBottom].Y = fCy + RadiusY;
                        m_aPoints[(int)EPointId.piRectLeftBottom ].X = fCx - RadiusX; m_aPoints[(int)EPointId.piRectLeftBottom ].Y = fCy + RadiusY;
                    }                    
                    
                }
                else if (MoveStat.eDnType == EDownType.dtLeftCenter)
                {
                    if (fImgX > m_aPoints[(int)EPointId.piRectRightBottom].X - iSM) fImgX = m_aPoints[(int)EPointId.piRectRightBottom].X - iSM;
                    if (fImgX < 0) fImgX = 0;

                    if(trackerType == ETrackerType.ttEllipse)
                    {
                        m_aPoints[(int)EPointId.piRectLeftTop   ].X = fImgX;
                        m_aPoints[(int)EPointId.piRectLeftBottom].X = fImgX;
                    }
                    else
                    {
                        float RadiusX = Math.Abs(fCx - fImgX) ;
                        float RadiusY = Math.Abs(fCx - fImgX) ;

                        m_aPoints[(int)EPointId.piRectLeftTop    ].X = fCx - RadiusX; m_aPoints[(int)EPointId.piRectLeftTop    ].Y = fCy - RadiusY;
                        m_aPoints[(int)EPointId.piRectRightTop   ].X = fCx + RadiusX; m_aPoints[(int)EPointId.piRectRightTop   ].Y = fCy - RadiusY;
                        m_aPoints[(int)EPointId.piRectRightBottom].X = fCx + RadiusX; m_aPoints[(int)EPointId.piRectRightBottom].Y = fCy + RadiusY;
                        m_aPoints[(int)EPointId.piRectLeftBottom ].X = fCx - RadiusX; m_aPoints[(int)EPointId.piRectLeftBottom ].Y = fCy + RadiusY;
                    }
                }
                else if (MoveStat.eDnType == EDownType.dtRightCenter)
                {
                    if (fImgX < m_aPoints[(int)EPointId.piRectLeftTop].X + iSM) fImgX = m_aPoints[(int)EPointId.piRectLeftTop].X + iSM;

                    if(trackerType == ETrackerType.ttEllipse)
                    {
                        m_aPoints[(int)EPointId.piRectRightTop   ].X = fImgX;
                        m_aPoints[(int)EPointId.piRectRightBottom].X = fImgX;
                    }
                    else
                    {
                        float RadiusX = Math.Abs(fCx - fImgX) ;
                        float RadiusY = Math.Abs(fCx - fImgX) ;

                        m_aPoints[(int)EPointId.piRectLeftTop    ].X = fCx - RadiusX; m_aPoints[(int)EPointId.piRectLeftTop    ].Y = fCy - RadiusY;
                        m_aPoints[(int)EPointId.piRectRightTop   ].X = fCx + RadiusX; m_aPoints[(int)EPointId.piRectRightTop   ].Y = fCy - RadiusY;
                        m_aPoints[(int)EPointId.piRectRightBottom].X = fCx + RadiusX; m_aPoints[(int)EPointId.piRectRightBottom].Y = fCy + RadiusY;
                        m_aPoints[(int)EPointId.piRectLeftBottom ].X = fCx - RadiusX; m_aPoints[(int)EPointId.piRectLeftBottom ].Y = fCy + RadiusY;
                    }
                }
                else if (MoveStat.eDnType == EDownType.dtMove)
                {
                    m_aPoints[(int)EPointId.piRectLeftTop    ].X += fImgMoveX ; m_aPoints[(int)EPointId.piRectLeftTop    ].Y += fImgMoveY ;
                    m_aPoints[(int)EPointId.piRectRightTop   ].X += fImgMoveX ; m_aPoints[(int)EPointId.piRectRightTop   ].Y += fImgMoveY ;
                    m_aPoints[(int)EPointId.piRectRightBottom].X += fImgMoveX ; m_aPoints[(int)EPointId.piRectRightBottom].Y += fImgMoveY ;
                    m_aPoints[(int)EPointId.piRectLeftBottom ].X += fImgMoveX ; m_aPoints[(int)EPointId.piRectLeftBottom ].Y += fImgMoveY ;

                }
            }
            else if (trackerType == ETrackerType.ttPolygon || trackerType == ETrackerType.ttPolyline || trackerType == ETrackerType.ttCalibration)
            {
                if (MoveStat.eDnType >= 0)
                {
                    m_aPoints[(int)((EPointId)MoveStat.eDnType)].X = fImgX; 
                    m_aPoints[(int)((EPointId)MoveStat.eDnType)].Y = fImgY; 
                }
                else if (MoveStat.eDnType == EDownType.dtMove)
                {
                    for (int i = 0; i < m_iPointCount; i++)
                    {
                        m_aPoints[i].X += fImgMoveX;
                        m_aPoints[i].Y += fImgMoveY;
                    }
                }
            }

            return m_bSelected;
        }

        public bool MouseUp(Keys _Shift, MouseEventArgs _e)
        {
            bool bRet = (MoveStat.eDnType != EDownType.dtNone && MoveStat.bRight);
            MoveStat.eDnType = EDownType.dtNone;
            return bRet;
        }

        public void LineAllow(Graphics _g, float _fX1, float _fY1, float _fX2, float _fY2)
        {
            double Length = 10;
            double r2;
            double r3 = 15;

            if (_fX2 > _fX1) r2 = (180.0 * Math.Atan((double)(_fY2 - _fY1) / (double)(_fX1 - _fX2))) / 3.141519;
            else if (_fX2 < _fX1) r2 = ((180.0 * Math.Atan((double)(_fY2 - _fY1) / (double)(_fX1 - _fX2))) / 3.141519) + 180;
            else
            {
                if (_fY2 < _fY1) r2 = 90;
                else r2 = -90;
            }

            PointF a, b;
            a = new PointF(_fX2 - (float)(Length * Math.Cos((3.141519 * (r2 + r3)) / 180)),
                           _fY2 + (float)(Length * Math.Sin((3.141519 * (r2 + r3)) / 180)));

            b = new PointF(_fX2 - (float)(Length * Math.Cos((3.141519 * (r2 - r3)) / 180)),
                           _fY2 + (float)(Length * Math.Sin((3.141519 * (r2 - r3)) / 180)));

            PointF[] Points = new PointF[3];
            Points[0].X = _fX2;
            Points[0].Y = _fY2;
            Points[1] = a;
            Points[2] = b;

            using (Pen P = new Pen(Color.Black))
            {
                _g.DrawLine(P, _fX1, _fY1, _fX2, _fY2);
            }
            using (SolidBrush B = new SolidBrush(Color.Black))
            {
                _g.FillPolygon(B, Points);
            }
        }

        public PointF FindCalPoint(int _Col, int _Row)
        {
            PointF Rslt = new PointF();

            PointF[] Left   = new PointF[7];
            PointF[] Right  = new PointF[7];
            PointF[] Top    = new PointF[7];
            PointF[] Bottom = new PointF[7];

            Left  [0] = Top   [0] = m_aPoints[0];
            Top   [6] = Right [0] = m_aPoints[1];
            Right [6] = Bottom[6] = m_aPoints[2];
            Bottom[0] = Left  [6] = m_aPoints[3]; 
            for(int i = 1; i < 6; i++)
            {
                Left  [i] = CalLeft  [i - 1];
                Right [i] = CalRight [i - 1];
                Top   [i] = CalTop   [i - 1];
                Bottom[i] = CalBottom[i - 1];
            }

            Rslt.X = ((((Left[_Row].X * Right[_Row].Y) - (Left[_Row].Y * Right[_Row].X)) * (Bottom[_Col].X - Top[_Col].X)) -
                       ((Left[_Row].X - Right[_Row].X) * ((Bottom[_Col].X * Top[_Col].Y) - (Bottom[_Col].Y * Top[_Col].X)))) /
                     (((Left[_Row].X - Right[_Row].X) * (Bottom[_Col].Y - Top[_Col].Y)) - 
                      ((Left[_Row].Y - Right[_Row].Y) * (Bottom[_Col].X - Top[_Col].X)));

            Rslt.Y = ((((Left[_Row].X * Right[_Row].Y) - (Left[_Row].Y * Right[_Row].X)) * (Bottom[_Col].Y - Top[_Col].Y)) -
                       ((Left[_Row].Y - Right[_Row].Y) * ((Bottom[_Col].X * Top[_Col].Y) - (Bottom[_Col].Y * Top[_Col].X)))) /
                     (((Left[_Row].X - Right[_Row].X) * (Bottom[_Col].Y - Top[_Col].Y)) -
                      ((Left[_Row].Y - Right[_Row].Y) * (Bottom[_Col].X - Top[_Col].X)));

            return Rslt;
        }

        //Load Save.
        public void LoadSave(bool _bLoad, string _sFilePath)
        {
            string sExeFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            CIniFile TrackerIni = new CIniFile(_sFilePath);

            int iTemp;

            if (_bLoad)
            {
                TrackerIni.Load("Tracker", "trackerType"   , out iTemp        ); if (iTemp != (int)ETrackerType.ttNone) trackerType = (ETrackerType)iTemp; //처음 생성하고 로딩하면서 타입이 바뀌는 문제땜에.
                TrackerIni.Load("Tracker", "roiWidth"      , out m_dRoiWidth  ); if (roiWidth <= 1) roiWidth = 1;
                TrackerIni.Load("Tracker", "m_iPointCount" , out m_iPointCount);
                if (m_iPointCount == 0)
                {
                         if (trackerType == ETrackerType.ttCircle) { m_iPointCount = 1; }
                    else if (trackerType == ETrackerType.ttLine  ) { m_iPointCount = 2; }
                    else { m_iPointCount = 4; }
                }

                m_aPoints = null;
                m_aPoints = new PointF[m_iPointCount];
                float iPointsX ;
                float iPointsY ;
                bool bNew = true;
                for (int i = 0; i < m_iPointCount; i++)
                {
                    iPointsX = m_aPoints[i].X;
                    iPointsY = m_aPoints[i].Y;
                    TrackerIni.Load("Tracker", "m_aPoints[" + i.ToString() + "].X", out iPointsX);
                    TrackerIni.Load("Tracker", "m_aPoints[" + i.ToString() + "].Y", out iPointsY);
                    if (iPointsX > 0 && iPointsY > 0) bNew = false;
                    m_aPoints[i].X = iPointsX;
                    m_aPoints[i].Y = iPointsY;
                    //
                }

                if (bNew)
                {
                    if (trackerType == ETrackerType.ttLine)
                    {
                        m_aPoints[(int)EPointId.piLineStart].X = 10;
                        m_aPoints[(int)EPointId.piLineStart].Y = 10;

                        m_aPoints[(int)EPointId.piLineEnd].X = 90;
                        m_aPoints[(int)EPointId.piLineEnd].Y = 90;
                    }
                    else
                    {
                        m_aPoints[(int)EPointId.piRectLeftTop].X = 10; m_aPoints[(int)EPointId.piRectRightTop].X = 90;
                        m_aPoints[(int)EPointId.piRectLeftTop].Y = 10; m_aPoints[(int)EPointId.piRectRightTop].Y = 10;

                        m_aPoints[(int)EPointId.piRectLeftBottom].X = 10; m_aPoints[(int)EPointId.piRectRightBottom].X = 90;
                        m_aPoints[(int)EPointId.piRectLeftBottom].Y = 90; m_aPoints[(int)EPointId.piRectRightBottom].Y = 90;
                    }
                }
            }
            else
            {
                iTemp = (int)trackerType;
                TrackerIni.Save("Tracker", "trackerType"  , iTemp        );
                TrackerIni.Save("Tracker", "roiWidth"     , roiWidth     );
                TrackerIni.Save("Tracker", "m_iPointCount", m_iPointCount);

                for (int i = 0; i < m_iPointCount; i++)
                {
                    TrackerIni.Save("Tracker",  "m_aPoints[" + i.ToString() + "].X", m_aPoints[i].X);
                    TrackerIni.Save("Tracker", "m_aPoints["  + i.ToString() + "].Y", m_aPoints[i].Y);
                }
            }

            m_bMoved = false;

        }

        public void LoadSave(bool _bLoad, string _sCaption , CIniFile TrackerIni)
        {
            int iTemp;
            if (_bLoad)
            {
                TrackerIni.Load(_sCaption, "trackerType", out iTemp); if (iTemp != (int)ETrackerType.ttNone) trackerType = (ETrackerType)iTemp; //처음 생성하고 로딩하면서 타입이 바뀌는 문제땜에.
                TrackerIni.Load(_sCaption, "roiWidth", out m_dRoiWidth); if (roiWidth <= 1) roiWidth = 1;
                TrackerIni.Load(_sCaption, "m_iPointCount", out m_iPointCount);
                if (m_iPointCount == 0)
                {
                    if (trackerType == ETrackerType.ttCircle) { m_iPointCount = 1; }
                    else if (trackerType == ETrackerType.ttLine) { m_iPointCount = 2; }
                    else { m_iPointCount = 4; }
                }

                m_aPoints = null;
                m_aPoints = new PointF[m_iPointCount];
                float iPointsX ;
                float iPointsY ;
                bool bNew = true;
                for (int i = 0; i < m_iPointCount; i++)
                {
                    iPointsX = m_aPoints[i].X;
                    iPointsY = m_aPoints[i].Y;
                    TrackerIni.Load(_sCaption, "m_aPoints[" + i.ToString() + "].X", out iPointsX);
                    TrackerIni.Load(_sCaption, "m_aPoints[" + i.ToString() + "].Y", out iPointsY);
                    if (iPointsX > 0 && iPointsY > 0) bNew = false;
                    m_aPoints[i].X = iPointsX;
                    m_aPoints[i].Y = iPointsY;
                    //
                }

                if (bNew)
                {
                    if (trackerType == ETrackerType.ttLine)
                    {
                        m_aPoints[(int)EPointId.piLineStart].X = 10;
                        m_aPoints[(int)EPointId.piLineStart].Y = 10;

                        m_aPoints[(int)EPointId.piLineEnd].X = 90;
                        m_aPoints[(int)EPointId.piLineEnd].Y = 90;
                    }
                    else
                    {
                        m_aPoints[(int)EPointId.piRectLeftTop].X = 10; m_aPoints[(int)EPointId.piRectRightTop].X = 90;
                        m_aPoints[(int)EPointId.piRectLeftTop].Y = 10; m_aPoints[(int)EPointId.piRectRightTop].Y = 10;

                        m_aPoints[(int)EPointId.piRectLeftBottom].X = 10; m_aPoints[(int)EPointId.piRectRightBottom].X = 90;
                        m_aPoints[(int)EPointId.piRectLeftBottom].Y = 90; m_aPoints[(int)EPointId.piRectRightBottom].Y = 90;
                    }
                }
            }
            else
            {
                iTemp = (int)trackerType;
                TrackerIni.Save(_sCaption, "trackerType", iTemp);
                TrackerIni.Save(_sCaption, "roiWidth", roiWidth);
                TrackerIni.Save(_sCaption, "m_iPointCount", m_iPointCount);

                for (int i = 0; i < m_iPointCount; i++)
                {
                    TrackerIni.Save(_sCaption, "m_aPoints[" + i.ToString() + "].X", m_aPoints[i].X);
                    TrackerIni.Save(_sCaption, "m_aPoints[" + i.ToString() + "].Y", m_aPoints[i].Y);
                }
            }

            m_bMoved = false;

        }





    }
}
 