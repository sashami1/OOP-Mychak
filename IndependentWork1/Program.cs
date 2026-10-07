using System;

namespace IndependentWork1
{
    public class Employee
    {
        private string _fullName;
        private double _hourlyRate;
        private int _hoursWorked;

        public string FullName
        {
            get { return _fullName; }
        }

        public double HourlyRate
        {
            get { return _hourlyRate; }
            set { if (value >= 0) _hourlyRate = value; }
        }

        public Employee(string fullName, double hourlyRate, int hoursWorked)
        {
            _fullName = fullName;
            _hourlyRate = hourlyRate;
            _hoursWorked = hoursWorked;
        }

        public double CalculateSalary()
        {
            return _hourlyRate * _hoursWorked;
        }

        public void PrintInfo()
        {
            Console.WriteLine($"Працівник: {_fullName}, ставка: {_hourlyRate} грн, відпрацьовано годин: {_hoursWorked}");
        }
    }

    public class Rectangle
    {
        private double _width;
        private double _height;

        public double Width
        {
            get { return _width; }
            set { if (value > 0) _width = value; }
        }

        public double Height
        {
            get { return _height; }
            set { if (value > 0) _height = value; }
        }

        public bool IsSquare
        {
            get { return _width == _height; }
        }

        public Rectangle(double width, double height)
        {
            _width = width;
            _height = height;
        }

        public double CalculateArea()
        {
            return _width * _height;
        }

        public double CalculatePerimeter()
        {
            return 2 * (_width + _height);
        }
    }

    public class Recipe
    {
        private string _title;
        private int _cookingTimeMinutes;
        private int _servings;

        public string Title => _title;

        public int CookingTimeMinutes
        {
            get => _cookingTimeMinutes;
            set { if (value > 0) _cookingTimeMinutes = value; }
        }

        public Recipe(string title, int cookingTimeMinutes, int servings)
        {
            _title = title;
            _cookingTimeMinutes = cookingTimeMinutes;
            _servings = servings;
        }

        public bool IsQuickRecipe()
        {
            return _cookingTimeMinutes <= 30;
        }

        public void PrintRecipeSummary()
        {
            string speedText = IsQuickRecipe() ? "Швидкий рецепт" : "Потребує часу";
            Console.WriteLine($"Рецепт: {_title}, час приготування: {_cookingTimeMinutes} хв ({speedText}), порцій: {_servings}");
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Employee emp = new Employee("Олександр Шевченко", 250.0, 160);
            emp.PrintInfo();
            Console.WriteLine($"Нарахована зарплата: {emp.CalculateSalary()} грн");

            Rectangle rect = new Rectangle(12.5, 8.0);
            Console.WriteLine($"Прямокутник: {rect.Width} x {rect.Height}");
            Console.WriteLine($"Площа: {rect.CalculateArea()}");
            Console.WriteLine($"Периметр: {rect.CalculatePerimeter()}");
            Console.WriteLine($"Квадрат: {rect.IsSquare}");

            Recipe quickRecipe = new Recipe("Паста Карбонара", 20, 2);
            Recipe slowRecipe = new Recipe("Борщ український", 90, 6);

            quickRecipe.PrintRecipeSummary();
            slowRecipe.PrintRecipeSummary();
        }
    }
}