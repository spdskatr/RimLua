using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using UnityEngine;
using OtherIntVec = Verse.IntVec3;
using RimWorld.Planet;

namespace RimLua.Terminal
{
    public class TerminalModule
    {
        public TerminalModule(CompLua comp)
        {
            Parent = comp.parent;
        }
        public static ThingWithComps Parent;
    }
    /*
     * IDEAS BANK
     * Place designators/plans
     * Terminal log
     * Prompting
     */
    public class General : TerminalModule
    {
        public General(CompLua comp) : base(comp)
        {
        }

        public int TicksGame => Find.TickManager.TicksGame;

        public int TicksAbs => Find.TickManager.TicksAbs;

        //Messaging
        public void Message(object message)
        {
            Messages.Message(message.ToString(), MessageSound.Standard);
        }
        public void MessageGood(object message)
        {
            Messages.Message(message.ToString(), MessageSound.Benefit);
        }
        public void MessageBad(object message)
        {
            Messages.Message(message.ToString(), MessageSound.SeriousAlert);
        }
        public void MessageSilent(object message)
        {
            Messages.Message(message.ToString(), MessageSound.Silent);
        }
        public void Letter(object label, object message)
        {
            Find.LetterStack.ReceiveLetter(label.ToString(), message.ToString(), LetterDefOf.Good);
        }
        public void LetterBad(object label, object message)
        {
            Find.LetterStack.ReceiveLetter(label.ToString(), message.ToString(), LetterDefOf.BadNonUrgent);
        }
        public void LetterVeryBad(object label, object message)
        {
            Find.LetterStack.ReceiveLetter(label.ToString(), message.ToString(), LetterDefOf.BadUrgent);
        }
        public void LetterCustom(object label, object message, string letterdef)
        {
            var letDef = DefDatabase<LetterDef>.GetNamedSilentFail(letterdef);
            if (letDef == null) LuaUtil.ThrowLuaScriptError(Parent, "invalid letter def");
        }
        public void DebugMessage(object message)
        {
            Log.Message(message.ToString());
        }
        public void DebugWarning(object message)
        {
            Log.Warning(message.ToString());
        }
        public void DebugError(object error)
        {
            Log.Error(error.ToString());
        }
        public void DialogBox(object text)
        {
            Find.WindowStack.Add(new Dialog_DisplayText(text.ToString()));
        }
        public void ThrowText(IntVec3 loc, int mapIndex, object text, float timeBeforeStartFadeout)
        {
            MoteMaker.ThrowText(loc.ToVector3ShiftedWithAltitude(AltitudeLayer.MoteOverhead), Find.Maps[mapIndex], text.ToString(), timeBeforeStartFadeout);
        }
    }
    public class Interaction : TerminalModule
    {
        public Interaction(CompLua comp) : base(comp)
        {
        }

        public void SetBroadcastChannel(string channel, Thing parent)
        {
            parent.TryGetComp<CompLua>().CurrentChannel = channel;
        }
        public void BroadcastSignal(int signal, Thing parent)
        {
            parent.TryGetComp<CompLua>().CurrentSignal = signal;
        }
        public int SignalAt(string channel, int mapIndex)
        {
            return Find.Maps[mapIndex].GetComponent<MapComponent_SignalNet>().GetSignalValueFor(channel);
        }
        public void TriggerGizmo(Thing thing, int gizmoIndex)
        {
            thing.GetGizmos()?.ElementAtOrDefault(gizmoIndex)?.ProcessInput(Event.current);
        }
        public void ToggleFlick(Thing thing)
        {
            CompFlickable flickableComp;
            if ((flickableComp = thing.TryGetComp<CompFlickable>()) != null)
            {
                if (!flickableComp.WantsFlick())
                {
                    typeof(CompFlickable).GetField("wantSwitchOn").SetValue(flickableComp, !flickableComp.SwitchIsOn);
                    FlickUtility.UpdateFlickDesignation(thing);
                }
            }
        }
    }
    public class Location : TerminalModule
    {
        public Location(CompLua comp) : base(comp)
        {
        }

        public IntVec3 PositionOfThing(Thing thing)
        {
            return thing.Position;
        }
        public int GetMapIndex(Thing thing)
        {
            return thing.Map.Index;
        }
    }
    public class Proximity : TerminalModule
    {
        public Proximity(CompLua comp) : base(comp)
        {
        }

        public Building GetFirstBuilding(IntVec3 loc, int mapIndex)
        {
            return GridsUtility.GetFirstBuilding(loc, Find.Maps[mapIndex]);
        }
        public Thing GetFirstItem(IntVec3 loc, int mapIndex)
        {
            return GridsUtility.GetFirstItem(loc, Find.Maps[mapIndex]);
        }
        public Pawn GetFirstPawn(IntVec3 loc, int mapIndex)
        {
            return GridsUtility.GetFirstPawn(loc, Find.Maps[mapIndex]);
        }
        public Pawn PawnNearby(int radius, Thing parent, Func<Pawn, bool> validator = null)
        {
            if (radius < 0)
            {
                return null;
            }
            if (radius == 0)
            {
                return parent.Position.GetFirstPawn(parent.Map);
            }
            var pos = parent.Position;
            var locs = GenRadial.RadialCellsAround(parent.Position, radius, true);
            foreach (var loc in locs)
            {
                Pawn p;
                if ((p = loc.GetFirstPawn(parent.Map)) != null && validator(p))
                    return p;
            }
            return null;
        }
        public Pawn PawnNearbyNonHuman(int radius, Thing parent) => PawnNearby(radius, parent, p => !p.RaceProps.Humanlike);
        public Pawn PawnNearbyHuman(int radius, Thing parent) => PawnNearby(radius, parent, p => p.RaceProps.Humanlike);
        public Pawn PawnNearbyEnemy(int radius, Thing parent) => PawnNearby(radius, parent, p => p.Faction.HostileTo(Faction.OfPlayer));
        public Pawn PawnNearbyHumanEnemy(int radius, Thing parent) => PawnNearby(radius, parent, p => p.RaceProps.Humanlike && p.Faction.HostileTo(Faction.OfPlayer));
        public Pawn PawnNearbyHumanAlly(int radius, Thing parent) => PawnNearby(radius, parent, p => p.RaceProps.Humanlike && !p.Faction.HostileTo(Faction.OfPlayer));
        public Pawn PawnNearbyColonyNonHuman(int radius, Thing parent) => PawnNearby(radius, parent, p => !p.RaceProps.Humanlike && p.Faction == Faction.OfPlayer);
    }
    public class Saving : TerminalModule
    {
        public Saving(CompLua comp) : base(comp)
        {
        }
    }
}
