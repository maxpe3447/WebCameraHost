using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WebCamHost.Services.TCPServer
{
    public interface ITCPServer: IDisposable
    {
        Task Start(MainWindowViewModel bitmapImage);
        void Stop();
        int ClientCount();
    }
}
