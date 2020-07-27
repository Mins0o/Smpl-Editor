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
    public class Smpl
    {
        public string name { get; set; }
        public List<Song> members { get; set; }
        public int recentlyPlayedDate { get; set; }
        public int sortBy { get; set; }
        public int version { get; set; }
        public Smpl(string name,List<Song> songList)
        {
            this.name = name;
            this.members = Smpl.DeepCopySongList(songList);
            int i = 0;
            foreach (Song member in members)
            {
                member.order = i++;
            }
            this.recentlyPlayedDate = 1595831442;
            this.sortBy = 4;
            this.version = 1;
        }
        public Smpl()
        {
            this.recentlyPlayedDate = 1595831442;
            this.sortBy = 4;
            this.version = 1;
        }
        public void AddSong(Song addition)
        {
            Song newAddition = addition.DeepCopy();
            newAddition.order = this.members.Count;
            this.members.Add(newAddition);
        }
        public void AddSongs(List<Song> additions)
        {
            int i = this.members.Count;
            foreach (Song song in additions)
            {
                AddSong(song);
            }
        }
        public void AddSongs(Song[] additions)
        {
            int i = this.members.Count;
            foreach (Song song in additions)
            {
                AddSong(song);
            }
        }
        public void RemoveSong(Song removingSong)
        {
            int removedOrder = removingSong.order;
            try
            {
                foreach (Song member in this.members)
                {
                    if (Song.DeepCompareSongs(member, removingSong))
                    {
                        this.members.Remove(member);
                        if (member.order > removedOrder)
                        {
                            member.order -= 1;
                        }
                    }
                }
            }
            catch (KeyNotFoundException e)
            {
                System.Diagnostics.Debug.Print(removingSong.ToString() + " not in " + this.name);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Print(e.Message);
            }
        }
        public void RemoveSongs(List<Song> removingSongs)
        {
            foreach (Song removingSong in removingSongs)
            {
                this.RemoveSong(removingSong);
            }
            
        }
        public void RemoveSongs(Song[] removingSongs)
        {
            foreach (Song removingSong in removingSongs)
            {
                this.RemoveSong(removingSong);
            }
        }
        public override string ToString()
        {
            return this.name;
        }
        public void SortByArtist()
        {
            this.members.Sort((Song x, Song y) => x.artist.CompareTo(y.artist));
        }
        public void SortByTitle()
        {
            this.members.Sort((Song x, Song y) => x.title.CompareTo(y.title));
        }
        public void SortByDirectory()
        {
            this.members.Sort((Song x, Song y) => x.info.CompareTo(y.info));
        }
        public void SortByOrder()
        {
            this.members.Sort((Song x, Song y) => x.info.CompareTo(y.info));
        }
        public static List<Song> DeepCopySongList(List<Song> songList)
        {
            List<Song> coppiedList = new List<Song>();
            foreach (Song song in songList)
            {
                coppiedList.Add(song.DeepCopy());
            }
            return coppiedList;
        }
    }
    public class Song
    {
        public string artist { get; set; }
        public string info { get; set; }
        public int order { get; set; }
        public string title { get; set; }
        public int type { get; set; }
        public string upperDirectory()
        {
            return info.Substring(0, info.LastIndexOf('/'));
        }
        public Song DeepCopy()
        {
            Song copy = new Song();
            copy.artist = string.Copy(this.artist);
            copy.info = string.Copy(this.info);
            copy.order = this.order;
            copy.title = string.Copy(this.title);
            copy.type = this.type;
            return copy;
        }
        public override string ToString()
        {
            return artist +"  -  " +title;
        }
        public static bool DeepCompareSongs(Song x, Song y,bool strict=true)
        {
            bool result = true;
            if (strict)
            {
                result = result && (x.info == y.info);
            }
            result = result && (x.artist == y.artist && x.title == y.title);
            return result;
        }
    }
}
