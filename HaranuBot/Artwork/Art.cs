using System;
using System.Collections.Generic;
using System.Text;

namespace HaranuBot.Artwork
{
    class Art
    {
        public string Artist { get; set; }
        public string URL { get; set; }

        public Art(string character, string text)
        {
            Artist = character;
            URL = text;
        }

        public string GetFormattedArt()
        {
            return string.Format("{0} - {1}", URL, Artist.ToTitleCase());
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Artist, URL);
        }
    }
}
