using DaveTheMonitor.Core.Helpers;
using StudioForge.TotalMiner;
using System;
using System.Text.Json;

namespace DaveTheMonitor.Core.Components.Items
{
    [Component("Core.ItemTexture", "Texture", "Item")]
    public sealed class ItemTextureComponent : Component, IComponentDeserializable
    {
        public override string ComponentId => "Core.ItemTexture";
        public string HD { get; private set; }
        public string SD { get; private set; }

        Type IComponentDeserializable.GetDeserializeType(ModVersion version) => typeof(JsonElement);
        void IComponentDeserializable.ReadFrom(ModVersion version, object obj)
        {
            JsonElement element = (JsonElement)obj;
            HD = DeserializationHelper.GetStringProperty(element, "HD");
            SD = DeserializationHelper.GetStringProperty(element, "SD");
        }

        public override void ReplaceWith(Component replacement)
        {
            var component = (ItemTextureComponent)replacement;
            if (component.HD != null) HD = component.HD;
            if (component.SD != null) SD = component.SD;
        }

        public override void SetDefaults()
        {
            HD ??= null;
            SD ??= null;
        }
    }
}
