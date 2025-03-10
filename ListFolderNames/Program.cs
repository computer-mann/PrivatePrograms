using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using static System.Net.WebRequestMethods;

namespace ListFolderNames
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var folder = "D:\\Projects\\dotnet\\Hubtel\\Hubtel-Library";
            var toReplace = "D:\\Documents";
            var info = new DirectoryInfo(folder);
            try
            {
                foreach (var file in info.EnumerateDirectories())
                {
                    Console.WriteLine($"{file.Name}");
                    await Task.Delay(2000);
                    Console.Clear();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }




        }
    }
}
