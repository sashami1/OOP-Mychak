using System;

namespace lab4v16
{
    // Базовий клас
    class Document
    {
        public string FileName { get; set; }
        public double FileSize { get; set; }

        public Document(string fileName, double fileSize)
        {
            FileName = fileName;
            FileSize = fileSize;
        }

        public virtual void Open()
        {
            Console.WriteLine($"Відкриваємо документ: {FileName} (Розмір: {FileSize} МБ)");
        }

        public string GetDocumentType()
        {
            return "Звичайний документ";
        }
    }

    class TextDocument : Document
    {
        public int WordCount { get; set; }

        public TextDocument(string fileName, double fileSize, int wordCount) 
            : base(fileName, fileSize)
        {
            WordCount = wordCount;
        }

        public override void Open()
        {
            Console.WriteLine($"Відкриття текстового файлу {FileName}. Кількість слів: {WordCount}");
        }

        public void EditContent()
        {
            Console.WriteLine($"Редагуємо текст у файлі {FileName}...");
        }

        public new string GetDocumentType()
        {
            return "Текстовий документ";
        }
    }

    class ImageDocument : Document
    {
        public string Resolution { get; set; }

        public ImageDocument(string fileName, double fileSize, string resolution) 
            : base(fileName, fileSize)
        {
            Resolution = resolution;
        }

        public override void Open()
        {
            Console.WriteLine($"Показ картинки {FileName}. Роздільна здатність: {Resolution}");
        }

        public void ResizeImage()
        {
            Console.WriteLine($"Змінюємо розмір зображення {FileName}...");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            TextDocument txt = new TextDocument("lab_report.docx", 1.5, 450);
            ImageDocument img = new ImageDocument("photo.png", 3.2, "1920x1080");

            Console.WriteLine(" Виклики власних методів");
            txt.EditContent();
            img.ResizeImage();

            Console.WriteLine("\n Демонстрація поліморфізму (override)");
            Document[] docs = new Document[]
            {
                new Document("file.dat", 0.5),
                txt,
                img
            };

            foreach (var doc in docs)
            {
                doc.Open();
            }

            Console.WriteLine("\n Демонстрація різниці між override та new");
            
            TextDocument txtRef = txt;
            Console.WriteLine("Виклик через TextDocument: " + txtRef.GetDocumentType());

            Document docRef = txt;
            Console.WriteLine("Виклик через Document: " + docRef.GetDocumentType());
        }
    }
}
