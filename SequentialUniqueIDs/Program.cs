using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SequentialUniqueIDs
{
    internal class Program
    {
        static void Main(string[] args)
        {
            GenNewIdId();
        }

        static void GenNewIdId()
        {
            var newids = new List<NewId>();
            for (int i = 0; i < 15; i++)
            {
                newids.Add(NewId.Next());
                // Console.WriteLine($"Guid no.{i} => {Guid.NewGuid()}");
            }
            int count = 0;

            newids.ForEach(guid => Console.WriteLine($"Guid no.{count++} => {guid}"));
            Console.WriteLine("now sorting");
            newids.Sort();
            Task.Delay(1000).Wait();
            count = 0;
            newids.ForEach(guid => Console.WriteLine($"Guid no.{count++} => {guid}"));

        }
        static void GenSystemGuid()
        {
            var guids = new List<Guid>();
            for (int i = 0; i < 15; i++)
            {
                guids.Add(Guid.NewGuid());
                // Console.WriteLine($"Guid no.{i} => {Guid.NewGuid()}");
            }

            int count = 0;

            guids.ForEach(guid => Console.WriteLine($"Guid no.{count++} => {guid}"));
            Console.WriteLine("now sorting");
            guids.Sort();
            Task.Delay(1000).Wait();
            count = 0;
            guids.ForEach(guid => Console.WriteLine($"Guid no.{count++} => {guid}"));
        }
    }
}
