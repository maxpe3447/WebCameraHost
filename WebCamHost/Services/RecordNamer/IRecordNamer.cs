using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCamHost.Services.RecordNamer
{
    public interface IRecordNamer
    {
        string GetFileName();
        string GetVideoDir();
    }
}
