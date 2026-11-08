using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace IndependentWork_10
{
    public class Document
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

        public override string ToString() => $"[{Id}] {Title}";
    }

    public static class TextProcessor
    {
        private static readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase)
        {
            "a", "an", "the", "is", "are", "was", "were",
            "in", "on", "at", "to", "for", "of", "and", "or"
        };

        public static IEnumerable<string> Tokenize(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return Enumerable.Empty<string>();
            
            var matches = Regex.Matches(text.ToLowerInvariant(), @"\w+");
            return matches.Select(m => m.Value);
        }

        public static IEnumerable<string> RemoveStopWords(IEnumerable<string> words)
        {
            return words.Where(w => !StopWords.Contains(w));
        }
    }

    public class InvertedIndex
    {
        private readonly Dictionary<string, HashSet<int>> _index = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<int, Document> _documents = new();

        public void AddDocument(Document doc)
        {
            _documents[doc.Id] = doc;
            var tokens = TextProcessor.Tokenize(doc.Content);

            foreach (var token in tokens)
            {
                if (!_index.ContainsKey(token))
                {
                    _index[token] = new HashSet<int>();
                }
                _index[token].Add(doc.Id);
            }
        }

        public List<Document> SearchSingleWord(string word)
        {
            var normalizedWord = word.ToLowerInvariant();
            if (_index.TryGetValue(normalizedWord, out var docIds))
            {
                return docIds.Select(id => _documents[id]).ToList();
            }
            return new List<Document>();
        }

        public List<Document> SearchMultipleWords(params string[] words)
        {
            if (words == null || words.Length == 0) return new List<Document>();

            HashSet<int>? resultSet = null;

            foreach (var word in words)
            {
                var normalizedWord = word.ToLowerInvariant();
                var docIds = _index.TryGetValue(normalizedWord, out var ids) ? ids : new HashSet<int>();

                if (resultSet == null)
                {
                    resultSet = new HashSet<int>(docIds);
                }
                else
                {
                    resultSet.IntersectWith(docIds);
                }
            }

            return (resultSet ?? new HashSet<int>()).Select(id => _documents[id]).ToList();
        }

        public List<Document> SearchAnyWord(params string[] words)
        {
            if (words == null || words.Length == 0) return new List<Document>();

            var resultSet = new HashSet<int>();

            foreach (var word in words)
            {
                var normalizedWord = word.ToLowerInvariant();
                if (_index.TryGetValue(normalizedWord, out var ids))
                {
                    resultSet.UnionWith(ids);
                }
            }

            return resultSet.Select(id => _documents[id]).ToList();
        }

        public int DocumentCount => _documents.Count;
        public int UniqueWords => _index.Keys.Count;
    }

    public class PerformanceTest
    {
        public static void RunTests()
        {
            Console.WriteLine("Вимірювання продуктивності (Лінійний пошук vs Inverted Index) \n");

            TestForDocumentCount(10);
            TestForDocumentCount(1000);
        }

        private static void TestForDocumentCount(int count)
        {
            var documents = GenerateDocuments(count);
            string searchWord = "programming";

            var sw = new Stopwatch();

            sw.Start();
            var normalizedWord = searchWord.ToLowerInvariant();
            var linearResults = documents.Where(d => 
                TextProcessor.Tokenize(d.Content).Contains(normalizedWord)).ToList();
            sw.Stop();
            long linearTicks = sw.ElapsedTicks;

            var index = new InvertedIndex();
            foreach (var doc in documents)
            {
                index.AddDocument(doc);
            }

            sw.Restart();
            var indexResults = index.SearchSingleWord(searchWord);
            sw.Stop();
            long indexTicks = sw.ElapsedTicks;

            Console.WriteLine($"[Обсяг: {count} документів]");
            Console.WriteLine($" Лінійний пошук: {linearTicks} ticks");
            Console.WriteLine($" Inverted Index:  {indexTicks} ticks");
            Console.WriteLine($" Знайдено елементів: Linear={linearResults.Count}, Index={indexResults.Count}\n");
        }

        private static List<Document> GenerateDocuments(int count)
        {
            var baseDocs = GetBaseDocuments();
            var list = new List<Document>();

            for (int i = 1; i <= count; i++)
            {
                var sample = baseDocs[(i - 1) % baseDocs.Count];
                list.Add(new Document
                {
                    Id = i,
                    Title = $"{sample.Title} #{i}",
                    Content = $"{sample.Content} Extra tag_{i % 10}"
                });
            }
            return list;
        }

        public static List<Document> GetBaseDocuments()
        {
            return new List<Document>
            {
                new Document { Id = 1, Title = "Introduction to C#", Content = "C# is a modern object-oriented programming language developed by Microsoft." },
                new Document { Id = 2, Title = "Java Basics", Content = "Java is a popular programming language known for its portability and performance." },
                new Document { Id = 3, Title = "Python Overview", Content = "Python is a high-level programming language with simple syntax." },
                new Document { Id = 4, Title = "C# Advanced Features", Content = "C# supports LINQ, async/await, and many modern programming paradigms." },
                new Document { Id = 5, Title = "Comparing Languages", Content = "C#, Java, and Python are among the most popular programming languages today." }
            };
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            PerformanceTest.RunTests();

            var index = new InvertedIndex();
            foreach (var doc in PerformanceTest.GetBaseDocuments())
            {
                index.AddDocument(doc);
            }

            Console.WriteLine($"Завантажено {index.DocumentCount} базових документів.");
            Console.WriteLine($"Індекс містить {index.UniqueWords} унікальних слів.\n");

            Console.WriteLine(" Демонстрація пошуку (AND) ");
            var queryWords = new[] { "programming", "language" };
            var results = index.SearchMultipleWords(queryWords);

            Console.WriteLine($"Запит: '{string.Join(" ", queryWords)}'");
            foreach (var doc in results)
            {
                Console.WriteLine($" {doc}");
            }
            Console.WriteLine($"Знайдено документів: {results.Count}\n");

            Console.WriteLine(" Демонстрація пошуку (OR) ");
            var queryOr = new[] { "async", "python" };
            var resultsOr = index.SearchAnyWord(queryOr);

            Console.WriteLine($"Запит: '{string.Join(" ", queryOr)}'");
            foreach (var doc in resultsOr)
            {
                Console.WriteLine($" {doc}");
            }
            Console.WriteLine($"Знайдено документів: {resultsOr.Count}");
        }
    }
}