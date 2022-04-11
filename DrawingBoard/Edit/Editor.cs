using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;


namespace DrawingBoard
{
    class Editor//负责编辑的类
    {
        CommonData commondata;

        public delegate void EventHandler();//自定义委托类
        public event EventHandler Draw;//绑定Drawer的作图函数
        public event EventHandler ListViewFlush;//绑定Drawer的刷新列表函数
        public event EventHandler DrawCache;//绑定Drawer的缓冲函数

        public void DataInit(CommonData cd)//初始化数据
        {
            commondata = cd;
            //初始化钩子
            commondata.keyboardHook = new KeyboardHook();
            commondata.keyboardHook.SetHook();//键盘钩子
            commondata.keyboardHook.OnKeyDownEvent += kh_OnKeyDownEvent;
            commondata.keyboardHook.OnKeyUpEvent += kh_OnKeyUpEvent;
        }

        public void DownEdit(Point p)//鼠标按下编辑
        {
            switch (commondata.canvasEdit)
            {
                case CanvasEdit.DRAG:
                    {
                        DownDragEdit(p);//拖动
                        break;
                    }
                case CanvasEdit.SELECT:
                    {
                        DownSelectEdit(p);//单选编辑
                        break;
                    }
                case CanvasEdit.SELECTPOINT:
                    {
                        DownSelectPointEdit(p);
                        break;
                    }
            }
        }

        public void DownDragEdit(Point p)//按下进行拖拽
        {
            commondata.isDown = true;
        }

        public void DownSelectEdit(Point p)//按下单选编辑
        {
            if (commondata.control == false)//单选情况下
            {
                if (commondata.isSelected == false)//还没选定要编辑的图元的情况
                {
                    FigureBase fb = commondata.figureManager.Select(p);
                    if (fb == null)//如果没选中则退出
                    {
                        commondata.isDrawRectTracker = false;
                        return;
                    }
                    RectTracker rT = new RectTracker();
                    fb.SetIsBeenEdit(true);
                    rT.DataInit(commondata, fb);
                    commondata.rectTrackers.Add(rT);
                    commondata.isSelected = true;
                    commondata.isDrawRectTracker = true;
                }
                else//考虑已经选定的情况
                {
                    Boolean b = RectTrackersClick(p);
                    if (b == false)//如果点在橡皮筋组外面
                    {
                        commondata.isDrawRectTracker = false;
                        commondata.isSelected = false;
                        SetRTEdit(false);
                        commondata.rectTrackers = new List<RectTracker>();//清空橡皮筋组                        
                        commondata.figureManager.FiguresSelectFlush();//刷新图元选中状态                        
                    }
                }
            }
            else//考虑多选情况
            {
                FigureBase fb = commondata.figureManager.Select(p);
                if (fb == null)//如果没选中则退出
                {
                    return;
                }
                fb.SetIsBeenEdit(true);
                RectTracker rT = new RectTracker();
                rT.DataInit(commondata, fb);
                commondata.rectTrackers.Add(rT);
                commondata.isSelected = true;
                commondata.isDrawRectTracker = true;
            }
            DrawCache();
        }

        private void DownSelectPointEdit(Point p)//按下选点编辑
        {
            if (commondata.isSelected == false)//还没选定要编辑的图元的情况
            {
                FigureBase fb = commondata.figureManager.SelectPoint(p);
                if (fb == null)//如果没选中则退出
                {
                    commondata.isDrawRectTracker = false;
                    return;
                }
                ((BaPoint)fb).SetIsBeenEdit(true);
                commondata.rectTrackers = new List<RectTracker>();//重置
                commondata.rectTrackers.Add(new RectTracker());
                commondata.rectTrackers[0].DataInit(commondata, fb);
                commondata.isSelected = true;
                commondata.isDrawRectTracker = true;
            }
            else//考虑已经选定的情况
            {
                Boolean b = commondata.rectTrackers[0].MouseDown(p);
                if (b == false)//如果点在编辑器外面
                {
                    SetRTEdit(false);
                    commondata.isDrawRectTracker = false;
                    commondata.isSelected = false;                    
                }
            }
            DrawCache();
        }

        public void MoveEdit(Point p)//鼠标移动编辑
        {
            switch (commondata.canvasEdit)
            {
                case CanvasEdit.DRAG:
                    {
                        MoveDragEdit(p);//拖动
                        break;
                    }
                case CanvasEdit.SELECT:
                    {
                        MoveSelectEdit(p);//单选编辑
                        break;
                    }
                case CanvasEdit.SELECTPOINT:
                    {
                        MoveSeletcPointEdit(p);//点编辑
                        break;
                    }
            }
            commondata.figureManager.FiguresPixelsFlush();
        }

        public void MoveDragEdit(Point p)//移动进行拖拽
        {
            if (commondata.isDown == true)
            {
                int dx = p.X - commondata.currentLocation.X;
                int dy = p.Y - commondata.currentLocation.Y;
                float dxC = commondata.PblToCvl(dx);
                float dyC = commondata.PblToCvl(dy);
                //改变观察点中心
                commondata.watchLocation.X -= dxC;
                commondata.watchLocation.Y += dyC;
            }
            commondata.currentLocation = new Point(p.X, p.Y);//当前鼠标位置
            DrawCache();
        }

        public void MoveSelectEdit(Point p)//单选编辑拖动中
        {
            if (commondata.isSelected == false)//如果并没有选择图元
                return;
            else//考虑选定图元的情况
            {
                for (int i = 0; i < commondata.rectTrackers.Count(); i++)
                {
                    if (commondata.rectTrackers[i].MouseMove(p) == true)
                        return;//如果有一个橡皮筋有操作响应，那么屏蔽别的橡皮筋
                }
            }
        }

        private void MoveSeletcPointEdit(Point p)//单选点拖动中
        {
            if (commondata.isSelected == false)//如果并没有选择图元
                return;
            else//考虑选定图元的情况
            {
                commondata.rectTrackers[0].MouseMove(p);
            }
        }

        public void UpEdit(Point p)//鼠标抬起编辑
        {
            switch (commondata.canvasEdit)
            {
                case CanvasEdit.DRAG:
                    {
                        UpDragEdit(p);//拖动
                        break;
                    }
                case CanvasEdit.SELECT:
                    {
                        UpSelectEdit(p);//单选编辑
                        break;
                    }
                case CanvasEdit.SELECTPOINT:
                    {
                        UpSelectPointEdit(p);//点
                        break;
                    }
            }
        }

        public void UpDragEdit(Point p)//抬起进行编辑
        {
            commondata.isDown = false;
        }

        public void UpSelectEdit(Point p)//单选编辑鼠标抬起
        {
            if (commondata.isSelected == true)
            {
                for (int i = 0; i < commondata.rectTrackers.Count(); i++)
                {
                    commondata.rectTrackers[i].MouseUp(p);
                }
            }
        }

        private void UpSelectPointEdit(Point p)//单选点鼠标抬起
        {
            if (commondata.isSelected == true)
            {
                commondata.rectTrackers[0].MouseUp(p);
            }
        }

        public void WheelEdit(int Delta)//滚轮编辑
        {
            if (Delta > 0)
            {
                commondata.scale += 0.1f;//放大
            }
            else
            {
                if (commondata.scale <= 0.1f)
                    return;
                commondata.scale -= 0.1f;//缩小
            }
            commondata.figureManager.FiguresPixelsFlush();//刷新像素点
            DrawCache();
            Draw();
        }

        void kh_OnKeyDownEvent(object sender, KeyEventArgs e)//键盘按下
        {
            if (e.Control == true) //control设为真
            {
                commondata.control = true;
            }
            if (e.KeyData == (Keys.C | Keys.Control)) //Ctrl+C复制到粘贴板
            {
                Copy();
            }
            if (e.KeyData == (Keys.V | Keys.Control)) //Ctrl+V复制到粘贴板
            {
                Paste();
            }
            if (e.KeyData == (Keys.X | Keys.Control)) //Ctrl+X剪切
            {
                Cut();
            }
        }

        void kh_OnKeyUpEvent(object sender, KeyEventArgs e)//键盘抬起
        {
            if (e.Control == false) //control设为假
            {
                commondata.control = false;
            }
        }

        public void Split()//拆分
        {
            if (commondata.rectTrackers.Count() != 1)
                return;
            if (commondata.rectTrackers[0].editFigure.FigureKind != BaseFigure.COMPOSETE)
                return;
            FigureComposite fc = (FigureComposite)commondata.rectTrackers[0].editFigure;
            for (int i = 0; i < fc.figures.Count(); i++)
            {
                commondata.figureManager.AddFigure(fc.figures[i]);
                ListViewFlush();
                commondata.figureManager.IndexFlush();//刷新下标
            }
            Cut();
            DrawCache();
            Draw();
        }

        public void Combine()//组合
        {
            Copy();
            commondata.figureManager.FiguresSelectFlush();
            FigureComposite fc = new FigureComposite(commondata.clipboard,commondata);
            Cut();
            commondata.figureManager.AddFigure(fc);            
            commondata.figureManager.PointsMagnetFlush();
            ListViewFlush();
            commondata.figureManager.IndexFlush();//刷新下标      
            DrawCache();
            Draw();
        }

        public void Cut()//剪切
        {
            Copy();
            for (int i = 0; i < commondata.clipboard.Count(); i++)
            {
                commondata.figureManager.IndexFlush();//先刷新一下下标
                commondata.figureManager.figures.RemoveAt(commondata.clipboard[i].index);
            }
            commondata.figureManager.IndexFlush();
            commondata.isDrawRectTracker = false;
            commondata.rectTrackers = new List<RectTracker>();
            DrawCache();
            Draw();
        }

        public void Copy()//图元复制到粘贴板
        {
            commondata.clipboard = new List<FigureBase>();//重置粘贴板
            for (int i = 0; i < commondata.rectTrackers.Count(); i++)
            {
                commondata.clipboard.Add(commondata.rectTrackers[i].editFigure);
            }            
        }

        public void Paste()//粘贴
        {
            List<FigureBase> listTemp = new List<FigureBase>();
            for (int i = 0; i < commondata.clipboard.Count(); i++)
            {
                listTemp.Add(commondata.clipboard[i].CopyNewOne(commondata));//挨个克隆
            }
            for (int i = 0; i < listTemp.Count(); i++)
            {
                commondata.figureManager.AddFigure(listTemp[i]);
            }
            commondata.clipboard = listTemp;
            ListViewFlush();
            commondata.figureManager.IndexFlush();//刷新下标
            DrawCache();
            Draw();
        }

        public void ListSelect()//列表选择的操作
        {            
            commondata.canvasEdit = CanvasEdit.SELECT;
            if (commondata.listView_Figure.SelectedItems.Count > 0)
            {
                int index = (int)commondata.listView_Figure.SelectedItems[0].Tag;
                commondata.rectTrackers = new List<RectTracker>();
                commondata.rectTrackers.Add(new RectTracker());                
                commondata.rectTrackers[0].DataInit(commondata, commondata.figureManager.figures[index]);
                commondata.isSelected = true;
                commondata.isDrawRectTracker = true;
            }
        }

        public Boolean RectTrackersClick(Point p)//橡皮筋编辑选择
        {
            for (int i = 0; i < commondata.rectTrackers.Count(); i++)
            {
                Boolean b = commondata.rectTrackers[i].MouseDown(p);
                if (b == true)//只要有一个按到了就返回
                    return true;
            }
            return false;
        }

        public void SetRTEdit(Boolean b)
        {
            for (int i = 0; i < commondata.rectTrackers.Count(); i++)
            {
                commondata.rectTrackers[i].editFigure.SetIsBeenEdit(b);
            }
        }
    }
}
