using System;
using System.Collections.Generic;

namespace Lab9Self
{
    public static class CollectionUtils
    {
        public static IEnumerable<TResult> Map<TSource, TResult>(
            IEnumerable<TSource> collection,
            Func<TSource, TResult> mapper)
        {
            var result = new List<TResult>();
            foreach (var item in collection)
            {
                result.Add(mapper(item));
            }
            return result;
        }

        public static T MaxBy<T, TKey>(
            IEnumerable<T> collection,
            Func<T, TKey> keySelector)
            where TKey : IComparable<TKey>
        {
            T? maxItem = default;
            TKey? maxKey = default;
            bool first = true;

            foreach (var item in collection)
            {
                TKey key = keySelector(item);
                if (first || (maxKey != null && key.CompareTo(maxKey) > 0))
                {
                    maxKey = key;
                    maxItem = item;
                    first = false;
                }
            }

            if (first || maxItem == null)
            {
                throw new InvalidOperationException("Колекція порожня або не містить елементів.");
            }

            return maxItem;
        }

        public static IEnumerable<T> Filter<T>(
            IEnumerable<T> collection,
            Predicate<T> predicate)
        {
            var result = new List<T>();
            foreach (var item in collection)
            {
                if (predicate(item))
                {
                    result.Add(item);
                }
            }
            return result;
        }

        public static void ForEach<T>(
            IEnumerable<T> collection,
            Action<T> action)
        {
            foreach (var item in collection)
            {
                action(item);
            }
        }
    }

    public class Product
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;


            Console.WriteLine("1. Робота з колекцією об'єктів (List<Product>) ");
            var products = new List<Product>
            {
                new Product { Name = "Laptop", Price = 25000 },
                new Product { Name = "Mouse", Price = 500 },
                new Product { Name = "Keyboard", Price = 1500 },
                new Product { Name = "Monitor", Price = 8000 }
            };

            var names = CollectionUtils.Map(products, p => p.Name);
            Console.WriteLine("Назви товарів: " + string.Join(", ", names));

            var mostExpensive = CollectionUtils.MaxBy(products, p => p.Price);
            Console.WriteLine($"Найдорожчий товар: {mostExpensive.Name} - {mostExpensive.Price} грн");

            var expensive = CollectionUtils.Filter(products, p => p.Price > 1000);
            Console.WriteLine("Товари дорожчі за 1000 грн:");
            CollectionUtils.ForEach(expensive, p => Console.WriteLine($" - {p.Name}: {p.Price} грн"));


            Console.WriteLine("\n2. Робота з колекцією чисел (List<int>) ");
            var numbers = new List<int> { 3, 12, 8, 25, 4, 19, 2 };

            var squares = CollectionUtils.Map(numbers, n => n * n);
            Console.WriteLine("Квадрати чисел: " + string.Join(", ", squares));

            var maxNum = CollectionUtils.MaxBy(numbers, n => n);
            Console.WriteLine($"Максимальне число: {maxNum}");


            var evenNumbers = CollectionUtils.Filter(numbers, n => n % 2 == 0);
            Console.Write("Парні числа: ");
            CollectionUtils.ForEach(evenNumbers, n => Console.Write($"{n} "));
            Console.WriteLine();


            Console.WriteLine("\n3. Робота з колекцією рядків (List<string>) ");
            var words = new List<string> { "Apple", "Banana", "Kiwi", "Watermelon", "Orange" };

            var lengths = CollectionUtils.Map(words, w => w.Length);
            Console.WriteLine("Довжини слів: " + string.Join(", ", lengths));

            var longestWord = CollectionUtils.MaxBy(words, w => w.Length);
            Console.WriteLine($"Найдовше слово: {longestWord}");

            var longWords = CollectionUtils.Filter(words, w => w.Length > 5);
            Console.WriteLine("Слова довші за 5 символів:");
            CollectionUtils.ForEach(longWords, w => Console.WriteLine($" * {w}"));
        }
    }
}