using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCamHost.Services.IPEndPointFinder
{
    public interface IIPEndPointFinder
    {
        string GetHost();
        int GetPort();
    }
}
