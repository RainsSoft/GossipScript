using System;
using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Interfaces;
using Druids.ScriptingEngine;

namespace Druids.ScriptingEngine.Commands
{
    internal class AssignVariable : IActionNode
    {
        private readonly Expression m_Expression;
        private readonly int m_Index;
        private readonly TokenType m_Operand;
        private readonly TokenType m_OperatorType;


        public AssignVariable(Int32 index, TokenType operand, TokenType operatorType, Expression expression)
        {
            m_Index = index;
            m_Operand = operand;
            m_OperatorType = operatorType;
            m_Expression = expression;

            if (operand != TokenType.GlobalVariableAccess && operand != TokenType.LocalVariableAccess)
                throw new ArgumentException();

            if (m_OperatorType != TokenType.Assignment)
                throw new ArgumentException();
        }

        public ScriptResult Update(ScriptExecutionContext context)
        {
            return ScriptResult.Continue;
        }

        public void OnEnter(ScriptExecutionContext context)
        {
            double value = context.Vm.HostBridge.ExpressionEvaluator.EvaluateDouble(m_Expression, context);

            if (m_Operand == TokenType.GlobalVariableAccess)
            {
                if (m_OperatorType == TokenType.Assignment)
                    context.Vm.GlobalState.Var[m_Index] = value;

                if (m_OperatorType == TokenType.Increment)
                    context.Vm.GlobalState.Var[m_Index] += value;

                if (m_OperatorType == TokenType.Decrement)
                    context.Vm.GlobalState.Var[m_Index] -= value;

                return;
            }

            if (m_Operand == TokenType.LocalVariableAccess)
            {
                if (m_OperatorType == TokenType.Assignment)
                    context.LocalState.Var[m_Index] = value;

                if (m_OperatorType == TokenType.Increment)
                    context.LocalState.Var[m_Index] += value;

                if (m_OperatorType == TokenType.Decrement)
                    context.LocalState.Var[m_Index] -= value;

                return;
            }

            throw new Exception("Invalid operand type");
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