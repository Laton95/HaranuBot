using Discord;
using Discord.Commands;
using HaranuBot.Artwork;
using HaranuBot.Currency;
using HaranuBot.Mapping;
using HaranuBot.Quoting;
using HaranuBot.Wiki;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HaranuBot.Currency.CurrencyConversion;
using static HaranuBot.PBDBG.Rolling;

namespace HaranuBot
{
    class Commands : ModuleBase<SocketCommandContext>
    {
        private CommandService service;

        private Random random = new Random();

        public Commands(CommandService service)
        {
            this.service = service;
        }

        [Command("wiki"), Summary("Search the Obsidian portal wiki")]
        public async Task Wiki(params string[] inputs)
        {
            if (inputs.Length == 0)
            {
                EmbedBuilder embed = new EmbedBuilder();
                embed.Title = "Obsidian Portal Wiki";
                embed.Url = WikiScraper.GetMainURL();
                await ReplyAsync("", false, embed.Build());
                return;
            }

            string search = inputs.BuildName();

            WikiScraper scraper = new WikiScraper();
            var results = scraper.Search(search, 5);

            if (results != null)
            {
                EmbedBuilder embed = new EmbedBuilder();
                embed.WithTitle("Articles Mentioning '" + search + "'");

                foreach (var article in results)
                {
                    EmbedFieldBuilder field = new EmbedFieldBuilder();
                    field.WithName(article.Key);
                    field.WithValue(article.Value);
                    embed.AddField(field);
                }

                await ReplyAsync("", false, embed.Build());
            }
            else
            {
                await ReplyAsync("No articles found.");
            }
        }

        [Command("character"), Summary("Get details on a character")]
        public async Task Character(params string[] inputs)
        {
            if (inputs.Length == 0)
            {
                await ReplyAsync("The input text has too few parameters.");
                return;
            }

            string name = inputs.BuildName();

            CharacterScraper scraper = new CharacterScraper();

            Character character = scraper.GetCharacter(name);

            if (character != null)
            {
                EmbedBuilder embed = new EmbedBuilder();
                embed.WithTitle(character.Name);
                embed.WithUrl(character.obsidianURL);
                embed.WithDescription(character.Tagline);
                embed.WithImageUrl(character.ImageURL);

                EmbedFieldBuilder field = new EmbedFieldBuilder();
                field.WithName("Bio");

                if (character.Bio.Length > 1024)
                {
                    field.WithValue(character.Bio.Substring(0, 1021) + "...");
                }
                else
                {
                    field.WithValue(character.Bio);
                }
                embed.AddField(field);

                await ReplyAsync("", false, embed.Build());
            }
            else
            {
                await ReplyAsync("No character could be found with name: " + name);
            }
        }

        [Command("session"), Summary("Get time until next session")]
        public async Task Session()
        {
            DateTime now = DateTime.UtcNow;

            int sessionHour = Program.options.DaylightSavings ? 20 : 21;

            DateTime session = new DateTime(now.Year, now.Month, now.Day, sessionHour, 0, 0);
            session = session.AddDays(((int)DayOfWeek.Friday - (int)now.DayOfWeek + 7) % 7);

            TimeSpan span = session.Subtract(now);

            if (span.Hours < 0 || (span.Days == 6 && span.Hours > 20))
            {
                await ReplyAsync("Session is currently ongoing!");
            }
            else
            {
                await ReplyAsync(string.Format("The next session is in {0} days, {1} hours and {2} minutes.", span.Days, span.Hours, span.Minutes));
            }
        }

        [Command("toggledaylightsavings"), Summary("Toggle daylight savings when calculating session time")]
        public async Task ToggleDaylightSavings()
        {
            Program.options.DaylightSavings = !Program.options.DaylightSavings;

            using (StreamWriter file = new StreamWriter("resources/options.json"))
            {
                using (JsonTextWriter writer = new JsonTextWriter(file))
                {
                    file.Write(JsonConvert.SerializeObject(Program.options, Formatting.Indented));
                }
            }

            await ReplyAsync(string.Format("Daylight savings is now {0}.", Program.options.DaylightSavings ? "on" : "off"));
        }

        [Command("quote"), Summary("Quote commands")]
        public async Task GetQuote(params string[] args)
        {
            if (args.Length == 0)
            {
                await ReplyAsync(Quotes.GetRandomQuote(random).GetFormattedQuote());
            }
            else if (args[0] == "add")
            {
                if (args.Length > 2)
                {
                    string name = args.BuildName(1, args.Length - 2).ToLower();
                    string text = args[args.Length - 1];
                    Quote quote = new Quote(name, text);
                    if (!Quotes.QuoteExists(quote))
                    {
                        Quotes.AddQuote(quote);
                        await ReplyAsync("Added quote.");
                    }
                    else
                    {
                        await ReplyAsync("Quote already exists.");
                    }
                }
                else
                {
                    await ReplyAsync("You need to say which character said the quote ya silly doofus " + Context.User.Mention);
                }
                          
            }
            else if (args[0] == "count")
            {
                await ReplyAsync(string.Format("There are {0} quotes stored.", Quotes.GetQuoteCount()));
            }
            else if (args[0] == "characters")
            {
                List<string> characters = Quotes.GetCharacters();
                if (characters.Count > 0)
                {
                    StringBuilder builder = new StringBuilder();
                    builder.Append(string.Format("Characters with quotes:{0}", Environment.NewLine));
                    
                    foreach (string character in characters)
                    {
                        builder.Append(string.Format("{0}{1}", character.ToTitleCase(), Environment.NewLine));
                    }

                    await ReplyAsync(builder.ToString());
                }
                else
                {
                    await ReplyAsync("There are currently no characters with quotes.");
                }
            }
            else if (args[0] == "list")
            {
                string name = args.BuildName(1, args.Length - 1).ToLower();

                List<Quote> quotes = Quotes.GetCharacterQuotes(name);
                if (quotes.Count > 0)
                {
                    StringBuilder builder = new StringBuilder();
                    builder.Append(string.Format("Quotes attributed to {0}:{1}", name.ToTitleCase(), Environment.NewLine));

                    int i = 1;
                    foreach (Quote quote in quotes)
                    {
                        string line = string.Format("{0} - {1}{2}", i, quote.Text, Environment.NewLine);
                        if (builder.Length + line.Length <= 2000)
                        {
                            builder.Append(line);
                        }
                        else
                        {
                            await ReplyAsync(builder.ToString());
                            builder = new StringBuilder();
                        }

                        i++;
                    }

                    await ReplyAsync(builder.ToString());
                }
                else
                {
                    await ReplyAsync(string.Format("{0} has no quotes.", name.ToTitleCase()));
                }
            }
            else if (args[0] == "remove")
            {
                string name = args.BuildName(1, args.Length - 2).ToLower();
                int index = int.Parse(args[args.Length - 1]) - 1;

                try
                {
                    Quote quote = Quotes.GetQuote(name, index);
                    Quotes.RemoveQuote(name, index);
                    Quotes.SaveQuotes();
                    await ReplyAsync(string.Format("Quote removed from {0}: \"{1}\"", name, quote.Text));
                }
                catch (ArgumentOutOfRangeException)
                {
                    await ReplyAsync("Invalid quote number.");
                }
            }
            else
            {
                if (int.TryParse(args.Last(), out int index))
                {
                    string name = args.BuildName(0, args.Length - 2).ToLower();

                    index -= 1;

                    try
                    {
                        await ReplyAsync(Quotes.GetQuote(name, index).GetFormattedQuote());
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        await ReplyAsync(string.Format("{0} has no quote at index {1}.", name.ToTitleCase(), index));
                    }                 
                }
                else
                {
                    string name = args.BuildName().ToLower();

                    List<Quote> quotes = Quotes.GetCharacterQuotes(name);
                    if (quotes.Count > 0)
                    {
                        Quote quote = quotes[random.Next(quotes.Count)];
                        if (quote != null)
                        {
                            await ReplyAsync(quote.GetFormattedQuote());
                        }
                    }
                    else
                    {
                        await ReplyAsync(string.Format("{0} has no quotes.", name.ToTitleCase()));
                    }
                }
            }
        }

        [Command("art"), Summary("Art commands")]
        public async Task ArtCommands(params string[] args)
        {
            if (args.Length == 0)
            {
                await ReplyAsync(Arts.GetRandomArt(random).GetFormattedArt());
            }
            else if (args[0] == "add")
            {
                if (args.Length > 2)
                {
                    string name = args.BuildName(1, args.Length - 2).ToLower();
                    string url = args[args.Length - 1];
                    Art art = new Art(name, url);
                    Arts.AddArt(art);
                    await ReplyAsync("Added art.");
                }
                else
                {
                    await ReplyAsync("Please say which artist made this " + Context.User.Mention);
                }
            }
            else if (args[0] == "count")
            {
                await ReplyAsync(string.Format("There are {0} arts stored.", Arts.GetArtCount()));
            }
            else if (args[0] == "remove")
            {
                string name = args.BuildName(1, args.Length - 2).ToLower();
                int index = int.Parse(args[args.Length - 1]) - 1;

                try
                {
                    Art art = Arts.GetArt(name, index);
                    Arts.RemoveArt(name, index);
                    Arts.SaveArts();
                    await ReplyAsync(string.Format("Art removed from {0}: \"{1}\"", name, art.URL));
                }
                catch (ArgumentOutOfRangeException)
                {
                    await ReplyAsync("Invalid art number.");
                }
            }
            else
            {
                string name = args.BuildName().ToLower();

                List<Art> arts = Arts.GetArtistArts(name);
                if (arts.Count > 0)
                {
                    Art art = arts[random.Next(arts.Count)];
                    if (art != null)
                    {
                        await ReplyAsync(art.GetFormattedArt());
                    }
                }
                else
                {
                    await ReplyAsync(string.Format("{0} has no arts.", name.ToTitleCase()));
                }
            }
        }

        [Command("meme"), Summary("Meme commands")]
        public async Task MemeCommands(params string[] args)
        {
            if (args.Length == 0)
            {
                await ReplyAsync(Memes.GetRandomMeme(random).GetFormattedUrl());
            }
            else if (args[0] == "add")
            {
                string url = args[1];
                Meme meme = new Meme(url);
                Memes.AddMeme(meme);
                await ReplyAsync("Added meme.");
            }
            else if (args[0] == "count")
            {
                await ReplyAsync(string.Format("There are {0} memes stored.", Memes.GetMemeCount()));
            }
            else if (args[0] == "remove")
            {
                string url = args[1];

                Memes.RemoveMeme(url);
                Memes.SaveMemes();
                await ReplyAsync(string.Format("Meme removed"));
            }
        }

        [Command("map"), Summary("Map related commands")]
        public async Task Map(params string[] args)
        {
            if (args.Length != 0)
            {
                Location location = Locations.GetLocation(args.BuildName());
                if (location != null)
                {
                    MapImaging.CreateMap(location);
                    await Context.Channel.SendFileAsync(MapImaging.imageFile);
                    if (location.Detail != null)
                    {
                        await Context.Channel.SendMessageAsync(location.Detail);
                    }
                }
                else if (args[0] == "count")
                {
                    await ReplyAsync(string.Format("There are currently {0} maps avaliable", Locations.GetCount()));
                }
                else if (MapImaging.GetMap(args.BuildName()))
                {
                    await Context.Channel.SendFileAsync(MapImaging.imageFile);
                }
                else
                {
                    await ReplyAsync("No location with name: " + args.BuildName().ToTitleCase());
                }
            }
            else
            {
                var location = Locations.GetRandomLocation(random);
                MapImaging.CreateMap(location.Value);
                await Context.Channel.SendMessageAsync(location.Key.ToTitleCase());
                await Context.Channel.SendFileAsync(MapImaging.imageFile);
                if (location.Value.Detail != null)
                {
                    await Context.Channel.SendMessageAsync(location.Value.Detail);
                }
            }
        }

        [Command("convert"), Summary("Currency conversion")]
        public async Task Convert(string amountInput, string fromInput, string ignoreThis, string toInput)
        {
            bool validFrom = Enum.TryParse(fromInput.ToUpper(), out CoinType from);
            bool validTo = Enum.TryParse(toInput.ToUpper(), out CoinType to);
            bool validAmount = double.TryParse(amountInput, out double amount);

            if (!validFrom)
            {
                await ReplyAsync(string.Format("'{0}' is not a valid currency", fromInput));
            }
            else if (!validTo)
            {
                await ReplyAsync(string.Format("'{0}' is not a valid currency", toInput));
            }
            else if (!validAmount || amount < 0)
            {
                await ReplyAsync("The amount you gave is invalid");
            }
            else if (amount % 1 != 0)
            {
                await ReplyAsync("Please use whole numbers, you can't have half a coin. Convert to smaller coin values if you need to");
            }
            else if(amount >= 1000000000000000)
            {
                await ReplyAsync("The amount you gave is too large");
            }
            else
            {
                ConversionResult result = CurrencyConversion.Convert(from, to, amount);

                string reply = string.Format("{0:n0} {1} is {2:n0} {3}", amount, from.ToTileString(), result.result, result.resultCurrency.ToTileString());

                List<string> remainders = new List<string>();

                for (int i = result.remainders.Length - 1; i >= 0; i--)
                {
                    if (result.remainders[i] != 0)
                    {
                        remainders.Add(string.Format("{0:n0} {1}", result.remainders[i], ((CoinType)i).ToTileString()));
                    }
                }

                if (remainders.Count > 0)
                {
                    for (int i = 0; i < remainders.Count - 1; i++)
                    {
                        reply += ", " + remainders[i];
                    }

                    reply += " and " + remainders[remainders.Count - 1];
                }                

                await ReplyAsync(reply);
            }
        }

        [Command("setrate"), Summary("Set the rate at which chimes convert to silver")]
        public async Task Playlists(double rate)
        {
            Program.options.ChimeRate = rate;

            using (StreamWriter file = new StreamWriter("resources/options.json"))
            {
                using (JsonTextWriter writer = new JsonTextWriter(file))
                {
                    file.Write(JsonConvert.SerializeObject(Program.options, Formatting.Indented));
                }
            }

            CreateConversionTable();

            await ReplyAsync(string.Format("A chime is now worth {0} silver", rate));
        }

        [Command("playlists"), Summary("Map related commands")]
        public async Task Playlists()
        {
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle("Playlists");

            foreach (var item in YouTube.Playlists.PlaylistList)
            {
                EmbedFieldBuilder field = new EmbedFieldBuilder();
                field.WithName(item.Key.ToTitleCase());
                field.Value = item.Value.URL;
                embed.AddField(field);
            }

            await ReplyAsync("", false, embed.Build());
        }

        [Command("dadroll"), Summary("Roll for Powered By Dad's Bad Gift"), Alias("p")]
        public async Task PBDBGRoll()
        {
            Rolls rolls = Rolls.Create();

            StringBuilder diceRolled = new StringBuilder();
            foreach (int i in rolls.DiceRolled.SkipLast(1))
            {
                diceRolled.Append(i + ",");
            }

            diceRolled.Append(rolls.DiceRolled.Last());

            String rollOutput = string.Format("{0}|+{1}|++{2}|-{3}|--{4}", rolls.NormalRoll, rolls.AdvantageRoll, rolls.GreatAdvantageRoll, rolls.DisadvantageRoll, rolls.GreatDisadvantageRoll);

            await ReplyAsync(Context.User.Mention + ", you rolled: " + diceRolled.ToString() + Environment.NewLine + rollOutput);
        }

        [Command("setname")]
        public async Task Test(string name)
        {
            var guild = Context.Guild;
            var user = guild.GetUser(Context.Client.CurrentUser.Id);
            await user.ModifyAsync(x => {
                x.Nickname = name;
            });
        }

        [Command("help"), Summary("List avaliable commands.")]
        public async Task Help()
        {
            EmbedBuilder embed = new EmbedBuilder();
            embed.Description = "These are the commands you can use";

            foreach (ModuleInfo module in service.Modules)
            {
                string description = string.Empty;
                foreach (CommandInfo command in module.Commands)
                {
                    PreconditionResult result = await command.CheckPreconditionsAsync(Context);
                    if (result.IsSuccess)
                        description += string.Format("!{0} (!{1}) - {2}" + Environment.NewLine, command.Aliases.First(), command.Aliases.Last(), command.Summary);
                }

                if (!string.IsNullOrWhiteSpace(description))
                {
                    embed.AddField(x =>
                    {
                        x.Name = module.Name;
                        x.Value = description;
                        x.IsInline = false;
                    });
                }
            }

            await ReplyAsync("", false, embed.Build());
        }
    }
}
