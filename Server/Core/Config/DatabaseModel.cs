using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uniars.Shared.Foundation.Config;

namespace Uniars.Server.Core.Config
{
    public class DatabaseModel : IConfigModel
    {
        public string Host { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }
    }
}
