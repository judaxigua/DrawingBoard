using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;


namespace DrawingBoard
{
    class FigureComposite : FigureBase//组合图元类
    {
        public List<FigureBase> figures = new List<FigureBase>();//子图元

        public FigureComposite(List<FigureBase> fs,CommonData cd)//构造函数
        {
            for (int i = 0; i < fs.Count(); i++)//挨个塞入列表
            {
                figures.Add(fs[i]);
            }
            this.FigureKind = BaseFigure.COMPOSETE;//种类定为组合图元类

            //计算图元块属性
            SizeFlush(cd);//计算规模
            Figures_SizeFlush(cd);//刷新子图元们的相对坐标
            this.mid = new PointF(location.X+width/2,location.Y+height/2);
            this.iD = CreateId(cd);
            PixelsFlush(cd);//刷新像素点
            BaPointsFlush();//刷新图元点集合
            this.isBeenEdit = false;
            for (int i = 0; i < baPoints.Count(); i++)
            {
                baPoints[i].fathers.Add(this);
            }
        }

        public void SizeFlush(CommonData cd)//更新规模
        {
            //计算宽高
            List<PointF> points = new List<PointF>();
            for (int i = 0; i < figures.Count(); i++)
            {
                PointF min = new PointF(figures[i].location.X, figures[i].location.Y);
                PointF max = new PointF(figures[i].location.X + figures[i].width, figures[i].location.Y + figures[i].height);
                points.Add(min);
                points.Add(max);
            }
            PointF pMin = cd.GetMin(points);
            PointF pMax = cd.GetMax(points);
            this.location = pMin;
            this.width = pMax.X - pMin.X;
            this.height = pMax.Y - pMin.Y;
        }

        public void Figures_SizeFlush(CommonData cd)//刷新子图元们的相对坐标
        {
            for (int i = 0; i < figures.Count(); i++)
            {
                PointF l = new PointF();
                l.X = (figures[i].location.X - this.location.X) / this.width;
                l.Y = (figures[i].location.Y - this.location.Y) / this.height;
                figures[i].relativeLocation = new PointF(l.X, l.Y);
                figures[i].relativeWidth = figures[i].width / this.width;
                figures[i].relativeHeight = figures[i].height / this.height;
            }
        }

        public void BaPointsFlush()//刷新图元点集合
        {
            this.baPoints = new List<BaPoint>();//刷新点集合
            for (int i = 0; i < figures.Count(); i++)
            {
                for (int j = 0; j < figures[i].baPoints.Count(); j++)
                {
                    this.baPoints.Add(figures[i].baPoints[j]);
                }
            }
        }   

        public FigureComposite()//空的构造函数
        {
        }             

        public override void Draw(CommonData cd,Bitmap btm)//绘图
        {
            for (int i = 0; i < figures.Count(); i++)
            {
                figures[i].Draw(cd,btm);
            }
        }

        public override void PixelsFlush(CommonData cd)//计算用于显示的像素点
        {
            this.pixels = new List<Point>();            
            for (int i = 0; i < figures.Count(); i++)
            {
                figures[i].PixelsFlush(cd);
                for (int j = 0; j < figures[i].pixels.Count(); j++)
                {
                    pixels.Add(figures[i].pixels[j]);
                }
            }            
        }                

        public override void Resize(Point lc, int w, int h, CommonData cd)//根据新的坐标和宽高计算新的各属性
        {            
            location = cd.PblToCvl(new Point(lc.X, lc.Y + h));//新坐标
            this.width = cd.PblToCvl(w);
            this.height = cd.PblToCvl(h);            
            mid = new PointF(location.X + width / 2, location.Y + height / 2);//新中心
            for (int i = 0; i < figures.Count(); i++)
            {                
                PointF lCv = new PointF();
                lCv.X = location.X + figures[i].relativeLocation.X * width;
                lCv.Y = location.Y + figures[i].relativeLocation.Y * height;                
                int wN = cd.CvlToPbl(width * figures[i].relativeWidth);
                int hN = cd.CvlToPbl(height * figures[i].relativeHeight);
                Point l = cd.CvlToPbl(lCv);
                l.Y -= hN;
                figures[i].Resize(l,wN,hN,cd);
            }
            SizeFlush(cd);//计算规模
            Figures_SizeFlush(cd);//刷新子图元们的相对坐标            
            PixelsFlush(cd);
        }

        public override void Resize(CommonData cd)//相关图元点更新产生的resize
        {            
            SizeFlush(cd);//计算规模
            Figures_SizeFlush(cd);//刷新子图元们的相对坐标
            this.mid = new PointF(location.X + width / 2, location.Y + height / 2);
            this.iD = CreateId(cd);
            PixelsFlush(cd);//刷新像素点
            BaPointsFlush();//刷新图元点集合            
        }

        public override void Turn(float s, CommonData cd)//顺时针转s角度（mid为基点）
        {
            for (int i = 0; i < figures.Count(); i++)
            {
                figures[i].Turn(s, mid, cd);
            }
            for (int i = 0; i < baPoints.Count(); i++)
            {
                baPoints[i].hasResized = false;
            }
            SizeFlush(cd);//计算规模
            Figures_SizeFlush(cd);//刷新子图元们的相对坐标
            this.mid = new PointF(location.X + width / 2, location.Y + height / 2);                                                
            PixelsFlush(cd);
        }

        public override void Turn(float s,PointF m, CommonData cd)//顺时针转s角度（m为基点）
        {
            for (int i = 0; i < figures.Count(); i++)
            {
                figures[i].Turn(s, m, cd);
            }
            this.mid = this.PointTurn(this.mid,m,s);
            SizeFlush(cd);//计算规模
            Figures_SizeFlush(cd);//刷新子图元们的相对坐标
            this.mid = new PointF(location.X + width / 2, location.Y + height / 2);            
            PixelsFlush(cd);
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
            for (int i = 0; i < this.figures.Count(); i++)
            {
                figures[i].ReplacePoint(bP, tP);
            }
        }

        public override bool Select(Point p, CommonData cd)//通过一个点选择图元
        {
            for (int i = 0; i < figures.Count(); i++)
            {
                if (figures[i].Select(p, cd) == true)
                    return true;
            }
            return false;
        }

        public override FigureBase GetNewOne(CommonData cd)//重写克隆函数
        {
            FigureComposite resultF = new FigureComposite();

            //初始化图元子集合
            resultF.figures = new List<FigureBase>();
            for (int i = 0; i < figures.Count(); i++)
            {
                resultF.figures.Add(this.figures[i].GetNewOne(cd));
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
            resultF.baPoints = new List<BaPoint>();
            for (int i = 0; i < resultF.figures.Count(); i++)
            {
                for (int j = 0; j < resultF.figures[i].baPoints.Count(); j++)
                {
                    resultF.baPoints.Add(resultF.figures[i].baPoints[j]);
                }
            }
            resultF.isSelect = false;
            resultF.FigureKind = BaseFigure.COMPOSETE;
            resultF.thickness = this.thickness;
            resultF.color = this.color;
            return resultF;
        }

        public override FigureBase CopyNewOne(CommonData cd)//重写克隆函数
        {
            float d = cd.PblToCvl(10);
            FigureComposite resultF = new FigureComposite();
            //初始化子图元集合
            resultF.figures = new List<FigureBase>();
            for (int i = 0; i < this.figures.Count(); i++)
            {
                resultF.figures.Add(this.figures[i].CopyNewOne(cd));
            }
            resultF.location = new PointF(location.X + d, location.Y + d);
            resultF.width = this.width;
            resultF.height = this.height;
            resultF.relativeLocation = new PointF(this.relativeLocation.X, this.relativeLocation.Y);
            resultF.relativeWidth = this.relativeWidth;
            resultF.relativeHeight = this.height;
            resultF.mid = new PointF(mid.X + d, mid.Y + d);
            resultF.iD = CreateId(cd);
            resultF.PixelsFlush(cd);
            resultF.baPoints = new List<BaPoint>();
            for (int i = 0; i < resultF.figures.Count(); i++)
            {
                for (int j = 0; j < resultF.figures[i].baPoints.Count(); j++)
                {
                    resultF.baPoints.Add(resultF.figures[i].baPoints[j]);
                }
            }
            resultF.isSelect = false;
            resultF.FigureKind = BaseFigure.COMPOSETE;
            resultF.thickness = this.thickness;
            resultF.color = this.color;
            return resultF;
        }
    }
}
