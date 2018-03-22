using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace Conway_Game_of_Life
{
    //class ColoredPoint
    //{
    //    int x;
    //    int y;
    //    int color;

    //    public int X
    //    {
    //        get { return x; }
    //        set { if (value >= 0) x = value; }
    //    }

    //    public int Y
    //    {
    //        get { return y; }
    //        set { if (value >= 0) y = value; }
    //    }

    //    public int Color
    //    {
    //        get { return color; }
    //        set { color = value; }
    //    }

    //    public ColoredPoint(int x, int y, int color)
    //    {
    //        X = x;
    //        Y = y;
    //        Color = color;
    //    }
    //}

    public class GameOfLife
    {
        public struct LifeSelection
        {
            public int sx;
            public int sy;
            public int fx;
            public int fy;

            public bool Check()
            {
                return sx != fx && sy != fy;
            }
        }

        public bool[,] field;
        public int[,] colors;
        public bool[,] raw;
        bool[][,] step;
        int index = 0;
        int areax = 0;
        int areay = 0;
        int fstart_x = 0;
        int fstart_y = 0;
        public bool[] isx;
        public bool[] isy;
        bool colored = false;
        bool griding = true;
        int scale = 0;
        int updatetime = 0;
        int interval = 0;
        int mainsize = 0;
        public LifeSelection Selecton = new LifeSelection();
        public int curcol = 1;
        Brush curbrush;
        Brush[] brushes = { Brushes.Blue, Brushes.Red };
        int brushindex = 0;
        Pen area;

        Graphics g;
        public Bitmap bmp;
        PictureBox pb;

        public int curcolor
        {
            get { return curcol; }
            set
            {
                if (value == 1 || value == 2)
                    curcol = value;
            }
        }

        public int Scale
        {
            get { return scale; }
            set
            {
                scale = value;
            }
        }

        public int FstartX
        {
            get { return fstart_x; }
            set
            {
                fstart_x = value;
                RefreshCorner(ref fstart_x);
            }
        }

        public int FstartY
        {
            get { return fstart_y; }
            set
            {
                fstart_y = value;
                RefreshCorner(ref fstart_y);
            }
        }

        public int Interval
        {
            get { return interval; }
            set { if (value >= 0) interval = value; }
        }

        void RefreshCorner(ref int fstart)
        {
            if (fstart / (1 + Scale) >= mainsize - (pb.Width / (1 + Scale))) fstart = mainsize - (pb.Width / (1 + Scale)) - 1;
            else
                if (fstart < 0) fstart = 0;
        }

        public void RefreshCorners()
        {
            RefreshCorner(ref fstart_x);
            RefreshCorner(ref fstart_y);
        }

        public GameOfLife(PictureBox picturebox, int mincell)
        {
            pb = picturebox;
            bmp = new Bitmap(picturebox.Width, picturebox.Height);
            g = Graphics.FromImage(bmp);
            int MAINSIZE = Math.Max(picturebox.Height / mincell, picturebox.Width / mincell);

            raw = new bool[MAINSIZE, MAINSIZE];
            step = new bool[2][,];
            step[0] = new bool[MAINSIZE, MAINSIZE];
            step[1] = new bool[MAINSIZE, MAINSIZE];
            isx = new bool[MAINSIZE];
            isy = new bool[MAINSIZE];
            colors = new int[MAINSIZE, MAINSIZE];

            Scale = mincell;
            field = step[1];
            mainsize = MAINSIZE;
            FstartX = 0;
            FstartY = 0;
            curbrush = brushes[brushindex];
            area = new Pen(Color.DarkGreen, 2);
            DrawField();
            Life();
        }

        public void PointToCells(ref int newx, ref int newy, Point mouseposition)
        {

        }

        public void SwapBrushes()
        {
            if (colored)
            {
                curcolor = GetNextIndex(curcolor - 1) + 1;
                brushindex = GetNextIndex(brushindex);
                curbrush = brushes[brushindex];
            }
        }

        public void UpdateBorders(int tmpx, int tmpy)
        {
            if (tmpx > areax - 1)
                if (tmpx + 1 < field.GetLength(0))
                    areax = tmpx + 1;
            if (tmpy > areay - 1)
                if (tmpy + 1 < field.GetLength(1))
                    areay = tmpy + 1;
        }

        public bool OnOffColoredMode()
        {
            colored = !colored;
            if (!colored)
            {
                brushindex = 0;
                curbrush = brushes[brushindex];
                curcolor = 1;
            }
            return colored;
        }

        public bool OnOffGrid()
        {
            griding = !griding;
            return griding;
        }

        public void DrawField()
        {
            if (colored)
                DrawField_Colored();
            else DrawField_Standart();
        }

        public void DrawField_Standart()
        {
            g.Clear(Color.White);
            if (griding)
            {
                int sizer = 0;
                while (sizer <= pb.Width)
                {
                    g.DrawLine(Pens.Gray, new Point(0 + sizer, 0), new Point(0 + sizer, pb.Height));
                    g.DrawLine(Pens.Gray, new Point(0, 0 + sizer), new Point(pb.Width, 0 + sizer));
                    sizer += 1 + Scale;
                }
            }
            int startx = fstart_x / (1 + Scale);
            int starty = fstart_y / (1 + Scale);
            for (int i = startx; i < field.GetLength(0); i++)
                for (int j = starty; j < field.GetLength(1); j++)
                {
                    if (field[i, j])
                    {
                        //if (i * (ScaleTrackBar.Value + 1) < pictureBox1.Width && j * (ScaleTrackBar.Value + 1) < pictureBox1.Height)
                        g.FillRectangle(Brushes.Blue, (i - startx) * (Scale + 1) + 1, (j - starty) * (Scale + 1) + 1, Scale, Scale);
                    }
                }
            if (Selecton.Check())
                g.DrawRectangle(area, (Selecton.sx - startx) * (Scale + 1), (Selecton.sy - starty) * (Scale + 1), (Selecton.fx - Selecton.sx) * (Scale + 1), (Selecton.fy - Selecton.sy) * (Scale + 1));
            pb.Image = bmp;
        }

        public void DrawField_Colored()
        {
            g.Clear(Color.White);
            if (griding)
            {
                int sizer = 0;
                while (sizer <= pb.Width)
                {
                    g.DrawLine(Pens.Gray, new Point(0 + sizer, 0), new Point(0 + sizer, pb.Height));
                    g.DrawLine(Pens.Gray, new Point(0, 0 + sizer), new Point(pb.Width, 0 + sizer));
                    sizer += 1 + Scale;
                }
            }
            int startx = fstart_x / (1 + Scale);
            int starty = fstart_y / (1 + Scale);
            for (int i = startx; i < field.GetLength(0); i++)
                for (int j = starty; j < field.GetLength(1); j++)
                {
                    if (field[i, j])
                    {
                        //if (i * (ScaleTrackBar.Value + 1) < pictureBox1.Width && j * (ScaleTrackBar.Value + 1) < pictureBox1.Height)
                        g.FillRectangle(brushes[colors[i, j] - 1], (i - startx) * (Scale + 1) + 1, (j - starty) * (Scale + 1) + 1, Scale, Scale);
                    }
                }
            if (Selecton.Check())
                g.DrawRectangle(area, Selecton.sx * (Scale + 1), Selecton.sy * (Scale + 1), (Selecton.fx - Selecton.sx) * (Scale + 1), (Selecton.fy - Selecton.sy) * (Scale + 1));
            pb.Image = bmp;
        }

        private void SwapSteps()
        {
            index = GetNextIndex(index);
            field = step[index];
        }

        private int GetNextIndex(int index)
        {
            if (index == 0) return 1;
            return 0;
        }

        public void SetArea(ref bool[] a, int index)
        {
            if (index >= 0) a[index] = true;
            if (index - 1 >= 0) a[index - 1] = true;
            if (index + 1 < a.Length) a[index + 1] = true;
        }

        public void Life()
        {
            if (colored)
                Life_Colored();
            else Life_Standart();
        }

        private void Life_Standart()
        {
            bool[] isxtmp = isx;
            bool[] isytmp = isy;
            bool[,] next = step[GetNextIndex(index)];
            for (int i = 0; i <= areax; i++)
                if (isx[i])
                    for (int j = 0; j <= areay; j++)
                        if (isy[j])
                        {
                            int count = 0;
                            for (int x = i - 1; x <= i + 1; x++)
                            {
                                for (int y = j - 1; y <= j + 1; y++)
                                {
                                    if ((x == i && j == y) || x < 0 || y < 0 || x >= areax || y >= areay) continue;
                                    if (field[x, y])
                                        count++;
                                }
                            }
                            if (count < 1 || count >= 4)
                            {
                                next[i, j] = false;
                            }
                            else if (field[i, j] && (count == 2 || count == 3)) next[i, j] = field[i, j];
                            else if (!field[i, j] && count == 3)
                            {
                                SetArea(ref isxtmp, i);
                                SetArea(ref isytmp, j);
                                next[i, j] = true;
                                colors[i, j] = 1;
                                if (i + 1 > areax)
                                    if (areax + 1 < field.GetLength(0))
                                        areax++;
                                if (j + 1 > areay)
                                    if (areay + 1 < field.GetLength(1))
                                        areay++;
                            }
                            else
                            {
                                next[i, j] = false;
                            }
                        }
            isx = isxtmp;
            isy = isytmp;
            SwapSteps();
        }

        private void Life_Colored()
        {
            Random rnd = new Random();
            bool[] isxtmp = isx;
            bool[] isytmp = isy;
            int[,] nextcol = new int[mainsize, mainsize];
            Array.Copy(colors, nextcol, colors.Length);
            bool[,] next = step[GetNextIndex(index)];
            for (int i = 0; i <= areax; i++)
                if (isx[i])
                    for (int j = 0; j <= areay; j++)
                        if (isy[j])
                        {
                            int count = 0;
                            int cb = 0;
                            int cr = 0;
                            for (int x = i - 1; x <= i + 1; x++)
                            {
                                for (int y = j - 1; y <= j + 1; y++)
                                {
                                    if ((x == i && j == y) || x < 0 || y < 0 || x >= areax || y >= areay) continue;
                                    if (field[x, y])
                                    {
                                        count++;
                                        if (colors[x, y] == 1) cb++;
                                        else if (colors[x, y] == 2) cr++;
                                    }
                                }
                            }
                            if (count < 1 || count >= 4)
                            {
                                next[i, j] = false;
                            }
                            else if (field[i, j] && (count == 2 || count == 3))
                            {
                                next[i, j] = field[i, j];
                                if (cb > cr)
                                    nextcol[i, j] = 1;
                                else if (cr > cb)
                                    nextcol[i, j] = 2;
                                else
                                    nextcol[i, j] = rnd.Next(1, 3);
                            }
                            else if (!field[i, j] && count == 3)
                            {
                                SetArea(ref isxtmp, i);
                                SetArea(ref isytmp, j);
                                next[i, j] = true;
                                if (cb > cr)
                                    nextcol[i, j] = 1;
                                else if (cr > cb)
                                    nextcol[i, j] = 2;
                                else
                                    nextcol[i, j] = rnd.Next(1, 3);
                                if (i + 1 > areax)
                                    if (areax + 1 < field.GetLength(0))
                                        areax++;
                                if (j + 1 > areay)
                                    if (areay + 1 < field.GetLength(1))
                                        areay++;
                            }
                            else
                            {
                                next[i, j] = false;
                            }
                        }
            isx = isxtmp;
            isy = isytmp;
            colors = nextcol;
            SwapSteps();
        }

        public void Reset()
        {
            field = raw;
        }

        public void LifeStep()
        {
            Life();
            DrawField();
            updatetime++;
        }

        public void LiveTheLife()
        {
            LifeStep();
            updatetime++;
            if (updatetime >= 50)
            {
                areax = 0;
                areay = 0;
                updatetime = 0;
                bool[] isxtmp = new bool[isx.Length];
                bool[] isytmp = new bool[isy.Length];
                foreach (bool[,] item in step)
                {
                    for (int i = 0; i < isx.Length; i++)
                    {
                        if (isx[i])
                        {
                            bool check = true;
                            for (int j = 0; j < isx.Length; j++)
                            {
                                check &= !item[i, j];
                            }
                            if (!check)
                            {
                                SetArea(ref isxtmp, i);
                                if (i > areax - 1)
                                    if (i + 1 < field.GetLength(0))
                                        areax = i + 1;
                            }
                        }

                        if (isy[i])
                        {
                            bool check = true;
                            for (int j = 0; j < isy.Length; j++)
                            {
                                check &= !item[j, i];
                            }
                            if (!check)
                            {
                                SetArea(ref isytmp, i);
                                if (i > areay - 1)
                                    if (i + 1 < field.GetLength(1))
                                        areay = i + 1;
                            }
                        }
                    }
                }
                isx = isxtmp;
                isy = isytmp;
            } 
            DrawField();
        }

        public void ClearField()
        {
            foreach (bool[,] item in step)
            {
                Array.Clear(item, 0, item.Length);
            }
            Array.Clear(field, 0, field.Length);
            Array.Clear(isx, 0, isx.Length);
            Array.Clear(isy, 0, isy.Length);
            Array.Clear(raw, 0, raw.Length);
            Array.Clear(colors, 0, colors.Length);
        }

        public void SaveStructure(FileInfo target, LifeSelection selection)
        {
            if (selection.sx < selection.fx || selection.sy < selection.fy)
            {
                StreamWriter sw = target.CreateText();
                sw.WriteLine(target.Name);
                sw.WriteLine(selection.fx - selection.sx);
                sw.WriteLine(selection.fy - selection.sy);
                for (int i = selection.sx; i < selection.fx; i++)
                    for (int j = selection.sy; j < selection.fy; j++)
                    {
                        if (field[i, j])
                        {
                            sw.WriteLine((i - selection.sx) + " " + (j - selection.sy) + " " + colors[i, j]);
                        }
                    }
                sw.Close();
            }
        }

        public void OpenStructure(FileInfo target, int x, int y)
        {
            StreamReader sr = target.OpenText();
            sr.ReadLine(); //я так и не понял зачем сохранил имя
            int xl = Convert.ToInt32(sr.ReadLine());
            int yl = Convert.ToInt32(sr.ReadLine());
            if (x + xl <= mainsize && y + yl <= mainsize)
            {
                while (sr.Peek() != -1)
                {
                    string[] pointdata = sr.ReadLine().Split(' ');
                    int curx = Convert.ToInt32(pointdata[0]);
                    int cury = Convert.ToInt32(pointdata[1]);
                    int color = Convert.ToInt32(pointdata[2]);
                    field[x + curx, y + cury] = true;
                    SetArea(ref isx, x + curx);
                    SetArea(ref isy, y + cury);
                    colors[x + curx, y + cury] = color;
                    UpdateBorders(x + curx, y + cury);
                }
            }
            sr.Close();
            DrawField();
        }

        public void DrawStructure(PictureBox pb, FileInfo file)
        {
            StreamReader sr = file.OpenText();
            sr.ReadLine();
            Bitmap bmptmp = new Bitmap(pb.Width, pb.Height);
            Graphics gtmp = Graphics.FromImage(bmptmp);
            int xl = Convert.ToInt32(sr.ReadLine());
            int yl = Convert.ToInt32(sr.ReadLine());
            int a = Math.Max(xl, yl);
            int w = (pb.Width / a);
            int h = (pb.Height / a);
            while (sr.Peek() != -1)
            {
                string[] pointdata = sr.ReadLine().Split(' ');
                int curx = Convert.ToInt32(pointdata[0]);
                int cury = Convert.ToInt32(pointdata[1]);
                int color = Convert.ToInt32(pointdata[2]);
                gtmp.FillRectangle(brushes[color - 1], w * curx, h * cury, w, h);
            }
            pb.Image = bmptmp;
            sr.Close();
        }

        public void ClearArea(LifeSelection selection)
        {
            for (int i = selection.sx; i <= selection.fx; i++)
                for (int j = selection.sy; j < selection.fy; j++)
                {
                    field[i, j] = false;
                    colors[i, j] = 0;
                }
            DrawField();        
        }

        public string WhoWin()
        {
            int cr = 0;
            int cb = 0;
            for (int i = 0; i < colors.GetLength(0); i++)
                for (int j = 0; j < colors.GetLength(0); j++)
                {
                    if (colors[i, j] == 1) cb++;
                    else if (colors[i, j] == 2) cr++;
                }
            if (cr > cb) return "Победили красные";
            if (cb > cr) return "Победили синие";
            if (cb == cr) return "Ничья.";
            return "Игра не окончена.";
        }
    }
}

