using UnityEngine;

namespace HopSplit.UI
{
    internal class UIObject : MonoBehaviour
    {
        private static float prevTime;
        internal static float FPS;

        internal void Start()
        {
            UIHandler.Windows[UIHandler.WindowTypes.FPS].State = ConfigHandler.DisplayFPS;
        }

        internal void Update()
        {
            prevTime += (Time.unscaledDeltaTime - prevTime) * 0.1f;
            FPS = 1f / prevTime;

            if (Input.GetKeyDown(KeyCode.F1))
                UIHandler.Windows[UIHandler.WindowTypes.Main].State = !UIHandler.Windows[UIHandler.WindowTypes.Main].State;
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
