using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DaveTheMonitor.Core.Patches
{
    internal static class CommonPatches
    {
        public static void InsertFirst(this List<CodeInstruction> list, Predicate<CodeInstruction> predicate, int offset, IEnumerable<CodeInstruction> instructions, bool moveLabels = false)
        {
            InsertFirst(list, predicate, offset, 0, instructions, moveLabels);
        }

        public static void ReplaceFirst(this List<CodeInstruction> list, Predicate<CodeInstruction> predicate, int offset, int instructionsToReplace, IEnumerable<CodeInstruction> instructions, bool moveLabels = false)
        {
            InsertFirst(list, predicate, offset, instructionsToReplace, instructions, moveLabels);
        }

        private static void InsertFirst(this List<CodeInstruction> list, Predicate<CodeInstruction> predicate, int offset, int instructrionsToRemove, IEnumerable<CodeInstruction> instructions, bool moveLabels = false)
        {
            int index = list.FindIndex(predicate);
            if (index == -1)
            {
                return;
            }

            index += offset;
            List<Label> labels = moveLabels ? list[index].ExtractLabels() : null;
            
            if (instructrionsToRemove > 0)
            {
                list.RemoveRange(index, instructrionsToRemove);
            }

            list.InsertRange(index, instructions);

            if (labels != null && labels.Count > 0)
            {
                list[index] = list[index].WithLabels(labels);
            }
        }

        public static void InsertAll(this List<CodeInstruction> list, Predicate<CodeInstruction> predicate, int offset, IEnumerable<CodeInstruction> instructions, bool moveLabels = false)
        {
            InsertAll(list, predicate, offset, 0, instructions, moveLabels);
        }

        public static void ReplaceAll(this List<CodeInstruction> list, Predicate<CodeInstruction> predicate, int offset, int instructionsToReplace, IEnumerable<CodeInstruction> instructions, bool moveLabels = false)
        {
            InsertAll(list, predicate, offset, instructionsToReplace, instructions, moveLabels);
        }

        private static void InsertAll(this List<CodeInstruction> list, Predicate<CodeInstruction> predicate, int offset, int instructrionsToRemove, IEnumerable<CodeInstruction> instructions, bool moveLabels = false)
        {
            int index = list.FindIndex(predicate);
            while (index != -1)
            {
                CodeInstruction[] instructionsArray = instructions.ToArray();
                for (int i = 0; i < instructionsArray.Length; i++)
                {
                    instructionsArray[i] = instructionsArray[i].Clone();
                }

                index += offset;
                int end = index + instructionsArray.Length - instructrionsToRemove + 1;

                List<Label> labels = moveLabels ? list[index].ExtractLabels() : null;

                if (instructrionsToRemove > 0)
                {
                    list.RemoveRange(index, instructrionsToRemove);
                }

                list.InsertRange(index, instructions);

                if (labels != null && labels.Count > 0)
                {
                    list[index] = list[index].WithLabels(labels);
                }

                index = list.FindIndex(end, predicate);
            }
        }

        public static void ModifyFirst(this List<CodeInstruction> list, Predicate<CodeInstruction> predicate, Action<int> action)
        {
            int index = list.FindIndex(predicate);
            if (index == -1)
            {
                return;
            }

            action(index);
        }
    }
}
