using DaveTheMonitor.Core.API;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace DaveTheMonitor.Core.Effects
{
    [ActorData]
    public sealed class EffectData : ICoreData<ICoreActor>, IEnumerable<ActorEffect>
    {
        public ICoreActor Actor { get; private set; }
        public bool ShouldSave => _effects.Count > 0;
        private List<ActorEffect> _effects;

        public void Initialize(ICoreActor actor)
        {
            Actor = actor;
        }

        public void Update()
        {
            for (int i = _effects.Count - 1; i >= 0; i--)
            {
                ActorEffect effect = _effects[i];
                ActorEffectDefinition def = effect.Definition;
                effect.Update();
            }
        }

        public ActorEffect Add(string id, float duration, bool allowMultiple)
        {
            if (Actor.Game.EffectRegistry().TryGetDefinition(id, out ActorEffectDefinition definition))
            {
                return Add(definition, duration, allowMultiple);
            }
            return null;
        }

        public ActorEffect Add(ActorEffectDefinition definition, float duration, bool allowMultiple)
        {
            if (!allowMultiple)
            {
                ActorEffect existing = GetEffect(definition);
                if (existing != null)
                {
                    if (existing.Age < duration)
                    {
                        existing.SetDuration(duration);
                    }
                    return existing;
                }
            }

            ActorEffect effect = new ActorEffect(definition, duration);
            effect.SetActor(Actor);
            _effects.Add(effect);
            definition.EffectAdded(effect);
            return effect;
        }

        public bool Remove(string id)
        {
            return Remove(_effects.Find(e => e.Definition.Id == id));
        }

        public bool Remove(ActorEffect effect)
        {
            if (effect == null)
            {
                return false;
            }
            if (_effects.Remove(effect))
            {
                effect.Definition.EffectRemoved(effect);
                return true;
            }
            return false;
        }

        public void Clear()
        {
            for (int i = _effects.Count - 1; i >= 0; i--)
            {
                ActorEffect effect = _effects[i];
                _effects.RemoveAt(i);
                effect.Definition.EffectRemoved(effect);
            }
        }

        public ActorEffect GetEffect(string id)
        {
            foreach (ActorEffect effect in _effects)
            {
                if (effect.Definition.Id == id)
                {
                    return effect;
                }
            }
            return null;
        }

        public ActorEffect GetEffect(ActorEffectDefinition definition)
        {
            foreach (ActorEffect effect in _effects)
            {
                if (effect.Definition == definition)
                {
                    return effect;
                }
            }
            return null;
        }

        public bool TryGetEffect(string id, out ActorEffect effect)
        {
            effect = GetEffect(id);
            return effect != null;
        }

        public bool TryGetEffect(ActorEffectDefinition definition, out ActorEffect effect)
        {
            effect = GetEffect(definition);
            return effect != null;
        }

        public bool HasEffect(string id)
        {
            return GetEffect(id) != null;
        }

        public bool HasEffect(ActorEffectDefinition definition)
        {
            return GetEffect(definition) != null;
        }

        public void ReadState(BinaryReader reader, int tmVersion, int coreVersion)
        {
            reader.ReadInt32();
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                ActorEffect effect = new ActorEffect(null, 0);
                effect.SetActor(Actor);
                effect.ReadState(reader, tmVersion, coreVersion);
                if (effect.Definition == null)
                {
                    continue;
                }

                _effects.Add(effect);
                effect.Definition.EffectAdded(effect);
            }
        }

        public void WriteState(BinaryWriter writer)
        {
            writer.Write(0);
            writer.Write(_effects.Count);
            foreach (ActorEffect effect in _effects)
            {
                effect.WriteState(writer);
            }
        }

        public IEnumerator<ActorEffect> GetEnumerator()
        {
            return ((IEnumerable<ActorEffect>)_effects).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_effects).GetEnumerator();
        }

        public EffectData()
        {
            _effects = new List<ActorEffect>();
        }
    }
}
