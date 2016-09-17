using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Uniars.Shared.Foundation
{
    public class Helper
    {
        public static string GetRandomAlphaNumeric()
        {
            return Path.GetRandomFileName().Replace(".", "").Substring(0, 8);
        }
    }
}
