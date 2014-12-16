using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Interfaces;
using Druids.ScriptingEngine;

namespace Druids.ScriptingEngine.Commands
{
    internal class YeildNode : IActionNode
    {
        public ScriptResult Update(ScriptExecutionContext context)
        {
            context.CurrentScriptEvent.CurrentExecutingActionNode = Next;
            return ScriptResult.Yield;
        }

        public void OnEnter(ScriptExecutionContext context)
        {
            // Noop
        }

        public void OnExit(ScriptExecutionContext context)
        {
            // Noop
        }

        public IActionNode Next { get; set; }

        public string CommandName { get; private set; }

        public bool Recall { get; private set; }
    }
}