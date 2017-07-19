using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimLua
{
    public class Dialog_DisplayText : Window
    {
        public Vector2 scrollPos;
        public Dialog_DisplayText() : base()
        {
            doCloseX = true;
            closeOnClickedOutside = true;
            closeOnEscapeKey = true;
            forcePause = true;
            draggable = true;
        }
        public Dialog_DisplayText(string s) : this()
        {
            textToDisplay = s;
        }
        public string textToDisplay = "No text to display.";
        public override Vector2 InitialSize => new Vector2(500, 600);
        public override void DoWindowContents(Rect inRect)
        {
            Rect contentRect = inRect.ContractedBy(10f);
            Resources.TextAreaScrollableRichText(contentRect, textToDisplay, ref scrollPos, true);
        }
    }
}
