using System;
using System.Collections.Generic;

namespace IndependentWork7
{
    public class Cache<TKey, TValue> where TKey : notnull
    {
        private readonly int _capacity;
        private readonly Dictionary<TKey, TValue> _cache;
        private readonly Queue<TKey> _keysOrder;

        public int Count => _cache.Count;

        public Cache(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentException("Ємність кешу повинна бути більше 0.", nameof(capacity));

            _capacity = capacity;
            _cache = new Dictionary<TKey, TValue>();
            _keysOrder = new Queue<TKey>();
        }

        public void Add(TKey key, TValue value)
        {
            if (_cache.ContainsKey(key))
            {
                _cache[key] = value;
                return;
            }

            if (_cache.Count >= _capacity)
            {
                TKey oldestKey = _keysOrder.Dequeue();
                _cache.Remove(oldestKey);
            }

            _cache[key] = value;
            _keysOrder.Enqueue(key);
        }

        public TValue Get(TKey key)
        {
            if (!_cache.TryGetValue(key, out TValue? value))
            {
                throw new KeyNotFoundException($"Ключ '{key}' не знайдено в кеші.");
            }

            return value;
        }

        public bool ContainsKey(TKey key)
        {
            return _cache.ContainsKey(key);
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine(" Демонстрація роботи FIFO Cache (ємність = 3)");
            var cache = new Cache<string, string>(3);

            cache.Add("K1", "Item 1");
            cache.Add("K2", "Item 2");
            cache.Add("K3", "Item 3");
            Console.WriteLine($"Додано K1, K2, K3. Поточна кількість: {cache.Count}");

            Console.WriteLine($"K1 є у кеші: {cache.ContainsKey("K1")} (Значення: {cache.Get("K1")})");

            Console.WriteLine("Додаємо K4 (перевищення ємності)...");
            cache.Add("K4", "Item 4");

            Console.WriteLine($"Поточна кількість: {cache.Count}");
            Console.WriteLine($"K1 є у кеші: {cache.ContainsKey("K1")} (має бути False, бо витіснено K1 за FIFO)");
            Console.WriteLine($"K4 є у кеші: {cache.ContainsKey("K4")} (Значення: {cache.Get("K4")})");
        }
    }
}