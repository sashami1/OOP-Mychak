using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

/*

# САМОСТІЙНОА РОБОТА №17 
# Тема: LINQ pipelines: ETL та аналітика
# Мета: Розробити та проаналізувати складні LINQ-пайплайни для обробки даних,
#       що імітують сценарії ETL та аналітики, із вимірюванням продуктивності.



## ОПИС РЕАЛІЗОВАНИХ ПАЙПЛАЙНІВ

1. Пайплайн 1: "Найпопулярніші продукти за категоріями"
   - Використані оператори: GroupBy, Select, Join, Take, ToList.
   - Мета: Агрегація продажів за продуктами, об'єднання з довідником продуктів,
     групування за категоріями та відбір TOP-3 найпродаваніших товарів.

2. Пайплайн 2: "Загальний дохід за містами клієнтів"
   - Використані оператори: Join, Select, GroupBy, OrderByDescending.
   - Мета: ETL-трансформація об'єднаних даних (Sales + Products + Customers)
     для розрахунку фінансового обороту (Quantity * UnitPrice) у розрізі міст.

3. Пайплайн 3: "Продажі за місяцями та роками"
   - Використані оператори: Join, GroupBy, Select, OrderBy, ThenBy.
   - Мета: Часовий аналіз обсягів продажів та доходу з двохрівневим сортуванням.

4. Пайплайн 4 (Додатковий): "VIP-клієнти з витратами понад 50,000 грн"
   - Використані оператори: Join, GroupBy, Select, Where, OrderByDescending.
   - Мета: Сегментація клієнтської бази та відбір найбільш прибуткових покупців.


## АНАЛІЗ ПРОДУКТИВНОСТІ ТА ВИСНОВКИ

1. Фактори впливу на час виконання:
   - Операції Join та GroupBy є найбільш ресурсоємними ($O(N \cdot M)$ або $O(N \log N)$).
   - Матеріалізація запитів (`ToList()`) зафіксувала час виконання та виключила
     вплив відкладеного виконання (Lazy Execution).
   - Псевдовипадкова генерація 100 елементів забезпечила миттєву обробку (< 10 мс),
     проте при масштабуванні до $10^6$ елементів знадобиться оптимізація через індекси/PLINQ.

2. Висновки щодо ефективності LINQ в ETL:
   - LINQ значно скорочує обсяг коду та підвищує його читабельність порівняно
     з ручними циклами `foreach` та словниками `Dictionary`.
   - Декларативний стиль дозволяє легко комбінувати трансформації даних.
================================================================================
*/

namespace IndependentWork_11
{
    public class Sale
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime SaleDate { get; set; }
        public int CustomerId { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
    }

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== LINQ Pipelines: ETL та Аналітика ===\n");

            var (products, customers, sales) = GenerateTestData();

            Console.WriteLine($"Завантажено даних: Продуктів={products.Count}, Клієнтів={customers.Count}, Продажів={sales.Count}\n");
            Console.WriteLine(new string('=', 70) + "\n");

            RunPipeline1(sales, products);
            RunPipeline2(sales, products, customers);
            RunPipeline3(sales, products);
            RunPipeline4(sales, products, customers);
        }

        private static void RunPipeline1(List<Sale> sales, List<Product> products)
        {
            var sw = Stopwatch.StartNew();

            var topProducts = sales
                .GroupBy(s => s.ProductId)
                .Select(g => new { ProductId = g.Key, TotalQuantity = g.Sum(s => s.Quantity) })
                .Join(products,
                    stat => stat.ProductId,
                    prod => prod.Id,
                    (stat, prod) => new { prod.Name, prod.Category, stat.TotalQuantity })
                .GroupBy(p => p.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    TopItems = g.OrderByDescending(x => x.TotalQuantity).Take(3).ToList()
                })
                .ToList();

            sw.Stop();

            Console.WriteLine($"[Пайплайн 1] Найпопулярніші продукти за категоріями (Час: {sw.ElapsedTicks} ticks / {sw.ElapsedMilliseconds} ms):");
            foreach (var cat in topProducts)
            {
                Console.WriteLine($" Категорія: {cat.Category}");
                foreach (var item in cat.TopItems)
                {
                    Console.WriteLine($"   - {item.Name}: {item.TotalQuantity} шт.");
                }
            }
            Console.WriteLine();
        }

        private static void RunPipeline2(List<Sale> sales, List<Product> products, List<Customer> customers)
        {
            var sw = Stopwatch.StartNew();

            var revenueByCity = sales
                .Join(products, s => s.ProductId, p => p.Id, (s, p) => new { s.CustomerId, Revenue = s.Quantity * p.UnitPrice })
                .Join(customers, s => s.CustomerId, c => c.Id, (s, c) => new { c.City, s.Revenue })
                .GroupBy(x => x.City)
                .Select(g => new { City = g.Key, TotalRevenue = g.Sum(x => x.Revenue) })
                .OrderByDescending(x => x.TotalRevenue)
                .ToList();

            sw.Stop();

            Console.WriteLine($"[Пайплайн 2] Загальний дохід за містами (Час: {sw.ElapsedTicks} ticks / {sw.ElapsedMilliseconds} ms):");
            foreach (var item in revenueByCity)
            {
                Console.WriteLine($"  - {item.City}: {item.TotalRevenue:N2} грн");
            }
            Console.WriteLine();
        }

        private static void RunPipeline3(List<Sale> sales, List<Product> products)
        {
            var sw = Stopwatch.StartNew();

            var monthlyStats = sales
                .Join(products, s => s.ProductId, p => p.Id, (s, p) => new { s.SaleDate, s.Quantity, Revenue = s.Quantity * p.UnitPrice })
                .GroupBy(s => new { s.SaleDate.Year, s.SaleDate.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    TotalUnits = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.Revenue)
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();

            sw.Stop();

            Console.WriteLine($"[Пайплайн 3] Продажі за місяцями та роками (Час: {sw.ElapsedTicks} ticks / {sw.ElapsedMilliseconds} ms):");
            foreach (var item in monthlyStats)
            {
                Console.WriteLine($"  - {item.Year}-{item.Month:D2}: Продано {item.TotalUnits} шт., Дохід {item.TotalRevenue:N2} грн");
            }
            Console.WriteLine();
        }

        private static void RunPipeline4(List<Sale> sales, List<Product> products, List<Customer> customers)
        {
            var sw = Stopwatch.StartNew();

            var vipCustomers = sales
                .Join(products, s => s.ProductId, p => p.Id, (s, p) => new { s.CustomerId, Revenue = s.Quantity * p.UnitPrice })
                .GroupBy(x => x.CustomerId)
                .Select(g => new { CustomerId = g.Key, TotalSpent = g.Sum(x => x.Revenue) })
                .Where(c => c.TotalSpent > 30000)
                .Join(customers, stat => stat.CustomerId, cust => cust.Id, (stat, cust) => new { cust.Name, cust.City, stat.TotalSpent })
                .OrderByDescending(x => x.TotalSpent)
                .ToList();

            sw.Stop();

            Console.WriteLine($"[Пайплайн 4 - Додатковий] VIP-клієнти з витратами понад 30,000 грн (Час: {sw.ElapsedTicks} ticks / {sw.ElapsedMilliseconds} ms):");
            foreach (var item in vipCustomers)
            {
                Console.WriteLine($"  - {item.Name} ({item.City}): Витрачено {item.TotalSpent:N2} грн");
            }
            Console.WriteLine();
        }

        private static (List<Product>, List<Customer>, List<Sale>) GenerateTestData()
        {
            var rand = new Random(42);

            var categories = new[] { "Електроніка", "Одяг", "Книги", "Дім та сад" };
            var cities = new[] { "Київ", "Львів", "Одеса", "Харків", "Дніпро", "Рівне" };

            var products = new List<Product>();
            for (int i = 1; i <= 100; i++)
            {
                products.Add(new Product
                {
                    Id = i,
                    Name = $"Товар_{i}",
                    Category = categories[rand.Next(categories.Length)],
                    UnitPrice = rand.Next(100, 5000)
                });
            }

            var customers = new List<Customer>();
            for (int i = 1; i <= 100; i++)
            {
                customers.Add(new Customer
                {
                    Id = i,
                    Name = $"Клієнт_{i}",
                    City = cities[rand.Next(cities.Length)]
                });
            }

            var sales = new List<Sale>();
            var startDate = new DateTime(2024, 1, 1);
            for (int i = 1; i <= 100; i++)
            {
                sales.Add(new Sale
                {
                    Id = i,
                    ProductId = rand.Next(1, 101),
                    CustomerId = rand.Next(1, 101),
                    Quantity = rand.Next(1, 10),
                    SaleDate = startDate.AddDays(rand.Next(0, 365))
                });
            }

            return (products, customers, sales);
        }
    }
}