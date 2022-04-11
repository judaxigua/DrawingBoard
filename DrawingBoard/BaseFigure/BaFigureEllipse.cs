using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DrawingBoard
{
    class BaFigureEllipse : FigureBase//基本图形椭圆类
    {        
        public BaPoint pC1, pC2;//椭圆的两个焦点
        public float a, b, c;//椭圆的长半轴，短半轴，焦距 
        public float the;//下下策，设置一个从正椭圆旋转的角度

        public BaFigureEllipse(CommonData cd)//构造函数
        {
            this.FigureKind = BaseFigure.ELLIPSE;
            this.thickness = cd.thickness;
            this.color = cd.color;
            this.lineStyle = cd.lineStyle;
        }

        public BaFigureEllipse()//空的构造函数
        {
        }        

        public override void Draw(CommonData cd,Bitmap btm)//显示图形
        {
            //pC1.Draw(cd);
            //pC2.Draw(cd);
            //把像素点画出来
            for (int i = 0; i < pixels.Count; i++)
            {
                DrawPoint(cd, pixels[i].X, pixels[i].Y,btm);
            } 
        }

        public override void Turn(float s, CommonData cd)//顺时针转s角度（mid为基点）
        {
            //旋转各焦点            
            PointF pc1 = this.PointTurn(pC1.location, s);
            PointF pc2 = this.PointTurn(pC2.location, s);
            pC1.location = new PointF(pc1.X, pc1.Y);
            pC2.location = new PointF(pc2.X, pc2.Y);            
            the += s;
            Point min = cd.GetMin(pixels);
            Point max = cd.GetMax(pixels);
            this.location = cd.PblToCvl(new Point(min.X,max.Y));
            this.width = cd.PblToCvl(max.X - min.X);
            this.height = cd.PblToCvl(max.Y - min.Y);            
            PixelsFlush(cd);
        }

        public override void Turn(float s,PointF m, CommonData cd)//顺时针转s角度（m为基点）
        {
            //旋转各焦点            
            pC1.Turn(m, s,cd);
            pC2.Turn(m, s,cd);
            mid = this.PointTurn(mid,m,s);
            the += s;
            Point min = cd.GetMin(pixels);
            Point max = cd.GetMax(pixels);
            this.location = cd.PblToCvl(new Point(min.X, max.Y));
            this.width = cd.PblToCvl(max.X - min.X);
            this.height = cd.PblToCvl(max.Y - min.Y);            
            PixelsFlush(cd);
        }

        public override void Resize(Point lc, int w, int h, CommonData cd)//重写属性调整
        {            
            //注意lc是相对于画板的左上角坐标
            //未解决的事：方便起见，每次进行resize时我们都把椭圆重新转成“正”的
            //得到各顶点的画板坐标
            location = cd.PblToCvl(new Point(lc.X, lc.Y + h));//新的坐标
            width = cd.PblToCvl(w);//新宽
            height = cd.PblToCvl(h);//新高
            a = width / 2;//新的半轴a
            b = height / 2;//新的半轴b
            c = (float)Math.Sqrt(Math.Abs(Math.Pow(b, 2) - Math.Pow(a, 2)));//新的半轴c            
            mid = new PointF(location.X + width / 2, location.Y + height / 2);//新中心
            if (a >= b)
            {
                pC1.location = new PointF(mid.X - c, mid.Y);                
                pC2.location = new PointF(mid.X + c, mid.Y);
            }
            else
            {
                pC1.location = new PointF(mid.X, mid.Y - c);
                pC2.location = new PointF(mid.X, mid.Y + c);
            }//新的焦点
            the = 0;//角度重置为0
            //ResizeToPoints(cd);
            PixelsFlush(cd);
        }

        public override bool Select(Point p, CommonData cd)//通过一个点选择图元
        {
            if (isSelect == true)//已经选定了就不能再选了
            {
                return false;
            }
            ///创建一个虚拟画板
            Bitmap image = new Bitmap(cd.picBoxWidth, cd.picBoxHeight);
            Graphics g = Graphics.FromImage(image);
            Color c = Color.FromArgb(0, 0, 1);///设置背景色特殊 0，0，1
            g.Clear(Color.White);
            ///加粗原图形后画在画板上
            for (int i = 0; i < pixels.Count; i++)
            {
                DrawPoint(cd, pixels[i].X, pixels[i].Y,image);
                if (pixels[i].X > 0 && pixels[i].X < image.Width && pixels[i].Y > 0 && pixels[i].Y < image.Height)//判断是否越界
                {
                    image.SetPixel(pixels[i].X, pixels[i].Y, c);
                }
                
            }
            ///取色，比较后是否选定
            Color cget;
            cget = image.GetPixel(p.X, p.Y);
            if (cget.B == 1 && cget.G == 0 && cget.R == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void PixelsFlush(CommonData cd)//重新计算用于显示的像素点
        {
            btmTemp = new Bitmap(cd.pictureBox.Width, cd.pictureBox.Height);    
            this.pixels = new List<Point>();
            GetEllpisePixels(cd);
            btmTemp = new Bitmap(cd.pictureBox.Width, cd.pictureBox.Height);    
        }

        public void GetEllpisePixels(CommonData cd)//用椭圆的中点生成算法计算像素点并且保存
        {
            lineMatrixIndex = 0;
            Point pc;
            int aP = cd.CvlToPbl(a),bP = cd.CvlToPbl(b),cP = cd.CvlToPbl(c);
            int aa=aP*aP, bb=bP*bP, twoaa=2*aa, twobb=2*bb; 
            Point p = new Point(0,cd.CvlToPbl(b));
            Point cp = new Point((cd.CvlToPbl(pC1.location).X+cd.CvlToPbl(pC2.location).X)/2,(cd.CvlToPbl(pC1.location).Y+cd.CvlToPbl(pC2.location).Y)/2);
            pc = PointTurn(new Point(cp.X + p.X, cp.Y + p.Y), the, cd);
            PixelBrush(pc.X, pc.Y, thickness, cd);
            pc = PointTurn(new Point(cp.X + p.X, cp.Y - p.Y), the, cd);
            PixelBrush(pc.X, pc.Y, thickness, cd);
            int dy=0, dx=twoaa*p.Y;
            /* 上半弧 */
            int d=(int)(bb+aa*(-b+0.25)+0.5);
            while(dx>dy) 
	        { 
                p.X++;    dy=dy+twobb; 
                if(d<0)   d=d+bb+dy;
                else	{ p.Y--;  dx=dx-twoaa;  d=d+bb+dy-dx; }                                
                pc = PointTurn(new Point(cp.X + p.X, cp.Y + p.Y), the, cd);
                PixelBrush(pc.X, pc.Y, thickness, cd);                               
                pc = PointTurn(new Point(cp.X + p.X, cp.Y - p.Y), the, cd);
                PixelBrush(pc.X, pc.Y, thickness, cd);                
                pc = PointTurn(new Point(cp.X - p.X, cp.Y + p.Y), the, cd);
                PixelBrush(pc.X, pc.Y, thickness, cd);                
                pc = PointTurn(new Point(cp.X - p.X, cp.Y - p.Y), the, cd);
                PixelBrush(pc.X, pc.Y, thickness, cd);
                lineMatrixIndex = (lineMatrixIndex + 1) % (6 * thickness);
	        }
            /* 下半弧 */
            d=(int)(bb*(p.X+0.5)*(p.X+0.5)+aa*(p.Y-1)*(p.Y-1)-aa*bb+0.5);
            while(p.Y>0)
	        { p.Y--;   dx=dx-twoaa;  
            if(d>0)  d=d+aa-dx;
            else  { p.X++; dy=dy+twobb; d=d+aa+dy-dx; }                                    
            pc = PointTurn(new Point(cp.X + p.X, cp.Y + p.Y), the, cd);
            PixelBrush(pc.X, pc.Y, thickness, cd);
            pc = PointTurn(new Point(cp.X + p.X, cp.Y - p.Y), the, cd);
            PixelBrush(pc.X, pc.Y, thickness, cd);
            pc = PointTurn(new Point(cp.X - p.X, cp.Y + p.Y), the, cd);
            PixelBrush(pc.X, pc.Y, thickness, cd);
            pc = PointTurn(new Point(cp.X - p.X, cp.Y - p.Y), the, cd);
            PixelBrush(pc.X, pc.Y, thickness, cd);
            lineMatrixIndex = (lineMatrixIndex + 1) % (6 * thickness);
	    }
            pc = PointTurn(new Point(cp.X - p.X, cp.Y - p.Y), the, cd);
            PixelBrush(pc.X, pc.Y, thickness, cd);
            pc = PointTurn(new Point(cp.X + p.X, cp.Y + p.Y), the, cd);
            PixelBrush(pc.X, pc.Y, thickness, cd);
        }

        public override FigureBase GetNewOne(CommonData cd)//重写克隆函数
        {
            BaFigureEllipse resultF = new BaFigureEllipse();
            resultF.pC1 = new BaPoint(pC1.location, cd);
            resultF.pC2 = new BaPoint(pC2.location, cd);
            resultF.a = this.a;
            resultF.b = this.b;
            resultF.c = this.c;
            resultF.the = this.the;
            resultF.location = new PointF(location.X, location.Y);
            resultF.width = this.width;
            resultF.height = this.height;
            resultF.relativeLocation = new PointF(this.relativeLocation.X, this.relativeLocation.Y);
            resultF.relativeWidth = this.relativeWidth;
            resultF.relativeHeight = this.height;
            resultF.mid = new PointF(mid.X, mid.Y);
            resultF.iD = CreateId(cd);
            resultF.PixelsFlush(cd);
            resultF.baPoints = new List<BaPoint>();
            resultF.baPoints.Add(resultF.pC1);
            resultF.baPoints.Add(resultF.pC2);
            resultF.isSelect = false;
            resultF.FigureKind = BaseFigure.LINE;
            resultF.thickness = this.thickness;
            resultF.color = this.color;
            resultF.lineStyle = this.lineStyle;
            return resultF;
        }

        public override FigureBase CopyNewOne(CommonData cd)//用于复制粘贴，位置会偏移
        {
            float d = cd.PblToCvl(10);
            BaFigureEllipse resultF = new BaFigureEllipse();
            resultF.pC1 = new BaPoint(new PointF(pC1.location.X + d, pC1.location.Y + d), cd);
            resultF.pC2 = new BaPoint(new PointF(pC2.location.X + d, pC2.location.Y + d), cd);
            resultF.a = this.a;
            resultF.b = this.b;
            resultF.c = this.c;
            resultF.the = this.the;
            resultF.location = new PointF(location.X + d, location.Y + d);
            resultF.width = this.width;
            resultF.height = this.height;
            resultF.relativeLocation = new PointF(this.relativeLocation.X, this.relativeLocation.Y);
            resultF.relativeWidth = this.relativeWidth;
            resultF.relativeHeight = this.relativeHeight;
            resultF.mid = new PointF(mid.X + d, mid.Y + d);
            resultF.iD = CreateId(cd);
            resultF.PixelsFlush(cd);
            resultF.baPoints = new List<BaPoint>();
            resultF.baPoints.Add(resultF.pC1);
            resultF.baPoints.Add(resultF.pC2);
            resultF.isSelect = false;
            resultF.FigureKind = BaseFigure.LINE;
            resultF.thickness = this.thickness;
            resultF.color = this.color;
            resultF.lineStyle = this.lineStyle;
            return resultF;
        }
    }
}
