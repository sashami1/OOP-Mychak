using System;
using System.Collections.Generic;

namespace lab14v16
{
    public static class CollectionHelper<T>
    {
        public static IEnumerable<TResult> Zip<TSecond, TResult>(
            IEnumerable<T> first,
            IEnumerable<TSecond> second,
            Func<T, TSecond, TResult> resultSelector)
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));
            if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));

            var result = new List<TResult>();

            using var enum1 = first.GetEnumerator();
            using var enum2 = second.GetEnumerator();

            while (enum1.MoveNext() && enum2.MoveNext())
            {
                result.Add(resultSelector(enum1.Current, enum2.Current));
            }

            return result;
        }
    }

    public class Person
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }

        public override string ToString() => $"{Name} ({Age} років)";
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine(" Демонстрація 1: Рядки та числа ");
            var names = new List<string> { "Олександр", "Марія", "Іван", "Олена" };
            var ages = new List<int> { 20, 19, 21, 18 };

            var mergedStrings = CollectionHelper<string>.Zip(
                names,
                ages,
                (name, age) => $"{name} - {age} років"
            );

            foreach (var item in mergedStrings)
            {
                Console.WriteLine(item);
            }

            Console.WriteLine("\n Демонстрація 2: Створення об'єктів Person ");
            var people = CollectionHelper<string>.Zip(
                names,
                ages,
                (name, age) => new Person { Name = name, Age = age }
            );

            foreach (var person in people)
            {
                Console.WriteLine(person);
            }

            Console.WriteLine("\n  Демонстрація 3: Обчислення підсумкових цін ");
            var prices = new List<double> { 100.0, 250.5, 500.0 };
            var discounts = new List<double> { 10.0, 50.5, 100.0 };

            var finalPrices = CollectionHelper<double>.Zip(
                prices,
                discounts,
                (price, discount) => price - discount
            );

            foreach (var finalPrice in finalPrices)
            {
                Console.WriteLine($"Підсумкова ціна: {finalPrice} грн");
            }
        }
    }
}