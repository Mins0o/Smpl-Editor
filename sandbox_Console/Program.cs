namespace sandbox_Console
{
    class Tester
    {
        private int testingLocals(int passGlobal)
        {
            passGlobal += 1;
            return passGlobal;
        }
        private List<int> testingLocals(List<int> passGlobal)
        {
            passGlobal.Add(passGlobal.Count);
            return passGlobal;
        }
        private List<int> testingGlobalList = new List<int> { 1, 2, 3 };
        private int testingGlobalInt = 4;
        public Tester()
        {
            ;
        }
        public void test()
        {
            int outputInt;
            List<int> outputList;
            Console.WriteLine("inputInt " + testingGlobalInt);
            Console.WriteLine("inputList " + string.Join(", ", testingGlobalList));
            outputInt = testingLocals(testingGlobalInt);
            outputList = testingLocals(testingGlobalList);
            Console.WriteLine("outputInt " + outputInt);
            Console.WriteLine("outputList " + string.Join(", ", outputList));
            Console.WriteLine("afterOutInt " + testingGlobalInt);
            Console.WriteLine("afterOutList " + string.Join(", ", testingGlobalList));

        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            Tester test = new Tester();
            Console.WriteLine("Hello, World!");
            var iTunesLibrary = new
                ITunesLibraryParser.ITunesLibrary("../../../../SmplEditor/Playlists/iTunes/Library.xml");
            string currd = System.IO.Directory.GetCurrentDirectory();
            Console.WriteLine(currd);
            test.test();
            int breakhere = 1;
        }
    }
}