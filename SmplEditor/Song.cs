using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmplEditor
{
    public class Song
    {
        // field name should match the json format when using
        // the .smpl uses lowercase.
        public string artist { get; set; }
        public string info { get; set; }
        public int order { get; set; }
        public string title { get; set; }
        public int type { get; set; }
        public string UpperDirectory()
        {
            return info.Substring(0, info.LastIndexOf('/'));
        }
        public Song DeepCopy() //!!do I need a deepcopy?
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
            return artist + "  -  " + title;
        }
        public static bool DeepCompareSongs(Song x, Song y, bool strict = true) //!!why do I need this?
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
    /*public class Song
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
    }*/

}
