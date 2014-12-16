using System;
using Druids.ScriptingEngine;

namespace Druids.ScriptingEngine
{
    public class ScriptExecutionContext
    {
        private readonly CompiledScript m_Script;
        private readonly LocalState m_LocalState;

        public ScriptExecutionContext(CompiledScript script, LocalState localState)
        {
            m_Script = script;
            m_LocalState = localState;
        }

        public Int32 DeltaMs { get; set; }

        public GossipVM Vm { get; set; }

        public ScriptEvent CurrentScriptEvent { get; set; }

        public LocalState LocalState
        {
            get { return m_LocalState; }
        }

        public CompiledScript Script
        {
            get { return m_Script; }
        }
    }
}