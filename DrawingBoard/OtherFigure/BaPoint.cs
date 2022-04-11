using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DrawingBoard
{
    class BaPoint : FigureBase
    {        
        public Boolean isVisible;
        public Boolean isMagnet;//是否可吸附
        public int magentTime = 0;
        public List<FigureBase> fathers = new List<FigureBase>();
        public Boolean hasResized = false;

        public BaPoint(PointF p,CommonData cd)
        {
            this.FigureKind = BaseFigure.POINT;            
            location = p;//点的逻辑坐标
            isVisible = true;//是否要画这个点
            isMagnet = true;
            thickness = cd.thickness+2;            
            color = Color.Blue;
        }

        public void Turn(PointF m,float s,CommonData cd)//旋转
        {
            if (hasResized == true)
                return;
            location = this.PointTurn(location, m, s);//旋转起点
            this.hasResized = true;
            PixelsFlush(cd);
        }

        public override void Draw(CommonData cd,Bitmap btm)//显示
        {
            for (int i = 0; i < pixels.Count(); i++)
            {
                DrawPoint(cd,pixels[i].X, pixels[i].Y,btm);
            }
        }

        public override void PixelsFlush(CommonData cd)//用填充法计算图元像素
        {
            Bitmap btm = new Bitmap(cd.canvas.Width, cd.canvas.Height);
            if (isVisible == false)
                return;
            Point cp = new Point(cd.canvas.Width / 2, cd.canvas.Height/2);
            int rad = thickness;//半径

            //先在虚拟画板上画出圆的轮廓
            int d = 1 - rad; Point p = new Point(0, rad);
            while (p.X <= p.Y)
            {
                if (p.X == p.Y)
                {
                    SetPixel(btm,cp.X + p.X, cp.Y + p.Y, Color.Blue);
                    SetPixel(btm, cp.X - p.X, cp.Y + p.Y, Color.Blue); ;
                    SetPixel(btm, cp.X + p.X, cp.Y - p.Y, Color.Blue);
                    SetPixel(btm, cp.X - p.X, cp.Y - p.Y, Color.Blue);
                }
                SetPixel(btm, cp.X + p.X, cp.Y + p.Y, Color.Blue);
                SetPixel(btm, cp.X + p.X, cp.Y - p.Y, Color.Blue);
                SetPixel(btm, cp.X - p.X, cp.Y + p.Y, Color.Blue);
                SetPixel(btm, cp.X - p.X, cp.Y - p.Y, Color.Blue);
                SetPixel(btm, cp.X + p.Y, cp.Y + p.X, Color.Blue);
                SetPixel(btm, cp.X + p.Y, cp.Y - p.X, Color.Blue);
                SetPixel(btm, cp.X - p.Y, cp.Y + p.X, Color.Blue);
                SetPixel(btm, cp.X - p.Y, cp.Y - p.X, Color.Blue);
                p.X++;
                if (d < 0) d = d + 2 * p.X + 1;
                else { p.Y--; d = d + 2 * p.X - 2 * p.Y + 1; }
            }

            pixels = new List<Point>();
            PixelExpand(btm, cp.X, cp.Y, Color.FromArgb(0, 0, 0, 0),cd);
        }        

        public Boolean PixelExpand(Bitmap btm, int x, int y, Color c,CommonData cd)//填充图扩展的递归算法
        {
            if (IsExpand(btm, x, y, c,cd) == false)
                return false;
            PixelExpand(btm, x - 1, y, c,cd);
            PixelExpand(btm, x + 1, y, c,cd);
            PixelExpand(btm, x, y - 1, c,cd);
            PixelExpand(btm, x, y + 1, c,cd);
            return true;
        }

        public Boolean IsExpand(Bitmap btm, int x, int y, Color c,CommonData cd)//判断是否扩展这个像素点
        {
            if (x < 0 || x >= btm.Width || y < 0 || y >= btm.Height)
                return false;
            Color ct = btm.GetPixel(x, y);
            if (ct != c)
                return false;
            Point p = cd.CvlToPbl(location);
            Point pl = new Point(p.X + x - cd.canvas.Width / 2, p.Y + y - cd.canvas.Height / 2);
            if (pl.X > 0 && pl.X < cd.canvas.Width && pl.Y > 0 && pl.Y < cd.canvas.Height)
                pixels.Add(new Point(pl.X,pl.Y));
            btm.SetPixel(x, y, Color.FromArgb(100, 100, 100, 100));
            return true;
        }

        public void SetPixel(Bitmap btm, int x, int y, Color c)//计算时的单点塞入
        {
            if (x > 0 && x < btm.Width && y > 0 && y < btm.Height)
                btm.SetPixel(x, y, c);
        }

        public override void SetIsBeenEdit(Boolean b)//设置自己以及相关图元的edit状态
        {
            this.isBeenEdit = b;
            for (int i = 0; i < this.fathers.Count(); i++)
            {
                fathers[i].isBeenEdit = b;
                for (int j = 0; j < fathers[i].baPoints.Count(); j++)
                {
                    fathers[i].baPoints[j].isBeenEdit = b;
                }
            }
        }
 
        public void PointResize(Point lc,PointF LC, int w, int h,float W,float H, CommonData cd)//在resize时按照比例重新确定点的方位
        {
            if (hasResized == true)
                return;
            PointF lcP = cd.PblToCvl(new Point(lc.X, lc.Y + h));
            float wP = cd.PblToCvl(w);
            float hP = cd.PblToCvl(h);
            float x = wP * (location.X - LC.X) / W + lcP.X;
            float y = hP * (location.Y - LC.Y) / H + lcP.Y;
            location = new PointF(x, y);
            hasResized = true;
            PixelsFlush(cd);
        }

        public override void Resize(Point lc, int w, int h, CommonData cd)//根据新的坐标和宽高计算新的各属性
        {
            location = cd.PblToCvl(lc);
            FathersResize(cd);
            PixelsFlush(cd);
        }    

        public void FathersResize(CommonData cd)//这个点刷新时带动相关图元 一起刷新
        {
            for (int i = 0; i < fathers.Count(); i++)
            {
                fathers[i].Resize(cd);
            }
        }

        public override Boolean Select(Point p, CommonData cd)//通过距离判定来选定
        {
            Point pP = cd.CvlToPbl(location);
            int d = (int)cd.GetDistance(p, pP);
            if (d <= 5)
                return true;
            else
                return false;
        }

        public Boolean IsBeMagnet(Point p, CommonData cd)//判断和pictureBox上的一个点是否发生吸附关系
        {
            if (this.isMagnet == false)//如果这个点不能吸附直接否
                return false;
            Point l = cd.CvlToPbl(location);
            int d = (int)Math.Sqrt((p.X - l.X) * (p.X - l.X) + (p.Y - l.Y) * (p.Y - l.Y));
            if (d <= cd.magnetDistance)
                return true;
            else
                return false;
        }        
    }
}
