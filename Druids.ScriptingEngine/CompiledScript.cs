using System;
using System.Collections.Generic;
using Druids.ScriptingEngine;
using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Host;

namespace Druids.ScriptingEngine
{
    public class CompiledScript
    {
        private readonly ScriptExecutionContext m_Context;

        public CompiledScript(List<ScriptNode> nodes, Object scriptOwner)
        {
            if (nodes == null)
                throw new ArgumentNullException("nodes");

            if (nodes.Count == 0)
                throw new ArgumentException("Process list must contain at least one ScriptNode");

            LocalState = new LocalState();
            this.Nodes = nodes;
            ActiveNodes = new Stack<ScriptNode>();
            ScriptOwner = scriptOwner;
            m_Context = new ScriptExecutionContext(this, LocalState);
            QueuedEvents = new List<QueuedScriptletEvent>(4);

            Restart();
        }

        // If the script is running with an owner context
        public Object ScriptOwner { get; set; }

        public LocalState LocalState { get; set; }

        private List<ScriptNode> Nodes { get; set; } // Inert Nodes

        private Stack<ScriptNode> ActiveNodes { get; set; } // Active Nodes

        private List<QueuedScriptletEvent> QueuedEvents { get; set; }

        public void Restart()
        {
            LocalState.Reset();

            while (ActiveNodes.Count > 0)
            {
                ActiveNodes.Pop();
            }

            ActiveNodes.Push(Nodes[0]);

            foreach (ScriptNode scriptlet in Nodes)
            {
                scriptlet.Restart();
            }

            if (Nodes[0].OnStartEvent != null)
                QueuedEvents.Add(new QueuedScriptletEvent(Nodes[0].OnStart, Nodes[0].OnStartEvent));
        }

        public ScriptResult Update(GossipVM engine)
        {
            return Update(engine, 0);
        }

        public ScriptResult Update(GossipVM engine, Int32 deltaMs)
        {
            m_Context.Vm = engine;
            m_Context.DeltaMs = deltaMs;

            // Always run OnInterrupt
            if (ActiveNodes.Count > 0)
            {
                ScriptNode top = ActiveNodes.Peek();

                if (top.OnInterruptEvent == null)
                {
                    // Go down the stack
                    foreach (ScriptNode node in ActiveNodes)
                    {
                        if (node.OnInterruptEvent != null)
                        {
                            m_Context.CurrentScriptEvent = node.OnInterruptEvent;
                            ScriptResult interruptResult = node.OnInterrupt(m_Context);
                            if (interruptResult != ScriptResult.Ok)
                                return interruptResult;

                            break;
                        }
                    }
                }
                else
                {
                    m_Context.CurrentScriptEvent = top.OnInterruptEvent;
                    ScriptResult interruptResult = top.OnInterrupt(m_Context);
                    if (interruptResult != ScriptResult.Ok)
                        return interruptResult;
                }
            }

            // Run queued Events
            for (int i = 0; i < QueuedEvents.Count; i++)
            {
                QueuedScriptletEvent queuedEvent = QueuedEvents[i];
                m_Context.CurrentScriptEvent = queuedEvent.ScriptEvent;
                m_Context.CurrentScriptEvent.Owner = ScriptOwner;

                ScriptResult queuedEventResult = queuedEvent.Func(m_Context);
                if (queuedEventResult != ScriptResult.Ok)
                    return queuedEventResult;

                // Remove from queue
                QueuedEvents.RemoveAt(i);
                i--;
            }

            // On Update
            {
                if (ActiveNodes.Count == 0)
                    return ScriptResult.Ok;

                ScriptNode top = ActiveNodes.Peek();

                var result = ScriptResult.Ok;
                if (top.OnUpdateEvent == null)
                {
                    // Go down the stack
                    foreach (ScriptNode scriptlet in ActiveNodes)
                    {
                        if (scriptlet.OnUpdateEvent != null)
                        {
                            m_Context.CurrentScriptEvent = scriptlet.OnUpdateEvent;
                            result = scriptlet.OnRun(m_Context);
                            break;
                        }
                    }
                }
                else
                {
                    m_Context.CurrentScriptEvent = top.OnUpdateEvent;
                    m_Context.CurrentScriptEvent.Owner = ScriptOwner;
                    result = top.OnUpdateEvent.Run(m_Context);
                }

                if (result == ScriptResult.EndProcess)
                    PopActiveNode();
                
                if (result == ScriptResult.Ok)
                {
                    if (top.OnEndEvent != null)
                        QueuedEvents.Add(new QueuedScriptletEvent(top.OnEnd, top.OnEndEvent));
                }

                // Run any queued events
                for (int i = 0; i < QueuedEvents.Count; i++)
                {
                    QueuedScriptletEvent queuedEvent = QueuedEvents[i];
                    m_Context.CurrentScriptEvent = queuedEvent.ScriptEvent;
                    m_Context.CurrentScriptEvent.Owner = ScriptOwner;

                    ScriptResult queuedEventResult = queuedEvent.Func(m_Context);
                    if (queuedEventResult != ScriptResult.Ok)
                        return queuedEventResult;

                    // Remove from queue
                    QueuedEvents.RemoveAt(i);
                    i--;
                }

                return result;
            }
        }

        /// <summary>
        ///     Pops the top ScriptNode off the stack. If there is only one active ScriptNode it will not be popped.
        /// </summary>
        public void PopActiveNode()
        {
            ScriptNode scriptNode = ActiveNodes.Pop();
            if (scriptNode.OnEndEvent != null)
                QueuedEvents.Add(new QueuedScriptletEvent(scriptNode.OnEnd, scriptNode.OnEndEvent));
        }

        public void PushActiveNode(ScriptNode scriptNode)
        {
            ActiveNodes.Push(scriptNode);

            if (scriptNode.OnStartEvent != null)
                QueuedEvents.Add(new QueuedScriptletEvent(scriptNode.OnStart, scriptNode.OnStartEvent));
        }

        public void SetActiveNode(String name)
        {
            var scriptNode = GetNodeByName(name);
            if (scriptNode != null)
            {
                // Pop all nodes
                while (ActiveNodes.Count > 0)
                {
                    ScriptNode oldNode = ActiveNodes.Pop();
                    if (oldNode.OnEndEvent != null)
                        QueuedEvents.Add(new QueuedScriptletEvent(oldNode.OnEnd, oldNode.OnEndEvent));
                }

                // Reset the execution point
                scriptNode.OnUpdateEvent.CurrentExecutingActionNode = scriptNode.OnUpdateEvent.RootNode;
                PushActiveNode(scriptNode);
            }
        }

        public void SetActiveNode(ScriptNode scriptNode)
        {
            // Pop all nodes
            while (ActiveNodes.Count > 0)
            {
                ScriptNode oldNode = ActiveNodes.Pop();
                if (oldNode.OnEndEvent != null)
                    QueuedEvents.Add(new QueuedScriptletEvent(oldNode.OnEnd, oldNode.OnEndEvent));
            }

            PushActiveNode(scriptNode);
        }

        public void RaiseEvent(EventBinding binding, Object eventParameter = null)
        {
            if (ActiveNodes.Count == 0)
                return;

            foreach (ScriptNode scriptlet in ActiveNodes)
            {
                if (scriptlet.HasEvent(binding))
                {
                    m_Context.CurrentScriptEvent.EventParameter = eventParameter;
                    scriptlet.RaiseEvent(binding, m_Context);
                    break;
                }
            }
        }

        public IEnumerable<ScriptNode> GetNodes()
        {
            return Nodes;
        }

        public ScriptNode GetNodeByName(String name)
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].Name == name)
                    return Nodes[i];
            }
            return null;
        }
    }
}