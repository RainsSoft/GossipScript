using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Interfaces;
using Druids.ScriptingEngine;

namespace Druids.ScriptingEngine.Commands
{
    internal class ExitNode : IActionNode
    {
        public ScriptResult Update(ScriptExecutionContext context)
        {
            return ScriptResult.EndProcess;
        }

        public void OnEnter(ScriptExecutionContext context)
        {
            // NOOP
        }

        public void OnExit(ScriptExecutionContext context)
        {
            // NOOP
        }

        public IActionNode Next { get; set; }
        public string CommandName { get; private set; }
        public bool Recall { get; private set; }
    }
}