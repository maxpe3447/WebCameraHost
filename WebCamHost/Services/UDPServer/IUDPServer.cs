using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WebCamHost.Services.UDPServer
{
    public interface IUDPServer
    {
        //Task SendImage();
        Task Start(MainWindowViewModel mainModel);
        void Stop();
    }
}
