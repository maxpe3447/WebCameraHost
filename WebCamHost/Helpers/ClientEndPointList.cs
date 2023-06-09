using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCamHost.Helpers
{
    public static  class ClientEndPointList
    {
        public static List<string> Hosts { get; } = new List<string>();
        public static List<int> Ports { get; } = new List<int>();
    }
}
