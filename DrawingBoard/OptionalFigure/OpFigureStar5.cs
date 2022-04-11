using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DrawingBoard
{
    class OpFigureStar5 : OptionalFigure
    {
        public OpFigureStar5(CommonData cd, PointF sp, PointF ep)
            : base(cd,sp,ep) //调用父类构造函数
        {
            relativePoints = new List<PointF>();
            relativePoints.Add(new PointF(0.5f,1f));
            relativePoints.Add(new PointF(0.6f, 0.6f));
            relativePoints.Add(new PointF(1f, 0.6f));
            relativePoints.Add(new PointF(0.7f, 0.4f));
            relativePoints.Add(new PointF(0.8f, 0f));
            relativePoints.Add(new PointF(0.5f, 0.2f));
            relativePoints.Add(new PointF(0.2f, 0f));
            relativePoints.Add(new PointF(0.3f, 0.4f));
            relativePoints.Add(new PointF(0f, 0.6f));
            relativePoints.Add(new PointF(0.4f, 0.6f));
            this.FigureKind = BaseFigure.STAR5;
            PixelsFlush(cd);
        }
    }
}
