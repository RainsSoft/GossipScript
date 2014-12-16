using System;
using System.Reflection;
using Druids.ScriptingEngine.Compiler;
using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Exceptions;
using Druids.ScriptingEngine.Exceptions;
using Druids.ScriptingEngine.Helper;
using Druids.ScriptingEngine.Interfaces;

namespace Druids.ScriptingEngine.Host
{
    public class MethodBinding : IBinding
    {
        private readonly Type m_HostType;
        private readonly int m_Id;
        private readonly MethodInfo m_MethodInfo;
        private readonly string m_Name;
        private readonly Int32 m_NumParameters;
        private readonly TypeAdapter[] m_ParameterInfo;
        private readonly GossipType m_ParameterType;

        public MethodBinding(Int32 id, String name, Type type, MethodInfo methodInfo)
        {
            m_Id = id;
            m_Name = name;
            m_MethodInfo = methodInfo;
            m_HostType = methodInfo.ReturnType;
            m_ParameterType = TypeConverter.ConvertHostType(HostReturnType);

            ParameterInfo[] parameters = methodInfo.GetParameters();
            m_NumParameters = parameters.Length;
            m_ParameterInfo = new TypeAdapter[m_NumParameters];
            for (int i = 0; i < ParameterInfo.Length; i++)
            {
                GossipType convertedType = TypeConverter.ConvertHostType(parameters[i].ParameterType);
                ParameterInfo[i] = new TypeAdapter(convertedType, parameters[i].ParameterType);
            }
        }

        public int Id
        {
            get { return m_Id; }
        }

        public string Name
        {
            get { return m_Name; }
        }

        public int NumParameters
        {
            get { return m_NumParameters; }
        }

        public Type HostReturnType
        {
            get { return m_HostType; }
        }

        public TypeAdapter[] ParameterInfo
        {
            get { return m_ParameterInfo; }
        }

        public GossipType GossipType
        {
            get { return m_ParameterType; }
        }

        public Object Invoke(Object obj)
        {
            try
            {
                return m_MethodInfo.Invoke(obj, null);
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
                return (T) m_MethodInfo.Invoke(obj, null);
            }
            catch (Exception exception)
            {
                throw new GossipScriptException("Unable to invoke method on object", exception);
            }
        }

        public Object Invoke(Object obj, Object[] parameters)
        {
            try
            {
                return m_MethodInfo.Invoke(obj, parameters);
            }
            catch (Exception exception)
            {
                throw new GossipScriptException("Unable to invoke method on object", exception);
            }
        }

        public T Invoke<T>(Object obj, Object[] parameters)
        {
            try
            {
                return (T) m_MethodInfo.Invoke(obj, parameters);
            }
            catch (Exception exception)
            {
                throw new GossipScriptException("Unable to invoke method on object", exception);
            }
        }
    }
}