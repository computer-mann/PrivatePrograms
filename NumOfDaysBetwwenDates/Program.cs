namespace NumOfDaysBetwwenDates
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var diff = FindNumberOfDaysBetweenProvidedDates(DateOnly.Parse("14 09 2023"), DateOnly.Parse("12 01 2024"));
            Console.WriteLine($"I did not touch real code for {diff} days"); //120 days
        }

        static int FindNumberOfDaysBetweenProvidedDates(DateOnly from, DateOnly to)
        {
            return to.DayOfYear + (365-from.DayOfYear);
        }
    }
}
