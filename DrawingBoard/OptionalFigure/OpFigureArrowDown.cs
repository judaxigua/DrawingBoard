using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DrawingBoard
{
    class OpFigureArrowDown : OptionalFigure//自选图形 下箭头
    {
        public OpFigureArrowDown(CommonData cd, PointF sp, PointF ep)
            : base(cd,sp,ep) //调用父类构造函数
        {
            relativePoints = new List<PointF>();
            relativePoints.Add(new PointF(0.75f, 1f));
            relativePoints.Add(new PointF(0.75f, 0.5f));
            relativePoints.Add(new PointF(1f, 0.5f));
            relativePoints.Add(new PointF(0.5f, 0f));
            relativePoints.Add(new PointF(0f, 0.5f));
            relativePoints.Add(new PointF(0.25f, 0.5f));
            relativePoints.Add(new PointF(0.25f, 1f));
            this.FigureKind = BaseFigure.ARROWDOWN;
            PixelsFlush(cd);
        }
    }
}
