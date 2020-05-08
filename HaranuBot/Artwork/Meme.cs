using System;
using System.Collections.Generic;
using System.Text;

namespace HaranuBot.Artwork
{
    class Meme
    {
        public string URL { get; set; }

        public Meme(string uRL)
        {
            URL = uRL;
        }

        public string GetFormattedUrl()
        {
            return string.Format("{0}", URL);
        }
    }
}
