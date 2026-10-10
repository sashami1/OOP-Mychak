using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace InvertedIndexDemo
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
        public static IEnumerable<string> Tokenize(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return Enumerable.Empty<string>();

            var matches = Regex.Matches(text.ToLowerInvariant(), @"\w+");
            return matches.Select(m => m.Value);
        }

        public static IEnumerable<string> RemoveStopWords(IEnumerable<string> words)
        {
            var stopWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "a", "an", "the", "is", "are", "was", "were",
                "in", "on", "at", "to", "for", "of", "and", "or"
            };
            return words.Where(w => !stopWords.Contains(w));
        }
    }

    public class InvertedIndex
    {
        private readonly Dictionary<string, HashSet<int>> _index = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<int, Document> _documents = new();

        public void AddDocument(Document doc)
        {
            _documents[doc.Id] = doc;

            var tokens = TextProcessor.RemoveStopWords(TextProcessor.Tokenize(doc.Content));
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
            if (words == null || words.Length == 0)
                return new List<Document>();

            HashSet<int>? resultSet = null;

            foreach (var rawWord in words)
            {
                var word = rawWord.ToLowerInvariant();
                if (_index.TryGetValue(word, out var docIds))
                {
                    if (resultSet == null)
                    {
                        resultSet = new HashSet<int>(docIds);
                    }
                    else
                    {
                        resultSet.IntersectWith(docIds);
                    }
                }
                else
                {
                    return new List<Document>();
                }
            }

            return resultSet != null
                ? resultSet.Select(id => _documents[id]).ToList()
                : new List<Document>();
        }

        public List<Document> SearchAnyWord(params string[] words)
        {
            if (words == null || words.Length == 0)
                return new List<Document>();

            var resultSet = new HashSet<int>();

            foreach (var rawWord in words)
            {
                var word = rawWord.ToLowerInvariant();
                if (_index.TryGetValue(word, out var docIds))
                {
                    resultSet.UnionWith(docIds);
                }
            }

            return resultSet.Select(id => _documents[id]).ToList();
        }

        public int DocumentCount => _documents.Count;
        public int UniqueWords => _index.Keys.Count;
    }

    public class PerformanceTest
    {
        public static List<Document> GenerateDocuments(int count)
        {
            var baseDocs = GetBaseDocuments();
            var list = new List<Document>();

            for (int i = 1; i <= count; i++)
            {
                var template = baseDocs[(i - 1) % baseDocs.Count];
                list.Add(new Document
                {
                    Id = i,
                    Title = $"{template.Title} #{i}",
                    Content = $"{template.Content} Additional random token sequence {i} for performance testing."
                });
            }

            return list;
        }

        public static List<Document> GetBaseDocuments()
        {
            return new List<Document>
            {
                new Document
                {
                    Id = 1,
                    Title = "Introduction to C#",
                    Content = "C# is a modern object-oriented programming language developed by Microsoft."
                },
                new Document
                {
                    Id = 2,
                    Title = "Java Basics",
                    Content = "Java is a popular programming language known for its portability and performance."
                },
                new Document
                {
                    Id = 3,
                    Title = "Python Overview",
                    Content = "Python is a high-level programming language with simple syntax."
                },
                new Document
                {
                    Id = 4,
                    Title = "C# Advanced Features",
                    Content = "C# supports LINQ, async/await, and many modern programming paradigms."
                },
                new Document
                {
                    Id = 5,
                    Title = "Comparing Languages",
                    Content = "C#, Java, and Python are among the most popular programming languages today."
                }
            };
        }

        public static void RunBenchmarks()
        {
            Console.WriteLine("\nВИМІРЮВАННЯ ПРОДУКТИВНОСТІ (Benchmark Results)");

            int[] sizes = { 10, 100, 1000, 10000 };
            string searchWord = "programming";

            Console.WriteLine($"{"Кількість",-12} | {"Лінійний (Ticks)",-20} | {"Inverted Index (Ticks)",-22}");

            foreach (var size in sizes)
            {
                var docs = GenerateDocuments(size);
                var sw = new Stopwatch();

                var normalizedWord = searchWord.ToLowerInvariant();
                sw.Start();
                var linearResults = docs.Where(d =>
                    TextProcessor.Tokenize(d.Content).Contains(normalizedWord)).ToList();
                sw.Stop();
                long linearTicks = sw.ElapsedTicks;

                var index = new InvertedIndex();
                foreach (var doc in docs)
                    index.AddDocument(doc);

                sw.Restart();
                var indexResults = index.SearchSingleWord(searchWord);
                sw.Stop();
                long indexTicks = sw.ElapsedTicks;

                Console.WriteLine($"{size,-12} | {linearTicks,-20} | {indexTicks,-22}");
            }
            Console.WriteLine();
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            PerformanceTest.RunBenchmarks();

            var index = new InvertedIndex();
            var baseDocs = PerformanceTest.GetBaseDocuments();

            foreach (var doc in baseDocs)
            {
                index.AddDocument(doc);
            }

            Console.WriteLine($"Loaded {index.DocumentCount} documents");
            Console.WriteLine($"Index contains {index.UniqueWords} unique words\n");

            while (true)
            {
                Console.Write("Enter search query (or 'exit'): ");
                var query = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(query) || query.Trim().ToLower() == "exit")
                    break;

                var words = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                Console.WriteLine("\nSearch Results (AND)");
                var results = index.SearchMultipleWords(words);

                foreach (var doc in results)
                {
                    Console.WriteLine(doc);
                    string snippet = doc.Content.Length > 100
                        ? doc.Content.Substring(0, 100) + "..."
                        : doc.Content;
                    Console.WriteLine($"   {snippet}");
                }

                Console.WriteLine($"\nFound {results.Count} documents\n");
            }
        }
    }
}