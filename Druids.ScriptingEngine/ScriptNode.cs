using System;
using System.Collections.Generic;
using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Host;

namespace Druids.ScriptingEngine
{
    public class ScriptNode
    {
        private object m_Owner;

        public ScriptNode(String name)
        {
            OnCustomEvents = new List<ScriptEvent>();
            Name = name;
        }

        public String Name { get; set; }

        public ScriptEvent OnUpdateEvent { get; set; } // Main event

        public ScriptEvent OnStartEvent { get; set; }

        public ScriptEvent OnEndEvent { get; set; }

        public ScriptEvent OnInterruptEvent { get; set; }

        public List<ScriptEvent> OnCustomEvents { get; set; }

        // Refered to as 'self' in scripts
        public Object Owner
        {
            get { return m_Owner; }
            set
            {
                m_Owner = value;
                if (OnUpdateEvent != null)
                    OnUpdateEvent.Owner = m_Owner;

                if (OnStartEvent != null)
                    OnStartEvent.Owner = m_Owner;

                if (OnEndEvent != null)
                    OnEndEvent.Owner = m_Owner;

                if (OnInterruptEvent != null)
                    OnInterruptEvent.Owner = m_Owner;
            }
        }

        /// <summary>
        ///     Creates a new ScriptNode based on this ScriptNode
        /// </summary>
        public ScriptNode Create()
        {
            var rv = new ScriptNode(this.Name)
                {
                    Owner = Owner,
                    OnUpdateEvent = OnUpdateEvent,
                    OnStartEvent = OnStartEvent,
                    OnEndEvent = OnEndEvent,
                    OnInterruptEvent = OnInterruptEvent,
                };
            rv.Restart();
            return rv;
        }

        public void Restart()
        {
            if (OnUpdateEvent != null)
                OnUpdateEvent.Restart();

            if (OnStartEvent != null)
                OnStartEvent.Restart();

            if (OnEndEvent != null)
                OnEndEvent.Restart();

            if (OnInterruptEvent != null)
                OnInterruptEvent.Restart();

            foreach (ScriptEvent customEvent in OnCustomEvents)
            {
                customEvent.Restart();
            }
        }

        public ScriptResult OnRun(ScriptExecutionContext context)
        {
            if (OnUpdateEvent == null)
                return ScriptResult.Ok;

            return OnUpdateEvent.Run(context);
        }

        public ScriptResult OnStart(ScriptExecutionContext context)
        {
            if (OnStartEvent == null)
                return ScriptResult.Ok;

            return OnStartEvent.Run(context);
        }

        public ScriptResult OnEnd(ScriptExecutionContext context)
        {
            if (OnEndEvent == null)
                return ScriptResult.Ok;

            return OnEndEvent.Run(context);
        }

        public ScriptResult OnInterrupt(ScriptExecutionContext context)
        {
            if (OnInterruptEvent == null)
                return ScriptResult.Ok;

            ScriptResult result = OnInterruptEvent.Run(context);

            if (result == ScriptResult.Ok)
                OnInterruptEvent.Restart();

            return result;
        }

        public bool HasEvent(EventBinding binding)
        {
            foreach (ScriptEvent scriptletEvent in OnCustomEvents)
            {
                if (scriptletEvent.CustomEventId == binding.Id)
                    return true;
            }

            return false;
        }

        public void RaiseEvent(EventBinding binding, ScriptExecutionContext context)
        {
            foreach (ScriptEvent scriptEvent in OnCustomEvents)
            {
                if (scriptEvent.CustomEventId == binding.Id)
                {
                    scriptEvent.Run(context);
                    return;
                }
            }
        }
    }
}