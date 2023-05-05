﻿using System;
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
        public Song(SmplSong smplMusic) {
            this.smplMusic = smplMusic;
            this.title = smplMusic.title;
            this.artist = smplMusic.artist;
        }

        public bool HasSmplSong() {
            return this.smplMusic != default(SmplSong);
        }
        public bool HasITunesSong() {
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

        public bool IsEqualTo(SmplSong smplSong) {
            if (this.HasSmplSong()) {
                return smplSong.IsEqualTo(this.SmplMusic);
            }
            if (this.HasITunesSong()) {
                return smplSong.IsEqualTo(this.ITunesSong);
            }
            System.Diagnostics.Debug.Print("Song.CompareWith: Not implemented for this type");
            return false;
        }
        public bool IsEqualTo(ITunesLibraryParser.Track iTunesSong) {
            if (this.HasSmplSong()) {
                return this.SmplMusic.IsEqualTo(iTunesSong);
            }
            else if (this.HasITunesSong()) {
                double artistScore = this.levenstein.GetSimilarity(this.ITunesSong.Artist, iTunesSong.Artist);
                double titleScore = this.levenstein.GetSimilarity(this.ITunesSong.Name, iTunesSong.Name);
                double albumScore = this.levenstein.GetSimilarity(this.ITunesSong.Album, iTunesSong.Album);

                if (artistScore > 0.9 && titleScore > 0.8 && albumScore > 0.8) {
                    return true;
                }
            }
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

            }
            else{
                System.Diagnostics.Debug.Print("Song.CompareByOrder: This case is not handled");
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
