using System;
using System.Collections.Generic;

namespace IndependentWork6
{
    public interface INotifier
    {
        void Send(string message);
    }

    public class EmailNotifier : INotifier
    {
        public void Send(string message)
        {
            Console.WriteLine($"[Email] Надсилання листа: '{message}'");
        }
    }

    public class SmsNotifier : INotifier
    {
        public void Send(string message)
        {
            Console.WriteLine($"[SMS] Надсилання повідомлення: '{message}'");
        }
    }

    public class PushNotifier : INotifier
    {
        public void Send(string message)
        {
            Console.WriteLine($"[Push] Надсилання сповіщення: '{message}'");
        }
    }


    public abstract class FileProcessor
    {
        public string FilePath { get; }

        protected FileProcessor(string filePath)
        {
            FilePath = filePath;
        }

        public void ProcessFile()
        {
            OpenFile();
            ProcessContent();
            CloseFile();
        }

        private void OpenFile()
        {
            Console.WriteLine($"Відкриття файлу '{FilePath}'...");
        }

        private void CloseFile()
        {
            Console.WriteLine($"Закриття файлу '{FilePath}'.");
        }

        protected abstract void ProcessContent();
    }

    public class TextFileProcessor : FileProcessor
    {
        public TextFileProcessor(string filePath) : base(filePath) { }

        protected override void ProcessContent()
        {
            Console.WriteLine($"[TextProcessor] Зчитування та аналіз текстових рядків із '{FilePath}'.");
        }
    }

    public class XmlFileProcessor : FileProcessor
    {
        public XmlFileProcessor(string filePath) : base(filePath) { }

        protected override void ProcessContent()
        {
            Console.WriteLine($"[XmlProcessor] Парсинг XML-структури та вузлів із '{FilePath}'.");
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine(" Сценарій 1: Система сповіщень (Інтерфейс) ");
            List<INotifier> notifiers = new List<INotifier>
            {
                new EmailNotifier(),
                new SmsNotifier(),
                new PushNotifier()
            };

            foreach (var notifier in notifiers)
            {
                notifier.Send("Ваше замовлення успішно сформовано!");
            }

            Console.WriteLine("\n Сценарій 2: Обробка файлів (Абстрактний клас)");
            List<FileProcessor> fileProcessors = new List<FileProcessor>
            {
                new TextFileProcessor("document.txt"),
                new XmlFileProcessor("config.xml")
            };

            foreach (var processor in fileProcessors)
            {
                processor.ProcessFile();
                Console.WriteLine();
            }
        }
    }
}