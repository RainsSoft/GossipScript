using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Interfaces;
using Druids.ScriptingEngine;

namespace Druids.ScriptingEngine.Commands
{
    internal class ActionNode : IActionNode
    {
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

        public bool Recall
        {
            get { return false; }
        }
    }
}