namespace FmoviesNameReplacer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var folder = "D:\\Projects\\dotnet\\NunooCorp.IdentityServer.Admin\\tests";
            var toReplace = "Skoruba";
            var replaceWith = "PrinceHarry";
            var info = new DirectoryInfo(folder);
            try
            {
                foreach (var firstLevel in info.EnumerateDirectories())
                {
                    if (Directory.Exists(firstLevel.FullName))
                    {
                        ChangeFileNames(firstLevel, toReplace, replaceWith);
                        //ChangeFolderNames(firstLevel, toReplace, replaceWith);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
        public static void ChangeFolderNames(DirectoryInfo firstLevel,string toReplace,string replaceWith)
        {
            var oldname = firstLevel.Name;
            Console.WriteLine($"{firstLevel.Name}");
            if (firstLevel.Name.StartsWith(toReplace))
            {
                firstLevel.MoveTo(firstLevel.FullName.Replace(toReplace, replaceWith));
                Console.WriteLine($"{oldname} -> {firstLevel.Name}");
            }
        }
        public static void ChangeFileNames(DirectoryInfo firstLevel, string toReplace, string replaceWith)
        {
            var secondInfo = new DirectoryInfo(firstLevel.FullName);
            foreach (var secondLevel in secondInfo.EnumerateFiles())
            {
                Console.WriteLine(secondLevel.Name);
                if (secondLevel.Name.StartsWith(toReplace) && secondLevel.Name.EndsWith(".csproj"))
                {
                    var oldname = secondLevel.Name;
                    secondLevel.MoveTo(secondLevel.FullName.Replace(toReplace, replaceWith));
                    Console.WriteLine($"{oldname} -> {secondLevel.Name}");
                }
            }
        }
    }

}