using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebCamHost.Services.RecordNamer;

namespace WebCamHost.Services.ShowRecords
{
    public class ShowRecords : IShowRecords
    {
        private readonly IRecordNamer _recordNamer;

        public ShowRecords(IRecordNamer recordNamer)
        {
            _recordNamer = recordNamer;
        }
        public void Show()
        {
            Process.Start("explorer.exe", _recordNamer.GetVideoDir());
        }
    }
}
