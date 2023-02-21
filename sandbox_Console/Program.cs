namespace sandbox_Console
{
    class LocalGlobalTester
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
        public LocalGlobalTester()
        {
            ;
        }
        public void LocalGlobalTest()
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
    
    class LevensteinTester
    {
        private SimMetrics.Net.Metric.Levenstein levenstein = new SimMetrics.Net.Metric.Levenstein();
        public LevensteinTester()
        {
            ;
        }
        public void LevensteinTest()
        {
            string firstString = "File-Importing";
            string secondString = "File-exporting";
            double compareScore = this.levenstein.GetSimilarity(firstString, secondString);
            int length = firstString.Length;
            Console.WriteLine(firstString + " " + secondString);
            Console.WriteLine(compareScore.ToString("0.00") + " len " + length + " sim " + (compareScore * length));
        }
    }
    
        internal class Program
    {
        static void Main(string[] args)
        {
            LocalGlobalTester localGlobal = new LocalGlobalTester();
            LevensteinTester levenstein = new LevensteinTester();

            Console.WriteLine("Hello, World!");

            var iTunesLibrary = new
                ITunesLibraryParser.ITunesLibrary("../../../../SmplEditor/Playlists/iTunes/Library.xml");

            string currd = System.IO.Directory.GetCurrentDirectory();
            Console.WriteLine(currd);

            localGlobal.LocalGlobalTest();
            levenstein.LevensteinTest();
            int breakhere = 1;
        }
    }
}