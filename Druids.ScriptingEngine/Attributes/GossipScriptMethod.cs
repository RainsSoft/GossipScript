using System;

namespace Druids.ScriptingEngine.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class GossipScriptMethod : Attribute
    {
        private readonly string m_MethodName;

        public GossipScriptMethod(String methodName)
        {
            m_MethodName = methodName;
        }

        public string MethodName
        {
            get { return m_MethodName; }
        }
    }
}