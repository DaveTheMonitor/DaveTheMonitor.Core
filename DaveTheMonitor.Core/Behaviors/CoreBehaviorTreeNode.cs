using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Plugin;
using StudioForge.TotalMiner.AI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaveTheMonitor.Core.Behaviors
{
    /// <summary>
    /// A behavior node for use with the Core Mod.
    /// </summary>
    [BehaviourTreeNode("PlayAnimation", BehaviourTreeNodeType.Action)]
    public abstract class CoreBehaviorTreeNode : BehaviourTreeNode
    {
        /// <summary>
        /// The actor for this execution.
        /// </summary>
        protected new ICoreActor Actor { get; private set; }
        /// <summary>
        /// The world the actor for this execution is in.
        /// </summary>
        protected ICoreWorld World { get; private set; }

        /// <summary>
        /// CoreBehaviorTreeNode's implementation of <see cref="BehaviourTreeNode.UpdateCore(ITMBehaviourExecutionEngine)"/>.
        /// </summary>
        /// <remarks>
        /// If overriding this implementation, <see cref="Actor"/> and <see cref="World"/> will be null. Override <see cref="CoreUpdateCore(ITMBehaviourExecutionEngine)"/> instead unless you must override this method.
        /// </remarks>
        /// <param name="engine">The execution engine.</param>
        protected override void UpdateCore(ITMBehaviourExecutionEngine engine)
        {
            if (actor == null || CorePlugin.Instance?.Game == null)
            {
                Status = BehaviourTreeNodeStatus.Failure;
                return;
            }

            Actor = CorePlugin.Instance.Game.GetActor(actor);
            if (Actor == null)
            {
                Status = BehaviourTreeNodeStatus.Failure;
                return;
            }

            World = Actor.World;
            Status = CoreUpdateCore(engine);
        }

        /// <summary>
        /// Called when this node updates.
        /// </summary>
        /// <param name="engine">The execution engine.</param>
        /// <returns></returns>
        protected abstract BehaviourTreeNodeStatus CoreUpdateCore(ITMBehaviourExecutionEngine engine);

        /// <summary>
        /// Core's implementation of <see cref="BehaviourTreeNode.WriteStateCore(BinaryWriter)"/>. Do not override this method unless you have to. Implement <see cref="CoreWriteStateCore(BinaryWriter)"/> instead.
        /// </summary>
        /// <param name="writer">The writer to write data to.</param>
        protected override void WriteStateCore(BinaryWriter writer)
        {
            base.WriteStateCore(writer);
            writer.Write(CoreGlobalData.CoreSaveVersion);
            CoreWriteStateCore(writer);
        }

        /// <summary>
        /// Core's implementation of <see cref="BehaviourTreeNode.ReadStateCore(BinaryReader, int)"/>. Do not override this method unless you have to. Implement <see cref="CoreReadStateCore(BinaryReader, int, int)"/> instead.
        /// </summary>
        /// <param name="reader">The reader to read data from.</param>
        /// <param name="version">The TM version the data was written in.</param>
        protected override void ReadStateCore(BinaryReader reader, int version)
        {
            base.ReadStateCore(reader, version);
            int coreVersion = reader.ReadInt32();
            CoreReadStateCore(reader, version, coreVersion);
        }

        /// <summary>
        /// Called when this node's data is serialized.
        /// </summary>
        /// <param name="writer">The writer to write data to.</param>
        protected virtual void CoreWriteStateCore(BinaryWriter writer)
        {

        }

        /// <summary>
        /// Called when this node's data is deserialized.
        /// </summary>
        /// <param name="reader">The reader to read data from.</param>
        /// <param name="tmVersion">The TM version this data was written in.</param>
        /// <param name="coreVersion">The Core version this data was written in.</param>
        protected virtual void CoreReadStateCore(BinaryReader reader, int tmVersion, int coreVersion)
        {

        }
    }
}
