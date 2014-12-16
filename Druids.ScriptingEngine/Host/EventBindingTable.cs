using System;
using System.Collections.Generic;
using Druids.ScriptingEngine.Exceptions;
using Druids.ScriptingEngine.Exceptions;

namespace Druids.ScriptingEngine.Host
{
    public class EventBindingTable
    {
        private readonly Dictionary<Int32, EventBinding> m_BindingById = new Dictionary<int, EventBinding>();
        private readonly Dictionary<String, EventBinding> m_BindingByKey = new Dictionary<String, EventBinding>();
        private Int32 m_NextActionId;

        public void Register(String name, Type eventType)
        {
            lock (this)
            {
                if (!name.EndsWith("()"))
                    throw new GossipScriptException("Events must end with ()");

                string key = name;
                if (!m_BindingByKey.ContainsKey(key))
                {
                    m_NextActionId++;
                    var binding = new EventBinding(m_NextActionId, name, eventType);
                    m_BindingById.Add(m_NextActionId, binding);

                    m_BindingByKey.Add(key, binding);
                }
                m_NextActionId++;
            }
        }

        public EventBinding GetBindingById(Int32 id)
        {
            return m_BindingById[id];
        }

        public EventBinding GetBindingByName(String name)
        {
            return m_BindingByKey[name];
        }
    }
}