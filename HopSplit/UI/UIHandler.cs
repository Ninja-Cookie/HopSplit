using System.Collections.Generic;
using UnityEngine;

namespace HopSplit.UI
{
    internal static class UIHandler
    {
        internal enum StyleTypes
        {
            Box,
            Button,
            Label,
            Checkbox
        }

        internal class RectData
        {
            internal    static Vector2 BaseResolution   =   new Vector2(1920f, 1080f);
            internal    static Vector2 Scale            =>  new Vector2(SingletonPropertyItem<SettingsManager>.Instance?.Resolution.width ?? Screen.width, SingletonPropertyItem<SettingsManager>.Instance?.Resolution.height ?? Screen.height) / BaseResolution;

            internal Vector2Int Position    { get; }
            internal Vector2Int Size        { get; }
            internal Rect       Rect;

            internal RectData(Vector2Int position, Vector2Int size, bool autoScale = false)
            {
                this.Position   = position;
                this.Size       = size;
                RebuildRect(autoScale);
            }

            internal void RebuildRect(bool autoScale = true)
            {
                var res = new Vector2((float)Screen.width, (float)Screen.height);

                this.Rect = new Rect
                (
                    autoScale ? Vector2Int.RoundToInt((Vector2)this.Position    * Scale) : this.Position,
                    autoScale ? Vector2Int.RoundToInt((Vector2)this.Size        * Scale) : this.Size
                );
            }
        }

        internal class ElementData
        {
            internal StyleTypes StyleType       { get; }
            internal Color      Background      { get; }
            internal Color      Foreground      { get; }
            internal bool       DynamicColor    { get; }
            internal RectData   RectData        { get; set; }
            internal string     Text            { get; set; }
            internal TextAnchor TextAnchor      { get; }
            internal int        TextSize        { get; }

            private  GUIStyle   GUIStyle        { get; set; }
            private  GUIStyle   GUIStyleAlt     { get; set; }

            internal ElementData(StyleTypes styleType, Color? background = null, Color? foreground = null, bool dynamicColor = true, RectData rectData = null, string text = null, TextAnchor textAnchor = TextAnchor.MiddleCenter, int textSize = 12)
            {
                this.StyleType      = styleType;
                this.Background     = background ?? Color.clear;
                this.Foreground     = foreground ?? Color.clear;
                this.DynamicColor   = dynamicColor;
                this.RectData       = rectData;
                this.Text           = text ?? string.Empty;
                this.TextAnchor     = textAnchor;
                this.TextSize       = textSize;
            }

            internal GUIStyle GetStyle(bool alt = false, Color? background = null)
            {
                if (alt && background != null && this.GUIStyleAlt?.normal?.background == null)
                    return GUIStyleAlt = GenerateStyle(this.StyleType, (Color)background, this.Foreground, this.TextAnchor, this.TextSize, this.DynamicColor);
                else if (alt && this.GUIStyleAlt?.normal?.background != null)
                    return this.GUIStyleAlt;

                if (this.GUIStyle?.normal?.background == null)
                    return this.GUIStyle = GenerateStyle(this.StyleType, this.Background, this.Foreground, this.TextAnchor, this.TextSize, this.DynamicColor);
                return this.GUIStyle;
            }
        }

        internal class WindowData
        {
            internal int                ID          { get; }
            internal GUI.WindowFunction Function    { get; }
            internal ElementData        ElementData { get; }
            internal bool               State       { get; set; } = false;

            internal WindowData(int id, GUI.WindowFunction function, ElementData elementData, bool state = false)
            {
                this.ID             = id;
                this.Function       = function;
                this.ElementData    = elementData;
                this.State          = state;
            }
        }

        internal enum WindowTypes
        {
            Main = 0,
            FPS = 1
        }

        internal readonly static Dictionary<WindowTypes, WindowData> Windows = new Dictionary<WindowTypes, WindowData>()
        {
            { WindowTypes.Main, new WindowData((int)WindowTypes.Main,   UIWindows.Main, new ElementData(StyleTypes.Box, UIColors.WindowBackground, null, false, new RectData(new Vector2Int(5, 5), new Vector2Int(300, 300), true))) },
            { WindowTypes.FPS,  new WindowData((int)WindowTypes.FPS,    UIWindows.FPS,  new ElementData(StyleTypes.Box, null, null, false, new RectData(Vector2Int.zero, Vector2Int.RoundToInt(RectData.BaseResolution), true)), true) }
        };

        private static Texture2D GenerateTexture(Color color, float alpha = 1f)
        {
            color = new Color(color.r, color.g, color.b, alpha);

            var texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();

            return texture;
        }

        private static GUIStyle GenerateStyle(StyleTypes styleType, Color background, Color foreground, TextAnchor textAnchor, int textSize, bool dynamic)
        {
            GUIStyle type = GUIStyle.none;
            switch (styleType)
            {
                case StyleTypes.Box:        type = GUI.skin.box;    break;
                case StyleTypes.Button:     type = GUI.skin.button; break;
                case StyleTypes.Label:      type = GUI.skin.label;  break;
                case StyleTypes.Checkbox:   type = GUI.skin.toggle; break;
            }

            var style   = new GUIStyle(type);
            var texture = GenerateTexture(background, background.a);

            style.normal    .background = texture;
            style.hover     .background = dynamic ? GenerateTexture(background * 0.9f, background.a) : texture;
            style.active    .background = dynamic ? GenerateTexture(background * 0.8f, background.a) : texture;

            style.normal    .textColor  = foreground;
            style.hover     .textColor  = foreground;
            style.active    .textColor  = foreground;

            style.alignment = textAnchor;
            style.fontSize  = textSize;
            style.fontStyle = FontStyle.Bold;

            style.wordWrap = false;

            return style;
        }

        internal static void CreateUIObject()
        {
            GameObject.DontDestroyOnLoad(new GameObject(Plugin.pluginName, typeof(UIObject)));
        }

        internal static void Rebuild(PropertyItem propertyItem = null)
        {
            foreach (var window in Windows)
                window.Value.ElementData.RectData.RebuildRect(true);
            UIElements.Elements.Clear();
        }
    }
}
