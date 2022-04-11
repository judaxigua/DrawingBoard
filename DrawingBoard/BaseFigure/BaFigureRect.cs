using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DrawingBoard
{
    class BaFigureRect : FigureBase//基本图形矩形类
    {
        public BaPoint pLU;//左上角点在画布上的坐标
        public BaPoint pRU;//右上角
        public BaPoint pRD;//右下角
        public BaPoint pLD;//左下角      

        public BaFigureRect(CommonData cd)//构造函数
        {
            this.FigureKind = BaseFigure.RECT;
            this.thickness = cd.thickness;
            this.color = cd.color;
            this.lineStyle = cd.lineStyle;
        }

        public BaFigureRect()
        {
        }

        public override void Draw(CommonData cd,Bitmap btm)//显示函数
        {            
            //把像素点画出来
            for (int i = 0; i < pixels.Count; i++)
            {
                DrawPoint(cd, pixels[i].X, pixels[i].Y,btm);
            }
            pLU.Draw(cd,btm);//左上
            pRU.Draw(cd,btm);//右上
            pRD.Draw(cd,btm);//右下
            pLD.Draw(cd,btm);//左下
        }        

        public override void PixelsFlush(CommonData cd)//计算用于显示的像素点
        {
            btmTemp = new Bitmap(cd.pictureBox.Width, cd.pictureBox.Height);   
            pixels = new List<Point>();
            //得到各顶点的画板坐标
            Point pLUP = cd.CvlToPbl(pLU.location);
            Point pRUP = cd.CvlToPbl(pRU.location);
            Point pRDP = cd.CvlToPbl(pRD.location);
            Point pLDP = cd.CvlToPbl(pLD.location);

            //计算各条边的像素点
            GetLinePixels(pLUP, pRUP, cd);
            GetLinePixels(pRUP, pRDP, cd);
            GetLinePixels(pRDP, pLDP, cd);
            GetLinePixels(pLUP, pLDP, cd);
            btmTemp = new Bitmap(cd.pictureBox.Width, cd.pictureBox.Height);
            for (int i = 0; i < baPoints.Count(); i++)
            {
                baPoints[i].PixelsFlush(cd);
            }
        }

        public override bool Select(Point p, CommonData cd)//根据一个像素点来选择这个图形
        {
            if (isSelect == true)//已经选定了就不能再选了
            {
                return false;
            }
            ///创建一个虚拟画板
            Bitmap image = new Bitmap(cd.picBoxWidth, cd.picBoxHeight);
            Graphics g = Graphics.FromImage(image);
            Color c = Color.FromArgb(0, 0, 1);///设置背景色特殊 0，0，1
            g.Clear(Color.White);
            ///加粗原图形后画在画板上
            int thicknessB = this.thickness + 6;
            g.DrawLine(new Pen(c, thicknessB), cd.CvlToPbl(pLU.location), cd.CvlToPbl(pRU.location));
            g.DrawLine(new Pen(c, thicknessB), cd.CvlToPbl(pRU.location), cd.CvlToPbl(pRD.location));
            g.DrawLine(new Pen(c, thicknessB), cd.CvlToPbl(pRD.location), cd.CvlToPbl(pLD.location));
            g.DrawLine(new Pen(c, thicknessB), cd.CvlToPbl(pLD.location), cd.CvlToPbl(pLU.location));
            ///取色，比较后是否选定
            Color cget;
            cget = image.GetPixel(p.X, p.Y);
            if (cget.B == 1 && cget.G == 0 && cget.R == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void ReplacePoint(BaPoint bP, BaPoint tP)//替换其中的点 bP被替换的点，tP替换目标 
        {
            for (int i = 0; i < baPoints.Count(); i++)
            {
                if (baPoints[i].location.X == bP.location.X && baPoints[i].location.Y == bP.location.Y)
                {
                    baPoints[i] = tP;
                }
            }
            if (pLU.location.X == bP.location.X && pLU.location.Y == bP.location.Y)
            {
                pLU = tP;
            }
            if (pRU.location.X == bP.location.X && pRU.location.Y == bP.location.Y)
            {
                pRU = tP;
            }
            if (pRD.location.X == bP.location.X && pRD.location.Y == bP.location.Y)
            {
                pRD = tP;
            }
            if (pLD.location.X == bP.location.X && pLD.location.Y == bP.location.Y)
            {
                pLD = tP;
            }
        }

        public override void Resize(Point lc, int w, int h, CommonData cd)//重写属性调整
        {
            //注意lc是相对于画板的左上角坐标            
            pLU.PointResize(lc,location, w, h, width, height, cd);
            pLD.PointResize(lc, location, w, h, width, height, cd);
            pRU.PointResize(lc, location, w, h, width, height, cd);
            pRD.PointResize(lc, location, w, h, width, height, cd);
            location = cd.PblToCvl(new Point(lc.X, lc.Y + h));//新的坐标
            width = cd.PblToCvl(w);//新宽
            height = cd.PblToCvl(h);//新高
            mid = new PointF(location.X + width / 2, location.Y + height / 2);//新中心
            //新的顶点                        
            PixelsFlush(cd);
        }

        public override void Resize(CommonData cd)//由于点图元变动产生的resize
        {
            PointF pMax = cd.GetPMax(pLU.location, pRU.location);
            pMax = cd.GetPMax(pMax, pRD.location);
            pMax = cd.GetPMax(pMax, pLD.location);
            PointF pMin = cd.GetPMin(pLU.location, pRU.location);
            pMin = cd.GetPMin(pMin, pRD.location);
            pMin = cd.GetPMin(pMin, pLD.location);
            location = new PointF(pMin.X, pMin.Y);
            width = pMax.X - pMin.X;
            height = pMax.Y - pMin.Y;
        }

        public override void Turn(float s,CommonData cd)//顺时针转s角度（mid为基点）
        {
            //旋转各顶点
            PointF plu = this.PointTurn(pLU.location, s);
            PointF pru = this.PointTurn(pRU.location, s);
            PointF prd = this.PointTurn(pRD.location, s);
            PointF pld = this.PointTurn(pLD.location, s);
            pLU.location = new PointF(plu.X, plu.Y);
            pRU.location = new PointF(pru.X, pru.Y);
            pRD.location = new PointF(prd.X, prd.Y);
            pLD.location = new PointF(pld.X, pld.Y);          

            PointF pMin = this.GetPMin(plu, pru);
            pMin = this.GetPMin(pMin, prd);
            pMin = this.GetPMin(pMin, pld);
            PointF pMax = this.GetPMax(plu, pru);
            pMax = this.GetPMax(pMax, prd);
            pMax = this.GetPMax(pMax, pld);
            this.location = pMin;
            this.width = pMax.X - pMin.X;
            this.height = pMax.Y - pMin.Y;
            this.mid = new PointF(location.X + width / 2, location.Y + height / 2);
            ResizeToPoints(cd);
            PixelsFlush(cd);
        }

        public override void Turn(float s,PointF m, CommonData cd)//顺时针转s角度（m为基点）
        {
            //旋转各顶点
            pLU.Turn(m, s,cd);
            pRU.Turn(m, s,cd);
            pRD.Turn(m, s,cd);
            pLD.Turn(m, s,cd);

            PointF pMin = this.GetPMin(pLU.location, pRU.location);
            pMin = this.GetPMin(pMin, pRD.location);
            pMin = this.GetPMin(pMin, pLD.location);
            PointF pMax = this.GetPMax(pLU.location, pRU.location);
            pMax = this.GetPMax(pMax, pRD.location);
            pMax = this.GetPMax(pMax, pLD.location);
            this.location = pMin;
            this.width = pMax.X - pMin.X;
            this.height = pMax.Y - pMin.Y;
            this.mid = new PointF(location.X + width / 2, location.Y + height / 2);            
            PixelsFlush(cd);
        }

        public override FigureBase GetNewOne(CommonData cd)//重写克隆函数
        {
            BaFigureRect resultF = new BaFigureRect();
            resultF.pLU = new BaPoint(pLU.location, cd);
            resultF.pLD = new BaPoint(pLD.location, cd);
            resultF.pRD = new BaPoint(pRD.location, cd);
            resultF.pRU = new BaPoint(pRU.location, cd);
            resultF.location = new PointF(location.X, location.Y);
            resultF.width = this.width;
            resultF.height = this.height;
            resultF.relativeLocation = new PointF(this.relativeLocation.X, this.relativeLocation.Y);
            resultF.relativeWidth = this.relativeWidth;
            resultF.relativeHeight = this.height;
            resultF.mid = new PointF(mid.X, mid.Y);
            resultF.iD = CreateId(cd);
            resultF.PixelsFlush(cd);
            resultF.baPoints = new List<BaPoint>();
            resultF.baPoints.Add(resultF.pLU);
            resultF.baPoints.Add(resultF.pLD);
            resultF.baPoints.Add(resultF.pRU);
            resultF.baPoints.Add(resultF.pRD);
            resultF.isSelect = false;
            resultF.FigureKind = BaseFigure.LINE;
            resultF.thickness = this.thickness;
            resultF.color = this.color;
            resultF.lineStyle = this.lineStyle;
            return resultF;
        }

        public override FigureBase CopyNewOne(CommonData cd)//重写克隆函数
        {
            float d = cd.PblToCvl(10);
            BaFigureRect resultF = new BaFigureRect();
            resultF.pLU = new BaPoint(new PointF(pLU.location.X + d, pLU.location.Y + d), cd);
            resultF.pLD = new BaPoint(new PointF(pLD.location.X + d, pLD.location.Y + d), cd);
            resultF.pRU = new BaPoint(new PointF(pRU.location.X + d, pRU.location.Y + d), cd);
            resultF.pRD = new BaPoint(new PointF(pRD.location.X + d, pRD.location.Y + d), cd);
            resultF.location = new PointF(location.X + d, location.Y + d);
            resultF.width = this.width;
            resultF.height = this.height;
            resultF.relativeLocation = new PointF(this.relativeLocation.X, this.relativeLocation.Y);
            resultF.relativeWidth = this.relativeWidth;
            resultF.relativeHeight = this.relativeHeight;
            resultF.mid = new PointF(mid.X + d, mid.Y + d);
            resultF.iD = CreateId(cd);
            resultF.PixelsFlush(cd);
            resultF.baPoints = new List<BaPoint>();
            resultF.baPoints.Add(resultF.pLU);
            resultF.baPoints.Add(resultF.pLD);
            resultF.baPoints.Add(resultF.pRU);
            resultF.baPoints.Add(resultF.pRD);
            resultF.isSelect = false;
            resultF.FigureKind = BaseFigure.LINE;
            resultF.thickness = this.thickness;
            resultF.color = this.color;
            resultF.lineStyle = this.lineStyle;
            return resultF;
        }
    }
}
