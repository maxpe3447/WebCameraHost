using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WebCamHost.Services.DrowText
{
    public interface IDrowText
    {
        BitmapImage RenderStopText (BitmapImage image);
        Bitmap RenderRecordText (Bitmap image);
    }
}
