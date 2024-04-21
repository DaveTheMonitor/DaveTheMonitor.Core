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
        private Dictionary<ICoreMod, ICoreData<TData>> _data;
        private TData _item;

        /// <summary>
        /// Gets the data from the specified mod, or default(T) if the mod didn't add any data. If the type of the data is not known at runtime, use <see cref="GetData(ICoreMod)"/>
        /// </summary>
        /// <typeparam name="T">The type of data to get.</typeparam>
        /// <param name="mod">The mod that added the data.</param>
        /// <returns>The data from the specified mod, or default(T) if the mod didn't add any data.</returns>
        public T GetData<T>(ICoreMod mod) where T : ICoreData<TData>
        {
            ICoreData<TData> data = GetData(mod);
            return data is T t ? t : default;
        }

        /// <summary>
        /// Gets the data from the specified mod, or null if the mod didn't add any data.
        /// </summary>
        /// <param name="mod">The mod that added the data.</param>
        /// <returns>The data from the specified mod, or null if the mod didn't add any data.</returns>
        public ICoreData<TData> GetData(ICoreMod mod)
        {
            if (_data.TryGetValue(mod, out ICoreData<TData> data))
            {
                return data;
            }
            return null;
        }

        /// <summary>
        /// Sets the data for the specified mod, adding it if it doesn't exist.
        /// </summary>
        /// <param name="mod">The mod to associate the data with.</param>
        /// <param name="data">The data to set for the mod.</param>
        public void SetData(ICoreMod mod, ICoreData<TData> data)
        {
            _data[mod] = data;
            data.Initialize(_item);
        }

        /// <summary>
        /// Sets the data for the specified mod to a new instance of <typeparamref name="T"/>, adding it if it doesn't exist.
        /// </summary>
        /// <typeparam name="T">The type of the data to set for the mod.</typeparam>
        /// <param name="mod">The mod to associate the data with.</param>
        /// <returns>The newly created data.</returns>
        public T SetDefaultData<T>(ICoreMod mod) where T : ICoreData<TData>, new()
        {
            T data = new T();
            SetData(mod, data);
            return data;
        }

        /// <summary>
        /// Returns true if the collection contains any data that should save state.
        /// </summary>
        /// <returns>True if the collection contains any data that should save state.</returns>
        public bool ShouldSaveState()
        {
            foreach (ICoreData<TData> data in _data.Values)
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
                string modId = reader.ReadString();
                string typeName = reader.ReadString();
                int bytes = reader.ReadInt32();

                ICoreMod mod = modManager.GetMod(modId);
                if (mod == null)
                {
                    reader.BaseStream.Position += bytes;
                    continue;
                }

                Type type = mod.Assembly.GetType(typeName);
                if (type == null)
                {
#if DEBUG
                    CorePlugin.Warn($"Type {typeName} does not exist in {modId}");
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
                    SetData(mod, data);
                }
            }
        }

        private ICoreData<TData> GetOrCreateData(Type type, out bool created)
        {
            foreach (ICoreData<TData> data in _data.Values)
            {
                if (data.GetType() == type)
                {
                    created = false;
                    return data;
                }
            }

            created = true;
            ICoreData<TData> newData = Activator.CreateInstance(type) as ICoreData<TData>;
            newData?.Initialize(_item);
            return newData;
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
            foreach (KeyValuePair<ICoreMod, ICoreData<TData>> pair in _data)
            {
                if (!pair.Value.ShouldSave)
                {
                    continue;
                }

                count++;
                writer.Write(pair.Key.Id);
                writer.Write(pair.Value.GetType().FullName);
                long lengthPos = writer.BaseStream.Position;
                writer.Write(0);
                long itemStart = lengthPos + sizeof(int);
                pair.Value.WriteState(writer);
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
            return ((IEnumerable<ICoreData<TData>>)_data.Values).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_data.Values).GetEnumerator();
        }

        /// <summary>
        /// Creates a new <see cref="CoreDataCollection{TData}"/> instance for the specified item.
        /// </summary>
        /// <param name="item">The item this <see cref="CoreDataCollection{TData}"/> contains custom data for.</param>
        public CoreDataCollection(TData item)
        {
            _data = new Dictionary<ICoreMod, ICoreData<TData>>();
            _item = item;
        }
    }
}
