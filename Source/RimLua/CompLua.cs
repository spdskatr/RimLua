using NLua;
using NLua.Exceptions;
using RimWorld;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimLua
{
    public class CompProperties_Lua : CompProperties
    {
        public CompProperties_Lua()
        {
            compClass = typeof(CompLua);
        }
        public List<string> additionalAssemblyReferences;
        public string defaultLuaScript;//This field can be a translation key or Lua code
    }
    public class CompLua : ThingComp
    {
        public const string StdoutString = "RimLua_stdout";
        int currentSignalInt;
        string currentChannelInt;
        LuaFunction tickFunction;
        public int CurrentSignal
        {
            get => currentSignalInt;
            set
            {
                var offset = value - currentSignalInt;
                if (CurrentChannel != null)
                {
                    IncrementCurrentChannelSignalValue(offset);
                }
                currentSignalInt = value;
            }
        }
        public string CurrentChannel
        {
            get => currentChannelInt;
            set
            {
                if (currentChannelInt != null)
                {
                    IncrementCurrentChannelSignalValue(-CurrentSignal);
                }
                currentChannelInt = value;
                if (currentChannelInt != null)
                {
                    IncrementCurrentChannelSignalValue(CurrentSignal);
                }
            }
        }
        public string Stdout
        {
            get
            {
                return (string)interpreter[StdoutString];
            }
        }
        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);
            CurrentChannel = null;
        }
        public string code;
        public CompProperties_Lua Props { get { return (CompProperties_Lua)props; } }
        public void IncrementCurrentChannelSignalValue(int amount)
        {
            parent.Map.GetComponent<MapComponent_SignalNet>().IncrementSignalValueBy(CurrentChannel, amount);
        }
        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            RefreshInterpreter();
            if (Props.defaultLuaScript != null)
            {
                if (Props.defaultLuaScript.CanTranslate())
                {
                    code = Props.defaultLuaScript.Translate();
                }
                code = Props.defaultLuaScript;
            }
            else
            {
                code = "LuaTerminalInitText".Translate();
            }
        }
        public virtual void RefreshInterpreter()
        {
            interpreter = LuaUtil.CreateInstance(Props.additionalAssemblyReferences);
            interpreter["parent"] = parent;
        }
        public Lua interpreter;
        public virtual void UpdateCode()
        {
            RefreshInterpreter();
            interpreter.DoStringErrorHandled(parent, "position = Location.PositionOfThing(parent)");
            interpreter["mapIndex"] = parent.Map.Index;
            interpreter.DoString(StdoutString + " = 'RimLua :: Running on '.._VERSION..'\\n'");
            interpreter.DoString(string.Format("function print(...) for k, v in ipairs({{...}}) do {0} = {0} .. v .. '\\t' end {0} = {0} .. '\\n' end", StdoutString));
            interpreter.DoStringErrorHandled(parent, code);
            PostUpdateCode();
        }
        public virtual void PostUpdateCode()
        {
            tickFunction = interpreter["tick"] as LuaFunction;
            if (tickFunction != null)
            {
                Log.Warning("Found tick function.");
            }
        }
        public override void CompTick()
        {
            base.CompTick();
            LuaUtil.LuaErrorHandledWrapper(() => tickFunction?.Call(), parent);
        }
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            UpdateCode();
        }
        public override void PostExposeData()
        {
            base.PostExposeData();
            if (Scribe.mode == LoadSaveMode.Saving && code != null)
            {
                code.Replace("<", "&lt;");
                code.Replace(">", "&gt;");
            }
            Scribe_Values.Look(ref code, "code");
        }
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (var g in base.CompGetGizmosExtra()) yield return g;
            for (int i = 1; i < 10; i++)
            {
                var str = "button_" + i;
                if (interpreter[str] != null && interpreter[str] is LuaFunction)
                {
                    KeyBindingDef hKey;
                    switch (i)
                    {
                        case 1:
                            hKey = KeyBindingDefOf.Misc1;
                            break;
                        case 2:
                            hKey = KeyBindingDefOf.Misc2;
                            break;
                        case 3:
                            hKey = KeyBindingDefOf.Misc3;
                            break;
                        case 4:
                            hKey = KeyBindingDefOf.Misc4;
                            break;
                        case 5:
                            hKey = KeyBindingDefOf.Misc5;
                            break;
                        case 6:
                            hKey = KeyBindingDefOf.Misc6;
                            break;
                        case 7:
                            hKey = KeyBindingDefOf.Misc7;
                            break;
                        case 8:
                            hKey = KeyBindingDefOf.Misc8;
                            break;
                        case 9:
                            hKey = KeyBindingDefOf.Misc9;
                            break;
                        default:
                            hKey = KeyBindingDefOf.Misc10;
                            break;
                    }
                    yield return new Command_Action()
                    {
                        action = () => interpreter.DoStringErrorHandled(parent, str + "()"),
                        defaultLabel = "LuaButton".Translate(i),
                        defaultDesc = "LuaButton_Desc".Translate(str),
                        icon = ContentFinder<Texture2D>.Get("Things/Mote/ColonistAttacking"),
                        hotKey = hKey
                    };
                }
            }
        }
    }
}
