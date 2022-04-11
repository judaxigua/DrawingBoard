using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace DrawingBoard
{
    class FigureCreater//用于生成图元
    {
        private CommonData commondata;
        private List<BaPoint> lstPoints = new List<BaPoint>();//在图元还未生成前点击过的点图元  
        private List<Point> fillPixels = new List<Point>();//填充时产生的像素点集合

        public delegate void EventHandler();//自定义委托
        public event EventHandler Draw;//绑定Drawer的作图函数
        public event EventHandler DrawVirtul;//绑定Drawer的虚拟作图函数
        public event EventHandler ListViewFlush;//绑定Drawer的刷新列表函数
        public event EventHandler DrawCache;//绑定Drawer的缓冲函数

        public void DataInit(CommonData cd)//初始化数据
        {
            commondata = cd;
        }

        public void DownCreateFigure(Point p)//鼠标按下创建图元
        {
            //注意传入的p是鼠标在pictureBox上的位置
            switch (commondata.baseFigure)
            {
                case BaseFigure.LINE:
                    {
                        HandleCreateLine(p);//画的直线
                        break;
                    }
                case BaseFigure.RECT:
                    {
                        HandleCreateRect(p);//矩形
                        break;
                    }
                case BaseFigure.ELLIPSE://椭圆
                    {
                        HandleCreateEllipse(p);
                        break;
                    }
                case BaseFigure.CIRCLE://圆
                    {
                        HandleCreateCircle(p);
                        break;
                    }
                case BaseFigure.FIGUREPOINT://点
                    {
                        HandleCreatePoint(p);
                        break;
                    }
                default:
                    {
                        HandleCreateOptionalFigure(p);//自选图形
                        break;
                    }
            }
        }

        public void MoveCreateFigure(Point p)//鼠标移动中创建图元
        {
            //注意传入的p是鼠标在pictureBox上的位置
            commondata.currentLocation = new Point(p.X, p.Y);
            switch (commondata.baseFigure)
            {
                case BaseFigure.LINE:
                    {
                        HandleMoveCreateLine(p);//直线
                        break;
                    }
                case BaseFigure.RECT:
                    {
                        HandleMoveCreateRect(p);//矩形
                        break;
                    }
                case BaseFigure.ELLIPSE:
                    {
                        HandleMoveCreateEllipse(p);//椭圆
                        break;
                    }
                case BaseFigure.CIRCLE:
                    {
                        HandleMoveCreateCircle(p);//圆
                        break;
                    }
                default:
                    {
                        HandleMoveCreateOptionalFigure(p);//自选图形
                        break;
                    }
            }
        }

        public void DownOtherCreate(Point p)
        {
            switch (commondata.otherDraw)
            {
                case OtherDraw.COLORGET:
                    {
                        ColorSelectDown(p);
                        break;
                    }
                case OtherDraw.FILL:
                    {
                        FillDown(p);
                        break;
                    }
                default:
                    return;
            }
        }

        public void HandleUpCreateFigure(Point p)
        {
            commondata.isDown = false;//鼠标标记为抬起
        }                         
        
        private void HandleCreateLine(Point p)//作直线
        {
            if (lstPoints.Count() == 0)//考虑编辑起始点的情况
            {
                lstPoints.Add(GetMagentPoint(p));//初始化起点            
                commondata.FigureDemo = CreateLine(p,true);
                commondata.isDemo = true;                
                return;
            }
            else//考虑编辑末端的情况
            {
                commondata.isDemo = false;//不用动态显示
                FigureBase fb = CreateLine(p,false);
                EndOfCreate(fb);                
;           }
        }

        private void HandleCreateRect(Point p)//作矩形
        {
            if (lstPoints.Count() == 0)//考虑编辑起始点的情况
            {
                lstPoints.Add(GetMagentPoint(p));//初始化起点
                commondata.FigureDemo = CreateRect(p,true);
                commondata.isDemo = true;                
                return;
            }
            else//考虑编辑末端的情况
            {                
                commondata.isDemo = false;//不用动态显示
                FigureBase fb = CreateRect(p,false);
                EndOfCreate(fb);       
            }
        }

        public void HandleCreateEllipse(Point p)//作椭圆
        {
            if (lstPoints.Count() == 0)//考虑编辑起始点的情况
            {
                lstPoints.Add(GetMagentPoint(p));
                commondata.FigureDemo = CreateEllipse(p,true);
                commondata.isDemo = true;                
                return;
            }
            else//考虑编辑末端的情况
            {                
                commondata.isDemo = false;//不用动态显示
                FigureBase fb = CreateEllipse(p,false);
                EndOfCreate(fb);         
            }
        }

        public void HandleCreateCircle(Point p)//作圆
        {
            if (lstPoints.Count() == 0)//考虑编辑起始点的情况
            {
                lstPoints.Add(GetMagentPoint(p));
                commondata.FigureDemo = CreateCircle(p,true);
                commondata.isDemo = true;                
                return;
            }
            else//考虑编辑末端的情况
            {                
                commondata.isDemo = false;//不用动态显示
                FigureBase fb = CreateCircle(p,false);
                EndOfCreate(fb);
            }
        }

        public void HandleCreatePoint(Point p)//作点
        {
            lstPoints.Add(GetMagentPoint(p));
            FigureBase fb = CreatePoint(p,false);
            EndOfCreate(fb);
        }

        public void HandleCreateOptionalFigure(Point p)//作自选图形
        {
            if (lstPoints.Count() == 0)//考虑编辑起始点的情况
            {
                lstPoints.Add(GetMagentPoint(p));//初始化起点            
                commondata.FigureDemo = CreateOptionalFigure(p, true);
                commondata.isDemo = true;
                return;
            }
            else//考虑编辑末端的情况
            {
                commondata.isDemo = false;//不用动态显示
                FigureBase fb = CreateOptionalFigure(p, false);
                EndOfCreate(fb);
            }
        }

        private void HandleMoveCreateLine(Point p)//动态作直线
        {
            if (commondata.isDemo == true)
                commondata.FigureDemo = CreateLine(p,true);
        }

        private void HandleMoveCreateRect(Point p)//动态作矩形
        {
            if (commondata.isDemo == true)
                commondata.FigureDemo = CreateRect(p, true);
        }

        private void HandleMoveCreateEllipse(Point p)//动态作椭圆
        {
            if (commondata.isDemo == true)
                commondata.FigureDemo = CreateEllipse(p, true);
        }

        private void HandleMoveCreateCircle(Point p)//动态作圆
        {
            if (commondata.isDemo == true)
                commondata.FigureDemo = CreateCircle(p, true);
        }

        public void HandleMoveCreateOptionalFigure(Point p)//动态作自选图形
        {
            if (commondata.isDemo == true)
                commondata.FigureDemo = CreateOptionalFigure(p,true);
        }

        private FigureBase CreateLine(Point p, Boolean isDemo)//生成直线的函数
        {
            BaFigureLine resultF = new BaFigureLine(commondata);

            resultF.sp = lstPoints[0];//起点
            resultF.ep = GetMagentPoint(p);
            if (isDemo == false)//如果不是作的示范
            {
                resultF.sp.fathers.Add(resultF);
                resultF.ep.fathers.Add(resultF);
                resultF.sp.isBeenEdit = false;
                resultF.ep.isBeenEdit = false;
                resultF.isBeenEdit = false;
            }
            else
            {
                resultF.sp.isBeenEdit = true;
                resultF.ep.isBeenEdit = true;
                resultF.isBeenEdit = true;
            }
            resultF.baPoints.Add(resultF.sp);//添加点集合
            resultF.baPoints.Add(resultF.ep);

            PointF p1 = new PointF(); PointF p2 = new PointF();
            p1 = resultF.sp.location; p2 = resultF.ep.location;

            //计算图元块属性
            resultF.iD = CreateId();
            PointF lc = new PointF();
            lc.X = p1.X < p2.X ? p1.X : p2.X;
            lc.Y = p1.Y < p2.Y ? p1.Y : p2.Y;
            resultF.location = lc;//块的左上角坐标
            resultF.width = Math.Abs(p1.X - p2.X);
            resultF.height = Math.Abs(p1.Y - p2.Y);
            resultF.mid = new PointF(resultF.location.X + resultF.width / 2, resultF.location.Y + resultF.height / 2);
            resultF.PixelsFlush(commondata);

            return resultF;
        }

        private FigureBase CreateRect(Point p, Boolean isDemo)//生成矩形的函数
        {
            BaPoint p1 = lstPoints[0];
            BaPoint p2 = GetMagentPoint(p);
            BaFigureRect resultF = new BaFigureRect(commondata);

            PointF pMin = commondata.GetPMin(p1.location, p2.location);
            PointF pMax = commondata.GetPMax(p1.location, p2.location);

            //初始化顶点
            resultF.pLU = new BaPoint(new PointF(pMin.X, pMax.Y), commondata);//左上角
            resultF.pRU = new BaPoint(new PointF(pMax.X, pMax.Y), commondata);//右上角
            resultF.pRD = new BaPoint(new PointF(pMax.X, pMin.Y), commondata);//右下角
            resultF.pLD = new BaPoint(new PointF(pMin.X, pMin.Y), commondata);//左下角
            //添加原来的图元点
            resultF.pLU = PointCombine(resultF.pLU, p1); resultF.pLU = PointCombine(resultF.pLU, p2);
            resultF.pRU = PointCombine(resultF.pRU, p1); resultF.pRU = PointCombine(resultF.pRU, p2);
            resultF.pLD = PointCombine(resultF.pLD, p1); resultF.pLD = PointCombine(resultF.pLD, p2);
            resultF.pRD = PointCombine(resultF.pRD, p1); resultF.pRD = PointCombine(resultF.pRD, p2);
            //给点添加father            
            if (isDemo == false)//如果不是作的示范
            {
                resultF.pLU.fathers.Add(resultF);
                resultF.pLD.fathers.Add(resultF);
                resultF.pRU.fathers.Add(resultF);
                resultF.pRD.fathers.Add(resultF);
                resultF.isBeenEdit = false;
                resultF.pLU.isBeenEdit = false;
                resultF.pLD.isBeenEdit = false;
                resultF.pRU.isBeenEdit = false;
                resultF.pRD.isBeenEdit = false;
            }
            else
            {
                resultF.isBeenEdit = true;
                resultF.pLU.isBeenEdit = true;
                resultF.pLD.isBeenEdit = true;
                resultF.pRU.isBeenEdit = true;
                resultF.pRD.isBeenEdit = true;
            }
            //添加点集合
            resultF.baPoints.Add(resultF.pLU);
            resultF.baPoints.Add(resultF.pRU);
            resultF.baPoints.Add(resultF.pRD);
            resultF.baPoints.Add(resultF.pLD);
            //计算图元基本属性
            resultF.iD = CreateId();
            resultF.location = new PointF(pMin.X, pMin.Y);
            resultF.width = Math.Abs(pMax.X - pMin.X);
            resultF.height = Math.Abs(pMax.Y - pMin.Y);
            resultF.PixelsFlush(commondata);

            return resultF;
        }

        private FigureBase CreateEllipse(Point p, Boolean isDemo)//生成椭圆函数
        {
            BaFigureEllipse resultF = new BaFigureEllipse(commondata);

            //椭圆起止点
            PointF sp = lstPoints[0].location;
            PointF ep = commondata.PblToCvl(p);
            //规范化起止点
            PointF pMin = commondata.GetPMin(sp, ep);
            PointF pMax = commondata.GetPMax(sp, ep);
            //计算a,b,c
            resultF.a = (pMax.X - pMin.X) / 2;
            resultF.b = (pMax.Y - pMin.Y) / 2;
            resultF.c = (float)Math.Sqrt(Math.Abs(Math.Pow(resultF.b, 2) - Math.Pow(resultF.a, 2)));
            //计算两个焦点
            PointF pMid = new PointF();//先计算中点
            pMid.X = (pMin.X + pMax.X) / 2;
            pMid.Y = (pMin.Y + pMax.Y) / 2;
            if (resultF.a >= resultF.b)
            {
                resultF.pC1 = new BaPoint(new PointF(pMid.X - resultF.c, pMid.Y), commondata);
                resultF.pC2 = new BaPoint(new PointF(pMid.X + resultF.c, pMid.Y), commondata);
            }
            else
            {
                resultF.pC1 = new BaPoint(new PointF(pMid.X, pMid.Y - resultF.c), commondata);
                resultF.pC2 = new BaPoint(new PointF(pMid.X, pMid.Y + resultF.c), commondata);
            }

            if (isDemo == true)
                resultF.isBeenEdit = true;
            else
                resultF.isBeenEdit = false;

            //计算图元块属性
            resultF.iD = CreateId();
            resultF.location = new PointF();
            resultF.location.X = pMin.X; resultF.location.Y = pMin.Y;//左下角坐标
            resultF.width = pMax.X - pMin.X;
            resultF.height = pMax.Y - pMin.Y;
            resultF.mid = new PointF(resultF.location.X + resultF.width / 2, resultF.location.Y + resultF.height / 2);
            resultF.the = 0;
            //添加点集合
            resultF.baPoints.Add(resultF.pC1);
            resultF.baPoints.Add(resultF.pC2);
            //添加点的father
            //resultF.pC1.fathers.Add(resultF);
            //resultF.pC2.fathers.Add(resultF);
            resultF.PixelsFlush(commondata);

            return resultF;
        }

        private FigureBase CreateCircle(Point p, Boolean isDemo)//生成圆函数
        {
            BaFigureCircle resultF = new BaFigureCircle(commondata);

            resultF.center = lstPoints[0];//圆心            
            resultF.rad = commondata.GetDistance(resultF.center.location, commondata.PblToCvl(p));//半径       

            resultF.iD = CreateId();
            resultF.location = new PointF(resultF.center.location.X - resultF.rad, resultF.center.location.Y - resultF.rad);//坐标
            resultF.width = resultF.rad * 2;
            resultF.height = resultF.rad * 2;
            resultF.baPoints.Add(resultF.center);//添加点集合
            if (isDemo == false)//如果不是作的示范
            {
                resultF.center.fathers.Add(resultF);//给点添加父亲
                resultF.center.isBeenEdit = false;
                resultF.isBeenEdit = false;
            }
            else
            {
                resultF.center.isBeenEdit = true;
                resultF.isBeenEdit = true;
            }
            resultF.PixelsFlush(commondata);

            return resultF;
        }

        private FigureBase CreatePoint(Point p, Boolean isDemo)//生成点函数
        {
            BaFigurePoint resultF = new BaFigurePoint(commondata);
            resultF.point = lstPoints[0];
            resultF.location = resultF.point.location;
            resultF.width = 0; resultF.height = 0;
            resultF.baPoints.Add(resultF.point);
            if (isDemo == false)//如果不是作的示范
            {
                resultF.point.fathers.Add(resultF);
            }
            resultF.PixelsFlush(commondata);
            resultF.iD = CreateId();
            commondata.idNum++;

            return resultF;

        }

        private FigureBase CreateOptionalFigure(Point p, Boolean isDemo)//生成自选图形函数
        {
            OptionalFigure resultF = new OptionalFigure();
            switch (commondata.baseFigure)
            {
                case BaseFigure.TRIANGLE:
                    {
                        resultF = new OpFigureTriangle(commondata, lstPoints[0].location, commondata.PblToCvl(p));
                        break;
                    }
                case BaseFigure.PENTAGON:
                    {
                        resultF = new OpFigurePentagon(commondata, lstPoints[0].location, commondata.PblToCvl(p));
                        break;
                    }
                case BaseFigure.SEXANGLE:
                    {
                        resultF = new OpFigureSexangle(commondata, lstPoints[0].location, commondata.PblToCvl(p));
                        break;
                    }
                case BaseFigure.DIAMOND:
                    {
                        resultF = new OpFigureDiamond(commondata, lstPoints[0].location, commondata.PblToCvl(p));
                        break;
                    }
                case BaseFigure.ARROWLEFT:
                    {
                        resultF = new OpFigureArrowLeft(commondata, lstPoints[0].location, commondata.PblToCvl(p));
                        break;
                    }
                case BaseFigure.ARROWUP:
                    {
                        resultF = new OpFigureArrowUp(commondata, lstPoints[0].location, commondata.PblToCvl(p));
                        break;
                    }
                case BaseFigure.ARROWDOWN:
                    {
                        resultF = new OpFigureArrowDown(commondata, lstPoints[0].location, commondata.PblToCvl(p));
                        break;
                    }
                case BaseFigure.ARROWRIGHT:
                    {
                        resultF = new OpFigureArrowRight(commondata, lstPoints[0].location, commondata.PblToCvl(p));
                        break;
                    }
                case BaseFigure.STAR4:
                    {
                        resultF = new OpFigureStar4(commondata, lstPoints[0].location, commondata.PblToCvl(p));
                        break;
                    }
                case BaseFigure.STAR5:
                    {
                        resultF = new OpFigureStar5(commondata, lstPoints[0].location, commondata.PblToCvl(p));
                        break;
                    }
                case BaseFigure.STAR6:
                    {
                        resultF = new OpFigureStar6(commondata, lstPoints[0].location, commondata.PblToCvl(p));
                        break;
                    }

                    
            }
            if (isDemo == false)//如果不是作的示范
            {
                resultF.isBeenEdit = false;
            }
            else
            {
                resultF.isBeenEdit = true;
            }
            return resultF;
        }

        private void EndOfCreate(FigureBase fb)//创建图形末的共同工作
        {
            commondata.figureManager.AddFigure(fb);
            lstPoints = new List<BaPoint>();
            commondata.figureManager.PointsMagnetFlush();
            ListViewFlush();
            commondata.figureManager.IndexFlush();//刷新下标
            commondata.isDown = true;
            DrawCache();//刷新缓冲区域
        }

        private BaPoint GetMagentPoint(Point p)//用吸附方法生成图元点
        {
            if (commondata.magnet.isMagneting == true)//如果这个时候在吸附
            {
                return commondata.magnet.magnetPoint;//把吸附的点返回
            }
            else
            {
                return new BaPoint(commondata.PblToCvl(p),commondata);
            }    
        }

        private BaPoint PointCombine(BaPoint p, BaPoint p1)//将相同的两个点合一 否则不合一
        {
            if (p.location.X == p1.location.X && p.location.Y == p1.location.Y)
                return p1;
            else
                return p;
        }   

        public void ColorSelectDown(Point p)//取色
        {
            commondata.color = commondata.canvas.GetPixel(p.X, p.Y);
            commondata.colorDemoButton.BackColor = commondata.color;
        }       

        public void FillDown(Point p)//填充
        {
            Color c = commondata.canvas.GetPixel(p.X, p.Y);
            DrawVirtul();
            Fill(commondata.canvas, p.X, p.Y, c);
            for (int i = 0; i < fillPixels.Count(); i++)
            {
                PointF pf = commondata.PblToCvl(fillPixels[i]);
                commondata.figureManager.figurePixels.cores.Add(new PixelPointF(pf, commondata.color));
            }
            commondata.figureManager.figurePixels.PixelFlush(commondata);
            DrawCache();
            Draw();
        }

        //填充算法
        public void Fill(Bitmap btm, int x, int y, Color c)//填充函数
        {
            Bitmap bm = new Bitmap(btm);
            fillPixels = new List<Point>();//初始化像素点集合
            try
            {
                PixelExpand(bm, x, y, c);
            }
            catch (System.ArgumentOutOfRangeException e)
            {
                string str = e.ToString();
                fillPixels = new List<Point>();
                MessageBox.Show("请确保填充边界完整");
            }
        }
        public Boolean PixelExpand(Bitmap btm, int x, int y, Color c)//填充图扩展的递归算法
        {
            if (IsExpand(btm, x, y, c) == false)
                return false;
            PixelExpand(btm, x - 1, y, c);
            PixelExpand(btm, x + 1, y, c);
            PixelExpand(btm, x, y - 1, c);
            PixelExpand(btm, x, y + 1, c);

            return true;
        }
        public Boolean IsExpand(Bitmap btm, int x, int y, Color c)//判断是否扩展这个像素点
        {
            Color ct = btm.GetPixel(x, y);
            if (ct != c)
                return false;
            fillPixels.Add(new Point(x, y));
            btm.SetPixel(x, y, Color.FromArgb(100, 100, 100, 100));
            return true;
        }                   

        public string CreateId()//创建一个新的ID
        {
            string resultS;
            System.DateTime currentTime = new System.DateTime();
            currentTime = System.DateTime.Now;
            resultS = currentTime.ToString() + commondata.idNum;
            commondata.idNum++;
            return resultS;            
        }               
    }
}
