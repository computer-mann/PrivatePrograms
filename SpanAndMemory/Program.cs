using BenchmarkDotNet.Running;

namespace SpanAndMemory
{
    internal class Program
    {
        static void Main(string[] args)
        {
             BenchmarkRunner.Run<SpanLearn>();
            //var spanlearn = new SpanLearn();
            //var list = spanlearn.RawNaiveGetColumnsFromCsvList();
           // Console.WriteLine(list.First());
        }
    }
}
