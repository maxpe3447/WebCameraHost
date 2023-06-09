using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.ImgHash;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;

namespace WebCamHost.Services.PeopleSearcher
{
    internal class PeopleSearcher : IPeopleSearcher
    {
        public Bitmap SearchPeople(Bitmap frame)
        {
            MCvObjectDetection[] regions;
            var image = frame.ToImage<Bgr, byte>();
            using(HOGDescriptor descriptor = new HOGDescriptor())
            {
                descriptor.SetSVMDetector(HOGDescriptor.GetDefaultPeopleDetector());

                regions = descriptor.DetectMultiScale(image);
            }
            foreach (MCvObjectDetection obj in regions)
            {
                image.Draw(obj.Rect, new Bgr(Color.FromArgb(206, 90, 87)), 3);

                CvInvoke.PutText(image, "HUMAN", new Point(obj.Rect.X, obj.Rect.Y), FontFace.HersheyPlain, 1, new MCvScalar(206, 90, 87), 2);
            }
            return image.ToBitmap();

        }
    }
}
