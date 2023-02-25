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
    internal class Smpl
    {
        private string name;
        public string Name 
        {
            get { return name; } 
        }
        private List<SmplSong> members;
        public List<SmplSong> Members
        {
            get { return members; }
        }
        private int recentlyPlayedDate;
        public int RecentlyPlayedDate
        {
            get { return recentlyPlayedDate;}
        }
        private int sortBy;
        public int SortBy
        {
            get { return sortBy; }
        }
        private int version;
        public int Version
        {
            get { return version; }
        }
        public Smpl(string name, int recentlyPlayedDate
                    , int sortBy, int version){
            this.name = string.Copy(name);
            this.recentlyPlayedDate = recentlyPlayedDate;
            this.members = new List<SmplSong> ();
            this.sortBy = sortBy;
            this.version = version;
        }
        public Smpl Clone(){
            Smpl cloned = new Smpl(this.name, 
                                this.recentlyPlayedDate,
                                this.sortBy,
                                this.version);
            return cloned;
        }
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
