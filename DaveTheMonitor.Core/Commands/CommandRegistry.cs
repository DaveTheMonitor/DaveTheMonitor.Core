using DaveTheMonitor.Core.API;
using StudioForge.Engine.Integration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DaveTheMonitor.Core.Commands
{
    /// <summary>
    /// A registry containing <see cref="CommandInfo"/>.
    /// </summary>
    public sealed class CommandRegistry : IEnumerable<CommandInfo>
    {
        private Dictionary<string, CommandInfo> _dictionary;

        /// <summary>
        /// Registers all commands in the specified assembly by looking for methods with the <see cref="ConsoleCommandAttribute"/>.
        /// </summary>
        /// <param name="assembly">The assembly to search for commands.</param>
        public void RegisterCommands(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                foreach (MethodInfo method in type.GetMethods())
                {
                    if (method.GetCustomAttribute<ConsoleCommandAttribute>() != null)
                    {
                        RegisterCommand(method);
                    }
                }
            }
        }

        /// <summary>
        /// Registers a specific command from a method with the <see cref="ConsoleCommandAttribute"/>.
        /// </summary>
        /// <param name="method">The method to register as a command.</param>
        public void RegisterCommand(MethodInfo method)
        {
            ConsoleCommandAttribute attribute = method.GetCustomAttribute<ConsoleCommandAttribute>();
            if (attribute == null)
            {
                throw new InvalidOperationException("Command method must have ConsoleCommandAttribute");
            }
            else if (!method.IsStatic)
            {
                throw new InvalidOperationException("Command method must be static");
            }

            CommandInfo info = CommandInfo.FromMethod(method);
            _dictionary.Add(attribute.Name, info);
        }

        /// <summary>
        /// Runs the command with the specified arguments.
        /// </summary>
        /// <param name="name">The name or alias of the command to run.</param>
        /// <param name="player">The player executing the command.</param>
        /// <param name="log">The log to write results to.</param>
        /// <param name="args">The arguments of the command.</param>
        /// <returns>True if the command executed successfully, otherwise false.</returns>
        public bool RunCommand(string name, ICorePlayer player, IOutputLog log, string args)
        {
            bool error = false;
            CommandArgs args2 = args != null ? CommandArgs.FromString(args, out error) : null;
            if (error)
            {
                log?.WriteLine("There was an error parsing arguments.");
                return false;
            }
            return RunCommand(name, player, log, args2);
        }

        /// <summary>
        /// Runs the command with the specified arguments.
        /// </summary>
        /// <param name="name">The name or alias of the command to run.</param>
        /// <param name="player">The player executing the command.</param>
        /// <param name="log">The log to write results to.</param>
        /// <param name="args">The arguments of the command.</param>
        /// <returns>True if the command executed successfully, otherwise false.</returns>
        public bool RunCommand(string name, ICorePlayer player, IOutputLog log, CommandArgs args)
        {
            if (_dictionary.TryGetValue(name, out CommandInfo info))
            {
                info.Invoke(player, log, args);
                return true;
            }
            foreach (KeyValuePair<string, CommandInfo> pair in _dictionary)
            {
                if (pair.Value.Aliases.Contains(name))
                {
                    pair.Value.Invoke(player, log, args);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns true if a command with the specified name exists.
        /// </summary>
        /// <param name="name">The name or alias of the command.</param>
        /// <returns>True if the specified command exists.</returns>
        public bool CommandExists(string name)
        {
            return GetCommand(name) != null;
        }

        /// <summary>
        /// Returns the <see cref="CommandInfo"/> with the specified name if it exists, otherwise null.
        /// </summary>
        /// <param name="name">The name or alias of the command.</param>
        /// <returns>The command if it exists, otherwise null.</returns>
        public CommandInfo GetCommand(string name)
        {
            if (_dictionary.TryGetValue(name, out CommandInfo info))
            {
                return info;
            }
            foreach (KeyValuePair<string, CommandInfo> pair in _dictionary)
            {
                if (pair.Value.Aliases?.Contains(name) == true)
                {
                    return pair.Value;
                }
            }
            return null;
        }

        IEnumerator<CommandInfo> IEnumerable<CommandInfo>.GetEnumerator()
        {
            return ((IEnumerable<CommandInfo>)_dictionary.Values).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_dictionary.Values).GetEnumerator();
        }

        /// <summary>
        /// Creates a new empty <see cref="CommandRegistry"/>.
        /// </summary>
        public CommandRegistry()
        {
            _dictionary = new Dictionary<string, CommandInfo>();
        }
    }
}
