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
        /// <summary>
        /// Repopulate an existing collection with new collection
        /// </summary>
        /// <typeparam name="T">Collection type</typeparam>
        /// <param name="destination">Destination</param>
        /// <param name="source">Source</param>
        /// <param name="checkSequence">Check for collection sequence matches</param>
        public static void Repopulate<T>(this ICollection<T> destination, ICollection<T> source, bool checkSequence = true)
        {
            if (checkSequence && destination.SequenceEqual(source))
            {
                return;
            }

            destination.Clear();

            foreach (T element in source)
            {
                destination.Add(element);
            }
        }
    }
}
