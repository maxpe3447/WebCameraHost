using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WebCamHost.Services.Record.AviRecorder;

namespace WebCamHost.Services.Record
{
    public interface IRecorder
    {
        void Start(int h, int w, GetScreen getScreen);
        void StopRecording();
    }
}
