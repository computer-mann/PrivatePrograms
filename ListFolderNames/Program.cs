using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using static System.Net.WebRequestMethods;

namespace ListFolderNames
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var folder = "D:\\Projects\\dotnet\\Hubtel\\Hubtel60\\Hubtel-Library\\src";
            var toReplace = "D:\\Documents";
            var info = new DirectoryInfo(folder);
            try
            {
                foreach (var file in info.EnumerateDirectories())
                {
                    Console.WriteLine($"{file.Name}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }




        }
    }
}
