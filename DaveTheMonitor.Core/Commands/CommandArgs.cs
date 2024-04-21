using DaveTheMonitor.Core.Helpers;
using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine.Integration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace DaveTheMonitor.Core.Commands

{
    /// <summary>
    /// A list of arguments for a <see cref="CommandInvoker"/>.
    /// </summary>
    [DebuggerDisplay("Count = {Count}")]
    public sealed class CommandArgs : IEnumerable<KeyValuePair<string, string>>
    {
        /// <summary>
        /// The number of arguments.
        /// </summary>
        public int Count => _args.Count;
        private Dictionary<string, string> _args;

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, string>>)_args).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_args).GetEnumerator();
        }

        /// <summary>
        /// Returns true if the argument with the specified name exists.
        /// </summary>
        /// <param name="name">The name of the argument.</param>
        /// <returns>True if the specified argument exists.</returns>
        /// <remarks>
        /// <paramref name="name"/> must match the name (full or alias) provided in the string this <see cref="CommandArgs"/> was created from.
        /// </remarks>
        public bool HasArg(string name)
        {
            return _args.ContainsKey(name);
        }

        /// <summary>
        /// Returns true if the specified argument exists.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <returns>True if the argument exists.</returns>
        public bool HasArg(CommandArgInfo arg)
        {
            if (HasArg(arg.Name))
            {
                return true;
            }
            if (arg.Aliases?.Length > 0)
            {
                foreach (string alias in arg.Aliases)
                {
                    if (HasArg(alias))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the argument with the specified name, writing to <paramref name="log"/> if it does not exist.
        /// </summary>
        /// <param name="name">The name of the argument.</param>
        /// <param name="log">The log to write to if the argument doesn't exist.</param>
        /// <returns>The argument if it exists, otherwise null.</returns>
        /// <remarks>
        /// <paramref name="name"/> must match the name (full or alias) provided in the string this <see cref="CommandArgs"/> was created from.
        /// </remarks>
        public string GetArg(string name, IOutputLog log)
        {
            if (!_args.TryGetValue(name, out string result))
            {
                log?.WriteLine($"Arg not found: {name}");
            }
            return result;
        }

        /// <summary>
        /// Gets the argument with the specified name.
        /// </summary>
        /// <param name="name">The name of the argument.</param>
        /// <param name="result">The argument if it exists, otherwise null.</param>
        /// <returns>True if the argument exists, otherwise false.</returns>
        /// <remarks>
        /// <paramref name="name"/> must match the name (full or alias) provided in the string this <see cref="CommandArgs"/> was created from.
        /// </remarks>
        public bool TryGetArg(string name, out string result)
        {
            return _args.TryGetValue(name, out result);
        }

        /// <summary>
        /// Gets the argument with the specified name and parses it using the <see cref="IParsable{TSelf}.TryParse(string?, IFormatProvider?, out TSelf)"/> implementation of <typeparamref name="T"/>.
        /// </summary>
        /// <param name="name">The name of the argument.</param>
        /// <param name="result">The argument if it exists and could be parsed, otherwise default(T).</param>
        /// <param name="log">The log to write to if the argument fails to parse.</param>
        /// <param name="parseError">True if the argument exists, but could not be parsed.</param>
        /// <returns>True if the argument exists, otherwise false.</returns>
        /// <remarks>
        /// <paramref name="name"/> must match the name (full or alias) provided in the string this <see cref="CommandArgs"/> was created from.
        /// </remarks>
        public bool TryGetArg<T>(string name, out T result, IOutputLog log, out bool parseError) where T : IParsable<T>
        {
            parseError = false;
            if (_args.TryGetValue(name, out string value))
            {
                if (T.TryParse(value, null, out result))
                {
                    return true;
                }
                else
                {
                    parseError = true;
                    log?.WriteLine($"Arg {name}: expected {typeof(T).Name}, received {value}");
                }
            }
            result = default(T);
            return false;
        }

        /// <summary>
        /// Gets the argument with the specified name and parses it as a <see cref="Vector2"/>.
        /// </summary>
        /// <param name="name">The name of the argument.</param>
        /// <param name="result">The argument if it exists and could be parsed, otherwise default(Vector2).</param>
        /// <param name="log">The log to write to if the argument fails to parse.</param>
        /// <param name="parseError">True if the argument exists, but could not be parsed.</param>
        /// <returns>True if the argument exists, otherwise false.</returns>
        /// <remarks>
        /// <paramref name="name"/> must match the name (full or alias) provided in the string this <see cref="CommandArgs"/> was created from.
        /// </remarks>
        public bool TryGetArgVector2(string name, out Vector2 result, IOutputLog log, out bool parseError)
        {
            parseError = false;
            if (_args.TryGetValue(name, out string value))
            {
                if (DeserializationHelper.TryParseVector2(value, out result))
                {
                    return true;
                }
                else
                {
                    parseError = true;
                    log?.WriteLine($"Arg {name}: expected Vector2, received {value}");
                }
            }
            result = default(Vector2);
            return false;
        }

        /// <summary>
        /// Gets the argument with the specified name and parses it as a <see cref="Vector3"/>.
        /// </summary>
        /// <param name="name">The name of the argument.</param>
        /// <param name="result">The argument if it exists and could be parsed, otherwise default(Vector3).</param>
        /// <param name="log">The log to write to if the argument fails to parse.</param>
        /// <param name="parseError">True if the argument exists, but could not be parsed.</param>
        /// <returns>True if the argument exists, otherwise false.</returns>
        /// <remarks>
        /// <paramref name="name"/> must match the name (full or alias) provided in the string this <see cref="CommandArgs"/> was created from.
        /// </remarks>
        public bool TryGetArgVector3(string name, out Vector3 result, IOutputLog log, out bool parseError)
        {
            parseError = false;
            if (_args.TryGetValue(name, out string value))
            {
                if (DeserializationHelper.TryParseVector3(value, out result))
                {
                    return true;
                }
                else
                {
                    parseError = true;
                    log?.WriteLine($"Arg {name}: expected Vector3, received {value}");
                }
            }
            result = default(Vector3);
            return false;
        }

        /// <summary>
        /// Gets the argument with the specified name and parses it as a <see cref="Vector4"/>.
        /// </summary>
        /// <param name="name">The name of the argument.</param>
        /// <param name="result">The argument if it exists and could be parsed, otherwise default(Vector4).</param>
        /// <param name="log">The log to write to if the argument fails to parse.</param>
        /// <param name="parseError">True if the argument exists, but could not be parsed.</param>
        /// <returns>True if the argument exists, otherwise false.</returns>
        /// <remarks>
        /// <paramref name="name"/> must match the name (full or alias) provided in the string this <see cref="CommandArgs"/> was created from.
        /// </remarks>
        public bool TryGetArgVector4(string name, out Vector4 result, IOutputLog log, out bool parseError)
        {
            parseError = false;
            if (_args.TryGetValue(name, out string value))
            {
                if (DeserializationHelper.TryParseVector4(value, out result))
                {
                    return true;
                }
                else
                {
                    parseError = true;
                    log?.WriteLine($"Arg {name}: expected Vector4, received {value}");
                }
            }
            result = default(Vector4);
            return false;
        }

        /// <summary>
        /// Gets the argument with the specified name and parses it as a <see cref="Point"/>.
        /// </summary>
        /// <param name="name">The name of the argument.</param>
        /// <param name="result">The argument if it exists and could be parsed, otherwise default(Point).</param>
        /// <param name="log">The log to write to if the argument fails to parse.</param>
        /// <param name="parseError">True if the argument exists, but could not be parsed.</param>
        /// <returns>True if the argument exists, otherwise false.</returns>
        /// <remarks>
        /// <paramref name="name"/> must match the name (full or alias) provided in the string this <see cref="CommandArgs"/> was created from.
        /// </remarks>
        public bool TryGetArgPoint(string name, out Point result, IOutputLog log, out bool parseError)
        {
            parseError = false;
            if (_args.TryGetValue(name, out string value))
            {
                if (DeserializationHelper.TryParsePoint(value, out result))
                {
                    return true;
                }
                else
                {
                    parseError = true;
                    log?.WriteLine($"Arg {name}: expected Point, received {value}");
                }
            }
            result = default(Point);
            return false;
        }

        /// <summary>
        /// Gets the argument with the specified name and parses it as a <see cref="Point3D"/>.
        /// </summary>
        /// <param name="name">The name of the argument.</param>
        /// <param name="result">The argument if it exists and could be parsed, otherwise default(Point3D).</param>
        /// <param name="log">The log to write to if the argument fails to parse.</param>
        /// <param name="parseError">True if the argument exists, but could not be parsed.</param>
        /// <returns>True if the argument exists, otherwise false.</returns>
        /// <remarks>
        /// <paramref name="name"/> must match the name (full or alias) provided in the string this <see cref="CommandArgs"/> was created from.
        /// </remarks>
        public bool TryGetArgPoint3D(string name, out Point3D result, IOutputLog log, out bool parseError)
        {
            parseError = false;
            if (_args.TryGetValue(name, out string value))
            {
                if (DeserializationHelper.TryParsePoint3D(value, out result))
                {
                    return true;
                }
                else
                {
                    parseError = true;
                    log?.WriteLine($"Arg {name}: expected Point3D, received {value}");
                }
            }
            result = default(Point3D);
            return false;
        }

        /// <summary>
        /// Gets the argument with the specified name and parses it as a <see cref="GlobalPoint3D"/>.
        /// </summary>
        /// <param name="name">The name of the argument.</param>
        /// <param name="result">The argument if it exists and could be parsed, otherwise default(GlobalPoint3D).</param>
        /// <param name="log">The log to write to if the argument fails to parse.</param>
        /// <param name="parseError">True if the argument exists, but could not be parsed.</param>
        /// <returns>True if the argument exists, otherwise false.</returns>
        /// <remarks>
        /// <paramref name="name"/> must match the name (full or alias) provided in the string this <see cref="CommandArgs"/> was created from.
        /// </remarks>
        public bool TryGetArgGlobalPoint3D(string name, out GlobalPoint3D result, IOutputLog log, out bool parseError)
        {
            parseError = false;
            if (_args.TryGetValue(name, out string value))
            {
                if (DeserializationHelper.TryParseGlobalPoint3D(value, out result))
                {
                    return true;
                }
                else
                {
                    parseError = true;
                    log?.WriteLine($"Arg {name}: expected GlobalPoint3D, received {value}");
                }
            }
            result = default(GlobalPoint3D);
            return false;
        }

        /// <summary>
        /// Adds an argument with the specified name and value to the list.
        /// </summary>
        /// <param name="name">The name of the argument.</param>
        /// <param name="value">The value of the argument.</param>
        public void AddArg(string name, string value)
        {
            _args.Add(name, value);
        }

        /// <summary>
        /// Parses a string as a <see cref="CommandArgs"/>.
        /// </summary>
        /// <param name="s">The string to parse.</param>
        /// <param name="error">True if there was an error parsing, otherwise false.</param>
        /// <returns>The <see cref="CommandArgs"/> representing <paramref name="s"/></returns>
        public static CommandArgs FromString(string s, out bool error)
        {
            if (string.IsNullOrEmpty(s))
            {
                error = false;
                return null;
            }

            CommandArgTokenizer tokenizer = new CommandArgTokenizer();
            CommandArgToken[] tokens = tokenizer.Tokenize(s, out error);
            if (error)
            {
                return null;
            }
            
            CommandArgParser parser = new CommandArgParser();
            CommandArgs args = parser.Parse(tokens, out error);
            if (error)
            {
                return null;
            }
            return args;
        }

        /// <summary>
        /// Creates a new empty instance of <see cref="CommandArgs"/>.
        /// </summary>
        public CommandArgs()
        {
            _args = new Dictionary<string, string>();
        }
    }
}
