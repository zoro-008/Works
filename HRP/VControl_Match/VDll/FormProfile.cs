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
            
            TopMost = true;

            Tracker.trackerType = CTracker.ETrackerType.ttLine;
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

            Tracker.trackerType = CTracker.ETrackerType.ttNone;//이거 이전 트랙커 타입 저장해놨다가 다시 바꾸도록 해야될거같다. 진섭
            for (int i = 0; i < GV.Para.VisionCnt; i++)
            {
                GV.FrmVisions[i].PaintInvoke();
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

            //메모리에 그린 후 Rendering 한다.
            using(BufferedGraphics bufferedgraphic = BufferedGraphicsManager.Current.Allocate(e.Graphics, this.ClientRectangle))
            {
                bufferedgraphic.Graphics.InterpolationMode = InterpolationMode.High ;
                bufferedgraphic.Graphics.SmoothingMode     = SmoothingMode.AntiAlias;
                bufferedgraphic.Graphics.TranslateTransform(this.AutoScrollPosition.X, this.AutoScrollPosition.Y);

                int AvrRange = Math.Abs(CConfig.StrToIntDef(tbAvrRange.Text, 1));

                Graphics g = bufferedgraphic.Graphics;
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
                string Text;

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
                             
                             for (int i = Canvas.Left; i < Canvas.Right; i += font.Height + 15)
                             {
                                 iTemp = (int)(DataCnt * (i - Canvas.Left) / (Canvas.Right - Canvas.Left) + StartX);
                                 g.DrawLine(pen, i, Canvas.Bottom, i, Canvas.Bottom + 3);
                                 if (cbHor.Checked) g.DrawLine(pen, i, Canvas.Top, i, Canvas.Bottom);
                                 g.DrawString(iTemp.ToString(), font, brush, (i - font.Height / 2), Canvas.Bottom + 10);
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
                             for (int k = 1; k < DataCnt - 1; k++)
                             {
                                 RatioX = (Canvas.Right  - Canvas.Left) / (double)DataCnt ;
                                 RatioY = (Canvas.Bottom - Canvas.Top ) / (double)ONE_BYTE;
                             
                                 if(Image.ElementSize == 3) //컬러 이미지
                                 {
                             
                                 }
                                 else //흑백 이미지
                                 {
                                     pen.Color = Color.SlateGray;
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
                             
                                     pen.Color = Color.FromArgb(0, PixVal[k], PixVal[k], PixVal[k]);
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
                                         g.FillRectangle(brush, Canvas.Left + (int)(k * RatioX) - 2, Canvas.Bottom - (int)(DifVal[k] * RatioY) - 2, Canvas.Left + (int)(k * RatioX) + 2, Canvas.Bottom - (int)(DifVal[k] * RatioY) + 2);
                                     }
                                     
                                     ////Dif2Edge
                                     //Gdi->m_tPen.Color = clFuchsia;
                                     //IsEdge = (Df2Val[k + 1] < Df2Val[k] && Df2Val[k - 1] < Df2Val[k]) || //(Df2Val[k+1] < Df2Val[k] && Df2Val[k-1] <= Df2Val[k]) ||
                                     //         (Df2Val[k + 1] > Df2Val[k] && Df2Val[k - 1] > Df2Val[k]);//|| (Df2Val[k+1] > Df2Val[k] && Df2Val[k-1] >= Df2Val[k]) ;
                                     //if (IsEdge && ShowDf2)
                                     //{
                                     //    Gdi->m_tPen.Color = clBlue;
                                     //    Gdi->Rect(true, rect.left + (int)(k * RatioX) - 2, rect.bottom - (int)(Df2Val[k] * RatioY) - 2, rect.left + (int)(k * RatioX) + 2, rect.bottom - (int)(Df2Val[k] * RatioY) + 2);
                                     //}
                                 }
                             }
                             
                             
                             //SelPoint1
                             pen.Color = Color.Lime;
                             g.DrawLine(pen, Canvas.Left, SelPnt1.Y, Canvas.Right, SelPnt1.Y); // Hor Bar
                             g.DrawLine(pen, SelPnt1.X, Canvas.Top, SelPnt1.X, Canvas.Bottom); // Ver Bar
                             g.DrawString((ONE_BYTE - (int)(ONE_BYTE / (double)Canvas.Height * (SelPnt1.Y - iCanvasOffset))).ToString(), font, brush, Canvas.Right - 20, SelPnt1.Y - font.Height - 2);
                             //Gdi->m_tFont.Escapement = 90; //폰트 각도 돌리는거
                             g.DrawString((DataCnt * (SelPnt1.X - Canvas.Left) / (Canvas.Right - Canvas.Left) + StartX).ToString(), font, brush, SelPnt1.X - font.Height - 2, Canvas.Top + 20);
                             //Gdi->m_tFont.Escapement = 0;
                             
                             //SelPoint2
                             pen.Color = Color.Yellow;
                             g.DrawLine(pen, Canvas.Left, SelPnt2.Y, Canvas.Right, SelPnt2.Y); // Hor Bar
                             g.DrawLine(pen, SelPnt2.X, Canvas.Top, SelPnt2.X, Canvas.Bottom); // Ver Bar
                             g.DrawString((ONE_BYTE - (int)(ONE_BYTE / (double)Canvas.Height * (SelPnt2.Y - iCanvasOffset))).ToString(), font, brush, Canvas.Right - 20, SelPnt2.Y - font.Height - 2);
                             //Gdi->m_tFont.Escapement = 90;
                             g.DrawString((DataCnt * (SelPnt2.X - Canvas.Left) / (Canvas.Right - Canvas.Left) + StartX).ToString(), font, brush, SelPnt2.X - font.Height - 2, Canvas.Top + 20);
                             //Gdi->m_tFont.Escapement = 0;
                             
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
                             g.FillRectangle(brush, Canvas.Right + 7, (SelPnt1.Y + SelPnt2.Y) / 2 - font.Height, Canvas.Right + 30, (SelPnt1.Y + SelPnt2.Y) / 2 + font.Height);
                             g.DrawString(((int)(ONE_BYTE / (double)Canvas.Height * Math.Abs(SelPnt1.Y - SelPnt2.Y))).ToString(), font, brush, Canvas.Right + 9, (SelPnt1.Y + SelPnt2.Y) / 2 - font.Height / 2);
                             
                             //y Diffrence.
                             //Gdi->m_tFont.Escapement = 90;
                             //Gdi->m_tBrush.Color = (TColor)0x00363636;
                             pen.Color = Color.Silver;
                             g.DrawLine(pen, SelPnt1.X, Canvas.Top, SelPnt1.X, Canvas.Top - 5);
                             g.DrawLine(pen, SelPnt2.X, Canvas.Top, SelPnt2.X, Canvas.Top - 5);
                             g.DrawLine(pen, SelPnt2.X, Canvas.Top - 5, SelPnt1.X, Canvas.Top - 5);
                             
                             g.FillRectangle(brush, (SelPnt1.X + SelPnt2.X) / 2 - font.Height, Canvas.Top - 7, (SelPnt1.X + SelPnt2.X) / 2 + font.Height, Canvas.Top - 30);
                             g.DrawString(((int)(ONE_BYTE / (double)Canvas.Height * Math.Abs(SelPnt1.X - SelPnt2.X))).ToString(), font, brush, (SelPnt1.X + SelPnt2.X) / 2 - font.Height / 2, Canvas.Top - 9);
                             //Gdi->m_tFont.Escapement = 0;
                             
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

                bufferedgraphic.Render(e.Graphics); //마지막에 렌더링한다.
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
            ibProfile.Invalidate();
        }

        private void ibProfile_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (e.Y < SelPnt1.Y + 3 && e.Y > SelPnt1.Y - 3 && e.X < Canvas.Right  && e.X > Canvas.Left) { LastSelPnt = SelPnt1; IsSelHor1 = true; }
                if (e.X < SelPnt1.X + 3 && e.X > SelPnt1.X - 3 && e.Y < Canvas.Bottom && e.Y > Canvas.Top ) { LastSelPnt = SelPnt1; IsSelVer1 = true; }
                if (IsSelHor1 || IsSelVer1) return;

                if (e.Y < SelPnt2.Y + 3 && e.Y > SelPnt2.Y - 3 && e.X < Canvas.Right  && e.X > Canvas.Left) { LastSelPnt = SelPnt2; IsSelHor2 = true; }
                if (e.X < SelPnt2.X + 3 && e.X > SelPnt2.X - 3 && e.Y < Canvas.Bottom && e.Y > Canvas.Top ) { LastSelPnt = SelPnt2; IsSelVer2 = true; }
            }
            ibProfile.Invalidate();
        }

        private void ibProfile_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X > Canvas.Right || e.X < Canvas.Left || e.Y > Canvas.Bottom || e.Y < Canvas.Top) return;

            if (e.Button == MouseButtons.Left)
            {
                if (IsSelHor1) { SelPnt1.Y = e.Y; ibProfile.Invalidate(); }
                if (IsSelVer1) { SelPnt1.X = e.X; ibProfile.Invalidate(); }

                if (IsSelHor2) { SelPnt2.Y = e.Y; ibProfile.Invalidate(); }
                if (IsSelVer2) { SelPnt2.X = e.X; ibProfile.Invalidate(); }
            }
        }
    }
}
