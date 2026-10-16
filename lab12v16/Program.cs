using System;
using System.Collections.Generic;
using System.Linq;

namespace lab12v16
{
    public interface IEntity
    {
        int Id { get; set; }
    }

    public class BlogPost : IEntity
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int AuthorId { get; set; }
    }

    public interface IRepository<T> where T : class, IEntity
    {
        T GetById(int id);
        IEnumerable<T> GetAll();
        void Add(T item);
        void Update(T item);
        void Delete(int id);
    }

    public class InMemoryRepository<T> : IRepository<T> where T : class, IEntity
    {
        private readonly List<T> _items = new();

        public void Add(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            _items.Add(item);
        }

        public T GetById(int id)
        {
            var item = _items.FirstOrDefault(x => x.Id == id);
            if (item == null)
                throw new KeyNotFoundException($"Елемент з ID {id} не знайдено.");

            return item;
        }

        public IEnumerable<T> GetAll()
        {
            return _items.ToList();
        }

        public void Update(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var index = _items.FindIndex(x => x.Id == item.Id);
            if (index == -1)
                throw new KeyNotFoundException($"Елемент з ID {item.Id} не знайдено для оновлення.");

            _items[index] = item;
        }

        public void Delete(int id)
        {
            var item = _items.FirstOrDefault(x => x.Id == id);
            if (item == null)
                throw new KeyNotFoundException($"Елемент з ID {id} не знайдено для видалення.");

            _items.Remove(item);
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var repository = new InMemoryRepository<BlogPost>();

            var post1 = new BlogPost { Id = 1, Title = "Вступ до Generics", Content = "Узагальнення в C#...", AuthorId = 101 };
            var post2 = new BlogPost { Id = 2, Title = "Юніт-тести з xUnit", Content = "Тестування коду...", AuthorId = 102 };

            repository.Add(post1);
            repository.Add(post2);

            Console.WriteLine($"Додано публікацій: {repository.GetAll().Count()}");

            var fetchedPost = repository.GetById(1);
            Console.WriteLine($"Отримано за ID 1: {fetchedPost.Title}");

            fetchedPost.Title = "Вступ до Generics у C# (Оновлено)";
            repository.Update(fetchedPost);
            Console.WriteLine($"Оновлена назва: {repository.GetById(1).Title}");

            repository.Delete(2);
            Console.WriteLine($"Залишилося публікацій після видалення: {repository.GetAll().Count()}");
        }
    }
}