using System;

namespace Lab2
{
    public class SportTeam
    {
        private string _name = string.Empty;
        private string _coach = string.Empty;
        private int _playersCount;

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public string Coach
        {
            get => _coach;
            set => _coach = value;
        }

        public int PlayersCount
        {
            get => _playersCount;
            set
            {
                if (value <= 0)
                {
                    Console.WriteLine($"[Помилка] Кількість гравців для {Name} має бути більше 0! Встановлено значення 1.");
                    _playersCount = 1;
                }
                else
                {
                    _playersCount = value;
                }
            }
        }

        public SportTeam(string name, string coach, int playersCount)
        {
            Name = name;
            Coach = coach;
            PlayersCount = playersCount;
        }

        public SportTeam() : this("Team A", "Unknown", 11)
        {
        }

        public void PlayMatch(string opponent)
        {
            Console.WriteLine($"Команда '{Name}' під керівництвом тренера {Coach} грає матч проти '{opponent}'! Склад: {PlayersCount} гравців.");
        }

        ~SportTeam()
        {
            Console.WriteLine($"--- Об'єкт команди '{_name}' знищено з пам'яті (працює деструктор) ---");
        }
    }

    internal class Program
    {
        static void CreateAndReleaseTeams()
        {
            SportTeam team1 = new SportTeam();
            team1.PlayMatch("Team B");

            Console.WriteLine();

            SportTeam team2 = new SportTeam("Динамо", "Олександр Шовковський", 23);
            team2.PlayMatch("Шахтар");

            Console.WriteLine();

            SportTeam team3 = new SportTeam("Атлетик", "Павло", -5);
            team3.PlayMatch("Реал");
        }

        static void Main(string[] args)
        {
            Console.WriteLine("=== Демонстрація роботи класу SportTeam ===\n");

            CreateAndReleaseTeams();

            Console.WriteLine("\n=== Демонстрація роботи GC (Збирача сміття) ===");

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Console.WriteLine("\nПрограму завершено.");
        }
    }
}