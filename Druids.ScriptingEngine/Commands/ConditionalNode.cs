using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Exceptions;
using Druids.ScriptingEngine.Interfaces;
using Druids.ScriptingEngine;
using Druids.ScriptingEngine.Exceptions;

namespace Druids.ScriptingEngine.Commands
{
    internal class ConditionalNode : IActionNode
    {
        private readonly IActionNode m_AlternativeConsequence; // The else part
        private readonly IActionNode m_Consequence;
        private readonly Expression m_Expression;
        private IActionNode m_StoreNext;

        public ConditionalNode()
        {
            CommandName = "Conditional";
        }

        public ConditionalNode(Expression expression, IActionNode consequence)
            : this()
        {
            m_Expression = expression;
            m_Consequence = consequence;

            if (m_Expression.ReturnType != GossipType.Number)
                throw new GossipScriptException("Expected double return type");
        }

        public ConditionalNode(Expression expression, IActionNode consequence, IActionNode alternativeConsequence)
            : this()
        {
            m_Expression = expression;
            m_Consequence = consequence;
            m_AlternativeConsequence = alternativeConsequence;

            if (m_Expression.ReturnType != GossipType.Number)
                throw new GossipScriptException("Expected double return type");
        }

        public ScriptResult Update(ScriptExecutionContext context)
        {
            return ScriptResult.Continue;
        }

        public void OnEnter(ScriptExecutionContext context)
        {
            if (context.Vm.HostBridge.ExpressionEvaluator.EvaluateDouble(m_Expression, context) == 1.0d)
            {
                //   Trace.WriteLine("Command Conditional True");
                m_StoreNext = Next;
                Next = m_Consequence;
            }
            else
            {
                //   Trace.WriteLine("Command Conditional False");
                m_StoreNext = Next;
                Next = m_AlternativeConsequence;
            }
        }

        public void OnExit(ScriptExecutionContext context)
        {
            // No op
            Next = m_StoreNext;
        }

        public IActionNode Next { get; set; }

        public string CommandName { get; private set; }

        public bool Recall
        {
            get { return true; }
        }
    }
}