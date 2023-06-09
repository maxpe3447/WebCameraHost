using System.Collections.Generic;
using System.Security.Policy;
using WebCamHost.Enums;

namespace WebCamHost.Models
{
    public class ActionDataModel
    {
        public ActionType ActionType { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public List<string> Camers { get; set; }
        public string CameraName { get; set; }
    }
}
