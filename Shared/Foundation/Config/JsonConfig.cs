using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace Uniars.Shared.Foundation.Config
{
    public class JsonConfig
    {
        /// <summary>
        /// Load JSON config file
        /// </summary>
        /// <typeparam name="T">Config model that implements IConfigModel</typeparam>
        /// <param name="filePath">Path config file</param>
        /// <returns>Instance of config model</returns>
        public static IConfigModel Load<T>(string filePath) where T : IConfigModel
        {
            string jsonString = "{}";

            try
            {
                jsonString = File.ReadAllText(filePath);
            }
            catch {}

            return JsonConvert.DeserializeObject<T>(jsonString);
        }
    }
}
