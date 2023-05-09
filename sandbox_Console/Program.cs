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
            Console.WriteLine("\n\n↓↓↓↓↓Local Global Test↓↓↓↓↓\n");
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
            Console.WriteLine("\n\n↑↑↑↑↑Local Global Test↑↑↑↑↑\n");
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
            Console.WriteLine("\n\n↓↓↓↓↓Levenstein Test↓↓↓↓↓\n");
            string firstString = "File-Importing";
            string secondString = "File-exporting";
            double compareScore = this.levenstein.GetSimilarity(firstString, secondString);
            int length = firstString.Length;
            Console.WriteLine(firstString + " " + secondString);
            Console.WriteLine(compareScore.ToString("0.00") + " len " + length + " sim " + (compareScore * length));
            Console.WriteLine("\n\n↑↑↑↑↑Levenstein Testing↑↑↑↑↑\n");
        }
    }
    
    class InheritanceBase
    {
        private int code = 1; 
        public int Code { get { return code;} }
        private int virtualCode = 2;
        virtual public int VirtualCode { get { return virtualCode;} }
        private int selectiveInt = 3;
        public int SelectiveInt { get { return selectiveInt; } }
        private string selectiveString = "Hi";
        public string SelectiveString { get { return selectiveString; } }
        private List<int> selectiveList= new List<int> { 1, 2, 3 };
        public List<int> SelectiveList { get { return selectiveList ; } }

        public void Method01()
        {
            Console.WriteLine("01Call from Method01()");
        }
        public void Method02_override()
        {
            Console.WriteLine("02Call from the Base");
        }
        //abstract public void Method03_override();
        // cannot be declared abstract if it belongs to a non-abstract class
        /*{
        // cannot declare body because it is abstract.
            Console.WriteLine("Abstract, from Base");
        }*/
        /*public abstract void Method04_override()
        {

        }*/
        virtual public void Method05_override()
        {
            Console.WriteLine("05Virtual, from Base");
        }

        /*override public void Method06_override()
        {
            Console.WriteLine("override, from Base");
        }*/
        // This is not overriding a base of this class
        // This classs have it's base class
    }

    class InheritanceDerivedA : InheritanceBase
    {
        private int code = -1;
        public int Code { get { return code;} }
        private int virtualCode = -2;
        //public override int VirtualCode => base.VirtualCode;
        public override int VirtualCode { get { return virtualCode; } }
        public void Method02_override()
        {
            Console.WriteLine("02override, from A");
        }
        public override void Method05_override()
        {
            Console.WriteLine("05Virtual, from A");
        }
    }

    class InheritanceDerivedB : InheritanceBase
    {
        private int onlyBCode = 3;
        public int OnlyBCode { get { return onlyBCode; } }
        public void OnlyBMethod()
        {
            Console.WriteLine("Only B Method");
        }
    }

    class InheritanceTester
    {
        InheritanceBase baseClass = new InheritanceBase();
        InheritanceDerivedA aClass = new InheritanceDerivedA();
        InheritanceDerivedB bClass = new InheritanceDerivedB();

        private void printAll<T>(T inheritanceClass) where T : InheritanceBase
        {
            string printString = "Code : "
                               + inheritanceClass.Code.ToString()
                               + "|VirtualCode : "
                               + inheritanceClass.VirtualCode.ToString()
                               + "|SelectiveInt : "
                               + inheritanceClass.SelectiveInt.ToString()
                               + "|SelectiveString : "
                               + inheritanceClass.SelectiveString
                               + "|SelectiveList : "
                               + string.Join(", ",inheritanceClass.SelectiveList);
            Console.WriteLine(printString);
            inheritanceClass.Method01();
            inheritanceClass.Method02_override();
            inheritanceClass.Method05_override();
            /*Console.WriteLine(inheritanceClass.OnlyBCode.ToString());
            inheritanceClass.OnlyBMethod();*/
        }
        public void Test()
        {
            Console.WriteLine("\n\n↓↓↓↓↓Inheritance Testing↓↓↓↓↓\n");
            Console.WriteLine("Base");
            printAll(baseClass);
            Console.WriteLine("A");
            printAll(aClass);
            Console.WriteLine("B");
            printAll(bClass);
            Console.WriteLine(bClass.OnlyBCode.ToString());
            bClass.OnlyBMethod();
            Console.Write("A.Code, A not as inheritBase --> ");
            Console.WriteLine(aClass.Code);

            InheritanceDerivedA asKeywordA = new InheritanceDerivedA();
            InheritanceBase asKeywordBase = asKeywordA as InheritanceBase;
            Console.WriteLine("\nA itself →");
            asKeywordA.Method01();
            asKeywordA.Method02_override();
            asKeywordA.Method05_override();
            Console.WriteLine("A as Base →");
            asKeywordBase.Method01();
            asKeywordBase.Method02_override();
            asKeywordBase.Method05_override();
            Console.WriteLine("\n\n↑↑↑↑↑Inheritance Testing↑↑↑↑↑\n");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            LocalGlobalTester localGlobal = new LocalGlobalTester();
            LevensteinTester levenstein = new LevensteinTester();
            InheritanceTester inheritance01 = new InheritanceTester();

            Console.WriteLine("Hello, World!");

            var iTunesLibrary = new
                ITunesLibraryParser.ITunesLibrary("../../../../SmplEditor/Playlists/iTunes/Library.xml");
            var tracks = iTunesLibrary.Tracks;
            var playlists = iTunesLibrary.Playlists;
            var tracksList = tracks.ToList();
            var songFromTracks = tracksList[0];
            var songFromPlaylist = playlists.ToList()[0].Tracks.ToList()[0];
            var songFromPlaylist_ = playlists.ToList()[8].Tracks.ToList()[244];
            Console.WriteLine(songFromTracks);
            Console.WriteLine(songFromPlaylist);
            Console.WriteLine(songFromPlaylist_);
            Console.WriteLine("songFromTracks == songFromPlaylist: {0}", songFromTracks == songFromPlaylist);
            Console.WriteLine("songFromTracks.Equals(songFromPlaylist): {0}", songFromTracks.Equals(songFromPlaylist));
            Console.WriteLine("songFromPlaylist == songFromPlaylist_: {0}", songFromPlaylist == songFromPlaylist_);
            Console.WriteLine("songFromPlaylist.Equals(songFromPlaylist_): {0}", songFromPlaylist.Equals(songFromPlaylist_));
            Console.WriteLine(tracksList.Find(listsong => (listsong == songFromTracks)) != default(ITunesLibraryParser.Track));
            Console.WriteLine(tracksList.Find(listsong => (listsong == songFromPlaylist)) != default(ITunesLibraryParser.Track));
            Console.WriteLine(tracksList.Find(listsong => listsong.Equals(songFromTracks)) != default(ITunesLibraryParser.Track));
            Console.WriteLine(tracksList.Find(listsong => listsong.Equals(songFromPlaylist)) != default(ITunesLibraryParser.Track));

            string A = "Hello";
            string B = "Hello_";
            Console.WriteLine("A.RefEqual(B): {0}", A == B);
            Console.WriteLine("A.Equals(B): {0}", A.Equals(B));
            B += " World!";
            Console.WriteLine("A.RefEqual(B): {0}", A == B);
            Console.WriteLine("A.Equals(B): {0}", A.Equals(B));
            B = "Hello";
            Console.WriteLine("A.RefEqual(B): {0}", A == B);
            Console.WriteLine("A.Equals(B): {0}", A.Equals(B));
            B = A;
            Console.WriteLine("A.RefEqual(B): {0}", A == B);
            Console.WriteLine("A.Equals(B): {0}", A.Equals(B));

            string currd = System.IO.Directory.GetCurrentDirectory();

            Console.WriteLine(currd);

            localGlobal.LocalGlobalTest();
            levenstein.LevensteinTest();
            inheritance01.Test();
            int breakhere = 1;
        }
    }
}