using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DaveTheMonitor.Core.Patches
{
    /// <summary>
    /// A helper for patching methods using methods with the <see cref="PatchAttribute"/>.
    /// </summary>
    public class PatchHelper
    {
        /// <summary>
        /// The <see cref="HarmonyLib.Harmony"/> instance used by this <see cref="PatchHelper"/>.
        /// </summary>
        public Harmony Harmony { get; private set; }
        private List<PatchInfo> _patches;

        /// <summary>
        /// Patches the targets of the specified type with the <see cref="PatchAttribute"/>.
        /// </summary>
        /// <param name="type">The type with the patches to apply.</param>
        public void Patch(Type type)
        {
#if DEBUG
            CorePlugin.Log($"Patching {type.FullName}");
#endif
            bool nullTarget = (bool)(type.GetMethod("NullTarget", BindingFlags.Static | BindingFlags.Public)?.Invoke(null, null) ?? false);

            IEnumerable<MethodInfo> targets;
            PatchAttribute attribute = type.GetCustomAttribute<PatchAttribute>();
            if (attribute?.TypeName != null && attribute?.MethodName != null)
            {
                string typeName = attribute.TypeName;
                string methodName = attribute.MethodName;
                string[] paramTypes = attribute.ParamTypes;
                if (paramTypes?.Length > 0)
                {
                    Type[] types = new Type[paramTypes.Length];
                    for (int i = 0; i < paramTypes.Length; i++)
                    {
                        Type paramType = AccessTools.TypeByName(paramTypes[i]);
                        if (paramType == null)
                        {
                            if (nullTarget)
                            {
                                return;
                            }
                            else
                            {
                                throw new PatchException(type, "Parameter type must exist.");
                            }
                        }
                        types[i] = paramType;
                    }
                    targets = new MethodInfo[] { AccessTools.Method(AccessTools.TypeByName(typeName), methodName, types) };
                }
                else
                {
                    MethodInfo target = AccessTools.Method(AccessTools.TypeByName(typeName), methodName);
                    if (target == null)
                    {
                        if (nullTarget)
                        {
                            return;
                        }
                        else
                        {
                            throw new PatchException(type, "Target method must exist.");
                        }
                    }
                    targets = new MethodInfo[] { target };
                }
            }
            else
            {
                MethodInfo targetMethodMethod = type.GetMethod("TargetMethod", BindingFlags.Static | BindingFlags.Public);
                if (targetMethodMethod != null)
                {
                    MethodInfo target = (MethodInfo)targetMethodMethod.Invoke(null, null);
                    if (target == null)
                    {
                        if (nullTarget)
                        {
                            return;
                        }
                        else
                        {
                            throw new PatchException(type, "TargetMethod must not return null.");
                        }
                    }
                    targets = new MethodInfo[] { target };
                }
                else
                {
                    MethodInfo targetMethodsMethod = type.GetMethod("TargetMethods", BindingFlags.Static | BindingFlags.Public);
                    if (targetMethodsMethod == null)
                    {
                        throw new PatchException(type, "Must implement TargetMethod or TargetMethods.");
                    }
                    targets = (IEnumerable<MethodInfo>)targetMethodsMethod.Invoke(null, null);
                    if (targets == null)
                    {
                        if (nullTarget)
                        {
                            return;
                        }
                        else
                        {
                            throw new PatchException(type, "TargetMethods must not return null.");
                        }
                    }

                    foreach (MethodInfo target in targets)
                    {
                        if (target == null)
                        {
                            if (nullTarget)
                            {
                                return;
                            }
                            else
                            {
                                throw new PatchException(type, "TargetMethod must not return a null target.");
                            }
                        }
                    }
                }
            }
            
            HarmonyMethod prefix = GetMethod(type, "Prefix");
            HarmonyMethod postfix = GetMethod(type, "Postfix");
            HarmonyMethod transpiler = GetMethod(type, "Transpiler");
            HarmonyMethod finalizer = GetMethod(type, "Finalizer");
            foreach (MethodInfo target in targets)
            {
                Harmony.Patch(target, prefix, postfix, transpiler, finalizer);
                _patches.Add(new PatchInfo(target, prefix?.method, postfix?.method, transpiler?.method, finalizer?.method));
            }
        }

        /// <summary>
        /// Applies all patches in an assembly.
        /// </summary>
        /// <param name="assembly">The assembly containing the patches to apply.</param>
        public void PatchAll(Assembly assembly)
        {
#if DEBUG
            CorePlugin.Log($"Patching all for {assembly.FullName}");
#endif
            foreach (Type type in assembly.GetTypes())
            {
                if (type.GetCustomAttribute<PatchAttribute>() != null)
                {
                    Patch(type);
                }
            }
        }

        private HarmonyMethod GetMethod(Type type, string name)
        {
            MethodInfo patch = type.GetMethod(name, BindingFlags.Static | BindingFlags.Public);
            return patch != null ? new HarmonyMethod(patch) : null;
        }

        /// <summary>
        /// Unpatches all patches applies by this <see cref="PatchHelper"/>.
        /// </summary>
        public void Unpatch()
        {
            Harmony.UnpatchAll(Harmony.Id);
        }

        /// <summary>
        /// Gets the info for patches applied by this <see cref="PatchHelper"/>.
        /// </summary>
        /// <returns>The info for patches applies by this <see cref="PatchHelper"/>.</returns>
        public PatchInfo[] GetPatchedMethods()
        {
            return _patches.ToArray();
        }

        /// <summary>
        /// Creates a new <see cref="PatchHelper"/> with the specified ID.
        /// </summary>
        /// <param name="id">The ID of this <see cref="PatchHelper"/>.</param>
        public PatchHelper(string id)
        {
            _patches = new List<PatchInfo>();
            Harmony = new Harmony(id);
        }
    }
}
