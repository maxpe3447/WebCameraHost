using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using WebCamHost.Models;

namespace WebCamHost.Services.IPEndPointFinder
{
    public class IPEndPointFinder : IIPEndPointFinder
    {
        private int _port = 0;
        private string _host = string.Empty;
        private TcpListener _tcpListener;
        public string GetHost()
        {
            if (_host != string.Empty)
            {
                return _host;
            }

            var host = Dns.GetHostEntry(Dns.GetHostName());
            if (host == null)
            {
                throw new Exception("Hosr is not founded");
            }
            _host = host.AddressList.Where(x => x.AddressFamily == AddressFamily.InterNetwork)
                                    .ToList()
                                    .Where(x => int.Parse(x.ToString().Split('.').Last()) != 1)
                                    .LastOrDefault()
                                    ?.ToString() ?? string.Empty;
            if(_host == string.Empty)
            {
                throw new Exception("NO HOST!");
            }

            return _host;
        }

        public int GetPort()
        {
            if (_port != 0)
            {
                return _port;
            }
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] tcpEndPoints = properties.GetActiveTcpListeners();
            TcpConnectionInformation[] tcpConnections = properties.GetActiveTcpConnections();

            var ports = tcpConnections.Select(p => p.LocalEndPoint.Port).ToList();
            var host = GetHost();
            foreach (var item in ports)
            {
                _port = item;
                try
                {
                    _tcpListener = new TcpListener(IPAddress.Parse(host), _port);

                    _tcpListener.Start();
                    _tcpListener.Stop();
                    break;
                }
                catch { }

            }

            return _port;
        }

    }
}
