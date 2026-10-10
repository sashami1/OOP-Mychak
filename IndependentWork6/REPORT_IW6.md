# Звіт з аналізу: Інтерфейси vs Абстрактні класи (СР №6)

## 1. Сценарій 1: Система сповіщень

### Обраний підхід: Інтерфейс `INotifier`

### Чому саме він:
1. **Немає спільного коду:** Способи відправки (Email, SMS, Push) абсолютно різні. Їм потрібен лише один спільний метод — `Send()`.
2. **Гнучкість:** Один клас у C# може мати кілька інтерфейсів одночасно (наприклад, надсилати повідомлення і записувати логи).
3. **Простота:** Коду простіше працювати зі сповіщеннями, коли його не цікавлять внутрішні деталі відправки.

```csharp
public interface INotifier
{
    void Send(string message);
}

public class EmailNotifier : INotifier
{
    public void Send(string message) => Console.WriteLine($"[Email] {message}");
}
2. Сценарій 2: Обробка файлів
Обраний підхід: Абстрактний клас FileProcessor
Чому саме він:
Спільний код: Відкриття та закриття файлу однакове для всіх типів (TXT, XML). Ми пишемо цей код один раз у базовому класі, щоб не дублювати його.

Заданий порядок дій: Базовий клас сам вирішує, у якому порядку виконувати дії (спочатку відкрити, потім обробити, потім закрити).
public abstract class FileProcessor
{
    public string FilePath { get; }

    protected FileProcessor(string filePath) => FilePath = filePath;

    public void ProcessFile()
    {
        OpenFile();
        ProcessContent();
        CloseFile();
    }

    private void OpenFile() => Console.WriteLine($"Відкриття {FilePath}...");
    private void CloseFile() => Console.WriteLine($"Закриття {FilePath}.");
    protected abstract void ProcessContent();
}
3. висновки
Інтерфейс обираємо, коли класи абсолютно різні, але вони мають робити одну й ту саму дію (наприклад, "відправити", "надрукувати").

Абстрактний клас обираємо, коли класи схожі між собою і мають спільний готовий код або однаковий алгоритм дій.