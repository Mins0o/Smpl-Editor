using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmplEditor
{
    public class Smpl
    {
        public Song[] members { get; set; }
    }
    public class Song
    {
        public string artist { get; set; }
        public string info { get; set; }
        public int order { get; set; }
        public string title { get; set; }
        public int type { get; set; }
    }
}
