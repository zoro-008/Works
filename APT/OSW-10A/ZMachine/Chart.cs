using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Machine
{
    class Chart
    {
        public enum XValue
        {
            DateTime ,
            Value    
        }

        public class Item
        {
            public string       Name     ;
            public double       XUnit    ;
            public Color        LineColor;
            public List<double> YDatas   = new List<double>();
        }
        List<Item> Items = new List<Item>();

        double dXStt ; 
        double dXEnd ;
        double dXUnit;
        double dYStt ;
        double dYEnd ;
        double dYUnit;
        XValue XUnit ;

        public Chart()
        {
        }

        public void Clear()
        {
            Items.Clear();
        }
        public void SetBase(double _dXStt , double _dXEnd ,double _dXUnit , double _dYStt , double _dYEnd , double _dYUnit  , XValue _eValue)
        {
            dXStt  = _dXStt ;
            dXEnd  = _dXEnd ;
            dXUnit = _dXUnit;
            dYStt  = _dYStt ;
            dYEnd  = _dYEnd ;
            dYUnit = _dYUnit;
            XUnit  = _eValue;
        }
        public void AddItem(ref Item _tItem)
        {
            Items.Add(_tItem);
        }
        public void Paint(PictureBox _pbBase) 
        {
            if(Items.Count <= 0) return ;
            SolidBrush Brush  = new SolidBrush(Color.LightGray);
            Pen        Pen    = new Pen(Color.LightGray);
            Font       font   = new Font("Verdana",10);
            Graphics   gArray = Graphics.FromImage(_pbBase.Image); //pbArray.CreateGraphics();                    
            using(BufferedGraphics Buffer = BufferedGraphicsManager.Current.Allocate(gArray , _pbBase.ClientRectangle))
            {
                //전체 그림영역.
                Rectangle RectBase  = new Rectangle(0  , 0 , _pbBase.Width - 1, _pbBase.Height - 1);
                //그래프 영역.
                const int ChartMarginL = 150 ;
                const int ChartMarginT = 20  ;
                const int ChartMarginR = 20  ;
                const int ChartMarginB = 100 ;
                int ChartWidth   = _pbBase.Width  - ChartMarginL - ChartMarginR ;
                int ChartHeight  = _pbBase.Height - ChartMarginT - ChartMarginB ;
                Rectangle RectChart = new Rectangle(ChartMarginL , ChartMarginT , ChartWidth , ChartHeight);

                Buffer.Graphics.Clear(Color.White);
                Brush.Color = Color.LightGray ;
                Buffer.Graphics.FillRectangle(Brush, RectChart) ;
                Pen.Color = Color.Black ;
                Buffer.Graphics.DrawRectangle(Pen  , RectChart) ;

                double dXDataToPx = RectChart.Width/(dXEnd - dXStt);
                double dXPxToData = (dXEnd - dXStt) / RectChart.Height;
                double dYDataToPx = RectChart.Height/(dYEnd - dYStt);
                double dYPxToData = (dYEnd - dYStt)/RectChart.Height;

                //int iXUnitCnt = (dXEnd - dXStt)/dXUnit;
                //int iXUnitPx  = dXUnit * dXDataToPx ;

                int iX = 0 ;
                int iY = 0 ;
                double dTemp = 0 ;
                Pen.Color = Color.Gray ;
                RectangleF Rect = new RectangleF (0,0,10,90);
                string sText = "";
                for (double x = dXStt; x < dXEnd; x+=dXUnit)
                {   //RectChart
                    dTemp =  (x - dXStt) * dXDataToPx ;
                    iX = RectChart.Left + (int)dTemp;
                    //세로라인긋기.
                    Buffer.Graphics.DrawLine(Pen , iX , RectChart.Top , iX , RectChart.Bottom);
                    //세로라인밑에 X축 데이터입력.
                    if (XUnit == XValue.DateTime)sText = DateTime.FromOADate(x).ToString("MM-dd");
                    else                         sText = x.ToString();
                    Rect.X = iX ;
                    Rect.Y = RectChart.Bottom+5 ;
                    Buffer.Graphics.DrawString(sText,font , Brushes.Black , Rect);

                }

                for (double y = dYStt; y < dYEnd; y+=dYUnit)
                {   //RectChart
                    dTemp = - (y - dYStt) * dYDataToPx ;
                    iY = RectChart.Bottom + (int)dTemp;
                    Buffer.Graphics.DrawLine(Pen , RectChart.Left , iY , RectChart.Right , iY);
                    sText = y.ToString();
                    Buffer.Graphics.DrawString(sText,font , Brushes.Black , RectChart.Left-30 , iY-font.Size/2);
                }
                //dXStt 
                //dXEnd 
                //dXUnit
                int iPreX = 0 , iPreY =0 ;
                int iPosY = 0 ;
                foreach (Item it in Items)
                {
                    Pen.Color = it.LineColor ;
                    Pen.Width = 5 ;

                    //왼쪽에 보기 
                    iPosY+=20;
                    Buffer.Graphics.DrawLine  (Pen , 5 , iPosY , 10 , iPosY);
                    Buffer.Graphics.DrawString(it.Name ,font , Brushes.Black , 15 , iPosY-font.Size/2);

                    Pen.Width = 2 ;
                    for(int i = 0 ; i < it.YDatas.Count ; i++)
                    {            //XUnit = X의간격.            
                        iX =(int)((it.XUnit * i) * dXDataToPx) ;
                        iY =(int)((it.YDatas[i] - dYStt) * dYDataToPx) ;
                        iX = RectChart.Left + iX;
                        iY = RectChart.Bottom - iY;
                        if(i==0){
                            iPreX =(int)((it.XUnit * i) * dXDataToPx) ;
                            iPreY =(int)((it.YDatas[i] - dYStt) * dYDataToPx) ;

                            iPreX = RectChart.Left + iPreX;
                            iPreY = RectChart.Bottom - iPreY;

                            continue ;
                        }
                        
                        Buffer.Graphics.DrawLine(Pen , iPreX , iPreY , iX , iY);
                        iPreX = iX ;
                        iPreY = iY ;
                    }
                }
                Buffer.Render(gArray);
            }            
            gArray.Dispose();
            _pbBase.Invalidate();
        }

    }
}
