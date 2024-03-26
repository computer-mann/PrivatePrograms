using BenchmarkDotNet.Running;

namespace SpanAndMemory
{
    internal class Program
    {
        static void Main(string[] args)
        {
           // BenchmarkRunner.Run<SpanLearn>();
           var spanlearn = new SpanLearn();
            //var csvHelper = new UsingCsvHelper();
            //Console.WriteLine(csvHelper.GetTicketsUsingCsvHelper()[0]);
            var list = spanlearn.CsvParser_optimized();
            Console.WriteLine(list[0]);
        }
    }
}
