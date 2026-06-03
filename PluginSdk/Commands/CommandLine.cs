using System.Collections.Generic;
using System.Text;

namespace PluginSdk.Commands
{
    /// <summary>
    /// Splits a command line into words, honouring double-quoted groups so a
    /// single argument may contain spaces (e.g. <c>say "hello there"</c> yields
    /// <c>say</c> and <c>hello there</c>). A backslash escapes the next
    /// character inside or outside quotes.
    /// </summary>
    internal static class CommandLine
    {
        public static List<string> Tokenize(string input)
        {
            var tokens = new List<string>();
            if (string.IsNullOrEmpty(input))
                return tokens;

            var sb = new StringBuilder();
            bool inToken = false;
            bool inQuotes = false;
            bool escape = false;

            foreach (char c in input)
            {
                if (escape)
                {
                    sb.Append(c);
                    inToken = true;
                    escape = false;
                    continue;
                }

                if (c == '\\')
                {
                    escape = true;
                    continue;
                }

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                    inToken = true;
                    continue;
                }

                if (!inQuotes && (c == ' ' || c == '\t'))
                {
                    if (inToken)
                    {
                        tokens.Add(sb.ToString());
                        sb.Clear();
                        inToken = false;
                    }
                    continue;
                }

                sb.Append(c);
                inToken = true;
            }

            if (inToken)
                tokens.Add(sb.ToString());

            return tokens;
        }
    }
}
