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
        private void getDirectoryFileName(SmplSong smplTrack, ref string fileName, ref string dirName){
            string trackInfo;
            string urlDecoded;

            trackInfo = smplTrack.info;
            urlDecoded = System.Net.WebUtility.UrlDecode(trackInfo);
            dirName = System.IO.Directory.GetParent(urlDecoded).Name;
            fileName = System.IO.Path.GetFileName(urlDecoded);
        }
        private void getReasonableFileNames(SmplSong thisSong,
                                            SmplSong otherSong,
                                            ref string thisFileName,
                                            ref string otherFileName){
            string thisDirName = "";
            string otherDirName = "";
            this.getDirectoryFileName(thisSong, ref thisFileName, ref thisDirName);
            this.getDirectoryFileName(otherSong, ref otherFileName, ref otherDirName);
            if (thisFileName.Length < 11){ // If the filename itself is too short or not unique
                thisFileName = thisDirName + thisFileName;
                otherFileName = otherDirName+otherFileName;
            }
            return;
        }
        private void getReasonableFileNames(SmplSong thisSong,
                                            ITunesLibraryParser.Track otherSong,
                                            ref string thisFileName,
                                            ref string otherFileName){
            string thisDirName = "";
            string otherDirName = "";
            this.getDirectoryFileName(thisSong, ref thisFileName, ref thisDirName);
            this.getDirectoryFileName(otherSong, ref otherFileName, ref otherDirName);
            if (thisFileName.Length < 11){ // If the filename itself is too short or not unique
                thisFileName = thisDirName + thisFileName;
                otherFileName = otherDirName+otherFileName;
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
        public bool IsSoftEqualTo(SmplSong smplSong){
            bool isSimilarArtist = this.levenstein.GetSimilarity(this.artist, smplSong.artist) > ARTIST_TH;
            bool isSimilarTitle = this.levenstein.GetSimilarity(this.title, smplSong.title) > TITLE_TH;
            string thisFileName = "";
            string otherFileName = "";
            getReasonableFileNames(this, smplSong, ref thisFileName, ref otherFileName);
            bool isSimilarFile = this.levenstein.GetSimilarity(thisFileName, otherFileName) > FILENAME_TH;
            if (isSimilarArtist && isSimilarTitle && isSimilarFile){
                return true;
            }
            return false;
        }
        public bool IsSoftEqualTo(ITunesLibraryParser.Track iTunesSong){
            bool isSimilarArtist = this.levenstein.GetSimilarity(this.artist, iTunesSong.Artist) > ARTIST_TH;
            bool isSimilarTitle = this.levenstein.GetSimilarity(this.title, iTunesSong.Name) > TITLE_TH;
            string thisFileName = "";
            string otherFileName = "";
            getReasonableFileNames(this, iTunesSong, ref thisFileName, ref otherFileName);
            bool isSimilarFile = this.levenstein.GetSimilarity(thisFileName, otherFileName) > FILENAME_TH;
            if (isSimilarArtist && isSimilarTitle && isSimilarFile){
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
