using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace DrawingBoard
{
    class RectTracker//橡皮筋类，负责图像的动态编辑
    {
        CommonData commondata;
        public FigureBase editFigure;//编辑的图元

        private Point location;//橡皮筋在pictureBox上的坐标(左上角)
        private int width, height;//橡皮筋的宽高

        private bool isDown = false;//鼠标是否被按下
        private OperDire operDire = OperDire.MID;//编辑状态
        Point lastMose = new Point(0, 0);///上一次鼠标移动时的位置
        Point lastLocat = new Point(0, 0);///上一次移动时的窗口坐标               
        
        public void DataInit(CommonData cd,FigureBase bf)
        {
            commondata = cd;//初始化公共数据
            editFigure = bf;//加载要编辑的图元
            editFigure.isSelect = true;//图元被选中

            if (editFigure.FigureKind == BaseFigure.POINT)
            {
                ((BaPoint)editFigure).isMagnet = false;
                commondata.magnet.magnetPoint = null;
                commondata.magnet.isMagneting = false;
            }
            DataFlush();//刷新数据
        }

        private void DataFlush()//刷新数据
        {
            //初始化橡皮筋的Location
            this.location = commondata.CvlToPbl(editFigure.location);
            //初始化宽高
            width = commondata.CvlToPbl(editFigure.width);
            height = commondata.CvlToPbl(editFigure.height);

            location.Y = location.Y - height;
        }

        public Boolean MouseMove(Point p)//鼠标移动
        {
            DataFlush();//根据图元刷新橡皮筋的属性

            Point mousLot = new Point();//鼠标相对于橡皮筋的位置
            mousLot.X = p.X - location.X;
            mousLot.Y = p.Y - location.Y;
            //如果鼠标在橡皮筋的外面则结束

            if (this.editFigure.FigureKind == BaseFigure.POINT)//如果编辑的是点另外考虑
            {
                if (isDown == false)
                {
                    if (commondata.GetDistance(p, commondata.CvlToPbl(editFigure.location)) <= 6)
                    {
                        this.operDire = OperDire.MID;
                        commondata.pictureBox.Cursor = Cursors.SizeAll;//中间   
                        lastLocat = this.location;
                        return true;  
                    }
                    else
                    {
                        commondata.pictureBox.Cursor = Cursors.Arrow;//鼠标在外面移动
                        return false;
                    }
                }
                else
                {
                    if (operDire == OperDire.MID)
                    {
                        this.location = new Point(mousLot.X - lastMose.X + lastLocat.X, mousLot.Y - lastMose.Y + lastLocat.Y);
                        FigureResize();
                        lastLocat = this.location;
                        return true;
                    }
                }         
            }

            //下面是点以外的一般图元的情况
            if (isDown == false)//考虑鼠标还未按下的情况
            {                
                if (mousLot.X < 0 || mousLot.Y < 0 || mousLot.X > width || mousLot.Y > height )
                {
                    commondata.pictureBox.Cursor = Cursors.Arrow;//鼠标在外面移动
                    lastLocat = this.location;  
                    return false;
                }                

                if (mousLot.X>(width/2 - width/20) && mousLot.X<(width/2+width/20) && mousLot.Y<height/10)
                {
                    this.operDire = OperDire.TURN;
                    commondata.pictureBox.Cursor = Cursors.NoMove2D;//旋转
                    lastLocat = this.location;  
                    return true;
                }

                if (mousLot.X > 5 && mousLot.X < width - 5 && mousLot.Y < 5 && mousLot.X<(width/2 - width/20) && mousLot.X>(width/2+width/20))
                {
                    this.operDire = OperDire.UP;                    
                    commondata.pictureBox.Cursor = Cursors.SizeNS;//上
                    lastLocat = this.location;
                    return true;
                }
                if (mousLot.X > 5 && mousLot.X < width - 5 && mousLot.Y > height - 5)
                {
                    this.operDire = OperDire.DOWN;
                    commondata.pictureBox.Cursor = Cursors.SizeNS;///下
                    ///lastLocat = this.location;  
                    return true;
                }
                if (mousLot.Y > 5 && mousLot.Y < height - 5 && mousLot.X < 5)
                {
                    this.operDire = OperDire.LEFT;
                    commondata.pictureBox.Cursor = Cursors.SizeWE;///左
                    lastLocat = this.location;
                    return true;
                }
                if (mousLot.Y > 5 && mousLot.Y < height - 5 && mousLot.X > width - 5)
                {
                    this.operDire = OperDire.RIGHT;
                    commondata.pictureBox.Cursor = Cursors.SizeWE;//右
                    lastLocat = this.location;
                    return true;
                }
                if (mousLot.X <= 5 && mousLot.Y <= 5)
                {
                    this.operDire = OperDire.LEFT_UP;
                    commondata.pictureBox.Cursor = Cursors.SizeNWSE;//左上
                    lastLocat = this.location;
                    return true;
                }
                if (mousLot.X >= width - 5 && mousLot.Y <= 5)
                {
                    this.operDire = OperDire.RIGHT_UP;
                    commondata.pictureBox.Cursor = Cursors.SizeNESW;//右上
                    lastLocat = this.location;
                    return true;
                }
                if (mousLot.X <= 5 && mousLot.Y >= height - 5)
                {
                    this.operDire = OperDire.LEFT_DOWN;
                    commondata.pictureBox.Cursor = Cursors.SizeNESW;//左下
                    lastLocat = this.location;
                    return true;
                }
                if (mousLot.X >= width - 5 && mousLot.Y >= height - 5)
                {
                    this.operDire = OperDire.RIGHT_DOWN;
                    commondata.pictureBox.Cursor = Cursors.SizeNWSE;//右下
                    lastLocat = this.location;
                    return true;
                }
                if (mousLot.X > 5 && mousLot.X < width - 5 && mousLot.Y > 5 && mousLot.Y < height - 5)
                {
                    this.operDire = OperDire.MID;
                    commondata.pictureBox.Cursor = Cursors.SizeAll;//中间
                    lastLocat = this.location;
                    return true;
                }
            }
            else///如果按住则进行拖拽操作
            {
                
                if (operDire == OperDire.LEFT)
                {
                    this.location = new Point(mousLot.X + lastLocat.X, this.location.Y);
                    this.width -= this.location.X - lastLocat.X;                    
                    lastMose = mousLot;
                    FigureResize();
                    lastLocat = this.location;
                    return true;
                }
                if (operDire == OperDire.UP)
                {
                    this.location = new Point(this.location.X, mousLot.Y + lastLocat.Y);
                    this.height -= this.location.Y - lastLocat.Y;
                    lastMose = mousLot;
                    FigureResize();
                    lastLocat = this.location;
                    return true;
                }
                if (operDire == OperDire.RIGHT)
                {
                    this.width += mousLot.X - lastMose.X;
                    lastMose = mousLot;
                    FigureResize();
                    lastLocat = this.location;
                    return true;
                }
                if (operDire == OperDire.DOWN)
                {
                    this.height += mousLot.Y - lastMose.Y;
                    lastMose = mousLot;
                    FigureResize();
                    lastLocat = this.location;
                    return true;
                }
                if (operDire == OperDire.LEFT_UP)
                {
                    this.location = new Point(mousLot.X + lastLocat.X, mousLot.Y + lastLocat.Y);
                    this.width -= this.location.X - lastLocat.X;
                    this.height -= this.location.Y - lastLocat.Y;
                    lastMose = mousLot;
                    FigureResize();
                    lastLocat = this.location;
                    return true;
                }
                if (operDire == OperDire.RIGHT_UP)
                {
                    this.width += mousLot.X - lastMose.X;
                    this.location = new Point(this.location.X, mousLot.Y + lastLocat.Y);
                    this.height -= this.location.Y - lastLocat.Y;
                    lastMose = mousLot;
                    FigureResize();
                    lastLocat = this.location;
                    return true;
                }
                if (operDire == OperDire.LEFT_DOWN)
                {
                    this.location = new Point(mousLot.X + lastLocat.X, this.location.Y);
                    this.width -= this.location.X - lastLocat.X;
                    this.height += mousLot.Y - lastMose.Y;
                    lastMose = mousLot;
                    FigureResize();
                    lastLocat = this.location;
                    return true;
                }
                if (operDire == OperDire.RIGHT_DOWN)
                {
                    this.width += mousLot.X - lastMose.X;
                    this.height += mousLot.Y - lastMose.Y;
                    lastMose = mousLot;
                    FigureResize();
                    lastLocat = this.location;
                    return true;
                }
                if (operDire == OperDire.MID)
                {
                    this.location = new Point(mousLot.X - lastMose.X + lastLocat.X, mousLot.Y - lastMose.Y + lastLocat.Y);
                    FigureResize();
                    lastLocat = this.location;
                    return true;
                }

                if (operDire == OperDire.TURN)
                {
                    Point midP = new Point(width/2,height/2);
                    float a = commondata.GetDistance(mousLot,midP);
                    float b = commondata.GetDistance(lastMose, midP);
                    float c = commondata.GetDistance(mousLot, lastMose);
                    float cosB = (a * a + b * b - c * c) / (2 * a * b);//余弦定理
                    float the = (float)Math.Acos((double)cosB);//计算旋转的角度
                    commondata.tolStpStsLabelCavX.Text = the + "";
                    //讨论旋转角的正负
                    if (lastMose.Y < height / 2)
                    {
                        if (lastMose.X < mousLot.X)
                            editFigure.Turn(the,commondata);
                        else
                            editFigure.Turn(-the,commondata);
                    }
                    else
                    {
                        if (lastMose.X < mousLot.X)
                            editFigure.Turn(-the,commondata);
                        else
                            editFigure.Turn(the,commondata);
                    }
                    editFigure.ResizeToPoints(commondata);
                    for (int i = 0; i < editFigure.baPoints.Count(); i++)
                    {
                        editFigure.baPoints[i].hasResized = false;
                    }
                    lastMose = mousLot;
                    this.DataFlush();
                    lastLocat = this.location;
                    return true;
                }                             
            }
            return false;//这个貌似不需要加
        }

        public Boolean MouseDown(Point p)//按下
        {
            DataFlush();

            Point mousLot = new Point();//鼠标相对于橡皮筋的位置
            mousLot.X = p.X - this.location.X;
            mousLot.Y = p.Y - this.location.Y;

            if (editFigure.FigureKind == BaseFigure.POINT)
            {
                if (commondata.GetDistance(p, commondata.CvlToPbl(editFigure.location)) > 6)
                    return false;
            }
            else
            {
                if (mousLot.X < 0 || mousLot.Y < 0 || mousLot.X > width || mousLot.Y > height)
                    return false;
            }

            this.isDown = true;
            this.lastMose = mousLot;
            this.lastLocat = this.location;

            return true;
        }

        public void MouseUp(Point p)//抬起
        {
            DataFlush();

            if (editFigure.FigureKind == BaseFigure.POINT)//如果点，可能还要吸附
            {
                if (commondata.magnet.isMagneting == true)
                {
                    for (int i = 0; i < ((BaPoint)editFigure).fathers.Count(); i++)
                    {
                        commondata.magnet.magnetPoint.fathers.Add(((BaPoint)editFigure).fathers[i]);
                    }
                    for (int i = 0; i < ((BaPoint)editFigure).fathers.Count(); i++)
                    {
                        ((BaPoint)editFigure).fathers[i].ReplacePoint(((BaPoint)editFigure), commondata.magnet.magnetPoint);
                    }
                    commondata.magnet.magnetPoint.FathersResize(commondata);
                    commondata.figureManager.PointsMagnetFlush();
                }
            }

            this.isDown = false;
        }

        public void FigureResize()//调整规模
        {
            if (width <= 1)
                width = 2;
            if (height <= 1)
                height = 2;

            this.editFigure.Resize(this.location, this.width, this.height, commondata);
            editFigure.ResizeToPoints(commondata);
            for (int i = 0; i < editFigure.baPoints.Count(); i++)
            {
                editFigure.baPoints[i].hasResized = false;
            }
            this.DataFlush();
        }
        
        public void Draw()//画自己
        {
            DataFlush();

            if (editFigure.FigureKind == BaseFigure.POINT)//如果编辑的是点
            {
                Point mid = commondata.CvlToPbl(editFigure.location);
                SolidBrush brushs = new SolidBrush(Color.Red);
                commondata.g.FillEllipse(brushs, mid.X - 4, mid.Y - 4, 8, 8);                
                return;
            }
            
            Pen pen = new Pen(Color.Gray);
            pen.DashStyle = DashStyle.DashDotDot;///设置虚线
            Point[] point = new Point[4];
            point[0] = new Point(0 + location.X, 0 + location.Y);
            point[1] = new Point(width - 1+location.X, 0+location.Y);
            point[2] = new Point(width - 1+location.X, height - 1+location.Y);
            point[3] = new Point(0+location.X, height - 1+location.Y);
            commondata.g.DrawLine(pen, point[0], point[1]);
            commondata.g.DrawLine(pen, point[1], point[2]);
            commondata.g.DrawLine(pen, point[2], point[3]);
            commondata.g.DrawLine(pen, point[0], point[3]);

            //画小框
            int w = width / 20; int h = height / 20;
            SolidBrush brush = new SolidBrush(Color.Gray);
            SolidBrush brushB = new SolidBrush(Color.Black);
            commondata.g.FillEllipse(brushB, width / 2 - w / 2 + location.X, 0 - h / 2 + location.Y, w, h);///上
            commondata.g.FillRectangle(brush, width / 2 - w / 2 + location.X, height - h / 2 - 1 + location.Y, w, h);///下
            commondata.g.FillRectangle(brush, 0 - w / 2 + location.X, height / 2 - h / 2 + location.Y, w, h);///左
            commondata.g.FillRectangle(brush, width - w / 2 - 1 + location.X, height / 2 - h / 2 + location.Y, w, h);///右
            commondata.g.FillRectangle(brush, 0 - w / 2 + location.X, 0 - h / 2 + location.Y, w, h);///左上
            commondata.g.FillRectangle(brush, width - w / 2 - 1 + location.X, 0 - h / 2 + location.Y, w, h);///右上
            commondata.g.FillRectangle(brush, 0 - w / 2 + location.X, height - h / 2 - 1 + location.Y, w, h);///左下
            commondata.g.FillRectangle(brush, width - w / 2 - 1 + location.X, height - h / 2 - 1 + location.Y, w, h);///右下                          
        }
    }

    public enum OperDire///拖拽操作的方向
    {
        LEFT,
        RIGHT,
        UP,
        DOWN,
        LEFT_UP,
        RIGHT_UP,
        LEFT_DOWN,
        RIGHT_DOWN,
        TURN,//旋转
        MID,
    };
}