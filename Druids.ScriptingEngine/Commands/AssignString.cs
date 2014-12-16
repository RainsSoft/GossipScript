using System;
using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Exceptions;
using Druids.ScriptingEngine.Interfaces;
using Druids.ScriptingEngine;
using Druids.ScriptingEngine.Exceptions;

namespace Druids.ScriptingEngine.Commands
{
    public class AssignString : IActionNode
    {
        private readonly int m_DestIndex;
        private readonly VariableScope m_DestScope;
        private readonly Expression m_Expression;

        public AssignString(Int32 destIndex, VariableScope destScope, Expression expression)
        {
            m_DestIndex = destIndex;
            m_DestScope = destScope;
            m_Expression = expression;

            if (expression.ReturnType != GossipType.String)
                throw new GossipScriptException("Expression must return type string");
        }

        public ScriptResult Update(ScriptExecutionContext context)
        {
            return ScriptResult.Continue;
        }

        public void OnEnter(ScriptExecutionContext context)
        {
            var value = context.Vm.HostBridge.ExpressionEvaluator.Evaluate<String>(m_Expression, context);
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
        }

        public IActionNode Next { get; set; }
        public string CommandName { get; private set; }
        public bool Recall { get; private set; }
    }
}