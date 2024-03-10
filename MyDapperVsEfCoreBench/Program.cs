namespace MyDapperVsEfCoreBench
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }
    }
    /* test ef core store procedure vs dapper stored procedure || dapper is faster
     * test getting the connection string from ef core vs creating a manual one for dapper
     * test raw queries in efcore vs dapper || dapper is faster
     */
}