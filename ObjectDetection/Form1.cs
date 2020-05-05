using Alturos.Yolo;
using Alturos.Yolo.Model;
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

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using(OpenFileDialog ofd = new OpenFileDialog() { Filter = "PNG|*.png|JPEG|*.jpeg"})
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    pic.Image = Image.FromFile(ofd.FileName);
                }
            }
        }

        private void btnDetect_Click(object sender, EventArgs e)
        {
            var configurationDetector = new ConfigurationDetector();
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
                //items[0].Type -> "Person, Car, ..."
                //items[0].Confidence -> 0.0 (low) -> 1.0 (high)
                //items[0].X -> bounding box
                //items[0].Y -> bounding box
                //items[0].Width -> bounding box
                //items[0].Height -> bounding box
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

                var point = new Point(x, y);

                graphics.DrawRectangle(pen, rect);
                graphics.DrawString(item.Type, font, brush, point);
            }
            pic.Image = img;
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