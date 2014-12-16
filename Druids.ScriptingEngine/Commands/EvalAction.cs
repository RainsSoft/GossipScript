using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Interfaces;
using Druids.ScriptingEngine;

namespace Druids.ScriptingEngine.Commands
{
    public class EvalAction : IActionNode
    {
        private readonly Expression m_Expression;

        public EvalAction(Expression expression)
        {
            m_Expression = expression;
        }

        public ScriptResult Update(ScriptExecutionContext context)
        {
            return ScriptResult.Continue;
        }

        public void OnEnter(ScriptExecutionContext context)
        {
            context.Vm.HostBridge.ExpressionEvaluator.EvaluateDouble(m_Expression, context);
        }

        public void OnExit(ScriptExecutionContext context)
        {
            //
        }

        public IActionNode Next { get; set; }
        public string CommandName { get; private set; }
        public bool Recall { get; private set; }
    }
}