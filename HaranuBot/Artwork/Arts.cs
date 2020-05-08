using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HaranuBot.Artwork
{
    class Arts
    {
        private static readonly string artJson = "resources/arts.json";

        private static List<Art> artList;
        private static List<Art> ArtList
        {
            get
            {
                if (artList == null)
                {
                    LoadArts();
                }

                return artList;
            }
        }
        private static DateTime fileDate;

        private static void LoadArts()
        {
            artList = new List<Art>();

            using (StreamReader file = new StreamReader(artJson))
            {
                JsonTextReader reader = new JsonTextReader(file);
                JObject o = (JObject)JToken.ReadFrom(reader);
                foreach (var artist in o.Children())
                {
                    string artistName = artist.Path;
                    foreach (var item in artist.First.Children())
                    {
                        artList.Add(new Art(artistName.Replace("['", "").Replace("']", "").Replace("\\", ""), item.ToString().Replace("['", "").Replace("']", "").Replace("\\", "")));
                    }
                }
            }
            fileDate = File.GetLastWriteTimeUtc(artJson);
        }

        public static void SaveArts()
        {
            if (artList == null)
            {
                LoadArts();
            }
            else if (fileDate < File.GetLastWriteTimeUtc(artJson))
            {
                LoadArts();
            }

            using (StreamWriter file = new StreamWriter(artJson))
            {
                using (JsonTextWriter writer = new JsonTextWriter(file))
                {
                    JObject o = new JObject();
                    foreach (string name in ArtList.Select(s => s.Artist.ToLower()).Distinct())
                    {
                        JArray arts = new JArray();

                        foreach (Art art in ArtList.Where(q => q.Artist == name))
                        {
                            arts.Add(art.URL);
                        }

                        o.Add(name, arts);
                    }

                    file.Write(o.ToString());
                }
            }
            fileDate = File.GetLastWriteTimeUtc(artJson);
        }

        public static void RefreshArts()
        {
            if (fileDate < File.GetLastWriteTimeUtc(artJson))
            {
                LoadArts();
            }
        }

        public static Art GetRandomArt(Random random)
        {
            RefreshArts();
            return ArtList[random.Next(ArtList.Count)];
        }

        public static List<Art> GetArtistArts(string artist)
        {
            RefreshArts();
            return ArtList.Where(q => q.Artist == artist).ToList();
        }

        public static int GetArtCount()
        {
            RefreshArts();
            return ArtList.Count;
        }

        public static void AddArt(Art art)
        {
            RefreshArts();
            ArtList.Add(art);
            SaveArts();
        }

        public static void RemoveArt(string artistName, int index)
        {
            RefreshArts();
            ArtList.Remove(ArtList.Where(q => q.Artist == artistName).ToList()[index]);
        }

        public static Art GetArt(string artistName, int index)
        {
            RefreshArts();
            return ArtList.Where(q => q.Artist == artistName).ToList()[index];
        }

        public static List<string> GetArtists()
        {
            RefreshArts();
            return ArtList.Select(q => q.Artist).Distinct().ToList();
        }
    }
}
