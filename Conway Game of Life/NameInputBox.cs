using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Conway_Game_of_Life
{
    public partial class NameInputBox : Form
    {
        public NameInputBox(Form1 f)
        {
            InitializeComponent();
            form = f;
        }

        Form1 form;

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            form.name = textBox1.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void NameInputBox_FormClosing(object sender, FormClosingEventArgs e)
        {
            //this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter) this.Close();
        }
    }
}
