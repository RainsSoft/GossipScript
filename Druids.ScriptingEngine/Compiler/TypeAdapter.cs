using System;
using Druids.ScriptingEngine.Enums;

namespace Druids.ScriptingEngine.Compiler
{
    /// <summary>
    ///     Class to hold conversion information between GossipScript types and .Net types
    /// </summary>
    public class TypeAdapter
    {
        private readonly GossipType m_GossipType;
        private readonly Type m_HostType;

        public TypeAdapter(GossipType gossipType)
        {
            m_GossipType = gossipType;
        }

        public TypeAdapter(GossipType gossipType, Type hostType)
        {
            m_HostType = hostType;
            m_GossipType = gossipType;
        }

        public GossipType GossipType
        {
            get { return m_GossipType; }
        }

        public Type HostType
        {
            get { return m_HostType; }
        }
    }
}