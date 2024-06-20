using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Plugin;
using StudioForge.TotalMiner;
using System.Collections.Generic;
using System.Diagnostics;

namespace DaveTheMonitor.Core
{
    internal sealed class ActorRegistry : DefinitionRegistry<CoreActor>, ICoreActorRegistry
    {
        public void InitializeAllActors(IEnumerable<ActorTypeDataXML> data)
        {
            foreach (ActorTypeDataXML xml in data)
            {
                string id = xml.IDString;
                if (HasDefinition(id))
                {
#if DEBUG
                    Debugger.Break();
                    CorePlugin.Warn($"Duplicate actor Id: {id}");
#endif
                    continue;
                }

                CoreActor actor = CoreActor.FromActorTypeDataXML(xml);
                ICoreMod mod = Game.ModManager.GetDefiningMod(actor.ActorType);
                RegisterDefinition(actor, mod);
            }
        }

        public void UpdateGlobalActorData()
        {
            foreach (CoreActor actor in this)
            {
                SetItemData(Globals1.NpcTypeData[actor.NumId], actor);
            }
        }

        private void SetItemData(ActorTypeDataXML data, CoreActor actor)
        {
            actor.Definition?.ReplaceXmlData(data);
            actor.Display?.ReplaceXmlData(data);
            actor.NaturalSpawn?.ReplaceXmlData(data);
            actor.ImmuneToFire?.ReplaceXmlData(data);
            actor.BreatheUnderwater?.ReplaceXmlData(data);
            actor.Passive?.ReplaceXmlData(data);
            actor.Combat?.ReplaceXmlData(data);
        }

        protected override void OnRegister(CoreActor definition)
        {
            definition._game = Game;
        }

        public CoreActor GetActor(ActorType actor)
        {
            return GetDefinition((int)actor);
        }

        public ActorRegistry(ICoreGame game) : base(game, null)
        {

        }
    }
}
