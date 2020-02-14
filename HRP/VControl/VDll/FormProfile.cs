using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;

using VDll.Pakage;
using COMMON;

namespace VDll
{
    public partial class FormProfile : Form
    {
        private Mat       Image     ;
        private CTracker  Tracker   ;
        private Point     SelPnt1   ;
        private Point     SelPnt2   ;
        private Point     LastSelPnt;
        private Rectangle Canvas    ;

        private bool      IsSelVer1 ;
        private bool      IsSelHor1 ;
        private bool      IsSelVer2 ;
        private bool      IsSelHor2 ;
        public FormProfile(Mat _mat, CTracker _Tracker)
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            Image = _mat;
            Tracker = _Tracker;
            Tracker.visible = true;
            
            TopMost = true;

            //Tracker.trackerType = CTracker.ETrackerType.ttLine;
            for (int i = 0; i < GV.Para.VisionCnt; i++)
            {
                GV.FrmVisions[i].PaintInvoke();
            }

            if (tbAvrRange.Text == "") tbAvrRange.Text = "1";
            cbRaw.Checked = true;
            cbDif.Checked = true;

            SelPnt1.X = 80;
            SelPnt1.Y = 80;
            SelPnt2.X = 90;
            SelPnt2.Y = 90;

            IsSelVer1 = false;
            IsSelHor1 = false;
            IsSelVer2 = false;
            IsSelHor2 = false;

            LastSelPnt = SelPnt1;
        }

        private void FormProfile_FormClosing(object sender, FormClosingEventArgs e)
        {
            Tracker.visible = false;
            for (int i = 0; i < GV.Para.VisionCnt; i++)
            {
                GV.FrmVisions[i].PaintInvoke();
            }
        }

        //Paint 이벤트에 Invalidate 넣어서 실시간으로 바뀌게하면
        //폼 생성할때 이미지박스 외의 다른 컨트롤들이 이상하게 생성되서
        //인보크 만들어서 Vision 폼의 MouseMove 이벤트에 넣어서 쓴다.
        delegate void FInvalidateImageBox();
        public void ProfilePaintInvoke() //일단 외부 억세스 막고.  이벤트 방식에서 다시 바꿈.
        {
            if (ibProfile.InvokeRequired) // Invoke가 필요하면
            {
                ibProfile.Invoke(new FInvalidateImageBox(ibProfile.Invalidate), new object[] { }); // 대리자를 호출
            }
            else
            {
                ibProfile.Invalidate();
            }
        }

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
        public int GetPixelRed(Mat mat, int x, int y)
        {
            byte red = 0;

            int c = mat.Cols;
            int e = mat.ElementSize;
            unsafe
            {
                byte* Data = (byte*)mat.DataPointer;
                
                red = *(Data + (y * c + x) * e + 2);
            }
            return red;
        }
        public int GetPixelGreen(Mat mat, int x, int y)
        {
            byte green = 0;

            int c = mat.Cols;
            int e = mat.ElementSize;
            unsafe
            {
                byte* Data = (byte*)mat.DataPointer;

                green = *(Data + (y * c + x) * e + 1);
            }
            return green;
        }
        public int GetPixelBlue(Mat mat, int x, int y)
        {
            byte blue = 0;

            int c = mat.Cols;
            int e = mat.ElementSize;
            unsafe
            {
                byte* Data = (byte*)mat.DataPointer;

                blue = *(Data + (y * c + x) * e + 0);
            }
            return blue;
        }

        private void ibProfile_Paint(object sender, PaintEventArgs e)
        {
            
            const int iCanvasOffset = 40;
            if (ibProfile.Width  <= iCanvasOffset) return;
            if (ibProfile.Height <= iCanvasOffset) return;
            if (Tracker == null) return;
            if (!Visible) return;
            if (Image == null) return;

            int AvrRange = Math.Abs(CConfig.StrToIntDef(tbAvrRange.Text, 1));
            
            Graphics g = e.Graphics;
            
            //canvas크기.
            Canvas = new Rectangle();
            Canvas.X      = iCanvasOffset;
            Canvas.Y      = iCanvasOffset;
            Canvas.Width  = ibProfile.Width  - (iCanvasOffset * 2);
            Canvas.Height = ibProfile.Height - (iCanvasOffset * 2);
            
            //라인 트랙커 시작 위치, 끝위치
            int iStart = (int)CTracker.EPointId.piLineStart;
            int iEnd   = (int)CTracker.EPointId.piLineEnd  ;
            
            Point StartPoint = new Point();
            Point EndPoint   = new Point();
            StartPoint.X = (int)Tracker.GetPointX(iStart); StartPoint.Y = (int)Tracker.GetPointY(iStart);
            EndPoint  .X = (int)Tracker.GetPointX(iEnd  ); EndPoint  .Y = (int)Tracker.GetPointY(iEnd  );
            
            ////나중에 벽면과의 교점 구해서 적용 하게 SetPoint 수정하자.
            //m_pImage->SetPoint(&sp);
            //m_pImage->SetPoint(&ep);
            
            //X축이 크면 펫.
            bool IsPat = Math.Abs(EndPoint.X - StartPoint.X) >= Math.Abs(EndPoint.Y - StartPoint.Y);
            
            Point Temp;
            if (IsPat)
            { //넙적한놈.
                if (EndPoint.X < StartPoint.X)
                { //그래프에 넣으려면 왼쪽을 기준으로 바꿔준다.
                    Temp       = EndPoint  ;
                    EndPoint   = StartPoint;
                    StartPoint = Temp      ;
                }
            }
            else
            {
                if (EndPoint.Y < StartPoint.Y)
                { //그래프에 위쪽을 기준으로 바꾼다.
                    Temp       = EndPoint  ;
                    EndPoint   = StartPoint;
                    StartPoint = Temp      ;
                }
            }
            
            //장축이 눈금 기준 X
            //단축이 눈금 기준 Y
            int DataCnt = IsPat ? EndPoint  .X - StartPoint.X : EndPoint  .Y - StartPoint.Y;
            int StartX  = IsPat ? StartPoint.X                : StartPoint.Y               ;
            int EndX    = IsPat ? EndPoint  .X                : EndPoint  .Y               ;
            int StartY  = IsPat ? StartPoint.Y                : StartPoint.X               ;
            int EndY    = IsPat ? EndPoint  .Y                : EndPoint  .X               ;
            
            bool ShowRaw = cbRaw.Checked;
            bool ShowAvr = cbAvr.Checked;
            bool ShowDif = cbDif.Checked;
            bool ShowVer = cbVer.Checked;
            bool ShowHor = cbHor.Checked;
            
            int SumCnt;
            int Sum;
            
            //Set Fomula of Line.
            double a;
            double b;
            if (EndX - StartX > 0) a = (EndY - StartY) / (double)(EndX - StartX);
            else a = (EndY - StartY) / 0.0000000000001;
            b = StartY - a * StartX;
            
            
            //Graph Value.
            int[] PixVal = new int[DataCnt];
            int[] AvrVal = new int[DataCnt];
            int[] DifVal = new int[DataCnt];
            
            int MaxPix = 0;
            int MinPix = 256;
            int MidPix = 256 / 2;
            
            int y;
            if(Image.ElementSize == 3) //컬러 이미지
            {
            
            }
            else //흑백 이미지
            {
                for (int i = (int)StartX; i < EndX; i++)
                {
                    y = (int)(a * i + b);
                    if (IsPat)
                    {
                        PixVal[i - (int)StartX] = GetPixelGray(Image, i, y);
                        if (MaxPix < GetPixelGray(Image, i, y)) MaxPix = GetPixelGray(Image, i, y);
                        if (MinPix > GetPixelGray(Image, i, y)) MinPix = GetPixelGray(Image, i, y);
                    }
                    else
                    {
                        PixVal[i - (int)StartX] = GetPixelGray(Image, y, i);
                        if (MaxPix < GetPixelGray(Image, y, i)) MaxPix = GetPixelGray(Image, y, i);
                        if (MinPix > GetPixelGray(Image, y, i)) MinPix = GetPixelGray(Image, y, i);
                    }
                }
            }
            
            MidPix = (MaxPix + MinPix) / 2;
            
            for (int i = 0; i < DataCnt; i++)
            {
                SumCnt = 0;
                Sum = 0;
                for (int j = -AvrRange; j < AvrRange; j++)
                {
                    if ((i + j) <  0      ) continue;
                    if ((i + j) >= DataCnt) continue;
                     Sum += PixVal[i + j];
                    SumCnt++;
                }
                AvrVal[i] = SumCnt > 0 ? Sum / SumCnt : PixVal[i];
                DifVal [i] = (i - 1) > 0 ? AvrVal[i] - AvrVal[i - 1] + MidPix : MidPix;
            }
            
            //Display.
            //==========================================================================
            // Create buffer with memory DC and bitmap, then clear it with background.
            
            int iViewWidth  = ibProfile.Width ;
            int iViewHeight = ibProfile.Height;
            
            const int iFontSize = 10;
            const int OneByte = 256;
            //Gdi->m_tBrush.Color = PPxView->Color;
            //Gdi->m_tPen.Color = PPxView->Color;
            //그래프 사각 박스 만든다.
            int iTemp;
            using (Pen pen = new Pen(Color.Silver))
            {
                using (SolidBrush brush = new SolidBrush(Color.DarkGray))
                {
                    g.DrawRectangle(pen, Canvas.X, Canvas.Y, Canvas.Width, Canvas.Height);
                    using (Font font = new Font("Arial", iFontSize))
                    {    
                        for (int i = Canvas.Bottom; i > Canvas.Top; i -= (font.Height + 5))
                        {
                            iTemp = OneByte * (Canvas.Bottom - i) / (Canvas.Bottom - Canvas.Top);
                            pen.Color = Color.Silver;
                            g.DrawLine(pen, Canvas.Left - 5, i, Canvas.Left, i);
                            brush.Color = Color.Yellow;
                            if (cbVer.Checked) g.DrawLine(pen, Canvas.Left, i, Canvas.Right, i);
                            g.DrawString(iTemp.ToString(), font, brush, Canvas.Left - 25, (i - font.Height / 2));
                        }
                        
                        for (int i = Canvas.Left; i < Canvas.Right; i += 31)
                        {
                            iTemp = (int)(DataCnt * (i - Canvas.Left) / (Canvas.Right - Canvas.Left) + StartX);
                            int iFontWidth = (int)g.MeasureString(iTemp.ToString(), font).Width;
                            g.DrawLine(pen, i, Canvas.Bottom, i, Canvas.Bottom + 16);
                            if (cbHor.Checked) g.DrawLine(pen, i, Canvas.Top, i, Canvas.Bottom);
                            g.DrawString(iTemp.ToString(), font, brush, (i - iFontWidth / 2), Canvas.Bottom + 20);
                        }
                        
                        pen.Color = Color.White;
                        g.DrawLine(pen, Canvas.Left, Canvas.Top   , Canvas.Left , Canvas.Bottom); 
                        g.DrawLine(pen, Canvas.Left, Canvas.Bottom, Canvas.Right, Canvas.Bottom); 
                        
                        
                        //scale.
                        bool IsEdge;
                        string Gray;
                        int DeGray;
                        int ONE_BYTE = 256;
                        
                        double RatioX;
                        double RatioY;

                        //Rectangle 그릴때 계산 너무 길어서 변수에 넣어 쓴다. 진섭
                        int iLeft  ;
                        int iTop   ;
                        int iWidth ;
                        int iHeight;
                        for (int k = 1; k < DataCnt - 1; k++)
                        {
                            RatioX = (Canvas.Right  - Canvas.Left) / (double)DataCnt ;
                            RatioY = (Canvas.Bottom - Canvas.Top ) / (double)ONE_BYTE;
                       
                            if(Image.ElementSize == 3) //컬러 이미지
                            {
                            
                            }
                            else //흑백 이미지
                            {
                                pen.Color = Color.Blue;
                                if (ShowRaw) g.DrawLine(pen, (int)(Canvas.Left +  k      * RatioX), (int)(Canvas.Bottom - PixVal[k    ] * RatioY),
                                                             (int)(Canvas.Left + (k - 1) * RatioX), (int)(Canvas.Bottom - PixVal[k - 1] * RatioY));
                                pen.Color = Color.Red;
                                if (ShowAvr) g.DrawLine(pen, (int)(Canvas.Left +  k      * RatioX), (int)(Canvas.Bottom - AvrVal[k    ] * RatioY),
                                                             (int)(Canvas.Left + (k - 1) * RatioX), (int)(Canvas.Bottom - AvrVal[k - 1] * RatioY));
                                pen.Color = Color.Fuchsia;
                                if (ShowDif) g.DrawLine(pen, (int)(Canvas.Left +  k      * RatioX), (int)(Canvas.Bottom - DifVal[k    ] * RatioY),
                                                             (int)(Canvas.Left + (k - 1) * RatioX), (int)(Canvas.Bottom - DifVal[k - 1] * RatioY));
                                
                                string sHexOutput = string.Format("{0:X}", PixVal[k]);
                                Gray = "#" + sHexOutput + sHexOutput + sHexOutput;
                                DeGray = CConfig.StrToIntDef(Gray, 0);

                                //바닥에 검정 흰색 그리는거
                                pen.Color = Color.FromArgb(PixVal[k], PixVal[k], PixVal[k]);
                                g.DrawLine(pen, (int)(Canvas.Left + k * RatioX), Canvas.Bottom + 1, (int)(Canvas.Left + (k - 1) * RatioX), Canvas.Bottom + 1);
                                g.DrawLine(pen, (int)(Canvas.Left + k * RatioX), Canvas.Bottom + 2, (int)(Canvas.Left + (k - 1) * RatioX), Canvas.Bottom + 2);
                                g.DrawLine(pen, (int)(Canvas.Left + k * RatioX), Canvas.Bottom + 3, (int)(Canvas.Left + (k - 1) * RatioX), Canvas.Bottom + 3);
                                g.DrawLine(pen, (int)(Canvas.Left + k * RatioX), Canvas.Bottom + 4, (int)(Canvas.Left + (k - 1) * RatioX), Canvas.Bottom + 4);
                                g.DrawLine(pen, (int)(Canvas.Left + k * RatioX), Canvas.Bottom + 5, (int)(Canvas.Left + (k - 1) * RatioX), Canvas.Bottom + 5);
                                
                                pen.Color = PixVal[k] > ONE_BYTE - (int)(ONE_BYTE / (double)Canvas.Height * (LastSelPnt.Y - iCanvasOffset)) ? Color.White : Color.Black;
                                g.DrawLine(pen, (int)(Canvas.Left + k * RatioX), Canvas.Bottom + 6 , (int)(Canvas.Left + (k - 1) * RatioX), Canvas.Bottom + 6);
                                g.DrawLine(pen, (int)(Canvas.Left + k * RatioX), Canvas.Bottom + 7 , (int)(Canvas.Left + (k - 1) * RatioX), Canvas.Bottom + 7);
                                g.DrawLine(pen, (int)(Canvas.Left + k * RatioX), Canvas.Bottom + 8 , (int)(Canvas.Left + (k - 1) * RatioX), Canvas.Bottom + 8);
                                g.DrawLine(pen, (int)(Canvas.Left + k * RatioX), Canvas.Bottom + 9 , (int)(Canvas.Left + (k - 1) * RatioX), Canvas.Bottom + 9);
                                g.DrawLine(pen, (int)(Canvas.Left + k * RatioX), Canvas.Bottom + 10, (int)(Canvas.Left + (k - 1) * RatioX), Canvas.Bottom + 10);
                                
                                //Dif1Edge
                                if ((k - 1) < 0 || (k + 1) > DataCnt) continue;
                                pen.Color = Color.Fuchsia;
                                IsEdge = (DifVal[k + 1] < DifVal[k] && DifVal[k - 1] < DifVal[k]) || //(Df1Val[k+1] < Df1Val[k] && Df1Val[k-1] <= Df1Val[k]) ||
                                         (DifVal[k + 1] > DifVal[k] && DifVal[k - 1] > DifVal[k]);//|| (Df1Val[k+1] > Df1Val[k] && Df1Val[k-1] >= Df1Val[k]) ;
                                if (IsEdge && ShowDif)
                                {    
                                    brush.Color = Color.Red;
                                    iLeft   = Canvas.Left    + (int)(k         * RatioX) - 2;
                                    iTop    = Canvas.Bottom  - (int)(DifVal[k] * RatioY) - 2;
                                    iWidth  = (Canvas.Left   + (int)(k         * RatioX) + 2) - iLeft;
                                    iHeight = (Canvas.Bottom - (int)(DifVal[k] * RatioY) + 2) - iTop ;

                                    g.FillRectangle(brush, iLeft, iTop, iWidth, iHeight);
                                    //g.DrawRectangle(pen, iLeft, iTop, iWidth, iHeight);
                                }
                            }
                        }
                       
                       
                        //SelPoint1
                        pen.Color = Color.Lime;
                        g.DrawLine(pen, Canvas.Left, SelPnt1.Y, Canvas.Right, SelPnt1.Y); // Hor Bar
                        g.DrawLine(pen, SelPnt1.X, Canvas.Top, SelPnt1.X, Canvas.Bottom); // Ver Bar
                        g.DrawString((ONE_BYTE - (int)(ONE_BYTE / (double)Canvas.Height * (SelPnt1.Y - iCanvasOffset))).ToString(), font, brush, Canvas.Right - 25, SelPnt1.Y - font.Height - 2);
                        g.DrawString((DataCnt * (SelPnt1.X - Canvas.Left) / (Canvas.Right - Canvas.Left) + StartX).ToString(), font, brush, SelPnt1.X - 25, Canvas.Top +5);
                        
                        //SelPoint2
                        pen.Color = Color.Yellow;
                        g.DrawLine(pen, Canvas.Left, SelPnt2.Y, Canvas.Right, SelPnt2.Y); // Hor Bar
                        g.DrawLine(pen, SelPnt2.X, Canvas.Top, SelPnt2.X, Canvas.Bottom); // Ver Bar
                        g.DrawString((ONE_BYTE - (int)(ONE_BYTE / (double)Canvas.Height * (SelPnt2.Y - iCanvasOffset))).ToString(), font, brush, Canvas.Right - 25, SelPnt2.Y - font.Height - 2);
                        g.DrawString((DataCnt * (SelPnt2.X - Canvas.Left) / (Canvas.Right - Canvas.Left) + StartX).ToString(), font, brush, SelPnt2.X - 25, Canvas.Top +5);

                        pen.Color = Color.Blue;
                        g.DrawLine(pen, LastSelPnt.X - 1, Canvas.Top, LastSelPnt.X - 1, Canvas.Bottom);
                        g.DrawLine(pen, LastSelPnt.X + 1, Canvas.Top, LastSelPnt.X + 1, Canvas.Bottom);
                        g.DrawLine(pen, Canvas.Left, LastSelPnt.Y - 1, Canvas.Right, LastSelPnt.Y - 1);
                        g.DrawLine(pen, Canvas.Left, LastSelPnt.Y + 1, Canvas.Right, LastSelPnt.Y + 1);
                       
                        //y Diffrence.
                        //Gdi->m_tBrush.Color = (TColor)0x00363636;
                        pen.Color = Color.Silver;
                        g.DrawLine(pen, Canvas.Right, SelPnt1.Y, Canvas.Right + 5, SelPnt1.Y);
                        g.DrawLine(pen, Canvas.Right, SelPnt2.Y, Canvas.Right + 5, SelPnt2.Y);
                        g.DrawLine(pen, Canvas.Right + 5, SelPnt1.Y, Canvas.Right + 5, SelPnt2.Y);
                        iLeft   =  Canvas.Right + 9;
                        iTop    = ((SelPnt1.Y + SelPnt2.Y) / 2) - (font.Height/2);
                        iWidth  = (Canvas.Right + 35) - iLeft;
                        iHeight = (((SelPnt1.Y + SelPnt2.Y) / 2) + (font.Height/2)) - iTop;
                        g.DrawRectangle(pen, iLeft, iTop, iWidth, iHeight);
                        g.DrawString(((int)(ONE_BYTE / (double)Canvas.Height * Math.Abs(SelPnt1.Y - SelPnt2.Y))).ToString(), font, brush, Canvas.Right + 9, (SelPnt1.Y + SelPnt2.Y) / 2 - font.Height / 2);

                        //x Diffrence.
                        //Gdi->m_tBrush.Color = (TColor)0x00363636;
                        pen.Color = Color.Silver;
                        g.DrawLine(pen, SelPnt1.X, Canvas.Top, SelPnt1.X, Canvas.Top - 5);
                        g.DrawLine(pen, SelPnt2.X, Canvas.Top, SelPnt2.X, Canvas.Top - 5);
                        g.DrawLine(pen, SelPnt2.X, Canvas.Top - 5, SelPnt1.X, Canvas.Top - 5);
                        iLeft = (SelPnt1.X + SelPnt2.X) / 2 - 13;
                        iTop = Canvas.Top - 25;
                        iWidth = ((SelPnt1.X + SelPnt2.X) / 2 + 13) - iLeft;
                        iHeight = (Canvas.Top - 9) - iTop;
                        g.DrawRectangle(pen, iLeft, iTop, iWidth, iHeight);
                        g.DrawString(((int)(ONE_BYTE / (double)Canvas.Height * Math.Abs(SelPnt1.X - SelPnt2.X))).ToString(), font, brush, (SelPnt1.X + SelPnt2.X) / 2 - 13, Canvas.Top - 25);

                        PixVal = null;
                        AvrVal = null;
                        DifVal = null;
                        
                        //delete Gdi;
                        
                        //  Copy buffer bitmap into window dc.
                        //BitBlt(hDc, 0, 0, PPxView->Width, PPxView->Height, hMemDc, 0, 0, SRCCOPY);
                        //
                        //// Delete object
                        //SelectObject(hMemDc, hOldMemBm);
                        //DeleteObject(hMemBm);
                        //DeleteDC(hMemDc);
                        //ReleaseDC(PPxView->Handle, hDc);
                    }
                }
            }
            
        }

        private void FormProfile_Move(object sender, EventArgs e)
        {
            //pnProfile.Invalidate();
        }

        private void FormProfile_Resize(object sender, EventArgs e)
        {
            ibProfile.Invalidate(); //ㅇㅇ
        }

        private void cbRaw_CheckedChanged(object sender, EventArgs e)
        {
            ibProfile.Invalidate();
        }

        private void ibProfile_MouseUp(object sender, MouseEventArgs e)
        {
            
            if (e.Button == MouseButtons.Left)
            {
                IsSelVer1 = false;
                IsSelHor1 = false;
                IsSelVer2 = false;
                IsSelHor2 = false;
                ibProfile.Invalidate();
            }
        }

        private void ibProfile_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (e.Y < SelPnt1.Y + 3 && e.Y > SelPnt1.Y - 3 && e.X < Canvas.Right  && e.X > Canvas.Left) { IsSelHor1 = true; }
                if (e.X < SelPnt1.X + 3 && e.X > SelPnt1.X - 3 && e.Y < Canvas.Bottom && e.Y > Canvas.Top ) { IsSelVer1 = true; }
                if (IsSelHor1 || IsSelVer1) return;

                if (e.Y < SelPnt2.Y + 3 && e.Y > SelPnt2.Y - 3 && e.X < Canvas.Right  && e.X > Canvas.Left) { IsSelHor2 = true; }
                if (e.X < SelPnt2.X + 3 && e.X > SelPnt2.X - 3 && e.Y < Canvas.Bottom && e.Y > Canvas.Top ) { IsSelVer2 = true; }
            }
            ibProfile.Invalidate();
        }

        private void ibProfile_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X > Canvas.Right || e.X < Canvas.Left || e.Y > Canvas.Bottom || e.Y < Canvas.Top) return;

            if (e.Button == MouseButtons.Left)
            {
                //if (IsSelHor1) { SelPnt1.Y = e.Y; ibProfile.Invalidate(); }
                //if (IsSelVer1) { SelPnt1.X = e.X; ibProfile.Invalidate(); }

                //if (IsSelHor2) { SelPnt2.Y = e.Y; ibProfile.Invalidate(); }
                //if (IsSelVer2) { SelPnt2.X = e.X; ibProfile.Invalidate(); }

                if (IsSelHor1) { SelPnt1.Y = e.Y; LastSelPnt = SelPnt1; ibProfile.Invalidate(); }
                if (IsSelVer1) { SelPnt1.X = e.X; LastSelPnt = SelPnt1; ibProfile.Invalidate(); }

                if (IsSelHor2) { SelPnt2.Y = e.Y; LastSelPnt = SelPnt2; ibProfile.Invalidate(); }
                if (IsSelVer2) { SelPnt2.X = e.X; LastSelPnt = SelPnt2; ibProfile.Invalidate(); }
            }
        } 
    }
}
