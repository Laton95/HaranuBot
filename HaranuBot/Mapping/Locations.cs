﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HaranuBot.Mapping
{
    public static class Locations
    {
        private static readonly string locationsJson = "resources/locations.json";

        private static Dictionary<string, Location> locations;
        private static Dictionary<string, Location> LocationsList
        {
            get
            {
                if (locations == null)
                {
                    LoadLocations();
                }
                else if (fileDate < File.GetLastWriteTimeUtc(locationsJson))
                {
                    LoadLocations();
                }

                return locations;
            }
        }
        private static DateTime fileDate;

        public static void LoadLocations()
        {
            locations = new Dictionary<string, Location>();
            fileDate = File.GetLastWriteTimeUtc(locationsJson);
            using (StreamReader file = new StreamReader(locationsJson))
            {
                JsonTextReader reader = new JsonTextReader(file);
                JObject o = (JObject)JToken.ReadFrom(reader);
                foreach (var location in o.Children())
                {
                    string locationName = location.Path.Replace("['", "").Replace("']", "").Replace("\\", "");
                    locations.Add(locationName, JsonConvert.DeserializeObject<Location>(location.First.ToString()));
                }
            }

            //using (StreamWriter file = new StreamWriter("resources/locations2.json"))
            //{
            //    JsonTextWriter writer = new JsonTextWriter(file);
            //    JObject o = new JObject();
            //    foreach (KeyValuePair<string, Location> entry in LocationsList)
            //    {

            //        o.Add(entry.Key, JObject.FromObject(entry.Value));
            //    }
            //    file.Write(o.ToString());
            //}
        }

        public static List<KeyValuePair<string, Location>> CheckLocations()
        {
            List<KeyValuePair<string, Location>> badLocations = new List<KeyValuePair<string, Location>>();

            foreach (KeyValuePair<string, Location> location in LocationsList)
            {
                Console.WriteLine(location.Key);
                try
                {
                    MapImaging.CreateMap(location.Value);
                }
                catch (Exception)
                {
                    badLocations.Add(location);
                }
                
            }

            return badLocations;
        }

        public static Location GetLocation(string name)
        {   
            return LocationsList.GetValueOrDefault(name.ToLower(), null);
        }

        public static int GetCount()
        {
            return LocationsList.Count;
        }

        public static KeyValuePair<string, Location> GetRandomLocation(Random random)
        {
            var maps = LocationsList.Where(l => l.Value.Map == "gielinor").ToList();
            return maps[random.Next(maps.Count)];
        }
    }
}
