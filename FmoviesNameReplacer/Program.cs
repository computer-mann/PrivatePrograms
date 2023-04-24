namespace FmoviesNameReplacer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var folder = "D:\\IDM\\Video";
            var toReplace = "FMovies - Watch ";
            var info = new DirectoryInfo(folder);
            try
            {
                foreach (var file in info.EnumerateFiles())
                {
                    var oldname = file.Name;
                    if (file.Name.StartsWith(toReplace))
                    {
                        file.MoveTo(file.FullName.Replace(toReplace, ""));
                        Console.WriteLine($"{oldname} -> {file.Name}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
    }
}