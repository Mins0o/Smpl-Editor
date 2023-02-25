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
        private Smpl smplProperties;
        private ITunesPlaylist iTunesProperties;
        private List<Song> listOfTracks;
        public Playlist(Smpl smpl, List<Song> songList){
            isSmpl = true;
            isITunes = false;
            smplProperties = smpl.Clone();
            listOfTracks = songList;
        }
        public virtual void AddSongs(Song song)
        {
            System.Diagnostics.Trace.WriteLine("AddSongs: Not implemented");
            return;
        }
        public virtual void DeleteSongs()
        {
            System.Diagnostics.Trace.WriteLine("DeleteSongs: Not implemented");
            return;
        }
        public virtual void AddSong()
        {
            System.Diagnostics.Trace.WriteLine("AddSong: Not implemented");
            return;
        }
        public virtual void DeleteSong()
        {
            System.Diagnostics.Trace.WriteLine("DeleteSong: Not implemented");
            return;
        }
    }
}
