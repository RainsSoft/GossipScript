using System;
using System.Collections.Generic;
using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Exceptions;
using Druids.ScriptingEngine.Exceptions;

namespace Druids.ScriptingEngine.Compiler
{
    public enum ExpressionIndexType
    {
        None = 0,
        Base = 1,
        Operator = 2,
        Bracket = 3
    }

    public class ExpressionIndex
    {
        public ExpressionIndex(ExpressionIndexType type, Int32 index)
        {
            Type = type;
            Index = index;
        }

        public ExpressionIndexType Type { get; set; } // Operator or Open bracket

        public Int32 Index { get; set; } // Index into the expression
    }

    public class ExpressionNormalizer
    {
        public List<SemanticToken> NormalizeExpression(Int32 startIndex, Int32 stopIndex, List<SemanticToken> input)
        {
            var rv = new List<SemanticToken>();

            if (input.Count == 0)
                return rv;

            var unwindStack = new Stack<ExpressionIndex>();
            unwindStack.Push(new ExpressionIndex(ExpressionIndexType.Base, 0));

            var stack = new Stack<List<SemanticToken>>();
            var currentTokens = new List<SemanticToken>();

            bool isPostFix = false;
            for (int i = startIndex; i < stopIndex; i++)
            {
                OperationType op = input[i].OperationType;
                TokenType tk = input[i].TokenType;

                // Encountered postfix
                if (op == OperationType.PropertyCall)
                {
                    // Unwind the stack
                    UnwindStack(unwindStack.Peek().Index, stack, rv);

                    isPostFix = true;
                    currentTokens = new List<SemanticToken>();
                    stack.Push(currentTokens);
                }

                if (op == OperationType.MethodCall || op == OperationType.CreateAction)
                {
                    UnwindStack(unwindStack.Peek().Index, stack, rv);

                    isPostFix = true;
                    currentTokens = new List<SemanticToken>();
                    stack.Push(currentTokens);

                    currentTokens.Add(input[i]); // Add method call
                    i++; // Read ahead one

                    int end = getEndIndex(i, input) + 1;
                    List<SemanticToken> methodTokens = NormalizeExpression(i, end, input);

                    var localStack = new Stack<List<SemanticToken>>();
                    currentTokens = new List<SemanticToken>();
                    for (int j = 0; j < methodTokens.Count; j++) // Don't add the brackets
                    {
                        currentTokens.Add(methodTokens[j]);
                        if (methodTokens[j].TokenType == TokenType.FunctionArgumentSeperator)
                        {
                            localStack.Push(currentTokens);
                            currentTokens = new List<SemanticToken>();
                        }
                    }
                    localStack.Push(currentTokens);

                    while (localStack.Count > 0)
                    {
                        stack.Push(localStack.Pop());
                    }

                    i += (end - i) - 1; // Skip tokens
                    continue;
                }

                // Encountered infix
                if (tk == TokenType.FunctionArgumentSeperator ||
                    op == OperationType.Operator)
                {
                    isPostFix = false;
                    UnwindStack(unwindStack.Peek().Index, stack, rv);
                }

                if (tk == TokenType.CloseBracket)
                {
                    isPostFix = false;
                    UnwindStack(unwindStack.Peek().Index, stack, rv);
                }

                // Update the unwind position
                if (op == OperationType.Operator || tk == TokenType.FunctionArgumentSeperator)
                    unwindStack.Push(new ExpressionIndex(ExpressionIndexType.Operator, Math.Max(0, rv.Count + 1)));

                if (tk == TokenType.OpenBracket)
                {
                    unwindStack.Push(new ExpressionIndex(ExpressionIndexType.Bracket, rv.Count + 1));
                }

                if (tk == TokenType.CloseBracket)
                {
                    // Pop all operators off
                    while (unwindStack.Peek().Type == ExpressionIndexType.Operator)
                    {
                        unwindStack.Pop();
                    }
                    unwindStack.Pop();
                }

                if (isPostFix)
                {
                    currentTokens.Add(input[i]);
                }
                else
                {
                    rv.Add(input[i]);
                }
            }

            // Unwind stack if anything left
            while (unwindStack.Count > 0)
                UnwindStack(unwindStack.Pop().Index, stack, rv);

            return rv;
        }

        private static int getEndIndex(Int32 start, List<SemanticToken> input)
        {
            int height = 0;
            for (int i = start; i < input.Count; i++)
            {
                if (input[i].TokenType == TokenType.OpenBracket)
                    height++;

                if (input[i].TokenType == TokenType.CloseBracket)
                    height--;

                if (height == 0)
                    return i;

                if (input[i].TokenType == TokenType.EndStatement)
                    throw new GossipScriptException("Method not closed");
            }
            return input.Count - 1;
        }

        private static void UnwindStack(Int32 index, Stack<List<SemanticToken>> stack, List<SemanticToken> rv)
        {
            foreach (var item in stack)
            {
                //   if (index < rv.Count)
                {
                    rv.Insert(index, new SemanticToken {TokenType = TokenType.OpenBracket});
                    for (int j = item.Count - 1; j >= 0; j--)
                    {
                        rv.Insert(index, item[j]);
                    }
                    rv.Add(new SemanticToken {TokenType = TokenType.CloseBracket});
                }
            }
            stack.Clear();
        }
    }
}