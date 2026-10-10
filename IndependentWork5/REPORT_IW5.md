# Звіт з аналізу поліморфізму в .NET (Самостійна робота №5)

## 1. Поліморфізм через базовий клас `System.Object`

### Опис та приклад
Усі типи даних у C# успадковуються від `System.Object`, що дозволяє зберігати різні об'єкти в єдиній колекції `List<object>`. Метод `ToString()` є віртуальним у `System.Object`, а класи `Product`, `Customer` та `Order` перевизначають його (`override`).

```csharp
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
Переваги
Уніфікована обробка: Можливість працювати з різнорідними об'єктами через єдиний базовий тип object.

Динамічне зв'язування: Викликається конкретна реалізація ToString() для кожного об'єкта під час виконання програми.

2. Поліморфізм через інтерфейс IEnumerable<T>
Опис та приклад
Інтерфейс IEnumerable<T> надає контракт для перебору елементів колекції. Реалізація методу GetEnumerator() у класі MyCustomCollection дозволяє використовувати синтаксичну конструкцію foreach.
public class MyCustomCollection : IEnumerable<string>
{
    private List<string> _items = new List<string>();

    public void Add(string item) => _items.Add(item);
    public IEnumerator<string> GetEnumerator() => _items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

// Використання
MyCustomCollection collection = new MyCustomCollection();
collection.Add("Елемент 1");

foreach (string item in collection)
{
    Console.WriteLine(item);
}
Переваги
Сховання внутрішньої структури: Зовнішній код не знає, як саме зберігаються дані (масив, список або зв'язаний список), але може їх перебирати.

Стандартизація: Усі колекції в .NET реалізують цей інтерфейс, що дає змогу писати універсальні алгоритми.

3. Поліморфізм через абстрактний клас System.IO.Stream
Опис та приклад
Абстрактний клас Stream визначає спільний інтерфейс для роботи з потоками даних. Метод ProcessStream(Stream stream) приймає абстрактне посилання та працює як з FileStream, так і з MemoryStream.
public static void ProcessStream(Stream stream)
{
    byte[] buffer = new byte[16];
    int bytesRead = stream.Read(buffer, 0, buffer.Length);
    string result = Encoding.UTF8.GetString(buffer, 0, bytesRead);
    Console.WriteLine($"Прочитано {bytesRead} байт з {stream.GetType().Name}: {result}");
}
Переваги
Гнучкість та розширюваність: Метод ProcessStream працює з будь-яким джерелом даних (файл, оперативна пам'ять, мережевий сокет), яке успадковується від Stream.

Дотримання Open/Closed Principle: Додавання нових типів потоків не вимагає зміни коду методу ProcessStream.

4. Відповіді на контрольні запитання
Як перевизначення ToString() ілюструє поліморфізм?
Викликаючи obj.ToString() для змінної типу object, середовище виконання .NET визначає фактичний тип об'єкта в пам'яті та викликає його перевизначену версію.

Чому інтерфейс IEnumerable є ключовим для поліморфної роботи з колекціями?
Він виступає контрактом, який гарантує наявність ітератора. Це дозволяє циклу foreach працювати з будь-яким об'єктом, що імплементує цей інтерфейс, незалежно від його внутрішньої реалізації.

Яку роль відіграє абстрактний клас Stream у забезпеченні уніфікованої роботи з даними?
Stream задає спільні абстрактні методи (Read, Write, Seek), що дозволяє обробляти дані однаково, незалежно від того, де вони знаходяться (у файлі, пам'яті чи мережі).