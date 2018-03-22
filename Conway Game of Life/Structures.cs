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

namespace Conway_Game_of_Life
{
    public partial class Structures : Form
    {
        public Structures(PictureBox pb)
        {
            InitializeComponent();
            pibo = pb;
        }

        FileInfo fileinf;
        DirectoryInfo libfolder;
        PictureBox pibo;

        private void Structures_Load(object sender, EventArgs e)
        {
            libfolder = new DirectoryInfo(Application.StartupPath + @"\structlib");
            RefreshLib();
        }

        void RefreshLib()
        {
            listBox1.Items.Clear();
            foreach (FileInfo item in libfolder.GetFiles())
            {
                listBox1.Items.Add(item);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ((Form1)this.Owner).life.DrawStructure(pictureBox1, (FileInfo)listBox1.SelectedItem);
            fileinf = (FileInfo)listBox1.SelectedItem;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                pibo.Cursor = Cursors.Cross;
                ((Form1)this.Owner).fi = (FileInfo)listBox1.SelectedItem;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Удалить структуру из библиотеки?\nДанная операция необратима!","Внимание!",MessageBoxButtons.OKCancel)== System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    //if (((Form1)this.Owner).fi == (FileInfo)listBox1.SelectedItem)
                    fileinf.Delete();
                    fileinf = null;
                    RefreshLib();
                }
                catch
                {
                    MessageBox.Show("Ошибка при удалении файла.", "Ошибка", MessageBoxButtons.OK);
                }
            }
        }

        private void Structures_Activated(object sender, EventArgs e)
        {
            int ind = listBox1.SelectedIndex;
            RefreshLib();
            if (ind < listBox1.Items.Count)
                listBox1.SelectedIndex = ind;
            else listBox1.SelectedIndex = listBox1.Items.Count - 1;
        }
    }
}
