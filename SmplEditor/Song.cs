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
        public Song(SmplSong smplSong){
            this.smplMusic = smplSong;
        }

        public bool HasSmplSong(){
            return this.smplMusic != default(SmplSong);
        }
        public bool HasITunesSong(){
            return this.iTunesSong != default(ITunesLibraryParser.Track);
        }
        // An iTunes song have
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

        private Levenstein levenstein = new Levenstein();
        private SmplSong smplMusic = default(SmplSong);
        public SmplSong SmplMusic{
            set{
                this.smplMusic = value;
            }
            get{
                return this.smplMusic;
            }
        }
        private ITunesLibraryParser.Track iTunesSong = default(ITunesLibraryParser.Track);
        public ITunesLibraryParser.Track ITunesSong{
            set{
                this.iTunesSong = value;
            }
            get{
                return this.iTunesSong;
            }
        }

        public bool CompareWith(SmplSong smplSong){
            if (this.HasSmplSong()){
                return smplSong.CompareWith(this.SmplMusic);
            }
            if (this.HasITunesSong()){
                return smplSong.CompareWith(this.ITunesSong);
            }
            return false;
        }
        public bool CompareWith(ITunesLibraryParser.Track iTunesSong){
            if (this.HasSmplSong()){
                return this.SmplMusic.CompareWith(iTunesSong);
            }
            if (this.HasITunesSong()){
                double artistScore = this.levenstein.GetSimilarity(this.ITunesSong.Artist, iTunesSong.Artist);
                double titleScore = this.levenstein.GetSimilarity(this.ITunesSong.Name, iTunesSong.Name);
                double albumScore = this.levenstein.GetSimilarity(this.ITunesSong.Album, iTunesSong.Album);

                if (artistScore > 0.9 && titleScore > 0.8 && albumScore > 0.8){
                    return true;
                }
            }
            return false;
        }
        void remove_from_lib()
        {
            return;
        }
    }
}
