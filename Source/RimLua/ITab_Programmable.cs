using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimLua
{
    public class ITab_Programmable : ITab
    {
        public string textInTerminal;
        [Unsaved] string selThingReference;
        public static readonly Vector2 WinSize = new Vector2(600f, 400f);
        public ITab_Programmable()
        {
            size = WinSize;
            labelKey = "LuaConfig";
        }
        public override bool IsVisible => base.IsVisible;
        public void RefreshTextInTerminal()
        {
            textInTerminal = SelThing.TryGetComp<CompLua>().code;
        }
        protected override void FillTab()
        {
            if (SelThing.GetUniqueLoadID() != selThingReference)
            {
                RefreshTextInTerminal();
                selThingReference = SelThing.GetUniqueLoadID();
            }
            var inRect = new Rect(0f, 0f, WinSize.x, WinSize.y).ContractedBy(20f);//inner rectangle
            var contentRect = new Rect(inRect);
            contentRect.yMax -= 20f;
            var style = new GUIStyle(Text.CurTextFieldStyle) //Construct our GUIStyle
            {
                alignment = TextAnchor.UpperLeft,
                wordWrap = true
            };
            if (Resources.CourierNew) //Implicit conversion to bool
            {
                style.font = Resources.CourierNew;
            }
            textInTerminal = GUI.TextArea(contentRect, textInTerminal, style);
            var buttonsRect = inRect.BottomPartPixels(18f);
            var margin = 5f;
            var width = (inRect.width - margin) / 2f;
            var leftRect = buttonsRect.LeftPartPixels(width);
            var rightRect = buttonsRect.RightPartPixels(width);
            var save = Widgets.ButtonText(leftRect, "LuaSave".Translate());
            var help = Widgets.ButtonText(rightRect, "LuaHelp".Translate());//TODO: Implement help
            if (save)
            {
                Save();
            }
            if (help)
            {
                Find.WindowStack.Add(new Dialog_LuaHelp());
            }
        }
        public void Save()
        {
            CompLua luaComp = SelThing.TryGetComp<CompLua>();
            luaComp.code = textInTerminal;
            luaComp.UpdateCode();
            Messages.Message("LuaSaveSuccessful".Translate(), MessageSound.Standard);
        }
    }
    public class ITab_Out : ITab
    {
        Vector2 scrollbarPosition;
        string selThingId;
        public ITab_Out()
        {
            size = new Vector3(600f, 400f);
            labelKey = "LuaOutput";
        }
        protected override void FillTab()
        {
            var rect = new Rect(0f, 0f, 300f, 400f).ContractedBy(20f);
            if (SelThing.GetUniqueLoadID() != selThingId)
            {
                scrollbarPosition = Vector2.zero;
                selThingId = SelThing.GetUniqueLoadID();
            }
            Widgets.TextAreaScrollable(rect, SelThing.TryGetComp<CompLua>().Stdout, ref scrollbarPosition, true);
        }
    }
}
