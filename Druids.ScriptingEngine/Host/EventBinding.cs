using System;

namespace Druids.ScriptingEngine.Host
{
    public class EventBinding
    {
        private readonly string m_EventName;
        private readonly Type m_EventParameterType;
        private readonly Int32 m_Id;

        public EventBinding(Int32 id, String eventName, Type eventParameterType)
        {
            m_Id = id;
            m_EventName = eventName;
            m_EventParameterType = eventParameterType;
        }

        public int Id
        {
            get { return m_Id; }
        }

        public string EventName
        {
            get { return m_EventName; }
        }

        public Type EventParameterType
        {
            get { return m_EventParameterType; }
        }
    }
}