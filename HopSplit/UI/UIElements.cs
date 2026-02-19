using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using static HopSplit.UI.UIHandler;

namespace HopSplit.UI
{
    internal static class UIElements
    {
        private const float _elementMargin      = 2;
        private const float _elementHeight      = 18;
        private const float _elementTextSize    = 16;

        internal static int ElementMargin   => (int)(_elementMargin     * RectData.Scale.y);
        internal static int ElementHeight   => (int)(_elementHeight     * RectData.Scale.y * 1.2f);
        internal static int ElementTextSize => (int)(_elementTextSize   * RectData.Scale.y);

        internal readonly static Dictionary<int, ElementData> Elements = new Dictionary<int, ElementData>();

        internal static void Label(int id, ref int elementID, string text, TextAnchor textAnchor, Color? color = null)
        {
            var element = BuildElement(id, elementID++, StyleTypes.Label, null, color, false, text, textAnchor, ElementTextSize);
            element.Text = text;

            GUI.Label(element.RectData.Rect, element.Text, element.GetStyle());
        }

        internal static bool Checkbox(int id, ref int elementID, bool state, string text, TextAnchor textAnchor, Color? color = null)
        {
            var element = BuildElement(id, elementID++, StyleTypes.Checkbox, color ?? UIColors.ElementBackgroundOn, Color.white, true, text, textAnchor, ElementTextSize);
            element.Text = text;

            return GUI.Toggle(element.RectData.Rect, state, element.Text, element.GetStyle(!state, UIColors.ElementBackgroundOff));
        }

        internal static bool Button(int id, ref int elementID, string text, TextAnchor textAnchor, Color? color = null, bool toggleState = false)
        {
            var element = BuildElement(id, elementID++, StyleTypes.Button, color ?? UIColors.ElementBackgroundOn, Color.white, true, text, textAnchor, ElementTextSize);
            element.Text = text;

            return GUI.Button(element.RectData.Rect, element.Text, element.GetStyle(!toggleState, UIColors.ElementBackgroundOff));
        }

        internal static void EmptySpace(int id, ref int elementID)
        {
            BuildElement(id, elementID++, StyleTypes.Box);
        }

        private static ElementData BuildElement(int id, int elementID, StyleTypes styleType, Color? background = null, Color? foreground = null, bool dynamicColor = true, string text = null, TextAnchor textAnchor = TextAnchor.MiddleCenter, int textSize = 12)
        {
            if (Elements.TryGetValue(elementID, out var value))
                return value;

            var windowRect = Windows[(WindowTypes)id].ElementData.RectData.Rect;

            int x = ElementMargin;
            int y = (Elements.Count * ElementHeight) + (Elements.Count * ElementMargin) + ElementMargin;

            RectData    rect    = new RectData(new Vector2Int(x, y), new Vector2Int((int)windowRect.size.x - (ElementMargin * 2), ElementHeight));
            ElementData element = new ElementData(StyleTypes.Label, background ?? Color.clear, foreground ?? Color.white, dynamicColor, rect, text, textAnchor, textSize);

            Elements.Add(elementID, element);

            Windows[(WindowTypes)id].ElementData.RectData.Rect.height = (Elements.Count * ElementHeight) + (Elements.Count * ElementMargin) + ElementMargin;

            return element;
        }
    }
}
