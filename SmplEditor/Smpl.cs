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
        public Smpl(Playlist playlist){
            if(playlist.IsSmpl){
                Smpl smplOfPlaylist = playlist.SmplProperties;
                this.name = smplOfPlaylist.name;
                this.recentlyPlayedDate = smplOfPlaylist.recentlyPlayedDate;
                this.sortBy = smplOfPlaylist.sortBy;
                this.version = smplOfPlaylist.version;
                this.members = new List<SmplSong>();

                int orderCount = 0;
                foreach(Song track in playlist.ListOfTracks){
                    if(track.HasSmplSong()){
                        SmplSong trackToAdd = track.SmplMusic;
                        trackToAdd.order = orderCount++;
                        members.Add(trackToAdd);
                    }
                    else{
                        System.Diagnostics.Debug.WriteLine("{0} does not have a smpl representation", track);
                    }
                }
            }
            else if (playlist.IsITunes){
                ;
            }
            else{
                System.Diagnostics.Debug.WriteLine("This type is not supported");
            }
        }
        public Smpl CloneProperties(){
            Smpl cloned = new Smpl(this.name, 
                                this.recentlyPlayedDate,
                                this.sortBy,
                                this.version);
            return cloned;
        }
        public Dictionary<Song,int> GetOrdering(List<Song> listOfSongs){
            Dictionary<Song, int> orderingMapping;
            if (listOfSongs.Count == this.members.Count){
                orderingMapping = listOfSongs
                    .Select((k, i) => new { k = k, v = this.members[i].order })
                    .ToDictionary(x => x.k, x => x.v);
                System.Diagnostics.Debug.Print("Ordering Generated Succesfully");
            }
            else{
                var debugText = this.name + " - GetOrdering: The number of songs didn't match. The listOfSong has " + listOfSongs.Count + "tracks.";
                System.Diagnostics.Debug.Print(debugText);
                orderingMapping = listOfSongs
                .Select((k, i) => new { k = k, v = i })
                .ToDictionary(x => x.k, x => x.v);
            } 
            return orderingMapping;
        }
        public override string ToString()
        {
            return this.name;
        }
    }
}
