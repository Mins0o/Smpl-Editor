using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmplEditor
{
    public class Song
    {
        // This class should be able to accomodate both 
        // iTunes songs' data and smpl songs' data
        public string artist { get; set; }
        public string info { get; set; }
        public int order { get; set; }
        public string title { get; set; }
        public int type { get; set; }

        // An iTunes song have
        //        Album string
        //        AlbumArtist string
        //        AlbumRating int?
        //        AlbumRatingComputed bool
        //        Artist string -------------------*
        //        BitRate int?
        //        Composer string
        //+		  DateAdded	System.DateTime
        //+		  DateModified System.DateTime?
        //        Genre string
        //		  Kind string
        //		  Location string
        //		  Name string   -------------------*
        //		  PartOfCompilation bool
        //		  PersistentId string
        //		  PlayCount int?
        //+		  PlayDate System.DateTime?
        //        PlayingTime string
        //		  Rating int?
        //		  SampleRate int?
        //		  Size long?
        //		  TrackId int
        //		  TrackNumber int?
        //		  Year int?

        SmplSong smplSong;
        ITunesLibraryParser.Track iTunesSong;
        bool is_mapped;

        void remove_from_lib()
        {
            return;
        }
    }
}
