using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCamHost.Models;

namespace WebCamHost.Helpers
{
    public static class ActionDataModelConvert
    {
        public static byte[] ToBytes(ActionDataModel actionDataModel)
        {
            var jsonData = JsonConvert.SerializeObject(actionDataModel);

            byte[] jsonDataBytes = Encoding.ASCII.GetBytes(jsonData);

            return jsonDataBytes;
        }

        public static ActionDataModel ToModel(byte[] bytes)
        {
            var str = Encoding.ASCII.GetString(bytes, 0, bytes.Length);

            var actionData = JsonConvert.DeserializeObject<ActionDataModel>(str) ?? new ActionDataModel();

            return actionData;
        }
    }
}
