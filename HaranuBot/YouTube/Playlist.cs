using System;
using System.Collections.Generic;
using System.Text;

namespace HaranuBot.YouTube
{
    public class Playlist
    {
        public string URL { get; set; }

        public Playlist(string url)
        {
            URL = url;
        }
    }
}
