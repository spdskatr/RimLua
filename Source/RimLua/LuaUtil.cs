using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLua;
using NLua.Exceptions;
using Verse;
using RimWorld;
using RimWorld.Planet;

namespace RimLua
{
    public static class LuaUtil
    {
        public static int lastLetterReceivedTick = -1;
        /// <summary>
        /// Creates a new instance of the Lua 5.1 interpreter. Credits go to NLua for interpreter
        /// </summary>
        public static Lua CreateInstance(IEnumerable<string> additionalAssemblies = null)
        {
            var lua = new Lua();
            lua.LoadCLRPackage();
            lua.DoString("import ('RimLua', 'RimLua.Terminal')");
            if (additionalAssemblies != null)
            {
                foreach (string assembly in additionalAssemblies)
                {
                    try
                    {
                        if (assembly.Length > 0)
                            lua.DoString(string.Format("import '{0}'", assembly));
                    }
                    catch (Exception ex)
                    {
                        Log.Error(string.Format("Project RimFactory::RimLua :: Could not load assembly {0}. Error: " + ex, assembly));
                    }
                }
            }
            lua.UndefineCLRPackage();
            return lua;
        }
        public static void UndefineCLRPackage(this Lua lua)
        {
            lua.DoString("mt, set_global_mt, CLRPackage, import, luanet = nil;");
        }
        public static object[] DoStringErrorHandled(this Lua lua, GlobalTargetInfo lookTarget, string chunk, string chunkName = "chunk")
        {
            object[] result = new object[0];
            LuaErrorHandledWrapper(() => result = lua.DoString(chunk, chunkName), lookTarget);
            return result;
        }
        public static void LuaErrorHandledWrapper(Action handledAction, GlobalTargetInfo lookTarget)
        {
            try
            {
                handledAction();
            }
            catch (LuaScriptException ex)
            {
                var label = "LuaError".Translate();
        var abs = Find.TickManager.TicksAbs;
                if (lastLetterReceivedTick + 240 < abs  //So it doesnt spam the letterstack
                    || lastLetterReceivedTick > abs) //So when the game reloads with a different ticksabs it can get back up again
                {
                    Find.LetterStack.ReceiveLetter(label, "LuaErrorContent".Translate(lookTarget.Cell, ex.Message), LetterDefOf.BadNonUrgent, lookTarget);
                    lastLetterReceivedTick = abs;
                }
            }
        }
    }
}
