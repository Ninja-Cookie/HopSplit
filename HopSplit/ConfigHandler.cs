using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HopSplit
{
    internal static class ConfigHandler
    {
        internal readonly static Dictionary<string, bool> ActiveSplits = new Dictionary<string, bool>();
        internal static bool ForceSyncTime = false;
        internal static bool DisplayFPS = true;

        internal static void InitSplits(GoalData[] goalData)
        {
            ActiveSplits.Clear();
            foreach (GoalData goal in goalData)
                if (!ActiveSplits.ContainsKey(goal.name))
                    ActiveSplits.Add(goal.name, false);
        }

        internal static void UpdateSplit(string split, bool state)
        {
            if (ActiveSplits.ContainsKey(split) && ActiveSplits[split] != state)
            {
                ActiveSplits[split] = state;
                DataHandler.Save();
            }
        }
    }
}
