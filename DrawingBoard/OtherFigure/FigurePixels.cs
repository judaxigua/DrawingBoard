using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DrawingBoard
{
    class FigurePixels//像素点图元
    {
        public List<PixelPointF> cores = new List<PixelPointF>();//像素块中心
        public List<PixelPoint> pixels = new List<PixelPoint>();//像素点

        public void Draw(CommonData cd)//画出来
        {
            for (int i = 0; i < pixels.Count(); i++)
            {                
                cd.canvas.SetPixel(pixels[i].location.X,pixels[i].location.Y,pixels[i].color);
            }
        }

        public void PixelFlush(CommonData cd)//刷新像素点
        {
            pixels = new List<PixelPoint>();
            Bitmap btm = new Bitmap(cd.canvas.Width,cd.canvas.Height);
            int w = cd.CvlToPbl(1);
            w /= 2;
            w++;
            for(int n=0;n<cores.Count();n++)
            {
                Point p = cd.CvlToPbl(cores[n].location);
                for (int i = p.X - w; i < p.X + w + 1; i++)
                {
                    for (int j = p.Y - w; j < p.Y + w + 1; j++)
                    {
                        if (i > 0 && i < btm.Width && j > 0 && j < btm.Height)
                        {
                            Color c = btm.GetPixel(i, j);
                            if (c == Color.FromArgb(0, 0, 0, 0))
                            {
                                pixels.Add(new PixelPoint(i,j,cores[n].color));
                                btm.SetPixel(i, j, cores[n].color);
                            }
                        }
                    }
                }
            }
        }
    }
}
