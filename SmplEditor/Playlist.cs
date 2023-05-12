using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmplEditor
{
    internal class Playlist
    {
        private bool isSmpl;
        private bool isITunes;
        public string Name{
            get{
                if (this.isSmpl){
                    return this.smplProperties.name;
                }
                else{
                    return "iTunes not implemented yet";
                }
            }
        }
        private Smpl smplProperties;
        public Smpl SmplProperties{
            get{
                return this.smplProperties;
            }
        }
        private ITunesPlaylist iTunesProperties;
        private List<Song> listOfTracks;
        public List<Song> ListOfTracks{
            get{
                return this.listOfTracks;
            }
        }
        private Dictionary<Song,int> trackOrdering;
        public Playlist(string name, List<Song> songList){
            isSmpl = true;
            isITunes = false;
            smplProperties = new Smpl();
            smplProperties.name = name;
            smplProperties.version = 1;
            smplProperties.recentlyPlayedDate = 0;
            smplProperties.sortBy = 0;
            listOfTracks = songList;
        }
        public Playlist(Smpl smpl, List<Song> songList){
            isSmpl = true;
            isITunes = false;
            smplProperties = smpl.CloneProperties();
            listOfTracks = songList;
            trackOrdering = smpl.GetOrdering(songList);
        }
        public Playlist(ITunesLibraryParser.Playlist iTunesList, List<Song> songList){
            isITunes = true;
            isSmpl = false;
            listOfTracks = songList;
            ;
        }
        public Playlist(ITunesLibraryParser.Playlist iTunesList, Dictionary<ITunesLibraryParser.Track, Song> trackMapper)
        {
            isITunes = true;
            isSmpl = false;
            var trackList = iTunesList.Tracks.ToList();
            this.listOfTracks = new List<Song>();
            foreach(var iTunesTrack in trackList){
                Song mappedSong = trackMapper[iTunesTrack];
                this.listOfTracks.Add(mappedSong);
            }
        }
        public Playlist(){
            ;
        }
        public void AddSongs(List<Song> songsToAdd)
        {
            this.listOfTracks.AddRange(songsToAdd);
            return;
        }
        public void RemoveSongs(List<Song> tracksToDelete){
            int successCount = 0;
            List<Song> failedTracks = new List<Song>();
            foreach (Song track in tracksToDelete){
                bool success = this.listOfTracks.Remove(track);
                if (success)
                {
                    successCount++;
                }
                else
                {
                    failedTracks.Add(track);
                }
            }
            string debugMessage = "Deleted " + successCount + " tracks from " + this.Name;
            System.Diagnostics.Debug.Print(debugMessage);
            if (failedTracks.Count > 0)
            {
                System.Diagnostics.Debug.Print("These tracks failed while trying to remove from the playlist");
                foreach(Song track in failedTracks)
                {
                    System.Diagnostics.Debug.Print(track.ToString());
                }
            }
            return;
        }
        public override string ToString()
        {
            return this.Name;
        }
        public void SortByOrder(){
            this.listOfTracks.Sort((Song x, Song y) => this.trackOrdering[x].CompareTo(this.trackOrdering[y]));
        }
        public void SortByArtist()
        {
            this.listOfTracks.Sort((Song x, Song y) => x.Artist.CompareTo(y.Artist));
            return;
        }
        public void SortByTitle()
        {
            this.listOfTracks.Sort((Song x, Song y) => x.Title.CompareTo(y.Title));
            return;
        }
        public void SortByDirectory(){
            if (this.isSmpl){
                this.listOfTracks.Sort((Song x, Song y) => x.SmplMusic.info.CompareTo(y.SmplMusic.info));
                return;
            }
            else if (this.isITunes){
                return;
            }
        }
    }
}
