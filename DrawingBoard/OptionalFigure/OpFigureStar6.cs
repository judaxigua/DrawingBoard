using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DrawingBoard
{
    class OpFigureStar6 : OptionalFigure
    {
        public OpFigureStar6(CommonData cd, PointF sp, PointF ep)
            : base(cd,sp,ep) //调用父类构造函数
        {
            relativePoints = new List<PointF>();
            relativePoints.Add(new PointF(0.5f,1f));
            relativePoints.Add(new PointF(0.67f, 0.75f));
            relativePoints.Add(new PointF(1f, 0.75f));
            relativePoints.Add(new PointF(0.85f, 0.5f));
            relativePoints.Add(new PointF(1f, 0.25f));
            relativePoints.Add(new PointF(0.67f, 0.25f));
            relativePoints.Add(new PointF(0.5f, 0f));
            relativePoints.Add(new PointF(0.33f, 0.25f));
            relativePoints.Add(new PointF(0f, 0.25f));
            relativePoints.Add(new PointF(0.15f, 0.5f));
            relativePoints.Add(new PointF(0f, 0.75f));
            relativePoints.Add(new PointF(0.33f, 0.75f));
            this.FigureKind = BaseFigure.STAR6;
            PixelsFlush(cd);
        }
    }
}
