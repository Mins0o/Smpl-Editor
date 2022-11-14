﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmplEditor
{
    internal class Song
    {
        // field name should match the json format when using
        // the .smpl uses lowercase.
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
