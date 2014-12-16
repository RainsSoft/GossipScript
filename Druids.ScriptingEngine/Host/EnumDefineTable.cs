using System;
using System.Collections.Generic;

namespace Druids.ScriptingEngine.Host
{
    public class EnumDefineTable
    {
        private readonly Dictionary<String, Int32> m_DefinesByString = new Dictionary<string, int>();

        public Dictionary<string, int> DefinesByString
        {
            get { return m_DefinesByString; }
        }

        public void Add(String name, Int32 value)
        {
            DefinesByString.Add(name, value);
        }

        public bool HasEnum(String name)
        {
            return DefinesByString.ContainsKey(name);
        }
    }
}