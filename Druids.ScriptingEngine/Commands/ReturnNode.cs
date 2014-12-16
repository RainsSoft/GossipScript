using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Interfaces;
using Druids.ScriptingEngine;

namespace Druids.ScriptingEngine.Commands
{
    internal class ReturnNode : IActionNode
    {
        public ScriptResult Update(ScriptExecutionContext context)
        {
            return ScriptResult.Return;
        }

        public void OnEnter(ScriptExecutionContext context)
        {
            //    throw new NotImplementedException();
        }

        public void OnExit(ScriptExecutionContext context)
        {
            //  throw new NotImplementedException();
        }

        public IActionNode Next { get; set; }
        public string CommandName { get; private set; }
        public bool Recall { get; private set; }
    }
}