using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DrawingBoard
{
    class PixelPoint//物理像素点类
    {
        public Point location;//物理坐标
        public Color color;//颜色

        public PixelPoint(int x, int y, Color c)
        {
            location = new Point(x, y);
            color = c;
        }

        public PixelPoint(Point p, Color c)
        {
            location = new Point(p.X, p.Y);
            color = c;
        }
    }
}
