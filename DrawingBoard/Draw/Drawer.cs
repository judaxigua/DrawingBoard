using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace DrawingBoard
{
    class Drawer//负责将图元等显示在pictureBox上的类
    {
        CommonData commonData;

        public void DataInit(CommonData cd)//数据初始化
        {
            commonData = cd;            
        }

        public void DrawCache()//绘制缓冲区
        {
            // 初始化画板
            commonData.canvas = new Bitmap(commonData.pictureBox.ClientSize.Width, commonData.pictureBox.ClientSize.Height);
            commonData.gb = Graphics.FromImage(commonData.canvas);

            DrawCorrecrtLines();//画校准线
            DrawFigures(true);//绘制图元们
            DrawFigurePixels();//画像素点图元

            commonData.pictureBox.BackgroundImage = commonData.canvas; // 设置为背景层
        }

        public void Draw()
        {
            DrawPictureBox();//重绘pictureBox
            DrawToolStatus();//重绘状态栏                       
        }

        public void DrawVirtual()//在虚拟画板上画图
        {
            // 获取背景层
            commonData.canvas = new Bitmap(commonData.pictureBox.ClientSize.Width, commonData.pictureBox.ClientSize.Height);            

            // 绘图部分 Begin            

            DrawFigures();//绘制图元们            
            DrawFigurePixels();            
            // 绘图部分 End
        }

        public void DrawPictureBox()//重绘pictureBox
        {
            // 初始化画板            
            commonData.image = new Bitmap(commonData.canvas);
            commonData.g = Graphics.FromImage(commonData.image);

            DrawFigures(false);//绘制图元们
            DrawRectTrackers();//画橡皮筋         

            // 绘图部分 End          
            commonData.pictureBox.BackgroundImage = commonData.image; // 设置为背景层
            commonData.pictureBox.Refresh();            
        }

        private void DrawFigures(Boolean isBackGround)//绘制图元们
        {
            //绘制动态作图的图元
            if (isBackGround == true)//如果是画在背景上面
            {
                if (commonData.isDemo == true)
                {
                    if (commonData.FigureDemo.isBeenEdit == false)
                        commonData.FigureDemo.Draw(commonData, commonData.canvas);
                }
                //绘制图元们
                for (int i = 0; i < commonData.figureManager.figures.Count; i++)
                {
                    if (commonData.figureManager.figures[i].isBeenEdit == false)
                        commonData.figureManager.figures[i].Draw(commonData, commonData.canvas);
                }
            }
            else
            {
                if (commonData.isDemo == true)
                {
                    if (commonData.FigureDemo.isBeenEdit == true)
                        commonData.FigureDemo.Draw(commonData, commonData.image);
                }
                //绘制图元们
                for (int i = 0; i < commonData.figureManager.figures.Count; i++)
                {
                    if (commonData.figureManager.figures[i].isBeenEdit == true)
                        commonData.figureManager.figures[i].Draw(commonData, commonData.image);
                }
            }
            
        }

        private void DrawFigures()//绘制图元们
        {            
            if (commonData.isDemo == true)
            {
                commonData.FigureDemo.Draw(commonData, commonData.canvas);
            }
            //绘制图元们
            for (int i = 0; i < commonData.figureManager.figures.Count; i++)
            {
                commonData.figureManager.figures[i].Draw(commonData, commonData.canvas);
            }
        }

        private void DrawRectTrackers()//画橡皮筋
        {
            if (commonData.isDrawRectTracker == true)
            {
                for (int i = 0; i < commonData.rectTrackers.Count(); i++)
                {
                    commonData.rectTrackers[i].Draw();
                }
            }   
        }

        public void DrawFigurePixels()//重绘像素点图元
        {
            commonData.figureManager.figurePixels.Draw(commonData);
        }

        public void ListViewFlush()//重绘列表
        {
            commonData.listView_Figure.BeginUpdate();            

            commonData.listView_Figure.Items.Clear();  //只移除所有的项。
            List<FigureBase> listFB = commonData.figureManager.figures;
            for (int i = 0; i < listFB.Count(); i++)
            {
                ListViewItem lvi = new ListViewItem();

                lvi.ImageIndex = i;     //通过与imageList绑定，显示imageList中第i项图标 ，目前未使用

                lvi.Text = listFB[i].iD;//插入id
                lvi.SubItems.Add(listFB[i].KindToString());//插入类别
                lvi.SubItems.Add(listFB[i].LocationToString());//插入坐标
                lvi.Tag = i;//tag里面存放图元下标
                commonData.listView_Figure.Items.Add(lvi);
            }

            commonData.listView_Figure.EndUpdate();            
        }

        public void DrawToolStatus()//重绘状态栏
        {
            //鼠标在画板上的坐标
            commonData.tolStpStsLabelPicX.Text = commonData.currentLocation.X+"";
            commonData.tolStpStsLabelPicY.Text = commonData.currentLocation.Y + "";
            PointF p = commonData.PblToCvl(commonData.currentLocation);
            //鼠标在画布上的做坐标
            commonData.tolStpStsLabelCavX.Text = p.X + "";
            commonData.tolStpStsLabelCavY.Text = p.Y + "";            
        }                        

        private void DrawCorrecrtLines()//绘制校准线
        {
            if (commonData.isCorrectLine == false)
                return;
            //竖着的校准线
            for (int i = -(int)(commonData.paperWidth / 2); i < (int)(commonData.paperWidth / 2); i += 10)
            {
                PointF spX = new PointF(i, -commonData.paperHeight / 2);
                PointF epX = new PointF(i, commonData.paperHeight / 2);
                DrawCorrectLine(spX, epX);
            }
            //横着的校准线
            for (int j = -(int)(commonData.paperHeight) / 2; j < (int)(commonData.paperHeight) / 2; j += 10)
            {
                PointF spY = new PointF(-commonData.paperWidth / 2, j);
                PointF epY = new PointF(commonData.paperWidth / 2, j);
                DrawCorrectLine(spY, epY);
            }            
        }

        private void DrawCorrectLine(PointF sp,PointF ep)//绘制单条校准线
        {
            Point spP = commonData.CvlToPbl(sp);
            Point epP = commonData.CvlToPbl(ep);
            Pen pen = new Pen(Color.FromArgb(128, 200, 200, 200));//半透明的灰色            
            commonData.gb.DrawLine(pen, spP, epP);
        }
    }
}
