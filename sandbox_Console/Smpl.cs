using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using Microsoft.Win32;

namespace SmplEditor
{
    internal class Smpl:Playlist
    {
        // field name should match the json format when using
        // the .smpl uses lowercase.
        public string name { get; set; }
        public List<SmplSong> members { get; set; }
        public int recentlyPlayedDate { get; set; }
        public int sortBy { get; set; }
        public int version { get; set; }

        public void SortByArtist()
        {
            this.members.Sort((SmplSong x, SmplSong y) => x.Artist.CompareTo(y.Artist));
        }
        public void SortByTitle()
        {
            this.members.Sort((SmplSong x, SmplSong y) => x.Title.CompareTo(y.Title));
        }
        public void SortByDirectory()
        {
            this.members.Sort((SmplSong x, SmplSong y) => x.Info.CompareTo(y.Info));
        }
        public void SortByOrder()
        {
            this.members.Sort((SmplSong x, SmplSong y) => x.Order.CompareTo(y.Order));
        }
    }
}
