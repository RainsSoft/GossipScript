using System;
using System.Collections.Generic;
using Druids.ScriptingEngine.Commands;
using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Interfaces;

namespace Druids.ScriptingEngine
{
    public class ScriptEvent
    {
        private readonly int m_CustomEventId;
        private readonly Stack<IActionNode> m_JumpStack;
        private IActionNode m_CurrentExecutingActionNode;
        private Boolean m_HasRunCurrentNodeOnEnter;

        public ScriptEvent(ActionNodeRoot rootNode, Int32 customEventId)
        {
            m_CustomEventId = customEventId;
            RootNode = rootNode;
            m_JumpStack = new Stack<IActionNode>(8);
            Restart();
        }

        public ActionNodeRoot RootNode { get; set; }

        public Object Owner { get; set; }

        public Object EventParameter { get; set; }

        public IActionNode CurrentExecutingActionNode
        {
            get { return m_CurrentExecutingActionNode; }
            set
            {
                if (CurrentExecutingActionNode != value)
                    m_HasRunCurrentNodeOnEnter = false;

                m_CurrentExecutingActionNode = value;
            }
        }

        public int CustomEventId
        {
            get { return m_CustomEventId; }
        }

        public void Restart()
        {
            CurrentExecutingActionNode = RootNode;
        }

        public ScriptResult Run(ScriptExecutionContext context)
        {
            IActionNode currentNode = CurrentExecutingActionNode;
            while (currentNode != null)
            {
                // First time run
                if (m_HasRunCurrentNodeOnEnter == false)
                {
                    // Check if anything needs to be added to the jump stack before entering
                    if (currentNode.Recall)
                    {
                        if (currentNode.Next != null)
                            m_JumpStack.Push(currentNode.Next);
                    }

                    currentNode.OnEnter(context);
                    m_HasRunCurrentNodeOnEnter = true;
                }

                ScriptResult scriptResult = currentNode.Update(context);
                if (scriptResult == ScriptResult.Continue)
                {
                    IActionNode next = currentNode.Next; // Note: this save and load is important for conditionals
                    currentNode.OnExit(context); //       To obtain the correct branch and to then be correctly restored
                    currentNode = next; //       for any yeiding or continuation.

                    // If the current node is null check the jump stack
                    if (currentNode == null)
                    {
                        if (m_JumpStack.Count > 0)
                            currentNode = m_JumpStack.Pop();
                    }

                    CurrentExecutingActionNode = currentNode;
                }

                if (scriptResult == ScriptResult.ChangedNode)
                    return scriptResult;

                if (scriptResult == ScriptResult.Ok)
                    return scriptResult;

                if (scriptResult == ScriptResult.Yield)
                    return scriptResult;

                if (scriptResult == ScriptResult.EndProcess)
                    return scriptResult;

                if (scriptResult == ScriptResult.Return)
                {
                    CurrentExecutingActionNode = RootNode;
                    m_JumpStack.Clear();

                    return ScriptResult.Yield;
                }
            }
            return ScriptResult.Ok; // Script completed
        }
    }
}