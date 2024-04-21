namespace DaveTheMonitor.Core.Commands
{
    internal struct CommandArgParser
    {
        private int _index;

        public CommandArgs Parse(CommandArgToken[] tokens, out bool error)
        {
            error = false;
            _index = 0;
            CommandArgToken? token = PeekToken(tokens);
            CommandArgs args = new CommandArgs();
            while (token.HasValue && !error)
            {
                token = PeekToken(tokens);
                if (!token.HasValue)
                {
                    break;
                }

                switch (token.Value.Type)
                {
                    case CommandArgTokenType.ShortOption:
                    case CommandArgTokenType.LongOption:
                    {
                        (string, string) value = ParseOption(tokens, out error);
                        args.AddArg(value.Item1, value.Item2);
                        break;
                    }
                    default: error = true; break;
                }
            }
            return args;
        }

        private CommandArgToken? NextToken(CommandArgToken[] tokens)
        {
            if (_index >= tokens.Length)
            {
                return null;
            }
            return tokens[_index++];
        }

        private CommandArgToken? PeekToken(CommandArgToken[] tokens)
        {
            if (_index >= tokens.Length)
            {
                return null;
            }
            return tokens[_index];
        }

        private (string, string) ParseOption(CommandArgToken[] tokens, out bool error)
        {
            error = false;
            string optionName = GetOptionName(NextToken(tokens).Value);
            CommandArgToken? token = PeekToken(tokens);
            if (!token.HasValue)
            {
                return (optionName, null);
            }

            if (token.Value.Type == CommandArgTokenType.Equals)
            {
                NextToken(tokens);
                token = PeekToken(tokens);

                if (!token.HasValue)
                {
                    error = true;
                    return (optionName, null);
                }
            }

            if (token.Value.Type != CommandArgTokenType.Argument)
            {
                error = true;
                return (optionName, null);
            }
            NextToken(tokens);
            return (optionName, GetArgumentValue(token.Value));
        }

        private string GetOptionName(CommandArgToken token)
        {
            return token.Lexeme.Substring(token.Type == CommandArgTokenType.LongOption ? 2 : 1);
        }

        private string GetArgumentValue(CommandArgToken token)
        {
            if (token.Lexeme.StartsWith('"'))
            {
                return token.Lexeme.Substring(1, token.Lexeme.Length - 2);
            }
            else
            {
                return token.Lexeme;
            }
        }
    }
}
