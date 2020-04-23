using System;
using System.Collections.Generic;
using System.Text;

namespace HaranuBot.Quoting
{
    public class Quote
    {
        public string Character { get; set; }
        public string Text { get; set; }

        public Quote(string character, string text)
        {
            Character = character;
            Text = text;
        }

        public string GetFormattedQuote()
        {
            Uri uriResult;
            bool isURL = Uri.TryCreate(Text, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            if (isURL)
            {
                return string.Format("{0} - {1}", Text, Character.ToTitleCase());
            }
            else
            {
                return string.Format("\"{0}\" - {1}", Text, Character.ToTitleCase());
            }
        }

        public override bool Equals(object obj)
        {
            var quote = obj as Quote;
            return quote != null &&
                   Character == quote.Character &&
                   Text == quote.Text;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Character, Text);
        }
    }
}
