using UnityEngine;

namespace HopSplit.UI
{
    internal class UIObject : MonoBehaviour
    {
        internal void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                UIHandler.Windows[UIHandler.WindowTypes.Main].State = !UIHandler.Windows[0].State;
                Debug.LogError(UIHandler.Windows[UIHandler.WindowTypes.Main].State);
            }
        }

        internal void OnGUI()
        {
            foreach (var window in UIHandler.Windows)
                if (window.Value.State)
                    UnityEngine.GUI.Window(window.Value.ID, window.Value.ElementData.RectData.Rect, window.Value.Function, string.Empty, window.Value.ElementData.GetStyle());
        }

        internal void OnDestroy()
        {
            UIHandler.CreateUIObject();
        }

        public void OnApplicationQuit()
        {
            LiveSplit.ConnectionManager.StartPausingTimer();
        }
    }
}
