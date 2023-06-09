using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCamHost.Services.ClientsEndPoints
{
    public interface IClientsEndPoints
    {
        List<string> Hosts { get; }
        List<int> Ports { get; }
    }
}
