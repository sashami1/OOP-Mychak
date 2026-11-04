using System;
using System.Collections.Generic;
using System.Linq;

namespace lab17v16
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
    }

    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Course> Courses { get; set; } = new List<Course>();
    }

    public class University
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Student> Students { get; set; } = new List<Student>();
    }

    public static class StudentExtensions
    {
        public static bool IsEnrolledIn(this Student student, string courseTitle)
        {
            if (student == null || student.Courses == null) return false;
            return student.Courses.Any(c => c.Title.Equals(courseTitle, StringComparison.OrdinalIgnoreCase));
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var c1 = new Course { Id = 1, Title = "Вища математика" };
            var c2 = new Course { Id = 2, Title = "Об'єктно-орієнтоване програмування" };
            var c3 = new Course { Id = 3, Title = "Бази даних" };
            var c4 = new Course { Id = 4, Title = "Веб-технології" };
            var c5 = new Course { Id = 5, Title = "Алгоритми і структури даних" };

            var universities = new List<University>
            {
                new University
                {
                    Id = 1,
                    Name = "Національний університет",
                    Students = new List<Student>
                    {
                        new Student { Id = 101, Name = "Олександр Іваненко", Courses = new List<Course> { c1, c2 } },
                        new Student { Id = 102, Name = "Марія Петренко", Courses = new List<Course> { c2, c3 } },
                        new Student { Id = 103, Name = "Іван Васильченко", Courses = new List<Course> { c1, c4 } },
                        new Student { Id = 104, Name = "Олена Ковальчук", Courses = new List<Course> { c3, c5 } }
                    }
                },
                new University
                {
                    Id = 2,
                    Name = "Технічний інститут",
                    Students = new List<Student>
                    {
                        new Student { Id = 201, Name = "Дмитро Сидоренко", Courses = new List<Course> { c2, c5 } },
                        new Student { Id = 202, Name = "Анна Бондаренко", Courses = new List<Course> { c1, c3, c4 } },
                        new Student { Id = 203, Name = "Павло Мороз", Courses = new List<Course> { c4 } },
                        new Student { Id = 204, Name = "Софія Шевченко", Courses = new List<Course> { c1, c2, c5 } }
                    }
                }
            };

            Console.WriteLine("Запит 1. Усі курси всіх студентів всіх університетів (SelectMany)\n");

            var allCourses = universities
                .SelectMany(u => u.Students)
                .SelectMany(s => s.Courses)
                .Select(c => c.Title)
                .Distinct()
                .ToList();

            foreach (var course in allCourses)
            {
                Console.WriteLine($"Курс: {course}");
            }

            Console.WriteLine("\nЗапит 2. Пагінація студентів (по 5 студентів на сторінку)\n");

            var allStudents = universities.SelectMany(u => u.Students).ToList();
            int pageSize = 5;
            int totalPages = (int)Math.Ceiling((double)allStudents.Count / pageSize);

            for (int page = 1; page <= totalPages; page++)
            {
                Console.WriteLine($"Сторінка {page}:");
                var pageStudents = allStudents.Skip((page - 1) * pageSize).Take(pageSize);

                foreach (var student in pageStudents)
                {
                    Console.WriteLine($" Студент: {student.Name}");
                }
                Console.WriteLine();
            }

            Console.WriteLine("Запит 3. Перевірка методу розширення IsEnrolledIn\n");

            var testStudent = allStudents.First(s => s.Name == "Олександр Іваненко");
            string searchCourse1 = "Об'єктно-орієнтоване програмування";
            string searchCourse2 = "Бази даних";

            Console.WriteLine($"Студент: {testStudent.Name}");
            Console.WriteLine($"Записаний на '{searchCourse1}': {testStudent.IsEnrolledIn(searchCourse1)}");
            Console.WriteLine($"Записаний на '{searchCourse2}': {testStudent.IsEnrolledIn(searchCourse2)}");
        }
    }
}