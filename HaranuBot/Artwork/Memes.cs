using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HaranuBot.Artwork
{
    class Memes
    {
        private static readonly string memeJson = "resources/memes.json";

        private static List<Meme> memeList;
        private static List<Meme> MemeList
        {
            get
            {
                if (memeList == null)
                {
                    LoadMemes();
                }

                return memeList;
            }
        }
        private static DateTime fileDate;

        private static void LoadMemes()
        {
            memeList = new List<Meme>();

            using (StreamReader file = new StreamReader(memeJson))
            {
                while (!file.EndOfStream)
                {
                    string memeUrl = file.ReadLine();
                    memeList.Add(new Meme(memeUrl.Replace("['", "").Replace("']", "").Replace("\\", "")));
                }
            }
            fileDate = File.GetLastWriteTimeUtc(memeJson);
        }

        public static void SaveMemes()
        {
            if (memeList == null)
            {
                LoadMemes();
            }
            else if (fileDate < File.GetLastWriteTimeUtc(memeJson))
            {
                LoadMemes();
            }

            using (StreamWriter file = new StreamWriter(memeJson))
            {
                foreach (Meme meme in MemeList)
                {
                    file.WriteLine(meme.URL);
                }
            }
            fileDate = File.GetLastWriteTimeUtc(memeJson);
        }

        public static void RefreshMemes()
        {
            if (fileDate < File.GetLastWriteTimeUtc(memeJson))
            {
                LoadMemes();
            }
        }

        public static Meme GetRandomMeme(Random random)
        {
            RefreshMemes();
            return MemeList[random.Next(MemeList.Count)];
        }

        public static int GetMemeCount()
        {
            RefreshMemes();
            return MemeList.Count;
        }

        public static void AddMeme(Meme meme)
        {
            RefreshMemes();
            MemeList.Add(meme);
            SaveMemes();
        }

        public static void RemoveMeme(string url)
        {
            RefreshMemes();
            MemeList.Remove(MemeList.Where(q => q.URL == url).First());
        }
    }
}
