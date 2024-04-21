using DaveTheMonitor.Core.API;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DaveTheMonitor.Core.Commands
{
    internal class CommandInvoker
    {
        private Dictionary<string, int> _argIndex;
        private FastInvokeHandler _handler;
        private object[] _argsCache;
        private CommandInfo _command;
        private Type[] _paramTypes;

        public static CommandInvoker FromMethod(MethodInfo method, CommandInfo command)
        {
            CommandInvoker invoker = new CommandInvoker(method, command);
            return invoker;
        }

        public void Invoke(ICorePlayer player, IOutputLog log, CommandArgs args)
        {
            for (int i = 0; i < _argsCache.Length; i++)
            {
                _argsCache[i] = null;
            }

            _argsCache[0] = player;
            _argsCache[1] = log;
            if (args != null)
            {
                foreach (KeyValuePair<string, string> pair in args)
                {
                    CommandArgInfo arg = _command.GetArgument(pair.Key);
                    if (arg == null)
                    {
                        continue;
                    }

                    if (_argIndex.TryGetValue(arg.Name, out int index))
                    {
                        _argsCache[index] = GetArg(args, pair.Key, _paramTypes[index], log, out bool parseError);
                        if (parseError)
                        {
                            return;
                        }
                    }
                }
            }

            foreach (CommandArgInfo arg in _command.Args)
            {
                if (arg.Required && (args == null || !args.HasArg(arg)))
                {
                    log?.WriteLine($"Missing required argument: {arg.Name}");
                    return;
                }
            }

            _handler(null, _argsCache);
        }

        private object GetArg(CommandArgs args, string name, Type type, IOutputLog log, out bool parseError)
        {
            parseError = false;
            if (type == typeof(string))
            {
                if (args.TryGetArg(name, out var r))
                {
                    if (r == null)
                    {
                        parseError = true;
                    }
                    return r;
                }
                return null;
            }
            else if (type == typeof(bool))
            {
                return args.HasArg(name);
            }
            else if (type == typeof(byte))
            {
                return args.TryGetArg<byte>(name, out var r, log, out parseError) ? r : null;
            }
            else if (type == typeof(sbyte))
            {
                return args.TryGetArg<sbyte>(name, out var r, log, out parseError) ? r : null;
            }
            else if (type == typeof(short))
            {
                return args.TryGetArg<short>(name, out var r, log, out parseError) ? r : null;
            }
            else if (type == typeof(ushort))
            {
                return args.TryGetArg<ushort>(name, out var r, log, out parseError) ? r : null;
            }
            else if (type == typeof(int))
            {
                return args.TryGetArg<int>(name, out var r, log, out parseError) ? r : null;
            }
            else if (type == typeof(uint))
            {
                return args.TryGetArg<uint>(name, out var r, log, out parseError) ? r : null;
            }
            else if (type == typeof(long))
            {
                return args.TryGetArg<long>(name, out var r, log, out parseError) ? r : null;
            }
            else if (type == typeof(ulong))
            {
                return args.TryGetArg<ulong>(name, out var r, log, out parseError) ? r : null;
            }
            else if (type == typeof(float))
            {
                return args.TryGetArg<float>(name, out var r, log, out parseError) ? r : null;
            }
            else if (type == typeof(double))
            {
                return args.TryGetArg<double>(name, out var r, log, out parseError) ? r : null;
            }
            else if (type == typeof(decimal))
            {
                return args.TryGetArg<decimal>(name, out var r, log, out parseError) ? r : null;
            }
            else if (type == typeof(Half))
            {
                return args.TryGetArg<Half>(name, out var r, log, out parseError) ? r : null;
            }
            else if (type == typeof(char))
            {
                return args.TryGetArg<char>(name, out var r, log, out parseError) ? r : null;
            }
            else if (type == typeof(System.Numerics.BigInteger))
            {
                return args.TryGetArg<System.Numerics.BigInteger>(name, out var r, log, out parseError) ? r : null;
            }
            else if (type == typeof(Vector2))
            {
                return args.TryGetArgVector2(name, out var r, log, out parseError) ? r : null;
            }
            else if (type == typeof(Vector3))
            {
                return args.TryGetArgVector3(name, out var r, log, out parseError) ? r : null;
            }
            else if (type == typeof(Vector4))
            {
                return args.TryGetArgVector4(name, out var r, log, out parseError) ? r : null;
            }
            else if (type == typeof(Point))
            {
                return args.TryGetArgPoint(name, out var r, log, out parseError) ? r : null;
            }
            else if (type == typeof(Point3D))
            {
                return args.TryGetArgPoint3D(name, out var r, log, out parseError) ? r : null;
            }
            else if (type == typeof(GlobalPoint3D))
            {
                return args.TryGetArgGlobalPoint3D(name, out var r, log, out parseError) ? r : null;
            }
            else
            {
                throw new Exception("Invalid command arg type");
            }
        }

        private CommandInvoker(MethodInfo method, CommandInfo command)
        {
            ParameterInfo[] @params = method.GetParameters();

            _argIndex = new Dictionary<string, int>();
            _handler = MethodInvoker.GetHandler(method);
            _argsCache = new object[@params.Length];
            _command = command;

            if (_command.Args?.Length > 0)
            {
                IEnumerable<ConsoleCommandArgAttribute> attributes = method.GetCustomAttributes<ConsoleCommandArgAttribute>();

                foreach (CommandArgInfo arg in _command.Args)
                {
                    ConsoleCommandArgAttribute attribute = attributes.First(a => a.Name == arg.Name);
                    if (attribute == null)
                    {
                        throw new Exception($"Attribute for {arg.Name} cannot be found.");
                    }

                    int index = Array.FindIndex(@params, p => p.Name == attribute.Param);
                    if (index == -1)
                    {
                        throw new Exception($"Param {attribute.Param} cannot be found.");
                    }

                    _argIndex.Add(arg.Name, index);
                }
            }

            _paramTypes = new Type[@params.Length];
            for (int i = 0; i < @params.Length; i++)
            {
                Type paramType = @params[i].ParameterType;
                Type underlying = Nullable.GetUnderlyingType(paramType);
                _paramTypes[i] = underlying ?? @params[i].ParameterType;
            }
        }
    }
}
