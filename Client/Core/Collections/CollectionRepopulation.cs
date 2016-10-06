using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Collections;

namespace Uniars.Client.Core.Collections
{
    public static class CollectionRepopulation
    {
        public static void Repopulate<T>(this ICollection<T> collection, ICollection<T> newCollection)
        {
            collection.Clear();

            foreach (T element in newCollection)
            {
                collection.Add(element);
            }
        }
    }
}
