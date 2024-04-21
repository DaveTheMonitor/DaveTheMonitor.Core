using Microsoft.Xna.Framework.Graphics;
using StudioForge.BlockWorld;
using StudioForge.TotalMiner.API;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace DaveTheMonitor.Core.Loader
{
    public class LoaderPlugin : ITMPlugin
    {
        public static LoaderPlugin Instance { get; private set; }
        public ITMPlugin Plugin { get; private set; }
        private List<Assembly> _assemblies;
        private List<LoaderAssemblyLoadContext> _contexts;
        private Assembly _core;
        private bool _assembliesLoaded;

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

        public void Callback(string data, GlobalPoint3D? p, ITMActor actor, ITMActor contextActor) => Plugin.Callback(data, p, actor, contextActor);
        public void Draw(ITMPlayer player, ITMPlayer virtualPlayer, Viewport vp) => Plugin.Draw(player, virtualPlayer, vp);
        public bool HandleInput(ITMPlayer player) => Plugin.HandleInput(player);
        public void Initialize(ITMPluginManager mgr, ITMMod mod)
        {
            Instance = this;

            if (!_assembliesLoaded)
            {
                LoadAssemblies(mod.FullPath);
            }

            Type type = _core.GetType("DaveTheMonitor.Core.Plugin.CorePlugin");
            ConstructorInfo ctor = type.GetConstructor(new Type[] { typeof(List<Assembly>) });
            Plugin = (ITMPlugin)ctor.Invoke(new object[] { _assemblies });

            Plugin.Initialize(mgr, mod);
        }
        public void InitializeGame(ITMGame game) => Plugin.InitializeGame(game);
        public void PlayerJoined(ITMPlayer player) => Plugin.PlayerJoined(player);
        public void PlayerLeft(ITMPlayer player) => Plugin.PlayerLeft(player);
        public object[] RegisterLuaFunctions(ITMScriptInstance si) => Plugin.RegisterLuaFunctions(si);
        public void UnloadMod() => Plugin.UnloadMod();
        public void Update() => Plugin.Update();
        public void Update(ITMPlayer player) => Plugin.Update(player);
        public void WorldSaved(int version) => Plugin.WorldSaved(version);

        private void LoadAssemblies(string modPath)
        {
            _assemblies = new List<Assembly>();
            _contexts = new List<LoaderAssemblyLoadContext>();

            LoadEmbedded("0Harmony.dll", "0Harmony, Version=2.3.0.0, Culture=neutral, PublicKeyToken=null", false, false);
            LoadFile(Path.Combine(modPath, "Modules", "Scripts", "DaveTheMonitor.Scripts.dll"), true);
            _core = LoadFile(Path.Combine(modPath, "Modules", "DaveTheMonitor.Core.dll"), true);

            _assembliesLoaded = true;
        }

        private Assembly LoadEmbedded(string name, string assemblyName, bool collectible, bool forceLoad)
        {
            if (!forceLoad)
            {
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (Assembly loadedAssembly in assemblies)
                {
                    if (loadedAssembly.FullName == assemblyName)
                    {
                        _assemblies.Add(loadedAssembly);
                        return loadedAssembly;
                    }
                }
            }

            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DaveTheMonitor.Core.Loader." + name);
            LoaderAssemblyLoadContext context = new LoaderAssemblyLoadContext(assemblyName, collectible);
            _contexts.Add(context);

            Assembly asm = context.LoadFromStream(stream);
            _assemblies.Add(asm);
            return asm;
        }

        private Assembly LoadFile(string file, bool collectible)
        {
            using Stream stream = File.OpenRead(file);
            LoaderAssemblyLoadContext context = new LoaderAssemblyLoadContext(Path.GetFileName(file), collectible);
            Assembly assembly = context.LoadFromStream(stream);
            _contexts.Add(context);
            _assemblies.Add(assembly);
            return assembly;
        }

        internal Assembly Resolve(AssemblyName name)
        {
            foreach (Assembly assembly in _assemblies)
            {
                AssemblyName target = assembly.GetName();
                if (name.Name == target.Name &&
                    name.CultureName == target.CultureName &&
                    name.Version.Major == target.Version.Major &&
                    name.Version <= target.Version)
                {
                    return assembly;
                }
            }
            return null;
        }
    }
}
