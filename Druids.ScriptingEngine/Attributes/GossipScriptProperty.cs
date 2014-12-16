using System;

namespace Druids.ScriptingEngine.Attributes
{
    public enum PropertyUsage
    {
        ReadWrite = 0,
        ReadOnly = 1
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class GossipScriptProperty : Attribute
    {
        private readonly string m_Propertyname;

        public GossipScriptProperty(String propertyname)
        {
            m_Propertyname = propertyname;
        }

        public string Propertyname
        {
            get { return m_Propertyname; }
        }

        public PropertyUsage Usage { get; set; }
    }
}