using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCamHost.Services.VMCommandManager
{
    public interface IVMCommandManager
    {
        //static void Init(MainWindowViewModel mainWindowViewModel);
        void StartRecord();
        void StopRecord();
        void StartShow();
        void StopShow();
        void SearchPeople();

    }
}
