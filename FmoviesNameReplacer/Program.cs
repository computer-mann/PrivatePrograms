namespace FmoviesNameReplacer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var folder = "D:\\IDM\\Video";
            var toreplace = "FMovies - Watch ";
            var info = new DirectoryInfo(folder);
            try
            {
                foreach (var file in info.EnumerateFiles())
                {
                    var oldname = file.Name;
                    if (file.Name.StartsWith(toreplace))
                    {
                        file.MoveTo(file.FullName.Replace(toreplace, ""));
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