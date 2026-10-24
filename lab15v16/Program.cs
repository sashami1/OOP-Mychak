using System;
using System.Collections.Generic;

namespace lab15v16
{
    public class ValidationTask
    {
        public int Id { get; set; }
        public string EntityName { get; set; } = string.Empty;

        public override string ToString() => $"Об'єкт #{Id} ({EntityName})";
    }

    public class ValidationError
    {
        public int TaskId { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;

        public override string ToString() => $"[Помилка для #{TaskId}]: {ErrorMessage}";
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("ЛАБОРАТОРНА РОБОТА №9 (Система валідації) \n");

            Console.WriteLine(" 1. HashSet<T>: Унікальні правила валідації ");
            var rules = new HashSet<string>();

            Console.WriteLine($"Додаємо 'NotNull': {rules.Add("NotNull")}");
            Console.WriteLine($"Додаємо 'MinLength': {rules.Add("MinLength")}");
            Console.WriteLine($"Додаємо 'NotNull' ще раз: {rules.Add("NotNull")}"); // повернути false
            Console.WriteLine($"Додаємо 'EmailFormat': {rules.Add("EmailFormat")}");

            Console.WriteLine($"\nКількість унікальних правил: {rules.Count}");
            Console.WriteLine("Список активних правил:");
            foreach (var rule in rules)
            {
                Console.WriteLine($" - {rule}");
            }

            Console.WriteLine($"Перевірка наявності 'EmailFormat': {rules.Contains("EmailFormat")}");


            Console.WriteLine("\n 2. Queue<T>: Черга об'єктів на валідацію (FIFO) ");
            var validationQueue = new Queue<ValidationTask>();

            validationQueue.Enqueue(new ValidationTask { Id = 101, EntityName = "Користувач" });
            validationQueue.Enqueue(new ValidationTask { Id = 102, EntityName = "Замовлення" });
            validationQueue.Enqueue(new ValidationTask { Id = 103, EntityName = "Платіж" });

            Console.WriteLine($"Перший у черзі (Peek): {validationQueue.Peek()}");
            Console.WriteLine($"Всього об'єктів у черзі: {validationQueue.Count}");

            Console.WriteLine("\nОбробка черги:");
            while (validationQueue.Count > 0)
            {
                var task = validationQueue.Dequeue();
                Console.WriteLine($"  Оброблено: {task}");
            }


            Console.WriteLine("\n 3. Stack<T>: Історія помилок валідації (LIFO) ");
            var errorHistory = new Stack<ValidationError>();

            errorHistory.Push(new ValidationError { TaskId = 101, ErrorMessage = "Невірний email" });
            errorHistory.Push(new ValidationError { TaskId = 102, ErrorMessage = "Сума замовлення від'ємна" });
            errorHistory.Push(new ValidationError { TaskId = 103, ErrorMessage = "Таймаут з'єднання" });

            Console.WriteLine($"Остання зареєстрована помилка (Peek): {errorHistory.Peek()}");
            Console.WriteLine($"Всього помилок в історії: {errorHistory.Count}");

            Console.WriteLine("\nСкасування/Перегляд останніх помилок (Undo/Pop):");
            while (errorHistory.Count > 0)
            {
                var error = errorHistory.Pop();
                Console.WriteLine($" Вилучено з топу стеку: {error}");
            }
        }
    }
}