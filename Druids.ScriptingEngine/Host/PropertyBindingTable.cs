using System;
using System.Collections.Generic;
using System.Reflection;
using Druids.ScriptingEngine.Exceptions;
using Druids.ScriptingEngine.Exceptions;

namespace Druids.ScriptingEngine.Host
{
    public class PropertyBindingTable
    {
        private readonly Dictionary<Int32, PropertyGetBinding> m_PropertyBindingGetById =
            new Dictionary<int, PropertyGetBinding>();

        private readonly Dictionary<String, Dictionary<Type, PropertyGetBinding>> m_PropertyBindingGetByName =
            new Dictionary<string, Dictionary<Type, PropertyGetBinding>>();

        private readonly Dictionary<Int32, PropertySetBinding> m_PropertyBindingSetById =
            new Dictionary<int, PropertySetBinding>();

        private readonly Dictionary<String, Dictionary<Type, PropertySetBinding>> m_PropertyBindingSetByName =
            new Dictionary<string, Dictionary<Type, PropertySetBinding>>();

        private Int32 m_NextPropertyGetId;
        private Int32 m_NextPropertySetId;

        public void RegisterPropertyGet(String name, Type type, MethodInfo methodInfo)
        {
            lock (this)
            {
                // Type needs to be part of the key
                string key = type + "." + name;
                if (!m_PropertyBindingGetByName.ContainsKey(key))
                {
                    m_NextPropertyGetId++;
                    var binding = new PropertyGetBinding(m_NextPropertyGetId, name, type, methodInfo);
                    m_PropertyBindingGetById.Add(m_NextPropertyGetId, binding);

                    var dict = new Dictionary<Type, PropertyGetBinding> {{type, binding}};
                    m_PropertyBindingGetByName.Add(key, dict);
                }
            }
        }

        public void RegisterPropertySet(String name, Type type, MethodInfo methodInfo)
        {
            lock (this)
            {
                string key = type + "." + name;
                if (!m_PropertyBindingSetByName.ContainsKey(key))
                {
                    m_NextPropertySetId++;
                    var binding = new PropertySetBinding(m_NextPropertySetId, name, type, methodInfo);
                    m_PropertyBindingSetById.Add(m_NextPropertySetId, binding);

                    var dict = new Dictionary<Type, PropertySetBinding> {{type, binding}};
                    m_PropertyBindingSetByName.Add(key, dict);
                }
            }
        }

        public PropertyGetBinding GetPropertyGetById(int actionId, Type type)
        {
            return m_PropertyBindingGetById[actionId];
        }

        public PropertyGetBinding GetPropertyGetByName(string name, Type type)
        {
            string key = type + "." + name;
            if (m_PropertyBindingGetByName.ContainsKey(key))
            {
                Dictionary<Type, PropertyGetBinding> dict = m_PropertyBindingGetByName[key];
                if (dict.ContainsKey(type))
                    return dict[type];
            }
            throw new GossipScriptException(String.Format("Unknown Get property:{0} on type{1}", name, type));
        }

        public PropertySetBinding GetPropertySetByName(string name, Type type)
        {
            string key = type + "." + name;
            if (m_PropertyBindingSetByName.ContainsKey(key))
            {
                Dictionary<Type, PropertySetBinding> dict = m_PropertyBindingSetByName[key];
                if (dict.ContainsKey(type))
                    return dict[type];
            }
            throw new GossipScriptException(String.Format("Unknown Set property:{0} on type{1}", name, type));
        }

        public PropertySetBinding GetPropertySetById(int id, Type type)
        {
            return m_PropertyBindingSetById[id];
        }

        public bool GetPropertyExists(string name, Type type)
        {
            string key = type + "." + name;
            return m_PropertyBindingGetByName.ContainsKey(key);
        }

        public bool SetPropertyExists(string name, Type type)
        {
            string key = type + "." + name;
            return m_PropertyBindingSetByName.ContainsKey(key);
        }
    }
}