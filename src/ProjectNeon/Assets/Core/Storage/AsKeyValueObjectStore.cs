    using System;

    public sealed class AsObjectStore<T> : IKeyValueStore<object>
    {
        private readonly IKeyValueStore<T> _inner;

        public AsObjectStore(IKeyValueStore<T> inner) => _inner = inner;

        public object GetOrDefault(string key, Func<object> getDefaultValue) => _inner.GetOrDefault(key, () => (T)getDefaultValue());
        public void Remove(string key) => _inner.Remove(key);
        public void Clear() => _inner.Clear();
        public void Put(string key, object obj)
        {
            if (obj.GetType() != typeof(T))
                throw new InvalidOperationException($"Cannot store an object of type {obj.GetType().Name} in a Store of {typeof(T).Name}");
            _inner.Put(key, (T)obj);
        }
    }
