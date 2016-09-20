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
        /// <param name="filePath">Path to config file</param>
        /// <returns>Instance of config model</returns>
        public static IConfigModel Load<T>(string filePath) where T : IConfigModel
        {
            string jsonString = File.ReadAllText(filePath);

            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        /// <summary>
        /// Write config to file
        /// </summary>
        /// <param name="filePath">Path to config file</param>
        /// <param name="model">Instance of config model</param>
        public static void Write(string filePath, IConfigModel model)
        {
            File.WriteAllText(filePath, JsonConvert.SerializeObject(model, Formatting.Indented));
        }
    }
}
