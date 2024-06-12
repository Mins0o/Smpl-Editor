using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimMetrics.Net.Metric;

namespace SmplEditor
{
    public class Song
    {
        private string title;
        public string Title{
            get{
                return this.title;
            }
        }
        private string artist;
        public string Artist{
            get{
                return this.artist;
            }
        }
        private SmplSong smplMusic = default(SmplSong);
        public SmplSong SmplMusic {
            set {
                this.smplMusic = value;
            }
            get {
                return this.smplMusic;
            }
        }
        private ITunesLibraryParser.Track iTunesSong = default(ITunesLibraryParser.Track);
        public ITunesLibraryParser.Track ITunesSong {
            set {
                this.iTunesSong = value;
            }
            get {
                return this.iTunesSong;
            }
        }
        private const double ARTIST_TH = 7/8.0;
        private const double TITLE_TH = 13/16.0;
        private const double FILENAME_TH = 7/8.0;

        public Song(SmplSong smplMusic) {
            this.smplMusic = smplMusic;
            this.title = smplMusic.title;
            this.artist = smplMusic.artist;
        }

        public Song(ITunesLibraryParser.Track iTunesMusic){
            this.iTunesSong = iTunesMusic;
            this.title = iTunesMusic.Name;
            this.artist = iTunesMusic.Artist==default(string)?"unknown":iTunesMusic.Artist;
        }
        public bool HasSmplSong() {
            return this.smplMusic != default(SmplSong);
        }
        public bool HasITunesSong() {
            return this.iTunesSong != default(ITunesLibraryParser.Track);
        }
        // An iTunes track have
        //        Album string
        //        AlbumArtist string
        //        AlbumRating int?
        //        AlbumRatingComputed bool
        //        Artist string -------------------*
        //        BitRate int?
        //        Composer string
        //+		  DateAdded	System.DateTime
        //+		  DateModified System.DateTime?
        //        Genre string
        //		  Kind string
        //		  Location string
        //		  Name string   -------------------*
        //		  PartOfCompilation bool
        //		  PersistentId string
        //		  PlayCount int?
        //+		  PlayDate System.DateTime?
        //        PlayingTime string
        //		  Rating int?
        //		  SampleRate int?
        //		  Size long?
        //		  TrackId int
        //		  TrackNumber int?
        //		  Year int?

        // A Smpl track have
        // artist
        // info (path)
        // order (per playlist, not an attribute of a track)
        // title
        // type (???)

        private Levenstein levenstein = new Levenstein();

        // Compare returns comparator output:int
        // Equal returns boolean of whether two are equal or not. (T/F)

        public bool IsEqualTo(SmplSong smplSong) {
            if (this.HasSmplSong()) {
                return smplSong.IsEqualTo(this.SmplMusic);
            }
            else if (this.HasITunesSong()) {
                return smplSong.IsEqualTo(this.ITunesSong);
            }
            System.Diagnostics.Debug.Print("Song.IsEqualTo: Not implemented for this type");
            return false;
        }
        public bool IsSoftEqualTo(SmplSong smplSong){
            if (this.HasSmplSong()){
                return smplSong.IsSoftEqualTo(this.SmplMusic);
            }
            else if (this.HasITunesSong()){
                return smplSong.IsSoftEqualTo(this.ITunesSong);
            }
            System.Diagnostics.Debug.WriteLine("Song.IsSoftEqualTo: Not implemented for this type");
            return false;
        }
        
        private void getDirectoryFileName(ITunesLibraryParser.Track iTunesTrack, ref string fileName, ref string dirName){
            string trackLocation;
            string urlDecoded;
            string safeName;

            trackLocation = iTunesTrack.Location;
            urlDecoded = System.Net.WebUtility.UrlDecode(trackLocation);
            safeName = urlDecoded.Replace(":", "_");
            dirName = System.IO.Directory.GetParent(safeName).Name;
            fileName = System.IO.Path.GetFileName(safeName);
        }
        private void getReasonableFileNames(ITunesLibraryParser.Track thisTrack,
                                            ITunesLibraryParser.Track otherTrack,
                                            ref string thisFileName,
                                            ref string otherFileName){
            string thisDirName = "";
            string otherDirName = "";
            this.getDirectoryFileName(thisTrack, ref thisFileName, ref thisDirName);
            this.getDirectoryFileName(otherTrack, ref otherFileName, ref otherDirName);
            if (thisFileName.Length < 11){ // If the filename itself is too short or not unique
                thisFileName = thisDirName + thisFileName;
                otherFileName = otherDirName + otherFileName;
            }
            return;
        }
        public bool IsEqualTo(ITunesLibraryParser.Track iTunesTrack) {
            if (this.HasITunesSong()) {
                bool hasSameArtist = (this.iTunesSong.Artist == iTunesTrack.Artist);
                bool hasSameTitle = (this.iTunesSong.Name == iTunesTrack.Name);
                string thisFileName = "";
                string otherFileName = "";
                getReasonableFileNames(this.iTunesSong, iTunesTrack, ref thisFileName, ref otherFileName);
                bool hasSameFileName = (thisFileName == otherFileName);
                if (hasSameArtist && hasSameTitle && hasSameFileName){
                    return true;
                }
            }
            else if (this.HasSmplSong()) {
                return this.SmplMusic.IsEqualTo(iTunesTrack);
            }
            System.Diagnostics.Debug.Print("Song.CompareWith: Not implemented for this type");
            return false;
        }
        public bool IsSoftEqualTo(ITunesLibraryParser.Track iTunesTrack) {
            if (this.HasITunesSong()) {
                bool hasSimilarArtist = this.levenstein.GetSimilarity(this.iTunesSong.Artist, iTunesTrack.Artist) > ARTIST_TH;
                bool hasSimilarTitle = this.levenstein.GetSimilarity(this.iTunesSong.Name, iTunesTrack.Name) > TITLE_TH;
                string thisFileName = "";
                string otherFileName = "";
                getReasonableFileNames(this.iTunesSong, iTunesTrack, ref thisFileName, ref otherFileName);
                bool hasSimilarFileName = this.levenstein.GetSimilarity(thisFileName, otherFileName) > FILENAME_TH;
                if (hasSimilarArtist && hasSimilarTitle && hasSimilarFileName){
                    return true;
                }
            }
            else if (this.HasSmplSong()) {
                return this.SmplMusic.IsSoftEqualTo(iTunesTrack);
            }
            System.Diagnostics.Debug.Print("Song.CompareWith: Not implemented for this type");
            return false;
        }
        public int CompareByOrder(Song comparingTo)
        {
            bool thisHasSmpl = this.HasSmplSong();
            bool thisHasITunes = this.HasITunesSong();
            bool compHasSmpl = comparingTo.HasSmplSong();
            bool compHasITunes = comparingTo.HasITunesSong();
            if (thisHasSmpl && compHasSmpl){
                // smpl <--> smpl
                SmplSong x = this.SmplMusic;
                SmplSong y = comparingTo.SmplMusic;

                return x.CompareByOrder(y);
            }
            else if (thisHasITunes && compHasITunes){
                // iTuens <--> iTunes
                return 0;
            }
            else if (thisHasSmpl && compHasITunes){
                // smpl <--> iTunes

                return 0;
            }
            else if (thisHasITunes && compHasITunes){
                return 0;
            }
            else{
                System.Diagnostics.Debug.Print("Song.CompareByOrder: This case is not handled");
                return 0;
            }
        }
        public int CompareByArtist(Song comparingTo){
            if (this.HasSmplSong() && comparingTo.HasSmplSong()){
                // smpl <--> smpl
                SmplSong x = this.SmplMusic;
                SmplSong y = comparingTo.SmplMusic;

                return x.CompareByArtist(y);
            }
            else if (this.HasITunesSong() && comparingTo.HasITunesSong()){
                // iTuens <--> iTunes
                return 0;
            }
            else {
                bool thisIsGlitched = !this.HasITunesSong() && !this.HasSmplSong();
                bool comparingIsGlitched = !this.HasITunesSong() && !this.HasSmplSong();

                if(thisIsGlitched || comparingIsGlitched){
                    System.Diagnostics.Debug.Print(thisIsGlitched.ToString());
                    System.Diagnostics.Debug.Print(comparingIsGlitched.ToString());
                }
                // smpl <--> iTunes
                return 0;
            }
        }
        public int CompareByTitle(Song comparingTo){
            if (this.HasSmplSong() && comparingTo.HasSmplSong()){
                // smpl <--> smpl
                SmplSong x = this.SmplMusic;
                SmplSong y = comparingTo.SmplMusic;

                return x.CompareByTitle(y);
            }
            else if (this.HasITunesSong() && comparingTo.HasITunesSong()){
                // iTuens <--> iTunes
                return 0;
            }
            else {
                bool thisIsGlitched = !this.HasITunesSong() && !this.HasSmplSong();
                bool comparingIsGlitched = !this.HasITunesSong() && !this.HasSmplSong();

                if(thisIsGlitched || comparingIsGlitched){
                    System.Diagnostics.Debug.Print(thisIsGlitched.ToString());
                    System.Diagnostics.Debug.Print(comparingIsGlitched.ToString());
                }
                // smpl <--> iTunes
                return 0;
            }
        }
        public int CompareByDir(Song comparingTo){
            if (this.HasSmplSong() && comparingTo.HasSmplSong()){
                // smpl <--> smpl
                SmplSong x = this.SmplMusic;
                SmplSong y = comparingTo.SmplMusic;

                return x.CompareByTitle(y);
            }
            else if (this.HasITunesSong() && comparingTo.HasITunesSong()){
                // iTuens <--> iTunes
                return 0;
            }
            else {
                bool thisIsGlitched = !this.HasITunesSong() && !this.HasSmplSong();
                bool comparingIsGlitched = !this.HasITunesSong() && !this.HasSmplSong();

                if(thisIsGlitched || comparingIsGlitched){
                    System.Diagnostics.Debug.Print(thisIsGlitched.ToString());
                    System.Diagnostics.Debug.Print(comparingIsGlitched.ToString());
                }
                // smpl <--> iTunes
                return 0;
            }
        }
        void remove_from_lib()
        {
            return;
        }
        public override string ToString()
        {
            return this.artist + " - " + this.title;
        }
    }
}
