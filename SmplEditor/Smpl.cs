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
        public Smpl CloneProperties(){
            Smpl cloned = new Smpl(this.name, 
                                this.recentlyPlayedDate,
                                this.sortBy,
                                this.version);
            return cloned;
        }
        public Dictionary<Song,int> GetOrdering(List<Song> listOfSongs){
            var orderingMapping = new Dictionary<Song,int>();
            return orderingMapping;
        }
        public override string ToString()
        {
            return this.name;
        }
    }
}
