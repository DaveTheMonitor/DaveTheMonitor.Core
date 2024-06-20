using DaveTheMonitor.Core.Helpers;
using StudioForge.TotalMiner;
using System;
using System.Text.Json;

namespace DaveTheMonitor.Core.Components.Items
{
    [Component("Core.ItemType", "Type", "Item")]
    public sealed class ItemTypeComponent : Component, IComponentDeserializable
    {
        public override string ComponentId => "Core.ItemType";
        public ItemUse Use => _use.Value;
        public ItemType Type => _type.Value;
        public ItemSubType SubType => _subType.Value;
        public ItemTypeClass Class => _class.Value;
        public ItemInvType InvTab => _invTab.Value;
        public ItemModelType Model => _model.Value;
        public ItemSwingType SwingType => _swingType.Value;
        public EquipIndex EquipSlot => _equipSlot.Value;
        public ItemRarityType Rarity => _rarity.Value;
        private ItemUse? _use;
        private ItemType? _type;
        private ItemSubType? _subType;
        private ItemTypeClass? _class;
        private ItemInvType? _invTab;
        private ItemModelType? _model;
        private ItemSwingType? _swingType;
        private EquipIndex? _equipSlot;
        private ItemRarityType? _rarity;

        Type IComponentDeserializable.GetDeserializeType(ModVersion version) => typeof(JsonElement);
        void IComponentDeserializable.ReadFrom(ModVersion version, object obj)
        {
            JsonElement element = (JsonElement)obj;
            _use = DeserializationHelper.GetEnumProperty<ItemUse>(element, "Use");
            _type = DeserializationHelper.GetEnumProperty<ItemType>(element, "Type");
            _subType = DeserializationHelper.GetEnumProperty<ItemSubType>(element, "SubType");
            _class = DeserializationHelper.GetEnumProperty<ItemTypeClass>(element, "Class");
            _invTab = DeserializationHelper.GetEnumProperty<ItemInvType>(element, "InvTab");
            _model = DeserializationHelper.GetEnumProperty<ItemModelType>(element, "Model");
            _swingType = DeserializationHelper.GetEnumProperty<ItemSwingType>(element, "SwingType");
            _equipSlot = DeserializationHelper.GetEnumProperty<EquipIndex>(element, "EquipSlot");
            _rarity = DeserializationHelper.GetEnumProperty<ItemRarityType>(element, "Rarity");
        }

        public override void ReplaceWith(Component replacement)
        {
            var component = (ItemTypeComponent)replacement;
            if (component._use.HasValue) _use = component._use;
            if (component._type.HasValue) _type = component._type;
            if (component._subType.HasValue) _subType = component._subType;
            if (component._class.HasValue) _class = component._class;
            if (component._invTab.HasValue) _invTab = component._invTab;
            if (component._model.HasValue) _model = component._model;
            if (component._swingType.HasValue) _swingType = component._swingType;
            if (component._equipSlot.HasValue) _equipSlot = component._equipSlot;
            if (component._rarity.HasValue) _rarity = component._rarity;
        }

        public void ReplaceXmlData(ref ItemTypeDataXML data)
        {
            if (_use.HasValue) data.Use = Use;
            if (_type.HasValue) data.Type = Type;
            if (_subType.HasValue) data.SubType = SubType;
            if (_class.HasValue) data.Class = Class;
            if (_invTab.HasValue) data.Inv = InvTab;
            if (_model.HasValue) data.Model = Model;
            if (_swingType.HasValue) data.Swing = SwingType;
            if (_equipSlot.HasValue) data.Equip = EquipSlot;
            if (_rarity.HasValue) data.Rarity = Rarity;
        }

        public override void SetDefaults()
        {
            _use ??= ItemUse.Item;
            _type ??= ItemType.Item;
            _subType ??= ItemSubType.None;
            _class ??= ItemTypeClass.CantMine;
            _invTab ??= ItemInvType.Other;
            _model ??= ItemModelType.Item;
            _swingType ??= ItemSwingType.Item;
            _equipSlot ??= EquipIndex.RightHand;
            _rarity ??= ItemRarityType.None;
        }

        public static ItemTypeComponent FromXML(ItemTypeDataXML data)
        {
            var component = new ItemTypeComponent
            {
                _use = data.Use,
                _type = data.Type,
                _subType = data.SubType,
                _class = data.Class,
                _invTab = data.Inv,
                _model = data.Model,
                _swingType = data.Swing,
                _equipSlot = data.Equip,
                _rarity = data.Rarity
            };

            return component;
        }
    }
}
