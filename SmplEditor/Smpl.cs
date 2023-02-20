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
        public List<Song> members { get; set; }
        public int recentlyPlayedDate { get; set; }
        public int sortBy { get; set; }
        public int version { get; set; }

        // vvv All the methods requires revision vvv
        public Smpl(string name,List<Song> songList)
        {
            this.name = name;
            this.members = Smpl.DeepCopySongList(songList);
            int i = 0;
            foreach (SmplSong member in members)
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
        public bool CompareWith(Song target){
            return false;
        }
        public void AddSong(SmplSong addition)
        {
            SmplSong newAddition = addition.DeepCopy();
            newAddition.order = this.members.Count;
            this.members.Add(newAddition);
        }
        public void AddSongs(List<SmplSong> additions)
        {
            // int i = this.members.Count;
            foreach (SmplSong song in additions)
            {
                AddSong(song);
            }
        }
        public void AddSongs(SmplSong[] additions)
        {
            // int i = this.members.Count;
            foreach (SmplSong song in additions)
            {
                AddSong(song);
            }
        }
        public void RemoveSong(SmplSong removingSong)
        {
            int removedOrder = removingSong.order;
            try
            {
                foreach (SmplSong member in this.members)
                {
                    if (SmplSong.DeepCompareSongs(member, removingSong))
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
        public void RemoveSongs(List<SmplSong> removingSongs)
        {
            removingSongs.Sort((SmplSong x, SmplSong y) => -x.order.CompareTo(y.order));
            foreach (SmplSong removingSong in removingSongs)
            {
                this.RemoveSong(removingSong);
            }
            
        }
        public void RemoveSongs(SmplSong[] removingSongs)
        {
            Array.Sort(removingSongs, (SmplSong x, SmplSong y) => -x.order.CompareTo(y.order));
            foreach (SmplSong removingSong in removingSongs)
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
            this.members.Sort((SmplSong x, SmplSong y) => x.info.CompareTo(y.info));
        }
        public static List<Song> DeepCopySongList(List<Song> songList)
        {
            List<SmplSong> coppiedList = new List<SmplSong>();
            foreach (SmplSong song in songList)
            {
                coppiedList.Add(song.DeepCopy());
            }
            return coppiedList;
        }
    }
}
