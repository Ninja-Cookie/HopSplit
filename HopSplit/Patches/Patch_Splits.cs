using HarmonyLib;
using HopSplit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HopSplit.Patches
{
    internal class Patch_Splits : HarmonyPatch
    {
        internal static GoalData[] Splits = Array.Empty<GoalData>();

        // Is a new game if no world state was loaded
        [HarmonyPatch(typeof(LoadManager), "LoadToSaveScene")]
        public static class Patch_LoadManager_LoadToSaveScene
        {
            public static void Prefix(WorldState loadWorldState)
            {
                if (loadWorldState == null)
                    LiveSplit.ConnectionManager.StartingNewGame = true;
            }
        }

        // Split triggered if result is True
        [HarmonyPatch(typeof(SpeedrunManager), "AddSplit")]
        public static class Patch_SpeedrunManager_AddSplit
        {
            public static void Postfix(bool __result, SpeedrunManager.SpeedrunSplit split, List<SpeedrunManager.SpeedrunSplit> ___splits)
            {
                if (__result && ConfigHandler.ActiveSplits.TryGetValue(split.goal.name, out var shouldSplit) && shouldSplit)
                {
                    if (ConfigHandler.ForceSyncTime)
                    {
                        var finalSplit = ___splits.FirstOrDefault(x => x.goal == split.goal);
                        if (finalSplit != null)
                            LiveSplit.ConnectionManager.StartSettingGameTime(TimeSpan.FromSeconds(finalSplit.absoluteTime));
                    }

                    LiveSplit.ConnectionManager.StartSplit();
                }
            }
        }

        // Loading has started
        [HarmonyPatch(typeof(LoadManager), "StartLoading")]
        public static class Patch_LoadManager_StartLoading
        {
            public static void Prefix()
            {
                if (!ConfigHandler.ForceSyncTime)
                    LiveSplit.ConnectionManager.StartPausingTimer();
            }
        }

        // Loading has ended
        [HarmonyPatch(typeof(LoadManager), "FinishLoading")]
        public static class Patch_LoadManager_FinishLoading
        {
            public static void Postfix()
            {
                LiveSplit.ConnectionManager.StartUnpausingTimer();
                if (ConfigHandler.ForceSyncTime)
                    LiveSplit.ConnectionManager.StartSyncingToGame();
            }
        }

        // Get splits
        [HarmonyPatch(typeof(PanelSpeedrun), "AwakeIfNeeded")]
        public static class Patch_PanelSpeedrun_AwakeIfNeeded
        {
            public static void Postfix(GoalData ___splitsGoal)
            {
                if (Splits.Length == 0)
                {
                    Splits = ___splitsGoal.subgoals.Distinct().ToArray();
                    ConfigHandler.InitSplits(Splits);
                    DataHandler.LoadSplits();
                }
            }
        }
    }
}
