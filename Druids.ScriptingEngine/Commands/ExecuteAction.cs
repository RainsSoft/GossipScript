using System.Collections.Generic;
using System.Linq;
using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Interfaces;
using Druids.ScriptingEngine;
using Druids.ScriptingEngine.Host;

namespace Druids.ScriptingEngine.Commands
{
    internal class ExecuteAction : IActionNode
    {
        private readonly GossipActionParameter m_ActionParameter;
        private readonly ActionInfo m_FunctionInfo;
        private readonly List<Expression> m_Parameters;

        public ExecuteAction(ActionInfo functionInfo, List<Expression> parameters)
        {
            m_FunctionInfo = functionInfo;
            m_Parameters = parameters;

            CommandName = functionInfo.FunctionName;
            m_ActionParameter = new GossipActionParameter {Parameters = new object[parameters.Count]};
        }

        public GossipActionParameter ActionParameter
        {
            get { return m_ActionParameter; }
        }

        public ScriptResult Update(ScriptExecutionContext context)
        {
            ResolveParameters(context);
            ActionParameter.DeltaMs = context.DeltaMs;
            ActionParameter.Peek = Next.CommandName;
            ActionParameter.Script = context.Script;
            return m_FunctionInfo.OnUpdate(ActionParameter);
        }

        public void OnEnter(ScriptExecutionContext context)
        {
            ResolveParameters(context);
            ActionParameter.DeltaMs = context.DeltaMs;
            ActionParameter.Peek = Next.CommandName;
            ActionParameter.Script = context.Script;

            if (m_FunctionInfo.OnEnter != null)
                m_FunctionInfo.OnEnter.Invoke(ActionParameter);
        }

        public void OnExit(ScriptExecutionContext context)
        {
            ResolveParameters(context);
            ActionParameter.DeltaMs = context.DeltaMs;
            ActionParameter.Peek = Next.CommandName;
            ActionParameter.Script = context.Script;

            if (m_FunctionInfo.OnExit != null)
                m_FunctionInfo.OnExit.Invoke(ActionParameter);
        }

        public IActionNode Next { get; set; }

        public string CommandName { get; private set; }

        public bool Recall
        {
            get { return false; }
        }

        private void ResolveParameters(ScriptExecutionContext context)
        {
            for (int i = 0; i < m_ActionParameter.Parameters.Count(); i++)
            {
                m_ActionParameter.Parameters[i] =
                    context.Vm.HostBridge.ExpressionEvaluator.EvaluateExpression(m_Parameters[i], context);
            }
        }
    }
}