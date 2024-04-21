using System.Reflection;

namespace DaveTheMonitor.Core.Patches
{
    /// <summary>
    /// Contains info about a patch applies by a <see cref="PatchHelper"/>.
    /// </summary>
    public sealed class PatchInfo
    {
        /// <summary>
        /// The methods that was patched.
        /// </summary>
        public MethodInfo Target { get; private set; }
        /// <summary>
        /// The prefix applied to the method, if any.
        /// </summary>
        public MethodInfo Prefix { get; private set; }
        /// <summary>
        /// The postfix applied to the method, if any.
        /// </summary>
        public MethodInfo Postfix { get; private set; }
        /// <summary>
        /// The transpiler applied to the method, if any.
        /// </summary>
        public MethodInfo Transpiler { get; private set; }
        /// <summary>
        /// The finalizer applied to the method, if any.
        /// </summary>
        public MethodInfo Finalizer { get; private set; }

        public PatchInfo(MethodInfo target, MethodInfo prefix, MethodInfo postfix, MethodInfo transpiler, MethodInfo finalizer)
        {
            Target = target;
            Prefix = prefix;
            Postfix = postfix;
            Transpiler = transpiler;
            Finalizer = finalizer;
        }
    }
}
