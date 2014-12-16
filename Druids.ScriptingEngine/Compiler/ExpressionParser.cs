using System;
using System.Collections.Generic;
using System.Linq;
using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Exceptions;
using Druids.ScriptingEngine.Exceptions;

namespace Druids.ScriptingEngine.Compiler
{
    public class ExpressionParser
    {
        private readonly ExpressionAnalyzer m_ExpressionAnalyzer = new ExpressionAnalyzer();
        private readonly ExpressionNormalizer m_ExpressionNormalizer = new ExpressionNormalizer();
        private readonly ExpressionSemantics m_ExpressionSemantics = new ExpressionSemantics();

        /// <summary>
        ///     Returns a list of semantic tokens in Reverse Polish Notation
        /// </summary>
        public Expression Parse(List<Token> tokenstream, CompilationContext compilationContext, Boolean normalize = true)
        {
            var exp = new Expression();
            var instructions = new List<SemanticToken>();
            List<SemanticToken> opcodes =
                m_ExpressionSemantics.ApplyExpressionSemantics(tokenstream, compilationContext).ToList();
            if (normalize)
                opcodes = m_ExpressionNormalizer.NormalizeExpression(0, opcodes.Count, opcodes);

            var stack = new Stack<SemanticToken>(tokenstream.Count);

            // Shunting Yard Algorithm
            // http://en.wikipedia.org/wiki/Shunting-yard_algorithm
            foreach (SemanticToken opcode in opcodes)
            {
                if (opcode.IsValue())
                {
                    instructions.Add(opcode);
                }
                else
                {
                    if (opcode.IsFunction())
                    {
                        stack.Push(opcode);
                    }
                    else
                    {
                        if (opcode.IsFunctionArgumentSeperator())
                        {
                            while (stack.Count > 0)
                            {
                                if (stack.Peek().TokenType == TokenType.OpenBracket)
                                    break;

                                SemanticToken o = stack.Pop();
                                instructions.Add(o);
                            }
                        }
                        else
                        {
                            if (opcode.IsOperator())
                            {
                                while (stack.Count > 0)
                                {
                                    SemanticToken peek = stack.Peek();
                                    if ((opcode.Precedence < peek.Precedence) ||
                                        opcode.OperatorAssociativity == OperatorAssociativity.Left &&
                                        opcode.Precedence == peek.Precedence)
                                    {
                                        instructions.Add(stack.Pop());
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                stack.Push(opcode);
                            }
                            else
                            {
                                // Left Bracket
                                if (opcode.TokenType == TokenType.OpenBracket)
                                    stack.Push(opcode);

                                if (opcode.TokenType == TokenType.CloseBracket)
                                {
                                    SemanticToken token = stack.Pop();
                                    while (stack.Count > 0 && token.TokenType != TokenType.OpenBracket)
                                    {
                                        instructions.Add(token);
                                        token = stack.Pop();
                                    }

                                    if (token.TokenType != TokenType.OpenBracket)
                                        throw new GossipScriptException("Mismatched brackets");
                                }
                            }
                        }
                    }
                }
            }

            // When there are no more tokens to read
            while (stack.Count > 0)
            {
                SemanticToken op = stack.Pop();
                if (op.IsBracket())
                    throw new GossipScriptException("Mismatched brackets");

                instructions.Add(op);
            }

            // Apply the instructs to the expression and determine the return type        
            ExtendedTypeAdapter result = m_ExpressionAnalyzer.DetermineReturnType(instructions, compilationContext);
            exp.Instructions = ConvertToInstructions(instructions);
            exp.ReturnType = result.TypeAdapter.GossipType;
            exp.HostReturnType = result.TypeAdapter.HostType;

            // TODO split into ExpressionInfo and Expression classes
            exp.HostData = (Int32) result.Data;
            exp.Target = (Int32) result.Target;
            return exp;
        }


        public List<Instruction> ConvertToInstructions(List<SemanticToken> tokens)
        {
            var rv = new List<Instruction>(tokens.Count);
            foreach (SemanticToken token in tokens)
            {
                rv.Add(new Instruction
                    {
                        TokenType = token.TokenType,
                        Data = token.Data,
                    }
                    );
            }
            return rv;
        }
    }
}