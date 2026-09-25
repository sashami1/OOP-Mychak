using System;
using System.Collections.Generic;

namespace lab8v16
{
    class User
    {
        public string Username { get; set; }

        public User(string username)
        {
            Username = username;
        }

        public virtual void GetPermissions()
        {
            Console.WriteLine($"Користувач: {Username}");
            Console.WriteLine("  - Перегляд сайта");
        }
    }

    class Admin : User
    {
        public string Department { get; set; }

        public Admin(string username, string department) : base(username)
        {
            Department = department;
        }

        public override void GetPermissions()
        {
            Console.WriteLine($"Адмін: {Username} (Відділ: {Department})");
            Console.WriteLine("  - Перегляд сайта");
            Console.WriteLine("  - Управління користувачами");
            Console.WriteLine("  - Доступ до системних налаштувань");
        }
    }

    class Moderator : User
    {
        public string ForumName { get; set; }

        public Moderator(string username, string forumName) : base(username)
        {
            ForumName = forumName;
        }

        public override void GetPermissions()
        {
            Console.WriteLine($"Модератор: {Username} (Форум: {ForumName})");
            Console.WriteLine("  - Перегляд сайта");
            Console.WriteLine("  - Редагування постів");
            Console.WriteLine("  - Блокування спамерів");
        }
    }

    class Guest : User
    {
        public string SessionId { get; set; }

        public Guest(string username, string sessionId) : base(username)
        {
            SessionId = sessionId;
        }

        public override void GetPermissions()
        {
            Console.WriteLine($"Гість: {Username} (ID сесії: {SessionId})");
            Console.WriteLine("  - Тільки перегляд публічних сторінок");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            List<User> users = new List<User>();

            users.Add(new Admin("admin_kate", "IT-відділ"));
            users.Add(new Moderator("mod_alex", "Новини та обговорення"));
            users.Add(new Guest("guest_user", "SESS-10293"));
            users.Add(new Admin("super_admin", "Адміністрація"));

            Console.WriteLine("=== Список користувачів та їхні права ===\n");

            int count = 0;
            foreach (User user in users)
            {
                user.GetPermissions();
                Console.WriteLine();
                count++;
            }

            Console.WriteLine($"Всього оброблено користувачів у системі: {count}");
        }
    }
}