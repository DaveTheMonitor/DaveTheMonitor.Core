using DaveTheMonitor.Core.Helpers;
using DaveTheMonitor.Core.Json;
using StudioForge.TotalMiner;
using System;
using System.Text.Json;

namespace DaveTheMonitor.Core.Components.Items
{
    [Component("Core.ItemSound", "Sound", "Item")]
    public sealed class ItemSoundComponent : Component, IComponentDeserializable
    {
        public override string ComponentId => "Core.ItemSound";
        public ItemSoundGroup Group => _group.Value;
        public string[] Step { get; private set; }
        public string[] Mine { get; private set; }
        public string[] Dig { get; private set; }
        public string[] Chop { get; private set; }
        public string[] Use { get; private set; }
        public string[] UseFail { get; private set; }
        public string[] Hit { get; private set; }
        private ItemSoundGroup? _group;

        Type IComponentDeserializable.GetDeserializeType(ModVersion version) => typeof(JsonElement);
        void IComponentDeserializable.ReadFrom(ModVersion version, object obj)
        {
            JsonElement element = (JsonElement)obj;
            _group = DeserializationHelper.GetEnumProperty<ItemSoundGroup>(element, "Group");
            Step = GetStringArray(element, "Step");
            Mine = GetStringArray(element, "Mine");
            Dig = GetStringArray(element, "Dig");
            Chop = GetStringArray(element, "Chop");
            Use = GetStringArray(element, "Use");
            UseFail = GetStringArray(element, "UseFail");
            Hit = GetStringArray(element, "Hit");
        }

        private string[] GetStringArray(JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out JsonElement property) && property.ValueKind == JsonValueKind.Array)
            {
                string[] arr = new string[property.GetArrayLength()];
                for (int i = 0; i < arr.Length; i++)
                {
                    JsonElement value = property[i];
                    if (value.ValueKind != JsonValueKind.String)
                    {
                        throw new InvalidCoreJsonException($"ItemSoundComponent {propertyName} must be an array of strings.");
                    }

                    arr[i] = value.GetString();
                }
                return arr;
            }
            return null;
        }

        public override void ReplaceWith(Component replacement)
        {
            var component = (ItemSoundComponent)replacement;
            if (component._group.HasValue) _group = component._group;
            if (component.Step != null) Step = component.Step;
            if (component.Mine != null) Mine = component.Mine;
            if (component.Dig != null) Dig = component.Dig;
            if (component.Chop != null) Chop = component.Chop;
            if (component.Use != null) Use = component.Use;
            if (component.UseFail != null) UseFail = component.UseFail;
            if (component.Hit != null) Hit = component.Hit;
        }

        public void ReplaceXmlData(ref ItemSoundDataXML data)
        {
            if (_group.HasValue) data.Group = Group;
            if (Step != null) data.Sounds.Step = Step;
            if (Mine != null) data.Sounds.Mine = Mine;
            if (Dig != null) data.Sounds.Dig = Dig;
            if (Chop != null) data.Sounds.Chop = Chop;
            if (Use != null) data.Sounds.Use = Use;
            if (UseFail != null) data.Sounds.UseFail = UseFail;
            if (Hit != null) data.Sounds.Hit = Hit;
        }

        public override void SetDefaults()
        {
            _group ??= ItemSoundGroup.ItemWoodTool;
            Step ??= null;
            Mine ??= null;
            Dig ??= null;
            Chop ??= null;
            Use ??= null;
            UseFail ??= null;
            Hit ??= null;
        }

        public static ItemSoundComponent FromXML(ItemSoundDataXML data)
        {
            var component = new ItemSoundComponent
            {
                _group = data.Group,
                Step = data.Sounds.Step,
                Mine = data.Sounds.Mine,
                Dig = data.Sounds.Dig,
                Chop = data.Sounds.Chop,
                Use = data.Sounds.Use,
                UseFail = data.Sounds.UseFail,
                Hit = data.Sounds.Hit
            };

            return component;
        }
    }
}
