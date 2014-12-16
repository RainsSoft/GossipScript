using System.Text.RegularExpressions;
using Druids.ScriptingEngine.Enums;

namespace Druids.ScriptingEngine.Compiler
{
    public class GossipLexer : Lexer
    {
        
        public GossipLexer()
        {
            AddToken(TokenType.LogicalAnd, OperationType.Operator, new Regex(Regex.Escape("&&")));
           
            m_TokenDefinitions.Add(new TokenDefinition(TokenType.LogicalOr, OperationType.Operator, new Regex(Regex.Escape("||"))));

            m_TokenDefinitions.Add(new TokenDefinition(TokenType.Whitespace, OperationType.None, new Regex(@"\s+"),TokenDiscardPolicy.Discard));
            m_TokenDefinitions.Add(new TokenDefinition(TokenType.OpenCurlyBrace, OperationType.None, new Regex("{")));
            m_TokenDefinitions.Add(new TokenDefinition(TokenType.CloseCurlyBrace, OperationType.None, new Regex("}")));
            m_TokenDefinitions.Add(new TokenDefinition(TokenType.ParallelBlock, OperationType.None,new Regex("parallel")));
            m_TokenDefinitions.Add(new TokenDefinition(TokenType.EvaluateExpression, OperationType.Operator,new Regex("eval")));
            m_TokenDefinitions.Add(new TokenDefinition(TokenType.If, OperationType.Operator, new Regex("if")));
            m_TokenDefinitions.Add(new TokenDefinition(TokenType.Else, OperationType.Operator, new Regex("else")));

            m_TokenDefinitions.Add(new TokenDefinition(TokenType.Yeild, OperationType.Operator, new Regex("yeild")));
            m_TokenDefinitions.Add(new TokenDefinition(TokenType.Return, OperationType.Operator, new Regex("return")));
            m_TokenDefinitions.Add(new TokenDefinition(TokenType.Exit, OperationType.Operator, new Regex("exit")));

            m_TokenDefinitions.Add(new TokenDefinition(TokenType.OpenBracket, OperationType.None,new Regex(Regex.Escape("("))));
            m_TokenDefinitions.Add(new TokenDefinition(TokenType.CloseBracket, OperationType.None,new Regex(Regex.Escape(")"))));

            m_TokenDefinitions.Add(new TokenDefinition(TokenType.StringLiteral, OperationType.Operand,new Regex(@"""[^""\\]*(?:\\.[^""\\]*)*""")));

            m_TokenDefinitions.Add(new TokenDefinition(TokenType.EndStatement, OperationType.None, new Regex(";")));
            m_TokenDefinitions.Add(new TokenDefinition(TokenType.WaitStatement, OperationType.SystemCall,new Regex("system.Wait")));
            m_TokenDefinitions.Add(new TokenDefinition(TokenType.DecimalLiteral, OperationType.Operand,new Regex("[0-9]+([.][0-9]+)?")));
            m_TokenDefinitions.Add(new TokenDefinition(TokenType.TrueLiteral, OperationType.Operand, new Regex("true")));
            m_TokenDefinitions.Add(new TokenDefinition(TokenType.FalseLiteral, OperationType.Operand, new Regex("false")));

            m_TokenDefinitions.Add(new TokenDefinition(TokenType.GlobalVariableAccess, OperationType.Operand,new Regex("globalVar" + Regex.Escape("[") + "[0-9]+" +Regex.Escape("]"))));
            m_TokenDefinitions.Add(new TokenDefinition(TokenType.GlobalFlagAccess, OperationType.Operand,new Regex("globalFlag" + Regex.Escape("[") + "[0-9]+" +Regex.Escape("]"))));
            m_TokenDefinitions.Add(new TokenDefinition(TokenType.GlobalStringAccess, OperationType.Operand,new Regex("globalStr" + Regex.Escape("[") + "[0-9]+" +Regex.Escape("]"))));

            m_TokenDefinitions.Add(new TokenDefinition(TokenType.LocalVariableAccess, OperationType.Operand,new Regex("localVar" + Regex.Escape("[") + "[0-9]+" +Regex.Escape("]"))));
            m_TokenDefinitions.Add(new TokenDefinition(TokenType.LocalFlagAccess, OperationType.Operand,new Regex("localFlag" + Regex.Escape("[") + "[0-9]+" +Regex.Escape("]"))));
            m_TokenDefinitions.Add(new TokenDefinition(TokenType.LocalStringAccess, OperationType.Operand,new Regex("localStr" + Regex.Escape("[") + "[0-9]+" +Regex.Escape("]"))));

            m_TokenDefinitions.Add(new TokenDefinition(TokenType.GlobalVariableAccessEnum, OperationType.Operand,new Regex("globalVar" + Regex.Escape("[") + "[^\\].]*" +Regex.Escape("]"))));
            m_TokenDefinitions.Add(new TokenDefinition(TokenType.GlobalFlagAccessEnum, OperationType.Operand,new Regex("globalFlag" + Regex.Escape("[") + "[^\\].]*" +Regex.Escape("]"))));
            m_TokenDefinitions.Add(new TokenDefinition(TokenType.GlobalStringAccessEnum, OperationType.Operand,new Regex("globalStr" + Regex.Escape("[") + "[^\\].]*" +Regex.Escape("]"))));

            m_TokenDefinitions.Add(new TokenDefinition(TokenType.LocalVariableAccessEnum, OperationType.Operand,new Regex("localVar" + Regex.Escape("[") + "[^\\].]*" +Regex.Escape("]"))));
            m_TokenDefinitions.Add(new TokenDefinition(TokenType.LocalFlagAccessEnum, OperationType.Operand,new Regex("localFlag" + Regex.Escape("[") + "[^\\].]*" +Regex.Escape("]"))));
            m_TokenDefinitions.Add(new TokenDefinition(TokenType.LocalStringAccessEnum, OperationType.Operand,new Regex("localStr" + Regex.Escape("[") + "[^\\].]*" +Regex.Escape("]"))));

            m_TokenDefinitions.Add(new TokenDefinition(TokenType.ReferenceObjectAccess, OperationType.Operand,new Regex("ref" + Regex.Escape("[") + "[0-9]+" +Regex.Escape("]"))));
            m_TokenDefinitions.Add(new TokenDefinition(TokenType.ReferenceObjectAccessEnum, OperationType.Operand,new Regex("ref" + Regex.Escape("[") + "[^\\].]*" +Regex.Escape("]"))));

            m_TokenDefinitions.Add(new TokenDefinition(TokenType.Self, OperationType.Operand, new Regex("self")));
            m_TokenDefinitions.Add(new TokenDefinition(TokenType.Event, OperationType.Operand, new Regex("event")));

            // For slightly better error messages
            m_TokenDefinitions.Add(new TokenDefinition(TokenType.GlobalVariableAccess, OperationType.Error,new Regex("globalVar")));
            m_TokenDefinitions.Add(new TokenDefinition(TokenType.GlobalFlagAccess, OperationType.Error,new Regex("globalFlag")));
            m_TokenDefinitions.Add(new TokenDefinition(TokenType.GlobalStringAccess, OperationType.Error,new Regex("globalStr")));

            m_TokenDefinitions.Add(new TokenDefinition(TokenType.LocalStringAccess, OperationType.Error,new Regex("localStr")));
            m_TokenDefinitions.Add(new TokenDefinition(TokenType.LocalVariableAccess, OperationType.Error,new Regex("localVar")));
            m_TokenDefinitions.Add(new TokenDefinition(TokenType.LocalFlagAccess, OperationType.Error,new Regex("localFlag")));

            m_TokenDefinitions.Add(new TokenDefinition(TokenType.Decrement, OperationType.Operator,new Regex(Regex.Escape("--"))));
            m_TokenDefinitions.Add(new TokenDefinition(TokenType.Increment, OperationType.Operator,new Regex(Regex.Escape("++"))));
            m_TokenDefinitions.Add(new TokenDefinition(TokenType.IncrementAndAssign, OperationType.Operator,new Regex(Regex.Escape("+="))));
            m_TokenDefinitions.Add(new TokenDefinition(TokenType.DecrementAndAssign, OperationType.Operator,new Regex(Regex.Escape("-="))));

            m_TokenDefinitions.Add(new TokenDefinition(TokenType.Equal, OperationType.Operator, new Regex("==")));
            m_TokenDefinitions.Add(new TokenDefinition(TokenType.GreaterThanOrEqualTo, OperationType.Operator,new Regex(">=")));
            m_TokenDefinitions.Add(new TokenDefinition(TokenType.LessThanOrEqualTo, OperationType.Operator,new Regex("<=")));
            m_TokenDefinitions.Add(new TokenDefinition(TokenType.GreaterThan, OperationType.Operator, new Regex(">")));
            m_TokenDefinitions.Add(new TokenDefinition(TokenType.LessThan, OperationType.Operator, new Regex("<")));

            m_TokenDefinitions.Add(new TokenDefinition(TokenType.Assignment, OperationType.Operator, new Regex("=")));
            m_TokenDefinitions.Add(new TokenDefinition(TokenType.Comment, OperationType.None,new Regex(Regex.Escape("//") + ".*" + "(\n|\r|\r\n)*"),TokenDiscardPolicy.Discard));
            m_TokenDefinitions.Add(new TokenDefinition(TokenType.Subtract, OperationType.Operator,new Regex(Regex.Escape("-"))));
            m_TokenDefinitions.Add(new TokenDefinition(TokenType.Addition, OperationType.Operator,new Regex(Regex.Escape("+"))));
            m_TokenDefinitions.Add(new TokenDefinition(TokenType.Divide, OperationType.Operator,new Regex(Regex.Escape("/"))));
            m_TokenDefinitions.Add(new TokenDefinition(TokenType.Multiply, OperationType.Operator,new Regex(Regex.Escape("*"))));
            m_TokenDefinitions.Add(new TokenDefinition(TokenType.NotEqual, OperationType.Operator,new Regex(Regex.Escape("!="))));
            m_TokenDefinitions.Add(new TokenDefinition(TokenType.FunctionArgumentSeperator, OperationType.None,new Regex(Regex.Escape(","))));
            m_TokenDefinitions.Add(new TokenDefinition(TokenType.Negation, OperationType.Operator,new Regex(Regex.Escape("!"))));
        }

        private void AddToken(TokenType type, OperationType op, Regex regex)
        {
            m_TokenDefinitions.Add(new TokenDefinition(type, op, regex));
        }
    }
}