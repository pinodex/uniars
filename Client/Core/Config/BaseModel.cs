using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uniars.Shared.Foundation.Config;

namespace Uniars.Client.Core.Config
{
    public class BaseModel : IConfigModel
    {
        public string ServerAddress { get; set; }
    }
}
