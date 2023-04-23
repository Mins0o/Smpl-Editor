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
        public string name {get; set;}
        public List<SmplSong> members {get; set;}
        public int recentlyPlayedDate {get; set;}
        public int sortBy {get; set;}
        public int version {get; set;}
        public Smpl(){
            ;
        }
        public Smpl(string name, int recentlyPlayedDate
                    , int sortBy, int version){
            this.name = string.Copy(name);
            this.recentlyPlayedDate = recentlyPlayedDate;
            this.members = new List<SmplSong> ();
            this.sortBy = sortBy;
            this.version = version;
        }
        public Smpl(string name, List<SmplSong> songList){
            ;
        }
        public Smpl CloneProperties(){
            Smpl cloned = new Smpl(this.name, 
                                this.recentlyPlayedDate,
                                this.sortBy,
                                this.version);
            return cloned;
        }
        public void SortByArtist()
        {
            this.members.Sort((SmplSong x, SmplSong y) => x.artist.CompareTo(y.artist));
        }
        public void SortByTitle()
        {
            this.members.Sort((SmplSong x, SmplSong y) => x.title.CompareTo(y.title));
        }
        public void SortByDirectory()
        {
            this.members.Sort((SmplSong x, SmplSong y) => x.info.CompareTo(y.info));
        }
        public void SortByOrder()
        {
            this.members.Sort((SmplSong x, SmplSong y) => x.order.CompareTo(y.order));
        }

        public void AddSongs(SmplSong[] selectedSongs){
            ;
        }
        public void RemoveSongs(SmplSong[] songsToDelete){
            ;
        }
        public override string ToString()
        {
            return this.name;
        }
    }
}
