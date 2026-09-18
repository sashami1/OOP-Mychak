using System;

class SportTeam
{
    // Приватні поля
    private string name;
    private string coach;

    // Публічна властивість
    public int PlayersCount { get; set; }

    // Конструктор
    public SportTeam(string name, string coach, int playersCount)
    {
        this.name = name;
        this.coach = coach;
        PlayersCount = playersCount;
    }

    // Метод для проведення матчу
    public void PlayMatch()
    {
        Console.WriteLine($"Команда {name} під керівництвом тренера {coach} грає матч.");
        Console.WriteLine($"Кількість гравців: {PlayersCount}");
    }
}

class Program
{
    static void Main()
    {
        // Створення об'єктів
        SportTeam team1 = new SportTeam("Динамо", "Олександр", 11);
        SportTeam team2 = new SportTeam("Шахтар", "Михайло", 11);
        SportTeam team3 = new SportTeam("Карпати", "Андрій", 11);

        // Виклик методу PlayMatch()
        team1.PlayMatch();
        Console.WriteLine();

        team2.PlayMatch();
        Console.WriteLine();

        team3.PlayMatch();
    }
}
