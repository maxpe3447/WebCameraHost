using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCamHost.Services.RecordNamer
{
    public class RecordNamer : IRecordNamer
    {
        private readonly string _dirPath;
        public RecordNamer()
        {
            _dirPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), $"{nameof(WebCamHost)}");
            if (!Directory.Exists(_dirPath)) Directory.CreateDirectory(_dirPath);
        }
        public string GetFileName()
            => Path.Combine(_dirPath, $"{DateTime.Now.ToString("dd.MM.yyyy hh_mm_ss")}.mp4");

        public string GetVideoDir() => _dirPath;
    }
}
