using System;
using System.Collections.Generic;
using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Exceptions;
using Druids.ScriptingEngine;
using Druids.ScriptingEngine.Exceptions;
using Druids.ScriptingEngine.Helper;
using Druids.ScriptingEngine.Host;
using Druids.ScriptingEngine.Interfaces;

namespace Druids.ScriptingEngine
{
    public class ExpressionEvaluator
    {
        private readonly HostCallTable m_CallTable;

        public ExpressionEvaluator(HostCallTable callTable)
        {
            m_CallTable = callTable;
        }

        public T Evaluate<T>(Expression expression, ScriptExecutionContext context) where T : class
        {
            if (expression.Instructions.Count == 0)
                return null;

            object result = EvaluateExpression(expression, context);
            return result as T;
        }

        public double EvaluateDouble(Expression expression, ScriptExecutionContext context)
        {
            if (expression.ReturnType != GossipType.Number)
                throw new GossipScriptException("Invalid return type for expression");
            if (expression.Instructions.Count == 0)
                return 0;
            object result = EvaluateExpression(expression, context);
            return (double) result;
        }


        public object EvaluateExpression(Expression expression, ScriptExecutionContext context)
        {
            if (expression.ReturnType == GossipType.Action)
                throw new GossipScriptRuntimeException("Cannot evaluate expression with return type action which cannot be converted to bool.");

            Stack<double> stack = expression.EvaluationStack;
            Stack<string> stringStack = expression.StringEvaluationStack;
            Stack<object> refStack = expression.ReferenceObjectEvaluationStack;

            foreach (Instruction opcode in expression.Instructions)
            {
                if (opcode.IsNumber())
                {
                    stack.Push(opcode.Data);
                }
                else
                {
                    // Evaluate opcode
                    switch (opcode.TokenType)
                    {
                        case TokenType.StringDoubleConcatination:
                            {
                                double rhs = stack.Pop();
                                string lhs = stringStack.Pop();
                                stringStack.Push(lhs + rhs);
                                break;
                            }
                        case TokenType.DoubleStringConcatination:
                            {
                                string rhs = stringStack.Pop();
                                double lhs = stack.Pop();
                                stringStack.Push(lhs + rhs);
                                break;
                            }
                        case TokenType.StringStringConcatination:
                            {
                                string rhs = stringStack.Pop();
                                string lhs = stringStack.Pop();
                                stringStack.Push(lhs + rhs);
                                break;
                            }
                        case TokenType.Addition:
                            {
                                double rhs = stack.Pop();
                                double lhs = stack.Pop();
                                stack.Push(lhs + rhs);
                                break;
                            }
                        case TokenType.Subtract:
                            {
                                double rhs = stack.Pop();
                                double lhs = stack.Pop();
                                stack.Push(lhs - rhs);
                                break;
                            }
                        case TokenType.Multiply:
                            {
                                double rhs = stack.Pop();
                                double lhs = stack.Pop();
                                stack.Push(lhs*rhs);
                                break;
                            }
                        case TokenType.Divide:
                            {
                                double rhs = stack.Pop();
                                double lhs = stack.Pop();
                                stack.Push(lhs/rhs);
                                break;
                            }
                        case TokenType.PowerOf:
                            {
                                double rhs = stack.Pop();
                                double lhs = stack.Pop();
                                stack.Push(Math.Pow(lhs, rhs));
                                break;
                            }
                        case TokenType.UnaryMinus:
                            {
                                double rhs = stack.Pop();
                                stack.Push(rhs*-1);
                                break;
                            }
                        case TokenType.Modulo:
                            {
                                double rhs = stack.Pop();
                                double lhs = stack.Pop();
                                stack.Push(lhs%rhs);
                                break;
                            }
                        case TokenType.Negation:
                            {
                                double rhs = stack.Pop();
                                int data = (rhs == 0) ? 1 : 0;
                                stack.Push(data);
                                break;
                            }
                        case TokenType.GreaterThan:
                            {
                                double rhs = stack.Pop();
                                double lhs = stack.Pop();
                                int data = (lhs > rhs) ? 1 : 0;
                                stack.Push(data);
                                break;
                            }
                        case TokenType.LessThan:
                            {
                                double rhs = stack.Pop();
                                double lhs = stack.Pop();
                                int data = (lhs < rhs) ? 1 : 0;
                                stack.Push(data);
                                break;
                            }
                        case TokenType.GreaterThanOrEqualTo:
                            {
                                double rhs = stack.Pop();
                                double lhs = stack.Pop();
                                int data = (lhs >= rhs) ? 1 : 0;
                                stack.Push(data);
                                break;
                            }
                        case TokenType.LessThanOrEqualTo:
                            {
                                double rhs = stack.Pop();
                                double lhs = stack.Pop();
                                int data = (lhs <= rhs) ? 1 : 0;
                                stack.Push(data);
                                break;
                            }
                        case TokenType.Equal:
                            {
                                double rhs = stack.Pop();
                                double lhs = stack.Pop();
                                int data = (lhs == rhs) ? 1 : 0;
                                stack.Push(data);
                                break;
                            }
                        case TokenType.NotEqual:
                            {
                                double rhs = stack.Pop();
                                double lhs = stack.Pop();
                                int data = (lhs != rhs) ? 1 : 0;
                                stack.Push(data);
                                break;
                            }
                        case TokenType.LogicalAnd:
                            {
                                double rhs = stack.Pop();
                                double lhs = stack.Pop();
                                bool d1 = (rhs == 0) ? false : true;
                                bool d2 = (lhs == 0) ? false : true;
                                bool d3 = d1 && d2;
                                double data = d3 ? 1.0d : 0.0d;
                                stack.Push(data);
                                break;
                            }
                        case TokenType.LogicalOr:
                            {
                                double rhs = stack.Pop();
                                double lhs = stack.Pop();
                                bool d1 = (rhs == 0) ? false : true;
                                bool d2 = (lhs == 0) ? false : true;
                                bool d3 = d1 || d2;
                                double data = d3 ? 1.0d : 0.0d;
                                stack.Push(data);
                                break;
                            }
                        //case TokenType.ModuleMethodCall:
                        //    {
                        //        var methodId = (Int32) opcode.Data;
                        //        var moduleId = (Int32) stack.Pop();
                        //        ModuleBinding moduleBinding =
                        //            context.Vm.HostBridge.ModuleBindingTable.GetModuleById(moduleId);
                        //        MethodBinding binding =
                        //            context.Vm.HostBridge.ModuleBindingTable.GetMethodBindingById(methodId);
                        //        object[] parameters = GetMethodParameters(binding, stack, refStack, stringStack);

                        //        InvokeBinding(binding, moduleBinding.Module, parameters, expression);
                        //        break;
                        //    }
                        case TokenType.MethodCall:
                            {
                                Object obj;
                                if (refStack.Count > 0)
                                {
                                    obj = refStack.Pop(); // Located in the temp store
                                }
                                else
                                {
                                    var id = (Int32) stack.Pop(); // Should be reference object id
                                    obj = context.LocalState.Ref[id];
                                }

                                var methodId = (Int32) opcode.Data;
                                MethodBinding binding = context.Vm.HostBridge.TypeBindingTable.GetMethodById(methodId);

                                object[] parameters = GetMethodParameters(binding, stack, refStack, stringStack);

                                InvokeBinding(binding, obj, parameters, expression);
                                break;
                            }
                        //case TokenType.ModulePropertyGet:
                        //    {
                        //        var methodId = (Int32) opcode.Data;
                        //        var moduleId = (Int32) stack.Pop();
                        //        ModuleBinding moduleBinding =
                        //            context.Vm.HostBridge.ModuleBindingTable.GetModuleById(moduleId);
                        //        PropertyGetBinding binding =
                        //            context.Vm.HostBridge.ModuleBindingTable.GetPropertyGetById(methodId,
                        //                                                                        moduleBinding.ModuleType);

                        //        InvokeBinding(binding, moduleBinding.Module, null, expression);
                        //        break;
                        //    }
                        case TokenType.ReferencePropertyGet:
                            {
                                Object obj;
                                if (refStack.Count > 0)
                                {
                                    obj = refStack.Pop();
                                }
                                else
                                {
                                    var id = (Int32) stack.Pop(); // Should be reference object id
                                    obj = context.LocalState.Ref[id];
                                }

                                var propertyId = (Int32) opcode.Data;
                                PropertyGetBinding binding =
                                    context.Vm.HostBridge.TypeBindingTable.GetPropertyGetById(propertyId, obj.GetType());


                                InvokeBinding(binding, obj, null, expression);
                                break;
                            }
                        case TokenType.HostFunctionCall:
                            {
                                var functionId = (Int32) opcode.Data;
                                HostCall function = m_CallTable.GetFunctionById(functionId);
                                object[] parameters = function.ParameterList;
                                for (int i = parameters.Length - 1; i >= 0; i--)
                                {
                                    // TODO Depends on the parameter type as to which stack is being poped
                                    if (function.ParameterTypeList[i] == typeof (Int32))
                                    {
                                        parameters[i] = (Int32) stack.Pop();
                                    }
                                    else
                                    {
                                        parameters[i] = stack.Pop();
                                    }
                                }

                                switch (function.ReturnType)
                                {
                                    case GossipType.Number:
                                        {
                                            double data = function.Invoke();
                                            stack.Push(data);
                                            break;
                                        }
                                    case GossipType.ReferenceObject:
                                        {
                                            object data = function.InvokeObjectReturnType();
                                            refStack.Push(data);
                                            break;
                                        }
                                    case GossipType.String:
                                        {
                                            var data = function.Invoke<String>();
                                            stringStack.Push(data);
                                            break;
                                        }
                                    default:
                                        throw new GossipScriptException("Unhandled Return type");
                                }

                                break;
                            }
                        case TokenType.GlobalVariableAccess:
                            {
                                var index = (Int32) opcode.Data;
                                double data = context.Vm.GlobalState.Var[index];
                                stack.Push(data);
                                break;
                            }
                        case TokenType.LocalVariableAccess:
                            {
                                var index = (Int32) opcode.Data;
                                double data = context.LocalState.Var[index];
                                stack.Push(data);
                                break;
                            }
                        case TokenType.GlobalFlagAccess:
                            {
                                var index = (Int32) opcode.Data;
                                bool b = context.Vm.GlobalState.Flags[index];
                                int data = b ? 1 : 0;
                                stack.Push(data);
                                break;
                            }
                        case TokenType.LocalFlagAccess:
                            {
                                var index = (Int32) opcode.Data;
                                bool b = context.LocalState.Flags[index];
                                int data = b ? 1 : 0;
                                stack.Push(data);
                                break;
                            }
                        case TokenType.GlobalStringAccess:
                            {
                                var index = (Int32) opcode.Data; // Should be string table id
                                string value = context.Vm.GlobalState.Str[index];
                                stringStack.Push(value);
                                break;
                            }
                        case TokenType.LocalStringAccess:
                            {
                                var index = (Int32) opcode.Data; // Should be string table id
                                string value = context.LocalState.Str[index];
                                stringStack.Push(value);
                                break;
                            }
                        case TokenType.StringLiteral:
                            {
                                var id = (Int32) opcode.Data;
                                string value = context.Vm.StringTable.GetStringById(id);
                                stringStack.Push(value);
                                break;
                            }
                        case TokenType.StringEquality:
                            {
                                string rhs = stringStack.Pop();
                                string lhs = stringStack.Pop();
                                if (rhs == lhs)
                                {
                                    stack.Push(1);
                                }
                                else
                                {
                                    stack.Push(0);
                                }
                                break;
                            }
                        case TokenType.StringInequality:
                            {
                                string rhs = stringStack.Pop();
                                string lhs = stringStack.Pop();
                                if (rhs == lhs)
                                {
                                    stack.Push(0);
                                }
                                else
                                {
                                    stack.Push(1);
                                }
                                break;
                            }
                        case TokenType.ReferenceEquality:
                            {
                                var rhs = (Int32) stack.Pop();
                                var lhs = (Int32) stack.Pop();
                                bool equality = context.LocalState.Ref[rhs] == context.LocalState.Ref[lhs];
                                if (equality)
                                {
                                    stack.Push(1);
                                }
                                else
                                {
                                    stack.Push(0);
                                }
                                break;
                            }
                        case TokenType.ReferenceInEquality:
                            {
                                var rhs = (Int32) stack.Pop();
                                var lhs = (Int32) stack.Pop();
                                bool equality = context.LocalState.Ref[rhs] == context.LocalState.Ref[lhs];
                                if (equality)
                                {
                                    stack.Push(0);
                                }
                                else
                                {
                                    stack.Push(1);
                                }
                                break;
                            }
                        case TokenType.ReferenceObjectAccess:
                            {
                                stack.Push(opcode.Data);
                                break;
                            }
                        case TokenType.Event:
                            refStack.Push(context.CurrentScriptEvent.EventParameter);
                            break;
                        case TokenType.Self:
                            {
                                refStack.Push(context.CurrentScriptEvent.Owner);
                                break;
                            }
                        case TokenType.ModuleAccess:
                            {
                                stack.Push(opcode.Data); // Push the module Id
                                break;
                            }
                        default:
                            throw new Exception(String.Format("Unknown operator{0}", opcode.TokenType));
                            break;
                    }
                }
            }

            if (expression.ReturnType == GossipType.Number)
            {
                if (stack.Count > 1)
                    throw new Exception("Stack should only have one remaining entry");

                double r = stack.Pop();
                return r;
            }

            if (expression.ReturnType == GossipType.String)
            {
                if (stringStack.Count > 1)
                    throw new Exception("Stack should only have one remaining entry");

                string r = stringStack.Pop();
                return r;
            }

            if (expression.ReturnType == GossipType.ReferenceObject)
            {
                if (refStack.Count > 1)
                    throw new Exception("Stack should only have one remaining entry");

                object r = refStack.Pop();
                return r;
            }

            if (expression.ReturnType == GossipType.Action)
            {
                if (refStack.Count > 0 || stringStack.Count > 0 || stack.Count > 0)
                    throw new Exception(("Stack should be empty"));

                return null;
            }

            if (expression.ReturnType == GossipType.ReferenceModule)
            {
                if (stack.Count > 1)
                    throw new Exception("Stack should only have one remaining entry");

                double r = stack.Pop();
                return r;
            }

            throw new Exception("Unhandled return type when evaluating expression");
        }

        private static object[] GetActionParameters(ActionBinding binding, Stack<double> stack, Stack<object> refStack,
                                                    Stack<string> stringStack)
        {
            var parameters = new object[binding.NumParameters];
            for (int i = 0; i < binding.NumParameters; i++)
            {
                GossipType gossipType = binding.ParameterInfo[i].GossipType;
                Type hostType = binding.ParameterInfo[i].HostType;
                switch (gossipType)
                {
                    case GossipType.Number:
                        double item = stack.Pop();
                        parameters[i] = TypeConverter.ConvertNumber(item, hostType);
                        break;
                    case GossipType.ReferenceObject:
                        parameters[i] = refStack.Pop();
                        break;
                    case GossipType.String:
                        parameters[i] = stringStack.Pop();
                        break;
                    default:
                        throw new GossipScriptException(String.Format("Unhandled Gossipscript type:{0}", gossipType));
                }
            }
            return parameters;
        }

        private static object[] GetMethodParameters(MethodBinding binding, Stack<double> stack, Stack<object> refStack,
                                                    Stack<string> stringStack)
        {
            var parameters = new object[binding.NumParameters];
            for (int i = 0; i < binding.NumParameters; i++)
            {
                GossipType gossipType = binding.ParameterInfo[i].GossipType;
                Type hostType = binding.ParameterInfo[i].HostType;
                switch (gossipType)
                {
                    case GossipType.Number:
                        double item = stack.Pop();
                        parameters[i] = TypeConverter.ConvertNumber(item, hostType);
                        break;
                    case GossipType.ReferenceObject:
                        parameters[i] = refStack.Pop();
                        break;
                    case GossipType.String:
                        parameters[i] = stringStack.Pop();
                        break;
                    default:
                        throw new GossipScriptException(String.Format("Unhandled Gossipscript type:{0}", gossipType));
                }
            }
            return parameters;
        }

        private static void InvokeBinding(IBinding binding, object obj, object[] parameters, Expression expression)
        {
            switch (binding.GossipType)
            {
                case GossipType.Number:
                    {
                        object result = binding.Invoke(obj, parameters);
                        double data = Convert.ToDouble(result);
                        expression.EvaluationStack.Push(data);
                        break;
                    }
                case GossipType.ReferenceObject:
                    {
                        object data = binding.Invoke(obj, parameters);
                        expression.ReferenceObjectEvaluationStack.Push(data);
                        break;
                    }
                case GossipType.String:
                    {
                        var data = binding.Invoke<String>(obj, parameters);
                        expression.StringEvaluationStack.Push(data);
                        break;
                    }
                default:
                    throw new GossipScriptException("Unhandled Return type");
            }
        }
    }
}