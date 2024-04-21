using StudioForge.TotalMiner;
using System;

namespace DaveTheMonitor.Core.Components
{
    public interface IComponentDeserializable
    {
        public Type GetDeserializeType(ModVersion version);
        public void ReadFrom(ModVersion version, object obj);
    }
}
