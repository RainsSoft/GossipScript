using System;
using Druids.ScriptingEngine.Enums;

namespace Druids.ScriptingEngine
{
    public class QueuedScriptletEvent
    {
        private readonly Func<ScriptExecutionContext, ScriptResult> m_Func;
        private readonly ScriptEvent m_ScriptEvent;
        // private readonly StringTable m_StringTable;

        public QueuedScriptletEvent(Func<ScriptExecutionContext, ScriptResult> func, ScriptEvent scriptEvent)
            //, StringTable stringTable)
        {
            m_Func = func;
            m_ScriptEvent = scriptEvent;
            // m_StringTable = stringTable;
        }

        public Func<ScriptExecutionContext, ScriptResult> Func
        {
            get { return m_Func; }
        }

        //public StringTable StringTable
        //{
        //    get { return m_StringTable; }
        //}

        public ScriptEvent ScriptEvent
        {
            get { return m_ScriptEvent; }
        }
    }
}