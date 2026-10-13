using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace IndependentWork12
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine(" 1. Дослідження продуктивності: LINQ vs PLINQ ");
            RunBenchmark();

            Console.WriteLine("\n 2. Дослідження безпеки та побічних ефектів у PLINQ ");
            RunThreadSafetyDemo();
        }

        private static bool IsPrime(int number)
        {
            if (number <= 1) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;

            var boundary = (int)Math.Floor(Math.Sqrt(number));
            for (int i = 3; i <= boundary; i += 2)
            {
                if (number % i == 0) return false;
            }
            return true;
        }

        private static void RunBenchmark()
        {
            int[] sizes = { 1_000_000, 5_000_000, 10_000_000 };
            var random = new Random(42);

            Console.WriteLine($"{"Розмір",-12} | {"LINQ (ms)",-12} | {"PLINQ (ms)",-12} | {"Прискорення",-12}");
            Console.WriteLine(new string('-', 55));

            foreach (var size in sizes)
            {
                var numbers = new List<int>(size);
                for (int i = 0; i < size; i++)
                {
                    numbers.Add(random.Next(1, 100_000));
                }

                var sw = Stopwatch.StartNew();
                var linqCount = numbers.Where(IsPrime).Count();
                sw.Stop();
                double linqMs = sw.Elapsed.TotalMilliseconds;

                sw.Restart();
                var plinqCount = numbers.AsParallel().Where(IsPrime).Count();
                sw.Stop();
                double plinqMs = sw.Elapsed.TotalMilliseconds;

                double speedup = linqMs / plinqMs;
                Console.WriteLine($"{size,-12:N0} | {linqMs,-12:F2} | {plinqMs,-12:F2} | {speedup,-12:F2}x");
            }
        }

        private static void RunThreadSafetyDemo()
        {
            int itemCount = 1_000_000;
            var data = Enumerable.Range(1, itemCount).ToList();

            int unsafeSum = 0;
            data.AsParallel().ForAll(x =>
            {
                if (x % 2 == 0)
                {
                    unsafeSum++; 
                }
            });

            int safeSumInterlocked = 0;
            data.AsParallel().ForAll(x =>
            {
                if (x % 2 == 0)
                {
                    Interlocked.Increment(ref safeSumInterlocked);
                }
            });

            int idiomaticSum = data.AsParallel().Count(x => x % 2 == 0);

            int expectedSum = itemCount / 2;

            Console.WriteLine($"Очікуваний результат:             {expectedSum:N0}");
            Console.WriteLine($"Небезпечний PLINQ (з Race Condition): {unsafeSum:N0} (Помилка!)");
            Console.WriteLine($"Безпечний PLINQ (Interlocked):    {safeSumInterlocked:N0} (Коректно)");
            Console.WriteLine($"Чистий PLINQ (без побічних ефектів): {idiomaticSum:N0} (Коректно)");
        }
    }
}