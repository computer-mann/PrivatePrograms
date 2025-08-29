using HashidsNet;
using Sqids;

namespace Base10ToBaseNConverter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int number = 255;
            int newBigNumber = int.MaxValue;
            int negativeNumber = -1;
            int negativeBigNumber = int.MinValue;

            var hashids = new Hashids("this is my salt");
            var hash = hashids.Encode(number);
            var bigHash = hashids.Encode(newBigNumber);
            var negativeHash = hashids.Encode(negativeNumber);
            var negativeBigHash = hashids.Encode(negativeBigNumber);

            Console.WriteLine($"hashId: small number {hash} to big number: {bigHash} negative numbers does not support negative numbers");

            var options = new SqidsOptions
            {
                Salt = "this is my custom salt"
            };

            var sqids = new SqidsEncoder<int>(new()
            {
                Alphabet = "qAp5PRBF09n8NX4gZ7raEQew2hY6moVSicuWklJbTKz1IUyOvCxDsLGHMdt3fj",
                Sa
            });
            var sqid = sqids.Encode(number);
            var bigSqid = sqids.Encode(newBigNumber);
           
            Console.WriteLine($"sqds: small number {sqid} to big number: {bigSqid} and negative sqids : does not support negative sqids");

        }
    }
}
