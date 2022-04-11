using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DrawingBoard
{
    class FigureManager//图元管理器
    {
        public CommonData commonData;
        public List<FigureBase> figures = new List<FigureBase>();//图元的集合         
        public FigurePixels figurePixels = new FigurePixels();//像素图元        

        public void DataInit(CommonData cd)//数据初始化
        {
            commonData = cd;
        }

        public void AddFigure(FigureBase fb)//添加图元
        {
            figures.Add(fb);
        }

        public int Count()
        {
            return figures.Count();
        }

        public FigureBase GetFigure(int index)//获得下标为index的图元
        {
            if (index < 0 || index >= figures.Count)
                return null;
            else return figures[index];
        }

        public void IndexFlush()//刷新图元下标
        {
            for (int i = 0; i < figures.Count(); i++)
            {
                figures[i].index = i;//下标赋值
            }
        }

        public FigureBase Select(Point p)//通过点选选择图元
        {
            for (int i = 0; i < figures.Count; i++)
            {
                if (figures[i].Select(p, commonData) == true)
                    return figures[i];
            }
            return null;
        }

        public void FiguresSelectFlush()//刷新所有图元的选中状态为假值
        {
            for (int i = 0; i < figures.Count(); i++)
            {
                figures[i].isSelect = false;
            }
        }

        public FigureBase SelectPoint(Point p)//通过点选择图元点
        {
            for (int i = 0; i < figures.Count(); i++)
            {
                for (int j = 0; j < figures[i].baPoints.Count(); j++)
                {
                    if (figures[i].baPoints[j].Select(p, commonData) == true)
                        return figures[i].baPoints[j];
                }
            }
            return null;
        }

        public void FiguresPixelsFlush()//刷新所有图元的像素点
        {
            for (int i = 0; i < figures.Count; i++)
            {
                figures[i].PixelsFlush(commonData);
            }
            figurePixels.PixelFlush(commonData);
        }

        public void PointsMagnetFlush()//刷新所有点的吸附状态
        {
            for (int i = 0; i < figures.Count(); i++)
            {
                figures[i].PointsMagnetFlush();
            }
        }
    }
}
