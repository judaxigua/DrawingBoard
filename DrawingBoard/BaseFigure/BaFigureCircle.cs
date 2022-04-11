using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DrawingBoard
{
    class BaFigureCircle : FigureBase//基本图形圆类
    {
        public BaPoint center;//中心点
        public float rad;//半径      

        public BaFigureCircle(CommonData cd)//构造函数
        {
            this.FigureKind = BaseFigure.CIRCLE;
            this.thickness = cd.thickness;
            this.color = cd.color;
            this.lineStyle = cd.lineStyle;
        }

        public BaFigureCircle()//空的构造函数
        {   }

        public override void Draw(CommonData cd,Bitmap btm)//显示像素点
        {
            //把像素点画出来            
            for (int i = 0; i < pixels.Count; i++)
            {
                DrawPoint(cd, pixels[i].X, pixels[i].Y,btm);
            }
            center.Draw(cd,btm);
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
            if (center.location.X == bP.location.X && center.location.Y == bP.location.Y)
            {
                center = tP;
            }
        }

        public override bool Select(Point p, CommonData cd)//选择函数
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
            Point ct = cd.CvlToPbl(center.location);
            int r = cd.CvlToPbl(rad);
            g.DrawEllipse(new Pen(c, thicknessB), ct.X - r, ct.Y - r, r * 2, r * 2);
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

        public override void Resize(Point lc, int w, int h,CommonData cd)//重写属性调整
        {
            center.PointResize(lc,location, w, h, width, height, cd);
            int lMax = this.GetMax(w, h);//由于是正圆，挑大的高宽作为新的高宽            
            rad = cd.PblToCvl(lMax) / 2;//新半径
            location = cd.PblToCvl(new Point(lc.X, lc.Y + lMax));//新坐标
            //新宽高
            width = cd.PblToCvl(lMax);
            height = cd.PblToCvl(lMax);
            //新中心
            mid = new PointF(location.X + lMax / 2, location.Y + lMax / 2);                        
            PixelsFlush(cd);//刷新像素点
        }

        public override void Resize(CommonData cd)//因为点的变动产生的resize
        {
            this.location.X = center.location.X - rad;
            this.location.Y = center.location.Y - rad;
            this.width = rad * 2;
            this.height = rad * 2;
        }        

        public override void Turn(float s, CommonData cd)//顺时针转s角度（mid为基点）
        {
            //圆的话暂时应该没有什么需要旋转的
        }

        public override void Turn(float s,PointF m, CommonData cd)//顺时针转s角度（m为基点）
        {            
            center.Turn(m,s,cd);
            location.X = center.location.X - rad;
            location.Y = center.location.Y - rad;
            mid.X = center.location.X;
            mid.Y = center.location.Y;            
            PixelsFlush(cd);
        }
        
        public override void PixelsFlush(CommonData cd)//重新计算用于显示的像素点
        {
            btmTemp = new Bitmap(cd.pictureBox.Width, cd.pictureBox.Height);  //用作计算的虚拟画板  
            pixels= new List<Point>();
            GetCirclePixels(cd);
            btmTemp = new Bitmap(cd.pictureBox.Width, cd.pictureBox.Height);            
            for (int i = 0; i < baPoints.Count(); i++)//点的刷新像素点
            {
                baPoints[i].PixelsFlush(cd);
            }
        }

        public void GetCirclePixels(CommonData cd)//用八分画圆计算像素点并且保存
        {
            lineMatrixIndex = 0;
            int d=1-cd.CvlToPbl(rad); Point p = new Point(0,cd.CvlToPbl(rad));
            Point cp = cd.CvlToPbl(center.location);
            while(p.X<=p.Y)
            {
                if (p.X == p.Y)
                {
                    PixelBrush(cp.X + p.X, cp.Y + p.Y, thickness, cd);                                        
                    PixelBrush(cp.X - p.X, cp.Y + p.Y, thickness, cd);                    
                    PixelBrush(cp.X + p.X, cp.Y - p.Y, thickness, cd);                    
                    PixelBrush(cp.X - p.X, cp.Y - p.Y, thickness, cd);
                }                
                PixelBrush(cp.X + p.X, cp.Y + p.Y, thickness, cd);                
                PixelBrush(cp.X + p.X, cp.Y - p.Y, thickness, cd);                
                PixelBrush(cp.X - p.X, cp.Y + p.Y, thickness, cd);                
                PixelBrush(cp.X - p.X, cp.Y - p.Y, thickness, cd);                
                PixelBrush(cp.X + p.Y, cp.Y + p.X, thickness, cd);                
                PixelBrush(cp.X + p.Y, cp.Y - p.X, thickness, cd);                
                PixelBrush(cp.X - p.Y, cp.Y + p.X, thickness, cd);                
                PixelBrush(cp.X - p.Y, cp.Y - p.X, thickness, cd);
                lineMatrixIndex = (lineMatrixIndex + 1) % (6 * thickness);
	            p.X++;
                if(d<0) d=d+2*p.X+1;
                else   { p.Y--; d=d+2*p.X-2*p.Y+1; }
            }
        }

        public override FigureBase GetNewOne(CommonData cd)//原封不动的克隆
        {
            BaFigureCircle resultF = new BaFigureCircle();
            resultF.center = new BaPoint(center.location, cd);
            resultF.rad = this.rad;
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
            resultF.baPoints.Add(resultF.center);
            resultF.isSelect = false;
            resultF.FigureKind = BaseFigure.CIRCLE;
            resultF.thickness = this.thickness;
            resultF.color = this.color;
            resultF.lineStyle = this.lineStyle;
            return resultF;
        }

        public override FigureBase CopyNewOne(CommonData cd)//复制粘贴用的克隆，位置向左上角偏移
        {
            float d = cd.PblToCvl(10);
            BaFigureCircle resultF = new BaFigureCircle();
            resultF.center = new BaPoint(new PointF(center.location.X + d, center.location.Y + d), cd);
            resultF.rad = this.rad;
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
            resultF.baPoints.Add(resultF.center);
            resultF.isSelect = false;
            resultF.FigureKind = BaseFigure.CIRCLE;
            resultF.thickness = this.thickness;
            resultF.color = this.color;
            resultF.lineStyle = this.lineStyle;
            return resultF;
        }        
    }
}
