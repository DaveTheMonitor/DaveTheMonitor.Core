using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Plugin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace DaveTheMonitor.Core
{
    /// <summary>
    /// A collection of <see cref="ICoreData{T}"/>
    /// </summary>
    /// <typeparam name="TData">The type of data contained in the collection.</typeparam>
    public sealed class CoreDataCollection<TData> : IHasCoreData<TData>, IEnumerable<ICoreData<TData>>
    {
        private List<ICoreData<TData>> _list;
        private Dictionary<Type, ICoreData<TData>> _dict;
        private TData _item;

        private static int CompareData(ICoreData<TData> left, ICoreData<TData> right)
        {
            return left.Priority.CompareTo(right.Priority);
        }

        /// <summary>
        /// Gets the data from the specified mod as <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of data to get.</typeparam>
        /// <returns>The data of type <typeparamref name="T"/>, or default(T) if no data is found.</returns>
        public T GetData<T>() where T : ICoreData<TData>
        {
            if (_dict.TryGetValue(typeof(T), out ICoreData<TData> data))
            {
                return (T)data;
            }
            return default(T);
        }

        /// <summary>
        /// Sets <paramref name="result"/> to the data of type <typeparamref name="T"/>, or default if it doesn't exist.
        /// </summary>
        /// <typeparam name="T">The type of data to get.</typeparam>
        /// <returns>True if the data is exists, otherwise false.</returns>
        public bool TryGetData<T>(out T result) where T : ICoreData<TData>
        {
            if (_dict.TryGetValue(typeof(T), out ICoreData<TData> data))
            {
                result = (T)data;
                return true;
            }
            
            result = default(T);
            return false;
        }

        /// <summary>
        /// Clears and fills <paramref name="result"/> with the data for this item.
        /// </summary>
        /// <param name="result">The list to fill.</param>
        public void GetAllData(List<ICoreData<TData>> result)
        {
            result.Clear();
            result.AddRange(_list);
        }

        /// <summary>
        /// Returns true if this item has data of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the data.</typeparam>
        /// <returns>True if the data exists, otherwise false.</returns>
        public bool HasData<T>()
        {
            return _dict.ContainsKey(typeof(T));
        }

        /// <summary>
        /// Sets the data of type of <paramref name="data"/> to the value. It's best practice to only call this for your own mod.
        /// </summary>
        /// <param name="data">The data to set.</param>
        public void SetData(ICoreData<TData> data)
        {
            if (_dict.TryGetValue(data.GetType(), out ICoreData<TData> existing))
            {
                int index = _list.IndexOf(existing);
                _list[index] = data;
                _dict[data.GetType()] = data;
                data.Initialize(_item);
                return;
            }

            SetAndInitialize(data.GetType(), data);
        }

        /// <summary>
        /// Sets the data of type <typeparamref name="T"/> to the value. It's best practice to only call this for your own data.
        /// </summary>
        /// <param name="data">The data to set.</param>
        public void SetData<T>(T data) where T : ICoreData<TData>
        {
            if (_dict.TryGetValue(typeof(T), out ICoreData<TData> existing))
            {
                int index = _list.IndexOf(existing);
                _list[index] = data;
                _dict[typeof(T)] = data;
                data.Initialize(_item);
                return;
            }

            SetAndInitialize(typeof(T), data);
        }

        /// <summary>
        /// Sets the data of type <typeparamref name="T"/> to a new instance of <typeparamref name="T"/>. It's best practice to only call this for your own data.
        /// </summary>
        /// <typeparam name="T">The type of data to set.</typeparam>
        /// <returns>The newly created data.</returns>
        public T SetData<T>() where T : ICoreData<TData>, new()
        {
            T data = new T();
            if (_dict.TryGetValue(typeof(T), out ICoreData<TData> existing))
            {
                int index = _list.IndexOf(existing);
                _list[index] = data;
                _dict[typeof(T)] = data;
                data.Initialize(_item);
                return data;
            }

            SetAndInitialize(typeof(T), data);
            return data;
        }

        /// <summary>
        /// Sets the data of type of <paramref name="data"/> to the value if it doesn't already exist. It's best practice to only call this for your own data.
        /// </summary>
        /// <param name="data">The data to set.</param>
        /// <returns>The existing data if data of type of <paramref name="data"/> if it already exists, otherwise <paramref name="data"/>.</returns>
        public ICoreData<TData> SetDefaultData(ICoreData<TData> data)
        {
            if (_dict.TryGetValue(data.GetType(), out ICoreData<TData> existing))
            {
                return existing;
            }

            SetAndInitialize(data.GetType(), data);
            return data;
        }

        /// <summary>
        /// Sets the data of type <typeparamref name="T"/> to the value if it doesn't already exist. It's best practice to only call this for your own data.
        /// </summary>
        /// <param name="data">The data to set.</param>
        /// <returns>The existing data if data of type <typeparamref name="T"/> already exists, otherwise <paramref name="data"/>.</returns>
        public T SetDefaultData<T>(T data) where T : ICoreData<TData>
        {
            if (_dict.TryGetValue(typeof(T), out ICoreData<TData> existing))
            {
                return (T)existing;
            }

            SetAndInitialize(typeof(T), data);
            return data;
        }

        /// <summary>
        /// Sets the data of type <typeparamref name="T"/> to a new instance of <typeparamref name="T"/> if it doesn't already exist. It's best practice to only call this for your own data.
        /// </summary>
        /// <typeparam name="T">The type of data to set.</typeparam>
        /// <returns>The newly created data, or the existing data if it already exists.</returns>
        public T SetDefaultData<T>() where T : ICoreData<TData>, new()
        {
            if (_dict.TryGetValue(typeof(T), out ICoreData<TData> existing))
            {
                return (T)existing;
            }

            T data = new T();
            SetAndInitialize(typeof(T), data);
            return data;
        }

        private void SetAndInitialize(Type type, ICoreData<TData> data)
        {
            _list.Add(data);
            _list.Sort(CompareData);
            _dict.Add(type, data);
            data.Initialize(_item);
        }

        /// <summary>
        /// Returns true if the collection contains any data that should save state.
        /// </summary>
        /// <returns>True if the collection contains any data that should save state.</returns>
        public bool ShouldSaveState()
        {
            foreach (ICoreData<TData> data in _list)
            {
                if (data.ShouldSave)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Reads the state of a <see cref="CoreDataCollection{TData}"/> from a <see cref="BinaryReader"/>.
        /// </summary>
        /// <param name="modManager">The main <see cref="ICoreModManager"/>, used to find mods adding the types saved.</param>
        /// <param name="reader">The <see cref="BinaryReader"/> to read from.</param>
        /// <param name="tmVersion">The TM version the data was written in.</param>
        /// <param name="coreVersion">The Core Mod version the data was written in.</param>
        public void ReadState(ICoreModManager modManager, BinaryReader reader, int tmVersion, int coreVersion)
        {
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                if (coreVersion < 1)
                {
                    // mod ID no longer needed
                    reader.ReadString();
                }
                string typeName = reader.ReadString();
                int bytes = reader.ReadInt32();

                Type type = null;
                foreach (ICoreMod mod in CorePlugin.Instance.Game.ModManager.GetAllActivePlugins())
                {
                    type = mod.Assembly.GetType(typeName);
                    break;
                }
                
                if (type == null)
                {
#if DEBUG
                    CorePlugin.Warn($"Type {typeName} does not exist.");
#endif
                    reader.BaseStream.Position += bytes;
                    continue;
                }
                else if (!type.IsAssignableTo(typeof(ICoreData<TData>)))
                {
#if DEBUG
                    CorePlugin.Warn($"Type {typeName} does not implement {typeof(ICoreData<TData>).Name}");
#endif
                    reader.BaseStream.Position += bytes;
                    continue;
                }

                ICoreData<TData> data = GetOrCreateData(type, out bool created);

                long start = reader.BaseStream.Position;
                long end = start + bytes;

                if (data == null)
                {
                    reader.BaseStream.Position = end;
                    continue;
                }

                // Just in case a mod reads data incorrectly, other mods should be unaffected.
                try
                {
                    data.ReadState(reader, tmVersion, coreVersion);
                }
                catch (EndOfStreamException)
                {
                    reader.BaseStream.Position = end;
                }

                if (reader.BaseStream.Position != end)
                {
                    reader.BaseStream.Position = end;
                }

                if (created)
                {
                    _list.Add(data);
                    _dict.Add(type, data);
                }
            }

            _list.Sort(CompareData);
        }

        private ICoreData<TData> GetOrCreateData(Type type, out bool created)
        {
            if (_dict.TryGetValue(type, out ICoreData<TData> data))
            {
                created = false;
                return data;
            }

            created = true;
            data = Activator.CreateInstance(type) as ICoreData<TData>;
            data.Initialize(_item);
            return data;
        }

        /// <summary>
        /// Writes the state of this <see cref="CoreDataCollection{TData}"/> to a <see cref="BinaryWriter"/>.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        public void WriteState(BinaryWriter writer)
        {
            long start = writer.BaseStream.Position;
            writer.Write(0);

            int count = 0;
            foreach (ICoreData<TData> data in _list)
            {
                if (!data.ShouldSave)
                {
                    continue;
                }

                count++;
                writer.Write(data.GetType().FullName);
                long lengthPos = writer.BaseStream.Position;
                writer.Write(0);
                long itemStart = lengthPos + sizeof(int);
                data.WriteState(writer);
                long itemEnd = writer.BaseStream.Position;
                writer.BaseStream.Position = lengthPos;
                writer.Write((int)(itemEnd - itemStart));
                writer.BaseStream.Position = itemEnd;
            }

            long end = writer.BaseStream.Position;
            writer.BaseStream.Position = start;
            writer.Write(count);
            writer.BaseStream.Position = end;
        }

        IEnumerator<ICoreData<TData>> IEnumerable<ICoreData<TData>>.GetEnumerator()
        {
            return ((IEnumerable<ICoreData<TData>>)_list).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_list).GetEnumerator();
        }

        IEnumerator<ICoreData<TData>> IHasCoreData<TData>.GetDataEnumerator()
        {
            return ((IEnumerable<ICoreData<TData>>)_list).GetEnumerator();
        }

        /// <summary>
        /// Creates a new <see cref="CoreDataCollection{TData}"/> instance for the specified item.
        /// </summary>
        /// <param name="item">The item this <see cref="CoreDataCollection{TData}"/> contains custom data for.</param>
        public CoreDataCollection(TData item)
        {
            _list = new List<ICoreData<TData>>();
            _dict = new Dictionary<Type, ICoreData<TData>>();
            _item = item;
        }
    }
}
