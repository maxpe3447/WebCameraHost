using System;

using Emgu.CV;
using Emgu.CV.Util;
using Emgu.CV.Structure;
using Emgu.Util;
using AForge.Video.DirectShow;
using System.Windows.Media;
using Emgu.CV.CvEnum;
using System.Drawing;
using WebCamHost.Helpers;
using System.Windows;
using System.Linq;
using System.Windows.Documents;
using WebCamHost.Models;
using System.Collections.Generic;
using Emgu.CV.Reg;
using System.Windows.Forms;
using AForge.Video;
using System.Threading.Tasks;

namespace WebCamHost.Services.WebCameraManager
{
    public class WebCamManager : IWebCamManager
    {
        private FilterInfoCollection _webCams;
        private VideoCaptureDevice _webcam;
        private int _selectedCamera = -1;

        public List<WebCameraModel> GetWebCamList()
        {

            _webCams = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            var lst = new List<WebCameraModel>();
            int i = 0;
            foreach (var item in _webCams)
            {
                if(item is FilterInfo cam)
                {
                    VideoCaptureDevice dev = new VideoCaptureDevice(cam.MonikerString);
                    lst.Add(new WebCameraModel()
                    {
                        Name = cam.Name,
                        Description = $"{dev.VideoCapabilities[i++].FrameSize.Width}x" +
                                      $"{dev.VideoCapabilities[i++].FrameSize.Height}",
                        Opacity = 0.5
                    });
                }
            }
            return lst;
        }

        public void SetSelectCamera(int index)
        {
            _selectedCamera = index;
        }
        private CaptureHandler _captureHandler;
        public void StartShow(CaptureHandler captureHandler)
        {
            _captureHandler = captureHandler;
            if (_webCams.Count == 0)
            {
                throw new Exception("No cameras available");
            }
            if (_selectedCamera == -1)
            {
                throw new Exception("You need to select the web camera");
            }

            _webcam = new VideoCaptureDevice(_webCams[_selectedCamera].MonikerString);
            
            _webcam.NewFrame += Capturing ;
            _webcam.Start();

        }

        private void Capturing(object sender, NewFrameEventArgs eventArgs)
        {
            _captureHandler.Invoke(sender, eventArgs);
        }

        public delegate void CaptureHandler(object sender, NewFrameEventArgs eventArgs);

        public void StopShow() => _webcam?.SignalToStop();
    }
}
