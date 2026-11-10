using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using Polly.CircuitBreaker;

/*
САМОСТІЙНА РОБОТА №19 
Тема: Дизайн винятків і політики retry: теорія та кейси Polly.
Мета: Опрацювати теорію обробки винятків у .NET, вивчити патерни відмовостійкості (Retry, Circuit Breaker, Timeout), реалізувати власний тип винятку та практичні сценарії з Polly.

СЦЕНАРІЙ 1: Власний тип винятку InsufficientBalanceException
Опис проблеми: В банківській системі спроба списання суми, що перевищує поточний баланс рахунку, є порушенням бізнес-правил.
Обґрунтування: Використано власний виняток з трьома стандартними конструкторами та додатковими властивостями (AccountHolder, RequestedAmount, CurrentBalance) для передачі контексту помилки.

СЦЕНАРІЙ 2: Оплата через зовнішній платіжний шлюз (Retry + Exponential Backoff + Jitter)
Опис проблеми: Зовнішній API шлюз може мати тимчасові затримки або короткочасні збої мережі.
Обґрунтування стратегії: Використано Retry з експоненційною затримкою та випадковим коливанням (Jitter). Це дозволяє дати шлюзу час на відновлення і запобігає "шторму" одночасних повторних запитів.

СЦЕНАРІЙ 3: Запити до бази даних з авто-відключенням (Circuit Breaker)
Опис проблеми: База даних зазнала критичної аварії. Постійні повторні запити лише погіршують стан сервера та даремно блокують потоки застосунку.
Обґрунтування стратегії: Використано Circuit Breaker. Після 2 послідовних збоїв вимикач переходить у стан Open і миттєво відхиляє нові запити без звернення до БД. Через 3 секунди паузи вимикач переходить у Half-Open для перевірки спроможності бази відновити роботу.

КОЛИ КИДАТИ ВИНЯТОК, А КОЛИ ПОВЕРТАТИ RESULT<T>
Винятки (Exceptions) слід використовувати виключно для невизначених, аномальних та справді виняткових ситуацій, які порушують контракт виконання методу (наприклад, розрив з'єднання з БД, пошкоджений конфігураційний файл, відсутність критичного ресурсу). Винятки мають високу ціну розкрутки стека (stack trace).
Повернення Result<T> доцільно застосовувати для очікуваних результатів бізнес-логіки та валідації вводу (наприклад, "користувача не знайдено", "некоректний пароль", "формат email невірний"). Це дозволяє явно декларувати можливий неуспіх у сигнатурі методу, уникати оверхеду винятків та зручно будувати ланцюжки обробки.

ЗАГАЛЬНІ ВИСНОВКИ ЩОДО ПЕРЕВАГ ТА ОБМЕЖЕНЬ POLLY
Переваги:
1. Декларативне налаштування складних стратегій відмовостійкості (Resilience Pipeline).
2. Повна підтримка асинхронності, експоненційних затримок та Jitter.
3. Можливість комбінування декількох стратегій в один пайплайн.
Обмеження:
1. Застосування Retry до неідемпотентних операцій (наприклад, повторний списання грошей) може призвести до дублювання дій.
2. Некоректно налаштований Circuit Breaker може блокувати доступ до працюючого сервісу при поодиноких помилках.
*/

namespace IndependentWork19
{
    public class InsufficientBalanceException : Exception
    {
        public string AccountHolder { get; } = string.Empty;
        public decimal RequestedAmount { get; }
        public decimal CurrentBalance { get; }

        public InsufficientBalanceException() { }

        public InsufficientBalanceException(string message) : base(message) { }

        public InsufficientBalanceException(string message, Exception innerException) 
            : base(message, innerException) { }

        public InsufficientBalanceException(string message, string accountHolder, decimal requested, decimal balance)
            : base(message)
        {
            AccountHolder = accountHolder;
            RequestedAmount = requested;
            CurrentBalance = balance;
        }
    }

    public class Program
    {
        private static int _paymentAttempts = 0;
        private static int _dbAttempts = 0;

        public static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("1. Демонстрація власного винятку InsufficientBalanceException\n");
            TestCustomException();

            Console.WriteLine("\n2. Сценарій Polly: Retry + Exponential Backoff + Jitter\n");
            await TestPollyRetryAsync();

            Console.WriteLine("\n3. Сценарій Polly: Circuit Breaker (Автоматичний вимикач)\n");
            await TestPollyCircuitBreakerAsync();
        }

        private static void TestCustomException()
        {
            try
            {
                MakeBankTransaction("Олена Мичак", requested: 5000m, balance: 1200m);
            }
            catch (InsufficientBalanceException ex)
            {
                Console.WriteLine($"Перехоплено InsufficientBalanceException: {ex.Message}");
                Console.WriteLine($"Клієнт: {ex.AccountHolder}");
                Console.WriteLine($"Запитано: {ex.RequestedAmount} грн, Наявний баланс: {ex.CurrentBalance} грн");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Загальна помилка: {ex.Message}");
            }
        }

        private static void MakeBankTransaction(string accountHolder, decimal requested, decimal balance)
        {
            if (requested > balance)
            {
                throw new InsufficientBalanceException(
                    "Недостатньо коштів на рахунку для здійснення операції.",
                    accountHolder,
                    requested,
                    balance
                );
            }
        }

        private static async Task TestPollyRetryAsync()
        {
            var retryPipeline = new ResiliencePipelineBuilder()
                .AddRetry(new Polly.Retry.RetryStrategyOptions
                {
                    ShouldHandle = new PredicateBuilder().Handle<HttpRequestException>(),
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromMilliseconds(200),
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true,
                    OnRetry = args =>
                    {
                        Console.WriteLine($"  Лог Retry: Спроба #{args.AttemptNumber + 1} невдала. Затримка: {args.RetryDelay.TotalMilliseconds:F0} мс. Причина: {args.Outcome.Exception?.Message}");
                        return ValueTask.CompletedTask;
                    }
                })
                .Build();

            try
            {
                await retryPipeline.ExecuteAsync(async cancellationToken =>
                {
                    _paymentAttempts++;
                    Console.WriteLine($"  Виконання запиту до платіжного шлюзу (Спроба {_paymentAttempts})...");
                    
                    if (_paymentAttempts < 3)
                    {
                        throw new HttpRequestException("Платіжний шлюз тимчасово недоступний (503 Service Unavailable).");
                    }

                    Console.WriteLine("  Платіж успішно проведено!");
                    await Task.CompletedTask;
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Остаточний результат: {ex.Message}");
            }
        }

        private static async Task TestPollyCircuitBreakerAsync()
        {
            var circuitBreakerPipeline = new ResiliencePipelineBuilder()
                .AddCircuitBreaker(new Polly.CircuitBreaker.CircuitBreakerStrategyOptions
                {
                    ShouldHandle = new PredicateBuilder().Handle<InvalidOperationException>(),
                    FailureRatio = 1.0,
                    MinimumThroughput = 2,
                    SamplingDuration = TimeSpan.FromSeconds(10),
                    BreakDuration = TimeSpan.FromSeconds(3),
                    OnOpened = args =>
                    {
                        Console.WriteLine("  ПОДІЯ: Circuit Breaker перейшов у стан OPEN! Запити блокуються.");
                        return ValueTask.CompletedTask;
                    },
                    OnClosed = args =>
                    {
                        Console.WriteLine("  ПОДІЯ: Circuit Breaker повернувся у стан CLOSED.");
                        return ValueTask.CompletedTask;
                    },
                    OnHalfOpened = args =>
                    {
                        Console.WriteLine("  ПОДІЯ: Circuit Breaker перейшов у стан HALF-OPEN (перевірка відновлення).");
                        return ValueTask.CompletedTask;
                    }
                })
                .Build();

            for (int i = 1; i <= 5; i++)
            {
                Console.WriteLine($"\n  Запит до БД #{i}:");
                try
                {
                    await circuitBreakerPipeline.ExecuteAsync(async cancellationToken =>
                    {
                        _dbAttempts++;
                        if (_dbAttempts <= 3)
                        {
                            throw new InvalidOperationException("З'єднання з БД розірвано.");
                        }

                        Console.WriteLine("  Запит до БД виконано успішно!");
                        await Task.CompletedTask;
                    });
                }
                catch (BrokenCircuitException)
                {
                    Console.WriteLine("  Перехоплено BrokenCircuitException: Запит миттєво відхилено (Circuit Breaker OPEN).");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  Помилка запиту: {ex.Message}");
                }

                await Task.Delay(1000);
            }
        }
    }
}