namespace sandbox_Console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            var iTunesLibrary = new
                ITunesLibraryParser.ITunesLibrary("../../../../SmplEditor/Playlists/iTunes/Library.xml");
            string currd = System.IO.Directory.GetCurrentDirectory();
            Console.WriteLine(currd);
            int breakhere = 1;
        }
    }
}