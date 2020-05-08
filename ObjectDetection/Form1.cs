using Alturos.Yolo;
using Alturos.Yolo.Model;
using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ObjectDetection
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static Bitmap bitmapImage;

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using(OpenFileDialog ofd = new OpenFileDialog() { Filter = "PNG|*.png|JPEG|*.jpeg"})
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    pic.Image = System.Drawing.Image.FromFile(ofd.FileName);
                    bitmapImage = new Bitmap(pic.Image);
                }
            }
        }

        private void btnDetect_Click(object sender, EventArgs e)
        {
            var configurationDetector = new YoloConfigurationDetector();
            var config = configurationDetector.Detect();

            using (var yoloWrapper = new YoloWrapper(config))
            {
                using(MemoryStream ms = new MemoryStream())
                {
                    pic.Image.Save(ms, ImageFormat.Png);
                    var items = yoloWrapper.Detect(ms.ToArray());
                    yoloItemBindingSource.DataSource = items;
                    AddDetailsToPictureBox(pic, items.ToList());
                }
            }
        }

        private void AddDetailsToPictureBox(PictureBox pic, List<YoloItem> items)
        {
            var img = pic.Image;
            var graphics = Graphics.FromImage(img);

            var font = new Font("Arial", 30, FontStyle.Regular);
            var brush = new SolidBrush(Color.Black);

            foreach (var item in items)
            {
                var x = item.X;
                var y = item.Y;
                var width = item.Width;
                var height = item.Height;

                var rect = new Rectangle(x, y, width, height);
                var pen = new Pen(Color.LightCoral, 3);

                var point = new System.Drawing.Point(x, y);

                graphics.DrawRectangle(pen, rect);
                graphics.DrawString(item.Type, font, brush, point);
            }
            pic.Image = img;
        }

        private void gaussianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bitmapImage = new Bitmap(pic.Image);
            GaussianBlur filter = new GaussianBlur(4, 11);
            filter.ApplyInPlace(bitmapImage);
            pic.Image = bitmapImage;
        }

        private void noiseFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bitmapImage = new Bitmap(pic.Image);
            GaussianSharpen filter = new GaussianSharpen(4, 11);
            filter.ApplyInPlace(bitmapImage);
            pic.Image = bitmapImage;
        }

        private void grayscaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bitmapImage = new Bitmap(pic.Image);
            Grayscale filter = new Grayscale(0.2125, 0.7154, 0.0721);
            pic.Image = AForge.Imaging.Image.Clone(filter.Apply(bitmapImage), PixelFormat.Format32bppArgb);
        }
    }
}

/*
aeroplane
bicycle
bird
boat
bottle
bus
car
cat
chair
cow
diningtable
dog
horse
motorbike
person
pottedplant
sheep
sofa
train
tvmonitor
 */
// https://github.com/AlturosDestinations/Alturos.Yolo