using System;
using System.Collections.Generic;
using System.Linq;
using Cli.Code.Runtime.Model;

namespace Cli.Code.Runtime.Controller
{
    public class TextService
    {
        private Dictionary<string, CommandInfo> _commands;

        public TextService(Dictionary<string, CommandInfo> commands)
        {
            _commands = commands;
        }

        public List<string> Suggest(string text)
        {
            var suggestions = new List<string>();
            if (string.IsNullOrEmpty(text))
            {
                return suggestions;
            }

            var tokens = GetTokens(text);
            var token = GetLastTokenInText(text);

            var command = tokens.Length > 0 ? tokens[0] : string.Empty;
            var argument = tokens.Length > 1 ? tokens[tokens.Length - 1] : string.Empty;

            if (string.IsNullOrEmpty(command))
            {
                return suggestions;
            }


            IEnumerable<string> names = null;
            if (!string.IsNullOrEmpty(argument))
            {
                var c = _commands
                    .FirstOrDefault(t => t.Key.Equals(command, StringComparison.OrdinalIgnoreCase))
                    .Value;

                if (c.Name != null && c.Args != null)
                {
                    names = c.Args;
                }
            }
            else
            {
                names = _commands
                    .Select(t => t.Key.ToLower())
                    .Where(t => t.StartsWith(command));
            }

            if (names != null)
            {
                suggestions = names
                    .OrderBy(t => LevenshteinDistance.Compute(token, t))
                    .ToList();
            }


            return suggestions;
        }

        public static string GetLastTokenInText(string text)
        {
            var tokens = GetTokens(text);
            if (tokens.Length <= 0)
            {
                return string.Empty;
            }


            var t = tokens[tokens.Length - 1];
            return t;
        }

        public static string[] GetTokens(string text)
        {
            var tokens = text.Split(' ');
            for (int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i].Equals(string.Empty))
                {
                    tokens[i] = " ";
                }
            }

            return tokens;
        }
    }
}