using System;
using Druids.ScriptingEngine.Enums;

namespace Druids.ScriptingEngine.Compiler
{
    public class SemanticToken // Ecapsulates the idea of an operator and an operand
    {
        public double Data;
        public Int32 NumOperands; // 0, 1, 2 or more (or number of function parameters)
        public OperationType OperationType;

        public OperatorAssociativity OperatorAssociativity;

        public Int32 Precedence; // Higher evaluates first

        public string StringData;
        public TokenType TokenType;

        public String TokenValue;

        public bool IsOperator()
        {
            return OperationType == OperationType.Operator;
        }

        public bool IsValue()
        {
            return OperationType == OperationType.Operand;
        }

        public bool IsNumber()
        {
            return IsValue() && (TokenType == TokenType.DecimalLiteral);
        }

        public bool IsBracket()
        {
            return (TokenType == TokenType.OpenBracket || TokenType == TokenType.CloseBracket);
        }

        public bool IsFunction()
        {
            return TokenType == TokenType.HostFunctionCall ||
                   OperationType == OperationType.FunctionCall ||
                   OperationType == OperationType.SystemCall ||
                   OperationType == OperationType.PropertyCall ||
                   OperationType == OperationType.MethodCall ||
                   OperationType == OperationType.CreateAction;
        }

        public bool IsFunctionArgumentSeperator()
        {
            return TokenType == TokenType.FunctionArgumentSeperator;
        }
    }
}