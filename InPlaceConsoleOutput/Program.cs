using UuidExtensions;

namespace InPlaceConsoleOutput
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //var output = "Hello, World!";
            //Console.WriteLine(output);
            //await Task.Delay(1000);
            //output = "Hello, World! (Updated)";
            var key = Uuid7.Id25();
            Console.WriteLine(key);
            Console.WriteLine(key.Count());
        }
    }
}
