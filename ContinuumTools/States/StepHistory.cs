using Last_Known_Reality.Reality;
using System;
using System.Collections.Generic;

namespace ContinuumTools.States
{
    public static class StepHistory
    {
        public static List<DissLine> History = new();
        private static int MaxHistoryLength = 1000;

        public static void PushToHistory(DissLine instruction)
        {
            if (instruction != null)
            {
                History.Add(instruction);

                if (History.Count > MaxHistoryLength)
                {
                    int itemsToRemove = History.Count - MaxHistoryLength;
                    History.RemoveRange(0, itemsToRemove);
                }
            }
        }

        public static List<DissLine> GetAtMost(int count)
        {
            List<DissLine> response = new();
            for (int i = Math.Max(0, History.Count - count); i < History.Count; i++)
            {
                response.Add(History[i]);
            }

            return response;
        }

        public static void Clear()
        {
            History.Clear();
        }
    }
}
