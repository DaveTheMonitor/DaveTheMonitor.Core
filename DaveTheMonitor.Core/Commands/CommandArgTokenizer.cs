using System.Collections.Generic;
using System.Text;

namespace DaveTheMonitor.Core.Commands
{
    internal struct CommandArgTokenizer
    {
        private int _index;

        public CommandArgToken[] Tokenize(string s, out bool error)
        {
            List<CommandArgToken> list = new List<CommandArgToken>();
            StringBuilder builder = new StringBuilder();
            _index = 0;
            error = false;
            while (_index < s.Length && !error)
            {
                char? c = PeekChar(s);
                if (!c.HasValue)
                {
                    break;
                }

                if (char.IsWhiteSpace(c.Value))
                {
                    _index++;
                    continue;
                }

                if (c == '-')
                {
                    list.Add(ParseOption(s, builder, out error));
                }
                else if (c == '=')
                {
                    list.Add(ParseEquals(s, out error));
                }
                else if (c == '"')
                {
                    list.Add(ParseStringArgument(s, builder, out error));
                }
                else
                {
                    list.Add(ParseWordArgument(s, builder, out error));
                }
            }

            return list.ToArray();
        }

        private char? NextChar(string s)
        {
            if (_index >= s.Length)
            {
                return null;
            }
            return s[_index++];
        }

        private char? PeekChar(string s)
        {
            if (_index >= s.Length)
            {
                return null;
            }
            return s[_index];
        }

        private CommandArgToken ParseOption(string s, StringBuilder builder, out bool error)
        {
            error = false;
            builder.Clear();
            char? c = PeekChar(s);
            while (c.HasValue && !char.IsWhiteSpace(c.Value) && c.Value != '=' && c.Value != '"')
            {
                builder.Append(c.Value);
                NextChar(s);
                c = PeekChar(s);
            }

            string str = builder.ToString();
            CommandArgToken token = new CommandArgToken(str, str.StartsWith("--") ? CommandArgTokenType.LongOption : CommandArgTokenType.ShortOption);
            return token;
        }

        private CommandArgToken ParseEquals(string s, out bool error)
        {
            error = false;
            CommandArgToken token = new CommandArgToken(NextChar(s).ToString(), CommandArgTokenType.Equals);
            return token;
        }

        private CommandArgToken ParseStringArgument(string s, StringBuilder builder, out bool error)
        {
            error = false;
            builder.Clear();
            char? c = PeekChar(s);
            if (!c.HasValue || c.Value != '"')
            {
                error = true;
                return new CommandArgToken(null, CommandArgTokenType.Argument);
            }
            builder.Append(c.Value);
            NextChar(s);
            c = PeekChar(s);

            while (c.HasValue && c.Value != '"')
            {
                builder.Append(c.Value);
                NextChar(s);
                c = PeekChar(s);
            }
            if (!c.HasValue)
            {
                error = true;
                return new CommandArgToken(builder.ToString(), CommandArgTokenType.Argument);
            }
            builder.Append(c.Value);
            NextChar(s);
            return new CommandArgToken(builder.ToString(), CommandArgTokenType.Argument);
        }

        private CommandArgToken ParseWordArgument(string s, StringBuilder builder, out bool error)
        {
            error = false;
            builder.Clear();
            char? c = PeekChar(s);
            while (c.HasValue && !char.IsWhiteSpace(c.Value))
            {
                builder.Append(c.Value);
                NextChar(s);
                c = PeekChar(s);
            }
            CommandArgToken token = new CommandArgToken(builder.ToString(), CommandArgTokenType.Argument);
            return token;
        }
    }
}
