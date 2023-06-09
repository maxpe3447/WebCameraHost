using Prism.Ioc;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WebCamHost.Services.ClientsEndPoints;
using WebCamHost.Services.DrowText;
using WebCamHost.Services.IPEndPointFinder;
using WebCamHost.Services.PeopleSearcher;
using WebCamHost.Services.Record;
using WebCamHost.Services.RecordNamer;
using WebCamHost.Services.ShowRecords;
using WebCamHost.Services.TCPServer;
using WebCamHost.Services.UDPServer;
using WebCamHost.Services.VMCommandManager;
using WebCamHost.Services.WebCameraManager;

namespace WebCamHost
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Services
            containerRegistry.Register<IVMCommandManager, VMCommandManager>();
            containerRegistry.Register<IDrowText, DrowText>();
            containerRegistry.Register<IWebCamManager, WebCamManager>();
            containerRegistry.Register<IPeopleSearcher, PeopleSearcher>();
            containerRegistry.Register<ITCPServer, TCPServer>();
            containerRegistry.Register<IClientsEndPoints, ClientsEndPoints>();
            containerRegistry.Register<IIPEndPointFinder, IPEndPointFinder>();
            containerRegistry.Register<IUDPServer, UDPServer>();
            containerRegistry.Register<IRecordNamer, RecordNamer>();
            containerRegistry.Register<IRecordNamer, RecordNamer>();
            containerRegistry.Register<IRecorder, Recorder>();
            containerRegistry.Register<IShowRecords, ShowRecords>();

        }
        protected override Window CreateShell()
        {
            var w = Container.Resolve<MainWindow>();
            return w;
        }
    }
}
