using System;
using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Interfaces;
using Druids.ScriptingEngine;

namespace Druids.ScriptingEngine.Commands
{
    internal class AssignStringLiteral : IActionNode
    {
        private readonly int m_DestIndex;
        private readonly VariableScope m_DestScope;
        private readonly int m_SourceIndex;
        private readonly VariableScope m_SourceScope;
        private readonly string m_SourceValue;

        public AssignStringLiteral(Int32 destinationDestIndex, VariableScope destinationDestScope, Int32 sourceIndex,
                                   VariableScope sourceScope)
        {
            m_DestIndex = destinationDestIndex;
            m_DestScope = destinationDestScope;
            m_SourceIndex = sourceIndex;
            m_SourceScope = sourceScope;
        }

        public AssignStringLiteral(Int32 destinationDestIndex, VariableScope destinationDestScope, String sourceValue)
        {
            m_DestIndex = destinationDestIndex;
            m_DestScope = destinationDestScope;
            m_SourceValue = sourceValue;
        }

        public ScriptResult Update(ScriptExecutionContext context)
        {
            return ScriptResult.Continue;
        }

        public void OnEnter(ScriptExecutionContext context)
        {
            string value = m_SourceValue;
            if (value == null)
            {
                if (m_SourceScope == VariableScope.Global)
                    value = context.Vm.GlobalState.Str[m_SourceIndex];

                if (m_SourceScope == VariableScope.Local)
                    value = context.LocalState.Str[m_SourceIndex];
            }

            if (m_DestScope == VariableScope.Global)
            {
                context.Vm.GlobalState.Str[m_DestIndex] = value;
            }
            else
            {
                context.LocalState.Str[m_DestIndex] = value;
            }
        }

        public void OnExit(ScriptExecutionContext context)
        {
            //throw new NotImplementedException();
        }

        public IActionNode Next { get; set; }
        public string CommandName { get; private set; }
        public bool Recall { get; private set; }
    }
}