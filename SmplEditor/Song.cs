using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmplEditorExperiments
{
    public class Song
    {
        public string artist { get; set; }
        public string info { get; set; }
        public int order { get; set; }
        public string title { get; set; }
        public int type { get; set; }
        public string UpperDirectory()
        {
            return info.Substring(0, info.LastIndexOf('/'));
        }
        public Song Copy()
        {
            return new Song()
            {
                artist = artist,
                info = info,
                order = order,
                title = title,
                type = type
            };
        }
        public override string ToString()
        {
            return artist + "  -  " + title;
        }
        public static bool operator ==(Song a, Song b) =>
            a.info == b.info && a.artist == b.artist && a.title == b.title;
        public static bool operator !=(Song a, Song b) => !(a == b);
    }

}
