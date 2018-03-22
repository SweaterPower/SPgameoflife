using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace Conway_Game_of_Life
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public GameOfLife life;
        string tool = "pen";
        bool isdraw = false;
        Point Current;
        Point Previous;
        int hat = 0;
        int stripe = 0;
        public FileInfo fi;

        private void Form1_Load(object sender, EventArgs e)
        {
            life = new GameOfLife(pictureBox1, ScaleTrackBar.Minimum);
            ColorGB.Visible = false;
            saveFileDialog1.Filter = "Game of Life structure |*.cgls";
            openFileDialog1.Filter = "Game of Life structure |*.cgls";
            saveFileDialog2.Filter = "BMP Image |*.bmp";
            Rectangle rectAll = this.RectangleToClient(this.Bounds);
            Rectangle rectClient = this.ClientRectangle;
            hat = rectClient.Top - rectAll.Top + 1;
            stripe = rectClient.Left - rectAll.Left + 2;
            toolStripButton1.Checked = true;
        }

        private void ScaleTrackBar_Scroll(object sender, EventArgs e)
        {
            life.Scale = ScaleTrackBar.Value;
            life.RefreshCorners();
            life.DrawField();
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            Array.Copy(life.field, life.raw, life.field.Length);
            timer1.Start();
            StepButton.Enabled = false;
            ResetButton.Enabled = false;
            PauseButton.Enabled = true;
            StartButton.Enabled = false;
        }

        private void StepButton_Click(object sender, EventArgs e)
        {
            ResetButton.Enabled = true;
            life.LifeStep();
        }

        private void PauseButton_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            StepButton.Enabled = true;
            ResetButton.Enabled = true;
            PauseButton.Enabled = false;
            StartButton.Enabled = true;
        }

        private void SpeedTrackBar_Scroll(object sender, EventArgs e)
        {
            timer1.Interval = Convert.ToInt32(Math.Pow((11 - SpeedTrackBar.Value), 3));
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            tool = "pen";
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                Point position = new Point(MousePosition.X - this.Location.X - pictureBox1.Location.X - stripe, MousePosition.Y - this.Location.Y - pictureBox1.Location.Y - hat);
                //PEREDELAT PEREDELAT PEREDELAT PEREDELAT PEREDELAT PEREDELAT
                int tmpx = (position.X) / (1 + ScaleTrackBar.Value) + (life.FstartX) / (1 + ScaleTrackBar.Value);
                int tmpy = (position.Y) / (1 + ScaleTrackBar.Value) + (life.FstartY) / (1 + ScaleTrackBar.Value);
                if (this.pictureBox1.Cursor == Cursors.Cross)
                {
                    if (Control.ModifierKeys != Keys.Control)
                    this.pictureBox1.Cursor = Cursors.Arrow;
                    life.OpenStructure(fi, tmpx, tmpy);
                }
                else
                {
                    isdraw = true;
                    Current = e.Location;
                    switch (tool)
                    {
                        case "pen":
                            life.field[tmpx, tmpy] = !life.field[tmpx, tmpy];
                            life.colors[tmpx, tmpy] = life.curcolor;
                            break;
                        case "brush":
                            life.field[tmpx, tmpy] = true;
                            life.colors[tmpx, tmpy] = life.curcolor;
                            break;
                        case "eraser":
                            life.field[tmpx, tmpy] = false;
                            life.colors[tmpx, tmpy] = 0;
                            break;
                        case "selection": life.Selecton.sx = tmpx; life.Selecton.sy = tmpy; break;
                    }
                    life.SetArea(ref life.isx, tmpx);
                    life.SetArea(ref life.isy, tmpy);
                    life.UpdateBorders(tmpx, tmpy);
                    life.DrawField();
                }
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (this.pictureBox1.Cursor != Cursors.Cross)
                {
                    life.SwapBrushes();
                    if (life.curcolor == 2)
                        ColorPanel.BackColor = Color.Red;
                    else ColorPanel.BackColor = Color.Blue;
                }
                if (tool == "hand")
                    this.pictureBox1.Cursor = Cursors.Hand;
                else
                    this.pictureBox1.Cursor = Cursors.Arrow;
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            isdraw = false;
            memx = memy = 0;
        }

        int memx = -1;
        int memy = -1;
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            pictureBox1.Focus();
            if (isdraw)
            {
                Previous = Current;
                Current = e.Location;
                    switch (tool)
                    {
                        case ("hand"):
                            life.FstartX += Previous.X - Current.X;
                            life.FstartY += Previous.Y - Current.Y;
                            life.DrawField();
                            break;

                        case ("eraser"):
                        case ("brush"):
                            Point position = new Point(MousePosition.X - this.Location.X - pictureBox1.Location.X - stripe, MousePosition.Y - this.Location.Y - pictureBox1.Location.Y - hat);
                            if (position.X >= pictureBox1.Width || position.Y >= pictureBox1.Height || position.X < 0 || position.Y < 0) return;
                            int tmpx = (position.X + life.FstartX) / (1 + ScaleTrackBar.Value);
                            int tmpy = (position.Y + life.FstartY) / (1 + ScaleTrackBar.Value);
                            if (tool == "brush")
                            {
                                life.field[tmpx, tmpy] = true;
                                life.colors[tmpx, tmpy] = life.curcolor;
                                life.SetArea(ref life.isx, tmpx);
                                life.SetArea(ref life.isy, tmpy);
                            }
                            else
                            {
                                life.field[tmpx, tmpy] = false;
                                life.colors[tmpx, tmpy] = 0;
                            }
                            life.UpdateBorders(tmpx, tmpy);
                            if (tmpx != memx || tmpy != memy)
                            {
                                life.DrawField();
                                memx = tmpx;
                                memy = tmpy;
                            }
                            break;
                        case ("selection"):
                            position = new Point(MousePosition.X - this.Location.X - pictureBox1.Location.X - stripe, MousePosition.Y - this.Location.Y - pictureBox1.Location.Y - hat);
                            if (position.X >= pictureBox1.Width || position.Y >= pictureBox1.Height || position.X < 0 || position.Y < 0) return;
                            tmpx = (position.X + life.FstartX) / (1 + ScaleTrackBar.Value);
                            tmpy = (position.Y + life.FstartY) / (1 + ScaleTrackBar.Value);
                            if (tmpx != memx || tmpy != memy)
                            {
                                memx = tmpx;
                                memy = tmpy;
                                life.Selecton.fx = tmpx; life.Selecton.fy = tmpy;
                                life.DrawField();
                            }
                            break;
                    }
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            tool = "hand";
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            tool = "brush";
        }

        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (MouseButtons == MouseButtons.None)
            {
                if (e.Delta < 0)
                {
                    if (ScaleTrackBar.Value - 1 < ScaleTrackBar.Minimum) return;
                    if (ScaleTrackBar.Value > ScaleTrackBar.Minimum)
                        ScaleTrackBar.Value--;
                    life.Scale = ScaleTrackBar.Value;
                }
                else if (e.Delta > 0)
                {
                    if (ScaleTrackBar.Value + 1 > ScaleTrackBar.Maximum) return;
                    if (ScaleTrackBar.Value < ScaleTrackBar.Maximum)
                        ScaleTrackBar.Value++;
                    life.Scale = ScaleTrackBar.Value;
                }
                life.RefreshCorners();
                life.DrawField();
            }
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            ResetButton.Enabled = false;
            life.Reset();
            life.DrawField();
        }

        private void библиотекаСтруктурToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Structures sform = new Structures(pictureBox1);
            sform.Owner = this;
            sform.Show();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            tool = "eraser";
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            tool = "selection";
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "Навигация")
                this.pictureBox1.Cursor = Cursors.Hand;
            else this.pictureBox1.Cursor = Cursors.Arrow;
            foreach (ToolStripItem item in ((ToolStrip)sender).Items)
            {
                if (item is ToolStripButton)
                    ((ToolStripButton)(item)).Checked = false;
            }
            if (e.ClickedItem is ToolStripButton)
                ((ToolStripButton)(e.ClickedItem)).Checked = true;
            life.Selecton.fx = life.Selecton.sx;
            life.Selecton.fy = life.Selecton.sy;
            life.DrawField();
        }

        private void очиститьПолеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Поле будет полностью очищено.\nДанная операция необратима.\nВы уверены?", "Внимание!", MessageBoxButtons.YesNo);
            if (dr != System.Windows.Forms.DialogResult.No)
            {
                life.ClearField();
                life.DrawField();
            }
                
        }

        private void вклотклСеткуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((ToolStripMenuItem)sender).Checked = life.OnOffGrid();
            life.DrawField();
        }

        private void режимДвухЦветовToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((ToolStripMenuItem)sender).Checked = ColorGB.Visible = life.OnOffColoredMode();
            life.DrawField();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            life.LiveTheLife();
        }

        private void ColorButton_Click(object sender, EventArgs e)
        {
            life.SwapBrushes();
            if (life.curcolor == 2)
                ColorPanel.BackColor = Color.Red;
            else ColorPanel.BackColor = Color.Blue;
        }

        private void WinnerButton_Click(object sender, EventArgs e)
        {
            ResultLabel.Text = life.WhoWin();
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                life.SaveStructure(new FileInfo(saveFileDialog1.FileName), life.Selecton);
                FileInfo fi = new FileInfo(saveFileDialog1.FileName);
                life.SaveStructure(new FileInfo(Application.StartupPath + @"\structlib\" + fi.Name), life.Selecton);
            }
        }

        public string name = "";

        private void сохранитьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            NameInputBox nib = new NameInputBox(this);
            nib.ShowDialog();
            if (nib.DialogResult == System.Windows.Forms.DialogResult.OK && name != "")
            {
                life.SaveStructure(new FileInfo(Application.StartupPath + @"\structlib\" + name + ".cgls"), life.Selecton);
            }
        }

        private void загрузитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.pictureBox1.Cursor = Cursors.Cross;
                fi = new FileInfo(openFileDialog1.FileName);
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F1)
            {
                Process.Start("GameOfLife_help.hnd");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (saveFileDialog2.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    life.bmp.Save(saveFileDialog2.FileName);
                }
                catch
                {
                    MessageBox.Show("Ошибка при сохранении изображения.", "Ошибка", MessageBoxButtons.OK);
                }
            }
        }

        private void menuStrip1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.F1)
            {
                Process.Start("GameOfLifeHelp.chm");
            }
        }

        private void pictureBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.F1)
            {
                Process.Start("GameOfLifeHelp.chm");
            }
            //else if (e.KeyData == Keys.Delete)
            //{
            //    Deleting();
            //}
        }

        void Deleting()
        {
            if (MessageBox.Show("Очистить выделенную область?", "Внимание!", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                life.ClearArea(life.Selecton);                
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Deleting();
        }
    }
}
