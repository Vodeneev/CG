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
    public partial class Photo : Form
    {
        public Bitmap image;
        public Stack<Bitmap> saving = new Stack<Bitmap>(5);
        public StructElem strelem = new StructElem();
        public float[,] elem;
        public Photo()
        {
            InitializeComponent();
            elem = new float[3, 3] { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };
        }

        private void Photo_Load(object sender, EventArgs e)
        {

        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog(); //для открытия файла
            dialog.Filter = "Картинки|*.png;*.jpg;*.bmp|Все файлы(*.*)|*.*";
            if (dialog.ShowDialog() == DialogResult.OK)
                image = new Bitmap(dialog.FileName);
            pictureBox1.Image = image;
            saving.Push(image);
            pictureBox1.Refresh();
        }

        private void инверсияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InvertFilters filter = new InvertFilters();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Bitmap newImage = ((Filtres)e.Argument).processImage(image, backgroundWorker1);
            if (backgroundWorker1.CancellationPending != true)
            {
                image = newImage;
                saving.Push(image);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(!e.Cancelled)
            {
                pictureBox1.Image = image;
                pictureBox1.Refresh();
            }
            progressBar1.Value = 0;
        }

        private void матричныеToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void размытиеToolStripMenuItem_Click(object sender, EventArgs e)
        { }
        private void стандартныйФильтрToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filtres filter = new BlurFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void фильтрГауссаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filtres filter = new GaussianFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void фильтрСобеляToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filtres filter = new SobelFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void резкостьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filtres filter = new SharpnessFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void фильтрШаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filtres filter = new SharrFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void фильтрПрюиттаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filtres filter = new PruittFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }
        private void устранениеШумаToolStripMenuItem_Click(object sender, EventArgs e)
        { }
        private void медианныйФильтрToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filtres filter = new MedianFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }
        private void откатДействияToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (saving.Count() == 1)
            {
                pictureBox1.Image = saving.Peek();
                pictureBox1.Refresh();
            }
            else
            {
                saving.Pop();
                pictureBox1.Image = saving.Pop();
                pictureBox1.Refresh();
            }
        }

        private void серыйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GrayScale filter = new GrayScale();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void сепияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sepia filter = new Sepia();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void увеличениеЯркостиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bright filter = new bright();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void серыйМирToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GrayWorld filter = new GrayWorld();
            backgroundWorker1.RunWorkerAsync(filter);
        }
        private void линейноеРастяжениеГистограммыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StretchingTheHistogram filter = new StretchingTheHistogram();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void стеклоToolStripMenuItem_Click(object sender, EventArgs e)
        {
            glass filter = new glass();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
            saveFileDialog1.Title = "Save an Image File";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                switch (saveFileDialog1.FilterIndex)
                {
                    case 1:
                        this.pictureBox1.Image.Save(fs,
                          System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;

                    case 2:
                        this.pictureBox1.Image.Save(fs,
                          System.Drawing.Imaging.ImageFormat.Bmp);
                        break;

                    case 3:
                        this.pictureBox1.Image.Save(fs,
                          System.Drawing.Imaging.ImageFormat.Gif);
                        break;
                }

                fs.Close();
            }

            this.pictureBox1.Click += new System.EventHandler(this.сохранитьToolStripMenuItem_Click);
        }
        private void волныToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Waves filter = new Waves();
            backgroundWorker1.RunWorkerAsync(filter);
        }
        private void открытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filtres f6 = new Opening();
            backgroundWorker1.RunWorkerAsync(f6);
        }

        private void закрытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filtres f6 = new Closing();
            backgroundWorker1.RunWorkerAsync(f6);
        }

        private void градиентToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filtres f6 = new Grad();
            backgroundWorker1.RunWorkerAsync(f6);
        }

        private void расширениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filtres f6 = new Dilation(elem);
            backgroundWorker1.RunWorkerAsync(f6);
        }

        private void сужениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filtres f6 = new Erosion(elem);
            backgroundWorker1.RunWorkerAsync(f6);
        }

        private void topHotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filtres f6 = new TopHat();
            backgroundWorker1.RunWorkerAsync(f6);
        }

        private void изменитьСтруктурныйЭлементToolStripMenuItem_Click(object sender, EventArgs e)
        {
            strelem.ShowDialog();
            elem = strelem.structelem;
            
        }
    }
}
