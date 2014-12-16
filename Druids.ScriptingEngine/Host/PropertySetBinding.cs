using System;
using System.Reflection;
using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Helper;

namespace Druids.ScriptingEngine.Host
{
    public class PropertySetBinding
    {
        private readonly Type m_HostType;
        private readonly int m_Id;
        private readonly MethodInfo m_MethodInfo;
        private readonly string m_Name;
        private readonly GossipType m_ParameterType;
        private readonly Object[] m_Parameters = new object[1];
        private readonly Type m_Type;

        public PropertySetBinding(Int32 id, String name, Type type, MethodInfo methodInfo)
        {
            m_Id = id;
            m_Name = name;
            m_Type = type;
            m_MethodInfo = methodInfo;
            m_HostType = m_MethodInfo.GetParameters()[0].ParameterType;
            m_ParameterType = TypeConverter.ConvertHostType(m_HostType);
        }

        public int Id
        {
            get { return m_Id; }
        }

        public string Name
        {
            get { return m_Name; }
        }

        public GossipType ParameterType
        {
            get { return m_ParameterType; }
        }

        public void InvokeSet(Object obj, Object value)
        {
            m_Parameters[0] = value;

            // TODO Other type conversions
            if (m_HostType == typeof (Int32))
            {
                m_Parameters[0] = Convert.ToInt32(value);
            }

            m_MethodInfo.Invoke(obj, m_Parameters);
        }
    }
}