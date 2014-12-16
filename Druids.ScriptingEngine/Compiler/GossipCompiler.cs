using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Druids.ScriptingEngine.Commands;
using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Exceptions;
using Druids.ScriptingEngine.Interfaces;
using Druids.ScriptingEngine.Host;

namespace Druids.ScriptingEngine.Compiler
{
    public class GossipCompiler
    {
        private readonly GossipVM m_Engine;
        private readonly ExpressionParser m_ExpressionParser;
        private readonly HostCallTable m_HostCallTable;
        private readonly GossipLexer m_Lexer;
        private readonly Precompiler m_Precompiler;
        private readonly ScriptNodeParser m_ScriptNodeParser;
        private readonly StateMachineParser m_StateMachineParser;

        public GossipCompiler(GossipVM engine,
                              GossipLexer lexer,
                              ScriptNodeParser scriptNodeParser)
        {
            m_HostCallTable = engine.HostBridge.HostCallTable;
            m_Precompiler = new Precompiler();
            m_Engine = engine;
            m_Lexer = lexer;
            m_ScriptNodeParser = scriptNodeParser;
            m_ExpressionParser = new ExpressionParser();
            m_StateMachineParser = new StateMachineParser();
        }

        public GossipLexer Lexer
        {
            get { return m_Lexer; }
        }

        public ExpressionParser ExpressionParser
        {
            get { return m_ExpressionParser; }
        }

        public Precompiler Precompiler
        {
            get { return m_Precompiler; }
        }

        public ScriptNodeParser ScriptNodeParser
        {
            get { return m_ScriptNodeParser; }
        }

        private void AssertExpectedTokenValue(Token token, Regex expectedtokenValue, String errorMessage)
        {
            if (expectedtokenValue.Match(token.Value).Success == false)
                throw new GossipScriptException(errorMessage, token);
        }

        private void AssertExpectedTokenValue(Token token, String expectedtokenValue, String errorMessage)
        {
            if (token.Value != expectedtokenValue)
                throw new GossipScriptException(errorMessage, token);
        }

        private void AssertExpectedTokenType(Token token, TokenType expectedtokenType, String errorMessage)
        {
            if (token.Type != expectedtokenType)
                throw new GossipScriptException(errorMessage, token);
        }

        private IActionNode Compile(List<Token> tokenlist, CompilationContext compilationContext)
        {
            Precompiler.CompilationContext = compilationContext;
            List<Token> tokens = m_Precompiler.ApplyPrecompileSemantic(tokenlist);

            IActionNode rootNode = new ActionNode();
            IActionNode currentParent = rootNode;

            int index = -1;
            while (index < tokens.Count - 2)
            {
                Token currentToken = tokens[++index];
                string currentTokenValue = tokens[index].Value;

                switch (currentToken.Type)
                {
                    case TokenType.EndStatement:
                        continue;
                    case TokenType.EvaluateExpression:
                        index = ParseEvaluateExpression(compilationContext, tokens, index, ref currentParent);
                        continue;
                    case TokenType.ParallelBlock:
                        index = ParseParallelBlock(compilationContext, index, tokens, ref currentParent);
                        continue;
                    case TokenType.LocalStringAccess:
                    case TokenType.LocalStringAccessEnum:
                    case TokenType.GlobalStringAccess:
                    case TokenType.GlobalStringAccessEnum:
                        index = ParseStringAccess(compilationContext, currentToken, tokens, index, ref currentParent);
                        continue;
                    case TokenType.LocalVariableAccess:
                    case TokenType.LocalVariableAccessEnum:
                    case TokenType.GlobalVariableAccess:
                    case TokenType.GlobalVariableAccessEnum:
                        index = ParseVariableAccess(compilationContext, currentToken, tokens, index, ref currentParent);
                        continue;
                    case TokenType.GlobalFlagAccess:
                    case TokenType.GlobalFlagAccessEnum:
                    case TokenType.LocalFlagAccess:
                    case TokenType.LocalFlagAccessEnum:
                        index = ParseFlagAccess(compilationContext, currentToken, tokens, index, ref currentParent);
                        continue;
                    case TokenType.Event:
                        index = ParseEvent(compilationContext, tokens, index, ref currentParent);
                        continue;
                    case TokenType.Self:
                        index = ParseSelf(compilationContext, tokens, index, ref currentParent);
                        continue;
                    //case TokenType.ModuleAccess:
                    //    index = ParseModuleAccess(compilationContext, index, tokens, ref currentParent);
                    //    continue;
                    case TokenType.ReferenceObjectAccess:
                    case TokenType.ReferenceObjectAccessEnum:
                        index = ParseReferenceObjectAccess(compilationContext, currentToken, tokens, index,
                                                           ref currentParent);
                        continue;
                    case TokenType.WaitStatement:
                        index = ParseWait(tokens, index, ref currentParent);
                        continue;
                    case TokenType.If:
                        index = ParseConditional(compilationContext, tokens, index, ref currentParent);
                        continue;
                    case TokenType.Else:
                        throw new GossipScriptException("Else without If", currentToken);
                    case TokenType.Yeild:
                        currentParent = ParseYeild(currentParent, tokens, ref index);
                        continue;
                    case TokenType.Return:
                        currentParent = ParseReturn(currentParent, tokens, ref index);
                        continue;
                    case TokenType.Exit:
                        currentParent = ParseExit(currentParent, tokens, ref index);
                        continue;
                }

                // If none of the above in the switch statement
                index = ParseApiCalls(compilationContext, currentTokenValue, index, tokens, currentToken, ref currentParent);
            }

            return rootNode;
        }

        private int ParseApiCalls(CompilationContext compilationContext, string currentTokenValue, int index,
                                  List<Token> tokens, Token currentToken, ref IActionNode currentParent)
        {
            bool match = false;
            foreach (ActionInfo function in m_Engine.GlobalActions.Values)
            {
                if (currentTokenValue == function.TokenName)
                {
                    List<Expression> parametersAsExpressions;
                    int skip = ParseFunctionParameterExpressions(index, tokens, compilationContext,out parametersAsExpressions);

                    currentParent.Next = new ExecuteAction(function, parametersAsExpressions) {Next = new ActionNode()};
                    currentParent = currentParent.Next;
                    match = true;

                    index += skip;
                    currentToken = tokens[index];
                    break;
                }
            }
            if (match == false)
            {
                foreach (HostCall function in m_Engine.HostBridge.HostCallTable.Values)
                {
                    if (function.ReturnType == GossipType.ReferenceObject)
                    {
                        // First check if we are going to encounter an assignment
                        Int32 enounterAssignmentIndex = 0;
                        if (EncounterAssignment(index, tokens, ref enounterAssignmentIndex))
                        {
                            return ParsePropertySet(compilationContext, index, tokens, ref currentParent,
                                                    enounterAssignmentIndex);
                        }

                        Expression expression;
                        int skip = ParseExpression(index, tokens, compilationContext, out expression);
                        if (tokens[index + skip].Type == TokenType.ReferenceAction)
                        {
                            index = index + skip;
                            currentToken = tokens[index];

                            // Create Action
                            ActionBinding action =
                                compilationContext.TypeBindingTable.GetActionByName(currentToken.Value,
                                                                                    expression.HostReturnType);
                            ICustomActionNode node = action.CreateActionNode();

                            List<Expression> expressions;
                            skip = ParseActionParameters(index + 1, tokens, compilationContext, out expressions);

                            if (action.NumParameters != expressions.Count)
                                throw new GossipScriptParameterException(
                                    String.Format("Invalid number of parameters for action:{0} found:{1} expected:{2}",
                                                  action.Name, expressions.Count, action.NumParameters));

                            var customActionContext = new CustomActionContext(expressions, expression);

                            currentParent.Next = new ExecuteCustomActionNode(node, customActionContext);
                            currentParent = currentParent.Next;

                            index += (skip + 1);
                            currentToken = tokens[index];
                            return index;
                        }

                        throw new NotImplementedException(
                            "TODO anything that returns a reference might want to be invoked");
                    }
                }
            }

            if (match == false)
                throw new GossipScriptException("Encountered Unknown Token", currentToken);

            return index;
        }

        private int ParsePropertySet(CompilationContext compilationContext, int index, List<Token> tokens,
                                     ref IActionNode currentParent, int enounterAssignmentIndex)
        {
            Expression lhsExpression;
            int lhsSkip = ParseExpression(index, tokens, compilationContext, out lhsExpression);

            Token propertyToken = tokens[enounterAssignmentIndex - 1];
            if (propertyToken.Type != TokenType.ReferencePropertySet)
                throw new GossipScriptException("Expected property set");

            // Get Set Method
            PropertySetBinding setMethod = m_Engine.HostBridge.TypeBindingTable.GetPropertySetByName(
                propertyToken.Value,
                lhsExpression.HostReturnType);

            // Skip Assignment 
            Token currentToken = tokens[enounterAssignmentIndex];
            if (currentToken.Type != TokenType.Assignment)
                throw new Exception("Exprected assignment");

            index = enounterAssignmentIndex + 1;

            // Parse Right hand side
            Expression rhsExpression;
            int rhsSkip = ParseExpression(index, tokens, compilationContext, out rhsExpression);

            currentParent.Next = new AssignReferenceProperty(lhsExpression, setMethod.Id, rhsExpression);
            currentParent = currentParent.Next;

            return index + rhsSkip;
        }

        private IActionNode ParseExit(IActionNode currentParent, List<Token> tokens, ref int index)
        {
            Token currentToken;
            var conditionalCommand = new ExitNode();
            currentParent.Next = conditionalCommand;
            currentParent = currentParent.Next;

            currentToken = tokens[++index];
            AssertExpectedTokenType(currentToken, TokenType.EndStatement, "Expected ;");
            return currentParent;
        }

        private IActionNode ParseReturn(IActionNode currentParent, List<Token> tokens, ref int index)
        {
            Token currentToken;
            var conditionalCommand = new ReturnNode();
            currentParent.Next = conditionalCommand;
            currentParent = currentParent.Next;

            currentToken = tokens[++index];
            AssertExpectedTokenType(currentToken, TokenType.EndStatement, "Expected ;");
            return currentParent;
        }

        private IActionNode ParseYeild(IActionNode currentParent, List<Token> tokens, ref int index)
        {
            Token currentToken;
            var conditionalCommand = new YeildNode();
            currentParent.Next = conditionalCommand;
            currentParent = currentParent.Next;

            currentToken = tokens[++index];
            AssertExpectedTokenType(currentToken, TokenType.EndStatement, "Expected ;");
            return currentParent;
        }

        private int ParseConditional(CompilationContext compilationContext, List<Token> tokens, int index,
                                     ref IActionNode currentParent)
        {
            Token currentToken;
            currentToken = tokens[++index]; // Open Bracket
            AssertExpectedTokenValue(currentToken, "(", "Expected Open Bracket");

            currentToken = tokens[++index]; // Parameters

            // Parse logical expression
            Expression expression;
            int skipTokens = ParseExpression(index - 1, tokens, compilationContext, out expression);
            index += (skipTokens - 2);
            currentToken = tokens[index]; // Close Bracket
            AssertExpectedTokenValue(currentToken, ")", "Expected Close Bracket");

            // Obtain consequence block
            var blockStream = new List<Token>();
            skipTokens = GetInnerBlock(index, tokens, ref blockStream);

            // Compile consequence
            IActionNode consequnce = Compile(blockStream, compilationContext);

            // Jump ahead in the Token stream
            index += skipTokens;
            if (index >= tokens.Count)
                throw new GossipScriptException("Unexpected end of input");

            currentToken = tokens[index];

            // Obtain Else consequence
            IActionNode elseConsequnce = null;
            if (tokens[index + 1].Value == "else")
            {
                var elseBlockStream = new List<Token>();
                skipTokens = GetInnerBlock(index + 1, tokens, ref elseBlockStream);
                elseConsequnce = Compile(elseBlockStream, compilationContext);

                // Jump ahead in the Token stream
                index += (skipTokens + 1);
                currentToken = tokens[index];
            }

            var conditionalCommand = new ConditionalNode(expression, consequnce, elseConsequnce);
            currentParent.Next = conditionalCommand;
            currentParent = currentParent.Next;
            return index;
        }

        private int ParseWait(List<Token> tokens, int index, ref IActionNode currentParent)
        {
            Token currentToken;
            currentToken = tokens[++index]; // Open Bracket
            AssertExpectedTokenValue(currentToken, "(", "Expected Open Bracket");

            currentToken = tokens[++index]; // Parameters

            List<string> parameters = ParseParameters(currentToken.Value);
            int ms = Int32.Parse(parameters[0]);
            currentParent.Next = new WaitNode(ms) {Next = new ActionNode()};
            currentParent = currentParent.Next;

            currentToken = tokens[++index]; // Close Bracket
            AssertExpectedTokenValue(currentToken, ")", "Expected Close Bracket");

            return index;
        }

        private int ParseReferenceObjectAccess(CompilationContext compilationContext, Token currentToken,
                                               List<Token> tokens, int index, ref IActionNode currentParent)
        {
            int varIndex = currentToken.GetAccessIndex(m_Engine.HostBridge.EnumDefineTable);

            // Map enums to their index counterparts
            if (currentToken.Type == TokenType.ReferenceObjectAccessEnum)
                currentToken.Type = TokenType.ReferenceObjectAccess;

            currentToken = tokens[++index]; // Assignment or Property set or Action only

            Token operand = currentToken;

            Expression expression;

            if (currentToken.Type == TokenType.ReferencePropertySet)
            {
                currentToken = tokens[++index];
                AssertExpectedTokenType(currentToken, TokenType.Assignment, "Unexpected token");
                currentToken = tokens[++index];
                int skip = ParseExpression(index, tokens, compilationContext, out expression);
                index += skip;
                currentToken = tokens[index];
                ReferenceAssignment assignment = compilationContext.ReferenceAssignmentTable.GetAssignment(varIndex);

                if (!m_Engine.HostBridge.TypeBindingTable.SetPropertyExists(operand.Value, assignment.Type))
                    throw new GossipScriptException(String.Format("Property {0} not found or is read only.",
                                                                  operand.Value));

                PropertySetBinding setter = m_Engine.HostBridge.TypeBindingTable.GetPropertySetByName(operand.Value,
                                                                                                      assignment.Type);

                if (setter.ParameterType != expression.ReturnType)
                    throw new GossipScriptException("Incompatile property set parameter");

                currentParent.Next = new AssignReferenceProperty(varIndex, setter.Id, expression);
                currentParent = currentParent.Next;
                return index;
            }

            if (currentToken.Type == TokenType.Assignment)
            {
                currentToken = tokens[++index];
                int skip = ParseExpression(index, tokens, compilationContext, out expression);
                index += skip;
                currentToken = tokens[index];

                compilationContext.ReferenceAssignmentTable.TrackReference(varIndex, expression.HostReturnType);

                if (expression.ReturnType != GossipType.ReferenceObject)
                    throw new GossipScriptException("Expected return type reference object");

                // Create the assignment command
                currentParent.Next = new AssignReference(varIndex, expression);
                currentParent = currentParent.Next;

                return index;
            }


            // Parse Action
            {
                int skip = ParseExpression(index - 1, tokens, compilationContext, out expression);
                index += (skip);
                currentToken = tokens[index - 1];
                if (currentToken.Type == TokenType.ReferenceAction)
                {
                    // Create an action
                    ReferenceAssignment actionType = compilationContext.ReferenceAssignmentTable.GetAssignment(varIndex);
                    ActionBinding action = compilationContext.TypeBindingTable.GetActionByName(currentToken.Value,
                                                                                               actionType.Type);
                    ICustomActionNode node = action.CreateActionNode();

                    int refIndex = varIndex; // Unless the expression result is 
                    List<Expression> expressions;
                    skip = ParseActionParameters(index, tokens, compilationContext, out expressions);

                    if (action.NumParameters != expressions.Count)
                        throw new GossipScriptParameterException(
                            String.Format("Invalid number of parameters for action:{0} found:{1} expected:{2}",
                                          action.Name, expressions.Count, action.NumParameters));

                    var customActionContext = new CustomActionContext(expressions, refIndex);

                    currentParent.Next = new ExecuteCustomActionNode(node, customActionContext);
                    currentParent = currentParent.Next;

                    index += (skip + 1);
                    currentToken = tokens[index];
                    return index;
                }
            }

            throw new GossipScriptException("Unhandled operand");
        }

        //private int ParseModuleAccess(CompilationContext compilationContext, int index, List<Token> tokens, ref IActionNode currentParent)
        //{
        //    var nextToken = tokens[index + 1].Type;

        //    if (nextToken == TokenType.ModulePropertySet)
        //    {
        //        if (tokens[index + 2].Type != TokenType.Assignment)
        //            throw new GossipScriptException("Expected assignment operator");

        //        Expression expression;
        //        int skip = ParseExpression(index + 3, tokens, compilationContext, out expression);

        //        ModuleBinding module = m_Engine.HostBridge.ModuleBindingTable.GetModuleByName(tokens[index].Value);
        //        PropertySetBinding property = m_Engine.HostBridge.ModuleBindingTable.GetPropertySetByName(tokens[index + 1].Value, module.ModuleType);

        //        currentParent.Next = new AssignModuleProperty(module.Id, property.Id, expression);
        //        currentParent = currentParent.Next;

        //        return skip + 2;
        //    }
        //    else
        //    {
        //        if (nextToken == TokenType.ModuleMethodCall)
        //        {
        //            Expression expression;
        //            var skip = ParseExpression(index + 1, tokens, compilationContext, out expression);
        //            currentParent.Next = new EvalAction(expression);
        //            currentParent = currentParent.Next;

        //            return skip + 1;

        //            //throw new NotImplementedException("TODO");
        //        }
        //        else
        //        {
        //            // First check if we are going to encounter an assignment
        //            Int32 enounterAssignmentIndex = 0;
        //            if (EncounterAssignment(index, tokens, ref enounterAssignmentIndex))
        //            {
        //                return ParsePropertySet(compilationContext, index, tokens, ref currentParent,
        //                                        enounterAssignmentIndex);
        //            }

        //            Expression expression;
        //            int skip = ParseExpression(index, tokens, compilationContext, out expression);

        //            index += (skip);
        //            Token currentToken = tokens[index];
        //            if (currentToken.Type == TokenType.ReferenceAction)
        //            {
        //                ActionBinding action = compilationContext.TypeBindingTable.GetActionByName(currentToken.Value, compilationContext.ScriptOwnerType);
        //                ICustomActionNode node = action.CreateActionNode();

        //                List<Expression> expressions;
        //                skip = ParseActionParameters(index + 1, tokens, compilationContext, out expressions);

        //                if (action.NumParameters != expressions.Count)
        //                    throw new GossipScriptParameterException(String.Format("Invalid number of parameters for action:{0} found:{1} expected:{2}",action.Name, expressions.Count, action.NumParameters));

        //                var customActionContext = new CustomActionContext(expressions, expression);

        //                currentParent.Next = new ExecuteCustomActionNode(node, customActionContext);
        //                currentParent = currentParent.Next;

        //                index += (skip + 1);
        //                currentToken = tokens[index];
        //                return index;
        //            }

        //        }
        //    }

        //    throw new Exception("Unexpected expression result after module access");
        //}

        private Boolean EncounterAssignment(Int32 index, List<Token> tokens, ref Int32 encounterIndex)
        {
            while (tokens[index].Type != TokenType.EndStatement &&
                   tokens[index].Type != TokenType.EndOfProgram &&
                   tokens[index].Type != TokenType.OpenCurlyBrace)
            {
                if (tokens[index].Type == TokenType.Assignment)
                {
                    encounterIndex = index;
                    return true;
                }

                index++;
            }
            return false;
        }

        private int ParseEvent(CompilationContext compilationContext, List<Token> tokens, int index,
                               ref IActionNode currentParent)
        {
            Token currentToken = tokens[++index];

            if (currentToken.Type == TokenType.ReferenceAction)
            {
                throw new GossipScriptException("Only Properties may be called on an event parameter");
            }

            if (currentToken.Type == TokenType.ReferencePropertySet)
            {
                Token operand = currentToken;
                Expression expression;
                currentToken = tokens[++index];
                AssertExpectedTokenType(currentToken, TokenType.Assignment, "Unexpected token");
                currentToken = tokens[++index];
                int skip = ParseExpression(index, tokens, compilationContext, out expression);
                index += skip;
                currentToken = tokens[index];

                if (!m_Engine.HostBridge.TypeBindingTable.SetPropertyExists(operand.Value, compilationContext.EventType))
                    throw new GossipScriptException(String.Format("Property {0} not found or is read only.",
                                                                  operand.Value));

                PropertySetBinding setter = m_Engine.HostBridge.TypeBindingTable.GetPropertySetByName(operand.Value,
                                                                                                      compilationContext
                                                                                                          .EventType);

                if (setter.ParameterType != expression.ReturnType)
                    throw new GossipScriptException("Incompatile property set parameter");

                currentParent.Next = new AssignReferenceProperty(-2, setter.Id, expression);
                currentParent = currentParent.Next;
                return index;
            }

            throw new Exception("Unexpected token");
        }

        private int ParseSelf(CompilationContext compilationContext, List<Token> tokens, int index,
                              ref IActionNode currentParent)
        {
            Token currentToken = tokens[++index];

            if (currentToken.Type == TokenType.ReferenceAction)
            {
                // Create an action
                ActionBinding action = compilationContext.TypeBindingTable.GetActionByName(currentToken.Value,
                                                                                           compilationContext
                                                                                               .ScriptOwnerType);
                ICustomActionNode node = action.CreateActionNode();

                List<Expression> expressions;
                int skip = ParseActionParameters(index + 1, tokens, compilationContext, out expressions);

                if (action.NumParameters != expressions.Count)
                    throw new GossipScriptParameterException(String.Format("Invalid number of parameters for action:{0} found:{1} expected:{2}", action.Name,expressions.Count, action.NumParameters));

                var customActionContext = new CustomActionContext(expressions);

                currentParent.Next = new ExecuteCustomActionNode(node, customActionContext);
                currentParent = currentParent.Next;

                index += (skip + 1);
                currentToken = tokens[index];
                return index;
            }

            if (currentToken.Type == TokenType.ReferencePropertySet)
            {
                Token operand = currentToken;
                Expression expression;
                currentToken = tokens[++index];
                AssertExpectedTokenType(currentToken, TokenType.Assignment, "Unexpected token");
                currentToken = tokens[++index];
                int skip = ParseExpression(index, tokens, compilationContext, out expression);
                index += skip;
                currentToken = tokens[index];

                if (
                    !m_Engine.HostBridge.TypeBindingTable.SetPropertyExists(operand.Value,
                                                                            compilationContext.ScriptOwnerType))
                    throw new GossipScriptException(String.Format("Property {0} not found or is read only.",
                                                                  operand.Value));

                PropertySetBinding setter = m_Engine.HostBridge.TypeBindingTable.GetPropertySetByName(operand.Value,
                                                                                                      compilationContext
                                                                                                          .ScriptOwnerType);

                if (setter.ParameterType != expression.ReturnType)
                    throw new GossipScriptException("Incompatile property set parameter");

                currentParent.Next = new AssignReferenceProperty(-1, setter.Id, expression);
                currentParent = currentParent.Next;
                return index;
            }

            // Parse Action
            {
                // First check if we are going to encounter an assignment
                Int32 enounterAssignmentIndex = 0;
                if (EncounterAssignment(index, tokens, ref enounterAssignmentIndex))
                {
                    return ParsePropertySet(compilationContext, index - 1, tokens, ref currentParent,
                                            enounterAssignmentIndex);
                }

                Expression expression;
                int skip = ParseExpression(index - 1, tokens, compilationContext, out expression);

                index += (skip);
                currentToken = tokens[index - 1];
                if (currentToken.Type == TokenType.ReferenceAction)
                {
                    // Create an action
                    ActionBinding action = compilationContext.TypeBindingTable.GetActionByName(currentToken.Value,
                                                                                               expression.HostReturnType);
                    ICustomActionNode node = action.CreateActionNode();

                    List<Expression> expressions;
                    skip = ParseActionParameters(index, tokens, compilationContext, out expressions);

                    if (action.NumParameters != expressions.Count)
                        throw new GossipScriptParameterException(
                            String.Format("Invalid number of parameters for action:{0} found:{1} expected:{2}",
                                          action.Name, expressions.Count, action.NumParameters));

                    var customActionContext = new CustomActionContext(expressions, expression);

                    currentParent.Next = new ExecuteCustomActionNode(node, customActionContext);
                    currentParent = currentParent.Next;

                    index += (skip + 1);
                    currentToken = tokens[index];
                    return index;
                }
            }

            throw new Exception("Unexpected token");
        }

        private int ParseFlagAccess(CompilationContext compilationContext, Token currentToken, List<Token> tokens,
                                    int index, ref IActionNode currentParent)
        {
            int varIndex = currentToken.GetAccessIndex(m_Engine.HostBridge.EnumDefineTable);

            // Map enums to their index counterparts
            if (currentToken.Type == TokenType.LocalFlagAccessEnum)
                currentToken.Type = TokenType.LocalFlagAccess;
            if (currentToken.Type == TokenType.GlobalFlagAccessEnum)
                currentToken.Type = TokenType.GlobalFlagAccess;

            // Global or local?
            var op = TokenType.GlobalFlagAccess;
            if (currentToken.Type == TokenType.LocalFlagAccess)
                op = TokenType.LocalFlagAccess;

            currentToken = tokens[++index]; // Assignment
            AssertExpectedTokenType(currentToken, TokenType.Assignment, "Expected Assignment");

            currentToken = tokens[++index];

            Expression expression;
            int skip = ParseExpression(index, tokens, compilationContext, out expression);

            index += skip;
            currentToken = tokens[index];

            // Create the assignment command
            currentParent.Next = new ModifyFlag(varIndex, op, expression);
            currentParent = currentParent.Next;

            return index;
        }

        private int ParseVariableAccess(CompilationContext compilationContext, Token currentToken, List<Token> tokens,
                                        int index, ref IActionNode currentParent)
        {
            int varIndex = currentToken.GetAccessIndex(m_Engine.HostBridge.EnumDefineTable);

            // Map enums to their index counterparts
            if (currentToken.Type == TokenType.LocalVariableAccessEnum)
                currentToken.Type = TokenType.LocalVariableAccess;
            if (currentToken.Type == TokenType.GlobalVariableAccessEnum)
                currentToken.Type = TokenType.GlobalVariableAccess;

            // Global or local?
            var op = TokenType.GlobalVariableAccess;
            if (currentToken.Type == TokenType.LocalVariableAccess)
                op = TokenType.LocalVariableAccess;

            currentToken = tokens[++index]; // Assignment or increment or decrement
            if (currentToken.Value != "=" && currentToken.Value != "++" && currentToken.Value != "--" &&
                currentToken.Value != "+=" && currentToken.Value != "-=")
                throw new GossipScriptException("Unexpected Token following variable access", currentToken);

            if (currentToken.Value == "++")
            {
                currentParent.Next = new ModifyVariable(varIndex, op, TokenType.Increment, 1);
                currentParent = currentParent.Next;
                return index;
            }

            if (currentToken.Value == "--")
            {
                currentParent.Next = new ModifyVariable(varIndex, op, TokenType.Decrement, 1);
                currentParent = currentParent.Next;
                return index;
            }

            if (currentToken.Type == TokenType.Assignment || currentToken.Type == TokenType.IncrementAndAssign ||
                currentToken.Type == TokenType.DecrementAndAssign)
            {
                TokenType tokenType = currentToken.Type;
                currentToken = tokens[++index]; // First token after assignment

                Expression expression;
                int skip = ParseExpression(index, tokens, compilationContext, out expression);
                if (expression.ReturnType != GossipType.Number)
                    throw new GossipScriptException(String.Format("Invalid return type:{0}, Expected Number",
                                                                  expression.ReturnType));

                index += skip;
                currentToken = tokens[index];

                // Specal case
                if (op == TokenType.LocalVariableAccess && currentToken.Type == TokenType.Assignment &&
                    expression.Instructions.Count == 1)
                {
                    currentParent.Next = new AssignLocalVariableWithLiteral(varIndex,
                                                                            Convert.ToDouble(
                                                                                expression.Instructions[0].Data));
                    currentParent = currentParent.Next;

                    return index;
                }

                // Generic Case
                currentParent.Next = new AssignVariable(varIndex, op, tokenType, expression);
                currentParent = currentParent.Next;

                return index;
            }
            return index;
        }

        private int ParseStringAccess(CompilationContext compilationContext, Token currentToken, List<Token> tokens,
                                      int index, ref IActionNode currentParent)
        {
            int destIndex = currentToken.GetAccessIndex(m_Engine.HostBridge.EnumDefineTable);

            // Map enums to their index counterparts
            if (currentToken.Type == TokenType.LocalStringAccessEnum)
                currentToken.Type = TokenType.LocalStringAccess;
            if (currentToken.Type == TokenType.GlobalFlagAccessEnum)
                currentToken.Type = TokenType.GlobalFlagAccess;

            // Global or local?
            var destScope = VariableScope.Global;

            if (currentToken.Type == TokenType.LocalStringAccess)
                destScope = VariableScope.Local;

            currentToken = tokens[++index];
            if (currentToken.Type != TokenType.Assignment)
                throw new GossipScriptException("Unexpected Token following string access", currentToken);

            Token op = currentToken;
            currentToken = tokens[++index];

            if (currentToken.Type == TokenType.StringLiteral)
            {
                string stringValue = currentToken.Value.Substring(1, currentToken.Value.Length - 2);
                currentParent.Next = new AssignStringLiteral(destIndex, destScope, stringValue);
                currentParent = currentParent.Next;
                return index;
            }

            // If none of the above it must be an expression
            Expression expression;
            int skipTokens = ParseExpression(index, tokens, compilationContext, out expression);
            index += skipTokens;
            currentToken = tokens[index];
            currentParent.Next = new AssignString(destIndex, destScope, expression);
            currentParent = currentParent.Next;
            return index;
        }

        private int ParseParallelBlock(CompilationContext compilationContext, int index, List<Token> tokens,
                                       ref IActionNode currentParent)
        {
            Token currentToken;
            var blockStream = new List<Token>();
            int skipTokens = GetInnerBlock(index, tokens, ref blockStream);

            // Compile consequence
            IActionNode consequnce = Compile(blockStream, compilationContext);
            index += skipTokens;
            currentToken = tokens[index];

            var conditionalCommand = new ParallelNode(consequnce);
            currentParent.Next = conditionalCommand;
            currentParent = currentParent.Next;
            return index;
        }

        private int ParseEvaluateExpression(CompilationContext compilationContext, List<Token> tokens, int index,
                                            ref IActionNode currentParent)
        {
            Token currentToken = tokens[++index];
            AssertExpectedTokenValue(currentToken, "(", "Expected Open Bracket");

            currentToken = tokens[++index]; // Parameters

            // Parse logical expression
            Expression expression;
            int skipTokens = ParseExpression(index - 1, tokens, compilationContext, out expression);
            index += (skipTokens - 2);
            currentToken = tokens[index]; // Close Bracket
            AssertExpectedTokenValue(currentToken, ")", "Expected Close Bracket");

            var conditionalCommand = new EvalAction(expression);
            currentParent.Next = conditionalCommand;
            currentParent = currentParent.Next;
            return index;
        }

        private Int32 ParseFunctionParameterExpressions(Int32 startIndex, List<Token> tokenStream,
                                                        CompilationContext compilationContext, out List<Expression> rv)
        {
            rv = new List<Expression>();

            int currentIndex = startIndex;
            var positionStack = new Stack<Int32>();
            Token currentToken = tokenStream[startIndex];
            int position = startIndex;
            while (currentToken.Type != TokenType.EndStatement)
            {
                if (currentToken.Type == TokenType.OpenBracket)
                {
                    positionStack.Push(currentIndex);
                }
                if (currentToken.Type == TokenType.CloseBracket)
                {
                    position = positionStack.Pop();
                }
                if ((currentToken.Type == TokenType.FunctionArgumentSeperator && positionStack.Count == 1) ||
                    (currentToken.Type == TokenType.CloseBracket && positionStack.Count == 0))
                {
                    if (position == startIndex)
                        position += 1;

                    var tokens = new List<Token>();
                   
                    for (int i = position + 1; i < currentIndex; i++)
                    {
                        tokens.Add(tokenStream[i]);
                    }
                    if (tokens.Count > 0)
                    {
                        tokens.Add(new Token(TokenType.EndStatement, OperationType.None, "", new TokenPosition(0, 0, 0)));
                        Expression expression;
                        ParseExpression(0, tokens, compilationContext, out expression);
                        rv.Add(expression);

                        if (positionStack.Count == 1)
                        {
                            positionStack.Pop();
                            positionStack.Push(currentIndex);
                            position = currentIndex;
                        }
                    }
                }

                currentToken = tokenStream[++currentIndex];
            }

            return currentIndex - startIndex;
        }

        /// <summary>
        ///     Returns number of tokens to skip
        /// </summary>
        private Int32 ParseExpression(Int32 startIndex, List<Token> tokenStream, CompilationContext compilationContext, out Expression rv)
        {
            var countTokensToProcess = 0;
            int currentIndex = startIndex;
            var expressionTokenstream = new List<Token>();
            var currentToken = tokenStream[startIndex];
            while (currentToken.Type != TokenType.OpenCurlyBrace &&
                   currentToken.Type != TokenType.EndStatement &&
                   currentToken.Type != TokenType.EndOfProgram &&
                   currentToken.Type != TokenType.ReferenceAction &&
                   currentToken.Type != TokenType.ReferencePropertySet)
            {
                countTokensToProcess++;
                expressionTokenstream.Add(currentToken);
                currentToken = tokenStream[++currentIndex];
            }

            var expression = ExpressionParser.Parse(expressionTokenstream, compilationContext);

            rv = expression;
            return countTokensToProcess;
        }

        /// <summary>
        ///     For testing expression parsing only
        /// </summary>
        public Expression CompileExpression(string input, bool normalize = true)
        {
            var tokens = Lexer.Tokenize(input).ToList();
            var commandNodeRoot = new ActionNodeRoot();

            // Build string tables
            foreach (Token token in tokens)
            {
                if (token.Type == TokenType.StringLiteral)
                {
                    string data = token.Value.Substring(1, token.Value.Count() - 2);
                    commandNodeRoot.StringTable.RegisterString(data);
                }
            }

            var compilationContext = new CompilationContext(m_HostCallTable,
                                                            commandNodeRoot.StringTable,
                                                            m_Engine.HostBridge.EnumDefineTable,
                                                            m_Engine.HostBridge.TypeBindingTable,
                                                            m_Engine.HostBridge.EventBindingTable);

            Expression expression = ExpressionParser.Parse(tokens, compilationContext, normalize);
            return expression;
        }

        public CompiledScript CompileScript(String text, StringTable stringTable, Type self = null)
        {
            var machineInfo = m_StateMachineParser.ParseStateMachineInfo(text);
            var nodeList = new List<ScriptNode>();

            foreach(var node in machineInfo.Nodes)
            {
                var scriptNode = CompileScriptNode(node.Name, node.Text, stringTable, self);
                nodeList.Add(scriptNode);
            }
            
            // var stringTable = ScriptNode.StringTable;
            var rv = new CompiledScript(nodeList, self);

            return rv;
        }

        public ScriptNode CompileScriptNode(string nodeName, string text, StringTable stringTable, Type self)
        {
            if (String.IsNullOrEmpty(text))
                throw new GossipScriptException("Script cannot be null or empty");

            var scriptletInfo = ScriptNodeParser.ParseScriptNode(text);

            var rv = new ScriptNode(nodeName);


            if (scriptletInfo.OnUpdateScript != null)
            {
                var tokens = Lexer.Tokenize(scriptletInfo.OnUpdateScript.EventScript).ToList();
                PopulateStringTable(tokens, stringTable);
                rv.OnUpdateEvent = new ScriptEvent(CompileEvent(tokens, self, stringTable), 0);
            }

            if (scriptletInfo.OnExitScript != null)
            {
                List<Token> tokens = Lexer.Tokenize(scriptletInfo.OnExitScript.EventScript).ToList();
                PopulateStringTable(tokens, stringTable);
                rv.OnEndEvent = new ScriptEvent(CompileEvent(tokens, self, stringTable), 0);
            }

            if (scriptletInfo.OnEnterScript != null)
            {
                List<Token> tokens = Lexer.Tokenize(scriptletInfo.OnEnterScript.EventScript).ToList();
                PopulateStringTable(tokens, stringTable);
                rv.OnStartEvent = new ScriptEvent(CompileEvent(tokens, self, stringTable), 0);
            }

            if (scriptletInfo.OnInterruptScript != null)
            {
                List<Token> tokens = Lexer.Tokenize(scriptletInfo.OnInterruptScript.EventScript).ToList();
                PopulateStringTable(tokens, stringTable);
                rv.OnInterruptEvent = new ScriptEvent(CompileEvent(tokens, self, stringTable), 0);
            }

            // Custom events
            foreach (ScriptNodeEventInfo customEvent in scriptletInfo.CustomEventScripts)
            {
                List<Token> tokens = Lexer.Tokenize(customEvent.EventScript).ToList();
                PopulateStringTable(tokens, stringTable);

                EventBinding binding = m_Engine.HostBridge.EventBindingTable.GetBindingByName(customEvent.Token.Value);
                var scriptletEvent =
                    new ScriptEvent(CompileEvent(tokens, self, stringTable, binding.EventParameterType), binding.Id);

                rv.OnCustomEvents.Add(scriptletEvent);
            }

            return rv;
        }

        private static void PopulateStringTable(IEnumerable<Token> tokens, StringTable stringTable)
        {
            foreach (Token token in tokens)
            {
                if (token.Type == TokenType.StringLiteral)
                {
                    string data = token.Value.Substring(1, token.Value.Count() - 2);
                    stringTable.RegisterString(data);
                }
            }
        }


        private ActionNodeRoot CompileEvent(List<Token> tokens, Type selfTarget, StringTable stringTable,
                                            Type eventType = null)
        {
            var commandNodeRoot = new ActionNodeRoot();

            var compilationContext = new CompilationContext(m_HostCallTable,
                                                            stringTable,
                                                            m_Engine.HostBridge.EnumDefineTable,
                                                            m_Engine.HostBridge.TypeBindingTable,
                                                            m_Engine.HostBridge.EventBindingTable);
            compilationContext.ScriptOwnerType = selfTarget;
            compilationContext.EventType = eventType;
            commandNodeRoot.Next = Compile(tokens, compilationContext);

            return commandNodeRoot;
        }

        public Int32 GetInnerBlock(Int32 startIndex, List<Token> tokens, ref List<Token> blockTokenStream)
        {
            int rv = 0;
            int numOpenBraces = 0;
            bool seenOpenBrace = false;
            for (int i = startIndex; i < tokens.Count; i++)
            {
                if (numOpenBraces > 0 && seenOpenBrace)
                    blockTokenStream.Add(tokens[i]);

                if (tokens[i].Value == "{")
                {
                    seenOpenBrace = true;
                    numOpenBraces++;
                }
                if (tokens[i].Value == "}")
                    numOpenBraces--;

                if (numOpenBraces == 0 && seenOpenBrace)
                    break;

                rv++;
            }

            return rv;
        }

        public Int32 ParseActionParameters(Int32 startIndex, List<Token> tokenStream, CompilationContext context,
                                           out List<Expression> expressions)
        {
            if (tokenStream[startIndex].Type != TokenType.OpenBracket)
                throw new GossipScriptException("Expected Open bracket as first token");

            expressions = new List<Expression>();
            int height = 1;
            int currentIndex = startIndex;
            var tokens = new List<Token>();
            for (int i = 1; i < tokenStream.Count; i++)
            {
                currentIndex = i + startIndex;

                if (tokenStream[currentIndex].Type == TokenType.OpenBracket)
                    height++;

                if (tokenStream[currentIndex].Type == TokenType.CloseBracket)
                    height--;

                if (height == 0 || tokenStream[currentIndex].Type == TokenType.FunctionArgumentSeperator)
                {
                    // Parse
                    if (tokens.Count > 0)
                    {
                        Expression expression = ExpressionParser.Parse(tokens, context);
                        expressions.Add(expression);
                        tokens.Clear();
                    }
                }
                else
                {
                    tokens.Add(tokenStream[currentIndex]);
                }

                if (height == 0)
                    break;
            }

            return currentIndex - startIndex;
        }

        public List<String> ParseParameters(String line)
        {
            var rv = new List<String>();

            int startIndex = 0;
            int numQuotes = 0;
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '"')
                    numQuotes++;
                if (line[i] == ',')
                {
                    if (numQuotes == 0 || numQuotes == 2) // Found parameter
                    {
                        rv.Add(line.Substring(startIndex, i - startIndex));
                        numQuotes = 0;
                        startIndex = i + 1;
                    }
                }
            }
            rv.Add(line.Substring(startIndex, line.Length - startIndex));
            return rv;
        }
    }
}