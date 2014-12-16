using System;
using System.Collections.Generic;
using System.Reflection;
using Druids.ScriptingEngine.Exceptions;
using Druids.ScriptingEngine.Exceptions;

namespace Druids.ScriptingEngine.Host
{
    public class MethodBindingTable
    {
        private readonly Dictionary<Int32, MethodBinding> m_MethodBindingById = new Dictionary<int, MethodBinding>();

        private readonly Dictionary<String, MethodBinding> m_MethodBindingByKey =
            new Dictionary<String, MethodBinding>();

        private Int32 m_NextMethodId;

        public void RegisterMethod(String name, Type type, MethodInfo methodInfo)
        {
            lock (this)
            {
                // Only if not already registered

                // TODO Might need function overloading

                string key = type + "." + name;
                if (!m_MethodBindingByKey.ContainsKey(key))
                {
                    m_NextMethodId++;
                    var binding = new MethodBinding(m_NextMethodId, name, type, methodInfo);
                    m_MethodBindingById.Add(m_NextMethodId, binding);
                    m_MethodBindingByKey.Add(key, binding);
                }
            }
        }


        public MethodBinding GetMethodByName(string name, Type type)
        {
            try
            {
                string key = type + name;
                return m_MethodBindingByKey[key];
            }
            catch (KeyNotFoundException exception)
            {
                throw new GossipScriptMethodNotFoundException(
                    String.Format("Method name:{0} not found on type:{1}", name, type), exception);
            }
        }

        public MethodBinding GetMethodById(int methodId)
        {
            return m_MethodBindingById[methodId];
        }
    }
}