using System;

namespace lab3v16
{
    public class SensorReader : IDisposable
    {
        private bool _disposed = false;
        private readonly int _sensorId;
        private bool _isReading;

        public int SensorId => _sensorId;
        public bool IsReading => _isReading;

        public SensorReader(int sensorId)
        {
            _sensorId = sensorId;
            _isReading = true;
            Console.WriteLine($"[Sensor {SensorId}] Сенсор підключено, читання розпочато.");
        }

        public void ReadValue()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(SensorReader), $"[Sensor {SensorId}] Помилка: спроба прочитати дані зі звільненого сенсора!");
            }

            Random rand = new Random();
            double value = Math.Round(20.0 + rand.NextDouble() * 10.0, 2);
            Console.WriteLine($"[Sensor {SensorId}] Зчитано значення: {value} °C");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Console.WriteLine($"[Sensor {SensorId}] Звільнення керованих ресурсів.");
                }

                if (_isReading)
                {
                    _isReading = false;
                    Console.WriteLine($"[Sensor {SensorId}] Некерований ресурс звільнено: читання сенсора зупинено.");
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); 
        }

        ~SensorReader()
        {
            Console.WriteLine($"[Sensor {SensorId}] Виклик фіналізатора (~SensorReader)!");
            Dispose(false);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine(" СЦЕНАРІЙ 1: Використання блoку 'using' ");
            using (SensorReader sensor1 = new SensorReader(101))
            {
                sensor1.ReadValue();
                sensor1.ReadValue();
            } 
            Console.WriteLine("Блок 'using' завершено.\n");


            Console.WriteLine(" СЦЕНАРІЙ 2: Створення об'єкта та явний виклик Dispose()");
            SensorReader sensor2 = new SensorReader(102);
            sensor2.ReadValue();
            sensor2.Dispose(); 
            Console.WriteLine("Явний виклик Dispose() завершено.\n");


            Console.WriteLine(" СЦЕНАРІЙ 3: Об'єкт без виклику Dispose() (робота GC та фіналізатора) ");
            CreateUnmanagedSensor();

            Console.WriteLine("Примусовий запуск збирача сміття (GC.Collect)...");
            GC.Collect();
            GC.WaitForPendingFinalizers(); 

            Console.WriteLine("\n Демонстрацію всіх сценаріїв завершено успішно");
        }

        static void CreateUnmanagedSensor()
        {
            SensorReader sensor3 = new SensorReader(103);
            sensor3.ReadValue();
        }
    }
}
