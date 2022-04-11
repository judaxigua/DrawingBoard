using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DrawingBoard
{
    class BaFigureLine : FigureBase//基本图形直线类
    {
        public BaPoint sp;//起点在画布上的坐标
        public BaPoint ep;//终点        

        public BaFigureLine(CommonData cd)//构造函数
        {            
            this.FigureKind = BaseFigure.LINE;
            this.thickness = cd.thickness;
            this.color = cd.color;
            this.lineStyle = cd.lineStyle;
        }

        public BaFigureLine()
        {
        }

        public override void Draw(CommonData cd,Bitmap btm)//显示函数
        {            
            //把像素点画出来
            for (int i = 0; i < pixels.Count; i++)
            {
                DrawPoint(cd, pixels[i].X, pixels[i].Y, btm);
            }
            sp.Draw(cd,btm);
            ep.Draw(cd,btm);            
        }

        public override void PixelsFlush(CommonData cd)//计算用于显示的像素点
        {
            btmTemp = new Bitmap(cd.pictureBox.Width, cd.pictureBox.Height);            
            pixels = new List<Point>();
            Point spP = cd.CvlToPbl(sp.location);
            Point epP = cd.CvlToPbl(ep.location);
            GetLinePixels(spP, epP, cd);
            btmTemp = new Bitmap(cd.pictureBox.Width, cd.pictureBox.Height);
            for (int i = 0; i < baPoints.Count(); i++)
            {
                baPoints[i].PixelsFlush(cd);
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
            if (sp.location.X == bP.location.X && sp.location.Y == bP.location.Y)
            {
                sp = tP;
            }
            if (ep.location.X == bP.location.X && ep.location.Y == bP.location.Y)
            {
                ep = tP;
            }
        }

        public override void Resize(Point lc, int w, int h, CommonData cd)//根据新的坐标和宽高计算新的各属性
        {
            sp.PointResize(lc,location, w, h, width, height, cd);
            ep.PointResize(lc,location, w, h, width, height, cd);
            location = cd.PblToCvl(new Point(lc.X, lc.Y + h));//新坐标
            width = cd.PblToCvl(w);//新宽
            height = cd.PblToCvl(h);//新高
            mid = new PointF(location.X + width / 2, location.Y + height / 2);//新中心                        
            PixelsFlush(cd);
        }        

        public override void Resize(CommonData cd)//由于图元点的变动产生的resize
        {
            PointF pMax = cd.GetPMax(sp.location, ep.location);
            PointF pMin = cd.GetPMin(sp.location, ep.location);
            location = new PointF(pMin.X,pMin.Y);
            width = pMax.X - pMin.X;
            height = pMax.Y - pMin.Y;            
        }

        public override void Turn(float s,CommonData cd)//顺时针转s角度（mid为基点）
        {
            PointF p1 = this.PointTurn(sp.location, s);//旋转起点
            PointF p2 = this.PointTurn(ep.location, s);//旋转终点
            sp.location = new PointF(p1.X, p1.Y);//新起点
            ep.location = new PointF(p2.X, p2.Y);//新终点                        

            PointF pMin = this.GetPMin(p1, p2);//左下角
            PointF pMax = this.GetPMax(p1, p2);//右上角
            this.location = pMin;//新坐标
            this.width = pMax.X - pMin.X;//新宽
            this.height = pMax.Y - pMin.Y;//新高
            this.mid = new PointF(location.X + width / 2, location.Y + height / 2);//新中心            
            PixelsFlush(cd);
        }

        public override void Turn(float s, PointF m, CommonData cd)//顺时针转s角度（m为基点）
        {
            sp.Turn(m, s,cd);
            ep.Turn(m, s,cd);                        

            PointF pMin = this.GetPMin(sp.location, ep.location);//左下角
            PointF pMax = this.GetPMax(sp.location, ep.location);//右上角
            this.location = pMin;//新坐标
            this.width = pMax.X - pMin.X;//新宽
            this.height = pMax.Y - pMin.Y;//新高
            this.mid = new PointF(location.X + width / 2, location.Y + height / 2);//新中心            
            PixelsFlush(cd);
        }

        public override bool Select(Point p,CommonData cd)//通过一个点选择图元
        {
            if (isSelect == true)//已经选定了就不能再选了
            {
                return false;
            }
            ///创建一个虚拟画板
            Bitmap image = new Bitmap(cd.picBoxWidth,cd.picBoxHeight);
            Graphics g = Graphics.FromImage(image);
            Color c = Color.FromArgb(0, 0, 1);///设置背景色特殊 0，0，1
            g.Clear(Color.White);
            ///加粗原图形后画在画板上
            int thicknessB = this.thickness+6;
            g.DrawLine(new Pen(c,thicknessB),cd.CvlToPbl(sp.location),cd.CvlToPbl(ep.location));            
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

        public override FigureBase GetNewOne(CommonData cd)//重写克隆函数
        {
            BaFigureLine resultF = new BaFigureLine();
            resultF.sp = new BaPoint(sp.location, cd);
            resultF.ep = new BaPoint(ep.location, cd);
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
            resultF.baPoints.Add(resultF.sp);
            resultF.baPoints.Add(resultF.ep);
            resultF.isSelect = false;
            resultF.FigureKind = BaseFigure.LINE;
            resultF.thickness = this.thickness;
            resultF.color = this.color;
            resultF.lineStyle = this.lineStyle;
            return resultF;
        }

        public override FigureBase CopyNewOne(CommonData cd)//重写克隆函数,用于复制粘贴
        {
            float d = cd.PblToCvl(10);
            BaFigureLine resultF = new BaFigureLine();
            resultF.sp = new BaPoint(new PointF(sp.location.X + d, sp.location.Y + d), cd);            
            resultF.ep = new BaPoint(new PointF(ep.location.X + d, ep.location.Y + d), cd);
            resultF.isBeenEdit = false;
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
            resultF.baPoints.Add(resultF.sp);
            resultF.baPoints.Add(resultF.ep);
            resultF.isSelect = false;
            resultF.FigureKind = BaseFigure.LINE;
            resultF.thickness = this.thickness;
            resultF.color = this.color;
            resultF.lineStyle = this.lineStyle;
            return resultF;
        }        
    }
}
