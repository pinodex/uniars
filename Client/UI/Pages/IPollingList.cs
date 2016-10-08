using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniars.Client.UI.Pages
{
    interface IPollingList
    {
        void LoadList(bool autoTriggered = false);
    }
}
