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
        public Smpl(string name,List<Song> songList)
        {
            this.name = name;
            this.members = songList;
        }
        public Smpl()
        {
        }
        public void AddSong(Song addition)
        {
            this.members.Add(addition);
        }
        public void AddSongs(List<Song> additions)
        {
            this.members.AddRange(additions);
        }
        public void AddSongs(Song[] additions)
        {
            this.members.AddRange(additions);
        }
        public void RemoveSongs(List<Song> removingSongs)
        {
            foreach (Song removingSong in removingSongs)
            {
                try
                {
                    this.members.Remove(removingSong);
                }
                catch(KeyNotFoundException e)
                {
                    System.Diagnostics.Debug.Print(removingSong.ToString() + " not in " + this.name);
                }catch(Exception e)
                {
                    System.Diagnostics.Debug.Print(e.Message);
                }
            }
            
        }
        public Smpl RemoveSongs(Song[] removingSongs)
        {
            foreach (Song removingSong in removingSongs)
            {
                try
                {
                    this.members.Remove(removingSong);
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
            return this;
        }
        public override string ToString()
        {
            return this.name;
        }
    }
    public class Song
    {
        public string artist { get; set; }
        public string info { get; set; }
        public int order { get; set; }
        public string title { get; set; }
        public int type { get; set; }
        public override string ToString()
        {
            return artist +"-" +title;
        }
    }
}
