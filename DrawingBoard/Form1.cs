using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DrawingBoard
{
    public partial class MainForm : Form
    {
        private CommonData commonData = new CommonData();//公共数据
        private FigureCreater figureCreater = new FigureCreater();//图元生成器
        private Drawer drawer = new Drawer();//绘图器
        private Editor editor = new Editor();//编辑器
                
        public MainForm()//构造函数
        {
            InitializeComponent();               
        }

        private void MainForm_Load(object sender, EventArgs e)//窗体load
        {
            DataInit();//初始化公共数据
            figureCreater.DataInit(commonData);//初始化图形生成器   
            drawer.DataInit(commonData);//初始化绘图器
            editor.DataInit(commonData);//初始化编辑器
            this.picBoxMain.MouseWheel += new MouseEventHandler(picBoxMain_MouseWheel); //注册滚轮事件                       
            ListViewInit();//初始化列表控件
            drawer.DrawCache();
        }

        private void MainForm_LocationChanged(object sender, EventArgs e)//窗体位置发生改变时
        {
            commonData.formLocation = new Point(this.Location.X, this.Location.Y);
        }

        private void DataInit()//commondata初始化
        {
            commonData.editState = EditState.BASEFIGURE;
            commonData.baseFigure = BaseFigure.LINE;
            commonData.canvasEdit = CanvasEdit.SELECT;

            commonData.thickness = 1;//线条粗细
            commonData.color = Color.Black;//当前鼠标操作的颜色
            commonData.lineStyle = LineStyle.LSA;//线条样式
            commonData.LineMatrixInit();//初始化线形矩阵

            commonData.paperWidth = 2000;//画布的实际宽（像素为单位）
            commonData.paperHeight = 2000;//画布的实际高
            commonData.scale = 1;//视宽和画布的比例
            
            
            commonData.picBoxWidth = picBoxMain.Width;//pictureBox的宽
            commonData.picBoxHeight = picBoxMain.Height;//pictureBOx的高  
 
            commonData.watchLocation.X = 0;
            commonData.watchLocation.Y = 0;

            commonData.clickTime = 0;//画图时的点击次数
            commonData.currentLocation = new Point(0, 0);//初始化当前鼠标位置
            commonData.magnetDistance = 7;
            commonData.formLocation = new Point(this.Location.X,this.Location.Y);//初始化此窗体的坐标

            commonData.figureManager = new FigureManager();//初始化图元管理器
            commonData.figureManager.commonData = commonData;
            commonData.magnet = new Magnet();//初始化吸铁石
            commonData.magnet.commonData = commonData;

            commonData.pictureBox = picBoxMain;//初始化pictureBox
            commonData.pen = new Pen(commonData.color);//初始化画笔
            commonData.listView_Figure = listView_Figures;//初始化列表控件

            commonData.image = new Bitmap(commonData.pictureBox.ClientSize.Width, commonData.pictureBox.ClientSize.Height);// 初始化画板
            commonData.bg = (Bitmap)commonData.pictureBox.BackgroundImage;// 获取背景层
            commonData.canvas = new Bitmap(commonData.pictureBox.ClientSize.Width, commonData.pictureBox.ClientSize.Height);// 初始化画布
            commonData.g = Graphics.FromImage(commonData.image);// 初始化图形面板
            commonData.gb = Graphics.FromImage(commonData.canvas);

            commonData.tolStpStsLabelPicX = tolStpStusLabel_pictureBox_X;//初始化状态栏画板横坐标
            commonData.tolStpStsLabelPicY = tolStpStusLabel_pictureBox_Y;//纵坐标
            commonData.tolStpStsLabelCavX = tolStpStusLabel_Canvus_X;//初始化状态栏画布横坐标
            commonData.tolStpStsLabelCavY = tolStpStusLabel_Canvus_Y;//总坐标
            commonData.colorDemoButton = ButtonColorDemo;

            editor.Draw += drawer.Draw;
            editor.ListViewFlush += drawer.ListViewFlush;
            editor.DrawCache += drawer.DrawCache;
            figureCreater.Draw += drawer.Draw;
            figureCreater.DrawVirtul += drawer.DrawVirtual;
            figureCreater.ListViewFlush += drawer.ListViewFlush;
            figureCreater.DrawCache += drawer.DrawCache;
        }

        private void picBoxMain_MouseDown(object sender, MouseEventArgs e)//鼠标按下
        {
            HandlePBMsDown(sender,e);
        }

        private void picBoxMain_MouseMove(object sender, MouseEventArgs e)//鼠标移动
        {
            HandlePBMsMove(sender, e);            
        }

        private void picBoxMain_MouseUp(object sender, MouseEventArgs e)//鼠标抬起
        {
            HandlePBMsUp(sender,e);
        }

        private void HandlePBMsDown(object sender, MouseEventArgs e)//处理pictureBox鼠标按下事件
        {
            //画板按下
            switch (commonData.editState)
            {
                case EditState.BASEFIGURE:
                    {
                        FigureDownDraw(e.Location);
                        break;
                    }
                case EditState.CANVASEDIT:
                    {
                        EditDownDraw(e.Location);
                        break;
                    }
                case EditState.OTHERDRAW:
                    {
                        OtherDown(e.Location);
                        break;
                    }
            }
            Draw();//重绘
        }

        private void HandlePBMsMove(object sender, MouseEventArgs e)//处理pictureBox鼠标移动事件
        {
            //画板移动            
            switch (commonData.editState)
            {
                case EditState.BASEFIGURE:
                    {
                        FigureMoveDraw(e.Location);
                        break;
                    }
                case EditState.CANVASEDIT:
                    {
                        EditMoveDraw(e.Location);
                        break;
                    }
            }                       
            Draw();//重绘
        }

        private void HandlePBMsUp(object sender, MouseEventArgs e)//处理pictureBox鼠标抬起事件
        {
            switch (commonData.editState)
            {
                case EditState.BASEFIGURE:
                    {
                        FigureUpDraw(e.Location);
                        break;
                    }
                case EditState.CANVASEDIT:
                    {
                        EditUpDraw(e.Location);
                        break;
                    }
            }
            Draw();//重绘
        }        
        
        private void FigureDownDraw(Point p)//鼠标按下时作图
        {
            figureCreater.DownCreateFigure(p);//生成图元            
        }

        private void FigureMoveDraw(Point p)//鼠标移动时作图
        {
            commonData.magnet.MouseMove(p); 
            figureCreater.MoveCreateFigure(p);
        }

        private void FigureUpDraw(Point p)//鼠标抬起时作图
        {
            figureCreater.HandleUpCreateFigure(p);
        }        

        private void EditDownDraw(Point p)//鼠标按下时编辑
        {            
            editor.DownEdit(p);
        }        

        private void EditMoveDraw(Point p)//鼠标按下时编辑
        {
            commonData.magnet.MouseMove(p);            
            editor.MoveEdit(p);
        }

        private void EditUpDraw(Point p)//鼠标按下时编辑
        {
            editor.UpEdit(p);
        }

        private void OtherDown(Point p)//鼠标按下其他绘图
        {
            figureCreater.DownOtherCreate(p);
        }

        private void Draw()//显示图像
        {
            drawer.Draw();            
        }

        private void FigureFlagFlush()//选定图形编辑将commondata的属性刷新
        {
            commonData.editState = EditState.BASEFIGURE;
            commonData.clickTime = 0;
            commonData.isDemo = false;
            commonData.isSelected = false;
            commonData.isDown = false;
            commonData.isDrawRectTracker = false;
            commonData.rectTrackers = new List<RectTracker>();
            commonData.pictureBox.Cursor = Cursors.Arrow;//鼠标样式
        }

        private void EditFlagFlush()//选择各类编辑时将commondata各个属性刷新
        {
            commonData.editState = EditState.CANVASEDIT;
            commonData.isDemo = false;
            commonData.isDown = false;
            commonData.isDrawRectTracker = false;
            commonData.isSelected = false;
            commonData.pictureBox.Cursor = Cursors.Arrow;//鼠标样式
        }

        private void picBoxMain_Resize(object sender, EventArgs e)//画板resize
        {
            //画板resize
        }             

        private void ListViewInit()//相互石化列表属性
        {            
            this.listView_Figures.BeginUpdate();   //数据更新，UI暂时挂起，直到EndUpdate绘制控件，可以有效避免闪烁并大大提高加载速度 

            this.listView_Figures.View = View.Details;
            listView_Figures.Columns.Add("图元ID", 80, HorizontalAlignment.Left);//添加标题项
            listView_Figures.Columns.Add("类别", 80, HorizontalAlignment.Left);//添加类别项                        
            listView_Figures.Columns.Add("坐标", 80, HorizontalAlignment.Left);//添加类别项                        

            this.listView_Figures.EndUpdate();  //结束数据处理，UI界面一次性绘制。 
        }

        private void picBoxMain_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)//pictureBox滚轮事件
        {
            editor.WheelEdit(e.Delta);            
        }

        private void picBoxMain_MouseEnter(object sender, EventArgs e)//进入时设置pictureBox的焦点
        {
            picBoxMain.Focus();
        }

        private void listView_Figures_SelectedIndexChanged(object sender, EventArgs e)//列表被选中的事件
        {
            EditFlagFlush();
            editor.ListSelect();//列表选中处理
            Draw();
        }

        private void tolStpBtnLine_Click(object sender, EventArgs e)//按钮选择画直线
        {
            FigureFlagFlush();
            commonData.baseFigure = BaseFigure.LINE;
        }

        private void tolStpBtnRect_Click(object sender, EventArgs e)//按钮选择画矩形
        {
            FigureFlagFlush();
            commonData.baseFigure = BaseFigure.RECT;
        }

        private void tolStpBtnEllipse_Click(object sender, EventArgs e)//按钮选择画椭圆
        {
            FigureFlagFlush();
            commonData.baseFigure = BaseFigure.ELLIPSE;
        }

        private void tolStpBtnCirlce_Click(object sender, EventArgs e)//按钮选择画圆
        {
            FigureFlagFlush();
            commonData.baseFigure = BaseFigure.CIRCLE;
        }

        private void tolStpBtnPoint_Click(object sender, EventArgs e)//按钮选择画点
        {
            FigureFlagFlush();
            commonData.baseFigure = BaseFigure.FIGUREPOINT;
        }

        private void tolStpBtnDrag_Click(object sender, EventArgs e)//按钮选择拖拽动作
        {
            EditFlagFlush();
            commonData.canvasEdit = CanvasEdit.DRAG;            
        }

        private void tolStpBtnSel_Click(object sender, EventArgs e)//按钮选择单选编辑
        {
            EditFlagFlush();
            commonData.canvasEdit = CanvasEdit.SELECT;
        }                

        private void tolStpBtnSelPoint_Click(object sender, EventArgs e)//按钮选择点选编辑
        {
            EditFlagFlush();
            commonData.canvasEdit = CanvasEdit.SELECTPOINT;
        }

        private void tolStpBtnColGet_Click(object sender, EventArgs e)//取色
        {
            commonData.editState = EditState.OTHERDRAW;
            commonData.otherDraw = OtherDraw.COLORGET;
        }

        private void tolStpBtnColFill_Click(object sender, EventArgs e)//填充
        {
            commonData.editState = EditState.OTHERDRAW;
            commonData.otherDraw = OtherDraw.FILL;
        }

        private void tolStpBtnCopy_Click(object sender, EventArgs e)//复制
        {
            editor.Copy();
        }

        private void tolStpBtnCut_Click(object sender, EventArgs e)//剪切
        {
            editor.Cut();
        }

        private void tolStpBtnPaste_Click(object sender, EventArgs e)//粘贴
        {
            editor.Paste();
        }

        private void tolStpBtnCom_Click(object sender, EventArgs e)//组合
        {
            editor.Combine();
        }

        private void tolStpBtnDes_Click(object sender, EventArgs e)//拆分
        {
            editor.Split();
        }

        //颜色
        private void tolStpBtnBlack_Click(object sender, EventArgs e)
        {   
            commonData.color = Color.Black;
            ButtonColorDemo.BackColor = Color.Black;
        }
        private void tolStpBtnRed_Click(object sender, EventArgs e)
        {   
            commonData.color = Color.Red;
            ButtonColorDemo.BackColor = Color.Red;
        }
        private void tolStpBtnRedL_Click(object sender, EventArgs e)
        {   
            commonData.color = Color.Pink;
            ButtonColorDemo.BackColor = Color.Pink;
        }
        private void tolStpBtnOriange_Click(object sender, EventArgs e)
        {   
            commonData.color = Color.Orange;
            ButtonColorDemo.BackColor = Color.Orange;
        }
        private void tolStpBtnYellow_Click(object sender, EventArgs e)
        {   
            commonData.color = Color.Yellow;
            ButtonColorDemo.BackColor = Color.Yellow;
        }
        private void tolStpBtnGreen_Click(object sender, EventArgs e)
        {   
            commonData.color = Color.LightGreen;
            ButtonColorDemo.BackColor = Color.Green;
        }
        private void tolStpBtnGreenL_Click(object sender, EventArgs e)
        {   
            commonData.color = Color.LightGreen;
            ButtonColorDemo.BackColor = Color.LightGreen;
        }
        private void tolStpBtnCyan_Click(object sender, EventArgs e)
        {   
            commonData.color = Color.ForestGreen;
            ButtonColorDemo.BackColor = Color.ForestGreen;
        }
        private void tolStpBtnBlue_Click(object sender, EventArgs e)
        {   
            commonData.color = Color.Blue;
            ButtonColorDemo.BackColor = Color.Blue;
        }
        private void tolStpBtnBlueL_Click(object sender, EventArgs e)
        {   
            commonData.color = Color.LightSkyBlue;
            ButtonColorDemo.BackColor = Color.LightSkyBlue;
        }
        private void tolStpBtnPurple_Click(object sender, EventArgs e)
        {   
            commonData.color = Color.Purple;
            ButtonColorDemo.BackColor = Color.Purple;
        }
        private void tolStpBtnPurpleL_Click(object sender, EventArgs e)
        {   
            commonData.color = Color.LightCoral;
            ButtonColorDemo.BackColor = Color.LightCoral;
        }
        private void tolStpBtnGray_Click(object sender, EventArgs e)
        {   
            commonData.color = Color.DimGray;
            ButtonColorDemo.BackColor = Color.DimGray;
        }
        private void tolStpBtnGrayL_Click(object sender, EventArgs e)
        {   
            commonData.color = Color.Gray;
            ButtonColorDemo.BackColor = Color.Gray;
        }
        //粗细
        private void toolStripMenuItem_thickness1_Click(object sender, EventArgs e)
        {
            commonData.thickness = 1;
        }
        private void toolStripMenuItem_thickness3_Click(object sender, EventArgs e)
        {
            commonData.thickness = 3;
        }
        private void toolStripMenuItem_thickness5_Click(object sender, EventArgs e)
        {
            commonData.thickness = 5;
        }        

        //线形
        private void toolStripMenuItem_LineStyle1_Click(object sender, EventArgs e)
        {
            commonData.lineStyle = LineStyle.LSA;
        }

        private void toolStripMenuItem_LineStyle2_Click(object sender, EventArgs e)
        {
            commonData.lineStyle = LineStyle.LSB;
        }

        private void toolStripMenuItem_LineStyle3_Click(object sender, EventArgs e)
        {
            commonData.lineStyle = LineStyle.LSC;
        }

        private void toolStripMenuItem_LineStyle4_Click(object sender, EventArgs e)
        {
            commonData.lineStyle = LineStyle.LSD;
        }

        private void tolStpBtnTriangle_Click(object sender, EventArgs e)
        {
            FigureFlagFlush();
            commonData.baseFigure = BaseFigure.TRIANGLE;
        }

        private void tolStpBtnPentagon_Click(object sender, EventArgs e)
        {
            FigureFlagFlush();
            commonData.baseFigure = BaseFigure.PENTAGON;
        }

        private void tolStpBtnSexangle_Click(object sender, EventArgs e)
        {
            FigureFlagFlush();
            commonData.baseFigure = BaseFigure.SEXANGLE;
        }

        private void tolStpBtnDiamond_Click(object sender, EventArgs e)
        {
            FigureFlagFlush();
            commonData.baseFigure = BaseFigure.DIAMOND;
        }

        private void tolStpBtnArrowLeft_Click(object sender, EventArgs e)
        {
            FigureFlagFlush();
            commonData.baseFigure = BaseFigure.ARROWLEFT;
        }

        private void tolStpBtnArrowUp_Click(object sender, EventArgs e)
        {
            FigureFlagFlush();
            commonData.baseFigure = BaseFigure.ARROWUP;
        }

        private void tolStpBtnArrowDown_Click(object sender, EventArgs e)
        {
            FigureFlagFlush();
            commonData.baseFigure = BaseFigure.ARROWDOWN;
        }

        private void tolStpBtnArrowRight_Click(object sender, EventArgs e)
        {
            FigureFlagFlush();
            commonData.baseFigure = BaseFigure.ARROWRIGHT;
        }

        private void tolStpBtnStar4_Click(object sender, EventArgs e)
        {
            FigureFlagFlush();
            commonData.baseFigure = BaseFigure.STAR4;
        }

        private void tolStpBtnStar5_Click(object sender, EventArgs e)
        {
            FigureFlagFlush();
            commonData.baseFigure = BaseFigure.STAR5;
        }

        private void tolStpBtnStar6_Click(object sender, EventArgs e)
        {
            FigureFlagFlush();
            commonData.baseFigure = BaseFigure.STAR6;
        }                
    }
}
