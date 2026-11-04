using System;
using System.Collections.Generic;
using System.Linq;

namespace IndependentWork_8
{
    public class FileMetadata
    {
        public string FileName { get; set; } = string.Empty;
        public long Size { get; set; }
        public DateTime LastModified { get; set; }
        public string Hash { get; set; } = string.Empty;
    }

    public class FileMetadataCache
    {
        private readonly Dictionary<string, FileMetadata> _cache = new(StringComparer.OrdinalIgnoreCase);

        public void Add(FileMetadata metadata)
        {
            _cache[metadata.FileName] = metadata;
        }

        public FileMetadata Get(string fileName)
        {
            if (_cache.TryGetValue(fileName, out var metadata))
            {
                return metadata;
            }
            throw new KeyNotFoundException($"Метадані для файлу '{fileName}' не знайдені.");
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
            var words = content.Split(new[] { ' ', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
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
            if (_invertedIndex.TryGetValue(word, out var docs))
            {
                return new HashSet<int>(docs);
            }
            return new HashSet<int>();
        }

        public HashSet<int> SearchMultiple(params string[] words)
        {
            if (words == null || words.Length == 0) return new HashSet<int>();

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

            Console.WriteLine("Сценарій 1. Кеш метаданих (Dictionary)\n");
            var cache = new FileMetadataCache();
            cache.Add(new FileMetadata { FileName = "document.pdf", Size = 2048, LastModified = DateTime.Now, Hash = "abc123hash" });
            
            Console.WriteLine($"Чи є document.pdf: {cache.Contains("document.pdf")}");
            var file = cache.Get("document.pdf");
            Console.WriteLine($"Розмір файлу: {file.Size} байт");

            Console.WriteLine("\nСценарій 2. Унікальні теги (HashSet)\n");
            var tagManager = new TagManager();
            tagManager.AddTag("art1", "C#");
            tagManager.AddTag("art1", "LINQ");
            tagManager.AddTag("art2", "LINQ");
            tagManager.AddTag("art2", "DotNet");

            Console.WriteLine($"Усі унікальні теги: {string.Join(", ", tagManager.GetAllUniqueTags())}");
            Console.WriteLine($"Спільні теги для art1 і art2: {string.Join(", ", tagManager.FindCommonTags("art1", "art2"))}");

            Console.WriteLine("\nСценарій 3. Пошуковий індекс (Dictionary + HashSet)\n");
            var index = new SearchIndex();
            index.AddDocument(1, "C# is an awesome programming language");
            index.AddDocument(2, "LINQ in C# makes queries simple");

            var searchResults = index.SearchMultiple("C#", "LINQ");
            Console.WriteLine($"Документи, що містять 'C#' та 'LINQ': {string.Join(", ", searchResults)}");
        }
    }
}