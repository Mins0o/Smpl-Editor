using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmplEditor
{
    internal class Playlist<T>
    {
        T data;
        bool type;
        public virtual void AddSongs(Song song)
        {
            return;
        }
        public virtual void DeleteSongs()
        {
            return;
        }
        public virtual void AddSong()
        {
            return;
        }
        public virtual void DeleteSong()
        {
            return;
        }
    }
}
