using System;

namespace DaveTheMonitor.Core.Commands
{
    /// <summary>
    /// Marks a method parameter as a command argument.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class ConsoleCommandArgAttribute : Attribute
    {
        /// <summary>
        /// The name of the method parameter.
        /// </summary>
        public string Param { get; set; }
        /// <summary>
        /// The name of the argument when running the command.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The full help for this argument.
        /// </summary>
        public string Help { get; set; }
        /// <summary>
        /// True if this argument is required, otherwise false.
        /// </summary>
        public bool Required { get; set; }
        /// <summary>
        /// The aliases of this argument, if any.
        /// </summary>
        public string[] Aliases { get; set; }

        /// <summary>
        /// Creates a new <see cref="ConsoleCommandArgAttribute"/>.
        /// </summary>
        public ConsoleCommandArgAttribute()
        {
            Param = null;
            Name = null;
            Help = null;
            Required = false;
            Aliases = null;
        }

        /// <summary>
        /// Creates a new <see cref="ConsoleCommandArgAttribute"/> with the specified info.
        /// </summary>
        /// <param name="param">The name of the method parameter.</param>
        /// <param name="name">The name of the argument when running the command.</param>
        /// <param name="help">The full help for this argument.</param>
        /// <param name="required">True if the argument is required, otherwise false. Optional value type arguments should be <see cref="Nullable{T}"/>.</param>
        /// <param name="aliases">The aliases of this argument, if any.</param>
        public ConsoleCommandArgAttribute(string param, string name, string help, bool required, params string[] aliases)
        {
            Param = param;
            Name = name;
            Help = help;
            Required = required;
            Aliases = aliases;
        }
    }
}
