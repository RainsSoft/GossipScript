using System;
using System.Collections.Generic;
using System.Linq;
using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Exceptions;
using Druids.ScriptingEngine.Exceptions;
using Druids.ScriptingEngine.Host;

namespace Druids.ScriptingEngine.Compiler
{
    public class ExpressionSemantics
    {
        public IEnumerable<SemanticToken> ApplyExpressionSemantics(IEnumerable<Token> tokenStream,
                                                                   CompilationContext compilationContext)
        {
            List<Token> tokens = tokenStream.ToList();

            // Preprocess semantics
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].Type == TokenType.Subtract)
                {
                    if (tokens.Count - 1 < i + 1)
                        throw new Exception("Unexpected end of token stream");

                    if (tokens[i + 1].Type == TokenType.OpenBracket)
                        tokens[i].Type = TokenType.UnaryMinus;

                    if (i > 0 && tokens[i - 1].OperationType == OperationType.Operator)
                        tokens[i].Type = TokenType.UnaryMinus;

                    if (i > 0 && tokens[i - 1].Type == TokenType.OpenBracket)
                        tokens[i].Type = TokenType.UnaryMinus;

                    if (i == 0)
                        tokens[i].Type = TokenType.UnaryMinus;
                }

                // Missing Semi colon checks
                if (i > 0)
                {
                    if (tokens[i].Type == TokenType.HostFunctionCall)
                    {
                        if (tokens[i - 1].Type != TokenType.OpenBracket &&
                            tokens[i - 1].OperationType != OperationType.Operator &&
                            tokens[i - 1].Type != TokenType.FunctionArgumentSeperator)
                        {
                            throw new GossipScriptMissingSemiColonException("Missing SemiColon", tokens[i]);
                        }
                    }
                }
            }

            // Convert to semantic tokens
            var rv = new List<SemanticToken>(tokens.Count());
            for (int i = 0; i < tokens.Count; i++)
            {
                Token token = tokens[i];

                if (token.Type == TokenType.EndStatement || token.Type == TokenType.EndOfProgram)
                    continue;

                var t = new SemanticToken {TokenType = token.Type, OperationType = OperationType.Operator};

                switch (token.Type)
                {
                    case TokenType.DecimalLiteral:
                        t.OperationType = OperationType.Operand;
                        t.Data = Double.Parse(token.Value);
                        break;
                    case TokenType.Addition:
                        t.Precedence = 6;
                        t.OperatorAssociativity = OperatorAssociativity.Left;
                        break;
                    case TokenType.Subtract:
                        t.Precedence = 6;
                        t.OperatorAssociativity = OperatorAssociativity.Left;
                        break;
                    case TokenType.Multiply:
                        t.Precedence = 7;
                        t.OperatorAssociativity = OperatorAssociativity.Left;
                        break;
                    case TokenType.Divide:
                        t.Precedence = 7;
                        t.OperatorAssociativity = OperatorAssociativity.Left;
                        break;
                    case TokenType.PowerOf:
                        t.Precedence = 7;
                        t.OperatorAssociativity = OperatorAssociativity.Right;
                        break;
                    case TokenType.UnaryMinus:
                        t.Precedence = 8;
                        t.OperatorAssociativity = OperatorAssociativity.Right;
                        break;
                    case TokenType.Negation:
                        t.Precedence = 8;
                        t.OperatorAssociativity = OperatorAssociativity.Right;
                        break;
                    case TokenType.Modulo:
                        t.Precedence = 7;
                        t.OperatorAssociativity = OperatorAssociativity.Left;
                        break;
                    case TokenType.GreaterThan:
                        t.Precedence = 5;
                        t.OperatorAssociativity = OperatorAssociativity.Left;
                        break;
                    case TokenType.GreaterThanOrEqualTo:
                        t.Precedence = 5;
                        t.OperatorAssociativity = OperatorAssociativity.Left;
                        break;
                    case TokenType.LessThan:
                        t.Precedence = 5;
                        t.OperatorAssociativity = OperatorAssociativity.Left;
                        break;
                    case TokenType.LessThanOrEqualTo:
                        t.Precedence = 5;
                        t.OperatorAssociativity = OperatorAssociativity.Left;
                        break;
                    case TokenType.Equal:
                        t.Precedence = 4;
                        t.OperatorAssociativity = OperatorAssociativity.Left;
                        break;
                    case TokenType.NotEqual:
                        t.Precedence = 4;
                        t.OperatorAssociativity = OperatorAssociativity.Left;
                        break;
                    case TokenType.LogicalAnd:
                        t.Precedence = 3;
                        t.OperatorAssociativity = OperatorAssociativity.Left;
                        break;
                    case TokenType.LogicalOr:
                        t.Precedence = 2;
                        t.OperatorAssociativity = OperatorAssociativity.Left;
                        break;
                    case TokenType.HostFunctionCall:
                        HostCall function = compilationContext.HostCallTable.GetFunctionByName(token.Value);
                        t.OperationType = OperationType.FunctionCall;
                        t.Precedence = 9;
                        t.NumOperands = function.NumParameters;
                        t.Data = function.Id;
                        break;
                    case TokenType.FunctionArgumentSeperator:
                        t.OperationType = OperationType.None;
                        break;
                    case TokenType.LocalFlagAccess:
                        t.OperationType = OperationType.Operand;
                        t.Precedence = 9;
                        t.Data = token.GetAccessIndex(compilationContext.EnumTable);
                        break;
                    case TokenType.LocalVariableAccess:
                        t.OperationType = OperationType.Operand;
                        t.Precedence = 9;
                        t.Data = token.GetAccessIndex(compilationContext.EnumTable);
                        break;
                    case TokenType.GlobalFlagAccess:
                        t.OperationType = OperationType.Operand;
                        t.Precedence = 9;
                        t.Data = token.GetAccessIndex(compilationContext.EnumTable);
                        break;
                    case TokenType.GlobalVariableAccess:
                        t.OperationType = OperationType.Operand;
                        t.Precedence = 9;
                        t.Data = token.GetAccessIndex(compilationContext.EnumTable);
                        break;
                    case TokenType.TrueLiteral:
                        t.TokenType = TokenType.DecimalLiteral;
                        t.OperationType = OperationType.Operand;
                        t.Data = 1;
                        t.Precedence = 9;
                        break;
                    case TokenType.FalseLiteral:
                        t.TokenType = TokenType.DecimalLiteral;
                        t.OperationType = OperationType.Operand;
                        t.Precedence = 9;
                        break;
                    case TokenType.OpenBracket:
                        t.OperationType = OperationType.None;
                        break;
                    case TokenType.CloseBracket:
                        t.OperationType = OperationType.None;
                        break;
                    case TokenType.Assignment:
                        throw new GossipScriptException("Assignment operator not allowed within an expression");
                        break;
                    case TokenType.DecrementAndAssign:
                        throw new GossipScriptException("Decrement operator not allowed within an expression");
                        break;
                    case TokenType.IncrementAndAssign:
                        throw new GossipScriptException("Decrement operator not allowed within an expression");
                        break;
                    case TokenType.LocalStringAccess:
                        t.OperationType = OperationType.Operand;
                        t.Precedence = 9;
                        t.Data = token.GetAccessIndex(compilationContext.EnumTable);
                        break;
                    case TokenType.GlobalStringAccess:
                        t.OperationType = OperationType.Operand;
                        t.Precedence = 9;
                        t.Data = token.GetAccessIndex(compilationContext.EnumTable);
                        break;
                    case TokenType.StringLiteral:
                        t.TokenValue = token.Value;
                        t.OperationType = OperationType.Operand;
                        string data = token.Value.Substring(1, token.Value.Length - 2); // Remove brackets
                        t.Data = compilationContext.StringTable.GetIdByString(data);
                        break;
                    case TokenType.ReferenceObjectAccess:
                        t.OperationType = OperationType.Operand;
                        t.Precedence = 9;
                        t.Data = token.GetAccessIndex(compilationContext.EnumTable);
                        break;
                    case TokenType.GlobalStringAccessEnum:
                        t.OperationType = OperationType.Operand;
                        t.Precedence = 9;
                        t.Data = token.GetAccessIndex(compilationContext.EnumTable);
                        t.TokenType = TokenType.GlobalStringAccess;
                        break;
                    case TokenType.LocalStringAccessEnum:
                        t.OperationType = OperationType.Operand;
                        t.Precedence = 9;
                        t.Data = token.GetAccessIndex(compilationContext.EnumTable);
                        t.TokenType = TokenType.LocalStringAccess;
                        break;
                    case TokenType.LocalVariableAccessEnum:
                        t.OperationType = OperationType.Operand;
                        t.Precedence = 9;
                        t.Data = token.GetAccessIndex(compilationContext.EnumTable);
                        t.TokenType = TokenType.LocalVariableAccess;
                        break;
                    case TokenType.GlobalVariableAccessEnum:
                        t.OperationType = OperationType.Operand;
                        t.Precedence = 9;
                        t.Data = token.GetAccessIndex(compilationContext.EnumTable);
                        t.TokenType = TokenType.GlobalVariableAccess;
                        break;
                    case TokenType.LocalFlagAccessEnum:
                        t.OperationType = OperationType.Operand;
                        t.Precedence = 9;
                        t.Data = token.GetAccessIndex(compilationContext.EnumTable);
                        t.TokenType = TokenType.LocalFlagAccess;
                        break;
                    case TokenType.GlobalFlagAccessEnum:
                        t.OperationType = OperationType.Operand;
                        t.Precedence = 9;
                        t.Data = token.GetAccessIndex(compilationContext.EnumTable);
                        t.TokenType = TokenType.GlobalFlagAccess;
                        break;
                    case TokenType.Event:
                    case TokenType.Self:
                        {
                            t.OperationType = OperationType.Operand;
                            t.Precedence = 9;
                            break;
                        }
                    case TokenType.ReferencePropertyGet:
                        {
                            t.OperationType = OperationType.PropertyCall;
                            t.Precedence = 9;
                            t.StringData = token.Value;
                            t.Data = -666; // Unknown at this stage
                            break;
                        }
                    case TokenType.MethodCall:
                        {
                            t.OperationType = OperationType.MethodCall;
                            t.Precedence = 9;
                            t.StringData = token.Value;
                            t.Data = -666; // Unknown at this stage
                            break;
                        }
                    case TokenType.ReferenceAction:
                        {
                            t.OperationType = OperationType.CreateAction;
                            t.Precedence = 9;
                            t.StringData = token.Value;
                            t.Data = -666; // Unknown at this stage
                            break;
                        }
                    //case TokenType.ModuleAccess:
                    //    {
                    //        t.OperationType = OperationType.Operand;
                    //        t.Precedence = 9;
                    //        t.StringData = token.Value;
                    //        t.Data = compilationContext.ModuleBindingTable.GetModuleByName(token.Value).Id;
                    //        break;
                    //    }
                    case TokenType.ModuleMethodCall:
                        {
                            t.OperationType = OperationType.MethodCall;
                            t.Precedence = 9;
                            t.StringData = token.Value;
                            t.Data = -666; // Unknown at this stage
                            break;
                        }
                    case TokenType.ModulePropertyGet:
                        {
                            t.OperationType = OperationType.PropertyCall;
                            t.Precedence = 9;
                            t.StringData = token.Value;
                            t.Data = -666; // Unknown at this stage
                            break;
                        }
                    case TokenType.Enum:
                    {
                            t.TokenType = TokenType.DecimalLiteral;
                            t.OperationType = OperationType.Operand;
                            t.Data = compilationContext.EnumTable.DefinesByString[token.Value];
                            t.Precedence = 9;
                            break;
                        }
                    default:

                        throw new NotImplementedException(t.TokenType.ToString());

                        break;
                }
                rv.Add(t);
            }

            ValidateSemantics(rv);
            return rv;
        }

        private void ValidateSemantics(List<SemanticToken> tokens)
        {
            // Check function argument seperator actually belongs to a function
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].TokenType == TokenType.FunctionArgumentSeperator)
                {
                    bool foundFunction = false;
                    for (int j = i - 1; j >= 0; j--)
                    {
                        if (tokens[j].IsFunction())
                        {
                            foundFunction = true;
                            break;
                        }
                    }

                    if (foundFunction == false)
                        throw new GossipScriptException("Could not find argument seperator for function");
                }
            }

            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].TokenType == TokenType.HostFunctionCall)
                {
                    int numParameters = 0;
                    int insubFunction = 0;
                    for (int j = i + 1; j < tokens.Count; j++)
                    {
                        if (tokens[j].TokenType == TokenType.HostFunctionCall)
                        {
                            insubFunction++;
                        }
                        if (tokens[j].TokenType == TokenType.CloseBracket)
                        {
                            insubFunction--;
                        }
                        if (insubFunction == 0)
                        {
                            if (tokens[j].TokenType == TokenType.FunctionArgumentSeperator)
                            {
                                numParameters++;
                            }
                        }
                    }

                    if (tokens[i + 1].TokenType == TokenType.OpenBracket &&
                        tokens[i + 2].TokenType == TokenType.CloseBracket)
                    {
                        if (numParameters == 0 && tokens[i].NumOperands != 0)
                            throw new GossipScriptException("Invalid Number of arguments in function");
                    }

                    if (numParameters + 1 != tokens[i].NumOperands &&
                        !(numParameters == 0 && tokens[i].NumOperands == 0))
                        throw new GossipScriptException("Invalid Number of arguments in function");
                }
            }
        }
    }
}