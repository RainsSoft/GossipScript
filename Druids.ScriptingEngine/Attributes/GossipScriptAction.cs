using System;

namespace Druids.ScriptingEngine.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class GossipScriptAction : Attribute
    {
        private readonly Type m_ActionType;
        private readonly string m_Name;

        public GossipScriptAction(String name, Type actionType)
        {
            m_Name = name;
            m_ActionType = actionType;
        }

        public string Name
        {
            get { return m_Name; }
        }

        public Type ActionType
        {
            get { return m_ActionType; }
        }
    }
}