using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace IndependentWork11
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

    public class DataGenerator
    {
        public static (List<Sale> sales, List<Product> products, List<Customer> customers) GenerateData()
        {
            var random = new Random(42);

            var categories = new[] { "Electronics", "Clothing", "Books", "Home & Kitchen", "Sports" };
            var cities = new[] { "Kyiv", "Lviv", "Odesa", "Kharkiv", "Dnipro", "Rivne" };

            var products = Enumerable.Range(1, 60).Select(i => new Product
            {
                Id = i,
                Name = $"Product_{i}",
                Category = categories[random.Next(categories.Length)],
                UnitPrice = Math.Round((decimal)(random.NextDouble() * 490 + 10), 2)
            }).ToList();

            var customers = Enumerable.Range(1, 80).Select(i => new Customer
            {
                Id = i,
                Name = $"Customer_{i}",
                City = cities[random.Next(cities.Length)]
            }).ToList();

            var startDate = new DateTime(2024, 1, 1);
            var sales = Enumerable.Range(1, 500).Select(i => new Sale
            {
                Id = i,
                ProductId = products[random.Next(products.Count)].Id,
                CustomerId = customers[random.Next(customers.Count)].Id,
                Quantity = random.Next(1, 15),
                SaleDate = startDate.AddDays(random.Next(0, 730))
            }).ToList();

            return (sales, products, customers);
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var (sales, products, customers) = DataGenerator.GenerateData();

            Console.WriteLine($"Завантажено даних: Продажів = {sales.Count}, Товарів = {products.Count}, Клієнтів = {customers.Count}\n");

            Pipeline1_TopProductsByCategory(sales, products);
            Pipeline2_TotalRevenueByCity(sales, products, customers);
            Pipeline3_SalesByMonthAndYear(sales, products);
            Pipeline4_VIPCustomersAnalysis(sales, products, customers);
        }

        public static void Pipeline1_TopProductsByCategory(List<Sale> sales, List<Product> products)
        {
            var sw = Stopwatch.StartNew();

            var result = sales
                .GroupBy(s => s.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalQuantity = g.Sum(s => s.Quantity)
                })
                .Join(products,
                    stat => stat.ProductId,
                    prod => prod.Id,
                    (stat, prod) => new
                    {
                        prod.Name,
                        prod.Category,
                        stat.TotalQuantity
                    })
                .GroupBy(p => p.Category)
                .Select(catGroup => new
                {
                    Category = catGroup.Key,
                    TopProducts = catGroup
                        .OrderByDescending(p => p.TotalQuantity)
                        .Take(3)
                        .ToList()
                })
                .ToList();

            sw.Stop();

            Console.WriteLine("--- Пайплайн 1: Найпопулярніші продукти за категоріями ---");
            foreach (var cat in result)
            {
                Console.WriteLine($"Категорія: {cat.Category}");
                foreach (var prod in cat.TopProducts)
                {
                    Console.WriteLine($"   * {prod.Name} - {prod.TotalQuantity} шт.");
                }
            }
            Console.WriteLine($"Час виконання: {sw.Elapsed.TotalMilliseconds:F4} ms ({sw.ElapsedTicks} ticks)\n");
        }

        public static void Pipeline2_TotalRevenueByCity(List<Sale> sales, List<Product> products, List<Customer> customers)
        {
            var sw = Stopwatch.StartNew();

            var result = sales
                .Join(products, s => s.ProductId, p => p.Id, (s, p) => new { s.CustomerId, s.Quantity, p.UnitPrice })
                .Join(customers, sp => sp.CustomerId, c => c.Id, (sp, c) => new { c.City, Revenue = sp.Quantity * sp.UnitPrice })
                .GroupBy(x => x.City)
                .Select(g => new
                {
                    City = g.Key,
                    TotalRevenue = g.Sum(x => x.Revenue)
                })
                .OrderByDescending(x => x.TotalRevenue)
                .ToList();

            sw.Stop();

            Console.WriteLine("--- Пайплайн 2: Загальний дохід за містами клієнтів ---");
            foreach (var item in result)
            {
                Console.WriteLine($"Місто: {item.City,-10} | Дохід: {item.TotalRevenue,12:C2}");
            }
            Console.WriteLine($"Час виконання: {sw.Elapsed.TotalMilliseconds:F4} ms ({sw.ElapsedTicks} ticks)\n");
        }

        public static void Pipeline3_SalesByMonthAndYear(List<Sale> sales, List<Product> products)
        {
            var sw = Stopwatch.StartNew();

            var result = sales
                .Join(products, s => s.ProductId, p => p.Id, (s, p) => new { s.SaleDate, s.Quantity, Revenue = s.Quantity * p.UnitPrice })
                .GroupBy(x => new { x.SaleDate.Year, x.SaleDate.Month })
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

            Console.WriteLine("--- Пайплайн 3: Продажі за місяцями та роками (Перші 5 періодів) ---");
            foreach (var item in result.Take(5))
            {
                Console.WriteLine($"Період: {item.Year}-{item.Month:D2} | Одиниць: {item.TotalUnits,4} | Дохід: {item.TotalRevenue,10:C2}");
            }
            Console.WriteLine($"Час виконання: {sw.Elapsed.TotalMilliseconds:F4} ms ({sw.ElapsedTicks} ticks)\n");
        }

        public static void Pipeline4_VIPCustomersAnalysis(List<Sale> sales, List<Product> products, List<Customer> customers)
        {
            var sw = Stopwatch.StartNew();

            var result = sales
                .Join(products, s => s.ProductId, p => p.Id, (s, p) => new { s.CustomerId, Revenue = s.Quantity * p.UnitPrice })
                .GroupBy(x => x.CustomerId)
                .Select(g => new
                {
                    CustomerId = g.Key,
                    TotalSpent = g.Sum(x => x.Revenue),
                    OrdersCount = g.Count()
                })
                .Where(x => x.TotalSpent > 5000)
                .Join(customers, stat => stat.CustomerId, c => c.Id, (stat, c) => new
                {
                    c.Name,
                    c.City,
                    stat.TotalSpent,
                    stat.OrdersCount
                })
                .OrderByDescending(x => x.TotalSpent)
                .ToList();

            sw.Stop();

            Console.WriteLine("--- Пайплайн 4: Аналіз VIP-клієнтів (Витрати > 5000 грн) ---");
            foreach (var vip in result.Take(5))
            {
                Console.WriteLine($"Клієнт: {vip.Name,-12} | Місто: {vip.City,-8} | Замовлень: {vip.OrdersCount,2} | Витрачено: {vip.TotalSpent,10:C2}");
            }
            Console.WriteLine($"Час виконання: {sw.Elapsed.TotalMilliseconds:F4} ms ({sw.ElapsedTicks} ticks)\n");
        }
    }
}