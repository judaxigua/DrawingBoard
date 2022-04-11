using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;


namespace DrawingBoard
{
    class CommonData//管理一些公共数据
    {
        //状态属性
        public EditState editState = EditState.BASEFIGURE;//鼠标选择状态
        public BaseFigure baseFigure = BaseFigure.LINE;//基本图形
        public CanvasEdit canvasEdit = CanvasEdit.SELECT;//画布编辑
        public OtherDraw otherDraw = OtherDraw.COLORGET;//其他操作
        //对象属性
        public FigureManager figureManager = new FigureManager();//图元管理器
        public FigureBase FigureDemo;//动态作图时的临时图元
        public Magnet magnet;//吸铁石类
        public List<FigureBase> clipboard = new List<FigureBase>();//粘贴板        
        public List<RectTracker> rectTrackers = new List<RectTracker>();//橡皮筋组
        //控件
        public ToolStripStatusLabel tolStpStsLabelPicX;//状态栏画板坐标X
        public ToolStripStatusLabel tolStpStsLabelPicY;//状态栏画板坐标Y
        public ToolStripStatusLabel tolStpStsLabelCavX;//状态栏画布坐标X
        public ToolStripStatusLabel tolStpStsLabelCavY;//状态栏画布坐标Y
        public ToolStripButton colorDemoButton;//显示颜色的按钮
        public ListView listView_Figure = null;//图元列表控件
        //作图属性
        public int thickness = 1;//线条粗细
        public Color color = Color.Black;//当前鼠标操作的颜色
        public LineStyle lineStyle = LineStyle.LSA;//线条样式
        public int[][][] lineStyleMatrix;
        //画板相关
        public float paperWidth;//画布的实际宽（像素为单位）
        public float paperHeight;//画布的实际高
        //**这里画布的坐标右为x正向，上为y正向，中心为坐标原点**//
        public float scale;//视宽和画布的比例(视窗/画布)
        public float scaleMax = 7;//允许的最大比例
        public PointF watchLocation = new PointF();//视窗中心在画布上的坐标 
        public int picBoxWidth;//pictureBox的宽
        public int picBoxHeight;//pictureBOx的高   
        
        public PictureBox pictureBox;
        public Bitmap image;//画板
        public Bitmap bg;//背景层
        public Bitmap canvas;//画布
        public Graphics g;//图形面板
        public Graphics gb;//图形面板
        public Pen pen = new Pen(Color.Black);//画笔                        
        //操作状态
        public int idNum = 0;
        public int clickTime = 0;//画图时的点击次数
        public PointF lstCkLocat = new PointF(0,0);//上一次点击鼠标的位置（相对于画布的实际位置）
        public BaPoint lstPoint = null;//上一次吸附的图元点
        public Boolean isLstMagnet = false;
        public Point currentLocation;//当前鼠标的位置（相对于画板）
        public Boolean isDemo = false;//是否需要绘制临时图元
        public Boolean isDown = false;//鼠标是否被按下
        public Boolean isCorrectLine = true;//是否显示校准线
        public Boolean isSelected = false;//是否已经选定要编辑的图元
        public Boolean isDrawRectTracker = false;//是否该绘制橡皮筋
        public Boolean isMagnet = true;//是否调用吸铁石
        public int magnetDistance;//造成吸附的最大距离
        public Point formLocation;//主窗体的坐标
        public KeyboardHook keyboardHook;//键盘钩子
        public Boolean control = false;//Ctrl键是否被按下                

        public void LineMatrixInit()//给线形矩阵赋值
        {
            lineStyleMatrix = new int[6][][];
            lineStyleMatrix[1] = new int[4][];
            lineStyleMatrix[1][0] = new int[6] { 1, 1, 1, 1, 1, 1 };
            lineStyleMatrix[1][1] = new int[6] { 1, 1, 1, 1, 0, 0 };
            lineStyleMatrix[1][2] = new int[6] { 1, 0, 1, 0, 1, 0 };
            lineStyleMatrix[1][3] = new int[6] { 1, 1, 1, 0, 1, 0 };
            lineStyleMatrix[3] = new int[4][];
            lineStyleMatrix[3][0] = new int[18] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            lineStyleMatrix[3][1] = new int[18] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0 };
            lineStyleMatrix[3][2] = new int[18] { 1, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0 };
            lineStyleMatrix[3][3] = new int[18] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, };
            lineStyleMatrix[5] = new int[4][];
            lineStyleMatrix[5][0] = new int[30] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            lineStyleMatrix[5][1] = new int[30] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            lineStyleMatrix[5][2] = new int[30] { 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0 };
            lineStyleMatrix[5][3] = new int[30] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, };
        }

        public PointF PblToCvl(Point p)//视窗坐标转画布坐标
        {            
            float x;
            float y;
            x = watchLocation.X - (picBoxWidth / 2 - p.X) / scale;
            y = watchLocation.Y + (picBoxHeight / 2 - p.Y) / scale;
            PointF resultp = new PointF(x, y);
            return resultp;
        }

        public float PblToCvl(int length)//重载，视窗长度转画布长度
        {
            float resultL;
            resultL = length /scale;
            return resultL;
        }

        public Point CvlToPbl(PointF p)//画布坐标转视窗坐标
        {
            int x, y;
            x = (int)(picBoxWidth / 2 - (watchLocation.X - p.X) * scale);
            y = (int)(picBoxHeight / 2 - (p.Y - watchLocation.Y) * scale);
            Point resultp = new Point(x, y);
            return resultp;
        }

        public int CvlToPbl(float length)//重载，画布长度转视窗长度
        {
            int resultI;
            resultI = (int)(length * scale);
            return resultI;
        }

        public float GetDistance(PointF sp,PointF ep)//获得两点之间距离
        {
            float resultF;
            resultF = (float)Math.Sqrt(Math.Pow(sp.X-ep.X,2)+Math.Pow(sp.Y-ep.Y,2));
            return resultF;
        }

        public float GetDistance(Point sp, Point ep)//获得两点之间距离
        {
            float resultF;
            resultF = (float)Math.Sqrt(Math.Pow(sp.X - ep.X, 2) + Math.Pow(sp.Y - ep.Y, 2));
            return resultF;
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

        public Point GetMin(List<Point> points)//重载，获得一串点的最小值
        { 
            if(points.Count<=2)
                return new Point(0,0);
            Point resultP = new Point();
            resultP.X = points[0].X;
            resultP.Y = points[0].Y;
            for (int i = 1; i < points.Count; i++)
            {
                if (resultP.X > points[i].X)
                    resultP.X = points[i].X;
                if (resultP.Y > points[i].Y)
                    resultP.Y = points[i].Y;
            }
            return resultP;
        }

        public PointF GetMin(List<PointF> points)//重载，获得一串点的最小值
        {
            if (points.Count <= 2)
                return new Point(0, 0);
            PointF resultP = new Point();
            resultP.X = points[0].X;
            resultP.Y = points[0].Y;
            for (int i = 1; i < points.Count; i++)
            {
                if (resultP.X > points[i].X)
                    resultP.X = points[i].X;
                if (resultP.Y > points[i].Y)
                    resultP.Y = points[i].Y;
            }
            return resultP;
        }

        public Point GetMax(List<Point> points)//重载，获得一串点的最大值
        {
            if (points.Count <= 2)
                return new Point(0, 0);
            Point resultP = new Point();
            resultP.X = points[0].X;
            resultP.Y = points[0].Y;
            for (int i = 1; i < points.Count; i++)
            {
                if (resultP.X < points[i].X)
                    resultP.X = points[i].X;
                if (resultP.Y < points[i].Y)
                    resultP.Y = points[i].Y;
            }
            return resultP;
        }

        public PointF GetMax(List<PointF> points)//重载，获得一串点的最大值
        {
            if (points.Count <= 2)
                return new Point(0, 0);
            PointF resultP = new Point();
            resultP.X = points[0].X;
            resultP.Y = points[0].Y;
            for (int i = 1; i < points.Count; i++)
            {
                if (resultP.X < points[i].X)
                    resultP.X = points[i].X;
                if (resultP.Y < points[i].Y)
                    resultP.Y = points[i].Y;
            }
            return resultP;
        }

    }

    //各种枚举
    public enum EditState//鼠标操作状态
    {
        BASEFIGURE,//画基本图形
        CANVASEDIT,//画布编辑
        OTHERDRAW,//其他画图手段
    };

    public enum BaseFigure//基本图形
    {
        COMPOSETE,//组合图元
        FIGUREPOINT,//图元点
        LINE,//直线
        POINT,//基本点
        RECT,//矩形
        ELLIPSE,//椭圆
        CIRCLE,//圆
        PEN,//铅笔
        BRUSH,//刷子
        CURVE,//曲线
        POLYGON,//多边形
        CHAR,//文字
        RUBBER,//橡皮擦
        TRIANGLE,//三角形
        PENTAGON,//五边形
        SEXANGLE,//六边形
        DIAMOND,//菱形
        ARROWLEFT,//左箭头
        ARROWUP,//上箭头
        ARROWRIGHT,//右箭头
        ARROWDOWN,//下箭头
        BUBBLERECT,//矩形气泡
        BUBBLEELLIPSE,//椭圆气泡
        STAR4,//四角星
        STAR5,//五角星
        STAR6,//六芒星
        STARLOVE,//爱星
        COLORFILL,//颜色填充
        COLORGET,//取色
    }

    public enum CanvasEdit//画布编辑
    {
        SELECT,//选定
        SELECTMORE,//复选
        SELECTPOINT,//选取点
        DRAG,//拖拽
    }

    public enum OtherDraw//其他状态
    {
        COLORGET,//取色
        FILL,//填充
    }

    public enum LineStyle//线形？
    {
        LSA,//实线
        LSB,//————————
        LSC,//..........
        LSD,//_._._._._.
    }
}
