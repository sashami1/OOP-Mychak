using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace lab16v16
{
    public class Flight
    {
        public int Id { get; set; }
        public string FlightNumber { get; set; } = string.Empty;
        public int DepartureAirportId { get; set; }
        public int ArrivalAirportId { get; set; }
        public DateTime DepartureTime { get; set; }
    }

    public class Airport
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var airports = new List<Airport>
            {
                new Airport { Id = 1, Name = "Kyiv Boryspil", City = "Київ" },
                new Airport { Id = 2, Name = "Kyiv Zhuliany", City = "Київ" },
                new Airport { Id = 3, Name = "Lviv Danylo Halytskyi", City = "Львів" },
                new Airport { Id = 4, Name = "Odesa International", City = "Одеса" },
                new Airport { Id = 5, Name = "Kharkiv International", City = "Харків" },
                new Airport { Id = 6, Name = "Warsaw Chopin", City = "Варшава" },
                new Airport { Id = 7, Name = "London Heathrow", City = "Лондон" },
                new Airport { Id = 8, Name = "Frankfurt Airport", City = "Франкфурт" },
                new Airport { Id = 9, Name = "Paris CDG", City = "Париж" },
                new Airport { Id = 10, Name = "Istanbul Airport", City = "Стамбул" }
            };

            var today = DateTime.Today;
            var flights = new List<Flight>
            {
                new Flight { Id = 101, FlightNumber = "PS-101", DepartureAirportId = 1, ArrivalAirportId = 6, DepartureTime = today.AddHours(8) },
                new Flight { Id = 102, FlightNumber = "PS-102", DepartureAirportId = 1, ArrivalAirportId = 7, DepartureTime = today.AddHours(14) },
                new Flight { Id = 103, FlightNumber = "PS-103", DepartureAirportId = 1, ArrivalAirportId = 8, DepartureTime = today.AddHours(16).AddMinutes(30) },
                new Flight { Id = 104, FlightNumber = "PS-104", DepartureAirportId = 1, ArrivalAirportId = 9, DepartureTime = today.AddHours(11) },
                new Flight { Id = 105, FlightNumber = "LH-201", DepartureAirportId = 3, ArrivalAirportId = 8, DepartureTime = today.AddHours(9) },
                new Flight { Id = 106, FlightNumber = "LH-202", DepartureAirportId = 3, ArrivalAirportId = 6, DepartureTime = today.AddHours(15) },
                new Flight { Id = 107, FlightNumber = "TK-301", DepartureAirportId = 4, ArrivalAirportId = 10, DepartureTime = today.AddHours(13) },
                new Flight { Id = 108, FlightNumber = "TK-302", DepartureAirportId = 1, ArrivalAirportId = 10, DepartureTime = today.AddHours(18) },
                new Flight { Id = 109, FlightNumber = "WZZ-401", DepartureAirportId = 2, ArrivalAirportId = 6, DepartureTime = today.AddHours(6) },
                new Flight { Id = 110, FlightNumber = "WZZ-402", DepartureAirportId = 2, ArrivalAirportId = 7, DepartureTime = today.AddHours(17) },
                new Flight { Id = 111, FlightNumber = "RYR-501", DepartureAirportId = 3, ArrivalAirportId = 9, DepartureTime = today.AddHours(10) },
                new Flight { Id = 112, FlightNumber = "PS-105", DepartureAirportId = 1, ArrivalAirportId = 3, DepartureTime = today.AddHours(19).AddMinutes(45) }
            };

            var sw = new Stopwatch();

            Console.WriteLine("Запит 1. Рейси з Kyiv Boryspil після 12:00\n");

            var boryspilId = airports.First(a => a.Name == "Kyiv Boryspil").Id;

            sw.Restart();
            var q1Query = (from f in flights
                           where f.DepartureAirportId == boryspilId && f.DepartureTime.TimeOfDay > new TimeSpan(12, 0, 0)
                           select f).ToList();
            sw.Stop();
            long t1Query = sw.ElapsedTicks;

            sw.Restart();
            var q1Method = flights
                .Where(f => f.DepartureAirportId == boryspilId && f.DepartureTime.TimeOfDay > new TimeSpan(12, 0, 0))
                .ToList();
            sw.Stop();
            long t1Method = sw.ElapsedTicks;

            foreach (var f in q1Method)
            {
                Console.WriteLine($"Рейс {f.FlightNumber}, час відправлення {f.DepartureTime:HH:mm}");
            }
            Console.WriteLine($"Час виконання Query: {t1Query} ticks, Method: {t1Method} ticks\n\n");

            Console.WriteLine("Запит 2. Рейси з містами відправлення та прибуття\n");

            sw.Restart();
            var q2Query = (from f in flights
                           join dep in airports on f.DepartureAirportId equals dep.Id
                           join arr in airports on f.ArrivalAirportId equals arr.Id
                           select new { f.FlightNumber, DepCity = dep.City, ArrCity = arr.City }).ToList();
            sw.Stop();
            long t2Query = sw.ElapsedTicks;

            sw.Restart();
            var q2Method = flights
                .Join(airports, f => f.DepartureAirportId, dep => dep.Id, (f, dep) => new { f, dep })
                .Join(airports, x => x.f.ArrivalAirportId, arr => arr.Id, (x, arr) => new { x.f.FlightNumber, DepCity = x.dep.City, ArrCity = arr.City })
                .ToList();
            sw.Stop();
            long t2Method = sw.ElapsedTicks;

            foreach (var item in q2Method.Take(5))
            {
                Console.WriteLine($"Рейс {item.FlightNumber}: {item.DepCity} в {item.ArrCity}");
            }
            Console.WriteLine($"Показано перші 5 з {q2Method.Count}");
            Console.WriteLine($"Час виконання Query: {t2Query} ticks, Method: {t2Method} ticks\n\n");

            Console.WriteLine("Запит 3. Кількість рейсів з кожного аеропорту\n");

            sw.Restart();
            var q3Query = (from a in airports
                           join f in flights on a.Id equals f.DepartureAirportId into g
                           select new { AirportName = a.Name, Count = g.Count() }).ToList();
            sw.Stop();
            long t3Query = sw.ElapsedTicks;

            sw.Restart();
            var q3Method = airports
                .GroupJoin(flights, a => a.Id, f => f.DepartureAirportId, (a, g) => new { AirportName = a.Name, Count = g.Count() })
                .ToList();
            sw.Stop();
            long t3Method = sw.ElapsedTicks;

            foreach (var item in q3Method)
            {
                Console.WriteLine($"Аеропорт {item.AirportName}: {item.Count} рейсів");
            }
            Console.WriteLine($"Час виконання Query: {t3Query} ticks, Method: {t3Method} ticks\n\n");

            Console.WriteLine("Запит 4. Рейс з найранішим часом відправлення\n");

            sw.Restart();
            var minTimeQuery = (from f in flights select f.DepartureTime).Min();
            var q4Query = (from f in flights where f.DepartureTime == minTimeQuery select f).FirstOrDefault();
            sw.Stop();
            long t4Query = sw.ElapsedTicks;

            sw.Restart();
            var q4Method = flights.OrderBy(f => f.DepartureTime).FirstOrDefault();
            sw.Stop();
            long t4Method = sw.ElapsedTicks;

            if (q4Method != null)
            {
                Console.WriteLine($"Найраніший рейс: {q4Method.FlightNumber} о {q4Method.DepartureTime:HH:mm}");
            }
            Console.WriteLine($"Час виконання Query: {t4Query} ticks, Method: {t4Method} ticks");
        }
    }
}