using System;
using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Interfaces;
using Druids.ScriptingEngine;

namespace Druids.ScriptingEngine.Commands
{
    internal class ModifyFlag : IActionNode
    {
        private readonly Expression m_Expression;
        private readonly int m_Index;
        private readonly TokenType m_OperandType;

        public ModifyFlag(Int32 index, TokenType operandType, Expression expression)
        {
            m_Index = index;
            m_OperandType = operandType;
            m_Expression = expression;

            if (operandType != TokenType.GlobalFlagAccess && operandType != TokenType.LocalFlagAccess)
                throw new ArgumentException();
        }

        public ScriptResult Update(ScriptExecutionContext context)
        {
            return ScriptResult.Continue;
        }

        public void OnEnter(ScriptExecutionContext context)
        {
            double result = context.Vm.HostBridge.ExpressionEvaluator.EvaluateDouble(m_Expression, context);

            if (m_OperandType == TokenType.LocalFlagAccess)
                context.LocalState.Flags[m_Index] = (result == 1);

            if (m_OperandType == TokenType.GlobalFlagAccess)
                context.Vm.GlobalState.Flags[m_Index] = (result == 1);
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