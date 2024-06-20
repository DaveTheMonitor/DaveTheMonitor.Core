using DaveTheMonitor.Core.Animation;
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
    [BehaviourTreeNode("PlayAnimation", BehaviourTreeNodeType.Action)]
    internal sealed class PlayAnimationNode : CoreBehaviorTreeNode
    {
        public string Animation;
        public bool WaitForCompletion;
        private bool _played;

        public override string ToStringParms => Animation;

        protected override BehaviourTreeNodeStatus CoreUpdateCore(ITMBehaviourExecutionEngine engine)
        {
            AnimationController animation = Actor.Animation;
            if (Actor.Model == null || animation == null)
            {
                return BehaviourTreeNodeStatus.Failure;
            }

            if (WaitForCompletion && _played)
            {
                if (animation.CurrentState.Id != Animation || animation.Finished)
                {
                    _played = false;
                    return BehaviourTreeNodeStatus.Success;
                }
                else
                {
                    return BehaviourTreeNodeStatus.Running;
                }
            }
            bool success = animation.PlayAnimation(Animation);
            _played = success;
            if (WaitForCompletion)
            {
                return BehaviourTreeNodeStatus.Running;
            }
            return success ? BehaviourTreeNodeStatus.Success : BehaviourTreeNodeStatus.Failure;
        }

        protected override void CoreWriteStateCore(BinaryWriter writer)
        {
            writer.Write(Animation ?? "");
            writer.Write(WaitForCompletion);
        }

        protected override void CoreReadStateCore(BinaryReader reader, int tmVersion, int coreVersion)
        {
            Animation = reader.ReadString();
            WaitForCompletion = reader.ReadBoolean();
        }
    }
}
