using System;
using Druids.ScriptingEngine.Enums;

namespace Druids.ScriptingEngine.Compiler
{
    public class ExtendedTypeAdapter
    {
        public ExtendedTypeAdapter(GossipType gossipType)
        {
            TypeAdapter = new TypeAdapter(gossipType);
        }

        public ExtendedTypeAdapter(GossipType gossipType, Type hostType)
        {
            TypeAdapter = new TypeAdapter(gossipType, hostType);
        }

        public ExtendedTypeAdapter(GossipType gossipType, Type hostType, Int32 data)
        {
            TypeAdapter = new TypeAdapter(gossipType, hostType);
            Data = data;
        }

        public TypeAdapter TypeAdapter { get; set; }

        public Double Data { get; set; }

        public double Target { get; set; }
    }
}