using System;
using System.Collections.Generic;
using System.Threading;

namespace Druids.ScriptingEngine.Host
{
    public class StringTable
    {
        private readonly Dictionary<String, Int32> m_IdTableByString = new Dictionary<string, Int32>();
        private readonly Dictionary<Int32, String> m_StringTableById = new Dictionary<int, String>();

        private Int32 m_NextId;

        public StringTable()
        {
            
        }

        public void RegisterString(String value)
        {
            lock (this)
            {
                if (m_IdTableByString.ContainsKey(value))
                    return; // Already registered

                Interlocked.Increment(ref m_NextId);
                int id = m_NextId;
                m_StringTableById.Add(id, value);
                m_IdTableByString.Add(value, id);
            }
        }

        public String GetStringById(Int32 id)
        {
            return m_StringTableById[id];
        }

        public Int32 GetIdByString(String name)
        {
            return m_IdTableByString[name];
        }
    }
}