using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HopSplit.UI
{
    internal static class UIFunctions
    {
        internal static void ToggleConnectionLiveSplit(bool state)
        {
            if (state && LiveSplit.LiveSplitManager.ConnectionStatus == LiveSplit.LiveSplitManager.Status.Disconnected)
                LiveSplit.LiveSplitManager.Connect();
            else if (state && LiveSplit.ConnectionManager.IsConnected)
                LiveSplit.LiveSplitManager.Disconnect();
        }

        internal static void ToggleForceSyncTime(bool state)
        {
            if (state)
            {
                ConfigHandler.ForceSyncTime = !ConfigHandler.ForceSyncTime;
                DataHandler.Save();

                if (ConfigHandler.ForceSyncTime)
                    LiveSplit.ConnectionManager.StartSyncingToGame();
            }
        }

        internal static void ToggleFPSDisplay(bool state)
        {
            if (state)
            {
                UIHandler.Windows[UIHandler.WindowTypes.FPS].State = ConfigHandler.DisplayFPS = !ConfigHandler.DisplayFPS;
                DataHandler.Save();
            }
        }
    }
}
