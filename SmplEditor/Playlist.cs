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
        private ITunesPlaylist iTunesProperties;
        private List<Song> listOfTracks;
        public List<Song> ListOfTracks{
            get{
                return this.listOfTracks;
            }
        }
        public Playlist(string name, List<Song> songList){
            isSmpl = true;
            isITunes = false;
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
        }
        public Playlist(){
            ;
        }
        public void AddSongs(List<Song> song)
        {
            System.Diagnostics.Trace.WriteLine("AddSongs: Not implemented");
            return;
        }
        public void AddSongs(SmplSong[] songs){
            System.Diagnostics.Trace.WriteLine("AddSongs: Not implemented");
            return;
        }
        public void DeleteSongs()
        {
            System.Diagnostics.Trace.WriteLine("DeleteSongs: Not implemented");
            return;
        }
        public void AddSong()
        {
            System.Diagnostics.Trace.WriteLine("AddSong: Not implemented");
            return;
        }
        public void DeleteSong()
        {
            System.Diagnostics.Trace.WriteLine("DeleteSong: Not implemented");
            return;
        }
        public void RemoveSongs(List<Song> songsToDelete){
            System.Diagnostics.Trace.WriteLine("Removesongs: Not implemented");
            return;
        }
        public override string ToString()
        {
            return this.Name;
        }
    }
}
