using System;
using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Interfaces;
using Druids.ScriptingEngine;
using Druids.ScriptingEngine.Host;

namespace Druids.ScriptingEngine.Commands
{
    public class AssignReferenceProperty : IActionNode
    {
        private readonly Expression m_AssignmentExpression;
        private readonly Expression m_ExpressionTarget;
        private readonly int m_PropertySetMethodId;
        private readonly int m_RefIndex;

        /// <summary>
        ///     Assign to the result from expressionTarget via the proprertySetMethodId the result from assignmentExpression
        /// </summary>
        public AssignReferenceProperty(Expression expressionTarget, Int32 propertySetMethodId,
                                       Expression assignmentExpression)
        {
            m_ExpressionTarget = expressionTarget;
            m_PropertySetMethodId = propertySetMethodId;
            m_AssignmentExpression = assignmentExpression;
        }

        /// <summary>
        ///     Assign to the object reference in index 'refIndex' using the result from assignmentExpression via propertySetMethodId
        ///     Note: If refIndex is -1 then the result is applied to the self object.
        /// </summary>
        public AssignReferenceProperty(Int32 refIndex, Int32 propertySetMethodId, Expression assignmentExpression)
        {
            m_RefIndex = refIndex;
            m_PropertySetMethodId = propertySetMethodId;
            m_AssignmentExpression = assignmentExpression;
        }

        public ScriptResult Update(ScriptExecutionContext context)
        {
            return ScriptResult.Continue;
        }

        public void OnEnter(ScriptExecutionContext context)
        {
            if (m_ExpressionTarget != null)
            {
                object obj = context.Vm.HostBridge.ExpressionEvaluator.EvaluateExpression(m_ExpressionTarget, context);

                object value = context.Vm.HostBridge.ExpressionEvaluator.EvaluateExpression(m_AssignmentExpression,
                                                                                            context);
                PropertySetBinding setter =
                    context.Vm.HostBridge.TypeBindingTable.GetPropertySetById(m_PropertySetMethodId, obj.GetType());

                setter.InvokeSet(obj, value);
            }
            else
            {
                object obj = context.CurrentScriptEvent.Owner;
                if (m_RefIndex >= 0)
                    obj = context.LocalState.Ref[m_RefIndex];

                if (m_RefIndex == -2)
                    obj = context.CurrentScriptEvent.EventParameter;

                object value = context.Vm.HostBridge.ExpressionEvaluator.EvaluateExpression(m_AssignmentExpression,
                                                                                            context);
                PropertySetBinding setter =
                    context.Vm.HostBridge.TypeBindingTable.GetPropertySetById(m_PropertySetMethodId, obj.GetType());

                setter.InvokeSet(obj, value);
            }
        }

        public void OnExit(ScriptExecutionContext context)
        {
            // throw new NotImplementedException();
        }

        public IActionNode Next { get; set; }
        public string CommandName { get; private set; }
        public bool Recall { get; private set; }
    }
}