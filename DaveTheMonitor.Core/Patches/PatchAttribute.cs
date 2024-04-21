using System;

namespace DaveTheMonitor.Core.Patches
{
    /// <summary>
    /// Marks a method as a patch for a <see cref="PatchHelper"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class PatchAttribute : Attribute
    {
        /// <summary>
        /// The name of the type to patch.
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// The name of the method to patch.
        /// </summary>
        public string MethodName { get; set; }
        /// <summary>
        /// The names of the types of parameters of the method to patch.
        /// </summary>
        public string[] ParamTypes { get; set; }

        /// <summary>
        /// Creates a new <see cref="PatchAttribute"/>.
        /// </summary>
        public PatchAttribute()
        {

        }

        /// <summary>
        /// Creates a new <see cref="PatchAttribute"/> with the specified info.
        /// </summary>
        /// <param name="typeName">The name of the type to patch.</param>
        /// <param name="methodName">The name of the method to patch.</param>
        /// <param name="paramTypes">The names of the types of parameters of the method to patch.</param>
        public PatchAttribute(string typeName, string methodName, params string[] paramTypes)
        {
            TypeName = typeName;
            MethodName = methodName;
            ParamTypes = paramTypes;
        }
    }
}
