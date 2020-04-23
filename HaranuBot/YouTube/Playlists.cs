using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HaranuBot.YouTube
{
    public class Playlists
    {
        private static readonly string playlistsJson = "resources/playlists.json";

        private static Dictionary<string, Playlist> playlists;
        public static Dictionary<string, Playlist> PlaylistList
        {
            get
            {
                if (playlists == null)
                {
                    LoadPlaylists();
                }
                else if (fileDate < File.GetLastWriteTimeUtc(playlistsJson))
                {
                    LoadPlaylists();
                }

                return playlists;
            }
        }
        private static DateTime fileDate;

        private static void LoadPlaylists()
        {
            playlists = new Dictionary<string, Playlist>();
            fileDate = File.GetLastWriteTimeUtc(playlistsJson);
            using (StreamReader file = new StreamReader(playlistsJson))
            {
                JsonTextReader reader = new JsonTextReader(file);
                JObject o = (JObject)JToken.ReadFrom(reader);
                foreach (var playlist in o.Children())
                {
                    string playlistName = playlist.Path.Replace("['", "").Replace("']", "").Replace("\\", "");
                    playlists.Add(playlistName, JsonConvert.DeserializeObject<Playlist>(playlist.First.ToString()));
                }
            }

            //using (StreamWriter file = new StreamWriter("resources/playlists2.json"))
            //{
            //    JsonTextWriter writer = new JsonTextWriter(file);
            //    JObject o = new JObject();
            //    foreach (KeyValuePair<string, Playlist> entry in PlaylistList)
            //    {
            //        o.Add(entry.Key, JObject.FromObject(entry.Value));
            //    }
            //    file.Write(o.ToString());
            //}
        }
    }
}
