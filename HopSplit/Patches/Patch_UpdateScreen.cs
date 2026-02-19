using HarmonyLib;
using HopSplit.UI;
using UnityEngine;

namespace HopSplit.Patches
{
    internal class Patch_UpdateScreen : HarmonyPatch
    {
        [HarmonyPatch(typeof(SettingsManager), "AwakeIfNeeded")]
        public static class Patch_SettingsManager_AwakeIfNeeded
        {
            public static void Postfix()
            {
                SingletonPropertyItem<SettingsManager>.Instance.OnStateChanged -= UIHandler.Rebuild;
                SingletonPropertyItem<SettingsManager>.Instance.OnStateChanged += UIHandler.Rebuild;
            }
        }
    }
}
