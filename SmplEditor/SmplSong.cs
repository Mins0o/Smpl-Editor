﻿using System;
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

        private const double ARTIST_TH = 0.9;
        private const double TITLE_TH = 0.8;
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

        public bool IsEqualTo(SmplSong smplSong){
            bool sameArtist = (this.artist == smplSong.artist);
            bool sameTitle = (this.title == smplSong.title);
            bool sameFile = (this.info == smplSong.info);
            if (sameArtist && sameTitle && sameFile){
                return true;
            }
            // // needs optimization
            // double artistScore = this.levenstein.GetSimilarity(this.artist, smplSong.artist);
            // double titleScore = this.levenstein.GetSimilarity(this.title, smplSong.artist);

            // if (artistScore > ARTIST_TH && titleScore > TITLE_TH){
            //     return true;
            // }
            return false;
        }

        public bool IsEqualTo(ITunesLibraryParser.Track iTunesSong){
            bool sameArtist = (this.artist == iTunesSong.Artist);
            bool sameTitle = (this.title == iTunesSong.Name);
            string smplFileName = System.IO.Path.GetFileName(this.info);
            string iTunesFileName = System.IO.Path.GetFileName(iTunesSong.Location);
            bool sameFile = (smplFileName == iTunesFileName);
            if (sameArtist && sameTitle && sameFile){
                return true;
            }
            // // needs optimization
            // double artistScore = this.levenstein.GetSimilarity(this.artist, iTunesSong.Artist);
            // double titleScore = this.levenstein.GetSimilarity(this.title, iTunesSong.Name);
            
            // if (artistScore > ARTIST_TH && titleScore > TITLE_TH){
            //     return true;
            // }
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
