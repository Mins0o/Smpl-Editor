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
        private string artist;
        private string info;
        private int order;
        private string title;
        private int type;
        private int extraStuff;
        public string Artist {
            get{
                return this.artist;
            }
        }
        public string Info {
            get{
                return this.info;
            }
        }   
        public int Order {
            get{
                return this.order;
            }
            set{
                this.order = value;
            }
        }
        public string Title {
            get{
                return this.title;
            }
        }
        public int Type {
            get{
                return this.type;
            }
        }
        public int ExtraStuff {
            get{
                return this.extraStuff;
            }
            set{
                this.extraStuff = value;
            }
        }

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
            double artistScore = this.levenstein.GetSimilarity(this.artist, smplSong.Artist);
            double titleScore = this.levenstein.GetSimilarity(this.title, smplSong.Artist);

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
