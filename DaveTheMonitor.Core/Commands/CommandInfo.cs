using DaveTheMonitor.Core.API;
using StudioForge.Engine.Integration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DaveTheMonitor.Core.Commands
{
    /// <summary>
    /// Represents a command that can be run from the console.
    /// </summary>
    [DebuggerDisplay("{Name}")]
    public class CommandInfo
    {
        /// <summary>
        /// The full name of the command.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// The aliases of the command.
        /// </summary>
        /// <remarks>
        /// This may be null if the command has no aliases.
        /// </remarks>
        public string[] Aliases { get; private set; }
        /// <summary>
        /// The short help for the command.
        /// </summary>
        public string ShortHelp { get; private set; }
        /// <summary>
        /// The full, descriptive help for the command.
        /// </summary>
        public string FullHelp { get; private set; }
        /// <summary>
        /// The arguments for the command.
        /// </summary>
        public CommandArgInfo[] Args { get; private set; }
        private CommandInvoker _invoker;

        /// <summary>
        /// Creates a <see cref="CommandInfo"/> from a method with the <see cref="ConsoleCommandAttribute"/>.
        /// </summary>
        /// <param name="method">The method to create this <see cref="CommandInfo"/> from.</param>
        /// <returns>A new <see cref="CommandInfo"/> representing the method.</returns>
        public static CommandInfo FromMethod(MethodInfo method)
        {
            ConsoleCommandAttribute attribute = method.GetCustomAttribute<ConsoleCommandAttribute>();
            CommandInfo info = new CommandInfo();
            info.Name = attribute.Name ?? method.Name.ToLowerInvariant();
            info.ShortHelp = attribute.ShortHelp;
            info.FullHelp = attribute.FullHelp ?? info.ShortHelp;
            info.Aliases = attribute.Aliases;

            ParameterInfo[] @params = method.GetParameters();
            if (@params.Length < 2 || @params[0].ParameterType != typeof(ICorePlayer) || @params[1].ParameterType != typeof(IOutputLog))
            {
                throw new Exception("ConsoleCommand must have ICorePlayer and IOutputLog parameters");
            }

            IEnumerable<ConsoleCommandArgAttribute> args = method.GetCustomAttributes<ConsoleCommandArgAttribute>();
            List<CommandArgInfo> list = new List<CommandArgInfo>();
            foreach (ConsoleCommandArgAttribute arg in args)
            {
                ParameterInfo param = @params.First(p => p.Name == arg.Param);
                if (param == null)
                {
                    throw new Exception("ConsoleCommandArgAttribute.Param must match a method parameter");
                }

                if (!IsValidParameter(param))
                {
                    throw new Exception($"ConsoleCommand parameter {param.Name} is not valid.");
                }

                if (!arg.Required)
                {
                    Type underlying = Nullable.GetUnderlyingType(param.ParameterType);
                    if (underlying == null)
                    {
                        if (param.ParameterType.IsValueType)
                        {
                            throw new Exception("ConsoleCommand non-required parameter must be nullable.");
                        }
                    }
                }

                list.Add(CommandArgInfo.FromAttribute(arg));
            }
            info.Args = list.ToArray();

            info._invoker = CommandInvoker.FromMethod(method, info);
            return info;
        }

        private static bool IsValidParameter(ParameterInfo info)
        {
            return true;
        }

        /// <summary>
        /// Gets a full help string for this command, including all arguments.
        /// </summary>
        /// <returns>A full help string for this command.</returns>
        public string GetFullHelpString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(FullHelp);
            builder.AppendLine();

            if (Args?.Length > 0)
            {
                if (Args.Length > 8)
                {
                    builder.Append($"Usage: {Name} [OPTIONS]");
                }
                else
                {
                    builder.Append($"Usage: {Name}");
                    foreach (CommandArgInfo arg in Args)
                    {
                        builder.Append(arg.Aliases?.Length > 0 ? $" -{arg.Aliases[0]}" : $" --{arg.Name}");
                    }
                }
                builder.AppendLine();
                builder.AppendLine();
            }
            else
            {
                builder.AppendLine($"Usage: {Name}");
                builder.AppendLine();
            }

            if (Aliases?.Length > 0)
            {
                builder.Append("Aliases:");
                foreach (string alias in Aliases)
                {
                    builder.Append($" {alias}");
                }
                builder.AppendLine();
                builder.AppendLine();
            }

            if (Args?.Length > 0)
            {
                builder.AppendLine("Options");
                foreach (CommandArgInfo arg in Args)
                {
                    if (arg.Aliases?.Length > 0)
                    {
                        foreach (string alias in arg.Aliases)
                        {
                            builder.Append($"-{alias}, ");
                        }
                    }
                    builder.Append($"--{arg.Name}");

                    if (arg.Required)
                    {
                        builder.Append(" (Required)");
                    }
                    builder.AppendLine();
                    builder.AppendLine(arg.Help);
                    builder.AppendLine();
                }
            }

            return builder.ToString().Trim();
        }

        /// <summary>
        /// Gets the argument with the specified name for this command.
        /// </summary>
        /// <param name="name">The name or alias of the argument.</param>
        /// <returns>An <see cref="CommandArgInfo"/> for the argument if it exists, otherwise null.</returns>
        public CommandArgInfo GetArgument(string name)
        {
            if (Args?.Length > 0)
            {
                foreach (CommandArgInfo arg in Args)
                {
                    if (arg.Name == name || arg.Aliases?.Contains(name) == true)
                    {
                        return arg;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Invokes this command with the specified arguments.
        /// </summary>
        /// <param name="player">The player executing this command.</param>
        /// <param name="log">The log to write results to.</param>
        /// <param name="args">The arguments passed to the command.</param>
        public void Invoke(ICorePlayer player, IOutputLog log, CommandArgs args)
        {
            _invoker.Invoke(player, log, args);
        }
    }
}
