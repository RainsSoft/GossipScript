using System;
using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Interfaces;
using Druids.ScriptingEngine;

namespace Druids.ScriptingEngine.Commands
{
    public class ModifyVariable : IActionNode
    {
        private readonly int m_Index;
        private readonly TokenType m_Operand;
        private readonly TokenType m_OperatorType;
        private readonly double m_Value;


        public ModifyVariable(Int32 index, TokenType operand, TokenType operatorType, Double value)
        {
            m_Index = index;
            m_Operand = operand;
            m_OperatorType = operatorType;
            m_Value = value;
            if (m_OperatorType != TokenType.Assignment && m_OperatorType != TokenType.Increment &&
                m_OperatorType != TokenType.Decrement)
                throw new ArgumentException();
        }

        public ScriptResult Update(ScriptExecutionContext context)
        {
            return ScriptResult.Continue;
        }

        public void OnEnter(ScriptExecutionContext context)
        {
            if (m_Operand == TokenType.GlobalVariableAccess)
            {
                if (m_OperatorType == TokenType.Increment)
                    context.Vm.GlobalState.Var[m_Index] += m_Value;

                if (m_OperatorType == TokenType.Decrement)
                    context.Vm.GlobalState.Var[m_Index] -= m_Value;

                return;
            }

            if (m_Operand == TokenType.LocalVariableAccess)
            {
                if (m_OperatorType == TokenType.Increment)
                    context.LocalState.Var[m_Index] += m_Value;

                if (m_OperatorType == TokenType.Decrement)
                    context.LocalState.Var[m_Index] -= m_Value;

                return;
            }

            throw new Exception("Invalid operand type");
        }

        public void OnExit(ScriptExecutionContext context)
        {
        }

        public IActionNode Next { get; set; }
        public string CommandName { get; private set; }
        public bool Recall { get; private set; }
    }
}