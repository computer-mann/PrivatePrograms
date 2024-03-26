using BenchmarkDotNet.Attributes;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SpanAndMemory.SpanLearn;
using CsvHelper.Configuration;

namespace SpanAndMemory
{
    internal class UsingCsvHelper
    {
        //[Benchmark]
        public Ticket[] GetTicketsUsingCsvHelper()
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => args.Header.ToLower(),
                HasHeaderRecord = true,
              //  HeaderValidated=new HeaderValidated(),
                MissingFieldFound=null,
            };
            using (var reader = new StreamReader(SpanLearn.Path))
            using (var csv = new CsvReader(reader, config))
            {
                return csv.GetRecords<Ticket>().ToArray();
            }
            
        }
    }
}
