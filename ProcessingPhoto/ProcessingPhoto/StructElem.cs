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
    public partial class StructElem : Form
    {
        public float[,] structelem;
        public StructElem()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int n = Convert.ToInt32(textBox1.Text);
            dataGridView1.ColumnCount = n;
            dataGridView1.RowCount = n;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int n = Convert.ToInt32(textBox1.Text);
            structelem = new float[n, n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    structelem[i, j] = Convert.ToInt16(dataGridView1.Rows[i].Cells[j].Value);
            Close();
        }
    }
}
