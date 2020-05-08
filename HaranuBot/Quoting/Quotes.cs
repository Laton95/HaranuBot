using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HaranuBot.Quoting
{
    public class Quotes
    {
        private static readonly string quoteJson = "resources/quotes.json";

        private static List<Quote> quoteList;
        private static List<Quote> QuoteList {
            get
            {
                if (quoteList == null)
                {
                    LoadQuotes();
                }

                return quoteList;
            }
        }
        private static DateTime fileDate;

        private static void LoadQuotes()
        {
            quoteList = new List<Quote>();
            
            using (StreamReader file = new StreamReader(quoteJson))
            {
                JsonTextReader reader = new JsonTextReader(file);
                JObject o = (JObject) JToken.ReadFrom(reader);
                foreach (var character in o.Children())
                {
                    string characterName = character.Path;
                    foreach (var item in character.First.Children())
                    {
                        quoteList.Add(new Quote(characterName.Replace("['", "").Replace("']", "").Replace("\\", ""), item.ToString().Replace("['", "").Replace("']", "").Replace("\\", "")));
                    }
                }
            }
            fileDate = File.GetLastWriteTimeUtc(quoteJson);
        }

        public static void SaveQuotes()
        {
            if (quoteList == null)
            {
                LoadQuotes();
            }
            else if (fileDate < File.GetLastWriteTimeUtc(quoteJson))
            {
                LoadQuotes();
            }

            using (StreamWriter file = new StreamWriter(quoteJson))
            {
                using (JsonTextWriter writer = new JsonTextWriter(file))
                {
                    JObject o = new JObject();
                    foreach (string name in QuoteList.Select(s => s.Character.ToLower()).Distinct())
                    {
                        JArray quotes = new JArray();

                        foreach (Quote quote in QuoteList.Where(q => q.Character == name))
                        {
                            quotes.Add(quote.Text);
                        }

                        o.Add(name, quotes);
                    }

                    file.Write(o.ToString());
                }            
            }
            fileDate = File.GetLastWriteTimeUtc(quoteJson);
        }

        public static void RefreshQuotes()
        {
            if (fileDate < File.GetLastWriteTimeUtc(quoteJson))
            {
                LoadQuotes();
            }
        }

        public static Quote GetRandomQuote(Random random)
        {
            RefreshQuotes();
            return QuoteList[random.Next(QuoteList.Count)];
        }

        public static List<Quote> GetCharacterQuotes(string character)
        {
            RefreshQuotes();
            return QuoteList.Where(q => q.Character == character).ToList();
        }

        public static bool QuoteExists(Quote quote)
        {
            RefreshQuotes();
            return QuoteList.Contains(quote);
        }

        public static int GetQuoteCount()
        {
            RefreshQuotes();
            return QuoteList.Count;
        }

        public static void AddQuote(Quote quote)
        {
            RefreshQuotes();
            QuoteList.Add(quote);
            SaveQuotes();
        }

        public static void RemoveQuote(string characterName, int index)
        {
            RefreshQuotes();
            QuoteList.Remove(QuoteList.Where(q => q.Character == characterName).ToList()[index]);
        }

        public static Quote GetQuote(string characterName, int index)
        {
            RefreshQuotes();
            return QuoteList.Where(q => q.Character == characterName).ToList()[index];
        }

        public static List<string> GetCharacters()
        {
            RefreshQuotes();
            return QuoteList.Select(q => q.Character).Distinct().ToList();
        }
    }
}
