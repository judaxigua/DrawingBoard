using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DrawingBoard
{
    class OpFigurePentagon:OptionalFigure//自选图形五边形
    {
        public OpFigurePentagon(CommonData cd, PointF sp, PointF ep)
            : base(cd,sp,ep) //调用父类构造函数
        {
            relativePoints = new List<PointF>();
            relativePoints.Add(new PointF(0.5f,1f));
            relativePoints.Add(new PointF(1f, 0.6f));
            relativePoints.Add(new PointF(0.8f, 0f));
            relativePoints.Add(new PointF(0.2f, 0f));
            relativePoints.Add(new PointF(0f, 0.6f));
            this.FigureKind = BaseFigure.PENTAGON;
            PixelsFlush(cd);
        }
    }
}
