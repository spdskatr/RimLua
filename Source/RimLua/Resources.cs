using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using UnityEngine;

namespace RimLua
{
    [StaticConstructorOnStartup]
    public static class Resources
    {
        static Resources()
        {
            CourierNew = Font.CreateDynamicFontFromOSFont("Courier New", 12);
        }
        public static readonly Font CourierNew;
        public static string TextAreaScrollableRichText(Rect rect, string text, ref Vector2 scrollbarPosition, bool readOnly = false)
        {
            Rect rect2 = new Rect(0f, 0f, rect.width - 16f, Mathf.Max(Text.CalcHeight(text, rect.width) + 10f, rect.height));
            Widgets.BeginScrollView(rect, ref scrollbarPosition, rect2, true);
            string result = TextAreaRichText(rect2, text, readOnly);
            Widgets.EndScrollView();
            return result;
        }
        public static string TextAreaRichText(Rect rect, string text, bool readOnly = false)
        {
            if (text == null)
            {
                text = string.Empty;
            }
            GUIStyle style = new GUIStyle((!readOnly) ? Text.CurTextAreaStyle : Text.CurTextAreaReadOnlyStyle)
            {
                richText = true
            };
            return GUI.TextArea(rect, text, style);
        }
    }
}
