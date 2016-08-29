using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Uniars.Core
{
    public class Config
    {
        protected string path;

        protected Dictionary<String, String> values;

        public Config(String path)
        {
            this.path = path;
            this.values = new Dictionary<String, String>();

            LoadConfig();
        }

        public void LoadConfig()
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(this.path);

            XmlNode xNode = xDoc.SelectSingleNode("/Config");
            RecurseNodes(xNode);
        }

        public String Get(String key, String defaultValue = null)
        {
            String value;

            values.TryGetValue(key, out value);

            if (value != null)
            {
                return value;
            }

            return defaultValue;
        }

        private void RecurseNodes(XmlNode parent)
        {
            foreach (XmlNode node in parent.ChildNodes)
            {
                foreach (XmlAttribute xAttribute in node.Attributes)
                {
                    String key = String.Format("{0}.{1}", node.Name, xAttribute.Name);

                    values.Add(key, xAttribute.Value);
                }

                if (node.HasChildNodes)
                {
                    RecurseNodes(node);
                }
            }
        }
    }
}
