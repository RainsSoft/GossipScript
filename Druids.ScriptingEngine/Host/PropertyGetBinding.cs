using System;
using System.Reflection;
using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Exceptions;
using Druids.ScriptingEngine.Exceptions;
using Druids.ScriptingEngine.Helper;
using Druids.ScriptingEngine.Interfaces;

namespace Druids.ScriptingEngine.Host
{
    public class PropertyGetBinding : IBinding
    {
        private readonly MethodInfo m_GetMethod;
        private readonly GossipType m_GossipType;
        private readonly int m_Id;
        private readonly string m_Name;
        private readonly Type m_Type;

        public PropertyGetBinding(Int32 id, String name, Type type, MethodInfo getMethod)
        {
            m_Id = id;
            m_Name = name;
            m_Type = type;
            m_GetMethod = getMethod;

            m_Type = getMethod.ReturnType;
            m_GossipType = TypeConverter.ConvertHostType(HostType);
        }

        public int Id
        {
            get { return m_Id; }
        }

        public string Name
        {
            get { return m_Name; }
        }

        public Type HostType
        {
            get { return m_Type; }
        }

        public GossipType GossipType
        {
            get { return m_GossipType; }
        }

        public Object Invoke(Object obj)
        {
            try
            {
                return m_GetMethod.Invoke(obj, null);
            }
            catch (Exception exception)
            {
                throw new GossipScriptException("Unable to invoke method on object", exception);
            }
        }

        public T Invoke<T>(Object obj)
        {
            try
            {
                return (T) m_GetMethod.Invoke(obj, null);
            }
            catch (Exception exception)
            {
                throw new GossipScriptException("Unable to invoke method on object");
            }
        }

        public object Invoke(object obj, object[] parameters)
        {
            return Invoke(obj);
        }

        public T Invoke<T>(object obj, object[] parameters)
        {
            return Invoke<T>(obj);
        }
    }
}