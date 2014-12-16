using System;
using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Interfaces;
using Druids.ScriptingEngine;

namespace Druids.ScriptingEngine.Commands
{
    internal class WaitNode : IActionNode
    {
        private int m_Milliseconds;

        public WaitNode()
        {
            CommandName = "Wait";
        }

        public WaitNode(Int32 milliseconds)
        {
            m_Milliseconds = milliseconds;
        }

        public ScriptResult Update(ScriptExecutionContext context)
        {
            if (m_Milliseconds <= 0)
                return ScriptResult.Continue;

            m_Milliseconds -= context.DeltaMs;
            return ScriptResult.Yield;
        }

        public void OnEnter(ScriptExecutionContext context)
        {
            // No op
        }

        public void OnExit(ScriptExecutionContext context)
        {
            // No op
        }

        public IActionNode Next { get; set; }

        public string CommandName { get; private set; }

        public bool Recall
        {
            get { return false; }
        }
    }
}