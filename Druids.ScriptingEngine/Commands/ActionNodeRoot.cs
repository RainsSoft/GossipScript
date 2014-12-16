using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Interfaces;
using Druids.ScriptingEngine;
using Druids.ScriptingEngine.Host;

namespace Druids.ScriptingEngine.Commands
{
    public class ActionNodeRoot : IActionNode
    {
        private readonly StringTable m_StringTable = new StringTable();

        public StringTable StringTable
        {
            get { return m_StringTable; }
        }

        public ScriptResult Update(ScriptExecutionContext context)
        {
            return ScriptResult.Continue;
        }

        public void OnEnter(ScriptExecutionContext context)
        {
        }

        public void OnExit(ScriptExecutionContext context)
        {
        }

        public IActionNode Next { get; set; }
        public string CommandName { get; private set; }
        public bool Recall { get; private set; }
    }
}