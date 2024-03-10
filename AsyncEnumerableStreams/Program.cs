using System.Runtime.CompilerServices;

namespace AsyncEnumerableStreams
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //foreach (var num in await chronousStream())
            //{
            //    Console.WriteLine($"{DateTime.Now.ToLongTimeString()}: {num}");
            //}
            await foreach (var num in ASynchStream(new CancellationTokenSource().Token))
            {
                Console.WriteLine($"{DateTime.Now.ToLongTimeString()}: {num}");
            }
            //foreach(var num in YieldStream())
            //{
            //   Console.WriteLine($"{DateTime.Now.ToLongTimeString()}: {num}");
            //}
        }
        static async IAsyncEnumerable<int> ASynchStream([EnumeratorCancellation]CancellationToken token)
        {
            Console.WriteLine("about to do it asynchronously stream");
            var listing = new List<int>();
            for (int i = 0; i < 10; i++)
            {
                await Task.Delay(1000,token);
                yield return i;

            }
          //  return listing;
        }

        static IEnumerable<int> YieldStream()
        {
            Console.WriteLine("about to do yield stream");
            var listing = new List<int>();
            for (int i = 0; i < 10; i++)
            {
                 Task.Delay(2000).Wait();
                yield return i;

            }
            //return listing;
        }
        static async Task<IEnumerable<int>> chronousStream()
        {
            Console.WriteLine("is this also an async call through out?");
            var listing = new List<int>();
            for(int i = 0; i < 10;i++)
            {
                await Task.Delay(1000);
                listing.Add(i);

            }
            return listing;
        }
    }

   
}