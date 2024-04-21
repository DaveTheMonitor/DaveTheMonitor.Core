using System.Diagnostics;

namespace DaveTheMonitor.Core.Commands
{
    [DebuggerDisplay("{Lexeme}, Type = {Type}")]
    internal struct CommandArgToken
    {
        public CommandArgTokenType Type { get; private set; }
        public string Lexeme { get; private set; }

        public CommandArgToken(string lexeme, CommandArgTokenType type)
        {
            Lexeme = lexeme;
            Type = type;
        }
    }
}
