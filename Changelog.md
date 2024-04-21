# 0.1.0-dev

Initial release

# 0.1.1-dev

## General Changes

`help` command without arguments now displays Core commands.

Mods can now save game and world data.

Added save data to the `Effects` module.

## C# Changes

`ICorePlugin`
- Removed `void WorldSaved(int)` and `void WorldLoaded()` - use world and game data with binary state instead.
- Added default empty implementations for `void Initialize(ICoreMod)` and `void Initialize(ICoreGame)`.

`ICoreGame`
- Added `bool ShouldSaveState()`
- Added `string FullPath`
- Added `void LoadPlayerState(ICorePlayer)`

`ICoreWorld`
- Now implements `IHasBinaryState`
- Removed `event ComponentPasted`. It may be re-added at a later date.
- Added `bool ShouldSaveState()`
- Renamed `WorldPath` to `FullPath` and changed to always return the full path.

`ICorePlayer`
- Now implements `IHasBinaryState`
- Added `bool ShouldSaveState()`

`IHasBinaryState`
- Replaced `void ReadState(BinaryReader, int)` with `void ReadState(BinaryReader, int, int)`

`CoreDataCollection<T>`
- Now implements `IEnumerable<ICoreData<T>>`
- Added `bool ShouldSaveState()`

`ActorEffect`
- Now implements `IHasBinaryState`

Renamed `CommandRegister` to `CommandRegistry`
- Earlier in development the `DefinitionRegistry` was named `DefinitionRegister` before being renamed. `CommandRegister` has now been renamed for consistency.

# 0.1.2-dev

This update is primarily focused on C# API cleanup and documentation. If you aren't a C# developer, most of these changes won't affect you.

## General Changes

Particles Module now uses instanced rendering.

## C# Changes

`KeyframeCollection`
- Added `void SetKeyframes(IEnumerable<Keyframe<T>>)`

`DeserializationHelper`
- Removed `bool TryParseArrayUnmanaged<T>(ReadOnlySpan<char>, IFormatProvider, out T[]) where T : unmanaged, ISpanParsable<T>`
  - I don't remember what this was for, and it didn't appear any different from `bool TryParseArray<T>(ReadOnlySpan<char>, IFormatProvider, out T[]) where T : ISpanParsable<T>`
- Added `ComponentCollection ReadComponents(JsonElement, string, JsonSerializerOptions)`
- Replaced `T[] ReadAllFromPath<T>(string) where T : IJsonType<T>` with `T[] ReadAllFromPath<T>(string, SearchOption) where T : IJsonType<T>`
- Removed `void ReadData<T>(BinaryReader, ICollection<ICoreData<T>>)` and `void WriteData<T>(BinaryWriter, ICollection<ICoreData<T>>)`
  - These were left over from before `CoreDataCollection` was added and were never implemented.
- Added `bool TryParseModVersion(string, out ModVersion)`

`PatchInfo`
- Replaced `MethodBase Method` with `MethodInfo Target`
- Replaced `MethodBase Prefix` with `MethodInfo Prefix`
- Replaced `MethodBase Postfix` with `MethodInfo Postfix`
- Replaced `MethodBase Transpiler` with `MethodInfo Transpiler`
- Added `MethodInfo Finalizer { get; }`

`IMapLoader`
- Renamed `IMapLoader` to `IMapComponentLoader`
  - The `IMapLoader` was only able to load TM components, so this rename is to make that clearer.
- Renamed `ITMMap LoadMap(*)` to `ITMMap LoadComponent(*)`

`ICoreGame`
- Renamed `IMapComponentLoader MapLoader` to `IMapComponentLoader MapComponentLoader`
- Renamed `void TeleportEntities(*)` to `void TeleportActors(*)`
  - The TM API contains this method, but because entities and actors are different, it has been renamed here to make its function clearer.
- Added `ICoreWorld GetWorld(ICoreMap)`

`ICoreActor`
- Replaced `bool EquipFromInventory(Item)` with `bool EquipFromInventory(CoreItem)`
- Replaced `bool EquipFromInventory(ICoreHand, Item)` with `bool EquipFromInventory(ICoreHand, CoreItem)`
- Replaced `bool IsItemEquipped(Item)` with `bool IsItemEquipped(CoreItem)`
- Replaced `bool IsItemEquippedAndUsable(Item)` with `bool IsItemEquippedAndUsable(CoreItem)`
- Replaced `int GetItemEquippedSlot(Item)` with `bool GetItemEquippedSlot(CoreItem)`

`ICoreActorManager`
- Added `ICoreActor GetActor(GamerID)`
- Added `ICorePlayer GetPlayer(GamerID)`

`CoreDataCollection`
- Added `GetData(ICoreMod)`
- Renamed `AddData<T>(ICoreMod)` to `SetDefaultData<T>(ICoreMod)`

`CoreExtensions`
- Moved from `DaveTheMonitor.Core` namespace to `DaveTheMonitor.Core.Helpers`
- Replaced `float GetFarClip(this ITMActor)` with `float GetFarClip(this ITMPlayer)`
  - While technically all actors have a far clip, this method really only makes sense for players (and is only used for `ICorePlayer`, not `ICoreActor`)
- Renamed `float Size(this Zone)` to `float Volume(this Zone)`
- Added `Vector3 Size(this Zone)`
- Added `Texture2D GetTexture(this ITMTexturePack, CoreItem)`
- Added `Rectangle GetSrcRect(this ITMTexturePack, CoreItem)`

`ParticleManager`
- Replaces `Update()` with `Update(ITMPlayer, bool, bool)`
  - Earlier in development particles were updated every frame independent of drawing. It was then changed so `Update` was called each rendered frame. Since `Update` and `Draw` are now called at the same time, they have been merged into a single method that both updates active particles and builds the instance buffer data. The new method takes a bool for whether or not it should update particles, and a bool for whether or not it should build the instance data. This is helpful if you want to draw particles without updating them ("pausing" the updates but still drawing)

`ICoreItemRegistry`
- Renamed `UpdateAllItemData` to `UpdateGlobalItemData`

`ICoreActorRegistry`
- Renamed `UpdateAllActorData` to `UpdateGlobalActorData`

`IGameShader`
- Removed `Vector4 TintColor`
- Removed `Vector4 SkyColor`
- Removed `void AddTintColorModifier(ShaderModifier<Vector4>)`
- Removed `void AddSkyColorModifier(ShaderModifier<Vector4>)`
- Removed `void ApplyTintColorModifiers(ICorePlayer, ITMPlayer, ref Vector4)`
- Removed `void ApplySkyColorModifiers(ICorePlayer, ITMPlayer, ref Vector4)`
  - These were never implemented. They will be re-added when they are properly implemented.

`GlobalData`
- Removed `IEnumerable<CoreItem> Items`
- Removed `void RegisterOrReplaceItems(IEnumerable<CoreItem>)`
- Removed `Item GetItemId(string)`
- Removed `void ApplyItemChanges()`
  - These were leftover from before the `ICoreItemRegistry` was added, and were never fully implemented. All intended functionality is available through `ICoreItemRegistry`.

`HasBinaryState`
- Moved to namespace `DaveTheMonitor.Core.API`.

`IJsonType<T>`
- Moved to namespace `DaveTheMonitor.Core.API`.

`ActorRegistry` is now internal.
- The actor registry is exposed through `ICoreActorRegistry`, there's no reason to have the base implementation public.

`ItemRegistry` is now internal.
- The item registry is exposed through `ICoreItemRegistry`, there's no reason to have the base implementation public.

Removed `CoreUtils`
- `ModVersion? CoreUtils.ParseModVersion(string)` is now available as `bool DeserializationHelper.TryParseModVersion(string, out ModVersion)`