using MessagingToolkit.QRCode.Codec;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WebCamHost.Helpers;
using WebCamHost.Services.TCPServer;
using Application = System.Windows.Application;
using Color = System.Windows.Media.Color;

namespace WebCamHost
{
    /// <summary>
    /// Interaction logic for QRCodePageView.xaml
    /// </summary>
    public partial class QRCodePageView : Window
    {
        public QRCodePageView(string data)
        {
            InitializeComponent();

            Title = "SERVER STARTED";

            QRCodeEncoder encoder = new QRCodeEncoder(); //создаем объект класса QRCodeEncoder
            Bitmap qrcode = encoder.Encode(data); // кодируем слово, полученное из TextBox'a (qrtext) в переменную qrcode. класса Bitmap(класс, который используется для работы с изображениями)

            var nqrcode = new Bitmap(qrcode.Size.Width + 15, qrcode.Height + 15);
            using (Graphics g = Graphics.FromImage(nqrcode))
            {
                //// set background color
                g.Clear(System.Drawing.Color.White);

                // go through each image and draw it on the final image
                //foreach (Bitmap image in images)
                {
                    g.DrawImage(qrcode, 7, 7, qrcode.Width, qrcode.Height);
                }
            }
            image.ImageSource = BitmapConverter.BitmapToBitmapImage( nqrcode );
            HostPort.Content = $"Host:port=>\n{data}";
        }
    }
}
