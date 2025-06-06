﻿// Copyright (c) Mixed Reality Toolkit Contributors
// Licensed under the BSD 3-Clause

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MixedReality.Toolkit
{
    /// <summary>
    /// Extension methods to simplify working with IEnumerable collections.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Disposes of all non-null elements in a collection.
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        /// <param name="items">The collection of items to be disposed.</param>
        public static void DisposeElements<T>(this IEnumerable<T> items)
            where T : IDisposable
        {
#if OPTIMISATION_LISTPOOL
            using var _0 = UnityEngine.Pool.ListPool<T>.Get(out var list);
            list.AddRange(items);
            int count = list.Count;

            for (int i = 0; i < count; i++)
            {
                list[i]?.Dispose();
            }
#else
            T[] array = items.ToArray();
            int count = array.Length;

            for (int i = 0; i < count; i++)
            {
                array[i]?.Dispose();
            }
#endif // OPTIMISATION_LISTPOOL
        }

        /// <summary>
        /// Returns the max item based on the provided comparer or the default value when the list is empty
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        /// <param name="items">Collection of items being evaluated.</param>
        /// <param name="comparer">The comparer used to determine the correct item to return.</param>
        /// <returns>Max or default value of T.</returns>
        public static T MaxOrDefault<T>(
            this IEnumerable<T> items,
            IComparer<T> comparer = null)
        {
            if (items == null) { throw new ArgumentNullException(nameof(items)); }
            comparer ??= Comparer<T>.Default;

            using (var enumerator = items.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    return default(T);
                }

                var max = enumerator.Current;
                while (enumerator.MoveNext())
                {
                    if (comparer.Compare(max, enumerator.Current) < 0)
                    {
                        max = enumerator.Current;
                    }
                }

                return max;
            }
        }

        /// <summary>
        /// Creates a read-only copy of an existing collection.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="items">The collection to be copied.</param>
        /// <returns>The new, read-only copy of <paramref name="items"/>.</returns>
        public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> items)
        {
#if OPTIMISATION_LISTPOOL
            using var _0 = UnityEngine.Pool.ListPool<T>.Get(out var list);
            list.AddRange(items);
            return Array.AsReadOnly<T>(list.ToArray());
#else
            return Array.AsReadOnly<T>(items.ToArray());
#endif // OPTIMISATION_LISTPOOL
        }
    }
}
