using AForge.Video;
using Emgu.CV.Flann;
using Prism.Commands;
using Prism.Modularity;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WebCamHost.Helpers;
using WebCamHost.Models;
using WebCamHost.Services.DrowText;
using WebCamHost.Services.PeopleSearcher;
using WebCamHost.Services.Record;
using WebCamHost.Services.ShowRecords;
using WebCamHost.Services.TCPServer;
using WebCamHost.Services.UDPServer;
using WebCamHost.Services.VMCommandManager;
using WebCamHost.Services.WebCameraManager;

namespace WebCamHost
{
    public class MainWindowViewModel: BindableBase
    {
        //Services
        private readonly IDrowText _drowText;
        private readonly IPeopleSearcher _peopleSearcher;
        private readonly ITCPServer _tCPServer;
        private readonly IUDPServer _uDPClient;
        private readonly IRecorder _recorder;
        private readonly IShowRecords _showRecords;
        private readonly IVMCommandManager _iVMCommandManager;

        public MainWindowViewModel(IVMCommandManager vMCommandManager,
                                   IWebCamManager camManager,
                                   IDrowText drowText,
                                   ITCPServer tCPServer,
                                   IPeopleSearcher peopleSearcher,
                                   IUDPServer uDPClient,
                                   IRecorder recorder,
                                   IShowRecords showRecords)
        {
            _iVMCommandManager = vMCommandManager;
            _tCPServer = tCPServer;
            _webCamManager = camManager;
            _drowText = drowText;
            _peopleSearcher = peopleSearcher;
            _uDPClient = uDPClient;
            _recorder = recorder;
            _showRecords = showRecords;

            VMCommandManager.Init(this);

            Title = "WebCam Host";
        }

        #region Properties
        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private ImageSource _mainImg;
        public ImageSource MainImg
        {
            get => _mainImg;
            set => SetProperty(ref _mainImg, value);
        }
        private List<WebCameraModel> _webCamers;
        public List<WebCameraModel> WebCamers
        {
            get => _webCamers ??= new List<WebCameraModel>();
            set => SetProperty(ref _webCamers, value);
        }

        private WebCameraModel _selectedCamera;
        public WebCameraModel SelectedCamera
        {
            get => _selectedCamera;
            set => SetProperty(ref _selectedCamera, value, SelectedCameraOnPropertyChange);
        }
        private SolidColorBrush _serverStartFrame;
        public SolidColorBrush ServerStartFrame
        {
            get => _serverStartFrame;
            set => SetProperty(ref _serverStartFrame, value);
        }

        private SolidColorBrush _serverStopFrame;
        public SolidColorBrush ServerStopFrame
        {
            get => _serverStopFrame;
            set => SetProperty(ref _serverStopFrame, value);
        }

        private SolidColorBrush _peopleSearchFrame;
        public SolidColorBrush PeopleSearchFrame
        {
            get => _peopleSearchFrame;
            set => SetProperty(ref _peopleSearchFrame, value);
        }

        private SolidColorBrush _cameraStartFrame;
        public SolidColorBrush CameraStartFrame
        {
            get => _cameraStartFrame;
            set => SetProperty(ref _cameraStartFrame, value);
        }

        private SolidColorBrush _cameraStopFrame;
        public SolidColorBrush CameraStopFrame
        {
            get => _cameraStopFrame;
            set => SetProperty(ref _cameraStopFrame, value);
        }

        private SolidColorBrush _recordStartFrame;
        public SolidColorBrush RecordStartFrame
        {
            get => _recordStartFrame;
            set => SetProperty(ref _recordStartFrame, value);
        }

        private SolidColorBrush _recordStopFrame;
        public SolidColorBrush RecordStopFrame
        {
            get => _recordStopFrame;
            set => SetProperty(ref _recordStopFrame, value);
        }
        #endregion

        #region Commands
        private ICommand _loadedCommand;
        public ICommand LoadedCommand
        {
            get => _loadedCommand ??= new DelegateCommand(LoadedCommandRelease);
        }
        private ICommand _closingCommand;
        public ICommand ClosingCommand
        {
            get => _closingCommand ??= new DelegateCommand(ClosingCommandRelease);
        }
        private ICommand _startShowCommand;
        public ICommand StartShowCommand
        {
            get => _startShowCommand ??= new DelegateCommand(StartShowCommandRelease);
        }
        private ICommand _stopShowCommand;
        public ICommand StopShowCommand
        {
            get => _stopShowCommand ??= new DelegateCommand(StopShowCommandRelease);
        }

        private ICommand _peopleSearchCommand;
        public ICommand PeopleSearchCommand
        {
            get => _peopleSearchCommand ??= new DelegateCommand(PeopleSearchCommandRelease);
        }

        private ICommand _startServerCommand;
        public ICommand StartServerCommand
        {
            get => _startServerCommand ??= new DelegateCommand(StartServerCommandRelease);
        }

        private ICommand _stopServerCommand;
        public ICommand StopServerCommand
        {
            get => _stopServerCommand ??= new DelegateCommand(StopServerCommandRelease);
        }

        private ICommand _checkClientCommand;
        public ICommand CheckClientCommand
        {
            get => _checkClientCommand ??= new DelegateCommand(CheckClientCommandRelease);
        }
        
        private ICommand _startRecordCommand;
        public ICommand StartRecordCommand
        {
            get => _startRecordCommand ??= new DelegateCommand(StartRecordCommandRelease);
        }

        private ICommand _stopRecordCommand;
        public ICommand StopRecordCommand
        {
            get => _stopRecordCommand ??= new DelegateCommand(StopRecordCommandRelease);
        }

        private ICommand _showRecordsCommand;
        public ICommand ShowRecordsCommand
        {
            get => _showRecordsCommand ??= new DelegateCommand(ShowRecordCommandRelease);
        }
        #endregion

        #region Command Release
        private void LoadedCommandRelease()
        {
            WebCamers = new List<WebCameraModel>(_webCamManager.GetWebCamList());

            RecordStartFrame = PeopleSearchFrame = CameraStartFrame = ServerStartFrame = _disableBrush;
            RecordStopFrame = CameraStopFrame = ServerStopFrame = _activeBrush;

            if (!File.Exists("Asset/logo.jpg"))
            {
                MessageBox.Show("Error");
            }
            string path = Path.Combine(Environment.CurrentDirectory, "Asset/logo.jpg");
            MainImg = new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute));
        }
        private void ClosingCommandRelease()
        {
            StopShowCommandRelease();
            StopServerCommandRelease();
            var thread = new Thread(StopRecordCommandRelease)
            {
                IsBackground = true
            };
            thread.Start();
        }
        private void StartShowCommandRelease()
        {
            if (IsShowing)
            {
                return;
            }

            try
            {
                _webCamManager.StartShow(CaptureImage);
                IsShowing = true;
            }
            catch (Exception ex)
            {
                IsShowing = false;
                MessageBox.Show(ex.Message, "Start show ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void StopShowCommandRelease()
        {
            if (!IsShowing)
            {
                return;
            }
            try
            {
                StopCamera();
                MainImg = _drowText.RenderStopText((BitmapImage)MainImg.Clone());

                if (IsRecord)
                {
                    _recorder.StopRecording();
                }
                IsRecord = false;
                IsShowing = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Stop show ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void PeopleSearchCommandRelease()
        {
            IsPeopleSearch = !IsPeopleSearch;
        }

        private void CheckClientCommandRelease()
        {
            MessageBox.Show($"Clients count: {ClientEndPointList.Hosts.Count()}\n{string.Join("\n", ClientEndPointList.Hosts)}",
                             "Connected clients", 
                             MessageBoxButton.OK, 
                             MessageBoxImage.Information);
        }
        private void StartServerCommandRelease()
        {
            try
            {
                if (!IsShowing)
                {
                    return;
                }
                IsServerWork = true;
                //_tCPServer.Start(this);
                _uDPClient.Start(this);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Server Error START", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void StartRecordCommandRelease()
        {
            try
            {
                lock (Constants.Locker)
                {
                    if (_lastFrame == null)
                    {
                        throw new Exception("Image no founded!");
                    }
                    if (IsRecord)
                    {
                        return;
                    }
                    _recorder.Start(_lastFrame.Height, _lastFrame.Width, GetBitmapImage);
                    IsRecord = true;
                }
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Try again later", "Start record ERROR", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Start record ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                IsRecord = false;
                StopRecordCommandRelease();
            }
        }
        private void StopRecordCommandRelease()
        {
            try
            {
                if (!IsRecord)
                {
                    return;
                }
                _recorder?.StopRecording();
                IsRecord = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Stop record ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void StopServerCommandRelease()
        {
            try
            {
                if (!IsShowing || !IsServerWork)
                {
                    return;
                }
                IsServerWork = false;

                //_tCPServer?.Stop();
                _uDPClient?.Stop();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Stop server ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ShowRecordCommandRelease()
        {
            try
            {
                _showRecords.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Stop server ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region OnPropertyChange
        public void ChangeCameraByName(string name)
        {
            var camera = WebCamers.FirstOrDefault(x => x.Name == name);
            if(camera == null)
            {
                return;
            }

            SelectedCamera = camera;
        }

        void SelectedCameraOnPropertyChange()
        {
            StopCamera();

            var index = WebCamers.IndexOf(SelectedCamera);
            _webCamManager.SetSelectCamera(index);
            WebCamers.ForEach(x=>x.Opacity = 0.5);
            WebCamers[index].Opacity = 1;

            WebCamers = new List<WebCameraModel>(WebCamers);
            if (IsShowing)
            {
                _isShowing = false;
                StartShowCommandRelease();
            }
        }
        #endregion
        Bitmap _lastFrame;
        private void CaptureImage(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                lock (Constants.Locker)
                {
                    _lastFrame = (Bitmap)eventArgs.Frame.Clone();
                    _lastFrame.RotateFlip(RotateFlipType.Rotate180FlipY);

                    if (IsRecord)
                    {
                        _lastFrame = _drowText.RenderRecordText(_lastFrame);
                    }

                    if (IsPeopleSearch)
                    {
                        _lastFrame = _peopleSearcher.SearchPeople(_lastFrame);
                    }
                    MainImg = BitmapConverter.BitmapToBitmapImage(_lastFrame);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Capture image ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private Bitmap GetBitmapImage()
        {
            lock (Constants.Locker)
            {
                return (Bitmap)_lastFrame.Clone() ;
            }
        }
        private void StopCamera()
        {
            _webCamManager.StopShow();
            WebCamers.ForEach(x => x.Opacity = 0.5);
            WebCamers = new List<WebCameraModel>(WebCamers);
        }
        private IWebCamManager _webCamManager;
        private bool _isShowing = false;
        private bool _isPeopleSearch = false;
        private bool _isServerWork = false;
        private bool _isRecord = false;

        private SolidColorBrush _activeBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(120, 165, 163));
        private SolidColorBrush _disableBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(206, 90, 87));

        private bool IsServerWork
        {
            get => _isServerWork;
            set
            {
                _isServerWork = value;
                ServerStartFrame = (_isServerWork) ? _activeBrush : _disableBrush;
                ServerStopFrame = (!_isServerWork) ? _activeBrush : _disableBrush;
            }
        }

        private bool IsShowing
        {
            get => _isShowing;
            set
            {
                _isShowing = value;
                CameraStartFrame = (_isShowing) ? _activeBrush : _disableBrush;
                CameraStopFrame = (!_isShowing) ? _activeBrush : _disableBrush;
            }
        }

        private bool IsPeopleSearch
        {
            get => _isPeopleSearch;
            set
            {
                _isPeopleSearch = value;
                PeopleSearchFrame = (_isPeopleSearch) ? _activeBrush : _disableBrush;
            }
        }

        private bool IsRecord
        {
            get => _isRecord;
            set
            {
                _isRecord = value;
                RecordStartFrame = (_isRecord) ? _activeBrush : _disableBrush;
                RecordStopFrame= (!_isRecord) ? _activeBrush : _disableBrush;
            }
        }
    }
}
