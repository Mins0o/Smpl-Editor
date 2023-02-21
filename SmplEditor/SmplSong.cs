using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmplEditor
{
    public class SmplSong
    {
        // field name should match the json format when using
        // the .smpl uses lowercase.
        public string artist { get; set; }
        public string info { get; set; }
        public int order { get; set; }
        public string title { get; set; }
        public int type { get; set; }
        public int extraStuff { get; set; }
        public string UpperDirectory()
        {
            return info.Substring(0, info.LastIndexOf('/'));
        }

        public bool Compare(Song song){
            return false;
        }
    }
}
