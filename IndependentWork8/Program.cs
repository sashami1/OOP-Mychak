using System;
using System.Collections.Generic;
using System.Linq;

namespace IndependentWork8
{
    public class FileMetadata
    {
        public string FileName { get; set; } = string.Empty;
        public long Size { get; set; }
        public DateTime LastModified { get; set; }
        public string Hash { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"File: {FileName}, Size: {Size} bytes, Modified: {LastModified:g}, Hash: {Hash}";
        }
    }

    public class FileMetadataCache
    {
        private readonly Dictionary<string, FileMetadata> _cache = new(StringComparer.OrdinalIgnoreCase);

        public void Add(FileMetadata metadata)
        {
            _cache[metadata.FileName] = metadata;
        }

        public FileMetadata? Get(string fileName)
        {
            _cache.TryGetValue(fileName, out var metadata);
            return metadata;
        }

        public bool Remove(string fileName)
        {
            return _cache.Remove(fileName);
        }

        public bool Contains(string fileName)
        {
            return _cache.ContainsKey(fileName);
        }
    }

    public class TagManager
    {
        private readonly Dictionary<string, HashSet<string>> _articleTags = new();

        public void AddTag(string articleId, string tag)
        {
            if (!_articleTags.ContainsKey(articleId))
            {
                _articleTags[articleId] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }
            _articleTags[articleId].Add(tag);
        }

        public void RemoveTag(string articleId, string tag)
        {
            if (_articleTags.TryGetValue(articleId, out var tags))
            {
                tags.Remove(tag);
            }
        }

        public HashSet<string> GetAllUniqueTags()
        {
            var uniqueTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var tags in _articleTags.Values)
            {
                uniqueTags.UnionWith(tags);
            }
            return uniqueTags;
        }

        public HashSet<string> FindCommonTags(string articleId1, string articleId2)
        {
            if (_articleTags.TryGetValue(articleId1, out var tags1) &&
                _articleTags.TryGetValue(articleId2, out var tags2))
            {
                var common = new HashSet<string>(tags1, StringComparer.OrdinalIgnoreCase);
                common.IntersectWith(tags2);
                return common;
            }
            return new HashSet<string>();
        }
    }

    public class SearchIndex
    {
        private readonly Dictionary<string, HashSet<int>> _invertedIndex = new(StringComparer.OrdinalIgnoreCase);

        public void AddDocument(int docId, string content)
        {
            var words = content.Split(new[] { ' ', ',', '.', '!', '?', '-', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in words)
            {
                if (!_invertedIndex.ContainsKey(word))
                {
                    _invertedIndex[word] = new HashSet<int>();
                }
                _invertedIndex[word].Add(docId);
            }
        }

        public HashSet<int> Search(string word)
        {
            if (_invertedIndex.TryGetValue(word, out var docIds))
            {
                return new HashSet<int>(docIds);
            }
            return new HashSet<int>();
        }

        public HashSet<int> SearchMultiple(params string[] words)
        {
            if (words == null || words.Length == 0)
                return new HashSet<int>();

            HashSet<int>? result = null;

            foreach (var word in words)
            {
                var docs = Search(word);
                if (result == null)
                {
                    result = new HashSet<int>(docs);
                }
                else
                {
                    result.IntersectWith(docs);
                }
            }

            return result ?? new HashSet<int>();
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== Сценарій 1: Кеш метаданих (Dictionary) ===");
            var cache = new FileMetadataCache();
            cache.Add(new FileMetadata { FileName = "document.pdf", Size = 2048, LastModified = DateTime.Now, Hash = "A1B2C3" });
            cache.Add(new FileMetadata { FileName = "image.jpg", Size = 51200, LastModified = DateTime.Now, Hash = "D4E5F6" });

            Console.WriteLine($"Чи є image.jpg у кеші: {cache.Contains("image.jpg")}");
            Console.WriteLine($"Отримано з кешу: {cache.Get("document.pdf")}");
            cache.Remove("document.pdf");
            Console.WriteLine($"Чи є document.pdf після видалення: {cache.Contains("document.pdf")}");

            Console.WriteLine("\n Сценарій 2: Унікальні теги (HashSet)");
            var tagManager = new TagManager();
            tagManager.AddTag("art1", "C#");
            tagManager.AddTag("art1", ".NET");
            tagManager.AddTag("art1", "OOP");

            tagManager.AddTag("art2", ".NET");
            tagManager.AddTag("art2", "Architecture");

            Console.WriteLine($"Усі унікальні теги: {string.Join(", ", tagManager.GetAllUniqueTags())}");
            Console.WriteLine($"Спільні теги для art1 та art2: {string.Join(", ", tagManager.FindCommonTags("art1", "art2"))}");

            Console.WriteLine("\n Сценарій 3: Індекс для пошуку (Dictionary + HashSet)");
            var index = new SearchIndex();
            index.AddDocument(1, "C# is an object-oriented programming language");
            index.AddDocument(2, ".NET framework supports C# language");
            index.AddDocument(3, "Object-oriented architecture in .NET");

            Console.WriteLine($"Документи зі словом 'language': {string.Join(", ", index.Search("language"))}");
            Console.WriteLine($"Документи зі словами 'C#' ТА 'language': {string.Join(", ", index.SearchMultiple("C#", "language"))}");
        }
    }
}
