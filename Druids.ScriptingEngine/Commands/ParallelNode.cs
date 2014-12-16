using System.Collections.Generic;
using System.Diagnostics;
using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Interfaces;
using Druids.ScriptingEngine;

namespace Druids.ScriptingEngine.Commands
{
    public class ParallelNode : IActionNode
    {
        private readonly IActionNode m_BlockRoot;
        private readonly List<IActionNode> m_ExecutionList;

        public ParallelNode(IActionNode blockRoot)
        {
            m_BlockRoot = blockRoot;
            int i = 0;
            IActionNode currentBlock = m_BlockRoot;
            while (currentBlock != null)
            {
                i++;
                currentBlock = currentBlock.Next;
            }
            m_ExecutionList = new List<IActionNode>(i);
        }

        public ScriptResult Update(ScriptExecutionContext context)
        {
            for (int i = 0; i < m_ExecutionList.Count; i++)
            {
                IActionNode currentCommand = m_ExecutionList[i];
                if (currentCommand.Update(context) == ScriptResult.Continue)
                {
                    m_ExecutionList.Remove(currentCommand);
                    i--;
                }
            }

            if (m_ExecutionList.Count > 0)
                return ScriptResult.Yield;

            return ScriptResult.Continue;
        }

        public void OnEnter(ScriptExecutionContext context)
        {
            IActionNode currentBlock = m_BlockRoot;
            while (currentBlock != null)
            {
                m_ExecutionList.Add(currentBlock);
                currentBlock.OnEnter(context);
                currentBlock = currentBlock.Next;
            }
        }

        public void OnExit(ScriptExecutionContext context)
        {
            IActionNode currentBlock = m_BlockRoot;
            while (currentBlock != null)
            {
                currentBlock.OnExit(context);
                currentBlock = currentBlock.Next;
            }

            Debug.Assert(m_ExecutionList.Count == 0);
        }

        public IActionNode Next { get; set; }

        public string CommandName { get; private set; }

        public bool Recall
        {
            get { return false; }
        }
    }
}