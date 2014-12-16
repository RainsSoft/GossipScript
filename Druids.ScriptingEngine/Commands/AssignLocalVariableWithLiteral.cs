using System;
using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Interfaces;
using Druids.ScriptingEngine;

namespace Druids.ScriptingEngine.Commands
{
    public class AssignLocalVariableWithLiteral : IActionNode
    {
        private readonly int m_Index;
        private readonly double m_Value;

        public AssignLocalVariableWithLiteral(Int32 index, Double value)
        {
            m_Index = index;
            m_Value = value;
        }

        public ScriptResult Update(ScriptExecutionContext context)
        {
            return ScriptResult.Continue;
        }

        public void OnEnter(ScriptExecutionContext context)
        {
            context.LocalState.Var[m_Index] = m_Value;
        }

        public void OnExit(ScriptExecutionContext context)
        {
            // throw new NotImplementedException();
        }

        public IActionNode Next { get; set; }
        public string CommandName { get; private set; }
        public bool Recall { get; private set; }
    }
}