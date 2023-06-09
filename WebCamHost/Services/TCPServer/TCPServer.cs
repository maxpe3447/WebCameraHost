using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebCamHost.Helpers;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Net.NetworkInformation;
using WebCamHost.Models;
using WebCamHost.Services.ClientsEndPoints;
using System.Windows.Media.Animation;

namespace WebCamHost.Services.TCPServer
{
    public class TCPServer : ITCPServer, IDisposable
    {
        private static TcpListener tcpListener; 

        private List<ClientObject>? _clients = new(); 
        private QRCodePageView? _qrcodePageView;
        private IPAddress? _host;
        private PortInfoModel? _port;

        private bool _isLitening = false;
        private bool _isStarted = false;

        public void Listen()
        {
            try
            {
                while (_isLitening)
                {
                    TcpClient tcpClient;
                    try
                    {
                        tcpClient = tcpListener.AcceptTcpClient();
                    }
                    catch (SocketException ex)
                    {
                        if (!ex.Message.Contains("WSACancelBlockingCall"))
                            throw ex;
                        continue;
                    }

                    if (_qrcodePageView?.Dispatcher.CheckAccess() ?? false)
                    {
                        _qrcodePageView.Close();
                    }
                    else
                    {
                        _qrcodePageView?.Dispatcher.Invoke(new ThreadStart(_qrcodePageView.Close));
                    }
                    ClientEndPointList.Ports.Add(GetClientPort(tcpClient.GetStream()));
                    ClientEndPointList.Hosts.Add(GetClientIP(tcpClient.GetStream()));

                    tcpClient.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void Disconnect()
        {
            for (int i = 0; i < _clients?.Count; i++)
            {
                _clients[i].Close();
            }
        }

        public async Task Start(MainWindowViewModel bitmapImage)
        {
            GenerationQrCode();
            _isLitening = true;
            if (_isStarted)
            {
                return;
            }
            _isStarted = true;
            
            await Task.Factory.StartNew(() => Listen());
        }

        public void Stop()
        {
            Disconnect();
        }

        public int ClientCount() => _clients?.Count()?? 0;

        private List<PortInfoModel> GetOpenPort()
        {
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] tcpEndPoints = properties.GetActiveTcpListeners();
            TcpConnectionInformation[] tcpConnections = properties.GetActiveTcpConnections();

            return tcpConnections.Select(p =>
            {
                return new PortInfoModel(
                    i: p.LocalEndPoint.Port);
            }).ToList();
        }

        private string GetClientIP(NetworkStream Stream)
        {
            byte[] data = new byte[64];
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.ASCII.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            return builder.ToString().Remove(builder.Length);
        }
        private int GetClientPort(NetworkStream Stream)
        {
            byte[] reciveLen = new byte[4];
            StringBuilder builder = new StringBuilder();
            int lenght = 0;
            int bytes = 0;

            bytes = Stream.Read(reciveLen, 0, 4);

            lenght = BitConverter.ToInt32(reciveLen, 0);

            return lenght;
        }

        private async void GenerationQrCode()
        {
            try
            {
                if (_host == null || _port == null)
                {
                    var host = Dns.GetHostEntry(Dns.GetHostName());
                    _host = host.AddressList.Where(x => x.AddressFamily == AddressFamily.InterNetwork)
                                            .ToList()
                                            .Where(x => int.Parse(x.ToString().Split('.').Last()) != 1).LastOrDefault();
                    if(host == null)
                    {
                        throw new Exception("Hosr is not founded");
                    }
                    //MessageBox.Show(String.Join("\n", host.AddressList.Where(x => x.AddressFamily == AddressFamily.InterNetwork).ToList()));
                    foreach (var item in GetOpenPort())
                    {
                        _port = item;
                        try
                        {
                            tcpListener = new TcpListener(_host, _port.PortNumber);

                            tcpListener.Start();
                            break;
                        }
                        catch { }

                    }
                }
                string resultData = _host.ToString() + ":" + _port?.PortNumber.ToString();
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    _qrcodePageView = new QRCodePageView(resultData);
                    _qrcodePageView.Show();
                });
            }
            catch (Exception ex)
            {
                await Task.Run(() => MessageBox.Show(ex.Message));
            }
        }

        public void Dispose()
        {
            _isLitening = false;
            Disconnect();
            tcpListener?.Stop(); //остановка сервера
        }
    }
}
