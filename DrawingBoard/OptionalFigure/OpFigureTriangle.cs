using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DrawingBoard
{
    class OpFigureTriangle: OptionalFigure
    {
        public OpFigureTriangle(CommonData cd, PointF sp, PointF ep)
            : base(cd,sp,ep) //调用父类构造函数
        {
            relativePoints = new List<PointF>();
            relativePoints.Add(new PointF(0.5f,1f));
            relativePoints.Add(new PointF(1f, 0f));
            relativePoints.Add(new PointF(0f, 0f));
            this.FigureKind = BaseFigure.TRIANGLE;
            PixelsFlush(cd);
        }
    }
}
