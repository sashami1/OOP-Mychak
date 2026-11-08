using System;
using System.IO;
using System.Net.Http;
using System.Threading;

namespace lab18_v16
{
    public static class RetryHelper
    {
        public static T ExecuteWithRetry<T>(
            Func<T> operation,
            int retryCount = 5,
            TimeSpan initialDelay = default,
            Func<Exception, bool>? shouldRetry = null)
        {
            if (initialDelay == default)
            {
                initialDelay = TimeSpan.FromMilliseconds(200);
            }

            for (int attempt = 1; attempt <= retryCount; attempt++)
            {
                try
                {
                    Console.WriteLine($"[Спроба {attempt}/{retryCount}] Виконання операції...");
                    return operation();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($" Спроба {attempt} завершилася помилкою: {ex.GetType().Name} - {ex.Message}");

                    if (attempt == retryCount)
                    {
                        Console.WriteLine(" Досягнуто максимальну кількість спроб. Операція провалена.");
                        throw;
                    }

                    if (shouldRetry != null && !shouldRetry(ex))
                    {
                        Console.WriteLine(" Виняток не підпадає під критерії shouldRetry. Повтори припинено.");
                        throw;
                    }

                    int delayMs = (int)(initialDelay.TotalMilliseconds * Math.Pow(2, attempt - 1));
                    Console.WriteLine($" Пауза перед наступною спробою: {delayMs} мс (експоненційна затримка)\n");
                    Thread.Sleep(delayMs);
                }
            }

            throw new InvalidOperationException("Операція не вдалася.");
        }

        public static void ExecuteWithRetry(
            Action operation,
            int retryCount = 5,
            TimeSpan initialDelay = default,
            Func<Exception, bool>? shouldRetry = null)
        {
            ExecuteWithRetry<object?>(() =>
            {
                operation();
                return null;
            }, retryCount, initialDelay, shouldRetry);
        }
    }

    public class FileProcessor
    {
        private int _readAttempts = 0;

        public string GetSmsTemplate(string path)
        {
            _readAttempts++;
            if (_readAttempts <= 3)
            {
                throw new FileNotFoundException($"[Імітація] Файл шаблону '{path}' не знайдено (спроба {_readAttempts}).");
            }

            return "Шаблон SMS: Шановний клієнте, ваше замовлення готове!";
        }
    }

    public class NetworkClient
    {
        private int _sendAttempts = 0;

        public void SendSms(string phoneNumber, string message)
        {
            _sendAttempts++;
            if (_sendAttempts <= 2)
            {
                throw new HttpRequestException($"[Імітація] Помилка мережі при відправці SMS на {phoneNumber} (спроба {_sendAttempts}).");
            }

            Console.WriteLine($" SMS успішно відправлено на номер {phoneNumber}: \"{message}\"");
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var fileProcessor = new FileProcessor();
            var networkClient = new NetworkClient();

            Func<Exception, bool> shouldRetryPredicate = ex => 
                ex is FileNotFoundException || ex is HttpRequestException;

            Console.WriteLine(" Сценарій 1: Читання шаблону SMS з файлу (FileNotFoundException 3 рази -> Успіх) \n");

            string template = RetryHelper.ExecuteWithRetry(
                () => fileProcessor.GetSmsTemplate("sms_template.txt"),
                retryCount: 5,
                initialDelay: TimeSpan.FromMilliseconds(100),
                shouldRetry: shouldRetryPredicate
            );

            Console.WriteLine($"Результат: {template}\n");
            Console.WriteLine(new string('-', 60) + "\n");

            Console.WriteLine(" Сценарій 2: Відправка SMS через мережу (HttpRequestException 2 рази -> Успіх) \n");

            RetryHelper.ExecuteWithRetry(
                () => networkClient.SendSms("+380971234567", template),
                retryCount: 5,
                initialDelay: TimeSpan.FromMilliseconds(100),
                shouldRetry: shouldRetryPredicate
            );

            Console.WriteLine("\n Усі операції успішно завершено ");
        }
    }
}