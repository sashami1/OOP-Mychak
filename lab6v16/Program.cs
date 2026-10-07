using System;

namespace Lab6Variant16
{
    public class Document
    {

        private string _fileName;
        private long _fileSize; 

        public string FileName
        {
            get => _fileName;
            set => _fileName = value;
        }

        public long FileSize
        {
            get => _fileSize;
            set => _fileSize = value;
        }

        public Document(string fileName, long fileSize)
        {
            _fileName = fileName;
            _fileSize = fileSize;
        }

        public virtual void Open()
        {
            Console.WriteLine($"[Document] Відкриття документа: {FileName} ({FileSize} байт)");
        }

        public string GetDocumentType()
        {
            return "Загальний документ";
        }
    }

    public class TextDocument : Document
    {
        public int WordCount { get; set; }

        public TextDocument(string fileName, long fileSize, int wordCount)
            : base(fileName, fileSize)
        {
            WordCount = wordCount;
        }

        public override void Open()
        {
            Console.WriteLine($"[TextDocument] Відкриття текстового документа: {FileName} ({FileSize} байт, Слів: {WordCount})");
        }

        public void EditContent()
        {
            Console.WriteLine($"[TextDocument] Редагування вмісту {FileName}...");
        }

        public new string GetDocumentType()
        {
            return "Текстовий документ";
        }
    }

    public class ImageDocument : Document
    {
        public string Resolution { get; set; } 

        public ImageDocument(string fileName, long fileSize, string resolution)
            : base(fileName, fileSize)
        {
            Resolution = resolution;
        }

        public override void Open()
        {
            Console.WriteLine($"[ImageDocument] Перегляд зображення: {FileName} ({FileSize} байт, Роздільна здатність: {Resolution})");
        }

        public void ResizeImage()
        {
            Console.WriteLine($"[ImageDocument] Зміна розміру зображення {FileName}...");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine(" 1. Створення об'єктів та виклик власних методів ");
            Document baseDoc = new Document("generic.dat", 1024);
            TextDocument textDoc = new TextDocument("report.docx", 20480, 500);
            ImageDocument imgDoc = new ImageDocument("photo.jpg", 5242880, "3840x2160");

            textDoc.EditContent();
            imgDoc.ResizeImage();
            Console.WriteLine();

            Console.WriteLine(" 2. Демонстрація Поліморфізму (override) ");
            Document[] documents = new Document[]
            {
                baseDoc,
                textDoc,
                imgDoc
            };

            foreach (var doc in documents)
            {
                doc.Open();
            }
            Console.WriteLine();

            Console.WriteLine(" 3. Демонстрація різниці між override та new ");
            
            Console.WriteLine($"textDoc.GetDocumentType(): {textDoc.GetDocumentType()}"); 

            Document docRef = textDoc;
            Console.WriteLine($"docRef.GetDocumentType():   {docRef.GetDocumentType()}"); 

            Console.WriteLine("\n--- Порівняння з override (Open) ---");
            docRef.Open(); 
        }
    }
}