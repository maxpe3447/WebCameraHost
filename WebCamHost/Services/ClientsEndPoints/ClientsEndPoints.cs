using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCamHost.Services.ClientsEndPoints
{
    public class ClientsEndPoints : IClientsEndPoints
    {
        private List<string> _hosts;
        private List<int> _ports;
        public List<string> Hosts 
        { 
            get=>_hosts ??= new(); 
        }
        public List<int> Ports 
        {
            get => _ports ??= new(); 
        }
    }
}
