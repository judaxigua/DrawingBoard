using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DrawingBoard
{
    class FigureBase//图元的基类
    {
        //图元基本属性
        public PointF location;//图元在画布上的坐标
        public float width;//图元的宽度
        public float height;//图元的高度
        public PointF mid;//图元的中心
        public string iD;//图元ID
        public BaseFigure FigureKind;//图元类别
        public int thickness;//线条粗细        
        public Color color;//图元颜色
        public LineStyle lineStyle;//线形
        //其他属性
        public PointF relativeLocation = new PointF(0, 0);//如果这个图元是组合图形的子图元，他的相对坐标 于是范围在(0,0)-(1,1)之间
        public float relativeWidth = 0;//相对宽度
        public float relativeHeight = 0;//相对高度
        public int index = -1;//此图元在图元管理器列表中的下标
        public Boolean isSelect = false;//是否被选中                
        //绘图相关
        public List<Point> pixels = new List<Point>();//用于绘画的像素点
        public List<BaPoint> baPoints = new List<BaPoint>();//所有图元点的集合                
        public Bitmap btmTemp;//用作计算像素点的临时位图
        public Boolean isBeenEdit = false;//是否处于编辑状态，不是则只需进入缓冲区即可
        public int lineMatrixIndex = 0;//绘图线形相关的下标

        public virtual void Draw(CommonData cd,Bitmap btm)//将像素集合的点全部画出来
        {            
        }

        public virtual void PixelsFlush(CommonData cd)//计算用于显示的像素点
        {
        }

        public virtual Boolean Select(Point p, CommonData cd)//图形选定的虚函数
        {
            return false;
        }

        public virtual void SetIsBeenEdit(Boolean b)//设置自己以及相关图元的edit状态
        {
            this.isBeenEdit = b;
            for (int i = 0; i < baPoints.Count(); i++)
            {
                baPoints[i].isBeenEdit = b;
                for (int j = 0; j < baPoints[i].fathers.Count(); j++)
                {
                    baPoints[i].fathers[j].isBeenEdit = b;
                    for (int n = 0; n < baPoints[i].fathers[j].baPoints.Count(); n++)
                    {
                        baPoints[i].fathers[j].baPoints[n].isBeenEdit = b;
                    }
                }
            }
        }

        public virtual void Resize(Point lc, int w, int h, CommonData cd)//属性调整的虚函数
        {
        }

        public virtual void Resize(CommonData cd)//重载 ，根据自身点集合调整属性
        {
 
        }

        public virtual void Turn(float s, CommonData cd)//顺时针转s角度
        {
        }

        public virtual void Turn(float s,PointF m, CommonData cd)//以m为圆心 顺时针转s角度
        {
        }

        public virtual void ReplacePoint(BaPoint bP, BaPoint tP)//替换其中的点 bP被替换的点，tP替换目标 
        {
        }

        public virtual FigureBase GetNewOne(CommonData cd)//克隆自己
        {
            return null;
        }

        public virtual FigureBase CopyNewOne(CommonData cd)//克隆自己
        {
            return null;
        }               

        public string LocationToString()//将坐标转为字符串
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            sb.Append(location.X+"");
            sb.Append(",");
            sb.Append(location.Y + "");
            sb.Append(")");
            return sb.ToString();
        }

        public string KindToString()
        {
            switch (this.FigureKind)
            {
                case BaseFigure.ARROWDOWN: return "ARROWDOWN";
                case BaseFigure.ARROWLEFT: return "ARROWLEFT";
                case BaseFigure.ARROWRIGHT: return "ARROWRIGHT";
                case BaseFigure.ARROWUP: return "ARROWUP";
                case BaseFigure.BRUSH: return "BRUSH";
                case BaseFigure.BUBBLEELLIPSE: return "BUBBLEELLIPSE";
                case BaseFigure.BUBBLERECT: return "BUBBLERECT";
                case BaseFigure.CHAR: return "CHAR";
                case BaseFigure.CIRCLE: return "CIRCLE";
                case BaseFigure.COLORFILL: return "COLORFILL";
                case BaseFigure.COLORGET: return "COLORGET";
                case BaseFigure.CURVE: return "CURVE";
                case BaseFigure.DIAMOND: return "DIAMOND";
                case BaseFigure.ELLIPSE: return "ELLIPSE";
                case BaseFigure.FIGUREPOINT: return "FIGUREPOINT";
                case BaseFigure.LINE: return "LINE";
                case BaseFigure.PEN: return "PEN";
                case BaseFigure.PENTAGON: return "PENTAGON";
                case BaseFigure.POINT: return "POINT";
                case BaseFigure.POLYGON: return "POLYGON";
                case BaseFigure.RECT: return "RECT";
                case BaseFigure.RUBBER: return "RUBBER";
                case BaseFigure.SEXANGLE: return "SEXANGLE";
                case BaseFigure.STAR4: return "STAR4";
                case BaseFigure.STAR5: return "STAR5";
                case BaseFigure.STAR6: return "STAR6";
                case BaseFigure.STARLOVE: return "STARLOVE";
                case BaseFigure.TRIANGLE: return "TRIANGLE";
                case BaseFigure.COMPOSETE: return "COMPOSETE";
                default: return null;
            }
        }        

        public void ResizeToPoints(CommonData cd)//因为点而产生的resize
        {
            for (int i = 0; i < baPoints.Count(); i++)
            {
                baPoints[i].FathersResize(cd);
            }
        }

        public int BaPointNum()
        {
            return baPoints.Count();
        }

        public string CreateId(CommonData cd)//创建一个新的ID
        {
            string resultS;
            System.DateTime currentTime = new System.DateTime();
            currentTime = System.DateTime.Now;
            resultS = currentTime.ToString() + cd.idNum;
            cd.idNum++;
            return resultS;
        } 

        public void PointsMagnetFlush()//刷新所有点的吸附状态
        {
            for (int i = 0; i < baPoints.Count(); i++)
            {
                baPoints[i].magentTime = 0;
                baPoints[i].isMagnet = true;
            }
        }

        public PointF PointResize(Point lc,int w,int h,CommonData cd,PointF pf)//在resize时按照比例重新确定点的方位
        {
            PointF lcP = cd.PblToCvl(new Point(lc.X, lc.Y + h));
            float wP = cd.PblToCvl(w);
            float hP = cd.PblToCvl(h);
            float x = wP * (pf.X - location.X) / width + lcP.X;
            float y = hP * (pf.Y - location.Y) / height + lcP.Y;
            return new PointF(x, y);
        }

        public PointF PointTurn(PointF p,float s)//以mid为基点顺时针旋转s角度
        {
            PointF p0 = new PointF();
            p0.X = p.X - mid.X;
            p0.Y = p.Y - mid.Y;
            PointF resultP = new PointF();
            resultP.X = (float)(p0.X * Math.Cos(-s) - p0.Y * Math.Sin(-s));
            resultP.Y = (float)(p0.X * Math.Sin(-s) + p0.Y * Math.Cos(-s));
            resultP.X += mid.X;
            resultP.Y += mid.Y;
            return resultP;
        }

        public PointF PointTurn(PointF p,PointF m, float s)//以m为基点顺时针旋转s角度
        {
            PointF p0 = new PointF();
            p0.X = p.X - m.X;
            p0.Y = p.Y - m.Y;
            PointF resultP = new PointF();
            resultP.X = (float)(p0.X * Math.Cos(-s) - p0.Y * Math.Sin(-s));
            resultP.Y = (float)(p0.X * Math.Sin(-s) + p0.Y * Math.Cos(-s));
            resultP.X += m.X;
            resultP.Y += m.Y;
            return resultP;
        }

        public Point PointTurn(Point p, float s,CommonData cd)//重载，以mid为基点顺时针旋转s角度（相对于画板而言）
        {
            PointF p0 = new PointF();
            PointF pP = cd.PblToCvl(p);
            p0.X = pP.X - mid.X;
            p0.Y = pP.Y - mid.Y;
            PointF resultP = new PointF();
            resultP.X = (float)(p0.X * Math.Cos(-s) - p0.Y * Math.Sin(-s));
            resultP.Y = (float)(p0.X * Math.Sin(-s) + p0.Y * Math.Cos(-s));
            resultP.X += mid.X;
            resultP.Y += mid.Y;
            return cd.CvlToPbl(resultP);            
        }

        public Point PointTurn(Point p, float s,PointF m, CommonData cd)//重载，以mid为基点顺时针旋转s角度（相对于画板而言）
        {
            PointF p0 = new PointF();
            PointF pP = cd.PblToCvl(p);
            p0.X = pP.X - m.X;
            p0.Y = pP.Y - m.Y;
            PointF resultP = new PointF();
            resultP.X = (float)(p0.X * Math.Cos(-s) - p0.Y * Math.Sin(-s));
            resultP.Y = (float)(p0.X * Math.Sin(-s) + p0.Y * Math.Cos(-s));
            resultP.X += m.X;
            resultP.Y += m.Y;
            return cd.CvlToPbl(resultP);
        }                

        public void DrawPoint(CommonData cd, int x, int y,Bitmap btm)//画一个点
        {
            btm.SetPixel(x, y, color);            
        }

        public void PixelBrush(int x, int y, int th,CommonData cd)//用方块刷子法计算像素点
        {
            if (cd.lineStyleMatrix[thickness][(int)lineStyle][lineMatrixIndex] == 0)
                return;
            int d = (th - 1) / 2;
            for (int i = x - d; i < x + d + 1; i++)
            {
                for (int j = y - d; j < y + d + 1; j++)
                {
                    if (i <= 0 || i >= cd.canvas.Width || j <= 0 || j >= cd.canvas.Height)
                        continue;//如果越界进行下一个循环
                    Color c = btmTemp.GetPixel(i,j);
                    if (c.A == 0 && c.R == 0 && c.G == 0 && c.B == 0)
                    {
                        pixels.Add(new Point(i,j));
                        btmTemp.SetPixel(i,j,color);
                    }
                }
            }
        }

        //生成直线像素点的算法
        public void GetLinePixels(Point sp, Point ep, CommonData cd)//计算直线的像素点
        {
            lineMatrixIndex = 0;
            int deltaX = ep.X - sp.X, deltaY = ep.Y - sp.Y;
            int a = sp.Y - ep.Y, b = ep.X - sp.X;
            int d = 2 * a + b, d1 = 2 * a, d2 = 2 * (a + b);
            if (Math.Abs(deltaY) <= Math.Abs(deltaX))
            {
                if (deltaX >= 0 && deltaY >= 0) GetX1Pixels(cd, sp, ep);
                if (deltaX <= 0 && deltaY >= 0) GetX2Pixels(cd, sp, ep);
                if (deltaX <= 0 && deltaY <= 0) GetX3Pixels(cd, sp, ep);
                if (deltaX >= 0 && deltaY <= 0) GetX4Pixels(cd, sp, ep);
            }
            else
            {
                if (deltaX >= 0 && deltaY >= 0) GetY1Pixels(cd, sp, ep);
                if (deltaX <= 0 && deltaY >= 0) GetY2Pixels(cd, sp, ep);
                if (deltaX <= 0 && deltaY <= 0) GetY3Pixels(cd, sp, ep);
                if (deltaX >= 0 && deltaY <= 0) GetY4Pixels(cd, sp, ep);
            }
        }
        //八中画直线的情况
        public void GetX1Pixels(CommonData cd, Point sp, Point ep)
        {
            int a = sp.Y - ep.Y, b = ep.X - sp.X;
            int d = 2 * a + b, d1 = 2 * a, d2 = 2 * (a + b);
            PixelBrush(sp.X,sp.Y,thickness,cd);
            lineMatrixIndex = (lineMatrixIndex + 1) % (6 * thickness);
            for (int x = sp.X + 1, y = sp.Y; x < ep.X; x++)
            {
                if (d >= 0) d = d + d1;
                else { y++; d = d + d2; }
                PixelBrush(x, y, thickness, cd);
                lineMatrixIndex = (lineMatrixIndex + 1) % (6 * thickness);
            }
        }
        public void GetX2Pixels(CommonData cd, Point sp, Point ep)
        {
            int a = sp.Y - ep.Y, b = ep.X - sp.X;
            int d = b - 2 * a, d1 = 2 * (b - a), d2 = -2 * a;
            PixelBrush(sp.X, sp.Y, thickness, cd);
            lineMatrixIndex = (lineMatrixIndex + 1) % (6 * thickness);
            for (int x = sp.X - 1, y = sp.Y; x > ep.X; x--)
            {
                if (d >= 0) { y++; d = d + d1; }
                else d = d + d2;
                PixelBrush(x, y, thickness, cd);
                lineMatrixIndex = (lineMatrixIndex + 1) % (6 * thickness);
            }
        }
        public void GetX3Pixels(CommonData cd, Point sp, Point ep)
        {
            int a = sp.Y - ep.Y, b = ep.X - sp.X;
            int d = -2 * a - b, d1 = -2 * a, d2 = -2 * (a + b);
            PixelBrush(sp.X, sp.Y, thickness, cd);
            lineMatrixIndex = (lineMatrixIndex + 1) % (6 * thickness);
            for (int x = sp.X - 1, y = sp.Y; x > ep.X; x--)
            {
                if (d >= 0) d = d + d1;
                else { y--; d = d + d2; }
                PixelBrush(x, y, thickness, cd);
                lineMatrixIndex = (lineMatrixIndex + 1) % (6 * thickness);
            }
        }
        public void GetX4Pixels(CommonData cd, Point sp, Point ep)
        {
            int a = sp.Y - ep.Y, b = ep.X - sp.X;
            int d = 2 * a - b, d1 = 2 * (a - b), d2 = 2 * a;
            PixelBrush(sp.X, sp.Y, thickness, cd);
            lineMatrixIndex = (lineMatrixIndex + 1) % (6 * thickness);
            for (int x = sp.X + 1, y = sp.Y; x < ep.X; x++)
            {
                if (d >= 0) { y--; d = d + d1; }
                else d = d + d2;
                PixelBrush(x, y, thickness, cd);
                lineMatrixIndex = (lineMatrixIndex + 1) % (6 * thickness);
            }
        }
        public void GetY1Pixels(CommonData cd, Point sp, Point ep)
        {
            int a = sp.Y - ep.Y, b = ep.X - sp.X;
            int d = a + 2 * b, d1 = 2 * (a + b), d2 = 2 * b;
            PixelBrush(sp.X, sp.Y, thickness, cd);
            lineMatrixIndex = (lineMatrixIndex + 1) % (6 * thickness);
            for (int x = sp.X, y = sp.Y + 1; y < ep.Y; y++)
            {
                if (d >= 0) { x++; d = d + d1; }
                else d = d + d2;
                PixelBrush(x, y, thickness, cd);
                lineMatrixIndex = (lineMatrixIndex + 1) % (6 * thickness);
            }
        }
        public void GetY2Pixels(CommonData cd, Point sp, Point ep)
        {
            int a = sp.Y - ep.Y, b = ep.X - sp.X;
            int d = -a + 2 * b, d1 = 2 * b, d2 = 2 * (b - a);
            PixelBrush(sp.X, sp.Y, thickness, cd);
            lineMatrixIndex = (lineMatrixIndex + 1) % (6 * thickness);
            for (int x = sp.X, y = sp.Y + 1; y < ep.Y; y++)
            {
                if (d >= 0) d = d + d1;
                else { x--; d = d + d2; }
                PixelBrush(x, y, thickness, cd);
                lineMatrixIndex = (lineMatrixIndex + 1) % (6 * thickness);
            }
        }
        public void GetY3Pixels(CommonData cd, Point sp, Point ep)
        {
            int a = sp.Y - ep.Y, b = ep.X - sp.X;
            int d = -a - 2 * b, d1 = -2 * (a + b), d2 = -2 * b;
            PixelBrush(sp.X, sp.Y, thickness, cd);
            lineMatrixIndex = (lineMatrixIndex + 1) % (6 * thickness);
            for (int x = sp.X, y = sp.Y - 1; y > ep.Y; y--)
            {
                if (d >= 0) { x--; d = d + d1; }
                else d = d + d2;
                PixelBrush(x, y, thickness, cd);
                lineMatrixIndex = (lineMatrixIndex + 1) % (6 * thickness);
            }
        }
        public void GetY4Pixels(CommonData cd, Point sp, Point ep)
        {
            int a = sp.Y - ep.Y, b = ep.X - sp.X;
            int d = a - 2 * b, d1 = -2 * b, d2 = 2 * (a - b);
            PixelBrush(sp.X, sp.Y, thickness, cd);
            lineMatrixIndex = (lineMatrixIndex + 1) % (6 * thickness);
            for (int x = sp.X, y = sp.Y - 1; y > ep.Y; y--)
            {
                if (d >= 0) d = d + d1;
                else { x++; d = d + d2; }
                PixelBrush(x, y, thickness, cd);
                lineMatrixIndex = (lineMatrixIndex + 1) % (6 * thickness);
            }
        }

        public PointF GetPMin(PointF sp, PointF ep)//获得xy较小的点
        {
            PointF resultP = new PointF();
            resultP.X = sp.X < ep.X ? sp.X : ep.X;
            resultP.Y = sp.Y < ep.Y ? sp.Y : ep.Y;
            return resultP;
        }

        public PointF GetPMax(PointF sp, PointF ep)//获得xy较大的点
        {
            PointF resultP = new PointF();
            resultP.X = sp.X > ep.X ? sp.X : ep.X;
            resultP.Y = sp.Y > ep.Y ? sp.Y : ep.Y;
            return resultP;
        }

        public Point GetPMin(Point sp, Point ep)//重载，获得xy较小的点
        {
            Point resultP = new Point();
            resultP.X = sp.X < ep.X ? sp.X : ep.X;
            resultP.Y = sp.Y < ep.Y ? sp.Y : ep.Y;
            return resultP;
        }

        public Point GetPMax(Point sp, Point ep)//重载，获得xy较大的点
        {
            Point resultP = new Point();
            resultP.X = sp.X > ep.X ? sp.X : ep.X;
            resultP.Y = sp.Y > ep.Y ? sp.Y : ep.Y;
            return resultP;
        }

        public float GetMax(float l1, float l2)//获得两条线断中较大的那条
        {
            return l1 > l2 ? l1 : l2;
        }

        public int GetMin(int l1, int l2)//获得两条线断中较小的那条
        {
            return l1 < l2 ? l1 : l2;
        }

        public int GetMax(int l1, int l2)//获得两条线断中较大的那条
        {
            return l1 > l2 ? l1 : l2;
        }

        public float GetMin(float l1, float l2)//获得两条线断中较小的那条
        {
            return l1 < l2 ? l1 : l2;
        }   
    }
}
