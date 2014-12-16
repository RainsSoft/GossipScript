using Druids.ScriptingEngine;
using Druids.ScriptingEngine.Compiler;
using Druids.ScriptingEngine.Host;

namespace Druids.ScriptingEngine.Host
{
    public class HostBridge
    {
        private readonly EnumDefineTable m_EnumDefineTable;
        private readonly EventBindingTable m_EventBindingTable;

        private readonly ExpressionEvaluator m_ExpressionEvaluator; // Note: This might go in host bridge?
        private readonly HostCallTable m_HostCallTable;

        private readonly TypeBindingTable m_TypeBindingTable;

        public HostBridge(GossipLexer gossipLexer)
        {
            m_HostCallTable = new HostCallTable();
            m_TypeBindingTable = new TypeBindingTable(gossipLexer);
            m_EnumDefineTable = new EnumDefineTable();
            m_ExpressionEvaluator = new ExpressionEvaluator(m_HostCallTable);
            m_EventBindingTable = new EventBindingTable();
        }

        public HostCallTable HostCallTable
        {
            get { return m_HostCallTable; }
        }

        public TypeBindingTable TypeBindingTable
        {
            get { return m_TypeBindingTable; }
        }

        public EnumDefineTable EnumDefineTable
        {
            get { return m_EnumDefineTable; }
        }

        public ExpressionEvaluator ExpressionEvaluator
        {
            get { return m_ExpressionEvaluator; }
        }

        public EventBindingTable EventBindingTable
        {
            get { return m_EventBindingTable; }
        }
    }
}