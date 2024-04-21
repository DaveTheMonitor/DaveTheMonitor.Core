using DaveTheMonitor.Core.API;
using DaveTheMonitor.Scripts;
using DaveTheMonitor.Scripts.Attributes;
using System.Collections;
using System.Collections.Generic;

namespace DaveTheMonitor.Core.Scripts
{
    [ScriptType(Name = "playerarray")]
    [ScriptIterator(Count = nameof(Count), GetItem = nameof(ItemAt))]
    public sealed class ScriptArrayPlayer : IScriptObject, IEnumerable<ICorePlayer>
    {
        [ScriptTypeField]
        private static ScriptType _scriptType;
        public ScriptType ScriptType => _scriptType;
        public static ScriptArrayPlayer EmptyReadOnly => _emptyReadOnly;

        [ScriptProperty]
        public bool ReadOnly { get; private set; }
        [ScriptProperty]
        public int Count => _list.Count;
        private static readonly ScriptArrayPlayer _emptyReadOnly = new ScriptArrayPlayer(0);
        private IList<ICorePlayer> _list;

        [ScriptMethod]
        public void Add(IScriptRuntime runtime, ICorePlayer item)
        {
            if (ReadOnly)
            {
                runtime.Error(ScriptErrorCode.R_ReadonlyCollection, "Array is Readonly", "Element cannot be added to array; array is readonly.");
                return;
            }
            runtime.Reference.AddReference(item);
            _list.Add(item);
        }

        [ScriptMethod]
        public void Insert(IScriptRuntime runtime, ICorePlayer item, int index)
        {
            if (ReadOnly)
            {
                runtime.Error(ScriptErrorCode.R_ReadonlyCollection, "Array is Readonly", "Element cannot be added to array; array is readonly.");
                return;
            }
            runtime.Reference.AddReference(item);
            _list.Insert(index, item);
        }

        [ScriptMethod]
        public bool Remove(IScriptRuntime runtime, ICorePlayer item)
        {
            if (ReadOnly)
            {
                runtime.Error(ScriptErrorCode.R_ReadonlyCollection, "Array is Readonly", "Element cannot be removed from array; array is readonly.");
                return false;
            }
            runtime.Reference.RemoveReference(item);
            return _list.Remove(item);
        }

        [ScriptMethod]
        public void RemoveAt(IScriptRuntime runtime, int index)
        {
            if (ReadOnly)
            {
                runtime.Error(ScriptErrorCode.R_ReadonlyCollection, "Array is Readonly", "Element cannot be removed from array; array is readonly.");
                return;
            }
            if (index < 0 || index >= _list.Count)
            {
                runtime.Error(ScriptErrorCode.R_OutOfBounds, "Out Of Bounds Access", "Index is out of bounds.");
                return;
            }
            runtime.Reference.RemoveReference(_list[index]);
            _list.RemoveAt(index);
        }

        [ScriptMethod]
        public void Clear(IScriptRuntime runtime)
        {
            if (ReadOnly)
            {
                runtime.Error(ScriptErrorCode.R_ReadonlyCollection, "Array is Readonly", "Array cannot be cleared; it is readonly.");
                return;
            }
            foreach (ICorePlayer item in _list)
            {
                runtime.Reference.RemoveReference(item);
            }
            _list.Clear();
        }

        [ScriptMethod]
        public bool Contains(ICorePlayer item)
        {
            return _list.Contains(item);
        }

        [ScriptMethod]
        public int IndexOf(ICorePlayer item)
        {
            return _list.IndexOf(item);
        }

        [ScriptMethod]
        public ICorePlayer ItemAt(IScriptRuntime runtime, int index)
        {
            if (index < 0 || index >= _list.Count)
            {
                runtime.Error(ScriptErrorCode.R_OutOfBounds, "Out Of Bounds Access", "Index is out of bounds.");
                return null;
            }
            return _list[index];
        }

        [ScriptMethod]
        public ScriptArrayPlayer Copy(IScriptRuntime runtime)
        {
            ScriptArrayPlayer arr = new ScriptArrayPlayer(_list.Count);
            foreach (ICorePlayer item in _list)
            {
                runtime.Reference.AddReference(item);
                arr._list.Add(item);
            }
            return arr;
        }

        public void MakeReadOnly()
        {
            ReadOnly = true;
        }

        void IScriptObject.ReferenceAdded(IScriptReference references)
        {

        }

        void IScriptObject.ReferenceRemoved(IScriptReference references)
        {
            foreach (IScriptObject item in _list)
            {
                references.RemoveReference(item);
            }
        }

        string IScriptObject.ScriptToString() => $"array[{Count}]";

        public IEnumerator<ICorePlayer> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_list).GetEnumerator();
        }

        public ScriptArrayPlayer() : this(4)
        {
            
        }

        public ScriptArrayPlayer(int capacity)
        {
            _list = new List<ICorePlayer>(capacity);
            ReadOnly = false;
        }

        public ScriptArrayPlayer(IEnumerable<ICorePlayer> arr, bool readOnly)
        {
            _list = new List<ICorePlayer>(arr);
            ReadOnly = readOnly;
        }
    }
}
