namespace ImageResizze
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            var path = "C:\\Users\\hpsn1\\OneDrive\\Documents\\Projects\\dotnet\\PrivatePrograms\\ImageResizze";
            var info = new FileInfo(path+"\\1299476.png");
            using (Image image = Image.Load(info.FullName))
            {
                image.Mutate(x => x.Resize(image.Width / 2,image.Height/2));
                image.Save(path + "\\imagesharp1.png");
            }
            Console.WriteLine(info.FullName);
        }
    }
}