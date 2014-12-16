using System;
using System.Collections.Generic;
using System.Threading;

namespace Druids.ScriptingEngine.Host
{
    public class HostCallTable
    {
        private readonly Dictionary<String, HostCall> m_FuctionTableByName = new Dictionary<string, HostCall>();
        private readonly Dictionary<Int32, HostCall> m_FunctionTableById = new Dictionary<int, HostCall>();

        private Int32 m_NextId;

        public IEnumerable<HostCall> Values
        {
            get { return m_FunctionTableById.Values; }
        }

        public void RegisterFunction(string name, Delegate function)
        {
            var call = new HostCall(name, function);

            lock (this)
            {
                Interlocked.Increment(ref m_NextId);
                call.Id = m_NextId;

                m_FunctionTableById.Add(call.Id, call);
                m_FuctionTableByName.Add(call.Name, call);
            }
        }

        public HostCall GetFunctionById(Int32 id)
        {
            return m_FunctionTableById[id];
        }

        public HostCall GetFunctionByName(String name)
        {
            return m_FuctionTableByName[name];
        }
    }
}