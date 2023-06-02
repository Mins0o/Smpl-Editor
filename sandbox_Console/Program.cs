using System.Runtime.CompilerServices;

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

        static void ITunesEqualsTester(ITunesLibraryParser.ITunesLibrary iTunesLibrary)
        {
            var tracks = iTunesLibrary.Tracks;
            var playlists = iTunesLibrary.Playlists;
            var tracksList = tracks.ToList();
            var playlistsList = playlists.ToList();
            var songFromTracks = tracksList[0];
            var songFromPlaylist = playlistsList[0].Tracks.ToList()[0];
            var songFromPlaylist_ = playlistsList[8].Tracks.ToList()[244];
            var otherSongFromPlaylist = playlistsList[0].Tracks.ToList()[1];
            Console.WriteLine(songFromTracks);
            Console.WriteLine(songFromPlaylist);
            Console.WriteLine(songFromPlaylist_);


            Console.WriteLine("songFromTracks == songFromPlaylist: {0}", songFromTracks == songFromPlaylist);
            Console.WriteLine("songFromTracks.Equals(songFromPlaylist): {0}", songFromTracks.Equals(songFromPlaylist));
            Console.WriteLine("songFromPlaylist == songFromPlaylist_: {0}", songFromPlaylist == songFromPlaylist_);
            Console.WriteLine("songFromPlaylist.Equals(songFromPlaylist_): {0}", songFromPlaylist.Equals(songFromPlaylist_));
            //songFromTracks == songFromPlaylist: False
            //songFromTracks.Equals(songFromPlaylist): True
            //songFromPlaylist == songFromPlaylist_: True
            //songFromPlaylist.Equals(songFromPlaylist_): True


            Console.WriteLine(tracksList.Find(listsong => (listsong == songFromTracks)) != default(ITunesLibraryParser.Track));
            Console.WriteLine(tracksList.Find(listsong => (listsong == songFromPlaylist)) != default(ITunesLibraryParser.Track));
            Console.WriteLine(tracksList.Find(listsong => listsong.Equals(songFromTracks)) != default(ITunesLibraryParser.Track));
            Console.WriteLine(tracksList.Find(listsong => listsong.Equals(songFromPlaylist)) != default(ITunesLibraryParser.Track));
            //True
            //False
            //True
            //True

            var trackKeyedMapper = new Dictionary<ITunesLibraryParser.Track, int>();
            trackKeyedMapper.Add(songFromTracks, 1);
            trackKeyedMapper.Add(otherSongFromPlaylist, 2);
            int breakHere = 1;
            // Exception occurred in the line below.
            //    "An item with the same key has already been added."
            // Which means in dictionary, keys are matched by .Equals() - by value.
            // trackKeyedMapper.Add(songFromPlaylist, 3);

            breakHere = 2;

            Console.WriteLine(trackKeyedMapper[songFromTracks]);

            var playlistSoundTracks = playlistsList[36];
            // Turns out, the iTunesLibraryParser doesn't import all the attributes in the xml
        }

        static void StringEqualsTester()
        {
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
        }

        static void FilePathTester(){
            string testPath1 = "C:\\Users\\steng\\Music\\iTunes\\iTunes Media\\Audiobooks\\Toni Morrison";
            string testPath2 = "http://file-ex.ssenhosting.com/data1/nick/n3d40.mp3";
            string parent1;
            string parent2;
            string leaf1;
            string leaf2;
            try{
                parent1 = System.IO.Directory.GetParent(testPath1).Name;
                Console.WriteLine(parent1);
            }
            catch{
                Console.WriteLine("failed parent1");
            }
            try{
                parent2 = System.IO.Directory.GetParent(testPath2).Name;
                Console.WriteLine(parent2);
            }
            catch{
                Console.WriteLine("failed parent2");
            }
            try{
                leaf1 = System.IO.Path.GetFileName(testPath1);
                Console.WriteLine(leaf1);
            }
            catch{
                Console.WriteLine("failed leaf1");
            }
            try{
                leaf2 = System.IO.Path.GetFileName(testPath2);
                Console.WriteLine(leaf2);
            }
            catch{
                Console.WriteLine("failed leaf1");
            }
            return;
        }

        static void Main(string[] args)
        {
            LocalGlobalTester localGlobal = new LocalGlobalTester();
            LevensteinTester levenstein = new LevensteinTester();
            InheritanceTester inheritance01 = new InheritanceTester();

            Console.WriteLine("Hello, World!");

            var iTunesLibrary = new
                ITunesLibraryParser.ITunesLibrary("../../../../SmplEditor/Playlists/iTunes/Library.xml");

            ITunesEqualsTester(iTunesLibrary);
            //StringEqualsTester();

            string currd = System.IO.Directory.GetCurrentDirectory();

            Console.WriteLine(currd);

            //localGlobal.LocalGlobalTest();
            //levenstein.LevensteinTest();
            //inheritance01.Test();
            int breakhere = 1;

            FilePathTester();
        }
    }
}