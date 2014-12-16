using System;
using System.Collections.Generic;
using System.Reflection;

namespace Druids.ScriptingEngine.Host
{
    public class ActionBindingTable
    {
        private readonly Dictionary<Int32, ActionBinding> m_ActionBindingById = new Dictionary<int, ActionBinding>();

        private readonly Dictionary<String, ActionBinding> m_ActionBindingByKey =
            new Dictionary<String, ActionBinding>();

        private Int32 m_NextActionId;

        public void Register(string name, Type targetType, Type actionType, MethodInfo methodInfo)
        {
            lock (this)
            {
                string key = targetType + "." + name;
                if (!m_ActionBindingByKey.ContainsKey(key))
                {
                    m_NextActionId++;
                    var binding = new ActionBinding(m_NextActionId, name, actionType, methodInfo);
                    m_ActionBindingById.Add(m_NextActionId, binding);

                    m_ActionBindingByKey.Add(key, binding);
                }
            }
        }

        public ActionBinding GetActionByName(string name, Type type)
        {
            string key = type + name;
            return m_ActionBindingByKey[key];
        }

        public ActionBinding GetActionById(int id)
        {
            return m_ActionBindingById[id];
        }
    }
}