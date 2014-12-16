using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Interfaces;
using Druids.ScriptingEngine;
using Druids.ScriptingEngine.Interfaces;

namespace Druids.ScriptingEngine.Commands
{
    public class ExecuteCustomActionNode : IActionNode
    {
        private readonly CustomActionContext m_CustomActionContext;
        private readonly ICustomActionNode m_CustomActionNode;

        public ExecuteCustomActionNode(ICustomActionNode customActionNode, CustomActionContext customActionContext)
        {
            m_CustomActionNode = customActionNode;
            m_CustomActionContext = customActionContext;
        }

        public ScriptResult Update(ScriptExecutionContext context)
        {
            m_CustomActionContext.EvaluateContext(context);
            m_CustomActionNode.CustomActionContext = m_CustomActionContext;
            return m_CustomActionNode.Update(context);
        }

        public void OnEnter(ScriptExecutionContext context)
        {
            m_CustomActionContext.EvaluateContext(context);
            m_CustomActionNode.CustomActionContext = m_CustomActionContext;
            m_CustomActionNode.OnEnter(context);
        }

        public void OnExit(ScriptExecutionContext context)
        {
            m_CustomActionContext.EvaluateContext(context);
            m_CustomActionNode.CustomActionContext = m_CustomActionContext;
            m_CustomActionNode.OnExit(context);
        }

        public IActionNode Next { get; set; }

        public string CommandName
        {
            get { return "Execute Custom Action Node"; }
        }

        public bool Recall
        {
            get { return false; }
        }
    }
}