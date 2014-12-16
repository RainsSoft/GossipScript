using System;
using System.Reflection;
using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Exceptions;
using Druids.ScriptingEngine.Exceptions;

namespace Druids.ScriptingEngine.Host
{
    public class HostCall
    {
        private readonly Delegate m_Function;
        private readonly Type m_HostType;
        private readonly MethodInfo m_Method;
        private readonly string m_Name;
        private readonly object[] m_ParameterList;
        private readonly Type[] m_ParameterTypeList;
        private readonly GossipType m_ReturnType;

        public HostCall(string name, Delegate function)
        {
            m_Method = function.GetType().GetMethod("Invoke");

            m_HostType = m_Method.ReturnType;
            m_ReturnType = GossipType.None;

            if (m_Method.ReturnType == null)
                throw new GossipScriptException("Void is not a supported return type");

            if (m_Method.ReturnType == typeof (double)) // TODO Any number
                m_ReturnType = GossipType.Number;

            if (m_Method.ReturnType == typeof (String))
                m_ReturnType = GossipType.String;

            if (m_Method.ReturnType.BaseType == typeof (Object))
                m_ReturnType = GossipType.ReferenceObject;

            if (m_ReturnType == GossipType.None)
                throw new GossipScriptException("Unsupported return type: {0}");

            m_Name = name;
            m_Function = function;
            ParameterInfo[] parameters = Method.GetParameters();
            m_ParameterTypeList = new Type[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterTypeList[i] = parameters[i].ParameterType;
            }

            m_ParameterList = new object[parameters.Length];
            NumParameters = m_ParameterList.Length;
        }

        public Int32 Id { get; set; }

        public Int32 NumParameters { get; set; }

        public MethodInfo Method
        {
            get { return m_Method; }
        }

        public object[] ParameterList
        {
            get { return m_ParameterList; }
        }

        public string Name
        {
            get { return m_Name; }
        }

        public GossipType ReturnType
        {
            get { return m_ReturnType; }
        }

        public Type[] ParameterTypeList
        {
            get { return m_ParameterTypeList; }
        }

        public Type HostType
        {
            get { return m_HostType; }
        }

        public Double Invoke()
        {
            return (Double) Method.Invoke(m_Function, ParameterList);
        }

        public T Invoke<T>()
        {
            return (T) Method.Invoke(m_Function, ParameterList);
        }

        public Object InvokeObjectReturnType()
        {
            return Method.Invoke(m_Function, ParameterList);
        }
    }
}