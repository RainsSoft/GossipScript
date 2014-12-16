using System;
using System.Collections.Generic;
using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Exceptions;
using Druids.ScriptingEngine.Exceptions;
using Druids.ScriptingEngine.Host;

namespace Druids.ScriptingEngine.Compiler
{
    internal class ExpressionAnalyzer
    {
        public ExtendedTypeAdapter DetermineReturnType(List<SemanticToken> tokens, CompilationContext context)
        {
            var stack = new Stack<ExtendedTypeAdapter>(tokens.Count);

            foreach (SemanticToken opcode in tokens)
            {
                if (opcode.IsNumber())
                {
                    stack.Push(new ExtendedTypeAdapter(GossipType.Number));
                }
                else
                {
                    // Evaluate opcode
                    switch (opcode.TokenType)
                    {
                        case TokenType.ModuleAccess:
                            {
                                var result = new ExtendedTypeAdapter(GossipType.ReferenceModule);
                                result.Data = opcode.Data; // Module Id

                                stack.Push(result);
                                break;
                            }
                        case TokenType.Event:
                            {
                                var adapter = new ExtendedTypeAdapter(GossipType.ReferenceObject);
                                adapter.Data = -2; // Reference type -2 indicates the event object
                                //  adapter.Target = opcode.Data;
                                stack.Push(adapter);
                                break;
                            }
                        case TokenType.Self:
                            {
                                var adapter = new ExtendedTypeAdapter(GossipType.ReferenceObject);
                                adapter.Data = -1; // Reference type -1 indicates the script owner
                                stack.Push(adapter);
                                break;
                            }
                        case TokenType.Addition:
                            {
                                GossipType rhs = stack.Pop().TypeAdapter.GossipType;
                                GossipType lhs = stack.Pop().TypeAdapter.GossipType;

                                if (rhs == GossipType.String && lhs == GossipType.String)
                                {
                                    opcode.TokenType = TokenType.StringStringConcatination;
                                    stack.Push(new ExtendedTypeAdapter(GossipType.String));
                                    break;
                                }

                                if (rhs == GossipType.String && lhs == GossipType.Number)
                                {
                                    opcode.TokenType = TokenType.DoubleStringConcatination;
                                    stack.Push(new ExtendedTypeAdapter(GossipType.String));
                                    break;
                                }

                                if (rhs == GossipType.Number && lhs == GossipType.String)
                                {
                                    opcode.TokenType = TokenType.StringDoubleConcatination;
                                    stack.Push(new ExtendedTypeAdapter(GossipType.String));
                                    break;
                                }

                                if (rhs == GossipType.Number && lhs == GossipType.Number)
                                {
                                    stack.Push(new ExtendedTypeAdapter(GossipType.Number));
                                    break;
                                }

                                throw new GossipScriptException(
                                    String.Format("Invalid operands: {0},{1} on operator:{2}", rhs, lhs,
                                                  opcode.TokenType));

                                break;
                            }
                        case TokenType.HostFunctionCall:
                            {
                                var functionId = (Int32) opcode.Data;
                                HostCall function = context.HostCallTable.GetFunctionById(functionId);
                                for (int i = 0; i < function.NumParameters; i++)
                                {
                                    stack.Pop(); // TODO Check function parameter types are expected
                                }
                                stack.Push(new ExtendedTypeAdapter(function.ReturnType, function.HostType));
                                break;
                            }
                        case TokenType.Negation:
                            {
                                GossipType rhs = stack.Peek().TypeAdapter.GossipType;
                                if (rhs != GossipType.Number)
                                    throw new GossipScriptException(String.Format("Invalid operator:{0} on string",
                                                                                  opcode.TokenType));

                                break;
                            }

                        case TokenType.Subtract:
                        case TokenType.Multiply:
                        case TokenType.Divide:
                        case TokenType.PowerOf:
                        case TokenType.UnaryMinus:
                        case TokenType.Modulo:
                        case TokenType.LogicalAnd:
                        case TokenType.LogicalOr:
                            {
                                GossipType rhs = stack.Pop().TypeAdapter.GossipType;
                                GossipType lhs = stack.Pop().TypeAdapter.GossipType;
                                if (rhs == GossipType.String || lhs == GossipType.String)
                                    throw new GossipScriptException(String.Format("Invalid operator:{0} on string",
                                                                                  opcode.TokenType));

                                stack.Push(new ExtendedTypeAdapter(GossipType.Number));

                                break;
                            }
                        case TokenType.Equal:
                        case TokenType.NotEqual:
                        case TokenType.GreaterThan: //  TODO Implement for strings
                        case TokenType.LessThan: // TODO Implement for strings
                        case TokenType.GreaterThanOrEqualTo: //  TODO Implement for strings
                        case TokenType.LessThanOrEqualTo: //  TODO Implement for strings
                            {
                                GossipType rhs = stack.Pop().TypeAdapter.GossipType;
                                GossipType lhs = stack.Pop().TypeAdapter.GossipType;
                                if (rhs == GossipType.String && lhs == GossipType.String)
                                {
                                    opcode.TokenType = OperatorToStringOperator(opcode.TokenType);
                                    stack.Push(new ExtendedTypeAdapter(GossipType.Number));
                                    break;
                                }
                                if (rhs == GossipType.Number && lhs == GossipType.Number)
                                {
                                    stack.Push(new ExtendedTypeAdapter(GossipType.Number));
                                    break;
                                }
                                if (rhs == GossipType.ReferenceObject &&
                                    lhs == GossipType.ReferenceObject)
                                {
                                    opcode.TokenType = OperatorToReferenceOperator(opcode.TokenType);
                                    stack.Push(new ExtendedTypeAdapter(GossipType.Number));
                                    break;
                                }

                                throw new GossipScriptException(
                                    String.Format("Invalid operands: {0},{1} on operator:{2}", rhs, lhs,
                                                  opcode.TokenType));
                                break;
                            }
                        case TokenType.LocalVariableAccess:
                        case TokenType.GlobalVariableAccess:
                        case TokenType.GlobalFlagAccess:
                        case TokenType.LocalFlagAccess:
                            {
                                stack.Push(new ExtendedTypeAdapter(GossipType.Number));
                                break;
                            }
                        case TokenType.ReferenceObjectAccess:
                            {
                                var result = new ExtendedTypeAdapter(GossipType.ReferenceObject);
                                result.Data = opcode.Data;
                                stack.Push(result);
                                break;
                            }
                        case TokenType.GlobalStringAccess:
                        case TokenType.LocalStringAccess:
                        case TokenType.StringLiteral:
                            {
                                stack.Push(new ExtendedTypeAdapter(GossipType.String));
                                break;
                            }
                        case TokenType.StringEquality:
                            {
                                stack.Push(new ExtendedTypeAdapter(GossipType.Number));
                                break;
                            }
                        case TokenType.StringInequality:
                            {
                                stack.Push(new ExtendedTypeAdapter(GossipType.Number));
                                break;
                            }
                        case TokenType.ReferenceAction:
                            {
                                ExtendedTypeAdapter stackObject = stack.Pop(); // The reference object
                                Type objType = stackObject.TypeAdapter.HostType;
                                var target = (Int32) TargetObjectType.TempReferenceObject; // Target Temp Object

                                if (objType == null) // Unknown object, need to look at the assignment table
                                {
                                    var id = (Int32) stackObject.Data; // Reference Index
                                    if (context.ReferenceAssignmentTable.HasAssignment(id) == false)
                                        throw new GossipScriptException(
                                            String.Format("Ref[{0}] has not been assigned any value", id));

                                    ReferenceAssignment reference = context.ReferenceAssignmentTable.GetAssignment(id);
                                    objType = reference.Type;

                                    target = id; // Ref index
                                }

                                string name = opcode.StringData; // Should be name of Action
                                ActionBinding action = context.TypeBindingTable.GetActionByName(name, objType);
                                opcode.Data = action.Id; // write back the id

                                // Pop off number of parameters from the stack
                                for (int i = 0; i < action.NumParameters; i++)
                                {
                                    TypeAdapter parameter = stack.Pop().TypeAdapter;
                                    if (action.ParameterInfo[i].GossipType != parameter.GossipType)
                                    {
                                        throw new GossipScriptException(
                                            String.Format(
                                                "Invalid parameter type:{0} for parameter #{1} expected type:{2}",
                                                parameter.GossipType, i + 1, action.ParameterInfo[i].GossipType));
                                    }
                                }

                                var typeAdapter = new ExtendedTypeAdapter(GossipType.Action, objType)
                                    {
                                        Data = opcode.Data, // Action Id
                                        Target = target
                                    };
                                stack.Push(typeAdapter);
                                break;
                            }
                        //case TokenType.ModulePropertyGet:
                        //    {
                        //        ExtendedTypeAdapter stackObject = stack.Pop();
                        //            // This should be the object to call the method on
                        //        var moduleId = (Int32) stackObject.Data;
                        //        ModuleBinding module = context.ModuleBindingTable.GetModuleById(moduleId);

                        //        string name = opcode.StringData; // Should be name of method
                        //        PropertyGetBinding method = context.ModuleBindingTable.GetPropertyGetByName(name,
                        //                                                                                    module
                        //                                                                                        .ModuleType);
                        //        opcode.Data = method.Id; // write back the id

                        //        var typeAdapter = new ExtendedTypeAdapter(method.GossipType, method.HostType,
                        //                                                  (Int32) opcode.Data);
                        //        stack.Push(typeAdapter);
                        //        break;
                        //    }
                        //case TokenType.ModuleMethodCall:
                        //    {
                        //        ExtendedTypeAdapter stackObject = stack.Pop();
                        //            // This should be the object to call the method on
                        //        var moduleId = (Int32) stackObject.Data;
                        //        ModuleBinding module = context.ModuleBindingTable.GetModuleById(moduleId);

                        //        string name = opcode.StringData; // Should be name of method
                        //        MethodBinding method = context.ModuleBindingTable.GetMethodBindingByName(name,
                        //                                                                                 module
                        //                                                                                     .ModuleType);
                        //        opcode.Data = method.Id; // write back the id

                        //        // Pop off number of parameters from the stack
                        //        for (int i = 0; i < method.NumParameters; i++)
                        //        {
                        //            TypeAdapter parameter = stack.Pop().TypeAdapter;
                        //            if (method.ParameterInfo[i].GossipType != parameter.GossipType)
                        //                throw new GossipScriptParameterException(
                        //                    String.Format(
                        //                        "Invalid parameter type:{0} for parameter #{1} expected type:{2}",
                        //                        parameter.GossipType, i + 1, method.ParameterInfo[i].GossipType));
                        //        }
                        //        var typeAdapter = new ExtendedTypeAdapter(method.GossipType, method.HostReturnType,
                        //                                                  (Int32) opcode.Data);
                        //        stack.Push(typeAdapter);
                        //        break;
                        //    }
                        case TokenType.MethodCall:
                            {
                                ExtendedTypeAdapter stackObject = stack.Pop();
                                    // This should be the object to call the method on
                                Type objType = stackObject.TypeAdapter.HostType;

                                if (objType == null) // Unknown object, need to look at the assignment table
                                {
                                    var id = (Int32) stackObject.Data; // Reference Index
                                    if (id == -1)
                                    {
                                        objType = context.ScriptOwnerType;
                                    }
                                    else
                                    {
                                        if (context.ReferenceAssignmentTable.HasAssignment(id) == false)
                                            throw new GossipScriptException(
                                                String.Format("Ref[{0}] has not been assigned any value", id));

                                        ReferenceAssignment reference =
                                            context.ReferenceAssignmentTable.GetAssignment(id);
                                        objType = reference.Type;
                                    }
                                }

                                string name = opcode.StringData; // Should be name of method
                                MethodBinding method = context.TypeBindingTable.GetMethodByName(name, objType);
                                opcode.Data = method.Id; // write back the id

                                // Pop off number of parameters from the stack
                                for (int i = 0; i < method.NumParameters; i++)
                                {
                                    TypeAdapter parameter = stack.Pop().TypeAdapter;
                                    if (method.ParameterInfo[i].GossipType != parameter.GossipType)
                                    {
                                        throw new GossipScriptParameterException(
                                            String.Format(
                                                "Invalid parameter type:{0} for parameter #{1} expected type:{2}",
                                                parameter.GossipType, i + 1, method.ParameterInfo[i].GossipType));
                                    }
                                }
                                var typeAdapter = new ExtendedTypeAdapter(method.GossipType, method.HostReturnType)
                                    {
                                        Data = opcode.Data
                                    };
                                stack.Push(typeAdapter);
                                break;
                            }
                        case TokenType.ReferencePropertyGet:
                            {
                                ExtendedTypeAdapter stackObject = stack.Pop();
                                Type objType = stackObject.TypeAdapter.HostType;
                                if (objType == null) // Unknown object, need to look at the assignment table
                                {
                                    var id = (Int32) stackObject.Data; // Reference Index
                                    if (id == -1)
                                    {
                                        objType = context.ScriptOwnerType;
                                    }
                                    else
                                    {
                                        if (id == -2)
                                        {
                                            objType = context.EventType;
                                        }
                                        else
                                        {
                                            if (context.ReferenceAssignmentTable.HasAssignment(id) == false)
                                                throw new GossipScriptException(
                                                    String.Format("Ref[{0}] has not been assigned any value", id));

                                            ReferenceAssignment reference =
                                                context.ReferenceAssignmentTable.GetAssignment(id);
                                            objType = reference.Type;
                                        }
                                    }
                                }

                                string index = opcode.StringData; // Property Index
                                PropertyGetBinding property = context.TypeBindingTable.GetPropertyGetByName(index,
                                                                                                            objType);
                                opcode.Data = property.Id; // write back the id

                                stack.Push(new ExtendedTypeAdapter(property.GossipType, property.HostType));
                                break;
                            }
                        default:
                            throw new Exception(String.Format("Unknown operator{0}", opcode.TokenType));
                            break;
                    }
                }
            }

            if (stack.Count > 1)
                throw new Exception("Stack should only have one remaining entry");

            ExtendedTypeAdapter r = stack.Pop();
            return r;
        }

        private TokenType OperatorToStringOperator(TokenType tokenType)
        {
            if (tokenType == TokenType.Equal)
                return TokenType.StringEquality;

            if (tokenType == TokenType.NotEqual)
                return TokenType.StringInequality;

            throw new GossipScriptException(String.Format("No string operation for operator:{0} exists", tokenType));
        }

        private TokenType OperatorToReferenceOperator(TokenType tokenType)
        {
            if (tokenType == TokenType.Equal)
                return TokenType.ReferenceEquality;

            if (tokenType == TokenType.NotEqual)
                return TokenType.ReferenceInEquality;

            throw new GossipScriptException(String.Format("No reference type operation for operator:{0} exists",
                                                          tokenType));
        }
    }
}