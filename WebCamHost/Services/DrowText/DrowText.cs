using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WebCamHost.Helpers;
using static System.Net.Mime.MediaTypeNames;

namespace WebCamHost.Services.DrowText
{
    internal class DrowText : IDrowText
    {
        public Bitmap RenderRecordText(Bitmap image)
        {
            var graphix = Graphics.FromImage(image);

            string text = "Recording";
            var br = new SolidBrush(System.Drawing.Color.Red);
            //graphix.DrawEllipse(new System.Drawing.Pen(br),10, 40, 10, 10);
            graphix.FillEllipse(br, 10, 30, 20, 20);
            graphix.DrawString(text, new Font("Consolas", 20, FontStyle.Bold), br, 30, 20);

            return image;
        }

        public BitmapImage RenderStopText(BitmapImage image)
        {
            var bm = BitmapConverter.BitmapImageToBitmap(image);

            var graphix = Graphics.FromImage(bm);

            graphix.FillRectangle(new SolidBrush(System.Drawing.Color.LightGray), 0, bm.Size.Height / 4, bm.Size.Width, bm.Size.Height / 4 * 2);

            string text = "STOPING VIDEO";
            var br = new SolidBrush(System.Drawing.Color.Orange);
            graphix.DrawString(text, new Font("Consolas", bm.Size.Width/(text.Length*2), FontStyle.Bold), br, bm.Size.Width/4, bm.Size.Height/2);

            return BitmapConverter.BitmapToBitmapImage(bm);
        }
    }
}
