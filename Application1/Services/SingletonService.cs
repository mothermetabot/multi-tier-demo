using System;
using System.Collections.Generic;

namespace Application1.Services
{
    public static class SingletonService
    {
        private static readonly Dictionary<Type, object> _map = new Dictionary<Type, object>();
        public static bool Register<Tabstract, Tinstance>(Tinstance obj) where Tinstance : Tabstract
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            return _map.TryAdd(typeof(Tabstract), obj);
        }

        public static T Retrieve<T>()
        {
            return (T)_map[typeof(T)];
        }

        public static void Clear()
        {
            _map.Clear();
        }
    }
}
