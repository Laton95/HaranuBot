using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaranuBot.Wiki
{
    class WikiScraper
    {
        public List<KeyValuePair<string, string>> Search(string search, int amount)
        {
            List<KeyValuePair<string, string>> results = new List<KeyValuePair<string, string>>();

            HtmlWeb web = new HtmlWeb();

            HtmlDocument doc = web.Load(GetSearchURL(search));
            
            HtmlNodeCollection articles = doc.DocumentNode.SelectNodes("//ul[@class='results-list no-bullet']").First().SelectNodes("//article");

            if (articles != null)
            {
                foreach (HtmlNode item in articles.Take(amount))
                {
                    results.Add(new KeyValuePair<string, string>(item.ChildNodes[1].FirstChild.InnerText, GetObsidianURL() + item.ChildNodes[1].Attributes.First().Value));
                }

                return results;
            }
            else
            {
                return null;
            }
        }

        public static string GetObsidianURL()
        {
            return "https://" + Program.options.ObsidianPage + ".obsidianportal.com";
        }

        public static string GetSearchURL(string search)
        {
            return "https://" + Program.options.ObsidianPage + ".obsidianportal.com/search?utf8=%E2%9C%93&q=" + search;
        }

        public static string GetMainURL()
        {
            return "https://" + Program.options.ObsidianPage + ".obsidianportal.com/wikis/main-page";
        }
    }    
}
