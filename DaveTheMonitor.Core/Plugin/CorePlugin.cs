﻿using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Components;
using DaveTheMonitor.Core.Json;
using DaveTheMonitor.Core.Patches;
using DaveTheMonitor.Scripts;
using DaveTheMonitor.Scripts.Compiler;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.BlockWorld;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace DaveTheMonitor.Core.Plugin
{
    /// <summary>
    /// The main plugin for the Core mod.
    /// </summary>
    public sealed class CorePlugin : ITMPlugin
    {
        /// <summary>
        /// Main main CorePlugin instance.
        /// </summary>
        public static CorePlugin Instance { get; private set; }

        /// <summary>
        /// Core's ICoreMod.
        /// </summary>
        public static ICoreMod CoreMod { get; private set; }

        /// <summary>
        /// True if this instance of Core Mod is currently loaded. Used by patches as they may execute during a hot reload.
        /// </summary>
        public static bool IsValid => Instance?.Game != null;

        /// <summary>
        /// Core's ITMMod.
        /// </summary>
        public ITMMod TMMod { get; private set; }
        
        /// <summary>
        /// The main game instance.
        /// </summary>
        public ICoreGame Game => _game;
        internal CoreGame _game;
        private PatchHelper _patchHelper;
        private int _itemOffset;
        private List<Assembly> _loadedAssemblies;

#if DEBUG
        public static void Log(string message)
        {
            Debug.WriteLine($"Core: {message}");
        }

        public static void Warn(string message)
        {
            Debug.WriteLine($"WARNING: Core: {message}");
        }
#endif

        internal Assembly Resolve(AssemblyName name)
        {
            foreach (Assembly assembly in _loadedAssemblies)
            {
                AssemblyName target = assembly.GetName();
                if (name.Name == target.Name &&
                    name.CultureName == target.CultureName &&
                    name.Version.Major == target.Version.Major &&
                    name.Version <= target.Version)
                {
#if DEBUG
                    Log($"Resolved {name}");
#endif
                    return assembly;
                }
            }
            return null;
        }

        /// <summary>
        /// ITMPlugin.Callback implementation
        /// </summary>
        public void Callback(string data, GlobalPoint3D? p, ITMActor actor, ITMActor contextActor)
        {
            
        }

        /// <summary>
        /// ITMPlugin.Draw implementation
        /// </summary>
        public void Draw(ITMPlayer player, ITMPlayer virtualPlayer, Viewport vp)
        {
            ((CoreWorld)_game._currentWorld).ActorRenderer.Draw(_game.GetPlayer(player), virtualPlayer);
            _game.ModManager.ModDraw(_game.GetPlayer(player), virtualPlayer, vp);
        }

        /// <summary>
        /// ITMPlugin.HandleInput implementation
        /// </summary>
        public bool HandleInput(ITMPlayer tmPlayer)
        {
            ICorePlayer player = _game.GetPlayer(tmPlayer);
            return _game.ModManager.ModHandleInput(player);
        }

        /// <summary>
        /// ITMPlugin.Initialize implementation
        /// </summary>
        public void Initialize(ITMPluginManager mgr, ITMMod mod)
        {
            CoreGlobalData.Initialize(Path.Combine(mod.FullPath, ModManager.ContentPath));
            _itemOffset = mgr.Offsets.ItemID;
            TMMod = mod;
            Instance = this;
            _patchHelper = new PatchHelper("DaveTheMonitor.Core");
            _patchHelper.PatchAll(Assembly.GetExecutingAssembly());
        }

        /// <summary>
        /// ITMPlugin.InitializeGame implementation
        /// </summary>
        public void InitializeGame(ITMGame game)
        {
            JsonCondition.RegisterConditionTypes(Assembly.GetExecutingAssembly());
            Component.RegisterComponents(Assembly.GetExecutingAssembly());
            _game = new CoreGame(game);
            CoreMod = _game.ModManager.LoadMod(TMMod, Path.Combine(TMMod.FullPath, ModManager.ContentPath));
            _game.ModManager.EnableMod(CoreMod);
            ((CoreWorld)_game._currentWorld).ActorRenderer.LoadContent();

            string modulesPath = Path.Combine(TMMod.FullPath, "Modules");
            _game.ModManager.EnableMod(_game.ModManager.LoadMod(TMMod, Path.Combine(modulesPath, "Particles")));
            _game.ModManager.EnableMod(_game.ModManager.LoadMod(TMMod, Path.Combine(modulesPath, "Effects")));
            //_game.ModManager.EnableMod(_game.ModManager.LoadMod(Mod, Path.Combine(modulesPath, "Biomes")));
            _game.ModManager.EnableAll(_game.ModManager.LoadAll(game.GetActiveMods()));
            _game.AllModsLoaded();

            _game.ItemRegistry.InitializeAllItems(Globals1.ItemData);
            _game.ItemRegistry.RegisterAllJson<CoreItem>(_game.ModManager.GetAllActiveMods(), "Items");
            _game.ItemRegistry.UpdateGlobalItemData();

            RegisterBlueprints();

            _game.ActorRegistry.InitializeAllActors(Globals1.NpcTypeData);
            _game.ActorRegistry.RegisterAllJson<CoreActor>(_game.ModManager.GetAllActiveMods(), "Actors");
            _game.ActorRegistry.UpdateGlobalActorData();

            // TODO: support depth texture?
            Texture2D texture = _game.StitchModItemTextures(game.TexturePack, game.TexturePack.ItemTexture, game.TexturePack.ItemTextureSize(), out bool changed);
            if (changed)
            {
                game.TexturePack.ItemTexture.Dispose();
                Traverse traverse = new Traverse(game.TexturePack);
                traverse.Field("ItemTexture").SetValue(texture);
            }

            Assembly[] asm = AppDomain.CurrentDomain.GetAssemblies();
            List<Assembly> assemblies = new List<Assembly>()
            {
                Assembly.GetExecutingAssembly()
            };
            foreach (ICoreMod mod in _game.ModManager.GetAllActivePlugins())
            {
                assemblies.Add(mod.Assembly);
            }

            ScriptType.RegisterTypes(assemblies);

            ScriptRuntime runtime = new ScriptRuntime(1024, 1024, 128);
            runtime.PrintHandler += ScriptPrintHandler;
            runtime.ErrorHandler += ScriptErrorHandler;
            _game.ScriptRuntime = runtime;
            _game.ScriptCompiler = new ScriptCompiler();
            _game.ScriptCompiler.ErrorHandler += ScriptCompilerErrorHandler;

            foreach (Mod mod in Game.ModManager.GetAllActivePlugins())
            {
                mod.Plugin.InitializeGame(Game);
                foreach (ICoreWorld world in _game.GetAllWorlds())
                {
                    mod.Plugin.InitializeWorld(world);
                }
            }

            _game.RegisterCommands();
            _game.GetModules();

            ReadState(Game.FullPath);

            // We call this after all mods have initialized so any
            // events they added are called.
            // We call this after ReadState so the player save state
            // is read before the player is added.
            _game.AddActors();
        }

        private void RegisterBlueprints()
        {
            if (ModManager.RegisterAllBlueprints(Game) == 0)
            {
                return;
            }

            MethodInfo unlockItem = AccessTools.Method(Game.TMGame.GetType(), "UnlockItem", new Type[]
            {
                typeof(Item),
                typeof(bool)
            });
            Traverse traverse = new Traverse(AccessTools.TypeByName("StudioForge.TotalMiner.Blueprints"));
            traverse.Method("InitializeBlueprints", _game.TMGame).GetValue();
            Array blueprintsList = traverse.Field<Array>("BlueprintList").Value;
            for (int i = 0; i < blueprintsList.Length; i++)
            {
                object bp = blueprintsList.GetValue(i);
                Traverse blueprint = new Traverse(bp);
                if (!blueprint.Field<bool>("IsValid").Value)
                {
                    continue;
                }

                bool enabled = Game.GameMode == GameMode.Survival || Game.GameMode == GameMode.Creative || blueprint.Field<bool>("IsDefault").Value;
                if (!enabled)
                {
                    continue;
                }

                blueprint.Field<bool>("IsEnabled").Value = enabled;
                blueprint.Field<bool>("IsGenerated").Value = enabled;
                InventoryItem item = blueprint.Field<InventoryItem>("Result").Value;

                unlockItem.Invoke(Game.TMGame, new object[] { item.ItemID, false });
            }
        }

        private void ScriptPrintHandler(object sender, ScriptPrintEventArgs e)
        {
            Game.AddNotification(e.Message);
        }

        private void ScriptErrorHandler(object sender, ScriptErrorEventArgs e)
        {
            Game.AddNotification($"{e.Code} : {e.Header} : {e.Message}");
        }

        private void ScriptCompilerErrorHandler(object sender, ScriptCompilerErrorEventArgs e)
        {
            Game.AddNotification($"{e.Code} : {e.Header} : {e.Message}");
        }

        /// <summary>
        /// ITMPlugin.PlayerJoined implementation
        /// </summary>
        public void PlayerJoined(ITMPlayer player)
        {
            
        }

        /// <summary>
        /// ITMPlugin.PlayerLeft implementation
        /// </summary>
        public void PlayerLeft(ITMPlayer player)
        {
            
        }

        /// <summary>
        /// ITMPlugin.RegisterLuaFunctions implementation
        /// </summary>
        public object[] RegisterLuaFunctions(ITMScriptInstance si)
        {
            List<object> funcs = new List<object>();
            foreach (ICoreMod mod in Game.ModManager.GetAllActivePlugins())
            {
                IEnumerable<object> modFuncs = mod.Plugin.RegisterLuaFunctions(si);
                if (modFuncs == null)
                {
                    continue;
                }

                funcs.AddRange(modFuncs);
            }
            return funcs.ToArray();
        }

        /// <summary>
        /// ITMPlugin.UnloadMod implementation
        /// </summary>
        public void UnloadMod()
        {
            Unload();
        }

        internal void HotLoad()
        {
#if DEBUG
            Log("Hotload");
#endif
            foreach (Mod mod in Game.ModManager.GetAllActivePlugins())
            {
                mod.Plugin.HotLoadMod();
            }
            Unload();
        }

        private void Unload()
        {
#if DEBUG
            Log("Unload");
#endif
            _game.ModManager.UnloadAndDisableAll();
            _game.Dispose();
            _game = null;
            Instance = null;
            _patchHelper.Unpatch();
        }

        /// <summary>
        /// ITMPlugin.Update implementation
        /// </summary>
        public void Update()
        {
            _game.Update();
        }

        /// <summary>
        /// ITMPlugin.Update implementation
        /// </summary>
        public void Update(ITMPlayer player)
        {

        }

        /// <summary>
        /// ITMPlugin.WorldSaved implementation
        /// </summary>
        public void WorldSaved(int version)
        {
            string path = _game.FullPath;
            string corePath = Path.Combine(path, "coredata.dat");

            if (Game.ShouldSaveState() || File.Exists(corePath))
            {
                using Stream stream = File.Create(corePath);
                using BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(version);
                writer.Write(CoreGlobalData.CoreSaveVersion);
                Game.WriteState(writer);
            }
        }

        private void ReadState(string path)
        {
            string file = Path.Combine(path, "coredata.dat");
            if (File.Exists(file))
            {
                using Stream stream = File.OpenRead(file);
                using BinaryReader reader = new BinaryReader(stream);
                int tmVersion = reader.ReadInt32();
                int coreVersion = reader.ReadInt32();
                Game.ReadState(reader, tmVersion, coreVersion);
            }
        }

        public CorePlugin(List<Assembly> loadedAssemblies)
        {
            // loadedAssemblies is passed by the loader and contains
            // Harmony and the Scripts assembly.
            _loadedAssemblies = loadedAssemblies;
        }
    }
}
