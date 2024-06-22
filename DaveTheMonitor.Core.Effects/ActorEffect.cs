using DaveTheMonitor.Core.API;
using StudioForge.Engine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DaveTheMonitor.Core.Effects
{
    public sealed class ActorEffect : IHasCoreData<ActorEffect>, IHasBinaryState
    {
        public ActorEffectDefinition Definition { get; private set; }
        public ICoreActor Actor { get; private set; }
        public float Age { get; private set; }
        public float Duration { get; private set; }
        public ICoreGame Game => Actor?.Game;
        private CoreDataCollection<ActorEffect> _data;

        public void SetActor(ICoreActor actor)
        {
            Actor = actor;
        }

        public void SetDuration(float duration)
        {
            Age = 0;
            Duration = duration;
        }

        public void Remove()
        {
            Actor.Effects().Remove(this);
        }

        public void Update()
        {
            if (Duration != -1 && Age >= Duration)
            {
                Remove();
                return;
            }
            Definition.Update(this);
            Age += Services.ElapsedTime;
        }

        public T GetData<T>() where T : ICoreData<ActorEffect>
        {
            if (_data == null)
            {
                return default(T);
            }
            return _data.GetData<T>();
        }

        public bool TryGetData<T>(out T result) where T : ICoreData<ActorEffect>
        {
            if (_data == null)
            {
                result = default(T);
                return false;
            }
            return _data.TryGetData(out result);
        }

        public void GetAllData(List<ICoreData<ActorEffect>> result)
        {
            if (_data == null)
            {
                result.Clear();
                return;
            }
            _data.GetAllData(result);
        }

        public bool HasData<T>()
        {
            return _data?.HasData<T>() ?? false;
        }

        public void SetData(ICoreData<ActorEffect> data)
        {
            _data ??= new CoreDataCollection<ActorEffect>(this);
            _data.SetData(data);
        }

        public void SetData<T>(T data) where T : ICoreData<ActorEffect>
        {
            _data ??= new CoreDataCollection<ActorEffect>(this);
            _data.SetData(data);
        }

        public T SetData<T>() where T : ICoreData<ActorEffect>, new()
        {
            _data ??= new CoreDataCollection<ActorEffect>(this);
            return _data.SetData<T>();
        }

        public ICoreData<ActorEffect> SetDefaultData(ICoreData<ActorEffect> data)
        {
            _data ??= new CoreDataCollection<ActorEffect>(this);
            return _data.SetDefaultData(data);
        }

        public T SetDefaultData<T>(T data) where T : ICoreData<ActorEffect>
        {
            _data ??= new CoreDataCollection<ActorEffect>(this);
            return _data.SetDefaultData(data);
        }

        public T SetDefaultData<T>() where T : ICoreData<ActorEffect>, new()
        {
            _data ??= new CoreDataCollection<ActorEffect>(this);
            return _data.SetDefaultData<T>();
        }

        public IEnumerator<ICoreData<ActorEffect>> GetDataEnumerator()
        {
            if (_data != null)
            {
                return ((IHasCoreData<ActorEffect>)_data).GetDataEnumerator();
            }

            return Enumerable.Empty<ICoreData<ActorEffect>>().GetEnumerator();
        }

        public void ReadState(BinaryReader reader, int tmVersion, int coreVersion)
        {
            string definition = reader.ReadString();
            float age = reader.ReadSingle();
            float duration = reader.ReadSingle();            
            if (reader.ReadBoolean())
            {
                _data = new CoreDataCollection<ActorEffect>(this);
                _data.ReadState(Game.ModManager, reader, tmVersion, coreVersion);
            }
            Definition = Game.EffectRegistry().GetDefinition(definition);
            Age = age;
            Duration = duration;
        }

        public void WriteState(BinaryWriter writer)
        {
            writer.Write(Definition.Id);
            writer.Write(Age);
            writer.Write(Duration);
            if (_data?.ShouldSaveState() == true)
            {
                writer.Write(true);
                _data.WriteState(writer);
            }
            else
            {
                writer.Write(false);
            }
        }

        public ActorEffect(ActorEffectDefinition definition, float duration)
        {
            Definition = definition;
            Age = 0;
            Duration = duration;
            _data = null;
        }

        internal ActorEffect(ActorEffectDefinition definition, float age, float duration)
        {
            Definition = definition;
            Age = age;
            Duration = duration;
            _data = null;
        }
    }
}
