using System;
using MessageBox = System.Windows.MessageBox;
using static WebCamHost.Services.Record.AviRecorder;
using System.IO;
using WebCamHost.Helpers;
using WebCamHost.Services.RecordNamer;

namespace WebCamHost.Services.Record
{
    public class Recorder :IRecorder
    {
        private readonly IRecordNamer _recordNamer;
        AviRecorder? _aviRecrderc;

        public Recorder(IRecordNamer recordNamer)
        {
            _recordNamer = recordNamer;
        }

        public void Start(int h, int w, GetScreen getScreen)
        {
            try
            {
                _aviRecrderc = new AviRecorder(new RecorderParams(_recordNamer.GetFileName(), 10, SharpAvi.KnownFourCCs.Codecs.MotionJpeg, 70, h, w), getScreen);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Start recorder ERROR");
            }
        }
        public void StopRecording()
        {
            try
            {
                _aviRecrderc?.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Stop recorder ERROR");
            }
        }
    }
}
