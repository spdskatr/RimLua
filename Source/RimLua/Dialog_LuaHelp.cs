using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;
using NLua;

namespace RimLua
{
    public class Dialog_LuaHelp : Dialog_DisplayText
    {
        public override Vector2 InitialSize => new Vector2(1000, 600);
        public Dialog_LuaHelp() : base()
        {
            textToDisplay = "LuaHelpText".Translate();
        }
        public override void DoWindowContents(Rect inRect)
        {
            var topSection = inRect.BottomPartPixels(30f);
            inRect.yMax -= 30f;
            var topSectionInnerRect = topSection.ContractedBy(10f);
            topSectionInnerRect.yMin -= 10f;
            if (Widgets.ButtonText(topSectionInnerRect, "LuaWebsite".Translate(), false))
                Application.OpenURL("https://www.lua.org/");
            base.DoWindowContents(inRect);
        }
    }
}
