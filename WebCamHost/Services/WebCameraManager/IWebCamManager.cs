using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using WebCamHost.Models;
using static WebCamHost.Services.WebCameraManager.WebCamManager;

namespace WebCamHost.Services.WebCameraManager
{
    public interface IWebCamManager
    {
        List<WebCameraModel> GetWebCamList();
        void SetSelectCamera(int index);
        void StartShow(CaptureHandler captureHandler);
        void StopShow();
    }
}
