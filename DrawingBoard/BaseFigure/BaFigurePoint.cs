using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DrawingBoard
{
    class BaFigurePoint:FigureBase//图元点类
    {
        public BaPoint point;//就是这个点

        public BaFigurePoint(CommonData cd)
        {
            this.FigureKind = BaseFigure.FIGUREPOINT;
            this.thickness = cd.thickness;
            this.color = cd.color;
        }

        public BaFigurePoint()
        {
        }

        public override void PixelsFlush(CommonData cd)//计算用于显示的像素点
        {
            point.PixelsFlush(cd);
        }

        public override void Draw(CommonData cd,Bitmap btm)//显示函数
        {
            //把像素点画出来
            for (int i = 0; i < pixels.Count; i++)
            {
                DrawPoint(cd, pixels[i].X, pixels[i].Y,btm);
            }
            point.Draw(cd,btm);            
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
            if (point.location.X == bP.location.X && point.location.Y == bP.location.Y)
            {
                point = tP;
            }
        }

        public override FigureBase GetNewOne(CommonData cd)//重写克隆函数
        {
            BaFigurePoint resultF = new BaFigurePoint();
            resultF.point = new BaPoint(point.location, cd);
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
            resultF.baPoints.Add(resultF.point);
            resultF.isSelect = false;
            resultF.FigureKind = BaseFigure.LINE;
            resultF.thickness = this.thickness;
            resultF.color = this.color;
            return resultF;
        }

        public override FigureBase CopyNewOne(CommonData cd)//重写克隆函数
        {
            float d = cd.PblToCvl(10);
            BaFigurePoint resultF = new BaFigurePoint();
            resultF.point = new BaPoint(new PointF(point.location.X + d, point.location.Y + d), cd);
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
            resultF.baPoints.Add(resultF.point);
            resultF.isSelect = false;
            resultF.FigureKind = BaseFigure.LINE;
            resultF.thickness = this.thickness;
            resultF.color = this.color;
            return resultF;
        }
    }
}
