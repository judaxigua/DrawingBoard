using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Configuration;
using System.Runtime.InteropServices;

namespace DrawingBoard
{
    class Magnet//吸铁石类，支持用吸附方式进行图元编辑
    {
        public CommonData commonData;//公共数据
        public BaPoint magnetPoint = null;//吸附对象
        public Boolean isMagneting = false;//是否正在吸附        
        public Point magentLocation;//被吸附到的点

        [DllImport("user32.dll")]//设定鼠标位置
        private static extern int SetCursorPos(int x, int y);
        [DllImport("user32.dll")]//获取鼠标位置
        public static extern bool GetCursorPos(out Point pt);

        public void DataInit(CommonData cd)//数据的初始化
        {
            commonData = cd;
        }

        public void MouseMove(Point p)//鼠标移动时进行鼠标的吸附
        {
            switch (commonData.editState)
            {
                case EditState.BASEFIGURE://作图时的吸附
                {
                    BaseFigureMove(p);
                    break;
                }
                case EditState.CANVASEDIT://编辑时的吸附
                {
                    CanvasEditMove(p);
                    break;
                }
            }
        }

        private void BaseFigureMove(Point p)//画基本图元
        {
            if (isMagneting == false)//如果当前不在吸附
            {
                MagnetJudge(p);
            }
            else//如果正在吸附
            {
                Point ps = new Point();
                GetCursorPos(out ps);
                int d = (int)Math.Sqrt((ps.X - magentLocation.X) * (ps.X - magentLocation.X) + (ps.Y - magentLocation.Y) * (ps.X - magentLocation.X));//拉开的距离
                if (d>=commonData.magnetDistance)//挣脱吸附
                {
                    commonData.pictureBox.Cursor = Cursors.Default;
                    if(magnetPoint.magentTime>=3)
                        magnetPoint.isMagnet = false;//多次吸过的点不能再吸                    
                    magnetPoint = null;
                    isMagneting = false;                    
                }
            }
        }

        public void CanvasEditMove(Point p)//编辑状态
        {
            if (isMagneting == false)//如果当前不在吸附
            {
                MagnetJudge(p);
            }
            else//如果正在吸附
            {
                Point ps = new Point();
                GetCursorPos(out ps);
                int d = (int)Math.Sqrt((ps.X - magentLocation.X) * (ps.X - magentLocation.X) + (ps.Y - magentLocation.Y) * (ps.X - magentLocation.X));//拉开的距离
                if (d >= commonData.magnetDistance)//挣脱吸附
                {
                    commonData.pictureBox.Cursor = Cursors.Default;
                    if (magnetPoint.magentTime >= 3)
                        magnetPoint.isMagnet = false;//多次吸过的点不能再吸                    
                    magnetPoint = null;
                    isMagneting = false;
                }
            }
        }

        private Boolean MagnetJudge(Point p)//对吸附的判定
        {
            for (int i = 0; i < commonData.figureManager.Count(); i++)
            {
                for (int j = 0; j < commonData.figureManager.figures[i].BaPointNum(); j++)
                {
                    if (commonData.figureManager.figures[i].baPoints[j].IsBeMagnet(p, commonData))//如果发生吸附
                    {
                        this.magnetPoint = commonData.figureManager.figures[i].baPoints[j];//设置吸附的点       
                        this.magnetPoint.isVisible = true;
                        this.isMagneting = true;//正在吸附
                        this.commonData.pictureBox.Cursor = Cursors.Cross;//改变鼠标样式
                        Point pl = commonData.CvlToPbl(magnetPoint.location);
                        Point ps = new Point(); 
                        GetCursorPos(out ps);//获得当前鼠标位置
                        int dx = pl.X - p.X;
                        int dy = pl.Y - p.Y;
                        SetCursorPos(ps.X+dx,ps.Y+dy);//设置鼠标被吸到的位置
                        magentLocation = new Point(ps.X + dx, ps.Y + dy);//记录吸附位置                        
                        magnetPoint.magentTime++;
                        return true;
                    }
                }
            }
            this.magnetPoint = null;
            this.isMagneting = false;
            return false;
        }
    }
}
