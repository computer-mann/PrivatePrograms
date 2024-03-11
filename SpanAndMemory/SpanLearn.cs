using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpanAndMemory
{
    [MemoryDiagnoser]
    public class SpanLearn
    {
        [Benchmark]
        public void RawNaiveGetColumnsFromCsv()
        {
            var range = Enumerable.Range(1, 567890);
            foreach (var line in range)
            {
                int mul=line*2;
            }
        }

        [Benchmark]
        public void SpanOptimizedGetLinesFromCsv()
        {
            var range = Enumerable.Range(1, 324321231);
            foreach (var line in range)
            {
                int mul = line * 2; 
            }
        }
    }
}
