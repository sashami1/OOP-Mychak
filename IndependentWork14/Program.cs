using System;
using System.Net.Http;
using System.Threading;

/*
САМОСТІЙНА РОБОТА №20 
Тема: Реалізація exponential backoff.
Мета: Розробити власний узагальнений допоміжний клас RetryPolicy для реалізації патерну Retry з експоненційною затримкою та випадковим Jitter.

ОПИС РЕАЛІЗАЦІЇ КЛАСУ RetryPolicy
1. Клас RetryPolicy містить приватні поля _maxRetries, _initialDelay та _jitterFactor.
2. Розрахунок затримки виконується за строгою формулою:
   delay = initialDelay * Math.Pow(2, retryAttempt) * (1 + random.NextDouble() * jitterFactor * 2 - jitterFactor)
3. Логуються всі спроби, причини невдачі, розрахована затримка в мілісекундах та фінальний результат або виняток.

ВИСНОВКИ ЩОДО МЕЖ ЗАСТОСУВАННЯ ПАТЕРНУ RETRY З EXPONENTIAL BACKOFF
1. Коли застосовувати: Для обробки тимчасових мережевих збоїв, коротривалих таймаутів баз даних, високого навантаження на API або перезапуску сервісів.
2. Навіщо потрібен Exponential Backoff: Дозволяє поступово збільшувати паузу між спробами, даючи системі, що зазнала збою, час на відновлення замість повторного її перевантаження.
3. Навіщо потрібен Jitter: Додавання випадкової складової розсинхронізовує запити від тисяч клієнтів (запобігає проблемам Thundering Herd Problem та Retry Storm).
4. Межі застосування:
   - Патерн НЕ слід застосовувати для бізнес-помилок (невірний пароль, відсутність прав доступу, валідація даних).
   - НЕбезпечно використовувати Retry для неідемпотентних операцій (наприклад, списання коштів з картки), оскільки повторна спроба може призвести до дублювання дій.

ВІДПОВІДІ НА КОНТРОЛЬНІ ЗАПИТАННЯ
1. Експоненційна затримка ефективніша за фіксовану, оскільки не перевантажує систему частими запитами, якщо сервіс потребує більше часу на відновлення.
2. Jitter додає рандомізацію в затримку, завдяки чому множина паралельних клієнтів не робить повторні запити одночасно в одну й ту ж секунду.
3. Делегат shouldRetry дозволяє вибірково перевіряти виняток і повторювати спроби лише для тимчасових помилок (наприклад, HttpRequestException), відразу пропускаючи фатальні помилки.
4. Повторна спроба не є безпечною для неідемпотентних операцій, а також при помилках авторизації чи фатальних збоях конфігурації.
*/

namespace IndependentWork14
{
    public class RetryPolicy
    {
        private readonly int _maxRetries;
        private readonly TimeSpan _initialDelay;
        private readonly double _jitterFactor;
        private readonly Random _random = new Random();

        public RetryPolicy(int maxRetries, TimeSpan initialDelay, double jitterFactor = 0.2)
        {
            if (maxRetries <= 0)
            {
                throw new ArgumentException("Кількість спроб повинна бути більшою за 0.", nameof(maxRetries));
            }

            _maxRetries = maxRetries;
            _initialDelay = initialDelay;
            _jitterFactor = jitterFactor;
        }

        public T Execute<T>(Func<T> operation, Func<Exception, bool>? shouldRetry = null)
        {
            for (int attempt = 0; attempt < _maxRetries; attempt++)
            {
                try
                {
                    Console.WriteLine($"[Спроба {attempt + 1}/{_maxRetries}] Виконання операції...");
                    return operation();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Спроба {attempt + 1} завершилася помилкою: {ex.GetType().Name} - {ex.Message}");

                    if (attempt == _maxRetries - 1)
                    {
                        Console.WriteLine("Вичерпано всі спроби. Операцію провалено.\n");
                        throw;
                    }

                    if (shouldRetry != null && !shouldRetry(ex))
                    {
                        Console.WriteLine("Виняток не задовольняє умові shouldRetry. Повтори припинено.\n");
                        throw;
                    }

                    double jitterMultiplier = 1 + (_random.NextDouble() * _jitterFactor * 2 - _jitterFactor);
                    double delayMilliseconds = _initialDelay.TotalMilliseconds * Math.Pow(2, attempt) * jitterMultiplier;
                    TimeSpan delay = TimeSpan.FromMilliseconds(delayMilliseconds);

                    Console.WriteLine($"Пауза перед наступною спробою: {delay.TotalMilliseconds:F1} мс (Exponential Backoff + Jitter)\n");
                    Thread.Sleep(delay);
                }
            }

            throw new InvalidOperationException("Операція не була виконана.");
        }
    }

    public class Program
    {
        private static int _successScenarioAttempts = 0;
        private static int _failScenarioAttempts = 0;

        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("Демонстрація робота узагальненого класу RetryPolicy\n");

            var policy = new RetryPolicy(
                maxRetries: 4, 
                initialDelay: TimeSpan.FromMilliseconds(150), 
                jitterFactor: 0.2
            );

            Func<Exception, bool> shouldRetryPredicate = ex => 
                ex is HttpRequestException || ex is InvalidOperationException;

            Console.WriteLine("1. Сценарій: Тимчасовий збій (успіх на 3-й спробі)");
            try
            {
                string result = policy.Execute(() => SimulateTemporaryFailureOperation(), shouldRetryPredicate);
                Console.WriteLine($"Результат виконання: {result}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Неочікувана помилка: {ex.Message}\n");
            }

            Console.WriteLine("2. Сценарій: Постійний збій (остаточна невдача після всіх спроб)");
            try
            {
                policy.Execute(() => SimulatePermanentFailureOperation(), shouldRetryPredicate);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Перехоплено остаточний виняток у Main: {ex.GetType().Name} - {ex.Message}\n");
            }

            Console.WriteLine("Усі сценарії успішно виконано.");
        }

        private static string SimulateTemporaryFailureOperation()
        {
            _successScenarioAttempts++;
            if (_successScenarioAttempts < 3)
            {
                throw new HttpRequestException($"[Імітація] Мережа недоступна (помилка №{_successScenarioAttempts}).");
            }

            return "Дані успішно отримано з сервера!";
        }

        private static bool SimulatePermanentFailureOperation()
        {
            _failScenarioAttempts++;
            throw new InvalidOperationException($"[Імітація] Критичний збій бази даних (помилка №{_failScenarioAttempts}).");
        }
    }
}