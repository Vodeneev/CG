using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Drawing;

namespace ProcessingPhoto
{
    abstract class Filtres
    {
        public int Clamp(int value, int min, int max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }
        protected abstract Color calculateNewPixelColor(Bitmap sourseImage, int x, int y);
        public virtual Bitmap processImage(Bitmap sourseImage, BackgroundWorker worker)
        {
            Bitmap resultImage = new Bitmap(sourseImage.Width, sourseImage.Height);
            for (int i = 0; i < sourseImage.Width; i++)
            {
                worker.ReportProgress((int)((float)i / resultImage.Width * 100));
                if (worker.CancellationPending)
                    return null;
                for (int j = 0; j < sourseImage.Height; j++)
                    resultImage.SetPixel(i, j, calculateNewPixelColor(sourseImage, i, j));
            }
            return resultImage;
        }
    }

    class InvertFilters : Filtres
    {
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            Color sourseColor = sourseImage.GetPixel(x, y);
            Color resultColor = Color.FromArgb(255 - sourseColor.R, 255 - sourseColor.G, 255 - sourseColor.B);
            return resultColor;
        }
    }
    class MatrixFilters : Filtres
    {
        protected float[,] kernel = null;
        protected MatrixFilters() { }
        public MatrixFilters(float[,] kernel)
        {
            this.kernel = kernel;
        }
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;
            float resultR = 0, resultG = 0, resultB = 0;
            for (int l = -radiusY; l <= radiusY; l++)
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = Clamp(x + k, 0, sourseImage.Width - 1);
                    int idY = Clamp(y + l, 0, sourseImage.Height - 1);
                    Color neighborColor = sourseImage.GetPixel(idX, idY);
                    resultR += neighborColor.R * kernel[k + radiusX, l + radiusY];
                    resultG += neighborColor.G * kernel[k + radiusX, l + radiusY];
                    resultB += neighborColor.B * kernel[k + radiusX, l + radiusY];
                }
            return Color.FromArgb(Clamp((int)resultR, 0, 255), Clamp((int)resultG, 0, 255), Clamp((int)resultB, 0, 255));
        }
    }

    class BlurFilter : MatrixFilters
    {
        public BlurFilter()
        {
            int sizeX = 3;
            int sizeY = 3;
            kernel = new float[sizeX, sizeY];
            for (int i = 0; i < sizeX; i++)
                for (int j = 0; j < sizeY; j++)
                    kernel[i, j] = 1.0f / (float)(sizeX * sizeY);
        }
    }

    class GaussianFilter : MatrixFilters
    {
        private int radius = 3;
        private int sigma = 2;
        public GaussianFilter()
        {
            createCaussianKernel(radius, sigma);
        }
        public void createCaussianKernel(int radius, float sigma)
        {
            int size = 2 * radius + 1; //размер ядра
            kernel = new float[size, size]; //ядро фильтра
            float norm = 0; //коэффициент нормировки ядра
            for (int i = -radius; i <= radius; i++) //заполнение элементов ядра
                for (int j = -radius; j <= radius; j++)
                {
                    kernel[i + radius, j + radius] = (float)(Math.Exp(-(i * i + j * j) / (2 * sigma * sigma)));
                    norm += kernel[i + radius, j + radius];
                }
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    kernel[i, j] /= norm;
        }
    }
    class SobelFilter : MatrixFilters
    {
        private float[,] kernelX = null;
        private float[,] kernelY = null;
        public SobelFilter()
        {
            kernelX = new float[3, 3] { { -1f, 0f, -1f }, { -2f, 0f, 2f }, { -1f, 0f, 1f } }; // по Х
            kernelY = new float[3, 3] { { -1f, -2f, -1f }, { 0f, 0f, 0f }, { 1f, 2f, 1f } }; // по Y
        }
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            float red, green, blue;
            kernel = kernelX;
            Color gradX = base.calculateNewPixelColor(sourseImage, x, y);
            kernel = kernelY;
            Color gradY = base.calculateNewPixelColor(sourseImage, x, y);
            red = (float)Math.Sqrt(gradX.R * gradX.R + gradY.R * gradY.R);
            green = (float)Math.Sqrt(gradX.G * gradX.G + gradY.G * gradY.G);
            blue = (float)Math.Sqrt(gradX.B * gradX.B + gradY.B * gradY.B);
            return Color.FromArgb(Clamp((int)red, 0, 255), Clamp((int)green, 0, 255), Clamp((int)blue, 0, 255));
        }
    }
    class SharpnessFilter : MatrixFilters
    {
        public SharpnessFilter()
        {
            kernel = new float[3, 3] { { 0f, -1f, 0f }, { -1f, 5f, -1f }, { 0f, -1f, 0f } };
        }
    }
    class SharrFilter : MatrixFilters
    {
        private float[,] kernelX = null;
        private float[,] kernelY = null;
        public SharrFilter()
        {
            kernelX = new float[3, 3] { { 3f, 0f, -3f }, { 10f, 0f, -10f }, { 3f, 0f, -3f } }; // по Х
            kernelY = new float[3, 3] { { 3f, 10f, 3f }, { 0f, 0f, 0f }, { -3f, -10f, -3f } }; // по Y
        }
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            float red, green, blue;
            kernel = kernelX;
            Color gradX = base.calculateNewPixelColor(sourseImage, x, y);
            kernel = kernelY;
            Color gradY = base.calculateNewPixelColor(sourseImage, x, y);
            red = (float)Math.Sqrt(gradX.R * gradX.R + gradY.R * gradY.R);
            green = (float)Math.Sqrt(gradX.G * gradX.G + gradY.G * gradY.G);
            blue = (float)Math.Sqrt(gradX.B * gradX.B + gradY.B * gradY.B);
            return Color.FromArgb(Clamp((int)red, 0, 255), Clamp((int)green, 0, 255), Clamp((int)blue, 0, 255));
        }
    }
    class PruittFilter : MatrixFilters
    {
        private float[,] kernelX = null;
        private float[,] kernelY = null;
        public PruittFilter()
        {
            kernelX = new float[3, 3] { { -1f, 0f, 1f }, { -1f, 0f, 1f }, { -1f, 0f, 1f } }; // по Х
            kernelY = new float[3, 3] { { -1f, -1f, -1f }, { 0f, 0f, 0f }, { 1f, 1f, 1f } }; // по Y
        }
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            float red, green, blue;
            kernel = kernelX;
            Color gradX = base.calculateNewPixelColor(sourseImage, x, y);
            kernel = kernelY;
            Color gradY = base.calculateNewPixelColor(sourseImage, x, y);
            red = (float)Math.Sqrt(gradX.R * gradX.R + gradY.R * gradY.R);
            green = (float)Math.Sqrt(gradX.G * gradX.G + gradY.G * gradY.G);
            blue = (float)Math.Sqrt(gradX.B * gradX.B + gradY.B * gradY.B);
            return Color.FromArgb(Clamp((int)red, 0, 255), Clamp((int)green, 0, 255), Clamp((int)blue, 0, 255));
        }
    }
    class MedianFilter : PruittFilter
    {
        private float[] arrayR = null;
        private float[] arrayG = null;
        private float[] arrayB = null;
        public MedianFilter()
        {
            arrayR = new float[9];
            arrayG = new float[9];
            arrayB = new float[9];
        }
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            int count = 0;
            for (int l = -1; l <= 1; l++)
                for (int k = -1; k <= 1; k++, count++)
                {
                    int idX = Clamp(x + k, 0, sourseImage.Width - 1); // номерация с нуля, контроль границ
                    int idY = Clamp(y + l, 0, sourseImage.Height - 1); // номерация с нуля, контроль границ
                    Color neighborColor = sourseImage.GetPixel(idX, idY);
                    arrayR[count] = neighborColor.R;
                    arrayG[count] = neighborColor.G;
                    arrayB[count] = neighborColor.B;
                }
            Array.Sort(arrayR); Array.Sort(arrayG); Array.Sort(arrayB);
            return Color.FromArgb(Clamp((int)arrayR[4], 0, 255), Clamp((int)arrayG[4], 0, 255), Clamp((int)arrayB[4], 0, 255));
        }
    }
    class GrayScale : Filtres
    {
        protected override Color calculateNewPixelColor(Bitmap Source, int W, int H)
        {
            Color sourceColor = Source.GetPixel(W, H);
            double Intens = 0.299 * sourceColor.R + 0.587 * sourceColor.G + 0.114 * sourceColor.B;

            Color result = Color.FromArgb((int)Intens, (int)Intens, (int)Intens);

            return result;
        }
    };
    class Sepia : Filtres
    {
        protected override Color calculateNewPixelColor(Bitmap Source, int W, int H)
        {
            Color sourceColor = Source.GetPixel(W, H);
            double k = 30.0;
            double Intens = 0.299 * sourceColor.R + 0.587 * sourceColor.G + 0.114 * sourceColor.B;
            int R = (int)(Intens + 2.0 * k);
            int G = (int)(Intens + 0.5 * k);
            int B = (int)(Intens - k);
            Color result = Color.FromArgb(Clamp(R, 0, 255), Clamp(G, 0, 255), Clamp(B, 0, 255));

            return result;
        }
    };
    class bright : Filtres //увеличение яркости
    {
        protected override Color calculateNewPixelColor(Bitmap Source, int W, int H)
        {
            Color sourceColor = Source.GetPixel(W, H);

            Color result = Color.FromArgb(Clamp(sourceColor.R + 20, 0, 255), Clamp(sourceColor.G + 20, 0, 255), Clamp(sourceColor.B + 20, 0, 255));

            return result;
        }
    };
    class GrayWorld : Filtres //серый мир
    {
        public int Avg;
        public int R, G, B;

        protected override Color calculateNewPixelColor(Bitmap Source, int W, int H)
        {
            Color sourceColor = Source.GetPixel(W, H);

            Color result = Color.FromArgb(Clamp(sourceColor.R * Avg / R, 0, 255), Clamp(sourceColor.G * Avg / G, 0, 255), Clamp(sourceColor.B * Avg / B, 0, 255));
            return result;
        }

        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worked)
        {
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            R = 0; G = 0; B = 0; Avg = 0;
            for (int i = 0; i < sourceImage.Width; i++)
            {
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    Color sourceColor = sourceImage.GetPixel(i, j);
                    R += sourceColor.R;
                    G += sourceColor.G;
                    B += sourceColor.B;
                }
            }
            int size = sourceImage.Width * sourceImage.Height;
            R = R / size;
            G = G / size;
            B = B / size;
            Avg = (R + G + B) / 3;

            for (int i = 0; i < sourceImage.Width; i++)
            {
                worked.ReportProgress((int)((float)i / sourceImage.Width * 100));
                if (worked.CancellationPending)
                    return null;
                for (int j = 0; j < sourceImage.Height; j++)
                    resultImage.SetPixel(i, j, calculateNewPixelColor(sourceImage, i, j));
            }
            return resultImage;
        }
    };
    class StretchingTheHistogram : Filtres //линейное растяжение гистограммы
    {
        public int F(int y, int ymax, int ymin)
        {
            return Clamp(((255 * (y - ymin)) / (ymax - ymin)), 0, 255);
        }
        protected override Color calculateNewPixelColor(Bitmap Source, int W, int H)
        {
            return Source.GetPixel(W, H);
        }
        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worked)
        {
            Bitmap result = new Bitmap(sourceImage.Width, sourceImage.Height);
            int minR = 0;
            int maxR = 0;
            int minG = 0;
            int maxG = 0;
            int minB = 0;
            int maxB = 0;
            for (int i = 0; i < sourceImage.Width; i++)
            {
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    Color pixColor = sourceImage.GetPixel(i, j);
                    if (minR > pixColor.R)
                        minR = pixColor.R;
                    if (maxR < pixColor.R)
                        maxR = pixColor.R;
                    if (minG > pixColor.G)
                        minG = pixColor.G;
                    if (maxG < pixColor.G)
                        maxG = pixColor.G;
                    if (minB > pixColor.B)
                        minB = pixColor.B;
                    if (maxB < pixColor.B)
                        maxB = pixColor.B;
                }
            }

            for (int i = 0; i < sourceImage.Width; i++)
            {
                worked.ReportProgress((int)((float)i / sourceImage.Width * 100));
                if (worked.CancellationPending)
                {
                    return null;
                }
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    int R = sourceImage.GetPixel(i, j).R;
                    int G = sourceImage.GetPixel(i, j).G;
                    int B = sourceImage.GetPixel(i, j).B;
                    result.SetPixel(i, j, Color.FromArgb(F(R, maxR, minR), F(G, maxG, minG), F(B, maxB, minB)));
                }
            }
            return result;
        }
    };
    class glass : Filtres //стекло
    {
        Random R;
        public glass()
        {
            R = new Random();
        }
        protected override Color calculateNewPixelColor(Bitmap Source, int W, int H)
        {
            //Random R = new Random();
            int X = Clamp((int)(W + R.NextDouble() * 10.0 - 5), 0, Source.Width - 1);
            int Y = Clamp((int)(H + R.NextDouble() * 10.0 - 5), 0, Source.Height - 1);
            Color result = Source.GetPixel(X, Y);
            return result;
        }

        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worked)
        {
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);

            for (int i = 0; i < sourceImage.Width; i++)
            {
                worked.ReportProgress((int)((float)i / resultImage.Width * 100));
                if (worked.CancellationPending)
                {
                    return null;
                }
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    resultImage.SetPixel(i, j, calculateNewPixelColor(sourceImage, i, j));
                }
            }
            return resultImage;
        }
    };

    //волны
    class Waves : Filtres
    {
        protected override Color calculateNewPixelColor(Bitmap Source, int W, int H)
        {
            Color sourceColor = Source.GetPixel(W, H);
            int nX = Clamp((int)(W + 20 * Math.Sin(0.20943951023 * W)), 0, Source.Width - 1);
            int nY = Clamp(H, 0, Source.Height - 1);
            return Source.GetPixel(nX, nY);

        }
    };
}
