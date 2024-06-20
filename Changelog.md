# v0.1.0-dev

Initial release

# v0.1.1-dev

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

# v0.1.2-dev

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

# v0.1.3-dev

This update is primarily focused on improving the C# API and adding a new animation system.

## Highlights

This is a new section of the patch notes that aims to highlight the most important changes/additions of the update in an easier to understand way. All future updates will include this section.

Changed the default load directory of Core content from `CSR` to `CoreContent`. This change will break all existing core mods, so be sure to rename that directory accordingly.

Added Core content and `ModInfo.xml` to repo.

Added a new animation system that supports skeletal animations.

Added `ActorDataAttribute` and `PlayerDataAttribute` to allow auto-initializing `ICoreData` implementations on actors without using an `ICoreActorManager.ActorAdded` event listener.

`ICoreData` is no longer limited to one data instance per mod, now being limited to one data instance per type. This means one mod can add multiple pieces of data to an actor.

Mod assets are now lazy loaded. This means that until an asset is used, it is not loaded.

C# mods can now add custom asset types thanks to the above using `ICoreModManager.AddAssetLoader(Type, string, string, ICoreAssetLoader)` in `ICorePlugin.Initialize` before they load any assets. To make this possible, the mod manager is now available through `ICoreMod`.

## C# Changes

`DaveTheMonitor.Core.API`
- Added `ActorDataAttribute`
  - This attribute can be applied to implementations of `ICoreData<ICoreActor>` to automatically create and initialize it for actors without requiring an `ActorManager.ActorAdded` event listener.
- Added `PlayerDataAttribute`
  - Like `ActorDataAttribute`, this attribute tells the Core mod to automatically create and initialize the data, but only for players.
- `ICoreActor`
  - Renamed `IsOnGround` to `Grounded`
  - Added `ActorModel Model { get; }`
  - Added `AnimationControllerAnimation { get; }`
  - Added `bool PlayAnimation(string)`
  - Added `bool IsOnGround(float)`
  - Added `void Update()`
- `ICoreActorManager`
  - Added `void Update()`
- `ICoreData`
  - Added `int Priority { get; }`
- `ICoreGame`
  - Added `ICoreActor` GetActor(GamerID)
  - Added `ICorePlayer` GetPlayer(GamerID)
  - Added `bool TryGetPlayer(ITMPlayer, out ICorePlayer)`
  - This method exists as not all `ITMPlayer` instances will have an associated `ICorePlayer`, eg. CCTV, but custom data from the virtual player may be needed for UI drawing. This method can be used to easily ensure the virtual player is an actual player.
  - Added `void InitializeDefaultData(ICoreActor)`
    - Implementation detail, don't call this method.
- `ICoreMod`
  - Replaced `void Load(ModInfo, IMapComponentLoader)` with `void Load(ModInfo)`
  - Removed `ContentManager MGContent`
    - MonoGame content is now loaded through `ModContentManager.MGContent`
  - Removed `CoreModAsset GetAsset(string)`
  - Removed `T GetAsset<T>(string)`
  - Removed `Texture2D GetTexture(string, int)`
  - Removed `ICoreMap GetComponent(string)`
  - Removed `string GetFullPathToAsset(string)`
  - Added `ModContentManager Content { get; }`
    - All mod content loading is now done through this content manager.
  - Added `ICoreModManager ModManager { get; }`
- `ICoreModManager`
  - Replaced `CoreModAsset GetAsset(ICoreMod, string)` with `LoadAsset(ICoreMod, string)`
  - Replaced `T GetAsset<T>(ICoreMod, string)` with `LoadAsset<T>(ICoreMod, string)`
  - Replaced `Texture2D GetTexture(ICoreMod, string)` with `Texture2D LoadTexture(ICoreMod, string)`
  - Replaced `ICoreMap GetComponent(ICoreMod, string)` with `ICoreMap LoadComponent(ICoreMod, string)`
  - Added `ActorModel LoadActorModel(ICoreMod, string)`
  - Added `JsonActorAnimation LoadActorAnimation(ICoreMod, string)`
  - Added `JsonAnimationController LoadAnimationController(ICoreMod, string)`
  - Added `void AddAssetLoader(Type, string, string, ICoreAssetLoader)`
- `ICoreWorld`
  - Added `ICoreActorRenderer ActorRenderer { get; }`
  - Added `void Update()`
- `IHasCoreData<T>`
  - Data is no longer limited to one data instance per mod, it is now instead limited to one instance per type. This means a mod instance is no longer required to set or get any data, only the type.
  - Replaced `T GetData<T>(ICoreMod)` with `T GetData<T>()`
  - Replaced `void SetData(ICoreMod, ICoreData<TSelf>)` with `void SetData<ICoreData<TSelf>)`
  - Replaced `T SetDefaultData<T>(ICoreMod)` with `T SetDefaultData<T>()`
  - Added `bool TryGetData<T>(out T)`
  - Added `void GetAllData(List<ICoreData<TSelf>> result)`
  - Added `bool HasData<T>()`
  - Added `void SetData<T>(T)`
  - Added `T SetData<T>()`
  - Added `ICoreData<TSelf> SetDefaultData(<ICoreData<TSelf>)`
  - Added `T SetDefaultData<T>(T)`

`DaveTheMonitor.Core`
- Renamed `GlobalData` to `CoreGlobalData`
- Added `CoreDataInitializer<T>`
- Added `ModContentManager`

`DaveTheMonitor.Core.Animation`
- `KeyframeCollection<T>`
  - Renamed `TotalTime` to `Length`
  - Renamed `SetKeyFrames` to `SetKeyframes`
  - Added `void GetAllKeyframes(float, float, List<T>)`
  - Added `List<T> GetAllKeyframes(float, float)`
  - Added `KeyframeCollection<T> Add(float, T)`
  - Added `KeyframeCollection<T> Clone()`
  - Added constructor `KeyframeCollection()`
  - Fixed some bugs in `GetValue(float)` and `GetKeyframes(float, out Keyframe<T>, out Keyframe<T>)`
- Added `ActorAnimation`
- Added `ActorKeyframeChannel`
- Added `ActorModel`
- Added `ActorPart`
- Added `ActorPartKeyframe`
- Added `ActorPartSnapshot`
- Added `AnimationController`
- Added `AnimationLoopType`
- Added `AnimationState`
- Added `EasingType`
- Added `InterpolationExtensions`
- Added `ICoreActorRenderer`

`DaveTheMonitor.Core.Animation.Json`
- Added `JsonActorAnimation`
- Added `JsonAnimationController`
- Added `JsonAnimationState`
- Added `JsonAnimationTransition`

`DaveTheMonitor.Core.Assets.Loaders`
- Added `ICoreAssetLoader`

`DaveTheMonitor.Core.Assets`
- `CoreModAsset`
  - Replaced constructor `CoreModAsset(string)` with `CoreModAsset(string, string)`
  - Added `string Name { get; }`
- `CoreMapAsset`
  - Replaced constructor `CoreMapAsset(string, ICoreMap)` with `CoreMapAsset(string, string, ICoreMap)`
- `CoreTextureAsset`
  - Replaced constructor `CoreTextureAsset(string, Texture2D)` with `CoreMapAsset(string, string, Texture2D)`
- Added `CoreActorAnimationAsset`
- Added `CoreActorModelAsset`
- Added `CoreAnimationControllerAsset`

`DaveTheMonitor.Core.Behaviors`
- Added `CoreBehaviorTreeNode`
- Added `CoreBehaviorTreeNodeAttribute`

`DaveTheMonitor.Core.Components`
- Moved to namespace `DaveTheMonitor.Core.Components.Actors`:
  - `ActorBreatheUnderwaterComponent`
  - `ActorCombatComponent`
  - `ActorDefinitionComponent`
  - `ActorImmuneToFireComponent`
  - `ActorNaturalSpawnComponent`
  - `ActorPassiveComponent`
- Moved to namespace `DaveTheMonitor.Core.Components.Items`:
  - `ItemDefinitionComponent`
  - `ItemDisplayComponent`
  - `ItemDurabilityComponent`
  - `ItemLockedComponent`
  - `ItemStackableComponent`
  - `ItemStatBonusComponent`
  - `ItemTextureComponent`
  - `ItemTradeableComponent`
  - `ItemTypeComponent`
  - `ItemWeaponComponent`

`DaveTheMonitor.Core.Components.Actors`
- Added `ActorAnimationControllerComponent`
- Added `ActorModelComponent`

`DaveTheMonitor.Core.Helpers`
- `MethodHelper`
  - Fixed return type conversion verification
- `DeserializationHelper`
  - Added `BoundingBox? GetBoundingBoxProperty(JsonElement, string)`
- Removed `Interpolation`
  - Use `KeyframeCollection<T>` with `InterpolationExtensions` instead.

`DaveTheMonitor.Core.Json`
- Added `InvalidCoreJsonException`
- Added `JsonCondition`
  - JsonConditions can be deserialized from Json and provide a way for Json definitions to define more complex conditions for certain actions. Currently only used for animation controllers.
- Added `JsonConditionAttribute`
- Added `JsonConditionOperator`
- Added `AllCondition`
- Added `AnimationFinishedCondition`
- Added `AnyCondition`
- Added `BooleanCondition`
- Added `FalseCondition`
- Added `HealthCondition`
- Added `HorizontalSpeedCondition`
- Added `IsOnGroundCondition`
- Added `IsSwingingCondition`
- Added `SingleComparisonCondition`
- Added `TrueCondition`
- Added `VerticalSpeedCondition`
- Added `XVelocityCondition`
- Added `YVelocityCondition`
- Added `ZVelocityCondition`