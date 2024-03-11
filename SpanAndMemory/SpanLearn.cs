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
        private string Path = "C:\\Users\\hpsn1\\OneDrive\\Documents\\Projects\\dotnet\\PrivatePrograms\\SpanAndMemory\\passenger_output.csv";
        [Benchmark]
        public Ticket[] RawNaiveGetColumnsFromCsvArray()
        {
            
            var csv = File.ReadLines(Path).Skip(2).ToArray();
            var tickets = new Ticket[csv.Length];
            for (int i = 0; i < csv.Length; i++)
            {
                var line = csv[i].Split("|");
                var ticket_no = line[2].Trim();
                var passenger_name = line[0].Trim();
                var book_ref = line[1].Trim();
                tickets[i]=new Ticket(passenger_name,ticket_no,book_ref);
            }
            return tickets;
        }
        [Benchmark]
        public List<Ticket> RawNaiveGetColumnsFromCsvList()
        {

            var csv = File.ReadLines(Path).Skip(2).ToList();
            var tickets = new List<Ticket>();
            foreach (var row in csv)
            {
                var line = row.Split("|");
                var passenger_name = line[0].Trim();
                var book_ref = line[1].Trim();
                var ticket_no = line[2].Trim();
                tickets.Add(new Ticket(passenger_name, ticket_no, book_ref));
            }
            return tickets;
        }

        //[Benchmark]
        //public void SpanOptimizedGetLinesFromCsv()
        //{
        //    var range = Enumerable.Range(1, 324321231);
        //    foreach (var line in range)
        //    {
        //        int mul = line * 2; 
        //    }
        //}

        public record Ticket(string passenger_id,string ticket_no,string book_ref);
    }
}
