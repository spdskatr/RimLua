using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimLua
{
    /// <summary>
    /// A signal net is a map-wide net of string channels, each channel having an integer value.
    /// </summary>
    public class MapComponent_SignalNet : MapComponent
    {
        public MapComponent_SignalNet(Map map) : base(map) { }
        public Dictionary<string, int> signalNet = new Dictionary<string, int>();
        public int GetSignalValueFor(string channel)
        {
            if (signalNet.TryGetValue(channel, out int signal)) return signal;
            return 0;
        }
        public void IncrementSignalValueBy(string signal, int val)
        {
            if (!signalNet.ContainsKey(signal))
            {
                signalNet.Add(signal, val);
            }
            signalNet[signal] += val;
        }
    }
}
