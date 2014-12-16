using System;
using System.Collections.Generic;
using Druids.ScriptingEngine;

namespace Druids.ScriptingEngine
{
    public class CustomActionContext
    {
        private readonly List<Expression> m_Expressions;
                                          // These expressions need to be evaluated before being consumed by the custom action

        private readonly Object[] m_Parameters;
        private readonly int m_ReferenceTargetId;

        private readonly Expression m_TargetExpression;

        private Object m_ActionTarget;

        /// <summary>
        /// </summary>
        /// <param name="parameters">The parameters to be consumed by the Custom Action presented as a list of expressions to be evaluated at runtime.</param>
        /// <param name="referenceTargetId">The id of the reference to use as the target for the custom action context. To target the ScriptProcess's owner object use the value -1.</param>
        public CustomActionContext(List<Expression> parameters, Int32 referenceTargetId = -1)
        {
            m_Expressions = parameters;
            m_ReferenceTargetId = referenceTargetId;
            m_Parameters = new object[parameters.Count];
        }

        public CustomActionContext(List<Expression> parameters, Expression targetExpression)
        {
            m_Expressions = parameters;
            m_TargetExpression = targetExpression;
            m_Parameters = new object[parameters.Count];
        }

        public object[] Parameters
        {
            get { return m_Parameters; }
        }

        public object ActionTarget
        {
            get { return m_ActionTarget; }
        }

        public void EvaluateContext(ScriptExecutionContext scriptExecutionContext)
        {
            // Set the target object
            // Or the temp object
            if (m_TargetExpression != null)
            {
                m_ActionTarget =
                    scriptExecutionContext.Vm.HostBridge.ExpressionEvaluator.EvaluateExpression(m_TargetExpression,
                                                                                                scriptExecutionContext);
            }
            else
            {
                if (m_ReferenceTargetId == -1)
                {
                    m_ActionTarget = scriptExecutionContext.CurrentScriptEvent.Owner;
                }
                else
                {
                    m_ActionTarget = scriptExecutionContext.LocalState.Ref[m_ReferenceTargetId];
                }
            }

            // Evaluate the expressions
            for (int i = 0; i < m_Parameters.Length; i++)
            {
                m_Parameters[i] =
                    scriptExecutionContext.Vm.HostBridge.ExpressionEvaluator.EvaluateExpression(m_Expressions[i],
                                                                                                scriptExecutionContext);
            }
        }
    }
}