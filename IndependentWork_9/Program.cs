using System;
using System.Collections.Generic;

namespace IndependentWork_9
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
            T maxItem = default!;
            TKey maxKey = default!;
            bool first = true;

            foreach (var item in collection)
            {
                TKey key = keySelector(item);
                if (first || key.CompareTo(maxKey) > 0)
                {
                    maxKey = key;
                    maxItem = item;
                    first = false;
                }
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

            Console.WriteLine("Демонстрація CollectionUtils \n");

            Console.WriteLine(" Колекція 1: List<int> ");
            var numbers = new List<int> { 5, 12, 3, 20, 8, 15 };

            var squaredNumbers = CollectionUtils.Map(numbers, n => n * n);
            Console.WriteLine("Квадрати чисел: " + string.Join(", ", squaredNumbers));

            int maxNumber = CollectionUtils.MaxBy(numbers, n => n);
            Console.WriteLine($"Максимальне число: {maxNumber}");

            var evenNumbers = CollectionUtils.Filter(numbers, n => n % 2 == 0);
            Console.Write("Парні числа: ");
            CollectionUtils.ForEach(evenNumbers, n => Console.Write($"{n} "));
            Console.WriteLine("\n");

            Console.WriteLine(" Колекція 2: List<string> ");
            var words = new List<string> { "C#", "Delegate", "Lambda", "LINQ", "Collection" };

            var wordLengths = CollectionUtils.Map(words, w => w.Length);
            Console.WriteLine("Довжини слів: " + string.Join(", ", wordLengths));

            string longestWord = CollectionUtils.MaxBy(words, w => w.Length);
            Console.WriteLine($"Найдовше слово: {longestWord}");

            var shortWords = CollectionUtils.Filter(words, w => w.Length <= 5);
            Console.WriteLine("Слова довжиною <= 5:");
            CollectionUtils.ForEach(shortWords, w => Console.WriteLine($" - {w}"));
            Console.WriteLine();

            Console.WriteLine(" Колекція 3: List<Product> ");
            var products = new List<Product>
            {
                new Product { Name = "Ноутбук", Price = 35000 },
                new Product { Name = "Миша", Price = 650 },
                new Product { Name = "Клавіатура", Price = 1800 },
                new Product { Name = "Монітор", Price = 8500 }
            };

            var productNames = CollectionUtils.Map(products, p => p.Name);
            Console.WriteLine("Назви продуктів: " + string.Join(", ", productNames));

            var mostExpensive = CollectionUtils.MaxBy(products, p => p.Price);
            Console.WriteLine($"Найдорожчий продукт: {mostExpensive.Name} ({mostExpensive.Price} грн)");

            var expensiveProducts = CollectionUtils.Filter(products, p => p.Price > 2000);
            Console.WriteLine("Продукти дорожчі за 2000 грн:");
            CollectionUtils.ForEach(expensiveProducts, p => Console.WriteLine($" - {p.Name}: {p.Price} грн"));
        }
    }
}