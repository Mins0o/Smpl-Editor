using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimMetrics.Net.Metric;

namespace SmplEditor
{
    public class SmplSong
    {
        // field name should match the json format when using
        // the .smpl uses lowercase.
        public string artist {get; set;}
        public string info {get; set;}
        public int order {get; set;}
        public string title {get; set;}
        public int type {get; set;}

        private const double ARTIST_TH = 7/8.0;
        private const double TITLE_TH = 13/16.0;
        private const double FILENAME_TH = 7/8.0;
        private Levenstein levenstein = new Levenstein();
        public string UpperDirectory()
        {
            return info.Substring(0, info.LastIndexOf('/'));
        }

        // Compare returns comparator output:int
        // Equal returns boolean of whether two are equal or not. (T/F)

        public bool IsEqualTo(Song song){
            if (song.HasSmplSong()){
                return this.IsEqualTo(song.SmplMusic);
            }
            else if(song.HasITunesSong()){
                return this.IsEqualTo(song.ITunesSong);
            }
            return false;
        }

        private void getReasonableFileNames(SmplSong thisSong,
                                            SmplSong otherSong,
                                            ref string thisFileName,
                                            ref string otherFileName){
            thisFileName = System.IO.Path.GetFileName(thisSong.info);
            if (thisFileName.Length < 11){ // If the filename itself is too short or not unique
                thisFileName = System.IO.Directory.GetParent(thisSong.info).Name
                                + System.IO.Path.GetFileNameWithoutExtension(thisSong.info);
                otherFileName = System.IO.Directory.GetParent(otherSong.info).Name
                                + System.IO.Path.GetFileNameWithoutExtension(otherSong.info);
            }
            else{
                otherFileName = System.IO.Path.GetFileName(otherSong.info);
            }
            return;
        }
        private void getReasonableFileNames(SmplSong thisSong,
                                            ITunesLibraryParser.Track otherSong,
                                            ref string thisFileName,
                                            ref string otherFileName){            
            thisFileName = System.IO.Path.GetFileName(thisSong.info);
            if (thisFileName.Length < 11){
                thisFileName = System.IO.Directory.GetParent(thisSong.info).Name
                                + System.IO.Path.GetFileName(thisSong.info);
                otherFileName = System.IO.Directory.GetParent(
            System.Net.WebUtility.UrlDecode(otherSong.Location).Replace(":","_")).Name
                                + System.IO.Path.GetFileName(otherSong.Location);
            }
            else{
                otherFileName = System.IO.Path.GetFileName(otherSong.Location);
            }
            return;
        }
        public bool IsEqualTo(SmplSong smplSong){
            bool sameArtist = (this.artist == smplSong.artist);
            bool sameTitle = (this.title == smplSong.title);
            string thisFileName = "";
            string otherFileName = "";
            getReasonableFileNames(this, smplSong, ref thisFileName, ref otherFileName);
            bool sameFile = (thisFileName == otherFileName);
            if (sameArtist && sameTitle && sameFile){
                return true;
            }
            return false;
        }

        public bool IsEqualTo(ITunesLibraryParser.Track iTunesSong){
            bool sameArtist = (this.artist == iTunesSong.Artist);
            bool sameTitle = (this.title == iTunesSong.Name);
            string thisFileName = "";
            string otherFileName = "";
            getReasonableFileNames(this, iTunesSong, ref thisFileName, ref otherFileName);
            bool sameFile = (thisFileName == otherFileName);
            if (sameArtist && sameTitle && sameFile){
                return true;
            }
             return false;
        }
        
        public int CompareByOrder(SmplSong comparingTo){
            int directoryCompareResult = this.UpperDirectory().CompareTo(comparingTo.UpperDirectory());
            if (directoryCompareResult == 0) {
                return this.title.CompareTo(comparingTo.title);
            } else{
                return directoryCompareResult;
            }
        }
        public int CompareByOrder(ITunesLibraryParser.Track comparingTo){
            return 0;
        }

        public int CompareByArtist(SmplSong comparingTo){
            return this.artist.CompareTo(comparingTo.artist);
        }
        public int CompareByArtist(ITunesLibraryParser.Track comparingTo){
            return 0;
        }

        public int CompareByTitle(SmplSong comparingTo){
            return this.title.CompareTo(comparingTo.title);
        }
        public int CompareByTitle(ITunesLibraryParser.Track comparingTo){
            return 0;
        }

        public int CompareByDir(SmplSong comparingTo){
            return this.info.CompareTo(comparingTo.info);
        }
        public int CompareBydir(ITunesLibraryParser.Track comparingTo){
            return 0;
        }
    }
}
