using System;

namespace DaveTheMonitor.Core.Commands
{
    /// <summary>
    /// Marks a method as callable as a console command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class ConsoleCommandAttribute : Attribute
    {
        /// <summary>
        /// The full name of this command.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The short help for this command. Should only be one or two sentences.
        /// </summary>
        public string ShortHelp { get; set; }
        /// <summary>
        /// The full, descriptive help for this command.
        /// </summary>
        public string FullHelp { get; set; }
        /// <summary>
        /// The aliases of this command, if any.
        /// </summary>
        public string[] Aliases { get; set; }

        /// <summary>
        /// Creates a new <see cref="ConsoleCommandAttribute"/>.
        /// </summary>
        public ConsoleCommandAttribute()
        {
            Name = null;
            ShortHelp = null;
            FullHelp = null;
        }

        /// <summary>
        /// Creates a new <see cref="ConsoleCommandAttribute"/> with the specified info.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <param name="shortHelp">The short help for this command. Should only be one or two sentences.</param>
        /// <param name="fullHelp">The full, descriptive help for this command.</param>
        /// <param name="aliases">The aliases of this command, if any.</param>
        public ConsoleCommandAttribute(string name, string shortHelp, string fullHelp, params string[] aliases)
        {
            Name = name;
            ShortHelp = shortHelp;
            FullHelp = fullHelp;
            Aliases = aliases;
        }
    }
}
