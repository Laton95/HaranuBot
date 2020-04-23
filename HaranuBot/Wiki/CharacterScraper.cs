using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaranuBot.Wiki
{
    class CharacterScraper
    {
        public Character GetCharacter(string name)
        {
            HtmlWeb web = new HtmlWeb();

            HtmlDocument doc = web.Load(WikiScraper.GetSearchURL(name));

            var characterSearchResult = doc.DocumentNode.SelectNodes("//article[@class='gamecharacter search-result']");

            if (characterSearchResult != null)
            {
                string characterURL = WikiScraper.GetObsidianURL() + characterSearchResult.First().ChildNodes[1].Attributes.First().Value;

                doc = web.Load(characterURL);

                HtmlNode details = doc.DocumentNode.SelectNodes("//section[@id='character-details']").First();

                string characterName = details.SelectNodes("//h2[@class='character-name title']").First().InnerText;
                string tagline = details.SelectNodes("//div[@class='tagline']").First().InnerText;
                string bio = details.SelectNodes("//div[@class='bio']").First().InnerText;
                string imageURL = doc.DocumentNode.SelectNodes("//div[@class='character-avatar']").First().ChildNodes[1].ChildNodes[1].Attributes[1].Value;

                bio = bio.Substring(5);
                bio = System.Net.WebUtility.HtmlDecode(bio);

                return new Character
                {
                    ImageURL = imageURL,
                    obsidianURL = characterURL,
                    Name = characterName,
                    Tagline = tagline,
                    Bio = bio
                };
            }
            else
            {
                return null;
            }
        }
    }
}
