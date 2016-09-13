using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uniars.Shared.Foundation.Config;

namespace Uniars.Server.Core.Config
{
    public class ServerModel : IConfigModel
    {
        public string IPAddress { get; set; }

        public uint Port { get; set; }
    }
}
