using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProcessingPhoto
{
    public partial class Form2 : Form
    {
        public double x, y, my;

        public Form2()
        {
            x = 0; y = 0; my = 0;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            x = Convert.ToInt32(textBox1.Text);
            y = Convert.ToInt32(textBox2.Text);
            my = Convert.ToInt32(textBox3.Text);

            Close();
        }
    }
}
