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
        public int extraStuff {get; set;}

        private Levenstein levenstein = new Levenstein();
        public string UpperDirectory()
        {
            return info.Substring(0, info.LastIndexOf('/'));
        }

        public bool CompareWith(Song song){
            if (song.HasSmplSong()){
                return this.CompareWith(song.SmplMusic);
            }
            else if(song.HasITunesSong()){
                return this.CompareWith(song.ITunesSong);
            }
            return false;
        }

        public bool CompareWith(SmplSong smplSong){
            double artistScore = this.levenstein.GetSimilarity(this.artist, smplSong.artist);
            double titleScore = this.levenstein.GetSimilarity(this.title, smplSong.artist);

            if (artistScore > 0.9 && titleScore > 0.8){
                return true;
            }
            return false;
        }

        public bool CompareWith(ITunesLibraryParser.Track iTunesSong){
            double artistScore = this.levenstein.GetSimilarity(this.artist, iTunesSong.Artist);
            double titleScore = this.levenstein.GetSimilarity(this.title, iTunesSong.Name);
            
            if (artistScore > 0.9 && titleScore > 0.8){
                return true;
            }
            return false;
        }
        
        public int CompareByOrder(SmplSong comparingTo){
            if (this.UpperDirectory().CompareTo(comparingTo.UpperDirectory()) == 0) {
                return this.order.CompareTo(comparingTo.order);
            } else{
                return this.UpperDirectory().CompareTo(comparingTo.UpperDirectory());
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
