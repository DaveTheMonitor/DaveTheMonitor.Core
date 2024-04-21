using DaveTheMonitor.Core.API;
using StudioForge.Engine;
using System.IO;

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

        public T GetData<T>(ICoreMod mod) where T : ICoreData<ActorEffect>
        {
            if (_data == null)
            {
                return default(T);
            }
            return _data.GetData<T>(mod);
        }

        public void SetData(ICoreMod mod, ICoreData<ActorEffect> data)
        {
            _data ??= new CoreDataCollection<ActorEffect>(this);
            _data.SetData(mod, data);
        }

        public T SetDefaultData<T>(ICoreMod mod) where T : ICoreData<ActorEffect>, new()
        {
            _data ??= new CoreDataCollection<ActorEffect>(this);
            return _data.SetDefaultData<T>(mod);
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
