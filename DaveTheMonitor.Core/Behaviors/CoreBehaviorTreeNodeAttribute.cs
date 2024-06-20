using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaveTheMonitor.Core.Behaviors
{
    /// <summary>
    /// Marks a subclass of <see cref="CoreBehaviorTreeNode"/> as a behavior tree node to be automatically added.
    /// </summary>
    public sealed class CoreBehaviorTreeNodeAttribute : Attribute
    {
        /// <summary>
        /// The name of this node.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Creates a new <see cref="CoreBehaviorTreeNodeAttribute"/> with the specified name.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        public CoreBehaviorTreeNodeAttribute(string name)
        {
            Name = name;
        }
    }
}
