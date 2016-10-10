using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Uniars.Shared.Foundation
{
    public static class ObjectCloner
    {
        public static T Clone<T>(this T instance)
        {
            var serializedObject = JsonConvert.SerializeObject(instance);
            return JsonConvert.DeserializeObject<T>(serializedObject);
        }
    }
}
