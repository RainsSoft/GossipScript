using System;
using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Exceptions;
using Druids.ScriptingEngine.Interfaces;
using Druids.ScriptingEngine;
using Druids.ScriptingEngine.Exceptions;

namespace Druids.ScriptingEngine.Commands
{
    internal class AssignReference : IActionNode
    {
        private readonly Expression m_Expression;
        private readonly int m_Index;

        public AssignReference(Int32 index, Expression expression)
        {
            m_Index = index;
            m_Expression = expression;

            if (m_Expression.ReturnType != GossipType.ReferenceObject)
                throw new GossipScriptException("Expected Return type is reference object");
        }

        public ScriptResult Update(ScriptExecutionContext context)
        {
            return ScriptResult.Continue;
        }

        public void OnEnter(ScriptExecutionContext context)
        {
            var value = context.Vm.HostBridge.ExpressionEvaluator.Evaluate<Object>(m_Expression, context);

            context.LocalState.Ref[m_Index] = value;
        }

        public void OnExit(ScriptExecutionContext context)
        {
        }

        public IActionNode Next { get; set; }
        public string CommandName { get; private set; }
        public bool Recall { get; private set; }
    }
}