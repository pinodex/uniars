using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uniars.Shared.Foundation.Config;

namespace Uniars.Server.Core.Config
{
    public class BaseModel : IConfigModel
    {
        public DatabaseModel Database { get; set; }

        public ServerModel Server { get; set; }

        public string CertificateFile { get; set; }
    }
}
