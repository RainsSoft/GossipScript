using System;

namespace Druids.ScriptingEngine.Host
{
    public class GossipActionParameter
    {
        private int m_ReadHead;

        public CompiledScript Script;

        public Int32 DeltaMs;
        public Object Owner;

        public Object[] Parameters; // These could be expressions...

        public GossipActionParameter()
        {
    
        }

        [Obsolete]  // Remove when moving to new dialog system
        public String Peek; 


        // TODO Need a readIntParam() readStringParam() etc.. ?
        public Int32 ReadInt()
        {
            var rv = (Int32) ((double) Parameters[m_ReadHead]);
            m_ReadHead++;
            return rv;
        }
    }
}