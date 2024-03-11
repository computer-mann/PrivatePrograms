using BenchmarkDotNet.Running;

namespace SpanAndMemory
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<SpanLearn>();
        }
    }
}
