using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IndependentWork5
{
    public class Product
    {
        private string _name;
        private decimal _price;

        public Product(string name, decimal price)
        {
            _name = name;
            _price = price;
        }

        public override string ToString()
        {
            return $"Товар: {_name}, Ціна: {_price} грн";
        }
    }

    public class Customer
    {
        private string _fullName;
        private string _email;

        public Customer(string fullName, string email)
        {
            _fullName = fullName;
            _email = email;
        }

        public override string ToString()
        {
            return $"Клієнт: {_fullName} ({_email})";
        }
    }

    public class Order
    {
        private int _orderId;
        private decimal _totalAmount;

        public Order(int orderId, decimal totalAmount)
        {
            _orderId = orderId;
            _totalAmount = totalAmount;
        }

        public override string ToString()
        {
            return $"Замовлення №{_orderId}, Сума: {_totalAmount} грн";
        }
    }

    public class MyCustomCollection : IEnumerable<string>
    {
        private List<string> _items = new List<string>();

        public void Add(string item)
        {
            _items.Add(item);
        }

        public IEnumerator<string> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class Program
    {
        public static void ProcessStream(Stream stream)
        {
            byte[] buffer = new byte[16];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string result = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine($"Прочитано {bytesRead} байт з {stream.GetType().Name}: {result}");
        }

        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            List<object> objects = new List<object>
            {
                new Product("Ноутбук", 25000m),
                new Customer("Іван Іваненко", "ivan@example.com"),
                new Order(1001, 25000m)
            };

            foreach (var obj in objects)
            {
                Console.WriteLine(obj.ToString());
            }

            MyCustomCollection collection = new MyCustomCollection();
            collection.Add("Елемент 1");
            collection.Add("Елемент 2");
            collection.Add("Елемент 3");

            foreach (string item in collection)
            {
                Console.WriteLine(item);
            }

            string filePath = "test.txt";
            File.WriteAllText(filePath, "Hello, C# Stream!");

            using (FileStream fileStream = File.OpenRead(filePath))
            {
                ProcessStream(fileStream);
            }

            byte[] bytes = Encoding.UTF8.GetBytes("Memory Stream Data");
            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                ProcessStream(memoryStream);
            }

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}