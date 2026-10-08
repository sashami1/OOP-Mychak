using System;
using System.Collections.Generic;

namespace IndependentWork4
{
    public class LibraryItem
    {
        private string _uniqueId;
        private string _title;
        private int _publicationYear;

        public string UniqueId => _uniqueId;
        public string Title => _title;
        public int PublicationYear => _publicationYear;

        public LibraryItem(string uniqueId, string title, int publicationYear)
        {
            _uniqueId = uniqueId;
            _title = title;
            _publicationYear = publicationYear;
        }

        public virtual void DisplayDetails()
        {
            Console.WriteLine($"[ID: {UniqueId}] Назва: '{Title}', Рік видання: {PublicationYear}");
        }
    }

    public class Book : LibraryItem
    {
        private string _author;

        public string Author => _author;

        public Book(string uniqueId, string title, int publicationYear, string author)
            : base(uniqueId, title, publicationYear)
        {
            _author = author;
        }

        public override void DisplayDetails()
        {
            Console.WriteLine($"[Книга] ID: {UniqueId} | Назва: '{Title}' | Автор: {Author} | Рік: {PublicationYear}");
        }
    }

    public class Journal : LibraryItem
    {
        private int _issueNumber;

        public int IssueNumber => _issueNumber;

        public Journal(string uniqueId, string title, int publicationYear, int issueNumber)
            : base(uniqueId, title, publicationYear)
        {
            _issueNumber = issueNumber;
        }

        public override void DisplayDetails()
        {
            Console.WriteLine($"[Журнал] ID: {UniqueId} | Назва: '{Title}' | Випуск №{IssueNumber} | Рік: {PublicationYear}");
        }
    }

    public class AudioBook : Book
    {
        private string _narrator;

        public string Narrator => _narrator;

        public AudioBook(string uniqueId, string title, int publicationYear, string author, string narrator)
            : base(uniqueId, title, publicationYear, author)
        {
            _narrator = narrator;
        }

        public override void DisplayDetails()
        {
            Console.WriteLine($"[Аудіокнига] ID: {UniqueId} | Назва: '{Title}' | Автор: {Author} | Оповідач: {Narrator} | Рік: {PublicationYear}");
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine(" Демонстрація роботи ієрархії LibraryItem \n");

            Book book = new Book("B-001", "Тіні забутих предків", 1911, "Михайло Коцюбинський");
            Journal journal = new Journal("J-102", "Наука і техніка", 2023, 4);
            AudioBook audioBook = new AudioBook("AB-501", "Кобзар", 2020, "Тарас Шевченко", "Олександр Бондаренко");

            List<LibraryItem> items = new List<LibraryItem> { book, journal, audioBook };

            foreach (var item in items)
            {
                item.DisplayDetails();
            }
        }
    }
}