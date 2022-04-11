using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DrawingBoard
{
    class OptionalFigure : FigureBase//自选图形基类
    {
        public List<PointF> relativePoints = new List<PointF>();//图形顶点于左下角的相对坐标 （0，0）—（1，1）
        public List<PointF> points = new List<PointF>();//顶点的实际逻辑坐标

        public OptionalFigure(CommonData cd,PointF sp,PointF ep)//留给子类的构造函数
        {            
            this.thickness = cd.thickness;
            this.color = cd.color;
            this.lineStyle = cd.lineStyle;

            PointF pMin = this.GetPMin(sp, ep);
            PointF pMax = this.GetPMax(sp, ep);
            this.location = new PointF(pMin.X, pMin.Y);
            this.width = pMax.X - pMin.X;
            this.height = pMax.Y - pMin.Y;
            this.mid = new PointF(location.X + width / 2, location.Y + height / 2);
            this.iD = CreateId(cd);
        }

        public OptionalFigure()
        {
        }

        public override void Draw(CommonData cd, Bitmap btm)//显示函数
        {
            //把像素点画出来
            for (int i = 0; i < pixels.Count; i++)
            {
                DrawPoint(cd, pixels[i].X, pixels[i].Y, btm);
            }
        }

        public override void PixelsFlush(CommonData cd)//计算用于显示的像素点
        {
            //先计算各个点的实际逻辑坐标                        
            PointsFlush();//刷新顶点的逻辑坐标
            //再计算各顶点的物理坐标
            List<Point> PbPoints = new List<Point>();//各顶点的物理坐标
            for (int i = 0; i < points.Count(); i++)
            {
                PbPoints.Add(cd.CvlToPbl(points[i]));
            }
            //将各顶点连成线
            btmTemp = new Bitmap(cd.pictureBox.Width, cd.pictureBox.Height);
            pixels = new List<Point>();
            for (int i = 0; i < PbPoints.Count() - 1; i++)
            {
                GetLinePixels(PbPoints[i], PbPoints[i + 1], cd);
            }
            GetLinePixels(PbPoints[0], PbPoints[PbPoints.Count() - 1], cd);
            btmTemp = new Bitmap(cd.pictureBox.Width, cd.pictureBox.Height);
        }

        private void PointsFlush()//刷新顶点逻辑坐标
        {
            points = new List<PointF>();
            for (int i = 0; i < relativePoints.Count(); i++)
            {
                points.Add(RelToCvl(relativePoints[i]));
            }
        }

        private PointF RelToCvl(PointF p)//将相对顶点转为实际逻辑顶点
        {
            PointF resultP = new PointF();
            resultP.X = location.X + width * p.X;
            resultP.Y = location.Y + height * p.Y;
            return resultP;
        }

        private void RelativePointsFlush()//由逻辑顶点刷新相对顶点
        {
            relativePoints = new List<PointF>();
            for(int i=0;i<points.Count();i++)
            {
                PointF pf = new PointF();
                if (width == 0)
                {
                    pf.X = 0;
                }
                else
                {
                    pf.X = (points[i].X - location.X) / width;
                }
                if (height == 0)
                {
                    pf.Y = 0;
                }
                else
                {
                    pf.Y = (points[i].Y - location.Y) / height;
                }
                relativePoints.Add(pf);
            }            
        }

        public override void Resize(Point lc, int w, int h, CommonData cd)//根据新的坐标和宽高计算新的各属性
        {
            location = cd.PblToCvl(new Point(lc.X, lc.Y + h));//新坐标
            width = cd.PblToCvl(w);//新宽
            height = cd.PblToCvl(h);//新高
            mid = new PointF(location.X + width / 2, location.Y + height / 2);//新中心                                    
            PixelsFlush(cd);
        }

        public override void Turn(float s, CommonData cd)//顺时针转s角度（mid为基点）
        {
            for (int i = 0; i < points.Count(); i++)
            {
                points[i] = this.PointTurn(points[i],s);
            }
            PointF pMin = cd.GetMin(points);//左下角
            PointF pMax = cd.GetMax(points);//右上角
            this.location = pMin;//新坐标
            this.width = pMax.X - pMin.X;//新宽
            this.height = pMax.Y - pMin.Y;//新高
            this.mid = new PointF(location.X + width / 2, location.Y + height / 2);//新中心
            RelativePointsFlush();//刷新相对顶点                      
            PixelsFlush(cd);
        }

        public override void Turn(float s, PointF m, CommonData cd)//顺时针转s角度（m为基点）
        {
            for (int i = 0; i < points.Count(); i++)
            {
                points[i] = this.PointTurn(points[i],m, s);
            }
            PointF pMin = cd.GetMin(points);//左下角
            PointF pMax = cd.GetMax(points);//右上角
            this.location = pMin;//新坐标
            this.width = pMax.X - pMin.X;//新宽
            this.height = pMax.Y - pMin.Y;//新高
            this.mid = new PointF(location.X + width / 2, location.Y + height / 2);//新中心
            RelativePointsFlush();//刷新相对顶点                      
            PixelsFlush(cd);
        }

        public override bool Select(Point p, CommonData cd)//通过一个点选择图元
        {
            if (isSelect == true)//已经选定了就不能再选了
            {
                return false;
            }
            ///创建一个虚拟画板
            Bitmap image = new Bitmap(cd.picBoxWidth, cd.picBoxHeight);                        
            ///加粗原图形后画在画板上
            Draw(cd, image);
            ///取色，比较后是否选定
            Color cget;
            cget = image.GetPixel(p.X, p.Y);
            if (cget == Color.FromArgb(0,0,0,0))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public override FigureBase GetNewOne(CommonData cd)//重写克隆函数
        {
            OptionalFigure resultF = new OptionalFigure();
            resultF.relativePoints = new List<PointF>();
            for (int i = 0; i < relativePoints.Count(); i++)
            {
                resultF.relativePoints.Add(new PointF(relativePoints[i].X,relativePoints[i].Y));
            }
            resultF.points = new List<PointF>();
            for (int i = 0; i < points.Count(); i++)
            {
                resultF.points.Add(new PointF(points[i].X,points[i].Y));
            }

            resultF.location = new PointF(location.X, location.Y);    
            resultF.width = this.width;
            resultF.height = this.height;
            resultF.relativeLocation = new PointF(this.relativeLocation.X, this.relativeLocation.Y);
            resultF.relativeWidth = this.relativeWidth;
            resultF.relativeHeight = this.height;
            resultF.mid = new PointF(mid.X, mid.Y);
            resultF.iD = CreateId(cd);
            resultF.PixelsFlush(cd);
            resultF.isSelect = false;
            resultF.FigureKind = this.FigureKind;
            resultF.thickness = this.thickness;
            resultF.color = this.color;
            resultF.lineStyle = this.lineStyle;
            return resultF;
        }

        public override FigureBase CopyNewOne(CommonData cd)//重写克隆函数,用于复制粘贴
        {
            float d = cd.PblToCvl(10);
            OptionalFigure resultF = new OptionalFigure();
            resultF.relativePoints = new List<PointF>();
            for (int i = 0; i < relativePoints.Count(); i++)
            {
                resultF.relativePoints.Add(new PointF(relativePoints[i].X, relativePoints[i].Y));
            }
            resultF.points = new List<PointF>();
            for (int i = 0; i < points.Count(); i++)
            {
                resultF.points.Add(new PointF(points[i].X+d, points[i].Y+d));
            }

            resultF.location = new PointF(location.X+d, location.Y+d);
            resultF.width = this.width;
            resultF.height = this.height;
            resultF.relativeLocation = new PointF(this.relativeLocation.X, this.relativeLocation.Y);
            resultF.relativeWidth = this.relativeWidth;
            resultF.relativeHeight = this.height;
            resultF.mid = new PointF(mid.X+d, mid.Y+d);
            resultF.iD = CreateId(cd);
            resultF.PixelsFlush(cd);
            resultF.isSelect = false;
            resultF.FigureKind = this.FigureKind;
            resultF.thickness = this.thickness;
            resultF.color = this.color;
            resultF.lineStyle = this.lineStyle;
            return resultF;
        }    
    }
}
