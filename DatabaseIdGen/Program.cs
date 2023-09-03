using UuidExtensions;
using UUIDNext;
using RT.Comb;
using nuild = NUlid;
using MassTransit;

namespace DatabaseIdGen
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Uuidv7Gen();
            UuidNext();
            RtCombGen();
            NUlidGens();
            GenNewIdId();
            GenSystemGuid();
            UlidGens();


        }

        static void NUlidGens()
        {
            //Console.WriteLine(Ulid.NewUlid());
            Console.WriteLine("newulid fast gen");
            int count = 0;
            var ids = new List<nuild.Ulid>();
            for (int i = 0; i < 20; i++)
            {
                ids.Add(nuild.Ulid.NewUlid());

            }
            // ids.ForEach(guid => Console.WriteLine($"Guid no.{count++}  => {guid}"));
            Console.WriteLine("now sorting");
            var sorted = new List<nuild.Ulid>(ids);
            sorted.Sort();
            Task.Delay(1000).Wait();
            //count = 0;
            //ids.ForEach(guid => Console.WriteLine($"Guid no.{count++} => {guid}"));
            PrintIntableForm<nuild.Ulid>(ids, sorted);
        }
        static void UlidGens()
        {
            //Console.WriteLine(Ulid.NewUlid());
            Console.WriteLine("ulid fast gen");
            int count = 0;
            var ids = new List<Ulid>();
            for (int i = 0; i < 20; i++)
            {
                ids.Add(Ulid.NewUlid());

            }
           // ids.ForEach(guid => Console.WriteLine($"Guid no.{count++}  => {guid}"));
            Console.WriteLine("now sorting");
            var sorted = new List<Ulid>(ids);
            sorted.Sort();
            Task.Delay(1000).Wait();
            PrintIntableForm(ids, sorted);
        }

        static void RtCombGen()
        {
            Console.WriteLine("rt comb gen");
            int count = 0;
            var ids = new List<Guid>();
            for (int i = 0; i < 20; i++)
            {
                ids.Add(Provider.PostgreSql.Create());

            }
            var sorted = new List<Guid>(ids);
            sorted.Sort();
            Task.Delay(1000).Wait();
            PrintIntableForm(ids, sorted);
        }
        static void UuidNext()
        {
            Console.WriteLine("UuidNext");
            int count = 0;
            var ids = new List<Guid>();
            for (int i = 0; i < 20; i++)
            {
                ids.Add(Uuid.NewDatabaseFriendly(Database.PostgreSql));

            }
            var sorted = new List<Guid>(ids);
            sorted.Sort();
            Task.Delay(1000).Wait();
            PrintIntableForm(ids, sorted);
        }

        static void Uuidv7Gen()
        {
            //var uuid = new Uuid7();
            //cant convert from the nice string back to a guid 
            Console.WriteLine("Uuidv7Gen");
            int count = 0;
            var ids= new List<string>();
            for(int i=0; i < 20;i++)
            {
                ids.Add(Uuid7.Id25());
                
            }
            //ids.ForEach(guid => Console.WriteLine($"Guid no.{count++} => {guid}"));
            Console.WriteLine("now sorting");
            var sorted= new List<string>(ids);
            sorted.Sort();
            Task.Delay(1000).Wait();
            count = 0;
            //ids.ForEach(guid => Console.WriteLine($"Guid no.{count++} => {guid}"));
            PrintIntableForm<string>(ids, sorted);
        }
        static void GenNewIdId()
        {
            var ids = new List<NewId>();
            for (int i = 0; i < 15; i++)
            {
                ids.Add(NewId.Next());
                // Console.WriteLine($"Guid no.{i} => {Guid.NewGuid()}");
            }
            var sorted = new List<NewId>(ids);
            sorted.Sort();
            Task.Delay(1000).Wait();
            //ids.ForEach(guid => Console.WriteLine($"Guid no.{count++} => {guid}"));
            PrintIntableForm<NewId>(ids, sorted);

        }
        static void GenSystemGuid()
        {
            Console.WriteLine("GenSystemGuid");
            var ids = new List<Guid>();
            for (int i = 0; i < 15; i++)
            {
                ids.Add(Guid.NewGuid());
                // Console.WriteLine($"Guid no.{i} => {Guid.NewGuid()}");
            }

            int count = 0;

            ids.ForEach(guid => Console.WriteLine($"Guid no.{count++} => {guid}"));
            Console.WriteLine("now sorting");
            var sorted = new List<Guid>(ids);
            sorted.Sort();
            Task.Delay(1000).Wait();
            //ids.ForEach(guid => Console.WriteLine($"Guid no.{count++} => {guid}"));
            PrintIntableForm<Guid>(ids, sorted);
        }


        static void PrintIntableForm<T>(List<T> unsorted,List<T> sorted) 
        {
            if (!Equals(unsorted.Count(), sorted.Count())) throw new ArgumentException("the array counts must be equal");
            Console.WriteLine("Unsorted Uuid              |     Sorted uuid");
            for(int i = 0;i<unsorted.Count();i++)
            {
                Console.WriteLine($"{unsorted[i]} | {sorted[i]}");
            }
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}