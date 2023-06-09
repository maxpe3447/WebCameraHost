using Emgu.CV.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WebCamHost.Helpers;
using WebCamHost.Models;
using WebCamHost.Services.ClientsEndPoints;
using System.Windows;
using Application = System.Windows.Application;
using System.Net.NetworkInformation;
using System.Net.Http;
using static Emgu.CV.ML.KNearest;
using Newtonsoft.Json;
using WebCamHost.Services.IPEndPointFinder;
using WebCamHost.Services.VMCommandManager;
using ImageMagick;

namespace WebCamHost.Services.UDPServer
{
    public class UDPServer : IUDPServer
    {
        private MainWindowViewModel? _mainModel;
        private UdpClient _udpClient = new();
        private UdpClient _udpListener;
        private bool _isWork = true;
        private IPAddress? _host;
        private int? _port;
        private QRCodePageView? _qrcodePageView;
        private bool _isFirstConnection = true;
        private readonly IIPEndPointFinder _iPEndPointFinder;
        private readonly IVMCommandManager _vMCommandManager;
        public UDPServer(IIPEndPointFinder iPEndPointFinder,
                         IVMCommandManager vMCommandManager)
        {
            _iPEndPointFinder = iPEndPointFinder;
            _vMCommandManager = vMCommandManager;
        }
        private void SendImage()
        {
            try
            {
                while (_isWork)
                {
                    byte[] data = BitmapConverter.BitmapImageToBytes((BitmapImage)_mainModel.MainImg);

                    int quality = (data.Length > 45000)? 40: 60;
                    using (MagickImage image = new MagickImage(data))
                    {
                        int width = (int)((data.Length > 45000) ? image.Width * 0.7 : image.Width);
                        int height = (int)((data.Length > 45000) ? image.Height * 0.7 : image.Height);

                        image.Format = image.Format; // Get or Set the format of the image.
                        image.Resize(width, height);
                        image.Quality = quality; // This is the Compression level.
                        data = image.ToByteArray();
                    }
                    for (int i = 0; i < ClientEndPointList.Hosts.Count; i++)
                    {
                        _udpClient.Send(data, data.Length,ClientEndPointList.Hosts[i], ClientEndPointList.Ports[i]);
                    }
                    Thread.Sleep(30);
                }
            }catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private async void Reciving()
        {
            try
            {
                while (_isWork)
                {
                    try
                    {
                        var data = (await _udpListener.ReceiveAsync()).Buffer;
                        var actionData = ActionDataModelConvert.ToModel(data);
                        ActionProcessing(actionData);
                    }
                    catch (SocketException ex)
                    {
                        if (!ex.Message.Contains("WSACancelBlockingCall"))
                            throw ex;
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public Task Start(MainWindowViewModel mainModel)
        {
            _mainModel = mainModel;
            _isWork = true;
            GenerationQrCode();
            if(_udpListener == null)
                _udpListener = new UdpClient(_iPEndPointFinder.GetPort());

            Task.Factory.StartNew(Reciving);
            Task.Factory.StartNew(SendImage);

            return Task.CompletedTask;
        }

        public void Stop()
        {
            _isWork = false;
        }
        private async void GenerationQrCode()
        {
            try
            {
                if (_host == null || _port == null)
                {
                    _host = IPAddress.Parse(_iPEndPointFinder.GetHost());
                    _port = _iPEndPointFinder.GetPort();
                }
                string resultData = _host.ToString() + ":" + _port?.ToString();
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


        //private async Task<string> GetClientData()
        //{
        //    byte[] data = (await _udpListener.ReceiveAsync()).Buffer;
        //    return Encoding.ASCII.GetString(data, 0, data.Length);
        //}
        private void ActionProcessing(ActionDataModel actionDataModel)
        {
            switch (actionDataModel.ActionType)
            {
                case Enums.ActionType.NONE:
                    break;
                case Enums.ActionType.REGISTRATION:
                    RegistrationAction(actionDataModel);
                    break;
                case Enums.ActionType.START_RECORD:
                    _vMCommandManager.StartRecord();
                    break;
                case Enums.ActionType.STOP_RECORD:
                    _vMCommandManager.StopRecord();
                    break;
                case Enums.ActionType.SEARCH_PEOPLE:
                    _vMCommandManager.SearchPeople();
                    break;
                case Enums.ActionType.START_SHOW:
                    _vMCommandManager.StartShow();
                    break;
                case Enums.ActionType.STOP_SHOW:
                    _vMCommandManager.StopShow();
                    break;
                case Enums.ActionType.DISCONNECT:
                    DisconnectClient(actionDataModel);
                    break;
                case Enums.ActionType.CAMERA_CHANGE:
                    CameraChange(actionDataModel);
                    break;
                default:
                    break;
            }
        }
        private void RegistrationAction(ActionDataModel actionDataModel)
        {
            SendCamerasList(actionDataModel);

            //Thread.Sleep(100);
            if (_qrcodePageView?.Dispatcher.CheckAccess() ?? false)
            {
                _qrcodePageView.Close();
            }
            else
            {
                _qrcodePageView?.Dispatcher.Invoke(new ThreadStart(_qrcodePageView.Close));
            }


            ClientEndPointList.Ports.Add(actionDataModel.Port);
            ClientEndPointList.Hosts.Add(actionDataModel.Host);
        }
        private void DisconnectClient(ActionDataModel actionDataModel)
        {
            ClientEndPointList.Ports.Remove(actionDataModel.Port);
            ClientEndPointList.Hosts.Remove(actionDataModel.Host);
        }
        private void SendCamerasList(ActionDataModel actionDataModel)
        {
            var data = new ActionDataModel()
            {
                ActionType = Enums.ActionType.GET_CAMERAS,
                Camers = _mainModel.WebCamers.Select(x => x.Name).ToList()
            };
            var sendingData = ActionDataModelConvert.ToBytes(data);

            _udpClient.Send(sendingData, sendingData.Length, actionDataModel.Host, actionDataModel.Port);
        }
        private void CameraChange(ActionDataModel actionDataModel)
        {
            var name = actionDataModel.CameraName;
            _mainModel.ChangeCameraByName(name);
        }
    }
}
