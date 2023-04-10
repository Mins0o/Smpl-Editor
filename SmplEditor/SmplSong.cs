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
    }
}
