using System;
using System.Collections.Generic;
using System.Windows;

namespace TvDbSearchControl
{
    public static class CollectionHelper
    {
        public static void AddOnUi<T>(this ICollection<T> collection, T item)
        {
            Action<T> addMethod = collection.Add;
            Application.Current.Dispatcher.BeginInvoke(addMethod, item);
        }
    }
}