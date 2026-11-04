using System;

namespace IndependentWork_7
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== Демонстрація роботи Cache (FIFO) ===\n");

            var cache = new Cache<string, string>(3);

            cache.Add("key1", "Значення 1");
            cache.Add("key2", "Значення 2");
            cache.Add("key3", "Значення 3");

            Console.WriteLine($"Додано 3 елементи. Поточний Count: {cache.Count}");
            Console.WriteLine($"key1: {cache.Get("key1")}");

            Console.WriteLine("\nДодаємо 4-й елемент ('key4')...");
            cache.Add("key4", "Значення 4");

            Console.WriteLine($"Поточний Count: {cache.Count}");
            Console.WriteLine($"Чи є 'key1' (найстаріший)? {cache.ContainsKey("key1")}");
            Console.WriteLine($"Чи є 'key4' (новий)? {cache.ContainsKey("key4")}");
        }
    }
}