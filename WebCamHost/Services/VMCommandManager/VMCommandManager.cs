using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCamHost.Services.VMCommandManager
{
    class VMCommandManager : IVMCommandManager
    {
        private static MainWindowViewModel _mainWindowViewModel;
        public static void Init(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
        }

        public void SearchPeople()
        {
            _mainWindowViewModel.PeopleSearchCommand.Execute(null);
        }

        public void StartRecord()
        {
            _mainWindowViewModel.StartRecordCommand.Execute(null);
        }

        public void StartShow()
        {
            _mainWindowViewModel.StartShowCommand.Execute(null);
        }

        public void StopRecord()
        {
            _mainWindowViewModel.StopRecordCommand.Execute(null);
        }

        public void StopShow()
        {
            _mainWindowViewModel.StopShowCommand.Execute(null); 
        }
    }
}
