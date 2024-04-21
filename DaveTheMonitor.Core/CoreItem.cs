﻿using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Components;
using DaveTheMonitor.Core.Helpers;
using DaveTheMonitor.Core.Plugin;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.TotalMiner;
using System;
using System.Diagnostics;
using System.Text.Json;

namespace DaveTheMonitor.Core
{
    /// <summary>
    /// Represents a block or item definition.
    /// </summary>
    [DebuggerDisplay(@"{(IsBlock ? ""Block"" : ""Item"")} : {Id}")]
    public sealed class CoreItem : IDefinition, IJsonType<CoreItem>
    {
        /// <summary>
        /// True if this item is a block, otherwise false.
        /// </summary>
        public bool IsBlock => NumId <= (int)Block.zLastBlockID;

        /// <summary>
        /// The string ID of this item.
        /// </summary>
        public string Id => Definition.ItemId;

        /// <summary>
        /// The numeric ID of this item.
        /// </summary>
        public int NumId { get => (int)ItemType; set => ItemType = (Item)value; }

        /// <summary>
        /// The type of this item.
        /// </summary>
        public Item ItemType { get; private set; }

        /// <summary>
        /// The type of this block, or <see cref="Block.None"/> if this item is not a block.
        /// </summary>
        public Block BlockType => IsBlock ? (Block)ItemType : Block.None;

        /// <summary>
        /// This item's defined components.
        /// </summary>
        public ComponentCollection Components { get; private set; }

        /// <summary>
        /// This item's <see cref="ItemDefinitionComponent"/>.
        /// </summary>
        public ItemDefinitionComponent Definition { get; private set; }

        /// <summary>
        /// This item's <see cref="ItemDisplayComponent"/>.
        /// </summary>
        public ItemDisplayComponent Display { get; private set; }

        /// <summary>
        /// This item's <see cref="ItemDurabilityComponent"/>, if it has one.
        /// </summary>
        /// <remarks>This may be null.</remarks>
        public ItemDurabilityComponent Durability { get; private set; }

        /// <summary>
        /// This item's <see cref="ItemLockedComponent"/>, if it has one.
        /// </summary>
        /// <remarks>This may be null.</remarks>
        public ItemLockedComponent Locked { get; private set; }

        /// <summary>
        /// This item's <see cref="ItemStackableComponent"/>, if it has one.
        /// </summary>
        /// <remarks>This may be null.</remarks>
        public ItemStackableComponent Stackable { get; private set; }

        /// <summary>
        /// This item's <see cref="ItemStatBonusComponent"/>, if it has one.
        /// </summary>
        /// <remarks>This may be null.</remarks>
        public ItemStatBonusComponent StatBonus { get; private set; }

        /// <summary>
        /// This item's <see cref="ItemTradeableComponent"/>, if it has one.
        /// </summary>
        /// <remarks>This may be null.</remarks>
        public ItemTradeableComponent Tradeable { get; private set; }

        /// <summary>
        /// This item's <see cref="ItemTypeComponent"/>, if it has one.
        /// </summary>
        /// <remarks>This may be null.</remarks>
        public ItemTypeComponent TypeComponent { get; private set; }

        /// <summary>
        /// This item's <see cref="ItemWeaponComponent"/>, if it has one.
        /// </summary>
        /// <remarks>This may be null.</remarks>
        public ItemWeaponComponent Weapon { get; private set; }

        /// <summary>
        /// This item's HD texture if it was added with the Json API. Use `<see cref="CoreExtensions.GetTexture(StudioForge.TotalMiner.API.ITMTexturePack, CoreItem)"/> if you want to actually display the item's texture.
        /// </summary>
        /// <remarks>This may be null.</remarks>
        public Texture2D TextureHD { get; private set; }

        /// <summary>
        /// This item's HD texture src rectangle if it was added with the Json API. Use `<see cref="CoreExtensions.GetSrcRect(StudioForge.TotalMiner.API.ITMTexturePack, CoreItem)"/> if you want to actually display the item's texture.
        /// </summary>
        public Rectangle TextureHDSrc { get; private set; }

        /// <summary>
        /// This item's SD texture if it was added with the Json API. Use `<see cref="CoreExtensions.GetTexture(StudioForge.TotalMiner.API.ITMTexturePack, CoreItem)"/> if you want to actually display the item's texture.
        /// </summary>
        /// <remarks>This may be null.</remarks>
        public Texture2D TextureSD { get; private set; }

        /// <summary>
        /// This item's SD texture src rectangle if it was added with the Json API. Use `<see cref="CoreExtensions.GetSrcRect(StudioForge.TotalMiner.API.ITMTexturePack, CoreItem)"/> if you want to actually display the item's texture.
        /// </summary>
        public Rectangle TextureSDSrc { get; private set; }
        internal ICoreGame _game;
        
        public static CoreItem FromJson(string json)
        {
            JsonDocumentOptions docOptions = DeserializationHelper.DocumentOptionsTrailingCommasSkipComments;
            JsonSerializerOptions serializerOptions = DeserializationHelper.SerializerOptionsTrailingCommasSkipComments;
            ComponentCollection components = DeserializationHelper.ReadComponents(json, "Item", docOptions, serializerOptions);

            if (!components.HasComponent<ItemDefinitionComponent>())
            {
                throw new InvalidOperationException("Item must have a Definition component.");
            }

            return new CoreItem(Item.None, components);
        }

        public void ReplaceWith(ICoreMod mod, IJsonType<CoreItem> other)
        {
            other.Components.CopyTo(Components, true);
            LoadAssets(mod, other.Components);
            UpdateFields();
        }

        public void OnRegister(ICoreMod mod)
        {
            LoadAssets(mod, Components);
            UpdateFields();
        }

        private void LoadAssets(ICoreMod mod, ComponentCollection components)
        {
            var textureComponent = components.GetComponent<ItemTextureComponent>();
            ICoreGame Game = CorePlugin.Instance.Game;
            if (textureComponent != null)
            {
                int hdSize = IsBlock ? 64 : 32;
                TextureHD = Game.ModManager.GetTexture(mod, textureComponent.HD, hdSize);
                TextureHDSrc = new Rectangle(0, 0, hdSize, hdSize);

                TextureSD = Game.ModManager.GetTexture(mod, textureComponent.SD, 16);
                TextureSDSrc = new Rectangle(0, 0, 16, 16);
            }
        }

        private void UpdateFields()
        {
            Definition = Components.GetComponent<ItemDefinitionComponent>();
            Display = Components.GetComponent<ItemDisplayComponent>();
            Durability = Components.GetComponent<ItemDurabilityComponent>();
            Locked = Components.GetComponent<ItemLockedComponent>();
            Stackable = Components.GetComponent<ItemStackableComponent>();
            StatBonus = Components.GetComponent<ItemStatBonusComponent>();
            Tradeable = Components.GetComponent<ItemTradeableComponent>();
            TypeComponent = Components.GetComponent<ItemTypeComponent>();
            Weapon = Components.GetComponent<ItemWeaponComponent>();
        }

        /// <summary>
        /// Creates a new <see cref="CoreItem"/> definition from <see cref="ItemDataXML"/>.
        /// </summary>
        /// <param name="data">The data </param>
        /// <returns></returns>
        public static CoreItem FromItemDataXML(ItemDataXML data)
        {
            ItemTypeDataXML typeData = Globals1.ItemTypeData[(int)data.ItemID];

            ComponentCollection components = new ComponentCollection();
            components.AddComponent(ItemDefinitionComponent.FromXML(data));
            components.AddComponent(ItemDisplayComponent.FromXML(data));
            components.AddComponent(ItemDurabilityComponent.FromXML(data));
            components.AddComponent(ItemLockedComponent.FromXML(data));
            components.AddComponent(ItemStackableComponent.FromXML(data));
            components.AddComponent(ItemStatBonusComponent.FromXML(Globals1.ItemCombatData[(int)typeData.Combat]));
            components.AddComponent(ItemTradeableComponent.FromXML(data));
            components.AddComponent(ItemTypeComponent.FromXML(typeData));
            components.AddComponent(ItemWeaponComponent.FromXML(data));
            components.SetComponentDefaults();
            CoreItem item = new CoreItem(data.ItemID, components);

            return item;
        }

        public SwingTime GetSwingTime(float currentTime)
        {
            ref ItemSwingTimeDataXML data = ref Globals1.ItemSwingTimeData[NumId];
            float extendTime;
            if (data.RetractTime == -1)
            {
                extendTime = (data.Time - data.ExtendedPause - data.Pause) / 2f;
            }
            else
            {
                extendTime = data.Time - data.ExtendedPause - data.RetractTime - data.Pause;
            }

            return new SwingTime(currentTime, data.Time, extendTime);
        }

        public SwingTime GetSwingTime(SwingState state)
        {
            ref ItemSwingTimeDataXML data = ref Globals1.ItemSwingTimeData[NumId];
            float extendTime;
            if (data.RetractTime == -1)
            {
                extendTime = (data.Time - data.ExtendedPause - data.Pause) / 2f;
            }
            else
            {
                extendTime = data.Time - data.ExtendedPause - data.RetractTime - data.Pause;
            }

            float currentTime = state switch
            {
                SwingState.Extended => extendTime,
                SwingState.Retracting => extendTime + data.ExtendedPause,
                SwingState.Delay => data.Time - data.Pause,
                SwingState.Complete => data.Time,
                _ => 0
            };
            return new SwingTime(currentTime, data.Time, extendTime);
        }

        public override string ToString()
        {
            return Id;
        }

        private CoreItem(Item item, ComponentCollection components)
        {
            ItemType = item;
            Components = components;
            UpdateFields();
        }
    }
}
