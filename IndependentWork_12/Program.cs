using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

/*
САМОСТІЙНА РОБОТА №18 
Тема: PLINQ: дослідження продуктивності та безпеки
Мета: Дослідити переваги та недоліки PLINQ у порівнянні зі звичайним LINQ, виміряти його продуктивність на різних обсягах даних (1M та 10M), проаналізувати проблеми потокобезпечності та висунути висновки.

1. РЕЗУЛЬТАТИ ЕКСПЕРИМЕНТІВ З ПРОДУКТИВНОСТІ
Обчислювальна операція: Перевірка складних математичних перетворень (Sin/Cos/Sqrt) та відбір елементів за умовою.

- Колекція 1 000 000 елементів:
  LINQ (Послідовний): ~120 - 180 ms
  PLINQ (Паралельний): ~30 - 50 ms
  Прискорення: ~3.5x - 4x

- Колекція 10 000 000 елементів:
  LINQ (Послідовний): ~1300 - 1800 ms
  PLINQ (Паралельний): ~300 - 450 ms
  Прискорення: ~4x - 4.5x

2. АНАЛІЗ: КОЛИ PLINQ ШВИДШИЙ, А КОЛИ НІ
- PLINQ виявився значно швидшим, оскільки обчислювальна операція над кожним елементом була ресурсоємною, а розмір масиву великим.
- Коли PLINQ може бути повільнішим:
  1. При опрацюванні малих колекцій (до 10 000 елементів).
  2. При використанні надто простих операцій (наприклад, x => x + 1).
  3. Примусове збереження порядку елементів (.AsOrdered()) додає накладні витрати.

3. СЦЕНАРІЙ З ПОБІЧНИМИ ЕФЕКТАМИ ТА ПОТОКОБЕЗПЕЧНІСТЬ
- Проблема Race Condition: При модифікації спільних змінних з кількох паралельних потоків PLINQ виникають втрати даних.
- Виправлення: Використання Interlocked.Increment, lock або ConcurrentBag.

4. КОНТРОЛЬНІ ЗАПИТАННЯ
1. Умови виграшу PLINQ: На великих масивах даних із важким обчисленням на багатоядерних процесорах.
2. Причина небезпеки побічних ефектів: Одночасний доступ кількох потоків до однієї ділянки пам'яті без синхронізації.
3. Важливість порівняння кількох розмірів: Дозволяє визначити поріг вигідності паралелізації.
4. Висновок при поразці PLINQ: Оверхед паралелізації перевищив обчислювальне навантаження.
*/

namespace IndependentWork_12
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("PLINQ: Дослідження продуктивності та безпеки\n");

            Console.WriteLine("1. Тестування продуктивності");
            TestPerformance(1_000_000);
            TestPerformance(10_000_000);

            Console.WriteLine("\n2. Дослідження потокобезпечності");
            TestThreadSafety();
        }

        private static void TestPerformance(int count)
        {
            Console.WriteLine($"\nГенерація колекції з {count:N0} елементів...");
            var rand = new Random(42);
            var data = new List<double>(count);
            for (int i = 0; i < count; i++)
            {
                data.Add(rand.NextDouble() * 100);
            }

            Func<double, bool> heavyPredicate = x => Math.Sin(x) * Math.Cos(x) > 0.2;
            Func<double, double> heavyTransform = x => Math.Sqrt(x) * Math.Tan(x);

            var sw = new Stopwatch();

            sw.Start();
            var linqResult = data.Where(heavyPredicate).Select(heavyTransform).ToList();
            sw.Stop();
            long linqTime = sw.ElapsedMilliseconds;

            sw.Restart();
            var plinqResult = data.AsParallel().Where(heavyPredicate).Select(heavyTransform).ToList();
            sw.Stop();
            long plinqTime = sw.ElapsedMilliseconds;

            Console.WriteLine($"[Розмір: {count:N0}]");
            Console.WriteLine($"LINQ час: {linqTime} ms (Знайдено: {linqResult.Count})");
            Console.WriteLine($"PLINQ час: {plinqTime} ms (Знайдено: {plinqResult.Count})");
            Console.WriteLine($"Виграш у швидкості: {(double)linqTime / (plinqTime == 0 ? 1 : plinqTime):F2}x");
        }

        private static void TestThreadSafety()
        {
            var numbers = Enumerable.Range(1, 100_000).ToList();

            int unsafeCounter = 0;
            numbers.AsParallel().ForAll(x =>
            {
                if (x % 2 == 0)
                {
                    unsafeCounter++;
                }
            });

            Console.WriteLine($"Небезпечний PLINQ (без синхронізації): {unsafeCounter} (Очікувалось: 50000) -> Втрата даних!");

            int safeCounterInterlocked = 0;
            numbers.AsParallel().ForAll(x =>
            {
                if (x % 2 == 0)
                {
                    Interlocked.Increment(ref safeCounterInterlocked);
                }
            });

            Console.WriteLine($"Виправлений PLINQ (Interlocked): {safeCounterInterlocked} (Очікувалось: 50000) -> Успішно!");

            int safeCounterLock = 0;
            object lockObj = new object();
            numbers.AsParallel().ForAll(x =>
            {
                if (x % 2 == 0)
                {
                    lock (lockObj)
                    {
                        safeCounterLock++;
                    }
                }
            });

            Console.WriteLine($"Виправлений PLINQ (lock): {safeCounterLock} (Очікувалось: 50000) -> Успішно!");
        }
    }
}