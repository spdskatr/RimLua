using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using UnityEngine;
using OtherIntVec = Verse.IntVec3;

namespace RimLua.Terminal
{
    /*
     * IDEAS BANK
     * Place designators/plans
     * Terminal log
     * Prompting
     */
    public static class General
    {
        public static int TicksGame => Find.TickManager.TicksGame;

        public static int TicksAbs => Find.TickManager.TicksAbs;

        //Messaging
        public static void Message(object message)
        {
            Messages.Message(message.ToString(), MessageSound.Standard);
        }
        public static void MessageGood(object message)
        {
            Messages.Message(message.ToString(), MessageSound.Benefit);
        }
        public static void MessageBad(object message)
        {
            Messages.Message(message.ToString(), MessageSound.SeriousAlert);
        }
        public static void MessageSilent(object message)
        {
            Messages.Message(message.ToString(), MessageSound.Silent);
        }
        public static void Letter(object label, object message)
        {
            Find.LetterStack.ReceiveLetter(label.ToString(), message.ToString(), LetterDefOf.Good);
        }
        public static void LetterBad(object label, object message)
        {
            Find.LetterStack.ReceiveLetter(label.ToString(), message.ToString(), LetterDefOf.BadNonUrgent);
        }
        public static void LetterVeryBad(object label, object message)
        {
            Find.LetterStack.ReceiveLetter(label.ToString(), message.ToString(), LetterDefOf.BadUrgent);
        }
        public static void DebugMessage(object message)
        {
            Log.Message(message.ToString());
        }
        public static void DebugWarning(object message)
        {
            Log.Warning(message.ToString());
        }
        public static void DebugError(object error)
        {
            Log.Error(error.ToString());
        }
        public static void DialogBox(object text)
        {
            Find.WindowStack.Add(new Dialog_DisplayText(text.ToString()));
        }
        public static void ThrowText(IntVec3 loc, int mapIndex, object text, float timeBeforeStartFadeout)
        {
            MoteMaker.ThrowText(loc.ToVector3ShiftedWithAltitude(AltitudeLayer.MoteOverhead), Find.Maps[mapIndex], text.ToString(), timeBeforeStartFadeout);
        }
    }
    public static class Interaction
    {
        public static void SetBroadcastChannel(string channel, Thing parent)
        {
            parent.TryGetComp<CompLua>().CurrentChannel = channel;
        }
        public static void BroadcastSignal(int signal, Thing parent)
        {
            parent.TryGetComp<CompLua>().CurrentSignal = signal;
        }
        public static int SignalAt(string channel, int mapIndex)
        {
            return Find.Maps[mapIndex].GetComponent<MapComponent_SignalNet>().GetSignalValueFor(channel);
        }
        public static void TriggerGizmo(Thing thing, int gizmoIndex)
        {
            thing.GetGizmos()?.ElementAtOrDefault(gizmoIndex)?.ProcessInput(Event.current);
        }
        public static void ToggleFlick(Thing thing)
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
    public static class Location
    {
        public static IntVec3 PositionOfThing(Thing thing)
        {
            return thing.Position;
        }
        public static int GetMapIndex(Thing thing)
        {
            return thing.Map.Index;
        }
    }
    public static class Proximity
    {
        public static Building GetFirstBuilding(IntVec3 loc, int mapIndex)
        {
            return GridsUtility.GetFirstBuilding(loc, Find.Maps[mapIndex]);
        }
        public static Thing GetFirstItem(IntVec3 loc, int mapIndex)
        {
            return GridsUtility.GetFirstItem(loc, Find.Maps[mapIndex]);
        }
        public static Pawn GetFirstPawn(IntVec3 loc, int mapIndex)
        {
            return GridsUtility.GetFirstPawn(loc, Find.Maps[mapIndex]);
        }
        public static Pawn PawnNearby(int radius, Thing parent, Func<Pawn, bool> validator = null)
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
        public static Pawn PawnNearbyNonHuman(int radius, Thing parent) => PawnNearby(radius, parent, p => !p.RaceProps.Humanlike);
        public static Pawn PawnNearbyHuman(int radius, Thing parent) => PawnNearby(radius, parent, p => p.RaceProps.Humanlike);
        public static Pawn PawnNearbyEnemy(int radius, Thing parent) => PawnNearby(radius, parent, p => p.Faction.HostileTo(Faction.OfPlayer));
        public static Pawn PawnNearbyHumanEnemy(int radius, Thing parent) => PawnNearby(radius, parent, p => p.RaceProps.Humanlike && p.Faction.HostileTo(Faction.OfPlayer));
        public static Pawn PawnNearbyHumanAlly(int radius, Thing parent) => PawnNearby(radius, parent, p => p.RaceProps.Humanlike && !p.Faction.HostileTo(Faction.OfPlayer));
        public static Pawn PawnNearbyColonyNonHuman(int radius, Thing parent) => PawnNearby(radius, parent, p => !p.RaceProps.Humanlike && p.Faction == Faction.OfPlayer);
    }
    public static class Saving
    {
    }
}
