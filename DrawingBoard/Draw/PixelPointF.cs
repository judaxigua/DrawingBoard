using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DrawingBoard
{
    class PixelPointF//逻辑像素块中心
    {
        public PointF location;//逻辑坐标
        public Color color;//颜色

        public PixelPointF(float x, float y, Color c)
        {
            location = new PointF(x, y);
            color = c;
        }

        public PixelPointF(PointF p, Color c)
        {
            location = new PointF(p.X, p.Y);
            color = c;
        }
    }
}
