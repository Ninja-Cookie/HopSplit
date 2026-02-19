using SharpCompress.Common;
using UnityEngine;

namespace HopSplit.UI
{
    internal static class UIWindows
    {
        private static int MainChain = 0;

        internal static void Main(int id)
        {
            MainChain = 0;

            for (int i = 0; i < Patches.Patch_Splits.Splits.Length; i++)
                if (ConfigHandler.ActiveSplits.TryGetValue(Patches.Patch_Splits.Splits[i].name, out var value))
                    ConfigHandler.UpdateSplit(Patches.Patch_Splits.Splits[i].name, UIElements.Checkbox(id, ref MainChain, value, Patches.Patch_Splits.Splits[i].displayText, UnityEngine.TextAnchor.MiddleCenter));

            UIElements.EmptySpace(id, ref MainChain);

            UIFunctions.ToggleForceSyncTime(UIElements.Button(id, ref MainChain, ConfigHandler.ForceSyncTime ? "Disable Syncing to In-Game Display Time" : "Force Sync to In-Game Display Time", UnityEngine.TextAnchor.MiddleCenter, UIColors.ElementBackgroundOnAlt, ConfigHandler.ForceSyncTime));
            UIFunctions.ToggleConnectionLiveSplit(UIElements.Button(id, ref MainChain, LiveSplit.ConnectionManager.IsConnected ? "Disconnect LiveSplit" : "Connect LiveSplit", UnityEngine.TextAnchor.MiddleCenter, UIColors.ElementBackgroundOnAlt, LiveSplit.ConnectionManager.IsConnected));
        }
    }
}
