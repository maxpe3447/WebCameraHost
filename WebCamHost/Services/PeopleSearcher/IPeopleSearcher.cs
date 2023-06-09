using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCamHost.Services.PeopleSearcher
{
    public interface IPeopleSearcher
    {
        Bitmap SearchPeople(Bitmap frame);
    }
}
