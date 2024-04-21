using System.Diagnostics;

namespace DaveTheMonitor.Core.Commands
{
    /// <summary>
    /// Represents an argument for a <see cref="CommandInfo"/>.
    /// </summary>
    [DebuggerDisplay("{Name}, Required = {Required}")]
    public class CommandArgInfo
    {
        /// <summary>
        /// The full name of the argument.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// The aliases of the argument.
        /// </summary>
        /// <remarks>
        /// This may be null if the argument has no aliases.
        /// </remarks>
        public string[] Aliases { get; private set; }
        /// <summary>
        /// The full help string for the argument.
        /// </summary>
        public string Help { get; private set; }
        /// <summary>
        /// True if this argument is required when executing the command.
        /// </summary>
        public bool Required { get; private set; }

        /// <summary>
        /// Creates a new <see cref="CommandArgInfo"/> from a <see cref="ConsoleCommandArgAttribute"/>.
        /// </summary>
        /// <param name="attribute">The attribute for this argument.</param>
        /// <returns>A new CommandArgInfo.</returns>
        public static CommandArgInfo FromAttribute(ConsoleCommandArgAttribute attribute)
        {
            CommandArgInfo info = new CommandArgInfo();
            info.Name = attribute.Name;
            info.Aliases = attribute.Aliases;
            info.Help = attribute.Help;
            info.Required = attribute.Required;
            return info;
        }
    }
}
