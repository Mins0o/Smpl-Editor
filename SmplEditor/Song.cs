using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmplEditor
{
    internal class Song
    {
        // This class should be able to accomodate both 
        // iTunes songs' data and smpl songs' data
        public string artist { get; set; }
        public string info { get; set; }
        public int order { get; set; }
        public string title { get; set; }
        public int type { get; set; }
        SmplSong smplSong;
        ITunesLibraryParser.Track iTunesSong;
        bool is_mapped;

        void remove_from_lib()
        {
            return;
        }
    }
}
