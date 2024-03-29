﻿using BenchmarkDotNet.Attributes;
using nietras.SeparatedValues;
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
        public const string Path = "C:\\Users\\hpsn1\\OneDrive\\Documents\\Projects\\dotnet\\PrivatePrograms\\SpanAndMemory\\passenger_output.csv";
        List<string> csv = File.ReadLines(Path).Skip(2).ToList();
        [Benchmark]
        public Ticket[] CsvParser_Naive()
        {
            var csvCount = csv.Count();
            var tickets = new Ticket[csvCount];
            for (int i = 0; i < csvCount; i++)
            {
                var line = csv.ElementAt(i).Split("|");
                var ticket_no = line[2].Trim();
                var passenger_name = line[0].Trim();
                var book_ref = line[1].Trim();
                tickets[i]=new Ticket(passenger_name,ticket_no,book_ref);
            }
            return tickets;
        }
        

        [Benchmark]
        public Ticket[] CsvParser_optimized()
        {
            /*
             *       passenger_id        | book_ref |   ticket_no   
                ------------------------------------------------------
                 0qpzc0vu6tn0vuy9g3jy0arg9 | FCC5B7   | 0005432000302
             */
            //bookref=6,ticketno=13,passenger=25
            var csvCount = csv.Count();
            var tickets = new Ticket[csvCount];
            
            for (int i = 0; i < csvCount; i++)
            {
                ReadOnlySpan<char> passenger_name = csv.ElementAt(i).AsSpan().Slice(1, 25);
                ReadOnlySpan<char> book_ref = csv.ElementAt(i).AsSpan().Slice(29, 6);
                ReadOnlySpan<char> ticket_no = csv.ElementAt(i).AsSpan().Slice(40, 13);

                tickets[i] = new Ticket(passenger_name.ToString(), ticket_no.ToString(), book_ref.ToString());
            }
            return tickets;
        }

        [Benchmark]
        public Ticket[] Sep_CsvParser()
        {
            using var reader = Sep.New('|').Reader().FromFile(Path);
            var tickets=new List<Ticket>();
            
            foreach(var row in reader)
            {
                
                if (row.ColCount < 2)
                {
                    Console.WriteLine(row.ToString());
                    continue;
                }
                tickets.Add(new Ticket(row[0].ToString(), row[1].ToString(), row[2].ToString()));
                
            }
            
            return tickets.ToArray();
        }

        public record Ticket(string passenger_id,string ticket_no,string book_ref);
    }
}
