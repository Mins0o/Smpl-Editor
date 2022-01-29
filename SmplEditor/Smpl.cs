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
        public string Name { get; set; }
        public List<Song> Members { get; set; }
        public int RecentlyPlayedDate { get; set; }
        public int SortBy { get; set; }
        public int Version { get; set; }
        public Smpl(string name,List<Song> songList)
        {
            this.Name = name;
            this.Members = Smpl.DeepCopySongList(songList);
            int i = 0;
            foreach (Song member in Members)
            {
                member.Order = i++;
            }
            this.RecentlyPlayedDate = 1595831442;
            this.SortBy = 4;
            this.Version = 1;
        }
        public Smpl()
        {
            this.RecentlyPlayedDate = 1595831442;
            this.SortBy = 4;
            this.Version = 1;
        }
        public void AddSong(Song addition)
        {
            Song newAddition = addition.DeepCopy();
            newAddition.Order = this.Members.Count;
            this.Members.Add(newAddition);
        }
        public void AddSongs(List<Song> additions)
        {
            int i = this.Members.Count;
            foreach (Song song in additions)
            {
                AddSong(song);
            }
        }
        public void AddSongs(Song[] additions)
        {
            int i = this.Members.Count;
            foreach (Song song in additions)
            {
                AddSong(song);
            }
        }
        public void RemoveSong(Song removingSong)
        {
            int removedOrder = removingSong.Order;
            try
            {
                foreach (Song member in this.Members)
                {
                    if (Song.DeepCompareSongs(member, removingSong))
                    {
                        this.Members.Remove(member);
                        if (member.Order > removedOrder)
                        {
                            member.Order -= 1;
                        }
                    }
                }
            }
            catch (KeyNotFoundException e)
            {
                System.Diagnostics.Debug.Print(removingSong.ToString() + " not in " + this.Name);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Print(e.Message);
            }
        }
        public void RemoveSongs(List<Song> removingSongs)
        {
            removingSongs.Sort((Song x, Song y) => -x.Order.CompareTo(y.Order));
            foreach (Song removingSong in removingSongs)
            {
                this.RemoveSong(removingSong);
            }
            
        }
        public void RemoveSongs(Song[] removingSongs)
        {
            Array.Sort(removingSongs, (Song x, Song y) => -x.Order.CompareTo(y.Order));
            foreach (Song removingSong in removingSongs)
            {
                this.RemoveSong(removingSong);
            }
        }
        public override string ToString()
        {
            return this.Name;
        }
        public void SortByArtist()
        {
            this.Members.Sort((Song x, Song y) => x.Artist.CompareTo(y.Artist));
        }
        public void SortByTitle()
        {
            this.Members.Sort((Song x, Song y) => x.Title.CompareTo(y.Title));
        }
        public void SortByDirectory()
        {
            this.Members.Sort((Song x, Song y) => x.Info.CompareTo(y.Info));
        }
        public void SortByOrder()
        {
            this.Members.Sort((Song x, Song y) => x.Info.CompareTo(y.Info));
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
        public string Artist { get; set; }
        public string Info { get; set; }
        public int Order { get; set; }
        public string Title { get; set; }
        public int Type { get; set; }
        public string UpperDirectory()
        {
            return Info.Substring(0, Info.LastIndexOf('/'));
        }
        public Song DeepCopy()
        {
            Song copy = new Song();
            copy.Artist = string.Copy(this.Artist);
            copy.Info = string.Copy(this.Info);
            copy.Order = this.Order;
            copy.Title = string.Copy(this.Title);
            copy.Type = this.Type;
            return copy;
        }
        public override string ToString()
        {
            return Artist +"  -  " +Title;
        }
        public static bool DeepCompareSongs(Song x, Song y,bool strict=true)
        {
            bool result = true;
            if (strict)
            {
                result = result && (x.Info == y.Info);
            }
            result = result && (x.Artist == y.Artist && x.Title == y.Title);
            return result;
        }
    }
}
